using SmartColor.My_AutomaticModule;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Form.DyeingMan;
using SmartColor.My_Form.HistoricalData;
using SmartColor.My_Form.Homepage;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 批次数据控件，仅负责批次数据的展示、删除、杯号修改等UI操作。
    /// 所有批次队列、自动滴液、批次号生成等业务逻辑已迁移到后台静态类（DropBatchManager/CupAuxiliary）。
    /// </summary>
    public partial class CtBatchData : UserControl
    {
        /// <summary>
        /// 数据表名参数化，便于不同页面复用
        /// </summary>
        public string TableHeadName { get; set; }

        /// <summary>
        /// 目标头部控件（用于数据联动填充）
        /// </summary>
        public CtDropHead HeadTarget { get; set; }

        /// <summary>
        /// 批次表当前行改变事件
        /// </summary>
        public event EventHandler<DataTable> CurrentRowChanged;

        /// <summary>
        /// 头部数据缓存
        /// </summary>
        private DataTable _head = null;

        // 节流刷新相关
        private bool _pendingRefresh = false;
        private Timer _refreshTimer;

        public CtBatchData()
        {
            InitializeComponent();

            // 初始化节流Timer
            _refreshTimer = new Timer();
            _refreshTimer.Interval = 100; // 100ms内合并多次刷新
            _refreshTimer.Tick += _refreshTimer_Tick;


            // 绑定表格当前行变化事件
            ctDataGridView1.CurrentCellChanged += CtDataGridView1_CurrentCellChanged;

            // 订阅滴液完成事件（用于刷新UI）
            SmartColor.My_AutomaticModule.DropRobotTask.CupFinished += OnBatchRelatedEvent;
            SmartColor.My_Tool.CupAuxiliary.CupFinished += OnBatchRelatedEvent;
            SmartColor.My_AutomaticModule.DropBatchManager.BatchChanged += OnBatchRelatedEvent;
        }

        private void _refreshTimer_Tick(object sender, EventArgs e)
        {

            _refreshTimer.Stop();
            _pendingRefresh = false;
            DoRefreshData(0);

        }

        private void OnBatchRelatedEvent()
        {
            OnBatchRelatedEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// 合并批次相关事件，节流刷新
        /// </summary>
        private void OnBatchRelatedEvent(object sender, EventArgs e)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => OnBatchRelatedEvent(sender, e)));
                return;
            }
            if (!_pendingRefresh)
            {
                _pendingRefresh = true;
                _refreshTimer.Stop();
                _refreshTimer.Start();
            }
        }

        // 适配 CupFinished(int cupNum) 事件
        public void OnBatchRelatedEvent(int cupNum)
        {
            OnBatchRelatedEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// DataGridView当前行变化时触发，联动填充头部控件
        /// </summary>
        private void CtDataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            var row = ctDataGridView1.CurrentRow;
            DataTable absTable = null;
            if (row != null && _head != null)
            {
                // 获取当前行的唯一ID
                var id = Convert.ToInt32(row.Cells["Column3"].Value);

                // 根据表名选择ID字段
                string fieldName = TableHeadName == My_DataBase.DROP_HEAD.TableName
                    ? My_DataBase.DROP_HEAD.MyID
                    : My_DataBase.ABS_DROP_HEAD.MyID;

                // 查找对应的头部数据行
                var headRows = _head.AsEnumerable()
                    .Where(r => Convert.ToInt32(r[fieldName]) == id);

                // 若有数据则填充，否则填充空表结构
                DataTable headTable = headRows.Any() ? headRows.CopyToDataTable() : _head.Clone();

                // 联动填充头部控件
                HeadTarget?.FillControlsFromDataTable(headTable, CtDropHead.DataSource.Batch);

                //如果是ABS页面，填充曲线
                if (TableHeadName == My_DataBase.ABS_DROP_HEAD.TableName)
                {
                    var formulaCode = row.Cells["Column2"].Value?.ToString();

                    // 标样
                    var absS = SqlServer.Select(My_DataBase.ABS_HISTORY_HEAD.TableName,
                        $"{My_DataBase.ABS_HISTORY_HEAD.FormulaCode} = @FormulaCode AND {My_DataBase.ABS_HISTORY_HEAD.Stand} = 1",
                        new System.Data.SqlClient.SqlParameter("@FormulaCode", formulaCode));

                    // 试样
                    var absR = SqlServer.Select(My_DataBase.ABS_DROP_HEAD.TableName, $"{My_DataBase.ABS_DROP_HEAD.MyID} = @ID",
                        new System.Data.SqlClient.SqlParameter("@ID", id)
                    );

                    // 合并到同一个 DataTable
                    var firstS = absS?.AsEnumerable().FirstOrDefault();
                    var firstR = absR?.AsEnumerable().FirstOrDefault();

                    if (firstS != null || firstR != null)
                    {
                        // 以标样结构为模板
                        absTable = absS.Clone();
                        var abs = firstR[My_DataBase.ABS_HISTORY_HEAD.Abs];
                        // 标样直接导入
                        if (firstS != null)
                            absTable.ImportRow(firstS);

                        // 试样只取 Abs 字段，其他字段用标样或默认值
                        if (firstR != null && abs != DBNull.Value)
                        {
                            DataRow newRow = absTable.NewRow();
                            // 如果有标样，复制其结构
                            if (firstS != null)
                            {
                                foreach (DataColumn col in absTable.Columns)
                                {
                                    if (col.ColumnName == My_DataBase.ABS_HISTORY_HEAD.Abs)
                                        newRow[col.ColumnName] = firstR[My_DataBase.ABS_DROP_HEAD.Abs];
                                    else if (col.ColumnName == My_DataBase.ABS_HISTORY_HEAD.Stand)
                                        newRow[col.ColumnName] = 0;
                                    else
                                        newRow[col.ColumnName] = firstS[col.ColumnName];
                                }
                            }
                            else
                            {
                                // 没有标样时，试样字段自己填充
                                foreach (DataColumn col in absTable.Columns)
                                {
                                    if (col.ColumnName == My_DataBase.ABS_HISTORY_HEAD.Abs)
                                        newRow[col.ColumnName] = firstR[My_DataBase.ABS_DROP_HEAD.Abs];
                                    else if (col.ColumnName == My_DataBase.ABS_HISTORY_HEAD.Stand)
                                        newRow[col.ColumnName] = 0;
                                    else
                                        newRow[col.ColumnName] = DBNull.Value;
                                }
                            }
                            absTable.Rows.Add(newRow);
                        }
                    }
                }
            }
            // 触发外部事件
            CurrentRowChanged?.Invoke(this, absTable);
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
        /// 刷新批次表数据，并高亮有批次名的行
        /// </summary>
        /// <param name="cup">需要选中的杯号（0为不选中）</param>
        private void RefreshData(int cup)
        {
            // 只负责UI刷新，不做数据获取
            if (this.IsDisposed)
                return;

            // 如果还没创建句柄，等创建后再自适应
            if (!this.IsHandleCreated)
            {
                void handler(object s, EventArgs e)
                {
                    this.HandleCreated -= handler;
                    this.BeginInvoke(new Action(() => RefreshData(cup)));
                }
                this.HandleCreated += handler;
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => RefreshData(cup)));
                return;
            }

            try
            {
                ctDataGridView1.CurrentCellChanged -= CtDataGridView1_CurrentCellChanged;
                // 确保列存在
                if (ctDataGridView1.Columns.Count == 0)
                {
                    ctDataGridView1.Columns.AddRange(
                        new DataGridViewTextBoxColumn { Name = "Column1", HeaderText = "杯号", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewTextBoxColumn { Name = "Column2", HeaderText = "配方代码", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewTextBoxColumn { Name = "Column3", HeaderText = "序号", ReadOnly = true, SortMode = DataGridViewColumnSortMode.NotSortable, Visible = false },
                        new DataGridViewTextBoxColumn { Name = "Column4", HeaderText = "批次号", ReadOnly = true, SortMode = DataGridViewColumnSortMode.NotSortable, Visible = false }
                    );
                }

                if (_head != null)
                {
                    // 动态获取列名，支持不同表结构
                    string cupNumN, formulaCodeN, myIDN, batchNameN, descN;
                    if (TableHeadName == My_DataBase.DROP_HEAD.TableName)
                    {
                        cupNumN = My_DataBase.DROP_HEAD.CupNum;
                        formulaCodeN = My_DataBase.DROP_HEAD.FormulaCode;
                        myIDN = My_DataBase.DROP_HEAD.MyID;
                        batchNameN = My_DataBase.DROP_HEAD.BatchName;
                        descN = My_DataBase.DROP_HEAD.DescribeChar;
                    }
                    else
                    {
                        cupNumN = My_DataBase.ABS_DROP_HEAD.CupNum;
                        formulaCodeN = My_DataBase.ABS_DROP_HEAD.FormulaCode;
                        myIDN = My_DataBase.ABS_DROP_HEAD.MyID;
                        batchNameN = My_DataBase.ABS_DROP_HEAD.BatchName;
                        descN = My_DataBase.ABS_DROP_HEAD.DescribeChar;
                    }

                    // 清空并绑定新数据
                    ctDataGridView1.Rows.Clear();
                    ctDataGridView1.BindDataTable(
                        _head,
                        r => new object[] { r[cupNumN], r[formulaCodeN], r[myIDN], r[batchNameN], r.Table.Columns.Contains(descN) ? r[descN] : "" },
                        cupNumN
                    );

                    // 行背景色处理
                    foreach (DataGridViewRow row in ctDataGridView1.Rows)
                    {
                        var batchName = row.Cells["Column4"].Value?.ToString();
                        string desc = null;

                        // 通过唯一ID去_head查找对应DataRow
                        if (row.Cells["Column3"].Value != null)
                        {
                            int id = Convert.ToInt32(row.Cells["Column3"].Value);
                            var dataRow = _head.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r[myIDN]) == id);
                            if (dataRow != null && dataRow.Table.Columns.Contains(descN))
                            {
                                desc = dataRow[descN]?.ToString();
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(desc))
                        {
                            if (desc.Contains("滴液成功"))
                            {
                                row.DefaultCellStyle.BackColor = Color.LimeGreen;
                            }
                            else if (desc.Contains("滴液失败"))
                            {
                                row.DefaultCellStyle.BackColor = Color.Red;
                            }
                            else if (!string.IsNullOrWhiteSpace(batchName))
                            {
                                row.DefaultCellStyle.BackColor = Color.LightGray;
                            }
                            else
                            {
                                row.DefaultCellStyle.BackColor = Color.White;
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(batchName))
                        {
                            row.DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        else
                        {
                            row.DefaultCellStyle.BackColor = Color.White;
                        }
                    }

                    // 自动适应列宽
                    ctDataGridView1.AutoFitAllColumns();

                    // 选中指定杯号行
                    if (cup == 0)
                        ctDataGridView1.ClearSelection();
                    else
                    {
                        foreach (DataGridViewRow row in ctDataGridView1.Rows)
                        {
                            if (Convert.ToInt32(row.Cells["Column1"].Value) == cup)
                            {
                                ctDataGridView1.CurrentCell = row.Cells[0];
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RefreshData: 刷新批次表数据异常。", ex);
            }

            ctDataGridView1.CurrentCell = null;
            ctDataGridView1.ClearSelection();
            ctDataGridView1.CurrentCellChanged += CtDataGridView1_CurrentCellChanged;
        }

        /// <summary>
        /// 数据获取+UI刷新（合并入口）
        /// </summary>
        private void DoRefreshData(int cup)
        {
            _head = GetHeadData();
            RefreshData(cup);
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        private DataTable GetHeadData()
        {
            if (TableHeadName == My_DataBase.DROP_HEAD.TableName)
            {
                return My_DataBase.DropBatchData.GetHeadData();
            }
            else if (TableHeadName == My_DataBase.ABS_DROP_HEAD.TableName)
            {
                return My_DataBase.ABSBatchData.GetHeadData();
            }
            Logger.Error($"CtBatchData.GetData: 未知的数据表名 {TableHeadName}。");
            return null;
        }

        public void SetTableName(string tableName)
        {
            TableHeadName = tableName;
            DoRefreshData(0);
        }

        /// <summary>
        /// 删除选中的批次数据（仅无批次名的可删），并刷新表格
        /// </summary>
        private void Tsmi_Delete_Click(object sender, EventArgs e)
        {
            if (ctDataGridView1.CurrentRow == null)
                return;

            // 二次确认
            DialogResult result = My_File.LocalTranslator.ShowMessage("确定要删除选中的批次数据吗？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;

            // 遍历选中行，删除无批次名的数据
            foreach (DataGridViewRow row in ctDataGridView1.SelectedRows)
            {
                int id = Convert.ToInt32(row.Cells["Column3"].Value);

                var batch = _head.AsEnumerable()
                    .Where(r => Convert.ToInt32(r[My_DataBase.DROP_HEAD.MyID]) == id)
                    .Select(r => r[My_DataBase.DROP_HEAD.BatchName]?.ToString())
                    .FirstOrDefault();

                // 仅删除无批次名的数据
                if (string.IsNullOrEmpty(batch))
                {
                    if (TableHeadName == My_DataBase.DROP_HEAD.TableName)
                    {
                        My_DataBase.SqlServer.Delete(My_DataBase.DROP_HEAD.TableName, $"{My_DataBase.DROP_HEAD.MyID} = @MyID",
                            new System.Data.SqlClient.SqlParameter("@MyID", id));
                        My_DataBase.SqlServer.Delete(My_DataBase.DROP_DETAILS.TableName, $"{My_DataBase.DROP_DETAILS.HeadID} = @MyID",
                            new System.Data.SqlClient.SqlParameter("@MyID", id));
                        My_DataBase.SqlServer.Delete(My_DataBase.DYE_DETAILS.TableName, $"{My_DataBase.DYE_DETAILS.HeadID} = @MyID",
                            new System.Data.SqlClient.SqlParameter("@MyID", id));
                    }
                    else if (TableHeadName == My_DataBase.ABS_DROP_HEAD.TableName)
                    {
                        My_DataBase.SqlServer.Delete(My_DataBase.ABS_DROP_HEAD.TableName, $"{My_DataBase.ABS_DROP_HEAD.MyID} = @MyID",
                           new System.Data.SqlClient.SqlParameter("@MyID", id));
                        My_DataBase.SqlServer.Delete(My_DataBase.ABS_DROP_DETAILS.TableName, $"{My_DataBase.ABS_DROP_DETAILS.HeadID} = @MyID",
                            new System.Data.SqlClient.SqlParameter("@MyID", id));
                    }
                }
            }

            // 刷新数据
            DoRefreshData(0);
        }

        /// <summary>
        /// 修改选中行的杯号，弹窗输入新杯号后刷新表格
        /// </summary>
        private void Tsmi_CupChange_Click(object sender, EventArgs e)
        {
            if (ctDataGridView1.CurrentRow == null)
                return;

            var cupNum = Convert.ToInt32(ctDataGridView1.CurrentRow.Cells["Column1"].Value);
            var id = Convert.ToInt32(ctDataGridView1.CurrentRow.Cells["Column3"].Value);

            // 构造杯号修改信息
            var cupInfo = new My_Form.DyeingMan.CupChange.CupChangeInfo
            {
                OldCupNo = cupNum.ToString(),
                MyID = id,
                Type = CupChange.CupChangeType.DropBatch
            };

            // 弹窗修改杯号
            var dlg = new CupChange(cupInfo);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                int newCupNum = dlg.NewCupNum;
                DoRefreshData(newCupNum);
            }
        }

        /// <summary>
        /// 启动滴液按钮点击事件。所有批次队列、批次号生成、自动滴液等业务逻辑已迁移到后台。
        /// 此处只需调用后台接口即可。
        /// </summary>
        private void Btn_Start_Click(object sender, EventArgs e)
        {
            try
            {
                var dt = DateTime.Now;
                _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                {
                    [SmartColor.My_DataBase.RUN_TABLE.Machine] = "点击开始滴液按钮"
                }, dt);

                // 获取选中行ID
                var selectedIds = ctDataGridView1.SelectedRows
                    .Cast<DataGridViewRow>()
                    .Select(row => Convert.ToInt32(row.Cells["Column3"].Value))
                    .ToList();

                if (selectedIds.Count > 0)
                {
                    SmartColor.My_AutomaticModule.DropBatchManager.RequestBatchStartByIds(selectedIds);
                }
                else
                {
                    // 兼容原有逻辑：无选中则全部
                    SmartColor.My_AutomaticModule.DropBatchManager.RequestBatchStart();
                }

                // 刷新数据
                DoRefreshData(0);
            }
            catch (Exception ex)
            {
                Logger.Error("Btn_Start_Click异常", ex);
            }
        }

        /// <summary>
        /// 停止滴液按钮点击事件。可根据实际需求调用后台接口。
        /// </summary>
        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                var dt = DateTime.Now;
                _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                {
                    [SmartColor.My_DataBase.RUN_TABLE.Machine] = "点击停止滴液按钮"
                }, dt);

                bool hasActiveTask = false;
                Dictionary<int, int> toDeleteHeadIds = new Dictionary<int, int>();

                // 获取杯详情表
                var cupTable = My_DataBase.CupData.GetData();

                foreach (DataGridViewRow row in ctDataGridView1.Rows)
                {
                    int cupNum = Convert.ToInt32(row.Cells["Column1"].Value);
                    var cupRow = cupTable?.AsEnumerable()
                        .FirstOrDefault(r => Convert.ToInt32(r[My_DataBase.CUP_DETAILS.CupNum]) == cupNum);

                    string state = cupRow?[My_DataBase.CUP_DETAILS.Statues]?.ToString();
                    if (row.DefaultCellStyle.BackColor == Color.LightGray)
                    {
                        if (DropBatchManager.CurrentDroppingBatchNo != null)
                        {
                            hasActiveTask = true;
                        }
                        else
                        {
                            int id = Convert.ToInt32(row.Cells["Column3"].Value);
                            int cupNo = cupNum;
                            toDeleteHeadIds.Add(id, cupNo);
                        }
                    }
                    else
                    {
                        if (state == "待机" && row.Cells["Column3"].Value != null)
                        {
                            int id = Convert.ToInt32(row.Cells["Column3"].Value);
                            int cupNo = cupNum;
                            toDeleteHeadIds.Add(id, cupNo);
                        }
                    }
                }

                // 删除不在滴液状态的批次数据
                foreach (var dh in toDeleteHeadIds)
                {
                    My_DataBase.SqlServer.Delete(My_DataBase.DROP_HEAD.TableName, $"{My_DataBase.DROP_HEAD.MyID} = @MyID",
                        new System.Data.SqlClient.SqlParameter("@MyID", dh.Key));
                    My_DataBase.SqlServer.Delete(My_DataBase.DROP_DETAILS.TableName, $"{My_DataBase.DROP_DETAILS.HeadID} = @MyID",
                        new System.Data.SqlClient.SqlParameter("@MyID", dh.Key));
                    My_DataBase.SqlServer.Delete(My_DataBase.DYE_DETAILS.TableName, $"{My_DataBase.DYE_DETAILS.HeadID} = @MyID",
                        new System.Data.SqlClient.SqlParameter("@MyID", dh.Key));

                    var updateDict = new Dictionary<string, object>
                        {
                            { CUP_DETAILS.Statues, "" },
                            { CUP_DETAILS.Enable,1 },
                            { CUP_DETAILS.IsUsing,0 },
                            { CUP_DETAILS.CurrentWeight, 0 },
                            { CUP_DETAILS.TotalWeight,0 },
                            { CUP_DETAILS.SetTime,0 },
                            { CUP_DETAILS.DyeType, DBNull.Value },
                            { CUP_DETAILS.StartTime, DBNull.Value },
                            { CUP_DETAILS.StepStartTime, DBNull.Value },
                            { CUP_DETAILS.FormulaCode, DBNull.Value },
                            { CUP_DETAILS.DyeingCode, DBNull.Value },
                            { CUP_DETAILS.StepNum, 0 },
                            { CUP_DETAILS.TotalStep, 0 },
                            { CUP_DETAILS.TechnologyName, DBNull.Value },
                            { CUP_DETAILS.SetTemp, DBNull.Value },
                            { CUP_DETAILS.RecordIndex, 0 },
                            { CUP_DETAILS.Cooperate, 0 },
                            { CUP_DETAILS.Fail, 0 },
                            { CUP_DETAILS.HeadID, DBNull.Value },
                            { CUP_DETAILS.CurrentStepFinish,0 }
                        };
                    //修改杯状态为待机
                    My_DataBase.SqlServer.Update(My_DataBase.CUP_DETAILS.TableName,
                      updateDict,
                        $"{My_DataBase.CUP_DETAILS.CupNum} = @CupNum AND {My_DataBase.CUP_DETAILS.Type} = 2",
                        new System.Data.SqlClient.SqlParameter("@CupNum", dh.Value));

                    var area = CupCommManager.Instance.FindCupAreaByCupNum(dh.Value);
                    if (area != null)
                    {
                        area.OnCupDataReceived(dh.Value);
                    }
                }

                // 只对每个批次的杯号去重后归档一次
                foreach (var cupNum in toDeleteHeadIds.Values.Distinct())
                {
                    // 这里建议调用后台归档逻辑
                    SmartColor.My_Tool.CupAuxiliary.HandleCupFinished(cupNum);
                }

                if (hasActiveTask)
                {
                    DialogResult result = My_File.LocalTranslator.ShowMessage("确定要停止当前未完成的配方吗？", "滴液停止", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        return;
                    SmartColor.My_AutomaticModule.DropRobotTask.IsStopped = true;
                }
                else
                {
                    if (toDeleteHeadIds.Count == 0)
                        My_File.LocalTranslator.ShowMessage("当前没有正在滴液的任务", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // 刷新数据
                DoRefreshData(0);
            }
            catch (Exception ex)
            {
                Logger.Error("Btn_Stop_Click异常", ex);
            }
        }

        // 支持快捷键
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (HandleShortcutKeys(keyData))
                return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }
        public bool HandleShortcutKeys(Keys key)
        {
            if (key == Keys.F10 && btn_Start != null && btn_Start.Enabled)
            {
                Btn_Start_Click(btn_Start, EventArgs.Empty);
                return true;
            }
            return false;
        }
    }
}