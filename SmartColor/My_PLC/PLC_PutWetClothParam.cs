using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>放湿布参数</summary>
    internal class PLC_PutWetClothParam : PLC_SemiAutoParamBase
    {
        /// <summary>开夹放布脉冲 D801</summary>
        public ProtocolItem OpenClampPulse { get; set; }
        /// <summary>小针筒驱动速度 D803</summary>
        public ProtocolItem SmallSyringeSpeed { get; set; }
        /// <summary>小针筒加减速时间 D805</summary>
        public ProtocolItem SmallSyringeAcc { get; set; }
        /// <summary>放布时气缸位置 D807（0=气缸到阻挡位 1=气缸慢速中）</summary>
        public ProtocolItem OutCylinderPos { get; set; }

        /// <summary>
        /// 放湿布参数构造函数
        /// </summary>
        /// <param name="openClampPulse">开夹放布脉冲</param>
        /// <param name="smallSyringeSpeed">小针筒驱动速度</param>
        /// <param name="smallSyringeAcc">小针筒加减速时间</param>
        /// <param name="outCylinderPos">放布时气缸位置</param>
        public PLC_PutWetClothParam(
            int openClampPulse,
            int smallSyringeSpeed,
            int smallSyringeAcc,
            short outCylinderPos)
            : base(PLC.SemiAutomaticOperation.PutWetCloth)
        {
            OpenClampPulse = new ProtocolItem(801, typeof(int), openClampPulse);
            SmallSyringeSpeed = new ProtocolItem(803, typeof(int), smallSyringeSpeed);
            SmallSyringeAcc = new ProtocolItem(805, typeof(int), smallSyringeAcc);
            OutCylinderPos = new ProtocolItem(807, typeof(short), outCylinderPos);
        }

        /// <summary>
        /// 放湿布参数构造函数（自动从配置获取）
        /// <param name="outCylinderPos">放布时气缸位置</param>
        /// </summary>
        public PLC_PutWetClothParam(short outCylinderPos)
            : base(PLC.SemiAutomaticOperation.PutWetCloth)
        {
            var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;
            OpenClampPulse = new ProtocolItem(801, typeof(int), My_ConPar.Other.OpenPulse);
            SmallSyringeSpeed = new ProtocolItem(803, typeof(int), motion?.Move_S_MinHSpeed ?? 0);
            SmallSyringeAcc = new ProtocolItem(805, typeof(int), ((motion?.Move_S_USpeed ?? 500) * 10));
            OutCylinderPos = new ProtocolItem(807, typeof(short), outCylinderPos);
        }
    }
}
