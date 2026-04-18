namespace SmartColor.My_Control
{
    partial class CtTemperatureChart
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Tsmi_Process = new System.Windows.Forms.ToolStripMenuItem();
            this.Tmsi_Measured = new System.Windows.Forms.ToolStripMenuItem();
            this.Tsmi_Both = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Tmsi_Measured,
            this.Tsmi_Process,
            this.Tsmi_Both});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 92);
            // 
            // Tsmi_Process
            // 
            this.Tsmi_Process.Name = "Tsmi_Process";
            this.Tsmi_Process.Size = new System.Drawing.Size(180, 22);
            this.Tsmi_Process.Text = "单理论";
            this.Tsmi_Process.Click += new System.EventHandler(this.Tsmi_Process_Click);
            // 
            // Tmsi_Measured
            // 
            this.Tmsi_Measured.Name = "Tmsi_Measured";
            this.Tmsi_Measured.Size = new System.Drawing.Size(180, 22);
            this.Tmsi_Measured.Text = "单实际";
            this.Tmsi_Measured.Click += new System.EventHandler(this.Tmsi_Measured_Click);
            // 
            // Tsmi_Both
            // 
            this.Tsmi_Both.Name = "Tsmi_Both";
            this.Tsmi_Both.Size = new System.Drawing.Size(180, 22);
            this.Tsmi_Both.Text = "共同显示";
            this.Tsmi_Both.Click += new System.EventHandler(this.Tsmi_Both_Click);
            // 
            // CtTemperatureChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "CtTemperatureChart";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem Tsmi_Process;
        private System.Windows.Forms.ToolStripMenuItem Tmsi_Measured;
        private System.Windows.Forms.ToolStripMenuItem Tsmi_Both;
        public System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}
