using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>抽液参数</summary>
    internal class PLC_AspirateParam : PLC_SemiAutoParamBase
    {
        /// <summary>Z轴坐标 D801</summary>
        public ProtocolItem Z { get; set; }
        /// <summary>驱动速度 D803</summary>
        public ProtocolItem SyringeSpeed { get; set; }
        /// <summary>加减速时间 D805</summary>
        public ProtocolItem SyringeAcc { get; set; }

        /// <summary>Z轴排空时上移脉冲 D807</summary>
        public ProtocolItem ZPurgeUpPulse { get; set; }
        /// <summary>Z轴排空时下压脉冲 D809</summary>
        public ProtocolItem ZPurgeDownPulse { get; set; }
        /// <summary>Z轴抽液完反推脉冲 D811</summary>
        public ProtocolItem ZBackPulse { get; set; }
        /// <summary>偏移量Z轴 D813</summary>
        public ProtocolItem ZOffset { get; set; }
        /// <summary>屏蔽针筒感应器 D815</summary>
        public ProtocolItem IgnoreSyringeSensor { get; set; }
        /// <summary>排空次数 D816</summary>
        public ProtocolItem PurgeCount { get; set; }
        /// <summary>针筒规格 0=小针筒 1=大针筒 D817</summary>
        public ProtocolItem SyringeType { get; set; }

        /// <summary>当前重量 D818</summary>
        public ProtocolItem CurrentWeight { get; set; }

        /// <summary>是否使用拉高抽液 D819</summary>
        public ProtocolItem UseHighLiftAspiration { get; set; }


        /// <summary>
        /// 抽液参数构造函数
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeSpeed">小针筒驱动速度</param>
        /// <param name="syringeAcc">小针筒加减速时间</param>
        /// <param name="zPurgeUpPulse">Z轴排空时上移脉冲</param>
        /// <param name="zPurgeDownPulse">Z轴排空时下压脉冲</param>
        /// <param name="zBackPulse">Z轴抽液完反推脉冲</param>
        /// <param name="zOffset">偏移量Z轴</param>
        /// <param name="ignoreSyringeSensor">屏蔽针筒感应器</param>
        /// <param name="purgeCount">排空次数</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        public PLC_AspirateParam(
            int z, int syringeSpeed, int syringeAcc,
            int zPurgeUpPulse, int zPurgeDownPulse, int zBackPulse,
            int zOffset, short ignoreSyringeSensor, short purgeCount, short syringeType,short currentWeight,short useHighLiftAspiration)
            : base(PLC.SemiAutomaticOperation.Aspirate)
        {
            Z = new ProtocolItem(801, typeof(int), z);
            this.SyringeSpeed = new ProtocolItem(803, typeof(int), syringeSpeed);
            this.SyringeAcc = new ProtocolItem(805, typeof(int), syringeAcc);
            ZPurgeUpPulse = new ProtocolItem(807, typeof(int), zPurgeUpPulse);
            ZPurgeDownPulse = new ProtocolItem(809, typeof(int), zPurgeDownPulse);
            ZBackPulse = new ProtocolItem(811, typeof(int), zBackPulse);
            ZOffset = new ProtocolItem(813, typeof(int), zOffset);
            IgnoreSyringeSensor = new ProtocolItem(815, typeof(short), ignoreSyringeSensor);
            PurgeCount = new ProtocolItem(816, typeof(short), purgeCount);
            SyringeType = new ProtocolItem(817, typeof(short), syringeType);
            CurrentWeight = new ProtocolItem(818, typeof(short), currentWeight);
            UseHighLiftAspiration = new ProtocolItem(819, typeof(short), useHighLiftAspiration);
        }

        /// <summary>
        /// 抽液参数构造函数
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="purgeCount">排空次数</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <param name="currentWeight">当前重量</param>    
        public PLC_AspirateParam(
            int z, int purgeCount, short syringeType, short currentWeight)
            : base(PLC.SemiAutomaticOperation.Aspirate)
        {
            var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;
            Z = new ProtocolItem(801, typeof(int), z);
            SyringeSpeed = new ProtocolItem(803, typeof(int), syringeType == 0 ? motion.Move_S_HSpeed : motion.Move_B_HSpeed);
            SyringeAcc = new ProtocolItem(805, typeof(int), syringeType == 0 ?
                motion.Move_S_USpeed : motion.Move_B_USpeed);
            ZPurgeUpPulse = new ProtocolItem(807, typeof(int), My_ConPar.Other.Z_UpPulse);
            ZPurgeDownPulse = new ProtocolItem(809, typeof(int), My_ConPar.Other.Z_DownPulse);
            ZBackPulse = new ProtocolItem(811, typeof(int), My_ConPar.Other.Z_BackPulse);
            ZOffset = new ProtocolItem(813, typeof(int), motion?.Home_Z_Offset ?? 0);
            IgnoreSyringeSensor = new ProtocolItem(815, typeof(short), My_ConPar.Choices.IgnoreSyringeSensor);
            PurgeCount = new ProtocolItem(816, typeof(short), purgeCount);
            SyringeType = new ProtocolItem(817, typeof(short), syringeType);
            CurrentWeight = new ProtocolItem(818, typeof(short), currentWeight);
            UseHighLiftAspiration = new ProtocolItem(819, typeof(short), My_ConPar.Choices.UseHighLiftAspiration);

        }
    }
}
