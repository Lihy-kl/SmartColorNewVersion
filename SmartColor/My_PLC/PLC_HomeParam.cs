using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>回原点参数</summary>
    internal class PLC_HomeParam : PLC_SemiAutoParamBase
    {
        /// <summary> X轴回原点偏移量 /// </summary>
        public ProtocolItem X { get; set; } // D801

        /// <summary> Y轴回原点偏移量 </summary>
        public ProtocolItem Y { get; set; } // D803

        /// <summary> Z轴回原点偏移量 </summary>
        public ProtocolItem Z { get; set; } // D805

        /// <summary>
        /// 回原点参数构造函数
        /// </summary>
        /// <param name="x">X轴回原点偏移量</param>
        /// <param name="y">Y轴回原点偏移量</param>
        /// <param name="z">Z轴回原点偏移量</param>
        public PLC_HomeParam(int x, int y, int z)
            : base(PLC.SemiAutomaticOperation.Home)
        {
            X = new ProtocolItem(801, typeof(int), x);
            Y = new ProtocolItem(803, typeof(int), y);
            Z = new ProtocolItem(805, typeof(int), z);
        }

        /// <summary>
        /// 回原点参数构造函数(采用配置参数)
        /// </summary>
        public PLC_HomeParam()
             : base(PLC.SemiAutomaticOperation.Home)
        {
            var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;
            X = new ProtocolItem(801, typeof(int), motion.Home_X_Offset);
            Y = new ProtocolItem(803, typeof(int), motion.Home_Y_Offset);
            Z = new ProtocolItem(805, typeof(int), motion.Home_Z_Offset);
        }
    }

}
