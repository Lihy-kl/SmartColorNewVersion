using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>放干布参数</summary>
    internal class PLC_PutDryClothParam : PLC_SemiAutoParamBase
    {
        /// <summary>开夹放布脉冲 D801</summary>
        public ProtocolItem OpenClampPulse { get; set; }
        /// <summary>小针筒驱动速度 D803</summary>
        public ProtocolItem SmallSyringeSpeed { get; set; }
        /// <summary>小针筒加减速时间 D805</summary>
        public ProtocolItem SmallSyringeAcc { get; set; }

        /// <summary>
        /// 放干布参数构造函数
        /// </summary>
        /// <param name="openClampPulse">开夹放布脉冲</param>
        /// <param name="smallSyringeSpeed">小针筒驱动速度</param>
        /// <param name="smallSyringeAcc">小针筒加减速时间</param>
        public PLC_PutDryClothParam(
            int openClampPulse,
            int smallSyringeSpeed,
            int smallSyringeAcc)
            : base(PLC.SemiAutomaticOperation.PutDryCloth)
        {
            OpenClampPulse = new ProtocolItem(801, typeof(int), openClampPulse);
            SmallSyringeSpeed = new ProtocolItem(803, typeof(int), smallSyringeSpeed);
            SmallSyringeAcc = new ProtocolItem(805, typeof(int), smallSyringeAcc);
        }

        /// <summary>
        /// 放干布参数构造函数（自动从配置获取）
        /// </summary>
        public PLC_PutDryClothParam()
            : base(PLC.SemiAutomaticOperation.PutDryCloth)
        {
            var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;
            OpenClampPulse = new ProtocolItem(801, typeof(int), My_ConPar.Other.OpenPulse);
            SmallSyringeSpeed = new ProtocolItem(803, typeof(int), motion?.Move_S_MinHSpeed ?? 0);
            SmallSyringeAcc = new ProtocolItem(805, typeof(int), ((motion?.Move_S_USpeed ?? 500) * 10));
        }
    }
}
