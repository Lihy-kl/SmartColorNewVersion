using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Area.PrepareClothArea
{
    /// <summary>
    /// 备布区参数
    /// </summary>
    internal class PrepareClothArea:Base
    {
        public PrepareClothArea() {

            AreaType = 14;
            AreaName = "备布区";
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
        public int Column { get; set; } = 6;

        /// <summary>
        /// 开始杯号
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
        /// 该区域第一个备布位X坐标
        /// </summary>

        [Description("该区域第一个备布位X坐标")]
        public int PrepareClothX_1 { get; set; } = 0;

        /// <summary>
        /// 该区域第一个备布位Y坐标
        /// </summary>

        [Description("该区域第一个备布位Y坐标")]
        public int PrepareClothY_1 { get; set; } = 0;

        /// <summary>
        /// X坐标间隔
        /// </summary>
       
        [Description("X坐标间隔")]
        public int PrepareClothIX { get; set; } = 5200;

        /// <summary>
        /// Y坐标间隔
        /// </summary>
       
        [Description("Y坐标间隔")]
        public int PrepareClothIY { get; set; } = 6000;

        /// <summary>
        /// 备布区类型 | 0：20g布  1：50g布
        /// </summary>

        [Description("备布区类型 | 0：5g布(IX:5200;IY:6000)  1：50g布(IX:8000;IY:8000)")]
        public int PrepareCloth { get; set; } = 0;
    }
}
