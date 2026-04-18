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
    /// 常规染色机布局参数
    /// </summary>
    internal class Layout_2
    {
        /// <summary>
        /// 区域1
        /// </summary>
        [Description("区域1")]
        public static  Base Area_1 { get; set; } = new Base()
        {
            AreaType = 9,
            AreaName = "母液瓶区"
        };

        /// <summary>
        /// 区域2
        /// </summary>
        [Description("区域2")]
        public static  Base Area_2 { get; set; } = new Base()
        {
            AreaType = 8,
            AreaName = "滴液区"
        };

        /// <summary>
        /// 区域3|母液瓶区域
        /// </summary>
        [Description("区域3")]
        public static  Base Area_3 { get; set; } = new Base()
        {
            AreaType = 4,
            AreaName = "1号打版区"

        };

        /// <summary>
        /// 区域1
        /// </summary>
        [Description("区域4")]
        public static  Base Area_4 { get; set; } = new Base()
        {
            AreaType = 4,
            AreaName = "2号打版区"
        };

        /// <summary>
        /// 区域2
        /// </summary>
        [Description("区域5")]
        public static  Base Area_5 { get; set; } = new Base()
        {
            AreaType = 4,
            AreaName = "3号打版区"
        };

        /// <summary>
        /// 区域3|母液瓶区域
        /// </summary>
        [Description("区域6")]
        public static  Base Area_6 { get; set; } = new Base()
        {
            AreaType = 3,
            AreaName = "4号打版区"

        };
    }
}
