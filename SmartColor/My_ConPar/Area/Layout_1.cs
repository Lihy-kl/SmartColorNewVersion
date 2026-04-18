using SmartColor.My_ConPar.Area;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Area
{
    /// <summary>
    /// 常规滴液机布局参数
    /// </summary>
    internal class Layout_1
    {
        /// <summary>
        /// 区域1
        /// </summary>
        [Description("区域1")]
        public static  Base Area_1 { get; set; } = new Base()
        {
            AreaType = 9,
            AreaName = "母液瓶区域"

        };

        /// <summary>
        /// 区域2
        /// </summary>
        [Description("区域2")]
        public static  Base Area_2 { get; set; }=new Base()
        {
            AreaType = 1,
            AreaName = "天平区"
        };

        /// <summary>
        /// 区域3
        /// </summary>
        [Description("区域3")]
        public static  Base Area_3 { get; set; }=new Base()
        {
            AreaType = 8,
            AreaName = "滴液区"
        };

    }
}
