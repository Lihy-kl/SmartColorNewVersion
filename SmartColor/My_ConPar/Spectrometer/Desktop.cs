using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Spectrometer
{
    /// <summary>
    /// 台式分光仪参数
    /// </summary>
    internal class Desktop
    {
        /// <summary>
        /// 设备端口号
        /// </summary>
        [Description("设备端口号")]
        public static string Port { get; set; } = "COM1";

        /// <summary>
        /// 光源类型
        /// 0：A  1：C  2：D50  3：D55  4：D65  5：D75  6：F1  7：F2  8：F3  9：F4
        /// 10：F5  11：F6  12：F7  13：F8  14：F9  15：F10 16：F11 17：F12
        /// 18：CWF 19：U30 20：DLF 21：NBF 22：TL83 23：TL84 24：U35 25：B
        /// </summary>
        [Description("光源类型| 0:A  1:C  2:D50  3:D55  4:D65  5:D75  6:F1  7:F2  8:F3  9:F4  " +
         "10:F5  11:F6  12:F7  13:F8  14:F9  15:F10 16:F11 17:F12 " +
         "18:CWF 19:U30 20:DLF 21:NBF 22:TL83 23:TL84 24:U35 25:B")]

        public static int LightSourceType { get; set; } = 4;

        /// <summary>
        /// 观察者角度
        /// 2：2°  10：10°
        /// </summary>
        [Description("观察者角度| 2:2° 10:10°")]
        public static int ObserverAngle { get; set; } = 10;


    }
}
