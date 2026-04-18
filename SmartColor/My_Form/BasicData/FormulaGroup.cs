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
    public partial class FormulaGroup : Form
    {
      

        private bool _isEditing = false;
        private bool _isProcessingSelectionChange = false;
        private int _lastSelectedRowIndex = -1;
        private bool _isLoading = false;

        public FormulaGroup()
        {
            InitializeComponent();

            this.Text = "配方组合基本资料";
            this.grp_Dyeing.Text = "浏览";
            this.grp_Data.Text = "配方组合设定";

            this.dgv_FormulaGroupCode.SelectionChanged += Dgv_FormulaGroupCode_SelectionChanged;
            this.dgv_FormulaGroupCode.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgv_FormulaGroupData.KeyDown += Dgv_FormulaGroupData_KeyDown;
            this.dgv_FormulaGroupData.RowsAdded += (s, e) => ReorderStepNum();
            this.dgv_FormulaGroupData.RowsRemoved += (s, e) => ReorderStepNum();
            this.dgv_FormulaGroupData.CellValueChanged += Dgv_FormulaGroupData_CellValueChanged;
            this.Load += FormulaGroup_Load;

            this.dgv_FormulaGroupData.EnterKeyAction += () => { if (_isEditing) { FocusGroupDataAndEdit(); return true; } return false; };
            this.dgv_FormulaGroupCode.EnterKeyAction += () => { if (_isEditing) { FocusGroupCodeAndEdit(); return true; } return false; };

            this.btn_Add.Click += Btn_Add_Click;
            this.btn_Change.Click += Btn_Change_Click;
            this.btn_Save.Click += Btn_Save_Click;
            this.btn_Delete.Click += Btn_Delete_Click;
            this.Tsmi_Copy.Click += Tsmi_Copy_Click;
        }

        private void Dgv_FormulaGroupData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 判断是否是染助剂代码列（假设是第1列，索引为1）
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                var dgv = dgv_FormulaGroupData;
                var codeCell = dgv.Rows[e.RowIndex].Cells[1];
                var code = codeCell.Value?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(code)) return;

                // 从 Assistant_details 查找对应的名称和单位
                var assistantRow = My_DataBase.AssistantData. Assistant_details?
                    .AsEnumerable()
                    .FirstOrDefault(r => r.Field<string>(My_DataBase.ASSISTANT_DETAILS. AssistantCode) == code);

                if (assistantRow != null)
                {
                    dgv.Rows[e.RowIndex].Cells[2].Value = assistantRow[My_DataBase.ASSISTANT_DETAILS.AssistantName];
                    dgv.Rows[e.RowIndex].Cells[3].Value = assistantRow[My_DataBase.ASSISTANT_DETAILS.UnitOfAccount];
                }
                else
                {
                    dgv.Rows[e.RowIndex].Cells[2].Value = "";
                    dgv.Rows[e.RowIndex].Cells[3].Value = "";
                }
            }
        }

        private void ReorderStepNum()
        {
            int step = 1;
            foreach (DataGridViewRow row in dgv_FormulaGroupData.Rows)
                if (!row.IsNewRow) row.Cells[0].Value = step++;
        }

        private void FocusGroupCodeAndEdit()
        {
            var dgv = dgv_FormulaGroupCode;
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
                var cellValue = dgv.CurrentRow.Cells[1].Value;
                if (col == dgv.ColumnCount - 1)
                {
                    if (string.IsNullOrWhiteSpace(cellValue?.ToString()))
                    {
                        ShowErrorAndRemoveRow("配方组合代码不能为空！");
                        return;
                    }
                    if (CodeExists(cellValue.ToString().Trim()))
                    {
                        ShowError("配方组合代码已存在，不能重复！");
                        dgv.BeginEdit(true);
                        return;
                    }
                    dgv_FormulaGroupData.ReadOnly = false;
                    if (dgv_FormulaGroupData.Rows.Count == 0) dgv_FormulaGroupData.Rows.Add();
                    dgv_FormulaGroupData.CurrentCell = dgv_FormulaGroupData.Rows[0].Cells[1];
                    dgv_FormulaGroupData.BeginEdit(true);
                }
                else
                {
                    if (col == 1)
                    {
                        if (string.IsNullOrWhiteSpace(cellValue?.ToString()))
                        {
                            ShowErrorAndRemoveRow("配方组合代码不能为空！");
                            return;
                        }
                        if (CodeExists(cellValue.ToString().Trim()))
                        {
                            ShowError("配方组合代码已存在，不能重复！");
                            dgv.BeginEdit(true);
                            return;
                        }
                    }
                    JumpTo(row, col + 1);
                }
            }
            this.BeginInvoke(new Action(SmartJump));
        }

        private void FocusGroupDataAndEdit()
        {
            var dgv = dgv_FormulaGroupData;
            dgv.EndEdit();
            if (dgv.CurrentCell == null) return;
            int col = dgv.CurrentCell.ColumnIndex;
            int row = dgv.CurrentCell.RowIndex;
            int colCount = dgv.ColumnCount;
            int codeCol = 1;
            string assistantCode = dgv.Rows[row].Cells[codeCol].Value?.ToString();

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
                var codeCellValue = dgv.Rows[row].Cells[codeCol].Value;
                if (string.IsNullOrWhiteSpace(codeCellValue?.ToString())) { btn_Save.Focus(); return; }
                if (col == 1)
                {
                    EnsureNextRow(row); JumpTo(row + 1, codeCol); return;
                }
                if (col < colCount - 1) { JumpTo(row, col + 1); return; }
                EnsureNextRow(row); JumpTo(row + 1, codeCol);
            }
            this.BeginInvoke(new Action(SmartJump));
        }

        private void FormulaGroup_Load(object sender, EventArgs e)
        {
            try
            {
                Logger.Info("FormulaGroup_Load: 开始加载配方组合代码表。");
                _isLoading = true;
                dgv_FormulaGroupCode.Rows.Clear();
                if (My_DataBase.FormulaGradeData.Formula_group == null) return;
                int i = 0;
                var filteredRows = My_DataBase.FormulaGradeData.Formula_group.AsEnumerable()
                    .Where(r => r[My_DataBase.FORMULA_GROUP.GroupName] != DBNull.Value)
                    .GroupBy(r => r.Field<string>(My_DataBase.FORMULA_GROUP.GroupName))
                    .Select(g => g.First());

                if (filteredRows.Any())
                {
                    dgv_FormulaGroupCode.BindDataTable(
                        filteredRows.CopyToDataTable(),
                        row => new object[]
                        {
                             ++i,
                            row[My_DataBase.FORMULA_GROUP.GroupName]
                        },
                        My_DataBase.FORMULA_GROUP.GroupName
                    );
                }

                dgv_FormulaGroupCode.ClearSelection();
                dgv_FormulaGroupData.Rows.Clear();
                _isLoading = false;
                Logger.Info("FormulaGroup_Load: 加载完成。");

                // 设置染助剂代码下拉项
                SetAssistantCodeItems();
            }
            catch (Exception ex)
            {
                Logger.Error("FormulaGroup_Load: 加载配方组合代码表异常。", ex);
            }
        }

        private void SetAssistantCodeItems()
        {
            var assistantCodes = My_DataBase.AssistantData.Assistant_details?
                .AsEnumerable()
                .Select(r => r.Field<string>(My_DataBase.ASSISTANT_DETAILS.AssistantCode))
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Distinct()
                .ToArray();

            if (assistantCodes != null)
            {
                var comboCol = dgv_FormulaGroupData.Columns[1] as DataGridViewComboBoxColumn;
                if (comboCol != null)
                {
                    comboCol.Items.Clear();
                    comboCol.Items.AddRange(assistantCodes);
                }
            }
        }

        private void Dgv_FormulaGroupCode_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (_isLoading || _isProcessingSelectionChange) return;
                int prevRowIndex = _lastSelectedRowIndex;
                if (dgv_FormulaGroupCode.CurrentCell != null)
                    _lastSelectedRowIndex = dgv_FormulaGroupCode.CurrentCell.RowIndex;
                else if (dgv_FormulaGroupCode.SelectedCells.Count > 0)
                    _lastSelectedRowIndex = dgv_FormulaGroupCode.SelectedCells[0].RowIndex;
                else
                    _lastSelectedRowIndex = -1;

                if (_isEditing)
                {
                    bool hasUnsaved = false;
                    DataGridViewRow prevRow = (prevRowIndex >= 0 && prevRowIndex < dgv_FormulaGroupCode.Rows.Count)
                        ? dgv_FormulaGroupCode.Rows[prevRowIndex]
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
                                prevRow.Selected = true;
                                _lastSelectedRowIndex = prevRow.Index;
                            }

                            _isProcessingSelectionChange = false;
                            return;
                        }
                    }

                    _isProcessingSelectionChange = true;
                    dgv_FormulaGroupCode.EndEdit();

                    if (prevRow != null)
                    {
                        DataTable dataTable = My_DataBase.FormulaGradeData.Formula_group.AsEnumerable()
                         .Where(r => r[My_DataBase.FORMULA_GROUP.GroupName] != DBNull.Value)
                         .GroupBy(r => r.Field<string>(My_DataBase.FORMULA_GROUP.GroupName))
                         .Select(g => g.First()).CopyToDataTable();
                        if (dgv_FormulaGroupCode.Rows.Count > dataTable.Rows.Count)
                        {
                            dgv_FormulaGroupCode.Rows.RemoveAt(prevRow.Index);
                        }
                    }

                    _isEditing = false;
                    dgv_FormulaGroupCode.ReadOnly = true;
                    dgv_FormulaGroupData.ReadOnly = true;
                    _isProcessingSelectionChange = false;
                    dgv_FormulaGroupCode.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                }
                if (dgv_FormulaGroupCode.SelectedRows == null || dgv_FormulaGroupCode.SelectedRows.Count == 0)
                    return;
                dgv_FormulaGroupData.Rows.Clear();
                var codeValue = dgv_FormulaGroupCode.SelectedRows[0].Cells[1].Value;
                if (codeValue == null || codeValue == DBNull.Value || My_DataBase.FormulaGradeData.Formula_group == null) return;
                var drs = My_DataBase.FormulaGradeData.Formula_group.AsEnumerable()
                    .Where(r => r.Field<string>(My_DataBase.FORMULA_GROUP.GroupName) == codeValue.ToString())
                    .OrderBy(r => r[My_DataBase.FORMULA_GROUP.Id])
                    .ToList();
                if (drs.Count > 0)
                {
                    dgv_FormulaGroupData.BindDataTable(
                        drs.CopyToDataTable(),
                        row => new object[]
                        {
                            row[My_DataBase.FORMULA_GROUP.Id] ?? "",
                            row[My_DataBase.FORMULA_GROUP.AssistantCode] ?? "",
                            row[My_DataBase.FORMULA_GROUP.AssistantName] ?? "",
                            row[My_DataBase.FORMULA_GROUP.UnitOfAccount] ?? ""
                        },
                        My_DataBase.FORMULA_GROUP.Id
                    );
                }
                Logger.Info($"Dgv_FormulaGroupCode_SelectionChanged: 加载组合详情，代码={codeValue}");
            }
            catch (Exception ex)
            {
                Logger.Error("Dgv_FormulaGroupCode_SelectionChanged: 加载组合详情异常。", ex);
            }
        }

        private void Dgv_FormulaGroupData_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_isEditing) return;
            var dgv = dgv_FormulaGroupData;
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
                dgv_FormulaGroupCode.SelectionMode = DataGridViewSelectionMode.CellSelect;
                _isProcessingSelectionChange = true;
                _isEditing = true;
                dgv_FormulaGroupCode.CustomEditing = _isEditing;
                dgv_FormulaGroupData.CustomEditing = _isEditing;
                dgv_FormulaGroupCode.ReadOnly = false;
                dgv_FormulaGroupCode.Rows.Add();
                int newRowIdx = dgv_FormulaGroupCode.Rows.Count - 1;
                dgv_FormulaGroupCode.CurrentCell = dgv_FormulaGroupCode.Rows[newRowIdx].Cells[1];
                dgv_FormulaGroupCode.CurrentRow.Cells[0].Value = dgv_FormulaGroupCode.Rows.Count;
                _lastSelectedRowIndex = newRowIdx;
                dgv_FormulaGroupCode.BeginEdit(true);
                _isProcessingSelectionChange = false;
                dgv_FormulaGroupData.Rows.Clear();
                Logger.Info("Btn_Add_Click: 新增配方组合代码行。");
            }
            catch (Exception ex)
            {
                Logger.Error("Btn_Add_Click: 新增配方组合代码异常。", ex);
            }
        }

        private void Btn_Change_Click(object sender, EventArgs e)
        {
            if (!ValidateGroupCodeSelection()) return;
            _isEditing = true;
            dgv_FormulaGroupCode.CustomEditing = _isEditing;
            dgv_FormulaGroupData.CustomEditing = _isEditing;
            dgv_FormulaGroupData.ReadOnly = false;
            if (dgv_FormulaGroupData.Rows.Count == 0) dgv_FormulaGroupData.Rows.Add();
            dgv_FormulaGroupData.CurrentCell = dgv_FormulaGroupData.Rows[0].Cells[1];
            dgv_FormulaGroupData.BeginEdit(true);
            Logger.Info("Btn_Change_Click: 进入编辑模式。");
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            dgv_FormulaGroupData.EndEdit();
            _isEditing = false;
            dgv_FormulaGroupData.CustomEditing = _isEditing;
            if (!ValidateGroupCodeSelection()) return;
            DataGridViewCell cell = dgv_FormulaGroupCode.CurrentCell;
            if (cell == null) return;
            DataGridViewRow currentRow = dgv_FormulaGroupCode.Rows[cell.RowIndex];
            var dgvCode = currentRow.Cells[1];
            string newCode = dgvCode.Value.ToString().Trim();

            bool hasValidRow = false;
            var assistantCodeSet = new HashSet<string>();
            foreach (DataGridViewRow row in dgv_FormulaGroupData.Rows)
            {
                if (row.IsNewRow) continue;
                string assistantCode = row.Cells[1].Value?.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(assistantCode))
                {
                    hasValidRow = true;
                    // 查重
                    if (!assistantCodeSet.Add(assistantCode))
                    {
                        ShowError($"组合明细中染助剂代码“{assistantCode}”重复，请检查！");
                        return;
                    }
                }
            }
            if (!hasValidRow)
            {
                ShowError("组合明细不能为空，请填写至少一条有效组合明细！");
                return;
            }

            try
            {
                // 删除原有组合明细
                SqlServer.Delete(My_DataBase.FORMULA_GROUP.TableName, $"{My_DataBase.FORMULA_GROUP.GroupName} ='{newCode}'");
                foreach (DataGridViewRow row in dgv_FormulaGroupData.Rows)
                {
                    if (row.IsNewRow) continue;
                    string ac = row.Cells[1].Value?.ToString().Trim();
                    if (string.IsNullOrWhiteSpace(ac)) continue;
                    var groupData = new Dictionary<string, object>
            {
                {My_DataBase.FORMULA_GROUP.GroupName, newCode},
                {My_DataBase.FORMULA_GROUP.AssistantCode, ac},
                {My_DataBase.FORMULA_GROUP.AssistantName, row.Cells[2].Value},
                {My_DataBase.FORMULA_GROUP.UnitOfAccount, row.Cells[3].Value},
                {My_DataBase.FORMULA_GROUP.CreateTime, DateTime.Now}
            };
                    SqlServer.Insert(My_DataBase.FORMULA_GROUP.TableName, groupData);
                }
                Logger.Info($"Btn_Save_Click: 保存组合[{newCode}]及明细成功。");
            }
            catch (Exception ex)
            {
                Logger.Error($"Btn_Save_Click: 保存组合[{newCode}]异常。", ex);
                return;
            }
            dgv_FormulaGroupData.ReadOnly = true;
            dgv_FormulaGroupCode.ReadOnly = true;
            ShowInfo("保存完成");
            dgv_FormulaGroupCode.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            FormulaGroup_Load(sender, new EventArgs());
            foreach (DataGridViewRow row in dgv_FormulaGroupCode.Rows)
            {
                if (row.Cells[1].Value?.ToString() == newCode)
                {
                    row.Selected = true;
                    dgv_FormulaGroupCode.CurrentCell = row.Cells[0];
                    break;
                }
            }
           
        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            if (!ValidateGroupCodeSelection()) return;
            var dgvCode = dgv_FormulaGroupCode.SelectedRows[0].Cells[1];
            string newCode = dgvCode.Value.ToString().Trim();
            var existing = My_DataBase.FormulaGradeData.Formula_group.Select($"{My_DataBase.FORMULA_GROUP.GroupName} = '{newCode}'");
            if (existing.Length == 0)
            {
                ShowError("配方组合代码不存在，无法删除！");
                dgv_FormulaGroupCode.BeginEdit(true);
                return;
            }
            int currentRowIndex =  dgv_FormulaGroupCode.CurrentCell.RowIndex;
            // 增加删除确认提示
            var result = LocalTranslator.ShowMessage(
                $"确定要删除配方组合代码“{newCode}”及其所有明细吗？",
                "删除确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
            {
                return;
            }

            SqlServer.Delete(My_DataBase.FORMULA_GROUP.TableName, $"{My_DataBase.FORMULA_GROUP.GroupName} ='{newCode}'");
            Logger.Info($"Btn_Delete_Click: 删除原有组合[{newCode}]及其明细。");
            ShowInfo("删除完成");
            FormulaGroup_Load(sender, new EventArgs());
            if (currentRowIndex <  dgv_FormulaGroupCode.Rows.Count - 1)
            {
                 dgv_FormulaGroupCode.Rows[currentRowIndex].Selected = true;
                 dgv_FormulaGroupCode.CurrentCell =  dgv_FormulaGroupCode.Rows[currentRowIndex].Cells[0];
            }
            else
            {
                 dgv_FormulaGroupCode.Rows[ dgv_FormulaGroupCode.Rows.Count - 1].Selected = true;
                 dgv_FormulaGroupCode.CurrentCell =  dgv_FormulaGroupCode.Rows[ dgv_FormulaGroupCode.Rows.Count - 1].Cells[0];
            }


           
        }

        private void Tsmi_Copy_Click(object sender, EventArgs e)
        {
            var srcRow = dgv_FormulaGroupCode.CurrentRow;
            if (srcRow == null || srcRow.IsNewRow)
            {
                ShowWarn("请先选择要复制的组合行！");
                return;
            }
            var srcCode = srcRow.Cells[1].Value?.ToString();

            Btn_Add_Click(sender, e);

            var newRow = dgv_FormulaGroupCode.Rows[dgv_FormulaGroupCode.Rows.Count - 1];

            dgv_FormulaGroupData.Rows.Clear();
            if (!string.IsNullOrWhiteSpace(srcCode) && My_DataBase.FormulaGradeData.Formula_group != null)
            {
                var drs = My_DataBase.FormulaGradeData.Formula_group.AsEnumerable()
                    .Where(r => r.Field<string>(My_DataBase.FORMULA_GROUP.GroupName) == srcCode)
                    .OrderBy(r => r[My_DataBase.FORMULA_GROUP.Id])
                    .ToList();
                if (drs.Count > 0)
                {
                    dgv_FormulaGroupData.BindDataTable(
                        drs.CopyToDataTable(),
                        row => new object[]
                        {
                            row[My_DataBase.FORMULA_GROUP.Id] ?? "",
                            row[My_DataBase.FORMULA_GROUP.AssistantCode] ?? "",
                            row[My_DataBase.FORMULA_GROUP.AssistantName] ?? "",
                            row[My_DataBase.FORMULA_GROUP.UnitOfAccount] ?? ""
                        },
                        My_DataBase.FORMULA_GROUP.Id
                    );
                }
            }

            newRow.Cells[1].Value = srcCode + "拷贝";
            dgv_FormulaGroupCode.CurrentCell = newRow.Cells[1];
            dgv_FormulaGroupCode.EndEdit();
            dgv_FormulaGroupCode.BeginEdit(true);

            Logger.Info("Tsmi_Copy_Click: 触发新增并复制组合及明细，等待用户输入新组合代码。");
        }

        // ================= 工具方法 =================

        private bool ValidateGroupCodeSelection()
        {
            if (dgv_FormulaGroupCode.SelectedRows == null)
            {
                ShowInfo("请先选择一个配方组合代码！");
                dgv_FormulaGroupCode.BeginEdit(true);
                return false;
            }
            if (dgv_FormulaGroupCode.SelectedCells.Count == 0)
            {
                ShowError("未选中任何单元格！");
                return false;
            }
            int selectedRowIndex = dgv_FormulaGroupCode.SelectedCells[0].RowIndex;
            if (selectedRowIndex < 0 || selectedRowIndex >= dgv_FormulaGroupCode.Rows.Count)
            {
                ShowError("选中行索引无效！");
                return false;
            }

            DataGridViewCell dgvCode = dgv_FormulaGroupCode.Rows[selectedRowIndex].Cells[1];
            if (dgvCode.Value == null || string.IsNullOrWhiteSpace(dgvCode.Value.ToString()))
            {
                ShowError("配方组合代码不能为空！");
                dgv_FormulaGroupCode.BeginEdit(true);
                return false;
            }

            return true;
        }

        private bool CodeExists(string code)
        {
            string filter = $"{My_DataBase.FORMULA_GROUP.GroupName} = '{code}'";
            return My_DataBase.FormulaGradeData.Formula_group.Select(filter).Length > 0;
        }

        private void ShowErrorAndRemoveRow(string msg)
        {
            ShowError(msg);
            if (dgv_FormulaGroupCode.CurrentRow != null && !dgv_FormulaGroupCode.CurrentRow.IsNewRow)
                dgv_FormulaGroupCode.Rows.RemoveAt(dgv_FormulaGroupCode.CurrentRow.Index);
            _isEditing = false;
            dgv_FormulaGroupCode.ReadOnly = true;
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