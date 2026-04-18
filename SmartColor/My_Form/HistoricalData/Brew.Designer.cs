namespace SmartColor.My_Form.HistoricalData
{
    partial class Brew
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.txt_R = new System.Windows.Forms.TextBox();
            this.btn_Record_Select = new System.Windows.Forms.Button();
            this.dt_Record_End = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dt_Record_Start = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ctRecord1 = new SmartColor.My_Control.CtRecord();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txt_R);
            this.panel1.Controls.Add(this.btn_Record_Select);
            this.panel1.Controls.Add(this.dt_Record_End);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dt_Record_Start);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1889, 49);
            this.panel1.TabIndex = 1;
            // 
            // txt_R
            // 
            this.txt_R.Font = new System.Drawing.Font("宋体", 14.25F);
            this.txt_R.Location = new System.Drawing.Point(1150, 11);
            this.txt_R.MaxLength = 16;
            this.txt_R.Name = "txt_R";
            this.txt_R.ReadOnly = true;
            this.txt_R.Size = new System.Drawing.Size(732, 29);
            this.txt_R.TabIndex = 67;
            this.txt_R.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btn_Record_Select
            // 
            this.btn_Record_Select.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Record_Select.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Record_Select.Location = new System.Drawing.Point(1025, 8);
            this.btn_Record_Select.Name = "btn_Record_Select";
            this.btn_Record_Select.Size = new System.Drawing.Size(96, 35);
            this.btn_Record_Select.TabIndex = 66;
            this.btn_Record_Select.Text = "查询";
            this.btn_Record_Select.UseVisualStyleBackColor = true;
            // 
            // dt_Record_End
            // 
            this.dt_Record_End.CalendarFont = new System.Drawing.Font("宋体", 14.25F);
            this.dt_Record_End.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dt_Record_End.Font = new System.Drawing.Font("宋体", 14.25F);
            this.dt_Record_End.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_Record_End.Location = new System.Drawing.Point(741, 11);
            this.dt_Record_End.Name = "dt_Record_End";
            this.dt_Record_End.ShowUpDown = true;
            this.dt_Record_End.Size = new System.Drawing.Size(221, 29);
            this.dt_Record_End.TabIndex = 65;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(644, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 19);
            this.label2.TabIndex = 64;
            this.label2.Text = "结束日期:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dt_Record_Start
            // 
            this.dt_Record_Start.CalendarFont = new System.Drawing.Font("宋体", 14.25F);
            this.dt_Record_Start.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dt_Record_Start.Font = new System.Drawing.Font("宋体", 14.25F);
            this.dt_Record_Start.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_Record_Start.Location = new System.Drawing.Point(365, 11);
            this.dt_Record_Start.Name = "dt_Record_Start";
            this.dt_Record_Start.ShowUpDown = true;
            this.dt_Record_Start.Size = new System.Drawing.Size(221, 29);
            this.dt_Record_Start.TabIndex = 63;
            this.dt_Record_Start.Value = new System.DateTime(2021, 1, 1, 0, 0, 0, 0);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(267, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 19);
            this.label3.TabIndex = 62;
            this.label3.Text = "起始日期:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(69, 11);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(141, 29);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "瓶号：";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ctRecord1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 49);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1889, 882);
            this.panel2.TabIndex = 2;
            // 
            // ctRecord1
            // 
            this.ctRecord1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctRecord1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ctRecord1.Location = new System.Drawing.Point(0, 0);
            this.ctRecord1.Margin = new System.Windows.Forms.Padding(5);
            this.ctRecord1.Name = "ctRecord1";
            this.ctRecord1.Size = new System.Drawing.Size(1889, 882);
            this.ctRecord1.TabIndex = 1;
            // 
            // Brew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1889, 931);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "Brew";
            this.Text = "开料历史记录";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private My_Control.CtRecord ctRecord1;
        private System.Windows.Forms.DateTimePicker dt_Record_Start;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dt_Record_End;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_Record_Select;
        private System.Windows.Forms.TextBox txt_R;
    }
}