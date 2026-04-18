using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.CuttingMachine
{
    /// <summary>
    /// 威纶通触摸屏RTU
    /// </summary>
    internal class WEINVIEW_RTU
    {
        /// <summary>
        /// 威纶通触摸屏端口号
        /// </summary>
        [Description("威纶通触摸屏端口号")]
        public string PortName { get; set; } = "COM2";

        /// <summary>
        /// 波特率
        /// </summary>
        [Description("威纶通触摸屏波特率")]
        public int BaudRate { get; set; } = 9600;

        /// <summary>
        /// 校验位（0=None, 1=Odd, 2=Even, 3=Mark, 4=Space）
        /// </summary>
        [Description("威纶通触摸屏校验位|0=None, 1=Odd, 2=Even, 3=Mark, 4=Space")]
        public int Parity { get; set; } = 0;

        /// <summary>
        /// 数据位
        /// </summary>
        [Description("威纶通触摸屏数据位")]
        public int DataBits { get; set; } = 8;

        /// <summary>
        /// 停止位（1=One, 2=Two, 3=OnePointFive）
        /// </summary>
        [Description("威纶通触摸屏停止位|1=One, 2=Two, 3=OnePointFive")]
        public int StopBits { get; set; } = 1;

       
    }
}