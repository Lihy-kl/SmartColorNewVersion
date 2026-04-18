using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar
{
    /// <summary>
    /// 延时类
    /// </summary>
    internal class Delay
    {
        #region 时间参数

        /// <summary>
        /// 上下气缸检测延时|秒
        /// </summary>
        [Description("上下气缸检测延时|秒")]
        public static  double Cylinder { get; set; } = 5;

        /// <summary>
        /// 抓手检测延时|秒
        /// </summary>
        [Description("抓手检测延时|秒")]
        public static  double Tongs { get; set; }=2;

        /// <summary>
        /// 针筒检测延时|秒
        /// </summary>
        [Description("针筒检测延时|秒")]
        public static  double Syringe { get; set; }=2;

        /// <summary>
        /// 接液盘检测延时|秒
        /// </summary>
        [Description("接液盘检测延时|秒")]
        public static  double Tray { get; set; }=2;

        /// <summary>
        /// 泄压检测延时|秒
        /// </summary>
        [Description("泄压检测延时|秒")]
        public static  double Decompression { get; set; } = 2;

        /// <summary>
        /// 泄压时间|秒
        /// </summary>
        [Description("泄压时间|秒")]
        public static  double DecoTime { get; set; } = 5;

        /// <summary>
        /// 阻挡检测延时|秒
        /// </summary>
        [Description("阻挡检测延时|秒")]
        public static  double Block { get; set; }=2;

        /// <summary>
        /// 天平清零延时|秒
        /// </summary>
        [Description("天平清零延时|秒")]
        public static  double Balance_Reset { get; set; } = 1;

        /// <summary>
        /// 天平读数延时|秒
        /// </summary>
        [Description("天平读数延时|秒")]
        public static  double Balance_Read { get; set; }=1;

        /// <summary>
        /// 完成报警时间|秒
        /// </summary>
        [Description("完成报警时间|秒")]
        public static  double Buzzer_Finish { get; set; } = 2;

        /// <summary>
        /// 无人值守自动选择时间间隔|分钟
        /// </summary>
        [Description("无人值守自动选择时间间隔|分钟")]
        public static int AskTimes { get; set; } = 2;

        /// <summary>
        /// 温度采集间隔时间 |秒
        /// </summary>
        [Description("温度采集间隔时间 |秒")]
        public static int TemRecordInterval { get; set; } = 30;


        #endregion
    }
}
