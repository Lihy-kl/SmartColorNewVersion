using SmartColor.My_File;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 公共针筒控件
    /// </summary>
    public partial class CtNeedles : UserControl
    {
        private Color _borderColor = Color.Gray;
        private string _defaultText = "公共针筒";

        public CtNeedles()
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
                // 取消之前的事件绑定，避免重复
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

                int diameter = 32;
                int totalContentHeight = diameter + textMargin + (int)textSize.Height;
                int startY = (this.Height - totalContentHeight) / 2;

                int x = (this.Width - diameter) / 2;
                int y = startY;

                Rectangle bigCircle = new Rectangle(x, y, diameter, diameter);

                using (var pen = new Pen(_borderColor, 2))
                {
                    g.DrawEllipse(pen, bigCircle);
                }

                int smallDiameter = (int)(diameter * 0.5);
                int smallX = x + (diameter - smallDiameter) / 2;
                int smallY = y + (diameter - smallDiameter) / 2;
                Rectangle smallCircle = new Rectangle(smallX, smallY, smallDiameter, smallDiameter);

                using (var pen = new Pen(_borderColor, 2))
                {
                    g.DrawEllipse(pen, smallCircle);
                }

                float textX = (this.Width - textSize.Width) / 2;
                float textY = bigCircle.Bottom + textMargin;
                if (textY + textSize.Height <= this.Height)
                {
                    g.DrawString(this.DefaultText, font, Brushes.Black, textX, textY);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CtNeedles OnPaint", ex);
            }
        }

       

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            CenterInParent();
        }
    }
}