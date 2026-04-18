using SmartColor.My_Control;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Windows.Forms;

namespace SmartColor.My_Form.BasicData
{
    public partial class DyeType : Form
    {
       

        private bool _isEditing = false;
        private bool _isProcessingSelectionChange = false;
        private int _lastSelectedRowIndex = -1;
        private bool _isLoading = false;
        private readonly List<string> _defaultType = new List<string>
        {
            "酸性染料",
            "活性染料",
            "分散染料",
            "硫化染料",
            "冰染料",
            "阳离子染料",
            "其他染料",
            "醋酸染料",
            "直接染料"
        };


        public DyeType()
        {
            InitializeComponent();
            this.Load += LimitSet_Load;
            this.dgv_Type.SelectionChanged += Dgv_Dye_SelectionChanged;
            this.btn_Change.Click += Btn_Change_Click;
            this.btn_Save.Click += Btn_Save_Click;
            this.btn_Add.Click += Btn_Add_Click;
            this.btn_Delete.Click += Btn_Delete_Click;

            // 启用自定义编辑模式
            this.ctDHDataGridView1.CustomEditing = true;
            this.ctDHDataGridView1.EnterKeyAction += CtDHDataGridView1_EnterKeyAction;

            this.dgv_Type.CustomEditing = true;
            this.dgv_Type.EnterKeyAction += () => { if (_isEditing) { FocusDyeTypeAndEdit(); return true; } return false; };


        }

      

        /// <summary>
        /// 获取所有单位为g/l的助剂代码
        /// </summary>
        private List<string> GetGLAssistantCodes()
        {
            var dt = My_DataBase.AssistantData.Assistant_details;


            var codes = dt.AsEnumerable()
                .Where(r =>
                {
                    var unit = (r[My_DataBase.ASSISTANT_DETAILS.UnitOfAccount] + "").Trim();
                    return unit == "g/l";
                })
                .Select(r => r[My_DataBase.ASSISTANT_DETAILS.AssistantCode] + "")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();

            return codes;
        }

        private void LimitSet_Load(object sender, EventArgs e)
        {
            this._isLoading = true;
            dgv_Type.Rows.Clear();
            if (My_DataBase.LimitData.LimitTable != null)
            {
                var types = My_DataBase.LimitData.LimitTable.AsEnumerable()
                    .Select(r => r.Field<string>(My_DataBase.LIMIT_TABLE.Type))
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Distinct()
                    .ToList();

                int rowIndex = 0;
                foreach (var type in types)
                {
                    dgv_Type.Rows.Add(++rowIndex, type);
                }
            }
            dgv_Type.AutoFitAllColumns();
            ctDHDataGridView1.ReadOnly = true;
            _isEditing = false;
            dgv_Type.ClearSelection();
            ctDHDataGridView1.ClearSelection();
            this._isLoading = false;
        }

        private void Dgv_Dye_SelectionChanged(object sender, EventArgs e)
        {
            ctDHDataGridView1.AutoFitAllColumns();
            if (_isLoading || _isProcessingSelectionChange) return;
            int prevRowIndex = _lastSelectedRowIndex;
            if (dgv_Type.CurrentCell != null)
                _lastSelectedRowIndex = dgv_Type.CurrentCell.RowIndex;
            else if (dgv_Type.SelectedCells.Count > 0)
                _lastSelectedRowIndex = dgv_Type.SelectedCells[0].RowIndex;
            else
                _lastSelectedRowIndex = -1;

            if (_isEditing)
            {
                bool hasUnsaved = false;
                DataGridViewRow prevRow = (prevRowIndex >= 0 && prevRowIndex < dgv_Type.Rows.Count)
                    ? dgv_Type.Rows[prevRowIndex]
                    : null;

                if (prevRow != null)
                {
                    var cell = prevRow.Cells[1];
                    // 新增时类型为空也弹窗提示
                    if (!IsCellEmpty(cell))
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
                            dgv_Type.CurrentCell = prevRow.Cells[1];
                            prevRow.Selected = true;
                            _lastSelectedRowIndex = prevRow.Index;
                        }
                        _isProcessingSelectionChange = false;
                        return;
                    }
                }

