using com.google.zxing;
using SmartColor.My_ConPar.Area.Drop;
using SmartColor.My_DataBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Windows.Forms;

namespace SmartColor.My_File
{
    /// <summary>
    /// 配置文件读写辅助类，提供类型属性与配置文件的自动同步、界面交互等功能。
    /// </summary>
    static  class ConfigHelper
    {
        /// <summary>
        /// 检查txt配置文件是否包含目标类型的所有静态属性/字段（或实例属性），
        /// 若缺失则弹出配置页面，直到配置齐全为止，并自动赋值到目标类型。
        /// </summary>
        /// <param name="owner">父控件（用于弹出配置窗口）</param>
        /// <param name="txtPath">配置文件路径</param>
        /// <param name="targetType">目标类型（静态属性/字段）</param>
        /// <param name="configFormFactory">配置窗口工厂方法</param>
        /// <param name="instance">可选，若指定则为实例属性赋值</param>
        /// <param name="alwaysPrompt">是否始终弹出配置窗口</param>
        /// <returns>配置齐全返回true，否则循环弹窗</returns>
        public static  bool CheckAndAssignOrPrompt(
            Control owner, string txtPath, Type targetType, Func<Form> configFormFactory, object instance = null, bool alwaysPrompt = false)
        {
            var txtHelper = new My_Interaction.TxtInteraction();

            if (alwaysPrompt)
            {
                // 先读取配置文件并赋值到实例
                if (instance != null)
                {
                    var dict = txtHelper.ReadAll(txtPath);
                    foreach (var kv in dict)
                    {
                        var prop = instance.GetType().GetProperty(kv.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (prop != null && prop.CanWrite)
                        {
                            try
                            {
                                object value = Convert.ChangeType(kv.Value, prop.PropertyType);
                                prop.SetValue(instance, value);
                            }
                            catch { }
                        }
                    }
                }
                // 始终弹出配置窗口，允许中途退出
                using (var configForm = configFormFactory())
                {
                    var result = configForm.ShowDialog(owner);
                    return result == DialogResult.OK;
                }
            }

            while (true)
            {
                var dict = txtHelper.ReadAll(txtPath);

                if (instance == null)
                {
                    // 静态属性/字段赋值
                    foreach (var kv in dict)
                    {
                        // 先查找属性
                        var prop = targetType.GetProperty(
                            kv.Key, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
                        if (prop != null && prop.CanWrite)
                        {
                            try
                            {
                                object value = Convert.ChangeType(kv.Value, prop.PropertyType);
                                prop.SetValue(null, value);
                            }
                            catch { }
                            continue;
                        }
                        // 再查找字段
                        var field = targetType.GetField(
                            kv.Key, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
                        if (field != null)
                        {
                            try
                            {
                                object value = Convert.ChangeType(kv.Value, field.FieldType);
                                field.SetValue(null, value);
                            }
                            catch { }
                        }
                    }

                    // 校验所有静态属性/字段是否都已配置
                    var props = targetType.GetProperties(BindingFlags.Public | BindingFlags.Static);
                    var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Static);

                    bool needConfig = false;
                    foreach (var prop in props)
                    {
                        if (!dict.ContainsKey(prop.Name))
                        {
                            needConfig = true;
                            break;
                        }
                    }
                    if (!needConfig)
                    {
                        foreach (var field in fields)
                        {
                            if (!dict.ContainsKey(field.Name))
                            {
                                needConfig = true;
                                break;
                            }
                        }
                    }

                    // 若有缺失，弹出配置窗口
                    if (needConfig)
                    {
                        ShowConfig(owner, configFormFactory);
                        continue;
                    }
                }
                else
                {
                    // 实例属性赋值
                    foreach (var kv in dict)
                    {
                        var prop = instance.GetType().GetProperty(
                            kv.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (prop != null && prop.CanWrite)
                        {
                            try
                            {
                                object value = Convert.ChangeType(kv.Value, prop.PropertyType);
                                prop.SetValue(instance, value);
                            }
                            catch
                            {
                            
                            }
                        }
                    }

                    // 校验所有实例属性是否都已配置
                    var props = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    bool needConfig = false;
                    foreach (var prop in props)
                    {
                        if (!dict.ContainsKey(prop.Name))
                        {
                            needConfig = true;
                            break;
                        }
                    }

                    // 若有缺失，弹出配置窗口并保存最新参数
                    if (needConfig)
                    {
                        ShowConfig(owner, configFormFactory);
                        // 保存最新参数到配置文件
                        var dictToSave = new Dictionary<string, string>();
                        var propsToSave = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var prop in propsToSave)
                        {
                            if (prop.CanRead && prop.CanWrite)
                            {
                                var value = prop.GetValue(instance);
                                dictToSave[prop.Name] = value?.ToString() ?? string.Empty;
                            }
                        }
                        var txtHelper2 = new My_Interaction.TxtInteraction();
                        txtHelper2.WriteAll(txtPath, dictToSave);
                        continue;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// 写入单个配置项到txt配置文件（key=value格式），保留其它项不变
        /// </summary>
        /// <param name="txtPath">配置文件路径</param>
        /// <param name="key">键名</param>
        /// <param name="value">要写入的值</param>
        /// <returns>写入成功返回true</returns>
        public static  bool WriteValue(string txtPath, string key, string value)
        {
            var txtHelper = new SmartColor.My_Interaction.TxtInteraction();
            return txtHelper.Set(txtPath, key, value);
        }
        /// <summary>
        /// 写入单个配置项到txt配置文件（key=value格式），保留其它项不变
        /// </summary>
        /// <param name="section">节名</param>
        /// <param name="key">键名</param>
        /// <param name="value">要写入的值</param>
        /// <param name="iniPath">配置文件路径</param>
        /// <returns>写入成功返回true</returns>
        public static  bool WriteIniValue(string section, string key, string value, string iniPath)
        {
            var ini = new IniFile(iniPath); // 你需要有一个IniFile类，或用Win32 API
            ini.WriteValue(section, key, value);
            return true;
        }

        /// <summary>
        /// 显示配置窗口（线程安全，自动Invoke）
        /// </summary>
        /// <param name="owner">父控件</param>
        /// <param name="configFormFactory">配置窗口工厂方法</param>
        private static void ShowConfig(Control owner, Func<Form> configFormFactory)
        {
            if (owner.InvokeRequired)
            {
                owner.Invoke((Action)(() => ShowConfig(owner, configFormFactory)));
                return;
            }
            using (var configForm = configFormFactory())
            {
                configForm.ShowDialog(owner);
            }
        }

        /// <summary>
        /// 创建配置文件，并将目标类型的所有静态属性/字段的当前值写入到文件（key=value格式）
        /// </summary>
        /// <param name="txtPath">配置文件路径</param>
        /// <param name="targetType">目标类型（静态属性/字段）</param>
        /// <returns>写入成功返回true</returns>
        public static  bool WriteAllValuesToFile(string txtPath, Type targetType)
        {
            var dict = new Dictionary<string, string>();
            var props = targetType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var prop in props)
            {
                if (prop.CanRead)
                {
                    try
                    {
                        var value = prop.GetValue(null);
                        dict[prop.Name] = value?.ToString() ?? string.Empty;
                    }
                    catch
                    {
                        dict[prop.Name] = string.Empty;
                    }
                }
            }
            var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                try
                {
                    var value = field.GetValue(null);
                    dict[field.Name] = value?.ToString() ?? string.Empty;
                }
                catch
                {
                    dict[field.Name] = string.Empty;
                }
            }
            var txtHelper = new My_Interaction.TxtInteraction();
            return txtHelper.WriteAll(txtPath, dict);
        }

        public static  bool WriteAllValuesToFile(string txtPath, object instance)
        {
            var dict = new Dictionary<string, string>();
            var props = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    try
                    {
                        var value = prop.GetValue(instance);
                        dict[prop.Name] = value?.ToString() ?? string.Empty;
                    }
                    catch
                    {
                        dict[prop.Name] = string.Empty;
                    }
                }
            }
            var txtHelper = new My_Interaction.TxtInteraction();
            return txtHelper.WriteAll(txtPath, dict);
        }

        /// <summary>
        /// 将目标类型的所有静态属性加载到DataGridView中，供用户查看和编辑
        /// </summary>
        /// <param name="grid">需要显示的DataGridView控件</param>
        /// <param name="targetType">需要显示的变量类（静态属性）</param>
        public static  void LoadTypeToDataGridView(DataGridView grid, Type targetType)
        {
            // 初始化DataGridView列
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.Columns.Add("VarName", "变量名");
            grid.Columns.Add("Remark1", "名称");
            grid.Columns.Add("Value", "当前值");
            grid.Columns.Add("Remark2", "备注");

            foreach (DataGridViewColumn col in grid.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.Columns["Remark2"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // 加载数据
            var props = targetType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var prop in props)
            {
                string name = prop.Name;
                object value = prop.GetValue(null);
                string remark1 = "";
                string remark2 = "";
                var descAttr = prop.GetCustomAttribute<DescriptionAttribute>();
                if (descAttr != null)
                {
                    var descs = descAttr.Description.Split('|');
                    remark1 = descs.Length > 0 ? descs[0].Trim() : "";
                    remark2 = descs.Length > 1 ? descs[1].Trim() : "";
                }
                grid.Rows.Add(name, remark1, value, remark2);
            }
            grid.Columns["VarName"].Visible = false;
            grid.Columns["Remark1"].ReadOnly = true;
            grid.Columns["Remark2"].ReadOnly = true;

            // 列宽自适应
            if (grid is SmartColor.My_Control.CtDataGridView ctGrid)
                ctGrid.AutoFitAllColumns();
            else
                grid.AutoResizeColumns();

            // 行高自适应
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grid.AutoResizeRows();

            // 计算理想宽高
            int idealWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
            foreach (DataGridViewColumn col in grid.Columns)
                if (col.Visible) idealWidth += col.Width;
            idealWidth += SystemInformation.VerticalScrollBarWidth; // 预留滚动条宽度

            int idealHeight = grid.ColumnHeadersHeight;
            foreach (DataGridViewRow row in grid.Rows)
                if (row.Visible) idealHeight += row.Height;
            idealHeight += SystemInformation.HorizontalScrollBarHeight; // 预留滚动条高度

            // 获取父窗体
            var form = grid.FindForm();
            if (form != null)
            {
                var screen = Screen.GetWorkingArea(form);
                int maxWidth = (int)(screen.Width * 0.98);
                int maxHeight = (int)(screen.Height * 0.98);
                int minWidth = 600;
                int minHeight = 400;

                // 只在窗体未最大化时自动调整窗体大小
                if (form.WindowState != FormWindowState.Maximized)
                {
                    int newWidth = Math.Min(Math.Max(idealWidth + 30, minWidth), maxWidth);
                    int newHeight = Math.Min(Math.Max(idealHeight + 50, minHeight), maxHeight);

                    // 保持中心点不变
                    var oldCenter = new System.Drawing.Point(form.Left + form.Width / 2, form.Top + form.Height / 2);
                    form.Width = newWidth;
                    form.Height = newHeight;
                    form.Left = oldCenter.X - form.Width / 2;
                    form.Top = oldCenter.Y - form.Height / 2;                   
                }
            }
           
            grid.ScrollBars = ScrollBars.Both;
            grid.Dock = DockStyle.Fill;
        }


        /// <summary>
        /// 将实例对象的所有可读写属性（含基类）加载到DataGridView中，供用户查看和编辑
        /// </summary>
        /// <param name="grid">DataGridView控件</param>
        /// <param name="instance">实例对象</param>
        public static  void LoadTypeToDataGridView(DataGridView grid, object instance)
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.Columns.Add("VarName", "变量名");
            grid.Columns.Add("Remark1", "名称");
            grid.Columns.Add("Value", "当前值");
            grid.Columns.Add("Remark2", "备注");

            foreach (DataGridViewColumn col in grid.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // 设置自动换行
            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            grid.Columns["Remark2"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // 递归收集所有基类（从最基类到当前类）
            var typeList = new List<Type>();
            var t = instance.GetType();
            while (t != null && t != typeof(object))
            {
                typeList.Insert(0, t); // 最基类在最前面
                t = t.BaseType;
            }

            var loadedProps = new HashSet<string>();
            foreach (var tp in typeList)
            {
                foreach (var prop in tp.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (prop.CanRead && prop.CanWrite && !loadedProps.Contains(prop.Name))
                    {
                        string name = prop.Name;
                        object value = prop.GetValue(instance);
                        string remark1 = "";
                        string remark2 = "";
                        if (name == "EmbeddedBalance") continue; // 跳过特殊属性
                        var descAttr = prop.GetCustomAttribute<DescriptionAttribute>();
                        if (descAttr != null)
                        {
                            var descs = descAttr.Description.Split('|');
                            remark1 = descs.Length > 0 ? descs[0].Trim() : "";
                            remark2 = descs.Length > 1 ? descs[1].Trim() : "";
                        }

                        grid.Rows.Add(name, remark1, value, remark2);
                        loadedProps.Add(name);
                    }
                }
            }

            grid.Columns["VarName"].Visible = false;
            grid.Columns["Remark1"].ReadOnly = true;
            grid.Columns["Remark2"].ReadOnly = true;

            // 列宽自适应
            if (grid is SmartColor.My_Control.CtDataGridView ctGrid)
                ctGrid.AutoFitAllColumns();
            else
                grid.AutoResizeColumns();

            // 行高自适应
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grid.AutoResizeRows();

            // 计算理想宽高
            int idealWidth = grid.RowHeadersVisible ? grid.RowHeadersWidth : 0;
            foreach (DataGridViewColumn col in grid.Columns)
                if (col.Visible) idealWidth += col.Width;
            idealWidth += SystemInformation.VerticalScrollBarWidth; // 预留滚动条宽度

            int idealHeight = grid.ColumnHeadersHeight;
            foreach (DataGridViewRow row in grid.Rows)
                if (row.Visible) idealHeight += row.Height;
            idealHeight += SystemInformation.HorizontalScrollBarHeight; // 预留滚动条高度

            // 获取父窗体
            var form = grid.FindForm();
            if (form != null)
            {
                var screen = Screen.GetWorkingArea(form);
                int maxWidth = (int)(screen.Width * 0.98);
                int maxHeight = (int)(screen.Height * 0.98);
                int minWidth = 600;
                int minHeight = 400;

                // 只在窗体未最大化时自动调整窗体大小
                if (form.WindowState != FormWindowState.Maximized)
                {
                    int newWidth = Math.Min(Math.Max(idealWidth + 30, minWidth), maxWidth);
                    int newHeight = Math.Min(Math.Max(idealHeight + 50, minHeight), maxHeight);

                    // 保持中心点不变
                    var oldCenter = new System.Drawing.Point(form.Left + form.Width / 2, form.Top + form.Height / 2);
                    form.Width = newWidth;
                    form.Height = newHeight;
                    form.Left = oldCenter.X - form.Width / 2;
                    form.Top = oldCenter.Y - form.Height / 2;

                   
                }
               
            }
           
            grid.ScrollBars = ScrollBars.Both;
            grid.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// 将DataGridView中的数据写入目标类型的所有静态属性
        /// </summary>
        /// <param name="grid">DataGridView控件</param>
        /// <param name="targetType">目标类型（如typeof(My_ConPar.Machine)）</param>
        public static  void WriteDataGridViewToType(DataGridView grid, Type targetType)
        {
            var props = targetType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            var propDict = props.ToDictionary(p => p.Name, p => p);

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;
                string varName = row.Cells["VarName"].Value?.ToString();
                string valueStr = row.Cells["Value"].Value?.ToString();

                if (string.IsNullOrEmpty(varName) || !propDict.ContainsKey(varName))
                    continue;

                var prop = propDict[varName];
                try
                {
                    object convertedValue = Convert.ChangeType(valueStr, prop.PropertyType);
                    prop.SetValue(null, convertedValue);
                }
                catch
                {
                    LocalTranslator.ShowMessage($"变量 [{varName}] 的值 \"{valueStr}\" 无法转换为 {prop.PropertyType.Name}，已跳过。", "类型转换错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// 将DataGridView中的数据写入实例对象的属性
        /// </summary>
        /// <param name="grid">DataGridView控件</param>
        /// <param name="instance">目标实例对象</param>
        public static  void WriteDataGridViewToType(DataGridView grid, object instance)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;
                string varName = row.Cells["VarName"].Value?.ToString();
                string valueStr = row.Cells["Value"].Value?.ToString();
                var prop = instance.GetType().GetProperty(varName);
                if (prop != null && prop.CanWrite)
                {
                    try
                    {
                        object convertedValue = Convert.ChangeType(valueStr, prop.PropertyType);
                        prop.SetValue(instance, convertedValue);
                    }
                    catch
                    {
                        LocalTranslator.ShowMessage($"变量 [{varName}] 的值 \"{valueStr}\" 无法转换为 {prop.PropertyType.Name}，已跳过。", "类型转换错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        /// <summary>
        /// 读取布局参数配置文件（Layout.ini），并按区块编号与属性名映射，避免错位。
        /// 返回每个区域是否已配置。
        /// </summary>
        /// <param name="layoutType">布局类型（含静态属性）</param>
        /// <returns>每个区域是否已配置的布尔数组</returns>
        public static  bool[] LoadLayoutConfig(Type layoutType)
        {
            var layoutFile = Path.Combine(Environment.CurrentDirectory, "Config", "Layout.ini");
            if (!File.Exists(layoutFile))
            {
                // 文件不存在，返回全未配置
                return new bool[layoutType.GetProperties().Length];
            }

            // 只取静态公开属性
            var areaProps = layoutType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            // 构建编号到属性的映射，如 Area_1 => 1
            var areaPropDict = areaProps
                .Where(p => p.Name.StartsWith("Area_"))
                .ToDictionary(
                    p => int.TryParse(p.Name.Substring(5), out var num) ? num : -1,
                    p => p
                );

            bool[] areaConfigured = new bool[areaProps.Length];

            var lines = File.ReadAllLines(layoutFile);
            int currentAreaNum = -1;
            object currentAreaObj = null;
            foreach (var line in lines)
            {
                if (line.StartsWith("[Area"))
                {
                    // 解析区块编号
                    var match = System.Text.RegularExpressions.Regex.Match(line, @"\[Area(\d+)\]");
                    if (match.Success)
                    {
                        currentAreaNum = int.Parse(match.Groups[1].Value);
                        currentAreaObj = null;
                    }
                    continue;
                }
                else if (currentAreaNum > 0 && line.Contains("="))
                {
                    var parts = line.Split(new[] { '=' }, 2);
                    var propName = parts[0].Trim();
                    var propValue = parts[1].Trim();

                    if (propName == "AreaType")
                    {
                        // 根据AreaType创建区域对象
                        int areaType = int.TryParse(propValue, out var v) ? v : 0;
                        currentAreaObj = CreateAreaByType(areaType);
                        if (areaPropDict.ContainsKey(currentAreaNum))
                        {
                            areaPropDict[currentAreaNum].SetValue(null, currentAreaObj);
                            // 标记已配置
                            int idx = areaProps.ToList().IndexOf(areaPropDict[currentAreaNum]);
                            if (idx >= 0 && idx < areaConfigured.Length)
                                areaConfigured[idx] = true;
                        }
                    }

                    // 设置区域对象的属性
                    if (currentAreaObj != null)
                    {
                        var propInfo = currentAreaObj.GetType().GetProperty(propName);
                        if (propInfo != null)
                        {
                            object value = null;
                            if (propInfo.PropertyType == typeof(int))
                                value = int.TryParse(propValue, out var v) ? v : 0;
                            else if (propInfo.PropertyType == typeof(string))
                                value = propValue;
                            else
                                value = Convert.ChangeType(propValue, propInfo.PropertyType);

                            propInfo.SetValue(currentAreaObj, value);
                        }
                    }
                }
            }
            return areaConfigured;
        }

        /// <summary>
        /// 保存布局参数到Layout.ini文件，支持母液瓶区、各类杯区等多种区域类型。
        /// 并同步相关数据到数据库（如杯号等）。
        /// </summary>
        /// <param name="layoutType">布局类型（含静态属性）</param>
        //public static void SaveLayoutConfig(Type layoutType)
        //{
        //    var areaProps = layoutType.GetProperties();
        //    var layoutFile = Path.Combine(Environment.CurrentDirectory, "Config", "Layout.ini");

        //    var sb = new StringBuilder();
        //    int areaIndex = 1;
        //    var allAreaCupNums = new HashSet<int>(); // 新增：收集所有区域的杯号

        //    foreach (var prop in areaProps)
        //    {
        //        if (!typeof(SmartColor.My_ConPar.Area.Base).IsAssignableFrom(prop.PropertyType))
        //            continue;

        //        // 母液瓶区参数保存
        //        if (prop.GetValue(null) is SmartColor.My_ConPar.Area.BottleArea.Bottle bottleArea)
        //        {
        //            sb.AppendLine($"[Area{areaIndex}]");
        //            sb.AppendLine($"AreaType={bottleArea.AreaType}");
        //            sb.AppendLine($"AreaName={bottleArea.AreaName}");
        //            // 保存母液瓶参数
        //            foreach (var p in bottleArea.GetType().GetProperties())
        //            {
        //                if (p.Name == "AreaType" || p.Name == "AreaName" || p.Name == "EmbeddedBalance") continue;
        //                var value = p.GetValue(bottleArea);
        //                sb.AppendLine($"{p.Name}={value}");
        //            }
        //            // 嵌入天平参数
        //            if (bottleArea.BottleNum > 0 && bottleArea.BottleColumn > 0 && bottleArea.BottleNum % bottleArea.BottleColumn != 0)
        //            {
        //                foreach (var p in bottleArea.EmbeddedBalance.GetType().GetProperties())
        //                {
        //                    if (p.Name == "AreaType" || p.Name == "AreaName") continue;
        //                    var value = p.GetValue(bottleArea.EmbeddedBalance);
        //                    sb.AppendLine($"{p.Name}={value}");
        //                }
        //            }
        //            sb.AppendLine();
        //        }
        //        // 杯区参数保存及数据库同步
        //        else if (prop.GetValue(null) is SmartColor.My_ConPar.Area.Base areaObj)
        //        {
        //            int[] cupAreaTypes = { 2, 3, 4, 5, 6, 7, 8 };
        //            if (cupAreaTypes.Contains(areaObj.AreaType))
        //            {
        //                int start = 1, row = 1, col = 1;
        //                if (areaObj.GetType().GetProperty("StartPosition") != null)
        //                    start = (int)areaObj.GetType().GetProperty("StartPosition").GetValue(areaObj);
        //                if (areaObj.GetType().GetProperty("Row") != null)
        //                    row = (int)areaObj.GetType().GetProperty("Row").GetValue(areaObj);
        //                if (areaObj.GetType().GetProperty("Column") != null)
        //                    col = (int)areaObj.GetType().GetProperty("Column").GetValue(areaObj);
        //                if (start == 1 && row == 1 && col == 1) { areaIndex++; continue; }
        //                int total = row * col;
        //                int mainCupNum = 0;
        //                bool isMain = start % 2 == 1; // 假设奇数为主杯，偶数为副杯

        //                // 1. 生成本次所有杯号
        //                var cupNums = new List<int>();
        //                var cupDataList = new List<Dictionary<string, object>>();
        //                for (int i = 0; i < total; i++)
        //                {
        //                    int cupNum = start + i;
        //                    cupNums.Add(cupNum);

        //                    int type = 0;
        //                    if (areaObj.AreaType == 6)
        //                        type = 4;
        //                    else if (areaObj.AreaType == 8)
        //                        type = 2;
        //                    else
        //                        type = 3;

        //                    if (areaObj.AreaType == 4 || areaObj.AreaType == 5)
        //                    {
        //                        if (isMain)
        //                            mainCupNum = cupNum % 2 == 1 ? cupNum + 1 : cupNum - 1;
        //                        else
        //                            mainCupNum = cupNum % 2 == 1 ? cupNum - 1 : cupNum + 1;
        //                    }
        //                    else
        //                    {
        //                        mainCupNum = cupNum;
        //                    }

        //                    var cupData = new Dictionary<string, object>
        //            {
        //                { "CupNum", cupNum },
        //                { "MainCupNum", mainCupNum },
        //                { "Type", type },
        //                { "Statues", areaObj.AreaType == 8 ? null : "下线" },
        //                { "Enable", areaObj.AreaType == 8 ? 1 : 0 }
        //            };
        //                    cupDataList.Add(cupData);
        //                }

        //                // 新增：收集所有区域的杯号
        //                foreach (var n in cupNums)
        //                    allAreaCupNums.Add(n);

        //                // 2. 查询数据库中已存在的杯号
        //                var paramNames = cupNums.Select((n, i) => $"@CupNum{i}").ToList();
        //                string where = $"{CUP_DETAILS.CupNum} IN ({string.Join(",", paramNames)})";
        //                var sqlParams = cupNums.Select((n, i) => new System.Data.SqlClient.SqlParameter(paramNames[i], n)).ToArray();
        //                DataTable dtExist = SmartColor.My_DataBase.SqlServer.Select(CUP_DETAILS.TableName, where, sqlParams);

        //                var existCupNums = new HashSet<int>();
        //                foreach (DataRow row1 in dtExist.Rows)
        //                {
        //                    if (int.TryParse(row1[CUP_DETAILS.CupNum].ToString(), out int n))
        //                        existCupNums.Add(n);
        //                }

        //                // 3. 统一提示重复杯号
        //                if (existCupNums.Count > 0)
        //                {
        //                    string msg = $"以下杯号已存在：{string.Join(", ", existCupNums)}\n是否覆盖这些旧资料？";
        //                    var result = My_File.LocalTranslator.ShowMessage(msg, "杯号重复", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        //                    if (result == DialogResult.Yes)
        //                    {
        //                        // 批量删除旧杯号
        //                        foreach (var n in existCupNums)
        //                        {
        //                            SmartColor.My_DataBase.SqlServer.Delete(CUP_DETAILS.TableName, "CupNum=@CupNum",
        //                                new System.Data.SqlClient.SqlParameter("@CupNum", n));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        // 用户取消覆盖，跳过这些杯号
        //                        cupDataList.RemoveAll(d => existCupNums.Contains((int)d[CUP_DETAILS.CupNum]));
        //                    }
        //                }

        //                // 5. 批量插入新杯号
        //                if (cupDataList.Count > 0)
        //                    SmartColor.My_DataBase.SqlServer.Insert(CUP_DETAILS.TableName, cupDataList);
        //            }

        //            // 保存区域参数（原有代码不变）
        //            sb.AppendLine($"[Area{areaIndex}]");
        //            sb.AppendLine($"AreaType={areaObj.AreaType}");
        //            sb.AppendLine($"AreaName={areaObj.AreaName}");
        //            foreach (var p in areaObj.GetType().GetProperties())
        //            {
        //                if (p.Name == "AreaType" || p.Name == "AreaName") continue;
        //                var value = p.GetValue(areaObj);
        //                sb.AppendLine($"{p.Name}={value}");
        //            }
        //            sb.AppendLine();
        //        }
        //        areaIndex++;
        //    }

        //    // === 统一在循环外删除数据库中多余的杯号 ===
        //    DataTable dtAll = SmartColor.My_DataBase.SqlServer.Select(CUP_DETAILS.TableName);
        //    var allDbCupNums = new HashSet<int>();
        //    foreach (DataRow row1 in dtAll.Rows)
        //    {
        //        if (int.TryParse(row1[CUP_DETAILS.CupNum].ToString(), out int n))
        //            allDbCupNums.Add(n);
        //    }
        //    var extraCupNums = allDbCupNums.Except(allAreaCupNums).ToList();
        //    if (extraCupNums.Count > 0)
        //    {
        //        string inClause = string.Join(",", extraCupNums);
        //        SmartColor.My_DataBase.SqlServer.ExecuteNonQuery(
        //            $"DELETE FROM cup_details WHERE CupNum IN ({inClause})");
        //    }

        //    File.WriteAllText(layoutFile, sb.ToString());
        //}


        /// <summary>
        /// 保存布局参数到Layout.ini文件，仅保存配置文件，不再同步数据库杯号。
        /// </summary>
        /// <param name="layoutType">布局类型（含静态属性）</param>
        public static void SaveLayoutConfig(Type layoutType)
        {
            var areaProps = layoutType.GetProperties();
            var layoutFile = Path.Combine(Environment.CurrentDirectory, "Config", "Layout.ini");

            var sb = new StringBuilder();
            int areaIndex = 1;

            foreach (var prop in areaProps)
            {
                if (!typeof(SmartColor.My_ConPar.Area.Base).IsAssignableFrom(prop.PropertyType))
                    continue;

                // 母液瓶区参数保存
                if (prop.GetValue(null) is SmartColor.My_ConPar.Area.BottleArea.Bottle bottleArea)
                {
                    sb.AppendLine($"[Area{areaIndex}]");
                    sb.AppendLine($"AreaType={bottleArea.AreaType}");
                    sb.AppendLine($"AreaName={bottleArea.AreaName}");
                    foreach (var p in bottleArea.GetType().GetProperties())
                    {
                        if (p.Name == "AreaType" || p.Name == "AreaName" || p.Name == "EmbeddedBalance") continue;
                        var value = p.GetValue(bottleArea);
                        sb.AppendLine($"{p.Name}={value}");
                    }
                    if (bottleArea.BottleNum > 0 && bottleArea.BottleColumn > 0 && bottleArea.BottleNum % bottleArea.BottleColumn != 0)
                    {
                        foreach (var p in bottleArea.EmbeddedBalance.GetType().GetProperties())
                        {
                            if (p.Name == "AreaType" || p.Name == "AreaName") continue;
                            var value = p.GetValue(bottleArea.EmbeddedBalance);
                            sb.AppendLine($"{p.Name}={value}");
                        }
                    }
                    sb.AppendLine();
                }
                // 杯区参数保存（不再操作数据库）
                else if (prop.GetValue(null) is SmartColor.My_ConPar.Area.Base areaObj)
                {
                    sb.AppendLine($"[Area{areaIndex}]");
                    sb.AppendLine($"AreaType={areaObj.AreaType}");
                    sb.AppendLine($"AreaName={areaObj.AreaName}");
                    foreach (var p in areaObj.GetType().GetProperties())
                    {
                        if (p.Name == "AreaType" || p.Name == "AreaName") continue;
                        var value = p.GetValue(areaObj);
                        sb.AppendLine($"{p.Name}={value}");
                    }
                    sb.AppendLine();
                }
                areaIndex++;
            }

            File.WriteAllText(layoutFile, sb.ToString());
        }

        /// <summary>
        /// 工厂方法：根据AreaType实例化具体区域对象
        /// </summary>
        /// <param name="areaType">区域类型编号</param>
        /// <returns>对应的区域对象</returns>
        public static  SmartColor.My_ConPar.Area.Base CreateAreaByType(int areaType)
        {
            /// <summary>
            /// 区域类型
            /// 0：无
            /// 1：天平
            /// 2：4杯大翻转缸
            /// 3：6杯翻转缸
            /// 4：12杯翻转缸
            /// 5：16杯翻转缸
            /// 6：4杯转子缸
            /// 7：10杯转子缸
            /// 8：滴液区
            /// 9：母液瓶区
            /// 10：干布夹具
            /// 11：湿布夹具
            /// 12：公共针筒
            /// 13：清洗针筒
            /// 14：备布区
            /// 15：出布区
            /// </summary>

            switch (areaType)
            {
                case 1: return new SmartColor.My_ConPar.Area.Balance.Balance();
                case 2: return new SmartColor.My_ConPar.Area.FlipCylinder.FC_4();
                case 3: return new SmartColor.My_ConPar.Area.FlipCylinder.FC_6();
                case 4: return new SmartColor.My_ConPar.Area.FlipCylinder.FC_12();
                case 5: return new SmartColor.My_ConPar.Area.FlipCylinder.FC_16();
                case 6: return new SmartColor.My_ConPar.Area.RotorCylinder.RC_4();
                case 7: return new SmartColor.My_ConPar.Area.RotorCylinder.RC_10();
                case 8: return new SmartColor.My_ConPar.Area.Drop.Drop();
                case 9: return new SmartColor.My_ConPar.Area.BottleArea.Bottle();
                case 10: return new SmartColor.My_ConPar.Area.Clip.DryCloth();
                case 11: return new SmartColor.My_ConPar.Area.Clip.WetCloth();
                case 12: return new SmartColor.My_ConPar.Area.Needles.Needles();
                case 13: return new SmartColor.My_ConPar.Area.Wash.Wash();
                case 14: return new SmartColor.My_ConPar.Area.PrepareClothArea.PrepareClothArea();
                case 15: return new SmartColor.My_ConPar.Area.OutClothArea.OutClothArea();

                // ...其他类型...
                default: return new SmartColor.My_ConPar.Area.Base();
            }
        }

        /// <summary>
        /// 根据区域类型，生成对应的控件并设置布局参数，用于界面动态加载。
        /// </summary>
        /// <param name="areaBase">区域对象</param>
        /// <returns>对应的控件</returns>
        public static  Control FillControlsn(SmartColor.My_ConPar.Area.Base areaBase)
        {
            /// <summary>
            /// 区域类型
            /// 0：无
            /// 1：天平
            /// 2：4杯大翻转缸
            /// 3：6杯翻转缸
            /// 4：12杯翻转缸
            /// 5：16杯翻转缸
            /// 6：4杯转子缸
            /// 7：10杯转子缸
            /// 8：滴液区
            /// 9：母液瓶区
            /// 10：干布夹具
            /// 11：湿布夹具
            /// 12：公共针筒
            /// 13：清洗针筒
            /// 14：备布区
            /// 15：出布区
            /// </summary>
            switch (areaBase.AreaType)
            {
                case 1:
                    {
                        // 天平控件
                        var balance = new SmartColor.My_Control.CtBalance
                        {
                            Dock = DockStyle.Fill,
                            MaxValue = ((My_ConPar.Area.Balance.Balance)areaBase).MaxValue
                        };
                        return balance;
                    }
                case 2:
                    {
                        // 4杯大翻转缸控件
                        var fc4 = new SmartColor.My_Control.CtCupArea
                        {
                            Dock = DockStyle.Fill
                        };
                        var fc4par = (My_ConPar.Area.FlipCylinder.FC_4)areaBase;
                        fc4.SetLayout(fc4par.Row, fc4par.Column, fc4par.StartPosition, fc4par.Vertical, fc4par.AreaName,fc4par.AreaType,fc4par.IP,fc4par.Port,fc4par.AreaName);
                        return fc4;
                    }
                case 3:
                    {
                        // 6杯翻转缸控件
                        var fc6 = new SmartColor.My_Control.CtCupArea
                        {
                            Dock = DockStyle.Fill
                        };
                        var fc6par = (My_ConPar.Area.FlipCylinder.FC_6)areaBase;
                        fc6.SetLayout(fc6par.Row, fc6par.Column, fc6par.StartPosition, fc6par.Vertical, fc6par.AreaName,fc6par.AreaType,fc6par.IP,fc6par.Port,fc6par.AreaName);
                        return fc6;
                    }
                case 4:
                    {
                        // 12杯翻转缸控件
                        My_Control.CtCupArea fc12 = new SmartColor.My_Control.CtCupArea
                        {
                            Dock = DockStyle.Fill
                        };
                        var fc12par = (My_ConPar.Area.FlipCylinder.FC_12)areaBase;
                        fc12.SetLayout(fc12par.Row, fc12par.Column, fc12par.StartPosition, fc12par.Vertical, fc12par.AreaName, fc12par.AreaType, fc12par.IP, fc12par.Port, fc12par.AreaName,fc12par.HMIType);
                        return fc12;
                    }
                case 5:
                    {
                        // 16杯翻转缸控件
                        var fc16 = new SmartColor.My_Control.CtCupArea
                        {
                            Dock = DockStyle.Fill
                        };
                        var fc16par = (My_ConPar.Area.FlipCylinder.FC_16)areaBase;
                        fc16.SetLayout(fc16par.Row, fc16par.Column, fc16par.StartPosition, fc16par.Vertical, fc16par.AreaName, fc16par.AreaType, fc16par.IP, fc16par.Port, fc16par.AreaName);
                        return fc16;
                    }
                case 6:
                    {
                        // 4杯转子缸控件
                        var rc4 = new SmartColor.My_Control.CtCupArea
                        {
                            Dock = DockStyle.Fill
                        };
                        var rc4par = (My_ConPar.Area.RotorCylinder.RC_4)areaBase;
                        rc4.SetLayout(rc4par.Row, rc4par.Column, rc4par.StartPosition, rc4par.Vertical, rc4par.AreaName,rc4par.AreaType);
                        return rc4;
                    }
                case 7:
                    {
                        // 10杯转子缸控件
                        var rc10 = new SmartColor.My_Control.CtCupArea
                        {
                            Dock = DockStyle.Fill
                        };
                        var rc10par = (My_ConPar.Area.RotorCylinder.RC_10)areaBase;
                        rc10.SetLayout(rc10par.Row, rc10par.Column, rc10par.StartPosition, rc10par.Vertical, rc10par.AreaName,rc10par.AreaType,rc10par.IP,rc10par.Port,rc10par.AreaName);
                        return rc10;
                    }
                case 8:
                    {
                        // 滴液区控件
                        var drop = new SmartColor.My_Control.CtCupArea
                        {
                            Dock = DockStyle.Fill
                        };
                        var droppar = (My_ConPar.Area.Drop.Drop)areaBase;
                        drop.SetLayout(droppar.Row, droppar.Column, droppar.StartPosition, droppar.Vertical, droppar.AreaName,droppar.AreaType);
                        return drop;
                    }
                case 9:
                    {
                        // 母液瓶区控件
                        var bottle = new SmartColor.My_Control.CtBottleArea
                        {
                            Dock = DockStyle.Fill
                        };
                        var droppar = (My_ConPar.Area.BottleArea.Bottle)areaBase;
                        bottle.SetLayout(droppar.BottleNum, droppar.BottleColumn, null);
                        return bottle;
                    }
                case 10:
                    {
                        // 干布夹具控件
                        var clip = new SmartColor.My_Control.CtClip
                        {
                            Dock = DockStyle.None
                        };
                        var clippar = (My_ConPar.Area.Clip.DryCloth)areaBase;
                        clip.Desc = clippar.AreaName;
                        return clip;
                    }
                case 11:
                    {
                        // 湿布夹具控件
                        var clip = new SmartColor.My_Control.CtClip
                        {
                            Dock = DockStyle.None
                        };
                        var clippar = (My_ConPar.Area.Clip.WetCloth)areaBase;
                        clip.Desc = clippar.AreaName;
                        return clip;
                    }
                case 12:
                    {
                        // 公共针筒控件
                        var needles = new SmartColor.My_Control.CtNeedles
                        {
                            Dock = DockStyle.None
                        };
                        var needlespar = (My_ConPar.Area.Needles.Needles)areaBase;
                        needles.DefaultText = needlespar.AreaName;
                        return needles;
                    }
                case 13:
                    {
                        // 清洗针筒控件
                        var wash = new SmartColor.My_Control.CtWash
                        {
                            Dock = DockStyle.None
                        };
                        var washpar = (My_ConPar.Area.Wash.Wash)areaBase;
                        wash.DefaultText = washpar.AreaName;
                        return wash;
                    }
                case 14:
                    {
                        // 备布区控件
                        var pca = new SmartColor.My_Control.CtPrepareClothArea
                        {
                            Dock = DockStyle.Fill
                        };
                        var pcapar = (My_ConPar.Area.PrepareClothArea.PrepareClothArea)areaBase;
                        pca.SetLayout(pcapar.Row, pcapar.Column, pcapar.StartPosition, pcapar.Vertical, pcapar.AreaName);
                        return pca;
                    }
                case 15:
                    {
                        // 出布区控件
                        var oca = new SmartColor.My_Control.CtOutClothArea
                        {
                            Dock = DockStyle.Fill
                        };
                        var ocapar = (My_ConPar.Area.OutClothArea.OutClothArea)areaBase;
                        oca.SetLayout(ocapar.Row, ocapar.Column, ocapar.StartPosition, ocapar.Vertical, ocapar.AreaName);
                        return oca;
                    }
                // ...其他类型...
                default: return null;
            }
        }
    }

    public class IniFile
    {
        private readonly string _path;
        public IniFile(string path) { _path = path; }

        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static  extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        public void WriteValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, _path);
        }
    }
}