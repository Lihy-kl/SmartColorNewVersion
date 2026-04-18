using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>加溶解剂参数</summary>
    internal class PLC_AddSolventParam : PLC_SemiAutoParamBase
    {
        /// <summary>加溶解剂时间 D801 (/s)</summary>
        public ProtocolItem SolventTime { get; set; }

        /// <summary>
        /// 加溶解剂参数构造函数
        /// </summary>
        /// <param name="solventTime">加溶解剂时间/s</param>
        public PLC_AddSolventParam(double solventTime)
            : base(PLC.SemiAutomaticOperation.AddSolvent)
        {
            SolventTime = new ProtocolItem(801, typeof(int), (int)(solventTime * 10));

        }
    }

}
