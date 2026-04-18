using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Area.RotorCylinder
{
    /// <summary>
    /// 10杯转子缸参数
    /// </summary>
    internal class RC_10 : RC_4
    {
        public RC_10()
        {
            AreaName = "10杯转子缸";
            AreaType = 7;
            Row = 5;
            Column = 2;
        }

        /// <summary>
        /// 5号杯杯心X坐标
        /// </summary>

        [Description("5号杯杯心X坐标")]
        public int CupCX_5 { get; set; } = 0;

        /// <summary>
        /// 5号杯杯心Y坐标
        /// </summary>

        [Description("5号杯杯心Y坐标")]
        public int CupCY_5 { get; set; } = 0;

        /// <summary>
        /// 6号杯杯心X坐标
        /// </summary>

        [Description("6号杯杯心X坐标")]
        public int CupCX_6 { get; set; } = 0;

        /// <summary>
        /// 6号杯杯心Y坐标
        /// </summary>

        [Description("6号杯杯心Y坐标")]
        public int CupCY_6 { get; set; } = 0;

        /// <summary>
        /// 7号杯杯心X坐标
        /// </summary>

        [Description("7号杯杯心X坐标")]
        public int CupCX_7 { get; set; } = 0;

        /// <summary>
        /// 7号杯杯心Y坐标
        /// </summary>

        [Description("7号杯杯心Y坐标")]
        public int CupCY_7 { get; set; } = 0;

        /// <summary>
        /// 8号杯杯心X坐标
        /// </summary>

        [Description("8号杯杯心X坐标")]
        public int CupCX_8 { get; set; } = 0;

        /// <summary>
        /// 8号杯杯心Y坐标
        /// </summary>

        [Description("8号杯杯心Y坐标")]
        public int CupCY_8 { get; set; } = 0;

        /// <summary>
        /// 9号杯杯心X坐标
        /// </summary>

        [Description("9号杯杯心X坐标")]
        public int CupCX_9 { get; set; } = 0;

        /// <summary>
        /// 9号杯杯心Y坐标
        /// </summary>

        [Description("9号杯杯心Y坐标")]
        public int CupCY_9 { get; set; } = 0;

        /// <summary>
        /// 10号杯杯心X坐标
        /// </summary>

        [Description("10号杯杯心X坐标")]
        public int CupCX_10 { get; set; } = 0;

        /// <summary>
        /// 10号杯杯心Y坐标
        /// </summary>

        [Description("10号杯杯心Y坐标")]
        public int CupCY_10 { get; set; } = 0;

        /// <summary>
        /// 泄压X坐标偏移
        /// </summary>

        [Description("泄压X坐标偏移")]
        public int DecompressionOffsetX { get; set; } = 0;

        /// <summary>
        /// 泄压Y坐标偏移
        /// </summary>

        [Description("泄压Y坐标偏移")]
        public int DecompressionOffsetY { get; set; } = 0;

        /// <summary>
        /// IP地址
        /// </summary>

        [Description("IP地址")]
        public string IP { get; set; } = "192.168.68.21";

        /// <summary>
        /// 端口号
        /// </summary>

        [Description("端口号")]
        public int Port { get; set; } = 502;
    }
}
