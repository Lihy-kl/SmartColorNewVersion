using SmartColor.My_DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Form.HistoricalData
{
    public partial class Rectification : Form
    {
        public Rectification()
        {
            InitializeComponent();
            this.btn_Record_Select.Click += Btn_Record_Select_Click;
            LoadRectificationTable();
        }

        private void LoadRectificationTable(string bottleNum = null, DateTime? start = null, DateTime? end = null)
        {
            // 1. 定义列顺序，瓶号放第二列
            var columns = new List<string>
            {
                CHECK_TABLE.Date,
                CHECK_TABLE.BottleNum, // 瓶号第二列
                CHECK_TABLE.RealConcentration,
                CHECK_TABLE.CurrentWeight,
                CHECK_TABLE.CurrentAdjustWeight,
                CHECK_TABLE.AdjustValue,
                CHECK_TABLE.RecheckWeight,
                CHECK_TABLE.Fail
            };

            // 2. 构建查询条件
            List<string> conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(bottleNum))
                conditions.Add($"{CHECK_TABLE.BottleNum} = '{bottleNum.Replace("'", "''")}'");
            if (start.HasValue)
                conditions.Add($"{CHECK_TABLE.Date} >= '{start.Value:yyyy-MM-dd HH:mm:ss}'");
            if (end.HasValue)
                conditions.Add($"{CHECK_TABLE.Date} <= '{end.Value:yyyy-MM-dd HH:mm:ss}'");
            string where = conditions.Count > 0 ? string.Join(" AND ", conditions) : null;

            // 3. 查询数据，按校正日期倒序
            DataTable dt = SqlServer.Select(
                CHECK_TABLE.TableName,
                columns,
                where,
                CHECK_TABLE.Date,
                ascending: false
            );

            // 4. 构造行数据
            var rowData = dt.AsEnumerable()
                .Select(r => columns.Select(c => r[c]).ToArray())
                .ToList();

            // 5. 设置表头映射
            var headerMap = new Dictionary<string, string>
            {
                { CHECK_TABLE.Date, "校正日期" },
                { CHECK_TABLE.BottleNum, "瓶号" },
                { CHECK_TABLE.RealConcentration, "实际浓度" },
                { CHECK_TABLE.CurrentWeight, "当前液量" },
                { CHECK_TABLE.CurrentAdjustWeight, "当前校正重量" },
                { CHECK_TABLE.AdjustValue, "校正值" },
                { CHECK_TABLE.RecheckWeight, "复检重量" },
                { CHECK_TABLE.Fail, "是否失败" }
            };

            // 6. 绑定到控件
            ctRecord1.SetColumnHeaders(headerMap);
            ctRecord1.SetDataSource(columns, rowData);

            // 7. 统计并显示成功率
            SetSuccessRate(dt);
        }

        /// <summary>
        /// 统计并显示校正成功率
        /// </summary>
        private void SetSuccessRate(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                txt_R.Text = "";
                return;
            }
            int total = dt.Rows.Count;
            int success = dt.AsEnumerable().Count(row => row[CHECK_TABLE.Fail]?.ToString() == "0");
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
            LoadRectificationTable(bottleNum, start, end);
        }
    }
}