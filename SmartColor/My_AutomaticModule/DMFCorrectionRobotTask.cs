using SmartColor.My_ConPar.Order;
using SmartColor.My_RobotManager;
using SmartColor.My_SemiAutoModule;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SmartColor.My_AutomaticModule
{
    /// <summary>
    /// 溶解剂校正/验证结果类型
    /// 封装溶解剂校正/验证流程的详细业务结果信息
    /// </summary>
    public class DMFCorrectionResult
    {
        /// <summary>结果码</summary>

        public My_Tool.Result.ResultCode Code { get; set; }
        /// <summary>详细信息</summary>
        public string Message { get; set; }
        /// <summary>校正值</summary>
        public double? AdjustValue { get; set; }
        /// <summary>加溶解剂前重量</summary>
        public double? PreWeight { get; set; }
        /// <summary>加溶解剂后重量</summary>
        public double? PostWeight { get; set; }
        /// <summary>实际加溶解剂量</summary>
        public double? RealAddWeight { get; set; }
        /// <summary>误差百分比</summary>
        public double? ErrorPercent { get; set; }
        /// <summary>验证目标重量</summary>
        public double? RecheckWeight { get; set; }
        /// <summary>异常对象</summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// 溶解剂校正业务流程的原子任务封装工具类
    /// 只负责流程和详细结果，不负责弹窗交互
    /// </summary>
    public static class DMFCorrectionRobotTask
    {
        /// <summary>
        /// 验证加溶解剂流程（用于溶解剂校正/验证），返回详细业务结果
        /// </summary>
        /// <param name="preWeight">加溶解剂前天平的重量，默认9999表示需自动读取</param>
        /// <param name="recheckWeight">验证重量，默认0表示需自动读取</param>
        /// <param name="adjValue">校正值，默认0表示需自动读取</param>
        /// <returns>DMFCorrectionResult，包含详细业务信息</returns>
        private static async Task<DMFCorrectionResult> VerifyAddDMFAsync(double preWeight = 9999, int recheckWeight = 0, double adjValue = 0)
        {
            var result = new DMFCorrectionResult();
            try
            {
                var balanceCT = My_Tool.AreaCoordinateFinder.TryGetBalanceCoordinateAsync();
                var currentTask = RobotTaskManager.Instance.CurrentTask as RobotBusinessTask<DMFCorrectionResult>;
                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();

                // 自动读取加溶解剂前重量
                if (preWeight == 9999)
                {
                    if (currentTask != null)
                        await currentTask.CheckPauseAndCancelAsync();

                    // 天平检查
                    await My_Tool.BalanceStableReading.BalanceCheck();



                    // 并发读取天平重量和机械手移动
                    var readTask = My_Tool.BalanceStableReading.StableReadingAsync();
                    var balanceCTR = await balanceCT;
                    if (!balanceCTR.found)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Exception = new Exception("获取天平坐标异常");
                        result.Message = "获取天平坐标异常";
                        return result;
                    }
                    var moveTask = SemiAutoHelperFactory.Current.MoveToBalanceAsync(balanceCTR.x,balanceCTR.y);
                    await Task.WhenAll(readTask, moveTask);
                    preWeight = readTask.Result;
                }
                result.PreWeight = preWeight;

                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();

                // 自动读取校正值和验证重量
                if (adjValue == 0)
                    adjValue = My_ConPar.Correction.Correcting_Dissolving_Value;
                if (recheckWeight == 0)
                    recheckWeight = My_ConPar.Correction.Correcting_Dissolving_RWeight;
                result.AdjustValue = adjValue;
                result.RecheckWeight = recheckWeight;

                double verifyTime = recheckWeight / adjValue;

                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();

                // 加溶解剂操作
                var semiAutoResult = await SemiAutoHelperFactory.Current.AddSolventAsync(verifyTime);
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
                    await currentTask.CheckPauseAndCancelAsync();

                // 读取加溶解剂后重量，计算误差
                int roundDigits = My_Tool.BalanceStableReading.RetainDecimals();
                double postWeight = await My_Tool.BalanceStableReading.StableReadingAsync();
                result.PostWeight = postWeight;
                double verifyAddWeight = postWeight - preWeight;
                result.RealAddWeight = verifyAddWeight;
                double error = Math.Abs(verifyAddWeight - recheckWeight);
                double errorP = Math.Round(error / recheckWeight * 100.0, roundDigits);
                result.ErrorPercent = errorP;

                // 判断误差范围
                if (errorP <= My_ConPar.Other.AErr_Water)
                {
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "加溶解剂验证成功，误差在允许范围内";
                }
                else
                {
                    result.Code = My_Tool.Result.ResultCode.Failure;
                    result.Message = $"加溶解剂误差超限，误差为{errorP}%";
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
        /// 溶解剂校正流程（完整业务流程，返回详细业务结果）
        /// </summary>
        /// <returns>DMFCorrectionResult，包含详细业务信息</returns>
        private static async Task<DMFCorrectionResult> AddDMFCorrection()
        {
            var result = new DMFCorrectionResult();
            try
            {
                var balanceCT = My_Tool.AreaCoordinateFinder.TryGetBalanceCoordinateAsync();
                var currentTask = RobotTaskManager.Instance.CurrentTask as RobotBusinessTask<DMFCorrectionResult>;

                // 天平检查
                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();

                await My_Tool.BalanceStableReading.BalanceCheck();

                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();

                // 并发读取天平重量和机械手移动
                var readTask = My_Tool.BalanceStableReading.StableReadingAsync();
                var balanceCTR = await balanceCT;
                if (!balanceCTR.found)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Exception = new Exception("获取天平坐标异常");
                    result.Message = "获取天平坐标异常";
                    return result;
                }

                var moveTask = SemiAutoHelperFactory.Current.MoveToBalanceAsync(balanceCTR.x,balanceCTR.y);
                await Task.WhenAll(readTask, moveTask);

                double read1 = readTask.Result;
                result.PreWeight = read1;
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
                // 加溶解剂操作
                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();

                semiAutoResult = await SemiAutoHelperFactory.Current.AddSolventAsync(My_ConPar.Correction.Correcting_Dissolving_Time);
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



                // 读取加溶解剂后重量
                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();

                double read2 = await My_Tool.BalanceStableReading.StableReadingAsync();
                result.PostWeight = read2;
                double realAddWeight = read2 - read1;
                result.RealAddWeight = realAddWeight;
                if (realAddWeight <= 0)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "未检测到加溶解剂量，校正失败";
                    return result;
                }

                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();

                // 计算校正值
                int roundDigits = My_Tool.BalanceStableReading.RetainDecimals();
                double adjValue = Math.Round(realAddWeight / My_ConPar.Correction.Correcting_Dissolving_Time, roundDigits);
                result.AdjustValue = adjValue;

                // 验证加溶解剂
                var verifyResult = await VerifyAddDMFAsync(read2, 0, adjValue);
                result.ErrorPercent = verifyResult.ErrorPercent;
                result.RecheckWeight = verifyResult.RecheckWeight;
                switch (verifyResult.Code)
                {
                    case My_Tool.Result.ResultCode.Success:
                        {
                            // 校正通过，保存校正值
                            My_ConPar.Correction.Correcting_Dissolving_Value = adjValue;
                            var path = Path.Combine(Environment.CurrentDirectory, "Config", "Correction.ini");
                            My_File.ConfigHelper.WriteValue(path, "Correcting_Dissolving_Value", adjValue.ToString());
                            result.Code = My_Tool.Result.ResultCode.Success;
                            result.Message = "溶解剂校正成功，校正值已保存";
                            return result;
                        }
                    case My_Tool.Result.ResultCode.Failure:
                        {
                            result.Code = My_Tool.Result.ResultCode.Failure;
                            result.Message = $"校正误差超限，误差为{verifyResult.ErrorPercent}%";
                            return result;
                        }
                    case My_Tool.Result.ResultCode.Canceled:
                        {
                            result.Code = My_Tool.Result.ResultCode.Canceled;
                            result.Message = "任务被取消";
                            return result;
                        }
                    default:
                        {
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = verifyResult.Message;
                            result.Exception = verifyResult.Exception;
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
        /// 提交一次完整的溶解剂校正流程为原子任务到机械手调度中心，返回详细业务结果
        /// </summary>
        /// <returns>DMFCorrectionResult，包含详细业务信息</returns>
        public static async Task<DMFCorrectionResult> EnqueueDMFCorrectionAsync()
        {
            var task = new RobotBusinessTask<DMFCorrectionResult>
            {
                Priority = BigProcess.DMFCorrection *100,
                OriginalPriority = BigProcess.DMFCorrection * 100,
                BusinessType = BigProcess.RobotBusinessType.DMFCorrection,
                TaskName = "溶解剂校正",
                BusinessFlow = async () => await AddDMFCorrection()
            };

            try
            {
                var result = await RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<DMFCorrectionResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage("溶解剂校正异常", result.Message, btn =>
                            {
                                if (btn == "确认")
                                {
                                    tcs.SetResult(new DMFCorrectionResult
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
                            var tcs = new TaskCompletionSource<DMFCorrectionResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage("溶解剂校正询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueDMFCorrectionAsync();
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new DMFCorrectionResult
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
                return new DMFCorrectionResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                return new DMFCorrectionResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }

        /// <summary>
        /// 提交一次溶解剂验证流程为原子任务到机械手调度中心，返回详细业务结果
        /// </summary>
        /// <param name="preWeight">加溶解剂前天平重量，默认9999自动读取</param>
        /// <param name="recheckWeight">验证目标重量，默认0自动读取</param>
        /// <param name="adjValue">校正值，默认0自动读取</param>
        /// <returns>DMFCorrectionResult，包含详细业务信息</returns>
        public static async Task<DMFCorrectionResult> EnqueueDMFVerifyAsync(double preWeight = 9999, int recheckWeight = 0, double adjValue = 0)
        {
            var task = new RobotBusinessTask<DMFCorrectionResult>
            {
                Priority = BigProcess.VerifyAddDMF * 100,
                OriginalPriority = BigProcess.VerifyAddDMF * 100,
                BusinessType = BigProcess.RobotBusinessType.VerifyAddDMF,
                TaskName = $"溶解剂验证-{recheckWeight}g",
                BusinessFlow = async () => await VerifyAddDMFAsync(preWeight, recheckWeight, adjValue)
            };

            try
            {
                var result = await RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<DMFCorrectionResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"溶解剂验证{recheckWeight}克异常", result.Message, btn =>
                            {
                                if (btn == "确认")
                                {
                                    tcs.SetResult(new DMFCorrectionResult
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
                            var tcs = new TaskCompletionSource<DMFCorrectionResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"溶解剂验证{recheckWeight}克询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueDMFVerifyAsync(preWeight, recheckWeight, adjValue);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new DMFCorrectionResult
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
                return new DMFCorrectionResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                return new DMFCorrectionResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }
    }
}