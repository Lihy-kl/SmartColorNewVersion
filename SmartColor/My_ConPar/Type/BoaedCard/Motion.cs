using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Type.BoaedCard
{
    /// <summary>
    /// 板卡运动类
    /// </summary>
    internal class Motion : BaseMotion
    {

        public Motion()
        {
            Home_X_Offset = 100;
            Home_Y_Offset = 100;
            Home_Z_Offset = 1500;
            Move_X_HSpeed = 5000;
            Move_Y_HSpeed = 5000;
            Move_S_HSpeed  = 25000;
            Move_S_MinHSpeed = 7500;
            Move_B_HSpeed= 7500;
            Move_B_MinHSpeed = 4000;

            Move_X_USpeed  = 2500;
            Move_Y_USpeed  = 2500;
            Move_S_USpeed = 60000;
            Move_B_USpeed = 10000;
        }

        #region 回零参数

        /// <summary>
        /// X轴回零起始速度
        /// </summary>
        [Description("X轴回零起始速度|p/s")]
        public  int Home_X_LSpeed { get; set; } = 0;

        /// <summary>
        /// X轴回零驱动速度
        /// </summary>
        [Description("X轴回零驱动速度|p/s")]
        public  int Home_X_HSpeed { get; set; } = 5000;

        /// <summary>
        /// X轴回零加速度
        /// </summary>
        [Description("X轴回零加速度|p/s")]
        public  int Home_X_USpeed { get; set; } = 100;

        /// <summary>
        /// X轴回零爬行速度
        /// </summary>
        [Description("X轴回零爬行速度|p/s")]
        public  int Home_X_CSpeed { get; set; } = 1000;

        /// <summary>
        /// Y轴回零起始速度
        /// </summary>
        [Description("Y轴回零起始速度|p/s")]
        public  int Home_Y_LSpeed { get; set; } = 0;

        /// <summary>
        /// Y轴回零驱动速度
        /// </summary>
        [Description("Y轴回零驱动速度|p/s")]
        public  int Home_Y_HSpeed { get; set; } = 5000;

        /// <summary>
        /// Y轴回零加速度
        /// </summary>
        [Description("Y轴回零加速度|p/s")]
        public  int Home_Y_USpeed { get; set; } = 100;

        /// <summary>
        /// Y轴回零爬行速度
        /// </summary>
        [Description("Y轴回零爬行速度|p/s")]
        public  int Home_Y_CSpeed { get; set; } = 1000;

        /// <summary>
        /// Z轴回零起始速度
        /// </summary>
        [Description("Z轴回零起始速度|p/s")]
        public  int Home_Z_LSpeed { get; set; } = 0;

        /// <summary>
        /// Z轴回零驱动速度
        /// </summary>
        [Description("Z轴回零驱动速度|p/s")]
        public  int Home_Z_HSpeed { get; set; } = 5000;

        /// <summary>
        /// Z轴回零加速度
        /// </summary>
        [Description("Z轴回零加速度|p/s")]
        public  int Home_Z_USpeed { get; set; } = 100000;

        

        /// <summary>
        /// Z轴回零爬行速度
        /// </summary>
        [Description("Z轴回零爬行速度|p/s")]
        public int Home_Z_CSpeed { get; set; } = 1000;

        #endregion

        #region 运动参数

        /// <summary>
        /// X轴运行起始速度
        /// </summary>
        [Description("X轴运行起始速度|p/s")]
        public  int Move_X_LSpeed { get; set; } = 0;

       

        /// <summary>
        /// Y轴运行起始速度
        /// </summary>
        [Description("Y轴运行起始速度|p/s")]
        public  int Move_Y_LSpeed { get; set; } = 0;

       

        /// <summary>
        /// 小针筒运行起始速度
        /// </summary>
        [Description("小针筒运行起始速度|p/s")]
        public  int Move_S_LSpeed { get; set; } = 0;

       

        /// <summary>
        /// 大针筒运行起始速度
        /// </summary>
        [Description("大针筒运行起始速度|p/s")]
        public  int Move_B_LSpeed { get; set; } = 0;
       
       

        #endregion

    }
}
