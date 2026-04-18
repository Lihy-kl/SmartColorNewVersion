using SmartColor.My_File;
using SmartColor.My_Form.BasicData;
using SmartColor.My_Form.HistoricalData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_DataBase
{
    /// <summary>
	/// 字段结构体，描述表字段的所有元数据
	/// </summary>
	public struct TableField
    {
        /// <summary>字段名</summary>
        public string Name;

        /// <summary>字段类型</summary>
        public string Type;

        /// <summary>是否主键 </summary>
        public bool IsPrimaryKey;

        /// <summary>是否自增</summary>
        public bool IsIdentity;

        /// <summary>是否可空</summary>
        public bool IsNullable;

        /// <summary> 默认值</summary>
        public string DefaultValue;

        /// <summary>字段备注</summary>
        public string Comment;

        /// <summary>是否唯一约束</summary>
        public bool IsUnique;

        /// <summary>外键表 </summary>
        public string ForeignKeyTable;

        /// <summary>外键字段 </summary>
        public string ForeignKeyColumn;
    }

    internal class TableDefinition
    {
        /// <summary>
		/// 数据库表结构定义
		/// </summary>
		public static readonly Dictionary<string, List<TableField>> TableSchemas = new Dictionary<string, List<TableField>>
        {


            [ABS_DETAILS.TableName] = new List<TableField>{
                    new TableField { Name =DROP_DETAILS.HeadID, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "表头序号", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "杯号", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.Finish, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "是否完成", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.StartTime, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开始时间", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.FinishTime, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "结束时间", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.Cooperate, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "操作", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.StepNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "步号", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.TechnologyName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "工艺", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.StirringRate, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "搅拌速度", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.StirringTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "搅拌时间", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.DrainTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "排液时间", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.ParallelizingDishTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "排比色皿时间", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.PumpingTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "抽液时间", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.StartingWavelength, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开始波长", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.EndWavelength, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "结束波长", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.WavelengthInterval, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "波长间隔", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.Dosage, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "用量", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.Code, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "吸光度测量工艺名称", IsUnique = false },
                    new TableField { Name = ABS_DETAILS.GUID, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "GUID", IsUnique = false }},

            [ABS_DROP_DETAILS.TableName] = new List<TableField>{
                    new TableField { Name =ABS_DROP_DETAILS.HeadID, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "表头序号", IsUnique = false },
                    new TableField { Name = ABS_DROP_DETAILS.BatchName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "批次号", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "杯号", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方代码", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "版本号", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.IndexNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((1))", Comment = "序号", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂代码", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.FormulaDosage, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "配方用量", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'%')", Comment = "计算单位", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "瓶号", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "设定浓度", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际浓度", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂名称", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.ObjectDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "目标滴液重", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.RealDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际滴液重", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.BottleSelection, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "手动选瓶", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.MinWeight, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "液量低", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.Finish, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "完成", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.ObjectPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "目标粉重", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.RealPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际粉重", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.IsShow, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "是否显示", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.IsDrop, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((1))", Comment = "是否已经滴液", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.BrewingData, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开料日期", IsUnique = false },
                    new TableField { Name =ABS_DROP_DETAILS.NeedPulse, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "需加脉冲", IsUnique = false }},

            [ABS_DROP_HEAD.TableName] = new List<TableField>{
                    new TableField { Name =DROP_HEAD.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                        DefaultValue = null, Comment = "序号", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.BatchName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "批次号", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "杯号", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方代码", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "版本", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.State, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'尚未滴液')", Comment = "状态", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.FormulaName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "配方名称", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.ClothType, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "布种/基材", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.Customer, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "客户", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.AddWaterChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "是否加水", IsUnique = false },

                    new TableField { Name = ABS_DROP_HEAD.ClothWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "布重", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.BathRatio, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "浴比", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.TotalWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "总浴量", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.Operator, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "操作员", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.CupCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "染杯代码", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.CreateTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "存档时间", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.ObjectAddWaterWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((0.0))", Comment = "目标加水重量", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.RealAddWaterWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((0.0))", Comment = "实际加水重量", IsUnique = false },

                    new TableField { Name = ABS_DROP_HEAD.AddWaterFinish, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "加水完成", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.CupFinish, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "杯完成", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.DyeingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染固色代码", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.Step, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "步号", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.Non_AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "非脱水水比", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "脱水水比", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.HandleBathRatio, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "后处理浴比", IsUnique = false },

                    new TableField { Name = ABS_DROP_HEAD.DescribeChar, Type = "NVARCHAR(100)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "描述", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.FinishTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "完成时间", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.StartTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开始时间", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.Stage, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "类型", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.DescribeChar_EN, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "描述英文", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.HandleBRList, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染色后处理浴比集合", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.Abs, Type = "NVARCHAR(1000)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                         DefaultValue = null, Comment = "吸光度数据", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.Type, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                         DefaultValue = "((0))", Comment = "类型", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.StartWave, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                         DefaultValue = "((320))", Comment = "开始波长", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.EndWave, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                         DefaultValue = "((700))", Comment = "结束波长", IsUnique = false },
                    new TableField { Name = ABS_DROP_HEAD.IntWave, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                         DefaultValue = "((5))", Comment = "波长间隔", IsUnique = false }},

            [ABS_FORMULA_DETAILS.TableName] = new List<TableField>{
                    new TableField { Name = ABS_FORMULA_DETAILS.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方代码", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "版本号", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.IndexNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((1))", Comment = "序号", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂代码", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.FormulaDosage, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "配方用量", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'%')", Comment = "计算单位", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "瓶号", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "设定浓度", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际浓度", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂名称", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.ObjectDropWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "目标滴液重", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.RealDropWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际滴液量", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.BottleSelection, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "手动选瓶", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.ObjectPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "目标粉重", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.RealPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际粉重", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.IsShow, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "是否显示", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_DETAILS.BrewingData, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开料日期", IsUnique = false }},

            [ABS_FORMULA_GROUP.TableName] = new List<TableField>{
                    new TableField { Name = ABS_FORMULA_GROUP.Id, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                        DefaultValue = null,Comment = "ID", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_GROUP.GroupName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方组合名称", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_GROUP.Node, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "序号", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_GROUP.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方代码", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_GROUP.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方名称", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_GROUP.CreateTime, Type = "DATETIME", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "创建时间", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_GROUP.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "计算单位", IsUnique = false }},

            [ABS_FORMULA_HEAD.TableName] = new List<TableField>{
                    new TableField { Name = ABS_FORMULA_HEAD.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                        DefaultValue = null, Comment = "序号", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方代码", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "版本", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.State, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'尚未滴液')", Comment = "状态", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.FormulaName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "配方名称", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.ClothType, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "布种/基材", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.Customer, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "客户", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.AddWaterChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "是否加水", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.CompoundBoardChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "是否复板", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.ClothWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "布重", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.BathRatio, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "浴比", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.TotalWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "总浴量", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.Operator, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "操作员", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.CupCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "染杯代码", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.CreateTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "存档时间", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.ObjectAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "目标加水重量", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.RealAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际加水重量", IsUnique = false },

                    new TableField { Name = ABS_FORMULA_HEAD.DyeingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染固色代码", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "杯号", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.Non_AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "非脱水水比", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "脱水水比", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.HandleBathRatio, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "后处理浴比", IsUnique = false },

                    new TableField { Name = ABS_FORMULA_HEAD.Stage, Type="NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "类型", IsUnique = false },
                    new TableField { Name = ABS_FORMULA_HEAD.HandleBRList, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染色后处理浴比集合", IsUnique = false },
                    new TableField { Name =FORMULA_HEAD.Note1, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注1", IsUnique = false },
                    new TableField { Name =FORMULA_HEAD.Note2, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注2", IsUnique = false },
                    new TableField { Name =FORMULA_HEAD.Note3, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注3", IsUnique = false }},

            [ABS_HISTORY_DETAILS.TableName] = new List<TableField>{
                    new TableField { Name = ABS_HISTORY_DETAILS.BatchName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "批次号", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "杯号", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方代码", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((1))", Comment = "版本号", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.IndexNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "序号", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂代码", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.FormulaDosage, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "配方用量", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'%')", Comment = "计算单位", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "瓶号", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "设定浓度", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际浓度", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂名称", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.ObjectDropWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "目标滴液量", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.RealDropWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际滴液量", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.BottleSelection, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "手动选瓶", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.ObjectPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "目标粉重", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.RealPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际粉重", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.IsDrop, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((1))", Comment = "是否滴液", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_DETAILS.BrewingData, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开料日期", IsUnique = false }},

            [ABS_HISTORY_HEAD.TableName] = new List<TableField>{
                    new TableField { Name = ABS_HISTORY_HEAD.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                        DefaultValue = null, Comment = "ID", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.BatchName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "批次号", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "杯号", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方代码", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "版本", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.State, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'尚未滴液')", Comment = "状态", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.FormulaName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方名称", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.ClothType, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "布重/基材", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.Customer, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "客户", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.AddWaterChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "是否加水", IsUnique = false },

                    new TableField { Name = ABS_HISTORY_HEAD.ClothWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "布重", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.BathRatio, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "浴比", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.TotalWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "总浴量", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.Operator, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "操作员", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.CupCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染杯代码", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.CreateTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "存档时间", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.ObjectAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "目标加水重量", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.RealAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际加水重量", IsUnique = false },

                    new TableField { Name = ABS_HISTORY_HEAD.FinishTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "完成时间", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.DescribeChar, Type = "NVARCHAR(100)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "描述", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.DyeingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染固色代码", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.Step, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "步号", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.Non_AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "非脱水水比", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "脱水水比", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.HandleBathRatio, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "后处理浴比", IsUnique = false },

                    new TableField { Name = ABS_HISTORY_HEAD.StartTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开始时间", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.ProcessData, Type = "IMAGE", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "温度曲线", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.MarkStep, Type = "NVARCHAR(500)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "温度描点", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.Stage, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "类型", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.Spectrum, Type = "NVARCHAR(500)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "光谱数据", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.DescribeChar_EN, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "描述英文", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.HandleBRList, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染色后处理浴比集合", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.Abs, Type = "NVARCHAR(1000)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                         DefaultValue = null, Comment = "吸光度数据", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.Stand, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                         DefaultValue = "((0))", Comment = "标样", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.Type, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                         DefaultValue = "((0))", Comment = "类型", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.StartWave, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                         DefaultValue = "((320))", Comment = "开始波长", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.EndWave, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                         DefaultValue = "((700))", Comment = "结束波长", IsUnique = false },
                    new TableField { Name = ABS_HISTORY_HEAD.IntWave, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                         DefaultValue = "((5))", Comment = "波长间隔", IsUnique = false }},

            [ABS_PROCESS.TableName] = new List<TableField>{
                    new TableField { Name =ABS_PROCESS.StepNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "步号", IsUnique = false },
                    new TableField { Name =ABS_PROCESS.TechnologyName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "工艺", IsUnique = false },
                    new TableField { Name =ABS_PROCESS.StirringRate, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "搅拌速率", IsUnique = false },
                    new TableField { Name =ABS_PROCESS.StirringTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "搅拌时间", IsUnique = false },
                    new TableField { Name =ABS_PROCESS.DrainTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "排液时间", IsUnique = false },
                    new TableField { Name =ABS_PROCESS.ParallelizingDishTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "排比色皿时间", IsUnique = false },
                    new TableField { Name =ABS_PROCESS.PumpingTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "抽液时间", IsUnique = false },
                    new TableField { Name =ABS_PROCESS.StartingWavelength, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开始波长", IsUnique = false },
                    new TableField { Name =ABS_PROCESS.EndWavelength, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "结束波长", IsUnique = false },
                    new TableField { Name =ABS_PROCESS.WavelengthInterval, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "波长间隔", IsUnique = false },
                    new TableField { Name =ABS_PROCESS.Dosage, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "用量", IsUnique = false },
                    new TableField { Name =ABS_PROCESS.Code, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "吸光度测量代码", IsUnique = false }},

            [ABS_WAIT_LIST.TableName] = new List<TableField>{
                new TableField { Name =WAIT_LIST.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                    DefaultValue = null, Comment = "序号", IsUnique = false },
                new TableField { Name =WAIT_LIST.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方代码", IsUnique = false },
                new TableField { Name =WAIT_LIST.VersionNum, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "版本", IsUnique = false },
                new TableField { Name =WAIT_LIST.IndexNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "序号", IsUnique = false },
                new TableField { Name =WAIT_LIST.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "杯号", IsUnique = false },
                new TableField { Name =WAIT_LIST.Type, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "类型", IsUnique = false }},

            [ALARM_TABLE.TableName] = new List<TableField>{
                    new TableField { Name =ALARM_TABLE.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                        DefaultValue = null, Comment = "ID", IsUnique = false },
                    new TableField { Name =ALARM_TABLE.MyDate, Type = "DATE", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "日期", IsUnique = false },
                    new TableField { Name =ALARM_TABLE.MyTime, Type = "NVARCHAR(8)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "时间", IsUnique = false },
                    new TableField { Name =ALARM_TABLE.AlarmHead, Type = "NVARCHAR(2000)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "报警头", IsUnique = false },
                    new TableField { Name =ALARM_TABLE.AlarmDetails, Type = "NVARCHAR(2000)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "报警详情", IsUnique = false }},

            [ASSISTANT_DETAILS.TableName] = new List<TableField>{
                    new TableField { Name =ASSISTANT_DETAILS.ID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                        DefaultValue = null, Comment = "索引", IsUnique = false },
                    new TableField { Name =ASSISTANT_DETAILS.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂代码", IsUnique = false },
                    new TableField { Name =ASSISTANT_DETAILS.AssistantBarCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂条形码", IsUnique = false },
                    new TableField { Name =ASSISTANT_DETAILS.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂名称", IsUnique = false },
                    new TableField { Name =ASSISTANT_DETAILS.AssistantType, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂类型", IsUnique = false },
                    new TableField { Name =ASSISTANT_DETAILS.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'%')", Comment = "计算单位", IsUnique = false },
                    new TableField { Name =ASSISTANT_DETAILS.AllowMinColoringConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "允许最低打色浓度", IsUnique = false },
                    new TableField { Name =ASSISTANT_DETAILS.AllowMaxColoringConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "允许最高打色浓度", IsUnique = false },
                    new TableField { Name =ASSISTANT_DETAILS.TermOfValidity, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "有效期限(小时)", IsUnique = false },
                    new TableField { Name =ASSISTANT_DETAILS.Intensity, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "力度", IsUnique = false },
                    new TableField { Name =ASSISTANT_DETAILS.Cost, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "成本", IsUnique = false },
                    new TableField { Name =ASSISTANT_DETAILS.Correcting, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "校正值", IsUnique = false },

                    new TableField { Name =ASSISTANT_DETAILS.Reweigh, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染色工艺滴液时是否复称", IsUnique = false }},

            [BOTTLE_CHECK.TableName] = new List<TableField>{
                    new TableField { Name =BOTTLE_CHECK.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "瓶号", IsUnique = false },
                    new TableField { Name =BOTTLE_CHECK.Finish, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "完成标志位", IsUnique = false },
                    new TableField { Name =BOTTLE_CHECK.Successed, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "成功标志位", IsUnique = false }},

            [BOTTLE_DETAILS.TableName] = new List<TableField>{
                    new TableField { Name =BOTTLE_DETAILS.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "瓶号", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂代码", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "设定浓度", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际浓度", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.LastAdjustWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "上次针检重量", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.CurrentAdjustWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "当前针检重量", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.AdjustValue, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "校正值", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.BrewingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "泡制流程代码", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.AllowMaxWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((1000))", Comment = "允许最大调液量", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.CurrentWeight, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "当前库存量", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.BrewingData, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "调液日期", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.SyringeType, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'小针筒')", Comment = "针筒类型", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.DropMinWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((1))", Comment = "最小滴液量", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.OriginalBottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "开稀原瓶号", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.AdjustSuccess, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "针检完成", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.SelfChecking1, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "自检11g数据", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.SelfChecking2, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "自检5g数据", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.SelfChecking3, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "自检2g数据", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.SelfChecking4, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "自检0.5g数据", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.Abs, Type = "NVARCHAR(1000)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "吸光度数据", IsUnique = false },

                    new TableField { Name =BOTTLE_DETAILS.Compensate, Type = "DECIMAL(18,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "修正系数", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.Status, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "状态", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.DripReserveFirst, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "是否预滴液", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.WashSyringeSpan, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "洗针间隔", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.EvacuateSpan, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((3))", Comment = "排空间隔", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.LastWashTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "上次洗针时间", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.AbsCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "吸光度测量代码", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.StartingWavelength, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((320))", Comment = "开始波长", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.EndWavelength, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                         DefaultValue = "((700))", Comment = "结束波长", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.WavelengthInterval, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((5))", Comment = "波长间隔", IsUnique = false },
                    new TableField { Name =BOTTLE_DETAILS.LastUseTime, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "上次使用时间", IsUnique = false }},

            [BREW_RUN_TABLE.TableName] = new List<TableField>{
                    new TableField { Name =BREW_RUN_TABLE.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                        DefaultValue = null, Comment = "ID", IsUnique = false },
                    new TableField { Name =BREW_RUN_TABLE.StartDateTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开始时间", IsUnique = false },
                    new TableField { Name =BREW_RUN_TABLE.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "瓶号", IsUnique = false },
                    new TableField { Name =BREW_RUN_TABLE.BrewCode, Type = "NVARCHAR(500)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "调液流程代码", IsUnique = false },
                    new TableField { Name =BREW_RUN_TABLE.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "设定浓度", IsUnique = false },
                    new TableField { Name =BREW_RUN_TABLE.AllowMaxWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((1000))", Comment = "允许最大调液量", IsUnique = false },
                    new TableField { Name =BREW_RUN_TABLE.OriginalBottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "开稀原瓶号", IsUnique = false },
                    new TableField { Name =BREW_RUN_TABLE.OriginalConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "原浓度", IsUnique = false },
                    new TableField { Name =BREW_RUN_TABLE.InputMode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "输入模式", IsUnique = false },
                    new TableField { Name =BREW_RUN_TABLE.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂名称", IsUnique = false },
                    new TableField { Name =BREW_RUN_TABLE.FinishDateTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "完成时间", IsUnique = false },
                     new TableField { Name =BREW_RUN_TABLE.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((0))", Comment = "实际浓度", IsUnique = false },
                     new TableField { Name =BOTTLE_DETAILS.CurrentWeight, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((0))", Comment = "实际液量", IsUnique = false },
                     new TableField { Name =BREW_RUN_TABLE.UseingTime, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "总用时", IsUnique = false },
                      new TableField { Name =BREW_RUN_TABLE.ReasonCessation, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "停止原因", IsUnique = false },

            },

            [CHECK_TABLE.TableName] = new List<TableField>{
                    new TableField { Name =CHECK_TABLE.Date, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "时间", IsUnique = false },
                    new TableField { Name =CHECK_TABLE.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "瓶号", IsUnique = false },

                    new TableField { Name =CHECK_TABLE.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "实际浓度", IsUnique = false },
                    new TableField { Name =CHECK_TABLE.CurrentWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((0))", Comment = "当前液量", IsUnique = false },

                    new TableField { Name =CHECK_TABLE.CurrentAdjustWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "校正重量", IsUnique = false },
                    new TableField { Name =CHECK_TABLE.AdjustValue, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "校正值", IsUnique = false },
                    new TableField { Name =CHECK_TABLE.RecheckWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "复检重量", IsUnique = false },
                    new TableField { Name =CHECK_TABLE.Fail, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "是否失败", IsUnique = false }

            },

            [BREWING_CODE.TableName] = new List<TableField>{
                    new TableField { Name =BREWING_CODE.BrewingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "调液流程代码", IsUnique = false }},

            [BREWING_PROCESS.TableName] = new List<TableField>{
                    new TableField { Name =BREWING_PROCESS.StepNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((1))", Comment = "步号", IsUnique = false },
                    new TableField { Name =BREWING_PROCESS.TechnologyName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "工艺名称", IsUnique = false },
                    new TableField { Name =BREWING_PROCESS.ProportionOrTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "比例(%)/时间(s)", IsUnique = false },
                    new TableField { Name =BREWING_PROCESS.BrewingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "调液流程代码", IsUnique = false },
                    new TableField { Name =BREWING_PROCESS.Ratio, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "热水占比", IsUnique = false }},

            [CUP_DETAILS.TableName] = new List<TableField>{
                    new TableField { Name =CUP_DETAILS.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "杯号", IsUnique = false },
                     new TableField { Name =CUP_DETAILS.HeadID, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方HeadID", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.MainCupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "主副杯号", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.IsFixed, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((1))", Comment = "是否固定色", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.Enable, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "是否可用", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.IsUsing, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "是否正在使用", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.Statues, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false,  IsNullable = true,
                        DefaultValue = null, Comment = "状态", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方代码", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.DyeingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染固色代码", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.StartTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开始时间", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.SetTemp, Type = "NUMERIC(8,1)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "设定温度", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.RealTemp, Type = "NUMERIC(8,1)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "实际温度", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.TotalWeight, Type = "NUMERIC(8,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "总量", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.CurrentWeight, Type = "NUMERIC(8,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "当前重量", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.StepNum, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "当前步号", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.TotalStep, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "总步数", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.TechnologyName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "工艺", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.StepStartTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "当前步开始时间", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.SetTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "设定时间", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.RecordIndex, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "记录温度点数", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.Cooperate, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "操作", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.Type, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((1))", Comment = "类型", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.CoverStatus, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((1))", Comment = "杯盖状态", IsUnique = false },

                    new TableField { Name =CUP_DETAILS.Fail, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "是否失败", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.HaveCloth, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "是否有布", IsUnique = false },
                     new TableField { Name =CUP_DETAILS.CurrentStepFinish, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "当前步是否完成", IsUnique = false },


                    new TableField { Name =CUP_DETAILS.ReceptionTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "收到请求时间", IsUnique = false },
                    new TableField { Name =CUP_DETAILS.DyeType, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染色后处理类型", IsUnique = false }},

            [DROP_DETAILS.TableName] = new List<TableField>{
                    new TableField { Name =DROP_DETAILS.HeadID, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "表头序号", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.BatchName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "批次号", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "杯号", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方代码", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "版本号", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.IndexNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((1))", Comment = "序号", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂代码", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.FormulaDosage, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "配方用量", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'%')", Comment = "计算单位", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "瓶号", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "设定浓度", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际浓度", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染助剂名称", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.ObjectDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "目标滴液重", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.RealDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际滴液重", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.BottleSelection, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "手动选瓶", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.MinWeight, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "液量低", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.Finish, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "完成", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.ObjectPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "目标粉重", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.RealPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "实际粉重", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.IsShow, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "是否显示", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.IsDrop, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((1))", Comment = "是否已经滴液", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.BrewingData, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开料日期", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.NeedPulse, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "需加脉冲", IsUnique = false },
                    new TableField { Name =DROP_DETAILS.StandError, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "滴液误差", IsUnique = false }},

            [DROP_HEAD.TableName] = new List<TableField>{
                    new TableField { Name =DROP_HEAD.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                        DefaultValue = null, Comment = "序号", IsUnique = false },
                    new TableField { Name =DROP_HEAD.BatchName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "批次号", IsUnique = false },
                    new TableField { Name =DROP_HEAD.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "杯号", IsUnique = false },
                    new TableField { Name =DROP_HEAD.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方代码", IsUnique = false },
                    new TableField { Name =DROP_HEAD.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "版本", IsUnique = false },
                    new TableField { Name =DROP_HEAD.State, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'尚未滴液')", Comment = "状态", IsUnique = false },
                    new TableField { Name =DROP_HEAD.FormulaName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "配方名称", IsUnique = false },
                    new TableField { Name =DROP_HEAD.ClothType, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "布种/基材", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Customer, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "客户", IsUnique = false },
                    new TableField { Name =DROP_HEAD.AddWaterChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "是否加水", IsUnique = false },
                    new TableField { Name =DROP_HEAD.CompoundBoardChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "是否复版", IsUnique = false },
                    new TableField { Name =DROP_HEAD.ClothWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "布重", IsUnique = false },
                    new TableField { Name =DROP_HEAD.BathRatio, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "浴比", IsUnique = false },
                    new TableField { Name =DROP_HEAD.TotalWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "总浴量", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Operator, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "操作员", IsUnique = false },
                    new TableField { Name =DROP_HEAD.CupCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "(N'无')", Comment = "染杯代码", IsUnique = false },
                    new TableField { Name =DROP_HEAD.CreateTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "存档时间", IsUnique = false },
                    new TableField { Name =DROP_HEAD.ObjectAddWaterWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((0.0))", Comment = "目标加水重量", IsUnique = false },
                    new TableField { Name =DROP_HEAD.RealAddWaterWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = "((0.0))", Comment = "实际加水重量", IsUnique = false },
                    new TableField { Name =DROP_HEAD.TestTubeObjectAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable =
                        false, DefaultValue = "((0))", Comment = "试管目标加水量", IsUnique = false },
                    new TableField { Name =DROP_HEAD.TestTubeRealAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable =
                        false, DefaultValue = "((0))", Comment = "试管实际加水量", IsUnique = false },
                    new TableField { Name =DROP_HEAD.TestTubeFinish, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "试管加水完成", IsUnique = false },
                    new TableField { Name =DROP_HEAD.TestTubeWaterLower, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "试管液量低标志位", IsUnique = false },
                    new TableField { Name =DROP_HEAD.AddWaterFinish, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "加水完成", IsUnique = false },
                    new TableField { Name =DROP_HEAD.CupFinish, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "杯完成", IsUnique = false },
                    new TableField { Name =DROP_HEAD.DyeingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染固色代码", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Step, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "步号", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Non_AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "非脱水水比", IsUnique = false },
                    new TableField { Name =DROP_HEAD.AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "脱水水比", IsUnique = false },
                    new TableField { Name =DROP_HEAD.HandleBathRatio, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "后处理浴比", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Handle_Rev1, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "转速1", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Handle_Rev2, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "转速2", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Handle_Rev3, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "转速3", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Handle_Rev4, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "转速4", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Handle_Rev5, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "转速5", IsUnique = false },
                    new TableField { Name =DROP_HEAD.DescribeChar, Type = "NVARCHAR(100)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "描述", IsUnique = false },
                    new TableField { Name =DROP_HEAD.FinishTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "完成时间", IsUnique = false },
                    new TableField { Name =DROP_HEAD.StartTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开始时间", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Stage, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "类型", IsUnique = false },
                    new TableField { Name =DROP_HEAD.DescribeChar_EN, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "描述英文", IsUnique = false },
                    new TableField { Name =DROP_HEAD.HandleBRList, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染色后处理浴比集合", IsUnique = false },
                    new TableField { Name =DROP_HEAD.WaterStandError, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "加水误差", IsUnique = false },
                    new TableField { Name =DROP_HEAD.IsAutoIn, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "是否自动放布", IsUnique = false },
                    new TableField { Name =DROP_HEAD.ClothNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "放布位", IsUnique = false },
                    new TableField { Name =DROP_HEAD.DyeingCodeRemark, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染固色代码备注", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Recoloration, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "复色原因", IsUnique = false },
                    new TableField { Name =DROP_HEAD.VatNumber, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "缸位", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Note1, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "备注1", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Note2, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "备注2", IsUnique = false },
                    new TableField { Name =DROP_HEAD.Note3, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "备注3", IsUnique = false }},

            [DYE_DETAILS.TableName] = new List<TableField>{
                    new TableField { Name =DYE_DETAILS.HeadID, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "表头序号", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.BatchName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "批次号", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "杯号", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方代码", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "版本号", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "助剂代码", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.FormulaDosage, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方用量", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "计算单位", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "瓶号", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "设定浓度", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "实际浓度", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "助剂名称", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.ObjectDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "目标滴液量", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.RealDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "实际滴液量", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.BottleSelection, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "是否收到选瓶", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.MinWeight, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "类型", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.Finish, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "是否完成", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.StepNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "步号", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.TechnologyName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "工艺", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.Temp, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "设定温度", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.TempSpeed, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "升温速率", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.Time, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "保温时间", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.ObjectWaterWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "目标加水量", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.RotorSpeed, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "转速", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.OvertempNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "超温次数", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.OvertempTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "超温时间", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.Code, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "工艺名称", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.StartTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开始时间", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.FinishTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "完成时间", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.Cooperate, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                        DefaultValue = "((0))", Comment = "操作", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.Compensation, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "补偿系数", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.BrewingData, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开料日期", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.ReceptionTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "接收信号时间", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.DyeType, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "类型", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.NeedPulse, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "需加脉冲", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.WaterFinish, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "是否加水完成", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.Choose, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "待办事项选择结果", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.StandError, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "滴液误差", IsUnique = false },
                    new TableField { Name =DYE_DETAILS.IndexNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "顺序号", IsUnique = false }},

            [DYEING_CODE.TableName] = new List<TableField>{
                    new TableField { Name =DYEING_CODE.DyeingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染固色代码", IsUnique = false },
                    new TableField { Name =DYEING_CODE.Type, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "类型", IsUnique = false },
                    new TableField { Name =DYEING_CODE.Step, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "步号", IsUnique = false },
                    new TableField { Name =DYEING_CODE.Code, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染色后处理工艺", IsUnique = false },
                    new TableField { Name =DYEING_CODE.IndexNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "顺序号", IsUnique = false },
                    new TableField { Name =DYEING_CODE.IsUse, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "是否可用", IsUnique = false },
                    new TableField { Name =DYEING_CODE.Remark, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "备注", IsUnique = false }},

            [HISTORY_DYEING_CODE.TableName] = new List<TableField>{
                    new TableField { Name =HISTORY_DYEING_CODE.DyeingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染固色代码", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_CODE.Type, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "类型", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_CODE.Step, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "步号", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_CODE.Code, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染色后处理工艺", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_CODE.IndexNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "顺序号", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_CODE.IsUse, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "是否可用", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_CODE.Remark, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "备注", IsUnique = false }},

            [DYEING_DETAILS.TableName] = new List<TableField>{
                    new TableField { Name =DYEING_DETAILS.BatchName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "批次号", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "杯号", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方代码", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "版本号", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "助剂代码", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.FormulaDosage, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "配方用量", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "计算单位", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "瓶号", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "设定浓度", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "实际浓度", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "助剂名称", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.ObjectDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "目标滴液量", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.RealDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "实际滴液量", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.BottleSelection, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "是否自动选瓶", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.MinWeight, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "类型", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.Finish, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "是否完成", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.StepNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "步号", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.TechnologyName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "工艺", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.Temp, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "设定温度", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.TempSpeed, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "升温速率", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.Time, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "保温时间", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.ObjectWaterWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "目标加水量", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.RotorSpeed, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "转速", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.OvertempNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "超温次数", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.OvertempTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "超温时间", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.Code, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染色后处理工艺", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.StartTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开始时间", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.FinishTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "完成时间", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.Cooperate, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "操作", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.Compensation, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "补偿系数", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.BrewingData, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "开料日期", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.ReceptionTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "接收信号时间", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.DyeType, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "类型", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.NeedPulse, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "需加脉冲", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.Choose, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "待办事项选择结果", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.WaterFinish, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "加水是否完成", IsUnique = false },
                    new TableField { Name =DYEING_DETAILS.No, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "序号", IsUnique = false }},

            [DYEING_PROCESS.TableName] = new List<TableField>{
                    new TableField { Name =DYEING_PROCESS.StepNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "步号", IsUnique = false },
                    new TableField { Name =DYEING_PROCESS.TechnologyName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "工艺", IsUnique = false },
                    new TableField { Name =DYEING_PROCESS.ProportionOrTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "比例(%)/时间(s)", IsUnique = false },
                    new TableField { Name =DYEING_PROCESS.Code, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染色后处理工艺", IsUnique = false },
                    new TableField { Name =DYEING_PROCESS.Temp, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "设定温度", IsUnique = false },
                    new TableField { Name =DYEING_PROCESS.Rate, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "速率", IsUnique = false },
                    new TableField { Name =DYEING_PROCESS.Type, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "类型", IsUnique = false },
                    new TableField { Name =DYEING_PROCESS.Rev, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "转速", IsUnique = false },
                    new TableField { Name =DYEING_PROCESS.Remark, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "备注", IsUnique = false },
                    new TableField { Name =DYEING_PROCESS.OpenMedicine, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "是否关盖加药", IsUnique = false }},

            [HISTORY_DYEING_PROCESS.TableName] = new List<TableField>{
                    new TableField { Name =HISTORY_DYEING_PROCESS.StepNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "步号", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_PROCESS.TechnologyName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "工艺", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_PROCESS.ProportionOrTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "比例(%)/时间(s)", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_PROCESS.Code, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "染色后处理工艺", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_PROCESS.Temp, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "设定温度", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_PROCESS.Rate, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "速率", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_PROCESS.Type, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "类型", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_PROCESS.Rev, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "转速", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_PROCESS.Remark, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "备注", IsUnique = false },
                    new TableField { Name =HISTORY_DYEING_PROCESS.OpenMedicine, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "是否关盖加药", IsUnique = false }},

            [DYEING_REMARK.TableName] = new List<TableField>{
                    new TableField { Name =DYEING_REMARK.Code, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "代码", IsUnique = false },
                    new TableField { Name =DYEING_REMARK.Remark, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                        DefaultValue = null, Comment = "备注", IsUnique = false }},

            [ENABLED_SET.TableName] = new List<TableField>{
                new TableField { Name =ENABLED_SET.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = false, IsNullable = false,
                    DefaultValue = null, Comment = "自增", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_FormulaName, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "配方名称", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_ClothType, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "布种/基材", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_Customer, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "客户", IsUnique = false },
                new TableField { Name =ENABLED_SET.Chk_AddWaterChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((1))", Comment = "是否加水", IsUnique = false },
                new TableField { Name =ENABLED_SET.Chk_CompoundBoardChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "((1))", Comment = "是否复板", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_ClothWeight, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "((1))", Comment = "布重", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_BathRatio, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "((1))", Comment = "浴比", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_Operator, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "操作员", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_CupNum, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "染杯代码", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_DyeingCode, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "染固色代码", IsUnique = false },
                new TableField { Name =ENABLED_SET.Dgv_FormulaData, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "((1))", Comment = "配方表", IsUnique = false },
                new TableField { Name =ENABLED_SET.Dgv_AddAssistant, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "助剂", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_Non_AnhydrationWR, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "非脱水水比", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_AnhydrationWR, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "脱水水比", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_HandleBathRatio, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理浴比", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_Handle_Rev1, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速1", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_Handle_Rev2, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速2", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_Handle_Rev3, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速3", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_Handle_Rev4, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速4", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_Handle_Rev5, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速5", IsUnique = false },
                new TableField { Name =ENABLED_SET.Dgv_Handle1, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理1", IsUnique = false },
                new TableField { Name =ENABLED_SET.Dgv_Handle2, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理2", IsUnique = false },
                new TableField { Name =ENABLED_SET.Dgv_Handle3, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理3", IsUnique = false },
                new TableField { Name =ENABLED_SET.Dgv_Handle4, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理4", IsUnique = false },
                new TableField { Name =ENABLED_SET.Dgv_Handle5, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理5", IsUnique = false },
                new TableField { Name =ENABLED_SET.Dgv_Dye, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理", IsUnique = false },
                new TableField { Name =ENABLED_SET.BatchName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "当前批次号", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_Stage, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "类型", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_FormulaGroup, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方组合", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_CupCode, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = null, IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_Recoloration, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "复色原因", IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_Note1, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = null, IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_Note2, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = null, IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_Note3, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = null, IsUnique = false },
                new TableField { Name =ENABLED_SET.Txt_ClothNum, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "放布位", IsUnique = false },
                new TableField { Name =ENABLED_SET.Chk_Auto, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "自动出放布", IsUnique = false },
                new TableField { Name =ENABLED_SET.Note1Name, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注1名称", IsUnique = false },
                new TableField { Name =ENABLED_SET.Note1Items, Type = "NVARCHAR(500)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注1可选项", IsUnique = false },
                new TableField { Name =ENABLED_SET.Note2Name, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注2名称", IsUnique = false },
                new TableField { Name =ENABLED_SET.Note2Items, Type = "NVARCHAR(500)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注2可选项", IsUnique = false },
                new TableField { Name =ENABLED_SET.Note3Name, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注3名称", IsUnique = false },
                new TableField { Name =ENABLED_SET.Note3Items, Type = "NVARCHAR(500)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注3可选项", IsUnique = false }},

            [ABS_ENABLED_SET.TableName] = new List<TableField>{
                new TableField { Name =ABS_ENABLED_SET.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = false, IsNullable = false,
                    DefaultValue = null, Comment = "自增", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_FormulaName, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "配方名称", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_ClothType, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "布种/基材", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_Customer, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "客户", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Chk_AddWaterChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "((1))", Comment = "是否加水", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Chk_CompoundBoardChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "((1))", Comment = "是否复板", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_ClothWeight, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "((1))", Comment = "布重", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_BathRatio, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "((1))", Comment = "浴比", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_Operator, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "操作员", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_CupNum, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "染杯代码", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_DyeingCode, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "染固色代码", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Dgv_FormulaData, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "((1))", Comment = "配方表", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Dgv_AddAssistant, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "助剂", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_Non_AnhydrationWR, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "非脱水水比", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_AnhydrationWR, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "脱水水比", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_HandleBathRatio, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理浴比", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_Handle_Rev1, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速1", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_Handle_Rev2, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速2", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_Handle_Rev3, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速3", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_Handle_Rev4, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速4", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_Handle_Rev5, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速5", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Dgv_Handle1, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理1", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Dgv_Handle2, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理2", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Dgv_Handle3, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理3", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Dgv_Handle4, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理4", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Dgv_Handle5, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理5", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Dgv_Dye, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.BatchName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "当前批次号", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_Stage, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "类型", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_FormulaGroup, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方组合", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_CupCode, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = null, IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_Recoloration, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "复色原因", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_Note1, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = null, IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_Note2, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = null, IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_Note3, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = null, IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Txt_ClothNum, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "放布位", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Chk_Auto, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "自动出放布", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Note1Name, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注1名称", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Note1Items, Type = "NVARCHAR(500)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注1可选项", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Note2Name, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注2名称", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Note2Items, Type = "NVARCHAR(500)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注2可选项", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Note3Name, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注3名称", IsUnique = false },
                new TableField { Name =ABS_ENABLED_SET.Note3Items, Type = "NVARCHAR(500)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注3可选项", IsUnique = false }},

            [FORMULA_DETAILS.TableName] = new List<TableField>{
                new TableField { Name =FORMULA_DETAILS.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方代码", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "版本号", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.IndexNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "((1))", Comment = "序号", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染助剂代码", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.FormulaDosage, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "配方用量", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "(N'%')", Comment = "计算单位", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "瓶号", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "设定浓度", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "实际浓度", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染助剂名称", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.ObjectDropWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "目标滴液重", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.RealDropWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "实际滴液量", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.BottleSelection, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "手动选瓶", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.ObjectPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "目标粉重", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.RealPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "实际粉重", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.IsShow, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "是否显示", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS.BrewingData, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "开料日期", IsUnique = false }},

            [FORMULA_DETAILS_TEMP.TableName] = new List<TableField>{
                new TableField { Name =FORMULA_DETAILS_TEMP.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方代码", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "版本号", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.IndexNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "序号", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "助剂代码", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.FormulaDosage, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方用量", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "计算单位", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "瓶号", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "设定浓度", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "实际浓度", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "助剂名称", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.ObjectDropWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "目标滴液量", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.RealDropWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "实际滴液量", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.BottleSelection, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "是否收到选瓶", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.ObjectPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "目标加粉量", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.RealPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "实际加粉量", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.IsShow, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "是否显示", IsUnique = false },
                new TableField { Name =FORMULA_DETAILS_TEMP.BrewingData, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "开料日期", IsUnique = false }},

            [FORMULA_GROUP.TableName] = new List<TableField>{
                new TableField { Name =FORMULA_GROUP.Id, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                    DefaultValue = null, Comment = "ID", IsUnique = false },
                new TableField { Name =FORMULA_GROUP.GroupName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方组合", IsUnique = false },
                new TableField { Name =FORMULA_GROUP.Node, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "序号", IsUnique = false },
                new TableField { Name =FORMULA_GROUP.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "助剂代码", IsUnique = false },
                new TableField { Name =FORMULA_GROUP.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "助剂名称", IsUnique = false },
                new TableField { Name =FORMULA_GROUP.CreateTime, Type = "DATETIME", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "创建时间", IsUnique = false },
                new TableField { Name =FORMULA_GROUP.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "计算单位", IsUnique = false }},

            [FORMULA_HANDLE_DETAILS.TableName] = new List<TableField>{
                new TableField { Name =FORMULA_HANDLE_DETAILS.DyeingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染固色代码", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.Code, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染色后处理工艺", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.TechnologyName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "工艺", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方代码", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "版本号", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "助剂代码", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.FormulaDosage, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方用量", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "计算单位", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "瓶号", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "设定浓度", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "实际浓度", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "助剂名称", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.ObjectDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "目标滴液量", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.RealDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "实际滴液量", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.BottleSelection, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "是否自动选瓶", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.MinWeight, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "类型", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.Finish, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "是否完成", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS.No, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "序号", IsUnique = false }},

            [FORMULA_HANDLE_DETAILS_TEMP.TableName] = new List<TableField>{
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.DyeingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染固色代码", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.Code, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染色后处理名称", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.TechnologyName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "工艺", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方代码", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "版本号", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "助剂代码", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.FormulaDosage, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "用量", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "计算单位", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "瓶号", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "设定浓度", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "实际浓度", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "助剂名称", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.ObjectDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "目标滴液量", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.RealDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "实际滴液量", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.BottleSelection, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "瓶选择", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.MinWeight, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "类型", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.Finish, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "是否完成", IsUnique = false },
                new TableField { Name =FORMULA_HANDLE_DETAILS_TEMP.No, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "序号", IsUnique = false }},

            [FORMULA_HEAD.TableName] = new List<TableField>{
                new TableField { Name =FORMULA_HEAD.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                    DefaultValue = null, Comment = "ID", IsUnique = true },
                new TableField { Name =FORMULA_HEAD.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方代码", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "版本", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.State, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "(N'尚未滴液')", Comment = "状态", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.FormulaName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "(N'无')", Comment = "配方名称", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.ClothType, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "(N'无')", Comment = "布种/基材", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.Customer, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "(N'无')", Comment = "客户", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.AddWaterChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "是否加水", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.CompoundBoardChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "是否复板", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.ClothWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "布重", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.BathRatio, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "浴比", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.TotalWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "总浴量", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.Operator, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "(N'无')", Comment = "操作员", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.CupCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "(N'无')", Comment = "染杯代码", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.CreateTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "存档时间", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.ObjectAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false,IsNullable = false,
                    DefaultValue = "((0))", Comment = "目标加水重量", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.RealAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "实际加水重量", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.TestTubeObjectAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "试管目标加水量", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.TestTubeRealAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "试管实际加水量", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.DyeingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染固色代码", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "杯号", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.Non_AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "非脱水水比", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "脱水水比", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.HandleBathRatio, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理浴比", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.Handle_Rev1, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速1", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.Handle_Rev2, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速2", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.Handle_Rev3, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速3", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.Handle_Rev4, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速4", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.Handle_Rev5, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速5", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.Stage, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "类型", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.HandleBRList, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染色后处理浴比集合", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.ClothNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "放布位", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.IsAutoIn, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "自动放布", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.DyeingCodeRemark, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染固色代码备注", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.Recoloration, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "复色原因", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.Note1, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注1", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.Note2, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注2", IsUnique = false },
                new TableField { Name =FORMULA_HEAD.Note3, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注3", IsUnique = false }},

            [FORMULA_HEAD_TEMP.TableName] = new List<TableField>{
                new TableField { Name =FORMULA_HEAD_TEMP.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                    DefaultValue = null, Comment = "序号", IsUnique = true },
                new TableField { Name =FORMULA_HEAD_TEMP.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方代码", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "版本号", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.State, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "滴液状态", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.FormulaName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方名称", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.ClothType, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "布种", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.Customer, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "客户", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.AddWaterChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "是否加水", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.CompoundBoardChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = null, IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.ClothWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "布重", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.BathRatio, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "浴比", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.TotalWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "总浴量", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.Operator, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "操作员", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.CupCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "杯代码", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.CreateTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "创建日期", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.ObjectAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "目标加水量", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.RealAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "实际加水量", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.TestTubeObjectAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "试管目标加水量", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.TestTubeRealAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "试管实际加水量", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.DyeingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染固色代码", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "杯号", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.Non_AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "非脱水水比", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "脱水水比", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.HandleBathRatio, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理浴比", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.Handle_Rev1, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速1", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.Handle_Rev2, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速2", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.Handle_Rev3, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速3", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.Handle_Rev4, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速4", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.Handle_Rev5, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速5", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.Stage, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "类型", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.HandleBRList, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染色后处理浴比集合", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.ClothNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "放布位", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.IsAutoIn, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "自动放布", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.Note1, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注1", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.Note2, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注2", IsUnique = false },
                new TableField { Name =FORMULA_HEAD_TEMP.Note3, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注3", IsUnique = false }},

            [HANDLE_PROCESS.TableName] = new List<TableField>{
                new TableField { Name =HANDLE_PROCESS.StepNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "步号", IsUnique = false },
                new TableField { Name =HANDLE_PROCESS.TechnologyName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "工艺", IsUnique = false },
                new TableField { Name =HANDLE_PROCESS.ProportionOrTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "比例(%)/时间(s)", IsUnique = false },
                new TableField { Name =HANDLE_PROCESS.Code, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染色后处理工艺", IsUnique = false },
                new TableField { Name =HANDLE_PROCESS.Temp, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "设定温度", IsUnique = false },
                new TableField { Name =HANDLE_PROCESS.Rate, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "速率", IsUnique = false }},

            [HISTORY_ABSSTANDARD.TableName] = new List<TableField>{
                new TableField { Name =HISTORY_ABSSTANDARD.E1, Type = "NVARCHAR(1000)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "E1", IsUnique = false },
                new TableField { Name =HISTORY_ABSSTANDARD.E2, Type = "NVARCHAR(1000)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "E2", IsUnique = false },
                new TableField { Name =HISTORY_ABSSTANDARD.WL, Type = "NVARCHAR(1000)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "波长", IsUnique = false },
                new TableField { Name =HISTORY_ABSSTANDARD.FinishTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "完成时间", IsUnique = false },
                new TableField { Name =HISTORY_ABSSTANDARD.Type, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "类型", IsUnique = false }},

            [HISTORY_DETAILS.TableName] = new List<TableField>{
                new TableField { Name =HISTORY_DETAILS.HeadID, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "表头序号", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.BatchName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "批次号", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "杯号", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方代码", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "((1))", Comment = "版本号", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.IndexNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "序号", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染助剂代码", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.FormulaDosage, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "配方用量", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "(N'%')", Comment = "计算单位", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "瓶号", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "设定浓度", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "实际浓度", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染助剂名称", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.ObjectDropWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "目标滴液量", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.RealDropWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "实际滴液量", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.BottleSelection, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "手动选瓶", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.ObjectPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "目标粉重", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.RealPowderWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "实际粉重", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.IsDrop, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "((1))", Comment = "是否滴液", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.BrewingData, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "开料日期", IsUnique = false },
                new TableField { Name =HISTORY_DETAILS.StandError, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "滴液限值", IsUnique = false }},

            [HISTORY_DYE.TableName] = new List<TableField>{
                new TableField { Name =HISTORY_DYE.HeadID, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "表头序号", IsUnique = false },
                new TableField { Name =HISTORY_DYE.BatchName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "批次号", IsUnique = false },
                new TableField { Name =HISTORY_DYE.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "杯号", IsUnique = false },
                new TableField { Name =HISTORY_DYE.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方代码", IsUnique = false },
                new TableField { Name =HISTORY_DYE.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "版本号",IsUnique = false },
                new TableField { Name =HISTORY_DYE.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染助剂代码", IsUnique = false },
                new TableField { Name =HISTORY_DYE.FormulaDosage, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方用量", IsUnique = false },
                new TableField { Name =HISTORY_DYE.UnitOfAccount, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "计算单位", IsUnique = false },
                new TableField { Name =HISTORY_DYE.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "瓶号", IsUnique = false },
                new TableField { Name =HISTORY_DYE.SettingConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "设定浓度", IsUnique = false },
                new TableField { Name =HISTORY_DYE.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "实际浓度", IsUnique = false },
                new TableField { Name =HISTORY_DYE.AssistantName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染助剂名称", IsUnique = false },
                new TableField { Name =HISTORY_DYE.ObjectDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "目标滴液量", IsUnique = false },
                new TableField { Name =HISTORY_DYE.RealDropWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "实际滴液量", IsUnique = false },
                new TableField { Name =HISTORY_DYE.BottleSelection, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "手动选瓶", IsUnique = false },
                new TableField { Name =HISTORY_DYE.MinWeight, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "类型", IsUnique = false },
                new TableField { Name =HISTORY_DYE.Finish, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "是否完成", IsUnique = false },
                new TableField { Name =HISTORY_DYE.StepNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "步号", IsUnique = false },
                new TableField { Name =HISTORY_DYE.TechnologyName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "工艺名称", IsUnique = false },
                new TableField { Name =HISTORY_DYE.Temp, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "设定温度", IsUnique = false },
                new TableField { Name =HISTORY_DYE.TempSpeed, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "升温速率", IsUnique = false },
                new TableField { Name =HISTORY_DYE.Time, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "保温时间", IsUnique = false },
                new TableField { Name =HISTORY_DYE.ObjectWaterWeight, Type = "NUMERIC(10,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "目标加水量", IsUnique = false },
                new TableField { Name =HISTORY_DYE.RotorSpeed, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速", IsUnique = false },
                new TableField { Name =HISTORY_DYE.OvertempNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "超温次数", IsUnique = false },
                new TableField { Name =HISTORY_DYE.OvertempTime, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "超温时间", IsUnique = false },
                new TableField { Name =HISTORY_DYE.Code, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "工艺代码", IsUnique = false },
                new TableField { Name =HISTORY_DYE.StartTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "开始时间", IsUnique = false },
                new TableField { Name =HISTORY_DYE.FinishTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "结束时间", IsUnique = false },
                new TableField { Name =HISTORY_DYE.Compensation, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "补偿系数", IsUnique = false },
                new TableField { Name =HISTORY_DYE.BrewingData, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "开料日期", IsUnique = false },
                new TableField { Name =HISTORY_DYE.ReceptionTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "收到申请时间", IsUnique = false },
                new TableField { Name =HISTORY_DYE.DyeType, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染色后处理类型", IsUnique = false },
                new TableField { Name =HISTORY_DYE.StandError, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "滴液限值", IsUnique = false },
                new TableField { Name =HISTORY_DYE.No, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "顺序号", IsUnique = false }},

            [HISTORY_HEAD.TableName] = new List<TableField>{
                new TableField { Name =HISTORY_HEAD.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                    DefaultValue = null, Comment = null, IsUnique = false },
                 new TableField { Name =HISTORY_HEAD.HeadID, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = null, IsUnique = false },
                new TableField { Name =HISTORY_HEAD.BatchName, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "批次号", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "杯号", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方代码", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.VersionNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "版本", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.State, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = "(N'尚未滴液')", Comment = "状态", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.FormulaName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方名称", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.ClothType, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "布种/基材", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Customer, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "客户", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.AddWaterChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "是否加水", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.CompoundBoardChoose, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "是否复板", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.ClothWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "布重", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.BathRatio, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "浴比", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.TotalWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "总浴量", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Operator, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "操作员", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.CupCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染杯代码", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.CreateTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "存档时间", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.ObjectAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "目标加水重量", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.RealAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "实际加水重量", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.TestTubeObjectAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "试管目标加水量", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.TestTubeRealAddWaterWeight, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "试管实际加水量", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.FinishTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "完成时间", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.DescribeChar, Type = "NVARCHAR(100)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "描述", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.DyeingCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染固色代码", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Step, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "步数", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Non_AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "非脱水水比", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.AnhydrationWR, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "脱水水比", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.HandleBathRatio, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "后处理水比", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Handle_Rev1, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速1", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Handle_Rev2, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速2", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Handle_Rev3, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速3", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Handle_Rev4, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速4", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Handle_Rev5, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "转速5", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.StartTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "开始时间", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.ProcessData, Type = "IMAGE", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "温度曲线", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.MarkStep, Type = "NVARCHAR(500)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "曲线标记点", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Stage, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "类型", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Spectrum, Type = "NVARCHAR(500)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "光谱数据", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.DescribeChar_EN, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "描述英文", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.HandleBRList, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染色后处理浴比集合", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.WaterStandError, Type = "NVARCHAR(12)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "加水限值", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.ClothNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "放布位", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.IsAutoIn, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "自动方布", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.DyeingCodeRemark, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "染固色代码备注", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Recoloration, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "复色原因", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.VatNumber, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "缸位", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Result, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "结果", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Note1, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注1", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Note2, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "备注2", IsUnique = false },
                new TableField { Name =HISTORY_HEAD.Note3, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true, DefaultValue = null, Comment = "备注3", IsUnique = false }},

            [IMPORT_PARAMETERS.TableName] = new List<TableField>{
                new TableField { Name =IMPORT_PARAMETERS.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = false, IsNullable = false,
                    DefaultValue = null, Comment = "索引", IsUnique = false },
                new TableField { Name =IMPORT_PARAMETERS.StartUsing, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "启用", IsUnique = false },
                new TableField { Name =IMPORT_PARAMETERS.Route, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "路径", IsUnique = false },
                new TableField { Name =IMPORT_PARAMETERS.MyTime, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "扫描时间", IsUnique = false }},

            [LIMIT_TABLE.TableName] = new List<TableField>{
                new TableField { Name =LIMIT_TABLE.Type, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "类型", IsUnique = false },
                new TableField { Name =LIMIT_TABLE.Min, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "最小值", IsUnique = false },
                new TableField { Name =LIMIT_TABLE.Max, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "最大值", IsUnique = false },
                new TableField { Name =LIMIT_TABLE.AssistantCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "名称", IsUnique = false },
                new TableField { Name =LIMIT_TABLE.Value, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "值", IsUnique = false }},

            [LLJ.TableName] = new List<TableField>{
                new TableField { Name =LLJ.ID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                    DefaultValue = null, Comment = "序号", IsUnique = false },
                new TableField { Name =LLJ.Weight, Type = "DECIMAL(8,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "目标重量", IsUnique = false },
                new TableField { Name =LLJ.LLWeight, Type = "DECIMAL(8,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "理论重量", IsUnique = false },
                new TableField { Name =LLJ.Pulse, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "脉冲", IsUnique = false },
                new TableField { Name =LLJ.Time, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "时间", IsUnique = false },
                new TableField { Name =LLJ.LLC, Type = "DECIMAL(8,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "校正系数", IsUnique = false },
                new TableField { Name =LLJ.AddC, Type = "DECIMAL(8,3)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "校正重量", IsUnique = false }},

            [PRE_BREW.TableName] = new List<TableField>{
                new TableField { Name =PRE_BREW.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "瓶号", IsUnique = false },
                new TableField { Name =PRE_BREW.RealConcentration, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "实际浓度", IsUnique = false },
                new TableField { Name =PRE_BREW.CurrentWeight, Type = "NUMERIC(8,2)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "实际重量", IsUnique = false },
                new TableField { Name =PRE_BREW.BrewingData, Type = "DATETIME2(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "开料日期", IsUnique = false }},

            [PRETREATMENT_CODE.TableName] = new List<TableField>{
                new TableField { Name =PRETREATMENT_CODE.PretreatmentCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "前处理代码", IsUnique = false }},

            [RECHECK.TableName] = new List<TableField>{
                new TableField { Name =RECHECK.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = false, IsNullable = false,
                    DefaultValue = null, Comment = "序号", IsUnique = false },
                new TableField { Name =RECHECK.MyDate, Type = "DATE", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "日期", IsUnique = false },
                new TableField { Name =RECHECK.MyTime, Type = "TIME(7)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "时间", IsUnique = false },
                new TableField { Name =RECHECK.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "瓶号", IsUnique = false },
                new TableField { Name =RECHECK.CurrentWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "现存量", IsUnique = false },
                new TableField { Name =RECHECK.RealWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "实际滴液量", IsUnique = false }},

            [RUN_TABLE.TableName] = new List<TableField>{
                new TableField { Name =RUN_TABLE.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                    DefaultValue = null, Comment = "序号", IsUnique = false },
                new TableField { Name =RUN_TABLE.MyDate, Type = "DATE", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "动作日期", IsUnique = false },
                new TableField { Name =RUN_TABLE.MyTime, Type = "NVARCHAR(8)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "动作时刻", IsUnique = false },
                new TableField { Name =RUN_TABLE.Machine, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "系统动作", IsUnique = false },
                new TableField { Name =RUN_TABLE.RobotHand, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "机械手动作", IsUnique = false },
                new TableField { Name =RUN_TABLE.Dail, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "打板机动作", IsUnique = false },
                new TableField { Name =RUN_TABLE.Water, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "加水", IsUnique = false },
                new TableField { Name =RUN_TABLE.Powder, Type = "NVARCHAR(200)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "加粉", IsUnique = false }},

            [SELF_TABLE.TableName] = new List<TableField>{
                new TableField { Name =SELF_TABLE.Date, Type = "DATETIME", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "自检日期", IsUnique = false },
                new TableField { Name =SELF_TABLE.SelfChecking1, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "自检1", IsUnique = false },
                new TableField { Name =SELF_TABLE.SelfChecking2, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "自检2", IsUnique = false },
                new TableField { Name =SELF_TABLE.SelfChecking3, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "自检3", IsUnique = false },
                new TableField { Name =SELF_TABLE.SelfChecking4, Type = "NVARCHAR(11)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "自检4", IsUnique = false },
                new TableField { Name =SELF_TABLE.BottleNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "瓶号", IsUnique = false },
                new TableField { Name =SELF_TABLE.CurrentAdjustWeight, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "当前校正值", IsUnique = false },
                new TableField { Name =SELF_TABLE.AdjustValue, Type = "FLOAT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true, DefaultValue = null, Comment = "校正脉冲", IsUnique = false },
                new TableField { Name =SELF_TABLE.Fail, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "是否失败", IsUnique = false },
            },

            [SPEECHINFO.TableName] = new List<TableField>{
                new TableField { Name =SPEECHINFO.Info, Type = "NVARCHAR(250)", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = null, Comment = "播报内容", IsUnique = false },
                new TableField { Name =SPEECHINFO.IsFinished, Type = "TINYINT", IsPrimaryKey = false, IsIdentity = false, IsNullable = false,
                    DefaultValue = "((0))", Comment = "是否已播完", IsUnique = false }},

            [STANDARD.TableName] = new List<TableField>{
                new TableField { Name =STANDARD.E1, Type = "NVARCHAR(1000)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "E1", IsUnique = false },
                new TableField { Name =STANDARD.E2, Type = "NVARCHAR(1000)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "E2", IsUnique = false },
                new TableField { Name =STANDARD.WL, Type = "NVARCHAR(1000)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "波长", IsUnique = false },
                new TableField { Name =STANDARD.FinishTime, Type = "DATETIME2(0)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "完成时间", IsUnique = false },
                new TableField { Name =STANDARD.Type, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "类型", IsUnique = false }},

            [WAIT_LIST.TableName] = new List<TableField>{
                new TableField { Name =WAIT_LIST.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                    DefaultValue = null, Comment = "序号", IsUnique = false },
                new TableField { Name =WAIT_LIST.FormulaCode, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "配方代码", IsUnique = false },
                new TableField { Name =WAIT_LIST.VersionNum, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "版本", IsUnique = false },
                new TableField { Name =WAIT_LIST.IndexNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "序号", IsUnique = false },
                new TableField { Name =WAIT_LIST.CupNum, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "杯号", IsUnique = false },
                new TableField { Name =WAIT_LIST.Type, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "类型", IsUnique = false }},

            [USER_TALE.TableName] = new List<TableField>{
                new TableField { Name =WAIT_LIST.MyID, Type = "INT", IsPrimaryKey = true, IsIdentity = true, IsNullable = false,
                    DefaultValue = null, Comment = "序号", IsUnique = false },
                new TableField { Name =USER_TALE.Account, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "账户", IsUnique = false },
                new TableField { Name =USER_TALE.PassWord, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "密码", IsUnique = false },
                new TableField { Name =USER_TALE.Purview, Type = "INT", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "权限", IsUnique = false },
                new TableField { Name =USER_TALE.RealName, Type = "NVARCHAR(50)", IsPrimaryKey = false, IsIdentity = false, IsNullable = true,
                    DefaultValue = null, Comment = "姓名", IsUnique = false }}
        };
    }



    /// <summary>
    /// Abs_details表字段常量定义
    /// </summary>
    public static class ABS_DETAILS
    {
        /// <summary>表名</summary>
        public const string TableName = "Abs_details";
        /// <summary>表头序号</summary>
        public const string HeadID = "HeadID";

        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>是否完成</summary>
        public const string Finish = "Finish";
        /// <summary>开始时间</summary>
        public const string StartTime = "StartTime";
        /// <summary>结束时间</summary>
        public const string FinishTime = "FinishTime";
        /// <summary>操作</summary>
        public const string Cooperate = "Cooperate";
        /// <summary>步号</summary>
        public const string StepNum = "StepNum";
        /// <summary>工艺</summary>
        public const string TechnologyName = "TechnologyName";
        /// <summary>搅拌速度</summary>
        public const string StirringRate = "StirringRate";
        /// <summary>搅拌时间</summary>
        public const string StirringTime = "StirringTime";
        /// <summary>排液时间</summary>
        public const string DrainTime = "DrainTime";
        /// <summary>排比色皿时间</summary>
        public const string ParallelizingDishTime = "ParallelizingDishTime";
        /// <summary>抽液时间</summary>
        public const string PumpingTime = "PumpingTime";
        /// <summary>开始波长</summary>
        public const string StartingWavelength = "StartingWavelength";
        /// <summary>结束波长</summary>
        public const string EndWavelength = "EndWavelength";
        /// <summary>波长间隔</summary>
        public const string WavelengthInterval = "WavelengthInterval";
        /// <summary>用量</summary>
        public const string Dosage = "Dosage";
        /// <summary>吸光度测量工艺名称</summary>
        public const string Code = "Code";
        /// <summary>GUID</summary>
        public const string GUID = "GUID";
    }

    /// <summary>
    /// abs_drop_details表字段常量定义
    /// </summary>
    public static class ABS_DROP_DETAILS
    {
        /// <summary>表名</summary>
        public const string TableName = "abs_drop_details";
        /// <summary>表头序号</summary>
        public const string HeadID = "HeadID";
        /// <summary>批次号</summary>
        public const string BatchName = "BatchName";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本号</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>序号</summary>
        public const string IndexNum = "IndexNum";
        /// <summary>染助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>配方用量</summary>
        public const string FormulaDosage = "FormulaDosage";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>染助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>目标滴液重</summary>
        public const string ObjectDropWeight = "ObjectDropWeight";
        /// <summary>实际滴液重</summary>
        public const string RealDropWeight = "RealDropWeight";
        /// <summary>手动选瓶</summary>
        public const string BottleSelection = "BottleSelection";
        /// <summary>液量低</summary>
        public const string MinWeight = "MinWeight";
        /// <summary>完成</summary>
        public const string Finish = "Finish";
        /// <summary>目标粉重</summary>
        public const string ObjectPowderWeight = "ObjectPowderWeight";
        /// <summary>实际粉重</summary>
        public const string RealPowderWeight = "RealPowderWeight";
        /// <summary>是否显示</summary>
        public const string IsShow = "IsShow";
        /// <summary>是否已经滴液</summary>
        public const string IsDrop = "IsDrop";
        /// <summary>开料日期</summary>
        public const string BrewingData = "BrewingData";
        /// <summary>需加脉冲</summary>
        public const string NeedPulse = "NeedPulse";
    }

    /// <summary>
    /// abs_drop_head表字段常量定义
    /// </summary>
    public static class ABS_DROP_HEAD
    {
        /// <summary>表名</summary>
        public const string TableName = "abs_drop_head";
        /// <summary>序号</summary>
        public const string MyID = "MyID";
        /// <summary>批次号</summary>
        public const string BatchName = "BatchName";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>状态</summary>
        public const string State = "State";
        /// <summary>配方名称</summary>
        public const string FormulaName = "FormulaName";
        /// <summary>布种/基材</summary>
        public const string ClothType = "ClothType";
        /// <summary>客户</summary>
        public const string Customer = "Customer";
        /// <summary>是否加水</summary>
        public const string AddWaterChoose = "AddWaterChoose";

        /// <summary>布重</summary>
        public const string ClothWeight = "ClothWeight";
        /// <summary>浴比</summary>
        public const string BathRatio = "BathRatio";
        /// <summary>总浴量</summary>
        public const string TotalWeight = "TotalWeight";
        /// <summary>操作员</summary>
        public const string Operator = "Operator";
        /// <summary>染杯代码</summary>
        public const string CupCode = "CupCode";
        /// <summary>存档时间</summary>
        public const string CreateTime = "CreateTime";
        /// <summary>目标加水重量</summary>
        public const string ObjectAddWaterWeight = "ObjectAddWaterWeight";
        /// <summary>实际加水重量</summary>
        public const string RealAddWaterWeight = "RealAddWaterWeight";

        /// <summary>加水完成</summary>
        public const string AddWaterFinish = "AddWaterFinish";
        /// <summary>杯完成</summary>
        public const string CupFinish = "CupFinish";
        /// <summary>染固色代码</summary>
        public const string DyeingCode = "DyeingCode";
        /// <summary>步号</summary>
        public const string Step = "Step";
        /// <summary>非脱水水比</summary>
        public const string Non_AnhydrationWR = "Non_AnhydrationWR";
        /// <summary>脱水水比</summary>
        public const string AnhydrationWR = "AnhydrationWR";
        /// <summary>后处理浴比</summary>
        public const string HandleBathRatio = "HandleBathRatio";
        /// <summary>描述</summary>
        public const string DescribeChar = "DescribeChar";
        /// <summary>完成时间</summary>
        public const string FinishTime = "FinishTime";
        /// <summary>开始时间</summary>
        public const string StartTime = "StartTime";
        /// <summary>类型</summary>
        public const string Stage = "Stage";
        /// <summary>描述英文</summary>
        public const string DescribeChar_EN = "DescribeChar_EN";
        /// <summary>染色后处理浴比集合</summary>
        public const string HandleBRList = "HandleBRList";
        /// <summary>类型</summary>
        public const string Type = "Type";
        /// <summary>吸光度数据</summary>
        public const string Abs = "Abs";

        /// <summary>开始波长</summary>
        public const string StartWave = "StartWave";
        /// <summary>结束波长</summary>
        public const string EndWave = "EndWave";
        /// <summary>波长间隔</summary>
        public const string IntWave = "IntWave";
    }

    /// <summary>
    /// abs_formula_details表字段常量定义
    /// </summary>
    public static class ABS_FORMULA_DETAILS
    {
        /// <summary>表名</summary>
        public const string TableName = "abs_formula_details";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本号</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>序号</summary>
        public const string IndexNum = "IndexNum";
        /// <summary>染助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>配方用量</summary>
        public const string FormulaDosage = "FormulaDosage";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>染助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>目标滴液重</summary>
        public const string ObjectDropWeight = "ObjectDropWeight";
        /// <summary>实际滴液量</summary>
        public const string RealDropWeight = "RealDropWeight";
        /// <summary>手动选瓶</summary>
        public const string BottleSelection = "BottleSelection";
        /// <summary>目标粉重</summary>
        public const string ObjectPowderWeight = "ObjectPowderWeight";
        /// <summary>实际粉重</summary>
        public const string RealPowderWeight = "RealPowderWeight";
        /// <summary>是否显示</summary>
        public const string IsShow = "IsShow";
        /// <summary>开料日期</summary>
        public const string BrewingData = "BrewingData";
    }

    /// <summary>
    /// abs_formula_group表字段常量定义
    /// </summary>
    public static class ABS_FORMULA_GROUP
    {
        /// <summary>表名</summary>
        public const string TableName = "abs_formula_group";
        /// <summary>ID</summary>
        public const string Id = "Id";
        /// <summary>配方组合名称</summary>
        public const string GroupName = "group_Name";
        /// <summary>序号</summary>
        public const string Node = "node";
        /// <summary>配方代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>配方名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>创建时间</summary>
        public const string CreateTime = "createTime";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
    }

    /// <summary>
    /// abs_formula_head表字段常量定义
    /// </summary>
    public static class ABS_FORMULA_HEAD
    {
        /// <summary>表名</summary>
        public const string TableName = "abs_formula_head";
        /// <summary>ID</summary>
        public const string MyID = "MyID";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>状态</summary>
        public const string State = "State";
        /// <summary>配方名称</summary>
        public const string FormulaName = "FormulaName";
        /// <summary>布种/基材</summary>
        public const string ClothType = "ClothType";
        /// <summary>客户</summary>
        public const string Customer = "Customer";
        /// <summary>是否加水</summary>
        public const string AddWaterChoose = "AddWaterChoose";
        /// <summary>是否复板</summary>
        public const string CompoundBoardChoose = "CompoundBoardChoose";
        /// <summary>布重</summary>
        public const string ClothWeight = "ClothWeight";
        /// <summary>浴比</summary>
        public const string BathRatio = "BathRatio";
        /// <summary>总浴量</summary>
        public const string TotalWeight = "TotalWeight";
        /// <summary>操作员</summary>
        public const string Operator = "Operator";
        /// <summary>染杯代码</summary>
        public const string CupCode = "CupCode";
        /// <summary>存档时间</summary>
        public const string CreateTime = "CreateTime";
        /// <summary>目标加水重量</summary>
        public const string ObjectAddWaterWeight = "ObjectAddWaterWeight";
        /// <summary>实际加水重量</summary>
        public const string RealAddWaterWeight = "RealAddWaterWeight";

        /// <summary>染固色代码</summary>
        public const string DyeingCode = "DyeingCode";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>非脱水水比</summary>
        public const string Non_AnhydrationWR = "Non_AnhydrationWR";
        /// <summary>脱水水比</summary>
        public const string AnhydrationWR = "AnhydrationWR";
        /// <summary>后处理浴比</summary>
        public const string HandleBathRatio = "HandleBathRatio";

        /// <summary>类型</summary>
        public const string Stage = "Stage";
        /// <summary>染色后处理浴比集合</summary>
        public const string HandleBRList = "HandleBRList";

        /// <summary>备注1</summary>
        public const string Note1 = "Note1";
        /// <summary>备注2</summary>
        public const string Note2 = "Note2";
        /// <summary>备注3</summary>
        public const string Note3 = "Note3";
    }

    /// <summary>
    /// abs_history_details表字段常量定义
    /// </summary>
    public static class ABS_HISTORY_DETAILS
    {
        /// <summary>表名</summary>
        public const string TableName = "abs_history_details";
        /// <summary>批次号</summary>
        public const string BatchName = "BatchName";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本号</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>序号</summary>
        public const string IndexNum = "IndexNum";
        /// <summary>染助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>配方用量</summary>
        public const string FormulaDosage = "FormulaDosage";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>染助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>目标滴液量</summary>
        public const string ObjectDropWeight = "ObjectDropWeight";
        /// <summary>实际滴液量</summary>
        public const string RealDropWeight = "RealDropWeight";
        /// <summary>手动选瓶</summary>
        public const string BottleSelection = "BottleSelection";
        /// <summary>目标粉重</summary>
        public const string ObjectPowderWeight = "ObjectPowderWeight";
        /// <summary>实际粉重</summary>
        public const string RealPowderWeight = "RealPowderWeight";
        /// <summary>是否滴液</summary>
        public const string IsDrop = "IsDrop";
        /// <summary>开料日期</summary>
        public const string BrewingData = "BrewingData";
    }

    /// <summary>
    /// abs_history_head表字段常量定义
    /// </summary>
    public static class ABS_HISTORY_HEAD
    {
        /// <summary>表名</summary>
        public const string TableName = "abs_history_head";
        /// <summary>ID</summary>
        public const string MyID = "MyID";
        /// <summary>批次号</summary>
        public const string BatchName = "BatchName";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>状态</summary>
        public const string State = "State";
        /// <summary>配方名称</summary>
        public const string FormulaName = "FormulaName";
        /// <summary>布重/基材</summary>
        public const string ClothType = "ClothType";
        /// <summary>客户</summary>
        public const string Customer = "Customer";
        /// <summary>是否加水</summary>
        public const string AddWaterChoose = "AddWaterChoose";
        /// <summary>布重</summary>
        public const string ClothWeight = "ClothWeight";
        /// <summary>浴比</summary>
        public const string BathRatio = "BathRatio";
        /// <summary>总浴量</summary>
        public const string TotalWeight = "TotalWeight";
        /// <summary>操作员</summary>
        public const string Operator = "Operator";
        /// <summary>染杯代码</summary>
        public const string CupCode = "CupCode";
        /// <summary>存档时间</summary>
        public const string CreateTime = "CreateTime";
        /// <summary>目标加水重量</summary>
        public const string ObjectAddWaterWeight = "ObjectAddWaterWeight";
        /// <summary>实际加水重量</summary>
        public const string RealAddWaterWeight = "RealAddWaterWeight";
        /// <summary>完成时间</summary>
        public const string FinishTime = "FinishTime";
        /// <summary>描述</summary>
        public const string DescribeChar = "DescribeChar";
        /// <summary>染固色代码</summary>
        public const string DyeingCode = "DyeingCode";
        /// <summary>步号</summary>
        public const string Step = "Step";
        /// <summary>非脱水水比</summary>
        public const string Non_AnhydrationWR = "Non_AnhydrationWR";
        /// <summary>脱水水比</summary>
        public const string AnhydrationWR = "AnhydrationWR";
        /// <summary>后处理浴比</summary>
        public const string HandleBathRatio = "HandleBathRatio";
        /// <summary>开始时间</summary>
        public const string StartTime = "StartTime";
        /// <summary>温度曲线</summary>
        public const string ProcessData = "ProcessData";
        /// <summary>温度描点</summary>
        public const string MarkStep = "MarkStep";
        /// <summary>类型</summary>
        public const string Stage = "Stage";
        /// <summary>光谱数据</summary>
        public const string Spectrum = "Spectrum";
        /// <summary>描述英文</summary>
        public const string DescribeChar_EN = "DescribeChar_EN";
        /// <summary>染色后处理浴比集合</summary>
        public const string HandleBRList = "HandleBRList";

        /// <summary>类型</summary>
        public const string Type = "Type";
        /// <summary>吸光度数据</summary>
        public const string Abs = "Abs";
        /// <summary>是否是标样</summary>
        public const string Stand = "Stand";
        /// <summary>开始波长</summary>
        public const string StartWave = "StartWave";
        /// <summary>结束波长</summary>
        public const string EndWave = "EndWave";
        /// <summary>波长间隔</summary>
        public const string IntWave = "IntWave";
    }

    /// <summary>
    /// Abs_process表字段常量定义
    /// </summary>
    public static class ABS_PROCESS
    {
        /// <summary>表名</summary>
        public const string TableName = "Abs_process";
        /// <summary>步号</summary>
        public const string StepNum = "StepNum";
        /// <summary>工艺</summary>
        public const string TechnologyName = "TechnologyName";
        /// <summary>搅拌速率</summary>
        public const string StirringRate = "StirringRate";
        /// <summary>搅拌时间</summary>
        public const string StirringTime = "StirringTime";
        /// <summary>排液时间</summary>
        public const string DrainTime = "DrainTime";
        /// <summary>排比色皿时间</summary>
        public const string ParallelizingDishTime = "ParallelizingDishTime";
        /// <summary>抽液时间</summary>
        public const string PumpingTime = "PumpingTime";
        /// <summary>开始波长</summary>
        public const string StartingWavelength = "StartingWavelength";
        /// <summary>结束波长</summary>
        public const string EndWavelength = "EndWavelength";
        /// <summary>波长间隔</summary>
        public const string WavelengthInterval = "WavelengthInterval";
        /// <summary>用量</summary>
        public const string Dosage = "Dosage";
        /// <summary>吸光度测量代码</summary>
        public const string Code = "Code";
    }

    /// <summary>
    /// abs_wait_list表字段常量定义
    /// </summary>
    public static class ABS_WAIT_LIST
    {
        /// <summary>表名</summary>
        public const string TableName = "abs_wait_list";
        /// <summary>序号</summary>
        public const string MyID = "MyID";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>序号</summary>
        public const string IndexNum = "IndexNum";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>类型</summary>
        public const string Type = "Type";
    }

    /// <summary>
    /// alarm_table表字段常量定义
    /// </summary>
    public static class ALARM_TABLE
    {
        /// <summary>表名</summary>
        public const string TableName = "alarm_table";
        /// <summary>ID</summary>
        public const string MyID = "MyID";
        /// <summary>日期</summary>
        public const string MyDate = "MyDate";
        /// <summary>时间</summary>
        public const string MyTime = "MyTime";
        /// <summary>报警头</summary>
        public const string AlarmHead = "AlarmHead";
        /// <summary>报警详情</summary>
        public const string AlarmDetails = "AlarmDetails";
    }

    /// <summary>
    /// assistant_details表字段常量定义
    /// </summary>
    public static class ASSISTANT_DETAILS
    {
        /// <summary>表名</summary>
        public const string TableName = "assistant_details";
        /// <summary>索引</summary>
        public const string ID = "ID";
        /// <summary>染助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>染助剂条形码</summary>
        public const string AssistantBarCode = "AssistantBarCode";
        /// <summary>染助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>染助剂类型</summary>
        public const string AssistantType = "AssistantType";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
        /// <summary>允许最低打色浓度</summary>
        public const string AllowMinColoringConcentration = "AllowMinColoringConcentration";
        /// <summary>允许最高打色浓度</summary>
        public const string AllowMaxColoringConcentration = "AllowMaxColoringConcentration";
        /// <summary>有效期限(小时)</summary>
        public const string TermOfValidity = "TermOfValidity";
        /// <summary>力度</summary>
        public const string Intensity = "Intensity";
        /// <summary>成本</summary>
        public const string Cost = "Cost";
        /// <summary>校正值</summary>
        public const string Correcting = "Correcting";
        /// <summary>染色工艺滴液时是否复称</summary>
        public const string Reweigh = "Reweigh";
    }

    /// <summary>
    /// bottle_check表字段常量定义
    /// </summary>
    public static class BOTTLE_CHECK
    {
        /// <summary>表名</summary>
        public const string TableName = "bottle_check";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>完成标志位</summary>
        public const string Finish = "Finish";
        /// <summary>成功标志位</summary>
        public const string Successed = "Successed";
    }

    /// <summary>
    /// bottle_details表字段常量定义
    /// </summary>
    public static class BOTTLE_DETAILS
    {
        /// <summary>表名</summary>
        public const string TableName = "bottle_details";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>染助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>上次针检重量</summary>
        public const string LastAdjustWeight = "LastAdjustWeight";
        /// <summary>当前针检重量</summary>
        public const string CurrentAdjustWeight = "CurrentAdjustWeight";
        /// <summary>校正值</summary>
        public const string AdjustValue = "AdjustValue";
        /// <summary>泡制流程代码</summary>
        public const string BrewingCode = "BrewingCode";
        /// <summary>允许最大调液量</summary>
        public const string AllowMaxWeight = "AllowMaxWeight";
        /// <summary>当前库存量</summary>
        public const string CurrentWeight = "CurrentWeight";
        /// <summary>调液日期</summary>
        public const string BrewingData = "BrewingData";
        /// <summary>针筒类型</summary>
        public const string SyringeType = "SyringeType";
        /// <summary>最小滴液量</summary>
        public const string DropMinWeight = "DropMinWeight";
        /// <summary>开稀原瓶号</summary>
        public const string OriginalBottleNum = "OriginalBottleNum";
        /// <summary>针检完成</summary>
        public const string AdjustSuccess = "AdjustSuccess";
        /// <summary>自检11g数据</summary>
        public const string SelfChecking1 = "SelfChecking1";
        /// <summary>自检5g数据</summary>
        public const string SelfChecking2 = "SelfChecking2";
        /// <summary>自检2g数据</summary>
        public const string SelfChecking3 = "SelfChecking3";
        /// <summary>自检0.5g数据</summary>
        public const string SelfChecking4 = "SelfChecking4";
        /// <summary>吸光度数据</summary>
        public const string Abs = "Abs";
        /// <summary>修正系数</summary>
        public const string Compensate = "Compensate";
        /// <summary>状态</summary>
        public const string Status = "Status";
        /// <summary>是否预滴液</summary>
        public const string DripReserveFirst = "DripReserveFirst";
        /// <summary>洗针间隔</summary>
        public const string WashSyringeSpan = "WashSyringeSpan";
        /// <summary>排空间隔</summary>
        public const string EvacuateSpan = "EvacuateSpan";
        /// <summary>上次洗针时间</summary>
        public const string LastWashTime = "LastWashTime";
        /// <summary>吸光度测量代码</summary>
        public const string AbsCode = "AbsCode";
        /// <summary>开始波长</summary>
        public const string StartingWavelength = "StartingWavelength";
        /// <summary>结束波长</summary>
        public const string EndWavelength = "EndWavelength";
        /// <summary>波长间隔</summary>
        public const string WavelengthInterval = "WavelengthInterval";
        /// <summary>上次使用时间</summary>
        public const string LastUseTime = "LastUseTime";
    }

    /// <summary>
    /// brew_run_table表字段常量定义
    /// </summary>
    public static class BREW_RUN_TABLE
    {
        /// <summary>表名</summary>
        public const string TableName = "brew_run_table";
        /// <summary>ID</summary>
        public const string MyID = "MyID";
        /// <summary>开始时间</summary>
        public const string StartDateTime = "StartDateTime";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>调液流程代码</summary>
        public const string BrewCode = "BrewCode";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>允许最大调液量</summary>
        public const string AllowMaxWeight = "AllowMaxWeight";
        /// <summary>开稀原瓶号</summary>
        public const string OriginalBottleNum = "OriginalBottleNum";
        /// <summary>开稀原浓度</summary>
        public const string OriginalConcentration = "OriginalConcentration";
        /// <summary>输入模式</summary>
        public const string InputMode = "InputMode";
        /// <summary>染助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>完成时间</summary>
        public const string FinishDateTime = "FinishDateTime";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>实际液量</summary>
        public const string CurrentWeight = "CurrentWeight";
        /// <summary>总用时</summary>
        public const string UseingTime = "UseingTime";
        /// <summary>停止原因</summary>
        public const string ReasonCessation = "ReasonCessation";
    }

    /// <summary>
    /// brewing_code表字段常量定义
    /// </summary>
    public static class BREWING_CODE
    {
        /// <summary>表名</summary>
        public const string TableName = "brewing_code";
        /// <summary>调液流程代码</summary>
        public const string BrewingCode = "BrewingCode";
    }

    /// <summary>
    /// brewing_process表字段常量定义
    /// </summary>
    public static class BREWING_PROCESS
    {
        /// <summary>表名</summary>
        public const string TableName = "brewing_process";
        /// <summary>步号</summary>
        public const string StepNum = "StepNum";
        /// <summary>工艺名称</summary>
        public const string TechnologyName = "TechnologyName";
        /// <summary>比例(%)/时间(s)</summary>
        public const string ProportionOrTime = "ProportionOrTime";
        /// <summary>调液流程代码</summary>
        public const string BrewingCode = "BrewingCode";
        /// <summary>热水占比</summary>
        public const string Ratio = "Ratio";
    }

    /// <summary>
    /// cup_details表字段常量定义
    /// </summary>
    public static class CUP_DETAILS
    {
        /// <summary>表名</summary>
        public const string TableName = "cup_details";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>主副杯号</summary>
        public const string MainCupNum = "MainCupNum";
        /// <summary>是否固定色</summary>
        public const string IsFixed = "IsFixed";
        /// <summary>是否可用</summary>
        public const string Enable = "Enable";
        /// <summary>是否正在使用</summary>
        public const string IsUsing = "IsUsing";
        /// <summary>状态</summary>
        public const string Statues = "Statues";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>染固色代码</summary>
        public const string DyeingCode = "DyeingCode";
        /// <summary>开始时间</summary>
        public const string StartTime = "StartTime";
        /// <summary>设定温度</summary>
        public const string SetTemp = "SetTemp";
        /// <summary>实际温度</summary>
        public const string RealTemp = "RealTemp";
        /// <summary>总量</summary>
        public const string TotalWeight = "TotalWeight";
        /// <summary>当前量</summary>
        public const string CurrentWeight = "CurrentWeight";
        /// <summary>当前步号</summary>
        public const string StepNum = "StepNum";
        /// <summary>总步数</summary>
        public const string TotalStep = "TotalStep";
        /// <summary>工艺</summary>
        public const string TechnologyName = "TechnologyName";
        /// <summary>当前步开始时间</summary>
        public const string StepStartTime = "StepStartTime";
        /// <summary>设定时间</summary>
        public const string SetTime = "SetTime";
        /// <summary>当前步开始时对应的温度点索引</summary>
        public const string RecordIndex = "RecordIndex";
        /// <summary>当前需要配合的类型：
        ///0:无 
        ///1:加药 
        ///2:泄压 
        ///3:流程加水
        ///4:洗杯加水
        ///5:关盖
        ///6:报警锁止
        ///7:开盖
        ///</summary>
        public const string Cooperate = "Cooperate";
        /// <summary>当前杯类型：
        ///0：无 
        ///1：前处理
        ///2：滴液  
        ///3：染固色
        ///4: ABS杯
        ///</summary>
        public const string Type = "Type";
        /// <summary>杯盖状态 1:关盖 2：开盖</summary>
        public const string CoverStatus = "CoverStatus";

        /// <summary>是否失败</summary>
        public const string Fail = "Fail";
        /// <summary>收到申请时间</summary>
        public const string ReceptionTime = "ReceptionTime";
        /// <summary>染色后处理类型</summary>
        public const string DyeType = "DyeType";
        /// <summary>配方表头ID</summary>
        public const string HeadID = "HeadID";
        /// <summary>是否有布</summary>
        public const string HaveCloth = "HaveCloth";

        /// <summary>当前步是否完成</summary>
        public const string CurrentStepFinish = "CurrentStepFinish";


    }

    /// <summary>
    /// drop_details表字段常量定义
    /// </summary>
    public static class DROP_DETAILS
    {
        /// <summary>表名</summary>
        public const string TableName = "drop_details";
        /// <summary>表头序号</summary>
        public const string HeadID = "HeadID";
        /// <summary>批次号</summary>
        public const string BatchName = "BatchName";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本号</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>序号</summary>
        public const string IndexNum = "IndexNum";
        /// <summary>染助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>配方用量</summary>
        public const string FormulaDosage = "FormulaDosage";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>染助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>目标滴液重</summary>
        public const string ObjectDropWeight = "ObjectDropWeight";
        /// <summary>实际滴液重</summary>
        public const string RealDropWeight = "RealDropWeight";
        /// <summary>手动选瓶</summary>
        public const string BottleSelection = "BottleSelection";
        /// <summary>液量低</summary>
        public const string MinWeight = "MinWeight";
        /// <summary>完成</summary>
        public const string Finish = "Finish";
        /// <summary>目标粉重</summary>
        public const string ObjectPowderWeight = "ObjectPowderWeight";
        /// <summary>实际粉重</summary>
        public const string RealPowderWeight = "RealPowderWeight";
        /// <summary>是否显示</summary>
        public const string IsShow = "IsShow";
        /// <summary>是否已经滴液</summary>
        public const string IsDrop = "IsDrop";
        /// <summary>开料日期</summary>
        public const string BrewingData = "BrewingData";
        /// <summary>需加脉冲</summary>
        public const string NeedPulse = "NeedPulse";
        /// <summary>滴液误差</summary>
        public const string StandError = "StandError";
    }

    /// <summary>
    /// drop_head表字段常量定义
    /// </summary>
    public static class DROP_HEAD
    {
        /// <summary>表名</summary>
        public const string TableName = "drop_head";
        /// <summary>序号</summary>
        public const string MyID = "MyID";
        /// <summary>批次号</summary>
        public const string BatchName = "BatchName";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>状态</summary>
        public const string State = "State";
        /// <summary>配方名称</summary>
        public const string FormulaName = "FormulaName";
        /// <summary>布种/基材</summary>
        public const string ClothType = "ClothType";
        /// <summary>客户</summary>
        public const string Customer = "Customer";
        /// <summary>是否加水</summary>
        public const string AddWaterChoose = "AddWaterChoose";
        /// <summary>是否复版</summary>
        public const string CompoundBoardChoose = "CompoundBoardChoose";
        /// <summary>布重</summary>
        public const string ClothWeight = "ClothWeight";
        /// <summary>浴比</summary>
        public const string BathRatio = "BathRatio";
        /// <summary>总浴量</summary>
        public const string TotalWeight = "TotalWeight";
        /// <summary>操作员</summary>
        public const string Operator = "Operator";
        /// <summary>染杯代码</summary>
        public const string CupCode = "CupCode";
        /// <summary>存档时间</summary>
        public const string CreateTime = "CreateTime";
        /// <summary>目标加水重量</summary>
        public const string ObjectAddWaterWeight = "ObjectAddWaterWeight";
        /// <summary>实际加水重量</summary>
        public const string RealAddWaterWeight = "RealAddWaterWeight";
        /// <summary>试管目标加水量</summary>
        public const string TestTubeObjectAddWaterWeight = "TestTubeObjectAddWaterWeight";
        /// <summary>试管实际加水量</summary>
        public const string TestTubeRealAddWaterWeight = "TestTubeRealAddWaterWeight";
        /// <summary>试管加水完成</summary>
        public const string TestTubeFinish = "TestTubeFinish";
        /// <summary>试管液量低标志位</summary>
        public const string TestTubeWaterLower = "TestTubeWaterLower";
        /// <summary>加水完成</summary>
        public const string AddWaterFinish = "AddWaterFinish";
        /// <summary>杯完成</summary>
        public const string CupFinish = "CupFinish";
        /// <summary>染固色代码</summary>
        public const string DyeingCode = "DyeingCode";
        /// <summary>步号</summary>
        public const string Step = "Step";
        /// <summary>非脱水水比</summary>
        public const string Non_AnhydrationWR = "Non_AnhydrationWR";
        /// <summary>脱水水比</summary>
        public const string AnhydrationWR = "AnhydrationWR";
        /// <summary>后处理浴比</summary>
        public const string HandleBathRatio = "HandleBathRatio";
        /// <summary>转速1</summary>
        public const string Handle_Rev1 = "Handle_Rev1";
        /// <summary>转速2</summary>
        public const string Handle_Rev2 = "Handle_Rev2";
        /// <summary>转速3</summary>
        public const string Handle_Rev3 = "Handle_Rev3";
        /// <summary>转速4</summary>
        public const string Handle_Rev4 = "Handle_Rev4";
        /// <summary>转速5</summary>
        public const string Handle_Rev5 = "Handle_Rev5";
        /// <summary>描述</summary>
        public const string DescribeChar = "DescribeChar";
        /// <summary>完成时间</summary>
        public const string FinishTime = "FinishTime";
        /// <summary>开始时间</summary>
        public const string StartTime = "StartTime";
        /// <summary>类型</summary>
        public const string Stage = "Stage";
        /// <summary>描述英文</summary>
        public const string DescribeChar_EN = "DescribeChar_EN";
        /// <summary>染色后处理浴比集合</summary>
        public const string HandleBRList = "HandleBRList";
        /// <summary>加水误差</summary>
        public const string WaterStandError = "WaterStandError";
        /// <summary>是否自动放布</summary>
        public const string IsAutoIn = "IsAutoIn";
        /// <summary>放布位</summary>
        public const string ClothNum = "ClothNum";
        /// <summary>染固色代码备注</summary>
        public const string DyeingCodeRemark = "DyeingCodeRemark";
        /// <summary>复色原因</summary>
        public const string Recoloration = "Recoloration";
        /// <summary>缸位</summary>
        public const string VatNumber = "VatNumber";
        /// <summary>备注1</summary>
        public const string Note1 = "Note1";
        /// <summary>备注2</summary>
        public const string Note2 = "Note2";
        /// <summary>备注3</summary>
        public const string Note3 = "Note3";
    }

    /// <summary>
    /// dye_details表字段常量定义
    /// </summary>
    public static class DYE_DETAILS
    {
        /// <summary>表名</summary>
        public const string TableName = "dye_details";
        /// <summary>表头序号</summary>
        public const string HeadID = "HeadID";
        /// <summary>批次号</summary>
        public const string BatchName = "BatchName";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本号</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>配方用量</summary>
        public const string FormulaDosage = "FormulaDosage";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>目标滴液量</summary>
        public const string ObjectDropWeight = "ObjectDropWeight";
        /// <summary>实际滴液量</summary>
        public const string RealDropWeight = "RealDropWeight";
        /// <summary>是否收到选瓶</summary>
        public const string BottleSelection = "BottleSelection";
        /// <summary>类型</summary>
        public const string MinWeight = "MinWeight";
        /// <summary>是否完成</summary>
        public const string Finish = "Finish";
        /// <summary>步号</summary>
        public const string StepNum = "StepNum";
        /// <summary>工艺</summary>
        public const string TechnologyName = "TechnologyName";
        /// <summary>设定温度</summary>
        public const string Temp = "Temp";
        /// <summary>升温速率</summary>
        public const string TempSpeed = "TempSpeed";
        /// <summary>保温时间</summary>
        public const string Time = "Time";
        /// <summary>目标加水量</summary>
        public const string ObjectWaterWeight = "ObjectWaterWeight";
        /// <summary>转速</summary>
        public const string RotorSpeed = "RotorSpeed";
        /// <summary>超温次数</summary>
        public const string OvertempNum = "OvertempNum";
        /// <summary>超温时间</summary>
        public const string OvertempTime = "OvertempTime";
        /// <summary>工艺名称</summary>
        public const string Code = "Code";
        /// <summary>开始时间</summary>
        public const string StartTime = "StartTime";
        /// <summary>完成时间</summary>
        public const string FinishTime = "FinishTime";
        /// <summary>操作</summary>
        public const string Cooperate = "Cooperate";
        /// <summary>补偿系数</summary>
        public const string Compensation = "Compensation";
        /// <summary>开料日期</summary>
        public const string BrewingData = "BrewingData";
        /// <summary>接收信号时间</summary>
        public const string ReceptionTime = "ReceptionTime";
        /// <summary>类型</summary>
        public const string DyeType = "DyeType";
        /// <summary>需加脉冲</summary>
        public const string NeedPulse = "NeedPulse";
        /// <summary>是否加水完成</summary>
        public const string WaterFinish = "WaterFinish";
        /// <summary>待办事项选择结果</summary>
        public const string Choose = "Choose";
        /// <summary>滴液误差</summary>
        public const string StandError = "StandError";
        /// <summary>顺序号</summary>
        public const string IndexNum = "IndexNum";
    }

    /// <summary>
    /// dyeing_code表字段常量定义
    /// </summary>
    public static class DYEING_CODE
    {
        /// <summary>表名</summary>
        public const string TableName = "dyeing_code";
        /// <summary>染固色代码</summary>
        public const string DyeingCode = "DyeingCode";
        /// <summary>类型</summary>
        public const string Type = "Type";
        /// <summary>步号</summary>
        public const string Step = "Step";
        /// <summary>染色后处理工艺</summary>
        public const string Code = "Code";
        /// <summary>顺序号</summary>
        public const string IndexNum = "IndexNum";
        /// <summary>是否可用</summary>
        public const string IsUse = "IsUse";
        /// <summary>备注</summary>
        public const string Remark = "Remark";
    }

    /// <summary>
    /// history_dyeing_code表字段常量定义
    /// </summary>
    public static class HISTORY_DYEING_CODE
    {
        /// <summary>表名</summary>
        public const string TableName = "history_dyeing_code";
        /// <summary>染固色代码</summary>
        public const string DyeingCode = "DyeingCode";
        /// <summary>类型</summary>
        public const string Type = "Type";
        /// <summary>步号</summary>
        public const string Step = "Step";
        /// <summary>染色后处理工艺</summary>
        public const string Code = "Code";
        /// <summary>顺序号</summary>
        public const string IndexNum = "IndexNum";
        /// <summary>是否可用</summary>
        public const string IsUse = "IsUse";
        /// <summary>备注</summary>
        public const string Remark = "Remark";
    }

    /// <summary>
    /// dyeing_details表字段常量定义
    /// </summary>
    public static class DYEING_DETAILS
    {
        /// <summary>表名</summary>
        public const string TableName = "dyeing_details";
        /// <summary>批次号</summary>
        public const string BatchName = "BatchName";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本号</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>配方用量</summary>
        public const string FormulaDosage = "FormulaDosage";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>目标滴液量</summary>
        public const string ObjectDropWeight = "ObjectDropWeight";
        /// <summary>实际滴液量</summary>
        public const string RealDropWeight = "RealDropWeight";
        /// <summary>是否自动选瓶</summary>
        public const string BottleSelection = "BottleSelection";
        /// <summary>类型</summary>
        public const string MinWeight = "MinWeight";
        /// <summary>是否完成</summary>
        public const string Finish = "Finish";
        /// <summary>步号</summary>
        public const string StepNum = "StepNum";
        /// <summary>工艺</summary>
        public const string TechnologyName = "TechnologyName";
        /// <summary>设定温度</summary>
        public const string Temp = "Temp";
        /// <summary>升温速率</summary>
        public const string TempSpeed = "TempSpeed";
        /// <summary>保温时间</summary>
        public const string Time = "Time";
        /// <summary>目标加水量</summary>
        public const string ObjectWaterWeight = "ObjectWaterWeight";
        /// <summary>转速</summary>
        public const string RotorSpeed = "RotorSpeed";
        /// <summary>超温次数</summary>
        public const string OvertempNum = "OvertempNum";
        /// <summary>超温时间</summary>
        public const string OvertempTime = "OvertempTime";
        /// <summary>染色后处理工艺</summary>
        public const string Code = "Code";
        /// <summary>开始时间</summary>
        public const string StartTime = "StartTime";
        /// <summary>完成时间</summary>
        public const string FinishTime = "FinishTime";
        /// <summary>操作</summary>
        public const string Cooperate = "Cooperate";
        /// <summary>补偿系数</summary>
        public const string Compensation = "Compensation";
        /// <summary>开料日期</summary>
        public const string BrewingData = "BrewingData";
        /// <summary>接收信号时间</summary>
        public const string ReceptionTime = "ReceptionTime";
        /// <summary>类型</summary>
        public const string DyeType = "DyeType";
        /// <summary>需加脉冲</summary>
        public const string NeedPulse = "NeedPulse";
        /// <summary>待办事项选择结果</summary>
        public const string Choose = "Choose";
        /// <summary>加水是否完成</summary>
        public const string WaterFinish = "WaterFinish";
        /// <summary>序号</summary>
        public const string No = "No";
    }

    /// <summary>
    /// dyeing_process表字段常量定义
    /// </summary>
    public static class DYEING_PROCESS
    {
        /// <summary>表名</summary>
        public const string TableName = "dyeing_process";
        /// <summary>步号</summary>
        public const string StepNum = "StepNum";
        /// <summary>工艺</summary>
        public const string TechnologyName = "TechnologyName";
        /// <summary>比例(%)/时间(s)</summary>
        public const string ProportionOrTime = "ProportionOrTime";
        /// <summary>染色后处理工艺</summary>
        public const string Code = "Code";
        /// <summary>设定温度</summary>
        public const string Temp = "Temp";
        /// <summary>速率</summary>
        public const string Rate = "Rate";
        /// <summary>类型</summary>
        public const string Type = "Type";
        /// <summary>转速</summary>
        public const string Rev = "Rev";
        /// <summary>备注</summary>
        public const string Remark = "Remark";
        /// <summary>是否关盖加药</summary>
        public const string OpenMedicine = "OpenMedicine";
    }

    /// <summary>
    /// history_dyeing_process表字段常量定义
    /// </summary>
    public static class HISTORY_DYEING_PROCESS
    {
        /// <summary>表名</summary>
        public const string TableName = "history_dyeing_process";
        /// <summary>步号</summary>
        public const string StepNum = "StepNum";
        /// <summary>工艺</summary>
        public const string TechnologyName = "TechnologyName";
        /// <summary>比例(%)/时间(s)</summary>
        public const string ProportionOrTime = "ProportionOrTime";
        /// <summary>染色后处理工艺</summary>
        public const string Code = "Code";
        /// <summary>设定温度</summary>
        public const string Temp = "Temp";
        /// <summary>速率</summary>
        public const string Rate = "Rate";
        /// <summary>类型</summary>
        public const string Type = "Type";
        /// <summary>转速</summary>
        public const string Rev = "Rev";
        /// <summary>备注</summary>
        public const string Remark = "Remark";
        /// <summary>是否关盖加药</summary>
        public const string OpenMedicine = "OpenMedicine";
    }

    /// <summary>
    /// dyeing_Remark表字段常量定义
    /// </summary>
    public static class DYEING_REMARK
    {
        /// <summary>表名</summary>
        public const string TableName = "dyeing_Remark";
        /// <summary>代码</summary>
        public const string Code = "Code";
        /// <summary>备注</summary>
        public const string Remark = "Remark";
    }

    /// <summary>
    /// enabled_set表字段常量定义
    /// </summary>
    public static class ENABLED_SET
    {
        /// <summary>表名</summary>
        public const string TableName = "enabled_set";
        /// <summary>自增</summary>
        public const string MyID = "MyID";
        /// <summary>配方名称</summary>
        public const string Txt_FormulaName = "txt_FormulaName";
        /// <summary>布种/基材</summary>
        public const string Txt_ClothType = "txt_ClothType";
        /// <summary>客户</summary>
        public const string Txt_Customer = "txt_Customer";
        /// <summary>是否加水</summary>
        public const string Chk_AddWaterChoose = "txt_AddWaterChoose";
        /// <summary>是否复板</summary>
        public const string Chk_CompoundBoardChoose = "chk_CompoundBoardChoose";
        /// <summary>布重</summary>
        public const string Txt_ClothWeight = "txt_ClothWeight";
        /// <summary>浴比</summary>
        public const string Txt_BathRatio = "txt_BathRatio";
        /// <summary>操作员</summary>
        public const string Txt_Operator = "txt_Operator";
        /// <summary>染杯代码</summary>
        public const string Txt_CupNum = "txt_CupNum";
        /// <summary>染固色代码</summary>
        public const string Txt_DyeingCode = "txt_DyeingCode";
        /// <summary>配方表</summary>
        public const string Dgv_FormulaData = "dgv_FormulaData";
        /// <summary>助剂</summary>
        public const string Dgv_AddAssistant = "dgv_AddAssistant";
        /// <summary>非脱水水比</summary>
        public const string Txt_Non_AnhydrationWR = "txt_Non_AnhydrationWR";
        /// <summary>脱水水比</summary>
        public const string Txt_AnhydrationWR = "txt_AnhydrationWR";
        /// <summary>后处理浴比</summary>
        public const string Txt_HandleBathRatio = "txt_HandleBathRatio";
        /// <summary>转速1</summary>
        public const string Txt_Handle_Rev1 = "txt_Handle_Rev1";
        /// <summary>转速2</summary>
        public const string Txt_Handle_Rev2 = "txt_Handle_Rev2";
        /// <summary>转速3</summary>
        public const string Txt_Handle_Rev3 = "txt_Handle_Rev3";
        /// <summary>转速4</summary>
        public const string Txt_Handle_Rev4 = "txt_Handle_Rev4";
        /// <summary>转速5</summary>
        public const string Txt_Handle_Rev5 = "txt_Handle_Rev5";
        /// <summary>后处理1</summary>
        public const string Dgv_Handle1 = "dgv_Handle1";
        /// <summary>后处理2</summary>
        public const string Dgv_Handle2 = "dgv_Handle2";
        /// <summary>后处理3</summary>
        public const string Dgv_Handle3 = "dgv_Handle3";
        /// <summary>后处理4</summary>
        public const string Dgv_Handle4 = "dgv_Handle4";
        /// <summary>后处理5</summary>
        public const string Dgv_Handle5 = "dgv_Handle5";
        /// <summary>后处理</summary>
        public const string Dgv_Dye = "dgv_Dye";
        /// <summary>当前批次号</summary>
        public const string BatchName = "BatchName";
        /// <summary>类型</summary>
        public const string Txt_Stage = "txt_Stage";
        /// <summary>配方组合</summary>
        public const string Txt_FormulaGroup = "txt_FormulaGroup";
        /// <summary>染杯代码</summary>
        public const string Txt_CupCode = "txt_CupCode";
        /// <summary>复色原因</summary>
        public const string Txt_Recoloration = "txt_Recoloration";
        /// <summary>备注1</summary>
        public const string Txt_Note1 = "txt_Note1";
        /// <summary>备注2</summary>
        public const string Txt_Note2 = "txt_Note2";
        /// <summary>备注3</summary>
        public const string Txt_Note3 = "txt_Note3";
        /// <summary>放布位</summary>
        public const string Txt_ClothNum = "txt_ClothNum";
        /// <summary>自动出放布</summary>
        public const string Chk_Auto = "txt_IsAutoIn";
        /// <summary>备注1名称</summary>
        public const string Note1Name = "Note1Name";
        /// <summary>备注1可选项</summary>
        public const string Note1Items = "Note1Items";
        /// <summary>备注2名称</summary>
        public const string Note2Name = "Note2Name";
        /// <summary>备注2可选项</summary>
        public const string Note2Items = "Note2Items";
        /// <summary>备注3名称</summary>
        public const string Note3Name = "Note3Name";
        /// <summary>备注3可选项</summary>
        public const string Note3Items = "Note3Items";
    }

    /// <summary>
    /// abs_enabled_set表字段常量定义
    /// </summary>
    public static class ABS_ENABLED_SET
    {
        /// <summary>表名</summary>
        public const string TableName = "abs_enabled_set";
        /// <summary>自增</summary>
        public const string MyID = "MyID";
        /// <summary>配方名称</summary>
        public const string Txt_FormulaName = "txt_FormulaName";
        /// <summary>布种/基材</summary>
        public const string Txt_ClothType = "txt_ClothType";
        /// <summary>客户</summary>
        public const string Txt_Customer = "txt_Customer";
        /// <summary>是否加水</summary>
        public const string Chk_AddWaterChoose = "txt_AddWaterChoose";
        /// <summary>是否复板</summary>
        public const string Chk_CompoundBoardChoose = "chk_CompoundBoardChoose";
        /// <summary>布重</summary>
        public const string Txt_ClothWeight = "txt_ClothWeight";
        /// <summary>浴比</summary>
        public const string Txt_BathRatio = "txt_BathRatio";
        /// <summary>操作员</summary>
        public const string Txt_Operator = "txt_Operator";
        /// <summary>染杯代码</summary>
        public const string Txt_CupNum = "txt_CupNum";
        /// <summary>染固色代码</summary>
        public const string Txt_DyeingCode = "txt_DyeingCode";
        /// <summary>配方表</summary>
        public const string Dgv_FormulaData = "dgv_FormulaData";
        /// <summary>助剂</summary>
        public const string Dgv_AddAssistant = "dgv_AddAssistant";
        /// <summary>非脱水水比</summary>
        public const string Txt_Non_AnhydrationWR = "txt_Non_AnhydrationWR";
        /// <summary>脱水水比</summary>
        public const string Txt_AnhydrationWR = "txt_AnhydrationWR";
        /// <summary>后处理浴比</summary>
        public const string Txt_HandleBathRatio = "txt_HandleBathRatio";
        /// <summary>转速1</summary>
        public const string Txt_Handle_Rev1 = "txt_Handle_Rev1";
        /// <summary>转速2</summary>
        public const string Txt_Handle_Rev2 = "txt_Handle_Rev2";
        /// <summary>转速3</summary>
        public const string Txt_Handle_Rev3 = "txt_Handle_Rev3";
        /// <summary>转速4</summary>
        public const string Txt_Handle_Rev4 = "txt_Handle_Rev4";
        /// <summary>转速5</summary>
        public const string Txt_Handle_Rev5 = "txt_Handle_Rev5";
        /// <summary>后处理1</summary>
        public const string Dgv_Handle1 = "dgv_Handle1";
        /// <summary>后处理2</summary>
        public const string Dgv_Handle2 = "dgv_Handle2";
        /// <summary>后处理3</summary>
        public const string Dgv_Handle3 = "dgv_Handle3";
        /// <summary>后处理4</summary>
        public const string Dgv_Handle4 = "dgv_Handle4";
        /// <summary>后处理5</summary>
        public const string Dgv_Handle5 = "dgv_Handle5";
        /// <summary>后处理</summary>
        public const string Dgv_Dye = "dgv_Dye";
        /// <summary>当前批次号</summary>
        public const string BatchName = "BatchName";
        /// <summary>类型</summary>
        public const string Txt_Stage = "txt_Stage";
        /// <summary>配方组合</summary>
        public const string Txt_FormulaGroup = "txt_FormulaGroup";
        /// <summary>染杯代码</summary>
        public const string Txt_CupCode = "txt_CupCode";
        /// <summary>复色原因</summary>
        public const string Txt_Recoloration = "txt_Recoloration";
        /// <summary>备注1</summary>
        public const string Txt_Note1 = "txt_Note1";
        /// <summary>备注2</summary>
        public const string Txt_Note2 = "txt_Note2";
        /// <summary>备注3</summary>
        public const string Txt_Note3 = "txt_Note3";
        /// <summary>放布位</summary>
        public const string Txt_ClothNum = "txt_ClothNum";
        /// <summary>自动出放布</summary>
        public const string Chk_Auto = "txt_IsAutoIn";
        /// <summary>备注1名称</summary>
        public const string Note1Name = "Note1Name";
        /// <summary>备注1可选项</summary>
        public const string Note1Items = "Note1Items";
        /// <summary>备注2名称</summary>
        public const string Note2Name = "Note2Name";
        /// <summary>备注2可选项</summary>
        public const string Note2Items = "Note2Items";
        /// <summary>备注3名称</summary>
        public const string Note3Name = "Note3Name";
        /// <summary>备注3可选项</summary>
        public const string Note3Items = "Note3Items";
    }

    /// <summary>
    /// formula_details表字段常量定义
    /// </summary>
    public static class FORMULA_DETAILS
    {
        /// <summary>表名</summary>
        public const string TableName = "formula_details";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本号</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>序号</summary>
        public const string IndexNum = "IndexNum";
        /// <summary>染助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>配方用量</summary>
        public const string FormulaDosage = "FormulaDosage";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>染助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>目标滴液重</summary>
        public const string ObjectDropWeight = "ObjectDropWeight";
        /// <summary>实际滴液量</summary>
        public const string RealDropWeight = "RealDropWeight";
        /// <summary>手动选瓶</summary>
        public const string BottleSelection = "BottleSelection";
        /// <summary>目标粉重</summary>
        public const string ObjectPowderWeight = "ObjectPowderWeight";
        /// <summary>实际粉重</summary>
        public const string RealPowderWeight = "RealPowderWeight";
        /// <summary>是否显示</summary>
        public const string IsShow = "IsShow";
        /// <summary>开料日期</summary>
        public const string BrewingData = "BrewingData";
    }

    /// <summary>
    /// formula_details_temp表字段常量定义
    /// </summary>
    public static class FORMULA_DETAILS_TEMP
    {
        /// <summary>表名</summary>
        public const string TableName = "formula_details_temp";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本号</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>序号</summary>
        public const string IndexNum = "IndexNum";
        /// <summary>助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>配方用量</summary>
        public const string FormulaDosage = "FormulaDosage";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>目标滴液量</summary>
        public const string ObjectDropWeight = "ObjectDropWeight";
        /// <summary>实际滴液量</summary>
        public const string RealDropWeight = "RealDropWeight";
        /// <summary>是否收到选瓶</summary>
        public const string BottleSelection = "BottleSelection";
        /// <summary>目标加粉量</summary>
        public const string ObjectPowderWeight = "ObjectPowderWeight";
        /// <summary>实际加粉量</summary>
        public const string RealPowderWeight = "RealPowderWeight";
        /// <summary>是否显示</summary>
        public const string IsShow = "IsShow";
        /// <summary>开料日期</summary>
        public const string BrewingData = "BrewingData";
    }

    /// <summary>
    /// formula_group表字段常量定义
    /// </summary>
    public static class FORMULA_GROUP
    {
        /// <summary>表名</summary>
        public const string TableName = "formula_group";
        /// <summary>ID</summary>
        public const string Id = "Id";
        /// <summary>配方组合</summary>
        public const string GroupName = "group_Name";
        /// <summary>序号</summary>
        public const string Node = "node";
        /// <summary>助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>创建时间</summary>
        public const string CreateTime = "createTime";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
    }

    /// <summary>
    /// formula_handle_details表字段常量定义
    /// </summary>
    public static class FORMULA_HANDLE_DETAILS
    {
        /// <summary>表名</summary>
        public const string TableName = "formula_handle_details";
        /// <summary>染固色代码</summary>
        public const string DyeingCode = "DyeingCode";
        /// <summary>染色后处理工艺</summary>
        public const string Code = "Code";
        /// <summary>工艺</summary>
        public const string TechnologyName = "TechnologyName";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本号</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>配方用量</summary>
        public const string FormulaDosage = "FormulaDosage";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>目标滴液量</summary>
        public const string ObjectDropWeight = "ObjectDropWeight";
        /// <summary>实际滴液量</summary>
        public const string RealDropWeight = "RealDropWeight";
        /// <summary>是否自动选瓶</summary>
        public const string BottleSelection = "BottleSelection";
        /// <summary>类型</summary>
        public const string MinWeight = "MinWeight";
        /// <summary>是否完成</summary>
        public const string Finish = "Finish";
        /// <summary>序号</summary>
        public const string No = "No";
    }

    /// <summary>
    /// formula_handle_details_temp表字段常量定义
    /// </summary>
    public static class FORMULA_HANDLE_DETAILS_TEMP
    {
        /// <summary>表名</summary>
        public const string TableName = "formula_handle_details_temp";
        /// <summary>染固色代码</summary>
        public const string DyeingCode = "DyeingCode";
        /// <summary>染色后处理名称</summary>
        public const string Code = "Code";
        /// <summary>工艺</summary>
        public const string TechnologyName = "TechnologyName";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本号</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>用量</summary>
        public const string FormulaDosage = "FormulaDosage";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>目标滴液量</summary>
        public const string ObjectDropWeight = "ObjectDropWeight";
        /// <summary>实际滴液量</summary>
        public const string RealDropWeight = "RealDropWeight";
        /// <summary>瓶选择</summary>
        public const string BottleSelection = "BottleSelection";
        /// <summary>类型</summary>
        public const string MinWeight = "MinWeight";
        /// <summary>是否完成</summary>
        public const string Finish = "Finish";
        /// <summary>序号</summary>
        public const string No = "No";
    }

    /// <summary>
    /// formula_head表字段常量定义
    /// </summary>
    public static class FORMULA_HEAD
    {
        /// <summary>表名</summary>
        public const string TableName = "formula_head";
        /// <summary>ID</summary>
        public const string MyID = "MyID";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>状态</summary>
        public const string State = "State";
        /// <summary>配方名称</summary>
        public const string FormulaName = "FormulaName";
        /// <summary>布种/基材</summary>
        public const string ClothType = "ClothType";
        /// <summary>客户</summary>
        public const string Customer = "Customer";
        /// <summary>是否加水</summary>
        public const string AddWaterChoose = "AddWaterChoose";
        /// <summary>是否复板</summary>
        public const string CompoundBoardChoose = "CompoundBoardChoose";
        /// <summary>布重</summary>
        public const string ClothWeight = "ClothWeight";
        /// <summary>浴比</summary>
        public const string BathRatio = "BathRatio";
        /// <summary>总浴量</summary>
        public const string TotalWeight = "TotalWeight";
        /// <summary>操作员</summary>
        public const string Operator = "Operator";
        /// <summary>染杯代码</summary>
        public const string CupCode = "CupCode";
        /// <summary>存档时间</summary>
        public const string CreateTime = "CreateTime";
        /// <summary>目标加水重量</summary>
        public const string ObjectAddWaterWeight = "ObjectAddWaterWeight";
        /// <summary>实际加水重量</summary>
        public const string RealAddWaterWeight = "RealAddWaterWeight";
        /// <summary>试管目标加水量</summary>
        public const string TestTubeObjectAddWaterWeight = "TestTubeObjectAddWaterWeight";
        /// <summary>试管实际加水量</summary>
        public const string TestTubeRealAddWaterWeight = "TestTubeRealAddWaterWeight";
        /// <summary>染固色代码</summary>
        public const string DyeingCode = "DyeingCode";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>非脱水水比</summary>
        public const string Non_AnhydrationWR = "Non_AnhydrationWR";
        /// <summary>脱水水比</summary>
        public const string AnhydrationWR = "AnhydrationWR";
        /// <summary>后处理浴比</summary>
        public const string HandleBathRatio = "HandleBathRatio";
        /// <summary>转速1</summary>
        public const string Handle_Rev1 = "Handle_Rev1";
        /// <summary>转速2</summary>
        public const string Handle_Rev2 = "Handle_Rev2";
        /// <summary>转速3</summary>
        public const string Handle_Rev3 = "Handle_Rev3";
        /// <summary>转速4</summary>
        public const string Handle_Rev4 = "Handle_Rev4";
        /// <summary>转速5</summary>
        public const string Handle_Rev5 = "Handle_Rev5";
        /// <summary>类型</summary>
        public const string Stage = "Stage";
        /// <summary>染色后处理浴比集合</summary>
        public const string HandleBRList = "HandleBRList";
        /// <summary>放布位</summary>
        public const string ClothNum = "ClothNum";
        /// <summary>自动放布</summary>
        public const string IsAutoIn = "IsAutoIn";
        /// <summary>染固色代码备注</summary>
        public const string DyeingCodeRemark = "DyeingCodeRemark";
        /// <summary>复色原因</summary>
        public const string Recoloration = "Recoloration";
        /// <summary>备注1</summary>
        public const string Note1 = "Note1";
        /// <summary>备注2</summary>
        public const string Note2 = "Note2";
        /// <summary>备注3</summary>
        public const string Note3 = "Note3";
    }

    /// <summary>
    /// formula_head_temp表字段常量定义
    /// </summary>
    public static class FORMULA_HEAD_TEMP
    {
        /// <summary>表名</summary>
        public const string TableName = "formula_head_temp";
        /// <summary>序号</summary>
        public const string MyID = "MyID";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本号</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>滴液状态</summary>
        public const string State = "State";
        /// <summary>配方名称</summary>
        public const string FormulaName = "FormulaName";
        /// <summary>布种</summary>
        public const string ClothType = "ClothType";
        /// <summary>客户</summary>
        public const string Customer = "Customer";
        /// <summary>是否加水</summary>
        public const string AddWaterChoose = "AddWaterChoose";
        /// <summary>是否复板</summary>
        public const string CompoundBoardChoose = "CompoundBoardChoose";
        /// <summary>布重</summary>
        public const string ClothWeight = "ClothWeight";
        /// <summary>浴比</summary>
        public const string BathRatio = "BathRatio";
        /// <summary>总浴量</summary>
        public const string TotalWeight = "TotalWeight";
        /// <summary>操作员</summary>
        public const string Operator = "Operator";
        /// <summary>杯代码</summary>
        public const string CupCode = "CupCode";
        /// <summary>创建日期</summary>
        public const string CreateTime = "CreateTime";
        /// <summary>目标加水量</summary>
        public const string ObjectAddWaterWeight = "ObjectAddWaterWeight";
        /// <summary>实际加水量</summary>
        public const string RealAddWaterWeight = "RealAddWaterWeight";
        /// <summary>试管目标加水量</summary>
        public const string TestTubeObjectAddWaterWeight = "TestTubeObjectAddWaterWeight";
        /// <summary>试管实际加水量</summary>
        public const string TestTubeRealAddWaterWeight = "TestTubeRealAddWaterWeight";
        /// <summary>染固色代码</summary>
        public const string DyeingCode = "DyeingCode";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>非脱水水比</summary>
        public const string Non_AnhydrationWR = "Non_AnhydrationWR";
        /// <summary>脱水水比</summary>
        public const string AnhydrationWR = "AnhydrationWR";
        /// <summary>后处理浴比</summary>
        public const string HandleBathRatio = "HandleBathRatio";
        /// <summary>转速1</summary>
        public const string Handle_Rev1 = "Handle_Rev1";
        /// <summary>转速2</summary>
        public const string Handle_Rev2 = "Handle_Rev2";
        /// <summary>转速3</summary>
        public const string Handle_Rev3 = "Handle_Rev3";
        /// <summary>转速4</summary>
        public const string Handle_Rev4 = "Handle_Rev4";
        /// <summary>转速5</summary>
        public const string Handle_Rev5 = "Handle_Rev5";
        /// <summary>类型</summary>
        public const string Stage = "Stage";
        /// <summary>染色后处理浴比集合</summary>
        public const string HandleBRList = "HandleBRList";
        /// <summary>放布位</summary>
        public const string ClothNum = "ClothNum";
        /// <summary>自动放布</summary>
        public const string IsAutoIn = "IsAutoIn";
        /// <summary>备注1</summary>
        public const string Note1 = "Note1";
        /// <summary>备注2</summary>
        public const string Note2 = "Note2";
        /// <summary>备注3</summary>
        public const string Note3 = "Note3";
    }

    /// <summary>
    /// handle_process表字段常量定义
    /// </summary>
    public static class HANDLE_PROCESS
    {
        /// <summary>表名</summary>
        public const string TableName = "handle_process";
        /// <summary>步号</summary>
        public const string StepNum = "StepNum";
        /// <summary>工艺</summary>
        public const string TechnologyName = "TechnologyName";
        /// <summary>比例(%)/时间(s)</summary>
        public const string ProportionOrTime = "ProportionOrTime";
        /// <summary>染色后处理工艺</summary>
        public const string Code = "Code";
        /// <summary>设定温度</summary>
        public const string Temp = "Temp";
        /// <summary>速率</summary>
        public const string Rate = "Rate";
    }

    /// <summary>
    /// history_absstandard表字段常量定义
    /// </summary>
    public static class HISTORY_ABSSTANDARD
    {
        /// <summary>表名</summary>
        public const string TableName = "history_absstandard";
        /// <summary>E1</summary>
        public const string E1 = "E1";
        /// <summary>E2</summary>
        public const string E2 = "E2";
        /// <summary>波长</summary>
        public const string WL = "WL";
        /// <summary>完成时间</summary>
        public const string FinishTime = "FinishTime";
        /// <summary>类型</summary>
        public const string Type = "Type";
    }

    /// <summary>
    /// history_details表字段常量定义
    /// </summary>
    public static class HISTORY_DETAILS
    {
        /// <summary>表名</summary>
        public const string TableName = "history_details";
        /// <summary>表头序号</summary>
        public const string HeadID = "HeadID";
        /// <summary>批次号</summary>
        public const string BatchName = "BatchName";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本号</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>序号</summary>
        public const string IndexNum = "IndexNum";
        /// <summary>染助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>配方用量</summary>
        public const string FormulaDosage = "FormulaDosage";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>染助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>目标滴液量</summary>
        public const string ObjectDropWeight = "ObjectDropWeight";
        /// <summary>实际滴液量</summary>
        public const string RealDropWeight = "RealDropWeight";
        /// <summary>手动选瓶</summary>
        public const string BottleSelection = "BottleSelection";
        /// <summary>目标粉重</summary>
        public const string ObjectPowderWeight = "ObjectPowderWeight";
        /// <summary>实际粉重</summary>
        public const string RealPowderWeight = "RealPowderWeight";
        /// <summary>是否滴液</summary>
        public const string IsDrop = "IsDrop";
        /// <summary>开料日期</summary>
        public const string BrewingData = "BrewingData";
        /// <summary>滴液限值</summary>
        public const string StandError = "StandError";
    }

    /// <summary>
    /// history_dye表字段常量定义
    /// </summary>
    public static class HISTORY_DYE
    {
        /// <summary>表名</summary>
        public const string TableName = "history_dye";
        /// <summary>表头序号</summary>
        public const string HeadID = "HeadID";
        /// <summary>批次号</summary>
        public const string BatchName = "BatchName";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本号</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>染助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>配方用量</summary>
        public const string FormulaDosage = "FormulaDosage";
        /// <summary>计算单位</summary>
        public const string UnitOfAccount = "UnitOfAccount";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>设定浓度</summary>
        public const string SettingConcentration = "SettingConcentration";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>染助剂名称</summary>
        public const string AssistantName = "AssistantName";
        /// <summary>目标滴液量</summary>
        public const string ObjectDropWeight = "ObjectDropWeight";
        /// <summary>实际滴液量</summary>
        public const string RealDropWeight = "RealDropWeight";
        /// <summary>手动选瓶</summary>
        public const string BottleSelection = "BottleSelection";
        /// <summary>类型</summary>
        public const string MinWeight = "MinWeight";
        /// <summary>是否完成</summary>
        public const string Finish = "Finish";
        /// <summary>步号</summary>
        public const string StepNum = "StepNum";
        /// <summary>工艺名称</summary>
        public const string TechnologyName = "TechnologyName";
        /// <summary>设定温度</summary>
        public const string Temp = "Temp";
        /// <summary>升温速率</summary>
        public const string TempSpeed = "TempSpeed";
        /// <summary>保温时间</summary>
        public const string Time = "Time";
        /// <summary>目标加水量</summary>
        public const string ObjectWaterWeight = "ObjectWaterWeight";
        /// <summary>转速</summary>
        public const string RotorSpeed = "RotorSpeed";
        /// <summary>超温次数</summary>
        public const string OvertempNum = "OvertempNum";
        /// <summary>超温时间</summary>
        public const string OvertempTime = "OvertempTime";
        /// <summary>工艺代码</summary>
        public const string Code = "Code";
        /// <summary>开始时间</summary>
        public const string StartTime = "StartTime";
        /// <summary>结束时间</summary>
        public const string FinishTime = "FinishTime";
        /// <summary>补偿系数</summary>
        public const string Compensation = "Compensation";
        /// <summary>开料日期</summary>
        public const string BrewingData = "BrewingData";
        /// <summary>收到申请时间</summary>
        public const string ReceptionTime = "ReceptionTime";
        /// <summary>染色后处理类型</summary>
        public const string DyeType = "DyeType";
        /// <summary>滴液限值</summary>
        public const string StandError = "StandError";
        /// <summary>顺序号</summary>
        public const string No = "No";
    }

    /// <summary>
    /// history_head表字段常量定义
    /// </summary>
    public static class HISTORY_HEAD
    {
        /// <summary>表名</summary>
        public const string TableName = "history_head";
        /// <summary>ID</summary>
        public const string MyID = "MyID";
        /// <summary>批次号</summary>
        public const string BatchName = "BatchName";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>状态</summary>
        public const string State = "State";
        /// <summary>配方名称</summary>
        public const string FormulaName = "FormulaName";
        /// <summary>布种/基材</summary>
        public const string ClothType = "ClothType";
        /// <summary>客户</summary>
        public const string Customer = "Customer";
        /// <summary>是否加水</summary>
        public const string AddWaterChoose = "AddWaterChoose";
        /// <summary>是否复板</summary>
        public const string CompoundBoardChoose = "CompoundBoardChoose";
        /// <summary>布重</summary>
        public const string ClothWeight = "ClothWeight";
        /// <summary>浴比</summary>
        public const string BathRatio = "BathRatio";
        /// <summary>总浴量</summary>
        public const string TotalWeight = "TotalWeight";
        /// <summary>操作员</summary>
        public const string Operator = "Operator";
        /// <summary>染杯代码</summary>
        public const string CupCode = "CupCode";
        /// <summary>存档时间</summary>
        public const string CreateTime = "CreateTime";
        /// <summary>目标加水重量</summary>
        public const string ObjectAddWaterWeight = "ObjectAddWaterWeight";
        /// <summary>实际加水重量</summary>
        public const string RealAddWaterWeight = "RealAddWaterWeight";
        /// <summary>试管目标加水量</summary>
        public const string TestTubeObjectAddWaterWeight = "TestTubeObjectAddWaterWeight";
        /// <summary>试管实际加水量</summary>
        public const string TestTubeRealAddWaterWeight = "TestTubeRealAddWaterWeight";
        /// <summary>完成时间</summary>
        public const string FinishTime = "FinishTime";
        /// <summary>描述</summary>
        public const string DescribeChar = "DescribeChar";
        /// <summary>染固色代码</summary>
        public const string DyeingCode = "DyeingCode";
        /// <summary>步数</summary>
        public const string Step = "Step";
        /// <summary>非脱水水比</summary>
        public const string Non_AnhydrationWR = "Non_AnhydrationWR";
        /// <summary>脱水水比</summary>
        public const string AnhydrationWR = "AnhydrationWR";
        /// <summary>后处理水比</summary>
        public const string HandleBathRatio = "HandleBathRatio";
        /// <summary>转速1</summary>
        public const string Handle_Rev1 = "Handle_Rev1";
        /// <summary>转速2</summary>
        public const string Handle_Rev2 = "Handle_Rev2";
        /// <summary>转速3</summary>
        public const string Handle_Rev3 = "Handle_Rev3";
        /// <summary>转速4</summary>
        public const string Handle_Rev4 = "Handle_Rev4";
        /// <summary>转速5</summary>
        public const string Handle_Rev5 = "Handle_Rev5";
        /// <summary>开始时间</summary>
        public const string StartTime = "StartTime";
        /// <summary>温度曲线</summary>
        public const string ProcessData = "ProcessData";
        /// <summary>曲线标记点</summary>
        public const string MarkStep = "MarkStep";
        /// <summary>类型</summary>
        public const string Stage = "Stage";
        /// <summary>光谱数据</summary>
        public const string Spectrum = "Spectrum";
        /// <summary>描述英文</summary>
        public const string DescribeChar_EN = "DescribeChar_EN";
        /// <summary>染色后处理浴比集合</summary>
        public const string HandleBRList = "HandleBRList";
        /// <summary>加水限值</summary>
        public const string WaterStandError = "WaterStandError";
        /// <summary>放布位</summary>
        public const string ClothNum = "ClothNum";
        /// <summary>自动方布</summary>
        public const string IsAutoIn = "IsAutoIn";
        /// <summary>染固色代码备注</summary>
        public const string DyeingCodeRemark = "DyeingCodeRemark";
        /// <summary>复色原因</summary>
        public const string Recoloration = "Recoloration";
        /// <summary>缸位</summary>
        public const string VatNumber = "VatNumber";
        /// <summary>结果</summary>
        public const string Result = "Result";
        /// <summary>备注1</summary>
        public const string Note1 = "Note1";
        /// <summary>备注2</summary>
        public const string Note2 = "Note2";
        /// <summary>备注3</summary>
        public const string Note3 = "Note3";
        /// <summary>原MyID</summary>
        public const string HeadID = "HeadID";

    }

    /// <summary>
    /// import_parameters表字段常量定义
    /// </summary>
    public static class IMPORT_PARAMETERS
    {
        /// <summary>表名</summary>
        public const string TableName = "import_parameters";
        /// <summary>索引</summary>
        public const string MyID = "MyID";
        /// <summary>启用</summary>
        public const string StartUsing = "StartUsing";
        /// <summary>路径</summary>
        public const string Route = "Route";
        /// <summary>扫描时间</summary>
        public const string MyTime = "MyTime";
    }

    /// <summary>
    /// LimitTable表字段常量定义
    /// </summary>
    public static class LIMIT_TABLE
    {
        /// <summary>表名</summary>
        public const string TableName = "LimitTable";
        /// <summary>类型</summary>
        public const string Type = "Type";
        /// <summary>最小值</summary>
        public const string Min = "Min";
        /// <summary>最大值</summary>
        public const string Max = "Max";
        /// <summary>助剂代码</summary>
        public const string AssistantCode = "AssistantCode";
        /// <summary>值</summary>
        public const string Value = "Value";
    }

    /// <summary>
    /// LLJ表字段常量定义
    /// </summary>
    public static class LLJ
    {
        /// <summary>表名</summary>
        public const string TableName = "LLJ";
        /// <summary>序号</summary>
        public const string ID = "ID";
        /// <summary>目标重量</summary>
        public const string Weight = "Weight";
        /// <summary>理论重量</summary>
        public const string LLWeight = "LLWeight";
        /// <summary>脉冲</summary>
        public const string Pulse = "Pulse";
        /// <summary>时间</summary>
        public const string Time = "Time";
        /// <summary>校正系数</summary>
        public const string LLC = "LLC";
        /// <summary>校正重量</summary>
        public const string AddC = "AddC";
    }

    /// <summary>
    /// pre_brew表字段常量定义
    /// </summary>
    public static class PRE_BREW
    {
        /// <summary>表名</summary>
        public const string TableName = "pre_brew";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>实际重量</summary>
        public const string CurrentWeight = "CurrentWeight";
        /// <summary>开料日期</summary>
        public const string BrewingData = "BrewingData";
    }

    /// <summary>
    /// pretreatment_code表字段常量定义
    /// </summary>
    public static class PRETREATMENT_CODE
    {
        /// <summary>表名</summary>
        public const string TableName = "pretreatment_code";
        /// <summary>前处理代码</summary>
        public const string PretreatmentCode = "PretreatmentCode";
    }

    /// <summary>
    /// recheck表字段常量定义
    /// </summary>
    public static class RECHECK
    {
        /// <summary>表名</summary>
        public const string TableName = "recheck";
        /// <summary>序号</summary>
        public const string MyID = "MyID";
        /// <summary>日期</summary>
        public const string MyDate = "MyDate";
        /// <summary>时间</summary>
        public const string MyTime = "MyTime";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>现存量</summary>
        public const string CurrentWeight = "CurrentWeight";
        /// <summary>实际滴液量</summary>
        public const string RealWeight = "RealWeight";
    }

    /// <summary>
    /// run_table表字段常量定义
    /// </summary>
    public static class RUN_TABLE
    {
        /// <summary>表名</summary>
        public const string TableName = "run_table";
        /// <summary>序号</summary>
        public const string MyID = "MyID";
        /// <summary>动作日期</summary>
        public const string MyDate = "MyDate";
        /// <summary>动作时刻</summary>
        public const string MyTime = "MyTime";
        /// <summary>系统动作</summary>
        public const string Machine = "Machine";
        /// <summary>机械手动作</summary>
        public const string RobotHand = "RobotHand";
        /// <summary>打板机动作</summary>
        public const string Dail = "Dail";
        /// <summary>加水</summary>
        public const string Water = "Water";
        /// <summary>加粉</summary>
        public const string Powder = "Powder";
    }

    /// <summary>
    /// self_table表字段常量定义
    /// </summary>
    public static class SELF_TABLE
    {
        /// <summary>表名</summary>
        public const string TableName = "self_table";
        /// <summary>自检日期</summary>
        public const string Date = "Date";
        /// <summary>自检1</summary>
        public const string SelfChecking1 = "SelfChecking1";
        /// <summary>自检2</summary>
        public const string SelfChecking2 = "SelfChecking2";
        /// <summary>自检3</summary>
        public const string SelfChecking3 = "SelfChecking3";
        /// <summary>自检4</summary>
        public const string SelfChecking4 = "SelfChecking4";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>当前校正值</summary>
        public const string CurrentAdjustWeight = "CurrentAdjustWeight";
        /// <summary>校正脉冲</summary>
        public const string AdjustValue = "AdjustValue";
        /// <summary>失败</summary>
        public const string Fail = "Fail";
    }

    /// <summary>
    /// check_table表字段常量定义
    /// </summary>
    public static class CHECK_TABLE
    {
        /// <summary>表名</summary>
        public const string TableName = "check_table";
        /// <summary>针检日期</summary>
        public const string Date = "Date";
        /// <summary>瓶号</summary>
        public const string BottleNum = "BottleNum";
        /// <summary>实际浓度</summary>
        public const string RealConcentration = "RealConcentration";
        /// <summary>当前液量</summary>
        public const string CurrentWeight = "CurrentWeight";
        /// <summary>当前校正重量</summary>
        public const string CurrentAdjustWeight = "CurrentAdjustWeight";
        /// <summary>校正值</summary>
        public const string AdjustValue = "AdjustValue";
        /// <summary>复检重量</summary>
        public const string RecheckWeight = "RecheckWeight";
        /// <summary>失败</summary>
        public const string Fail = "Fail";
    }

    /// <summary>
    /// SpeechInfo表字段常量定义
    /// </summary>
    public static class SPEECHINFO
    {
        /// <summary>表名</summary>
        public const string TableName = "SpeechInfo";
        /// <summary>播报内容</summary>
        public const string Info = "Info";
        /// <summary>是否已播完</summary>
        public const string IsFinished = "IsFinished";
    }

    /// <summary>
    /// standard表字段常量定义
    /// </summary>
    public static class STANDARD
    {
        /// <summary>表名</summary>
        public const string TableName = "standard";
        /// <summary>E1</summary>
        public const string E1 = "E1";
        /// <summary>E2</summary>
        public const string E2 = "E2";
        /// <summary>波长</summary>
        public const string WL = "WL";
        /// <summary>完成时间</summary>
        public const string FinishTime = "FinishTime";
        /// <summary>类型</summary>
        public const string Type = "Type";
    }

    /// <summary>
    /// wait_list表字段常量定义
    /// </summary>
    public static class WAIT_LIST
    {
        /// <summary>表名</summary>
        public const string TableName = "wait_list";
        /// <summary>序号</summary>
        public const string MyID = "MyID";
        /// <summary>配方代码</summary>
        public const string FormulaCode = "FormulaCode";
        /// <summary>版本</summary>
        public const string VersionNum = "VersionNum";
        /// <summary>序号</summary>
        public const string IndexNum = "IndexNum";
        /// <summary>杯号</summary>
        public const string CupNum = "CupNum";
        /// <summary>类型</summary>
        public const string Type = "Type";
    }

    /// <summary>
    /// User_Tale表字段常量定义
    /// </summary>
    public static class USER_TALE
    {
        /// <summary>表名</summary>
        public const string TableName = "User_Tale";
        /// <summary>序号</summary>
        public const string MyID = "MyID";
        /// <summary>账户</summary>
        public const string Account = "Account";
        /// <summary>密码</summary>
        public const string PassWord = "PassWord";
        /// <summary>权限</summary>
        public const string Purview = "Purview";
        /// <summary>姓名</summary>
        public const string RealName = "RealName";
    }

    /// <summary>
    /// 泡制流程数据静态类
    /// </summary>
    public static class BrewData
    {
        /// <summary>泡制流程代码表</summary>
        public static DataTable Brewing_code = null;

        /// <summary>泡制流程详情表</summary>
        public static DataTable Brewing_process = null;

        /// <summary>
        /// 获取泡制流程数据
        /// </summary>
        public static void GetData()
        {
            try
            {
                Brewing_code = My_DataBase.SqlServer.Select(My_DataBase.BREWING_CODE.TableName);
                Brewing_process = My_DataBase.SqlServer.Select(My_DataBase.BREWING_PROCESS.TableName);
                Logger.Info("GetData: 刷新泡制流程数据。");
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新泡制流程数据异常。", ex);
            }
        }
    }

    /// <summary>
    /// 母液瓶资料数据静态类
    /// </summary>
    public static class BottleData
    {
        /// <summary>母液瓶资料表</summary>
        public static DataTable Bottle_details = null;

        /// <summary>
        /// 获取母液瓶资料数据
        /// </summary>
        public static void GetData()
        {
            try
            {
                Bottle_details = My_DataBase.SqlServer.Select(BOTTLE_DETAILS.TableName);
                Logger.Info("GetData: 刷新母液瓶资料数据。");
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新母液瓶资料数据异常。", ex);
            }
        }
    }

    /// <summary>
    /// 染助剂资料数据静态类
    /// </summary>
    public static class AssistantData
    {
        /// <summary>
        /// 染助剂资料表
        /// </summary>
        public static DataTable Assistant_details = null;

        /// <summary>
        /// 获取染助剂资料表
        /// </summary>
        public static void GetData()
        {
            try
            {
                Assistant_details = My_DataBase.SqlServer.Select(ASSISTANT_DETAILS.TableName);
                Logger.Info("GetData: 刷新染助剂资料表完成。");
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新染助剂资料数据异常。", ex);
            }
        }

    }

    /// <summary>
    /// 染色/后处理工艺代码静态类
    /// </summary>
    public static class DyeingData
    {
        /// <summary>
        /// 可用的染色和后处理工艺表
        /// </summary>
        public static DataTable Dyeing_process = null;

        /// <summary>
        /// 历史的染色和后处理工艺表
        /// </summary>
        public static DataTable History_Dyeing_process = null;

        /// <summary>
        /// 获取染色和后处理工艺表
        /// </summary>
        public static void GetData()
        {
            try
            {
                Dyeing_process = SqlServer.Select(DYEING_PROCESS.TableName);
                History_Dyeing_process = SqlServer.Select(HISTORY_DYEING_PROCESS.TableName);
                Logger.Info("GetData: 刷新染色和后处理工艺数据。");
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新染色和后处理工艺异常。", ex);
            }
        }

    }

    /// <summary>
    /// 染固色工艺代码静态类
    /// </summary>
    public static class DyeingCodeData
    {
        /// <summary>染固色工艺表</summary>
        public static DataTable Dyeing_code = null;

        /// <summary>旧染固色工艺表</summary>
        public static DataTable History_Dyeing_code = null;

        /// <summary>
        /// 获取染固色工艺表
        /// </summary>
        public static void GetData()
        {
            try
            {
                Dyeing_code = My_DataBase.SqlServer.Select(My_DataBase.DYEING_CODE.TableName);
                History_Dyeing_code = My_DataBase.SqlServer.Select(My_DataBase.HISTORY_DYEING_CODE.TableName);
                Logger.Info("GetData: 刷新染固色工艺数据。");
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新染固色工艺数据异常。", ex);
            }
        }

    }

    /// <summary>
    /// 助剂分量静态类
    /// </summary>
    public static class LimitData
    {
        /// <summary>助剂分量表</summary>
        public static DataTable LimitTable = null;

        /// <summary>
        /// 获取染助剂分量数据
        /// </summary>
        public static void GetData()
        {
            try
            {
                LimitTable = SqlServer.Select(My_DataBase.LIMIT_TABLE.TableName);
                Logger.Info("GetData: 刷新染助剂分量数据。");
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新染助剂分量异常。", ex);
            }
        }
    }

    /// <summary>
    /// 配方组合数据表静态类
    /// </summary>
    public static class FormulaGradeData
    {
        /// <summary>配方组合数据表</summary>
        public static DataTable Formula_group = null;

        /// <summary>
        /// 获取配方组合数据
        /// </summary>
        public static void GetData()
        {
            try
            {
                Formula_group = SqlServer.Select(My_DataBase.FORMULA_GROUP.TableName);
                Logger.Info("GetData: 刷新配方组合数据。");
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新配方组合异常。", ex);
            }
        }
    }

    /// <summary>
    /// 用户数据静态类
    /// </summary>
    public static class UserData
    {
        /// <summary>用户表数据</summary>
        public static DataTable UserTable = null;

        /// <summary>
        /// 获取用户数据
        /// </summary>
        public static void GetData()
        {
            try
            {
                UserTable = SqlServer.Select(My_DataBase.USER_TALE.TableName);
                Logger.Info("GetData: 刷新用户数据。");
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新用户数据异常。", ex);
            }
        }
    }

    /// <summary>
    /// 使能数据静态类
    /// </summary>
    public static class EnabledData
    {
        /// <summary>使能数据表</summary>
        public static DataTable Enabled_set = null;

        /// <summary>
        /// 获取使能数据（静态方法，刷新Enabled_set数据表）
        /// </summary>
        public static void GetData()
        {
            try
            {
                Enabled_set = My_DataBase.SqlServer.Select(My_DataBase.ENABLED_SET.TableName);
                if (Enabled_set.Rows.Count == 0)
                {
                    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
                    keyValuePairs.Add(My_DataBase.ENABLED_SET.MyID, 1);
                    My_DataBase.SqlServer.Insert(My_DataBase.ENABLED_SET.TableName, keyValuePairs);
                    Enabled_set = My_DataBase.SqlServer.Select(My_DataBase.ENABLED_SET.TableName);
                }
                Logger.Info("GetData: 刷新使能数据。");
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新使能异常。", ex);
            }
        }
    }

    /// <summary>
    /// 使能数据静态类
    /// </summary>
    public static class ABSEnabledData
    {
        /// <summary>使能数据表</summary>
        public static DataTable ABS_Enabled_set = null;

        /// <summary>
        /// 获取使能数据（静态方法，刷新Enabled_set数据表）
        /// </summary>
        public static void GetData()
        {
            try
            {
                ABS_Enabled_set = My_DataBase.SqlServer.Select(My_DataBase.ABS_ENABLED_SET.TableName);
                if (ABS_Enabled_set.Rows.Count == 0)
                {
                    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
                    keyValuePairs.Add(My_DataBase.ABS_ENABLED_SET.MyID, 1);
                    My_DataBase.SqlServer.Insert(My_DataBase.ABS_ENABLED_SET.TableName, keyValuePairs);
                    ABS_Enabled_set = My_DataBase.SqlServer.Select(My_DataBase.ABS_ENABLED_SET.TableName);
                }
                Logger.Info("GetData: 刷新使能数据。");
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新使能异常。", ex);
            }
        }
    }

    /// <summary>
    /// 配液杯数据静态类
    /// </summary>
    public static class CupData
    {
        /// <summary>
        /// 获取配液杯资料表
        /// </summary>
        public static DataTable GetData()
        {
            try
            {
                return My_DataBase.SqlServer.Select(My_DataBase.CUP_DETAILS.TableName);

            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新配液杯资料表异常。", ex);
                return null;
            }
        }
    }





    /// <summary>
    /// 染色批次静态类
    /// </summary>
    public static class DropBatchData
    {
        /// <summary>
        /// 获取染色批次数据
        /// </summary>
        public static DataTable GetHeadData()
        {
            try
            {

                return SqlServer.Select(
                   My_DataBase.DROP_HEAD.TableName,
                   null, // null表示查询所有字段
                   null,
                   My_DataBase.DROP_HEAD.CupNum,
                   true,
                   null
               );
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新染色批次表数据异常。", ex);
                return null;
            }
        }

     

        public static DataTable GetDyeData()
        {
            try
            {
                return My_DataBase.SqlServer.Select(My_DataBase.DYE_DETAILS.TableName);
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新染色批次染料表数据异常。", ex);
                return null;
            }
        }

    }

    /// <summary>
    /// ABS批次静态类
    /// </summary>
    public static class ABSBatchData
    {


        /// <summary>
        /// 获取ABS批次数据
        /// </summary>
       
        public static DataTable GetHeadData()
        {
            try
            {

                return SqlServer.Select(
                   My_DataBase.ABS_DROP_HEAD.TableName,
                   null, // null表示查询所有字段
                   null,
                   My_DataBase.DROP_HEAD.CupNum,
                   true,
                   null
               );
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新ABS批次表数据异常。", ex);
                return null;
            }
        }
    }

    /// <summary>
    /// 染色等待列表静态类
    /// </summary>
    public static class DropWaitData
    {
       

        /// <summary>
        /// 获取染色等待列表数据
        /// </summary>
        public static DataTable GetData()
        {
            try
            {
               return My_DataBase.SqlServer.Select(WAIT_LIST.TableName);
                
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新等待列表异常。", ex);
                return null;
            }
        }
    }

    /// <summary>
    /// ABS等待列表静态类
    /// </summary>
    public static class ABSWaitData
    {
      

        /// <summary>
        /// 获取ABS等待列表数据
        /// </summary>
        public static DataTable GetData()
        {
            try
            {
                return My_DataBase.SqlServer.Select(ABS_WAIT_LIST.TableName);
               
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新ABS等待列表异常。", ex);
                return null;
            }
        }
    }

    /// <summary>
    /// 吸光度测量流程静态类
    /// </summary>
    public static class ABSProcess
    {
        /// <summary>
        /// 吸光度测量流程
        /// </summary>
        public static DataTable ABS_Process = null;

        /// <summary>
        /// 获取吸光度测量流程
        /// </summary>
        public static void GetData()
        {
            try
            {
                ABS_Process = My_DataBase.SqlServer.Select(ABS_PROCESS.TableName);
                Logger.Info("GetData: 刷新吸光度测量流程数据。");
            }
            catch (Exception ex)
            {
                Logger.Error("GetData: 刷新吸光度测量流程异常。", ex);
            }
        }
    }
}


