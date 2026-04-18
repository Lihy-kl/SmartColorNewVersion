using SmartColor.My_Control;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Form.BasicData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SmartColor.My_Form.HistoricalData
{

    public partial class Dyeing : Form
    {

        private DateTime _startTime = DateTime.Now;
        private DataTable _head = null;
        private string _code = string.Empty;

        public Dyeing()
        {
            InitializeComponent();
            this.ctDropHead1.SetMode(CtDropHead.Mode.Dye);
            this.ctDropHistoryHead1.SetMode(My_Control.CtHistoryDyeBrowse.BrowseMode.HistoryDrop);
            this.ctDropHistoryHead1.CurrentRowChanged += CtDropHistoryHead1_CurrentRowChanged;
            this.ctTemperatureChart1.ShowModeChanged += CtTemperatureChart1_ShowModeChanged;
            // 设置CtDropDetail控件属性
            this.ctDropDetail1.SetMode(CtDropDetail.Mode.DYE);
            this.ctDropDetail1.BindDropHead(ctDropHead1);

            this.ctDropHead1.ReadOnly();
            this.ctDropDetail1.ReadOnly();
        }

        private void CtTemperatureChart1_ShowModeChanged(object sender, int e)
        {
            switch (e)
            {
                case 0:
                    LoadMeasuredCurve(_head, e);
                    break;
                case 1:
                    LoadProcessCurve(_code, e);
                    break;

                case 2:
                    LoadProcessCurve(_code, e);
                    LoadMeasuredCurve(_head, e);
                    break;
                default:
                    break;
            }
        }

        private void CtDropHistoryHead1_CurrentRowChanged(object sender, EventArgs e)
        {
            //获取当前行信息
            var row = this.ctDropHistoryHead1.CurrentRow;
            if (row != null)
            {
                int id = Convert.ToInt32(row[My_DataBase.HISTORY_HEAD.MyID]);
                //找到表头信息
                var dt = SqlServer.Select(My_DataBase.HISTORY_HEAD.TableName,
                    $"{My_DataBase.HISTORY_HEAD.MyID} = {id}");

                if (dt != null)
                {
                    //填充表头信息
                    this.ctDropHead1.FillControlsFromDataTable(dt, CtDropHead.DataSource.History);

                    //计算并填充实际用时
                    textBox1.Text = CalculateActualTime(dt);

                    //显示结果
                    textBox2.Text = dt.Rows[0][My_DataBase.HISTORY_HEAD.Result]?.ToString();


                    //填充明细信息
                    var dyeingCode = dt.Rows[0][My_DataBase.HISTORY_HEAD.DyeingCode]?.ToString();
                    if (!string.IsNullOrWhiteSpace(dyeingCode))
                    {
                        this.ctTemperatureChart1.ContextMenuStrip = this.ctTemperatureChart1.contextMenuStrip1;
                        _code = dyeingCode;
                        _head = dt;
                        CtTemperatureChart1_ShowModeChanged(sender, 0);

                    }
                    else
                    {
                        this.ctTemperatureChart1.ContextMenuStrip = null;
                        this.ctTemperatureChart1.ClearChart();
                    }
                }

                this.ctDropDetail1.ReadOnly();
            }
        }

        private string CalculateActualTime(DataTable head)
        {
            // 获取表头的开始和结束时间
            var startTimeStr = head.Rows[0][My_DataBase.HISTORY_HEAD.StartTime]?.ToString();
            var endTimeStr = head.Rows[0][My_DataBase.HISTORY_HEAD.FinishTime]?.ToString();
            var dyeingCode = head.Rows[0][My_DataBase.HISTORY_HEAD.DyeingCode]?.ToString();

            if (string.IsNullOrWhiteSpace(dyeingCode))
            {
                // 单纯滴液
                if (DateTime.TryParse(startTimeStr, out var startTime) && DateTime.TryParse(endTimeStr, out var endTime))
                {
                    var duration = endTime - startTime;
                    return duration.ToString(@"hh\:mm\:ss");
                }
                this.ctTemperatureChart1.ContextMenuStrip = null;
            }
            else
            {
                // 有染固色工艺
                var batchNum = head.Rows[0][My_DataBase.HISTORY_HEAD.BatchName]?.ToString();
                var cup = head.Rows[0][My_DataBase.HISTORY_HEAD.CupNum]?.ToString();

                // 只查一次，过滤掉StartTime和FinishTime为NULL的数据，按StartTime升序排序
                var dt = SqlServer.Select(
                    My_DataBase.HISTORY_DYE.TableName,
                    new[] { "*" }, // 查询所有字段
                    $"{My_DataBase.HISTORY_DYE.BatchName} = '{batchNum}' AND {My_DataBase.HISTORY_DYE.CupNum} = {cup} " +
                    $"AND {My_DataBase.HISTORY_DYE.StartTime} IS NOT NULL AND {My_DataBase.HISTORY_DYE.FinishTime} IS NOT NULL",
                    My_DataBase.HISTORY_DYE.StartTime, // 排序字段
                    true // 升序
                );

                if (dt != null && dt.Rows.Count > 0)
                {
                    // 第一行是最早的开始时间，最后一行是最晚的完成时间
                    var firstRow = dt.Rows[0];
                    var lastRow = dt.Rows[dt.Rows.Count - 1];

                    startTimeStr = firstRow[My_DataBase.HISTORY_DYE.StartTime]?.ToString();
                    endTimeStr = lastRow[My_DataBase.HISTORY_DYE.FinishTime]?.ToString();
                    _startTime = Convert.ToDateTime(startTimeStr);

                    this.ctTemperatureChart1.ContextMenuStrip = this.ctTemperatureChart1.contextMenuStrip1;

                    if (DateTime.TryParse(startTimeStr, out var startTime) && DateTime.TryParse(endTimeStr, out var endTime))
                    {
                        var duration = endTime - startTime;
                        return duration.ToString(@"hh\:mm\:ss");
                    }
                }
            }

            return "00:00:00";
        }

        private void LoadProcessCurve(string dyeingCode, int mode)
        {
            // 获取工艺步骤
            var row = this.ctDropHistoryHead1.CurrentRow;
            if (row == null) return;


            string fc = row[My_DataBase.HISTORY_HEAD.FormulaCode]?.ToString();
            int vn = Convert.ToInt32(row[My_DataBase.HISTORY_HEAD.VersionNum]);


            var allRows = My_Tool.FindDyeingCode.GetAllDyeDetailFromFormulaCode(dyeingCode, fc, vn);

            // 构造 ProcessStep[]
            var steps = new List<SmartColor.My_Control.ProcessStep>();
            foreach (var dr in allRows)
            {
                steps.Add(new SmartColor.My_Control.ProcessStep
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

            // 生成理论温度点和工艺标记
            var chartData = SmartColor.My_Control.CtTemperatureChart.GenerateChartData(steps.ToArray(), My_ConPar.Delay.TemRecordInterval);
            var temperatureArr = chartData.temperature.Split('@');
            var craftArr = chartData.craft.Split('@');


            // 清空所有曲线
            ctTemperatureChart1.ClearChart();

            // 添加理论曲线（红色）
            ctTemperatureChart1.AddTemperatureSeries("理论曲线", temperatureArr, _startTime, My_ConPar.Delay.TemRecordInterval, Color.Red);

            // 添加工艺标记点（只在单理论上显示）
            if (craftArr.Length > 0 && mode == 1)
                ctTemperatureChart1.SetDataWithCraft(temperatureArr, craftArr, _startTime, My_ConPar.Delay.TemRecordInterval);
        }

        private void LoadMeasuredCurve(DataTable head, int mode)
        {
            var processDataObj = head.Rows[0][My_DataBase.HISTORY_HEAD.ProcessData];
            var markStepObj = head.Rows[0][My_DataBase.HISTORY_HEAD.MarkStep];
            if (processDataObj is DBNull)
            {
                this.ctTemperatureChart1.ClearChart();
                return;
            }
            byte[] bytStr = (byte[])processDataObj;
            string temperatureStr = My_Tool.Base64.Base64Decrypt(Encoding.Default.GetString(bytStr));
            string[] temperatureArr = temperatureStr.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

            // 解析工艺标记点
            string[] craftArr = new string[0];
            if (!(markStepObj is DBNull))
            {
                string markStepStr = markStepObj.ToString();
                if (markStepStr.EndsWith("@")) markStepStr = markStepStr.Substring(0, markStepStr.Length - 1);
                if (!string.IsNullOrWhiteSpace(markStepStr))
                    craftArr = markStepStr.Split('@');
            }


            int intervalSeconds = My_ConPar.Delay.TemRecordInterval;

            // 添加实测曲线（绿色），
            ctTemperatureChart1.AddTemperatureSeries("实测曲线", temperatureArr, _startTime, intervalSeconds, Color.Green);

            // 添加工艺标记点（只在单实测曲线显示）
            if (craftArr.Length > 0 && mode == 0)
                ctTemperatureChart1.SetDataWithCraft(temperatureArr, craftArr, _startTime, intervalSeconds);

        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            //把结果修改到指定位置
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                My_File.LocalTranslator.ShowMessage("结果不能为空", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //获取当前行信息
            var row = this.ctDropHistoryHead1.CurrentRow;
            var dc = new Dictionary<string, object>();
            dc.Add(My_DataBase.HISTORY_HEAD.Result, textBox2.Text);
            if (row != null)
            {
                int id = Convert.ToInt32(row[My_DataBase.HISTORY_HEAD.MyID]);
                int i = My_DataBase.SqlServer.Update(My_DataBase.HISTORY_HEAD.TableName,
                   dc, $"{My_DataBase.HISTORY_HEAD.MyID} = {id}");
                if (i > 0)
                {
                    My_File.LocalTranslator.ShowMessage("保存成功", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 刷新当前行数据

                    CtDropHistoryHead1_CurrentRowChanged(this.ctDropHistoryHead1, EventArgs.Empty);
                }
                else
                {
                    My_File.LocalTranslator.ShowMessage("保存失败", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}