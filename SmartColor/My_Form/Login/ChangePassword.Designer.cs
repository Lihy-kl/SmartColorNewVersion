namespace SmartColor.My_Form.Login
{
    partial class ChangePassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePassword));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TxtAccount = new System.Windows.Forms.TextBox();
            this.TxtOldPassword = new System.Windows.Forms.TextBox();
            this.TxtNewPassword = new System.Windows.Forms.TextBox();
            this.TxtConfirmPassword = new System.Windows.Forms.TextBox();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.TxtRealName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "账户：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 82);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 19);
            this.label2.TabIndex = 1;
            this.label2.Text = "原密码：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 143);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 19);
            this.label3.TabIndex = 2;
            this.label3.Text = "新密码：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 204);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 19);
            this.label4.TabIndex = 3;
            this.label4.Text = "确认密码：";
            // 
            // TxtAccount
            // 
            this.TxtAccount.Location = new System.Drawing.Point(125, 16);
            this.TxtAccount.Name = "TxtAccount";
            this.TxtAccount.Size = new System.Drawing.Size(168, 29);
            this.TxtAccount.TabIndex = 4;
            // 
            // TxtOldPassword
            // 
            this.TxtOldPassword.Location = new System.Drawing.Point(125, 77);
            this.TxtOldPassword.Name = "TxtOldPassword";
            this.TxtOldPassword.Size = new System.Drawing.Size(168, 29);
            this.TxtOldPassword.TabIndex = 5;
            // 
            // TxtNewPassword
            // 
            this.TxtNewPassword.Location = new System.Drawing.Point(125, 138);
            this.TxtNewPassword.Name = "TxtNewPassword";
            this.TxtNewPassword.Size = new System.Drawing.Size(168, 29);
            this.TxtNewPassword.TabIndex = 6;
            // 
            // TxtConfirmPassword
            // 
            this.TxtConfirmPassword.Location = new System.Drawing.Point(125, 199);
            this.TxtConfirmPassword.Name = "TxtConfirmPassword";
            this.TxtConfirmPassword.Size = new System.Drawing.Size(168, 29);
            this.TxtConfirmPassword.TabIndex = 7;
            // 
            // Btn_Save
            // 
            this.Btn_Save.Location = new System.Drawing.Point(90, 321);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(136, 42);
            this.Btn_Save.TabIndex = 9;
            this.Btn_Save.Text = "修改";
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // TxtRealName
            // 
            this.TxtRealName.Location = new System.Drawing.Point(125, 260);
            this.TxtRealName.Name = "TxtRealName";
            this.TxtRealName.Size = new System.Drawing.Size(168, 29);
            this.TxtRealName.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 265);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 19);
            this.label5.TabIndex = 18;
            this.label5.Text = "姓名：";
            // 
            // ChangePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 371);
            this.Controls.Add(this.TxtRealName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Btn_Save);
            this.Controls.Add(this.TxtConfirmPassword);
            this.Controls.Add(this.TxtNewPassword);
            this.Controls.Add(this.TxtOldPassword);
            this.Controls.Add(this.TxtAccount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangePassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "密码修改";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TxtAccount;
        private System.Windows.Forms.TextBox TxtOldPassword;
        private System.Windows.Forms.TextBox TxtNewPassword;
        private System.Windows.Forms.TextBox TxtConfirmPassword;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.TextBox TxtRealName;
        private System.Windows.Forms.Label label5;
    }
}