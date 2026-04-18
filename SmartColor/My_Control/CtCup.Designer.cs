namespace SmartColor.My_Control
{
    partial class CtCup
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
       

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TsmiOnLine = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiOffLine = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiStop = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiWash = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiHighTemWash = new System.Windows.Forms.ToolStripMenuItem();
            this.TmsiChangeLidStatus = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.TsmiClose = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TsmiOnLine,
            this.TsmiOffLine,
            this.TsmiStop,
            this.TsmiWash,
            this.TsmiOpen,
            this.TsmiClose,
            this.TsmiHighTemWash,
            this.TmsiChangeLidStatus});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 202);
            // 
            // TsmiOnLine
            // 
            this.TsmiOnLine.Name = "TsmiOnLine";
            this.TsmiOnLine.Size = new System.Drawing.Size(180, 22);
            this.TsmiOnLine.Text = "上线";
            this.TsmiOnLine.Click += new System.EventHandler(this.TsmiOnLine_Click);
            // 
            // TsmiOffLine
            // 
            this.TsmiOffLine.Name = "TsmiOffLine";
            this.TsmiOffLine.Size = new System.Drawing.Size(180, 22);
            this.TsmiOffLine.Text = "下线";
            this.TsmiOffLine.Click += new System.EventHandler(this.TsmiOffLine_Click);
            // 
            // TsmiStop
            // 
            this.TsmiStop.Name = "TsmiStop";
            this.TsmiStop.Size = new System.Drawing.Size(180, 22);
            this.TsmiStop.Text = "停止";
            this.TsmiStop.Click += new System.EventHandler(this.TsmiStop_Click);
            // 
            // TsmiWash
            // 
            this.TsmiWash.Name = "TsmiWash";
            this.TsmiWash.Size = new System.Drawing.Size(180, 22);
            this.TsmiWash.Text = "洗杯";
            this.TsmiWash.Click += new System.EventHandler(this.TsmiWash_Click);
            // 
            // TsmiHighTemWash
            // 
            this.TsmiHighTemWash.Name = "TsmiHighTemWash";
            this.TsmiHighTemWash.Size = new System.Drawing.Size(180, 22);
            this.TsmiHighTemWash.Text = "高温洗杯";
            this.TsmiHighTemWash.Click += new System.EventHandler(this.TsmiHighTemWash_Click);
            // 
            // TmsiChangeLidStatus
            // 
            this.TmsiChangeLidStatus.Name = "TmsiChangeLidStatus";
            this.TmsiChangeLidStatus.Size = new System.Drawing.Size(180, 22);
            this.TmsiChangeLidStatus.Text = "切换杯盖状态";
            this.TmsiChangeLidStatus.Click += new System.EventHandler(this.TmsiChangeLidStatus_Click);
            // 
            // TsmiOpen
            // 
            this.TsmiOpen.Name = "TsmiOpen";
            this.TsmiOpen.Size = new System.Drawing.Size(180, 22);
            this.TsmiOpen.Text = "开盖";
            this.TsmiOpen.Click += new System.EventHandler(this.TsmiOpen_Click);
            // 
            // TsmiClose
            // 
            this.TsmiClose.Name = "TsmiClose";
            this.TsmiClose.Size = new System.Drawing.Size(180, 22);
            this.TsmiClose.Text = "关盖";
            this.TsmiClose.Click += new System.EventHandler(this.TsmiClose_Click);
            // 
            // CtCup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Name = "CtCup";
            this.Click += new System.EventHandler(this.CtCup_Click);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem TsmiOnLine;
        private System.Windows.Forms.ToolStripMenuItem TsmiOffLine;
        private System.Windows.Forms.ToolStripMenuItem TsmiStop;
        private System.Windows.Forms.ToolStripMenuItem TsmiHighTemWash;
        private System.Windows.Forms.ToolStripMenuItem TsmiWash;
        private System.Windows.Forms.ToolStripMenuItem TmsiChangeLidStatus;
        private System.Windows.Forms.ToolStripMenuItem TsmiOpen;
        private System.Windows.Forms.ToolStripMenuItem TsmiClose;
    }
}
