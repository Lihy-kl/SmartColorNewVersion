using SmartColor.My_ConPar.Area;
using SmartColor.My_ConPar.Area.Balance;
using SmartColor.My_ConPar.Area.FlipCylinder;
using SmartColor.My_DataBase;
using SmartColor.My_Form.ConPar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;



namespace SmartColor.My_Form.Login
{
    /// <summary>
    /// 启动时显示的自定义 Splash 窗体，带有进度条和透明穿透功能。
    /// 支持显示当前加载阶段文本。
    /// </summary>
    public partial class SplashForm : Form
    {
        #region Win32 API 声明（用于点击穿透）

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;

        #endregion

        // 进度值（0-100）
        private int _progressValue = 0;

        // 当前阶段文本
        private string _stageText = "开机检查...";
        // 进度条文本字体
        private readonly Font _textFont = new Font("Segoe UI", 10, FontStyle.Regular);
        // 进度条文本画刷（建议用白色，提升对比度）
        private readonly Brush _textBrush = new SolidBrush(Color.White);
        // 进度条背景画刷
        private readonly Brush _progressBgBrush = new SolidBrush(Color.FromArgb(100, 50, 50, 50));
        // 进度条填充画刷
        private readonly Brush _progressFillBrush = new SolidBrush(Color.FromArgb(255, 0, 120, 215));
        // 进度条边框画笔
        private readonly Pen _progressBorderPen = new Pen(Color.FromArgb(200, 200, 200), 1);

        // 进度条宽度
        private const int ProgressBarWidth = 300;
        // 进度条高度
        private const int ProgressBarHeight = 20;
        // 文本高度
        private const int TextHeight = 30;
        // 内边距
        private const int ProgressPadding = 20;

        /// <summary>
        /// 构造函数，初始化窗体
        /// </summary>
        public SplashForm()
        {
            InitializeComponent();
            SetupTransparentForm();
        }

        /// <summary>
        /// 设置窗体为无边框、居中、透明色、双缓冲等
        /// </summary>
        private void SetupTransparentForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(ProgressBarWidth + ProgressPadding * 2,
                                      ProgressBarHeight + TextHeight + ProgressPadding * 3);
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            MakeClickThrough();
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// 使窗体支持鼠标穿透
        /// </summary>
        private void MakeClickThrough()
        {
            int initialStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, initialStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }

        /// <summary>
        /// 更新进度条和阶段文本
        /// </summary>
        /// <param name="value">进度值（0-100）</param>
        /// <param name="stageText">当前阶段描述</param>
        public void UpdateProgress(int value, string stageText)
        {
            _progressValue = Math.Max(0, Math.Min(100, value));
            _stageText = stageText ?? "";
            this.Invalidate();
        }

        /// <summary>
        /// 绘制进度条和文本
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int progressX = ProgressPadding;
            int progressY = ProgressPadding * 2;
            int textY = progressY + ProgressBarHeight + 5;

            Rectangle progressRect = new Rectangle(progressX, progressY, ProgressBarWidth, ProgressBarHeight);
            g.FillRectangle(_progressBgBrush, progressRect);
            g.DrawRectangle(_progressBorderPen, progressRect);

            int fillWidth = (int)(ProgressBarWidth * (_progressValue / 100.0));
            if (fillWidth > 0)
            {
                Rectangle fillRect = new Rectangle(progressX, progressY, fillWidth, ProgressBarHeight);
                g.FillRectangle(_progressFillBrush, fillRect);
            }

            // 阶段文本+百分比
            string stageText = $"{_stageText} {_progressValue}%";
            SizeF stageSize = g.MeasureString(stageText, _textFont);
            float stageX = progressX + (ProgressBarWidth - stageSize.Width) / 2;

            // 文字底板
            RectangleF stageBgRect = new RectangleF(
                stageX - 6, textY - 2, stageSize.Width + 12, stageSize.Height + 4);
            using (Brush stageBgBrush = new SolidBrush(Color.FromArgb(180, 30, 30, 30)))
            {
                g.FillRectangle(stageBgBrush, stageBgRect);
            }
            g.DrawString(stageText, _textFont, _textBrush, stageX, textY);

        }

