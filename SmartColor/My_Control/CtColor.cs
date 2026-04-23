using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    public partial class CtColor : UserControl
    {
        private Label lblFormula;

        public CtColor()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            lblFormula = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("微软雅黑", 12F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = Color.Black,
                BackColor = Color.Transparent
            };
            this.Controls.Add(lblFormula);
        }

        /// <summary>
        /// 设置XYZ值并显示配方名，自动转换为RGB背景色
        /// </summary>
        /// <param name="x">X值</param>
        /// <param name="y">Y值</param>
        /// <param name="z">Z值</param>
        /// <param name="formulaName">配方名</param>
        public void SetXYZ(double x, double y, double z, string formulaName)
        {
            this.BackColor = XYZToColor(x, y, z);
            lblFormula.Text = formulaName;
            // 根据背景色自动调整字体颜色（黑/白）
            lblFormula.ForeColor = GetContrastColor(this.BackColor);
        }

        /// <summary>
        /// XYZ转Color（RGB）
        /// </summary>
        private Color XYZToColor(double x, double y, double z)
        {
            // 归一化到0~1
            x /= 100.0;
            y /= 100.0;
            z /= 100.0;

            // XYZ转线性RGB
            double r = x * 3.2406 + y * -1.5372 + z * -0.4986;
            double g = x * -0.9689 + y * 1.8758 + z * 0.0415;
            double b = x * 0.0557 + y * -0.2040 + z * 1.0570;

            // Gamma校正
            r = r > 0.0031308 ? 1.055 * Math.Pow(r, 1 / 2.4) - 0.055 : 12.92 * r;
            g = g > 0.0031308 ? 1.055 * Math.Pow(g, 1 / 2.4) - 0.055 : 12.92 * g;
            b = b > 0.0031308 ? 1.055 * Math.Pow(b, 1 / 2.4) - 0.055 : 12.92 * b;

            // 限制范围
            int R = Math.Min(Math.Max((int)Math.Round(r * 255), 0), 255);
            int G = Math.Min(Math.Max((int)Math.Round(g * 255), 0), 255);
            int B = Math.Min(Math.Max((int)Math.Round(b * 255), 0), 255);

            return Color.FromArgb(R, G, B);
        }

        /// <summary>
        /// 根据背景色自动选择黑色或白色字体
        /// </summary>
        private Color GetContrastColor(Color bg)
        {
            // YIQ公式
            int yiq = ((bg.R * 299) + (bg.G * 587) + (bg.B * 114)) / 1000;
            return yiq >= 128 ? Color.Black : Color.White;
        }

        /// <summary>
        /// 设置控件的宽度和高度
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void SetSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}