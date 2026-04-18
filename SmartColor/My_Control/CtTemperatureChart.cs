using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 图表数据结构，包含温度和工艺描述字符串
    /// </summary>
    public struct ChartData
    {
        public string temperature; // 温度数据字符串
        public string craft;       // 工艺描述字符串
    }

    


    /// <summary>
    /// 工艺步骤结构体，包含步骤名称、目标温度、升温速率和持续时间
    /// </summary>
    public struct ProcessStep
    {
        public string StepName;           // 步骤名称
        public double? TargetTemperature; // 目标温度
        public double? HeatingRate;       // 升温速率
        public double? Duration;          // 持续时间（分钟）
    }

    /// <summary>
    /// 温度曲线控件，支持温度曲线绘制、工艺标记、动态刻度、缩放、Tooltip等功能
    /// </summary>
    public partial class CtTemperatureChart : UserControl
    {
        /// <summary>
        /// 显示模式改变事件
        /// 0：默认模式，只显示实际曲线和工艺标记
        /// 1：单独显示逻辑曲线
        /// 2：两个都显示，但是为了避免混乱，不显示标记
        /// </summary>
        public event EventHandler<int> ShowModeChanged; // 显示模式改变事件
        public Chart Chart { get; private set; } // 图表控件
        private readonly ToolTip ToolTip1 = new ToolTip(); // 鼠标悬停提示
        private DateTime[] times; // 每个温度点对应的时间
        private const int DefaultIntervalMinutes = 30; // X轴默认每30分钟一个刻度
        private string[] craftsData; // 工艺标记数据
                                     // 在类中添加温度下限常量
        private const double MinTemperature = 15.0;


        /// <summary>
        /// 构造函数，初始化控件和图表
        /// </summary>
        public CtTemperatureChart()
        {
            InitializeComponent();
            InitChart();
        }

        /// <summary>
        /// 初始化图表控件及其样式、事件绑定
        /// </summary>
        private void InitChart()
        {
            Chart = new Chart { Dock = DockStyle.Fill, Visible = true };
            this.Controls.Add(Chart);

            // 配置图表区域
            var chartArea = new ChartArea
            {
                AxisX = {
                    IntervalType = DateTimeIntervalType.Minutes,
                    Interval = DefaultIntervalMinutes,
                    Title = @"时间",
                    IsLabelAutoFit = true,
                    LabelAutoFitMinFontSize = 8,
                    LabelStyle = { Format = "HH:mm", Angle = -15, IsEndLabelVisible = true },
                    LineWidth = 2,
                    LineColor = Color.Black,
                    Enabled = AxisEnabled.True,
                    MajorGrid = { Enabled = true }
                },
                AxisY = {
                    ScaleView = { Zoomable = true },
                    Title = @"温度(℃)",
                    LineWidth = 2,
                    LineColor = Color.Black,
                    Enabled = AxisEnabled.True,
                    MajorGrid = { Enabled = true },
                    LabelStyle = { Format = "0" }, // 只显示整数
                },
                CursorX = {
                    IsUserEnabled = true,
                    IsUserSelectionEnabled = true,
                    SelectionColor = Color.SkyBlue
                },
                CursorY = {
                    IsUserEnabled = true,
                    AutoScroll = true,
                    IsUserSelectionEnabled = true,
                    SelectionColor = Color.SkyBlue
                },
                BackColor = Color.White,
                BorderDashStyle = ChartDashStyle.NotSet,
                BorderColor = Color.Black,
                Position = { Height = 85, Width = 95, X = 0, Y = 13 }
            };


            Chart.ChartAreas.Add(chartArea);
            Chart.BackGradientStyle = GradientStyle.TopBottom;
            Chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            Chart.BorderlineDashStyle = ChartDashStyle.Solid;
            Chart.BorderlineWidth = 2;
            Chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

            // 配置图例
            var legend = new Legend
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Far,
                LegendStyle = LegendStyle.Row,
                BorderColor = Color.Black,
                BorderWidth = 1,
                BorderDashStyle = ChartDashStyle.Solid
            };
            Chart.Legends.Add(legend);

            // 绑定事件
            Chart.MouseMove += Chart_MouseMove;
            Chart.MouseWheel += Chart_MouseWheel;
            Chart.PostPaint += Chart_PostPaint;
            Chart.AxisViewChanged += AxisX_ViewChanged;
            Chart.Legends[0].Enabled = false; // 默认隐藏图例
        }

        /// <summary>
        /// 清空图表数据和工艺标记
        /// </summary>
        public void ClearChart()
        {
            Chart.Series.Clear();
            craftsData = null;
            times = null;
            if (Chart.ChartAreas.Count > 0)
            {
                var area = Chart.ChartAreas[0];
                area.AxisX.StripLines.Clear();
                area.AxisX.CustomLabels.Clear();
            }
            Chart.Invalidate();
        }

        /// <summary>
        /// 添加数据序列到图表
        /// </summary>
        /// <param name="seriesName">序列名称</param>
        /// <param name="seriesColor">序列颜色</param>
        /// <param name="chartType">序列类型（默认折线）</param>
        /// <param name="xValueType">X轴值类型（默认时间）</param>
        public void AddSeries(string seriesName, Color seriesColor, SeriesChartType chartType = SeriesChartType.Line, ChartValueType xValueType = ChartValueType.DateTime)
        {
            var series = new Series(seriesName)
            {
                ChartType = chartType,
                XValueType = xValueType,
                MarkerStyle = MarkerStyle.Circle,
                MarkerColor = Color.FromArgb(60, seriesColor),
                MarkerBorderColor = Color.FromArgb(60, seriesColor),
                MarkerBorderWidth = chartType == SeriesChartType.Point ? 2 : 1,
                MarkerSize = chartType == SeriesChartType.Point ? 10 : 3,
                Color = seriesColor,
                BorderWidth = 2,
                CustomProperties = "PointWidth=2",
                IsValueShownAsLabel = false,
                Legend = Chart.Legends[0].Name
            };
            Chart.Series.Add(series);
        }

        /// <summary>
        /// 添加一条温度曲线（不清空原有曲线）
        /// </summary>
        /// <param name="seriesName">曲线名称</param>
        /// <param name="data">温度点数组</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="intervalSeconds">间隔秒数</param>
        /// <param name="seriesColor">曲线颜色</param>
        /// <summary>
        /// 添加一条温度曲线（不清空原有曲线）
        /// </summary>
        public void AddTemperatureSeries(string seriesName, string[] data, DateTime startTime, int intervalSeconds, Color seriesColor)
        {
            // 如果已存在同名曲线，先移除
            if (Chart.Series.IndexOf(seriesName) >= 0)
                Chart.Series.Remove(Chart.Series[seriesName]);

            var series = new Series(seriesName)
            {
                ChartType = SeriesChartType.Line,
                XValueType = ChartValueType.DateTime,
                Color = seriesColor,
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.None,
                IsValueShownAsLabel = false
            };
            for (int i = 0; i < data.Length; i++)
            {
                if (double.TryParse(data[i], out double value))
                    series.Points.AddXY(startTime.AddSeconds(i * intervalSeconds), value);
            }
            Chart.Series.Add(series);
        }

        /// <summary>
        /// 设置主曲线和工艺标记（X轴为时间，支持动态刻度、错位显示、标记线、Tooltip）
        /// </summary>
        /// <param name="data">温度点数组</param>
        /// <param name="crafts">工艺点描述数组</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="intervalSeconds">每个点的间隔秒数</param>
        public void SetDataWithCraft(string[] data, string[] crafts, DateTime startTime, int intervalSeconds = 30)
        {
            int len = data.Length;
            times = new DateTime[len];
            // 合并同一温度点的标签
            var craftDict = new Dictionary<int, List<string>>();
            for (int i = 0; i < crafts.Length; i++)
            {
                var craft = crafts[i];
                var arr = craft.Split(',');
                if (arr.Length == 2 && int.TryParse(arr[1], out int idx) && idx > 0 && idx <= len)
                {
                    int pointIdx = idx - 1;
                    if (!craftDict.ContainsKey(pointIdx))
                        craftDict[pointIdx] = new List<string>();
                    craftDict[pointIdx].Add(arr[0]);
                }
            }

            craftsData = crafts;
            for (int i = 0; i < len; i++)
                times[i] = startTime.AddSeconds(i * intervalSeconds);

            Chart.Series.Clear();
            var area = Chart.ChartAreas[0];
            area.AxisX.StripLines.Clear();
            area.AxisX.CustomLabels.Clear();

            // 添加温度主曲线
            AddSeries("温度", Color.Red, SeriesChartType.Line, ChartValueType.DateTime);
            var mainSeries = Chart.Series["温度"];
            for (int i = 0; i < len; i++)
            {
                if (double.TryParse(data[i], out double value))
                    mainSeries.Points.AddXY(times[i], value);
            }

            // 添加工艺标记点（合并标签）
            AddSeries("工艺标记", Color.Blue, SeriesChartType.Point, ChartValueType.DateTime);
            var markSeries = Chart.Series["工艺标记"];
            foreach (var kv in craftDict)
            {
                int pointIdx = kv.Key;
                double y = mainSeries.Points[pointIdx].YValues[0];
                var dp = markSeries.Points.AddXY(times[pointIdx], y);
                markSeries.Points[markSeries.Points.Count - 1].ToolTip = string.Join("/", kv.Value); // 合并标签
            }

            // 设置X轴标签格式和刻度
            area.AxisX.LabelStyle.Format = "HH:mm";
            area.AxisX.IntervalType = DateTimeIntervalType.Minutes;
            area.AxisX.Interval = DefaultIntervalMinutes;
            area.AxisX.Title = "时间";

            // ----------- 智能动态边距调整 -----------
            // 1. 统计上下标签数量
            int topCount = 0, bottomCount = 0;
            for (int i = 0; i < crafts.Length; i++)
            {
                if (i % 2 == 0) topCount++;
                else bottomCount++;
            }

            // 2. 测量标签高度（与Chart_PostPaint一致）
            float labelFontSize = 9f;
            float labelHeight = 18f;
            using (var bmp = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(bmp))
            using (var font = new Font("微软雅黑", labelFontSize, FontStyle.Bold))
            {
                labelHeight = g.MeasureString("测试", font).Height;
            }

            // 3. 计算边距（每行标签高度*行数+额外间隔）
            float marginPerRow = labelHeight + 6f;
            float topMargin = topCount * marginPerRow;
            float bottomMargin = bottomCount * marginPerRow;

            // 4. 设置Y轴范围
            if (mainSeries.Points.Count > 0)
            {
                double min = mainSeries.Points[0].YValues[0];
                double max = min;
                foreach (var pt in mainSeries.Points)
                {
                    if (pt.YValues[0] < min) min = pt.YValues[0];
                    if (pt.YValues[0] > max) max = pt.YValues[0];
                }
                float chartHeightPx = Chart.Height;
                double yRange = max - min;
                if (chartHeightPx > 0 && yRange > 0)
                {
                    double yPerPx = yRange / chartHeightPx;
                    double topMarginY = topMargin * yPerPx;
                    double bottomMarginY = bottomMargin * yPerPx;
                    // 最小边距限制
                    if (topMarginY < 5) topMarginY = 5;
                    if (bottomMarginY < 5) bottomMarginY = 5;
                    area.AxisY.Minimum = min - bottomMarginY;
                    area.AxisY.Maximum = max + topMarginY;
                }
                else
                {
                    // 兜底方案
                    double margin = (max - min) * 0.3;
                    if (margin < 5) margin = 5;
                    area.AxisY.Minimum = min - margin;
                    area.AxisY.Maximum = max + margin;
                }
            }
            // 设置Y轴只显示整数
            area.AxisY.LabelStyle.Format = "0";
           
        }

        /// <summary>
        /// 鼠标移动事件，显示温度点的时间和温度Tooltip
        /// </summary>
        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (Chart.Series.Count == 0 || times == null || Chart.Series.IndexOf("温度") < 0) return;
            var area = Chart.ChartAreas[0];
            try
            {
                area.CursorX.SetCursorPixelPosition(new PointF(e.X, e.Y), true);
                area.CursorY.SetCursorPixelPosition(new PointF(e.X, e.Y), true);
            }
            catch
            {
                // 游标设置失败时直接返回，避免异常
                return;
            }

            var mainSeries = Chart.Series.IndexOf("温度") >= 0 ? Chart.Series["温度"] : null;
            if (mainSeries == null || mainSeries.Points.Count == 0) return;

            HitTestResult result = Chart.HitTest(e.X, e.Y);
            if (result.PointIndex >= 0 && result.Series == mainSeries)
            {
                int idx = result.PointIndex;
                ToolTip1.SetToolTip(Chart, $"时间:{times[idx]:HH:mm:ss}, 温度:{mainSeries.Points[idx].YValues[0]:F2}");
            }
        }

        /// <summary>
        /// 鼠标滚轮事件，支持图表缩放和重置
        /// </summary>
        private void Chart_MouseWheel(object sender, MouseEventArgs e)
        {
            var xAxis = Chart.ChartAreas[0].AxisX;
            var yAxis = Chart.ChartAreas[0].AxisY;
            if (e.Delta < 0)
            {
                // 向下滚动，重置缩放
                xAxis.ScaleView.ZoomReset();
                yAxis.ScaleView.ZoomReset();
            }
            else if (e.Delta > 0)
            {
                // 向上滚动，缩放到鼠标位置
                var xMin = xAxis.ScaleView.ViewMinimum;
                var xMax = xAxis.ScaleView.ViewMaximum;
                var yMin = yAxis.ScaleView.ViewMinimum;
                var yMax = yAxis.ScaleView.ViewMaximum;
                var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;
                xAxis.ScaleView.Zoom(posXStart, posXFinish);
                yAxis.ScaleView.Zoom(posYStart, posYFinish);
            }
        }

        /// <summary>
        /// 追加温度点到字符串构建器（用于冷却过程）
        /// </summary>
        /// <param name="builder">字符串构建器</param>
        /// <param name="currentTemperature">当前温度</param>
        /// <param name="ambientTemperature">环境温度</param>
        /// <param name="coolingConstant">冷却常数</param>
        /// <param name="points">点数</param>
        private static  void AppendTemperaturePoints(StringBuilder builder, ref double currentTemperature, double ambientTemperature, double coolingConstant, int points)
        {
            for (int i = 0; i < points; i++)
            {
                currentTemperature -= coolingConstant * (currentTemperature - ambientTemperature) / 2;
                currentTemperature = Math.Max(currentTemperature, MinTemperature); // 限制下限
                builder.Append($"{currentTemperature}@");
            }
        }

        /// <summary>
        /// 根据工艺步骤数组生成温度和工艺数据
        /// </summary>
        /// <param name="processSteps">工艺步骤数组</param>
        /// <param name="intervalSeconds">温度记录间隔（秒）</param>
        /// <returns>ChartData结构体</returns>
        public static  ChartData GenerateChartData(ProcessStep[] processSteps, int intervalSeconds = 30)
        {
            var temperatureBuilder = new StringBuilder();
            var craftBuilder = new StringBuilder();
            double currentTemperature = 25.0; // 初始温度
            int timePoint = 0;
            const double ambientTemperature = 25.0;
            const double coolingConstant = 0.04;

            foreach (var step in processSteps)
            {
                double fixedDuration = 0, duration = 0;
                craftBuilder.Append($"{step.StepName},{(timePoint == 0 ? 1 : timePoint)}@");

                int points;
                switch (step.StepName)
                {
                    // 固定时间步骤
                    case "放布":
                    case "出布":
                    case "取小样":
                    case "测PH":
                        fixedDuration = 3;
                        points = (int)Math.Round(fixedDuration * 60 / intervalSeconds);
                        AppendTemperaturePoints(temperatureBuilder, ref currentTemperature, ambientTemperature, coolingConstant, points);
                        timePoint += points;
                        break;
                    // 加料步骤
                    case "加A":
                    case "加B":
                    case "加C":
                    case "加D":
                    case "加E":
                    case "加F":
                    case "加G":
                    case "加H":
                    case "加I":
                    case "加J":
                    case "加K":
                    case "加L":
                    case "加M":
                    case "加N":
                        fixedDuration = 3;
                        points = (int)Math.Round(fixedDuration * 60 / intervalSeconds);
                        AppendTemperaturePoints(temperatureBuilder, ref currentTemperature, ambientTemperature, coolingConstant, points);
                        timePoint += points;
                        break;
                    // 洗杯步骤（机械手准备+加水）
                    case "洗杯":
                        // 机械手准备阶段：1分钟缓慢降温
                        int prepareSeconds = 120;
                        int preparePoints = (int)Math.Round(prepareSeconds / (double)intervalSeconds);
                        AppendTemperaturePoints(temperatureBuilder, ref currentTemperature, ambientTemperature, coolingConstant, preparePoints);

                        // 加水阶段：6秒线性快速降温
                        int addWaterSeconds = 120;
                        int addWaterPoints = Math.Max(1, (int)Math.Round(addWaterSeconds / (double)intervalSeconds));
                        double startTemp = currentTemperature;
                        double afterWaterTemp = startTemp - (startTemp - ambientTemperature) * 0.75; // 75%降温
                        for (int i = 0; i < addWaterPoints; i++)
                        {
                            currentTemperature = startTemp - (startTemp - afterWaterTemp) * ((i + 1.0) / addWaterPoints);
                            currentTemperature = Math.Max(currentTemperature, MinTemperature); // 限制下限
                            temperatureBuilder.Append($"{currentTemperature}@");
                        }

                        // 剩余时间（如有）继续缓慢降温
                        int totalSeconds = (int)(12 * 60); // 洗杯总时长10分钟
                        int leftSeconds = totalSeconds - prepareSeconds - addWaterSeconds;
                        int leftPoints = (int)Math.Round(leftSeconds / (double)intervalSeconds);
                        AppendTemperaturePoints(temperatureBuilder, ref currentTemperature, ambientTemperature, coolingConstant, leftPoints);

                        timePoint += preparePoints + addWaterPoints + leftPoints;
                        break;
                    // 排液步骤
                    case "排液":
                        duration = 2;
                        points = (int)Math.Round(duration * 60 / intervalSeconds);
                        AppendTemperaturePoints(temperatureBuilder, ref currentTemperature, ambientTemperature, coolingConstant, points);
                        timePoint += points;
                        break;
                    // 加水步骤
                    case "加水":
                        duration = 2;
                        points = (int)Math.Round(duration * 60 / intervalSeconds);
                        AppendTemperaturePoints(temperatureBuilder, ref currentTemperature, ambientTemperature, coolingConstant, points);
                        timePoint += points;
                        break;
                    // 冷行/搅拌步骤
                    case "冷行":
                    case "搅拌":
                        duration = 5;
                        points = (int)Math.Round(duration * 60 / intervalSeconds);
                        AppendTemperaturePoints(temperatureBuilder, ref currentTemperature, ambientTemperature, coolingConstant, points);
                        timePoint += points;
                        break;
                    // 温控步骤（升温+保温）
                    case "温控":
                        double targetTemperature = (double)step.TargetTemperature;
                        double holdTime = (double)step.Duration;
                        double heatingRate = (double)step.HeatingRate;
                        double temperatureDifference = targetTemperature - currentTemperature;
                        double heatingTime = Math.Abs(temperatureDifference) / heatingRate;
                        int heatingPoints = (int)Math.Round(heatingTime * 60 / intervalSeconds);
                        for (int i = 0; i < heatingPoints; i++)
                        {
                            currentTemperature += (temperatureDifference > 0 ? heatingRate : -heatingRate) * intervalSeconds / 60.0;
                            currentTemperature = Math.Max(currentTemperature, MinTemperature); // 限制下限
                            temperatureBuilder.Append($"{currentTemperature}@");
                        }
                        timePoint += heatingPoints;
                        int holdPoints = (int)Math.Round(holdTime * 60 / intervalSeconds);
                        for (int i = 0; i < holdPoints; i++)
                        {
                            currentTemperature = Math.Max(currentTemperature, MinTemperature); // 限制下限
                            temperatureBuilder.Append($"{currentTemperature}@");
                        }
                        timePoint += holdPoints;
                        break;
                    default:
                        throw new ArgumentException($"未知的工艺步骤: {step.StepName}");
                }
            }

            return new ChartData
            {
                temperature = temperatureBuilder.ToString().TrimEnd('@'),
                craft = craftBuilder.ToString().TrimEnd('@')
            };
        }

        /// <summary>
        /// 获取总时间格式化字符串
        /// </summary>
        /// <param name="pointCount">温度点数量</param>
        /// <param name="intervalSeconds">间隔秒数</param>
        /// <returns>格式化时间字符串</returns>
        public string GetTotalTimeFormatted(int pointCount, int intervalSeconds = 30)
        {
            double totalTimeInSeconds = pointCount * intervalSeconds;
            TimeSpan totalTime = TimeSpan.FromSeconds(totalTimeInSeconds);
            return $"{totalTime.Hours:D2}:{totalTime.Minutes:D2}:{totalTime.Seconds:D2}";
        }

        /// <summary>
        /// 获取总时间的TimeSpan对象
        /// </summary>
        /// <param name="pointCount">温度点数量</param>
        /// <param name="intervalSeconds">间隔秒数</param>
        /// <returns>总时间TimeSpan</returns>
        public TimeSpan GetTotalTimeSpan(int pointCount, int intervalSeconds = 30)
        {
            double totalTimeInSeconds = pointCount * intervalSeconds;
            return TimeSpan.FromSeconds(totalTimeInSeconds);
        }

        /// <summary>
        /// X轴视图变化事件，动态调整X轴刻度和标签格式
        /// </summary>
        private void AxisX_ViewChanged(object sender, ViewEventArgs e)
        {
            if (e.Axis.AxisName != AxisName.X) return;
            var area = Chart.ChartAreas[0];
            double min = e.Axis.ScaleView.ViewMinimum;
            double max = e.Axis.ScaleView.ViewMaximum;
            TimeSpan span = DateTime.FromOADate(max) - DateTime.FromOADate(min);

            if (span.TotalHours > 24)
            {
                area.AxisX.IntervalType = DateTimeIntervalType.Hours;
                area.AxisX.Interval = 6;
                area.AxisX.LabelStyle.Format = "MM-dd HH:mm";
            }
            else if (span.TotalHours > 6)
            {
                area.AxisX.IntervalType = DateTimeIntervalType.Hours;
                area.AxisX.Interval = 1;
                area.AxisX.LabelStyle.Format = "HH:mm";
            }
            else if (span.TotalMinutes > 60)
            {
                area.AxisX.IntervalType = DateTimeIntervalType.Minutes;
                area.AxisX.Interval = 30;
                area.AxisX.LabelStyle.Format = "HH:mm";
            }
            else if (span.TotalMinutes > 30)
            {
                area.AxisX.IntervalType = DateTimeIntervalType.Minutes;
                area.AxisX.Interval = 10;
                area.AxisX.LabelStyle.Format = "HH:mm";
            }
            else if (span.TotalMinutes > 10)
            {
                area.AxisX.IntervalType = DateTimeIntervalType.Minutes;
                area.AxisX.Interval = 2;
                area.AxisX.LabelStyle.Format = "HH:mm";
            }
            else
            {
                area.AxisX.IntervalType = DateTimeIntervalType.Minutes;
                area.AxisX.Interval = 1;
                area.AxisX.LabelStyle.Format = "HH:mm:ss";
            }
        }

        /// <summary>
        /// 图表绘制后事件，绘制工艺标记的箭头和文本
        /// </summary>
        private void Chart_PostPaint(object sender, ChartPaintEventArgs e)
        {
            if (!(e.ChartElement is ChartArea area) || craftsData == null || times == null)
                return;

            var mainSeries = Chart.Series.IndexOf("温度") >= 0 ? Chart.Series["温度"] : null;
            if (mainSeries == null || mainSeries.Points.Count == 0)
                return;

            Graphics g = e.ChartGraphics.Graphics;
            int topRowCount = 2, bottomRowCount = 2;
            int topIndex = 0, bottomIndex = 0;

            // 获取图表区域像素范围
            float chartLeft = (float)Chart.ChartAreas[0].AxisX.ValueToPixelPosition(area.AxisX.Minimum);
            float chartRight = (float)Chart.ChartAreas[0].AxisX.ValueToPixelPosition(area.AxisX.Maximum);
            float chartTop = (float)Chart.ChartAreas[0].AxisY.ValueToPixelPosition(area.AxisY.Maximum);
            float chartBottom = (float)Chart.ChartAreas[0].AxisY.ValueToPixelPosition(area.AxisY.Minimum);

            for (int i = 0; i < craftsData.Length; i++)
            {
                var arr = craftsData[i].Split(',');
                if (arr.Length == 2 && int.TryParse(arr[1], out int idx) && idx > 0 && idx <= times.Length)
                {
                    int pointIdx = idx - 1;
                    double xValue = times[pointIdx].ToOADate();
                    float xPixel = (float)Chart.ChartAreas[0].AxisX.ValueToPixelPosition(xValue);

                    bool isTop = (i % 2 == 0);
                    int row = isTop ? (topIndex++ % topRowCount) : (bottomIndex++ % bottomRowCount);

                    float margin = 8;
                    float textHeight = 18;
                    float yText = isTop
                        ? chartTop - margin - (topRowCount - row - 1) * textHeight
                        : chartBottom + margin + row * textHeight;

                    float yPoint = (float)Chart.ChartAreas[0].AxisY.ValueToPixelPosition(mainSeries.Points[pointIdx].YValues[0]);

                    // 检查像素坐标是否有效，防止溢出异常
                    if (float.IsNaN(chartTop) || float.IsInfinity(chartTop) ||
                        float.IsNaN(chartBottom) || float.IsInfinity(chartBottom) ||
                        float.IsNaN(yPoint) || float.IsInfinity(yPoint) ||
                        Math.Abs(chartTop) > 10000 || Math.Abs(chartBottom) > 10000 || Math.Abs(yPoint) > 10000)
                    {
                        continue;
                    }

                    // Y方向边界检测，防止标签超出图表高度
                    if (isTop && yText < chartTop + 2)
                        yText = chartTop + 2;
                    if (!isTop && yText + textHeight > chartBottom - 2)
                        yText = chartBottom - textHeight - 2;

                    // 重新计算箭头终点，使其指向文本边缘
                    float arrowStartY = yPoint;
                    float arrowEndY = isTop ? yText + textHeight : yText;

                    // 绘制箭头
                    using (var pen = new Pen(Color.LightSkyBlue, 1.2f)
                    {
                        DashStyle = System.Drawing.Drawing2D.DashStyle.Dot,
                        CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(4, 6)
                    })
                    {
                        g.DrawLine(pen, xPixel, arrowStartY, xPixel, arrowEndY);
                    }

                    // 绘制工艺文本（边界检测与偏移）
                    using (var font = new Font("微软雅黑", 9, FontStyle.Bold))
                    using (var brush = new SolidBrush(Color.FromArgb(160, 120, 180, 255)))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                        string text = arr[0];
                        SizeF textSize = g.MeasureString(text, font);

                        float xText = xPixel;
                        // 左边界检测
                        if (xText - textSize.Width / 2 < chartLeft)
                            xText = chartLeft + textSize.Width / 2 + 2;
                        // 右边界检测
                        if (xText + textSize.Width / 2 > chartRight)
                            xText = chartRight - textSize.Width / 2 - 2;

                        g.DrawString(text, font, brush, xText, yText, sf);
                    }
                }
            }
        }

        /// <summary>
        /// 根据工艺步骤数组生成温度曲线并可选显示总时间
        /// </summary>
        /// <param name="steps">工艺步骤数组</param>
        /// <param name="temRecordInterval">温度记录间隔（秒）</param>
        /// <param name="totalTimeTextBox">显示总时间的文本框（可选）</param>
        public void ShowProcessCurve(
            ProcessStep[] steps,
            int temRecordInterval,
            System.Windows.Forms.TextBox totalTimeTextBox = null)
        {
            if (steps == null || steps.Length == 0)
            {
                ClearChart();
                if (totalTimeTextBox != null) totalTimeTextBox.Clear();
                return;
            }

            // 生成温度和工艺数据
            var chartData = GenerateChartData(steps, temRecordInterval);
            var temperatureArr = chartData.temperature.Split('@');
            var craftArr = chartData.craft.Split('@');

            // 设置数据到图表
            SetDataWithCraft(
                temperatureArr,
                craftArr,
                DateTime.Now,
                temRecordInterval
            );

            // 显示总时间
            string totalTime = GetTotalTimeFormatted(temperatureArr.Length, temRecordInterval);
            if (totalTimeTextBox != null)
                totalTimeTextBox.Text = totalTime;
        }

        public static DateTime EstimateFinishTime(
             int currentStepIndex,
             DateTime currentStepStartTime,
             ProcessStep[] processSteps,
             int intervalSeconds = 30)
        {
            if (processSteps == null || processSteps.Length == 0)
                throw new ArgumentException("工艺步骤数组不能为空");
            if (currentStepIndex < 0 || currentStepIndex >= processSteps.Length)
                throw new ArgumentOutOfRangeException(nameof(currentStepIndex), "当前步骤索引超出范围");

            double currentTemperature = 25.0; // 初始温度
            const double ambientTemperature = 25.0;
            const double coolingConstant = 0.04;
            int totalPoints = 0;

            // 先模拟 currentStepIndex 之前的温度变化，得到当前步的起始温度
            for (int i = 0; i < currentStepIndex; i++)
            {
                var step = processSteps[i];
                switch (step.StepName)
                {
                    case "放布":
                    case "出布":
                    case "取小样":
                    case "测PH":
                    case "加A":
                    case "加B":
                    case "加C":
                    case "加D":
                    case "加E":
                    case "加F":
                    case "加G":
                    case "加H":
                    case "加I":
                    case "加J":
                    case "加K":
                    case "加L":
                    case "加M":
                    case "加N":
                        {
                            double fixedDuration = 3;
                            int points = (int)Math.Round(fixedDuration * 60 / intervalSeconds);
                            for (int j = 0; j < points; j++)
                                currentTemperature -= coolingConstant * (currentTemperature - ambientTemperature) / 2;
                            break;
                        }
                    case "洗杯":
                        {
                            int preparePoints = (int)Math.Round(120.0 / intervalSeconds);
                            for (int j = 0; j < preparePoints; j++)
                                currentTemperature -= coolingConstant * (currentTemperature - ambientTemperature) / 2;

                            int addWaterPoints = Math.Max(1, (int)Math.Round(120.0 / intervalSeconds));
                            double startTemp = currentTemperature;
                            double afterWaterTemp = startTemp - (startTemp - ambientTemperature) * 0.75;
                            for (int j = 0; j < addWaterPoints; j++)
                                currentTemperature = startTemp - (startTemp - afterWaterTemp) * ((j + 1.0) / addWaterPoints);

                            int leftPoints = (int)Math.Round((12 * 60 - 120 - 120) / (double)intervalSeconds);
                            for (int j = 0; j < leftPoints; j++)
                                currentTemperature -= coolingConstant * (currentTemperature - ambientTemperature) / 2;
                            break;
                        }
                    case "排液":
                    case "加水":
                        {
                            double duration = 2;
                            int points = (int)Math.Round(duration * 60 / intervalSeconds);
                            for (int j = 0; j < points; j++)
                                currentTemperature -= coolingConstant * (currentTemperature - ambientTemperature) / 2;
                            break;
                        }
                    case "冷行":
                    case "搅拌":
                        {
                            double duration = 5;
                            int points = (int)Math.Round(duration * 60 / intervalSeconds);
                            for (int j = 0; j < points; j++)
                                currentTemperature -= coolingConstant * (currentTemperature - ambientTemperature) / 2;
                            break;
                        }
                    case "温控":
                        {
                            if (step.TargetTemperature.HasValue && step.HeatingRate.HasValue && step.Duration.HasValue)
                            {
                                double targetTemperature = step.TargetTemperature.Value;
                                double heatingRate = step.HeatingRate.Value;
                                double holdTime = step.Duration.Value;
                                double temperatureDifference = targetTemperature - currentTemperature;
                                double heatingTime = Math.Abs(temperatureDifference) / heatingRate;
                                int heatingPoints = (int)Math.Round(heatingTime * 60 / intervalSeconds);
                                for (int j = 0; j < heatingPoints; j++)
                                    currentTemperature += (temperatureDifference > 0 ? heatingRate : -heatingRate) * intervalSeconds / 60.0;
                                int holdPoints = (int)Math.Round(holdTime * 60 / intervalSeconds);
                                // 保温阶段温度不变
                            }
                            break;
                        }
                    default:
                        throw new ArgumentException($"未知的工艺步骤: {step.StepName}");
                }
            }

            // 再模拟 currentStepIndex 及后续所有步骤，统计点数
            for (int i = currentStepIndex; i < processSteps.Length; i++)
            {
                var step = processSteps[i];
                switch (step.StepName)
                {
                    case "放布":
                    case "出布":
                    case "取小样":
                    case "测PH":
                    case "加A":
                    case "加B":
                    case "加C":
                    case "加D":
                    case "加E":
                    case "加F":
                    case "加G":
                    case "加H":
                    case "加I":
                    case "加J":
                    case "加K":
                    case "加L":
                    case "加M":
                    case "加N":
                        {
                            double fixedDuration = 3;
                            int points = (int)Math.Round(fixedDuration * 60 / intervalSeconds);
                            totalPoints += points;
                            for (int j = 0; j < points; j++)
                                currentTemperature -= coolingConstant * (currentTemperature - ambientTemperature) / 2;
                            break;
                        }
                    case "洗杯":
                        {
                            int preparePoints = (int)Math.Round(120.0 / intervalSeconds);
                            totalPoints += preparePoints;
                            for (int j = 0; j < preparePoints; j++)
                                currentTemperature -= coolingConstant * (currentTemperature - ambientTemperature) / 2;

                            int addWaterPoints = Math.Max(1, (int)Math.Round(120.0 / intervalSeconds));
                            totalPoints += addWaterPoints;
                            double startTemp = currentTemperature;
                            double afterWaterTemp = startTemp - (startTemp - ambientTemperature) * 0.75;
                            for (int j = 0; j < addWaterPoints; j++)
                                currentTemperature = startTemp - (startTemp - afterWaterTemp) * ((j + 1.0) / addWaterPoints);

                            int leftPoints = (int)Math.Round((12 * 60 - 120 - 120) / (double)intervalSeconds);
                            totalPoints += leftPoints;
                            for (int j = 0; j < leftPoints; j++)
                                currentTemperature -= coolingConstant * (currentTemperature - ambientTemperature) / 2;
                            break;
                        }
                    case "排液":
                    case "加水":
                        {
                            double duration = 2;
                            int points = (int)Math.Round(duration * 60 / intervalSeconds);
                            totalPoints += points;
                            for (int j = 0; j < points; j++)
                                currentTemperature -= coolingConstant * (currentTemperature - ambientTemperature) / 2;
                            break;
                        }
                    case "冷行":
                    case "搅拌":
                        {
                            double duration = 5;
                            int points = (int)Math.Round(duration * 60 / intervalSeconds);
                            totalPoints += points;
                            for (int j = 0; j < points; j++)
                                currentTemperature -= coolingConstant * (currentTemperature - ambientTemperature) / 2;
                            break;
                        }
                    case "温控":
                        {
                            if (step.TargetTemperature.HasValue && step.HeatingRate.HasValue && step.Duration.HasValue)
                            {
                                double targetTemperature = step.TargetTemperature.Value;
                                double heatingRate = step.HeatingRate.Value;
                                double holdTime = step.Duration.Value;
                                double temperatureDifference = targetTemperature - currentTemperature;
                                double heatingTime = Math.Abs(temperatureDifference) / heatingRate;
                                int heatingPoints = (int)Math.Round(heatingTime * 60 / intervalSeconds);
                                totalPoints += heatingPoints;
                                for (int j = 0; j < heatingPoints; j++)
                                    currentTemperature += (temperatureDifference > 0 ? heatingRate : -heatingRate) * intervalSeconds / 60.0;
                                int holdPoints = (int)Math.Round(holdTime * 60 / intervalSeconds);
                                totalPoints += holdPoints;
                                // 保温阶段温度不变
                            }
                            break;
                        }
                    default:
                        throw new ArgumentException($"未知的工艺步骤: {step.StepName}");
                }
            }

            double totalSeconds = totalPoints * intervalSeconds;
            return currentStepStartTime.AddSeconds(totalSeconds);
        }

        /// <summary>
        /// 计算预计完成时间，并返回“HH:mm:ss”格式的字符串
        /// </summary>
        /// <param name="currentStepIndex">当前步骤索引（从0开始）</param>
        /// <param name="currentStepStartTime">当前步骤开始时间</param>
        /// <param name="processSteps">工艺步骤数组</param>
        /// <param name="intervalSeconds">温度记录间隔（秒）</param>
        /// <returns>预计完成时间（如“14:23:45”）</returns>
        public static string GetEstimatedFinishTimeString(
            int currentStepIndex,
            DateTime currentStepStartTime,
            ProcessStep[] processSteps,
            int intervalSeconds = 30)
        {
            DateTime finishTime = EstimateFinishTime(currentStepIndex, currentStepStartTime, processSteps, intervalSeconds);
            return finishTime.ToString("HH:mm:ss");
        }

        private void Tsmi_Process_Click(object sender, EventArgs e)
        {
            ShowModeChanged?.Invoke(this, 1);
        }

        private void Tmsi_Measured_Click(object sender, EventArgs e)
        {
            ShowModeChanged?.Invoke(this, 0);
        }

        private void Tsmi_Both_Click(object sender, EventArgs e)
        {
            ShowModeChanged?.Invoke(this, 2);
        }
    }
}