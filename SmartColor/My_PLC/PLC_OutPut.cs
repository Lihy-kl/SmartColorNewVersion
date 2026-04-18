using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>
    /// PLC输出区数据结构，包含所有输出相关的协议项。
    /// 用于描述PLC输出区的所有数据点，便于批量读写和业务逻辑处理。
    /// </summary>
    internal class PLC_OutPut
    {
        /// <summary>X轴脉冲 （3900）</summary>
        public ProtocolItem X_Pulse { get; set; }
        /// <summary>X轴方向 （3902）</summary>
        public ProtocolItem X_Direction { get; set; }

        /// <summary>Y轴脉冲 （3904）</summary>
        public ProtocolItem Y_Pulse { get; set; }
        /// <summary>Y轴方向 （3906）</summary>
        public ProtocolItem Y_Direction { get; set; }

        /// <summary>Z轴脉冲 （3908）</summary>
        public ProtocolItem Z_Pulse { get; set; }

        /// <summary>Z轴方向 （3910）</summary>
        public ProtocolItem Z_Direction { get; set; }

        /// <summary>X轴使能 （3912）</summary>
        public ProtocolItem X_Power { get; set; }

        /// <summary>Y轴使能 （3914）</summary>
        public ProtocolItem Y_Power { get; set; }

        /// <summary>Z轴使能 （3916）</summary>
        public ProtocolItem Z_Power { get; set; }

        /// <summary>X轴复位 （3918）</summary>
        public ProtocolItem X_Reset { get; set; }

        /// <summary>Y轴复位 （3920）</summary>
        public ProtocolItem Y_Reset { get; set; }

        /// <summary>Z轴复位 （3922）</summary>
        public ProtocolItem Z_Reset { get; set; }

        /// <summary>搅拌停 （3924）</summary>
        public ProtocolItem Blender { get; set; }

        /// <summary>报警 （3926）</summary>
        public ProtocolItem Buzzer { get; set; }

        /// <summary>抓手合 （3928）</summary>
        public ProtocolItem TongsOn { get; set; }

        /// <summary>抓手开 （3930）</summary>
        public ProtocolItem TongsOff { get; set; }

        /// <summary>气缸上 （3932）</summary>
        public ProtocolItem Cylinder_Up { get; set; }

        /// <summary>气缸下 （3934）</summary>
        public ProtocolItem Cylinder_Down { get; set; }

        /// <summary>接液盘 （3936）</summary>
        public ProtocolItem Tray { get; set; }

        /// <summary>抽废液 （3938）</summary>
        public ProtocolItem Waste { get; set; }

        /// <summary>加水 （3940）</summary>
        public ProtocolItem Water { get; set; }

        /// <summary>泄压 （3942）</summary>
        public ProtocolItem Decompression { get; set; }

        /// <summary>红灯 （3944）</summary>
        public ProtocolItem RedLight { get; set; }

        /// <summary>绿灯 （3946）</summary>
        public ProtocolItem GreenLight { get; set; }

        /// <summary>阻挡出 （3948）</summary>
        public ProtocolItem Block_Out { get; set; }

        /// <summary>阻挡回 （3950）</summary>
        public ProtocolItem Block_In { get; set; }

        /// <summary>气缸慢下阀 （3952）</summary>
        public ProtocolItem Slow_Cylinder { get; set; }

        /// <summary>洗针进水阀 （3954）</summary>
        public ProtocolItem Wash_In { get; set; }

        /// <summary>洗针排水阀 （3956）</summary>
        public ProtocolItem Wash_Out { get; set; }

        /// <summary>洗针吹气阀 （3958）</summary>
        public ProtocolItem Wash_Blow { get; set; }

        /// <summary>备用1 （3960）</summary>
        public ProtocolItem Backup_1 { get; set; }

        /// <summary>加溶解剂 （3962）</summary>
        public ProtocolItem Solvent { get; set; }

        /// <summary>备用2 （3964）</summary>
        public ProtocolItem Backup_2 { get; set; }

        /// <summary>备用3 （3966）</summary>
        public ProtocolItem Backup_3 { get; set; }

        /// <summary>开料气缸上升 （3968）</summary>
        public ProtocolItem Brew_Cylinder_Up { get; set; }

        /// <summary>开料气缸下降 （3970）</summary>
        public ProtocolItem Brew_Cylinder_Down { get; set; }

        /// <summary>开料热水阀 （3972）</summary>
        public ProtocolItem Brew_HotWater { get; set; }

        /// <summary>开料大冷水阀 （3974）</summary>
        public ProtocolItem Brew_BigColdWater { get; set; }

        /// <summary>开料小冷水阀 （3976）</summary>
        public ProtocolItem Brew_SmallColdWater { get; set; }

        /// <summary>开料热水泵 （3978）</summary>
        public ProtocolItem Brew_HotWaterPump { get; set; }

        /// <summary>摆臂伸 （3980）</summary>
        public ProtocolItem Brew_SwingArmOut { get; set; }

        /// <summary>摆臂缩 （3982）</summary>
        public ProtocolItem Brew_SwingArmIn { get; set; }

        /// <summary>洗瓶夹瓶伸 （3984）</summary>
        public ProtocolItem Brew_BWHC_Out { get; set; }

        /// <summary>洗瓶夹瓶缩 （3986）</summary>
        public ProtocolItem Brew_BWHC_In { get; set; }

        /// <summary>备用4 （3988）</summary>
        public ProtocolItem Backup_4 { get; set; }

        /// <summary>水箱进热水 （3990）</summary>
        public ProtocolItem HotWaterTankInletValve { get; set; }

        /// <summary>洗瓶上翻 （3992）</summary>
        public ProtocolItem Brew_BWHC_Up { get; set; }

        /// <summary>洗瓶下翻 （3994）</summary>
        public ProtocolItem Brew_BWHC_Down { get; set; }

        /// <summary>洗瓶吹气阀 （3996）</summary>
        public ProtocolItem Brew_Wash_Blow { get; set; }

        /// <summary>洗瓶进冷水阀 （3998）</summary>
        public ProtocolItem Brew_WashColdWater { get; set; }

        /// <summary>洗瓶热风 （4000）</summary>
        public ProtocolItem Brew_HotWind { get; set; }

        /// <summary> 热水箱加热  （4002）</summary>
        public ProtocolItem Brew_WaterHeaterHeating { get; set; }

    }
}
