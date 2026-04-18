namespace SmartColor.My_Form.Help
{
    partial class Register
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
            this.button1 = new System.Windows.Forms.Button();
            this.btnReg = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLicence = new System.Windows.Forms.TextBox();
            this.labRegInfo = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 14.25F);
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(11, 167);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(415, 34);
            this.button1.TabIndex = 18;
            this.button1.Text = "同步";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // btnReg
            // 
            this.btnReg.Enabled = false;
            this.btnReg.Font = new System.Drawing.Font("宋体", 14.25F);
            this.btnReg.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnReg.Location = new System.Drawing.Point(338, 169);
            this.btnReg.Name = "btnReg";
            this.btnReg.Size = new System.Drawing.Size(75, 29);
            this.btnReg.TabIndex = 16;
            this.btnReg.Text = "注册";
            this.btnReg.UseVisualStyleBackColor = true;
            this.btnReg.Click += new System.EventHandler(this.BtnReg_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(8, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 19);
            this.label2.TabIndex = 15;
            this.label2.Text = "序列号:";
            // 
            // txtLicence
            // 
            this.txtLicence.Enabled = false;
            this.txtLicence.Font = new System.Drawing.Font("宋体", 14.25F);
            this.txtLicence.Location = new System.Drawing.Point(84, 169);
            this.txtLicence.Name = "txtLicence";
            this.txtLicence.Size = new System.Drawing.Size(239, 29);
            this.txtLicence.TabIndex = 14;
            // 
            // labRegInfo
            // 
            this.labRegInfo.AutoSize = true;
            this.labRegInfo.Font = new System.Drawing.Font("宋体", 14.25F);
            this.labRegInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labRegInfo.Location = new System.Drawing.Point(334, 76);
            this.labRegInfo.Name = "labRegInfo";
            this.labRegInfo.Size = new System.Drawing.Size(66, 19);
            this.labRegInfo.TabIndex = 13;
            this.labRegInfo.Text = "未注册";
            // 
            // pictureBox2
            // 
            this.pictureBox2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox2.Location = new System.Drawing.Point(202, 61);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(36, 34);
            this.pictureBox2.TabIndex = 19;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.Location = new System.Drawing.Point(129, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(181, 152);
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // Register
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 207);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnReg);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLicence);
            this.Controls.Add(this.labRegInfo);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Register";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "注册";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Register_FormClosing);
            this.Load += new System.EventHandler(this.Register_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnReg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLicence;
        private System.Windows.Forms.Label labRegInfo;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}