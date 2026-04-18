namespace SmartColor.My_Form.ConPar
{
    partial class OrderInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderInfo));
            this.ctModuleDragPanel1 = new SmartColor.My_Control.CtModuleDragPanel();
            this.SuspendLayout();
            // 
            // ctModuleDragPanel1
            // 
            this.ctModuleDragPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctModuleDragPanel1.Location = new System.Drawing.Point(0, 0);
            this.ctModuleDragPanel1.Name = "ctModuleDragPanel1";
            this.ctModuleDragPanel1.Size = new System.Drawing.Size(470, 468);
            this.ctModuleDragPanel1.TabIndex = 0;
            // 
            // OrderInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 468);
            this.Controls.Add(this.ctModuleDragPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OrderInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "优先级调整";
            this.Load += new System.EventHandler(this.OrderInfo_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private My_Control.CtModuleDragPanel ctModuleDragPanel1;
    }
}