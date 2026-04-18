using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SmartColor.My_Control;
using SmartColor.My_File;

namespace SmartColor.My_Form.BasicData
{
    public partial class BrewingProcess : Form
    {
        

        // 魔法值常量
        private const string DefaultManualAddValue = "100";
        private const string DefaultSumBase = "100";

       
        private bool _isEditing = false;
        private bool _internalCodeSelectionChange = false;
        private bool _isLoading = false;
        private int _lastSelectedRowIndex = -1;
        private bool _suppressSelectionChanged = false;

        public BrewingProcess()
        {
            InitializeComponent();
            var font = new Font("宋体", 14.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this. dgv_BrewCode.Font = font;
            this.dgv_BrewProcess.Font = font;

            // 优化：启用自定义编辑模式和回车跳转
            this.dgv_BrewProcess.CustomEditing = true;
            this.dgv_BrewProcess.EnterKeyAction += Dgv_BrewProcess_EnterKeyAction;

            this.dgv_BrewCode.CellBeginEdit += (s, e) => { if (e.RowIndex != dgv_BrewCode.Rows.Count - 1) e.Cancel = true; };
            this.dgv_BrewCode.SelectionChanged += Dgv_BrewCode_SelectionChanged;
            this.dgv_BrewCode.CustomEditing = true;
            this.dgv_BrewCode.EnterKeyAction += Dgv_BrewCode_EnterKeyAction;

            this.dgv_BrewProcess.KeyDown += Dgv_BrewProcess_KeyDown;
            this.dgv_BrewProcess.RowsAdded += (s, e) => ReorderStepNum();
            this.dgv_BrewProcess.RowsRemoved += (s, e) => ReorderStepNum();
            this.Load += BrewingProcess_Load;
        }

        private bool Dgv_BrewCode_EnterKeyAction()
        {
            if (!_isEditing)
                return false;

            var dgv = dgv_BrewCode;
            dgv.EndEdit();
            if (dgv.CurrentCell == null) return false;
            int col = dgv.CurrentCell.ColumnIndex;
            int row = dgv.CurrentCell.RowIndex;

            void SmartJump()
            {
                if (dgv.Rows.Count == 0) return;
                if (row < 0 || row >= dgv.Rows.Count) return;
                var cellValue = dgv.CurrentRow.Cells[0].Value;
                // 只有一列，回车时校验并跳转到明细编辑
                if (string.IsNullOrWhiteSpace(cellValue?.ToString()))
                {
                    ShowError("泡制流程代码不能为空！");
                    if (!dgv.CurrentRow.IsNewRow)
                        dgv.Rows.RemoveAt(dgv.CurrentRow.Index);
                    _internalCodeSelectionChange = false;
                    _isEditing = false;
                    dgv.ReadOnly = true;
                    return;
                }
                // 检查是否重复
                string code = cellValue.ToString().Trim();
                string filter = $"{My_DataBase.BREWING_CODE.BrewingCode} = '{code.Replace("'", "''")}'";
                if (My_DataBase.BrewData.Brewing_code != null && My_DataBase.BrewData.Brewing_code.Select(filter).Length > 0)
                {
                    ShowError("泡制流程代码已存在，不能重复！");
                    dgv.BeginEdit(true);
                    return;
                }
                // 跳转到明细表编辑
                dgv_BrewProcess.ReadOnly = false;
                if (dgv_BrewProcess.Rows.Count == 0)
                    dgv_BrewProcess.Rows.Add();
                dgv_BrewProcess.CurrentCell = dgv_BrewProcess.Rows[0].Cells[1];
                dgv_BrewProcess.BeginEdit(true);
            }
            this.BeginInvoke(new Action(SmartJump));
            return true;
        }

        // 优化：自定义回车跳转行为
        private bool Dgv_BrewProcess_EnterKeyAction()
        {
            if (_isEditing)
            {
                FocusBrewProcessAndEdit();
                return true;
            }
            return false;
        }

        private void BrewingProcess_Load(object sender, EventArgs e)
        {
            try
            {
                Logger.Info("BrewingProcess_Load: 开始加载泡制流程代码表。");
                _isLoading = true;
                dgv_BrewCode.Rows.Clear();
                if (My_DataBase.BrewData.Brewing_code == null)
                {
                    Logger.Info("BrewingProcess_Load: Brewing_code为空，跳过加载。");
                    return;
                }
                // 优化：使用 CtDataGridView 的 BindDataTable 方法进行表格绑定
                dgv_BrewCode.BindDataTable(
                    My_DataBase.BrewData.Brewing_code,
                    row => new object[] { row[My_DataBase.BREWING_CODE.BrewingCode] },
                    My_DataBase.BREWING_CODE.BrewingCode
                );
                dgv_BrewCode.ClearSelection();
                dgv_BrewProcess.Rows.Clear();
                _isLoading = false;
                Logger.Info("BrewingProcess_Load: 加载完成。");

            }
            catch (Exception ex)
            {
                Logger.Error("BrewingProcess_Load: 加载泡制流程代码表异常。", ex);
            }
        }

        private void Dgv_BrewCode_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                int prevRowIndex = _lastSelectedRowIndex;
                if (dgv_BrewCode.CurrentCell != null)
                    _lastSelectedRowIndex = dgv_BrewCode.CurrentCell.RowIndex;
                else if (dgv_BrewCode.SelectedCells.Count > 0)
                    _lastSelectedRowIndex = dgv_BrewCode.SelectedCells[0].RowIndex;
                else
                    _lastSelectedRowIndex = -1;
                if (_isLoading || _suppressSelectionChanged) return;
                if (_isEditing)
                {
                    bool hasUnsaved = false;
                    DataGridViewRow prevRow = (prevRowIndex >= 0 && prevRowIndex < dgv_BrewCode.Rows.Count)
                        ? dgv_BrewCode.Rows[prevRowIndex]
                        : null;

                    if (prevRow != null)
                    {
                        var codeCell = prevRow.Cells[0];
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
                            _suppressSelectionChanged = true;
                            if (prevRow != null)
                            {
                                prevRow.Selected = true;
                                _lastSelectedRowIndex = prevRow.Index;
                            }

                            _suppressSelectionChanged = false;
                            return;
                        }
                    }

                    // 放弃修改，清理编辑状态
                    _suppressSelectionChanged = true;
                    dgv_BrewCode.EndEdit();

                    if (prevRow != null)
                    {
                        if (dgv_BrewCode.Rows.Count > My_DataBase.BrewData.Brewing_code.Rows.Count)
                        {
                            // 新增行未保存，删除该行
                            if (prevRow.Index == dgv_BrewCode.Rows.Count - 1)
                            {
                                dgv_BrewCode.Rows.RemoveAt(prevRow.Index);
                            }
                        }
                    }

                    _isEditing = false;
                    dgv_BrewCode.ReadOnly = true;
                    dgv_BrewProcess.ReadOnly = true;
                    _suppressSelectionChanged = false;
                    dgv_BrewCode.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                }

