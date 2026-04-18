using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>
    /// 半自动操作参数基类，所有半自动操作参数类需继承
    /// </summary>
    internal abstract class PLC_SemiAutoParamBase
    {
        /// <summary>动作编号 D800</summary>
        public ProtocolItem OperationType { get; set; }

        protected PLC_SemiAutoParamBase(PLC.SemiAutomaticOperation operation)
        {
            OperationType = new ProtocolItem(800, typeof(ushort), (int)operation);
        }

        /// <summary>
        /// 获取所有ProtocolItem，按地址排序
        /// </summary>
        public virtual List<ProtocolItem> GetAllItems()
        {
            var props = GetType().GetProperties();
            var items = new List<ProtocolItem>();
            foreach (var p in props)
            {
                var item = p.GetValue(this) as ProtocolItem;
                if (item != null && item.Value != null)
                    items.Add(item);
            }
            // 按地址排序
            items = items.OrderBy(i => i.Address).ToList();

            // 自动补齐中间未用地址
            if (items.Count > 0)
            {
                int start = items[0].Address;
                int end = items[items.Count - 1].Address;
                var fullList = new List<ProtocolItem>();
                for (int addr = start; addr <= end; addr++)
                {
                    var found = items.FirstOrDefault(i => i.Address == addr);
                    if (found != null)
                        fullList.Add(found);
                    else
                        fullList.Add(new ProtocolItem(addr, typeof(short), 0)); // 或0xFFFF
                }
                return fullList;
            }
            return items;
        }
    }

}
