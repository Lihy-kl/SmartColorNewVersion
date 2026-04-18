using SmartColor.My_Control;
using SmartColor.My_DataBase;
using SmartColor.My_File; // 日志组件
using SmartColor.My_Form.BasicData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Form.DyeingMan
{
    /// <summary>
    /// Preview 预览窗口，显示染色配方的头部信息、工艺曲线和明细数据。
    /// </summary>
    public partial class Preview : Form
    {
        private TableLayoutPanel _panel; // 明细控件容器
        private CtDropHead _dropHead;    // 头部信息控件

        /// <summary>
        /// 构造函数，初始化预览窗口并绑定备注数据变更事件。
        /// </summary>
        /// <param name="head">头部信息控件</param>
        /// <param name="panel">明细控件容器</param>
        public Preview(CtDropHead head, TableLayoutPanel panel)
        {
            InitializeComponent();
            this._dropHead = head;
            this._panel = panel;
            // 备注数据变更时自动刷新备注标签
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
            SqlServer.TableDataChanged += SqlServer_TableDataChanged;
            this.FormClosed += Preview_FormClosed;
            Logger.Info("Preview窗口初始化完成");
        }

        private void SqlServer_TableDataChanged(string obj)
        {
            if(obj== My_DataBase.ENABLED_SET.TableName)
                NoteChange();
        }

        private void Preview_FormClosed(object sender, FormClosedEventArgs e)
        {
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
        }

        private void Note_NoteDataChanged(object sender, EventArgs e)
        {
            NoteChange();
        }

        /// <summary>
        /// 窗口加载事件，填充备注、头部和明细数据。
        /// </summary>
        private void Preview_Load(object sender, EventArgs e)
        {
            try
            {
                NoteChange();
                FillHead();
                FillDetail();
                Logger.Info("Preview窗口加载并填充数据成功");
            }
            catch (Exception ex)
            {
                Logger.Error("Preview窗口加载异常", ex);
            }
        }

        /// <summary>
        /// 刷新备注标签内容，显示使能表中的备注字段。
        /// </summary>
        private void NoteChange()
        {
            try
            {
                var dt = My_DataBase.EnabledData.Enabled_set;
                var col = TableDefinition.TableSchemas[My_DataBase.ENABLED_SET.TableName]
                    .ToDictionary(f => f.Name, f => f.Name);
                if (dt == null) return;
                label27.Text = GetNoteLabel(dt, col, "Note1Name", "备注1", ":");
                label29.Text = GetNoteLabel(dt, col, "Note2Name", "备注2", ";");
                label31.Text = GetNoteLabel(dt, col, "Note3Name", "备注3", ":");
                Logger.Info("备注标签刷新完成");
            }
            catch (Exception ex)
            {
                Logger.Error("备注标签刷新异常", ex);
            }
        }

        /// <summary>
        /// 获取备注标签内容，优先显示数据表内容，否则显示默认文本。
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="col">字段名字典</param>
        /// <param name="colName">字段名</param>
        /// <param name="defaultText">默认文本</param>
        /// <param name="suffix">后缀</param>
        /// <returns>备注标签文本</returns>
        private string GetNoteLabel(DataTable dt, Dictionary<string, string> col, string colName, string defaultText, string suffix)
        {
            var note = dt.Rows[0][col[colName]];
            return (!(note is DBNull) ? note.ToString() : defaultText) + suffix;
        }

        /// <summary>
        /// 填充头部信息控件内容，包括配方、布重、客户等字段。
        /// </summary>
        private void FillHead()
        {
            try
            {
                var head = this._dropHead.GetAllInputValues();
                txt_FormulaCode.Text = head["txt_FormulaCode"]?.ToString();
                txt_VersionNum.Text = head["txt_VersionNum"]?.ToString();
                txt_FormulaName.Text = head["txt_FormulaName"]?.ToString();
                txt_ClothType.Text = head["txt_ClothType"]?.ToString();
                txt_Customer.Text = head["txt_Customer"]?.ToString();
                txt_CupNum.Text = head["txt_CupNum"]?.ToString();
                txt_ClothNum.Text = head["txt_ClothNum"]?.ToString();
                txt_ClothWeight.Text = head["txt_ClothWeight"]?.ToString();
                txt_BathRatio.Text = head["txt_BathRatio"]?.ToString();
                txt_TotalWeight.Text = head["txt_TotalWeight"]?.ToString();
                txt_AnhydrationWR.Text = head["txt_AnhydrationWR"]?.ToString();
                txt_Non_AnhydrationWR.Text = head["txt_Non_AnhydrationWR"]?.ToString();
                txt_DyeingCode.Text = head["txt_DyeingCode"]?.ToString();
                // 备注区支持下拉和文本框两种模式
                txt_Note1.Text = this._dropHead.cbo_Note1.Visible ? head["cbo_Note1"]?.ToString() : head["txt_Note1"]?.ToString();
                txt_Note2.Text = this._dropHead.cbo_Note1.Visible ? head["cbo_Note2"]?.ToString() : head["txt_Note2"]?.ToString();
                txt_Note3.Text = this._dropHead.cbo_Note1.Visible ? head["cbo_Note3"]?.ToString() : head["txt_Note3"]?.ToString();
                txt_CreateTime.Text = head["txt_CreateTime"]?.ToString();
             
                txt_AddWaterChoose.Checked = head["txt_AddWaterChoose"] != null && (bool)head["txt_AddWaterChoose"];
                // 若染色代码不为空，填充工艺曲线
                if (!string.IsNullOrEmpty(txt_DyeingCode.Text))
                {
                    FillChart();
                }
                Logger.Info("头部信息填充完成");
            }
            catch (Exception ex)
            {
                Logger.Error("头部信息填充异常", ex);
            }
        }

        /// <summary>
        /// 填充工艺曲线图表，根据染色代码查找工艺步骤并绘制温度曲线。
        /// </summary>
        private void FillChart()
        {
            try
            {
                string dyeingCode = txt_DyeingCode.Text?.Trim();
                if (string.IsNullOrEmpty(dyeingCode))
                {
                    ctTemperatureChart1.ClearChart();
                    textBox1.Clear();
                    Logger.Info("工艺曲线清空（无染色代码）");
                    return;
                }

                // 优先查找当前染色代码表，找不到则查历史表
                DataTable dyeingCodeTable = My_DataBase.DyeingCodeData.Dyeing_code;
                string dyeingCodeN = My_DataBase.DYEING_CODE.DyeingCode;
                string indexNumN = My_DataBase.DYEING_CODE.IndexNum;
                string codeN = My_DataBase.DYEING_CODE.Code;

                if (dyeingCodeTable == null ||
                    dyeingCodeTable.AsEnumerable().All(r => r.Field<string>(dyeingCodeN) != dyeingCode))
                {
                    dyeingCodeTable = My_DataBase.DyeingCodeData.History_Dyeing_code;
                    dyeingCodeN = My_DataBase.HISTORY_DYEING_CODE.DyeingCode;
                    indexNumN = My_DataBase.HISTORY_DYEING_CODE.IndexNum;
                    codeN = My_DataBase.HISTORY_DYEING_CODE.Code;
                }
                if (dyeingCodeTable == null)
                {
                    ctTemperatureChart1.ClearChart();
                    textBox1.Clear();
                    Logger.Info("工艺曲线清空（无染色代码表）");
                    return;
                }

                // 按染色代码和步骤序号排序
                var dyeingCodeRows = dyeingCodeTable.AsEnumerable()
                    .Where(r => r.Field<string>(dyeingCodeN) == dyeingCode)
                    .OrderBy(r => r.Field<int>(indexNumN))
                    .ToList();

                if (dyeingCodeRows.Count == 0)
                {
                    ctTemperatureChart1.ClearChart();
                    textBox1.Clear();
                    Logger.Info("工艺曲线清空（染色代码未找到）");
                    return;
                }

                // 查找工艺步骤表，优先当前表，找不到则查历史表
                DataTable processTable = DyeingData.Dyeing_process;
                string technologyNameN = My_DataBase.DYEING_PROCESS.TechnologyName;
                string tempN = My_DataBase.DYEING_PROCESS.Temp;
                string rateN = My_DataBase.DYEING_PROCESS.Rate;
                string proportionOrTimeN = My_DataBase.DYEING_PROCESS.ProportionOrTime;
                string codepN = My_DataBase.DYEING_PROCESS.Code;
                string stepNumN = My_DataBase.DYEING_PROCESS.StepNum;
                if (processTable == null)
                {
                    processTable = DyeingData.History_Dyeing_process;
                    technologyNameN = My_DataBase.HISTORY_DYEING_PROCESS.TechnologyName;
                    tempN = My_DataBase.HISTORY_DYEING_PROCESS.Temp;
                    rateN = My_DataBase.HISTORY_DYEING_PROCESS.Rate;
                    proportionOrTimeN = My_DataBase.HISTORY_DYEING_PROCESS.ProportionOrTime;
                    codepN = My_DataBase.HISTORY_DYEING_PROCESS.Code;
                    stepNumN = My_DataBase.HISTORY_DYEING_PROCESS.StepNum;
                }

                // 构造工艺步骤列表
                var processSteps = new List<ProcessStep>();
                foreach (var codeRow in dyeingCodeRows)
                {
                    string stepCode = codeRow[codeN]?.ToString();
                    if (string.IsNullOrEmpty(stepCode)) continue;

                    var stepRows = GetProcessStepRows(processTable, codepN, stepNumN, stepCode);
                    if (stepRows == null) continue;

                    foreach (var stepRow in stepRows)
                    {
                        var step = new ProcessStep
                        {
                            StepName = stepRow[technologyNameN]?.ToString() ?? ""
                        };
                        if (double.TryParse(stepRow[tempN]?.ToString(), out double temp))
                            step.TargetTemperature = temp;
                        if (double.TryParse(stepRow[rateN]?.ToString(), out double rate))
                            step.HeatingRate = rate;
                        if (double.TryParse(stepRow[proportionOrTimeN]?.ToString(), out double duration))
                            step.Duration = duration;
                        processSteps.Add(step);
                    }
                }

                // 绘制工艺曲线
                if (processSteps.Count > 0)
                {
                    ctTemperatureChart1.ShowProcessCurve(
                        processSteps.ToArray(),
                        My_ConPar.Delay.TemRecordInterval,
                        textBox2
                    );
                    Logger.Info($"工艺曲线填充完成，步骤数：{processSteps.Count}");
                }
                else
                {
                    ctTemperatureChart1.ClearChart();
                    textBox2.Clear();
                    Logger.Info("工艺曲线清空（无步骤数据）");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("工艺曲线填充异常", ex);
            }
        }

        /// <summary>
        /// 获取指定工艺步骤的所有行，优先当前表，找不到则查历史表。
        /// </summary>
        /// <param name="processTable">工艺步骤表</param>
        /// <param name="codeN">步骤代码字段名</param>
        /// <param name="stepNumN">步骤序号字段名</param>
        /// <param name="stepCode">步骤代码</param>
        /// <returns>步骤数据行列表</returns>
        private List<DataRow> GetProcessStepRows(DataTable processTable, string codeN, string stepNumN, string stepCode)
        {
            var stepRows = processTable?.AsEnumerable()
                .Where(r => r.Field<string>(codeN) == stepCode)
                .OrderBy(r => r.Field<int>(stepNumN))
                .ToList();

            if (stepRows == null || stepRows.Count == 0)
            {
                if (processTable != DyeingData.History_Dyeing_process && DyeingData.History_Dyeing_process != null)
                {
                    processTable = DyeingData.History_Dyeing_process;
                    stepRows = processTable.AsEnumerable()
                        .Where(r => r.Field<string>(My_DataBase.HISTORY_DYEING_PROCESS.Code) == stepCode)
                        .OrderBy(r => r.Field<int>(My_DataBase.HISTORY_DYEING_PROCESS.StepNum))
                        .ToList();
                }
            }
            return stepRows;
        }

        /// <summary>
        /// 按下回车键时关闭预览窗口。
        /// </summary>
        private void Preview_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Logger.Info("用户按下回车，关闭预览窗口");
                this.Close();
            }
        }

        /// <summary>
        /// 填充明细数据表，遍历所有明细控件，计算用量并显示。
        /// </summary>
        private void FillDetail()
        {
            try
            {
                int count = this._panel.Controls.Count;
                int order = 0; // 明细序号
                string wm = "自动"; // 默认加水方式
                string u = "克";   // 默认单位

                for (int i = 0; i < count; i++)
                {
                    var c = this._panel.Controls[i];

                    // 滴液明细控件
                    if (c is My_Control.CtFoldDataGridView f)
                    {
                        string type = "滴液";
                        if (f.Rows.Count == 0) continue;
                        order++;
                        foreach (DataGridViewRow dr in f.Rows)
                        {
                            var assistantCode = dr.Cells[1].Value;
                            if (assistantCode is DBNull || assistantCode == null) continue;
                            var assistantName = dr.Cells[2].Value;
                            var formulaDosage = dr.Cells[3].Value;
                            var unit = dr.Cells[4].Value;
                            string unitStr = unit?.ToString();
                            double weight = CalculateWeight(unitStr, txt_ClothWeight.Text, txt_TotalWeight.Text, formulaDosage);

                            ctDataGridView1.Rows.Add(
                                type, assistantCode, assistantName, formulaDosage.ToString() + unitStr, order, wm, weight, u, null, null
                            );
                            type = string.Empty; // 只在首行显示
                        }
                    }
                    // 染色/后处理明细控件
                    else if (c is My_Control.CtDyeing d)
                    {
                        if (d.IsEmpty()) continue;

                        (int t, string code, double rad) = d.AllHeadValues();

                        bool found = true;
                        var note = My_DataBase.DyeingData.Dyeing_process.Select(
                            $"{My_DataBase.DYEING_PROCESS.Code} ='{code}'");
                        if (note.Length == 0)
                        {
                            found = false;
                            note = My_DataBase.DyeingData.History_Dyeing_process.Select(
                                $"{My_DataBase.HISTORY_DYEING_PROCESS.Code} ='{code}'");
                        }

                        string type = t == 1 ? "染色" : "后处理";
                        string noteStr = string.Empty;
                        if (note.Length > 0)
                        {
                            noteStr = found
                                ? note[0][My_DataBase.DYEING_PROCESS.Remark]?.ToString() ?? string.Empty
                                : note[0][My_DataBase.HISTORY_DYEING_PROCESS.Remark]?.ToString() ?? string.Empty;
                        }
                        // 没有明细行时只显示类型和备注
                        if (d.dgv.Rows.Count == 0)
                        {
                            ctDataGridView1.Rows.Add(
                                type, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, code, noteStr
                            );
                        }
                        else
                        {
                            order++;
                            for (int rowIdx = 0; rowIdx < d.dgv.Rows.Count; rowIdx++)
                            {
                                var dr = d.dgv.Rows[rowIdx];
                                var assistantCode = dr.Cells[1].Value;
                                if (assistantCode is DBNull || assistantCode == null) continue;
                                var assistantName = dr.Cells[2].Value;
                                var formulaDosage = dr.Cells[3].Value;
                                var unit = dr.Cells[4].Value;
                                string unitStr = unit?.ToString();
                                double weight = CalculateWeight(unitStr, txt_ClothWeight.Text, txt_TotalWeight.Text, formulaDosage, rad);

                                ctDataGridView1.Rows.Add(
                                    type, assistantCode, assistantName, formulaDosage.ToString() + unitStr, order, wm, weight, u, code, noteStr
                                );
                                type = string.Empty; // 只在首行显示
                                code = string.Empty;
                                noteStr = string.Empty;

                                // 判断是否需要增加序号（步骤间有空隙时）
                                if (rowIdx < d.dgv.Rows.Count - 1)
                                {
                                    string curName = dr.Cells[0].Value?.ToString();
                                    if (string.IsNullOrEmpty(curName))
                                    {
                                        int offset = 1;
                                        while (rowIdx - offset >= 0)
                                        {
                                            curName = d.dgv.Rows[rowIdx - offset].Cells[0].Value?.ToString();
                                            if (!string.IsNullOrEmpty(curName))
                                                break;
                                            offset++;
                                        }
                                    }

                                    var table = note.CopyToDataTable();
                                    DataRow[] rows = found
                                        ? table.Select($"{My_DataBase.DYEING_PROCESS.TechnologyName} = '{curName}'")
                                        : table.Select($"{My_DataBase.HISTORY_DYEING_PROCESS.TechnologyName} = '{curName}'");

                                    int curindex = 0;
                                    if (rows != null && rows.Length > 0)
                                    {
                                        curindex = found
                                            ? Convert.ToInt32(rows[0][My_DataBase.DYEING_PROCESS.StepNum])
                                            : Convert.ToInt32(rows[0][My_DataBase.HISTORY_DYEING_PROCESS.StepNum]);
                                    }

                                    string nextName = d.dgv.Rows[rowIdx + 1].Cells[0].Value?.ToString();
                                    if (!string.IsNullOrEmpty(nextName))
                                    {
                                        DataRow[] rows1 = found
                                            ? table.Select($"{My_DataBase.DYEING_PROCESS.TechnologyName} = '{nextName}'")
                                            : table.Select($"{My_DataBase.HISTORY_DYEING_PROCESS.TechnologyName} = '{nextName}'");
                                        int nextindex = 0;
                                        if (rows1 != null && rows1.Length > 0)
                                        {
                                            nextindex = found
                                                ? Convert.ToInt32(rows1[0][My_DataBase.DYEING_PROCESS.StepNum])
                                                : Convert.ToInt32(rows1[0][My_DataBase.HISTORY_DYEING_PROCESS.StepNum]);
                                        }

                                        if (nextindex - curindex > 1)
                                        {
                                            order++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                ctDataGridView1.ClearSelection();
                ctDataGridView1.AutoResizeColumns();
                Logger.Info($"明细数据填充完成，行数：{ctDataGridView1.Rows.Count}");
            }
            catch (Exception ex)
            {
                Logger.Error("明细数据填充异常", ex);
            }
        }

        /// <summary>
        /// 计算用量重量，支持百分比、克/升等单位。
        /// </summary>
        /// <param name="unitStr">单位字符串</param>
        /// <param name="clothWeight">布重</param>
        /// <param name="totalWeight">总浴量</param>
        /// <param name="formulaDosage">配方用量</param>
        /// <param name="rad">倍率（默认1）</param>
        /// <returns>计算后的重量</returns>
        private double CalculateWeight(string unitStr, string clothWeight, string totalWeight, object formulaDosage, double rad = 1)
        {
          
            double dosage = Convert.ToDouble(formulaDosage);
            if (unitStr == "%")
                return Math.Round(Convert.ToDouble(clothWeight) * dosage / 100.00, 6);
            if (unitStr == "g/l" || unitStr == "G/L")
                return Math.Round(Convert.ToDouble(totalWeight) * dosage / 1000.00 * rad, 6);
            return Math.Round(dosage, 6);
        }
    }
}