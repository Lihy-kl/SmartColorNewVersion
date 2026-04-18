using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_ConPar.Area.BottleArea
{
    /// <summary>
    /// 母液瓶参数
    /// </summary>
    internal class Bottle:Base
    {
        /// <summary>
        /// 母液瓶参数
        /// </summary>
        public Bottle()
        {
            AreaType = 9;
            AreaName = "母液瓶参数";
        }

        /// <summary>
        /// 母液瓶数量
        /// </summary>
       
        [Description("母液瓶数量")]
        public  int BottleNum { get; set; } = 120;

        /// <summary>
        /// 母液瓶列数
        /// </summary>
       
        [Description("母液瓶列数")]
        public  int BottleColumn { get; set; } = 10;

        /// <summary>
        /// 1号瓶瓶心X坐标
        /// </summary>
       
        [Description("1号瓶瓶心X坐标")]
        public int BottleCX_1 { get; set; } = 0;

        /// <summary>
        /// 1号瓶瓶心Y坐标
        /// </summary>
       
        [Description("1号瓶瓶心Y坐标")]
        public int BottleCY_1 { get; set; } = 0;

        /// <summary>
        /// X坐标间隔
        /// </summary>
       
        [Description("X坐标间隔")]
        public int BottleIX { get; set; } = 10500;

        /// <summary>
        /// Y坐标间隔
        /// </summary>
       
        [Description("Y坐标间隔")]
        public int BottleIY { get; set; } = 10500;

       

        // 嵌入天平参数
        public Balance.Balance EmbeddedBalance { get; set; } = null;

        // 加载嵌入天平参数
        public void LoadBalanceFromDict(Dictionary<string, string> dict)
        {
            var balanceProps = EmbeddedBalance.GetType().GetProperties();
            foreach (var p in balanceProps)
            {
                if (!p.CanWrite) continue;
                if (dict.TryGetValue(p.Name, out var value))
                {
                    object converted = Convert.ChangeType(value, p.PropertyType);
                    p.SetValue(EmbeddedBalance, converted);
                }
            }
        }

    }
}
