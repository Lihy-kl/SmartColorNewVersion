using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>
    /// PLC接收区数据结构，包含所有接收相关的协议项。
    /// 用于描述PLC接收区的所有数据点，便于批量读写和业务逻辑处理。
    /// </summary>
    internal class PLC_Receive
    {
        /// <summary>机械手动作完成/异常编号（900）</summary>
        public ProtocolItem RobotActionCode { get; set; }

        /// <summary>电子秤数据（双字）（901）</summary>
        public ProtocolItem BalanceData { get; set; }

        /// <summary>光幕信号（903）</summary>
        public ProtocolItem LightCurtainSignal { get; set; }

        /// <summary>执行完成（904）</summary>
        public ProtocolItem ExecuteFinished { get; set; }

        /// <summary>输入点(双字)X0-X17（905）</summary>
        public ProtocolItem InputX0_X17 { get; set; }

        /// <summary>输出点(双字)Y0-Y17（907）</summary>
        public ProtocolItem OutputY0_Y17 { get; set; }

        /// <summary>X轴当前坐标（909）</summary>
        public ProtocolItem X_CurrentPosition { get; set; }

        /// <summary>Y轴当前坐标（911）</summary>
        public ProtocolItem Y_CurrentPosition { get; set; }

        /// <summary>Z轴当前坐标（保留）（913）</summary>
        public ProtocolItem Z_CurrentPosition { get; set; }

        /// <summary>X轴当前速度（915）</summary>
        public ProtocolItem X_CurrentSpeed { get; set; }

        /// <summary>Y轴当前速度（917）</summary>
        public ProtocolItem Y_CurrentSpeed { get; set; }

        /// <summary>Z轴当前速度（919）</summary>
        public ProtocolItem Z_CurrentSpeed { get; set; }

        /// <summary>X轴报警代码（921）</summary>
        public ProtocolItem X_AlarmCode { get; set; }

        /// <summary>Y轴报警代码（922）</summary>
        public ProtocolItem Y_AlarmCode { get; set; }

        /// <summary>输入点(双字)X20-X57（923）</summary>
        public ProtocolItem InputX20_X57 { get; set; }

        /// <summary>输出点(双字)Y20-Y57（925）</summary>
        public ProtocolItem OutputY20_Y57 { get; set; }

        /// <summary>PLC程序版本（年）（927）</summary>
        public ProtocolItem PLCVersionYear { get; set; }

        /// <summary>PLC程序版本（月日）（928）</summary>
        public ProtocolItem PLCVersionMonthDay { get; set; }

        /// <summary>A助剂步进当前速度（929）</summary>
        public ProtocolItem AAgentStepCurrentSpeed { get; set; }

        /// <summary>B助剂步进当前速度（931）</summary>
        public ProtocolItem BAgentStepCurrentSpeed { get; set; }

        /// <summary>流量计脉冲数（933）</summary>
        public ProtocolItem FlowMeterPulse { get; set; }

        /// <summary>气缸上的校验速度（935）</summary>
        public ProtocolItem CylinderCheckSpeed { get; set; }

        /// <summary>转盘当前位置（937）</summary>
        public ProtocolItem TurntableCurrentPosition { get; set; }

        /// <summary>转盘速度（939）</summary>
        public ProtocolItem TurntableSpeed { get; set; }

        /// <summary>转盘报警代码（941）</summary>
        public ProtocolItem TurntableAlarmCode { get; set; }

        /// <summary>已加粉重量（942）</summary>
        public ProtocolItem PowderWeight { get; set; }

        /// <summary>转盘动作返回值（0=无，1=未完成，2=完成，3=预留）（944）</summary>
        public ProtocolItem TurntableActionResult { get; set; }

        /// <summary>洗瓶工位动作返回值（0=无，1=未完成，2=完成，3=预留）（945）</summary>
        public ProtocolItem BottleWasherActionResult { get; set; }

        /// <summary>完成该点动动作使用时间（946）</summary>
        public ProtocolItem UseTime { get; set; }

        /// <summary>气缸编码器位置（947-948）</summary>
        public ProtocolItem CylinderEncoderPosition { get; set; }

    }
}
