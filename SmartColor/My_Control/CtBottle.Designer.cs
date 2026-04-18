namespace SmartColor.My_Control
{
    partial class CtBottle
    {
        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TSMICorrection = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMISelf = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMIABS = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSIWash = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMIMaterialPreparationData = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSIAddWater = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMICorrection,
            this.TSMISelf,
            this.TSMIABS,
            this.TMSIWash,
            this.TSMIMaterialPreparationData,
            this.TMSIAddWater});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 158);
            // 
            // TSMICorrection
            // 
            this.TSMICorrection.Name = "TSMICorrection";
            this.TSMICorrection.Size = new System.Drawing.Size(180, 22);
            this.TSMICorrection.Text = "校正";
            this.TSMICorrection.Click += new System.EventHandler(this.TSMICorrection_Click);
            // 
            // TSMISelf
            // 
            this.TSMISelf.Name = "TSMISelf";
            this.TSMISelf.Size = new System.Drawing.Size(180, 22);
            this.TSMISelf.Text = "自检";
            this.TSMISelf.Click += new System.EventHandler(this.TSMISelf_Click);
            // 
            // TSMIABS
            // 
            this.TSMIABS.Name = "TSMIABS";
            this.TSMIABS.Size = new System.Drawing.Size(180, 22);
            this.TSMIABS.Text = "测ABS";
            this.TSMIABS.Click += new System.EventHandler(this.TSMIABS_Click);
            // 
            // TMSIWash
            // 
            this.TMSIWash.Name = "TMSIWash";
            this.TMSIWash.Size = new System.Drawing.Size(180, 22);
            this.TMSIWash.Text = "洗针筒";
            this.TMSIWash.Click += new System.EventHandler(this.TMSIWash_Click);
            // 
            // TSMIMaterialPreparationData
            // 
            this.TSMIMaterialPreparationData.Name = "TSMIMaterialPreparationData";
            this.TSMIMaterialPreparationData.Size = new System.Drawing.Size(180, 22);
            this.TSMIMaterialPreparationData.Text = "采用备料数据";
            this.TSMIMaterialPreparationData.Click += new System.EventHandler(this.TSMIMaterialPreparationData_Click);
            // 
            // TMSIAddWater
            // 
            this.TMSIAddWater.Name = "TMSIAddWater";
            this.TMSIAddWater.Size = new System.Drawing.Size(180, 22);
            this.TMSIAddWater.Text = "加水调试";
            this.TMSIAddWater.Click += new System.EventHandler(this.TMSIAddWater_Click);
            // 
            // CtBottle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Name = "CtBottle";
            this.Click += new System.EventHandler(this.CtBottle_Click);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolStripMenuItem TSMICorrection;
        private System.Windows.Forms.ToolStripMenuItem TSMISelf;
        private System.Windows.Forms.ToolStripMenuItem TSMIABS;
        private System.Windows.Forms.ToolStripMenuItem TMSIWash;
        private System.Windows.Forms.ToolStripMenuItem TSMIMaterialPreparationData;
        private System.Windows.Forms.ToolStripMenuItem TMSIAddWater;
    }
}
