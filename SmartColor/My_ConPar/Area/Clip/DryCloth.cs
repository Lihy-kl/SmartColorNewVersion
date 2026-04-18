using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Area.Clip
{
    /// <summary>
    /// 干布夹具模组参数
    /// </summary>
    internal class DryCloth:Base
    {
        
        public DryCloth()
        {
            AreaType = 10;
            AreaName = "干布夹具";
        }

        /// <summary>
        /// 干布夹具X轴坐标
        /// </summary>
       
        [Description("干布夹具X轴坐标")]
        public int X { get; set; } = 0;

        /// <summary>
        /// 干布夹具Y轴坐标
        /// </summary>
       
        [Description("干布夹具Y轴坐标")]
        public int Y { get; set; } = 0;
       
    }
}
