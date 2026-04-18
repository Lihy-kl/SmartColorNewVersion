using SmartColor.My_ConPar.Order;
using SmartColor.My_DataBase;
using SmartColor.My_Form.BasicData;
using SmartColor.My_Form.Homepage;
using SmartColor.My_RobotManager;
using SmartColor.My_SemiAutoModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_AutomaticModule
{
    /// <summary>
    /// 母液瓶自检结果类型(成功，失败，异常，取消)
    /// </summary>
    public class BottleSelfResult
    {
        /// <summary>结果码，(成功，失败，异常，取消)</summary>
        public My_Tool.Result.ResultCode Code { get; set; }
        /// <summary>详细信息</summary>
        public string Message { get; set; }
        /// <summary>母液瓶号</summary>
        public int BottleNo { get; set; }
        /// <summary>校正值</summary>
        public double? AdjustValue { get; set; }
        /// <summary>自检1重量</summary>
        public double? RWeight1 { get; set; }
        /// <summary>自检2重量</summary>
        public double? RWeight2 { get; set; }
        /// <summary>自检3重量</summary>
        public double? RWeight3 { get; set; }
        /// <summary>自检4重量</summary>
        public double? RWeight4 { get; set; }
        /// <summary>当前校正重量</summary>
        public double? ConcentrationWeight { get; set; }
        /// <summary>数据库操作是否成功</summary>
        public bool? DbSuccess { get; set; }
        /// <summary>异常对象</summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// 母液瓶自检模块
    /// 只负责流程和结果信息，不负责弹窗交互
    /// </summary>
    internal static class BottleSelfRobotTask
    {
        /// <summary>
        /// 自检参数数组
        /// </summary>
        private static readonly double[] selfArray = new double[] { 0.5, 2, 5, 11 };

        /// <summary>
        /// 单瓶自检流程（原子任务，返回详细结果）
        /// </summary>
        /// <param name="bottleNo">母液瓶号</param>
        /// <returns>(成功，失败，异常，取消)</returns>
        private static async Task<BottleSelfResult> SingleBottle(int bottleNo)
        {
            var result = new BottleSelfResult { BottleNo = bottleNo };
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

                var currentTask = RobotTaskManager.Instance.CurrentTask as RobotBusinessTask<BottleSelfResult>;

                // 步骤1：并发查找瓶资料和移动到瓶位置
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                var findTask = Task.Run(() => TryGetSyringeTypeAndPulse(bottleNo));
                var bottleCTR = await My_Tool.AreaCoordinateFinder.TryGetBottleCoordinateAsync(bottleNo);
                if (!bottleCTR.found)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "未找到瓶坐标";
                    result.Exception = new Exception("未找到瓶坐标");
                    return result;
                }
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
                    case -6:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = "助剂资料未找到";
                        return result;
                    default:
                        break;
                }

                double currentWeight = findTask.Result.currentWeight;
                short syringeType = findTask.Result.syringeType;
                int z = findTask.Result.z;


                var semiAutoResult = moveTask.Result;
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


                // 步骤2：并发天平检查+稳定读取 和 抽液
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();


                var balanceTask = My_Tool.BalanceStableReading.CheckAndReadBalanceAsync();
                var balanceCT = My_Tool.AreaCoordinateFinder.TryGetBalanceCoordinateAsync();

                var aspirateTask = SemiAutoHelperFactory.Current.AspirateAsync(
                    z, My_ConPar.Other.DrainCount <= 0 ? 1 : My_ConPar.Other.DrainCount, syringeType, Convert.ToInt16(currentWeight));


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



                double initialWeight = balanceTask.Result;

                semiAutoResult = aspirateTask.Result;
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
                        break; ;
                }



                // 步骤3：移动到天平
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                var balanceCTR = await balanceCT;
                if (!balanceCTR.found)
                {

                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "未找到天平坐标";
                    result.Exception = new Exception("未找到天平坐标");
                    return result;

                }

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

                // 步骤4：分批注液和称重
                int rp = z + My_ConPar.Other.Z_BackPulse;
                var weights = new double[selfArray.Length];
                for (int i = 0; i < selfArray.Length; i++)
                {
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();

                    // 注液
                    rp -= (int)(selfArray[i] * findTask.Result.adjust);

                    semiAutoResult = await SemiAutoHelperFactory.Current.DispenseAsync(rp, findTask.Result.syringeType, 0);
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

                    // 读取稳定重量
                    double stableWeight = await My_Tool.BalanceStableReading.StableReadingAsync();
                    weights[i] = stableWeight - initialWeight;
                    initialWeight = stableWeight;

                    // 只有最后一次注液后才回母液瓶
                    if (i == selfArray.Length - 1)
                    {
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
                    }
                }

                // 步骤5：异步启动放针
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var releaseNeedleTask = SemiAutoHelperFactory.Current.ReleaseNeedleAsync(findTask.Result.syringeType);

                // 步骤6：读取最后一次稳定重量
                double finalWeight = await My_Tool.BalanceStableReading.StableReadingAsync();

                // 步骤7：结果赋值
                if (weights.Length > 0) result.RWeight1 = weights[0];
                if (weights.Length > 1) result.RWeight2 = weights[1];
                if (weights.Length > 2) result.RWeight3 = weights[2];
                if (weights.Length > 3) result.RWeight4 = weights[3];

                result.AdjustValue = findTask.Result.adjust;
                result.ConcentrationWeight = findTask.Result.adjustWeight;
                int roundDigits = My_Tool.BalanceStableReading.RetainDecimals();

                // 步骤8：判断是否失败
                bool fail = false;
                double allowErr = My_Tool.BottleAuxiliary.GetAllowErr(bottleNo);
                for (int i = 0; i < selfArray.Length; i++)
                {
                    double err = Math.Round(Math.Abs(weights[i] - selfArray[i]), roundDigits);

                    if (err > allowErr)
                    {
                        fail = true;
                        break;
                    }

                }

                // 步骤9：数据库操作
                try
                {
                    // 更新bottle_details
                    var updateDict = new Dictionary<string, object>
                    {
                        { BOTTLE_DETAILS.CurrentWeight, Math.Round(currentWeight - weights.Sum(), roundDigits) },
                        { BOTTLE_DETAILS.SelfChecking1, weights.Length > 0 ? Math.Round(weights[0], roundDigits).ToString() : "0" },
                        { BOTTLE_DETAILS.SelfChecking2, weights.Length > 1 ? Math.Round(weights[1], roundDigits).ToString() : "0" },
                        { BOTTLE_DETAILS.SelfChecking3, weights.Length > 2 ? Math.Round(weights[2], roundDigits).ToString() : "0" },
                        { BOTTLE_DETAILS.SelfChecking4, weights.Length > 3 ? Math.Round(weights[3], roundDigits).ToString() : "0" }
                    };
                    My_DataBase.SqlServer.Update(BOTTLE_DETAILS.TableName, updateDict, $"{BOTTLE_DETAILS.BottleNum} = {bottleNo}");



                    // 插入self_table
                    var insertDict = new Dictionary<string, object>
                    {
                        { SELF_TABLE.Date, DateTime.Now },
                        { SELF_TABLE.BottleNum, bottleNo },
                        { SELF_TABLE.SelfChecking1, weights.Length > 0 ? Math.Round(weights[0], roundDigits).ToString() : null },
                        { SELF_TABLE.SelfChecking2, weights.Length > 1 ? Math.Round(weights[1], roundDigits).ToString() : null },
                        { SELF_TABLE.SelfChecking3, weights.Length > 2 ? Math.Round(weights[2], roundDigits).ToString() : null },
                        { SELF_TABLE.SelfChecking4, weights.Length > 3 ? Math.Round(weights[3], roundDigits).ToString() : null },
                        { SELF_TABLE.CurrentAdjustWeight, findTask.Result.adjustWeight },
                        { SELF_TABLE.AdjustValue, findTask.Result.adjust },
                        { SELF_TABLE.Fail, fail ? 1 : 0 }
                    };
                    My_DataBase.SqlServer.Insert(SELF_TABLE.TableName, insertDict);

                    result.DbSuccess = true;
                }
                catch (Exception ex)
                {
                    result.DbSuccess = false;
                    My_File.Logger.Error("自检数据库操作异常", ex);
                }

                // 步骤10：等待放针完成
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
                if (!fail)
                {
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "校正成功";
                }
                else
                {
                    result.Code = My_Tool.Result.ResultCode.Failure;
                    result.Message = "校正误差超限";
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
        /// result=-4 校正值异常
        /// result=-5 代码异常
        /// result=-6 助剂资料未找到
        /// </returns>
        private static (int result, short syringeType, int z, double adjust, double adjustWeight, double currentWeight, string unit) TryGetSyringeTypeAndPulse(int bottleNo)
        {
            try
            {
                // 用工具类统一获取瓶资料
                var info = SmartColor.My_Tool.BottleAuxiliary.GetBottleInfo(bottleNo);
                if (info.Result != 0)
                    return (info.Result, 0, 0, 0, 0, 0, string.Empty);

                short syringeType = info.SyringeType;
                double adjust = info.Adjust;
                double adjustWeight = info.AdjustWeight;
                double currentWeight = info.CurrentWeight;
                string unit = info.UnitOfAccount;

                int maxP = syringeType == 0 ? My_ConPar.Other.S_MaxPulse : My_ConPar.Other.B_MaxPulse;
                int cP = 0;
                foreach (var d in selfArray)
                {
                    cP += Convert.ToInt32(d * adjust);
                }

                int z = cP + Convert.ToInt32(1 * adjust) - My_ConPar.Other.Z_BackPulse;
                if (z > maxP)
                    return (-3, 0, 0, 0, 0, 0, string.Empty);
                else if (z <= 0)
                    return (-4, 0, 0, 0, 0, 0, string.Empty);

                return (0, syringeType, z, adjust, adjustWeight, currentWeight, unit);
            }
            catch
            {
                return (-5, 0, 0, 0, 0, 0, string.Empty);
            }
        }



        /// <summary>
        /// 提交一次完整的母液瓶自检流程为原子任务到机械手调度中心
        /// </summary>
        /// <param name="bottleNo">母液瓶号</param>
        /// <returns>(成功，失败，异常，取消)</returns>
        public static async Task<BottleSelfResult> EnqueueBottleCorrectionAsync(int bottleNo)
        {
            var task = new RobotBusinessTask<BottleSelfResult>
            {
                Priority = BigProcess.BottleSelf * 100,
                OriginalPriority = BigProcess.BottleSelf * 100,
                BusinessType = BigProcess.RobotBusinessType.BottleSelf,
                TaskName = $"{bottleNo}号母液瓶自检",
                BusinessFlow = async () => await SingleBottle(bottleNo)
            };

            try
            {
                var result = await RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<BottleSelfResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{bottleNo}号母液瓶自检异常", result.Message, btn =>
                            {
                                if (btn == "确认")
                                {
                                    tcs.SetResult(new BottleSelfResult
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
                            var tcs = new TaskCompletionSource<BottleSelfResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{bottleNo}号母液瓶自检询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueBottleCorrectionAsync(bottleNo);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new BottleSelfResult
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
                return new BottleSelfResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消",
                    BottleNo = bottleNo
                };
            }
            catch (Exception ex)
            {
                return new BottleSelfResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex,
                    BottleNo = bottleNo
                };
            }
        }

        /// <summary>
        /// 批量母液瓶自检流程
        /// </summary>
        /// <param name="bottleNos">母液瓶号列表</param>
        /// <returns>字典，key为瓶号，value为BottleSelfResult</returns>
        public static async Task<Dictionary<int, BottleSelfResult>> EnqueueBatchBottleCorrectionAsync(List<int> bottleNos)
        {
            var results = new Dictionary<int, BottleSelfResult>();
            foreach (var bottleNo in bottleNos)
            {
                var result = await EnqueueBottleCorrectionAsync(bottleNo);
                results.Add(bottleNo, result);
            }
            return results;
        }
    }
}