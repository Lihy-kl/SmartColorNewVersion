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
    /// 6杯翻转缸参数
    /// </summary>
    internal class FC_6 : FC_4
    {
        public FC_6()
        {
            AreaName = "6杯翻转缸";
            AreaType = 3;
            Row = 6;
            Column = 1;
            Vertical = 1;
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
        /// 5号杯杯盖中心X坐标
        /// </summary>
       
        [Description("5号杯杯盖中心X坐标")]
        public int LidCX_5 { get; set; } = 0;

        /// <summary>
        /// 5号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("5号杯杯盖中心Y坐标")]
        public int LidCY_5 { get; set; } = 0;

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
        /// 6号杯杯盖中心X坐标
        /// </summary>
       
        [Description("6号杯杯盖中心X坐标")]
        public int LidCX_6 { get; set; } = 0;

        /// <summary>
        /// 6号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("6号杯杯盖中心Y坐标")]
        public int LidCY_6 { get; set; } = 0;
    }
}
