using SmartColor.My_DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Form.HistoricalData
{
    public partial class Brew : Form
    {
        public Brew()
        {
            InitializeComponent();
           this. btn_Record_Select.Click += Btn_Record_Select_Click;
            LoadBrewRunTable();
        }

        /// <summary>
        /// 加载 brew_run_table 数据并倒序显示，并计算成功率
        /// </summary>
        private void LoadBrewRunTable(string bottleNum = null, DateTime? start = null, DateTime? end = null)
        {
            // 构建查询条件
            List<string> conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(bottleNum))
                conditions.Add($"{BREW_RUN_TABLE.BottleNum} = '{bottleNum.Replace("'", "''")}'");
            if (start.HasValue)
                conditions.Add($"{BREW_RUN_TABLE.StartDateTime} >= '{start.Value:yyyy-MM-dd HH:mm:ss}'");
            if (end.HasValue)
                conditions.Add($"{BREW_RUN_TABLE.FinishDateTime} <= '{end.Value:yyyy-MM-dd HH:mm:ss}'");

            string where = conditions.Count > 0 ? string.Join(" AND ", conditions) : null;

            DataTable dt = My_DataBase.SqlServer.Select(
                BREW_RUN_TABLE.TableName,
                new[]
                {
                    BREW_RUN_TABLE.MyID,
                    BREW_RUN_TABLE.StartDateTime,
                    BREW_RUN_TABLE.BottleNum,
                    BREW_RUN_TABLE.BrewCode,
                    BREW_RUN_TABLE.SettingConcentration,
                    BREW_RUN_TABLE.AllowMaxWeight,
                    BREW_RUN_TABLE.OriginalBottleNum,
                    BREW_RUN_TABLE.OriginalConcentration,
                    BREW_RUN_TABLE.InputMode,
                    BREW_RUN_TABLE.AssistantName,
                    BREW_RUN_TABLE.FinishDateTime,
                    BREW_RUN_TABLE.RealConcentration,
                    BREW_RUN_TABLE.CurrentWeight,
                    BREW_RUN_TABLE.UseingTime,
                    BREW_RUN_TABLE.ReasonCessation
                },
                where,
                BREW_RUN_TABLE.FinishDateTime,
                ascending: false
            );

            // 设置中英文表头映射
            var headerMap = new Dictionary<string, string>
            {
                { BREW_RUN_TABLE.MyID, "序号" },
                { BREW_RUN_TABLE.StartDateTime, "开始时间" },
                { BREW_RUN_TABLE.BottleNum, "瓶号" },
                { BREW_RUN_TABLE.BrewCode, "调液流程代码" },
                { BREW_RUN_TABLE.SettingConcentration, "设定浓度" },
                { BREW_RUN_TABLE.AllowMaxWeight, "允许最大调液量" },
                { BREW_RUN_TABLE.OriginalBottleNum, "开稀原瓶号" },
                { BREW_RUN_TABLE.OriginalConcentration, "开稀原浓度" },
                { BREW_RUN_TABLE.InputMode, "输入模式" },
                { BREW_RUN_TABLE.AssistantName, "染助剂名称" },
                { BREW_RUN_TABLE.FinishDateTime, "完成时间" },
                { BREW_RUN_TABLE.RealConcentration, "实际浓度" },
                { BREW_RUN_TABLE.CurrentWeight, "实际液量" },
                { BREW_RUN_TABLE.UseingTime, "总用时" },
                { BREW_RUN_TABLE.ReasonCessation, "停止原因" }
            };
            ctRecord1.SetColumnHeaders(headerMap);

            // 绑定数据
            ctRecord1.SetDataSource(dt);

            // 计算成功率
            SetSuccessRate(dt);
        }

        /// <summary>
        /// 计算并显示成功率
        /// </summary>
        private void SetSuccessRate(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                txt_R.Text = "";
                return;
            }
            int total = dt.Rows.Count;
            int success = dt.AsEnumerable().Count(row => row[BREW_RUN_TABLE.ReasonCessation]?.ToString() != "1");
            int fail = total - success;
            string rate = total > 0 ? $"{(success * 100.0 / total):F1}%" : "0%";
            txt_R.Text = $"成功率：{rate}（成功{success}，失败{fail}，总{total}）";
        }

        /// <summary>
        /// 查询按钮点击事件
        /// </summary>
        private void Btn_Record_Select_Click(object sender, EventArgs e)
        {
            string bottleNum = textBox1.Text.Trim();
            DateTime? start = dt_Record_Start.Enabled ? (DateTime?)dt_Record_Start.Value : null;
            DateTime? end = dt_Record_End.Enabled ? (DateTime?)dt_Record_End.Value : null;
            LoadBrewRunTable(bottleNum, start, end);
        }
    }
}