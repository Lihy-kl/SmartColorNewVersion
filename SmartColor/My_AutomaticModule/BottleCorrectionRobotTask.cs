using com.google.zxing;
using SmartColor.My_ConPar.Order;
using SmartColor.My_DataBase;
using SmartColor.My_RobotManager;
using SmartColor.My_SemiAutoModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SmartColor.My_AutomaticModule
{
    /// <summary>
    /// 母液瓶校正结果类型
    /// </summary>
    public class BottleCorrectionResult
    {
        /// <summary>结果码，(成功，失败，异常，取消)</summary>
        public My_Tool.Result.ResultCode Code { get; set; }
        /// <summary>详细信息</summary>
        public string Message { get; set; }
        /// <summary>母液瓶号</summary>
        public int BottleNo { get; set; }
        /// <summary>校正值</summary>
        public double? AdjustValue { get; set; }
        /// <summary>加液重量</summary>
        public double? AddedWeight { get; set; }
        /// <summary>验证加液重量</summary>
        public double? VerifyAddedWeight { get; set; }
        /// <summary>实际浓度</summary>
        public double? RealConcentration { get; set; }
        /// <summary>初始重量</summary>
        public double? InitialWeight { get; set; }
        /// <summary>天平最终重量</summary>
        public double? FinalWeight { get; set; }
        /// <summary>天平验证后重量</summary>
        public double? VerifyFinalWeight { get; set; }
        /// <summary>数据库操作是否成功</summary>
        public bool? DbSuccess { get; set; }
        /// <summary>异常对象</summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// 母液瓶校正模块
    /// 只负责流程和结果信息，不负责弹窗交互
    /// </summary>
    internal static class BottleCorrectionRobotTask
    {
        /// <summary>
        /// 单瓶校正流程（原子任务，返回详细结果）
        /// </summary>
        /// <param name="bottleNo">母液瓶号</param>
        /// <returns>(成功，失败，异常，取消)</returns>
        public static async Task<BottleCorrectionResult> SingleBottle(int bottleNo)
        {
            var result = new BottleCorrectionResult { BottleNo = bottleNo };
            try
            {
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


                var currentTask = RobotTaskManager.Instance.CurrentTask as RobotBusinessTask<BottleCorrectionResult>;
                var bottleCT = My_Tool.AreaCoordinateFinder.TryGetBottleCoordinateAsync(bottleNo);
                var balanceCT = My_Tool.AreaCoordinateFinder.TryGetBalanceCoordinateAsync();
                int roundDigits = My_Tool.BalanceStableReading.RetainDecimals();
                // 步骤1：并发查找瓶资料和移动到瓶位置
                var bottleCTR = await bottleCT;
                if (!bottleCTR.found)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"获取{bottleNo}号母液瓶坐标异常";
                    result.Exception = new Exception($"获取{bottleNo}号母液瓶坐标异常");
                    return result;
                }

                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                var findTask = Task.Run(() => TryGetSyringeTypeAndPulse(bottleNo));
                var moveTask = SemiAutoHelperFactory.Current.MoveToBottleAsync(bottleNo, bottleCTR.x, bottleCTR.y, 2, 1);
                await Task.WhenAll(findTask, moveTask);

                switch (findTask.Result.result)
                {
                    case -1:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = "未找到瓶资料";
                        return result;
                    case -2:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = "针筒类型错误";
                        return result;
                    case -3:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = "脉冲参数异常";
                        return result;
                    case -4:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = "配置参数异常";
                        return result;
                    case -5:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = "代码异常";
                        return result;
                    default:
                        break;
                }


                result.RealConcentration = Convert.ToDouble(findTask.Result.data[My_DataBase.BOTTLE_DETAILS.RealConcentration] ?? 0);
                double currentWeight = Convert.ToDouble(findTask.Result.data[My_DataBase.BOTTLE_DETAILS.CurrentWeight] ?? 0);
                double lastAdjustWeight = Convert.ToDouble(findTask.Result.data[My_DataBase.BOTTLE_DETAILS.CurrentAdjustWeight] ?? 0);
                double adjustSucess = Convert.ToDouble(findTask.Result.data[My_DataBase.BOTTLE_DETAILS.AdjustSuccess] ?? 0);
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


                if (adjustSucess == 1)
                {
                    //这里可能自动校正被打断，有可能滴液过程中已经重新校正了
                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("上次校正已成功,无需重复校正");
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = $"上次校正已成功，当前重量:{currentWeight}g,上次校正重量:{lastAdjustWeight}g";
                    return result;
                }

                // 步骤2：并发天平检查+稳定读取 和 抽液
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                var balanceTask = My_Tool.BalanceStableReading.CheckAndReadBalanceAsync();
                var aspirateTask = SemiAutoHelperFactory.Current.AspirateAsync(
                    findTask.Result.z, My_ConPar.Other.DrainCount <= 0 ? 1 : My_ConPar.Other.DrainCount, findTask.Result.syringeType, findTask.Result.currentWeight);
                //更新当前瓶上次使用时间
                My_DataBase.SqlServer.Update(BOTTLE_DETAILS.TableName,
                    new Dictionary<string, object>
                    {
                        { BOTTLE_DETAILS.LastUseTime, DateTime.Now }
                    },
                    $"{BOTTLE_DETAILS.BottleNum} = {bottleNo}");

                await Task.WhenAll(balanceTask, aspirateTask);

                if (balanceTask.Result == 9999.99)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "天平异常";
                    result.Exception = new Exception("天平检查异常");
                    return result;
                }
                result.InitialWeight = balanceTask.Result;

                semiAutoResult = await aspirateTask;
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


                // 步骤3：移动到天平位置
                var balanceCTR = await balanceCT;
                if (!balanceCTR.found)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"获取天平坐标异常";
                    result.Exception = new Exception($"获取天平坐标异常");
                    return result;
                }

                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                semiAutoResult = await SemiAutoHelperFactory.Current.MoveToBalanceAsync(balanceCTR.x, balanceCTR.y);
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

                // 步骤4：注液
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                semiAutoResult = await SemiAutoHelperFactory.Current.DispenseAsync(10000, findTask.Result.syringeType, 0);
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


                // 步骤5：稳定读取天平
                double finalWeight = await My_Tool.BalanceStableReading.StableReadingAsync();
                result.FinalWeight = finalWeight;

                // 步骤6：计算校正值
                double addedWeight = Math.Round(finalWeight - result.InitialWeight.Value, roundDigits);
                result.AddedWeight = addedWeight;
                bool addedWeightError = false;
                if (addedWeight <= 0)
                {
                    // 标记异常，但不直接返回
                    result.Code = My_Tool.Result.ResultCode.Failure;
                    result.Message = $"校正重量为0,请检查{bottleNo}号母液瓶液量是否足够,或者Z轴失步";
                    addedWeightError = true;
                }

                double adjust = Math.Round((findTask.Result.syringeType == 0 ?
                    My_ConPar.Correction.Correcting_S_Pulse :
                    My_ConPar.Correction.Correcting_B_Pulse) / addedWeight, roundDigits);
                result.AdjustValue = adjust;

                // 步骤7：根据校正值计算验证脉冲
                int rw = findTask.Result.syringeType == 0 ?
                    My_ConPar.Correction.Correcting_S_Weight :
                    My_ConPar.Correction.Correcting_B_Weight;
                int verifyPulse = (int)(rw * adjust);

                if (!addedWeightError)
                {
                    if (verifyPulse <= 0 || verifyPulse > 10000)
                    {
                        // 标记异常，但不直接返回
                        result.Code = My_Tool.Result.ResultCode.Failure;
                        result.Message = $"校正脉冲过大或过小,请检查{bottleNo}号母液瓶液量是否足够,或者Z轴失步";
                        addedWeightError = true;
                    }
                }

                // 步骤8：验证注液（只有未出错时才执行，否则跳过）
                if (!addedWeightError)
                {
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();

                    semiAutoResult = await SemiAutoHelperFactory.Current.DispenseAsync(10000 - verifyPulse, findTask.Result.syringeType, 0);
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

                // 步骤9：移动回母液瓶
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                semiAutoResult = await SemiAutoHelperFactory.Current.MoveToBottleAsync(bottleNo, bottleCTR.x, bottleCTR.y, 1, 0);
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


                // 步骤10：异步启动放针
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var releaseNeedleTask = SemiAutoHelperFactory.Current.ReleaseNeedleAsync(findTask.Result.syringeType);

                // 步骤11：稳定读取天平

                double verifyFinalWeight = await My_Tool.BalanceStableReading.StableReadingAsync();
                result.VerifyFinalWeight = verifyFinalWeight;
                double verifyAddedWeight = Math.Round(verifyFinalWeight - finalWeight, My_Tool.BalanceStableReading.RetainDecimals());
                result.VerifyAddedWeight = verifyAddedWeight;
                if (!addedWeightError)
                {
                    if (verifyAddedWeight <= 0)
                    {
                        result.Code = My_Tool.Result.ResultCode.Failure;
                        result.Message = $"验证重量为0,请检查{bottleNo}号母液瓶液量是否足够,或者Z轴失步";
                        addedWeightError = true;
                    }
                }




                // 步骤12：判断验证结果，更新数据库

                double errorWeight = Math.Round(Math.Abs(rw - verifyAddedWeight), roundDigits);
                bool success = false;
                bool dbOk = false;
                if (errorWeight <= My_ConPar.Other.CorrectingAErr)
                {
                    // 验证通过，更新数据库
                    SqlServer.Update(BOTTLE_DETAILS.TableName,
                        new Dictionary<string, object>
                        {

                            { BOTTLE_DETAILS.CurrentWeight, Math.Round(currentWeight - addedWeight - verifyAddedWeight, roundDigits) },
                            { BOTTLE_DETAILS.LastAdjustWeight, lastAdjustWeight },
                            { BOTTLE_DETAILS.CurrentAdjustWeight, addedWeight },
                            { BOTTLE_DETAILS.AdjustValue, adjust },
                            { BOTTLE_DETAILS.AdjustSuccess, 1 }
                        },
                        $"{BOTTLE_DETAILS.BottleNum} = {bottleNo}");


                    success = true;
                    dbOk = true;
                }
                else
                {
                    // 验证失败，更新数据库
                    SqlServer.Update(BOTTLE_DETAILS.TableName,
                        new Dictionary<string, object>
                        {
                            { BOTTLE_DETAILS.CurrentWeight, Math.Round(currentWeight - addedWeight - verifyAddedWeight, roundDigits) },
                            { BOTTLE_DETAILS.AdjustSuccess, 0 }
                        },
                        $"{BOTTLE_DETAILS.BottleNum} = {bottleNo}");
                    success = false;
                    dbOk = true;
                }

                result.DbSuccess = dbOk;
                if (double.IsNaN(adjust) || double.IsInfinity(adjust))
                {
                    adjust = 0; // 或者 return/throw
                }
                // 步骤13：记录校正日志
                var checkTableRow = new Dictionary<string, object>
                {
                    { CHECK_TABLE.Date, DateTime.Now },
                    { CHECK_TABLE.BottleNum, bottleNo },
                    { CHECK_TABLE.RealConcentration, result.RealConcentration },
                    { CHECK_TABLE.CurrentWeight, currentWeight },
                    { CHECK_TABLE.CurrentAdjustWeight, addedWeight },
                    { CHECK_TABLE.AdjustValue, adjust },
                    { CHECK_TABLE.RecheckWeight, verifyAddedWeight },
                    { CHECK_TABLE.Fail, success ? 0 : 1 }
                };
                SqlServer.Insert(CHECK_TABLE.TableName, checkTableRow);

                // 步骤14：等待放针完成
                semiAutoResult = await releaseNeedleTask;
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

                // 返回最终结果
                if (success)
                {
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "校正成功";
                }
                else
                {
                    if (!addedWeightError)
                    {
                        result.Code = My_Tool.Result.ResultCode.Failure;
                        result.Message = "校正超出允许误差";
                    }
                }


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
        /// 查找母液瓶资料并计算脉冲
        /// </summary>
        /// <param name="bottleNo">瓶号</param>
        /// <returns>
        /// result=0成功
        /// result=-1 未找到资料
        /// result=-2 针筒类型错误
        /// result=-3 脉冲参数异常
        /// result=-4 配置参数异常
        /// result=-5 代码异常
        /// </returns>
        private static (int result, int z, short syringeType,short currentWeight, DataRow data) TryGetSyringeTypeAndPulse(int bottleNo)
        {
            try
            {
                var rows = BottleData.Bottle_details.Select($"{BOTTLE_DETAILS.BottleNum} = {bottleNo}");
                if (rows.Length == 0)
                    return (-1, 0, 0, 0, null); // 未找到

                DataRow data = rows[0];
                string s = data[BOTTLE_DETAILS.SyringeType]?.ToString();
                if (s == null)
                    return (-2, 0, 0, 0, null);

                short syringeType;
                switch (s)
                {
                    case "小针筒":
                        syringeType = 0;
                        break;
                    case "大针筒":
                        syringeType = 1;
                        break;
                    default:
                        return (-2, 0, 0, 0, null);
                }

                int maxP = syringeType == 0 ? My_ConPar.Other.S_MaxPulse : My_ConPar.Other.B_MaxPulse;
                int cP = syringeType == 0 ? My_ConPar.Correction.Correcting_S_Pulse : My_ConPar.Correction.Correcting_B_Pulse;
                int z = cP + 10000 - My_ConPar.Other.Z_BackPulse;
                short currentWeight = Convert.ToInt16(data[BOTTLE_DETAILS.CurrentWeight] ?? 0);
                if (z > maxP)
                    return (-3, 0, 0, 0, null);
                else if (z <= 0)
                    return (-4, 0, 0, 0, null);


                return (0, z, syringeType, currentWeight, data);
            }
            catch
            {
                return (-5, 0, 0, 0, null);
            }
        }



        /// <summary>
        /// 提交一次完整的母液瓶校正流程为原子任务到机械手调度中心
        /// </summary>
        /// <param name="bottleNo">母液瓶号</param>
        /// <returns>BottleCorrectionResult，包含详细业务信息</returns>
        public static async Task<BottleCorrectionResult> EnqueueBottleCorrectionAsync(int bottleNo,
            BigProcess.RobotBusinessType robotBusinessType = BigProcess.RobotBusinessType.BottleCorrection,
            int priority = 0)
        {
            var task = new RobotBusinessTask<BottleCorrectionResult>
            {
                Priority = priority == 0 ? BigProcess.BottleCorrection * 100 : priority * 100,
                OriginalPriority = priority == 0 ? BigProcess.BottleCorrection * 100 : priority * 100,
                BusinessType = robotBusinessType,
                TaskName = $"{bottleNo}号母液瓶校正",
                BusinessFlow = async () => await SingleBottle(bottleNo)
            };

            try
            {
                var result = await RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<BottleCorrectionResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{bottleNo}号母液瓶校正异常", result.Message, btn =>
                            {
                                if (btn == "确认")
                                {
                                    tcs.SetResult(new BottleCorrectionResult
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
                    case My_Tool.Result.ResultCode.Failure:
                    case My_Tool.Result.ResultCode.NeedInteraction:
                        {
                            var tcs = new TaskCompletionSource<BottleCorrectionResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{bottleNo}号母液瓶校正异常", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueBottleCorrectionAsync(bottleNo, robotBusinessType, priority);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new BottleCorrectionResult
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
                        {
                            return result;
                        }


                }
            }
            catch (TaskCanceledException)
            {
                return new BottleCorrectionResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消",
                    BottleNo = bottleNo
                };
            }
            catch (Exception ex)
            {
                My_File.Logger.Error("EnqueueBottleCorrectionAsync异常", ex);
                return new BottleCorrectionResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex,
                    BottleNo = bottleNo
                };
            }
        }

        /// <summary>
        /// 批量母液瓶校正流程
        /// </summary>
        /// <param name="bottleNos">母液瓶号列表</param>
        /// <returns>字典，key为瓶号，value为BottleCorrectionResult</returns>
        public static async Task<Dictionary<int, BottleCorrectionResult>> EnqueueBatchBottleCorrectionAsync(List<int> bottleNos, BigProcess.RobotBusinessType robotBusinessType = BigProcess.RobotBusinessType.BottleCorrection,
            int priority = 0)
        {
            var results = new Dictionary<int, BottleCorrectionResult>();
            foreach (var bottleNo in bottleNos)
            {
                var result = await EnqueueBottleCorrectionAsync(bottleNo, robotBusinessType, priority);
                results.Add(bottleNo, result);
            }
            return results;
        }
    }
}