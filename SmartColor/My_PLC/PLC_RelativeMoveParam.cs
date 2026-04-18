using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>相对移动参数</summary>
    internal class PLC_RelativeMoveParam : PLC_SemiAutoParamBase
    {
        /// <summary>轴号 D801 0：X 1:Y 2:Z 3:转盘</summary>
        public ProtocolItem Axis { get; set; } // D801

        /// <summary>坐标</summary>
        public ProtocolItem P { get; set; } // D802

        ///<summary>速度</summary>
        public ProtocolItem Speed { get; set; } // D804

        ///<summary>加减速</summary>
        public ProtocolItem Acc { get; set; } // D806

        /// <summary>
        /// 相对移动参数构造函数
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">坐标</param>
        /// <param name="speed">速度</param>
        /// <param name="acc">加减速</param>
        public PLC_RelativeMoveParam(short axis, int position, int speed, int acc)
            : base(PLC.SemiAutomaticOperation.RelativeMove)
        {
            Axis = new ProtocolItem(801, typeof(short), axis);
            P = new ProtocolItem(802, typeof(int), position);
            Speed = new ProtocolItem(804, typeof(int), speed);
            Acc = new ProtocolItem(806, typeof(int), acc);
        }

    }
}
