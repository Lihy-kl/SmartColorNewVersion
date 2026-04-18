namespace SmartColor.My_Form.BasicData
{
    partial class Operator
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv_DyeCode = new SmartColor.My_Control.CtDataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.grp_Dyeing = new System.Windows.Forms.GroupBox();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_Add = new System.Windows.Forms.Button();
            this.btn_Change = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_DyeCode)).BeginInit();
            this.grp_Dyeing.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv_DyeCode
            // 
            this.dgv_DyeCode.AllowUserToAddRows = false;
            this.dgv_DyeCode.AllowUserToDeleteRows = false;
            this.dgv_DyeCode.AllowUserToResizeColumns = false;
            this.dgv_DyeCode.AllowUserToResizeRows = false;
            this.dgv_DyeCode.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_DyeCode.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_DyeCode.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_DyeCode.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_DyeCode.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_DyeCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_DyeCode.Location = new System.Drawing.Point(5, 21);
            this.dgv_DyeCode.Margin = new System.Windows.Forms.Padding(5);
            this.dgv_DyeCode.MultiSelect = false;
            this.dgv_DyeCode.Name = "dgv_DyeCode";
            this.dgv_DyeCode.ReadOnly = true;
            this.dgv_DyeCode.RowHeadersVisible = false;
            this.dgv_DyeCode.RowHeadersWidth = 62;
            this.dgv_DyeCode.RowTemplate.Height = 23;
            this.dgv_DyeCode.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgv_DyeCode.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgv_DyeCode.Size = new System.Drawing.Size(1879, 815);
            this.dgv_DyeCode.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.HeaderText = "序号";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.HeaderText = "账户";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.HeaderText = "密码";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn4.HeaderText = "姓名";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btn_Save
            // 
            this.btn_Save.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Save.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Save.Location = new System.Drawing.Point(1088, 851);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(5);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(138, 66);
            this.btn_Save.TabIndex = 16;
            this.btn_Save.Text = "保存";
            this.btn_Save.UseVisualStyleBackColor = true;
            // 
            // btn_Delete
            // 
            this.btn_Delete.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Delete.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Delete.Location = new System.Drawing.Point(1482, 851);
            this.btn_Delete.Margin = new System.Windows.Forms.Padding(5);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(138, 66);
            this.btn_Delete.TabIndex = 15;
            this.btn_Delete.Text = "删除";
            this.btn_Delete.UseVisualStyleBackColor = true;
            // 
            // grp_Dyeing
            // 
            this.grp_Dyeing.Controls.Add(this.dgv_DyeCode);
            this.grp_Dyeing.Dock = System.Windows.Forms.DockStyle.Top;
            this.grp_Dyeing.Font = new System.Drawing.Font("宋体", 10.5F);
            this.grp_Dyeing.Location = new System.Drawing.Point(0, 0);
            this.grp_Dyeing.Margin = new System.Windows.Forms.Padding(5);
            this.grp_Dyeing.Name = "grp_Dyeing";
            this.grp_Dyeing.Padding = new System.Windows.Forms.Padding(5);
            this.grp_Dyeing.Size = new System.Drawing.Size(1889, 841);
            this.grp_Dyeing.TabIndex = 12;
            this.grp_Dyeing.TabStop = false;
            this.grp_Dyeing.Text = "浏览";
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
            this.Column2.HeaderText = "账户";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.HeaderText = "密码";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.HeaderText = "姓名";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btn_Add
            // 
            this.btn_Add.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Add.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Add.Location = new System.Drawing.Point(300, 851);
            this.btn_Add.Margin = new System.Windows.Forms.Padding(5);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(138, 66);
            this.btn_Add.TabIndex = 18;
            this.btn_Add.Text = "新增";
            this.btn_Add.UseVisualStyleBackColor = true;
            // 
            // btn_Change
            // 
            this.btn_Change.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Change.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Change.Location = new System.Drawing.Point(694, 851);
            this.btn_Change.Margin = new System.Windows.Forms.Padding(5);
            this.btn_Change.Name = "btn_Change";
            this.btn_Change.Size = new System.Drawing.Size(138, 66);
            this.btn_Change.TabIndex = 19;
            this.btn_Change.Text = "修改";
            this.btn_Change.UseVisualStyleBackColor = true;
            // 
            // Operator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1889, 931);
            this.Controls.Add(this.btn_Change);
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.grp_Dyeing);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "Operator";
            this.Text = "操作人员基本资料";
            ((System.ComponentModel.ISupportInitialize)(this.dgv_DyeCode)).EndInit();
            this.grp_Dyeing.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private My_Control.CtDataGridView dgv_DyeCode;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.GroupBox grp_Dyeing;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.Button btn_Add;
        private System.Windows.Forms.Button btn_Change;
    }
}