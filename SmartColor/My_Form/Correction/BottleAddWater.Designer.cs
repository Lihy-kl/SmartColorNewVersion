namespace SmartColor.My_Form.Correction
{
    partial class BottleAddWater
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
            this.TxtAddWaterBottleNos = new System.Windows.Forms.TextBox();
            this.BtnAddWaterDebug = new System.Windows.Forms.Button();
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
            this.TxtAddWaterBottleNos.Location = new System.Drawing.Point(2, 77);
            this.TxtAddWaterBottleNos.Multiline = true;
            this.TxtAddWaterBottleNos.Name = "TxtRecheckWeight";
            this.TxtAddWaterBottleNos.Size = new System.Drawing.Size(297, 101);
            this.TxtAddWaterBottleNos.TabIndex = 1;
            // 
            // BtnAddWaterDebug
            // 
            this.BtnAddWaterDebug.Location = new System.Drawing.Point(66, 193);
            this.BtnAddWaterDebug.Name = "BtnAddWaterDebug";
            this.BtnAddWaterDebug.Size = new System.Drawing.Size(169, 45);
            this.BtnAddWaterDebug.TabIndex = 4;
            this.BtnAddWaterDebug.Text = "加水调试启动";
            this.BtnAddWaterDebug.UseVisualStyleBackColor = true;
            this.BtnAddWaterDebug.Click += new System.EventHandler(this.BtnAddWaterDebug_Click);
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
            // BottleAddWater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 249);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BtnAddWaterDebug);
            this.Controls.Add(this.TxtAddWaterBottleNos);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.Name = "BottleAddWater";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "母液瓶批量加水调试";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtAddWaterBottleNos;
        private System.Windows.Forms.Button BtnAddWaterDebug;
        private System.Windows.Forms.Label label2;
    }
}