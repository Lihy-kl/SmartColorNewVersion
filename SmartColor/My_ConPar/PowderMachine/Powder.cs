using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.PowderMachine
{
    /// <summary>
    /// 称粉机参数
    /// </summary>
    internal class Powder
    {
        /// <summary>
        /// 称粉机 IP地址
        /// </summary>
        [Description("称粉机 IP地址")]
        public static  string HMIPMIP { get; set; } = "192.168.68.70";

        /// <summary>
        /// 称粉机 端口号
        /// </summary>
        [Description("称粉机 端口号")]
        public static  int HMIPMPort { get; set; } = 502;
    }
}
