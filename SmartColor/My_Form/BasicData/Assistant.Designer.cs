using System;
using System.Windows.Forms;

namespace SmartColor.My_Form.BasicData
{
    partial class Assistant
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Assistant));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.rdo_4 = new System.Windows.Forms.RadioButton();
            this.rdo_3 = new System.Windows.Forms.RadioButton();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Insert = new System.Windows.Forms.Button();
            this.rdo_2 = new System.Windows.Forms.RadioButton();
            this.rdo_1 = new System.Windows.Forms.RadioButton();
            this.cbo_AssistantType = new System.Windows.Forms.ComboBox();
            this.txt_AssistantBarCode = new System.Windows.Forms.TextBox();
            this.txt_Cost = new System.Windows.Forms.TextBox();
            this.txt_Intensity = new System.Windows.Forms.TextBox();
            this.txt_TermOfValidity = new System.Windows.Forms.TextBox();
            this.txt_AllowMaxColoringConcentration = new System.Windows.Forms.TextBox();
            this.txt_AllowMinColoringConcentration = new System.Windows.Forms.TextBox();
            this.txt_AssistantName = new System.Windows.Forms.TextBox();
            this.txt_AssistantCode = new System.Windows.Forms.TextBox();
            this.lab_Cost = new System.Windows.Forms.Label();
            this.lab_Intensity = new System.Windows.Forms.Label();
            this.lab_TermOfValidity = new System.Windows.Forms.Label();
            this.lab_AllowMaxColoringConcentration = new System.Windows.Forms.Label();
            this.lab_AllowMinColoringConcentration = new System.Windows.Forms.Label();
            this.lab_UnitOfAccount = new System.Windows.Forms.Label();
            this.lab_AssistantType = new System.Windows.Forms.Label();
            this.lab_AssistantName = new System.Windows.Forms.Label();
            this.lab_AssistantBarCode = new System.Windows.Forms.Label();
            this.lab_AssistantCode = new System.Windows.Forms.Label();
            this.grp_AssistantDetails = new System.Windows.Forms.GroupBox();
            this.grp_Browse = new System.Windows.Forms.GroupBox();
            this.dgv_Assistant = new SmartColor.My_Control.CtDataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grp_AssistantDetails.SuspendLayout();
            this.grp_Browse.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Assistant)).BeginInit();
            this.SuspendLayout();
            // 
            // rdo_4
            // 
            resources.ApplyResources(this.rdo_4, "rdo_4");
            this.rdo_4.Name = "rdo_4";
            this.rdo_4.TabStop = true;
            this.rdo_4.UseVisualStyleBackColor = true;
            // 
            // rdo_3
            // 
            resources.ApplyResources(this.rdo_3, "rdo_3");
            this.rdo_3.Name = "rdo_3";
            this.rdo_3.TabStop = true;
            this.rdo_3.UseVisualStyleBackColor = true;
            // 
            // btn_Delete
            // 
            resources.ApplyResources(this.btn_Delete, "btn_Delete");
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.Btn_Delete_Click);
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // btn_Insert
            // 
            resources.ApplyResources(this.btn_Insert, "btn_Insert");
            this.btn_Insert.Name = "btn_Insert";
            this.btn_Insert.UseVisualStyleBackColor = true;
            this.btn_Insert.Click += new System.EventHandler(this.Btn_Insert_Click);
            // 
            // rdo_2
            // 
            resources.ApplyResources(this.rdo_2, "rdo_2");
            this.rdo_2.Name = "rdo_2";
            this.rdo_2.TabStop = true;
            this.rdo_2.UseVisualStyleBackColor = true;
            // 
            // rdo_1
            // 
            resources.ApplyResources(this.rdo_1, "rdo_1");
            this.rdo_1.Name = "rdo_1";
            this.rdo_1.UseVisualStyleBackColor = true;
            // 
            // cbo_AssistantType
            // 
            this.cbo_AssistantType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cbo_AssistantType, "cbo_AssistantType");
            this.cbo_AssistantType.FormattingEnabled = true;
            this.cbo_AssistantType.Name = "cbo_AssistantType";
            // 
            // txt_AssistantBarCode
            // 
            resources.ApplyResources(this.txt_AssistantBarCode, "txt_AssistantBarCode");
            this.txt_AssistantBarCode.Name = "txt_AssistantBarCode";
            // 
            // txt_Cost
            // 
            resources.ApplyResources(this.txt_Cost, "txt_Cost");
            this.txt_Cost.Name = "txt_Cost";
            // 
            // txt_Intensity
            // 
            resources.ApplyResources(this.txt_Intensity, "txt_Intensity");
            this.txt_Intensity.Name = "txt_Intensity";
            // 
            // txt_TermOfValidity
            // 
            resources.ApplyResources(this.txt_TermOfValidity, "txt_TermOfValidity");
            this.txt_TermOfValidity.Name = "txt_TermOfValidity";
            // 
            // txt_AllowMaxColoringConcentration
            // 
            resources.ApplyResources(this.txt_AllowMaxColoringConcentration, "txt_AllowMaxColoringConcentration");
            this.txt_AllowMaxColoringConcentration.Name = "txt_AllowMaxColoringConcentration";
            // 
            // txt_AllowMinColoringConcentration
            // 
            resources.ApplyResources(this.txt_AllowMinColoringConcentration, "txt_AllowMinColoringConcentration");
            this.txt_AllowMinColoringConcentration.Name = "txt_AllowMinColoringConcentration";
            // 
            // txt_AssistantName
            // 
            resources.ApplyResources(this.txt_AssistantName, "txt_AssistantName");
            this.txt_AssistantName.Name = "txt_AssistantName";
            // 
            // txt_AssistantCode
            // 
            resources.ApplyResources(this.txt_AssistantCode, "txt_AssistantCode");
            this.txt_AssistantCode.Name = "txt_AssistantCode";
            // 
            // lab_Cost
            // 
            resources.ApplyResources(this.lab_Cost, "lab_Cost");
            this.lab_Cost.Name = "lab_Cost";
            // 
            // lab_Intensity
            // 
            resources.ApplyResources(this.lab_Intensity, "lab_Intensity");
            this.lab_Intensity.Name = "lab_Intensity";
            // 
            // lab_TermOfValidity
            // 
            resources.ApplyResources(this.lab_TermOfValidity, "lab_TermOfValidity");
            this.lab_TermOfValidity.Name = "lab_TermOfValidity";
            // 
            // lab_AllowMaxColoringConcentration
            // 
            resources.ApplyResources(this.lab_AllowMaxColoringConcentration, "lab_AllowMaxColoringConcentration");
            this.lab_AllowMaxColoringConcentration.Name = "lab_AllowMaxColoringConcentration";
            // 
            // lab_AllowMinColoringConcentration
            // 
            resources.ApplyResources(this.lab_AllowMinColoringConcentration, "lab_AllowMinColoringConcentration");
            this.lab_AllowMinColoringConcentration.Name = "lab_AllowMinColoringConcentration";
            // 
            // lab_UnitOfAccount
            // 
            resources.ApplyResources(this.lab_UnitOfAccount, "lab_UnitOfAccount");
            this.lab_UnitOfAccount.Name = "lab_UnitOfAccount";
            // 
            // lab_AssistantType
            // 
            resources.ApplyResources(this.lab_AssistantType, "lab_AssistantType");
            this.lab_AssistantType.Name = "lab_AssistantType";
            // 
            // lab_AssistantName
            // 
            resources.ApplyResources(this.lab_AssistantName, "lab_AssistantName");
            this.lab_AssistantName.Name = "lab_AssistantName";
            // 
            // lab_AssistantBarCode
            // 
            resources.ApplyResources(this.lab_AssistantBarCode, "lab_AssistantBarCode");
            this.lab_AssistantBarCode.Name = "lab_AssistantBarCode";
            // 
            // lab_AssistantCode
            // 
            resources.ApplyResources(this.lab_AssistantCode, "lab_AssistantCode");
            this.lab_AssistantCode.Name = "lab_AssistantCode";
            // 
            // grp_AssistantDetails
            // 
            this.grp_AssistantDetails.Controls.Add(this.rdo_4);
            this.grp_AssistantDetails.Controls.Add(this.rdo_3);
            this.grp_AssistantDetails.Controls.Add(this.btn_Delete);
            this.grp_AssistantDetails.Controls.Add(this.btn_Save);
            this.grp_AssistantDetails.Controls.Add(this.btn_Insert);
            this.grp_AssistantDetails.Controls.Add(this.rdo_2);
            this.grp_AssistantDetails.Controls.Add(this.rdo_1);
            this.grp_AssistantDetails.Controls.Add(this.cbo_AssistantType);
            this.grp_AssistantDetails.Controls.Add(this.txt_AssistantBarCode);
            this.grp_AssistantDetails.Controls.Add(this.txt_Cost);
            this.grp_AssistantDetails.Controls.Add(this.txt_Intensity);
            this.grp_AssistantDetails.Controls.Add(this.txt_TermOfValidity);
            this.grp_AssistantDetails.Controls.Add(this.txt_AllowMaxColoringConcentration);
            this.grp_AssistantDetails.Controls.Add(this.txt_AllowMinColoringConcentration);
            this.grp_AssistantDetails.Controls.Add(this.txt_AssistantName);
            this.grp_AssistantDetails.Controls.Add(this.txt_AssistantCode);
            this.grp_AssistantDetails.Controls.Add(this.lab_Cost);
            this.grp_AssistantDetails.Controls.Add(this.lab_Intensity);
            this.grp_AssistantDetails.Controls.Add(this.lab_TermOfValidity);
            this.grp_AssistantDetails.Controls.Add(this.lab_AllowMaxColoringConcentration);
            this.grp_AssistantDetails.Controls.Add(this.lab_AllowMinColoringConcentration);
            this.grp_AssistantDetails.Controls.Add(this.lab_UnitOfAccount);
            this.grp_AssistantDetails.Controls.Add(this.lab_AssistantType);
            this.grp_AssistantDetails.Controls.Add(this.lab_AssistantName);
            this.grp_AssistantDetails.Controls.Add(this.lab_AssistantBarCode);
            this.grp_AssistantDetails.Controls.Add(this.lab_AssistantCode);
            resources.ApplyResources(this.grp_AssistantDetails, "grp_AssistantDetails");
            this.grp_AssistantDetails.Name = "grp_AssistantDetails";
            this.grp_AssistantDetails.TabStop = false;
            // 
            // grp_Browse
            // 
            this.grp_Browse.Controls.Add(this.dgv_Assistant);
            resources.ApplyResources(this.grp_Browse, "grp_Browse");
            this.grp_Browse.Name = "grp_Browse";
            this.grp_Browse.TabStop = false;
            // 
            // dgv_Assistant
            // 
            this.dgv_Assistant.AllowUserToAddRows = false;
            this.dgv_Assistant.AllowUserToDeleteRows = false;
            this.dgv_Assistant.AllowUserToResizeColumns = false;
            this.dgv_Assistant.AllowUserToResizeRows = false;
            this.dgv_Assistant.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Assistant.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_Assistant.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Assistant.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_Assistant.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.dgv_Assistant, "dgv_Assistant");
            this.dgv_Assistant.MultiSelect = false;
            this.dgv_Assistant.Name = "dgv_Assistant";
            this.dgv_Assistant.ReadOnly = true;
            this.dgv_Assistant.RowHeadersVisible = false;
            this.dgv_Assistant.RowTemplate.Height = 23;
            this.dgv_Assistant.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.Column1, "Column1");
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.Column2, "Column2");
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.Column3, "Column3");
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Assistant
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grp_AssistantDetails);
            this.Controls.Add(this.grp_Browse);
            this.Name = "Assistant";
            this.grp_AssistantDetails.ResumeLayout(false);
            this.grp_AssistantDetails.PerformLayout();
            this.grp_Browse.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Assistant)).EndInit();
            this.ResumeLayout(false);

        }

       

        #endregion
        private System.Windows.Forms.RadioButton rdo_4;
        private System.Windows.Forms.RadioButton rdo_3;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_Insert;
        private System.Windows.Forms.RadioButton rdo_2;
        private System.Windows.Forms.RadioButton rdo_1;
        private System.Windows.Forms.TextBox txt_AssistantBarCode;
        private System.Windows.Forms.TextBox txt_Cost;
        private System.Windows.Forms.TextBox txt_Intensity;
        private System.Windows.Forms.TextBox txt_TermOfValidity;
        private System.Windows.Forms.TextBox txt_AllowMaxColoringConcentration;
        private System.Windows.Forms.TextBox txt_AllowMinColoringConcentration;
        private System.Windows.Forms.TextBox txt_AssistantName;
        private System.Windows.Forms.TextBox txt_AssistantCode;
        private System.Windows.Forms.Label lab_Cost;
        private System.Windows.Forms.Label lab_Intensity;
        private System.Windows.Forms.Label lab_TermOfValidity;
        private System.Windows.Forms.Label lab_AllowMaxColoringConcentration;
        private System.Windows.Forms.Label lab_AllowMinColoringConcentration;
        private System.Windows.Forms.Label lab_UnitOfAccount;
        private System.Windows.Forms.Label lab_AssistantType;
        private System.Windows.Forms.Label lab_AssistantName;
        private System.Windows.Forms.Label lab_AssistantBarCode;
        private System.Windows.Forms.Label lab_AssistantCode;
        private System.Windows.Forms.GroupBox grp_AssistantDetails;
        private System.Windows.Forms.GroupBox grp_Browse;
        private SmartColor.My_Control.CtDataGridView dgv_Assistant;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        public ComboBox cbo_AssistantType;
    }
}