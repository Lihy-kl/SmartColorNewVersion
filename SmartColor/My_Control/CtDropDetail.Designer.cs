namespace SmartColor.My_Control
{
    partial class CtDropDetail
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
                My_Form.Login.LoginForm.UserChanged -= LoginForm_UserChanged;
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_upd = new System.Windows.Forms.Button();
            this.btn_FormulaCodeAdd = new System.Windows.Forms.Button();
            this.btn_pre = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_BatchAdd = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_upd);
            this.panel1.Controls.Add(this.btn_FormulaCodeAdd);
            this.panel1.Controls.Add(this.btn_pre);
            this.panel1.Controls.Add(this.btn_Save);
            this.panel1.Controls.Add(this.btn_BatchAdd);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 459);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1258, 44);
            this.panel1.TabIndex = 2;
            // 
            // btn_upd
            // 
            this.btn_upd.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_upd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_upd.Location = new System.Drawing.Point(807, 7);
            this.btn_upd.Margin = new System.Windows.Forms.Padding(4);
            this.btn_upd.Name = "btn_upd";
            this.btn_upd.Size = new System.Drawing.Size(177, 33);
            this.btn_upd.TabIndex = 53;
            this.btn_upd.Text = "修改(F1)";
            this.btn_upd.UseVisualStyleBackColor = true;
            this.btn_upd.Click += new System.EventHandler(this.Btn_upd_Click);
            // 
            // btn_FormulaCodeAdd
            // 
            this.btn_FormulaCodeAdd.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_FormulaCodeAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_FormulaCodeAdd.Location = new System.Drawing.Point(1076, 7);
            this.btn_FormulaCodeAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btn_FormulaCodeAdd.Name = "btn_FormulaCodeAdd";
            this.btn_FormulaCodeAdd.Size = new System.Drawing.Size(176, 33);
            this.btn_FormulaCodeAdd.TabIndex = 50;
            this.btn_FormulaCodeAdd.Text = "新增/编辑(F5)";
            this.btn_FormulaCodeAdd.UseVisualStyleBackColor = true;
            this.btn_FormulaCodeAdd.Click += new System.EventHandler(this.Btn_FormulaCodeAdd_Click);
            // 
            // btn_pre
            // 
            this.btn_pre.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_pre.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_pre.Location = new System.Drawing.Point(538, 7);
            this.btn_pre.Margin = new System.Windows.Forms.Padding(4);
            this.btn_pre.Name = "btn_pre";
            this.btn_pre.Size = new System.Drawing.Size(177, 33);
            this.btn_pre.TabIndex = 52;
            this.btn_pre.Text = "预览(F3)";
            this.btn_pre.UseVisualStyleBackColor = true;
            this.btn_pre.Click += new System.EventHandler(this.Btn_pre_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_Save.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Save.Location = new System.Drawing.Point(269, 7);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(4);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(177, 33);
            this.btn_Save.TabIndex = 49;
            this.btn_Save.Text = "存档(F2)";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // btn_BatchAdd
            // 
            this.btn_BatchAdd.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btn_BatchAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_BatchAdd.Location = new System.Drawing.Point(0, 7);
            this.btn_BatchAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btn_BatchAdd.Name = "btn_BatchAdd";
            this.btn_BatchAdd.Size = new System.Drawing.Size(177, 33);
            this.btn_BatchAdd.TabIndex = 48;
            this.btn_BatchAdd.Text = "加入批次(F4)";
            this.btn_BatchAdd.UseVisualStyleBackColor = true;
            this.btn_BatchAdd.Click += new System.EventHandler(this.btn_BatchAdd_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1258, 459);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // CtDropDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "CtDropDetail";
            this.Size = new System.Drawing.Size(1258, 503);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Button btn_upd;
        private System.Windows.Forms.Button btn_FormulaCodeAdd;
        public System.Windows.Forms.Button btn_pre;
        public System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_BatchAdd;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
