namespace SmartColor.My_Form.MachineDebugging
{
    partial class Debug
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
            if (disposing)
            {
                if (Tmr != null)
                {
                    Tmr.Stop();
                    Tmr.Dispose();
                }
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.components = new System.ComponentModel.Container();
            this.BtnOutPut_Slow = new System.Windows.Forms.Button();
            this.BtnOutPut_ResetX = new System.Windows.Forms.Button();
            this.ChkInPut_SupportCover = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Back = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Slow_Cylinder_Mid = new System.Windows.Forms.CheckBox();
            this.BtnOutPut_ResetY = new System.Windows.Forms.Button();
            this.BtnOutPut_Block_Out = new System.Windows.Forms.Button();
            this.ChkInPut_Cylinder_Block = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Apocenosis_Up = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Decompression_Up = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Block_Out = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Block_In = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Decompression_Down = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Cylinder_Mid = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Y_Ready = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Tray_Out = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Stop = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Sunx_B = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Sunx_A = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Syringe = new System.Windows.Forms.CheckBox();
            this.BtnOutPut_Wash_Blow = new System.Windows.Forms.Button();
            this.BtnOutPut_Wash_Out = new System.Windows.Forms.Button();
            this.BtnOutPut_Wash_In = new System.Windows.Forms.Button();
            this.BtnOutPut_Block_Cylinder = new System.Windows.Forms.Button();
            this.BtnOutPut_Decompression = new System.Windows.Forms.Button();
            this.grp_out = new System.Windows.Forms.GroupBox();
            this.BtnOutPut_Buzzer = new System.Windows.Forms.Button();
            this.BtnOutPut_Tray = new System.Windows.Forms.Button();
            this.BtnOutPut_Water = new System.Windows.Forms.Button();
            this.BtnOutPut_Waste = new System.Windows.Forms.Button();
            this.BtnOutPut_Blender = new System.Windows.Forms.Button();
            this.BtnOutPut_Cylinder_Up = new System.Windows.Forms.Button();
            this.BtnOutPut_TongsOn = new System.Windows.Forms.Button();
            this.BtnOutPut_Y_Power = new System.Windows.Forms.Button();
            this.BtnOutPut_X_Power = new System.Windows.Forms.Button();
            this.Tmr = new System.Windows.Forms.Timer(this.components);
            this.RdoWash = new System.Windows.Forms.RadioButton();
            this.BtnGenerate = new System.Windows.Forms.Button();
            this.BtnWrite = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.RdoWetClamp = new System.Windows.Forms.RadioButton();
            this.RdoDryClamp = new System.Windows.Forms.RadioButton();
            this.RdoWetCloth = new System.Windows.Forms.RadioButton();
            this.RdoDryCloth = new System.Windows.Forms.RadioButton();
            this.RdoDecompression = new System.Windows.Forms.RadioButton();
            this.RdoCupLid = new System.Windows.Forms.RadioButton();
            this.RdoBalance = new System.Windows.Forms.RadioButton();
            this.RdoCup = new System.Windows.Forms.RadioButton();
            this.RdoBottle = new System.Windows.Forms.RadioButton();
            this.BtnStartMove = new System.Windows.Forms.Button();
            this.TxtNum = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.ChkInPut_Tongs_B = new System.Windows.Forms.CheckBox();
            this.BtnStop = new System.Windows.Forms.Button();
            this.TxtRPosY = new System.Windows.Forms.TextBox();
            this.TxtRPosX = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.RdoZ = new System.Windows.Forms.RadioButton();
            this.RdoY = new System.Windows.Forms.RadioButton();
            this.RdoX = new System.Windows.Forms.RadioButton();
            this.TxtCPosZ = new System.Windows.Forms.TextBox();
            this.TxtCSpeedZ = new System.Windows.Forms.TextBox();
            this.TxtUpSpeedZ = new System.Windows.Forms.TextBox();
            this.TxtHSpeedZ = new System.Windows.Forms.TextBox();
            this.TxtLSpeedZ = new System.Windows.Forms.TextBox();
            this.TxtPulseZ = new System.Windows.Forms.TextBox();
            this.TxtCSpeedY = new System.Windows.Forms.TextBox();
            this.TxtUpSpeedY = new System.Windows.Forms.TextBox();
            this.TxtHSpeedY = new System.Windows.Forms.TextBox();
            this.TxtLSpeedY = new System.Windows.Forms.TextBox();
            this.TxtPulseY = new System.Windows.Forms.TextBox();
            this.TxtCSpeedX = new System.Windows.Forms.TextBox();
            this.TxtUpSpeedX = new System.Windows.Forms.TextBox();
            this.TxtHSpeedX = new System.Windows.Forms.TextBox();
            this.TxtLSpeedX = new System.Windows.Forms.TextBox();
            this.TxtPulseX = new System.Windows.Forms.TextBox();
            this.BtnMove = new System.Windows.Forms.Button();
            this.grp_move = new System.Windows.Forms.GroupBox();
            this.BtnHome = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ChkInPut_Tongs_A = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Tray_In = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Cylinder_Down = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Cylinder_Up = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Z_Origin = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Z_Corotation = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Y_Origin = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Y_Alarm = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Y_Reverse = new System.Windows.Forms.CheckBox();
            this.ChkInPut_Y_Corotation = new System.Windows.Forms.CheckBox();
            this.ChkInPut_X_Origin = new System.Windows.Forms.CheckBox();
            this.ChkInPut_X_Alarm = new System.Windows.Forms.CheckBox();
            this.ChkInPut_X_Ready = new System.Windows.Forms.CheckBox();
            this.ChkInPut_X_Reverse = new System.Windows.Forms.CheckBox();
            this.grp_in = new System.Windows.Forms.GroupBox();
            this.ChkInPut_X_Corotation = new System.Windows.Forms.CheckBox();
            this.Btn_Reset = new System.Windows.Forms.Button();
            this.LabBalanceValue = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TxtCylinderEncoder = new System.Windows.Forms.Label();
            this.grp_out.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.grp_move.SuspendLayout();
            this.grp_in.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnOutPut_Slow
            // 
            this.BtnOutPut_Slow.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Slow.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Slow.Location = new System.Drawing.Point(555, 170);
            this.BtnOutPut_Slow.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Slow.Name = "BtnOutPut_Slow";
            this.BtnOutPut_Slow.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Slow.TabIndex = 19;
            this.BtnOutPut_Slow.Text = "气缸慢速中";
            this.BtnOutPut_Slow.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_ResetX
            // 
            this.BtnOutPut_ResetX.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_ResetX.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_ResetX.Location = new System.Drawing.Point(284, 303);
            this.BtnOutPut_ResetX.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_ResetX.Name = "BtnOutPut_ResetX";
            this.BtnOutPut_ResetX.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_ResetX.TabIndex = 17;
            this.BtnOutPut_ResetX.Text = "X轴报警复位";
            this.BtnOutPut_ResetX.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_SupportCover
            // 
            this.ChkInPut_SupportCover.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_SupportCover.Location = new System.Drawing.Point(526, 479);
            this.ChkInPut_SupportCover.Name = "ChkInPut_SupportCover";
            this.ChkInPut_SupportCover.Size = new System.Drawing.Size(123, 24);
            this.ChkInPut_SupportCover.TabIndex = 46;
            this.ChkInPut_SupportCover.Text = "撑盖开到位";
            this.ChkInPut_SupportCover.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Back
            // 
            this.ChkInPut_Back.AutoSize = true;
            this.ChkInPut_Back.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Back.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Back.Location = new System.Drawing.Point(372, 480);
            this.ChkInPut_Back.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Back.Name = "ChkInPut_Back";
            this.ChkInPut_Back.Size = new System.Drawing.Size(142, 23);
            this.ChkInPut_Back.TabIndex = 45;
            this.ChkInPut_Back.Text = "后光幕感应位";
            this.ChkInPut_Back.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Slow_Cylinder_Mid
            // 
            this.ChkInPut_Slow_Cylinder_Mid.AutoSize = true;
            this.ChkInPut_Slow_Cylinder_Mid.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Slow_Cylinder_Mid.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Slow_Cylinder_Mid.Location = new System.Drawing.Point(199, 480);
            this.ChkInPut_Slow_Cylinder_Mid.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Slow_Cylinder_Mid.Name = "ChkInPut_Slow_Cylinder_Mid";
            this.ChkInPut_Slow_Cylinder_Mid.Size = new System.Drawing.Size(161, 23);
            this.ChkInPut_Slow_Cylinder_Mid.TabIndex = 44;
            this.ChkInPut_Slow_Cylinder_Mid.Text = "气缸慢速中限位";
            this.ChkInPut_Slow_Cylinder_Mid.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_ResetY
            // 
            this.BtnOutPut_ResetY.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_ResetY.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_ResetY.Location = new System.Drawing.Point(555, 303);
            this.BtnOutPut_ResetY.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_ResetY.Name = "BtnOutPut_ResetY";
            this.BtnOutPut_ResetY.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_ResetY.TabIndex = 18;
            this.BtnOutPut_ResetY.Text = "Y轴报警复位";
            this.BtnOutPut_ResetY.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_Block_Out
            // 
            this.BtnOutPut_Block_Out.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Block_Out.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Block_Out.Location = new System.Drawing.Point(11, 169);
            this.BtnOutPut_Block_Out.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Block_Out.Name = "BtnOutPut_Block_Out";
            this.BtnOutPut_Block_Out.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Block_Out.TabIndex = 15;
            this.BtnOutPut_Block_Out.Text = "阻挡";
            this.BtnOutPut_Block_Out.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Cylinder_Block
            // 
            this.ChkInPut_Cylinder_Block.AutoSize = true;
            this.ChkInPut_Cylinder_Block.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Cylinder_Block.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Cylinder_Block.Location = new System.Drawing.Point(26, 480);
            this.ChkInPut_Cylinder_Block.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Cylinder_Block.Name = "ChkInPut_Cylinder_Block";
            this.ChkInPut_Cylinder_Block.Size = new System.Drawing.Size(142, 23);
            this.ChkInPut_Cylinder_Block.TabIndex = 43;
            this.ChkInPut_Cylinder_Block.Text = "气缸阻挡限位";
            this.ChkInPut_Cylinder_Block.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Apocenosis_Up
            // 
            this.ChkInPut_Apocenosis_Up.AutoSize = true;
            this.ChkInPut_Apocenosis_Up.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Apocenosis_Up.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Apocenosis_Up.Location = new System.Drawing.Point(526, 416);
            this.ChkInPut_Apocenosis_Up.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Apocenosis_Up.Name = "ChkInPut_Apocenosis_Up";
            this.ChkInPut_Apocenosis_Up.Size = new System.Drawing.Size(123, 23);
            this.ChkInPut_Apocenosis_Up.TabIndex = 42;
            this.ChkInPut_Apocenosis_Up.Text = "排液上限位";
            this.ChkInPut_Apocenosis_Up.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Decompression_Up
            // 
            this.ChkInPut_Decompression_Up.AutoSize = true;
            this.ChkInPut_Decompression_Up.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Decompression_Up.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Decompression_Up.Location = new System.Drawing.Point(372, 352);
            this.ChkInPut_Decompression_Up.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Decompression_Up.Name = "ChkInPut_Decompression_Up";
            this.ChkInPut_Decompression_Up.Size = new System.Drawing.Size(123, 23);
            this.ChkInPut_Decompression_Up.TabIndex = 41;
            this.ChkInPut_Decompression_Up.Text = "泄压上到位";
            this.ChkInPut_Decompression_Up.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Block_Out
            // 
            this.ChkInPut_Block_Out.AutoSize = true;
            this.ChkInPut_Block_Out.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Block_Out.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Block_Out.Location = new System.Drawing.Point(199, 352);
            this.ChkInPut_Block_Out.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Block_Out.Name = "ChkInPut_Block_Out";
            this.ChkInPut_Block_Out.Size = new System.Drawing.Size(123, 23);
            this.ChkInPut_Block_Out.TabIndex = 40;
            this.ChkInPut_Block_Out.Text = "阻挡出到位";
            this.ChkInPut_Block_Out.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Block_In
            // 
            this.ChkInPut_Block_In.AutoSize = true;
            this.ChkInPut_Block_In.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Block_In.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Block_In.Location = new System.Drawing.Point(26, 352);
            this.ChkInPut_Block_In.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Block_In.Name = "ChkInPut_Block_In";
            this.ChkInPut_Block_In.Size = new System.Drawing.Size(123, 23);
            this.ChkInPut_Block_In.TabIndex = 39;
            this.ChkInPut_Block_In.Text = "阻挡回到位";
            this.ChkInPut_Block_In.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Decompression_Down
            // 
            this.ChkInPut_Decompression_Down.AutoSize = true;
            this.ChkInPut_Decompression_Down.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Decompression_Down.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Decompression_Down.Location = new System.Drawing.Point(526, 352);
            this.ChkInPut_Decompression_Down.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Decompression_Down.Name = "ChkInPut_Decompression_Down";
            this.ChkInPut_Decompression_Down.Size = new System.Drawing.Size(123, 23);
            this.ChkInPut_Decompression_Down.TabIndex = 38;
            this.ChkInPut_Decompression_Down.Text = "泄压下到位";
            this.ChkInPut_Decompression_Down.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Cylinder_Mid
            // 
            this.ChkInPut_Cylinder_Mid.AutoSize = true;
            this.ChkInPut_Cylinder_Mid.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Cylinder_Mid.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Cylinder_Mid.Location = new System.Drawing.Point(372, 224);
            this.ChkInPut_Cylinder_Mid.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Cylinder_Mid.Name = "ChkInPut_Cylinder_Mid";
            this.ChkInPut_Cylinder_Mid.Size = new System.Drawing.Size(123, 23);
            this.ChkInPut_Cylinder_Mid.TabIndex = 37;
            this.ChkInPut_Cylinder_Mid.Text = "气缸中限位";
            this.ChkInPut_Cylinder_Mid.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Y_Ready
            // 
            this.ChkInPut_Y_Ready.AutoSize = true;
            this.ChkInPut_Y_Ready.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Y_Ready.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Y_Ready.Location = new System.Drawing.Point(372, 96);
            this.ChkInPut_Y_Ready.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Y_Ready.Name = "ChkInPut_Y_Ready";
            this.ChkInPut_Y_Ready.Size = new System.Drawing.Size(114, 23);
            this.ChkInPut_Y_Ready.TabIndex = 36;
            this.ChkInPut_Y_Ready.Text = "Y轴准备位";
            this.ChkInPut_Y_Ready.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Tray_Out
            // 
            this.ChkInPut_Tray_Out.AutoSize = true;
            this.ChkInPut_Tray_Out.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Tray_Out.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Tray_Out.Location = new System.Drawing.Point(372, 288);
            this.ChkInPut_Tray_Out.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Tray_Out.Name = "ChkInPut_Tray_Out";
            this.ChkInPut_Tray_Out.Size = new System.Drawing.Size(142, 23);
            this.ChkInPut_Tray_Out.TabIndex = 35;
            this.ChkInPut_Tray_Out.Text = "接液盘出到位";
            this.ChkInPut_Tray_Out.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Stop
            // 
            this.ChkInPut_Stop.AutoSize = true;
            this.ChkInPut_Stop.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Stop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Stop.Location = new System.Drawing.Point(372, 416);
            this.ChkInPut_Stop.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Stop.Name = "ChkInPut_Stop";
            this.ChkInPut_Stop.Size = new System.Drawing.Size(142, 23);
            this.ChkInPut_Stop.TabIndex = 34;
            this.ChkInPut_Stop.Text = "前光幕感应位";
            this.ChkInPut_Stop.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Sunx_B
            // 
            this.ChkInPut_Sunx_B.AutoSize = true;
            this.ChkInPut_Sunx_B.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Sunx_B.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Sunx_B.Location = new System.Drawing.Point(199, 416);
            this.ChkInPut_Sunx_B.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Sunx_B.Name = "ChkInPut_Sunx_B";
            this.ChkInPut_Sunx_B.Size = new System.Drawing.Size(142, 23);
            this.ChkInPut_Sunx_B.TabIndex = 33;
            this.ChkInPut_Sunx_B.Text = "左光幕感应位";
            this.ChkInPut_Sunx_B.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Sunx_A
            // 
            this.ChkInPut_Sunx_A.AutoSize = true;
            this.ChkInPut_Sunx_A.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Sunx_A.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Sunx_A.Location = new System.Drawing.Point(26, 416);
            this.ChkInPut_Sunx_A.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Sunx_A.Name = "ChkInPut_Sunx_A";
            this.ChkInPut_Sunx_A.Size = new System.Drawing.Size(142, 23);
            this.ChkInPut_Sunx_A.TabIndex = 32;
            this.ChkInPut_Sunx_A.Text = "右光幕感应位";
            this.ChkInPut_Sunx_A.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Syringe
            // 
            this.ChkInPut_Syringe.AutoSize = true;
            this.ChkInPut_Syringe.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Syringe.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Syringe.Location = new System.Drawing.Point(526, 224);
            this.ChkInPut_Syringe.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Syringe.Name = "ChkInPut_Syringe";
            this.ChkInPut_Syringe.Size = new System.Drawing.Size(123, 23);
            this.ChkInPut_Syringe.TabIndex = 31;
            this.ChkInPut_Syringe.Text = "针筒感应位";
            this.ChkInPut_Syringe.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_Wash_Blow
            // 
            this.BtnOutPut_Wash_Blow.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Wash_Blow.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Wash_Blow.Location = new System.Drawing.Point(11, 303);
            this.BtnOutPut_Wash_Blow.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Wash_Blow.Name = "BtnOutPut_Wash_Blow";
            this.BtnOutPut_Wash_Blow.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Wash_Blow.TabIndex = 23;
            this.BtnOutPut_Wash_Blow.Text = "洗针吹气阀";
            this.BtnOutPut_Wash_Blow.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_Wash_Out
            // 
            this.BtnOutPut_Wash_Out.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Wash_Out.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Wash_Out.Location = new System.Drawing.Point(555, 373);
            this.BtnOutPut_Wash_Out.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Wash_Out.Name = "BtnOutPut_Wash_Out";
            this.BtnOutPut_Wash_Out.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Wash_Out.TabIndex = 22;
            this.BtnOutPut_Wash_Out.Text = "洗针排水阀";
            this.BtnOutPut_Wash_Out.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_Wash_In
            // 
            this.BtnOutPut_Wash_In.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Wash_In.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Wash_In.Location = new System.Drawing.Point(284, 373);
            this.BtnOutPut_Wash_In.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Wash_In.Name = "BtnOutPut_Wash_In";
            this.BtnOutPut_Wash_In.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Wash_In.TabIndex = 21;
            this.BtnOutPut_Wash_In.Text = "洗针进水阀";
            this.BtnOutPut_Wash_In.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_Block_Cylinder
            // 
            this.BtnOutPut_Block_Cylinder.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Block_Cylinder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Block_Cylinder.Location = new System.Drawing.Point(11, 370);
            this.BtnOutPut_Block_Cylinder.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Block_Cylinder.Name = "BtnOutPut_Block_Cylinder";
            this.BtnOutPut_Block_Cylinder.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Block_Cylinder.TabIndex = 20;
            this.BtnOutPut_Block_Cylinder.Text = "气缸到阻挡位";
            this.BtnOutPut_Block_Cylinder.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_Decompression
            // 
            this.BtnOutPut_Decompression.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Decompression.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Decompression.Location = new System.Drawing.Point(284, 169);
            this.BtnOutPut_Decompression.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Decompression.Name = "BtnOutPut_Decompression";
            this.BtnOutPut_Decompression.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Decompression.TabIndex = 14;
            this.BtnOutPut_Decompression.Text = "泄压下";
            this.BtnOutPut_Decompression.UseVisualStyleBackColor = true;
            // 
            // grp_out
            // 
            this.grp_out.Controls.Add(this.BtnOutPut_Wash_Blow);
            this.grp_out.Controls.Add(this.BtnOutPut_Wash_Out);
            this.grp_out.Controls.Add(this.BtnOutPut_Wash_In);
            this.grp_out.Controls.Add(this.BtnOutPut_Block_Cylinder);
            this.grp_out.Controls.Add(this.BtnOutPut_Slow);
            this.grp_out.Controls.Add(this.BtnOutPut_ResetY);
            this.grp_out.Controls.Add(this.BtnOutPut_ResetX);
            this.grp_out.Controls.Add(this.BtnOutPut_Block_Out);
            this.grp_out.Controls.Add(this.BtnOutPut_Decompression);
            this.grp_out.Controls.Add(this.BtnOutPut_Buzzer);
            this.grp_out.Controls.Add(this.BtnOutPut_Tray);
            this.grp_out.Controls.Add(this.BtnOutPut_Water);
            this.grp_out.Controls.Add(this.BtnOutPut_Waste);
            this.grp_out.Controls.Add(this.BtnOutPut_Blender);
            this.grp_out.Controls.Add(this.BtnOutPut_Cylinder_Up);
            this.grp_out.Controls.Add(this.BtnOutPut_TongsOn);
            this.grp_out.Controls.Add(this.BtnOutPut_Y_Power);
            this.grp_out.Controls.Add(this.BtnOutPut_X_Power);
            this.grp_out.Font = new System.Drawing.Font("宋体", 14.25F);
            this.grp_out.Location = new System.Drawing.Point(678, 305);
            this.grp_out.Margin = new System.Windows.Forms.Padding(2);
            this.grp_out.Name = "grp_out";
            this.grp_out.Padding = new System.Windows.Forms.Padding(2);
            this.grp_out.Size = new System.Drawing.Size(744, 625);
            this.grp_out.TabIndex = 7;
            this.grp_out.TabStop = false;
            this.grp_out.Text = "输出监控";
            // 
            // BtnOutPut_Buzzer
            // 
            this.BtnOutPut_Buzzer.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Buzzer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Buzzer.Location = new System.Drawing.Point(11, 236);
            this.BtnOutPut_Buzzer.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Buzzer.Name = "BtnOutPut_Buzzer";
            this.BtnOutPut_Buzzer.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Buzzer.TabIndex = 12;
            this.BtnOutPut_Buzzer.Text = "蜂鸣器";
            this.BtnOutPut_Buzzer.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_Tray
            // 
            this.BtnOutPut_Tray.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Tray.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Tray.Location = new System.Drawing.Point(284, 102);
            this.BtnOutPut_Tray.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Tray.Name = "BtnOutPut_Tray";
            this.BtnOutPut_Tray.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Tray.TabIndex = 7;
            this.BtnOutPut_Tray.Text = "接液盘";
            this.BtnOutPut_Tray.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_Water
            // 
            this.BtnOutPut_Water.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Water.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Water.Location = new System.Drawing.Point(284, 236);
            this.BtnOutPut_Water.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Water.Name = "BtnOutPut_Water";
            this.BtnOutPut_Water.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Water.TabIndex = 6;
            this.BtnOutPut_Water.Text = "水";
            this.BtnOutPut_Water.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_Waste
            // 
            this.BtnOutPut_Waste.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Waste.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Waste.Location = new System.Drawing.Point(556, 236);
            this.BtnOutPut_Waste.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Waste.Name = "BtnOutPut_Waste";
            this.BtnOutPut_Waste.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Waste.TabIndex = 5;
            this.BtnOutPut_Waste.Text = "废液回抽";
            this.BtnOutPut_Waste.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_Blender
            // 
            this.BtnOutPut_Blender.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Blender.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Blender.Location = new System.Drawing.Point(11, 102);
            this.BtnOutPut_Blender.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Blender.Name = "BtnOutPut_Blender";
            this.BtnOutPut_Blender.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Blender.TabIndex = 4;
            this.BtnOutPut_Blender.Text = "搅拌停";
            this.BtnOutPut_Blender.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_Cylinder_Up
            // 
            this.BtnOutPut_Cylinder_Up.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Cylinder_Up.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Cylinder_Up.Location = new System.Drawing.Point(555, 35);
            this.BtnOutPut_Cylinder_Up.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Cylinder_Up.Name = "BtnOutPut_Cylinder_Up";
            this.BtnOutPut_Cylinder_Up.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Cylinder_Up.TabIndex = 3;
            this.BtnOutPut_Cylinder_Up.Text = "气缸上";
            this.BtnOutPut_Cylinder_Up.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_TongsOn
            // 
            this.BtnOutPut_TongsOn.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_TongsOn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_TongsOn.Location = new System.Drawing.Point(556, 102);
            this.BtnOutPut_TongsOn.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_TongsOn.Name = "BtnOutPut_TongsOn";
            this.BtnOutPut_TongsOn.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_TongsOn.TabIndex = 2;
            this.BtnOutPut_TongsOn.Text = "抓手";
            this.BtnOutPut_TongsOn.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_Y_Power
            // 
            this.BtnOutPut_Y_Power.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_Y_Power.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_Y_Power.Location = new System.Drawing.Point(283, 35);
            this.BtnOutPut_Y_Power.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_Y_Power.Name = "BtnOutPut_Y_Power";
            this.BtnOutPut_Y_Power.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_Y_Power.TabIndex = 1;
            this.BtnOutPut_Y_Power.Text = "Y矢能";
            this.BtnOutPut_Y_Power.UseVisualStyleBackColor = true;
            // 
            // BtnOutPut_X_Power
            // 
            this.BtnOutPut_X_Power.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnOutPut_X_Power.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnOutPut_X_Power.Location = new System.Drawing.Point(11, 35);
            this.BtnOutPut_X_Power.Margin = new System.Windows.Forms.Padding(2);
            this.BtnOutPut_X_Power.Name = "BtnOutPut_X_Power";
            this.BtnOutPut_X_Power.Size = new System.Drawing.Size(166, 64);
            this.BtnOutPut_X_Power.TabIndex = 0;
            this.BtnOutPut_X_Power.Text = "X矢能";
            this.BtnOutPut_X_Power.UseVisualStyleBackColor = true;
            // 
            // Tmr
            // 
            this.Tmr.Enabled = true;
            // 
            // RdoWash
            // 
            this.RdoWash.AutoSize = true;
            this.RdoWash.Font = new System.Drawing.Font("宋体", 14.25F);
            this.RdoWash.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RdoWash.Location = new System.Drawing.Point(127, 162);
            this.RdoWash.Margin = new System.Windows.Forms.Padding(2);
            this.RdoWash.Name = "RdoWash";
            this.RdoWash.Size = new System.Drawing.Size(65, 23);
            this.RdoWash.TabIndex = 19;
            this.RdoWash.TabStop = true;
            this.RdoWash.Text = "洗针";
            this.RdoWash.UseVisualStyleBackColor = true;
            // 
            // BtnGenerate
            // 
            this.BtnGenerate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnGenerate.Location = new System.Drawing.Point(248, 404);
            this.BtnGenerate.Name = "BtnGenerate";
            this.BtnGenerate.Size = new System.Drawing.Size(173, 48);
            this.BtnGenerate.TabIndex = 18;
            this.BtnGenerate.Text = "生成坐标";
            this.BtnGenerate.UseVisualStyleBackColor = true;
            this.BtnGenerate.Click += new System.EventHandler(this.BtnGenerate_Click);
            // 
            // BtnWrite
            // 
            this.BtnWrite.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnWrite.Location = new System.Drawing.Point(44, 404);
            this.BtnWrite.Name = "BtnWrite";
            this.BtnWrite.Size = new System.Drawing.Size(173, 48);
            this.BtnWrite.TabIndex = 17;
            this.BtnWrite.Text = "写入坐标";
            this.BtnWrite.UseVisualStyleBackColor = true;
            this.BtnWrite.Click += new System.EventHandler(this.BtnWrite_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.RdoWash);
            this.groupBox5.Controls.Add(this.BtnGenerate);
            this.groupBox5.Controls.Add(this.BtnWrite);
            this.groupBox5.Controls.Add(this.RdoWetClamp);
            this.groupBox5.Controls.Add(this.RdoDryClamp);
            this.groupBox5.Controls.Add(this.RdoWetCloth);
            this.groupBox5.Controls.Add(this.RdoDryCloth);
            this.groupBox5.Controls.Add(this.RdoDecompression);
            this.groupBox5.Controls.Add(this.RdoCupLid);
            this.groupBox5.Controls.Add(this.RdoBalance);
            this.groupBox5.Controls.Add(this.RdoCup);
            this.groupBox5.Controls.Add(this.RdoBottle);
            this.groupBox5.Controls.Add(this.BtnStartMove);
            this.groupBox5.Controls.Add(this.TxtNum);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Font = new System.Drawing.Font("宋体", 14.25F);
            this.groupBox5.Location = new System.Drawing.Point(1426, 305);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(458, 625);
            this.groupBox5.TabIndex = 8;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "定点移动";
            // 
            // RdoWetClamp
            // 
            this.RdoWetClamp.AutoSize = true;
            this.RdoWetClamp.Font = new System.Drawing.Font("宋体", 14.25F);
            this.RdoWetClamp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RdoWetClamp.Location = new System.Drawing.Point(19, 162);
            this.RdoWetClamp.Margin = new System.Windows.Forms.Padding(2);
            this.RdoWetClamp.Name = "RdoWetClamp";
            this.RdoWetClamp.Size = new System.Drawing.Size(103, 23);
            this.RdoWetClamp.TabIndex = 15;
            this.RdoWetClamp.TabStop = true;
            this.RdoWetClamp.Text = "湿布夹子";
            this.RdoWetClamp.UseVisualStyleBackColor = true;
            // 
            // RdoDryClamp
            // 
            this.RdoDryClamp.AutoSize = true;
            this.RdoDryClamp.Font = new System.Drawing.Font("宋体", 14.25F);
            this.RdoDryClamp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RdoDryClamp.Location = new System.Drawing.Point(343, 121);
            this.RdoDryClamp.Margin = new System.Windows.Forms.Padding(2);
            this.RdoDryClamp.Name = "RdoDryClamp";
            this.RdoDryClamp.Size = new System.Drawing.Size(103, 23);
            this.RdoDryClamp.TabIndex = 14;
            this.RdoDryClamp.TabStop = true;
            this.RdoDryClamp.Text = "干布夹子";
            this.RdoDryClamp.UseVisualStyleBackColor = true;
            // 
            // RdoWetCloth
            // 
            this.RdoWetCloth.AutoSize = true;
            this.RdoWetCloth.Font = new System.Drawing.Font("宋体", 14.25F);
            this.RdoWetCloth.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RdoWetCloth.Location = new System.Drawing.Point(235, 121);
            this.RdoWetCloth.Margin = new System.Windows.Forms.Padding(2);
            this.RdoWetCloth.Name = "RdoWetCloth";
            this.RdoWetCloth.Size = new System.Drawing.Size(84, 23);
            this.RdoWetCloth.TabIndex = 13;
            this.RdoWetCloth.TabStop = true;
            this.RdoWetCloth.Text = "出布区";
            this.RdoWetCloth.UseVisualStyleBackColor = true;
            // 
            // RdoDryCloth
            // 
            this.RdoDryCloth.AutoSize = true;
            this.RdoDryCloth.Font = new System.Drawing.Font("宋体", 14.25F);
            this.RdoDryCloth.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RdoDryCloth.Location = new System.Drawing.Point(127, 121);
            this.RdoDryCloth.Margin = new System.Windows.Forms.Padding(2);
            this.RdoDryCloth.Name = "RdoDryCloth";
            this.RdoDryCloth.Size = new System.Drawing.Size(84, 23);
            this.RdoDryCloth.TabIndex = 12;
            this.RdoDryCloth.TabStop = true;
            this.RdoDryCloth.Text = "备布区";
            this.RdoDryCloth.UseVisualStyleBackColor = true;
            // 
            // RdoDecompression
            // 
            this.RdoDecompression.AutoSize = true;
            this.RdoDecompression.Font = new System.Drawing.Font("宋体", 14.25F);
            this.RdoDecompression.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RdoDecompression.Location = new System.Drawing.Point(19, 121);
            this.RdoDecompression.Margin = new System.Windows.Forms.Padding(2);
            this.RdoDecompression.Name = "RdoDecompression";
            this.RdoDecompression.Size = new System.Drawing.Size(84, 23);
            this.RdoDecompression.TabIndex = 11;
            this.RdoDecompression.TabStop = true;
            this.RdoDecompression.Text = "泄压区";
            this.RdoDecompression.UseVisualStyleBackColor = true;
            // 
            // RdoCupLid
            // 
            this.RdoCupLid.AutoSize = true;
            this.RdoCupLid.Font = new System.Drawing.Font("宋体", 14.25F);
            this.RdoCupLid.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RdoCupLid.Location = new System.Drawing.Point(235, 76);
            this.RdoCupLid.Margin = new System.Windows.Forms.Padding(2);
            this.RdoCupLid.Name = "RdoCupLid";
            this.RdoCupLid.Size = new System.Drawing.Size(84, 23);
            this.RdoCupLid.TabIndex = 10;
            this.RdoCupLid.TabStop = true;
            this.RdoCupLid.Text = "杯盖区";
            this.RdoCupLid.UseVisualStyleBackColor = true;
            // 
            // RdoBalance
            // 
            this.RdoBalance.AutoSize = true;
            this.RdoBalance.Font = new System.Drawing.Font("宋体", 14.25F);
            this.RdoBalance.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RdoBalance.Location = new System.Drawing.Point(343, 76);
            this.RdoBalance.Margin = new System.Windows.Forms.Padding(2);
            this.RdoBalance.Name = "RdoBalance";
            this.RdoBalance.Size = new System.Drawing.Size(84, 23);
            this.RdoBalance.TabIndex = 9;
            this.RdoBalance.TabStop = true;
            this.RdoBalance.Text = "天平位";
            this.RdoBalance.UseVisualStyleBackColor = true;
            // 
            // RdoCup
            // 
            this.RdoCup.AutoSize = true;
            this.RdoCup.Font = new System.Drawing.Font("宋体", 14.25F);
            this.RdoCup.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RdoCup.Location = new System.Drawing.Point(127, 76);
            this.RdoCup.Margin = new System.Windows.Forms.Padding(2);
            this.RdoCup.Name = "RdoCup";
            this.RdoCup.Size = new System.Drawing.Size(84, 23);
            this.RdoCup.TabIndex = 8;
            this.RdoCup.TabStop = true;
            this.RdoCup.Text = "配液区";
            this.RdoCup.UseVisualStyleBackColor = true;
            // 
            // RdoBottle
            // 
            this.RdoBottle.AutoSize = true;
            this.RdoBottle.Font = new System.Drawing.Font("宋体", 14.25F);
            this.RdoBottle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RdoBottle.Location = new System.Drawing.Point(19, 76);
            this.RdoBottle.Margin = new System.Windows.Forms.Padding(2);
            this.RdoBottle.Name = "RdoBottle";
            this.RdoBottle.Size = new System.Drawing.Size(84, 23);
            this.RdoBottle.TabIndex = 7;
            this.RdoBottle.TabStop = true;
            this.RdoBottle.Text = "母液区";
            this.RdoBottle.UseVisualStyleBackColor = true;
            // 
            // BtnStartMove
            // 
            this.BtnStartMove.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnStartMove.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnStartMove.Location = new System.Drawing.Point(176, 295);
            this.BtnStartMove.Margin = new System.Windows.Forms.Padding(2);
            this.BtnStartMove.Name = "BtnStartMove";
            this.BtnStartMove.Size = new System.Drawing.Size(147, 80);
            this.BtnStartMove.TabIndex = 5;
            this.BtnStartMove.Text = "启  动";
            this.BtnStartMove.UseVisualStyleBackColor = true;
            this.BtnStartMove.Click += new System.EventHandler(this.BtnStartMove_Click);
            // 
            // TxtNum
            // 
            this.TxtNum.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtNum.Location = new System.Drawing.Point(185, 225);
            this.TxtNum.Margin = new System.Windows.Forms.Padding(2);
            this.TxtNum.Name = "TxtNum";
            this.TxtNum.Size = new System.Drawing.Size(184, 29);
            this.TxtNum.TabIndex = 4;
            this.TxtNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(95, 230);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(86, 19);
            this.label12.TabIndex = 3;
            this.label12.Text = "瓶/杯号:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ChkInPut_Tongs_B
            // 
            this.ChkInPut_Tongs_B.AutoSize = true;
            this.ChkInPut_Tongs_B.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Tongs_B.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Tongs_B.Location = new System.Drawing.Point(199, 288);
            this.ChkInPut_Tongs_B.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Tongs_B.Name = "ChkInPut_Tongs_B";
            this.ChkInPut_Tongs_B.Size = new System.Drawing.Size(133, 23);
            this.ChkInPut_Tongs_B.TabIndex = 30;
            this.ChkInPut_Tongs_B.Text = "B抓手回到位";
            this.ChkInPut_Tongs_B.UseVisualStyleBackColor = true;
            // 
            // BtnStop
            // 
            this.BtnStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnStop.Location = new System.Drawing.Point(608, 240);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(202, 40);
            this.BtnStop.TabIndex = 68;
            this.BtnStop.Text = "停止";
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // TxtRPosY
            // 
            this.TxtRPosY.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtRPosY.Location = new System.Drawing.Point(1241, 130);
            this.TxtRPosY.Margin = new System.Windows.Forms.Padding(2);
            this.TxtRPosY.Name = "TxtRPosY";
            this.TxtRPosY.Size = new System.Drawing.Size(139, 29);
            this.TxtRPosY.TabIndex = 67;
            this.TxtRPosY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtRPosX
            // 
            this.TxtRPosX.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtRPosX.Location = new System.Drawing.Point(1241, 80);
            this.TxtRPosX.Margin = new System.Windows.Forms.Padding(2);
            this.TxtRPosX.Name = "TxtRPosX";
            this.TxtRPosX.Size = new System.Drawing.Size(139, 29);
            this.TxtRPosX.TabIndex = 66;
            this.TxtRPosX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(1263, 37);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 19);
            this.label7.TabIndex = 65;
            this.label7.Text = "实际位置";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RdoZ
            // 
            this.RdoZ.AutoSize = true;
            this.RdoZ.Font = new System.Drawing.Font("宋体", 14.25F);
            this.RdoZ.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RdoZ.Location = new System.Drawing.Point(25, 183);
            this.RdoZ.Margin = new System.Windows.Forms.Padding(2);
            this.RdoZ.Name = "RdoZ";
            this.RdoZ.Size = new System.Drawing.Size(76, 23);
            this.RdoZ.TabIndex = 60;
            this.RdoZ.TabStop = true;
            this.RdoZ.Text = "Z  轴";
            this.RdoZ.UseVisualStyleBackColor = true;
            // 
            // RdoY
            // 
            this.RdoY.AutoSize = true;
            this.RdoY.Font = new System.Drawing.Font("宋体", 14.25F);
            this.RdoY.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RdoY.Location = new System.Drawing.Point(25, 133);
            this.RdoY.Margin = new System.Windows.Forms.Padding(2);
            this.RdoY.Name = "RdoY";
            this.RdoY.Size = new System.Drawing.Size(76, 23);
            this.RdoY.TabIndex = 59;
            this.RdoY.TabStop = true;
            this.RdoY.Text = "Y  轴";
            this.RdoY.UseVisualStyleBackColor = true;
            // 
            // RdoX
            // 
            this.RdoX.AutoSize = true;
            this.RdoX.Font = new System.Drawing.Font("宋体", 14.25F);
            this.RdoX.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.RdoX.Location = new System.Drawing.Point(25, 83);
            this.RdoX.Margin = new System.Windows.Forms.Padding(2);
            this.RdoX.Name = "RdoX";
            this.RdoX.Size = new System.Drawing.Size(76, 23);
            this.RdoX.TabIndex = 58;
            this.RdoX.TabStop = true;
            this.RdoX.Text = "X  轴";
            this.RdoX.UseVisualStyleBackColor = true;
            // 
            // TxtCPosZ
            // 
            this.TxtCPosZ.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtCPosZ.Location = new System.Drawing.Point(1241, 180);
            this.TxtCPosZ.Margin = new System.Windows.Forms.Padding(2);
            this.TxtCPosZ.Name = "TxtCPosZ";
            this.TxtCPosZ.Size = new System.Drawing.Size(139, 29);
            this.TxtCPosZ.TabIndex = 33;
            this.TxtCPosZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtCSpeedZ
            // 
            this.TxtCSpeedZ.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtCSpeedZ.Location = new System.Drawing.Point(1029, 180);
            this.TxtCSpeedZ.Margin = new System.Windows.Forms.Padding(2);
            this.TxtCSpeedZ.Name = "TxtCSpeedZ";
            this.TxtCSpeedZ.Size = new System.Drawing.Size(139, 29);
            this.TxtCSpeedZ.TabIndex = 32;
            this.TxtCSpeedZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtUpSpeedZ
            // 
            this.TxtUpSpeedZ.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtUpSpeedZ.Location = new System.Drawing.Point(813, 180);
            this.TxtUpSpeedZ.Margin = new System.Windows.Forms.Padding(2);
            this.TxtUpSpeedZ.Name = "TxtUpSpeedZ";
            this.TxtUpSpeedZ.Size = new System.Drawing.Size(139, 29);
            this.TxtUpSpeedZ.TabIndex = 31;
            this.TxtUpSpeedZ.Text = "50000";
            this.TxtUpSpeedZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtHSpeedZ
            // 
            this.TxtHSpeedZ.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtHSpeedZ.Location = new System.Drawing.Point(596, 180);
            this.TxtHSpeedZ.Margin = new System.Windows.Forms.Padding(2);
            this.TxtHSpeedZ.Name = "TxtHSpeedZ";
            this.TxtHSpeedZ.Size = new System.Drawing.Size(139, 29);
            this.TxtHSpeedZ.TabIndex = 30;
            this.TxtHSpeedZ.Text = "8400";
            this.TxtHSpeedZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtLSpeedZ
            // 
            this.TxtLSpeedZ.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtLSpeedZ.Location = new System.Drawing.Point(389, 180);
            this.TxtLSpeedZ.Margin = new System.Windows.Forms.Padding(2);
            this.TxtLSpeedZ.Name = "TxtLSpeedZ";
            this.TxtLSpeedZ.Size = new System.Drawing.Size(139, 29);
            this.TxtLSpeedZ.TabIndex = 29;
            this.TxtLSpeedZ.Text = "0";
            this.TxtLSpeedZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtPulseZ
            // 
            this.TxtPulseZ.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtPulseZ.Location = new System.Drawing.Point(182, 180);
            this.TxtPulseZ.Margin = new System.Windows.Forms.Padding(2);
            this.TxtPulseZ.Name = "TxtPulseZ";
            this.TxtPulseZ.Size = new System.Drawing.Size(139, 29);
            this.TxtPulseZ.TabIndex = 28;
            this.TxtPulseZ.Text = "50000";
            this.TxtPulseZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtCSpeedY
            // 
            this.TxtCSpeedY.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtCSpeedY.Location = new System.Drawing.Point(1029, 130);
            this.TxtCSpeedY.Margin = new System.Windows.Forms.Padding(2);
            this.TxtCSpeedY.Name = "TxtCSpeedY";
            this.TxtCSpeedY.Size = new System.Drawing.Size(139, 29);
            this.TxtCSpeedY.TabIndex = 26;
            this.TxtCSpeedY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtUpSpeedY
            // 
            this.TxtUpSpeedY.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtUpSpeedY.Location = new System.Drawing.Point(813, 130);
            this.TxtUpSpeedY.Margin = new System.Windows.Forms.Padding(2);
            this.TxtUpSpeedY.Name = "TxtUpSpeedY";
            this.TxtUpSpeedY.Size = new System.Drawing.Size(139, 29);
            this.TxtUpSpeedY.TabIndex = 25;
            this.TxtUpSpeedY.Text = "2000";
            this.TxtUpSpeedY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtHSpeedY
            // 
            this.TxtHSpeedY.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtHSpeedY.Location = new System.Drawing.Point(596, 130);
            this.TxtHSpeedY.Margin = new System.Windows.Forms.Padding(2);
            this.TxtHSpeedY.Name = "TxtHSpeedY";
            this.TxtHSpeedY.Size = new System.Drawing.Size(139, 29);
            this.TxtHSpeedY.TabIndex = 24;
            this.TxtHSpeedY.Text = "1000";
            this.TxtHSpeedY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtLSpeedY
            // 
            this.TxtLSpeedY.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtLSpeedY.Location = new System.Drawing.Point(389, 130);
            this.TxtLSpeedY.Margin = new System.Windows.Forms.Padding(2);
            this.TxtLSpeedY.Name = "TxtLSpeedY";
            this.TxtLSpeedY.Size = new System.Drawing.Size(139, 29);
            this.TxtLSpeedY.TabIndex = 23;
            this.TxtLSpeedY.Text = "0";
            this.TxtLSpeedY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtPulseY
            // 
            this.TxtPulseY.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtPulseY.Location = new System.Drawing.Point(182, 130);
            this.TxtPulseY.Margin = new System.Windows.Forms.Padding(2);
            this.TxtPulseY.Name = "TxtPulseY";
            this.TxtPulseY.Size = new System.Drawing.Size(139, 29);
            this.TxtPulseY.TabIndex = 22;
            this.TxtPulseY.Text = "50000";
            this.TxtPulseY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtCSpeedX
            // 
            this.TxtCSpeedX.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtCSpeedX.Location = new System.Drawing.Point(1029, 80);
            this.TxtCSpeedX.Margin = new System.Windows.Forms.Padding(2);
            this.TxtCSpeedX.Name = "TxtCSpeedX";
            this.TxtCSpeedX.Size = new System.Drawing.Size(139, 29);
            this.TxtCSpeedX.TabIndex = 20;
            this.TxtCSpeedX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtUpSpeedX
            // 
            this.TxtUpSpeedX.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtUpSpeedX.Location = new System.Drawing.Point(813, 80);
            this.TxtUpSpeedX.Margin = new System.Windows.Forms.Padding(2);
            this.TxtUpSpeedX.Name = "TxtUpSpeedX";
            this.TxtUpSpeedX.Size = new System.Drawing.Size(139, 29);
            this.TxtUpSpeedX.TabIndex = 19;
            this.TxtUpSpeedX.Text = "2000";
            this.TxtUpSpeedX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtHSpeedX
            // 
            this.TxtHSpeedX.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtHSpeedX.Location = new System.Drawing.Point(596, 80);
            this.TxtHSpeedX.Margin = new System.Windows.Forms.Padding(2);
            this.TxtHSpeedX.Name = "TxtHSpeedX";
            this.TxtHSpeedX.Size = new System.Drawing.Size(139, 29);
            this.TxtHSpeedX.TabIndex = 18;
            this.TxtHSpeedX.Text = "1000";
            this.TxtHSpeedX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtLSpeedX
            // 
            this.TxtLSpeedX.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtLSpeedX.Location = new System.Drawing.Point(389, 80);
            this.TxtLSpeedX.Margin = new System.Windows.Forms.Padding(2);
            this.TxtLSpeedX.Name = "TxtLSpeedX";
            this.TxtLSpeedX.Size = new System.Drawing.Size(139, 29);
            this.TxtLSpeedX.TabIndex = 17;
            this.TxtLSpeedX.Text = "0";
            this.TxtLSpeedX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtPulseX
            // 
            this.TxtPulseX.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtPulseX.Location = new System.Drawing.Point(182, 80);
            this.TxtPulseX.Margin = new System.Windows.Forms.Padding(2);
            this.TxtPulseX.Name = "TxtPulseX";
            this.TxtPulseX.Size = new System.Drawing.Size(139, 29);
            this.TxtPulseX.TabIndex = 16;
            this.TxtPulseX.Text = "50000";
            this.TxtPulseX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BtnMove
            // 
            this.BtnMove.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnMove.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnMove.Location = new System.Drawing.Point(986, 240);
            this.BtnMove.Margin = new System.Windows.Forms.Padding(2);
            this.BtnMove.Name = "BtnMove";
            this.BtnMove.Size = new System.Drawing.Size(202, 40);
            this.BtnMove.TabIndex = 15;
            this.BtnMove.Text = "联   动";
            this.BtnMove.UseVisualStyleBackColor = true;
            this.BtnMove.Click += new System.EventHandler(this.BtnMove_Click);
            // 
            // grp_move
            // 
            this.grp_move.Controls.Add(this.BtnStop);
            this.grp_move.Controls.Add(this.TxtRPosY);
            this.grp_move.Controls.Add(this.TxtRPosX);
            this.grp_move.Controls.Add(this.label7);
            this.grp_move.Controls.Add(this.RdoZ);
            this.grp_move.Controls.Add(this.RdoY);
            this.grp_move.Controls.Add(this.RdoX);
            this.grp_move.Controls.Add(this.TxtCPosZ);
            this.grp_move.Controls.Add(this.TxtCSpeedZ);
            this.grp_move.Controls.Add(this.TxtUpSpeedZ);
            this.grp_move.Controls.Add(this.TxtHSpeedZ);
            this.grp_move.Controls.Add(this.TxtLSpeedZ);
            this.grp_move.Controls.Add(this.TxtPulseZ);
            this.grp_move.Controls.Add(this.TxtCSpeedY);
            this.grp_move.Controls.Add(this.TxtUpSpeedY);
            this.grp_move.Controls.Add(this.TxtHSpeedY);
            this.grp_move.Controls.Add(this.TxtLSpeedY);
            this.grp_move.Controls.Add(this.TxtPulseY);
            this.grp_move.Controls.Add(this.TxtCSpeedX);
            this.grp_move.Controls.Add(this.TxtUpSpeedX);
            this.grp_move.Controls.Add(this.TxtHSpeedX);
            this.grp_move.Controls.Add(this.TxtLSpeedX);
            this.grp_move.Controls.Add(this.TxtPulseX);
            this.grp_move.Controls.Add(this.BtnMove);
            this.grp_move.Controls.Add(this.BtnHome);
            this.grp_move.Controls.Add(this.label6);
            this.grp_move.Controls.Add(this.label5);
            this.grp_move.Controls.Add(this.label4);
            this.grp_move.Controls.Add(this.label3);
            this.grp_move.Controls.Add(this.label2);
            this.grp_move.Controls.Add(this.label1);
            this.grp_move.Font = new System.Drawing.Font("宋体", 14.25F);
            this.grp_move.Location = new System.Drawing.Point(-6, 1);
            this.grp_move.Margin = new System.Windows.Forms.Padding(2);
            this.grp_move.Name = "grp_move";
            this.grp_move.Padding = new System.Windows.Forms.Padding(2);
            this.grp_move.Size = new System.Drawing.Size(1428, 300);
            this.grp_move.TabIndex = 4;
            this.grp_move.TabStop = false;
            this.grp_move.Text = "运动监控";
            // 
            // BtnHome
            // 
            this.BtnHome.Font = new System.Drawing.Font("宋体", 14.25F);
            this.BtnHome.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnHome.Location = new System.Drawing.Point(230, 240);
            this.BtnHome.Margin = new System.Windows.Forms.Padding(2);
            this.BtnHome.Name = "BtnHome";
            this.BtnHome.Size = new System.Drawing.Size(202, 40);
            this.BtnHome.TabIndex = 13;
            this.BtnHome.Text = "原  点";
            this.BtnHome.UseVisualStyleBackColor = true;
            this.BtnHome.Click += new System.EventHandler(this.BtnHome_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(1056, 37);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 19);
            this.label6.TabIndex = 11;
            this.label6.Text = "当前速度";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(849, 37);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 19);
            this.label5.TabIndex = 10;
            this.label5.Text = "加减速";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(623, 37);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 19);
            this.label4.TabIndex = 9;
            this.label4.Text = "驱动速度";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(416, 37);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 19);
            this.label3.TabIndex = 8;
            this.label3.Text = "初始速度";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(209, 37);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 19);
            this.label2.TabIndex = 7;
            this.label2.Text = "目标脉冲";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 14.25F);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(40, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "轴号";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ChkInPut_Tongs_A
            // 
            this.ChkInPut_Tongs_A.AutoSize = true;
            this.ChkInPut_Tongs_A.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Tongs_A.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Tongs_A.Location = new System.Drawing.Point(26, 288);
            this.ChkInPut_Tongs_A.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Tongs_A.Name = "ChkInPut_Tongs_A";
            this.ChkInPut_Tongs_A.Size = new System.Drawing.Size(133, 23);
            this.ChkInPut_Tongs_A.TabIndex = 29;
            this.ChkInPut_Tongs_A.Text = "A抓手回到位";
            this.ChkInPut_Tongs_A.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Tray_In
            // 
            this.ChkInPut_Tray_In.AutoSize = true;
            this.ChkInPut_Tray_In.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Tray_In.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Tray_In.Location = new System.Drawing.Point(526, 288);
            this.ChkInPut_Tray_In.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Tray_In.Name = "ChkInPut_Tray_In";
            this.ChkInPut_Tray_In.Size = new System.Drawing.Size(142, 23);
            this.ChkInPut_Tray_In.TabIndex = 23;
            this.ChkInPut_Tray_In.Text = "接液盘回到位";
            this.ChkInPut_Tray_In.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Cylinder_Down
            // 
            this.ChkInPut_Cylinder_Down.AutoSize = true;
            this.ChkInPut_Cylinder_Down.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Cylinder_Down.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Cylinder_Down.Location = new System.Drawing.Point(199, 224);
            this.ChkInPut_Cylinder_Down.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Cylinder_Down.Name = "ChkInPut_Cylinder_Down";
            this.ChkInPut_Cylinder_Down.Size = new System.Drawing.Size(123, 23);
            this.ChkInPut_Cylinder_Down.TabIndex = 22;
            this.ChkInPut_Cylinder_Down.Text = "气缸下限位";
            this.ChkInPut_Cylinder_Down.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Cylinder_Up
            // 
            this.ChkInPut_Cylinder_Up.AutoSize = true;
            this.ChkInPut_Cylinder_Up.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Cylinder_Up.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Cylinder_Up.Location = new System.Drawing.Point(26, 224);
            this.ChkInPut_Cylinder_Up.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Cylinder_Up.Name = "ChkInPut_Cylinder_Up";
            this.ChkInPut_Cylinder_Up.Size = new System.Drawing.Size(123, 23);
            this.ChkInPut_Cylinder_Up.TabIndex = 18;
            this.ChkInPut_Cylinder_Up.Text = "气缸上限位";
            this.ChkInPut_Cylinder_Up.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Z_Origin
            // 
            this.ChkInPut_Z_Origin.AutoSize = true;
            this.ChkInPut_Z_Origin.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Z_Origin.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Z_Origin.Location = new System.Drawing.Point(199, 160);
            this.ChkInPut_Z_Origin.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Z_Origin.Name = "ChkInPut_Z_Origin";
            this.ChkInPut_Z_Origin.Size = new System.Drawing.Size(114, 23);
            this.ChkInPut_Z_Origin.TabIndex = 13;
            this.ChkInPut_Z_Origin.Text = "Z轴原点位";
            this.ChkInPut_Z_Origin.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Z_Corotation
            // 
            this.ChkInPut_Z_Corotation.AutoSize = true;
            this.ChkInPut_Z_Corotation.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Z_Corotation.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Z_Corotation.Location = new System.Drawing.Point(26, 160);
            this.ChkInPut_Z_Corotation.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Z_Corotation.Name = "ChkInPut_Z_Corotation";
            this.ChkInPut_Z_Corotation.Size = new System.Drawing.Size(114, 23);
            this.ChkInPut_Z_Corotation.TabIndex = 12;
            this.ChkInPut_Z_Corotation.Text = "Z轴反限位";
            this.ChkInPut_Z_Corotation.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Y_Origin
            // 
            this.ChkInPut_Y_Origin.AutoSize = true;
            this.ChkInPut_Y_Origin.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Y_Origin.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Y_Origin.Location = new System.Drawing.Point(526, 160);
            this.ChkInPut_Y_Origin.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Y_Origin.Name = "ChkInPut_Y_Origin";
            this.ChkInPut_Y_Origin.Size = new System.Drawing.Size(114, 23);
            this.ChkInPut_Y_Origin.TabIndex = 10;
            this.ChkInPut_Y_Origin.Text = "Y轴原点位";
            this.ChkInPut_Y_Origin.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Y_Alarm
            // 
            this.ChkInPut_Y_Alarm.AutoSize = true;
            this.ChkInPut_Y_Alarm.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Y_Alarm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Y_Alarm.Location = new System.Drawing.Point(526, 96);
            this.ChkInPut_Y_Alarm.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Y_Alarm.Name = "ChkInPut_Y_Alarm";
            this.ChkInPut_Y_Alarm.Size = new System.Drawing.Size(114, 23);
            this.ChkInPut_Y_Alarm.TabIndex = 9;
            this.ChkInPut_Y_Alarm.Text = "Y轴报警位";
            this.ChkInPut_Y_Alarm.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Y_Reverse
            // 
            this.ChkInPut_Y_Reverse.AutoSize = true;
            this.ChkInPut_Y_Reverse.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Y_Reverse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Y_Reverse.Location = new System.Drawing.Point(199, 96);
            this.ChkInPut_Y_Reverse.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Y_Reverse.Name = "ChkInPut_Y_Reverse";
            this.ChkInPut_Y_Reverse.Size = new System.Drawing.Size(114, 23);
            this.ChkInPut_Y_Reverse.TabIndex = 7;
            this.ChkInPut_Y_Reverse.Text = "Y轴反限位";
            this.ChkInPut_Y_Reverse.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_Y_Corotation
            // 
            this.ChkInPut_Y_Corotation.AutoSize = true;
            this.ChkInPut_Y_Corotation.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_Y_Corotation.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_Y_Corotation.Location = new System.Drawing.Point(26, 96);
            this.ChkInPut_Y_Corotation.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_Y_Corotation.Name = "ChkInPut_Y_Corotation";
            this.ChkInPut_Y_Corotation.Size = new System.Drawing.Size(114, 23);
            this.ChkInPut_Y_Corotation.TabIndex = 6;
            this.ChkInPut_Y_Corotation.Text = "Y轴正限位";
            this.ChkInPut_Y_Corotation.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_X_Origin
            // 
            this.ChkInPut_X_Origin.AutoSize = true;
            this.ChkInPut_X_Origin.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_X_Origin.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_X_Origin.Location = new System.Drawing.Point(372, 160);
            this.ChkInPut_X_Origin.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_X_Origin.Name = "ChkInPut_X_Origin";
            this.ChkInPut_X_Origin.Size = new System.Drawing.Size(114, 23);
            this.ChkInPut_X_Origin.TabIndex = 4;
            this.ChkInPut_X_Origin.Text = "X轴原点位";
            this.ChkInPut_X_Origin.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_X_Alarm
            // 
            this.ChkInPut_X_Alarm.AutoSize = true;
            this.ChkInPut_X_Alarm.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_X_Alarm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_X_Alarm.Location = new System.Drawing.Point(526, 32);
            this.ChkInPut_X_Alarm.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_X_Alarm.Name = "ChkInPut_X_Alarm";
            this.ChkInPut_X_Alarm.Size = new System.Drawing.Size(114, 23);
            this.ChkInPut_X_Alarm.TabIndex = 3;
            this.ChkInPut_X_Alarm.Text = "X轴报警位";
            this.ChkInPut_X_Alarm.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_X_Ready
            // 
            this.ChkInPut_X_Ready.AutoSize = true;
            this.ChkInPut_X_Ready.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_X_Ready.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_X_Ready.Location = new System.Drawing.Point(372, 32);
            this.ChkInPut_X_Ready.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_X_Ready.Name = "ChkInPut_X_Ready";
            this.ChkInPut_X_Ready.Size = new System.Drawing.Size(114, 23);
            this.ChkInPut_X_Ready.TabIndex = 2;
            this.ChkInPut_X_Ready.Text = "X轴准备位";
            this.ChkInPut_X_Ready.UseVisualStyleBackColor = true;
            // 
            // ChkInPut_X_Reverse
            // 
            this.ChkInPut_X_Reverse.AutoSize = true;
            this.ChkInPut_X_Reverse.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_X_Reverse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_X_Reverse.Location = new System.Drawing.Point(199, 32);
            this.ChkInPut_X_Reverse.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_X_Reverse.Name = "ChkInPut_X_Reverse";
            this.ChkInPut_X_Reverse.Size = new System.Drawing.Size(114, 23);
            this.ChkInPut_X_Reverse.TabIndex = 1;
            this.ChkInPut_X_Reverse.Text = "X轴反限位";
            this.ChkInPut_X_Reverse.UseVisualStyleBackColor = true;
            // 
            // grp_in
            // 
            this.grp_in.Controls.Add(this.ChkInPut_SupportCover);
            this.grp_in.Controls.Add(this.ChkInPut_Back);
            this.grp_in.Controls.Add(this.ChkInPut_Slow_Cylinder_Mid);
            this.grp_in.Controls.Add(this.ChkInPut_Cylinder_Block);
            this.grp_in.Controls.Add(this.ChkInPut_Apocenosis_Up);
            this.grp_in.Controls.Add(this.ChkInPut_Decompression_Up);
            this.grp_in.Controls.Add(this.ChkInPut_Block_Out);
            this.grp_in.Controls.Add(this.ChkInPut_Block_In);
            this.grp_in.Controls.Add(this.ChkInPut_Decompression_Down);
            this.grp_in.Controls.Add(this.ChkInPut_Cylinder_Mid);
            this.grp_in.Controls.Add(this.ChkInPut_Y_Ready);
            this.grp_in.Controls.Add(this.ChkInPut_Tray_Out);
            this.grp_in.Controls.Add(this.ChkInPut_Stop);
            this.grp_in.Controls.Add(this.ChkInPut_Sunx_B);
            this.grp_in.Controls.Add(this.ChkInPut_Sunx_A);
            this.grp_in.Controls.Add(this.ChkInPut_Syringe);
            this.grp_in.Controls.Add(this.ChkInPut_Tongs_B);
            this.grp_in.Controls.Add(this.ChkInPut_Tongs_A);
            this.grp_in.Controls.Add(this.ChkInPut_Tray_In);
            this.grp_in.Controls.Add(this.ChkInPut_Cylinder_Down);
            this.grp_in.Controls.Add(this.ChkInPut_Cylinder_Up);
            this.grp_in.Controls.Add(this.ChkInPut_Z_Origin);
            this.grp_in.Controls.Add(this.ChkInPut_Z_Corotation);
            this.grp_in.Controls.Add(this.ChkInPut_Y_Origin);
            this.grp_in.Controls.Add(this.ChkInPut_Y_Alarm);
            this.grp_in.Controls.Add(this.ChkInPut_Y_Reverse);
            this.grp_in.Controls.Add(this.ChkInPut_Y_Corotation);
            this.grp_in.Controls.Add(this.ChkInPut_X_Origin);
            this.grp_in.Controls.Add(this.ChkInPut_X_Alarm);
            this.grp_in.Controls.Add(this.ChkInPut_X_Ready);
            this.grp_in.Controls.Add(this.ChkInPut_X_Reverse);
            this.grp_in.Controls.Add(this.ChkInPut_X_Corotation);
            this.grp_in.Font = new System.Drawing.Font("宋体", 14.25F);
            this.grp_in.Location = new System.Drawing.Point(-6, 305);
            this.grp_in.Margin = new System.Windows.Forms.Padding(2);
            this.grp_in.Name = "grp_in";
            this.grp_in.Padding = new System.Windows.Forms.Padding(2);
            this.grp_in.Size = new System.Drawing.Size(680, 625);
            this.grp_in.TabIndex = 6;
            this.grp_in.TabStop = false;
            this.grp_in.Text = "输入监控";
            // 
            // ChkInPut_X_Corotation
            // 
            this.ChkInPut_X_Corotation.AutoSize = true;
            this.ChkInPut_X_Corotation.Font = new System.Drawing.Font("宋体", 14.25F);
            this.ChkInPut_X_Corotation.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChkInPut_X_Corotation.Location = new System.Drawing.Point(26, 32);
            this.ChkInPut_X_Corotation.Margin = new System.Windows.Forms.Padding(2);
            this.ChkInPut_X_Corotation.Name = "ChkInPut_X_Corotation";
            this.ChkInPut_X_Corotation.Size = new System.Drawing.Size(114, 23);
            this.ChkInPut_X_Corotation.TabIndex = 0;
            this.ChkInPut_X_Corotation.Text = "X轴正限位";
            this.ChkInPut_X_Corotation.UseVisualStyleBackColor = true;
            // 
            // Btn_Reset
            // 
            this.Btn_Reset.Font = new System.Drawing.Font("宋体", 14.25F);
            this.Btn_Reset.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Btn_Reset.Location = new System.Drawing.Point(42, 230);
            this.Btn_Reset.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_Reset.Name = "Btn_Reset";
            this.Btn_Reset.Size = new System.Drawing.Size(146, 60);
            this.Btn_Reset.TabIndex = 5;
            this.Btn_Reset.Text = "清 零";
            this.Btn_Reset.UseVisualStyleBackColor = true;
            this.Btn_Reset.Click += new System.EventHandler(this.Btn_Reset_Click);
            // 
            // LabBalanceValue
            // 
            this.LabBalanceValue.Font = new System.Drawing.Font("宋体", 14.25F);
            this.LabBalanceValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabBalanceValue.Location = new System.Drawing.Point(13, 24);
            this.LabBalanceValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabBalanceValue.Name = "LabBalanceValue";
            this.LabBalanceValue.Size = new System.Drawing.Size(198, 135);
            this.LabBalanceValue.TabIndex = 2;
            this.LabBalanceValue.Text = "0.00";
            this.LabBalanceValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Btn_Reset);
            this.groupBox2.Controls.Add(this.LabBalanceValue);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 14.25F);
            this.groupBox2.Location = new System.Drawing.Point(1661, 1);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(223, 300);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "天平监控";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TxtCylinderEncoder);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 14.25F);
            this.groupBox1.Location = new System.Drawing.Point(1426, 1);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(223, 300);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "气缸编码器监控";
            // 
            // TxtCylinderEncoder
            // 
            this.TxtCylinderEncoder.Font = new System.Drawing.Font("宋体", 14.25F);
            this.TxtCylinderEncoder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.TxtCylinderEncoder.Location = new System.Drawing.Point(3, 24);
            this.TxtCylinderEncoder.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TxtCylinderEncoder.Name = "TxtCylinderEncoder";
            this.TxtCylinderEncoder.Size = new System.Drawing.Size(216, 274);
            this.TxtCylinderEncoder.TabIndex = 2;
            this.TxtCylinderEncoder.Text = "0";
            this.TxtCylinderEncoder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Debug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1889, 931);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grp_out);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.grp_move);
            this.Controls.Add(this.grp_in);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "Debug";
            this.Text = "调试页面";
            this.grp_out.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.grp_move.ResumeLayout(false);
            this.grp_move.PerformLayout();
            this.grp_in.ResumeLayout(false);
            this.grp_in.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnOutPut_Slow;
        private System.Windows.Forms.Button BtnOutPut_ResetX;
        private System.Windows.Forms.CheckBox ChkInPut_SupportCover;
        private System.Windows.Forms.CheckBox ChkInPut_Back;
        private System.Windows.Forms.CheckBox ChkInPut_Slow_Cylinder_Mid;
        private System.Windows.Forms.Button BtnOutPut_ResetY;
        private System.Windows.Forms.Button BtnOutPut_Block_Out;
        private System.Windows.Forms.CheckBox ChkInPut_Cylinder_Block;
        private System.Windows.Forms.CheckBox ChkInPut_Apocenosis_Up;
        private System.Windows.Forms.CheckBox ChkInPut_Decompression_Up;
        private System.Windows.Forms.CheckBox ChkInPut_Block_Out;
        private System.Windows.Forms.CheckBox ChkInPut_Block_In;
        private System.Windows.Forms.CheckBox ChkInPut_Decompression_Down;
        private System.Windows.Forms.CheckBox ChkInPut_Cylinder_Mid;
        private System.Windows.Forms.CheckBox ChkInPut_Y_Ready;
        private System.Windows.Forms.CheckBox ChkInPut_Tray_Out;
        private System.Windows.Forms.CheckBox ChkInPut_Stop;
        private System.Windows.Forms.CheckBox ChkInPut_Sunx_B;
        private System.Windows.Forms.CheckBox ChkInPut_Sunx_A;
        private System.Windows.Forms.CheckBox ChkInPut_Syringe;
        private System.Windows.Forms.Button BtnOutPut_Wash_Blow;
        private System.Windows.Forms.Button BtnOutPut_Wash_Out;
        private System.Windows.Forms.Button BtnOutPut_Wash_In;
        private System.Windows.Forms.Button BtnOutPut_Block_Cylinder;
        private System.Windows.Forms.Button BtnOutPut_Decompression;
        private System.Windows.Forms.GroupBox grp_out;
        private System.Windows.Forms.Button BtnOutPut_Buzzer;
        private System.Windows.Forms.Button BtnOutPut_Tray;
        private System.Windows.Forms.Button BtnOutPut_Water;
        private System.Windows.Forms.Button BtnOutPut_Waste;
        private System.Windows.Forms.Button BtnOutPut_Blender;
        private System.Windows.Forms.Button BtnOutPut_Cylinder_Up;
        private System.Windows.Forms.Button BtnOutPut_TongsOn;
        private System.Windows.Forms.Button BtnOutPut_Y_Power;
        private System.Windows.Forms.Button BtnOutPut_X_Power;
        private System.Windows.Forms.Timer Tmr;
        private System.Windows.Forms.RadioButton RdoWash;
        private System.Windows.Forms.Button BtnGenerate;
        private System.Windows.Forms.Button BtnWrite;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton RdoWetClamp;
        private System.Windows.Forms.RadioButton RdoDryClamp;
        private System.Windows.Forms.RadioButton RdoWetCloth;
        private System.Windows.Forms.RadioButton RdoDryCloth;
        private System.Windows.Forms.RadioButton RdoDecompression;
        private System.Windows.Forms.RadioButton RdoCupLid;
        private System.Windows.Forms.RadioButton RdoBalance;
        private System.Windows.Forms.RadioButton RdoCup;
        private System.Windows.Forms.RadioButton RdoBottle;
        private System.Windows.Forms.Button BtnStartMove;
        private System.Windows.Forms.TextBox TxtNum;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox ChkInPut_Tongs_B;
        private System.Windows.Forms.Button BtnStop;
        private System.Windows.Forms.TextBox TxtRPosY;
        private System.Windows.Forms.TextBox TxtRPosX;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton RdoZ;
        private System.Windows.Forms.RadioButton RdoY;
        private System.Windows.Forms.RadioButton RdoX;
        private System.Windows.Forms.TextBox TxtCPosZ;
        private System.Windows.Forms.TextBox TxtCSpeedZ;
        private System.Windows.Forms.TextBox TxtUpSpeedZ;
        private System.Windows.Forms.TextBox TxtHSpeedZ;
        private System.Windows.Forms.TextBox TxtLSpeedZ;
        private System.Windows.Forms.TextBox TxtPulseZ;
        private System.Windows.Forms.TextBox TxtCSpeedY;
        private System.Windows.Forms.TextBox TxtUpSpeedY;
        private System.Windows.Forms.TextBox TxtHSpeedY;
        private System.Windows.Forms.TextBox TxtLSpeedY;
        private System.Windows.Forms.TextBox TxtPulseY;
        private System.Windows.Forms.TextBox TxtCSpeedX;
        private System.Windows.Forms.TextBox TxtUpSpeedX;
        private System.Windows.Forms.TextBox TxtHSpeedX;
        private System.Windows.Forms.TextBox TxtLSpeedX;
        private System.Windows.Forms.TextBox TxtPulseX;
        private System.Windows.Forms.Button BtnMove;
        private System.Windows.Forms.GroupBox grp_move;
        private System.Windows.Forms.Button BtnHome;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox ChkInPut_Tongs_A;
        private System.Windows.Forms.CheckBox ChkInPut_Tray_In;
        private System.Windows.Forms.CheckBox ChkInPut_Cylinder_Down;
        private System.Windows.Forms.CheckBox ChkInPut_Cylinder_Up;
        private System.Windows.Forms.CheckBox ChkInPut_Z_Origin;
        private System.Windows.Forms.CheckBox ChkInPut_Z_Corotation;
        private System.Windows.Forms.CheckBox ChkInPut_Y_Origin;
        private System.Windows.Forms.CheckBox ChkInPut_Y_Alarm;
        private System.Windows.Forms.CheckBox ChkInPut_Y_Reverse;
        private System.Windows.Forms.CheckBox ChkInPut_Y_Corotation;
        private System.Windows.Forms.CheckBox ChkInPut_X_Origin;
        private System.Windows.Forms.CheckBox ChkInPut_X_Alarm;
        private System.Windows.Forms.CheckBox ChkInPut_X_Ready;
        private System.Windows.Forms.CheckBox ChkInPut_X_Reverse;
        private System.Windows.Forms.GroupBox grp_in;
        private System.Windows.Forms.CheckBox ChkInPut_X_Corotation;
        private System.Windows.Forms.Button Btn_Reset;
        private System.Windows.Forms.Label LabBalanceValue;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label TxtCylinderEncoder;
    }
}