using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using SmartColor.My_ADT8940A1;

namespace SmartColor.My_Form.Main
{
    public partial class MainForm : Form
    {

        private Dictionary<string, FlowLayoutPanel> _loopSpeakRows = new Dictionary<string, FlowLayoutPanel>();

        /// <summary>
        /// Choices属性与菜单项的映射
        /// </summary>
        private readonly Dictionary<ToolStripMenuItem, (string propName, string iniKey)> _choicesMenuMap;

        private SmartColor.My_Form.Homepage.Home _home = null;
        private bool _layoutChanged = false;

        public MainForm()
        {
            InitializeComponent();
            RefreshAreaMenu();
            // 初始化映射
            _choicesMenuMap = new Dictionary<ToolStripMenuItem, (string, string)>
            {
                { MIRepeatGetCloth, ("RepeatGetCloth", "RepeatGetCloth") },
                { MIDripCheckLow, ("DripCheckLow", "DripCheckLow") },
                { MIDripCheckExpired, ("DripCheckExpired", "DripCheckExpired") },
                { MIDripAllowLow, ("DripAllowLow", "DripAllowLow") },
                { MIDripAllowExpired, ("DripAllowExpired", "DripAllowExpired") },
                { MIDripFull, ("DripFull", "DripFull") },
                { MICutNeedCheck, ("CutNeedCheck", "CutNeedCheck") },
                { MIUseMotherDate, ("UseMotherDate", "UseMotherDate") },
                { MIIsDebug, ("IsDebug", "IsDebug") },
                { MIUseAutoChoose, ("UseAutoChoose", "UseAutoChoose") },
                { MIUseClamp, ("UseClamp", "UseClamp") },
                { MIUseClampOut, ("UseClampOut", "UseClampOut") },
                { MIUseAutoDrip, ("UseAutoDrip", "UseAutoDrip") },
                { MIAutoAbs, ("AutoAbs", "AutoAbs") },
                { MIWaterRecheck, ("WaterRecheck", "WaterRecheck") },

                { MIBathRatioTxtDyBath, ("BathRatioTxtDyBath", "BathRatioTxtDyBath") },
                { MIHighWash, ("HighWash", "HighWash") },
                { MIIgnoreSyringeSensor,("IgnoreSyringeSensor","IgnoreSyringeSensor" ) },
                { MiUseAutoCheck,("UseAutoCheck", "UseAutoCheck") },
                { MiUseAutoWashSyringe,("UseAutoWashSyringe","UseAutoWashSyringe") },
                { MiUseAutoUpdateCupCoor,("UseAutoUpdateCupCoor","UseAutoUpdateCupCoor") },
                { MiUseLimit,("UseLimit","UseLimit") },
                { MIUseHighLiftAspiration,("UseHighLiftAspiration","UseHighLiftAspiration") }

            };

            foreach (var menu in _choicesMenuMap.Keys)
            {
                menu.CheckOnClick = true;
                menu.Click += ChoicesMenu_Click;
            }
            if (My_ConPar.Machine.MachineType == 0)
            {

                My_ConPar.Object.CurrentPLC.ShowState += CurrentPLC_ShowState;
            }
            else if (My_ConPar.Machine.MachineType == 1)
            {

                My_ConPar.Object.CurrentADT8940A1.ShowState += CurrentPLC_ShowState;
            }


            // 订阅任务队列变更事件（异步刷新，避免阻塞主线程）
            SmartColor.My_RobotManager.RobotTaskManager.Instance.TaskQueueChanged += () =>
            {
                if (this.IsHandleCreated && this.InvokeRequired)
                    this.BeginInvoke(new Action(RefreshTaskQueuePanel));
                else
                    RefreshTaskQueuePanel();
            };

            // 拖拽排序后同步优先级
            ctTaskQueuePanel1.TaskOrderChanged += TaskQueuePanel_TaskOrderChanged;


            MessageEventManager.Instance.ShowMessageRequested += Iss_Show;
            MessageEventManager.Instance.ShowBalloonTip += NotifyIcon1_Show;

            //MessageEventManager.Instance.LoopSpeakRequested += (key, text, intervalMs) =>
            //{
            //    if (ctIss.InvokeRequired)
            //        ctIss.Invoke(new Action(() => ctIss.StartLoopSpeak(key, text, intervalMs)));
            //    else
            //        ctIss.StartLoopSpeak(key, text, intervalMs);
            //};

            //MessageEventManager.Instance.StopLoopSpeakRequested += key =>
            //{
            //    if (ctIss.InvokeRequired)
            //        ctIss.Invoke(new Action(() => ctIss.StopLoopSpeak(key)));
            //    else
            //        ctIss.StopLoopSpeak(key);
            //};

            MessageEventManager.Instance.ShowLoopSpeakMessageRequested += (key, text) =>
            {
                // 添加无按钮交互行，key可作为唯一标识
                if (ctIss.InvokeRequired)
                    ctIss.Invoke(new Action(() => AddLoopSpeakRow(key, text)));
                else
                    AddLoopSpeakRow(key, text);
            };

            MessageEventManager.Instance.CloseLoopSpeakMessageRequested += key =>
            {
                if (ctIss.InvokeRequired)
                    ctIss.Invoke(new Action(() => RemoveLoopSpeakRow(key)));
                else
                    RemoveLoopSpeakRow(key);
            };


        }

