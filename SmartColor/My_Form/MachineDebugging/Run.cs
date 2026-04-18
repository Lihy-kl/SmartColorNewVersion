using com.google.zxing;
using SmartColor.My_File;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Form.MachineDebugging
{
    public partial class Run : Form
    {
        // 运行表数据缓存
        private DataTable _runTable;

        public Run()
        {
            InitializeComponent();

            // 订阅运行表变更事件，数据变更时刷新表格
            RunTableMan.RunTableChanged += RunTableMan_RunTableChanged;

            // 订阅首页按钮点击事件，点击时刷新表格
            ctRecord1.HomeButtonClicked += CtRecord1_HomeButtonClicked; ;
        }

        private void CtRecord1_HomeButtonClicked(object sender, EventArgs e)
        {
            RefreshRunTable();
        }

        private void RunTableMan_RunTableChanged(object sender, RunTableMan.RunTableChangedEventArgs e)
        {
            RefreshRunTable();
        }

        // 窗体加载事件，初始化数据
        private void Run_Load(object sender, EventArgs e)
        {
            RefreshRunTable();
        }

        /// <summary>
        /// 刷新运行表数据并更新界面显示
        /// </summary>
        private void RefreshRunTable()
        {
            if (ctRecord1.InvokeRequired)
            {
                ctRecord1.Invoke(new Action(RefreshRunTable));
                return;
            }
            try
            {
                // 如果用户切换过页面则不刷新，避免影响用户操作
                if (ctRecord1.HasPageSwitched) return;

                // 查询数据库，获取运行表数据
                _runTable = My_DataBase.SqlServer.Select(
                    My_DataBase.RUN_TABLE.TableName,
                    new[] {
                        My_DataBase.RUN_TABLE.MyID,
                        My_DataBase.RUN_TABLE.MyDate,
                        My_DataBase.RUN_TABLE.MyTime,
                        My_DataBase.RUN_TABLE.Machine,
                        My_DataBase.RUN_TABLE.RobotHand,
                        My_DataBase.RUN_TABLE.Dail
                    },
                    orderBy: My_DataBase.RUN_TABLE.MyID,
                    ascending: false // 按编号倒序排列
                );

                // 设置数据源到控件
                ctRecord1.SetDataSource(_runTable);

                // 设置表头显示名称
                ctRecord1.SetColumnHeaders(new Dictionary<string, string>
                {
                    { My_DataBase.RUN_TABLE.MyID, "编号" },
                    { My_DataBase.RUN_TABLE.MyDate, "日期" },
                    { My_DataBase.RUN_TABLE.MyTime, "时间" },
                    { My_DataBase.RUN_TABLE.Machine, "系统" },
                    { My_DataBase.RUN_TABLE.RobotHand, "机械手" },
                    { My_DataBase.RUN_TABLE.Dail, "染色机" }
                });

                // 日志记录加载完成及数据条数
                Logger.Info($"Run: 加载完成，共 {(_runTable?.Rows.Count ?? 0)} 条数据。");
            }
            catch (Exception ex)
            {
                // 异常日志记录
                Logger.Error("Run: 加载运行表异常", ex);
            }
        }

        private void Run_FormClosed(object sender, FormClosedEventArgs e)
        {
            RunTableMan.RunTableChanged -= RunTableMan_RunTableChanged;
            ctRecord1.HomeButtonClicked -= CtRecord1_HomeButtonClicked; ;
        }
    }
}