using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>机械手抓取参数</summary>
    internal class PLC_RobotHandGraspingParam : PLC_SemiAutoParamBase
    {
        /// <summary>抓取类型 D801（0=布夹 1=UV针筒 2=洗针针筒 3=PH针筒 4=母液瓶夹子 5=粉罐）</summary>
        public ProtocolItem GraspType { get; set; }
        /// <summary>针筒规格 D802（0=小针筒 1=大针筒）</summary>
        public ProtocolItem SyringeType { get; set; }
        /// <summary>Z轴偏移量</summary>
        public ProtocolItem Offset { get; set; }

        /// <summary>
        /// 机械手抓取参数构造函数（全部参数）
        /// </summary>
        /// <param name="graspType">抓取类型（0=布夹 1=UV针筒 2=洗针针筒 3=PH针筒 4=母液瓶夹子 5=粉罐）</param>
        /// <param name="syringeType">针筒规格</param>
        public PLC_RobotHandGraspingParam(short graspType, short syringeType)
            : base(PLC.SemiAutomaticOperation.RobotHandGrasping)
        {
            var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;
            GraspType = new ProtocolItem(801, typeof(short), graspType);
            SyringeType = new ProtocolItem(802, typeof(short), syringeType);
            Offset = new ProtocolItem(803, typeof(int), motion.Home_Z_Offset); 
        }
    }
}
