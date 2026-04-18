using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SmartColor.My_Form.Help;

namespace SmartColor.My_AutomaticModule
{
    public static class DropBatchManager
    {
        public static event Action BatchChanged;

        private static readonly Queue<string> PendingBatchQueue = new Queue<string>();
        public static string CurrentDroppingBatchNo = null;
        private static bool _hasPendingBatchStart = false;
        private static readonly object _lock = new object();

        /// <summary>
        /// 外部调用：请求批量启动滴液
        /// </summary>
        public static void RequestBatchStart(Func<DataRow, bool> filter = null)
        {
            // 新增：检查软件是否过期或即将到期
            int? trialDaysLeft = Register.GetTrialDaysLeft();
            if (trialDaysLeft.HasValue)
            {
                if (trialDaysLeft.Value <= 0)
                {
                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("软件已过期，请联系供应商注册激活。");
                    return;
                }
                else if (trialDaysLeft.Value < 10)
                {
                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip($"软件试用期剩余{trialDaysLeft.Value}天，请尽快注册激活。");
                }
            }
            lock (_lock)
            {
                if (CurrentDroppingBatchNo != null)
                {
                    _hasPendingBatchStart = true;
                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("有批次正在滴液，待当前批次完成后自动合并启动。");
                    return;
                }
                string batchNo = GenerateBatchNoAndUpdateAll(filter);
                if (!string.IsNullOrEmpty(batchNo))
                {
                    EnqueueBatch(batchNo);
                }
            }
        }


        /// <summary>
        /// 入队并尝试启动
        /// </summary>
        public static void EnqueueBatch(string batchNo)
        {
            lock (_lock)
            {
                if (!PendingBatchQueue.Contains(batchNo))
                    PendingBatchQueue.Enqueue(batchNo);

                if (CurrentDroppingBatchNo == null)
                {
                    TryStartNextDrop();
                }
                else
                {
                    // SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("有批次正在滴液，已自动排队，待前批次完成后自动启动。");
                }
            }
        }

        /// <summary>
        /// 启动下一个批次
        /// </summary>
        public static void TryStartNextDrop()
        {
            lock (_lock)
            {
                if (PendingBatchQueue.Count > 0 && CurrentDroppingBatchNo == null)
                {
                    string nextBatch = PendingBatchQueue.Peek();
                    CurrentDroppingBatchNo = nextBatch;
                    _ = StartDropProcessAsync(nextBatch);
                }
            }
        }

        private static Task StartDropProcessAsync(string batchNo)
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = await SmartColor.My_AutomaticModule.DropRobotTask.EnqueueBatchDropAsync(batchNo);
                    if (result.Code == SmartColor.My_Tool.Result.ResultCode.Success)
                    {
                        _ = Task.Run(() =>
                         {
                             MessageEventManager.Instance.RequestLoopSpeak("滴液完成", $"{batchNo}批次滴液完成");
                             Task.Delay(Convert.ToInt32(My_ConPar.Delay.Buzzer_Finish * 1000)).ContinueWith(_ => MessageEventManager.Instance.RequestStopLoopSpeak("滴液完成"));
                         });

                    }

                    else if (result.Code == SmartColor.My_Tool.Result.ResultCode.Canceled)
                    {
                        _ = Task.Run(() =>
                        {
                            MessageEventManager.Instance.RequestLoopSpeak("批次取消", $"{batchNo}批次已取消");
                            Task.Delay(Convert.ToInt32(My_ConPar.Delay.Buzzer_Finish * 1000)).ContinueWith(_ => MessageEventManager.Instance.RequestStopLoopSpeak("批次取消"));
                        });
                    }

