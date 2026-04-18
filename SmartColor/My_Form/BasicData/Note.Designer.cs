namespace SmartColor.My_Form.BasicData
{
    partial class Note
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grp_Note1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txt_Note1Name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgv_Note1Items = new SmartColor.My_Control.CtDataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grp_Note2 = new System.Windows.Forms.GroupBox();
            this.dgv_Note2Items = new SmartColor.My_Control.CtDataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.grp_Note3 = new System.Windows.Forms.GroupBox();
            this.dgv_Note3Items = new SmartColor.My_Control.CtDataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txt_Note2Name = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_Note3Name = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.grp_Note1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Note1Items)).BeginInit();
            this.grp_Note2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Note2Items)).BeginInit();
            this.panel2.SuspendLayout();
            this.grp_Note3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Note3Items)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // grp_Note1
            // 
            this.grp_Note1.Controls.Add(this.dgv_Note1Items);
            this.grp_Note1.Controls.Add(this.panel1);
            this.grp_Note1.Dock = System.Windows.Forms.DockStyle.Left;
            this.grp_Note1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grp_Note1.Location = new System.Drawing.Point(0, 0);
            this.grp_Note1.Margin = new System.Windows.Forms.Padding(5);
            this.grp_Note1.Name = "grp_Note1";
            this.grp_Note1.Padding = new System.Windows.Forms.Padding(5);
            this.grp_Note1.Size = new System.Drawing.Size(630, 931);
            this.grp_Note1.TabIndex = 18;
            this.grp_Note1.TabStop = false;
            this.grp_Note1.Text = "备注1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txt_Note1Name);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(5, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(620, 51);
            this.panel1.TabIndex = 0;
            // 
            // txt_Note1Name
            // 
            this.txt_Note1Name.Location = new System.Drawing.Point(154, 12);
            this.txt_Note1Name.Name = "txt_Note1Name";
            this.txt_Note1Name.Size = new System.Drawing.Size(389, 29);
            this.txt_Note1Name.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "名称：";
            // 
            // dgv_Note1Items
            // 
            this.dgv_Note1Items.AllowUserToResizeColumns = false;
            this.dgv_Note1Items.AllowUserToResizeRows = false;
            this.dgv_Note1Items.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Note1Items.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_Note1Items.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Note1Items.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Note1Items.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_Note1Items.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Note1Items.Location = new System.Drawing.Point(5, 78);
            this.dgv_Note1Items.Name = "dgv_Note1Items";
            this.dgv_Note1Items.RowHeadersVisible = false;
            this.dgv_Note1Items.RowTemplate.Height = 23;
            this.dgv_Note1Items.Size = new System.Drawing.Size(620, 848);
            this.dgv_Note1Items.TabIndex = 1;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "可选项";
            this.Column1.Name = "Column1";
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // grp_Note2
            // 
            this.grp_Note2.Controls.Add(this.dgv_Note2Items);
            this.grp_Note2.Controls.Add(this.panel2);
            this.grp_Note2.Dock = System.Windows.Forms.DockStyle.Left;
            this.grp_Note2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grp_Note2.Location = new System.Drawing.Point(630, 0);
            this.grp_Note2.Margin = new System.Windows.Forms.Padding(5);
            this.grp_Note2.Name = "grp_Note2";
            this.grp_Note2.Padding = new System.Windows.Forms.Padding(5);
            this.grp_Note2.Size = new System.Drawing.Size(630, 931);
            this.grp_Note2.TabIndex = 19;
            this.grp_Note2.TabStop = false;
            this.grp_Note2.Text = "备注2";
            // 
            // dgv_Note2Items
            // 
            this.dgv_Note2Items.AllowUserToResizeColumns = false;
            this.dgv_Note2Items.AllowUserToResizeRows = false;
            this.dgv_Note2Items.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Note2Items.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv_Note2Items.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Note2Items.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Note2Items.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgv_Note2Items.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Note2Items.Location = new System.Drawing.Point(5, 78);
            this.dgv_Note2Items.Name = "dgv_Note2Items";
            this.dgv_Note2Items.RowHeadersVisible = false;
            this.dgv_Note2Items.RowTemplate.Height = 23;
            this.dgv_Note2Items.Size = new System.Drawing.Size(620, 848);
            this.dgv_Note2Items.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.HeaderText = "可选项";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txt_Note2Name);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(5, 27);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(620, 51);
            this.panel2.TabIndex = 0;
            // 
            // grp_Note3
            // 
            this.grp_Note3.Controls.Add(this.dgv_Note3Items);
            this.grp_Note3.Controls.Add(this.panel3);
            this.grp_Note3.Dock = System.Windows.Forms.DockStyle.Left;
            this.grp_Note3.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.grp_Note3.Location = new System.Drawing.Point(1260, 0);
            this.grp_Note3.Margin = new System.Windows.Forms.Padding(5);
            this.grp_Note3.Name = "grp_Note3";
            this.grp_Note3.Padding = new System.Windows.Forms.Padding(5);
            this.grp_Note3.Size = new System.Drawing.Size(630, 931);
            this.grp_Note3.TabIndex = 20;
            this.grp_Note3.TabStop = false;
            this.grp_Note3.Text = "备注3";
            // 
            // dgv_Note3Items
            // 
            this.dgv_Note3Items.AllowUserToResizeColumns = false;
            this.dgv_Note3Items.AllowUserToResizeRows = false;
            this.dgv_Note3Items.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Note3Items.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgv_Note3Items.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Note3Items.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Note3Items.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgv_Note3Items.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Note3Items.Location = new System.Drawing.Point(5, 78);
            this.dgv_Note3Items.Name = "dgv_Note3Items";
            this.dgv_Note3Items.RowHeadersVisible = false;
            this.dgv_Note3Items.RowTemplate.Height = 23;
            this.dgv_Note3Items.Size = new System.Drawing.Size(620, 848);
            this.dgv_Note3Items.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.HeaderText = "可选项";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.txt_Note3Name);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(5, 27);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(620, 51);
            this.panel3.TabIndex = 0;
            // 
            // txt_Note2Name
            // 
            this.txt_Note2Name.Location = new System.Drawing.Point(151, 11);
            this.txt_Note2Name.Name = "txt_Note2Name";
            this.txt_Note2Name.Size = new System.Drawing.Size(389, 29);
            this.txt_Note2Name.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(81, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "名称：";
            // 
            // txt_Note3Name
            // 
            this.txt_Note3Name.Location = new System.Drawing.Point(151, 11);
            this.txt_Note3Name.Name = "txt_Note3Name";
            this.txt_Note3Name.Size = new System.Drawing.Size(389, 29);
            this.txt_Note3Name.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(81, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 19);
            this.label3.TabIndex = 4;
            this.label3.Text = "名称：";
            // 
            // Note
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1889, 931);
            this.Controls.Add(this.grp_Note3);
            this.Controls.Add(this.grp_Note2);
            this.Controls.Add(this.grp_Note1);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "Note";
            this.Text = "配方备注基础资料";
            this.grp_Note1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Note1Items)).EndInit();
            this.grp_Note2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Note2Items)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.grp_Note3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Note3Items)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox grp_Note1;
        private My_Control.CtDataGridView dgv_Note1Items;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txt_Note1Name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grp_Note2;
        private My_Control.CtDataGridView dgv_Note2Items;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox grp_Note3;
        private My_Control.CtDataGridView dgv_Note3Items;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txt_Note2Name;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_Note3Name;
        private System.Windows.Forms.Label label3;
    }
}