                // 放弃修改，清理编辑状态
                _isProcessingSelectionChange = true;
                dgv_Type.EndEdit();

                if (prevRow != null)
                {
                    var types = My_DataBase.LimitData.LimitTable.AsEnumerable()
                   .Select(r => r.Field<string>(My_DataBase.LIMIT_TABLE.Type))
                   .Where(t => !string.IsNullOrWhiteSpace(t))
                   .Distinct()
                   .ToList();

                    if (dgv_Type.Rows.Count > types.Count)
                    {
                        // 删除新增但未保存的空行
                        dgv_Type.Rows.RemoveAt(prevRow.Index);
                    }

                }

                _isEditing = false;
                dgv_Type.ReadOnly = true;
                ctDHDataGridView1.ReadOnly = true;
                _isProcessingSelectionChange = false;
                dgv_Type.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            if (dgv_Type.SelectedRows == null || dgv_Type.SelectedRows.Count == 0)
                return;
            ctDHDataGridView1.Rows.Clear();
            var dyeTypeValue = dgv_Type.SelectedRows[0].Cells[1].Value;
            if (dyeTypeValue == null || dyeTypeValue == DBNull.Value || My_DataBase.LimitData.LimitTable == null) return;
            ShowLimit(dyeTypeValue.ToString());

            Logger.Info($"Dgv_Dye_SelectionChanged: 加载分量详情，类型={dyeTypeValue}");
        }

