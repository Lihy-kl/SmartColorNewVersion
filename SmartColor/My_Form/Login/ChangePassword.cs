using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;


namespace SmartColor.My_Form.Login
{
    public partial class ChangePassword : Form
    {

        public ChangePassword()
        {
            InitializeComponent();
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtAccount.Text))
            {
                My_File.LocalTranslator.ShowMessage("请输入账户！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TxtAccount.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(TxtOldPassword.Text))
            {
                My_File.LocalTranslator.ShowMessage("请输入旧密码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TxtOldPassword.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(TxtNewPassword.Text))
            {
                My_File.LocalTranslator.ShowMessage("请输入新密码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TxtNewPassword.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(TxtConfirmPassword.Text))
            {
                My_File.LocalTranslator.ShowMessage("请输入确认密码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TxtConfirmPassword.Focus();
                return;
            }
            else if(string.IsNullOrEmpty(TxtRealName.Text))
            {
                My_File.LocalTranslator.ShowMessage("请输入真实姓名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TxtRealName.Focus();
                return;
            }
            else if (TxtNewPassword.Text != TxtConfirmPassword.Text)
            {
                My_File.LocalTranslator.ShowMessage("新密码与确认密码不一致！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TxtNewPassword.Focus();
                return;
            }
            
            else
            {
                string account = TxtAccount.Text.Trim();
                string oldPassword = TxtOldPassword.Text.Trim();
                string newPassword = TxtNewPassword.Text.Trim();
                string realName = TxtRealName.Text.Trim();
                if (LoginForm.UserCache.TryGetValue(account, out var userInfo) &&
                    userInfo.Password == oldPassword && userInfo.Purview <2)
                {
                    // 构造更新字段和值
                    var updateValues = new Dictionary<string, object>
                    {
                        { "Password", newPassword },
                        { "RealName",realName}
                    };
                    // 构造条件
                    string where = $"Account='{account}'";

                    int result = My_DataBase.SqlServer.Update("User_Tale", updateValues, where);
                    if (result > 0)
                    {
                        // 更新内存缓存
                        LoginForm.UserCache[account] = (newPassword, userInfo.Purview,realName);
                        My_File.LocalTranslator.ShowMessage("密码修改成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        My_File.LocalTranslator.ShowMessage("密码修改失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (userInfo.Purview >= 2)
                    {
                        My_File.LocalTranslator.ShowMessage("该账户不允许修改密码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        My_File.LocalTranslator.ShowMessage("账户或密码错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    TxtAccount.Focus();
                }
            }
        }
    }
}
