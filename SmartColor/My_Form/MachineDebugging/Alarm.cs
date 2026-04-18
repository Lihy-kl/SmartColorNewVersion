using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SmartColor.My_File;
using SmartColor.My_Tool;

namespace SmartColor.My_Form.MachineDebugging
{
    public partial class Alarm : Form
    {
        private DataTable _alarmTable;

        public Alarm()
        {
            InitializeComponent();
            AlarmTableMan.AlarmTableChanged += AlarmTableMan_AlarmTableChanged; ;
            ctRecord1.HomeButtonClicked += CtRecord1_HomeButtonClicked;
            ctRecord1.dgv.CellClick += Dgv_CellClick;
        }

        private void CtRecord1_HomeButtonClicked(object sender, EventArgs e)
        {
            RefreshAlarmTable();
        }

        private void AlarmTableMan_AlarmTableChanged(object sender, AlarmTableMan.AlarmTableChangedEventArgs e)
        {
            RefreshAlarmTable();
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            try
            {
                var currentPageTable = (ctRecord1.dgv.DataSource as BindingSource)?.DataSource as DataTable;
                if (currentPageTable == null || _alarmTable == null) return;
                if (e.RowIndex >= currentPageTable.Rows.Count) return;

                string detailsCol =My_DataBase.ALARM_TABLE. AlarmDetails;
                string alarmDetail = currentPageTable.Rows[e.RowIndex][detailsCol].ToString();

                // 模糊统计（包含关键字）
                int count = _alarmTable.AsEnumerable()
                    .Count(row => row[detailsCol]?.ToString().IndexOf(alarmDetail, StringComparison.OrdinalIgnoreCase) >= 0);

                Logger.Info($"Alarm: 用户模糊统计报警详情“{alarmDetail}”出现次数 {count}。");

                My_File.LocalTranslator.ShowMessage(
                    $"包含“{alarmDetail}”的报警详情在表中共出现 {count} 次。",
                    "模糊统计",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                Logger.Error("Alarm: 模糊统计报警详情异常", ex);
            }
        }

        private void Alarm_Load(object sender, EventArgs e)
        {
            RefreshAlarmTable();
        }

        private void RefreshAlarmTable()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RefreshAlarmTable));
                return;
            }
            try
            {
                if (ctRecord1.HasPageSwitched) return; // 用户切换过页面则不刷新，避免影响用户操作
                Logger.Info("Alarm: 开始加载报警表。");
                _alarmTable = My_DataBase.SqlServer.Select(
                    My_DataBase.ALARM_TABLE.TableName,
                    new[] {
                        My_DataBase.ALARM_TABLE.MyID,
                        My_DataBase.ALARM_TABLE.MyDate,
                        My_DataBase.ALARM_TABLE.MyTime,
                        My_DataBase.ALARM_TABLE.AlarmHead,
                        My_DataBase.ALARM_TABLE.AlarmDetails
                    },
                    null,
                    orderBy: My_DataBase.ALARM_TABLE.MyID,
                    ascending: false
                );
                ctRecord1.SetDataSource(_alarmTable);
                ctRecord1.SetColumnHeaders(new Dictionary<string, string>
                {
                    { My_DataBase.ALARM_TABLE.MyID, "编号" },
                    { My_DataBase.ALARM_TABLE.MyDate, "日期" },
                    { My_DataBase.ALARM_TABLE.MyTime, "时间" },
                    { My_DataBase.ALARM_TABLE.AlarmHead, "报警标题" },
                    { My_DataBase.ALARM_TABLE.AlarmDetails, "报警详情" }
                });
                Logger.Info($"Alarm: 加载完成，共 {(_alarmTable?.Rows.Count ?? 0)} 条数据。");
            }
            catch (Exception ex)
            {
                Logger.Error("Alarm: 加载报警表异常", ex);
            }
        }

        private void Alarm_FormClosed(object sender, FormClosedEventArgs e)
        {
            AlarmTableMan.AlarmTableChanged -= AlarmTableMan_AlarmTableChanged; ;
            ctRecord1.HomeButtonClicked -= CtRecord1_HomeButtonClicked;
            ctRecord1.dgv.CellClick -= Dgv_CellClick;
        }
    }
}