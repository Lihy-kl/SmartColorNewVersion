using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 支持自动展开/折叠的明细表DataGridView控件。
    /// 继承自 CtDetailDataGridView，具备所有明细表通用功能，并扩展了折叠/展开功能。
    /// 适用于主明细表可折叠场景（如配方主表），可通过点击表头进行折叠/展开。
    /// </summary>
    public partial class CtFoldDataGridView : CtDataGridView
    {
        // 是否处于折叠状态
        private bool _isFolded = false;
        // 折叠时的控件高度（只显示表头）
        private int _foldedHeight = 40;

        /// <summary>
        /// 折叠/展开状态变化事件（用于通知父容器刷新布局）
        /// </summary>
        public event EventHandler ExpandRequested;

        /// <summary>
        /// 构造函数，初始化控件和折叠相关事件
        /// </summary>
        public CtFoldDataGridView()
        {
            InitializeComponent();
            InitFoldEvents();
            this.KeyDown += CtFoldDataGridView_KeyDown;
        }

        /// <summary>
        /// 构造函数（设计器支持），初始化控件和折叠相关事件
        /// </summary>
        public CtFoldDataGridView(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            InitFoldEvents();
            this.KeyDown += CtFoldDataGridView_KeyDown;
        }

        /// <summary>
        /// 是否处于折叠状态
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("是否处于折叠状态")]
        public bool IsFolded
        {
            get => _isFolded;
            set
            {
                _isFolded = value;
                UpdateFoldState();
            }
        }

        /// <summary>
        /// 折叠时的高度（只显示表头）
        /// </summary>
        [Browsable(true)]
        [DefaultValue(40)]
        [Description("折叠时的高度")]
        public int FoldedHeight
        {
            get => _foldedHeight;
            set
            {
                _foldedHeight = value;
                if (_isFolded)
                    Height = _foldedHeight;
            }
        }

        /// <summary>
        /// 初始化折叠/展开相关事件（表头点击、控件失焦等）
        /// </summary>
        private void InitFoldEvents()
        {
            // 点击表头时切换折叠/展开状态
            this.ColumnHeaderMouseClick += CtFoldDataGridView_ColumnHeaderMouseClick;
            // 失去焦点时清除选中
            this.Leave += CtFoldDataGridView_Leave;
            // 初始化时根据当前状态刷新显示
            UpdateFoldState();
        }

        /// <summary>
        /// 失去焦点时清除选中行
        /// </summary>
        private void CtFoldDataGridView_Leave(object sender, EventArgs e)
        {
            this.ClearSelection();
        }

        /// <summary>
        /// 表头点击事件，切换折叠/展开状态
        /// </summary>
        private void CtFoldDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (IsFolded)
            {
                Expand();
            }
            else
            {
                Fold();
            }
        }

        /// <summary>
        /// 展开控件（显示所有行，填满父容器）
        /// </summary>
        public void Expand()
        {
            IsFolded = false;
        }

        /// <summary>
        /// 折叠控件（只显示表头，隐藏所有数据行）
        /// </summary>
        public void Fold()
        {
            IsFolded = true;
        }

        /// <summary>
        /// 根据折叠状态更新控件高度、Dock方式和行可见性
        /// </summary>
        private void UpdateFoldState()
        {
            if (_isFolded)
            {
                // 折叠时只显示表头，Dock到顶部
                Dock = DockStyle.Top;
                Height = ColumnHeadersHeight;
                foreach (DataGridViewRow row in this.Rows)
                    if (!row.IsNewRow) row.Visible = false;
            }
            else
            {
                // 展开时显示所有行，Dock填满父容器
                foreach (DataGridViewRow row in this.Rows)
                    row.Visible = true;
                Dock = DockStyle.Fill;
            }
            // 通知父容器刷新布局
            ExpandRequested?.Invoke(this, EventArgs.Empty);
        }

        private void CtFoldDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (this.CurrentRow != null && !this.CurrentRow.IsNewRow)
                {
                    var cr = this.CurrentRow;
                    // 判断当前行第一列是否为空
                    var cellValue = cr.Cells.Count > 0 ? cr.Cells[0].Value : null;
                    if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
                    {
                        this.Rows.Remove(cr);
                    }
                    // 如果不为空则不允许删除，直接忽略
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// 获取推荐控件高度（用于父容器自适应布局）
        /// 折叠时只显示表头，展开时根据所有行高度自动计算
        /// </summary>
        public int GetRecommendedHeight()
        {
            if (_isFolded)
            {
                // 折叠时只显示表头
                return ColumnHeadersHeight;
            }

            int totalRowHeight = 0;
            int newRowHeight = 0;
            bool hasNewRow = false;

            // 统计所有可见行的高度
            foreach (DataGridViewRow row in this.Rows)
            {
                if (row.Visible)
                {
                    if (row.IsNewRow)
                    {
                        if (!hasNewRow)
                        {
                            newRowHeight = row.Height;
                            hasNewRow = true;
                        }
                    }
                    else
                    {
                        totalRowHeight += row.Height;
                    }
                }
            }

            int borderHeight = 2;
            int minHeight = this.ColumnHeadersHeight + 20;
            int height = this.ColumnHeadersHeight + totalRowHeight + newRowHeight + borderHeight;

            // 如果有横向滚动条，额外加高度
            height += SystemInformation.HorizontalScrollBarHeight + 8;

            // 返回推荐高度（不小于最小高度）
            return Math.Max(height, minHeight);
        }

      
    }
}