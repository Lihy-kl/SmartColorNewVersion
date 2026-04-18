namespace SmartColor.My_Control
{
    partial class CtDyeing
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

       

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Txt_DDRadio = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Cbo_DDPCode = new System.Windows.Forms.ComboBox();
            this.Cbo_Type = new System.Windows.Forms.ComboBox();
            this.dgv = new SmartColor.My_Control.CtFoldDataGridView(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Txt_DDRadio);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.Cbo_DDPCode);
            this.panel1.Controls.Add(this.Cbo_Type);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1258, 34);
            this.panel1.TabIndex = 0;
            // 
            // Txt_DDRadio
            // 
            this.Txt_DDRadio.Location = new System.Drawing.Point(597, 2);
            this.Txt_DDRadio.Name = "Txt_DDRadio";
            this.Txt_DDRadio.Size = new System.Drawing.Size(100, 29);
            this.Txt_DDRadio.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(540, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "浴比:";
            // 
            // Cbo_DDPCode
            // 
            this.Cbo_DDPCode.FormattingEnabled = true;
            this.Cbo_DDPCode.Location = new System.Drawing.Point(160, 3);
            this.Cbo_DDPCode.Name = "Cbo_DDPCode";
            this.Cbo_DDPCode.Size = new System.Drawing.Size(374, 27);
            this.Cbo_DDPCode.TabIndex = 1;
            this.Cbo_DDPCode.TextChanged += new System.EventHandler(this.Cbo_DDPCode_SelectionChangeCommitted);
            // 
            // Cbo_Type
            // 
            this.Cbo_Type.FormattingEnabled = true;
            this.Cbo_Type.Items.AddRange(new object[] {
            "染色工艺",
            "后处理工艺"});
            this.Cbo_Type.Location = new System.Drawing.Point(3, 3);
            this.Cbo_Type.Name = "Cbo_Type";
            this.Cbo_Type.Size = new System.Drawing.Size(151, 27);
            this.Cbo_Type.TabIndex = 0;
            this.Cbo_Type.TextChanged += new System.EventHandler(this.Cbo_Type_SelectionChangeCommitted);
            // 
            // dgv
            // 
            this.dgv.AllowUserToResizeColumns = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.IsFolded = false;
            this.dgv.Location = new System.Drawing.Point(0, 34);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowTemplate.Height = 23;
            this.dgv.Size = new System.Drawing.Size(1258, 166);
            this.dgv.TabIndex = 1;
            // 
            // CtDyeing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "CtDyeing";
            this.Size = new System.Drawing.Size(1258, 200);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        public CtFoldDataGridView dgv;
        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.ComboBox Cbo_Type;
        public System.Windows.Forms.ComboBox Cbo_DDPCode;
        public System.Windows.Forms.TextBox Txt_DDRadio;
    }
}
