using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>动作检查参数</summary>
    internal class PLC_ActionCheckParam : PLC_SemiAutoParamBase
    {
        /// <summary>屏蔽针筒感应器 D801</summary>
        public ProtocolItem IgnoreSyringeSensor { get; set; }

        /// <summary>
        /// 动作检查参数构造函数
        /// </summary>
        /// <param name="ignoreSyringeSensor">屏蔽针筒感应器 0：否 1：是</param>
        public PLC_ActionCheckParam(int ignoreSyringeSensor)
            : base(PLC.SemiAutomaticOperation.ActionCheck)
        {
            IgnoreSyringeSensor = new ProtocolItem(801, typeof(short), ignoreSyringeSensor);
        }

        /// <summary>
        /// 动作检查参数构造函数
        /// </summary>
        public PLC_ActionCheckParam()
          : base(PLC.SemiAutomaticOperation.ActionCheck)
        {
            IgnoreSyringeSensor = new ProtocolItem(801, typeof(short), My_ConPar.Choices.IgnoreSyringeSensor);
        }
    }
}
