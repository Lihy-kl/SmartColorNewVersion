using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SmartColor.My_File;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 夹子控件
    /// </summary>
    public partial class CtClip : UserControl
    {
        private Color _borderColor = Color.Gray;
        private string _desc = "说明";

        public CtClip()
        {
            InitializeComponent();
            this.Size = new Size(50, 50);
            this.MinimumSize = new Size(50, 50);
            this.MaximumSize = new Size(50, 50);
            this.BackColor = Color.Transparent;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.Resize += (s, e) => this.Size = new Size(50, 50);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            CenterInParent();

            if (this.Parent != null)
            {
                this.Parent.ClientSizeChanged -= Parent_ClientSizeChanged;
                this.Parent.ClientSizeChanged += Parent_ClientSizeChanged;
            }
        }

        private void Parent_ClientSizeChanged(object sender, EventArgs e)
        {
            CenterInParent();
        }

        public void CenterInParent()
        {
            if (this.Parent != null && this.Dock == DockStyle.None)
            {
                int x = (this.Parent.ClientSize.Width - this.Width) / 2;
                int y = (this.Parent.ClientSize.Height - this.Height) / 2;
                this.Location = new Point(Math.Max(0, x), Math.Max(0, y));
            }
        }

        [Description("边框颜色"), Category("自定义")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        [Description("说明"), Category("自定义")]
        public string Desc
        {
            get => _desc;
            set
            {
                _desc = value ?? "";
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // 正方形区域
                int side = Math.Min(this.Width, this.Height - 20) - 4;
                int squareX = (this.Width - side) / 2;
                int squareY = 2;
                Rectangle squareRect = new Rectangle(squareX, squareY, side, side);

                // 画正方形边框
                using (var pen = new Pen(_borderColor, 2))
                {
                    g.DrawRectangle(pen, squareRect);
                }

                // 圆形区域（居中于正方形）
                int circleDiameter = (int)(side * 0.6);
                int circleX = squareX + (side - circleDiameter) / 2;
                int circleY = squareY + (side - circleDiameter) / 2;
                Rectangle circleRect = new Rectangle(circleX, circleY, circleDiameter, circleDiameter);

                using (var pen = new Pen(Color.DarkGray, 2))
                {
                    g.DrawEllipse(pen, circleRect);
                }

                // 画奔驰标（三叉星）
                using (var pen = new Pen(Color.Green, 2))
                {
                    // 圆心
                    int cx = circleRect.Left + circleRect.Width / 2;
                    int cy = circleRect.Top + circleRect.Height / 2;
                    int r = circleRect.Width / 2 - 2;

                    // 三个方向，分别为 -90, 30, 150 度
                    for (int i = 0; i < 3; i++)
                    {
                        double angle = (-90 + i * 120) * Math.PI / 180.0;
                        int ex = cx + (int)(r * Math.Cos(angle));
                        int ey = cy + (int)(r * Math.Sin(angle));
                        g.DrawLine(pen, cx, cy, ex, ey);
                    }
                }

                // 说明文字（正方形下方居中）
                var font = new Font("宋体", 9F);
                var descText = _desc ?? "";
                var descSize = g.MeasureString(descText, font);
                float descX = (this.Width - descSize.Width) / 2;
                float descY = squareRect.Bottom + 4;
                g.DrawString(descText, font, new SolidBrush(Color.Black), descX, descY);
            }
            catch (Exception ex)
            {
                Logger.Error($"CtClip OnPaint Desc={_desc}", ex);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            CenterInParent();
        }
    }
}