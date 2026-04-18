using SmartColor.My_Control;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Form.Homepage
{
    /// <summary>
    /// 主界面窗体，负责布局加载、控件注册与数据刷新
    /// </summary>
    public partial class Home : Form
    {

        // 所有需要批量刷新的自定义控件集合
        private readonly List<My_Interface.ICustomUpdatable> _customControls = new List<My_Interface.ICustomUpdatable>();

        // 天平控件引用
        private CtBalance _ctBalance;




        public Home()
        {
            InitializeComponent();
            UpdateLayout();

            // 事件注册
            SqlServer.TableDataChanged -= SqlServer_TableDataChanged;
            SqlServer.TableDataChanged += SqlServer_TableDataChanged;


        }

        private void SqlServer_TableDataChanged(string obj)
        {
            if (obj == My_DataBase.ASSISTANT_DETAILS.TableName)
            {
                UpdateAllCustomControls(false);
            }
            else if (obj == My_DataBase.BOTTLE_DETAILS.TableName)
            {
                UpdateAllCustomControls(false);
            }
            else if (obj == My_DataBase.CUP_DETAILS.TableName)
            {
                UpdateAllCustomControls();
            }

        }



        /// <summary>
        /// 更新布局，根据配置重新创建布局控件并注册自定义控件
        /// </summary>
        public void UpdateLayout()
        {
            this.Controls.Clear();
            _customControls.Clear();
            UserControl layout = CreateLayoutByConfig();
            this.Controls.Add(layout);
            RegisterAllCustomAreas(layout);
            UpdateAllCustomControls();
            _ctBalance = FindCtBalance(this);
            // 1. 构建区域坐标缓存（极大提升后续查找效率）
            var layoutType = My_ConPar.Object.CurrentLayout as Type;
            if (layoutType != null)
                SmartColor.My_Tool.AreaCoordinateFinder.BuildAllCoordinateCache(layoutType);

            // 新增：查找CtWash控件并决定是否启动Timer4
            var ctWash = FindCtWash(this);
            if (ctWash != null)
            {
                if (timer4 != null)
                    timer4.Enabled = true;
            }
            else
            {
                if (timer4 != null)
                    timer4.Enabled = false;
            }

        }

        // 递归查找CtWash控件
        private CtWash FindCtWash(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is CtWash wash)
                    return wash;
                var found = FindCtWash(c);
                if (found != null)
                    return found;
            }
            return null;
        }

        /// <summary>
        /// 根据配置创建布局控件
        /// </summary>
        private UserControl CreateLayoutByConfig()
        {
            switch (My_ConPar.Machine.MachineLayout)
            {
                case 0:
                    return new My_Control.Ctlayout_1() { Dock = DockStyle.Fill };
                case 1:
                    return new My_Control.Ctlayout_2() { Dock = DockStyle.Fill };
                case 2:
                    return new My_Control.Ctlayout_3() { Dock = DockStyle.Fill };
                case 3:
                    return new My_Control.Ctlayout_4() { Dock = DockStyle.Fill };

                default:
                    LocalTranslator.ShowMessage("未知布局，请修改布局参数", "布局参数", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                    return null;
            }
        }

        /// <summary>
        /// 递归注册所有 CtBottleArea 和 CtCupArea 区域的自定义控件注册方法
        /// </summary>
        private void RegisterAllCustomAreas(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is CtBottleArea bottleArea)
                    bottleArea.RegisterCustomControl = RegisterCustomControl;
                if (ctrl is CtCupArea cupArea)
                    cupArea.RegisterCustomControl = RegisterCustomControl;
                if (ctrl.HasChildren)
                    RegisterAllCustomAreas(ctrl);
            }
        }

        /// <summary>
        /// 注册自定义控件到刷新列表，避免重复注册
        /// </summary>
        public void RegisterCustomControl(My_Interface.ICustomUpdatable ctrl)
        {
            if (!_customControls.Contains(ctrl))
                _customControls.Add(ctrl);
        }



        /// <summary>
        /// 批量刷新所有自定义控件
        /// </summary>
        public void UpdateAllCustomControls(bool both = true)
        {
            var bottleDt = My_DataBase.BottleData.Bottle_details;
            var cupDt = My_DataBase.CupData.GetData();
            if (bottleDt == null && cupDt == null) return;

            foreach (var ctrl in _customControls)
            {
                if (ctrl is CtBottle)
                    UpdateCustomControl<CtBottle>(ctrl.ControlKey);
                else if (ctrl is CtCup && both)
                    UpdateCustomControl<CtCup>(ctrl.ControlKey);
                // 可扩展其他类型
            }
        }

        /// <summary>
        /// 刷新指定类型和编号的控件，避免同编号不同类型冲突
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="key">控件唯一标识</param>
        /// <param name="row">可选数据行</param>
        public void UpdateCustomControl<T>(string key, DataRow row = null) where T : My_Interface.ICustomUpdatable
        {
            var ctrl = _customControls.FirstOrDefault(c => c is T && c.ControlKey == key);
            if (ctrl == null) return;

            DataRow data = row;
            if (ctrl is CtBottle)
            {
                var dt = My_DataBase.BottleData.Bottle_details;
                if (data == null && dt != null)
                {

                    var rows = dt.Select($"BottleNum = '{key}'");
                    if (rows.Length > 0)
                    {
                        data = rows[0];
                    }


                }
            }
            else if (ctrl is CtCup)
            {
                var dt = SqlServer.Select(CUP_DETAILS.TableName, $"CupNum = '{key}'");
                if (data == null && dt != null && dt.Rows.Count > 0)
                {
                        data = dt.Rows[0];
                }
            }
            // 可扩展其他类型

            ctrl.UpdateFromData(data);
        }

        /// <summary>
        /// 定时器事件，定期刷新控件和天平数据
        /// </summary>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateAllCustomControls(false);

        }

        /// <summary>
        /// 递归查找天平控件 CtBalance
        /// </summary>
        private CtBalance FindCtBalance(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is CtBalance cb)
                    return cb;
                var found = FindCtBalance(c);
                if (found != null)
                    return found;
            }
            return null;
        }

        /// <summary>
        /// 刷新天平读数并处理异常
        /// </summary>
        private void UpdateBalanceValue()
        {
            if (_ctBalance == null) return;
            double value = My_Tool.BalanceStableReading.CurrentRead;
            if (!My_Tool.BalanceStableReading.BalanceError)
            {
                switch (value)
                {
                    case 6666:
                        My_Tool.BalanceStableReading.BalanceError = true;
                        MessageEventManager.Instance.RequestShowMessage("天平报警", "通讯失败", btn =>
                        {
                            if (btn == "确定")
                                My_Tool.BalanceStableReading.BalanceError = false;
                        }, new[] { "确定" }, "确定");
                        break;
                    case 7777:
                        My_Tool.BalanceStableReading.BalanceError = true;
                        MessageEventManager.Instance.RequestShowMessage("天平报警", "检测到废液桶未移开。请先将其移走，待天平归零后放回，再点击“确定”", btn =>
                        {
                            if (btn == "确定")
                                My_Tool.BalanceStableReading.BalanceError = false;
                        }, new[] { "确定" }, "确定");
                        break;
                    case 8888:
                        My_Tool.BalanceStableReading.BalanceError = true;
                        MessageEventManager.Instance.RequestShowMessage("天平报警", "超下限", btn =>
                        {
                            if (btn == "确定")
                                My_Tool.BalanceStableReading.BalanceError = false;
                        }, new[] { "确定" }, "确定");
                        break;
                    case 9999:
                        My_Tool.BalanceStableReading.BalanceError = true;
                        MessageEventManager.Instance.RequestShowMessage("天平报警", "超上限", btn =>
                        {
                            if (btn == "确定")
                                My_Tool.BalanceStableReading.BalanceError = false;
                        }, new[] { "确定" }, "确定");
                        break;
                    default:
                        My_Tool.BalanceStableReading.BalanceError = false;
                        // 正常显示天平读数
                        _ctBalance.Title = value.ToString();



                        // 超重变色
                        if (value >= My_ConPar.Other.BalanceMaxWeight - 100)
                            _ctBalance.LiquidColor = System.Drawing.Color.Red;
                        else
                            _ctBalance.LiquidColor = System.Drawing.Color.DeepSkyBlue;
                        break;
                }
            }


        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            UpdateBalanceValue();
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            if (My_ConPar.Choices.UseAutoCheck != 1)
                return;

            var dt = My_DataBase.BottleData.Bottle_details;
            if (dt == null) return;
            // 检查任务队列是否为空
            var allTasks = SmartColor.My_RobotManager.RobotTaskManager.Instance.GetAllTasksSnapshot();
            if (allTasks == null || allTasks.Count == 0)
            {
                // 检查所有瓶子
                foreach (DataRow row in dt.Rows)
                {
                    var key = row[My_DataBase.BOTTLE_DETAILS.BottleNum].ToString();
                    var adjustSuccess = Convert.ToInt32(row[My_DataBase.BOTTLE_DETAILS.AdjustSuccess]);
                    var currentWeight = Convert.ToDouble(row[My_DataBase.BOTTLE_DETAILS.CurrentWeight]);
                    var datetime = Convert.ToDateTime(row[My_DataBase.BOTTLE_DETAILS.BrewingData]);
                    bool readly = (DateTime.Now - datetime).TotalMinutes > 5;
                    if (adjustSuccess != 1 && currentWeight >= My_ConPar.Other.Bottle_MinWeight && readly)
                    {
                        // 启动自动校正
                        _ = SmartColor.My_AutomaticModule.BottleCorrectionRobotTask.EnqueueBottleCorrectionAsync(Convert.ToInt32(key));
                    }
                }
            }
        }

        private void Timer4_Tick(object sender, EventArgs e)
        {
            if (My_ConPar.Choices.UseAutoWashSyringe != 1)
                return;
            var dt = My_DataBase.BottleData.Bottle_details;
            if (dt == null) return;

            foreach (DataRow row in dt.Rows)
            {
                // 获取瓶号
                if (!int.TryParse(row[My_DataBase.BOTTLE_DETAILS.BottleNum]?.ToString(), out int bottleNo))
                    continue;

                // 获取洗针间隔（小时）
                int washSyringeSpan = 0;
                if (row.Table.Columns.Contains(My_DataBase.BOTTLE_DETAILS.WashSyringeSpan) &&
                    int.TryParse(row[My_DataBase.BOTTLE_DETAILS.WashSyringeSpan]?.ToString(), out int span))
                {
                    washSyringeSpan = span;
                }

                // 获取上次洗针时间
                DateTime? lastWashTime = null;
                if (row.Table.Columns.Contains(My_DataBase.BOTTLE_DETAILS.LastWashTime) &&
                    DateTime.TryParse(row[My_DataBase.BOTTLE_DETAILS.LastWashTime]?.ToString(), out DateTime lastTime))
                {
                    lastWashTime = lastTime;
                }

                // 如果没有上次洗针时间，跳过
                if (washSyringeSpan <= 0)
                    continue;

                // 判断是否需要洗针
                if (!lastWashTime.HasValue || lastWashTime.Value.AddHours(washSyringeSpan) <= DateTime.Now)
                {
                    // 调用洗针任务（异步，不等待结果）
                    _ = SmartColor.My_AutomaticModule.WashSyringeRobotTask.EnqueueWashSyringeAsync(bottleNo);
                }
            }
        }
    }
}