using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>
    /// PLC配置区数据结构，包含所有配置相关的协议项。
    /// 用于描述PLC配置区的所有参数，便于批量读写和业务逻辑处理。
    /// </summary>
    internal class PLC_ConPar
    {
        ///<summary>搅拌配置常开点 （1000）</summary>
        public ProtocolItem BlenderType { get; set; }

        ///<summary>是否有吸光度机配置 （1002）</summary>
        public ProtocolItem UseAbs { get; set; }

        ///<summary>是否配置阀岛 (0:不配置 1:配置) （1004）</summary>
        public ProtocolItem ValveTerminal { get; set; }

        ///<summary>是否有自动开料模块 （1006）</summary>
        public ProtocolItem AutoBrew { get; set; }

        ///<summary>天平配置  （1008）</summary>
        public ProtocolItem BalanceType { get; set; }

        ///<summary>泄压气缸配置  （1010）</summary>
        public ProtocolItem Decompression { get; set; }

        ///<summary>阻挡气缸配置 (单控/双控) （1012）</summary>
        public ProtocolItem BlockType { get; set; }

        ///<summary>气缸中配置 （1014）</summary>
        public ProtocolItem MidCylinder { get; set; }

        ///<summary>后光幕配置 （1016）</summary>
        public ProtocolItem Sunx_Back { get; set; }

        ///<summary>气缸检测延时(秒) （1018）</summary>
        public ProtocolItem Delay_Cylinder { get; set; }

        ///<summary>抓手检测延时(秒)（1020）</summary>
        public ProtocolItem Delay_Tongs { get; set; }

        ///<summary>针检检测延时(秒)（1022）</summary>
        public ProtocolItem Delay_Syringe { get; set; }

        ///<summary>接液盘检测延时(秒)（1024）</summary>
        public ProtocolItem Delay_Tray { get; set; }

        ///<summary>天平清零延时(秒) （1026）</summary>
        public ProtocolItem Balance_Reset { get; set; }

        ///<summary>天平读数延时(秒) （1028）</summary>
        public ProtocolItem Balance_Read { get; set; }

        ///<summary>完成报警延时(秒) （1030）</summary>
        public ProtocolItem Buzzer_Finish { get; set; }

        ///<summary>泄压检测延时(秒) （1032）</summary>
        public ProtocolItem Delay_Decompression { get; set; }

        ///<summary>阻挡气缸检测延时（毫秒） （1034）</summary>
        public ProtocolItem Delay_Block { get; set; }

        ///<summary>前光幕生效Y轴位置 （1036）</summary>
        public ProtocolItem Sunx_Stop_Y { get; set; }

        ///<summary>后光幕生效Y轴位置 （1038）</summary>
        public ProtocolItem Sunx_Back_Y { get; set; }

        ///<summary>小针筒最大脉冲 （1040）</summary>
        public ProtocolItem S_MaxPulse { get; set; }

        ///<summary>大针筒最大脉冲 （1042）</summary>
        public ProtocolItem B_MaxPulse { get; set; }

        ///<summary>废液桶最大液量 （1044）</summary>
        public ProtocolItem BalanceMaxWeight { get; set; }

        ///<summary>开料范围下限 （1046）</summary>
        public ProtocolItem BrewBottleWeightMin { get; set; }

        ///<summary>开料范围上限 （1048）</summary>
        public ProtocolItem BrewBottleWeightMax { get; set; }

        ///<summary>气缸高度偏差脉冲 （1050）</summary>
        public ProtocolItem CylinderHeightDeviationPulse { get; set; }

        ///<summary>气缸中位置 （1052）</summary>
        public ProtocolItem PositionInCylinder { get; set; }

        ///<summary>气缸慢速中1位置 （1054）</summary>
        public ProtocolItem PositionInCylinderSlow1 { get; set; }

        ///<summary>气缸慢速中2位置 （1056）</summary>
        public ProtocolItem PositionInCylinderSlow2 { get; set; }

        ///<summary>气缸慢速中3位置 （1058）</summary>
        public ProtocolItem PositionInCylinderSlow3 { get; set; }

        ///<summary>气缸到阻挡位位置 （1060）</summary>
        public ProtocolItem CylinderStopPosition { get; set; }

        ///<summary>气缸下位置 （1062）</summary>
        public ProtocolItem CylinderDownPosition { get; set; }

        ///<summary>气缸定位范围 （1064）</summary>
        public ProtocolItem CylinderPositioningRange { get; set; }

        ///<summary>是否使用气缸定位编码器 （1066）</summary>
        public ProtocolItem UseCylinderPositioningEncoder { get; set; }


    }
}
