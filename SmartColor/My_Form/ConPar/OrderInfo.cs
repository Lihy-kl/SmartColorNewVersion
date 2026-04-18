using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SmartColor.My_Form.ConPar
{
    /// <summary>
    /// 用于显示和调整工序模块顺序的窗体。
    /// </summary>
    public partial class OrderInfo : Form
    {
        /// <summary>
        /// 顺序配置文件路径，保存模块顺序
        /// </summary>
        public static readonly string OrderConfigPath = Path.Combine(Environment.CurrentDirectory, "Config", "Order.ini");

        /// <summary>
        /// 当前工序类型（包含静态属性，表示各模块顺序）。
        /// </summary>
        private Type processType;

        private bool _saved = false;

        public struct PriorityType
        {
            public string Name;
            public Type Type;

        }

        /// <summary>
        /// 工序优先级类型列表。
        /// </summary>
        public static readonly PriorityType[] PriorityTypes = new[]
        {
            new PriorityType { Name = "染色进程", Type = typeof(SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess) },
            new PriorityType { Name = "后处理进程", Type = typeof(SmartColor.My_ConPar.Order.PostProcess.PostProcess) },
            new PriorityType { Name = "滴液进程", Type = typeof(SmartColor.My_ConPar.Order.DropProcess.DropProcess) },
            new PriorityType { Name = "自动开料进程", Type = typeof(SmartColor.My_ConPar.Order.BrewProcess.BrewProcess) },
            new PriorityType { Name = "吸光度进程", Type = typeof(SmartColor.My_ConPar.Order.UVProcess.UVProcess) },
            new PriorityType { Name = "机械手进程", Type = typeof(SmartColor.My_ConPar.Order.BigProcess) }
        };

        /// <summary>
        /// 构造函数，初始化 OrderInfo 窗体并设置工序类型。
        /// </summary>
        /// <param name="processType">工序类型（Type），包含静态属性用于描述模块顺序。</param>
        public OrderInfo(string name, Type processType)
        {
            InitializeComponent();
            this.processType = processType;
            this.Text = name + "模块顺序调整";
            this.ctModuleDragPanel1.SaveClicked += CtModuleDragPanel1_SaveClicked;
            this.FormClosing += OrderInfo_FormClosing;
            this.FormClosed += OrderInfo_FormClosed;
            this.ctModuleDragPanel1.SetProcessType(processType);
            this.ctModuleDragPanel1.GetModuleInfosFunc = GetModuleInfos;
            this.ctModuleDragPanel1.ModuleButtonClicked += CtModuleDragPanel1_ModuleButtonClicked;
        }

        private void CtModuleDragPanel1_ModuleButtonClicked(object sender, string e)
        {
           foreach(var v in PriorityTypes)
            {
                if(v.Name == e)
                {
                    using (var dlg = new SmartColor.My_Form.ConPar.OrderInfo(v.Name, v.Type))
                    {
                        dlg.ShowDialog(this);
                    }
                    return;
                }
            }
            My_File.LocalTranslator.ShowMessage($"{e}没有模块顺序！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);

        }

        private void OrderInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.ctModuleDragPanel1.SaveClicked -= CtModuleDragPanel1_SaveClicked;
        }

        private void OrderInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_saved)
            {
                // 新增：判断是否全部拖拽完成
                if (!ctModuleDragPanel1.AllModulesPlaced)
                {
                    My_File.LocalTranslator.ShowMessage("请将所有模块拖拽到工作区后再保存！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }
                // 触发保存逻辑
                DialogResult dialogResult = this.ctModuleDragPanel1.Save();
                if (dialogResult == DialogResult.OK)
                {
                    _saved = true;
                    // 允许关闭
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    // 允许关闭，但不保存
                    _saved = false;
                    // e.Cancel = false; // 可省略，默认就是false
                }
                else
                {
                    // 阻止关闭
                    e.Cancel = true;
                }
            }
        }

        private void CtModuleDragPanel1_SaveClicked(object sender, List<ModuleInfo> orderList)
        {
            // 1. 先按拖拽顺序给静态属性赋值，并同步更新ModuleInfo.Order
            int idx = 1;
            foreach (var module in orderList)
            {
                var prop = processType.GetProperty(module.PropName, BindingFlags.Public | BindingFlags.Static);
                if (prop != null)
                {
                    prop.SetValue(null, idx);
                    module.Order = idx; // 同步更新ModuleInfo.Order
                    idx++;
                }
            }

            // 2. 读取原有 Order.ini 内容
            var orderFile = OrderConfigPath;
            var allLines = File.Exists(orderFile) ? File.ReadAllLines(orderFile).ToList() : new List<string>();

            // 3. 找到当前类型段落，准备替换
            string sectionName = $"[{processType.Name}]";
            int startIdx = allLines.FindIndex(l => l.Trim().Equals(sectionName, StringComparison.OrdinalIgnoreCase));
            int endIdx = -1;
            if (startIdx >= 0)
            {
                // 找到下一个段落或文件结尾
                endIdx = allLines.FindIndex(startIdx + 1, l => l.StartsWith("[") && l.EndsWith("]"));
                if (endIdx == -1) endIdx = allLines.Count;
                allLines.RemoveRange(startIdx, endIdx - startIdx);
            }

            // 4. 生成当前类型的新段落（顺序号就是拖拽顺序）
            var sb = new StringBuilder();
            sb.AppendLine(sectionName);
            foreach (var module in orderList)
            {
                sb.AppendLine($"{module.PropName}={module.Order}");
            }
            // 5. 插入到合适位置（文件末尾或原位置）
            var linesToInsert = sb.ToString().Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            if (startIdx >= 0)
                allLines.InsertRange(startIdx, linesToInsert);
            else
                allLines.AddRange(linesToInsert);

            // 6. 写回文件
            Directory.CreateDirectory(Path.GetDirectoryName(orderFile));
            File.WriteAllLines(orderFile, allLines.Where(l => !string.IsNullOrWhiteSpace(l)), Encoding.UTF8);

            My_File.LocalTranslator.ShowMessage("保存成功！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _saved = true;
            this.Close();
        }

        /// <summary>
        /// 窗体加载事件，加载当前工序的模块顺序。
        /// </summary>
        private void OrderInfo_Load(object sender, EventArgs e)
        {
            LoadCurrentMachineOrder();
            this.CenterToScreen();
        }

        /// <summary>
        /// 加载当前工序的模块顺序，并初始化模块拖拽面板。
        /// </summary>
        private void LoadCurrentMachineOrder()
        {
            // 获取模块信息并按顺序排序
            var modules = GetModuleInfos(processType)
                .OrderBy(m => m.Order)
                .ToList();

            // 初始化面板模块
            ctModuleDragPanel1.InitModules(modules);
        }

        /// <summary>
        /// 获取指定类型的所有模块信息。
        /// 通过反射获取类型的所有公共静态属性，读取 DescriptionAttribute 作为显示名，属性值作为顺序。
        /// </summary>
        /// <param name="type">包含模块定义的类型。</param>
        /// <returns>模块信息列表。</returns>
        private List<ModuleInfo> GetModuleInfos(Type type)
        {
            var list = new List<ModuleInfo>();
            // 遍历所有公共静态属性
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                // 获取 DescriptionAttribute 作为显示名
                var descAttr = prop.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
                string name = descAttr != null ? descAttr.Description : prop.Name;
                // 属性值作为顺序（假设为 int）
                int order = (int)prop.GetValue(null);
                if (type == typeof(My_ConPar.Order.BigProcess))
                {
                    if (My_ConPar.Machine.MachineLayout == 0 && (name == "染色进程" || name == "后处理进程"))
                    {
                        continue;
                    }

                    if (My_ConPar.Machine.UseAbs == 0 && name == "吸光度进程")
                    {
                        continue;
                    }

                    if (My_ConPar.Machine.CuttingMachine != 5 && name == "自动开料进程")
                    {
                        continue;
                    }
                }


                list.Add(new ModuleInfo { Name = name, PropName = prop.Name, Order = order });
            }


            return list;
        }
    }

    /// <summary>
    /// 表示模块信息，包括显示名、属性名和顺序。
    /// </summary>
    public class ModuleInfo
    {
        /// <summary>
        /// 模块显示名（通常来源于 DescriptionAttribute）。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 模块对应的属性名。
        /// </summary>
        public string PropName { get; set; }

        /// <summary>
        /// 模块顺序（用于排序）。
        /// </summary>
        public int Order { get; set; }
    }

}