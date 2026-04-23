using SmartColor.My_File;
using SmartColor.My_PLC;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SmartColor.My_PLC.PLC;

namespace SmartColor.My_ADT8940A1
{
    internal class ADT8940A1
    {
        /// <summary>
        /// 通知机械手状态更新
        /// </summary>
        public event Action<string> ShowState;

        /// <summary>
        /// 异步更新动作
        /// </summary>
        /// <param name="param">半自动动作参数（包含所有协议项）</param>
        /// <param name="runningCode">运行中标志值（通常为1）</param>
        /// <returns>Task，完成时返回PLC的实际结果码（2为成功，其他为各自错误码）</returns>
        public Task<int> UpdateStatusAsync(string s)
        {
            return Task.Run(() =>
            {

                Logger.Info($"半自动动作发送，名称:{s}");
                ShowState?.Invoke(s);

                return Task.FromResult(0);
            });
        }
    }
}