        /// <summary>
        /// 绘制窗体背景（透明色）
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textFont?.Dispose();
                _textBrush?.Dispose();
                _progressBgBrush?.Dispose();
                _progressFillBrush?.Dispose();
                _progressBorderPen?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 窗体显示后自动执行加载并关闭自身
        /// </summary>
        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await PerformLoadingAsync();
            this.Close();
        }


        /// <summary>
        /// 异步加载过程，逐步更新进度条和阶段文本
        /// </summary>
        private async Task PerformLoadingAsync()
        {
            try
            {
                UpdateProgress(0, "检查版本号...");
                await VersionCheck();

                UpdateProgress(20, "检查基础配置参数...");
                await ConparCheck(true);

                UpdateProgress(30, "检查数据库连接...");
                bool dbOk = await Task.Run(My_DataBase.SqlServer.TestConnection);
                if (!dbOk)
                {
                    My_File.LocalTranslator.ShowMessage("数据库连接失败，程序即将退出！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                    return;
                }

                UpdateProgress(40, "检查数据库结构...");
                try
                {
                    await Task.Run(My_DataBase.SqlServer.SyncAllTableStructure);
                }
                catch (Exception ex)
                {
                    My_File.LocalTranslator.ShowMessage("数据库结构检查或修复失败，程序即将退出！\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                    return;
                }

                UpdateProgress(60, "检查其他配置参数...");
                await ConparCheck(false);

                UpdateProgress(80, "获取数据库数据...");
                try
                {
                    await Task.Run(GetData);
                }
                catch (Exception ex)
                {
                    My_File.LocalTranslator.ShowMessage("获取数据库数据失败，程序即将退出！\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                    return;
                }


                if (My_ConPar.Machine.MachineType == 0)
                {
                    UpdateProgress(90, "检查PLC链接...");
                    bool plcOk = await Task.Run(PlcCheck);
                    if (!plcOk)
                    {
                        Environment.Exit(1);
                        return;
                    }

                }
                else if (My_ConPar.Machine.MachineType == 1)
                {
                    UpdateProgress(90, "检查运动控制卡链接...");
                    bool cadOk = await Task.Run(CardCheck);
                    if (!cadOk)
                    {
                        Environment.Exit(1);
                        return;
                    }
                }

                UpdateProgress(100, "启动完成");
                //await Task.Delay(300);
            }
            catch (Exception ex)
            {
                My_File.LocalTranslator.ShowMessage("加载异常: " + ex.Message, "PerformLoadingAsync", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Task GetData()
        {
            My_DataBase.BrewData.GetData();
            My_DataBase.BottleData.GetData();
            My_DataBase.AssistantData.GetData();
            My_DataBase.DyeingData.GetData();
            My_DataBase.DyeingCodeData.GetData();
            My_DataBase.LimitData.GetData();
            My_DataBase.FormulaGradeData.GetData();
            My_DataBase.UserData.GetData();
            My_DataBase.EnabledData.GetData();
            My_DataBase.CupData.GetData();


            
            My_DataBase.DropWaitData.GetData();

            if (My_ConPar.Machine.UseAbs == 1)
            {
               
                My_DataBase.ABSProcess.GetData();
                My_DataBase.ABSWaitData.GetData();
                My_DataBase.ABSEnabledData.GetData();

            }



            return Task.CompletedTask;
        }

        private Task VersionCheck()
        {
            //比较版本号
            var path = Path.Combine(Environment.CurrentDirectory, "Config", "AbortInfo.ini");
            My_File.ConfigHelper.CheckAndAssignOrPrompt(
              this, path, typeof(My_ConPar.AbortInfo),
              () => new My_Form.ConPar.AbortInfo());
            if (My_ConPar.AbortInfo.LastVersion != Application.ProductVersion)
            {
                new My_Form.ConPar.AbortInfo().ShowDialog();
            }
            return Task.CompletedTask;
        }

        private Task ConparCheck(bool first)
        {
            if (first)
                // 1. 加载机台参数
                LoadMachineConfig();

            else
            {

                // 2. 加载布局参数
                var layoutType = GetLayoutType();
                if (layoutType == null)
                    return Task.CompletedTask;
                My_ConPar.Object.CurrentLayout = layoutType;
                My_File.ConfigHelper.LoadLayoutConfig(layoutType);

                // 3. 加载区域参数
                LoadAreaConfig(this, layoutType);

                // 4. 继续加载其他基础参数
                LoadBasicConfigs();

                // 5. 加载运动参数

                LoadMotionConfig();



            }


            return Task.CompletedTask;
        }

        /// <summary>加载机台参数</summary>
        private void LoadMachineConfig()
        {
            var machinePath = Path.Combine(Environment.CurrentDirectory, "Config", "Machine.ini");
            My_File.ConfigHelper.CheckAndAssignOrPrompt(
                this, machinePath, typeof(My_ConPar.Machine),
                () => new My_Form.ConPar.ConParShow("机台参数", "Machine.ini", typeof(My_ConPar.Machine)));

            var path = Path.Combine(Environment.CurrentDirectory, "Config", "Database.ini");
            My_File.ConfigHelper.CheckAndAssignOrPrompt(
             this, path, typeof(My_ConPar.Database),
             () => new My_Form.ConPar.ConParShow("数据库参数", "Database.ini", typeof(My_ConPar.Database)));


            //加载IO参数
            LoadIO();

            //加载开料机参数
            LoadCuttingMachineConfig();

            //加载称布机参数
            LoadWeighingMachineConfig();

            //加载称粉机参数
            LoadPowderMachineConfig();

            //加载ERP交互参数
            LoadERPConfig();

        }

        /// <summary>
        /// ERP交互参数
        /// </summary>
        private void LoadERPConfig()
        {
            var erpPath = Path.Combine(Environment.CurrentDirectory, "Config", "ERP.ini");

            switch (My_ConPar.Machine.ERPInteraction)
            {
                case 0:
                    return;
                case 1:
                    {
                        var erp = new My_ConPar.ERPInteraction.Txt();
                        SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                             this, erpPath, typeof(My_ConPar.ERPInteraction.Txt),
                             () => new My_Form.ConPar.ConParShow("ERP交互参数", "ERP.ini", typeof(My_ConPar.ERPInteraction.Txt), erp), erp);
                        // 可选：赋值到全局变量
                        My_ConPar.Object.CurrentERP = erp;
                        break;
                    }

                case 2:
                    {
                        var erp = new My_ConPar.ERPInteraction.Database();
                        SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                             this, erpPath, typeof(My_ConPar.ERPInteraction.Database),
                             () => new My_Form.ConPar.ConParShow("ERP交互参数", "ERP.ini", typeof(My_ConPar.ERPInteraction.Database), erp), erp);
                        // 可选：赋值到全局变量
                        My_ConPar.Object.CurrentERP = erp;
                        break;
                    }
                default:
                    My_File.LocalTranslator.ShowMessage("ERP交互参数配置错误，程序即将退出！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                    return;

            }

        }

        /// <summary>
        /// 加载运动参数
        /// </summary>
        private void LoadMotionConfig()
        {
            // 0: PLC, 1: 板卡, 2: 脱机
            if (My_ConPar.Machine.MachineType == 0)
            {
                // PLC运动参数
                var plcMotionPath = Path.Combine(Environment.CurrentDirectory, "Config", "PLC_Motion.ini");
                var plcMotion = new SmartColor.My_ConPar.Type.PLC.Motion();
                SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                    this, plcMotionPath, typeof(SmartColor.My_ConPar.Type.PLC.Motion),
                    () => new My_Form.ConPar.ConParShow("PLC运动参数", "PLC_Motion.ini", typeof(SmartColor.My_ConPar.Type.PLC.Motion), plcMotion), plcMotion);
                My_ConPar.Object.CurrentMotion = plcMotion;
            }
            else if (My_ConPar.Machine.MachineType == 1)
            {
                // 板卡运动参数
                var cardMotionPath = Path.Combine(Environment.CurrentDirectory, "Config", "BoardCard_Motion.ini");
                var cardMotion = new SmartColor.My_ConPar.Type.BoaedCard.Motion();
                SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                    this, cardMotionPath, typeof(SmartColor.My_ConPar.Type.BoaedCard.Motion),
                    () => new My_Form.ConPar.ConParShow("板卡运动参数", "BoardCard_Motion.ini", typeof(SmartColor.My_ConPar.Type.BoaedCard.Motion), cardMotion), cardMotion);
                My_ConPar.Object.CurrentMotion = cardMotion;
            }
            else
            {
                // 脱机模式不加载
                My_ConPar.Object.CurrentMotion = null;
            }
        }

        /// <summary>
        /// 称布机参数
        /// </summary>
        private void LoadWeighingMachineConfig()
        {
            switch (My_ConPar.Machine.UseCloth)
            {
                case 0:
                    return;
                case 1:
                    {
                        var machinePath = Path.Combine(Environment.CurrentDirectory, "Config", "Weighing.ini");
                        My_File.ConfigHelper.CheckAndAssignOrPrompt(
                            this, machinePath, typeof(My_ConPar.WeighingMachine.Weighing),
                            () => new My_Form.ConPar.ConParShow("称布机参数", "Weighing.ini", typeof(My_ConPar.WeighingMachine.Weighing)));

                        break;
                    }
                default:
                    My_File.LocalTranslator.ShowMessage("称布机配置错误，程序即将退出！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                    return;

            }

        }

        /// <summary>
        /// 称粉机参数
        /// </summary>
        private void LoadPowderMachineConfig()
        {
            switch (My_ConPar.Machine.UsePowder)
            {
                case 0:
                    return;
                case 1:
                case 2:
                    {
                        var machinePath = Path.Combine(Environment.CurrentDirectory, "Config", "Powder.ini");
                        My_File.ConfigHelper.CheckAndAssignOrPrompt(
                            this, machinePath, typeof(My_ConPar.PowderMachine.Powder),
                            () => new My_Form.ConPar.ConParShow("称粉机参数", "Powder.ini", typeof(My_ConPar.PowderMachine.Powder)));

                        break;
                    }
                default:
                    My_File.LocalTranslator.ShowMessage("称粉机配置错误，程序即将退出！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                    return;

            }
        }

        /// <summary>
        /// 加载IO参数
        /// </summary>
        private void LoadIO()
        {

            //机台类型
            if (My_ConPar.Machine.MachineType < 0 || My_ConPar.Machine.MachineType > 2)
            {
                My_File.LocalTranslator.ShowMessage("机台类型配置错误，程序即将退出！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
                return;
            }
            // PLC 机台
            if (My_ConPar.Machine.MachineType == 0)
            {
                var plcIOPath = Path.Combine(Environment.CurrentDirectory, "Config", "PLC_IO.ini");
                // 实例化 PLC IO
                var plcIO = new SmartColor.My_ConPar.Type.PLC.IO();
                SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                    this, plcIOPath, typeof(SmartColor.My_ConPar.Type.PLC.IO),
                    () => new My_Form.ConPar.ConParShow("PLC IO参数", "PLC_IO.ini", typeof(SmartColor.My_ConPar.Type.PLC.IO), plcIO), plcIO);
                // 可选：赋值到全局变量
                My_ConPar.Object.CurrentMachine = plcIO;
            }
            // 板卡机台
            else if (My_ConPar.Machine.MachineType == 1)
            {
                var boardIOPath = Path.Combine(Environment.CurrentDirectory, "Config", "BoardCard_IO.ini");
                // 实例化板卡 IO
                var boardIO = new SmartColor.My_ConPar.Type.BoaedCard.IO();
                SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                    this, boardIOPath, typeof(SmartColor.My_ConPar.Type.BoaedCard.IO),
                    () => new My_Form.ConPar.ConParShow("板卡 IO参数", "BoardCard_IO.ini", typeof(SmartColor.My_ConPar.Type.BoaedCard.IO), boardIO), boardIO);
                // 可选：赋值到全局变量
                My_ConPar.Object.CurrentMachine = boardIO;
            }
        }

        /// <summary>
        /// 加载开料机参数
        /// </summary>
        private void LoadCuttingMachineConfig()
        {

            var cuttingPath = Path.Combine(Environment.CurrentDirectory, "Config", "CuttingMachine.ini");
            switch (My_ConPar.Machine.CuttingMachine)
            {
                case 0:
                    return;
                case 1:
                    {
                        var cutting = new My_ConPar.CuttingMachine.WEINVIEW_TCP();
                        SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                             this, cuttingPath, typeof(SmartColor.My_ConPar.CuttingMachine.WEINVIEW_TCP),
                             () => new My_Form.ConPar.ConParShow("开料机参数", "CuttingMachine.ini", typeof(SmartColor.My_ConPar.CuttingMachine.WEINVIEW_TCP), cutting), cutting);
                        // 可选：赋值到全局变量
                        My_ConPar.Object.CurrentCutting = cutting;
                        break;
                    }


                case 2:
                    {
                        var cutting = new My_ConPar.CuttingMachine.Inovance_TCP();
                        SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                             this, cuttingPath, typeof(SmartColor.My_ConPar.CuttingMachine.Inovance_TCP),
                             () => new My_Form.ConPar.ConParShow("开料机参数", "CuttingMachine.ini", typeof(SmartColor.My_ConPar.CuttingMachine.Inovance_TCP), cutting), cutting);
                        // 可选：赋值到全局变量
                        My_ConPar.Object.CurrentCutting = cutting;
                        break;
                    }


                case 3:
                    {
                        var cutting = new My_ConPar.CuttingMachine.WEINVIEW_RTU();
                        SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                             this, cuttingPath, typeof(SmartColor.My_ConPar.CuttingMachine.WEINVIEW_RTU),
                             () => new My_Form.ConPar.ConParShow("开料机参数", "CuttingMachine.ini", typeof(SmartColor.My_ConPar.CuttingMachine.WEINVIEW_RTU), cutting), cutting);
                        // 可选：赋值到全局变量
                        My_ConPar.Object.CurrentCutting = cutting;
                        break;
                    }


                case 4:
                    {
                        var cutting = new My_ConPar.CuttingMachine.DELTA_RTU();
                        SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                             this, cuttingPath, typeof(SmartColor.My_ConPar.CuttingMachine.DELTA_RTU),
                             () => new My_Form.ConPar.ConParShow("开料机参数", "CuttingMachine.ini", typeof(SmartColor.My_ConPar.CuttingMachine.DELTA_RTU), cutting), cutting);
                        // 可选：赋值到全局变量
                        My_ConPar.Object.CurrentCutting = cutting;
                        break;
                    }
                case 5:
                    //预留自动开料的
                    break;

                default:
                    My_File.LocalTranslator.ShowMessage("开料机类型配置错误，程序即将退出！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                    return;

            }
        }


        /// <summary>获取布局类型</summary>
        public static Type GetLayoutType()
        {
            switch (My_ConPar.Machine.MachineLayout)
            {
                case 0: return typeof(Layout_1);
                case 1: return typeof(Layout_2);
                case 2: return typeof(Layout_3);
                case 3: return typeof(Layout_4);
                default: return null;
            }
        }

        /// <summary>加载区域参数（优化：只在有变更时统一保存一次）</summary>
        public static void LoadAreaConfig(IWin32Window owner, Type layoutType)
        {
            var areaProps = layoutType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var layoutFile = Path.Combine(Environment.CurrentDirectory, "Config", "Layout.ini");
            string[] lines = File.Exists(layoutFile) ? File.ReadAllLines(layoutFile) : new string[0];

            var areaDictMap = new Dictionary<int, Dictionary<string, string>>();
            int currentAreaNum = 0;
            foreach (var line in lines)
            {
                if (line.StartsWith("[Area"))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(line, @"\[Area(\d+)\]");
                    if (match.Success)
                    {
                        currentAreaNum = int.Parse(match.Groups[1].Value);
                        areaDictMap[currentAreaNum] = new Dictionary<string, string>();
                    }
                    continue;
                }
                if (areaDictMap.ContainsKey(currentAreaNum) && line.Contains("="))
                {
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length == 2)
                        areaDictMap[currentAreaNum][parts[0].Trim()] = parts[1].Trim();
                }
            }

            bool needSave = false; // 标记是否需要保存

            foreach (var prop in areaProps)
            {
                var name = prop.Name;
                if (!name.StartsWith("Area_")) continue;
                if (!int.TryParse(name.Substring(5), out int areaNum)) continue;

                Dictionary<string, string> dict = areaDictMap.ContainsKey(areaNum) ? areaDictMap[areaNum] : null;
                SmartColor.My_ConPar.Area.Base areaObj = prop.GetValue(null) as SmartColor.My_ConPar.Area.Base;

                if (dict == null)
                {
                    var layoutDefault = new SmartColor.My_ConPar.Area.Base();
                    if (prop.GetValue(null) is SmartColor.My_ConPar.Area.Base layoutSource)
                    {
                        layoutDefault.AreaType = layoutSource.AreaType;
                        layoutDefault.AreaName = layoutSource.AreaName;
                    }
                    using (var dlg = new SmartColor.My_Form.ConPar.ConParShow($"{layoutDefault.AreaName}类型选择", null, typeof(SmartColor.My_ConPar.Area.Base), layoutDefault))
                    {
                        if (dlg.ShowDialog(owner) != DialogResult.OK)
                            continue;
                    }
                    int selectedType = layoutDefault.AreaType;
                    string selectedName = layoutDefault.AreaName;

                    areaObj = SmartColor.My_File.ConfigHelper.CreateAreaByType(selectedType);
                    areaObj.AreaType = selectedType;
                    areaObj.AreaName = selectedName;
                    if (selectedType == 0)
                    {
                        // 用户选择不配置该区域，跳过
                        prop.SetValue(null, areaObj);
                        continue;
                    }
                    if (areaObj is SmartColor.My_ConPar.Area.BottleArea.Bottle bottleArea)
                    {
                        using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("母液瓶参数补全", null, bottleArea.GetType(), bottleArea))
                        {
                            if (dlg.ShowDialog(owner) != DialogResult.OK)
                                continue;
                        }
                        if (bottleArea.BottleNum > 0 && bottleArea.BottleColumn > 0 && bottleArea.BottleNum % bottleArea.BottleColumn != 0)
                        {
                            if (bottleArea.EmbeddedBalance == null)
                                bottleArea.EmbeddedBalance = new SmartColor.My_ConPar.Area.Balance.Balance();
                            using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("母液瓶区天平参数补全", null, bottleArea.EmbeddedBalance.GetType(), bottleArea.EmbeddedBalance))
                            {
                                if (dlg.ShowDialog(owner) != DialogResult.OK)
                                    continue;
                            }
                            // 赋值全局天平参数
                            SmartColor.My_ConPar.Object.CurrentBalance = bottleArea.EmbeddedBalance;
                        }

                    }
                    else
                    {
                        using (var dlg = new SmartColor.My_Form.ConPar.ConParShow($"{areaObj.AreaName}参数补全", null, areaObj.GetType(), areaObj))
                        {
                            if (dlg.ShowDialog(owner) != DialogResult.OK)
                                continue;
                        }

                        if (areaObj.AreaType == 1)
                        {
                            var ba = areaObj as My_ConPar.Area.Balance.Balance;
                            SmartColor.My_ConPar.Object.CurrentBalance = ba;
                        }
                    }

                    prop.SetValue(null, areaObj);

                    needSave = true; // 有变更，标记需要保存
                    continue;
                }

                // 区块存在，检查参数补全
                if (areaObj is SmartColor.My_ConPar.Area.BottleArea.Bottle bottleArea2)
                {
                    var bottleProps = bottleArea2.GetType().GetProperties();
                    bool bottleMissing = false;
                    foreach (var p in bottleProps)
                    {
                        if (!p.CanWrite || p.Name == "EmbeddedBalance") continue;
                        if (!dict.ContainsKey(p.Name))
                        {
                            bottleMissing = true;
                            break;
                        }
                    }
                    if (bottleMissing)
                    {
                        using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("母液瓶参数补全", null, bottleArea2.GetType(), bottleArea2))
                        {
                            if (dlg.ShowDialog(owner) != DialogResult.OK)
                                continue;
                        }
                        needSave = true; // 有变更，标记需要保存
                    }
                    if (bottleArea2.BottleNum > 0 && bottleArea2.BottleColumn > 0 && bottleArea2.BottleNum % bottleArea2.BottleColumn != 0)
                    {
                        if (bottleArea2.EmbeddedBalance == null)
                            bottleArea2.EmbeddedBalance = new SmartColor.My_ConPar.Area.Balance.Balance();
                        var balanceProps = bottleArea2.EmbeddedBalance.GetType().GetProperties();
                        bool balanceMissing = false;
                        foreach (var p in balanceProps)
                        {
                            if (!p.CanWrite) continue;
                            if (!dict.ContainsKey(p.Name))
                            {
                                balanceMissing = true;
                                break;
                            }
                        }
                        if (balanceMissing)
                        {
                            using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("母液瓶区天平参数补全", null, bottleArea2.EmbeddedBalance.GetType(), bottleArea2.EmbeddedBalance))
                            {
                                if (dlg.ShowDialog(owner) != DialogResult.OK)
                                    continue;
                            }
                            needSave = true; // 有变更，标记需要保存
                        }
                        bottleArea2.LoadBalanceFromDict(dict);
                        // 赋值全局天平参数
                        SmartColor.My_ConPar.Object.CurrentBalance = bottleArea2.EmbeddedBalance;
                    }

                }
                else
                {
                    var props = areaObj.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    bool hasMissingField = false;
                    foreach (var p in props)
                    {
                        if (!p.CanWrite) continue;
                        if (!dict.ContainsKey(p.Name))
                        {
                            hasMissingField = true;
                            break;
                        }
                    }
                    if (areaObj.AreaType == 1)
                    {
                        var ba = areaObj as My_ConPar.Area.Balance.Balance;
                        SmartColor.My_ConPar.Object.CurrentBalance = ba;
                    }
                    if (hasMissingField)
                    {
                        using (var dlg = new SmartColor.My_Form.ConPar.ConParShow($"{areaObj.AreaName}参数补全", null, areaObj.GetType(), areaObj))
                        {
                            if (dlg.ShowDialog(owner) != DialogResult.OK)
                                continue;
                        }
                        needSave = true; // 有变更，标记需要保存
                    }
                }
            }

            // 统一保存
            if (needSave)
                SmartColor.My_File.ConfigHelper.SaveLayoutConfig(layoutType);

            //同步数据库cup_details表中的杯号，使其与布局参数一致（插入缺失、删除多余）。
            SyncCupDetailsWithLayout(layoutType);


        }

        /// <summary>
        /// 同步数据库cup_details表中的杯号，使其与布局参数一致（插入缺失、删除多余）。
        /// </summary>
        /// <param name="layoutType">布局类型（含静态属性）</param>
        public static void SyncCupDetailsWithLayout(Type layoutType)
        {
            var areaProps = layoutType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var expectedCupNums = new HashSet<int>();

            foreach (var prop in areaProps)
            {
                if (!prop.Name.StartsWith("Area_")) continue;
                var areaObj = prop.GetValue(null) as SmartColor.My_ConPar.Area.Base;
                if (areaObj == null) continue;

                int[] cupAreaTypes = { 2, 3, 4, 5, 6, 7, 8 };
                if (cupAreaTypes.Contains(areaObj.AreaType))
                {
                    int start = 1, row = 1, col = 1;
                    var t = areaObj.GetType();
                    if (t.GetProperty("StartPosition") != null)
                        start = (int)t.GetProperty("StartPosition").GetValue(areaObj);
                    if (t.GetProperty("Row") != null)
                        row = (int)t.GetProperty("Row").GetValue(areaObj);
                    if (t.GetProperty("Column") != null)
                        col = (int)t.GetProperty("Column").GetValue(areaObj);
                    if (start == 1 && row == 1 && col == 1) continue;
                    int total = row * col;

                    bool isMain = start % 2 == 1;

                    for (int i = 0; i < total; i++)
                    {
                        int cupNum = start + i;
                        expectedCupNums.Add(cupNum);
                    }
                }
            }

            // 查询数据库现有的杯号
            var dt = SmartColor.My_DataBase.SqlServer.Select(SmartColor.My_DataBase.CUP_DETAILS.TableName);
            var dbCupNums = new HashSet<int>();
            foreach (DataRow row in dt.Rows)
            {
                if (int.TryParse(row[SmartColor.My_DataBase.CUP_DETAILS.CupNum]?.ToString(), out int n))
                    dbCupNums.Add(n);
            }

            // 需要插入的杯号
            var toAdd = expectedCupNums.Except(dbCupNums).ToList();
            // 需要删除的杯号
            var toRemove = dbCupNums.Except(expectedCupNums).ToList();

            // 插入缺失的杯号（参考SaveLayoutConfig的插入逻辑）
            foreach (var prop in areaProps)
            {
                if (!prop.Name.StartsWith("Area_")) continue;
                var areaObj = prop.GetValue(null) as SmartColor.My_ConPar.Area.Base;
                if (areaObj == null) continue;

                int[] cupAreaTypes = { 2, 3, 4, 5, 6, 7, 8 };
                if (!cupAreaTypes.Contains(areaObj.AreaType)) continue;

                int start = 1, row = 1, col = 1;
                var t = areaObj.GetType();
                if (t.GetProperty("StartPosition") != null)
                    start = (int)t.GetProperty("StartPosition").GetValue(areaObj);
                if (t.GetProperty("Row") != null)
                    row = (int)t.GetProperty("Row").GetValue(areaObj);
                if (t.GetProperty("Column") != null)
                    col = (int)t.GetProperty("Column").GetValue(areaObj);
                if (start == 1 && row == 1 && col == 1) continue;
                int total = row * col;
                int mainCupNum = 0;
                bool isMain = start % 2 == 1;

                for (int i = 0; i < total; i++)
                {
                    int cupNum = start + i;
                    if (!toAdd.Contains(cupNum)) continue;

                    int type = 0;
                    if (areaObj.AreaType == 6)
                        type = 4;
                    else if (areaObj.AreaType == 8)
                        type = 2;
                    else
                        type = 3;

                    if (areaObj.AreaType == 4 || areaObj.AreaType == 5)
                    {
                        if (isMain)
                            mainCupNum = cupNum % 2 == 1 ? cupNum + 1 : cupNum - 1;
                        else
                            mainCupNum = cupNum % 2 == 1 ? cupNum - 1 : cupNum + 1;
                    }
                    else
                    {
                        mainCupNum = cupNum;
                    }

                    var dict = new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.CUP_DETAILS.CupNum] = cupNum,
                        [SmartColor.My_DataBase.CUP_DETAILS.MainCupNum] = mainCupNum,
                        [SmartColor.My_DataBase.CUP_DETAILS.Type] = type,
                        [SmartColor.My_DataBase.CUP_DETAILS.Statues] = areaObj.AreaType == 8 ? null : "下线",
                        [SmartColor.My_DataBase.CUP_DETAILS.Enable] = areaObj.AreaType == 8 ? 1 : 0,
                        [SmartColor.My_DataBase.CUP_DETAILS.TotalWeight] = 0,
                        [SmartColor.My_DataBase.CUP_DETAILS.CurrentWeight] = 0
                    };
                    SmartColor.My_DataBase.SqlServer.Insert(SmartColor.My_DataBase.CUP_DETAILS.TableName, dict);
                }
            }

            // 删除多余的杯号
            foreach (var cupNum in toRemove)
            {
                SmartColor.My_DataBase.SqlServer.Delete(
                    SmartColor.My_DataBase.CUP_DETAILS.TableName,
                    $"{SmartColor.My_DataBase.CUP_DETAILS.CupNum}=@CupNum",
                    new System.Data.SqlClient.SqlParameter("@CupNum", cupNum));
            }
        }

        /// <summary>加载基础参数</summary>
        private void LoadBasicConfigs()
        {
            var configList = new[]
            {
                 new { Name = "选项", File = "Choices.ini", Type = typeof(My_ConPar.Choices) },
                 new { Name = "硬件", File = "Hardware.ini", Type = typeof(My_ConPar.Hardware) },
                 new { Name = "延时", File = "Delay.ini", Type = typeof(My_ConPar.Delay) },
                 new { Name = "校正", File = "Correction.ini", Type = typeof(My_ConPar.Correction) },
                 new { Name = "洗杯", File = "WashCup.ini", Type = typeof(My_ConPar.WashCup) },
                 new { Name = "其他", File = "Other.ini", Type = typeof(My_ConPar.Other) }
            };
            foreach (var cfg in configList)
            {
                var path = Path.Combine(Environment.CurrentDirectory, "Config", cfg.File);
                My_File.ConfigHelper.CheckAndAssignOrPrompt(
                    this, path, cfg.Type,
                    () => new My_Form.ConPar.ConParShow(cfg.Name, cfg.File, cfg.Type));

                if (My_ConPar.Machine.MachineType == 2)
                    break;
            }

            //检查优先级工艺参数补全

            foreach (var type in My_Form.ConPar.OrderInfo.PriorityTypes)
            {
                if (My_ConPar.Machine.MachineLayout == 0 && (
                    type.Type == typeof(SmartColor.My_ConPar.Order.DyeingProcess.DyeingProcess) ||
                    type.Type == typeof(SmartColor.My_ConPar.Order.PostProcess.PostProcess)))
                {
                    continue;
                }

                if (My_ConPar.Machine.UseAbs == 0 &&
                    type.Type == typeof(SmartColor.My_ConPar.Order.UVProcess.UVProcess))
                {
                    continue;
                }

                if (My_ConPar.Machine.CuttingMachine != 5 &&
                    type.Type == typeof(SmartColor.My_ConPar.Order.BrewProcess.BrewProcess))
                {
                    continue;
                }


                bool ok = LoadOrderSectionToStaticVars(type.Type);
                if (!ok)
                {
                    // 只要有缺失就弹窗补全
                    using (var dlg = new SmartColor.My_Form.ConPar.OrderInfo(type.Name, type.Type))
                    {
                        dlg.ShowDialog(this);
                    }
                }
            }
        }


        private bool LoadOrderSectionToStaticVars(Type type)
        {
            string orderPath = My_Form.ConPar.OrderInfo.OrderConfigPath;
            if (!File.Exists(orderPath))
                return false;

            var lines = File.ReadAllLines(orderPath);
            string sectionName = $"[{type.Name}]";
            bool inSection = false;
            var dict = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                var l = line.Trim();
                if (l.StartsWith("[") && l.EndsWith("]"))
                {
                    inSection = l.Equals(sectionName, StringComparison.OrdinalIgnoreCase);
                    continue;
                }
                if (inSection)
                {
                    if (l.StartsWith("[") && l.EndsWith("]")) break; // 下一个段落
                    if (l.Contains("="))
                    {
                        var parts = l.Split(new[] { '=' }, 2);
                        if (parts.Length == 2)
                            dict[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }
            bool allAssigned = true;
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                // 获取 DescriptionAttribute 作为显示名
                var descAttr = prop.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
                string displayName = descAttr != null ? descAttr.Description : prop.Name;
                if (!IsModuleEnabled(type, displayName))
                    continue;

                if (dict.TryGetValue(prop.Name, out string valueStr))
                {
                    try
                    {
                        object value = Convert.ChangeType(valueStr, prop.PropertyType);
                        prop.SetValue(null, value);
                    }
                    catch
                    {
                        allAssigned = false;
                    }
                }
                else
                {
                    allAssigned = false;
                }
            }
            return allAssigned;
        }


        private bool IsModuleEnabled(Type type, string displayName)
        {
            if (type == typeof(My_ConPar.Order.BigProcess))
            {
                if (My_ConPar.Machine.MachineLayout == 0 && (displayName == "染色进程" || displayName == "后处理进程"))
                    return false;
                if (My_ConPar.Machine.UseAbs == 0 && displayName == "吸光度进程")
                    return false;
                if (My_ConPar.Machine.CuttingMachine != 5 && displayName == "自动开料进程")
                    return false;
            }
            return true;
        }


        private bool CardCheck()
        {
            //板卡自检
            try
            {
                My_ADT8940A1.Card.CurrentBoardCard = new My_ADT8940A1.Card();
                return true;
            }
            catch (Exception ex)
            {
                My_File.LocalTranslator.ShowMessage("运动控制卡初始化失败，程序即将退出！\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool PlcCheck()
        {
            //PLC自检
            try
            {
                My_ConPar.Object.CurrentPLC = new My_PLC.PLC();
                return true;
            }
            catch (Exception ex)
            {
                My_File.LocalTranslator.ShowMessage("PLC初始化失败，程序即将退出！\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}