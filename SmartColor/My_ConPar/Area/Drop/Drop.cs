using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Area.Drop
{
    /// <summary>
    /// 滴液区
    /// </summary>
    internal class Drop : Base
    {
        public Drop()
        {
            AreaType = 8;
            AreaName = "滴液区";
        }

        /// <summary>
        /// 行数
        /// </summary>
       
        [Description("行数")]
        public int Row { get; set; } = 4;

        /// <summary>
        /// 列数
        /// </summary>
       
        [Description("列数")]
        public int Column { get; set; } = 3;

        /// <summary>
        /// 开始杯号
        /// </summary>
       
        [Description("开始杯号")]
        public int StartPosition { get; set; } = 1;

        /// <summary>
        /// 顺序编号方式
        /// 0：横着编号
        /// 1：竖着编号
        /// </summary>
       
        [Description("顺序编号方式|0：横着编号  1：竖着编号")]
        public int Vertical { get; set; } = 0;

        /// <summary>
        /// 该区域首杯杯心X坐标
        /// </summary>

        [Description("该区域首杯杯心X坐标")]
        public int CupCX_1 { get; set; } = 0;

        /// <summary>
        /// 该区域首杯杯心Y坐标
        /// </summary>

        [Description("该区域首杯杯心Y坐标")]
        public int CupCY_1 { get; set; } = 0;

        /// <summary>
        /// X坐标间隔
        /// </summary>
       
        [Description("X坐标间隔(12杯框间隔8000，30杯间隔)")]
        public int CupIX { get; set; } = 8000;

        /// <summary>
        /// Y坐标间隔
        /// </summary>
       
        [Description("Y坐标间隔(12杯框间隔8000，30杯间隔)")]
        public int CupIY { get; set; } = 8000;


    }
}
