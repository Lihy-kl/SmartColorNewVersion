using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>取干布参数</summary>
    internal class PLC_TakeDryClothParam : PLC_SemiAutoParamBase
    {
        /// <summary>备布区下探开夹脉冲 D801</summary>
        public ProtocolItem PrepareAreaDownOpenPulse { get; set; }
        /// <summary>合夹夹布脉冲 D803</summary>
        public ProtocolItem CloseClampPulse { get; set; }
        /// <summary>小针筒驱动速度 D805</summary>
        public ProtocolItem SmallSyringeSpeed { get; set; }
        /// <summary>小针筒加减速时间 D807</summary>
        public ProtocolItem SmallSyringeAcc { get; set; }
        /// <summary>备布区取布气缸位置 D809（0=慢速中取布、1=慢速中2取布）</summary>
        public ProtocolItem TakeClothCylinderPos { get; set; }
        /// <summary>超20g布布框下探补偿脉冲 D810（备布框类型）</summary>
        public ProtocolItem Over20gCompensatePulse { get; set; }

        /// <summary>
        /// 取干布参数构造函数
        /// </summary>
        /// <param name="prepareAreaDownOpenPulse">备布区下探开夹脉冲</param>
        /// <param name="closeClampPulse">合夹夹布脉冲</param>
        /// <param name="smallSyringeSpeed">小针筒驱动速度</param>
        /// <param name="smallSyringeAcc">小针筒加减速时间</param>
        /// <param name="takeClothCylinderPos">备布区取布气缸位置（0=慢速中取布、1=慢速中2取布）</param>
        /// <param name="over20gCompensatePulse">超20g布布框下探补偿脉冲 （备布框类型）</param>
        public PLC_TakeDryClothParam(
            int prepareAreaDownOpenPulse,
            int closeClampPulse,
            int smallSyringeSpeed,
            int smallSyringeAcc,
            short takeClothCylinderPos,
            short over20gCompensatePulse)
            : base(PLC.SemiAutomaticOperation.TakeDryCloth)
        {
            PrepareAreaDownOpenPulse = new ProtocolItem(801, typeof(int), prepareAreaDownOpenPulse);
            CloseClampPulse = new ProtocolItem(803, typeof(int), closeClampPulse);
            SmallSyringeSpeed = new ProtocolItem(805, typeof(int), smallSyringeSpeed);
            SmallSyringeAcc = new ProtocolItem(807, typeof(int), smallSyringeAcc);
            TakeClothCylinderPos = new ProtocolItem(809, typeof(short), takeClothCylinderPos);
            Over20gCompensatePulse = new ProtocolItem(810, typeof(short), over20gCompensatePulse);
        }

        /// <summary>
        /// 取干布参数构造函数
        /// </summary>
        /// <param name="takeClothCylinderPos">备布区取布气缸位置（0=慢速中取布、1=慢速中2取布）</param>
        /// <param name="over20gCompensatePulse">超20g布布框下探补偿脉冲 （备布框类型）</param>
        public PLC_TakeDryClothParam(
            short takeClothCylinderPos,
            short over20gCompensatePulse)
            : base(PLC.SemiAutomaticOperation.TakeDryCloth)
        {
            var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;
            PrepareAreaDownOpenPulse = new ProtocolItem(801, typeof(int), My_ConPar.Other.ClothDownPulse);
            CloseClampPulse = new ProtocolItem(803, typeof(int), My_ConPar.Other.ClosePulse);
            SmallSyringeSpeed = new ProtocolItem(805, typeof(int), motion.Move_S_MinHSpeed);
            SmallSyringeAcc = new ProtocolItem(807, typeof(int), ((motion?.Move_S_USpeed ?? 500) * 10));
            TakeClothCylinderPos = new ProtocolItem(809, typeof(short), takeClothCylinderPos);
            Over20gCompensatePulse = new ProtocolItem(810, typeof(short), over20gCompensatePulse);
        }
    }

}
