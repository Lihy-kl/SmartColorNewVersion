using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.WeighingMachine
{
    /// <summary>
    /// 称布机参数
    /// </summary>
    internal class Weighing
    {
        /// <summary>
        /// 称布系统 IP地址
        /// </summary>
        [Description("称布系统 IP地址")]
        public static  string HMIBaCloIP { get; set; } = "192.168.68.70";

        /// <summary>
        /// 称布系统 端口号
        /// </summary>
        [Description("称布系统 端口号")]
        public static  int HMIBaCloPort { get; set; } = 502;
    }
}
