using SmartColor.My_Control;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.UI.DataVisualization.Charting;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SmartColor.My_Form.BasicData
{
    public partial class DPPConfiguration : Form
    {



        // 内部状态变量
        private bool _isEditing = false;
        private bool _isProcessingSelectionChange = false;
        private bool _isLoading = false;
        private int _lastSelectedRowIndex = -1;
       

        private const int COL_INDEX_STEP = 0;
        private const int COL_INDEX_CODE = 1;

        public DPPConfiguration()
        {
            InitializeComponent();
            RegisterEvents();
            InitDataGridViewSettings();
        }

        private void RegisterEvents()
        {
            this.Load += DPPConfiguration_Load;
            this.FormClosed += DPPConfiguration_FormClosed;
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
            SqlServer.TableDataChanged += SqlServer_TableDataChanged;

            this.dgv_Dyeing_Code.CellBeginEdit += Dgv_DyeingCode_CellBeginEdit;
            this.dgv_Dyeing_Code.SelectionChanged += Dgv_Dyeing_Code_SelectionChanged;
            this.dgv_Dye_Code.SelectionChanged += Dgv_Dye_Code_SelectionChanged;
            this.dgv_Post_Code.SelectionChanged += Dgv_Post_Code_SelectionChanged;

            this.dgv_Dyeing_Code.Enter += (s, e) => { dgv_Dye_Code.ClearSelection(); dgv_Post_Code.ClearSelection(); };
            this.dgv_Post_Code.Enter += (s, e) => { dgv_Dye_Code.ClearSelection(); dgv_Dyeing_Code.ClearSelection(); };
            this.dgv_Dye_Code.Enter += (s, e) => { dgv_Dyeing_Code.ClearSelection(); dgv_Post_Code.ClearSelection(); };

            this.dgv_Combination.RowsAdded += (s, e) => ReorderStepNum();
            this.dgv_Combination.RowsRemoved += (s, e) => ReorderStepNum();
            this.dgv_Combination.KeyDown += Dgv_Combination_KeyDown;
        }

        private void SqlServer_TableDataChanged(string obj)
        {
           
            ;
            if (obj == My_DataBase.DYEING_PROCESS.TableName)
            {
                DPConfiguration_Dyeing_processChanged(this, new EventArgs());
            }
            if (obj == My_DataBase.DYEING_CODE.TableName)
            {
                UpdateDPP();
            }
        }

        private void DPPConfiguration_FormClosed(object sender, FormClosedEventArgs e)
        {
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;

        }

        private void DPConfiguration_Dyeing_processChanged(object sender, EventArgs e)
        {
            DPPConfiguration_Load(sender, e);
        }


        private void Dgv_DyeingCode_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgv_Dyeing_Code.CurrentCell == null || e.RowIndex != dgv_Dyeing_Code.CurrentCell.RowIndex)
                e.Cancel = true;
        }

        private void InitDataGridViewSettings()
        {
            dgv_Dyeing_Code.CustomEditing = true;
            dgv_Combination.CustomEditing = true;
            dgv_Combination.EnterKeyAction += () => { if (_isEditing) { FocusCombinationAndEdit(); return true; } return false; };
            dgv_Dyeing_Code.EnterKeyAction += () => { if (_isEditing) { FocusDyeingCodeAndEdit(); return true; } return false; };
        }

        private bool IsCellEmpty(DataGridViewCell cell)
        {
            return cell == null || string.IsNullOrWhiteSpace(Convert.ToString(cell.Value));
        }

        private bool IsDyeingCodeDuplicate(string code, int excludeRow)
        {
            for (int i = 0; i < dgv_Dyeing_Code.Rows.Count; i++)
            {
                if (i == excludeRow) continue;
                var cellVal = dgv_Dyeing_Code.Rows[i].Cells[COL_INDEX_CODE].Value;
                if (cellVal != null && cellVal.ToString().Trim() == code)
                    return true;
            }
            return false;
        }

        private bool HasValidCombinationRow()
        {
            foreach (DataGridViewRow row in dgv_Combination.Rows)
            {
                if (row.IsNewRow) continue;
                string code = row.Cells[COL_INDEX_CODE].Value?.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(code))
                    return true;
            }
            return false;
        }

        private void FocusDyeingCodeAndEdit()
        {
            var dgv = dgv_Dyeing_Code;
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
                var cellValue = dgv.Rows[row].Cells[COL_INDEX_CODE].Value;
                if (col == dgv.ColumnCount - 1)
                {
                    if (string.IsNullOrWhiteSpace(cellValue?.ToString()))
                    {
                        ShowError("染固色工艺代码不能为空！");
                        if (dgv.CurrentRow != null && !dgv.CurrentRow.IsNewRow)
                            dgv.Rows.RemoveAt(dgv.CurrentRow.Index);
                        _isEditing = false;
                        dgv.ReadOnly = true;
                        return;
                    }
                    if (IsDyeingCodeDuplicate(cellValue.ToString(), row))
                    {
                        ShowError("染固色工艺代码已存在，不能重复！");
                        dgv.BeginEdit(true);
                        return;
                    }
                    dgv_Combination.ReadOnly = false;
                    if (dgv_Combination.Rows.Count == 0) dgv_Combination.Rows.Add();
                    dgv_Combination.CurrentCell = dgv_Combination.Rows[0].Cells[COL_INDEX_CODE];
                    dgv_Combination.BeginEdit(true);
                }
                else
                {
                    if (col == COL_INDEX_CODE)
                    {
                        if (string.IsNullOrWhiteSpace(cellValue?.ToString()))
                        {
                            ShowError("染固色工艺代码不能为空！");
                            if (dgv.CurrentRow != null && !dgv.CurrentRow.IsNewRow)
                                dgv.Rows.RemoveAt(dgv.CurrentRow.Index);
                            _isEditing = false;
                            dgv.ReadOnly = true;
                            return;
                        }
                        if (cellValue.ToString() == My_Tool.CupAuxiliary.StopWashCupType ||
                            cellValue.ToString() == My_Tool.CupAuxiliary.PreWashCupType ||
                            cellValue.ToString() == My_Tool.CupAuxiliary.FailWashCupType ||
                            cellValue.ToString() == My_Tool.CupAuxiliary.HighTempWashCupType)
                        {
                            ShowError($"该染固色工艺代码系统已内置，不能重复！\n" +
                                $"({My_Tool.CupAuxiliary.StopWashCupType};\n" +
                                $" {My_Tool.CupAuxiliary.PreWashCupType}\n" +
                                $" {My_Tool.CupAuxiliary.FailWashCupType}\n" +
                                $" {My_Tool.CupAuxiliary.HighTempWashCupType})");
                            dgv.BeginEdit(true);
                            return;
                        }

                        if (IsDyeingCodeDuplicate(cellValue.ToString(), row))
                        {
                            ShowError("染固色工艺代码已存在，不能重复！");
                            dgv.BeginEdit(true);
                            return;
                        }
                    }
                    JumpTo(row, col + 1);
                }
            }
            this.BeginInvoke(new Action(SmartJump));
        }

        private void FocusCombinationAndEdit()
        {
            var dgv = dgv_Combination;
            dgv.EndEdit();
            if (dgv.CurrentCell == null) return;
            int col = dgv.CurrentCell.ColumnIndex;
            int row = dgv.CurrentCell.RowIndex;
            int colCount = dgv.ColumnCount;

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
                var codeCellValue = dgv.Rows[row].Cells[COL_INDEX_CODE].Value;
                if (string.IsNullOrWhiteSpace(codeCellValue?.ToString())) { btn_Save.Focus(); return; }
                if (col < colCount - 1)
                {
                    JumpTo(row, col + 1);
                    return;
                }
                EnsureNextRow(row);
                JumpTo(row + 1, COL_INDEX_CODE);
            }
            this.BeginInvoke(new Action(SmartJump));
        }

        private void Dgv_Combination_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_isEditing) return;
            var dgv = dgv_Combination;
            switch (e.KeyCode)
            {
                case Keys.Insert:
                    InsertCombinationRow(dgv);
                    e.Handled = true;
                    break;
                case Keys.Delete:
                    DeleteCombinationRow(dgv);
                    e.Handled = true;
                    break;
            }
        }

        private void InsertCombinationRow(DataGridView dgv)
        {
            dgv.EndEdit();
            int idx = dgv.CurrentCell?.RowIndex ?? 0;
            dgv.Rows.Insert(idx, 1);
            ReorderStepNum();
            dgv.CurrentCell = dgv.Rows[idx].Cells[COL_INDEX_CODE];
            dgv.BeginEdit(true);
        }

        private void DeleteCombinationRow(DataGridView dgv)
        {
            dgv.EndEdit();
            int delIdx = dgv.CurrentCell?.RowIndex ?? -1;
            if (delIdx >= 0 && !dgv.Rows[delIdx].IsNewRow)
            {
                dgv.Rows.RemoveAt(delIdx);
                ReorderStepNum();
            }
        }

        private void ReorderStepNum()
        {
            int step = 1;
            foreach (DataGridViewRow row in dgv_Combination.Rows)
            {
                if (row.IsNewRow) continue;
                row.Cells[COL_INDEX_STEP].Value = step++;
            }
        }

        private void Update_dgv_CombinationItems()
        {

            DataTable dt = My_DataBase.DyeingData.Dyeing_process.AsEnumerable()
                .Where(r => r[My_DataBase.DYEING_PROCESS.Code] != DBNull.Value)
                .GroupBy(r => r.Field<string>(My_DataBase.DYEING_PROCESS.Code))
                .Select(g => g.First())
                .CopyToDataTable();
            dataGridViewComboBoxColumn1.Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                dataGridViewComboBoxColumn1.Items.Add(row[My_DataBase.DYEING_PROCESS.Code].ToString());
            }
        }

        private void Dgv_Post_Code_SelectionChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            if (!HasSelectedRows(dgv_Post_Code)) return;
            try
            {
                ClearDetailUI();
                var codeValue = dgv_Post_Code.SelectedRows[0].Cells[0].Value;
                if (IsNullOrDbNull(codeValue) || My_DataBase.DyeingData.Dyeing_process == null) return;
                var drs = GetProcessRowsByCode(codeValue.ToString());
                BindChildGrid(drs);
                Logger.Info($" Dgv_Post_Code_SelectionChanged: 加载流程详情，代码={codeValue}");
            }
            catch (Exception ex)
            {
                Logger.Error(" Dgv_Post_Code_SelectionChanged: 加载流程详情异常。", ex);
            }
        }

        private void Dgv_Dye_Code_SelectionChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            if (!HasSelectedRows(dgv_Dye_Code)) return;
            try
            {
                ClearDetailUI();
                var codeValue = dgv_Dye_Code.SelectedRows[0].Cells[0].Value;
                if (IsNullOrDbNull(codeValue) || My_DataBase.DyeingData.Dyeing_process == null) return;
                var drs = GetProcessRowsByCode(codeValue.ToString());
                BindChildGrid(drs);
                Logger.Info($" Dgv_Dye_Code_SelectionChanged: 加载流程详情，代码={codeValue}");
            }
            catch (Exception ex)
            {
                Logger.Error(" Dgv_Dye_Code_SelectionChanged: 加载流程详情异常。", ex);
            }
        }

        private void Dgv_Dyeing_Code_SelectionChanged(object sender, EventArgs e)
        {
            int prevRowIndex = _lastSelectedRowIndex;
            if (dgv_Dyeing_Code.CurrentCell != null)
                _lastSelectedRowIndex = dgv_Dyeing_Code.CurrentCell.RowIndex;
            else if (dgv_Dyeing_Code.SelectedCells.Count > 0)
                _lastSelectedRowIndex = dgv_Dyeing_Code.SelectedCells[0].RowIndex;
            else
                _lastSelectedRowIndex = -1;

            if (_isLoading || _isProcessingSelectionChange) return;

            if (_isEditing)
            {
                bool hasUnsaved = false;
                DataGridViewRow prevRow = (prevRowIndex >= 0 && prevRowIndex < dgv_Dyeing_Code.Rows.Count)
                    ? dgv_Dyeing_Code.Rows[prevRowIndex]
                    : null;

                if (prevRow != null)
                {
                    var codeCell = prevRow.Cells[COL_INDEX_CODE];
                    // 新增时代码为空也弹窗提示
                    if (!IsCellEmpty(codeCell) || prevRow.IsNewRow)
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
                            prevRow.Selected = true;
                        _isProcessingSelectionChange = false;
                        return;
                    }
                }

                // 放弃修改，清理编辑状态
                _isProcessingSelectionChange = true;
                dgv_Dyeing_Code.EndEdit();

                if (prevRow != null)
                {
                    DataTable dataTable = My_DataBase.DyeingCodeData.Dyeing_code.AsEnumerable()
                           .Where(r => r[My_DataBase.DYEING_CODE.DyeingCode] != DBNull.Value)
                           .GroupBy(r => r.Field<string>(My_DataBase.DYEING_CODE.DyeingCode))
                           .Select(g => g.First())
                           .CopyToDataTable();
                    // 如果是新增且未保存，删除该行
                    if (dgv_Dyeing_Code.Rows.Count > dataTable.Rows.Count)
                    {
                        dgv_Dyeing_Code.Rows.RemoveAt(prevRow.Index);
                    }

                }

                _isEditing = false;
                dgv_Dyeing_Code.ReadOnly = true;
                dgv_Combination.ReadOnly = true;
                _isProcessingSelectionChange = false;
                dgv_Dyeing_Code.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            if (!HasSelectedRows(dgv_Dyeing_Code)) return;
            try
            {
                dgv_Combination.Rows.Clear();
                dgv_Child.Rows.Clear();
                var codeValue = dgv_Dyeing_Code.SelectedRows[0].Cells[COL_INDEX_CODE].Value;
                if (IsNullOrDbNull(codeValue) || My_DataBase.DyeingCodeData.Dyeing_code == null) return;
                var (drs, allRows) = My_Tool.FindDyeingCode.GetDyeingDetails(codeValue.ToString());
                int i = 0;

                if (drs.Count > 0)
                {
                    dgv_Combination.BindDataTable(
                        drs.CopyToDataTable(),
                        row => new object[] { ++i, row[My_DataBase.DYEING_CODE.Code]?.ToString() ?? "" },
                        My_DataBase.DYEING_CODE.IndexNum
                    );
                }

                dgv_Child.Rows.Clear();
                foreach (var arr in allRows)
                    dgv_Child.Rows.Add(arr);
                dgv_Child.AutoFitAllColumns();
                dgv_Child.ClearSelection();
                ShowChildCurve();
                Logger.Info($"Dgv_Dyeing_Code_SelectionChanged: 加载流程详情，代码={codeValue}");
            }
            catch (Exception ex)
            {
                Logger.Error("Dgv_Dyeing_Code_SelectionChanged: 加载流程详情异常。", ex);
            }
        }




        private void UpdateDPP()
        {
            try
            {
                Logger.Info("DPPConfiguration_Load: 开始加载染固色代码表。");
                int rowIndex = 0;
                dgv_Dyeing_Code.BindDataTable(
                    My_DataBase.DyeingCodeData.Dyeing_code,
                    row => new object[] { ++rowIndex, row[My_DataBase.DYEING_CODE.DyeingCode] },
                    My_DataBase.DYEING_CODE.DyeingCode
                );
                dgv_Combination.Rows.Clear();
                Logger.Info("DPPConfiguration_Load: 加载完成。");
            }
            catch (Exception ex)
            {
                Logger.Error("DPPConfiguration_Load: 加载染固色代码表异常。", ex);
            }
        }


        private void DPPConfiguration_Load(object sender, EventArgs e)
        {
            _isLoading = true;
            try { Update_dgv_CombinationItems(); }
            catch (Exception ex) { Logger.Error("DPPConfiguration_Load: 更新组合框数据异常。", ex); }

            UpdateDPP();


            try
            {
                Logger.Info("DPPConfiguration_Load: 开始加载染色工艺代码表。");
                dgv_Dye_Code.BindDataTable(
                    My_DataBase.DyeingData.Dyeing_process.AsEnumerable()
                        .Where(r => r.Field<int>(My_DataBase.DYEING_PROCESS.Type) == 1 && r[My_DataBase.DYEING_PROCESS.Code] != DBNull.Value)
                        .GroupBy(r => r.Field<string>(My_DataBase.DYEING_PROCESS.Code))
                        .Select(g => g.First())
                        .CopyToDataTable(),
                    row => new object[]
                    {
                        row[My_DataBase.DYEING_PROCESS.Code],
                        row[My_DataBase.DYEING_PROCESS.OpenMedicine].ToString() == "1",
                        row[My_DataBase.DYEING_PROCESS.Remark]
                    },
                    My_DataBase.DYEING_PROCESS.Code
                );
                dgv_Child.Rows.Clear();
                Logger.Info("DPPConfiguration_Load: 加载完成。");
            }
            catch (Exception ex)
            {
                Logger.Error("DPPConfiguration_Load: 加载染色工艺代码表异常。", ex);
            }

            try
            {
                Logger.Info("DPPConfiguration_Load: 开始加载后处理工艺代码表。");
                dgv_Post_Code.BindDataTable(
                    My_DataBase.DyeingData.Dyeing_process.AsEnumerable()
                        .Where(r => r.Field<int>(My_DataBase.DYEING_PROCESS.Type) == 2 && r[My_DataBase.DYEING_PROCESS.Code] != DBNull.Value)
                        .GroupBy(r => r.Field<string>(My_DataBase.DYEING_PROCESS.Code))
                        .Select(g => g.First())
                        .CopyToDataTable(),
                    row => new object[]
                    {
                        row[My_DataBase.DYEING_PROCESS.Code],
                        row[My_DataBase.DYEING_PROCESS.Remark]
                    },
                    My_DataBase.DYEING_PROCESS.Code
                );
                dgv_Child.Rows.Clear();
                Logger.Info("DPPConfiguration_Load: 加载完成。");
            }
            catch (Exception ex)
            {
                Logger.Error("DPPConfiguration_Load: 加载后处理工艺代码表异常。", ex);
            }
            _isLoading = false;
        }

        private void ClearDetailUI()
        {
            dgv_Child.Rows.Clear();
            ctChart1.ClearChart();
            textBox1.Clear();
            dgv_Combination.Rows.Clear();
        }

        private bool HasSelectedRows(DataGridView dgv)
        {
            return dgv.SelectedRows != null && dgv.SelectedRows.Count > 0;
        }

        private bool IsNullOrDbNull(object value)
        {
            return value == null || value == DBNull.Value;
        }

        private List<DataRow> GetProcessRowsByCode(string code)
        {

            return My_DataBase.DyeingData.Dyeing_process.AsEnumerable()
                .Where(r => r.Field<string>(My_DataBase.DYEING_PROCESS.Code) == code)
                .OrderBy(r => r[My_DataBase.DYEING_PROCESS.StepNum])
                .ToList();
        }

        private void BindChildGrid(List<DataRow> drs)
        {

            int i = 0;
            if (drs.Count > 0)
            {
                dgv_Child.BindDataTable(
                    drs.CopyToDataTable(),
                    row => new object[]
                    {
                        ++i,
                        row[My_DataBase.DYEING_PROCESS.TechnologyName]?.ToString() ?? "",
                        row[My_DataBase.DYEING_PROCESS.Temp]?.ToString() ?? "",
                        row[My_DataBase.DYEING_PROCESS.Rate]?.ToString() ?? "",
                        row[My_DataBase.DYEING_PROCESS.ProportionOrTime]?.ToString() ?? "",
                        row[My_DataBase.DYEING_PROCESS.Rev]?.ToString() ?? "",
                        1,
                    },
                    My_DataBase.DYEING_PROCESS.StepNum
                );
            }
        }

        private ProcessStep[] GetProcessStepsFromChild()
        {
            List<ProcessStep> steps = new List<ProcessStep>();
            foreach (DataGridViewRow row in dgv_Child.Rows)
            {
                if (row.IsNewRow) continue;
                var step = new ProcessStep
                {
                    StepName = row.Cells["Column6"].Value?.ToString() ?? ""
                };
                if (double.TryParse(row.Cells["Column7"].Value?.ToString(), out double temp))
                    step.TargetTemperature = temp;
                if (double.TryParse(row.Cells["Column8"].Value?.ToString(), out double rate))
                    step.HeatingRate = rate;
                if (double.TryParse(row.Cells["Column9"].Value?.ToString(), out double duration))
                    step.Duration = duration;
                steps.Add(step);
            }
            return steps.ToArray();
        }

        private void ShowChildCurve()
        {
            var processSteps = GetProcessStepsFromChild();
            ctChart1.ShowProcessCurve(processSteps, My_ConPar.Delay.TemRecordInterval, textBox1);
        }

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isEditing) return;
                dgv_Dyeing_Code.SelectionMode = DataGridViewSelectionMode.CellSelect;
                _isProcessingSelectionChange = true;
                _isEditing = true;
                dgv_Dyeing_Code.CustomEditing = _isEditing;
                dgv_Combination.CustomEditing = _isEditing;
                dgv_Dyeing_Code.ReadOnly = false;
                dgv_Dyeing_Code.Rows.Add();

                int newRowIdx = dgv_Dyeing_Code.Rows.Count - 1;
                dgv_Dyeing_Code.CurrentCell = dgv_Dyeing_Code.Rows[newRowIdx].Cells[COL_INDEX_CODE];
                dgv_Dyeing_Code.CurrentRow.Cells[COL_INDEX_STEP].Value = dgv_Dyeing_Code.Rows.Count;
                _lastSelectedRowIndex = newRowIdx;

                dgv_Dyeing_Code.BeginEdit(true);
                _isProcessingSelectionChange = false;
                dgv_Combination.Rows.Clear();
                Logger.Info($"btn_Add_Click: 新增染固色工艺代码行。");
            }
            catch (Exception ex)
            {
                Logger.Error($"btn_Add_Click: 新增染固色工艺代码异常。", ex);
            }
        }

        private void Btn_Change_Click(object sender, EventArgs e)
        {
            if (!ValidateDyeingCodeSelection()) return;
            _isEditing = true;
            dgv_Dyeing_Code.CustomEditing = _isEditing;
            dgv_Combination.CustomEditing = _isEditing;
            dgv_Combination.ReadOnly = false;
            if (dgv_Combination.Rows.Count == 0) dgv_Combination.Rows.Add();
            dgv_Combination.CurrentCell = dgv_Combination.Rows[0].Cells[COL_INDEX_CODE];
            dgv_Combination.BeginEdit(true);
            Logger.Info("btn_Change_Click: 进入编辑模式。");
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            // 临时注销事件
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;

            _isEditing = false;
            dgv_Dyeing_Code.CustomEditing = _isEditing;
            dgv_Combination.CustomEditing = _isEditing;
            dgv_Dyeing_Code.ReadOnly = true;
            dgv_Combination.ReadOnly = true;

            if (!ValidateDyeingCodeSelection())
            {
                // 恢复事件
                SqlServer.TableDataChanged += SqlServer_TableDataChanged;
                return;
            }

            DataGridViewCell cell = dgv_Dyeing_Code.CurrentCell;
            if (cell == null) return;
            DataGridViewRow currentRow = dgv_Dyeing_Code.Rows[cell.RowIndex];
            var dgvCode = currentRow.Cells[COL_INDEX_CODE];
            string newCode = dgvCode.Value.ToString().Trim();

            if (IsDyeingCodeDuplicate(newCode, currentRow.Index))
            {
                ShowError("染固色工艺代码已存在，不能重复！");
                dgv_Dyeing_Code.BeginEdit(true);
                return;
            }

            if (!HasValidCombinationRow())
            {
                ShowError("组合步骤不能为空，请填写至少一条有效组合！");
                return;
            }

            try
            {
                var existRows = My_DataBase.DyeingCodeData.Dyeing_code.Select($"{My_DataBase.DYEING_CODE.DyeingCode} = '{newCode}'");
                if (existRows.Length > 0)
                {
                   
                    string datePrefix = DateTime.Now.ToString("yyyyMMddHHmmss");
                    bool result = My_Tool.HistoryBackupHelper.BackupAndUpdateHistory(
                        newCode,
                        datePrefix,
                        "修改",
                        "染固色工艺",
                        My_DataBase.DYEING_CODE.TableName,
                        My_DataBase.HISTORY_DYEING_CODE.TableName,
                        code => My_DataBase.DyeingCodeData.Dyeing_code.Select($"{My_DataBase.DYEING_CODE.Code} = '{code}' AND {My_DataBase.DYEING_CODE.Type} = 1 AND {My_DataBase.DYEING_CODE.DyeingCode} IS NOT NULL")
                        .AsEnumerable().Select(r => r.Field<string>(My_DataBase.DYEING_CODE.DyeingCode)).Distinct().ToList(),
                        dyeingCodeList =>
                        {
                            string joinedCodes = string.Join("', '", dyeingCodeList.Select(s => s.Replace("'", "''")));
                            return My_DataBase.DyeingCodeData.Dyeing_code.Select($"{My_DataBase.DYEING_CODE.DyeingCode} IN ('{joinedCodes}')")
                            .AsEnumerable().Select(r => r.Field<string>("Code")).Distinct().ToList();
                        }
                    );
                    if (!result) return;
                    My_DataBase.SqlServer.Delete(My_DataBase.DYEING_CODE.TableName, $"{My_DataBase.DYEING_CODE.DyeingCode} = '{newCode}'");
                  
                }

                int indexNum = 1;
                foreach (DataGridViewRow row in dgv_Combination.Rows)
                {
                    if (row.IsNewRow) continue;
                    string code = row.Cells[COL_INDEX_CODE].Value?.ToString().Trim();
                    string type = My_DataBase.DyeingData.Dyeing_process.AsEnumerable()
                        .Where(r => r.Field<string>(My_DataBase.DYEING_PROCESS.Code) == code)
                        .Select(r => r.Field<int>(My_DataBase.DYEING_PROCESS.Type).ToString())
                        .FirstOrDefault() ?? "1";
                    if (string.IsNullOrWhiteSpace(code)) continue;
                    var data = new Dictionary<string, object>
                    {
                        { My_DataBase.DYEING_CODE.DyeingCode, newCode },
                        { My_DataBase.DYEING_CODE.Type,type },
                        { My_DataBase.DYEING_CODE.Step, indexNum },
                        { My_DataBase.DYEING_CODE.Code, code },
                        { My_DataBase.DYEING_CODE.IndexNum, indexNum },
                        { My_DataBase.DYEING_CODE.IsUse, 1 },
                        { My_DataBase.DYEING_CODE.Remark, "" }
                    };
                    My_DataBase.SqlServer.Insert(My_DataBase.DYEING_CODE.TableName, data);
                    indexNum++;
                }

                Logger.Info($"Btn_Save_Click: 保存染固色工艺代码[{newCode}]及组合成功。");
            }
            catch (Exception ex)
            {
                Logger.Error($"Btn_Save_Click: 保存[{newCode}]异常。", ex);
                return;
            }


            ShowInfo("保存完成");
            dgv_Dyeing_Code.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DPPConfiguration_Load(sender, new EventArgs());
            foreach (DataGridViewRow row in dgv_Dyeing_Code.Rows)
            {
                if (row.Cells[COL_INDEX_CODE].Value?.ToString() == newCode)
                {
                    row.Selected = true;
                    dgv_Dyeing_Code.CurrentCell = row.Cells[0];
                    break;
                }
            }
            // 恢复事件
            SqlServer.TableDataChanged += SqlServer_TableDataChanged;
        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            if (!ValidateDyeingCodeSelection()) return;
            var dgvCode = dgv_Dyeing_Code.SelectedRows[0].Cells[COL_INDEX_CODE];
            string code = dgvCode.Value.ToString().Trim();
            var existRows = My_DataBase.DyeingCodeData.Dyeing_code.Select($"{My_DataBase.DYEING_CODE.DyeingCode} = '{code}'");
            if (existRows.Length == 0)
            {
                ShowError("该染固色工艺代码不存在，无法删除！");
                dgv_Dyeing_Code.BeginEdit(true);
                return;
            }
            int currentRowIndex = dgv_Dyeing_Code.CurrentCell.RowIndex;
            string datePrefix = DateTime.Now.ToString("yyyyMMddHHmmss");
            bool result = SmartColor.My_Tool.HistoryBackupHelper.BackupAndUpdateHistory(
                code,
                datePrefix,
                "删除",
                "染固色工艺",
                My_DataBase.DYEING_CODE.TableName,
                My_DataBase.HISTORY_DYEING_CODE.TableName,
                c => My_DataBase.DyeingCodeData.Dyeing_code.Select($"{My_DataBase.DYEING_CODE.Code} = '{c}' AND {My_DataBase.DYEING_CODE.DyeingCode} IS NOT NULL")
                    .AsEnumerable().Select(r => r.Field<string>(My_DataBase.DYEING_CODE.DyeingCode)).Distinct().ToList(),
                dyeingCodeList =>
                {
                    string joinedCodes = string.Join("', '", dyeingCodeList.Select(s => s.Replace("'", "''")));
                    return My_DataBase.DyeingCodeData.Dyeing_code.Select($"{My_DataBase.DYEING_CODE.DyeingCode} IN ('{joinedCodes}')")
                        .AsEnumerable().Select(r => r.Field<string>(My_DataBase.DYEING_CODE.Code)).Distinct().ToList();
                }
            );
            if (!result) return;

            My_DataBase.SqlServer.Delete(My_DataBase.DYEING_CODE.TableName, $"{My_DataBase.DYEING_CODE.DyeingCode} = '{code}'");
            Logger.Info($"Btn_Delete_Click: 删除染固色工艺代码[{code}]及其组合。");

            ShowInfo("删除完成");
            DPPConfiguration_Load(sender, new EventArgs());
            if (currentRowIndex < dgv_Dyeing_Code.Rows.Count - 1)
            {
                dgv_Dyeing_Code.Rows[currentRowIndex].Selected = true;
                dgv_Dyeing_Code.CurrentCell = dgv_Dyeing_Code.Rows[currentRowIndex].Cells[0];
            }
            else
            {
                dgv_Dyeing_Code.Rows[dgv_Dyeing_Code.Rows.Count - 1].Selected = true;
                dgv_Dyeing_Code.CurrentCell = dgv_Dyeing_Code.Rows[dgv_Dyeing_Code.Rows.Count - 1].Cells[0];
            }



        }

        private void Tsmi_Copy_Click(object sender, EventArgs e)
        {
            var srcRow = dgv_Dyeing_Code.CurrentRow;
            if (srcRow == null || srcRow.IsNewRow)
            {
                ShowWarn("请先选择要复制的染固色工艺行！");
                return;
            }
            var srcCode = srcRow.Cells[COL_INDEX_CODE].Value?.ToString();

            Btn_Add_Click(sender, e);

            var newRow = dgv_Dyeing_Code.Rows[dgv_Dyeing_Code.Rows.Count - 1];
            dgv_Combination.Rows.Clear();
            if (!string.IsNullOrWhiteSpace(srcCode) && My_DataBase.DyeingCodeData.Dyeing_code != null)
            {
                var drs = My_DataBase.DyeingCodeData.Dyeing_code.AsEnumerable()
                    .Where(r => r.Field<string>(My_DataBase.DYEING_CODE.DyeingCode) == srcCode)
                    .OrderBy(r => r[My_DataBase.DYEING_CODE.IndexNum])
                    .ToList();
                int i = 0;
                foreach (var row in drs)
                {
                    dgv_Combination.Rows.Add(++i, row[My_DataBase.DYEING_CODE.Code]?.ToString() ?? "");
                }
            }
            newRow.Cells[COL_INDEX_CODE].Value = srcCode + "拷贝";
            dgv_Dyeing_Code.CurrentCell = newRow.Cells[COL_INDEX_CODE];
            dgv_Dyeing_Code.EndEdit();
            dgv_Dyeing_Code.BeginEdit(true);

            Logger.Info("Tsmi_Copy_Click: 触发新增并复制染固色工艺及组合，等待用户输入新工艺代码。");
        }

        private bool ValidateDyeingCodeSelection()
        {
            if (dgv_Dyeing_Code.SelectedRows == null)
            {
                ShowInfo("请先选择一个染固色工艺代码！");
                dgv_Dyeing_Code.BeginEdit(true);
                return false;
            }
            if (dgv_Dyeing_Code.SelectedCells.Count == 0)
            {
                ShowError("未选中任何单元格！");
                return false;
            }
            int selectedRowIndex = dgv_Dyeing_Code.SelectedCells[0].RowIndex;
            if (selectedRowIndex < 0 || selectedRowIndex >= dgv_Dyeing_Code.Rows.Count)
            {
                ShowError("选中行索引无效！");
                return false;
            }

            DataGridViewCell dgvCode = dgv_Dyeing_Code.Rows[selectedRowIndex].Cells[COL_INDEX_CODE];
            if (IsCellEmpty(dgvCode))
            {
                ShowError("染固色工艺代码不能为空！");
                dgv_Dyeing_Code.BeginEdit(true);
                return false;
            }

            return true;
        }

        private void ShowError(string msg) => LocalTranslator.ShowMessage(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowInfo(string msg) => LocalTranslator.ShowMessage(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void ShowWarn(string msg) => LocalTranslator.ShowMessage(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}