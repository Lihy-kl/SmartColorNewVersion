using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>取湿布参数</summary>
    internal class PLC_TakeWetClothParam : PLC_SemiAutoParamBase
    {
        /// <summary>染杯口下探开夹脉冲 D801</summary>
        public ProtocolItem CupDownOpenPulse { get; set; }
        /// <summary>合夹夹布脉冲 D803</summary>
        public ProtocolItem CloseClampPulse { get; set; }
        /// <summary>小针筒驱动速度 D805</summary>
        public ProtocolItem SmallSyringeSpeed { get; set; }
        /// <summary>小针筒加减速时间 D807</summary>
        public ProtocolItem SmallSyringeAcc { get; set; }

        /// <summary>
        /// 取湿布参数构造函数
        /// </summary>
        /// <param name="cupDownOpenPulse">染杯口下探开夹脉冲</param>
        /// <param name="closeClampPulse">合夹夹布脉冲</param>
        /// <param name="smallSyringeSpeed">小针筒驱动速度</param>
        /// <param name="smallSyringeAcc">小针筒加减速时间</param>
        public PLC_TakeWetClothParam(
            int cupDownOpenPulse,
            int closeClampPulse,
            int smallSyringeSpeed,
            int smallSyringeAcc)
            : base(PLC.SemiAutomaticOperation.TakeWetCloth)
        {
            CupDownOpenPulse = new ProtocolItem(801, typeof(int), cupDownOpenPulse);
            CloseClampPulse = new ProtocolItem(803, typeof(int), closeClampPulse);
            SmallSyringeSpeed = new ProtocolItem(805, typeof(int), smallSyringeSpeed);
            SmallSyringeAcc = new ProtocolItem(807, typeof(int), smallSyringeAcc);
        }

        /// <summary>
        /// 取湿布参数构造函数（自动从配置获取）
        /// </summary>
        public PLC_TakeWetClothParam()
            : base(PLC.SemiAutomaticOperation.TakeWetCloth)
        {
            var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;
            CupDownOpenPulse = new ProtocolItem(801, typeof(int), My_ConPar.Other.CupDownPulse);
            CloseClampPulse = new ProtocolItem(803, typeof(int), My_ConPar.Other.ClosePulse);
            SmallSyringeSpeed = new ProtocolItem(805, typeof(int), motion?.Move_S_MinHSpeed ?? 0);
            SmallSyringeAcc = new ProtocolItem(807, typeof(int), ((motion?.Move_S_USpeed ?? 500) * 10));
        }
    }
}