        /// <summary>
        /// 动态加载区间表头和数据
        /// </summary>
        private void ShowLimit(string assistant)
        {
            if (string.IsNullOrWhiteSpace(assistant)) return;
            if (My_DataBase.LimitData.LimitTable == null) return;

            ctDHDataGridView1.IsLoading = true;

            // 获取g/l助剂代码
            var assistantCodes = GetGLAssistantCodes();

            // 过滤LimitTable，按Type分组
            var rows = My_DataBase.LimitData.LimitTable?.AsEnumerable()
                .Where(r => r.Field<string>(My_DataBase.LIMIT_TABLE.Type) == assistant)
                .OrderBy(r => r.Field<decimal?>(My_DataBase.LIMIT_TABLE.Min) ?? 0)
                .ToList();

            // 动态收集所有区间（min-max），并生成区间表头
            var rangeList = new List<(decimal min, decimal? max)>();
            if (rows != null && rows.Count > 0)
            {
                foreach (var r in rows)
                {
                    decimal min = r.IsNull(My_DataBase.LIMIT_TABLE.Min) ? 0m : r.Field<decimal>(My_DataBase.LIMIT_TABLE.Min);
                    decimal? max = r.IsNull(My_DataBase.LIMIT_TABLE.Max) ? (decimal?)null : r.Field<decimal>(My_DataBase.LIMIT_TABLE.Max);
                    if (!rangeList.Any(x => x.min == min && x.max == max))
                        rangeList.Add((min, max));
                }
            }

            ctDHDataGridView1.Columns.Clear();
            ctDHDataGridView1.Rows.Clear();
            ctDHDataGridView1.AssistantCodes = assistantCodes;

            // 第一列：助剂代码
            var comboCol = new DataGridViewComboBoxColumn
            {
                HeaderText = "",
                Name = "AssistantCode",
                DataSource = assistantCodes,
                Width = 100,
                FlatStyle = FlatStyle.Popup,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            ctDHDataGridView1.Columns.Add(comboCol);

            // 区间列
            if (rangeList.Count == 0)
            {
                var txtCol = new DataGridViewTextBoxColumn
                {
                    HeaderText = "0-∞",
                    Name = "Range_1",
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                ctDHDataGridView1.Columns.Add(txtCol);
            }
            else
            {
                for (int i = 0; i < rangeList.Count; i++)
                {
                    var (min, max) = rangeList[i];
                    string header = max.HasValue ? $"{min}-{max}" : $"{min}-∞";
                    var txtCol = new DataGridViewTextBoxColumn
                    {
                        HeaderText = header,
                        Name = $"Range_{i + 1}",
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    };
                    ctDHDataGridView1.Columns.Add(txtCol);
                }
            }

            // 按助剂代码分组，填充每行数据
            if (rows != null && rows.Count > 0)
            {
                var codeGroups = rows.GroupBy(r => r.Field<string>(My_DataBase.LIMIT_TABLE.AssistantCode));
                foreach (var group in codeGroups)
                {
                    var code = group.Key;
                    if (string.IsNullOrWhiteSpace(code)) continue; // 跳过空助剂代码
                    var rowIdx = ctDHDataGridView1.Rows.Add();
                    ctDHDataGridView1.Rows[rowIdx].Cells[0].Value = code;

                    for (int i = 0; i < rangeList.Count; i++)
                    {
                        var (min, max) = rangeList[i];
                        var data = group.FirstOrDefault(r =>
                            (r.IsNull(My_DataBase.LIMIT_TABLE.Min) ? 0m : r.Field<decimal>(My_DataBase.LIMIT_TABLE.Min)) == min &&
                            ((r.IsNull(My_DataBase.LIMIT_TABLE.Max) && !max.HasValue) || (!r.IsNull(My_DataBase.LIMIT_TABLE.Max) && max.HasValue && r.Field<decimal>(My_DataBase.LIMIT_TABLE.Max) == max.Value))
                        );
                        if (data != null)
                        {
                            ctDHDataGridView1.Rows[rowIdx].Cells[i + 1].Value = data[My_DataBase.LIMIT_TABLE.Value];
                        }
                    }
                }
            }

            ctDHDataGridView1.IsLoading = false;
            ctDHDataGridView1.ReadOnly = !_isEditing;
            ctDHDataGridView1.ClearSelection();
            Logger.Info($"ShowAssistantLimit: 显示助剂[{assistant}]的分量区间，区间数={rows?.Count ?? 0}");
        }

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isEditing) return;
                dgv_Type.SelectionMode = DataGridViewSelectionMode.CellSelect;
                _isProcessingSelectionChange = true;
                _isEditing = true;
                dgv_Type.CustomEditing = _isEditing;
                ctDHDataGridView1.CustomEditing = _isEditing;
                dgv_Type.ReadOnly = false;
                int newRowIdx = dgv_Type.Rows.Add();
                dgv_Type.Rows[newRowIdx].Cells[0].Value = dgv_Type.Rows.Count;
                dgv_Type.Rows[newRowIdx].Cells[1].Value = "";
                dgv_Type.ClearSelection();
                dgv_Type.CurrentCell = dgv_Type.Rows[newRowIdx].Cells[1];
                _lastSelectedRowIndex = newRowIdx;
                dgv_Type.BeginEdit(true);
                _isProcessingSelectionChange = false;
                ctDHDataGridView1.Rows.Clear();
                Logger.Info("Btn_Add_Click: 新增染料类型行。");
            }
            catch (Exception ex)
            {
                Logger.Error("Btn_Add_Click: 新增染料类型异常。", ex);
            }
        }

        private void Btn_Change_Click(object sender, EventArgs e)
        {
            if (!ValidateDyeTypeSelection()) return;
            _isEditing = true;
            dgv_Type.CustomEditing = _isEditing;
            ctDHDataGridView1.CustomEditing = _isEditing;
            ctDHDataGridView1.ReadOnly = false;
            if (ctDHDataGridView1.Rows.Count == 0) ctDHDataGridView1.Rows.Add();
            ctDHDataGridView1.CurrentCell = ctDHDataGridView1.Rows[0].Cells[1];
            ctDHDataGridView1.BeginEdit(true);
            Logger.Info("Btn_Change_Click: 进入编辑模式。");
        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            if (!ValidateDyeTypeSelection()) return;
            var selectedRow = dgv_Type.SelectedRows[0];
            string dyeType = selectedRow.Cells[1].Value?.ToString();
            var existing = My_DataBase.LimitData.LimitTable?.Select($"{My_DataBase.LIMIT_TABLE.Type} = '{dyeType}'");
            if (existing == null || existing.Length == 0)
            {
                ShowError("染料类型不存在，无法删除！");
                dgv_Type.BeginEdit(true);
                return;
            }

            DataRow[] dr = My_DataBase.AssistantData.Assistant_details.Select($"{My_DataBase.ASSISTANT_DETAILS.AssistantType} = '{dyeType}'");
            if (dr.Length > 0)
            {
                ShowError("该染料类型在助剂资料中有使用，无法删除！");
                dgv_Type.BeginEdit(true);
                return;
            }

            var result = LocalTranslator.ShowMessage(
                $"确定要删除染料类型 [{dyeType}] 及其所有分量区间吗？",
                "删除确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                dgv_Type.BeginEdit(true);
                return;
            }

            try
            {
                int currentRowIndex = dgv_Type.CurrentCell.RowIndex;
                SqlServer.Delete(My_DataBase.LIMIT_TABLE.TableName, $"{My_DataBase.LIMIT_TABLE.Type} = '{dyeType}'");
                Logger.Info($"Btn_Delete_Click: 删除染料类型[{dyeType}]及其分量区间。");

                LimitSet_Load(sender, e);
                ShowInfo("删除完成");
                if (currentRowIndex < dgv_Type.Rows.Count - 1)
                {
                    dgv_Type.Rows[currentRowIndex].Selected = true;
                    dgv_Type.CurrentCell = dgv_Type.Rows[currentRowIndex].Cells[0];
                }
                else
                {
                    dgv_Type.Rows[dgv_Type.Rows.Count - 1].Selected = true;
                    dgv_Type.CurrentCell = dgv_Type.Rows[dgv_Type.Rows.Count - 1].Cells[0];
                }
               

            }
            catch (Exception ex)
            {
                Logger.Error($"Btn_Delete_Click: 删除染料类型[{dyeType}]异常。", ex);
            }
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (!_isEditing) return;
            if (dgv_Type.CurrentRow == null)
            {
                LocalTranslator.ShowMessage("请先选择一个染料种类！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            dgv_Type.EndEdit();
            string dyeType = dgv_Type.CurrentRow.Cells[1].Value?.ToString();
            if (string.IsNullOrWhiteSpace(dyeType))
            {
                LocalTranslator.ShowMessage("染料种类不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isAddOnlyType = ctDHDataGridView1.ReadOnly || ctDHDataGridView1.Rows.Count == 0;

            if (isAddOnlyType)
            {
                if (TypeExists(dyeType))
                {
                    ShowError("染料类型已存在，不能重复！");
                    return;
                }
                try
                {
                    var rowData = new Dictionary<string, object>
                    {
                        { My_DataBase.LIMIT_TABLE.Type, dyeType },
                        { My_DataBase.LIMIT_TABLE.Min, 0m },
                        { My_DataBase.LIMIT_TABLE.Max, DBNull.Value },
                        { My_DataBase.LIMIT_TABLE.AssistantCode, "" },
                        { My_DataBase.LIMIT_TABLE.Value, DBNull.Value }
                    };
                    SqlServer.Insert(My_DataBase.LIMIT_TABLE.TableName, rowData);
                    Logger.Info($"新增染料类型[{dyeType}]成功。");
                    _isEditing = false;
                    dgv_Type.ReadOnly = true;
                    dgv_Type.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    ShowInfo("保存完成");
                    LimitSet_Load(sender, e);
                    foreach (DataGridViewRow row in dgv_Type.Rows)
                    {
                        if (row.Cells[1].Value?.ToString() == dyeType)
                        {
                            row.Selected = true;
                            dgv_Type.CurrentCell = row.Cells[0];
                            break;
                        }
                    }
                    btn_Add.Focus();

                   
                }
                catch (Exception ex)
                {
                    Logger.Error($"新增染料类型[{dyeType}]异常。", ex);
                }
                return;
            }

            var codeSet = new HashSet<string>();
            foreach (DataGridViewRow row in ctDHDataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                var codeCell = row.Cells[0];
                string code = codeCell.Value?.ToString().Trim();
                if (string.IsNullOrEmpty(code)) continue;
                if (!codeSet.Add(code))
                {
                    LocalTranslator.ShowMessage($"存在重复的助剂代码：{code}，请检查！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            var dataList = new List<Dictionary<string, object>>();
          
        
            foreach (DataGridViewRow row in ctDHDataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                var codeCell = row.Cells[0];
                string code = codeCell.Value?.ToString().Trim();
                if (string.IsNullOrEmpty(code)) continue;

                for (int col = 1; col < ctDHDataGridView1.Columns.Count; col++)
                {
                    var colObj = ctDHDataGridView1.Columns[col];
                    string header = colObj.HeaderText;
                    var match = System.Text.RegularExpressions.Regex.Match(header, @"^(?<min>-?\d+(\.\d+)?)-(?<max>-?\d+(\.\d+)?|∞)$");
                    if (!match.Success) continue;
                    string minStr = match.Groups["min"].Value;
                    string maxStr = match.Groups["max"].Value;

                    decimal min;
                    decimal? max = null;
                    if (!decimal.TryParse(minStr, out min)) continue;
                    if (maxStr != "∞")
                    {
                        decimal maxVal;
                        if (decimal.TryParse(maxStr, out maxVal))
                            max = maxVal;
                    }

                    var valueCell = row.Cells[col];
                    if (valueCell == null || valueCell.Value == null || string.IsNullOrWhiteSpace(valueCell.Value.ToString()))
                        continue;
                    decimal value;
                    if (!decimal.TryParse(valueCell.Value.ToString(), out value)) continue;

                    dataList.Add(new Dictionary<string, object>
                    {
                        { My_DataBase.LIMIT_TABLE.Type, dyeType },
                        { My_DataBase.LIMIT_TABLE.Min, min },
                        { My_DataBase.LIMIT_TABLE.Max, max ?? (object)DBNull.Value },
                        { My_DataBase.LIMIT_TABLE.AssistantCode, code },
                        { My_DataBase.LIMIT_TABLE.Value, value }
                    });
                }
            }
            if (dataList.Count == 0)
            {
                LocalTranslator.ShowMessage("请填写有效的分量区间数据！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                SqlServer.Delete(My_DataBase.LIMIT_TABLE.TableName, $"{My_DataBase.LIMIT_TABLE.Type} = '{dyeType}'");
                SqlServer.Insert(My_DataBase.LIMIT_TABLE.TableName, dataList);
                Logger.Info($"LimitSet: 保存分量区间成功。");
                _isEditing = false;
                ctDHDataGridView1.ReadOnly = true;
                LocalTranslator.ShowMessage("保存完成", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimitSet_Load(sender, e);
                foreach (DataGridViewRow row in dgv_Type.Rows)
                {
                    if (row.Cells[1].Value?.ToString() == dyeType)
                    {
                        row.Selected = true;
                        dgv_Type.CurrentCell = row.Cells[0];
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"LimitSet: 保存分量区间异常。", ex);
            }
        }
        private void Tsmi_DefaultType_Click(object sender, EventArgs e)
        {
            var result = LocalTranslator.ShowMessage(
                       "是否确认加入使用默认染料类型？",
                       "提示",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Question);

            if (result == DialogResult.No) return;

            foreach (string s in _defaultType)
            {
                if (TypeExists(s)) continue;
                var rowData = new Dictionary<string, object>
                    {
                        { My_DataBase.LIMIT_TABLE.Type, s },
                        { My_DataBase.LIMIT_TABLE.Min, 0m },
                        { My_DataBase.LIMIT_TABLE.Max, DBNull.Value },
                        { My_DataBase.LIMIT_TABLE.AssistantCode, "" },
                        { My_DataBase.LIMIT_TABLE.Value, DBNull.Value }
                    };
                SqlServer.Insert(My_DataBase.LIMIT_TABLE.TableName, rowData);
                Logger.Info($"新增染料类型[{s}]成功。");
            }
            LimitSet_Load(sender, e);
           
        }

        // dgv_Dye的回车跳转和校验逻辑
        private void FocusDyeTypeAndEdit()
        {
            var dgv = dgv_Type;
            dgv.EndEdit();
            if (dgv.CurrentCell == null) return;
            int col = dgv.CurrentCell.ColumnIndex;
            int row = dgv.CurrentCell.RowIndex;

            void JumpTo(int targetRow, int targetCol)
            {
                dgv.CurrentCell = dgv.Rows[targetRow].Cells[targetCol];
                dgv.BeginEdit(true);
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
                        ShowErrorAndRemoveRow("染料类型不能为空！");
                        return;
                    }
                    if (TypeExists(cellValue.ToString().Trim()))
                    {
                        ShowError("染料类型已存在，不能重复！");
                        dgv.BeginEdit(true);
                        return;
                    }
                    btn_Save.Focus();
                }
                else
                {
                    if (col == 1)
                    {
                        if (string.IsNullOrWhiteSpace(cellValue?.ToString()))
                        {
                            ShowErrorAndRemoveRow("染料类型不能为空！");
                            return;
                        }
                        if (TypeExists(cellValue.ToString().Trim()))
                        {
                            ShowError("染料类型已存在，不能重复！");
                            dgv.BeginEdit(true);
                            return;
                        }
                    }
                    JumpTo(row, col + 1);
                }
            }
            this.BeginInvoke(new Action(SmartJump));
        }

        // 分量详情表的回车跳转逻辑
        private bool CtDHDataGridView1_EnterKeyAction()
        {
            if (!_isEditing) return false;
            var dgv = ctDHDataGridView1;
            if (dgv.CurrentCell == null) return false;
            int col = dgv.CurrentCell.ColumnIndex;
            int row = dgv.CurrentCell.RowIndex;
            int colCount = dgv.ColumnCount;

            if (col == 0)
            {
                if (dgv.CurrentCell.Value == null || string.IsNullOrWhiteSpace(dgv.CurrentCell.Value.ToString()))
                {
                    btn_Save.Focus();
                    return true;
                }
                dgv.CurrentCell = dgv.Rows[row].Cells[1];
                dgv.BeginEdit(true);
                return true;
            }

            if (dgv.CurrentCell.Value == null || string.IsNullOrWhiteSpace(dgv.CurrentCell.Value.ToString()))
            {
                LocalTranslator.ShowMessage("该列不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgv.BeginEdit(true);
                return true;
            }

            if (col < colCount - 1)
            {
                dgv.CurrentCell = dgv.Rows[row].Cells[col + 1];
                dgv.BeginEdit(true);
                return true;
            }
            if (row < dgv.Rows.Count - 1)
            {
                dgv.CurrentCell = dgv.Rows[row + 1].Cells[0];
                dgv.BeginEdit(true);
                return true;
            }
            dgv.Rows.Add();
            dgv.CurrentCell = dgv.Rows[dgv.Rows.Count - 1].Cells[0];
            dgv.BeginEdit(true);
            return true;
        }

        private bool ValidateDyeTypeSelection()
        {
            if (dgv_Type.SelectedRows == null)
            {
                ShowInfo("请先选择一个染料类型！");
                dgv_Type.BeginEdit(true);
                return false;
            }
            if (dgv_Type.SelectedCells.Count == 0)
            {
                ShowError("未选中任何单元格！");
                return false;
            }
            int selectedRowIndex = dgv_Type.SelectedCells[0].RowIndex;
            if (selectedRowIndex < 0 || selectedRowIndex >= dgv_Type.Rows.Count)
            {
                ShowError("选中行索引无效！");
                return false;
            }

            DataGridViewCell cell = dgv_Type.Rows[selectedRowIndex].Cells[1];
            if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
            {
                ShowError("染料类型不能为空！");
                dgv_Type.BeginEdit(true);
                return false;
            }

            return true;
        }

        private bool TypeExists(string type)
        {
            if (My_DataBase.LimitData.LimitTable == null) return false;
            return My_DataBase.LimitData.LimitTable.AsEnumerable()
                .Any(r => r.Field<string>(My_DataBase.LIMIT_TABLE.Type)?.Trim() == type);
        }

        private void ShowErrorAndRemoveRow(string msg)
        {
            ShowError(msg);
            if (dgv_Type.CurrentRow != null && !dgv_Type.CurrentRow.IsNewRow)
                dgv_Type.Rows.RemoveAt(dgv_Type.CurrentRow.Index);
            _isEditing = false;
            dgv_Type.ReadOnly = true;
        }

        private bool IsCellEmpty(DataGridViewCell cell)
        {
            return cell == null || string.IsNullOrWhiteSpace(Convert.ToString(cell.Value));
        }

        private void ShowError(string msg) => LocalTranslator.ShowMessage(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowInfo(string msg) => LocalTranslator.ShowMessage(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

