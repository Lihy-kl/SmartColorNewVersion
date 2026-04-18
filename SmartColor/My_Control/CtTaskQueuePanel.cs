using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SmartColor.My_File;
using SmartColor.My_RobotManager;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 任务队列可视化控件（右侧任务栏，支持拖拽排序，最大化/最小化为左右展开/收起，参考CtInteractiveStatusStrip风格）
    /// </summary>
    public partial class CtTaskQueuePanel : UserControl
    {
        private const int MinimizedWidth = 12;
        private const int MaximizedWidth = 290;

        private FlowLayoutPanel _panel;
        private TrapezoidButton _toggleButton;
        private bool _isMinimized = true;
        private Point _dragStartPoint = Point.Empty;

        private List<object> _tasks = new List<object>();
        private readonly object _tasksLock = new object();
        private SynchronizationContext _uiContext;
      

        private readonly System.Windows.Forms.Timer _throttleTimer;
        private const int ThrottleIntervalMs = 80; // 节流间隔，按需调整

        // 控件池：未使用的CtTask控件
        private readonly Stack<CtTask> _taskPool = new Stack<CtTask>();
        // 当前任务与控件的映射
        private readonly Dictionary<object, CtTask> _taskControlMap = new Dictionary<object, CtTask>();

        public event Action<List<object>> TaskOrderChanged;

        public CtTaskQueuePanel()
        {
            InitializeComponent();
            _uiContext = SynchronizationContext.Current;

            // 主面板
            _panel = new CtDoubleBufferedFlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                ContextMenuStrip = this.contextMenuStrip1,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(12, 0, 0, 0)
            };
            this.Controls.Add(_panel);

            // 倒梯形按钮
            _toggleButton = new TrapezoidButton
            {
                Visible = false,
                IsMinimized = _isMinimized,
                Size = new Size(12, 120)
            };
            _toggleButton.Clicked += (s, e) =>
            {
                if (_isMinimized)
                    Maximize();
                else
                    Minimize();
            };
            this.Controls.Add(_toggleButton);

            // this.Visible = false;
            _panel.Visible = false;
            _isMinimized = true;
            _toggleButton.IsMinimized = true;

            this.Resize += (s, e) => UpdateButtonPosition();

            _panel.AllowDrop = true;
            _panel.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(typeof(CtTask)))
                    e.Effect = DragDropEffects.Move;
            };
            _panel.DragDrop += (s, e) =>
            {
                var src = e.Data.GetData(typeof(CtTask)) as CtTask;
                if (src != null)
                {
                    Point p = _panel.PointToClient(new Point(e.X, e.Y));
                    int insertIndex = _panel.Controls.Count;
                    for (int i = 0; i < _panel.Controls.Count; i++)
                    {
                        var c = _panel.Controls[i];
                        if (p.Y < c.Top + c.Height / 2)
                        {
                            insertIndex = i;
                            break;
                        }
                    }
                    var currentTask = SmartColor.My_RobotManager.RobotTaskManager.Instance.CurrentTask;
                    if (src.Tag != null && src.Tag != currentTask)
                    {
                        if (_panel.Controls.Contains(src))
                        {
                            _panel.Controls.Remove(src);
                        }
                        if (insertIndex > _panel.Controls.Count)
                            insertIndex = _panel.Controls.Count;
                        _panel.Controls.Add(src);
                        _panel.Controls.SetChildIndex(src, insertIndex);

                        var newOrder = _panel.Controls.OfType<CtTask>()
                            .Select(c => c.Tag)
                            .Where(t => t != currentTask)
                            .ToList();
                        TaskOrderChanged?.Invoke(newOrder);
                    }
                }
            };

            _throttleTimer = new System.Windows.Forms.Timer();
            _throttleTimer.Interval = ThrottleIntervalMs;
            _throttleTimer.Tick += _throttleTimer_Tick;
        }

        private void _throttleTimer_Tick(object sender, EventArgs e)
        {
            _throttleTimer.Stop();
            List<object> tasksCopy;
            lock (_tasksLock)
            {
                tasksCopy = _tasks.ToList();
            }
            DoUpdateTasks(tasksCopy);
        }

        /// <summary>
        /// 线程安全高性能刷新任务队列（合并高频调用，UI线程只做最小操作）
        /// </summary>
        public void UpdateTasks(List<object> tasks)
        {
            lock (_tasksLock)
            {
                _tasks = tasks.ToList();
            }
            // 重置节流定时器
            if (!_throttleTimer.Enabled)
                _throttleTimer.Start();
            else
            {
                _throttleTimer.Stop();
                _throttleTimer.Start();
            }
        }

        /// <summary>
        /// 增量更新任务控件，支持控件池复用
        /// </summary>
        private void DoUpdateTasks(List<object> tasks)
        {
            if (this.IsDisposed)
                return;



            // 保证句柄已创建
            if (!this.IsHandleCreated)
                this.CreateControl();
            if (!_panel.IsHandleCreated)
                _panel.CreateControl();

            var currentTask = SmartColor.My_RobotManager.RobotTaskManager.Instance.CurrentTask;
            var newTaskSet = new HashSet<object>(tasks);

            // 1. 移除不再存在的任务控件，放入控件池
            var toRemove = _taskControlMap.Keys.Except(newTaskSet).ToList();
            foreach (var key in toRemove)
            {
                if (_taskControlMap.TryGetValue(key, out var ctl))
                {
                    _panel.Controls.Remove(ctl);
                    ctl.Visible = false;
                    ctl.Tag = null;
                    _taskPool.Push(ctl); // 放入池
                }
                _taskControlMap.Remove(key);
            }

            // 2. 按顺序插入/更新控件
            int idx = 1;
            for (int i = 0; i < tasks.Count; i++)
            {
                var t = tasks[i];
                CtTask ct;
                if (!_taskControlMap.TryGetValue(t, out ct) || ct.IsDisposed)
                {
                    // 优先从池中取
                    if (_taskPool.Count > 0)
                    {
                        ct = _taskPool.Pop();
                        ct.Visible = true;
                        ct.Tag = t;
                        // 重新初始化显示内容
                        ct.ResetContent(idx.ToString(), t);
                    }
                    else
                    {
                        ct = new CtTask(idx.ToString(), t);
                        ct.Tag = t;
                        // 拖拽支持
                        ct.AllowDrop = false;
                        ct.MouseDown += (s, e) =>
                        {
                            if (e.Button == MouseButtons.Left)
                            {
                                _dragStartPoint = e.Location;
                            }
                        };
                        ct.MouseMove += (s, e) =>
                        {
                            if (e.Button == MouseButtons.Left)
                            {
                                if (Math.Abs(e.X - _dragStartPoint.X) > SystemInformation.DragSize.Width / 2 ||
                                    Math.Abs(e.Y - _dragStartPoint.Y) > SystemInformation.DragSize.Height / 2)
                                {
                                    ct.DoDragDrop(ct, DragDropEffects.Move);
                                }
                            }
                        };
                    }
                    _taskControlMap[t] = ct;
                }
                else
                {
                    // 只更新显示内容
                    ct.ResetContent(idx.ToString(), t);
                }

                // 更新状态
                if (currentTask != null && t == currentTask && idx == 1)
                    ct.UpdateStatus(true);
                else
                    ct.UpdateStatus(false);

                // 保证顺序
                if (!_panel.Controls.Contains(ct))
                {
                    _panel.Controls.Add(ct);
                }
                _panel.Controls.SetChildIndex(ct, i);
                idx++;
            }

            // 3. 移除多余控件（如果有）
            while (_panel.Controls.Count > tasks.Count)
            {
                var ctl = _panel.Controls[_panel.Controls.Count - 1];
                if (ctl is CtTask ctTask)
                {
                    _panel.Controls.RemoveAt(_panel.Controls.Count - 1);
                    if (!_taskControlMap.Values.Contains(ctTask))
                    {
                        ctTask.Visible = false;
                        ctTask.Tag = null;
                        _taskPool.Push(ctTask);
                    }
                }
                else
                {
                    _panel.Controls.RemoveAt(_panel.Controls.Count - 1);
                }
            }

            _tasks = tasks.ToList();
            if (_tasks.Count > 0)
            {
                this.Visible = true;
                // Maximize(); // 自动展开
            }
            else
            {
                this.Visible = false;
            }
            UpdateToggleButton();
        }

        public void Minimize()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(Minimize));
                return;
            }

            _isMinimized = true;
            _toggleButton.IsMinimized = true;
            _panel.Visible = false;
            this.Width = MinimizedWidth;
            UpdateToggleButton();
        }

        public void Maximize()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(Maximize));
                return;
            }

            _isMinimized = false;
            _toggleButton.IsMinimized = false;
            _panel.Visible = true;
            this.Width = MaximizedWidth;

            UpdateToggleButton();
        }

        private void UpdateToggleButton()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateToggleButton));
                return;
            }

            if (_tasks.Count == 0)
            {
                _toggleButton.Visible = false;
                this.Visible = false;
                return;
            }

            this.Visible = true;
            _toggleButton.Visible = true;
            _toggleButton.Size = new Size(12, 120);
            _toggleButton.Invalidate();
            UpdateButtonPosition();
        }

        private void UpdateButtonPosition()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateButtonPosition));
                return;
            }

            int x = 2;
            int y = (this.Height - _toggleButton.Height) / 2;
            _toggleButton.Location = new Point(x, y);
            _toggleButton.BringToFront();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0)
                return;
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(210, 240, 255),
                Color.FromArgb(170, 210, 240),
                0f))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void TSMIResetDefOrder_Click(object sender, EventArgs e)
        {
            SmartColor.My_RobotManager.RobotTaskManager.Instance.ResetToPriorityOrder();
        }

        /// <summary>
        /// 内部类：倒梯形按钮（负责最小化/最大化切换）
        /// </summary>
        private class TrapezoidButton : Control
        {
            public event EventHandler Clicked;
            private bool _hover = false;
            private bool _pressed = false;
            private bool _isMinimized = true;

            public bool IsMinimized
            {
                get => _isMinimized;
                set { _isMinimized = value; Invalidate(); }
            }

            public TrapezoidButton()
            {
                this.SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.SupportsTransparentBackColor,
                    true);
                this.BackColor = Color.Transparent;
                this.Size = new Size(12, 120);
                this.Cursor = Cursors.Hand;
            }

            protected override void OnPaintBackground(PaintEventArgs pevent)
            {
                if (Parent != null)
                {
                    var e = pevent.Graphics;
                    var parentRect = this.Bounds;
                    var state = e.Save();
                    e.TranslateTransform(-this.Left, -this.Top);
                    PaintEventArgs pea = new PaintEventArgs(e, parentRect);
                    InvokePaintBackground(Parent, pea);
                    InvokePaint(Parent, pea);
                    e.Restore(state);
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = this.ClientRectangle;

                var path = new System.Drawing.Drawing2D.GraphicsPath();
                int leftWidth = (int)(rect.Height * 0.95);
                int rightWidth = (int)(rect.Height * 0.55);
                int width = rect.Width;
                int leftY = (rect.Height - leftWidth) / 2;
                int rightY = (rect.Height - rightWidth) / 2;
                path.AddPolygon(new Point[]
                {
                    new Point(0, leftY),
                    new Point(width, rightY),
                    new Point(width, rightY + rightWidth),
                    new Point(0, leftY + leftWidth)
                });

                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    rect,
                    _hover ? Color.FromArgb(0, 220, 255) : Color.FromArgb(120, 180, 255),
                    Color.FromArgb(180, 230, 255),
                    0f))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(Color.FromArgb(0, 220, 255), _hover ? 3 : 2))
                {
                    g.DrawPath(pen, path);
                }

                string icon = _isMinimized ? "▶" : "◀";
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (var f = new Font("Segoe UI", 18, FontStyle.Bold))
                using (var b = new SolidBrush(Color.White))
                {
                    g.DrawString(icon, f, b, rect, sf);
                }
            }

            protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); }
            protected override void OnMouseLeave(EventArgs e) { _hover = false; _pressed = false; Invalidate(); }
            protected override void OnMouseDown(MouseEventArgs e) { _pressed = true; Invalidate(); }
            protected override void OnMouseUp(MouseEventArgs e)
            {
                if (_pressed && _hover)
                    Clicked?.Invoke(this, EventArgs.Empty);
                _pressed = false;
                Invalidate();
            }
        }
    }

    // CtTask 扩展方法：重置内容（用于控件池复用）
    public static class CtTaskExtensions
    {
        public static void ResetContent(this CtTask ct, string index, object task)
        {
            ct.SetTask(index, task);
            ct.Tag = task;
        }
    }
}