namespace SmartColor.My_Form.BasicData
{
    partial class DPConfiguration
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grp_BrewingProcess = new System.Windows.Forms.GroupBox();
            this.dgv_DyeData = new SmartColor.My_Control.CtDataGridView();
            this.grp_Dyeing = new System.Windows.Forms.GroupBox();
            this.dgv_DyeCode = new SmartColor.My_Control.CtDataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Tsmi_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_Add = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Change = new System.Windows.Forms.Button();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grp_BrewingProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_DyeData)).BeginInit();
            this.grp_Dyeing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_DyeCode)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grp_BrewingProcess
            // 
            this.grp_BrewingProcess.Controls.Add(this.dgv_DyeData);
            this.grp_BrewingProcess.Font = new System.Drawing.Font("宋体", 10.5F);
            this.grp_BrewingProcess.Location = new System.Drawing.Point(709, 1);
            this.grp_BrewingProcess.Name = "grp_BrewingProcess";
            this.grp_BrewingProcess.Size = new System.Drawing.Size(1178, 880);
            this.grp_BrewingProcess.TabIndex = 6;
            this.grp_BrewingProcess.TabStop = false;
            this.grp_BrewingProcess.Text = "染色工艺设定";
            // 
            // dgv_DyeData
            // 
            this.dgv_DyeData.AllowUserToAddRows = false;
            this.dgv_DyeData.AllowUserToDeleteRows = false;
            this.dgv_DyeData.AllowUserToResizeColumns = false;
            this.dgv_DyeData.AllowUserToResizeRows = false;
            this.dgv_DyeData.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_DyeData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_DyeData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_DyeData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column5,
            this.Column6,
            this.Column7,
            this.Column8,
            this.Column9,
            this.Column10});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_DyeData.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_DyeData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_DyeData.Location = new System.Drawing.Point(3, 19);
            this.dgv_DyeData.MultiSelect = false;
            this.dgv_DyeData.Name = "dgv_DyeData";
            this.dgv_DyeData.ReadOnly = true;
            this.dgv_DyeData.RowHeadersVisible = false;
            this.dgv_DyeData.RowHeadersWidth = 62;
            this.dgv_DyeData.RowTemplate.Height = 23;
            this.dgv_DyeData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgv_DyeData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgv_DyeData.Size = new System.Drawing.Size(1172, 858);
            this.dgv_DyeData.TabIndex = 4;
            // 
            // grp_Dyeing
            // 
            this.grp_Dyeing.Controls.Add(this.dgv_DyeCode);
            this.grp_Dyeing.Font = new System.Drawing.Font("宋体", 10.5F);
            this.grp_Dyeing.Location = new System.Drawing.Point(2, 1);
            this.grp_Dyeing.Name = "grp_Dyeing";
            this.grp_Dyeing.Size = new System.Drawing.Size(701, 880);
            this.grp_Dyeing.TabIndex = 5;
            this.grp_Dyeing.TabStop = false;
            this.grp_Dyeing.Text = "浏览";
            // 
            // dgv_DyeCode
            // 
            this.dgv_DyeCode.AllowUserToAddRows = false;
            this.dgv_DyeCode.AllowUserToDeleteRows = false;
            this.dgv_DyeCode.AllowUserToResizeColumns = false;
            this.dgv_DyeCode.AllowUserToResizeRows = false;
            this.dgv_DyeCode.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_DyeCode.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv_DyeCode.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_DyeCode.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.dgv_DyeCode.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_DyeCode.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgv_DyeCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_DyeCode.Location = new System.Drawing.Point(3, 19);
            this.dgv_DyeCode.MultiSelect = false;
            this.dgv_DyeCode.Name = "dgv_DyeCode";
            this.dgv_DyeCode.ReadOnly = true;
            this.dgv_DyeCode.RowHeadersVisible = false;
            this.dgv_DyeCode.RowHeadersWidth = 62;
            this.dgv_DyeCode.RowTemplate.Height = 23;
            this.dgv_DyeCode.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgv_DyeCode.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_DyeCode.Size = new System.Drawing.Size(695, 858);
            this.dgv_DyeCode.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "序号";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "染色工艺代码";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.HeaderText = "是否关盖加药";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.HeaderText = "备注";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Tsmi_Copy});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 26);
            // 
            // Tsmi_Copy
            // 
            this.Tsmi_Copy.Name = "Tsmi_Copy";
            this.Tsmi_Copy.Size = new System.Drawing.Size(100, 22);
            this.Tsmi_Copy.Text = "拷贝";
            this.Tsmi_Copy.Click += new System.EventHandler(this.Tsmi_Copy_Click);
            // 
            // btn_Delete
            // 
            this.btn_Delete.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Delete.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Delete.Location = new System.Drawing.Point(1494, 887);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(83, 42);
            this.btn_Delete.TabIndex = 8;
            this.btn_Delete.Text = "删除";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.Btn_Delete_Click);
            // 
            // btn_Add
            // 
            this.btn_Add.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Add.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Add.Location = new System.Drawing.Point(324, 887);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(83, 42);
            this.btn_Add.TabIndex = 7;
            this.btn_Add.Text = "新增";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Save.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Save.Location = new System.Drawing.Point(1104, 887);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(83, 42);
            this.btn_Save.TabIndex = 9;
            this.btn_Save.Text = "保存";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // btn_Change
            // 
            this.btn_Change.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Change.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Change.Location = new System.Drawing.Point(714, 887);
            this.btn_Change.Name = "btn_Change";
            this.btn_Change.Size = new System.Drawing.Size(83, 42);
            this.btn_Change.TabIndex = 11;
            this.btn_Change.Text = "修改";
            this.btn_Change.UseVisualStyleBackColor = true;
            this.btn_Change.Click += new System.EventHandler(this.Btn_Change_Click);
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column5.HeaderText = "步号";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column6
            // 
            this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Column6.HeaderText = "操作类型";
            this.Column6.Items.AddRange(new object[] {
            "放布",
            "冷行",
            "温控",
            "加水",
            "搅拌",
            "排液",
            "出布",
            "取小样",
            "测PH",
            "加A",
            "加B",
            "加C",
            "加D",
            "加E",
            "加F",
            "加G",
            "加H",
            "加I",
            "加J",
            "加K",
            "加L",
            "加M",
            "加N"});
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Column7
            // 
            this.Column7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column7.HeaderText = "温度";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column8
            // 
            this.Column8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column8.HeaderText = "速率(℃/m)";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column9
            // 
            this.Column9.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column9.HeaderText = "比例(%)/时间(m)";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            this.Column9.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column10
            // 
            this.Column10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column10.HeaderText = "转速";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            this.Column10.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // DPConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1889, 931);
            this.Controls.Add(this.btn_Change);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.grp_BrewingProcess);
            this.Controls.Add(this.grp_Dyeing);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "DPConfiguration";
            this.Text = "染色工艺配置";
            this.grp_BrewingProcess.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_DyeData)).EndInit();
            this.grp_Dyeing.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_DyeCode)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grp_BrewingProcess;
        private My_Control.CtDataGridView dgv_DyeData; 
        private System.Windows.Forms.GroupBox grp_Dyeing;
        private My_Control.CtDataGridView dgv_DyeCode;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Tsmi_Copy;
        private System.Windows.Forms.Button btn_Change;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
    }
}