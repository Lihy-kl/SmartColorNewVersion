using System;
using System.ComponentModel;

namespace SmartColor.My_ConPar.Order.BrewProcess
{
    /// <summary>
    /// 自动开料进程优先级参数
    /// </summary>
    internal class BrewProcess
    {
        // 默认值集中管理
        public static readonly int Default_OutProcess = 1;
        public static readonly int Default_InProcess = 2;

        /// <summary>
        /// 取瓶进程
        /// </summary>
        [Description("取瓶进程")]
        public static int OutProcess { get; set; } = Default_OutProcess;

        /// <summary>
        /// 放瓶进程
        /// </summary>
        [Description("放瓶进程")]
        public static int InProcess { get; set; } = Default_InProcess;

        /// <summary>
        /// 恢复默认顺序
        /// </summary>
        public static void RestoreDefault()
        {
            OutProcess = Default_OutProcess;
            InProcess = Default_InProcess;
        }
    }
}