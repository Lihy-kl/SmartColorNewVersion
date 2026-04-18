namespace SmartColor.My_Form.BasicData
{
    partial class BrewingProcess
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
            this.dgv_BrewProcess = new SmartColor.My_Control.CtDataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grp_Browse = new System.Windows.Forms.GroupBox();
            this.dgv_BrewCode = new SmartColor.My_Control.CtDataGridView();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_Add = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Change = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Tsmi_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.grp_BrewingProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BrewProcess)).BeginInit();
            this.grp_Browse.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BrewCode)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grp_BrewingProcess
            // 
            this.grp_BrewingProcess.Controls.Add(this.dgv_BrewProcess);
            this.grp_BrewingProcess.Font = new System.Drawing.Font("宋体", 10.5F);
            this.grp_BrewingProcess.Location = new System.Drawing.Point(394, 2);
            this.grp_BrewingProcess.Name = "grp_BrewingProcess";
            this.grp_BrewingProcess.Size = new System.Drawing.Size(1496, 869);
            this.grp_BrewingProcess.TabIndex = 4;
            this.grp_BrewingProcess.TabStop = false;
            this.grp_BrewingProcess.Text = "调液流程设定";
            // 
            // dgv_BrewProcess
            // 
            this.dgv_BrewProcess.AllowUserToAddRows = false;
            this.dgv_BrewProcess.AllowUserToDeleteRows = false;
            this.dgv_BrewProcess.AllowUserToResizeColumns = false;
            this.dgv_BrewProcess.AllowUserToResizeRows = false;
            this.dgv_BrewProcess.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_BrewProcess.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_BrewProcess.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_BrewProcess.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_BrewProcess.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_BrewProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_BrewProcess.Location = new System.Drawing.Point(3, 19);
            this.dgv_BrewProcess.MultiSelect = false;
            this.dgv_BrewProcess.Name = "dgv_BrewProcess";
            this.dgv_BrewProcess.ReadOnly = true;
            this.dgv_BrewProcess.RowHeadersVisible = false;
            this.dgv_BrewProcess.RowHeadersWidth = 62;
            this.dgv_BrewProcess.RowTemplate.Height = 23;
            this.dgv_BrewProcess.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgv_BrewProcess.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgv_BrewProcess.Size = new System.Drawing.Size(1490, 847);
            this.dgv_BrewProcess.TabIndex = 4;
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
            this.Column2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Column2.HeaderText = "操作流程";
            this.Column2.Items.AddRange(new object[] {
            "手动加染助剂",
            "加大冷水",
            "加小冷水",
            "加热水",
            "搅拌",
            "加补充剂",
            "加温水"});
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.HeaderText = "比例(%)/时间(s)";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.HeaderText = "热水比例(%)";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // grp_Browse
            // 
            this.grp_Browse.Controls.Add(this.dgv_BrewCode);
            this.grp_Browse.Font = new System.Drawing.Font("宋体", 10.5F);
            this.grp_Browse.Location = new System.Drawing.Point(0, 2);
            this.grp_Browse.Name = "grp_Browse";
            this.grp_Browse.Size = new System.Drawing.Size(387, 869);
            this.grp_Browse.TabIndex = 3;
            this.grp_Browse.TabStop = false;
            this.grp_Browse.Text = "浏览";
            // 
            // dgv_BrewCode
            // 
            this.dgv_BrewCode.AllowUserToAddRows = false;
            this.dgv_BrewCode.AllowUserToDeleteRows = false;
            this.dgv_BrewCode.AllowUserToResizeColumns = false;
            this.dgv_BrewCode.AllowUserToResizeRows = false;
            this.dgv_BrewCode.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_BrewCode.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv_BrewCode.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_BrewCode.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column5});
            this.dgv_BrewCode.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_BrewCode.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgv_BrewCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_BrewCode.Location = new System.Drawing.Point(3, 19);
            this.dgv_BrewCode.MultiSelect = false;
            this.dgv_BrewCode.Name = "dgv_BrewCode";
            this.dgv_BrewCode.ReadOnly = true;
            this.dgv_BrewCode.RowHeadersVisible = false;
            this.dgv_BrewCode.RowHeadersWidth = 62;
            this.dgv_BrewCode.RowTemplate.Height = 23;
            this.dgv_BrewCode.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_BrewCode.Size = new System.Drawing.Size(381, 847);
            this.dgv_BrewCode.TabIndex = 0;
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Column5.DefaultCellStyle = dataGridViewCellStyle4;
            this.Column5.HeaderText = "调液流程代码";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btn_Delete
            // 
            this.btn_Delete.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Delete.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Delete.Location = new System.Drawing.Point(1443, 877);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(165, 42);
            this.btn_Delete.TabIndex = 6;
            this.btn_Delete.Text = "删除";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.Btn_Delete_Click);
            // 
            // btn_Add
            // 
            this.btn_Add.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Add.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Add.Location = new System.Drawing.Point(249, 877);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(165, 42);
            this.btn_Add.TabIndex = 5;
            this.btn_Add.Text = "新增";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Save.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Save.Location = new System.Drawing.Point(1045, 877);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(165, 42);
            this.btn_Save.TabIndex = 7;
            this.btn_Save.Text = "保存";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // btn_Change
            // 
            this.btn_Change.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Change.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Change.Location = new System.Drawing.Point(647, 877);
            this.btn_Change.Name = "btn_Change";
            this.btn_Change.Size = new System.Drawing.Size(165, 42);
            this.btn_Change.TabIndex = 8;
            this.btn_Change.Text = "修改";
            this.btn_Change.UseVisualStyleBackColor = true;
            this.btn_Change.Click += new System.EventHandler(this.Btn_Change_Click);
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
            // BrewingProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1889, 931);
            this.Controls.Add(this.btn_Change);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.grp_BrewingProcess);
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.grp_Browse);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "BrewingProcess";
            this.Text = "染助剂泡制流程";
            this.grp_BrewingProcess.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BrewProcess)).EndInit();
            this.grp_Browse.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BrewCode)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grp_BrewingProcess;
        private My_Control.CtDataGridView dgv_BrewProcess;
        private System.Windows.Forms.GroupBox grp_Browse;
        private SmartColor.My_Control.CtDataGridView dgv_BrewCode;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.Button btn_Change;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Tsmi_Copy;
    }
}