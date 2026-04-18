using SmartColor.My_DataBase;
using SmartColor.My_File;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SmartColor.My_Tool
{
    /// <summary>
    /// 运行表管理工具类，负责插入数据及跨天清理等操作
    /// 支持异步插入顺序保证（队列串行化）
    /// </summary>
    internal static class RunTableMan
    {
        /// <summary>
        /// 运行表改变事件（静态），用于通知外部运行表数据变更
        /// </summary>
        public static event EventHandler<RunTableChangedEventArgs> RunTableChanged;

        // 队列与锁，保证插入顺序
        private static readonly Queue<Tuple<DateTime, Dictionary<string, object>>> _pendingInsertQueue = new Queue<Tuple<DateTime, Dictionary<string, object>>>();
        private static readonly object _queueLock = new object();
        private static bool _isProcessing = false;

        /// <summary>
        /// 插入数据到运行表（run_table），自动补充日期和时间，并判断是否跨天清理
        /// </summary>
        /// <param name="data">待插入的数据字典</param>
        public static Task InsertAsync(Dictionary<string, object> data, DateTime timestamp)
        {
            EnqueueInsert(data, timestamp);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 插入单个字段数据到运行表
        /// </summary>
        /// <param name="header">字段名</param>
        /// <param name="value">字段值</param>
        public static Task Insert(string header, object value)
        {
            var dt = DateTime.Now;
            return InsertAsync(new Dictionary<string, object> { [header] = value }, dt);
        }

        /// <summary>
        /// 入队并启动处理线程
        /// </summary>
        private static void EnqueueInsert(Dictionary<string, object> data, DateTime timestamp)
        {
            lock (_queueLock)
            {
                _pendingInsertQueue.Enqueue(Tuple.Create(timestamp, new Dictionary<string, object>(data)));
                if (!_isProcessing)
                {
                    _isProcessing = true;
                    Task.Run(ProcessQueue);
                }
            }
        }

        
        /// <summary>
        /// 串行处理队列，保证插入顺序
        /// </summary>
        private static async Task ProcessQueue()
        {
            while (true)
            {
                Tuple<DateTime, Dictionary<string, object>> item = null;
                lock (_queueLock)
                {
                    if (_pendingInsertQueue.Count == 0)
                    {
                        _isProcessing = false;
                        return;
                    }
                    item = _pendingInsertQueue.Dequeue();
                }

                try
                {
                    var insertData = item.Item2;
                    insertData[My_DataBase.RUN_TABLE.MyDate] = item.Item1.Date;
                    insertData[My_DataBase.RUN_TABLE.MyTime] = item.Item1.ToString("HH:mm:ss");

                    // 判断是否需要清理（跨天）
                    if (NeedClearTable())
                    {
                        await Task.Run(() =>
                        {
                            int deleted = SqlServer.DeleteAll(My_DataBase.RUN_TABLE.TableName);
                            Logger.Info($"RunTableMan: 跨天清理运行表，已删除 {deleted} 条数据。");
                        });
                    }

                    // 只保留合法字段
                    var validFields = TableDefinition.TableSchemas[My_DataBase.RUN_TABLE.TableName];
                    var validNames = new HashSet<string>(validFields.Select(f => f.Name), StringComparer.OrdinalIgnoreCase);

                    var filteredData = insertData
                        .Where(kv => validNames.Contains(kv.Key))
                        .ToDictionary(kv => kv.Key, kv => kv.Value);

                    if (filteredData.Count == 0)
                        throw new ArgumentException("没有有效字段可插入");

                    await Task.Run(() =>
                    {
                        SqlServer.Insert(My_DataBase.RUN_TABLE.TableName, filteredData);
                        Logger.Info($"RunTableMan: 插入运行数据 {string.Join(", ", filteredData.Select(kv => kv.Key + "=" + kv.Value))}");
                    });

                    // 触发运行表改变事件
                    RunTableChanged?.Invoke(null, new RunTableChangedEventArgs("Insert", filteredData));
                }
                catch (Exception ex)
                {
                    Logger.Error("RunTableMan: 插入运行数据异常", ex);
                }
            }
        }

        /// <summary>
        /// 判断运行表是否需要清理（跨天），仅在最后一条数据日期与今天不同时清理
        /// </summary>
        /// <returns>是否需要清理</returns>
        private static bool NeedClearTable()
        {
            try
            {
                var dt = SqlServer.Select(My_DataBase.RUN_TABLE.TableName, null);
                if (dt.Rows.Count == 0) return false;

                var lastDateObj = dt.Rows[dt.Rows.Count - 1][My_DataBase.RUN_TABLE.MyDate];
                if (lastDateObj == null || lastDateObj == DBNull.Value) return false;

                DateTime lastDate;
                if (lastDateObj is DateTime dtVal)
                    lastDate = dtVal.Date;
                else if (DateTime.TryParse(lastDateObj.ToString(), out var parsed))
                    lastDate = parsed.Date;
                else
                    return false;

                return lastDate != DateTime.Today;
            }
            catch (Exception ex)
            {
                Logger.Error("RunTableMan: NeedClearTable异常", ex);
                return false;
            }
        }

        /// <summary>
        /// 运行表改变事件参数扩展类
        /// </summary>
        public class RunTableChangedEventArgs : EventArgs
        {
            /// <summary>
            /// 变更类型（如 Insert）
            /// </summary>
            public string ChangeType { get; }

            /// <summary>
            /// 变更涉及的数据
            /// </summary>
            public Dictionary<string, object> Data { get; }

            public RunTableChangedEventArgs(string changeType, Dictionary<string, object> data)
            {
                ChangeType = changeType;
                Data = data;
            }
        }
    }
}