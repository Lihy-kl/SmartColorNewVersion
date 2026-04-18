using System;
using System.ComponentModel;

namespace SmartColor.My_ConPar.Order
{
    /// <summary>
    /// 机械手进程参数
    /// </summary>
    public class BigProcess
    {
        /// <summary>
        /// 机械手主流程业务类型枚举
        /// </summary>
        public enum RobotBusinessType
        {
            /// <summary>
            /// 滴液进程
            /// </summary>
            DropProcess = 0,

            /// <summary>
            /// 染色进程
            /// </summary>
            DyeingProcess = 1,

            /// <summary>
            /// 后处理进程
            /// </summary>
            PostProcess = 2,

            /// <summary>
            /// 吸光度进程
            /// </summary>
            UVProcess = 3,

            /// <summary>
            /// 自动开料进程
            /// </summary>
            BrewProcess = 4,

            /// <summary>
            /// 水校正进程
            /// </summary>
            WaterCorrection = 5,

            /// <summary>
            /// 水验证进程
            /// </summary>
            VerifyAddWater = 6,

            /// <summary>
            /// 溶解剂校正进程
            /// </summary>
            DMFCorrection = 7,

            /// <summary>
            /// 溶解剂验证进程
            /// </summary>
            VerifyAddDMF = 8,

            /// <summary>
            /// 母液瓶校正进程
            /// </summary>
            BottleCorrection = 9,

            /// <summary>
            /// 母液瓶自检进程
            /// </summary>
            BottleSelf = 10,

            /// <summary>
            /// 洗针进程
            /// </summary>
            WashSyringe = 11,

            /// <summary>
            /// 调机进程
            /// </summary>
            Debug = 12
        }


        // 默认值集中管理
        public static readonly int Default_WaterCorrection = 1;
        public static readonly int Default_DMFCorrection = 2;
        public static readonly int Default_DyeingProcess = 3;
        public static readonly int Default_DropProcess = 4;
        public static readonly int Default_PostProcess = 5;
        public static readonly int Default_UVProcess = 6;
        public static readonly int Default_BrewProcess = 7;
        public static readonly int Default_VerifyAddWater = 8;
        public static readonly int Default_VerifyAddDMF = 9;
        public static readonly int Default_BottleCorrection = 10;
        public static readonly int Default_WashSyringe = 11;
        public static readonly int Default_BottleSelf = 12;


        /// <summary>
        /// 染色进程
        /// </summary>
        [Description("染色进程")]
        public static int DyeingProcess { get; set; } = Default_DyeingProcess;

        /// <summary>
        /// 滴液进程
        /// </summary>
        [Description("滴液进程")]
        public static int DropProcess { get; set; } = Default_DropProcess;

        /// <summary>
        /// 后处理进程
        /// </summary>
        [Description("后处理进程")]
        public static int PostProcess { get; set; } = Default_PostProcess;

        /// <summary>
        /// 吸光度进程
        /// </summary>
        [Description("吸光度进程")]
        public static int UVProcess { get; set; } = Default_UVProcess;

        /// <summary>
        /// 自动开料进程
        /// </summary>
        [Description("自动开料进程")]
        public static int BrewProcess { get; set; } = Default_BrewProcess;

        /// <summary>
        /// 水校正进程
        /// </summary>
        [Description("水校正进程")]
        public static int WaterCorrection { get; set; } = Default_WaterCorrection;

        /// <summary>
        /// 水验证进程
        /// </summary>
        [Description("水验证进程")]
        public static int VerifyAddWater { get; set; } = Default_VerifyAddWater;

        /// <summary>
        /// 溶解剂校正进程
        /// </summary>
        [Description("溶解剂校正进程")]
        public static int DMFCorrection { get; set; } = Default_DMFCorrection;

        /// <summary>
        /// 溶解剂验证进程
        /// </summary>
        [Description("溶解剂验证进程")]
        public static int VerifyAddDMF { get; set; } = Default_VerifyAddDMF;

        /// <summary>
        /// 母液瓶校正进程
        /// </summary>
        [Description("母液瓶校正进程")]
        public static int BottleCorrection { get; set; } = Default_BottleCorrection;

        /// <summary>
        /// 母液瓶自检进程
        /// </summary>
        [Description("母液瓶自检进程")]
        public static int BottleSelf { get; set; } = Default_BottleSelf;

        /// <summary>
        /// 母液瓶洗针进程
        /// </summary>
        [Description("母液瓶洗针进程")]
        public static int WashSyringe { get; set; } = Default_WashSyringe;

        /// <summary>
        /// 恢复默认顺序
        /// </summary>
        public static void RestoreDefault()
        {
            DyeingProcess = Default_DyeingProcess;
            DropProcess = Default_DropProcess;
            PostProcess = Default_PostProcess;
            UVProcess = Default_UVProcess;
            BrewProcess = Default_BrewProcess;
            WaterCorrection = Default_WaterCorrection;
            VerifyAddWater = Default_VerifyAddWater;
            DMFCorrection = Default_DMFCorrection;
            VerifyAddDMF = Default_VerifyAddDMF;
            BottleCorrection = Default_BottleCorrection;
            BottleSelf = Default_BottleSelf;
            WashSyringe = Default_WashSyringe;
        }

        /// <summary>
        /// 根据业务类型获取名称
        /// </summary>
        /// <param name="robotBusinessType">类型</param>
        /// <returns>名称</returns>
        public static string GetTypeName(RobotBusinessType robotBusinessType)
        {
            switch (robotBusinessType)
            {
                case RobotBusinessType.DropProcess:
                    return "滴液";
                case RobotBusinessType.PostProcess:
                    return "后处理";
                case RobotBusinessType.UVProcess:
                    return "吸光度";
                case RobotBusinessType.WaterCorrection:
                    return "水校正";
                case RobotBusinessType.VerifyAddWater:
                    return "水验证";
                case RobotBusinessType.BrewProcess:
                    return "自动开料";
                case RobotBusinessType.DyeingProcess:
                    return "染色";
                case RobotBusinessType.DMFCorrection:
                    return "溶解剂校正";
                case RobotBusinessType.VerifyAddDMF:
                    return "溶解剂验证";
                case RobotBusinessType.BottleCorrection:
                    return "母液瓶校正";
                case RobotBusinessType.BottleSelf:
                    return "母液瓶自检";
                case RobotBusinessType.Debug:
                    return "手动";
                case RobotBusinessType.WashSyringe:
                    return "洗针";

                default:
                    return "未知进程";
            }
        }
    }
}