using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SmartColor.My_File;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 洗针筒控件
    /// </summary>
    public partial class CtWash : UserControl
    {
        private Color _borderColor = Color.Gray;
        private string _defaultText = "洗针筒";

        public CtWash()
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

        [Description("显示的默认文字"), Category("自定义")]
        public string DefaultText
        {
            get => _defaultText;
            set
            {
                _defaultText = value;
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

                var font = new Font("宋体", 9F); ;
                var textSize = g.MeasureString(this.DefaultText, font);
                int textMargin = 4;

                // 先扣除底部文字高度和间距
                int availableHeight = this.Height - (int)textSize.Height - textMargin - 4;
                int side = Math.Min(this.Width - 4, availableHeight);

                // 居中正方形
                int squareX = (this.Width - side) / 2;
                int squareY = 2;
                Rectangle squareRect = new Rectangle(squareX, squareY, side, side);

                // 画正方形边框
                using (var pen = new Pen(_borderColor, 2))
                {
                    g.DrawRectangle(pen, squareRect);
                }

                // 圆形区域（居中于正方形，比例0.6）
                int circleDiameter = (int)(side * 0.6);
                int circleX = squareX + (side - circleDiameter) / 2;
                int circleY = squareY + (side - circleDiameter) / 2;
                Rectangle circleRect = new Rectangle(circleX, circleY, circleDiameter, circleDiameter);

                using (var pen = new Pen(Color.DarkGray, 2))
                {
                    g.DrawEllipse(pen, circleRect);
                }

                // 说明文字（正方形下方居中）
                float textX = (this.Width - textSize.Width) / 2;
                float textY = squareRect.Bottom + textMargin;
                g.DrawString(this.DefaultText, font, Brushes.Black, textX, textY);
            }
            catch (Exception ex)
            {
                Logger.Error("CtWash OnPaint", ex);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            CenterInParent();
        }
    }
}