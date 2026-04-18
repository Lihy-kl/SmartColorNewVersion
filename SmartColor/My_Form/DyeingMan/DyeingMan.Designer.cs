namespace SmartColor.My_Form.DyeingMan
{
    partial class DyeingMan
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
                this.ctFormulaBrowse1.CurrentRowChanged -= CurrentRowChangedHandler;
                this.ctDropRecord1.CurrentRowChanged -= CurrentRowChangedHandler;
                this.ctBatchData1.CurrentRowChanged -= CurrentRowChangedHandler;
                this.ctDropDetail1.FormulaDataChange -= CtDropDetail1_FormulaDataChange;
                this.ctDropDetail1.BatchChange -= CtDropDetail1_BatchChange;
                this.ctDropDetail1.WaitChange -= CtDropDetail1_WaitChange;
                SmartColor.My_Tool.CupAuxiliary.CupFinished -= CupAuxiliary_CupFinished;
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.ctFormulaBrowse1 = new SmartColor.My_Control.CtFormulaBrowse();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ctDropRecord1 = new SmartColor.My_Control.CtDropRecord();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ctBatchData1 = new SmartColor.My_Control.CtBatchData();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ctDropDetail1 = new SmartColor.My_Control.CtDropDetail();
            this.ctDropHead1 = new SmartColor.My_Control.CtDropHead();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ctFormulaBrowse1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1576, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(313, 931);
            this.panel1.TabIndex = 0;
            // 
            // ctFormulaBrowse1
            // 
            this.ctFormulaBrowse1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctFormulaBrowse1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ctFormulaBrowse1.HeadTarget = null;
            this.ctFormulaBrowse1.Location = new System.Drawing.Point(0, 0);
            this.ctFormulaBrowse1.Margin = new System.Windows.Forms.Padding(5);
            this.ctFormulaBrowse1.Name = "ctFormulaBrowse1";
            this.ctFormulaBrowse1.Size = new System.Drawing.Size(313, 931);
            this.ctFormulaBrowse1.TabIndex = 0;
            this.ctFormulaBrowse1.TableWaitName = null;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ctDropRecord1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 686);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1576, 245);
            this.panel2.TabIndex = 1;
            // 
            // ctDropRecord1
            // 
            this.ctDropRecord1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctDropRecord1.DropHeadTarget = null;
            this.ctDropRecord1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ctDropRecord1.Location = new System.Drawing.Point(0, 0);
            this.ctDropRecord1.Margin = new System.Windows.Forms.Padding(5);
            this.ctDropRecord1.Name = "ctDropRecord1";
            this.ctDropRecord1.Size = new System.Drawing.Size(1576, 245);
            this.ctDropRecord1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ctBatchData1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(318, 686);
            this.panel3.TabIndex = 2;
            // 
            // ctBatchData1
            // 
            this.ctBatchData1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctBatchData1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ctBatchData1.HeadTarget = null;
            this.ctBatchData1.Location = new System.Drawing.Point(0, 0);
            this.ctBatchData1.Margin = new System.Windows.Forms.Padding(5);
            this.ctBatchData1.Name = "ctBatchData1";
            this.ctBatchData1.Size = new System.Drawing.Size(318, 686);
            this.ctBatchData1.TabIndex = 0;
            this.ctBatchData1.TableHeadName = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ctDropDetail1);
            this.groupBox1.Controls.Add(this.ctDropHead1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(318, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1258, 686);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配方详情";
            // 
            // ctDropDetail1
            // 
            this.ctDropDetail1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctDropDetail1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ctDropDetail1.Location = new System.Drawing.Point(3, 208);
            this.ctDropDetail1.Margin = new System.Windows.Forms.Padding(5);
            this.ctDropDetail1.Name = "ctDropDetail1";
            this.ctDropDetail1.Size = new System.Drawing.Size(1252, 475);
            this.ctDropDetail1.TabIndex = 1;
            
            // 
            // ctDropHead1
            // 
            this.ctDropHead1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ctDropHead1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ctDropHead1.Location = new System.Drawing.Point(3, 25);
            this.ctDropHead1.Margin = new System.Windows.Forms.Padding(5);
            this.ctDropHead1.Name = "ctDropHead1";
            this.ctDropHead1.Size = new System.Drawing.Size(1252, 183);
            this.ctDropHead1.TabIndex = 0;
            // 
            // DyeingMan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1889, 931);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "DyeingMan";
            this.Text = "染色管理页面";
            this.Load += new System.EventHandler(this.DyeingMan_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox groupBox1;
        private My_Control.CtDropDetail ctDropDetail1;
        private My_Control.CtDropHead ctDropHead1;
        private My_Control.CtBatchData ctBatchData1;
        private My_Control.CtDropRecord ctDropRecord1;
        private My_Control.CtFormulaBrowse ctFormulaBrowse1;
    }
}