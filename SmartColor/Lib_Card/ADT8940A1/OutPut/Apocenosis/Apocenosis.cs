using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib_Card.ADT8940A1.OutPut.Apocenosis
{
    /// <summary>
    /// 排液
    /// </summary>
   public abstract class Apocenosis
    {
        /// <summary>
        /// 排液打开
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Apocenosis_On();

        /// <summary>
        /// 排液关闭      
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Apocenosis_Off();
    }
}
