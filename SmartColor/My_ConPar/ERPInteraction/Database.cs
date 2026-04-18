using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.ERPInteraction
{
    internal class Database
    {
        /// <summary>
        /// IP地址
        /// </summary>
        [Description("IP地址")]
        public  string Server { get; set; } = "";

        /// <summary>
        /// 端口号
        /// </summary>
        [Description("端口号")]
        public  int Port { get; set; } = 0;

        /// <summary>
        /// 数据库名称
        /// </summary>
        [Description("数据库名称")]
        public  string DataBase { get; set; } = "";

        /// <summary>
        /// 用户名
        /// </summary>
        [Description("用户名")]
        public  string UserName { get; set; } = "";

        /// <summary>
        /// 密码
        /// </summary>
        [Description("密码")]
        public  string Password { get; set; } = "";

    }
}
