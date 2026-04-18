using SmartColor.My_DataBase;
using SmartColor.My_File;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Form.BasicData
{
    public partial class Operator : Form
    {
       

        private bool _isEditing = false;
        private bool _isProcessingSelectionChange = false;
        private int _lastSelectedRowIndex = -1;

        public Operator()
        {
            InitializeComponent();

            this.Text = LocalTranslator.ZhToEn("操作人员基本资料");
            this.dgv_DyeCode.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.Load += Operator_Load;

            this.btn_Add.Click += Btn_Add_Click;
            this.btn_Change.Click += Btn_Change_Click;
            this.btn_Save.Click += Btn_Save_Click;
            this.btn_Delete.Click += Btn_Delete_Click;

            this.dgv_DyeCode.EnterKeyAction += () => { if (_isEditing) { FocusDyeCodeAndEdit(); return true; } return false; };
            this.dgv_DyeCode.SelectionChanged += Dgv_DyeCode_SelectionChanged;
        }



        private void Operator_Load(object sender, EventArgs e)
        {
            try
            {
                Logger.Info("Operator_Load: 开始加载操作人员数据。");
                dgv_DyeCode.Rows.Clear();
                
                dgv_DyeCode.BindDataTable(
                    My_DataBase.UserData.UserTable.AsEnumerable()
                        .Where(r => r.Field<int?>(My_DataBase.USER_TALE.Purview) == 0)
                        .GroupBy(r => r.Field<string>(My_DataBase.USER_TALE.Account))
                        .Select(g => g.First())
                        .CopyToDataTable(),
                    row => new object[]
                    {
                        row[My_DataBase.USER_TALE.MyID],
                        row[My_DataBase.USER_TALE.Account],
                        string.IsNullOrEmpty(row[My_DataBase.USER_TALE.PassWord]?.ToString()) ? "" : new string('*', row[My_DataBase.USER_TALE.PassWord].ToString().Length),
                        row[My_DataBase.USER_TALE.RealName]
                    },
                    My_DataBase.USER_TALE.Account
                );
                dgv_DyeCode.ClearSelection();
                Logger.Info("Operator_Load: 加载完成。");
            }
            catch (Exception ex)
            {
                Logger.Error("Operator_Load: 加载操作人员数据异常。", ex);
            }
        }

        private void Btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                if(_isEditing) return;
                _isProcessingSelectionChange = true;
                dgv_DyeCode.SelectionMode = DataGridViewSelectionMode.CellSelect;
                _isEditing = true;
                dgv_DyeCode.CustomEditing = _isEditing;
                dgv_DyeCode.ReadOnly = false;
                dgv_DyeCode.Rows.Add();
                int newRowIdx = dgv_DyeCode.Rows.Count - 1;
                dgv_DyeCode.CurrentCell = dgv_DyeCode.Rows[newRowIdx].Cells[1];
                dgv_DyeCode.Rows[newRowIdx].Cells[0].Value = dgv_DyeCode.Rows.Count;
                _lastSelectedRowIndex = newRowIdx;
                dgv_DyeCode.BeginEdit(true);
                _isProcessingSelectionChange = false;
                Logger.Info("Btn_Add_Click: 新增操作人员行。");
            }
            catch (Exception ex)
            {
                Logger.Error("Btn_Add_Click: 新增操作人员异常。", ex);
            }
        }

        private void Btn_Change_Click(object sender, EventArgs e)
        {
            if (!ValidateOperatorSelection()) return;
            _isProcessingSelectionChange = true;
            _isEditing = true;
            dgv_DyeCode.CustomEditing = _isEditing;
            dgv_DyeCode.ReadOnly = false;
            int rowIdx = dgv_DyeCode.SelectedRows[0].Index;
            dgv_DyeCode.CurrentCell = dgv_DyeCode.Rows[rowIdx].Cells[2];
            dgv_DyeCode.Rows[rowIdx].Cells[1].ReadOnly = true;
            dgv_DyeCode.BeginEdit(true);
            dgv_DyeCode.SelectionMode = DataGridViewSelectionMode.CellSelect;
            _lastSelectedRowIndex = rowIdx;
            _isProcessingSelectionChange = false;
            Logger.Info("Btn_Change_Click: 进入编辑模式。");
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            _isProcessingSelectionChange = true;
            _isEditing = false;
            dgv_DyeCode.CustomEditing = _isEditing;
            if (!ValidateOperatorSelection()) { _isProcessingSelectionChange = false; return; }
            DataGridViewCell cell = dgv_DyeCode.CurrentCell;
            if (cell == null) { _isProcessingSelectionChange = false; return; }
            DataGridViewRow currentRow = dgv_DyeCode.Rows[cell.RowIndex];

            // 依次校验所有字段
            string id = currentRow.Cells[0].Value?.ToString()?.Trim();
            string account = currentRow.Cells[1].Value?.ToString()?.Trim();
            string pwd = currentRow.Cells[2].Value?.ToString()?.Trim();
            string realName = currentRow.Cells[3].Value?.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(account))
            {
                ShowError("账户不能为空！");
                btn_Save.Focus();
                _isProcessingSelectionChange = false;
                return;
            }
            if (string.IsNullOrWhiteSpace(pwd))
            {
                ShowError("密码不能为空！");
                dgv_DyeCode.CurrentCell = currentRow.Cells[2];
                dgv_DyeCode.BeginEdit(true);
                _isProcessingSelectionChange = false;
                return;
            }
            if (string.IsNullOrWhiteSpace(realName))
            {
                ShowError("姓名不能为空！");
                dgv_DyeCode.CurrentCell = currentRow.Cells[3];
                dgv_DyeCode.BeginEdit(true);
                _isProcessingSelectionChange = false;
                return;
            }

            var data = new Dictionary<string, object>
            {
                {My_DataBase.USER_TALE.Account, account},
                {My_DataBase.USER_TALE.PassWord, pwd},
                {My_DataBase.USER_TALE.RealName, realName},
                {My_DataBase.USER_TALE.Purview, 0} // 只允许权限为0
            };

            try
            {
                var dt = SqlServer.Select(My_DataBase.USER_TALE.TableName, $"{My_DataBase.USER_TALE.MyID} = '{id}'");
                if (dt.Rows.Count > 0)
                {
                    SqlServer.Update(My_DataBase.USER_TALE.TableName, data, $"{My_DataBase.USER_TALE.MyID}='{id}'");
                    Logger.Info($"操作人员信息已更新：{account}");
                }
                else
                {
                    SqlServer.Insert(My_DataBase.USER_TALE.TableName, data);
                    Logger.Info($"新增操作人员：{account}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Btn_Save_Click: 保存操作人员异常。", ex);
                _isProcessingSelectionChange = false;
                return;
            }

            ShowInfo("保存完成");
            dgv_DyeCode.Rows[cell.RowIndex].Cells[1].ReadOnly = false;
            dgv_DyeCode.ReadOnly = true;
            dgv_DyeCode.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Operator_Load(sender, EventArgs.Empty);
            btn_Add.Focus();
            _isProcessingSelectionChange = false;


           
        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            _isProcessingSelectionChange = true;
            if (!ValidateOperatorSelection()) { _isProcessingSelectionChange = false; return; }
            var row = dgv_DyeCode.SelectedRows[0];
            string id = row.Cells[0].Value?.ToString()?.Trim();
            var dt = SqlServer.Select(My_DataBase.USER_TALE.TableName, $"{My_DataBase.USER_TALE.MyID} = '{id}'");
            if (dt.Rows.Count == 0)
            {
                ShowError("账户不存在，无法删除！");
                dgv_DyeCode.BeginEdit(true);
                _isProcessingSelectionChange = false;
                return;
            }

            var result = LocalTranslator.ShowMessage(
                $"确定要删除账户吗？",
                "删除确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) { _isProcessingSelectionChange = false; return; }

            SqlServer.Delete(My_DataBase.USER_TALE.TableName, $"{My_DataBase.USER_TALE.MyID}= @ID",
                new System.Data.SqlClient.SqlParameter("@ID", id));
            Logger.Info($"删除操作人员");
            ShowInfo("删除完成");
            Operator_Load(sender, EventArgs.Empty);
            _isProcessingSelectionChange = false;
            
        }

        // ================= 行切换未保存处理 =================

        private void Dgv_DyeCode_SelectionChanged(object sender, EventArgs e)
        {
            if (_isProcessingSelectionChange) return;
            int prevRowIndex = _lastSelectedRowIndex;
            if (dgv_DyeCode.CurrentCell != null)
                _lastSelectedRowIndex = dgv_DyeCode.CurrentCell.RowIndex;
            else if (dgv_DyeCode.SelectedCells.Count > 0)
                _lastSelectedRowIndex = dgv_DyeCode.SelectedCells[0].RowIndex;
            else
                _lastSelectedRowIndex = -1;

            if (_isEditing)
            {
                DataGridViewRow prevRow = (prevRowIndex >= 0 && prevRowIndex < dgv_DyeCode.Rows.Count)
                    ? dgv_DyeCode.Rows[prevRowIndex]
                    : null;

                bool hasContent = false;
                if (prevRow != null)
                {
                    hasContent =
                        !string.IsNullOrWhiteSpace(Convert.ToString(prevRow.Cells[1].Value)) ||
                        !string.IsNullOrWhiteSpace(Convert.ToString(prevRow.Cells[2].Value)) ||
                        !string.IsNullOrWhiteSpace(Convert.ToString(prevRow.Cells[3].Value));
                }

                if (hasContent)
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

                // 放弃修改，清理编辑状态
                _isProcessingSelectionChange = true;
                dgv_DyeCode.EndEdit();

                if (prevRow != null)
                {
                    DataTable dataTable = My_DataBase.UserData.UserTable.AsEnumerable()
                         .Where(r => r.Field<int?>(My_DataBase.USER_TALE.Purview) == 0)
                         .GroupBy(r => r.Field<string>(My_DataBase.USER_TALE.Account))
                         .Select(g => g.First())
                         .CopyToDataTable();
                    if (dgv_DyeCode.Rows.Count > dataTable.Rows.Count)
                    {
                        dgv_DyeCode.Rows.RemoveAt(prevRow.Index);
                    }

                }

                _isEditing = false;
                dgv_DyeCode.ReadOnly = true;
                _isProcessingSelectionChange = false;
                dgv_DyeCode.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }

        // ================= 工具方法 =================

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

                int prevCol = col;
                if (prevCol < 1) prevCol = 1; // 跳过序号列
                var prevCell = dgv.Rows[row].Cells[prevCol];
                bool isEmpty = string.IsNullOrWhiteSpace(prevCell.Value?.ToString());
                if (isEmpty)
                {
                    ShowWarn(GetFieldName(prevCol) + "不能为空！");
                    dgv.CurrentCell = prevCell;
                    dgv.BeginEdit(true);
                    return;
                }

                int nextCol = col + 1;
                if (nextCol < dgv.ColumnCount)
                {
                    JumpTo(row, nextCol);
                }
                else if (row + 1 < dgv.RowCount)
                {
                    JumpTo(row + 1, 1);
                }
                else
                {
                    btn_Save.Focus();
                }
            }
            this.BeginInvoke(new Action(SmartJump));
        }

        private bool ValidateOperatorSelection()
        {
            if (dgv_DyeCode.SelectedRows == null)
            {
                ShowInfo("请先选择一个账户！");
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

            DataGridViewCell dgvAccount = dgv_DyeCode.Rows[selectedRowIndex].Cells[1];
            if (dgvAccount.Value == null || string.IsNullOrWhiteSpace(dgvAccount.Value.ToString()))
            {
                ShowError("账户不能为空！");
                dgv_DyeCode.BeginEdit(true);
                return false;
            }

            return true;
        }

        private string GetFieldName(int colIndex)
        {
            switch (colIndex)
            {
                case 1: return "账户";
                case 2: return "密码";
                case 3: return "姓名";
                default: return "";
            }
        }

        private void ShowError(string msg) => LocalTranslator.ShowMessage(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowInfo(string msg) => LocalTranslator.ShowMessage(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void ShowWarn(string msg) => LocalTranslator.ShowMessage(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}