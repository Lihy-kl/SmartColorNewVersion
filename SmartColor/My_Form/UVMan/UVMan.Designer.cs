namespace SmartColor.My_Form.UVMan
{
    partial class UVMan
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
               
                this.ctBatchData1.CurrentRowChanged -= CurrentRowChangedHandler;
               
                this.ctDropDetail1.FormulaDataChange -= CtDropDetail1_FormulaDataChange;
              

                this.ctDropDetail1.BatchChange -= CtDropDetail1_BatchChange;
               
                this.ctDropDetail1.WaitChange -= CtDropDetail1_WaitChange;
              
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
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.ctDropHead1 = new SmartColor.My_Control.CtDropHead();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ctBatchData1 = new SmartColor.My_Control.CtBatchData();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ctUvCurveChart1 = new SmartColor.My_Control.CtUvCurveChart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ctFormulaBrowse1 = new SmartColor.My_Control.CtFormulaBrowse();
            this.ctDropDetail1 = new SmartColor.My_Control.CtDropDetail();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.ctDropDetail1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(318, 183);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1258, 503);
            this.panel5.TabIndex = 9;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.ctDropHead1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(318, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1258, 183);
            this.panel4.TabIndex = 8;
            // 
            // ctDropHead1
            // 
            this.ctDropHead1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctDropHead1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ctDropHead1.Location = new System.Drawing.Point(0, 0);
            this.ctDropHead1.Margin = new System.Windows.Forms.Padding(5);
            this.ctDropHead1.Name = "ctDropHead1";
            this.ctDropHead1.Size = new System.Drawing.Size(1258, 183);
            this.ctDropHead1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ctBatchData1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(318, 686);
            this.panel3.TabIndex = 7;
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
            this.ctBatchData1.TableHeadName = "drop_head";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ctUvCurveChart1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 686);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1576, 245);
            this.panel2.TabIndex = 6;
            // 
            // ctUvCurveChart1
            // 
            this.ctUvCurveChart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctUvCurveChart1.Location = new System.Drawing.Point(0, 0);
            this.ctUvCurveChart1.Name = "ctUvCurveChart1";
            this.ctUvCurveChart1.Size = new System.Drawing.Size(1576, 245);
            this.ctUvCurveChart1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ctFormulaBrowse1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1576, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(313, 931);
            this.panel1.TabIndex = 5;
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
            this.ctFormulaBrowse1.TableWaitName = "wait_list";
            // 
            // ctDropDetail1
            // 
            this.ctDropDetail1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctDropDetail1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ctDropDetail1.Location = new System.Drawing.Point(0, 0);
            this.ctDropDetail1.Margin = new System.Windows.Forms.Padding(5);
            this.ctDropDetail1.Name = "ctDropDetail1";
            this.ctDropDetail1.Size = new System.Drawing.Size(1258, 503);
            this.ctDropDetail1.TabIndex = 0;
            // 
            // UVMan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1889, 931);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "UVMan";
            this.Text = "UVMan";
            this.Load += new System.EventHandler(this.UVMan_Load);
            this.panel5.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private My_Control.CtDropHead ctDropHead1;
        private My_Control.CtBatchData ctBatchData1;
        private My_Control.CtUvCurveChart ctUvCurveChart1;
        private My_Control.CtFormulaBrowse ctFormulaBrowse1;
        private My_Control.CtDropDetail ctDropDetail1;
    }
}