using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    public partial class CtDHDataGridView : CtDataGridView
    {
        public List<string> AssistantCodes { get; set; } = new List<string>();

        // 记录表头选中列
        private int _headerSelectedColumnIndex = -1;
        public bool IsLoading { get; set; } = false;

        public CtDHDataGridView()
        {
            this.CellPainting += DiagonalHeaderDataGridView_CellPainting;
            this.CellValueChanged += CtDHDataGridView_CellValueChanged;
            this.CurrentCellDirtyStateChanged += CtDHDataGridView_CurrentCellDirtyStateChanged;
            this.ColumnHeaderMouseDoubleClick += CtDHDataGridView_ColumnHeaderMouseDoubleClick;
            this.CellValidating += CtDHDataGridView_CellValidating;
            this.CellClick += CtDHDataGridView_CellClick;

            // 新增：拦截表头插入/删除行为
            this.ColumnAdded += CtDHDataGridView_ColumnAdded;
            this.ColumnRemoved += CtDHDataGridView_ColumnRemoved;

            // 基础属性
            this.BackgroundColor = SystemColors.Control;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = true;
            this.RowHeadersVisible = false;
            this.AutoGenerateColumns = false;
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.ColumnHeadersHeight = 40;
            this.AllowUserToOrderColumns = false;
            this.AllowUserToResizeColumns = true;
            this.AllowUserToResizeRows = true;
            this.EnableHeadersVisualStyles = false;
            // 居中表头
            this.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // 居中单元格
            this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // 智能调整区间列（插入/删除后）
        private void CtDHDataGridView_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            // 只处理区间列（跳过助剂代码列）
            int colIndex = e.Column.Index;
            if (colIndex <= 0) return;
            AdjustRangeHeaders(colIndex);
        }

        private void CtDHDataGridView_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            // 只处理区间列（跳过助剂代码列）
            int colIndex = e.Column.Index;
            if (colIndex <= 0) return;
            AdjustRangeHeaders(colIndex);
        }

        // 表头点击，记录选中列并高亮
        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnColumnHeaderMouseClick(e);
            this.ClearSelection();

            // 如果有正在编辑的单元格，结束编辑
            if (this.IsCurrentCellInEditMode)
            {
                this.EndEdit();
            }
            // 清除当前行选中
            this.CurrentCell = null;

            if (e.ColumnIndex > 0)
            {
                _headerSelectedColumnIndex = e.ColumnIndex;
                HighlightHeader(e.ColumnIndex);
            }
            else
            {
                _headerSelectedColumnIndex = -1;
                HighlightHeader(-1);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // 仅在表头选中时响应
            if (_headerSelectedColumnIndex > 0 && this.CustomEditing && !this.ReadOnly)
            {
                if (e.KeyCode == Keys.Insert)
                {
                    // 在选中列后插入新区间列
                    string curHeader = this.Columns[_headerSelectedColumnIndex].HeaderText;
                    var match = Regex.Match(curHeader, @"-(?<max>-?\d+(\.\d+)?|∞)$");
                    string min = match.Success ? match.Groups["max"].Value : "0";
                    string max = "∞";
                    var matchNext = (_headerSelectedColumnIndex < this.Columns.Count - 1)
                        ? Regex.Match(this.Columns[_headerSelectedColumnIndex + 1].HeaderText, @"-(?<max>-?\d+(\.\d+)?|∞)$")
                        : null;
                    if (matchNext != null && matchNext.Success)
                        max = matchNext.Groups["max"].Value;

                    var newCol = new DataGridViewTextBoxColumn
                    {
                        HeaderText = $"{min}-{max}",
                        Name = "Range_" + this.Columns.Count,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    };
                    this.Columns.Insert(_headerSelectedColumnIndex + 1, newCol);
                    AdjustRangeHeaders(_headerSelectedColumnIndex + 1);
                    HighlightHeader(_headerSelectedColumnIndex + 1);
                    _headerSelectedColumnIndex++;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    // 删除选中区间列（保留助剂代码列和至少一个区间列）
                    if (this.Columns.Count > 2)
                    {
                        this.Columns.RemoveAt(_headerSelectedColumnIndex);
                        _headerSelectedColumnIndex = -1;
                        HighlightHeader(-1);
                        AdjustRangeHeaders(1);
                        e.Handled = true;
                    }
                }
            }
        }

        // 单元格点击，清除表头选中
        private void CtDHDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                _headerSelectedColumnIndex = -1;
                HighlightHeader(-1);
            }
        }

        // 高亮表头
        private void HighlightHeader(int colIndex)
        {
            foreach (DataGridViewColumn col in this.Columns)
            {
                col.HeaderCell.Style.BackColor = SystemColors.Control;
            }
            if (colIndex > 0 && colIndex < this.Columns.Count)
            {
                this.Columns[colIndex].HeaderCell.Style.BackColor = Color.LightBlue;
            }
            this.Invalidate();
        }

        // 双击范围表头弹窗，智能区间输入
        private void CtDHDataGridView_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex > 0 && this.CustomEditing && !this.ReadOnly)
            {
                string minValue = "0";
                if (e.ColumnIndex > 1)
                {
                    string prevHeader = this.Columns[e.ColumnIndex - 1].HeaderText;
                    var match = Regex.Match(prevHeader, @"-(?<max>-?\d+(\.\d+)?|∞)$");
                    if (match.Success)
                        minValue = match.Groups["max"].Value;
                }
                string oldHeader = this.Columns[e.ColumnIndex].HeaderText;
                string maxValue = "∞";
                var match2 = Regex.Match(oldHeader, @"-(?<max>-?\d+(\.\d+)?|∞)$");
                if (match2.Success)
                    maxValue = match2.Groups["max"].Value;

                using (var input = new RangeInputBoxForm(minValue, maxValue))
                {
                    if (input.ShowDialog() == DialogResult.OK)
                    {
                        string minStr = input.MinValue;
                        string maxStr = input.MaxValue;
                        double min, max;
                        bool isInf = maxStr == "∞";
                        if (!double.TryParse(minStr, out min)) min = 0;
                        if (!isInf)
                        {
                            double.TryParse(maxStr, out max);
                            if (max <= min)
                            {
                                MessageBox.Show("结束值必须大于起始值", "提示");
                                return;
                            }
                        }
                        if (e.ColumnIndex == this.Columns.Count - 1 && !isInf)
                        {
                            this.Columns[e.ColumnIndex].HeaderText = $"{minStr}-{maxStr}";
                            AddRangeColumn($"{maxStr}-∞");
                            return;
                        }
                        this.Columns[e.ColumnIndex].HeaderText = $"{minStr}-{maxStr}";
                        if (isInf && e.ColumnIndex < this.Columns.Count - 1)
                        {
                            for (int i = this.Columns.Count - 1; i > e.ColumnIndex; i--)
                                this.Columns.RemoveAt(i);
                        }
                        // 智能调整后续区间
                        AdjustRangeHeaders(e.ColumnIndex + 1);
                    }
                }
            }
        }

        // 范围列只允许输入浮点数
        private void CtDHDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex > 0 && e.RowIndex >= 0)
            {
                string value = e.FormattedValue?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    double d;
                    if (!double.TryParse(value, out d))
                    {
                        this.Rows[e.RowIndex].ErrorText = "请输入有效的数字";
                        e.Cancel = true;
                    }
                    else
                    {
                        this.Rows[e.RowIndex].ErrorText = "";
                    }
                }
                else
                {
                    this.Rows[e.RowIndex].ErrorText = "";
                }
            }
        }

        public void InitGrid(List<string> assistantCodes)
        {
            this.Columns.Clear();
            this.Rows.Clear();
            this.AssistantCodes = assistantCodes;

            var comboCol = new DataGridViewComboBoxColumn
            {
                HeaderText = "",
                Name = "AssistantCode",
                DataSource = assistantCodes,
                Width = 100,
                FlatStyle = FlatStyle.Popup,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            this.Columns.Add(comboCol);

            var txtCol = new DataGridViewTextBoxColumn
            {
                HeaderText = "0-∞",
                Name = "Range_1",
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            this.Columns.Add(txtCol);

            this.Rows.Add();
        }

        // 保证始终有一个空行和空列
        private void CtDHDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (IsLoading) return;
            if (e.RowIndex >= 0 && IsLastRowFilled())
            {
                this.Rows.Add();
            }
            if (e.ColumnIndex >= 0 && IsLastColumnFilled())
            {
                string lastHeader = this.Columns[this.Columns.Count - 1].HeaderText;
                if (!lastHeader.Trim().EndsWith("-∞"))
                {
                    var match = Regex.Match(lastHeader, @"-(?<max>-?\d+(\.\d+)?|∞)$");
                    if (match.Success)
                    {
                        string maxStr = match.Groups["max"].Value;
                        if (maxStr != "∞")
                        {
                            AddRangeColumn($"{maxStr}-∞");
                        }
                    }
                }
            }
        }

        private void CtDHDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.IsCurrentCellDirty)
            {
                this.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private bool IsLastRowFilled()
        {
            if (this.Rows.Count == 0) return false;
            var lastRow = this.Rows[this.Rows.Count - 1];
            foreach (DataGridViewCell cell in lastRow.Cells)
            {
                if (cell.ColumnIndex == 0) continue;
                if (cell.Value != null && !string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    return true;
            }
            return false;
        }

        private bool IsLastColumnFilled()
        {
            if (this.Columns.Count == 0) return false;
            int lastCol = this.Columns.Count - 1;
            foreach (DataGridViewRow row in this.Rows)
            {
                if (row.IsNewRow) continue;
                var cell = row.Cells[lastCol];
                if (cell.Value != null && !string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    return true;
            }
            return false;
        }

        public void AddRangeColumn(string header = "")
        {
            var txtCol = new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                Name = "Range_" + this.Columns.Count,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            this.Columns.Add(txtCol);
        }

        private void DiagonalHeaderDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex == 0)
            {
                e.PaintBackground(e.ClipBounds, false);

                using (Pen pen = new Pen(Color.Black, 1))
                {
                    e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Right, e.CellBounds.Bottom);
                }

                var font = e.CellStyle.Font;
                var leftText = "助剂代码";
                var rightText = "范围";
                var leftSize = TextRenderer.MeasureText(leftText, font);
                var rightSize = TextRenderer.MeasureText(rightText, font);

                TextRenderer.DrawText(
                    e.Graphics, leftText, font,
                    new Rectangle(e.CellBounds.Left + 4, e.CellBounds.Bottom - leftSize.Height - 2, leftSize.Width, leftSize.Height),
                    e.CellStyle.ForeColor, TextFormatFlags.Left | TextFormatFlags.Bottom);

                TextRenderer.DrawText(
                    e.Graphics, rightText, font,
                    new Rectangle(e.CellBounds.Right - rightSize.Width - 4, e.CellBounds.Top + 2, rightSize.Width, rightSize.Height),
                    e.CellStyle.ForeColor, TextFormatFlags.Right | TextFormatFlags.Top);

                e.Handled = true;
            }
        }

        /// <summary>
        /// 智能调整区间列表头：后续区间的最小值自动等于前一列最大值
        /// </summary>
        private void AdjustRangeHeaders(int startCol)
        {
            for (int i = startCol; i < this.Columns.Count; i++)
            {
                if (i == 0) continue; // 跳过助剂代码列
                // 获取前一列的最大值
                string prevHeader = this.Columns[i - 1].HeaderText;
                var matchPrev = Regex.Match(prevHeader, @"-(?<max>-?\d+(\.\d+)?|∞)$");
                if (!matchPrev.Success) continue;
                string prevMax = matchPrev.Groups["max"].Value;

                // 获取当前列的最大值
                string curHeader = this.Columns[i].HeaderText;
                var matchCur = Regex.Match(curHeader, @"^(?<min>-?\d+(\.\d+)?)-(?<max>-?\d+(\.\d+)?|∞)$");
                string curMax = matchCur.Success ? matchCur.Groups["max"].Value : "∞";

                // 自动调整当前列的最小值为前一列最大值
                this.Columns[i].HeaderText = $"{prevMax}-{curMax}";
            }
        }
    }

    // 区间输入窗体
    public class RangeInputBoxForm : Form
    {
        public string MinValue => txtMin.Text.Trim();
        public string MaxValue => txtMax.Text.Trim();

        private TextBox txtMin;
        private TextBox txtMax;

        public RangeInputBoxForm(string minValue, string maxValue)
        {
            this.Text = "请输入范围";
            this.Width = 320;
            this.Height = 150;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblMin = new Label { Text = "起始：", Left = 20, Top = 18, Width = 50 };
            txtMin = new TextBox { Left = 70, Top = 15, Width = 80, Text = minValue, ReadOnly = true };

            Label lblDash = new Label { Text = "-", Left = 155, Top = 18, Width = 20, TextAlign = ContentAlignment.MiddleCenter };

            Label lblMax = new Label { Text = "结束：", Left = 180, Top = 18, Width = 50 };
            txtMax = new TextBox { Left = 230, Top = 15, Width = 60, Text = maxValue };

            var btnOK = new Button { Text = "确定", Left = 90, Width = 60, Top = 60, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "取消", Left = 170, Width = 60, Top = 60, DialogResult = DialogResult.Cancel };

            this.Controls.Add(lblMin);
            this.Controls.Add(txtMin);
            this.Controls.Add(lblDash);
            this.Controls.Add(lblMax);
            this.Controls.Add(txtMax);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            this.Shown += (s, e) =>
            {
                txtMax.Focus();
                txtMax.SelectAll();
            };
        }
    }
}