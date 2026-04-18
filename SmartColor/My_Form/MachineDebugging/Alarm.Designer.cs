namespace SmartColor.My_Form.MachineDebugging
{
    partial class Alarm
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
            this.ctRecord1 = new SmartColor.My_Control.CtRecord();
            this.SuspendLayout();
            // 
            // ctRecord1
            // 
            this.ctRecord1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctRecord1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ctRecord1.Location = new System.Drawing.Point(0, 0);
            this.ctRecord1.Margin = new System.Windows.Forms.Padding(5);
            this.ctRecord1.Name = "ctRecord1";
            this.ctRecord1.Size = new System.Drawing.Size(1889, 931);
            this.ctRecord1.TabIndex = 0;
            // 
            // Alarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1889, 931);
            this.Controls.Add(this.ctRecord1);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "Alarm";
            this.Text = "报警页面";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Alarm_FormClosed);
            this.Load += new System.EventHandler(this.Alarm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private My_Control.CtRecord ctRecord1;
    }
}