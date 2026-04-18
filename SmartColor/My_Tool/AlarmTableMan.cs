using SmartColor.My_DataBase;
using SmartColor.My_File;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SmartColor.My_Tool
{
    /// <summary>
    /// 报警表管理类，负责插入报警数据并自动清理超出保留天数的数据。
    /// 支持字段合法性校验、异常日志记录、事件通知等功能。
    /// </summary>
    internal static  class AlarmTableMan
    {
        /// <summary>
        /// 报警表数据变更事件（静态），用于通知外部有数据插入等操作。
        /// </summary>
        public static  event EventHandler<AlarmTableChangedEventArgs> AlarmTableChanged;

        /// <summary>
        /// 报警数据保留天数（可配置，默认15天）。
        /// 超过该天数的数据会被自动清理。
        /// </summary>
        public static  int RetentionDays { get; set; } = 15;

        /// <summary>
        /// 向报警表插入一条数据，并自动清理超出保留天数的数据。
        /// 插入前会自动补充日期和时间字段，并校验字段合法性。
        /// </summary>
        /// <param name="data">待插入的数据字典，键为字段名，值为字段值。</param>
        public static  void Insert(Dictionary<string, object> data)
        {
            try
            {
                // 自动补充日期和时间字段，确保每条报警数据都有时间戳
                data[My_DataBase.ALARM_TABLE.MyDate] = DateTime.Today;
                data[My_DataBase.ALARM_TABLE.MyTime] = DateTime.Now.ToString("HH:mm:ss");

                // 插入前先清理过期数据，保证表数据不会无限增长
                ClearOldData();

                // 获取报警表的合法字段列表，过滤掉不合法字段
                var validFields = TableDefinition.TableSchemas[My_DataBase.ALARM_TABLE.TableName];
                var validNames = new HashSet<string>(validFields.Select(f => f.Name), StringComparer.OrdinalIgnoreCase);

                // 构造仅包含合法字段的数据字典
                var insertData = new Dictionary<string, object>();
                foreach (var kv in data)
                {
                    if (validNames.Contains(kv.Key))
                        insertData[kv.Key] = kv.Value;
                }

                // 如果没有合法字段则抛出异常
                if (insertData.Count == 0)
                    throw new ArgumentException("没有有效字段可插入");

                // 执行数据库插入操作
                SqlServer.Insert(My_DataBase.ALARM_TABLE.TableName, insertData);

                // 记录插入日志，便于后期追踪
                Logger.Info($"AlarmTableMan: 插入报警数据 {string.Join(", ", insertData.Select(kv => kv.Key + "=" + kv.Value))}");

                // 触发数据变更事件，通知外部有新数据插入
                AlarmTableChanged?.Invoke(null, new AlarmTableChangedEventArgs("Insert", insertData));
            }
            catch (Exception ex)
            {
                // 捕获异常并记录错误日志，避免异常导致程序崩溃
                Logger.Error("AlarmTableMan: 插入报警数据异常", ex);
            }
        }

        /// <summary>
        /// 插入单个字段的报警数据（自动补充日期和时间）。
        /// </summary>
        /// <param name="header">字段名</param>
        /// <param name="value">字段值</param>
        public static  void Insert(string header, object value)
        {
            Insert(new Dictionary<string, object> { [header] = value });
        }

        /// <summary>
        /// 清理报警表中超出保留天数的数据。
        /// 删除 MyDate 字段早于当前日期减去保留天数的数据。
        /// </summary>
        public static  void ClearOldData()
        {
            try
            {
                // 构造SQL条件，删除超期数据
                string where = $"{My_DataBase.ALARM_TABLE.MyDate} < @dateLimit";
                var dateLimit = DateTime.Today.AddDays(-RetentionDays);

                // 执行删除操作，返回删除条数
                int deleted = SqlServer.Delete(
                    My_DataBase.ALARM_TABLE.TableName,
                    where,
                    new System.Data.SqlClient.SqlParameter("@dateLimit", dateLimit)
                );

                // 记录清理日志
                Logger.Info($"AlarmTableMan: 清理过期报警数据，已删除 {deleted} 条。");
            }
            catch (Exception ex)
            {
                // 捕获异常并记录错误日志
                Logger.Error("AlarmTableMan: 清理过期报警数据异常", ex);
            }
        }

        /// <summary>
        /// 报警表变更事件参数类，包含变更类型和相关数据。
        /// </summary>
        public class AlarmTableChangedEventArgs : EventArgs
        {
            /// <summary>
            /// 变更类型（如"Insert"）
            /// </summary>
            public string ChangeType { get; }

            /// <summary>
            /// 变更涉及的数据字典
            /// </summary>
            public Dictionary<string, object> Data { get; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="changeType">变更类型</param>
            /// <param name="data">相关数据</param>
            public AlarmTableChangedEventArgs(string changeType, Dictionary<string, object> data)
            {
                ChangeType = changeType;
                Data = data;
            }
        }
    }
}