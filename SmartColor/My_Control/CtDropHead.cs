using SmartColor.My_DataBase;
using SmartColor.My_File;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;



namespace SmartColor.My_Control
{
    /// <summary>
    /// 染色头部信息输入控件，支持多页面复用（普通/ABS），自动切换数据源、权限控制、数据填充、事件通知等功能。
    /// 使用 SetTableName(tableName) 方法自动推断所有相关表，无需手动赋值多个表名。
    /// 字段名和数据缓存均自动适配表类型，避免混用和频繁数据库访问。
    /// </summary>
    public partial class CtDropHead : UserControl
    {
        #region 公共变量

        /// <summary>
        /// 当布重（txt_ClothWeight）输入框内容发生变化时触发。
        /// 参数为当前布重文本值（string）。
        /// 用于通知外部控件或页面进行相关联的计算或显示更新。
        /// </summary>
        public event EventHandler<string> ClothWeightChanged;

        /// <summary>
        /// 当浴比（txt_BathRatio）输入框内容发生变化时触发。
        /// 参数为当前浴比文本值（string）。
        /// 用于通知外部控件或页面进行相关联的计算或显示更新。
        /// </summary>
        public event EventHandler<string> BathRatioChanged;

        /// <summary>
        /// 当所有输入控件（_inputControls）已输入完毕并按下回车时触发。
        /// 无参数。
        /// 用于通知外部流程可以进行下一步操作（如自动保存、跳转等）。
        /// </summary>
        public event EventHandler LastInputControlEnter;

        /// <summary>
        /// 当染固色代码（txt_DyeingCode）内容发生变化时触发。
        /// 参数为DyeInfo结构体，包含相关数据表、ID、来源等信息。
        /// 用于通知外部控件或页面刷新染固色工艺相关数据。
        /// </summary>
        public event EventHandler<DyeInfo> DyeingCodeChanged;

        /// <summary>
        /// 当配方编码（txt_FormulaCode）发生变化时触发。
        /// 参数为相关的DataTable数据。
        /// 用于通知外部控件或页面刷新配方详情或相关数据。
        /// </summary>
        public event EventHandler<DataTable> FormulaCodeChanged;

        /// <summary>
        /// 当组合名（txt_FormulaGroup）等发生变化时触发。
        /// 参数为相关的DataTable数据。
        /// 用于通知外部控件或页面刷新配方详情或相关数据。
        public event EventHandler<DataTable> FormulaGroupChanged;

        #endregion

        /// <summary>
        /// 模式类型枚举
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// 染色模式
            /// </summary>
            Dye = 0,

            /// <summary>
            /// ABS模式
            /// </summary>
            ABS = 1

        }

        /// <summary>
        /// 数据来源枚举
        /// </summary>
        public enum DataSource
        {
            /// <summary>
            /// 配方表
            /// </summary>
            Formula = 0,
            /// <summary>
            /// 批次表
            /// </summary>
            Batch = 1,
            /// <summary>
            /// 历史表
            /// </summary>
            History = 2
        }


        /// <summary>
        /// 显示来源
        /// 0：配方表
        /// 1：批次表
        /// 2：历史表
        /// </summary>
        public DataSource Source = 0;


        // 数据表名参数化
        private string TableHeadName { get; set; }
        private string TableDetailsName { get; set; }
        private string TableEnabledName { get; set; }
        private string TableFormulaHeadName { get; set; }
        private string TableFormulaDetailsName { get; set; }

        private string TableHistoryDetailName { get; set; }

        // 字段名参数化
        private string FieldFormulaCode { get; set; }
        private string FieldVersionNum { get; set; }
        private string FieldMyID { get; set; }
        private string FieldNote1 { get; set; }
        private string FieldNote2 { get; set; }
        private string FieldNote3 { get; set; }
        private string FieldHeadID { get; set; }
        private string FieldBatchName { get; set; }
        private string FieldCupNum { get; set; }

        // 使能表字段名参数化
        private string FieldEnabledMyID { get; set; }
        private string FieldEnabledNote1Name { get; set; }
        private string FieldEnabledNote2Name { get; set; }
        private string FieldEnabledNote3Name { get; set; }

        private string FieldEnabledNote1Items { get; set; }
        private string FieldEnabledNote2Items { get; set; }
        private string FieldEnabledNote3Items { get; set; }

        // 数据缓存

        private DataTable _enabledCache = null;



        private Control[] _inputControls;
        private bool _isNewAddDyeingCode = false;
        //是否只读
        private bool _isReadOnly = false;



        private int _myID = -1;


        /// <summary>
        /// 染固色代码信息结构体
        /// </summary>
        public struct DyeInfo
        {
            public DataTable DT;
            public int ID;

        }

