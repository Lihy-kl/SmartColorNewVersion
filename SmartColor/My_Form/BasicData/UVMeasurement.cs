using SmartColor.My_File;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Form.BasicData
{
    /// <summary>
    /// 吸光度测量流程管理界面
    /// </summary>
    public partial class UVMeasurement : Form
    {
        

        // 编辑状态标记
        private bool _isEditing = false; // 是否处于编辑模式
        private bool _isLoading = false; // 是否正在加载数据
        private int _lastSelectedRowIndex = -1; // 上一次选中的流程代码行索引
        private bool _suppressSelectionChanged = false; // 是否抑制选中变化事件
        private bool _internalCodeSelectionChange = false; // 内部流程代码选中变化标记

        /// <summary>
        /// 构造函数，初始化界面及事件绑定
        /// </summary>
        public UVMeasurement()
        {
            InitializeComponent();

            // 设置表格字体
            var font = new Font("宋体", 14.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.dgv_Code.Font = font;
            this.dgv_Process.Font = font;

            // 启用自定义编辑模式和回车跳转
            this.dgv_Process.CustomEditing = true;
            this.dgv_Code.CustomEditing = true;

            // 仅允许最后一行编辑流程代码
            this.dgv_Code.CellBeginEdit += (s, e) => { if (e.RowIndex != dgv_Code.Rows.Count - 1) e.Cancel = true; };
            this.dgv_Code.SelectionChanged += Dgv_Code_SelectionChanged;

            // 流程明细表快捷键处理
            this.dgv_Process.KeyDown += Dgv_Process_KeyDown;
            this.dgv_Process.RowsAdded += (s, e) => ReorderStepNum();
            this.dgv_Process.RowsRemoved += (s, e) => ReorderStepNum();

            // 回车跳转事件
            this.dgv_Code.EnterKeyAction += Dgv_Code_EnterKeyAction;
            this.dgv_Process.EnterKeyAction += Dgv_Process_EnterKeyAction;

            // 按钮事件绑定
            this.btn_Add.Click += Btn_Add_Click;
            this.btn_Change.Click += Btn_Change_Click;
            this.btn_Save.Click += Btn_Save_Click;
            this.btn_Delete.Click += Btn_Delete_Click;
            this.Tsmi_Copy.Click += Tsmi_Copy_Click;

            // 界面加载事件
            this.Load += UVMeasurement_Load;
        }

        /// <summary>
        /// 流程明细表回车跳转逻辑
        /// </summary>
        private bool Dgv_Process_EnterKeyAction()
        {
            if (_isEditing)
            {
                FocusProcessAndEdit();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 智能跳转流程明细表必填项
        /// </summary>
        private void FocusProcessAndEdit()
        {
            var dgv = dgv_Process;
            dgv.EndEdit();
            if (dgv.CurrentCell == null)
                return;

            int row = dgv.CurrentCell.RowIndex;
            int col = dgv.CurrentCell.ColumnIndex;
            int techCol = 1;
            string techName = dgv.Rows[row].Cells[techCol].Value?.ToString();

            var techCellValue = dgv.Rows[row].Cells[techCol].Value;
            if (techCellValue == null || string.IsNullOrWhiteSpace(techCellValue.ToString()))
            {
                btn_Save.Focus();
                return;
            }

            // 定义每种工艺的必填列索引
            Dictionary<string, int[]> requiredCols = new Dictionary<string, int[]>
            {
                { "加药", new[] { 1,2, 7, 8, 9, 10 } },
                { "加水", new[] { 1, 2 } },
                { "抽染液", new int[] { 1, } },
                { "搅拌", new[] { 1, 2, 3 } },
                { "排液", new[] { 1, 2, 4, 5, 6 } },
                { "测吸光度", new[] { 1, 6 } },
                { "加溶解剂", new[] { 1, 2, 10 } }
            };

            // 跳转到指定单元格并进入编辑
            void JumpTo(int targetRow, int targetCol)
            {
                _suppressSelectionChanged = true;
                dgv.CurrentCell = dgv.Rows[targetRow].Cells[targetCol];
                dgv.BeginEdit(true);
                _suppressSelectionChanged = false;
            }

            // 保证下一行存在
            void EnsureNextRow(int currentRow)
            {
                if (currentRow == dgv.Rows.Count - 1 || dgv.Rows[currentRow + 1].IsNewRow)
                {
                    dgv.Rows.Add();
                    ReorderStepNum();
                }
            }

            // 智能跳转逻辑
            void SmartJump()
            {
                if (row < 0 || row >= dgv.Rows.Count)
                    return;

                var mustCols = (!string.IsNullOrWhiteSpace(techName) && requiredCols.ContainsKey(techName)) ? requiredCols[techName] : null;

                // 当前单元格为必填项且未填写，提示并停留
                if (mustCols != null && mustCols.Contains(col))
                {
                    var cell = dgv.Rows[row].Cells[col];
                    if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        ShowWarn($"[{dgv.Columns[col].HeaderText}]为必填项，请填写！");
                        dgv.CurrentCell = cell;
                        dgv.BeginEdit(true);
                        return;
                    }
                }

                // 跳转到下一个必填项或下一行
                if (mustCols != null && mustCols.Length > 0)
                {
                    int idxInRequired = Array.IndexOf(mustCols, col);
                    if (idxInRequired < mustCols.Length - 1 && idxInRequired >= 0)
                    {
                        JumpTo(row, mustCols[idxInRequired + 1]);
                        return;
                    }
                    else
                    {
                        EnsureNextRow(row);
                        int nextRow = row + 1;
                        var nextTechName = dgv.Rows[nextRow].Cells[techCol].Value?.ToString();
                        if (string.IsNullOrWhiteSpace(nextTechName) || !requiredCols.ContainsKey(nextTechName))
                        {
                            JumpTo(nextRow, techCol);
                        }
                        else
                        {
                            var nextMustCols = requiredCols[nextTechName];
                            if (nextMustCols.Length > 0)
                                JumpTo(nextRow, nextMustCols[0]);
                            else
                                JumpTo(nextRow, techCol);
                        }
                        return;
                    }
                }
                else
                {
                    EnsureNextRow(row);
                    JumpTo(row + 1, techCol);
                }
            }

            this.BeginInvoke(new Action(SmartJump));
        }

        /// <summary>
        /// 流程代码表回车跳转逻辑
        /// </summary>
        private bool Dgv_Code_EnterKeyAction()
        {
            if (!_isEditing)
                return false;

            var dgv = dgv_Code;
            dgv.EndEdit();
            if (dgv.CurrentCell == null) return false;
            int col = dgv.CurrentCell.ColumnIndex;
            int row = dgv.CurrentCell.RowIndex;

            void SmartJump()
            {
                if (dgv.Rows.Count == 0) return;
                if (row < 0 || row >= dgv.Rows.Count) return;
                var cellValue = dgv.CurrentRow.Cells[0].Value;
                // 校验流程代码不能为空
                if (string.IsNullOrWhiteSpace(cellValue?.ToString()))
                {
                    ShowError("吸光度测量流程代码不能为空！");
                    if (!dgv.CurrentRow.IsNewRow)
                        dgv.Rows.RemoveAt(dgv.CurrentRow.Index);
                    _internalCodeSelectionChange = false;
                    _isEditing = false;
                    dgv.ReadOnly = true;
                    return;
                }
                // 校验流程代码不能重复
                string code = cellValue.ToString().Trim();
                string filter = $"{My_DataBase.ABS_PROCESS.Code} = '{code.Replace("'", "''")}'";
                if (My_DataBase.ABSProcess.ABS_Process != null && My_DataBase.ABSProcess.ABS_Process.Select(filter).Length > 0)
                {
                    ShowError("吸光度测量流程代码已存在，不能重复！");
                    dgv.BeginEdit(true);
                    return;
                }
                // 跳转到明细表编辑
                dgv_Process.ReadOnly = false;
                if (dgv_Process.Rows.Count == 0)
                    dgv_Process.Rows.Add();
                dgv_Process.CurrentCell = dgv_Process.Rows[0].Cells[1];
                dgv_Process.BeginEdit(true);
            }
            this.BeginInvoke(new Action(SmartJump));
            return true;
        }

        /// <summary>
        /// 界面加载时，刷新流程代码列表
        /// </summary>
        private void UVMeasurement_Load(object sender, EventArgs e)
        {
            try
            {
                _isLoading = true;
                dgv_Code.Rows.Clear();

               
                var dt = SmartColor.My_DataBase.ABSProcess.ABS_Process;
                if (dt == null)
                    return;

                // 只显示唯一的流程代码（Code），去重
                var codes = dt.AsEnumerable()
                    .Select(r => r[SmartColor.My_DataBase.ABS_PROCESS.Code]?.ToString())
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .ToList();

                foreach (var code in codes)
                {
                    dgv_Code.Rows.Add(code);
                }
                dgv_Code.ClearSelection();
                dgv_Process.Rows.Clear();
                _isLoading = false;
            }
            catch (Exception ex)
            {
                Logger.Error("加载数据异常", ex);
            }
        }

        /// <summary>
        /// 流程代码选中变化时，加载对应流程明细，并处理未保存数据提示
        /// </summary>
        private void Dgv_Code_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                int prevRowIndex = _lastSelectedRowIndex;
                if (dgv_Code.CurrentCell != null)
                    _lastSelectedRowIndex = dgv_Code.CurrentCell.RowIndex;
                else if (dgv_Code.SelectedCells.Count > 0)
                    _lastSelectedRowIndex = dgv_Code.SelectedCells[0].RowIndex;
                else
                    _lastSelectedRowIndex = -1;

                if (_isLoading || _suppressSelectionChanged) return;
                if (_isEditing)
                {
                    HandleUnsavedEdit(prevRowIndex);
                }

                if (!_internalCodeSelectionChange)
                    _isEditing = false;

                dgv_Process.Rows.Clear();

                if (dgv_Code.SelectedRows == null || dgv_Code.SelectedRows.Count == 0)
                    return;

                var codeValue = dgv_Code.SelectedRows[0].Cells[0].Value;
                if (codeValue == null || SmartColor.My_DataBase.ABSProcess.ABS_Process == null)
                    return;

                // 过滤出所有该Code的流程步骤
                string filter = $"{SmartColor.My_DataBase.ABS_PROCESS.Code} = '{codeValue.ToString().Replace("'", "''")}'";
                DataRow[] drs = SmartColor.My_DataBase.ABSProcess.ABS_Process.Select(filter);

                // 按步号排序
                var ordered = drs
                    .Where(r => r[SmartColor.My_DataBase.ABS_PROCESS.StepNum] != DBNull.Value)
                    .OrderBy(r => Convert.ToInt32(r[SmartColor.My_DataBase.ABS_PROCESS.StepNum]));

                var comboCol = dgv_Process.Columns[1] as DataGridViewComboBoxColumn;
                var comboItems = comboCol.Items.Cast<object>().ToList();
                dgv_Process.Columns[1].ReadOnly = false;
                foreach (DataRow row in ordered)
                {
                    dgv_Process.Rows.Add(
                         row[SmartColor.My_DataBase.ABS_PROCESS.StepNum]?.ToString(),
                         row[SmartColor.My_DataBase.ABS_PROCESS.TechnologyName]?.ToString(),
                         row[SmartColor.My_DataBase.ABS_PROCESS.StirringRate]?.ToString(),
                         row[SmartColor.My_DataBase.ABS_PROCESS.StirringTime]?.ToString(),
                         row[SmartColor.My_DataBase.ABS_PROCESS.DrainTime]?.ToString(),
                         row[SmartColor.My_DataBase.ABS_PROCESS.ParallelizingDishTime]?.ToString(),
                         row[SmartColor.My_DataBase.ABS_PROCESS.PumpingTime]?.ToString(),
                         row[SmartColor.My_DataBase.ABS_PROCESS.StartingWavelength]?.ToString(),
                         row[SmartColor.My_DataBase.ABS_PROCESS.EndWavelength]?.ToString(),
                         row[SmartColor.My_DataBase.ABS_PROCESS.WavelengthInterval]?.ToString(),
                         row[SmartColor.My_DataBase.ABS_PROCESS.Dosage]?.ToString()
                     );
                }
                dgv_Process.Columns[1].ReadOnly = true;
                dgv_Process.ClearSelection();
            }
            catch (Exception ex)
            {
                Logger.Error("选中数据异常", ex);
            }
        }

        /// <summary>
        /// 新增流程代码及明细
        /// </summary>
        private void Btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                if(_isEditing)
                {
                    return;
                }
                _suppressSelectionChanged = true;
                _internalCodeSelectionChange = true;
                _isEditing = true;
                dgv_Code.ReadOnly = false;
                dgv_Code.Rows.Add();
                int newRowIdx = dgv_Code.Rows.Count - 1;
                dgv_Code.CurrentCell = dgv_Code.Rows[newRowIdx].Cells[0];
                _lastSelectedRowIndex = newRowIdx;
                dgv_Code.BeginEdit(true);
                _suppressSelectionChanged = false;
                _internalCodeSelectionChange = false;
                dgv_Process.Rows.Clear();
            }
            catch (Exception ex)
            {
                Logger.Error("新增异常", ex);
            }
        }

        /// <summary>
        /// 修改流程明细
        /// </summary>
        private void Btn_Change_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv_Code.SelectedRows == null || dgv_Code.SelectedRows.Count == 0)
                {
                    ShowWarn("请先选择一个流程代码进行修改！");
                    return;
                }
                _isEditing = true;
                dgv_Process.ReadOnly = false;
                if (dgv_Process.Rows.Count > 0)
                {
                    dgv_Process.CurrentCell = dgv_Process.Rows[0].Cells[2];
                    dgv_Process.BeginEdit(true);
                }
                else
                {
                    dgv_Process.Rows.Add();
                    dgv_Process.CurrentCell = dgv_Process.Rows[0].Cells[2];
                    dgv_Process.BeginEdit(true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("进入编辑模式异常", ex);
            }
        }

        /// <summary>
        /// 保存流程代码及明细
        /// </summary>
        private void Btn_Save_Click(object sender, EventArgs e)
        {
            _isEditing = false;
            dgv_Process.ReadOnly = true;
            dgv_Code.ReadOnly = true;

            if (dgv_Code.SelectedRows == null || dgv_Code.SelectedRows.Count == 0)
            {
                ShowWarn("请先选择一个流程代码进行保存！");
                dgv_Code.BeginEdit(true);
                return;
            }
            var dgvCode = dgv_Code.SelectedRows[0].Cells[0];
            if (dgvCode.Value == null || string.IsNullOrWhiteSpace(dgvCode.Value.ToString()))
            {
                ShowError("流程代码不能为空！");
                dgv_Code.BeginEdit(true);
                return;
            }
            string newCode = dgvCode.Value.ToString().Trim();

            // 检查是否重复
            string filter = $"{SmartColor.My_DataBase.ABS_PROCESS.Code} = '{newCode.Replace("'", "''")}'";
            DataRow[] existing = SmartColor.My_DataBase.ABSProcess.ABS_Process.Select(filter);

            try
            {
                // 删除原有流程
                if (existing.Length > 0)
                {
                    SmartColor.My_DataBase.SqlServer.Delete(SmartColor.My_DataBase.ABS_PROCESS.TableName, filter);
                }

                // 清理空行（明细表第二列为空则删除）
                for (int i = dgv_Process.Rows.Count - 1; i >= 0; i--)
                {
                    var row = dgv_Process.Rows[i];
                    if (!row.IsNewRow && (row.Cells[2].Value == null || string.IsNullOrWhiteSpace(row.Cells[2].Value.ToString())))
                        dgv_Process.Rows.RemoveAt(i);
                }

                // 保存所有流程步骤
                foreach (DataGridViewRow row in dgv_Process.Rows)
                {
                    if (row.IsNewRow) continue;
                    var processData = new Dictionary<string, object>
                    {
                        { SmartColor.My_DataBase.ABS_PROCESS.Code, newCode },
                        { SmartColor.My_DataBase.ABS_PROCESS.StepNum, string.IsNullOrWhiteSpace(row.Cells[0].Value?.ToString()) ? DBNull.Value : (object)Convert.ToInt32(row.Cells[0].Value) },
                        { SmartColor.My_DataBase.ABS_PROCESS.TechnologyName, row.Cells[1].Value?.ToString().Trim() },
                        { SmartColor.My_DataBase.ABS_PROCESS.StirringRate, row.Cells[2].Value?.ToString().Trim() },
                        { SmartColor.My_DataBase.ABS_PROCESS.StirringTime, row.Cells[3].Value?.ToString().Trim() },
                        { SmartColor.My_DataBase.ABS_PROCESS.DrainTime, row.Cells[4].Value?.ToString().Trim() },
                        { SmartColor.My_DataBase.ABS_PROCESS.ParallelizingDishTime, row.Cells[5].Value?.ToString().Trim() },
                        { SmartColor.My_DataBase.ABS_PROCESS.PumpingTime, row.Cells[6].Value?.ToString().Trim() },
                        { SmartColor.My_DataBase.ABS_PROCESS.StartingWavelength, row.Cells[7].Value?.ToString().Trim() },
                        { SmartColor.My_DataBase.ABS_PROCESS.EndWavelength, row.Cells[8].Value?.ToString().Trim() },
                        { SmartColor.My_DataBase.ABS_PROCESS.WavelengthInterval, row.Cells[9].Value?.ToString().Trim() },
                        { SmartColor.My_DataBase.ABS_PROCESS.Dosage, row.Cells[10].Value?.ToString().Trim() }
                    };
                    SmartColor.My_DataBase.SqlServer.Insert(SmartColor.My_DataBase.ABS_PROCESS.TableName, processData);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("保存失败", ex);
                return; 
            }

            ShowInfo("保存完成");
            UVMeasurement_Load(sender, new EventArgs());
            foreach (DataGridViewRow row in dgv_Code.Rows)
            {
                if (row.Cells[0].Value?.ToString() == newCode)
                {
                    row.Selected = true;
                    dgv_Code.CurrentCell = row.Cells[0];
                    break;
                }
            }

           
            Logger.Error($"流程代码[{newCode}]及明细已保存。");
        }

        /// <summary>
        /// 删除流程代码及明细，删除前校验是否被母液瓶引用
        /// </summary>
        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            if (dgv_Code.SelectedRows == null || dgv_Code.SelectedRows.Count == 0)
            {
                ShowWarn("请先选择一个流程代码进行删除！");
                return;
            }
            var dgvCode = dgv_Code.SelectedRows[0].Cells[0];
            string code = dgvCode.Value?.ToString().Trim();
            if (string.IsNullOrEmpty(code)) return;

            // 校验流程代码是否被母液瓶引用
            if (IsCodeReferenced(code)) return;

            string filter = $"{SmartColor.My_DataBase.ABS_PROCESS.Code} = '{code.Replace("'", "''")}'";
            try
            {
                DialogResult dialogResult = ShowConfirm("确定删除吗？");
                if (dialogResult == DialogResult.Yes)
                {
                    SmartColor.My_DataBase.SqlServer.Delete(SmartColor.My_DataBase.ABS_PROCESS.TableName, filter);

                    ShowInfo("删除完成");

                    UVMeasurement_Load(sender, new EventArgs());

                 
                    Logger.Error($"流程代码[{code}]及明细已删除。");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("删除失败", ex);
                return;
            }
        }

        /// <summary>
        /// 拷贝流程代码及明细，自动加“拷贝”后缀
        /// </summary>
        private void Tsmi_Copy_Click(object sender, EventArgs e)
        {
            var srcRow = dgv_Code.CurrentRow;
            if (srcRow == null || srcRow.IsNewRow)
            {
                ShowWarn("请先选择要复制的流程代码行！");
                return;
            }
            var srcCode = srcRow.Cells[0].Value?.ToString();

            // 触发新增
            Btn_Add_Click(sender, e);

            var newRow = dgv_Code.Rows[dgv_Code.Rows.Count - 1];

            // 复制明细
            dgv_Process.Rows.Clear();
            if (!string.IsNullOrWhiteSpace(srcCode) && SmartColor.My_DataBase.ABSProcess.ABS_Process != null)
            {
                string filter = $"{SmartColor.My_DataBase.ABS_PROCESS.Code} = '{srcCode.Replace("'", "''")}'";
                DataRow[] drs = SmartColor.My_DataBase.ABSProcess.ABS_Process.Select(filter);
                var ordered = drs
                    .Where(r => r[SmartColor.My_DataBase.ABS_PROCESS.StepNum] != DBNull.Value)
                    .OrderBy(r => Convert.ToInt32(r[SmartColor.My_DataBase.ABS_PROCESS.StepNum]));

                foreach (DataRow row in ordered)
                {
                    dgv_Process.Rows.Add(
                        row[SmartColor.My_DataBase.ABS_PROCESS.StepNum]?.ToString(),
                        row[SmartColor.My_DataBase.ABS_PROCESS.TechnologyName]?.ToString(),
                        row[SmartColor.My_DataBase.ABS_PROCESS.StirringRate]?.ToString(),
                        row[SmartColor.My_DataBase.ABS_PROCESS.StirringTime]?.ToString(),
                        row[SmartColor.My_DataBase.ABS_PROCESS.DrainTime]?.ToString(),
                        row[SmartColor.My_DataBase.ABS_PROCESS.ParallelizingDishTime]?.ToString(),
                        row[SmartColor.My_DataBase.ABS_PROCESS.PumpingTime]?.ToString(),
                        row[SmartColor.My_DataBase.ABS_PROCESS.StartingWavelength]?.ToString(),
                        row[SmartColor.My_DataBase.ABS_PROCESS.EndWavelength]?.ToString(),
                        row[SmartColor.My_DataBase.ABS_PROCESS.WavelengthInterval]?.ToString(),
                        row[SmartColor.My_DataBase.ABS_PROCESS.Dosage]?.ToString()
                    );
                }
            }
            // 新行工艺代码自动加“拷贝”后缀
            newRow.Cells[0].Value = srcCode + "拷贝";
            dgv_Code.CurrentCell = newRow.Cells[0];
            dgv_Code.EndEdit();
            dgv_Code.BeginEdit(true);
        }

        /// <summary>
        /// 步号自动排序，保证流程明细步号连续
        /// </summary>
        private void ReorderStepNum()
        {
            int step = 1;
            foreach (DataGridViewRow row in dgv_Process.Rows)
            {
                if (row.IsNewRow) continue;
                row.Cells[0].Value = step++;
            }
        }

        /// <summary>
        /// 流程表快捷键处理（插入/删除行）
        /// </summary>
        private void Dgv_Process_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_isEditing) return;

            var dgv = dgv_Process;

            switch (e.KeyCode)
            {
                case Keys.Insert:
                    dgv.EndEdit();
                    int idx = dgv.CurrentCell?.RowIndex ?? 0;
                    dgv.Rows.Insert(idx, 1);
                    ReorderStepNum();
                    dgv.CurrentCell = dgv.Rows[idx].Cells[2];
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

        /// <summary>
        /// 判断单元格是否为空
        /// </summary>
        private bool IsCellEmpty(DataGridViewCell cell)
        {
            return cell == null || string.IsNullOrWhiteSpace(Convert.ToString(cell.Value));
        }

        /// <summary>
        /// 构建通用过滤条件
        /// </summary>
        private string BuildFilter(DataTable table, object codeValue, string colName)
        {
            bool isString = table.Columns[colName].DataType == typeof(string);
            string codeStr = codeValue.ToString().Replace("'", "''");
            return isString
                ? $"{colName} = '{codeStr}'"
                : $"{colName} = {codeStr}";
        }

        /// <summary>
        /// 编辑状态切换时，处理未保存数据提示
        /// </summary>
        private void HandleUnsavedEdit(int prevRowIndex)
        {
            bool hasUnsaved = false;
            DataGridViewRow prevRow = (prevRowIndex >= 0 && prevRowIndex < dgv_Code.Rows.Count)
                ? dgv_Code.Rows[prevRowIndex]
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
                var result = ShowConfirm("有未保存的数据，是否放弃修改？");
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
                else
                {
                    // 如果是拷贝行（最后一行且内容以“拷贝”结尾），则清理明细
                    if (prevRow != null && prevRow.Index == dgv_Code.Rows.Count - 1)
                    {
                        var codeVal = prevRow.Cells[0].Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(codeVal) && codeVal.EndsWith("拷贝"))
                        {
                            dgv_Process.Rows.Clear();
                        }
                        dgv_Code.Rows.RemoveAt(prevRow.Index);
                    }
                }
            }

            _suppressSelectionChanged = true;
            dgv_Code.EndEdit();

            if (prevRow != null)
            {
                if (dgv_Code.Rows.Count > SmartColor.My_DataBase.ABSProcess.ABS_Process.Rows.Count)
                {
                    if (prevRow.Index == dgv_Code.Rows.Count - 1)
                    {
                        dgv_Code.Rows.RemoveAt(prevRow.Index);
                    }
                }
            }

            _isEditing = false;
            dgv_Code.ReadOnly = true;
            dgv_Process.ReadOnly = true;
            _suppressSelectionChanged = false;
            dgv_Code.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        /// <summary>
        /// 删除前校验流程代码是否被母液瓶引用
        /// </summary>
        private bool IsCodeReferenced(string code)
        {
            if (SmartColor.My_DataBase.BottleData.Bottle_details == null)
                return false;
            string filter = BuildFilter(SmartColor.My_DataBase.BottleData.Bottle_details, code, SmartColor.My_DataBase.BOTTLE_DETAILS.AbsCode);
            DataRow[] existing = SmartColor.My_DataBase.BottleData.Bottle_details.Select(filter);
            if (existing.Length > 0)
            {
                ShowInfo($"当前吸光度测量流程代码有{existing.Length}个染料正在使用，请先修改后再删除");
                return true;
            }
            return false;
        }

       

        /// <summary>
        /// 统一错误消息提示
        /// </summary>
        private void ShowError(string msg) => LocalTranslator.ShowMessage(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

        /// <summary>
        /// 统一信息消息提示
        /// </summary>
        private void ShowInfo(string msg) => LocalTranslator.ShowMessage(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

        /// <summary>
        /// 统一警告消息提示
        /// </summary>
        private void ShowWarn(string msg) => LocalTranslator.ShowMessage(msg, "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        /// <summary>
        /// 统一确认消息提示
        /// </summary>
        private DialogResult ShowConfirm(string msg) => LocalTranslator.ShowMessage(msg, "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
    }
}