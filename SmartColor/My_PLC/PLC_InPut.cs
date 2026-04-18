using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>
    /// PLC输入区数据结构，包含所有输入相关的协议项（寄存器地址、数据类型、当前值等）。
    /// 用于描述PLC输入区的所有数据点，便于批量读写和业务逻辑处理。
    /// </summary>
    internal class PLC_InPut
    {
        /// <summary>X轴A相反馈脉冲 （3500）</summary>
        public ProtocolItem X_A_FeedbackLine { get; set; }

        /// <summary>X轴B相反馈脉冲 （3502）</summary>
        public ProtocolItem X_B_FeedbackLine { get; set; }

        /// <summary>Y轴A相反馈脉冲 （3504）</summary>
        public ProtocolItem Y_A_FeedbackLine { get; set; }

        /// <summary>Y轴B相反馈脉冲 （3506）</summary>
        public ProtocolItem Y_B_FeedbackLine { get; set; }

        /// <summary>Z轴A相反馈脉冲 （3508）</summary>
        public ProtocolItem Z_A_FeedbackLine { get; set; }

        /// <summary>Z轴B相反馈脉冲 （3510）</summary>
        public ProtocolItem Z_B_FeedbackLine { get; set; }

        /// <summary>X轴异常 （3512）</summary>
        public ProtocolItem X_Exceptional { get; set; }

        /// <summary>Y轴异常 （3514）</summary>
        public ProtocolItem Y_Exceptional { get; set; }

        /// <summary>Z轴异常 （3516）</summary>
        public ProtocolItem Z_Exceptional { get; set; }

        /// <summary> X轴准备好 （3518）</summary>
        public ProtocolItem X_Ready { get; set; }

        /// <summary>Y轴准备好 （3520）</summary>
        public ProtocolItem Y_Ready { get; set; }

        /// <summary>Z轴准备好 （3522）</summary>
        public ProtocolItem Z_Ready { get; set; }

        /// <summary>X轴正限位 （3524）</summary>
        public ProtocolItem X_Corotation { get; set; }

        /// <summary>X轴反限位 （3526）</summary>
        public ProtocolItem X_Reverse { get; set; }

        /// <summary>X轴原点 （3528）</summary>
        public ProtocolItem X_Origin { get; set; }

        /// <summary>Y轴正限位 （3530）</summary>
        public ProtocolItem Y_Corotation { get; set; }

        /// <summary>Y轴反限位 （3532）</summary>
        public ProtocolItem Y_Reverse { get; set; }

        /// <summary>Y轴原点 （3534）</summary>
        public ProtocolItem Y_Origin { get; set; }

        /// <summary>Z轴正限位 （3536）</summary>
        public ProtocolItem Z_Corotation { get; set; }

        /// <summary>Z轴反限位 （3538）</summary>
        public ProtocolItem Z_Reverse { get; set; }

        /// <summary>Z轴原点 （3540）</summary>
        public ProtocolItem Z_Origin { get; set; }

        /// <summary>前光幕 （3542）</summary>
        public ProtocolItem Sunx_Stop { get; set; }

        /// <summary>左光幕 （3544）</summary>
        public ProtocolItem Sunx_A { get; set; }

        /// <summary>右光幕 （3546）</summary>
        public ProtocolItem Sunx_B { get; set; }

        /// <summary>针筒 （3548）</summary>
        public ProtocolItem Syringe { get; set; }

        /// <summary>抓手A （3550）</summary>
        public ProtocolItem Tongs_A { get; set; }

        /// <summary>抓手B （3552）</summary>
        public ProtocolItem Tongs_B { get; set; }

        /// <summary>上限位 （3554）</summary>
        public ProtocolItem Cylinder_Up { get; set; }

        /// <summary>中限位 （3556）</summary>
        public ProtocolItem Cylinder_Mid { get; set; }

        /// <summary>下限位 （3558）</summary>
        public ProtocolItem Cylinder_Down { get; set; }

        /// <summary>接液盘出 （3560）</summary>
        public ProtocolItem Tray_Out { get; set; }

        /// <summary>接液盘回 （3562）</summary>
        public ProtocolItem Tray_In { get; set; }

        /// <summary>泄压上限位 （3564）</summary>
        public ProtocolItem Decompression_Up { get; set; }

        /// <summary>泄压下限位 （3566）</summary>
        public ProtocolItem Decompression_Down { get; set; }

        /// <summary>阻挡出限位 （3568）</summary>
        public ProtocolItem Block_Out { get; set; }

        /// <summary> 阻挡回限位 （3570）</summary>
        public ProtocolItem Block_In { get; set; }

        /// <summary> 气缸慢速中限位 （3572）</summary>
        public ProtocolItem Slow_Cylinder_Mid { get; set; }

        /// <summary> 气缸阻挡限位 （3574）</summary>
        public ProtocolItem Cylinder_Block { get; set; }

        /// <summary>后光幕 （3576）</summary>
        public ProtocolItem Sunx_Back { get; set; }

        /// <summary>撑盖气缸开到位 （3578）</summary>
        public ProtocolItem SupportCover { get; set; }

        /// <summary>气缸到慢速中2 （3580）</summary>
        public ProtocolItem Slow_Cylinder_Mid_2 { get; set; }

        /// <summary>气缸到慢速中3 （3582）</summary>
        public ProtocolItem Slow_Cylinder_Mid_3 { get; set; }

        /// <summary>转子缸急停信号 （3584）</summary>
        public ProtocolItem RotorCylinderStop { get; set; }

        /// <summary>备用1 （3586）</summary>
        public ProtocolItem Backup_1 { get; set; }

        /// <summary>液位开关/气压表 （3588）</summary>
        public ProtocolItem Barometer { get; set; }

        /// <summary>备用2 （3590）</summary>
        public ProtocolItem Backup_2 { get; set; }

        /// <summary>备用3 （3592）</summary>
        public ProtocolItem Backup_3 { get; set; }

        /// <summary>备用4 （3594）</summary>
        public ProtocolItem Backup_4 { get; set; }

        /// <summary>开料气缸上升到位 （3596）</summary>
        public ProtocolItem Brew_Cylinder_Up { get; set; }

        /// <summary>开料气缸下降到位 （3598）</summary>
        public ProtocolItem Brew_Cylinder_Down { get; set; }

        /// <summary>开料摆臂伸到位 （3600）</summary>
        public ProtocolItem Brew_SwingArmOut { get; set; }

        /// <summary>开料摆臂缩到位 （3602）</summary>
        public ProtocolItem Brew_SwingArmIn { get; set; }

        /// <summary>洗瓶夹瓶伸到位 （3604）</summary>
        public ProtocolItem Brew_BWHC_Out { get; set; }

        /// <summary>洗瓶夹瓶缩到位 （3606）</summary>
        public ProtocolItem Brew_BWHC_In { get; set; }

        /// <summary>洗瓶上翻限位 （3608）</summary>
        public ProtocolItem Brew_BWHC_Up { get; set; }

        /// <summary>洗瓶下翻限位 （3610）</summary>
        public ProtocolItem Brew_BWHC_Down { get; set; }

        /// <summary>备用5 （3612）</summary>
        public ProtocolItem Backup_5 { get; set; }

        /// <summary>热水箱浮球 （3614）</summary>
        public ProtocolItem HotWaterTankFloatBall { get; set; }

        /// <summary>备用6 （3616）</summary>
        public ProtocolItem Backup_6 { get; set; }

    }
}