        public CtDropHead()
        {
            InitializeComponent();
            My_DataBase.SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
            My_DataBase.SqlServer.TableDataChanged += SqlServer_TableDataChanged;


            txt_AddWaterChoose.Enter += (s, e) => { txt_AddWaterChoose.ForeColor = Color.DeepSkyBlue; };
            txt_AddWaterChoose.Leave += (s, e) => { txt_AddWaterChoose.ForeColor = Color.Black; };
            txt_ClothWeight.TextChanged += (s, e) => CalculateTotalWeight(1);
            txt_BathRatio.TextChanged += (s, e) => CalculateTotalWeight(2);
            this.txt_FormulaCode.TextChanged += Txt_FormulaCode_TextChanged;
            this.lstFormulaSuggest.Click += LstFormulaSuggest_Click;
            this.lstFormulaSuggest.Leave += LstFormulaSuggest_Leave;
            this.txt_FormulaCode.KeyDown += Txt_FormulaCode_KeyDown;
            this.lstFormulaSuggest.KeyDown += LstFormulaSuggest_KeyDown;

            Logger.Info("CtDropHead 初始化完成。");
        }

        private void Txt_FormulaCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (lstFormulaSuggest.Visible && lstFormulaSuggest.Items.Count > 0)
            {
                if (e.KeyCode == Keys.Down)
                {
                    lstFormulaSuggest.Focus();
                    if (lstFormulaSuggest.SelectedIndex < 0 && lstFormulaSuggest.Items.Count > 0)
                        lstFormulaSuggest.SelectedIndex = 0;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Up)
                {
                    lstFormulaSuggest.Focus();
                    if (lstFormulaSuggest.SelectedIndex < 0 && lstFormulaSuggest.Items.Count > 0)
                        lstFormulaSuggest.SelectedIndex = lstFormulaSuggest.Items.Count - 1;
                    e.Handled = true;
                }
            }
        }

        // lstFormulaSuggest 的 KeyDown 事件
        private void LstFormulaSuggest_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && lstFormulaSuggest.SelectedItem != null)
            {
                txt_FormulaCode.Text = lstFormulaSuggest.SelectedItem.ToString();
                lstFormulaSuggest.Visible = false;
                txt_FormulaCode.Focus();
                txt_FormulaCode.SelectionStart = txt_FormulaCode.Text.Length;
                // 这里手动调用
                Txt_FormulaCode_Leave(txt_FormulaCode, EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                lstFormulaSuggest.Visible = false;
                txt_FormulaCode.Focus();
                e.Handled = true;
            }
        }

        private void LstFormulaSuggest_Leave(object sender, EventArgs e)
        {
            lstFormulaSuggest.Visible = false;
        }

        private void Txt_FormulaCode_TextChanged(object sender, EventArgs e)
        {
            // 只在控件获得焦点时（即用户手动输入时）弹出建议
            if (!txt_FormulaCode.Focused )
            {
                lstFormulaSuggest.Visible = false;
                return;
            }

            string input = txt_FormulaCode.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                lstFormulaSuggest.Visible = false;
                return;
            }

            // 模糊查询配方表
            var dt = My_DataBase.SqlServer.Select(
                TableFormulaHeadName,
                $"{FieldFormulaCode} LIKE @code",
                new System.Data.SqlClient.SqlParameter("@code", "%" + input + "%")
            );

            lstFormulaSuggest.Items.Clear();
            if (dt != null && dt.Rows.Count > 0)
            {
                var codes = new HashSet<string>();
                foreach (DataRow row in dt.Rows)
                {
                    var code = row[FieldFormulaCode]?.ToString();
                    if (!string.IsNullOrWhiteSpace(code) && codes.Add(code))
                    {
                        lstFormulaSuggest.Items.Add(code);
                    }
                }
                lstFormulaSuggest.Visible = lstFormulaSuggest.Items.Count > 0;
                lstFormulaSuggest.Width = txt_FormulaCode.Width;
                lstFormulaSuggest.Left = txt_FormulaCode.Left;
                lstFormulaSuggest.Top = txt_FormulaCode.Bottom;
                lstFormulaSuggest.BringToFront();
            }
            else
            {
                lstFormulaSuggest.Visible = false;
            }
        }

        // 选择建议项
        private void LstFormulaSuggest_Click(object sender, EventArgs e)
        {
            if (lstFormulaSuggest.SelectedItem != null)
            {
                txt_FormulaCode.Text = lstFormulaSuggest.SelectedItem.ToString();
                lstFormulaSuggest.Visible = false;
                txt_FormulaCode.Focus();
                txt_FormulaCode.SelectionStart = txt_FormulaCode.Text.Length;
                // 可手动触发失去焦点事件
                Txt_FormulaCode_Leave(txt_FormulaCode, EventArgs.Empty);
            }
        }

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                My_DataBase.SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void SqlServer_TableDataChanged(string obj)
        {
            if (obj == My_DataBase.FORMULA_GROUP.TableName)
            {
                LoadFormulaGroupItems();
            }
            else if (obj == My_DataBase.DYEING_CODE.TableName)
            {
                LoadDPPItems();
            }
            else if (obj == My_DataBase.ENABLED_SET.TableName || obj == My_DataBase.ABS_ENABLED_SET.TableName)
            {
                _enabledCache = null;
                ApplyEnabledSet();
                UpdateNote();
            }


        }

        /// <summary>
        /// 设置模式
        /// </summary>
        /// <param name="headSource">模式</param>
        public void SetMode(Mode headSource)
        {
            // 普通染色表
            if (headSource == Mode.Dye)
            {
                TableHeadName = My_DataBase.DROP_HEAD.TableName;
                TableDetailsName = My_DataBase.DROP_DETAILS.TableName;
                TableEnabledName = My_DataBase.ENABLED_SET.TableName;
                TableFormulaHeadName = My_DataBase.FORMULA_HEAD.TableName;
                TableFormulaDetailsName = My_DataBase.FORMULA_DETAILS.TableName;
                TableHistoryDetailName = My_DataBase.HISTORY_DETAILS.TableName;



                FieldFormulaCode = My_DataBase.FORMULA_HEAD.FormulaCode;
                FieldVersionNum = My_DataBase.FORMULA_HEAD.VersionNum;
                FieldMyID = My_DataBase.FORMULA_HEAD.MyID;
                FieldNote1 = My_DataBase.FORMULA_HEAD.Note1;
                FieldNote2 = My_DataBase.FORMULA_HEAD.Note2;
                FieldNote3 = My_DataBase.FORMULA_HEAD.Note3;
                FieldHeadID = My_DataBase.DROP_DETAILS.HeadID;
                FieldBatchName = My_DataBase.HISTORY_HEAD.BatchName;
                FieldCupNum = My_DataBase.HISTORY_HEAD.CupNum;

                FieldEnabledMyID = My_DataBase.ENABLED_SET.MyID;
                FieldEnabledNote1Name = My_DataBase.ENABLED_SET.Note1Name;
                FieldEnabledNote2Name = My_DataBase.ENABLED_SET.Note2Name;
                FieldEnabledNote3Name = My_DataBase.ENABLED_SET.Note3Name;
                FieldEnabledNote1Items = My_DataBase.ENABLED_SET.Note1Items;
                FieldEnabledNote2Items = My_DataBase.ENABLED_SET.Note2Items;
                FieldEnabledNote3Items = My_DataBase.ENABLED_SET.Note3Items;

                _enabledCache = My_DataBase.EnabledData.Enabled_set;



            }
            // ABS染色表
            else if (headSource == Mode.ABS)
            {
                TableHeadName = My_DataBase.ABS_DROP_HEAD.TableName;
                TableDetailsName = My_DataBase.ABS_DROP_DETAILS.TableName;
                TableEnabledName = My_DataBase.ABS_ENABLED_SET.TableName;
                TableFormulaHeadName = My_DataBase.ABS_FORMULA_HEAD.TableName;
                TableFormulaDetailsName = My_DataBase.ABS_FORMULA_DETAILS.TableName;
                TableHistoryDetailName = My_DataBase.ABS_HISTORY_DETAILS.TableName;

                FieldFormulaCode = My_DataBase.ABS_FORMULA_HEAD.FormulaCode;
                FieldVersionNum = My_DataBase.ABS_FORMULA_HEAD.VersionNum;
                FieldMyID = My_DataBase.ABS_FORMULA_HEAD.MyID;
                FieldNote1 = My_DataBase.ABS_FORMULA_HEAD.Note1;
                FieldNote2 = My_DataBase.ABS_FORMULA_HEAD.Note2;
                FieldNote3 = My_DataBase.ABS_FORMULA_HEAD.Note3;
                FieldHeadID = My_DataBase.ABS_DROP_DETAILS.HeadID;
                FieldBatchName = My_DataBase.ABS_HISTORY_HEAD.BatchName;
                FieldCupNum = My_DataBase.ABS_HISTORY_HEAD.CupNum;

                FieldEnabledMyID = My_DataBase.ABS_ENABLED_SET.MyID;
                FieldEnabledNote1Name = My_DataBase.ABS_ENABLED_SET.Note1Name;
                FieldEnabledNote2Name = My_DataBase.ABS_ENABLED_SET.Note2Name;
                FieldEnabledNote3Name = My_DataBase.ABS_ENABLED_SET.Note3Name;
                FieldEnabledNote1Items = My_DataBase.ABS_ENABLED_SET.Note1Items;
                FieldEnabledNote2Items = My_DataBase.ABS_ENABLED_SET.Note2Items;
                FieldEnabledNote3Items = My_DataBase.ABS_ENABLED_SET.Note3Items;


                _enabledCache = My_DataBase.ABSEnabledData.ABS_Enabled_set;


            }
            // 清空缓存
            else
            {
                TableHeadName = TableDetailsName = TableEnabledName = TableFormulaHeadName = TableFormulaDetailsName = null;
                FieldFormulaCode = FieldVersionNum = FieldMyID = FieldNote1 = FieldNote2 = FieldNote3 = null;
                FieldHeadID = FieldBatchName = FieldCupNum = null;
                FieldEnabledMyID = FieldEnabledNote1Name = FieldEnabledNote2Name = FieldEnabledNote3Name = null;
                _enabledCache = null;
            }

            InitInputControls();
            UpdateNote();

            BindKeyDownEvents();

            LoadFormulaGroupItems();
            LoadDPPItems();
            ApplyEnabledSet();
            BindLabelClickEvents();

        }

        /// <summary>
        /// 计算总浴量（布重*浴比），并触发相关事件
        /// </summary>
        /// <param name="type">1=布重变化，2=浴比变化</param>
        private void CalculateTotalWeight(int type)
        {
            if (type == 1)
            {
                if (string.IsNullOrEmpty(txt_ClothWeight.Text))
                    return;
                else
                {
                    if (!double.TryParse(txt_ClothWeight.Text, out double cw1))
                    {
                        LocalTranslator.ShowMessage("布重输入有误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (cw1 > My_ConPar.Other.ClothAlarmWeight)
                    {

                        LocalTranslator.ShowMessage($"布重输入有误,{cw1} >{My_ConPar.Other.ClothAlarmWeight}！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txt_ClothWeight.Text = string.Empty;
                        return;
                    }
                }
            }
            if (string.IsNullOrEmpty(txt_BathRatio.Text) || string.IsNullOrEmpty(txt_ClothWeight.Text)) return;
            int roundDigits = My_Tool.BalanceStableReading.RetainDecimals();
            double cw = Convert.ToDouble(txt_ClothWeight.Text);
            double br = Convert.ToDouble(txt_BathRatio.Text);
            txt_TotalWeight.Text = Math.Round(cw * br, roundDigits).ToString();
            if (type == 1)
                ClothWeightChanged?.Invoke(this, txt_ClothWeight.Text);
            else
            {
                if (My_ConPar.Choices.BathRatioTxtDyBath == 1)
                    BathRatioChanged?.Invoke(this, txt_BathRatio.Text);
            }

        }

        /// <summary>
        /// 更新备注区（根据使能表自动切换），备注字段名和内容均用变量控制
        /// </summary>
        private void UpdateNote()
        {
            if (_enabledCache == null)
                _enabledCache = My_DataBase.SqlServer.Select(TableEnabledName);
            var dt = _enabledCache;
            if (dt == null || dt.Rows.Count == 0) return;
            var row = dt.Rows[0];

            // 备注字段名和内容均用变量
            string note1N = FieldEnabledNote1Name;
            string note2N = FieldEnabledNote2Name;
            string note3N = FieldEnabledNote3Name;

            UpdateNoteControl(lab_Note1, row[note1N]?.ToString(), txt_Note1, cbo_Note1, row[FieldEnabledNote1Items]?.ToString());
            UpdateNoteControl(lab_Note2, row[note2N]?.ToString(), txt_Note2, cbo_Note2, row[FieldEnabledNote2Items]?.ToString());
            UpdateNoteControl(lab_Note3, row[note3N]?.ToString(), txt_Note3, cbo_Note3, row[FieldEnabledNote3Items]?.ToString());
        }

        /// <summary>
        /// 填充备注控件的值（字段名自动适配，全部用变量）
        /// </summary>
        private void FillNoteControlsFromDataRow(DataRow row)
        {
            FillNoteControl(txt_Note1, cbo_Note1, row.Table.Columns.Contains(FieldNote1) ? row[FieldNote1]?.ToString() : "");
            FillNoteControl(txt_Note2, cbo_Note2, row.Table.Columns.Contains(FieldNote2) ? row[FieldNote2]?.ToString() : "");
            FillNoteControl(txt_Note3, cbo_Note3, row.Table.Columns.Contains(FieldNote3) ? row[FieldNote3]?.ToString() : "");
        }

        private void FillNoteControl(TextBox txt, ComboBox cbo, string value)
        {
            if (cbo.Visible)
            {
                if (cbo.Items.Contains(value))
                    cbo.Text = value;
                else
                    cbo.Text = "";
            }
            else
            {
                txt.Text = value ?? "";
            }
        }

        /// <summary>
        /// 更新备注控件的显示和下拉内容（全部用变量控制）
        /// </summary>
        private void UpdateNoteControl(Label label, string name, TextBox txt, ComboBox cbo, string items)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateNoteControl(label, name, txt, cbo, items)));
                return;
            }
            // 标签文本用变量
            if (label == lab_Note1)
                label.Text = string.IsNullOrWhiteSpace(name) ? "备注1：" : name + ":";
            else if (label == lab_Note2)
                label.Text = string.IsNullOrWhiteSpace(name) ? "备注2：" : name + ":";
            else if (label == lab_Note3)
                label.Text = string.IsNullOrWhiteSpace(name) ? "备注3：" : name + ":";

            // 下拉内容用变量
            if (!string.IsNullOrWhiteSpace(items))
            {
                var arr = items.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                cbo.Items.Clear();
                foreach (var item in arr)
                    cbo.Items.Add(item);
                cbo.Visible = true;
                txt.Visible = false;
            }
            else
            {
                cbo.Visible = false;
                txt.Visible = true;
            }
        }

        /// <summary>
        /// 加载配方组合下拉框内容（全部用变量控制字段名）
        /// </summary>
        private void LoadFormulaGroupItems()
        {
            var dt = My_DataBase.FormulaGradeData.Formula_group;
            if (dt == null || dt.Rows.Count == 0)
            {
                Logger.Info("LoadFormulaGroupItems: 组合名数据为空。");
                return;
            }
            txt_FormulaGroup.Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                var name = row[My_DataBase.FORMULA_GROUP.GroupName]?.ToString();
                if (!string.IsNullOrWhiteSpace(name) && !txt_FormulaGroup.Items.Contains(name))
                {
                    txt_FormulaGroup.Items.Add(name);
                }
            }
            Logger.Info("LoadFormulaGroupItems: 组合名下拉框已刷新。");
        }

        /// <summary>
        /// 加载染固色工艺下拉框内容（全部用变量控制字段名）
        /// </summary>
        private void LoadDPPItems()
        {
            if (TableHeadName != My_DataBase.DROP_HEAD.TableName) return;
            var dt = My_DataBase.DyeingCodeData.Dyeing_code;
            if (dt == null || dt.Rows.Count == 0)
            {
                Logger.Info("LoadDPPItems: 染固色工艺数据为空。");
                return;
            }
            txt_DyeingCode.Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                var name = row[My_DataBase.DYEING_CODE.DyeingCode]?.ToString();
                if (!string.IsNullOrWhiteSpace(name) && !txt_DyeingCode.Items.Contains(name))
                {
                    txt_DyeingCode.Items.Add(name);
                }
            }
            Logger.Info("LoadDPPItems: 染固色工艺下拉框已刷新。");
        }

        /// <summary>
        /// 初始化输入控件数组（全部用变量控制）
        /// </summary>
        private void InitInputControls()
        {
            _inputControls = new Control[]
            {
                txt_FormulaCode,
                txt_FormulaName,
                txt_ClothType,
                txt_Customer,
                txt_CupNum,
                txt_ClothNum,

                txt_AddWaterChoose,
                txt_ClothWeight,
                txt_BathRatio,
                txt_FormulaGroup,
                txt_AnhydrationWR,
                txt_Non_AnhydrationWR,
                txt_DyeingCode,
                txt_Note1,
                cbo_Note1,
                txt_Note2,
                cbo_Note2,
                txt_Note3,
                cbo_Note3
            };
        }

        /// <summary>
        /// 绑定输入控件的回车事件（全部用变量控制）
        /// </summary>
        private void BindKeyDownEvents()
        {
            foreach (var ctrl in _inputControls)
            {
                ctrl.KeyDown += InputControl_KeyDown;
            }
        }

        /// <summary>
        /// 输入控件回车事件处理，自动跳转下一个可编辑控件（全部用变量控制）
        /// </summary>
        private void InputControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            var current = sender as Control;
            if ((current is TextBox tb && string.IsNullOrWhiteSpace(tb.Text)) ||
                (current is ComboBox combo && string.IsNullOrWhiteSpace(combo.Text) && 
                combo.Name != txt_DyeingCode.Name && combo.Name != txt_FormulaGroup.Name))
            {

                e.Handled = true;
                return;
            }
            int idx = Array.IndexOf(_inputControls, current);
            for (int i = idx + 1; i < _inputControls.Length; i++)
            {
                var nextCtrl = _inputControls[i];
                if (nextCtrl == txt_Note1 || nextCtrl == txt_Note2 || nextCtrl == txt_Note3)
                {
                    ComboBox cbo = null;
                    if (nextCtrl == txt_Note1) cbo = cbo_Note1;
                    if (nextCtrl == txt_Note2) cbo = cbo_Note2;
                    if (nextCtrl == txt_Note3) cbo = cbo_Note3;
                    if (cbo != null && cbo.Visible && cbo.Enabled)
                    {
                        cbo.Focus();
                        e.Handled = true;
                        return;
                    }
                    else if (nextCtrl.Visible && nextCtrl is TextBox tbNote && !tbNote.ReadOnly)
                    {
                        nextCtrl.Focus();
                        e.Handled = true;
                        return;
                    }
                }
                else if (nextCtrl.Visible)
                {
                    if (nextCtrl is TextBox tbOther)
                    {
                        if (!tbOther.ReadOnly)
                        {
                            nextCtrl.Focus();
                            e.Handled = true;
                            return;
                        }
                    }
                    else
                    {
                        if (nextCtrl.Enabled)
                        {
                            nextCtrl.Focus();
                            e.Handled = true;
                            return;
                        }
                    }
                }
            }
            LastInputControlEnter?.Invoke(this, EventArgs.Empty);
            Logger.Info("InputControl_KeyDown: 已到最后一个输入控件，触发 LastInputControlEnter 事件。");
            e.Handled = true;
        }

        /// <summary>
        /// 绑定标签点击事件，切换控件使能状态（全部用变量控制）
        /// </summary>
        private void BindLabelClickEvents()
        {
            var labelMap = new Dictionary<Control, string>
            {
                { lab_FormulaName, txt_FormulaName.Name },
                { lab_ClothType, txt_ClothType.Name },
                { lab_Customer, txt_Customer.Name },
                { lab_CupNum, txt_CupNum.Name },
                { lab_ClothNum, txt_ClothNum.Name },
                { lab_ClothWeight, txt_ClothWeight.Name },
                { lab_BathRatio, txt_BathRatio.Name },
                { lab_FormulaGroup, txt_FormulaGroup.Name },
                { lab_DyeingCode, txt_DyeingCode.Name },
                { lab_AnhydrationWR, txt_AnhydrationWR.Name },
                { lab_Non_AnhydrationWR, txt_Non_AnhydrationWR.Name },
                { lab_Note1, cbo_Note1.Name },
                { lab_Note2, cbo_Note2.Name },
                { lab_Note3, cbo_Note3.Name },

            };



            foreach (var kv in labelMap)
            {
                kv.Key.Click += (s, e) => ToggleControlEnabled(kv.Value);
            }
        }

        public void FocusFormulaName()
        {
            txt_FormulaName.Focus();
            if (txt_FormulaName is TextBox tb)
            {
                tb.SelectAll();
            }
        }

        /// <summary>
        /// 切换控件使能状态（全部用变量控制字段名和表名）
        /// </summary>
        private void ToggleControlEnabled(string controlName)
        {
            if (this._isReadOnly) return;
            if (!My_Form.Login.LoginForm.UserCache.TryGetValue(Properties.Settings.Default.Account, out var userInfo) || userInfo.Purview != 2)
            {
                Logger.Info("ToggleControlEnabled: 当前用户无权限切换控件状态。");
                LocalTranslator.ShowMessage("无权限操作！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var ctrl = FindControlByName(controlName);
            if (ctrl == null)
            {
                Logger.Error($"ToggleControlEnabled: 未找到控件 {controlName}。");
                return;
            }
            bool enabled;
            if (ctrl is TextBox tb)
            {
                tb.ReadOnly = !tb.ReadOnly;
                tb.Enabled = true;
                enabled = !tb.ReadOnly;
            }
            else
            {
                ctrl.Enabled = !ctrl.Enabled;
                enabled = ctrl.Enabled;
            }
            if (controlName == cbo_Note1.Name)
            {
                controlName = txt_Note1.Name;
            }
            else if (controlName == cbo_Note2.Name)
            {
                controlName = txt_Note2.Name;
            }
            else if (controlName == cbo_Note3.Name)
            {
                controlName = txt_Note3.Name;
            }

            var data = new Dictionary<string, object>
            {
                { controlName, enabled ? 1 : 0 }
            };
            try
            {
                SmartColor.My_DataBase.SqlServer.Update(TableEnabledName, data, $"{FieldEnabledMyID}=@MyID", new System.Data.SqlClient.SqlParameter("@MyID", 1));
                Logger.Info($"ToggleControlEnabled: 控件 {controlName} 状态已切换为 {(enabled ? "启用" : "禁用")} 并写入数据库。");
            }
            catch (Exception ex)
            {
                Logger.Error($"ToggleControlEnabled: 控件 {controlName} 状态写入数据库异常。", ex);
                LocalTranslator.ShowMessage("控件状态写入数据库失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _enabledCache = null;
            ApplyEnabledSet();
        }

        /// <summary>
        /// 应用使能表设置到控件（全部用变量控制字段名和表名）
        /// </summary>
        private void ApplyEnabledSet()
        {
            var dt = _enabledCache;
            if (dt == null || dt.Rows.Count == 0)
            {
                Logger.Info("ApplyEnabledSet: 使能数据为空。");
                return;
            }
            var row = dt.Rows[0];

            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName == FieldEnabledMyID) continue;
                var ctrl = FindControlByName(col.ColumnName);
                if (ctrl == null) continue;
                bool enabled = false;
                if (row[col.ColumnName] != null && byte.TryParse(row[col.ColumnName].ToString(), out var b))
                    enabled = b == 1;
                if (col.ColumnName == "txt_Note1" || col.ColumnName == "txt_Note2" || col.ColumnName == "txt_Note3")
                {
                    var txt = FindControlByName(col.ColumnName) as TextBox;
                    var cbo = FindControlByName(col.ColumnName.Replace("txt_", "cbo_")) as ComboBox;
                    if (txt != null)
                    {
                        txt.ReadOnly = !enabled;
                        txt.Enabled = true;
                    }
                    if (cbo != null)
                    {
                        cbo.Enabled = enabled;
                    }
                }
                else if (ctrl is TextBox tb)
                {
                    tb.ReadOnly = !enabled;
                    tb.Enabled = true;
                }
                else
                {
                    ctrl.Enabled = enabled;
                }
            }
            Logger.Info("ApplyEnabledSet: 控件使能状态已应用。");
        }

        /// <summary>
        /// 根据控件名查找控件（全部用变量控制）
        /// </summary>
        private Control FindControlByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (name == "cbo_Note1" || name == "cbo_Note2" || name == "cbo_Note3")
            {
                var cbo = this.Controls.Find(name, true).FirstOrDefault() as ComboBox;
                if (cbo.Items.Count == 0)
                {
                    var txt = this.Controls.Find(name.Replace("cbo_", "txt_"), true).FirstOrDefault() as TextBox;
                    return txt ?? (Control)cbo;
                }
                else
                {
                    return cbo;
                }

            }
            return this.Controls.Find(name, true).FirstOrDefault();
        }

        /// <summary>
        /// 填充表头资料（自动切换表，字段名自动适配，数据缓存优先，全部用变量控制）
        /// </summary>
        public void FillControlsFromDataTable(DataTable dt, DataSource i = DataSource.Formula)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                Logger.Info("FillControlsFromDataTable: 数据表为空。");
                return;
            }
            FillNoteControlsFromDataRow(dt.Rows[0]);
            _myID = -1;
            this.Source = i;
            GetDetailData(dt);
            ClearInputControls();
            var row = dt.Rows[0];
            foreach (DataColumn col in dt.Columns)
            {
                var ctrl = FindControlByName("txt_" + col.ColumnName);
                if (ctrl == null) continue;
                switch (ctrl)
                {
                    case TextBox tb:
                        tb.Text = row[col.ColumnName]?.ToString() ?? "";
                        break;
                    case ComboBox cb:
                        if (cb.Items.Contains(row[col.ColumnName]?.ToString()))
                            cb.Text = row[col.ColumnName]?.ToString() ?? "";
                        else
                        {
                            if (cb.Name == txt_FormulaGroup.Name)
                            {
                                cb.Text = "";
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(row[col.ColumnName]?.ToString()))
                                {
                                    cb.Items.Add(row[col.ColumnName]?.ToString() ?? "");
                                    cb.Text = row[col.ColumnName]?.ToString() ?? "";

                                }

                                else
                                {
                                    cb.Text = "";
                                    DyeInfo dyeInfo = new DyeInfo
                                    {
                                        DT = new DataTable(),

                                    };
                                    DyeingCodeChanged?.Invoke(this, dyeInfo);
                                }
                            }
                        }
                        break;
                    case CheckBox chk:
                        var val = row[col.ColumnName];
                        chk.Checked = val != null && (val.ToString() == "1" || val.ToString().ToLower() == "true");
                        break;
                }
            }
            Logger.Info("FillControlsFromDataTable: 控件数据已填充。");
            //txt_FormulaCode.ReadOnly = true;
        }

        /// <summary>
        /// 获取所有输入控件的值（全部用变量控制）
        /// </summary>
        public Dictionary<string, object> GetAllInputValues()
        {
            var dict = new Dictionary<string, object>();
            foreach (var ctrl in this.Controls)
            {
                switch (ctrl)
                {
                    case TextBox tb:
                        dict[((TextBox)ctrl).Name] = tb.Text;
                        break;
                    case ComboBox cb:
                        dict[((ComboBox)ctrl).Name] = cb.Text;
                        break;
                    case CheckBox chk:
                        dict[((CheckBox)ctrl).Name] = chk.Checked;
                        break;
                }
            }
            return dict;
        }

        /// <summary>
        /// 清空所有输入控件（全部用变量控制）
        /// </summary>
        private void ClearInputControls()
        {
            foreach (Control ctrl in this.Controls)
            {
                switch (ctrl)
                {
                    case TextBox tb when tb.Name != txt_FormulaCode.Name:
                        tb.Text = "";
                        break;
                    case ComboBox cb:
                        cb.Text = "";
                        break;
                    case CheckBox chk:
                        chk.Checked = false;
                        break;
                }
            }
            Logger.Info("ClearInputControls: 输入控件已清空。");
        }

        /// <summary>
        /// 配方编码控件失去焦点时，自动填充相关数据（自动切换表，字段名自动适配，缓存优先，全部用变量控制）
        /// </summary>
        private void Txt_FormulaCode_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_FormulaCode.Text))
            {
                txt_FormulaCode.Focus();
                return;
            }

            if (!lstFormulaSuggest.Focused)
                lstFormulaSuggest.Visible = false;

            // 直接查数据库，不用缓存
            var dt = SqlServer.Select(
                TableFormulaHeadName,
                $"{FieldFormulaCode} = @FormulaCode",
                new System.Data.SqlClient.SqlParameter("@FormulaCode", txt_FormulaCode.Text)
            );

            DataTable head = null;
            if (dt != null && dt.Rows.Count > 0)
            {
                // 按版本号降序取最新一条
                var rows = dt.AsEnumerable()
                    .OrderByDescending(r =>
                    {
                        int v = 0;
                        int.TryParse(r[FieldVersionNum]?.ToString(), out v);
                        return v;
                    })
                    .Take(1);

                if (rows.Any())
                {
                    head = rows.CopyToDataTable();
                    FillControlsFromDataTable(head);
                }

            }

        }

        /// <summary>
        /// 获取详情数据（自动切换表，字段名自动适配，缓存优先，全部用变量控制）
        /// </summary>
        private void GetDetailData(DataTable head = null)
        {
            DataTable detail = null;
            if (this.Source == 0)
            {
                string foormulaCode = head != null && head.Rows.Count > 0
                  ? head.Rows[0][FieldFormulaCode]?.ToString()
                  : null;
                string verNum = head != null && head.Rows.Count > 0
                  ? head.Rows[0][FieldVersionNum]?.ToString()
                  : null;

                detail = SqlServer.Select(TableFormulaDetailsName,
                    $"{FieldFormulaCode} = '{foormulaCode}' AND {FieldVersionNum} = {verNum}");

            }
            else if (this.Source == DataSource.Batch)
            {
                if (head != null && head.Rows.Count > 0)
                {
                    this._myID = head != null && head.Rows.Count > 0
                     ? Convert.ToInt32(head.Rows[0][FieldMyID])
                     : -1;


                    detail = SqlServer.Select(TableDetailsName,
                        $"{FieldHeadID} = {this._myID}");

                }
            }
            else if (this.Source == DataSource.History)
            {
                if (head != null && head.Rows.Count > 0)
                {
                    this._myID = head != null && head.Rows.Count > 0
                     ? Convert.ToInt32(head.Rows[0][FieldHeadID])
                     : -1;
                    //历史表通过批次名和杯号定位
                    var batchName = head.Rows[0][FieldBatchName]?.ToString();
                    var cupNum = head.Rows[0][FieldCupNum]?.ToString();



                    detail = SqlServer.Select(TableHistoryDetailName,
                        $"{FieldBatchName} = '{batchName}' AND {FieldCupNum} = {cupNum}");

                }
            }

            FormulaCodeChanged?.Invoke(this, detail);
            Logger.Info("Txt_FormulaCode_Leave: 配方编码数据已填充。");
        }

        private void Txt_FormulaGroup_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // 优先用 SelectedItem，避免 Text 为空
            string selectedGroup = txt_FormulaGroup.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selectedGroup))
                return;

            var dt = My_DataBase.FormulaGradeData.Formula_group;
            if (dt == null) return;

            var rows = dt.AsEnumerable().Where(r =>
                r[My_DataBase.FORMULA_GROUP.GroupName]?.ToString() == selectedGroup);

            DataTable detail = (rows != null && rows.Any())
                ? rows.CopyToDataTable()
                : new DataTable();

            FormulaGroupChanged?.Invoke(this, detail);
            Logger.Info("Txt_FormulaGroup_SelectionChangeCommitted: 组合名数据已填充。");
        }

        private void Txt_DyeingCode_TextChanged(object sender, EventArgs e)
        {
            if (this._isNewAddDyeingCode) return;
            if (TableHeadName != My_DataBase.DROP_HEAD.TableName) return;
            var text = txt_DyeingCode.Text;

            DataTable detail = null;

            var dt = My_DataBase.DyeingCodeData.Dyeing_code;
            if (dt != null)
            {
                var dr = dt.AsEnumerable().Where(r =>
                    r[My_DataBase.DYEING_CODE.DyeingCode]?.ToString() == text)
                    .OrderBy(r => Convert.ToInt32(r[My_DataBase.DYEING_CODE.IndexNum]));
                if (dr != null && dr.Any())
                    detail = dr.CopyToDataTable();
            }
            if (detail == null)
            {
                dt = My_DataBase.DyeingCodeData.History_Dyeing_code;
                if (dt == null) return;
                var dr = dt.AsEnumerable().Where(r =>
                    r[My_DataBase.HISTORY_DYEING_CODE.DyeingCode]?.ToString() == text)
                    .OrderBy(r => Convert.ToInt32(r[My_DataBase.HISTORY_DYEING_CODE.IndexNum]));
                detail = (dr != null && dr.Any()) ? dr.CopyToDataTable() : new DataTable();
            }

            DyeInfo detailInfo = new DyeInfo
            {
                DT = detail,
                ID = this._myID,

            };

            DyeingCodeChanged?.Invoke(this, detailInfo);
            Logger.Info("txt_DyeingCode_TextChanged: 染固色代码数据已填充。");
        }

        public void SetDyeingCode(string code)
        {
            this._isNewAddDyeingCode = true;
            this.txt_DyeingCode.Text = code;
            this._isNewAddDyeingCode = false;
        }

        public void FocusFormulaCode()
        {
           // txt_FormulaCode.ReadOnly = false;
            txt_FormulaCode.Focus();
            if (txt_FormulaCode is TextBox tb)
            {
                tb.SelectAll();
            }
        }

        public void FocusNext()
        {
            for (int i = 1; i < _inputControls.Length; i++)
            {
                var nextCtrl = _inputControls[i];
                if (nextCtrl == txt_CupNum)
                {
                    continue;
                }
                if (nextCtrl == txt_Note1 || nextCtrl == txt_Note2 || nextCtrl == txt_Note3)
                {
                    ComboBox cbo = null;
                    if (nextCtrl == txt_Note1) cbo = cbo_Note1;
                    if (nextCtrl == txt_Note2) cbo = cbo_Note2;
                    if (nextCtrl == txt_Note3) cbo = cbo_Note3;
                    if (cbo != null && cbo.Visible && cbo.Enabled)
                    {
                        cbo.Focus();
                        return;
                    }
                    else if (nextCtrl.Visible && nextCtrl is TextBox tbNote && !tbNote.ReadOnly)
                    {
                        nextCtrl.Focus();
                        return;
                    }
                }
                else if (nextCtrl.Visible)
                {
                    if (nextCtrl is TextBox tbOther)
                    {
                        if (!tbOther.ReadOnly)
                        {
                            nextCtrl.Focus();
                            return;
                        }
                    }
                    else
                    {
                        if (nextCtrl.Enabled)
                        {
                            nextCtrl.Focus();
                            return;
                        }
                    }
                }
            }
        }

        public void ClearFormulaCode()
        {
            //txt_FormulaCode.ReadOnly = true;
        }

        public void ReadOnly()
        {
            this._isReadOnly = true;
            foreach (var ctrl in _inputControls)
            {
                if (ctrl is TextBox tb)
                {
                    tb.ReadOnly = true;
                    tb.Enabled = true;
                }
                else
                {
                    ctrl.Enabled = false;
                }
            }
        }
    }
}