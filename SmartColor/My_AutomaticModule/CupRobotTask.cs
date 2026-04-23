using com.google.zxing;

using SmartColor.My_ConPar.Area.Clip;
using SmartColor.My_ConPar.Area.Drop;
using SmartColor.My_ConPar.Order;
using SmartColor.My_ConPar.Order.DyeingProcess;
using SmartColor.My_Control;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Form.BasicData;
using SmartColor.My_Form.Homepage;
using SmartColor.My_RobotManager;
using SmartColor.My_SemiAutoModule;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Result = SmartColor.My_Tool.Result;

namespace SmartColor.My_AutomaticModule
{
    /// <summary>
    /// 染色进程相关任务（冷行、温控、加药等）
    /// </summary>
    internal class CupRobotTask
    {
        #region 任务结果类型定义
        public class TaskResult
        {
            public My_Tool.Result.ResultCode Code { get; set; }
            public string Message { get; set; }
            public Exception Exception { get; set; }
        }
        #endregion

        #region 1. 冷行任务

        /// <summary>
        /// 冷行任务预处理：提前查找所有需要冷行的杯的坐标和状态
        /// </summary>
        private static async Task<object> PrecomputeColdRunAsync(int cupNo)
        {
            // 1. 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 2. 过滤需要冷行的杯（开盖、当前步未完成、正在使用）
            var needColdRunCups = cupInfos
                .Where(info => info.IsUsing == 1 && info.CoverStatus == 2 && info.CurrentStepFinish == 0)
                .ToList();

            // 3. 并发准备每个杯子的坐标和状态
            var cupTasks = needColdRunCups.Select(async info =>
            {
                var cupObjTask = CupCommManager.Instance.FindCupByCupNumAsync(info.CupNum);
                var cupCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, false);
                var lidCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, true);
                int cno = info.CupNum;
                int coverStatus = info.CoverStatus ?? 1;
                var cupCTR = await cupCTTask;
                var lidCTR = await lidCTTask;
                var cupObj = await cupObjTask;
                return new
                {
                    Info = info,
                    CupNum = cno,
                    CupCTR = cupCTR,
                    LidCTR = lidCTR,
                    CupObj = cupObj
                };
            }).ToArray();

