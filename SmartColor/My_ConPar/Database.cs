using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SmartColor.My_ConPar
{
    /// <summary>
    /// 数据库类
    /// </summary>
    internal class Database
    {
        /// <summary>
        /// IP地址，默认本地(local)
        /// </summary>
        [Description("IP地址")]
        public static  string Server { get; set; } = "(local)";

        /// <summary>
        /// 端口号，默认1433
        /// </summary>
        [Description("端口号")]
        public static  int Port { get; set; } = 1433;

        /// <summary>
        /// 数据库名称，默认drop_system
        /// </summary>
        [Description("数据库名称")]
        public static  string DataBase { get; set; } = "drop_system";

        /// <summary>
        /// 用户名，默认sa
        /// </summary>
        [Description("用户名")]
        public static  string UserName { get; set; } = "sa";

        /// <summary>
        /// 密码，默认123456
        /// </summary>
        [Description("密码")]
        public static  string Password { get; set; } = "123456";
    }
}
