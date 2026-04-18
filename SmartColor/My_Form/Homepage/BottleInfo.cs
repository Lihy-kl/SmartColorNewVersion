using SmartColor.My_DataBase;
using SmartColor.My_Form.BasicData;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Form.Homepage
{
    /// <summary>
    /// 瓶信息展示窗体，负责显示指定瓶号的详细信息及相关助剂信息，并绘制UV曲线。
    /// </summary>
    public partial class BottleInfo : Form
    {
        // 当前选中的瓶数据行
        private DataRow _currentBottleRow;
        // 当前选中的助剂数据行
        private DataRow _currentAssistantRow;
        // 当前瓶号
        private readonly string _bottleNo;

        /// <summary>
        /// 构造函数，初始化窗体并绑定事件。
        /// </summary>
        /// <param name="bottleNo">要显示的瓶号</param>
        public BottleInfo(string bottleNo)
        {
            InitializeComponent();
            this.Load += BottleInfo_Load;
            this.timer1.Tick += Timer1_Tick;
            this._bottleNo = bottleNo;

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 停止并释放定时器
                if (timer1 != null)
                {
                    timer1.Stop();
                    timer1.Dispose();
                }
                // 释放自定义控件
                if (ctUvCurveChart1 != null)
                {
                    ctUvCurveChart1.Dispose();
                }
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 窗体加载或数据变更时触发，异步加载瓶和助剂信息，并更新界面。
        /// </summary>
        private async void BottleInfo_Load(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                // 1. 查询瓶数据
                var dt = My_DataBase.BottleData.Bottle_details;
                if (dt == null || dt.Rows.Count == 0) return;

                DataRow bottleRow = GetBottleRow(dt, _bottleNo);
                if (bottleRow == null) return;

                // 2. 查询助剂数据
                DataRow assistantRow = GetAssistantRow(bottleRow);

                var assistantCode = bottleRow[My_DataBase.BOTTLE_DETAILS.AssistantCode]?.ToString();


                // 3. 当前瓶UV曲线数据准备
                double[] yValues = null;
                int startWave = 320, interval = 5;
                if (TryGetUvCurveParams(bottleRow, out yValues, out startWave, out interval))
                {
                    // UV曲线参数已获取
                }

                // 4. 回到UI线程赋值
                this.Invoke((Action)(() =>
                {
                    _currentBottleRow = bottleRow;
                    _currentAssistantRow = assistantRow;

                    // 更新瓶信息控件
                    SetBottleInfoControls(bottleRow);

                    // 更新助剂信息控件
                    SetAssistantInfoControls(assistantRow);

                    //绘制标样UV曲线

                    ctUvCurveChart1.ClearCurves();

                    DataTable dataTable = SqlServer.Select(My_DataBase.ABS_HISTORY_HEAD.TableName,
                        $"{My_DataBase.ABS_HISTORY_HEAD.Stand} = 1 AND {My_DataBase.ABS_HISTORY_HEAD.FormulaCode} = '{assistantCode}' AND {My_DataBase.ABS_HISTORY_HEAD.Type} = 0");
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        var standRow = dataTable.Rows[0];

                        if (standRow[My_DataBase.ABS_HISTORY_HEAD.Abs] != DBNull.Value)
                        {
                            string absStr = bottleRow[My_DataBase.BOTTLE_DETAILS.Abs].ToString();
                            var SValues = absStr.Split('/').Select(s =>
                               {
                                   double v;
                                   return double.TryParse(s, out v) ? v : 0;
                               }).ToArray();

                            int standStartWave = Convert.ToInt32(standRow[My_DataBase.ABS_HISTORY_HEAD.StartWave]);
                            int standInterval = Convert.ToInt32(standRow[My_DataBase.ABS_HISTORY_HEAD.IntWave]);

                            ctUvCurveChart1.AddCurveAutoColor("标样UV", SValues, standStartWave, standInterval);
                        }
                    }

                    // 绘制UV曲线

                    if (yValues != null)
                    {
                        ctUvCurveChart1.AddCurveAutoColor($"{_bottleNo}号瓶UV", yValues, startWave, interval);
                    }

                    // 启动定时器，刷新剩余时间
                    timer1.Interval = 1000;
                    timer1.Start();
                    RefreshLeftTime();
                }));
            });
        }

        /// <summary>
        /// 定时器事件，刷新剩余有效时间显示。
        /// </summary>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            RefreshLeftTime();
        }

        /// <summary>
        /// 计算并显示剩余有效时间（格式：HH:mm:ss）。
        /// </summary>
        private void RefreshLeftTime()
        {
            txt_LeftTime.Text = "";
            // 有效期（小时）和开料日期（调液日期）均有效时才计算
            if (!string.IsNullOrWhiteSpace(txt_TermOfValidity.Text) && !string.IsNullOrWhiteSpace(txt_BrewingData.Text))
            {
                if (double.TryParse(txt_TermOfValidity.Text, out double hours) &&
                    DateTime.TryParse(txt_BrewingData.Text, out DateTime brewTime))
                {
                    var expireTime = brewTime.AddHours(hours);
                    var left = expireTime - DateTime.Now;
                    txt_LeftTime.Text = left.TotalSeconds > 0
                        ? string.Format("{0:D2}:{1:D2}:{2:D2}", (int)left.TotalHours, left.Minutes, left.Seconds)
                        : "00:00:00";
                }
            }
        }

        #region 优化：辅助方法提取

        /// <summary>
        /// 获取指定瓶号的数据行，若未找到则返回首行。
        /// </summary>
        private DataRow GetBottleRow(DataTable dt, string bottleNo)
        {
            if (!string.IsNullOrWhiteSpace(bottleNo) && dt.Columns.Contains(My_DataBase.BOTTLE_DETAILS.BottleNum))
            {
                var rows = dt.Select($"{My_DataBase.BOTTLE_DETAILS.BottleNum}='{bottleNo}'");
                return rows.Length > 0 ? rows[0] : null;
            }
            return null;
        }

        /// <summary>
        /// 获取瓶对应的助剂数据行，若未找到则返回null。
        /// </summary>
        private DataRow GetAssistantRow(DataRow bottleRow)
        {
            if (bottleRow == null) return null;
            string assistantCode = bottleRow[My_DataBase.BOTTLE_DETAILS.AssistantCode]?.ToString();
            var drs = My_DataBase.AssistantData.Assistant_details?.Select($"{My_DataBase.ASSISTANT_DETAILS.AssistantCode}='{assistantCode}'");
            return (drs != null && drs.Length > 0) ? drs[0] : null;
        }

        /// <summary>
        /// 获取UV曲线参数（y值数组、起始波长、间隔），返回是否成功。
        /// </summary>
        private bool TryGetUvCurveParams(DataRow bottleRow, out double[] yValues, out int startWave, out int interval)
        {
            yValues = null;
            startWave = 400;
            interval = 10;
            if (bottleRow?.Table.Columns.Contains(My_DataBase.BOTTLE_DETAILS.Abs) == true && bottleRow[My_DataBase.BOTTLE_DETAILS.Abs] != DBNull.Value)
            {
                string absStr = bottleRow[My_DataBase.BOTTLE_DETAILS.Abs].ToString();
                yValues = absStr.Split('/').Select(s =>
                {
                    double v;
                    return double.TryParse(s, out v) ? v : 0;
                }).ToArray();


                if (bottleRow.Table.Columns.Contains(My_DataBase.BOTTLE_DETAILS.StartingWavelength) && bottleRow[My_DataBase.BOTTLE_DETAILS.StartingWavelength] != DBNull.Value)
                    startWave = Convert.ToInt32(bottleRow[My_DataBase.BOTTLE_DETAILS.StartingWavelength]);
                if (bottleRow.Table.Columns.Contains(My_DataBase.BOTTLE_DETAILS.WavelengthInterval) && bottleRow[My_DataBase.BOTTLE_DETAILS.WavelengthInterval] != DBNull.Value)
                    interval = Convert.ToInt32(bottleRow[My_DataBase.BOTTLE_DETAILS.WavelengthInterval]);

                return true;
            }
            return false;
        }

        /// <summary>
        /// 批量设置瓶信息控件的文本。
        /// </summary>
        private void SetBottleInfoControls(DataRow bottleRow)
        {
            txt_BottleNum.Text = bottleRow[My_DataBase.BOTTLE_DETAILS.BottleNum]?.ToString();
            txt_AssistantCode.Text = bottleRow[My_DataBase.BOTTLE_DETAILS.AssistantCode]?.ToString();
            txt_SettingConcentration.Text = bottleRow[My_DataBase.BOTTLE_DETAILS.SettingConcentration]?.ToString();
            txt_RealConcentration.Text = bottleRow[My_DataBase.BOTTLE_DETAILS.RealConcentration]?.ToString();
            txt_LastAdjustWeight.Text = GetColumnValue(bottleRow, My_DataBase.BOTTLE_DETAILS.LastAdjustWeight);
            txt_CurrentAdjustWeight.Text = GetColumnValue(bottleRow, My_DataBase.BOTTLE_DETAILS.CurrentAdjustWeight);
            txt_AdjustValue.Text = GetColumnValue(bottleRow, My_DataBase.BOTTLE_DETAILS.AdjustValue);
            txt_BrewingCode.Text = GetColumnValue(bottleRow, My_DataBase.BOTTLE_DETAILS.BrewingCode);
            txt_AllowMaxWeight.Text = GetColumnValue(bottleRow, My_DataBase.BOTTLE_DETAILS.AllowMaxWeight);
            txt_CurrentWeight.Text = GetColumnValue(bottleRow, My_DataBase.BOTTLE_DETAILS.CurrentWeight);
            txt_BrewingData.Text = GetColumnValue(bottleRow, My_DataBase.BOTTLE_DETAILS.BrewingData);
            txt_SelfChecking1.Text = GetColumnValue(bottleRow, My_DataBase.BOTTLE_DETAILS.SelfChecking1);
            txt_SelfChecking2.Text = GetColumnValue(bottleRow, My_DataBase.BOTTLE_DETAILS.SelfChecking2);
            txt_SelfChecking3.Text = GetColumnValue(bottleRow, My_DataBase.BOTTLE_DETAILS.SelfChecking3);
            txt_SelfChecking4.Text = GetColumnValue(bottleRow, My_DataBase.BOTTLE_DETAILS.SelfChecking4);
        }

        /// <summary>
        /// 批量设置助剂信息控件的文本。
        /// </summary>
        private void SetAssistantInfoControls(DataRow assistantRow)
        {
            if (assistantRow != null)
            {
                txt_AssistantName.Text = assistantRow[My_DataBase.ASSISTANT_DETAILS.AssistantName]?.ToString();
                txt_AssistantType.Text = assistantRow[My_DataBase.ASSISTANT_DETAILS.AssistantType]?.ToString();
                txt_UnitOfAccount.Text = assistantRow[My_DataBase.ASSISTANT_DETAILS.UnitOfAccount]?.ToString();
                txt_AllowMinColoringConcentration.Text = assistantRow[My_DataBase.ASSISTANT_DETAILS.AllowMinColoringConcentration]?.ToString();
                txt_AllowMaxColoringConcentration.Text = assistantRow[My_DataBase.ASSISTANT_DETAILS.AllowMaxColoringConcentration]?.ToString();
                txt_TermOfValidity.Text = assistantRow[My_DataBase.ASSISTANT_DETAILS.TermOfValidity]?.ToString();
                txt_Intensity.Text = assistantRow[My_DataBase.ASSISTANT_DETAILS.Intensity]?.ToString();
                txt_Cost.Text = assistantRow[My_DataBase.ASSISTANT_DETAILS.Cost]?.ToString();
            }
            else
            {
                txt_AssistantName.Text = "";
                txt_AssistantType.Text = "";
                txt_UnitOfAccount.Text = "";
                txt_AllowMinColoringConcentration.Text = "";
                txt_AllowMaxColoringConcentration.Text = "";
                txt_TermOfValidity.Text = "";
                txt_Intensity.Text = "";
                txt_Cost.Text = "";
            }
        }

        /// <summary>
        /// 安全获取DataRow指定列的字符串值，若列不存在则返回空字符串。
        /// </summary>
        private string GetColumnValue(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) ? row[columnName]?.ToString() : "";
        }

        #endregion
    }
}