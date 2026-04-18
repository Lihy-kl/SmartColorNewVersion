using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>开盖参数</summary>
    internal class PLC_OpenLidParam : PLC_SemiAutoParamBase
    {
        /// <summary>屏蔽针筒感应器 D801</summary>
        public ProtocolItem IgnoreSyringeSensor { get; set; }
        /// <summary>X轴是否关矢能 D802 (0=开, 1=关)</summary>
        public ProtocolItem XEnableOff { get; set; }
        /// <summary>Y轴是否关矢能 D803 (0=开, 1=关)</summary>
        public ProtocolItem YEnableOff { get; set; }

        /// <summary>锁止信号 D804</summary>
        public ProtocolItem LockSignal { get; set; }

        /// <summary>
        /// 开盖参数构造函数
        /// </summary>
        /// <param name="ignoreSyringeSensor">屏蔽针筒感应器</param>
        /// <param name="xEnableOff">X轴是否关矢能 0=开, 1=关</param>
        /// <param name="yEnableOff">Y轴是否关矢能 0=开, 1=关</param>
        /// <param name="lockSignal">锁止信号</param>
        public PLC_OpenLidParam(
            short ignoreSyringeSensor,
            short xEnableOff,
            short yEnableOff,
          
            short lockSignal) : base(PLC.SemiAutomaticOperation.OpenLid)
        {
            IgnoreSyringeSensor = new ProtocolItem(801, typeof(short), ignoreSyringeSensor);
            XEnableOff = new ProtocolItem(802, typeof(short), xEnableOff);
            YEnableOff = new ProtocolItem(803, typeof(short), yEnableOff);
            LockSignal = new ProtocolItem(804, typeof(short), lockSignal);
        }

        /// <summary>
        /// 开盖参数构造函数
        /// </summary>
        /// <param name="lockSignal">锁止信号</param>
        public PLC_OpenLidParam(short lockSignal)
            : base(PLC.SemiAutomaticOperation.OpenLid)
        {
            IgnoreSyringeSensor = new ProtocolItem(801, typeof(short), (short)My_ConPar.Choices.IgnoreSyringeSensor);
            XEnableOff = new ProtocolItem(802, typeof(short), 1);
            YEnableOff = new ProtocolItem(803, typeof(short), 1);
            LockSignal = new ProtocolItem(804, typeof(short), lockSignal);
        }
    }
}
