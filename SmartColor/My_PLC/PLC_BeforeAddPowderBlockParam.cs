using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>加粉前气缸到阻挡位参数</summary>
    internal class PLC_BeforeAddPowderBlockParam : PLC_SemiAutoParamBase
    {
        // 根据实际协议补充参数
        public PLC_BeforeAddPowderBlockParam() : base(PLC.SemiAutomaticOperation.BeforeAddPowderBlock) { }
    }
}
