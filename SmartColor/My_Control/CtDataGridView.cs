using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 支持自定义回车跳转和编辑行为的DataGridView组件
    /// </summary>
    public partial class CtDataGridView : DataGridView
    {
        /// <summary>
        /// 是否处于自定义编辑模式（外部控制，决定是否拦截回车）
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("是否处于自定义编辑模式，决定是否拦截回车并执行自定义逻辑")]
        public bool CustomEditing { get; set; } = false;

        /// <summary>
        /// 回车自定义跳转事件（返回true则阻止默认行为）
        /// </summary>
        [Browsable(false)]
        public event Func<bool> EnterKeyAction;

        public CtDataGridView()
        {
            InitializeComponent();
            this.EditingControlShowing += CtDataGridView_EditingControlShowing;
            this.BackgroundColor = System.Drawing.SystemColors.Control;
            this.RowHeadersVisible = false;
            this.AllowUserToResizeColumns = false;
            this.AllowUserToResizeRows = false;
            // 关闭所有列的排序功能
            this.ColumnAdded += (s, e) =>
            {
                e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
            };
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

        }

        /// <summary>
        /// 通用绑定DataTable到DataGridView
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="rowSelector">行数据选择器</param>
        /// <param name="requiredColumns">必须不为DBNull的列名（如主键）</param>
        public void BindDataTable(DataTable dt,
            Func<DataRow, object[]> rowSelector,
            params string[] requiredColumns)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => BindDataTable(dt, rowSelector, requiredColumns)));
                return;
            }
            if (this.IsDisposed || !this.IsHandleCreated)
                return;
            if (dt == null || dt.Columns.Count == 0)
            {
                this.Rows.Clear();
                this.Columns.Clear();
                return; // 或者 throw new ArgumentException("DataTable 没有列，无法绑定。");
            }

            // 如果没有列，自动生成
            if (this.Columns.Count == 0)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    this.Columns.Add(col.ColumnName, col.ColumnName);
                }
            }

            this.Rows.Clear();
            if (dt == null) return;

            var pkCol = requiredColumns[0];
            var colType = dt.Columns[pkCol].DataType;

            IEnumerable<DataRow> drs;

            if (colType == typeof(string))
            {
                drs = dt.AsEnumerable()
                    .Where(r => requiredColumns.All(col => r[col] != DBNull.Value))
                    .GroupBy(r => r.Field<string>(pkCol))
                    .Select(g => g.First());
            }
            else if (colType == typeof(int))
            {
                drs = dt.AsEnumerable()
                    .Where(r => requiredColumns.All(col => r[col] != DBNull.Value))
                    .GroupBy(r => r.Field<int>(pkCol))
                    .Select(g => g.First());
            }
            else if (colType == typeof(long))
            {
                drs = dt.AsEnumerable()
                    .Where(r => requiredColumns.All(col => r[col] != DBNull.Value))
                    .GroupBy(r => r.Field<long>(pkCol))
                    .Select(g => g.First());
            }
            else if (colType == typeof(DateTime))
            {
                drs = dt.AsEnumerable()
                    .Where(r => requiredColumns.All(col => r[col] != DBNull.Value))
                    .GroupBy(r => r.Field<DateTime>(pkCol))
                    .Select(g => g.First());
            }
            else
            {
                // 兜底方案，仍用 object，但建议尽量避免
                drs = dt.AsEnumerable()
                    .Where(r => requiredColumns.All(col => r[col] != DBNull.Value))
                    .GroupBy(r => r[pkCol])
                    .Select(g => g.First());
            }

            foreach (var row in drs)
            {
                this.Rows.Add(rowSelector(row));
            }
            this.ClearSelection();
            this.AutoFitAllColumns();
        }



        /// <summary>
        /// 绑定所有常见编辑控件的KeyDown事件
        /// </summary>
        private void CtDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // TextBox
            if (e.Control is TextBox tb)
            {
                tb.KeyDown -= EditingControl_KeyDown;
                tb.KeyDown += EditingControl_KeyDown;
            }
            // ComboBox
            else if (e.Control is ComboBox cb)
            {
                cb.KeyDown -= EditingControl_KeyDown;
                cb.KeyDown += EditingControl_KeyDown;
            }
            // CheckBox
            else if (e.Control is CheckBox chk)
            {
                chk.KeyDown -= EditingControl_KeyDown;
                chk.KeyDown += EditingControl_KeyDown;
            }
            // DateTimePicker
            else if (e.Control is DateTimePicker dtp)
            {
                dtp.KeyDown -= EditingControl_KeyDown;
                dtp.KeyDown += EditingControl_KeyDown;
            }
            // NumericUpDown
            else if (e.Control is NumericUpDown nud)
            {
                nud.KeyDown -= EditingControl_KeyDown;
                nud.KeyDown += EditingControl_KeyDown;
            }
            // MaskedTextBox
            else if (e.Control is MaskedTextBox mtb)
            {
                mtb.KeyDown -= EditingControl_KeyDown;
                mtb.KeyDown += EditingControl_KeyDown;
            }
            // 其它自定义控件可在此扩展
        }

        /// <summary>
        /// 编辑控件回车拦截
        /// </summary>
        private void EditingControl_KeyDown(object sender, KeyEventArgs e)
        {
            // ComboBox下拉时不处理
            if (sender is ComboBox cb && cb.DroppedDown)
                return;
            if (e.KeyCode == Keys.Enter && CustomEditing)
            {
                if (EnterKeyAction != null && EnterKeyAction())
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        /// <summary>
        /// 彻底拦截DataGridView的回车行为
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter && CustomEditing)
            {
                if (EnterKeyAction != null && EnterKeyAction())
                {
                    return true; // 阻止默认行为
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Enter && CustomEditing)
            {
                // 当前列是CheckBox列时，手动触发跳转
                if (this.CurrentCell != null && this.Columns[this.CurrentCell.ColumnIndex] is DataGridViewCheckBoxColumn)
                {
                    if (EnterKeyAction != null && EnterKeyAction())
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                }
            }
        }

        /// <summary>
        /// 智能自适应所有列宽度，确保列头和内容都能完整显示且不过宽，列头始终一行，
        /// 多余宽度平均分配到所有可见列
        /// </summary>
        public void AutoFitAllColumns()
        {
            if (this.IsDisposed)
                return;

            // 如果还没创建句柄，等创建后再自适应
            if (!this.IsHandleCreated)
            {
                void handler(object s, EventArgs e)
                {
                    this.HandleCreated -= handler;
                    // 用BeginInvoke避免阻塞
                    this.BeginInvoke(new Action(AutoFitAllColumns));
                }
                this.HandleCreated += handler;
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(AutoFitAllColumns));
                return;
            }
            if (this.ColumnCount == 0) return;

            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

            int[] colWidths = new int[this.ColumnCount];
            int totalWidth = 0;
            int visibleCount = 0;
            using (var g = this.CreateGraphics())
            {
                for (int i = 0; i < this.ColumnCount; i++)
                {
                    var col = this.Columns[i];
                    if (!col.Visible)
                    {
                        colWidths[i] = 0;
                        continue;
                    }
                    visibleCount++;
                    var headerText = col.HeaderText ?? string.Empty;
                    var headerSize = TextRenderer.MeasureText(headerText, col.InheritedStyle.Font ?? this.Font, new System.Drawing.Size(int.MaxValue, col.HeaderCell.Size.Height), TextFormatFlags.SingleLine);
                    int maxWidth = headerSize.Width;
                    foreach (DataGridViewRow row in this.Rows)
                    {
                        if (row.IsNewRow) continue;
                        try
                        {
                            var cell = row.Cells[i];
                            var cellValue = cell.FormattedValue?.ToString() ?? string.Empty;
                            var cellSize = TextRenderer.MeasureText(cellValue, cell.InheritedStyle.Font ?? col.InheritedStyle.Font ?? this.Font, new System.Drawing.Size(int.MaxValue, cell.Size.Height), TextFormatFlags.SingleLine);
                            if (cellSize.Width > maxWidth)
                                maxWidth = cellSize.Width;
                        }
                        catch
                        {
                            // 某些特殊类型或索引异常时，跳过该单元格
                            continue;
                        }
                    }
                    maxWidth += 16;
                    int maxLimit = 1000;
                    int minLimit = 40;
                    maxWidth = Math.Max(minLimit, Math.Min(maxWidth, maxLimit));
                    colWidths[i] = maxWidth;
                    totalWidth += maxWidth;
                }
            }

            // 更精确的可用宽度
            int availableWidth = this.DisplayRectangle.Width;
            if (this.RowHeadersVisible)
                availableWidth -= this.RowHeadersWidth;

            // 更精确判断垂直滚动条
            bool hasVScroll = false;
            foreach (Control c in this.Controls)
            {
                if (c is VScrollBar vsb && vsb.Visible)
                {
                    hasVScroll = true;
                    break;
                }
            }
            if (hasVScroll)
                availableWidth -= SystemInformation.VerticalScrollBarWidth;

            // 预留边框误差
            availableWidth -= 2;

            // 横向误差
            int diff = totalWidth - availableWidth;

            if (totalWidth > availableWidth && availableWidth > 0)
            {
                double scale = (double)availableWidth / totalWidth;
                int sum = 0;
                for (int i = 0; i < this.ColumnCount; i++)
                {
                    var col = this.Columns[i];
                    if (!col.Visible) continue;
                    int newWidth = (int)(colWidths[i] * scale);
                    if (i == this.ColumnCount - 1)
                        newWidth = availableWidth - sum; // 最后一列补齐
                    col.Width = Math.Max(40, newWidth);
                    sum += col.Width;
                }
            }
            else if (totalWidth < availableWidth && visibleCount > 0)
            {
                int extra = (availableWidth - totalWidth) / visibleCount;
                int remainder = (availableWidth - totalWidth) % visibleCount;
                int visibleIndex = 0;
                int sum = 0;
                for (int i = 0; i < this.ColumnCount; i++)
                {
                    var col = this.Columns[i];
                    if (!col.Visible) continue;
                    int add = extra + (visibleIndex < remainder ? 1 : 0);
                    int newWidth = colWidths[i] + add;
                    if (i == this.ColumnCount - 1)
                        newWidth = availableWidth - sum; // 最后一列补齐
                    col.Width = newWidth;
                    sum += col.Width;
                    visibleIndex++;
                }
            }
            else
            {
                for (int i = 0; i < this.ColumnCount; i++)
                {
                    var col = this.Columns[i];
                    if (!col.Visible) continue;
                    col.Width = colWidths[i];
                }
            }

            // 强制关闭横向滚动条（如果误差很小，比如8像素以内）
            if (diff > 0 && diff <= 8)
            {
                this.ScrollBars = ScrollBars.Vertical;
                // 再强制最后一列减去误差
                int lastVisibleCol = -1;
                for (int i = this.ColumnCount - 1; i >= 0; i--)
                {
                    if (this.Columns[i].Visible)
                    {
                        lastVisibleCol = i;
                        break;
                    }
                }
                if (lastVisibleCol >= 0)
                {
                    int reduce = diff;
                    int newWidth = Math.Max(40, this.Columns[lastVisibleCol].Width - reduce);
                    this.Columns[lastVisibleCol].Width = newWidth;
                }
            }
            else
            {
                this.ScrollBars = ScrollBars.Both;
            }
        }
    }
}