using SmartColor.My_ConPar.Order;
using SmartColor.My_RobotManager;
using SmartColor.My_SemiAutoModule;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartColor.My_AutomaticModule
{
    /// <summary>
    /// 洗针任务结果类型
    /// </summary>
    public class WashSyringeResult
    {
        /// <summary>结果码</summary>
        public My_Tool.Result.ResultCode Code { get; set; }
        /// <summary>详细信息</summary>
        public string Message { get; set; }
        /// <summary>瓶号（0为公共针筒）</summary>
        public int BottleNo { get; set; }
        /// <summary>异常对象</summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// 母液瓶/公共针筒洗针任务
    /// 只负责流程和结果信息，不负责弹窗交互
    /// </summary>
    internal static class WashSyringeRobotTask
    {
        /// <summary>
        /// 单瓶/公共针筒洗针流程（原子任务，返回详细结果）
        /// </summary>
        /// <param name="bottleNo">母液瓶号，0为公共针筒</param>
        /// <param name="syringeType">针筒类型</param>
        /// <returns>洗针结果</returns>
        private static async Task<WashSyringeResult> SingleWashSyringe(int bottleNo)
        {
            var result = new WashSyringeResult { BottleNo = bottleNo };
            try
            {
                // 步骤1：并发动作检查和坐标查找
                var checkTask = SemiAutoHelperFactory.Current.ActionCheckAsync();
                Task<(bool found, int x, int y)> coordTask;
                if (bottleNo != 0)
                    coordTask = AreaCoordinateFinder.TryGetBottleCoordinateAsync(bottleNo);
                else
                    coordTask = AreaCoordinateFinder.TryGetWashCoordinateAsync();
                var info = SmartColor.My_Tool.BottleAuxiliary.GetBottleInfo(bottleNo);
                if (info.Result != 0)
                {
                    return new WashSyringeResult
                    {
                        Code = My_Tool.Result.ResultCode.Exception,
                        Message = $"未找到{bottleNo}号母液瓶信息",
                        Exception = new Exception($"未找到{bottleNo}号母液瓶信息")

                    };
                }
                short syringeType = info.SyringeType;
                await Task.WhenAll(checkTask, coordTask);

                var checkResult = checkTask.Result;
                var coordResult = coordTask.Result;

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

                if (!coordResult.found)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = bottleNo != 0 ? "未找到母液瓶坐标" : "未找到公共针筒坐标";
                    result.Exception = new Exception(result.Message);
                    return result;
                }

                int x = coordResult.x, y = coordResult.y;

                // 步骤2：移动到抓取点
                SemiAutoResult moveToGrabResult;
                if (bottleNo != 0)
                    moveToGrabResult = await SemiAutoHelperFactory.Current.MoveToBottleAsync(bottleNo, x, y, 2,1);
                else
                    moveToGrabResult = await SemiAutoHelperFactory.Current.MoveToWashAsync(x, y);

                switch (moveToGrabResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = moveToGrabResult.Message;
                        result.Exception = moveToGrabResult.Exception;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = moveToGrabResult.Message;
                        return result;
                    default:
                        break;
                }

                // 步骤3：机械手抓取
                var graspResult = await SemiAutoHelperFactory.Current.RobotHandGraspingAsync(
                   2, syringeType);
                switch (graspResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = graspResult.Message;
                        result.Exception = graspResult.Exception;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = graspResult.Message;
                        return result;
                    default:
                        break;
                }

                // 步骤4：移动到洗针区
                var washCTR2 = await AreaCoordinateFinder.TryGetWashCoordinateAsync();
                if (!washCTR2.found)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "未找到洗针区坐标";
                    result.Exception = new Exception(result.Message);
                    return result;
                }
                var moveResult = await SemiAutoHelperFactory.Current.MoveToWashAsync(washCTR2.x, washCTR2.y);
                switch (moveResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = moveResult.Message;
                        result.Exception = moveResult.Exception;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = moveResult.Message;
                        return result;
                    default:
                        break;
                }

                // 步骤5：洗针
                var washResult = await SemiAutoHelperFactory.Current.WashSyringeAsync(syringeType);
                switch (washResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = washResult.Message;
                        result.Exception = washResult.Exception;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = washResult.Message;
                        return result;
                    default:
                        break;
                }

                // 步骤6：回到抓取点
                SemiAutoResult backResult;
                if (bottleNo != 0)
                    backResult = await SemiAutoHelperFactory.Current.MoveToBottleAsync(bottleNo, x, y, 1,0);
                else
                    backResult = await SemiAutoHelperFactory.Current.MoveToWashAsync(x, y);

                switch (backResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = backResult.Message;
                        result.Exception = backResult.Exception;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = backResult.Message;
                        return result;
                    default:
                        break;
                }

                // 步骤7：放针
                var releaseResult = await SemiAutoHelperFactory.Current.ReleaseNeedleAsync(syringeType);
                switch (releaseResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = releaseResult.Message;
                        result.Exception = releaseResult.Exception;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = releaseResult.Message;
                        return result;
                    default:
                        break;
                }

                // 成功
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "洗针完成";
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
        /// 提交一次完整的洗针流程为原子任务到机械手调度中心
        /// </summary>
        /// <param name="bottleNo">母液瓶号，0为公共针筒</param>
        /// <param name="syringeType">针筒类型</param>
        /// <returns>洗针结果</returns>
        public static async Task<WashSyringeResult> EnqueueWashSyringeAsync(int bottleNo)
        {
            var task = new RobotBusinessTask<WashSyringeResult>
            {
                Priority = BigProcess.WashSyringe * 100,
                OriginalPriority = BigProcess.WashSyringe * 100,
                BusinessType = BigProcess.RobotBusinessType.WashSyringe,
                TaskName = bottleNo != 0 ? $"{bottleNo}号母液瓶洗针" : "公共针筒洗针",
                BusinessFlow = async () => await SingleWashSyringe(bottleNo)
            };

            try
            {
                var result = await RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<WashSyringeResult>();
                            MessageEventManager.Instance.RequestShowMessage(
                                $"{bottleNo}号母液瓶洗针异常", result.Message, btn =>
                                {
                                    if (btn == "确认")
                                    {
                                        tcs.SetResult(new WashSyringeResult
                                        {
                                            Code = My_Tool.Result.ResultCode.Success
                                        });
                                    }
                                },
                                new[] { "确认" }, "确认"
                            );
                            return await tcs.Task;
                        }
                    case My_Tool.Result.ResultCode.NeedInteraction:
                        {
                            var tcs = new TaskCompletionSource<WashSyringeResult>();
                            MessageEventManager.Instance.RequestShowMessage(
                                $"{bottleNo}号母液瓶洗针询问", result.Message, async btn =>
                                {
                                    if (btn == "重试")
                                    {
                                        var retryResult = await EnqueueWashSyringeAsync(bottleNo);
                                        tcs.SetResult(retryResult);
                                    }
                                    else if (btn == "退出")
                                    {
                                        tcs.SetResult(new WashSyringeResult
                                        {
                                            Code = My_Tool.Result.ResultCode.Canceled,
                                            Message = "用户选择退出，任务已取消"
                                        });
                                    }
                                },
                                new[] { "重试", "退出" }, "重试"
                            );
                            return await tcs.Task;
                        }
                    default:
                        return result;
                }
            }
            catch (TaskCanceledException)
            {
                return new WashSyringeResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消",
                    BottleNo = bottleNo
                };
            }
            catch (Exception ex)
            {
                return new WashSyringeResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex,
                    BottleNo = bottleNo
                };
            }
        }

        /// <summary>
        /// 批量洗针流程
        /// </summary>
        /// <param name="bottleNos">瓶号列表（0为公共针筒）</param>
        /// <returns>字典，key为瓶号，value为WashSyringeResult</returns>
        public static async Task<Dictionary<int, WashSyringeResult>> EnqueueBatchWashSyringeAsync(List<int> bottleNos)
        {
            var results = new Dictionary<int, WashSyringeResult>();
            foreach (var bottleNo in bottleNos)
            {
                var result = await EnqueueWashSyringeAsync(bottleNo);
                results.Add(bottleNo, result);
            }
            return results;
        }
    }
}