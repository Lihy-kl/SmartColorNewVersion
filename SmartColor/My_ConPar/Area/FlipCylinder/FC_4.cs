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
    /// 4杯大翻转缸参数
    /// </summary>
    internal class FC_4:Base
    {

        public FC_4()
        {
            AreaName = "4杯大翻转缸";
            AreaType = 2;
          
        }


        /// <summary>
        /// 行数
        /// </summary>
       
        [Description("行数")]
        public int Row { get; set; } = 4;

        /// <summary>
        /// 列数
        /// </summary>
       
        [Description("列数")]
        public int Column { get; set; } = 1;

        /// <summary>
        /// 开始杯号
        /// </summary>
       
        [Description("开始杯号")]
        public int StartPosition { get; set; } = 1;

        /// <summary>
        /// 顺序编号方式
        /// 0：横着编号
        /// 1：竖着编号
        /// </summary>
       
        [Description("顺序编号方式|0：横着编号  1：竖着编号")]
        public int Vertical { get; set; } = 1;

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

        /// <summary>
        /// 1号杯杯心X坐标
        /// </summary>
       
        [Description("1号杯杯心X坐标")]
        public int CupCX_1 { get; set; } = 0;

        /// <summary>
        /// 1号杯杯心Y坐标
        /// </summary>
       
        [Description("1号杯杯心Y坐标")]
        public int CupCY_1 { get; set; } = 0;

        /// <summary>
        /// 1号杯杯盖中心X坐标
        /// </summary>
       
        [Description("1号杯杯盖中心X坐标")]
        public int LidCX_1 { get; set; } = 0;

        /// <summary>
        /// 1号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("1号杯杯盖中心Y坐标")]
        public int LidCY_1 { get; set; } = 0;

        /// <summary>
        /// 2号杯杯心X坐标
        /// </summary>
       
        [Description("2号杯杯心X坐标")]
        public int CupCX_2 { get; set; } = 0;

        /// <summary>
        /// 2号杯杯心Y坐标
        /// </summary>
       
        [Description("2号杯杯心Y坐标")]
        public int CupCY_2 { get; set; } = 0;

        /// <summary>
        /// 2号杯杯盖中心X坐标
        /// </summary>
       
        [Description("2号杯杯盖中心X坐标")]
        public int LidCX_2 { get; set; } = 0;

        /// <summary>
        /// 2号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("2号杯杯盖中心Y坐标")]
        public int LidCY_2 { get; set; } = 0;

        /// <summary>
        /// 3号杯杯心X坐标
        /// </summary>
       
        [Description("3号杯杯心X坐标")]
        public int CupCX_3 { get; set; } = 0;

        /// <summary>
        /// 3号杯杯心Y坐标
        /// </summary>
       
        [Description("3号杯杯心Y坐标")]
        public int CupCY_3 { get; set; } = 0;

        /// <summary>
        /// 3号杯杯盖中心X坐标
        /// </summary>
       
        [Description("3号杯杯盖中心X坐标")]
        public int LidCX_3 { get; set; } = 0;

        /// <summary>
        /// 3号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("3号杯杯盖中心Y坐标")]
        public int LidCY_3 { get; set; } = 0;

        /// <summary>
        /// 4号杯杯心X坐标
        /// </summary>
       
        [Description("4号杯杯心X坐标")]
        public int CupCX_4 { get; set; } = 0;

        /// <summary>
        /// 4号杯杯心Y坐标
        /// </summary>
       
        [Description("4号杯杯心Y坐标")]
        public int CupCY_4 { get; set; } = 0;

        /// <summary>
        /// 4号杯杯盖中心X坐标
        /// </summary>
       
        [Description("4号杯杯盖中心X坐标")]
        public int LidCX_4 { get; set; } = 0;

        /// <summary>
        /// 4号杯杯盖中心Y坐标
        /// </summary>
       
        [Description("4号杯杯盖中心Y坐标")]
        public int LidCY_4 { get; set; } = 0;

    }
}
