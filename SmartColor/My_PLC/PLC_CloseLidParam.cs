using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>关盖参数</summary>
    internal class PLC_CloseLidParam : PLC_SemiAutoParamBase
    {
        /// <summary>X轴是否关矢能 D801 (0=开, 1=关)</summary>
        public ProtocolItem XEnableOff { get; set; }
        /// <summary>Y轴是否关矢能 D802 (0=开, 1=关)</summary>
        public ProtocolItem YEnableOff { get; set; }

        /// <summary>是否配置抓手撑盖 D803 (0=无, 1=有)</summary>
        public ProtocolItem Tongs_Decompression { get; set; }

        /// <summary>撑盖偏移 D804-D805</summary>
        public ProtocolItem OffsetPosition { get; set; }

        /// <summary>
        /// 关盖参数构造函数
        /// </summary>
        /// <param name="xEnableOff">X轴是否关矢能</param>
        /// <param name="yEnableOff">Y轴是否关矢能</param>
        /// <param name="tongsDecompression">是否配置抓手撑盖</param>
        /// <param name="offsetPosition">撑盖偏移</param>
        public PLC_CloseLidParam(short xEnableOff, short yEnableOff, short tongsDecompression, int offsetPosition) : base(PLC.SemiAutomaticOperation.CloseLid)
        {
            XEnableOff = new ProtocolItem(801, typeof(short), xEnableOff);
            YEnableOff = new ProtocolItem(802, typeof(short), yEnableOff);
            Tongs_Decompression = new ProtocolItem(803, typeof(short), tongsDecompression);
            OffsetPosition = new ProtocolItem(804, typeof(int), offsetPosition);
        }

        /// <summary>
        /// 关盖参数构造函数
        /// </summary>
        public PLC_CloseLidParam() : base(PLC.SemiAutomaticOperation.CloseLid)
        {
            XEnableOff = new ProtocolItem(801, typeof(short), 1);
            YEnableOff = new ProtocolItem(802, typeof(short), 1);
            Tongs_Decompression = new ProtocolItem(803, typeof(short), My_ConPar.Hardware.Tongs_Decompression);
            OffsetPosition = new ProtocolItem(804, typeof(int), My_ConPar.Other.SupportCoverY);
        }
    }

}
