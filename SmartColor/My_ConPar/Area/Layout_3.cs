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
    /// 全自动染色机布局参数
    /// </summary>
    internal class Layout_3
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
            AreaType = 4,
            AreaName = "1号打版区"
        };

        /// <summary>
        /// 区域3
        /// </summary>
        [Description("区域3")]
        public static  Base Area_3 { get; set; } = new Base()
        {
            AreaType = 4,
            AreaName = "2号打版区"

        };

        /// <summary>
        /// 区域4
        /// </summary>
        [Description("区域4")]
        public static  Base Area_4 { get; set; } = new Base()
        {
            AreaType = 4,
            AreaName = "3号打版区"
        };

        /// <summary>
        /// 区域5
        /// </summary>
        [Description("区域5")]
        public static  Base Area_5 { get; set; } = new Base()
        {
            AreaType = 15,
            AreaName = "1号出布区"
        };

        /// <summary>
        /// 区域6
        /// </summary>
        [Description("区域6")]
        public static  Base Area_6 { get; set; } = new Base()
        {
            AreaType = 15,
            AreaName = "2号出布区"

        };

        /// <summary>
        /// 区域7
        /// </summary>
        [Description("区域7")]
        public static  Base Area_7 { get; set; } = new Base()
        {
            AreaType = 15,
            AreaName = "3号出布区"

        };

        /// <summary>
        /// 区域8
        /// </summary>
        [Description("区域8")]
        public static  Base Area_8 { get; set; } = new Base()
        {
            AreaType = 1,
            AreaName = "电子秤区"

        };

        /// <summary>
        /// 区域9
        /// </summary>
        [Description("区域9")]
        public static  Base Area_9 { get; set; } = new Base()
        {
            AreaType = 10,
            AreaName = "干布夹具区"

        };

        /// <summary>
        /// 区域10
        /// </summary>
        [Description("区域10")]
        public static  Base Area_10 { get; set; } = new Base()
        {
            AreaType = 10,
            AreaName = "湿布夹具区"

        };

        /// <summary>
        /// 区域11
        /// </summary>
        [Description("区域11")]
        public static  Base Area_11 { get; set; } = new Base()
        {
            AreaType = 13,
            AreaName = "洗针筒区"

        };

        /// <summary>
        /// 区域12
        /// </summary>
        [Description("区域12")]
        public static  Base Area_12 { get; set; } = new Base()
        {
            AreaType = 12,
            AreaName = "公共针筒区"

        };

        /// <summary>
        /// 区域13
        /// </summary>
        [Description("区域13")]
        public static  Base Area_13 { get; set; } = new Base()
        {
            AreaType = 14,
            AreaName = "1号备布区"

        };

        /// <summary>
        /// 区域14
        /// </summary>
        [Description("区域14")]
        public static  Base Area_14 { get; set; } = new Base()
        {
            AreaType = 14,
            AreaName = "2号备布区"

        };

        /// <summary>
        /// 区域15
        /// </summary>
        [Description("区域15")]
        public static  Base Area_15 { get; set; } = new Base()
        {
            AreaType = 14,
            AreaName = "3号备布区"

        };

        /// <summary>
        /// 区域16
        /// </summary>
        [Description("区域16")]
        public static  Base Area_16 { get; set; } = new Base()
        {
            AreaType = 8,
            AreaName = "1号滴液区"

        };

        /// <summary>
        /// 区域17
        /// </summary>
        [Description("区域17")]
        public static  Base Area_17 { get; set; } = new Base()
        {
            AreaType = 8,
            AreaName = "2号滴液区"

        };

        /// <summary>
        /// 区域18
        /// </summary>
        [Description("区域18")]
        public static  Base Area_18 { get; set; } = new Base()
        {
            AreaType = 8,
            AreaName = "3号滴液区"

        };
    }
}
