using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Type
{
    /// <summary>
    /// 运动参数基类
    /// </summary>
    internal class BaseMotion
    {
        /// <summary>
        /// X轴回零偏移量
        /// </summary>
        [Description("X轴回零偏移量|p")]
        public  int Home_X_Offset { get; set; } 

        /// <summary>
        /// Y轴回零偏移量
        /// </summary>
        [Description("Y轴回零偏移量|p")]
        public  int Home_Y_Offset { get; set; } 

        /// <summary>
        /// Z轴回零偏移量
        /// </summary>
        [Description("Z轴回零偏移量|p")]
        public int Home_Z_Offset { get; set; } 

        /// <summary>
        /// X轴运行驱动速度
        /// </summary>
        [Description("X轴运行驱动速度|p/s")]
        public  int Move_X_HSpeed { get; set; }

        /// <summary>
        /// X轴运行加减速
        /// </summary>
        [Description("X轴运行加减速|p/s²")]
        public int Move_X_USpeed { get; set; } 

        /// <summary>
        /// Y轴运行驱动速度
        /// </summary>
        [Description("Y轴运行驱动速度|p/s")]
        public  int Move_Y_HSpeed { get; set; }

        /// <summary>
        /// Y轴运行加减速
        /// </summary>
        [Description("Y轴运行加减速|p/s²")]
        public int Move_Y_USpeed { get; set; }

        /// <summary>
        /// 小针筒运行驱动速度
        /// </summary>
        [Description("小针筒运行驱动速度|p/s")]
        public  int Move_S_HSpeed { get; set; }

        /// <summary>
        /// 小针筒运行加减速
        /// </summary>
        [Description("小针筒运行加减速|p/s²")]
        public int Move_S_USpeed { get; set; } 

        /// <summary>
        /// 小针筒运行慢速驱动速度
        /// </summary>
        [Description("小针筒运行慢速驱动速度|p/s")]
        public  int Move_S_MinHSpeed { get; set; } 

        /// <summary>
        /// 大针筒运行驱动速度
        /// </summary>
        [Description("大针筒运行驱动速度|p/s")]
        public  int Move_B_HSpeed { get; set; }

        /// <summary>
        /// 大针筒运行加减速
        /// </summary>
        [Description("大针筒运行加减速|p/s²")]
        public int Move_B_USpeed { get; set; }

        /// <summary>
        /// 大针筒运行慢速驱动速度
        /// </summary>
        [Description("大针筒运行慢速驱动速度|p/s")]
        public  int Move_B_MinHSpeed { get; set; } 
    }
}
