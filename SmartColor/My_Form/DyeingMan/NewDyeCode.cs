using SmartColor.My_Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartColor.My_File; // 引入Logger命名空间

namespace SmartColor.My_Form.DyeingMan
{
    /// <summary>
    /// 新增染固色工艺代码窗口
    /// </summary>
    public partial class NewDyeCode : Form
    {
        

        private readonly TableLayoutPanel _panel; // 染色步骤容器
        private readonly CtDropHead _dropHead;    // 染色头部信息控件

        /// <summary>
        /// 构造函数，初始化窗口及控件
        /// </summary>
        /// <param name="panel">染色步骤容器</param>
        /// <param name="head">染色头部信息控件</param>
        public NewDyeCode(TableLayoutPanel panel, CtDropHead head)
        {
            this._panel = panel ?? throw new ArgumentNullException(nameof(panel));
            this._dropHead = head ?? throw new ArgumentNullException(nameof(head));
            InitializeComponent();
            textBox1.KeyDown += TextBox1_KeyDown;
        }

        /// <summary>
        /// 监听输入框回车事件，回车后聚焦保存按钮
        /// </summary>
        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && e.KeyCode == Keys.Enter)
            {
                btn_Save.Focus();
            }
        }

        /// <summary>
        /// 保存按钮点击事件，校验输入并保存新染固色工艺代码
        /// </summary>
        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Info("开始保存新染固色工艺代码流程。");

                // 1. 校验输入
                string inputCode = textBox1.Text?.Trim();
                if (string.IsNullOrEmpty(inputCode))
                {
                    Logger.Info("未输入新染固色工艺代码。");
                    My_File.LocalTranslator.ShowMessage("请输入新染固色工艺代码", "Btn_Save_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Focus();
                    textBox1.SelectAll();
                    return;
                }

                // 2. 检查代码是否已存在
                var dt = My_DataBase.DyeingCodeData.Dyeing_code;
                var existRows = dt.Select($"{My_DataBase.DYEING_CODE.DyeingCode}= '{inputCode}'");
                if (existRows.Length > 0)
                {
                    Logger.Info($"输入的染固色工艺代码已存在：{inputCode}");
                    My_File.LocalTranslator.ShowMessage("该染固色工艺代码已存在，请重新输入", "Btn_Save_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Focus();
                    textBox1.SelectAll();
                    return;
                }

                // 3. 收集所有有效染色步骤
                var lines = new List<Dictionary<string, object>>();
                foreach (Control c in _panel.Controls)
                {
                    if (c is CtDyeing cd)
                    {
                        int indexNum = _panel.Controls.IndexOf(cd);
                        (int type, string code) = cd.HeadValues();
                        // 跳过无效步骤
                        if (type == 0 || string.IsNullOrEmpty(code))
                        {
                            Logger.Info($"跳过无效的染色步骤，Index: {indexNum}, Type: {type}, Code: {code}");
                            continue;
                        }
                        // 构造步骤数据
                        var data = new Dictionary<string, object>
                        {
                            { My_DataBase.DYEING_CODE.DyeingCode, inputCode },
                            { My_DataBase.DYEING_CODE.Type, type },
                            { My_DataBase.DYEING_CODE.Step, indexNum },
                            { My_DataBase.DYEING_CODE.Code, code },
                            { My_DataBase.DYEING_CODE.IndexNum, indexNum },
                            { My_DataBase.DYEING_CODE.IsUse, 1 },
                            { My_DataBase.DYEING_CODE.Remark, "" }
                        };
                        lines.Add(data);
                        Logger.Info($"添加染色步骤，IndexNum: {indexNum}, Type: {type}, DyeingCode: {inputCode}");
                    }
                }

                // 4. 校验步骤是否有效
                if (lines.Count == 0)
                {
                    Logger.Info("未检测到有效的染色步骤，保存操作终止。");
                    My_File.LocalTranslator.ShowMessage("未检测到有效的染色步骤，无法保存。", "Btn_Save_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 5. 执行数据库插入
                My_DataBase.SqlServer.Insert(My_DataBase.DYEING_CODE.TableName, lines);
                Logger.Info($"新染固色工艺代码保存成功：{inputCode}，共{lines.Count}条步骤。");

               
              
                _dropHead.SetDyeingCode(inputCode);

                My_File.LocalTranslator.ShowMessage("保存成功!", "Btn_Save_Click", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 7. 关闭窗口
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("保存新染固色工艺代码时发生异常。", ex);
            }
        }
    }
}