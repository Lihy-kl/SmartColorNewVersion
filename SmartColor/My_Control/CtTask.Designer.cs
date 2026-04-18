namespace SmartColor.My_Control
{
    partial class CtTask
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
            this.LabIndex = new System.Windows.Forms.Label();
            this.LabType = new System.Windows.Forms.Label();
            this.LabUseingTime = new System.Windows.Forms.Label();
            this.LabTaskName = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TSMIPause = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMIResume = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TMSICancel = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // LabIndex
            // 
            this.LabIndex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabIndex.Dock = System.Windows.Forms.DockStyle.Left;
            this.LabIndex.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LabIndex.Location = new System.Drawing.Point(0, 0);
            this.LabIndex.Name = "LabIndex";
            this.LabIndex.Size = new System.Drawing.Size(49, 61);
            this.LabIndex.TabIndex = 0;
            this.LabIndex.Text = "1";
            this.LabIndex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LabType
            // 
            this.LabType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabType.Dock = System.Windows.Forms.DockStyle.Left;
            this.LabType.Font = new System.Drawing.Font("宋体", 14.25F);
            this.LabType.Location = new System.Drawing.Point(49, 0);
            this.LabType.Name = "LabType";
            this.LabType.Size = new System.Drawing.Size(51, 61);
            this.LabType.TabIndex = 1;
            this.LabType.Text = "染色";
            this.LabType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LabUseingTime
            // 
            this.LabUseingTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabUseingTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LabUseingTime.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LabUseingTime.Location = new System.Drawing.Point(100, 32);
            this.LabUseingTime.Name = "LabUseingTime";
            this.LabUseingTime.Size = new System.Drawing.Size(156, 29);
            this.LabUseingTime.TabIndex = 9;
            this.LabUseingTime.Text = "00：00：00";
            this.LabUseingTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LabTaskName
            // 
            this.LabTaskName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LabTaskName.Dock = System.Windows.Forms.DockStyle.Top;
            this.LabTaskName.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LabTaskName.Location = new System.Drawing.Point(100, 0);
            this.LabTaskName.Name = "LabTaskName";
            this.LabTaskName.Size = new System.Drawing.Size(156, 32);
            this.LabTaskName.TabIndex = 8;
            this.LabTaskName.Text = "开盖";
            this.LabTaskName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMIPause,
            this.TSMIResume});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 48);
            // 
            // TSMIPause
            // 
            this.TSMIPause.Name = "TSMIPause";
            this.TSMIPause.Size = new System.Drawing.Size(124, 22);
            this.TSMIPause.Text = "暂停任务";
            this.TSMIPause.Click += new System.EventHandler(this.TSMIPause_Click);
            // 
            // TSMIResume
            // 
            this.TSMIResume.Name = "TSMIResume";
            this.TSMIResume.Size = new System.Drawing.Size(124, 22);
            this.TSMIResume.Text = "恢复任务";
            this.TSMIResume.Click += new System.EventHandler(this.TSMIResume_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TMSICancel});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(125, 26);
            // 
            // TMSICancel
            // 
            this.TMSICancel.Name = "TMSICancel";
            this.TMSICancel.Size = new System.Drawing.Size(124, 22);
            this.TMSICancel.Text = "取消任务";
            this.TMSICancel.Click += new System.EventHandler(this.TMSICancel_Click);
            // 
            // CtTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.LabUseingTime);
            this.Controls.Add(this.LabTaskName);
            this.Controls.Add(this.LabType);
            this.Controls.Add(this.LabIndex);
            this.Name = "CtTask";
            this.Size = new System.Drawing.Size(256, 61);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem TSMIPause;
        private System.Windows.Forms.ToolStripMenuItem TSMIResume;
        private System.Windows.Forms.ToolStripMenuItem TMSICancel;
        public System.Windows.Forms.Label LabType;
        public System.Windows.Forms.Label LabUseingTime;
        public System.Windows.Forms.Label LabTaskName;
        public System.Windows.Forms.Label LabIndex;
    }
}
