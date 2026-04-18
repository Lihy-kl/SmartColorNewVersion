using SmartColor.My_AutomaticModule;
using SmartColor.My_File; // 引入Logger命名空间
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 母液瓶控件
    /// </summary>
    public partial class CtBottle : UserControl, My_Interface.ICustomUpdatable
    {
        // 瓶身区域
        Rectangle m_workingRect;

        private string title = "";
        [Description("染助剂名称"), Category("自定义")]
        public string Title
        {
            get { return title; }
            set
            {
                try
                {
                    title = value;
                    ResetWorkingRect();
                    Invalidate();
                }
                catch (Exception ex)
                {
                    Logger.Error("CtBottle.Title属性设置异常", ex);
                }
            }
        }

        private Color bottleColor = Color.Black;
        [Description("瓶子颜色"), Category("自定义")]
        public Color BottleColor
        {
            get { return bottleColor; }
            set
            {
                try
                {
                    bottleColor = value;
                    Invalidate();
                }
                catch (Exception ex)
                {
                    Logger.Error("CtBottle.BottleColor属性设置异常", ex);
                }
            }
        }

        private Color liquidColor = Color.DeepSkyBlue;
        [Description("液体颜色"), Category("自定义")]
        public Color LiquidColor
        {
            get { return liquidColor; }
            set
            {
                try
                {
                    liquidColor = value;
                    Invalidate();
                }
                catch (Exception ex)
                {
                    Logger.Error("CtBottle.LiquidColor属性设置异常", ex);
                }
            }
        }

        [Description("文字字体"), Category("自定义")]
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                try
                {
                    base.Font = value;
                    ResetWorkingRect();
                    Invalidate();
                }
                catch (Exception ex)
                {
                    Logger.Error("CtBottle.Font属性设置异常", ex);
                }
            }
        }

        [Description("文字颜色"), Category("自定义")]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                try
                {
                    base.ForeColor = value;
                    Invalidate();
                }
                catch (Exception ex)
                {
                    Logger.Error("CtBottle.ForeColor属性设置异常", ex);
                }
            }
        }

        private decimal maxValue = 1200;
        [Description("最大值"), Category("自定义")]
        public decimal MaxValue
        {
            get { return maxValue; }
            set
            {
                try
                {
                    if (value < m_value)
                        return;
                    maxValue = value;
                    Invalidate();
                }
                catch (Exception ex)
                {
                    Logger.Error("CtBottle.MaxValue属性设置异常", ex);
                }
            }
        }

        private decimal m_value = 0;
        [Description("值"), Category("自定义")]
        public decimal Value
        {
            get { return m_value; }
            set
            {
                try
                {
                    if (value < 0 || value > maxValue)
                        return;
                    m_value = value;
                    Invalidate();
                }
                catch (Exception ex)
                {
                    Logger.Error("CtBottle.Value属性设置异常", ex);
                }
            }
        }

        private string m_NO = "1";
        [Description("编号"), Category("自定义")]
        public string NO
        {
            get { return m_NO; }
            set
            {
                try
                {
                    m_NO = value;
                    Invalidate();
                }
                catch (Exception ex)
                {
                    Logger.Error("CtBottle.NO属性设置异常", ex);
                }
            }
        }

        private readonly float bottleRatio = 50f / 93f; // 宽高比

        public CtBottle()
        {
            try
            {
                InitializeComponent();
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer |
                         ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                         ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
                AutoScaleMode = AutoScaleMode.None;
                SizeChanged += UCBottle_SizeChanged;
                Size = new Size(50, 93);
                BackColor = Color.Transparent;

                My_Form.Login.LoginForm.UserChanged -= LoginForm_UserChanged;
                My_Form.Login.LoginForm.UserChanged += LoginForm_UserChanged;
                UpdateAddWaterMenuVisibility();
            }
            catch (Exception ex)
            {
                Logger.Error("CtBottle构造函数异常", ex);
            }
        }

        private void LoginForm_UserChanged(object sender, EventArgs e)
        {
            UpdateAddWaterMenuVisibility();
        }

        void UCBottle_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                // 保持宽高比
                if (Width > 0 && Height > 0)
                {
                    int newWidth = Width;
                    int newHeight = Height;
                    float currentRatio = (float)Width / Height;

                    if (Math.Abs(currentRatio - bottleRatio) > 0.01f)
                    {
                        if (currentRatio > bottleRatio)
                        {
                            // 太宽，按高度调整宽度
                            newWidth = (int)(Height * bottleRatio);
                        }
                        else
                        {
                            // 太高，按宽度调整高度
                            newHeight = (int)(Width / bottleRatio);
                        }
                        // 防止递归死循环
                        if (newWidth != Width || newHeight != Height)
                        {
                            Size = new Size(newWidth, newHeight);
                            return;
                        }
                    }
                }
                ResetWorkingRect();
            }
            catch (Exception ex)
            {
                Logger.Error("CtBottle.SizeChanged事件异常", ex);
            }
        }

        private void ResetWorkingRect()
        {
            try
            {
                m_workingRect = new Rectangle(0, 10, Width, Height - 15);
            }
            catch (Exception ex)
            {
                Logger.Error("CtBottle.ResetWorkingRect异常", ex);
            }
        }

        private void UpdateAddWaterMenuVisibility()
        {
            // 获取当前用户权限，1=操作员，2=工程师
            int purview = 1;
            if (SmartColor.My_Form.Login.LoginForm.UserCache.TryGetValue(Properties.Settings.Default.Account, out var userInfo))
            {
                purview = userInfo.Purview;
            }
            // 只有工程师权限可见
            TMSIAddWater.Visible = (purview == 2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
                var g = e.Graphics;

                // 画空瓶子
                using (var pathPS = new GraphicsPath())
                using (var myPen = new Pen(bottleColor, 1))
                {
                    Point[] psPS = new Point[]
                    {
                new Point(m_workingRect.Left + m_workingRect.Width / 4, m_workingRect.Top),
                new Point(m_workingRect.Right - 1 - m_workingRect.Width / 4, m_workingRect.Top),
                new Point(m_workingRect.Right - 1, m_workingRect.Top + 15),
                new Point(m_workingRect.Right - 1, m_workingRect.Bottom),
                new Point(m_workingRect.Left, m_workingRect.Bottom),
                new Point(m_workingRect.Left, m_workingRect.Top + 15),
                    };
                    pathPS.AddLines(psPS);
                    pathPS.CloseAllFigures();
                    g.DrawPolygon(myPen, psPS);

                    // 画液体
                    decimal decYTHeight = (maxValue > 0) ? (m_value / maxValue) * m_workingRect.Height : 0;
                    float bottleTopY = m_workingRect.Top + 15;
                    float bottleBottomY = m_workingRect.Bottom;
                    float leftX = m_workingRect.Left + 1;
                    float rightX = m_workingRect.Right - 1;
                    using (var pathYT = new GraphicsPath())
                    {
                        if (decYTHeight > m_workingRect.Height - 15)
                        {
                            // 1. 先填满瓶身
                            PointF[] psYT = new PointF[]
                            {
                                new PointF(leftX, bottleTopY),
                                new PointF(rightX, bottleTopY),
                                new PointF(rightX, bottleBottomY),
                                new PointF(leftX, bottleBottomY),
                            };
                            pathYT.AddLines(psYT);
                            pathYT.CloseAllFigures();

                            using (var brushLiquid = new SolidBrush(liquidColor))
                            using (var brushLiquidAlpha = new SolidBrush(Color.FromArgb(50, liquidColor)))
                            {
                                g.FillPath(brushLiquid, pathYT);
                                g.FillPath(brushLiquidAlpha, pathYT);
                            }
                            // 2. 计算超出高度
                            float overflowHeight = (float)(decYTHeight - (m_workingRect.Height - 15));
                            if (overflowHeight > 0)
                            {
                                // 让液体继续往上延伸，宽度逐渐收窄
                                float topWidth = (rightX - leftX) * 0.5f; // 顶部宽度为瓶口宽度的一半
                                float topLeftX = leftX + (rightX - leftX - topWidth) / 2;
                                float topRightX = rightX - (rightX - leftX - topWidth) / 2;
                                float topY = bottleTopY - overflowHeight;

                                PointF[] overflowPoly = new PointF[]
                                {
                                    new PointF(leftX, bottleTopY),
                                    new PointF(rightX, bottleTopY),
                                    new PointF(topRightX, topY),
                                    new PointF(topLeftX, topY)
                                };

                                using (var pathOverflow = new GraphicsPath())
                                {
                                    pathOverflow.AddPolygon(overflowPoly);
                                    using (var brushLiquid = new SolidBrush(liquidColor))
                                    using (var brushLiquidAlpha = new SolidBrush(Color.FromArgb(50, liquidColor)))
                                    {
                                        g.FillPath(brushLiquid, pathOverflow);
                                        g.FillPath(brushLiquidAlpha, pathOverflow);
                                    }
                                }
                            }
                        }
                        else
                        {
                            PointF[] psYT = new PointF[]
                            {
                                new PointF(m_workingRect.Left + 1, (float)(m_workingRect.Bottom - decYTHeight)),
                                new PointF(m_workingRect.Right - 1, (float)(m_workingRect.Bottom - decYTHeight)),
                                new PointF(m_workingRect.Right - 1, m_workingRect.Bottom),
                                new PointF(m_workingRect.Left + 1, m_workingRect.Bottom),
                            };
                            pathYT.AddLines(psYT);
                            pathYT.CloseAllFigures();
                        }

                        using (var brushLiquid = new SolidBrush(liquidColor))
                        using (var brushLiquidAlpha = new SolidBrush(Color.FromArgb(50, liquidColor)))
                        {
                            g.FillPath(brushLiquid, pathYT);
                            g.FillPath(brushLiquidAlpha, pathYT);
                        }
                    }

                    // 画瓶口
                    using (var pathBM = new GraphicsPath())
                    {
                        Point[] psBM = new Point[]
                        {
                            new Point(m_workingRect.Left + m_workingRect.Width / 4, m_workingRect.Top - 10 + 1),
                            new Point(m_workingRect.Right - m_workingRect.Width / 4, m_workingRect.Top - 10 + 1),
                            new Point(m_workingRect.Right - m_workingRect.Width / 4, m_workingRect.Top),
                            new Point(m_workingRect.Left + m_workingRect.Width / 4, m_workingRect.Top),
                        };
                        pathBM.AddLines(psBM);
                        pathBM.CloseAllFigures();
                        g.DrawPolygon(myPen, psBM);
                    }
                }

                // 写编号（调高位置）
                float noY = m_workingRect.Top + 2;
                if (!string.IsNullOrEmpty(m_NO))
                {
                    var nosize = g.MeasureString(m_NO, Font);
                    using (var brushNO = new SolidBrush(Color.DarkBlue))
                    {
                        g.DrawString(m_NO, Font, brushNO, new PointF((Width - nosize.Width) / 2, noY));
                    }
                }

                // 写文字（自动换行+自适应字体大小）
                float titleTop = noY + g.MeasureString(m_NO, Font).Height + 5;
                var layoutRect = new RectangleF(0, titleTop, Width, m_workingRect.Bottom - titleTop);

                // 动态调整字体大小
                if (!string.IsNullOrEmpty(title))
                {
                    float fontSize = Font.Size;
                    Font bestFitFont = Font;
                    SizeF textSize;
                    using (var brushText = new SolidBrush(ForeColor))
                    {
                        do
                        {
                            using (var testFont = new Font(Font.FontFamily, fontSize, Font.Style))
                            {
                                textSize = g.MeasureString(title, testFont, (int)layoutRect.Width, new StringFormat(StringFormatFlags.LineLimit));
                                if (textSize.Height <= layoutRect.Height && textSize.Width <= layoutRect.Width)
                                {
                                    bestFitFont = (Font)testFont.Clone();
                                    break;
                                }
                            }
                            fontSize -= 0.5f;
                        } while (fontSize > 6.5);

                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Near;
                        sf.FormatFlags = StringFormatFlags.LineLimit;
                        g.DrawString(title, bestFitFont, brushText, layoutRect, sf);
                        if (bestFitFont != Font) bestFitFont.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CtBottle.OnPaint绘制异常", ex);
            }
        }

        private void CtBottle_Click(object sender, EventArgs e)
        {
            using (var frm = new My_Form.Homepage.BottleInfo(m_NO))
            {
                frm.ShowDialog();
            }
        }

        public string ControlKey => NO;

        public void UpdateFromData(System.Data.DataRow row)
        {
            if (row == null)
            {
                Title = "";
                Value = 0;
                maxValue = 0;
                return;
            }

            // 如果不是UI线程，切换到UI线程执行
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<DataRow>(UpdateFromData), row);
                return;
            }


            // 染助剂名称
            var assistantRows = My_DataBase.AssistantData.Assistant_details
                .Select($"{My_DataBase.ASSISTANT_DETAILS.AssistantCode} = '{row[My_DataBase.BOTTLE_DETAILS.AssistantCode]?.ToString()}'");
            Title = assistantRows.Length > 0 ? assistantRows[0][My_DataBase.ASSISTANT_DETAILS.AssistantName]?.ToString() : "";

            // 有效期（小时）
            var termOfValidity = Convert.ToInt32(assistantRows[0][My_DataBase.ASSISTANT_DETAILS.TermOfValidity]?.ToString());

            //开料时间
            var openTimeObj = Convert.ToDateTime(row[My_DataBase.BOTTLE_DETAILS.BrewingData]);

            //针检是否合格
            var check = row[My_DataBase.BOTTLE_DETAILS.AdjustSuccess]?.ToString();

            // 当前重量
            if (decimal.TryParse(row[My_DataBase.BOTTLE_DETAILS.CurrentWeight]?.ToString(), out decimal weight))
                Value = weight;
            else
                Value = 0;

            // 最大重量
            if (decimal.TryParse(row[My_DataBase.BOTTLE_DETAILS.AllowMaxWeight]?.ToString(), out decimal maxWeight))
                MaxValue = maxWeight;
            else
                MaxValue = 0;

            // 液体颜色逻辑
            var now = DateTime.Now;
            var hoursElapsed = (termOfValidity - Math.Abs((now - openTimeObj).TotalHours)) > 0 ?
                termOfValidity - Math.Abs((now - openTimeObj).TotalHours) : termOfValidity;
            var expired = hoursElapsed >= termOfValidity;

            if (Value >= (decimal)My_ConPar.Other.Bottle_AlarmWeight)
            {
                if (expired)
                {
                    LiquidColor = Color.Red;
                }
                else if (hoursElapsed < 4)
                {
                    LiquidColor = Color.Green;
                }
                else
                {
                    LiquidColor = Color.DeepSkyBlue; // 默认色
                }
            }
            else
            {
                if (Value < (decimal)My_ConPar.Other.Bottle_MinWeight)
                {
                    LiquidColor = Color.White;

                }
                else if (Value < (decimal)My_ConPar.Other.Bottle_AlarmWeight)
                {
                    LiquidColor = Color.Yellow;
                }

            }

            // 瓶子框颜色逻辑
            if (check == "1")
            {
                BottleColor = Color.Black;
            }
            else
            {
                BottleColor = Color.Red;
            }
        }

        private async void TSMICorrection_Click(object sender, EventArgs e)
        {
            // 校正（单瓶）
            if (!int.TryParse(m_NO, out int bottleNo))
            {
                My_File.LocalTranslator.ShowMessage("瓶号格式错误，无法校正！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 插入操作日志
            var dt = DateTime.Now;
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"点击母液瓶校正启动按钮（瓶号：{bottleNo}）"
            }, dt);

            var result = await SmartColor.My_AutomaticModule.BottleCorrectionRobotTask.EnqueueBottleCorrectionAsync(bottleNo);

            if (result.Code == My_Tool.Result.ResultCode.Success)
            {
                SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip($"校正成功：{bottleNo}");
            }
            else if (result.Code == My_Tool.Result.ResultCode.Failure)
            {
                SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                    "校正失败",
                    $"瓶号{bottleNo}校正失败，是否重试？",
                    async btn =>
                    {
                        if (btn == "重试")
                        {
                            var retryResult = await SmartColor.My_AutomaticModule.BottleCorrectionRobotTask.EnqueueBottleCorrectionAsync(bottleNo);
                            if (retryResult.Code == 0)
                            {
                                SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip($"重试校正成功：{bottleNo}");
                            }
                            else
                            {
                                SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                                    "校正失败",
                                    $"瓶号{bottleNo}多次校正失败，请检查针筒密封圈！",
                                    null,
                                    new[] { "确定" },
                                    "确定"
                                );
                            }
                        }
                    },
                    new[] { "重试", "取消" },
                    "重试"
                );
            }
            else
            {
                Logger.Error($"瓶号{bottleNo}校正异常", result.Exception);
            }
        }

        private async void TSMISelf_Click(object sender, EventArgs e)
        {
            // 自检（单瓶）
            if (!int.TryParse(m_NO, out int bottleNo))
            {
                My_File.LocalTranslator.ShowMessage("瓶号格式错误，无法自检！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 插入操作日志
            var dt = DateTime.Now;
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"点击母液瓶自检启动按钮（瓶号：{bottleNo}）"
            }, dt);

            var result = await SmartColor.My_AutomaticModule.BottleSelfRobotTask.EnqueueBottleCorrectionAsync(bottleNo);

            switch (result.Code)
            {
                case My_Tool.Result.ResultCode.Exception:
                    {
                        SmartColor.My_File.Logger.Error($"瓶号{bottleNo}自检异常", result.Exception);
                        break;
                    }
                case My_Tool.Result.ResultCode.Canceled:
                    My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("水验证任务已被取消");
                    break;
                case My_Tool.Result.ResultCode.Failure:
                    {
                        SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                        "自检失败",
                        $"瓶号{bottleNo}自检失败，是否重试？",
                        async btn =>
                        {
                            if (btn == "重试")
                            {
                                var retryResult = await SmartColor.My_AutomaticModule.BottleSelfRobotTask.EnqueueBottleCorrectionAsync(bottleNo);
                                if (retryResult.Code == 0)
                                {
                                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip($"重试自检成功：{bottleNo}");
                                }
                                else
                                {
                                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                                        "自检失败",
                                        $"瓶号{bottleNo}多次自检失败，请检查针筒密封圈！",
                                        null,
                                        new[] { "确定" },
                                        "确定"
                                    );
                                }
                            }
                        },
                        new[] { "重试", "取消" },
                        "重试"
                    );
                        break;
                    }
                default:
                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip($"自检成功：{bottleNo}");
                    break;

            }


        }

        private void TSMIABS_Click(object sender, EventArgs e)
        {

        }

        private async void TMSIWash_Click(object sender, EventArgs e)
        {
            // 洗针（单杯）
            if (!int.TryParse(m_NO, out int bottleNo))
            {
                My_File.LocalTranslator.ShowMessage("瓶号格式错误，无法洗针！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 插入操作日志
            var dt = DateTime.Now;
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"点击母液瓶洗针启动按钮（瓶号：{bottleNo}）"
            }, dt);

            var result = await SmartColor.My_AutomaticModule.WashSyringeRobotTask.EnqueueWashSyringeAsync(bottleNo);

            switch (result.Code)
            {
                case My_Tool.Result.ResultCode.Exception:
                    SmartColor.My_File.Logger.Error($"瓶号{bottleNo}洗针异常", result.Exception);
                    break;
                case My_Tool.Result.ResultCode.Canceled:
                    My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("洗针任务已被取消");
                    break;
                case My_Tool.Result.ResultCode.Failure:
                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                        "洗针失败",
                        $"瓶号{bottleNo}洗针失败，是否重试？",
                        async btn =>
                        {
                            if (btn == "重试")
                            {
                                var retryResult = await SmartColor.My_AutomaticModule.WashSyringeRobotTask.EnqueueWashSyringeAsync(bottleNo);
                                if (retryResult.Code == My_Tool.Result.ResultCode.Success)
                                {
                                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip($"重试洗针成功：{bottleNo}");
                                }
                                else
                                {
                                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                                        "洗针失败",
                                        $"瓶号{bottleNo}多次洗针失败，请检查设备！",
                                        null,
                                        new[] { "确定" },
                                        "确定"
                                    );
                                }
                            }
                        },
                        new[] { "重试", "取消" },
                        "重试"
                    );
                    break;
                default:
                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip($"洗针成功：{bottleNo}");
                    break;
            }
        }

        private void TSMIMaterialPreparationData_Click(object sender, EventArgs e)
        {
            var data = My_DataBase.SqlServer.Select(My_DataBase.PRE_BREW.TableName,
                  $"{My_DataBase.PRE_BREW.BottleNum} = @bottleNo",
                  new System.Data.SqlClient.SqlParameter("@bottleNo", m_NO));
            if (data.Rows.Count > 0)
            {
                var row = data.Rows[0];
                string info = $"瓶号：{row[My_DataBase.PRE_BREW.BottleNum]}\n" +
                    $"染助剂浓度：{row[My_DataBase.PRE_BREW.RealConcentration]}%\n" +
                    $"当前重量：{row[My_DataBase.PRE_BREW.CurrentWeight]}g\n" +
                    $"开料时间：{Convert.ToDateTime(row[My_DataBase.PRE_BREW.BrewingData]):yyyy-MM-dd HH:mm:ss}";
                DialogResult dialogResult = LocalTranslator.ShowMessage(info, "更新母液瓶数据", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.Yes)
                {
                    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
                    {
                        [My_DataBase.BOTTLE_DETAILS.CurrentWeight] = row[My_DataBase.PRE_BREW.CurrentWeight],
                        [My_DataBase.BOTTLE_DETAILS.RealConcentration] = row[My_DataBase.PRE_BREW.RealConcentration],
                        [My_DataBase.BOTTLE_DETAILS.BrewingData] = row[My_DataBase.PRE_BREW.BrewingData]

                    };
                    if (My_ConPar.Choices.CutNeedCheck == 1)
                    {
                        keyValuePairs.Add(My_DataBase.BOTTLE_DETAILS.AdjustSuccess, 0);
                    }
                    My_DataBase.SqlServer.Update(My_DataBase.BOTTLE_DETAILS.TableName,
                        keyValuePairs,
                         $"{My_DataBase.BOTTLE_DETAILS.BottleNum} = @bottleNo",
                         new System.Data.SqlClient.SqlParameter("@bottleNo", m_NO));
                    My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("母液瓶数据已更新");
                }

            }
        }

        private async void TMSIAddWater_Click(object sender, EventArgs e)
        {
            // 加水调试（单瓶）
            if (!int.TryParse(m_NO, out int bottleNo))
            {
                My_File.LocalTranslator.ShowMessage("瓶号格式错误，无法加水调试！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 插入操作日志
            var dt = DateTime.Now;
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"点击母液瓶加水调试启动按钮（瓶号：{bottleNo}）"
            }, dt);

            var result = await SmartColor.My_AutomaticModule.BottleAddWaterDebug.SingleBottle(bottleNo);

            if (result.Code == My_Tool.Result.ResultCode.Success)
            {
                SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip($"加水调试成功：{bottleNo}");
            }
            else if (result.Code == My_Tool.Result.ResultCode.Failure)
            {
                SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                    "加水调试失败",
                    $"瓶号{bottleNo}加水调试失败，是否重试？",
                    async btn =>
                    {
                        if (btn == "重试")
                        {
                            var retryResult = await SmartColor.My_AutomaticModule.BottleAddWaterDebug.SingleBottle(bottleNo);
                            if (retryResult.Code == My_Tool.Result.ResultCode.Success)
                            {
                                SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip($"重试加水调试成功：{bottleNo}");
                            }
                            else
                            {
                                SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                                    "加水调试失败",
                                    $"瓶号{bottleNo}多次加水调试失败，请检查设备！",
                                    null,
                                    new[] { "确定" },
                                    "确定"
                                );
                            }
                        }
                    },
                    new[] { "重试", "取消" },
                    "重试"
                );
            }
            else
            {
                Logger.Error($"瓶号{bottleNo}加水调试异常", result.Exception);
            }
        }
    }
}