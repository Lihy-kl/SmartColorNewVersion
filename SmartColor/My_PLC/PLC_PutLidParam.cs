using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>放盖参数</summary>
    internal class PLC_PutLidParam : PLC_SemiAutoParamBase
    {
        /// <summary>X轴是否关矢能 D801 (0=开, 1=关)</summary>
        public ProtocolItem XEnableOff { get; set; }
        /// <summary>Y轴是否关矢能 D802 (0=开, 1=关)</summary>
        public ProtocolItem YEnableOff { get; set; }

        /// <summary>
        /// 放盖参数构造函数
        /// </summary>
        /// <param name="xEnableOff">X轴是否关矢能 0=开, 1=关</param>
        /// <param name="yEnableOff">Y轴是否关矢能 0=开, 1=关</param>
        public PLC_PutLidParam(short xEnableOff, short yEnableOff)
            : base(PLC.SemiAutomaticOperation.PutLid)
        {
            XEnableOff = new ProtocolItem(801, typeof(short), xEnableOff);
            YEnableOff = new ProtocolItem(802, typeof(short), yEnableOff);
        }

        /// <summary>
        /// 放盖参数构造函数
        /// </summary>
        public PLC_PutLidParam()
            : base(PLC.SemiAutomaticOperation.PutLid)
        {
            XEnableOff = new ProtocolItem(801, typeof(short), 1);
            YEnableOff = new ProtocolItem(802, typeof(short), 1);
        }
    }
}
