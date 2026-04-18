using com.google.zxing;
using SmartColor.My_AutomaticModule;
using SmartColor.My_Control;
using SmartColor.My_File;
using SmartColor.My_PLC;
using SmartColor.My_SemiAutoModule;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace SmartColor.My_Form.MachineDebugging
{
    public partial class Debug : Form
    {
        // 标记Debug窗体是否处于活动状态
        public static bool IsDebugFormActive { get; private set; } = false;

        private bool _monitorUseTime = false;
        private int _lastUseTime = -1;
        private string _currentActionName = null;

        // 缓存IO表功能名到字段的映射，提升性能
        private Dictionary<string, MemberInfo> _ioFieldMap;

        private static readonly string[] X905BitToCheckBoxName = new string[32]
        {
            "ChkInPut_Stop",                // 905.00  前光幕
            "ChkInPut_Z_Corotation",        // 905.01  Z轴正限位/反限位
             null,                          // 905.02  流量计脉冲
            "ChkInPut_Tray_Out",            // 905.03  接液盘出
            "ChkInPut_Tray_In",             // 905.04  接液盘回
            "ChkInPut_Syringe",             // 905.05  针筒
            "ChkInPut_Tongs_A",             // 905.06  抓手A
            "ChkInPut_Tongs_B",             // 905.07  抓手B
            "ChkInPut_Cylinder_Up",         // 905.08 上限位
            "ChkInPut_Cylinder_Down",       // 905.09 下限位.
            "ChkInPut_Sunx_A",              // 905.10 左光幕
            "ChkInPut_Sunx_B",              // 905.11 右光幕
            "ChkInPut_Cylinder_Mid",        // 905.12 气缸中间限位
            "ChkInPut_Decompression_Up",    // 905.13 泄压上限位
            "ChkInPut_Decompression_Down",  // 905.14 泄压下限位
            "ChkInPut_Back",                // 905.15 后光幕
            "ChkInPut_X_Reverse",           // 906.00 X轴反限位
            "ChkInPut_X_Corotation",        // 906.01 X轴正限位
            "ChkInPut_X_Ready",             // 906.02 X轴准备好
            "ChkInPut_X_Alarm",             // 906.03 X轴报警
            "BtnOutPut_X_Power",            // 906.04 X轴矢能
            "ChkInPut_Y_Reverse",           // 906.05 Y轴反限位
            "ChkInPut_Y_Corotation",        // 906.06 Y轴正限位
            "ChkInPut_Y_Ready",             // 906.07 Y轴准备好
            "ChkInPut_Y_Alarm",             // 906.08 Y轴报警
            "BtnOutPut_Y_Power",            // 906.09 Y轴矢能
             null,                          // 906.10 称粉原点信号
             null,                          // 906.11 转盘原点信号
             null,                          // 906.12 转盘轴准备好
             null,                          // 906.13 转盘轴报警
             null,                          // 906.14 转盘轴矢能
             null                           // 906.15 
        };

        private static readonly string[] X923BitToCheckBoxName = new string[32]
        {
            "ChkInPut_Block_Out",           //923.00 阻挡出限位
            "ChkInPut_Block_In",            //923.01 阻挡回限位
            "ChkInPut_Slow_Cylinder_Mid",   //923.02 气缸慢速中限位
            "ChkInPut_Cylinder_Block",      //923.03 气缸阻挡限位
            "ChkInPut_SupportCover",        //923.04 撑盖气缸开到位
            null,                           //923.05 气压表信号/液位开关
            null,                           //923.06 气缸慢速中限位2
            null,                           //923.07 气缸慢速中限位3
            "ChkInPut_Apocenosis_Up",       //923.08 转子缸急停
            null,                           //923.09
            null,                           //923.10
            null,                           //923.11 开料升降气缸上升到位
            null,                           //923.12 开料升降气缸下降到位
            null,                           //923.13 开料摆臂伸到位
            null,                           //923.14 开料摆臂缩到位
            null,                           //923.15 洗瓶夹瓶伸到位
            null,                           //924.00 洗瓶夹瓶缩到位
            null,                           //924.01 洗瓶翻转上到位
            null,                           //924.02 洗瓶翻转下到位
            null,                           //924.03
            null,                           //924.04 热水箱浮球
            null,                           //924.05
            null,                           //924.06
            null,                           //924.07
            null,                           //924.08
            null,                           //924.09
            null,                           //924.10
            null,                           //924.11
            null,                           //924.12
            null,                           //924.13
            null,                           //924.14
            null                            //924.15
        };

        private static readonly string[] Y907BitToButtonName = new string[32]
        {
            null,                       // 907.00
            null,                       // 907.01
            null,                       // 907.02
            null,                       // 907.03  
            "BtnOutPut_Blender",        // 907.04  搅拌停
            "BtnOutPut_Waste",          // 907.05  抽废液
            "BtnOutPut_Buzzer",         // 907.06  报警
            "BtnOutPut_Water",          // 907.07  加水
            "BtnOutPut_Tray",           // 907.08 接液盘
            "BtnOutPut_TongsOn",        // 907.09 抓手合
             null,                      // 907.10 气缸下
            "BtnOutPut_Cylinder_Up",    // 907.11 气缸上（无按钮）
            null,                       // 907.12 抓手开（无按钮）
            "BtnOutPut_Decompression",  // 907.13 泄压
            "BtnOutPut_Block_Out",      // 907.14 阻挡出
            null,                       // 907.15 阻挡回（无按钮）
            null,                       // 908.00 
            null,                       // 908.01 
            null,                       // 908.02
            null,                       // 908.03 
            null,                       // 908.04 
            null,                       // 908.05 
            null,                       // 908.06 
            null,                       // 908.07 
            null,                       // 908.08 
            null,                       // 908.09 
            null,                       // 908.10 
            null,                       // 908.11 
            null,                       // 908.12 
            null,                       // 908.13 
            null,                       // 908.14 
            null,                       // 908.15 
        };

        private static readonly string[] Y925BitToButtonName = new string[32]
        {
            "BtnOutPut_Slow",           // 925.00 气缸慢下阀
            "BtnOutPut_Wash_In",        // 925.01 洗针进水阀
            "BtnOutPut_Wash_Out",       // 925.02 洗针排水阀
            "BtnOutPut_Wash_Blow",      // 925.03 洗针吹气阀
            null,                       // 925.04 加溶解剂
            null,                       // 925.05               
            null,                       // 925.06      
            null,                       // 925.07
            null,                       // 925.08
            null,                       // 925.09
            null,                       // 925.10
            null,                       // 925.11
            null,                       // 925.12
            null,                       // 925.13
            null,                       // 925.14
            null,                       // 925.15
            null,                       // 926.00 
            null,                       // 926.01 
            null,                       // 926.02 
            null,                       // 926.03 
            null,                       // 926.04 
            null,                       // 926.05
            null,                       // 926.06
            null,                       // 926.07
            null,                       // 926.08
            null,                       // 926.09
            null,                       // 926.10
            null,                       // 926.11
            null,                       // 926.12
            null,                       // 926.13
            null,                       // 926.14
            null                        // 926.15
        };

        public Debug()
        {
            InitializeComponent();
            // 设置定时器间隔（如500ms）
            Tmr.Interval = 100;
            Tmr.Tick += Tmr_Tick;

            foreach (Control ctrl in this.grp_out.Controls)
            {
                if (ctrl is Button btn && btn.Name.StartsWith("BtnOutPut_"))
                {
                    btn.Click += BtnOutPut_Click;
                }
            }
        }

        /// <summary>
        /// 窗体加载时，切换PLC为Debug模式，并构建IO映射表
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            IsDebugFormActive = true;
            // 切换PLC读取模式为Debug，确保能获取全部输入输出数据
            My_ConPar.Object.CurrentPLC?.SetReadMode(PLC.PlcReadMode.Debug);
            // 构建IO表功能名到字段的映射，提升后续查找效率
            BuildIoFieldMap();
            Tmr.Start();
        }

        /// <summary>
        /// 窗体关闭时，恢复PLC为Normal模式，释放资源
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            IsDebugFormActive = false;
            My_ConPar.Object.CurrentPLC?.SetReadMode(PLC.PlcReadMode.Normal);
            Tmr.Stop();
            base.OnFormClosed(e);
        }

        /// <summary>
        /// 构建IO表功能名到字段的映射（只需构建一次，提升性能）
        /// </summary>
        private void BuildIoFieldMap()
        {
            _ioFieldMap = new Dictionary<string, MemberInfo>(StringComparer.OrdinalIgnoreCase);
            var io = My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.PLC.IO;
            if (io == null) return;
            // 遍历所有InPut_开头的属性，提取功能名
            foreach (var prop in io.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (prop.Name.StartsWith("InPut_"))
                {
                    string funcName = prop.Name.Substring("InPut_".Length);
                    _ioFieldMap[funcName] = prop;
                }
            }
        }

        /// <summary>
        /// 定时刷新PLC数据到界面
        /// </summary>
        private void Tmr_Tick(object sender, EventArgs e)
        {
            // 获取PLC对象
            var plc = My_ConPar.Object.CurrentPLC;
            if (plc == null) return;

            // 反射获取PLC的_Receive字段（包含所有接收数据）
            var receiveField = typeof(PLC).GetField("_Receive", BindingFlags.NonPublic | BindingFlags.Instance);
            var receive = receiveField?.GetValue(plc) as PLC_Receive;
            if (receive == null) return;

            // 天平值显示（单位：g，保留两位小数）
            if (receive.BalanceData?.Value != null)
            {
                int roundDigits = My_Tool.BalanceStableReading.RetainDecimals();
                double value = Convert.ToInt32(receive.BalanceData.Value) / 1000.0;

                LabBalanceValue.Text = (Math.Round(value, roundDigits)).ToString();

            }

            if (My_ConPar.Hardware.UseCylinderPositioningEncoder == 1)
            {
                // 气缸定位编码器显示
                TxtCylinderEncoder.Text = (Math.Round(Convert.ToDouble(receive.CylinderEncoderPosition?.Value?.ToString() ?? "0") / 100, 2)).ToString();
            }



            // 轴坐标显示
            TxtRPosX.Text = receive.X_CurrentPosition?.Value?.ToString() ?? "";
            TxtRPosY.Text = receive.Y_CurrentPosition?.Value?.ToString() ?? "";
            TxtCPosZ.Text = receive.Z_CurrentPosition?.Value?.ToString() ?? "";

            // 轴速度显示
            TxtCSpeedX.Text = receive.X_CurrentSpeed?.Value?.ToString() ?? "";
            TxtCSpeedY.Text = receive.Y_CurrentSpeed?.Value?.ToString() ?? "";
            TxtCSpeedZ.Text = receive.Z_CurrentSpeed?.Value?.ToString() ?? "";

            // 刷新X0-X17输入信号
            if (receive.InputX0_X17?.Value != null)
                RefreshInputPack(Convert.ToInt32(receive.InputX0_X17.Value), X905BitToCheckBoxName);
            // 刷新X20-X57输入信号
            if (receive.InputX20_X57?.Value != null)
                RefreshInputPack(Convert.ToInt32(receive.InputX20_X57.Value), X923BitToCheckBoxName);

            // 刷新输出点字体颜色
            if (receive.OutputY0_Y17?.Value != null)
                RefreshOutputPack(Convert.ToInt32(receive.OutputY0_Y17.Value), Y907BitToButtonName);
            if (receive.OutputY20_Y57?.Value != null)
                RefreshOutputPack(Convert.ToInt32(receive.OutputY20_Y57.Value), Y925BitToButtonName);

            // UseTime监控逻辑
            if (_monitorUseTime && receive.UseTime != null && receive.UseTime.Value != null)
            {
                int useTime = Convert.ToInt32(receive.UseTime.Value);
                if (useTime != 0 && useTime != -1 && useTime != _lastUseTime)
                {
                    _monitorUseTime = false; // 只触发一次
                    _lastUseTime = useTime;
                    MessageEventManager.Instance.RequestShowBalloonTip($"{_currentActionName}动作完成，用时 {useTime} ms");
                }
            }
        }


        private void RefreshInputPack(int packValue, string[] bitToName)
        {
            for (int i = 0; i < bitToName.Length; i++)
            {
                string chkName = bitToName[i];
                if (string.IsNullOrEmpty(chkName)) continue;
                var chk = this.grp_in.Controls.Find(chkName, false).FirstOrDefault() as CheckBox;
                if (chk != null)
                    chk.Checked = (packValue & (1 << i)) != 0;
                else
                {
                    if (chkName == BtnOutPut_X_Power.Name)
                    {
                        // 特例处理X轴使能按钮
                        var btn = this.grp_out.Controls.Find(chkName, false).FirstOrDefault() as Button;
                        if (btn != null)
                            btn.ForeColor = (packValue & (1 << i)) != 0 ? Color.Red : Color.Black;
                    }
                    else if (chkName == BtnOutPut_Y_Power.Name)
                    {
                        // 特例处理Y轴使能按钮
                        var btn = this.grp_out.Controls.Find(chkName, false).FirstOrDefault() as Button;
                        if (btn != null)
                            btn.ForeColor = (packValue & (1 << i)) != 0 ? Color.Red : Color.Black;
                    }
                }
            }
        }

        private void RefreshOutputPack(int packValue, string[] bitToName)
        {
            for (int i = 0; i < bitToName.Length; i++)
            {
                string btnName = bitToName[i];
                if (string.IsNullOrEmpty(btnName)) continue;
                var btn = this.grp_out.Controls.Find(btnName, false).FirstOrDefault() as Button;
                if (btn != null)
                    btn.ForeColor = (packValue & (1 << i)) != 0 ? Color.Red : Color.Black;
            }
        }

        private async void BtnHome_Click(object sender, EventArgs e)
        {
            try
            {
                var result = await SemiAutoHelperFactory.Current.HomeAsync();
                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("回原点成功！");
                        break;

                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("回原点询问", result.Message, btn =>
                         {
                             if (btn == "重试")
                                 BtnHome_Click(sender, e);
                         },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("回原点异常！", result.Exception);
                        break;
                }
            }
            catch (Exception ex)
            {
                My_File.LocalTranslator.ShowMessage("回原点异常：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            //预留
            My_File.LocalTranslator.ShowMessage("功能预留，待开发！", "停止", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void BtnMove_Click(object sender, EventArgs e)
        {
            if (RdoX.Checked)
            {
                var result = await SemiAutoHelperFactory.Current.RelativeMoveAsync(0, Convert.ToInt32(TxtPulseX.Text),
                     Convert.ToInt32(TxtHSpeedX.Text),
                     Convert.ToInt32(TxtUpSpeedX.Text));
                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("X轴移动成功！");
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("X轴移动异常！", result.Exception);
                        break;
                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("X轴移动询问", result.Message, btn =>
                        {
                            if (btn == "重试")
                                BtnHome_Click(sender, e);
                        },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                }


            }
            else if (RdoY.Checked)
            {
                var result = await SemiAutoHelperFactory.Current.RelativeMoveAsync(1, Convert.ToInt32(TxtPulseY.Text),
                    Convert.ToInt32(TxtHSpeedY.Text),
                    Convert.ToInt32(TxtUpSpeedY.Text));
                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("Y轴移动成功！");
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("Y轴移动异常！", result.Exception);
                        break;
                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("Y轴移动询问", result.Message, btn =>
                        {
                            if (btn == "重试")
                                BtnHome_Click(sender, e);
                        },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                }
            }
            else if (RdoZ.Checked)
            {

                var result = await SemiAutoHelperFactory.Current.RelativeMoveAsync(2, Convert.ToInt32(TxtPulseZ.Text),
                    Convert.ToInt32(TxtHSpeedZ.Text),
                    Convert.ToInt32(TxtUpSpeedZ.Text));
                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("Z轴移动成功！");
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("Z轴移动异常！", result.Exception);
                        break;
                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("Z轴移动询问", result.Message, btn =>
                        {
                            if (btn == "重试")
                                BtnHome_Click(sender, e);
                        },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                }
            }
            else
            {
                //预留转盘
            }
        }

        private void Btn_Reset_Click(object sender, EventArgs e)
        {
            //预留
            My_File.LocalTranslator.ShowMessage("功能预留，待开发！", "天平清零", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        // 通用输出按钮点击事件
        private void BtnOutPut_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            // 提取功能名（如Cylinder_Down、TongsOn等）
            string funcName = btn.Name.Substring("BtnOutPut_".Length);

            // 判断当前字体颜色（红色=输出点已激活，黑色=未激活）
            bool isOn = btn.ForeColor == Color.Red;

            // 获取PLC对象
            var plc = My_ConPar.Object.CurrentPLC as PLC;
            if (plc == null) return;

            // 枚举名与功能名映射（需与PLC.ManualOperation一致）
            PLC.ManualOperation opOn = PLC.ManualOperation.None;
            PLC.ManualOperation opOff = PLC.ManualOperation.None;

            switch (funcName)
            {
                //X轴使能
                case "X_Power":
                    opOn = PLC.ManualOperation.XEnableOn;
                    opOff = PLC.ManualOperation.XEnableOff;
                    break;

                //Y轴使能
                case "Y_Power":
                    opOn = PLC.ManualOperation.YEnableOn;
                    opOff = PLC.ManualOperation.YEnableOff;
                    break;

                //气缸下
                case "Cylinder_Up":
                    opOn = PLC.ManualOperation.CylinderUp;
                    opOff = PLC.ManualOperation.CylinderDown;
                    _currentActionName = isOn ? "气缸下" : "气缸上";
                    break;

                //搅拌停
                case "Blender":
                    opOn = PLC.ManualOperation.BlenderOn;
                    opOff = PLC.ManualOperation.BlenderOff;
                    break;

                //接液盘
                case "Tray":
                    opOn = PLC.ManualOperation.TrayOut;
                    opOff = PLC.ManualOperation.TrayIn;
                    _currentActionName = isOn ? "接液盘收回" : "接液盘伸出";
                    break;

                //抓手合
                case "TongsOn":
                    opOn = PLC.ManualOperation.TongsClose;
                    opOff = PLC.ManualOperation.TongsOpen;
                    _currentActionName = isOn ? "抓手打开" : "抓手关闭";
                    break;

                //阻挡出
                case "Block_Out":
                    opOn = PLC.ManualOperation.BlockOut;
                    opOff = PLC.ManualOperation.BlockIn;
                    _currentActionName = isOn ? "阻挡回" : "阻挡出";
                    break;

                //泄压下
                case "Decompression":
                    opOn = PLC.ManualOperation.DecompressionDown;
                    opOff = PLC.ManualOperation.DecompressionUp;
                    _currentActionName = isOn ? "泄压上" : "泄压下";
                    break;

                //气缸慢速中
                case "Slow":
                    opOn = PLC.ManualOperation.CylinderSlowMid;
                    opOff = PLC.ManualOperation.None;
                    _currentActionName = isOn ? "" : "气缸到慢速中";
                    break;

                //报警
                case "Buzzer":
                    opOn = PLC.ManualOperation.BuzzerOn;
                    opOff = PLC.ManualOperation.BuzzerOff;
                    break;

                //加水
                case "Water":
                    opOn = PLC.ManualOperation.WaterOn;
                    opOff = PLC.ManualOperation.WaterOff;
                    break;

                //抽废液
                case "Waste":
                    opOn = PLC.ManualOperation.WasteOn;
                    opOff = PLC.ManualOperation.WasteOff;
                    break;

                //洗针吹气阀
                case "Wash_Blow":
                    opOn = PLC.ManualOperation.WashBlowValveOn;
                    opOff = PLC.ManualOperation.WashBlowValveOff;
                    break;

                //轴报警复位
                case "ResetX":
                    opOn = PLC.ManualOperation.XAlarmReset;
                    opOff = PLC.ManualOperation.None;
                    break;

                //轴报警复位
                case "ResetY":
                    opOn = PLC.ManualOperation.YAlarmReset;
                    opOff = PLC.ManualOperation.None;
                    break;

                //气缸到阻挡位
                case "Block_Cylinder":
                    opOn = PLC.ManualOperation.CylinderBlock;
                    opOff = PLC.ManualOperation.None;
                    _currentActionName = isOn ? "" : "气缸到阻挡位";
                    break;

                //洗针进水阀
                case "Wash_In":
                    opOn = PLC.ManualOperation.WashInValveOn;
                    opOff = PLC.ManualOperation.WashInValveOff;
                    break;
                //洗针排水阀
                case "Wash_Out":
                    opOn = PLC.ManualOperation.WashOutValveOn;
                    opOff = PLC.ManualOperation.WashOutValveOff;
                    break;

                // 可继续扩展其它功能名
                default:
                    return;
            }

            // 根据当前字体颜色决定下发命令
            if (isOn)
                plc.EnqueueManualOperation(opOff);
            else
                plc.EnqueueManualOperation(opOn);

            // 启动UseTime监控
            Thread.Sleep(Tmr.Interval);
            _monitorUseTime = true;
            _lastUseTime = -1;
        }


        private async void BtnStartMove_Click(object sender, EventArgs e)
        {
            if (RdoBottle.Checked)
            {
                //母液瓶移动
                if (!int.TryParse(TxtNum.Text, out int num))
                {
                    My_File.LocalTranslator.ShowMessage("请输入正确的母液瓶号！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var bottleCTR = await My_Tool.AreaCoordinateFinder.TryGetBottleCoordinateAsync(num);
                if (!bottleCTR.found)
                {
                    My_File.LocalTranslator.ShowMessage("未找到母液瓶坐标，请先设置母液瓶坐标！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = await SemiAutoHelperFactory.Current.MoveToBottleAsync(num, bottleCTR.x, bottleCTR.y, 0, 0);

                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("母液瓶移动成功！");
                        break;

                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("母液瓶移动询问", result.Message, btn =>
                        {
                            if (btn == "重试")
                                BtnHome_Click(sender, e);
                        },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("母液瓶移动异常！", result.Exception);
                        break;

                }
            }
            else if (RdoCup.Checked)
            {
                //预留杯子移动
                if (!int.TryParse(TxtNum.Text, out int num))
                {
                    My_File.LocalTranslator.ShowMessage("请输入正确的配液杯号！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var cupCTR = await My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(num, false);
                if (!cupCTR.found)
                {
                    My_File.LocalTranslator.ShowMessage("未找到配液杯坐标，请先设置母液瓶坐标！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var result = await SemiAutoHelperFactory.Current.MoveToCupAsync(num, 0, cupCTR.x, cupCTR.y, 0, 0);

                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("配液杯移动成功！");
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("配液杯移动异常！", result.Exception);
                        break;
                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("配液杯移动询问", result.Message, btn =>
                        {
                            if (btn == "重试")
                                BtnHome_Click(sender, e);
                        },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                }

            }
            else if (RdoCupLid.Checked)
            {
                //杯盖移动
                if (!int.TryParse(TxtNum.Text, out int num))
                {
                    My_File.LocalTranslator.ShowMessage("请输入正确的配液杯盖位！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var cupCTR = await My_Tool.AreaCoordinateFinder.TryGetCupOrLidCoordinateAsync(num, true);
                if (!cupCTR.found)
                {
                    My_File.LocalTranslator.ShowMessage("未找到配液杯盖坐标，请先设置母液瓶坐标！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = await SemiAutoHelperFactory.Current.MoveToCupLidAsync(num, cupCTR.x, cupCTR.y, 0, 0);

                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("配液杯盖移动成功！");
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("配液杯盖移动异常！", result.Exception);
                        break;
                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("配液杯盖移动询问", result.Message, btn =>
                        {
                            if (btn == "重试")
                                BtnHome_Click(sender, e);
                        },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                }
            }
            else if (RdoBalance.Checked)
            {
                var balanceCTR = await My_Tool.AreaCoordinateFinder.TryGetBalanceCoordinateAsync();
                if (!balanceCTR.found)
                {
                    My_File.LocalTranslator.ShowMessage("未找到天平坐标，请先设置天平坐标！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var result = await SemiAutoHelperFactory.Current.MoveToBalanceAsync(balanceCTR.x, balanceCTR.y, 0);

                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("天平移动成功！");
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("天平移动异常！", result.Exception);
                        break;
                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("天平移动询问", result.Message, btn =>
                        {
                            if (btn == "重试")
                                BtnHome_Click(sender, e);
                        },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                }

            }
            else if (RdoDecompression.Checked)
            {
                //预留泄压移动
                if (!int.TryParse(TxtNum.Text, out int num))
                {
                    My_File.LocalTranslator.ShowMessage("请输入正确的泄压位！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var decompressionCTR = await My_Tool.AreaCoordinateFinder.TryGetDecompressionCoordinateAsync(num);
                if (!decompressionCTR.found)
                {
                    My_File.LocalTranslator.ShowMessage("未找到泄压位坐标，请先设置泄压位坐标！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = await SemiAutoHelperFactory.Current.MoveToDecompressionAsync(num, decompressionCTR.x, decompressionCTR.y, 0);

                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("泄压移动成功！");
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("泄压移动异常！", result.Exception);
                        break;
                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("泄压移动询问", result.Message, btn =>
                        {
                            if (btn == "重试")
                                BtnHome_Click(sender, e);
                        },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                }

            }
            else if (RdoDryCloth.Checked)
            {
                // 备布区移动
                if (!int.TryParse(TxtNum.Text, out int num))
                {
                    My_File.LocalTranslator.ShowMessage("请输入正确的备布位！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var prepareCTR = await My_Tool.AreaCoordinateFinder.TryGetPrepareClothCoordinateAsync(num);
                if (!prepareCTR.found)
                {
                    My_File.LocalTranslator.ShowMessage($"未找到{num}号备布位坐标，请先设置备布位坐标！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = await SemiAutoHelperFactory.Current.MoveToPrepareClothAsync(num, 0, prepareCTR.x, prepareCTR.y, 0);

                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("备布区移动成功！");
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("备布区移动异常！", result.Exception);
                        break;
                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("备布区移动询问", result.Message, btn =>
                        {
                            if (btn == "重试")
                                BtnHome_Click(sender, e);
                        },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                }

            }
            else if (RdoWetCloth.Checked)
            {
                //出布区移动 
                if (!int.TryParse(TxtNum.Text, out int num))
                {
                    My_File.LocalTranslator.ShowMessage("请输入正确的出布位！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var outCTR = await My_Tool.AreaCoordinateFinder.TryGetOutClothCoordinateAsync(num);
                if (!outCTR.found)
                {
                    My_File.LocalTranslator.ShowMessage($"未找到{num}号出布位坐标，请先设置备布位坐标！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = await SemiAutoHelperFactory.Current.MoveToOutClothAsync(num, outCTR.x, outCTR.y, 0);

                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("出布区移动成功！");
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("出布区移动异常", result.Exception);
                        break;
                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("出布区移动询问", result.Message, btn =>
                        {
                            if (btn == "重试")
                                BtnHome_Click(sender, e);
                        },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                }


            }
            else if (RdoDryClamp.Checked)
            {
                //干布夹子移动
                var dryCTR = await My_Tool.AreaCoordinateFinder.TryGetDryClampCoordinateAsync();
                if (!dryCTR.found)
                {
                    My_File.LocalTranslator.ShowMessage($"未找到干布夹具坐标，请先设置干布夹具坐标！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = await SemiAutoHelperFactory.Current.MoveToDryClampAsync(0, dryCTR.x, dryCTR.y, 0, 0);


                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("干布夹子移动成功！");
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("干布夹子移动异常！", result.Exception);
                        break;
                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("干布夹子移动询问", result.Message, btn =>
                        {
                            if (btn == "重试")
                                BtnHome_Click(sender, e);
                        },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                }

            }
            else if (RdoWetClamp.Checked)
            {
                //湿布夹子移动
                var wetCTR = await My_Tool.AreaCoordinateFinder.TryGetWetClampCoordinateAsync();
                if (!wetCTR.found)
                {
                    My_File.LocalTranslator.ShowMessage($"未找到湿布夹具坐标，请先设置湿布夹具坐标！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var result = await SemiAutoHelperFactory.Current.MoveToWetClampAsync(0, wetCTR.x, wetCTR.y, 0, 0);

                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("湿布夹具移动成功！");
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("湿布夹具移动异常！", result.Exception);
                        break;
                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("湿布夹具移动询问", result.Message, btn =>
                        {
                            if (btn == "重试")
                                BtnHome_Click(sender, e);
                        },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                }

            }
            else if (RdoWash.Checked)
            {
                //洗针移动
                var washCTR = await My_Tool.AreaCoordinateFinder.TryGetWashCoordinateAsync();
                if (!washCTR.found)
                {
                    My_File.LocalTranslator.ShowMessage($"未找到洗针筒模块坐标，请先设置洗针筒模块坐标！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var result = await SemiAutoHelperFactory.Current.MoveToWashAsync(washCTR.x, washCTR.y, 0);

                switch (result.Level)
                {
                    case SemiAutoResultCode.Success:
                        My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("洗针筒模块移动成功！");
                        break;
                    case SemiAutoResultCode.Exception:
                        Logger.Error("洗针筒模块移动异常！", result.Exception);
                        break;
                    case SemiAutoResultCode.NeedInteraction:
                        My_Tool.MessageEventManager.Instance.RequestShowMessage("洗针筒模块移动询问", result.Message, btn =>
                        {
                            if (btn == "重试")
                                BtnHome_Click(sender, e);
                        },
                        new[] { "重试", "退出" },
                        "重试"
                        );
                        break;
                }
            }
            else
            {
                My_File.LocalTranslator.ShowMessage("请选择要移动的区域类型！", "定点移动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            // 只允许在配液杯或杯盖模式下批量生成
            if (!RdoCup.Checked && !RdoCupLid.Checked)
            {
                My_File.LocalTranslator.ShowMessage("无需生成坐标，仅需写入该区域首位坐标即可", "生成坐标", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 检查编号输入
            if (!int.TryParse(TxtNum.Text, out int num))
            {
                My_File.LocalTranslator.ShowMessage("请输入正确的编号！", "生成坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 查找当前编号所属的区域对象
            var areaObj = FindCupAreaByNum(num);
            if (areaObj == null)
            {
                My_File.LocalTranslator.ShowMessage("未找到该编号所属的杯子区域！", "生成坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int start = GetIntProperty(areaObj, "StartPosition");
            if (num != start)
            {
                My_File.LocalTranslator.ShowMessage("请在该区域首位编号生成坐标！", "生成坐标", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 获取布局类型和节名（section），与BtnWrite_Click保持一致
            var layoutType = SmartColor.My_ConPar.Object.CurrentLayout as Type;
            if (layoutType == null)
            {
                My_File.LocalTranslator.ShowMessage("未找到布局类型！", "生成坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 获取当前区域的属性名作为section
            string section = null;
            var areaProps = layoutType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var prop in areaProps)
            {
                if (object.ReferenceEquals(prop.GetValue(null), areaObj))
                {
                    section = prop.Name.Replace("_", "");
                    break;
                }
            }
            if (string.IsNullOrEmpty(section))
            {
                My_File.LocalTranslator.ShowMessage("未找到该区域配置节名！", "生成坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string iniPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Config", "Layout.ini");

            // 生成并写入
            bool success = false;
            if (RdoCup.Checked)
            {
                if (areaObj is My_ConPar.Area.Drop.Drop)
                {
                    My_File.LocalTranslator.ShowMessage("当前杯号所属机台不需要设置各个杯子独立坐标！", "生成坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                success = GenerateCupAreaCoordinates(areaObj, section, iniPath);
                if (!success)
                {
                    My_File.LocalTranslator.ShowMessage("生成配液杯坐标失败，该类型尚未开发！", "生成坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                AreaCoordinateFinder.BuildAllCoordinateCache(SmartColor.My_ConPar.Object.CurrentLayout as Type);
            }
            else // RdoCupLid.Checked
            {
                if (areaObj is My_ConPar.Area.Drop.Drop ||
                    areaObj is My_ConPar.Area.RotorCylinder.RC_4 ||
                    areaObj is My_ConPar.Area.RotorCylinder.RC_10)
                {
                    My_File.LocalTranslator.ShowMessage("当前杯号所属机台不需要设置杯盖坐标！", "生成坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                success = GenerateLidAreaCoordinates(areaObj, section, iniPath);
                if (!success)
                {
                    My_File.LocalTranslator.ShowMessage("生成杯盖坐标失败，该类型尚未开发或用户取消！", "生成坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                AreaCoordinateFinder.BuildAllCoordinateCache(SmartColor.My_ConPar.Object.CurrentLayout as Type);
            }


            My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("批量生成坐标完成！");
        }

        /// <summary>
        /// 查找杯号属于哪个杯子区域（指出Drop、FlipCylinder、RotorCylinder等）
        /// </summary>
        /// <param name="num">杯号</param>
        /// <returns>区域对象，未找到返回null</returns>
        private object FindCupAreaByNum(int num)
        {
            var layoutType = SmartColor.My_ConPar.Object.CurrentLayout as Type;
            if (layoutType == null) return null;
            var areaProps = layoutType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var prop in areaProps)
            {
                var areaObj = prop.GetValue(null) as SmartColor.My_ConPar.Area.Base;
                if (areaObj == null) continue;
                // 只查杯子相关区域
                if (!(areaObj is SmartColor.My_ConPar.Area.Drop.Drop
                    || areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_4
                    || areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_6
                    || areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_12
                    || areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_16
                    || areaObj is SmartColor.My_ConPar.Area.RotorCylinder.RC_4
                    || areaObj is SmartColor.My_ConPar.Area.RotorCylinder.RC_10))
                    continue;

                int start = GetIntProperty(areaObj, "StartPosition");
                int row = GetIntProperty(areaObj, "Row");
                int col = GetIntProperty(areaObj, "Column");
                if (row > 0 && col > 0)
                {
                    int end = start + row * col - 1;
                    if (num >= start && num <= end)
                        return areaObj;
                }
            }
            return null;
        }

        /// <summary>
        /// 生成杯子区域所有杯的坐标（支持FlipCylinder、RotorCylinder），并写入INI配置文件
        /// </summary>
        /// <param name="areaObj">区域对象</param>
        /// <param name="section">INI文件中的节名（区域名）</param>
        /// <param name="iniPath">INI文件路径</param>
        /// <returns>生成成功返回true，否则false</returns>
        private bool GenerateCupAreaCoordinates(object areaObj, string section, string iniPath)
        {
            if (areaObj == null) return false;
            int start = GetIntProperty(areaObj, "StartPosition");
            int row = GetIntProperty(areaObj, "Row");
            int col = GetIntProperty(areaObj, "Column");
            if (row <= 0 || col <= 0) return false;
            int total = row * col;

            int x1 = GetIntProperty(areaObj, $"CupCX_1");
            int y1 = GetIntProperty(areaObj, $"CupCY_1");

            // 根据区域类型，采用不同的坐标生成方式
            string typeName = areaObj.GetType().Name;
            switch (typeName)
            {
                case "RC_4":
                case "RC_10":
                    {
                        // 旋转缸类型，按行列生成
                        int ix = 11000, iy = 12000;
                        for (int i = 0; i < total; i++)
                        {
                            int curNum = i + 1;
                            int r = i / col, c = i % col;
                            int x = My_ConPar.Hardware.OriginPosition == 0 ? x1 - c * ix : x1 + c * ix;
                            int y = y1 + r * iy;
                            SetIntProperty(areaObj, $"CupCX_{curNum}", x);
                            SetIntProperty(areaObj, $"CupCY_{curNum}", y);
                            // 写入INI文件
                            ConfigHelper.WriteIniValue(section, $"CupCX_{curNum}", x.ToString(), iniPath);
                            ConfigHelper.WriteIniValue(section, $"CupCY_{curNum}", y.ToString(), iniPath);
                        }
                        break;
                    }
                case "FC_4":
                    {
                        // 4杯翻转缸，Y方向间隔特殊
                        int[] yInterval = { 0, 12600, 22900, 12600 };
                        int y = y1;
                        for (int i = 0; i < total; i++)
                        {
                            int curNum = i + 1;
                            SetIntProperty(areaObj, $"CupCX_{curNum}", x1);
                            SetIntProperty(areaObj, $"CupCY_{curNum}", y);
                            ConfigHelper.WriteIniValue(section, $"CupCX_{curNum}", x1.ToString(), iniPath);
                            ConfigHelper.WriteIniValue(section, $"CupCY_{curNum}", y.ToString(), iniPath);
                            if (i < yInterval.Length - 1)
                                y += yInterval[i + 1];
                        }
                        break;
                    }
                case "FC_6":
                    {
                        // 6杯翻转缸，Y方向间隔特殊
                        int[] yInterval = { 0, 8000, 15650, 8000, 15650, 8000 };
                        int y = y1;
                        for (int i = 0; i < total; i++)
                        {
                            int curNum = i + 1;
                            SetIntProperty(areaObj, $"CupCX_{curNum}", x1);
                            SetIntProperty(areaObj, $"CupCY_{curNum}", y);
                            ConfigHelper.WriteIniValue(section, $"CupCX_{curNum}", x1.ToString(), iniPath);
                            ConfigHelper.WriteIniValue(section, $"CupCY_{curNum}", y.ToString(), iniPath);
                            if (i < yInterval.Length - 1)
                                y += yInterval[i + 1];
                        }
                        break;
                    }
                case "FC_12":
                    {
                        // 12杯翻转缸，X方向等距，Y方向分段
                        int ix = 10500;
                        int[] yIntervals = { 8000, 15650, 8000, 15650, 8000 };
                        for (int i = 0; i < total; i++)
                        {
                            int curNum = i + 1;
                            int colIdx = i % col, rowIdx = i / col;
                            int x = My_ConPar.Hardware.OriginPosition == 0 ? x1 - colIdx * ix : x1 + colIdx * ix;
                            int y = y1;
                            for (int r = 0; r < rowIdx; r++)
                                y += yIntervals[r];
                            SetIntProperty(areaObj, $"CupCX_{curNum}", x);
                            SetIntProperty(areaObj, $"CupCY_{curNum}", y);
                            ConfigHelper.WriteIniValue(section, $"CupCX_{curNum}", x.ToString(), iniPath);
                            ConfigHelper.WriteIniValue(section, $"CupCY_{curNum}", y.ToString(), iniPath);
                        }
                        break;
                    }
                default:
                    // 其它类型未实现
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 生成杯盖区域所有盖子的坐标（支持FlipCylinder），并写入INI配置文件
        /// </summary>
        /// <param name="areaObj">区域对象</param>
        /// <param name="section">INI文件中的节名（区域名）</param>
        /// <param name="iniPath">INI文件路径</param>
        /// <returns>生成成功返回true，否则false</returns>
        private bool GenerateLidAreaCoordinates(object areaObj, string section, string iniPath)
        {
            if (areaObj == null) return false;
            int start = GetIntProperty(areaObj, "StartPosition");
            int row = GetIntProperty(areaObj, "Row");
            int col = GetIntProperty(areaObj, "Column");
            if (row <= 0 || col <= 0) return false;
            int total = row * col;

            int x1 = GetIntProperty(areaObj, $"LidCX_1");
            int y1 = GetIntProperty(areaObj, $"LidCY_1");

            // 根据区域类型，采用不同的坐标生成方式
            string typeName = areaObj.GetType().Name;
            switch (typeName)
            {
                case "FC_4":
                    {
                        // 4盖翻转缸，按特殊规则生成
                        int x = 0, y = 0;
                        for (int i = 0; i < total; i++)
                        {
                            int curNum = i + 1;
                            if (i == 0) continue;
                            else if (i == 1)
                            {
                                x = x1 + 10500;
                                y = y1;
                            }
                            else if (i == 2)
                            {
                                x = x1 + 10500;
                                y = y1 + 35750;
                            }
                            else if (i == 3)
                            {
                                x = x1;
                                y = y1 + 35750;
                            }
                            SetIntProperty(areaObj, $"LidCX_{curNum}", x);
                            SetIntProperty(areaObj, $"LidCY_{curNum}", y);
                            ConfigHelper.WriteIniValue(section, $"LidCX_{curNum}", x.ToString(), iniPath);
                            ConfigHelper.WriteIniValue(section, $"LidCY_{curNum}", y.ToString(), iniPath);
                        }
                        break;
                    }
                case "FC_6":
                    {
                        // 6盖翻转缸，按特殊规则生成
                        int x = 0, y = 0;
                        for (int i = 0; i < total; i++)
                        {
                            int curNum = i + 1;
                            if (i == 0) continue;
                            else if (i == 1)
                            {
                                x = x1 + 10200;
                                y = y1;
                            }
                            else if (i == 2)
                            {
                                x = x1;
                                y = y1 + 23975;
                            }
                            else if (i == 3)
                            {
                                x = x1 + 10200;
                                y = y1 + 23975;
                            }
                            else if (i == 4)
                            {
                                x = x1;
                                y = y1 + 23975 + 23650;
                            }
                            else if (i == 5)
                            {
                                x = x1 + 10200;
                                y = y1 + 23975 + 23650;
                            }
                            SetIntProperty(areaObj, $"LidCX_{curNum}", x);
                            SetIntProperty(areaObj, $"LidCY_{curNum}", y);
                            ConfigHelper.WriteIniValue(section, $"LidCX_{curNum}", x.ToString(), iniPath);
                            ConfigHelper.WriteIniValue(section, $"LidCY_{curNum}", y.ToString(), iniPath);
                        }
                        break;
                    }
                case "FC_12":
                    {
                        // 12盖翻转缸，弹窗选择布局类型
                        int x = 0, y = 0;
                        using (LidLayout lidLayout = new LidLayout())
                        {
                            if (lidLayout.ShowDialog() == DialogResult.OK)
                            {
                                switch (lidLayout.SelectedLayoutType)
                                {
                                    case 0:
                                        // 布局类型1
                                        for (int i = 0; i < total; i++)
                                        {
                                            int curNum = i + 1;
                                            if (i == 0) continue;
                                            else if (i == 1)
                                            {
                                                x = x1 + 10500;
                                                y = y1;
                                            }
                                            else if (i == 2)
                                            {
                                                x = x1 + 10500 * 2;
                                                y = y1;
                                            }
                                            else if (i == 3)
                                            {
                                                x = x1;
                                                y = y1 + 6500;
                                            }
                                            else if (i == 4)
                                            {
                                                x = x1 + 10500;
                                                y = y1 + 6500;
                                            }
                                            else if (i == 5)
                                            {
                                                x = x1 + 10500 * 2;
                                                y = y1 + 6500;
                                            }
                                            else if (i == 6)
                                            {
                                                x = x1;
                                                y = y1 + 6500 * 2;
                                            }
                                            else if (i == 7)
                                            {
                                                x = x1 + 10500;
                                                y = y1 + 6500 * 2;
                                            }
                                            else if (i == 8)
                                            {
                                                x = x1 + 10500 * 2;
                                                y = y1 + 6500 * 2;
                                            }
                                            else if (i == 9)
                                            {
                                                x = x1;
                                                y = y1 + 6500 * 3;
                                            }
                                            else if (i == 10)
                                            {
                                                x = x1 + 10500;
                                                y = y1 + 6500 * 3;
                                            }
                                            else if (i == 11)
                                            {
                                                x = x1 + 10500 * 2;
                                                y = y1 + 6500 * 3;
                                            }
                                            SetIntProperty(areaObj, $"LidCX_{curNum}", x);
                                            SetIntProperty(areaObj, $"LidCY_{curNum}", y);
                                            ConfigHelper.WriteIniValue(section, $"LidCX_{curNum}", x.ToString(), iniPath);
                                            ConfigHelper.WriteIniValue(section, $"LidCY_{curNum}", y.ToString(), iniPath);
                                        }
                                        break;
                                    case 1:
                                        // 布局类型2
                                        for (int i = 0; i < total; i++)
                                        {
                                            int curNum = i + 1;
                                            if (i == 0) continue;
                                            else if (i == 1)
                                            {
                                                x = x1 + 10200;

                                                y = y1;
                                            }
                                            else if (i == 2)
                                            {
                                                x = x1 + 10200 * 2;

                                                y = y1;
                                            }
                                            else if (i == 3)
                                            {
                                                x = x1;
                                                y = y1 + 6500;
                                            }
                                            else if (i == 4)
                                            {
                                                x = x1 + 10200;

                                                y = y1 + 6500;
                                            }
                                            else if (i == 5)
                                            {
                                                x = x1 + 10200 * 2;

                                                y = y1 + 6500;
                                            }
                                            else if (i == 6)
                                            {
                                                x = x1;
                                                y = y1 + 6500 + 23400;
                                            }
                                            else if (i == 7)
                                            {
                                                x = x1 + 10200;

                                                y = y1 + 6500 + 23400;
                                            }
                                            else if (i == 8)
                                            {
                                                x = x1 + 10200 * 2;
                                                x = x1 + 10200 * 2;
                                                y = y1 + 6500 + 23400;
                                            }
                                            else if (i == 9)
                                            {
                                                x = x1;
                                                y = y1 + 6500 + 23400 + 23650;
                                            }
                                            else if (i == 10)
                                            {
                                                x = x1 + 10200;
                                                x = x1 + 10200;
                                                y = y1 + 6500 + 23400 + 23650;
                                            }
                                            else if (i == 11)
                                            {
                                                x = x1 + 10200 * 2;
                                                x = x1 + 10200 * 2;
                                                y = y1 + 6500 + 23400 + 23650;
                                            }
                                            SetIntProperty(areaObj, $"LidCX_{curNum}", x);
                                            SetIntProperty(areaObj, $"LidCY_{curNum}", y);
                                            ConfigHelper.WriteIniValue(section, $"LidCX_{curNum}", x.ToString(), iniPath);
                                            ConfigHelper.WriteIniValue(section, $"LidCY_{curNum}", y.ToString(), iniPath);
                                        }
                                        break;
                                    case 2:
                                        //布局类型3
                                        for (int i = 0; i < total; i++)
                                        {
                                            int curNum = i + 1;
                                            if (i == 0) continue;
                                            else if (i == 1)
                                            {
                                                x = My_ConPar.Hardware.OriginPosition == 0 ? x1 + 10200 : x1 + 10200;

                                                y = y1;
                                            }
                                            else if (i == 2)
                                            {
                                                x = My_ConPar.Hardware.OriginPosition == 0 ? x1 + 10200 * 2 : x1 + 10200 * 2;

                                                y = y1;
                                            }
                                            else if (i == 3)
                                            {
                                                x = x1;
                                                y = y1 + 6500;
                                            }
                                            else if (i == 4)
                                            {
                                                x = My_ConPar.Hardware.OriginPosition == 0 ? x1 + 10200 : x1 + 10200;

                                                y = y1 + 6500;
                                            }
                                            else if (i == 5)
                                            {
                                                x = My_ConPar.Hardware.OriginPosition == 0 ? x1 + 10200 * 2 : x1 + 10200 * 2;

                                                y = y1 + 6500;
                                            }
                                            else if (i == 6)
                                            {
                                                x = x1;
                                                y = y1 + 6500 + 23400;
                                            }
                                            else if (i == 7)
                                            {
                                                x = My_ConPar.Hardware.OriginPosition == 0 ? x1 + 10200 : x1 + 10200;

                                                y = y1 + 6500 + 23400;
                                            }
                                            else if (i == 8)
                                            {
                                                x = My_ConPar.Hardware.OriginPosition == 0 ? x1 + 10200 * 2 : x1 + 10200 * 2;
                                                x = x1 + 10200 * 2;
                                                y = y1 + 6500 + 23400;
                                            }
                                            else if (i == 9)
                                            {
                                                x = x1;
                                                y = y1 + 6500 + 23400 + 23650;
                                            }
                                            else if (i == 10)
                                            {
                                                x = My_ConPar.Hardware.OriginPosition == 0 ? x1 + 10200 : x1 + 10200;
                                                x = x1 + 10200;
                                                y = y1 + 6500 + 23400 + 23650;
                                            }
                                            else if (i == 11)
                                            {
                                                x = My_ConPar.Hardware.OriginPosition == 0 ? x1 + 10200 * 2 : x1 + 10200 * 2;
                                                x = x1 + 10200 * 2;
                                                y = y1 + 6500 + 23400 + 23650;
                                            }
                                            SetIntProperty(areaObj, $"LidCX_{curNum}", x);
                                            SetIntProperty(areaObj, $"LidCY_{curNum}", y);
                                            ConfigHelper.WriteIniValue(section, $"LidCX_{curNum}", x.ToString(), iniPath);
                                            ConfigHelper.WriteIniValue(section, $"LidCY_{curNum}", y.ToString(), iniPath);
                                        }
                                        break;
                                    case 3:
                                        //布局类型4
                                        for (int i = 0; i < total; i++)
                                        {
                                            int curNum = i + 1;
                                            if (i == 0) continue;
                                            else if (i == 1)
                                            {
                                                x = x1;

                                                y = y1 + 6500;
                                            }
                                            else if (i == 2)
                                            {
                                                x = x1;

                                                y = y1 + 6500 * 2;
                                            }
                                            else if (i == 3)
                                            {
                                                x = x1;
                                                y = y1 + 6500 * 3;
                                            }
                                            else if (i == 4)
                                            {
                                                x = x1 + 10200;

                                                y = y1;
                                            }
                                            else if (i == 5)
                                            {
                                                x = x1 + 10200;

                                                y = y1 + 6500;
                                            }
                                            else if (i == 6)
                                            {
                                                x = x1 + 10200;
                                                y = y1 + 6500 * 2;
                                            }
                                            else if (i == 7)
                                            {
                                                x = x1 + 10200;

                                                y = y1 + 6500 * 3;
                                            }
                                            else if (i == 8)
                                            {
                                                x = x1 + 10200 * 2;
                                                y = y1;
                                            }
                                            else if (i == 9)
                                            {
                                                x = x1 + 10200 * 2;
                                                y = y1 + 6500;
                                            }
                                            else if (i == 10)
                                            {
                                                x = x1 + 10200 * 2;

                                                y = y1 + 6500 * 2;
                                            }
                                            else if (i == 11)
                                            {

                                                x = x1 + 10200 * 2;
                                                y = y1 + 6500 * 3;
                                            }
                                            SetIntProperty(areaObj, $"LidCX_{curNum}", x);
                                            SetIntProperty(areaObj, $"LidCY_{curNum}", y);
                                            ConfigHelper.WriteIniValue(section, $"LidCX_{curNum}", x.ToString(), iniPath);
                                            ConfigHelper.WriteIniValue(section, $"LidCY_{curNum}", y.ToString(), iniPath);
                                        }
                                        break;
                                    case 4:
                                        //布局类型5
                                        for (int i = 0; i < total; i++)
                                        {
                                            int curNum = i + 1;
                                            if (i == 0) continue;
                                            else if (i == 1)
                                            {
                                                x = x1;

                                                y = y1 + 6500;
                                            }
                                            else if (i == 2)
                                            {
                                                x = x1;

                                                y = y1 + 6500 * 2;
                                            }
                                            else if (i == 3)
                                            {
                                                x = x1 + 10200;
                                                y = y1;
                                            }
                                            else if (i == 4)
                                            {
                                                x = x1 + 10200;

                                                y = y1 + 6500;
                                            }
                                            else if (i == 5)
                                            {
                                                x = x1 + 10200;

                                                y = y1 + 6500 * 2;
                                            }
                                            else if (i == 6)
                                            {
                                                x = x1 + 10200 * 2;
                                                y = y1;
                                            }
                                            else if (i == 7)
                                            {
                                                x = x1 + 10200 * 2;

                                                y = y1 + 6500;
                                            }
                                            else if (i == 8)
                                            {
                                                x = x1 + 10200 * 2;
                                                y = y1 + 6500 * 2;
                                            }
                                            else if (i == 9)
                                            {
                                                x = x1 + 10200 * 3;
                                                y = y1;
                                            }
                                            else if (i == 10)
                                            {
                                                x = x1 + 10200 * 3;

                                                y = y1 + 6500;
                                            }
                                            else if (i == 11)
                                            {
                                                x = x1 + 10200 * 3;
                                                y = y1 + 6500 * 2;
                                            }
                                            SetIntProperty(areaObj, $"LidCX_{curNum}", x);
                                            SetIntProperty(areaObj, $"LidCY_{curNum}", y);
                                            ConfigHelper.WriteIniValue(section, $"LidCX_{curNum}", x.ToString(), iniPath);
                                            ConfigHelper.WriteIniValue(section, $"LidCY_{curNum}", y.ToString(), iniPath);
                                        }
                                        break;
                                    default:
                                        SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip("其他类型需要独立为每个杯盖去定位");
                                        return false;
                                }
                            }
                            else
                            {
                                // 用户取消
                                return false;
                            }
                        }
                        break;
                    }
                default:
                    // 其它类型未实现
                    return false;
            }
            return true;
        }

        // 反射获取int属性
        private int GetIntProperty(object obj, string propName)
        {
            var prop = obj.GetType().GetProperty(propName);
            if (prop != null && prop.PropertyType == typeof(int))
                return (int)prop.GetValue(obj);
            return 0;
        }

        // 反射设置int属性
        private void SetIntProperty(object obj, string propName, int value)
        {
            var prop = obj.GetType().GetProperty(propName);
            if (prop != null && prop.PropertyType == typeof(int) && prop.CanWrite)
                prop.SetValue(obj, value);
        }

        private void BtnWrite_Click(object sender, EventArgs e)
        {
            int x = Convert.ToInt32(TxtRPosX.Text);
            int y = Convert.ToInt32(TxtRPosY.Text);

            var layoutType = SmartColor.My_ConPar.Object.CurrentLayout as Type;
            if (layoutType == null)
            {
                My_File.LocalTranslator.ShowMessage("未找到布局类型！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string iniPath = System.IO.Path.Combine(Environment.CurrentDirectory, "Config", "Layout.ini");
            var areaProps = layoutType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            // 统一处理所有区域
            bool found = false;
            foreach (var prop in areaProps)
            {
                var areaObj = prop.GetValue(null) as SmartColor.My_ConPar.Area.Base;
                if (areaObj == null) continue;
                string section = prop.Name.Replace("_", "");

                // 母液瓶区
                if (RdoBottle.Checked && areaObj is My_ConPar.Area.BottleArea.Bottle)
                {
                    if (!int.TryParse(TxtNum.Text, out int num))
                    {
                        My_File.LocalTranslator.ShowMessage("请输入正确的母液瓶号！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    int start = 1;
                    int bottleNum = GetIntProperty(areaObj, "BottleNum");
                    int end = start + bottleNum - 1;
                    if (num >= start && num <= end)
                    {
                        if (num != start)
                        {
                            My_File.LocalTranslator.ShowMessage("只能设置该区域第一个母液瓶的坐标！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        SetIntProperty(areaObj, "BottleCX_1", x);
                        SetIntProperty(areaObj, "BottleCY_1", y);
                        ConfigHelper.WriteIniValue(section, "BottleCX_1", x.ToString(), iniPath);
                        ConfigHelper.WriteIniValue(section, "BottleCY_1", y.ToString(), iniPath);
                        My_File.LocalTranslator.ShowMessage($"{section}第一个坐标写入成功！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AreaCoordinateFinder.BuildAllCoordinateCache(SmartColor.My_ConPar.Object.CurrentLayout as Type);
                        found = true; break;
                    }
                }
                // 配液杯区
                else if (RdoCup.Checked)
                {
                    if (!int.TryParse(TxtNum.Text, out int num))
                    {
                        My_File.LocalTranslator.ShowMessage("请输入正确的配液杯号！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                    // 滴液区：只允许写首杯
                    if (areaObj is SmartColor.My_ConPar.Area.Drop.Drop)
                    {
                        int start = GetIntProperty(areaObj, "StartPosition");
                        if (num == start)
                        {
                            SetIntProperty(areaObj, "CupCX_1", x);
                            SetIntProperty(areaObj, "CupCY_1", y);
                            ConfigHelper.WriteIniValue(section, "CupCX_1", x.ToString(), iniPath);
                            ConfigHelper.WriteIniValue(section, "CupCY_1", y.ToString(), iniPath);
                            My_File.LocalTranslator.ShowMessage($"{section}首杯坐标写入成功！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            AreaCoordinateFinder.BuildAllCoordinateCache(SmartColor.My_ConPar.Object.CurrentLayout as Type);
                            return;
                        }
                        else
                        {
                            My_File.LocalTranslator.ShowMessage("滴液区只允许写入首杯坐标！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    // 其它杯区：允许写任意编号
                    else if (
                        areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_4 ||
                        areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_6 ||
                        areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_12 ||
                        areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_16 ||
                        areaObj is SmartColor.My_ConPar.Area.RotorCylinder.RC_4 ||
                        areaObj is SmartColor.My_ConPar.Area.RotorCylinder.RC_10)
                    {
                        int start = GetIntProperty(areaObj, "StartPosition");
                        int row = GetIntProperty(areaObj, "Row");
                        int col = GetIntProperty(areaObj, "Column");
                        if (row > 0 && col > 0)
                        {
                            int end = start + row * col - 1;
                            if (num >= start && num <= end)
                            {
                                SetIntProperty(areaObj, $"CupCX_{num - start + 1}", x);
                                SetIntProperty(areaObj, $"CupCY_{num - start + 1}", y);
                                ConfigHelper.WriteIniValue(section, $"CupCX_{num - start + 1}", x.ToString(), iniPath);
                                ConfigHelper.WriteIniValue(section, $"CupCY_{num - start + 1}", y.ToString(), iniPath);
                                My_File.LocalTranslator.ShowMessage($"配液杯 {num} 坐标写入成功！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                AreaCoordinateFinder.BuildAllCoordinateCache(SmartColor.My_ConPar.Object.CurrentLayout as Type);
                                return;
                            }
                        }

                    }

                }
                // 杯盖区
                else if (RdoCupLid.Checked && (
                    areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_4 ||
                    areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_6 ||
                    areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_12 ||
                    areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_16))
                {
                    if (!int.TryParse(TxtNum.Text, out int num))
                    {
                        My_File.LocalTranslator.ShowMessage("请输入正确的配液杯盖位！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    int start = GetIntProperty(areaObj, "StartPosition");
                    int row = GetIntProperty(areaObj, "Row");
                    int col = GetIntProperty(areaObj, "Column");
                    if (row > 0 && col > 0)
                    {
                        int end = start + row * col - 1;
                        if (num >= start && num <= end)
                        {
                            SetIntProperty(areaObj, $"LidCX_{num - start + 1}", x);
                            SetIntProperty(areaObj, $"LidCY_{num - start + 1}", y);
                            ConfigHelper.WriteIniValue(section, $"LidCX_{num - start + 1}", x.ToString(), iniPath);
                            ConfigHelper.WriteIniValue(section, $"LidCY_{num - start + 1}", y.ToString(), iniPath);
                            My_File.LocalTranslator.ShowMessage($"配液杯盖 {num} 坐标写入成功！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            AreaCoordinateFinder.BuildAllCoordinateCache(SmartColor.My_ConPar.Object.CurrentLayout as Type);
                            found = true; break;
                        }
                    }
                }
                // 天平区
                else if (RdoBalance.Checked)
                {

                    // 独立天平区
                    if (areaObj is My_ConPar.Area.Balance.Balance)
                    {
                        SetIntProperty(areaObj, "BalanceCX", x);
                        SetIntProperty(areaObj, "BalanceCY", y);
                        ConfigHelper.WriteIniValue(section, "BalanceCX", x.ToString(), iniPath);
                        ConfigHelper.WriteIniValue(section, "BalanceCY", y.ToString(), iniPath);
                        My_File.LocalTranslator.ShowMessage("天平坐标写入成功！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AreaCoordinateFinder.BuildAllCoordinateCache(SmartColor.My_ConPar.Object.CurrentLayout as Type);
                        return;
                    }
                    // 嵌入母液瓶区
                    else if (areaObj is My_ConPar.Area.BottleArea.Bottle bottle)
                    {
                        var balance = bottle.EmbeddedBalance;
                        if (balance != null)
                        {
                            balance.BalanceCX = x;
                            balance.BalanceCY = y;
                            ConfigHelper.WriteIniValue(section, "BalanceCX", x.ToString(), iniPath);
                            ConfigHelper.WriteIniValue(section, "BalanceCY", y.ToString(), iniPath);
                            My_File.LocalTranslator.ShowMessage("母液瓶区嵌入天平坐标写入成功！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            AreaCoordinateFinder.BuildAllCoordinateCache(SmartColor.My_ConPar.Object.CurrentLayout as Type);
                            return;
                        }
                    }


                }
                // 备布区
                else if (RdoDryCloth.Checked && areaObj is My_ConPar.Area.PrepareClothArea.PrepareClothArea)
                {
                    if (!int.TryParse(TxtNum.Text, out int num))
                    {
                        My_File.LocalTranslator.ShowMessage("请输入正确的备布位！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    int start = GetIntProperty(areaObj, "StartPosition");
                    int row = GetIntProperty(areaObj, "Row");
                    int col = GetIntProperty(areaObj, "Column");
                    if (row > 0 && col > 0)
                    {
                        int end = start + row * col - 1;
                        if (num >= start && num <= end)
                        {
                            if (num != start)
                            {
                                My_File.LocalTranslator.ShowMessage("只能设置该区域第一个备布位的坐标！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            SetIntProperty(areaObj, "PrepareClothX_1", x);
                            SetIntProperty(areaObj, "PrepareClothY_1", y);
                            ConfigHelper.WriteIniValue(section, "PrepareClothX_1", x.ToString(), iniPath);
                            ConfigHelper.WriteIniValue(section, "PrepareClothY_1", y.ToString(), iniPath);
                            My_File.LocalTranslator.ShowMessage($"{section}第一个坐标写入成功！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            AreaCoordinateFinder.BuildAllCoordinateCache(SmartColor.My_ConPar.Object.CurrentLayout as Type);
                            found = true; break;
                        }
                    }
                }
                // 出布区
                else if (RdoWetCloth.Checked && areaObj is My_ConPar.Area.OutClothArea.OutClothArea)
                {
                    if (!int.TryParse(TxtNum.Text, out int num))
                    {
                        My_File.LocalTranslator.ShowMessage("请输入正确的出布位！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    int start = GetIntProperty(areaObj, "StartPosition");
                    int row = GetIntProperty(areaObj, "Row");
                    int col = GetIntProperty(areaObj, "Column");
                    if (row > 0 && col > 0)
                    {
                        int end = start + row * col - 1;
                        if (num >= start && num <= end)
                        {
                            if (num != start)
                            {
                                My_File.LocalTranslator.ShowMessage("只能设置该区域第一个出布位的坐标！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            SetIntProperty(areaObj, "OutClothX_1", x);
                            SetIntProperty(areaObj, "OutClothY_1", y);
                            ConfigHelper.WriteIniValue(section, "OutClothX_1", x.ToString(), iniPath);
                            ConfigHelper.WriteIniValue(section, "OutClothY_1", y.ToString(), iniPath);
                            My_File.LocalTranslator.ShowMessage($"{section}第一个坐标写入成功！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            AreaCoordinateFinder.BuildAllCoordinateCache(SmartColor.My_ConPar.Object.CurrentLayout as Type);
                            found = true; break;
                        }
                    }
                }
                // 干布夹子
                else if (RdoDryClamp.Checked && areaObj is My_ConPar.Area.Clip.DryCloth)
                {
                    SetIntProperty(areaObj, "X", x);
                    SetIntProperty(areaObj, "Y", y);
                    ConfigHelper.WriteIniValue(section, "X", x.ToString(), iniPath);
                    ConfigHelper.WriteIniValue(section, "Y", y.ToString(), iniPath);
                    My_File.LocalTranslator.ShowMessage("干布夹子坐标写入成功！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AreaCoordinateFinder.BuildAllCoordinateCache(SmartColor.My_ConPar.Object.CurrentLayout as Type);
                    found = true; break;
                }
                // 湿布夹子
                else if (RdoWetClamp.Checked && areaObj is My_ConPar.Area.Clip.WetCloth)
                {
                    SetIntProperty(areaObj, "X", x);
                    SetIntProperty(areaObj, "Y", y);
                    ConfigHelper.WriteIniValue(section, "X", x.ToString(), iniPath);
                    ConfigHelper.WriteIniValue(section, "Y", y.ToString(), iniPath);
                    My_File.LocalTranslator.ShowMessage("湿布夹子坐标写入成功！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AreaCoordinateFinder.BuildAllCoordinateCache(SmartColor.My_ConPar.Object.CurrentLayout as Type);
                    found = true; break;
                }
                // 洗针区
                else if (RdoWash.Checked && areaObj is My_ConPar.Area.Wash.Wash)
                {
                    SetIntProperty(areaObj, "X", x);
                    SetIntProperty(areaObj, "Y", y);
                    ConfigHelper.WriteIniValue(section, "X", x.ToString(), iniPath);
                    ConfigHelper.WriteIniValue(section, "Y", y.ToString(), iniPath);
                    My_File.LocalTranslator.ShowMessage("洗针坐标写入成功！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AreaCoordinateFinder.BuildAllCoordinateCache(SmartColor.My_ConPar.Object.CurrentLayout as Type);
                    found = true; break;
                }
            }

            if (!found)
            {
                if (RdoDecompression.Checked)
                    My_File.LocalTranslator.ShowMessage("泄压区是根据杯号坐标和泄压偏移量计算出来的，不需要写入坐标！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    My_File.LocalTranslator.ShowMessage("未找到该编号所属的区域！", "写入坐标", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}