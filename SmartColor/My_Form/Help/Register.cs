using com.google.zxing;
using com.google.zxing.common;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Form.Help
{
    public partial class Register : Form
    {
        public static  string _s_mNum;

        public Register()
        {
            InitializeComponent();
            My_Tool.MyRegister.MyReg = new My_Tool.MyRegister();
        }

        /// <summary>
        /// 检查当前机器是否已注册
        /// </summary>
        public static  bool IsRegistered()
        {
            try
            {
                using (RegistryKey retkey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true)
                    .CreateSubKey("mySoftWare")
                    .CreateSubKey("Register.INI"))
                {
                    foreach (string strRNum in retkey.GetSubKeyNames())
                    {
                        if (strRNum == My_Tool.MyRegister.MyReg.GetRNum())
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
                // 忽略异常，视为未注册
            }
            return false;
        }


        /// <summary>
        /// 注册按钮点击事件，处理注册码校验和注册逻辑
        /// </summary>
        private void BtnReg_Click(object sender, EventArgs e)
        {
            try
            {
                // 完全匹配注册码
                if (txtLicence.Text == My_Tool.MyRegister.MyReg.GetRNum())
                {
                    My_File.LocalTranslator.ShowMessage("注册成功！重启软件后生效！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 写入注册表，标记已注册
                    using (RegistryKey retkey = Registry.CurrentUser.OpenSubKey("Software", true)
                        .CreateSubKey("chec")
                        .CreateSubKey("Register.INI")
                        .CreateSubKey(My_Tool.MyRegister.MyReg.GetRNum()))
                    {
                        retkey.SetValue("UserName", "Rsoft");
                    }
                    this.Close();
                }
                else
                {
                    // 检查注册码长度
                    if (txtLicence.Text.Length == 32)
                    {
                        string s_head = txtLicence.Text.Substring(0, 4);
                        if (s_head != "GZKL")
                        {
                            My_File.LocalTranslator.ShowMessage("注册码错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtLicence.SelectAll();
                        }
                        else
                        {
                            try
                            {
                                // 解析日期信息
                                string s_year = Convert.ToInt32(txtLicence.Text.Substring(7, 4), 16).ToString();
                                string s_month = Convert.ToInt32(txtLicence.Text.Substring(14, 2), 16).ToString();
                                string s_dayNow = Convert.ToInt32(txtLicence.Text.Substring(22, 2), 16).ToString();
                                string s_date = $"{s_year}/{s_month}/{s_dayNow}";
                                string s_dateNow = DateTime.Now.Date.ToString("yyyy/M/d");
                                string s_temp = txtLicence.Text.Substring(24, 8);

                                // 校验日期和动态注册码
                                if (s_dateNow == s_date && s_temp == My_Tool.MyRegister.MyReg.GetRNumDS())
                                {
                                    // 解析允许天数
                                    string s_day = $"{txtLicence.Text[5]}{txtLicence.Text[11]}{txtLicence.Text[18]}";
                                    int i_day = Convert.ToInt16(s_day);

                                    // 加密当前时间和天数，写入注册表
                                    string s_enNow = My_Tool.AES.AesEncrypt(DateTime.Now.ToString());
                                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\chec", "CreateDateTime", s_enNow, RegistryValueKind.String);

                                    string s_enP_int_day = My_Tool.AES.AesEncrypt(i_day.ToString());
                                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\chec", "UseTimes", s_enP_int_day, RegistryValueKind.String);

                                    My_File.LocalTranslator.ShowMessage($"试用期延长{i_day}天成功！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    this.Close();
                                }
                                else
                                {
                                    My_File.LocalTranslator.ShowMessage("注册码错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    txtLicence.SelectAll();
                                }
                            }
                            catch
                            {
                                My_File.LocalTranslator.ShowMessage("注册码错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txtLicence.SelectAll();
                            }
                        }
                    }
                    else
                    {
                        My_File.LocalTranslator.ShowMessage("注册码错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtLicence.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                // 建议直接提示用户错误信息，避免抛出新异常丢失堆栈
                My_File.LocalTranslator.ShowMessage("注册过程发生异常：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 窗体加载事件，初始化界面和注册状态
        /// </summary>
        private async void Register_Load(object sender, EventArgs e)
        {
            // 根据注册方式调整界面
            if (My_ConPar.Machine.RegisterWay == 0)
            {
                this.pictureBox2.Visible = false;
                this.button1.Visible = false;
                this.label2.Visible = true;
                this.txtLicence.Visible = true;
                this.btnReg.Visible = true;
            }

            _s_mNum = My_Tool.MyRegister.MyReg.GetMNum();

            // 检查注册状态
            // 使用静态方法检查注册状态
            if (IsRegistered())
            {
                this.labRegInfo.Text = My_ConPar.Machine.Language == 0 ? "已注册" : "Registered";
                this.btnReg.Enabled = false;
                this.txtLicence.Enabled = false;
                return;
            }
            this.labRegInfo.Text = My_ConPar.Machine.Language == 0 ? "未注册" : "NotRegistered";
            this.btnReg.Enabled = true;
            this.txtLicence.Enabled = true;

            await Task.Run(() => ShowZxing());
        }

        /// <summary>
        /// 将二维码矩阵写入图片并显示
        /// </summary>
        public void WriteToFile(ByteMatrix matrix, System.Drawing.Imaging.ImageFormat format, string file)
        {
            Bitmap bmap = ToBitmap(matrix);
            pictureBox1.Image = bmap;
        }

        /// <summary>
        /// 将ByteMatrix转换为Bitmap
        /// </summary>
        private Bitmap ToBitmap(ByteMatrix matrix)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            Bitmap bmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Color black = Color.Black;
            Color white = ColorTranslator.FromHtml("#f6f1f1");

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bmap.SetPixel(x, y, matrix.get_Renamed(x, y) != -1 ? black : white);
                }
            }
            return bmap;
        }

        /// <summary>
        /// 生成并显示二维码
        /// </summary>
        private void ShowZxing()
        {
            ByteMatrix byteMatrix = new MultiFormatWriter().encode(
                _s_mNum + "##" + My_ConPar.AbortInfo.LastVersion,
                BarcodeFormat.QR_CODE, 180, 180);
            WriteToFile(byteMatrix, System.Drawing.Imaging.ImageFormat.Png, "");
        }

        /// <summary>
        /// 窗体关闭事件，恢复主窗体菜单可用
        /// </summary>
        private void Register_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Owner is My_Form.Main.MainForm mainForm)
            {
                mainForm.MiRegister.Enabled = true;
            }
        }

        /// <summary>
        /// 同步按钮点击事件，处理在线注册/试用期同步
        /// </summary>
        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBox2.Visible = true;
                string s_msg = My_Tool.MyRegister.SyncDy(_s_mNum);
                if (!string.IsNullOrEmpty(s_msg))
                {
                    string s_demsg = My_Tool.AES.AesDecrypt(s_msg);
                    string[] sa_array = s_demsg.Split('#');
                    string s_errcode = sa_array[0];
                    if (s_errcode == "errcode=0")
                    {
                        // 完全注册
                        if (sa_array[3] == "isreg=true")
                        {
                            My_File.LocalTranslator.ShowMessage("注册成功！重启软件后生效！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            using (RegistryKey retkey = Registry.CurrentUser.OpenSubKey("Software", true)
                                .CreateSubKey("chec")
                                .CreateSubKey("Register.INI")
                                .CreateSubKey(My_Tool.MyRegister.MyReg.GetRNum()))
                            {
                                retkey.SetValue("UserName", "Rsoft");
                            }
                            this.Close();
                        }
                        else
                        {
                            // 校验时间
                            string[] sa_array2 = sa_array[2].Split('=');
                            DateTime distanceTime = Convert.ToDateTime(sa_array2[1]);
                            DateTime now = DateTime.Now;
                            int i_differ = (now - distanceTime).Days;
                            if (Math.Abs(i_differ) > 1)
                            {
                                My_File.LocalTranslator.ShowMessage("同步失败!请先校准系统时间!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                // 延长天数
                                int i_day = Convert.ToInt32(sa_array[1].Split('=')[1]);
                                string s_enNow = My_Tool.AES.AesEncrypt(DateTime.Now.ToString());
                                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\chec", "CreateDateTime", s_enNow, RegistryValueKind.String);
                                string s_enP_int_day = My_Tool.AES.AesEncrypt(i_day.ToString());
                                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\chec", "UseTimes", s_enP_int_day, RegistryValueKind.String);
                                My_File.LocalTranslator.ShowMessage($"试用期延长{i_day}天成功！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Close();
                            }
                        }
                    }
                    else if (s_errcode == "errcode=10010")
                    {
                        // 被锁定
                        string s_enNow = My_Tool.AES.AesEncrypt(DateTime.Now.ToString());
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\chec", "CreateDateTime", s_enNow, RegistryValueKind.String);
                        string s_enP_int_day = My_Tool.AES.AesEncrypt("0");
                        Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\chec", "UseTimes", s_enP_int_day, RegistryValueKind.String);
                        My_File.LocalTranslator.ShowMessage("已被锁定,请联系经销商!", "信息", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.Close();
                    }
                    else
                    {
                        My_File.LocalTranslator.ShowMessage("同步失败!" + sa_array[4], "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                My_File.Logger.Error("网络连接中断!", ex);
            }
        }

        /// <summary>
        /// 获取试用期剩余天数，已注册返回 null
        /// </summary>
        /// <returns></returns>
        public static  int? GetTrialDaysLeft()
        {
            // 已注册直接返回 null
            if (IsRegistered())
                return null;

            try
            {
                // 读取注册表
                object createDateObj = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\chec", "CreateDateTime", null);
                object useTimesObj = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\chec", "UseTimes", null);

                if (createDateObj == null || useTimesObj == null)
                    return null;

                // 解密
                string createDateStr = My_Tool.AES.AesDecrypt(createDateObj.ToString());
                string useTimesStr = My_Tool.AES.AesDecrypt(useTimesObj.ToString());

                if (!DateTime.TryParse(createDateStr, out DateTime createDate))
                    return null;
                if (!int.TryParse(useTimesStr, out int totalDays))
                    return null;

                int usedDays = (DateTime.Now.Date - createDate.Date).Days;
                int leftDays = totalDays - usedDays;
                return leftDays < 0 ? 0 : leftDays;
            }
            catch
            {
                return null;
            }
        }
    }
}