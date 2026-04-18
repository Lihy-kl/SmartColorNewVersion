using SmartColor.My_DataBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    public partial class CtRecord : UserControl
    {
        // 分页相关字段
        private int pageSize = 30; // 每页条数，自动计算
        private int pageIndex = 1; // 当前页码
        private int pageCount = 1; // 总页数
        private int recordCount = 0; // 总记录数

        private DataTable dataSource; // 数据源
        private Dictionary<string, string> columnHeaderMap = new Dictionary<string, string>(); // 英文->中文表头

        /// <summary>
        /// 是否切换过页面（首页按钮会重置为false，其它翻页设为true）
        /// </summary>
        public bool HasPageSwitched { get; private set; } = false;
        public event EventHandler HomeButtonClicked;

        public bool SuppressCurrentCellChanged = false;
     
        public CtRecord()
        {
            bds = new BindingSource(); // 添加这一行
            InitializeComponent();
            BindEvents();

        }

        // 在构造函数或 BindEvents 方法中添加 KeyDown 事件绑定
        private void BindEvents()
        {
            tsbtn_ProcessFirstPage.Click += Tsbtn_ProcessFirstPage_Click;
            tsbtn_ProcessUpPage.Click += Tsbtn_ProcessUpPage_Click;
            tsbtn_ProcessDownPage.Click += Tsbtn_ProcessDownPage_Click;
            tsbtn_ProcessEndPage.Click += Tsbtn_ProcessEndPage_Click;
            tstxt_ProcessPageNow.KeyDown += Tstxt_ProcessPageNow_KeyDown;
            this.Resize += CtRecord_Resize;
            dgv.Resize += Dgv_Resize;
            dgv.KeyDown += Dgv_KeyDown_AutoPage; // 新增：键盘上下翻页
        }

        private void UnbindEvents()
        {
            tsbtn_ProcessFirstPage.Click -= Tsbtn_ProcessFirstPage_Click;
            tsbtn_ProcessUpPage.Click -= Tsbtn_ProcessUpPage_Click;
            tsbtn_ProcessDownPage.Click -= Tsbtn_ProcessDownPage_Click;
            tsbtn_ProcessEndPage.Click -= Tsbtn_ProcessEndPage_Click;
            tstxt_ProcessPageNow.KeyDown -= Tstxt_ProcessPageNow_KeyDown;
            this.Resize -= CtRecord_Resize;
            dgv.Resize -= Dgv_Resize;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnbindEvents();
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void CtRecord_Resize(object sender, EventArgs e)
        {
            AutoCalcPageSize();
        }

        private void Dgv_Resize(object sender, EventArgs e)
        {
            AutoCalcPageSize();
        }

        private void Tsbtn_ProcessFirstPage_Click(object sender, EventArgs e)
        {
            HasPageSwitched = false;

            GoToPage(1);

            HomeButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void Tsbtn_ProcessUpPage_Click(object sender, EventArgs e)
        {

            HasPageSwitched = true;

            GoToPage(pageIndex - 1);

        }

        private void Tsbtn_ProcessDownPage_Click(object sender, EventArgs e)
        {
            HasPageSwitched = true;

            GoToPage(pageIndex + 1);

        }

        private void Tsbtn_ProcessEndPage_Click(object sender, EventArgs e)
        {
            HasPageSwitched = true;

            GoToPage(pageCount);

        }

        /// <summary>
        /// 设置英文字段名与中文表头映射
        /// </summary>
        /// <param name="headerMap">key:英文字段名，value:中文表头</param>
        public void SetColumnHeaders(Dictionary<string, string> headerMap)
        {
            columnHeaderMap = headerMap ?? new Dictionary<string, string>();
            UpdateColumnHeaders();
        }

        /// <summary>
        /// 设置数据源（英文字段名）
        /// </summary>
        public void SetDataSource(DataTable dt)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<DataTable>(SetDataSource), dt);
                return;
            }
            dataSource = dt;
            recordCount = dt?.Rows.Count ?? 0;
            AutoCalcPageSize();
            pageCount = Math.Max(1, (int)Math.Ceiling(recordCount / (double)pageSize));
            pageIndex = 1;
            RefreshPage();
            if (dt != null)
            {
                this.dgv.AutoFitAllColumns(); // 新增：自动调整列宽
                                              // 关键：控件渲染后再重新计算一次分页
                if (this.IsHandleCreated)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        AutoCalcPageSize();
                    }));
                }
            }
        }

        /// <summary>
        /// 设置数据源和自定义列标题
        /// </summary>
        public void SetDataSource(IEnumerable<string> columns, IEnumerable<object[]> rows)
        {
           

            var dt = new DataTable();
            foreach (var col in columns)
                dt.Columns.Add(col);

            foreach (var row in rows)
                dt.Rows.Add(row);

            SetDataSource(dt);
            dgv.AutoFitAllColumns(); // 新增：自动调整列宽
        }

        /// <summary>
        /// 刷新当前页数据
        /// </summary>
        private void RefreshPage()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RefreshPage));
                return;
            }
            if (dataSource == null)
            {
                bds.DataSource = null;
                dgv.DataSource = bds;
                SafeSetText(tslab_ProcessAllNum, "0");
                SafeSetText(tslab_ProcessAllPage, "1");
                SafeSetText(tstxt_ProcessPageNow, "1");
                dgv.AutoFitAllColumns();
                dgv.ScrollBars = ScrollBars.None;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                return;
            }

            int start = (pageIndex - 1) * pageSize;
            int end = Math.Min(start + pageSize, recordCount);

            // 优化分页数据生成
            DataTable pageTable;
            if (dataSource != null && dataSource.Rows.Count > 0)
            {
                var rows = dataSource.AsEnumerable().Skip(start).Take(end - start);
                pageTable = rows.Any() ? rows.CopyToDataTable() : dataSource.Clone();
            }
            else
            {
                pageTable = dataSource?.Clone();
            }

            bds.DataSource = pageTable;
            dgv.DataSource = bds;

            SafeSetText(tslab_ProcessAllNum, recordCount.ToString());
            SafeSetText(tslab_ProcessAllPage, pageCount.ToString());
            SafeSetText(tstxt_ProcessPageNow, pageIndex.ToString());

            tsbtn_ProcessFirstPage.Enabled = pageIndex > 1;
            tsbtn_ProcessUpPage.Enabled = pageIndex > 1;
            tsbtn_ProcessDownPage.Enabled = pageIndex < pageCount;
            tsbtn_ProcessEndPage.Enabled = pageIndex < pageCount;

            UpdateColumnHeaders();
            dgv.AutoFitAllColumns();
            dgv.ClearSelection();
            dgv.ScrollBars = ScrollBars.None;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 新增：强制刷新导航条，确保ToolStripLabel立即更新
            bdn.Refresh();
            bdn.Invalidate();
        }

        /// <summary>
        /// 安全设置ToolStripItem文本
        /// </summary>
        private void SafeSetText(ToolStripItem item, string text)
        {
            if (item is ToolStripLabel label && label != null && !label.IsDisposed)
                label.Text = text;
            else if (item is ToolStripTextBox txt && txt != null && !txt.IsDisposed)
            {
                txt.Text = text;

                txt.Control?.Refresh();
            }
        }

        /// <summary>
        /// 自动计算每页显示条数
        /// </summary>
        private void AutoCalcPageSize()
        {
            // 用实际行高更精确
            int rowHeight = dgv.Rows.Count > 0 ? dgv.Rows[0].Height : (dgv.RowTemplate.Height > 0 ? dgv.RowTemplate.Height : 27);
            int availableHeight = dgv.ClientSize.Height - dgv.ColumnHeadersHeight;

            // 计算边框和可能的滚动条高度
            int extraHeight = dgv.ClientRectangle.Height - dgv.DisplayRectangle.Height;
            availableHeight -= extraHeight;

            int page = availableHeight / rowHeight;
            if (page < 1) page = 1;
            if (page != pageSize)
            {
                pageSize = page;
                if (dataSource != null)
                {
                    pageCount = Math.Max(1, (int)Math.Ceiling(recordCount / (double)pageSize));
                    pageIndex = Math.Min(pageIndex, pageCount);
                    RefreshPage();
                }
            }
        }

        private void GoToPage(int page)
        {
            SuppressCurrentCellChanged = true;
            if (page < 1) page = 1;
            if (page > pageCount) page = pageCount;
            pageIndex = page;
            RefreshPage();
            SuppressCurrentCellChanged = false;
        }

        private void Tstxt_ProcessPageNow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (tstxt_ProcessPageNow != null && !tstxt_ProcessPageNow.IsDisposed)
                {
                    if (int.TryParse(tstxt_ProcessPageNow.Text, out int page))
                    {

                        GoToPage(page);

                    }

                }
            }
        }

        /// <summary>
        /// 根据映射刷新DataGridView表头
        /// </summary>
        private void UpdateColumnHeaders()
        {
            if (dgv.DataSource == null || columnHeaderMap == null) return;
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (!col.Visible) continue; // 只处理可见列
                if (columnHeaderMap.TryGetValue(col.Name, out string header))
                    col.HeaderText = header;
                else
                    col.HeaderText = col.Name; // 没有映射则用原名
            }
        }

        // 新增：键盘上下翻页功能
        private void Dgv_KeyDown_AutoPage(object sender, KeyEventArgs e)
        {
            if (dgv.Rows.Count == 0) return;
            int rowIndex = dgv.CurrentCell?.RowIndex ?? -1;
            if (e.KeyCode == Keys.Down && rowIndex == dgv.Rows.Count - 1)
            {
                if (pageIndex < pageCount)
                {
                    GoToPage(pageIndex + 1);
                    if (dgv.Rows.Count > 0)
                    {
                        dgv.CurrentCell = dgv.Rows[0].Cells[0];
                        dgv.Rows[0].Selected = true;
                    }
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Up && rowIndex == 0)
            {
                if (pageIndex > 1)
                {
                    GoToPage(pageIndex - 1);
                    int lastRow = dgv.Rows.Count - 1;
                    if (lastRow >= 0)
                    {
                        dgv.CurrentCell = dgv.Rows[lastRow].Cells[0];
                        dgv.Rows[lastRow].Selected = true;
                    }
                    e.Handled = true;
                }
            }
        }
    }
}