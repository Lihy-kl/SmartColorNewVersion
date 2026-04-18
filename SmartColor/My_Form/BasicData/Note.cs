using SmartColor.My_Control;
using SmartColor.My_File;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Form.BasicData
{
    public partial class Note : Form
    {
        

        // 增加字段保存原值
        private string _cellOldValue = null;
        public Note()
        {
            InitializeComponent();
            LoadNoteItems();
            BindEvents();
        }

        // 加载备注名称和可选项
        private void LoadNoteItems()
        {
            var dt = My_DataBase.EnabledData.Enabled_set;
            if (dt == null || dt.Rows.Count == 0) return;
            var row = dt.Rows[0];

            // 加载名称
            txt_Note1Name.Text = row[My_DataBase.ENABLED_SET.Note1Name]?.ToString() ?? "";
            txt_Note2Name.Text = row[My_DataBase.ENABLED_SET.Note2Name]?.ToString() ?? "";
            txt_Note3Name.Text = row[My_DataBase.ENABLED_SET.Note3Name]?.ToString() ?? "";

            // 加载可选项
            LoadItemsToGrid(dgv_Note1Items, row[My_DataBase.ENABLED_SET.Note1Items]?.ToString());
            LoadItemsToGrid(dgv_Note2Items, row[My_DataBase.ENABLED_SET.Note2Items]?.ToString());
            LoadItemsToGrid(dgv_Note3Items, row[My_DataBase.ENABLED_SET.Note3Items]?.ToString());
        }

        private void LoadItemsToGrid(DataGridView dgv, string items)
        {
            dgv.Rows.Clear();
            if (string.IsNullOrWhiteSpace(items)) return;
            var arr = items.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in arr)
            {
                dgv.Rows.Add(item);
            }
        }

        // 绑定事件
        private void BindEvents()
        {
            // 名称文本框失去焦点时保存
            txt_Note1Name.TextChanged += (s, e) => SaveNoteName(1);
            txt_Note2Name.TextChanged += (s, e) => SaveNoteName(2);
            txt_Note3Name.TextChanged += (s, e) => SaveNoteName(3);

            // DataGridView内容变化时保存
            dgv_Note1Items.CellValueChanged += (s, e) => SaveNoteItems(1);
            dgv_Note2Items.CellValueChanged += (s, e) => SaveNoteItems(2);
            dgv_Note3Items.CellValueChanged += (s, e) => SaveNoteItems(3);


            dgv_Note1Items.UserDeletedRow += (s, e) => SaveNoteItems(1);
            dgv_Note2Items.UserDeletedRow += (s, e) => SaveNoteItems(2);
            dgv_Note3Items.UserDeletedRow += (s, e) => SaveNoteItems(3);

            dgv_Note1Items.UserAddedRow += (s, e) => SaveNoteItems(1);
            dgv_Note2Items.UserAddedRow += (s, e) => SaveNoteItems(2);
            dgv_Note3Items.UserAddedRow += (s, e) => SaveNoteItems(3);

            dgv_Note1Items.Leave += Dgv_Note1Items_Leave;
            dgv_Note2Items.Leave += Dgv_Note1Items_Leave;
            dgv_Note3Items.Leave += Dgv_Note1Items_Leave;


            dgv_Note1Items.KeyDown += Dgv_NoteItems_KeyDown;
            dgv_Note2Items.KeyDown += Dgv_NoteItems_KeyDown;
            dgv_Note3Items.KeyDown += Dgv_NoteItems_KeyDown;

            dgv_Note1Items.CellBeginEdit += (s, e) => RecordCellOldValue(dgv_Note1Items, e);
            dgv_Note2Items.CellBeginEdit += (s, e) => RecordCellOldValue(dgv_Note2Items, e);
            dgv_Note3Items.CellBeginEdit += (s, e) => RecordCellOldValue(dgv_Note3Items, e);


            dgv_Note1Items.CellValueChanged += (s, e) => ModifyCell(1, e);
            dgv_Note2Items.CellValueChanged += (s, e) => ModifyCell(2, e);
            dgv_Note3Items.CellValueChanged += (s, e) => ModifyCell(3, e);
        }

        // 记录原值
        private void RecordCellOldValue(DataGridView dgv, DataGridViewCellCancelEventArgs e)
        {
            var cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            _cellOldValue = cell.Value?.ToString();
        }

        private void Dgv_NoteItems_KeyDown(object sender, KeyEventArgs e)
        {
            var dgv = sender as CtDataGridView;
            if (dgv == null) return;

            // Enter在最后一行自动新增
            if (e.KeyCode == Keys.Enter)
            {
                if (dgv.CurrentRow != null && dgv.CurrentRow.Index == dgv.Rows.Count - 1)
                {
                    dgv.Rows.Add();
                    dgv.CurrentCell = dgv.Rows[dgv.Rows.Count - 1].Cells[0];
                    e.Handled = true;
                }
            }
            // 删除当前行
            else if (e.KeyCode == Keys.Delete)
            {
                if (dgv.CurrentRow != null && !dgv.CurrentRow.IsNewRow)
                {
                    var cellValue = dgv.CurrentRow.Cells[0].Value?.ToString();

                    var result = LocalTranslator.ShowMessage(
                        "确认删除此项吗?", "温馨提示",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int idx = dgv.CurrentRow.Index;
                        dgv.Rows.RemoveAt(idx);
                        // 自动选中下一行
                        if (dgv.Rows.Count > 0)
                        {
                            int nextIdx = Math.Min(idx, dgv.Rows.Count - 1);
                            dgv.CurrentCell = dgv.Rows[nextIdx].Cells[0];
                        }
                        SaveNoteItems(GetNoteIndexByGrid(dgv)); // 删除后同步数据库
                        Logger.Info($"删除行: 索引={idx}, 内容={cellValue}");
                    }
                    e.Handled = true;
                }
            }
        }

        private void ModifyCell(int noteIndex, DataGridViewCellEventArgs e)
        {
            var dgv = GetGridByNoteIndex(noteIndex);
            var row = dgv.Rows[e.RowIndex];
            var value = row.Cells[0].Value?.ToString();

            var result = LocalTranslator.ShowMessage(
                "确认修改此项吗?", "温馨提示",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SaveNoteItems(noteIndex);
                Logger.Info($"修改行: 索引={e.RowIndex}, 新内容={value}");
            }
            else
            {
                // 撤销修改，恢复原值
                row.Cells[0].Value = _cellOldValue;
            }
        }

        // 辅助方法：根据DataGridView获取noteIndex
        private int GetNoteIndexByGrid(DataGridView dgv)
        {
            if (dgv == dgv_Note1Items) return 1;
            if (dgv == dgv_Note2Items) return 2;
            if (dgv == dgv_Note3Items) return 3;
            return 0;
        }
        private DataGridView GetGridByNoteIndex(int noteIndex)
        {
            return noteIndex == 1 ? dgv_Note1Items
                : noteIndex == 2 ? dgv_Note2Items
                : dgv_Note3Items;
        }

        private void Dgv_Note1Items_Leave(object sender, EventArgs e)
        {
            var dgv = sender as CtDataGridView;
            if (dgv != null)
            {
                dgv.ClearSelection();
            }
        }

        // 保存备注名称
        private void SaveNoteName(int noteIndex)
        {
            var dt = My_DataBase.EnabledData.Enabled_set;
            if (dt == null || dt.Rows.Count == 0) return;
            var row = dt.Rows[0];
            string field = $"Note{noteIndex}Name";
            TextBox txt = noteIndex == 1 ? txt_Note1Name
                : noteIndex == 2 ? txt_Note2Name
                : txt_Note3Name;

            string value = txt.Text?.Trim() ?? "";

            // 更新本地 DataTable
            row[field] = value;

            // 更新数据库
            var data = new Dictionary<string, object>
            {
                { field, value }
            };
            My_DataBase.SqlServer.Update(
                My_DataBase.ENABLED_SET.TableName,
                data,
                $"{My_DataBase.ENABLED_SET.MyID}=@MyID",
                new System.Data.SqlClient.SqlParameter("@MyID", row[My_DataBase.ENABLED_SET.MyID])
            );

            My_DataBase.SqlServer.Update(
               My_DataBase.ABS_ENABLED_SET.TableName,
               data,
               $"{My_DataBase.ABS_ENABLED_SET.MyID}=@MyID",
               new System.Data.SqlClient.SqlParameter("@MyID", row[My_DataBase.ABS_ENABLED_SET.MyID])
           );

           
        }

        // 保存备注可选项
        private void SaveNoteItems(int noteIndex)
        {
            var dt = My_DataBase.EnabledData.Enabled_set;
            if (dt == null || dt.Rows.Count == 0) return;
            var row = dt.Rows[0];
            string field = $"Note{noteIndex}Items";
            DataGridView dgv = noteIndex == 1 ? dgv_Note1Items
                : noteIndex == 2 ? dgv_Note2Items
                : dgv_Note3Items;

            var items = dgv.Rows
                .OfType<DataGridViewRow>()
                .Where(r => r.Cells[0].Value != null && !string.IsNullOrWhiteSpace(r.Cells[0].Value.ToString()))
                .Select(r => r.Cells[0].Value.ToString().Trim())
                .ToArray();

            string value = string.Join("|", items);

            // 更新本地 DataTable
            row[field] = value;

            // 更新数据库
            var data = new Dictionary<string, object>
            {
                { field, value }
            };
            My_DataBase.SqlServer.Update(
                My_DataBase.ENABLED_SET.TableName,
                data,
                $"{My_DataBase.ENABLED_SET.MyID}=@MyID",
                new System.Data.SqlClient.SqlParameter("@MyID", row[My_DataBase.ENABLED_SET.MyID])
            );

            My_DataBase.SqlServer.Update(
                My_DataBase.ABS_ENABLED_SET.TableName,
                data,
                $"{My_DataBase.ABS_ENABLED_SET.MyID}=@MyID",
                new System.Data.SqlClient.SqlParameter("@MyID", row[My_DataBase.ABS_ENABLED_SET.MyID])
            );

           
        }
    }
}