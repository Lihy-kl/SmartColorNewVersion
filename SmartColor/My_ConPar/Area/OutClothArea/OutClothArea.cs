using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Area.OutClothArea
{
    /// <summary>
    /// 出布区参数
    /// </summary>
    internal class OutClothArea:Base
    {
        public OutClothArea() {

            AreaType = 15;
            AreaName = "出布区";
        }

        /// <summary>
        /// 行数
        /// </summary>
       
        [Description("行数")]
        public int Row { get; set; } = 3;

        /// <summary>
        /// 列数
        /// </summary>
       
        [Description("列数")]
        public int Column { get; set; } = 4;

        /// <summary>
        /// 开始编号
        /// </summary>

        [Description("开始编号")]
        public int StartPosition { get; set; } = 1;

        /// <summary>
        /// 顺序编号方式
        /// 0：横着编号
        /// 1：竖着编号
        /// </summary>
       
        [Description("顺序编号方式|0：横着编号  1：竖着编号")]
        public int Vertical { get; set; } = 0;

        /// <summary>
        /// 该区域第一个出布位X坐标
        /// </summary>
       
        [Description("该区域第一个出布位X坐标")]
        public int OutClothX_1 { get; set; } = 0;

        /// <summary>
        /// 该区域第一个出布位Y坐标
        /// </summary>

        [Description("该区域第一个出布位Y坐标")]
        public int OutClothY_1 { get; set; } = 0;

        /// <summary>
        /// X坐标间隔
        /// </summary>
       
        [Description("X坐标间隔")]
        public int OutClothIX { get; set; } = 6000;

        /// <summary>
        /// Y坐标间隔
        /// </summary>
       
        [Description("Y坐标间隔")]
        public int OutClothIY { get; set; } = 7800;


    }
}
