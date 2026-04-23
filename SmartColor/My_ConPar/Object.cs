using SmartColor.My_ADT8940A1;
using SmartColor.My_PLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SmartColor.My_ConPar
{
    internal class Object
    {
        /// <summary>
        /// 布局参数类型
        /// </summary>
        public static  dynamic CurrentLayout = null;

        /// <summary>
        /// 天平配置
        /// </summary>
        public static  Area.Balance.Balance CurrentBalance = null;

        /// <summary>
        /// 机台配置
        /// </summary>
        public static  dynamic CurrentMachine = null;

        /// <summary>
        /// 开料机配置
        /// </summary>
        public static  dynamic CurrentCutting = null;

        /// <summary>
        /// 开料机对象
        /// </summary>
        public static object CurrentCuttingObj = null;

        /// <summary>
        /// ERP配置
        /// </summary>
        public static  dynamic CurrentERP = null;

        /// <summary>
        /// 运动配置
        /// </summary>
        public static  dynamic CurrentMotion = null;

        /// <summary>
        /// PLC对象（单例）
        /// </summary>
        public static  PLC CurrentPLC = null;

        /// <summary>
        /// 板块对象（单例）
        /// </summary>
        public static ADT8940A1 CurrentADT8940A1 = null;

        /// <summary>
        /// 串口天平对象
        /// </summary>
        public static dynamic Balance = null;

    }
}
