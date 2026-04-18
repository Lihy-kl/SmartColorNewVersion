using System;
using System.ComponentModel;

namespace SmartColor.My_ConPar.Order.DropProcess
{
    /// <summary>
    /// 滴液进程优先级参数
    /// </summary>
    internal class DropProcess
    {
        // 默认值集中管理
        public static readonly int Default_AddWaterProcess = 1;
        public static readonly int Default_AddDyeProcess = 2;
        public static readonly int Default_AddAuxiliaryAgentProcess = 3;

        /// <summary>
        /// 加水进程
        /// </summary>
        [Description("加水进程")]
        public static int AddWaterProcess { get; set; } = Default_AddWaterProcess;

        /// <summary>
        /// 加染料进程
        /// </summary>
        [Description("加染料进程")]
        public static int AddDyeProcess { get; set; } = Default_AddDyeProcess;

        /// <summary>
        /// 加助剂进程
        /// </summary>
        [Description("加助剂进程")]
        public static int AddAuxiliaryAgentProcess { get; set; } = Default_AddAuxiliaryAgentProcess;

        /// <summary>
        /// 恢复默认顺序
        /// </summary>
        public static void RestoreDefault()
        {
            AddWaterProcess = Default_AddWaterProcess;
            AddDyeProcess = Default_AddDyeProcess;
            AddAuxiliaryAgentProcess = Default_AddAuxiliaryAgentProcess;
        }
    }
}