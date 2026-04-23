using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartColor.My_ADT8940A1
{
    internal class ADT8940A1_Reset
    {
        /// <summary>
        /// 复位（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public Task<int> Reset()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            return Task.Run(() =>
            {
                try
                {
                    ////泄压下
                    //Lib_Card.ADT8940A1.OutPut.Decompression.Decompression decompression = new Lib_Card.ADT8940A1.OutPut.Decompression.Decompression_Condition();
                    //if (-1 == decompression.Decompression_Down())
                    //    throw new Exception("驱动异常");

                    ////泄压
                    //Thread.Sleep(Convert.ToInt32(SmartColor.My_ConPar.Delay.DecoTime * 1000.00));

                    ////泄压上
                    //if (-1 == decompression.Decompression_Up())
                    //    throw new Exception("驱动异常");
                }
                catch { }

                return 2;
            });
        }
    }
}
