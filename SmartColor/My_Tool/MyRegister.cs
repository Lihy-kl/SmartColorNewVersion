using System;
using System.Management;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace SmartColor.My_Tool
{
    /// <summary>
    /// 软件注册与授权管理类，负责生成机器码、注册码、校验注册状态、同步授权信息等功能。
    /// </summary>
    public class MyRegister
    {
        // 单例实例，避免多次创建
        public static  MyRegister MyReg;

        // 密钥数组（长度127，索引1~126有效）
        private readonly int[] ia_code = new int[127];
        // 机器码字符数组（长度25，索引1~24有效）
        private readonly char[] ca_code = new char[25];
        // 机器码ASCII值数组（长度25，索引1~24有效）
        private readonly int[] ia_number = new int[25];

        // 硬盘序列号字符数组（长度9，索引1~8有效）
        private readonly char[] ca_codeDS = new char[9];
        // 硬盘序列号ASCII值数组（长度9，索引1~8有效）
        private readonly int[] ia_numberDS = new int[9];

        /// <summary>
        /// 获取C盘硬盘卷标号
        /// </summary>
        /// <returns>卷标号字符串</returns>
        public string GetDiskVolumeSerialNumber()
        {
            // 通过WMI获取C盘卷标号
            using (ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\""))
            {
                disk.Get();
                return disk.GetPropertyValue("VolumeSerialNumber").ToString();
            }
        }

        /// <summary>
        /// 获取CPU序列号
        /// </summary>
        /// <returns>CPU序列号字符串</returns>
        public string GetCpu()
        {
            string s_cpu = null;
            using (ManagementClass myCpu = new ManagementClass("win32_Processor"))
            {
                foreach (ManagementObject myObject in myCpu.GetInstances())
                {
                    s_cpu = myObject.Properties["Processorid"].Value.ToString();
                    break; // 只取第一个CPU
                }
            }
            return s_cpu;
        }

        /// <summary>
        /// 生成机器码（CPU序列号+硬盘卷标号，取前24位）
        /// </summary>
        /// <returns>机器码字符串</returns>
        public string GetMNum()
        {
            string s_num = GetCpu() + GetDiskVolumeSerialNumber();
            // 若长度不足24位，右侧补0
            if (s_num.Length < 24)
                s_num = s_num.PadRight(24, '0');
            return s_num.Substring(0, 24);
        }

        /// <summary>
        /// 初始化密钥数组（ia_code），用于注册码生成算法
        /// </summary>
        private void SetIntCode()
        {
            for (int i = 1; i < ia_code.Length; i++)
            {
                ia_code[i] = i % 9;
            }
        }

        /// <summary>
        /// 根据机器码生成注册码
        /// </summary>
        /// <returns>注册码字符串</returns>
        public string GetRNum()
        {
            SetIntCode();
            string s_mNum = GetMNum();
            // 机器码字符填充到ca_code
            for (int i = 1; i < ca_code.Length; i++)
            {
                ca_code[i] = Convert.ToChar(s_mNum.Substring(i - 1, 1));
            }
            // 生成ASCII码值并加密
            for (int j = 1; j < ia_number.Length; j++)
            {
                ia_number[j] = Convert.ToInt32(ca_code[j]) + ia_code[Convert.ToInt32(ca_code[j])];
            }
            // 生成注册码字符串
            StringBuilder s_asciiName = new StringBuilder();
            for (int k = 1; k < ia_number.Length; k++)
            {
                int val = ia_number[k];
                // 保证注册码字符在0-9、A-Z、a-z范围
                if ((val >= 48 && val <= 57) || (val >= 65 && val <= 90) || (val >= 97 && val <= 122))
                {
                    s_asciiName.Append(Convert.ToChar(val));
                }
                else if (val > 122)
                {
                    s_asciiName.Append(Convert.ToChar(val - 10));
                }
                else
                {
                    s_asciiName.Append(Convert.ToChar(val - 9));
                }
            }
            return s_asciiName.ToString();
        }

        /// <summary>
        /// 根据硬盘序列号生成注册码（仅取前8位）
        /// </summary>
        /// <returns>注册码字符串</returns>
        public string GetRNumDS()
        {
            SetIntCode();
            string s_mNum = GetDiskVolumeSerialNumber();
            if (s_mNum.Length < 8)
                s_mNum = s_mNum.PadRight(8, '0');
            s_mNum = s_mNum.Substring(0, 8);
            // 填充到ca_codeDS
            for (int i = 1; i < ca_codeDS.Length; i++)
            {
                ca_codeDS[i] = Convert.ToChar(s_mNum.Substring(i - 1, 1));
            }
            // 生成ASCII码值并加密
            for (int j = 1; j < ia_numberDS.Length; j++)
            {
                ia_numberDS[j] = Convert.ToInt32(ca_codeDS[j]) + ia_code[Convert.ToInt32(ca_codeDS[j])];
            }
            // 生成注册码字符串
            StringBuilder s_asciiName = new StringBuilder();
            for (int k = 1; k < ia_numberDS.Length; k++)
            {
                int val = ia_numberDS[k];
                if ((val >= 48 && val <= 57) || (val >= 65 && val <= 90) || (val >= 97 && val <= 122))
                {
                    s_asciiName.Append(Convert.ToChar(val));
                }
                else if (val > 122)
                {
                    s_asciiName.Append(Convert.ToChar(val - 10));
                }
                else
                {
                    s_asciiName.Append(Convert.ToChar(val - 9));
                }
            }
            return s_asciiName.ToString();
        }

        /// <summary>
        /// 校验软件是否过期或已注册，处理注册逻辑和本地授权信息
        /// </summary>
        /// <returns>
        /// 0=未过期，1001=注册成功，1=系统时间异常，10010=重置，-1=校验失败
        /// </returns>
        public static  int overdue()
        {
            if (MyReg == null)
                MyReg = new MyRegister();

            string s_rnum = MyReg.GetRNum();
            string s_mnum = MyReg.GetMNum();

            // 打开注册表，查找注册码
            RegistryKey retkey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey("chec").CreateSubKey("Register.INI");
            foreach (string strRNum in retkey.GetSubKeyNames())
            {
                if (strRNum == s_rnum)
                {
                    return 0; // 已注册
                }
            }

            int i_usetime = 0;
            DateTime P_dt_create = DateTime.MinValue;
            DateTime P_dt_last = DateTime.MinValue;
            DateTime P_dt_now = DateTime.Now;

            try
            {
                // 获取允许使用天数
                string i_usetimeStr = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\chec", "UseTimes", "");
                i_usetimeStr = AES.AesDecrypt(i_usetimeStr);
                i_usetime = Convert.ToInt32(i_usetimeStr);

                // 获取上次使用时间
                P_dt_last = Convert.ToDateTime(Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\chec", "LastDateTime", null));

                // 获取创建时间
                string s_createStr = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\chec", "CreateDateTime", "");
                s_createStr = AES.AesDecrypt(s_createStr);
                P_dt_create = Convert.ToDateTime(s_createStr);
            }
            catch (Exception ex)
            {
                // 异常时弹窗提示并写入日志
                SmartColor.My_File.LocalTranslator.ShowMessage($"倒计时异常:{ex}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SmartColor.My_File.Logger.Error("倒计时异常:", ex);
            }

            // 检查系统时间是否被回调
            if (P_dt_last > P_dt_now)
            {
                return 1; // 系统时间异常
            }
            else if ((P_dt_now - P_dt_last).Days >= 1)
            {
                // 每天只更新一次上次使用时间
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\chec", "LastDateTime", P_dt_now, RegistryValueKind.String);
            }

            // 检查是否在允许使用天数内
            if ((P_dt_now - P_dt_create).Days < i_usetime)
            {
                return 0; // 未过期
            }
            else
            {
                // 超期后尝试同步服务器授权信息
                try
                {
                    string s_msg = SyncDy(s_mnum);
                    if (!string.IsNullOrEmpty(s_msg))
                    {
                        string s_demsg = AES.AesDecrypt(s_msg);
                        Console.WriteLine("解密后的文本" + s_demsg);
                        string[] sa_array = s_demsg.Split('#');
                        string s_errcode = sa_array[0];
                        if (s_errcode.Equals("errcode=0"))
                        {
                            // 判断是否完全注册
                            string s_isreg = sa_array[3];
                            if (s_isreg.Equals("isreg=true"))
                            {
                                // 注册成功，写入注册表
                                retkey = Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("chec").CreateSubKey("Register.INI").CreateSubKey(s_rnum);
                                retkey.SetValue("UserName", "Rsoft");
                                return 1001;
                            }
                            else
                            {
                                // 校验服务器返回的剩余天数和时间
                                string s_time = sa_array[2];
                                string[] sa_array2 = s_time.Split('=');
                                DateTime distanceTime = Convert.ToDateTime(sa_array2[1]);
                                DateTime now = DateTime.Now;
                                int i_differ = Math.Abs((now - distanceTime).Days);
                                if (i_differ > 1)
                                {
                                    return 1; // 时间差异常
                                }
                                else
                                {
                                    // 延长天数
                                    string s_dayStr = sa_array[1];
                                    string[] sa_array3 = s_dayStr.Split('=');
                                    int i_day = Convert.ToInt32(sa_array3[1]);
                                    string s_enNow = AES.AesEncrypt(DateTime.Now.ToString());
                                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\chec", "CreateDateTime", s_enNow, RegistryValueKind.String);
                                    string s_enP_int_day = AES.AesEncrypt(i_day.ToString());
                                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\chec", "UseTimes", s_enP_int_day, RegistryValueKind.String);
                                    return 1001;
                                }
                            }
                        }
                        else if (s_errcode.Equals("errcode=10010"))
                        {
                            // 服务器要求重置
                            string s_enNow = AES.AesEncrypt(DateTime.Now.ToString());
                            Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\chec", "CreateDateTime", s_enNow, RegistryValueKind.String);
                            string s_enP_int_day = AES.AesEncrypt("0");
                            Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\chec", "UseTimes", s_enP_int_day, RegistryValueKind.String);
                            return 10010;
                        }
                        else
                        {
                            return -1; // 其他错误
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 同步异常不抛出，返回-1
                    SmartColor.My_File.Logger.Error("注册同步异常:", ex);
                }
                return -1;
            }
        }

        /// <summary>
        /// 同步服务器授权信息，返回加密字符串
        /// </summary>
        /// <param name="s_rnum">机器码</param>
        /// <returns>服务器返回的加密字符串</returns>
        public static  string SyncDy(string s_rnum)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                string s_formattedDateTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                string s_enstr = s_rnum + "##" + s_formattedDateTime;
                string s_enstrCode = AES.AesEncrypt(s_enstr);
                string s_enstrCodeBase64 = HttpUtil.Base64Encrypt(s_enstrCode);
                IDictionary<string, string> dic_parameters = new Dictionary<string, string>
                {
                    { "msg", s_enstrCodeBase64 },
                    { "sendtime", s_formattedDateTime },
                    { "Version", My_ConPar.AbortInfo.LastVersion }
                };
                HttpWebResponse response = HttpUtil.CreatePostHttpResponse(My_ConPar.AbortInfo.URL + "/outer/product/getDySyn", dic_parameters, 15000, null, null);
                using (Stream st = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(st))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                // 抛出异常由调用方处理
                throw new Exception(ex.Message);
            }
        }
    }
}