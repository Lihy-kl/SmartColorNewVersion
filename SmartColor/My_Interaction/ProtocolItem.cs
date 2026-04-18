using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_Interaction
{
    /// <summary>
    /// 协议项，描述一个PLC寄存器的数据点，包括地址、名称、数据类型、说明、枚举映射及当前值。
    /// 用于统一管理工位相关的所有数据点，便于批量读取和写入。
    /// </summary>
    internal class ProtocolItem
    {
        /// <summary>寄存器地址（PLC中的物理地址）</summary>
        public int Address { get; set; }

        /// <summary>数据类型（如ushort、int），决定读取和解析方式</summary>
        public Type DataType { get; set; }

        /// <summary>当前值（读取后填充），可用于界面显示或业务逻辑判断</summary>
        public object Value { get; set; }

        /// <summary>
        /// 构造函数，便于快速初始化协议项
        /// </summary>
        public ProtocolItem(int address, Type dataType)
        {
            Address = address;
            DataType = dataType;
        }

        public ProtocolItem(int address, Type dataType, int value)
        {
            Address = address;
            DataType = dataType;
            Value = value;
        }

        /// <summary>
        /// 默认构造函数，支持对象初始化器
        /// </summary>
        public ProtocolItem() { }

        /// <summary>
        /// 根据数据类型安全赋值，保证类型一致性。
        /// </summary>
        public void SetValue(object value)
        {
            if (value == null)
            {
                Value = DataType == typeof(int) ? 0 : (object)(ushort)0;
                return;
            }
            try
            {
                if (DataType == typeof(int))
                    Value = Convert.ToInt32(value);
                else if (DataType == typeof(ushort))
                    Value = Convert.ToUInt16(value);
                else
                    Value = value;
            }
            catch
            {
                Value = DataType == typeof(int) ? 0 : (object)(ushort)0;
            }
        }


    }
}
