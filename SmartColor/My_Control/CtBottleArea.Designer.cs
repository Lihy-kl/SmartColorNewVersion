namespace SmartColor.My_Control
{
    partial class CtBottleArea
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
            this.TSMIWaterCorrection = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSIDMF = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMIBottleCorrection = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMIBottleCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMIBottleABS = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMIBottleWash = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMIBottleAddWater = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMIWaterCorrection,
            this.TMSIDMF,
            this.TSMIBottleCorrection,
            this.TSMIBottleCheck,
            this.TSMIBottleABS,
            this.TSMIBottleWash,
            this.TSMIBottleAddWater});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 180);
            // 
            // TSMIWaterCorrection
            // 
            this.TSMIWaterCorrection.Name = "TSMIWaterCorrection";
            this.TSMIWaterCorrection.Size = new System.Drawing.Size(180, 22);
            this.TSMIWaterCorrection.Text = "水校正/验证";
            this.TSMIWaterCorrection.Click += new System.EventHandler(this.TSMIWaterCorrection_Click);
            // 
            // TMSIDMF
            // 
            this.TMSIDMF.Name = "TMSIDMF";
            this.TMSIDMF.Size = new System.Drawing.Size(180, 22);
            this.TMSIDMF.Text = "溶解液校正/验证";
            this.TMSIDMF.Click += new System.EventHandler(this.TMSIDMF_Click);
            // 
            // TSMIBottleCorrection
            // 
            this.TSMIBottleCorrection.Name = "TSMIBottleCorrection";
            this.TSMIBottleCorrection.Size = new System.Drawing.Size(180, 22);
            this.TSMIBottleCorrection.Text = "母液瓶批量校正";
            this.TSMIBottleCorrection.Click += new System.EventHandler(this.TSMIBottleCorrection_Click);
            // 
            // TSMIBottleCheck
            // 
            this.TSMIBottleCheck.Name = "TSMIBottleCheck";
            this.TSMIBottleCheck.Size = new System.Drawing.Size(180, 22);
            this.TSMIBottleCheck.Text = "母液瓶批量自检";
            this.TSMIBottleCheck.Click += new System.EventHandler(this.TSMIBottleSelf_Click);
            // 
            // TSMIBottleABS
            // 
            this.TSMIBottleABS.Name = "TSMIBottleABS";
            this.TSMIBottleABS.Size = new System.Drawing.Size(180, 22);
            this.TSMIBottleABS.Text = "母液瓶批量测ABS";
            this.TSMIBottleABS.Click += new System.EventHandler(this.TSMIBottleABS_Click);
            // 
            // TSMIBottleWash
            // 
            this.TSMIBottleWash.Name = "TSMIBottleWash";
            this.TSMIBottleWash.Size = new System.Drawing.Size(180, 22);
            this.TSMIBottleWash.Text = "母液瓶批量洗针筒";
            this.TSMIBottleWash.Click += new System.EventHandler(this.TSMIBottleWash_Click);
            // 
            // TSMIBottleAddWater
            // 
            this.TSMIBottleAddWater.Name = "TSMIBottleAddWater";
            this.TSMIBottleAddWater.Size = new System.Drawing.Size(180, 22);
            this.TSMIBottleAddWater.Text = "母液瓶批量加水";
            this.TSMIBottleAddWater.Click += new System.EventHandler(this.TSMIBottleAddWater_Click);
            // 
            // CtBottleArea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "CtBottleArea";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem TSMIWaterCorrection;
        private System.Windows.Forms.ToolStripMenuItem TMSIDMF;
        private System.Windows.Forms.ToolStripMenuItem TSMIBottleCorrection;
        private System.Windows.Forms.ToolStripMenuItem TSMIBottleCheck;
        private System.Windows.Forms.ToolStripMenuItem TSMIBottleABS;
        private System.Windows.Forms.ToolStripMenuItem TSMIBottleWash;
        private System.Windows.Forms.ToolStripMenuItem TSMIBottleAddWater;
    }
}
