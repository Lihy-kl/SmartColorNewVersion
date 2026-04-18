using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_DataBase
{

    /// <summary>
    /// SQL Server数据库类，支持自动建库、建表、同步字段（增删改）、加备注、唯一约束、外键，并支持变更前自动备份和回退
    /// </summary>
    partial class SqlServer
    {
       
        private static readonly object RefreshLock = new object();

        /// <summary>
        /// 数据变更事件，参数为表名
        /// </summary>
        public static event Action<string> TableDataChanged;

       

        /// <summary>
        /// 获取数据库连接字符串（私有方法）
        /// </summary>
        /// <returns>SQL Server连接字符串</returns>
        private static string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = $"{My_ConPar.Database.Server},{My_ConPar.Database.Port}",
                InitialCatalog = My_ConPar.Database.DataBase,
                UserID = My_ConPar.Database.UserName,
                Password = My_ConPar.Database.Password,
                IntegratedSecurity = false,
                ConnectTimeout = 5
            };
            return builder.ConnectionString;
        }

        /// <summary>
        /// 获取SqlConnection对象（私有方法）
        /// </summary>
        /// <returns>SqlConnection实例</returns>
        private static SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

        /// <summary>
        /// 获取master库连接字符串（用于建库）（私有方法）
        /// </summary>
        /// <returns>SQL Server master库连接字符串</returns>
        private static string GetMasterConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = $"{My_ConPar.Database.Server},{My_ConPar.Database.Port}",
                InitialCatalog = "master",
                UserID = My_ConPar.Database.UserName,
                Password = My_ConPar.Database.Password,
                IntegratedSecurity = false,
                ConnectTimeout = 5
            };
            return builder.ConnectionString;
        }

        /// <summary>
        /// 确保数据库存在，不存在则自动创建
        /// </summary>
        public static void EnsureDatabaseExists()
        {
            string dbName = My_ConPar.Database.DataBase;
            string checkSql = $"SELECT COUNT(*) FROM sys.databases WHERE name = @dbName";
            string createSql = $"CREATE DATABASE [{dbName}]";
            using (var conn = new SqlConnection(GetMasterConnectionString()))
            using (var cmd = new SqlCommand(checkSql, conn))
            {
                cmd.Parameters.AddWithValue("@dbName", dbName);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                if (count == 0)
                {
                    using (var createCmd = new SqlCommand(createSql, conn))
                    {
                        createCmd.ExecuteNonQuery();
                        My_File.Logger.Info($"自动创建数据库：{dbName}");
                    }
                }
            }
        }

        /// <summary>
        /// 自动同步所有表结构（增删改字段、唯一约束、备注等），变更前自动备份，失败自动回退，成功自动清理备份
        /// </summary>
        public static void SyncAllTableStructure()
        {
            foreach (var table in TableDefinition.TableSchemas)
            {
                if (!TableExists(table.Key))
                {
                    string columnsDef = string.Join(", ", table.Value.Select(GetFieldSql));
                    string createSql = $"CREATE TABLE {table.Key} ({columnsDef})";
                    try
                    {
                        ExecuteNonQuery(createSql);
                        My_File.Logger.Info($"自动创建表：{table.Key}");
                        foreach (var field in table.Value)
                        {
                            if (!string.IsNullOrEmpty(field.Comment))
                                AddColumnComment(table.Key, field);
                            if (field.IsUnique && !field.IsPrimaryKey)
                                AddUniqueConstraint(table.Key, field.Name);
                            if (!string.IsNullOrEmpty(field.ForeignKeyTable))
                                AddForeignKey(table.Key, field.Name, field.ForeignKeyTable, field.ForeignKeyColumn);
                        }
                    }
                    catch (Exception ex)
                    {
                        My_File.Logger.Error($"自动创建表失败：{table.Key}", ex);
                        My_File.LocalTranslator.ShowMessage($"自动创建表失败：{table.Key}\n{ex.Message}", "数据库错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                }
                else
                {
                    var dbFields = GetDbFields(table.Key);

                    // 删除多余字段（变更前自动备份）
                    foreach (var dbField in dbFields.Keys)
                    {
                        if (!table.Value.Any(f => f.Name.Equals(dbField, StringComparison.OrdinalIgnoreCase)))
                        {
                            TryUpdateWithRollback(table.Key, () =>
                            {
                                // 1. 查找并删除默认约束
                                string findDefaultSql = @"
									DECLARE @dfname NVARCHAR(128)
									SELECT @dfname = d.name
									FROM sys.default_constraints d
									JOIN sys.columns c ON d.parent_object_id = c.object_id AND d.parent_column_id = c.column_id
									WHERE d.parent_object_id = OBJECT_ID(@table) AND c.name = @col
									IF @dfname IS NOT NULL
										EXEC('ALTER TABLE ' + @table + ' DROP CONSTRAINT ' + @dfname)";
                                ExecuteNonQuery(findDefaultSql,
                                    new SqlParameter("@table", table.Key),
                                    new SqlParameter("@col", dbField));

                                // 2. 删除字段
                                ExecuteNonQuery($"ALTER TABLE {table.Key} DROP COLUMN {dbField}");
                                My_File.Logger.Info($"自动删除多余字段：{table.Key}.{dbField}");
                            });
                        }
                    }

                    // 字段类型变更、补字段、备注、唯一约束、外键
                    foreach (var field in table.Value)
                    {
                        if (!dbFields.ContainsKey(field.Name))
                        {
                            // 补字段
                            try
                            {
                                ExecuteNonQuery($"ALTER TABLE {table.Key} ADD {GetFieldSql(field)}");
                                My_File.Logger.Info($"自动添加字段：{table.Key}.{field.Name}");
                                if (!string.IsNullOrEmpty(field.Comment))
                                    AddColumnComment(table.Key, field);
                                if (field.IsUnique && !field.IsPrimaryKey)
                                    AddUniqueConstraint(table.Key, field.Name);
                                if (!string.IsNullOrEmpty(field.ForeignKeyTable))
                                    AddForeignKey(table.Key, field.Name, field.ForeignKeyTable, field.ForeignKeyColumn);
                            }
                            catch (Exception ex)
                            {
                                My_File.Logger.Error($"自动添加字段失败：{table.Key}.{field.Name}", ex);
                                My_File.LocalTranslator.ShowMessage($"自动添加字段失败：{table.Key}.{field.Name}\n{ex.Message}", "数据库错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            string dbType = dbFields[field.Name].Replace(" ", "").ToUpper();
                            string codeType = field.Type.Replace(" ", "").ToUpper();
                            // 类型变更（变更前自动备份）
                            if (IsFieldTypeDifferent(dbFields[field.Name], field.Type))
                            {
                                TryUpdateWithRollback(table.Key, () =>
                                {
                                    // 1. 先ALTER COLUMN
                                    ExecuteNonQuery($"ALTER TABLE {table.Key} ALTER COLUMN {field.Name} {GetFieldTypeSql(field)}");
                                    My_File.Logger.Info($"自动变更字段类型：{table.Key}.{field.Name} {dbType} -> {codeType}");

                                    // 2. 处理默认值（先删后加）
                                    if (!string.IsNullOrEmpty(field.DefaultValue))
                                    {
                                        // 删除旧默认约束
                                        string dropDefaultSql = $@"
											DECLARE @dfname NVARCHAR(128)
											SELECT @dfname = d.name
											FROM sys.default_constraints d
											JOIN sys.columns c ON d.parent_object_id = c.object_id AND d.parent_column_id = c.column_id
											WHERE d.parent_object_id = OBJECT_ID(@table) AND c.name = @col
											IF @dfname IS NOT NULL
											EXEC('ALTER TABLE {table.Key} DROP CONSTRAINT ' + @dfname)";
                                        ExecuteNonQuery(dropDefaultSql,
                                            new SqlParameter("@table", table.Key),
                                            new SqlParameter("@col", field.Name));

                                        // 添加新默认约束
                                        string addDefaultSql = $"ALTER TABLE {table.Key} ADD CONSTRAINT DF_{table.Key}_{field.Name} DEFAULT {field.DefaultValue} FOR {field.Name}";
                                        ExecuteNonQuery(addDefaultSql);
                                    }
                                    else
                                    {
                                        // 如果没有默认值，删除旧默认约束
                                        string dropDefaultSql = $@"
											DECLARE @dfname NVARCHAR(128)
											SELECT @dfname = d.name
											FROM sys.default_constraints d
											JOIN sys.columns c ON d.parent_object_id = c.object_id AND d.parent_column_id = c.column_id
											WHERE d.parent_object_id = OBJECT_ID(@table) AND c.name = @col
											IF @dfname IS NOT NULL
											EXEC('ALTER TABLE {table.Key} DROP CONSTRAINT ' + @dfname)";
                                        ExecuteNonQuery(dropDefaultSql,
                                            new SqlParameter("@table", table.Key),
                                            new SqlParameter("@col", field.Name));
                                    }
                                });
                            }
                            // 备注、唯一约束、外键
                            if (!string.IsNullOrEmpty(field.Comment))
                                AddColumnComment(table.Key, field);
                            if (field.IsUnique && !field.IsPrimaryKey)
                                AddUniqueConstraint(table.Key, field.Name);
                            if (!string.IsNullOrEmpty(field.ForeignKeyTable))
                                AddForeignKey(table.Key, field.Name, field.ForeignKeyTable, field.ForeignKeyColumn);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 统一字段类型字符串格式，判断字段类型是否不同
        /// </summary>
        /// <param name="dbType">数据库字段类型</param>
        /// <param name="codeType">代码中字段类型</param>
        /// <returns>true: 一样  false: 不同</returns>
        private static bool IsFieldTypeDifferent(string dbType, string codeType)
        {
            // 去除空格，统一大小写
            dbType = dbType.Replace(" ", "").ToUpper();
            codeType = codeType.Replace(" ", "").ToUpper();
            return !dbType.Equals(codeType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 备份表，返回备份表名
        /// </summary>
        /// <param name="tableName">要备份的表名</param>
        /// <returns>备份表名</returns>
        private static string BackupTable(string tableName)
        {
            string bakTable = $"{tableName}_BAK_{DateTime.Now:yyyyMMddHHmmss}";
            string sql = $"SELECT * INTO {bakTable} FROM {tableName}";
            try
            {
                ExecuteNonQuery(sql);
                My_File.Logger.Info($"表[{tableName}]已备份为[{bakTable}]");
            }
            catch (Exception ex)
            {
                My_File.Logger.Error($"表[{tableName}]备份失败", ex);
            }
            return bakTable;
        }

        /// <summary>
        /// 删除指定表
        /// </summary>
        /// <param name="tableName">表名</param>
        private static void DropTable(string tableName)
        {
            ExecuteNonQuery($"DROP TABLE {tableName}");
            My_File.Logger.Info($"表[{tableName}]已删除");
        }

        /// <summary>
        /// 用备份表还原原表
        /// </summary>
        /// <param name="tableName">原表名</param>
        /// <param name="bakTable">备份表名</param>
        private static void RestoreTable(string tableName, string bakTable)
        {
            DropTable(tableName);
            ExecuteNonQuery($"EXEC sp_rename '{bakTable}', '{tableName}'");
            My_File.Logger.Info($"表[{tableName}]已从[{bakTable}]还原");
        }

        /// <summary>
        /// 高风险操作自动备份、失败自动回退、成功自动清理备份
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="updateAction">高风险操作委托</param>
        private static void TryUpdateWithRollback(string tableName, Action updateAction)
        {
            string bakTable = null;
            try
            {
                bakTable = BackupTable(tableName);
                updateAction();
                DropTable(bakTable); // 更新成功，删除备份
            }
            catch (Exception ex)
            {
                My_File.Logger.Error($"表[{tableName}]结构变更失败，尝试回退", ex);
                try
                {
                    if (!string.IsNullOrEmpty(bakTable))
                    {
                        RestoreTable(tableName, bakTable);
                        DropTable(bakTable); // 回退成功，删除备份
                        My_File.LocalTranslator.ShowMessage(
                            $"表[{tableName}]结构变更失败，已自动回退！", "数据库回退", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception rex)
                {
                    My_File.Logger.Error($"表[{tableName}]回退失败，请手动处理！", rex);
                    My_File.LocalTranslator.ShowMessage(
                        $"表[{tableName}]结构变更和回退均失败，请联系管理员！", "严重错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 获取数据库表的所有字段及类型
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>字段名与类型的字典</returns>
        private static Dictionary<string, string> GetDbFields(string tableName)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string sql = @"
				SELECT 
				COLUMN_NAME, 
				DATA_TYPE +
				CASE 
				WHEN DATA_TYPE IN ('decimal', 'numeric') THEN 
				'(' + CAST(NUMERIC_PRECISION AS VARCHAR) + ',' + CAST(NUMERIC_SCALE AS VARCHAR) + ')'
				WHEN DATA_TYPE IN ('char', 'varchar', 'nchar', 'nvarchar') THEN 
				'(' + 
                CASE 
                    WHEN CHARACTER_MAXIMUM_LENGTH = -1 THEN 'MAX' 
                    ELSE CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) 
                END + 
				')'
				WHEN DATA_TYPE IN ('datetime2', 'time') THEN
				'(' + CAST(DATETIME_PRECISION AS VARCHAR) + ')'
				ELSE ''
				END AS TYPE
				FROM INFORMATION_SCHEMA.COLUMNS
				WHERE TABLE_NAME = @TableName";
            var dt = ExecuteQuery(sql, new SqlParameter("@TableName", tableName));
            foreach (DataRow row in dt.Rows)
            {
                result[row["COLUMN_NAME"].ToString()] = row["TYPE"].ToString();
            }
            return result;
        }

        /// <summary>
        /// 生成字段SQL定义（含主键、自增、默认值、可空）
        /// </summary>
        /// <param name="field">字段结构体</param>
        /// <returns>字段SQL片段</returns>
        private static string GetFieldSql(TableField field)
        {
            var sb = new StringBuilder();
            sb.Append(field.Name).Append(" ").Append(field.Type);
            if (field.IsIdentity)
                sb.Append(" IDENTITY(1,1)");
            if (!field.IsNullable)
                sb.Append(" NOT NULL");
            else
                sb.Append(" NULL");
            if (!string.IsNullOrEmpty(field.DefaultValue))
                sb.Append(" DEFAULT ").Append(field.DefaultValue);
            if (field.IsPrimaryKey)
                sb.Append(" PRIMARY KEY");
            return sb.ToString();
        }

        /// <summary>
        /// 生成字段类型SQL（含可空、默认值）
        /// </summary>
        /// <param name="field">字段结构体</param>
        /// <returns>字段类型SQL片段</returns>
        private static string GetFieldTypeSql(TableField field)
        {
            var sb = new StringBuilder();
            sb.Append(field.Type);
            if (!field.IsNullable)
                sb.Append(" NOT NULL");
            else
                sb.Append(" NULL");

            return sb.ToString();
        }

        /// <summary>
        /// 添加字段备注（如已存在则自动覆盖）
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="field">字段结构体</param>
        private static void AddColumnComment(string table, TableField field)
        {
            try
            {
                string delSql = $@"
					IF EXISTS (SELECT 1 FROM sys.extended_properties 
					WHERE major_id=OBJECT_ID(@table) AND minor_id=
					(SELECT column_id FROM sys.columns WHERE object_id=OBJECT_ID(@table) AND name=@col) 
					AND name=N'MS_Description')
					BEGIN
					EXEC sys.sp_dropextendedproperty @name=N'MS_Description', 
					@level0type=N'SCHEMA',@level0name=N'dbo',
					@level1type=N'TABLE',@level1name=@table,
					@level2type=N'COLUMN',@level2name=@col
					END";
                ExecuteNonQuery(delSql,
                    new SqlParameter("@table", table),
                    new SqlParameter("@col", field.Name));

                string addSql = $@"
					EXEC sys.sp_addextendedproperty 
					@name=N'MS_Description', @value=@comment, 
					@level0type=N'SCHEMA',@level0name=N'dbo',
					@level1type=N'TABLE',@level1name=@table,
					@level2type=N'COLUMN',@level2name=@col";
                ExecuteNonQuery(addSql,
                    new SqlParameter("@comment", field.Comment),
                    new SqlParameter("@table", table),
                    new SqlParameter("@col", field.Name));
            }
            catch (Exception ex)
            {
                My_File.Logger.Error($"添加字段备注失败：{table}.{field.Name}", ex);
            }
        }

        /// <summary>
        /// 添加唯一约束（如已存在则跳过）
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="column">字段名</param>
        private static void AddUniqueConstraint(string table, string column)
        {
            try
            {
                string constraintName = $"UQ_{table}_{column}";
                string checkSql = $@"
					SELECT COUNT(*) FROM sys.indexes WHERE name = @constraintName AND object_id = OBJECT_ID(@table)";
                int count = (int)(ExecuteScalar(checkSql,
                    new SqlParameter("@constraintName", constraintName),
                    new SqlParameter("@table", table)) ?? 0);
                if (count == 0)
                {
                    string addSql = $"ALTER TABLE {table} ADD CONSTRAINT {constraintName} UNIQUE({column})";
                    ExecuteNonQuery(addSql);
                    My_File.Logger.Info($"添加唯一约束：{table}.{column}");
                }
            }
            catch (Exception ex)
            {
                My_File.Logger.Error($"添加唯一约束失败：{table}.{column}", ex);
            }
        }

        /// <summary>
        /// 添加外键约束（如已存在则跳过）
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="column">字段名</param>
        /// <param name="refTable">外键引用表名</param>
        /// <param name="refColumn">外键引用字段名</param>
        private static void AddForeignKey(string table, string column, string refTable, string refColumn)
        {
            try
            {
                if (string.IsNullOrEmpty(refTable) || string.IsNullOrEmpty(refColumn))
                    return;
                string constraintName = $"FK_{table}_{column}_{refTable}_{refColumn}";
                string checkSql = $@"
					SELECT COUNT(*) FROM sys.foreign_keys WHERE name = @constraintName AND parent_object_id = OBJECT_ID(@table)";
                int count = (int)(ExecuteScalar(checkSql,
                    new SqlParameter("@constraintName", constraintName),
                    new SqlParameter("@table", table)) ?? 0);
                if (count == 0)
                {
                    string addSql = $"ALTER TABLE {table} ADD CONSTRAINT {constraintName} FOREIGN KEY({column}) REFERENCES {refTable}({refColumn})";
                    ExecuteNonQuery(addSql);
                    My_File.Logger.Info($"添加外键约束：{table}.{column} -> {refTable}.{refColumn}");
                }
            }
            catch (Exception ex)
            {
                My_File.Logger.Error($"添加外键约束失败：{table}.{column} -> {refTable}.{refColumn}", ex);
            }
        }

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>存在返回true，否则返回false</returns>
        private static bool TableExists(string tableName)
        {
            string sql = @"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName";
            var count = (int)(ExecuteScalar(sql, new SqlParameter("@TableName", tableName)) ?? 0);
            return count > 0;
        }


        /// <summary>
        /// 表名到GetData方法的映射（全部用静态变量，类型安全，易维护）
        /// </summary>
        private static readonly Dictionary<string, Action> TableGetDataMap = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
        {
            // 基础数据
            { BOTTLE_DETAILS.TableName, BottleData.GetData },
            { ASSISTANT_DETAILS.TableName, AssistantData.GetData },
           
            { ENABLED_SET.TableName, EnabledData.GetData },
            { ABS_ENABLED_SET.TableName, ABSEnabledData.GetData },
            { FORMULA_GROUP.TableName, FormulaGradeData.GetData },
            { USER_TALE.TableName, UserData.GetData },
            { LIMIT_TABLE.TableName, LimitData.GetData },

            // 工艺/流程
            { BREWING_CODE.TableName, BrewData.GetData },
            { BREWING_PROCESS.TableName, BrewData.GetData },
            { DYEING_PROCESS.TableName, DyeingData.GetData },
            { HISTORY_DYEING_PROCESS.TableName, DyeingData.GetData },
            { DYEING_CODE.TableName, DyeingCodeData.GetData },
            { HISTORY_DYEING_CODE.TableName, DyeingCodeData.GetData },
            { ABS_PROCESS.TableName, ABSProcess.GetData },

          
        };

        /// <summary>
        /// 刷新表数据并触发事件
        /// </summary>
        /// <param name="tableName">表名</param>
        private static void RefreshTableData(string tableName)
        {

            lock (RefreshLock)
            {
                if (TableGetDataMap.TryGetValue(tableName, out var getData))
                {
                    try
                    {
                        getData?.Invoke();
                        TableDataChanged?.Invoke(tableName);

                    }
                    catch (Exception ex)
                    {
                        My_File.Logger.Error($"异步刷新表[{tableName}]数据异常", ex);
                    }
                }
                


            }
        }

     

        /// <summary>
        /// 克隆参数数组，避免 SqlParameter 被多次复用
        /// </summary>
        private static SqlParameter[] CloneParameters(SqlParameter[] parameters)
        {
            if (parameters == null) return null;
            return parameters.Select(p =>
            {
                var clone = new SqlParameter(p.ParameterName, p.SqlDbType)
                {
                    Value = p.Value,
                    Direction = p.Direction,
                    Size = p.Size,
                    Precision = p.Precision,
                    Scale = p.Scale,
                    IsNullable = p.IsNullable,
                    SourceColumn = p.SourceColumn,
                    SourceVersion = p.SourceVersion
                };
                return clone;
            }).ToArray();
        }

        /// <summary>
        /// 执行增、删、改操作，返回受影响的行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            const int maxRetry = 3;
            int retry = 0;
            // --- 新增：检测空的 IN 子句 ---
            if (!string.IsNullOrEmpty(sql) &&
                (sql.Contains("IN ()") || sql.Contains("IN  ()") || sql.Contains("IN('')") || sql.Contains("IN ('')")))
            {
                //My_File.Logger.Error($"检测到空的 IN 子句，SQL 未执行：{sql}");
                return 0;
            }
            // --- END ---

            while (true)
            {
                try
                {
                    string paramStr = parameters != null && parameters.Length > 0
                        ? string.Join(", ", parameters.Select(p => $"{p.ParameterName}={FormatParamValue(p.Value)}"))
                        : "无参数";
                    My_File.Logger.Info($"ExecuteNonQuery SQL: {sql} | 参数: {paramStr}");

                    int result;
                    using (var conn = GetConnection())
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(CloneParameters(parameters));
                        conn.Open();
                        result = cmd.ExecuteNonQuery();
                    }

                    // --- 自动识别增删改并触发事件 ---
                    string sqlTrim = sql.TrimStart();
                    string sqlType = sqlTrim.Substring(0, Math.Min(10, sqlTrim.Length)).ToUpper();
                    string tableName = null;

                    if (sqlType.StartsWith("INSERT"))
                    {
                        int idx = sqlTrim.IndexOf("INTO", StringComparison.OrdinalIgnoreCase);
                        if (idx >= 0)
                        {
                            var rest = sqlTrim.Substring(idx + 4).Trim();
                            tableName = rest.Split(' ', '[', ']','(', ')', '\r', '\n').FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
                        }
                    }
                    else if (sqlType.StartsWith("UPDATE"))
                    {
                        var rest = sqlTrim.Substring(6).Trim();
                        tableName = rest.Split(' ', '[', ']', '(', ')', '\r', '\n').FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
                    }
                    else if (sqlType.StartsWith("DELETE"))
                    {
                        int idx = sqlTrim.IndexOf("FROM", StringComparison.OrdinalIgnoreCase);
                        if (idx >= 0)
                        {
                            var rest = sqlTrim.Substring(idx + 4).Trim();
                            tableName = rest.Split(' ', '[', ']', '(', ')', '\r', '\n').FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
                        }
                    }
                    else if (sqlType.StartsWith("TRUNCATE"))
                    {
                        int idx = sqlTrim.IndexOf("TABLE", StringComparison.OrdinalIgnoreCase);
                        if (idx >= 0)
                        {
                            var rest = sqlTrim.Substring(idx + 5).Trim();
                            tableName = rest.Split(' ', '[', ']', '(', ')', '\r', '\n').FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
                        }
                    }

                    if (!string.IsNullOrEmpty(tableName))
                    {
                      
                        RefreshTableData(tableName);
                    }
                    // --- END ---

                    return result;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205 && retry < maxRetry)
                    {
                        My_File.Logger.Info($"SQL死锁，自动重试第{retry + 1}次，SQL：{sql}");
                        System.Threading.Thread.Sleep(100);
                        retry++;
                        continue;
                    }
                    My_File.Logger.Error("ExecuteNonQuery异常，SQL：" + sql, ex);
                    return 0;
                }
                catch (Exception ex)
                {
                    My_File.Logger.Error("ExecuteNonQuery异常，SQL：" + sql, ex);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 查询数据，返回DataTable（私有方法）
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns>查询结果DataTable</returns>
        public static DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
        {
            try
            {
                string paramStr = parameters != null && parameters.Length > 0
                    ? string.Join(", ", parameters.Select(p => $"{p.ParameterName}={FormatParamValue(p.Value)}"))
                    : "无参数";
                My_File.Logger.Info($"ExecuteQuery SQL: {sql} | 参数: {paramStr}");

                using (var conn = GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(CloneParameters(parameters));
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                My_File.Logger.Error("ExecuteQuery异常，SQL：" + sql, ex);
                throw;
            }
        }
        /// <summary>
        /// 查询单个值（如计数、聚合等）（私有方法）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns>查询结果的首行首列对象</returns>
        public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            try
            {
                string paramStr = parameters != null && parameters.Length > 0
                    ? string.Join(", ", parameters.Select(p => $"{p.ParameterName}={FormatParamValue(p.Value)}"))
                    : "无参数";
                My_File.Logger.Info($"ExecuteScalar SQL: {sql} | 参数: {paramStr}");

                using (var conn = GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(CloneParameters(parameters));
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                My_File.Logger.Error("ExecuteScalar异常，SQL：" + sql, ex);
                throw;
            }
        }
        /// <summary>
        /// 通用插入方法，根据表名和字段数据插入一条记录
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="data">字段名与值的字典</param>
        /// <returns>受影响的行数</returns>
        public static int Insert(string tableName, Dictionary<string, object> data)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("表名不能为空", nameof(tableName));
            if (data == null || data.Count == 0)
                throw new ArgumentException("插入数据不能为空", nameof(data));

            var converted = data.ToDictionary(
                kv => kv.Key,
                kv => ConvertValue(tableName, kv.Key, kv.Value)
            );

            var columns = string.Join(",", converted.Keys);
            var parameters = string.Join(",", converted.Keys.Select(k => "@" + k));
            string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";
            var sqlParams = converted.Select(kv => new SqlParameter("@" + kv.Key, kv.Value ?? DBNull.Value)).ToArray();

            return ExecuteNonQuery(sql, sqlParams);
        }

        /// <summary>
        /// 通用批量插入方法，根据表名和字段数据列表插入多条记录
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dataList">字段名和值的字典列表</param>
        /// <returns>受影响的行数</returns>
        public static int Insert(string tableName, List<Dictionary<string, object>> dataList)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("表名不能为空", nameof(tableName));

            if (dataList == null || dataList.Count == 0)
                throw new ArgumentException("插入数据不能为空", nameof(dataList));

            // 获取所有字段名（假设所有字典的Key都一致）
            var fields = dataList[0].Keys.ToList();
            var fieldList = string.Join(", ", fields.Select(f => $"[{f}]"));

            // 构建参数化SQL和参数列表
            var valueRows = new List<string>();
            var parameters = new List<SqlParameter>();
            int rowIndex = 0;
            foreach (var data in dataList)
            {
                var valueNames = new List<string>();
                foreach (var field in fields)
                {
                    string paramName = $"@{field}_{rowIndex}";
                    valueNames.Add(paramName);
                    parameters.Add(new SqlParameter(paramName, ConvertValue(tableName, field, data[field])));
                }
                valueRows.Add("(" + string.Join(", ", valueNames) + ")");
                rowIndex++;
            }

            string sql = $"INSERT INTO [{tableName}] ({fieldList}) VALUES {string.Join(", ", valueRows)}";

            return ExecuteNonQuery(sql, parameters.ToArray());
        }

        /// <summary>
        /// 通用更新方法，根据表名、字段数据和条件更新记录
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="data">字段名与值的字典</param>
        /// <param name="where">更新条件（如 "Id=@Id"）</param>
        /// <param name="whereParams">条件参数</param>
        /// <returns>受影响的行数</returns>
        public static int Update(string tableName, Dictionary<string, object> data, string where, params SqlParameter[] whereParams)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("表名不能为空", nameof(tableName));
            if (data == null || data.Count == 0)
                throw new ArgumentException("更新数据不能为空", nameof(data));
            if (string.IsNullOrWhiteSpace(where))
                throw new ArgumentException("更新条件不能为空", nameof(where));

            // 字段类型转换
            var converted = data.ToDictionary(
                kv => kv.Key,
                kv => ConvertValue(tableName, kv.Key, kv.Value)
            );

            var setClause = string.Join(",", converted.Keys.Select(k => $"{k}=@{k}"));
            string sql = $"UPDATE {tableName} SET {setClause} WHERE {where}";
            var sqlParams = converted.Select(kv => new SqlParameter("@" + kv.Key, kv.Value ?? DBNull.Value)).ToList();
            if (whereParams != null) sqlParams.AddRange(whereParams);
            int i = ExecuteNonQuery(sql, sqlParams.ToArray());

            return i;
        }

        /// <summary>
        /// 通用删除方法，根据表名和条件删除记录
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="where">删除条件（如 "Id=@Id"）</param>
        /// <param name="whereParams">条件参数</param>
        /// <returns>受影响的行数</returns>
        public static int Delete(string tableName, string where, params SqlParameter[] whereParams)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("表名不能为空", nameof(tableName));
            if (string.IsNullOrWhiteSpace(where))
                throw new ArgumentException("删除条件不能为空", nameof(where));

            string sql = $"DELETE FROM {tableName} WHERE {where}";

            return ExecuteNonQuery(sql, whereParams);
        }

        /// <summary>
        /// 清空表数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>受影响的行数</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int DeleteAll(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("表名不能为空", nameof(tableName));
            string sql = $"TRUNCATE TABLE {tableName}";
            return ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 通用查询方法，根据表名和条件查询数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="where">查询条件（可选）</param>
        /// <param name="whereParams">条件参数</param>
        /// <returns>查询结果DataTable</returns>
        public static DataTable Select(string tableName, string where = null, params SqlParameter[] whereParams)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("表名不能为空", nameof(tableName));

            string sql = $"SELECT * FROM {tableName}";
            if (!string.IsNullOrWhiteSpace(where))
                sql += $" WHERE {where}";

            return ExecuteQuery(sql, whereParams);
        }

        /// <summary>
        /// 通用查询方法（重载），可指定字段和排序方式
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fields">要查询的字段集合</param>
        /// <param name="where">查询条件（可选）</param>
        /// <param name="orderBy">排序字段（可选）</param>
        /// <param name="ascending">是否升序（默认true）</param>
        /// <param name="whereParams">条件参数</param>
        /// <returns>查询结果DataTable</returns>
        public static DataTable Select(
            string tableName,
            IEnumerable<string> fields,
            string where = null,
            string orderBy = null,
            bool ascending = true,
            params SqlParameter[] whereParams)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("表名不能为空", nameof(tableName));
            if (fields == null || !fields.Any())
                fields = new[] { "*" };

            string fieldList = fields.Any(f => f == "*") ? "*" : string.Join(", ", fields.Select(f => $"[{f}]"));
            string sql = $"SELECT {fieldList} FROM {tableName}";
            if (!string.IsNullOrWhiteSpace(where))
                sql += $" WHERE {where}";
            if (!string.IsNullOrWhiteSpace(orderBy))
                sql += $" ORDER BY [{orderBy}] {(ascending ? "ASC" : "DESC")}";

            return ExecuteQuery(sql, whereParams);
        }

        /// <summary>
        /// 测试数据库服务器和目标数据库连接是否正常，不存在则自动创建
        /// </summary>
        /// <returns>连接成功返回 true，否则返回 false</returns>
        public static bool TestConnection()
        {
            try
            {
                // 1. 测试服务器（master库）连接
                var masterBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = $"{My_ConPar.Database.Server},{My_ConPar.Database.Port}",
                    InitialCatalog = "master",
                    UserID = My_ConPar.Database.UserName,
                    Password = My_ConPar.Database.Password,
                    IntegratedSecurity = false,
                    ConnectTimeout = 5
                };
                using (var conn = new SqlConnection(masterBuilder.ConnectionString))
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                        throw new Exception("无法连接数据库服务器");
                }

                // 2. 确保数据库存在
                EnsureDatabaseExists();

                // 3. 测试目标数据库连接
                using (var conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    My_File.Logger.Info("数据库连接测试成功");
                    return conn.State == ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                My_File.Logger.Error("数据库连接测试失败", ex);
                return false;
            }
        }

        private static object ConvertValue(string tableName, string fieldName, object value)
        {
            if (value == null) return DBNull.Value;
            var schema = TableDefinition.TableSchemas.ContainsKey(tableName) ? TableDefinition.TableSchemas[tableName] : null;
            if (schema == null) return value;
            var field = schema.FirstOrDefault(f => f.Name == fieldName);
            if (string.IsNullOrEmpty(field.Name)) return value;

            string type = field.Type.ToUpper();
            string str = value.ToString().Trim();
            if (string.IsNullOrEmpty(str)) return DBNull.Value;

            // 数值类型
            if (type.StartsWith("BIT"))
            {
                if (str == "1" || str.ToLower() == "true") return true;
                if (str == "0" || str.ToLower() == "false") return false;
                return DBNull.Value;
            }
            if (type.StartsWith("TINYINT"))
            {
                if (byte.TryParse(str, out var b)) return b;
                return DBNull.Value;
            }
            if (type.StartsWith("SMALLINT"))
            {
                if (short.TryParse(str, out var s)) return s;
                return DBNull.Value;
            }
            if (type.StartsWith("INT"))
            {
                if (int.TryParse(str, out var i)) return i;
                return DBNull.Value;
            }
            if (type.StartsWith("BIGINT"))
            {
                if (long.TryParse(str, out var l)) return l;
                return DBNull.Value;
            }
            if (type.StartsWith("DECIMAL") || type.StartsWith("NUMERIC"))
            {
                if (decimal.TryParse(str, out var d)) return d;
                return DBNull.Value;
            }
            if (type.StartsWith("FLOAT"))
            {
                if (double.TryParse(str, out var f)) return f;
                return DBNull.Value;
            }
            if (type.StartsWith("REAL"))
            {
                if (float.TryParse(str, out var r)) return r;
                return DBNull.Value;
            }
            if (type.StartsWith("SMALLMONEY") || type.StartsWith("MONEY"))
            {
                if (decimal.TryParse(str, out var m)) return m;
                return DBNull.Value;
            }

            // 字符串类型
            if (type.StartsWith("CHAR") || type.StartsWith("VARCHAR") || type.StartsWith("TEXT"))
            {
                return str;
            }
            if (type.StartsWith("NCHAR") || type.StartsWith("NVARCHAR") || type.StartsWith("NTEXT"))
            {
                return str;
            }

            // 日期和时间类型
            if (type.StartsWith("DATE") || type.StartsWith("DATETIME") || type.StartsWith("DATETIME2") || type.StartsWith("SMALLDATETIME"))
            {
                if (DateTime.TryParse(str, out var dt)) return dt;
                return DBNull.Value;
            }
            if (type.StartsWith("TIME"))
            {
                if (TimeSpan.TryParse(str, out var ts)) return ts;
                if (DateTime.TryParse(str, out var dt)) return dt.TimeOfDay;
                return DBNull.Value;
            }
            if (type.StartsWith("DATETIMEOFFSET"))
            {
                if (DateTimeOffset.TryParse(str, out var dto)) return dto;
                return DBNull.Value;
            }

            // 二进制类型
            if (type.StartsWith("BINARY") || type.StartsWith("VARBINARY"))
            {
                if (value is byte[] bytes) return bytes;
                try
                {
                    return Convert.FromBase64String(str);
                }
                catch
                {
                    return DBNull.Value;
                }
            }
            if (type.StartsWith("IMAGE"))
            {
                if (value is byte[] bytes) return bytes;
                try
                {
                    return Convert.FromBase64String(str);
                }
                catch
                {
                    return DBNull.Value;
                }
            }

            // 其它类型
            if (type.StartsWith("UNIQUEIDENTIFIER"))
            {
                if (Guid.TryParse(str, out var guid)) return guid;
                return DBNull.Value;
            }
            if (type.StartsWith("XML"))
            {
                return str;
            }
            if (type.StartsWith("SQL_VARIANT"))
            {
                return value;
            }
            // cursor, table, hierarchyid, geometry, geography 通常不直接映射
            // 可根据实际业务需求扩展

            // 未知类型直接返回原值
            return value;
        }

        /// <summary>
        /// 格式化参数值用于日志
        /// </summary>
        private static string FormatParamValue(object value)
        {
            if (value == null || value == DBNull.Value) return "NULL";
            if (value is string s) return $"'{s.Replace("'", "''")}'";
            if (value is DateTime dt) return $"'{dt:yyyy-MM-dd HH:mm:ss.fff}'";
            if (value is bool b) return b ? "1" : "0";
            if (value is byte[] bytes) return "[BINARY]";
            return value.ToString();
        }

        public static int InsertAndGetIdentity(string tableName, Dictionary<string, object> data)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("表名不能为空", nameof(tableName));
            if (data == null || data.Count == 0)
                throw new ArgumentException("插入数据不能为空", nameof(data));

            var converted = data.ToDictionary(
                kv => kv.Key,
                kv => ConvertValue(tableName, kv.Key, kv.Value)
            );

            var columns = string.Join(",", converted.Keys);
            var parameters = string.Join(",", converted.Keys.Select(k => "@" + k));
            string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters}); SELECT SCOPE_IDENTITY();";
            var sqlParams = converted.Select(kv => new SqlParameter("@" + kv.Key, kv.Value ?? DBNull.Value)).ToArray();

            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddRange(sqlParams);
                conn.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }
    }
}