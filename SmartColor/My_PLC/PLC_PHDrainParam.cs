using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>PH排液参数</summary>
    internal class PLC_PHDrainParam : PLC_SemiAutoParamBase
    {
        /// <summary>Z轴坐标 D801</summary>
        public ProtocolItem Z { get; set; }
        /// <summary>小针筒驱动速度 D803</summary>
        public ProtocolItem SmallSyringeSpeed { get; set; }
        /// <summary>小针筒加减速时间 D805</summary>
        public ProtocolItem SmallSyringeAcc { get; set; }
        /// <summary>大针筒驱动速度 D807</summary>
        public ProtocolItem LargeSyringeSpeed { get; set; }
        /// <summary>大针筒加减速时间 D809</summary>
        public ProtocolItem LargeSyringeAcc { get; set; }

        /// <summary>
        /// PH排液参数构造函数
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="smallSyringeSpeed">小针筒驱动速度</param>
        /// <param name="smallSyringeAcc">小针筒加减速时间</param>
        /// <param name="largeSyringeSpeed">大针筒驱动速度</param>
        /// <param name="largeSyringeAcc">大针筒加减速时间</param>
        public PLC_PHDrainParam(
            int z,
            int smallSyringeSpeed,
            int smallSyringeAcc,
            int largeSyringeSpeed,
            int largeSyringeAcc)
            : base(PLC.SemiAutomaticOperation.PHDrain)
        {
            Z = new ProtocolItem(801, typeof(int), z);
            SmallSyringeSpeed = new ProtocolItem(803, typeof(int), smallSyringeSpeed);
            SmallSyringeAcc = new ProtocolItem(805, typeof(int), smallSyringeAcc);
            LargeSyringeSpeed = new ProtocolItem(807, typeof(int), largeSyringeSpeed);
            LargeSyringeAcc = new ProtocolItem(809, typeof(int), largeSyringeAcc);
        }

        /// <summary>
        /// PH排液参数构造函数（自动从配置获取）
        /// </summary>
        /// <param name="z">Z轴坐标</param>

        public PLC_PHDrainParam(int z)
            : base(PLC.SemiAutomaticOperation.PHDrain)
        {
            var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;
            Z = new ProtocolItem(801, typeof(int), z);
            SmallSyringeSpeed = new ProtocolItem(803, typeof(int), motion?.Move_S_HSpeed ?? 0);
            SmallSyringeAcc = new ProtocolItem(805, typeof(int), (motion?.Move_S_USpeed ?? 500) * 10);
            LargeSyringeSpeed = new ProtocolItem(807, typeof(int), motion?.Move_B_HSpeed ?? 0);
            LargeSyringeAcc = new ProtocolItem(809, typeof(int), (motion?.Move_B_USpeed ?? 500) * 10);
        }
    }
}