                    else
                    {
                        _ = Task.Run(() =>
                        {
                            SmartColor.My_File.Logger.Error("滴液异常", result.Exception);
                            MessageEventManager.Instance.RequestLoopSpeak("滴液异常", $"{batchNo}滴液异常,详情请看日志");
                            Task.Delay(Convert.ToInt32(My_ConPar.Delay.Buzzer_Finish * 1000)).ContinueWith(_ => MessageEventManager.Instance.RequestStopLoopSpeak("滴液异常"));
                        });

                    }

                }
                catch (Exception ex)
                {
                    SmartColor.My_File.Logger.Error("StartDropProcessAsync异常", ex);
                }
                finally
                {
                    RobotTaskManager_TaskQueueChanged();
                }
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// 滴液队列变更，自动处理批量合并启动
        /// </summary>
        public static void RobotTaskManager_TaskQueueChanged()
        {
            lock (_lock)
            {
                if (CurrentDroppingBatchNo != null)
                {
                    if (PendingBatchQueue.Count > 0 && PendingBatchQueue.Peek() == CurrentDroppingBatchNo)
                        PendingBatchQueue.Dequeue();
                    CurrentDroppingBatchNo = null;
                }
                TryStartNextDrop();

                // 关键：如果有待合并启动的批次，且当前没有批次在滴液，则统一生成批次号并启动
                if (_hasPendingBatchStart && CurrentDroppingBatchNo == null)
                {
                    _hasPendingBatchStart = false;
                    string batchNo = GenerateBatchNoAndUpdateAll();
                    if (!string.IsNullOrEmpty(batchNo))
                    {
                        EnqueueBatch(batchNo);
                    }
                }
            }
            OnBatchChanged(); // 新增：通知UI刷新
        }

