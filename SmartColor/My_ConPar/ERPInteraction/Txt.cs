using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.ERPInteraction
{
    /// <summary>
    /// TXT交互格式类
    /// </summary>
    internal class Txt 
    {  
        #region 路径

        /// <summary>
        /// 绝对路径
        /// </summary>
        [Description("交互绝对路径")]
        public string TxtPath { get; set; } =
            AppDomain.CurrentDomain.BaseDirectory + "TxtFile\\FLINK.DAT";

        #endregion

        #region 表头
        /// <summary>
        /// 表头总长度
        /// </summary>
        [Description("表头总长度")]
        public int Head_Total { get; set; } = 86;

        /// <summary>
        /// 表头开始识别码开始位置
        /// </summary>
        [Description("表头开始识别码开始位置")]
        public int Head_Ip { get; set; } = 1;

        /// <summary>
        /// 表头开始识别码长度
        /// </summary>
        [Description("表头开始识别码长度")]
        public int Head_Ip_Len { get; set; } = 4;

        /// <summary>
        /// 表头配方代码开始位置
        /// </summary>
        [Description("表头配方代码开始位置")]
        public int Head_FormulaCode { get; set; } = 5;

        /// <summary>
        /// 表头配方代码长度
        /// </summary>
        [Description("表头配方代码长度")]
        public int Head_FormulaCode_Len { get; set; } = 12;

        /// <summary>
        /// 表头配方版本开始位置
        /// </summary>
        [Description("表头配方版本开始位置")]
        public int Head_VersionNum { get; set; } = 17;

        /// <summary>
        /// 表头配方版本长度
        /// </summary>
        [Description("表头配方版本长度")]
        public int Head_VersionNum_Len { get; set; } = 2;

        /// <summary>
        /// 表头染助劑支數开始位置
        /// </summary>
        [Description("表头染助劑支數开始位置")]
        public int Head_Count { get; set; } = 19;

        /// <summary>
        /// 表头染助劑支數长度
        /// </summary>
        [Description("表头染助劑支數长度")]
        public int Head_Count_Len { get; set; } = 2;

        /// <summary>
        /// 表头配方單位开始位置
        /// </summary>
        [Description("表头配方單位开始位置")]
        public int Head_Unit { get; set; } = 21;

        /// <summary>
        /// 表头配方單位长度
        /// </summary>
        [Description("表头配方單位长度")]
        public int Head_Unit_Len { get; set; } = 1;

        /// <summary>
        /// 表头滴定序號开始位置
        /// </summary>
        [Description("表头滴定序號开始位置")]
        public int Head_Index { get; set; } = 22;

        /// <summary>
        /// 表头滴定序號长度
        /// </summary>
        [Description("表头滴定序號长度")]
        public int Head_Index_Len { get; set; } = 3;

        /// <summary>
        /// 表头配方名称开始位置
        /// </summary>
        [Description("表头配方名称开始位置")]
        public int Head_FormulaName { get; set; } = 25;

        /// <summary>
        /// 表头配方名称长度
        /// </summary>
        [Description("表头配方名称长度")]
        public int Head_FormulaName_Len { get; set; } = 24;

        /// <summary>
        /// 表头布重开始位置
        /// </summary>
        [Description("表头布重开始位置")]
        public int Head_ClothWeight { get; set; } = 49;

        /// <summary>
        /// 表头布重长度
        /// </summary>
        [Description("表头布重长度")]
        public int Head_ClothWeight_Len { get; set; } = 8;

        /// <summary>
        /// 表头总浴量开始位置
        /// </summary>
        [Description("表头总浴量开始位置")]
        public int Head_TotalWeight { get; set; } = 57;

        /// <summary>
        /// 表头总浴量长度
        /// </summary>
        [Description("表头总浴量长度")]
        public int Head_TotalWeight_Len { get; set; } = 8;

        /// <summary>
        /// 表头是否加水开始位置
        /// </summary>
        [Description("表头是否加水开始位置")]
        public int Head_AddWater { get; set; } = 65;

        /// <summary>
        /// 表头是否加水长度
        /// </summary>
        [Description("表头是否加水长度")]
        public int Head_AddWater_Len { get; set; } = 1;

        /// <summary>
        /// 表头是否续加开始位置
        /// </summary>
        [Description("表头是否续加开始位置")]
        public int Head_ConAdd { get; set; } = 66;

        /// <summary>
        /// 表头是否续加长度
        /// </summary>
        [Description("表头是否续加长度")]
        public int Head_ConAdd_Len { get; set; } = 1;

        /// <summary>
        /// 表头机台编号开始位置
        /// </summary>
        [Description("表头机台编号开始位置")]
        public int Head_MNum { get; set; } = 67;

        /// <summary>
        /// 表头机台编号长度
        /// </summary>
        [Description("表头机台编号长度")]
        public int Head_MNum_Len { get; set; } = 2;

        /// <summary>
        /// 表头日期开始位置
        /// </summary>
        [Description("表头日期开始位置")]
        public int Head_Date { get; set; } = 69;

        /// <summary>
        /// 表头日期长度
        /// </summary>
        [Description("表头日期长度")]
        public int Head_Date_Len { get; set; } = 8;

        /// <summary>
        /// 表头染程开始位置
        /// </summary>
        [Description("表头染程开始位置")]
        public int Head_Code { get; set; } = 77;

        /// <summary>
        /// 表头染程长度
        /// </summary>
        [Description("表头染程长度")]
        public int Head_Code_Len { get; set; } = 8;

        /// <summary>
        /// 表头配方類型开始位置
        /// </summary>
        [Description("表头配方類型开始位置")]
        public int Head_Type { get; set; } = 85;

        /// <summary>
        /// 表头配方類型长度
        /// </summary>
        [Description("表头配方類型长度")]
        public int Head_Type_Len { get; set; } = 1;

        /// <summary>
        /// 表头是否滴過开始位置
        /// </summary>
        [Description("表头是否滴過开始位置")]
        public int Head_Drip { get; set; } = 86;

        /// <summary>
        /// 表头是否滴過长度
        /// </summary>
        [Description("表头是否滴過长度")]
        public int Head_Drip_Len { get; set; } = 1;

        /// <summary>
        /// 表头结束识别码开始位置
        /// </summary>
        [Description("表头结束识别码开始位置")]
        public int Head_SIN { get; set; } = 87;

        /// <summary>
        /// 表头结束识别码长度
        /// </summary>
        [Description("表头结束识别码长度")]
        public int Head_SIN_Len { get; set; } = 2;

        #endregion

        #region 详情

        /// <summary>
        /// 表详情总长度
        /// </summary>
        [Description("表详情总长度")]
        public int Detail_Total { get; set; } = 41;

        /// <summary>
        /// 详情开始识别码开始位置
        /// </summary>
        [Description("详情开始识别码开始位置")]
        public int Detail_Ip { get; set; } = 1;

        /// <summary>
        /// 详情开始识别码长度
        /// </summary>
        [Description("详情开始识别码长度")]
        public int Detail_Ip_Len { get; set; } = 4;

        /// <summary>
        /// 详情配方代码开始位置
        /// </summary>
        [Description("详情配方代码开始位置")]
        public int Detail_FormulaCode { get; set; } = 5;

        /// <summary>
        /// 详情配方代码长度
        /// </summary>
        [Description("详情配方代码长度")]
        public int Detail_FormulaCode_Len { get; set; } = 12;

        /// <summary>
        /// 详情配方版本开始位置
        /// </summary>
        [Description("详情配方版本开始位置")]
        public int Detail_VersionNum { get; set; } = 17;

        /// <summary>
        /// 详情配方版本长度
        /// </summary>
        [Description("详情配方版本长度")]
        public int Detail_VersionNum_Len { get; set; } = 2;

        /// <summary>
        /// 详情滴定序號开始位置
        /// </summary>
        [Description("详情滴定序號开始位置")]
        public int Detail_Index { get; set; } = 19;

        /// <summary>
        /// 详情滴定序號长度
        /// </summary>
        [Description("详情滴定序號长度")]
        public int Detail_Index_Len { get; set; } = 2;

        /// <summary>
        /// 详情配方單位开始位置
        /// </summary>
        [Description("详情配方單位开始位置")]
        public int Detail_Unit { get; set; } = 21;

        /// <summary>
        /// 详情配方單位长度
        /// </summary>
        [Description("详情配方單位长度")]
        public int Detail_Unit_Len { get; set; } = 1;

        /// <summary>
        /// 详情库存编号开始位置
        /// </summary>
        [Description("详情库存编号开始位置")]
        public int Detail_Num { get; set; } = 22;

        /// <summary>
        /// 详情库存编号长度
        /// </summary>
        [Description("详情库存编号长度")]
        public int Detail_Num_Len { get; set; } = 3;

        /// <summary>
        /// 详情助剂代码开始位置
        /// </summary>
        [Description("详情助剂代码开始位置")]
        public int Detail_AssistantCode { get; set; } = 25;

        /// <summary>
        /// 详情助剂代码长度
        /// </summary>
        [Description("详情助剂代码长度")]
        public int Detail_AssistantCode_Len { get; set; } = 8;

        /// <summary>
        /// 详情助剂浓度开始位置
        /// </summary>
        [Description("详情助剂浓度开始位置")]
        public int Detail_RealConcentration { get; set; } = 33;

        /// <summary>
        /// 详情助剂浓度长度
        /// </summary>
        [Description("详情助剂浓度长度")]
        public int Detail_RealConcentration_Len { get; set; } = 8;

        /// <summary>
        /// 详情结束识别码开始位置
        /// </summary>
        [Description("详情结束识别码开始位置")]
        public int Detail_SIN { get; set; } = 42;

        /// <summary>
        /// 详情结束识别码长度
        /// </summary>
        [Description("详情结束识别码长度")]
        public int Detail_SIN_Len { get; set; } = 2;

        #endregion
    }
}
