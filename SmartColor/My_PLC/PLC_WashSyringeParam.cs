using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>洗针筒参数</summary>
    internal class PLC_WashSyringeParam : PLC_SemiAutoParamBase
    {
        /// <summary>偏移量Z轴 D801</summary>
        public ProtocolItem ZOffset { get; set; }
        /// <summary>针筒驱动速度 D803</summary>
        public ProtocolItem SyringeSpeed { get; set; }
        /// <summary>针筒加减速时间 D805</summary>
        public ProtocolItem SyringeAcc { get; set; }
      
        /// <summary>针筒规格 D807（0=小针筒 1=大针筒）</summary>
        public ProtocolItem SyringeType { get; set; }
        /// <summary>洗针次数 D808</summary>
        public ProtocolItem WashCount { get; set; }

        /// <summary>
        /// 洗针筒参数构造函数
        /// </summary>
        public PLC_WashSyringeParam(
            short zOffset,
            short smallSyringeSpeed,
            short smallSyringeAcc,
            short syringeType,
            short washCount)
            : base(PLC.SemiAutomaticOperation.WashSyringe)
        {
            ZOffset = new ProtocolItem(801, typeof(short), zOffset);
            SyringeSpeed = new ProtocolItem(803, typeof(short), smallSyringeSpeed);
            SyringeAcc = new ProtocolItem(805, typeof(short), smallSyringeAcc);
         
            SyringeType = new ProtocolItem(807, typeof(short), syringeType);
            WashCount = new ProtocolItem(808, typeof(short), washCount);
        }

        /// <summary>
        /// 洗针筒参数构造函数（自动从配置获取）
        /// </summary>
        /// <param name="syringeType">针筒类型</param>
        public PLC_WashSyringeParam(short syringeType)
            : base(PLC.SemiAutomaticOperation.WashSyringe)
        {
            var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;
            ZOffset = new ProtocolItem(801, typeof(short), (short)(motion?.Home_Z_Offset ?? 0));
            SyringeSpeed = new ProtocolItem(803, typeof(short), (short)(syringeType ==0? motion?.Move_S_HSpeed ?? 10000: motion?.Move_B_HSpeed ?? 5000));
            SyringeAcc = new ProtocolItem(805, typeof(short), (short)(syringeType == 0 ? ((motion?.Move_S_USpeed ?? 500) * 10):((motion?.Move_B_USpeed ?? 500) * 10)));
            SyringeType = new ProtocolItem(807, typeof(short), syringeType);
            WashCount = new ProtocolItem(808, typeof(short), (short)My_ConPar.Other.WashTime);
        }
    }
}
