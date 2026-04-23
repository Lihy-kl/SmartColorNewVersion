using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Form.DyeingMan;
using SmartColor.My_Form.HistoricalData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 配方浏览控件，支持条件筛选、分页、排序、删除等功能。
    /// 新增：支持“等待列表”模式，显示wait_list表内容。
    /// </summary>
    public partial class CtFormulaBrowse : UserControl
    {
        public string TableWaitName { get; set; }
        public CtDropHead HeadTarget { get; set; }
        public event EventHandler<DataTable> CurrentRowChanged;

        private DataTable _head = null;
        private DataTable _wait = null;
        private bool isLoading = false;
        private bool isReverseOrder = true;
        private int pageSize = 27;
        private int currentPage = 1;
        private int totalPage = 1;
        private DataTable allRowsTable = null;
        private DataTable allWaitListTable = null;
        private bool _pendingRefresh = false;
        private Timer _refreshTimer;

        private bool IsWaitListMode => rdo_Wait.Checked;

        public CtFormulaBrowse()
        {
            InitializeComponent();

            // 节流刷新Timer
            _refreshTimer = new Timer();
            _refreshTimer.Interval = 100;
            _refreshTimer.Tick += _refreshTimer_Tick;

            BindEvents();
        }

        private void _refreshTimer_Tick(object sender, EventArgs e)
        {
            _refreshTimer.Stop();
            _pendingRefresh = false;
            ShowFormulaHead();
        }

        #region 字段名选择工具
        private (string formulaCode, string versionNum, string state, string myID, string createTime) GetHeadFields()
        {
            if (TableWaitName == My_DataBase.WAIT_LIST.TableName)
                return (My_DataBase.FORMULA_HEAD.FormulaCode, My_DataBase.FORMULA_HEAD.VersionNum, My_DataBase.FORMULA_HEAD.State, My_DataBase.FORMULA_HEAD.MyID, My_DataBase.FORMULA_HEAD.CreateTime);
            else
                return (My_DataBase.ABS_FORMULA_HEAD.FormulaCode, My_DataBase.ABS_FORMULA_HEAD.VersionNum, My_DataBase.ABS_FORMULA_HEAD.State, My_DataBase.ABS_FORMULA_HEAD.MyID, My_DataBase.ABS_FORMULA_HEAD.CreateTime);
        }
        private (string formulaCode, string versionNum, string indexNum, string myID, string cupNum, string type) GetWaitFields()
        {
            if (TableWaitName == My_DataBase.WAIT_LIST.TableName)
                return (My_DataBase.WAIT_LIST.FormulaCode, My_DataBase.WAIT_LIST.VersionNum, My_DataBase.WAIT_LIST.IndexNum, My_DataBase.WAIT_LIST.MyID, My_DataBase.WAIT_LIST.CupNum, My_DataBase.WAIT_LIST.Type);
            else
                return (My_DataBase.ABS_WAIT_LIST.FormulaCode, My_DataBase.ABS_WAIT_LIST.VersionNum, My_DataBase.ABS_WAIT_LIST.IndexNum, My_DataBase.ABS_WAIT_LIST.MyID, My_DataBase.ABS_WAIT_LIST.CupNum, My_DataBase.ABS_WAIT_LIST.Type);
        }
        #endregion

        #region 事件绑定
        private void BindEvents()
        {
            btn_Browse_Select.Click += (s, e) => { currentPage = 1; RequestRefresh(); };
            btn_Browse_Delete.Click += Btn_Browse_Delete_Click;
            rdo_Browse_All.CheckedChanged += (s, e) => { currentPage = 1; RequestRefresh(); };
            rdo_Browse_NoDrop.CheckedChanged += (s, e) => { currentPage = 1; RequestRefresh(); };
            rdo_Browse_condition.CheckedChanged += (s, e) => { currentPage = 1; RequestRefresh(); };
            rdo_Wait.CheckedChanged += (s, e) => { currentPage = 1; RequestRefresh(); };
            ctDataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ctDataGridView1.CurrentCellChanged += CtDataGridView1_CurrentCellChanged;
            Tsm_PS.Click += (s, e) => { isReverseOrder = false; currentPage = 1; RequestRefresh(); };
            Tsm_RO.Click += (s, e) => { isReverseOrder = true; currentPage = 1; RequestRefresh(); };
            ctDataGridView1.KeyDown += CtDataGridView1_KeyDown_AutoPage;
            bindingNavigatorMoveFirstItem.Click += (s, e) => NavigateToPage(1);
            bindingNavigatorMovePreviousItem.Click += (s, e) => NavigateToPage(currentPage - 1);
            bindingNavigatorMoveNextItem.Click += (s, e) => NavigateToPage(currentPage + 1);
            bindingNavigatorMoveLastItem.Click += (s, e) => NavigateToPage(totalPage);
            bindingNavigatorPositionItem.Leave += (s, e) =>
            {
                if (int.TryParse(bindingNavigatorPositionItem.Text, out int page))
                    NavigateToPage(page);
            };
        }
        #endregion

        #region 节流刷新
        public void RequestRefresh()
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(RequestRefresh));
                return;
            }
            if (!_pendingRefresh)
            {
                _pendingRefresh = true;
                _refreshTimer.Stop();
                _refreshTimer.Start();
            }
        }
        #endregion

        #region 分页工具
        private void NavigateToPage(int page)
        {
            isLoading = true;
            currentPage = Math.Max(1, Math.Min(page, totalPage));
            if (IsWaitListMode)
                ShowPagedWaitList();
            else
                ShowPagedData();
            isLoading = false;
        }
        private void UpdateNavigator()
        {
            bindingNavigatorCountItem.Text = $"/ {totalPage}";
            if (bindingNavigatorPositionItem != null && !bindingNavigatorPositionItem.IsDisposed)
            {
                bindingNavigatorPositionItem.Text = currentPage.ToString();
            }
            bindingNavigatorMoveFirstItem.Enabled = currentPage > 1;
            bindingNavigatorMovePreviousItem.Enabled = currentPage > 1;
            bindingNavigatorMoveNextItem.Enabled = currentPage < totalPage;
            bindingNavigatorMoveLastItem.Enabled = currentPage < totalPage;
        }
        #endregion

        #region 主体功能
        private void CtDataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (isLoading || HeadTarget == null || ctDataGridView1.CurrentRow == null) return;
            try
            {
                var (formulaCodeN, versionNumN, _, myIDN, _) = GetHeadFields();
                string formulaCode = ctDataGridView1.CurrentRow.Cells[0].Value?.ToString();
                object versionNumObj = ctDataGridView1.CurrentRow.Cells[1].Value;
                if (string.IsNullOrEmpty(formulaCode) || versionNumObj == null) return;
                if (!int.TryParse(versionNumObj.ToString(), out int versionNum)) return;

                var dt = _head;
                var found = dt.AsEnumerable().Where(r =>
                    r.Field<string>(formulaCodeN) == formulaCode &&
                    r.Field<int>(versionNumN) == versionNum
                );
                DataTable singleRowTable = found.Any() ? found.CopyToDataTable() : dt.Clone();

                if (rdo_Wait.Checked)
                {
                    string cupNumN;
                    if (TableWaitName == My_DataBase.WAIT_LIST.TableName)
                        cupNumN = My_DataBase.WAIT_LIST.CupNum;
                    else
                        cupNumN = My_DataBase.ABS_WAIT_LIST.CupNum;

                    var dr = allWaitListTable.AsEnumerable().FirstOrDefault(r =>
                        r.Field<int>(myIDN) == Convert.ToInt32(ctDataGridView1.CurrentRow.Cells[2].Value));
                    if (dr != null && singleRowTable.Rows.Count > 0)
                        singleRowTable.Rows[0][cupNumN] = dr.Field<int>(cupNumN);
                }
                HeadTarget.FillControlsFromDataTable(singleRowTable);

                //如果是ABS页面，填充曲线
                if (TableWaitName == My_DataBase.ABS_WAIT_LIST.TableName)
                {
                    var absS = SqlServer.Select(My_DataBase.ABS_HISTORY_HEAD.TableName,
                        $"{My_DataBase.ABS_HISTORY_HEAD.FormulaCode}=@code AND {My_DataBase.ABS_HISTORY_HEAD.Stand}=1",
                        new System.Data.SqlClient.SqlParameter("@code", formulaCode));
                    DataTable absTable = null;
                    var firstS = absS?.AsEnumerable().FirstOrDefault();
                    if (firstS != null)
                    {
                        absTable = absS.Clone();
                        absTable.ImportRow(firstS);
                    }
                    CurrentRowChanged?.Invoke(this, absTable);
                }
                else
                {
                    CurrentRowChanged?.Invoke(this, null);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("填充目标控件时发生异常", ex);
            }
        }

        private void ShowFormulaHead()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowFormulaHead()));
                return;
            }
            isLoading = true;
            try
            {
                GetData();

                if (IsWaitListMode)
                {
                    ShowWaitList();
                    return;
                }
                ctDataGridView1.CurrentCellChanged -= CtDataGridView1_CurrentCellChanged;
                ctDataGridView1.ContextMenuStrip = contextMenuStrip1;
                if (_head == null)
                {
                    Logger.Info("配方头部数据为空，无法显示列表");
                    return;
                }
                var (formulaCodeN, versionNumN, stateN, myIDN, createTimeN) = GetHeadFields();
                var dt = _head;
                IEnumerable<DataRow> rows = dt.AsEnumerable();
                rows = FilterRowsByBrowseMode(rows, stateN, formulaCodeN, createTimeN);
                rows = isReverseOrder
                    ? rows.OrderByDescending(r => r.Field<DateTime>(createTimeN))
                    : rows.OrderBy(r => r.Field<DateTime>(createTimeN));
                allRowsTable = BuildAllRowsTable(rows, formulaCodeN, versionNumN, myIDN);
                int totalCount = allRowsTable.Rows.Count;
                toolStripLabel2.Text = $"共 {totalCount} 条";
                totalPage = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);
                currentPage = Math.Max(1, Math.Min(currentPage, totalPage));
                ShowPagedData();
            }
            catch (Exception ex)
            {
                Logger.Error("显示配方头部列表时发生异常", ex);
            }
            finally
            {
                isLoading = false;
            }
            ctDataGridView1.CurrentCell = null;
            ctDataGridView1.ClearSelection();
            ctDataGridView1.CurrentCellChanged += CtDataGridView1_CurrentCellChanged;
        }

        private void ShowWaitList()
        {
            try
            {
                ctDataGridView1.CurrentCellChanged -= CtDataGridView1_CurrentCellChanged;
                ctDataGridView1.ContextMenuStrip = contextMenuStrip2;
                var dt = _wait;
                allWaitListTable = new DataTable();
                var (formulaCodeN, versionNumN, indexNumN, myIDN, cupNumN, typeN) = GetWaitFields();
                allWaitListTable.Columns.Add(formulaCodeN, typeof(string));
                allWaitListTable.Columns.Add(versionNumN, typeof(string));
                allWaitListTable.Columns.Add(indexNumN, typeof(int));
                allWaitListTable.Columns.Add(cupNumN, typeof(int));
                allWaitListTable.Columns.Add(typeN, typeof(int));
                allWaitListTable.Columns.Add(myIDN, typeof(int));
                foreach (DataRow r in dt.Rows)
                {
                    allWaitListTable.Rows.Add(
                        r[formulaCodeN],
                        r[versionNumN],
                        r[indexNumN],
                        r[cupNumN],
                        r[typeN],
                        r[myIDN]
                    );
                }
                int totalCount = allWaitListTable.Rows.Count;
                toolStripLabel2.Text = $"共 {totalCount} 条";
                totalPage = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);
                currentPage = Math.Max(1, Math.Min(currentPage, totalPage));
                ShowPagedWaitList();
            }
            catch (Exception ex)
            {
                Logger.Error("显示等待列表时发生异常", ex);
            }
            ctDataGridView1.CurrentCell = null;
            ctDataGridView1.ClearSelection();
            ctDataGridView1.CurrentCellChanged += CtDataGridView1_CurrentCellChanged;
        }

        private void ShowPagedWaitList()
        {
            if (allWaitListTable == null) return;
            try
            {
                DataTable pageTable = allWaitListTable.Clone();
                int startIndex = (currentPage - 1) * pageSize;
                int endIndex = Math.Min(startIndex + pageSize, allWaitListTable.Rows.Count);
                for (int i = startIndex; i < endIndex; i++)
                    pageTable.ImportRow(allWaitListTable.Rows[i]);

                ctDataGridView1.Rows.Clear();
                if (ctDataGridView1.Columns.Count == 0)
                {
                    ctDataGridView1.Columns.Add("FormulaCode", "配方代码");
                    ctDataGridView1.Columns.Add("VersionNum", "版本号");
                    ctDataGridView1.Columns.Add("MyID", "MyID");
                }
                else
                {
                    if (ctDataGridView1.Columns.Count != 3 ||
                        ctDataGridView1.Columns[0].Name != "FormulaCode" ||
                        ctDataGridView1.Columns[1].Name != "VersionNum" ||
                        ctDataGridView1.Columns[2].Name != "MyID")
                    {
                        ctDataGridView1.Columns.Clear();
                        ctDataGridView1.Columns.Add("FormulaCode", "配方代码");
                        ctDataGridView1.Columns.Add("VersionNum", "版本号");
                        ctDataGridView1.Columns.Add("MyID", "MyID");
                    }
                }

                var (formulaCodeN, versionNumN, _, myIDN, _, _) = GetWaitFields();
                foreach (DataRow row in pageTable.Rows)
                    ctDataGridView1.Rows.Add(row[formulaCodeN], row[versionNumN], row[myIDN]);
                UpdateNavigator();
            }
            catch (Exception ex)
            {
                Logger.Error("分页显示等待列表时发生异常", ex);
            }
        }

        private IEnumerable<DataRow> FilterRowsByBrowseMode(IEnumerable<DataRow> rows, string colState, string colCode, string colCreateTime)
        {
            if (rdo_Browse_NoDrop.Checked)
                return rows.Where(r => r.Field<string>(colState) == "尚未滴液");
            else if (rdo_Browse_condition.Checked)
            {
                string code = txt_Browse_Code.Enabled ? txt_Browse_Code.Text.Trim() : "";
                DateTime? start = dt_Browse_Start.Enabled ? (DateTime?)dt_Browse_Start.Value.Date : null;
                DateTime? end = dt_Browse_End.Enabled ? (DateTime?)dt_Browse_End.Value.Date : null;
                return rows.Where(r =>
                    (string.IsNullOrEmpty(code) || (r.Field<string>(colCode)?.Contains(code) ?? false)) &&
                    (!start.HasValue || r.Field<DateTime>(colCreateTime) >= start.Value) &&
                    (!end.HasValue || r.Field<DateTime>(colCreateTime) <= end.Value)
                );
            }
            return rows;
        }

        private DataTable BuildAllRowsTable(IEnumerable<DataRow> rows, string colCode, string colVer, string colMyID)
        {
            var table = new DataTable();
            table.Columns.Add(colCode, typeof(string));
            table.Columns.Add(colVer, typeof(int));
            table.Columns.Add(colMyID, typeof(int));
            foreach (var r in rows)
                table.Rows.Add(r[colCode], r[colVer], r[colMyID]);
            return table;
        }

        private void ShowPagedData()
        {
            if (allRowsTable == null) return;
            try
            {
                var (formulaCodeN, versionNumN, _, myIDN, _, _) = GetWaitFields();
                DataTable pageTable = allRowsTable.Clone();
                int startIndex = (currentPage - 1) * pageSize;
                int endIndex = Math.Min(startIndex + pageSize, allRowsTable.Rows.Count);
                for (int i = startIndex; i < endIndex; i++)
                    pageTable.ImportRow(allRowsTable.Rows[i]);
                ctDataGridView1.Rows.Clear();
                ctDataGridView1.BindDataTable(
                    pageTable,
                    row => new object[] { row[formulaCodeN], row[versionNumN], row[myIDN] },
                    myIDN
                );
                UpdateNavigator();
                ctDataGridView1.ClearSelection();
                ctDataGridView1.CurrentCell = null;
            }
            catch (Exception ex)
            {
                Logger.Error("分页显示数据时发生异常", ex);
            }
        }
        #endregion

        #region 删除功能
        private void Btn_Browse_Delete_Click(object sender, EventArgs e)
        {
            if (ctDataGridView1.SelectedRows.Count == 0)
            {
                LocalTranslator.ShowMessage("请选择要删除的记录!", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var dr = LocalTranslator.ShowMessage("确认删除选中记录吗?", "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr != DialogResult.Yes) return;
            int deleteCount = 0;
            if (IsWaitListMode)
            {
                var (formulaCodeN, versionNumN, _, myIDN, _, _) = GetWaitFields();
                foreach (DataGridViewRow row in ctDataGridView1.SelectedRows)
                {
                    var formulaCode = row.Cells[0].Value?.ToString();
                    var versionNum = row.Cells[1].Value?.ToString();
                    var myID = row.Cells[2].Value;
                    try
                    {
                        My_DataBase.SqlServer.Delete(
                            TableWaitName,
                            $"{formulaCodeN}=@code AND {versionNumN}=@ver AND {myIDN}=@myID",
                            new System.Data.SqlClient.SqlParameter("@code", formulaCode),
                            new System.Data.SqlClient.SqlParameter("@ver", versionNum),
                            new System.Data.SqlClient.SqlParameter("@myID", myID)
                        );
                        var found = allWaitListTable.AsEnumerable().FirstOrDefault(r =>
                            r.Field<string>(formulaCodeN) == formulaCode &&
                            r.Field<string>(versionNumN) == versionNum &&
                            r.Field<int>(myIDN) == Convert.ToInt32(myID)
                        );
                        if (found != null)
                        {
                            allWaitListTable.Rows.Remove(found);
                            deleteCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"删除等待列表失败: {formulaCode}, 版本: {versionNum}, 杯号: {myID}", ex);
                    }
                }
                Logger.Info($"成功删除 {deleteCount} 条等待列表记录");
                GetData();
                ShowWaitList();
            }
            else
            {
                var (formulaCodeN, versionNumN, _, _, _) = GetHeadFields();
                string tableHN, tableDN;
                if (TableWaitName == My_DataBase.WAIT_LIST.TableName)
                {
                    tableHN = My_DataBase.FORMULA_HEAD.TableName;
                    tableDN = My_DataBase.FORMULA_DETAILS.TableName;
                }
                else
                {
                    tableHN = My_DataBase.ABS_FORMULA_HEAD.TableName;
                    tableDN = My_DataBase.ABS_FORMULA_DETAILS.TableName;
                }
                foreach (DataGridViewRow row in ctDataGridView1.SelectedRows)
                {
                    var formulaCode = row.Cells[0].Value?.ToString();
                    var versionNum = row.Cells[1].Value;
                    try
                    {
                        My_DataBase.SqlServer.Delete(
                            tableHN,
                            $"{formulaCodeN}=@code AND {versionNumN}=@ver",
                            new System.Data.SqlClient.SqlParameter("@code", formulaCode),
                            new System.Data.SqlClient.SqlParameter("@ver", versionNum));
                        My_DataBase.SqlServer.Delete(
                           tableDN,
                           $"{formulaCodeN}=@code AND {versionNumN}=@ver",
                           new System.Data.SqlClient.SqlParameter("@code", formulaCode),
                           new System.Data.SqlClient.SqlParameter("@ver", versionNum));
                        if (TableWaitName == My_DataBase.WAIT_LIST.TableName)
                        {
                            My_DataBase.SqlServer.Delete(
                                My_DataBase.FORMULA_HANDLE_DETAILS.TableName,
                                $"{My_DataBase.FORMULA_HANDLE_DETAILS.FormulaCode}=@code AND {My_DataBase.FORMULA_HANDLE_DETAILS.VersionNum}=@ver",
                                new System.Data.SqlClient.SqlParameter("@code", formulaCode),
                                new System.Data.SqlClient.SqlParameter("@ver", versionNum));
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"删除配方失败: {formulaCode}, 版本: {versionNum}", ex);
                    }
                }
                Logger.Info($"成功删除 {deleteCount} 条配方记录");
                GetData();
                ShowFormulaHead();
            }
        }
        #endregion

        #region 排序与键盘分页
        private void CtDataGridView1_KeyDown_AutoPage(object sender, KeyEventArgs e)
        {
            if (ctDataGridView1.Rows.Count == 0 || isLoading)
                return;
            int rowIndex = ctDataGridView1.CurrentCell?.RowIndex ?? -1;
            if (e.KeyCode == Keys.Down && rowIndex == ctDataGridView1.Rows.Count - 1)
            {
                if (currentPage < totalPage)
                {
                    NavigateToPage(currentPage + 1);
                    if (ctDataGridView1.Rows.Count > 0)
                    {
                        ctDataGridView1.CurrentCell = ctDataGridView1.Rows[0].Cells[0];
                        ctDataGridView1.Rows[0].Selected = true;
                    }
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Up && rowIndex == 0)
            {
                if (currentPage > 1)
                {
                    NavigateToPage(currentPage - 1);
                    int lastRow = ctDataGridView1.Rows.Count - 1;
                    if (lastRow >= 0)
                    {
                        ctDataGridView1.CurrentCell = ctDataGridView1.Rows[lastRow].Cells[0];
                        ctDataGridView1.Rows[lastRow].Selected = true;
                    }
                    e.Handled = true;
                }
            }
        }
        #endregion

        #region 其它功能
        public void ClearSelect()
        {
            this.ctDataGridView1.CurrentCell = null;
            this.ctDataGridView1.ClearSelection();
        }

        private void Tsm_Up_Click(object sender, EventArgs e)
        {
            if (!IsWaitListMode || ctDataGridView1.SelectedRows.Count != 1)
                return;
            var row = ctDataGridView1.SelectedRows[0];
            string formulaCode = row.Cells[0].Value?.ToString();
            string versionNum = row.Cells[1].Value?.ToString();
            int myID = Convert.ToInt32(row.Cells[2].Value);
            var (formulaCodeN, versionNumN, indexNumN, myIDN, _, _) = GetWaitFields();
            var rows = allWaitListTable.AsEnumerable()
                .OrderBy(r => r.Field<int>(indexNumN))
                .ToList();
            int idx = rows.FindIndex(r =>
                r.Field<string>(formulaCodeN) == formulaCode &&
                r.Field<string>(versionNumN) == versionNum &&
                r.Field<int>(myIDN) == myID);
            if (idx <= 0) return;
            var current = rows[idx];
            var prev = rows[idx - 1];
            int currentIndexNum = current.Field<int>(indexNumN);
            int prevIndexNum = prev.Field<int>(indexNumN);
            My_DataBase.SqlServer.Update(TableWaitName,
                new Dictionary<string, object> { { indexNumN, prevIndexNum } },
                $"{myIDN}=@MyID",
                new System.Data.SqlClient.SqlParameter("@MyID", myID)
            );
            My_DataBase.SqlServer.Update(TableWaitName,
                new Dictionary<string, object> { { indexNumN, currentIndexNum } },
                $"{myIDN}=@MyID",
                new System.Data.SqlClient.SqlParameter("@MyID", prev.Field<int>(myIDN))
            );
            current[indexNumN] = prevIndexNum;
            prev[indexNumN] = currentIndexNum;
            ShowWaitList();
        }

        private void Tsm_Down_Click(object sender, EventArgs e)
        {
            if (!IsWaitListMode || ctDataGridView1.SelectedRows.Count != 1)
                return;
            var row = ctDataGridView1.SelectedRows[0];
            string formulaCode = row.Cells[0].Value?.ToString();
            string versionNum = row.Cells[1].Value?.ToString();
            int myID = Convert.ToInt32(row.Cells[2].Value);
            var (formulaCodeN, versionNumN, indexNumN, myIDN, _, _) = GetWaitFields();
            var rows = allWaitListTable.AsEnumerable()
                .OrderBy(r => r.Field<int>(indexNumN))
                .ToList();
            int idx = rows.FindIndex(r =>
                r.Field<string>(formulaCodeN) == formulaCode &&
                r.Field<string>(versionNumN) == versionNum &&
                r.Field<int>(myIDN) == myID);
            if (idx < 0 || idx >= rows.Count - 1) return;
            var current = rows[idx];
            var next = rows[idx + 1];
            int currentIndexNum = current.Field<int>(indexNumN);
            int nextIndexNum = next.Field<int>(indexNumN);
            My_DataBase.SqlServer.Update(TableWaitName,
                new Dictionary<string, object> { { indexNumN, nextIndexNum } },
                $"{myIDN}=@MyID",
                new System.Data.SqlClient.SqlParameter("@MyID", myID)
            );
            My_DataBase.SqlServer.Update(TableWaitName,
                new Dictionary<string, object> { { indexNumN, currentIndexNum } },
                $"{myIDN}=@MyID",
                new System.Data.SqlClient.SqlParameter("@MyID", next.Field<int>(myIDN))
            );
            current[indexNumN] = nextIndexNum;
            next[indexNumN] = currentIndexNum;
            ShowWaitList();
        }

        private void Tsmi_CupChange_Click(object sender, EventArgs e)
        {
            if (ctDataGridView1.CurrentRow == null)
                return;
            var (_, _, _, myIDN, cupNumN, _) = GetWaitFields();
            var id = Convert.ToInt32(ctDataGridView1.CurrentRow.Cells[2].Value);
            var dr = allWaitListTable.AsEnumerable().FirstOrDefault(r =>
                       r.Field<int>(myIDN) == id);
            if (dr != null)
            {
                int cupNum = dr.Field<int>(cupNumN);
                var cupInfo = new My_Form.DyeingMan.CupChange.CupChangeInfo
                {
                    OldCupNo = cupNum.ToString(),
                    MyID = id,
                    Type = CupChange.CupChangeType.DropWaitList
                };
                var dlg = new CupChange(cupInfo);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    int newCupNum = dlg.NewCupNum;
                    GetData();
                    ShowFormulaHead();
                    SetCurrentRowByMyID(id);
                }
            }
        }

        public void SetCurrentRowByMyID(int myID)
        {
            foreach (DataGridViewRow row in ctDataGridView1.Rows)
            {
                if (row.Cells[2].Value != null && Convert.ToInt32(row.Cells[2].Value) == myID)
                {
                    ctDataGridView1.CurrentCell = row.Cells[0];
                    row.Selected = true;
                    return;
                }
            }
        }

        public void SetTableWaitName(string tableName)
        {
            TableWaitName = tableName;
            GetData();
            ShowFormulaHead();
        }

        public void GetData()
        {
            if (TableWaitName == My_DataBase.WAIT_LIST.TableName)
            {
                _head = SqlServer.Select(My_DataBase.FORMULA_HEAD.TableName);
                _wait = My_DataBase.DropWaitData.GetData();
            }
            else if (TableWaitName == My_DataBase.ABS_WAIT_LIST.TableName)
            {
                _head = SqlServer.Select(My_DataBase.ABS_FORMULA_HEAD.TableName);
                _wait = My_DataBase.ABSWaitData.GetData();
            }
            else
            {
                Logger.Error($"CtFormulaBrowse.GetData: 未知的数据表名 {TableWaitName}。");
                _head = null;
                _wait = null;
            }
            rdo_Wait.Text = $"等待列表({_wait?.Rows.Count ?? 0})";
        }

        public List<(string FormulaCode, string VersionNum)> GetSelectedFormulaCodes()
        {
            var result = new List<(string, string)>();
            if (rdo_Wait.Checked == true)
                return result;

            // 按行号（Index）升序排序
            var rows = ctDataGridView1.SelectedRows
                .Cast<DataGridViewRow>()
                .OrderBy(r => r.Index)
                .ToList();

            foreach (var row in rows)
            {
                if (row.Cells.Count >= 2)
                {
                    string code = row.Cells[0].Value?.ToString();
                    string ver = row.Cells[1].Value?.ToString();
                    if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(ver))
                        result.Add((code, ver));
                }
            }
            return result;
        }
        #endregion
    }
}