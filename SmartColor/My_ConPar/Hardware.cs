using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar
{
    /// <summary>
    /// 机台硬件配置参数类
    /// </summary>
    internal class Hardware
    {
        /// <summary>
        /// 伺服类型
        /// 0：富士
        /// 1：绿维
        /// </summary>
        [Description("板卡版专用：伺服类型 |0：富士  1：绿维")]
        public static  int ServoType { get; set; } = 0;

        

        /// <summary>
        /// 搅拌类型
        /// 0：常开
        /// 1：常闭
        /// </summary>
        [Description("搅拌类型 |0：常开  1：常闭")]
        public static  int BlenderType { get; set; } = 1;

        /// <summary>
        /// 是否使用阀岛模块
        /// </summary>
        [Description("是否使用阀岛模块 |0：否  1：是")]
        public static  int ValveTerminal { get; set; } = 0;

        /// <summary>
        /// 是否使用气缸中传感器
        /// 0：不使用
        /// 1：使用
        /// </summary>
        [Description("是否使用气缸中传感器 |0：不使用  1：使用")]
        public static  int MidCylinder { get; set; } = 1;

       

        /// <summary>
        /// 是否使用阻挡气缸
        /// 0：不使用
        /// 1：使用
        /// </summary>
        [Description("是否使用阻挡气缸 |0：不使用  1：使用")]
        public static  int Block { get; set; } = 0;

        

        /// <summary>
        /// 是否使用撑盖模块
        /// 0：无
        /// 1：有
        /// </summary>
        [Description("是否使用撑盖模块 |0：无  1：有")]
        public static  int Tongs_Decompression { get; set; } = 1;

        /// <summary>
        /// 是否使用流量计模块
        /// 0：无
        /// 1：有
        /// </summary>
        [Description("是否使用流量计模块 |0：无  1：有")]
        public static  int UseFlowmeter { get; set; } = 0;

        /// <summary>
        /// 是否使用气缸定位编码器
        /// 0：无
        /// 1：有
        /// </summary>
        [Description("是否使用气缸定位编码器 |0：无  1：有")]
        public static int UseCylinderPositioningEncoder { get; set; } = 0;

        /// <summary>
        /// 是否使用泄压模块
        /// 0：无
        /// 1：有
        /// </summary>
        [Description("是否使用泄压模块 |0：无  1：有")]
        public static  int Decompression { get; set; } = 0;

        /// <summary>
        /// 是否使用后光幕
        /// 0：无
        /// 1：有
        /// </summary>
        [Description("是否使用后光幕 |0：无  1：有")]
        public static  int Sunx_Back { get; set; } = 0;

       

        /// <summary>
        /// 是否使用防护罩
        /// 0：光幕
        /// 1：防护罩
        /// </summary>
        [Description("是否使用防护罩 |0：光幕  1：防护罩")]
        public static  int Shield { get; set; } = 0;


        /// <summary>
        /// 原点位置
        /// 0：1号母液瓶
        /// 1：10号母液瓶
        /// </summary>

        [Description("原点位置|0：1号母液瓶  1：10号母液瓶")]
        public static  int OriginPosition { get; set; } = 1;
    }
}
