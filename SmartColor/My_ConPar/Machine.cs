using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SmartColor.My_ConPar
{
    /// <summary>
    /// 机台硬件配置
    /// </summary>
    public class Machine
    {
        /// <summary>
        /// 机台ID
        /// </summary>
        [Description("机台ID")]
        public static  int ID { get; set; } = 1;

        /// <summary>
        /// 机台布局
        /// 0：常规滴液机布局
        /// 1：常规打版机布局
        /// 2：全自动打板机布局
        /// </summary>
        [Description("机台布局 |0：常规滴液机布局  1：常规打版机布局  2：滴液打板机布局 3:全自动打板机布局")]
        public static  int MachineLayout { get; set; } = 0;

        /// <summary>
        /// 机台类型
        /// 0：PLC版机台
        /// 1：板卡版机台
        /// 2：脱机版
        /// </summary>
        [Description("机台类型 |0：PLC版机台  1：板卡版机台  2：脱机版")]
        public static  int MachineType { get; set; } = 0;

        /// <summary>
        /// 开料机类型
        /// 0：无
        /// 1：威伦触摸屏(TCP)
        /// 2：汇川PLC(TCP)
        /// 3：威伦触摸屏(485)
        /// 4：台达触摸屏(485)
        /// 5：自动开料
        /// </summary>
        [Description("开料机类型 |0：无  1：威伦触摸屏(TCP)  2：汇川PLC(TCP)  3：威伦触摸屏(485)  4：台达触摸屏(485)  5:自动开料 ")]
        public static  int CuttingMachine { get; set; } = 1;

        /// <summary>
        /// 是否开启日志
        /// 0：不开启
        /// 1：开启
        /// </summary>
        [Description("是否开启日志 |0：不开启  1：开启")]
        public static  int OpenLog { get; set; } = 0;

        /// <summary>
        /// 是否有吸光度机
        /// 0：无
        /// 1：有
        /// </summary>
        [Description("是否有吸光度机 |0：无  1：有")]
        public static  int UseAbs { get; set; } = 0;

        /// <summary>
        /// 是否配有称布系统
        /// 0：无
        /// 1：有
        /// </summary>
        [Description("是否配有称布系统 |0：无  1：有")]
        public static  int UseCloth { get; set; } = 0;

        /// <summary>
        /// 是否配有称粉机
        /// 0：无
        /// 1：单头称粉机
        /// 2：双头称粉机
        /// </summary>
        [Description("是否配有称粉机 |0：无  1：单头称粉机  2：双头称粉机")]
        public static  int UsePowder { get; set; } = 0;
       
        /// <summary>
        /// ERP交互方式
        /// 0：无
        /// 1：TXT交互
        /// 2：数据库交互
        /// </summary>
        [Description("ERP交互方式 |0：无  1：TXT交互  2：数据库交互")]
        public static  int ERPInteraction { get; set; } = 0;

        /// <summary>
        /// 语言
        /// zh - 中文
        /// en - English
        /// </summary>
        [Description("语言|0：中文  1：English")]
        public static  int Language { get; set; } = 0;

        /// <summary>
        /// 注册时的注册方式
        /// 0：脱机注册
        /// 1：联网注册
        /// </summary>
        [Description("注册时是否使用脱机注册方式|0：脱机注册  1：联网注册")]
        public static  int RegisterWay { get; set; } = 1;

        /// <summary>
        /// 是否启用网络推送
        /// 0：不启用
        /// 1：启用
        /// </summary>
        [Description("是否启用网络推送|0：不启用  1：启用")]
        public static  int NetWork { get; set; } = 0;
    }
}