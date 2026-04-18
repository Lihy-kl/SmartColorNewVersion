using Microsoft.VisualBasic.ApplicationServices;
using SmartColor.My_Control;
using SmartColor.My_DataBase;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Form.Homepage
{
    /// <summary>
    /// 配液杯信息窗体，负责显示指定杯号的配方详情和染固色工艺信息。
    /// 支持实际温度曲线、理论曲线、共同显示，右键切换，自动刷新。
    /// </summary>
    public partial class CupInfo : Form
    {
        /// <summary>
        /// 当前选中的杯子
        /// </summary>
        private readonly CtCup _ctCup;

        // 用于定时刷新用时
        private DateTime? _startTime;
        private DateTime? _stepStartTime;
        private TimeSpan _usedTime = TimeSpan.Zero;
        private List<ProcessStep> _steps = new List<ProcessStep>();
        private bool _isWashCup = false;
        private string _dyeingCode = "";
        private int _rd = 0;

        // 当前温度曲线显示模式（0=实际，1=理论，2=共同）
        private int _chartShowMode = 0;

        public CupInfo(CtCup ctCup)
        {
            InitializeComponent();
            this._ctCup = ctCup;
            this.Load += CupInfo_Load;

            // 绑定定时器事件
            this.timer1.Tick += Timer1_Tick;

            // 绑定杯子步号变化事件，异步处理，避免阻塞UI线程
            this._ctCup.CurrentStepNoChanged += CtCup_CurrentStepNoChanged;

            _rd = My_Tool.BalanceStableReading.RetainDecimals();

            // 右键菜单与显示模式事件绑定
            this.ctTemperatureChart1.ContextMenuStrip = this.ctTemperatureChart1.contextMenuStrip1;
            this.ctTemperatureChart1.ShowModeChanged += CtTemperatureChart1_ShowModeChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 解绑事件，防止内存泄漏
                if (_ctCup != null)
                    _ctCup.CurrentStepNoChanged -= CtCup_CurrentStepNoChanged;
                if (timer1 != null)
                    timer1.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }


        // 把事件处理器提到成员方法，便于解绑
        private async void CtCup_CurrentStepNoChanged(object s, EventArgs e)
        {
            await Task.Delay(500);
            if (this.IsHandleCreated)
                await LoadCupInfoAsync();
        }

        /// <summary>
        /// 定时刷新界面，包括温度曲线
        /// </summary>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            // 只做UI赋值，数据已在后台线程处理
            textBox6.Text = Math.Round(_ctCup.ActualTemp / 10, 1).ToString();
            textBox7.Text = Math.Round(Convert.ToDouble(_ctCup.Value), _rd).ToString();
            textBox2.Text = _ctCup.CurrentStepNo.ToString();
            textBox10.Text = _ctCup.TechnologyName;
            textBox9.Text = _ctCup.HoldingTime.ToString();

            // 总用时
            if (_startTime.HasValue)
            {
                var used = DateTime.Now - _startTime.Value;
                txt_usedTime.Text = used.ToString(@"hh\:mm\:ss");
            }
            // 步骤用时
            if (_stepStartTime.HasValue)
            {
                var stepUsed = DateTime.Now - _stepStartTime.Value;
                // 可根据需要显示到其它控件
            }

            // 定时刷新温度曲线
            RefreshTemperatureChart();
        }

        /// <summary>
        /// 右键菜单切换显示模式事件
        /// </summary>
        private void CtTemperatureChart1_ShowModeChanged(object sender, int mode)
        {
            _chartShowMode = mode;
            RefreshTemperatureChart();
        }

        /// <summary>
        /// 刷新温度曲线（实际/理论/共同），带工艺标签，自动适配模式
        /// </summary>
        private void RefreshTemperatureChart()
        {
            // 1. 读取实际温度数据
            string tempFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "APP_DATA", $"{_ctCup.NO}.txt");
            string[] actualTemps = null;
            DateTime? recordStartTime = null;
            if (System.IO.File.Exists(tempFile))
            {
                using (var fs = new FileStream(tempFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fs, Encoding.UTF8))
                {
                    string tempStr = reader.ReadToEnd();
                    actualTemps = tempStr.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (_startTime.HasValue)
                    recordStartTime = _startTime.Value;
                else
                    recordStartTime = DateTime.Now.AddSeconds(-actualTemps.Length * My_ConPar.Delay.TemRecordInterval);
            }

            // 2. 读取工艺标签（step文件）
            string stepFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "APP_DATA", $"{_ctCup.NO}_step.txt");
            string[] crafts = new string[0];
            if (System.IO.File.Exists(stepFile))
            {
                string craftStr = System.IO.File.ReadAllText(stepFile, Encoding.UTF8);
                if (!string.IsNullOrWhiteSpace(craftStr))
                    crafts = craftStr.TrimEnd('@').Split('@');
            }

            // 3. 生成理论曲线
            string[] logicTemps = null;
            if (_steps.Count > 0)
            {
                var chartData = CtTemperatureChart.GenerateChartData(_steps.ToArray(), My_ConPar.Delay.TemRecordInterval);
                logicTemps = chartData.temperature.Split('@');
            }

            // 4. 清空图表
            ctTemperatureChart1.ClearChart();
            DateTime strartTime = _startTime ?? DateTime.Now;
            // 5. 按模式显示
            if (_chartShowMode == 0)
            {
                // 只显示实际曲线+工艺标签
                if (actualTemps != null && recordStartTime.HasValue)
                    ctTemperatureChart1.SetDataWithCraft(actualTemps, crafts, strartTime, My_ConPar.Delay.TemRecordInterval);
            }
            else if (_chartShowMode == 1)
            {
                // 只显示理论曲线（带理论标签）
                if (logicTemps != null)
                    ctTemperatureChart1.ShowProcessCurve(_steps.ToArray(), My_ConPar.Delay.TemRecordInterval, textBox11);
            }
            else if (_chartShowMode == 2)
            {
                // 共同显示：实际+理论（不显示标签，颜色区分）
                if (actualTemps != null && recordStartTime.HasValue)
                    ctTemperatureChart1.AddTemperatureSeries("实际曲线", actualTemps, strartTime, My_ConPar.Delay.TemRecordInterval, Color.OrangeRed);
                if (logicTemps != null)
                    ctTemperatureChart1.AddTemperatureSeries("理论曲线", logicTemps, strartTime, My_ConPar.Delay.TemRecordInterval, Color.Gray);
            }

            // 6. 理论总时间显示
            if (_steps.Count > 0 && logicTemps != null)
                textBox11.Text = ctTemperatureChart1.GetTotalTimeFormatted(logicTemps.Length, My_ConPar.Delay.TemRecordInterval);
            else
                textBox11.Text = "";
        }

        /// <summary>
        /// 窗体加载时，异步加载配方详情和染固色工艺信息，并更新界面。
        /// </summary>
        private async void CupInfo_Load(object sender, EventArgs e)
        {
            // 首次加载或刷新时，异步加载数据
            await LoadCupInfoAsync();
        }

        /// <summary>
        /// 异步加载配方详情和工艺信息，后台线程处理数据，主线程刷新UI
        /// </summary>
        private async Task LoadCupInfoAsync()
        {
            if (_ctCup.Status == "下线")
                return;

            // 1. 后台线程处理数据
            var result = await Task.Run((Func<CupInfoData>)(() =>
            {
                // 数据结构用于传递到UI线程
                var data = new CupInfoData();

                // 查询杯子详细表找到HeadID
                DataRow headIDRow = null;
                string headID = "", dyeingCode = "", totalStep = "", stepNum = "", technologyName = "";
                string setTemp = "", realTemp = "", setTime = "", currentWeight = "", stepStartTime = "", startTime = "";

                var dtHeadID = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = '{_ctCup.NO}'");
                if (dtHeadID != null && dtHeadID.Rows.Count > 0)
                {

                    headIDRow = dtHeadID.Rows[0];
                    headID = headIDRow[CUP_DETAILS.HeadID]?.ToString();
                    dyeingCode = headIDRow[CUP_DETAILS.DyeingCode]?.ToString();
                    totalStep = headIDRow[CUP_DETAILS.TotalStep]?.ToString();
                    stepNum = headIDRow[CUP_DETAILS.StepNum]?.ToString();
                    technologyName = headIDRow[CUP_DETAILS.TechnologyName]?.ToString();
                    setTemp = headIDRow[CUP_DETAILS.SetTemp]?.ToString();
                    realTemp = headIDRow[CUP_DETAILS.RealTemp]?.ToString();
                    setTime = headIDRow[CUP_DETAILS.SetTime]?.ToString();
                    currentWeight = headIDRow[CUP_DETAILS.CurrentWeight]?.ToString();
                    stepStartTime = headIDRow[CUP_DETAILS.StepStartTime]?.ToString();
                    startTime = headIDRow[CUP_DETAILS.StartTime]?.ToString();
                }

                data.DyeingCode = dyeingCode;
                data.IsWashCup = !string.IsNullOrEmpty(dyeingCode) && CupAuxiliary.WashCupDic.ContainsKey(dyeingCode);

                // 查询滴液表获取配方代码和版本号
                DataRow headRow = null;
                string formulaCode = "", versionNum = "", objectWater = "", realWater = "";
                TimeSpan usedTime = TimeSpan.Zero;
                if (!string.IsNullOrEmpty(headID))
                {
                    var dtHeadRows = SqlServer.Select(DROP_HEAD.TableName, $"{DROP_HEAD.MyID} = {Convert.ToInt32(headID)}");
                    if (dtHeadRows != null && dtHeadRows.Rows.Count > 0)
                    {
                        headRow = dtHeadRows.Rows[0];
                        formulaCode = headRow[DROP_HEAD.FormulaCode]?.ToString();
                        versionNum = headRow[DROP_HEAD.VersionNum]?.ToString();
                        objectWater = headRow[DROP_HEAD.ObjectAddWaterWeight]?.ToString();
                        realWater = headRow[DROP_HEAD.RealAddWaterWeight]?.ToString();

                        if (!(headRow[DROP_HEAD.StartTime] is DBNull || headRow[DROP_HEAD.FinishTime] is DBNull))
                        {
                            DateTime startTime1 = Convert.ToDateTime(headRow[DROP_HEAD.StartTime]);
                            DateTime finishTime = Convert.ToDateTime(headRow[DROP_HEAD.FinishTime]);
                            usedTime = finishTime - startTime1;
                        }
                    }
                }

                // 查询滴液详情表获取配方详情
                DataTable dtDropDetails = null;
                if (!string.IsNullOrEmpty(headID))
                {
                    var dtDropDetailsRows = SqlServer.Select(DROP_DETAILS.TableName, $"{DROP_DETAILS.HeadID} = '{headID}'");
                    if (dtDropDetailsRows != null && dtDropDetailsRows.Rows.Count > 0)
                        dtDropDetails = dtDropDetailsRows;
                }

                // 查询染固色详情表获取染固色工艺详情
                DataTable dtDyeDetails = null;
                if (!string.IsNullOrEmpty(headID))
                {
                    dtDyeDetails = SqlServer.Select(DYE_DETAILS.TableName, null, $"{DYE_DETAILS.HeadID} = '{headID}'", DYE_DETAILS.StepNum, true);


                }

                // 处理工艺步骤
                var steps = new List<ProcessStep>();
                if (data.IsWashCup)
                {
                    // 洗杯类型
                    var washSteps = CupAuxiliary.WashCupDic[dyeingCode];
                    foreach (var ws in washSteps)
                    {
                        steps.Add(new ProcessStep
                        {
                            StepName = ws.TechnologyName,
                            TargetTemperature = ws.Temp,
                            HeatingRate = ws.TempSpeed,
                            Duration = 0 // 如有时间可补充
                        });
                    }
                    // 步骤号、总步数
                    stepNum = _ctCup.CurrentStepNo.ToString();
                    totalStep = washSteps.Count.ToString();
                    technologyName = _ctCup.TechnologyName;
                    setTemp = washSteps[_ctCup.CurrentStepNo == 0 ? 0 : _ctCup.CurrentStepNo - 1].Temp.ToString();
                    realTemp = Math.Round(_ctCup.ActualTemp / 10, 1).ToString();
                    setTime = washSteps[_ctCup.CurrentStepNo == 0 ? 0 : _ctCup.CurrentStepNo - 1].SetTime.ToString();
                }
                else
                {
                    // 常规染色
                    if (!string.IsNullOrEmpty(dyeingCode))
                    {
                        var allRows = SqlServer.Select(DYE_DETAILS.TableName, null, $"{DYE_DETAILS.HeadID} = '{headID}'", DYE_DETAILS.StepNum, true);

                        foreach (DataRow dr in allRows.Rows)
                        {
                            steps.Add(new ProcessStep
                            {
                                // 工艺名称
                                StepName = dr[DYE_DETAILS.TechnologyName]?.ToString() ?? "",
                                // 温度
                                TargetTemperature = double.TryParse(dr[DYE_DETAILS.Temp]?.ToString(), out var temp) ? temp : (double?)null,
                                // 速率
                                HeatingRate = double.TryParse(dr[DYE_DETAILS.TempSpeed]?.ToString(), out var rate) ? rate : (double?)null,
                                // 时间
                                Duration = double.TryParse(dr[DYE_DETAILS.Time]?.ToString(), out var duration) ? duration : (double?)null
                            });
                        }

                        // 步骤号、总步数、工艺名、设定温度、实际温度、设定时间
                        stepNum = _ctCup.CurrentStepNo.ToString();
                        totalStep = allRows.Rows.Count.ToString();
                        technologyName = _ctCup.TechnologyName;
                        realTemp = Math.Round(_ctCup.ActualTemp / 10, 1).ToString();

                        setTemp = allRows.Rows[_ctCup.CurrentStepNo == 0 ? 0 : _ctCup.CurrentStepNo - 1][DYE_DETAILS.Temp]?.ToString() ?? "";

                        setTime = allRows.Rows[_ctCup.CurrentStepNo == 0 ? 0 : _ctCup.CurrentStepNo - 1][DYE_DETAILS.Time]?.ToString() ?? "";
                    }

                }

                // 填充数据结构
                data.FormulaCode = formulaCode;
                data.VersionNum = versionNum;
                data.ObjectWater = objectWater;
                data.RealWater = realWater;
                data.UsedTime = usedTime;
                data.DropDetails = dtDropDetails;
                data.DyeDetails = dtDyeDetails;
                data.WashSteps = data.IsWashCup ? CupAuxiliary.WashCupDic[dyeingCode] : null;
                data.TotalStep = totalStep;
                data.StepNum = stepNum;
                data.TechnologyName = technologyName;
                data.SetTemp = setTemp;
                data.RealTemp = realTemp;
                data.SetTime = setTime;
                data.CurrentWeight = currentWeight;
                data.StepStartTime = stepStartTime;
                data.StartTime = startTime;
                data.Steps = steps;

                return data;
            }));

            // 2. UI线程刷新界面
            if (this.IsHandleCreated)
            {
                // 更新字段
                _dyeingCode = result.DyeingCode;
                _isWashCup = result.IsWashCup;
                _steps = result.Steps;

                // 显示头部信息
                txt_CupNum.Text = _ctCup.NO;
                txt_FormulaCode.Text = result.FormulaCode;
                txt_VersionNum.Text = result.VersionNum;

                // 显示加水量
                txt_objectWater.Text = result.ObjectWater;
                txt_realWater.Text = result.RealWater;




                // 配方详情
                ctDataGridView1.DataSource = null;
                ctDataGridView1.Rows.Clear();
                if (result.DropDetails != null)
                {
                    foreach (DataRow row in result.DropDetails.Rows)
                    {
                        ctDataGridView1.Rows.Add(
                            row[My_DataBase.DROP_DETAILS.IndexNum]?.ToString() ?? "",
                            row[My_DataBase.DROP_DETAILS.AssistantName]?.ToString() ?? "",
                            row[My_DataBase.DROP_DETAILS.FormulaDosage]?.ToString() ?? "",
                            row[My_DataBase.DROP_DETAILS.UnitOfAccount]?.ToString() ?? "",
                            row[My_DataBase.DROP_DETAILS.BottleNum]?.ToString() ?? "",
                            row[My_DataBase.DROP_DETAILS.ObjectDropWeight]?.ToString() ?? "",
                            row[My_DataBase.DROP_DETAILS.RealDropWeight]?.ToString() ?? ""
                        );
                    }
                    ctDataGridView1.ClearSelection();
                }

                // 染固色工艺或洗杯流程
                ctDataGridView2.DataSource = null;
                ctDataGridView2.Rows.Clear();

                if (_isWashCup && result.WashSteps != null)
                {
                    foreach (var ws in result.WashSteps)
                    {
                        ctDataGridView2.Rows.Add(
                            ws.StepNo,
                            ws.TechnologyName,
                            ws.Temp,
                            ws.TempSpeed,
                            ws.SetTime, "", "", "", "", "", "", "", ""
                        );
                    }
                }
                else if (result.DyeDetails != null)
                {
                    foreach (DataRow row in result.DyeDetails.Rows)
                    {
                        ctDataGridView2.Rows.Add(
                            row[My_DataBase.DYE_DETAILS.StepNum]?.ToString() ?? "",
                            row[My_DataBase.DYE_DETAILS.TechnologyName]?.ToString() ?? "",
                            row[My_DataBase.DYE_DETAILS.Temp]?.ToString() ?? "",
                            row[My_DataBase.DYE_DETAILS.TempSpeed]?.ToString() ?? "",
                            row[My_DataBase.DYE_DETAILS.Time]?.ToString() ?? "",
                            row[My_DataBase.DYE_DETAILS.ObjectWaterWeight]?.ToString() ?? "",
                            row[My_DataBase.DYE_DETAILS.AssistantName]?.ToString() ?? "",
                            row[My_DataBase.DYE_DETAILS.FormulaDosage]?.ToString() ?? "",
                            row[My_DataBase.DYE_DETAILS.BottleNum]?.ToString() ?? "",
                            row[My_DataBase.DYE_DETAILS.ObjectDropWeight]?.ToString() ?? "",
                            row[My_DataBase.DYE_DETAILS.RealDropWeight]?.ToString() ?? "",
                            row[My_DataBase.DYE_DETAILS.StartTime]?.ToString() ?? "",
                            row[My_DataBase.DYE_DETAILS.FinishTime]?.ToString() ?? ""
                        );
                    }
                    ctDataGridView2.ClearSelection();
                }

                // groupBox2左侧信息（染固色相关/洗杯相关）
                textBox3.Text = result.DyeingCode;
                textBox4.Text = result.TotalStep;
                textBox5.Text = result.SetTemp;
                textBox8.Text = result.SetTime;
                textBox6.Text = Math.Round(_ctCup.ActualTemp / 10, 1).ToString();
                textBox7.Text = Math.Round(Convert.ToDouble(result.CurrentWeight), _rd).ToString();
                textBox2.Text = result.StepNum;
                textBox10.Text = result.TechnologyName;
                textBox9.Text = _ctCup.HoldingTime.ToString();

                // 已用时、预计完成时间、预计剩余时间
                if (!string.IsNullOrEmpty(result.StartTime) && DateTime.TryParse(result.StartTime, out var st))
                {
                    _startTime = st;
                    // 显示已用时间
                    var used = DateTime.Now - _startTime.Value;
                    txt_usedTime.Text = used.ToString(@"hh\:mm\:ss");
                }

                else
                {
                    _startTime = null;
                    txt_usedTime.Text = string.Empty;
                }

                if (!string.IsNullOrEmpty(result.StepStartTime) && DateTime.TryParse(result.StepStartTime, out var sst))
                    _stepStartTime = sst;
                else
                    _stepStartTime = null;



                // 刷新温度曲线
                RefreshTemperatureChart();

                // Timer控制
                if (_ctCup.Status == "待机")
                {
                    if (timer1.Enabled)
                        timer1.Stop();
                }
                else
                {
                    if (!timer1.Enabled)
                        timer1.Start();
                }

                // 预计完成时间
                textBox12.Text = (_startTime.HasValue && _steps.Count > 0)
                    ? CtTemperatureChart.GetEstimatedFinishTimeString(0, _startTime.Value, _steps.ToArray(), My_ConPar.Delay.TemRecordInterval)
                    : "";
                textBox13.Text = (_stepStartTime.HasValue && _steps.Count > 0)
                    ? CtTemperatureChart.GetEstimatedFinishTimeString(Convert.ToInt16(result.StepNum) == 0 ? 0 : Convert.ToInt16(result.StepNum) - 1, _stepStartTime.Value, _steps.ToArray(), My_ConPar.Delay.TemRecordInterval)
                    : "";

                // 设置ctDataGridView2行背景色
                if (int.TryParse(result.StepNum, out int currentStep) && ctDataGridView2.Rows.Count > 0)
                {
                    for (int i = 0; i < ctDataGridView2.Rows.Count; i++)
                    {
                        var row = ctDataGridView2.Rows[i];
                        if (i < currentStep - 1)
                        {
                            // 已完成步骤，黄色
                            row.DefaultCellStyle.BackColor = Color.LightYellow;
                        }
                        else if (i == currentStep - 1)
                        {
                            // 当前步骤，蓝色
                            row.DefaultCellStyle.BackColor = Color.DeepSkyBlue;
                        }
                        else
                        {
                            // 未来步骤，默认色
                            row.DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                }
                ctDataGridView2.ClearSelection();
                ctDataGridView2.CurrentCell = null;
            }
        }

        /// <summary>
        /// 用于后台线程与UI线程数据传递的结构体
        /// </summary>
        private class CupInfoData
        {
            public string DyeingCode;
            public bool IsWashCup;
            public string FormulaCode;
            public string VersionNum;
            public string ObjectWater;
            public string RealWater;
            public TimeSpan UsedTime;
            public DataTable DropDetails;
            public DataTable DyeDetails;
            public List<CupAuxiliary.WashCupPar> WashSteps; // 类型修正
            public string TotalStep;
            public string StepNum;
            public string TechnologyName;
            public string SetTemp;
            public string RealTemp;
            public string SetTime;
            public string CurrentWeight;
            public string StepStartTime;
            public string StartTime;
            public List<ProcessStep> Steps;
        }
    }
}