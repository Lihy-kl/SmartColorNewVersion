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
    /// 16杯翻转缸参数
    /// </summary>
    internal class FC_16 : FC_12
    {
        public FC_16()
        {
            AreaName = "16杯翻转缸";
            AreaType = 5;
            Row = 8;
            Column = 2;
            Vertical = 1;
        }

        /// <summary>
        /// 13号杯杯心X坐标
        /// </summary>
       
        [Description("13号杯杯心X坐标")]
        public int CupCX_13 { get; set; } = 0;

        /// <summary>
        /// 13号杯杯心Y坐标
        /// </summary>
       
        [Description("13号杯杯心Y坐标")]
        public int CupCY_13 { get; set; } = 0;

        /// <summary>
        /// 13号杯杯盖中心X坐标
        /// </summary>
       
        [Description("13号杯杯盖中心X坐标")]
        public int LidCX_13 { get; set; } = 0;

        /// <summary>
        /// 13号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("13号杯杯盖中心Y坐标")]
        public int LidCY_13 { get; set; } = 0;

        /// <summary>
        /// 14号杯杯心X坐标
        /// </summary>
       
        [Description("14号杯杯心X坐标")]
        public int CupCX_14 { get; set; } = 0;

        /// <summary>
        /// 14号杯杯心Y坐标
        /// </summary>
       
        [Description("14号杯杯心Y坐标")]
        public int CupCY_14 { get; set; } = 0;

        /// <summary>
        /// 14号杯杯盖中心X坐标
        /// </summary>
       
        [Description("14号杯杯盖中心X坐标")]
        public int LidCX_14 { get; set; } = 0;

        /// <summary>
        /// 14号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("14号杯杯盖中心Y坐标")]
        public int LidCY_14 { get; set; } = 0;

        /// <summary>
        /// 15号杯杯心X坐标
        /// </summary>
       
        [Description("15号杯杯心X坐标")]
        public int CupCX_15 { get; set; } = 0;

        /// <summary>
        /// 15号杯杯心Y坐标
        /// </summary>
       
        [Description("15号杯杯心Y坐标")]
        public int CupCY_15 { get; set; } = 0;

        /// <summary>
        /// 15号杯杯盖中心X坐标
        /// </summary>
       
        [Description("15号杯杯盖中心X坐标")]
        public int LidCX_15 { get; set; } = 0;

        /// <summary>
        /// 15号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("15号杯杯盖中心Y坐标")]
        public int LidCY_15 { get; set; } = 0;

        /// <summary>
        /// 16号杯杯心X坐标
        /// </summary>
       
        [Description("16号杯杯心X坐标")]
        public int CupCX_16 { get; set; } = 0;

        /// <summary>
        /// 16号杯杯心Y坐标
        /// </summary>
       
        [Description("16号杯杯心Y坐标")]
        public int CupCY_16 { get; set; } = 0;

        /// <summary>
        /// 16号杯杯盖中心X坐标
        /// </summary>
       
        [Description("16号杯杯盖中心X坐标")]
        public int LidCX_16 { get; set; } = 0;

        /// <summary>
        /// 16号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("16号杯杯盖中心Y坐标")]
        public int LidCY_16 { get; set; } = 0;
    }
}
