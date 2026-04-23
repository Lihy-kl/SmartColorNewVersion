using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>放母液瓶参数</summary>
    internal class PLC_PutMotherBottleParam : PLC_SemiAutoParamBase
    {
        /// <summary>Z轴坐标 D801</summary>
        public ProtocolItem Z { get; set; }
        /// <summary>小针筒驱动速度 D803</summary>
        public ProtocolItem SmallSyringeSpeed { get; set; }
        /// <summary>小针筒加减速时间 D805</summary>
        public ProtocolItem SmallSyringeAcc { get; set; }
        /// <summary>取瓶高度 D807（0=气缸下、1=气缸到阻挡位）</summary>
        public ProtocolItem TakeHeight { get; set; }

        /// <summary>
        /// 放母液瓶参数构造函数
        /// </summary>
        public PLC_PutMotherBottleParam(
            int z,
            int smallSyringeSpeed,
            int smallSyringeAcc,
            short takeHeight)
            : base(PLC.SemiAutomaticOperation.PutMotherBottle)
        {
            Z = new ProtocolItem(801, typeof(int), z);
            SmallSyringeSpeed = new ProtocolItem(803, typeof(int), smallSyringeSpeed);
            SmallSyringeAcc = new ProtocolItem(805, typeof(int), smallSyringeAcc);
            TakeHeight = new ProtocolItem(807, typeof(short), takeHeight);
        }

        /// <summary>
        /// 放母液瓶参数构造函数
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="takeHeight">取瓶高度（0=气缸下、1=气缸到阻挡位）</param>
        public PLC_PutMotherBottleParam(int z, short takeHeight)
            : base(PLC.SemiAutomaticOperation.PutMotherBottle)
        {
            var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;
            Z = new ProtocolItem(801, typeof(int), z);
            SmallSyringeSpeed = new ProtocolItem(803, typeof(int), motion?.Move_S_HSpeed ?? 0);
            SmallSyringeAcc = new ProtocolItem(805, typeof(int), (motion?.Move_S_USpeed ?? 500) * 10);
            TakeHeight = new ProtocolItem(807, typeof(short), takeHeight);
        }
    }
}
