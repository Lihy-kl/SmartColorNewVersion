using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_Interface
{
    public interface ICustomUpdatable
    {
        /// <summary>
        /// 控件的唯一标识符，用于数据匹配
        /// </summary>
        string ControlKey { get; }

        /// <summary>
        /// 更新控件内容的方法
        /// </summary>
        /// <param name="row">数据</param>
        void UpdateFromData(System.Data.DataRow row);
    }
}
