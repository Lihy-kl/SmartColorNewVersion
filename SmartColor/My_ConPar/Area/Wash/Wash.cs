using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Area.Wash
{
    /// <summary>
    /// 清洗针筒模组参数
    /// </summary>
    internal class Wash:Base
    {
        public Wash() 
        {
            AreaType = 13;
            AreaName = "清洗针筒";
        }

        /// <summary>
        /// 清洗针筒模组X轴坐标
        /// </summary>
       
        [Description("清洗针筒模组X轴坐标")]
        public int X { get; set; } = 0;

        /// <summary>
        /// 清洗针筒模组Y轴坐标
        /// </summary>
       
        [Description("清洗针筒模组Y轴坐标")]
        public int Y { get; set; } = 0;
    }
}