            var cupDataList = await Task.WhenAll(cupTasks);
            return cupDataList;
        }

        /// <summary>
        /// 单杯冷行任务（原子业务流程）
        /// </summary>
        /// <param name="cupNo">杯号</param>
        /// <returns>任务结果</returns>
        public static async Task<CupRobotTask.TaskResult> ColdRunAsync(List<int> cupNos, object precomputed = null)
        {
            var result = new TaskResult()
            {
                Code = My_Tool.Result.ResultCode.Success,
                Message = "冷行关盖完成"
            };

            dynamic[] cupDataList;
            if (precomputed is Array arr)
            {
                cupDataList = arr.Cast<dynamic>().ToArray();
            }
            else
            {
                // 兼容未预处理的情况
                cupDataList = cupNos.Select(cupNo => new { CupNum = cupNo, CupCTR = (object)null, LidCTR = (object)null }).ToArray();
            }

            foreach (var cupData in cupDataList)
            {
                int cupNo = cupData.CupNum;
                var cupCTR = cupData.CupCTR;
                var lidCTR = cupData.LidCTR;
                var ctCup = cupData.CupObj;
                _ = Task.Run(() =>
                 {
                     CupTempRecorder.Get(cupNo).RecordStep("冷行");
                 });

                var result1 = await CloseLidAsync(cupNo, cupCTR, lidCTR, ctCup);
                if (result1.Code != My_Tool.Result.ResultCode.Success)
                {
                    result = result1;
                }
            }
            return result;
        }

        /// <summary>
        /// 提交冷行任务到任务中心
        /// </summary>
        /// <param name="cupNo">杯号</param>
        /// <returns>任务结果</returns>
        public static async Task<CupRobotTask.TaskResult> EnqueueColdRunAsync(int cupNo, int dyeType)
        {
            var result = new TaskResult();

            // 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 只对正在使用、开盖、当前步未完成的杯进行关盖
            var needCloseCups = new List<int>();
            foreach (var info in cupInfos)
            {
                if (info.IsUsing == 1 && info.CoverStatus == 2 && info.CurrentStepFinish == 0)
                {
                    needCloseCups.Add(info.CupNum);
                }
                else if (info.IsUsing == 1 && (info.CoverStatus == null || info.CoverStatus == 0))
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"{info.CupNum}杯盖状态未知";
                    result.Exception = new Exception($"{info.CupNum}杯盖状态未知");
                    return result;
                }
                else if (info.IsUsing == 1 && info.CoverStatus == 1 && info.CurrentStepFinish == 0)
                {
                    _ = Task.Run(() =>
                    {
                        CupTempRecorder.Get(info.CupNum).RecordStep("冷行");
                    });
                }
            }

            // 如果主副杯都为关盖或未使用或已完成，直接返回
            if (needCloseCups.Count == 0)
            {
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "主副杯均为关盖或未使用或当前步已完成，无需关盖";
                return result;
            }



            var task = new SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>
            {
                Priority = dyeType == 1 ?
           SmartColor.My_ConPar.Order.BigProcess.DyeingProcess * 100 + SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.ColdRunProcess :
           SmartColor.My_ConPar.Order.BigProcess.PostProcess * 100 + SmartColor.My_ConPar.Order.PostProcess.PostProcess.ColdRunProcess,
                OriginalPriority = dyeType == 1 ?
           SmartColor.My_ConPar.Order.BigProcess.DyeingProcess * 100 + SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.ColdRunProcess :
           SmartColor.My_ConPar.Order.BigProcess.PostProcess * 100 + SmartColor.My_ConPar.Order.PostProcess.PostProcess.ColdRunProcess,
                BusinessType = dyeType == 1 ? SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DyeingProcess :
           SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.PostProcess,
                TaskName = $"冷行-{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}",
                PrecomputeAsync = () => PrecomputeColdRunAsync(cupNo)
            };
            task.BusinessFlow = async () =>
            {
                var precomputed = task.PrecomputeResult;
                return await ColdRunAsync(needCloseCups, precomputed);
            };

            try
            {


                result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯冷行异常", result.Message, async btn =>
                            {
                                if (btn == "确认")
                                {
                                    var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                    foreach (var c in needCloseCups)
                                    {

                                        int coverStatus = 0;
                                        if (mainInfo.CupNum == c)
                                        {
                                            coverStatus = mainInfo.CoverStatus ?? 0;
                                        }
                                        else
                                        {
                                            coverStatus = subInfo.CoverStatus ?? 0;
                                        }

                                        if (coverStatus == 1) continue;


                                        SqlServer.Update(
                                            CUP_DETAILS.TableName,
                                            new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 1 } },
                                            $"{CUP_DETAILS.CupNum} = @CupNo",
                                            new SqlParameter("@CupNo", c)
                                        );



                                        var area = CupCommManager.Instance.FindCupAreaByCupNum(c);
                                        if (area != null)
                                        {
                                            var comm = CupCommManager.Instance.GetCommObject(area);
                                            if (comm != null)
                                            {
                                                await comm.SyncCoverStatus(c);
                                            }
                                            area.OnCupDataReceived(c);
                                        }
                                    }



                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯冷行询问", result.Message, async btn =>
                            {

                                if (btn == "重试")
                                {
                                    if (result.Message.Contains("关盖时撑盖开失败"))
                                    {
                                        var retryResult = await EnqueueSupportCoverAsync(cupNo, task.Priority, dyeType == 1 ? SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DyeingProcess :
           SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.PostProcess);
                                        if (retryResult.Code == Result.ResultCode.Success)
                                        {
                                            retryResult = await EnqueueColdRunAsync(cupNo, dyeType);
                                            tcs.SetResult(retryResult);
                                        }
                                        else
                                        {
                                            tcs.SetResult(retryResult);
                                        }


                                    }
                                    else
                                    {
                                        var retryResult = await EnqueueColdRunAsync(cupNo, dyeType);
                                        tcs.SetResult(retryResult);
                                    }
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                Logger.Error("EnqueueColdRunAsync：发生异常。", ex);
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }
        #endregion

        #region 2. 温控任务

        /// <summary>
        /// 温控任务预处理：提前查找所有需要温控的杯的坐标和状态
        /// </summary>
        private static async Task<object> PrecomputeTemperatureControlAsync(int cupNo)
        {
            // 1. 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 2. 过滤需要温控的杯（开盖、当前步未完成、正在使用）
            var needTempControlCups = cupInfos
                .Where(info => info.IsUsing == 1 && info.CoverStatus == 2 && info.CurrentStepFinish == 0)
                .ToList();

            // 3. 并发准备每个杯子的坐标和状态
            var cupTasks = needTempControlCups.Select(async info =>
            {
                var cupObjTask = CupCommManager.Instance.FindCupByCupNumAsync(info.CupNum);
                var cupCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, false);
                var lidCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, true);
                int cno = info.CupNum;

                var cupCTR = await cupCTTask;
                var lidCTR = await lidCTTask;
                var cupObj = await cupObjTask;
                return new
                {
                    Info = info,
                    CupNum = cno,

                    CupCTR = cupCTR,
                    LidCTR = lidCTR,
                    CupObj = cupObj
                };
            }).ToArray();

            var cupDataList = await Task.WhenAll(cupTasks);
            return cupDataList;
        }

        /// <summary>
        /// 单杯温控任务（原子业务流程）
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> TemperatureControlAsync(List<int> cupNos, object precomputed = null)
        {
            var result = new TaskResult()
            {
                Code = My_Tool.Result.ResultCode.Success,
                Message = "温控关盖完成"
            };

            dynamic[] cupDataList;
            if (precomputed is Array arr)
            {
                cupDataList = arr.Cast<dynamic>().ToArray();
            }
            else
            {
                // 兼容未预处理的情况
                cupDataList = cupNos.Select(cupNo => new { CupNum = cupNo, CupCTR = (object)null, LidCTR = (object)null }).ToArray();
            }

            foreach (var cupData in cupDataList)
            {
                int cupNo = cupData.CupNum;
                var cupCTR = cupData.CupCTR;
                var lidCTR = cupData.LidCTR;
                var ctCup = cupData.CupObj;
                _ = Task.Run(() =>
                {
                    CupTempRecorder.Get(cupNo).RecordStep("温控");
                });

                var result1 = await CloseLidAsync(cupNo, cupCTR, lidCTR, ctCup);
                if (result1.Code != My_Tool.Result.ResultCode.Success)
                {
                    result = result1;
                }
            }
            return result;
        }

        /// <summary>
        /// 提交温控任务到任务中心
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> EnqueueTemperatureControlAsync(int cupNo, int dyeType)
        {
            var result = new TaskResult();

            // 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 只对正在使用、开盖、当前步未完成的杯进行关盖
            var needCloseCups = new List<int>();
            foreach (var info in cupInfos)
            {
                if (info.IsUsing == 1 && info.CoverStatus == 2 && info.CurrentStepFinish == 0)
                {
                    needCloseCups.Add(info.CupNum);
                }
                else if (info.IsUsing == 1 && (info.CoverStatus == null || info.CoverStatus == 0))
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"{info.CupNum}杯盖状态未知";
                    result.Exception = new Exception($"{info.CupNum}杯盖状态未知");
                    return result;
                }
                else if (info.IsUsing == 1 && info.CoverStatus == 1 && info.CurrentStepFinish == 0)
                {
                    _ = Task.Run(() => { CupTempRecorder.Get(info.CupNum).RecordStep("温控"); });
                }
            }

            // 如果主副杯都为关盖或未使用或已完成，直接返回
            if (needCloseCups.Count == 0)
            {
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "主副杯均为关盖或未使用或当前步已完成，无需关盖";
                return result;
            }

            int priority = int.MaxValue;
            My_ConPar.Order.BigProcess.RobotBusinessType businessType = BigProcess.RobotBusinessType.Debug;
            if (dyeType == 1)
            {
                priority = BigProcess.DyeingProcess * 100 + SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.TemperatureControlProcess;
                businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DyeingProcess;
            }
            else if (dyeType == 2)
            {
                priority = BigProcess.PostProcess * 100 + SmartColor.My_ConPar.Order.PostProcess.PostProcess.TemperatureControlProcess;
                businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.PostProcess;
            }
            else
            {
                if (FixDyeTypeForPreWash(cupNo))
                {
                    priority = BigProcess.DropProcess * 100;
                    businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DropProcess;
                }
            }

            var task = new SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>
            {
                Priority = priority,
                OriginalPriority = priority,
                BusinessType = businessType,
                TaskName = $"温控-{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}",
                PrecomputeAsync = () => PrecomputeTemperatureControlAsync(cupNo)
            };
            task.BusinessFlow = async () =>
            {
                var precomputed = task.PrecomputeResult;
                return await TemperatureControlAsync(needCloseCups, precomputed);
            };

            try
            {
                result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯温控异常", result.Message, async btn =>
                            {
                                if (btn == "确认")
                                {
                                    var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                    foreach (var c in needCloseCups)
                                    {

                                        int coverStatus = 0;
                                        if (mainInfo.CupNum == c)
                                        {
                                            coverStatus = mainInfo.CoverStatus ?? 0;
                                        }
                                        else
                                        {
                                            coverStatus = subInfo.CoverStatus ?? 0;
                                        }

                                        if (coverStatus == 1) continue;


                                        SqlServer.Update(
                                            CUP_DETAILS.TableName,
                                            new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 1 } },
                                            $"{CUP_DETAILS.CupNum} = @CupNo",
                                            new SqlParameter("@CupNo", c)
                                        );

                                        var area = CupCommManager.Instance.FindCupAreaByCupNum(c);
                                        if (area != null)
                                        {
                                            var comm = CupCommManager.Instance.GetCommObject(area);
                                            if (comm != null)
                                            {
                                                await comm.SyncCoverStatus(c);
                                            }
                                            area.OnCupDataReceived(c);
                                        }
                                    }



                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯温控询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    if (result.Message.Contains("关盖时撑盖开失败"))
                                    {
                                        var retryResult = await EnqueueSupportCoverAsync(cupNo, task.Priority, businessType);
                                        if (retryResult.Code == Result.ResultCode.Success)
                                        {
                                            retryResult = await EnqueueTemperatureControlAsync(cupNo, dyeType);
                                            tcs.SetResult(retryResult);
                                        }
                                        else
                                        {
                                            tcs.SetResult(retryResult);
                                        }
                                    }
                                    else
                                    {
                                        var retryResult = await EnqueueTemperatureControlAsync(cupNo, dyeType);
                                        tcs.SetResult(retryResult);
                                    }
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                Logger.Error("EnqueueTemperatureControlAsync：发生异常。", ex);
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }

        #endregion

        #region 3. 加药任务
        /// <summary>
        /// 加药主流程，支持主副杯、天平复检、母液瓶检查、注液高度处理
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> AddChemicalAsync(int cupNo, bool noCheckExpired, bool noCheckLow)
        {
            var result = new TaskResult()
            {
                Code = Result.ResultCode.Success
            };
            var currentTask = SmartColor.My_RobotManager.RobotTaskManager.Instance.CurrentTask as SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>;
            var cupTask = CupCommManager.Instance.FindCupByCupNumAsync(cupNo);
            try
            {
                // 1. 获取主副杯信息，只处理当前需要加药的杯

                var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
                if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

                // 2. 过滤只需要加药的杯
                var needCups = new List<CupChemicalInfo>();
                foreach (var info in cupInfos)
                {

                    if (info.IsUsing == 1 && info.TechnologyName.Contains("加") && info.TechnologyName != "加水" && info.CurrentStepFinish == 0)
                    {
                        var cupChemicalInfo = await GetCupChemicalInfoWithCheck(info.CupNum, noCheckExpired, noCheckLow);
                        if (cupChemicalInfo.TaskResult.Code != My_Tool.Result.ResultCode.Success)
                        {
                            result.Code = cupChemicalInfo.TaskResult.Code;
                            result.Message = cupChemicalInfo.TaskResult.Message;
                            result.Exception = cupChemicalInfo.TaskResult.Exception;

                        }


                        _ = Task.Run(() => { CupTempRecorder.Get(info.CupNum).RecordStep(info.TechnologyName); });
                        needCups.Add(cupChemicalInfo);
                    }
                    else
                    {


                    }
                }

                if (result.Code != My_Tool.Result.ResultCode.Success)
                {
                    return result;
                }




                // 2.2 等当前杯锁止上信号通
                var ctCup = await cupTask;
                if (ctCup == null)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"未找到{cupNo}号杯控件";
                    result.Exception = new Exception($"未找到{cupNo}号杯控件");
                    return result;
                }

                var wr = await CupAuxiliary.WaitLockUpOk(cupNo);
                if (!wr)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"等待{cupNo}号杯锁止上信号异常";
                    result.Exception = new Exception($"等待{cupNo}号杯锁止上信号异常");
                    return result;
                }


                // 3. 按母液瓶分组（同瓶合并，不同瓶分开）
                var bottleGroups = needCups.GroupBy(x => x.BottleNo);
                foreach (var group in bottleGroups)
                {
                    var checkTask = SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.ActionCheckAsync();
                    var checkResult = await checkTask;
                    switch (checkResult.Level)
                    {
                        case SemiAutoResultCode.Exception:
                            return new TaskResult
                            {
                                Code = My_Tool.Result.ResultCode.Exception,
                                Message = checkResult.Message,
                                Exception = checkResult.Exception
                            };

                        case SemiAutoResultCode.NeedInteraction:
                            return new TaskResult
                            {
                                Code = My_Tool.Result.ResultCode.NeedInteraction,
                                Message = checkResult.Message
                            };

                        default:
                            break;
                    }
                    var groupList = group.ToList();
                    var groupResult = await AddChemicalForCupsWithBalance(groupList, currentTask);
                    if (groupResult.Code != My_Tool.Result.ResultCode.Success)
                        return groupResult;
                }



                // 5. 查找下一步工艺，并只发送一次给HMI，同时遍历needCups可用于更新每个杯的步号
                if (needCups.Count > 0)
                {
                    var cupNoForNext = needCups[0].CupNo;
                    var dt = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {cupNoForNext}");
                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        int headID = int.TryParse(row[CUP_DETAILS.HeadID]?.ToString() ?? "0", out int hid) ? hid : 0;
                        int stepNo = int.TryParse(row[CUP_DETAILS.StepNum]?.ToString() ?? "0", out int sn) ? sn : 0;

                        var nextStepInfo = My_Tool.CupAuxiliary.GetNextStepInfo(headID, stepNo);


                        // 如果下一步还是加药，则递归继续加药
                        if (nextStepInfo.TechnologyName.Contains("加") && nextStepInfo.TechnologyName != "加水")
                        {
                            // 遍历needCups，更新每个杯的步号（如有需要）
                            foreach (var info in needCups)
                            {
                                SqlServer.Update(
                                    CUP_DETAILS.TableName,
                                    new Dictionary<string, object>
                                    {
                                        { CUP_DETAILS.StepNum, nextStepInfo.StepNum },
                                        { CUP_DETAILS.StepStartTime, DateTime.Now  },
                                        { CUP_DETAILS.CurrentStepFinish, 0    }
                                    },
                                    $"{CUP_DETAILS.CupNum} = @CupNo",
                                    new SqlParameter("@CupNo", info.CupNo)
                                );

                                var area1 = CupCommManager.Instance.FindCupAreaByCupNum(info.CupNo);
                                if (area1 != null)
                                {
                                    area1.OnCupDataReceived(info.CupNo);
                                }


                            }

                            // 只发一次给HMI（主副杯共用工艺）
                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNoForNext);
                            if (area != null)
                            {
                                var comm = CupCommManager.Instance.GetCommObject(area);
                                if (comm != null)
                                {
                                    // 4. 通知HMI主副杯都完成

                                    _ = Task.Run(async () =>
                                    {
                                        await RunTableMan.InsertAsync(new Dictionary<string, object>
                                        {
                                            [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNoForNext}号配液杯加药完成触发发送下一步信号"
                                        }, DateTime.Now);
                                        await comm.SendNextStep(cupNoForNext, nextStepInfo);
                                        await NotifyHMIAddChemicalFinish(needCups.Select(x => x.CupNo).ToList());
                                    });





                                }
                            }


                            return await AddChemicalAsync(cupNoForNext, false, false);
                        }
                        else
                        {

                            if (nextStepInfo.TechnologyName == "冷行" ||
                            nextStepInfo.TechnologyName == "温控" ||
                            nextStepInfo.TechnologyName == "搅拌")
                            {
                                // 这些工艺需要关盖，先关盖

                                foreach (var info in needCups)
                                {
                                    // 查询当前杯盖状态
                                    var drCover = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {info.CupNo}");
                                    int coverStatus = 0;
                                    if (drCover.Rows.Count > 0)
                                    {
                                        var rowCover = drCover.Rows[0];
                                        coverStatus = Convert.ToInt16(rowCover.Table.Columns.Contains(CUP_DETAILS.CoverStatus) ? rowCover[CUP_DETAILS.CoverStatus]?.ToString() : "1");
                                    }

                                    if (coverStatus != 1)
                                    {
                                        var closeLidResult = await CloseLidAsync(info.CupNo);
                                        if (closeLidResult.Code != My_Tool.Result.ResultCode.Success)
                                        {
                                            result.Code = closeLidResult.Code;
                                            result.Message = closeLidResult.Message;
                                            result.Exception = closeLidResult.Exception;

                                        }
                                    }
                                }

                                if (result.Code != Result.ResultCode.Success)
                                {
                                    return result;
                                }
                            }


                            // 遍历needCups，更新每个杯的步号（如有需要）
                            foreach (var info in needCups)
                            {
                                SqlServer.Update(
                                    CUP_DETAILS.TableName,
                                     new Dictionary<string, object>
                                    {
                                            { CUP_DETAILS.StepNum, nextStepInfo.StepNum },
                                            { CUP_DETAILS.StepStartTime, DateTime.Now  },
                                            { CUP_DETAILS.CurrentStepFinish, 0 }
                                    },
                                    $"{CUP_DETAILS.CupNum} = @CupNo",
                                    new SqlParameter("@CupNo", info.CupNo)
                                );

                                var area1 = CupCommManager.Instance.FindCupAreaByCupNum(info.CupNo);
                                if (area1 != null)
                                {
                                    area1.OnCupDataReceived(info.CupNo);
                                }
                            }

                            // 只发一次给HMI（主副杯共用工艺）
                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNoForNext);
                            if (area != null)
                            {
                                var comm = CupCommManager.Instance.GetCommObject(area);
                                if (comm != null)
                                {
                                    // 4. 通知HMI主副杯都完成
                                    _ = Task.Run(async () =>
                                    {
                                        await RunTableMan.InsertAsync(new Dictionary<string, object>
                                        {
                                            [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNoForNext}号配液杯加药完成触发发送下一步信号"
                                        }, DateTime.Now);
                                        await comm.SendNextStep(cupNoForNext, nextStepInfo);
                                        await NotifyHMIAddChemicalFinish(needCups.Select(x => x.CupNo).ToList());
                                    });
                                }
                            }

                            result.Code = My_Tool.Result.ResultCode.Success;
                            result.Message = "加药完成";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error("AddChemicalAsync：发生异常。", ex);
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = "加药异常：" + ex.Message;
                result.Exception = ex;
            }
            return result;
        }

        /// <summary>
        /// 获取单杯加药相关信息，并做母液瓶过期、液量、校正等检查
        /// </summary>
        private static async Task<CupChemicalInfo> GetCupChemicalInfoWithCheck(int cupNo, bool noCheckExpired, bool noCheckLow)
        {
            try
            {
                var dr = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {cupNo}");
                if (dr.Rows.Count == 0)
                    return CupChemicalInfo.Fail(cupNo,
                        new TaskResult
                        {
                            Code = My_Tool.Result.ResultCode.Exception,
                            Message = $"{cupNo}杯不存在",
                            Exception = new Exception($"{cupNo}杯不存在")
                        });
                var row = dr.Rows[0];
                int stepNo = int.TryParse(row[CUP_DETAILS.StepNum]?.ToString() ?? "0", out int sn) ? sn : 0;
                int headID = int.TryParse(row[CUP_DETAILS.HeadID]?.ToString() ?? "0", out int hid) ? hid : 0;
                if (stepNo == 0 || headID == 0)
                    return CupChemicalInfo.Fail(cupNo,
                        new TaskResult
                        {
                            Code = My_Tool.Result.ResultCode.Exception,
                            Message = $"{cupNo}杯未找到配方或步号",
                            Exception = new Exception($"{cupNo}杯未找到配方或步号")
                        });
                var dyeDetailDt = SqlServer.Select(DYE_DETAILS.TableName, $"{DYE_DETAILS.HeadID} = {headID} AND {DYE_DETAILS.StepNum} = {stepNo}");
                if (dyeDetailDt.Rows.Count == 0)
                    return CupChemicalInfo.Fail(cupNo, new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = $"{cupNo}杯未找到配方步骤" });
                var dyeRow = dyeDetailDt.Rows[0];
                double addWeight = 0;
                try
                {
                    addWeight = Convert.ToDouble(dyeRow[DYE_DETAILS.ObjectDropWeight] ?? "0");
                }
                catch (Exception ex)
                {

                    return CupChemicalInfo.Fail(cupNo,
                        new TaskResult
                        {
                            Code = My_Tool.Result.ResultCode.Exception,
                            Message = $"{cupNo}杯加药量转换异常",
                            Exception = ex
                        });
                }
                double waterWeight = Convert.ToDouble(dyeRow.Table.Columns.Contains(DYE_DETAILS.ObjectWaterWeight) ? dyeRow[DYE_DETAILS.ObjectWaterWeight]?.ToString() : "0");
                int bottleNo = Convert.ToInt32(dyeRow[DYE_DETAILS.BottleNum] ?? "0");
                if (bottleNo == 0)
                    return CupChemicalInfo.Fail(cupNo,
                        new TaskResult
                        {
                            Code = My_Tool.Result.ResultCode.Exception,
                            Message = $"{cupNo}杯未找到加药瓶号",
                            Exception = new Exception($"{cupNo}杯未找到加药瓶号")
                        });
                var bottleInfo = My_DataBase.BottleData.Bottle_details.Select($"{BOTTLE_DETAILS.BottleNum} = {bottleNo}").FirstOrDefault();
                if (bottleInfo == null)
                    return CupChemicalInfo.Fail(cupNo,
                        new TaskResult
                        {
                            Code = My_Tool.Result.ResultCode.Exception,
                            Message = $"{cupNo}杯加药瓶{bottleNo}不存在",
                            Exception = new Exception($"{cupNo}杯加药瓶{bottleNo}不存在")
                        });

                // 检查母液瓶过期
                if (My_ConPar.Choices.DripCheckExpired == 1)
                {
                    DateTime expireDate = Convert.ToDateTime(bottleInfo[BOTTLE_DETAILS.BrewingData] ?? DateTime.MinValue.ToString());
                    string assistantCode = bottleInfo[BOTTLE_DETAILS.AssistantCode]?.ToString() ?? "";
                    var assistantInfo = My_DataBase.AssistantData.Assistant_details.Select($"{ASSISTANT_DETAILS.AssistantCode} = '{assistantCode}'").FirstOrDefault();
                    int termOfValidity = assistantInfo != null ? Convert.ToInt32(assistantInfo[ASSISTANT_DETAILS.TermOfValidity] ?? "0") : 0;
                    if (expireDate.AddHours(termOfValidity) < DateTime.Now)
                    {
                        if (My_ConPar.Choices.DripAllowExpired == 0)
                        {
                            return CupChemicalInfo.Fail(cupNo,
                                new TaskResult
                                {
                                    Code = My_Tool.Result.ResultCode.Exception,
                                    Message = $"{bottleNo}号母液瓶已过期,请重新开料后点'确认'",

                                });
                        }
                        else
                        {
                            if (!noCheckExpired)
                            {
                                return CupChemicalInfo.Fail(cupNo,
                                new TaskResult
                                {
                                    Code = My_Tool.Result.ResultCode.Exception,
                                    Message = $"{bottleNo}号母液瓶已过期,继续加药请点'确认'",
                                });
                            }
                        }
                    }
                }
                // 检查母液瓶液量
                double cw = Convert.ToDouble(bottleInfo[BOTTLE_DETAILS.CurrentWeight] ?? "0");
                if (My_ConPar.Choices.DripCheckLow == 1 && cw <= My_ConPar.Other.Bottle_MinWeight)
                {
                    if (My_ConPar.Choices.DripAllowLow == 0)
                    {
                        return CupChemicalInfo.Fail(cupNo,
                              new TaskResult
                              {
                                  Code = My_Tool.Result.ResultCode.Exception,
                                  Message = $"{bottleNo}号母液瓶液量过低,请重新开料后点继续",

                              });
                    }
                    else
                    {
                        if (!noCheckLow)
                        {
                            return CupChemicalInfo.Fail(cupNo,
                              new TaskResult
                              {
                                  Code = My_Tool.Result.ResultCode.Exception,
                                  Message = $"{bottleNo}号母液瓶液量过低,继续加药请点'确认'",

                              });
                        }
                    }

                }
                // 校正
                int adjustSuccess = Convert.ToInt32(bottleInfo[BOTTLE_DETAILS.AdjustSuccess] ?? "0");
                if (adjustSuccess == 0)
                {
                    var adjustResult = await SmartColor.My_AutomaticModule.BottleCorrectionRobotTask.SingleBottle(bottleNo);
                    if (adjustResult.Code != My_Tool.Result.ResultCode.Success)
                        return CupChemicalInfo.Fail(cupNo,
                             new TaskResult
                             {
                                 Code = adjustResult.Code,
                                 Message = adjustResult.Message,
                                 Exception = adjustResult.Exception

                             });
                }
                // 针筒参数
                var syringeTypeStr = bottleInfo[BOTTLE_DETAILS.SyringeType]?.ToString();
                short syringeType = syringeTypeStr == "大针筒" ? (short)1 : (short)0;
                double adjust = Convert.ToDouble(bottleInfo[BOTTLE_DETAILS.AdjustValue] ?? "0");
                var lastUseTimeObj = bottleInfo[BOTTLE_DETAILS.LastUseTime];
                DateTime lastUseTime = (lastUseTimeObj != null && lastUseTimeObj != DBNull.Value)
                    ? Convert.ToDateTime(lastUseTimeObj)
                    : DateTime.MinValue;
                int evacuateSpan = Convert.ToInt32(bottleInfo[BOTTLE_DETAILS.EvacuateSpan] ?? "0");
                int rw = syringeType == 0 ? My_ConPar.Correction.Correcting_S_Weight : My_ConPar.Correction.Correcting_B_Weight;
                int purgeCount = lastUseTime.AddHours(evacuateSpan) > DateTime.Now ? 0 : My_ConPar.Other.DrainCount;

                // 2.1. 发送开始加药通知HMI
                // var ctCup = await CupCommManager.Instance.FindCupByCupNumAsync(cupNo);
                //if (ctCup.LockStatus != 1)
                //{
                var cupArea = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                if (cupArea != null)
                {
                    var comm = CupCommManager.Instance.GetCommObject(cupArea);
                    if (comm != null)
                    {
                        bool result = await comm.SendAddChemicaStart(cupNo);

                        if (!result)
                        {
                            return CupChemicalInfo.Fail(cupNo,
                                new TaskResult
                                {
                                    Code = My_Tool.Result.ResultCode.Exception,
                                    Message = $"通知HMI{cupNo}号杯开始加药失败,锁止上一直不到位",
                                });
                        }
                    }
                }
                // }


                // 是否需要开盖（注液高度参数）
                var (dispenseHeight, openLidResult) = await EnsureCupOpenLid(cupNo, headID, stepNo);
                if (openLidResult.Code != My_Tool.Result.ResultCode.Success)
                {
                    return CupChemicalInfo.Fail(cupNo, openLidResult);
                }

                return CupChemicalInfo.Ok(cupNo, addWeight, waterWeight, bottleNo, syringeType, adjust, rw, purgeCount, dispenseHeight);
            }
            catch (Exception ex)
            {
                Logger.Error("GetCupChemicalInfoWithCheck：发生异常。", ex);
                return CupChemicalInfo.Fail(cupNo, new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "获取加药信息异常",
                    Exception = ex
                });
            }
        }

        /// <summary>
        /// 判断并处理开盖，返回注液高度参数（0=开盖，2=关盖）
        /// </summary>
        private static async Task<(short, TaskResult)> EnsureCupOpenLid(int cupNo, int headID, int stepNo)
        {
            // 查找工艺参数

            var ctCupTask = CupCommManager.Instance.FindCupByCupNumAsync(cupNo);
            var dyeDetailDt = SqlServer.Select(DYE_DETAILS.TableName, $"{DYE_DETAILS.HeadID} = {headID} AND {DYE_DETAILS.StepNum} = {stepNo}");
            if (dyeDetailDt.Rows.Count == 0) return ((short)0, new TaskResult()
            {
                Code = My_Tool.Result.ResultCode.Exception,
                Message = "未找到工艺参数",
                Exception = new Exception("未找到工艺参数")
            });
            var dyeDetailRow = dyeDetailDt.Rows[0];
            string code = dyeDetailRow[DYE_DETAILS.Code]?.ToString();
            int dyeType = 0;
            if (dyeDetailRow.Table.Columns.Contains(DYE_DETAILS.DyeType))
                int.TryParse(dyeDetailRow[DYE_DETAILS.DyeType]?.ToString(), out dyeType);

            var processRows = SmartColor.My_DataBase.DyeingData.Dyeing_process?.Select(
                $"{DYEING_PROCESS.Code} = '{code}' AND {DYEING_PROCESS.Type} = {dyeType}");
            bool needOpenLid = true;
            if (processRows != null && processRows.Length > 0)
            {
                var processRow = processRows[0];
                int openMedicine = 0;
                if (processRow.Table.Columns.Contains(DYEING_PROCESS.OpenMedicine))
                    int.TryParse(processRow[DYEING_PROCESS.OpenMedicine]?.ToString(), out openMedicine);
                if (openMedicine == 1)
                    needOpenLid = false;
            }

            // 查杯盖状态
            var ctCup = await ctCupTask;

            TaskResult taskResult = new TaskResult()
            {
                Code = My_Tool.Result.ResultCode.Success
            };

            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            int coverStatus = 0;
            if (mainCup.CupNum == cupNo)
            {
                coverStatus = mainCup.CoverStatus ?? 0;
            }
            else
            {
                coverStatus = subCup.CoverStatus ?? 0;
            }
            if (coverStatus == 1 && needOpenLid)
            {
                var wr = await CupAuxiliary.WaitLockUpOk(cupNo);
                if (!wr)
                {
                    return ((short)0, new TaskResult()
                    {
                        Code = My_Tool.Result.ResultCode.Exception,
                        Message = $"等待{cupNo}号杯锁止上信号异常",
                        Exception = new Exception($"等待{cupNo}号杯锁止上信号异常")
                    });
                }


                taskResult = await OpenLidAsync(cupNo);
            }

            // 返回注液高度参数
            return ((short)(needOpenLid ? 0 : 2), taskResult);
        }

        private class CupPulseInfo
        {
            public int CupNo;
            public int Pulse;
            public double ObjectDropWeight;
            public double WaterWeight;
            public short DispenseHeight;

        }



        /// <summary>
        /// 合并抽液/注液（同瓶），天平复检，放针，数据库更新，注液高度处理
        /// 参考SingleSyringeDropAsync流程，增加加水逻辑
        /// </summary>
        //private static async Task<TaskResult> AddChemicalForCupsWithBalance(
        //     List<CupChemicalInfo> cupInfos,
        //     SmartColor.My_RobotManager.RobotBusinessTask<TaskResult> currentTask,
        //     Task updateTask = null)
        //{
        //    // 安全转换辅助方法
        //    double ToDoubleSafe(object value)
        //    {
        //        if (value == null || value == DBNull.Value) return 0;
        //        double result;
        //        return double.TryParse(value.ToString(), out result) ? result : 0;
        //    }


        //    int bottleNo = cupInfos[0].BottleNo;
        //    short syringeType = cupInfos[0].SyringeType;
        //    double adjust = cupInfos[0].Adjust;
        //    int rw = cupInfos[0].RW;
        //    int purgeCount = cupInfos[0].PurgeCount;

        //    // 1. 计算每个杯子剩余脉冲
        //    var remainPulseDict = cupInfos.ToDictionary(
        //        x => x.CupNo,
        //        x => Convert.ToInt32(Math.Round(x.AddWeight * adjust))
        //    );

        //    // 2. 计算本轮最大可抽脉冲
        //    int maxPulse = syringeType == 0 ? My_ConPar.Other.S_MaxPulse : My_ConPar.Other.B_MaxPulse;
        //    int backPulse = My_ConPar.Other.Z_BackPulse;
        //    int minPulse = Convert.ToInt32(rw * adjust) + Convert.ToInt32(1 * adjust);
        //    int availablePulse = maxPulse + backPulse - minPulse;

        //    // 3. 本轮分配脉冲到各杯
        //    var currentDropData = new List<CupPulseInfo>();
        //    int totalPulse = 0;
        //    if (My_ConPar.Choices.DripFull == 1)
        //    {
        //        // 满量程抽液
        //        foreach (var cup in cupInfos)
        //        {
        //            int remain = remainPulseDict[cup.CupNo];
        //            if (remain <= 0) continue;
        //            int leftPulse = availablePulse - totalPulse;
        //            if (leftPulse <= 0) break;
        //            int thisPulse = Math.Min(remain, leftPulse);
        //            if (thisPulse > 0)
        //            {
        //                currentDropData.Add(new CupPulseInfo
        //                {
        //                    CupNo = cup.CupNo,
        //                    Pulse = thisPulse,
        //                    ObjectDropWeight = cup.AddWeight,
        //                    WaterWeight = cup.WaterWeight,
        //                    DispenseHeight = cup.DispenseHeight
        //                });
        //                remainPulseDict[cup.CupNo] -= thisPulse;
        //                totalPulse += thisPulse;
        //                if (totalPulse >= availablePulse)
        //                    break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // 精准抽液
        //        foreach (var cup in cupInfos)
        //        {
        //            int remain = remainPulseDict[cup.CupNo];
        //            if (remain <= 0) continue;
        //            int leftPulse = availablePulse - totalPulse;
        //            if (leftPulse <= 0) break;
        //            int thisPulse = Math.Min(remain, leftPulse);
        //            if (thisPulse > 0)
        //            {
        //                currentDropData.Add(new CupPulseInfo
        //                {
        //                    CupNo = cup.CupNo,
        //                    Pulse = thisPulse,
        //                    ObjectDropWeight = cup.AddWeight,
        //                    WaterWeight = cup.WaterWeight,
        //                    DispenseHeight = cup.DispenseHeight
        //                });
        //                remainPulseDict[cup.CupNo] -= thisPulse;
        //                totalPulse += thisPulse;
        //            }
        //        }
        //    }
        //    if (currentDropData.Count == 0)
        //    {
        //        if (updateTask != null) await updateTask;
        //        return new TaskResult { Code = My_Tool.Result.ResultCode.Success, Message = "加药完成" };
        //    }
        //    int z = totalPulse + (Convert.ToInt32(rw * adjust) + Convert.ToInt32(1 * adjust) - backPulse);

        //    // 4. 并发查找母液瓶、天平、首个杯坐标
        //    var bottleCTRTask = My_Tool.AreaCoordinateFinder.TryGetBottleCoordinateAsync(bottleNo);
        //    var balanceCTTask = My_Tool.AreaCoordinateFinder.TryGetBalanceCoordinateAsync();
        //    var cupCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(currentDropData[0].CupNo, false);

        //    var bottleCTR = await bottleCTRTask;
        //    if (!bottleCTR.found)
        //        return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = $"未找到母液瓶{bottleNo}坐标", Exception = new Exception($"未找到母液瓶{bottleNo}坐标") };
        //    if (currentTask != null)
        //        await currentTask.CheckPauseOnlyAsync();


        //    // 5. 移动到母液瓶并抽液
        //    var moveResult = SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.MoveToBottleAsync(bottleNo, bottleCTR.x, bottleCTR.y, 2, 1);
        //    var semiAutoResult = await moveResult;
        //    if (semiAutoResult.Level == SemiAutoResultCode.Exception)
        //        return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = semiAutoResult.Message, Exception = semiAutoResult.Exception };
        //    if (semiAutoResult.Level == SemiAutoResultCode.NeedInteraction)
        //        return new TaskResult { Code = My_Tool.Result.ResultCode.NeedInteraction, Message = semiAutoResult.Message };

        //    if (currentTask != null)
        //        await currentTask.CheckPauseOnlyAsync();
        //    semiAutoResult = await SemiAutoHelperFactory.Current.AspirateAsync(z, purgeCount, syringeType);
        //    My_DataBase.SqlServer.Update(BOTTLE_DETAILS.TableName,
        //        new Dictionary<string, object> { { BOTTLE_DETAILS.LastUseTime, DateTime.Now } },
        //        $"{BOTTLE_DETAILS.BottleNum} = {bottleNo}");
        //    if (semiAutoResult.Level == SemiAutoResultCode.Exception)
        //        return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = semiAutoResult.Message, Exception = semiAutoResult.Exception };
        //    if (semiAutoResult.Level == SemiAutoResultCode.NeedInteraction)
        //        return new TaskResult { Code = My_Tool.Result.ResultCode.NeedInteraction, Message = semiAutoResult.Message };
        //    if (updateTask != null) await updateTask;

        //    // 6. 分批注液（加水+加药）
        //    int rp = z + backPulse;
        //    Task<double> balanceTask = null;
        //    var cupCT = cupCTTask;
        //    int j = 0;
        //    for (int i = 0; i < currentDropData.Count; i++)
        //    {
        //        if (currentTask != null)
        //            await currentTask.CheckPauseOnlyAsync();

        //        var drop = currentDropData[i];
        //        // 注液到最后两杯时并发启动天平读数
        //        if (i >= currentDropData.Count - 2 && balanceTask == null)
        //            balanceTask = My_Tool.BalanceStableReading.CheckAndReadBalanceAsync();

        //        var cupCTR = await cupCT;
        //        if (!cupCTR.found)
        //            return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = $"未找到{drop.CupNo}号杯坐标", Exception = new Exception($"未找到{drop.CupNo}号杯坐标") };
        //        semiAutoResult = await SemiAutoHelperFactory.Current.MoveToCupAsync(drop.CupNo, 0, cupCTR.x, cupCTR.y, 1, 0);
        //        if (semiAutoResult.Level == SemiAutoResultCode.Exception)
        //            return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = semiAutoResult.Message, Exception = semiAutoResult.Exception };
        //        if (semiAutoResult.Level == SemiAutoResultCode.NeedInteraction)
        //            return new TaskResult { Code = My_Tool.Result.ResultCode.NeedInteraction, Message = semiAutoResult.Message };

        //        // 加水（如有）
        //        if (drop.WaterWeight > 0)
        //        {
        //            double addTime = drop.WaterWeight / SmartColor.My_ConPar.Correction.Correcting_Water_Value;
        //            var addWater = await SemiAutoHelperFactory.Current.AddWaterAsync(addTime);
        //            if (addWater.Level == SemiAutoResultCode.Exception)
        //                return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = addWater.Message, Exception = addWater.Exception };
        //            if (addWater.Level == SemiAutoResultCode.NeedInteraction)
        //                return new TaskResult { Code = My_Tool.Result.ResultCode.NeedInteraction, Message = addWater.Message };
        //        }

        //        if (j + 1 < currentDropData.Count)
        //            cupCT = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(currentDropData[++j].CupNo, false);

        //        rp -= drop.Pulse;
        //        semiAutoResult = await SemiAutoHelperFactory.Current.DispenseAsync(rp, syringeType, drop.DispenseHeight);
        //        if (semiAutoResult.Level == SemiAutoResultCode.Exception)
        //            return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = semiAutoResult.Message, Exception = semiAutoResult.Exception };
        //        if (semiAutoResult.Level == SemiAutoResultCode.NeedInteraction)
        //            return new TaskResult { Code = My_Tool.Result.ResultCode.NeedInteraction, Message = semiAutoResult.Message };
        //        if (semiAutoResult.Level == SemiAutoResultCode.MechanicalReset)
        //        {

        //            var r = await MechanicalReset.ResetMechanical(bottleNo, semiAutoResult.Message);
        //            var resetResult = new TaskResult
        //            {
        //                Code = r.Code,
        //                Message = r.Message,
        //                Exception = r.Exception
        //            };
        //            return resetResult;

        //        }
        //    }

        //    // 7. 天平复检
        //    var balanceResult = await balanceTask;
        //    if (balanceResult == 9999.99)
        //        throw new Exception("天平异常");
        //    double initialWeight = balanceResult;

        //    var balanceCTR = await balanceCTTask;
        //    if (!balanceCTR.found)
        //        throw new Exception("获取天平坐标失败");
        //    // 8. 移动到天平
        //    var semiAutoResult2 = await SemiAutoHelperFactory.Current.MoveToBalanceAsync(balanceCTR.x, balanceCTR.y);
        //    if (semiAutoResult2.Level == SemiAutoResultCode.Exception)
        //        throw semiAutoResult2.Exception ?? new Exception(semiAutoResult2.Message);
        //    if (semiAutoResult2.Level == SemiAutoResultCode.NeedInteraction)
        //        throw new Exception(semiAutoResult2.Message);
        //    // 9. 天平上注液到预留1克
        //    rp -= Convert.ToInt32(rw * adjust);
        //    semiAutoResult2 = await SemiAutoHelperFactory.Current.DispenseAsync(rp, syringeType, 0);
        //    if (semiAutoResult2.Level == SemiAutoResultCode.Exception)
        //        throw semiAutoResult2.Exception ?? new Exception(semiAutoResult2.Message);
        //    if (semiAutoResult2.Level == SemiAutoResultCode.NeedInteraction)
        //        throw new Exception(semiAutoResult2.Message);
        //    // 10. 回母液瓶
        //    semiAutoResult2 = await SemiAutoHelperFactory.Current.MoveToBottleAsync(bottleNo, bottleCTR.x, bottleCTR.y, 1, 0);
        //    if (semiAutoResult2.Level == SemiAutoResultCode.Exception)
        //        throw semiAutoResult2.Exception ?? new Exception(semiAutoResult2.Message);
        //    if (semiAutoResult2.Level == SemiAutoResultCode.NeedInteraction)
        //        throw new Exception(semiAutoResult2.Message);

        //    // 11. 异步更新数据库
        //    var updateAddTask = Task.Run(async () =>
        //    {
        //        var info = SmartColor.My_Tool.BottleAuxiliary.GetBottleInfo(bottleNo);
        //        int retainDecimals = My_Tool.BalanceStableReading.RetainDecimals();
        //        // 天平稳定读数
        //        var balanceAfter = await My_Tool.BalanceStableReading.StableReadingAsync();
        //        if (balanceAfter == 9999.99)
        //            throw new Exception("天平异常");
        //        double realAddWeight = balanceAfter - initialWeight;
        //        double realError = Math.Round(realAddWeight - rw, retainDecimals);
        //        double totalAddWeight = realAddWeight;
        //        // 只处理本轮已加完的杯子
        //        var finishedCups = currentDropData
        //            .Where(drop => remainPulseDict[drop.CupNo] == 0)
        //            .ToList();
        //        string unit = info.UnitOfAccount;

        //        foreach (var drop in finishedCups)
        //        {
        //            var dr = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {drop.CupNo}");
        //            if (dr.Rows.Count == 0) continue;
        //            int headID = int.TryParse(dr.Rows[0][CUP_DETAILS.HeadID]?.ToString() ?? "0", out int hid) ? hid : 0;
        //            int stepNo = int.TryParse(dr.Rows[0][CUP_DETAILS.StepNum]?.ToString() ?? "0", out int sn) ? sn : 0;
        //            var oldCW = ToDoubleSafe(dr.Rows[0][CUP_DETAILS.CurrentWeight]);

        //            // 查找该杯的总需加量（ObjectDropWeight）
        //            var dyeDetailDt = SqlServer.Select(DYE_DETAILS.TableName, $"{DYE_DETAILS.HeadID} = {headID} AND {DYE_DETAILS.StepNum} = {stepNo}");
        //            if (dyeDetailDt.Rows.Count == 0) continue;
        //            var dyeRow = dyeDetailDt.Rows[0];
        //            double objectDropWeight = ToDoubleSafe(dyeRow[DYE_DETAILS.ObjectDropWeight]);

        //            // 实际滴液量 = 总需加量 + realError
        //            double actualAdd = objectDropWeight + realError;
        //            totalAddWeight += actualAdd;
        //            double allowErr = My_Tool.BottleAuxiliary.GetAllowErr(bottleNo);

        //            // === 新增修正逻辑 ===
        //            if (My_ConPar.Choices.CorrectData == 1 && Math.Abs(realError) > allowErr && realAddWeight > 0)
        //            {
        //                actualAdd = objectDropWeight;
        //            }
        //            SqlServer.Update(DYE_DETAILS.TableName,
        //                new Dictionary<string, object>
        //                {
        //                    { DYE_DETAILS.RealDropWeight, actualAdd },
        //                    { DYE_DETAILS.Finish, 1 }
        //                },
        //                $"{DYE_DETAILS.HeadID} = @HeadID AND {DYE_DETAILS.StepNum} = @StepNo",
        //                new SqlParameter("@HeadID", headID),
        //                new SqlParameter("@StepNo", stepNo));

        //            SqlServer.Update(CUP_DETAILS.TableName,
        //                new Dictionary<string, object>
        //                {
        //                    { CUP_DETAILS.CurrentWeight, oldCW + actualAdd + drop.WaterWeight },
        //                    { CUP_DETAILS.CurrentStepFinish, 1 }
        //                },
        //                $"{CUP_DETAILS.CupNum} = @cupNo",
        //                new SqlParameter("@cupNo", drop.CupNo));

        //            var area = My_AutomaticModule.CupCommManager.Instance.FindCupAreaByCupNum(drop.CupNo);
        //            if (area != null)
        //            {
        //                area.OnCupDataReceived(drop.CupNo);
        //            }

        //        }

        //        var bottleInfo = My_DataBase.BottleData.Bottle_details.Select($"{BOTTLE_DETAILS.BottleNum} = {bottleNo}").FirstOrDefault();
        //        if (bottleInfo != null)
        //        {
        //            double oldBottleWeight = ToDoubleSafe(bottleInfo[BOTTLE_DETAILS.CurrentWeight]);
        //            double newBottleWeight = (oldBottleWeight - totalAddWeight) >= 0 ? (oldBottleWeight - totalAddWeight) : 0;
        //            SqlServer.Update(BOTTLE_DETAILS.TableName,
        //                new Dictionary<string, object>
        //                {
        //            { BOTTLE_DETAILS.CurrentWeight, newBottleWeight },
        //            { BOTTLE_DETAILS.LastUseTime, DateTime.Now }
        //                },
        //                $"{BOTTLE_DETAILS.BottleNum} = @BottleNum",
        //                new SqlParameter("@BottleNum", bottleNo));
        //        }
        //    });

        //    // 12. 判断是否全部加完
        //    bool isFinish = remainPulseDict.Values.All(v => v <= 0);

        //    // 13. 只在全部加完时放针，否则递归继续
        //    if (!isFinish)
        //    {
        //        var remainCupInfos = cupInfos
        //            .Where(x => remainPulseDict[x.CupNo] > 0)
        //            .Select(x =>
        //            {
        //                var clone = new CupChemicalInfo
        //                {
        //                    CupNo = x.CupNo,
        //                    AddWeight = remainPulseDict[x.CupNo] / adjust,
        //                    WaterWeight = x.WaterWeight,
        //                    BottleNo = x.BottleNo,
        //                    SyringeType = x.SyringeType,
        //                    Adjust = x.Adjust,
        //                    RW = x.RW,
        //                    PurgeCount = x.PurgeCount,
        //                    DispenseHeight = x.DispenseHeight,
        //                    TaskResult = x.TaskResult,
        //                };
        //                return clone;
        //            }).ToList();

        //        // 递归继续加药，不放针，传递 updateAddTask
        //        return await AddChemicalForCupsWithBalance(remainCupInfos, currentTask, updateAddTask);
        //    }

        //    // 全部加完才放针，等待 updateAddTask
        //    if (currentTask != null)
        //        await currentTask.CheckPauseOnlyAsync();
        //    var releaseNeedle = await SemiAutoHelperFactory.Current.ReleaseNeedleAsync(syringeType);
        //    if (releaseNeedle.Level == SemiAutoResultCode.Exception)
        //        return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = releaseNeedle.Message, Exception = releaseNeedle.Exception };
        //    if (releaseNeedle.Level == SemiAutoResultCode.NeedInteraction)
        //        return new TaskResult { Code = My_Tool.Result.ResultCode.NeedInteraction, Message = releaseNeedle.Message };

        //    try
        //    {
        //        if (updateAddTask != null)
        //            await updateAddTask;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("updateAddTask异常", ex);
        //        return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = "天平/数据库异常：" + ex.Message, Exception = ex };
        //    }

        //    return new TaskResult { Code = My_Tool.Result.ResultCode.Success, Message = "加药完成" };
        //}

        /// <summary>
        /// 合并抽液/注液（同瓶），天平复检，放针，数据库更新，注液高度处理
        /// 支持断点续加，利用dye_details.NeedPulse防止重试重复加药
        /// </summary>
        private static async Task<TaskResult> AddChemicalForCupsWithBalance(
    List<CupChemicalInfo> cupInfos,
    SmartColor.My_RobotManager.RobotBusinessTask<TaskResult> currentTask,
    Task updateTask = null)
        {
            double ToDoubleSafe(object value)
            {
                if (value == null || value == DBNull.Value) return 0;
                double result;
                return double.TryParse(value.ToString(), out result) ? result : 0;
            }

            int bottleNo = cupInfos[0].BottleNo;
            short syringeType = cupInfos[0].SyringeType;
            double adjust = cupInfos[0].Adjust;
            int rw = cupInfos[0].RW;
            int purgeCount = cupInfos[0].PurgeCount;

            // 1. 读取每个杯的NeedPulse，决定本次还需加多少（带StepNum）
            var filteredCups = new List<CupChemicalInfo>();
            var cupStepNumDict = new Dictionary<int, int>();
            foreach (var cup in cupInfos)
            {
                // 查当前步号
                var dr = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {cup.CupNo}");
                int stepNum = 0;
                int headID = 0;
                if (dr.Rows.Count > 0)
                {
                    int.TryParse(dr.Rows[0][CUP_DETAILS.StepNum]?.ToString() ?? "0", out stepNum);
                    int.TryParse(dr.Rows[0][CUP_DETAILS.HeadID]?.ToString() ?? "0", out headID);
                }

                cupStepNumDict[cup.CupNo] = stepNum;

                // 查询dye_details
                var dt = SqlServer.Select(
                    DYE_DETAILS.TableName,
                    $"{DYE_DETAILS.CupNum} = {cup.CupNo} AND {DYE_DETAILS.StepNum} = {stepNum} AND {DYE_DETAILS.Finish} = 0 AND {DYE_DETAILS.HeadID} = {headID}"
                );
                int needPulse = 0;
                int totalPulse1 = Convert.ToInt32(Math.Round(cup.AddWeight * adjust));
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    int.TryParse(row[DYE_DETAILS.NeedPulse]?.ToString() ?? "0", out needPulse);
                    // 首次加药时初始化NeedPulse
                    if (needPulse <= 0)
                    {
                        needPulse = totalPulse1;
                        SqlServer.Update(DYE_DETAILS.TableName,
                            new Dictionary<string, object> { { DYE_DETAILS.NeedPulse, needPulse } },
                            $"{DYE_DETAILS.CupNum} = @CupNo AND {DYE_DETAILS.StepNum} = @StepNum AND {DYE_DETAILS.Finish} = 0 AND {DYE_DETAILS.HeadID} = @HeadID",
                            new SqlParameter("@CupNo", cup.CupNo),
                            new SqlParameter("@StepNum", stepNum),
                            new SqlParameter("@HeadID", headID));
                    }
                    // 以NeedPulse为准分配加药量
                    if (needPulse > 0)
                    {
                        cup.AddWeight = needPulse / adjust;
                        filteredCups.Add(cup);
                    }
                }
                else
                {
                    // 没有记录
                    SqlServer.Update(CUP_DETAILS.TableName,
                         new Dictionary<string, object>
                         {
                            { CUP_DETAILS.CurrentStepFinish, 1 }
                         },
                         $"{CUP_DETAILS.CupNum} = @cupNo",
                         new SqlParameter("@cupNo", cup.CupNo));

                }
            }
            if (filteredCups.Count == 0)
            {
                if (updateTask != null) await updateTask;
                return new TaskResult { Code = My_Tool.Result.ResultCode.Success, Message = "加药完成" };
            }

            // 2. 计算每个杯子剩余脉冲
            var remainPulseDict = filteredCups.ToDictionary(
                x => x.CupNo,
                x => Convert.ToInt32(Math.Round(x.AddWeight * adjust))
            );

            // 3. 计算本轮最大可抽脉冲
            int maxPulse = syringeType == 0 ? My_ConPar.Other.S_MaxPulse : My_ConPar.Other.B_MaxPulse;
            int backPulse = My_ConPar.Other.Z_BackPulse;
            int minPulse = Convert.ToInt32(rw * adjust) + Convert.ToInt32(1 * adjust);
            int availablePulse = maxPulse + backPulse - minPulse;

            // 4. 本轮分配脉冲到各杯
            var currentDropData = new List<CupPulseInfo>();
            int totalPulse = 0;
            if (My_ConPar.Choices.DripFull == 1)
            {
                foreach (var cup in filteredCups)
                {
                    int remain = remainPulseDict[cup.CupNo];
                    if (remain <= 0) continue;
                    int leftPulse = availablePulse - totalPulse;
                    if (leftPulse <= 0) break;
                    int thisPulse = Math.Min(remain, leftPulse);
                    if (thisPulse > 0)
                    {
                        currentDropData.Add(new CupPulseInfo
                        {
                            CupNo = cup.CupNo,
                            Pulse = thisPulse,
                            ObjectDropWeight = cup.AddWeight,
                            WaterWeight = cup.WaterWeight,
                            DispenseHeight = cup.DispenseHeight
                        });
                        remainPulseDict[cup.CupNo] -= thisPulse;
                        totalPulse += thisPulse;
                        if (totalPulse >= availablePulse)
                            break;
                    }
                }
            }
            else
            {
                foreach (var cup in filteredCups)
                {
                    int remain = remainPulseDict[cup.CupNo];
                    if (remain <= 0) continue;
                    int leftPulse = availablePulse - totalPulse;
                    if (leftPulse <= 0) break;
                    int thisPulse = Math.Min(remain, leftPulse);
                    if (thisPulse > 0)
                    {
                        currentDropData.Add(new CupPulseInfo
                        {
                            CupNo = cup.CupNo,
                            Pulse = thisPulse,
                            ObjectDropWeight = cup.AddWeight,
                            WaterWeight = cup.WaterWeight,
                            DispenseHeight = cup.DispenseHeight
                        });
                        remainPulseDict[cup.CupNo] -= thisPulse;
                        totalPulse += thisPulse;
                    }
                }
            }
            if (currentDropData.Count == 0)
            {
                if (updateTask != null) await updateTask;
                return new TaskResult { Code = My_Tool.Result.ResultCode.Success, Message = "加药完成" };
            }
            int z = totalPulse + (Convert.ToInt32(rw * adjust) + Convert.ToInt32(1 * adjust) - backPulse);

            // 5. 并发查找母液瓶、天平、首个杯坐标
            var bottleCTRTask = My_Tool.AreaCoordinateFinder.TryGetBottleCoordinateAsync(bottleNo);
            var balanceCTTask = My_Tool.AreaCoordinateFinder.TryGetBalanceCoordinateAsync();
            var cupCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(currentDropData[0].CupNo, false);

            var bottleCTR = await bottleCTRTask;
            if (!bottleCTR.found)
                return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = $"未找到母液瓶{bottleNo}坐标", Exception = new Exception($"未找到母液瓶{bottleNo}坐标") };
            if (currentTask != null)
                await currentTask.CheckPauseOnlyAsync();

            // 6. 移动到母液瓶并抽液
            var moveResult = SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.MoveToBottleAsync(bottleNo, bottleCTR.x, bottleCTR.y, 2, 1);
            var semiAutoResult = await moveResult;
            if (semiAutoResult.Level == SemiAutoResultCode.Exception)
                return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = semiAutoResult.Message, Exception = semiAutoResult.Exception };
            if (semiAutoResult.Level == SemiAutoResultCode.NeedInteraction)
                return new TaskResult { Code = My_Tool.Result.ResultCode.NeedInteraction, Message = semiAutoResult.Message };

            if (currentTask != null)
                await currentTask.CheckPauseOnlyAsync();

            var currentWeight = ToDoubleSafe(My_DataBase.SqlServer.Select(BOTTLE_DETAILS.TableName, $"{BOTTLE_DETAILS.BottleNum} = {bottleNo}").Rows[0][BOTTLE_DETAILS.CurrentWeight]);


            semiAutoResult = await SemiAutoHelperFactory.Current.AspirateAsync(z, purgeCount, syringeType, Convert.ToInt16(currentWeight));
            My_DataBase.SqlServer.Update(BOTTLE_DETAILS.TableName,
                new Dictionary<string, object> { { BOTTLE_DETAILS.LastUseTime, DateTime.Now } },
                $"{BOTTLE_DETAILS.BottleNum} = {bottleNo}");
            if (semiAutoResult.Level == SemiAutoResultCode.Exception)
                return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = semiAutoResult.Message, Exception = semiAutoResult.Exception };
            if (semiAutoResult.Level == SemiAutoResultCode.NeedInteraction)
                return new TaskResult { Code = My_Tool.Result.ResultCode.NeedInteraction, Message = semiAutoResult.Message };
            if (updateTask != null) await updateTask;

            // 7. 分批注液（加水+加药），并实时更新NeedPulse和Finish
            int rp = z + backPulse;
            Task<double> balanceTask = null;
            var cupCT = cupCTTask;
            int j = 0;
            for (int i = 0; i < currentDropData.Count; i++)
            {
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                var drop = currentDropData[i];
                int stepNum = cupStepNumDict[drop.CupNo];

                if (i >= currentDropData.Count - 2 && balanceTask == null)
                    balanceTask = My_Tool.BalanceStableReading.CheckAndReadBalanceAsync();

                var cupCTR = await cupCT;
                if (!cupCTR.found)
                    return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = $"未找到{drop.CupNo}号杯坐标", Exception = new Exception($"未找到{drop.CupNo}号杯坐标") };
                semiAutoResult = await SemiAutoHelperFactory.Current.MoveToCupAsync(drop.CupNo, 0, cupCTR.x, cupCTR.y, 1, 0);
                if (semiAutoResult.Level == SemiAutoResultCode.Exception)
                    return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = semiAutoResult.Message, Exception = semiAutoResult.Exception };
                if (semiAutoResult.Level == SemiAutoResultCode.NeedInteraction)
                    return new TaskResult { Code = My_Tool.Result.ResultCode.NeedInteraction, Message = semiAutoResult.Message };

                if (drop.WaterWeight > 0)
                {
                    double addTime = drop.WaterWeight / SmartColor.My_ConPar.Correction.Correcting_Water_Value;
                    var addWater = await SemiAutoHelperFactory.Current.AddWaterAsync(drop.WaterWeight, addTime);
                    if (addWater.Level == SemiAutoResultCode.Exception)
                        return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = addWater.Message, Exception = addWater.Exception };
                    if (addWater.Level == SemiAutoResultCode.NeedInteraction)
                        return new TaskResult { Code = My_Tool.Result.ResultCode.NeedInteraction, Message = addWater.Message };
                }

                if (j + 1 < currentDropData.Count)
                    cupCT = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(currentDropData[++j].CupNo, false);

                rp -= drop.Pulse;
                semiAutoResult = await SemiAutoHelperFactory.Current.DispenseAsync(rp, syringeType, drop.DispenseHeight);
                if (semiAutoResult.Level == SemiAutoResultCode.Exception)
                    return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = semiAutoResult.Message, Exception = semiAutoResult.Exception };
                if (semiAutoResult.Level == SemiAutoResultCode.NeedInteraction)
                    return new TaskResult { Code = My_Tool.Result.ResultCode.NeedInteraction, Message = semiAutoResult.Message };
                if (semiAutoResult.Level == SemiAutoResultCode.MechanicalReset)
                {

                    var r = await MechanicalReset.ResetMechanical(bottleNo, semiAutoResult.Message);
                    var resetResult = new TaskResult
                    {
                        Code = r.Code,
                        Message = r.Message,
                        Exception = r.Exception
                    };
                    return resetResult;

                }
                // === 立即更新NeedPulse和Finish（带StepNum） ===
                int remainPulse = remainPulseDict[drop.CupNo];
                var updateDict = new Dictionary<string, object>
                {
                    { DYE_DETAILS.NeedPulse, remainPulse }
                };
                if (remainPulse <= 0)
                {
                    updateDict[DYE_DETAILS.Finish] = 1;
                    SqlServer.Update(CUP_DETAILS.TableName,
                        new Dictionary<string, object> { { CUP_DETAILS.CurrentStepFinish, 1 } },
                        $"{CUP_DETAILS.CupNum} = @CupNo",
                        new SqlParameter("@CupNo", drop.CupNo));
                }
                SqlServer.Update(DYE_DETAILS.TableName,
                    updateDict,
                    $"{DYE_DETAILS.CupNum} = @CupNo AND {DYE_DETAILS.StepNum} = @StepNum AND {DYE_DETAILS.Finish} = 0",
                    new SqlParameter("@CupNo", drop.CupNo),
                    new SqlParameter("@StepNum", stepNum));
            }

            // 8. 天平复检
            var balanceResult = await balanceTask;
            if (balanceResult == 9999.99)
                throw new Exception("天平异常");
            double initialWeight = balanceResult;

            var balanceCTR = await balanceCTTask;
            if (!balanceCTR.found)
                throw new Exception("获取天平坐标失败");
            var semiAutoResult2 = await SemiAutoHelperFactory.Current.MoveToBalanceAsync(balanceCTR.x, balanceCTR.y);
            if (semiAutoResult2.Level == SemiAutoResultCode.Exception)
                throw semiAutoResult2.Exception ?? new Exception(semiAutoResult2.Message);
            if (semiAutoResult2.Level == SemiAutoResultCode.NeedInteraction)
                throw new Exception(semiAutoResult2.Message);
            rp -= Convert.ToInt32(rw * adjust);
            semiAutoResult2 = await SemiAutoHelperFactory.Current.DispenseAsync(rp, syringeType, 0);
            if (semiAutoResult2.Level == SemiAutoResultCode.Exception)
                throw semiAutoResult2.Exception ?? new Exception(semiAutoResult2.Message);
            if (semiAutoResult2.Level == SemiAutoResultCode.NeedInteraction)
                throw new Exception(semiAutoResult2.Message);
            semiAutoResult2 = await SemiAutoHelperFactory.Current.MoveToBottleAsync(bottleNo, bottleCTR.x, bottleCTR.y, 1, 0);
            if (semiAutoResult2.Level == SemiAutoResultCode.Exception)
                throw semiAutoResult2.Exception ?? new Exception(semiAutoResult2.Message);
            if (semiAutoResult2.Level == SemiAutoResultCode.NeedInteraction)
                throw new Exception(semiAutoResult2.Message);

            // 9. 异步更新数据库（带StepNum）
            var updateAddTask = Task.Run(async () =>
            {
                var info = SmartColor.My_Tool.BottleAuxiliary.GetBottleInfo(bottleNo);
                int retainDecimals = My_Tool.BalanceStableReading.RetainDecimals();
                var balanceAfter = await My_Tool.BalanceStableReading.StableReadingAsync();
                if (balanceAfter == 9999.99)
                    throw new Exception("天平异常");
                double realAddWeight = balanceAfter - initialWeight;
                double realError = Math.Round(realAddWeight - rw, retainDecimals);
                double totalAddWeight = realAddWeight;
                var finishedCups = currentDropData
                    .Where(drop => remainPulseDict[drop.CupNo] == 0)
                    .ToList();
                string unit = info.UnitOfAccount;

                foreach (var drop in finishedCups)
                {
                    int stepNum = cupStepNumDict[drop.CupNo];
                    var dr = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {drop.CupNo}");
                    if (dr.Rows.Count == 0) continue;
                    int headID = int.TryParse(dr.Rows[0][CUP_DETAILS.HeadID]?.ToString() ?? "0", out int hid) ? hid : 0;
                    var dyeDetailDt = SqlServer.Select(DYE_DETAILS.TableName, $"{DYE_DETAILS.HeadID} = {headID} AND {DYE_DETAILS.StepNum} = {stepNum}");
                    if (dyeDetailDt.Rows.Count == 0) continue;
                    var dyeRow = dyeDetailDt.Rows[0];
                    double objectDropWeight = ToDoubleSafe(dyeRow[DYE_DETAILS.ObjectDropWeight]);
                    var oldCW = ToDoubleSafe(dr.Rows[0][CUP_DETAILS.CurrentWeight]);

                    double actualAdd = objectDropWeight + realError;
                    totalAddWeight += actualAdd;
                    double allowErr = My_Tool.BottleAuxiliary.GetAllowErr(bottleNo);

                    if (My_ConPar.Choices.CorrectData == 1 && Math.Abs(realError) > allowErr && realAddWeight > 0)
                    {
                        actualAdd = objectDropWeight;
                    }
                    SqlServer.Update(DYE_DETAILS.TableName,
                        new Dictionary<string, object>
                        {
                            { DYE_DETAILS.RealDropWeight, actualAdd },
                            { DYE_DETAILS.Finish, 1 },
                            { DYE_DETAILS.NeedPulse, 0 }
                        },
                        $"{DYE_DETAILS.HeadID} = @HeadID AND {DYE_DETAILS.StepNum} = @StepNum",
                        new SqlParameter("@HeadID", headID),
                        new SqlParameter("@StepNum", stepNum));

                    SqlServer.Update(CUP_DETAILS.TableName,
                        new Dictionary<string, object>
                        {
                    { CUP_DETAILS.CurrentWeight, oldCW + actualAdd + drop.WaterWeight },
                    { CUP_DETAILS.CurrentStepFinish, 1 }
                        },
                        $"{CUP_DETAILS.CupNum} = @cupNo",
                        new SqlParameter("@cupNo", drop.CupNo));

                    var area = My_AutomaticModule.CupCommManager.Instance.FindCupAreaByCupNum(drop.CupNo);
                    if (area != null)
                    {
                        area.OnCupDataReceived(drop.CupNo);
                    }
                }

                var bottleInfo = My_DataBase.BottleData.Bottle_details.Select($"{BOTTLE_DETAILS.BottleNum} = {bottleNo}").FirstOrDefault();
                if (bottleInfo != null)
                {
                    double oldBottleWeight = ToDoubleSafe(bottleInfo[BOTTLE_DETAILS.CurrentWeight]);
                    double newBottleWeight = (oldBottleWeight - totalAddWeight) >= 0 ? (oldBottleWeight - totalAddWeight) : 0;
                    SqlServer.Update(BOTTLE_DETAILS.TableName,
                        new Dictionary<string, object>
                        {
                    { BOTTLE_DETAILS.CurrentWeight, newBottleWeight },
                    { BOTTLE_DETAILS.LastUseTime, DateTime.Now }
                        },
                        $"{BOTTLE_DETAILS.BottleNum} = @BottleNum",
                        new SqlParameter("@BottleNum", bottleNo));
                }
            });

            // 10. 判断是否全部加完
            bool isFinish = remainPulseDict.Values.All(v => v <= 0);

            // 11. 只在全部加完时放针，否则递归继续
            if (!isFinish)
            {
                var remainCupInfos = filteredCups
                    .Where(x => remainPulseDict[x.CupNo] > 0)
                    .Select(x =>
                    {
                        var clone = new CupChemicalInfo
                        {
                            CupNo = x.CupNo,
                            AddWeight = remainPulseDict[x.CupNo] / adjust,
                            WaterWeight = x.WaterWeight,
                            BottleNo = x.BottleNo,
                            SyringeType = x.SyringeType,
                            Adjust = x.Adjust,
                            RW = x.RW,
                            PurgeCount = x.PurgeCount,
                            DispenseHeight = x.DispenseHeight,
                            TaskResult = x.TaskResult,
                        };
                        return clone;
                    }).ToList();

                return await AddChemicalForCupsWithBalance(remainCupInfos, currentTask, updateAddTask);
            }

            if (currentTask != null)
                await currentTask.CheckPauseOnlyAsync();
            var releaseNeedle = await SemiAutoHelperFactory.Current.ReleaseNeedleAsync(syringeType);
            if (releaseNeedle.Level == SemiAutoResultCode.Exception)
                return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = releaseNeedle.Message, Exception = releaseNeedle.Exception };
            if (releaseNeedle.Level == SemiAutoResultCode.NeedInteraction)
                return new TaskResult { Code = My_Tool.Result.ResultCode.NeedInteraction, Message = releaseNeedle.Message };

            try
            {
                if (updateAddTask != null)
                    await updateAddTask;
            }
            catch (Exception ex)
            {
                Logger.Error("updateAddTask异常", ex);
                return new TaskResult { Code = My_Tool.Result.ResultCode.Exception, Message = "天平/数据库异常：" + ex.Message, Exception = ex };
            }

            return new TaskResult { Code = My_Tool.Result.ResultCode.Success, Message = "加药完成" };
        }

        /// <summary>
        /// 通知HMI主副杯加药完成
        /// </summary>
        private static async Task NotifyHMIAddChemicalFinish(List<int> cupNos)
        {
            if (cupNos.Count == 0) return;
            var cupNo = cupNos[0];
            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
            if (area != null)
            {
                var comm = CupCommManager.Instance.GetCommObject(area);
                if (comm != null)
                {
                    await comm.SendAddChemicaFinish(cupNo);
                }
            }

        }

        /// <summary>
        /// 加药信息结构体
        /// </summary>
        private class CupChemicalInfo
        {
            public int CupNo;
            public double AddWeight;
            public double WaterWeight;
            public int BottleNo;
            public short SyringeType;
            public double Adjust;
            public int RW;
            public int PurgeCount;
            public short DispenseHeight; // 注液高度参数（0=开盖，2=关盖）
            public TaskResult TaskResult;

            public static CupChemicalInfo Ok(int cupNo, double addWeight, double waterWeight, int bottleNo, short syringeType, double adjust, int rw, int purgeCount, short dispenseHeight)
            {
                return new CupChemicalInfo
                {
                    CupNo = cupNo,
                    AddWeight = addWeight,
                    WaterWeight = waterWeight,
                    BottleNo = bottleNo,
                    SyringeType = syringeType,
                    Adjust = adjust,
                    RW = rw,
                    PurgeCount = purgeCount,
                    DispenseHeight = dispenseHeight,
                    TaskResult = new TaskResult { Code = My_Tool.Result.ResultCode.Success }
                };
            }
            public static CupChemicalInfo Fail(int cupNo, TaskResult taskResult)
            {
                return new CupChemicalInfo
                {
                    CupNo = cupNo,
                    TaskResult = taskResult

                };
            }
        }

        /// <summary>
        /// 提交加药任务到任务中心
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> EnqueueAddChemicalAsync(int cupNo, int dyeType, bool noCheckExpired, bool noCheckLow)
        {
            var result = new TaskResult();

            // 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 只对正在使用、当前步未完成的杯进行加药
            var needAddChemicalCups = new List<int>();
            foreach (var info in cupInfos)
            {
                if (info.IsUsing == 1 && info.CurrentStepFinish == 0)
                {
                    needAddChemicalCups.Add(info.CupNum);
                }
            }

            // 如果主副杯都未使用或当前步已完成，直接返回
            if (needAddChemicalCups.Count == 0)
            {
                await RunTableMan.InsertAsync(new Dictionary<string, object>
                {
                    [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNo}号配液杯加药已经完成触发发送下一步"
                }, DateTime.Now);
                await My_Tool.CupAuxiliary.SendNextStepIfNeeded(cupNo);
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "主副杯当前步已完成或未使用，无需加药";
                return result;
            }



            var task = new SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>
            {
                Priority = dyeType == 1 ?
                BigProcess.DyeingProcess * 100 +
                SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.AddChemicalProcess :
                BigProcess.PostProcess * 100 +
                   SmartColor.My_ConPar.Order.PostProcess.PostProcess.AddChemicalProcess,
                OriginalPriority = dyeType == 1 ?
                BigProcess.DyeingProcess * 100 +
                SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.AddChemicalProcess :
                BigProcess.PostProcess * 100 +
                   SmartColor.My_ConPar.Order.PostProcess.PostProcess.AddChemicalProcess,
                BusinessType = dyeType == 1 ? SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DyeingProcess :
                   SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.PostProcess,
                TaskName = $"加药-{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}",
                BusinessFlow = async () => await AddChemicalAsync(cupNo, noCheckExpired, noCheckLow)
            };

            try
            {
                result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯加药异常", result.Message, async btn =>
                            {
                                if (btn == "确认")
                                {
                                    if (result.Message.Contains("盖"))
                                    {
                                        var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                        foreach (var c in needAddChemicalCups)
                                        {

                                            int coverStatus = 0;
                                            if (mainInfo.CupNum == c)
                                            {
                                                coverStatus = mainInfo.CoverStatus ?? 0;
                                            }
                                            else
                                            {
                                                coverStatus = subInfo.CoverStatus ?? 0;
                                            }

                                            if (coverStatus == 2) continue;



                                            SqlServer.Update(
                                                CUP_DETAILS.TableName,
                                                new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 2 } },
                                                $"{CUP_DETAILS.CupNum} = @CupNo",
                                                new SqlParameter("@CupNo", cupNo)
                                            );

                                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                                            if (area != null)
                                            {
                                                var comm = CupCommManager.Instance.GetCommObject(area);
                                                if (comm != null)
                                                {
                                                    await comm.SyncCoverStatus(c);
                                                }

                                                area.OnCupDataReceived(c);
                                            }
                                        }
                                    }
                                    else if (result.Message.Contains("号母液瓶已过期,继续加药请点'确认'"))
                                    {
                                        noCheckExpired = true;
                                    }
                                    else if (result.Message.Contains("号母液瓶液量过低,继续加药请点'确认'"))
                                    {
                                        noCheckLow = true;
                                    }



                                    var retryResult = await EnqueueAddChemicalAsync(cupNo, dyeType, noCheckExpired, noCheckLow);
                                    tcs.SetResult(retryResult);
                                }

                            },
                            new[] { "确认" },
                            "确认"
                             );
                            return await tcs.Task;
                        }
                    case My_Tool.Result.ResultCode.NeedInteraction:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯加药询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    if (result.Message.Contains("关盖时撑盖开失败"))
                                    {
                                        var retryResult = await EnqueueSupportCoverAsync(cupNo, task.Priority, dyeType == 1 ? SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DyeingProcess :
                   SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.PostProcess);
                                        if (retryResult.Code == Result.ResultCode.Success)
                                        {
                                            retryResult = await EnqueueAddChemicalAsync(cupNo, dyeType, noCheckExpired, noCheckLow);
                                            tcs.SetResult(retryResult);
                                        }
                                        else
                                        {
                                            tcs.SetResult(retryResult);
                                        }
                                    }
                                    else
                                    {
                                        var retryResult = await EnqueueAddChemicalAsync(cupNo, dyeType, noCheckExpired, noCheckLow);
                                        tcs.SetResult(retryResult);
                                    }
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                Logger.Error("EnqueueAddChemicalAsync：发生异常。", ex);
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }
        #endregion

        #region 4. 放布任务
        /// <summary>
        /// 放布任务预处理：提前查找所有需要放布的杯的坐标和状态（含干布夹子、备布区、杯、杯盖）
        /// </summary>
        private static async Task<object> PrecomputePutClothAsync(int cupNo)
        {
            // 1. 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 2. 过滤需要放布的杯
            var needPutClothCups = cupInfos
                .Where(info => info.IsUsing == 1 && (info.HaveCloth == null || info.HaveCloth == 0) && info.TechnologyName == "放布")
                .ToList();

            // 3. 并发准备每个杯子的所有相关坐标
            var cupTasks = needPutClothCups.Select(async info =>
            {
                int cno = info.CupNum;

                int headID = info.HeadID ?? 0;
                int stepNo = 0;
                int.TryParse(info.StepNum, out stepNo);
                int clothPos = 0;
                if (headID != 0)
                {
                    var dropHeadRows = SqlServer.Select(DROP_HEAD.TableName, null, $"{DROP_HEAD.MyID} = {headID}");
                    if (dropHeadRows.Rows.Count > 0)
                    {
                        var dropHeadRow = dropHeadRows.Rows[0];
                        if (dropHeadRow.Table.Columns.Contains(DROP_HEAD.ClothNum))
                            clothPos = int.TryParse(dropHeadRow[DROP_HEAD.ClothNum]?.ToString() ?? "0", out int pos) ? pos : 0;
                    }
                }
                // 坐标并发获取
                var cupObjTask = CupCommManager.Instance.FindCupByCupNumAsync(info.CupNum);

                var cupCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cno, false);
                var lidCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cno, true);
                var prepareClothCTTask = clothPos > 0
                    ? My_Tool.AreaCoordinateFinder.TryGetPrepareClothCoordinateAsync(clothPos)
                    : Task.FromResult<(bool, int, int)>((false, 0, 0));

                var cupCTR = await cupCTTask;
                var lidCTR = await lidCTTask;
                var cupObj = await cupObjTask;
                var prepareClothCTR = await prepareClothCTTask;

                return new
                {
                    Info = info,
                    CupNum = cno,

                    HeadID = headID,
                    StepNo = stepNo,
                    ClothPos = clothPos,
                    CupCTR = cupCTR,
                    LidCTR = lidCTR,
                    PrepareClothCTR = prepareClothCTR,
                    CupObj = cupObj
                };
            }).ToArray();

            var cupDataList = await Task.WhenAll(cupTasks);
            // 干布夹子坐标也提前查好
            var dryCTR = await My_Tool.AreaCoordinateFinder.TryGetDryClampCoordinateAsync();
            return new { CupDataList = cupDataList, DryCTR = dryCTR };
        }

        /// <summary>
        /// 放布任务（支持预处理结果）
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> PutClothAsync(int cupNo, object precomputed = null)
        {
            var result = new TaskResult()
            {
                Code = Result.ResultCode.Success
            };
            var currentTask = SmartColor.My_RobotManager.RobotTaskManager.Instance.CurrentTask as SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>;
            try
            {
                dynamic cupDataList;
                dynamic dryCTR;
                if (precomputed != null)
                {
                    dynamic pre = precomputed;
                    cupDataList = pre.CupDataList;
                    dryCTR = pre.DryCTR;
                }
                else
                {
                    // 兼容未预处理的情况
                    var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                    var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
                    if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);
                    var needPutClothCups = cupInfos
                        .Where(info => info.IsUsing == 1 && (info.HaveCloth == null || info.HaveCloth == 0) && info.TechnologyName == "放布")
                        .ToList();
                    var cupTasks = needPutClothCups.Select(async info =>
                    {
                        int cno = info.CupNum;

                        int headID = info.HeadID ?? 0;
                        int stepNo = 0;
                        int.TryParse(info.StepNum, out stepNo);
                        int clothPos = 0;
                        if (headID != 0)
                        {
                            var dropHeadDt = SqlServer.Select(DROP_HEAD.TableName, null, $"{DROP_HEAD.MyID} = {headID}");
                            if (dropHeadDt.Rows.Count > 0)
                            {
                                var dropHeadRow = dropHeadDt.Rows[0];
                                if (dropHeadRow.Table.Columns.Contains(DROP_HEAD.ClothNum))
                                    clothPos = int.TryParse(dropHeadRow[DROP_HEAD.ClothNum]?.ToString() ?? "0", out int pos) ? pos : 0;
                            }
                        }
                        var cupCTR = await My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cno, false);
                        var lidCTR = await My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cno, true);
                        var prepareClothCTTask = clothPos > 0
                            ? My_Tool.AreaCoordinateFinder.TryGetPrepareClothCoordinateAsync(clothPos)
                            : Task.FromResult((found: false, x: 0, y: 0));
                        var prepareClothCTR = await prepareClothCTTask; // 修正点
                        return new
                        {
                            Info = info,
                            CupNum = cno,

                            HeadID = headID,
                            StepNo = stepNo,
                            ClothPos = clothPos,
                            CupCTR = cupCTR,
                            LidCTR = lidCTR,
                            PrepareClothCTR = prepareClothCTR
                        };
                    }).ToArray();
                    cupDataList = await Task.WhenAll(cupTasks);
                    dryCTR = await My_Tool.AreaCoordinateFinder.TryGetDryClampCoordinateAsync();
                }

                if (cupDataList.Length == 0)
                {
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "无需放布";
                    return result;
                }

                var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                // 3. 开盖
                foreach (var cupData in cupDataList)
                {

                    var coverStatus = 0;
                    if (mainInfo.CupNum == cupNo)
                    {
                        coverStatus = mainInfo.CoverStatus ?? 0;
                    }
                    else
                    {
                        coverStatus = subInfo.CoverStatus ?? 0;
                    }

                    if (coverStatus != 2) // 2=开盖
                    {
                        var openLidResult = await OpenLidAsync(cupData.CupNum, cupData.CupCTR, cupData.LidCTR, cupData.CupObj);
                        if (openLidResult.Code != My_Tool.Result.ResultCode.Success)
                        {
                            result.Code = openLidResult.Code;
                            result.Message = openLidResult.Message;
                            result.Exception = openLidResult.Exception;
                        }
                    }
                }
                if (result.Code != My_Tool.Result.ResultCode.Success)
                    return result;

                if (My_ConPar.Choices.UseClamp == 0)
                {
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "盖已打开";
                    return result;
                }

                // 4. 移动到干布夹子位
                if (!dryCTR.Item1)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "获取干布夹具坐标异常";
                    result.Exception = new Exception("获取干布夹具坐标异常");
                    return result;
                }

                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var dryRackPos = await SemiAutoHelperFactory.Current.MoveToDryClampAsync(0, dryCTR.Item2, dryCTR.Item3, 1);
                var semiAutoResult = dryRackPos;
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

                // 5. 取夹子
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var takeClip = await SemiAutoHelperFactory.Current.RobotHandGraspingAsync(0, 0);
                semiAutoResult = takeClip;
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

                // 6. 遍历需要放布的杯
                foreach (var cupData in cupDataList)
                {
                    int cno = cupData.CupNum;
                    int headID = cupData.HeadID;
                    int stepNo = cupData.StepNo;
                    int clothPos = cupData.ClothPos;
                    var cupCTR = cupData.CupCTR;
                    var prepareClothCTR = cupData.PrepareClothCTR;

                    if (headID == 0 || stepNo == 0)
                    {

                        var (mainInfo1, subInfo1) = My_Tool.CupAuxiliary.GetMSCupInfo(cno);
                        var useInfo = mainInfo1.CupNum == cno ? mainInfo1 : subInfo1;
                        headID = useInfo.HeadID ?? 0;
                        stepNo = int.TryParse(useInfo.StepNum, out int sn) ? sn : 0;

                        if (headID == 0 || stepNo == 0)
                        {
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = $"{cno}杯未找到配方或步号";
                            result.Exception = new Exception($"{cno}杯未找到配方或步号");
                            return result;
                        }
                    }
                    if (clothPos == 0)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = $"{cno}杯未找到放布位";
                        result.Exception = new Exception($"{cno}杯未找到放布位");
                        return result;
                    }


                    _ = Task.Run(() => { SmartColor.My_File.CupTempRecorder.Get(cno).RecordStep("放布"); });

                    // 6.1 移动到放布位
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();
                    if (!prepareClothCTR.Item1)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = $"未找到{clothPos}号备布位坐标";
                        result.Exception = new Exception($"未找到{clothPos}号备布位坐标");
                        return result;
                    }
                    var moveToDryRack = await SemiAutoHelperFactory.Current.MoveToPrepareClothAsync(clothPos, 0, prepareClothCTR.Item2, prepareClothCTR.Item3);
                    semiAutoResult = moveToDryRack;
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

                    // 6.2 取干布
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();
                    var takeCloth = await SemiAutoHelperFactory.Current.TakeDryClothAsync((short)My_ConPar.Other.PutClothPosition, 0);
                    semiAutoResult = takeCloth;
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

                    // 6.3 移动到杯位
                    if (!cupCTR.Item1)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = $"未找到{cno}号杯坐标";
                        result.Exception = new Exception($"未找到{cno}号杯坐标");
                        return result;
                    }
                    var cupTask = CupCommManager.Instance.FindCupByCupNumAsync(cno);
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();

                    var moveToCup = await SemiAutoHelperFactory.Current.MoveToCupAsync(cno, 0, cupCTR.Item2, cupCTR.Item3, 2, 0);
                    semiAutoResult = moveToCup;
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

                    var cup = await cupTask;
                    if (cup == null)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Exception = new Exception($"未找到{cno}号杯杯控件");
                        result.Message = $"未找到{cno}号杯杯控件";
                        return result;
                    }

                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();

                    var wr = await CupAuxiliary.WaitLockUpOk(cno);
                    if (!wr)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Exception = new Exception($"{cno}号杯等待上锁异常");
                        result.Message = $"{cno}号杯等待上锁异常";
                        return result;
                    }

                    // 6.4 放布
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();
                    var putCloth = await SemiAutoHelperFactory.Current.PutDryClothAsync();
                    semiAutoResult = putCloth;
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

                    // 6.5 更新杯HaveCloth有布状态
                    SqlServer.Update(
                        CUP_DETAILS.TableName,
                        new Dictionary<string, object> {
                            { CUP_DETAILS.HaveCloth, 1 },
                            { CUP_DETAILS.CurrentStepFinish,1 }
                        },
                        $"{CUP_DETAILS.CupNum} = @CupNo",
                        new SqlParameter("@CupNo", cno)
                    );

                    var area = My_AutomaticModule.CupCommManager.Instance.FindCupAreaByCupNum(cno);
                    if (area != null)
                    {
                        area.OnCupDataReceived(cno);
                    }

                    // 6.6 更新DYE_DETAILS表的当前布完成状态
                    SqlServer.Update(
                        DYE_DETAILS.TableName,
                        new Dictionary<string, object> {
                            { DYE_DETAILS.Finish, 1 }
                        },
                        $"{DYE_DETAILS.HeadID} = @HeadID AND {DYE_DETAILS.StepNum} = @StepNo",
                        new SqlParameter("@HeadID", headID),
                        new SqlParameter("@StepNo", stepNo)
                    );
                }

                // 7.移动到干布夹子位
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var moveDryRackPos = await SemiAutoHelperFactory.Current.MoveToDryClampAsync(0, dryCTR.Item2, dryCTR.Item3, 0);
                semiAutoResult = moveDryRackPos;
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

                // 8. 放夹子
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var releaseClip = await SemiAutoHelperFactory.Current.RobotArmReleaseAsync(0, 0);
                semiAutoResult = releaseClip;
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

                // 9.放布后判断是否需要关盖 
                (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                foreach (var cupData in cupDataList)
                {
                    int cno = cupData.CupNum;
                    int headID = cupData.HeadID;
                    int stepNo = cupData.StepNo;
                    if (headID == 0 || stepNo == 0)
                        continue;
                    int coverStatus = 0;
                    if (mainInfo.CupNum == cno)
                    {
                        coverStatus = mainInfo.CoverStatus ?? 0;
                    }
                    else
                    {
                        coverStatus = subInfo.CoverStatus ?? 0;
                    }


                    // 获取下一步工艺
                    var nextStepInfo = My_Tool.CupAuxiliary.GetNextStepInfo(headID, stepNo);
                    if (nextStepInfo.TechnologyName == "冷行" ||
                        nextStepInfo.TechnologyName == "温控" ||
                        nextStepInfo.TechnologyName == "搅拌")
                    {



                        if (coverStatus != 1)
                        {
                            var closeLidResult = await CloseLidAsync(cno, cupData.CupCTR, cupData.LidCTR, cupData.CupObj);
                            if (closeLidResult.Code != My_Tool.Result.ResultCode.Success)
                            {
                                result.Code = closeLidResult.Code;
                                result.Message = closeLidResult.Message;
                                result.Exception = closeLidResult.Exception;
                            }
                        }
                    }
                    else
                    {
                        if (nextStepInfo.StepNum == 0)
                        {
                            //发送放布完成消息

                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                            if (area != null)
                            {
                                var comm = CupCommManager.Instance.GetCommObject(area);
                                if (comm != null)
                                {
                                    await comm.SendShowSure(cupNo, 1);
                                }
                            }
                        }
                    }
                }
                if (result.Code != Result.ResultCode.Success)
                {
                    return result;
                }
                else
                {
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "放布完成";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("PutClothAsync：发生异常。", ex);
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = "放布异常：" + ex.Message;
                result.Exception = ex;
            }
            return result;
        }

        public static async Task<CupRobotTask.TaskResult> EnqueuePutClothAsync(int cupNo, int dyeType)
        {
            TaskResult result = new TaskResult();

            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            bool needPutCloth = false;
            if (mainCup.IsUsing == 1)
            {
                if (mainCup.HaveCloth == 0)
                {
                    needPutCloth = true;

                }
            }
            if (subCup.IsUsing == 1)
            {
                if (subCup.HaveCloth == 0)
                {
                    needPutCloth = true;
                }
            }

            if (!needPutCloth)
            {
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "无需放布";
                return result;
            }



            var task = new SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>
            {
                Priority = dyeType == 1 ?
            BigProcess.DyeingProcess * 100 +
            SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.PutClothProcess :
            BigProcess.PostProcess * 100 +
            SmartColor.My_ConPar.Order.PostProcess.PostProcess.PutClothProcess,
                OriginalPriority = dyeType == 1 ?
            BigProcess.DyeingProcess * 100 + SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.PutClothProcess :
            BigProcess.PostProcess * 100 +
            SmartColor.My_ConPar.Order.PostProcess.PostProcess.PutClothProcess,
                BusinessType = dyeType == 1 ? SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DyeingProcess :
            SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.PostProcess,
                TaskName = $"放布-{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}",
                PrecomputeAsync = () => PrecomputePutClothAsync(cupNo)
            };
            task.BusinessFlow = async () =>
            {
                var precomputed = task.PrecomputeResult;
                return await PutClothAsync(cupNo, precomputed);
            };

            int[] needPutClothCups = My_Tool.CupAuxiliary.GetIsUseing(cupNo)
                .Split('和')
                .Select(int.Parse)
                .ToArray();
            try
            {
                result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯放布异常", result.Message, async btn =>
                            {
                                if (btn == "确认")
                                {
                                    if (result.Message.Contains("开盖"))
                                    {

                                        var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                        foreach (var c in needPutClothCups)
                                        {

                                            int coverStatus = 0;
                                            if (mainInfo.CupNum == c)
                                            {
                                                coverStatus = mainInfo.CoverStatus ?? 0;
                                            }
                                            else
                                            {
                                                coverStatus = subInfo.CoverStatus ?? 0;
                                            }

                                            if (coverStatus == 2) continue;

                                            SqlServer.Update(
                                            CUP_DETAILS.TableName,
                                            new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 2 } },
                                            $"{CUP_DETAILS.CupNum} = @CupNo",
                                            new SqlParameter("@CupNo", cupNo)
                                        );

                                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                                            if (area != null)
                                            {
                                                var comm = CupCommManager.Instance.GetCommObject(area);
                                                if (comm != null)
                                                {
                                                    await comm.SyncCoverStatus(c);
                                                }
                                                area.OnCupDataReceived(c);
                                            }
                                        }

                                        var retryResult = await EnqueuePutClothAsync(cupNo, dyeType);
                                        tcs.SetResult(retryResult);
                                    }
                                    else if (result.Message.Contains("关盖"))
                                    {
                                        var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                        foreach (var c in needPutClothCups)
                                        {

                                            int coverStatus = 0;
                                            if (mainInfo.CupNum == c)
                                            {
                                                coverStatus = mainInfo.CoverStatus ?? 0;
                                            }
                                            else
                                            {
                                                coverStatus = subInfo.CoverStatus ?? 0;
                                            }

                                            if (coverStatus == 1) continue;


                                            SqlServer.Update(
                                                CUP_DETAILS.TableName,
                                                new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 1 } },
                                                $"{CUP_DETAILS.CupNum} = @CupNo",
                                                new SqlParameter("@CupNo", c)
                                            );

                                            var area = CupCommManager.Instance.FindCupAreaByCupNum(c);
                                            if (area != null)
                                            {
                                                var comm = CupCommManager.Instance.GetCommObject(area);
                                                if (comm != null)
                                                {
                                                    await comm.SyncCoverStatus(c);
                                                }
                                                area.OnCupDataReceived(c);
                                            }
                                        }

                                        tcs.SetResult(new CupRobotTask.TaskResult
                                        {
                                            Code = My_Tool.Result.ResultCode.Success
                                        });
                                    }
                                }

                            },
                            new[] { "确认" },
                            "确认"
                             );
                            return await tcs.Task;
                        }
                    case My_Tool.Result.ResultCode.NeedInteraction:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯放布询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    if (result.Message.Contains("关盖时撑盖开失败"))
                                    {
                                        var retryResult = await EnqueueSupportCoverAsync(cupNo, task.Priority, dyeType == 1 ? SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DyeingProcess :
                   SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.PostProcess);
                                        if (retryResult.Code == Result.ResultCode.Success)
                                        {
                                            retryResult = await EnqueuePutClothAsync(cupNo, dyeType);
                                            tcs.SetResult(retryResult);
                                        }
                                        else
                                        {
                                            tcs.SetResult(retryResult);
                                        }
                                    }
                                    else
                                    {
                                        var retryResult = await EnqueuePutClothAsync(cupNo, dyeType);
                                        tcs.SetResult(retryResult);
                                    }
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                            // 15. 调用PutClothConfirm接口通知HMI
                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                            if (area != null)
                            {
                                var comm = CupCommManager.Instance.GetCommObject(area);
                                if (comm != null)
                                {
                                    await comm.PutClothConfirm(cupNo, "放布");
                                }
                            }
                            return result;
                        }


                }
            }
            catch (TaskCanceledException)
            {
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                Logger.Error("EnqueuePutClothAsync：发生异常。", ex);
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }
        #endregion

        #region 5. 出布任务

        /// <summary>
        /// 出布任务预处理：提前查找所有需要出布的杯的坐标和状态（含杯、杯盖、出布区、湿布夹子）
        /// </summary>
        private static async Task<object> PrecomputeOutClothAsync(int cupNo)
        {
            // 1. 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 2. 过滤需要出布的杯（正在使用且有布且当前步未完成）
            var needOutClothCups = cupInfos
                .Where(info => info.IsUsing == 1 && info.HaveCloth == 1 && info.CurrentStepFinish == 0)
                .ToList();

            // 3. 并发准备每个杯子的所有相关坐标
            var cupTasks = needOutClothCups.Select(async info =>
            {
                int cno = info.CupNum;

                int headID = info.HeadID ?? 0;
                int stepNo = 0;
                int.TryParse(info.StepNum, out stepNo);
                string dyeingCode = info.DyeingCode ?? "";
                int clothPos = 0;
                if (headID != 0)
                {
                    var dropHeadRows = SqlServer.Select(DROP_HEAD.TableName, $"{DROP_HEAD.MyID} = {headID} ");
                    if (dropHeadRows.Rows.Count > 0)
                    {
                        var dropHeadRow = dropHeadRows.Rows[0];
                        if (dropHeadRow.Table.Columns.Contains(DROP_HEAD.ClothNum))
                            clothPos = int.TryParse(dropHeadRow[DROP_HEAD.ClothNum]?.ToString() ?? "0", out int pos) ? pos : 0;
                    }
                }
                // 坐标并发获取
                var cupObjTask = CupCommManager.Instance.FindCupByCupNumAsync(info.CupNum);
                var cupCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cno, false);
                var lidCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cno, true);
                var outClothCTTask = My_Tool.AreaCoordinateFinder.TryGetOutClothCoordinateAsync(clothPos);

                var cupCTR = await cupCTTask;
                var lidCTR = await lidCTTask;
                var outClothCTR = await outClothCTTask;

                var cupObj = await cupObjTask;
                return new
                {
                    Info = info,
                    CupNum = cno,

                    HeadID = headID,
                    StepNo = stepNo,
                    DyeingCode = dyeingCode,
                    CupCTR = cupCTR,
                    LidCTR = lidCTR,
                    ClothPos = clothPos,
                    OutClothCTR = outClothCTR,
                    CupObj = cupObj
                };
            }).ToArray();

            var cupDataList = await Task.WhenAll(cupTasks);
            // 湿布夹子坐标也提前查好
            var wetCTR = await My_Tool.AreaCoordinateFinder.TryGetWetClampCoordinateAsync();
            return new { CupDataList = cupDataList, WetCTR = wetCTR };
        }

        /// <summary>
        /// 出布任务（支持预处理结果）
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> OutClothAsync(List<int> cupNos, object precomputed = null)
        {
            var result = new TaskResult()
            {
                Code = Result.ResultCode.Success
            };
            var currentTask = SmartColor.My_RobotManager.RobotTaskManager.Instance.CurrentTask as SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>;
            try
            {

                dynamic cupDataList;
                dynamic wetCTR;
                if (precomputed != null)
                {
                    dynamic pre = precomputed;
                    cupDataList = pre.CupDataList;
                    wetCTR = pre.WetCTR;
                }
                else
                {
                    // 兼容未预处理的情况
                    var cupTasks = cupNos.Select(async cno =>
                    {
                        var drCover = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {cno}");
                        int coverStatus = 0;
                        int headID = 0, stepNo = 0;
                        string dyeingCode = "";
                        if (drCover.Rows.Count > 0)
                        {
                            var rowCover = drCover.Rows[0];
                            coverStatus = Convert.ToInt16(rowCover.Table.Columns.Contains(CUP_DETAILS.CoverStatus) ? rowCover[CUP_DETAILS.CoverStatus]?.ToString() : "1");
                            dyeingCode = rowCover[CUP_DETAILS.DyeingCode]?.ToString();
                            headID = int.TryParse(rowCover[CUP_DETAILS.HeadID]?.ToString() ?? "0", out int hid) ? hid : 0;
                            stepNo = int.TryParse(rowCover[CUP_DETAILS.StepNum]?.ToString() ?? "0", out int sn) ? sn : 0;
                        }
                        var cupCTR = await My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cno, false);
                        var lidCTR = await My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cno, true);




                        int clothPos = 0;
                        if (headID != 0)
                        {
                            var dropHeadRows = SqlServer.Select(DROP_HEAD.TableName, $"{DROP_HEAD.MyID} = {headID} ");
                            if (dropHeadRows.Rows.Count > 0)
                            {
                                var dropHeadRow = dropHeadRows.Rows[0];
                                if (dropHeadRow.Table.Columns.Contains(DROP_HEAD.ClothNum))
                                    clothPos = int.TryParse(dropHeadRow[DROP_HEAD.ClothNum]?.ToString() ?? "0", out int pos) ? pos : 0;
                            }
                        }
                        var outClothCTR = await My_Tool.AreaCoordinateFinder.TryGetOutClothCoordinateAsync(clothPos);
                        return new
                        {
                            CupNum = cno,
                            CoverStatus = coverStatus,
                            HeadID = headID,
                            StepNo = stepNo,
                            DyeingCode = dyeingCode,
                            CupCTR = cupCTR,
                            LidCTR = lidCTR,
                            OutClothCTR = outClothCTR
                        };
                    }).ToArray();
                    cupDataList = await Task.WhenAll(cupTasks);
                    wetCTR = await My_Tool.AreaCoordinateFinder.TryGetWetClampCoordinateAsync();
                }


                // 1.开盖
                foreach (var cupData in cupDataList)
                {
                    int cno = cupData.CupNum;
                    var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cno);
                    int coverStatus = 0;
                    if (mainInfo.CupNum == cno)
                    {
                        coverStatus = mainInfo.CoverStatus ?? 0;
                    }
                    else if (subInfo.CupNum == cno)
                    {
                        coverStatus = subInfo.CoverStatus ?? 0;
                    }


                    if (coverStatus != 2) // 2=开盖
                    {
                        var openLidResult = await OpenLidAsync(cno, cupData.CupCTR, cupData.LidCTR, cupData.CupObj);
                        if (openLidResult.Code != My_Tool.Result.ResultCode.Success)
                        {
                            result.Code = openLidResult.Code;
                            result.Message = openLidResult.Message;
                            result.Exception = openLidResult.Exception;
                        }
                    }
                }
                if (result.Code != My_Tool.Result.ResultCode.Success)
                    return result;

                if (My_ConPar.Choices.UseClampOut == 0)
                {
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "盖已打开";
                    return result;
                }
                bool needClamp = false;
                foreach (var cupData in cupDataList)
                {
                    var outClothCTR = cupData.OutClothCTR;
                    if (outClothCTR.Item1)
                    {
                        needClamp = true;
                        break;
                    }
                }

                if (!needClamp)
                {
                    // 如果所有杯子都没有出布位坐标，则直接出布完成，把步丢在杯子里，
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "未找到出布位,直接完成";
                    return result;

                }

                // 2.移动到湿布夹子位
                if (!wetCTR.Item1)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "获取湿布夹具坐标异常";
                    result.Exception = new Exception("获取湿布夹具坐标异常");
                    return result;
                }

                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var wetRackPos = await SemiAutoHelperFactory.Current.MoveToWetClampAsync(0, wetCTR.Item2, wetCTR.Item3, 1);
                var semiAutoResult = wetRackPos;
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

                // 3.取湿布夹子
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var takeClip = await SemiAutoHelperFactory.Current.RobotHandGraspingAsync(6, 0);
                semiAutoResult = takeClip;
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

                // 4.遍历出布
                bool stayTank = false;
                int i = 0;
                int outClothTime = My_ConPar.Choices.RepeatGetCloth == 0 ? 1 : 2;
                foreach (var cupData in cupDataList)
                {
                    for (int t = 0; t < outClothTime; t++)
                    {
                        int cno = cupData.CupNum;
                        int headID = cupData.HeadID;
                        int stepNo = cupData.StepNo;
                        string dyeingCode = cupData.DyeingCode;
                        var cupCTR = cupData.CupCTR;
                        var outClothCTR = cupData.OutClothCTR;
                        var clothPos = cupData.ClothPos;
                        if (!outClothCTR.Item1)
                        {
                            stayTank = true;
                            continue;
                        }

                        _ = Task.Run(() =>
                    {
                        SmartColor.My_File.CupTempRecorder.Get(cno).RecordStep("出布");
                    });

                        // 5.移动到杯位
                        if (!cupCTR.Item1)
                        {
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = $"未找到{cno}号杯坐标";
                            result.Exception = new Exception($"未找到{cno}号杯坐标");
                            return result;
                        }
                        var cupTask = CupCommManager.Instance.FindCupByCupNumAsync(cno);
                        if (currentTask != null)
                            await currentTask.CheckPauseOnlyAsync();

                        semiAutoResult = await SemiAutoHelperFactory.Current.MoveToCupAsync(cno, 0, cupCTR.Item2, cupCTR.Item3, 2, 0);
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

                        var cup = await cupTask;
                        if (cup == null)
                        {
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Exception = new Exception($"未找到{cno}号杯杯控件");
                            result.Message = $"未找到{cno}号杯杯控件";
                            return result;
                        }

                        if (currentTask != null)
                            await currentTask.CheckPauseOnlyAsync();

                        var wr = await CupAuxiliary.WaitLockUpOk(cno);
                        if (!wr)
                        {
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Exception = new Exception($"{cno}号杯等待上锁异常");
                            result.Message = $"{cno}号杯等待上锁异常";
                            return result;
                        }

                        // 6.取湿布
                        if (currentTask != null)
                            await currentTask.CheckPauseOnlyAsync();
                        semiAutoResult = await SemiAutoHelperFactory.Current.TakeWetClothAsync();
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

                        // 7.移动到出布区
                        if (!outClothCTR.Item1)
                        {
                            result.Code = My_Tool.Result.ResultCode.Exception;
                            result.Message = $"未找到{cno}号出布位坐标";
                            result.Exception = new Exception($"未找到{cno}号出布位坐标");
                            return result;
                        }

                        if (currentTask != null)
                            await currentTask.CheckPauseOnlyAsync();
                        semiAutoResult = await SemiAutoHelperFactory.Current.MoveToOutClothAsync(clothPos, outClothCTR.Item2, outClothCTR.Item3);
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

                        // 8.放湿布
                        if (i + 1 < cupDataList.Length)
                        {
                            // 预处理已全部查好，不需要再查
                        }

                        if (currentTask != null)
                            await currentTask.CheckPauseOnlyAsync();
                        semiAutoResult = await SemiAutoHelperFactory.Current.PutWetClothAsync((short)My_ConPar.Other.OutClothPosition);
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


                        // 9.更新杯资料无布状态
                        SqlServer.Update(
                            CUP_DETAILS.TableName,
                            new Dictionary<string, object> {
                                { CUP_DETAILS.HaveCloth, 0 },
                                { CUP_DETAILS.CurrentStepFinish,1 }
                            },
                            $"{CUP_DETAILS.CupNum} = @CupNo",
                            new SqlParameter("@CupNo", cno)
                        );
                        var area = My_AutomaticModule.CupCommManager.Instance.FindCupAreaByCupNum(cno);
                        if (area != null)
                        {
                            area.OnCupDataReceived(cno);
                        }

                        // 10.更新dye_details表当前步完成状态
                        bool isWash = My_Tool.CupAuxiliary.WashCupDic.ContainsKey(dyeingCode);
                        if (isWash)
                        {
                            // 清洗杯出布后不更新当前步完成状态，等待清洗杯任务完成后再更新
                            continue;
                        }
                        else
                        {
                            if (headID == 0 || stepNo == 0)
                            {
                                result.Code = My_Tool.Result.ResultCode.Exception;
                                result.Message = $"{cno}杯未找到配方或步号";
                                result.Exception = new Exception($"{cno}杯未找到配方或步号");
                                return result;
                            }

                            SqlServer.Update(
                                DYE_DETAILS.TableName,
                                new Dictionary<string, object> {
                                    { DYE_DETAILS.Finish, 1 }
                                },
                                $"{DYE_DETAILS.HeadID} = @HeadID AND {DYE_DETAILS.StepNum} = @StepNo",
                                new SqlParameter("@HeadID", headID),
                                new SqlParameter("@StepNo", stepNo)
                            );
                        }
                    }
                    i++;
                }

                // 11.移动到湿布夹子位
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                semiAutoResult = await SemiAutoHelperFactory.Current.MoveToWetClampAsync(0, wetCTR.Item2, wetCTR.Item3, 0);
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

                // 12.放湿布夹子
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                semiAutoResult = await SemiAutoHelperFactory.Current.RobotArmReleaseAsync(0, 0);
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

                // 13.查下一步工艺是否需要关盖，需要关盖则关盖
                My_Tool.CupAuxiliary.StepInfo nextStepInfo;
                if (cupDataList == null || cupDataList.Length == 0)
                {
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "未找到需要出布的杯，cupDataList为空。";
                    return result;
                }
                var firstCup = cupDataList[0];
                bool isFirstWash = My_Tool.CupAuxiliary.WashCupDic.ContainsKey(firstCup.DyeingCode);
                if (isFirstWash)
                {
                    nextStepInfo = My_Tool.CupAuxiliary.GetNextWashCupStepInfo(firstCup.DyeingCode, firstCup.StepNo);
                }
                else
                {
                    nextStepInfo = My_Tool.CupAuxiliary.GetNextStepInfo(firstCup.HeadID, firstCup.StepNo);
                }

                if (nextStepInfo.TechnologyName == "冷行" ||
                    nextStepInfo.TechnologyName == "温控" ||
                    nextStepInfo.TechnologyName == "搅拌")
                {
                    foreach (var cupData in cupDataList)
                    {
                        int cno = cupData.CupNum;
                        var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cno);
                        int coverStatus = 0;
                        if (mainInfo.CupNum == cno)
                        {
                            coverStatus = mainInfo.CoverStatus ?? 0;
                        }
                        else
                        {
                            coverStatus = subInfo.CoverStatus ?? 0;
                        }
                        if (coverStatus != 1)
                        {
                            var closeLidResult = await CloseLidAsync(cno, cupData.CupCTR, cupData.LidCTR, cupData.CupObj);
                            if (closeLidResult.Code != My_Tool.Result.ResultCode.Success)
                            {
                                result.Code = closeLidResult.Code;
                                result.Message = closeLidResult.Message;
                                result.Exception = closeLidResult.Exception;
                            }
                        }
                    }
                    if (result.Code != Result.ResultCode.Success)
                    {
                        return result;
                    }
                }
                else
                {
                    if (nextStepInfo.StepNum == 0)
                    {
                        //发送出布完成消息

                        var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNos[0]);
                        if (area != null)
                        {
                            var comm = CupCommManager.Instance.GetCommObject(area);
                            if (comm != null)
                            {
                                await comm.SendShowSure(cupNos[0], 1);
                            }
                        }
                    }
                }

                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = stayTank == false ? "出布完成" : "未找到出布位,直接完成";
            }
            catch (Exception ex)
            {
                Logger.Error("OutClothAsync：发生异常。", ex);
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = "出布异常：" + ex.Message;
                result.Exception = ex;
            }
            return result;
        }

        public static async Task<CupRobotTask.TaskResult> EnqueueOutClothAsync(int cupNo, int dyeType)
        {
            var result = new TaskResult();
            // 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 只对正在使用且有布的杯进行出布
            var needOutClothCups = new List<int>();
            foreach (var info in cupInfos)
            {
                if (info.IsUsing == 1 && info.HaveCloth == 1 && info.CurrentStepFinish == 0)
                {

                    needOutClothCups.Add(info.CupNum);
                }

            }

            // 如果主副杯都无布或未使用，直接返回
            if (needOutClothCups.Count == 0)
            {
                await RunTableMan.InsertAsync(new Dictionary<string, object>
                {
                    [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNo}号配液杯出布已经完成触发发送下一步"
                }, DateTime.Now);
                await My_Tool.CupAuxiliary.SendNextStepIfNeeded(cupNo);
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "主副杯均无布或未使用，无需出布";
                return result;
            }



            int priority = int.MaxValue;
            My_ConPar.Order.BigProcess.RobotBusinessType businessType = BigProcess.RobotBusinessType.Debug;
            if (dyeType == 1)
            {
                priority = BigProcess.DyeingProcess * 100 + SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.OutClothProcess;
                businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DyeingProcess;
            }
            else if (dyeType == 2)
            {
                priority = BigProcess.PostProcess * 100 + SmartColor.My_ConPar.Order.PostProcess.PostProcess.OutClothProcess;
                businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.PostProcess;
            }
            else
            {
                if (FixDyeTypeForPreWash(cupNo))
                {
                    priority = BigProcess.DropProcess * 100;
                    businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DropProcess;
                }
            }


            var task = new SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>
            {
                Priority = priority,
                OriginalPriority = priority,
                BusinessType = businessType,
                TaskName = $"出布-{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}",
                PrecomputeAsync = () => PrecomputeOutClothAsync(cupNo)
            };
            task.BusinessFlow = async () =>
            {
                var precomputed = task.PrecomputeResult;
                return await OutClothAsync(needOutClothCups, precomputed);
            };

            try
            {
                result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);

                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯出布异常", result.Message, async btn =>
                            {
                                if (btn == "确认")
                                {
                                    if (result.Message.Contains("开盖"))
                                    {

                                        var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                        foreach (var c in needOutClothCups)
                                        {

                                            int coverStatus = 0;
                                            if (mainInfo.CupNum == c)
                                            {
                                                coverStatus = mainInfo.CoverStatus ?? 0;
                                            }
                                            else
                                            {
                                                coverStatus = subInfo.CoverStatus ?? 0;
                                            }

                                            if (coverStatus == 2) continue;

                                            SqlServer.Update(
                                            CUP_DETAILS.TableName,
                                            new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 2 } },
                                            $"{CUP_DETAILS.CupNum} = @CupNo",
                                            new SqlParameter("@CupNo", cupNo)
                                        );

                                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                                            if (area != null)
                                            {
                                                var comm = CupCommManager.Instance.GetCommObject(area);
                                                if (comm != null)
                                                {
                                                    await comm.SyncCoverStatus(c);
                                                }
                                                area.OnCupDataReceived(c);
                                            }
                                        }

                                        var retryResult = await EnqueueOutClothAsync(cupNo, dyeType);
                                        tcs.SetResult(retryResult);
                                    }
                                    else if (result.Message.Contains("关盖"))
                                    {

                                        var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                        foreach (var c in needOutClothCups)
                                        {

                                            int coverStatus = 0;
                                            if (mainInfo.CupNum == c)
                                            {
                                                coverStatus = mainInfo.CoverStatus ?? 0;
                                            }
                                            else
                                            {
                                                coverStatus = subInfo.CoverStatus ?? 0;
                                            }

                                            if (coverStatus == 1) continue;


                                            SqlServer.Update(
                                            CUP_DETAILS.TableName,
                                            new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 1 } },
                                            $"{CUP_DETAILS.CupNum} = @CupNo",
                                            new SqlParameter("@CupNo", c)
                                        );

                                            var area = CupCommManager.Instance.FindCupAreaByCupNum(c);
                                            if (area != null)
                                            {
                                                var comm = CupCommManager.Instance.GetCommObject(area);
                                                if (comm != null)
                                                {
                                                    await comm.SyncCoverStatus(c);
                                                }
                                                area.OnCupDataReceived(c);
                                            }
                                        }

                                        tcs.SetResult(new CupRobotTask.TaskResult
                                        {
                                            Code = My_Tool.Result.ResultCode.Success
                                        });
                                    }
                                }

                            },
                            new[] { "确认" },
                            "确认"
                             );
                            return await tcs.Task;
                        }
                    case My_Tool.Result.ResultCode.NeedInteraction:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯出布询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {

                                    if (result.Message.Contains("关盖时撑盖开失败"))
                                    {
                                        var retryResult = await EnqueueSupportCoverAsync(cupNo, task.Priority, businessType);
                                        if (retryResult.Code == Result.ResultCode.Success)
                                        {
                                            retryResult = await EnqueueOutClothAsync(cupNo, dyeType);
                                            tcs.SetResult(retryResult);
                                        }
                                        else
                                        {
                                            tcs.SetResult(retryResult);
                                        }
                                    }
                                    else
                                    {
                                        var retryResult = await EnqueueOutClothAsync(cupNo, dyeType);
                                        tcs.SetResult(retryResult);
                                    }
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                            // 15. 调用PutClothConfirm接口通知HMI
                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                            if (area != null)
                            {
                                var comm = CupCommManager.Instance.GetCommObject(area);
                                if (comm != null)
                                {
                                    await comm.PutClothConfirm(cupNo, "出布", result.Message == "未找到出布位,直接完成");
                                }
                            }
                            return result;
                        }


                }
            }
            catch (TaskCanceledException)
            {
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                Logger.Error("EnqueueOutClothAsync：发生异常。", ex);
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };

            }
        }
        #endregion

        #region 6. 排液任务

        /// <summary>
        /// 排液任务预处理：提前查找所有需要排液的杯的坐标和状态
        /// </summary>
        private static async Task<object> PrecomputeDrainAsync(int cupNo)
        {
            // 1. 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 2. 过滤需要排液的杯（正在使用、关盖）
            var needDrainCups = cupInfos
                .Where(info => info.IsUsing == 1 && info.CoverStatus == 1)
                .ToList();

            // 3. 并发准备每个杯子的坐标和状态
            var cupTasks = needDrainCups.Select(async info =>
            {
                var cupObjTask = CupCommManager.Instance.FindCupByCupNumAsync(info.CupNum);
                var cupCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, false);
                var lidCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, true);
                int cno = info.CupNum;

                var cupCTR = await cupCTTask;
                var lidCTR = await lidCTTask;
                var cupObj = await cupObjTask;
                return new
                {
                    Info = info,
                    CupNum = cno,

                    CupCTR = cupCTR,
                    LidCTR = lidCTR,
                    CupObj = cupObj
                };
            }).ToArray();

            var cupDataList = await Task.WhenAll(cupTasks);
            return cupDataList;
        }

        /// <summary>
        /// 排液任务（支持预处理结果）
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> DrainAsync(List<int> cupNos, object precomputed = null)
        {
            var result = new TaskResult()
            {
                Code = My_Tool.Result.ResultCode.Success,
                Message = "排液开盖完成"
            };

            dynamic[] cupDataList;
            if (precomputed is Array arr)
            {
                cupDataList = arr.Cast<dynamic>().ToArray();
            }
            else
            {
                // 兼容未预处理的情况
                cupDataList = cupNos.Select(cupNo => new { CupNum = cupNo, CupCTR = (object)null, LidCTR = (object)null }).ToArray();
            }

            foreach (var cupData in cupDataList)
            {
                int cupNo = cupData.CupNum;
                var cupCTR = cupData.CupCTR;
                var lidCTR = cupData.LidCTR;
                var ctCup = cupData.CupObj;
                Task task = Task.Run(() =>
                {
                    CupTempRecorder.Get(cupNo).RecordStep("排液");
                });

                var result1 = await OpenLidAsync(cupNo, cupCTR, lidCTR, ctCup);
                if (result1.Code != My_Tool.Result.ResultCode.Success)
                {
                    result = result1;
                }
            }
            return result;
        }

        /// <summary>
        /// 提交排液任务到任务中心（带预处理）
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> EnqueueDrainAsync(int cupNo, int dyeType)
        {
            var result = new TaskResult();

            // 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 只对正在使用且关盖的杯进行开盖排液
            var needOpenCups = new List<int>();
            foreach (var info in cupInfos)
            {
                if (info.IsUsing == 1)
                {
                    if (info.CoverStatus == null || info.CoverStatus == 0)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = $"{info.CupNum}杯盖状态未知";
                        result.Exception = new Exception($"{info.CupNum}杯盖状态未知");
                        return result;
                    }
                    if (info.CoverStatus == 1) // 1=关盖
                    {
                        needOpenCups.Add(info.CupNum);
                    }
                    else if (info.CoverStatus == 2)
                    {
                        _ = Task.Run(() =>
                        {
                            CupTempRecorder.Get(info.CupNum).RecordStep("排液");
                        });
                    }
                }
            }

            // 如果主副杯都为开盖或未使用，直接返回
            if (needOpenCups.Count == 0)
            {
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "主副杯均为开盖或未使用，无需开盖";
                return result;
            }

            int priority = int.MaxValue;
            My_ConPar.Order.BigProcess.RobotBusinessType businessType = BigProcess.RobotBusinessType.Debug;
            if (dyeType == 1)
            {
                priority = BigProcess.DyeingProcess * 100 + SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.DrainProcess;
                businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DyeingProcess;
            }
            else if (dyeType == 2)
            {
                priority = BigProcess.PostProcess * 100 + SmartColor.My_ConPar.Order.PostProcess.PostProcess.DrainProcess;
                businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.PostProcess;
            }
            else
            {
                if (FixDyeTypeForPreWash(cupNo))
                {
                    priority = BigProcess.DropProcess * 100;
                    businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DropProcess;
                }
            }

            var task = new SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>
            {
                Priority = priority,
                OriginalPriority = priority,
                BusinessType = businessType,
                TaskName = $"排液-{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}",
                PrecomputeAsync = () => PrecomputeDrainAsync(cupNo)
            };
            task.BusinessFlow = async () =>
            {
                var precomputed = task.PrecomputeResult;
                return await DrainAsync(needOpenCups, precomputed);
            };

            try
            {
                result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯排液异常", result.Message, async btn =>
                            {
                                if (btn == "确认")
                                {
                                    var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                    foreach (var c in needOpenCups)
                                    {

                                        int coverStatus = 0;
                                        if (mainInfo.CupNum == c)
                                        {
                                            coverStatus = mainInfo.CoverStatus ?? 0;
                                        }
                                        else
                                        {
                                            coverStatus = subInfo.CoverStatus ?? 0;
                                        }

                                        if (coverStatus == 2) continue;


                                        SqlServer.Update(
                                            CUP_DETAILS.TableName,
                                            new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 2 } },
                                            $"{CUP_DETAILS.CupNum} = @CupNo",
                                            new SqlParameter("@CupNo", c)
                                        );

                                        var area = CupCommManager.Instance.FindCupAreaByCupNum(c);
                                        if (area != null)
                                        {
                                            var comm = CupCommManager.Instance.GetCommObject(area);
                                            if (comm != null)
                                            {
                                                await comm.SyncCoverStatus(c);
                                            }
                                            area.OnCupDataReceived(c);
                                        }
                                    }

                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯排液询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueDrainAsync(cupNo, dyeType);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                result.Code = My_Tool.Result.ResultCode.Canceled;
                result.Message = "任务被取消";
            }
            catch (Exception ex)
            {
                Logger.Error("EnqueueDrainAsync：发生异常。", ex);
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = "代码异常：" + ex.Message;
                result.Exception = ex;
            }
            return result;
        }
        #endregion

        #region 7. 加水任务

        /// <summary>
        /// 加水任务预处理：提前查找和计算所有加水相关数据
        /// </summary>
        private static async Task<object> PrecomputeAddWaterAsync(int cupNo)
        {
            // 1. 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 2. 过滤需要加水的杯
            var needAddWaterCups = cupInfos
                .Where(info => info.IsUsing == 1 && info.TechnologyName == "加水" && info.CurrentStepFinish == 0)
                .ToList();

            // 3. 并发准备每个杯子的加水前置数据
            var cupTasks = needAddWaterCups.Select(async info =>
            {
                var cupObjTask = CupCommManager.Instance.FindCupByCupNumAsync(info.CupNum);
                var cupCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, false);
                var lidCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, true);

                int cno = info.CupNum;

                double currentWeight = Convert.ToDouble(info.CurrentWeight ?? 0);
                string dyeingCode = info.DyeingCode ?? "";
                int stepNo = 0;
                int.TryParse(info.StepNum, out stepNo);
                int headID = info.HeadID ?? 0;
                bool isWash = My_Tool.CupAuxiliary.WashCupDic.ContainsKey(dyeingCode);
                double objectWeight = 0;

                // 计算加水量
                if (isWash)
                {
                    bool isHighTemp = dyeingCode == My_Tool.CupAuxiliary.HighTempWashCupType;
                    var cupArea = CupCommManager.Instance.FindCupAreaByCupNum(cno);
                    if (cupArea != null)
                    {
                        if (cupArea.AreaType == 2)
                            objectWeight = isHighTemp ? My_ConPar.WashCup.HighTempWash_BigAddWater : My_ConPar.WashCup.Wash_BigAddWater;
                        else
                            objectWeight = isHighTemp ? My_ConPar.WashCup.HighTempWash_AddWater : My_ConPar.WashCup.Wash_AddWater;
                    }
                }
                else
                {
                    if (headID != 0 && stepNo != 0)
                    {
                        var dyedr = SqlServer.Select(DYE_DETAILS.TableName, $"{DYE_DETAILS.HeadID} = {headID} AND {DYE_DETAILS.StepNum} = {stepNo}");
                        if (dyedr.Rows.Count > 0)
                        {
                            var dyeRow = dyedr.Rows[0];
                            objectWeight = Convert.ToDouble(dyeRow.Table.Columns.Contains(DYE_DETAILS.ObjectWaterWeight) ? dyeRow[DYE_DETAILS.ObjectWaterWeight]?.ToString() : "0");
                        }
                    }
                }

                var cupCTR = await cupCTTask;
                var lidCTR = await lidCTTask;
                var cupObj = await cupObjTask;
                return new
                {
                    Info = info,
                    CupNum = cno,

                    CurrentWeight = currentWeight,
                    DyeingCode = dyeingCode,
                    StepNo = stepNo,
                    HeadID = headID,
                    IsWash = isWash,
                    ObjectWeight = objectWeight,
                    CupCTR = cupCTR,
                    LidCTR = lidCTR,
                    CupObj = cupObj
                };
            }).ToArray();

            var cupDataList = await Task.WhenAll(cupTasks);
            return cupDataList;
        }

        /// <summary>
        /// 单杯加水任务（支持预处理结果）
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> AddWaterAsync(int cupNo, object precomputed = null)
        {
            var result = new TaskResult()
            {
                Code = Result.ResultCode.Success
            };
            var currentTask = SmartColor.My_RobotManager.RobotTaskManager.Instance.CurrentTask as SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>;
            try
            {
                // 1. 获取预处理数据或实时查找
                dynamic[] cupDataList;
                if (precomputed is Array arr)
                {
                    cupDataList = arr.Cast<dynamic>().ToArray();
                }
                else
                {
                    // 兼容未预处理的情况，直接调用原有逻辑
                    var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                    var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
                    if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);
                    var needAddWaterCups = cupInfos
                        .Where(info => info.IsUsing == 1 && info.TechnologyName == "加水" && info.CurrentStepFinish == 0)
                        .ToList();
                    var cupTasks = needAddWaterCups.Select(async info =>
                    {
                        var cupCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, false);
                        var lidCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, true);
                        int cno = info.CupNum;

                        double currentWeight = Convert.ToDouble(info.CurrentWeight ?? 0);
                        string dyeingCode = info.DyeingCode ?? "";
                        int stepNo = 0;
                        int.TryParse(info.StepNum, out stepNo);
                        int headID = info.HeadID ?? 0;
                        bool isWash = My_Tool.CupAuxiliary.WashCupDic.ContainsKey(dyeingCode);
                        double objectWeight = 0;
                        if (isWash)
                        {
                            bool isHighTemp = dyeingCode == My_Tool.CupAuxiliary.HighTempWashCupType;
                            var cupArea = CupCommManager.Instance.FindCupAreaByCupNum(cno);
                            if (cupArea != null)
                            {
                                if (cupArea.AreaType == 2)
                                    objectWeight = isHighTemp ? My_ConPar.WashCup.HighTempWash_BigAddWater : My_ConPar.WashCup.Wash_BigAddWater;
                                else
                                    objectWeight = isHighTemp ? My_ConPar.WashCup.HighTempWash_AddWater : My_ConPar.WashCup.Wash_AddWater;
                            }
                        }
                        else
                        {
                            if (headID != 0 && stepNo != 0)
                            {
                                var dyedr = SqlServer.Select(DYE_DETAILS.TableName, $"{DYE_DETAILS.HeadID} = {headID} AND {DYE_DETAILS.StepNum} = {stepNo}");
                                if (dyedr.Rows.Count > 0)
                                {
                                    var dyeRow = dyedr.Rows[0];
                                    objectWeight = Convert.ToDouble(dyeRow.Table.Columns.Contains(DYE_DETAILS.ObjectWaterWeight) ? dyeRow[DYE_DETAILS.ObjectWaterWeight]?.ToString() : "0");
                                }
                            }
                        }
                        var cupCTR = await cupCTTask;
                        var lidCTR = await lidCTTask;
                        return new
                        {
                            Info = info,
                            CupNum = cno,

                            CurrentWeight = currentWeight,
                            DyeingCode = dyeingCode,
                            StepNo = stepNo,
                            HeadID = headID,
                            IsWash = isWash,
                            ObjectWeight = objectWeight,
                            CupCTR = cupCTR,
                            LidCTR = lidCTR
                        };
                    }).ToArray();
                    cupDataList = await Task.WhenAll(cupTasks);
                }

                if (cupDataList.Length == 0)
                {
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "无需加水";
                    return result;
                }





                // 2. 机械手操作部分需串行，防止物理冲突
                foreach (var cupData in cupDataList)
                {
                    var info = cupData.Info;
                    int cno = cupData.CupNum;

                    double currentWeight = cupData.CurrentWeight;
                    string dyeingCode = cupData.DyeingCode;
                    int stepNo = cupData.StepNo;
                    int headID = cupData.HeadID;
                    bool isWash = cupData.IsWash;
                    double objectWeight = cupData.ObjectWeight;
                    var cupCTR = cupData.CupCTR;

                    // 记录步骤
                    _ = Task.Run(() => { CupTempRecorder.Get(cno).RecordStep("加水"); });
                    var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cno);
                    int coverStatus = 0;
                    if (mainInfo.CupNum == cno)
                    {
                        coverStatus = mainInfo.CoverStatus ?? 0;
                    }
                    else if (subInfo.CupNum == cno)
                    {
                        coverStatus = subInfo.CoverStatus ?? 0;
                    }

                    // 检查盖子状态
                    if (coverStatus == 0)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = $"{cno}杯盖状态未知";
                        result.Exception = new Exception($"{cno}杯盖状态未知");
                        return result;
                    }
                    if (coverStatus == 1)
                    {
                        // 机械手开盖
                        var openLidResult = await OpenLidAsync(cno, cupData.CupCTR, cupData.LidCTR, cupData.CupObj);

                        if (openLidResult.Code != My_Tool.Result.ResultCode.Success)
                        {
                            result.Code = openLidResult.Code;
                            result.Message = openLidResult.Message;
                            result.Exception = openLidResult.Exception;
                        }
                    }

                    if (result.Code != My_Tool.Result.ResultCode.Success)
                    {
                        return result;
                    }

                    // 机械手移动到杯
                    if (!cupCTR.Item1)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = $"获取{cno}号杯坐标异常";
                        result.Exception = new Exception($"获取{cno}号杯坐标异常");
                        return result;
                    }

                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();
                    var semiAutoResult = await SemiAutoHelperFactory.Current.MoveToCupAsync(cno, 0, cupCTR.Item2, cupCTR.Item3, 1, 0);
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

                    // 机械手加水
                    if (currentTask != null)
                        await currentTask.CheckPauseOnlyAsync();
                    double addTime = objectWeight / SmartColor.My_ConPar.Correction.Correcting_Water_Value;
                    semiAutoResult = await SemiAutoHelperFactory.Current.AddWaterAsync(objectWeight, addTime);
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

                    // 更新数据库
                    SqlServer.Update(CUP_DETAILS.TableName,
                        new Dictionary<string, object>
                        {
                    {CUP_DETAILS.CurrentWeight, currentWeight + objectWeight },
                    {CUP_DETAILS.CurrentStepFinish, 1 }
                        },
                        $"{CUP_DETAILS.CupNum} = @cupNo",
                        new SqlParameter("@cupNo", cno));
                    var area1 = My_AutomaticModule.CupCommManager.Instance.FindCupAreaByCupNum(cno);
                    if (area1 != null)
                    {
                        area1.OnCupDataReceived(cno);
                    }

                    if (!isWash)
                    {
                        SqlServer.Update(DYE_DETAILS.TableName,
                            new Dictionary<string, object>
                            {
                        {DYE_DETAILS.Finish, 1 }
                            },
                            $"{DYE_DETAILS.HeadID} = @HeadID AND {DYE_DETAILS.StepNum} = @StepNum",
                            new SqlParameter("@HeadID", headID),
                            new SqlParameter("@StepNum", stepNo));
                    }
                }

                // 5. 只查一次下一步工艺（主副杯共用）
                var firstCup = cupDataList[0].Info;
                My_Tool.CupAuxiliary.StepInfo nextStepInfo = My_Tool.CupAuxiliary.WashCupDic.ContainsKey(firstCup.DyeingCode ?? "")
                    ? My_Tool.CupAuxiliary.GetNextWashCupStepInfo(firstCup.DyeingCode ?? "", int.TryParse(firstCup.StepNum, out int sn) ? sn : 0)
                    : My_Tool.CupAuxiliary.GetNextStepInfo(firstCup.HeadID ?? 0, int.TryParse(firstCup.StepNum, out int sn2) ? sn2 : 0);

                // 6. 判断是否需要关盖（主副杯一起关）
                if (nextStepInfo.TechnologyName == "冷行" ||
                    nextStepInfo.TechnologyName == "温控" ||
                    nextStepInfo.TechnologyName == "搅拌")
                {
                    var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                    foreach (var cupData in cupDataList)
                    {
                        var cno = cupData.CupNum;

                        int coverStatus = 0;
                        if (mainInfo.CupNum == cno)
                        {
                            coverStatus = mainInfo.CoverStatus ?? 0;
                        }
                        else if (subInfo.CupNum == cno)
                        {
                            coverStatus = subInfo.CoverStatus ?? 0;
                        }
                        if (coverStatus != 1)
                        {
                            var closeLidResult = await CloseLidAsync(cupData.CupNum, cupData.CupCTR, cupData.LidCTR, cupData.CupObj);
                            if (closeLidResult.Code != My_Tool.Result.ResultCode.Success)
                            {
                                result.Code = closeLidResult.Code;
                                result.Message = closeLidResult.Message;
                                result.Exception = closeLidResult.Exception;
                            }
                        }
                    }
                    if (result.Code != Result.ResultCode.Success)
                    {
                        return result;
                    }
                }

                // 7. 通知HMI主副杯加水完成（只发一次），并发送下一步工艺指令
                var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                if (area != null)
                {
                    var comm = CupCommManager.Instance.GetCommObject(area);
                    if (comm != null)
                    {
                        _ = Task.Run(async () =>
                          {
                              await RunTableMan.InsertAsync(new Dictionary<string, object>
                              {
                                  [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNo}号配液杯加水完成触发发送下一步"
                              }, DateTime.Now);
                              await comm.SendNextStep(cupNo, nextStepInfo);
                              await comm.SendAddChemicaFinish(cupNo);
                          });

                    }
                }

                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "加水完成";
            }
            catch (Exception ex)
            {
                Logger.Error("AddWaterAsync：发生异常。", ex);
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = "加水异常：" + ex.Message;
                result.Exception = ex;
            }
            return result;
        }

        /// <summary>
        /// 提交加水任务到任务中心（带预处理）
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> EnqueueAddWaterAsync(int cupNo, int dyeType)
        {
            TaskResult result = new TaskResult();
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            var needAddWaterCups = My_Tool.CupAuxiliary.GetNeedActionCupNos(cupInfos, "加水");
            if (needAddWaterCups.Count == 0)
            {
                await RunTableMan.InsertAsync(new Dictionary<string, object>
                {
                    [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNo}号配液杯加水已经完成触发发送下一步"
                }, DateTime.Now);
                await My_Tool.CupAuxiliary.SendNextStepIfNeeded(cupNo);
                return new TaskResult
                {

                    Code = My_Tool.Result.ResultCode.Success,
                    Message = "主副杯当前步已完成，无需加水"
                };
            }

            int priority = int.MaxValue;
            My_ConPar.Order.BigProcess.RobotBusinessType businessType = BigProcess.RobotBusinessType.Debug;
            if (dyeType == 1)
            {
                priority = BigProcess.DyeingProcess * 100 + SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.AddWaterProcess;
                businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DyeingProcess;
            }
            else if (dyeType == 2)
            {
                priority = BigProcess.PostProcess * 100 + SmartColor.My_ConPar.Order.PostProcess.PostProcess.AddWaterProcess;
                businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.PostProcess;
            }
            else
            {
                if (FixDyeTypeForPreWash(cupNo))
                {
                    priority = BigProcess.DropProcess * 100;
                    businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DropProcess;
                }
            }
            SmartColor.My_RobotManager.RobotBusinessTask<TaskResult> task = null;
            task = new SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>
            {
                Priority = priority,
                OriginalPriority = priority,
                BusinessType = businessType,
                TaskName = $"加水-{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}",
                PrecomputeAsync = () => PrecomputeAddWaterAsync(cupNo)
            };
            task.BusinessFlow = async () =>
            {
                var precomputed = task.PrecomputeResult;
                return await AddWaterAsync(cupNo, precomputed);
            };

            try
            {
                result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯加水异常", result.Message, async btn =>
                            {
                                if (btn == "确认")
                                {

                                    var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                    foreach (var c in needAddWaterCups)
                                    {

                                        int coverStatus = 0;
                                        if (mainInfo.CupNum == c)
                                        {
                                            coverStatus = mainInfo.CoverStatus ?? 0;
                                        }
                                        else
                                        {
                                            coverStatus = subInfo.CoverStatus ?? 0;
                                        }

                                        if (coverStatus == 2) continue;
                                        SqlServer.Update(
                                        CUP_DETAILS.TableName,
                                        new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 2 } },
                                        $"{CUP_DETAILS.CupNum} = @CupNo",
                                        new SqlParameter("@CupNo", c)
                                    );

                                        var area = CupCommManager.Instance.FindCupAreaByCupNum(c);
                                        if (area != null)
                                        {
                                            var comm = CupCommManager.Instance.GetCommObject(area);
                                            if (comm != null)
                                            {
                                                await comm.SyncCoverStatus(c);
                                            }
                                            area.OnCupDataReceived(c);
                                        }
                                    }

                                    var retryResult = await EnqueueAddWaterAsync(cupNo, dyeType);
                                    tcs.SetResult(retryResult);
                                }

                            },
                            new[] { "确认" },
                            "确认"
                            );
                            return await tcs.Task;
                        }
                    case My_Tool.Result.ResultCode.NeedInteraction:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯加水询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    if (result.Message.Contains("关盖时撑盖开失败"))
                                    {
                                        var retryResult = await EnqueueSupportCoverAsync(cupNo, task.Priority, businessType);
                                        if (retryResult.Code == Result.ResultCode.Success)
                                        {
                                            retryResult = await EnqueueAddWaterAsync(cupNo, dyeType);
                                            tcs.SetResult(retryResult);
                                        }
                                        else
                                        {
                                            tcs.SetResult(retryResult);
                                        }
                                    }
                                    else
                                    {
                                        var retryResult = await EnqueueAddWaterAsync(cupNo, dyeType);
                                        tcs.SetResult(retryResult);
                                    }
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                Logger.Error("EnqueueAddWaterAsync：发生异常。", ex);
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }
        #endregion

        #region 8. 搅拌任务
        /// <summary>
        /// 搅拌任务预处理：提前查找所有需要搅拌的杯的坐标和状态
        /// </summary>
        private static async Task<object> PrecomputeStirAsync(int cupNo)
        {
            // 1. 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 2. 过滤需要搅拌的杯（开盖、当前步未完成、正在使用）
            var needStirCups = cupInfos
                .Where(info => info.IsUsing == 1 && info.CoverStatus == 2 && info.CurrentStepFinish == 0)
                .ToList();

            // 3. 并发准备每个杯子的坐标和状态
            var cupTasks = needStirCups.Select(async info =>
            {
                var cupObjTask = CupCommManager.Instance.FindCupByCupNumAsync(info.CupNum);
                var cupCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, false);
                var lidCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, true);
                int cno = info.CupNum;

                var cupCTR = await cupCTTask;
                var lidCTR = await lidCTTask;
                var cupObj = await cupObjTask;
                return new
                {
                    Info = info,
                    CupNum = cno,

                    CupCTR = cupCTR,
                    LidCTR = lidCTR,
                    CupObj = cupObj
                };
            }).ToArray();

            var cupDataList = await Task.WhenAll(cupTasks);
            return cupDataList;
        }

        public static async Task<CupRobotTask.TaskResult> StirAsync(List<int> cupNos, object precomputed = null)
        {
            var result = new TaskResult()
            {
                Code = My_Tool.Result.ResultCode.Success,
                Message = "搅拌关盖完成"
            };

            dynamic[] cupDataList;
            if (precomputed is Array arr)
            {
                cupDataList = arr.Cast<dynamic>().ToArray();
            }
            else
            {
                // 兼容未预处理的情况
                cupDataList = cupNos.Select(cupNo => new { CupNum = cupNo, CupCTR = (object)null, LidCTR = (object)null }).ToArray();
            }

            foreach (var cupData in cupDataList)
            {
                int cupNo = cupData.CupNum;
                var cupCTR = cupData.CupCTR;
                var lidCTR = cupData.LidCTR;
                var ctCup = cupData.CupObj;
                _ = Task.Run(() =>
                {
                    CupTempRecorder.Get(cupNo).RecordStep("搅拌");
                });

                var result1 = await CloseLidAsync(cupNo, cupCTR, lidCTR, ctCup);
                if (result1.Code != My_Tool.Result.ResultCode.Success)
                {
                    result = result1;
                }
            }
            return result;
        }

        public static async Task<CupRobotTask.TaskResult> EnqueueStirAsync(int cupNo, int dyeType)
        {
            var result = new TaskResult();

            // 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 只对正在使用、开盖、当前步未完成的杯进行关盖
            var needCloseCups = new List<int>();
            foreach (var info in cupInfos)
            {
                if (info.IsUsing == 1 && info.CoverStatus == 2 && info.CurrentStepFinish == 0)
                {
                    needCloseCups.Add(info.CupNum);
                }
                else if (info.IsUsing == 1 && (info.CoverStatus == null || info.CoverStatus == 0))
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"{info.CupNum}杯盖状态未知";
                    result.Exception = new Exception($"{info.CupNum}杯盖状态未知");
                    return result;
                }
                else if (info.IsUsing == 1 && info.CoverStatus == 1 && info.CurrentStepFinish == 0)
                {
                    _ = Task.Run(() =>
                    {
                        CupTempRecorder.Get(info.CupNum).RecordStep("搅拌");
                    });
                }
            }

            // 如果主副杯都为关盖或未使用或已完成，直接返回
            if (needCloseCups.Count == 0)
            {
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "主副杯均为关盖或未使用或当前步已完成，无需关盖";
                return result;
            }

            int priority = int.MaxValue;
            My_ConPar.Order.BigProcess.RobotBusinessType businessType = BigProcess.RobotBusinessType.Debug;
            if (dyeType == 1)
            {
                priority = BigProcess.DyeingProcess * 100 + SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.StirProcess;
                businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DyeingProcess;
            }
            else if (dyeType == 2)
            {
                priority = BigProcess.PostProcess * 100 + SmartColor.My_ConPar.Order.PostProcess.PostProcess.StirProcess;
                businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.PostProcess;
            }
            else
            {
                if (FixDyeTypeForPreWash(cupNo))
                {
                    priority = BigProcess.DropProcess * 100;
                    businessType = SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DropProcess;
                }
            }


            var task = new SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>
            {
                Priority = priority,
                OriginalPriority = priority,
                BusinessType = businessType,
                TaskName = $"搅拌-{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}",
                PrecomputeAsync = () => PrecomputeStirAsync(cupNo)
            };
            task.BusinessFlow = async () =>
            {
                var precomputed = task.PrecomputeResult;
                return await StirAsync(needCloseCups, precomputed);
            };

            try
            {
                result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯搅拌异常", result.Message, async btn =>
                            {
                                if (btn == "确认")
                                {

                                    var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                    foreach (var c in needCloseCups)
                                    {

                                        int coverStatus = 0;
                                        if (mainInfo.CupNum == c)
                                        {
                                            coverStatus = mainInfo.CoverStatus ?? 0;
                                        }
                                        else
                                        {
                                            coverStatus = subInfo.CoverStatus ?? 0;
                                        }

                                        if (coverStatus == 1) continue;

                                        SqlServer.Update(
                                        CUP_DETAILS.TableName,
                                        new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 1 } },
                                        $"{CUP_DETAILS.CupNum} = @CupNo",
                                        new SqlParameter("@CupNo", c)
                                    );

                                        var area = CupCommManager.Instance.FindCupAreaByCupNum(c);
                                        if (area != null)
                                        {
                                            var comm = CupCommManager.Instance.GetCommObject(area);
                                            if (comm != null)
                                            {
                                                await comm.SyncCoverStatus(c);
                                            }
                                            area.OnCupDataReceived(c);
                                        }
                                    }



                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯搅拌询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    if (result.Message.Contains("关盖时撑盖开失败"))
                                    {
                                        var retryResult = await EnqueueSupportCoverAsync(cupNo, task.Priority, businessType);
                                        if (retryResult.Code == Result.ResultCode.Success)
                                        {
                                            retryResult = await EnqueueStirAsync(cupNo, dyeType);
                                            tcs.SetResult(retryResult);
                                        }
                                        else
                                        {
                                            tcs.SetResult(retryResult);
                                        }
                                    }
                                    else
                                    {
                                        var retryResult = await EnqueueStirAsync(cupNo, dyeType);
                                        tcs.SetResult(retryResult);
                                    }
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                Logger.Error("EnqueueStirAsync：发生异常。", ex);
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }


        }
        #endregion

        #region 9. 取小样任务
        // 9. 取小样任务

        /// <summary>
        /// 取小样任务预处理：提前查找所有需要取小样的杯的坐标和状态
        /// </summary>
        private static async Task<object> PrecomputeSampleAsync(int cupNo)
        {
            // 1. 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 2. 过滤需要取小样的杯（正在使用、关盖、当前步未完成）
            var needSampleCups = cupInfos
                .Where(info => info.IsUsing == 1 && info.CoverStatus == 1 && info.CurrentStepFinish == 0)
                .ToList();

            // 3. 并发准备每个杯子的坐标和状态
            var cupTasks = needSampleCups.Select(async info =>
            {
                var cupObjTask = CupCommManager.Instance.FindCupByCupNumAsync(info.CupNum);
                var cupCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, false);
                var lidCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, true);
                int cno = info.CupNum;

                var cupCTR = await cupCTTask;
                var lidCTR = await lidCTTask;
                var cupObj = await cupObjTask;
                return new
                {
                    Info = info,
                    CupNum = cno,

                    CupCTR = cupCTR,
                    LidCTR = lidCTR,
                    CupObj = cupObj
                };
            }).ToArray();

            var cupDataList = await Task.WhenAll(cupTasks);
            return cupDataList;
        }

        /// <summary>
        /// 取小样任务（支持预处理结果）
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> SampleAsync(List<int> cupNos, object precomputed = null)
        {
            var result = new TaskResult()
            {
                Code = My_Tool.Result.ResultCode.Success,
                Message = "取小样开盖完成"
            };

            dynamic[] cupDataList;
            if (precomputed is Array arr)
            {
                cupDataList = arr.Cast<dynamic>().ToArray();
            }
            else
            {
                // 兼容未预处理的情况
                cupDataList = cupNos.Select(cupNo => new { CupNum = cupNo, CupCTR = (object)null, LidCTR = (object)null }).ToArray();
            }

            foreach (var cupData in cupDataList)
            {
                int cupNo = cupData.CupNum;
                var cupCTR = cupData.CupCTR;
                var lidCTR = cupData.LidCTR;
                var ctCup = cupData.CupObj;
                _ = Task.Run(() => { CupTempRecorder.Get(cupNo).RecordStep("取小样"); });
                var result1 = await OpenLidAsync(cupNo, cupCTR, lidCTR, ctCup);
                if (result1.Code != My_Tool.Result.ResultCode.Success)
                {
                    result = result1;
                }
            }
            return result;
        }

        /// <summary>
        /// 提交取小样任务到任务中心（带预处理）
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> EnqueueSampleAsync(int cupNo, int dyeType)
        {
            var result = new TaskResult();

            // 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 只对正在使用、关盖、当前步未完成的杯进行开盖取样
            var needOpenCups = new List<int>();
            foreach (var info in cupInfos)
            {
                if (info.IsUsing == 1 && info.CoverStatus == 1 && info.CurrentStepFinish == 0)
                {
                    needOpenCups.Add(info.CupNum);
                }
                else if (info.IsUsing == 1 && (info.CoverStatus == null || info.CoverStatus == 0))
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"{info.CupNum}杯盖状态未知";
                    result.Exception = new Exception($"{info.CupNum}杯盖状态未知");
                    return result;
                }
                else if (info.IsUsing == 1 && info.CoverStatus == 2 && info.CurrentStepFinish == 0)
                {
                    _ = Task.Run(() =>
                    {
                        CupTempRecorder.Get(info.CupNum).RecordStep("取小样");
                    });
                }
            }

            // 如果主副杯都为开盖或未使用或已完成，直接返回
            if (needOpenCups.Count == 0)
            {
                var nm = My_Tool.CupAuxiliary.GetIsUseing(cupNo);
                MessageEventManager.Instance.RequestLoopSpeak($"{nm}号配液杯测PH", $"{nm}号杯请测PH");
                // 显示确定键
                var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                if (area != null)
                {
                    var comm = CupCommManager.Instance.GetCommObject(area);
                    if (comm != null)
                    {
                        await comm.SendShowSure(cupNo, 5);
                    }
                }
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "主副杯均为开盖或未使用或当前步已完成，无需开盖";
                return result;
            }

            var task = new SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>
            {
                Priority = dyeType == 1 ?
                    BigProcess.DyeingProcess * 100 +
                    SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.SampleProcess :
                    BigProcess.PostProcess * 100 +
                    SmartColor.My_ConPar.Order.PostProcess.PostProcess.SampleProcess,
                OriginalPriority = dyeType == 1 ?
                    BigProcess.DyeingProcess * 100 +
                    SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.SampleProcess :
                    BigProcess.PostProcess * 100 +
                    SmartColor.My_ConPar.Order.PostProcess.PostProcess.SampleProcess,
                BusinessType = dyeType == 1 ? SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DyeingProcess :
                    SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.PostProcess,
                TaskName = $"取小样-{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}",
                PrecomputeAsync = () => PrecomputeSampleAsync(cupNo)
            };
            task.BusinessFlow = async () =>
            {
                var precomputed = task.PrecomputeResult;
                return await SampleAsync(needOpenCups, precomputed);
            };

            try
            {
                result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);

                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯取小样异常", result.Message, async btn =>
                            {
                                if (btn == "确认")
                                {

                                    var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                    foreach (var c in needOpenCups)
                                    {

                                        int coverStatus = 0;
                                        if (mainInfo.CupNum == c)
                                        {
                                            coverStatus = mainInfo.CoverStatus ?? 0;
                                        }
                                        else
                                        {
                                            coverStatus = subInfo.CoverStatus ?? 0;
                                        }

                                        if (coverStatus == 2) continue;
                                        SqlServer.Update(
                                        CUP_DETAILS.TableName,
                                        new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 2 } },
                                        $"{CUP_DETAILS.CupNum} = @CupNo",
                                        new SqlParameter("@CupNo", c)
                                    );

                                        var area = CupCommManager.Instance.FindCupAreaByCupNum(c);
                                        if (area != null)
                                        {
                                            var comm = CupCommManager.Instance.GetCommObject(area);
                                            if (comm != null)
                                            {
                                                await comm.SyncCoverStatus(c);
                                            }
                                            area.OnCupDataReceived(c);
                                        }
                                    }

                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯取小样询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueSampleAsync(cupNo, dyeType);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new CupRobotTask.TaskResult
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

                            var nm = My_Tool.CupAuxiliary.GetIsUseing(cupNo);
                            MessageEventManager.Instance.RequestLoopSpeak($"{nm}号配液杯取小样", $"{nm}号杯请取小样");

                            // 显示确定键
                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                            if (area != null)
                            {
                                var comm = CupCommManager.Instance.GetCommObject(area);
                                if (comm != null)
                                {
                                    await comm.SendShowSure(cupNo, 5);
                                }
                            }
                            return result;
                        }
                }
            }
            catch (TaskCanceledException)
            {
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                Logger.Error("EnqueueSampleAsync：发生异常。", ex);
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }
        #endregion

        #region 10. 测PH任务
        /// <summary>
        /// 测PH任务预处理：提前查找所有需要测PH的杯的坐标和状态
        /// </summary>
        private static async Task<object> PrecomputePHAsync(int cupNo)
        {
            // 1. 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 2. 过滤需要测PH的杯（正在使用、关盖、当前步未完成）
            var needPHCups = cupInfos
                .Where(info => info.IsUsing == 1 && info.CoverStatus == 1 && info.CurrentStepFinish == 0)
                .ToList();

            // 3. 并发准备每个杯子的坐标和状态
            var cupTasks = needPHCups.Select(async info =>
            {
                var cupObjTask = CupCommManager.Instance.FindCupByCupNumAsync(info.CupNum);
                var cupCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, false);
                var lidCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(info.CupNum, true);
                int cno = info.CupNum;

                var cupCTR = await cupCTTask;
                var lidCTR = await lidCTTask;
                var cupObj = await cupObjTask;
                return new
                {
                    Info = info,
                    CupNum = cno,

                    CupCTR = cupCTR,
                    LidCTR = lidCTR,
                    CupObj = cupObj
                };
            }).ToArray();

            var cupDataList = await Task.WhenAll(cupTasks);
            return cupDataList;
        }

        /// <summary>
        /// 测PH任务（支持预处理结果）
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> PHAsync(List<int> cupNos, object precomputed = null)
        {
            var result = new TaskResult()
            {
                Code = My_Tool.Result.ResultCode.Success,
                Message = "测PH开盖完成"
            };

            dynamic[] cupDataList;
            if (precomputed is Array arr)
            {
                cupDataList = arr.Cast<dynamic>().ToArray();
            }
            else
            {
                // 兼容未预处理的情况
                cupDataList = cupNos.Select(cupNo => new { CupNum = cupNo, CupCTR = (object)null, LidCTR = (object)null }).ToArray();
            }

            foreach (var cupData in cupDataList)
            {
                int cupNo = cupData.CupNum;
                var cupCTR = cupData.CupCTR;
                var lidCTR = cupData.LidCTR;
                var ctCup = cupData.CupObj;
                _ = Task.Run(() => { CupTempRecorder.Get(cupNo).RecordStep("测PH"); });
                var result1 = await OpenLidAsync(cupNo, cupCTR, lidCTR, ctCup);
                if (result1.Code != My_Tool.Result.ResultCode.Success)
                {
                    result = result1;
                }
            }
            return result;
        }

        /// <summary>
        /// 提交测PH任务到任务中心（带预处理）
        /// </summary>
        public static async Task<CupRobotTask.TaskResult> EnqueuePHAsync(int cupNo, int dyeType)
        {
            var result = new TaskResult();

            // 获取主副杯所有字段
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 只对正在使用、关盖、当前步未完成的杯进行开盖测PH
            var needOpenCups = new List<int>();
            foreach (var info in cupInfos)
            {
                if (info.IsUsing == 1 && info.CoverStatus == 1 && info.CurrentStepFinish == 0)
                {
                    needOpenCups.Add(info.CupNum);
                }
                else if (info.IsUsing == 1 && (info.CoverStatus == null || info.CoverStatus == 0))
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"{info.CupNum}杯盖状态未知";
                    result.Exception = new Exception($"{info.CupNum}杯盖状态未知");
                    return result;
                }
                else if (info.IsUsing == 1 && info.CoverStatus == 2 && info.CurrentStepFinish == 0)
                {
                    _ = Task.Run(() =>
                    {
                        CupTempRecorder.Get(info.CupNum).RecordStep("测PH");
                    });
                }
            }

            // 如果主副杯都为开盖或未使用或已完成，直接返回
            if (needOpenCups.Count == 0)
            {

                var nm = My_Tool.CupAuxiliary.GetIsUseing(cupNo);
                MessageEventManager.Instance.RequestLoopSpeak($"{nm}号配液杯测PH", $"{nm}号杯请测PH");
                // 显示确定键
                var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                if (area != null)
                {
                    var comm = CupCommManager.Instance.GetCommObject(area);
                    if (comm != null)
                    {
                        await comm.SendShowSure(cupNo, 5);
                    }
                }
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "主副杯均为开盖或未使用或当前步已完成，无需开盖";
                return result;
            }

            var task = new SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>
            {
                Priority = dyeType == 1 ?
                    BigProcess.DyeingProcess * 100 +
                    SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.PHProcess :
                    BigProcess.PostProcess * 100 +
                    SmartColor.My_ConPar.Order.PostProcess.PostProcess.PHProcess,
                OriginalPriority = dyeType == 1 ?
                    BigProcess.DyeingProcess * 100 +
                    SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess.PHProcess :
                    BigProcess.PostProcess * 100 +
                    SmartColor.My_ConPar.Order.PostProcess.PostProcess.PHProcess,
                BusinessType = dyeType == 1 ? SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.DyeingProcess :
                    SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType.PostProcess,
                TaskName = $"测PH-{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}",
                PrecomputeAsync = () => PrecomputePHAsync(cupNo)
            };
            task.BusinessFlow = async () =>
            {
                var precomputed = task.PrecomputeResult;
                return await PHAsync(needOpenCups, precomputed);
            };

            try
            {
                result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯测PH异常", result.Message, async btn =>
                            {
                                if (btn == "确认")
                                {

                                    var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                    foreach (var c in needOpenCups)
                                    {

                                        int coverStatus = 0;
                                        if (mainInfo.CupNum == c)
                                        {
                                            coverStatus = mainInfo.CoverStatus ?? 0;
                                        }
                                        else
                                        {
                                            coverStatus = subInfo.CoverStatus ?? 0;
                                        }

                                        if (coverStatus == 2) continue;
                                        SqlServer.Update(
                                        CUP_DETAILS.TableName,
                                        new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 2 } },
                                        $"{CUP_DETAILS.CupNum} = @CupNo",
                                        new SqlParameter("@CupNo", c)
                                    );

                                        var area = CupCommManager.Instance.FindCupAreaByCupNum(c);
                                        if (area != null)
                                        {
                                            var comm = CupCommManager.Instance.GetCommObject(area);
                                            if (comm != null)
                                            {
                                                await comm.SyncCoverStatus(c);
                                            }
                                            area.OnCupDataReceived(c);
                                        }
                                    }

                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯测PH询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueuePHAsync(cupNo, dyeType);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                            var nm = My_Tool.CupAuxiliary.GetIsUseing(cupNo);
                            MessageEventManager.Instance.RequestLoopSpeak($"{nm}号配液杯测PH", $"{nm}号杯请测PH");

                            // 显示确定键
                            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                            if (area != null)
                            {
                                var comm = CupCommManager.Instance.GetCommObject(area);
                                if (comm != null)
                                {
                                    await comm.SendShowSure(cupNo, 5);
                                }
                            }
                            return result;
                        }
                }
            }
            catch (TaskCanceledException)
            {
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                Logger.Error("EnqueuePHAsync：发生异常。", ex);
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }
        #endregion

        #region 11. 开盖任务
        /// <summary>
        /// 单杯开盖任务（原子业务流程）
        /// 优化：并发获取坐标，机械手操作串行，功能不变
        /// </summary>
        public static async Task<TaskResult> OpenLidAsync(int cupNo, object cupCTR = null, object lidCTR = null, object cupObj = null)
        {
            var result = new TaskResult();

            var cupTask = CupCommManager.Instance.FindCupByCupNumAsync(cupNo);
            // 优先用预处理坐标
            dynamic lidCT = lidCTR;
            dynamic cupCT = cupCTR;
            dynamic cup = cupObj;
            if (cup is null)
                cup = await CupCommManager.Instance.FindCupByCupNumAsync(cupNo);
            if (lidCT is null)
                lidCT = await My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cupNo, true);
            if (cupCT is null)
                cupCT = await My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cupNo, false);

            var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var coverStatus = 0;
            if (mainInfo.CupNum == cupNo)
            {
                coverStatus = mainInfo.CoverStatus ?? 0;
            }
            else
            {
                coverStatus = subInfo.CoverStatus ?? 0;
            }

            var currentTask = SmartColor.My_RobotManager.RobotTaskManager.Instance.CurrentTask as SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>;
            try
            {
                if (!cupCT.Item1)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"获取{cupNo}号配液杯位置异常";
                    result.Exception = new Exception($"获取{cupNo}号配液杯位置异常");
                    return result;
                }


                if (cup == null)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Exception = new Exception($"未找到{cupNo}号杯杯控件");
                    result.Message = $"未找到{cupNo}号杯杯控件";
                    return result;
                }

                if (coverStatus == 2)
                {
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "开盖完成";
                    return result;
                }

                ////调用动作检查防止装针
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



                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var moveCup = await SemiAutoHelperFactory.Current.MoveToCupAsync(cupNo, 0, cupCT.Item2, cupCT.Item3, 2, 0);
                var moveCupResult = moveCup;
                switch (moveCupResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = moveCupResult.Message;
                        result.Exception = moveCupResult.Exception;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = moveCupResult.Message;
                        return result;
                    default:
                        break;
                }

                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                while (true)
                {
                    if (cup.LockStatus == 1)
                    {
                        break;
                    }
                    Thread.Sleep(1);
                }

                var openLid = await SemiAutoHelperFactory.Current.OpenLidAsync(Convert.ToInt16(cup.LockStatus == 0 ? 1 : 0));
                var openLidResult = openLid;
                switch (openLidResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = openLidResult.Message;
                        result.Exception = openLidResult.Exception;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = openLidResult.Message;
                        return result;
                    default:
                        break;
                }

                // 更新数据库
                SqlServer.Update(
                    CUP_DETAILS.TableName,
                    new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 2 } },
                    $"{CUP_DETAILS.CupNum} = @CupNo",
                    new SqlParameter("@CupNo", cupNo)
                );

                // 通知HMI
                var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                if (area != null)
                {
                    var comm = CupCommManager.Instance.GetCommObject(area);
                    if (comm != null)
                    {
                        await comm.SyncCoverStatus(cupNo);
                    }
                    area.OnCupDataReceived(cupNo);
                }



                // 移动到杯盖
                if (!lidCT.Item1)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"获取{cupNo}号配液杯盖位置异常";
                    result.Exception = new Exception($"获取{cupNo}号配液杯盖位置异常");
                    return result;
                }
                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var moveLid = await SemiAutoHelperFactory.Current.MoveToCupLidAsync(cupNo, lidCT.Item2, lidCT.Item3, 1);
                var moveLidResult = moveLid;
                switch (moveLidResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Message = moveLidResult.Message;
                        result.Exception = moveLidResult.Exception;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                        result.Message = moveLidResult.Message;
                        return result;
                    default:
                        break;
                }

                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var putLid = await SemiAutoHelperFactory.Current.PutLidAsync();
                var putLidResult = putLid;
                switch (putLidResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Exception = putLidResult.Exception;
                        result.Message = putLidResult.Message;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = Result.ResultCode.NeedInteraction;
                        result.Message = putLidResult.Message;
                        return result;
                    default:
                        break;
                }

                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "开盖完成";
            }
            catch (Exception ex)
            {
                Logger.Error("OpenLidAsync：发生异常。", ex);
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = "开盖异常：" + ex.Message;
                result.Exception = ex;
            }
            return result;
        }

        /// <summary>
        /// 提交开盖任务到任务中心（自动查找优先级和业务类型）
        /// </summary>
        public static async Task<TaskResult> EnqueueOpenLidAsync(int cupNo)
        {
            TaskResult result = new TaskResult();

            // 查找杯资料
            var ctCup = await CupCommManager.Instance.FindCupByCupNumAsync(cupNo);
            if (ctCup == null)
            {
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = $"未找到{cupNo}号配液杯";
                result.Exception = new Exception($"未找到{cupNo}号配液杯");
                return result;
            }
            var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);

            int coverStatus = 0;
            if (mainInfo.CupNum == cupNo)
            {
                coverStatus = mainInfo.CoverStatus ?? 0;
            }
            else
            {
                coverStatus = subInfo.CoverStatus ?? 0;
            }

            if (coverStatus == 2)
            {
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "开盖完成";
                return result;
            }

            int priority = int.MaxValue;
            var robotBusinessType = BigProcess.RobotBusinessType.Debug;

            bool isDrop = ctCup.Status == "滴液";
            if (isDrop)
            {
                priority = BigProcess.DropProcess * 100;
                robotBusinessType = BigProcess.RobotBusinessType.DropProcess;
            }


            var task = new RobotBusinessTask<TaskResult>
            {
                Priority = priority,
                OriginalPriority = priority,
                BusinessType = robotBusinessType,
                TaskName = $"开盖-{cupNo}",
                BusinessFlow = async () => await OpenLidAsync(cupNo)
            };
            try
            {
                result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{cupNo}号配液杯开盖异常", result.Message, async btn =>
                            {
                                if (btn == "确认")
                                {

                                    (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);


                                    coverStatus = 0;
                                    if (mainInfo.CupNum == cupNo)
                                    {
                                        coverStatus = mainInfo.CoverStatus ?? 0;
                                    }
                                    else
                                    {
                                        coverStatus = subInfo.CoverStatus ?? 0;
                                    }

                                    if (coverStatus != 2)
                                    {
                                        SqlServer.Update(
                                            CUP_DETAILS.TableName,
                                            new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 2 } },
                                            $"{CUP_DETAILS.CupNum} = @CupNo",
                                            new SqlParameter("@CupNo", cupNo)
                                        );


                                        var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                                        if (area != null)
                                        {
                                            var comm = CupCommManager.Instance.GetCommObject(area);
                                            if (comm != null)
                                            {
                                                await comm.SyncCoverStatus(cupNo);
                                            }
                                            area.OnCupDataReceived(cupNo);
                                        }
                                    }

                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{cupNo}号配液杯开盖询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueOpenLidAsync(cupNo);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                Logger.Error("EnqueueOpenLidAsync：发生异常。", ex);
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }
        #endregion

        #region 12. 关盖任务
        /// <summary>
        /// 单杯关盖任务（原子业务流程）
        /// 优化：并发获取坐标，机械手操作串行，功能不变
        /// </summary>
        public static async Task<TaskResult> CloseLidAsync(int cupNo, object cupCTR = null, object lidCTR = null, object cupObj = null)
        {
            var result = new TaskResult();


            // 优先用预处理坐标
            dynamic lidCT = lidCTR;
            dynamic cupCT = cupCTR;
            dynamic cup = cupObj;
            if (cup is null)
                cup = await CupCommManager.Instance.FindCupByCupNumAsync(cupNo);
            if (lidCT is null)
                lidCT = await My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cupNo, true);
            if (cupCT is null)
                cupCT = await My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cupNo, false);



            var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var coverStatus = 0;
            if (mainInfo.CupNum == cupNo)
            {
                coverStatus = mainInfo.CoverStatus ?? 0;
            }
            else
            {
                coverStatus = subInfo.CoverStatus ?? 0;
            }

            var currentTask = SmartColor.My_RobotManager.RobotTaskManager.Instance.CurrentTask as SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>;
            try
            {
                if (!lidCT.Item1)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"获取{cupNo}号配液杯盖位置异常";
                    result.Exception = new Exception($"获取{cupNo}号配液杯盖位置异常");
                    return result;
                }


                if (cup == null)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Exception = new Exception($"未找到{cupNo}号杯杯控件");
                    result.Message = $"未找到{cupNo}号杯杯控件";
                    return result;
                }

                if (coverStatus == 1)
                {
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = "关盖完成";
                    return result;
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

                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var moveLid = await SemiAutoHelperFactory.Current.MoveToCupLidAsync(cupNo, lidCT.Item2, lidCT.Item3, 0);
                var moveLidResult = moveLid;
                switch (moveLidResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Exception = moveLidResult.Exception;
                        result.Message = moveLidResult.Message;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = Result.ResultCode.NeedInteraction;
                        result.Message = moveLidResult.Message;
                        return result;
                    default:
                        break;
                }

                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();
                var takeLid = await SemiAutoHelperFactory.Current.TakeLidAsync();
                var takeLidResult = takeLid;
                switch (takeLidResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Exception = takeLidResult.Exception;
                        result.Message = takeLidResult.Message;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = Result.ResultCode.NeedInteraction;
                        result.Message = takeLidResult.Message;
                        return result;
                    default:
                        break;
                }

                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                if (!cupCT.Item1)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"获取{cupNo}号配液杯位置异常";
                    result.Exception = new Exception($"获取{cupNo}号配液杯位置异常");
                    return result;
                }

                var moveCup = await SemiAutoHelperFactory.Current.MoveToCupAsync(cupNo, 0, cupCT.Item2, cupCT.Item3, 2, 1);
                var moveCupResult = moveCup;
                switch (moveCupResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Exception = moveCupResult.Exception;
                        result.Message = moveCupResult.Message;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = Result.ResultCode.NeedInteraction;
                        result.Message = moveCupResult.Message;
                        return result;
                    default:
                        break;
                }

                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                while (true)
                {
                    if (cup.LockStatus == 1)
                    {
                        break;
                    }
                    Thread.Sleep(1);
                }

                var closeLid = await SemiAutoHelperFactory.Current.CloseLidAsync(Convert.ToInt16(cup.LockStatus == 0 ? 1 : 0));
                var closeLidResult = closeLid;
                switch (closeLidResult.Level)
                {
                    case SemiAutoResultCode.Exception:
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.Exception = closeLidResult.Exception;
                        result.Message = closeLidResult.Message;
                        return result;
                    case SemiAutoResultCode.NeedInteraction:
                        result.Code = Result.ResultCode.NeedInteraction;
                        result.Message = closeLidResult.Message;
                        return result;
                    default:
                        break;
                }

                // 6. 更新数据库
                SqlServer.Update(
                    CUP_DETAILS.TableName,
                    new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 1 } },
                    $"{CUP_DETAILS.CupNum} = @CupNo",
                    new SqlParameter("@CupNo", cupNo)
                );

                // 7. 通知HMI
                var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                if (area != null)
                {
                    var comm = CupCommManager.Instance.GetCommObject(area);
                    if (comm != null)
                    {
                        await comm.SyncCoverStatus(cupNo);
                    }
                    area.OnCupDataReceived(cupNo);
                }



                // 8. PLC坐标写入（原逻辑不变）
                if (My_ConPar.Choices.UseAutoUpdateCupCoor == 1)
                {
                    var plc = SmartColor.My_ConPar.Object.CurrentPLC as SmartColor.My_PLC.PLC;
                    if (plc != null)
                    {
                        plc.EnqueueRead(909, 4, regs =>
                        {
                            int x = (regs[1] << 16) | regs[0];
                            int y = (regs[3] << 16) | regs[2];
                            if (My_ConPar.Hardware.Tongs_Decompression == 1)
                            {
                                y += My_ConPar.Other.SupportCoverY;
                            }
                            Task.Run(() =>
                            {
                                var layoutType = SmartColor.My_ConPar.Object.CurrentLayout as Type;
                                if (layoutType != null)
                                {
                                    var areaProps = layoutType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                                    foreach (var prop in areaProps)
                                    {
                                        var areaObj = prop.GetValue(null) as SmartColor.My_ConPar.Area.Base;
                                        if (areaObj == null) continue;
                                        if (!(areaObj is SmartColor.My_ConPar.Area.Drop.Drop
                                            || areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_4
                                            || areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_6
                                            || areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_12
                                            || areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_16
                                            || areaObj is SmartColor.My_ConPar.Area.RotorCylinder.RC_4
                                            || areaObj is SmartColor.My_ConPar.Area.RotorCylinder.RC_10))
                                            continue;

                                        int start = GetIntProperty(areaObj, "StartPosition");
                                        int row = GetIntProperty(areaObj, "Row");
                                        int col = GetIntProperty(areaObj, "Column");
                                        if (row > 0 && col > 0)
                                        {
                                            int end = start + row * col - 1;
                                            if (cupNo >= start && cupNo <= end)
                                            {
                                                SetIntProperty(areaObj, $"CupCX_{cupNo - start + 1}", x);
                                                SetIntProperty(areaObj, $"CupCY_{cupNo - start + 1}", y);

                                                string section = prop.Name.Replace("_", "");
                                                string iniPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Config", "Layout.ini");
                                                SmartColor.My_File.ConfigHelper.WriteIniValue(section, $"CupCX_{cupNo - start + 1}", x.ToString(), iniPath);
                                                SmartColor.My_File.ConfigHelper.WriteIniValue(section, $"CupCY_{cupNo - start + 1}", y.ToString(), iniPath);
                                                AreaCoordinateFinder.UpdateCupCoordinateCache(cupNo, x, y);
                                                break;
                                            }
                                        }
                                    }
                                }
                            });
                        });
                    }
                }
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "关盖完成";
            }
            catch (Exception ex)
            {
                Logger.Error("CloseLidAsync：发生异常。", ex);
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = "关盖异常：" + ex.Message;
                result.Exception = ex;
            }
            return result;
        }

        private static int GetIntProperty(object obj, string propName)
        {
            var prop = obj.GetType().GetProperty(propName);
            if (prop != null && prop.PropertyType == typeof(int))
                return (int)prop.GetValue(obj);
            return 0;
        }

        // 反射设置int属性
        private static void SetIntProperty(object obj, string propName, int value)
        {
            var prop = obj.GetType().GetProperty(propName);
            if (prop != null && prop.PropertyType == typeof(int) && prop.CanWrite)
                prop.SetValue(obj, value);
        }

        /// <summary>
        /// 提交关盖任务到任务中心（自动查找优先级和业务类型）
        /// </summary>
        public static async Task<TaskResult> EnqueueCloseLidAsync(int cupNo)
        {
            TaskResult result = new TaskResult();

            var cup = await CupCommManager.Instance.FindCupByCupNumAsync(cupNo);
            if (cup == null)
            {
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Exception = new Exception($"未找到{cupNo}号杯杯控件");
                result.Message = $"未找到{cupNo}号杯杯控件";
                return result;
            }
            var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);

            int coverStatus = 0;
            if (mainInfo.CupNum == cupNo)
            {
                coverStatus = mainInfo.CoverStatus ?? 0;
            }
            else
            {
                coverStatus = subInfo.CoverStatus ?? 0;
            }

            if (coverStatus == 1)
            {
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "关盖完成";
                return result;
            }


            int priority = int.MaxValue;
            var robotBusinessType = BigProcess.RobotBusinessType.Debug;
            var task = new RobotBusinessTask<TaskResult>
            {
                Priority = priority,
                OriginalPriority = priority,
                BusinessType = robotBusinessType,
                TaskName = $"关盖-{cupNo}",
                BusinessFlow = async () => await CloseLidAsync(cupNo)
            };
            try
            {
                result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{cupNo}号配液杯关盖异常", result.Message, async btn =>
                            {
                                if (btn == "确认")
                                {
                                    (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);

                                    coverStatus = 0;
                                    if (mainInfo.CupNum == cupNo)
                                    {
                                        coverStatus = mainInfo.CoverStatus ?? 0;
                                    }
                                    else
                                    {
                                        coverStatus = subInfo.CoverStatus ?? 0;
                                    }

                                    if (coverStatus != 1)
                                    {
                                        SqlServer.Update(
                                            CUP_DETAILS.TableName,
                                            new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 1 } },
                                            $"{CUP_DETAILS.CupNum} = @CupNo",
                                            new SqlParameter("@CupNo", cupNo)
                                        );


                                        var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                                        if (area != null)
                                        {
                                            var comm = CupCommManager.Instance.GetCommObject(area);
                                            if (comm != null)
                                            {
                                                await comm.SyncCoverStatus(cupNo);
                                            }
                                            area.OnCupDataReceived(cupNo);
                                        }
                                    }
                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{cupNo}号配液杯关盖询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    if (result.Message.Contains("关盖时撑盖开失败"))
                                    {
                                        var retryResult = await EnqueueSupportCoverAsync(cupNo, task.Priority, robotBusinessType);
                                        if (retryResult.Code == Result.ResultCode.Success)
                                        {
                                            retryResult = await EnqueueCloseLidAsync(cupNo);
                                            tcs.SetResult(retryResult);
                                        }
                                        else
                                        {
                                            tcs.SetResult(retryResult);
                                        }
                                    }
                                    else
                                    {
                                        var retryResult = await EnqueueCloseLidAsync(cupNo);
                                        tcs.SetResult(retryResult);
                                    }
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                Logger.Error("EnqueueCloseLidAsync：发生异常。", ex);
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }
        #endregion


        #region 撑盖

        /// <summary>
        /// 撑盖任务预处理：提前查找杯坐标
        /// </summary>
        private static async Task<object> PrecomputeSupportCoverAsync(int cupNo)
        {
            // 查找主副杯
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 只处理正在使用的杯
            var needCups = cupInfos.Where(info => info.IsUsing == 1).ToList();

            var cupTasks = needCups.Select(async info =>
            {
                int cno = info.CupNum;
                var cupObjTask = CupCommManager.Instance.FindCupByCupNumAsync(cno);
                var cupCTTask = My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cno, false);
                var cupCTR = await cupCTTask;

                var cupObj = await cupObjTask;
                return new
                {
                    CupNum = cno,
                    CupCTR = cupCTR,
                    CupObj = cupObj

                };
            }).ToArray();

            var cupDataList = await Task.WhenAll(cupTasks);
            return cupDataList;
        }

        /// <summary>
        /// 单杯撑盖任务（原子业务流程）
        /// </summary>
        public static async Task<TaskResult> SupportCoverAsync(List<int> cupNos, object precomputed = null)
        {
            var result = new TaskResult()
            {
                Code = My_Tool.Result.ResultCode.Success,
                Message = "撑盖完成"
            };

            var currentTask = SmartColor.My_RobotManager.RobotTaskManager.Instance.CurrentTask as SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>;

            dynamic[] cupDataList;
            if (precomputed is Array arr)
            {
                cupDataList = arr.Cast<dynamic>().ToArray();
            }
            else
            {
                // 兼容未预处理的情况
                cupDataList = cupNos.Select(cupNo => new { CupNum = cupNo, CupCTR = (object)null }).ToArray();
            }

            foreach (var cupData in cupDataList)
            {
                int cupNo = cupData.CupNum;
                dynamic cupCTR = cupData.CupCTR;
                if (cupCTR is null)
                    cupCTR = await My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(cupNo, false);
                dynamic cup = cupData.CupObj;
                if (cup is null)
                    cup = await CupCommManager.Instance.FindCupByCupNumAsync(cupNo);

                //有盖则证明已经关好了，不需要撑盖
                if (cup.CoverStatus == 1) continue;
                if (!cupCTR.Item1)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = $"未找到{cupNo}号杯坐标";
                    result.Exception = new Exception(result.Message);
                    return result;
                }

                int x = cupCTR.Item2;
                int y = cupCTR.Item3 - SmartColor.My_ConPar.Other.SupportCoverY;

                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                // 移动到撑盖位
                var moveResult = await SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.MoveToCupAsync(
                    cupNo, 0, x, y, 2, 0);
                if (moveResult.Level == SmartColor.My_SemiAutoModule.SemiAutoResultCode.Exception)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = moveResult.Message;
                    result.Exception = moveResult.Exception;
                    return result;
                }
                if (moveResult.Level == SmartColor.My_SemiAutoModule.SemiAutoResultCode.NeedInteraction)
                {
                    result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                    result.Message = moveResult.Message;
                    return result;
                }

                if (currentTask != null)
                    await currentTask.CheckPauseOnlyAsync();

                while (true)
                {
                    if (cup.LockStatus == 1)
                    {
                        break;
                    }
                    Thread.Sleep(1);
                }

                // 执行撑盖动作
                var supportResult = await SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.SupportCoverAsync();
                if (supportResult.Level == SmartColor.My_SemiAutoModule.SemiAutoResultCode.Exception)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = supportResult.Message;
                    result.Exception = supportResult.Exception;
                    return result;
                }
                if (supportResult.Level == SmartColor.My_SemiAutoModule.SemiAutoResultCode.NeedInteraction)
                {
                    result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                    result.Message = supportResult.Message;
                    return result;
                }

                _ = Task.Run(async () =>
                {
                    // 6. 更新数据库
                    SqlServer.Update(
                        CUP_DETAILS.TableName,
                        new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 1 } },
                        $"{CUP_DETAILS.CupNum} = @CupNo",
                        new SqlParameter("@CupNo", cupNo)
                    );

                    // 7. 通知HMI
                    var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
                    if (area != null)
                    {
                        var comm = CupCommManager.Instance.GetCommObject(area);
                        if (comm != null)
                        {
                            await comm.SyncCoverStatus(cupNo);
                        }
                        area.OnCupDataReceived(cupNo);
                    }
                });



            }
            return result;
        }

        /// <summary>
        /// 提交撑盖任务到任务中心
        /// </summary>
        /// <param name="cupNo">杯号</param>
        /// <param name="priority">优先级</param>
        /// <param name="businessType">业务类型</param>
        /// <returns>任务结果</returns>
        public static async Task<TaskResult> EnqueueSupportCoverAsync(int cupNo, int priority, SmartColor.My_ConPar.Order.BigProcess.RobotBusinessType businessType)
        {
            var result = new TaskResult();

            // 查找主副杯
            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
            var cupInfos = new List<My_Tool.CupAuxiliary.CupDetailInfo> { mainCup };
            if (subCup.CupNum != mainCup.CupNum) cupInfos.Add(subCup);

            // 只处理正在使用的杯
            var needCups = cupInfos.Where(info => info.IsUsing == 1).Select(info => info.CupNum).ToList();
            if (needCups.Count == 0)
            {
                result.Code = My_Tool.Result.ResultCode.Success;
                result.Message = "主副杯均未使用，无需撑盖";
                return result;
            }

            var task = new SmartColor.My_RobotManager.RobotBusinessTask<TaskResult>
            {
                Priority = priority,
                OriginalPriority = priority,
                BusinessType = businessType,
                TaskName = $"撑盖-{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}",
                PrecomputeAsync = () => PrecomputeSupportCoverAsync(cupNo)
            };
            task.BusinessFlow = async () =>
            {
                var precomputed = task.PrecomputeResult;
                return await SupportCoverAsync(needCups, precomputed);
            };

            try
            {
                result = await SmartColor.My_RobotManager.RobotTaskManager.Instance.EnqueueTask(task);
                switch (result.Code)
                {
                    case My_Tool.Result.ResultCode.Exception:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯撑盖异常", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueSupportCoverAsync(cupNo, priority, businessType);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "退出")
                                {
                                    tcs.SetResult(new CupRobotTask.TaskResult
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
                    case My_Tool.Result.ResultCode.NeedInteraction:
                        {
                            var tcs = new TaskCompletionSource<CupRobotTask.TaskResult>();
                            My_Tool.MessageEventManager.Instance.RequestShowMessage($"{My_Tool.CupAuxiliary.GetIsUseing(cupNo)}号配液杯撑盖询问", result.Message, async btn =>
                            {
                                if (btn == "重试")
                                {
                                    var retryResult = await EnqueueSupportCoverAsync(cupNo, priority, businessType);
                                    tcs.SetResult(retryResult);
                                }
                                else if (btn == "已确认")
                                {


                                    await Task.Run(async () =>
                                     {
                                         List<int> ints = new List<int>();
                                         if (mainCup.CupNum != subCup.CupNum)
                                         {
                                             if (mainCup.IsUsing == 1)
                                             {
                                                 ints.Add(mainCup.CupNum);
                                             }
                                             if (subCup.IsUsing == 1)
                                             {
                                                 ints.Add(subCup.CupNum);
                                             }
                                         }
                                         else
                                         {
                                             if (mainCup.IsUsing == 1)
                                             {
                                                 ints.Add(mainCup.CupNum);
                                             }
                                         }
                                         foreach (var c in ints)
                                         {

                                             // 6. 更新数据库
                                             SqlServer.Update(
                                              CUP_DETAILS.TableName,
                                              new Dictionary<string, object> { { CUP_DETAILS.CoverStatus, 1 } },
                                              $"{CUP_DETAILS.CupNum} = @CupNo",
                                              new SqlParameter("@CupNo", c)
                                          );

                                             // 7. 通知HMI
                                             var area = CupCommManager.Instance.FindCupAreaByCupNum(c);
                                             if (area != null)
                                             {
                                                 var comm = CupCommManager.Instance.GetCommObject(area);
                                                 if (comm != null)
                                                 {
                                                     await comm.SyncCoverStatus(c);
                                                 }
                                                 area.OnCupDataReceived(c);
                                             }
                                         }
                                     });

                                    tcs.SetResult(new CupRobotTask.TaskResult
                                    {
                                        Code = My_Tool.Result.ResultCode.Success,
                                        Message = "用户已确认关盖正常，任务已完成"
                                    });
                                }
                            },
                            new[] { "重试", "已确认" },
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
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Canceled,
                    Message = "任务被取消"
                };
            }
            catch (Exception ex)
            {
                Logger.Error("EnqueueSupportCoverAsync：发生异常。", ex);
                return new TaskResult
                {
                    Code = My_Tool.Result.ResultCode.Exception,
                    Message = "代码异常：" + ex.Message,
                    Exception = ex
                };
            }
        }
        #endregion

        public static bool FixDyeTypeForPreWash(int cupNo)
        {
            var (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupNo);
            if ((mainInfo.DyeingCode == CupAuxiliary.PreWashCupType) || (subInfo.DyeingCode == CupAuxiliary.PreWashCupType))
                return true;
            return false;

        }


    }
}