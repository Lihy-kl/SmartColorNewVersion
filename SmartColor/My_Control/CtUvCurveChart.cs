using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SmartColor.My_Control
{
    /// <summary>
    /// CtUvCurveChart 控件用于显示紫外吸光度曲线图。
    /// 支持添加多条曲线、自动着色、清除曲线、设置波长参数及记录模式。
    /// </summary>
    public partial class CtUvCurveChart : UserControl
    {
        /// <summary>
        /// Chart 控件实例，用于绘制曲线。
        /// </summary>
        private Chart chart;

        /// <summary>
        /// ToolTip 实例，用于显示鼠标悬停时的数据信息。
        /// </summary>
        private ToolTip toolTip;

     

        /// <summary>
        /// 是否为记录模式，影响曲线显示逻辑。
        /// </summary>
        private bool b_isRecord = true;

        /// <summary>
        /// 随机数生成器，用于自动生成曲线颜色。
        /// </summary>
        private static  readonly Random _rand = new Random();

        /// <summary>
        /// 构造函数，初始化控件及图表。
        /// </summary>
        public CtUvCurveChart()
        {
            chart = new Chart();
            toolTip = new ToolTip();
            chart.Dock = DockStyle.Fill;
            this.Controls.Add(chart);
            InitChart();
        }

    

        /// <summary>
        /// 初始化图表区域、图例及相关属性。
        /// </summary>
        public void InitChart()
        {
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Legends.Clear();

            ChartArea chartArea = new ChartArea();
            chartArea.AxisX.Interval = 40;
            chartArea.AxisX.Minimum = 300;
            chartArea.CursorX.IsUserEnabled = true;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            chartArea.CursorX.SelectionColor = Color.SkyBlue;
            chartArea.CursorY.IsUserEnabled = true;
            chartArea.CursorY.AutoScroll = true;
            chartArea.CursorY.IsUserSelectionEnabled = true;
            chartArea.CursorY.SelectionColor = Color.SkyBlue;
            chartArea.AxisX.ScaleView.Zoomable = true;
            chartArea.AxisY.ScaleView.Zoomable = true;
            chartArea.AxisY.IsStartedFromZero = true;
            chartArea.BackColor = Color.White;
            chartArea.BorderDashStyle = ChartDashStyle.NotSet;
            chartArea.BorderColor = Color.Black;
            chartArea.AxisY.Title = @"吸光度";
            chartArea.AxisY.LineWidth = 2;
            chartArea.AxisY.LineColor = Color.Black;
            chartArea.AxisY.Enabled = AxisEnabled.True;
            chartArea.AxisX.Title = @"波长";
            chartArea.AxisX.IsLabelAutoFit = true;
            chartArea.AxisX.LabelAutoFitMinFontSize = 5;
            chartArea.AxisX.LabelStyle.Angle = -15;
            chartArea.AxisX.LabelStyle.IsEndLabelVisible = true;
            chartArea.AxisX.LineWidth = 2;
            chartArea.AxisX.LineColor = Color.Black;
            chartArea.AxisX.Enabled = AxisEnabled.True;
            chartArea.Position.Height = 80;
            chartArea.Position.Width = 95;
            chartArea.Position.X = 0;
            chartArea.Position.Y = 20;
            chartArea.AxisX.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.Enabled = true;

            chart.ChartAreas.Add(chartArea);
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BorderlineWidth = 2;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

            Legend legend = new Legend("L1");
            legend.Docking = Docking.Top; // 停靠在顶部
            legend.IsDockedInsideChartArea = false; // 图例在图表区域外
            legend.Font = new Font("宋体", 14.25F);
            legend.BackColor = Color.Transparent; // 背景透明
            chart.Legends.Add(legend);

            chart.MouseMove += Chart_MouseMove;
        }

        /// <summary>
        /// 添加一条曲线到图表。
        /// </summary>
        /// <param name="name">曲线名称</param>
        /// <param name="color">曲线颜色</param>
        /// <param name="yValues">吸光度数据数组</param>
        /// <param name="start">起始波长</param>
        /// <param name="interval">间隔</param>
        public void AddCurve(string name, Color color, double[] yValues, int start, int interval)
        {
            Series series = new Series(name)
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.Double,
                MarkerStyle = MarkerStyle.Circle,
                MarkerColor = Color.Black,
                MarkerSize = 3,
                Color = color,
                BorderWidth = 2,
                CustomProperties = "PointWidth=2",
                IsValueShownAsLabel = false
            };
            for (int i = 0; i < yValues.Length; i++)
            {
                series.Points.AddXY(start + i * interval, yValues[i]);
            }
            chart.Series.Add(series);

            // 自动调整Y轴范围
            double minY = double.MaxValue, maxY = double.MinValue;
            foreach (var s in chart.Series)
            {
                foreach (var pt in s.Points)
                {
                    if (pt.YValues[0] < minY) minY = pt.YValues[0];
                    if (pt.YValues[0] > maxY) maxY = pt.YValues[0];
                }
            }
            if (minY == maxY)
            {
                minY -= 0.1;
                maxY += 0.1;
            }
            chart.ChartAreas[0].AxisY.Minimum = minY;
            chart.ChartAreas[0].AxisY.Maximum = maxY;
        }

        /// <summary>
        /// 自动生成颜色并添加曲线到图表。
        /// </summary>
        /// <param name="name">曲线名称</param>
        /// <param name="yValues">吸光度数据数组</param>
        public void AddCurveAutoColor(string name, double[] yValues, int start, int interval)
        {
            const int minColorDistance = 100; // 最小颜色距离，可根据实际调整
            Color color;
            int tryCount = 0;
            do
            {
                color = Color.FromArgb(255, _rand.Next(0, 256), _rand.Next(0, 256), _rand.Next(0, 256));
                bool isDistinct = true;
                foreach (var series in chart.Series)
                {
                    Color existing = series.Color;
                    int dist = ColorDistance(color, existing);
                    if (dist < minColorDistance)
                    {
                        isDistinct = false;
                        break;
                    }
                }
                if (isDistinct) break;
                tryCount++;
            } while (tryCount < 50); // 最多尝试50次，防止死循环

            AddCurve(name, color, yValues, start, interval);
        }

        // 计算颜色欧氏距离
        private int ColorDistance(Color c1, Color c2)
        {
            int dr = c1.R - c2.R;
            int dg = c1.G - c2.G;
            int db = c1.B - c2.B;
            return (int)Math.Sqrt(dr * dr + dg * dg + db * db);
        }

        /// <summary>
        /// 清除所有曲线。
        /// </summary>
        public void ClearCurves()
        {
            chart.Series.Clear();
        }

        /// <summary>
        /// 设置是否为记录模式。
        /// </summary>
        /// <param name="isRecord">记录模式标志</param>
        public void SetRecordMode(bool isRecord)
        {
            b_isRecord = isRecord;
        }

        // 删除以下成员变量
        // private int i_start;
        // private int i_int;

        // 删除 SetWaveParams 方法

        // 修改 Chart_MouseMove 方法
        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (chart.Series.Count == 0) return;
            chart.ChartAreas[0].CursorX.SetCursorPixelPosition(new PointF(e.X, e.Y), true);
            chart.ChartAreas[0].CursorY.SetCursorPixelPosition(new PointF(e.X, e.Y), true);
            double cursorXPosition = chart.ChartAreas[0].CursorX.Position;
            if (double.IsNaN(cursorXPosition))
            {
                return;
            }

            // 寻找距离 cursorXPosition 最近的点索引
            int nearestIndex = -1;
            double minDiff = double.MaxValue;
            var points = chart.Series[0].Points;
            for (int i = 0; i < points.Count; i++)
            {
                double diff = Math.Abs(points[i].XValue - cursorXPosition);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    nearestIndex = i;
                }
            }
            if (nearestIndex < 0) return;

            StringBuilder sb = new StringBuilder();
            double wave = chart.Series[0].Points[nearestIndex].XValue;
            sb.AppendFormat("波长:{0}\n", wave);
            for (int s = 0; s < chart.Series.Count; s++)
            {
                var series = chart.Series[s];
                if (nearestIndex < series.Points.Count)
                {
                    sb.AppendFormat("{0}:{1}\n", series.Name + "吸光度值", series.Points[nearestIndex].YValues[0]);
                }
            }
            toolTip.SetToolTip(chart, sb.ToString());
        }
    }
}