using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>注液参数</summary>
    internal class PLC_DispenseParam : PLC_SemiAutoParamBase
    {
        /// <summary>Z轴坐标 D801</summary>
        public ProtocolItem Z { get; set; }
        /// <summary>驱动速度 D803</summary>
        public ProtocolItem SyringeSpeed { get; set; }
        /// <summary>加减速 D805</summary>
        public ProtocolItem SyringeAcc { get; set; }
        
        /// <summary>注液高度 D807（0=气缸上，1=慢速中，2=慢速3位置(带盖加药)）</summary>
        public ProtocolItem DispenseHeight { get; set; }

        /// <summary>
        /// 注液参数构造函数
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="SyringeSpeed">驱动速度</param>
        /// <param name="SyringeAcc">加减速<param>
        /// <param name="dispenseHeight">注液高度（0=气缸上，1=慢速中，2=慢速3位置(带盖加药)</param>
        public PLC_DispenseParam(
            int z,int SyringeSpeed, int SyringeAcc,
             short dispenseHeight)
            : base(PLC.SemiAutomaticOperation.Dispense)
        {
            Z = new ProtocolItem(801, typeof(int), z);
            this.SyringeSpeed = new ProtocolItem(803, typeof(int), SyringeSpeed);
            this.SyringeAcc = new ProtocolItem(805, typeof(int), SyringeAcc);
            DispenseHeight = new ProtocolItem(807, typeof(short), dispenseHeight);
        }

        /// <summary>
        /// 注液参数构造函数
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param> 
        /// <param name="dispenseHeight">注液高度（0=气缸上，1=慢速中，2=慢速3位置(带盖加药)</param>
        public PLC_DispenseParam(int z, int syringeType,  short dispenseHeight)
            : base(PLC.SemiAutomaticOperation.Dispense)
        {
            var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;
            Z = new ProtocolItem(801, typeof(int), z);
            if (dispenseHeight == 0)
            {
              
                    SyringeSpeed = new ProtocolItem(803, typeof(int),syringeType == 0? motion.Move_S_HSpeed :motion.Move_B_HSpeed);
                    SyringeAcc = new ProtocolItem(805, typeof(int), syringeType == 0 ?
                        motion.Move_S_USpeed:motion.Move_B_USpeed);


            }
            else
            {
                // 带盖加药，使用慢速3位置速度
                SyringeSpeed = new ProtocolItem(803, typeof(int),syringeType == 0? 
                    motion.Move_S_MinHSpeed:motion.Move_B_MinHSpeed);
                SyringeAcc = new ProtocolItem(805, typeof(int), syringeType == 0?
                    motion.SMin_UDSpeed:motion.BMin_UDSpeed);
            }

            DispenseHeight = new ProtocolItem(807, typeof(short), dispenseHeight);
        }
    }
}
