using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Area.FlipCylinder
{
    /// <summary>
    /// 12杯翻转缸参数
    /// </summary>
    internal class FC_12 : FC_6
    {
        public FC_12()
        {
            AreaName = "12杯翻转缸";
            AreaType = 4;
            Row = 6;
            Column = 2;
            Vertical = 1;
        }

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
        /// 7号杯杯盖中心X坐标
        /// </summary>
       
        [Description("7号杯杯盖中心X坐标")]
        public int LidCX_7 { get; set; } = 0;

        /// <summary>
        /// 7号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("7号杯杯盖中心Y坐标")]
        public int LidCY_7 { get; set; } = 0;

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
        /// 8号杯杯盖中心X坐标
        /// </summary>
       
        [Description("8号杯杯盖中心X坐标")]
        public int LidCX_8 { get; set; } = 0;

        /// <summary>
        /// 8号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("8号杯杯盖中心Y坐标")]
        public int LidCY_8 { get; set; } = 0;

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
        /// 9号杯杯盖中心X坐标
        /// </summary>
       
        [Description("9号杯杯盖中心X坐标")]
        public int LidCX_9 { get; set; } = 0;

        /// <summary>
        /// 9号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("9号杯杯盖中心Y坐标")]
        public int LidCY_9 { get; set; } = 0;

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
        /// 10号杯杯盖中心X坐标
        /// </summary>
       
        [Description("10号杯杯盖中心X坐标")]
        public int LidCX_10 { get; set; } = 0;

        /// <summary>
        /// 10号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("10号杯杯盖中心Y坐标")]
        public int LidCY_10 { get; set; } = 0;

        /// <summary>
        /// 11号杯杯心X坐标
        /// </summary>
       
        [Description("11号杯杯心X坐标")]
        public int CupCX_11 { get; set; } = 0;

        /// <summary>
        /// 11号杯杯心Y坐标
        /// </summary>
       
        [Description("11号杯杯心Y坐标")]
        public int CupCY_11 { get; set; } = 0;

        /// <summary>
        /// 11号杯杯盖中心X坐标
        /// </summary>
       
        [Description("11号杯杯盖中心X坐标")]
        public int LidCX_11 { get; set; } = 0;

        /// <summary>
        /// 11号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("11号杯杯盖中心Y坐标")]
        public int LidCY_11 { get; set; } = 0;

        /// <summary>
        /// 12号杯杯心X坐标
        /// </summary>
       
        [Description("12号杯杯心X坐标")]
        public int CupCX_12 { get; set; } = 0;

        /// <summary>
        /// 12号杯杯心Y坐标
        /// </summary>
       
        [Description("12号杯杯心Y坐标")]
        public int CupCY_12 { get; set; } = 0;

        /// <summary>
        /// 12号杯杯盖中心X坐标
        /// </summary>
       
        [Description("12号杯杯盖中心X坐标")]
        public int LidCX_12 { get; set; } = 0;

        /// <summary>
        /// 12号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("12号杯杯盖中心Y坐标")]
        public int LidCY_12 { get; set; } = 0;

        /// <summary>
        /// HMI类型
        /// 0：威纶通
        /// 1: 昆仑通态
        /// </summary>

        [Description("HMI类型|0：威纶通 1: 昆仑通态")]
        public int HMIType { get; set; } = 0;

    }
}
