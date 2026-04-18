using SmartColor.My_AutomaticModule;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Form.Homepage;
using SmartColor.My_Form.Login;
using SmartColor.My_Tool;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 配液杯区域控件，仅负责区域参数、UI布局、杯子属性刷新。
    /// 通讯线程的启动/停止/滴液等由 CylinderCommManager 统一管理。
    /// </summary>
    public partial class CtCupArea : UserControl
    {
        private int _rowCount = 2;
        private int _colCount = 6;
        private int _startCupNo = 1;
        private Size _cupOriginalSize = new Size(40, 80);
        private GroupBox _groupBox;
        private int _vertical = 1; // 0-横着编号,1-竖着编号

        // 区域参数（由SetLayout传入）
        private int _areaType = 0;
        private string _ip = "";
        private int _port = 502;
        private string _areaName = "";
        private int _totalCupNum = 0;
        private int _hmiType = 0;

        // 注册自定义控件的回调
        private Action<My_Interface.ICustomUpdatable> _registerCustomControl;

        // 优化1：引入线程安全的缓存，用于批量更新杯子技术名称，减少UI线程调用频率
        private ConcurrentDictionary<int, (string techName, int stepNo)> _cupTechCache = new ConcurrentDictionary<int, (string, int)>();

        public CtCupArea()
        {
            InitializeComponent();
            InitGroupBox("X号区域");

            this.Resize += (s, e) => LayoutCups();
            LayoutCups();

            // 权限控制菜单项可见性
            RefreshLidMenuVisibility();
            RefreshHighWashButtonVisibility(); // 新增：根据HighWash显示高温洗杯按钮
            LoginForm.UserChanged -= LoginForm_UserChanged; // 防止重复绑定
            LoginForm.UserChanged += LoginForm_UserChanged;
            SmartColor.My_ConPar.Choices.HighWashChanged -= Choices_HighWashChanged;
            SmartColor.My_ConPar.Choices.HighWashChanged += Choices_HighWashChanged;
        }

        private void Choices_HighWashChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(RefreshHighWashButtonVisibility));
            else
                RefreshHighWashButtonVisibility();
        }

        /// <summary>
        /// 根据Choices.HighWash刷新“高温洗杯”菜单项的可见性
        /// </summary>
        private void RefreshHighWashButtonVisibility()
        {
            TsmiAllHighTemWash.Visible = SmartColor.My_ConPar.Choices.HighWash == 1;
        }

        private void LoginForm_UserChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(RefreshLidMenuVisibility));
            else
                RefreshLidMenuVisibility();
        }

        private void RefreshLidMenuVisibility()
        {
            var account = SmartColor.Properties.Settings.Default.Account;
            bool isEngineer = false;
            if (LoginForm.UserCache != null && LoginForm.UserCache.TryGetValue(account, out var userInfo))
                isEngineer = userInfo.Purview == 2;

            TsmiAllOpenLid.Visible = isEngineer;
            TsmiAllCloseLid.Visible = isEngineer;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LoginForm.UserChanged -= LoginForm_UserChanged;
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 区域类型
        /// </summary>
        public int AreaType => _areaType;

        /// <summary>
        /// PLC IP
        /// </summary>
        public string Ip => _ip;

        /// <summary>
        /// PLC端口
        /// </summary>
        public int Port => _port;

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName => _areaName;

        /// <summary>
        /// 起始杯号
        /// </summary>
        public int StartCupNo => _startCupNo;

        /// <summary>
        /// HMI类型
        /// </summary>
        public int HmiType => _hmiType;

        /// <summary>
        /// 总杯号
        /// </summary>
        public int TotalCupNum => _totalCupNum;

        /// <summary>
        /// 当前区域是否包含指定杯号
        /// </summary>
        public bool ContainsCup(int cupNum)
        {
            return cupNum >= _startCupNo && cupNum < _startCupNo + _rowCount * _colCount;
        }

        /// <summary>
        /// 初始化GroupBox
        /// </summary>
        private void InitGroupBox(string text)
        {
            if (_groupBox == null)
            {
                _groupBox = new GroupBox
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                    Font = new Font("微软雅黑", 10, FontStyle.Bold)
                };
                this.Controls.Add(_groupBox);
            }
            else
            {
                _groupBox.Text = text;
            }
        }

        /// <summary>
        /// 设置区域布局及通讯参数
        /// </summary>
        public void SetLayout(int rowCount, int colCount, int startCupNo = 1, int vertical = 1, string groupText = "",
            int areaType = 0, string ip = "", int port = 502, string areaName = "", int hmiType = 0)
        {
            // 软件布局和实际布局行和列相反
            if (vertical == 0)
            {
                _rowCount = rowCount;
                _colCount = colCount;
            }
            else
            {
                _rowCount = colCount;
                _colCount = rowCount;
            }

            _startCupNo = startCupNo;
            _vertical = vertical;
            _areaType = areaType;
            _ip = ip;
            _port = port;
            _areaName = areaName;
            _totalCupNum = rowCount * colCount;
            _hmiType = hmiType;
            InitGroupBox(groupText);
            LayoutCups();
            if (areaType >= 2 && areaType <= 7)
            {
                this.ContextMenuStrip = this.contextMenuStrip1;
            }
        }

        /// <summary>
        /// PLC数据推送到UI，自动刷新本区域的CtCup控件
        /// 优化2：合并UI更新逻辑，减少对UI控件的频繁属性访问
        /// </summary>
        public void OnCupDataReceived(int cupIndex, Dictionary<string, object> cupData)
        {
            if (this.IsHandleCreated)
            {
                // 使用BeginInvoke确保在UI线程执行
                _ = this.BeginInvoke((Action)(() =>
                {
                    string cupNoStr = cupData.ContainsKey("CupIndex") ? cupData["CupIndex"].ToString() : cupIndex.ToString();
                    if (!int.TryParse(cupNoStr, out int cupNum)) return;

                    var ctrl = _groupBox.Controls.OfType<CtCup>().FirstOrDefault(c => c.NO == cupNoStr);
                    if (ctrl == null) return;

                    bool needRefresh = false; // 标记是否需要最终刷新控件

                    // 1. 更新状态
                    string newStatus = cupData.ContainsKey("CurrentStatus") ? cupData["CurrentStatus"]?.ToString() : null;
                    if (!string.IsNullOrEmpty(newStatus) && ctrl.Status != newStatus)
                    {
                        ctrl.Status = newStatus;
                        needRefresh = true;
                        // 状态更新到数据库的操作移到后台，避免阻塞UI
                        My_Tool.CupAuxiliary.UpdateCupState(cupNum, newStatus);
                    }

                    // 2. 更新温度
                    if (cupData.ContainsKey("ActualTemp") && ctrl.ActualTemp != Convert.ToDouble(cupData["ActualTemp"] ?? .0))
                    {
                        ctrl.ActualTemp = Convert.ToDouble(cupData["ActualTemp"] ?? .0);
                        needRefresh = true;
                    }

                    // 3. 更新锁定状态
                    if (cupData.ContainsKey("LockSignal") && ctrl.LockStatus != Convert.ToInt32(cupData["LockSignal"] ?? 0))
                    {
                        ctrl.LockStatus = Convert.ToInt32(cupData["LockSignal"] ?? 0);
                        needRefresh = true;
                    }

                    // 4. 更新步骤号和技术名称 (合并处理，避免重复查找和更新)
                    int currentStepNo = cupData.ContainsKey("CurrentStepNo") ? Convert.ToInt32(cupData["CurrentStepNo"] ?? 0) : 0;
                    string techName = cupData.ContainsKey("TechnologyName") ? cupData["TechnologyName"]?.ToString() : null;

                    bool stepNoChanged = cupData.ContainsKey("CurrentStepNo") && ctrl.CurrentStepNo != currentStepNo;
                    bool techNameChanged = false;
                    if (techName != null)
                    {
                        // 使用缓存检查技术名是否真的变化
                        var cached = _cupTechCache.GetOrAdd(cupNum, (key) => (techName, currentStepNo));
                        if (cached.techName != techName || cached.stepNo != currentStepNo)
                        {
                            _cupTechCache[cupNum] = (techName, currentStepNo);
                            techNameChanged = true;
                        }
                    }


                    if (techNameChanged || stepNoChanged)
                    {
                        ctrl.CurrentStepNo = currentStepNo;
                        ctrl.TechnologyName = techName;
                        needRefresh = true;
                        // 技术名和步骤号更新数据库也移到后台
                        My_Tool.CupAuxiliary.UpdateCupTechnologyName(cupNum, techName, currentStepNo);
                    }

                    // 5. 更新排水、安全温度、保持时间等次要属性（通常变化不频繁）
                    if (cupData.ContainsKey("DrainageDown") && ctrl.DrainageDown != Convert.ToInt32(cupData["DrainageDown"] ?? 0))
                    {
                        ctrl.DrainageDown = Convert.ToInt32(cupData["DrainageDown"] ?? 0);
                        needRefresh = true;
                    }
                    if (cupData.ContainsKey("SafeOpeningTemp") && ctrl.SafeOpeningTemp != Convert.ToInt32(cupData["SafeOpeningTemp"] ?? 0))
                    {
                        ctrl.SafeOpeningTemp = Convert.ToInt32(cupData["SafeOpeningTemp"] ?? 0);
                        needRefresh = true;
                    }
                    if (cupData.ContainsKey("HoldingTime") && ctrl.HoldingTime != Convert.ToInt32(cupData["HoldingTime"] ?? 0))
                    {
                        ctrl.HoldingTime = Convert.ToInt32(cupData["HoldingTime"] ?? 0);
                        needRefresh = true;
                    }

                    // 6. 只有任何属性变化了，才执行一次刷新
                    if (needRefresh)
                    {
                        ctrl.Refresh();
                    }
                }));
            }
        }

        public void OnCupDataReceived(int cupIndex)
        {
            var ctrl = _groupBox.Controls.OfType<CtCup>().FirstOrDefault(c => c.NO == cupIndex.ToString());
            if (ctrl == null) return;
            var dt = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {cupIndex}");
            if (dt.Rows.Count > 0)
            {
                ctrl.UpdateFromData(dt.Rows[0]);
            }

        }




        /// <summary>
        /// 区域内杯子布局
        /// </summary>
        private void LayoutCups()
        {
            if (_groupBox == null) return;

            _groupBox.SuspendLayout();
            _groupBox.Controls.Clear();

            try
            {
                Rectangle area = _groupBox.DisplayRectangle;
                int totalWidth = area.Width;
                int totalHeight = area.Height;
                int offsetX = area.X;
                int offsetY = area.Y;

                int minSpacing = 4;

                int maxCupWidth = (totalWidth - minSpacing * (_colCount + 1)) / _colCount;
                int maxCupHeight = (totalHeight - minSpacing * (_rowCount + 1)) / _rowCount;
                float scaleW = (float)maxCupWidth / _cupOriginalSize.Width;
                float scaleH = (float)maxCupHeight / _cupOriginalSize.Height;
                float scale = Math.Min(scaleW, scaleH);

                Size cupSize = new Size(
                    Math.Max(10, (int)(_cupOriginalSize.Width * scale)),
                    Math.Max(20, (int)(_cupOriginalSize.Height * scale))
                );

                int usedWidth = cupSize.Width * _colCount;
                int usedHeight = cupSize.Height * _rowCount;
                int spacingX = _colCount > 1 ? (totalWidth - usedWidth) / (_colCount + 1) : (totalWidth - usedWidth) / 2;
                int spacingY = _rowCount > 1 ? (totalHeight - usedHeight) / (_rowCount + 1) : (totalHeight - usedHeight) / 2;
                spacingX = Math.Max(minSpacing, spacingX);
                spacingY = Math.Max(minSpacing, spacingY);

                int cupNo = _startCupNo;
                if (_vertical == 0)
                {
                    // 横着编号：行优先
                    for (int row = 0; row < _rowCount; row++)
                    {
                        for (int col = 0; col < _colCount; col++)
                        {
                            int x = offsetX + spacingX + col * (cupSize.Width + spacingX);
                            int y = offsetY + spacingY + row * (cupSize.Height + spacingY);

                            var cup = new CtCup
                            {
                                Size = cupSize,
                                Location = new Point(x, y),
                                NO = cupNo.ToString(),
                            };
                            if (!(_areaType >= 2 && _areaType <= 7))
                            {
                                cup.ContextMenuStrip = null;
                            }
                            _registerCustomControl?.Invoke(cup);
                            _groupBox.Controls.Add(cup);

                            cupNo++;
                        }
                    }
                }
                else
                {
                    // 竖着编号：列优先
                    for (int col = 0; col < _colCount; col++)
                    {
                        for (int row = 0; row < _rowCount; row++)
                        {
                            int x = offsetX + spacingX + col * (cupSize.Width + spacingX);
                            int y = offsetY + spacingY + row * (cupSize.Height + spacingY);

                            var cup = new CtCup
                            {
                                Size = cupSize,
                                Location = new Point(x, y),
                                NO = cupNo.ToString(),
                            };
                            if (!(_areaType >= 2 && _areaType <= 7))
                            {
                                cup.ContextMenuStrip = null;
                            }
                            _registerCustomControl?.Invoke(cup);
                            _groupBox.Controls.Add(cup);
                            cupNo++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CtCupArea LayoutCups", ex);
            }

            _groupBox.ResumeLayout();
        }

        /// <summary>
        /// 重新注册自定义控件的回调，在每次布局时调用
        /// </summary>
        public Action<My_Interface.ICustomUpdatable> RegisterCustomControl
        {
            get => _registerCustomControl;
            set
            {
                _registerCustomControl = value;
                LayoutCups(); // 赋值后重绘，保证注册
            }
        }





        private void TsmiAllOnLine_Click(object sender, EventArgs e)
        {
            _ = Task.Run(async () =>
            {
                // 立即禁用菜单项，防止重复点击
                SetContextMenuItemsEnabled(false);
                try
                {

                    var dt = DateTime.Now;
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"点击{_areaName}全部上线按钮"
                    }, dt);
                    SmartColor.My_AutomaticModule.CupCommManager.Instance.EnsureCommThread(this);

                    var cups = _groupBox.Controls.OfType<CtCup>().Where(c => c.Status == "下线").ToList();
                    int success = 0, fail = 0;
                    var comm = SmartColor.My_AutomaticModule.CupCommManager.Instance.GetCommObject(this);
                    var tasks = new List<Task>();

                    foreach (var cup in cups)
                    {
                        if (!int.TryParse(cup.NO, out int cupNo)) { fail++; continue; }
                        if (comm != null)
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                try
                                {
                                    await comm.SendOnLine(cupNo, 0);
                                    Interlocked.Increment(ref success);
                                }
                                catch
                                {
                                    Interlocked.Increment(ref fail);
                                }
                            }));
                        }
                        else
                        {
                            fail++;
                        }
                    }
                    await Task.WhenAll(tasks);





                }
                finally
                {
                    SetContextMenuItemsEnabled(true);
                }
            });
        }

        private void TsmiAllOffLine_Click(object sender, EventArgs e)
        {
            _ = Task.Run(() =>
            {
                SetContextMenuItemsEnabled(false);
                try
                {

                    var dt = DateTime.Now;
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"点击{_areaName}全部下线按钮"
                    }, dt);

                    var cups = _groupBox.Controls.OfType<CtCup>().Where(c => c.Status == "待机").ToList();
                    int success = 0, fail = 0;
                    var comm = SmartColor.My_AutomaticModule.CupCommManager.Instance.GetCommObject(this);
                    var tasks = new List<Task>();

                    foreach (var cup in cups)
                    {
                        if (!int.TryParse(cup.NO, out int cupNo)) { fail++; continue; }
                        if (comm != null)
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                try
                                {
                                    await comm.SendOffLine(cupNo, 0);
                                    Interlocked.Increment(ref success);
                                }
                                catch
                                {
                                    Interlocked.Increment(ref fail);
                                }
                            }));
                        }
                        else
                        {
                            fail++;
                        }
                    }
                    _ = Task.Run(async () =>
                       {
                           await Task.WhenAll(tasks);

                           if (!SmartColor.My_AutomaticModule.CupCommManager.Instance.ShouldStartComm(this))
                               SmartColor.My_AutomaticModule.CupCommManager.Instance.StopCommThread(this);
                       });


                }
                finally
                {
                    SetContextMenuItemsEnabled(true);
                }
            });
        }

        private void TsmiAllStop_Click(object sender, EventArgs e)
        {
            _ = Task.Run(async () =>
             {
                 SetContextMenuItemsEnabled(false);
                 try
                 {

                     var dt = DateTime.Now;
                     _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                     {
                         [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"点击{_areaName}全部停止按钮"
                     }, dt);

                     var cups = _groupBox.Controls.OfType<CtCup>().ToList();
                     int success = 0, fail = 0;
                     var comm = SmartColor.My_AutomaticModule.CupCommManager.Instance.GetCommObject(this);
                     var tasks = new List<Task>();

                     foreach (var cup in cups)
                     {
                         if (!int.TryParse(cup.NO, out int cupNo)) { fail++; continue; }
                         if (comm != null)
                         {
                             tasks.Add(Task.Run(async () =>
                             {
                                 try
                                 {
                                     await comm.SendStopAsync(cupNo);
                                     Interlocked.Increment(ref success);
                                 }
                                 catch
                                 {
                                     Interlocked.Increment(ref fail);
                                 }
                             }));
                         }
                         else
                         {
                             fail++;
                         }
                     }
                     await Task.WhenAll(tasks);



                 }
                 finally
                 {
                     SetContextMenuItemsEnabled(true);
                 }
             });
        }

        private void TsmiALLPause_Click(object sender, EventArgs e)
        {
            _ = Task.Run(async () =>
             {
                 SetContextMenuItemsEnabled(false);
                 try
                 {

                     var dt = DateTime.Now;
                     _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                     {
                         [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"点击{_areaName}全部暂停按钮"
                     }, dt);

                     var comm = SmartColor.My_AutomaticModule.CupCommManager.Instance.GetCommObject(this);
                     if (comm != null)
                     {
                         await comm.SendPause();
                     }



                 }
                 finally
                 {
                     SetContextMenuItemsEnabled(true);
                 }
             });
        }

        private void TsmiAllHighTemWash_Click(object sender, EventArgs e)
        {
            _ = Task.Run(async () =>
            {
                SetContextMenuItemsEnabled(false);
                try
                {

                    var cups = _groupBox.Controls.OfType<CtCup>().ToList();
                    int success = 0, fail = 0;
                    var comm = SmartColor.My_AutomaticModule.CupCommManager.Instance.GetCommObject(this);
                    var dt = DateTime.Now;
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"点击{_areaName}全部高温洗杯按钮"
                    }, dt);
                    // 只对主副杯模式区域去重
                    List<int> cupNos = GetDistinctMainCups(cups);
                    var tasks = new List<Task>();

                    foreach (var cupNo in cupNos)
                    {
                        if (comm != null)
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                try
                                {

                                    var cupObjTask = await CupCommManager.Instance.FindCupByCupNumAsync(cupNo);
                                    if (cupObjTask != null)
                                    {
                                        if (cupObjTask.Status == "待机")
                                        {
                                            await comm.SendWashAsync(cupNo, My_Tool.CupAuxiliary.HighTempWashCupType);
                                            Interlocked.Increment(ref success);

                                        }
                                        else
                                        {
                                            var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                            if (subInfo.Statues == "待机")
                                            {
                                                await comm.SendWashAsync(subInfo.CupNum, My_Tool.CupAuxiliary.HighTempWashCupType);
                                                Interlocked.Increment(ref success);
                                            }
                                            else
                                                Interlocked.Increment(ref fail);
                                            Interlocked.Increment(ref fail);
                                        }
                                    }
                                    else
                                    {
                                        Interlocked.Increment(ref fail);
                                    }
                                }
                                catch
                                {
                                    Interlocked.Increment(ref fail);
                                }
                            }));
                        }
                        else
                        {
                            fail++;
                        }
                    }
                    await Task.WhenAll(tasks);
                }
                finally
                {
                    SetContextMenuItemsEnabled(true);
                }
            });
        }

        private void TsmiAllWash_Click(object sender, EventArgs e)
        {
            _ = Task.Run(async () =>
            {
                SetContextMenuItemsEnabled(false);
                try
                {

                    var cups = _groupBox.Controls.OfType<CtCup>().ToList();
                    int success = 0, fail = 0;
                    var comm = SmartColor.My_AutomaticModule.CupCommManager.Instance.GetCommObject(this);
                    var dt = DateTime.Now;
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"点击{_areaName}全部洗杯按钮"
                    }, dt);
                    // 只对主副杯模式区域去重
                    List<int> cupNos = GetDistinctMainCups(cups);
                    var tasks = new List<Task>();

                    foreach (var cupNo in cupNos)
                    {
                        if (comm != null)
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                try
                                {
                                    var cupObjTask = await CupCommManager.Instance.FindCupByCupNumAsync(cupNo);
                                    if (cupObjTask != null)
                                    {
                                        if (cupObjTask.Status == "待机")
                                        {
                                            await comm.SendWashAsync(cupNo, My_Tool.CupAuxiliary.StopWashCupType);
                                            Interlocked.Increment(ref success);
                                        }
                                        else
                                        {
                                            var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                                            if (subInfo.Statues == "待机")
                                            {
                                                await comm.SendWashAsync(subInfo.CupNum, My_Tool.CupAuxiliary.StopWashCupType);
                                                Interlocked.Increment(ref success);
                                            }
                                            else
                                                Interlocked.Increment(ref fail);
                                        }
                                    }
                                    else
                                    {
                                        Interlocked.Increment(ref fail);
                                    }

                                }
                                catch
                                {
                                    Interlocked.Increment(ref fail);
                                }
                            }));
                        }
                        else
                        {
                            fail++;
                        }
                    }
                    await Task.WhenAll(tasks);
                }
                finally
                {
                    SetContextMenuItemsEnabled(true);
                }
            });
        }

        private void TsmiALLResume_Click(object sender, EventArgs e)
        {
            _ = Task.Run(async () =>
            {
                SetContextMenuItemsEnabled(false);
                try
                {

                    var dt = DateTime.Now;
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Machine] = $"点击{_areaName}全部恢复按钮"
                    }, dt);

                    var comm = SmartColor.My_AutomaticModule.CupCommManager.Instance.GetCommObject(this);
                    if (comm != null)
                    {
                        await comm.SendResume();
                    }
                }
                finally
                {
                    SetContextMenuItemsEnabled(true);
                }
            });
        }

        private void TsmiAllOpenLid_Click(object sender, EventArgs e)
        {
            _ = Task.Run(async () =>
            {
                SetContextMenuItemsEnabled(false);
                try
                {

                    var cups = _groupBox.Controls.OfType<CtCup>().ToList();
                    int success = 0, fail = 0;
                    var comm = SmartColor.My_AutomaticModule.CupCommManager.Instance.GetCommObject(this);
                    var tasks = new List<Task>();

                    foreach (var cup in cups)
                    {
                        if (!int.TryParse(cup.NO, out int cupNo)) { fail++; continue; }
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                // 注意：cup.CoverStatus 是UI属性，必须在UI线程更新
                                this.Invoke(new Action(() => { cup.CoverStatus = 2; }));

                                await Task.Run(() => SqlServer.Update(CUP_DETAILS.TableName,
                                    new Dictionary<string, object> { [CUP_DETAILS.CoverStatus] = 2 },
                                    $"{CUP_DETAILS.CupNum}={cupNo}"));

                                if (comm != null)
                                    await comm.SyncCoverStatus(cupNo);

                                Interlocked.Increment(ref success);
                            }
                            catch
                            {
                                Interlocked.Increment(ref fail);
                            }
                        }));
                    }
                    await Task.WhenAll(tasks);
                }
                finally
                {
                    SetContextMenuItemsEnabled(true);
                }
            });
        }

        private void TsmiAllCloseLid_Click(object sender, EventArgs e)
        {
            _ = Task.Run(async () =>
            {
                SetContextMenuItemsEnabled(false);
                try
                {

                    var cups = _groupBox.Controls.OfType<CtCup>().ToList();
                    int success = 0, fail = 0;
                    var comm = SmartColor.My_AutomaticModule.CupCommManager.Instance.GetCommObject(this);
                    var tasks = new List<Task>();

                    foreach (var cup in cups)
                    {
                        if (!int.TryParse(cup.NO, out int cupNo)) { fail++; continue; }
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                this.Invoke(new Action(() => { cup.CoverStatus = 1; }));

                                await Task.Run(() => SqlServer.Update(CUP_DETAILS.TableName,
                                    new Dictionary<string, object> { [CUP_DETAILS.CoverStatus] = 1 },
                                    $"{CUP_DETAILS.CupNum}={cupNo}"));

                                if (comm != null)
                                    await comm.SyncCoverStatus(cupNo);

                                Interlocked.Increment(ref success);
                            }
                            catch
                            {
                                Interlocked.Increment(ref fail);
                            }
                        }));
                    }
                    await Task.WhenAll(tasks);
                }
                finally
                {
                    SetContextMenuItemsEnabled(true);
                }
            });
        }

        private void TsmiAllOpen_Click(object sender, EventArgs e)
        {
            _ = Task.Run(async () =>
            {
                SetContextMenuItemsEnabled(false);
                try
                {

                    var cups = _groupBox.Controls.OfType<CtCup>().ToList();
                    int success = 0, fail = 0;
                    var tasks = new List<Task>();

                    foreach (var cup in cups)
                    {
                        if (!int.TryParse(cup.NO, out int cupNo)) { fail++; continue; }
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {

                                var cupObjTask = await CupCommManager.Instance.FindCupByCupNumAsync(cupNo);
                                if (cupObjTask != null)
                                {
                                    if (cupObjTask.Status == "待机")
                                    {
                                        await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueOpenLidAsync(cupNo);
                                        Interlocked.Increment(ref success);

                                    }
                                    else
                                    {
                                        Interlocked.Increment(ref fail);
                                    }
                                }
                                else
                                {
                                    Interlocked.Increment(ref fail);
                                }
                            }
                            catch
                            {
                                Interlocked.Increment(ref fail);
                            }
                        }));
                    }
                    await Task.WhenAll(tasks);
                }
                finally
                {
                    SetContextMenuItemsEnabled(true);
                }
            });
        }

        private void TsmiAllClose_Click(object sender, EventArgs e)
        {
            _ = Task.Run(async () =>
             {
                 SetContextMenuItemsEnabled(false);
                 try
                 {

                     var cups = _groupBox.Controls.OfType<CtCup>().ToList();
                     int success = 0, fail = 0;
                     var tasks = new List<Task>();

                     foreach (var cup in cups)
                     {
                         if (!int.TryParse(cup.NO, out int cupNo)) { fail++; continue; }
                         tasks.Add(Task.Run(async () =>
                         {
                             try
                             {

                                 var cupObjTask = await CupCommManager.Instance.FindCupByCupNumAsync(cupNo);
                                 if (cupObjTask != null)
                                 {
                                     if (cupObjTask.Status == "待机")
                                     {
                                         await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueCloseLidAsync(cupNo);
                                         Interlocked.Increment(ref success);

                                     }
                                     else
                                     {
                                         Interlocked.Increment(ref fail);
                                     }
                                 }
                                 else
                                 {
                                     Interlocked.Increment(ref fail);
                                 }
                             }
                             catch
                             {
                                 Interlocked.Increment(ref fail);
                             }
                         }));
                     }
                     await Task.WhenAll(tasks);

                 }
                 finally
                 {
                     SetContextMenuItemsEnabled(true);
                 }
             });
        }

        /// <summary>
        /// 根据主副杯关系去重，只保留每组主杯
        /// </summary>
        private List<int> GetDistinctMainCups(IEnumerable<CtCup> cups)
        {
            var cupNos = cups.Select(c => int.Parse(c.NO)).ToList();
            var mainCups = new HashSet<int>();
            foreach (var cupNo in cupNos)
            {
                var pair = My_Tool.CupAuxiliary.GetCupPair(cupNo);
                // 只保留主杯，且主杯在本区域
                if (cupNos.Contains(pair.mainCup))
                    mainCups.Add(pair.mainCup);
                else
                    mainCups.Add(cupNo); // 兜底
            }

            return mainCups.ToList();
        }

        // 优化4：添加辅助方法，用于在长时间操作期间禁用上下文菜单项，防止重复点击
        private void SetContextMenuItemsEnabled(bool enabled)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool>(SetContextMenuItemsEnabled), enabled);
                return;
            }
            foreach (ToolStripItem item in contextMenuStrip1.Items)
            {
                item.Enabled = enabled;
            }
        }
    }
}