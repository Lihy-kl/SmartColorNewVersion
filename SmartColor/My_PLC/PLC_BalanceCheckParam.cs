using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>天平检查参数</summary>
    internal class PLC_BalanceCheckParam : PLC_SemiAutoParamBase
    {
        /// <summary>
        /// 天平检查参数构造函数
        /// </summary>
        public PLC_BalanceCheckParam() : base(PLC.SemiAutomaticOperation.BalanceCheck) { }
    }
}