        private void RefreshAreaMenu()
        {
            MiArea.DropDownItems.Clear();

            // 获取布局类型
            var layoutType = SmartColor.My_Form.Login.SplashForm.GetLayoutType();
            if (layoutType == null) return;

            // 获取所有区域属性
            var areaProps = layoutType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(p => p.Name.StartsWith("Area_")).ToList();

            foreach (var prop in areaProps)
            {
                var areaObj = prop.GetValue(null) as SmartColor.My_ConPar.Area.Base;
                if (areaObj == null)
                    continue; // 跳过未配置的区域

                var menuItem = new ToolStripMenuItem(areaObj.AreaName)
                {
                    Tag = prop // 记录属性，便于后续处理
                };
                menuItem.Click += AreaMenuItem_Click;
                MiArea.DropDownItems.Add(menuItem);
            }
        }
        private void AreaMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && menuItem.Tag is System.Reflection.PropertyInfo prop)
            {
                // 获取当前区域对象
                var currentArea = prop.GetValue(null) as SmartColor.My_ConPar.Area.Base;
                var layoutDefault = new SmartColor.My_ConPar.Area.Base();

                if (currentArea != null)
                {
                    layoutDefault.AreaType = currentArea.AreaType;
                    layoutDefault.AreaName = currentArea.AreaName;
                }

                // 弹出区域类型选择窗口
                using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("区域类型选择", null, typeof(SmartColor.My_ConPar.Area.Base), layoutDefault, true))
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK)
                        return; // 用户取消
                }
                int selectedType = layoutDefault.AreaType;
                string selectedName = layoutDefault.AreaName;

                if (selectedType == 0)
                {
                    // 用户选择不配置该区域，直接保存并退出
                    var areaObj1 = SmartColor.My_File.ConfigHelper.CreateAreaByType(0);
                    areaObj1.AreaType = 0;
                    areaObj1.AreaName = selectedName;
                    prop.SetValue(null, areaObj1);
                    SmartColor.My_File.ConfigHelper.SaveLayoutConfig(prop.DeclaringType);
                    _home?.UpdateLayout();
                    RefreshAreaMenu();
                    return;
                }

                SmartColor.My_ConPar.Area.Base areaObj;

                // 判断类型是否变化
                if (currentArea != null && currentArea.AreaType == selectedType)
                {
                    // 类型未变，直接用当前对象
                    areaObj = currentArea;
                }
                else
                {
                    // 类型变了，创建新对象
                    areaObj = SmartColor.My_File.ConfigHelper.CreateAreaByType(selectedType);
                    areaObj.AreaType = selectedType;
                    areaObj.AreaName = selectedName;
                }

                // 弹出详细参数配置窗口
                using (var dlg = new SmartColor.My_Form.ConPar.ConParShow($"{areaObj.AreaName}参数配置", null, areaObj.GetType(), areaObj, true))
                {
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        // 保存配置
                        prop.SetValue(null, areaObj);
                        SmartColor.My_File.ConfigHelper.SaveLayoutConfig(prop.DeclaringType);
                        _home?.UpdateLayout();

                        RefreshAreaMenu();
                    }
                }
            }
        }

        private void AddLoopSpeakRow(string key, string text)
        {
            if (_loopSpeakRows.ContainsKey(key))
                return;
            var row = ctIss.AddInteraction(key, text, null); // 无按钮
            _loopSpeakRows[key] = row;
        }

        private void RemoveLoopSpeakRow(string key)
        {
            if (_loopSpeakRows.TryGetValue(key, out var row))
            {
                ctIss.RemoveInteractionRow(row);
                _loopSpeakRows.Remove(key);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            PurviewChange();
            // 清空首页内容
            tabPage1.Controls.Clear();

            //启动机台线程
            StartMachine();

            //启动开料机通讯
            StartCuttingMachine();
            //启动吸光度通讯
            StartUVCommunication();


            //判断是否有使用ERP,开启线程
            if (My_ConPar.Machine.ERPInteraction != 0)
            {
                StartERP();
            }

            // 加载Home到首页
            _home = new SmartColor.My_Form.Homepage.Home();
            _home.TopLevel = false;
            _home.FormBorderStyle = FormBorderStyle.None;
            _home.Dock = DockStyle.Fill;
            tabPage1.Controls.Add(_home);
            _home.Show();

            //启动配液杯通讯
            StartCup();



            UpdateButtonVisibility();

            RefreshChoicesMenu();
        }




        // 刷新任务队列显示
        private void RefreshTaskQueuePanel()
        {
            // 获取所有任务（包括正在执行的）
            var tasks = SmartColor.My_RobotManager.RobotTaskManager.Instance.GetAllVisualTasks();

            // 按优先级排序（支持不同泛型任务）
            var orderedTasks = tasks.OrderBy(t =>
            {
                dynamic dt = t;
                return (int)dt.Priority;
            }).ToList();

            // 如果有正在执行的任务，放在最前面
            var current = SmartColor.My_RobotManager.RobotTaskManager.Instance.CurrentTask;
            if (current != null)
            {
                orderedTasks.Remove(current);
                orderedTasks.Insert(0, current);
            }

            ctTaskQueuePanel1.UpdateTasks(orderedTasks);
        }

        // 拖拽排序后同步优先级
        private void TaskQueuePanel_TaskOrderChanged(List<object> newOrder)
        {
            // 数字小优先，直接递增赋值
            for (int i = 0; i < newOrder.Count; i++)
            {
                dynamic task = newOrder[i];
                task.Priority = i + 1;
            }
            SmartColor.My_RobotManager.RobotTaskManager.Instance.UpdateTaskPriorities(newOrder);
            // 不需要手动刷新，UpdateTaskPriorities会自动触发事件
        }


        /// <summary>
        /// 更新UI机械手状态
        /// </summary>
        /// <param name="obj">状态</param>
        private void CurrentPLC_ShowState(string obj)
        {
            if (this.IsHandleCreated && this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    LabStatus.Text = obj;
                }));
            }
            else
            {
                LabStatus.Text = obj;
            }
        }

        /// <summary>
        /// 刷新选项菜单状态
        /// </summary>
        private void RefreshChoicesMenu()
        {
            foreach (var kv in _choicesMenuMap)
            {
                var menu = kv.Key;
                var propName = kv.Value.propName;
                var prop = typeof(My_ConPar.Choices).GetProperty(propName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (prop != null)
                {
                    int value = (int)prop.GetValue(null);
                    menu.Checked = value == 1;
                }
            }
        }

        private void ChoicesMenu_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menu && _choicesMenuMap.TryGetValue(menu, out var map))
            {
                // 反射获取当前值
                var prop = typeof(My_ConPar.Choices).GetProperty(map.propName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (prop != null)
                {
                    int newValue = menu.Checked ? 1 : 0;
                    prop.SetValue(null, newValue);

                    // 写入INI
                    var iniPath = Path.Combine(Environment.CurrentDirectory, "Config", "Choices.ini");
                    SmartColor.My_File.ConfigHelper.WriteValue(iniPath, map.iniKey, newValue.ToString());
                }
            }
        }

        /// <summary>
        /// 按钮可见性更新
        /// </summary>
        private void UpdateButtonVisibility()
        {
            // 称布机相关
            MiWeighingMachine.Visible = My_ConPar.Machine.UseCloth != 0;

            //分光仪相关
            MiSpectrometer.Visible = My_ConPar.Machine.Spectrometer != 0;

            // 称粉机相关
            MiPowderMachine.Visible = My_ConPar.Machine.UsePowder != 0;

            // 开料机相关
            MiCuttingMachine.Visible = My_ConPar.Machine.CuttingMachine != 0;

            // ERP交互相关
            MiERPInteraction.Visible = My_ConPar.Machine.ERPInteraction != 0;

            // 吸光度机相关
            bool hasAbs = My_ConPar.Machine.UseAbs != 0;
            BtnUV.Visible = hasAbs;
            toolStripSeparator8.Visible = hasAbs;
            Tsmi_UV.Visible = hasAbs;
            MiUVDebug.Visible = hasAbs;
            MiUVPage.Visible = hasAbs;

            // 机台类型相关
            bool hasRobotHand = (My_ConPar.Machine.MachineType == 0 || My_ConPar.Machine.MachineType == 1);
            MiHardware.Visible = hasRobotHand;
            MiIO.Visible = hasRobotHand;
            MiMotion.Visible = hasRobotHand;
            MiDelay.Visible = hasRobotHand;
            MiOther.Visible = hasRobotHand;
            MiRuning.Visible = hasRobotHand;
            MiOrder.Visible = hasRobotHand;
            MiDebug.Visible = hasRobotHand;
        }

        /// <summary>
        /// 气泡显示
        /// </summary>
        /// <param name="s">内容</param>
        private void NotifyIcon1_Show(string s)
        {
            ToastManager.ShowToast(s, 3000); // 2秒自动消失
        }

        public Task<string> ShowMessageAsync(string title, string content, string[] buttons, string defaultButton)
        {
            var tcs = new TaskCompletionSource<string>();
            // 这里可以用你原有的 Iss_Show 逻辑，只是 action 变成 tcs.SetResult
            void Callback(string btn)
            {
                tcs.TrySetResult(btn);
            }
            // UI线程安全
            if (InvokeRequired)
                Invoke(new Action(() => Iss_Show(title, content, Callback, buttons, defaultButton)));
            else
                Iss_Show(title, content, Callback, buttons, defaultButton);
            return tcs.Task;
        }

        /// <summary>
        /// 添加一行交互信息
        /// </summary>
        /// <param name="title">标题文本</param>
        /// <param name="content">内容文本</param>
        /// <param name="action">按钮点击回调，参数为按钮文本</param>
        /// <param name="buttons">可变参数，指定本行包含的按钮文本</param>
        /// <param name="defaultButton">默认按钮</param>
        private void Iss_Show(string title, string content, Action<string> action, string[] buttons, string defaultButton)
        {
            // 设置自动选择按钮的超时时间
            int timeoutMs = My_ConPar.Delay.AskTimes * 60 * 1000;

            // 自动选择的按钮，初始为传入的默认按钮
            string autoChooseButton = defaultButton;

            // 标记是否已经执行过回调，防止多次触发
            bool acted = false;
            // 封装回调，确保只会被调用一次
            Action<string> safeAction = btn =>
            {
                if (acted) return; // 已经执行过则直接返回
                acted = true;      // 标记为已执行
                action?.Invoke(btn); // 调用外部传入的回调
            };

            // 声明 row 变量，供后续线程访问
            FlowLayoutPanel row = null;

            // 展示交互信息的实际操作
            Action showAction = () =>
            {
                // 调用ctIss控件添加一行交互信息，传入标题、内容、回调和按钮
                row = this.ctIss.AddInteraction(title, content, safeAction, buttons);

                // 判断是否启用自动选择功能（配置项UseAutoChoose为1），且有可用的自动选择按钮
                if (SmartColor.My_ConPar.Choices.UseAutoChoose == 1 && autoChooseButton != null && safeAction != null)
                {
                    // 创建一个定时器，到达超时时间后自动选择默认按钮
                    var timer = new System.Windows.Forms.Timer();
                    timer.Interval = timeoutMs; // 设置定时器间隔
                    timer.Tick += (s, e) =>
                    {
                        timer.Stop();      // 停止定时器
                        timer.Dispose();   // 释放定时器资源
                        this.ctIss.RemoveInteractionRow(row); // 先移除UI上的行
                        safeAction(autoChooseButton); // 自动触发回调，传入默认按钮
                    };
                    timer.Start(); // 启动定时器
                }
            };

            // 判断当前线程是否为UI线程，如果不是则通过Invoke切换到UI线程执行showAction
            if (InvokeRequired)
                Invoke(showAction);
            else
                showAction();

            if (My_ConPar.Machine.NetWork == 0)
                return; // 如果未启用网络功能，则不进行微信推送

            // 生成唯一标识
            string timeKey = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N").Substring(0, 6);

            // 获取 machineCode 和 URL
            if (SmartColor.My_Tool.MyRegister.MyReg == null)
                SmartColor.My_Tool.MyRegister.MyReg = new SmartColor.My_Tool.MyRegister();
            string machineCode = SmartColor.My_Tool.MyRegister.MyReg.GetMNum();
            string baseUrl = SmartColor.My_ConPar.AbortInfo.URL;

            // 推送到微信
            var parameters = new Dictionary<string, string>
            {
                { "machineCode", machineCode },
                { "Text", $"{title}\n{content}" },
                { "time", timeKey },
                { "isFlag", "True" }
            };
            Task.Run(() =>
            {
                try
                {
                    SmartColor.My_Tool.HttpUtil.CreatePostHttpResponse(
                        baseUrl + "/outer/product/inBroadcastW",
                        parameters, 15000, null, null);
                }
                catch { }
            });

            // 弹窗显示后，轮询微信端结果
            Task.Run(() =>
            {
                int waited = 0, maxWaitMs = timeoutMs;
                while (waited < maxWaitMs)
                {
                    Thread.Sleep(500);
                    waited += 500;
                    var queryParams = new Dictionary<string, string>
                    {
                        { "machineCode", machineCode },
                        { "time", timeKey }
                    };
                    try
                    {
                        var response = SmartColor.My_Tool.HttpUtil.CreatePostHttpResponse(
                            baseUrl + "/outer/product/getBroadcastRe",
                            queryParams, 15000, null, null);
                        using (var st = response.GetResponseStream())
                        using (var reader = new System.IO.StreamReader(st))
                        {
                            string msg = reader.ReadToEnd();
                            var obj = JObject.Parse(msg);
                            if (obj["istrue"]?.Value<string>() == "true")
                            {
                                string btnResult = obj["state"]?.Value<string>();
                                int btnIndex;
                                if (int.TryParse(btnResult, out btnIndex) && buttons != null && btnIndex > 0 && btnIndex <= buttons.Length)
                                {
                                    // UI线程关闭弹窗并回调
                                    this.BeginInvoke(new Action(() =>
                                    {
                                        this.ctIss.RemoveInteractionRow(row);
                                        safeAction(buttons[btnIndex - 1]);
                                    }));
                                    break;
                                }
                            }
                        }
                    }
                    catch { }
                }
            });
        }
        /// <summary>
        /// 启动机台线程
        /// </summary>
        private void StartMachine()
        {

            try
            {
                switch (My_ConPar.Machine.MachineType)
                {
                    case 0:
                        //PLC版机台启动
                        {
                            Task.Run(() =>
                            {
                                My_ConPar.Object.CurrentPLC.WriteConParToPLC();
                                My_ConPar.Object.CurrentPLC.WriteInputToPLC();
                                My_ConPar.Object.CurrentPLC.WriteOutputToPLC();
                                My_ConPar.Object.CurrentPLC.StartAutoRead();
                            });
                        }
                        break;
                    case 1:
                        //板卡版机台启动
                        {
                            Task.Run(() =>
                            {
                                //天平启动
                                StartReadBalance();
                                //读取板卡所有输入输出
                            });
                        }
                        break;
                    case 2:
                        //脱机版机台启动

                        break;

                    default:
                        throw new Exception("未知机台类型，无法启动机台线程！");

                }

            }
            catch (Exception ex)
            {
                My_File.LocalTranslator.ShowMessage($"机台线程启动错误:{ex}", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// 启动配液杯通讯
        /// </summary>
        private void StartCup()
        {
            Task.Run(() =>
            {
                My_AutomaticModule.CupCommManager.Instance.SetHome(_home);
                foreach (var area in My_AutomaticModule.CupCommManager.Instance.GetAllCupAreas())
                {
                    if (My_AutomaticModule.CupCommManager.Instance.ShouldStartComm(area))
                        My_AutomaticModule.CupCommManager.Instance.EnsureCommThread(area);
                }
            });
        }

        /// <summary>
        /// 启动天平读数
        /// </summary>
        private void StartReadBalance()
        {
            while (true)
            {
                Thread.Sleep(1);

                My_ConPar.Object.Balance.WriteAndRead();
                if (My_ConPar.Object.CurrentBalance.BalanceType == 0)
                    My_Tool.BalanceStableReading.CurrentRead = Lib_SerialPort.Balance.METTLER.BalanceValue;
                else
                    My_Tool.BalanceStableReading.CurrentRead = Lib_SerialPort.Balance.SHINKO.BalanceValue;

                //if (Lib_Card.Configure.Parameter.Machine_BalanceType == 0)
                //{


                //    FADM_Object.Communal.dBalanceValue = Lib_SerialPort.Balance.METTLER.BalanceValue;

                //}
                //else
                //{
                //    FADM_Object.Communal.Shinko.WriteAndRead();
                //    FADM_Object.Communal.dBalanceValue = Lib_SerialPort.Balance.SHINKO.BalanceValue;
                //}

            }
        }

        /// <summary>
        /// 启动ERP
        /// </summary>
        private void StartERP()
        {

            try
            {
                switch (My_ConPar.Machine.ERPInteraction)
                {
                    case 0:
                        break;
                    case 1:
                        {
                            Task.Run(async () =>
                            {
                                SmartColor.My_ERPInteraction.ERPInsert eRPInsert = new My_ERPInteraction.ERPInsert();
                                await eRPInsert.StartReadAsync();
                            });
                        }
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                //ShowBtnResetBrew("开料机通讯失败", $"错误信息：{ex.Message}");
            }

        }


        /// <summary>
        /// 启动开料机通讯
        /// </summary>
        private void StartCuttingMachine()
        {
            BtnResetBrew.Visible = false;

            try
            {
                switch (My_ConPar.Machine.CuttingMachine)
                {
                    case 0:
                        BtnResetBrew.Visible = false;
                        break;
                    case 1:
                        {
                            Task.Run(() =>
                            {
                                // 启动威纶TCP通讯
                                // 获取当前汇川TCP配置对象
                                var Config = SmartColor.My_ConPar.Object.CurrentCutting as SmartColor.My_ConPar.CuttingMachine.WEINVIEW_TCP;
                                if (Config != null)
                                {
                                    string ip = Config.IP;
                                    int port = Config.Port;
                                    var tcp = new My_CuttingMachine.WEINVIEW_TCP(ip, port);
                                    My_ConPar.Object.CurrentCuttingObj = tcp;
                                    tcp.ShowResetBrewButton += ShowBtnResetBrew;



                                    //启动开料背景线程
                                    tcp.StartCom();
                                }
                                else
                                {
                                    My_File.LocalTranslator.ShowMessage("未配置威纶TCP版开料机，请先在开料机配置中完善开料机配置！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            });
                        }
                        break;
                    case 2:
                        {
                            Task.Run(() =>
                            {
                                // 启动汇川TCP通讯
                                // 获取当前汇川TCP配置对象
                                var inovanceConfig = SmartColor.My_ConPar.Object.CurrentCutting as SmartColor.My_ConPar.CuttingMachine.Inovance_TCP;
                                if (inovanceConfig != null)
                                {
                                    string ip = inovanceConfig.IP;
                                    int port = inovanceConfig.Port;
                                    var tcp = new My_CuttingMachine.Inovance_TCP(ip, port);
                                    My_ConPar.Object.CurrentCuttingObj = tcp;
                                    tcp.ShowResetBrewButton += ShowBtnResetBrew;



                                    //启动开料背景线程
                                    tcp.StartCom();
                                }
                                else
                                {
                                    My_File.LocalTranslator.ShowMessage("未配置汇川TCP开料机，请先在开料机配置中完善开料机配置！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            });
                        }
                        break;
                    case 3:
                        {
                            Task.Run(() =>
                            {
                                // 启动威纶RTU通讯
                                // 获取当前威纶RTU配置对象
                                var Config = SmartColor.My_ConPar.Object.CurrentCutting as SmartColor.My_ConPar.CuttingMachine.WEINVIEW_RTU;
                                if (Config != null)
                                {


                                    var rtu = new My_CuttingMachine.WEINVIEW_RTU(
                                        Config.PortName, Config.BaudRate, (Parity)Config.Parity,
                                        Config.DataBits, (StopBits)Config.StopBits);
                                    My_ConPar.Object.CurrentCuttingObj = rtu;
                                    rtu.ShowResetBrewButton += ShowBtnResetBrew;



                                    //启动开料背景线程
                                    rtu.StartCom();
                                }
                                else
                                {
                                    My_File.LocalTranslator.ShowMessage("未配置威纶RTU版开料机，请先在开料机配置中完善开料机配置！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            });
                        }
                        break;
                    case 4:
                        {
                            Task.Run(() =>
                            {
                                // 启动台达RTU通讯
                                // 获取当前台达RTU配置对象
                                var Config = SmartColor.My_ConPar.Object.CurrentCutting as SmartColor.My_ConPar.CuttingMachine.DELTA_RTU;
                                if (Config != null)
                                {
                                    var rtu = new My_CuttingMachine.DELTA_RTU(
                                        Config.PortName, Config.BaudRate, (Parity)Config.Parity,
                                        Config.DataBits, (StopBits)Config.StopBits);
                                    rtu.ShowResetBrewButton += ShowBtnResetBrew;


                                    My_ConPar.Object.CurrentCuttingObj = rtu;
                                    //启动开料背景线程
                                    rtu.StartCom();
                                }
                                else
                                {
                                    My_File.LocalTranslator.ShowMessage("未配置台达RTU版开料机，请先在开料机配置中完善开料机配置！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            });
                        }
                        break;
                    case 5:
                        //预留自动开料的
                        break;
                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                ShowBtnResetBrew("开料机通讯失败", $"错误信息：{ex.Message}");
            }

        }



        /// <summary>
        /// 显示重启开料机按钮
        /// </summary>
        private void ShowBtnResetBrew(string s1, string s2)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowBtnResetBrew(s1, s2)));
            }
            else
            {
                BtnResetBrew.Visible = true;
                Iss_Show(s1, s2, null, new string[] { "确定" }, "确定");
            }
        }

        /// <summary>
        /// 启动UV机通讯
        /// </summary>
        private void StartUVCommunication()
        {
            BtnResetUV.Visible = false;
            BtnUV.Visible = My_ConPar.Machine.UseAbs != 0;
            toolStripSeparator8.Visible = My_ConPar.Machine.UseAbs != 0;
            Tsmi_UV.Visible = My_ConPar.Machine.UseAbs != 0;
            MiUVDebug.Visible = My_ConPar.Machine.UseAbs != 0;
            MiUVPage.Visible = My_ConPar.Machine.UseAbs != 0;

            // 启动吸光度通讯

            try
            {
                switch (My_ConPar.Machine.UseAbs)
                {
                    case 0:
                        BtnResetUV.Visible = false;
                        break;
                    case 1:
                        // 启动UV机通讯
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                My_File.LocalTranslator.ShowMessage($"UV通讯启动失败！\n错误信息：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ShowBtnResetUV();
            }

        }

        /// <summary>
        /// 显示重启UV按钮
        /// </summary>
        private void ShowBtnResetUV()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ShowBtnResetUV));
            }
            else
            {
                BtnResetUV.Visible = true;
            }
        }

        private void PurviewChange()
        {
            Login.LoginForm.UserCache.TryGetValue(Properties.Settings.Default.Account, out var userInfo);
            int purview = userInfo.Purview;
            BtnUserSwitching.Text = userInfo.Name;
            // 在设置窗口标题后，添加欢迎当前用户的状态栏提示
            this.Text = $"智能染色系统 - 版本号: {Application.ProductVersion} - {BtnUserSwitching.Text}";
            switch (purview)
            {
                case 1:
                    toolStripSplitButton1.Visible = true;
                    toolStripSeparator4.Visible = true;
                    toolStripSplitButton6.Visible = true;
                    toolStripSeparator2.Visible = true;
                    toolStripSplitButton2.Visible = false;
                    toolStripSeparator3.Visible = false;
                    break;

                case 2:
                    toolStripSplitButton1.Visible = true;
                    toolStripSeparator4.Visible = true;
                    toolStripSplitButton6.Visible = true;
                    toolStripSeparator2.Visible = true;
                    toolStripSplitButton2.Visible = true;
                    toolStripSeparator3.Visible = true;
                    break;

                default:
                    toolStripSplitButton1.Visible = false;
                    toolStripSeparator4.Visible = false;
                    toolStripSplitButton6.Visible = false;
                    toolStripSeparator2.Visible = false;
                    toolStripSplitButton2.Visible = false;
                    toolStripSeparator3.Visible = false;
                    break;

            }
        }

        private void BtnUserSwitching_Click(object sender, EventArgs e)
        {
            using (var loginForm = new Login.LoginForm())
            {
                loginForm.ShowDialog();

                PurviewChange();

            }
        }

        private void MiBrewingProcess_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiBrewingProcess.Name.Substring(2),
                MiBrewingProcess.Text, new BasicData.BrewingProcess());
        }

        private void Tsmi_UV_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(Tsmi_UV.Name.Substring(2),
                Tsmi_UV.Text, new BasicData.UVMeasurement());
        }
        private void DyeingProcessConfiguration_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(DyeingProcessConfiguration.Name,
                DyeingProcessConfiguration.Text,
                new BasicData.DPConfiguration(1));
        }

        private void PostTreatmentProcessConfiguration_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(PostTreatmentProcessConfiguration.Name,
               PostTreatmentProcessConfiguration.Text,
               new BasicData.DPConfiguration(2));
        }

        private void DyeingAndFixationProcessConfiguration_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(DyeingAndFixationProcessConfiguration.Name,
                DyeingAndFixationProcessConfiguration.Text,
                new BasicData.DPPConfiguration());
        }

        private void MiAssistant_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiAssistant.Name.Substring(2),
                MiAssistant.Text, new BasicData.Assistant());
        }

        private void MiBottle_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiBottle.Name.Substring(2),
                MiBottle.Text, new BasicData.Bottle());
        }

        private void MiLimitSet_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiLimitSet.Name.Substring(2),
                MiLimitSet.Text, new BasicData.DyeType());
        }

        private void MiFormulaGroup_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiFormulaGroup.Name.Substring(2),
                MiFormulaGroup.Text, new BasicData.FormulaGroup());
        }

        private void MiOperator_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiOperator.Name.Substring(2),
                MiOperator.Text, new BasicData.Operator());
        }

        private void MiNote_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiNote.Name.Substring(2),
             MiNote.Text, new BasicData.Note());
        }

        private void MiDebug_Click(object sender, EventArgs e)
        {
            var debug = new MachineDebugging.Debug();
            ctTab1.OpenTab(MiDebug.Name.Substring(2),
                MiDebug.Text, debug);


        }

        private void MiUVDebug_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiUVDebug.Name.Substring(2),
                MiUVDebug.Text, new MachineDebugging.UVDebug());
        }

        private void MiRun_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiRun.Name.Substring(2),
                MiRun.Text, new MachineDebugging.Run());
        }

        private void MiAlarm_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiAlarm.Name.Substring(2),
                MiAlarm.Text, new MachineDebugging.Alarm());
        }

        private void MiHistoryPage_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiHistoryPage.Name.Substring(2),
                MiHistoryPage.Text, new HistoricalData.Dyeing());
        }

        private void MiFormulaPage_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiFormulaPage.Name.Substring(2),
                MiFormulaPage.Text, new HistoricalData.Formulas());
        }

        private void MiBrewPage_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiBrewPage.Name.Substring(2),
                MiBrewPage.Text, new HistoricalData.Brew());
        }

        private void MiUVPage_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiUVPage.Name.Substring(2),
                MiUVPage.Text, new HistoricalData.UV());
        }

        private void MiRectificationPage_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiRectificationPage.Name.Substring(2),
                MiRectificationPage.Text, new HistoricalData.Rectification());
        }

        private void MiSelfPage_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(MiSelfPage.Name.Substring(2),
                MiSelfPage.Text, new HistoricalData.Self());
        }

        private void MiAbort_Click(object sender, EventArgs e)
        {
            using (var abort = new Help.Abort())
            {
                abort.ShowDialog();
            }
        }

        private void MiRegister_Click(object sender, EventArgs e)
        {
            using (var regForm = new Help.Register())
            {
                regForm.ShowDialog();
            }
        }

        private void BtnDyeing_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(BtnDyeing.Name.Substring(2),
             BtnDyeing.Text, new My_Form.DyeingMan.DyeingMan());
        }
        private void BtnUV_Click(object sender, EventArgs e)
        {
            ctTab1.OpenTab(BtnUV.Name.Substring(2),
               BtnUV.Text, new My_Form.UVMan.UVMan());
        }


        private void MiMachine_Click(object sender, EventArgs e)
        {
            int oldLayout = My_ConPar.Machine.MachineLayout;
            var path = Path.Combine(Environment.CurrentDirectory, "Config", "Machine.ini");
            using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("机台参数", "Machine.ini", typeof(My_ConPar.Machine), true))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    if (My_ConPar.Machine.MachineType == 0)
                        // 机台参数已保存，写入PLC配置区
                        My_ConPar.Object.CurrentPLC.WriteConParToPLC();
                }
            }

            if (oldLayout != My_ConPar.Machine.MachineLayout)
            {
                _layoutChanged = true;
                // 布局变更，重新加载区域配置
                MiArea_Reset(null, null);

            }

        }

        private void MiHardware_Click(object sender, EventArgs e)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Config", "Hardware.ini");
            var oldCylinderPositioningEncoder = My_ConPar.Hardware.UseCylinderPositioningEncoder;
            using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("硬件参数", "Hardware.ini", typeof(My_ConPar.Hardware), true))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    // 硬件参数已保存，写入PLC配置区
                    if (My_ConPar.Machine.MachineType == 0)
                    {
                        My_ConPar.Object.CurrentPLC.WriteConParToPLC();
                        if (oldCylinderPositioningEncoder != My_ConPar.Hardware.UseCylinderPositioningEncoder)
                        {
                            // 气缸定位编码器使用状态变更，可能影响部分区域配置，重置区域配置
                            My_File.LocalTranslator.ShowMessage("气缸定位编码器使用状态变更，需要断电重启PLC才生效！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                    }

                }
            }
        }

        private void MiArea_Reset(object sender, EventArgs e)
        {
            var layoutType = SmartColor.My_Form.Login.SplashForm.GetLayoutType();
            if (layoutType == null) return;

            var areaProps = layoutType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(p => p.Name.StartsWith("Area_")).ToList();

            foreach (var prop in areaProps)
            {
                var name = prop.Name;
                if (!name.StartsWith("Area_")) continue;

                // 获取当前区域对象
                var areaObj = prop.GetValue(null) as SmartColor.My_ConPar.Area.Base;

                // 始终弹类型选择窗体，允许更换类型
                var layoutDefault = new SmartColor.My_ConPar.Area.Base();
                if (areaObj != null)
                {
                    layoutDefault.AreaType = areaObj.AreaType;
                    layoutDefault.AreaName = areaObj.AreaName;
                }
                using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("区域类型选择", null, typeof(SmartColor.My_ConPar.Area.Base), layoutDefault, !_layoutChanged))
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK)
                        return; // 用户取消，直接退出整个流程
                }
                int selectedType = layoutDefault.AreaType;
                string selectedName = layoutDefault.AreaName;



                // 类型变更/布局更改则新建对象，否则复用
                if (areaObj == null || areaObj.AreaType != selectedType || _layoutChanged)
                {
                    areaObj = SmartColor.My_File.ConfigHelper.CreateAreaByType(selectedType);
                    areaObj.AreaType = selectedType;
                    areaObj.AreaName = selectedName;

                }

                if (selectedType == 0)
                {
                    // 用户选择不配置该区域，跳过
                    prop.SetValue(null, areaObj);
                    continue;
                }

                // 母液瓶区特殊处理
                if (areaObj is SmartColor.My_ConPar.Area.BottleArea.Bottle bottleArea)
                {
                    // 母液瓶参数编辑
                    using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("母液瓶参数补全", null, bottleArea.GetType(), bottleArea, !_layoutChanged))
                    {
                        if (dlg.ShowDialog(this) != DialogResult.OK)

                            return;  // 用户取消，直接退出整个流程


                    }
                    // 天平参数补全
                    if (bottleArea.BottleNum > 0 && bottleArea.BottleColumn > 0 && bottleArea.BottleNum % bottleArea.BottleColumn != 0)
                    {
                        if (bottleArea.EmbeddedBalance == null)
                            bottleArea.EmbeddedBalance = new SmartColor.My_ConPar.Area.Balance.Balance();
                        using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("母液瓶区天平参数补全", null, bottleArea.EmbeddedBalance.GetType(), bottleArea.EmbeddedBalance, !_layoutChanged))
                        {
                            if (dlg.ShowDialog(this) != DialogResult.OK)

                                return;  // 用户取消，直接退出整个流程

                        }
                    }
                }
                else
                {
                    // 其它区域参数编辑
                    using (var dlg = new SmartColor.My_Form.ConPar.ConParShow($"{areaObj.AreaName}参数补全", null, areaObj.GetType(), areaObj, !_layoutChanged))
                    {
                        if (dlg.ShowDialog(this) != DialogResult.OK)

                            return;  // 用户取消，直接退出整个流程

                    }
                }

                // 保存配置
                prop.SetValue(null, areaObj);

            }
            SmartColor.My_File.ConfigHelper.SaveLayoutConfig(layoutType);

            //刷新布局显示


            _home.UpdateLayout();
            RefreshAreaMenu();
            _layoutChanged = false;

        }

        private void MiCuttingMachine_Click(object sender, EventArgs e)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Config", "CuttingMachine.ini");
            switch (My_ConPar.Machine.CuttingMachine)
            {
                case 0:
                    My_File.LocalTranslator.ShowMessage("未启用开料机，请先在机台配置中开启开料机配置！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case 1:
                    {
                        var cut = My_ConPar.Object.CurrentCutting as My_ConPar.CuttingMachine.WEINVIEW_TCP;
                        SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                            this, path, typeof(My_ConPar.CuttingMachine.WEINVIEW_TCP),
                            () => new SmartColor.My_Form.ConPar.ConParShow("开料机参数", "CuttingMachine.ini", typeof(My_ConPar.CuttingMachine.WEINVIEW_TCP), cut, true),
                            cut,
                            true
                        );
                    }
                    break;
                case 2:
                    {
                        var cut = My_ConPar.Object.CurrentCutting as My_ConPar.CuttingMachine.Inovance_TCP;
                        SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                            this, path, typeof(My_ConPar.CuttingMachine.Inovance_TCP),
                            () => new SmartColor.My_Form.ConPar.ConParShow("开料机参数", "CuttingMachine.ini", typeof(My_ConPar.CuttingMachine.Inovance_TCP), cut, true),
                            cut,
                            true
                        );

                    }
                    break;
                case 3:
                    {
                        var cut = My_ConPar.Object.CurrentCutting as My_ConPar.CuttingMachine.WEINVIEW_RTU;
                        SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                        this, path, typeof(My_ConPar.CuttingMachine.WEINVIEW_RTU),
                        () => new SmartColor.My_Form.ConPar.ConParShow("开料机参数", "CuttingMachine.ini", typeof(My_ConPar.CuttingMachine.WEINVIEW_RTU), cut, true),
                            cut,
                            true
                        );
                    }
                    break;
                case 4:
                    {
                        var cut = My_ConPar.Object.CurrentCutting as My_ConPar.CuttingMachine.DELTA_RTU;
                        SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                             this, path, typeof(My_ConPar.CuttingMachine.DELTA_RTU),
                             () => new SmartColor.My_Form.ConPar.ConParShow("开料机参数", "CuttingMachine.ini", typeof(My_ConPar.CuttingMachine.DELTA_RTU), cut, true),
                                 cut,
                                 true
                             );
                    }
                    break;
                case 5:
                    {
                        //预留自动开料的

                    }
                    break;
                default:
                    SmartColor.My_File.LocalTranslator.ShowMessage("开料机类型未配置或错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }

        private void MiPowderMachine_Click(object sender, EventArgs e)
        {
            if (My_ConPar.Machine.UsePowder == 0)
            {
                My_File.LocalTranslator.ShowMessage("未启用称粉机，请先在机台配置中开启称粉机配置！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            var path = Path.Combine(Environment.CurrentDirectory, "Config", "Powder.ini");
            SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                this, path, typeof(My_ConPar.PowderMachine.Powder),
                () => new SmartColor.My_Form.ConPar.ConParShow("称粉机参数", "Powder.ini", typeof(My_ConPar.PowderMachine.Powder), true),
                null,
                true
            );
        }

        private void MiWeighingMachine_Click(object sender, EventArgs e)
        {
            if (My_ConPar.Machine.UseCloth == 0)
            {
                My_File.LocalTranslator.ShowMessage("未启用称布机，请先在机台配置中开启称布机配置！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var path = Path.Combine(Environment.CurrentDirectory, "Config", "Weighing.ini");
            SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                this, path, typeof(My_ConPar.WeighingMachine.Weighing),
                () => new SmartColor.My_Form.ConPar.ConParShow("称布机参数", "Weighing.ini", typeof(My_ConPar.WeighingMachine.Weighing), true),
                null,
                true
            );
        }

        private void MiERPInteraction_Click(object sender, EventArgs e)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Config", "ERP.ini");
            if (My_ConPar.Machine.ERPInteraction == 0)
            {
                My_File.LocalTranslator.ShowMessage("未启用ERP交互，请先在机台配置中开启ERP交互配置！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (My_ConPar.Machine.ERPInteraction == 1)
            {
                SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                    this, path, typeof(My_ConPar.ERPInteraction.Txt),
                    () => new SmartColor.My_Form.ConPar.ConParShow("ERP交互参数", "ERP.ini", typeof(My_ConPar.ERPInteraction.Txt), true),
                    null,
                    true
                );
            }
            else if (My_ConPar.Machine.ERPInteraction == 2)
            {
                SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                    this, path, typeof(My_ConPar.ERPInteraction.Database),
                    () => new SmartColor.My_Form.ConPar.ConParShow("ERP交互参数", "ERP.ini", typeof(My_ConPar.ERPInteraction.Database), true),
                    null,
                    true
                );
            }
        }

        private void MiControl_Click(object sender, EventArgs e)
        {
            if (My_ConPar.Machine.MachineType < 0 || My_ConPar.Machine.MachineType > 2)
            {
                SmartColor.My_File.LocalTranslator.ShowMessage("机台类型配置错误，无法配置IO参数！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (My_ConPar.Machine.MachineType == 0)
            {
                var plcIOPath = Path.Combine(Environment.CurrentDirectory, "Config", "PLC_IO.ini");
                var plcIO = new SmartColor.My_ConPar.Type.PLC.IO();
                using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("PLC IO参数", "PLC_IO.ini", typeof(SmartColor.My_ConPar.Type.PLC.IO), plcIO, true))
                {
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        // 配置已保存，发送到PLC
                        My_ConPar.Object.CurrentPLC.WriteInputToPLC();
                        My_ConPar.Object.CurrentPLC.WriteOutputToPLC();

                    }
                }

            }
            else if (My_ConPar.Machine.MachineType == 1)
            {
                var boardIOPath = Path.Combine(Environment.CurrentDirectory, "Config", "BoardCard_IO.ini");
                var boardIO = new SmartColor.My_ConPar.Type.BoaedCard.IO();
                SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                    this, boardIOPath, typeof(SmartColor.My_ConPar.Type.BoaedCard.IO),
                    () => new SmartColor.My_Form.ConPar.ConParShow("板卡 IO参数", "BoardCard_IO.ini", typeof(SmartColor.My_ConPar.Type.BoaedCard.IO), boardIO, true),
                    boardIO,
                    true
                );
            }
            else if (My_ConPar.Machine.MachineType == 2)
            {
                SmartColor.My_File.LocalTranslator.ShowMessage("机台类型为脱机版，无法配置IO参数！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 其它类型暂不处理
        }

        private void MiMotion_Click(object sender, EventArgs e)
        {
            switch (My_ConPar.Machine.MachineType)
            {
                case 0:
                    {
                        var path = Path.Combine(Environment.CurrentDirectory, "Config", "PLC_Motion.ini");
                        var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.PLC.Motion;
                        SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                            this, path, typeof(My_ConPar.Type.PLC.Motion),
                            () => new SmartColor.My_Form.ConPar.ConParShow("PLC运动参数", "PLC_Motion.ini", typeof(My_ConPar.Type.PLC.Motion), motion, true),
                            motion,
                            true
                        );
                    }
                    break;

                case 1:
                    {
                        var path = Path.Combine(Environment.CurrentDirectory, "Config", "BoaedCard_Motion.ini");
                        var motion = My_ConPar.Object.CurrentMotion as My_ConPar.Type.BoaedCard.Motion;
                        SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                            this, path, typeof(My_ConPar.Type.BoaedCard.Motion),
                            () => new SmartColor.My_Form.ConPar.ConParShow("板卡运动参数", "BoaedCard_Motion.ini", typeof(My_ConPar.Type.BoaedCard.Motion), motion, true),
                            motion,
                            true
                        );
                    }
                    break;
                case 2:
                    {
                        SmartColor.My_File.LocalTranslator.ShowMessage("机台类型为脱机版，无法配置运动参数！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                default:
                    SmartColor.My_File.LocalTranslator.ShowMessage("机台类型配置错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }

        private void MiDelay_Click(object sender, EventArgs e)
        {
            if (My_ConPar.Machine.MachineType == 2)
            {
                SmartColor.My_File.LocalTranslator.ShowMessage("机台类型为脱机版，无法配置延时参数！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("延时参数", "Delay.ini", typeof(My_ConPar.Delay), true))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    if (My_ConPar.Machine.MachineType != 0)
                        return;
                    // 延时参数已保存，写入PLC配置区
                    My_ConPar.Object.CurrentPLC.WriteConParToPLC();
                }
            }
        }

        private void MiCorrection_Click(object sender, EventArgs e)
        {
            if (My_ConPar.Machine.MachineType == 2)
            {
                SmartColor.My_File.LocalTranslator.ShowMessage("机台类型为脱机版，无法配置校正参数！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("校正参数", "Correction.ini", typeof(My_ConPar.Correction), true))
            {
                dlg.ShowDialog(this);
            }
        }

        private void MiOther_Click(object sender, EventArgs e)
        {
            if (My_ConPar.Machine.MachineType == 2)
            {
                SmartColor.My_File.LocalTranslator.ShowMessage("机台类型为脱机版，无法配置其他参数！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("其他参数", "Other.ini", typeof(My_ConPar.Other), true))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    if (My_ConPar.Machine.MachineType != 0)
                        return;
                    // 其他参数已保存，写入PLC配置区
                    My_ConPar.Object.CurrentPLC.WriteConParToPLC();
                }
            }
        }

        private void MiDatabase_Click(object sender, EventArgs e)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Config", "Database.ini");
            SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                this, path, typeof(My_ConPar.Database),
                () => new SmartColor.My_Form.ConPar.ConParShow("数据库参数", "Database.ini", typeof(My_ConPar.Database), true),
                null,
                true
            );
        }

        private void BtnResetUV_Click(object sender, EventArgs e)
        {

        }

        private void BtnResetBrew_Click(object sender, EventArgs e)
        {
            BtnResetBrew.Visible = false;
            StartCuttingMachine();
        }

        private void MiOrder_Click(object sender, EventArgs e)
        {
            using (var order = new ConPar.OrderInfo("机械手进程", typeof(My_ConPar.Order.BigProcess)))
            {
                order.ShowDialog();
            }
        }

        private void MiWashCup_Click(object sender, EventArgs e)
        {
            if (My_ConPar.Machine.MachineType == 2)
            {
                SmartColor.My_File.LocalTranslator.ShowMessage("机台类型为脱机版，无法配置洗杯参数！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var dlg = new SmartColor.My_Form.ConPar.ConParShow("洗杯参数", "WashCup.ini", typeof(My_ConPar.WashCup), true))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    if (My_ConPar.Machine.MachineType != 0)
                        return;
                    // 洗杯参数已保存，写入PLC配置区
                    My_ConPar.Object.CurrentPLC.WriteConParToPLC();
                }
            }
        }



        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = My_File.LocalTranslator.ShowMessage("是否退出智能染色系统？", "退出确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true; // 阻止关闭
                return;
            }
            // 强制杀死当前进程（包括所有线程）
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void MiSpectrometer_Click(object sender, EventArgs e)
        {
            if (My_ConPar.Machine.Spectrometer == 0)
            {
                My_File.LocalTranslator.ShowMessage("未启用分光仪，请先在机台配置中开启分光仪配置！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var path = Path.Combine(Environment.CurrentDirectory, "Config", "Spectrometer.ini");
            SmartColor.My_File.ConfigHelper.CheckAndAssignOrPrompt(
                this, path, typeof(My_ConPar.Spectrometer.Desktop),
                () => new SmartColor.My_Form.ConPar.ConParShow("分光仪参数", "Spectrometer.ini", typeof(My_ConPar.Spectrometer.Desktop), true),
                null,
                true
            );
        }
    }
}
