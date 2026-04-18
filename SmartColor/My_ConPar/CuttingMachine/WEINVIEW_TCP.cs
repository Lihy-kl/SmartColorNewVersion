using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.CuttingMachine
{
    /// <summary>
    /// 威纶通触摸屏TCP
    /// </summary>
    internal class WEINVIEW_TCP
    {
        /// <summary>
        /// IP地址
        /// </summary>
        [Description("IP地址")]
        public string IP { get; set; } = "192.168.68.50";

        /// <summary>
        /// 端口号
        /// </summary>
        [Description("端口号")]
        public int Port { get; set; } = 502;
    }
}
