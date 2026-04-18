namespace SmartColor.My_Form.ConPar
{
    partial class AbortInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AbortInfo));
            this.TxtUpdateInfo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TxtUpdateInfo
            // 
            this.TxtUpdateInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtUpdateInfo.Enabled = false;
            this.TxtUpdateInfo.Location = new System.Drawing.Point(0, 0);
            this.TxtUpdateInfo.Margin = new System.Windows.Forms.Padding(5);
            this.TxtUpdateInfo.Multiline = true;
            this.TxtUpdateInfo.Name = "TxtUpdateInfo";
            this.TxtUpdateInfo.Size = new System.Drawing.Size(653, 418);
            this.TxtUpdateInfo.TabIndex = 0;
            // 
            // AbortInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 418);
            this.Controls.Add(this.TxtUpdateInfo);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AbortInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "更新说明";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AbortInfo_FormClosing);
            this.Load += new System.EventHandler(this.UpdateInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TxtUpdateInfo;
    }
}