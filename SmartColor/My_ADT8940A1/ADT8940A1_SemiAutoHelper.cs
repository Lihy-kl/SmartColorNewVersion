using SmartColor.My_PLC;
using SmartColor.My_SemiAutoModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ADT8940A1
{
    internal class ADT8940A1_SemiAutoHelper
    {
        /// <summary>
        /// 回原点
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async  Task<SemiAutoResult> HomeAsync() =>  throw new NotImplementedException();

        /// <summary>
        /// 定点移动
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="y">Y轴坐标</param>
        /// <param name="needHome">50g布夹是否需要回原点</param>
        /// <param name="isStandby">是否为待机位</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<int> MoveToPositionAsync(int x, int y, short needHome, short isStandby,short isTrayIn=0)
            =>  throw new NotImplementedException();

        /// <summary>
        /// 抽液
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="purgeCount">排空次数</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> AspirateAsync(int z, int purgeCount, short syringeType)
            => throw new NotImplementedException();

        /// <summary>
        /// 注液
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <param name="dispenseHeight">注液高度（0=气缸上，1=慢速中，2=慢速3位置(带盖加药)</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> DispenseAsync(int z, int syringeType, short dispenseHeight)
            => throw new NotImplementedException();

        /// <summary>
        /// 加水
        /// </summary>
        /// <param name="waterTime">加水时间/s</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> AddWaterAsync(double waterTime)
            => throw new NotImplementedException();

        /// <summary>
        /// 放针
        /// </summary>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> ReleaseNeedleAsync(int syringeType)
            =>  throw new NotImplementedException();

        /// <summary>
        /// 泄压
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> DecompressAsync()
            => throw new NotImplementedException();

        /// <summary>
        /// 相对移动
        /// </summary>
        /// <param name="axis">轴号  0：X 1:Y 2:Z 3:转盘</param>
        /// <param name="pulse">坐标</param>
        /// <param name="hSpeed">速度</param>
        /// <param name="upSpeed">加减速</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> RelativeMoveAsync(short axis, int pulse, int hSpeed, int upSpeed)
            => throw new NotImplementedException();

        /// <summary>
        /// 复位
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> ResetAsync()
            => throw new NotImplementedException();

        /// <summary>
        /// 动作检查
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> ActionCheckAsync()
            => throw new NotImplementedException();

        /// <summary>
        /// 开盖
        /// </summary>
        /// <param name="lockSignal">锁止信号</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> OpenLidAsync(short lockSignal)
            => throw new NotImplementedException();

        /// <summary>
        /// 放盖
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PutLidAsync()
            => throw new NotImplementedException();

        /// <summary>
        /// 取盖
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> TakeLidAsync()
            => throw new NotImplementedException();

        /// <summary>
        /// 关盖
        /// </summary>
        /// <param name="lockSignal">锁止信号</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> CloseLidAsync(short lockSignal)
            => throw new NotImplementedException();

        /// <summary>
        /// 天平检查
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> BalanceCheckAsync()
            => throw new NotImplementedException();

        /// <summary>
        /// 机械手抓取
        /// </summary>
        /// <param name="graspType">抓取类型（0=布夹 1=UV针筒 2=洗针针筒 3=PH针筒 4=母液瓶夹子 5=粉罐）</param>
        /// <param name="syringeType">针筒规格</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> RobotHandGraspingAsync(short graspType, short syringeType)
            => throw new NotImplementedException();

        /// <summary>
        /// 机械手松放
        /// </summary>
        /// <param name="graspType">抓取类型（0=布夹 1=UV针筒 2=洗针针筒 3=PH针筒 4=母液瓶夹子 5=粉罐）</param>
        /// <param name="syringeType">针筒规格</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> RobotArmReleaseAsync(short graspType, short syringeType)
            => throw new NotImplementedException();

        /// <summary>
        /// 取干布
        /// </summary>
        /// <param name="takeClothCylinderPos">备布区取布气缸位置（0=慢速中取布、1=慢速中2取布）</param>
        /// <param name="over20gCompensatePulse">超20g布布框下探补偿脉冲 （备布框类型）</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> TakeDryClothAsync(short takeClothCylinderPos,
            short over20gCompensatePulse)
            => throw new NotImplementedException();

        /// <summary>
        /// 取湿布
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> TakeWetClothAsync()
            => throw new NotImplementedException();

        /// <summary>
        /// 放干布
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PutDryClothAsync()
            => throw new NotImplementedException();

        /// <summary>
        /// 放湿布
        /// <param name="outCylinderPos">放布时气缸位置</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PutWetClothAsync(short outCylinderPos)
            =>   throw new NotImplementedException();

        /// <summary>
        /// UV抽液
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> UVAspirateAsync(int z,  short syringeType)
            => throw new NotImplementedException();

        /// <summary>
        /// 洗针筒
        /// </summary>
        /// <param name="syringeType">针筒类型</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> WashSyringeAsync(short syringeType)
            => throw new NotImplementedException();

        /// <summary>
        /// 加溶解剂
        /// </summary>
        /// <param name="solventTime">加溶解剂时间/s</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> AddSolventAsync(double solventTime)
            => throw new NotImplementedException();

        /// <summary>
        /// 撑盖
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> SupportCoverAsync()
            =>    throw new NotImplementedException();

        /// <summary>
        /// PH抽液参数构造函数
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PHAspirateAsync(int z, short syringeType)
            => throw new NotImplementedException();

        /// <summary>
        /// PH排液参数构造函数（自动从配置获取）
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PHDrainAsync(int z)
            => throw new NotImplementedException();

        /// <summary>
        /// 取母液瓶
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="takeHeight">取瓶高度（0=气缸下、1=气缸到阻挡位）</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> TakeMotherBottleAsync(int z, short takeHeight)
            => throw new NotImplementedException();

        /// <summary>
        /// 放母液瓶
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="takeHeight">取瓶高度（0=气缸下、1=气缸到阻挡位）</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PutMotherBottleAsync(int z, short takeHeight)
            => throw new NotImplementedException();

        /// <summary>
        /// 加粉
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> AddPowderAsync(int z)
            => throw new NotImplementedException();

        /// <summary>
        /// 加粉前气缸到阻挡位
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> BeforeAddPowderBlockAsync()
              => throw new NotImplementedException();
    }
}
