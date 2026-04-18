using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>泄压参数</summary>
    internal class PLC_DecompressParam : PLC_SemiAutoParamBase
    {
        // 根据实际协议补充参数
        public PLC_DecompressParam() : base(PLC.SemiAutomaticOperation.Decompress) { }
    }

}
