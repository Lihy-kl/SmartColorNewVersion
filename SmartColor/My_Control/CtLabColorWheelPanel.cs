using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// L*a*b* 色轮及点显示控件
    /// </summary>
    public partial class CtLabColorWheelPanel : Panel
    {
        private List<LabPoint> _points = new List<LabPoint>();
        private int _nextPointIndex = 1;

        public CtLabColorWheelPanel()
        {
            this.DoubleBuffered = true;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Size = new Size(450, 550);
            this.Name = "LabColorWheelPanel";
            this.Paint += LabColorWheelPanel_Paint;
        }

        /// <summary>
        /// 添加一个点
        /// </summary>
        public void AddPoint(double a, double b)
        {
            a = Clamp(a, -128, 127);
            b = Clamp(b, -128, 127);
            _points.Add(new LabPoint(a, b, _nextPointIndex++));
            this.Invalidate();
        }

        /// <summary>
        /// 清除所有点
        /// </summary>
        public void ClearPoints()
        {
            _points.Clear();
            _nextPointIndex = 1;
            this.Invalidate();
        }

        /// <summary>
        /// 获取所有点
        /// </summary>
        public IReadOnlyList<LabPoint> Points => _points.AsReadOnly();

        private void LabColorWheelPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int centerX = this.Width / 2;
            int centerY = this.Height / 2;
            int radius = Math.Min(centerX, centerY) - 20;

            DrawColorWheel(g, centerX, centerY, radius);
            DrawCoordinates(g, centerX, centerY, radius);
            DrawAllPoints(g, centerX, centerY, radius);
            DrawLabels(g, centerX, centerY, radius);
        }

        private void DrawColorWheel(Graphics g, int centerX, int centerY, int radius)
        {
            for (int angle = 0; angle < 360; angle++)
            {
                if (radius <= 0) return;

                using (GraphicsPath path = new GraphicsPath())
                {
                    float x1 = centerX + (float)(radius * Math.Cos(angle * Math.PI / 180));
                    float y1 = centerY + (float)(radius * Math.Sin(angle * Math.PI / 180));
                    path.AddLine(centerX, centerY, x1, y1);
                    path.AddArc(centerX - radius, centerY - radius, radius * 2, radius * 2, angle, 1);
                    float x2 = centerX + (float)(radius * Math.Cos((angle + 1) * Math.PI / 180));
                    float y2 = centerY + (float)(radius * Math.Sin((angle + 1) * Math.PI / 180));
                    path.AddLine(x2, y2, centerX, centerY);

                    double a = 100 * Math.Cos(angle * Math.PI / 180);
                    double b = 100 * Math.Sin(angle * Math.PI / 180);
                    Color color = LabToRgb(70, a, b);

                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
            using (SolidBrush brush = new SolidBrush(LabToRgb(70, 0, 0)))
            {
                g.FillEllipse(brush, centerX - 30, centerY - 30, 60, 60);
            }
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);
            }
        }

        private void DrawCoordinates(Graphics g, int centerX, int centerY, int radius)
        {
            using (Pen pen = new Pen(Color.Black, 1))
            {
                g.DrawLine(pen, centerX - radius, centerY, centerX + radius, centerY);
                g.DrawLine(pen, centerX, centerY - radius, centerX, centerY + radius);
            }
            int tickCount = 8;
            for (int i = 1; i <= tickCount; i++)
            {
                int x = (int)(centerX - radius + (i * 2 * radius / (tickCount + 1)));
                g.DrawLine(Pens.Black, x, centerY - 5, x, centerY + 5);
                int y = (int)(centerY - radius + (i * 2 * radius / (tickCount + 1)));
                g.DrawLine(Pens.Black, centerX - 5, y, centerX + 5, y);
            }
            using (Font font = new Font("Arial", 10, FontStyle.Bold))
            {
                g.DrawString("+a", font, Brushes.Black, centerX + radius + 5, centerY - 10);
                g.DrawString("-a", font, Brushes.Black, centerX - radius - 25, centerY - 10);
                g.DrawString("+b", font, Brushes.Black, centerX - 10, centerY - radius - 15);
                g.DrawString("-b", font, Brushes.Black, centerX - 10, centerY + radius + 5);
            }
            using (Font font = new Font("Arial", 8))
            {
                g.DrawString("127", font, Brushes.Black, centerX + radius - 15, centerY + 10);
                g.DrawString("-128", font, Brushes.Black, centerX - radius + 5, centerY + 10);
                g.DrawString("127", font, Brushes.Black, centerX + 10, centerY - radius + 5);
                g.DrawString("-128", font, Brushes.Black, centerX + 10, centerY + radius - 10);
            }
        }

        private void DrawAllPoints(Graphics g, int centerX, int centerY, int radius)
        {
            foreach (LabPoint point in _points)
            {
                double normalizedA = point.A / 128.0;
                double normalizedB = point.B / 128.0;
                double r = Math.Sqrt(normalizedA * normalizedA + normalizedB * normalizedB);
                r = Math.Min(r, 1.0);
                double angle = Math.Atan2(normalizedB, normalizedA) * 180 / Math.PI;
                if (angle < 0) angle += 360;
                int pointX = (int)(centerX + r * radius * Math.Cos(angle * Math.PI / 180));
                int pointY = (int)(centerY + r * radius * Math.Sin(angle * Math.PI / 180));
                using (Pen pen = new Pen(Color.Black, 2))
                {
                    g.DrawEllipse(pen, pointX - 6, pointY - 6, 12, 12);
                }
                Color pointColor = LabToRgb(70, point.A, point.B);
                using (SolidBrush brush = new SolidBrush(pointColor))
                {
                    g.FillEllipse(brush, pointX - 4, pointY - 4, 8, 8);
                }
                using (Font font = new Font("Arial", 8, FontStyle.Bold))
                {
                    string label = point.Index.ToString();
                    SizeF textSize = g.MeasureString(label, font);
                    g.DrawString(label, font, Brushes.White, pointX - textSize.Width / 2, pointY - textSize.Height / 2);
                }
            }
        }

        private void DrawLabels(Graphics g, int centerX, int centerY, int radius)
        {
            using (Font font = new Font("Arial", 10))
            {
                g.DrawString("L*a*b* 色轮", font, Brushes.Black, centerX - 50, centerY + radius + 20);
                g.DrawString("红", font, Brushes.Black, centerX + radius + 15, centerY - 20);
                g.DrawString("绿", font, Brushes.Black, centerX - radius - 20, centerY - 20);
                g.DrawString("黄", font, Brushes.Black, centerX + 20, centerY - radius - 20);
                g.DrawString("蓝", font, Brushes.Black, centerX + 20, centerY + radius + 5);
            }
        }

        private Color LabToRgb(double l, double a, double b)
        {
            double y = (l + 16) / 116.0;
            double x = a / 500.0 + y;
            double z = y + b / 200.0; // 修正此处符号，原为 y - b / 200.0

            x = x > 0.206893 ? Math.Pow(x, 3) : (x - 16.0 / 116.0) / 7.787;
            y = l > 8 ? Math.Pow(y, 3) : l / 903.3;
            z = z > 0.206893 ? Math.Pow(z, 3) : (z - 16.0 / 116.0) / 7.787;
            x *= 0.95047;
            z *= 1.08883;
            double r = x * 3.2406 + y * -1.5372 + z * -0.4986;
            double g = x * -0.9689 + y * 1.8758 + z * 0.0415;
            double bColor = x * 0.0557 + y * -0.2040 + z * 1.0570;
            r = r > 0.0031308 ? 1.055 * Math.Pow(r, 1.0 / 2.4) - 0.055 : 12.92 * r;
            g = g > 0.0031308 ? 1.055 * Math.Pow(g, 1.0 / 2.4) - 0.055 : 12.92 * g;
            bColor = bColor > 0.0031308 ? 1.055 * Math.Pow(bColor, 1.0 / 2.4) - 0.055 : 12.92 * bColor;
            int rByte = Clamp((int)(r * 255), 0, 255);
            int gByte = Clamp((int)(g * 255), 0, 255);
            int bByte = Clamp((int)(bColor * 255), 0, 255);
            return Color.FromArgb(rByte, gByte, bByte);
        }

        private int Clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }
        private double Clamp(double value, double min, double max)
        {
            return Math.Min(Math.Max(value, min), max);
        }
    }

    /// <summary>
    /// 点数据结构
    /// </summary>
    public class LabPoint
    {
        public double A { get; set; }
        public double B { get; set; }
        public int Index { get; set; }
        public LabPoint(double a, double b, int index)
        {
            A = a;
            B = b;
            Index = index;
        }
    }
}



