using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Area.Balance
{
    /// <summary>
    /// 天平参数
    /// </summary>
    public class Balance : Base
    {
        public Balance()
        {
            AreaType = 1;
            AreaName = "天平参数";
        }

        /// <summary>
        /// 天平类型
        /// 0：梅特勒百分位
        /// 1：新光千分位
        /// 2：AND百分位
        /// 3：KSS百分位
        /// 4：AND千分位
        /// </summary>

        [Description("天平类型 |0：梅特勒百分位  1：新光千分位  2：AND百分位  3：KSS百分位  4：AND千分位")]
        public int BalanceType { get; set; } = 0;

        /// <summary>
        /// 天平X轴坐标
        /// </summary>

        [Description("天平X轴坐标")]
        public int BalanceCX { get; set; } = 0;

        /// <summary>
        /// 天平Y轴坐标
        /// </summary>

        [Description("天平Y轴坐标")]
        public int BalanceCY { get; set; } = 0;

        /// <summary>
        /// 天平端口号
        /// </summary>

        [Description("天平端口号")]
        public string PortName { get; set; } = "COM4";

        /// <summary>
        /// 天平量程最大值
        /// </summary>

        [Description("天平量程最大值")]
        public int MaxValue { get; set; } = 1500;

        /// <summary>
        /// 波特率
        /// </summary>
        [Description("天平波特率")]
        public int BaudRate { get; set; } = 9600;

        /// <summary>
        /// 校验位（0=None, 1=Odd, 2=Even, 3=Mark, 4=Space）
        /// </summary>
        [Description("天平校验位|0=None, 1=Odd, 2=Even, 3=Mark, 4=Space")]
        public int Parity { get; set; } = 0;

        /// <summary>
        /// 数据位
        /// </summary>
        [Description("天平数据位")]
        public int DataBits { get; set; } = 8;

        /// <summary>
        /// 停止位（1=One, 2=Two, 3=OnePointFive）
        /// </summary>
        [Description("天平停止位|1=One, 2=Two, 3=OnePointFive")]
        public int StopBits { get; set; } = 1;



    }
}
