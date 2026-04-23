using SmartColor.My_DataBase;
using SmartColor.My_File;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 历史染色/配方/吸光度浏览控件
    /// 支持条件查询、删除、打印等功能
    /// </summary>
    public partial class CtHistoryDyeBrowse : UserControl
    {
        public enum BrowseMode
        {
            HistoryDrop,      // 显示染色记录表头
            HistoryFormula,   // 显示配方表头
            HistoryAbs        // 显示吸光度表头
        }

        public DataRow CurrentRow { get; private set; }
        public event EventHandler CurrentRowChanged;

        private BrowseMode _mode = BrowseMode.HistoryDrop;
        private bool _isLoading = false;

        public CtHistoryDyeBrowse()
        {
            InitializeComponent();
            this.InitRecordEvents();
            this.InitControlEvents();
            this.SetOperator();
            this.SetControlsEnabled();

            this.ctRecord1.dgv.RowPrePaint += this.Dgv_RowPrePaint_Custom;
            this.ctRecord1.dgv.ContextMenuStrip = this.contextMenuStrip1;
        }

        private void InitRecordEvents()
        {
            this.ctRecord1.dgv.CurrentCellChanged += this.Dgv_SelectionChanged;
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
            SqlServer.TableDataChanged += SqlServer_TableDataChanged;
        }

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void SqlServer_TableDataChanged(string obj)
        {
            if(obj == USER_TALE.TableName)
                this.SetOperator();
        }

        private void InitControlEvents()
        {
            this.btn_Record_Select.Click += this.Btn_Record_Select_Click;
            this.btn_Record_Delete.Click += this.Btn_Record_Delete_Click;
            this.btn_Record_Print.Click += this.Btn_Record_Print_Click;
            this.rdo_Record_Now.CheckedChanged += this.BrowseModeChanged;
            this.rdo_Record_All.CheckedChanged += this.BrowseModeChanged;
            this.rdo_Record_condition.CheckedChanged += this.BrowseModeChanged;
        }

        private void SetOperator()
        {
            if (My_DataBase.UserData.UserTable != null)
            {
                this.txt_Record_Operator.Items.Clear();
                foreach (DataRow row in My_DataBase.UserData.UserTable.Rows)
                {
                    if (row[My_DataBase.USER_TALE.RealName] != DBNull.Value
                        && row[My_DataBase.USER_TALE.Purview] != DBNull.Value
                        && Convert.ToInt32(row[My_DataBase.USER_TALE.Purview]) == 0)
                    {
                        this.txt_Record_Operator.Items.Add(row[My_DataBase.USER_TALE.RealName].ToString());
                    }
                }
            }
        }

        private void SetControlsEnabled()
        {
            bool enable = this.rdo_Record_condition.Checked;
            this.txt_Record_Operator.Enabled = enable;
            this.txt_Record_Code.Enabled = enable;
            this.txt_Record_CupNum.Enabled = enable;
            this.dt_Record_Start.Enabled = enable;
            this.dt_Record_End.Enabled = enable;

        }

        public void SetMode(BrowseMode mode)
        {
            this._mode = mode;
            this.RefreshData();
        }

        private void BrowseModeChanged(object sender, EventArgs e)
        {
            var radio = sender as RadioButton;
            // 只在选中时执行，取消选中时不处理
            if (radio == null || !radio.Checked) return;
            this.SetControlsEnabled();
            this.RefreshData();

        }

        /// <summary>
        /// 刷新数据和列名（核心优化：所有数据查询都走SQL）
        /// </summary>
        private void RefreshData()
        {
            if (this._isLoading) return;
            this._isLoading = true;
            try
            {
                DataTable dt = null;
                if (this.rdo_Record_condition.Checked)
                {
                    if (this._mode == BrowseMode.HistoryDrop)
                        dt = QueryHistoryDropData();
                    else if (this._mode == BrowseMode.HistoryFormula)
                        dt = QueryHistoryFormulaData();
                    else if (this._mode == BrowseMode.HistoryAbs)
                        dt = QueryHistoryAbsData();
                }
                else if (this.rdo_Record_Now.Checked)
                {
                    if (this._mode == BrowseMode.HistoryDrop)
                        dt = QueryHistoryDropData(todayOnly: true);
                    else if (this._mode == BrowseMode.HistoryFormula)
                        dt = QueryHistoryFormulaData(todayOnly: true);
                    else if (this._mode == BrowseMode.HistoryAbs)
                        dt = QueryHistoryAbsData(todayOnly: true);
                }
                else
                {
                    if (this._mode == BrowseMode.HistoryDrop)
                        dt = QueryHistoryDropData(all: true);
                    else if (this._mode == BrowseMode.HistoryFormula)
                        dt = QueryHistoryFormulaData(all: true);
                    else if (this._mode == BrowseMode.HistoryAbs)
                        dt = QueryHistoryAbsData(all: true);
                }

                if(dt == null) return;
              
                if (this._mode == BrowseMode.HistoryDrop)
                    LoadHistoryDropData(dt);
                else if (this._mode == BrowseMode.HistoryFormula)
                    LoadHistoryFormulaData(dt);
                else if (this._mode == BrowseMode.HistoryAbs)
                    LoadHistoryAbsData(dt);
            }
            catch (Exception ex)
            {
                Logger.Error("CtHistoryDyeBrowse.RefreshData异常", ex);
            }
            finally
            {
                _isLoading = false;
            }
        }

        /// <summary>
        /// SQL查询染色记录
        /// </summary>
        private DataTable QueryHistoryDropData(bool todayOnly = false, bool all = false)
        {
            var fields = new[]
            {
                HISTORY_HEAD.BatchName,
                HISTORY_HEAD.FormulaCode,
                HISTORY_HEAD.VersionNum,
                HISTORY_HEAD.CupNum,
                HISTORY_HEAD.DescribeChar,
                HISTORY_HEAD.MyID,
            };

            var whereList = new List<string>();
            var paramList = new List<SqlParameter>();

            if (!all)
            {
                if (this.txt_Record_Operator.Enabled && this.txt_Record_Operator.SelectedItem != null && !string.IsNullOrWhiteSpace(this.txt_Record_Operator.Text))
                {
                    whereList.Add($"{HISTORY_HEAD.Operator} = @operator");
                    paramList.Add(new SqlParameter("@operator", this.txt_Record_Operator.SelectedItem.ToString().Trim()));
                }
                if (!string.IsNullOrWhiteSpace(this.txt_Record_CupNum.Text))
                {
                    whereList.Add($"{HISTORY_HEAD.CupNum} = @cupNum");
                    paramList.Add(new SqlParameter("@cupNum", this.txt_Record_CupNum.Text.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(this.txt_Record_Code.Text))
                {
                    whereList.Add($"{HISTORY_HEAD.FormulaCode} = @formulaCode");
                    paramList.Add(new SqlParameter("@formulaCode", this.txt_Record_Code.Text.Trim()));
                }
                if (todayOnly)
                {
                    whereList.Add($"{HISTORY_HEAD.FinishTime} >= @start AND {HISTORY_HEAD.FinishTime} < @end");
                    paramList.Add(new SqlParameter("@start", DateTime.Today));
                    paramList.Add(new SqlParameter("@end", DateTime.Today.AddDays(1)));
                }
                else if (this.dt_Record_Start.Enabled && this.dt_Record_End.Enabled)
                {
                    whereList.Add($"{HISTORY_HEAD.FinishTime} >= @start AND {HISTORY_HEAD.FinishTime} <= @end");
                    paramList.Add(new SqlParameter("@start", this.dt_Record_Start.Value));
                    paramList.Add(new SqlParameter("@end", this.dt_Record_End.Value));
                }
            }

            string where = whereList.Count > 0 ? string.Join(" AND ", whereList) : null;

            // SQL排序，取需要的字段
            var dt = SqlServer.Select(
                HISTORY_HEAD.TableName,
                fields,
                where,
                HISTORY_HEAD.MyID,
                false,
                paramList.ToArray()
            );



            return dt;
        }

        /// <summary>
        /// SQL查询配方记录
        /// </summary>
        private DataTable QueryHistoryFormulaData(bool todayOnly = false, bool all = false)
        {
            var fields = new[]
            {
                FORMULA_HEAD.FormulaCode,
                FORMULA_HEAD.VersionNum,
                FORMULA_HEAD.MyID,

            };

            var whereList = new List<string>();
            var paramList = new List<SqlParameter>();

            if (!all)
            {
                if (this.txt_Record_Operator.Enabled && this.txt_Record_Operator.SelectedItem != null && !string.IsNullOrWhiteSpace(this.txt_Record_Operator.Text))
                {
                    whereList.Add($"{FORMULA_HEAD.Operator} = @operator");
                    paramList.Add(new SqlParameter("@operator", this.txt_Record_Operator.SelectedItem.ToString().Trim()));
                }
                if (!string.IsNullOrWhiteSpace(this.txt_Record_CupNum.Text))
                {
                    whereList.Add($"{FORMULA_HEAD.CupNum} = @cupNum");
                    paramList.Add(new SqlParameter("@cupNum", this.txt_Record_CupNum.Text.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(this.txt_Record_Code.Text))
                {
                    whereList.Add($"{FORMULA_HEAD.FormulaCode} = @formulaCode");
                    paramList.Add(new SqlParameter("@formulaCode", this.txt_Record_Code.Text.Trim()));
                }
                if (todayOnly)
                {
                    whereList.Add($"{FORMULA_HEAD.CreateTime} >= @start AND {FORMULA_HEAD.CreateTime} < @end");
                    paramList.Add(new SqlParameter("@start", DateTime.Today));
                    paramList.Add(new SqlParameter("@end", DateTime.Today.AddDays(1)));
                }
                else if (this.dt_Record_Start.Enabled && this.dt_Record_End.Enabled)
                {
                    whereList.Add($"{FORMULA_HEAD.CreateTime} >= @start AND {FORMULA_HEAD.CreateTime} <= @end");
                    paramList.Add(new SqlParameter("@start", this.dt_Record_Start.Value));
                    paramList.Add(new SqlParameter("@end", this.dt_Record_End.Value));
                }
            }

            string where = whereList.Count > 0 ? string.Join(" AND ", whereList) : null;

            var dt = SqlServer.Select(
                FORMULA_HEAD.TableName,
                fields,
                where,
                FORMULA_HEAD.MyID,
                false,
                paramList.ToArray()
            );



            return dt;
        }

        /// <summary>
        /// SQL查询吸光度历史
        /// </summary>
        private DataTable QueryHistoryAbsData(bool todayOnly = false, bool all = false)
        {
            var fields = new[]
            {
                ABS_HISTORY_HEAD.BatchName,
                ABS_HISTORY_HEAD.FormulaCode,
                ABS_HISTORY_HEAD.VersionNum,
                ABS_HISTORY_HEAD.CupNum,
                ABS_HISTORY_HEAD.DescribeChar,
                ABS_HISTORY_HEAD.Stand,
                ABS_HISTORY_HEAD.MyID,
            };

            var whereList = new List<string>();
            var paramList = new List<SqlParameter>();

            if (!all)
            {
                if (this.txt_Record_Operator.Enabled && this.txt_Record_Operator.SelectedItem != null && !string.IsNullOrWhiteSpace(this.txt_Record_Operator.Text))
                {
                    whereList.Add($"{ABS_HISTORY_HEAD.Operator} = @operator");
                    paramList.Add(new SqlParameter("@operator", this.txt_Record_Operator.SelectedItem.ToString().Trim()));
                }
                if (!string.IsNullOrWhiteSpace(this.txt_Record_CupNum.Text))
                {
                    whereList.Add($"{ABS_HISTORY_HEAD.CupNum} = @cupNum");
                    paramList.Add(new SqlParameter("@cupNum", this.txt_Record_CupNum.Text.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(this.txt_Record_Code.Text))
                {
                    whereList.Add($"{ABS_HISTORY_HEAD.FormulaCode} = @formulaCode");
                    paramList.Add(new SqlParameter("@formulaCode", this.txt_Record_Code.Text.Trim()));
                }
                if (todayOnly)
                {
                    whereList.Add($"{ABS_HISTORY_HEAD.FinishTime} >= @start AND {ABS_HISTORY_HEAD.FinishTime} < @end");
                    paramList.Add(new SqlParameter("@start", DateTime.Today));
                    paramList.Add(new SqlParameter("@end", DateTime.Today.AddDays(1)));
                }
                else if (this.dt_Record_Start.Enabled && this.dt_Record_End.Enabled)
                {
                    whereList.Add($"{ABS_HISTORY_HEAD.FinishTime} >= @start AND {ABS_HISTORY_HEAD.FinishTime} <= @end");
                    paramList.Add(new SqlParameter("@start", this.dt_Record_Start.Value));
                    paramList.Add(new SqlParameter("@end", this.dt_Record_End.Value));
                }
            }

            string where = whereList.Count > 0 ? string.Join(" AND ", whereList) : null;

            var dt = SqlServer.Select(
                ABS_HISTORY_HEAD.TableName,
                fields,
                where,
                ABS_HISTORY_HEAD.MyID,
                false,
                paramList.ToArray()
            );



            return dt;
        }

        /// <summary>
        /// 加载染色记录数据到控件
        /// </summary>
        private void LoadHistoryDropData(DataTable dt)
        {
            var displayColumns = new[]
            {
                HISTORY_HEAD.BatchName,
                HISTORY_HEAD.FormulaCode,
                HISTORY_HEAD.VersionNum,
                HISTORY_HEAD.CupNum,
                HISTORY_HEAD.DescribeChar,
                HISTORY_HEAD.MyID
            };

            if (dt != null && dt.Rows.Count > 0)
            {
                this.ctRecord1.SetDataSource(dt);
                this.ctRecord1.SetColumnHeaders(new Dictionary<string, string>
                {
                    { HISTORY_HEAD.BatchName, "批次号" },
                    { HISTORY_HEAD.FormulaCode, "配方代码" },
                    { HISTORY_HEAD.VersionNum, "版本号" },
                    { HISTORY_HEAD.CupNum, "杯号" },
                    { HISTORY_HEAD.DescribeChar, "描述" },
                    { HISTORY_HEAD.MyID, "ID" }
                });

                var showColumns = new[]
                {
                    HISTORY_HEAD.BatchName,
                    HISTORY_HEAD.FormulaCode,
                    HISTORY_HEAD.VersionNum,
                    HISTORY_HEAD.CupNum
                };
                this.ShowColumns(showColumns);
                this.UpdateSuccessRateSql();
                Logger.Info("染色记录数据已加载到控件。");
            }
            else
            {
                this.ctRecord1.SetDataSource(null);
                this.txt_R.Text = "";
                Logger.Info("染色记录数据为空。");
            }
        }

        /// <summary>
        /// 加载配方记录数据到控件
        /// </summary>
        private void LoadHistoryFormulaData(DataTable dt)
        {
            var displayColumns = new[] { FORMULA_HEAD.FormulaCode, FORMULA_HEAD.VersionNum, FORMULA_HEAD.MyID };

            if (dt != null && dt.Rows.Count > 0)
            {
                this.ctRecord1.SetDataSource(dt);
                this.ctRecord1.SetColumnHeaders(new Dictionary<string, string>
                {
                    { FORMULA_HEAD.FormulaCode, "配方代码" },
                    { FORMULA_HEAD.VersionNum, "版本号" },
                    { FORMULA_HEAD.MyID, "ID" }
                });

                this.ShowColumns(displayColumns);
                Logger.Info("配方记录数据已加载到控件。");
            }
            else
            {
                this.ctRecord1.SetDataSource(null);
                Logger.Info("配方记录数据为空。");
            }
        }

        /// <summary>
        /// 加载吸光度历史数据到控件
        /// </summary>
        private void LoadHistoryAbsData(DataTable dt)
        {
            var displayColumns = new[]
            {
                ABS_HISTORY_HEAD.BatchName,
                ABS_HISTORY_HEAD.FormulaCode,
                ABS_HISTORY_HEAD.VersionNum,
                ABS_HISTORY_HEAD.CupNum,
                ABS_HISTORY_HEAD.DescribeChar,
                ABS_HISTORY_HEAD.Stand,
                ABS_HISTORY_HEAD.MyID
            };

            if (dt != null && dt.Rows.Count > 0)
            {
                this.ctRecord1.SetDataSource(dt);
                this.ctRecord1.SetColumnHeaders(new Dictionary<string, string>
                {
                    { ABS_HISTORY_HEAD.BatchName, "批次号" },
                    { ABS_HISTORY_HEAD.FormulaCode, "配方代码" },
                    { ABS_HISTORY_HEAD.VersionNum, "版本号" },
                    { ABS_HISTORY_HEAD.CupNum, "杯号" },
                    { ABS_HISTORY_HEAD.DescribeChar, "描述" },
                    { ABS_HISTORY_HEAD.Stand, "标样" },
                    { ABS_HISTORY_HEAD.MyID, "ID" }
                });

                var showColumns = new[]
                {
                    ABS_HISTORY_HEAD.BatchName,
                    ABS_HISTORY_HEAD.FormulaCode,
                    ABS_HISTORY_HEAD.VersionNum,
                    ABS_HISTORY_HEAD.CupNum,
                };
                this.UpdateSuccessRateSql();
                this.ShowColumns(showColumns);
                Logger.Info("吸光度历史数据已加载到控件。");
            }
            else
            {
                this.ctRecord1.SetDataSource(null);
                Logger.Info("吸光度历史数据为空。");
            }
        }

        private void ShowColumns(string[] columnNames)
        {
            var dgv = this.ctRecord1.dgv;
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.Visible = columnNames.Contains(col.Name);
            }
            dgv.AutoFitAllColumns();
        }


        private void Dgv_RowPrePaint_Custom(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var dgv = ctRecord1.dgv;
            var row = dgv.Rows[e.RowIndex];
            var dataBound = row.DataBoundItem as DataRowView;
            if (dataBound == null) return;

            string describe = null;
            object standValue = null;

            if (_mode == BrowseMode.HistoryDrop)
            {
                if (dataBound.Row.Table.Columns.Contains(ABS_HISTORY_HEAD.DescribeChar))
                    describe = dataBound.Row[ABS_HISTORY_HEAD.DescribeChar]?.ToString();
                if (dataBound.Row.Table.Columns.Contains(ABS_HISTORY_HEAD.Stand))
                    standValue = dataBound.Row[ABS_HISTORY_HEAD.Stand];
            }
            else if (_mode == BrowseMode.HistoryAbs)
            {
                if (dataBound.Row.Table.Columns.Contains(HISTORY_HEAD.DescribeChar))
                    describe = dataBound.Row[HISTORY_HEAD.DescribeChar]?.ToString();
                if (dataBound.Row.Table.Columns.Contains("Stand"))
                    standValue = dataBound.Row["Stand"];
            }

            if (standValue != null && standValue != DBNull.Value && standValue.ToString() == "1")
            {
                row.DefaultCellStyle.BackColor = Color.LightGreen;
            }
            else if (!string.IsNullOrEmpty(describe) && describe.Contains("失败"))
            {
                row.DefaultCellStyle.BackColor = Color.LightCoral;
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.White;
            }
        }

        /// <summary>
        /// 用SQL统计成功率（仅染色记录用）
        /// 支持所有筛选条件，统计“滴液失败”
        /// </summary>
        private void UpdateSuccessRateSql()
        {
            txt_R.Text = "统计中...";

            // 判断当前模式
            string tableName, describeField, operatorField, cupNumField, formulaCodeField, finishedTimeField;
            string failKeyword;
            if (_mode == BrowseMode.HistoryDrop)
            {
                tableName = HISTORY_HEAD.TableName;
                describeField = HISTORY_HEAD.DescribeChar;
                operatorField = HISTORY_HEAD.Operator;
                cupNumField = HISTORY_HEAD.CupNum;
                formulaCodeField = HISTORY_HEAD.FormulaCode;
                finishedTimeField = HISTORY_HEAD.FinishTime;
                failKeyword = "滴液失败";
            }
            else if (_mode == BrowseMode.HistoryAbs)
            {
                tableName = ABS_HISTORY_HEAD.TableName;
                describeField = ABS_HISTORY_HEAD.DescribeChar;
                operatorField = ABS_HISTORY_HEAD.Operator;
                cupNumField = ABS_HISTORY_HEAD.CupNum;
                formulaCodeField = ABS_HISTORY_HEAD.FormulaCode;
                finishedTimeField = ABS_HISTORY_HEAD.FinishTime;
                failKeyword = "滴液失败"; // 如有不同请调整
            }
            else
            {
                txt_R.Text = "";
                return;
            }

            var whereList = new List<string>();
            var paramList = new List<SqlParameter>();

            if (this.txt_Record_Operator.Enabled && this.txt_Record_Operator.SelectedItem != null && !string.IsNullOrWhiteSpace(this.txt_Record_Operator.Text))
            {
                whereList.Add($"{operatorField} = @operator");
                paramList.Add(new SqlParameter("@operator", this.txt_Record_Operator.SelectedItem.ToString().Trim()));
            }
            if (this.txt_Record_CupNum.Enabled && !string.IsNullOrWhiteSpace(this.txt_Record_CupNum.Text))
            {
                whereList.Add($"{cupNumField} = @cupNum");
                paramList.Add(new SqlParameter("@cupNum", this.txt_Record_CupNum.Text.Trim()));
            }
            if (this.txt_Record_Code.Enabled && !string.IsNullOrWhiteSpace(this.txt_Record_Code.Text))
            {
                whereList.Add($"{formulaCodeField} = @formulaCode");
                paramList.Add(new SqlParameter("@formulaCode", this.txt_Record_Code.Text.Trim()));
            }
            // 时间筛选
            if (this.rdo_Record_Now.Checked)
            {
                whereList.Add($"{finishedTimeField} >= @start AND {finishedTimeField} < @end");
                paramList.Add(new SqlParameter("@start", DateTime.Today));
                paramList.Add(new SqlParameter("@end", DateTime.Today.AddDays(1)));
            }
            else if (this.dt_Record_Start.Enabled && this.dt_Record_End.Enabled && this.rdo_Record_condition.Checked)
            {
                whereList.Add($"{finishedTimeField} >= @start AND {finishedTimeField} <= @end");
                paramList.Add(new SqlParameter("@start", this.dt_Record_Start.Value));
                paramList.Add(new SqlParameter("@end", this.dt_Record_End.Value));
            }

            string where = whereList.Count > 0 ? "WHERE " + string.Join(" AND ", whereList) : "";

            // SQL统计总数和失败数
            string sql = $@"
                SELECT 
                    COUNT(*) AS Total,
                    SUM(CASE WHEN {describeField} LIKE '%{failKeyword}%' THEN 1 ELSE 0 END) AS Fail
                FROM {tableName}
                {where}
            ";

            Task.Run(() =>
            {
                try
                {
                    var dt = SqlServer.ExecuteQuery(sql, paramList.ToArray());
                    int total = 0, fail = 0;
                    if (dt.Rows.Count > 0)
                    {
                        total = dt.Rows[0]["Total"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["Total"]) : 0;
                        fail = dt.Rows[0]["Fail"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["Fail"]) : 0;
                    }
                    int success = total - fail;
                    string rate = total > 0 ? $"{(success * 100.0 / total):F1}%" : "0%";
                    string result = $"成功率：{rate}（成功{success}，失败{fail}，总{total}）";
                    while (true)
                    {
                        if (this.IsHandleCreated && !this.IsDisposed)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                this.txt_R.Text = result;
                                this.txt_R.Refresh();
                            }));
                            break;
                        }
                        else
                        {
                            // 如果控件尚未创建或已销毁，等待一段时间后重试，避免异常
                           // Task.Delay(100).Wait();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                            this.BeginInvoke(new Action(() =>
                        {
                            this.txt_R.Text = "";
                            this.txt_R.Refresh();
                        }));
                    }
                    Logger.Error("UpdateSuccessRateSql异常", ex);
                }
            });
        }

        private void Btn_Record_Select_Click(object sender, EventArgs e)
        {
            Logger.Info("用户点击查询按钮。");
            RefreshData();
        }

        private void Btn_Record_Delete_Click(object sender, EventArgs e)
        {
            if (CurrentRow == null)
            {
                LocalTranslator.ShowMessage("请选择要删除的记录!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Logger.Info("删除操作失败：未选中行。");
                return;
            }
            var result = LocalTranslator.ShowMessage("确认删除此记录吗?", "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    string keyField;
                    object keyValue = null;
                    if (this._mode == BrowseMode.HistoryDrop)
                    {
                        keyField = HISTORY_HEAD.MyID;
                        keyValue = CurrentRow[keyField];
                        object code = CurrentRow[HISTORY_HEAD.FormulaCode];
                        object version = CurrentRow[HISTORY_HEAD.VersionNum];
                        object cupNum = CurrentRow[HISTORY_HEAD.CupNum];
                        object batchName = CurrentRow[HISTORY_HEAD.BatchName];

                        My_DataBase.SqlServer.Delete(HISTORY_HEAD.TableName, $"{keyField}=@id", new SqlParameter("@id", keyValue));
                        My_DataBase.SqlServer.Delete(HISTORY_DETAILS.TableName, $"{HISTORY_DETAILS.FormulaCode}=@code AND {HISTORY_DETAILS.VersionNum} = @ver AND {HISTORY_HEAD.CupNum} = @cup AND {HISTORY_HEAD.BatchName} = @bat ",
                            new SqlParameter("@code", code),
                            new SqlParameter("@ver", version),
                            new SqlParameter("@cup", cupNum),
                            new SqlParameter("@bat", batchName)
                        );
                        My_DataBase.SqlServer.Delete(HISTORY_DYE.TableName,
                            $"{HISTORY_DYE.FormulaCode}=@code AND {HISTORY_DYE.VersionNum} = @ver AND {HISTORY_DYE.CupNum} = @cup AND {HISTORY_DYE.BatchName} = @bat ",
                            new SqlParameter("@code", code),
                            new SqlParameter("@ver", version),
                            new SqlParameter("@cup", cupNum),
                            new SqlParameter("@bat", batchName)
                        );
                        
                        Logger.Info($"删除染色记录成功，主表ID={keyValue}，相关明细已同步删除。");
                    }
                    else if (this._mode == BrowseMode.HistoryFormula)
                    {
                        keyField = FORMULA_HEAD.MyID;
                        keyValue = CurrentRow[keyField];
                        object code = CurrentRow[FORMULA_HEAD.FormulaCode];
                        object version = CurrentRow[FORMULA_HEAD.VersionNum];

                        My_DataBase.SqlServer.Delete(FORMULA_HEAD.TableName, $"{keyField}=@id", new SqlParameter("@id", keyValue));
                        My_DataBase.SqlServer.Delete(FORMULA_DETAILS.TableName, $"{FORMULA_DETAILS.FormulaCode}=@code AND {FORMULA_HEAD.VersionNum} = @ver",
                            new SqlParameter("@code", code),
                            new SqlParameter("@ver", version));
                        My_DataBase.SqlServer.Delete(FORMULA_HANDLE_DETAILS.TableName, $"{FORMULA_HANDLE_DETAILS.FormulaCode}=@code AND {FORMULA_HANDLE_DETAILS.VersionNum} = @ver",
                            new SqlParameter("@code", code),
                            new SqlParameter("@ver", version));
                       
                        Logger.Info($"删除配方记录成功，主表ID={keyValue}，相关明细已同步删除。");
                    }
                    else if (this._mode == BrowseMode.HistoryAbs)
                    {
                        keyField = ABS_HISTORY_HEAD.MyID;
                        keyValue = CurrentRow[keyField];
                        object code = CurrentRow[ABS_HISTORY_HEAD.FormulaCode];
                        object version = CurrentRow[ABS_HISTORY_HEAD.VersionNum];
                        object cupNum = CurrentRow[ABS_HISTORY_HEAD.CupNum];
                        object batchName = CurrentRow[ABS_HISTORY_HEAD.BatchName];

                        My_DataBase.SqlServer.Delete(ABS_HISTORY_HEAD.TableName, $"{keyField}=@id", new SqlParameter("@id", keyValue));
                        My_DataBase.SqlServer.Delete(ABS_HISTORY_DETAILS.TableName, $"{ABS_HISTORY_DETAILS.FormulaCode}=@code AND {ABS_HISTORY_DETAILS.VersionNum} = @ver AND {ABS_HISTORY_DETAILS.CupNum} = @cup AND {ABS_HISTORY_DETAILS.BatchName} = @bat ",
                           new SqlParameter("@code", code),
                           new SqlParameter("@ver", version),
                           new SqlParameter("@cup", cupNum),
                           new SqlParameter("@bat", batchName)
                       );
                       
                        Logger.Info($"删除吸光度历史记录成功，主表ID={keyValue}。");
                    }

                    LocalTranslator.ShowMessage("删除成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshData();
                }
                catch (Exception ex)
                {
                    Logger.Error("删除记录异常", ex);
                }
            }
        }

        private void Btn_Record_Print_Click(object sender, EventArgs e)
        {
            LocalTranslator.ShowMessage("打印功能待实现。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Logger.Info("用户点击打印按钮。");
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (this._isLoading || this.ctRecord1.SuppressCurrentCellChanged) return;
            var dgv = this.ctRecord1.dgv;
            if (dgv.CurrentRow != null && dgv.CurrentRow.Index >= 0 && dgv.CurrentRow.Index < dgv.Rows.Count)
            {
                var rowView = dgv.CurrentRow.DataBoundItem as DataRowView;
                CurrentRow = rowView?.Row;
                CurrentRowChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                CurrentRow = null;
                CurrentRowChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public object GetCurrentField(string columnName)
        {
            return CurrentRow != null && CurrentRow.Table.Columns.Contains(columnName)
                ? CurrentRow[columnName]
                : null;
        }

        public void SetMultiSelect(bool multiSelect)
        {
            ctRecord1.dgv.MultiSelect = multiSelect;
        }

        public void ExportToExcel(string filePath)
        {
            var dt = ctRecord1.dgv.DataSource as BindingSource;
            if (dt?.DataSource is DataTable table)
            {
                // 这里只做简单导出示例，实际可用NPOI等库
                // ...导出逻辑
            }
        }

        public void Clear()
        {
            ctRecord1.SetDataSource((DataTable)null);
        }

        public void Reload()
        {
            RefreshData();
        }

        private void MiSpectrometer_Click(object sender, EventArgs e)
        {
            if (ctRecord1.dgv.CurrentCell == null)
            {
                My_File.LocalTranslator.ShowMessage("请先选择一行数据！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var row = ctRecord1.dgv.CurrentRow;
            if (row != null)
            {
                var formulaCode = row.Cells[1].Value?.ToString() ?? "";
                var versionNum = row.Cells[2].Value?.ToString() ?? "";
                var id = row.Cells[5].Value?.ToString() ?? "";
                using (var fmr = new SmartColor.My_Form.Spectrometer.Spectrometer(formulaCode, versionNum, int.Parse(id)))
                {
                    fmr.ShowDialog();
                }
            }
            else
            {
                My_File.LocalTranslator.ShowMessage("请先选择一行数据！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}