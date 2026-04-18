using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>撑盖参数</summary>
    internal class PLC_SupportCoverParam : PLC_SemiAutoParamBase
    {
        // 根据实际协议补充参数
        public PLC_SupportCoverParam() : base(PLC.SemiAutomaticOperation.SupportCover) { }
    }
}
