using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar
{
    /// <summary>
    /// 其他类
    /// </summary>
    internal class Other
    {
        /// <summary>
        /// 校正允许误差
        /// </summary>
        [Description("校正允许误差|g")]
        public static  double CorrectingAErr { get; set; } = 0.02;

        /// <summary>
        /// 滴液允许误差
        /// </summary>
        [Description("滴液染料允许误差|g")]
        public static  double DyeAErr { get; set; } = 0.02;

        /// <summary>
        /// 助剂滴液允许误差
        /// </summary>
        [Description("滴液助剂允许误差|g")]
        public static  double AssAErr { get; set; } = 0.02;

        /// <summary>
        /// 加水允许误差
        /// </summary>
        [Description("加水允许误差|%")]
        public static  double AErr_Water { get; set; } = 5;

        /// <summary>
        /// 母液瓶库存量报警值
        /// </summary>
        [Description("母液瓶库存量报警值|g")]
        public static  double Bottle_AlarmWeight { get; set; } = 250;

        /// <summary>
        /// 母液瓶库存量最低值
        /// </summary>
        [Description("母液瓶库存量最低值|g")]
        public static  double Bottle_MinWeight { get; set; } = 200;

        /// <summary>
        /// 大针筒Z轴最大脉冲
        /// </summary>
        [Description("大针筒Z轴最大脉冲|p")]
        public static  int B_MaxPulse { get; set; } = 45000;

        /// <summary>
        /// 小针筒Z轴最大脉冲
        /// </summary>
        [Description("小针筒Z轴最大脉冲|p")]
        public static  int S_MaxPulse { get; set; } = 95000;

        /// <summary>
        /// 排空Z轴上移脉冲
        /// </summary>
        [Description("排空Z轴上移脉冲|p")]
        public static  int Z_UpPulse { get; set; } = 30000;

        /// <summary>
        /// 排空Z轴下压脉冲
        /// </summary>
        [Description("排空Z轴下压脉冲|p")]
        public static  int Z_DownPulse { get; set; } = -650;

        /// <summary>
        /// 抽液完成反推脉冲
        /// </summary>
        [Description("抽液完成反推脉冲|p")]
        public static  int Z_BackPulse { get; set; } = -5000;

        /// <summary>
        /// 废液桶最大重量
        /// </summary>
        [Description("废液桶最大重量|g")]
        public static  double BalanceMaxWeight { get; set; } = 1500;

        /// <summary>
        /// 纯滴液杯位最大重量
        /// </summary>
        [Description("纯滴液杯位最大重量|g")]
        public static  double DripMaxWeight { get; set; } = 400;

        /// <summary>
        /// 染色机杯位最大重量
        /// </summary>
        [Description("染色机杯位最大重量|g")]
        public static  double HandleMaxWeight { get; set; } = 300;

        /// <summary>
        /// 染色机杯位最大重量(大杯)
        /// </summary>
        [Description("染色机杯位最大重量(大杯)|g")]
        public static  double HandleMaxWeight_Big { get; set; } = 600;

        /// <summary>
        /// 输入染料配方用量报警值
        /// </summary>
        [Description("输入染料配方用量报警值")]
        public static  double DyeAlarmWeight { get; set; } = 10;

        /// <summary>
        /// 输入助剂配方用量报警值
        /// </summary>
        [Description("输入助剂配方用量报警值")]
        public static  double AdditivesAlarmWeight { get; set; } = 120;

        /// <summary>
        /// 默认湿布水比
        /// </summary>
        [Description("默认湿布水比")]
        public static  double Default_Non_AnhydrationWR { get; set; } = 2;

        /// <summary>
        /// 默认干布水比
        /// </summary>
        [Description("默认干布水比")]
        public static  double Default_AnhydrationWR { get; set; } = 1;

        /// <summary>
        /// 输入布重报警值
        /// </summary>
        [Description("输入布重报警值|g")]
        public static  double ClothAlarmWeight { get; set; } = 100;

        /// <summary>
        /// 天平稳定读数判断值
        /// </summary>
        [Description("天平稳定读数判断值|g")]
        public static  double Stable_Value { get; set; } = 0;

        /// <summary>
        /// 合夹夹布脉冲
        /// </summary>
        [Description("合夹夹布脉冲|p")]
        public static  int ClosePulse { get; set; } = -500;

        /// <summary>
        /// 开夹放布脉冲
        /// </summary>
        [Description("开夹放布脉冲|p")]
        public static  int OpenPulse { get; set; } = 8500;

        /// <summary>
        /// 备布区下探开夹脉冲
        /// </summary>
        [Description("备布区下探开夹脉冲|p")]
        public static  int ClothDownPulse { get; set; } = 7000;

        /// <summary>
        /// 染杯口下探开夹脉冲
        /// </summary>
        [Description("染杯口下探开夹脉冲|p")]
        public static  int CupDownPulse { get; set; } = 8500;

        /// <summary>
        /// 出布区放布时气缸位置 0阻挡位 1慢速中
        /// </summary>
        [Description("出布区放布时气缸位置|0:阻挡位 1:慢速中")]
        public static  int OutClothPosition { get; set; } = 0;

        /// <summary>
        /// 备布区取布时气缸位置 0慢速中 1慢速2
        /// </summary>
        [Description("备布区取布时气缸位置|0:慢速中 2:慢速2")]
        public static int PutClothPosition { get; set; } = 0;

        /// <summary>
        /// 洗针次数
        /// </summary>
        [Description("洗针次数|次数")]
        public static  int WashTime { get; set; } = 2;

        /// <summary>
        /// 排空次数
        /// </summary>
        [Description("排空次数|次数")]
        public static  int DrainCount { get; set; } = 1;

        /// <summary>
        /// 撑盖Y轴位置偏移
        /// </summary>
        [Description("撑盖Y轴位置偏移|p")]
        public static  int SupportCoverY { get; set; } = 2500;              

        /// <summary>
        /// 滴液时需加量最小值 |g
        /// </summary>
        [Description("滴液时需加量最小值 |g")]
        public static  double AlarmDropWeight { get; set; } = 0.1;

        /// <summary>
        /// 前光幕生效Y轴坐标 |p
        /// </summary>
        [Description("前光幕生效Y轴坐标 |p")]
        public static  int Sunx_Stop_Y { get; set; } = 0;

        /// <summary>
        /// 后光幕生效Y轴坐标 |p
        /// </summary>
        [Description("后光幕生效Y轴坐标 |p")]
        public static  int Sunx_Back_Y { get; set; } = 0;

        /// <summary>
        /// 开料范围下限 |%
        /// </summary>
        [Description("开料范围下限 |%")]
        public static  int BrewBottleWeightMin { get; set; } = 30;

        /// <summary>
        /// 开料范围上限 |%
        /// </summary>
        [Description("开料范围上限 |%")]
        public static  int BrewBottleWeightMax { get; set; } = 100;

        /// <summary>
        /// 待机位X轴坐标 |p
        /// </summary>
        [Description("待机位X轴坐标 |p")]
        public static  int StandbyX { get; set; } = 0;

        /// <summary>
        /// 待机位Y轴坐标 |p
        /// </summary>
        [Description("待机位Y轴坐标 |p")]
        public static  int StandbyY { get; set; } = 0;

        /// <summary>
        /// 气缸高度偏差脉冲 |p
        /// </summary>
        [Description("气缸高度偏差脉冲 |p")]
        public static int CylinderHeightDeviationPulse { get; set; } = 2360;

        /// <summary>
        /// 气缸中位置 |mm
        /// </summary>
        [Description("气缸中位置 |mm")]
        public static int PositionInCylinder { get; set; } = 300;

        /// <summary>
        /// 气缸慢速中1位置 |mm
        /// </summary>
        [Description("气缸慢速中1位置 |mm")]
        public static int PositionInCylinderSlow1 { get; set; } = 250;

        /// <summary>
        /// 气缸慢速中2位置 |mm
        /// </summary>
        [Description("气缸慢速中2位置 |mm")]
        public static int PositionInCylinderSlow2 { get; set; } = 200;

        /// <summary>
        /// 气缸慢速中3位置 |mm
        /// </summary>
        [Description("气缸慢速中3位置 |mm")]
        public static int PositionInCylinderSlow3 { get; set; } = 168;

        /// <summary>
        /// 气缸到阻挡位位置 |mm
        /// </summary>
        [Description("气缸到阻挡位位置|mm")]
        public static int CylinderStopPosition { get; set; } = 350;

        /// <summary>
        /// 气缸下位置 |mm
        /// </summary>
        [Description("气缸下位置|mm")]
      
        public static int CylinderDownPosition { get; set; } = 453;

        /// <summary>
        /// 气缸定位范围 |mm
        /// </summary>
        [Description("气缸定位范围|mm")]

        public static int CylinderPositioningRange { get; set; } = 15;
    }
}