        /// <summary>
        /// 统一生成批次号，并批量更新所有未启动的批次
        /// </summary>
        private static string GenerateBatchNoAndUpdateAll(Func<DataRow, bool> filter = null)
        {


            var dt = SmartColor.My_DataBase.DropBatchData.GetHeadData();

            if (dt.Rows.Count == 0) return null;

            var rows = dt.AsEnumerable()
                .Where(r => (r[SmartColor.My_DataBase.DROP_HEAD.BatchName] == DBNull.Value
                          || string.IsNullOrEmpty(r[SmartColor.My_DataBase.DROP_HEAD.BatchName]?.ToString())))
                .ToList();

            // 新增：根据filter筛选
            if (filter != null)
                rows = rows.Where(filter).ToList();

            if (rows.Count == 0) return null;

            string batchNo = SmartColor.My_Tool.BatchNoHelper.GenerateBatchNoAndUpdate();

            foreach (var row in rows)
            {
                int id = Convert.ToInt32(row[SmartColor.My_DataBase.DROP_HEAD.MyID]);
                SmartColor.My_DataBase.SqlServer.Update(
                    SmartColor.My_DataBase.DROP_HEAD.TableName,
                            new Dictionary<string, object>
                            {
                                [SmartColor.My_DataBase.DROP_HEAD.BatchName] = batchNo,
                                [SmartColor.My_DataBase.DROP_HEAD.State] = "已滴定配方"
                            },
                            $"{SmartColor.My_DataBase.DROP_HEAD.MyID} = @MyID",
                            new SqlParameter("@MyID", id)
                        );
                SmartColor.My_DataBase.SqlServer.Update(
                    SmartColor.My_DataBase.DROP_DETAILS.TableName,
                    new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.DROP_DETAILS.BatchName] = batchNo
                    },
                    $"{SmartColor.My_DataBase.DROP_DETAILS.HeadID} = @MyID",
                    new SqlParameter("@MyID", id)
                );
                SmartColor.My_DataBase.SqlServer.Update(
                    SmartColor.My_DataBase.DYE_DETAILS.TableName,
                    new Dictionary<string, object> { [SmartColor.My_DataBase.DYE_DETAILS.BatchName] = batchNo },
                    $"{SmartColor.My_DataBase.DYE_DETAILS.HeadID} = @MyID",
                    new SqlParameter("@MyID", id)
                );
                // 同步更新配方表状态
                string formulaCode = row[SmartColor.My_DataBase.DROP_HEAD.FormulaCode]?.ToString();
                int versionNum = Convert.ToInt32(row[SmartColor.My_DataBase.DROP_HEAD.VersionNum]);
                SmartColor.My_DataBase.SqlServer.Update(
                    SmartColor.My_DataBase.FORMULA_HEAD.TableName,
                    new Dictionary<string, object> { [SmartColor.My_DataBase.FORMULA_HEAD.State] = "已滴定配方" },
                    $"{SmartColor.My_DataBase.FORMULA_HEAD.FormulaCode} = @FormulaCode AND {SmartColor.My_DataBase.FORMULA_HEAD.VersionNum} = @VersionNum AND {SmartColor.My_DataBase.FORMULA_HEAD.State} = N'尚未滴液'",
                    new SqlParameter("@FormulaCode", formulaCode),
                    new SqlParameter("@VersionNum", versionNum)
                );
            }

            return batchNo;
        }


        private static void OnBatchChanged()
        {
            Task.Delay(500).ContinueWith(_ => BatchChanged?.Invoke());
        }


        public static void RequestBatchStartByIds(IEnumerable<int> ids)
        {
            // 新增：检查软件是否过期或即将到期
            int? trialDaysLeft = Register.GetTrialDaysLeft();
            if (trialDaysLeft.HasValue)
            {
                if (trialDaysLeft.Value <= 0)
                {
                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("软件已过期，请联系供应商注册激活。");
                    return;
                }
                else if (trialDaysLeft.Value < 10)
                {
                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip($"软件试用期剩余{trialDaysLeft.Value}天，请尽快注册激活。");
                }
            }
            lock (_lock)
            {
                if (CurrentDroppingBatchNo != null)
                {
                    _hasPendingBatchStart = true;
                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("有批次正在滴液，待当前批次完成后自动合并启动。");
                    return;
                }
                string batchNo = GenerateBatchNoAndUpdateByIds(ids);
                if (!string.IsNullOrEmpty(batchNo))
                {
                    EnqueueBatch(batchNo);
                }
            }
        }

        /// <summary>
        /// 只对指定ID生成批次号并批量更新
        /// </summary>
        private static string GenerateBatchNoAndUpdateByIds(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any()) return null;
            var dt = SmartColor.My_DataBase.DropBatchData.GetHeadData();
            var rows = dt.AsEnumerable()
                .Where(r => ids.Contains(Convert.ToInt32(r[SmartColor.My_DataBase.DROP_HEAD.MyID])))
                .Where(r => r[SmartColor.My_DataBase.DROP_HEAD.BatchName] == DBNull.Value
                         || string.IsNullOrEmpty(r[SmartColor.My_DataBase.DROP_HEAD.BatchName]?.ToString()))
                .ToList();

            if (rows.Count == 0) return null;

            string batchNo = SmartColor.My_Tool.BatchNoHelper.GenerateBatchNoAndUpdate();

            foreach (var row in rows)
            {
                int id = Convert.ToInt32(row[SmartColor.My_DataBase.DROP_HEAD.MyID]);
                SmartColor.My_DataBase.SqlServer.Update(
                    SmartColor.My_DataBase.DROP_HEAD.TableName,
                    new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.DROP_HEAD.BatchName] = batchNo,
                        [SmartColor.My_DataBase.DROP_HEAD.State] = "已滴定配方"
                    },
                    $"{SmartColor.My_DataBase.DROP_HEAD.MyID} = @MyID",
                    new SqlParameter("@MyID", id)
                );
                SmartColor.My_DataBase.SqlServer.Update(
                    SmartColor.My_DataBase.DROP_DETAILS.TableName,
                    new Dictionary<string, object> { [SmartColor.My_DataBase.DROP_DETAILS.BatchName] = batchNo },
                    $"{SmartColor.My_DataBase.DROP_DETAILS.HeadID} = @MyID",
                    new SqlParameter("@MyID", id)
                );
                SmartColor.My_DataBase.SqlServer.Update(
                    SmartColor.My_DataBase.DYE_DETAILS.TableName,
                    new Dictionary<string, object> { [SmartColor.My_DataBase.DYE_DETAILS.BatchName] = batchNo },
                    $"{SmartColor.My_DataBase.DYE_DETAILS.HeadID} = @MyID",
                    new SqlParameter("@MyID", id)
                );
                // 同步更新配方表状态
                string formulaCode = row[SmartColor.My_DataBase.DROP_HEAD.FormulaCode]?.ToString();
                int versionNum = Convert.ToInt32(row[SmartColor.My_DataBase.DROP_HEAD.VersionNum]);
                SmartColor.My_DataBase.SqlServer.Update(
                    SmartColor.My_DataBase.FORMULA_HEAD.TableName,
                    new Dictionary<string, object> { [SmartColor.My_DataBase.FORMULA_HEAD.State] = "已滴定配方" },
                    $"{SmartColor.My_DataBase.FORMULA_HEAD.FormulaCode} = @FormulaCode AND {SmartColor.My_DataBase.FORMULA_HEAD.VersionNum} = @VersionNum AND {SmartColor.My_DataBase.FORMULA_HEAD.State} = N'尚未滴液'",
                    new SqlParameter("@FormulaCode", formulaCode),
                    new SqlParameter("@VersionNum", versionNum)
                );
            }

            return batchNo;
        }

    }
}