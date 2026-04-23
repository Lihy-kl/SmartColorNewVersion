using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib_Card.ADT8940A1.OutPut.Block
{
    /// <summary>
    /// 阻挡气缸
    /// </summary>
    public abstract class Block
    {
        /// <summary>
        /// 阻挡气缸伸出
        /// 异常：
        ///     1：气缸未在上限位
        ///     2：阻挡气缸伸出超时
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Block_Out();

        /// <summary>
        /// 阻挡气缸收回     
        /// 异常：
        ///     1：阻挡气缸收回超时
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Block_In();
    }
}
