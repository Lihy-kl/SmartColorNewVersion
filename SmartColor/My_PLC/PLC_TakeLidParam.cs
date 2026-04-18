using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>取盖参数</summary>
    internal class PLC_TakeLidParam : PLC_SemiAutoParamBase
    {
        /// <summary>X轴是否关矢能 D801 (0=开, 1=关)</summary>
        public ProtocolItem XEnableOff { get; set; }
        /// <summary>Y轴是否关矢能 D802 (0=开, 1=关)</summary>
        public ProtocolItem YEnableOff { get; set; }
        /// <summary>屏蔽针筒感应器 (0=否, 1=是)</summary>
        public ProtocolItem IgnoreSyringeSensor { get; set; }

        /// <summary>
        /// 取盖参数构造函数
        /// </summary>
        /// <param name="xEnableOff">X轴是否关矢能 0=开, 1=关</param>
        /// <param name="yEnableOff">Y轴是否关矢能 0=开, 1=关</param>
        public PLC_TakeLidParam(short ignoreSyringeSensor,  short xEnableOff, short yEnableOff)
            : base(PLC.SemiAutomaticOperation.TakeLid)
        {
            IgnoreSyringeSensor = new ProtocolItem(801, typeof(short), ignoreSyringeSensor);
            XEnableOff = new ProtocolItem(802, typeof(short), xEnableOff);
            YEnableOff = new ProtocolItem(803, typeof(short), yEnableOff);
        }

        /// <summary>
        /// 取盖参数构造函数
        /// </summary>
        public PLC_TakeLidParam()
            : base(PLC.SemiAutomaticOperation.TakeLid)
        {
            IgnoreSyringeSensor = new ProtocolItem(801, typeof(short), My_ConPar.Choices.IgnoreSyringeSensor);
            XEnableOff = new ProtocolItem(802, typeof(short), 1);
            YEnableOff = new ProtocolItem(803, typeof(short), 1);
        }
    }

}
