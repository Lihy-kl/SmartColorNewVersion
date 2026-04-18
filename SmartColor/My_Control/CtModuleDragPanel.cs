using SmartColor.My_Form.ConPar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 可拖拽模块面板控件，支持模块拖拽、排序、删除及保存顺序。
    /// </summary>
    public partial class CtModuleDragPanel : UserControl
    {
        // 工具箱面板，存放可拖拽的模块按钮
        private FlowLayoutPanel flowPanelToolbox;
        // 工作区面板，存放已拖入的模块按钮
        private Panel panelWorkspace;
        // 保存按钮，仅在所有模块都拖入工作区时可见
        private Button btnSave;
        // 右键菜单，用于删除工作区中的模块
        private ContextMenuStrip contextMenuStrip1;

        private ContextMenuStrip contextMenuStripWorkspace;
        // 所有模块按钮的集合
        private List<Button> toolboxButtons = new List<Button>();
        // ToolTip控件，用于显示按钮提示
        private ToolTip toolTip1 = new ToolTip();
        private Type processType;
        /// <summary>
        /// 模块按钮点击事件，参数为模块名称
        /// </summary>
        public event EventHandler<string> ModuleButtonClicked;
        /// <summary>
        /// 保存按钮点击事件，参数为当前模块顺序
        /// </summary>
        public event EventHandler<List<ModuleInfo>> SaveClicked;

        public Func<Type, List<ModuleInfo>> GetModuleInfosFunc { get; set; }

        public bool AllModulesPlaced
        {
            get
            {
                // 工作区按钮数等于总模块数才算全部拖拽完成
                return panelWorkspace.Controls.Count == toolboxButtons.Count;
            }
        }

        /// <summary>
        /// 构造函数，初始化控件及布局
        /// </summary>
        public CtModuleDragPanel()
        {
            InitializeComponent();
            InitializeLayout();
        }

        public void SetProcessType(Type type)
        {
            this.processType = type;
        }

        /// <summary>
        /// 初始化控件布局，包括工具箱、工作区、保存按钮和右键菜单
        /// </summary>
        private void InitializeLayout()
        {
            // 工具箱面板设置
            flowPanelToolbox = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 90,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10, 15, 10, 15),
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 248, 255)
            };
            Controls.Add(flowPanelToolbox);

            // 工作区面板设置
            panelWorkspace = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 250, 250),
                AllowDrop = true
            };
            // 注册拖拽和重绘事件
            panelWorkspace.DragEnter += PanelWorkspace_DragEnter;
            panelWorkspace.DragDrop += PanelWorkspace_DragDrop;
            panelWorkspace.Paint += PanelWorkspace_Paint;
            panelWorkspace.Resize += (s, e) => LayoutWorkspaceButtons();
            Controls.Add(panelWorkspace);

            // 保存按钮设置
            btnSave = new Button
            {
                Text = "保存",
                Dock = DockStyle.Bottom,
                Height = 45,
                Visible = false,
                BackColor = Color.FromArgb(60, 140, 230),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("微软雅黑", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.MouseEnter += (s, e) => btnSave.BackColor = Color.FromArgb(80, 160, 250);
            btnSave.MouseLeave += (s, e) => btnSave.BackColor = Color.FromArgb(60, 140, 230);
            btnSave.Click += BtnSave_Click;
            Controls.Add(btnSave);

            // 右键菜单设置（仅包含删除项）
            contextMenuStrip1 = new ContextMenuStrip();
            var deleteItem = new ToolStripMenuItem("删除");
          
            deleteItem.Click += DeleteItem_Click;
          
            contextMenuStrip1.Items.Add(deleteItem);

            // 在 InitializeLayout() 里新增
            contextMenuStripWorkspace = new ContextMenuStrip();
            var resetItem = new ToolStripMenuItem("重置");
            var defaultItem = new ToolStripMenuItem("默认");
            resetItem.Click += RestItem_Click;
            defaultItem.Click += DefaultItem_Click;
            contextMenuStripWorkspace.Items.Add(resetItem);
            contextMenuStripWorkspace.Items.Add(defaultItem);

            // 注册工作区右键事件
            panelWorkspace.MouseUp += PanelWorkspace_MouseUp;

        }

        private void PanelWorkspace_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // 只在空白处弹出
                var ctrl = panelWorkspace.GetChildAtPoint(e.Location);
                if (ctrl == null)
                    contextMenuStripWorkspace.Show(panelWorkspace, e.Location);
            }
        }

        /// <summary>
        /// 创建美化后的模块按钮
        /// </summary>
        /// <param name="text">按钮显示文本</param>
        /// <param name="idx">按钮索引</param>
        /// <returns>生成的Button控件</returns>
        private Button CreateModuleButton(string text, int idx)
        {
            var btn = new Button
            {
                Text = text,
                Width = 120,
                Height = 40,
                Tag = idx,
                Margin = new Padding(10, 10, 10, 10),
                BackColor = Color.FromArgb(60, 140, 230),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("微软雅黑", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseDown += ToolboxButton_MouseDown;
            btn.Click += ModuleButton_Click;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(80, 160, 250);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(60, 140, 230);
            toolTip1.SetToolTip(btn, text);
            return btn;
        }

        /// <summary>
        /// 初始化模块列表，生成工具箱按钮并添加到工作区
        /// </summary>
        /// <param name="modules">模块信息列表</param>
        public void InitModules(List<ModuleInfo> modules)
        {
            flowPanelToolbox.Controls.Clear();
            panelWorkspace.Controls.Clear();
            toolboxButtons.Clear();
            btnSave.Visible = false;

            int idx = 1;
            foreach (var module in modules)
            {
                var btn = CreateModuleButton(module.Name, idx++);
                btn.Tag = module; // 绑定ModuleInfo
                toolboxButtons.Add(btn);

                // 绑定事件和右键菜单
                btn.MouseDown -= ToolboxButton_MouseDown;
                btn.Click -= ModuleButton_Click;
                btn.Click += ModuleButton_Click;
                btn.ContextMenuStrip = contextMenuStrip1;
                panelWorkspace.Controls.Add(btn);
            }
            LayoutWorkspaceButtons();
            panelWorkspace.Invalidate();
            btnSave.Visible = panelWorkspace.Controls.Count == toolboxButtons.Count;
            AdjustParentFormWidth();
            AdjustParentFormHeight();
        }

        /// <summary>
        /// 工具箱按钮鼠标按下事件，启动拖拽操作
        /// </summary>
        private void ToolboxButton_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
                DoDragDrop(btn, DragDropEffects.Move);
        }

        /// <summary>
        /// 工作区拖拽进入事件，判断拖拽对象类型
        /// </summary>
        private void PanelWorkspace_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Button)))
                e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// 工作区拖拽释放事件，将按钮从工具箱移动到工作区
        /// </summary>
        private void PanelWorkspace_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Button)))
            {
                Button btn = (Button)e.Data.GetData(typeof(Button));
                if (!panelWorkspace.Controls.Contains(btn))
                {
                    // 从工具箱移除，添加到工作区
                    flowPanelToolbox.Controls.Remove(btn);
                    panelWorkspace.Controls.Add(btn);

                    // 移除工具箱事件，添加右键菜单
                    btn.MouseDown -= ToolboxButton_MouseDown;
                    btn.Click -= ModuleButton_Click;
                    btn.Click += ModuleButton_Click;
                    btn.ContextMenuStrip = contextMenuStrip1;

                    LayoutWorkspaceButtons();
                    panelWorkspace.Invalidate();
                }
            }
            // 所有模块都拖入后显示保存按钮
            btnSave.Visible = panelWorkspace.Controls.Count == toolboxButtons.Count;
            AdjustParentFormHeight();
        }

        /// <summary>
        /// 右键菜单删除事件，将按钮从工作区移回工具箱
        /// </summary>
        private void DeleteItem_Click(object sender, EventArgs e)
        {
            if (contextMenuStrip1.SourceControl is Button btn)
            {
                panelWorkspace.Controls.Remove(btn);
                btn.ContextMenuStrip = null;
                btn.MouseDown += ToolboxButton_MouseDown;
                btn.Click -= ModuleButton_Click;
                btn.Click += ModuleButton_Click;
                flowPanelToolbox.Controls.Add(btn);
                LayoutWorkspaceButtons();
                panelWorkspace.Invalidate();
                btnSave.Visible = panelWorkspace.Controls.Count == toolboxButtons.Count;
                AdjustParentFormHeight();
                 AdjustParentFormWidth(); // 新增：删除后动态调整父窗体宽度
            }
        }

        private void RestItem_Click(object sender, EventArgs e)
        {
            // 将所有已拖入工作区的按钮移回工具箱
            var buttonsToMove = panelWorkspace.Controls.OfType<Button>().ToList();
            foreach (var btn in buttonsToMove)
            {
                panelWorkspace.Controls.Remove(btn);
                btn.ContextMenuStrip = null;
                btn.MouseDown += ToolboxButton_MouseDown;
                btn.Click -= ModuleButton_Click;
                btn.Click += ModuleButton_Click;
                flowPanelToolbox.Controls.Add(btn);
            }
            LayoutWorkspaceButtons();
            panelWorkspace.Invalidate();
            btnSave.Visible = panelWorkspace.Controls.Count == toolboxButtons.Count;
            AdjustParentFormHeight();
            AdjustParentFormWidth();
        }

        private void DefaultItem_Click(object sender, EventArgs e)
        {
            if (processType == null)
                return;

            // 1. 调用对应类型的 RestoreDefault
            var restoreMethod = processType.GetMethod("RestoreDefault", BindingFlags.Public | BindingFlags.Static);
            if (restoreMethod != null)
                restoreMethod.Invoke(null, null);

            // 2. 重新获取模块信息并刷新面板
            // 这里假设有一个委托或回调能获取最新的模块列表（如OrderInfo传进来）
            if (GetModuleInfosFunc != null)
            {
                var modules = GetModuleInfosFunc(processType)
                    .OrderBy(m => m.Order)
                    .ToList();
                InitModules(modules);
            }
            else
            {
                // 如果没有委托，建议在SetProcessType时传入或用事件/回调
                MessageBox.Show("未设置模块信息获取方法，无法刷新默认顺序。");
            }
        }

        /// <summary>
        /// 重新布局工作区内的按钮，垂直居中排列并绘制连线
        /// </summary>
        private void LayoutWorkspaceButtons()
        {
            int totalHeight = 0;
            int spacing = 30;
            foreach (Control ctrl in panelWorkspace.Controls)
                totalHeight += ctrl.Height + spacing;
            if (panelWorkspace.Controls.Count > 0)
                totalHeight -= spacing;

            int offsetY = 30; // 向下偏移30像素
            int startY = (panelWorkspace.ClientSize.Height - totalHeight) / 2 + offsetY;
            int centerX = panelWorkspace.ClientSize.Width / 2;

            int y = startY;
            foreach (Control ctrl in panelWorkspace.Controls)
            {
                ctrl.Top = y;
                ctrl.Left = centerX - ctrl.Width / 2;
                y += ctrl.Height + spacing;
            }
            panelWorkspace.Invalidate();
        }

        /// <summary>
        /// 工作区重绘事件，绘制模块之间的箭头连线
        /// </summary>
        private void PanelWorkspace_Paint(object sender, PaintEventArgs e)
        {
            var controls = panelWorkspace.Controls;
            for (int i = 0; i < controls.Count - 1; i++)
            {
                Control c1 = controls[i];
                Control c2 = controls[i + 1];
                Point p1 = new Point(c1.Left + c1.Width / 2, c1.Bottom);
                Point p2 = new Point(c2.Left + c2.Width / 2, c2.Top);
                DrawArrow(e.Graphics, p1, p2);
            }
        }

        /// <summary>
        /// 绘制带箭头的连线
        /// </summary>
        /// <param name="g">绘图对象</param>
        /// <param name="p1">起点</param>
        /// <param name="p2">终点</param>
        private void DrawArrow(Graphics g, Point p1, Point p2)
        {
            using (var pen = new Pen(Color.Red, 2))
            {
                pen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(4, 8);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawLine(pen, p1, p2);
            }
        }

        /// <summary>
        /// 模块按钮点击事件，触发外部事件并弹窗提示
        /// </summary>
        private void ModuleButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            // 触发外部事件
            ModuleButtonClicked?.Invoke(this, btn.Text);
           
        }

        /// <summary>
        /// 保存按钮点击事件，触发外部保存事件并弹窗显示顺序
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        public DialogResult Save()
        {
            DialogResult dialogResult = My_File.LocalTranslator.ShowMessage(
               "确认保存当前模块顺序吗？",
               "确认保存",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
            {
                return DialogResult.Cancel;

            }

            var orderList = panelWorkspace.Controls
                .OfType<Button>()
                .Select(btn => btn.Tag as ModuleInfo)
                .Where(m => m != null)
                .ToList();
            // 关键：同步更新Order字段
            for (int i = 0; i < orderList.Count; i++)
            {
                orderList[i].Order = i + 1;
            }
            SaveClicked?.Invoke(this, orderList);
            return DialogResult.OK;
        }

        /// <summary>
        /// 动态调整父窗体宽度，保证工具箱只占一行
        /// </summary>
        private void AdjustParentFormWidth()
        {
            int totalWidth = flowPanelToolbox.Padding.Left + flowPanelToolbox.Padding.Right;
            foreach (Control ctrl in flowPanelToolbox.Controls)
                totalWidth += ctrl.Width + ctrl.Margin.Left + ctrl.Margin.Right;

            // 预留滚动条宽度
            totalWidth += SystemInformation.VerticalScrollBarWidth;

            // 获取父窗体并调整宽度
            Form parentForm = this.FindForm();
            if (parentForm != null)
            {
                int minWidth = 400; // 可自定义最小宽度
                int newWidth = Math.Max(totalWidth, minWidth);
                parentForm.Width = newWidth + (parentForm.Width - parentForm.ClientSize.Width);
            }
        }

        /// <summary>
        /// 动态调整父窗体高度，保证工作区所有按钮都能完整显示
        /// </summary>
        private void AdjustParentFormHeight()
        {
            int spacing = 30;
            int workspaceButtonsHeight = 0;
            int count = 0;
            int marginSum = 0;
            foreach (Control ctrl in panelWorkspace.Controls)
            {
                if (ctrl is Button btn)
                {
                    workspaceButtonsHeight += btn.Height;
                    // 累加按钮的上下 Margin
                    marginSum += btn.Margin.Top + btn.Margin.Bottom;
                    count++;
                }
            }
            if (count > 0)
                workspaceButtonsHeight += spacing * (count - 1);

            // 加上所有按钮的 Margin
            workspaceButtonsHeight += marginSum;

            // 加上 panelWorkspace 的 Padding
            workspaceButtonsHeight += panelWorkspace.Padding.Top + panelWorkspace.Padding.Bottom;

            int minWorkspaceHeight = 100; // 最小工作区高度
            int workspaceHeight = Math.Max(workspaceButtonsHeight, minWorkspaceHeight);

            // 加上 flowPanelToolbox、btnSave 的高度和 Margin
            int totalHeight = flowPanelToolbox.Height
                + flowPanelToolbox.Margin.Top + flowPanelToolbox.Margin.Bottom
                + workspaceHeight
                + btnSave.Height
                + btnSave.Margin.Top + btnSave.Margin.Bottom;

            // 获取父窗体并调整高度
            Form parentForm = this.FindForm();
            if (parentForm != null)
            {
                int minHeight = 300; // 可自定义最小高度
                int newHeight = Math.Max(totalHeight, minHeight);
                parentForm.Height = newHeight + (parentForm.Height - parentForm.ClientSize.Height);
                parentForm.PerformLayout(); // 强制刷新布局
            }
        }
    }
}