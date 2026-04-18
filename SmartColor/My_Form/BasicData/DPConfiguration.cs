using SmartColor.My_File;
using SmartColor.My_DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Form.BasicData
{
    public partial class DPConfiguration : Form
    {
        private bool _isEditing = false;
        private bool _isProcessingSelectionChange = false;
        private readonly int _type = 1;
        private int _lastSelectedRowIndex = -1;
        private bool _isLoading = false;
        private string ShowType => _type == 1 ? "染色工艺" : "后处理工艺";

        public DPConfiguration(int type)
        {
            InitializeComponent();

            this._type = type;
            this.Text = $"{ShowType}配置";
            this.grp_BrewingProcess.Text = $"{ShowType}流程配置";
            this.dgv_DyeCode.Columns[2].Visible = _type == 1;
            this.dgv_DyeCode.Columns[1].HeaderText = $"{ShowType}代码";

            this.dgv_DyeCode.CellBeginEdit += Dgv_DyeCode_CellBeginEdit;
            this.dgv_DyeCode.SelectionChanged += Dgv_DyeCode_SelectionChanged;
            this.dgv_DyeCode.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgv_DyeData.KeyDown += Dgv_DyeData_KeyDown;
            this.dgv_DyeData.RowsAdded += (s, e) => ReorderStepNum();
            this.dgv_DyeData.RowsRemoved += (s, e) => ReorderStepNum();
            this.Load += DPConfiguration_Load;
            this.dgv_DyeCode.KeyDown += Dgv_DyeCode_KeyDown;
            this.dgv_DyeData.EnterKeyAction += () => { if (_isEditing) { FocusDyeDataAndEdit(); return true; } return false; };
            this.dgv_DyeCode.EnterKeyAction += () => { if (_isEditing) { FocusDyeCodeAndEdit(); return true; } return false; };
        }


        private void Dgv_DyeCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_isEditing) return;
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true; // 阻止默认行为
                FocusDyeCodeAndEdit();
            }
        }
        private void Dgv_DyeCode_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgv_DyeCode.CurrentCell == null || e.RowIndex != dgv_DyeCode.CurrentCell.RowIndex)
                e.Cancel = true;
        }

        private void ReorderStepNum()
        {
            int step = 1;
            foreach (DataGridViewRow row in dgv_DyeData.Rows)
                if (!row.IsNewRow) row.Cells[0].Value = step++;
        }

        private void FocusDyeCodeAndEdit()
        {
            var dgv = dgv_DyeCode;
            dgv.EndEdit();
            if (dgv.CurrentCell == null) return;
            int col = dgv.CurrentCell.ColumnIndex;
            int row = dgv.CurrentCell.RowIndex;

            void JumpTo(int targetRow, int targetCol)
            {
                _isProcessingSelectionChange = true;
                if (!dgv.Rows[targetRow].Cells[targetCol].Visible)
                {
                    JumpTo(targetRow, targetCol + 1);
                }
                else
                {
                    dgv.CurrentCell = dgv.Rows[targetRow].Cells[targetCol];
                }
                dgv.BeginEdit(true);
                _isProcessingSelectionChange = false;
            }

            void SmartJump()
            {
                if (dgv.Rows.Count == 0) return;
                if (row < 0 || row >= dgv.Rows.Count) return;

                // 判断是否为“修改”模式（代码列只读，且只允许编辑2、3列）
                bool isModifyMode = dgv.Columns[1].ReadOnly && !dgv.Columns[2].ReadOnly && !dgv.Columns[3].ReadOnly;

                if (isModifyMode)
                {
                    // 只允许在第3、4列之间跳转
                    if (col == 2)
                    {
                        JumpTo(row, 3);
                    }
                    else if (col == 3)
                    {
                        // 跳到dgv_DyeData
                        dgv_DyeData.ReadOnly = false;
                        if (dgv_DyeData.Rows.Count == 0) dgv_DyeData.Rows.Add();
                        dgv_DyeData.CurrentCell = dgv_DyeData.Rows[0].Cells[1];
                        dgv_DyeData.BeginEdit(true);
                    }
                    else
                    {
                        // 强制跳到第3列
                        JumpTo(row, 2);
                    }
                    return;
                }

                // 原有“新增”逻辑
                var cellValue = dgv.CurrentRow.Cells[1].Value;
                if (col == dgv.ColumnCount - 1)
                {
                    if (string.IsNullOrWhiteSpace(cellValue?.ToString()))
                    {
                        ShowErrorAndRemoveRow($"{ShowType}代码不能为空！");
                        return;
                    }
                    if (CodeExists(cellValue.ToString().Trim(), true))
                    {
                        ShowError($"{ShowType}代码已存在，不能重复！");
                        dgv.BeginEdit(true);
                        return;
                    }
                    dgv_DyeData.ReadOnly = false;
                    if (dgv_DyeData.Rows.Count == 0) dgv_DyeData.Rows.Add();
                    dgv_DyeData.CurrentCell = dgv_DyeData.Rows[0].Cells[1];
                    dgv_DyeData.BeginEdit(true);
                }
                else
                {
                    if (col == 1)
                    {
                        if (string.IsNullOrWhiteSpace(cellValue?.ToString()))
                        {
                            ShowErrorAndRemoveRow($"{ShowType}代码不能为空！");
                            return;
                        }
                        if (CodeExists(cellValue.ToString().Trim(), false))
                        {
                            ShowError($"{ShowType}代码已存在，不能重复！");
                            dgv.BeginEdit(true);
                            return;
                        }
                    }
                    JumpTo(row, col + 1);
                }
            }
            this.BeginInvoke(new Action(SmartJump));
        }

        private void FocusDyeDataAndEdit()
        {
            var dgv = dgv_DyeData;
            dgv.EndEdit();
            if (dgv.CurrentCell == null) return;
            int col = dgv.CurrentCell.ColumnIndex;
            int row = dgv.CurrentCell.RowIndex;
            int colCount = dgv.ColumnCount;
            int techCol = 1;
            string techName = dgv.Rows[row].Cells[techCol].Value?.ToString();

            void JumpTo(int targetRow, int targetCol)
            {
                var currentCell = dgv.Rows[row].Cells[col];
                var value = currentCell.Value;
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    ShowWarn("该列不能为空！");
                    dgv.CurrentCell = currentCell;
                    dgv.BeginEdit(true);
                    return;
                }
                dgv.CurrentCell = dgv.Rows[targetRow].Cells[targetCol];
                if (targetCol == 5)
                    dgv.CurrentCell.Value = 0;
                dgv.BeginEdit(true);
            }

            void EnsureNextRow(int currentRow)
            {
                if (currentRow == dgv.Rows.Count - 1 || dgv.Rows[currentRow + 1].IsNewRow)
                {
                    dgv.Rows.Add();
                    ReorderStepNum();
                }
            }

            void SmartJump()
            {
                if (row < 0 || row >= dgv.Rows.Count) return;
                var techCellValue = dgv.Rows[row].Cells[techCol].Value;
                if (string.IsNullOrWhiteSpace(techCellValue?.ToString())) { btn_Save.Focus(); return; }
                if (col == 1)
                {
                    if (new[] { "放布", "出布", "取小样", "测PH" }.Contains(techName))
                    { EnsureNextRow(row); JumpTo(row + 1, techCol); return; }
                    if (techName == "排液") { JumpTo(row, 5); return; }
                    if (techName != "温控") { JumpTo(row, 4); return; }
                    JumpTo(row, col + 1); return;
                }
                if (col == 5 && techName == "排液")
                {
                    if (string.IsNullOrWhiteSpace(dgv.Rows[row].Cells[col].Value?.ToString()))
                    { ShowWarn("该列不能为空！"); dgv.BeginEdit(true); return; }
                    EnsureNextRow(row); JumpTo(row + 1, techCol); return;
                }
                if (col == 4 && techName != "温控")
                {
                    if (string.IsNullOrWhiteSpace(dgv.Rows[row].Cells[col].Value?.ToString()))
                    { ShowWarn("该列不能为空！"); dgv.BeginEdit(true); return; }
                    EnsureNextRow(row); JumpTo(row + 1, techCol); return;
                }
                if (col < colCount - 1) { JumpTo(row, col + 1); return; }
                EnsureNextRow(row); JumpTo(row + 1, techCol);
            }
            this.BeginInvoke(new Action(SmartJump));
        }

        private void DPConfiguration_Load(object sender, EventArgs e)
        {
            try
            {
                Logger.Info($"DPConfiguration_Load: 开始加载{ShowType}代码表。");
                _isLoading = true;
                dgv_DyeCode.Rows.Clear();
                if (My_DataBase.DyeingData.Dyeing_process == null) return;
                int i = 0;
                dgv_DyeCode.BindDataTable(
                    My_DataBase.DyeingData.Dyeing_process.AsEnumerable()
                        .Where(r => r.Field<int>(My_DataBase.DYEING_PROCESS.Type) == _type && r[My_DataBase.DYEING_PROCESS.Code] != DBNull.Value)
                        .GroupBy(r => r.Field<string>(My_DataBase.DYEING_PROCESS.Code))
                        .Select(g => g.First())
                        .CopyToDataTable(),
                    row => new object[]
                    {
                        ++i,
                        row[My_DataBase.DYEING_PROCESS.Code],
                        (Convert.ToString(row[My_DataBase.DYEING_PROCESS.OpenMedicine]) == "1"),
                        row[My_DataBase.DYEING_PROCESS.Remark]
                    },
                    My_DataBase.DYEING_PROCESS.Code
                );
                dgv_DyeCode.ClearSelection();
                dgv_DyeData.Rows.Clear();
                _isLoading = false;
                Logger.Info("DPConfiguration_Load: 加载完成。");
            }
            catch (Exception ex)
            {
                Logger.Error($"DPConfiguration_Load: 加载{ShowType}代码表异常。", ex);
            }
        }

        private void Dgv_DyeCode_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (_isLoading || _isProcessingSelectionChange) return;

                int prevRowIndex = _lastSelectedRowIndex;
                int currentRowIndex = -1;
                if (dgv_DyeCode.CurrentCell != null)
                    currentRowIndex = dgv_DyeCode.CurrentCell.RowIndex;
                else if (dgv_DyeCode.SelectedCells.Count > 0)
                    currentRowIndex = dgv_DyeCode.SelectedCells[0].RowIndex;

                // 只有“行”发生变化时，才处理未保存数据
                bool isRowChanged = prevRowIndex != -1 && currentRowIndex != -1 && prevRowIndex != currentRowIndex;

                if (_isEditing && isRowChanged)
                {
                    bool hasUnsaved = false;
                    DataGridViewRow prevRow = (prevRowIndex >= 0 && prevRowIndex < dgv_DyeCode.Rows.Count)
                        ? dgv_DyeCode.Rows[prevRowIndex]
                        : null;

                    if (prevRow != null)
                    {
                        var codeCell = prevRow.Cells[1];
                        if (!IsCellEmpty(codeCell))
                            hasUnsaved = true;
                        else if (prevRow.IsNewRow)
                            hasUnsaved = true;
                    }

                    if (hasUnsaved)
                    {
                        var result = LocalTranslator.ShowMessage(
                            "有未保存的数据，是否放弃修改？",
                            "提示",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.No)
                        {
                            _isProcessingSelectionChange = true;
                            if (prevRow != null)
                            {
                                // 恢复选中行
                                prevRow.Selected = true;
                                _lastSelectedRowIndex = prevRow.Index;

                                // 恢复当前单元格到可编辑列（优先第2列可编辑，否则第3列）
                                int editableCol = 2;
                                if (dgv_DyeCode.Columns[editableCol].ReadOnly && !dgv_DyeCode.Columns[3].ReadOnly)
                                    editableCol = 3;
                                dgv_DyeCode.CurrentCell = prevRow.Cells[editableCol];
                                dgv_DyeCode.BeginEdit(true);
                            }
                            _isProcessingSelectionChange = false;
                            return;
                        }
                    }

                    // 放弃修改，清理编辑状态
                    _isProcessingSelectionChange = true;
                    dgv_DyeCode.EndEdit();

                    if (prevRow != null)
                    {
                        DataTable dataTable = My_DataBase.DyeingData.Dyeing_process.AsEnumerable()
                           .Where(r => r.Field<int>(My_DataBase.DYEING_PROCESS.Type) == _type && r[My_DataBase.DYEING_PROCESS.Code] != DBNull.Value)
                           .GroupBy(r => r.Field<string>(My_DataBase.DYEING_PROCESS.Code))
                           .Select(g => g.First())
                           .CopyToDataTable();
                        if (dgv_DyeCode.Rows.Count > dataTable.Rows.Count)
                        {
                            dgv_DyeCode.Rows.RemoveAt(prevRow.Index);
                        }
                    }

                    _isEditing = false;
                    dgv_DyeCode.ReadOnly = true;
                    dgv_DyeData.ReadOnly = true;
                    _isProcessingSelectionChange = false;
                    dgv_DyeCode.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                }

                _lastSelectedRowIndex = currentRowIndex;

                if (dgv_DyeCode.SelectedRows == null || dgv_DyeCode.SelectedRows.Count == 0)
                    return;
                dgv_DyeData.Rows.Clear();
                var codeValue = dgv_DyeCode.SelectedRows[0].Cells[1].Value;
                if (codeValue == null || codeValue == DBNull.Value || My_DataBase.DyeingData.Dyeing_process == null) return;
                var drs = My_DataBase.DyeingData.Dyeing_process.AsEnumerable()
                    .Where(r => r.Field<string>(My_DataBase.DYEING_PROCESS.Code) == codeValue.ToString() && r.Field<int>(My_DataBase.DYEING_PROCESS.Type) == _type)
                    .OrderBy(r => r[My_DataBase.DYEING_PROCESS.StepNum])
                    .ToList();
                if (drs.Count > 0)
                {
                    dgv_DyeData.BindDataTable(
                        drs.CopyToDataTable(),
                        row => new object[]
                        {
                    row[My_DataBase.DYEING_PROCESS.StepNum]?.ToString() ?? "",
                    row[My_DataBase.DYEING_PROCESS.TechnologyName]?.ToString() ?? "",
                    row[My_DataBase.DYEING_PROCESS.Temp]?.ToString() ?? "",
                    row[My_DataBase.DYEING_PROCESS.Rate]?.ToString() ?? "",
                    row[My_DataBase.DYEING_PROCESS.ProportionOrTime]?.ToString() ?? "",
                    row[My_DataBase.DYEING_PROCESS.Rev]?.ToString() ?? ""
                        },
                        My_DataBase.DYEING_PROCESS.StepNum
                    );
                }
                Logger.Info($"dgv_DyeCode_SelectionChanged: 加载流程详情，代码={codeValue}");
            }
            catch (Exception ex)
            {
                Logger.Error("dgv_DyeCode_SelectionChanged: 加载流程详情异常。", ex);
            }
        }

        private void Dgv_DyeData_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_isEditing) return;
            var dgv = dgv_DyeData;
            switch (e.KeyCode)
            {
                case Keys.Insert:
                    dgv.EndEdit();
                    int idx = dgv.CurrentCell?.RowIndex ?? 0;
                    dgv.Rows.Insert(idx, 1);
                    ReorderStepNum();
                    dgv.CurrentCell = dgv.Rows[idx].Cells[1];
                    dgv.BeginEdit(true);
                    e.Handled = true;
                    break;
                case Keys.Delete:
                    dgv.EndEdit();
                    int delIdx = dgv.CurrentCell?.RowIndex ?? -1;
                    if (delIdx >= 0 && !dgv.Rows[delIdx].IsNewRow)
                    {
                        dgv.Rows.RemoveAt(delIdx);
                        ReorderStepNum();
                        e.Handled = true;
                    }
                    break;
            }
        }



        private void Btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isEditing) return;
                dgv_DyeCode.SelectionMode = DataGridViewSelectionMode.CellSelect;
                _isProcessingSelectionChange = true;
                _isEditing = true;
                dgv_DyeCode.CustomEditing = _isEditing;
                dgv_DyeData.CustomEditing = _isEditing;

                dgv_DyeCode.ReadOnly = false;
                dgv_DyeCode.Rows.Add();
                int newRowIdx = dgv_DyeCode.Rows.Count - 1;
                dgv_DyeCode.CurrentCell = dgv_DyeCode.Rows[newRowIdx].Cells[1];
                dgv_DyeCode.CurrentRow.Cells[0].Value = dgv_DyeCode.Rows.Count;
                _lastSelectedRowIndex = newRowIdx;
                dgv_DyeCode.BeginEdit(true);
                _isProcessingSelectionChange = false;
                dgv_DyeData.Rows.Clear();
                Logger.Info($"btn_Add_Click: 新增{ShowType}代码行。");
            }
            catch (Exception ex)
            {
                Logger.Error($"btn_Add_Click: 新增{ShowType}代码异常。", ex);
            }
        }

        private void Btn_Change_Click(object sender, EventArgs e)
        {
            if (!ValidateDyeCodeSelection()) return;
            _isEditing = true;
            dgv_DyeCode.CustomEditing = _isEditing;
            dgv_DyeData.CustomEditing = _isEditing;

            // 记录当前行
            int rowIndex = dgv_DyeCode.CurrentRow?.Index ?? 0;

            // 屏蔽 SelectionChanged 事件
            _isProcessingSelectionChange = true;
            dgv_DyeCode.SelectionMode = DataGridViewSelectionMode.CellSelect;

            // 只允许第3、4列可编辑
            dgv_DyeCode.ReadOnly = false;
            for (int i = 0; i < dgv_DyeCode.Columns.Count; i++)
                dgv_DyeCode.Columns[i].ReadOnly = !(i == 2 || i == 3);

            // 先清除选择
            dgv_DyeCode.ClearSelection();

            // 用 BeginInvoke 保证 CurrentCell 设置在控件刷新后
            this.BeginInvoke(new Action(() =>
            {
                if (rowIndex >= 0 && rowIndex < dgv_DyeCode.Rows.Count)
                {
                    dgv_DyeCode.Focus();
                    if (!dgv_DyeCode.Rows[rowIndex].Cells[2].Visible)
                    {
                        dgv_DyeCode.CurrentCell = dgv_DyeCode.Rows[rowIndex].Cells[3];
                        dgv_DyeCode.Rows[rowIndex].Cells[3].Selected = true;
                    }
                    else
                    {
                        dgv_DyeCode.CurrentCell = dgv_DyeCode.Rows[rowIndex].Cells[2];
                        dgv_DyeCode.Rows[rowIndex].Cells[2].Selected = true;
                       
                    }
                    dgv_DyeCode.BeginEdit(true);
                }
                _isProcessingSelectionChange = false;
            }));

            Logger.Info("btn_Change_Click: 进入编辑模式。");
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {

            _isEditing = false;
            dgv_DyeData.CustomEditing = _isEditing;
            if (!ValidateDyeCodeSelection()) return;
            DataGridViewCell cell = dgv_DyeCode.CurrentCell;
            if (cell == null) return;
            DataGridViewRow currentRow = dgv_DyeCode.Rows[cell.RowIndex];
            var dgvCode = currentRow.Cells[1];
            string newCode = dgvCode.Value.ToString().Trim();
            var existing = My_DataBase.DyeingData.Dyeing_process.Select($"{My_DataBase.DYEING_PROCESS.Code} = '{newCode}' AND {My_DataBase.DYEING_PROCESS.Type} = {_type}");

            bool hasValidRow = false;
            foreach (DataGridViewRow row in dgv_DyeData.Rows)
            {
                if (row.IsNewRow) continue;
                string techName = row.Cells[1].Value?.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(techName))
                {
                    hasValidRow = true;
                    break;
                }
            }
            if (!hasValidRow)
            {
                ShowError("流程明细不能为空，请填写至少一条有效工艺流程！");
                return;
            }
            // ========== 新增校验：每种“加X”比例之和必须为100 ==========
            var addGroups = new Dictionary<string, double>();
            foreach (DataGridViewRow row in dgv_DyeData.Rows)
            {
                if (row.IsNewRow) continue;
                string techName = row.Cells[1].Value?.ToString().Trim();
                if (string.IsNullOrWhiteSpace(techName)) continue;
                if (techName.StartsWith("加") && techName != "加水")
                {
                    string key = techName;
                    double val = 0;
                    double.TryParse(Convert.ToString(row.Cells[4].Value), out val);
                    if (!addGroups.ContainsKey(key))
                        addGroups[key] = 0;
                    addGroups[key] += val;
                }
            }
            foreach (var kv in addGroups)
            {
                if (Math.Abs(kv.Value - 100) > 0.01)
                {
                    ShowError($"操作类型“{kv.Key}”的比例之和为{kv.Value}，必须等于100！");
                    return;
                }
            }
            // ========== 校验结束 ==========
            try
            {
                if (existing.Length > 0)
                {
                    string datePrefix = DateTime.Now.ToString("yyyyMMddHHmmss");
                    bool result = SmartColor.My_Tool.HistoryBackupHelper.BackupAndUpdateHistory(
                        newCode,
                        datePrefix,
                        "修改",
                        ShowType,
                        My_DataBase.DYEING_CODE.TableName,
                        My_DataBase.HISTORY_DYEING_CODE.TableName,
                        code => My_DataBase.DyeingCodeData.Dyeing_code
                            .Select($"{My_DataBase.DYEING_CODE.Code} = '{code}' AND {My_DataBase.DYEING_CODE.Type} = {_type} AND " +
                               "DyeingCode IS NOT NULL")
                            .AsEnumerable().Select(r => r.Field<string>("DyeingCode")).Distinct().ToList(),
                        dyeingCodeList =>
                        {
                            string joinedCodes = string.Join("', '", dyeingCodeList.Select(s => s.Replace("'", "''")));
                            return My_DataBase.DyeingCodeData.Dyeing_code
                                .Select($"DyeingCode IN ('{joinedCodes}')")
                                .AsEnumerable().Select(r => r.Field<string>("Code")).Distinct().ToList();
                        }
                    );
                    if (!result) return;
                    SqlServer.Delete(My_DataBase.DYEING_PROCESS.TableName, $"{My_DataBase.DYEING_PROCESS.Code} ='{newCode}' AND {My_DataBase.DYEING_PROCESS.Type} = {_type}");
                }
                foreach (DataGridViewRow row in dgv_DyeData.Rows)
                {
                    if (row.IsNewRow) continue;
                    string tn = row.Cells[1].Value?.ToString().Trim();
                    if (string.IsNullOrWhiteSpace(tn)) continue;
                    var processData = new Dictionary<string, object>
                    {
                        {My_DataBase.DYEING_PROCESS.StepNum, row.Cells[0].Value},
                        {My_DataBase.DYEING_PROCESS.TechnologyName, tn},
                        {My_DataBase.DYEING_PROCESS.Temp, row.Cells[2].Value},
                        {My_DataBase.DYEING_PROCESS.Rate, row.Cells[3].Value},
                        {My_DataBase.DYEING_PROCESS.ProportionOrTime, row.Cells[4].Value},
                        {My_DataBase.DYEING_PROCESS.Code, newCode},
                        {My_DataBase.DYEING_PROCESS.Type, _type},
                        {My_DataBase.DYEING_PROCESS.Rev, row.Cells[5].Value},
                        {My_DataBase.DYEING_PROCESS.Remark, currentRow.Cells[3].Value},
                        {My_DataBase.DYEING_PROCESS.OpenMedicine, (currentRow.Cells[2].Value is bool b && b) ? 1 : 0},
                    };
                    SqlServer.Insert(My_DataBase.DYEING_PROCESS.TableName, processData);
                }
                Logger.Info($"btn_Save_Click: 保存代码[{newCode}]及流程成功。");
            }
            catch (Exception ex)
            {
                Logger.Error($"btn_Save_Click: 保存代码[{newCode}]异常。", ex);
                return;
            }
            dgv_DyeData.ReadOnly = true;
            dgv_DyeCode.ReadOnly = true;
            foreach (DataGridViewColumn col in dgv_DyeCode.Columns)
                col.ReadOnly = true;
            ShowInfo("保存完成");
            dgv_DyeCode.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DPConfiguration_Load(sender, new EventArgs());
            foreach (DataGridViewRow row in dgv_DyeCode.Rows)
            {
                if (row.Cells[1].Value?.ToString() == newCode)
                {
                    row.Selected = true;
                    dgv_DyeCode.CurrentCell = row.Cells[0];
                    break;
                }
            }

        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateDyeCodeSelection()) return;
                if (dgv_DyeCode.SelectedRows.Count == 0) return;
                var dgvCode = dgv_DyeCode.SelectedRows[0].Cells[1];
                string newCode = dgvCode.Value.ToString().Trim();
                var existing = My_DataBase.DyeingData.Dyeing_process.Select($"{My_DataBase.DYEING_PROCESS.Code}  = '{newCode}' AND  {My_DataBase.DYEING_PROCESS.Type} = {_type}");
                if (existing.Length == 0)
                {
                    ShowError($"{ShowType}代码不存在，无法删除！");
                    dgv_DyeCode.BeginEdit(true);
                    return;
                }

                int currentRowIndex = dgv_DyeCode.CurrentCell.RowIndex;
                string datePrefix = DateTime.Now.ToString("yyyyMMddHHmmss");
                bool result = SmartColor.My_Tool.HistoryBackupHelper.BackupAndUpdateHistory(
                    newCode,
                    datePrefix,
                    "删除",
                    ShowType,
                     My_DataBase.DYEING_CODE.TableName,
                     My_DataBase.HISTORY_DYEING_CODE.TableName,
                    code => My_DataBase.DyeingCodeData.Dyeing_code.Select($"{My_DataBase.DYEING_CODE.Code} = '{code}' AND {My_DataBase.DYEING_CODE.Type} = {_type} AND DyeingCode IS NOT NULL")
                        .AsEnumerable().Select(r => r.Field<string>("DyeingCode")).Distinct().ToList(),
                    dyeingCodeList =>
                    {
                        string joinedCodes = string.Join("', '", dyeingCodeList.Select(s => s.Replace("'", "''")));
                        return My_DataBase.DyeingCodeData.Dyeing_code.Select($"DyeingCode IN ('{joinedCodes}')")
                            .AsEnumerable().Select(r => r.Field<string>("Code")).Distinct().ToList();
                    }
                );
                if (!result) return;

                SqlServer.Delete(My_DataBase.DYEING_PROCESS.TableName, $"{My_DataBase.DYEING_PROCESS.Code} ='{newCode}' AND {My_DataBase.DYEING_PROCESS.Type} = {_type}");
                Logger.Info($"btn_Delete_Click: 删除原有代码[{newCode}]及其流程。");

                ShowInfo("删除完成");
                DPConfiguration_Load(sender, new EventArgs());

                if (currentRowIndex < dgv_DyeCode.Rows.Count - 1)
                {
                    dgv_DyeCode.Rows[currentRowIndex].Selected = true;
                    dgv_DyeCode.CurrentCell = dgv_DyeCode.Rows[currentRowIndex].Cells[0];
                }
                else
                {
                    dgv_DyeCode.Rows[dgv_DyeCode.Rows.Count - 1].Selected = true;
                    dgv_DyeCode.CurrentCell = dgv_DyeCode.Rows[dgv_DyeCode.Rows.Count - 1].Cells[0];
                }
            }
            catch (Exception ex)
            {
                My_File.Logger.Error("Btn_Delete_Click", ex);
            }


        }

        private void Tsmi_Copy_Click(object sender, EventArgs e)
        {
            var srcRow = dgv_DyeCode.CurrentRow;
            if (srcRow == null || srcRow.IsNewRow)
            {
                ShowWarn("请先选择要复制的工艺行！");
                return;
            }
            var srcCode = srcRow.Cells[1].Value?.ToString();
            var openMedicine = srcRow.Cells[2].Value;
            var remark = srcRow.Cells[3].Value;

            Btn_Add_Click(sender, e);

            var newRow = dgv_DyeCode.Rows[dgv_DyeCode.Rows.Count - 1];

            newRow.Cells[2].Value = openMedicine;
            newRow.Cells[3].Value = remark;

            dgv_DyeData.Rows.Clear();
            if (!string.IsNullOrWhiteSpace(srcCode) && My_DataBase.DyeingData.Dyeing_process != null)
            {
                var drs = My_DataBase.DyeingData.Dyeing_process.AsEnumerable()
                    .Where(r => r.Field<string>(My_DataBase.DYEING_PROCESS.Code) == srcCode && r.Field<int>(My_DataBase.DYEING_PROCESS.Type) == _type)
                    .OrderBy(r => r[My_DataBase.DYEING_PROCESS.StepNum])
                    .ToList();
                if (drs.Count > 0)
                {
                    dgv_DyeData.BindDataTable(
                        drs.CopyToDataTable(),
                        row => new object[]
                        {
                            row[My_DataBase.DYEING_PROCESS.StepNum]?.ToString() ?? "",
                            row[My_DataBase.DYEING_PROCESS.TechnologyName]?.ToString() ?? "",
                            row[My_DataBase.DYEING_PROCESS.Temp]?.ToString() ?? "",
                            row[My_DataBase.DYEING_PROCESS.Rate]?.ToString() ?? "",
                            row[My_DataBase.DYEING_PROCESS.ProportionOrTime]?.ToString() ?? "",
                            row[My_DataBase.DYEING_PROCESS.Rev]?.ToString() ?? ""
                        },
                        My_DataBase.DYEING_PROCESS.StepNum
                    );
                }
            }

            newRow.Cells[1].Value = srcCode + "拷贝";
            dgv_DyeCode.CurrentCell = newRow.Cells[1];
            dgv_DyeCode.EndEdit();
            dgv_DyeCode.BeginEdit(true);

            Logger.Info("Tsmi_Copy_Click: 触发新增并复制工艺及明细，等待用户输入新工艺代码。");
        }


        // ================= 工具方法 =================

        private bool ValidateDyeCodeSelection()
        {
            if (dgv_DyeCode.SelectedRows == null)
            {
                ShowInfo($"请先选择一个{ShowType}代码！");
                dgv_DyeCode.BeginEdit(true);
                return false;
            }
            if (dgv_DyeCode.SelectedCells.Count == 0)
            {
                ShowError("未选中任何单元格！");
                return false;
            }
            int selectedRowIndex = dgv_DyeCode.SelectedCells[0].RowIndex;
            if (selectedRowIndex < 0 || selectedRowIndex >= dgv_DyeCode.Rows.Count)
            {
                ShowError("选中行索引无效！");
                return false;
            }

            DataGridViewCell dgvCode = dgv_DyeCode.Rows[selectedRowIndex].Cells[1];
            if (dgvCode.Value == null || string.IsNullOrWhiteSpace(dgvCode.Value.ToString()))
            {
                ShowError($"{ShowType}代码不能为空！");
                dgv_DyeCode.BeginEdit(true);
                return false;
            }

            return true;
        }

        private bool CodeExists(string code, bool onlyType1)
        {
            string filter = onlyType1 ? $"{My_DataBase.DYEING_PROCESS.Code} = '{code}' AND {My_DataBase.DYEING_PROCESS.Type} = {_type}" : $"{My_DataBase.DYEING_PROCESS.Code} = '{code}'";
            return My_DataBase.DyeingData.Dyeing_process.Select(filter).Length > 0;
        }

        private void ShowErrorAndRemoveRow(string msg)
        {
            ShowError(msg);
            if (dgv_DyeCode.CurrentRow != null && !dgv_DyeCode.CurrentRow.IsNewRow)
                dgv_DyeCode.Rows.RemoveAt(dgv_DyeCode.CurrentRow.Index);
            _isEditing = false;
            dgv_DyeCode.ReadOnly = true;
        }

        private bool IsCellEmpty(DataGridViewCell cell)
        {
            return cell == null || string.IsNullOrWhiteSpace(Convert.ToString(cell.Value));
        }

        private void ShowError(string msg) => LocalTranslator.ShowMessage(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowInfo(string msg) => LocalTranslator.ShowMessage(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void ShowWarn(string msg) => LocalTranslator.ShowMessage(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}