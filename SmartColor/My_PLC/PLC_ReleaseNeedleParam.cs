using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>放针参数</summary>
    internal class PLC_ReleaseNeedleParam : PLC_SemiAutoParamBase
    {
        /// <summary>驱动速度 D803</summary>
        public ProtocolItem SyringeSpeed { get; set; }
        /// <summary>加减速 D805</summary>
        public ProtocolItem SyringeAcc { get; set; }

        /// <summary>
        /// 放针参数构造函数
        /// </summary>
        /// <param name="syringeSpeed">驱动速度</param>
        /// <param name="syringeAcc">加减速</param>
        public PLC_ReleaseNeedleParam(int syringeSpeed, int syringeAcc) : base(PLC.SemiAutomaticOperation.ReleaseNeedle)
        {

            SyringeSpeed = new ProtocolItem(801, typeof(int), syringeSpeed);
            SyringeAcc = new ProtocolItem(803, typeof(int), syringeAcc);
        }

        /// <summary>
        /// 放针参数构造函数
        /// </summary>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        public PLC_ReleaseNeedleParam(int syringeType) : base(PLC.SemiAutomaticOperation.ReleaseNeedle)
        {
            var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;
            SyringeSpeed = new ProtocolItem(801, typeof(int), syringeType == 0 ?
                motion.Move_S_HSpeed : motion.Move_B_HSpeed);
            SyringeAcc = new ProtocolItem(803, typeof(int), syringeType == 0 ?
                motion.Move_S_USpeed : motion.Move_B_USpeed);
        }
    }

}
