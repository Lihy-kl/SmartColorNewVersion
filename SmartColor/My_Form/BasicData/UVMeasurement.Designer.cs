namespace SmartColor.My_Form.BasicData
{
    partial class UVMeasurement
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grp_BrewingProcess = new System.Windows.Forms.GroupBox();
            this.dgv_Process = new SmartColor.My_Control.CtDataGridView();
            this.grp_Browse = new System.Windows.Forms.GroupBox();
            this.dgv_Code = new SmartColor.My_Control.CtDataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Tsmi_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_Change = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_Add = new System.Windows.Forms.Button();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grp_BrewingProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Process)).BeginInit();
            this.grp_Browse.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Code)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grp_BrewingProcess
            // 
            this.grp_BrewingProcess.Controls.Add(this.dgv_Process);
            this.grp_BrewingProcess.Font = new System.Drawing.Font("宋体", 10.5F);
            this.grp_BrewingProcess.Location = new System.Drawing.Point(393, 7);
            this.grp_BrewingProcess.Name = "grp_BrewingProcess";
            this.grp_BrewingProcess.Size = new System.Drawing.Size(1496, 869);
            this.grp_BrewingProcess.TabIndex = 10;
            this.grp_BrewingProcess.TabStop = false;
            this.grp_BrewingProcess.Text = "吸光度测量流程设定";
            // 
            // dgv_Process
            // 
            this.dgv_Process.AllowUserToAddRows = false;
            this.dgv_Process.AllowUserToDeleteRows = false;
            this.dgv_Process.AllowUserToResizeColumns = false;
            this.dgv_Process.AllowUserToResizeRows = false;
            this.dgv_Process.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Process.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_Process.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Process.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column6,
            this.Column7,
            this.Column8,
            this.Column9,
            this.Column10,
            this.Column11,
            this.Column12});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Process.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_Process.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Process.Location = new System.Drawing.Point(3, 19);
            this.dgv_Process.MultiSelect = false;
            this.dgv_Process.Name = "dgv_Process";
            this.dgv_Process.ReadOnly = true;
            this.dgv_Process.RowHeadersVisible = false;
            this.dgv_Process.RowHeadersWidth = 62;
            this.dgv_Process.RowTemplate.Height = 23;
            this.dgv_Process.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgv_Process.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgv_Process.Size = new System.Drawing.Size(1490, 847);
            this.dgv_Process.TabIndex = 4;
            // 
            // grp_Browse
            // 
            this.grp_Browse.Controls.Add(this.dgv_Code);
            this.grp_Browse.Font = new System.Drawing.Font("宋体", 10.5F);
            this.grp_Browse.Location = new System.Drawing.Point(-1, 7);
            this.grp_Browse.Name = "grp_Browse";
            this.grp_Browse.Size = new System.Drawing.Size(387, 869);
            this.grp_Browse.TabIndex = 9;
            this.grp_Browse.TabStop = false;
            this.grp_Browse.Text = "浏览";
            // 
            // dgv_Code
            // 
            this.dgv_Code.AllowUserToAddRows = false;
            this.dgv_Code.AllowUserToDeleteRows = false;
            this.dgv_Code.AllowUserToResizeColumns = false;
            this.dgv_Code.AllowUserToResizeRows = false;
            this.dgv_Code.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Code.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv_Code.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Code.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column5});
            this.dgv_Code.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Code.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgv_Code.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Code.Location = new System.Drawing.Point(3, 19);
            this.dgv_Code.MultiSelect = false;
            this.dgv_Code.Name = "dgv_Code";
            this.dgv_Code.ReadOnly = true;
            this.dgv_Code.RowHeadersVisible = false;
            this.dgv_Code.RowHeadersWidth = 62;
            this.dgv_Code.RowTemplate.Height = 23;
            this.dgv_Code.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_Code.Size = new System.Drawing.Size(381, 847);
            this.dgv_Code.TabIndex = 0;
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
            // 
            // btn_Change
            // 
            this.btn_Change.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Change.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Change.Location = new System.Drawing.Point(646, 882);
            this.btn_Change.Name = "btn_Change";
            this.btn_Change.Size = new System.Drawing.Size(165, 42);
            this.btn_Change.TabIndex = 14;
            this.btn_Change.Text = "修改";
            this.btn_Change.UseVisualStyleBackColor = true;
            // 
            // btn_Save
            // 
            this.btn_Save.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Save.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Save.Location = new System.Drawing.Point(1044, 882);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(165, 42);
            this.btn_Save.TabIndex = 13;
            this.btn_Save.Text = "保存";
            this.btn_Save.UseVisualStyleBackColor = true;
            // 
            // btn_Delete
            // 
            this.btn_Delete.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Delete.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Delete.Location = new System.Drawing.Point(1442, 882);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(165, 42);
            this.btn_Delete.TabIndex = 12;
            this.btn_Delete.Text = "删除";
            this.btn_Delete.UseVisualStyleBackColor = true;
            // 
            // btn_Add
            // 
            this.btn_Add.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Add.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Add.Location = new System.Drawing.Point(248, 882);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(165, 42);
            this.btn_Add.TabIndex = 11;
            this.btn_Add.Text = "新增";
            this.btn_Add.UseVisualStyleBackColor = true;
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column5.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column5.HeaderText = "吸光度测量流程代码";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "步号";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "操作流程";
            this.Column2.Items.AddRange(new object[] {
            "加药",
            "加水",
            "抽染液",
            "搅拌",
            "排液",
            "测吸光度",
            "加溶解剂"});
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.HeaderText = "搅拌速度";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.HeaderText = "搅拌时间(s)";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column6
            // 
            this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column6.HeaderText = "排液时间(s)";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column7
            // 
            this.Column7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column7.HeaderText = "排比色皿时间(s)";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column8
            // 
            this.Column8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column8.HeaderText = "抽液时间(s)";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column9
            // 
            this.Column9.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column9.HeaderText = "起始波长(nm)";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            this.Column9.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column10
            // 
            this.Column10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column10.HeaderText = "结束波长(nm)";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            this.Column10.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column11
            // 
            this.Column11.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column11.HeaderText = "波长间隔(nm)";
            this.Column11.Name = "Column11";
            this.Column11.ReadOnly = true;
            this.Column11.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column12
            // 
            this.Column12.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column12.HeaderText = "加药量(g)";
            this.Column12.Name = "Column12";
            this.Column12.ReadOnly = true;
            this.Column12.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // UVMeasurement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1889, 931);
            this.Controls.Add(this.grp_BrewingProcess);
            this.Controls.Add(this.grp_Browse);
            this.Controls.Add(this.btn_Change);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.btn_Add);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "UVMeasurement";
            this.Text = "吸光度测量";
            this.grp_BrewingProcess.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Process)).EndInit();
            this.grp_Browse.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Code)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grp_BrewingProcess;
        private My_Control.CtDataGridView dgv_Process;
        private System.Windows.Forms.GroupBox grp_Browse;
        private My_Control.CtDataGridView dgv_Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Tsmi_Copy;
        private System.Windows.Forms.Button btn_Change;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
    }
}