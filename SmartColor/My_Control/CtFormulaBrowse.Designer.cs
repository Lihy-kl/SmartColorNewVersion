namespace SmartColor.My_Control
{
    partial class CtFormulaBrowse
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                this._refreshTimer.Stop();
                this._refreshTimer.Dispose();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtFormulaBrowse));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ctDataGridView1 = new SmartColor.My_Control.CtDataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingNavigator1 = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rdo_Wait = new System.Windows.Forms.RadioButton();
            this.rdo_Browse_condition = new System.Windows.Forms.RadioButton();
            this.dt_Browse_End = new System.Windows.Forms.DateTimePicker();
            this.dt_Browse_Start = new System.Windows.Forms.DateTimePicker();
            this.txt_Browse_Code = new System.Windows.Forms.TextBox();
            this.btn_Browse_Delete = new System.Windows.Forms.Button();
            this.btn_Browse_Select = new System.Windows.Forms.Button();
            this.rdo_Browse_NoDrop = new System.Windows.Forms.RadioButton();
            this.rdo_Browse_All = new System.Windows.Forms.RadioButton();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Tsm_PS = new System.Windows.Forms.ToolStripMenuItem();
            this.Tsm_RO = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Tsm_Up = new System.Windows.Forms.ToolStripMenuItem();
            this.Tsm_Down = new System.Windows.Forms.ToolStripMenuItem();
            this.Tsmi_CupChange = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).BeginInit();
            this.bindingNavigator1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ctDataGridView1);
            this.groupBox1.Controls.Add(this.bindingNavigator1);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(313, 931);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配方浏览";
            // 
            // ctDataGridView1
            // 
            this.ctDataGridView1.AllowUserToAddRows = false;
            this.ctDataGridView1.AllowUserToDeleteRows = false;
            this.ctDataGridView1.AllowUserToResizeColumns = false;
            this.ctDataGridView1.AllowUserToResizeRows = false;
            this.ctDataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ctDataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.ctDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ctDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ctDataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.ctDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctDataGridView1.Location = new System.Drawing.Point(3, 277);
            this.ctDataGridView1.Name = "ctDataGridView1";
            this.ctDataGridView1.ReadOnly = true;
            this.ctDataGridView1.RowHeadersVisible = false;
            this.ctDataGridView1.Size = new System.Drawing.Size(307, 651);
            this.ctDataGridView1.TabIndex = 3;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "配方代码";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "版本号";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "MyID";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column3.Visible = false;
            // 
            // bindingNavigator1
            // 
            this.bindingNavigator1.AddNewItem = null;
            this.bindingNavigator1.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigator1.DeleteItem = null;
            this.bindingNavigator1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.toolStripLabel2});
            this.bindingNavigator1.Location = new System.Drawing.Point(3, 252);
            this.bindingNavigator1.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigator1.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigator1.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigator1.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigator1.Name = "bindingNavigator1";
            this.bindingNavigator1.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigator1.Size = new System.Drawing.Size(307, 25);
            this.bindingNavigator1.TabIndex = 2;
            this.bindingNavigator1.Text = "bindingNavigator1";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(32, 22);
            this.bindingNavigatorCountItem.Text = "/ {0}";
            this.bindingNavigatorCountItem.ToolTipText = "总项数";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem.Text = "移到第一条记录";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem.Text = "移到上一条记录";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "位置";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 23);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "当前位置";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem.Text = "移到下一条记录";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem.Text = "移到最后一条记录";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(0, 22);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rdo_Wait);
            this.panel1.Controls.Add(this.rdo_Browse_condition);
            this.panel1.Controls.Add(this.dt_Browse_End);
            this.panel1.Controls.Add(this.dt_Browse_Start);
            this.panel1.Controls.Add(this.txt_Browse_Code);
            this.panel1.Controls.Add(this.btn_Browse_Delete);
            this.panel1.Controls.Add(this.btn_Browse_Select);
            this.panel1.Controls.Add(this.rdo_Browse_NoDrop);
            this.panel1.Controls.Add(this.rdo_Browse_All);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(307, 227);
            this.panel1.TabIndex = 0;
            // 
            // rdo_Wait
            // 
            this.rdo_Wait.AutoSize = true;
            this.rdo_Wait.Font = new System.Drawing.Font("宋体", 14.25F);
            this.rdo_Wait.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdo_Wait.Location = new System.Drawing.Point(166, 41);
            this.rdo_Wait.Name = "rdo_Wait";
            this.rdo_Wait.Size = new System.Drawing.Size(103, 23);
            this.rdo_Wait.TabIndex = 46;
            this.rdo_Wait.Text = "等待列表";
            this.rdo_Wait.UseVisualStyleBackColor = true;
            // 
            // rdo_Browse_condition
            // 
            this.rdo_Browse_condition.AutoSize = true;
            this.rdo_Browse_condition.Font = new System.Drawing.Font("宋体", 14.25F);
            this.rdo_Browse_condition.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdo_Browse_condition.Location = new System.Drawing.Point(32, 41);
            this.rdo_Browse_condition.Name = "rdo_Browse_condition";
            this.rdo_Browse_condition.Size = new System.Drawing.Size(103, 23);
            this.rdo_Browse_condition.TabIndex = 45;
            this.rdo_Browse_condition.Text = "条件查询";
            this.rdo_Browse_condition.UseVisualStyleBackColor = true;
            // 
            // dt_Browse_End
            // 
            this.dt_Browse_End.CalendarFont = new System.Drawing.Font("宋体", 14.25F);
            this.dt_Browse_End.CustomFormat = "yyyy-MM-dd ";
            this.dt_Browse_End.Enabled = false;
            this.dt_Browse_End.Font = new System.Drawing.Font("宋体", 14.25F);
            this.dt_Browse_End.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_Browse_End.Location = new System.Drawing.Point(99, 143);
            this.dt_Browse_End.Name = "dt_Browse_End";
            this.dt_Browse_End.ShowUpDown = true;
            this.dt_Browse_End.Size = new System.Drawing.Size(205, 29);
            this.dt_Browse_End.TabIndex = 44;
            // 
            // dt_Browse_Start
            // 
            this.dt_Browse_Start.CalendarFont = new System.Drawing.Font("宋体", 14.25F);
            this.dt_Browse_Start.CustomFormat = "yyyy-MM-dd ";
            this.dt_Browse_Start.Enabled = false;
            this.dt_Browse_Start.Font = new System.Drawing.Font("宋体", 14.25F);
            this.dt_Browse_Start.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_Browse_Start.Location = new System.Drawing.Point(99, 107);
            this.dt_Browse_Start.Name = "dt_Browse_Start";
            this.dt_Browse_Start.ShowUpDown = true;
            this.dt_Browse_Start.Size = new System.Drawing.Size(205, 29);
            this.dt_Browse_Start.TabIndex = 43;
            this.dt_Browse_Start.Value = new System.DateTime(2021, 1, 1, 0, 0, 0, 0);
            // 
            // txt_Browse_Code
            // 
            this.txt_Browse_Code.Enabled = false;
            this.txt_Browse_Code.Font = new System.Drawing.Font("宋体", 14.25F);
            this.txt_Browse_Code.Location = new System.Drawing.Point(99, 71);
            this.txt_Browse_Code.Name = "txt_Browse_Code";
            this.txt_Browse_Code.Size = new System.Drawing.Size(205, 29);
            this.txt_Browse_Code.TabIndex = 42;
            this.txt_Browse_Code.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btn_Browse_Delete
            // 
            this.btn_Browse_Delete.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Browse_Delete.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Browse_Delete.Location = new System.Drawing.Point(179, 179);
            this.btn_Browse_Delete.Name = "btn_Browse_Delete";
            this.btn_Browse_Delete.Size = new System.Drawing.Size(125, 29);
            this.btn_Browse_Delete.TabIndex = 41;
            this.btn_Browse_Delete.Text = "删除";
            this.btn_Browse_Delete.UseVisualStyleBackColor = true;
            // 
            // btn_Browse_Select
            // 
            this.btn_Browse_Select.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Browse_Select.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Browse_Select.Location = new System.Drawing.Point(7, 179);
            this.btn_Browse_Select.Name = "btn_Browse_Select";
            this.btn_Browse_Select.Size = new System.Drawing.Size(125, 29);
            this.btn_Browse_Select.TabIndex = 40;
            this.btn_Browse_Select.Text = "查询";
            this.btn_Browse_Select.UseVisualStyleBackColor = true;
            // 
            // rdo_Browse_NoDrop
            // 
            this.rdo_Browse_NoDrop.AutoSize = true;
            this.rdo_Browse_NoDrop.Checked = true;
            this.rdo_Browse_NoDrop.Font = new System.Drawing.Font("宋体", 14.25F);
            this.rdo_Browse_NoDrop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdo_Browse_NoDrop.Location = new System.Drawing.Point(166, 11);
            this.rdo_Browse_NoDrop.Name = "rdo_Browse_NoDrop";
            this.rdo_Browse_NoDrop.Size = new System.Drawing.Size(103, 23);
            this.rdo_Browse_NoDrop.TabIndex = 39;
            this.rdo_Browse_NoDrop.TabStop = true;
            this.rdo_Browse_NoDrop.Text = "尚未滴液";
            this.rdo_Browse_NoDrop.UseVisualStyleBackColor = true;
            // 
            // rdo_Browse_All
            // 
            this.rdo_Browse_All.AutoSize = true;
            this.rdo_Browse_All.Font = new System.Drawing.Font("宋体", 14.25F);
            this.rdo_Browse_All.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdo_Browse_All.Location = new System.Drawing.Point(32, 11);
            this.rdo_Browse_All.Name = "rdo_Browse_All";
            this.rdo_Browse_All.Size = new System.Drawing.Size(103, 23);
            this.rdo_Browse_All.TabIndex = 38;
            this.rdo_Browse_All.Text = "显示全部";
            this.rdo_Browse_All.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label14.Location = new System.Drawing.Point(3, 145);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(95, 25);
            this.label14.TabIndex = 37;
            this.label14.Text = "结束日期:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(3, 109);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(95, 25);
            this.label13.TabIndex = 36;
            this.label13.Text = "起始日期:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(3, 73);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(95, 25);
            this.label12.TabIndex = 35;
            this.label12.Text = "配方代码:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Tsm_PS,
            this.Tsm_RO});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 48);
            // 
            // Tsm_PS
            // 
            this.Tsm_PS.Name = "Tsm_PS";
            this.Tsm_PS.Size = new System.Drawing.Size(100, 22);
            this.Tsm_PS.Text = "正序";
            // 
            // Tsm_RO
            // 
            this.Tsm_RO.Name = "Tsm_RO";
            this.Tsm_RO.Size = new System.Drawing.Size(100, 22);
            this.Tsm_RO.Text = "倒序";
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Tsm_Up,
            this.Tsm_Down,
            this.Tsmi_CupChange});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(125, 70);
            // 
            // Tsm_Up
            // 
            this.Tsm_Up.Name = "Tsm_Up";
            this.Tsm_Up.Size = new System.Drawing.Size(124, 22);
            this.Tsm_Up.Text = "上移";
            this.Tsm_Up.Click += new System.EventHandler(this.Tsm_Up_Click);
            // 
            // Tsm_Down
            // 
            this.Tsm_Down.Name = "Tsm_Down";
            this.Tsm_Down.Size = new System.Drawing.Size(124, 22);
            this.Tsm_Down.Text = "下移";
            this.Tsm_Down.Click += new System.EventHandler(this.Tsm_Down_Click);
            // 
            // Tsmi_CupChange
            // 
            this.Tsmi_CupChange.Name = "Tsmi_CupChange";
            this.Tsmi_CupChange.Size = new System.Drawing.Size(124, 22);
            this.Tsmi_CupChange.Text = "修改杯号";
            this.Tsmi_CupChange.Click += new System.EventHandler(this.Tsmi_CupChange_Click);
            // 
            // CtFormulaBrowse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "CtFormulaBrowse";
            this.Size = new System.Drawing.Size(313, 931);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctDataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).EndInit();
            this.bindingNavigator1.ResumeLayout(false);
            this.bindingNavigator1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rdo_Browse_condition;
        private System.Windows.Forms.DateTimePicker dt_Browse_End;
        private System.Windows.Forms.DateTimePicker dt_Browse_Start;
        private System.Windows.Forms.TextBox txt_Browse_Code;
        private System.Windows.Forms.Button btn_Browse_Delete;
        private System.Windows.Forms.Button btn_Browse_Select;
        private System.Windows.Forms.RadioButton rdo_Browse_NoDrop;
        private System.Windows.Forms.RadioButton rdo_Browse_All;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Tsm_PS;
        private System.Windows.Forms.ToolStripMenuItem Tsm_RO;
        private CtDataGridView ctDataGridView1;
        private System.Windows.Forms.BindingNavigator bindingNavigator1;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.RadioButton rdo_Wait;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem Tsm_Up;
        private System.Windows.Forms.ToolStripMenuItem Tsm_Down;
        private System.Windows.Forms.ToolStripMenuItem Tsmi_CupChange;
    }
}
