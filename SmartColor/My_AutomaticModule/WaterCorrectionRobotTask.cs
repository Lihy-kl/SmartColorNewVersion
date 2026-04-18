using SmartColor.My_ConPar.Order;
using SmartColor.My_RobotManager;
using SmartColor.My_SemiAutoModule;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SmartColor.My_AutomaticModule
{
    

    /// <summary>
    /// 水校正结果类型
    /// 封装水校正/水验证流程的详细业务结果信息
    /// </summary>
    public class WaterCorrectionResult
    {
        /// <summary>
        /// 结果码
        /// 0=成功
        /// 1=失败
        /// -1=异常
        /// -2=取消
        /// </summary>
        public My_Tool.Result.ResultCode Code { get; set; }
        /// <summary>详细信息</summary>
        public string Message { get; set; }
        /// <summary>校正值</summary>
        public double? AdjustValue { get; set; }
        /// <summary>加水前重量</summary>
        public double? PreWeight { get; set; }
        /// <summary>加水后重量</summary>
        public double? PostWeight { get; set; }
        /// <summary>实际加水量</summary>
        public double? RealAddWeight { get; set; }
        /// <summary>误差百分比</summary>
        public double? ErrorPercent { get; set; }
        /// <summary>验证目标重量</summary>
        public double? RecheckWeight { get; set; }
        /// <summary>异常对象</summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// 水校正业务流程的原子任务封装工具类
    /// 只负责流程和详细结果，不负责弹窗交互
    /// </summary>
    public static class WaterCorrectionRobotTask
    {
        /// <summary>
        /// 验证加水流程（用于水校正/水验证），返回详细业务结果
        /// 业务失败原因：
        ///   1. 天平异常
        ///   2. 加水失败
        /// </summary>
        /// <param name="needGTBalance">是否需要移动到天平</param>
        /// <param name="preWeight">加水前天平的重量，默认9999表示需自动读取</param>
        /// <param name="recheckWeight">验证重量，默认0表示需自动读取</param>
        /// <param name="adjValue">校正值，默认0表示需自动读取</param>
        /// <returns>WaterCorrectionResult，包含详细业务信息</returns>
        private static async Task<WaterCorrectionResult> VerifyAddWaterAsync(bool needGTBalance ,double preWeight = 9999, int recheckWeight = 0, double adjValue = 0)
        {
            var result = new WaterCorrectionResult();
            try
            {
                var balanceCT = My_Tool.AreaCoordinateFinder.TryGetBalanceCoordinateAsync();
                var currentTask = RobotTaskManager.Instance.CurrentTask as RobotBusinessTask<WaterCorrectionResult>;
                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();

                // 自动读取加水前重量
                
                if (preWeight == 9999)
                {
                    if (currentTask != null)
                        await currentTask.CheckPauseAndCancelAsync();

                    // 天平检查
                    await My_Tool.BalanceStableReading.BalanceCheck();

                    // 并发读取天平重量和机械手移动
                    var readTask =  My_Tool.BalanceStableReading.StableReadingAsync();
                    var balanceCTR = await balanceCT;
                    if (!balanceCTR.found)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Exception = new Exception("获取天平坐标异常");
                        result.Message = "获取天平坐标异常";
                        return result;
                    }
                    var moveTask = SemiAutoHelperFactory.Current.MoveToBalanceAsync(balanceCTR.x, balanceCTR.y);
                    await Task.WhenAll(readTask, moveTask);
                    preWeight = readTask.Result;
                }
                else if(needGTBalance)
                {
                    if (currentTask != null)
                        await currentTask.CheckPauseAndCancelAsync();

                    // 天平检查
                    await My_Tool.BalanceStableReading.BalanceCheck();
                    var balanceCTR = await balanceCT;
                    if (!balanceCTR.found)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Exception = new Exception("获取天平坐标异常");
                        result.Message = "获取天平坐标异常";
                        return result;
                    }
                    var moveTask = await SemiAutoHelperFactory.Current.MoveToBalanceAsync(balanceCTR.x, balanceCTR.y);

                }

                result.PreWeight = preWeight;

                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();

                // 自动读取校正值和验证重量
                if (adjValue == 0)
                {
                    adjValue = My_ConPar.Correction.Correcting_Water_Value;
                    if(adjValue == 0)
                    {
                        //如果没有水校正值，则先执行一次水校正流程获取校正值
                        var addCTR = await AddWaterCorrection();
                        if(addCTR.Code != My_Tool.Result.ResultCode.Success)
                        {
                            return addCTR;
                        }
                        adjValue = (double)addCTR.AdjustValue;
                    }
                }
                    

               
                   
                if (recheckWeight == 0)
                    recheckWeight = My_ConPar.Correction.Correcting_Water_RWeight;
                result.AdjustValue = adjValue;
                result.RecheckWeight = recheckWeight;

                double verifyTime = recheckWeight / adjValue;

                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();

                // 加水操作
                var semiAutoResult = await SemiAutoHelperFactory.Current.AddWaterAsync(recheckWeight,verifyTime);
                switch (semiAutoResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Exception = semiAutoResult.Exception;
                        result.Message = semiAutoResult.Message;
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

                // 读取加水后重量，计算误差
                double postWeight = await My_Tool.BalanceStableReading.StableReadingAsync();
                result.PostWeight = postWeight;
                double verifyAddWeight = postWeight - preWeight;
                result.RealAddWeight = verifyAddWeight;
                double error = Math.Abs(verifyAddWeight - recheckWeight);
                double errorP = Math.Round(error / recheckWeight * 100.0, 1);
                result.ErrorPercent = errorP;

                // 判断误差范围
                if (errorP <= My_ConPar.Other.AErr_Water)
                {
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "加水验证成功，误差在允许范围内";
                }
                else
                {
                    result.Code = My_Tool.Result.ResultCode.Failure;
                    result.Message = $"加水误差超限，误差为{errorP}%";
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
        /// 水校正流程（完整业务流程，返回详细业务结果）
        /// 业务失败原因：
        ///   1. 天平异常
        ///   2. 机械手移动失败
        ///   3. 加水失败
        ///   4. 未检测到加水量
        ///   5. 校正误差超限
        /// </summary>
        /// <returns>WaterCorrectionResult，包含详细业务信息</returns>
        public static async Task<WaterCorrectionResult> AddWaterCorrection()
        {
            var result = new WaterCorrectionResult();
            try
            {
                var balanceCT = My_Tool.AreaCoordinateFinder.TryGetBalanceCoordinateAsync();
                var currentTask = RobotTaskManager.Instance.CurrentTask as RobotBusinessTask<WaterCorrectionResult>;
                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();

                // 天平检查 和读取加水前重量
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
                var moveTask = SemiAutoHelperFactory.Current.MoveToBalanceAsync(balanceCTR.x, balanceCTR.y);
                await Task.WhenAll(readTask, moveTask);

                double read1 = readTask.Result;
                result.PreWeight = read1;

              var  semiAutoResult = await moveTask;
                switch (semiAutoResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Exception = semiAutoResult.Exception;
                        result.Message = semiAutoResult.Message;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = semiAutoResult.Message;
                        return result;

                    default:
                        break;
                }

                // 加水操作
                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();


                semiAutoResult = await SemiAutoHelperFactory.Current.AddWaterAsync(999,My_ConPar.Correction.Correcting_Water_Time);
                switch (semiAutoResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Exception = semiAutoResult.Exception;
                        result.Message = semiAutoResult.Message;
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

                // 读取加水后重量
                double read2 = await My_Tool.BalanceStableReading.StableReadingAsync();
                result.PostWeight = read2;
                double realAddWeight = read2 - read1;
                result.RealAddWeight = realAddWeight;
                if (realAddWeight <= 0)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "未检测到加水量，校正失败";
                    return result;
                }

                if (currentTask != null)
                    await currentTask.CheckPauseAndCancelAsync();

                // 计算校正值
                int roundDigits = My_Tool.BalanceStableReading.RetainDecimals();
                double adjValue = Math.Round(realAddWeight / (My_ConPar.Correction.Correcting_Water_Time), roundDigits);
                result.AdjustValue = adjValue;

                // 验证加水
                var verifyResult = await VerifyAddWaterAsync(false, read2, 0, adjValue);
                result.ErrorPercent = verifyResult.ErrorPercent;
                result.RecheckWeight = verifyResult.RecheckWeight;
                if (verifyResult.Code == My_Tool.Result.ResultCode.Exception ||
                    verifyResult.Code == My_Tool.Result.ResultCode.Canceled)
                {
                    // 异常或取消直接返回
                    result.Code = verifyResult.Code;
                    result.Message = verifyResult.Message;
                    result.Exception = verifyResult.Exception;
                    return result;
                }

                switch (verifyResult.Code)
                {
                    case My_Tool.Result.ResultCode.Success:
                        // 验证通过
                        {
                            My_ConPar.Correction.Correcting_Water_Value = adjValue;
                            var path = Path.Combine(Environment.CurrentDirectory, "Config", "Correction.ini");
                            My_File.ConfigHelper.WriteValue(path, "Correcting_Water_Value", adjValue.ToString());
                            result.Code = 0;
                            result.Message = "水校正成功，校正值已保存";
                            return result;
                        }

                    case My_Tool.Result.ResultCode.Failure:
                        // 验证失败
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
        /// 提交一次完整的水校正流程为原子任务到机械手调度中心，返回详细业务结果
        /// </summary>
        /// <param name="robotBusinessType">机械手业务类型</param>
        /// <param name="priority">任务优先级</param>
        /// <returns>WaterCorrectionResult，包含详细业务信息</returns>
        public static async Task<WaterCorrectionResult> EnqueueWaterCorrectionAsync(
            BigProcess.RobotBusinessType robotBusinessType = BigProcess.RobotBusinessType.WaterCorrection,
            int priority = 0)
        {
            var task = new RobotBusinessTask<WaterCorrectionResult>
            {
                Priority =priority == 0 ? BigProcess.WaterCorrection *100: priority *100,
                OriginalPriority = priority == 0 ? BigProcess.WaterCorrection * 100 : priority * 100,
                BusinessType = robotBusinessType,
                TaskName = "水校正",
                BusinessFlow = async () => await AddWaterCorrection()
            };

            try
            {
                var result = await RobotTaskManager.Instance.EnqueueTask(task);
               switch(result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<WaterCorrectionResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage("水校正异常", result.Message, btn =>
                            {
                                if (btn == "确认")
                                {
                                    tcs.SetResult(new WaterCorrectionResult
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
                            var tcs = new TaskCompletionSource<WaterCorrectionResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage("水校正询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueWaterCorrectionAsync(robotBusinessType,priority);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new WaterCorrectionResult
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
                return new WaterCorrectionResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                return new WaterCorrectionResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }

        /// <summary>
        /// 提交一次水验证流程为原子任务到机械手调度中心，返回详细业务结果
        /// </summary>
        /// <param name="needGTBalance">是否需要移动到天平</param>
        /// <param name="preWeight">加水前天平重量，默认9999自动读取</param>
        /// <param name="recheckWeight">验证目标重量，默认0自动读取</param>
        /// <param name="adjValue">校正值，默认0自动读取</param>
        /// <param name="robotBusinessType">机械手业务类型</param>
        /// <param name="priority">任务优先级</param>
        /// <returns>WaterCorrectionResult，包含详细业务信息</returns>
        public static async Task<WaterCorrectionResult> EnqueueWaterVerifyAsync(
            bool needGTBalance, double preWeight = 9999, int recheckWeight = 0, double adjValue = 0,
            BigProcess.RobotBusinessType robotBusinessType = BigProcess.RobotBusinessType.VerifyAddWater,
            int priority = 0)
        {
            recheckWeight = recheckWeight == 0 ? My_ConPar.Correction.Correcting_Water_RWeight : recheckWeight;

            ////判断任务中心有没有优先级比他高的任务，如果有,则需要再读取天平初始数据，如果没有则不需要
            //if(preWeight != 9999)
            //{
            //    bool hasHigherPriorityTask = RobotTaskManager.Instance.HasHigherPriorityTask( priority *100);
            //    if(hasHigherPriorityTask) preWeight = 9999;
            //}

            preWeight = 9999;

            var task = new RobotBusinessTask<WaterCorrectionResult>
            {
                Priority =priority == 0? BigProcess.VerifyAddWater * 100 : priority*100,
                OriginalPriority = priority == 0 ? BigProcess.VerifyAddWater * 100 : priority * 100,
                BusinessType = robotBusinessType,
                TaskName = $"水验证-{recheckWeight}g",
                BusinessFlow = async () => await VerifyAddWaterAsync(needGTBalance, preWeight, recheckWeight, adjValue)
            };

            try
            {
                var result = await RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<WaterCorrectionResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"加水验证{recheckWeight}克异常", result.Message, btn =>
                            {
                                if (btn == "确认")
                                {
                                    tcs.SetResult(new WaterCorrectionResult
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
                            var tcs = new TaskCompletionSource<WaterCorrectionResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"加水验证{recheckWeight}克询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueWaterVerifyAsync(needGTBalance,preWeight,recheckWeight,adjValue,robotBusinessType,priority);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new WaterCorrectionResult
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
                return new WaterCorrectionResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                return new WaterCorrectionResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }
    }
}