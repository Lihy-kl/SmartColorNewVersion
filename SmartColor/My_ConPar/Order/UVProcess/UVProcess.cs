using System;
using System.ComponentModel;

namespace SmartColor.My_ConPar.Order.UVProcess
{
    /// <summary>
    /// 吸光度机进程优先级参数
    /// </summary>
    internal class UVProcess
    {
        // 默认值集中管理
        public static  readonly int Default_AddDyeProcess = 1;
        public static  readonly int Default_ExtractionWaterProcess = 2;
        public static  readonly int Default_AddWaterProcess = 3;
        public static  readonly int Default_AddDissolvingAgentProcess = 4;

        /// <summary>
        /// 加药进程
        /// </summary>
        [Description("加药进程")]
        public static  int AddDyeProcess { get; set; } = Default_AddDyeProcess;

        /// <summary>
        /// 抽染液进程
        /// </summary>
        [Description("抽染液进程")]
        public static  int ExtractionWaterProcess { get; set; } = Default_ExtractionWaterProcess;

        /// <summary>
        /// 加水进程
        /// </summary>
        [Description("加水进程")]
        public static  int AddWaterProcess { get; set; } = Default_AddWaterProcess;

        /// <summary>
        /// 加溶解剂进程
        /// </summary>
        [Description("加溶解剂进程")]
        public static  int AddDissolvingAgentProcess { get; set; } = Default_AddDissolvingAgentProcess;

        /// <summary>
        /// 恢复默认顺序
        /// </summary>
        public static  void RestoreDefault()
        {
            AddDyeProcess = Default_AddDyeProcess;
            ExtractionWaterProcess = Default_ExtractionWaterProcess;
            AddWaterProcess = Default_AddWaterProcess;
            AddDissolvingAgentProcess = Default_AddDissolvingAgentProcess;
        }
    }
}