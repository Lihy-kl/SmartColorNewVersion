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
    /// 湿布夹具模组参数
    /// </summary>
    internal class WetCloth:Base
    {
        public WetCloth()
        {
            AreaType = 11;
            AreaName = "湿布夹具";
        }

        /// <summary>
        /// 湿布夹具X坐标
        /// </summary>
       
        [Description("湿布夹具X轴坐标")]
        public int X { get; set; } = 0;

        /// <summary>
        /// 湿布夹具Y坐标
        /// </summary>
       
        [Description("湿布夹具Y轴坐标")]
        public int Y { get; set; } = 0;

    }
}
