using com.google.zxing;
using SmartColor.My_SemiAutoModule;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>
    /// 封装所有半自动操作的静态助手类，便于直接调用（所有方法均为异步方法）
    /// </summary>
    internal static class PLC_SemiAutoHelper
    {
        /// <summary>
        /// 回原点（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> HomeAsync()
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_HomeParam());
            return RobotAlarmOverview.HandleSemiAutoResult(result,  "回原点");
        }

        /// <summary>
        /// 定点移动（异步方法）
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="y">Y轴坐标</param>
        /// <param name="needHome">50g布夹是否需要回原点</param>
        /// <param name="isStandby">是否为待机位</param>
        /// <returns>动作完成/异常码</returns>
        public static Task<int> MoveToPositionAsync(int x, int y, short needHome, short isStandby, short isTrayIn, int allowMove)
        {
            if (isStandby == 1)
            {
                SemiAutoHelperFactory.IsStandby = true;
            }
            else
            {
                SemiAutoHelperFactory.IsStandby = false;
            }

            return My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_MoveToPositionParam(x, y, needHome, isStandby, isTrayIn,allowMove));
        }


        /// <summary>
        /// 抽液（异步方法）
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="purgeCount">排空次数</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> AspirateAsync(int z, int purgeCount, short syringeType, short currentWeight)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_AspirateParam(z, purgeCount, syringeType, currentWeight));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "抽液");
        }

        /// <summary>
        /// 注液（异步方法）
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <param name="dispenseHeight">注液高度（0=气缸上，1=慢速中，2=慢速3位置(带盖加药)</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> DispenseAsync(int z, int syringeType, short dispenseHeight)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_DispenseParam(z, syringeType, dispenseHeight));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "注液");
        }

        /// <summary>
        /// 加水（异步方法）
        /// </summary>
        /// <param name="objectWeight">目标重量</param>
        /// <param name="waterTime">加水时间/s</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> AddWaterAsync(double objectWeight, double waterTime)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_AddWaterParam(objectWeight, waterTime));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "加水");
        }

        /// <summary>
        /// 放针（异步方法）
        /// </summary>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> ReleaseNeedleAsync(int syringeType)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_ReleaseNeedleParam(syringeType));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "放针");
        }

        /// <summary>
        /// 泄压（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> DecompressAsync()
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_DecompressParam());
            return RobotAlarmOverview.HandleSemiAutoResult(result, "泄压");
        }

        /// <summary>
        /// 相对移动（异步方法）
        /// </summary>
        /// <param name="axis">轴号 0：X 1:Y 2:Z 3:转盘</param>
        /// <param name="pulse">坐标</param>
        /// <param name="hSpeed">速度</param>
        /// <param name="upSpeed">加减速</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> RelativeMoveAsync(short axis, int pulse, int hSpeed, int upSpeed)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_RelativeMoveParam(axis, pulse, hSpeed, upSpeed));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "相对移动");
        }

        /// <summary>
        /// 复位（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> ResetAsync()
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_ResetParam());
            return RobotAlarmOverview.HandleSemiAutoResult(result, "复位");
        }

        /// <summary>
        /// 动作检查（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> ActionCheckAsync()
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_ActionCheckParam());
            return RobotAlarmOverview.HandleSemiAutoResult(result, "动作检查");
        }

        /// <summary>
        /// 开盖（异步方法）
        /// </summary>
        /// <param name="lockSignal">锁止信号</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> OpenLidAsync(short lockSignal)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_OpenLidParam(lockSignal));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "开盖");
        }

        /// <summary>
        /// 放盖（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PutLidAsync()
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_PutLidParam());
            return RobotAlarmOverview.HandleSemiAutoResult(result, "放盖");
        }

        /// <summary>
        /// 取盖（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> TakeLidAsync()
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_TakeLidParam());
            return RobotAlarmOverview.HandleSemiAutoResult(result, "取盖");
        }

        /// <summary>
        /// 关盖（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> CloseLidAsync()
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_CloseLidParam());
            return RobotAlarmOverview.HandleSemiAutoResult(result, "关盖");
        }

        /// <summary>
        /// 天平检查（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> BalanceCheckAsync()
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_BalanceCheckParam());
            return RobotAlarmOverview.HandleSemiAutoResult(result, "天平检查");
        }

        /// <summary>
        /// 机械手抓取（异步方法）
        /// </summary>
        /// <param name="graspType">抓取类型（0=布夹 1=UV针筒 2=洗针针筒 3=PH针筒 4=母液瓶夹子 5=粉罐）</param>
        /// <param name="syringeType">针筒规格</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> RobotHandGraspingAsync(short graspType, short syringeType)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_RobotHandGraspingParam(graspType, syringeType));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "机械手抓取");
        }

        /// <summary>
        /// 机械手松放（异步方法）
        /// </summary>
        /// <param name="graspType">抓取类型（0=布夹 1=UV针筒 2=洗针针筒 3=PH针筒 4=母液瓶夹子 5=粉罐）</param>
        /// <param name="syringeType">针筒规格</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> RobotArmReleaseAsync(short graspType, short syringeType)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_RobotArmReleaseParam(graspType, syringeType));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "机械手松放");
        }

        /// <summary>
        /// 取干布（异步方法）
        /// </summary>
        /// <param name="takeClothCylinderPos">备布区取布气缸位置（0=慢速中取布、1=慢速中2取布）</param>
        /// <param name="over20gCompensatePulse">超20g布布框下探补偿脉冲（备布框类型）</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> TakeDryClothAsync(short takeClothCylinderPos, short over20gCompensatePulse)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_TakeDryClothParam(takeClothCylinderPos, over20gCompensatePulse));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "取干布");
        }

        /// <summary>
        /// 取湿布（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> TakeWetClothAsync()
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_TakeWetClothParam());
            return RobotAlarmOverview.HandleSemiAutoResult(result, "取湿布");
        }

        /// <summary>
        /// 放干布（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PutDryClothAsync()
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_PutDryClothParam());
            return RobotAlarmOverview.HandleSemiAutoResult(result, "放干布");
        }

        /// <summary>
        /// 放湿布（异步方法）
        /// </summary>
        /// <param name="outCylinderPos">放布时气缸位置</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PutWetClothAsync(short outCylinderPos)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_PutWetClothParam(outCylinderPos));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "放湿布");
        }

        /// <summary>
        /// UV抽液（异步方法）
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="purgeCount">排空次数</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> UVAspirateAsync(int z, short syringeType)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_UVAspirateParam(z, syringeType));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "UV抽液");
        }

        /// <summary>
        /// 洗针筒（异步方法）
        /// </summary>
        /// <param name="syringeType">针筒类型</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> WashSyringeAsync(short syringeType)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_WashSyringeParam(syringeType));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "洗针筒");
        }

        /// <summary>
        /// 加溶解剂（异步方法）
        /// </summary>
        /// <param name="solventTime">加溶解剂时间/s</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> AddSolventAsync(double solventTime)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_AddSolventParam(solventTime));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "加溶解剂");
        }

        /// <summary>
        /// 撑盖（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> SupportCoverAsync()
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_SupportCoverParam());
            return RobotAlarmOverview.HandleSemiAutoResult(result, "撑盖");
        }

        /// <summary>
        /// PH抽液（异步方法）
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PHAspirateAsync(int z, short syringeType)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_PHAspirateParam(z, syringeType));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "PH抽液");
        }

        /// <summary>
        /// PH排液（异步方法）
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PHDrainAsync(int z)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_PHDrainParam(z));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "PH排液");
        }

        /// <summary>
        /// 取母液瓶（异步方法）
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="takeHeight">取瓶高度（0=气缸下、1=气缸到阻挡位）</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> TakeMotherBottleAsync(int z, short takeHeight)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_TakeMotherBottleParam(z, takeHeight));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "取母液瓶");
        }

        /// <summary>
        /// 放母液瓶（异步方法）
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="takeHeight">取瓶高度（0=气缸下、1=气缸到阻挡位）</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PutMotherBottleAsync(int z, short takeHeight)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_PutMotherBottleParam(z, takeHeight));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "放母液瓶");
        }

        /// <summary>
        /// 加粉（异步方法）
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> AddPowderAsync(int z)
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_AddPowderParam(z));
            return RobotAlarmOverview.HandleSemiAutoResult(result, "加粉");
        }

        /// <summary>
        /// 加粉前气缸到阻挡位（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> BeforeAddPowderBlockAsync()
        {
            var result = await My_ConPar.Object.CurrentPLC.ExecuteSemiAutomaticOperationAsync(new PLC_BeforeAddPowderBlockParam());
            return RobotAlarmOverview.HandleSemiAutoResult(result, "加粉前气缸到阻挡位");
        }
    }
}