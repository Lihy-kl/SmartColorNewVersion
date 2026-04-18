using SmartColor.My_ConPar.Area.RotorCylinder;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Form.BasicData
{
    public partial class Bottle : Form
    {
       

        private const string DefaultCurrentWeight = "0";
        private const string DefaultDropMinWeight = "1";
        private const string DefaultSyringeType = "小针筒";
        private const string DefaultAllowMaxWeight = "1000";
        private const string DefaultDripReserveFirst = "否";

        private bool _isInsert = false;

        public Bottle()
        {
            InitializeComponent();

            // 优化：启用自定义编辑模式和回车跳转
            this.dgv_Bottle.CustomEditing = true;
            this.dgv_Bottle.EnterKeyAction += Dgv_Bottle_EnterKeyAction;

            this.dgv_Bottle.SelectionChanged += Dgv_Bottle_SelectionChanged;
            this.KeyPreview = true;
            this.Load += Bottle_Load;
            this.FormClosed += Bottle_FormClosed;
            foreach (Control c in this.grp_BottleDetails.Controls)
            {
                if (c is TextBox || c is ComboBox || c is DateTimePicker)
                {
                    c.KeyDown += C_KeyDown;
                }
            }
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
            SqlServer.TableDataChanged += SqlServer_TableDataChanged; 
            
        }

        private void SqlServer_TableDataChanged(string obj)
        {
           if(obj == My_DataBase.ASSISTANT_DETAILS.TableName)
            {
                LoadAssistantCode(null, null);
            }
           else if(obj == My_DataBase.BREWING_CODE.TableName)
            {
                LoadBrewingCode(null,null);
            }
           else if(obj == My_DataBase.ABS_PROCESS.TableName)
            {
                LoadABSCodes(null, null);
            }

        }

        private void Bottle_FormClosed(object sender, FormClosedEventArgs e)
        {
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
        }

        private void LoadABSCodes(object sender, EventArgs e)
        {
            cbo_AbsCode.Items.Clear();
            if (My_DataBase.ABSProcess.ABS_Process == null)
            {
                Logger.Info("LoadABSCodes：ABS_Process为空，跳过加载。");
                return;
            }
            var codes = My_DataBase.ABSProcess.ABS_Process.AsEnumerable()
                 .Select(r => r[SmartColor.My_DataBase.ABS_PROCESS.Code]?.ToString())
                 .Where(c => !string.IsNullOrWhiteSpace(c))
                 .Distinct()
                 .ToList();

            foreach (var code in codes)
            {
                cbo_AbsCode.Items.Add(code);
            }
        }

        private void LoadBrewingCode(object sender, EventArgs e)
        {
            cbo_BrewingCode.Items.Clear();
            foreach (DataRow dr in My_DataBase.BrewData.Brewing_code.Rows)
            {
                cbo_BrewingCode.Items.Add(dr[My_DataBase.BREWING_CODE. BrewingCode].ToString());
            }
        }

        private void LoadAssistantCode(object sender, EventArgs e)
        {
            cbo_AssistantCode.Items.Clear();
            foreach (DataRow dr in My_DataBase.AssistantData.Assistant_details.Rows)
            {
                cbo_AssistantCode.Items.Add(dr[My_DataBase.ASSISTANT_DETAILS.AssistantCode].ToString());
            }
        }

      
      

        private void SetBottleDetailsControlsEnabledExcept(string exceptName)
        {
            foreach (Control c in this.grp_BottleDetails.Controls)
            {
                c.Enabled = c.Name != exceptName;
            }
        }

        private void ClearControls(Control.ControlCollection controls, bool clearRadio)
        {
            foreach (Control c in controls)
            {
                if (c is TextBox || c is ComboBox)
                {
                    c.Text = null;
                }
                else if (clearRadio && c is RadioButton rb)
                {
                    rb.Checked = false;
                }
            }
        }

        private void C_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter) return;
                if (sender is TextBox textBox)
                {
                    Logger.Info($"C_KeyDown：文本框 {textBox.Name} 收到回车键。");
                    switch (textBox.Name)
                    {
                        case "txt_BottleNum":
                            BottleNumJump();
                            break;
                        case "txt_SettingConcentration":
                            SettingConcentrationCheck();
                            break;
                        case "txt_CurrentWeight":
                            if (string.IsNullOrWhiteSpace(txt_CurrentWeight.Text.Trim()))
                                txt_CurrentWeight.Focus();
                            else
                                cbo_SyringeType.Focus();
                            break;
                        case "txt_DropMinWeight":
                            if (string.IsNullOrWhiteSpace(txt_DropMinWeight.Text.Trim()))
                                txt_DropMinWeight.Focus();
                            else
                                cbo_BrewingCode.Focus();
                            break;
                        case "txt_AllowMaxWeight":
                            if (string.IsNullOrWhiteSpace(txt_AllowMaxWeight.Text.Trim()))
                                txt_AllowMaxWeight.Focus();
                            else
                                txt_WashSyringeSpan.Focus();
                            break;
                        case "txt_WashSyringeSpan":
                            txt_EvacuateSpan.Focus();
                            break;
                        case "txt_EvacuateSpan":
                            if (string.IsNullOrWhiteSpace(txt_EvacuateSpan.Text.Trim()))
                                txt_EvacuateSpan.Focus();
                            else
                                dtp_BrewingData.Focus();

                            break;
                    }
                }
                else if (sender is ComboBox comboBox)
                {
                    Logger.Info($"C_KeyDown：下拉框 {comboBox.Name} 收到回车键。");
                    switch (comboBox.Name)
                    {
                        case "cbo_AssistantCode":
                            if (string.IsNullOrEmpty(cbo_AssistantCode.Text.Trim()))
                                cbo_AssistantCode.Focus();
                            else
                                txt_SettingConcentration.Focus();
                            break;
                        case "cbo_SyringeType":
                            if (string.IsNullOrWhiteSpace(cbo_SyringeType.Text.Trim()))
                                cbo_SyringeType.Focus();
                            else
                                txt_DropMinWeight.Focus();
                            break;
                        case "cbo_BrewingCode":
                            if (string.IsNullOrWhiteSpace(cbo_BrewingCode.Text.Trim()))
                                cbo_BrewingCode.Focus();
                            else
                                cbo_OriginalBottleNum.Focus();
                            break;
                        case "cbo_OriginalBottleNum":
                            txt_AllowMaxWeight.Focus();
                            break;
                        case "cbo_AbsCode":
                           
                                btn_Save.Focus();
                            break;
                    }
                }
                else if (sender is DateTimePicker)
                {
                    cbo_AbsCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("C_KeyDown：发生异常。", ex);
            }
        }

        private void BottleNumJump()
        {
            try
            {
                var bn = txt_BottleNum.Text.Trim();
                if (string.IsNullOrWhiteSpace(bn)) return;

                string filter = BuildFilter(My_DataBase.BottleData.Bottle_details, bn);
                if (filter == null) return;

                DataRow[] dataRows = My_DataBase.BottleData.Bottle_details.Select(filter);
                if (dataRows.Length > 0)
                {
                    Logger.Info($"BottleNumJump：检测到瓶号 {bn} 重复。");
                    ShowWarn("瓶号重复,请重新输入");
                    txt_BottleNum.Text = null;
                    txt_BottleNum.Focus();
                    return;
                }

                SetBottleDetailsControlsEnabledExcept("txt_BottleNum");

                txt_CurrentWeight.Text = DefaultCurrentWeight;
                txt_DropMinWeight.Text = DefaultDropMinWeight;
                cbo_SyringeType.Text = DefaultSyringeType;
                txt_AllowMaxWeight.Text = DefaultAllowMaxWeight;
                cbo_AbsCode.Text = DefaultDripReserveFirst;
                cbo_AssistantCode.Focus();
                Logger.Info($"BottleNumJump：瓶号 {bn} 处理成功。");
            }
            catch (Exception ex)
            {
                Logger.Error("BottleNumJump：发生异常。", ex);
            }
        }

        private void SettingConcentrationCheck()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_SettingConcentration.Text.Trim()))
                    return;
                if (!double.TryParse(txt_SettingConcentration.Text.Trim(), out double setCon) || setCon <= 0)
                {
                    ShowWarn("请输入大于0的浓度值");
                    txt_SettingConcentration.Text = null;
                    txt_SettingConcentration.Focus();
                    return;
                }
                if (!rdo_3.Checked)
                {
                    DataRow[] drs = My_DataBase.BottleData.Bottle_details.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode}='{cbo_AssistantCode.Text.Trim()}' AND {My_DataBase.BOTTLE_DETAILS.SettingConcentration}={setCon}");
                    if (drs.Length > 0)
                    {
                        Logger.Info($"SettingConcentrationCheck：检测到助剂编码 {cbo_AssistantCode.Text.Trim()} 存在相同浓度 {setCon} 的母液瓶。");
                        ShowWarn("已存在相同浓度的母液瓶,请重新输入");
                        txt_SettingConcentration.Text = null;
                        txt_SettingConcentration.Focus();
                        return;
                    }
                }
                txt_CurrentWeight.Focus();
            }
            catch (Exception ex)
            {
                Logger.Error("SettingConcentrationCheck：发生异常。", ex);
            }
        }

        private void Bottle_Load(object sender, EventArgs e)
        {
            try
            {
                Logger.Info("Bottle_Load：开始加载母液瓶资料表。");
                LoadBrewingCode(sender,e);
                LoadAssistantCode(sender, e);
                LoadABSCodes(sender, e);
                if (My_DataBase.BottleData.Bottle_details == null)
                {
                    Logger.Info("Bottle_Load：Bottle_details为空，跳过加载。");
                    return;
                }
                My_DataBase.BottleData.Bottle_details.DefaultView.Sort = $"{My_DataBase.BOTTLE_DETAILS.BottleNum} ASC";
                DataTable sortedTable = My_DataBase.BottleData.Bottle_details.DefaultView.ToTable();

                dgv_Bottle.BindDataTable(
                    sortedTable,
                    row => new object[]
                    {
                        row[My_DataBase.BOTTLE_DETAILS.BottleNum],
                        row[My_DataBase.BOTTLE_DETAILS.AssistantCode],
                        My_DataBase.AssistantData.Assistant_details.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode}='{row[My_DataBase.BOTTLE_DETAILS.AssistantCode]}'").FirstOrDefault()?["AssistantName"] ?? "",
                        row[My_DataBase.BOTTLE_DETAILS.SettingConcentration],
                        row[My_DataBase.BOTTLE_DETAILS.RealConcentration]
                    },
                    My_DataBase.BOTTLE_DETAILS.BottleNum, My_DataBase.BOTTLE_DETAILS.AssistantCode
                );
                Logger.Info($"Bottle_Load：加载完成，共加载 {sortedTable.Rows.Count} 条数据。");
            }
            catch (Exception ex)
            {
                Logger.Error("Bottle_Load：加载母液瓶资料表异常。", ex);
            }
        }

        private bool Dgv_Bottle_EnterKeyAction()
        {
            if (dgv_Bottle.CurrentCell != null)
            {
                int nextRow = dgv_Bottle.CurrentCell.RowIndex + 1;
                if (nextRow < dgv_Bottle.Rows.Count)
                {
                    dgv_Bottle.CurrentCell = dgv_Bottle.Rows[nextRow].Cells[dgv_Bottle.CurrentCell.ColumnIndex];
                    return true;
                }
            }
            return false;
        }

        private void Dgv_Bottle_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                _isInsert = false;
                if (dgv_Bottle.SelectedRows == null || dgv_Bottle.SelectedRows.Count == 0)
                {
                    ClearControls(this.grp_AssistantDetails.Controls, true);
                    ClearControls(this.grp_BottleDetails.Controls, false);
                    return;
                }

                var assistantCode = dgv_Bottle.SelectedRows[0].Cells[1].Value;
                double currentSettingConcentration = dgv_Bottle.SelectedRows[0].Cells[3].Value == null ||
                    dgv_Bottle.SelectedRows[0].Cells[3].Value == DBNull.Value ?
                    0 : Convert.ToDouble(dgv_Bottle.SelectedRows[0].Cells[3].Value);
                UpdateOriginalBottleNum(assistantCode, currentSettingConcentration);

                var currentBottleNum = dgv_Bottle.SelectedRows[0].Cells[0].Value;
                if (currentBottleNum == null || currentBottleNum == DBNull.Value) return;

                string filter = BuildFilter(My_DataBase.BottleData.Bottle_details, currentBottleNum);
                DataRow[] rows = My_DataBase.BottleData.Bottle_details.Select(filter);
                if (rows.Length == 0) return;
                DataTable dt_bottledetails = rows.CopyToDataTable();
                foreach (DataColumn dc in dt_bottledetails.Columns)
                {
                    string s_name = dc.Caption.ToString();
                    foreach (Control c in this.grp_BottleDetails.Controls)
                    {
                        if (c is TextBox && c.Name.Substring(4) == s_name)
                        {
                            c.Text = dt_bottledetails.Rows[0][dc] is DBNull ? "" : dt_bottledetails.Rows[0][dc].ToString();
                            c.Enabled = c.Name != "txt_BottleNum";
                        }
                        else if (c is ComboBox && c.Name.Substring(4) == s_name)
                        {

                            c.Text = dt_bottledetails.Rows[0][dc] is DBNull ? "" : dt_bottledetails.Rows[0][dc].ToString();

                        }
                        else if (c is DateTimePicker picker && c.Name.Substring(4) == s_name)
                        {
                            if (dt_bottledetails.Rows[0][dc] is DBNull)
                                picker.Value = DateTime.Now;
                            else
                                picker.Value = Convert.ToDateTime(dt_bottledetails.Rows[0][dc]);
                        }
                    }
                }

                Logger.Info($"dgv_Bottle_SelectionChanged：选中瓶号 {currentBottleNum}。");
            }
            catch (Exception ex)
            {
                Logger.Error("dgv_Bottle_SelectionChanged：发生异常。", ex);
            }
        }

        private string BuildFilter(DataTable table, object codeValue)
        {
            if (table == null || codeValue == null) return null;
            bool isString = table.Columns[My_DataBase.BOTTLE_DETAILS.AssistantCode].DataType == typeof(string);
            string codeStr = codeValue.ToString().Replace("'", "''");
            return isString
                ? $"{My_DataBase.BOTTLE_DETAILS.BottleNum} = '{codeStr}'"
                : $"{My_DataBase.BOTTLE_DETAILS.BottleNum} = {codeStr}";
        }



        private void UpdateOriginalBottleNum(object assistantCode, double setCon)
        {
            cbo_OriginalBottleNum.Items.Clear();
            if (assistantCode == null || assistantCode == DBNull.Value) return;
            DataRow[] drs = My_DataBase.BottleData.Bottle_details.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode}='{assistantCode}' AND {My_DataBase.BOTTLE_DETAILS.SettingConcentration} > {setCon}");
            if (drs.Length > 0)
            {
                drs = drs.OrderBy(r => Convert.ToDouble(r[My_DataBase.BOTTLE_DETAILS.SettingConcentration])).ToArray();
                foreach (DataRow dr in drs)
                {
                    if (dr[My_DataBase.BOTTLE_DETAILS.SettingConcentration] != DBNull.Value && Convert.ToDouble(dr[My_DataBase.BOTTLE_DETAILS.SettingConcentration]) > setCon)
                    {
                        cbo_OriginalBottleNum.Items.Add(dr[My_DataBase.BOTTLE_DETAILS.BottleNum].ToString());
                    }
                }
            }
        }

        private void Btn_Insert_Click(object sender, EventArgs e)
        {
            ClearControls(this.grp_AssistantDetails.Controls, true);
            foreach (Control c in this.grp_BottleDetails.Controls)
            {
                if (c is TextBox || c is ComboBox || c is DateTimePicker)
                {
                    c.Enabled = c.Name == "txt_BottleNum";
                    c.Text = null;
                }
            }
            dgv_Bottle.ClearSelection();
            txt_BottleNum.Focus();
            _isInsert = true;
            Logger.Info("btn_Insert_Click：进入新增模式。");
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            foreach (Control c in this.grp_BottleDetails.Controls)
            {
                if ((c is TextBox || c is ComboBox) && c.Name != cbo_OriginalBottleNum.Name && c.Name != txt_WashSyringeSpan.Name && c.Name !=cbo_AbsCode.Name )
                {
                    if (c.Name == cbo_AssistantCode.Name)
                    {
                        if(string.IsNullOrEmpty( txt_AssistantName.Text))
                        {
                            ShowWarn("染助剂代码不存在，请补充后再添加");
                            c.Focus();
                            Logger.Info($"btn_Save_Click：染助剂代码不存在，请补充后再添加，控件 {c.Name} 为空。");
                            return;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(c.Text.Trim()))
                    {
                        ShowWarn("请完善所有资料后再点存档");
                        c.Focus();
                        Logger.Info($"btn_Save_Click：资料未完善，控件 {c.Name} 为空。");
                        return;
                    }
                }
            }

            if (rdo_4.Checked)
            {
                DataRow[] drs = My_DataBase.BottleData.Bottle_details.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{rdo_4.Text}'");
                if (drs.Length > 0)
                {
                    ShowWarn("此助剂只能存在一个母液瓶!");
                    cbo_AssistantCode.Focus();
                    Logger.Info("btn_Save_Click：Water助剂已存在母液瓶，禁止重复添加。");
                    return;
                }
            }

            DialogResult dr = ShowConfirm("是否将实际浓度设定为0，否则将设定浓度也设为实际浓度？");


            var dic = new Dictionary<string, object>
            {
                {My_DataBase.BOTTLE_DETAILS.BottleNum, txt_BottleNum.Text.Trim() },
                {My_DataBase.BOTTLE_DETAILS.AssistantCode, cbo_AssistantCode.Text.Trim() },
                {My_DataBase.BOTTLE_DETAILS.SettingConcentration, txt_SettingConcentration.Text.Trim()},
                {My_DataBase.BOTTLE_DETAILS.RealConcentration, dr == DialogResult.Yes ?  "0" :txt_SettingConcentration.Text.Trim()},
                {My_DataBase.BOTTLE_DETAILS.CurrentWeight, txt_CurrentWeight.Text.Trim()},
                {My_DataBase.BOTTLE_DETAILS.SyringeType, cbo_SyringeType.Text.Trim()},
                {My_DataBase.BOTTLE_DETAILS.DropMinWeight, txt_DropMinWeight.Text.Trim()},
                {My_DataBase.BOTTLE_DETAILS.BrewingCode, cbo_BrewingCode.Text.Trim() },
                {My_DataBase.BOTTLE_DETAILS.AllowMaxWeight, txt_AllowMaxWeight.Text.Trim()},
                {My_DataBase.BOTTLE_DETAILS.BrewingData, dtp_BrewingData.Text.Trim()},
                {My_DataBase.BOTTLE_DETAILS.AbsCode, cbo_AbsCode.Text.Trim()},
                {My_DataBase.BOTTLE_DETAILS.EvacuateSpan, txt_EvacuateSpan.Text.Trim() }

            };
            if (!string.IsNullOrWhiteSpace(cbo_OriginalBottleNum.Text.Trim()))
            {
                dic.Add(My_DataBase.BOTTLE_DETAILS.OriginalBottleNum, cbo_OriginalBottleNum.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(txt_WashSyringeSpan.Text.Trim()))
            {
                dic.Add(My_DataBase.BOTTLE_DETAILS.WashSyringeSpan, txt_WashSyringeSpan.Text.Trim());
            }
            

            try
            {
                if (_isInsert)
                {
                    My_DataBase.SqlServer.Insert(My_DataBase.BOTTLE_DETAILS.TableName, dic);
                    Logger.Info($"btn_Save_Click：新增母液瓶 {txt_BottleNum.Text.Trim()}。");
                }
                else
                {
                    string filter = BuildFilter(My_DataBase.BottleData.Bottle_details, txt_BottleNum.Text);
                    My_DataBase.SqlServer.Update(My_DataBase.BOTTLE_DETAILS.TableName, dic, filter);
                    Logger.Info($"btn_Save_Click：修改母液瓶 {txt_BottleNum.Text.Trim()}。");
                }

                
                Bottle_Load(sender, new EventArgs());

                foreach (DataGridViewRow viewRow in dgv_Bottle.Rows)
                {
                    if (viewRow.Cells[0].Value?.ToString().Trim() == dic[My_DataBase.BOTTLE_DETAILS.BottleNum].ToString().Trim())
                    {
                        viewRow.Selected = true;
                        dgv_Bottle.CurrentCell = viewRow.Cells[0];
                        break;
                    }
                }

                ShowInfo("保存成功!");
                btn_Insert.Focus();
                
            }
            catch (Exception ex)
            {
                Logger.Error("btn_Save_Click：保存母液瓶资料异常。", ex);
            }
        }

        private void Btn_Delete_Click(object sender, EventArgs e)
        {
            if (dgv_Bottle.SelectedRows == null || dgv_Bottle.SelectedRows.Count == 0)
            {
                ShowWarn("请选择要删除的母液瓶!");
                Logger.Info("btn_Delete_Click：未选中任何母液瓶，无法删除。");
                return;
            }
            int currentRowIndex = dgv_Bottle.CurrentCell.RowIndex;
            var bn = txt_BottleNum.Text.Trim();
            if (bn.Length > 0)
            {
                if (ShowConfirm("确认删除此母液瓶吗？") == DialogResult.Yes)
                {
                    try
                    {
                        string filter = BuildFilter(My_DataBase.BottleData.Bottle_details, txt_BottleNum.Text);
                        My_DataBase.SqlServer.Delete(My_DataBase.BOTTLE_DETAILS.TableName, filter);
                        Logger.Info($"btn_Delete_Click：删除母液瓶 {bn}。");
                       
                        Bottle_Load(sender, new EventArgs());
                        if (currentRowIndex < dgv_Bottle.Rows.Count - 1)
                        {
                            dgv_Bottle.Rows[currentRowIndex].Selected = true;
                            dgv_Bottle.CurrentCell = dgv_Bottle.Rows[currentRowIndex].Cells[0];
                        }
                        else
                        {
                            dgv_Bottle.Rows[dgv_Bottle.Rows.Count - 1].Selected = true;
                            dgv_Bottle.CurrentCell = dgv_Bottle.Rows[dgv_Bottle.Rows.Count - 1].Cells[0];
                        }

                       
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("btn_Delete_Click：删除母液瓶异常。", ex);
                    }
                }
            }
        }

        private void Cbo_OriginalBottleNum_Enter(object sender, EventArgs e)
        {
            UpdateOriginalBottleNum(cbo_AssistantCode.Text.Trim(), string.IsNullOrWhiteSpace(txt_SettingConcentration.Text.Trim()) ? 0 : Convert.ToDouble(txt_SettingConcentration.Text.Trim()));
        }

        private void Cbo_AssistantCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataRow[] drs = My_DataBase.AssistantData.Assistant_details.Select(
                    $"{My_DataBase.ASSISTANT_DETAILS.AssistantCode} = '{cbo_AssistantCode.Text.Trim()}'");
                if (drs.Length == 0) return;
                DataTable dt_assistant = drs.CopyToDataTable();
                foreach (DataColumn mDc in dt_assistant.Columns)
                {
                    string s_name = "txt_" + mDc.Caption.ToString();
                    foreach (Control c in this.grp_AssistantDetails.Controls)
                    {
                        if (c is TextBox && c.Name == s_name)
                        {
                            c.Text = dt_assistant.Rows[0][mDc] is DBNull ? "" : dt_assistant.Rows[0][mDc].ToString();
                        }

                        if (s_name == "txt_UnitOfAccount")
                        {
                            string unit = dt_assistant.Rows[0][mDc].ToString();
                            rdo_1.Checked = unit == "%";
                            rdo_2.Checked = unit == "g/l";
                            rdo_3.Checked = unit == "G/L";
                            rdo_4.Checked = !(rdo_1.Checked || rdo_2.Checked || rdo_3.Checked);
                        }
                    }
                }
                Logger.Info($"cbo_AssistantCode_SelectedIndexChanged：选择助剂编码 {cbo_AssistantCode.Text.Trim()}。");
            }
            catch (Exception ex)
            {
                Logger.Error("cbo_AssistantCode_SelectedIndexChanged：发生异常。", ex);
            }
        }

        private void Cbo_AssistantCode_Leave(object sender, EventArgs e)
        {
            Cbo_AssistantCode_SelectedIndexChanged(sender, e);
        }

        private void ShowError(string msg) => LocalTranslator.ShowMessage(msg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowInfo(string msg) => LocalTranslator.ShowMessage(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void ShowWarn(string msg) => LocalTranslator.ShowMessage(msg, "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        private DialogResult ShowConfirm(string msg) => LocalTranslator.ShowMessage(msg, "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
    }
}