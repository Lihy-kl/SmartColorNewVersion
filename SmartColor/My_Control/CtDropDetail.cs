using Microsoft.VisualBasic;
using SmartColor.My_AutomaticModule;
using SmartColor.My_DataBase;
using SmartColor.My_File; // 引入Logger
using SmartColor.My_Form.BasicData;
using SmartColor.My_Form.DyeingMan;
using SmartColor.My_Form.HistoricalData;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.Web.UI.DataVisualization.Charting;
using System.Windows.Forms;
using static SmartColor.My_Tool.CupAuxiliary;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 明细表控件，支持自定义插入、删除、序号管理和智能跳转
    /// 使用 TableLayoutPanel 动态布局所有明细控件
    /// </summary>
    public partial class CtDropDetail : UserControl
    {
        // 列索引常量
        private const int IndexCol = 0;
        private const int CodeCol = 1;
        private const int NameCol = 2;
        private const int DosageCol = 3;
        private const int UnitCol = 4;
        private const int BottleCol = 5;
        private const int SetConcCol = 6;
        private const int RealConcCol = 7;
        private const int ObjDropCol = 8;
        private const int RealDropCol = 9;
        private const int ManualBottleCol = 10;
        private CtFormulaBrowse formulaBrowse = null;
        private CtDropHead _dropHead = null;
        private CtDataGridView _dgvDatail = null;
        private int _mode = 0;

        /// <summary>
        /// 配方改变事件
        /// </summary>
        public event EventHandler FormulaDataChange;

        /// <summary>
        /// 批次改变事件
        /// </summary>
        public event EventHandler<int> BatchChange;

        /// <summary>
        /// 等待列表改变事件
        /// </summary>
        public event EventHandler WaitChange;

        /// <summary>
        /// 是否严格使用母液瓶最小滴液量来筛选瓶号
        /// </summary>
        public bool UseStrictDropMinWeight { get; set; } = false;

        /// <summary>
        /// Mode枚举，表示滴液模式
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// 染色模式
            /// </summary>
            DYE = 0,

            /// <summary>
            /// ABS模式
            /// </summary>
            ABS = 1
        }


        /// <summary>
        /// 滴液模式参数化
        /// </summary>
        private Mode _dropMode = 0;

        // 表名和字段名参数化
        /// <summary>滴液表头表名</summary>
        private string TableDropHeadName;
        /// <summary>滴液表详情表名</summary>
        private string TableDropDetailsName;
        /// <summary>配方表表头表名</summary>
        private string TableFormulaHeadName;
        /// <summary>配方表详情表名</summary>
        private string TableFormulaDetailsName;
        /// <summary>等待列表表名</summary>
        private string TableWaitListName;

        // 字段名参数化
        /// <summary>杯表中杯号字段名</summary>
        private string FieldCupNum;
        /// <summary>杯表中杯号是否可用字段名</summary>
        private string FieldEnable;
        /// <summary>杯表中杯号是否正在使用字段名</summary>
        private string FieldIsUsing;

        /// <summary>杯表中杯子类型段名</summary>
        private string FeildType;

        /// <summary>配方表中染助剂代码字段名</summary>
        private string FieldAssistantCode;
        /// <summary>配方表中瓶号字段名</summary>
        private string FieldBottleNum;
        /// <summary>配方表中配方代码字段名</summary>
        private string FieldFormulaCode;
        /// <summary>配方表中配方版本号字段名</summary>
        private string FieldVersionNum;
        /// <summary>配方表中序号字段名</summary>
        private string FieldIndexNum;
        /// <summary>配方表中染助剂名称字段名</summary>
        private string FieldAssistantName;
        /// <summary>配方表中配方用量字段名</summary>
        private string FieldFormulaDosage;
        /// <summary>配方表中配方表单位字段名</summary>
        private string FieldUnitOfAccount;
        /// <summary>配方表中设置浓度字段名</summary>
        private string FieldSettingConcentration;
        /// <summary>配方表中实际浓度字段名</summary>
        private string FieldRealConcentration;
        /// <summary>配方表中目标滴液量字段名</summary>
        private string FieldObjectDropWeight;
        /// <summary>配方表中实际滴液量字段名</summary>
        private string FieldRealDropWeight;
        /// <summary>配方表中瓶号选择字段名</summary>
        private string FieldBottleSelection;
        /// <summary>配方表头目标加水量字段名</summary>
        private string FieldObjectAddWaterWeight;
        /// <summary>配方表头操作员字段名</summary>
        private string FieldOperator;
        /// <summary>配方表头处理浴比列表字段名</summary>
        private string FieldHandleBRList;
        /// <summary>配方表头创建时间字段名</summary>
        private string FieldCreateTime;
        /// <summary>配方表头类型字段名</summary>
        private string FieldStage;

        /// <summary>备布位字段名</summary>
        private string FieldClothNum;

        /// <summary>滴液表中批次号字段名</summary>
        private string FieldDropBatchName;
        /// <summary>滴液表中表头ID字段名</summary>
        private string FieldDropHeadID;
        /// <summary>滴液表中表头配方代码字段名</summary>
        private string FieldDropFormulaCode;
        /// <summary>滴液表中表头头版本号字段名</summary>
        private string FieldDropVersionNum;
        /// <summary>滴液表中表头配方状态字段名</summary>
        private string FieldState;
        /// <summary>滴液表中杯号字段名</summary>
        private string FieldDropCupNum;
        /// <summary>滴液表中表头MyID字段名</summary>
        private string FieldDropMyID;

        /// <summary>等待列表配方代码字段名</summary>
        private string FieldWaitFormulaCode;
        /// <summary>等待列表版本号字段名</summary>
        private string FieldWaitVersionNum;
        /// <summary>等待列表类型字段名</summary>
        private string FieldWaitType;
        /// <summary>等待列表MyID字段名</summary>
        private string FieldWaitMyID;
        /// <summary>等待列表排队号字段名</summary>
        private string FieldWaitIndexNum;
        /// <summary>等待列表杯号字段名</summary>
        private string FieldWaitCupNum;





        /// <summary>
        /// 小数点保留位数
        /// </summary>
        private int _retainDecimals = 2;

        private struct _batchInfo
        {
            public string CupNum;
            public string HeadID;
        }

        /// <summary>
        /// 构造函数，初始化控件和事件
        /// </summary>
        public CtDropDetail()
        {
            InitializeComponent();
            this._retainDecimals = My_Tool.BalanceStableReading.RetainDecimals();

        }

        private void UseLimit()
        {
            if (this._dropMode != Mode.DYE) return;
            if (SmartColor.My_ConPar.Choices.UseLimit != 1) return;
            if (_dgvDatail == null) return;

            var assistantDt = My_DataBase.AssistantData.Assistant_details;
            if (assistantDt == null) return;

            // 1. 收集所有单位为%且有染助剂代码的行
            var typeSet = new HashSet<string>();
            var codeDosageDict = new Dictionary<string, double>(); // type -> 总用量

            for (int i = 0; i < _dgvDatail.Rows.Count; i++)
            {
                var row = _dgvDatail.Rows[i];
                if (row.IsNewRow) continue;
                var code = row.Cells[CodeCol].Value?.ToString()?.Trim();
                var unit = row.Cells[UnitCol].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(code) || unit != "%") continue;

                var found = assistantDt.Select($"{My_DataBase.ASSISTANT_DETAILS.AssistantCode} = '{code.Replace("'", "''")}'");
                if (found == null || found.Length == 0) continue;
                var type = found[0][My_DataBase.ASSISTANT_DETAILS.AssistantType]?.ToString();
                if (string.IsNullOrEmpty(type)) continue;

                typeSet.Add(type);

                double dosage = ParseDouble(row.Cells[DosageCol].Value);
                if (!codeDosageDict.ContainsKey(type))
                    codeDosageDict[type] = 0;
                codeDosageDict[type] += dosage;
            }

            // 如果类型不是唯一，直接退出
            if (typeSet.Count != 1) return;

            // 2. 只处理唯一类型
            string onlyType = typeSet.First();
            double totalDosage = codeDosageDict[onlyType];

            var limitDt = My_DataBase.LimitData.LimitTable;
            if (limitDt == null) return;

            // 找到所有区间行
            var limitRows = limitDt.Select($"{My_DataBase.LIMIT_TABLE.Type} = '{onlyType}'");
            if (limitRows == null || limitRows.Length == 0) return;

            // 找到所有区间行（同一个区间可能有多行助剂）
            var matchedLimits = new List<DataRow>();
            foreach (var limitRow in limitRows)
            {
                double min = ParseDouble(limitRow[My_DataBase.LIMIT_TABLE.Min]);
                object maxObj = limitRow[My_DataBase.LIMIT_TABLE.Max];
                bool isMaxNull = maxObj == DBNull.Value || maxObj == null || string.IsNullOrWhiteSpace(maxObj.ToString());
                double max = isMaxNull ? double.MaxValue : ParseDouble(maxObj);

                if (totalDosage >= min && totalDosage < max)
                {
                    matchedLimits.Add(limitRow);
                }
            }
            if (matchedLimits.Count == 0) return;

            foreach (var matchedLimit in matchedLimits)
            {
                string limitName = matchedLimit[My_DataBase.LIMIT_TABLE.AssistantCode]?.ToString();
                string limitValue = matchedLimit[My_DataBase.LIMIT_TABLE.Value]?.ToString();
                if (string.IsNullOrEmpty(limitName)) continue;

                // 检查是否已存在该助剂
                bool exists = false;
                for (int i = 0; i < _dgvDatail.Rows.Count; i++)
                {
                    var row = _dgvDatail.Rows[i];
                    if (row.IsNewRow) continue;
                    var code = row.Cells[CodeCol].Value?.ToString()?.Trim();
                    if (code == limitName)
                    {
                        exists = true;
                        break;
                    }
                }
                if (exists) continue; // 已存在则跳过

                // 查找助剂名称、单位
                var foundLimit = assistantDt.Select($"{My_DataBase.ASSISTANT_DETAILS.AssistantCode} = '{limitName.Replace("'", "''")}'");
                string assistantName = foundLimit != null && foundLimit.Length > 0 ? foundLimit[0][My_DataBase.ASSISTANT_DETAILS.AssistantName]?.ToString() : "";
                string unitLimit = foundLimit != null && foundLimit.Length > 0 ? foundLimit[0][My_DataBase.ASSISTANT_DETAILS.UnitOfAccount]?.ToString() : "%";

                int newRowIdx = _dgvDatail.Rows.Add();
                _dgvDatail.Rows[newRowIdx].Cells[IndexCol].Value = (_dgvDatail.Rows.Count).ToString();
                _dgvDatail.Rows[newRowIdx].Cells[CodeCol].Value = limitName;
                _dgvDatail.Rows[newRowIdx].Cells[NameCol].Value = assistantName;
                _dgvDatail.Rows[newRowIdx].Cells[DosageCol].Value = limitValue;
                _dgvDatail.Rows[newRowIdx].Cells[UnitCol].Value = unitLimit;

                // 清空瓶号下拉项
                var bottleCell = _dgvDatail.Rows[newRowIdx].Cells[BottleCol] as DataGridViewComboBoxCell;
                if (bottleCell != null)
                {
                    bottleCell.Items.Clear();
                    bottleCell.Value = null;
                    _dgvDatail.Rows[newRowIdx].Cells[BottleCol].Style.BackColor = System.Drawing.Color.White;
                }
                _dgvDatail.Rows[newRowIdx].Cells[SetConcCol].Value = null;
                _dgvDatail.Rows[newRowIdx].Cells[RealConcCol].Value = null;
                _dgvDatail.Rows[newRowIdx].Cells[ObjDropCol].Value = null;
                _dgvDatail.Rows[newRowIdx].Cells[RealDropCol].Value = null;
                _dgvDatail.Rows[newRowIdx].Cells[ManualBottleCol].Value = false;

                // 自动选瓶
                var (bathRatio, clothWeight, totalWeight) = GetHeadValues();
                TryFindBottleForGrid(_dgvDatail, newRowIdx, limitName, limitValue, unitLimit, clothWeight, totalWeight);
            }

            AutoFitColumns(_dgvDatail);
            RefreshLayout();
        }


        // 2. 重写 ProcessCmdKey，确保快捷键全局可用
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (HandleShortcutKeys(keyData))
                return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }
        public bool HandleShortcutKeys(Keys key)
        {
            if (key == Keys.F5 && btn_FormulaCodeAdd != null && btn_FormulaCodeAdd.Enabled)
            {
                Btn_FormulaCodeAdd_Click(btn_FormulaCodeAdd, EventArgs.Empty);
                return true;
            }
            if (key == Keys.F1 && btn_upd != null && btn_upd.Enabled)
            {
                Btn_upd_Click(btn_upd, EventArgs.Empty);
                return true;
            }
            if (key == Keys.F2 && btn_Save != null && btn_Save.Enabled)
            {
                Btn_Save_Click(btn_Save, EventArgs.Empty);
                return true;
            }
            if (key == Keys.F4 && btn_BatchAdd != null && btn_BatchAdd.Enabled)
            {
                btn_BatchAdd_Click(btn_BatchAdd, EventArgs.Empty);
                return true;
            }
            if (key == Keys.F3 && btn_pre != null && btn_pre.Enabled)
            {
                Btn_pre_Click(btn_pre, EventArgs.Empty);
                return true;
            }
            return false;
        }



        /// <summary>
        /// 设置滴液模式（Normal/ABS），自动切换所有表名和字段名
        /// </summary>
        /// <param name="mode">滴液模式:0:染色模式；1：ABS模式</param>
        public void SetMode(Mode mode)
        {
            _dropMode = mode;
            if (mode == Mode.DYE)
            {
                // 普通模式表名和字段名
                TableDropHeadName = DROP_HEAD.TableName;
                TableDropDetailsName = DROP_DETAILS.TableName;
                TableFormulaHeadName = FORMULA_HEAD.TableName;
                TableFormulaDetailsName = FORMULA_DETAILS.TableName;
                TableWaitListName = WAIT_LIST.TableName;

                FieldCupNum = CUP_DETAILS.CupNum;
                FieldEnable = CUP_DETAILS.Enable;
                FieldIsUsing = CUP_DETAILS.IsUsing;

                FeildType = CUP_DETAILS.Type;

                FieldAssistantCode = My_DataBase.FORMULA_DETAILS.AssistantCode;
                FieldBottleNum = My_DataBase.FORMULA_DETAILS.BottleNum;
                FieldFormulaCode = My_DataBase.FORMULA_DETAILS.FormulaCode;
                FieldVersionNum = My_DataBase.FORMULA_DETAILS.VersionNum;
                FieldIndexNum = My_DataBase.FORMULA_DETAILS.IndexNum;
                FieldAssistantName = My_DataBase.FORMULA_DETAILS.AssistantName;
                FieldFormulaDosage = My_DataBase.FORMULA_DETAILS.FormulaDosage;
                FieldUnitOfAccount = My_DataBase.FORMULA_DETAILS.UnitOfAccount;
                FieldSettingConcentration = My_DataBase.FORMULA_DETAILS.SettingConcentration;
                FieldRealConcentration = My_DataBase.FORMULA_DETAILS.RealConcentration;
                FieldObjectDropWeight = My_DataBase.FORMULA_DETAILS.ObjectDropWeight;
                FieldRealDropWeight = My_DataBase.FORMULA_DETAILS.RealDropWeight;
                FieldBottleSelection = My_DataBase.FORMULA_DETAILS.BottleSelection;

                FieldObjectAddWaterWeight = My_DataBase.FORMULA_HEAD.ObjectAddWaterWeight;
                FieldOperator = My_DataBase.FORMULA_HEAD.Operator;
                FieldHandleBRList = My_DataBase.FORMULA_HEAD.HandleBRList;
                FieldCreateTime = My_DataBase.FORMULA_HEAD.CreateTime;
                FieldStage = My_DataBase.FORMULA_HEAD.Stage;
                FieldClothNum = My_DataBase.FORMULA_HEAD.ClothNum;


                FieldDropBatchName = My_DataBase.DROP_DETAILS.BatchName;
                FieldDropHeadID = My_DataBase.DROP_DETAILS.HeadID;
                FieldDropFormulaCode = My_DataBase.DROP_HEAD.FormulaCode;
                FieldDropVersionNum = My_DataBase.DROP_HEAD.VersionNum;
                FieldState = My_DataBase.DROP_HEAD.State;
                FieldDropCupNum = My_DataBase.DROP_HEAD.CupNum;
                FieldDropMyID = My_DataBase.DROP_HEAD.MyID;

                FieldWaitFormulaCode = My_DataBase.WAIT_LIST.FormulaCode;
                FieldWaitVersionNum = My_DataBase.WAIT_LIST.VersionNum;
                FieldWaitType = My_DataBase.WAIT_LIST.Type;
                FieldWaitMyID = My_DataBase.WAIT_LIST.MyID;
                FieldWaitIndexNum = My_DataBase.WAIT_LIST.IndexNum;
                FieldWaitCupNum = My_DataBase.WAIT_LIST.CupNum;




            }
            else
            {
                // ABS模式表名和字段名
                TableDropHeadName = ABS_DROP_HEAD.TableName;
                TableDropDetailsName = ABS_DROP_DETAILS.TableName;
                TableFormulaHeadName = ABS_FORMULA_HEAD.TableName;
                TableFormulaDetailsName = ABS_FORMULA_DETAILS.TableName;
                TableWaitListName = ABS_WAIT_LIST.TableName;

                FieldCupNum = CUP_DETAILS.CupNum;
                FieldEnable = CUP_DETAILS.Enable;
                FieldIsUsing = CUP_DETAILS.IsUsing;


                FieldAssistantCode = ABS_FORMULA_DETAILS.AssistantCode;
                FieldBottleNum = ABS_FORMULA_DETAILS.BottleNum;
                FieldFormulaCode = ABS_FORMULA_DETAILS.FormulaCode;
                FieldVersionNum = ABS_FORMULA_DETAILS.VersionNum;
                FieldIndexNum = ABS_FORMULA_DETAILS.IndexNum;
                FieldAssistantName = ABS_FORMULA_DETAILS.AssistantName;
                FieldFormulaDosage = ABS_FORMULA_DETAILS.FormulaDosage;
                FieldUnitOfAccount = ABS_FORMULA_DETAILS.UnitOfAccount;
                FieldSettingConcentration = ABS_FORMULA_DETAILS.SettingConcentration;
                FieldRealConcentration = ABS_FORMULA_DETAILS.RealConcentration;
                FieldObjectDropWeight = ABS_FORMULA_DETAILS.ObjectDropWeight;
                FieldRealDropWeight = ABS_FORMULA_DETAILS.RealDropWeight;
                FieldBottleSelection = ABS_FORMULA_DETAILS.BottleSelection;

                FieldObjectAddWaterWeight = My_DataBase.ABS_FORMULA_HEAD.ObjectAddWaterWeight;
                FieldOperator = My_DataBase.ABS_FORMULA_HEAD.Operator;
                FieldHandleBRList = My_DataBase.ABS_FORMULA_HEAD.HandleBRList;
                FieldCreateTime = My_DataBase.ABS_FORMULA_HEAD.CreateTime;
                FieldStage = My_DataBase.ABS_FORMULA_HEAD.Stage;


                FieldDropBatchName = My_DataBase.ABS_DROP_DETAILS.BatchName;
                FieldDropHeadID = My_DataBase.ABS_DROP_DETAILS.HeadID;
                FieldDropFormulaCode = My_DataBase.ABS_DROP_HEAD.FormulaCode;
                FieldDropVersionNum = My_DataBase.ABS_DROP_HEAD.VersionNum;
                FieldState = My_DataBase.ABS_DROP_HEAD.State;
                FieldDropCupNum = My_DataBase.ABS_DROP_HEAD.CupNum;
                FieldDropMyID = My_DataBase.ABS_DROP_HEAD.MyID;

                FieldWaitFormulaCode = My_DataBase.ABS_WAIT_LIST.FormulaCode;
                FieldWaitVersionNum = My_DataBase.ABS_WAIT_LIST.VersionNum;
                FieldWaitType = My_DataBase.ABS_WAIT_LIST.Type;
                FieldWaitMyID = My_DataBase.ABS_WAIT_LIST.MyID;
                FieldWaitIndexNum = My_DataBase.ABS_WAIT_LIST.IndexNum;
                FieldWaitCupNum = My_DataBase.ABS_WAIT_LIST.CupNum;




            }



            InitEvents();
        }




        /// <summary>
        /// 初始化事件绑定
        /// </summary>
        private void InitEvents()
        {
            Logger.Info("CtDropDetail 初始化事件绑定。");
            My_Form.Login.LoginForm.UserChanged -= LoginForm_UserChanged; ;
            My_Form.Login.LoginForm.UserChanged += LoginForm_UserChanged;
            CtDropDetail_Load(null, EventArgs.Empty);
        }

        private void LoginForm_UserChanged(object sender, EventArgs e)
        {
            SetButtonEnabled();
        }



        /// <summary>
        /// 绑定头部控件
        /// </summary>
        /// <param name="h">头部控件</param>
        public void BindDropHead(CtDropHead h)
        {
            Logger.Info("CtDropDetail 绑定头部控件。");
            this._dropHead = h;
            this._dropHead.FormulaCodeChanged -= (s, e) => FillDropDetail(e as DataTable);
            this._dropHead.FormulaCodeChanged += (s, e) => FillDropDetail(e as DataTable);
            this._dropHead.FormulaGroupChanged -= (s, e) => FillGropDetail(e as DataTable);
            this._dropHead.FormulaGroupChanged += (s, e) => FillGropDetail(e as DataTable);
            this._dropHead.LastInputControlEnter -= (s, e) => FocusFirstGridCodeCell();
            this._dropHead.LastInputControlEnter += (s, e) => FocusFirstGridCodeCell();

            this._dropHead.DyeingCodeChanged -= (s, e) => FillDyeDetail((CtDropHead.DyeInfo)e);
            this._dropHead.DyeingCodeChanged += (s, e) => FillDyeDetail((CtDropHead.DyeInfo)e);


            this._dropHead.ClothWeightChanged -= Dgv_ClothWeightChangedHandler;
            this._dropHead.ClothWeightChanged += Dgv_ClothWeightChangedHandler;

            this._dropHead.BathRatioChanged -= Dgv_BathRatioChangedHandler;
            this._dropHead.BathRatioChanged += Dgv_BathRatioChangedHandler;
        }

        public void BindFormulaBrowse(CtFormulaBrowse h)
        {
            this.formulaBrowse = h;

        }



        private void FillGropDetail(DataTable dataTable)
        {
            Logger.Info("CtDropDetail 填充配方组合明细。");
            this._dgvDatail.Rows.Clear();
            if (dataTable == null || dataTable.Rows.Count == 0)
                return;
            int index = 1;
            foreach (DataRow row in dataTable.Rows)
            {
                // 组合中没有配方用量，DosageCol可留空或填0
                int rowIndex = this._dgvDatail.Rows.Add();
                this._dgvDatail.Rows[rowIndex].Cells[IndexCol].Value = index++;
                this._dgvDatail.Rows[rowIndex].Cells[CodeCol].Value = row.Table.Columns.Contains(FORMULA_GROUP.AssistantCode)
                    ? row[FORMULA_GROUP.AssistantCode]?.ToString()
                    : null;
                this._dgvDatail.Rows[rowIndex].Cells[NameCol].Value = row.Table.Columns.Contains(FORMULA_GROUP.AssistantName)
                    ? row[FORMULA_GROUP.AssistantName]?.ToString()
                    : null;
                this._dgvDatail.Rows[rowIndex].Cells[UnitCol].Value = row.Table.Columns.Contains(FORMULA_GROUP.UnitOfAccount)
                    ? row[FORMULA_GROUP.UnitOfAccount]?.ToString()
                    : null;

            }
            AutoFitColumns(this._dgvDatail);
            RefreshLayout();
        }

        /// <summary>
        /// 填充染色明细
        /// </summary>
        private void FillDyeDetail(CtDropHead.DyeInfo dyeInfo)
        {

            Logger.Info("CtDyeDetail 填充染色明细。");
            var dyeingCtrls = tableLayoutPanel1.Controls.OfType<CtDyeing>().ToList();
            foreach (var ctrl in dyeingCtrls)
            {
                int idx = tableLayoutPanel1.Controls.IndexOf(ctrl);
                tableLayoutPanel1.Controls.Remove(ctrl);
                ctrl.Dispose(); // 关键：销毁控件，解绑事件
                if (idx >= 0 && idx < tableLayoutPanel1.RowStyles.Count)
                    tableLayoutPanel1.RowStyles.RemoveAt(idx);
            }
            tableLayoutPanel1.RowCount = tableLayoutPanel1.Controls.Count;

            int i = 0;
            if (dyeInfo.DT == null) return;
            foreach (DataRow dr in dyeInfo.DT.Rows)
            {
                CtDyeing newDyeing = new CtDyeing(this._dropHead);
                newDyeing.dgv = InsertConlumn(newDyeing.dgv, 1);

                if (dr[My_DataBase.DYEING_CODE.Type] != DBNull.Value)
                {
                    int type = Convert.ToInt32(dr[My_DataBase.DYEING_CODE.Type]);
                    string code = dr[My_DataBase.DYEING_CODE.Code].ToString();
                    int no = Convert.ToInt16(dr[My_DataBase.DYEING_CODE.IndexNum]);
                    (string formulaCode, string versionNum) = GetHeadFormulaInfo();
                    (double bathRatio, double clothWeight, double totalWeight) = GetHeadValues();
                    double BathRatio = bathRatio;
                    DataTable data = null;
                    if (!string.IsNullOrEmpty(versionNum))
                    {

                        var headRows = SqlServer.Select(My_DataBase.FORMULA_HEAD.TableName,
                            $"{My_DataBase.FORMULA_HEAD.FormulaCode} = '{formulaCode.Replace("'", "''")}' AND " +
                            $"{My_DataBase.FORMULA_HEAD.VersionNum} = '{versionNum.Replace("'", "''")}'");
                        if (headRows != null && headRows.Rows.Count > 0)
                        {
                            var dyeCode = headRows.Rows[0][My_DataBase.FORMULA_HEAD.DyeingCode].ToString();
                            if (dyeCode == code)
                            {

                                string handleBRList = headRows.Rows[0][
                                    My_DataBase.FORMULA_HEAD.HandleBRList].ToString();
                                if (!string.IsNullOrEmpty(handleBRList))
                                {
                                    string[] strings = handleBRList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (strings != null && strings.Length > i)
                                        BathRatio = Convert.ToDouble(strings[i]);
                                }

                                var detailRows = SqlServer.Select(My_DataBase.FORMULA_HANDLE_DETAILS.TableName,
                                    null,
                                    $"{My_DataBase.FORMULA_HANDLE_DETAILS.FormulaCode} = '{formulaCode.Replace("'", "''")}' AND {My_DataBase.FORMULA_HANDLE_DETAILS.VersionNum} = '{versionNum.Replace("'", "''")}' AND {My_DataBase.FORMULA_HANDLE_DETAILS.No} = {no}",
                                    My_DataBase.FORMULA_HANDLE_DETAILS.TechnologyName,
                                    true);
                                data = detailRows;
                            }
                        }
                        else
                        {

                            headRows = SqlServer.Select(My_DataBase.FORMULA_HEAD_TEMP.TableName,
                                $"{My_DataBase.FORMULA_HEAD_TEMP.FormulaCode} = '{formulaCode.Replace("'", "''")}' AND {My_DataBase.FORMULA_HEAD_TEMP.VersionNum} = '{versionNum.Replace("'", "''")}'");
                            if (headRows != null && headRows.Rows.Count > 0)
                            {
                                var dyeCode = headRows.Rows[0][My_DataBase.FORMULA_HEAD.DyeingCode].ToString();
                                if (dyeCode == code)
                                {
                                    string handleBRList = headRows.Rows[0][My_DataBase.FORMULA_HEAD_TEMP.HandleBRList].ToString();
                                    if (!string.IsNullOrEmpty(handleBRList))
                                    {
                                        string[] strings = handleBRList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                        if (strings != null && strings.Length > i)
                                            BathRatio = Convert.ToDouble(strings[i]);
                                    }


                                    var detailRows = SqlServer.Select(My_DataBase.FORMULA_HANDLE_DETAILS.TableName,
                                     null,
                                     $"{My_DataBase.FORMULA_HANDLE_DETAILS.FormulaCode} = '{formulaCode.Replace("'", "''")}' AND {My_DataBase.FORMULA_HANDLE_DETAILS.VersionNum} = '{versionNum.Replace("'", "''")}' AND {My_DataBase.FORMULA_HANDLE_DETAILS.No} = {no}",
                                     My_DataBase.FORMULA_HANDLE_DETAILS.TechnologyName,
                                     true);
                                    data = detailRows;
                                }


                            }
                        }
                    }
                    if (data == null)
                    {
                        var bat = this._dropHead.GetAllInputValues();
                        if (bat != null)
                            BathRatio = Convert.ToDouble(bat["txt_BathRatio"]);
                    }




                    newDyeing.FillControlsFromDataTable(type, code, BathRatio, data, dyeInfo);

                }
                AutoFitColumns(newDyeing.dgv);
                int height = newDyeing.panel1.Height + newDyeing.dgv.GetRecommendedHeight();
                AddToTableLayout(newDyeing, height, false);
                i++;
            }
            RefreshLayout();
        }

        /// <summary>
        /// 加载明细表控件
        /// </summary>
        private void CtDropDetail_Load(object sender, EventArgs e)
        {
            Logger.Info("CtDropDetail 加载明细表控件。");
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowCount = 0;

            if (My_ConPar.Machine.MachineLayout == 0 || this._dropMode == Mode.ABS)
            {
                this._dgvDatail = InsertConlumn(new CtDataGridView());
                AddToTableLayout(this._dgvDatail, tableLayoutPanel1.Height, false);
            }
            else
            {
                var ctFoldDataGridView = new CtFoldDataGridView();
                ctFoldDataGridView = InsertConlumn(ctFoldDataGridView);
                int height = ctFoldDataGridView.GetRecommendedHeight();
                this._dgvDatail = ctFoldDataGridView;
                AddToTableLayout(this._dgvDatail, height, false);

            }

            AutoFitColumns(this._dgvDatail);
            tableLayoutPanel1.ResumeLayout();
            RefreshLayout();
        }


        // 新增：事件处理方法
        private void Dgv_ClothWeightChangedHandler(object sender, object val)
        {
            RecalculateAllRowsForGrid(_dgvDatail);
        }
        private void Dgv_BathRatioChangedHandler(object sender, object val)
        {
            RecalculateAllRowsForGrid(_dgvDatail);
        }

        // 新增：重新计算所有行瓶号
        private void RecalculateAllRowsForGrid(CtDataGridView dgv)
        {
            if (dgv == null) return;
            var (bathRatio, clothWeight, totalWeight) = GetHeadValues();
            for (int row = 0; row < dgv.Rows.Count; row++)
            {
                var codeCell = dgv.Rows[row].Cells[CodeCol];
                var dosageCell = dgv.Rows[row].Cells[DosageCol];
                var unitCell = dgv.Rows[row].Cells[UnitCol];

                string code = codeCell.Value?.ToString()?.Trim();
                string dosageStr = dosageCell.Value?.ToString();
                string unit = unitCell.Value?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(dosageStr) || string.IsNullOrWhiteSpace(unit))
                    continue;

                TryFindBottleForGrid(dgv, row, code, dosageCell.Value, unit, clothWeight, totalWeight);
            }
        }

        // 新增：瓶号查找逻辑（参考CtDyeing）
        private void TryFindBottleForGrid(CtDataGridView dgv, int row, string code, object dosageValue, string unit, double clothWeight, double totalWeight)
        {
            if (_dropHead.Source != 0) return;
            var bottleDt = My_DataBase.BottleData.Bottle_details;
            DateTime brewTime = DateTime.Now;
            // 按实际浓度从高到低排序
            var bottleRows = bottleDt?.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{code.Replace("'", "''")}'", $"{My_DataBase.BOTTLE_DETAILS.RealConcentration} DESC");
            var bottleCell = dgv.Rows[row].Cells[BottleCol] as DataGridViewComboBoxCell;


            dgv.Rows[row].Cells[SetConcCol].Value = null;
            dgv.Rows[row].Cells[RealConcCol].Value = null;
            dgv.Rows[row].Cells[ObjDropCol].Value = null;

            // 1. 手动选瓶优先
            bool isManual = false;
            if (dgv.Rows[row].Cells[ManualBottleCol].Value is bool b && b)
                isManual = b;

            if (isManual && bottleCell != null && bottleCell.Value != null && bottleCell.Value.ToString() != "无")
            {
                // 直接用手动选瓶，不再查找
                string manualBottleNum = bottleCell.Value.ToString();
                var manualBottleRows = bottleDt?.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{code.Replace("'", "''")}' AND {My_DataBase.BOTTLE_DETAILS.BottleNum} = '{manualBottleNum}'");
                if (manualBottleRows != null && manualBottleRows.Length > 0)
                {
                    var bRow = manualBottleRows[0];
                    double settingConc = ParseDouble(bRow[My_DataBase.BOTTLE_DETAILS.SettingConcentration]);
                    double realConc = ParseDouble(bRow[My_DataBase.BOTTLE_DETAILS.RealConcentration]);
                    brewTime = bRow[My_DataBase.BOTTLE_DETAILS.BrewingData] != DBNull.Value ? Convert.ToDateTime(bRow[My_DataBase.BOTTLE_DETAILS.BrewingData]) : DateTime.Now;
                    double objDropWeight = 0;
                    double formulaDosage1 = ParseDouble(dosageValue);
                    if (unit == "%")
                        objDropWeight = CalcDropWeight_Percent(code, formulaDosage1, clothWeight, realConc, brewTime);
                    else if (unit == "g/l")
                        objDropWeight = CalcDropWeight_GramPerLiter(code, formulaDosage1, totalWeight, realConc, brewTime);

                    bottleCell.Style.BackColor = Color.White;
                    dgv.Rows[row].Cells[SetConcCol].Value = settingConc;
                    dgv.Rows[row].Cells[RealConcCol].Value = realConc;
                    dgv.Rows[row].Cells[ObjDropCol].Value = string.Format("{0:F" + this._retainDecimals + "}", objDropWeight);
                    dgv.Rows[row].Cells[RealDropCol].Value = string.Format("{0:F" + this._retainDecimals + "}", dgv.Rows[row].Cells[RealDropCol].Value == null ? 0 : dgv.Rows[row].Cells[RealDropCol].Value);
                }
                return;
            }

            // 2. 自动选瓶逻辑
            bool foundBottle = false;
            double formulaDosage = ParseDouble(dosageValue);
            DataRow lowestConcRow = null;
            double lowestConc = double.MaxValue;
            double lowestObjDropWeight = 0;
            int lowestBottleNum = -1;
            double lowestSettingConc = 0, lowestRealConc = 0;


            if (bottleRows != null && bottleRows.Length > 0)
            {
                foreach (var bRow in bottleRows)
                {
                    double settingConc = ParseDouble(bRow[My_DataBase.BOTTLE_DETAILS.SettingConcentration]);
                    double realConc = ParseDouble(bRow[My_DataBase.BOTTLE_DETAILS.RealConcentration]);
                    double dropMinWeight = ParseDouble(bRow[My_DataBase.BOTTLE_DETAILS.DropMinWeight]);
                    int bottleNum = Convert.ToInt32(bRow[My_DataBase.BOTTLE_DETAILS.BottleNum]);
                    brewTime = bRow[My_DataBase.BOTTLE_DETAILS.BrewingData] != DBNull.Value ? Convert.ToDateTime(bRow[My_DataBase.BOTTLE_DETAILS.BrewingData]) : DateTime.Now;

                    double objDropWeight = 0;
                    if (unit == "%")
                        objDropWeight = CalcDropWeight_Percent(code, formulaDosage, clothWeight, realConc, brewTime);
                    else if (unit == "g/l")
                        objDropWeight = CalcDropWeight_GramPerLiter(code, formulaDosage, totalWeight, realConc, brewTime);

                    // 优先选目标需加量大于瓶最小滴液量的第一个瓶
                    if (objDropWeight >= dropMinWeight)
                    {
                        string bottleNumStr = bottleNum.ToString();
                        if (bottleCell != null)
                        {
                            if (!bottleCell.Items.Contains(bottleNumStr))
                                bottleCell.Items.Add(bottleNumStr);
                            bottleCell.Value = bottleNumStr;
                            bottleCell.Style.BackColor = Color.White;
                        }
                        dgv.Rows[row].Cells[SetConcCol].Value = settingConc;
                        dgv.Rows[row].Cells[RealConcCol].Value = realConc;
                        dgv.Rows[row].Cells[ObjDropCol].Value = string.Format("{0:F" + this._retainDecimals + "}", objDropWeight);
                        dgv.Rows[row].Cells[RealDropCol].Value = string.Format("{0:F" + this._retainDecimals + "}", dgv.Rows[row].Cells[RealDropCol].Value == null ? 0 : dgv.Rows[row].Cells[RealDropCol].Value);
                        foundBottle = true;
                        break;
                    }

                    // 记录最低浓度瓶
                    if (realConc < lowestConc)
                    {
                        lowestConc = realConc;
                        lowestConcRow = bRow;
                        lowestObjDropWeight = objDropWeight;
                        lowestBottleNum = bottleNum;
                        lowestSettingConc = settingConc;
                        lowestRealConc = realConc;
                    }
                }
            }

            if (!foundBottle)
            {
                // 3. 所有瓶都不满足，取最低浓度瓶
                if (lowestConcRow != null)
                {
                    double alarmDropWeight = SmartColor.My_ConPar.Other.AlarmDropWeight;

                    string bottleNumStr = lowestBottleNum.ToString();
                    if (bottleCell != null)
                    {
                        if (!bottleCell.Items.Contains(bottleNumStr))
                            bottleCell.Items.Add(bottleNumStr);
                        bottleCell.Value = bottleNumStr;
                        bottleCell.Style.BackColor = Color.White;
                    }
                    dgv.Rows[row].Cells[SetConcCol].Value = lowestSettingConc;
                    dgv.Rows[row].Cells[RealConcCol].Value = lowestRealConc;
                    dgv.Rows[row].Cells[ObjDropCol].Value = string.Format("{0:F" + this._retainDecimals + "}", lowestObjDropWeight);
                    dgv.Rows[row].Cells[RealDropCol].Value = string.Format("{0:F" + this._retainDecimals + "}", dgv.Rows[row].Cells[RealDropCol].Value == null ? 0 : dgv.Rows[row].Cells[RealDropCol].Value);
                    if (lowestObjDropWeight < alarmDropWeight)
                    {
                        if (bottleCell != null)
                            bottleCell.Style.BackColor = Color.Yellow;
                        string format = $"F{this._retainDecimals}";
                        LocalTranslator.ShowMessage(
                            $"警告：配方中{code}的目标滴液量{lowestObjDropWeight.ToString(format)}小于报警值{alarmDropWeight.ToString(format)}，请确认是否使用瓶号{lowestBottleNum}的染助剂。",
                            "温馨提示",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    // 没有任何瓶号，显示无
                    if (bottleCell != null)
                    {
                        bottleCell.Items.Add("无");
                        bottleCell.Value = "无";
                        bottleCell.Style.BackColor = Color.Red;
                        My_File.LocalTranslator.ShowMessage("没有任何母液瓶可选！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                }
            }
        }
        /// <summary>
        /// 将控件添加到 TableLayoutPanel 并设置高度
        /// 优化：可选择是否立即刷新布局
        /// </summary>
        private void AddToTableLayout(Control ctrl, int height, bool refresh = true)
        {
            Logger.Info($"CtDropDetail 添加控件到 TableLayoutPanel，类型：{ctrl.GetType().Name}，高度：{height}");
            if (ctrl is CtDataGridView && !(ctrl is CtFoldDataGridView))
            {
                tableLayoutPanel1.SuspendLayout();
                tableLayoutPanel1.Controls.Clear();
                tableLayoutPanel1.RowStyles.Clear();
                tableLayoutPanel1.RowCount = 1;
                // 使用百分比填充
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                // 让tableLayoutPanel1自身填满父容器
                tableLayoutPanel1.Dock = DockStyle.Fill;
                ctrl.Dock = DockStyle.Fill;
                tableLayoutPanel1.Controls.Add(ctrl, 0, 0);

                BindGridEvents((CtDataGridView)ctrl);
                SetGridEnterJump((CtDataGridView)ctrl);

                tableLayoutPanel1.ResumeLayout();
                if (refresh)
                    RefreshLayout();
                return;
            }


            tableLayoutPanel1.RowCount++;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
            ctrl.Dock = DockStyle.Fill;
            tableLayoutPanel1.Controls.Add(ctrl, 0, tableLayoutPanel1.RowCount - 1);


            if (ctrl is CtFoldDataGridView fdgv)
            {
                BindGridEvents(fdgv);
                SetGridEnterJump(fdgv);
            }
            else if (ctrl is CtDyeing dyeing)
            {
                dyeing.dgv.EditingControlShowing -= DataGridView_EditingControlShowing;
                dyeing.dgv.EditingControlShowing += DataGridView_EditingControlShowing;
                dyeing.dgv.CurrentCellDirtyStateChanged -= Dgv_CurrentCellDirtyStateChanged;
                dyeing.dgv.CurrentCellDirtyStateChanged += Dgv_CurrentCellDirtyStateChanged;
                dyeing.dgv.CurrentCellChanged -= Dgv_CurrentCellChanged;
                dyeing.dgv.CurrentCellChanged += Dgv_CurrentCellChanged;
                dyeing.dgv.CellBeginEdit -= Dgv_CellBeginEdit;
                dyeing.dgv.CellBeginEdit += Dgv_CellBeginEdit;
                dyeing.dgv.RowsAdded -= DyeingRowsChanged_RefreshLayout;
                dyeing.dgv.RowsAdded += DyeingRowsChanged_RefreshLayout;
                dyeing.dgv.RowsRemoved -= DyeingRowsChanged_RefreshLayout;
                dyeing.dgv.RowsRemoved += DyeingRowsChanged_RefreshLayout;
                dyeing.dgv.ExpandRequested -= DyeingRowsChanged_RefreshLayout;
                dyeing.dgv.ExpandRequested += DyeingRowsChanged_RefreshLayout;
                dyeing.OnAllRowsProcessed -= DyeingRowsChanged_CtDyeingJump;
                dyeing.OnAllRowsProcessed += DyeingRowsChanged_CtDyeingJump;
                dyeing.dgv.ClearSelection();


            }

            if (refresh)
                RefreshLayout();
        }

        // 优化：减少重复刷新
        private void DyeingRowsChanged_RefreshLayout(object sender, EventArgs e)
        {
            RefreshLayout();
        }
        private void DyeingRowsChanged_CtDyeingJump(object sender, string e)
        {
            CtDyeingJump(sender as CtDyeing);
        }

        /// <summary>
        /// 刷新布局
        /// </summary>
        private void RefreshLayout()
        {
            tableLayoutPanel1.SuspendLayout();
            if (_dropMode == Mode.DYE && My_ConPar.Machine.MachineLayout != 0)
            {
                for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
                {
                    var ctrl = tableLayoutPanel1.Controls[i];
                    int height = 40;
                    if (ctrl is CtFoldDataGridView fdgv)
                        height = fdgv.GetRecommendedHeight();
                    else if (ctrl is CtDyeing dyeing)
                        height = dyeing.dgv.GetRecommendedHeight() + dyeing.panel1.Height;

                    tableLayoutPanel1.RowStyles[i].Height = height;
                    ctrl.Height = height;
                    ctrl.MaximumSize = new Size(0, height);
                    ctrl.Margin = new Padding(0);

                    if (ctrl is CtDyeing dyeingCtrl)
                    {
                        dyeingCtrl.dgv.ScrollBars = ScrollBars.Vertical;
                        dyeingCtrl.dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                        dyeingCtrl.dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                        dyeingCtrl.dgv.MaximumSize = new Size(0, height - dyeingCtrl.panel1.Height);
                        dyeingCtrl.dgv.Height = height - dyeingCtrl.panel1.Height;
                    }
                    else if (ctrl is DataGridView dgv)
                    {
                        dgv.ScrollBars = ScrollBars.Vertical;
                        dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                        dgv.MaximumSize = new Size(0, height);
                        dgv.Height = height;
                    }
                }
            }
            else
            {
                tableLayoutPanel1.RowStyles[0].SizeType = SizeType.Percent;
                tableLayoutPanel1.RowStyles[0].Height = 100F;
                var ctrl = tableLayoutPanel1.Controls[0];
                ctrl.Dock = DockStyle.Fill;
                ctrl.Margin = new Padding(0);
                ctrl.MaximumSize = new Size(0, 0); // 不限制高度
            }
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
        }

        /// <summary>
        /// 绑定DataGridView相关事件
        /// </summary>
        private void BindGridEvents(CtDataGridView dgv)
        {
            dgv.EditingControlShowing -= DataGridView_EditingControlShowing;
            dgv.EditingControlShowing += DataGridView_EditingControlShowing;
            dgv.CurrentCellDirtyStateChanged -= Dgv_CurrentCellDirtyStateChanged;
            dgv.CurrentCellDirtyStateChanged += Dgv_CurrentCellDirtyStateChanged;
            dgv.CurrentCellChanged -= Dgv_CurrentCellChanged;
            dgv.CurrentCellChanged += Dgv_CurrentCellChanged;
            dgv.KeyDown -= Dgv_KeyDown;
            dgv.KeyDown += Dgv_KeyDown;
            dgv.RowsAdded -= Dgv_RowsChanged_ReorderStepNum;
            dgv.RowsAdded += Dgv_RowsChanged_ReorderStepNum;
            dgv.RowsRemoved -= Dgv_RowsChanged_ReorderStepNum;
            dgv.RowsRemoved += Dgv_RowsChanged_ReorderStepNum;
            dgv.CellBeginEdit -= Dgv_CellBeginEdit;
            dgv.CellBeginEdit += Dgv_CellBeginEdit;

            if (dgv is CtFoldDataGridView fdgv)
            {
                fdgv.ExpandRequested -= DyeingRowsChanged_RefreshLayout;
                fdgv.ExpandRequested += DyeingRowsChanged_RefreshLayout;
            }
        }

        private void Dgv_RowsChanged_ReorderStepNum(object sender, EventArgs e)
        {
            if (sender is CtDataGridView dgv)
                ReorderStepNum(dgv);
        }

        private void Dgv_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var dgv = sender as CtDataGridView;
            if (dgv.CurrentCell == null || e.RowIndex != dgv.CurrentCell.RowIndex)
                e.Cancel = true;
        }

        /// <summary>
        /// 设置自定义回车跳转
        /// </summary>
        private void SetGridEnterJump(CtDataGridView dgv)
        {
            dgv.CustomEditing = true;
            dgv.EnterKeyAction -= () => HandleGridEnterKey(dgv);
            dgv.EnterKeyAction += () => HandleGridEnterKey(dgv);
        }

        private void Dgv_KeyDown(object sender, KeyEventArgs e)
        {
            var dgv = sender as CtDataGridView;
            if (dgv == null) return;
            int idx = dgv.CurrentCell?.RowIndex ?? 0;
            if (e.KeyCode == Keys.Insert)
            {
                dgv.EndEdit();
                dgv.Rows.Insert(idx, 1);
                ReorderStepNum(dgv);
                dgv.CurrentCell = dgv.Rows[idx].Cells[CodeCol];
                dgv.BeginEdit(true);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                dgv.EndEdit();
                if (idx >= 0 && !dgv.Rows[idx].IsNewRow)
                {
                    dgv.Rows.RemoveAt(idx);
                    ReorderStepNum(dgv);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// 重新排序步骤编号
        /// </summary>
        private void ReorderStepNum(CtDataGridView dgv)
        {
            int step = 1;
            foreach (DataGridViewRow row in dgv.Rows)
                if (!row.IsNewRow) row.Cells[IndexCol].Value = step++;
            // 优化：不立即刷新，交由事件统一刷新
        }

        /// <summary>
        /// 聚焦第一个Code单元格
        /// </summary>
        public void FocusFirstGridCodeCell()
        {
            var dgv = tableLayoutPanel1.Controls.OfType<DataGridView>().FirstOrDefault(x => x.Columns.Count > CodeCol);
            if (dgv == null) return;
            if (dgv is CtFoldDataGridView fdgv)
                fdgv.Expand();
            int rowIndex = 0;
            if (dgv.Rows.Count == 0)
            {
                dgv.Rows.Add();
                dgv.Rows[0].Cells[IndexCol].Value = "1";
            }
            if (string.IsNullOrWhiteSpace(dgv.Rows[rowIndex].Cells[IndexCol].Value?.ToString()))
                dgv.Rows[rowIndex].Cells[IndexCol].Value = "1";
            dgv.CurrentCell = dgv.Rows[rowIndex].Cells[CodeCol];
            dgv.BeginEdit(true);
            dgv.Focus();
            dgv.BeginInvoke(new Action(() =>
            {
                var editingControl = dgv.EditingControl;
                if (editingControl != null)
                {
                    editingControl.Focus();
                    if (editingControl is TextBox tb)
                        tb.SelectAll();
                }
            }));
        }

        private void DataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv?.CurrentCell == null) return;
            if (dgv.CurrentCell.ColumnIndex == UnitCol || dgv.CurrentCell.ColumnIndex == BottleCol)
            {
                if (e.Control is ComboBox combo)
                {
                    combo.SelectionChangeCommitted -= ComboBox_SelectionChangeCommitted;
                    combo.SelectionChangeCommitted += ComboBox_SelectionChangeCommitted;
                }
            }
        }

        private void ComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var dgv = tableLayoutPanel1.Controls.OfType<DataGridView>().FirstOrDefault(x => x.Focused);
            if (dgv != null)
                Dgv_CurrentCellChanged(dgv, EventArgs.Empty);
        }

        private void Dgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv.CurrentCell is DataGridViewComboBoxCell)
                dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void Dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == CodeCol || e.ColumnIndex == BottleCol || e.ColumnIndex == UnitCol)
            {
                var dgv = sender as DataGridView;
                Dgv_CurrentCellChanged(dgv, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 处理回车跳转逻辑
        /// </summary>
        private bool HandleGridEnterKey(CtDataGridView dgv)
        {
            dgv.EndEdit();
            if (dgv.CurrentCell == null) return false;
            int col = dgv.CurrentCell.ColumnIndex;
            int row = dgv.CurrentCell.RowIndex;

            // 染助剂代码列
            if (col == CodeCol)
            {
                string code = dgv.Rows[row].Cells[CodeCol].Value?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(code))
                {

                    dgv.ClearSelection();
                    if (_dropMode == Mode.DYE)
                    {
                        UseLimit();

                        var dyeing = tableLayoutPanel1.Controls
                            .OfType<CtDyeing>()
                            .FirstOrDefault();
                        if (dyeing != null)
                        {
                            // 跳到CtDyeing的Cbo_Type
                            dyeing.Cbo_Type.Focus();
                        }
                        else
                        {
                            if (My_ConPar.Machine.MachineLayout == 0)
                            {
                                this.BeginInvoke(new Action(() => btn_Save.Focus()));
                            }
                            else
                            {
                                AddCtDyeing();
                                var newDyeing = tableLayoutPanel1.Controls
                                    .OfType<CtDyeing>()
                                    .LastOrDefault();
                                if (newDyeing != null)
                                {
                                    newDyeing.Cbo_Type.Focus();
                                }
                            }
                        }
                    }
                    else
                    {
                        this.BeginInvoke(new Action(() => btn_Save.Focus()));
                    }
                    return true;
                }

                // 校验代码
                var assistantDt = My_DataBase.AssistantData.Assistant_details;
                var assistantRows = assistantDt?.Select($"{My_DataBase.ASSISTANT_DETAILS.AssistantCode} = '{code.Replace("'", "''")}'");
                if (assistantRows == null || assistantRows.Length == 0)
                {
                    dgv.BeginEdit(false);
                    My_File.LocalTranslator.ShowMessage("染助剂代码不存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgv.CurrentCell = dgv.Rows[row].Cells[CodeCol];
                    dgv.BeginEdit(true);
                    if (dgv.EditingControl is TextBox tb) tb.SelectAll();
                    return true;
                }

                // 自动填充名称、单位
                var rowData = assistantRows[0];
                dgv.Rows[row].Cells[NameCol].Value = rowData[My_DataBase.ASSISTANT_DETAILS.AssistantName];
                if (rowData.Table.Columns.Contains(My_DataBase.ASSISTANT_DETAILS.UnitOfAccount))
                {
                    var unitCell = dgv.Rows[row].Cells[UnitCol] as DataGridViewComboBoxCell;
                    unitCell.Value = rowData[My_DataBase.ASSISTANT_DETAILS.UnitOfAccount];
                }

                // 填充瓶号下拉项
                //var bottleCell = dgv.Rows[row].Cells[BottleCol] as DataGridViewComboBoxCell;
                //bottleCell.Items.Clear();
                //dgv.Rows[row].Cells[BottleCol].Style.BackColor = Color.White;
                //var bottleDt = My_DataBase.BottleData.Bottle_details;
                //var bottleRows = bottleDt?.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{code.Replace("'", "''")}'");
                //if (bottleRows != null && bottleRows.Length > 0)
                //{
                //    foreach (var bRow in bottleRows)
                //    {
                //        var bottleNum = bRow[My_DataBase.BOTTLE_DETAILS.BottleNum].ToString();
                //        if (!bottleCell.Items.Contains(bottleNum))
                //            bottleCell.Items.Add(bottleNum);
                //    }
                //    bottleCell.Value = bottleCell.Items[0];
                //}
                //else
                //{
                //    bottleCell.Items.Add("无");
                //    bottleCell.Value = "无";
                //    dgv.Rows[row].Cells[BottleCol].Style.BackColor = Color.Red;
                //}

                // 跳转到用量
                this.BeginInvoke(new Action(() =>
                {
                    dgv.CurrentCell = dgv.Rows[row].Cells[DosageCol];
                    dgv.BeginEdit(true);
                }));
                return true;
            }

            // 配方用量列
            if (col == DosageCol)
            {
                string dosageStr = dgv.Rows[row].Cells[DosageCol].Value?.ToString();
                if (string.IsNullOrWhiteSpace(dosageStr))
                {
                    dgv.BeginEdit(false);
                    My_File.LocalTranslator.ShowMessage("配方用量不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgv.CurrentCell = dgv.Rows[row].Cells[DosageCol];
                    dgv.BeginEdit(true);
                    return true;
                }

                string code = dgv.Rows[row].Cells[CodeCol].Value?.ToString()?.Trim();
                string unit = dgv.Rows[row].Cells[UnitCol].Value?.ToString()?.Trim();

                double maxDosage = (unit == "G/l" || unit == "g/l") ? My_ConPar.Other.AdditivesAlarmWeight : My_ConPar.Other.DyeAlarmWeight;
                double d = Convert.ToDouble(dosageStr);
                if (d > maxDosage)
                {
                    My_File.LocalTranslator.ShowMessage($"配方用量超过允许最大值", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // 只做目标滴液量等相关计算，不再清空/重建瓶号下拉项
                var bottleCell = dgv.Rows[row].Cells[BottleCol] as DataGridViewComboBoxCell;
                var (bathRatio, clothWeight, totalWeight) = GetHeadValues();
                double formulaDosage = ParseDouble(dosageStr);
                double realConc = 0;
                DateTime brewTime = DateTime.Now;
                if (bottleCell != null && bottleCell.Value != null && bottleCell.Value.ToString() != "无")
                {
                    var bottleDt = My_DataBase.BottleData.Bottle_details;
                    var bottleRows = bottleDt?.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{code.Replace("'", "''")}' AND {My_DataBase.BOTTLE_DETAILS.BottleNum} = '{bottleCell.Value}'");
                    if (bottleRows != null && bottleRows.Length > 0)
                    {
                        realConc = ParseDouble(bottleRows[0][My_DataBase.BOTTLE_DETAILS.RealConcentration]);
                        brewTime = Convert.ToDateTime(bottleRows[0][My_DataBase.BOTTLE_DETAILS.BrewingData]);
                        dgv.Rows[row].Cells[SetConcCol].Value = realConc;
                        dgv.Rows[row].Cells[RealConcCol].Value = bottleRows[0][My_DataBase.BOTTLE_DETAILS.RealConcentration];
                    }
                }

                double objDropWeight = 0;
                if (unit == "%")
                    objDropWeight = CalcDropWeight_Percent(code, formulaDosage, clothWeight, realConc, brewTime);
                else if (unit == "g/l" || unit == "G/L")
                    objDropWeight = CalcDropWeight_GramPerLiter(code, formulaDosage, totalWeight, realConc, brewTime);

                dgv.Rows[row].Cells[ObjDropCol].Value = string.Format("{0:F" + this._retainDecimals + "}", objDropWeight);
                dgv.Rows[row].Cells[RealDropCol].Value = string.Format("{0:F" + this._retainDecimals + "}", 0);

                // 跳转到下一行
                int nextRow = row + 1;
                if (nextRow >= dgv.Rows.Count)
                {
                    dgv.Rows.Add();
                    dgv.Rows[nextRow].Cells[IndexCol].Value = (nextRow + 1).ToString();
                }
                this.BeginInvoke(new Action(() =>
                {
                    dgv.CurrentCell = dgv.Rows[nextRow].Cells[CodeCol];
                    dgv.BeginEdit(true);
                }));
                return true;
            }

            return false;
        }


        /// <summary>
        /// 添加新的染色控件
        /// </summary>
        private void AddCtDyeing()
        {
            Logger.Info("CtDropDetail 添加新的染色控件。");
            CtDyeing newDyeing = new CtDyeing(this._dropHead);
            newDyeing.dgv = InsertConlumn(newDyeing.dgv, 1);
            AutoFitColumns(newDyeing.dgv);
            int expandedHeight = newDyeing.dgv.GetRecommendedHeight() + newDyeing.panel1.Height;
            AddToTableLayout(newDyeing, expandedHeight);
            newDyeing.Focus();
        }

        /// <summary>
        /// 染色控件跳转
        /// </summary>
        private void CtDyeingJump(CtDyeing c = null)
        {
            var controls = tableLayoutPanel1.Controls.OfType<Control>().ToList();
            int currentIndex = c != null ? controls.FindIndex(ctrl => ctrl == c) : controls.FindIndex(ctrl => ctrl.Focus());
            if (c == null && controls.Count > 0 && controls[currentIndex] is DataGridView dgv)
                RemoveRowsWithEmptyCode(c);
            if (currentIndex >= 0 && currentIndex < controls.Count - 1)
            {
                var nextControl = controls[currentIndex + 1];
                nextControl.Focus();
            }
            else
            {
                string dyeingCode = GetHeadDyeCode();
                if (string.IsNullOrEmpty(dyeingCode))
                {
                    if (c != null)
                    {
                        if (c.IsEmpty())
                        {
                            if (this.tableLayoutPanel1.Controls.Count > 2)
                            {
                                using (var dlg = new NewDyeCode(this.tableLayoutPanel1, this._dropHead))
                                {
                                    if (dlg.ShowDialog() == DialogResult.OK)
                                    {
                                        btn_pre.Focus();
                                    }
                                }
                                return;
                            }
                            else
                            {
                                btn_pre.Focus();
                                return;
                            }
                        }
                        else
                        {
                            AddCtDyeing();
                            return;
                        }
                    }
                }
                else
                {
                    btn_pre.Focus();
                    return;
                }
            }
        }

        /// <summary>
        /// 填充明细表数据
        /// </summary>
        private void FillDropDetail(DataTable dt)
        {
            Logger.Info("CtDropDetail 填充明细表数据。");
            this._dgvDatail.Rows.Clear();
            if (dt == null) return;
            foreach (DataRow row in dt.Rows)
            {
                int rowIndex = this._dgvDatail.Rows.Add(
                    row[FieldIndexNum], row[FieldAssistantCode], row[FieldAssistantName], row[FieldFormulaDosage],
                    row[FieldUnitOfAccount], null, row[FieldSettingConcentration], row[FieldRealConcentration],
                    row[FieldObjectDropWeight], string.Format("{0:F" + this._retainDecimals + "}", Convert.ToDouble(row[FieldRealDropWeight])), (row[FieldBottleSelection] != DBNull.Value && Convert.ToInt32(row[FieldBottleSelection]) == 1)
                );
                FillBottleComboBoxItems(this._dgvDatail, rowIndex, row);
            }
            AutoFitColumns(this._dgvDatail);
            RefreshLayout();

        }

        /// <summary>
        /// 填充瓶号下拉项
        /// </summary>
        private void FillBottleComboBoxItems(DataGridView dgv, int rowIndex, DataRow row)
        {
            var bottleCell = dgv.Rows[rowIndex].Cells[BottleCol] as DataGridViewComboBoxCell;
            bottleCell.Items.Clear();

            var bottleDt = My_DataBase.BottleData.Bottle_details;
            var code = row[FieldAssistantCode]?.ToString()?.Trim();
            var bottleRows = bottleDt?.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{code.Replace("'", "''")}'");
            var currentBottleNum = row[FieldBottleNum]?.ToString();

            bool found = false;
            if (bottleRows != null && bottleRows.Length > 0)
            {
                foreach (var bRow in bottleRows)
                {
                    var bottleNum = bRow[My_DataBase.BOTTLE_DETAILS.BottleNum].ToString();

                    if (!bottleCell.Items.Contains(bottleNum))
                        bottleCell.Items.Add(bottleNum);
                }
                if (!string.IsNullOrEmpty(currentBottleNum) && bottleCell.Items.Contains(currentBottleNum))
                {
                    bottleCell.Value = currentBottleNum;
                    found = true;
                }
            }
            if (!found)
            {
                if (_dropHead != null && _dropHead.Source == CtDropHead.DataSource.History && !string.IsNullOrEmpty(currentBottleNum))
                {
                    string oldBottleDisplay = $"{currentBottleNum}(旧)";
                    bottleCell.Items.Add(oldBottleDisplay);
                    bottleCell.Value = oldBottleDisplay;
                    dgv.Rows[rowIndex].Cells[BottleCol].Style.BackColor = Color.Orange;
                }
                else
                {
                    bottleCell.Items.Add("无");
                    bottleCell.Value = "无";
                    dgv.Rows[rowIndex].Cells[BottleCol].Style.BackColor = Color.Red;
                }
            }
        }

        /// <summary>
        /// 设置按钮使能状态
        /// </summary>
        public void SetButtonEnabled()
        {
            My_Form.Login.LoginForm.UserCache.TryGetValue(Properties.Settings.Default.Account, out var userInfo);
            int purview = userInfo.Purview;
            btn_BatchAdd.Enabled = btn_Save.Enabled = btn_upd.Enabled = btn_pre.Enabled =
            btn_FormulaCodeAdd.Enabled = purview == 0;
        }

        /// <summary>
        /// 插入列到DataGridView
        /// </summary>
        private T InsertConlumn<T>(T dgv, int type = 0) where T : DataGridView
        {
            var colIndex = new DataGridViewTextBoxColumn { HeaderText = type == 0 ? "序号" : "名称", ReadOnly = true };
            dgv.Columns.Add(colIndex);

            var colCode = new DataGridViewTextBoxColumn { HeaderText = "染助剂代码" };
            dgv.Columns.Add(colCode);

            var colName = new DataGridViewTextBoxColumn { HeaderText = "染助剂名称", ReadOnly = true };
            dgv.Columns.Add(colName);

            var colDosage = new DataGridViewTextBoxColumn { HeaderText = "配方用量" };
            dgv.Columns.Add(colDosage);

            var colUnit = new DataGridViewComboBoxColumn { HeaderText = "单位", FlatStyle = FlatStyle.Popup };
            colUnit.Items.Add("%");
            colUnit.Items.Add("g/l");
            colUnit.Items.Add("G/L");
            colUnit.Items.Add("Water");
            dgv.Columns.Add(colUnit);

            var colBottle = new DataGridViewComboBoxColumn { HeaderText = "瓶号", FlatStyle = FlatStyle.Popup };
            dgv.Columns.Add(colBottle);

            var colSetConc = new DataGridViewTextBoxColumn { HeaderText = "设定浓度", ReadOnly = true };
            dgv.Columns.Add(colSetConc);

            var colRealConc = new DataGridViewTextBoxColumn { HeaderText = "实际浓度", ReadOnly = true };
            dgv.Columns.Add(colRealConc);

            var colObjDrop = new DataGridViewTextBoxColumn { HeaderText = "目标滴液量", ReadOnly = true };
            dgv.Columns.Add(colObjDrop);

            var colRealDrop = new DataGridViewTextBoxColumn { HeaderText = "实际滴液量", ReadOnly = true };
            dgv.Columns.Add(colRealDrop);

            var colManualBottle = new DataGridViewCheckBoxColumn { HeaderText = "手动选瓶", ReadOnly = true };
            dgv.Columns.Add(colManualBottle);

            return dgv;
        }

        /// <summary>
        /// 智能自适应所有列宽度
        /// </summary>
        private void AutoFitColumns(DataGridView dgv)
        {
            if (dgv.Rows.Count == 1)
            {
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    if (col.Visible)
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            else
            {
                if (dgv is CtDataGridView cdgv)
                    cdgv.AutoFitAllColumns();
                else if (dgv is CtFoldDataGridView fdgv)
                    fdgv.AutoFitAllColumns();
            }
            dgv.ClearSelection();
        }

        private void Dgv_CurrentCellChanged(object sender, EventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null || dgv.CurrentCell == null) return;
            dgv.CellValueChanged -= Dgv_CellValueChanged;
            try
            {
                var cell = dgv.CurrentCell;
                int rowIndex = cell.RowIndex;

                if (cell.ColumnIndex == CodeCol)
                {
                    HandleCodeColumnChanged(dgv, rowIndex);
                }
                else if (cell.ColumnIndex == BottleCol)
                {
                    HandleBottleColumnChanged(dgv, rowIndex);
                }
                else if (cell.ColumnIndex == UnitCol)
                {
                    HandleUnitColumnChanged(dgv, rowIndex);
                }
            }
            finally
            {
                dgv.CellValueChanged += Dgv_CellValueChanged;
            }
        }

        private void HandleCodeColumnChanged(DataGridView dgv, int rowIndex)
        {
            string code = dgv.Rows[rowIndex].Cells[CodeCol].Value?.ToString()?.Trim();
            // 新增：判断当前dgv中是否已存在相同的染助剂代码（排除本行）
            if (!string.IsNullOrEmpty(code))
            {
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    if (i == rowIndex) continue;
                    var cellValue = dgv.Rows[i].Cells[CodeCol].Value?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(cellValue) && cellValue == code)
                    {
                        My_File.LocalTranslator.ShowMessage("同一个染助剂代码不允许重复！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dgv.Rows[rowIndex].Cells[CodeCol].Value = null;
                        dgv.CurrentCell = dgv.Rows[rowIndex].Cells[CodeCol];
                        dgv.BeginEdit(true);
                        return;
                    }
                }
            }

            var bottleCell = dgv.Rows[rowIndex].Cells[BottleCol] as DataGridViewComboBoxCell;
            var oldBottleValue = bottleCell?.Value?.ToString();

            if (bottleCell != null)
            {
                bottleCell.Items.Clear();
                dgv.Rows[rowIndex].Cells[BottleCol].Style.BackColor = Color.White;
            }

            if (string.IsNullOrEmpty(code)) return;

            var dt = My_DataBase.AssistantData.Assistant_details;
            var found = dt?.Select($"{My_DataBase.ASSISTANT_DETAILS.AssistantCode} = '{code.Replace("'", "''")}'");
            if (found == null || found.Length == 0)
            {
                dgv.Rows[rowIndex].Cells[NameCol].Value = null;
                if (bottleCell != null)
                {
                    bottleCell.Items.Add("无");
                    bottleCell.Value = "无";
                    dgv.Rows[rowIndex].Cells[BottleCol].Style.BackColor = Color.Red;
                }
                return;
            }

            var row = found[0];
            dgv.Rows[rowIndex].Cells[NameCol].Value = row[My_DataBase.ASSISTANT_DETAILS.AssistantName];
            if (row.Table.Columns.Contains(My_DataBase.ASSISTANT_DETAILS.UnitOfAccount))
            {
                var unitCell = dgv.Rows[rowIndex].Cells[UnitCol] as DataGridViewComboBoxCell;
                unitCell.Value = row[My_DataBase.ASSISTANT_DETAILS.UnitOfAccount];
            }

            var bottleDt = My_DataBase.BottleData.Bottle_details;
            var bottleRows = bottleDt?.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{code.Replace("'", "''")}'");
            bool hasBottle = false;
            if (bottleCell != null)
            {
                if (bottleRows != null && bottleRows.Length > 0)
                {
                    foreach (var bRow in bottleRows)
                    {
                        var bottleNum = bRow[My_DataBase.BOTTLE_DETAILS.BottleNum].ToString();
                        if (!bottleCell.Items.Contains(bottleNum))
                            bottleCell.Items.Add(bottleNum);
                    }
                    hasBottle = true;
                }
                else
                {
                    bottleCell.Items.Add("无");
                    bottleCell.Value = "无";
                    dgv.Rows[rowIndex].Cells[BottleCol].Style.BackColor = Color.Red;
                }

                if (hasBottle && !string.IsNullOrEmpty(oldBottleValue) && bottleCell.Items.Contains(oldBottleValue))
                {
                    bottleCell.Value = oldBottleValue;
                }
                else if (hasBottle && bottleCell.Items.Count > 0)
                {
                    bottleCell.Value = bottleCell.Items[0];
                }
            }
        }

        private void HandleBottleColumnChanged(DataGridView dgv, int rowIndex)
        {
            var bottleNum = dgv.Rows[rowIndex].Cells[BottleCol].Value?.ToString();
            if (string.IsNullOrEmpty(bottleNum) || bottleNum == "无" || bottleNum.Contains("旧")) return;

            var code = dgv.Rows[rowIndex].Cells[CodeCol].Value?.ToString()?.Trim();
            var bottleDt = My_DataBase.BottleData.Bottle_details;
            var bottleRows = bottleDt?.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{code.Replace("'", "''")}' AND {My_DataBase.BOTTLE_DETAILS.BottleNum} = '{bottleNum}'");
            DateTime brewTime = DateTime.Now;
            if (bottleRows != null && bottleRows.Length > 0)
            {
                var bRow = bottleRows[0];
                brewTime = bRow[My_DataBase.BOTTLE_DETAILS.BrewingData] != DBNull.Value ? Convert.ToDateTime(bRow[My_DataBase.BOTTLE_DETAILS.BrewingData]) : DateTime.Now;
                dgv.Rows[rowIndex].Cells[SetConcCol].Value = bRow[
                    My_DataBase.BOTTLE_DETAILS.SettingConcentration];
                dgv.Rows[rowIndex].Cells[RealConcCol].Value = bRow[My_DataBase.BOTTLE_DETAILS.RealConcentration];
            }

            string unit = dgv.Rows[rowIndex].Cells[UnitCol].Value?.ToString()?.Trim();
            double formulaDosage = ParseDouble(dgv.Rows[rowIndex].Cells[DosageCol].Value);
            double realConc = ParseDouble(dgv.Rows[rowIndex].Cells[RealConcCol].Value);

            var (bathRatio, clothWeight, totalWeight) = GetHeadValues();

            double objDropWeight = 0;
            if (unit == "%")
                objDropWeight = CalcDropWeight_Percent(code, formulaDosage, clothWeight, realConc, brewTime);
            else if (unit == "g/l")
                objDropWeight = CalcDropWeight_GramPerLiter(code, formulaDosage, totalWeight, realConc, brewTime);

            dgv.Rows[rowIndex].Cells[ObjDropCol].Value = string.Format("{0:F" + this._retainDecimals + "}", objDropWeight);
            dgv.Rows[rowIndex].Cells[RealDropCol].Value = string.Format("{0:F" + this._retainDecimals + "}", 0.000); ;

            // 新增：判断是否手动选瓶
            var bottleCell = dgv.Rows[rowIndex].Cells[BottleCol] as DataGridViewComboBoxCell;
            bool isManual = false;
            if (bottleCell != null)
            {
                // 计算系统推荐瓶号
                var bottleDtAll = My_DataBase.BottleData.Bottle_details;
                var bottleRowsAll = bottleDtAll?.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{code.Replace("'", "''")}'", $"{My_DataBase.BOTTLE_DETAILS.RealConcentration} DESC");
                if (bottleRowsAll != null && bottleRowsAll.Length > 0)
                {
                    foreach (var bRowAll in bottleRowsAll)
                    {
                        double dropMinWeight = ParseDouble(
                            bRowAll[My_DataBase.BOTTLE_DETAILS.DropMinWeight]);
                        realConc = ParseDouble(
                            bRowAll[My_DataBase.BOTTLE_DETAILS.RealConcentration]);
                        int bottleNumSys = Convert.ToInt32(
                            bRowAll[My_DataBase.BOTTLE_DETAILS.BottleNum]);
                        brewTime = bRowAll[My_DataBase.BOTTLE_DETAILS.BrewingData] != DBNull.Value ? Convert.ToDateTime(bRowAll[My_DataBase.BOTTLE_DETAILS.BrewingData]) : DateTime.Now;
                        double objDropWeightSys = 0;
                        if (unit == "%")
                            objDropWeightSys = CalcDropWeight_Percent(code, formulaDosage, clothWeight, realConc, brewTime);
                        else if (unit == "g/l")
                            objDropWeightSys = CalcDropWeight_GramPerLiter(code, formulaDosage, totalWeight, realConc, brewTime);

                        if (objDropWeightSys >= dropMinWeight)
                        {
                            // 找到系统推荐瓶号
                            if (bottleNum != bottleNumSys.ToString())
                                isManual = true;
                            break;
                        }
                    }
                }
            }
            dgv.Rows[rowIndex].Cells[ManualBottleCol].Value = isManual;
        }

        private void HandleUnitColumnChanged(DataGridView dgv, int rowIndex)
        {
            string unit = dgv.Rows[rowIndex].Cells[UnitCol].Value?.ToString()?.Trim();
            if (string.IsNullOrEmpty(unit)) return;

            double formulaDosage = ParseDouble(dgv.Rows[rowIndex].Cells[DosageCol].Value);
            double realConc = ParseDouble(dgv.Rows[rowIndex].Cells[RealConcCol].Value);
            var code = dgv.Rows[rowIndex].Cells[CodeCol].Value?.ToString()?.Trim();
            var (bathRatio, clothWeight, totalWeight) = GetHeadValues();
            var bottleNo = dgv.Rows[rowIndex].Cells[BottleCol].Value?.ToString();
            var bottleDt = My_DataBase.BottleData.Bottle_details.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{code.Replace("'", "''")}' AND {My_DataBase.BOTTLE_DETAILS.BottleNum} = '{bottleNo}'");
            DateTime brewTime = DateTime.Now;
            if (bottleDt != null && bottleDt.Length > 0)
            {
                var bRow = bottleDt[0];
                brewTime = bRow[My_DataBase.BOTTLE_DETAILS.BrewingData] != DBNull.Value ? Convert.ToDateTime(bRow[My_DataBase.BOTTLE_DETAILS.BrewingData]) : DateTime.Now;
            }
            double objDropWeight = 0;
            if (unit == "%")
                objDropWeight = CalcDropWeight_Percent(code, formulaDosage, clothWeight, realConc, brewTime);
            else if (unit == "g/l" || unit == "G/L")
                objDropWeight = CalcDropWeight_GramPerLiter(code, formulaDosage, totalWeight, realConc, brewTime);
            else if (unit == "Water")
                objDropWeight = formulaDosage;

            dgv.Rows[rowIndex].Cells[ObjDropCol].Value = objDropWeight;
        }

        private (double bathRatio, double clothWeight, double totalWeight) GetHeadValues()
        {
            double bathRatio = 0, clothWeight = 0, totalWeight = 0;
            if (this._dropHead != null)
            {
                var values = this._dropHead.GetAllInputValues();
                double.TryParse(values["txt_BathRatio"]?.ToString(), out bathRatio);
                double.TryParse(values["txt_ClothWeight"]?.ToString(), out clothWeight);
                double.TryParse(values["txt_TotalWeight"]?.ToString(), out totalWeight);
            }
            return (bathRatio, clothWeight, totalWeight);
        }

        private (string formulaCode, string versionNum) GetHeadFormulaInfo()
        {
            string formulaCode = string.Empty;
            string versionNum = string.Empty;
            if (this._dropHead != null)
            {
                var values = this._dropHead.GetAllInputValues();
                formulaCode = values["txt_FormulaCode"]?.ToString();
                versionNum = values["txt_VersionNum"]?.ToString();
            }
            return (formulaCode, versionNum);
        }

        private string GetHeadDyeCode()
        {
            string dyeCode = string.Empty;
            if (this._dropHead != null)
            {
                var values = this._dropHead.GetAllInputValues();
                dyeCode = values["txt_DyeingCode"]?.ToString();
            }
            return dyeCode;
        }

        public static double ParseDouble(object value)
        {
            double result = 0;
            double.TryParse(value?.ToString(), out result);
            return result;
        }

        public static double CalcDropWeight_Percent(string code, double formulaDosage, double clothWeight, double settingConc, DateTime brewTime)
        {
            int roundDigits = My_Tool.BalanceStableReading.RetainDecimals();
            var dr = My_DataBase.AssistantData.Assistant_details.Select($"{My_DataBase.ASSISTANT_DETAILS.AssistantCode} = '{code.Replace("'", "''")}'").FirstOrDefault();
            if (dr != null)
            {
                var compensationCoefficient = Convert.ToDouble(dr[My_DataBase.ASSISTANT_DETAILS.AllowMinColoringConcentration]);
                var compensationConstant = Convert.ToDouble(dr[My_DataBase.ASSISTANT_DETAILS.AllowMaxColoringConcentration]);
                var ow = Math.Round(formulaDosage * clothWeight / settingConc, roundDigits);
                if (clothWeight > 0 && settingConc > 0)
                {
                    if (compensationCoefficient == 0 && compensationConstant == 0)
                    {
                        //不需要补偿，直接计算

                        return ow;
                    }
                    else
                    {
                        //需要补偿
                        var min = (DateTime.Now - brewTime).TotalMinutes;
                        return Math.Round((ow * (min * compensationCoefficient + compensationConstant) / 100), roundDigits);

                    }
                }


            }

            return 0;
        }

        public static double CalcDropWeight_GramPerLiter(string code, double formulaDosage, double totalWeight, double settingConc, DateTime brewTime)
        {

            int roundDigits = My_Tool.BalanceStableReading.RetainDecimals();
            var dr = My_DataBase.AssistantData.Assistant_details.Select($"{My_DataBase.ASSISTANT_DETAILS.AssistantCode} = '{code.Replace("'", "''")}'").FirstOrDefault();
            if (dr != null)
            {
                var compensationCoefficient = Convert.ToDouble(dr[My_DataBase.ASSISTANT_DETAILS.AllowMinColoringConcentration]);
                var compensationConstant = Convert.ToDouble(dr[My_DataBase.ASSISTANT_DETAILS.AllowMaxColoringConcentration]);
                var ow = Math.Round(formulaDosage * totalWeight / settingConc, roundDigits);
                if (totalWeight > 0 && settingConc > 0)
                {
                    if (compensationCoefficient == 0 && compensationConstant == 0)
                    {
                        //不需要补偿，直接计算

                        return ow;
                    }
                    else
                    {
                        //需要补偿
                        var min = (DateTime.Now - brewTime).TotalMinutes;
                        return Math.Round((ow * (min * compensationCoefficient + compensationConstant) / 100), roundDigits);

                    }
                }


            }

            return 0;
        }

        /// <summary>
        /// 移除所有Code为空的行
        /// </summary>
        public void RemoveRowsWithEmptyCode(DataGridView dgv)
        {
            for (int i = dgv.Rows.Count - 1; i >= 0; i--)
            {
                var row = dgv.Rows[i];
                if (row.IsNewRow) continue;
                var cell = row.Cells[CodeCol];
                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    dgv.Rows.RemoveAt(i);
                }
            }
        }

        public void RemoveRowsWithEmptyCode(CtDyeing dyeing)
        {
            var dgv = dyeing.dgv;
            for (int i = dgv.Rows.Count - 1; i >= 0; i--)
            {
                var row = dgv.Rows[i];
                if (row.IsNewRow) continue;
                var cell = row.Cells[CodeCol];
                var name = row.Cells[IndexCol];

                if ((name.Value == null || string.IsNullOrWhiteSpace(name.Value.ToString())) &&
                    (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString())))
                {
                    dgv.Rows.RemoveAt(i);
                }
            }
        }

        private void Btn_pre_Click(object sender, EventArgs e)
        {
            using (var previewForm = new My_Form.DyeingMan.Preview(this._dropHead, this.tableLayoutPanel1))
            {
                previewForm.ShowDialog();
            }
            btn_Save.Focus();
        }
        private void Btn_FormulaCodeAdd_Click(object sender, EventArgs e)
        {
            this._mode = 1;
            this._dropHead.FocusFormulaCode();
        }

        private void Btn_upd_Click(object sender, EventArgs e)
        {

            this._mode = 2;
            this._dropHead.FocusNext();
        }


        private bool ValidateBeforeSave()
        {
            // 先移除主明细表和所有 CtDyeing 控件中 dgv 的空行
            if (_dgvDatail != null)
                RemoveRowsWithEmptyCode(_dgvDatail);

            // 倒序遍历，安全移除控件
            for (int i = tableLayoutPanel1.Controls.Count - 1; i >= 0; i--)
            {
                if (tableLayoutPanel1.Controls[i] is CtDyeing dyeing)
                {
                    if (string.IsNullOrEmpty(dyeing.Cbo_Type.Text) || string.IsNullOrEmpty(dyeing.Cbo_DDPCode.Text))
                    {
                        tableLayoutPanel1.Controls.RemoveAt(i);
                        continue;
                    }

                    if (dyeing.dgv != null)
                        RemoveRowsWithEmptyCode(dyeing);
                }
            }

            // 校验主明细表
            if (_dgvDatail != null)
            {
                foreach (DataGridViewRow row in _dgvDatail.Rows)
                {
                    if (row.IsNewRow) continue;
                    for (int col = 0; col < _dgvDatail.Columns.Count; col++)
                    {
                        var cell = row.Cells[col];
                        var value = cell.Value;
                        // ComboBoxCell特殊处理
                        if (cell is DataGridViewComboBoxCell)
                        {
                            if (value == null || string.IsNullOrWhiteSpace(value.ToString()) || value.ToString() == "无" || value.ToString().Contains("旧"))
                            {
                                LocalTranslator.ShowMessage($"主明细第{row.Index + 1}行第{col + 1}列不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                _dgvDatail.CurrentCell = cell;
                                return false;
                            }
                        }
                        else
                        {
                            if (!(cell is DataGridViewCheckBoxCell))
                            {
                                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                                {
                                    LocalTranslator.ShowMessage($"主明细第{row.Index + 1}行第{col + 1}列不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    _dgvDatail.CurrentCell = cell;
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            // 校验 CtDyeing 控件中的 dgv
            foreach (var dyeing in tableLayoutPanel1.Controls.OfType<CtDyeing>())
            {
                var type = dyeing.Cbo_Type.Text;
                var dgv = dyeing.dgv;
                var code = dyeing.Cbo_DDPCode.Text;
                var Radio = dyeing.Txt_DDRadio.Text;
                if (string.IsNullOrEmpty(code))
                {
                    LocalTranslator.ShowMessage($"工艺代码不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dyeing.Cbo_DDPCode.Focus();
                    return false;
                }
                if (double.TryParse(Radio, out double radioValue))
                {
                    if (radioValue <= 0)
                    {
                        LocalTranslator.ShowMessage($"浴比比例必须大于0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dyeing.Txt_DDRadio.Focus();
                        return false;
                    }
                }
                else
                {
                    if (type == "后处理工艺")
                    {
                        LocalTranslator.ShowMessage($"浴比必须为数字！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dyeing.Txt_DDRadio.Focus();
                        return false;
                    }
                }

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;
                    for (int col = 0; col < dgv.Columns.Count; col++)
                    {
                        var cell = row.Cells[col];
                        var value = cell.Value;
                        if (cell is DataGridViewComboBoxCell)
                        {
                            if (value == null || string.IsNullOrWhiteSpace(value.ToString()) || value.ToString() == "无" || value.ToString().Contains("旧"))
                            {
                                LocalTranslator.ShowMessage($"{type}第{row.Index + 1}行第{col + 1}列不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                dgv.CurrentCell = cell;
                                return false;
                            }
                        }
                        else
                        {
                            if (!(cell is DataGridViewCheckBoxCell))
                            {
                                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                                {
                                    if (type == "后处理工艺" && cell.OwningColumn.HeaderText == "名称")
                                    {
                                        continue;
                                    }
                                    LocalTranslator.ShowMessage($"{type}第{row.Index + 1}行第{col + 1}列不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    dgv.CurrentCell = cell;
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private List<(string Code, string Type)> GetDyeingTechNamesAndCodesInOrder(string dyeingCode)
        {
            var result = new List<(string, string)>();
            var dt = SmartColor.My_DataBase.DyeingCodeData.Dyeing_code;
            if (dt == null || string.IsNullOrWhiteSpace(dyeingCode))
                return result;

            var rows = dt.Select($"{DYEING_CODE.DyeingCode} = '{dyeingCode.Replace("'", "''")}'", "IndexNum ASC");
            if (rows.Length == 0)
            {
                dt = SmartColor.My_DataBase.DyeingCodeData.History_Dyeing_code;
                rows = dt.Select($"{HISTORY_DYEING_CODE.DyeingCode} = '{dyeingCode.Replace("'", "''")}'", "IndexNum ASC");
            }

            foreach (var row in rows)
            {
                var code = row[DYEING_CODE.Code]?.ToString()?.Trim() ?? "";

                var type = string.Empty;
                switch (Convert.ToInt16(row[DYEING_CODE.Type]))
                {
                    case 1:
                        type = "染色工艺";
                        break;
                    case 2:
                        type = "后处理工艺";
                        break;
                    default:
                        type = "未知工艺";
                        break;
                }
                result.Add((code, type));
            }
            return result;
        }

        /// <summary>
        /// 保存按钮点击事件
        /// </summary>
        private void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Info("CtDropDetail 保存按钮点击，开始保存数据。");
                if (this._mode == 0)
                {
                    My_File.LocalTranslator.ShowMessage("请先选择【新增】或【修改】操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Logger.Info("CtDropDetail 保存失败，未选择新增或修改模式。");
                    return;
                }
                var headdc = this._dropHead.GetAllInputValues();
                var formulaCode = headdc["txt_FormulaCode"]?.ToString();
                var versionNum = headdc["txt_VersionNum"]?.ToString();
                var state = headdc["txt_State"]?.ToString();
                var TotalWeight = Convert.ToDouble(headdc["txt_TotalWeight"]);
                var cupNo = Convert.ToInt16(headdc["txt_CupNum"]);
                if (cupNo == 0)
                {
                    //统一按滴液杯处理
                    if (TotalWeight > My_ConPar.Other.DripMaxWeight)
                    {
                        My_File.LocalTranslator.ShowMessage("超出当前杯允许的最大量！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    double allowMaxWeight = CupCommManager.Instance.GetCupMaxWeight(cupNo);
                    if (TotalWeight > allowMaxWeight)
                    {
                        My_File.LocalTranslator.ShowMessage("超出当前杯允许的最大量！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }


                if (string.IsNullOrWhiteSpace(formulaCode))
                {
                    My_File.LocalTranslator.ShowMessage("配方代码不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                UseLimit();

                if (!ValidateBeforeSave())
                    return;
                bool needRename = false;
                string dyeingCode = headdc["txt_DyeingCode"]?.ToString();
                var dyeingControls = tableLayoutPanel1.Controls.OfType<CtDyeing>().ToList();
                if (!string.IsNullOrWhiteSpace(dyeingCode))
                {
                    var validTechs = GetDyeingTechNamesAndCodesInOrder(dyeingCode); // 顺序列表



                    if (dyeingControls.Count != validTechs.Count)
                    {
                        needRename = true;
                    }
                    else
                    {
                        for (int i = 0; i < dyeingControls.Count; i++)
                        {
                            string type = dyeingControls[i].Cbo_Type.Text?.Trim();
                            string ddpCode = dyeingControls[i].Cbo_DDPCode.Text?.Trim();
                            if (!string.Equals(type, validTechs[i].Type, StringComparison.OrdinalIgnoreCase) ||
                                !string.Equals(ddpCode, validTechs[i].Code, StringComparison.OrdinalIgnoreCase))
                            {
                                needRename = true;
                                break;
                            }
                        }
                    }

                }
                else
                {
                    if (dyeingControls.Count > 0)
                    {
                        needRename = true;
                    }
                }

                if (needRename)
                {
                    using (var dlg = new NewDyeCode(this.tableLayoutPanel1, this._dropHead))
                    {
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            btn_pre.Focus();
                        }
                    }
                    return; // 中断保存
                }

                string oldVersionNum = "0";
                if (this._mode == 1)
                {
                    if (string.IsNullOrEmpty(versionNum) || versionNum == null)
                        versionNum = "0";
                    else
                    {
                        int curVersion = 0;
                        int.TryParse(versionNum?.ToString(), out curVersion);
                        versionNum = (++curVersion).ToString();
                        oldVersionNum = versionNum;
                    }
                }
                else if (this._mode == 2)
                {
                    if (state == "已滴定配方")
                    {
                        //找到当前配方的最大版本号

                        var maxVersionRows = SqlServer.Select(TableFormulaHeadName,
                            $"{FieldFormulaCode} = '{formulaCode}' ORDER BY {FieldVersionNum} DESC");

                        if (maxVersionRows.Rows.Count > 0)
                        {
                            var maxVersionNum = Convert.ToInt32(maxVersionRows.Rows[0][FieldVersionNum]);
                            oldVersionNum = versionNum.ToString();
                            versionNum = (maxVersionNum + 1).ToString();
                        }
                    }
                    else
                    {
                        oldVersionNum = versionNum.ToString();
                    }
                }

                DeleteData(formulaCode, versionNum);

                var fc = SaveFormulaHead(formulaCode, versionNum, oldVersionNum);
                if (fc.Item1 == -1)
                {
                    Logger.Error("CtDropDetail 保存失败，配方错误，滴液量大于总浴量。");
                    return;
                }
                SaveFormulaDetails(formulaCode, versionNum, fc.Item3);
                if (fc.Item2 != null && !string.IsNullOrEmpty(fc.Item2.ToString()))
                    SaveHandleDetails(formulaCode, versionNum, fc.Item2.ToString(), fc.Item3);

                DataTable headdt = null;

                headdt = SqlServer.Select(TableFormulaHeadName, $"{FieldFormulaCode} = '{formulaCode}' AND {FieldVersionNum} = {versionNum}");


                FormulaDataChange?.Invoke(this, EventArgs.Empty);
                ResetMode();


                this._dropHead.FillControlsFromDataTable(headdt);

                My_File.LocalTranslator.ShowMessage("存档成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Logger.Info("CtDropDetail 保存成功。");
            }
            catch (Exception ex)
            {
                Logger.Error("CtDropDetail 保存异常：" + ex.Message, ex);
            }
        }


        private void DeleteData(string formulaCode, string versionNum)
        {
            SmartColor.My_DataBase.SqlServer.Delete(TableFormulaHeadName, $"{FieldFormulaCode}= @FormulaCode AND {FieldVersionNum}=@VersionNum",
                new System.Data.SqlClient.SqlParameter("@FormulaCode", formulaCode),
                new System.Data.SqlClient.SqlParameter("@VersionNum", versionNum));
            SmartColor.My_DataBase.SqlServer.Delete(TableFormulaDetailsName, $"{FieldFormulaCode}= @FormulaCode AND {FieldVersionNum} = @VersionNum",
                new System.Data.SqlClient.SqlParameter("@FormulaCode", formulaCode),
                new System.Data.SqlClient.SqlParameter("@VersionNum", versionNum));
            if (_dropMode == Mode.DYE)
                SmartColor.My_DataBase.SqlServer.Delete(My_DataBase.FORMULA_HANDLE_DETAILS.TableName, $"{My_DataBase.FORMULA_HANDLE_DETAILS.FormulaCode}=@FormulaCode AND {My_DataBase.FORMULA_HANDLE_DETAILS.VersionNum} =@VersionNum",
                new System.Data.SqlClient.SqlParameter("@FormulaCode", formulaCode),
                new System.Data.SqlClient.SqlParameter("@VersionNum", versionNum));
        }

        private (int, object, List<_batchInfo>) SaveFormulaHead(string formulaCode, string versionNum, string oldVersionNum)
        {
            var headValues = _dropHead.GetAllInputValues();
            var headDict = new Dictionary<string, object>();
            foreach (var field in TableDefinition.TableSchemas[TableFormulaHeadName])
            {
                if (headValues.ContainsKey("txt_" + field.Name))
                {
                    if (field.Name == "IsAutoIn" || field.Name == "AddWaterChoose")
                        headDict[field.Name] = ((bool)headValues["txt_" + field.Name]) ? 1 : 0;
                    else
                        headDict[field.Name] = headValues["txt_" + field.Name];
                }
                else if (headValues.ContainsKey(field.Name))
                {

                    headDict[field.Name] = headValues[field.Name];
                }
            }

            var dc = headValues["txt_DyeingCode"];
            string handleBRList = string.Empty;

            if (false == double.TryParse(headValues["txt_TotalWeight"].ToString(), out double objectAddWaterWeight))
                objectAddWaterWeight = 0;
            if (false == double.TryParse(headValues["txt_ClothWeight"].ToString(), out double clothWeight))
                clothWeight = 0;
            if (false == double.TryParse(headValues["txt_AnhydrationWR"].ToString(), out double anhydrationWR))
                anhydrationWR = 0;

            for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
            {
                if (tableLayoutPanel1.Controls[i] is CtDataGridView dataGridView)
                {
                    foreach (var row in dataGridView.Rows.OfType<DataGridViewRow>())
                    {
                        if (row.IsNewRow) continue;
                        if (row.Cells[IndexCol].Value != null && !string.IsNullOrEmpty(row.Cells[ObjDropCol].Value?.ToString()))
                        {
                            double.TryParse(row.Cells[ObjDropCol].Value?.ToString(), out double needAdd);
                            objectAddWaterWeight -= needAdd;
                        }
                    }
                }
                else if (tableLayoutPanel1.Controls[i] is CtDyeing cd)
                {
                    if (cd.IsEmpty())
                    {
                        handleBRList += "0|";
                        continue;
                    }
                    var it = cd.AllHeadValues();
                    if (it.Item1 == 1)
                    {
                        handleBRList += (headValues["txt_BathRatio"] + "|");
                        if (i == 1)
                        {
                            // 新增：扣除染固色工艺第一个排液分段的加药量
                            string dyeingCode = headValues["txt_DyeingCode"]?.ToString();
                            string formulaCode_ = headValues["txt_FormulaCode"]?.ToString();
                            int versionNum_ = 0;
                            int.TryParse(headValues["txt_VersionNum"]?.ToString(), out versionNum_);
                            if (!string.IsNullOrEmpty(dyeingCode) && !string.IsNullOrEmpty(formulaCode_) && versionNum_ > 0)
                            {
                                var dyeDetailList = FindDyeingCode.GetAllDyeDetailFromFormulaCode(dyeingCode, formulaCode_, versionNum_);
                                foreach (var dict in dyeDetailList)
                                {
                                    string techName = dict.ContainsKey(My_DataBase.DYE_DETAILS.TechnologyName) ? dict[My_DataBase.DYE_DETAILS.TechnologyName]?.ToString() : "";
                                    if (techName.Contains("排液"))
                                    {
                                        break;
                                    }
                                    if (techName.StartsWith("加") && techName != "加水")
                                    {
                                        double needAdd = 0;
                                        if (dict.ContainsKey(My_DataBase.DYE_DETAILS.ObjectDropWeight))
                                            double.TryParse(dict[My_DataBase.DYE_DETAILS.ObjectDropWeight]?.ToString(), out needAdd);
                                        objectAddWaterWeight -= needAdd;
                                    }
                                }
                            }
                        }
                    }
                    else if (it.Item1 == 2)
                    {
                        handleBRList += (it.Item3 + "|");
                        if (i == 1)
                        {
                            // 新增：扣除染固色工艺第一个排液分段的加药量
                            string dyeingCode = headValues["txt_DyeingCode"]?.ToString();
                            string formulaCode_ = headValues["txt_FormulaCode"]?.ToString();
                            int versionNum_ = 0;
                            int.TryParse(headValues["txt_VersionNum"]?.ToString(), out versionNum_);
                            if (!string.IsNullOrEmpty(dyeingCode) && !string.IsNullOrEmpty(formulaCode_) && versionNum_ > 0)
                            {
                                var dyeDetailList = FindDyeingCode.GetAllDyeDetailFromFormulaCode(dyeingCode, formulaCode_, versionNum_);
                                foreach (var dict in dyeDetailList)
                                {
                                    string techName = dict.ContainsKey(My_DataBase.DYE_DETAILS.TechnologyName) ? dict[My_DataBase.DYE_DETAILS.TechnologyName]?.ToString() : "";
                                    if (techName.Contains("排液"))
                                    {
                                        break;
                                    }
                                    if (techName.StartsWith("加") && techName != "加水")
                                    {
                                        double needAdd = 0;
                                        if (dict.ContainsKey(My_DataBase.DYE_DETAILS.ObjectDropWeight))
                                            double.TryParse(dict[My_DataBase.DYE_DETAILS.ObjectDropWeight]?.ToString(), out needAdd);
                                        objectAddWaterWeight -= needAdd;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            objectAddWaterWeight -= clothWeight * anhydrationWR;
            int roundDigits = My_Tool.BalanceStableReading.RetainDecimals();
            objectAddWaterWeight = Math.Round(objectAddWaterWeight, roundDigits);
            if (objectAddWaterWeight < 0)
            {
                My_File.LocalTranslator.ShowMessage("配方错误,滴液量>总浴量,请检查！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (-1, null, new List<_batchInfo>());
            }

            headDict[FieldObjectAddWaterWeight] = Convert.ToInt16(headDict["AddWaterChoose"]) == 0 ? 0 : objectAddWaterWeight;
            headDict[FieldOperator] = Properties.Settings.Default.Account;
            headDict[FieldHandleBRList] = handleBRList;
            headDict[FieldFormulaCode] = formulaCode;
            headDict[FieldVersionNum] = versionNum;
            headDict[FieldCreateTime] = DateTime.Now;
            headDict[FieldStage] = (dc == null || string.IsNullOrEmpty(dc.ToString())) ? "滴液" : "后处理";
            headDict["State"] = "尚未滴液";
            SmartColor.My_DataBase.SqlServer.Insert(TableFormulaHeadName, headDict);




            var waitDt = SqlServer.Select(TableWaitListName, $"{FieldWaitFormulaCode} = '{formulaCode.Replace("'", "''")}' AND {FieldWaitVersionNum} = '{oldVersionNum.Replace("'", "''")}'");
            if (waitDt != null && waitDt.Rows.Count > 0)
            {
                Dictionary<string, object> wait = new Dictionary<string, object>()
                {
                    [FieldWaitVersionNum] = versionNum,
                    [FieldWaitType] = (dc == null || string.IsNullOrEmpty(dc.ToString())) ? "0" : "1"
                };
                foreach (DataRow dataRow in waitDt.Rows)
                {
                    var id = dataRow["MyID"].ToString();
                    SmartColor.My_DataBase.SqlServer.Update(TableWaitListName, wait, $"{FieldWaitMyID}=@MyID ",
                    new System.Data.SqlClient.SqlParameter("@MyID", id));

                }
                WaitChange?.Invoke(this, EventArgs.Empty);
            }

            var batchDt = SqlServer.Select(TableDropHeadName, $"{FieldDropFormulaCode} = '{formulaCode.Replace("'", "''")}' AND {FieldDropVersionNum} = '{Convert.ToInt32(oldVersionNum.Replace("'", "''"))}' AND {FieldState} = '{"尚未滴液"}' ");
            if (batchDt != null && batchDt.Rows.Count > 0)
            {
                headDict.Remove(FieldDropCupNum);
                var batchDict = new List<_batchInfo>();
                foreach (DataRow dataRow in batchDt.Rows)
                {
                    var id = dataRow[FieldDropMyID].ToString();
                    var cup = dataRow[FieldDropCupNum].ToString();
                    if (headDict.ContainsKey(FieldClothNum) && headDict[FieldClothNum] != null && Convert.ToInt16(headDict[FieldClothNum]) == 0)
                    {
                        headDict.Remove(FieldClothNum);
                    }
                    SmartColor.My_DataBase.SqlServer.Update(TableDropHeadName, headDict, $"{FieldDropMyID} = @MyID ",
                        new System.Data.SqlClient.SqlParameter("@MyID", id));
                    var batchInfo = new _batchInfo
                    {
                        HeadID = id,
                        CupNum = cup
                    };
                    batchDict.Add(batchInfo);
                }
                return (0, dc, batchDict);
            }
            return (0, dc, new List<_batchInfo>());
        }

        private void SaveFormulaDetails(string formulaCode, string versionNum, List<_batchInfo> infos)
        {
            var detailList = new List<Dictionary<string, object>>();
            if (_dgvDatail != null)
            {
                for (int i = 0; i < _dgvDatail.Rows.Count; i++)
                {
                    var row = _dgvDatail.Rows[i];
                    if (row.IsNewRow) continue;
                    var dict = new Dictionary<string, object>
                    {
                        [FieldFormulaCode] = formulaCode,
                        [FieldVersionNum] = versionNum,
                        [FieldIndexNum] = row.Cells[IndexCol].Value,
                        [FieldAssistantCode] = row.Cells[CodeCol].Value,
                        [FieldAssistantName] = row.Cells[NameCol].Value,
                        [FieldFormulaDosage] = row.Cells[DosageCol].Value,
                        [FieldUnitOfAccount] = row.Cells[UnitCol].Value,
                        [FieldBottleNum] = row.Cells[BottleCol].Value,
                        [FieldSettingConcentration] = row.Cells[SetConcCol].Value,
                        [FieldRealConcentration] = row.Cells[RealConcCol].Value,
                        [FieldObjectDropWeight] = row.Cells[ObjDropCol].Value,
                        [FieldRealDropWeight] = row.Cells[RealDropCol].Value,
                        [FieldBottleSelection] = (row.Cells[ManualBottleCol].Value is bool b && b) ? 1 : 0
                    };
                    detailList.Add(dict);
                }
                if (detailList.Count > 0)
                    SmartColor.My_DataBase.SqlServer.Insert(TableFormulaDetailsName, detailList);

                if (infos != null && infos.Count > 0)
                {
                    foreach (var keyValuePair in infos)
                    {
                        var batchDetailList = new List<Dictionary<string, object>>();
                        foreach (var dict in detailList)
                        {
                            var newDict = new Dictionary<string, object>(dict)
                            {
                                [FieldDropHeadID] = keyValuePair.HeadID,
                                [FieldDropCupNum] = keyValuePair.CupNum
                            };
                            batchDetailList.Add(newDict);

                        }
                        SmartColor.My_DataBase.SqlServer.Delete(TableDropDetailsName, $"{FieldDropHeadID} = @HeadID ",
                          new System.Data.SqlClient.SqlParameter("@HeadID", Convert.ToInt32(keyValuePair.HeadID)));
                        SmartColor.My_DataBase.SqlServer.Insert(TableDropDetailsName, batchDetailList);
                    }
                }
            }
        }

        private void SaveHandleDetails(string formulaCode, string versionNum, string dyeingCode, List<_batchInfo> infos)
        {
            if (_dropMode == Mode.ABS)
            {
                // ABS模式下不保存后处理明细
                return;
            }
            var controls = tableLayoutPanel1.Controls.OfType<CtDyeing>().ToList();
            for (int i = 0; i < controls.Count; i++)
            {
                var dyeingHead = controls[i].AllHeadValues();
                int type = dyeingHead.Item1;
                string code = dyeingHead.Item2;
                double radio = dyeingHead.Item3;

                var dyeingDetailList = new List<Dictionary<string, object>>();
                var dgv = controls[i].dgv;
                string tn = string.Empty;
                for (int j = 0; j < dgv.Rows.Count; j++)
                {
                    var row = dgv.Rows[j];
                    if (row.IsNewRow) continue;
                    if (!(row.Cells[IndexCol].Value is DBNull || row.Cells[IndexCol].Value == null))
                        tn = row.Cells[IndexCol].Value.ToString();

                    var dict = new Dictionary<string, object>
                    {
                        [My_DataBase.FORMULA_HANDLE_DETAILS.DyeingCode] = dyeingCode,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.Code] = code,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.TechnologyName] = tn,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.FormulaCode] = formulaCode,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.VersionNum] = versionNum,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.AssistantCode] = row.Cells[CodeCol].Value,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.FormulaDosage] = row.Cells[DosageCol].Value,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.UnitOfAccount] = row.Cells[UnitCol].Value,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.BottleNum] = row.Cells[BottleCol].Value,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.SettingConcentration] = row.Cells[SetConcCol].Value,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.RealConcentration] = row.Cells[RealConcCol].Value,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.AssistantName] = row.Cells[NameCol].Value,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.ObjectDropWeight] = row.Cells[ObjDropCol].Value,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.RealDropWeight] = row.Cells[RealDropCol].Value,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.BottleSelection] = (row.Cells[ManualBottleCol].Value is bool b && b) ? 1 : 0,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.Finish] = 0,
                        [My_DataBase.FORMULA_HANDLE_DETAILS.No] = i + 1
                    };
                    dyeingDetailList.Add(dict);
                }
                if (dyeingDetailList.Count > 0)
                    SmartColor.My_DataBase.SqlServer.Insert(My_DataBase.FORMULA_HANDLE_DETAILS.TableName, dyeingDetailList);
            }

            if (infos != null && infos.Count > 0)
            {
                var head = this._dropHead.GetAllInputValues();
                string code = head["txt_DyeingCode"]?.ToString() ?? "";
                var detailList = FindDyeingCode.GetAllDyeDetailFromFormulaCode(code, formulaCode, Convert.ToInt32(versionNum));
                foreach (var keyValuePair in infos)
                {
                    var batchDetailList = new List<Dictionary<string, object>>();
                    foreach (var dict in detailList)
                    {
                        var newDict = new Dictionary<string, object>(dict)
                        {
                            [My_DataBase.DYE_DETAILS.HeadID] = keyValuePair.HeadID,
                            [My_DataBase.DYE_DETAILS.CupNum] = keyValuePair.CupNum
                        };


                        batchDetailList.Add(newDict);
                    }
                    SmartColor.My_DataBase.SqlServer.Delete(My_DataBase.DYE_DETAILS.TableName, $"{My_DataBase.DYE_DETAILS.HeadID} =@HeadID ",
                        new System.Data.SqlClient.SqlParameter("@HeadID", Convert.ToInt32(keyValuePair.HeadID)));
                    SmartColor.My_DataBase.SqlServer.Insert(My_DataBase.DYE_DETAILS.TableName, batchDetailList);
                }
            }
        }



        public void ResetMode()
        {
            this._mode = 0;
        }



        private void btn_BatchAdd_Click(object sender, EventArgs e)
        {

            string formulaCode = string.Empty;
            string versionNum = string.Empty;
            (formulaCode, versionNum) = GetHeadFormulaInfo();
            List<(string FormulaCode, string VersionNum)> needAddBatch = new List<(string, string)>();
            needAddBatch.Add((formulaCode, versionNum));
            if (this.formulaBrowse != null)
            {
                var selectedBatch = this.formulaBrowse.GetSelectedFormulaCodes();
                if (selectedBatch != null && selectedBatch.Count > 0)
                {
                    needAddBatch.Clear();
                    needAddBatch.AddRange(selectedBatch);
                }
            }

            foreach (var batch in needAddBatch)
            {
                AddBatch(batch.FormulaCode, batch.VersionNum);
            }


            BatchChange?.Invoke(this, 0);

            Logger.Info("批次添加成功。");
        }


        private void AddBatch(string formulaCode, string versionNum)
        {

            if (string.IsNullOrWhiteSpace(formulaCode))
            {
                My_File.LocalTranslator.ShowMessage("配方代码不能为空！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(versionNum) || this._mode != 0)
            {
                My_File.LocalTranslator.ShowMessage("请先保存再加入批次", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var headRows = SqlServer.Select(TableFormulaHeadName,
                $"{FieldFormulaCode} = '{formulaCode.Replace("'", "''")}' AND {FieldVersionNum} = '{versionNum.Replace("'", "''")}'");
            if (headRows == null || headRows.Rows.Count == 0)
            {
                headRows = SqlServer.Select(FORMULA_HEAD_TEMP.TableName,
            $"{FORMULA_HEAD_TEMP.FormulaCode} = '{formulaCode.Replace("'", "''")}' AND {FORMULA_HEAD_TEMP.VersionNum} = '{versionNum.Replace("'", "''")}'");
                if (headRows == null || headRows.Rows.Count == 0)
                {
                    My_File.LocalTranslator.ShowMessage("未找到对应配方表头数据！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // 获取配方类型（滴液/后处理）

            int formulaType = string.IsNullOrEmpty(headRows.Rows[0][FORMULA_HEAD.DyeingCode]?.ToString()) ? 0 : 1;


            // 杯号处理
            int cupNum = 0;
            int parsedCupNum = 0;
            int.TryParse(headRows.Rows[0][FORMULA_HEAD.CupNum]?.ToString(), out parsedCupNum);
            if (parsedCupNum > 0) cupNum = parsedCupNum;

            int? assignedCupNum = GetAvailableCupNum(formulaCode, versionNum, cupNum, formulaType);
            if (assignedCupNum == null)
            {
                return;
            }
            // 构造表头数据
            SqlServer.Delete(TableDropHeadName, $"{FieldCupNum} = {assignedCupNum}");
            var headDict = new Dictionary<string, object>();
            var colDROPHead = TableDefinition.TableSchemas[TableDropHeadName].ToDictionary(f => f.Name, f => f.Name);
            foreach (var col in colDROPHead.Keys)
            {
                if (headRows.Rows[0].Table.Columns.Contains(col))
                    headDict[col] = headRows.Rows[0][col];
            }
            headDict[FieldDropCupNum] = assignedCupNum; // 使用分配的杯号
            if (_dropMode == Mode.DYE)
            {
                if (headDict[FieldClothNum] is int clothNum && clothNum == 0)
                {
                    headDict[FieldClothNum] = assignedCupNum;
                }
            }

            headDict[FieldDropBatchName] = null;
            headDict.Remove(FieldDropMyID);
            int id = My_DataBase.SqlServer.InsertAndGetIdentity(TableDropHeadName, headDict);

            var cupNumStr = assignedCupNum.ToString();

            // 明细数据
            SqlServer.Delete(TableDropDetailsName, $"{FieldCupNum} = {assignedCupNum}");
            var detailRows = SqlServer.Select(TableFormulaDetailsName,
                $"{FieldFormulaCode} = '{formulaCode.Replace("'", "''")}' AND {FieldVersionNum} = '{versionNum.Replace("'", "''")}'");
            if (detailRows != null && detailRows.Rows.Count > 0)
            {
                var detailList = new List<Dictionary<string, object>>();
                foreach (DataRow dr in detailRows.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    var colDROPDetails = TableDefinition.TableSchemas[TableDropDetailsName].ToDictionary(f => f.Name, f => f.Name);
                    foreach (var col in colDROPDetails.Keys)
                    {
                        if (dr.Table.Columns.Contains(col))
                            dict[col] = dr[col];
                    }
                    dict[FieldDropHeadID] = id;
                    dict[FieldDropCupNum] = cupNumStr;
                    dict[FieldDropBatchName] = null;
                    detailList.Add(dict);
                }
                if (detailList.Count > 0)
                    My_DataBase.SqlServer.Insert(TableDropDetailsName, detailList);
            }

            // 染色明细数据
            if (_dropMode == Mode.DYE)
            {
                SqlServer.Delete(My_DataBase.DYE_DETAILS.TableName, $"{DYE_DETAILS.CupNum} = {assignedCupNum}");
                var head = headRows.Rows[0];
                string code = head[FORMULA_HEAD.DyeingCode]?.ToString() ?? "";
                var detailList = FindDyeingCode.GetAllDyeDetailFromFormulaCode(code, formulaCode, Convert.ToInt32(versionNum));
                foreach (var dict in detailList)
                {
                    dict[My_DataBase.DYE_DETAILS.HeadID] = id;
                    dict[My_DataBase.DYE_DETAILS.CupNum] = cupNumStr; // 每条染色明细加上杯号
                    dict[My_DataBase.DYE_DETAILS.BatchName] = null;
                }
                if (detailList.Count > 0)
                    My_DataBase.SqlServer.Insert(My_DataBase.DYE_DETAILS.TableName, detailList);
                var (mainCupinfo, subCupinfo) = My_Tool.CupAuxiliary.GetMSCupInfo(Convert.ToInt16(cupNumStr));
                if (mainCupinfo.CupNum != subCupinfo.CupNum)
                {
                    int currentCupNum = mainCupinfo.CupNum == Convert.ToInt16(cupNumStr) ? mainCupinfo.CupNum : subCupinfo.CupNum;
                    int otherCupNum = mainCupinfo.CupNum == currentCupNum ? subCupinfo.CupNum : mainCupinfo.CupNum;
                    SyncMainSubCupStepNo(currentCupNum, otherCupNum);
                }


            }
        }

        /// <summary>
        /// 批次添加时自动分配杯号，返回分配的杯号（如无可用则加入等待列表并返回null）
        /// formulaType: 0=滴液，1=后处理
        /// </summary>
        private int? GetAvailableCupNum(string formulaCode, string versionNum, int? cupNum, int formulaType)
        {
            int type = formulaType == 0 ? 2 : 3;

            // 获取当前染固色代码
            string currentDyeingCode = string.Empty;
            var headValues = SqlServer.Select(TableFormulaHeadName, $"{FieldFormulaCode} = '{formulaCode}' AND {FieldVersionNum} = '{versionNum}'");
            if (headValues != null && headValues.Rows.Count > 0)
            {
                currentDyeingCode = headValues.Rows[0][FORMULA_HEAD.DyeingCode]?.ToString() ?? "";
            }
            else
            {
                headValues = SqlServer.Select(FORMULA_HEAD_TEMP.TableName, $"{FORMULA_HEAD_TEMP.FormulaCode} = '{formulaCode}' AND {FORMULA_HEAD_TEMP.VersionNum} = '{versionNum}'");
                if (headValues != null && headValues.Rows.Count > 0)
                {
                    currentDyeingCode = headValues.Rows[0][FORMULA_HEAD_TEMP.DyeingCode]?.ToString() ?? "";
                }
            }



            // 1. 客户指定杯号
            if (cupNum.HasValue && cupNum.Value > 0)
            {
                // 查杯详情表是否存在
                var cupDt = SqlServer.Select(CUP_DETAILS.TableName, $"{FieldCupNum} = {cupNum.Value}");
                if (cupDt == null || cupDt.Rows.Count == 0)
                {
                    // 杯号不存在，走系统分配逻辑（暂不处理）
                    LocalTranslator.ShowMessage($"{cupNum.Value}号配液杯不存在,请修改后再加入", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                //判断类型是否一致
                if (Convert.ToInt16(cupDt.Rows[0][FeildType]) != type)
                {
                    //指定的杯号类型和配方类型不一致
                    LocalTranslator.ShowMessage($"{cupNum.Value}号配液杯类型和配方类型不一致,请修改后再加入", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return null;
                }

                //判断当前杯是否再用
                var (mainCupinfo, subCupinfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNum.Value);

                if (mainCupinfo.CupNum == subCupinfo.CupNum)
                {
                    // 主副杯相同，直接判断一个杯即可
                    if (mainCupinfo.IsUsing == 1 || mainCupinfo.Enable == 0)
                    {
                        AddToWaitList(formulaCode, versionNum, formulaType, cupNum.Value);
                        return null;
                    }

                }
                else
                {
                    if (mainCupinfo.CupNum == cupNum.Value)
                    {
                        // 指定杯是主杯，判断主杯
                        if (mainCupinfo.IsUsing == 1 || mainCupinfo.Enable == 0)
                        {
                            AddToWaitList(formulaCode, versionNum, formulaType, cupNum.Value);
                            return null;
                        }
                    }
                    if (subCupinfo.CupNum == cupNum.Value)
                    {
                        // 指定杯是副杯，判断副杯

                        if (subCupinfo.IsUsing == 1 || subCupinfo.Enable == 0)
                        {
                            AddToWaitList(formulaCode, versionNum, formulaType, cupNum.Value);
                            return null;
                        }
                    }
                }




                // 2. 等待列表是否有用该杯号
                bool existsInWaitList = SqlServer.Select(TableWaitListName, $"{FieldWaitCupNum} = {cupNum.Value}")?.Rows.Count > 0;
                if (existsInWaitList)
                {
                    AddToWaitList(formulaCode, versionNum, formulaType, cupNum.Value);
                    return null;
                }

                // 3. 批次表是否有用该杯号
                bool existsInBatch = SqlServer.Select(TableDropHeadName, $"{FieldDropCupNum} = {cupNum.Value}")?.Rows.Count > 0;
                if (existsInBatch)
                {
                    AddToWaitList(formulaCode, versionNum, formulaType, cupNum.Value);
                    return null;
                }

                if (type == 3)
                {

                    // 4. 查主副杯
                    if (mainCupinfo.CupNum != subCupinfo.CupNum)
                    {
                        // 主副杯不同，查另一杯
                        var otherInfo = mainCupinfo.CupNum == cupNum.Value ? subCupinfo : mainCupinfo;
                        if (otherInfo.Enable == 0)
                        {
                            // 另一杯如果下线，直接返回该杯号
                            return cupNum.Value;
                        }
                        else if (otherInfo.IsUsing == 1)
                        {
                            // 另一杯在用，加入等待列表
                            AddToWaitList(formulaCode, versionNum, formulaType, cupNum.Value);
                            return null;
                        }
                        else
                        {
                            // 3. 批次表是否有用另一个杯号
                            var otherDropData = SqlServer.Select(TableDropHeadName, $"{FieldDropCupNum} = {otherInfo.CupNum}");
                            existsInBatch = otherDropData.Rows.Count > 0;
                            if (existsInBatch)
                            {
                                // 另一杯在批次表有用
                                string pairDyeingCode = otherDropData.Rows[0][DROP_HEAD.DyeingCode]?.ToString();

                                if (pairDyeingCode != currentDyeingCode)
                                {
                                    // 染固色代码不一致，加入等待列表
                                    AddToWaitList(formulaCode, versionNum, formulaType, cupNum.Value);
                                    return null;
                                }
                                else
                                {
                                    //染固色代码一致直接返回该杯号
                                    return cupNum.Value;
                                }
                            }
                            else
                            {
                                // 另一杯不在批次表有用，直接返回该杯号

                                // 2. 等待列表是否有用另一杯杯号
                                existsInWaitList = SqlServer.Select(TableWaitListName, $"{FieldWaitCupNum} = {otherInfo.CupNum}")?.Rows.Count > 0;
                                if (existsInWaitList)
                                {
                                    AddToWaitList(formulaCode, versionNum, formulaType, cupNum.Value);
                                    return null;
                                }
                                else
                                {
                                    return cupNum.Value;
                                }


                            }
                        }



                    }
                    else
                    {
                        // 主副杯相同，直接返回该杯号
                        return cupNum.Value;
                    }
                }
                else
                {
                    //滴液区
                    return cupNum.Value;
                }

            }


            // 2. 系统分配杯号
            else if (!cupNum.HasValue || cupNum.Value == 0)
            {
                // 1. 查批次表已用的杯号
                string sql = $"SELECT DISTINCT {FieldDropCupNum} FROM {TableDropHeadName} WHERE {FieldDropCupNum} > 0";
                var usedCupNumsDt = SqlServer.ExecuteQuery(sql);
                var usedCupNums = new HashSet<int>(
                    usedCupNumsDt.AsEnumerable()
                        .Select(r => Convert.ToInt32(r[FieldDropCupNum])));



                // 2. 查杯详情表，enable=1，未被批次表用的杯号，升序遍历
                // 构造 usedCupNums 的 SQL 片段
                string usedCupNumsSql = usedCupNums.Count > 0
                    ? $"AND {FieldCupNum} NOT IN ({string.Join(",", usedCupNums)})"
                    : "";

                // 构造 SQL 查询语句
                string sqlWhere = $"{FieldEnable} = 1 AND {FeildType} = {type} AND {FieldIsUsing} = 0 {usedCupNumsSql}";

                // 查询数据库
                var availableRowsDt = SqlServer.Select(
                      CUP_DETAILS.TableName,
                      null,
                      sqlWhere,
                      FieldCupNum,
                      true
                  );

                var availableRows = availableRowsDt?.Rows.Cast<DataRow>().ToList();

                if (availableRows == null || availableRows.Count == 0)
                {
                    // 没有可用杯，加入等待列表
                    AddToWaitList(formulaCode, versionNum, formulaType, 0);
                    return null;
                }



                // 3. 遍历每个可用杯号，查其主副杯（另一杯）
                foreach (var row in availableRows)
                {
                    int cup = Convert.ToInt32(row[FieldCupNum]);
                    if (type == 3)
                    {
                        var (mainCupinfo, subCupinfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cup);
                        if (mainCupinfo.CupNum != subCupinfo.CupNum)
                        {
                            CupDetailInfo otherInfo = mainCupinfo.CupNum == cup ? subCupinfo : mainCupinfo;
                            if (otherInfo.Enable == 0)
                            {
                                //另一杯下线
                                return cup; // 直接返回该杯号
                            }
                            if (otherInfo.IsUsing == 1)
                            {
                                continue; // 另一杯在用，跳过
                            }
                            else
                            {
                                // 3. 批次表是否有用另一个杯号
                                var otherDropData = SqlServer.Select(TableDropHeadName, $"{FieldDropCupNum} = {otherInfo.CupNum}");
                                var existsInBatch = otherDropData.Rows.Count > 0;
                                if (existsInBatch)
                                {
                                    // 另一杯在批次表有用

                                    string pairDyeingCode = otherDropData.Rows[0][DROP_HEAD.DyeingCode]?.ToString();

                                    if (pairDyeingCode != currentDyeingCode)
                                    {
                                        // 染固色代码不一致
                                        continue;
                                    }
                                    else
                                    {
                                        //染固色代码一致直接返回该杯号
                                        return cup;
                                    }
                                }
                                else
                                {
                                    // 另一杯不在批次表有用，直接返回该杯号
                                    return cup;
                                }


                            }



                        }
                        else
                        {
                            // 2. 等待列表是否有用该杯号
                            bool existsInWaitList = SqlServer.Select(TableWaitListName, $"{FieldWaitCupNum} = {cup}")?.Rows.Count > 0;
                            if (existsInWaitList)
                            {
                                AddToWaitList(formulaCode, versionNum, formulaType, cupNum.Value);
                                return null;
                            }
                            else
                            {

                                return cup;
                            }

                        }
                    }
                    else
                    {
                        //滴液区只判断当前杯
                        return cup;
                    }
                }
            }

            // 3. 无可用杯，加入等待列表
            AddToWaitList(formulaCode, versionNum, formulaType, cupNum ?? 0);
            return null;

        }


        /// <summary>
        /// 加入等待列表
        /// </summary>
        private void AddToWaitList(string formulaCode, string versionNum, int formulaType, int cupNum)
        {
            // 从内存DataTable获取最大IndexNum


            string sql = $"SELECT ISNULL(MAX({FieldWaitIndexNum}), 0) FROM {TableWaitListName}";
            object maxObj = SmartColor.My_DataBase.SqlServer.ExecuteScalar(sql);
            int newIndexNum = Convert.ToInt32(maxObj) + 1;

            var waitDict = new Dictionary<string, object>
            {
                [FieldWaitFormulaCode] = formulaCode,
                [FieldWaitVersionNum] = versionNum,
                [FieldWaitIndexNum] = newIndexNum,
                [FieldWaitCupNum] = cupNum,
                [FieldWaitType] = formulaType
            };
            SmartColor.My_DataBase.SqlServer.Insert(TableWaitListName, waitDict);


            WaitChange?.Invoke(this, EventArgs.Empty);
        }

        public void ReadOnly()
        {
            foreach (Control control in this.tableLayoutPanel1.Controls)
            {
                if (control is My_Control.CtDyeing ctDyeing)
                {
                    ctDyeing.ReadOnly();
                }
                if (control is My_Control.CtDataGridView ctdgv)
                {
                    ctdgv.Enabled = false;
                }
            }
            this.btn_BatchAdd.Visible = false;
            this.btn_pre.Visible = false;
            this.btn_Save.Visible = false;
            this.btn_FormulaCodeAdd.Visible = false;
            this.btn_upd.Visible = false;


        }
    }
}