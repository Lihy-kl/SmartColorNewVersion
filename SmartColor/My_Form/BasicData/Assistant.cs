using SmartColor.My_DataBase;
using SmartColor.My_File;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Form.BasicData
{
    public partial class Assistant : Form
    {
        private bool _isInsert = false;
        // 魔法值常量
        private const string DefaultCost = "100";
        private const string DefaultIntensity = "100";
        private const string DefaultMinColoring = "0";
        private const string DefaultMaxColoring = "0";

        public Assistant()
        {
            InitializeComponent();

            // 优化：启用自定义编辑模式和回车跳转
            this.dgv_Assistant.CustomEditing = true;
            this.dgv_Assistant.EnterKeyAction += Dgv_Assistant_EnterKeyAction;

            this.dgv_Assistant.SelectionChanged += Dgv_Assistant_SelectionChanged;
            this.KeyPreview = true;
            this.Load += Assistant_Load;


            this.FormClosed += Assistant_FormClosed;

            foreach (Control c in this.grp_AssistantDetails.Controls)
            {
                if (c is TextBox || c is ComboBox || c is RadioButton)
                {
                    c.KeyDown += C_KeyDown;
                }
            }
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
            SqlServer.TableDataChanged += SqlServer_TableDataChanged;
           
        }

        private void SqlServer_TableDataChanged(string obj)
        {
            if (obj == My_DataBase.LIMIT_TABLE.TableName)
            {
                Assistant_Load(null, null);
            }
        }

        private void Assistant_FormClosed(object sender, FormClosedEventArgs e)
        {
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
        }
      

        // 控件批量启用/禁用与清空
        private void SetControlsEnabled(bool exceptCode)
        {
            foreach (Control c in this.grp_AssistantDetails.Controls)
            {
                if (c is RadioButton)
                {
                    c.Enabled = true;
                }
                else if (c is TextBox || c is ComboBox)
                {
                    c.Enabled = !exceptCode || c.Name == "txt_AssistantCode";
                    if (exceptCode) c.Text = null;
                }
            }
        }

        private void C_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Type type = sender.GetType();

                    if (type == typeof(TextBox))
                    {
                        TextBox textBox = (TextBox)sender;
                        Logger.Info($"C_KeyDown: 文本框 {textBox.Name} 收到回车键。");

                        switch (textBox.Name)
                        {
                            case "txt_AssistantCode":
                                AssistantCodeJump();
                                break;
                            case "txt_AssistantBarCode":
                                if (!string.IsNullOrWhiteSpace(txt_AssistantBarCode.Text))
                                    txt_AssistantName.Focus();
                                break;
                            case "txt_AssistantName":
                                if (!string.IsNullOrWhiteSpace(txt_AssistantName.Text))
                                    cbo_AssistantType.Focus();
                                break;
                            case "txt_AllowMinColoringConcentration":
                                if (!string.IsNullOrWhiteSpace(txt_AllowMinColoringConcentration.Text))
                                    txt_AllowMaxColoringConcentration.Focus();
                                break;
                            case "txt_AllowMaxColoringConcentration":
                                if (!string.IsNullOrWhiteSpace(txt_AllowMaxColoringConcentration.Text))
                                    txt_TermOfValidity.Focus();
                                break;
                            case "txt_TermOfValidity":
                                if (!string.IsNullOrWhiteSpace(txt_TermOfValidity.Text))
                                    btn_Save.Focus();
                                break;
                            case "txt_Intensity":
                            case "txt_Cost":
                                btn_Save.Focus();
                                break;
                        }
                    }
                    else if (type == typeof(ComboBox))
                    {
                        ComboBox comboBox = (ComboBox)sender;
                        Logger.Info($"C_KeyDown: 下拉框 {comboBox.Name} 收到回车键。");

                        switch (comboBox.Name)
                        {
                            case "cbo_AssistantType":
                                if (!string.IsNullOrWhiteSpace(cbo_AssistantType.Text))
                                    rdo_1.Focus();
                                break;
                           
                               
                        }
                    }
                    else if (type == typeof(RadioButton))
                    {
                        RadioButton radioButton = (RadioButton)sender;
                        Logger.Info($"C_KeyDown: 单选框 {radioButton.Name} 收到回车键。");
                        txt_AllowMinColoringConcentration.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("C_KeyDown: 发生异常。", ex);
            }
        }

        private void AssistantCodeJump()
        {
            try
            {
                var code = txt_AssistantCode.Text.Trim();
                if (string.IsNullOrWhiteSpace(code)) return;

                string filter = BuildFilter(My_DataBase.AssistantData.Assistant_details, code);
                if (filter == null) return;

                DataRow[] dataRows = My_DataBase.AssistantData.Assistant_details.Select(filter);
                if (dataRows.Length > 0)
                {
                    Logger.Info($"AssistantCodeJump: 检测到重复的染助剂代码 {code}。");
                    ShowWarn("染助剂代码重复,请重新输入");
                    txt_AssistantCode.Text = null;
                    txt_AssistantCode.Focus();
                    return;
                }

                SetControlsEnabled(false);

                txt_AssistantBarCode.Text = txt_AssistantCode.Text;
                txt_AllowMinColoringConcentration.Text = DefaultMinColoring;
                txt_AllowMaxColoringConcentration.Text = DefaultMaxColoring;
                txt_Cost.Text = DefaultCost;
                txt_Intensity.Text = DefaultIntensity;
                txt_AssistantName.Focus();
                Logger.Info($"AssistantCodeJump: 染助剂代码 {code} 处理成功。");
            }
            catch (Exception ex)
            {
                Logger.Error("AssistantCodeJump: 发生异常。", ex);
            }
        }

        private void Dgv_Assistant_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                _isInsert = false;
                if (dgv_Assistant.SelectedRows == null || dgv_Assistant.SelectedRows.Count == 0)
                {
                    SetControlsEnabled(true);
                    foreach (Control c in this.grp_AssistantDetails.Controls)
                    {
                        if (c is RadioButton button)
                            button.Checked = false;
                    }
                    return;
                }

                var codeValue = dgv_Assistant.SelectedRows[0].Cells[1].Value;
                if (codeValue == null || codeValue == DBNull.Value) return;

                string filter = BuildFilter(My_DataBase.AssistantData.Assistant_details, codeValue);
                var dr_assistantdetails = My_DataBase.AssistantData.Assistant_details.Select(filter);
                if(dr_assistantdetails.Length == 0) return;

                DataTable dt_assistantdetails = dr_assistantdetails.CopyToDataTable();
                foreach (DataColumn mDc in dt_assistantdetails.Columns)
                {
                    string s_name = "txt_" + mDc.Caption.ToString();
                    foreach (Control c in this.grp_AssistantDetails.Controls)
                    {
                        if (c is TextBox && c.Name == s_name)
                        {
                            c.Text = dt_assistantdetails.Rows[0][mDc] is DBNull ? "" : dt_assistantdetails.Rows[0][mDc].ToString();
                            c.Enabled = c.Name != "txt_AssistantCode";
                        }

                        if (s_name == "txt_UnitOfAccount")
                        {
                            string unit = dt_assistantdetails.Rows[0][mDc].ToString();
                            rdo_1.Checked = unit == "%";
                            rdo_2.Checked = unit == "g/l";
                            rdo_3.Checked = unit == "G/L";
                            rdo_4.Checked = !(rdo_1.Checked || rdo_2.Checked || rdo_3.Checked);
                        }

                        if (s_name == "txt_AssistantType")
                        {
                            cbo_AssistantType.Text = dt_assistantdetails.Rows[0][mDc].ToString();
                        }

                        if (s_name == "txt_Reweigh")
                        {
                            string reweighText = dt_assistantdetails.Rows[0][mDc] is DBNull ? "0" : dt_assistantdetails.Rows[0][mDc].ToString();
                            
                        }
                    }
                }
                Logger.Info($"dgv_Assistant_SelectionChanged: 选中染助剂代码 {codeValue}。");
            }
            catch (Exception ex)
            {
                Logger.Error("dgv_Assistant_SelectionChanged: 发生异常。", ex);
            }
        }

        // 优化：使用 CtDataGridView 的 BindDataTable 方法进行表格绑定
        private void Assistant_Load(object sender, EventArgs e)
        {
            try
            {

                Logger.Info("Assistant_Load: 开始加载染助剂资料表。");

                cbo_AssistantType.Items.Clear();
                if (My_DataBase.LimitData.LimitTable != null)
                {
                    var types = My_DataBase.LimitData.LimitTable.AsEnumerable()
                        .Select(r => r.Field<string>(My_DataBase.LIMIT_TABLE.Type))
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .Distinct()
                        .ToList();


                    foreach (var type in types)
                    {
                        cbo_AssistantType.Items.Add(type);
                    }
                }

                cbo_AssistantType.Items.Add("助剂");

                dgv_Assistant.BindDataTable(
                    My_DataBase.AssistantData.Assistant_details,
                    row => new object[] { dgv_Assistant.Rows.Count + 1, row[My_DataBase.ASSISTANT_DETAILS.AssistantCode], row[My_DataBase.ASSISTANT_DETAILS.AssistantName] },
                   My_DataBase.ASSISTANT_DETAILS.AssistantCode
                );
                Logger.Info($"Assistant_Load: 加载完成，共加载 {dgv_Assistant.Rows.Count} 条数据。");
            }
            catch (Exception ex)
            {
                Logger.Error("Assistant_Load: 加载染助剂资料表时发生异常。", ex);
            }
        }



        private string BuildFilter(DataTable table, object codeValue)
        {
            bool isString = table.Columns[My_DataBase.ASSISTANT_DETAILS.AssistantCode].DataType == typeof(string);
            string codeStr = codeValue.ToString().Replace("'", "''");
            return isString
                ? $"{My_DataBase.ASSISTANT_DETAILS.AssistantCode} = '{codeStr}'"
                : $"{My_DataBase.ASSISTANT_DETAILS.AssistantCode} = {codeStr}";
        }

        private void Btn_Insert_Click(object sender, EventArgs e)
        {
            try
            {
                SetControlsEnabled(true);
                foreach (Control c in this.grp_AssistantDetails.Controls)
                {
                    if (c is RadioButton button)
                        button.Checked = false;
                }
                dgv_Assistant.ClearSelection();
                txt_AssistantCode.Focus();
                _isInsert = true;
                Logger.Info("btn_Insert_Click: 启动新增模式。");
            }
            catch (Exception ex)
            {
                Logger.Error("btn_Insert_Click: 发生异常。", ex);
            }
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Control c in this.grp_AssistantDetails.Controls)
                {
                    if ((c is TextBox || c is ComboBox) && string.IsNullOrWhiteSpace(c.Text))
                    {
                        ShowWarn("请完善所有资料后再点存档");
                        return;
                    }
                }

                Dictionary<string, object> dic = new Dictionary<string, object>
                {
                    { My_DataBase.ASSISTANT_DETAILS.AssistantCode, txt_AssistantCode.Text.Trim() },
                    { My_DataBase.ASSISTANT_DETAILS.AssistantBarCode, txt_AssistantBarCode.Text.Trim() },
                    { My_DataBase.ASSISTANT_DETAILS.AssistantName, txt_AssistantName.Text.Trim() },
                    { My_DataBase.ASSISTANT_DETAILS.AssistantType, cbo_AssistantType.Text.Trim() },
                    { My_DataBase.ASSISTANT_DETAILS.UnitOfAccount, rdo_1.Checked ? rdo_1.Text.Trim() : (rdo_2.Checked ? rdo_2.Text.Trim() : (rdo_3.Checked ? rdo_3.Text.Trim() : rdo_4.Text.Trim())) },
                    { My_DataBase.ASSISTANT_DETAILS.AllowMinColoringConcentration, txt_AllowMinColoringConcentration.Text.Trim() },
                    { My_DataBase.ASSISTANT_DETAILS.AllowMaxColoringConcentration, txt_AllowMaxColoringConcentration.Text.Trim() },
                    { My_DataBase.ASSISTANT_DETAILS.TermOfValidity, txt_TermOfValidity.Text.Trim() },
                    { My_DataBase.ASSISTANT_DETAILS.Intensity, txt_Intensity.Text.Trim() },
                    { My_DataBase.ASSISTANT_DETAILS.Cost, txt_Cost.Text.Trim() },
                    { My_DataBase.ASSISTANT_DETAILS.Reweigh,  0 }
                };

                var u = dic[My_DataBase.ASSISTANT_DETAILS.UnitOfAccount];
                if (u != null)
                {
                    string s = u.ToString();
                    if (s == rdo_3.Text || s == rdo_4.Text)
                    {
                        DataRow[] drs = My_DataBase.AssistantData.Assistant_details.Select($"{My_DataBase.ASSISTANT_DETAILS.UnitOfAccount} ='{s}'");
                        if (drs != null && drs.Length > 0)
                        {
                            ShowWarn("只能存在一个此单位助剂代码!");
                            return;
                        }
                    }
                }

                if (_isInsert)
                {
                    My_DataBase.SqlServer.Insert(My_DataBase.ASSISTANT_DETAILS.TableName, dic);
                    Logger.Info($"btn_Save_Click: 新增染助剂代码 {dic[My_DataBase.ASSISTANT_DETAILS.AssistantCode]}。");
                }
                else
                {
                    string filter = BuildFilter(My_DataBase.AssistantData.Assistant_details, txt_AssistantCode.Text.Trim());
                    My_DataBase.SqlServer.Update(My_DataBase.ASSISTANT_DETAILS.TableName, dic, filter);
                    Logger.Info($"btn_Save_Click: 更新染助剂代码 {dic[My_DataBase.ASSISTANT_DETAILS.AssistantCode]}。");
                }

               
                Assistant_Load(sender, new EventArgs());

                foreach (DataGridViewRow viewRow in dgv_Assistant.Rows)
                {
                    if (viewRow.Cells[1].Value?.ToString().Trim() == dic[My_DataBase.ASSISTANT_DETAILS.AssistantCode].ToString().Trim())
                    {
                        viewRow.Selected = true;
                        dgv_Assistant.CurrentCell = viewRow.Cells[0];
                        break;
                    }
                }
                
                ShowInfo("保存成功!");
                btn_Insert.Focus();
            }
            catch (Exception ex)
            {
                Logger.Error("btn_Save_Click: 发生异常。", ex);
            }
        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgv_Assistant.SelectedRows == null || dgv_Assistant.SelectedRows.Count == 0)
                {
                    ShowWarn("请选择要删除的染助剂!");
                    return;
                }
                int currentRowIndex = dgv_Assistant.CurrentRow.Index;
                var codeValue = dgv_Assistant.SelectedRows[0].Cells[1].Value;
                if (codeValue != null)
                {
                    DataRow[] drs = My_DataBase.BottleData.Bottle_details.Select(BuildFilter(My_DataBase.BottleData.Bottle_details, codeValue));

                    if (drs.Length > 0)
                    {
                        if (DialogResult.OK == ShowConfirm("此染助剂正在使用,继续删除会把对应的母液瓶也删除，请问继续删除吗!"))
                        {
                            foreach (DataRow row in drs)
                            {
                                var bottle = row["BottleNum"];
                                if (bottle != null)
                                {
                                    My_DataBase.SqlServer.Delete("bottle_details", $"BottleNum = {bottle}");
                                }
                            }
                            

                            string filter = BuildFilter(My_DataBase.AssistantData.Assistant_details, codeValue);
                            My_DataBase.SqlServer.Delete(My_DataBase.ASSISTANT_DETAILS.TableName, filter);
                           
                            Assistant_Load(sender, new EventArgs());
                        }
                    }
                    else
                    {
                        if (ShowConfirm("确认删除此染助剂吗?") == DialogResult.OK)
                        {
                            string filter = BuildFilter(My_DataBase.AssistantData.Assistant_details, codeValue);
                            My_DataBase.SqlServer.Delete(My_DataBase.ASSISTANT_DETAILS.TableName, filter);
                          
                            Assistant_Load(sender, new EventArgs());
                        }
                    }
                    if (currentRowIndex < dgv_Assistant.Rows.Count - 1)
                    {
                        dgv_Assistant.Rows[currentRowIndex].Selected = true;
                        dgv_Assistant.CurrentCell = dgv_Assistant.Rows[currentRowIndex].Cells[0];
                    }
                    else
                    {
                        dgv_Assistant.Rows[dgv_Assistant.Rows.Count - 1].Selected = true;
                        dgv_Assistant.CurrentCell = dgv_Assistant.Rows[dgv_Assistant.Rows.Count - 1].Cells[0];
                    }                    
                    Logger.Info($"btn_Delete_Click: 删除染助剂代码 {codeValue}。");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("btn_Delete_Click: 发生异常。", ex);
            }
        }

        // 优化：自定义回车跳转行为
        private bool Dgv_Assistant_EnterKeyAction()
        {
            // 回车跳转到下一行
            if (dgv_Assistant.CurrentCell != null)
            {
                int nextRow = dgv_Assistant.CurrentCell.RowIndex + 1;
                if (nextRow < dgv_Assistant.Rows.Count)
                {
                    dgv_Assistant.CurrentCell = dgv_Assistant.Rows[nextRow].Cells[dgv_Assistant.CurrentCell.ColumnIndex];
                    return true;
                }
            }
            return false;
        }

        // 统一消息提示
        private void ShowError(string msg) => LocalTranslator.ShowMessage(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowInfo(string msg) => LocalTranslator.ShowMessage(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void ShowWarn(string msg) => LocalTranslator.ShowMessage(msg, "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        private DialogResult ShowConfirm(string msg) => LocalTranslator.ShowMessage(msg, "温馨提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
    }
}