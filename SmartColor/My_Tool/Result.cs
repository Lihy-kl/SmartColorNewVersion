using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_Tool
{
    public class Result
    {
        /// <summary>
        /// 返回值枚举
        /// </summary>
        public enum ResultCode
        {
            /// <summary>成功</summary>
            Success = 0,
            /// <summary>失败</summary>
            Failure = 1,
            /// <summary>需要交互</summary>
            NeedInteraction = 2,
            /// <summary>异常</summary>
            Exception = -1,
            /// <summary>取消</summary>
            Canceled = -2
        }
    }
}
