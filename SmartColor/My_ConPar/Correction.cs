using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SmartColor.My_ConPar
{
    /// <summary>
    /// 校正参数类
    /// </summary>
    internal class Correction
    {
        /// <summary>
        /// 大针筒校正脉冲
        /// </summary>
        [Description("大针筒校正脉冲|p")]
        public static int Correcting_B_Pulse { get; set; } = 30000;

        /// <summary>
        /// 大针筒验证重量
        /// </summary>
        [Description("大针筒验证重量|g")]
        public static int Correcting_B_Weight { get; set; } = 1;

        /// <summary>
        /// 小针筒校正脉冲
        /// </summary>
        [Description("小针筒校正脉冲|p")]
        public static int Correcting_S_Pulse { get; set; } = 80000;

        /// <summary>
        /// 小针筒验证重量
        /// </summary>
        [Description("小针筒验证重量|g")]
        public static int Correcting_S_Weight { get; set; } = 1;

        /// <summary>
        /// 加水校正时间
        /// </summary>
        [Description("加水校正时间|s")]
        public static int Correcting_Water_Time { get; set; } = 5;

        /// <summary>
        /// 加水验证重量
        /// </summary>
        [Description("加水验证重量|g")]
        public static int Correcting_Water_RWeight { get; set; } = 20;

        /// <summary>
        /// 加水校正值
        /// </summary>
        [Description("加水校正值|g/s")]
        public static double Correcting_Water_Value { get; set; } = 0;

        /// <summary>
        /// 流量计校正值
        /// </summary>
        [Description("流量计校正值|p/g")]
        public static double Correcting_FlowPulse_Value { get; set; } = 0;

        /// <summary>
        /// 加溶解剂校正时间
        /// </summary>
        [Description("加溶解剂校正时间|s")]
        public static int Correcting_Dissolving_Time { get; set; } = 5;

        /// <summary>
        /// 加溶解剂验证重量
        /// </summary>
        [Description("加溶解剂验证重量|g")]
        public static int Correcting_Dissolving_RWeight { get; set; } = 20;

        /// <summary>
        /// 加溶解剂校正值
        /// </summary>
        [Description("加溶解剂校正值|g/s")]
        public static double Correcting_Dissolving_Value { get; set; } = 0;

        

    }
}
