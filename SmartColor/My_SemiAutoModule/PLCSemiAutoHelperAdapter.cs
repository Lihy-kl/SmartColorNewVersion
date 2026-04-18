using SmartColor.My_PLC;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartColor.My_SemiAutoModule
{
    public class PLCSemiAutoHelperAdapter : ISemiAutoHelper
    {
        private async Task<SemiAutoResult> LogAndRun(string actionName, Func<Task<SemiAutoResult>> action)
        {
            // 启动日志
            var dt = DateTime.Now;
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.RobotHand] = $"{actionName}启动"
            }, dt);

            try
            {
                SemiAutoResult result = await action();
                // 判断成功与否，假设 Level == SemiAutoResultCode.Success 代表成功
                if (result.Level == SemiAutoResultCode.Success)
                {
                    dt = DateTime.Now;
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.RobotHand] = $"{actionName}完成"
                    }, dt);
                }
                else
                {
                    AlarmTableMan.Insert(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.ALARM_TABLE.AlarmHead] = $"{actionName}异常",
                        [SmartColor.My_DataBase.ALARM_TABLE.AlarmDetails] = $"错误码: {result.Level}, {result.Message}"
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                AlarmTableMan.Insert(new Dictionary<string, object>
                {
                    [SmartColor.My_DataBase.ALARM_TABLE.AlarmHead] = $"{actionName}异常",
                    [SmartColor.My_DataBase.ALARM_TABLE.AlarmDetails] = ex.ToString()
                });
                // 返回异常码
                return new SemiAutoResult
                {
                    Level = SemiAutoResultCode.Exception, // 你可以自定义异常码
                    Message = ex.Message,
                    Exception = ex
                };
            }
        }

        public Task<SemiAutoResult> HomeAsync() => LogAndRun("回原点", () => PLC_SemiAutoHelper.HomeAsync());

        public Task<SemiAutoResult> AspirateAsync(int z, int purgeCount, short syringeType, short currentWeight) => LogAndRun(
            $"抽液(脉冲：{z};排空次数:{purgeCount};针筒类型:{syringeType};当前重量:{currentWeight})",
            () => PLC_SemiAutoHelper.AspirateAsync(z, purgeCount, syringeType, currentWeight));
        public Task<SemiAutoResult> DispenseAsync(int z, int syringeType, short dispenseHeight) => LogAndRun(
            $"注液(脉冲：{z};针筒类型:{syringeType};注液高度:{dispenseHeight})",
            () => PLC_SemiAutoHelper.DispenseAsync(z, syringeType, dispenseHeight));
        public Task<SemiAutoResult> AddWaterAsync(double objectWeight, double waterTime) => LogAndRun(
            $"加水(目标重量:{objectWeight};时间:{Math.Round(waterTime, 3)}s)",
            () => PLC_SemiAutoHelper.AddWaterAsync(objectWeight, waterTime));
        public Task<SemiAutoResult> ReleaseNeedleAsync(int syringeType) => LogAndRun(
            $"放针(针筒类型:{syringeType})",
            () => PLC_SemiAutoHelper.ReleaseNeedleAsync(syringeType));
        public Task<SemiAutoResult> DecompressAsync() => LogAndRun(
            "泄压",
            () => PLC_SemiAutoHelper.DecompressAsync());
        public Task<SemiAutoResult> RelativeMoveAsync(short axis, int pulse, int hSpeed, int upSpeed) =>
            LogAndRun($"相对移动(轴号：{axis};脉冲:{pulse};速度:{hSpeed};加减速:{upSpeed})",
                () => PLC_SemiAutoHelper.RelativeMoveAsync(axis, pulse, hSpeed, upSpeed));
        public Task<SemiAutoResult> ResetAsync() => LogAndRun(
            "复位",
            () => PLC_SemiAutoHelper.ResetAsync());
        public Task<SemiAutoResult> ActionCheckAsync() => LogAndRun(
            "动作检查",
            () => PLC_SemiAutoHelper.ActionCheckAsync());
        public Task<SemiAutoResult> OpenLidAsync(short lockSignal) => LogAndRun(
            $"开盖(锁止信号:{lockSignal})",
            () => PLC_SemiAutoHelper.OpenLidAsync(lockSignal));
        public Task<SemiAutoResult> PutLidAsync() => LogAndRun(
            "放盖",
            () => PLC_SemiAutoHelper.PutLidAsync());
        public Task<SemiAutoResult> TakeLidAsync() => LogAndRun(
            "取盖",
            () => PLC_SemiAutoHelper.TakeLidAsync());
        public Task<SemiAutoResult> CloseLidAsync(short lockSignal) => LogAndRun(
            "关盖",
            () => PLC_SemiAutoHelper.CloseLidAsync());
        public Task<SemiAutoResult> BalanceCheckAsync() => LogAndRun(
            "天平检查",
            () => PLC_SemiAutoHelper.BalanceCheckAsync());
        public Task<SemiAutoResult> RobotHandGraspingAsync(short graspType, short syringeType) => LogAndRun(
            $"机械手抓取(抓取类型:{graspType};针筒类型:{syringeType})",
            () => PLC_SemiAutoHelper.RobotHandGraspingAsync(graspType, syringeType));
        public Task<SemiAutoResult> RobotArmReleaseAsync(short graspType, short syringeType) => LogAndRun(
            $"机械手松放(抓取类型:{graspType};针筒类型:{syringeType})",
            () => PLC_SemiAutoHelper.RobotArmReleaseAsync(graspType, syringeType));
        public Task<SemiAutoResult> TakeDryClothAsync(short takeClothCylinderPos, short over20gCompensatePulse) => LogAndRun(
            $"取干布(气缸位置:{takeClothCylinderPos};布框类型:{over20gCompensatePulse})",
            () => PLC_SemiAutoHelper.TakeDryClothAsync(takeClothCylinderPos, over20gCompensatePulse));
        public Task<SemiAutoResult> TakeWetClothAsync() => LogAndRun(
            "取湿布",
            () => PLC_SemiAutoHelper.TakeWetClothAsync());
        public Task<SemiAutoResult> PutDryClothAsync() => LogAndRun(
            "放干布",
            () => PLC_SemiAutoHelper.PutDryClothAsync());
        public Task<SemiAutoResult> PutWetClothAsync(short outCylinderPos) => LogAndRun(
            $"放湿布(气缸位置:{outCylinderPos})",
            () => PLC_SemiAutoHelper.PutWetClothAsync(outCylinderPos));
        public Task<SemiAutoResult> UVAspirateAsync(int z, short syringeType) => LogAndRun(
            $"UV抽液(脉冲：{z};针筒类型:{syringeType})",
            () => PLC_SemiAutoHelper.UVAspirateAsync(z, syringeType));
        public Task<SemiAutoResult> WashSyringeAsync(short syringeType) => LogAndRun(
            "洗针筒",
            () => PLC_SemiAutoHelper.WashSyringeAsync(syringeType));
        public Task<SemiAutoResult> AddSolventAsync(double solventTime) => LogAndRun(
            $"加溶解剂(时间:{Math.Round(solventTime, 3)}s)",
            () => PLC_SemiAutoHelper.AddSolventAsync(solventTime));
        public Task<SemiAutoResult> SupportCoverAsync() => LogAndRun(
            "撑盖",
            () => PLC_SemiAutoHelper.SupportCoverAsync());
        public Task<SemiAutoResult> PHAspirateAsync(int z, short syringeType) => LogAndRun(
            $"PH抽液(脉冲：{z};针筒类型:{syringeType})",
            () => PLC_SemiAutoHelper.PHAspirateAsync(z, syringeType));
        public Task<SemiAutoResult> PHDrainAsync(int z) => LogAndRun(
            $"PH排液(脉冲：{z})",
            () => PLC_SemiAutoHelper.PHDrainAsync(z));
        public Task<SemiAutoResult> TakeMotherBottleAsync(int z, short takeHeight) => LogAndRun(
            $"取母液瓶(脉冲：{z};取瓶高度:{takeHeight})",
            () => PLC_SemiAutoHelper.TakeMotherBottleAsync(z, takeHeight));
        public Task<SemiAutoResult> PutMotherBottleAsync(int z, short takeHeight) => LogAndRun(
            $"放母液瓶(脉冲：{z};取瓶高度:{takeHeight})",
            () => PLC_SemiAutoHelper.PutMotherBottleAsync(z, takeHeight));
        public Task<SemiAutoResult> AddPowderAsync(int z) => LogAndRun(
            $"加粉(脉冲：{z})",
            () => PLC_SemiAutoHelper.AddPowderAsync(z));
        public Task<SemiAutoResult> BeforeAddPowderBlockAsync() => LogAndRun(
            "加粉前气缸到阻挡位",
            () => PLC_SemiAutoHelper.BeforeAddPowderBlockAsync());

        public async Task<SemiAutoResult> MoveToBottleAsync(int bottleNum, int x, int y, short isTrayIn, int allowMove)
        {
           
            return await LogAndRun($"寻找{bottleNum}号母液瓶", async () =>
            {
               
                var result = await PLC_SemiAutoHelper.MoveToPositionAsync(x, y, 0, 0, isTrayIn,0);
                return RobotAlarmOverview.HandleSemiAutoResult(result, $"寻找{bottleNum}号母液瓶");
            });
        }

        public async Task<SemiAutoResult> MoveToCupAsync(int cupNum, short needHome, int x, int y, short isTrayIn, int allowMove)
        {
           

            return await LogAndRun($"寻找{cupNum}号配液杯", async () =>
            {
              
                var result = await PLC_SemiAutoHelper.MoveToPositionAsync(x, y, needHome, 0,isTrayIn, allowMove);
                return RobotAlarmOverview.HandleSemiAutoResult(result, $"寻找{cupNum}号配液杯");
            });
        }

        public async Task<SemiAutoResult> MoveToCupLidAsync(int lidNum, int x, int y, int allowMove, short isTrayIn = 2)
        {
          

            return await LogAndRun($"寻找{lidNum}号杯盖", async () =>
            {
               
                var result = await PLC_SemiAutoHelper.MoveToPositionAsync(x, y, 0, 0,isTrayIn, allowMove);
                return RobotAlarmOverview.HandleSemiAutoResult(result, $"寻找{lidNum}号杯盖");
            });
        }

        public async Task<SemiAutoResult> MoveToBalanceAsync(int x, int y, short isTrayIn = 1)
        {
           

            return await LogAndRun("寻找天平位", async () =>
            {
              
                var result = await PLC_SemiAutoHelper.MoveToPositionAsync(x, y, 0, 0,isTrayIn,0);
                return RobotAlarmOverview.HandleSemiAutoResult(result, "寻找天平位");
            });
        }

        public async Task<SemiAutoResult> MoveToDecompressionAsync(int cupNum, int x, int y, short isTrayIn = 1)
        {
         
            return await LogAndRun($"寻找{cupNum}号配液杯泄压位", async () =>
            {
               
                var result = await PLC_SemiAutoHelper.MoveToPositionAsync(x, y, 0, 0,isTrayIn,0);
                return RobotAlarmOverview.HandleSemiAutoResult(result, $"寻找{cupNum}号配液杯泄压位");
            });
        }

        public async Task<SemiAutoResult> MoveToPrepareClothAsync(int clothNum, short needHome, int x, int y, short isTrayIn = 2)
        {
         
            return await LogAndRun($"寻找{clothNum}号备布位", async () =>
            {
              
                var result = await PLC_SemiAutoHelper.MoveToPositionAsync(x, y, needHome, 0,isTrayIn,0);
                return RobotAlarmOverview.HandleSemiAutoResult(result, $"寻找{clothNum}号备布位");
            });
        }

        public async Task<SemiAutoResult> MoveToOutClothAsync(int clothNum, int x, int y, short isTrayIn = 1)
        {
          
            return await LogAndRun($"寻找{clothNum}号出布位", async () =>
            {
               
                var result = await PLC_SemiAutoHelper.MoveToPositionAsync(x, y, 0, 0, isTrayIn,0);
                return RobotAlarmOverview.HandleSemiAutoResult(result, $"寻找{clothNum}号出布位");
            });
        }

        public async Task<SemiAutoResult> MoveToDryClampAsync(short needHome, int x, int y, int allowMove, short isTrayIn = 2)
        {
           

            return await LogAndRun("寻找干布夹子", async () =>
            {
              
                var result = await PLC_SemiAutoHelper.MoveToPositionAsync(x, y, needHome, 0, isTrayIn,0);
                return RobotAlarmOverview.HandleSemiAutoResult(result, "寻找干布夹子");
            });
        }

        public async Task<SemiAutoResult> MoveToWetClampAsync(short needHome, int x, int y, int allowMove, short isTrayIn = 2)
        {
          

            return await LogAndRun("寻找湿布夹子", async () =>
            {
               
                var result = await PLC_SemiAutoHelper.MoveToPositionAsync(x, y, needHome, 0, isTrayIn,0);
                return RobotAlarmOverview.HandleSemiAutoResult(result, "寻找湿布夹子");
            });
        }

        public async Task<SemiAutoResult> MoveToWashAsync(int x, int y, short isTrayIn = 1)
        {
           

            return await LogAndRun("寻找洗针位", async () =>
            {
              
                var result = await PLC_SemiAutoHelper.MoveToPositionAsync(x, y, 0, 0,isTrayIn,0);

                return RobotAlarmOverview.HandleSemiAutoResult(result, "寻找洗针位");
            });
        }

        public async Task<SemiAutoResult> MoveToStandbyAsync()
        {
            if (SemiAutoHelperFactory.IsStandby)
            {
                return new SemiAutoResult
                {
                    Level = SemiAutoResultCode.Success,
                };
            }          

            return await LogAndRun("寻找待机位", async () =>
            {
                var standbyCTR = await AreaCoordinateFinder.TryGetStandbyCoordinateAsync();
                if(!standbyCTR.found)
                    throw new ArgumentException("待机位坐标获取失败");
                var result = await PLC_SemiAutoHelper.MoveToPositionAsync(standbyCTR.x, standbyCTR.y, 0, 1,2,0);
                return RobotAlarmOverview.HandleSemiAutoResult(result, "寻找待机位");
            });
        }
    }
}