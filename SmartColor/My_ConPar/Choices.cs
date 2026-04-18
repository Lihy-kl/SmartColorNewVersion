using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar
{
    /// <summary>
    /// 选项参数类
    /// </summary>
    internal class Choices
    {
        /// <summary>
        /// 出布时是否进行两次出布动作
        /// </summary>
        [Description("出布时是否进行两次出布动作|0：否  1：是")]
        public static int RepeatGetCloth { get; set; } = 0;

        /// <summary>
        /// 滴液是否检查母液瓶液量低
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("滴液是否检查母液瓶液量低|0：否  1：是")]
        public static int DripCheckLow { get; set; } = 1;

        /// <summary>
        /// 滴液是否检查母液瓶过期
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("滴液是否检查母液瓶过期|0：否  1：是")]
        public static int DripCheckExpired { get; set; } = 1;

        /// <summary>
        /// 液量低是否允许滴液
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("液量低是否允许滴液|0：否  1：是")]
        public static int DripAllowLow { get; set; } = 0;

        /// <summary>
        /// 母液瓶过期是否允许滴液
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("母液瓶过期是否允许滴液|0：否  1：是")]
        public static int DripAllowExpired { get; set; } = 0;

        /// <summary>
        /// 是否是满量程滴液
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("是否是满量程滴液|0：否  1：是")]
        public static int DripFull { get; set; } = 1;

        /// <summary>
        /// 开料完是否需要针检
        /// 0：不需要
        /// 1：需要
        /// </summary>
        [Description("开料完是否需要针检|0：不需要  1：需要")]
        public static int CutNeedCheck { get; set; } = 1;

        /// <summary>
        /// 开稀时的开料日期
        /// 0：原瓶号的开料日期
        /// 1：当前日期
        /// </summary>
        [Description("开稀时的开料日期|0：原瓶号的开料日期  1：当前日期")]
        public static int UseMotherDate { get; set; } = 0;

        /// <summary>
        /// 是否开启检测模式（滴液到废液桶）
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("是否开启检测模式（滴液到废液桶）|0：否  1：是")]
        public static int IsDebug { get; set; } = 0;

        /// <summary>
        /// 是否启动无人值守
        /// 0：关闭
        /// 1：开启
        /// </summary>
        [Description("是否启动无人值守|0：关闭  1：开启")]
        public static int UseAutoChoose { get; set; } = 1;

        /// <summary>
        /// 是否启动夹子自动放布
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("是否启动夹子自动放布|0：否  1：是")]
        public static int UseClamp { get; set; } = 0;

        /// <summary>
        /// 是否启动夹子自动出布
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("是否启动夹子自动出布|0：否  1：是")]
        public static int UseClampOut { get; set; } = 0;

        /// <summary>
        /// 是否自动启动滴液
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("是否自动启动滴液|0：否  1：是")]
        public static int UseAutoDrip { get; set; } = 1;

        /// <summary>
        /// 开完料后是否启动自动吸光度检测
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("开完料后是否启动自动吸光度检测|0：否  1：是")]
        public static int AutoAbs { get; set; } = 0;

        /// <summary>
        /// 是否修正数据
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("是否修正数据 |0：否  1：是")]
        public static int CorrectData { get; set; } = 0;

        /// <summary>
        /// 加水是否复检
        /// 0：复检
        /// 1：不复检
        /// </summary>
        [Description("加水是否复检 |0：复检  1：不复检")]
        public static int WaterRecheck { get; set; } = 0;

        /// <summary>
        /// 配方界面上的浴比更改后 底下的所有工艺浴比跟着变化
        /// </summary>
        [Description("配方界面上的浴比更改后 底下的所有工艺浴比跟着变化|0：否  1：是")]
        public static int BathRatioTxtDyBath { get; set; } = 0;

        // 在Choices类中添加
        public static event EventHandler HighWashChanged;

        /// <summary>
        /// 是否使用高温洗杯
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("是否使用高温洗杯|0：否  1：是")]
        public static int HighWash
        {
            get => _highWash;
            set
            {
                if (_highWash != value)
                {
                    _highWash = value;
                    HighWashChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }
        private static int _highWash = 0; // 用私有字段存储

        /// <summary>
        /// 是否屏蔽针筒感应器
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("是否屏蔽针筒感应器|0：否  1：是")]
        public static int IgnoreSyringeSensor { get; set; } = 0;

        /// <summary>
        /// 是否启动空闲时自动校正
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("是否启动空闲时自动校正|0：否  1：是")]
        public static int UseAutoCheck { get; set; } = 0;

        /// 是否启动空闲时自动洗针筒
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("是否启动空闲时自动洗针筒|0：否  1：是")]
        public static int UseAutoWashSyringe { get; set; } = 0;

        /// 是否启动自动更新杯子坐标
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("是否启动自动更新杯子坐标|0：否  1：是")]
        public static int UseAutoUpdateCupCoor { get; set; } = 1;

        /// 是否使用限值功能
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("是否使用限值功能|0：否  1：是")]
        public static int UseLimit { get; set; } = 1;

        /// 是否使用拉高抽液
        /// 0：否
        /// 1：是
        /// </summary>
        [Description("是否使用拉高抽液|0：否  1：是")]
        public static int UseHighLiftAspiration { get; set; } = 0;
    }
}
