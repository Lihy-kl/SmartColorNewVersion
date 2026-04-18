using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Type.PLC
{
    /// <summary>
    /// PLC运动参数类
    /// </summary>
    internal class Motion : BaseMotion
    {
        public Motion()
        {
            Home_X_Offset = 0;
            Home_Y_Offset = 0;
            Home_Z_Offset = 1500;
            Move_X_HSpeed = 5000;
            Move_Y_HSpeed = 5000;
            Move_S_HSpeed = 25000;
            Move_S_MinHSpeed = 7500;
            Move_B_HSpeed = 7500;
            Move_B_MinHSpeed = 4000;

            Move_X_USpeed = 2500;
            Move_Y_USpeed = 2500;
            Move_S_USpeed = 30000;
            Move_B_USpeed = 10000;
        }


        /// <summary>
        /// 小针筒运行慢速加减
        /// </summary>
        [Description("小针筒运行慢速加减速|p/s²")]
        public int SMin_UDSpeed { get; set; } = 30000;

        /// <summary>
        /// 大针筒运行慢速加减
        /// </summary>
        [Description("大针筒运行慢速加减速|p/s²")]
        public int BMin_UDSpeed { get; set; } = 100;
    }
}
