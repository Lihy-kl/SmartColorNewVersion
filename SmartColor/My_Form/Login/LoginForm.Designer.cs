namespace SmartColor.My_Form.Login
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.TxtPassword = new System.Windows.Forms.TextBox();
            this.TxtName = new System.Windows.Forms.TextBox();
            this.BtnLogOn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LabPasswoedChange = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TxtPassword
            // 
            this.TxtPassword.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.TxtPassword.Location = new System.Drawing.Point(318, 299);
            this.TxtPassword.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.TxtPassword.Name = "TxtPassword";
            this.TxtPassword.PasswordChar = '*';
            this.TxtPassword.Size = new System.Drawing.Size(110, 29);
            this.TxtPassword.TabIndex = 20;
            this.TxtPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TxtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtPassword_KeyDown);
            // 
            // TxtName
            // 
            this.TxtName.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.TxtName.Location = new System.Drawing.Point(318, 243);
            this.TxtName.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.TxtName.Name = "TxtName";
            this.TxtName.Size = new System.Drawing.Size(110, 29);
            this.TxtName.TabIndex = 19;
            this.TxtName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TxtName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtName_KeyDown);
            // 
            // BtnLogOn
            // 
            this.BtnLogOn.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.BtnLogOn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnLogOn.Location = new System.Drawing.Point(269, 373);
            this.BtnLogOn.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.BtnLogOn.Name = "BtnLogOn";
            this.BtnLogOn.Size = new System.Drawing.Size(120, 31);
            this.BtnLogOn.TabIndex = 21;
            this.BtnLogOn.Text = "登录";
            this.BtnLogOn.UseVisualStyleBackColor = true;
            this.BtnLogOn.Click += new System.EventHandler(this.BtnLogOn_Click);
            this.BtnLogOn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BtnLogOn_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(227, 303);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 19);
            this.label3.TabIndex = 18;
            this.label3.Text = "密  码:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(227, 247);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 19);
            this.label2.TabIndex = 17;
            this.label2.Text = "用户名:";
            // 
            // LabPasswoedChange
            // 
            this.LabPasswoedChange.AutoSize = true;
            this.LabPasswoedChange.BackColor = System.Drawing.Color.Transparent;
            this.LabPasswoedChange.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LabPasswoedChange.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabPasswoedChange.Location = new System.Drawing.Point(365, 347);
            this.LabPasswoedChange.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabPasswoedChange.Name = "LabPasswoedChange";
            this.LabPasswoedChange.Size = new System.Drawing.Size(63, 14);
            this.LabPasswoedChange.TabIndex = 23;
            this.LabPasswoedChange.Text = "修改密码";
            this.LabPasswoedChange.Click += new System.EventHandler(this.LabPasswoedChange_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::SmartColor.Properties.Resources.登录;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(740, 485);
            this.Controls.Add(this.LabPasswoedChange);
            this.Controls.Add(this.TxtPassword);
            this.Controls.Add(this.TxtName);
            this.Controls.Add(this.BtnLogOn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "欢迎使用智能染色系统";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.Shown += new System.EventHandler(this.LoginForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TxtPassword;
        private System.Windows.Forms.TextBox TxtName;
        private System.Windows.Forms.Button BtnLogOn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LabPasswoedChange;
    }
}