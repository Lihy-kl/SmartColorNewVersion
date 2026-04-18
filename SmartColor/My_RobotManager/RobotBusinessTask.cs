using SmartColor.My_Tool;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartColor.My_RobotManager
{
    public class RobotBusinessTask<TResult> : RobotBusinessTaskBase, IComparable<RobotBusinessTask<TResult>>
    {
        public override int Priority { get; set; }
        public int OriginalPriority { get; set; }
        public override object BusinessType
        {
            get => _businessType;
            set => _businessType = (My_ConPar.Order.BigProcess.RobotBusinessType)value;
        }
        private My_ConPar.Order.BigProcess.RobotBusinessType _businessType;
        public My_ConPar.Order.BigProcess.RobotBusinessType BusinessTypeField { get; set; }
        public override string TaskName { get; set; }
        public DateTime CreatedTime { get; } = DateTime.Now;
        public Func<Task<TResult>> BusinessFlow { get; set; }
        public TaskCompletionSource<TResult> CompletionSource { get; } = new TaskCompletionSource<TResult>();
        public int AtomPriority { get; set; } = int.MaxValue;
        public CancellationTokenSource CancellationSource { get; } = new CancellationTokenSource();

        // 预处理委托（可选）
        private Func<Task<object>> _precomputeAsync;
        public override Func<Task<object>> PrecomputeAsync
        {
            get => _precomputeAsync;
            set => _precomputeAsync = value;
        }

        // 预处理结果缓存
        private object _precomputeResult;
        public override object PrecomputeResult
        {
            get => _precomputeResult;
            set => _precomputeResult = value;
        }

        // 标记是否已预处理
        private bool _isPrecomputed = false;
        public override bool IsPrecomputed
        {
            get => _isPrecomputed;
            set => _isPrecomputed = value;
        }

        private volatile bool _isPaused = false;
        private readonly object _pauseLock = new object();
        public bool IsPaused => _isPaused;

        public override void Pause()
        {
            lock (_pauseLock)
            {
                _isPaused = true;
            }
        }

        public override void Resume()
        {
            lock (_pauseLock)
            {
                _isPaused = false;
                Monitor.PulseAll(_pauseLock);
            }
        }

        public override void Cancel()
        {
            CancellationSource.Cancel();
        }

        public async Task CheckPauseAndCancelAsync()
        {
            while (IsPaused)
            {
                await Task.Run(() =>
                {
                    lock (_pauseLock)
                    {
                        Monitor.Wait(_pauseLock, 100);
                    }
                });
            }
            CancellationSource.Token.ThrowIfCancellationRequested();
        }

        public async Task CheckPauseOnlyAsync()
        {

            while (IsPaused || My_Form.MachineDebugging.Debug.IsDebugFormActive)
            {

                MessageEventManager.Instance.RequestLoopSpeak("暂停任务", My_Form.MachineDebugging.Debug.IsDebugFormActive ? "调试页面已打开,请关闭调试页面继续任务" : "已暂停任务，请点击恢复任务按钮继续任务");


                await Task.Run(() =>
                {
                    lock (_pauseLock)
                    {
                        Monitor.Wait(_pauseLock, 100);
                    }
                });
            }

            MessageEventManager.Instance.RequestStopLoopSpeak("暂停任务");
        }

        public int CompareTo(RobotBusinessTask<TResult> other)
        {
            if (other == null) return -1;
            int cmp = Priority.CompareTo(other.Priority);
            if (cmp != 0) return cmp;
            cmp = AtomPriority.CompareTo(other.AtomPriority);
            if (cmp != 0) return cmp;
            cmp = CreatedTime.CompareTo(other.CreatedTime);
            if (cmp != 0) return cmp;
            return string.Compare(TaskName, other.TaskName, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (obj is RobotBusinessTask<TResult> other)
                return TaskName == other.TaskName && CreatedTime == other.CreatedTime;
            return false;
        }

        public override int GetHashCode()
        {
            return (TaskName ?? "").GetHashCode() ^ CreatedTime.GetHashCode();
        }

        // 新增私有方法
        private void SetResult(TResult result)
        {
            CompletionSource.SetResult(result);
        }

        // IRobotBusinessTask/RobotBusinessTaskBase接口实现
        public override Task ExecuteAsync()
        {
            return ExecuteAsyncImpl();
        }

        private async Task ExecuteAsyncImpl()
        {
            // 先执行预处理
            if (PrecomputeAsync != null && !IsPrecomputed)
            {
                PrecomputeResult = await PrecomputeAsync();
                IsPrecomputed = true;
            }
            TResult result = await BusinessFlow().ConfigureAwait(false);
            SetResult(result);
        }

        public override void SetResult(object result)
        {
            CompletionSource.SetResult((TResult)result);
        }

        public override void SetException(Exception ex)
        {
            CompletionSource.SetException(ex);
        }

        public override void TrySetCanceled()
        {
            CompletionSource.TrySetCanceled();
        }
    }
}