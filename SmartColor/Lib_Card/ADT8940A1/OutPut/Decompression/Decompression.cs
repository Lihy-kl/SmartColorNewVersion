using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib_Card.ADT8940A1.OutPut.Decompression
{
    /// <summary>
    /// 泄压气缸
    /// </summary>
    public abstract class Decompression
    {
        /// <summary>
        /// 泄压气缸下
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Decompression_Down();

        /// <summary>
        /// 泄压气缸上   
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Decompression_Up();


        /// <summary>
        /// 泄压气缸下（右）
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Decompression_Down_Right();

        /// <summary>
        /// 泄压气缸上（右）
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Decompression_Up_Right();
    }
}
