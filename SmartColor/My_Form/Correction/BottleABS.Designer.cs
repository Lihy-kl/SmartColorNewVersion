namespace SmartColor.My_Form.Correction
{
    partial class BottleABS
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
            this.label1 = new System.Windows.Forms.Label();
            this.TxtRecheckWeight = new System.Windows.Forms.TextBox();
            this.BtnABS = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(122, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "瓶号:";
            // 
            // TxtRecheckWeight
            // 
            this.TxtRecheckWeight.Location = new System.Drawing.Point(2, 77);
            this.TxtRecheckWeight.Multiline = true;
            this.TxtRecheckWeight.Name = "TxtRecheckWeight";
            this.TxtRecheckWeight.Size = new System.Drawing.Size(297, 101);
            this.TxtRecheckWeight.TabIndex = 1;
            // 
            // BtnABS
            // 
            this.BtnABS.Location = new System.Drawing.Point(66, 193);
            this.BtnABS.Name = "BtnABS";
            this.BtnABS.Size = new System.Drawing.Size(169, 45);
            this.BtnABS.TabIndex = 4;
            this.BtnABS.Text = "ABS启动";
            this.BtnABS.UseVisualStyleBackColor = true;
            this.BtnABS.Click += new System.EventHandler(this.BtnABS_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(31, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(239, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "(连续可用‘-’,否则可用‘,’)";
            // 
            // BottleABS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 249);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BtnABS);
            this.Controls.Add(this.TxtRecheckWeight);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.Name = "BottleABS";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "母液瓶批量测ABS";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtRecheckWeight;
        private System.Windows.Forms.Button BtnABS;
        private System.Windows.Forms.Label label2;
    }
}