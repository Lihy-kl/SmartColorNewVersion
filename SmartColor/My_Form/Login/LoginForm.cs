using SmartColor.My_DataBase;
using SmartColor.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace SmartColor.My_Form.Login
{
    public partial class LoginForm : Form
    {
        /// <summary>用户信息变更事件</summary>
        public static event EventHandler UserChanged;


        /// <summary>
        /// 用户信息缓存，键为用户名，值为(密码, 权限)
        /// </summary>
        public static  Dictionary<string, (string Password, int Purview, string Name)> UserCache;

        public LoginForm()
        {
            InitializeComponent();
            this.FormClosed += LoginForm_FormClosed;
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
            SqlServer.TableDataChanged += SqlServer_TableDataChanged;
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
        }

        private void SqlServer_TableDataChanged(string obj)
        {
            if (obj == USER_TALE.TableName)
            {
                LoadUserCache();
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            LoadUserCache(); // 加载用户缓存
            TxtName.Text = Properties.Settings.Default.Account;
          

        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtName.Text))
            {
                TxtName.Focus();
            }
            else
            {
                TxtPassword.Focus();
            }
        }

        private void TxtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && TxtName.Text.Trim().Length > 0)
            {
                TxtPassword.Focus();
            }
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && TxtPassword.Text.Trim().Length > 0)
            {
                BtnLogOn.Focus();
            }
        }

        private void BtnLogOn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnLogOn_Click(sender, e);
            }
        }

        private void BtnLogOn_Click(object sender, EventArgs e)
        {
            // 登录验证逻辑
            string userName = TxtName.Text.Trim();
            string password = TxtPassword.Text.Trim();

            if (UserCache.TryGetValue(userName, out var userInfo) &&
                string.Equals(userInfo.Password, password, StringComparison.OrdinalIgnoreCase))
            {
                Properties.Settings.Default.Account = userName;
                Properties.Settings.Default.Save();
                this.DialogResult = DialogResult.Yes;
                this.Close();
                UserChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                My_File.LocalTranslator.ShowMessage("用户名或密码错误，请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TxtPassword.Clear();
                TxtPassword.Focus();
            }
        }



        private void LabPasswoedChange_Click(object sender, EventArgs e)
        {
            // 密码修改逻辑
            using (ChangePassword changePassword = new ChangePassword())
            {
                changePassword.ShowDialog();
            }
        }

        // 加载所有用户到内存
        private void LoadUserCache()
        {
            UserCache = new Dictionary<string, (string, int,string)>(StringComparer.OrdinalIgnoreCase);
            DataTable dt = My_DataBase.UserData.UserTable;

            if (dt.Rows.Count == 0)
            {
                // 添加默认用户
                var uesrs = new List<Dictionary<string, object>>
                {
                     new Dictionary<string, object> { { "Account", "123" }, { "Password", "123" }, { "Purview", 0 },{ "RealName", "操作员" } },
                     new Dictionary<string, object> { { "Account", "admin" }, { "Password", "999" }, { "Purview", 1 } ,{ "RealName", "管理员" }},
                     new Dictionary<string, object> { { "Account", "eng" }, { "Password", "gzkl2010" }, { "Purview", 2 }, { "RealName", "工程师" } }
                };
                My_DataBase.SqlServer.Insert("User_Tale", uesrs);
                dt = My_DataBase.SqlServer.Select("User_Tale");
            }

            foreach (DataRow row in dt.Rows)
            {
                string name = row["Account"].ToString();
                string pwd = row["Password"].ToString();
                int purview = Convert.ToInt32(row["Purview"]);
                string realName =  row["RealName"].ToString();
                UserCache[name] = (pwd, purview,realName);
            }
        }
    }
}