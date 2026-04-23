using SmartColor.My_File;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartColor.My_RobotManager
{
    public class RobotTaskManager
    {
        private static readonly Lazy<RobotTaskManager> _instance = new Lazy<RobotTaskManager>(() => new RobotTaskManager());
        public static RobotTaskManager Instance => _instance.Value;

        public event Action TaskQueueChanged;



        private readonly object _lock = new object();
        private readonly List<object> _taskQueue = new List<object>();
        private object _currentTask = null;
        private static bool _robotArmLoopStarted = false;
        private static readonly object _startLock = new object();

        public object CurrentTask
        {
            get
            {
                lock (_lock)
                {
                    return _currentTask;
                }
            }
        }

        private volatile bool _running = true;

        private RobotTaskManager()
        {
            lock (_startLock)
            {
                if (!_robotArmLoopStarted)
                {
                    Task.Run(() => RobotArmLoop());
                    _robotArmLoopStarted = true;
                }
            }
        }

        public Task<TResult> EnqueueTask<TResult>(RobotBusinessTask<TResult> task)
        {
            lock (_lock)
            {


                // 统一去重：只要 BusinessType 和 TaskName 相同就不加入
                // 检查队列和当前任务，防止重复
                bool exists = _taskQueue.Concat(new[] { _currentTask })
                    .Any(t =>
                    {
                        if (t is IRobotBusinessTask ibt)
                        {
                            return Equals(ibt.BusinessType, task.BusinessType) &&
                                   ibt.TaskName == task.TaskName;
                        }
                        return false;
                    });
                if (exists)
                {
                   // Logger.Error($"{task.TaskName}任务已存在，忽略重复任务");
                    return task.CompletionSource.Task;
                }


                int idx = _taskQueue.FindIndex(t =>
                {
                    if (t is IRobotBusinessTask ibt)
                        return ibt.Priority > task.Priority;
                    return false;
                });
                if (idx < 0)
                    _taskQueue.Add(task); // 没有比它优先级低的，插到最后
                else
                    _taskQueue.Insert(idx, task); // 插到第一个比它优先级低的前面
                Monitor.Pulse(_lock);
            }
            TaskQueueChanged?.Invoke();
            return task.CompletionSource.Task;
        }

        private async Task RobotArmLoop()
        {
            TimeSpan idleTimeout = TimeSpan.FromMinutes(10);
            Task precomputeTask = null;

            while (_running)
            {
                object taskObj = null;
                bool isTimeout = false;
                lock (_lock)
                {
                    while (_taskQueue.Count == 0)
                    {
                        if (!Monitor.Wait(_lock, idleTimeout))
                        {
                            isTimeout = true;
                            break;
                        }
                    }
                    if (_taskQueue.Count > 0)
                    {
                        taskObj = _taskQueue[0];
                        _taskQueue.RemoveAt(0);
                        _currentTask = taskObj;

                        //_ = RunTableMan.InsertAsync(new Dictionary<string, object>
                        //{
                        //    [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"[RobotTaskManager] 取出任务: {GetTaskInfo(taskObj)}，当前线程ID: {Thread.CurrentThread.ManagedThreadId}"
                        //}, DateTime.Now);
                    }
                }
                TaskQueueChanged?.Invoke();

                // ====== 新增：预处理下一个任务 ======
                object nextTaskObj = null;
                lock (_lock)
                {
                    if (_taskQueue.Count > 0)
                        nextTaskObj = _taskQueue[0];
                }
                if (nextTaskObj is IRobotBusinessTask ibt
                         && ibt is RobotBusinessTaskBase nextTaskBase
                         && nextTaskBase.PrecomputeAsync != null
                         && !nextTaskBase.IsPrecomputed)
                {
                    precomputeTask = Task.Run(async () =>
                    {
                        try
                        {

                            //_ = RunTableMan.InsertAsync(new Dictionary<string, object>
                            //{
                            //    [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"[RobotTaskManager] 预处理任务: {GetTaskInfo(nextTaskObj)}"
                            //}, DateTime.Now);
                            nextTaskBase.PrecomputeResult = await nextTaskBase.PrecomputeAsync();
                            nextTaskBase.IsPrecomputed = true;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("任务预处理异常", ex);
                        }
                    });
                }
                // ====== 新增结束 ======

                if (isTimeout)
                {
                    try
                    {
                        Logger.Info("[RobotTaskManager] 机械手空闲超时，准备回待机位");

                        if (!SmartColor.My_Form.MachineDebugging.Debug.IsDebugFormActive)
                        {
                            await SmartColor.My_SemiAutoModule.SemiAutoHelperFactory.Current.MoveToStandbyAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("机械手空闲超时回待机位异常", ex);
                    }
                    continue;
                }

                if (taskObj is IRobotBusinessTask robotTask)
                {
                    try
                    {
                        
                        _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                        {
                            [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"开始执行任务: {GetTaskInfo(taskObj)}"
                        }, DateTime.Now);
                        await robotTask.ExecuteAsync().ConfigureAwait(false);
                        _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                        {
                            [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"任务执行完成: {GetTaskInfo(taskObj)}"
                        }, DateTime.Now);
                    }
                    catch (OperationCanceledException)
                    {
                        Logger.Error($"[RobotTaskManager] 任务被取消: {GetTaskInfo(taskObj)}");
                        robotTask.TrySetCanceled();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"[RobotTaskManager] 任务执行异常: {GetTaskInfo(taskObj)}", ex);
                        robotTask.SetException(ex);
                    }
                }

                lock (_lock)
                {

                    //_ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    //{
                    //    [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"任务清理: {GetTaskInfo(taskObj)}"
                    //}, DateTime.Now);
                    _currentTask = null;
                }
                TaskQueueChanged?.Invoke();
            }
        }

        // 辅助方法：获取任务关键信息
        private string GetTaskInfo(object taskObj)
        {
            if (taskObj is IRobotBusinessTask ibt)
                return $"{ibt.TaskName}(优先级:{ibt.Priority}):";
            return taskObj?.ToString() ?? "null";
        }

        public void Stop()
        {
            _running = false;
        }

        public List<object> GetAllTasksSnapshot()
        {
            lock (_lock)
            {
                return _taskQueue.ToList();
            }
        }

        public List<object> GetAllVisualTasks()
        {
            lock (_lock)
            {
                var list = _taskQueue.ToList();
                if (_currentTask != null)
                    list.Insert(0, _currentTask);
                return list;
            }
        }

        public void UpdateTaskPriorities(List<object> newOrder)
        {
            lock (_lock)
            {
                _taskQueue.Clear();
                _taskQueue.AddRange(newOrder);
            }
            TaskQueueChanged?.Invoke();
        }

       

        public void ResetToPriorityOrder()
        {
            lock (_lock)
            {
                _taskQueue.Sort((a, b) =>
                {
                    if (a is IRobotBusinessTask ia && b is IRobotBusinessTask ib)
                        return ia.Priority.CompareTo(ib.Priority);
                    return 0;
                });
            }
            TaskQueueChanged?.Invoke();
        }

        public void CancelTask(object taskObj)
        {
            lock (_lock)
            {
                if (_taskQueue.Contains(taskObj))
                {
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"取消任务: {GetTaskInfo(taskObj)}"
                    }, DateTime.Now);
                    _taskQueue.Remove(taskObj);
                    if (taskObj is IRobotBusinessTask dt)
                    {
                        dt.Cancel();
                        dt.TrySetCanceled();
                    }
                    TaskQueueChanged?.Invoke();
                }
                else if (_currentTask == taskObj)
                {
                    if (taskObj is IRobotBusinessTask dt)
                        dt.Cancel();
                }
            }
        }

        public void PauseTask(object taskObj)
        {
            lock (_lock)
            {
                if (_currentTask == taskObj)
                {
                    if (taskObj is IRobotBusinessTask dt)
                        dt.Pause();
                }
            }
        }

        public void ResumeTask(object taskObj)
        {
            lock (_lock)
            {
                if (_currentTask == taskObj)
                {
                    if (taskObj is IRobotBusinessTask dt)
                        dt.Resume();
                }
            }
        }

        /// <summary>
        /// 判断队列中是否存在比指定优先级更高的任务
        /// </summary>
        /// <param name="currentPriority">当前任务优先级</param>
        /// <returns>是否存在更高优先级任务</returns>
        public bool HasHigherPriorityTask(int currentPriority)
        {
            lock (_lock)
            {
                foreach (var t in _taskQueue)
                {
                    try
                    {
                        if (t is IRobotBusinessTask ibt)
                        {
                            if (ibt.Priority < currentPriority)
                                return true;
                        }
                    }
                    catch(Exception ex)
                    {
                        Logger.Error($"检查任务优先级时发生异常：",ex);
                    }
                }
            }
            return false;
        }
    }
}