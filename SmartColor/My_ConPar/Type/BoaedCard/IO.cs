using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Type.BoaedCard
{
    internal class IO : BaseIO
    {
        /// <summary>
        /// 板卡IO
        /// </summary>
        public IO()
        {
            #region 输入
           
           
           
            InPut_Sunx_Stop = "3";
           
           
            
            InPut_Tray_Out = "11";
            InPut_Z_Reverse = "12";
           
            InPut_Tray_In = "22";
            InPut_Syringe = "15";
            InPut_Tongs_A = "18";
            InPut_Tongs_B = "20";
            InPut_Cylinder_Up = "16";
            InPut_Cylinder_Down = "17";
            InPut_Sunx_A = "27";
            InPut_Sunx_B = "29";
            InPut_Cylinder_Mid = "4";
            InPut_Decompression_Down = "30";
            InPut_Decompression_Up = "31";
            InPut_Sunx_Back = "107";
            InPut_Block_Out = "23";
            InPut_Block_In = "21";
            InPut_Slow_Cylinder_Mid = "28";
            InPut_Cylinder_Block = "9";
            InPut_SupportCover = "26";
           
            #endregion

            #region 输出

            OutPut_Slow_Cylinder = "13";
            OutPut_Blender = "11";
            OutPut_Waste = "8";
            OutPut_Buzzer = "2";
            OutPut_Water = "3";
            OutPut_Tray = "7";
            OutPut_TongsOff = "5";
            OutPut_Cylinder_Down = "10";
            OutPut_Cylinder_Up = "9";
            OutPut_TongsOn = "6";
            OutPut_Block_Out = "12";
            OutPut_Block_In = "0";
            OutPut_Decompression = "4";
            OutPut_Wash_In = "-1";
            OutPut_Wash_Out = "-1";
            OutPut_Wash_Blow = "1";
          
            #endregion
        }

        /// <summary>
        /// X轴反限位
        /// </summary>
        [Description("X轴反限位|输入")]
        public int InPut_X_Reverse { get; set; } = 0;

        /// <summary>
        /// X轴正限位
        /// </summary>
        [Description("X轴正限位|输入")]
        public int InPut_X_Corotation { get; set; } = 1;

        /// <summary>
        /// X轴原点
        /// </summary>
        [Description("X轴原点|输入")]
        public int InPut_X_Origin { get; set; } = 2;

        /// <summary>
        /// Y轴反限位
        /// </summary>
        [Description("Y轴反限位|输入")]
        public int InPut_Y_Reverse { get; set; } = 6;

        /// <summary>
        /// Y轴正限位
        /// </summary>
        [Description("Y轴正限位|输入")]
        public int InPut_Y_Corotation { get; set; } = 7;

        /// <summary>
        /// Y轴原点
        /// </summary>
        [Description("Y轴原点|输入")]
        public int InPut_Y_Origin { get; set; } = 8;


        /// <summary>
        /// Z轴原点
        /// </summary>
        [Description("Z轴原点|输入")]
        public int InPut_Z_Origin { get; set; } = 14;

        /// <summary>
        /// X轴报警信号
        /// </summary>
        [Description("X轴报警信号|输入")]
        public int InPut_X_Alarm { get; set; } = 25;

        /// <summary>
        /// Y轴报警
        /// </summary>
        [Description("Y轴报警|输入")]
        public int InPut_Y_Alarm { get; set; } = 10;

        /// <summary>
        /// Y轴准备
        /// </summary>
        [Description("Y轴准备|输入")]
        public int InPut_Y_Ready { get; set; } = 5;

        /// <summary>
        /// X轴准备
        /// </summary>
        [Description("X轴准备|输入")]
        public int InPut_X_Ready { get; set; } = 24;

        /// <summary>
        /// Y轴矢能
        /// </summary>
        [Description("Y轴矢能|输出")]
        public int OutPut_Y_Power { get; set; } = 14;

        /// <summary>
        /// X轴矢能
        /// </summary>
        [Description("X轴矢能|输出")]
        public int OutPut_X_Power { get; set; } = 15;

        #region 轴

        /// <summary>
        /// X轴
        /// </summary>
        [Description("X轴|轴号")]
        public int Axis_X { get; set; } = 1;

        /// <summary>
        /// Y轴
        /// </summary>
        [Description("Y轴|轴号")]
        public int Axis_Y { get; set; } = 2;

        /// <summary>
        /// Z轴
        /// </summary>
        [Description("Z轴|轴号")]
        public int Axis_Z { get; set; } = 3;

        #endregion

    }
}
