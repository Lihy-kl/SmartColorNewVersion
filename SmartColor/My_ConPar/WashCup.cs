using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar
{
    /// <summary>
    /// 洗杯类
    /// </summary>
    internal class WashCup
    {
        // 参数变更事件
        public static event Action<string, double> ParameterChanged;

        private static double _wash_AddWater = 150;
        /// <summary>
        /// 洗杯加水量
        /// </summary>
        [Description("洗杯加水量|g")]
        public static double Wash_AddWater
        {
            get => _wash_AddWater;
            set
            {
                if (_wash_AddWater != value)
                {
                    _wash_AddWater = value;
                    ParameterChanged?.Invoke(nameof(Wash_AddWater), value);
                }
            }
        }

        private static double _wash_BigAddWater = 300;

        /// <summary>
        /// 洗杯加水量(大杯)
        /// </summary>
        [Description("洗杯加水量(大杯)|g")]
        public static double Wash_BigAddWater
        {
            get => _wash_BigAddWater;
            set
            {
                if (_wash_BigAddWater != value)
                {
                    _wash_BigAddWater = value;
                    ParameterChanged?.Invoke(nameof(Wash_BigAddWater), value);
                }
            }
        }


        private static double _wash_Temp = 0;
        /// <summary>
        /// 洗杯温度
        /// </summary>
        [Description("洗杯温度|℃")]
        public static double Wash_Temp
        {
            get => _wash_Temp;
            set
            {
                if (_wash_Temp != value)
                {
                    _wash_Temp = value;
                    ParameterChanged?.Invoke(nameof(Wash_Temp), value);
                }
            }
        }

        private static double _wash_TempSpeed = 10;
        /// <summary>
        /// 洗杯升温速率
        /// </summary>
        [Description("洗杯升温速率|℃/min")]
        public static double Wash_TempSpeed
        {
            get => _wash_TempSpeed;
            set
            {
                if (_wash_TempSpeed != value)
                {
                    _wash_TempSpeed = value;
                    ParameterChanged?.Invoke(nameof(Wash_TempSpeed), value);
                }
            }
        }

        private static double _highTempWash_AddWater = 100;
        /// <summary>
        /// 高温洗杯加水量
        /// </summary>
        [Description("高温洗杯加水量|g")]
        public static double HighTempWash_AddWater
        {
            get => _highTempWash_AddWater;
            set
            {
                if (_highTempWash_AddWater != value)
                {
                    _highTempWash_AddWater = value;
                    ParameterChanged?.Invoke(nameof(HighTempWash_AddWater), value);
                }
            }
        }

        private static double _highTempWash_BigAddWater = 100;
        /// <summary>
        /// 高温洗杯(大杯)加水量
        /// </summary>
        [Description("高温洗杯(大杯)加水量|g")]
        public static double HighTempWash_BigAddWater
        {
            get => _highTempWash_BigAddWater;
            set
            {
                if (_highTempWash_BigAddWater != value)
                {
                    _highTempWash_BigAddWater = value;
                    ParameterChanged?.Invoke(nameof(HighTempWash_BigAddWater), value);
                }
            }
        }

        private static double _highTempWash_Temp = 0;
        /// <summary>
        ///  高温洗杯温度
        /// </summary>
        [Description("高温洗杯温度|℃")]
        public static double HighTempWash_Temp
        {
            get => _highTempWash_Temp;
            set
            {
                if (_highTempWash_Temp != value)
                {
                    _highTempWash_Temp = value;
                    ParameterChanged?.Invoke(nameof(HighTempWash_Temp), value);
                }
            }
        }

        private static double _highTempWash_TempSpeed = 10;
        /// <summary>
        ///  高温洗杯升温速率
        /// </summary>
        [Description("高温洗杯升温速率|℃/min")]
        public static double HighTempWash_TempSpeed
        {
            get => _highTempWash_TempSpeed;
            set
            {
                if (_highTempWash_TempSpeed != value)
                {
                    _highTempWash_TempSpeed = value;
                    ParameterChanged?.Invoke(nameof(HighTempWash_TempSpeed), value);
                }
            }
        }

    }
}
