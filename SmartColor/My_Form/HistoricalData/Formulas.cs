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
using static  System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SmartColor.My_Form.HistoricalData
{
    public partial class Formulas : Form
    {
        private DateTime _startTime = DateTime.Now;
        private DataTable _head = null;
        private string _code = string.Empty;

        public Formulas()
        {
            InitializeComponent();
            this.ctDropHistoryHead1.SetMode(My_Control.CtHistoryDyeBrowse.BrowseMode.HistoryFormula);

            this.ctDropHead1.SetMode(CtDropHead.Mode.Dye);
            this.ctDropHistoryHead1.CurrentRowChanged += CtDropHistoryHead1_CurrentRowChanged;
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
                case 1:
                    LoadProcessCurve(_code, e);
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
                int id = Convert.ToInt32(row[My_DataBase.FORMULA_HEAD.MyID]);
                //找到表头信息
                var dt = SqlServer.Select(My_DataBase.FORMULA_HEAD.TableName,
                    $"{My_DataBase.FORMULA_HEAD.MyID} = {id}");
                      
                if (dt != null)
                {
                    //填充表头信息
                    this.ctDropHead1.FillControlsFromDataTable(dt, 0);

                    //填充明细信息
                    var dyeingCode = dt.Rows[0][My_DataBase.HISTORY_HEAD.DyeingCode]?.ToString();
                    if (!string.IsNullOrWhiteSpace(dyeingCode))
                    {
                        _code = dyeingCode;
                        _head = dt;
                        CtTemperatureChart1_ShowModeChanged(sender, 1);

                    }
                    else
                    {
                        this.ctTemperatureChart1.ClearChart();
                    }
                }

                this.ctDropDetail1.ReadOnly();
            }
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
    }
}
