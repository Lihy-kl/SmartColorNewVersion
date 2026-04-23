using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 多行交互状态栏控件（科技感美化版，上方悬浮倒梯形按钮，自动高度，带动画，支持语音循环播报）
    /// </summary>
    public partial class CtInteractiveStatusStrip : UserControl
    {
        private readonly Queue<(string key, string text, int intervalMs)> _speakQueue = new Queue<(string, string, int)>();
        private readonly HashSet<string> _speakKeys = new HashSet<string>();
        private readonly object _speakLock = new object();
        private CancellationTokenSource _speakQueueCts = null;
        private SpeechSynthesizer _queueSynth = null;


        // 内部类：双缓冲FlowLayoutPanel，防止控件闪烁
        private class DoubleBufferedFlowLayoutPanel : FlowLayoutPanel
        {
            public DoubleBufferedFlowLayoutPanel()
            {
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
                this.UpdateStyles();
            }
        }

        // 内部类：倒梯形按钮控件（上宽下窄，短边在下）
        private class TrapezoidButton : Control
        {
            public event EventHandler Clicked; // 点击事件
            private bool _hover = false;       // 鼠标悬停状态
            private bool _pressed = false;     // 鼠标按下状态
            private bool _isMinimized = true;  // 是否最小化
            [Browsable(false)]
            public bool IsMinimized
            {
                get => _isMinimized;
                set { _isMinimized = value; Invalidate(); }
            }

            public TrapezoidButton()
            {
                // 设置控件样式，支持透明和双缓冲
                this.SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.SupportsTransparentBackColor,
                    true);
                this.BackColor = Color.Transparent;
                this.Size = new Size(64, 32);
                this.Cursor = Cursors.Hand;
            }

            // 绘制背景（透明，显示父控件背景）
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

            // 绘制倒梯形按钮
            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = this.ClientRectangle;

                // 构造倒梯形路径
                var path = new System.Drawing.Drawing2D.GraphicsPath();
                int topWidth = (int)(rect.Width * 0.95);
                int bottomWidth = (int)(rect.Width * 0.55);
                int height = rect.Height;
                int topX = (rect.Width - topWidth) / 2;
                int bottomX = (rect.Width - bottomWidth) / 2;
                path.AddPolygon(new Point[]
                {
                    new Point(topX, 0),
                    new Point(topX + topWidth, 0),
                    new Point(bottomX + bottomWidth, height),
                    new Point(bottomX, height)
                });

                // 渐变填充按钮
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    rect,
                    _hover ? Color.FromArgb(0, 220, 255) : Color.FromArgb(120, 180, 255),
                    Color.FromArgb(180, 230, 255),
                    90f))
                {
                    g.FillPath(brush, path);
                }

                // 发光边框
                using (var pen = new Pen(Color.FromArgb(0, 220, 255), _hover ? 3 : 2))
                {
                    g.DrawPath(pen, path);
                }

                // 绘制箭头（根据最小化状态切换方向）
                string icon = _isMinimized ? "▼" : "▲";
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (var f = new Font("Segoe UI", 16, FontStyle.Bold))
                using (var b = new SolidBrush(Color.White))
                {
                    g.DrawString(icon, f, b, rect, sf);
                }
            }

            // 鼠标交互相关事件
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

        // 主内容面板，承载所有交互行
        private FlowLayoutPanel _panel = new DoubleBufferedFlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            BackColor = Color.Transparent
        };

        private bool _isMinimized = false;         // 当前是否最小化
        private TrapezoidButton _toggleButton;     // 悬浮倒梯形按钮
        private int _maximizedMaxHeight = 220;     // 最大化最大高度

        // 按钮尺寸
        private readonly Size _toggleButtonSizeMinimized = new Size(120, 24); // 最小化时按钮尺寸
        private readonly Size _toggleButtonSizeMaximized = new Size(120, 24);  // 最大化时按钮尺寸

        // 语音播报相关字段
        private SpeechSynthesizer _synthesizer;
        private string _lastSpeakText = "";
        private bool _isSpeaking = false;
        private bool _stopSpeakLoop = false;
        private CancellationTokenSource _speakCts = null;

        /// <summary>
        /// 构造函数，初始化控件
        /// </summary>
        public CtInteractiveStatusStrip()
        {
            // 设置控件样式，防止闪烁
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            this.BorderStyle = BorderStyle.None;
            this.BackColor = Color.Transparent; // 柔和蓝色渐变背景

            this.Controls.Add(_panel);

            // 初始化倒梯形按钮
            _toggleButton = new TrapezoidButton
            {
                Visible = false,
                IsMinimized = _isMinimized,
                Size = _toggleButtonSizeMaximized
            };
            _toggleButton.Clicked += (s, e) =>
            {
                if (_isMinimized)
                    Maximize();
                else
                    Minimize();
            };
            this.Controls.Add(_toggleButton);

            this.Resize += (s, e) => UpdateButtonPosition();

            // 初始化语音合成器
            _synthesizer = new SpeechSynthesizer();
        }

        /// <summary>
        /// 语音循环播报（async/await实现，防止多定时器和多线程问题）
        /// </summary>
        private async void StartSpeakLoop()
        {


            string speakText = GetAllSpeakText();
            if (string.IsNullOrWhiteSpace(speakText))
            {
                _stopSpeakLoop = true;
                _isSpeaking = false;
                _synthesizer.SpeakAsyncCancelAll();
                return;
            }
            if (_isSpeaking && speakText == _lastSpeakText) return;
            _stopSpeakLoop = false;
            _lastSpeakText = speakText;
            _isSpeaking = true;

            // 取消上一次的循环
            _speakCts?.Cancel();
            _speakCts = new CancellationTokenSource();
            var token = _speakCts.Token;

            try
            {
                while (!_stopSpeakLoop && !token.IsCancellationRequested)
                {
                    var tcs = new TaskCompletionSource<bool>();
                    EventHandler<SpeakCompletedEventArgs> handler = (s, e) => tcs.TrySetResult(true);
                    _synthesizer.SpeakCompleted += handler;
                    try
                    {
                        _synthesizer.SpeakAsync(speakText);
                        await tcs.Task;
                    }
                    finally
                    {
                        _synthesizer.SpeakCompleted -= handler;
                    }

                    if (_stopSpeakLoop || token.IsCancellationRequested) break;
                    await Task.Delay(2000, token);

                    speakText = GetAllSpeakText();
                    if (string.IsNullOrWhiteSpace(speakText))
                    {
                        _isSpeaking = false;
                        break;
                    }
                    _lastSpeakText = speakText;
                }
            }
            catch (TaskCanceledException)
            {
                // 忽略取消异常
            }
            catch (Exception)
            {
                // 可选：记录日志
            }
            _isSpeaking = false;
        }

        /// <summary>
        /// 获取所有内容文本
        /// </summary>
        private string GetAllSpeakText()
        {
            if (_panel.Controls.Count == 0) return "";
            string speakText = "";
            foreach (FlowLayoutPanel row in _panel.Controls)
            {
                foreach (Control c in row.Controls)
                {
                    if (c is Label lbl)
                        speakText += lbl.Text + "，";
                }
            }
            // 只保留常用中英文、数字和常用标点
            speakText = System.Text.RegularExpressions.Regex.Replace(
                speakText,
                @"[^\u4e00-\u9fa5a-zA-Z0-9，。,.!?！？:：;；\s]", // 允许的字符
                ""
            );

            if (speakText.Contains("重滴"))
                speakText = speakText.Replace("重滴", "虫滴");
            return speakText;
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stopSpeakLoop = true;
                _speakCts?.Cancel();
                _synthesizer?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 绘制控件背景（柔和蓝色渐变）
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(210, 240, 255), // 顶部亮蓝
                Color.FromArgb(170, 210, 240), // 底部淡蓝
                90f))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }


        /// <summary>
        /// 添加一行交互内容
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="onResult">按钮回调</param>
        /// <param name="buttons">按钮文本数组</param>
        /// <returns>交互对象</returns>
        public FlowLayoutPanel AddInteraction(string title, string content, Action<string> onResult, params string[] buttons)
        {
            // 判断是否已存在相同 title 和 content 的行，存在则直接返回
            foreach (FlowLayoutPanel row in _panel.Controls)
            {
                Label titleLabel = null;
                Label contentLabel = null;
                int labelCount = 0;
                foreach (Control c in row.Controls)
                {
                    if (c is Label lbl)
                    {
                        if (labelCount == 0) titleLabel = lbl;
                        else if (labelCount == 1) contentLabel = lbl;
                        labelCount++;
                    }
                }
                if (titleLabel != null && contentLabel != null &&
                    titleLabel.Text == title && contentLabel.Text == content)
                {
                    return row; // 已存在，直接返回
                }
            }

            var rowPanel = CreateInteractionRow(title, content, onResult, buttons);
            _panel.Controls.Add(rowPanel);
            UpdateToggleButton();
            UpdatePanelHeight();
            StartSpeakLoop();
            return rowPanel;
        }

        /// <summary>
        /// 移除一行交互内容
        /// </summary>
        /// <param name="row">交互对象</param>
        public void RemoveInteractionRow(FlowLayoutPanel row)
        {
            if (row != null && _panel.Controls.Contains(row))
            {
                _panel.Controls.Remove(row);
                UpdateToggleButton();
                UpdatePanelHeight();
                StartSpeakLoop();
            }
        }


        /// <summary>
        /// 创建一行交互内容的面板
        /// </summary>
        private FlowLayoutPanel CreateInteractionRow(string title, string content, Action<string> onResult, string[] buttons)
        {
            var rowPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                BackColor = Color.FromArgb(60, 120, 180, 180), // 柔和蓝色半透明
                Margin = new Padding(0, 6, 0, 6),
                Padding = new Padding(14, 8, 14, 8),
                BorderStyle = BorderStyle.None
            };

            // 自定义绘制圆角背景和边框
            rowPanel.Paint += (s, e) =>
            {
                if (!rowPanel.Visible)
                    return;
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var rect = rowPanel.ClientRectangle;
                rect.Inflate(-1, -1);

                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 14;
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                    path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                    path.CloseFigure();

                    using (var brush = new SolidBrush(rowPanel.BackColor))
                        g.FillPath(brush, path);

                    using (var pen = new Pen(Color.FromArgb(0, 220, 255), 2))
                        g.DrawPath(pen, path);
                }
            };

            // 标题标签
            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Red,
                AutoSize = true,
                Margin = new Padding(0, 6, 12, 0)
            };
            rowPanel.Controls.Add(titleLabel);

            // 内容标签
            var contentLabel = new Label
            {
                Text = content,
                Font = new Font("Segoe UI", 12, FontStyle.Bold), // 更大更粗
                ForeColor = Color.Black,
                AutoSize = true,
                Margin = new Padding(0, 6, 12, 0)
            };
            rowPanel.Controls.Add(contentLabel);

            // 添加按钮
            foreach (var btnText in buttons)
            {
                var btn = new Button
                {
                    Text = btnText,
                    AutoSize = true,
                    Margin = new Padding(4, 2, 4, 2),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(80, 140, 200),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 220, 255, 30);
                btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 220, 255, 60);

                // 自定义按钮圆角渐变
                btn.Paint += (s, e) =>
                {
                    var g = e.Graphics;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    var rect = btn.ClientRectangle;
                    rect.Inflate(-1, -1);
                    using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                    {
                        int radius = 10;
                        path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                        path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                        path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                        path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                        path.CloseFigure();

                        using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                            rect, Color.FromArgb(80, 140, 200), Color.FromArgb(0, 220, 255, 40), 45f))
                        {
                            g.FillPath(brush, path);
                        }
                        using (var pen = new Pen(Color.FromArgb(0, 220, 255), 1))
                            g.DrawPath(pen, path);
                    }
                };

                // 按钮点击事件：移除行并回调
                btn.Click += (s, e) =>
                {
                    _panel.Controls.Remove(rowPanel);
                    UpdateToggleButton();
                    UpdatePanelHeight();
                    onResult?.Invoke(btnText);
                    StartSpeakLoop();
                };
                rowPanel.Controls.Add(btn);
            }

            return rowPanel;
        }

        /// <summary>
        /// 清空所有交互行
        /// </summary>
        public void ClearInteractions()
        {
            _panel.Controls.Clear();
            UpdateToggleButton();
            UpdatePanelHeight();
            _stopSpeakLoop = true;
            _isSpeaking = false;
            _speakCts?.Cancel();
            _synthesizer.SpeakAsyncCancelAll();
        }

        /// <summary>
        /// 最小化状态栏：只显示按钮，隐藏内容面板
        /// </summary>
        public void Minimize()
        {
            _isMinimized = true;
            _toggleButton.IsMinimized = true;
            _panel.Visible = false; // 隐藏内容面板
            _toggleButton.Visible = true;
            _toggleButton.Size = _toggleButtonSizeMinimized; // 更宽更扁
            UpdateButtonPosition();
            UpdatePanelHeight();
        }

        /// <summary>
        /// 最大化状态栏：显示内容面板，按钮恢复原尺寸
        /// </summary>
        public void Maximize()
        {
            _isMinimized = false;
            _toggleButton.IsMinimized = false;
            _panel.Visible = true; // 显示内容面板
            _toggleButton.Size = _toggleButtonSizeMaximized; // 恢复原尺寸
            UpdateToggleButton();
            UpdateButtonPosition();
            UpdatePanelHeight();
        }

        /// <summary>
        /// 更新倒梯形按钮的可见性和位置
        /// </summary>
        private void UpdateToggleButton()
        {
            // 没有任何行时隐藏按钮
            if (_panel.Controls.Count == 0)
            {
                _toggleButton.Visible = false;
                return;
            }
            _toggleButton.Visible = true;
            if (_isMinimized)
                _toggleButton.Size = _toggleButtonSizeMinimized;
            else
                _toggleButton.Size = _toggleButtonSizeMaximized;
            _toggleButton.Invalidate();
            UpdateButtonPosition();
        }

        /// <summary>
        /// 更新倒梯形按钮的位置（居中悬浮于顶部）
        /// </summary>
        private void UpdateButtonPosition()
        {
            int x = (this.Width - _toggleButton.Width) / 2;
            int y = 0; // 紧贴panel最上方
            _toggleButton.Location = new Point(x, y);
            _toggleButton.BringToFront();
        }

        /// <summary>
        /// 更新面板高度，支持动画
        /// </summary>
        private void UpdatePanelHeight()
        {
            int target;
            if (_isMinimized)
            {
                target = _toggleButton.Height + 8; // 只保留按钮高度
            }
            else
            {
                int total = 0;
                foreach (Control c in _panel.Controls)
                {
                    if (c.Visible)
                        total += c.Height + c.Margin.Top + c.Margin.Bottom;
                }
                int h = Math.Min(_maximizedMaxHeight, total + _toggleButton.Height + 24);
                target = Math.Max(_toggleButton.Height + 8, h);
            }
            this.Height = target;
        }

        /// <summary>
        /// 循环语音播报（可多杯同时播报，key区分）
        /// </summary>
        public void StartLoopSpeak(string key, string text, int intervalMs = 2000)
        {
            if (text.Contains("重滴"))
                text = text.Replace("重滴", "虫滴");
            lock (_speakLock)
            {
                if (_speakKeys.Contains(key))
                    return; // 已在队列中
                _speakQueue.Enqueue((key, text, intervalMs));
                _speakKeys.Add(key);
                Monitor.PulseAll(_speakLock);
            }
            EnsureSpeakQueueTask();
        }


        /// <summary>
        /// 停止指定key的循环语音播报
        /// </summary>
        public void StopLoopSpeak(string key)
        {
            lock (_speakLock)
            {
                _speakKeys.Remove(key);
                // 移除队列中所有该key的内容
                var list = new List<(string, string, int)>();
                while (_speakQueue.Count > 0)
                {
                    var item = _speakQueue.Dequeue();
                    if (item.key != key)
                        list.Add(item);
                }
                foreach (var item in list)
                    _speakQueue.Enqueue(item);
                Monitor.PulseAll(_speakLock);
            }
        }

        // 顺序播报队列
        private void EnsureSpeakQueueTask()
        {
            if (_queueSynth == null)
                _queueSynth = new SpeechSynthesizer();
            if (_speakQueueCts != null && !_speakQueueCts.IsCancellationRequested)
                return;
            _speakQueueCts = new CancellationTokenSource();
            var token = _speakQueueCts.Token;
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    (string key, string text, int intervalMs) item;
                    lock (_speakLock)
                    {
                        if (_speakQueue.Count == 0)
                        {
                            Monitor.Wait(_speakLock, 500); // 等待新内容
                            continue;
                        }
                        item = _speakQueue.Peek();
                    }
                    try
                    {
                        var tcs = new TaskCompletionSource<bool>();
                        EventHandler<SpeakCompletedEventArgs> handler = (s, e) => tcs.TrySetResult(true);
                        _queueSynth.SpeakCompleted += handler;
                        _queueSynth.SpeakAsync(item.text);
                        await tcs.Task;
                        _queueSynth.SpeakCompleted -= handler;
                    }
                    catch { }
                    await Task.Delay(item.intervalMs, token);

                    lock (_speakLock)
                    {
                        // 循环队列：移到队尾
                        if (_speakQueue.Count > 0 && _speakQueue.Peek().key == item.key)
                        {
                            _speakQueue.Dequeue();
                            if (_speakKeys.Contains(item.key))
                                _speakQueue.Enqueue(item);
                        }
                    }
                }
            }, token);
        }


    }
}