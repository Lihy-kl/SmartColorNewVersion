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
    /// 母液瓶加水调试结果类型
    /// </summary>
    public class BottleAddWaterDebugResult
    {
        /// <summary>结果码</summary>
        public My_Tool.Result.ResultCode Code { get; set; }
        /// <summary>详细信息</summary>
        public string Message { get; set; }
        /// <summary>母液瓶号</summary>
        public int BottleNo { get; set; }
        /// <summary>加水量</summary>
        public double? AddedWater { get; set; }
        /// <summary>初始量</summary>
        public double? InitialVolume { get; set; }
        /// <summary>最大允许量</summary>
        public double? MaxVolume { get; set; }
        /// <summary>数据库操作是否成功</summary>
        public bool? DbSuccess { get; set; }
        /// <summary>异常对象</summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// 母液瓶加水调试模块
    /// 只负责流程和结果信息，不负责弹窗交互
    /// </summary>
    internal static class BottleAddWaterDebug
    {
        /// <summary>
        /// 单瓶加水调试流程（原子任务，返回详细结果）
        /// </summary>
        /// <param name="bottleNo">母液瓶号</param>
        /// <returns>调试结果</returns>
        public static async Task<BottleAddWaterDebugResult> SingleBottle(int bottleNo)
        {
            var result = new BottleAddWaterDebugResult { BottleNo = bottleNo };
            try
            {
                // 步骤1：启动检查
                var checkTask = SemiAutoHelperFactory.Current.ActionCheckAsync();
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

                var currentTask = RobotTaskManager.Instance.CurrentTask as RobotBusinessTask<BottleAddWaterDebugResult>;
                var bottleCT = My_Tool.AreaCoordinateFinder.TryGetBottleCoordinateAsync(bottleNo);

                // 步骤2：移动到母液瓶位置
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

                var moveTask = SemiAutoHelperFactory.Current.MoveToBottleAsync(bottleNo, bottleCTR.x, bottleCTR.y, 2, 1);
                var findTask = Task.Run(() => TryGetSyringeType(bottleNo));
                await Task.WhenAll(moveTask, findTask);

                var moveResult = await moveTask;
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

                // 步骤3：抽液
                bool hasSyringe = true;
                short syringeType = findTask.Result.syringeType;
              
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                var aspirateResult = await SemiAutoHelperFactory.Current.AspirateAsync(0, 0, syringeType, 0);
                switch (aspirateResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        {
                            if (aspirateResult.Message != null && aspirateResult.Message.Contains("未发现针筒"))
                            {
                                hasSyringe = false;
                                break;
                            }
                            else
                            {
                                result.Code = My_Tool.Result.ResultCode.Exception;
                                result.Message = aspirateResult.Message;
                                result.Exception = aspirateResult.Exception;
                                return result;
                            }
                        }
                        
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = aspirateResult.Message;
                        return result;
                    default:
                        break;
                }

                // 步骤4：查找母液瓶资料，查当前量和最大量，计算加水量并加水
                DataRow data = findTask.Result.data;
                if (data == null)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "未找到瓶资料";
                    return result;
                }
                double currentVolume = Convert.ToDouble(data[BOTTLE_DETAILS.CurrentWeight] ?? 0);
                double maxVolume = Convert.ToDouble(data[BOTTLE_DETAILS.AllowMaxWeight] ?? 0);
                result.InitialVolume = currentVolume;
                result.MaxVolume = maxVolume;

                double addWater = maxVolume - currentVolume;
                if (addWater <= 0)
                {
                    result.Code = My_Tool.Result.ResultCode.Failure;
                    result.Message = "当前量已达最大，无需加水";
                    result.AddedWater = 0;
                    return result;
                }
                result.AddedWater = addWater;

               //加水
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                double addWaterTime = addWater/ My_ConPar.Correction.Correcting_Water_Value;
                var addWaterResult = await SemiAutoHelperFactory.Current.AddWaterAsync(addWater, addWaterTime);
                switch (addWaterResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = addWaterResult.Message;
                        result.Exception = addWaterResult.Exception;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = addWaterResult.Message;
                        return result;
                    default:
                        break;
                }


                // 更新数据库
                bool dbOk = false;
                try
                {
                    SqlServer.Update(BOTTLE_DETAILS.TableName,
                        new Dictionary<string, object>
                        {
                            { BOTTLE_DETAILS.CurrentWeight, maxVolume }
                        },
                        $"{BOTTLE_DETAILS.BottleNum} = {bottleNo}");
                    dbOk = true;
                }
                catch (Exception ex)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "数据库更新失败：" + ex.Message;
                    result.Exception = ex;
                    result.DbSuccess = false;
                    return result;
                }
                result.DbSuccess = dbOk;

                // 步骤5：如果有针筒，则需要放针
                if (hasSyringe)
                {
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();

                    var releaseNeedleResult = await SemiAutoHelperFactory.Current.ReleaseNeedleAsync(syringeType);
                    switch (releaseNeedleResult.Level)
                    {
                        case SemiAutoResultCode.Exception:
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = releaseNeedleResult.Message;
                            result.Exception = releaseNeedleResult.Exception;
                            return result;
                        case SemiAutoResultCode.NeedInteraction:
                            result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                            result.Message = releaseNeedleResult.Message;
                            return result;
                        default:
                            break;
                    }
                }

                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "加水调试完成";
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
        /// 查找母液瓶资料并判断是否有针筒
        /// </summary>
        /// <param name="bottleNo">瓶号</param>
        /// <returns>hasSyringe=true表示有针筒</returns>
        private static (short syringeType, DataRow data) TryGetSyringeType(int bottleNo)
        {
            try
            {
                var rows = BottleData.Bottle_details.Select($"{BOTTLE_DETAILS.BottleNum} = {bottleNo}");
                if (rows.Length == 0)
                    return ( 0, null);

                DataRow data = rows[0];
                string s = data[BOTTLE_DETAILS.SyringeType]?.ToString();
                if (string.IsNullOrEmpty(s))
                    return ( 0, data);

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
                        return ( 0, data);
                }
                return ( syringeType, data);
            }
            catch
            {
                return ( 0, null);
            }
        }

        /// <summary>
        /// 提交一次完整的母液瓶加水调试流程为原子任务到机械手调度中心
        /// </summary>
        /// <param name="bottleNo">母液瓶号</param>
        /// <returns>BottleAddWaterDebugResult，包含详细业务信息</returns>
        public static async Task<BottleAddWaterDebugResult> EnqueueBottleAddWaterDebugAsync(int bottleNo)
        {
            var task = new RobotBusinessTask<BottleAddWaterDebugResult>
            {
                Priority = int.MaxValue,
                OriginalPriority = int.MaxValue,
                BusinessType = BigProcess.RobotBusinessType.Debug,
                TaskName = $"{bottleNo}号母液瓶加水调试",
                BusinessFlow = async () => await SingleBottle(bottleNo)
            };

            try
            {
                var result = await RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<BottleAddWaterDebugResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{bottleNo}号母液瓶加水调试异常", result.Message, btn =>
                            {
                                if (btn == "确认")
                                {
                                    tcs.SetResult(new BottleAddWaterDebugResult
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
                            var tcs = new TaskCompletionSource<BottleAddWaterDebugResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{bottleNo}号母液瓶加水调试异常", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueBottleAddWaterDebugAsync(bottleNo);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new BottleAddWaterDebugResult
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
                return new BottleAddWaterDebugResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消",
                    BottleNo = bottleNo
                };
            }
            catch (Exception ex)
            {
                My_File.Logger.Error("EnqueueBottleAddWaterDebugAsync异常", ex);
                return new BottleAddWaterDebugResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex,
                    BottleNo = bottleNo
                };
            }
        }

        /// <summary>
        /// 批量母液瓶加水调试流程
        /// </summary>
        /// <param name="bottleNos">母液瓶号列表</param>
        /// <returns>字典，key为瓶号，value为BottleAddWaterDebugResult</returns>
        public static async Task<Dictionary<int, BottleAddWaterDebugResult>> EnqueueBatchBottleAddWaterDebugAsync(List<int> bottleNos)
        {
            var results = new Dictionary<int, BottleAddWaterDebugResult>();
            foreach (var bottleNo in bottleNos)
            {
                var result = await EnqueueBottleAddWaterDebugAsync(bottleNo);
                results.Add(bottleNo, result);
            }
            return results;
        }
    }
}