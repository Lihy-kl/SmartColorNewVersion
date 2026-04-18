using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Order.PostProcess
{
    /// <summary>
    /// 后处理进程优先级参数
    /// </summary>
    internal class PostProcess
    {
        // 默认值集中管理
        public static readonly int Default_AddChemicalProcess = 1;
        public static readonly int Default_ColdRunProcess = 2;
        public static readonly int Default_TemperatureControlProcess = 3;
        public static readonly int Default_PutClothProcess = 4;
        public static readonly int Default_OutClothProcess = 5;
        public static readonly int Default_DrainProcess = 6;
        public static readonly int Default_AddWaterProcess = 7;
        public static readonly int Default_StirProcess = 8;
        public static readonly int Default_SampleProcess = 9;
        public static readonly int Default_PHProcess = 10;

        /// <summary>
        /// 冷行
        /// </summary>
        [Description("冷行进程")]
        public static int ColdRunProcess { get; set; } = Default_ColdRunProcess;

        /// <summary>
        /// 温控
        /// </summary>
        [Description("温控进程")]
        public static int TemperatureControlProcess { get; set; } = Default_TemperatureControlProcess;

        /// <summary>
        /// 加药
        /// </summary>
        [Description("加药进程")]
        public static int AddChemicalProcess { get; set; } = Default_AddChemicalProcess;

        /// <summary>
        /// 放布
        /// </summary>
        [Description("放布进程")]
        public static int PutClothProcess { get; set; } = Default_PutClothProcess;

        /// <summary>
        /// 出布
        /// </summary>
        [Description("出布进程")]
        public static int OutClothProcess { get; set; } = Default_OutClothProcess;

        /// <summary>
        /// 排液
        /// </summary>
        [Description("排液进程")]
        public static int DrainProcess { get; set; } = Default_DrainProcess;

        /// <summary>
        /// 加水
        /// </summary>
        [Description("加水进程")]
        public static int AddWaterProcess { get; set; } = Default_AddWaterProcess;

        /// <summary>
        /// 搅拌
        /// </summary>
        [Description("搅拌进程")]
        public static int StirProcess { get; set; } = Default_StirProcess;

        /// <summary>
        /// 取小样
        /// </summary>
        [Description("取小样进程")]
        public static int SampleProcess { get; set; } = Default_SampleProcess;

        /// <summary>
        /// 测PH
        /// </summary>
        [Description("测PH进程")]
        public static int PHProcess { get; set; } = Default_PHProcess;

        // 恢复默认值方法
        public static void RestoreDefault()
        {
            ColdRunProcess = Default_ColdRunProcess;
            TemperatureControlProcess = Default_TemperatureControlProcess;
            AddChemicalProcess = Default_AddChemicalProcess;
            PutClothProcess = Default_PutClothProcess;
            OutClothProcess = Default_OutClothProcess;
            DrainProcess = Default_DrainProcess;
            AddWaterProcess = Default_AddWaterProcess;
            StirProcess = Default_StirProcess;
            SampleProcess = Default_SampleProcess;
            PHProcess = Default_PHProcess;
        }
    }
}
