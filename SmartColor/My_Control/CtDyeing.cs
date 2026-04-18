using SmartColor.My_DataBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 染色工艺主控件，负责染助剂配方的录入、校验、瓶号分配等核心逻辑。
    /// 支持染色工艺与后处理工艺两种模式，自动处理数据源切换、事件通知、明细表填充等。
    /// </summary>
    public partial class CtDyeing : UserControl
    {
        // DataGridView 列索引常量，便于维护和代码可读性
        private const int IndexCol = 0;         // 工艺名称列
        private const int CodeCol = 1;          // 助剂代码列
        private const int NameCol = 2;          // 助剂名称列
        private const int DosageCol = 3;        // 用量列
        private const int UnitCol = 4;          // 单位列
        private const int BottleCol = 5;        // 母液瓶号列
        private const int SetConcCol = 6;       // 设定浓度列
        private const int RealConcCol = 7;      // 实际浓度列
        private const int ObjDropCol = 8;       // 目标滴液量列
        private const int RealDropCol = 9;      // 实际滴液量列
        private const int ManualBottleCol = 10; // 手动瓶号选择列

        private CtDropHead _dropHead = null;    // 染色头部控件引用

        // 工艺相关参数
        private double _clothWeight = 0;        // 布重
        private double _totalWeight = 0;        // 总浴量
        private double _bathRatio = 0;          // 浴比

        /// <summary>
        /// 小数点保留位数
        /// </summary>
        private int _retainDecimals = 2;

        /// <summary>
        /// 所有行处理完毕事件，外部可订阅
        /// </summary>
        public event EventHandler<string> OnAllRowsProcessed;

        /// <summary>
        /// 构造函数，初始化控件及事件绑定
        /// </summary>
        /// <param name="dropHead">染色头部控件</param>
        public CtDyeing(CtDropHead dropHead)
        {
            InitializeComponent();
           // label1.Visible = false;
            //Txt_DDRadio.Visible = false;
            this.dgv.AllowUserToAddRows = false;
            InitKeyboardNavigation(); // 初始化回车跳转逻辑
            this.dgv.CustomEditing = true;
            this.dgv.EnterKeyAction += () => HandleGridEnterKey();
            this._dropHead = dropHead;
            (this._bathRatio, this._clothWeight, this._totalWeight) = GetHeadValues();
            BindDropHead(); // 绑定头部控件事件
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
            SqlServer.TableDataChanged += SqlServer_TableDataChanged;
            this._retainDecimals = My_Tool.BalanceStableReading.RetainDecimals();
            var dic = dropHead.GetAllInputValues();
            this.Txt_DDRadio.Text = Convert.ToString(dic["txt_BathRatio"]);
        }

        private void SqlServer_TableDataChanged(string obj)
        {
            if (obj == My_DataBase.DYEING_PROCESS.TableName)
            {
                Cbo_Type_SelectionChangeCommitted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// DataGridView 回车键处理，校验数据并自动跳转编辑单元格
        /// </summary>
        private bool HandleGridEnterKey()
        {
            this.dgv.EndEdit();
            if (this.dgv.CurrentCell == null) return false;
            int col = this.dgv.CurrentCell.ColumnIndex;
            int row = this.dgv.CurrentCell.RowIndex;

            var codeCell = this.dgv.Rows[row].Cells[CodeCol];
            var dosageCell = this.dgv.Rows[row].Cells[DosageCol];

            // 校验染助剂代码
            if (col == CodeCol)
            {
                string code = codeCell.Value?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(code))
                {
                    // 后处理工艺允许工艺名称为空时直接跳到下一行
                    if (Cbo_Type.Text == "后处理工艺")
                    {
                        var name = this.dgv.Rows[row].Cells[IndexCol].Value?.ToString();
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            int nextRow = row + 1;
                            if (nextRow >= this.dgv.Rows.Count)
                            {
                                RemoveRowsWithEmptyCode();
                                this.OnAllRowsProcessed?.Invoke(this, string.Empty);
                                My_File.Logger.Info("所有行已处理完毕（染色工艺）");
                                return true;
                            }
                            this.BeginInvoke(new Action(() =>
                            {
                                this.dgv.CurrentCell = this.dgv.Rows[nextRow].Cells[CodeCol];
                                this.dgv.BeginEdit(true);
                            }));
                            return true;
                        }
                    }
                    // 代码为空，提示并回到当前单元格
                    My_File.Logger.Info("染助剂代码为空");
                    My_File.LocalTranslator.ShowMessage("染助剂代码不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.dgv.CurrentCell = codeCell;
                    this.dgv.BeginEdit(true);
                    return true;
                }
                // 校验助剂代码是否存在
                var assistantDt = My_DataBase.AssistantData.Assistant_details;
                if (assistantDt == null)
                {
                    My_File.Logger.Error("助剂数据表未初始化");
                    return true;
                }
                var assistantRows = assistantDt?.Select($"{My_DataBase.ASSISTANT_DETAILS.AssistantCode} = '{code.Replace("'", "''")}'");
                if (assistantRows == null || assistantRows.Length == 0)
                {
                    My_File.LocalTranslator.ShowMessage("染助剂代码不存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.dgv.CurrentCell = codeCell;
                    this.dgv.BeginEdit(true);
                    return true;
                }
                // 校验瓶号是否有实际浓度
                var bottleDt = My_DataBase.BottleData.Bottle_details;
                var bottleRows = bottleDt?.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{code.Replace("'", "''")}' AND ISNULL({My_DataBase.BOTTLE_DETAILS.RealConcentration}, 0) <> 0");
                if (bottleRows == null || bottleRows.Length == 0)
                {
                    My_File.Logger.Info($"未找到实际浓度不为0的瓶号: {code}");
                    My_File.LocalTranslator.ShowMessage("未找到实际浓度不为0的瓶号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.dgv.CurrentCell = codeCell;
                    this.dgv.BeginEdit(true);
                    return true;
                }
                // 跳到用量列
                this.BeginInvoke(new Action(() =>
                {
                    this.dgv.CurrentCell = this.dgv.Rows[row].Cells[DosageCol];
                    this.dgv.BeginEdit(true);
                }));
                return true;
            }

            // 校验配方用量
            if (col == DosageCol)
            {
                string dosageStr = dosageCell.Value?.ToString();
                if (string.IsNullOrWhiteSpace(dosageStr))
                {
                    My_File.Logger.Info("配方用量为空");
                    My_File.LocalTranslator.ShowMessage("配方用量不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.dgv.CurrentCell = dosageCell;
                    this.dgv.BeginEdit(true);
                    return true;
                }

                string code = this.dgv.Rows[row].Cells[CodeCol].Value?.ToString()?.Trim();
                string unit = this.dgv.Rows[row].Cells[UnitCol].Value?.ToString()?.Trim();

                // 优化：瓶号查找逻辑提取为方法
                bool foundBottle = TryFindBottle(row, code, dosageCell.Value, unit);

                if (!foundBottle)
                {
                    My_File.Logger.Info($"没有任何母液瓶满足最小滴液量要求: {code}, 用量: {dosageStr}, 单位: {unit}");
                    My_File.LocalTranslator.ShowMessage("没有任何母液瓶满足最小滴液量要求！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.dgv.CurrentCell = dosageCell;
                    this.dgv.BeginEdit(true);
                    return true;
                }

                // 工艺类型判断，自动插入或跳转新行
                if (Cbo_Type.Text == "后处理工艺")
                {
                    if (row + 1 >= this.dgv.Rows.Count)
                    {
                        // 最后一行，插入新行
                        this.dgv.Rows.Add(1);
                        My_File.Logger.Info("插入新行: 最后一行");
                        int newRowIndex = this.dgv.Rows.Count - 1;
                        this.BeginInvoke(new Action(() =>
                        {
                            this.dgv.CurrentCell = this.dgv.Rows[newRowIndex].Cells[CodeCol];
                            this.dgv.BeginEdit(true);
                        }));
                        return true;
                    }
                    var nextRow = this.dgv.Rows[row + 1];
                    bool nextRowIsEmpty = (nextRow.Cells[IndexCol].Value == null || string.IsNullOrWhiteSpace(nextRow.Cells[IndexCol].Value?.ToString()));
                    if (!nextRowIsEmpty)
                    {
                       
                       
                            int insertRow = row + 1;
                            this.dgv.Rows.Insert(insertRow, 1);
                            My_File.Logger.Info($"插入新行: {insertRow}");
                            this.BeginInvoke(new Action(() =>
                            {
                                this.dgv.CurrentCell = this.dgv.Rows[insertRow].Cells[CodeCol];
                                this.dgv.BeginEdit(true);
                            }));
                        
                    }
                    else
                    {
                        bool nextRowIsEmpty1 = (nextRow.Cells[CodeCol].Value == null || string.IsNullOrWhiteSpace(nextRow.Cells[CodeCol].Value?.ToString()));
                        if (!nextRowIsEmpty1)
                        {
                            int insertRow = row + 1;
                            this.BeginInvoke(new Action(() =>
                            {
                                this.dgv.CurrentCell = this.dgv.Rows[insertRow].Cells[CodeCol];
                                this.dgv.BeginEdit(true);
                            }));
                        }
                        else
                        {

                            // 已有空行，直接跳到空行
                            int lastRowIndex = this.dgv.Rows.Count - 1;
                            this.BeginInvoke(new Action(() =>
                            {
                                this.dgv.CurrentCell = this.dgv.Rows[lastRowIndex].Cells[CodeCol];
                                this.dgv.BeginEdit(true);
                            }));
                        }
                    }
                    return true;
                }
                else // 染色工艺
                {
                    int nextRow = row + 1;
                    if (nextRow >= this.dgv.Rows.Count)
                    {
                        RemoveRowsWithEmptyCode();
                        this.OnAllRowsProcessed?.Invoke(this, string.Empty);
                        My_File.Logger.Info("所有行已处理完毕（染色工艺）");
                        return true;
                    }
                    this.BeginInvoke(new Action(() =>
                    {
                        this.dgv.CurrentCell = this.dgv.Rows[nextRow].Cells[CodeCol];
                        this.dgv.BeginEdit(true);
                    }));
                    return true;
                }
            }

            // 默认跳到当前行第二列
            this.BeginInvoke(new Action(() =>
            {
                this.dgv.CurrentCell = this.dgv.Rows[row].Cells[CodeCol];
                this.dgv.BeginEdit(true);
            }));
            return true;
        }

        /// <summary>
        /// 母液瓶号查找及赋值逻辑，自动计算滴液量，优化重复代码
        /// </summary>
        /// <param name="row">行索引</param>
        /// <param name="code">助剂代码</param>
        /// <param name="dosageValue">用量</param>
        /// <param name="unit">单位</param>
        /// <returns>是否找到合适瓶号</returns>
        private bool TryFindBottle(int row, string code, object dosageValue, string unit)
        {
            var bottleDt = My_DataBase.BottleData.Bottle_details;
            var bottleRows = bottleDt?.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{code.Replace("'", "''")}'", $"{My_DataBase.BOTTLE_DETAILS.RealConcentration} DESC");
            var bottleCell = this.dgv.Rows[row].Cells[BottleCol] as DataGridViewComboBoxCell;

            // 清空相关单元格
            //if (bottleCell != null)
            //{
            //    bottleCell.Value = null;
            //    bottleCell.Items.Clear();
            //}
            this.dgv.Rows[row].Cells[SetConcCol].Value = null;
            this.dgv.Rows[row].Cells[RealConcCol].Value = null;
            this.dgv.Rows[row].Cells[ObjDropCol].Value = null;

            List<string> allBottleNums = new List<string>();
            if (bottleRows != null && bottleRows.Length > 0)
            {
                foreach (var bRow in bottleRows)
                {
                    var bottleNum = bRow[My_DataBase.BOTTLE_DETAILS.BottleNum].ToString();
                    if (!bottleCell.Items.Contains(bottleNum))
                    {
                        bottleCell.Items.Add(bottleNum);
                        allBottleNums.Add(bottleNum);
                    }
                }
            }

            if (bottleRows != null && bottleRows.Length > 0)
            {
                double formulaDosage = CtDropDetail.ParseDouble(dosageValue);

                foreach (var bRow in bottleRows)
                {
                    double settingConc = CtDropDetail.ParseDouble(bRow[My_DataBase.BOTTLE_DETAILS.SettingConcentration]);
                    double realConc = CtDropDetail.ParseDouble(bRow[My_DataBase.BOTTLE_DETAILS.RealConcentration]);
                    double dropMinWeight = CtDropDetail.ParseDouble(bRow[My_DataBase.BOTTLE_DETAILS.DropMinWeight]);
                    int bottleNum = Convert.ToInt32(bRow[My_DataBase.BOTTLE_DETAILS.BottleNum]);
                    var brewTime = bRow[My_DataBase.BOTTLE_DETAILS.BrewingData] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(bRow[My_DataBase.BOTTLE_DETAILS.BrewingData]);

                    double objDropWeight = 0;
                    // 根据单位计算目标滴液量
                    if (unit == "%")
                        objDropWeight = CtDropDetail.CalcDropWeight_Percent( code,formulaDosage, this._clothWeight, realConc, brewTime);
                    else if (unit == "g/l")
                        objDropWeight = CtDropDetail.CalcDropWeight_GramPerLiter(
                            code,
                            formulaDosage,
                            Cbo_Type.Text == "后处理工艺" ? this._clothWeight * Convert.ToDouble(Txt_DDRadio.Text) : this._totalWeight,
                            realConc,
                            brewTime);

                    // 满足最小滴液量要求
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
                        this.dgv.Rows[row].Cells[SetConcCol].Value = settingConc;
                        this.dgv.Rows[row].Cells[RealConcCol].Value = realConc;
                        this.dgv.Rows[row].Cells[ObjDropCol].Value = string.Format("{0:F" + this._retainDecimals + "}", objDropWeight);
                        this.dgv.Rows[row].Cells[RealDropCol].Value = string.Format("{0:F" + this._retainDecimals + "}", 0);
                        My_File.Logger.Info($"找到合适瓶号: {bottleNumStr}, 行: {row}");
                        return true;
                    }
                }
            }

            // 没有找到合适的母液瓶，瓶号单元格背景设为红色
            if (bottleCell != null)
            {
                bottleCell.Value = null;
                bottleCell.Style.BackColor = Color.Red;
            }
            My_File.Logger.Info($"未找到合适瓶号: {code}, 行: {row}");
            return false;
        }

        /// <summary>
        /// 工艺类型下拉框选中事件，自动填充工艺代码下拉项
        /// </summary>
        private void Cbo_Type_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Cbo_DDPCode.Text = String.Empty;
             var choice = Cbo_Type.Text;
            if (string.IsNullOrEmpty(choice)) return;

            DataTable dt = My_DataBase.DyeingData.Dyeing_process;
            Cbo_DDPCode.Items.Clear();
            if (choice == "染色工艺")
            {
                var dp = dt?.AsEnumerable().Where(r => r.Field<int>(My_DataBase.DYEING_PROCESS.Type) == 1 && r[My_DataBase.DYEING_PROCESS.Code] != DBNull.Value)
                        .GroupBy(r => r.Field<string>(My_DataBase.DYEING_PROCESS.Code))
                        .Select(g => g.First());
                foreach (var row in dp)
                {
                    Cbo_DDPCode.Items.Add(row.Field<string>(My_DataBase.DYEING_PROCESS.Code));
                }
                
                Txt_DDRadio.Text = _bathRatio.ToString();
            }
            else if (choice == "后处理工艺")
            {
                var dp = dt?.AsEnumerable().Where(r => r.Field<int>(My_DataBase.DYEING_PROCESS.Type) == 2 && r[My_DataBase.DYEING_PROCESS.Code] != DBNull.Value)
                       .GroupBy(r => r.Field<string>(My_DataBase.DYEING_PROCESS.Code))
                       .Select(g => g.First());
                foreach (var row in dp)
                {
                    Cbo_DDPCode.Items.Add(row.Field<string>(My_DataBase.DYEING_PROCESS.Code));
                }
                
                Txt_DDRadio.Text = _bathRatio.ToString();
            }
            else
            {
                My_File.LocalTranslator.ShowMessage("未知的工艺类型选择！", "Cbo_Type_SelectionChangeCommitted", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 初始化控件间回车跳转逻辑，提升录入效率
        /// </summary>
        private void InitKeyboardNavigation()
        {
            // 工艺类型回车跳转
            Cbo_Type.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(Cbo_Type.Text))
                    {
                        this.OnAllRowsProcessed?.Invoke(this, string.Empty);
                        e.Handled = true;
                    }
                    else
                    {
                        Cbo_DDPCode.Focus();
                        e.Handled = true;
                    }
                }
            };
            // 工艺代码回车跳转
            Cbo_DDPCode.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (Txt_DDRadio.Visible)
                    {
                        Txt_DDRadio.Focus();
                        e.Handled = true;
                    }
                    else
                    {
                        if (this.dgv.Rows.Count > 0 && this.dgv.ColumnCount > 1)
                        {
                            this.dgv.Expand();
                            this.dgv.CurrentCell = this.dgv.Rows[0].Cells[1];
                            this.dgv.BeginEdit(true);
                        }
                        else
                        {
                            this.OnAllRowsProcessed?.Invoke(this, string.Empty);
                        }
                        e.Handled = true;
                    }
                }
            };
            // 浴比输入框回车跳转
            Txt_DDRadio.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.dgv.Rows.Count > 0 && this.dgv.ColumnCount > 1)
                    {
                        this.dgv.Expand();
                        this.dgv.CurrentCell = this.dgv.Rows[0].Cells[1];
                        this.dgv.BeginEdit(true);
                    }
                    else
                    {
                        this.OnAllRowsProcessed?.Invoke(this, string.Empty);
                    }
                    e.Handled = true;
                }
            };
        }

        /// <summary>
        /// 工艺代码下拉框选中事件，自动填充所有“加X”工艺到明细表
        /// </summary>
        private void Cbo_DDPCode_SelectionChangeCommitted(object sender, EventArgs e)
        {


            DataTable dt = My_DataBase.DyeingData.Dyeing_process;

            if (dt == null) return;

            var selectedCode = Cbo_DDPCode.Text;
            if (string.IsNullOrEmpty(selectedCode)) return;

            var addRows = dt?.AsEnumerable()
                .Where(r => r.Field<string>(My_DataBase.DYEING_PROCESS.Code) == selectedCode
                    && (r.Field<string>(My_DataBase.DYEING_PROCESS.TechnologyName)?.Contains("加") ?? false)
                    && !(r.Field<string>(My_DataBase.DYEING_PROCESS.TechnologyName)?.Contains("加水") ?? false))
                .GroupBy(r => r.Field<string>(My_DataBase.DYEING_PROCESS.TechnologyName))
                .Select(g => g.First())
                .ToList();

            this.dgv.Rows.Clear();
            if (addRows != null && addRows.Count > 0)
            {
                foreach (var row in addRows)
                {
                    int idx = this.dgv.Rows.Add();
                    this.dgv.Rows[idx].Cells[0].Value = row.Field<string>(My_DataBase.DYEING_PROCESS.TechnologyName);
                }
            }
            else
            {
                dt = My_DataBase.DyeingData.History_Dyeing_process;

                if (dt == null) return;

                selectedCode = Cbo_DDPCode.Text;
                if (string.IsNullOrEmpty(selectedCode)) return;

                addRows = dt?.AsEnumerable()
                   .Where(r => r.Field<string>(My_DataBase.HISTORY_DYEING_PROCESS.Code) == selectedCode
                       && (r.Field<string>(My_DataBase.HISTORY_DYEING_PROCESS.TechnologyName)?.Contains("加") ?? false)
                       && !(r.Field<string>(My_DataBase.HISTORY_DYEING_PROCESS.TechnologyName)?.Contains("加水") ?? false))
                   .GroupBy(r => r.Field<string>(My_DataBase.HISTORY_DYEING_PROCESS.TechnologyName))
                   .Select(g => g.First())
                   .ToList();

                this.dgv.Rows.Clear();
                if (addRows != null)
                {
                    foreach (var row in addRows)
                    {
                        int idx = this.dgv.Rows.Add();
                        this.dgv.Rows[idx].Cells[0].Value = row.Field<string>(My_DataBase.HISTORY_DYEING_PROCESS.TechnologyName);
                    }
                }
            }



            this.dgv.ClearSelection();
            this.dgv.Expand();
        }

        /// <summary>
        /// 绑定头部控件事件，自动响应布重/浴比变化
        /// </summary>
        public void BindDropHead()
        {
            _dropHead.ClothWeightChanged -= ClothWeightChangedHandler;
            _dropHead.BathRatioChanged -= BathRatioChangedHandler;
            _dropHead.ClothWeightChanged += ClothWeightChangedHandler;
            _dropHead.BathRatioChanged += BathRatioChangedHandler;
        }

        /// <summary>
        /// 释放资源，解绑事件
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dropHead != null)
                {
                    _dropHead.ClothWeightChanged -= ClothWeightChangedHandler;
                    _dropHead.BathRatioChanged -= BathRatioChangedHandler;
                }
                SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
            }
            base.Dispose(disposing);
        }

        // 布重变化事件处理
        private void ClothWeightChangedHandler(object s, object val)
        {
            (this._bathRatio, this._clothWeight, this._totalWeight) = GetHeadValues();
            _clothWeight = Convert.ToDouble(val);
            RecalculateAllRows();
        }
        // 浴比变化事件处理
        private void BathRatioChangedHandler(object s, object val)
        {
            (this._bathRatio, this._clothWeight, this._totalWeight) = GetHeadValues();
            _bathRatio = Convert.ToDouble(val);
            //if (Cbo_Type.Text == "后处理工艺")
                Txt_DDRadio.Text = val.ToString();
            RecalculateAllRows();
        }

        /// <summary>
        /// 重新计算所有行的瓶号分配，适应布重/浴比变化
        /// </summary>
        private void RecalculateAllRows()
        {
            for (int row = 0; row < this.dgv.Rows.Count; row++)
            {
                var codeCell = this.dgv.Rows[row].Cells[CodeCol];
                var dosageCell = this.dgv.Rows[row].Cells[DosageCol];
                var unitCell = this.dgv.Rows[row].Cells[UnitCol];

                string code = codeCell.Value?.ToString()?.Trim();
                string dosageStr = dosageCell.Value?.ToString();
                string unit = unitCell.Value?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(dosageStr) || string.IsNullOrWhiteSpace(unit))
                    continue;

                TryFindBottle(row, code, dosageCell.Value, unit);
            }
        }

        /// <summary>
        /// 获取头部输入框的所有数值（浴比、布重、总浴量）
        /// </summary>
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

        /// <summary>
        /// 删除 DataGridView 中所有助剂代码为空的行
        /// </summary>
        public void RemoveRowsWithEmptyCode()
        {
            for (int i = this.dgv.Rows.Count - 1; i >= 0; i--)
            {
                var cell = this.dgv.Rows[i].Cells[CodeCol];
                if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    this.dgv.Rows.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 填充染固色工艺控件数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="code">工艺代码</param>
        /// <param name="radio">浴比</param>
        /// <param name="dt">数据</param>
        /// <param name="dyeInfo">染色头部信息</param>
        public void FillControlsFromDataTable(int type, string code, double radio, DataTable dt, CtDropHead.DyeInfo dyeInfo)
        {

            Cbo_Type.Text = type == 1 ? "染色工艺" : "后处理工艺";
          //  Txt_DDRadio.Visible = type == 2;
            if (!Cbo_DDPCode.Items.Contains(code))
            {
                Cbo_DDPCode.Items.Add(code);


            }
            Cbo_DDPCode.Text = code;
            Txt_DDRadio.Text = radio.ToString();

            if (dt != null)
            {
                FillDyeDetail(dt, dyeInfo);
            }

        }

        /// <summary>
        /// 填充明细表数据，自动处理瓶号下拉项
        /// </summary>
        private void FillDyeDetail(DataTable dt, CtDropHead.DyeInfo dyeInfo)
        {
            string batchName = string.Empty;


           

            string lastTn = null;
            string currentTn = null;



          


            if (this.dgv.Rows.Count == 0)
            {
                this.dgv.Rows.Clear();
                if (dt == null) return;


                foreach (DataRow row in dt.Rows)
                {

                    currentTn = row[My_DataBase.FORMULA_HANDLE_DETAILS.TechnologyName]?.ToString();
                    if (string.IsNullOrEmpty(currentTn))
                    {
                        continue;
                    }
                    int index = Convert.ToInt32(row[My_DataBase.FORMULA_HANDLE_DETAILS.No]);
                    string assistantCode = row[My_DataBase.FORMULA_HANDLE_DETAILS.AssistantCode]?.ToString();
                    double readDropWeight = 0;

                    // 处理实际滴液量，支持配方/批次/历史三种来源
                    DataTable batchDt = null;
                    if (this._dropHead.Source == 0)
                    {
                        // 配方表，实际滴液量默认为0
                    }
                    else if (this._dropHead.Source == CtDropHead.DataSource.Batch)
                    {
                        batchDt = My_DataBase.DropBatchData.GetDyeData();
                        string indexNumN = My_DataBase.DYE_DETAILS.IndexNum;
                        string technologyNameN = My_DataBase.DYE_DETAILS.TechnologyName;
                        string headIDN = My_DataBase.DYE_DETAILS.HeadID;
                        string assistantCodeN = My_DataBase.DYE_DETAILS.AssistantCode;
                        string realDropWeightN = My_DataBase.DYE_DETAILS.RealDropWeight;

                        var batchRows = batchDt?.Select($"{indexNumN} = {index} AND {technologyNameN} = '{currentTn}' AND {headIDN} = {dyeInfo.ID} AND {assistantCodeN} = '{assistantCode}'");
                        if (batchRows != null && batchRows.Length > 0)
                        {
                            foreach (var bRow in batchRows)
                            {
                                if (bRow[realDropWeightN] != DBNull.Value)
                                {
                                    readDropWeight += Convert.ToDouble(bRow[realDropWeightN]);
                                }
                            }
                            row[My_DataBase.FORMULA_HANDLE_DETAILS.RealDropWeight] = string.Format("{0:F" + this._retainDecimals + "}", readDropWeight);
                        }
                    }
                    else if (this._dropHead.Source == CtDropHead.DataSource.History)
                    {
                        // 直接用 No + TechnologyName + AssistantCode + HeadID 精确查找
                        int no = row.Table.Columns.Contains(My_DataBase.FORMULA_HANDLE_DETAILS.No) && row[My_DataBase.FORMULA_HANDLE_DETAILS.No] != DBNull.Value
                            ? Convert.ToInt32(row[My_DataBase.FORMULA_HANDLE_DETAILS.No]) : 0;
                        string techName = row[My_DataBase.FORMULA_HANDLE_DETAILS.TechnologyName]?.ToString();
                        string asstCode = row[My_DataBase.FORMULA_HANDLE_DETAILS.AssistantCode]?.ToString();

                        var hisDyeRows = SqlServer.Select(
                            My_DataBase.HISTORY_DYE.TableName,
                            $"{My_DataBase.HISTORY_DYE.HeadID} = @headID AND {My_DataBase.HISTORY_DYE.No} = @no AND {My_DataBase.HISTORY_DYE.TechnologyName} = @techName AND {My_DataBase.HISTORY_DYE.AssistantCode} = @assistantCode",
                            new System.Data.SqlClient.SqlParameter("@headID", dyeInfo.ID),
                            new System.Data.SqlClient.SqlParameter("@no", no),
                            new System.Data.SqlClient.SqlParameter("@techName", techName),
                            new System.Data.SqlClient.SqlParameter("@assistantCode", asstCode)
                        );
                        if (hisDyeRows != null && hisDyeRows.Rows.Count > 0)
                        {
                            var val = hisDyeRows.Rows[0][My_DataBase.HISTORY_DYE.RealDropWeight];
                            if (val != DBNull.Value)
                                readDropWeight = Convert.ToDouble(val);
                        }
                        row[My_DataBase.FORMULA_HANDLE_DETAILS.RealDropWeight] =string.Format("{0:F" + this._retainDecimals + "}",  readDropWeight);
                    }

                    // 工艺名称重复显示处理
                    if (lastTn != null && lastTn == currentTn)
                    {
                        currentTn = null;
                    }
                    else
                    {
                        lastTn = row[My_DataBase.FORMULA_HANDLE_DETAILS.TechnologyName].ToString();
                    }

                    row[My_DataBase.FORMULA_HANDLE_DETAILS.RealDropWeight] = string.Format("{0:F" +this._retainDecimals + "}", row[My_DataBase.FORMULA_HANDLE_DETAILS.RealDropWeight]);
                    int rowIndex = this.dgv.Rows.Add(
                        currentTn, row[My_DataBase.FORMULA_HANDLE_DETAILS.AssistantCode], row[My_DataBase.FORMULA_HANDLE_DETAILS.AssistantName], row[My_DataBase.FORMULA_HANDLE_DETAILS.FormulaDosage],
                        row[My_DataBase.FORMULA_HANDLE_DETAILS.UnitOfAccount], null, row[My_DataBase.FORMULA_HANDLE_DETAILS.SettingConcentration], row[My_DataBase.FORMULA_HANDLE_DETAILS.RealConcentration],
                        row[My_DataBase.FORMULA_HANDLE_DETAILS.ObjectDropWeight], row[My_DataBase.FORMULA_HANDLE_DETAILS.RealDropWeight], row[My_DataBase.FORMULA_HANDLE_DETAILS.BottleSelection]
                    );
                    FillBottleComboBoxItems(this.dgv, rowIndex, row);
                }
            }
            else
            {
                for (int i = 0; i < dt.Rows.Count; i++)//&& i < this.dgv.Rows.Count
                {
                    DataRow row = dt.Rows[i];
                    // 保证 DataGridView 行数足够
                    if (i >= this.dgv.Rows.Count)
                    {
                        this.dgv.Rows.Add();
                    }
                    if (lastTn != null && lastTn == row[My_DataBase.FORMULA_HANDLE_DETAILS.TechnologyName].ToString())
                    {
                        this.dgv.Rows[i].Cells[IndexCol].Value = null;
                    }
                    else
                    {
                        this.dgv.Rows[i].Cells[IndexCol].Value = row[My_DataBase.FORMULA_HANDLE_DETAILS.TechnologyName];
                        lastTn = row[My_DataBase.FORMULA_HANDLE_DETAILS.TechnologyName].ToString();
                    }

                    int index = Convert.ToInt32(row[My_DataBase.FORMULA_HANDLE_DETAILS.No]);
                    string assistantCode = row[My_DataBase.FORMULA_HANDLE_DETAILS.AssistantCode]?.ToString();
                    double readDropWeight = 0;

                    DataTable batchDt = null;
                    if (this._dropHead.Source == 0)
                    {
                        // 配方表，实际滴液量默认为0
                    }
                    else
                    {
                        string indexNumN, technologyNameN, headIDN, assistantCodeN, realDropWeightN;
                        if (this._dropHead.Source == CtDropHead.DataSource.Batch)
                        {
                            batchDt = My_DataBase.DropBatchData.GetDyeData();
                            indexNumN = My_DataBase.DYE_DETAILS.IndexNum;
                            technologyNameN = My_DataBase.DYE_DETAILS.TechnologyName;
                            headIDN = My_DataBase.DYE_DETAILS.HeadID;
                            assistantCodeN = My_DataBase.DYE_DETAILS.AssistantCode;
                            realDropWeightN = My_DataBase.DYE_DETAILS.RealDropWeight;
                            var batchRows = batchDt?.Select($"{indexNumN} = {index} AND {technologyNameN} = '{row[My_DataBase.FORMULA_HANDLE_DETAILS.TechnologyName]}' AND {headIDN} = {dyeInfo.ID} AND {assistantCodeN} = '{assistantCode}'");
                            if (batchRows != null && batchRows.Length > 0)
                            {
                                foreach (var bRow in batchRows)
                                {
                                    if (bRow[realDropWeightN] != DBNull.Value)
                                    {
                                        readDropWeight += Convert.ToDouble(bRow[realDropWeightN]);
                                    }
                                }
                                row[My_DataBase.FORMULA_HANDLE_DETAILS.RealDropWeight] = string.Format("{0:F" + this._retainDecimals + "}", readDropWeight);
                            }
                        }
                        else
                        {
                            int no = row.Table.Columns.Contains(My_DataBase.FORMULA_HANDLE_DETAILS.No) && row[My_DataBase.FORMULA_HANDLE_DETAILS.No] != DBNull.Value
                            ? Convert.ToInt32(row[My_DataBase.FORMULA_HANDLE_DETAILS.No]) : 0;
                            string techName = row[My_DataBase.FORMULA_HANDLE_DETAILS.TechnologyName]?.ToString();
                            string asstCode = row[My_DataBase.FORMULA_HANDLE_DETAILS.AssistantCode]?.ToString();

                            var hisDyeRows = SqlServer.Select(
                                My_DataBase.HISTORY_DYE.TableName,
                                $"{My_DataBase.HISTORY_DYE.HeadID} = @headID AND {My_DataBase.HISTORY_DYE.No} = @no AND {My_DataBase.HISTORY_DYE.TechnologyName} = @techName AND {My_DataBase.HISTORY_DYE.AssistantCode} = @assistantCode",
                                new System.Data.SqlClient.SqlParameter("@headID", dyeInfo.ID),
                                new System.Data.SqlClient.SqlParameter("@no", no),
                                new System.Data.SqlClient.SqlParameter("@techName", techName),
                                new System.Data.SqlClient.SqlParameter("@assistantCode", asstCode)
                            );
                            if (hisDyeRows != null && hisDyeRows.Rows.Count > 0)
                            {
                                var val = hisDyeRows.Rows[0][My_DataBase.HISTORY_DYE.RealDropWeight];
                                if (val != DBNull.Value)
                                    readDropWeight = Convert.ToDouble(val);
                            }
                            row[My_DataBase.FORMULA_HANDLE_DETAILS.RealDropWeight] = string.Format("{0:F" + this._retainDecimals + "}", readDropWeight);
                        }

                    }

                    this.dgv.Rows[i].Cells[CodeCol].Value = row[My_DataBase.FORMULA_HANDLE_DETAILS.AssistantCode];
                    this.dgv.Rows[i].Cells[NameCol].Value = row[My_DataBase.FORMULA_HANDLE_DETAILS.AssistantName];
                    this.dgv.Rows[i].Cells[DosageCol].Value = row[My_DataBase.FORMULA_HANDLE_DETAILS.FormulaDosage];
                    this.dgv.Rows[i].Cells[UnitCol].Value = row[My_DataBase.FORMULA_HANDLE_DETAILS.UnitOfAccount];
                    this.dgv.Rows[i].Cells[SetConcCol].Value = row[My_DataBase.FORMULA_HANDLE_DETAILS.SettingConcentration];
                    this.dgv.Rows[i].Cells[RealConcCol].Value = row[My_DataBase.FORMULA_HANDLE_DETAILS.RealConcentration];
                    this.dgv.Rows[i].Cells[ObjDropCol].Value = row[My_DataBase.FORMULA_HANDLE_DETAILS.ObjectDropWeight];
                    this.dgv.Rows[i].Cells[RealDropCol].Value = row[My_DataBase.FORMULA_HANDLE_DETAILS.RealDropWeight];
                    this.dgv.Rows[i].Cells[ManualBottleCol].Value = row[My_DataBase.FORMULA_HANDLE_DETAILS.BottleSelection];
                    FillBottleComboBoxItems(this.dgv, i, row);
                }
            }
        }

        /// <summary>
        /// 填充瓶号下拉项（ComboBoxCell），并设置当前选中项和样式
        /// </summary>
        private void FillBottleComboBoxItems(DataGridView dgv, int rowIndex, DataRow row)
        {
            var bottleCell = this.dgv.Rows[rowIndex].Cells[BottleCol] as DataGridViewComboBoxCell;
            bottleCell.Items.Clear();

            var bottleDt = My_DataBase.BottleData.Bottle_details;
            var code = row[My_DataBase.FORMULA_HANDLE_DETAILS.AssistantCode]?.ToString()?.Trim();
            var bottleRows = bottleDt?.Select($"{My_DataBase.BOTTLE_DETAILS.AssistantCode} = '{code.Replace("'", "''")}'");
            var currentBottleNum = row[My_DataBase.FORMULA_HANDLE_DETAILS.BottleNum]?.ToString();

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
                bottleCell.Items.Add("无");
                bottleCell.Value = "无";
                this.dgv.Rows[rowIndex].Cells[BottleCol].Style.BackColor = Color.Red;
            }
        }

        /// <summary>
        /// 判断当前控件是否为空
        /// </summary>
        /// <returns>是否为空</returns>
        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Cbo_Type.Text);
        }

        /// <summary>
        /// 获取头部类型与工艺代码
        /// </summary>
        public (int, string) HeadValues()
        {
            int type = Cbo_Type.Text == "染色工艺" ? 1 : 2;
            string code = Cbo_DDPCode.Text;
            return (type, code);
        }

        /// <summary>
        /// 获取头部所有值（类型、工艺代码、浴比）
        /// </summary>
        public (int, string, double) AllHeadValues()
        {
            int type = Cbo_Type.Text == "染色工艺" ? 1 : 2;
            string code = Cbo_DDPCode.Text;
            double rad = Convert.ToDouble(Txt_DDRadio.Text);
            return (type, code, rad);
        }

        public void ReadOnly()
        {
            Cbo_Type.Enabled = false;
            Cbo_DDPCode.Enabled = false;
            Txt_DDRadio.ReadOnly = true;
            dgv.Enabled = false;
        }
    }
}