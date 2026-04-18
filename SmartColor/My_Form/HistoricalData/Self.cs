using SmartColor.My_DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Form.HistoricalData
{
    public partial class Self : Form
    {
        public Self()
        {
            InitializeComponent();
            btn_Record_Select.Click += Btn_Record_Select_Click;
            LoadSelfTable();
        }

        private void LoadSelfTable(string bottleNum = null, DateTime? start = null, DateTime? end = null)
        {
            // 1. 定义列顺序，瓶号放第二列
            var columns = new List<string>
            {
                SELF_TABLE.Date,
                SELF_TABLE.BottleNum, // 瓶号第二列
                SELF_TABLE.SelfChecking1,
                SELF_TABLE.SelfChecking2,
                SELF_TABLE.SelfChecking3,
                SELF_TABLE.SelfChecking4,
                SELF_TABLE.CurrentAdjustWeight,
                SELF_TABLE.AdjustValue,
                SELF_TABLE.Fail
            };

            // 2. 构建查询条件
            List<string> conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(bottleNum))
                conditions.Add($"{SELF_TABLE.BottleNum} = '{bottleNum.Replace("'", "''")}'");
            if (start.HasValue)
                conditions.Add($"{SELF_TABLE.Date} >= '{start.Value:yyyy-MM-dd HH:mm:ss}'");
            if (end.HasValue)
                conditions.Add($"{SELF_TABLE.Date} <= '{end.Value:yyyy-MM-dd HH:mm:ss}'");
            string where = conditions.Count > 0 ? string.Join(" AND ", conditions) : null;

            // 3. 查询数据，按自检日期倒序
            DataTable dt = SqlServer.Select(
                SELF_TABLE.TableName,
                columns,
                where,
                SELF_TABLE.Date,
                ascending: false
            );

            // 4. 构造行数据
            var rowData = dt.AsEnumerable()
                .Select(r => columns.Select(c => r[c]).ToArray())
                .ToList();

            // 5. 设置表头映射
            var headerMap = new Dictionary<string, string>
            {
                { SELF_TABLE.Date, "自检日期" },
                { SELF_TABLE.BottleNum, "瓶号" },
                { SELF_TABLE.SelfChecking1, "自检1" },
                { SELF_TABLE.SelfChecking2, "自检2" },
                { SELF_TABLE.SelfChecking3, "自检3" },
                { SELF_TABLE.SelfChecking4, "自检4" },
                { SELF_TABLE.CurrentAdjustWeight, "校正重量" },
                { SELF_TABLE.AdjustValue, "校正值" },
                { SELF_TABLE.Fail, "是否失败" }
            };

            // 6. 绑定到控件
            ctRecord1.SetColumnHeaders(headerMap);
            ctRecord1.SetDataSource(columns, rowData);

            // 7. 统计并显示成功率
            SetSuccessRate(dt);
        }

        /// <summary>
        /// 统计并显示自检成功率
        /// </summary>
        private void SetSuccessRate(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                txt_R.Text = "";
                return;
            }
            int total = dt.Rows.Count;
            int success = dt.AsEnumerable().Count(row => row[SELF_TABLE.Fail]?.ToString() == "0");
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
            LoadSelfTable(bottleNum, start, end);
        }
    }
}