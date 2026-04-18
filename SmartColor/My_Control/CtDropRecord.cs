using SmartColor.My_Form.HistoricalData;
using SmartColor.My_File;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SmartColor.My_DataBase;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 染色历史记录展示控件，支持当天数据筛选、统计、行选中事件通知等功能
    /// </summary>
    public partial class CtDropRecord : UserControl
    {
        /// <summary>
        /// 当天历史记录当前行改变事件
        /// </summary>
        public event EventHandler<DataTable> CurrentRowChanged;

        /// <summary>
        /// 目标头部控件（如有），用于自动填充选中行数据
        /// </summary>
        public CtDropHead DropHeadTarget { get; set; }

        private bool _isShowTodayHistory = false;

        /// <summary>
        /// 构造函数，初始化控件及事件绑定
        /// </summary>
        public CtDropRecord()
        {
            InitializeComponent();
            ctDataGridView1.CurrentCellChanged += CtDataGridView1_CurrentCellChanged;
            SmartColor.My_Tool.CupAuxiliary.CupFinished += ShowTodayHistory;
            ShowTodayHistory();
            ClearSelect();
        }

       

        /// <summary>
        /// 显示当天的染色历史记录（按FinishTime筛选），并统计成功/失败数量
        /// </summary>
        public void ShowTodayHistory()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ShowTodayHistory));
                return; // Prevent further execution after invoking.
            }
            if (this.IsDisposed || ctDataGridView1 == null || ctDataGridView1.IsDisposed)
                return;
            try
            {
                _isShowTodayHistory = true;
                // 确保列已添加
                if (ctDataGridView1.Columns.Count == 0)
                {
                    ctDataGridView1.Columns.Add("FormulaCode", "配方编码");
                    ctDataGridView1.Columns.Add("VersionNum", "版本号");
                    ctDataGridView1.Columns.Add("FinishTime", "完成时间");
                    ctDataGridView1.Columns.Add("CupNum", "杯号");
                    ctDataGridView1.Columns.Add("ClothNum", "布号");
                    ctDataGridView1.Columns.Add("DescribeChar", "描述");
                    ctDataGridView1.Columns.Add("MyID", "MyID");
                }

                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                // 字段名缓存
                var colFormulaCode = My_DataBase.HISTORY_HEAD.FormulaCode;
                var colVersionNum = My_DataBase.HISTORY_HEAD.VersionNum;
                var colFinishTime = My_DataBase.HISTORY_HEAD.FinishTime;
                var colCupNum = My_DataBase.HISTORY_HEAD.CupNum;
                var colClothNum = My_DataBase.HISTORY_HEAD.ClothNum;
                var colDescribeChar = My_DataBase.HISTORY_HEAD.DescribeChar;
                var colMyID = My_DataBase.HISTORY_HEAD.MyID;

                // 查询当天数据
                string where = $"{colFinishTime} >= @today AND {colFinishTime} < @tomorrow";
                var dt = SmartColor.My_DataBase.SqlServer.Select(
                    My_DataBase.HISTORY_HEAD.TableName,
                    new[] { colFormulaCode, colVersionNum, colFinishTime, colCupNum, colClothNum, colDescribeChar, colMyID },
                    where,
                    colFinishTime,
                    false,
                    new System.Data.SqlClient.SqlParameter("@today", today),
                    new System.Data.SqlClient.SqlParameter("@tomorrow", tomorrow)
                );

                ctDataGridView1.Rows.Clear();

                int total = dt.Rows.Count;
                int success = 0;
                int fail = 0;

                foreach (System.Data.DataRow r in dt.Rows)
                {
                    string describe = r[colDescribeChar]?.ToString() ?? "";
                    bool isSuccess = describe.Contains("成功");
                    bool isFail = describe.Contains("失败");

                    if (isSuccess) success++;
                    if (isFail) fail++;

                    int idx = ctDataGridView1.Rows.Add(
                        r[colFormulaCode],
                        r[colVersionNum],
                        r[colFinishTime],
                        r[colCupNum],
                        colClothNum != null ? r[colClothNum] : "",
                        describe,
                        r[colMyID]
                    );

                    if (isFail)
                    {
                        ctDataGridView1.Rows[idx].DefaultCellStyle.ForeColor = Color.Red;
                    }
                }

                textBox1.Text = total.ToString();
                textBox2.Text = success.ToString();
                textBox3.Text = (total - success).ToString();

                ctDataGridView1.AutoFitAllColumns();
                ctDataGridView1.ClearSelection();
                _isShowTodayHistory = false;
            }
            catch (Exception ex)
            {
                Logger.Error("ShowTodayHistory: 显示当天历史记录异常", ex);
            }
        }

        /// <summary>
        /// 清除表格选中状态
        /// </summary>
        public void ClearSelect()
        {
            this.ctDataGridView1.CurrentCell = null;
            this.ctDataGridView1.ClearSelection();

        }

        /// <summary>
        /// 当前行改变事件处理，通过MyID唯一查找数据并自动填充目标控件
        /// </summary>
        private void CtDataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (_isShowTodayHistory) return;
            var row = ctDataGridView1.CurrentRow;

            if (row != null)
            {
                var colMyID = My_DataBase.HISTORY_HEAD.MyID;
                int myID = 0;
                // 第7列为MyID，安全转换
                if (row.Cells[6].Value != null)
                    int.TryParse(row.Cells[6].Value.ToString(), out myID);

                // 查找对应MyID的数据行
                var headTable = SqlServer.Select(My_DataBase.HISTORY_HEAD.TableName,
                    $"{colMyID} = {myID}");



                // 自动填充目标控件（如有）
                DropHeadTarget?.FillControlsFromDataTable(headTable, CtDropHead.DataSource.History);
            }

            // 触发外部事件通知
            CurrentRowChanged?.Invoke(this, null);
        }
    }
}