namespace SmartColor.My_Form.BasicData
{
    partial class DyeType
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DyeType));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grp_Browse = new System.Windows.Forms.GroupBox();
            this.dgv_Type = new SmartColor.My_Control.CtDataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Tsmi_DefaultType = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ctDHDataGridView1 = new SmartColor.My_Control.CtDHDataGridView();
            this.btn_Change = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_Add = new System.Windows.Forms.Button();
            this.grp_Browse.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Type)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctDHDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // grp_Browse
            // 
            this.grp_Browse.Controls.Add(this.dgv_Type);
            this.grp_Browse.Font = new System.Drawing.Font("宋体", 10.5F);
            this.grp_Browse.Location = new System.Drawing.Point(2, 2);
            this.grp_Browse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grp_Browse.Name = "grp_Browse";
            this.grp_Browse.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grp_Browse.Size = new System.Drawing.Size(344, 877);
            this.grp_Browse.TabIndex = 3;
            this.grp_Browse.TabStop = false;
            this.grp_Browse.Text = "浏览";
            // 
            // dgv_Type
            // 
            this.dgv_Type.AllowUserToAddRows = false;
            this.dgv_Type.AllowUserToDeleteRows = false;
            this.dgv_Type.AllowUserToResizeColumns = false;
            this.dgv_Type.AllowUserToResizeRows = false;
            this.dgv_Type.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Type.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgv_Type.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Type.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dgv_Type.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Type.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgv_Type.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Type.Location = new System.Drawing.Point(3, 20);
            this.dgv_Type.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgv_Type.MultiSelect = false;
            this.dgv_Type.Name = "dgv_Type";
            this.dgv_Type.ReadOnly = true;
            this.dgv_Type.RowHeadersVisible = false;
            this.dgv_Type.RowHeadersWidth = 62;
            this.dgv_Type.RowTemplate.Height = 23;
            this.dgv_Type.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgv_Type.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_Type.Size = new System.Drawing.Size(338, 853);
            this.dgv_Type.TabIndex = 0;
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
            this.Column2.HeaderText = "染料类型";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Tsmi_DefaultType});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 26);
            // 
            // Tsmi_DefaultType
            // 
            this.Tsmi_DefaultType.Name = "Tsmi_DefaultType";
            this.Tsmi_DefaultType.Size = new System.Drawing.Size(124, 22);
            this.Tsmi_DefaultType.Text = "默认类型";
            this.Tsmi_DefaultType.Click += new System.EventHandler(this.Tsmi_DefaultType_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ctDHDataGridView1);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 10.5F);
            this.groupBox1.Location = new System.Drawing.Point(349, 2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(1538, 877);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "染助剂分量详情";
            // 
            // ctDHDataGridView1
            // 
            this.ctDHDataGridView1.AllowUserToAddRows = false;
            this.ctDHDataGridView1.AllowUserToDeleteRows = false;
            this.ctDHDataGridView1.AssistantCodes = ((System.Collections.Generic.List<string>)(resources.GetObject("ctDHDataGridView1.AssistantCodes")));
            this.ctDHDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ctDHDataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.ctDHDataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ctDHDataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.ctDHDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ctDHDataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.ctDHDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctDHDataGridView1.EnableHeadersVisualStyles = false;
            this.ctDHDataGridView1.IsLoading = false;
            this.ctDHDataGridView1.Location = new System.Drawing.Point(3, 20);
            this.ctDHDataGridView1.Name = "ctDHDataGridView1";
            this.ctDHDataGridView1.ReadOnly = true;
            this.ctDHDataGridView1.RowHeadersVisible = false;
            this.ctDHDataGridView1.RowTemplate.Height = 23;
            this.ctDHDataGridView1.Size = new System.Drawing.Size(1532, 853);
            this.ctDHDataGridView1.TabIndex = 0;
            // 
            // btn_Change
            // 
            this.btn_Change.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Change.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Change.Location = new System.Drawing.Point(673, 883);
            this.btn_Change.Name = "btn_Change";
            this.btn_Change.Size = new System.Drawing.Size(83, 42);
            this.btn_Change.TabIndex = 17;
            this.btn_Change.Text = "修改";
            this.btn_Change.UseVisualStyleBackColor = true;
            // 
            // btn_Save
            // 
            this.btn_Save.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Save.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Save.Location = new System.Drawing.Point(1083, 883);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(83, 42);
            this.btn_Save.TabIndex = 16;
            this.btn_Save.Text = "保存";
            this.btn_Save.UseVisualStyleBackColor = true;
            // 
            // btn_Delete
            // 
            this.btn_Delete.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Delete.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Delete.Location = new System.Drawing.Point(1493, 883);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(83, 42);
            this.btn_Delete.TabIndex = 19;
            this.btn_Delete.Text = "删除";
            this.btn_Delete.UseVisualStyleBackColor = true;
            // 
            // btn_Add
            // 
            this.btn_Add.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Add.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Add.Location = new System.Drawing.Point(263, 883);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(83, 42);
            this.btn_Add.TabIndex = 18;
            this.btn_Add.Text = "新增";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.Btn_Add_Click);
            // 
            // DyeType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1889, 931);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.btn_Change);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grp_Browse);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "DyeType";
            this.Text = "染料类型基本资料";
            this.grp_Browse.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Type)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ctDHDataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grp_Browse;
        private My_Control.CtDataGridView dgv_Type;
        private System.Windows.Forms.GroupBox groupBox1;
        private My_Control.CtDHDataGridView ctDHDataGridView1;
        private System.Windows.Forms.Button btn_Change;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Tsmi_DefaultType;
    }
}