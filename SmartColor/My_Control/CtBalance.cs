using SmartColor.My_File;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 天平控件
    /// </summary>
    public partial class CtBalance : UserControl
    {
        private Rectangle _workingRect;
        private string _title = "0.00";
        private readonly Color _bottleColor = Color.Black;
        private Color _liquidColor = Color.DeepSkyBlue;
        private decimal _maxValue = 2000;
        private string _no = "天平";

        // 固定设计尺寸
        private const int DesignWidth = 160;
        private const int DesignHeight = 160;
        private const int BarrelHeight = 80;
        private const int TrayHeight = 10;
        private const int DisplayHeight = 30;

        public CtBalance()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            AutoScaleMode = AutoScaleMode.None;
            SizeChanged += CtBalance_SizeChanged;
            Size = new Size(DesignWidth, DesignHeight);
            BackColor = Color.Transparent;
        }

        [Description("天平读数"), Category("自定义")]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                ResetWorkingRect();
                Refresh();
            }
        }

        [Description("液体颜色"), Category("自定义")]
        public Color LiquidColor
        {
            get => _liquidColor;
            set
            {
                _liquidColor = value;
                Refresh();
            }
        }

        [Description("文字字体"), Category("自定义")]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                ResetWorkingRect();
                Refresh();
            }
        }

        [Description("文字颜色"), Category("自定义")]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                base.ForeColor = value;
                Refresh();
            }
        }

        [Description("最大值"), Category("自定义")]
        public decimal MaxValue
        {
            get => _maxValue;
            set
            {
                if (value <= 0) return;
                _maxValue = value;
                Refresh();
            }
        }

        [Description("天平编号"), Category("自定义")]
        public string NO
        {
            get => _no;
            set
            {
                _no = value;
                Refresh();
            }
        }

        private void CtBalance_SizeChanged(object sender, EventArgs e)
        {
            ResetWorkingRect();
        }

        /// <summary>
        /// 重新计算电子秤区域
        /// </summary>
        private void ResetWorkingRect()
        {
            try
            {
                using (var g = CreateGraphics())
                {
                    g.MeasureString(_title, Font);
                    _workingRect = new Rectangle(0, 10, DesignWidth, DesignHeight - 15);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CtBalance ResetWorkingRect", ex);
            }
        }

        /// <summary>
        /// 重绘控件
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
                var g = e.Graphics;

                // 计算缩放比例，保证不变形，居中显示
                float scale = Math.Min((float)this.Width / DesignWidth, (float)this.Height / DesignHeight);
                int drawWidth = (int)(DesignWidth * scale);
                int drawHeight = (int)(DesignHeight * scale);
                int offsetX = (this.Width - drawWidth) / 2;
                int offsetY = (this.Height - drawHeight) / 2;

                // 设置缩放和居中
                g.TranslateTransform(offsetX, offsetY);
                g.ScaleTransform(scale, scale);

                // 使用设计尺寸绘制
                Rectangle workingRect = new Rectangle(0, 10, DesignWidth, DesignHeight - 15);

                using (var myPen = new Pen(_bottleColor, 1))
                {
                    // 画废液桶口
                    DrawPolygon(g, myPen, new[]
                    {
                        new Point(workingRect.Left + workingRect.Width / 4 + 9, workingRect.Top - 9),
                        new Point(workingRect.Right - workingRect.Width / 4 - 10, workingRect.Top - 9),
                        new Point(workingRect.Right - workingRect.Width / 4 - 10, workingRect.Top),
                        new Point(workingRect.Left + workingRect.Width / 4 + 9, workingRect.Top)
                    });

                    // 画废液桶
                    DrawPolygon(g, myPen, new[]
                    {
                        new Point(workingRect.Left + workingRect.Width / 4 + 10, workingRect.Top),
                        new Point(workingRect.Right - 1 - workingRect.Width / 4 - 10, workingRect.Top),
                        new Point(workingRect.Right - 1 - workingRect.Width / 4 - 10, workingRect.Top + BarrelHeight),
                        new Point(workingRect.Left + workingRect.Width / 4 + 10, workingRect.Top + BarrelHeight)
                    });

                    // 画液体
                    if (!decimal.TryParse(_title, out decimal value)) value = 0;
                    decimal liquidHeight = (_maxValue > 0) ? (value / _maxValue) * BarrelHeight : 0;
                    DrawFilledPolygon(g, new SolidBrush(_liquidColor), new[]
                    {
                        new PointF(workingRect.Left + workingRect.Width / 4 + 11, (float)(workingRect.Top + BarrelHeight - liquidHeight)),
                        new PointF(workingRect.Right - 1 - workingRect.Width / 4 - 10, (float)(workingRect.Top + BarrelHeight - liquidHeight)),
                        new PointF(workingRect.Right - 1 - workingRect.Width / 4 - 10, workingRect.Top + BarrelHeight),
                        new PointF(workingRect.Left + workingRect.Width / 4 + 11, workingRect.Top + BarrelHeight)
                    });
                    DrawFilledPolygon(g, new SolidBrush(Color.FromArgb(50, _liquidColor)), new[]
                    {
                        new PointF(workingRect.Left + workingRect.Width / 4 + 11, (float)(workingRect.Top + BarrelHeight - liquidHeight)),
                        new PointF(workingRect.Right - 1 - workingRect.Width / 4 - 10, (float)(workingRect.Top + BarrelHeight - liquidHeight)),
                        new PointF(workingRect.Right - 1 - workingRect.Width / 4 - 10, workingRect.Top + BarrelHeight),
                        new PointF(workingRect.Left + workingRect.Width / 4 + 11, workingRect.Top + BarrelHeight)
                    });

                    // 画托盘
                    DrawFilledPolygon(g, new SolidBrush(Color.SlateBlue), new[]
                    {
                        new Point(workingRect.Left, workingRect.Top + BarrelHeight + 1),
                        new Point(workingRect.Right - 1, workingRect.Top + BarrelHeight + 1),
                        new Point(workingRect.Right - 1, workingRect.Top + BarrelHeight + TrayHeight),
                        new Point(workingRect.Left, workingRect.Top + BarrelHeight + TrayHeight)
                    });

                    // 画支柱1
                    DrawFilledPolygon(g, new SolidBrush(Color.Black), new[]
                    {
                        new Point(workingRect.Left + workingRect.Width / 3, workingRect.Top + BarrelHeight + TrayHeight),
                        new Point(workingRect.Left + workingRect.Width / 3 + 5, workingRect.Top + BarrelHeight + TrayHeight),
                        new Point(workingRect.Left + workingRect.Width / 3 + 5, workingRect.Top + BarrelHeight + TrayHeight + 10),
                        new Point(workingRect.Left + workingRect.Width / 3, workingRect.Top + BarrelHeight + TrayHeight + 10)
                    });

                    // 画支柱2
                    DrawFilledPolygon(g, new SolidBrush(Color.Black), new[]
                    {
                        new Point(workingRect.Right - 1 - workingRect.Width / 3, workingRect.Top + BarrelHeight + TrayHeight),
                        new Point(workingRect.Right - 1 - workingRect.Width / 3 - 5, workingRect.Top + BarrelHeight + TrayHeight),
                        new Point(workingRect.Right - 1 - workingRect.Width / 3 - 5, workingRect.Top + BarrelHeight + TrayHeight + 10),
                        new Point(workingRect.Right - 1 - workingRect.Width / 3, workingRect.Top + BarrelHeight + TrayHeight + 10)
                    });

                    // 画秤
                    DrawPolygon(g, myPen, new[]
                    {
                        new Point(workingRect.Left + workingRect.Width / 4, workingRect.Top + BarrelHeight + TrayHeight + 10),
                        new Point(workingRect.Right - 1 - workingRect.Width / 4, workingRect.Top + BarrelHeight + TrayHeight + 10),
                        new Point(workingRect.Right - 1 - workingRect.Width / 20, workingRect.Bottom - 1),
                        new Point(workingRect.Left + workingRect.Width / 20, workingRect.Bottom - 1)
                    });

                    // 画显示屏
                    DrawPolygon(g, myPen, new[]
                    {
                        new Point(workingRect.Left + workingRect.Width / 4, workingRect.Bottom - 1 - DisplayHeight),
                        new Point(workingRect.Right - workingRect.Width / 4, workingRect.Bottom - 1 - DisplayHeight),
                        new Point(workingRect.Right - workingRect.Width / 4, workingRect.Bottom - 1 - 5),
                        new Point(workingRect.Left + workingRect.Width / 4, workingRect.Bottom - 1 - 5)
                    });
                }

                // 写编号（居中）
                if (!string.IsNullOrEmpty(_no))
                {
                    var noRect = new RectangleF(
                        0,
                        workingRect.Bottom - 30 - Font.Height,
                        DesignWidth,
                        Font.Height + 5
                    );
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(_no, Font, new SolidBrush(ForeColor), noRect, sf);
                    }
                }

                // 写读数（居中）
                var titleRect = new RectangleF(
                    0,
                    workingRect.Bottom - 1 - 8 - Font.Height,
                    DesignWidth,
                    Font.Height + 5
                );
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    g.DrawString(_title, new Font("宋体", 10.5f, FontStyle.Bold), new SolidBrush(ForeColor), titleRect, sf);
                }

                // 恢复变换
                g.ResetTransform();
            }
            catch (Exception ex)
            {
                Logger.Error("CtBalance OnPaint", ex);
            }
        }

        private void DrawPolygon(Graphics g, Pen pen, Point[] points)
        {
            using (var path = new GraphicsPath())
            {
                path.AddLines(points);
                path.CloseAllFigures();
                g.DrawPolygon(pen, points);
            }
        }

        private void DrawFilledPolygon(Graphics g, Brush brush, PointF[] points)
        {
            using (var path = new GraphicsPath())
            {
                path.AddLines(points);
                path.CloseAllFigures();
                g.FillPath(brush, path);
            }
        }

        private void DrawFilledPolygon(Graphics g, Brush brush, Point[] points)
        {
            using (var path = new GraphicsPath())
            {
                path.AddLines(points);
                path.CloseAllFigures();
                g.FillPath(brush, path);
            }
        }
    }
}