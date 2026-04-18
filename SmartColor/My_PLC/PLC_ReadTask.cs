using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>
    /// 读任务结构体。用于描述一次PLC批量读操作的起始地址、长度和回调处理方法。
    /// 支持合并多个连续或重叠的读任务，提升通讯效率。
    /// </summary>
    internal class PLC_ReadTask
    {
        public ushort StartAddr { get; set; }
        public ushort Count { get; set; }
        public Action<ushort[]> Callback { get; set; }
        public PLC_ReadTask(ushort startAddr, ushort count, Action<ushort[]> callback)
        {
            StartAddr = startAddr;
            Count = count;
            Callback = callback;
        }
    }
}
