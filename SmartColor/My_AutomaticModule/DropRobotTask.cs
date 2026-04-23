using com.google.zxing;
using Microsoft.VisualBasic;
using SmartColor.My_ConPar.Area.Drop;
using SmartColor.My_ConPar.Order;
using SmartColor.My_Cup;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Form.Homepage;
using SmartColor.My_RobotManager;
using SmartColor.My_SemiAutoModule;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SmartColor.My_AutomaticModule
{
    /// <summary>
    /// 单杯加水操作结果类型(成功，异常，取消)
    /// </summary>
    public class SingleCupAddWaterResult
    {
        /// <summary>结果码(成功，异常，取消)</summary>
        public My_Tool.Result.ResultCode Code { get; set; }

        /// <summary>杯号</summary>
        public int CupNo { get; set; }

        /// <summary>详细信息</summary>
        public string Message { get; set; }
        /// <summary>目标加水重量</summary>
        public double TargetWeight { get; set; }
        /// <summary>加水时间</summary>
        public double Time { get; set; }
        /// <summary>异常对象</summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// 加水流程结果类型(成功，失败，异常，取消)
    /// </summary>
    public class AddWaterResult
    {
        /// <summary>结果码(成功，失败，异常，取消)</summary>
        public My_Tool.Result.ResultCode Code { get; set; }

        /// <summary>详细信息</summary>
        public string Message { get; set; }

        /// <summary>异常对象</summary>
        public Exception Exception { get; set; }

        /// <summary>加水失败的杯号列表</summary>
        public List<int> FailCupNo { get; set; } = new List<int>();
    }

    /// <summary>
    /// 滴液流程结果类型(完成， 异常  取消)
    /// </summary>
    public class DropProcessResult
    {
        /// <summary>结果码(完成， 异常  取消）</summary>
        public My_Tool.Result.ResultCode Code { get; set; }
        /// <summary>详细信息</summary>
        public string Message { get; set; }
        /// <summary>异常对象</summary>
        public Exception Exception { get; set; }

        /// <summary>母液瓶液量低列表</summary>
        public List<int> BottleWeightLow { get; set; } = new List<int>();

        /// <summary>母液瓶液量过期列表</summary>
        public List<int> BottleOutdated { get; set; } = new List<int>();

        /// <summary>未发现针筒列表</summary>
        public List<int> NoSyeing { get; set; } = new List<int>();

        /// <summary>滴液失败的杯号列表</summary>
        public List<int> FailCupNo { get; set; } = new List<int>();


    }

    /// <summary>
    /// 单瓶滴液操作结果类型{成功,失败,异常,取消）
    /// </summary>
    public class SingleBottleDropResult
    {
        /// <summary>结果码{成功,失败,异常,取消）</summary>
        public My_Tool.Result.ResultCode Code { get; set; }

        /// <summary>完成标志位(true=完成，false=未完成)</summary>
        public bool Finish { get; set; }

        /// <summary>详细信息</summary>
        public string Message { get; set; }

        /// <summary>当前瓶剩余杯需加脉冲</summary>
        public List<RemainingPulses> DropData { get; set; } = new List<RemainingPulses>();

        /// <summary>异常对象</summary>
        public Exception Exception { get; set; }

        /// <summary>滴液失败的杯号列表</summary>
        public List<int> FailCupNo { get; set; } = new List<int>();
    }

    /// <summary>
    /// 杯号和剩余脉冲封装类
    /// </summary>
    public class RemainingPulses
    {
        /// <summary>杯号</summary>
        public int CupNo { get; set; }

        /// <summary>脉冲</summary>
        public int Pulse { get; set; }

        /// <summary>目标滴液重量</summary>
        public double ObjectDropWeight { get; set; }
    }

    /// <summary>
    /// 滴液操作结果类型(成功，失败，异常,任务取消)
    /// </summary>
    public class DropResult
    {
        /// <summary>结果码，(成功，失败，异常,任务取消)</summary>
        public My_Tool.Result.ResultCode Code { get; set; }

        /// <summary>详细信息</summary>
        public string Message { get; set; }

        /// <summary>异常对象</summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// 滴液业务流程的原子任务封装工具类
    /// 只负责流程和详细结果，不负责弹窗交互
    /// </summary>
    internal class DropRobotTask
    {

        // 在 DropRobotTask 类内添加事件定义
        public static event Action<int> CupFinished; // 参数为 cupNum

        /// <summary>
        /// 停止变量，供外部调用以停止当前正在执行的滴液任务
        /// </summary>
        public static volatile bool IsStopped = false;

        private static readonly List<int> _pendingDyeingCups = new List<int>();

        /// <summary>
        /// 单杯加水
        /// </summary>
        /// <param name="batchNo">批次号</param>
        /// <param name="cupNo">杯号</param>
        /// <param name="targetAddWeight">加水量</param>
        /// <returns>成功，异常，取消</returns>
        private static async Task<SingleCupAddWaterResult> SingleCupAddWaterAsync(string batchNo, int cupNo, double targetAddWeight)
        {
            var result = new SingleCupAddWaterResult
            {
                CupNo = cupNo,
                TargetWeight = targetAddWeight
            };
            try
            {
                var currentTask = SmartColor.My_RobotManager.RobotTaskManager.Instance.CurrentTask as SmartColor.My_RobotManager.RobotBusinessTask<SingleCupAddWaterResult>;
                if (My_ConPar.Choices.IsDebug == 0)
                {
                    var cupCT = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cupNo, false);


                    //1. 并行 计算加水时间 和 杯号移动
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();

                    var calcTask = Task.Run(() =>
                    {
                        return targetAddWeight / SmartColor.My_ConPar.Correction.Correcting_Water_Value;
                    });


                    var cupCTR = await cupCT;
                    if (!cupCTR.found)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = $"获取{cupNo}号配液杯位置异常";
                        result.Exception = new Exception($"获取{cupNo}号配液杯位置异常");
                        return result;
                    }

                    var moveTask = SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.MoveToCupAsync(cupNo, 0, cupCTR.x, cupCTR.y, 1, 0);

                    await Task.WhenAll(calcTask, moveTask);

                    var addTime = calcTask.Result;
                    result.Time = addTime;

                    var semiAutoResult = await moveTask;
                    switch (semiAutoResult.Level)
                    {
                        case SemiAutoResultCode.Exception:
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = semiAutoResult.Message;
                            result.Exception = semiAutoResult.Exception;
                            return result;
                        case SemiAutoResultCode.NeedInteraction:
                            result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                            result.Message = semiAutoResult.Message;
                            return result;
                        default:
                            break;
                    }

                    //2. 执行加水
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();

                    semiAutoResult = await SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.AddWaterAsync(targetAddWeight, addTime);

                    switch (semiAutoResult.Level)
                    {
                        case SemiAutoResultCode.Exception:
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = semiAutoResult.Message;
                            result.Exception = semiAutoResult.Exception;
                            return result;
                        case SemiAutoResultCode.NeedInteraction:
                            result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                            result.Message = semiAutoResult.Message;
                            return result;
                        default:
                            break;
                    }

                }
                else
                {
                    var balanceCT = My_Tool.AreaCoordinateFinder.TryGetBalanceCoordinateAsync();
                    var balanceTask = My_Tool.BalanceStableReading.CheckAndReadBalanceAsync();

                    //1. 并行 计算加水时间 和 杯号移动
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();

                    var calcTask = Task.Run(() =>
                    {
                        return targetAddWeight / SmartColor.My_ConPar.Correction.Correcting_Water_Value;
                    });


                    var balanceCTR = await balanceCT;
                    if (!balanceCTR.found)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = $"获取天平位置异常";
                        result.Exception = new Exception($"获取天平位置异常");
                        return result;
                    }

                    var moveTask = SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.MoveToBalanceAsync(balanceCTR.x, balanceCTR.y);

                    await Task.WhenAll(calcTask, moveTask, balanceTask);

                    var addTime = calcTask.Result;
                    result.Time = addTime;

                    var semiAutoResult = await moveTask;
                    switch (semiAutoResult.Level)
                    {
                        case SemiAutoResultCode.Exception:
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = semiAutoResult.Message;
                            result.Exception = semiAutoResult.Exception;
                            return result;
                        case SemiAutoResultCode.NeedInteraction:
                            result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                            result.Message = semiAutoResult.Message;
                            return result;
                        default:
                            break;
                    }

                    var balanceResult = await balanceTask;
                    if (balanceResult == 9999.99)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = "天平异常";
                        result.Exception = new Exception("天平异常");
                        return result;
                    }
                    double initialWeight = balanceResult;

                    //2. 执行加水
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();

                    semiAutoResult = await SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.AddWaterAsync(targetAddWeight, addTime);

                    switch (semiAutoResult.Level)
                    {
                        case SemiAutoResultCode.Exception:
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = semiAutoResult.Message;
                            result.Exception = semiAutoResult.Exception;
                            return result;
                        case SemiAutoResultCode.NeedInteraction:
                            result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                            result.Message = semiAutoResult.Message;
                            return result;
                        default:
                            break;
                    }

                    var finalBalanceResult = await My_Tool.BalanceStableReading.CheckAndReadBalanceAsync();

                    if (finalBalanceResult == 9999.99)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = "天平异常";
                        result.Exception = new Exception("天平异常");
                        return result;
                    }

                    double finalWeight = finalBalanceResult;
                    double actualAddWeight = finalWeight - initialWeight;

                    //更新数据库加水结果
                    My_DataBase.SqlServer.Update(DROP_HEAD.TableName,
                        new Dictionary<string, object>
                        {
                            { DROP_HEAD.RealAddWaterWeight, actualAddWeight },
                            { DROP_HEAD.AddWaterFinish, 1 }
                        },
                        $"{DROP_HEAD.BatchName} = @BatchName AND {DROP_HEAD.CupNum} = @CupNum",
                        new SqlParameter("@BatchName", batchNo),
                        new SqlParameter("@CupNum", cupNo)
                        );
                    //更新杯中液量
                    My_DataBase.SqlServer.ExecuteNonQuery($@"
                        UPDATE {CUP_DETAILS.TableName} SET 
                            {CUP_DETAILS.CurrentWeight} = ISNULL({CUP_DETAILS.CurrentWeight},0) + @AddWeight
                        WHERE {CUP_DETAILS.CupNum} = @CupNum",
                        new SqlParameter("@AddWeight", actualAddWeight),
                        new SqlParameter("@CupNum", cupNo)
                        );

                    var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                    if (area != null)
                    {
                        area.OnCupDataReceived(cupNo);
                    }


                }
                await DropFinishAsync(batchNo, cupNo);
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "加水完成";
                return result;
            }
            catch (TaskCanceledException)
            {
                result.Code = My_Tool.Result.ResultCode.Canceled;
                result.Message = "任务被取消";
                return result;
            }
            catch (Exception ex)
            {
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = "代码异常：" + ex.Message;
                result.Exception = ex;
                return result;
            }
        }

        /// <summary>
        /// 单瓶滴液流程
        /// </summary>
        /// <param name="bottleNo">瓶号</param>
        /// <param name="batchNo">批次号</param>
        /// <param name="remainingPulses">剩余脉冲</param>
        /// <returns>成功,失败,异常,取消</returns>
        private static async Task<SingleBottleDropResult> SingleSyringeDropAsync(
            int bottleNo, string batchNo, List<RemainingPulses> remainingPulses, List<int> failCupNo = null, Task<(bool fail, List<RemainingPulses> dropData)> updateTask = null)
        {
            SingleBottleDropResult result = new SingleBottleDropResult();

            try
            {
                var currentTask = SmartColor.My_RobotManager.RobotTaskManager.Instance.CurrentTask as SmartColor.My_RobotManager.RobotBusinessTask<SingleBottleDropResult>;

                //1.并发查找资料,计算脉冲和移动到母液瓶
                var bottleCTR = await My_Tool.AreaCoordinateFinder.TryGetBottleCoordinateAsync(bottleNo);
                if (!bottleCTR.found)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"获取{bottleNo}号母液瓶坐标异常";
                    result.Exception = new Exception($"获取{bottleNo}号母液瓶坐标异常");
                    return result;
                }
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                var moveResult = SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.MoveToBottleAsync(bottleNo, bottleCTR.x, bottleCTR.y, 2, 1);
                int retainDecimals = My_Tool.BalanceStableReading.RetainDecimals();
                var dropData = remainingPulses;
                short syringe_Type = 0;
                double adjust = 0;

                DateTime lastUseTime = DateTime.MinValue;
                int evacuateSpan = 0;
                //查询母液瓶资料
                var bottleInfo = My_DataBase.BottleData.Bottle_details.Select($"{BOTTLE_DETAILS.BottleNum} = {bottleNo}").FirstOrDefault();
                if (bottleInfo == null)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "查找资料异常";
                    result.Exception = new Exception($"瓶号{bottleNo}母液瓶资料不存在");
                    return result;
                }

                var syringeType = bottleInfo[BOTTLE_DETAILS.SyringeType]?.ToString();
                var currentWeight = Convert.ToDouble(bottleInfo[BOTTLE_DETAILS.CurrentWeight] ?? "0");
                switch (syringeType)
                {
                    case "小针筒":
                        syringe_Type = 0;
                        break;
                    case "大针筒":
                        syringe_Type = 1;
                        break;
                    default:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = $"针筒类型异常";
                        result.Exception = new Exception($"瓶号{bottleNo}母液瓶针筒类型异常");
                        return result;
                }

                adjust = Convert.ToDouble(bottleInfo[BOTTLE_DETAILS.AdjustValue] ?? "0");
                var lastUseTimeObj = bottleInfo[BOTTLE_DETAILS.LastUseTime];
                lastUseTime = (lastUseTimeObj != null && lastUseTimeObj != DBNull.Value)
                   ? Convert.ToDateTime(lastUseTimeObj)
                   : DateTime.MinValue;
                evacuateSpan = Convert.ToInt32(bottleInfo[BOTTLE_DETAILS.EvacuateSpan] ?? "0");
                int z = 0;
                if (dropData == null || dropData.Count == 0)
                {
                    // 查询滴液资料
                    string sql = $@"
                        SELECT * FROM {DROP_DETAILS.TableName} 
                            WHERE {DROP_DETAILS.BatchName} = @BatchName 
                            AND {DROP_DETAILS.BottleNum} = @BottleNum 
                            AND {DROP_DETAILS.Finish} = 0 
                            AND {DROP_DETAILS.CupNum} IN (
                                SELECT {DROP_HEAD.CupNum}
                                FROM {DROP_HEAD.TableName}
                                WHERE {DROP_HEAD.StartTime} IS NOT NULL)
                        ORDER BY {DROP_DETAILS.CupNum} ASC  
                        ";
                    var dt = SqlServer.ExecuteQuery(sql,
                        new SqlParameter("@BatchName", batchNo),
                        new SqlParameter("@BottleNum", bottleNo));

                    if (dt.Rows.Count == 0)
                    {
                        //当前瓶无滴液数据
                        result.Code = My_Tool.Result.ResultCode.Success;
                        result.Message = $"瓶号{bottleNo}无滴液数据";
                        return result;
                    }



                    // 组装滴液数据和计算抽液脉冲

                    dropData = new List<RemainingPulses>();
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        int objPulse = Convert.ToInt32(Convert.ToDouble(row[DROP_DETAILS.ObjectDropWeight]) * adjust);
                        //组装滴液数据
                        dropData.Add(new RemainingPulses
                        {
                            CupNo = Convert.ToInt32(row[DROP_DETAILS.CupNum]),
                            Pulse = objPulse,
                            ObjectDropWeight = Convert.ToDouble(row[DROP_DETAILS.ObjectDropWeight])
                        });


                    }

                    var semiAutoResult1 = await moveResult;

                    switch (semiAutoResult1.Level)
                    {
                        case SemiAutoResultCode.Exception:
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = semiAutoResult1.Message;
                            result.Exception = semiAutoResult1.Exception;
                            return result;
                        case SemiAutoResultCode.NeedInteraction:
                            result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                            result.Message = semiAutoResult1.Message;
                            return result;
                        default:
                            break;
                    }

                    //调用动作检查防止装针
                    var checkTask = SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.ActionCheckAsync();
                    var checkResult = await checkTask;
                    switch (checkResult.Level)
                    {
                        case SemiAutoResultCode.Exception:
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = checkResult.Message;
                            result.Exception = checkResult.Exception;

                            return result;
                        case SemiAutoResultCode.NeedInteraction:
                            result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                            result.Message = checkResult.Message;
                            return result;

                        default:
                            break;
                    }
                }

                //计算抽液脉冲和组装该论针筒滴液数组
                var curretntDropData = new List<RemainingPulses>();
                int maxP = syringe_Type == 0 ? My_ConPar.Other.S_MaxPulse : My_ConPar.Other.B_MaxPulse;
                int rw = syringe_Type == 0 ? My_ConPar.Correction.Correcting_S_Weight : My_ConPar.Correction.Correcting_B_Weight;
                int availablePulse = maxP + My_ConPar.Other.Z_BackPulse - Convert.ToInt32(rw * adjust) - Convert.ToInt32(1 * adjust);
                int purgeCount = lastUseTime.AddHours(evacuateSpan) > DateTime.Now ? 0 : My_ConPar.Other.DrainCount;

                foreach (var drop in dropData)
                {
                    if (My_ConPar.Choices.DripFull == 1)
                    {
                        //满量程抽液
                        if (z + drop.Pulse > availablePulse)
                        {
                            curretntDropData.Add(new RemainingPulses
                            {
                                CupNo = drop.CupNo,
                                Pulse = availablePulse - z,
                                ObjectDropWeight = drop.ObjectDropWeight
                            });
                            z = availablePulse;
                            break;
                        }
                        else
                        {
                            z += drop.Pulse;
                            curretntDropData.Add(drop);
                        }
                    }
                    else
                    {
                        //精准抽液
                        if (z + drop.Pulse <= availablePulse)
                        {
                            z += drop.Pulse;
                            curretntDropData.Add(drop);
                        }
                        else
                        {
                            //如果一杯就超量，直接用满量程
                            if (curretntDropData.Count == 0)
                            {
                                curretntDropData.Add(new RemainingPulses
                                {
                                    CupNo = drop.CupNo,
                                    Pulse = availablePulse - z,
                                    ObjectDropWeight = drop.ObjectDropWeight
                                });
                                z = availablePulse;
                            }
                        }
                    }
                }

                z += (Convert.ToInt32(rw * adjust) + Convert.ToInt32(1 * adjust) - My_ConPar.Other.Z_BackPulse);
                var semiAutoResult = await moveResult;

                switch (semiAutoResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = semiAutoResult.Message;
                        result.Exception = semiAutoResult.Exception;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = semiAutoResult.Message;
                        return result;
                    default:
                        break;
                }

                //2. 抽液
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                semiAutoResult = await SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.AspirateAsync(z, purgeCount, syringe_Type, Convert.ToInt16(currentWeight));

                //更新当前瓶上次使用时间
                My_DataBase.SqlServer.Update(BOTTLE_DETAILS.TableName,
                    new Dictionary<string, object>
                    {
                        { BOTTLE_DETAILS.LastUseTime, DateTime.Now }
                    },
                    $"{BOTTLE_DETAILS.BottleNum} = {bottleNo}");



                switch (semiAutoResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = semiAutoResult.Message;
                        result.Exception = semiAutoResult.Exception;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = semiAutoResult.Message;
                        return result;
                    default:
                        break;
                }

                //等待上一轮更新任务完成
                if (updateTask != null)
                {
                    var updateResult = await updateTask;
                    if (updateResult.fail)
                    {
                        if (failCupNo == null)
                        {
                            failCupNo = new List<int>();
                        }
                        failCupNo.AddRange(updateResult.dropData.ToArray().Select(d => d.CupNo));
                        failCupNo.Distinct().ToList();
                    }

                }


                //3. 遍历滴液数据，依次滴液
                var rp = z + My_ConPar.Other.Z_BackPulse;
                Task<double> balanceTask = null;
                bool isFinish = true;
                var balanceCT = My_Tool.AreaCoordinateFinder.TryGetBalanceCoordinateAsync();
                var cupCT = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(curretntDropData[0].CupNo, false);
                double initialWeight = 0;
                int j = 0;
                for (int i = 0; i < curretntDropData.Count; i++)
                {
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();

                    var drop = curretntDropData[i];

                    //根据当前的索引判断是否小于等于倒数第二个滴液，如果是，则启动CheckAndReadBalanceAsync
                    if ((i >= curretntDropData.Count - 2 && balanceTask == null) || My_ConPar.Choices.IsDebug == 1)
                    {
                        balanceTask = My_Tool.BalanceStableReading.CheckAndReadBalanceAsync();
                    }


                    // 移动到杯号
                    var cupCTR = My_ConPar.Choices.IsDebug == 0 ?
                        await cupCT : await balanceCT;
                    if (!cupCTR.found)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = My_ConPar.Choices.IsDebug == 0 ? $"获取{drop.CupNo}号配液杯位置异常" : $"获取天平位置异常";
                        result.Exception = new Exception(My_ConPar.Choices.IsDebug == 0 ? $"获取{drop.CupNo}号配液杯位置异常" : $"获取天平位置异常");
                        return result;
                    }
                    semiAutoResult = My_ConPar.Choices.IsDebug == 0 ?
                        await SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.MoveToCupAsync(drop.CupNo, 0, cupCTR.x, cupCTR.y, 1, 0) :
                        await SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.MoveToBalanceAsync(cupCTR.x, cupCTR.y);
                    switch (semiAutoResult.Level)
                    {
                        case SemiAutoResultCode.Exception:
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = semiAutoResult.Message;
                            result.Exception = semiAutoResult.Exception;
                            return result;
                        case SemiAutoResultCode.NeedInteraction:
                            result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                            result.Message = semiAutoResult.Message;
                            return result;
                        default:
                            break;
                    }
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();

                    // 滴液
                    if (j + 1 < curretntDropData.Count && My_ConPar.Choices.IsDebug == 0)
                        cupCT = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(curretntDropData[++j].CupNo, false);
                    rp -= drop.Pulse;

                    if (My_ConPar.Choices.IsDebug == 1)
                    {
                        var balanceResult = await balanceTask;
                        if (balanceResult == 9999.99)
                        {
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = "天平异常";
                            result.Exception = new Exception("天平异常");
                            return result;
                        }
                        initialWeight = balanceResult;
                    }

                    semiAutoResult = await SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.DispenseAsync(rp, syringe_Type, 0);
                    switch (semiAutoResult.Level)
                    {
                        case SemiAutoResultCode.Exception:
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = semiAutoResult.Message;
                            result.Exception = semiAutoResult.Exception;
                            return result;
                        case SemiAutoResultCode.NeedInteraction:
                            result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                            result.Message = semiAutoResult.Message;
                            return result;
                        case SemiAutoResultCode.MechanicalReset:
                            {
                                var r = await MechanicalReset.ResetMechanical(bottleNo, semiAutoResult.Message);
                                result.Code = r.Code;
                                result.Message = r.Message;
                                return result;
                            }
                        default:
                            break;
                    }

                    //更新滴液数据
                    var dropDataItem = dropData.FirstOrDefault(d => d.CupNo == drop.CupNo);
                    int isCurrentCupFinished = 0;
                    if (dropDataItem != null)
                    {
                        dropDataItem.Pulse -= drop.Pulse;
                        if (dropDataItem.Pulse > 0)
                        {
                            isFinish = false;
                        }
                        else
                        {
                            // 滴液完成，移除该数据
                            dropData.Remove(dropDataItem);
                            isCurrentCupFinished = 1;
                            if (dropData.Count > 0)
                            {
                                isFinish = false;
                            }
                            else
                            {
                                isFinish = true;
                            }
                        }

                        if (My_ConPar.Choices.IsDebug == 1)
                        {
                            // 1. 读取天平稳定数据
                            var finialWeight = await My_Tool.BalanceStableReading.StableReadingAsync();
                            double actualDropWeight = Math.Round(finialWeight - initialWeight, retainDecimals);
                            if (actualDropWeight <= 0) actualDropWeight = 0;


                            //更新滴液详情表
                            My_DataBase.SqlServer.ExecuteNonQuery($@"
                                UPDATE {DROP_DETAILS.TableName} SET 
                                    {DROP_DETAILS.RealDropWeight} = ISNULL({DROP_DETAILS.RealDropWeight},0) + @AddWeight,
                                    {DROP_DETAILS.Finish} = @Finish 
                                WHERE {DROP_DETAILS.CupNum} = @CupNum AND {DROP_DETAILS.BottleNum} = @bottleNo AND {DROP_DETAILS.BatchName} = @batchNum",
                              new SqlParameter("@AddWeight", actualDropWeight),
                              new SqlParameter("@CupNum", drop.CupNo),
                              new SqlParameter("@Finish", isCurrentCupFinished),
                              new SqlParameter("@bottleNo", bottleNo),
                              new SqlParameter("@batchNum", batchNo));

                            My_DataBase.SqlServer.ExecuteNonQuery($@"
                                UPDATE {CUP_DETAILS.TableName} SET 
                                    {CUP_DETAILS.CurrentWeight} = ISNULL({CUP_DETAILS.CurrentWeight},0) + @AddWeight
                                WHERE {CUP_DETAILS.CupNum} = @CupNum",
                              new SqlParameter("@AddWeight", actualDropWeight),
                              new SqlParameter("@CupNum", drop.CupNo));

                            var area = CupCommManager.Instance.FindCupAreaByCupNum(drop.CupNo);
                            if (area != null)
                            {
                                area.OnCupDataReceived(drop.CupNo);
                            }

                            My_DataBase.SqlServer.ExecuteNonQuery($@"
                                UPDATE {BOTTLE_DETAILS.TableName} SET 
                                    {BOTTLE_DETAILS.CurrentWeight} = CASE 
                                        WHEN ({BOTTLE_DETAILS.CurrentWeight} - @AddWeight) < 0 THEN 0 
                                        ELSE ({BOTTLE_DETAILS.CurrentWeight} - @AddWeight) 
                                    END
                                WHERE {BOTTLE_DETAILS.BottleNum} = @bottleNo",
                                 new SqlParameter("@AddWeight", actualDropWeight),
                                 new SqlParameter("@bottleNo", bottleNo));
                            // _ = DropFinishAsync(batchNo, drop.CupNo);
                        }
                    }
                }

                if (My_ConPar.Choices.IsDebug == 0)
                {
                    //5.等待天平检查和读数完成
                    var balanceResult = await balanceTask;
                    if (balanceResult == 9999.99)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = "天平异常";
                        result.Exception = new Exception("天平异常");
                        return result;
                    }
                    initialWeight = balanceResult;

                    //6. 移动到天平
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();
                    var balanceCTR = await balanceCT;
                    if (!balanceCTR.found)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = "获取天平坐标异常";
                        result.Exception = new Exception("获取天平坐标异常");
                        return result;
                    }

                    semiAutoResult = await SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.MoveToBalanceAsync(balanceCTR.x, balanceCTR.y);
                    switch (semiAutoResult.Level)
                    {
                        case SemiAutoResultCode.Exception:
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = semiAutoResult.Message;
                            result.Exception = semiAutoResult.Exception;
                            return result;
                        case SemiAutoResultCode.NeedInteraction:
                            result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                            result.Message = semiAutoResult.Message;
                            return result;
                        default:
                            break;
                    }




                    //6. 注液验证
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();
                    rp -= Convert.ToInt32(rw * adjust);
                    semiAutoResult = await SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.DispenseAsync(rp, syringe_Type, 0);
                    switch (semiAutoResult.Level)
                    {
                        case SemiAutoResultCode.Exception:
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = semiAutoResult.Message;
                            result.Exception = semiAutoResult.Exception;
                            return result;
                        case SemiAutoResultCode.NeedInteraction:
                            result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                            result.Message = semiAutoResult.Message;
                            return result;
                        case SemiAutoResultCode.MechanicalReset:
                            {
                                var r = await MechanicalReset.ResetMechanical(bottleNo, semiAutoResult.Message);
                                result.Code = r.Code;
                                result.Message = r.Message;
                                return result;
                            }
                        default:
                            break;
                    }
                }

                //7.移动到母液瓶
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                semiAutoResult = await SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.MoveToBottleAsync(bottleNo, bottleCTR.x, bottleCTR.y, 1, 0);
                switch (semiAutoResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = semiAutoResult.Message;
                        result.Exception = semiAutoResult.Exception;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = semiAutoResult.Message;
                        return result;
                    default:
                        break;
                }


                //8. 异步更新滴液数据和母液瓶重量
                var updateDropTask = UpdateDropDataAndBottleWeightAsync(initialWeight, rw, curretntDropData, dropData, batchNo, bottleNo);


                //9. 如果当前瓶完成或者机械手任务中心有优先级比你高的任务，必须放针
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var releaseNeedleResult = await CheckAndReleaseNeedleIfNeededAsync(isFinish,
                    currentTask != null ? BigProcess.DropProcess * 100 : int.MaxValue, syringe_Type);

                switch (releaseNeedleResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        {
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = releaseNeedleResult.Message;
                            result.Exception = releaseNeedleResult.Exception;
                            return result;
                        }
                    case SemiAutoResultCode.NeedInteraction:
                        {
                            result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                            result.Message = releaseNeedleResult.Message;
                            return result;
                        }
                    case SemiAutoResultCode.NeedUseing:
                        {
                            //继续使用针筒
                            return await SingleSyringeDropAsync(bottleNo, batchNo, dropData, failCupNo, updateDropTask);
                        }
                    default:
                        {
                            //放针成功，等待更新结果
                            var updateResult = await updateDropTask;
                            if (updateResult.fail)
                            {
                                result.Code = My_Tool.Result.ResultCode.Failure;
                                result.Message = "滴液失败，放针成功";
                                result.DropData = dropData;
                                if (failCupNo == null)
                                {
                                    failCupNo = new List<int>();
                                }
                                failCupNo.AddRange(curretntDropData.ToArray().Select(d => d.CupNo).ToList());
                                var ints = failCupNo.Distinct().ToList();
                                ints.Sort();
                                result.FailCupNo = ints;
                            }
                            else
                            {
                                result.Code = My_Tool.Result.ResultCode.Success;
                                result.Message = "滴液完成，放针成功";
                                result.DropData = dropData;

                                if (failCupNo != null && failCupNo.Count > 0)
                                {
                                    var ints = failCupNo.Distinct().ToList();
                                    ints.Sort();
                                    result.FailCupNo = ints;
                                }

                            }
                            result.Finish = isFinish;
                            return result;
                        }
                }



            }
            catch (TaskCanceledException)
            {
                result.Code = My_Tool.Result.ResultCode.Canceled;
                result.Message = "任务被取消";
                return result;
            }
            catch (Exception ex)
            {
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = "代码异常：" + ex.Message;
                result.Exception = ex;
                return result;

            }


        }

        /// <summary>
        /// 异步更新滴液数据和母液瓶重量（天平读数+数据库更新+业务结果返回）
        /// 该方法会在独立线程中完成天平稳定读数、计算实际滴液重量、更新数据库和母液瓶重量，并返回关键业务数据
        /// </summary>
        /// <param name="initialWeight">天平初始重量</param>
        /// <param name="rw">针筒校正重量</param>
        /// <param name="curretntDropData">本轮滴液数据</param>
        /// <param name="dropData">剩余滴液数据（会被移除已完成项）</param>
        /// <param name="batchNo">批次号</param>
        /// <param name="bottleNo">瓶号</param>

        /// <returns>fail: 是否失败, dropData: 剩余滴液数据</returns>
        private static Task<(bool fail, List<RemainingPulses> dropData)> UpdateDropDataAndBottleWeightAsync(
            double initialWeight, int rw, List<RemainingPulses> curretntDropData, List<RemainingPulses> dropData,
            string batchNo, int bottleNo)
        {
            if (My_ConPar.Choices.IsDebug == 1)
            {

                return Task.FromResult((false, curretntDropData));
            }

            return Task.Run(async () =>
            {
                // 1. 读取天平稳定数据
                var finialWeight = await My_Tool.BalanceStableReading.StableReadingAsync();
                int retainDecimals = My_Tool.BalanceStableReading.RetainDecimals();
                double actualDropWeight = Math.Round(finialWeight - initialWeight, retainDecimals);
                if (actualDropWeight <= 0) actualDropWeight = 0;
                double re = Math.Round(actualDropWeight - rw, retainDecimals);
                double allowErr = My_Tool.BottleAuxiliary.GetAllowErr(bottleNo);
                // 2. 更新滴液详细数据库
                double totalDropWeight = 0;
                var cupWeightCaseWhens = new StringBuilder();
                var inParams = new StringBuilder();
                var paramList = new List<SqlParameter>();
                _pendingDyeingCups.Clear();
                foreach (var drop in curretntDropData)
                {
                    if (drop.Pulse > 0) continue;
                    dropData.RemoveAll(d => d.CupNo == drop.CupNo && d.Pulse == 0);
                    double aw;
                    if (My_ConPar.Choices.CorrectData == 1 && Math.Abs(re) > allowErr && actualDropWeight > 0)
                    {
                        // 启用修正，误差超限且复检重量大于0，直接写需加重量
                        aw = drop.ObjectDropWeight;
                    }
                    else
                    {
                        aw = Math.Round(drop.ObjectDropWeight + re, retainDecimals);
                    }
                    totalDropWeight += aw;
                    string updateSql = $@"
                        UPDATE {DROP_DETAILS.TableName}
                        SET {DROP_DETAILS.Finish} = 1, {DROP_DETAILS.RealDropWeight} = @ActualDropWeight
                        WHERE {DROP_DETAILS.BatchName} = @BatchName
                        AND {DROP_DETAILS.BottleNum} = @BottleNum
                        AND {DROP_DETAILS.CupNum} = @CupNum";
                    SqlServer.ExecuteNonQuery(updateSql,
                        new SqlParameter("@ActualDropWeight", aw),
                        new SqlParameter("@BatchName", batchNo),
                        new SqlParameter("@BottleNum", bottleNo),
                        new SqlParameter("@CupNum", drop.CupNo));
                    // 异步检查当前杯是否完成
                    await DropFinishAsync(batchNo, drop.CupNo);

                    // 累加杯详情液量
                    cupWeightCaseWhens.Append($" WHEN {CUP_DETAILS.CupNum} = @CupNo{drop.CupNo} THEN {CUP_DETAILS.CurrentWeight} + @ActualWeight{drop.CupNo}");
                    inParams.Append(inParams.Length == 0 ? $"@CupNo{drop.CupNo}" : $",@CupNo{drop.CupNo}");
                    paramList.Add(new SqlParameter($"@CupNo{drop.CupNo}", drop.CupNo));
                    paramList.Add(new SqlParameter($"@ActualWeight{drop.CupNo}", aw));

                    var area = CupCommManager.Instance.FindCupAreaByCupNum(drop.CupNo);
                    if (area != null)
                    {
                        area.OnCupDataReceived(drop.CupNo);
                    }
                }

                // 批量累加更新杯详情液量
                if (paramList.Count > 0)
                {
                    string cupSql = $@"
                        UPDATE {CUP_DETAILS.TableName}
                        SET {CUP_DETAILS.CurrentWeight} = CASE {cupWeightCaseWhens} END
                        WHERE {CUP_DETAILS.CupNum} IN ({inParams})
                        ";
                    My_DataBase.SqlServer.ExecuteNonQuery(cupSql, paramList.ToArray());
                }

                // 3. 更新母液瓶当前重量
                var bottleInfo = My_DataBase.BottleData.Bottle_details.Select($"{BOTTLE_DETAILS.BottleNum} = {bottleNo}").FirstOrDefault();
                double cw = 0;
                if (bottleInfo != null)
                    cw = Convert.ToDouble(bottleInfo[BOTTLE_DETAILS.CurrentWeight] ?? "0");

                bool fail = Math.Round(Math.Abs(re), retainDecimals) > allowErr;
                double usedWeight = Math.Round(totalDropWeight + actualDropWeight, retainDecimals);
                double fcWeight = Math.Round(cw - usedWeight, retainDecimals);
                if (fcWeight < 0) fcWeight = 0;
                SqlServer.Update(
                    BOTTLE_DETAILS.TableName,
                    new Dictionary<string, object>
                    {
                        [BOTTLE_DETAILS.AdjustSuccess] = fail ? 0 : 1,
                        [BOTTLE_DETAILS.CurrentWeight] = fcWeight
                    },
                    $"{BOTTLE_DETAILS.BottleNum}=@BottleNum",
                    new SqlParameter("@BottleNum", bottleNo)
                );


                // 4. 返回关键业务数据
                return (fail, curretntDropData);
            });
        }

        /// <summary>
        /// 判断是否需要放针（当前瓶完成或有更高优先级任务），如需放针则自动执行放针动作
        /// </summary>
        /// <param name="curretntBottleFinish">当前瓶是否完成</param>
        /// <param name="currentPriority">当前任务优先级</param>
        /// <param name="syringeType">针筒类型</param>
        /// <param name="currentTask">当前业务任务对象，可为null</param>
        /// <returns>放针结果（0 = 放针成功，3= 需要继续使用，-1=放针异常）</returns>
        private static async Task<SemiAutoResult> CheckAndReleaseNeedleIfNeededAsync(
            bool curretntBottleFinish, int currentPriority, int syringeType)
        {
            bool needRelease = curretntBottleFinish;
            // 检查是否被取消
            if (IsStopped)
            {
                needRelease = true;

            }
            if (!needRelease)
            {
                // 检查是否有更高优先级任务
                needRelease = SmartColor.My_RobotManager.RobotTaskManager.Instance.HasHigherPriorityTask(currentPriority);

            }

            if (needRelease)
            {
                var semiAutoResult = await SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.ReleaseNeedleAsync(syringeType);
                return semiAutoResult;

            }
            // 还要继续使用，不放针
            return new SemiAutoResult
            {
                Level = SemiAutoResultCode.NeedUseing,
                Message = "继续使用当前针筒"
            };
        }

        /// <summary>
        /// 提交单杯加水流程为原子任务到机械手调度中心，返回详细业务结果
        /// </summary>
        /// <param name="batchNo">批次号</param>
        /// <param name="cupNo">杯号</param>
        /// <param name="targetAddWeight">目标加水重量（g）</param>
        /// <returns>成功，异常，取消</returns>
        public static async Task<SingleCupAddWaterResult> EnqueueSingleCupAddWaterAsync(string batchNo, int cupNo, double targetAddWeight)
        {
            var task = new SmartColor.My_RobotManager.RobotBusinessTask<SingleCupAddWaterResult>
            {
                Priority = SmartColor.My_ConPar.Order.BigProcess.DropProcess * 100 + SmartColor.My_ConPar.Order.DropProcess.DropProcess.AddWaterProcess,
                OriginalPriority = SmartColor.My_ConPar.Order.BigProcess.DropProcess * 100 + SmartColor.My_ConPar.Order.DropProcess.DropProcess.AddWaterProcess,
                BusinessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DropProcess,
                TaskName = $"加水({cupNo}-{targetAddWeight}g)",
                BusinessFlow = async () => await SingleCupAddWaterAsync(batchNo, cupNo, targetAddWeight)
            };

            try
            {
                if (IsStopped)
                {
                    throw new TaskCanceledException("用户请求停止滴液流程");

                }
                var result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<SingleCupAddWaterResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{cupNo}号配液杯加水{targetAddWeight}克异常", result.Message, btn =>
                            {
                                if (btn == "确认")
                                {
                                    tcs.SetResult(new SingleCupAddWaterResult
                                    {
                                        Code = My_Tool.Result.ResultCode.Success
                                    });
                                }

                            },
                            new[] { "确认" },
                            "确认"
                             );
                            return await tcs.Task;
                        }
                    case My_Tool.Result.ResultCode.NeedInteraction:
                        {
                            var tcs = new TaskCompletionSource<SingleCupAddWaterResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{cupNo}号配液杯加水{targetAddWeight}克询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueSingleCupAddWaterAsync(batchNo, cupNo, targetAddWeight);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new SingleCupAddWaterResult
                                    {
                                        Code = My_Tool.Result.ResultCode.Canceled,
                                        Message = "用户选择退出，任务已取消"
                                    });
                                }
                            },
                              new[] { "重试", "退出" },
                              "重试"
                            );
                            return await tcs.Task;
                        }


                    default:
                        return result;

                }
            }
            catch (TaskCanceledException)
            {
                return new SingleCupAddWaterResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    CupNo = cupNo,
                    TargetWeight = targetAddWeight,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                return new SingleCupAddWaterResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    CupNo = cupNo,
                    TargetWeight = targetAddWeight,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }

        /// <summary>
        /// 提交单母液瓶滴液流程为原子任务到机械手调度中心，返回详细业务结果
        /// </summary>
        /// <param name="bottleNo">瓶号</param>
        /// <param name="batchNo">批次号</param>
        /// <param name="remainingPulses">剩余脉冲</param>
        /// <returns>成功，失败，异常，取消</returns>
        public static async Task<SingleBottleDropResult> EnqueueBottleDropAsync(int bottleNo, string batchNo, List<RemainingPulses> remainingPulses)
        {
            var task = new SmartColor.My_RobotManager.RobotBusinessTask<SingleBottleDropResult>
            {
                Priority = SmartColor.My_ConPar.Order.BigProcess.DropProcess * 100 + SmartColor.My_ConPar.Order.DropProcess.DropProcess.AddDyeProcess,
                OriginalPriority = SmartColor.My_ConPar.Order.BigProcess.DropProcess * 100 + SmartColor.My_ConPar.Order.DropProcess.DropProcess.AddDyeProcess,
                BusinessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DropProcess,
                TaskName = $"{bottleNo}号母液瓶滴液",
                BusinessFlow = async () => await SingleSyringeDropAsync(bottleNo, batchNo, remainingPulses)
            };

            try
            {
                if (IsStopped)
                {
                    throw new TaskCanceledException("用户请求停止滴液流程");
                }
                var result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<SingleBottleDropResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{bottleNo}号母液瓶滴液异常", result.Message, btn =>
                            {
                                if (btn == "确认")
                                {
                                    tcs.SetResult(new SingleBottleDropResult
                                    {
                                        Code = My_Tool.Result.ResultCode.Success
                                    });
                                }

                            },
                            new[] { "确认" },
                            "确认"
                             );
                            return await tcs.Task;
                        }
                    case My_Tool.Result.ResultCode.NeedInteraction:
                        {
                            var tcs = new TaskCompletionSource<SingleBottleDropResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{bottleNo}号母液瓶滴液询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueBottleDropAsync(bottleNo, batchNo, remainingPulses);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new SingleBottleDropResult
                                    {
                                        Code = My_Tool.Result.ResultCode.Canceled,
                                        Message = "用户选择退出，任务已取消"
                                    });
                                }
                            },
                              new[] { "重试", "退出" },
                              "重试"
                            );
                            return await tcs.Task;
                        }


                    default:
                        return result;

                }
            }
            catch (TaskCanceledException)
            {
                return new SingleBottleDropResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                My_File.Logger.Error("EnqueueSyringeDropAsync异常", ex);
                return new SingleBottleDropResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }



        /// <summary>
        /// 滴液加水流程
        /// </summary>
        /// <param name="dt">批次数据</param>
        /// <returns>成功，失败，异常，取消</returns>
        private static async Task<AddWaterResult> WaterProcess(DataTable dt)
        {
            AddWaterResult result = new AddWaterResult();
            try
            {
                // Task<double> balanceTask = null;
                // 1. 遍历加水数据，逐杯处理
                _pendingDyeingCups.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    int cupNo = Convert.ToInt32(row[DROP_HEAD.CupNum]);
                    string batchNo = Convert.ToString(row[DROP_HEAD.BatchName]);
                    double targetAddWeight = Convert.ToDouble(row[DROP_HEAD.ObjectAddWaterWeight]);
                    int finish = Convert.ToInt32(row[DROP_HEAD.AddWaterFinish]);

                    // 已加水完成，跳过
                    if (finish == 1)
                        continue;

                    // 目标加水量小于等于0，直接视为完成，不进行加水操作
                    if (targetAddWeight <= 0)
                    {
                        // 更新数据库为已完成，实际加水量为0
                        string sql = $@"
                    UPDATE {DROP_HEAD.TableName}
                    SET {DROP_HEAD.AddWaterFinish} = 1,
                        {DROP_HEAD.RealAddWaterWeight} = 0
                    WHERE {DROP_HEAD.BatchName} = @BatchName
                        AND {DROP_HEAD.CupNum} = @CupNo";
                        My_DataBase.SqlServer.ExecuteNonQuery(sql,
                            new SqlParameter("@BatchName", batchNo),
                            new SqlParameter("@CupNo", cupNo));
                        continue;
                    }

                    // 记录需要加水的杯号
                    result.FailCupNo.Add(cupNo);

                    ////根据当前的索引判断是否小于等于倒数第二个滴液，如果是，则启动CheckAndReadBalanceAsync
                    //if (i >= dt.Rows.Count - 2 && balanceTask == null && My_ConPar.Choices.IsDebug == 0)
                    //{
                    //    balanceTask = My_Tool.BalanceStableReading.CheckAndReadBalanceAsync();
                    //}

                    // 调用加水原子任务
                    var singleResult = await EnqueueSingleCupAddWaterAsync(batchNo, cupNo, targetAddWeight);
                    switch (singleResult.Code)
                    {
                        case My_Tool.Result.ResultCode.Exception:
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Exception = singleResult.Exception;
                            result.Message = $"杯号{cupNo}加水异常，原因：" + singleResult.Message;
                            return result;
                        case My_Tool.Result.ResultCode.Canceled:
                            result.Code = My_Tool.Result.ResultCode.Canceled;
                            result.Message = "任务被取消";
                            return result;
                        default:
                            break;
                    }
                }

                if (result.FailCupNo.Count == 0)
                {
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "加水已完成,不需要加水";
                    return result;
                }

                if (My_ConPar.Choices.IsDebug == 1)
                {
                    // 调试模式，直接返回成功
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "调试模式，直接返回成功";
                    return result;
                }
                else
                {
                    // 新增：如果WaterRecheck为1，直接批量写入完成，无需复检
                    if (SmartColor.My_ConPar.Choices.WaterRecheck == 1)
                    {
                        var caseWhens = new StringBuilder();
                        var inParams = new StringBuilder();
                        var paramList = new List<SqlParameter>();
                        string batchName = dt.Rows[0][DROP_HEAD.BatchName].ToString();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var row = dt.Rows[i];
                            int cupNo = Convert.ToInt32(row[DROP_HEAD.CupNum]);
                            double needWeight = Convert.ToDouble(row[DROP_HEAD.ObjectAddWaterWeight]);
                            if (!result.FailCupNo.Contains(cupNo)) continue;

                            caseWhens.Append($" WHEN {DROP_HEAD.CupNum} = @CupNo{i} THEN @ActualWeight{i}");
                            inParams.Append(inParams.Length == 0 ? $"@CupNo{i}" : $",@CupNo{i}");
                            paramList.Add(new SqlParameter($"@CupNo{i}", cupNo));
                            paramList.Add(new SqlParameter($"@ActualWeight{i}", needWeight));
                        }
                        paramList.Add(new SqlParameter("@BatchName", batchName));

                        // 批量更新加水完成状态和实际加水量
                        string sql = $@"
                        UPDATE {DROP_HEAD.TableName}
                        SET
                            {DROP_HEAD.AddWaterFinish} = 1,
                            {DROP_HEAD.RealAddWaterWeight} = CASE {caseWhens} END
                        WHERE {DROP_HEAD.BatchName} = @BatchName
                            AND {DROP_HEAD.CupNum} IN ({inParams})
                    ";
                        My_DataBase.SqlServer.ExecuteNonQuery(sql, paramList.ToArray());

                        // 批量累加更新杯详情液量
                        var cupWeightCaseWhens = new StringBuilder();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var row = dt.Rows[i];
                            int cupNo = Convert.ToInt32(row[DROP_HEAD.CupNum]);
                            double needWeight = Convert.ToDouble(row[DROP_HEAD.ObjectAddWaterWeight]);
                            if (!result.FailCupNo.Contains(cupNo)) continue;

                            cupWeightCaseWhens.Append($" WHEN {CUP_DETAILS.CupNum} = @CupNo{i} THEN {CUP_DETAILS.CurrentWeight} + @ActualWeight{i}");
                        }
                        string cupSql = $@"
                        UPDATE {CUP_DETAILS.TableName}
                        SET
                            {CUP_DETAILS.CurrentWeight} = CASE {cupWeightCaseWhens} END
                        WHERE {CUP_DETAILS.CupNum} IN ({inParams})
                    ";
                        var cupParamList = paramList.Select(p => new SqlParameter(p.ParameterName, p.Value)).ToArray();
                        My_DataBase.SqlServer.ExecuteNonQuery(cupSql, cupParamList);

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var row = dt.Rows[i];
                            int cupNo = Convert.ToInt32(row[DROP_HEAD.CupNum]);
                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                            if (area != null)
                            {
                                area.OnCupDataReceived(cupNo);
                            }
                        }

                        result.Code = My_Tool.Result.ResultCode.Success;
                        result.Message = "加水完成（跳过复检）";
                        return result;
                    }
                    else
                    {
                        // 2. 若有需要加水的杯号，进行加水验证
                        if (result.FailCupNo.Count > 0)
                        {
                            //// 等待天平检查和读数完成
                            //var balanceResult = await balanceTask;
                            //if (balanceResult == 9999.99)
                            //{
                            //    result.Code = My_Tool.Result.ResultCode.Exception;
                            //    result.Message = "天平异常";
                            //    result.Exception = new Exception("天平异常");
                            //    return result;
                            //}

                            //加水验证
                            var waterVerifyResult = await SmartColor.My_AutomaticModule.WaterCorrectionRobotTask.EnqueueWaterVerifyAsync(true, 9999, 0, 0, BigProcess.RobotBusinessType.DropProcess, BigProcess.DropProcess);
                            switch (waterVerifyResult.Code)
                            {
                                case My_Tool.Result.ResultCode.Exception:
                                    result.Code = My_Tool.Result.ResultCode.Exception;
                                    result.Exception = waterVerifyResult.Exception;
                                    result.Message = "加水验证异常，原因：" + waterVerifyResult.Message;
                                    return result;
                                case My_Tool.Result.ResultCode.Canceled:
                                    result.Code = My_Tool.Result.ResultCode.Canceled;
                                    result.Message = "任务被取消";
                                    return result;
                                default:
                                    break;
                            }

                            // 3. 批量更新加水完成状态、实际加水量和杯详情液量
                            var caseWhens = new StringBuilder();
                            var inParams = new StringBuilder();
                            var paramList = new List<SqlParameter>();
                            var cupWeightCaseWhens = new StringBuilder();
                            string batchName = dt.Rows[0][DROP_HEAD.BatchName].ToString();
                            int redit = My_Tool.BalanceStableReading.RetainDecimals();
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var row = dt.Rows[i];
                                int cupNo = Convert.ToInt32(row[DROP_HEAD.CupNum]);
                                double needWeight = Convert.ToDouble(row[DROP_HEAD.ObjectAddWaterWeight]);

                                double errorPercent = waterVerifyResult.ErrorPercent ?? 0;
                                double realAddWeight = waterVerifyResult.RealAddWeight ?? 0;
                                double recheckWeight = waterVerifyResult.RecheckWeight ?? 0;

                                double actualWeight;
                                if (My_ConPar.Choices.CorrectData == 1 && Math.Abs(errorPercent) > SmartColor.My_ConPar.Other.AErr_Water && recheckWeight > 0)
                                {
                                    // 启用修正，误差超限且复检重量大于0，直接写需加重量
                                    actualWeight = needWeight;
                                }
                                else if (needWeight >= 20)
                                    actualWeight = Math.Round(needWeight * (1 + errorPercent / 100.0), redit);
                                else if (needWeight > 0)
                                    actualWeight = Math.Round(needWeight + (realAddWeight - recheckWeight), redit);
                                else
                                    actualWeight = 0;

                                caseWhens.Append($" WHEN {DROP_HEAD.CupNum} = @CupNo{i} THEN @ActualWeight{i}");
                                cupWeightCaseWhens.Append($" WHEN {CUP_DETAILS.CupNum} = @CupNo{i} THEN {CUP_DETAILS.CurrentWeight} + @ActualWeight{i}");
                                inParams.Append(i == 0 ? $"@CupNo{i}" : $",@CupNo{i}");
                                paramList.Add(new SqlParameter($"@CupNo{i}", cupNo));
                                paramList.Add(new SqlParameter($"@ActualWeight{i}", actualWeight));
                            }
                            paramList.Add(new SqlParameter("@BatchName", batchName));

                            // 批量更新加水完成状态和实际加水量
                            string sql = $@"
                            UPDATE {DROP_HEAD.TableName}
                            SET
                                {DROP_HEAD.AddWaterFinish} = 1,
                                {DROP_HEAD.RealAddWaterWeight} = CASE {caseWhens} END
                            WHERE {DROP_HEAD.BatchName} = @BatchName
                                AND {DROP_HEAD.CupNum} IN ({inParams})
                        ";
                            My_DataBase.SqlServer.ExecuteNonQuery(sql, paramList.ToArray());

                            // 批量累加更新杯详情液量
                            string cupSql = $@"
                            UPDATE {CUP_DETAILS.TableName}
                            SET
                                {CUP_DETAILS.CurrentWeight} = CASE {cupWeightCaseWhens} END
                            WHERE {CUP_DETAILS.CupNum} IN ({inParams})
                        ";
                            var cupParamList = paramList.Select(p => new SqlParameter(p.ParameterName, p.Value)).ToArray();
                            My_DataBase.SqlServer.ExecuteNonQuery(cupSql, cupParamList);

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var row = dt.Rows[i];
                                int cupNo = Convert.ToInt32(row[DROP_HEAD.CupNum]);
                                var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                                if (area != null)
                                {
                                    area.OnCupDataReceived(cupNo);
                                }
                            }

                            if (waterVerifyResult.Code == My_Tool.Result.ResultCode.Failure)
                            {
                                result.Code = My_Tool.Result.ResultCode.Failure;
                                result.Message = "加水验证失败";
                                return result;
                            }
                        }
                    }

                    // 4. 全部加水流程完成
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "验证完成";
                    return result;
                }
            }
            catch (TaskCanceledException)
            {
                result.Code = My_Tool.Result.ResultCode.Canceled;
                result.Message = "任务被取消";
                return result;
            }
            catch (Exception ex)
            {
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = "代码异常：" + ex.Message;
                result.Exception = ex;
                return result;
            }
        }

        /// <summary>
        /// 单杯滴液加染料/助剂流程
        /// </summary>
        /// <param name="batchNo">批次号</param>
        /// <param name="cupNo">最小杯号</param>
        /// <param name="CurrentAdditionType">%或g/l</param>
        /// <param name="checkLow">检查母液量不足</param>
        /// <param name="checkExpired">检查母液过期</param>
        /// <returns>完成， 异常  任务取消</returns>
        private static async Task<DropProcessResult> SingleCupDropProcess(string batchNo, int cupNo, string CurrentAdditionType, bool checkLow, bool checkExpired)
        {
            DropProcessResult result = new DropProcessResult();
            try
            {
                //根据最小杯号找到染料/助剂的瓶号，按瓶号从小到大排序
                var dt = My_DataBase.SqlServer.Select(
                    DROP_DETAILS.TableName,
                    null,
                    $"{DROP_DETAILS.CupNum} = @cupNo AND {DROP_DETAILS.UnitOfAccount} = @type AND {DROP_DETAILS.Finish} = 0 AND {DROP_DETAILS.BatchName} = @batchNo",
                    $"{DROP_DETAILS.BottleNum}", true,
                    new SqlParameter("@cupNo", cupNo),
                    new SqlParameter("@type", CurrentAdditionType),
                    new SqlParameter("@batchNo", batchNo)
                    );

                if (dt == null || dt.Rows.Count == 0)
                {
                    await DropFinishAsync(batchNo, cupNo);

                    result.Code = My_Tool.Result.ResultCode.Success;
                    return result;
                }

                List<RemainingPulses> remainingPulses = null;

                //遍历瓶号，执行滴液流程
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //获取瓶号
                    var row = dt.Rows[i];
                    int bottleNo = Convert.ToInt32(row[DROP_DETAILS.BottleNum]);
                    var bottleRow = BottleData.Bottle_details.Select($"{BOTTLE_DETAILS.BottleNum} = {bottleNo}").FirstOrDefault();
                    if (bottleRow == null)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = "查找资料异常";
                        result.Exception = new Exception($"瓶号{bottleNo}母液瓶资料不存在");
                        return result;
                    }

                    // 检查母液瓶当前重量是否足够
                    if (checkLow)
                    {
                        if (My_ConPar.Choices.DripCheckLow == 1)
                        {
                            double cw = Convert.ToDouble(bottleRow[BOTTLE_DETAILS.CurrentWeight] ?? "0");
                            if (cw <= My_ConPar.Other.Bottle_MinWeight)
                            {
                                //液量不足
                                if (!result.BottleWeightLow.Contains(bottleNo))
                                {
                                    result.BottleWeightLow.Add(bottleNo);
                                }
                                continue;
                            }
                        }
                    }

                    //检查母液瓶是否过期
                    if (checkExpired)
                    {
                        if (My_ConPar.Choices.DripCheckExpired == 1)
                        {
                            DateTime expireDate = Convert.ToDateTime(bottleRow[BOTTLE_DETAILS.BrewingData] ?? DateTime.MinValue.ToString());
                            string assistantCode = bottleRow[BOTTLE_DETAILS.AssistantCode]?.ToString() ?? "";
                            var assistantInfo = My_DataBase.AssistantData.Assistant_details.Select($"{ASSISTANT_DETAILS.AssistantCode} = '{assistantCode}'").FirstOrDefault();
                            if (assistantInfo == null)
                            {
                                result.Code = My_Tool.Result.ResultCode.Exception;
                                result.Message = "查找资料异常";
                                result.Exception = new Exception($"{bottleNo}号母液瓶染助剂资料不存在");
                                return result;
                            }
                            int termOfValidity = Convert.ToInt32(assistantInfo[ASSISTANT_DETAILS.TermOfValidity] ?? "0");

                            if (expireDate.AddHours(termOfValidity) < DateTime.Now)
                            {
                                //母液过期
                                if (!result.BottleOutdated.Contains(bottleNo))
                                {
                                    result.BottleOutdated.Add(bottleNo);
                                }
                                continue;
                            }

                        }
                    }


                    // 检查母液瓶校正状态，如失败则重新校正
                    int adjustSuccess = Convert.ToInt32(bottleRow[BOTTLE_DETAILS.AdjustSuccess] ?? "0");
                    if (adjustSuccess == 0)
                    {
                        // 母液瓶重新校正，支持重试


                        List<int> retryBottleNos = new List<int> { bottleNo };
                        while (true)
                        {
                            var adjustResult = await SmartColor.My_AutomaticModule.BottleCorrectionRobotTask.EnqueueBatchBottleCorrectionAsync(retryBottleNos, BigProcess.RobotBusinessType.DropProcess, BigProcess.DropProcess);
                            var resultCode = adjustResult[bottleNo].Code;
                            if (resultCode == My_Tool.Result.ResultCode.Success)
                            {
                                // 校正成功，跳出循环
                                break;
                            }
                            else if (resultCode == My_Tool.Result.ResultCode.Failure)
                            {
                                // 弹窗询问是否重试
                                var btn = await My_Tool.MessageEventManager.Instance.RequestShowMessageAsync(
                                    "校正失败",
                                    adjustResult[bottleNo].Message,
                                    new[] { "重试", "跳过" },
                                    "重试"
                                );
                                if (btn == "跳过")
                                {
                                    break;
                                }
                                // 否则继续重试
                            }
                            else if ((int)resultCode < 0)
                            {
                                // 异常或取消
                                Logger.Error(
                                     $"瓶号{bottleNo}校正异常",
                                     adjustResult[bottleNo].Exception
                                 );
                                result.Code = My_Tool.Result.ResultCode.Exception;
                                result.Message = $"瓶号{bottleNo}校正异常：{adjustResult[bottleNo].Message}";
                                result.Exception = new Exception(result.Message);
                                return result;
                            }
                        }
                    }

                //执行单瓶滴液流程
                label1:
                    var singleDropResult = await EnqueueBottleDropAsync(bottleNo, batchNo.ToString(), remainingPulses);
                    switch (singleDropResult.Code)
                    {
                        case My_Tool.Result.ResultCode.Exception:
                            {
                                //异常
                                result.Code = My_Tool.Result.ResultCode.Exception;
                                result.Message = result.Message;
                                result.Exception = result.Exception;
                                return result;
                            }
                        case My_Tool.Result.ResultCode.Canceled:
                            {
                                //取消任务
                                result.Code = My_Tool.Result.ResultCode.Canceled;
                                result.Message = "取消任务";
                                return result;

                            }

                        default:
                            {
                                if (singleDropResult.FailCupNo != null && singleDropResult.FailCupNo.Count > 0)
                                {
                                    //把失败的瓶号记录下来
                                    result.FailCupNo.AddRange(singleDropResult.FailCupNo);
                                    result.FailCupNo = result.FailCupNo.Distinct().ToList();
                                }

                                if (singleDropResult.Finish)
                                {
                                    //当前瓶滴液完成，继续下一个瓶号滴液
                                    remainingPulses = null;
                                    break;
                                }

                                else
                                {
                                    //部分滴液未完成，赋值剩余脉冲， 跳转继续滴液
                                    remainingPulses = singleDropResult.DropData;
                                    await WaitTaskRemovedAsync(bottleNo);
                                    goto label1;
                                }
                            }
                    }

                }


                result.Code = My_Tool.Result.ResultCode.Success;
                return result;
            }
            catch (TaskCanceledException)
            {
                result.Code = My_Tool.Result.ResultCode.Canceled;
                result.Message = "任务被取消";
                return result;
            }
            catch (Exception ex)
            {
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = "代码异常：" + ex.Message;
                result.Exception = ex;
                return result;
            }


        }

        private static async Task WaitTaskRemovedAsync(int bottleNo)
        {
            // 最多等待30秒，防止死等
            var timeout = TimeSpan.FromSeconds(30);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.Elapsed < timeout)
            {
                await Task.Delay(200); // 200ms 轮询一次
                // 获取当前任务中心所有任务快照
                var allTasks = SmartColor.My_RobotManager.RobotTaskManager.Instance.GetAllTasksSnapshot();
                bool exists = false;
                foreach (var t in allTasks)
                {
                    try
                    {
                        dynamic dt = t;
                        // 这里根据你的任务命名或业务类型判断是否是同一个任务
                        // 例如：TaskName 包含 batchNo 和 bottleNo
                        if (dt.TaskName != null &&
                            dt.TaskName.ToString() == $"{bottleNo}号母液瓶滴液" &&
                            dt.BusinessType == SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DropProcess)
                        {
                            exists = true;
                            break;
                        }
                    }
                    catch { }
                }
                if (!exists)
                    break;

            }
        }

        /// <summary>
        /// 判断当前杯是否完成（异步）
        /// </summary>
        /// <param name="batchNo">批次号</param>
        /// <param name="cupNo">杯号</param>
        private static async Task DropFinishAsync(string batchNo, int cupNo)
        {
            try
            {
                // 1. 一次查出表头所有需要的字段
                var headTable = My_DataBase.SqlServer.Select(DROP_HEAD.TableName,
                    $"{DROP_HEAD.CupNum} = @cupNo AND {DROP_HEAD.BatchName} = @batchNo",
                    new SqlParameter("@cupNo", cupNo),
                    new SqlParameter("@batchNo", batchNo));
                if (headTable == null || headTable.Rows.Count == 0)
                    return;

                var headRow = headTable.Rows[0];
                // 判断加水是否完成
                if (headRow.Table.Columns.Contains(DROP_HEAD.AddWaterFinish) && Convert.ToInt32(headRow[DROP_HEAD.AddWaterFinish]) == 0)
                    return;

                // 读取目标加水量和实际加水量
                double targetAddWater = headRow.Table.Columns.Contains(DROP_HEAD.ObjectAddWaterWeight) && headRow[DROP_HEAD.ObjectAddWaterWeight] != DBNull.Value
                    ? Convert.ToDouble(headRow[DROP_HEAD.ObjectAddWaterWeight]) : 0;
                double realAddWater = headRow.Table.Columns.Contains(DROP_HEAD.RealAddWaterWeight) && headRow[DROP_HEAD.RealAddWaterWeight] != DBNull.Value
                    ? Convert.ToDouble(headRow[DROP_HEAD.RealAddWaterWeight]) : 0;

                // 2. 一次查出所有详情
                var detailsTable = My_DataBase.SqlServer.Select(DROP_DETAILS.TableName,
                    $"{DROP_DETAILS.CupNum} = @cupNo AND {DROP_DETAILS.BatchName} = @batchNo",
                    new SqlParameter("@cupNo", cupNo),
                    new SqlParameter("@batchNo", batchNo));
                //if (detailsTable == null || detailsTable.Rows.Count == 0)
                //    return;

                // 判断是否全部完成
                foreach (DataRow row in detailsTable.Rows)
                {
                    if (row.Table.Columns.Contains(DROP_DETAILS.Finish) && Convert.ToInt32(row[DROP_DETAILS.Finish]) == 0)
                        return; // 有未完成的，直接返回
                }

                // 误差判断
                int dig = My_Tool.BalanceStableReading.RetainDecimals();
                bool dropFail = false;
                foreach (DataRow row in detailsTable.Rows)
                {
                    double allowErr = My_Tool.BottleAuxiliary.GetAllowErr(row.Table.Columns.Contains(DROP_DETAILS.BottleNum) && row[DROP_DETAILS.BottleNum] != DBNull.Value
                        ? Convert.ToInt16(row[DROP_DETAILS.BottleNum]) : 0);
                    double targetDrop = row.Table.Columns.Contains(DROP_DETAILS.ObjectDropWeight) && row[DROP_DETAILS.ObjectDropWeight] != DBNull.Value
                        ? Convert.ToDouble(row[DROP_DETAILS.ObjectDropWeight]) : 0;
                    double realDrop = row.Table.Columns.Contains(DROP_DETAILS.RealDropWeight) && row[DROP_DETAILS.RealDropWeight] != DBNull.Value
                        ? Convert.ToDouble(row[DROP_DETAILS.RealDropWeight]) : 0;
                    var realErr = Math.Round(Math.Abs(targetDrop - realDrop), dig);
                    if (realErr > allowErr)
                    {
                        dropFail = true;
                        break;
                    }
                }



                // 判断加水误差
                bool waterFail = false;
                if (targetAddWater > 0)
                {
                    double percent = Math.Abs(targetAddWater - realAddWater) / targetAddWater * 100;
                    if (Math.Round(percent, 2) > Math.Round(SmartColor.My_ConPar.Other.AErr_Water, 2))
                        waterFail = true;
                }



                // 3. 查询当前杯类型
                var cupRows = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {cupNo}");
                if (cupRows.Rows.Count == 0)
                {
                    My_File.Logger.Error($"DropFinishAsync: 未找到{cupNo}杯的cup_details数据");
                    return;
                }
                var cupRow = cupRows.Rows[0];
                int type = cupRow.Table.Columns.Contains(CUP_DETAILS.Type) && cupRow[CUP_DETAILS.Type] != DBNull.Value
                    ? Convert.ToInt32(cupRow[CUP_DETAILS.Type])
                    : 0;


                int retainDecimals = My_Tool.BalanceStableReading.RetainDecimals();
                string format = $"F{retainDecimals}";
                string desc;
                if (dropFail || waterFail)
                {

                    desc = $"滴液失败，目标加水量：{targetAddWater.ToString(format)},实际加水量：{realAddWater.ToString(format)}";
                    SqlServer.Update(CUP_DETAILS.TableName,
                        new Dictionary<string, object>()
                        {
                                { CUP_DETAILS.Fail, 1 }
                        },
                        $"{CUP_DETAILS.CupNum} = @cupNo",
                        new SqlParameter("@cupNo", cupNo));
                    var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                    if (area != null)
                    {
                        area.OnCupDataReceived(cupNo);
                    }
                }
                else
                {
                    desc = $"滴液成功，目标加水量：{targetAddWater.ToString(format)},实际加水量：{realAddWater.ToString(format)}";
                }
                // 4. 完成状态更新
                My_DataBase.SqlServer.Update(DROP_HEAD.TableName,
                    new Dictionary<string, object>()
                    {
                            {DROP_HEAD.DescribeChar, desc },
                            {DROP_HEAD.FinishTime , DateTime.Now },
                            {DROP_HEAD.CupFinish, 1 }
                    },
                    $"{DROP_HEAD.CupNum} = @cupNo AND {DROP_HEAD.BatchName} = @batchNo",
                    new SqlParameter("@cupNo", cupNo),
                    new SqlParameter("@batchNo", batchNo));



                // 5. 根据类型处理
                if (type == 2) // 滴液
                {
                    // 归档滴液表头和详情
                    int headId = cupRow.Table.Columns.Contains(CUP_DETAILS.HeadID) && cupRow[CUP_DETAILS.HeadID] != DBNull.Value
                        ? Convert.ToInt32(cupRow[CUP_DETAILS.HeadID])
                        : 0;
                    if (headId == 0)
                    {
                        My_File.Logger.Error($"DropFinishAsync: 未找到{cupNo}杯的HeadID，无法归档");
                        return;
                    }

                    // 获取DROP_HEAD和DROP_DETAILS数据
                    var headRows = SqlServer.Select(DROP_HEAD.TableName, $"{DROP_HEAD.MyID} = {headId}");
                    var detailsRows = SqlServer.Select(DROP_DETAILS.TableName, $"{DROP_DETAILS.HeadID} = {headId}");

                    // 3. 迁移DROP_HEAD到HISTORY_HEAD（字段过滤）
                    var historyHeadDict = new Dictionary<string, object>();
                    var historyHeadFields = TableDefinition.TableSchemas[HISTORY_HEAD.TableName].Select(f => f.Name).ToHashSet();
                    foreach (DataColumn col in headRow.Table.Columns)
                    {
                        if (historyHeadFields.Contains(col.ColumnName) && col.ColumnName != HISTORY_HEAD.MyID)
                            historyHeadDict[col.ColumnName] = headRow[col.ColumnName];
                    }


                    historyHeadDict[HISTORY_HEAD.FinishTime] = DateTime.Now;
                    historyHeadDict[HISTORY_HEAD.HeadID] = headId;
                    historyHeadDict[HISTORY_HEAD.DescribeChar] = desc;
                    SqlServer.Insert(HISTORY_HEAD.TableName, historyHeadDict);

                    // 4. 迁移DROP_DETAILS到HISTORY_DETAILS（字段过滤）
                    var historyDetailsList = new List<Dictionary<string, object>>();
                    var historyDetailsFields = TableDefinition.TableSchemas[HISTORY_DETAILS.TableName].Select(f => f.Name).ToHashSet();

                    foreach (DataRow row in detailsRows.Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var field in historyDetailsFields)
                        {
                            if (row.Table.Columns.Contains(field))
                                dict[field] = row[field];
                        }
                        historyDetailsList.Add(dict);
                    }
                    if (historyDetailsList.Count > 0)
                        SqlServer.Insert(HISTORY_DETAILS.TableName, historyDetailsList);

                    // 删除原表数据
                    My_DataBase.SqlServer.Delete(DROP_HEAD.TableName, $"{DROP_HEAD.MyID} = @HeadID", new SqlParameter("@HeadID", headId));
                    My_DataBase.SqlServer.Delete(DROP_DETAILS.TableName, $"{DROP_DETAILS.HeadID} = @HeadID", new SqlParameter("@HeadID", headId));

                    // 重置cup_details
                    var updateDict = new Dictionary<string, object>
                        {
                            { CUP_DETAILS.Statues, "" },
                            { CUP_DETAILS.Enable,1 },
                            { CUP_DETAILS.IsUsing,0 },
                            { CUP_DETAILS.CurrentWeight, 0 },
                            { CUP_DETAILS.TotalWeight,0 },
                            { CUP_DETAILS.SetTime,0 },
                            { CUP_DETAILS.DyeType, DBNull.Value },
                            { CUP_DETAILS.StartTime, DBNull.Value },
                            { CUP_DETAILS.StepStartTime, DBNull.Value },
                            { CUP_DETAILS.FormulaCode, DBNull.Value },
                            { CUP_DETAILS.DyeingCode, DBNull.Value },
                            { CUP_DETAILS.StepNum, 0 },
                            { CUP_DETAILS.TotalStep, 0 },
                            { CUP_DETAILS.TechnologyName, DBNull.Value },
                            { CUP_DETAILS.SetTemp, DBNull.Value },
                            { CUP_DETAILS.RecordIndex, 0 },
                            { CUP_DETAILS.Cooperate, 0 },
                            { CUP_DETAILS.Fail, 0 },
                            { CUP_DETAILS.HeadID, DBNull.Value },
                            { CUP_DETAILS.CurrentStepFinish,0 }
                        };
                    My_DataBase.SqlServer.Update(CUP_DETAILS.TableName, updateDict, $"{CUP_DETAILS.CupNum} = @CupNum", new SqlParameter("@CupNum", cupNo));

                    var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                    if (area != null)
                    {
                        area.OnCupDataReceived(cupNo);
                    }
                    
                    My_Tool.CupAuxiliary.UpdateWaitList(cupNo, true);
                }
                else if (type == 3) // 染固色
                {
                    // 判断主副杯是否都完成
                    var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);

                    // 判断主副杯是否都在用
                    bool mainUsed = mainInfo.IsUsing == 1;
                    bool subUsed = subInfo.IsUsing == 1;

                    // 当前杯是主杯还是副杯
                    int mainCupNum = mainInfo.CupNum;
                    int subCupNum = subInfo.CupNum;

                    // 检查主副杯完成状态
                    bool mainFinish = false, subFinish = false;
                    if (mainUsed)
                    {
                        var mainHead = My_DataBase.SqlServer.Select(DROP_HEAD.TableName,
                            $"{DROP_HEAD.CupNum} = @cupNo AND {DROP_HEAD.CupFinish} = 1 ",
                            new SqlParameter("@cupNo", mainCupNum),
                            new SqlParameter("@batchNo", batchNo));
                        mainFinish = mainHead != null && mainHead.Rows.Count > 0;
                    }
                    if (subUsed)
                    {
                        var subHead = My_DataBase.SqlServer.Select(DROP_HEAD.TableName,
                            $"{DROP_HEAD.CupNum} = @cupNo AND {DROP_HEAD.CupFinish} = 1 ",
                            new SqlParameter("@cupNo", subCupNum),
                            new SqlParameter("@batchNo", batchNo));
                        subFinish = subHead != null && subHead.Rows.Count > 0;
                    }

                    // 查询主副杯的Fail状态
                    var mainFail = mainInfo.Fail == 1;
                    var subFail = subInfo.Fail == 1;


                    // 只用一个杯，判断等待列表是否有另一杯的，如果有则等待另一杯完成
                    if ((mainUsed && !subUsed) || (!mainUsed && subUsed))
                    {
                        CupFinished?.Invoke(cupNo);
                        int usedCupNum = mainUsed ? mainCupNum : subCupNum;

                        bool usedFail = mainUsed ? mainFail : subFail;
                        var otherCup = mainUsed ? subInfo : mainInfo;
                        if (!usedFail)
                        {
                            if (otherCup.Enable == 1)
                            {
                                var headDT = SqlServer.Select(DROP_HEAD.TableName,
                                  $"{DROP_HEAD.CupNum} = {otherCup.CupNum} AND {DROP_HEAD.CupFinish} = 0");

                                if (headDT != null && headDT.Rows.Count > 0)
                                {
                                    var otherHeadDT = headDT.Rows[0][DROP_HEAD.MyID];
                                    List<int> selectedIds = new List<int> { Convert.ToInt32(otherHeadDT) };
                                    SmartColor.My_AutomaticModule.DropBatchManager.RequestBatchStartByIds(selectedIds);
                                    return;

                                }
                                else
                                {
                                    var dt = SqlServer.Select(WAIT_LIST.TableName,
                                     null,
                                     $"{WAIT_LIST.CupNum} = {otherCup.CupNum}",
                                     WAIT_LIST.IndexNum,
                                     true
                                 );

                                    if (dt.Rows.Count > 0)
                                    {
                                        bool foundSameDyeingCode = false;
                                        string currentDyeingCode = headRow.Table.Columns.Contains(DROP_HEAD.DyeingCode) && headRow[DROP_HEAD.DyeingCode] != DBNull.Value
                                            ? headRow[DROP_HEAD.DyeingCode].ToString()
                                            : null;

                                        foreach (DataRow waitRow in dt.Rows)
                                        {
                                            string formulaCode = waitRow.Table.Columns.Contains(WAIT_LIST.FormulaCode) && waitRow[WAIT_LIST.FormulaCode] != DBNull.Value
                                                ? waitRow[WAIT_LIST.FormulaCode].ToString()
                                                : null;
                                            string versionNum = waitRow.Table.Columns.Contains(WAIT_LIST.VersionNum) && waitRow[WAIT_LIST.VersionNum] != DBNull.Value
                                                ? waitRow[WAIT_LIST.VersionNum].ToString()
                                                : null;

                                            // 先查FORMULA_HEAD表
                                            var formulaRows = SqlServer.Select(FORMULA_HEAD.TableName,
                                                $"{FORMULA_HEAD.FormulaCode} = @FormulaCode AND {FORMULA_HEAD.VersionNum} = @VersionNum",
                                                new SqlParameter("@FormulaCode", formulaCode),
                                                new SqlParameter("@VersionNum", versionNum)
                                            );

                                            string dyeingCode = null;
                                            if (formulaRows.Rows.Count > 0)
                                            {
                                                var dyeingCodeObj = formulaRows.Rows[0][FORMULA_HEAD.DyeingCode];
                                                dyeingCode = dyeingCodeObj != DBNull.Value ? dyeingCodeObj.ToString() : null;
                                            }
                                            else
                                            {
                                                // FORMULA_HEAD没找到，查HISTORY_HEAD
                                                var historyRows = SqlServer.Select(HISTORY_HEAD.TableName,
                                                    $"{HISTORY_HEAD.FormulaCode} = @FormulaCode AND {HISTORY_HEAD.VersionNum} = @VersionNum",
                                                    new SqlParameter("@FormulaCode", formulaCode),
                                                    new SqlParameter("@VersionNum", versionNum)
                                                );
                                                if (historyRows.Rows.Count > 0)
                                                {
                                                    var dyeingCodeObj = historyRows.Rows[0][HISTORY_HEAD.DyeingCode];
                                                    dyeingCode = dyeingCodeObj != DBNull.Value ? dyeingCodeObj.ToString() : null;
                                                }
                                            }

                                            if (!string.IsNullOrEmpty(dyeingCode) && dyeingCode == currentDyeingCode)
                                            {
                                                foundSameDyeingCode = true;
                                                break;
                                            }
                                        }

                                        if (foundSameDyeingCode)
                                        {
                                            SmartColor.My_Tool.CupAuxiliary.HandleCupFinished(otherCup.CupNum, true);
                                            return;
                                        }
                                        else
                                        {
                                            var cupSelect = My_Tool.CupAuxiliary.GetCupChioce(cupNo);
                                            await CupCommManager.Instance.RequestDyeingAsync(cupNo, cupSelect);
                                            return;
                                        }
                                    }


                                }
                            }


                            var headDt = SqlServer.Select(DROP_HEAD.TableName,
                                $"{DROP_HEAD.CupNum} = {usedCupNum} AND {DROP_HEAD.CupFinish} = 1");
                            if (headDt.Rows != null && headDt.Rows.Count > 0)
                            {
                                var cupSelect = My_Tool.CupAuxiliary.GetCupChioce(cupNo);
                                await CupCommManager.Instance.RequestDyeingAsync(cupNo, cupSelect);
                            }

                            return;
                        }

                        // 有失败则不发送
                    }
                    // 两个杯都用，且都完成，才发（需判断Fail）
                    else if (mainUsed && subUsed)
                    {
                        CupFinished?.Invoke(cupNo);
                        if (mainFinish && subFinish && !mainFail && !subFail)
                        {
                            if (!_pendingDyeingCups.Contains(cupNo))
                            {
                                int other = mainCupNum == cupNo ? subCupNum : mainCupNum;
                                _pendingDyeingCups.Add(other);
                                var cupSelect = My_Tool.CupAuxiliary.GetCupChioce(cupNo);
                                await CupCommManager.Instance.RequestDyeingAsync(cupNo, cupSelect);
                            }
                        }
                        // 有失败则不发送
                    }
                    // 其他情况（异常），可根据需要补充日志
                }

                // 其他类型不做处理
                return;

            }
            catch (Exception ex)
            {
                My_File.Logger.Error("DropFinishAsync异常", ex);
            }
        }

        /// <summary>
        /// 重置数据库批次信息
        /// </summary>
        /// <param name="batchNo">批次号</param>
        /// <param name="cupNoArray">错误杯号列表</param>
        private static async Task ResetCupState(string batchNo, List<int> cupNoArray)
        {
            //重置表头数据
            var inParams = string.Join(",", cupNoArray);
            My_DataBase.SqlServer.Update(DROP_HEAD.TableName,
                new Dictionary<string, object>()
                {
                    {DROP_HEAD.AddWaterFinish, 0 },
                    {DROP_HEAD.RealAddWaterWeight,0 },
                    {DROP_HEAD.StartTime,null },
                    {DROP_HEAD.FinishTime,null },
                    {DROP_HEAD.CupFinish,0 }
                },
                $"{DROP_HEAD.BatchName} = @batchNO AND {DROP_HEAD.CupNum} IN ({inParams})",
                new SqlParameter("@batchNO", batchNo),
                new SqlParameter("@par", inParams));

            //重置表详情数据
            My_DataBase.SqlServer.Update(DROP_DETAILS.TableName,
               new Dictionary<string, object>()
               {
                   {DROP_DETAILS.RealDropWeight, 0 },
                   {DROP_DETAILS.Finish,0 },
                   {DROP_DETAILS.RealPowderWeight,0 },

               },
               $"{DROP_HEAD.BatchName} = @batchNO AND {DROP_HEAD.CupNum} IN ({inParams})",
               new SqlParameter("@batchNO", batchNo),
               new SqlParameter("@par", inParams));

            List<int> sendCup = new List<int>();
            // 新增：类型为3的杯，发送HMI停止信号
            foreach (var cupNo in cupNoArray)
            {
                // 查询杯类型
                var cupRows = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {cupNo}");
                if (cupRows.Rows.Count > 0)
                {
                    var cupRow = cupRows.Rows[0];
                    int type = cupRow.Table.Columns.Contains(CUP_DETAILS.Type) && cupRow[CUP_DETAILS.Type] != DBNull.Value
                        ? Convert.ToInt32(cupRow[CUP_DETAILS.Type])
                        : 0;
                    if (type == 2)
                    {
                        // 重置cup_details
                        var updateDict = new Dictionary<string, object>
                        {
                            { CUP_DETAILS.Statues, "" },
                            { CUP_DETAILS.Enable,1 },
                            { CUP_DETAILS.IsUsing,0 },
                            { CUP_DETAILS.CurrentWeight, 0 },
                            { CUP_DETAILS.TotalWeight,0 },
                            { CUP_DETAILS.SetTime,0 },
                            { CUP_DETAILS.DyeType, DBNull.Value },
                            { CUP_DETAILS.StartTime, DBNull.Value },
                            { CUP_DETAILS.StepStartTime, DBNull.Value },
                            { CUP_DETAILS.FormulaCode, DBNull.Value },
                            { CUP_DETAILS.DyeingCode, DBNull.Value },
                            { CUP_DETAILS.StepNum, 0 },
                            { CUP_DETAILS.TotalStep, 0 },
                            { CUP_DETAILS.TechnologyName, DBNull.Value },
                            { CUP_DETAILS.SetTemp, DBNull.Value },
                            { CUP_DETAILS.RecordIndex, 0 },
                            { CUP_DETAILS.Cooperate, 0 },
                            { CUP_DETAILS.Fail, 0 },
                            { CUP_DETAILS.HeadID, DBNull.Value },
                            { CUP_DETAILS.CurrentStepFinish,0 }
                        };
                        My_DataBase.SqlServer.Update(CUP_DETAILS.TableName, updateDict, $"{CUP_DETAILS.CupNum} = @CupNum", new SqlParameter("@CupNum", cupNo));
                        var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                        if (area != null)
                        {
                            area.OnCupDataReceived(cupNo);
                        }

                    }
                    else if (type == 3)
                    {
                        // 查找区域和通讯对象
                        if (sendCup.Contains(cupNo))
                        {
                            continue;
                        }
                        else
                        {
                            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetCupPair(cupNo);
                            if (mainCup != subCup)
                            {
                                sendCup.Add(mainCup);
                                sendCup.Add(subCup);
                            }
                            else
                            {
                                sendCup.Add(cupNo);
                            }
                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                            if (area != null)
                            {
                                var comm = CupCommManager.Instance.GetCommObject(area);
                                if (comm != null)
                                {

                                    // 异步发送停止信号
                                    await comm.SendStopAsync(cupNo, false);

                                }
                            }
                        }
                    }

                }
            }

        }


        /// <summary>
        /// 滴液流程
        /// </summary>
        /// <param name="batchNo">批次号</param>
        /// <returns></returns>
        public static async Task<DropResult> EnqueueBatchDropAsync(string batchNo)
        {
            DropResult result = new DropResult();
            IsStopped = false;
            try
            {
                var dt = DateTime.Now;
                _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                {
                    [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"{batchNo}批次开始滴液"
                }, dt);
            label3:
                int roundDigits = My_Tool.BalanceStableReading.RetainDecimals();



                List<int> bottleWeightLow = new List<int>();
                List<int> bottleOutdated = new List<int>();
                List<int> bottleNotFound = new List<int>();
                List<int> failCupNo = new List<int>();

                bool checkLow = true;
                bool checkExpired = true;

            // 1. 查询该批次中的所需要用到的杯号
            label2:
                var batchData = My_DataBase.SqlServer.Select(
                    DROP_HEAD.TableName,
                    null,
                    $"{DROP_HEAD.BatchName} = @BatchName AND {DROP_HEAD.CupFinish} = 0",
                    $"{DROP_HEAD.CupNum}",
                    true,
                    new SqlParameter("@BatchName", batchNo)
                );
                if (batchData == null || batchData.Rows.Count == 0)
                {
                    //未找到批次
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "查找资料异常/逻辑错误";
                    result.Exception = new Exception($"未找到{batchNo}批次");
                    return result;

                }

                //获取杯号列表
                var cupNos = batchData.AsEnumerable().Select(r => Convert.ToInt32(r[DROP_HEAD.CupNum])).ToList();

                //改变杯子详细表
                var sentCupNos = new HashSet<int>();
                List<Task<DyeingResult>> sendTask = new List<Task<DyeingResult>>();
                foreach (DataRow cupData in batchData.Rows)
                {
                    var formulaCode = cupData[DROP_HEAD.FormulaCode]?.ToString();
                    var dyeingCode = cupData[DROP_HEAD.DyeingCode]?.ToString();
                    var cupNo = Convert.ToInt16(cupData[DROP_HEAD.CupNum]);
                    var headId = Convert.ToInt32(cupData[DROP_HEAD.MyID]);
                    var totalWeight = Convert.ToDouble(cupData[DROP_HEAD.TotalWeight] ?? "0");
                    if (string.IsNullOrEmpty(dyeingCode))
                    {
                        int updateResult = SqlServer.Update(CUP_DETAILS.TableName,
                                 new Dictionary<string, object>()
                                 {
                                    { CUP_DETAILS.HeadID,headId },
                                    { CUP_DETAILS.IsUsing,1 },

                                    { CUP_DETAILS.FormulaCode,formulaCode },
                                    { CUP_DETAILS.DyeingCode,dyeingCode },
                                    { CUP_DETAILS.CurrentWeight, 0 },
                                    { CUP_DETAILS.TotalWeight,totalWeight },
                                    { CUP_DETAILS.SetTime,0 },
                                    { CUP_DETAILS.DyeType, DBNull.Value },
                                    { CUP_DETAILS.StartTime, DBNull.Value },
                                    { CUP_DETAILS.StepStartTime, DBNull.Value },
                                    { CUP_DETAILS.StepNum, 0 },
                                    { CUP_DETAILS.TotalStep, 0 },
                                    { CUP_DETAILS.TechnologyName, DBNull.Value },
                                    { CUP_DETAILS.SetTemp, DBNull.Value },
                                    { CUP_DETAILS.RecordIndex, 0 },
                                    { CUP_DETAILS.Cooperate, 0 },
                                    { CUP_DETAILS.Fail, 0 },
                                    { CUP_DETAILS.CurrentStepFinish,0 }
                                 },
                                 $"{CUP_DETAILS.CupNum} = @cupNo  AND {CUP_DETAILS.IsUsing} = 0 AND {CUP_DETAILS.Enable} = 1 ",
                                 new SqlParameter("@cupNo", cupNo));

                        if (updateResult == 0)
                        {
                            //更新失败，可能是状态不对或者正在使用，跳过这个杯
                            cupNos.Remove(cupNo);

                            continue;
                        }

                        var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                        if (area != null)
                        {
                            area.OnCupDataReceived(cupNo);
                        }

                        continue;
                    }
                    else
                    {


                        int updateResult = SqlServer.Update(CUP_DETAILS.TableName,
                               new Dictionary<string, object>()
                               {
                                    { CUP_DETAILS.HeadID,headId },
                                    { CUP_DETAILS.IsUsing,1 },


                                    { CUP_DETAILS.FormulaCode,formulaCode },
                                    { CUP_DETAILS.DyeingCode,dyeingCode },

                                    { CUP_DETAILS.CurrentWeight, 0 },
                                    { CUP_DETAILS.TotalWeight,totalWeight },
                                    { CUP_DETAILS.SetTime,0 },
                                    { CUP_DETAILS.DyeType, DBNull.Value },
                                    { CUP_DETAILS.StartTime, DBNull.Value },
                                    { CUP_DETAILS.StepStartTime, DBNull.Value },
                                    { CUP_DETAILS.StepNum, 0 },
                                    { CUP_DETAILS.TotalStep, 0 },
                                    { CUP_DETAILS.TechnologyName, DBNull.Value },
                                    { CUP_DETAILS.SetTemp, DBNull.Value },
                                    { CUP_DETAILS.RecordIndex, 0 },
                                    { CUP_DETAILS.Cooperate, 0 },
                                    { CUP_DETAILS.Fail, 0 },
                                    { CUP_DETAILS.CurrentStepFinish,0 }
                               },
                               $"{CUP_DETAILS.CupNum} = @cupNo AND {CUP_DETAILS.Statues} = @statues AND {CUP_DETAILS.IsUsing} = 0 AND {CUP_DETAILS.Enable} = 1 ",
                               new SqlParameter("@cupNo", cupNo),
                               new SqlParameter("@statues", "待机"));

                        if (updateResult == 0)
                        {
                            //更新失败，可能是状态不对或者正在使用，跳过这个杯
                            cupNos.Remove(cupNo);

                            continue;
                        }

                        var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                        if (area != null)
                        {
                            area.OnCupDataReceived(cupNo);
                        }

                        if (sentCupNos.Contains(cupNo))
                            continue; // 已发过，跳过

                        // 1. 查找同组的另一杯
                        var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                        int mainCupNum = mainInfo.CupNum;
                        int subCupNum = subInfo.CupNum;

                        if (sentCupNos.Contains(mainCupNum) || sentCupNos.Contains(subCupNum))
                            continue; // 已发过，跳过
                        else
                        {
                            //没发过 - 那就要查批次中是否包含另一个杯
                            // 1 = A,2 = B,0 = A + B
                            int cupChoice = -1;
                            var mainData = batchData.Select($"{DROP_HEAD.CupNum} = {mainCupNum} AND {DROP_HEAD.BatchName} = '{batchNo}'");
                            var subData = batchData.Select($"{DROP_HEAD.CupNum} = {subCupNum} AND {DROP_HEAD.BatchName} = '{batchNo}'");
                            if (mainData.Length > 0 && subData.Length == 0)
                            {
                                cupChoice = 1;
                                sentCupNos.Add(mainCupNum);
                            }
                            else if (mainData.Length == 0 && subData.Length > 0)
                            {
                                cupChoice = 2;
                                sentCupNos.Add(subCupNum);
                            }
                            else
                            {
                                cupChoice = 0;
                                sentCupNos.Add(mainCupNum);
                                sentCupNos.Add(subCupNum);

                            }

                            var task = CupCommManager.Instance.RequestDropLiquidAsync(cupNo, cupChoice);
                            sendTask.Add(task);
                        }


                    }

                }



                //2.(等待所有请求滴液任务全部完成)
                DyeingResult[] results = await Task.WhenAll(sendTask);
                foreach (DyeingResult dyeing in results)
                {
                    if (dyeing == null) continue;
                    if (dyeing.Code == My_Tool.Result.ResultCode.Exception)
                    {
                        //出现异常了 现在先移除看后面怎么处理 先预留
                        cupNos.Remove(dyeing.CupNo);
                    }
                    else if (dyeing.Code == My_Tool.Result.ResultCode.Failure)
                    {
                        //需要前洗杯的
                        var (mainCup, subCup) = CupAuxiliary.GetCupPair(dyeing.CupNo);
                        if (cupNos.Contains(mainCup))
                        {
                            cupNos.Remove(mainCup);
                        }
                        if (cupNos.Contains(subCup))
                        {
                            cupNos.Remove(subCup);
                        }

                    }
                }


                //3.把开始时间写入滴液表头，表示开始滴液

                if (cupNos.Count > 0)
                {
                    // 构造 IN 子句参数
                    var inParams = string.Join(",", cupNos.Select((c, i) => $"@CupNo{i}"));
                    var sql = $"UPDATE {DROP_HEAD.TableName} SET {DROP_HEAD.StartTime} = @StartTime WHERE {DROP_HEAD.BatchName} = @BatchName AND {DROP_HEAD.CupNum} IN ({inParams})";
                    var paramList = new List<SqlParameter>
                    {
                        new SqlParameter("@StartTime", DateTime.Now),
                        new SqlParameter("@BatchName", batchNo)
                    };
                    paramList.AddRange(cupNos.Select((c, i) => new SqlParameter($"@CupNo{i}", c)));
                    My_DataBase.SqlServer.ExecuteNonQuery(sql, paramList.ToArray());
                }



                //4.寻找可以开始滴液最小杯号
                var canDropData = My_DataBase.SqlServer.Select(
                    DROP_HEAD.TableName,
                    null,
                    $"{DROP_HEAD.BatchName} = @BatchName AND {DROP_HEAD.StartTime} IS NOT NULL AND {DROP_HEAD.CupFinish} = 0",
                    $"{DROP_HEAD.CupNum}",
                    true,
                    new SqlParameter("@BatchName", batchNo)
                  );
                if (canDropData == null || canDropData.Rows.Count == 0)
                {
                    //目前没有杯子可以直接开始 都要洗杯 ,等洗杯完成后再来滴液
                    // 判断是否所有未完成杯都处于洗杯阶段（StartTime==null）
                    var unfinishedCups = batchData.AsEnumerable()
                        .Where(r => Convert.ToInt32(r[DROP_HEAD.CupFinish]) == 0)
                        .Select(r => Convert.ToInt32(r[DROP_HEAD.CupNum]))
                        .ToList();

                    var washingCups = batchData.AsEnumerable()
                        .Where(r => r[DROP_HEAD.StartTime] == DBNull.Value)
                        .Select(r => Convert.ToInt32(r[DROP_HEAD.CupNum]))
                        .ToList();

                    if (unfinishedCups.All(c => washingCups.Contains(c)))
                    {
                        // 所有未完成杯都在洗杯，主动等待或提示
                        await Task.Delay(1000); // 等待1秒，避免死循环CPU占用
                                                // 可加日志或用户提示
                    }
                    goto label2;
                }

                //5. 按照优先级排序，依次处理加水、加染料、加助剂
                var processList = new List<(int Priority, string Type)>
                {
                    (SmartColor.My_ConPar.Order.DropProcess.DropProcess.AddWaterProcess, "Water"),
                    (SmartColor.My_ConPar.Order.DropProcess.DropProcess.AddDyeProcess, "Dye"),
                    (SmartColor.My_ConPar.Order.DropProcess.DropProcess.AddAuxiliaryAgentProcess, "Aux")
                };
                processList.Sort((a, b) => a.Priority.CompareTo(b.Priority)); // 数字越小优先级越高

                foreach (var process in processList)
                {
                    if (process.Type == "Water")
                    {
                        var waterResult = await WaterProcess(canDropData);
                        switch (waterResult.Code)
                        {
                            case My_Tool.Result.ResultCode.Exception:
                                {
                                    throw new Exception("加水验证异常：" + waterResult.Message, waterResult.Exception);
                                }
                            case My_Tool.Result.ResultCode.Canceled:
                                {
                                    //取消任务
                                    throw new TaskCanceledException("任务被取消");
                                }
                            case My_Tool.Result.ResultCode.Failure:
                                {
                                    //加水失败，是否重滴
                                    var btn = await MessageEventManager.Instance.RequestShowMessageAsync(
                                        "水验证失败", "加水误差过大，是否重滴？",
                                        new[] { "重滴", "继续", "退出" },
                                        "重滴");

                                    if (btn == "重滴")
                                    {
                                        List<int> FailCupNo_Temp = new List<int>();
                                        FailCupNo_Temp.AddRange(waterResult.FailCupNo);
                                        foreach (var cupNo in waterResult.FailCupNo)
                                        {
                                            var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                            int mainCupNum = mainInfo.CupNum;
                                            int subCupNum = subInfo.CupNum;

                                            if (mainCupNum != subCupNum)
                                            {
                                                var otherInfo = mainCupNum == cupNo ? subInfo : mainInfo;
                                                if (otherInfo.IsUsing == 1 && !waterResult.FailCupNo.Contains(otherInfo.CupNum))
                                                {
                                                    FailCupNo_Temp.Add(otherInfo.CupNum);
                                                    SqlServer.Update(DROP_HEAD.TableName,
                                                         new Dictionary<string, object>()
                                                         {
                                                            { DROP_HEAD.BatchName, batchNo }

                                                         },
                                                         $"{DROP_HEAD.CupNum} = @cupNo",
                                                         new SqlParameter("@cupNo", otherInfo.CupNum)
                                                     );
                                                    SqlServer.Update(DROP_DETAILS.TableName,
                                                        new Dictionary<string, object>()
                                                        {
                                                             { DROP_DETAILS.BatchName, batchNo }
                                                        },
                                                        $"{DROP_DETAILS.CupNum} = @cupNo",
                                                        new SqlParameter("@cupNo", otherInfo.CupNum)
                                                    );
                                                    SqlServer.Update(DYE_DETAILS.TableName,
                                                        new Dictionary<string, object>()
                                                        {
                                                             { DYE_DETAILS.BatchName, batchNo }
                                                        },
                                                        $"{DYE_DETAILS.CupNum} = @cupNo",
                                                        new SqlParameter("@cupNo", otherInfo.CupNum)
                                                    );
                                                }
                                            }
                                        }
                                        waterResult.FailCupNo.Clear();
                                        waterResult.FailCupNo.AddRange(FailCupNo_Temp);

                                        //重置数据库
                                        await ResetCupState(batchNo, waterResult.FailCupNo);




                                        foreach (var cupNo in waterResult.FailCupNo)
                                        {
                                            My_DataBase.SqlServer.Update(
                                               CUP_DETAILS.TableName,
                                               new Dictionary<string, object> {
                                                   { CUP_DETAILS.Fail, 0 },
                                                   { CUP_DETAILS.IsUsing, 0 }
                                               },
                                               $"{CUP_DETAILS.CupNum} = @cupNo",
                                               new SqlParameter("@cupNo", cupNo)
                                           );

                                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                                            if (area != null)
                                            {
                                                area.OnCupDataReceived(cupNo);
                                            }
                                        }

                                        goto label3;
                                    }

                                    else if (btn == "退出")
                                    {
                                        result.Code = My_Tool.Result.ResultCode.Canceled;
                                        result.Message = "加水验证失败退出滴液";
                                        return result;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                            default:
                                break;
                        }


                    }
                    else if (process.Type == "Dye" || process.Type == "Aux")
                    {
                        string unitOfAccount = process.Type == "Dye" ? "%" : "g/l";
                        foreach (DataRow row in canDropData.Rows)
                        {

                            int minCupNo = Convert.ToInt32(row[DROP_HEAD.CupNum]);
                            int cupFinish = Convert.ToInt32(row[DROP_HEAD.CupFinish]);
                            if (cupFinish == 1)
                            {
                                //杯子已经完成了，跳过
                                continue;
                            }
                            var dyeResult = await SingleCupDropProcess(batchNo, minCupNo, unitOfAccount, checkLow, checkExpired);
                            switch (dyeResult.Code)
                            {
                                case My_Tool.Result.ResultCode.Exception:
                                    {
                                        throw new Exception($"{process.Type}验证异常：{dyeResult.Message}", dyeResult.Exception);
                                    }
                                case My_Tool.Result.ResultCode.Canceled:
                                    {
                                        //取消任务
                                        throw new TaskCanceledException("任务被取消");
                                    }
                            }

                            if (dyeResult.NoSyeing.Count > 0)
                            {
                                bottleNotFound.AddRange(dyeResult.NoSyeing);
                            }
                            if (dyeResult.BottleWeightLow.Count > 0)
                            {
                                bottleWeightLow.AddRange(dyeResult.BottleWeightLow);
                            }
                            if (dyeResult.BottleOutdated.Count > 0)
                            {
                                bottleOutdated.AddRange(dyeResult.BottleOutdated);
                            }
                            if (dyeResult.FailCupNo.Count > 0)
                            {
                                failCupNo.AddRange(dyeResult.FailCupNo);
                            }

                        }
                    }

                }

                //6. 母液过期环节 
                if (bottleOutdated.Count > 0)
                {
                    bottleOutdated = bottleOutdated.Distinct().ToList();
                    bottleNotFound.Sort();
                    string msg = string.Join(",", bottleOutdated) + "号母液瓶已过期,";
                    bool again = false;
                    if (My_ConPar.Choices.DripAllowExpired == 0)
                    {

                        //过期不允许滴液
                        var btn = await MessageEventManager.Instance.RequestShowMessageAsync(
                            "过期提醒", $"{msg}请重新泡制后点击继续，", new[] { "继续", "退出" }, "继续");
                        if (btn == "继续")
                        {
                            // 用户点击继续

                            again = true;
                        }
                    }
                    else
                    {
                        //过期允许滴液
                        var btn = await MessageEventManager.Instance.RequestShowMessageAsync("过期提醒",
                           $"{msg}是否允许继续滴液，", new[] { "允许", "退出" }, "允许");
                        if (btn == "允许")
                        {
                            // 用户点击允许
                            checkExpired = false;
                            again = true;
                        }
                    }

                    if (again)
                    {
                        bottleOutdated.Clear();
                        goto label2;
                    }
                    else
                    {
                        throw new TaskCanceledException("母液瓶过期退出滴液");

                    }
                }

                //7. 母液瓶不足环节

                if (bottleWeightLow.Count > 0)
                {
                    bottleWeightLow = bottleWeightLow.Distinct().ToList();
                    bottleWeightLow.Sort();
                    string msg = string.Join(",", bottleWeightLow) + "号母液瓶液量过低,";
                    bool again = false;
                    if (My_ConPar.Choices.DripAllowLow == 0)
                    {
                        //液量低不允许滴液
                        var btn = await MessageEventManager.Instance.RequestShowMessageAsync("液量不足提醒",
                           $"{msg}请重新泡制后点击继续", new[] { "继续", "退出" }, "继续");
                        if (btn == "继续")
                        {
                            // 用户点击继续
                            again = true;
                        }

                    }
                    else
                    {
                        //液量低允许滴液
                        var btn = await MessageEventManager.Instance.RequestShowMessageAsync("液量不足提醒",
                           $"{msg}是否允许继续滴液，", new[] { "允许", "退出" }, "允许");
                        if (btn == "允许")
                        {
                            // 用户点击允许
                            checkLow = false;
                            again = true;
                        }
                    }

                    if (again)
                    {
                        bottleWeightLow.Clear();
                        goto label2;
                    }
                    else
                    {

                        throw new TaskCanceledException("母液瓶液量不足退出滴液");
                    }
                }

                //8. 未发现针筒环节

                if (bottleNotFound.Count > 0)
                {
                    bottleNotFound = bottleNotFound.Distinct().ToList();
                    bottleNotFound.Sort();
                    string msg = string.Join(",", bottleNotFound) + "号母液瓶未发现针筒,";

                    var btn = await MessageEventManager.Instance.RequestShowMessageAsync("未找到针筒提醒",
                       $"{msg}+请放回针筒后点击继续，", new[] { "继续", "退出" }, "继续");
                    if (btn == "继续")
                    {
                        bottleNotFound.Clear();
                        goto label2;
                    }
                    else
                    {
                        throw new TaskCanceledException("未找到针筒退出滴液");
                    }
                }

                //9. 是否所有杯子滴液完成
                var unfinishedData = My_DataBase.SqlServer.Select(
                    DROP_HEAD.TableName,
                    null,
                    $"{DROP_HEAD.BatchName} = @BatchName " +
                    $"AND {DROP_HEAD.CupFinish} = 0",
                    $"{DROP_HEAD.CupNum}",
                    true,
                    new SqlParameter("@BatchName", batchNo)
                  );
                if (unfinishedData.Rows.Count > 0)
                {
                    goto label2;
                }


                //10. 判断是否有滴液失败的杯子（通过批次表和杯详情表的Fail字段判断）

                // 1. 查找当前批次所有用到的杯号


                // 2. 在杯详情表查找Fail=1的杯号
                string sql1 = $@"
                    SELECT DISTINCT c.{CUP_DETAILS.CupNum}
                    FROM {CUP_DETAILS.TableName} c
                    INNER JOIN {DROP_HEAD.TableName} h ON c.{CUP_DETAILS.CupNum} = h.{DROP_HEAD.CupNum}
                    WHERE h.{DROP_HEAD.BatchName} = @BatchName AND c.{CUP_DETAILS.Fail} = 1
                ";
                var failCupsDt = My_DataBase.SqlServer.ExecuteQuery(sql1, new SqlParameter("@BatchName", batchNo));
                var failCups = failCupsDt.AsEnumerable()
                    .Select(r => Convert.ToInt32(r[CUP_DETAILS.CupNum]))
                    .Distinct()
                    .ToList();

                if (failCups.Count > 0)
                {
                    failCups.Sort();
                    string msg = string.Join(",", failCups) + "号杯滴液失败,";

                    var btn = await MessageEventManager.Instance.RequestShowMessageAsync("滴液失败",
                        $"{msg}是否重滴，", new[] { "重滴", "继续", "退出" }, "重滴");
                    if (btn == "重滴")
                    {
                        List<int> FailCupNo_Temp = new List<int>();
                        FailCupNo_Temp.AddRange(failCups);

                        foreach (var cupNo in failCups)
                        {
                            var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                            int mainCupNum = mainInfo.CupNum;
                            int subCupNum = subInfo.CupNum;

                            if (mainCupNum != subCupNum)
                            {
                                var otherInfo = mainCupNum == cupNo ? subInfo : mainInfo;
                                if (otherInfo.IsUsing == 1 && !failCups.Contains(otherInfo.CupNum))
                                {
                                    FailCupNo_Temp.Add(otherInfo.CupNum);
                                    SqlServer.Update(DROP_HEAD.TableName,
                                         new Dictionary<string, object>()
                                         {
                                                            { DROP_HEAD.BatchName, batchNo }

                                         },
                                         $"{DROP_HEAD.CupNum} = @cupNo",
                                         new SqlParameter("@cupNo", otherInfo.CupNum)
                                     );
                                    SqlServer.Update(DROP_DETAILS.TableName,
                                        new Dictionary<string, object>()
                                        {
                                                             { DROP_DETAILS.BatchName, batchNo }
                                        },
                                        $"{DROP_DETAILS.CupNum} = @cupNo",
                                        new SqlParameter("@cupNo", otherInfo.CupNum)
                                    );
                                    SqlServer.Update(DYE_DETAILS.TableName,
                                        new Dictionary<string, object>()
                                        {
                                                             { DYE_DETAILS.BatchName, batchNo }
                                        },
                                        $"{DYE_DETAILS.CupNum} = @cupNo",
                                        new SqlParameter("@cupNo", otherInfo.CupNum)
                                    );
                                }
                            }
                        }
                        failCups.Clear();
                        failCups.AddRange(FailCupNo_Temp);

                        //重置数据库
                        await ResetCupState(batchNo, failCups);


                        foreach (var cupNo in failCups)
                        {
                            My_DataBase.SqlServer.Update(
                               CUP_DETAILS.TableName,
                               new Dictionary<string, object> {
                                   { CUP_DETAILS.Fail, 0 },
                                   { CUP_DETAILS.IsUsing, 0 }
                               },
                               $"{CUP_DETAILS.CupNum} = @cupNo",
                               new SqlParameter("@cupNo", cupNo)
                           );
                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                            if (area != null)
                            {
                                area.OnCupDataReceived(cupNo);
                            }
                        }

                        failCups.Clear();


                        goto label3;
                    }
                    else if (btn == "继续")
                    {
                        // 新增：类型为3的杯，自动启动染固色
                        foreach (var cupNo in failCups)
                        {
                            var cupRows = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {cupNo}");
                            if (cupRows.Rows.Count > 0)
                            {
                                var cupRow = cupRows.Rows[0];
                                int type = cupRow.Table.Columns.Contains(CUP_DETAILS.Type) && cupRow[CUP_DETAILS.Type] != DBNull.Value
                                    ? Convert.ToInt32(cupRow[CUP_DETAILS.Type])
                                    : 0;
                                if (type == 3)
                                {
                                    // 启动染固色
                                    var cupSelect = My_Tool.CupAuxiliary.GetCupChioce(cupNo);
                                    _ = CupCommManager.Instance.RequestDyeingAsync(cupNo, cupSelect);
                                }
                            }
                        }
                    }
                    else
                    {
                        StopCup(batchNo, failCups);
                        result.Code = My_Tool.Result.ResultCode.Canceled;
                        result.Message = "滴液失败退出滴液";
                        return result;
                    }
                }






                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = $"{batchNo}批次滴液完成";
                return result;
            }
            catch (TaskCanceledException)
            {
                IsStopped = false;
                // 1. 获取所有未完成的杯号
                var unfinishedData = My_DataBase.SqlServer.Select(
                    DROP_HEAD.TableName,
                    null,
                    $"{DROP_HEAD.BatchName} = @BatchName AND {DROP_HEAD.CupFinish} = 0",
                    $"{DROP_HEAD.CupNum}",
                    true,
                    new SqlParameter("@BatchName", batchNo)
                );
                var failCupNos = unfinishedData.AsEnumerable()
                    .Select(r => Convert.ToInt32(r[DROP_HEAD.CupNum]))
                    .ToList();

                // 2. 批量标记为失败
                foreach (var cupNo in failCupNos)
                {
                    My_DataBase.SqlServer.Update(
                        CUP_DETAILS.TableName,
                        new Dictionary<string, object> { { CUP_DETAILS.Fail, 1 } },
                        $"{CUP_DETAILS.CupNum} = @cupNo",
                        new SqlParameter("@cupNo", cupNo)
                    );

                    var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                    if (area != null)
                    {
                        area.OnCupDataReceived(cupNo);
                    }
                    My_DataBase.SqlServer.Update(
                        DROP_HEAD.TableName,
                        new Dictionary<string, object> {
                            { DROP_HEAD.DescribeChar, "滴液失败（用户停止）" },
                            { DROP_HEAD.FinishTime, DateTime.Now },
                            { DROP_HEAD.CupFinish, 1 }
                        },
                        $"{DROP_HEAD.CupNum} = @cupNo AND {DROP_HEAD.BatchName} = @batchNo",
                        new SqlParameter("@cupNo", cupNo),
                        new SqlParameter("@batchNo", batchNo)
                    );
                }





                // 3. 是否重滴提示
                var btn = await MessageEventManager.Instance.RequestShowMessageAsync(
                    "滴液中断", "检测到用户停止，是否重滴？",
                    new[] { "重滴", "退出" }, "重滴"
                );
                foreach (var cupNo in failCupNos)
                {
                    My_DataBase.SqlServer.Update(
                       CUP_DETAILS.TableName,
                       new Dictionary<string, object> {
                           { CUP_DETAILS.Fail, 1 },
                           { CUP_DETAILS.IsUsing, 0 }
                       },
                       $"{CUP_DETAILS.CupNum} = @cupNo",
                       new SqlParameter("@cupNo", cupNo)
                   );
                }

                if (btn == "重滴")
                {

                    // 重置这些杯号的状态
                    await ResetCupState(batchNo, failCupNos);


                    // 重新开始滴液
                    return await EnqueueBatchDropAsync(batchNo);
                }
                else
                {
                    StopCup(batchNo, failCupNos);

                    result.Code = My_Tool.Result.ResultCode.Canceled;
                    result.Message = "用户停止，批次全部标记为滴液失败";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = "代码异常：" + ex.Message;
                result.Exception = ex;
                return result;
            }
        }

        private static async void StopCup(string batchNo, List<int> failCupNos)
        {
            // 重置这些杯号的状态
            await ResetCupState(batchNo, failCupNos);
            foreach (var cupNo in failCupNos)
            {
                CupAuxiliary.ClearBatchCupData(cupNo);
                CupFinished?.Invoke(cupNo);
            }
        }
    }
}
