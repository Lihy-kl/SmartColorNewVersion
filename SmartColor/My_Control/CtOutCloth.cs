using SmartColor.My_File;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 出布控件
    /// </summary>
    public partial class CtOutCloth : UserControl
    {
        private Rectangle _workingRect;
        private decimal _weight = 0;
        private Color _borderColor = Color.Gray;
        private string _no = "1";
        private ToolTip _toolTip;

        public CtOutCloth()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            AutoScaleMode = AutoScaleMode.None;
            SizeChanged += CtOutCloth_SizeChanged;
            Size = new Size(40, 80);
            BackColor = Color.Transparent;

            _toolTip = new ToolTip();
            this.MouseEnter += CtOutCloth_MouseEnter;
            this.MouseLeave += CtOutCloth_MouseLeave;
        }

        [Description("文字"), Category("自定义")]
        public decimal Weight
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

        private void CtOutCloth_SizeChanged(object sender, EventArgs e)
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
                Logger.Error("CtOutCloth ResetWorkingRect异常", ex);
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

                using (var borderPen = new Pen(_borderColor, 1))
                {
                    // 画边框（矩形）
                    Point[] rectPoints = {
                new Point(_workingRect.Left, _workingRect.Top),
                new Point(_workingRect.Right - 1, _workingRect.Top),
                new Point(_workingRect.Right - 1, _workingRect.Bottom),
                new Point(_workingRect.Left, _workingRect.Bottom)
            };
                    using (var rectPath = new GraphicsPath())
                    {
                        rectPath.AddLines(rectPoints);
                        rectPath.CloseAllFigures();
                        g.DrawPolygon(borderPen, rectPoints);
                    }
                }

                // 编号严格居中在矩形内
                if (!string.IsNullOrEmpty(_no))
                {
                    var noSize = g.MeasureString(_no, Font);
                    var noPoint = new PointF(
                        _workingRect.Left + (_workingRect.Width - noSize.Width) / 2,
                        _workingRect.Top + (_workingRect.Height - noSize.Height) / 2
                    );
                    g.DrawString(_no, Font, Brushes.Black, noPoint);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CtOutCloth OnPaint异常", ex);
            }
        }
        private void CtOutCloth_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                _toolTip.SetToolTip(this, $"重量: {_weight:F3}");
            }
            catch (Exception ex)
            {
                Logger.Error($"CtOutCloth MouseEnter NO={_no}", ex);
            }
        }

        private void CtOutCloth_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                _toolTip.SetToolTip(this, "");
            }
            catch (Exception ex)
            {
                Logger.Error($"CtOutCloth MouseLeave NO={_no}", ex);
            }
        }
    }
}