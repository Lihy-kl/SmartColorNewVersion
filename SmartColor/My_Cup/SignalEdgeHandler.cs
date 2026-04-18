using System;
using System.Collections.Concurrent;

namespace SmartColor.My_Cup
{
    /// <summary>
    /// 通用信号上升沿+5秒防重入处理器
    /// </summary>
    public class SignalEdgeHandler
    {
        private readonly object _lock = new object();
        private readonly ConcurrentDictionary<int, ushort> _lastValue = new ConcurrentDictionary<int, ushort>();
        private readonly ConcurrentDictionary<int, DateTime> _lastTriggerTime = new ConcurrentDictionary<int, DateTime>();
        private readonly TimeSpan _minInterval = TimeSpan.FromMilliseconds(1000);

        /// <summary>
        /// 只在上升沿触发，且5秒内只处理一次上升沿
        /// </summary>
        /// <param name="key">信号唯一标识（如杯号）</param>
        /// <param name="currentValue">当前信号值</param>
        /// <param name="handler">信号处理方法（同步）</param>
        public void TryProcess(int key, ushort currentValue, Action handler)
        {
            lock (_lock)
            {
                ushort last = _lastValue.GetOrAdd(key, 0);
                DateTime now = DateTime.Now;
                DateTime lastTime = _lastTriggerTime.GetOrAdd(key, DateTime.MinValue);

                bool isRisingEdge = last == 0 && currentValue != 0;
                bool intervalOk = (now - lastTime) > _minInterval;

                if (isRisingEdge && intervalOk)
                {
                    _lastTriggerTime[key] = now;
                    handler();
                }

                _lastValue[key] = currentValue;
            }
        }

        /// <summary>
        /// 手动复位（如信号消隐后可调用）
        /// </summary>
        public void Reset(int key)
        {
            lock (_lock)
            {
                _lastValue[key] = 0;
                _lastTriggerTime[key] = DateTime.MinValue;
            }
        }
    }
}