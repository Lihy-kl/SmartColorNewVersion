using SmartColor.My_AutomaticModule;
using SmartColor.My_Cup;
using SmartColor.My_DataBase; // 引入TableDefinition命名空间以便使用CUP_DETAILS
using SmartColor.My_File;
using SmartColor.My_Form.Homepage;
using SmartColor.My_Form.Login;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 配液杯控件
    /// </summary>
    public partial class CtCup : UserControl, My_Interface.ICustomUpdatable
    {
        public event EventHandler CurrentStepNoChanged;

        private Rectangle _workingRect;
        private string _title = "";
        private Color _bottleColor = Color.Black;
        private Color _liquidColor = Color.DeepSkyBlue;
        private decimal _maxValue = 1;
        private double _actualTemp = 0;
        private decimal _value = 0;
        private string _no = "1";
        private string _status = null; // 状态，默认不显示
        private int _coverStatus = 0; // 0-无盖，1-有盖
        private int _lockStatus = 0; // 0-未锁，1-已锁
        private int _fail = 0; // 0-正常，1-失败
        private int _currentStepNo = 0; // 当前步号
        private int _holdingTime = 0; // 保温时间（分）
        private string _technologyName = string.Empty;//操作名称
        private int _drainageDown = 0;//排水下到位信号
        private int _safeOpeningTemp = 90; // 安全开盖温度
        private bool _hasFabric = false; // 是否有布

        public CtCup()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            DoubleBuffered = true; // 强制开启双缓冲
            AutoScaleMode = AutoScaleMode.None;
            SizeChanged += CtCup_SizeChanged;
            Size = new Size(40, 80);
            BackColor = Color.Transparent;

            RefreshLidStatusButtonVisibility();
            RefreshHighWashButtonVisibility(); // 新增：根据HighWash显示高温洗杯按钮
            // 防止重复绑定事件
            LoginForm.UserChanged -= LoginForm_UserChanged;
            LoginForm.UserChanged += LoginForm_UserChanged;
            SmartColor.My_ConPar.Choices.HighWashChanged -= Choices_HighWashChanged;
            SmartColor.My_ConPar.Choices.HighWashChanged += Choices_HighWashChanged;
        }

        private void Choices_HighWashChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(RefreshHighWashButtonVisibility));
            else
                RefreshHighWashButtonVisibility();
        }

        /// <summary>
        /// 根据Choices.HighWash刷新“高温洗杯”菜单项的可见性
        /// </summary>
        private void RefreshHighWashButtonVisibility()
        {
            TsmiHighTemWash.Visible = SmartColor.My_ConPar.Choices.HighWash == 1;
        }

        private void LoginForm_UserChanged(object sender, EventArgs e)
        {
            // UI线程安全更新
            if (this.InvokeRequired)
                this.Invoke(new Action(RefreshLidStatusButtonVisibility));
            else
                RefreshLidStatusButtonVisibility();
        }

        /// <summary>
        /// 根据当前登录用户的权限，刷新“切换杯盖状态”菜单项的可见性
        /// </summary>
        private void RefreshLidStatusButtonVisibility()
        {
            var account = SmartColor.Properties.Settings.Default.Account;
            if (LoginForm.UserCache != null && LoginForm.UserCache.TryGetValue(account, out var userInfo))
            {
                TmsiChangeLidStatus.Visible = (userInfo.Purview == 2); // 仅权限为2的用户可见
            }
            else
            {
                TmsiChangeLidStatus.Visible = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LoginForm.UserChanged -= LoginForm_UserChanged;
                components?.Dispose();
            }
            base.Dispose(disposing);
        }


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // base.OnPaintBackground(e); // 注释掉，减少背景重绘
            if (Parent != null)
            {
                var eClip = e.ClipRectangle;
                e.Graphics.TranslateTransform(-Left, -Top);
                var pe = new PaintEventArgs(e.Graphics, eClip);
                InvokePaintBackground(Parent, pe);
                InvokePaint(Parent, pe);
                e.Graphics.TranslateTransform(Left, Top);
            }
        }

        [Description("配方名称"), Category("自定义")]
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

        [Description("操作名称"), Category("自定义")]
        public string TechnologyName
        {
            get => _technologyName;
            set
            {
                if (_technologyName != value)
                {
                    _technologyName = value;
                   
                }
            }
        }

        [Description("杯子颜色"), Category("自定义")]
        public Color BottleColor
        {
            get => _bottleColor;
            set
            {
                _bottleColor = value;
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



        [Description("有无布"), Category("自定义")]
        public bool HasFabric
        {
            get => _hasFabric;
            set
            {
                if (_hasFabric != value)
                {
                    _hasFabric = value;
                    Refresh();
                }
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
                if (value < _value)
                    return;
                _maxValue = value;
                Refresh();
            }
        }

        [Description("当前步号"), Category("自定义")]
        public int CurrentStepNo
        {
            get => _currentStepNo;
            set
            {

                _currentStepNo = value;
                CurrentStepNoChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [Description("排水下到位"), Category("自定义")]
        public int DrainageDown
        {
            get => _drainageDown;
            set
            {

                _drainageDown = value;

            }
        }

        [Description("安全开盖温度"), Category("自定义")]
        public int SafeOpeningTemp
        {
            get => _safeOpeningTemp;
            set
            {

                _safeOpeningTemp = value;

            }
        }

        [Description("保温时间"), Category("自定义")]
        public int HoldingTime
        {
            get => _holdingTime;
            set
            {

                _holdingTime = value;

            }
        }

        [Description("值"), Category("自定义")]
        public decimal Value
        {
            get => _value;
            set
            {
                if (value < 0)
                    return;
                _value = value > _maxValue ? _maxValue : value;
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

        [Description("杯子状态"), Category("自定义")]
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                Refresh();
            }
        }



        /// <summary>
        /// 杯盖状态，0-无盖，1-有盖
        /// </summary>
        [Description("杯盖状态"), Category("自定义")]
        public int CoverStatus
        {
            get => _coverStatus;
            set
            {
                _coverStatus = value;
                Refresh();
            }
        }

        /// <summary>
        /// 锁定状态，0-未锁，1-已锁
        /// </summary>
        [Description("锁定状态"), Category("自定义")]
        public int LockStatus
        {
            get => _lockStatus;
            set
            {
                _lockStatus = value;
                Refresh();
            }
        }

        /// <summary>
        /// 失败，0-成功，1-失败
        /// </summary>
        [Description("是否失败"), Category("自定义")]

        public int Fail
        {
            get => _fail;
            set
            {
                _fail = value;
                Refresh();
            }
        }

        /// <summary>
        /// 实际温度
        /// </summary>
        [Description("实际温度"), Category("自定义")]

        public double ActualTemp
        {
            get => _actualTemp;
            set
            {
                _actualTemp = value;
            }
        }

        private void CtCup_SizeChanged(object sender, EventArgs e)
        {
            ResetWorkingRect();
        }

        /// <summary>
        /// 重新计算杯身区域
        /// </summary>
        private void ResetWorkingRect()
        {
            try
            {
                using (var g = CreateGraphics())
                {
                    g.MeasureString(_title, Font);
                    _workingRect = new Rectangle(0, 10, Width, Height - 35);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CtCup ResetWorkingRect异常", ex);
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

                if (_workingRect.Width <= 0 || _workingRect.Height <= 0)
                    return;

                // 杯子颜色根据锁定状态变化
                Color drawBottleColor = (_lockStatus == 1) ? Color.Orange : _bottleColor;
                // 液体颜色根据失败状态变化
                Color liquidColor = (_fail == 1) ? Color.Red : _liquidColor;

                using (var bottlePen = new Pen(drawBottleColor, 1))
                {
                    // 画空杯子
                    Point[] cupPoints = {
                        new Point(_workingRect.Left, _workingRect.Top),
                        new Point(_workingRect.Right - 1, _workingRect.Top),
                        new Point(_workingRect.Right - 1, _workingRect.Bottom),
                        new Point(_workingRect.Left, _workingRect.Bottom)
                    };
                    using (var cupPath = new GraphicsPath())
                    {
                        cupPath.AddLines(cupPoints);
                        cupPath.CloseAllFigures();
                        g.DrawPolygon(bottlePen, cupPoints);
                    }

                    // 画液体
                    decimal liquidHeight = (_maxValue > 0) ? (_value / _maxValue) * _workingRect.Height : 0;
                    PointF[] liquidPoints = {
                        new PointF(_workingRect.Left + 1, (float)(_workingRect.Bottom - liquidHeight)),
                        new PointF(_workingRect.Right - 1, (float)(_workingRect.Bottom - liquidHeight)),
                        new PointF(_workingRect.Right - 1, _workingRect.Bottom),
                        new PointF(_workingRect.Left + 1, _workingRect.Bottom)
                    };
                    using (var liquidPath = new GraphicsPath())
                    {
                        liquidPath.AddLines(liquidPoints);
                        liquidPath.CloseAllFigures();
                        g.FillPath(new SolidBrush(liquidColor), liquidPath);
                        g.FillPath(new SolidBrush(Color.FromArgb(50, liquidColor)), liquidPath);
                    }

                    // 画杯口（只有有盖时才显示）
                    if (_coverStatus == 1)
                    {
                        Point[] mouthPoints = {
                            new Point(_workingRect.Left, _workingRect.Top - 7),
                            new Point(_workingRect.Right - 1, _workingRect.Top - 7),
                            new Point(_workingRect.Right - 1, _workingRect.Top),
                            new Point(_workingRect.Left, _workingRect.Top)
                        };
                        using (var mouthPath = new GraphicsPath())
                        {
                            mouthPath.AddLines(mouthPoints);
                            mouthPath.CloseAllFigures();
                            g.DrawPolygon(bottlePen, mouthPoints);
                        }
                    }
                }

                // 画布（如果有布）
                if (_hasFabric)
                {
                    // 计算布的区域，放在液体上方或杯底中间
                    int fabricHeight = Math.Max(10, _workingRect.Height / 2);
                    int fabricWidth = Math.Max(10, _workingRect.Width / 2);
                    int fabricX = _workingRect.Left + (_workingRect.Width - fabricWidth) / 2;
                    // 向上移动圆柱体，比如上移20像素
                    int fabricY = _workingRect.Bottom - fabricHeight - 10; // 原来是-5，改为-25

                    // 空心圆柱体（椭圆+矩形，只画边框）
                    using (var pen = new Pen(Color.Peru, 2))
                    {
                        // 圆柱体主体
                        g.DrawRectangle(pen, fabricX, fabricY, fabricWidth, fabricHeight);
                        // 圆柱体底部椭圆
                        g.DrawEllipse(pen, fabricX, fabricY + fabricHeight - 4, fabricWidth, 8);
                        // 圆柱体顶部椭圆
                        g.DrawEllipse(pen, fabricX, fabricY - 4, fabricWidth, 8);
                    }
                }

                // 写编号
                if (!string.IsNullOrEmpty(_no))
                {
                    var noSize = g.MeasureString(_no, Font);
                    g.DrawString(_no, Font, new SolidBrush(ForeColor),
                        new PointF((Width - noSize.Width) / 2, _workingRect.Top + _workingRect.Height * 0.05f));
                }

                // 写配方名称（自动换行）
                var titleSize = g.MeasureString(_title, Font);
                string displayTitle;
                if (titleSize.Width > Width)
                {
                    var sb = new StringBuilder();
                    var line = new StringBuilder();
                    foreach (char c in _title)
                    {
                        line.Append(c);
                        var sz = g.MeasureString(line.ToString(), Font);
                        if (sz.Width > Width)
                        {
                            line.Length--; // 移除最后一个字符
                            sb.AppendLine(line.ToString());
                            line.Clear();
                            line.Append(c);
                        }
                    }
                    sb.Append(line);
                    displayTitle = sb.ToString();
                }
                else
                {
                    displayTitle = _title;
                }
                var displaySize = g.MeasureString(displayTitle, Font);
                var noSize2 = g.MeasureString(_no, Font);
                g.DrawString(displayTitle, Font, new SolidBrush(ForeColor),
                    new PointF((Width - displaySize.Width) / 2, _workingRect.Top + noSize2.Height + 15));

                // 写状态（居中，位于杯子底部下方）
                if (!string.IsNullOrEmpty(_status))
                {
                    // 写状态（居中，位于杯子底部下方，始终预留空间）
                    var statusSize = g.MeasureString(_status ?? "", Font);
                    float statusY = _workingRect.Bottom + 10; // 10像素下边距
                    g.DrawString(_status ?? "", Font, new SolidBrush(ForeColor),
                        new PointF((Width - statusSize.Width) / 2, statusY));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CtCup OnPaint异常", ex);
            }
        }

        private void CtCup_Click(object sender, EventArgs e)
        {
            using (var fmr = new My_Form.Homepage.CupInfo(this))
            {
                fmr.ShowDialog();
            }
        }

        public string ControlKey => NO;

       
        /// <summary>
        /// 根据数据行更新控件属性，字段名全部使用CUP_DETAILS常量，保证与表结构一致
        /// </summary>
        /// <param name="row">数据行</param>
        public void UpdateFromData(DataRow row)
        {
            if (row == null) return;

            // 如果不是UI线程，切换到UI线程执行
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<DataRow>(UpdateFromData), row);
                return;
            }

            // 批量更新属性，使用CUP_DETAILS常量保证字段名一致
            Title = row[CUP_DETAILS.FormulaCode]?.ToString() ?? "";
            TechnologyName = row[CUP_DETAILS.TechnologyName]?.ToString() ?? "";
            NO = row[CUP_DETAILS.CupNum]?.ToString() ?? "1";
            if(Status == null)
                Status = row[CUP_DETAILS.Statues]?.ToString();

            // 数值型属性更新
            if (decimal.TryParse(row[CUP_DETAILS.TotalWeight]?.ToString(), out var totalWeight))
                MaxValue = totalWeight;

            if (decimal.TryParse(row[CUP_DETAILS.CurrentWeight]?.ToString(), out var currentWeight))
                Value = currentWeight;

            // 杯盖状态
            CoverStatus = (row.Table.Columns.Contains(CUP_DETAILS.CoverStatus) &&
                          int.TryParse(row[CUP_DETAILS.CoverStatus]?.ToString(), out var cover)) ? cover : 0;

            // 失败状态
            Fail = (row.Table.Columns.Contains(CUP_DETAILS.Fail) &&
                   int.TryParse(row[CUP_DETAILS.Fail]?.ToString(), out var failVal)) ? failVal : 0;

            // 有无布
            HasFabric = (row.Table.Columns.Contains(CUP_DETAILS.HaveCloth) &&
                        int.TryParse(row[CUP_DETAILS.HaveCloth]?.ToString(), out var haveClothVal)) && haveClothVal == 1;
        }

        /// <summary>
        /// 尝试获取杯子的上下文信息（杯号、区域、通信对象）。
        /// 如果失败，显示错误提示并返回false。
        /// </summary>
        private bool TryGetCupContext(out int cupNo, out CtCupArea area, out ICylinderComm comm)
        {
            cupNo = 0;
            area = null;
            comm = null;

            if (!int.TryParse(NO, out cupNo))
            {
               Logger.Error("杯号格式错误，操作失败！");
                return false;
            }

            area = My_AutomaticModule.CupCommManager.Instance.FindCupAreaByCupNum(cupNo);
            if (area == null)
            {
                Logger.Error("未找到对应区域，操作失败！");
                return false;
            }

            SmartColor.My_AutomaticModule.CupCommManager.Instance.EnsureCommThread(area);

            comm = SmartColor.My_AutomaticModule.CupCommManager.Instance.GetCommObject(area);
            if (comm == null)
            {
                Logger.Error("未找到通信对象，操作失败！");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 记录操作日志到运行表
        /// </summary>
        private async Task LogRunTableActionAsync(int cupNo, string actionDescription)
        {
            await RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.Machine] = actionDescription
            }, DateTime.Now);
        }

        private async void TsmiOnLine_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Status))
            {
                MessageEventManager.Instance.RequestShowBalloonTip("当前配液杯为滴液杯，无需上线！");
                return;
            }

            if (Status != "下线")
            {
                MessageEventManager.Instance.RequestShowBalloonTip("当前配液杯已上线或非下线状态，无法操作！");
                return;
            }



          

            if (!TryGetCupContext(out int cupNo, out CtCupArea area, out ICylinderComm comm))
                return;

            await LogRunTableActionAsync(cupNo, $"点击{cupNo}号母液瓶上线按钮");


         

            var cupChoice = My_Tool.CupAuxiliary.ReturnCyrrentCupChioce(cupNo);
            await comm.SendOnLine(cupNo, cupChoice);
            

            // 确保通信线程运行
            if (My_AutomaticModule.CupCommManager.Instance.ShouldStartComm(area))
                My_AutomaticModule.CupCommManager.Instance.EnsureCommThread(area);
        }

        private async void TsmiOffLine_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Status))
            {
                MessageEventManager.Instance.RequestShowBalloonTip("当前配液杯为滴液杯，无需下线！");
                return;
            }

            if (Status != "待机")
            {
                string msg = (Status == "下线") ? "当前配液杯已下线，无需重复操作！" : "当前配液杯正在使用中，不允许下线！";
                MessageEventManager.Instance.RequestShowBalloonTip(msg);
                return;
            }

            if (!TryGetCupContext(out int cupNo, out CtCupArea area, out ICylinderComm comm))
                return;

            await LogRunTableActionAsync(cupNo, $"点击{cupNo}号母液瓶下线按钮");

            var cupChoice = My_Tool.CupAuxiliary.ReturnCyrrentCupChioce(cupNo);
            await comm.SendOffLine(cupNo, cupChoice);
           

            if (My_AutomaticModule.CupCommManager.Instance.ShouldStartComm(area))
                My_AutomaticModule.CupCommManager.Instance.EnsureCommThread(area);
        }

        private async void TsmiStop_Click(object sender, EventArgs e)
        {
            if (!TryGetCupContext(out int cupNo, out _, out var comm))
                return;

            await LogRunTableActionAsync(cupNo, $"点击{cupNo}号杯停止按钮");
            await comm.SendStopAsync(cupNo);
            
        }

        private async void TsmiWash_Click(object sender, EventArgs e)
        {
            if (!TryGetCupContext(out int cupNo, out _, out var comm))
                return;
            if(Status != "待机")
            {
                MessageEventManager.Instance.RequestShowBalloonTip("当前配液杯正在使用中，不允许洗杯！");
                return;
            }
            await LogRunTableActionAsync(cupNo, $"点击{cupNo}号杯洗杯按钮");
            await comm.SendWashAsync(cupNo, My_Tool.CupAuxiliary.StopWashCupType);
           
        }

        private async void TsmiHighTemWash_Click(object sender, EventArgs e)
        {
            if (!TryGetCupContext(out int cupNo, out _, out var comm))
                return;
            if (Status != "待机")
            {
                MessageEventManager.Instance.RequestShowBalloonTip("当前配液杯正在使用中，不允许高温洗杯！");
                return;
            }
            await LogRunTableActionAsync(cupNo, $"点击{cupNo}号杯高温洗杯按钮");
            await comm.SendWashAsync(cupNo, My_Tool.CupAuxiliary.HighTempWashCupType);
           
        }

        private async void TmsiChangeLidStatus_Click(object sender, EventArgs e)
        {
            if (!TryGetCupContext(out int cupNo, out _, out var comm))
                return;

            await LogRunTableActionAsync(cupNo, $"点击{cupNo}号杯切换杯盖状态按钮");

            // 直接更新数据库并同步状态
            int newCoverStatus = (CoverStatus == 1) ? 2 : 1; // 假设状态在1和2间切换
            SqlServer.Update(CUP_DETAILS.TableName,
                 new Dictionary<string, object> { [CUP_DETAILS.CoverStatus] = newCoverStatus },
                 $"{CUP_DETAILS.CupNum}={cupNo}");
            CoverStatus = newCoverStatus;
            await comm.SyncCoverStatus(cupNo);
           
        }

        private async void TsmiOpen_Click(object sender, EventArgs e)
        {
            if (Status != "待机")
            {
                MessageEventManager.Instance.RequestShowBalloonTip("当前配液杯正在使用中，不允许开盖！");
                return;
            }
            // 安全开盖检查
            if (_actualTemp >= _safeOpeningTemp)
            {
                MessageEventManager.Instance.RequestShowBalloonTip($"当前温度({_actualTemp}°C)过高，超过安全开盖温度({_safeOpeningTemp}°C)，禁止开盖！");
                return;
            }

            if (!TryGetCupContext(out int cupNo, out _, out _))
                return;

            await LogRunTableActionAsync(cupNo, $"点击{cupNo}号杯开盖按钮");
            await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueOpenLidAsync(cupNo);
            
        }

        private async void TsmiClose_Click(object sender, EventArgs e)
        {
            // 安全关盖检查（可选项，通常关盖不强制检查温度）
            // if (_actualTemp >= _safeOpeningTemp) { ... }

            if (!TryGetCupContext(out int cupNo, out _, out _))
                return;
            if (Status != "待机")
            {
                MessageEventManager.Instance.RequestShowBalloonTip("当前配液杯正在使用中，不允许关盖！");
                return;
            }
            await LogRunTableActionAsync(cupNo, $"点击{cupNo}号杯关盖按钮");
            await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueCloseLidAsync(cupNo);
            
        }
    }
}