                if (!_internalCodeSelectionChange)
                    _isEditing = false;

                dgv_BrewProcess.Rows.Clear();

                if (dgv_BrewCode.SelectedRows == null || dgv_BrewCode.SelectedRows.Count == 0)
                    return;

                var codeValue = dgv_BrewCode.SelectedRows[0].Cells[0].Value;
                if (codeValue == null || codeValue == DBNull.Value || My_DataBase.BrewData.Brewing_process == null)
                    return;

                string filter = BuildFilter(My_DataBase.BrewData.Brewing_process, codeValue, My_DataBase.BREWING_CODE.BrewingCode);

                DataRow[] drs = My_DataBase.BrewData.Brewing_process.Select(filter);

                var ordered = drs
                    .Where(r => r[My_DataBase.BREWING_PROCESS.StepNum] != DBNull.Value)
                    .OrderBy(r => Convert.ToInt32(r[My_DataBase.BREWING_PROCESS.StepNum]));

                foreach (DataRow row in ordered)
                {
                    string sn = row[My_DataBase.BREWING_PROCESS.StepNum] == DBNull.Value ? "" : row[My_DataBase.BREWING_PROCESS.StepNum].ToString();
                    string tn = row[My_DataBase.BREWING_PROCESS.TechnologyName] == DBNull.Value ? "" : row[My_DataBase.BREWING_PROCESS.TechnologyName].ToString();
                    string pt = row[My_DataBase.BREWING_PROCESS.ProportionOrTime] == DBNull.Value ? "" : row[My_DataBase.BREWING_PROCESS.ProportionOrTime].ToString();
                    string rt = row[My_DataBase.BREWING_PROCESS.Ratio] == DBNull.Value ? "" : row[My_DataBase.BREWING_PROCESS.Ratio].ToString();
                    dgv_BrewProcess.Rows.Add(sn, tn, pt, rt);
                }
                dgv_BrewProcess.ClearSelection();
                Logger.Info($"dgv_BrewCode_SelectionChanged: 加载流程详情，代码={codeValue}");
            }
            catch (Exception ex)
            {
                Logger.Error("dgv_BrewCode_SelectionChanged: 加载流程详情异常。", ex);
            }
        }

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isEditing) return;
                _suppressSelectionChanged = true;
                _internalCodeSelectionChange = true;
                _isEditing = true;
                dgv_BrewProcess.CustomEditing = _isEditing;
                dgv_BrewCode.ReadOnly = false;
                dgv_BrewCode.Rows.Add();
                int newRowIdx = dgv_BrewCode.Rows.Count - 1;
                dgv_BrewCode.CurrentCell = dgv_BrewCode.Rows[newRowIdx].Cells[0];
                _lastSelectedRowIndex = newRowIdx;
                dgv_BrewCode.BeginEdit(true);
                _internalCodeSelectionChange = false;
                _suppressSelectionChanged = false;
                dgv_BrewProcess.Rows.Clear();
                Logger.Info("btn_Add_Click: 新增泡制流程代码行。");
            }
            catch (Exception ex)
            {
                Logger.Error("btn_Add_Click: 新增泡制流程代码异常。", ex);
            }
        }

        private void Btn_Change_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv_BrewCode.SelectedRows == null || dgv_BrewCode.SelectedRows.Count == 0)
                {
                    ShowWarn("请先选择一个泡制流程代码进行修改！");
                    Logger.Info("btn_Change_Click: 未选择泡制流程代码。");
                    return;
                }

                _isEditing = true;
                dgv_BrewProcess.CustomEditing = _isEditing;
                dgv_BrewProcess.ReadOnly = false;
                if (dgv_BrewProcess.Rows.Count > 0)
                {
                    dgv_BrewProcess.CurrentCell = dgv_BrewProcess.Rows[0].Cells[1];
                    dgv_BrewProcess.BeginEdit(true);
                }
                else
                {
                    dgv_BrewProcess.Rows.Add();
                    dgv_BrewProcess.CurrentCell = dgv_BrewProcess.Rows[0].Cells[1];
                    dgv_BrewProcess.BeginEdit(true);
                }
                Logger.Info("btn_Change_Click: 进入编辑模式。");
            }
            catch (Exception ex)
            {
                Logger.Error("btn_Change_Click: 进入编辑模式异常。", ex);
            }
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            _isEditing = false;
            dgv_BrewProcess.CustomEditing = _isEditing;

            if (dgv_BrewCode.SelectedRows == null || dgv_BrewCode.SelectedRows.Count == 0)
            {
                ShowWarn("请先选择一个泡制流程代码进行保存！");
                dgv_BrewCode.BeginEdit(true);
                Logger.Info("btn_Save_Click: 未选择泡制流程代码。");
                return;
            }
            if (dgv_BrewCode.SelectedCells.Count == 0)
            {
                ShowError("未选中任何单元格！");
                Logger.Info("btn_Save_Click: 未选中任何单元格。");
                return;
            }
            var dgvCode = dgv_BrewCode.SelectedCells[0];
            if (dgvCode.Value == null || string.IsNullOrWhiteSpace(dgvCode.Value.ToString()))
            {
                ShowError("泡制流程代码不能为空！");
                dgv_BrewCode.BeginEdit(true);
                Logger.Info("btn_Save_Click: 泡制流程代码为空。");
                return;
            }


            // 新增：保存前检查所有“加水比例(%)”的和不能超过100
            double waterRatioSum = 0;
            foreach (DataGridViewRow row in dgv_BrewProcess.Rows)
            {
                if (row.IsNewRow) continue;
                var techName = row.Cells[1].Value?.ToString();
                var ratioStr = row.Cells[2].Value?.ToString();
                // 只检查“加大冷水”、“加小冷水”、“加热水”、“加温水”
                if (techName == "加大冷水" || techName == "加小冷水" || techName == "加热水" || techName == "加温水")
                {
                    if (!string.IsNullOrWhiteSpace(ratioStr) && double.TryParse(ratioStr, out double ratio))
                    {
                        waterRatioSum += ratio;
                    }
                }
            }
            if (waterRatioSum > 100)
            {
                ShowError("所有加水比例(%)之和不能超过100！");
                return;
            }

            string newCode = dgvCode.Value.ToString().Trim();

            string filter = BuildFilter(My_DataBase.BrewData.Brewing_code, newCode, My_DataBase.BREWING_CODE.BrewingCode);
            DataRow[] existing = My_DataBase.BrewData.Brewing_code.Select(filter);
            try
            {
                if (existing.Length > 0)
                {
                    if (existing.Length != 1)
                    {
                        ShowError("泡制流程代码已存在，不能重复！");
                        dgv_BrewCode.BeginEdit(true);
                        Logger.Info($"btn_Save_Click: 代码[{newCode}]已存在。");
                        return;
                    }
                    else
                    {
                        My_DataBase.SqlServer.Delete(My_DataBase.BREWING_CODE.TableName, filter);
                        My_DataBase.SqlServer.Delete(My_DataBase.BREWING_PROCESS.TableName, filter);
                        Logger.Info($"btn_Save_Click: 删除原有代码[{newCode}]及其流程。");
                    }
                }

                var codeData = new Dictionary<string, object> { { My_DataBase.BREWING_CODE.BrewingCode, newCode } };
                My_DataBase.SqlServer.Insert(My_DataBase.BREWING_CODE.TableName, codeData);

                for (int i = dgv_BrewProcess.Rows.Count - 1; i >= 0; i--)
                {
                    var row = dgv_BrewProcess.Rows[i];
                    if (!row.IsNewRow && (row.Cells[1].Value == null || string.IsNullOrWhiteSpace(row.Cells[1].Value.ToString())))
                        dgv_BrewProcess.Rows.RemoveAt(i);
                }

                foreach (DataGridViewRow row in dgv_BrewProcess.Rows)
                {
                    if (row.IsNewRow) continue;
                    string sn = row.Cells[0].Value?.ToString().Trim();
                    string tn = row.Cells[1].Value?.ToString().Trim();
                    string pt = row.Cells[2].Value?.ToString().Trim();
                    string rt = row.Cells[3].Value?.ToString().Trim();
                    if (string.IsNullOrWhiteSpace(tn)) continue;

                    var processData = new Dictionary<string, object>
                    {
                        { My_DataBase.BREWING_PROCESS.BrewingCode, newCode },
                        { My_DataBase.BREWING_PROCESS.StepNum, string.IsNullOrWhiteSpace(sn) ? DBNull.Value : (object)Convert.ToInt32(sn) },
                        { My_DataBase.BREWING_PROCESS.TechnologyName, tn },
                        { My_DataBase.BREWING_PROCESS.ProportionOrTime, string.IsNullOrWhiteSpace(pt) ? DBNull.Value : (object)pt },
                        { My_DataBase.BREWING_PROCESS.Ratio, string.IsNullOrWhiteSpace(rt) ? DBNull.Value : (object)rt }
                    };
                    My_DataBase.SqlServer.Insert(My_DataBase.BREWING_PROCESS.TableName, processData);
                }


                Logger.Info($"btn_Save_Click: 保存代码[{newCode}]及流程成功。");
            }
            catch (Exception ex)
            {
                Logger.Error($"btn_Save_Click: 保存代码[{newCode}]异常。", ex);
                return;
            }

            dgv_BrewProcess.ReadOnly = true;
            dgv_BrewCode.ReadOnly = true;

            ShowInfo("保存完成");

            BrewingProcess_Load(sender, new EventArgs());
            foreach (DataGridViewRow row in dgv_BrewCode.Rows)
            {
                if (row.Cells[0].Value?.ToString() == newCode)
                {
                    row.Selected = true;

                    dgv_BrewCode.CurrentCell = row.Cells[0];
                    break;
                }
            }

            
        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            if (dgv_BrewCode.SelectedRows == null || dgv_BrewCode.SelectedRows.Count == 0)
            {
                ShowWarn("请先选择一个泡制流程代码进行删除！");
                Logger.Info("btn_Delete_Click: 未选择泡制流程代码。");
                return;
            }
            if (dgv_BrewCode.SelectedCells.Count == 0)
            {
                ShowError("未选中任何单元格！");
                Logger.Info("btn_Delete_Click: 未选中任何单元格。");
                return;
            }


            var dgvCode = dgv_BrewCode.SelectedCells[0];

            int currentRowIndex = dgv_BrewCode.CurrentCell.RowIndex;
            string newCode = dgvCode.Value?.ToString().Trim();
            if (string.IsNullOrEmpty(newCode)) return;

            var dataRow = My_DataBase.BrewData.Brewing_code.Select(BuildFilter(My_DataBase.BrewData.Brewing_code, newCode, My_DataBase.BREWING_CODE.BrewingCode));
            if (dataRow.Length == 0)
            {
                ShowError("未找到对应的泡制流程代码记录，无法删除！");
                return;
            }

            string filter = BuildFilter(My_DataBase.BrewData.Brewing_code, newCode, My_DataBase.BREWING_CODE.BrewingCode);
            try
            {
                DataRow[] existing = My_DataBase.BottleData.Bottle_details.Select(filter);
                if (existing.Length > 0)
                {
                    ShowInfo($"当前泡制流程代码有{existing.Length}个母液瓶正在使用，请先修改后再删除");
                    Logger.Info($"btn_Delete_Click: 代码[{newCode}]被母液瓶引用，禁止删除。");
                    return;
                }

                DialogResult dialogResult = ShowConfirm("确定删除吗？");
                if (dialogResult == DialogResult.Yes)
                {
                    My_DataBase.SqlServer.Delete(My_DataBase.BREWING_CODE.TableName, filter);
                    My_DataBase.SqlServer.Delete(My_DataBase.BREWING_PROCESS.TableName, filter);
                    Logger.Info($"btn_Delete_Click: 删除代码[{newCode}]及流程。");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"btn_Delete_Click: 删除代码[{newCode}]异常。", ex);
                return;
            }

            ShowInfo("删除完成");
            BrewingProcess_Load(sender, new EventArgs());
            if (currentRowIndex < dgv_BrewCode.Rows.Count - 1)
            {
                dgv_BrewCode.Rows[currentRowIndex].Selected = true;
                dgv_BrewCode.CurrentCell = dgv_BrewCode.Rows[currentRowIndex].Cells[0];
            }
            else
            {
                dgv_BrewCode.Rows[dgv_BrewCode.Rows.Count - 1].Selected = true;
                dgv_BrewCode.CurrentCell = dgv_BrewCode.Rows[dgv_BrewCode.Rows.Count - 1].Cells[0];
            }
           
        }

        private void ReorderStepNum()
        {
            int step = 1;
            foreach (DataGridViewRow row in dgv_BrewProcess.Rows)
            {
                if (row.IsNewRow) continue;
                row.Cells[0].Value = step++;
            }
        }

        private void Dgv_BrewProcess_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_isEditing) return;

            var dgv = dgv_BrewProcess;

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

        private void FocusBrewProcessAndEdit()
        {
            var dgv = dgv_BrewProcess;
            dgv.EndEdit();
            if (dgv.CurrentCell == null)
                return;

            int col = dgv.CurrentCell.ColumnIndex;
            int row = dgv.CurrentCell.RowIndex;
            int colCount = dgv.ColumnCount;
            int techCol = 1;

            string techName = dgv.Rows[row].Cells[techCol].Value?.ToString();

            void JumpTo(int targetRow, int targetCol)
            {
                _suppressSelectionChanged = true;
                dgv.CurrentCell = dgv.Rows[targetRow].Cells[targetCol];
                dgv.BeginEdit(true);
                _suppressSelectionChanged = false;
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
                if (row < 0 || row >= dgv.Rows.Count)
                    return;

                var techCellValue = dgv.Rows[row].Cells[techCol].Value;
                if (techCellValue == null || string.IsNullOrWhiteSpace(techCellValue.ToString()))
                {
                    btn_Save.Focus();
                    return;
                }

                if (col == 1)
                {
                    if (techName == "手动加染助剂")
                    {
                        dgv.Rows[row].Cells[2].Value = DefaultManualAddValue;
                        EnsureNextRow(row);
                        JumpTo(row + 1, techCol);
                        return;
                    }
                    if (techName == "加小冷水")
                    {
                        double sum = 0;
                        for (int i = 0; i < row; i++)
                        {
                            var tName = dgv.Rows[i].Cells[techCol].Value?.ToString();
                            if (tName == "搅拌" || tName == "手动加染助剂") continue;
                            var valStr = dgv.Rows[i].Cells[2].Value?.ToString();
                            if (double.TryParse(valStr, out double val))
                                sum += val;
                        }
                        double result = double.Parse(DefaultSumBase) - sum;
                        if (result < 0) result = 0;
                        dgv.Rows[row].Cells[2].Value = result.ToString("0.##");
                        EnsureNextRow(row);
                        JumpTo(row + 1, techCol);
                        return;
                    }
                    JumpTo(row, col + 1);
                    return;
                }

                if (col == 2)
                {
                    if (techName == "加温水")
                    {
                        JumpTo(row, col + 1);
                        return;
                    }
                    var val = dgv.Rows[row].Cells[col].Value;
                    if (val == null || string.IsNullOrWhiteSpace(val.ToString()))
                    {
                        ShowWarn("该列不能为空！");
                        dgv.BeginEdit(true);
                        return;
                    }
                    EnsureNextRow(row);
                    JumpTo(row + 1, techCol);
                    return;
                }

                if (col < colCount - 1)
                {
                    JumpTo(row, col + 1);
                    return;
                }

                if (techName == "加温水")
                {
                    var val = dgv.Rows[row].Cells[col].Value;
                    if (val == null || string.IsNullOrWhiteSpace(val.ToString()))
                    {
                        ShowWarn("该列不能为空！");
                        dgv.BeginEdit(true);
                        return;
                    }
                }
                EnsureNextRow(row);
                JumpTo(row + 1, techCol);
            }

            this.BeginInvoke(new Action(SmartJump));
        }



        // 通用过滤条件构建
        private string BuildFilter(DataTable table, object codeValue, string colName)
        {
            bool isString = table.Columns[colName].DataType == typeof(string);
            string codeStr = codeValue.ToString().Replace("'", "''");
            return isString
                ? $"{colName} = '{codeStr}'"
                : $"{colName} = {codeStr}";
        }

        /// <summary>
        /// 判断单元格是否为空
        /// </summary>
        private bool IsCellEmpty(DataGridViewCell cell)
        {
            return cell == null || string.IsNullOrWhiteSpace(Convert.ToString(cell.Value));
        }

        private void Tsmi_Copy_Click(object sender, EventArgs e)
        {
            var srcRow = dgv_BrewCode.CurrentRow;
            if (srcRow == null || srcRow.IsNewRow)
            {
                ShowWarn("请先选择要复制的泡制流程代码行！");
                return;
            }
            var srcCode = srcRow.Cells[0].Value?.ToString();

            // 触发新增
            Btn_Add_Click(sender, e);

            var newRow = dgv_BrewCode.Rows[dgv_BrewCode.Rows.Count - 1];

            // 复制明细
            dgv_BrewProcess.Rows.Clear();
            if (!string.IsNullOrWhiteSpace(srcCode) && My_DataBase.BrewData.Brewing_process != null)
            {
                string filter = BuildFilter(My_DataBase.BrewData.Brewing_process, srcCode, My_DataBase.BREWING_PROCESS.BrewingCode);
                DataRow[] drs = My_DataBase.BrewData.Brewing_process.Select(filter);
                var ordered = drs
                    .Where(r => r[My_DataBase.BREWING_PROCESS.StepNum] != DBNull.Value)
                    .OrderBy(r => Convert.ToInt32(r[My_DataBase.BREWING_PROCESS.StepNum]));
                foreach (DataRow row in ordered)
                {
                    string sn = row[My_DataBase.BREWING_PROCESS.StepNum] == DBNull.Value ? "" : row[My_DataBase.BREWING_PROCESS.StepNum].ToString();
                    string tn = row[My_DataBase.BREWING_PROCESS.TechnologyName] == DBNull.Value ? "" : row[My_DataBase.BREWING_PROCESS.TechnologyName].ToString();
                    string pt = row[My_DataBase.BREWING_PROCESS.ProportionOrTime] == DBNull.Value ? "" : row[My_DataBase.BREWING_PROCESS.ProportionOrTime].ToString();
                    string rt = row[My_DataBase.BREWING_PROCESS.Ratio] == DBNull.Value ? "" : row[My_DataBase.BREWING_PROCESS.Ratio].ToString();
                    dgv_BrewProcess.Rows.Add(sn, tn, pt, rt);
                }
            }
            // 新行工艺代码自动加“拷贝”后缀
            newRow.Cells[0].Value = srcCode + "拷贝";
            dgv_BrewCode.CurrentCell = newRow.Cells[0];
            dgv_BrewCode.EndEdit();
            dgv_BrewCode.BeginEdit(true);

            Logger.Info("Tsmi_Copy_Click: 触发新增并复制泡制流程及明细，等待用户输入新流程代码。");
        }
        // 统一消息提示
        private void ShowError(string msg) => LocalTranslator.ShowMessage(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowInfo(string msg) => LocalTranslator.ShowMessage(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void ShowWarn(string msg) => LocalTranslator.ShowMessage(msg, "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        private DialogResult ShowConfirm(string msg) => LocalTranslator.ShowMessage(msg, "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


    }
}