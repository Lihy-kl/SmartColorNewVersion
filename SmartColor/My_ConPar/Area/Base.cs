using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Area
{
    public class Base
    {
        /// <summary>
        /// 区域类型
        /// 0：无
        /// 1：天平
        /// 2：4杯大翻转缸
        /// 3：6杯翻转缸
        /// 4：12杯翻转缸
        /// 5：16杯翻转缸
        /// 6：ABS缸
        /// 7：10杯转子缸
        /// 8：滴液区
        /// 9：母液瓶区
        /// 10：干布夹具
        /// 11：湿布夹具
        /// 12：公共针筒
        /// 13：清洗针筒
        /// 14：备布区
        /// 15：出布区
        /// </summary>
        [Description("区域类型|0：无  1：天平  2：4杯大翻转缸  3：6杯翻转缸  " +
            "4：12杯翻转缸  5：16杯翻转缸  6：ABS缸  7：10杯转子缸  8：滴液区  " +
            "9：母液瓶区  10：干布夹具  11：湿布夹具  12：公共针筒  13：清洗针筒  " +
            "14：备布区  15：出布区")]
        public int AreaType { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        [Description("区域名称")]
        public string AreaName { get; set; }

     
    }
}
