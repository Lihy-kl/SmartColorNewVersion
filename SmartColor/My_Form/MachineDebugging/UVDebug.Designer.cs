namespace SmartColor.My_Form.MachineDebugging
{
    partial class UVDebug
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
            this.grp_move = new System.Windows.Forms.GroupBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtUpTimeY = new System.Windows.Forms.TextBox();
            this.TxtHSpeedY = new System.Windows.Forms.TextBox();
            this.TxtLSpeedY = new System.Windows.Forms.TextBox();
            this.TxtUpTimeX = new System.Windows.Forms.TextBox();
            this.TxtHSpeedX = new System.Windows.Forms.TextBox();
            this.TxtLSpeedX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Tmr = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.button17 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button19 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.button20 = new System.Windows.Forms.Button();
            this.button21 = new System.Windows.Forms.Button();
            this.button22 = new System.Windows.Forms.Button();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.button23 = new System.Windows.Forms.Button();
            this.button24 = new System.Windows.Forms.Button();
            this.button25 = new System.Windows.Forms.Button();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.grp_move.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // grp_move
            // 
            this.grp_move.Controls.Add(this.checkBox2);
            this.grp_move.Controls.Add(this.checkBox1);
            this.grp_move.Controls.Add(this.label12);
            this.grp_move.Controls.Add(this.button8);
            this.grp_move.Controls.Add(this.button7);
            this.grp_move.Controls.Add(this.label11);
            this.grp_move.Controls.Add(this.button12);
            this.grp_move.Controls.Add(this.button13);
            this.grp_move.Controls.Add(this.button11);
            this.grp_move.Controls.Add(this.button10);
            this.grp_move.Controls.Add(this.label10);
            this.grp_move.Controls.Add(this.button6);
            this.grp_move.Controls.Add(this.button5);
            this.grp_move.Controls.Add(this.button4);
            this.grp_move.Controls.Add(this.button3);
            this.grp_move.Controls.Add(this.button2);
            this.grp_move.Controls.Add(this.button1);
            this.grp_move.Controls.Add(this.label9);
            this.grp_move.Controls.Add(this.label8);
            this.grp_move.Controls.Add(this.label7);
            this.grp_move.Controls.Add(this.label6);
            this.grp_move.Controls.Add(this.label2);
            this.grp_move.Controls.Add(this.TxtUpTimeY);
            this.grp_move.Controls.Add(this.TxtHSpeedY);
            this.grp_move.Controls.Add(this.TxtLSpeedY);
            this.grp_move.Controls.Add(this.TxtUpTimeX);
            this.grp_move.Controls.Add(this.TxtHSpeedX);
            this.grp_move.Controls.Add(this.TxtLSpeedX);
            this.grp_move.Controls.Add(this.label5);
            this.grp_move.Controls.Add(this.label4);
            this.grp_move.Controls.Add(this.label3);
            this.grp_move.Controls.Add(this.label1);
            this.grp_move.Font = new System.Drawing.Font("宋体", 14.25F);
            this.grp_move.Location = new System.Drawing.Point(-6, 1);
            this.grp_move.Margin = new System.Windows.Forms.Padding(2);
            this.grp_move.Name = "grp_move";
            this.grp_move.Padding = new System.Windows.Forms.Padding(2);
            this.grp_move.Size = new System.Drawing.Size(1884, 300);
            this.grp_move.TabIndex = 9;
            this.grp_move.TabStop = false;
            this.grp_move.Text = "分光光度计";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Enabled = false;
            this.checkBox2.Font = new System.Drawing.Font("宋体", 21.75F);
            this.checkBox2.Location = new System.Drawing.Point(1739, 231);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(119, 33);
            this.checkBox2.TabIndex = 50;
            this.checkBox2.Text = "出到位";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox1.Location = new System.Drawing.Point(1739, 139);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(119, 33);
            this.checkBox1.TabIndex = 49;
            this.checkBox1.Text = "出到位";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(1748, 49);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 29);
            this.label12.TabIndex = 48;
            this.label12.Text = "传感器";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button8
            // 
            this.button8.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button8.Location = new System.Drawing.Point(1541, 227);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(139, 41);
            this.button8.TabIndex = 47;
            this.button8.Text = "伸出";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button7.Location = new System.Drawing.Point(1541, 135);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(139, 41);
            this.button7.TabIndex = 46;
            this.button7.Text = "伸出";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(1575, 49);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 29);
            this.label11.TabIndex = 45;
            this.label11.Text = "气缸";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button12
            // 
            this.button12.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button12.Location = new System.Drawing.Point(1426, 227);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(91, 41);
            this.button12.TabIndex = 44;
            this.button12.Text = "反转";
            this.button12.UseVisualStyleBackColor = true;
            // 
            // button13
            // 
            this.button13.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button13.Location = new System.Drawing.Point(1335, 227);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(91, 41);
            this.button13.TabIndex = 43;
            this.button13.Text = "正转";
            this.button13.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            this.button11.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button11.Location = new System.Drawing.Point(1426, 135);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(91, 41);
            this.button11.TabIndex = 42;
            this.button11.Text = "反转";
            this.button11.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            this.button10.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button10.Location = new System.Drawing.Point(1335, 135);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(91, 41);
            this.button10.TabIndex = 41;
            this.button10.Text = "正转";
            this.button10.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label10.Location = new System.Drawing.Point(1373, 49);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 29);
            this.label10.TabIndex = 40;
            this.label10.Text = "蠕动泵";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button6.Location = new System.Drawing.Point(1152, 227);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(139, 41);
            this.button6.TabIndex = 36;
            this.button6.Text = "测UV";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button5.Location = new System.Drawing.Point(1152, 135);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(139, 41);
            this.button5.TabIndex = 35;
            this.button5.Text = "测UV";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button4.Location = new System.Drawing.Point(964, 227);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(139, 41);
            this.button4.TabIndex = 34;
            this.button4.Text = "测基线";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button3.Location = new System.Drawing.Point(964, 135);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(139, 41);
            this.button3.TabIndex = 33;
            this.button3.Text = "测基线";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button2.Location = new System.Drawing.Point(791, 227);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(139, 41);
            this.button2.TabIndex = 32;
            this.button2.Text = "凋零";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button1.Location = new System.Drawing.Point(791, 135);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 41);
            this.button1.TabIndex = 31;
            this.button1.Text = "凋零";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(1171, 49);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 29);
            this.label9.TabIndex = 30;
            this.label9.Text = "吸光度";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(998, 49);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 29);
            this.label8.TabIndex = 29;
            this.label8.Text = "基线";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(825, 49);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 29);
            this.label7.TabIndex = 28;
            this.label7.Text = "凋零";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(46, 233);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 29);
            this.label6.TabIndex = 27;
            this.label6.Text = "2#";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(46, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 29);
            this.label2.TabIndex = 26;
            this.label2.Text = "1#";
            // 
            // TxtUpTimeY
            // 
            this.TxtUpTimeY.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtUpTimeY.Location = new System.Drawing.Point(604, 227);
            this.TxtUpTimeY.Margin = new System.Windows.Forms.Padding(2);
            this.TxtUpTimeY.Name = "TxtUpTimeY";
            this.TxtUpTimeY.ReadOnly = true;
            this.TxtUpTimeY.Size = new System.Drawing.Size(139, 41);
            this.TxtUpTimeY.TabIndex = 25;
            this.TxtUpTimeY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtHSpeedY
            // 
            this.TxtHSpeedY.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtHSpeedY.Location = new System.Drawing.Point(416, 227);
            this.TxtHSpeedY.Margin = new System.Windows.Forms.Padding(2);
            this.TxtHSpeedY.Name = "TxtHSpeedY";
            this.TxtHSpeedY.ReadOnly = true;
            this.TxtHSpeedY.Size = new System.Drawing.Size(139, 41);
            this.TxtHSpeedY.TabIndex = 24;
            this.TxtHSpeedY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtLSpeedY
            // 
            this.TxtLSpeedY.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtLSpeedY.Location = new System.Drawing.Point(214, 227);
            this.TxtLSpeedY.Margin = new System.Windows.Forms.Padding(2);
            this.TxtLSpeedY.Name = "TxtLSpeedY";
            this.TxtLSpeedY.ReadOnly = true;
            this.TxtLSpeedY.Size = new System.Drawing.Size(139, 41);
            this.TxtLSpeedY.TabIndex = 23;
            this.TxtLSpeedY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtUpTimeX
            // 
            this.TxtUpTimeX.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtUpTimeX.Location = new System.Drawing.Point(604, 135);
            this.TxtUpTimeX.Margin = new System.Windows.Forms.Padding(2);
            this.TxtUpTimeX.Name = "TxtUpTimeX";
            this.TxtUpTimeX.ReadOnly = true;
            this.TxtUpTimeX.Size = new System.Drawing.Size(139, 41);
            this.TxtUpTimeX.TabIndex = 19;
            this.TxtUpTimeX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtHSpeedX
            // 
            this.TxtHSpeedX.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtHSpeedX.Location = new System.Drawing.Point(416, 135);
            this.TxtHSpeedX.Margin = new System.Windows.Forms.Padding(2);
            this.TxtHSpeedX.Name = "TxtHSpeedX";
            this.TxtHSpeedX.ReadOnly = true;
            this.TxtHSpeedX.Size = new System.Drawing.Size(139, 41);
            this.TxtHSpeedX.TabIndex = 18;
            this.TxtHSpeedX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TxtLSpeedX
            // 
            this.TxtLSpeedX.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtLSpeedX.Location = new System.Drawing.Point(214, 135);
            this.TxtLSpeedX.Margin = new System.Windows.Forms.Padding(2);
            this.TxtLSpeedX.Name = "TxtLSpeedX";
            this.TxtLSpeedX.ReadOnly = true;
            this.TxtLSpeedX.Size = new System.Drawing.Size(139, 41);
            this.TxtLSpeedX.TabIndex = 17;
            this.TxtLSpeedX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(623, 49);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 29);
            this.label5.TabIndex = 10;
            this.label5.Text = "状态码";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(450, 49);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 29);
            this.label4.TabIndex = 9;
            this.label4.Text = "节拍";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(219, 49);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 29);
            this.label3.TabIndex = 8;
            this.label3.Text = "当前状态";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(17, 49);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 29);
            this.label1.TabIndex = 6;
            this.label1.Text = "比色皿";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Tmr
            // 
            this.Tmr.Enabled = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.groupBox6);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Location = new System.Drawing.Point(3, 306);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1874, 622);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "转子缸";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.button17);
            this.groupBox6.Location = new System.Drawing.Point(13, 556);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(1846, 55);
            this.groupBox6.TabIndex = 52;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "共用部分";
            // 
            // button17
            // 
            this.button17.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button17.Location = new System.Drawing.Point(727, 14);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(258, 37);
            this.button17.TabIndex = 51;
            this.button17.Text = "总排水";
            this.button17.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Controls.Add(this.button19);
            this.groupBox3.Controls.Add(this.button18);
            this.groupBox3.Controls.Add(this.checkBox4);
            this.groupBox3.Controls.Add(this.checkBox3);
            this.groupBox3.Location = new System.Drawing.Point(13, 34);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1846, 126);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "一工位";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.button9);
            this.panel1.Location = new System.Drawing.Point(1424, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(416, 101);
            this.panel1.TabIndex = 54;
            // 
            // button19
            // 
            this.button19.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button19.Location = new System.Drawing.Point(1025, 49);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(258, 41);
            this.button19.TabIndex = 53;
            this.button19.Text = "排液";
            this.button19.UseVisualStyleBackColor = true;
            // 
            // button18
            // 
            this.button18.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button18.Location = new System.Drawing.Point(626, 49);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(258, 41);
            this.button18.TabIndex = 52;
            this.button18.Text = "开盖";
            this.button18.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            this.button9.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button9.Location = new System.Drawing.Point(318, 3);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(95, 95);
            this.button9.TabIndex = 51;
            this.button9.Text = "搅拌";
            this.button9.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox4.Location = new System.Drawing.Point(337, 53);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(148, 33);
            this.checkBox4.TabIndex = 1;
            this.checkBox4.Text = "关盖到位";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox3.Location = new System.Drawing.Point(48, 53);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(148, 33);
            this.checkBox3.TabIndex = 0;
            this.checkBox3.Text = "开盖到位";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(2, 12);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(158, 29);
            this.label13.TabIndex = 51;
            this.label13.Text = "搅拌速度：";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label14.Location = new System.Drawing.Point(2, 63);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(158, 29);
            this.label14.TabIndex = 52;
            this.label14.Text = "当前电流：";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(148, 6);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(139, 41);
            this.textBox1.TabIndex = 51;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox2.Location = new System.Drawing.Point(148, 57);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(139, 41);
            this.textBox2.TabIndex = 53;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel2);
            this.groupBox2.Controls.Add(this.button15);
            this.groupBox2.Controls.Add(this.button16);
            this.groupBox2.Controls.Add(this.checkBox5);
            this.groupBox2.Controls.Add(this.checkBox6);
            this.groupBox2.Location = new System.Drawing.Point(13, 166);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1846, 126);
            this.groupBox2.TabIndex = 53;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "二工位";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.textBox3);
            this.panel2.Controls.Add(this.textBox4);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.button14);
            this.panel2.Location = new System.Drawing.Point(1424, 19);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(416, 101);
            this.panel2.TabIndex = 54;
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox3.Location = new System.Drawing.Point(148, 57);
            this.textBox3.Margin = new System.Windows.Forms.Padding(2);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(139, 41);
            this.textBox3.TabIndex = 53;
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox4
            // 
            this.textBox4.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox4.Location = new System.Drawing.Point(148, 6);
            this.textBox4.Margin = new System.Windows.Forms.Padding(2);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(139, 41);
            this.textBox4.TabIndex = 51;
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label15.Location = new System.Drawing.Point(2, 63);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(158, 29);
            this.label15.TabIndex = 52;
            this.label15.Text = "当前电流：";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label16.Location = new System.Drawing.Point(2, 12);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(158, 29);
            this.label16.TabIndex = 51;
            this.label16.Text = "搅拌速度：";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button14
            // 
            this.button14.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button14.Location = new System.Drawing.Point(318, 3);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(95, 95);
            this.button14.TabIndex = 51;
            this.button14.Text = "搅拌";
            this.button14.UseVisualStyleBackColor = true;
            // 
            // button15
            // 
            this.button15.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button15.Location = new System.Drawing.Point(1025, 49);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(258, 41);
            this.button15.TabIndex = 53;
            this.button15.Text = "排液";
            this.button15.UseVisualStyleBackColor = true;
            // 
            // button16
            // 
            this.button16.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button16.Location = new System.Drawing.Point(626, 49);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(258, 41);
            this.button16.TabIndex = 52;
            this.button16.Text = "开盖";
            this.button16.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox5.Location = new System.Drawing.Point(337, 53);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(148, 33);
            this.checkBox5.TabIndex = 1;
            this.checkBox5.Text = "关盖到位";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox6.Location = new System.Drawing.Point(48, 53);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(148, 33);
            this.checkBox6.TabIndex = 0;
            this.checkBox6.Text = "开盖到位";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.panel3);
            this.groupBox4.Controls.Add(this.button21);
            this.groupBox4.Controls.Add(this.button22);
            this.groupBox4.Controls.Add(this.checkBox7);
            this.groupBox4.Controls.Add(this.checkBox8);
            this.groupBox4.Location = new System.Drawing.Point(13, 298);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(1846, 126);
            this.groupBox4.TabIndex = 55;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "三工位";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.textBox5);
            this.panel3.Controls.Add(this.textBox6);
            this.panel3.Controls.Add(this.label17);
            this.panel3.Controls.Add(this.label18);
            this.panel3.Controls.Add(this.button20);
            this.panel3.Location = new System.Drawing.Point(1424, 19);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(416, 101);
            this.panel3.TabIndex = 54;
            // 
            // textBox5
            // 
            this.textBox5.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox5.Location = new System.Drawing.Point(148, 57);
            this.textBox5.Margin = new System.Windows.Forms.Padding(2);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(139, 41);
            this.textBox5.TabIndex = 53;
            this.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox6
            // 
            this.textBox6.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox6.Location = new System.Drawing.Point(148, 6);
            this.textBox6.Margin = new System.Windows.Forms.Padding(2);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(139, 41);
            this.textBox6.TabIndex = 51;
            this.textBox6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label17.Location = new System.Drawing.Point(2, 63);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(158, 29);
            this.label17.TabIndex = 52;
            this.label17.Text = "当前电流：";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label18.Location = new System.Drawing.Point(2, 12);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(158, 29);
            this.label18.TabIndex = 51;
            this.label18.Text = "搅拌速度：";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button20
            // 
            this.button20.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button20.Location = new System.Drawing.Point(318, 3);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(95, 95);
            this.button20.TabIndex = 51;
            this.button20.Text = "搅拌";
            this.button20.UseVisualStyleBackColor = true;
            // 
            // button21
            // 
            this.button21.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button21.Location = new System.Drawing.Point(1025, 49);
            this.button21.Name = "button21";
            this.button21.Size = new System.Drawing.Size(258, 41);
            this.button21.TabIndex = 53;
            this.button21.Text = "排液";
            this.button21.UseVisualStyleBackColor = true;
            // 
            // button22
            // 
            this.button22.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button22.Location = new System.Drawing.Point(626, 49);
            this.button22.Name = "button22";
            this.button22.Size = new System.Drawing.Size(258, 41);
            this.button22.TabIndex = 52;
            this.button22.Text = "开盖";
            this.button22.UseVisualStyleBackColor = true;
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox7.Location = new System.Drawing.Point(337, 53);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(148, 33);
            this.checkBox7.TabIndex = 1;
            this.checkBox7.Text = "关盖到位";
            this.checkBox7.UseVisualStyleBackColor = true;
            // 
            // checkBox8
            // 
            this.checkBox8.AutoSize = true;
            this.checkBox8.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox8.Location = new System.Drawing.Point(48, 53);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(148, 33);
            this.checkBox8.TabIndex = 0;
            this.checkBox8.Text = "开盖到位";
            this.checkBox8.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.panel4);
            this.groupBox5.Controls.Add(this.button24);
            this.groupBox5.Controls.Add(this.button25);
            this.groupBox5.Controls.Add(this.checkBox9);
            this.groupBox5.Controls.Add(this.checkBox10);
            this.groupBox5.Location = new System.Drawing.Point(13, 424);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1846, 126);
            this.groupBox5.TabIndex = 55;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "四工位";
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.textBox7);
            this.panel4.Controls.Add(this.textBox8);
            this.panel4.Controls.Add(this.label19);
            this.panel4.Controls.Add(this.label20);
            this.panel4.Controls.Add(this.button23);
            this.panel4.Location = new System.Drawing.Point(1424, 19);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(416, 101);
            this.panel4.TabIndex = 54;
            // 
            // textBox7
            // 
            this.textBox7.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox7.Location = new System.Drawing.Point(148, 57);
            this.textBox7.Margin = new System.Windows.Forms.Padding(2);
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(139, 41);
            this.textBox7.TabIndex = 53;
            this.textBox7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox8
            // 
            this.textBox8.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox8.Location = new System.Drawing.Point(148, 6);
            this.textBox8.Margin = new System.Windows.Forms.Padding(2);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(139, 41);
            this.textBox8.TabIndex = 51;
            this.textBox8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label19.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label19.Location = new System.Drawing.Point(2, 63);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(158, 29);
            this.label19.TabIndex = 52;
            this.label19.Text = "当前电流：";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label20.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label20.Location = new System.Drawing.Point(2, 12);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(158, 29);
            this.label20.TabIndex = 51;
            this.label20.Text = "搅拌速度：";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button23
            // 
            this.button23.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button23.Location = new System.Drawing.Point(318, 3);
            this.button23.Name = "button23";
            this.button23.Size = new System.Drawing.Size(95, 95);
            this.button23.TabIndex = 51;
            this.button23.Text = "搅拌";
            this.button23.UseVisualStyleBackColor = true;
            // 
            // button24
            // 
            this.button24.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button24.Location = new System.Drawing.Point(1025, 49);
            this.button24.Name = "button24";
            this.button24.Size = new System.Drawing.Size(258, 41);
            this.button24.TabIndex = 53;
            this.button24.Text = "排液";
            this.button24.UseVisualStyleBackColor = true;
            // 
            // button25
            // 
            this.button25.Font = new System.Drawing.Font("宋体", 21.75F);
            this.button25.Location = new System.Drawing.Point(626, 49);
            this.button25.Name = "button25";
            this.button25.Size = new System.Drawing.Size(258, 41);
            this.button25.TabIndex = 52;
            this.button25.Text = "开盖";
            this.button25.UseVisualStyleBackColor = true;
            // 
            // checkBox9
            // 
            this.checkBox9.AutoSize = true;
            this.checkBox9.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox9.Location = new System.Drawing.Point(337, 53);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(148, 33);
            this.checkBox9.TabIndex = 1;
            this.checkBox9.Text = "关盖到位";
            this.checkBox9.UseVisualStyleBackColor = true;
            // 
            // checkBox10
            // 
            this.checkBox10.AutoSize = true;
            this.checkBox10.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox10.Location = new System.Drawing.Point(48, 53);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(148, 33);
            this.checkBox10.TabIndex = 0;
            this.checkBox10.Text = "开盖到位";
            this.checkBox10.UseVisualStyleBackColor = true;
            // 
            // UVDebug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1889, 931);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grp_move);
            this.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "UVDebug";
            this.Text = "UV调试页面";
            this.grp_move.ResumeLayout(false);
            this.grp_move.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox grp_move;
        private System.Windows.Forms.Timer Tmr;
        private System.Windows.Forms.TextBox TxtUpTimeY;
        private System.Windows.Forms.TextBox TxtHSpeedY;
        private System.Windows.Forms.TextBox TxtLSpeedY;
        private System.Windows.Forms.TextBox TxtUpTimeX;
        private System.Windows.Forms.TextBox TxtHSpeedX;
        private System.Windows.Forms.TextBox TxtLSpeedX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button19;
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Button button23;
        private System.Windows.Forms.Button button24;
        private System.Windows.Forms.Button button25;
        private System.Windows.Forms.CheckBox checkBox9;
        private System.Windows.Forms.CheckBox checkBox10;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button button20;
        private System.Windows.Forms.Button button21;
        private System.Windows.Forms.Button button22;
        private System.Windows.Forms.CheckBox checkBox7;
        private System.Windows.Forms.CheckBox checkBox8;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox6;
    }
}