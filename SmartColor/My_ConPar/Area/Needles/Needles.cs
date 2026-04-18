using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Area.Needles
{
    /// <summary>
    /// 公共针筒模组参数
    /// </summary>
    internal class Needles:Base
    {
        public Needles() 
        {
            AreaType = 12;
            AreaName = "公共针筒";
        }

        /// <summary>
        /// 公共针筒X轴坐标
        /// </summary>
       
        [Description("公共针筒X轴坐标")]
        public int X { get; set; } = 0;

        /// <summary>
        /// 公共针筒Y轴坐标
        /// </summary>
       
        [Description("公共针筒Y轴坐标")]
        public int Y { get; set; } = 0;
    }
}
