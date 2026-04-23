using SmartColor.My_PLC;
using SmartColor.My_SemiAutoModule;
using SmartColor.My_Tool;
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
        public static async  Task<SemiAutoResult> HomeAsync()
        {
            SmartColor.My_ADT8940A1.ADT8940A1_Home aDT8940A1_Home = new ADT8940A1_Home();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("回原点");
            var result = await aDT8940A1_Home.Home_XYZ(1);
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result==2?"待机":"异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "回原点");
        }

        /// <summary>
        /// 定点移动
        /// </summary>
        /// <param name="x">X轴坐标</param>
        /// <param name="y">Y轴坐标</param>
        /// <param name="needHome">50g布夹是否需要回原点</param>
        /// <param name="isStandby">是否为待机位</param>
        /// <returns>动作完成/异常码</returns>
        public static Task<int> MoveToPositionAsync(int x, int y, short needHome, short isStandby,short isTrayIn=0)
        {
            //if (!Lib_Card.ADT8940A1.Module.Home.Home.Home_XYZFinish)
            //{
            //    var result = HomeAsync();
            //}
            if (isStandby == 1)
            {
                SemiAutoHelperFactory.IsStandby = true;
            }
            else
            {
                SemiAutoHelperFactory.IsStandby = false;
            }

            SmartColor.My_ADT8940A1.ADT8940A1_MoveToPosition move = new ADT8940A1_MoveToPosition();
            return move.MoveToPosition(1,x, y, isStandby);
        }

        /// <summary>
        /// 抽液
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="purgeCount">排空次数</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> AspirateAsync(int z, int purgeCount, short syringeType, short currentWeight)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_Aspirate aDT8940A1_Aspirate = new SmartColor.My_ADT8940A1.ADT8940A1_Aspirate();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("抽液");
            var result = await aDT8940A1_Aspirate.Aspirate(z, purgeCount,syringeType);
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "抽液");
        }

        /// <summary>
        /// 注液
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <param name="dispenseHeight">注液高度（0=气缸上，1=慢速中，2=慢速3位置(带盖加药)</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> DispenseAsync(int z, int syringeType, short dispenseHeight)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_Dispense aDT8940A1_Dispense = new SmartColor.My_ADT8940A1.ADT8940A1_Dispense();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("注液");
            var result = await aDT8940A1_Dispense.DispenseAsync(z,  syringeType, dispenseHeight);
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "注液");
        }

        /// <summary>
        /// 加水
        /// </summary>
        /// <param name="waterTime">加水时间/s</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> AddWaterAsync(double waterTime)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_AddWater aDT8940A1_AddWater = new SmartColor.My_ADT8940A1.ADT8940A1_AddWater();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("加水");
            var result = await aDT8940A1_AddWater.AddWaterAsync(waterTime);
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "加水");
        }

        /// <summary>
        /// 放针
        /// </summary>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> ReleaseNeedleAsync(int syringeType)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_ReleaseNeedle aDT8940A1_ReleaseNeedle = new SmartColor.My_ADT8940A1.ADT8940A1_ReleaseNeedle();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("放针");
            var result = await aDT8940A1_ReleaseNeedle.ReleaseNeedle(syringeType);
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "放针");
        }

        /// <summary>
        /// 泄压
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> DecompressAsync()
        {
            SmartColor.My_ADT8940A1.ADT8940A1_Decompress aDT8940A1_Decompress = new SmartColor.My_ADT8940A1.ADT8940A1_Decompress();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("泄压");
            var result = await aDT8940A1_Decompress.Decompress();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "泄压");
        }

        /// <summary>
        /// 相对移动
        /// </summary>
        /// <param name="axis">轴号  0：X 1:Y 2:Z 3:转盘</param>
        /// <param name="pulse">坐标</param>
        /// <param name="hSpeed">速度</param>
        /// <param name="upSpeed">加减速</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> RelativeMoveAsync(short axis, int pulse, int hSpeed, int upSpeed)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_RelativeMove aDT8940A1_RelativeMove = new SmartColor.My_ADT8940A1.ADT8940A1_RelativeMove();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("相对移动");
            var result = await aDT8940A1_RelativeMove.RelativeMoveAsync(axis,pulse,hSpeed,upSpeed);
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "相对移动");
        }

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
        {
            SmartColor.My_ADT8940A1.ADT8940A1_ActionCheck aDT8940A1_ActionCheck = new SmartColor.My_ADT8940A1.ADT8940A1_ActionCheck();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("动作检查");
            var result = await aDT8940A1_ActionCheck.ActionCheckAsync();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "动作检查");
        }

        /// <summary>
        /// 开盖
        /// </summary>
        /// <param name="lockSignal">锁止信号</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> OpenLidAsync(short lockSignal)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_OpenLid aDT8940A1_OpenLid = new SmartColor.My_ADT8940A1.ADT8940A1_OpenLid();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("开盖");
            var result = await aDT8940A1_OpenLid.OpenLidAsync();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "开盖");
        }

        /// <summary>
        /// 放盖
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PutLidAsync()
        {
            SmartColor.My_ADT8940A1.ADT8940A1_PutLid aDT8940A1_PutLid = new SmartColor.My_ADT8940A1.ADT8940A1_PutLid();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("放盖");
            var result = await aDT8940A1_PutLid.PutLidLidAsync();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "放盖");
        }

        /// <summary>
        /// 取盖
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> TakeLidAsync()
        {
            SmartColor.My_ADT8940A1.ADT8940A1_TakeLid aDT8940A1_TakeLid = new SmartColor.My_ADT8940A1.ADT8940A1_TakeLid();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("取盖");
            var result = await aDT8940A1_TakeLid.TakeLidAsync();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "取盖");
        }

        /// <summary>
        /// 关盖
        /// </summary>
        /// <param name="lockSignal">锁止信号</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> CloseLidAsync(short lockSignal)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_CloseLid aDT8940A1_CloseLid = new SmartColor.My_ADT8940A1.ADT8940A1_CloseLid();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("关盖");
            var result = await aDT8940A1_CloseLid.CloseLidLidAsync();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "关盖");
        }

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
        {
            SmartColor.My_ADT8940A1.ADT8940A1_RobotHandGrasping aDT8940A1_RobotHandGrasping = new SmartColor.My_ADT8940A1.ADT8940A1_RobotHandGrasping();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("机械手抓取");
            var result = await aDT8940A1_RobotHandGrasping.RobotHandGraspingAsync(graspType,syringeType);
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "机械手抓取");
        }

        /// <summary>
        /// 机械手松放
        /// </summary>
        /// <param name="graspType">抓取类型（0=布夹 1=UV针筒 2=洗针针筒 3=PH针筒 4=母液瓶夹子 5=粉罐）</param>
        /// <param name="syringeType">针筒规格</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> RobotArmReleaseAsync(short graspType, short syringeType)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_RobotArmRelease aDT8940A1_RobotArmRelease = new SmartColor.My_ADT8940A1.ADT8940A1_RobotArmRelease();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("机械手松放");
            var result = await aDT8940A1_RobotArmRelease.RobotArmReleaseAsync(graspType, syringeType);
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "机械手松放");
        }

        /// <summary>
        /// 取干布
        /// </summary>
        /// <param name="takeClothCylinderPos">备布区取布气缸位置（0=慢速中取布、1=慢速中2取布）</param>
        /// <param name="over20gCompensatePulse">超20g布布框下探补偿脉冲 （备布框类型）</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> TakeDryClothAsync(short takeClothCylinderPos,
            short over20gCompensatePulse)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_TakeDryCloth aDT8940A1_TakeDryCloth = new SmartColor.My_ADT8940A1.ADT8940A1_TakeDryCloth();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("取干布");
            var result = await aDT8940A1_TakeDryCloth.TakeDryClothAsync(takeClothCylinderPos, over20gCompensatePulse);
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "取干布");
        }

        /// <summary>
        /// 取湿布
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> TakeWetClothAsync()
        {
            SmartColor.My_ADT8940A1.ADT8940A1_TakeWetCloth aDT8940A1_TakeWetCloth = new SmartColor.My_ADT8940A1.ADT8940A1_TakeWetCloth();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("取湿布");
            var result = await aDT8940A1_TakeWetCloth.TakeWetClothAsync();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "取湿布");
        }

        /// <summary>
        /// 放干布
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PutDryClothAsync()
        {
            SmartColor.My_ADT8940A1.ADT8940A1_PutDryCloth aDT8940A1_PutDryCloth = new SmartColor.My_ADT8940A1.ADT8940A1_PutDryCloth();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("放干布");
            var result = await aDT8940A1_PutDryCloth.PutDryClothAsync();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "放干布");
        }

        /// <summary>
        /// 放湿布
        /// <param name="outCylinderPos">放布时气缸位置</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PutWetClothAsync(short outCylinderPos)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_PutWetCloth aDT8940A1_PutWetCloth = new SmartColor.My_ADT8940A1.ADT8940A1_PutWetCloth();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("放湿布");
            var result = await aDT8940A1_PutWetCloth.PutWetClothAsync();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "放湿布");
        }

        /// <summary>
        /// UV抽液
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> UVAspirateAsync(int z,  short syringeType)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_UVAspirate aDT8940A1_UVAspirate = new SmartColor.My_ADT8940A1.ADT8940A1_UVAspirate();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("UV抽液");
            var result = await aDT8940A1_UVAspirate.UVAspirateAsync(z,syringeType);
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "UV抽液");
        }

        /// <summary>
        /// 洗针筒
        /// </summary>
        /// <param name="syringeType">针筒类型</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> WashSyringeAsync(short syringeType)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_WashSyringe aDT8940A1_WashSyringe = new SmartColor.My_ADT8940A1.ADT8940A1_WashSyringe();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("洗针筒");
            var result = await aDT8940A1_WashSyringe.WashSyringeAsync(syringeType);
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "洗针筒");
        }

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
        {
            SmartColor.My_ADT8940A1.ADT8940A1_SupportCover aDT8940A1_SupportCover = new SmartColor.My_ADT8940A1.ADT8940A1_SupportCover();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("撑盖");
            var result = await aDT8940A1_SupportCover.SupportCoverAsync();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "撑盖");
        }

        /// <summary>
        /// PH抽液参数构造函数
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PHAspirateAsync(int z, short syringeType)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_PHAspirate aDT8940A1_PHAspirate = new SmartColor.My_ADT8940A1.ADT8940A1_PHAspirate();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("PH抽液");
            var result = await aDT8940A1_PHAspirate.PHAspirateAsync(z,syringeType);
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "PH抽液");
        }

        /// <summary>
        /// PH排液参数构造函数（自动从配置获取）
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <returns>动作完成/异常码</returns>
        public static async Task<SemiAutoResult> PHDrainAsync(int z)
        {
            SmartColor.My_ADT8940A1.ADT8940A1_PHDrain aDT8940A1_PHDrain = new SmartColor.My_ADT8940A1.ADT8940A1_PHDrain();
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync("PH排液");
            short syringeType = 1;
            var result = await aDT8940A1_PHDrain.PHDrainAsync(z, syringeType);
            await My_ConPar.Object.CurrentADT8940A1.UpdateStatusAsync(result == 2 ? "待机" : "异常");
            return RobotAlarmOverview.HandleSemiAutoResult(result, "PH排液");
        }

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
