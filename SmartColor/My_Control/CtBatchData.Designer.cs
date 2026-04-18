namespace SmartColor.My_Control
{
    partial class CtBatchData
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
                // 解绑事件
                ctDataGridView1.CurrentCellChanged -= CtDataGridView1_CurrentCellChanged;
                SmartColor.My_AutomaticModule.DropRobotTask.CupFinished -= OnBatchRelatedEvent;
                SmartColor.My_Tool.CupAuxiliary.CupFinished -= OnBatchRelatedEvent;
                SmartColor.My_AutomaticModule.DropBatchManager.BatchChanged -= OnBatchRelatedEvent;
                _refreshTimer.Stop();
                _refreshTimer.Dispose();
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.btn_Start = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ctDataGridView1 = new SmartColor.My_Control.CtDataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Tsmi_Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.Tsmi_CupChange = new System.Windows.Forms.ToolStripMenuItem();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctDataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_Stop);
            this.panel1.Controls.Add(this.btn_Start);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 631);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(318, 55);
            this.panel1.TabIndex = 0;
            // 
            // btn_Stop
            // 
            this.btn_Stop.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Stop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Stop.Location = new System.Drawing.Point(165, 2);
            this.btn_Stop.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(145, 50);
            this.btn_Stop.TabIndex = 9;
            this.btn_Stop.Text = "停止滴液";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.Btn_Stop_Click);
            // 
            // btn_Start
            // 
            this.btn_Start.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Start.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Start.Location = new System.Drawing.Point(8, 2);
            this.btn_Start.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(145, 50);
            this.btn_Start.TabIndex = 8;
            this.btn_Start.Text = "开始滴液(F10)";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.Btn_Start_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ctDataGridView1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(318, 631);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "批次资料";
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
            this.Column3,
            this.Column4});
            this.ctDataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ctDataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.ctDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctDataGridView1.Location = new System.Drawing.Point(3, 25);
            this.ctDataGridView1.Name = "ctDataGridView1";
            this.ctDataGridView1.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ctDataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.ctDataGridView1.RowHeadersVisible = false;
            this.ctDataGridView1.RowTemplate.Height = 23;
            this.ctDataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ctDataGridView1.Size = new System.Drawing.Size(312, 603);
            this.ctDataGridView1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Tsmi_Delete,
            this.Tsmi_CupChange});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 48);
            // 
            // Tsmi_Delete
            // 
            this.Tsmi_Delete.Name = "Tsmi_Delete";
            this.Tsmi_Delete.Size = new System.Drawing.Size(124, 22);
            this.Tsmi_Delete.Text = "删除配方";
            this.Tsmi_Delete.Click += new System.EventHandler(this.Tsmi_Delete_Click);
            // 
            // Tsmi_CupChange
            // 
            this.Tsmi_CupChange.Name = "Tsmi_CupChange";
            this.Tsmi_CupChange.Size = new System.Drawing.Size(124, 22);
            this.Tsmi_CupChange.Text = "修改杯号";
            this.Tsmi_CupChange.Click += new System.EventHandler(this.Tsmi_CupChange_Click);
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "杯号";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "配方代码";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "序号";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column3.Visible = false;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "批次号";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column4.Visible = false;
            // 
            // CtBatchData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "CtBatchData";
            this.Size = new System.Drawing.Size(318, 686);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ctDataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_Stop;
        public System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.GroupBox groupBox1;
        private CtDataGridView ctDataGridView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Tsmi_Delete;
        private System.Windows.Forms.ToolStripMenuItem Tsmi_CupChange;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
    }
}
