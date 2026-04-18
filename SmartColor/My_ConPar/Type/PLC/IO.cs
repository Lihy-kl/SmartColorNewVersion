using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Type.PLC
{
    /// <summary>
    /// PLC IO
    /// </summary>
    internal class IO : BaseIO
    {
        /// <summary>
        /// PLC IO
        /// </summary>
        public IO()
        {
            #region 输入
            InPut_Sunx_Stop = "X0";
            InPut_Z_Reverse = "X1";
            InPut_Tray_Out = "X3";
            InPut_Tray_In = "X4";
            InPut_Syringe = "X5";
            InPut_Tongs_A = "X6";
            InPut_Tongs_B = "X7";
            InPut_Cylinder_Up = "X10";
            InPut_Cylinder_Down = "X11";
            InPut_Sunx_A = "X12";
            InPut_Sunx_B = "X13";
            InPut_Cylinder_Mid = "X14";
            InPut_Decompression_Down = "X16";
            InPut_Decompression_Up = "X15";
            InPut_Sunx_Back = "X17";
            InPut_Block_Out = "X20";
            InPut_Block_In = "X21";
            InPut_Slow_Cylinder_Mid = "X22";
            InPut_Cylinder_Block = "X23";
            InPut_SupportCover = "X24";

            #endregion

            #region 输出

            OutPut_Slow_Cylinder = "Y3";
            OutPut_Blender = "Y4";
            OutPut_Waste = "Y5";
            OutPut_Buzzer = "Y6";
            OutPut_Water = "Y7";
            OutPut_Tray = "Y10";
            OutPut_TongsOff = "Y14";
            OutPut_Cylinder_Down = "Y12";
            OutPut_Cylinder_Up = "Y13";
            OutPut_TongsOn = "Y11";
            OutPut_Block_Out = "Y16";
            OutPut_Block_In = "Y17";
            OutPut_Decompression = "Y15";
            OutPut_Wash_In = "Y20";
            OutPut_Wash_Out = "Y21";
            OutPut_Wash_Blow = "Y22";
          
            #endregion
        }

        #region 输入
      
        /// <summary>
        /// 气缸到慢速中2
        /// </summary>
        [Description("气缸到慢速中2|输入")]
        public string InPut_Slow_Cylinder_Mid_2 { get; set; } = "X25";

        /// <summary>
        /// 气缸到慢速中3
        /// </summary>
        [Description("气缸到慢速中3|输入")]
        public string InPut_Slow_Cylinder_Mid_3 { get; set; } = "X26";

        /// <summary>
        /// 转子缸急停信号
        /// </summary>
        [Description("转子缸急停信号|输入")]
        public string InPut_RotorCylinderStop { get; set; } = "X27";

        /// <summary>
        /// 开料气缸上升到位
        /// </summary>
        [Description("开料气缸上升到位|输入")]
        public string InPut_Brew_Cylinder_Up { get; set; } = "-1";

        /// <summary>
        /// 开料气缸下降到位
        /// </summary>
        [Description("开料气缸下降到位|输入")]
        public string InPut_Brew_Cylinder_Down { get; set; } = "-1";

        /// <summary>
        /// 开料摆臂伸到位
        /// </summary>
        [Description("开料摆臂伸到位|输入")]
        public string InPut_Brew_SwingArmOut { get; set; } = "-1";

        /// <summary>
        /// 开料摆臂缩到位
        /// </summary>
        [Description("开料摆臂缩到位|输入")]
        public string InPut_Brew_SwingArmIn { get; set; } = "-1";

        /// <summary>
        /// 洗瓶夹瓶伸到位
        /// </summary>
        [Description("洗瓶夹瓶伸到位|输入")]
        public string InPut_Brew_BWHC_Out { get; set; } = "-1";

        /// <summary>
        /// 洗瓶夹瓶缩到位
        /// </summary>
        [Description("洗瓶夹瓶缩到位|输入")]
        public string InPut_Brew_BWHC_In { get; set; } = "-1";

        /// <summary>
        /// 洗瓶上翻限位
        /// </summary>
        [Description("洗瓶上翻限位|输入")]
        public string InPut_Brew_BWHC_Up { get; set; } = "-1";

        /// <summary>
        /// 洗瓶下翻限位
        /// </summary>
        [Description("洗瓶下翻限位|输入")]
        public string InPut_Brew_BWHC_Down { get; set; } = "-1";

        /// <summary>
        /// 热水箱浮球
        /// </summary>
        [Description("热水箱浮球|输入")]
        public string InPut_HotWaterTankFloatBall { get; set; } = "-1";

        #endregion

        #region 输出

        /// <summary>
        /// 加溶解剂（3962）
        /// </summary>
        [Description("加溶解剂|输出")]
        public string OutPut_Solvent { get; set; } = "Y23";

        /// <summary>
        /// 开料气缸上升（3968）
        /// </summary>
        [Description("开料气缸上升|输出")]
        public string OutPut_Brew_Cylinder_Up { get; set; } = "-1";

        /// <summary>
        /// 开料气缸下降（3970）
        /// </summary>
        [Description("开料气缸下降|输出")]
        public string OutPut_Brew_Cylinder_Down { get; set; } = "-1";

        /// <summary>
        /// 开料热水阀（3972）
        /// </summary>
        [Description("开料热水阀|输出")]
        public string OutPut_Brew_HotWater { get; set; } = "-1";

        /// <summary>
        /// 开料大冷水阀（3974）
        /// </summary>
        [Description("开料大冷水阀|输出")]
        public string OutPut_Brew_BigColdWater { get; set; } = "-1";

        /// <summary>
        /// 开料小冷水阀（3976）
        /// </summary>
        [Description("开料小冷水阀|输出")]
        public string OutPut_Brew_SmallColdWater { get; set; } = "-1";

        /// <summary>
        /// 开料热水泵（3978）
        /// </summary>
        [Description("开料热水泵|输出")]
        public string OutPut_Brew_HotWaterPump { get; set; } = "-1";

        /// <summary>
        /// 摆臂伸（3980）
        /// </summary>
        [Description("摆臂伸|输出")]
        public string OutPut_Brew_SwingArmOut { get; set; } = "-1";

        /// <summary>
        /// 摆臂缩（3982）
        /// </summary>
        [Description("摆臂缩|输出")]
        public string OutPut_Brew_SwingArmIn { get; set; } = "-1";

        /// <summary>
        /// 洗瓶夹瓶伸（3984）
        /// </summary>
        [Description("洗瓶夹瓶伸|输出")]
        public string OutPut_Brew_BWHC_Out { get; set; } = "-1";

        /// <summary>
        /// 洗瓶夹瓶缩（3986）
        /// </summary>
        [Description("洗瓶夹瓶缩|输出")]
        public string OutPut_Brew_BWHC_In { get; set; } = "-1";

        /// <summary>
        /// 水箱进热水（3990）
        /// </summary>
        [Description("水箱进热水|输出")]
        public string OutPut_HotWaterTankInletValve { get; set; } = "-1";

        /// <summary>
        /// 洗瓶上翻（3992）
        /// </summary>
        [Description("洗瓶上翻|输出")]
        public string OutPut_Brew_BWHC_Up { get; set; } = "-1";

        /// <summary>
        /// 洗瓶下翻（3994）
        /// </summary>
        [Description("洗瓶下翻|输出")]
        public string OutPut_Brew_BWHC_Down { get; set; } = "-1";

        /// <summary>
        /// 洗瓶吹气阀（3996）
        /// </summary>
        [Description("洗瓶吹气阀|输出")]
        public string OutPut_Brew_Wash_Blow { get; set; } = "-1";

        /// <summary>
        /// 洗瓶进冷水阀（3998）
        /// </summary>
        [Description("洗瓶进冷水阀|输出")]
        public string OutPut_Brew_WashColdWater { get; set; } = "-1";

        /// <summary>
        /// 洗瓶热风（4000）
        /// </summary>
        [Description("洗瓶热风|输出")]
        public string OutPut_Brew_HotWind { get; set; } = "-1";

        /// <summary>
        /// 热水箱加热（4002）
        /// </summary>
        [Description("热水箱加热|输出")]
        public string OutPut_Brew_WaterHeaterHeating { get; set; } = "-1";

        #endregion

        /// <summary>
        /// PLC IP地址
        /// </summary>
        [Description("PLC IP地址")]
        public string IP { get; set; } = "192.168.68.20";

        /// <summary>
        /// PLC 端口号
        /// </summary>
        [Description("PLC 端口号")]
        public int Port { get; set; } = 502;
    }
}
