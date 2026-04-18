using System;
using System.Threading.Tasks;

namespace SmartColor.My_RobotManager
{

    public abstract class RobotBusinessTaskBase : IRobotBusinessTask
    {
        public abstract int Priority { get; set; }
        public abstract string TaskName { get; set; }
        public abstract object BusinessType { get; set; }
        public abstract Func<Task<object>> PrecomputeAsync { get; set; }
        public abstract object PrecomputeResult { get; set; }
        public abstract bool IsPrecomputed { get; set; }

        // IRobotBusinessTask接口方法声明
        public abstract Task ExecuteAsync();
        public abstract void SetResult(object result);
        public abstract void SetException(Exception ex);
        public abstract void TrySetCanceled();
        public abstract void Cancel();
        public abstract void Pause();
        public abstract void Resume();
    }
    public interface IRobotBusinessTask
    {
        int Priority { get; }
        string TaskName { get; }
        object BusinessType { get; }
        Task ExecuteAsync();
        void SetResult(object result);
        void SetException(Exception ex);
        void TrySetCanceled();
        void Cancel();
        void Pause();
        void Resume();
    }
}