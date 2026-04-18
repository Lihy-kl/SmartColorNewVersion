using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Type
{
    /// <summary>
    /// IO基类
    /// </summary>
    internal class BaseIO
    {
        #region 输入

        /// <summary>
        /// 前光幕阻挡信号
        /// </summary>
        [Description("前光幕阻挡信号|输入")]
        public string InPut_Sunx_Stop { get; set; }

        /// <summary>
        /// 左光幕阻挡信号
        /// </summary>
        [Description("左光幕阻挡信号|输入")]
        public string InPut_Sunx_A { get; set; }

        /// <summary>
        /// 右光幕阻挡信号
        /// </summary>
        [Description("右光幕阻挡信号|输入")]
        public string InPut_Sunx_B { get; set; }

        /// <summary>
        /// 后光幕
        /// </summary>
        [Description("后光幕|输入")]
        public string InPut_Sunx_Back { get; set; }

        /// <summary>
        /// 气缸上到位信号
        /// </summary>
        [Description("气缸上到位信号|输入")]
        public string InPut_Cylinder_Up { get; set; }

        /// <summary>
        /// 气缸慢速中限位
        /// </summary>
        [Description("气缸慢速中限位|输入")]
        public string InPut_Slow_Cylinder_Mid { get; set; }

        /// <summary>
        /// 气缸中间信号
        /// </summary>
        [Description("气缸中间信号|输入")]
        public string InPut_Cylinder_Mid { get; set; }

        /// <summary>
        /// 气缸阻挡限位
        /// </summary>
        [Description("气缸阻挡限位|输入")]
        public string InPut_Cylinder_Block { get; set; }

        /// <summary>
        /// 气缸下到位信号
        /// </summary>
        [Description("气缸下到位信号|输入")]
        public string InPut_Cylinder_Down { get; set; }

        /// <summary>
        /// 阻挡出限位
        /// </summary>
        [Description("阻挡出限位|输入")]
        public string InPut_Block_Out { get; set; }

        /// <summary>
        /// 阻挡回限位
        /// </summary>
        [Description("阻挡回限位|输入")]
        public string InPut_Block_In { get; set; }

        /// <summary>
        /// 左抓手松开到位信号
        /// </summary>
        [Description("左抓手松开到位信号|输入")]
        public string InPut_Tongs_A { get; set; }

        /// <summary>
        /// 右抓手松开到位信号
        /// </summary>
        [Description("右抓手松开到位信号|输入")]
        public string InPut_Tongs_B { get; set; }

        /// <summary>
        /// 针筒感应器信号
        /// </summary>
        [Description("针筒感应器信号|输入")]
        public string InPut_Syringe { get; set; }

        /// <summary>
        /// 撑盖到位信号
        /// </summary>
        [Description("撑盖到位信号|输入")]
        public string InPut_SupportCover { get; set; }


        /// <summary>
        /// 接液盘伸出到位信号
        /// </summary>
        [Description("接液盘伸出到位信号|输入")]
        public string InPut_Tray_Out { get; set; }

        /// <summary>
        /// 接液盘收回到位信号
        /// </summary>
        [Description("接液盘收回到位信号|输入")]
        public string InPut_Tray_In { get; set; }

        /// <summary>
        /// 泄压下到位
        /// </summary>
        [Description("泄压下到位|输入")]
        public string InPut_Decompression_Down { get; set; }

        /// <summary>
        /// 泄压上到位
        /// </summary>
        [Description("泄压上到位|输入")]
        public string InPut_Decompression_Up { get; set; }

        /// <summary>
        /// Z轴反限位
        /// </summary>
        [Description("Z轴反限位|输入")]
        public string InPut_Z_Reverse { get; set; }

       
        

       

        #endregion

        #region 输出

        /// <summary>
        /// 气缸下
        /// </summary>
        [Description("气缸下|输出")]
        public string OutPut_Cylinder_Down { get; set; }

        /// <summary>
        /// 气缸上
        /// </summary>
        [Description("气缸上|输出")]
        public string OutPut_Cylinder_Up { get; set; }

        /// <summary>
        /// 气缸慢下阀
        /// </summary>
        [Description("气缸慢下阀|输出")]
        public string OutPut_Slow_Cylinder { get; set; }

        /// <summary>
        /// 阻挡出
        /// </summary>
        [Description("阻挡出|输出")]
        public string OutPut_Block_Out { get; set; }

        /// <summary>
        /// 阻挡回
        /// </summary>
        [Description("阻挡回|输出")]
        public string OutPut_Block_In { get; set; }

        /// <summary>
        /// 抓手张开
        /// </summary>
        [Description("抓手张开|输出")]
        public string OutPut_TongsOff { get; set; }

        /// <summary>
        /// 抓手闭合
        /// </summary>
        [Description("抓手闭合|输出")]
        public string OutPut_TongsOn { get; set; }

        /// <summary>
        /// 接液盘伸出
        /// </summary>
        [Description("接液盘伸出|输出")]
        public string OutPut_Tray { get; set; }

        /// <summary>
        /// 泄压气缸下
        /// </summary>
        [Description("泄压气缸下|输出")]
        public string OutPut_Decompression { get; set; }

        /// <summary>
        /// 停止搅拌打开
        /// </summary>
        [Description("停止搅拌打开|输出")]
        public string OutPut_Blender { get; set; }

        /// <summary>
        /// 废液回收打开
        /// </summary>
        [Description("废液回收打开|输出")]
        public string OutPut_Waste { get; set; }

        /// <summary>
        /// 蜂鸣器打开
        /// </summary>
        [Description("蜂鸣器打开|输出")]
        public string OutPut_Buzzer { get; set; }

        /// <summary>
        /// 加水打开
        /// </summary>
        [Description("加水打开|输出")]
        public string OutPut_Water { get; set; }

        /// <summary>
        /// 洗针进水阀
        /// </summary>
        [Description("洗针进水阀|输出")]
        public string OutPut_Wash_In { get; set; }

        /// <summary>
        /// 洗针排水阀
        /// </summary>
        [Description("洗针排水阀|输出")]
        public string OutPut_Wash_Out { get; set; }

        /// <summary>
        /// 洗针吹气阀
        /// </summary>
        [Description("洗针吹气阀|输出")]
        public string OutPut_Wash_Blow { get; set; }

        

        #endregion
    }
}
