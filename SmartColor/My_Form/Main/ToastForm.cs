using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SmartColor.My_Form.Main
{
    
    public partial class ToastForm : Form
    {
        private Timer timer;
        private Label label;


        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_SHOWNOACTIVATE = 4;

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_NOACTIVATE = 0x08000000;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ShowWindow(this.Handle, SW_SHOWNOACTIVATE);
        }
        public ToastForm(string message, int offset, int durationMs = 2000)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = false;
            this.BackColor = Color.White; // 背景色更柔和
            this.Opacity = 0.95;
            this.Width = 340;
            this.Height = 80;

            // 圆角
            this.Load += (s, e) =>
            {
                var path = new GraphicsPath();
                int radius = 18;
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, this.Height - radius, radius, radius, 90, 90);
                path.CloseAllFigures();
                this.Region = new Region(path);
            };

            // 渐变背景
            this.Paint += (s, e) =>
            {
                using (var brush = new LinearGradientBrush(this.ClientRectangle, Color.FromArgb(255, 255, 255), Color.FromArgb(200, 220, 255), LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
                // 简单阴影
                using (var pen = new Pen(Color.LightGray, 2))
                {
                    e.Graphics.DrawPath(pen, GetRoundRectPath(this.ClientRectangle, 18));
                }
            };

            label = new Label
            {
                Text = message,
                ForeColor = Color.FromArgb(30, 30, 30),
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("微软雅黑", 10),
                Padding = new Padding(16, 8, 16, 8),
                AutoSize = false,
                MaximumSize = new Size(this.Width - 32, 0),
                AutoEllipsis = true,
                UseCompatibleTextRendering = true
            };
            this.Controls.Add(label);

            // 屏幕右下角，多个时自动上移
            var screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Right - this.Width - 20, screen.Bottom - this.Height - 20 - offset);

            // 定时关闭
            timer = new Timer();
            timer.Interval = durationMs;
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                this.Close();
            };
        }

      

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            timer.Start();
        }

        // 圆角辅助
        private GraphicsPath GetRoundRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return path;
        }
    }

    public static class ToastManager
    {
        private static readonly List<ToastForm> _toasts = new List<ToastForm>();
        private static readonly object _lock = new object();
        private const int ToastGap = 10; // 间距

        public static void ShowToast(string message, int durationMs = 2000)
        {
            if (Application.OpenForms.Count == 0)
                return; // 没有主窗体时不弹

            // 必须在UI线程创建和显示窗体
            Form mainForm = Application.OpenForms[0];
            if (mainForm.InvokeRequired)
            {
                mainForm.BeginInvoke(new Action(() => ShowToast(message, durationMs)));
                return;
            }

            ToastForm toast;
            lock (_lock)
            {
                int offset = 0;
                foreach (var t in _toasts)
                    offset += t.Height + ToastGap;

                toast = new ToastForm(message, offset, durationMs);
                _toasts.Add(toast);
            }

            toast.FormClosed += (s, e) =>
            {
                lock (_lock)
                {
                    _toasts.Remove(toast);
                    RearrangeToasts();
                }
            };
            toast.Show();
            ShowNoActivate(toast);
        }

        private static void RearrangeToasts()
        {
            int offset = 0;
            foreach (var toast in _toasts)
            {
                var screen = Screen.PrimaryScreen.WorkingArea;
                toast.Location = new System.Drawing.Point(
                    screen.Right - toast.Width - 20,
                    screen.Bottom - toast.Height - 20 - offset
                );
                offset += toast.Height + ToastGap;
            }
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static void ShowNoActivate(Form form)
        {
            const int SW_SHOWNOACTIVATE = 4;
            ShowWindow(form.Handle, SW_SHOWNOACTIVATE);
            form.Visible = true;
        }
    }
}