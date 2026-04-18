using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SmartColor.My_File;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 备布控件
    /// </summary>
    public partial class CtPrepareCloth : UserControl
    {
        private Rectangle _workingRect;
        private decimal _weight = 0;
        private Color _borderColor = Color.Gray;
        private string _no = "1";
        private ToolTip _toolTip;

        public CtPrepareCloth()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            AutoScaleMode = AutoScaleMode.None;
            SizeChanged += CtPrepareCloth_SizeChanged;
            Size = new Size(40, 40);
            BackColor = Color.Transparent;

            _toolTip = new ToolTip();
            this.MouseEnter += CtPrepareCloth_MouseEnter;
            this.MouseLeave += CtPrepareCloth_MouseLeave;
        }

        [Description("重量"), Category("自定义")]
        public decimal Title
        {
            get => _weight;
            set
            {
                _weight = value;
                ResetWorkingRect();
                Refresh();
            }
        }

        [Description("边框颜色"), Category("自定义")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Refresh();
            }
        }

        [Description("编号"), Category("自定义")]
        public string NO
        {
            get => _no;
            set
            {
                _no = value;
                Refresh();
            }
        }

        private void CtPrepareCloth_SizeChanged(object sender, EventArgs e)
        {
            ResetWorkingRect();
        }

        private void ResetWorkingRect()
        {
            try
            {
                _workingRect = new Rectangle(0, 10, Width, Height - 15);
            }
            catch (Exception ex)
            {
                Logger.Error("CtPrepareCloth ResetWorkingRect异常", ex);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
                var g = e.Graphics;

                if (_workingRect.Width <= 0 || _workingRect.Height <= 0)
                    return;

                // 画圆形边框
                int diameter = Math.Min(_workingRect.Width, _workingRect.Height);
                Rectangle circleRect = new Rectangle(
                    _workingRect.Left + (_workingRect.Width - diameter) / 2,
                    _workingRect.Top + (_workingRect.Height - diameter) / 2,
                    diameter, diameter);

                using (var borderPen = new Pen(_borderColor, 2))
                {
                    g.DrawEllipse(borderPen, circleRect);
                }

                // 编号居中在圆形内
                if (!string.IsNullOrEmpty(_no))
                {
                    var noSize = g.MeasureString(_no, Font);
                    var noPoint = new PointF(
                        circleRect.Left + (circleRect.Width - noSize.Width) / 2,
                        circleRect.Top + (circleRect.Height - noSize.Height) / 2
                    );
                    g.DrawString(_no, Font, Brushes.Black, noPoint);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CtPrepareCloth OnPaint异常", ex);
            }
        }

        private void CtPrepareCloth_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                _toolTip.SetToolTip(this, $"重量: {_weight:F3}");
            }
            catch (Exception ex)
            {
                Logger.Error($"CtPrepareCloth MouseEnter NO={_no}", ex);
            }
        }

        private void CtPrepareCloth_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                _toolTip.SetToolTip(this, "");
            }
            catch (Exception ex)
            {
                Logger.Error($"CtPrepareCloth MouseLeave NO={_no}", ex);
            }
        }
    }
}