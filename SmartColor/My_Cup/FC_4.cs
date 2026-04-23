using SmartColor.My_AutomaticModule;
using SmartColor.My_Control;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Form.Homepage;
using SmartColor.My_Interaction;
using SmartColor.My_Tool;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SmartColor.My_Tool.CupAuxiliary;

namespace SmartColor.My_Cup
{
    /// <summary>
    /// 1#发送区-启动控制区（292~298）
    /// 包含启动/停止/滴液、工艺代码、总步号、AB杯选择等基本控制命令。
    /// </summary>
    internal class FC_4_Send_Base
    {
        /// <summary>1#启动/停止/滴液（启动=1，停止=2，滴液=3，下线=5  上线=6  重启 = 7）</summary>
        public ProtocolItem StartStopReady { get; set; }
        /// <summary>1#染固色工艺代码1（GB2312编码，低字节在前）</summary>
        public ProtocolItem ProcessCode1 { get; set; }
        /// <summary>1#染固色工艺代码2</summary>
        public ProtocolItem ProcessCode2 { get; set; }
        /// <summary>1#染固色工艺代码3</summary>
        public ProtocolItem ProcessCode3 { get; set; }
        /// <summary>1#染固色工艺代码4</summary>
        public ProtocolItem ProcessCode4 { get; set; }
        /// <summary>1#总步号</summary>
        public ProtocolItem TotalStepNum { get; set; }
        /// <summary>备用</summary>
        public ProtocolItem Reserved { get; set; }
    }

    /// <summary>
    /// 1#发送区-工艺参数区（299~308）
    /// 包含当前步号、工艺编号、温度、速率、液量、加药完成等工艺参数。
    /// </summary>
    internal class FC_4_Send_Param
    {
        /// <summary>1#当前步号</summary>
        public ProtocolItem CurrentStepNum { get; set; }
        /// <summary>1#工艺编号（工艺动作/编号）</summary>
        public ProtocolItem TechnologyNameNo { get; set; }
        /// <summary>1#目标温度（单位0.1℃）</summary>
        public ProtocolItem TargetTemp { get; set; }
        /// <summary>1#温度速率（单位0.1℃/min）</summary>
        public ProtocolItem TempRate { get; set; }
        /// <summary>1#保温时间/分</summary>
        public ProtocolItem KeepWarmTime { get; set; }
        /// <summary>1#转子速率（%）</summary>
        public ProtocolItem RotorRate { get; set; }
        /// <summary>1#当前液量（单位0.01kg，主副杯最大值）</summary>
        public ProtocolItem CurrentLiquidAmount { get; set; }
        /// <summary>1#确定键（HMI操作确认）</summary>
        public ProtocolItem ConfirmButton { get; set; }
        /// <summary>1#加药完成（加药完成=2，加药开始=3）</summary>
        public ProtocolItem AddDrugFinish { get; set; }
        /// <summary>1#接收完成（0=未完成，1=完成）</summary>
        public ProtocolItem ReceiveFinish { get; set; }
    }

    /// <summary>
    /// 1#发送区-盖子状态区（309）
    /// 仅包含盖子状态，便于单独同步盖子状态到HMI。
    /// </summary>
    internal class FC_4_Send_Cover
    {
        /// <summary>盖子状态（0=无，1=有盖 2 = 无盖）</summary>
        public ProtocolItem CoverStatus { get; set; }

    }

    internal class FC_4_Send_Pause
    {
        /// <summary>
        /// 暂停信号（1=暂停，2=恢复）
        /// </summary>
        public ProtocolItem PauseSignal { get; set; } = new ProtocolItem(920, typeof(ushort));
    }

    /// <summary>
    /// 1#发送区-总结构体，包含基础控制、工艺参数、盖子状态三大分区
    /// </summary>
    internal class FC_4_Send
    {
        /// <summary>基础控制区（292~298）</summary>
        public FC_4_Send_Base Base { get; set; } = new FC_4_Send_Base();
        /// <summary>工艺参数区（299~308）</summary>
        public FC_4_Send_Param Param { get; set; } = new FC_4_Send_Param();
        /// <summary>盖子状态区（309）</summary>
        public FC_4_Send_Cover Cover { get; set; } = new FC_4_Send_Cover();
    }

    /// <summary>
    /// 6杯翻转缸接收区协议结构体。
    /// 每个分区对应20个寄存器，涵盖主/副杯的实时状态、温度、工艺、报警等信息。
    /// 字段说明：
    /// - 主/副杯：每组包含2个杯位（如1-2区、3-4区等），主杯为奇数号，副杯为偶数号。
    /// - 每个ProtocolItem的Address为PLC寄存器地址，Value为实时值。
    /// </summary>
    internal class FC_4_Receive_Zone
    {
        /// <summary>等待数据标志（1=等待HMI/上位机处理，0=无）</summary>
        public ProtocolItem WaitData { get; set; }
        /// <summary>全部完成标志（1=本组所有工艺完成，0=未完成）</summary>
        public ProtocolItem AllFinish { get; set; }
        /// <summary>主杯实际温度（单位0.1℃，如251=25.1℃）</summary>
        public ProtocolItem ActualTemp { get; set; }
        /// <summary>备用1</summary>
        public ProtocolItem Reserved { get; set; }
        /// <summary>当前工艺代码（1=冷行，2=温控，3=加药，4=放布，5=出布，6=排液，8=加水，9=搅拌，12=取小样，13=测PH）</summary>
        public ProtocolItem CurrentProcess { get; set; }
        /// <summary>当前状态（0=待机 1=运行中 2=暂停 3=保温运行 4=排水 5=滴液 6=停止中  7=下线  8=冷行降温）</summary>
        public ProtocolItem CurrentStatus { get; set; }
        /// <summary>备用2</summary>
        public ProtocolItem Reserved2 { get; set; }
        /// <summary>当前步号（当前工艺流程的步号）</summary>
        public ProtocolItem CurrentStepNum { get; set; }
        /// <summary>当前保温时间（单位：分钟，当前工艺步的保温剩余时间）</summary>
        public ProtocolItem CurrentKeepWarmTime { get; set; }
        /// <summary>杯盖交互信号（0=无，1=需开盖，2=需关盖，4=申请加药）</summary>
        public ProtocolItem CupCoverSignal { get; set; }
        /// <summary>备用3</summary>
        public ProtocolItem Reserved3 { get; set; }
        /// <summary>历史状态（上一次工艺状态，便于判断是否需前洗杯等）</summary>
        public ProtocolItem HistoryStatus { get; set; }
        /// <summary>备用4</summary>
        public ProtocolItem Reserved4 { get; set; }
        /// <summary>锁止上状态（主/副杯锁止信号，0=未锁，1=已锁）</summary>
        public ProtocolItem LockUpStatus { get; set; }
        /// <summary>报警提示（按位报警，详见协议文档，0=无报警）</summary>
        public ProtocolItem AlarmTip { get; set; }
        /// <summary>放布申请确认（1=HMI已确认放布，0=未确认）</summary>
        public ProtocolItem PutClothConfirm { get; set; }
        /// <summary>模块重启信号（1=重启，0=正常）</summary>
        public ProtocolItem ModuleRestartSignal { get; set; }
        /// <summary>排水下到位信号</summary>
        public ProtocolItem DrainageDown { get; set; }
        /// <summary>安全开盖温度</summary>
        public ProtocolItem SafeOpeningTemp { get; set; }
        /// <summary>备用5（预留扩展，当前无实际用途）</summary>
        public ProtocolItem Reserved5 { get; set; }

        /// <summary>
        /// 构造函数，按区块起始地址自动初始化所有协议项
        /// </summary>
        /// <param name="baseAddr">区块起始寄存器地址（如500、520等）</param>
        public FC_4_Receive_Zone(int baseAddr)
        {
            WaitData = new ProtocolItem(baseAddr + 0, typeof(short));
            AllFinish = new ProtocolItem(baseAddr + 1, typeof(short));
            ActualTemp = new ProtocolItem(baseAddr + 2, typeof(short));
            Reserved = new ProtocolItem(baseAddr + 3, typeof(short));
            CurrentProcess = new ProtocolItem(baseAddr + 4, typeof(short));
            CurrentStatus = new ProtocolItem(baseAddr + 5, typeof(short));
            Reserved2 = new ProtocolItem(baseAddr + 6, typeof(short));
            CurrentStepNum = new ProtocolItem(baseAddr + 7, typeof(short));
            CurrentKeepWarmTime = new ProtocolItem(baseAddr + 8, typeof(short));
            CupCoverSignal = new ProtocolItem(baseAddr + 9, typeof(short));
            Reserved3 = new ProtocolItem(baseAddr + 10, typeof(short));
            HistoryStatus = new ProtocolItem(baseAddr + 11, typeof(short));
            Reserved4 = new ProtocolItem(baseAddr + 12, typeof(short));
            LockUpStatus = new ProtocolItem(baseAddr + 13, typeof(short));
            AlarmTip = new ProtocolItem(baseAddr + 14, typeof(short));
            PutClothConfirm = new ProtocolItem(baseAddr + 15, typeof(short));
            ModuleRestartSignal = new ProtocolItem(baseAddr + 16, typeof(short));
            DrainageDown = new ProtocolItem(baseAddr + 17, typeof(short));
            SafeOpeningTemp = new ProtocolItem(baseAddr + 18, typeof(short));
            Reserved5 = new ProtocolItem(baseAddr + 19, typeof(short));
        }
    }

    /// <summary>
    /// 6杯翻转缸所有接收区协议结构体
    /// 每组对应20个寄存器，涵盖实时状态、温度、工艺、报警等信息。
    /// 便于批量读取、遍历和维护。
    /// </summary>
    internal class FC_4_Receive_All
    {
        /// <summary>1区（500~519），1杯所有接收数据</summary>
        public FC_4_Receive_Zone Zone1 { get; set; } = new FC_4_Receive_Zone(500);
        /// <summary>2区（520~539），2杯所有接收数据</summary>
        public FC_4_Receive_Zone Zone2 { get; set; } = new FC_4_Receive_Zone(520);
        /// <summary>3区（540~559），3杯所有接收数据</summary>
        public FC_4_Receive_Zone Zone3 { get; set; } = new FC_4_Receive_Zone(540);
        /// <summary>4区（560~579），4杯所有接收数据</summary>
        public FC_4_Receive_Zone Zone4 { get; set; } = new FC_4_Receive_Zone(560);



        public FC_4_Receive_Zone GetZoneFromGroup(int group)
        {

            switch (group)
            {
                case 0: return Zone1;
                case 1: return Zone2;
                case 2: return Zone3;
                case 3: return Zone4;

                default: throw new ArgumentOutOfRangeException($"无效的杯号: {group}");
            }
        }

        public FC_4_Receive_Zone GetZoneFromCupNo(int cupNo, int startCupNo)
        {
            int group = cupNo - startCupNo;
            return GetZoneFromGroup(group);
        }
    }

    /// <summary>
    /// 4杯翻转缸版本区协议结构体
    /// </summary>
    internal class FC_4_Version
    {
        /// <summary>触摸屏版本1</summary>
        public ProtocolItem TouchScreenVersion1 { get; set; }
        /// <summary>触摸屏版本2</summary>
        public ProtocolItem TouchScreenVersion2 { get; set; }
        /// <summary>1#模块硬件版本</summary>
        public ProtocolItem Module1HardwareVersion { get; set; }
        /// <summary>1#模块软件版本</summary>
        public ProtocolItem Module1SoftwareVersion { get; set; }
        /// <summary>2#模块硬件版本</summary>
        public ProtocolItem Module2HardwareVersion { get; set; }
        /// <summary>2#模块软件版本</summary>
        public ProtocolItem Module2SoftwareVersion { get; set; }
    }


    /// <summary>
    /// 翻转缸4杯位通讯协议主类，包含所有协议项集合及工厂方法。
    /// 支持PLC通讯、数据解析，并通过事件推送每个杯位的数据到UI层。
    /// </summary>
    internal class FC_4 : ICylinderComm
    {
        private volatile bool _isRunning = false;
        public bool IsRunning => _isRunning;

        //起始杯号
        private int _startCupNo = 0;

        /// <summary>
        /// Modbus TCP通讯实例，负责底层数据收发
        /// </summary>
        private readonly ModbusTCP _modbus;

        /// <summary>
        /// PLC站号（UnitId），通常为1
        /// </summary>
        private readonly byte _unitId = 1;

        private CtCupArea _ctCupArea;

        /// <summary>
        /// 区域名称
        /// </summary>
        private string _areaName;

        /// <summary>
        /// 重滴列表
        /// </summary>
        private List<int> _retrying = new List<int>();

        /// <summary>
        /// 需要停止并下线的杯号
        /// </summary>
        private List<int> _needStop = new List<int>();

        /// <summary>
        /// 构造函数，初始化ModbusTCP通讯实例和协议区
        /// </summary>
        /// <param name="ip">服务器IP</param>
        /// <param name="port">服务器端口</param>
        /// <param name="hmiType">HMI类型（0威纶通，1昆仑通态）</param>
        /// <param name="areaName">区域名称</param>
        public FC_4(string ip, int port, int startCupNo, int hmiType, string areaName)
        {
            this._startCupNo = startCupNo;
            this._areaName = areaName;
            this._modbus = new ModbusTCP(ip, port, areaName);
        }

        /// <summary>
        /// 所有杯位的发送区协议项集合
        /// </summary>
        private readonly List<FC_4_Send> _send = new List<FC_4_Send>()
        {
            CreateSend(100),
            CreateSend(164),
            CreateSend(228),
            CreateSend(292),

        };

        private readonly FC_4_Receive_All _receiveAll = new FC_4_Receive_All();

        private readonly FC_4_Send_Pause _pause = new FC_4_Send_Pause();

        /// <summary>
        /// 版本区协议项集合
        /// </summary>
        private readonly FC_4_Version _version = CreateVersionInfo();



        /// <summary>
        /// 通讯数据推送事件
        /// 参数1：杯位索引（1~12），参数2：该杯位的主要数据字典（字段名-值）
        /// </summary>
        public event Action<int, Dictionary<string, object>> OnCupDataReceived;

        private readonly object _zoneLock = new object();

        private readonly ConcurrentDictionary<int, byte> _waitDataProcessing = new ConcurrentDictionary<int, byte>();
        private readonly SignalEdgeHandler _alarmTipHandler = new SignalEdgeHandler();
        private readonly SignalEdgeHandler _allFinishHandler = new SignalEdgeHandler();
        private readonly SignalEdgeHandler _cupSignalHandler = new SignalEdgeHandler();
        private readonly SignalEdgeHandler _putClothHandler = new SignalEdgeHandler();
        private readonly SignalEdgeHandler _waitDataHandler = new SignalEdgeHandler();
        private readonly SignalEdgeHandler _moduleRestartHandler = new SignalEdgeHandler();
        private readonly SignalEdgeHandler _historyHandler = new SignalEdgeHandler();

        /// <summary>
        /// 工厂方法：创建发送区协议项结构体
        /// </summary>
        /// <param name="baseAddr">发送区起始地址</param>
        /// <param name="hmiType">HMI类型</param>
        /// <returns>初始化后的发送区对象</returns>
        private static FC_4_Send CreateSend(int baseAddr)
        {

            var send = new FC_4_Send
            {
                Base = new FC_4_Send_Base
                {
                    StartStopReady = new ProtocolItem(baseAddr + 0, typeof(ushort)),
                    ProcessCode1 = new ProtocolItem(baseAddr + 1, typeof(ushort)),
                    ProcessCode2 = new ProtocolItem(baseAddr + 2, typeof(ushort)),
                    ProcessCode3 = new ProtocolItem(baseAddr + 3, typeof(ushort)),
                    ProcessCode4 = new ProtocolItem(baseAddr + 4, typeof(ushort)),
                    TotalStepNum = new ProtocolItem(baseAddr + 5, typeof(ushort)),
                    Reserved = new ProtocolItem(baseAddr + 6, typeof(ushort))
                },
                Param = new FC_4_Send_Param
                {
                    CurrentStepNum = new ProtocolItem(baseAddr + 7, typeof(ushort)),
                    TechnologyNameNo = new ProtocolItem(baseAddr + 8, typeof(ushort)),
                    TargetTemp = new ProtocolItem(baseAddr + 9, typeof(ushort)),
                    TempRate = new ProtocolItem(baseAddr + 10, typeof(ushort)),
                    KeepWarmTime = new ProtocolItem(baseAddr + 11, typeof(ushort)),
                    RotorRate = new ProtocolItem(baseAddr + 12, typeof(ushort)),
                    CurrentLiquidAmount = new ProtocolItem(baseAddr + 13, typeof(ushort)),
                    ConfirmButton = new ProtocolItem(baseAddr + 14, typeof(ushort)),
                    AddDrugFinish = new ProtocolItem(baseAddr + 15, typeof(ushort)),
                    ReceiveFinish = new ProtocolItem(baseAddr + 16, typeof(ushort))
                },
                Cover = new FC_4_Send_Cover
                {
                    CoverStatus = new ProtocolItem(baseAddr + 17, typeof(ushort)),

                }
            };
            return send;
        }

        /// <summary>
        /// 工厂方法：创建版本区协议项结构体
        /// </summary>
        /// <returns>初始化后的版本区对象</returns>
        private static FC_4_Version CreateVersionInfo()
        {
            return new FC_4_Version
            {
                TouchScreenVersion1 = new ProtocolItem(4, typeof(ushort)),
                TouchScreenVersion2 = new ProtocolItem(5, typeof(ushort)),
                Module1HardwareVersion = new ProtocolItem(6, typeof(ushort)),
                Module1SoftwareVersion = new ProtocolItem(7, typeof(ushort)),
                Module2HardwareVersion = new ProtocolItem(8, typeof(ushort)),
                Module2SoftwareVersion = new ProtocolItem(9, typeof(ushort))
            };
        }

        /// <summary>
        /// 获取所有发送区协议项集合
        /// </summary>
        public List<FC_4_Send> GetSendList() => _send;

        /// <summary>
        /// 获取版本区协议项
        /// </summary>
        public FC_4_Version GetVersionInfo() => _version;

        /// <summary>
        /// 连接到HMI服务器。
        /// 实际调用ModbusTCP的Connect方法，支持自动重连。
        /// </summary>
        public void Connect()
        {
            this._modbus.Connect();
        }

        /// <summary>
        /// 断开与PLC服务器的连接。
        /// </summary>
        public void Disconnect()
        {
            this._modbus.Dispose();
        }


        /// <summary>
        /// 计算版本区需要读取的寄存器数量（根据数据类型自动判断）
        /// </summary>
        /// <param name="version">版本区对象</param>
        /// <returns>需要操作的寄存器数量</returns>
        private static int GetVersionRegisterCount(FC_4_Version version)
        {
            int count = 0;
            foreach (var prop in typeof(FC_4_Version).GetProperties())
            {
                var item = prop.GetValue(version) as ProtocolItem;
                if (item != null)
                {
                    count += item.DataType == typeof(int) ? 2 : 1;
                }
            }
            return count;
        }

        /// <summary>
        /// 将操作工艺名称转换为对应的代码。
        /// </summary>
        /// <param name="technologyName">工艺名称</param>
        /// <returns>代码</returns>
        private ushort ConvertTechnologyNameToCode(string technologyName)
        {
            ushort i = 0;
            switch (technologyName)
            {
                case "冷行":
                    i = 1; break;
                case "温控":
                    i = 2; break;
                case "加A":
                case "加B":
                case "加C":
                case "加D":
                case "加E":
                case "加F":
                case "加G":
                case "加H":
                case "加I":
                case "加J":
                case "加K":
                case "加L":
                case "加M":
                case "加N":
                case "加药":
                    i = 3; break;
                case "放布":
                    i = 4; break;
                case "出布":
                    i = 5; break;
                case "排液":
                    i = 6; break;
                case "加水":
                    i = 8; break;
                case "搅拌":
                    i = 9; break;
                case "取小样":
                    i = 12; break;
                case "测PH":
                    i = 13; break;
                default:
                    break;
            }
            return i;
        }

        /// <summary>
        /// 将工艺代码转换为对应的操作工艺名称。
        /// </summary>
        /// <param name="code">工艺代码</param>
        /// <returns>工艺名称</returns>
        public static string ConvertCodeToTechnologyName(ushort code)
        {
            switch (code)
            {
                case 1: return "冷行";
                case 2: return "温控";
                case 3: return "加药";
                case 4: return "放布";
                case 5: return "出布";
                case 6: return "排液";
                case 8: return "加水";
                case 9: return "搅拌";
                case 12: return "取小样";
                case 13: return "测PH";
                default: return string.Empty;
            }
        }

        /// <summary>
        /// 将当前状态转换为对应的操作工艺名称。
        /// </summary>
        /// <param name="code">工艺代码</param>
        /// <returns>工艺名称</returns>
        public static string ConvertCodeToStatues(ushort code)
        {
            //状态：0=待机 1=运行中 2=暂停 3=保温运行 4=排水 5=滴液 6=停止中  7=下线  8=冷行降温
            switch (code)
            {
                case 0: return "待机";
                case 1: return "运行中";
                case 2: return "暂停";
                case 3: return "保温";
                case 4: return "排水";
                case 5: return "滴液";
                case 6: return "停止中";
                case 7: return "下线";
                case 8: return "降温";

                default: return string.Empty;
            }
        }

        /// <summary>
        /// 将工艺名称（如“前洗杯”）填充到ProcessCode1~ProcessCode4中（GB2312编码，每字2字节，空位补0x0D00）
        /// </summary>
        /// <param name="processName">工艺名称（最多4个汉字）</param>
        /// <param name="send">FC_4_Send结构体</param>
        private void FillProcessNameToCodes(string processName, FC_4_Send_Base send)
        {
            // 1. GB2312编码
            var fromEncoding = System.Text.Encoding.UTF8;
            var toEncoding = System.Text.Encoding.GetEncoding("GB2312");
            byte[] srcBytes = fromEncoding.GetBytes(processName ?? "");
            byte[] gbBytes = System.Text.Encoding.Convert(fromEncoding, toEncoding, srcBytes);

            // 2. 组装4个ushort（每个2字节，低字节在前，高字节在后，空位补0x0D00）
            ushort[] processWords = new ushort[4];
            for (int i = 0; i < processWords.Length; i++)
            {
                int idx = i * 2;
                byte low = idx < gbBytes.Length ? gbBytes[idx] : (byte)0x00;
                byte high = (idx + 1) < gbBytes.Length ? gbBytes[idx + 1] : (byte)0x0D;
                processWords[i] = (ushort)((high << 8) | low);
            }

            // 3. 填充到ProcessCode1~4
            send?.ProcessCode1.SetValue(processWords[0]);
            send?.ProcessCode2.SetValue(processWords[1]);
            send?.ProcessCode3.SetValue(processWords[2]);
            send?.ProcessCode4.SetValue(processWords[3]);
        }

        /// <summary>
        /// 解析报警提示（AlarmTip），按位报警
        /// </summary>
        /// <param name="cupNo">主杯杯号</param>
        /// <param name="alarmTip">报警值</param>
        private void ParseAlarmTip(int cupNo, ushort alarmTip, FC_4_Receive_Zone zone)
        {

            _ = Task.Run(async () =>
            {
                try
                {



                    var ms = My_Tool.CupAuxiliary.GetCupPair(cupNo);

                    if ((alarmTip & (1 << 0)) != 0)
                        Logger.Error($"{ms.mainCup}号杯超极限温度报警");
                    if ((alarmTip & (1 << 1)) != 0)
                        Logger.Error($"{ms.mainCup}/{ms.subCup}号杯电机电流过大报警");
                    if ((alarmTip & (1 << 2)) != 0)
                        Logger.Error($"{ms.mainCup}号杯高于安全温度进入冷行");
                    if ((alarmTip & (1 << 3)) != 0)
                        Logger.Error($"{ms.mainCup}/{ms.subCup}号杯回原点超时报警");
                    if ((alarmTip & (1 << 4)) != 0)
                        Logger.Error($"{ms.mainCup}/{ms.subCup}号杯上下锁止信号异常报警");
                    if ((alarmTip & (1 << 5)) != 0)
                        Logger.Error($"{ms.subCup}号杯超极限温度报警");
                    if ((alarmTip & (1 << 6)) != 0)
                        Logger.Error($"{ms.subCup}号杯高于安全温度进入冷行");
                    if ((alarmTip & (1 << 7)) != 0)
                        Logger.Error($"{ms.mainCup}/{ms.subCup}号杯排水下到位信号异常");
                    if ((alarmTip & (1 << 8)) != 0)
                        Logger.Error($"{ms.mainCup}号杯加热输出异常");
                    if ((alarmTip & (1 << 9)) != 0)
                        Logger.Error($"{ms.subCup}号杯加热输出异常");

                    if ((alarmTip & (1 << 2)) != 0 || (alarmTip & (1 << 6)) != 0 ||
                       (alarmTip & (1 << 8)) != 0 || (alarmTip & (1 << 9)) != 0)
                    {
                    }
                    else
                    {
                        await SendOffLine(cupNo, 0);
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error($"ParseAlarmTip: 杯号{cupNo}处理异常", ex);
                }
                finally
                {
                    lock (this._zoneLock)
                    {

                        WriteSingleProtocolItem(zone.AlarmTip);
                    }
                }
            });

        }

        /// <summary>
        /// 写入单个协议项（寄存器）到HMI
        /// </summary>
        /// <param name="item">协议项</param>
        /// <param name="value">要写入的值</param>
        private void WriteSingleProtocolItem(ProtocolItem item, ushort value = 0)
        {
            lock (_zoneLock)
            {
                if (item == null) return;

                // 在转换前添加类型检查
                if (value is ushort ushortValue)
                {
                    this._modbus.WriteSingleRegister(this._unitId, (ushort)item.Address, ushortValue);
                }

                else
                {
                    // 尝试安全转换
                    try
                    {
                        ushort convertedValue = Convert.ToUInt16(value);
                        this._modbus.WriteSingleRegister(this._unitId, (ushort)item.Address, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        // 记录错误并处理
                        Logger.Error($"值转换失败: {item.Value} - {ex.Message}");
                        throw;
                    }
                }
            }
        }


        /// <summary>
        /// 将任意区数据写入HMI
        /// </summary>
        private void WriteToHMI(object sendArea)
        {
            lock (_zoneLock)
            {
                if (sendArea == null) return;

                var props = sendArea.GetType().GetProperties();
                if (props.Length == 0) return;

                // 获取起始地址
                var firstItem = props[0].GetValue(sendArea) as ProtocolItem;
                if (firstItem == null) return;
                ushort startAddr = (ushort)firstItem.Address;

                List<ushort> values = new List<ushort>();
                foreach (var prop in props)
                {
                    var item = prop.GetValue(sendArea) as ProtocolItem;
                    if (item != null && item.Value != null)
                    {
                        if (item.DataType == typeof(int))
                        {
                            int val = Convert.ToInt32(item.Value);
                            values.Add((ushort)(val & 0xFFFF));
                            values.Add((ushort)(val >> 16));
                        }
                        else
                        {
                            values.Add(Convert.ToUInt16(item.Value));
                        }
                    }
                    else
                    {
                        values.Add(0);
                    }
                }

                this._modbus.WriteMultipleRegisters(this._unitId, startAddr, values.ToArray());
            }
        }

        /// <summary>
        /// 启动通讯时同步所有杯盖状态到HMI
        /// </summary>
        private async Task SyncAllCoverStatus()
        {
            for (int i = 0; i < this._send.Count; i++)
            {
                int cupNum = this._startCupNo + i;
                await SyncCoverStatus(cupNum);
            }
        }

        /// <summary>
        ///  等待数据处理
        /// </summary>
        /// <param name="cupNo">杯号</param>
        /// <param name="currentStepNum">当前步号</param>
        private void WaitDataProcess(int cupNo, int currentStepNum, FC_4_Receive_Zone zone)
        {
            // 防止同一杯号并发处理
            if (!_waitDataProcessing.TryAdd(cupNo, 0))
                return;
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNo}号配液杯等待数据触发发送下一步信号"
            }, DateTime.Now);



            try
            {
                var (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupNo);
                int mainCup = mainInfo.CupNum;

                int group = mainCup - this._startCupNo;


                DateTime receptionTime = DateTime.Now;
                // 1. 获取主副杯详细信息
                var send = this._send[group].Param;



                // 3. 查找工艺参数（任选主杯或副杯，优先主杯）
                var info = mainInfo.IsUsing == 1 ? mainInfo : subInfo;
                string dyeingCode = info.DyeingCode ?? "";

                // 4. 查找下一步工艺
                bool isWash = CupAuxiliary.WashCupDic.ContainsKey(dyeingCode);
                CupAuxiliary.StepInfo nextProcess;
                if (isWash)
                {
                    nextProcess = CupAuxiliary.GetNextWashCupStepInfo(dyeingCode, currentStepNum);
                    int haveCloth1 = mainInfo.HaveCloth ?? 0;

                    if (nextProcess.TechnologyName == "出布")
                    {

                        haveCloth1 = mainInfo.HaveCloth ?? 0;

                        if (haveCloth1 == 0)
                        {
                            //都没有布，则找下一步
                            nextProcess = CupAuxiliary.GetNextWashCupStepInfo(dyeingCode, currentStepNum + 1);
                        }
                    }
                }
                else
                {
                    int mainCupHeadID = mainInfo.HeadID ?? 0;

                    if (mainCupHeadID == 0)
                    {
                        throw new Exception($"WaitDataProcess: 杯号{cupNo}当前步骤{currentStepNum}HeadID均为0，无法获取下一步工艺");
                    }

                    int headID = 0;

                    headID = mainCupHeadID;


                    nextProcess = CupAuxiliary.GetNextStepInfo(headID, currentStepNum);
                }

                // 5. 更新数据库（根据杯选择）
                DateTime startTime = DateTime.Now;
                var cupdic = new Dictionary<string, object>()
                     {
                        { CUP_DETAILS.StepNum, nextProcess.StepNum },
                        { CUP_DETAILS.TechnologyName, nextProcess.TechnologyName },
                        { CUP_DETAILS.SetTemp, nextProcess.Temp },
                        { CUP_DETAILS.ReceptionTime, receptionTime },
                        { CUP_DETAILS.StepStartTime, startTime },
                        { CUP_DETAILS.DyeType, nextProcess.DyeType },
                        { CUP_DETAILS.SetTime, nextProcess.SetTime },
                        { CUP_DETAILS.CurrentStepFinish, 0}
                     };
                if (Convert.ToInt16(zone.CurrentProcess.Value) == 6)
                {
                    cupdic.Add(CUP_DETAILS.CurrentWeight, 0);
                }


                My_DataBase.SqlServer.Update(CUP_DETAILS.TableName,
                   cupdic,
                   $"{CUP_DETAILS.CupNum} = @cupNo",
                   new SqlParameter("@cupNo", mainCup)
               );
                var area = CupCommManager.Instance.FindCupAreaByCupNum(mainInfo.CupNum);
                if (area != null)
                {
                    area.OnCupDataReceived(mainInfo.CupNum);
                }

                _ = SendNextStep(cupNo, nextProcess);

            }
            catch (Exception ex)
            {
                var useName = My_Tool.CupAuxiliary.GetIsUseing(cupNo);
                Logger.Error($"WaitDataProcess: {useName}号杯处理等待数据异常", ex);
            }
            finally
            {

            }

        }

        /// <summary>
        /// 检查是否需要机械手介入任务
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <param name="stepInfo">步骤信息</param>
        private async Task CheckNeedRobotTask(int cupNum, CupAuxiliary.StepInfo stepInfo, bool isFrist = false)
        {
            await Task.Run(async () =>
            {
                try
                {
                    // 1. 查找主副杯号
                    var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNum);
                    int group = (mainInfo.CupNum - this._startCupNo);

                    // 2. 获取分区
                    FC_4_Receive_Zone zone = this._receiveAll.GetZoneFromGroup(group);
                    // 4. 获取dyeType
                    int dyeType = stepInfo.DyeType;
                    string technologyName = stepInfo.TechnologyName;

                    // 5. 判断是否需要机械手介入
                    // 冷行、温控、搅拌：关盖（盖状态为2时），等待锁止上信号
                    if (technologyName == "冷行" || technologyName == "温控" || technologyName == "搅拌")
                    {
                        // 需要关盖->等待锁止上信号到位
                        bool needCloseCover = false;
                        if (mainInfo.IsUsing == 1 && mainInfo.CoverStatus == 2)
                        {
                            needCloseCover = true;
                        }


                        if (needCloseCover)
                            await WaitLockAndEnqueue(mainInfo.CupNum, zone, dyeType, technologyName);
                        else
                        {
                            if (mainInfo.IsUsing == 1)
                            {
                                var recorder = SmartColor.My_File.CupTempRecorder.Get(mainInfo.CupNum);
                                recorder.SetCylinderComm(this);
                                recorder.RecordStep(stepInfo.TechnologyName);
                            }

                        }
                    }

                    else if (technologyName.Contains("加") && technologyName != "加水")
                    {
                        //加药工艺：（预留）
                        if (isFrist)
                        {
                            await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueAddChemicalAsync(cupNum, dyeType, false, false);
                        }
                    }
                    else
                    {
                        // 其他工艺（除加药）：开盖，等待锁止下信号
                        await WaitLockAndEnqueue(mainInfo.CupNum, zone, dyeType, technologyName);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"CheckNeedRobotTask: 机械手任务判断异常 cupNum={cupNum}, step={stepInfo.StepNum}, tech={stepInfo.TechnologyName}", ex);
                }
            });
        }

        /// <summary>
        /// 等待锁止上信号为1后提交任务，超时报警但不退出
        /// </summary>
        private async Task WaitLockAndEnqueue(int cupNo, FC_4_Receive_Zone zone, int dyeType, string technologyName = null)
        {
            // 统一等待锁止信号
            var wr = await SmartColor.My_Tool.CupAuxiliary.WaitLockUpOk(cupNo, allowAlarm: technologyName != "排液");
            if (!wr)
            {
                Logger.Error($"等待{cupNo}号杯锁止上信号异常");
                return;
            }

            if (technologyName != null)
            {
                if (technologyName != "排液")
                {
                    await CallProcessTask(cupNo, technologyName, dyeType);
                }
                else
                {
                label1:
                    // 排液工艺，要区分是否是高温排液，还是常温排液
                    int sol = Convert.ToInt32(zone.SafeOpeningTemp.Value) / 10;

                    bool readly = false;
                    // 主杯
                    if (Convert.ToInt16(zone.CurrentStatus.Value) != 7)
                    {
                        double temp = Math.Round(Convert.ToDouble(zone.ActualTemp.Value) / 10, 1);
                        if (temp < sol)
                        {
                            // 低温
                            readly = true;
                        }
                        else
                        {
                            // 高温
                            Thread.Sleep(1000);
                            readly = true;

                        }
                    }

                    if (readly)
                    {
                        // 排液
                        await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueDrainAsync(cupNo, dyeType);
                    }
                    else
                    {
                        Logger.Error($"{cupNo}号杯排液工艺条件未达标，无法执行排液任务");
                        goto label1;
                    }
                }
            }
        }

        /// <summary>
        /// 根据工艺名称调用对应的任务提交接口
        /// </summary>
        private async Task CallProcessTask(int cupNo, string technologyName, int dyeType)
        {
            switch (technologyName)
            {
                case "冷行":
                    await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueColdRunAsync(cupNo, dyeType);
                    break;
                case "温控":
                    await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueTemperatureControlAsync(cupNo, dyeType);
                    break;
                case "放布":
                    await PutClothConfirm(cupNo, "放布");
                    break;
                case "出布":
                    await PutClothConfirm(cupNo, "出布");
                    break;
                case "加水":
                    await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueAddWaterAsync(cupNo, dyeType);
                    break;
                case "搅拌":
                    await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueStirAsync(cupNo, dyeType);
                    break;
                case "取小样":
                    await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueSampleAsync(cupNo, dyeType);
                    break;

                case "测PH":
                    await SmartColor.My_AutomaticModule.CupRobotTask.EnqueuePHAsync(cupNo, dyeType);
                    break;

                case "加A":
                case "加B":
                case "加C":
                case "加D":
                case "加E":
                case "加F":
                case "加G":
                case "加H":
                case "加I":
                case "加J":
                case "加K":
                case "加L":
                case "加M":
                case "加N":
                    break;
                default:
                    Logger.Error($"CheckNeedRobotTask: 未知工艺类型 {technologyName}");
                    break;
            }
        }

        /// <summary>
        /// 杯号完成处理
        /// </summary>
        /// <param name="cupNo">杯号</param>
        private void AllFinishProcess(int cupNo, FC_4_Receive_Zone zone)
        {
            lock (this._zoneLock)
            {

                this.WriteSingleProtocolItem(zone.AllFinish);
            }

            try
            {

                var (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupNo);

                // 5. 重置杯完成信号
                int group = mainInfo.CupNum - this._startCupNo;

                FC_4_Send_Param send = this._send[group].Param;


                ResetSendParamToZero(send);

                var useName = My_Tool.CupAuxiliary.GetIsUseing(cupNo);
                var dt = DateTime.Now;
                _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                {
                    [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{useName}号杯收到完成信号"
                }, dt);

                while (true)
                {
                    bool mainOk = false;
                    (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupNo);

                    if (mainInfo.Enable == 1)
                    {
                        if (mainInfo.Statues == "待机")
                            mainOk = true;


                    }
                    else
                    {
                        mainOk = true;
                    }


                    if (mainOk)
                        break;

                    Thread.Sleep(500);


                }

                var msCups = My_Tool.CupAuxiliary.GetIsUseing(cupNo);

                // 1. 获取主副杯详细信息
                int mainCup = mainInfo.CupNum;


                // 1. 取消任务中心关于该杯的所有等待中的任务
                var allTasks = SmartColor.My_RobotManager.RobotTaskManager.Instance.GetAllTasksSnapshot();
                foreach (var taskObj in allTasks)
                {
                    // 只处理业务任务
                    var type = taskObj.GetType();
                    var taskNameProp = type.GetProperty("TaskName");
                    if (taskNameProp != null)
                    {
                        string taskName = taskNameProp.GetValue(taskObj) as string;
                        if (!string.IsNullOrEmpty(taskName) && taskName.EndsWith($"-{msCups}"))
                        {
                            SmartColor.My_RobotManager.RobotTaskManager.Instance.CancelTask(taskObj);
                        }
                    }
                }

                // 2. 删除该杯的所有播报
                string[] speakKeys = new string[]
                {
                        $"{mainCup}号配液杯入布",
                        $"{mainCup}号配液杯出布",
                        $"{mainCup}号配液杯取小样",
                        $"{mainCup}号配液杯测PH",

                };
                foreach (var key in speakKeys)
                {
                    SmartColor.My_Tool.MessageEventManager.Instance.RequestStopLoopSpeak(key);
                }

                // 2. 染固色代码
                string mainDyeingCode = mainInfo.DyeingCode;
                string subDyeingCode = subInfo.DyeingCode;

                // 3. 判断是否为洗杯工艺
                bool mainIsWash = !string.IsNullOrEmpty(mainDyeingCode) && CupAuxiliary.WashCupDic.ContainsKey(mainDyeingCode) || mainDyeingCode == null;


                ushort cupSelect = 0;
                if (mainInfo.IsUsing == 1)
                {
                    //停止温度记录
                    var recorder = SmartColor.My_File.CupTempRecorder.Get(mainInfo.CupNum);
                    recorder.SetCylinderComm(this);
                    recorder.StopRecord();


                    if (!mainIsWash)
                    {
                        if (this._retrying.Contains(mainCup))
                        {
                            if (this._retrying.Contains(mainCup))
                                this._retrying.Remove(mainCup);

                        }
                        else
                        {

                            CupAuxiliary.CupFinish(mainCup, cupSelect, !this._needStop.Contains(mainCup));


                        }
                    }
                    else
                    {
                        CupAuxiliary.ResetCupDetails(mainInfo.CupNum);

                        if ((mainIsWash && mainInfo.DyeingCode != My_Tool.CupAuxiliary.PreWashCupType))
                        {
                            if (!this._needStop.Contains(mainCup))
                                UpdateWaitList(mainCup, true);
                        }


                    }

                }

                else
                {
                    if (mainInfo.Enable == 1)
                    {
                        CupAuxiliary.ResetCupDetails(mainInfo.CupNum);
                        if (!this._needStop.Contains(mainCup))
                            UpdateWaitList(cupNo, true);
                    }



                }


            }
            catch (Exception ex)
            {
                var useName = My_Tool.CupAuxiliary.GetIsUseing(cupNo);
                Logger.Error($"AllFinishProcess: {useName}号杯完成处理异常", ex);
            }

            finally
            {
                lock (this._zoneLock)
                {

                    this.WriteSingleProtocolItem(zone.AllFinish);
                }
                ResetAllSignalHandlers(cupNo);
            }



        }

        /// <summary>
        /// 将FC_4_Send_Param所有协议项的值置为0
        /// </summary>
        /// <param name="param">工艺参数区对象</param>
        private void ResetSendParamToZero(FC_4_Send_Param param)
        {
            lock (this._zoneLock)
            {
                if (param == null) return;
                param.CurrentStepNum?.SetValue(0);
                param.TechnologyNameNo?.SetValue(0);
                param.TargetTemp?.SetValue(0);
                param.TempRate?.SetValue(0);
                param.KeepWarmTime?.SetValue(0);
                param.RotorRate?.SetValue(0);
                param.CurrentLiquidAmount?.SetValue(0);
                param.ConfirmButton?.SetValue(0);
                param.AddDrugFinish?.SetValue(0);
                param.ReceiveFinish?.SetValue(0);
                WriteToHMI(param);
            }
        }


        /// <summary>
        /// 杯交互信号处理
        /// </summary>
        /// <param name="cupNo">杯号</param>
        /// <param name="signal">信号</param>
        /// <param name="teachName">工艺名称编号</param>
        private void CupSignalProcess(int cupNo, int signal, FC_4_Receive_Zone zone)
        {


            _ = Task.Run(async () =>
            {


                try
                {
                    // 逻辑
                    // 1.signal=1，请求开盖
                    // 2.signal=2，请求关盖
                    // 3.signal=4，申请加药

                    //查找主副杯
                    var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);

                    //复位信号
                    int group = cupNo - this._startCupNo;





                    if (signal == 1 || signal == 2)
                    {
                        //等待锁止下信号为1后提交任务，超时报警但不退出
                        await WaitLockAndEnqueue(cupNo, zone, 0);

                        if (signal == 1)
                        {
                            await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueOpenLidAsync(cupNo);
                        }
                        else if (signal == 2)
                        {
                            await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueCloseLidAsync(cupNo);
                        }


                    }
                    else if (signal == 4)
                    {
                        int cupNum = mainInfo.CupNum;
                        var (dyeType, technologyName) = My_Tool.CupAuxiliary.GetCurrentStepDyeType(cupNum);

                        if ((technologyName.Contains("加") && technologyName != "加水"))
                        {
                            //申请加药
                            await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueAddChemicalAsync(cupNo, dyeType, false, false);
                        }

                    }

                }
                catch (Exception ex)
                {
                    var useName = My_Tool.CupAuxiliary.GetIsUseing(cupNo);
                    Logger.Error($"CupSignalProcess: {useName}号杯信号处理异常", ex);
                }
                finally
                {
                    lock (this._zoneLock)
                    {


                        WriteSingleProtocolItem(zone.CupCoverSignal);
                    }
                }
            });
        }


        /// <summary>
        /// 模块重启处理
        /// </summary>
        /// <param name="cupNo">杯号</param>
        private void ModuleRestartProcess(int cupNo, FC_4_Receive_Zone zone)
        {

            try
            {
                var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                int group = mainInfo.CupNum - this._startCupNo;

                Logger.Error($"{mainInfo.CupNum}号杯收到重启信号");
                if (mainInfo.IsUsing == 1)
                {
                    lock (this._zoneLock)
                    {
                        this._send[group].Base.StartStopReady.SetValue((ushort)7);
                        WriteSingleProtocolItem(this._send[group].Base.StartStopReady, (ushort)this._send[group].Base.StartStopReady.Value);
                    }

                    var useinfo = mainInfo;
                    if (useinfo.Statues == "滴液")
                    {
                    }
                    else if (useinfo.Statues == "待机")
                    {
                        string dyeingCode = useinfo.DyeingCode ?? "";
                        int stepNum = 0;
                        int.TryParse(useinfo.StepNum, out stepNum);

                        StepInfo stepInfo;
                        if (CupAuxiliary.WashCupDic.ContainsKey(dyeingCode))
                        {
                            // 洗杯工艺
                            stepInfo = CupAuxiliary.GetNextWashCupStepInfo(dyeingCode, Math.Max(0, stepNum - 1));
                        }
                        else
                        {
                            // 染色工艺
                            int headID = useinfo.HeadID ?? 0;

                            stepInfo = CupAuxiliary.GetNextStepInfo(headID, Math.Max(0, stepNum - 1));
                        }
                        if (Convert.ToInt16(zone.WaitData.Value) == 0 &&
                            Convert.ToInt16(zone.AllFinish.Value) == 0 &&
                            Convert.ToUInt16(zone.CupCoverSignal.Value) == 0 &&
                            Convert.ToUInt16(zone.PutClothConfirm.Value) == 0)
                            _ = CheckNeedRobotTask(useinfo.CupNum, stepInfo, true);
                    }

                }
                lock (this._zoneLock)
                {
                    WriteSingleProtocolItem(zone.ModuleRestartSignal);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ModuleRestartProcess", ex);
            }


        }

        /// <summary>
        /// 读取翻转缸的版本信息，返回HMI版本、卡1软件版本、卡2软件版本
        /// </summary>
        /// <returns>Tuple: HMI版本, 卡1软件版本, 卡2软件版本</returns>
        public Tuple<string, string, string> ReadVersionInfo()
        {
            try
            {
                int regCount = GetVersionRegisterCount(_version);
                ushort[] regs = this._modbus.ReadHoldingRegisters(this._unitId, (ushort)_version.TouchScreenVersion1.Address, (ushort)regCount);
                int idx = 0;
                _version.TouchScreenVersion1?.SetValue(regs[idx++]);
                _version.TouchScreenVersion2?.SetValue(regs[idx++]);
                _version.Module1HardwareVersion?.SetValue(regs[idx++]);
                _version.Module1SoftwareVersion?.SetValue(regs[idx++]);
                _version.Module2HardwareVersion?.SetValue(regs[idx++]);
                _version.Module2SoftwareVersion?.SetValue(regs[idx++]);

                // 组装HMI版本（5位数，分段为1.2.2位）
                int ver1 = Convert.ToInt32(_version.TouchScreenVersion1?.Value ?? 0);
                int ver2 = Convert.ToInt32(_version.TouchScreenVersion2?.Value ?? 0);
                string ver1Str = ver1.ToString("D2");
                string ver2Str = ver2.ToString("D3");
                string version5 = ver1Str + ver2Str;
                string hmiVersion = $"{version5.Substring(0, 1)}.{version5.Substring(1, 2)}.{version5.Substring(3, 2)}";


                // 卡1软件版本
                ushort card1Ver = Convert.ToUInt16(_version.Module1SoftwareVersion?.Value ?? 0);
                string card1Version = card1Ver.ToString();

                // 卡2软件版本
                ushort card2Ver = Convert.ToUInt16(_version.Module2SoftwareVersion?.Value ?? 0);
                string card2Version = card2Ver.ToString();

                return Tuple.Create(hmiVersion, card1Version, card2Version);
            }
            catch (Exception ex)
            {
                Logger.Error("读取版本信息失败", ex);
                return Tuple.Create("未知", "未知", "未知");
            }
        }

        /// <summary>
        /// 读取所有工位的接收区数据，并填充到各ProtocolItem的Value属性。
        /// 每个工位按顺序批量读取寄存器，分别对应各数据点。
        /// 增加异常处理，保证通讯异常时不会导致程序崩溃。
        /// 增加内部重试机制，提升偶发性通讯异常的容错能力。
        /// </summary>
        private int ReadAllReceive(bool onlyRead)
        {

            try
            {
                lock (_zoneLock)
                {
                    // 一次性读取全部120个寄存器（500~619）
                    for (int i = 0; i < 4; i++)
                    {
                        var zone = this._receiveAll.GetZoneFromGroup(i);

                        ushort[] regs = this._modbus.ReadHoldingRegisters(this._unitId, (ushort)zone.WaitData.Address, 20);

                        // 依次赋值给每个ProtocolItem
                        zone.WaitData.SetValue(regs[0]);
                        zone.AllFinish.SetValue(regs[1]);
                        zone.ActualTemp.SetValue(regs[2]);
                        //zone.ActualTemp2.SetValue(regs[3]);
                        zone.CurrentProcess.SetValue(regs[4]);
                        zone.CurrentStatus.SetValue(regs[5]);
                        // zone.CurrentStatus2.SetValue(regs[6]);
                        zone.CurrentStepNum.SetValue(regs[7]);
                        zone.CurrentKeepWarmTime.SetValue(regs[8]);
                        zone.CupCoverSignal.SetValue(regs[9]);
                        //zone.CupCoverSignal2.SetValue(regs[10]);
                        zone.HistoryStatus.SetValue(regs[11]);
                        // zone.HistoryStatus2.SetValue(regs[12]);
                        zone.LockUpStatus.SetValue(regs[13]);
                        zone.AlarmTip.SetValue(regs[14]);
                        zone.PutClothConfirm.SetValue(regs[15]);
                        zone.ModuleRestartSignal.SetValue(regs[16]);
                        zone.DrainageDown.SetValue(regs[17]);
                        zone.SafeOpeningTemp.SetValue(regs[18]);
                        zone.Reserved3.SetValue(regs[19]);
                        _ = ReceiveDataProcess(onlyRead, zone, i);
                    }
                }


                Thread.Sleep(500); // 短暂延迟，模拟处理时间，避免过快执行后续指令可能导致的竞态条件

                return 0;
            }
            catch (Exception ex)
            {
                Logger.Error($"批量读取接收区寄存器失败", ex);
                Thread.Sleep(500);
                return -1;
            }




        }

        /// <summary>
        /// 处理所有接收区数据，将每组数据分发到杯，并触发数据接收事件。
        /// 包括：
        /// 1. 推送杯主要数据到上层（如界面/业务层）
        /// 2. 解析报警提示，输出日志
        /// 3. 处理等待数据、全部完成、机械手信号、放布确认、模块重启等业务指令
        /// </summary>
        private async Task ReceiveDataProcess(bool onlyRead, FC_4_Receive_Zone zone, int group)
        {
            await Task.Run(() =>
                {


                    // 计算主/副杯的全局索引（如1/2、3/4...）
                    int cupIndex1 = this._startCupNo + group;


                    // 1. 推送主杯数据
                    var cupData1 = new Dictionary<string, object>
                        {
                        { "CupIndex", cupIndex1 },
                        { "CurrentStatus", ConvertCodeToStatues(Convert.ToUInt16(zone.CurrentStatus.Value)) },
                        { "ActualTemp", zone.ActualTemp.Value },
                        { "LockSignal", zone.LockUpStatus.Value },
                        { "CurrentStepNo", zone.CurrentStepNum.Value },
                        { "HoldingTime", zone.CurrentKeepWarmTime.Value },
                        { "TechnologyName", ConvertCodeToTechnologyName(Convert.ToUInt16(zone.CurrentProcess.Value)) },
                        { "DrainageDown",zone.DrainageDown.Value},
                        { "SafeOpeningTemp",zone.SafeOpeningTemp.Value}
                        };

                    OnCupDataReceived?.Invoke(cupIndex1, cupData1);

                    // Thread.Sleep(50); // 短暂延迟，确保数据推送完成后再处理指令，避免竞态条件
                    if (onlyRead) return;



                    // ----------- 临时变量快照所有信号 -----------
                    ushort moduleRestartSignal = Convert.ToUInt16(zone.ModuleRestartSignal.Value);
                    ushort alarmTip = Convert.ToUInt16(zone.AlarmTip.Value);
                    ushort allFinish = Convert.ToUInt16(zone.AllFinish.Value);
                    ushort cupCoverSignal1 = Convert.ToUInt16(zone.CupCoverSignal.Value);
                    ushort putClothConfirm = Convert.ToUInt16(zone.PutClothConfirm.Value);
                    ushort waitData = Convert.ToUInt16(zone.WaitData.Value);
                    ushort stepNum = Convert.ToUInt16(zone.CurrentStepNum.Value);
                    ushort historyStatus1 = Convert.ToUInt16(zone.HistoryStatus.Value);

                    // 3. 处理模块重启信号（主杯）
                    _moduleRestartHandler.TryProcess(
                        cupIndex1,
                        moduleRestartSignal,
                        () => ModuleRestartProcess(cupIndex1, zone)
                    );

                    // 4. 解析报警提示
                    _alarmTipHandler.TryProcess(
                        cupIndex1,
                        alarmTip,
                        () => ParseAlarmTip(cupIndex1, alarmTip, zone)
                    );

                    // 5. 处理全部完成指令（主杯）
                    _allFinishHandler.TryProcess(
                        cupIndex1,
                        allFinish,
                        () => AllFinishProcess(cupIndex1, zone)
                    );

                    // 6. 处理机械手交互信号（主/副杯）
                    ushort capturedSignal = 0;
                    if (cupCoverSignal1 != 0)
                    {
                        capturedSignal = cupCoverSignal1;
                    }

                    int capturedCupNo = cupIndex1;
                    _cupSignalHandler.TryProcess(
                        capturedCupNo,
                        capturedSignal,
                        () => CupSignalProcess(capturedCupNo, capturedSignal, zone)
                    );

                    // 7. 处理放布/出布确认（主杯）
                    _putClothHandler.TryProcess(
                        cupIndex1,
                        putClothConfirm,
                        () => PutOrOutCloth(cupIndex1, putClothConfirm, zone)
                    );

                    // 8. 处理等待数据指令（主杯）
                    _waitDataHandler.TryProcess(
                        cupIndex1,
                        waitData,
                        () => WaitDataProcess(cupIndex1, stepNum, zone)
                    );

                    // 9. 历史状态处理
                    ushort capturedHistorySignal = historyStatus1 == 1 ? (ushort)1 : (ushort)0;
                    _historyHandler.TryProcess(
                        cupIndex1,
                        capturedHistorySignal,
                        () => HistoryStateSendWashAsync(cupIndex1)
                    );

                });

        }

        private void PutOrOutCloth(int cupIndex1, ushort signal, FC_4_Receive_Zone zone)
        {

            var (mainInfo, subInfo) = My_Tool.CupAuxiliary.GetMSCupInfo(cupIndex1);
            var tc = string.Empty;
            if (mainInfo.IsUsing == 1)
            {
                tc = mainInfo.TechnologyName;
            }


            if (tc == "出布" || tc == "放布")
            {
                bool isPutCloth = tc == "放布" ? true : false;
                My_Tool.CupAuxiliary.UpdateHaveCloth(cupIndex1, isPutCloth ? 1 : 0);

            }
            _ = PutClothConfirm(cupIndex1, tc);



        }


        private void HistoryStateSendWashAsync(int cupNo)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    FC_4_Receive_Zone zone = this._receiveAll.GetZoneFromCupNo(cupNo, this._startCupNo);
                    if (Convert.ToInt16(zone.HistoryStatus.Value) == 1 && Convert.ToInt16(zone.CurrentStatus.Value) == 0)
                    {
                        _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                        {
                            [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNo}号杯收到预洗杯信号"
                        }, DateTime.Now);
                        await Task.Delay(300000);

                        // 只有在状态依然满足时才执行
                        if (Convert.ToInt16(zone.HistoryStatus.Value) == 1 && Convert.ToInt16(zone.CurrentStatus.Value) == 0)
                        {
                            //并且批次资料中不能有该杯或者副杯
                            var (mainCup, subCup) = My_Tool.CupAuxiliary.GetCupPair(cupNo);
                            var filter = $"{DROP_HEAD.CupNum} = {mainCup} OR {DROP_HEAD.CupNum} = {subCup}";
                            var datarows = SqlServer.Select(DROP_HEAD.TableName, filter);

                            if (datarows == null || datarows.Rows.Count == 0)
                                await SendWashAsync(cupNo, My_Tool.CupAuxiliary.StopWashCupType);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("HistoryStateSendWashAsync", ex);
                }
            });

        }

        /// <summary>
        /// 启动翻转缸通讯线程
        /// </summary>
        public async Task StartCom(CtCupArea ctCupArea, bool isRestart = false)
        {
            _isRunning = true;
            this._ctCupArea = ctCupArea;

        label1:
            int retryCount = 0;
            const int maxRetry = 5;
            const int retryDelayMs = 1000;

            try
            {
                await Task.Delay(1000); // 启动前短暂等待，确保系统其他部分就绪
                while (retryCount < maxRetry)
                {
                    try
                    {
                        Connect();

                        if (this._modbus == null || !this._modbus.IsConnected)
                        {
                            throw new Exception("Modbus未连接");
                        }

                        if (this.ReadAllReceive(true) == -1)
                        {
                            throw new Exception("读取工位数据失败");
                        }

                        await SyncAllCoverStatus();

                        // 启动后补偿所有正在运行的杯
                        for (int group = 0; group < 4; group++)
                        {
                            try
                            {
                                int cupIndex1 = this._startCupNo + group; // 主杯
                                FC_4_Receive_Zone zone = this._receiveAll.GetZoneFromGroup(group);
                                var (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupIndex1);

                                // 主杯在用
                                if (mainInfo.IsUsing == 1)
                                {
                                    var useinfo = mainInfo;
                                    if (useinfo.Statues == "滴液" && isRestart)
                                    {
                                        Logger.Error($"杯号{useinfo.CupNum}在滴液状态时重启软件，重启后把当前杯处理完成");
                                        _ = Task.Run(async () =>
                                        {
                                            AllFinishProcess(useinfo.CupNum, zone);
                                            await SendStopAsync(useinfo.CupNum, false);
                                        });

                                    }
                                    else if (useinfo.DyeingCode == My_Tool.CupAuxiliary.PreWashCupType)
                                    {
                                        var useName = My_Tool.CupAuxiliary.GetIsUseing(useinfo.CupNum);
                                        Logger.Error($"杯号{useinfo.CupNum}在前洗杯时重启软件，重启后把当前杯处理完成");

                                        CupAuxiliary.ClearBatchCupData(mainInfo.CupNum);
                                    }
                                    else if (useinfo.Statues != "待机")
                                    {
                                        string dyeingCode = useinfo.DyeingCode ?? "";
                                        int stepNum = 0;
                                        int.TryParse(useinfo.StepNum, out stepNum);

                                        StepInfo stepInfo;
                                        if (CupAuxiliary.WashCupDic.ContainsKey(dyeingCode))
                                        {
                                            // 洗杯工艺
                                            stepInfo = CupAuxiliary.GetNextWashCupStepInfo(dyeingCode, Math.Max(0, stepNum - 1));
                                        }
                                        else
                                        {
                                            // 染色工艺
                                            int headID = useinfo.HeadID ?? 0;
                                            if (headID == 0) continue;
                                            stepInfo = CupAuxiliary.GetNextStepInfo(headID, Math.Max(0, stepNum - 1));
                                        }
                                        if (Convert.ToInt16(zone.WaitData.Value) == 0 &&
                                            Convert.ToInt16(zone.AllFinish.Value) == 0 &&
                                            Convert.ToUInt16(zone.CupCoverSignal.Value) == 0 &&
                                            Convert.ToUInt16(zone.PutClothConfirm.Value) == 0 &&
                                            Convert.ToUInt16(zone.ModuleRestartSignal.Value) == 0)
                                            _ = CheckNeedRobotTask(useinfo.CupNum, stepInfo, true);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"启动后补偿杯号{this._startCupNo + group}异常", ex);
                            }
                        }

                        // 主循环
                        while (true)
                        {
                            if (this._modbus == null || !this._modbus.IsConnected)
                            {
                                Logger.Error($"{_areaName} Modbus连接断开，尝试自动重连...");
                                try
                                {
                                    Connect();
                                    if (this._modbus == null || !this._modbus.IsConnected)
                                    {
                                        throw new Exception("Modbus自动重连失败");
                                    }
                                    Logger.Info($"{_areaName} Modbus自动重连成功。");
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"{_areaName} Modbus自动重连异常: {ex.Message}", ex);
                                    throw; // 保持原有重试机制
                                }
                            }
                            if (this.ReadAllReceive(false) == -1)
                            {
                                throw new Exception("读取工位数据失败");
                            }

                        }


                    }
                    catch (Exception ex)
                    {
                        if (My_AutomaticModule.CupCommManager.Instance.ShouldStartComm(this._ctCupArea))
                        {
                            retryCount++;

                            Logger.Error($"{_areaName}翻转缸通讯异常，第{retryCount}次重试: {ex.Message}", ex);
                            Disconnect();
                            Thread.Sleep(retryDelayMs);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (My_AutomaticModule.CupCommManager.Instance.ShouldStartComm(this._ctCupArea))
                {
                    // 弹窗询问是否重试
                    var btn = await My_Tool.MessageEventManager.Instance.RequestShowMessageAsync(
                        $"{_areaName}翻转缸通讯失败",
                        $"{_areaName}翻转缸通讯连续失败，已超过最大重试次数，退出通讯线程，请修复后重试",
                        new[] { "重试" },
                        "重试"
                    );
                    if (btn == "重试")
                    {
                        goto label1;
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int cupNo = _startCupNo + i;
                        var dt = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {cupNo}");
                        if (dt.Rows.Count > 0)
                        {
                            var states = dt.Rows[0][CUP_DETAILS.Statues]?.ToString();
                            var cupData2 = new Dictionary<string, object>
                            {
                                { "CupIndex", cupNo },
                                { "CurrentStatus", states }
                            };
                            OnCupDataReceived?.Invoke(cupNo, cupData2);
                        }
                    }
                }
            }
            finally
            {
                Disconnect();
                _isRunning = false;
            }
        }

        public async Task<DyeingResult> DyeingStartAsync(int cupNum, int cupChoice)
        {
            return await Task.Run(async () =>
            {
                var result = new DyeingResult { CupNo = cupNum };
                try
                {


                    // 1. 获取主副杯详细信息（优先主杯）
                    var (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupNum);








                    FC_4_Receive_Zone zone = this._receiveAll.GetZoneFromCupNo(cupNum, this._startCupNo);

                    var mainOk = true;

                    if (mainInfo.IsUsing == 1)
                    {
                        var states1 = Convert.ToUInt16(zone.CurrentStatus.Value);
                        if (states1 != 0 && states1 != 5)
                            mainOk = false;
                    }



                    if (!mainOk)
                    {
                        result.Code = My_Tool.Result.ResultCode.Success;
                        result.Message = $"杯号{cupNum}已经启动，禁止重复启动";
                        return result;
                    }





                    var info = mainInfo;

                    if (info.CupNum == 0)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.CupNo = cupNum;
                        result.Message = $"{cupNum}杯不存在";
                        result.Exception = new Exception($"{cupNum}杯不存在");
                        return result;
                    }

                    string dyeingCode = info.DyeingCode ?? "";
                    if (string.IsNullOrEmpty(dyeingCode))
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.CupNo = cupNum;
                        result.Message = $"{cupNum}号杯未找到染色工艺代码";
                        result.Exception = new Exception($"{cupNum}号杯未找到染色工艺代码");
                        return result;
                    }



                    var headID = info.HeadID ?? 0;
                    var dropHeadInfo = SqlServer.Select(DROP_HEAD.TableName, $"{DROP_HEAD.MyID} = {headID}");
                    if (dropHeadInfo.Rows.Count == 0)
                    {
                        result.Code = My_Tool.Result.ResultCode.Exception;
                        result.CupNo = cupNum;
                        result.Message = $"{cupNum}号杯工艺代码[{dyeingCode}]对应的头ID[{headID}]在数据库中未找到";
                        result.Exception = new Exception($"{cupNum}号杯工艺代码[{dyeingCode}]对应的头ID[{headID}]在数据库中未找到");
                        return result;
                    }
                    var formulaCode = dropHeadInfo.Rows[0][DROP_HEAD.FormulaCode]?.ToString() ?? "";
                    var versionNum = Convert.ToInt32(dropHeadInfo.Rows[0][DROP_HEAD.VersionNum] ?? 0);

                    int totalStepNum = SqlServer.Select(DYE_DETAILS.TableName, $"{DYE_DETAILS.CupNum} = '{cupNum}'").Rows.Count;

                    // 2. 同步cup_details表步号 并启动温度记录
                    var updateDict = new Dictionary<string, object>
                    {
                        { CUP_DETAILS.StartTime, DateTime.Now },
                        { CUP_DETAILS.TotalStep, totalStepNum },
                        { CUP_DETAILS.RecordIndex, 0 },
                        { CUP_DETAILS.StepNum, 0 },
                        { CUP_DETAILS.Cooperate, 0 },
                        { CUP_DETAILS.CurrentStepFinish,0 }
                    };



                    string where = $"{CUP_DETAILS.CupNum}=@CupNum";
                    var whereParam = new SqlParameter("@CupNum", mainInfo.CupNum);
                    My_DataBase.SqlServer.Update(CUP_DETAILS.TableName, updateDict, where, whereParam);

                    var area = CupCommManager.Instance.FindCupAreaByCupNum(mainInfo.CupNum);
                    if (area != null)
                    {
                        area.OnCupDataReceived(mainInfo.CupNum);
                    }
                    //4.  启动温度记录
                    var recorder = SmartColor.My_File.CupTempRecorder.Get(mainInfo.CupNum);
                    recorder.SetCylinderComm(this);
                    recorder.StartRecord();




                    int waitMs = 0;
                    const int maxWaitMs = 60000;
                    const int pollInterval = 500;
                    while (waitMs < maxWaitMs)
                    {
                        // 3. 填充发送区
                        int sendIdx = (cupNum - this._startCupNo);
                        var send = this._send[sendIdx];
                        lock (this._zoneLock)
                        {
                            send.Base.StartStopReady?.SetValue(1);
                            FillProcessNameToCodes(dyeingCode, send.Base);

                            send.Base.TotalStepNum?.SetValue(totalStepNum);
                            ResetSendParamToZero(send.Param);
                            WriteToHMI(send.Base);
                        }

                        bool sucess = false;
                        for (int i = 0; i < 5; i++)
                        {
                            await Task.Delay(pollInterval); // 短暂等待，确保HMI处理指令
                            waitMs += pollInterval;                        // 直接用zone里的最新值
                            ushort waitCurrentStatus = Convert.ToUInt16(zone.CurrentStatus.Value);


                            if (waitCurrentStatus == 1) // 1=运行中
                            {
                                sucess = true;
                                result.Code = My_Tool.Result.ResultCode.Success;
                                result.Message = "染色命令下发成功，HMI已进入染色状态";
                                break;
                            }


                        }
                        if (sucess) break;
                    }
                    if (waitMs >= maxWaitMs)
                    {
                        throw new Exception($"{cupNum}号配液杯下发染色命令后等待HMI进入染色状态超时");
                    }



                    // 5. 成功返回
                    result.Code = My_Tool.Result.ResultCode.Success;
                    result.Message = $"{cupNum}号杯染色启动完成";

                    // 6. 记录日志
                    var useName = My_Tool.CupAuxiliary.GetIsUseing(cupNum);
                    var dt = DateTime.Now;
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{useName}号杯启动染固色指令"
                    }, dt);
                }
                catch (Exception ex)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "染色启动异常: " + ex.Message;
                    result.Exception = ex;
                    Logger.Error("DyeingStartAsync", ex);
                }
                finally
                {
                    ResetAllSignalHandlers(cupNum);
                }
                return result;
            });
        }

        public double GetCupTemp(int cupNum)
        {
            FC_4_Receive_Zone zone = this._receiveAll.GetZoneFromCupNo(cupNum, this._startCupNo);
            return Convert.ToInt16(zone.ActualTemp.Value) / 10;
        }

        public async Task PutClothConfirm(int cupNo, string tc, bool stayTank = false)
        {
            int group = cupNo - this._startCupNo;
            FC_4_Receive_Zone zone = this._receiveAll.GetZoneFromGroup(group);

            try
            {
                // 1. 获取主副杯详细信息
                var (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupNo);
                int mainCup = mainInfo.CupNum;


                // 2. 查找分区




                // 3. 判断放布确认值
                PutClothConfirmResult confirmValue = new PutClothConfirmResult();
                switch (tc)
                {
                    case "放布":
                        confirmValue = CupAuxiliary.GetPutClothConfirmValue(cupNo);
                        break;
                    case "出布":
                        confirmValue = CupAuxiliary.GetOutClothConfirmValue(cupNo);
                        break;
                    default:
                        {
                            if (mainInfo.IsUsing == 1)
                            {
                                confirmValue = new PutClothConfirmResult
                                {
                                    sureKey = 3,
                                    dyeType = mainInfo.DyeType ?? 0,
                                    headID = mainInfo.HeadID ?? 0,
                                    stepNo = int.TryParse(mainInfo.StepNum?.ToString(), out var stepNum) ? stepNum : 0,
                                    dyeingCode = mainInfo.DyeingCode ?? ""
                                };
                            }

                            else
                            {
                                return;
                            }
                        }

                        break;
                }



                if (stayTank)
                {
                    confirmValue.sureKey = 3;
                }
                switch (confirmValue.sureKey)
                {
                    case 0:
                        // 另一杯有滴液任务
                        break;
                    case 1:
                        // 自动放布/出布
                        if (tc == "放布")
                            await SmartColor.My_AutomaticModule.CupRobotTask.EnqueuePutClothAsync(cupNo, confirmValue.dyeType);
                        else if (tc == "出布")
                            await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueOutClothAsync(cupNo, confirmValue.dyeType);
                        break;
                    case 2:
                        // 先判断盖子是否打开
                        if ((mainInfo.IsUsing == 1 && mainInfo.CoverStatus == 1) || (subInfo.IsUsing == 1 && subInfo.CoverStatus == 1))
                        {
                            if (tc == "放布")
                                await SmartColor.My_AutomaticModule.CupRobotTask.EnqueuePutClothAsync(cupNo, confirmValue.dyeType);
                            else if (tc == "出布")
                                await SmartColor.My_AutomaticModule.CupRobotTask.EnqueueOutClothAsync(cupNo, confirmValue.dyeType);

                        }
                        else
                        {
                            if (mainInfo.IsUsing == 1 && mainInfo.CoverStatus == 2)
                            {
                                if (tc == "放布")
                                {
                                    if (mainInfo.HaveCloth == 0)
                                    {
                                        var recorder = SmartColor.My_File.CupTempRecorder.Get(mainInfo.CupNum);
                                        recorder.SetCylinderComm(this);
                                        recorder.RecordStep("放布");
                                    }
                                }
                                else if (tc == "出布")
                                {
                                    if (mainInfo.HaveCloth == 1)
                                    {
                                        var recorder = SmartColor.My_File.CupTempRecorder.Get(mainInfo.CupNum);
                                        recorder.SetCylinderComm(this);
                                        recorder.RecordStep("出布");
                                    }
                                }
                            }

                        }

                        // 显示确定键
                        lock (this._zoneLock)
                        {
                            this._send[group].Param.ConfirmButton.SetValue((ushort)5);
                            WriteSingleProtocolItem(this._send[group].Param.ConfirmButton, (ushort)this._send[group].Param.ConfirmButton.Value);
                        }

                        if (mainInfo.IsUsing == 1)
                        {
                            if (tc == "放布")
                                MessageEventManager.Instance.RequestLoopSpeak($"{mainCup}号配液杯入布", $"{mainCup}号杯请放布");
                            else if (mainInfo.HaveCloth == 1)
                                MessageEventManager.Instance.RequestLoopSpeak($"{mainCup}号配液杯出布", $"{mainCup}号杯请出布");
                        }

                        break;
                    case 3:
                        // 发送下一步工艺
                        CupAuxiliary.StepInfo nextProcess;
                        if (tc == "出布")
                        {
                            bool isWash = !string.IsNullOrEmpty(confirmValue.dyeingCode) && CupAuxiliary.WashCupDic.ContainsKey(confirmValue.dyeingCode);
                            if (isWash)
                                nextProcess = CupAuxiliary.GetNextWashCupStepInfo(confirmValue.dyeingCode, confirmValue.stepNo);
                            else
                                nextProcess = CupAuxiliary.GetNextStepInfo(confirmValue.headID, confirmValue.stepNo);
                        }
                        else
                        {
                            nextProcess = CupAuxiliary.GetNextStepInfo(confirmValue.headID, confirmValue.stepNo);
                        }

                        if (nextProcess.StepNum != 0)
                        {
                            await RunTableMan.InsertAsync(new Dictionary<string, object>
                            {
                                [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNo}号配液杯出入布/取小样/测PH触发发送下一步信号"
                            }, DateTime.Now);
                            _ = SendNextStep(cupNo, nextProcess);
                        }
                        else
                        {
                            //发送出布完成消息
                            _ = SendShowSure(cupNo, 1);

                        }


                        if (mainInfo.IsUsing == 1)
                        {
                            if (tc == "出布")
                                MessageEventManager.Instance.RequestStopLoopSpeak($"{mainCup}号配液杯出布");
                            else if (tc == "放布")
                                MessageEventManager.Instance.RequestStopLoopSpeak($"{mainCup}号配液杯入布");
                            else if (tc == "取小样")
                            {

                                MessageEventManager.Instance.RequestStopLoopSpeak($"{mainCup}号配液杯取小样");
                            }

                            else if (tc == "测PH")
                            {

                                MessageEventManager.Instance.RequestStopLoopSpeak($"{mainCup}号配液杯测PH");
                            }

                        }

                        break;
                }




            }
            catch (Exception ex)
            {
                Logger.Error("PutClothConfirm", ex);
            }
            finally
            {
                lock (this._zoneLock)
                {

                    WriteSingleProtocolItem(zone.PutClothConfirm);
                }
            }


        }

        public async Task<DyeingResult> RequestDropLiquidAsync(int cupNum, int cupChoice)
        {
            return await Task.Run(async () =>
            {
                var result = new DyeingResult { CupNo = cupNum };
                try
                {

                    // 1. 计算分区和主/副杯索引
                    var (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupNum);
                    int group = cupNum - this._startCupNo;


                    FC_4_Receive_Zone zone = this._receiveAll.GetZoneFromCupNo(cupNum, this._startCupNo);


                    //4.  清空温度记录

                    CupTempRecorder.ClearFiles(mainInfo.CupNum);


                    // 2. 读取主/副杯当前状态和历史状态
                    ushort mainCurrentStatus = Convert.ToUInt16(zone.CurrentStatus.Value);
                    ushort mainHistoryStatus = Convert.ToUInt16(zone.HistoryStatus.Value);




                    var mainHaveCloth = Convert.ToUInt16(mainInfo.HaveCloth);


                    // 当前杯的状态和历史状态
                    ushort currentStatus = mainCurrentStatus;
                    ushort historyStatus = mainHistoryStatus;

                    // 3. 非待机状态直接返回异常
                    if (currentStatus != 0) // 0=待机
                    {
                        throw new Exception($"当前杯[{cupNum}]状态为[{ConvertCodeToStatues(currentStatus)}]，不能滴液");
                    }

                    // 4. 判断是否需要前洗杯
                    // 只要主/副杯有一个历史状态为1，并且状态不为下线就需要洗杯


                    var send = this._send[group];

                    bool needPreWash = (mainCurrentStatus == 0 && (mainHistoryStatus == 1 || mainHaveCloth == 1 || mainInfo.CoverStatus == 1));

                    if (needPreWash)
                    {
                        // 发送前洗杯命令到HMI                      
                        await SendWashAsync(cupNum, My_Tool.CupAuxiliary.PreWashCupType);

                        result.Code = My_Tool.Result.ResultCode.Failure;
                        result.Message = $"主/副杯历史状态有滴液，需前洗杯";
                        return result;

                    }
                    //7 记录日志
                    var useName = My_Tool.CupAuxiliary.GetIsUseing(cupNum);
                    var dt = DateTime.Now;
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{useName}号杯启动滴液指令"
                    }, dt);

                    // 8. 等待HMI变为滴液状态（状态码5），超时自动失败


                    int waitMs = 0;
                    const int maxWaitMs = 60000;
                    const int pollInterval = 500;
                    while (waitMs < maxWaitMs)
                    {
                        lock (this._zoneLock)
                        {
                            // 6. 发送滴液命令
                            send.Base.StartStopReady.SetValue(3); // 3=滴液
                            FillProcessNameToCodes(string.Empty, send.Base);
                            send.Base.TotalStepNum.SetValue(0);

                            ResetSendParamToZero(send.Param);
                            WriteToHMI(send.Base);
                        }

                        bool sucess = false;
                        for (int i = 0; i < 5; i++)
                        {
                            await Task.Delay(pollInterval);
                            waitMs += pollInterval;
                            // 直接用zone里的最新值
                            ushort waitCurrentStatus = Convert.ToUInt16(zone.CurrentStatus.Value);

                            currentStatus = waitCurrentStatus;
                            if (currentStatus == 5) // 5=滴液
                            {
                                sucess = true;
                                result.Code = My_Tool.Result.ResultCode.Success;
                                result.Message = "滴液命令下发成功，HMI已进入滴液状态";
                                break;
                            }

                        }

                        if (sucess)
                        {
                            break;
                        }
                    }
                    if (waitMs >= maxWaitMs)
                    {
                        throw new Exception($"{cupNum}号配液杯下发滴液命令后等待HMI进入滴液状态超时");
                    }

                }
                catch (Exception ex)
                {
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = "滴液请求异常: " + ex.Message;
                    result.Exception = ex;
                    Logger.Error("RequestDropLiquidAsync", ex);
                }
                finally
                {
                    ResetAllSignalHandlers(cupNum);
                }
                return result;
            });
        }

        public void RequestStop()
        {

            var dt = DateTime.Now;
            //1. 记录日志
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{this._startCupNo}号杯所属机台启动停止指令"
            }, dt);
        }

        public async Task SendAddChemicaFinish(int cupNum)
        {
            await Task.Run(() =>
            {
                try
                {
                    var (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupNum);
                    int mainCup = mainInfo.CupNum;
                    int group = mainCup - this._startCupNo;

                    FC_4_Receive_Zone zone = this._receiveAll.GetZoneFromCupNo(cupNum, this._startCupNo);


                    var send = this._send[group].Param;

                    lock (this._zoneLock)
                    {


                        WriteSingleProtocolItem(zone.CupCoverSignal);

                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("SendAddChemicaFinish", ex);
                }

            });

        }

        public async Task<bool> SendAddChemicaStart(int cupNum)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var (mainCup, subCup) = CupAuxiliary.GetCupPair(cupNum);
                    int group = (mainCup - this._startCupNo);
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNum}号杯发送加药启动"
                    }, DateTime.Now);
                    bool wr = false;
                    for (int i = 0; i < 5; i++)
                    {
                        FC_4_Receive_Zone zone = this._receiveAll.GetZoneFromCupNo(cupNum, this._startCupNo);
                        var send = this._send[group].Param;
                        lock (this._zoneLock)
                        {
                            WriteSingleProtocolItem(zone.CupCoverSignal);
                            send.AddDrugFinish.SetValue((ushort)3);
                            WriteSingleProtocolItem(send.AddDrugFinish, (ushort)send.AddDrugFinish.Value);
                        }

                        wr = await My_Tool.CupAuxiliary.WaitLockUpOk(cupNum);
                        if (!wr)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }


                    return wr ? true : false;
                }
                catch (Exception ex)
                {
                    Logger.Error("SendAddChemicaStart", ex);
                    return false;
                }
            });
        }


        public async Task SendNextStep(int cupNum, CupAuxiliary.StepInfo stepInfo)
        {
            var (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupNum);
            int mainCup = mainInfo.CupNum;
            int group = mainCup - this._startCupNo;
            var send = this._send[group].Param;

            FC_4_Receive_Zone zone = this._receiveAll.GetZoneFromCupNo(cupNum, this._startCupNo);

            try
            {
                if ((mainInfo.Statues == "下线"))
                {

                    ResetAllSignalHandlers(cupNum);
                    return;
                }


                // 数据库时间更新逻辑（区分洗杯/染色）

                DateTime now = DateTime.Now;
                int prevStepNum = stepInfo.StepNum - 1;

                if (mainInfo.IsUsing == 1)
                {
                    string mainDyeingCode = mainInfo.DyeingCode ?? "";
                    bool mainIsWash = !string.IsNullOrEmpty(mainDyeingCode) && CupAuxiliary.WashCupDic.ContainsKey(mainDyeingCode);
                    if (!mainIsWash)
                    {
                        if (prevStepNum >= 0)
                        {
                            My_DataBase.SqlServer.Update(DYE_DETAILS.TableName,
                                new Dictionary<string, object>
                                {
                                    { DYE_DETAILS.FinishTime, now },
                                    { DYE_DETAILS.Finish, 1 }
                                },
                                $"{DYE_DETAILS.HeadID} = @headID AND {DYE_DETAILS.StepNum} = @currentStepNum ",
                                new SqlParameter("@headID", mainInfo.HeadID),
                                new SqlParameter("@currentStepNum", prevStepNum)
                            );
                        }
                        My_DataBase.SqlServer.Update(DYE_DETAILS.TableName,
                            new Dictionary<string, object>
                            {
                                { DYE_DETAILS.StartTime, now },
                                { DYE_DETAILS.Finish,0 },
                                { DYE_DETAILS.FinishTime,null }
                            },
                            $"{DYE_DETAILS.HeadID} = @headID AND {DYE_DETAILS.StepNum} = @nextStepNum ",
                            new SqlParameter("@headID", mainInfo.HeadID),
                            new SqlParameter("@nextStepNum", stepInfo.StepNum)
                        );
                    }

                    SqlServer.Update(
                          CUP_DETAILS.TableName,
                          new Dictionary<string, object> {
                         { CUP_DETAILS.CurrentStepFinish,0 }
                          },
                          $"{CUP_DETAILS.CupNum} = @CupNo",
                          new SqlParameter("@CupNo", mainInfo.CupNum));

                    var area = CupCommManager.Instance.FindCupAreaByCupNum(mainInfo.CupNum);
                    if (area != null)
                    {
                        area.OnCupDataReceived(mainInfo.CupNum);
                    }

                    var recorder = SmartColor.My_File.CupTempRecorder.Get(mainInfo.CupNum);
                    recorder.SetCylinderComm(this);
                }






                // 循环下发，所有参数赋值都在循环内
                int waitMs = 0;
                const int maxWaitMs = 360000;
                const int pollInterval = 1000;
                ushort targetStepNum = (ushort)stepInfo.StepNum;
                ushort targetProcess = (ushort)ConvertTechnologyNameToCode(stepInfo.TechnologyName);

                while (waitMs < maxWaitMs)
                {

                    send.CurrentStepNum.SetValue((ushort)stepInfo.StepNum);
                    send.TechnologyNameNo.SetValue((ushort)ConvertTechnologyNameToCode(stepInfo.TechnologyName));
                    send.TargetTemp.SetValue((ushort)(stepInfo.Temp * 10));
                    send.TempRate.SetValue((ushort)(stepInfo.TempSpeed * 10));
                    send.KeepWarmTime.SetValue((ushort)(stepInfo.SetTime));
                    send.RotorRate.SetValue((ushort)(stepInfo.RotorSpeed * 10));
                    send.CurrentLiquidAmount.SetValue((ushort)(CupAuxiliary.ReturnMaxWeight(cupNum) * 100));
                    send.ConfirmButton.SetValue((ushort)0);
                    send.AddDrugFinish.SetValue((ushort)0);
                    send.ReceiveFinish.SetValue((ushort)1);

                    WriteToHMI(send);
                    bool sure = false;
                    for (int i = 0; i < 5; i++)
                    {
                        await Task.Delay(pollInterval);
                        waitMs += pollInterval;

                        var (mainInfo2, subInfo2) = CupAuxiliary.GetMSCupInfo(cupNum);
                        bool mainUsing = mainInfo2.IsUsing == 1;


                        if (mainInfo2.Enable == 0)
                        {
                            return;
                        }

                        bool mainOk = true;
                        if (mainUsing)
                        {
                            ushort dbStepNum = Convert.ToUInt16(mainInfo2.StepNum);

                            ushort dbProcess = ConvertTechnologyNameToCode(mainInfo2.TechnologyName);
                            mainOk = (dbStepNum == targetStepNum && dbProcess == targetProcess);
                        }


                        if (mainUsing && mainOk)
                        {
                            sure = true;
                            break;
                        }

                    }

                    if (sure)
                    {
                        break;
                    }




                }


                string useName = My_Tool.CupAuxiliary.GetIsUseing(cupNum);


                if (waitMs >= maxWaitMs)
                {
                    throw new Exception($"SendNextStep: {useName}号杯发送步骤{stepInfo.StepNum}: 工艺[{stepInfo.TechnologyName}] 超时，重试已达上限({maxWaitMs}ms)，请检查通讯或HMI状态。");

                }


                var dt = DateTime.Now;
                _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                {
                    [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{useName}号杯发送步骤{stepInfo.StepNum}:工艺({stepInfo.TechnologyName})"
                }, dt);

                _ = CheckNeedRobotTask(cupNum, stepInfo);






            }
            catch (Exception ex)
            {
                Logger.Error("SendNextStep", ex);

            }
            finally
            {
                // 处理完成后移除cupNo，允许下次处理
                _waitDataProcessing.TryRemove(cupNum, out _);


                // 推荐：复位SignalEdgeHandler的状态
                _waitDataHandler.Reset(cupNum);
            }
        }

        public async Task SendOffLine(int cupNum, int cupChoice)
        {
            await Task.Run(() =>
            {
                try
                {
                    var (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupNum);


                    int group = (cupNum - this._startCupNo);
                    lock (this._zoneLock)
                    {
                        var send = this._send[group];
                        send.Base.StartStopReady?.SetValue(5);
                        send.Base.TotalStepNum?.SetValue(0);
                        FillProcessNameToCodes(string.Empty, send.Base);
                        ResetSendParamToZero(send.Param);
                        WriteToHMI(send.Base);
                    }

                    // 记录日志
                    var dt = DateTime.Now;
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNum}号杯启动下线指令"
                    }, dt);

                  //  CupAuxiliary.ClearBatchCupData(cupNum);


                }
                catch (Exception ex)
                {
                    Logger.Error("SendOffLine", ex);
                }

            });
        }

        public async Task SendOnLine(int cupNum, int cupChoice)
        {
            await Task.Run(() =>
            {
                try
                {


                    int group = (cupNum - this._startCupNo);
                    lock (this._zoneLock)
                    {
                        var send = this._send[group];
                        send.Base.StartStopReady?.SetValue(6);
                        send.Base.TotalStepNum?.SetValue(0);
                        FillProcessNameToCodes(string.Empty, send.Base);

                        ResetSendParamToZero(send.Param);
                        WriteToHMI(send.Base);
                    }



                    // 记录日志
                    var dt = DateTime.Now;
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNum}号杯启动上线指令"
                    }, dt);


                    SqlServer.Update(CUP_DETAILS.TableName,
                        new Dictionary<string, object>()
                        {
                            { CUP_DETAILS.Enable,1},
                            { CUP_DETAILS.IsUsing, 0 },
                            { CUP_DETAILS.DyeingCode, DBNull.Value },
                            { CUP_DETAILS.FormulaCode, DBNull.Value },
                            { CUP_DETAILS.CurrentWeight, 0 },
                            { CUP_DETAILS.TotalWeight, 0 },
                            { CUP_DETAILS.SetTime, 0 },
                            { CUP_DETAILS.DyeType, DBNull.Value },
                            { CUP_DETAILS.StartTime, DBNull.Value },
                            { CUP_DETAILS.StepStartTime, DBNull.Value },
                            { CUP_DETAILS.StepNum, DBNull.Value },
                            { CUP_DETAILS.TotalStep, DBNull.Value },
                            { CUP_DETAILS.TechnologyName, DBNull.Value },
                            { CUP_DETAILS.SetTemp, DBNull.Value },
                            { CUP_DETAILS.RecordIndex, 0 },
                            { CUP_DETAILS.Cooperate, 0 },
                            { CUP_DETAILS.Fail, 0 },
                            { CUP_DETAILS.HeadID, DBNull.Value },
                            { CUP_DETAILS.ReceptionTime, DBNull.Value },
                            { CUP_DETAILS.CurrentStepFinish,0 }
                        },
                        $"{CUP_DETAILS.CupNum} = {cupNum}");

                    var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNum);
                    if (area != null)
                    {
                        area.OnCupDataReceived(cupNum);
                    }
                    UpdateWaitList(cupNum, true);
                    ResetAllSignalHandlers(cupNum);
                }
                catch (Exception ex)
                {
                    Logger.Error("SendOnLine", ex);
                }
            });
        }

        public async Task SendPause()
        {
            await Task.Run(() =>
            {
                try
                {
                    lock (this._zoneLock)
                    {
                        _pause.PauseSignal?.SetValue((ushort)1);
                        WriteSingleProtocolItem(_pause.PauseSignal, (ushort)_pause.PauseSignal.Value);
                    }
                    // 记录日志
                    var dt = DateTime.Now;
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{this._startCupNo}号杯所属机台启动暂停指令"
                    }, dt);
                }
                catch (Exception ex)
                {
                    Logger.Error("SendPause", ex);
                }
            });
        }

        public async Task SendResume()
        {
            await Task.Run(() =>
            {
                try
                {
                    lock (this._zoneLock)
                    {
                        _pause.PauseSignal?.SetValue((ushort)2);
                        WriteSingleProtocolItem(_pause.PauseSignal, (ushort)_pause.PauseSignal.Value);
                    }
                    // 记录日志
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{this._startCupNo}号杯所属机台启动恢复指令"
                    }, DateTime.Now);
                }
                catch (Exception ex)
                {
                    Logger.Error("SendResume", ex);
                }
            });
        }

        public async Task SendShowSure(int cupNum, int choice)
        {
            await Task.Run(() =>
            {
                try
                {
                    lock (this._zoneLock)
                    {
                        var sendIdx = (cupNum - this._startCupNo);
                        var send = this._send[sendIdx].Param;
                        send.ConfirmButton.SetValue((ushort)choice);
                        WriteSingleProtocolItem(send.ConfirmButton, (ushort)send.ConfirmButton.Value);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("SendShowSure", ex);
                }
            });
        }

        public async Task SendStopAsync(int cupNum, bool needOffLine = true)
        {

            await Task.Run(async () =>
            {
                try
                {
                    if (!needOffLine)
                    {
                        this._retrying.Add(cupNum);
                    }
                    else
                    {
                        this._needStop.Add(cupNum);
                    }
                    int group = (cupNum - this._startCupNo);
                    lock (this._zoneLock)
                    {
                        var send = this._send[group];
                        send.Base.StartStopReady?.SetValue(2);
                        send.Base.TotalStepNum?.SetValue(0);
                        FillProcessNameToCodes(string.Empty, send.Base);

                        ResetSendParamToZero(send.Param);
                        WriteToHMI(send.Base);
                    }




                    // 1. 取消任务中心关于该杯的所有等待中的任务
                    var allTasks = SmartColor.My_RobotManager.RobotTaskManager.Instance.GetAllTasksSnapshot();
                    foreach (var taskObj in allTasks)
                    {
                        // 只处理业务任务
                        var type = taskObj.GetType();
                        var taskNameProp = type.GetProperty("TaskName");
                        if (taskNameProp != null)
                        {
                            string taskName = taskNameProp.GetValue(taskObj) as string;
                            if (!string.IsNullOrEmpty(taskName) && taskName.Contains(cupNum.ToString()))
                            {
                                SmartColor.My_RobotManager.RobotTaskManager.Instance.CancelTask(taskObj);
                            }
                        }
                    }

                    // 2. 删除该杯的所有播报
                    var nm = My_Tool.CupAuxiliary.GetIsUseing(cupNum);
                    string[] speakKeys = new string[]
                    {
                        $"{nm}号配液杯入布",
                        $"{nm}号配液杯出布",
                        $"{nm}号配液杯取小样",
                        $"{nm}号配液杯测PH"
                    };

                    foreach (var key in speakKeys)
                    {
                        SmartColor.My_Tool.MessageEventManager.Instance.RequestStopLoopSpeak(key);
                    }

                    var dt = DateTime.Now;
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNum}号杯启动停止指令"
                    }, dt);

                    // 3. 等待机台状态变为“停止中”(6)、“待机”(0)或“下线”(7)后再发下线
                    FC_4_Receive_Zone zone = this._receiveAll.GetZoneFromCupNo(cupNum, this._startCupNo);

                    var (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupNum);

                    int waitMs = 0;
                    const int maxWaitMs = 600000; // 最多等待60秒
                    const int pollInterval = 1000;
                    while (waitMs < maxWaitMs)
                    {
                        (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupNum);
                        var status1 = mainInfo.Statues;


                        // 检查主/副杯是否都已进入停止、待机或下线
                        bool mainOk = status1 == "待机" || status1 == "下线";


                        if (mainOk)
                            break;

                        await Task.Delay(pollInterval);
                        waitMs += pollInterval;

                    }


                    // 4. 发送下线指令
                    if (needOffLine)
                    {
                        await SendOffLine(cupNum, 0);
                        this._needStop.Remove(cupNum);
                    }

                    else
                    {
                        My_DataBase.SqlServer.Update(
                           CUP_DETAILS.TableName,
                           new Dictionary<string, object> {
                             { CUP_DETAILS.IsUsing,0 },
                            { CUP_DETAILS.CurrentWeight, 0 },
                            { CUP_DETAILS.TotalWeight,0 },
                            { CUP_DETAILS.SetTime,0 },
                            { CUP_DETAILS.DyeType, DBNull.Value },
                            { CUP_DETAILS.StartTime, DBNull.Value },
                            { CUP_DETAILS.StepStartTime, DBNull.Value },
                            { CUP_DETAILS.FormulaCode, DBNull.Value },
                            { CUP_DETAILS.DyeingCode, DBNull.Value },
                            { CUP_DETAILS.StepNum, 0 },
                            { CUP_DETAILS.TotalStep, 0 },
                            { CUP_DETAILS.TechnologyName, DBNull.Value },
                            { CUP_DETAILS.SetTemp, DBNull.Value },
                            { CUP_DETAILS.RecordIndex, 0 },
                            { CUP_DETAILS.Cooperate, 0 },
                            { CUP_DETAILS.Fail, 0 },
                            { CUP_DETAILS.HeadID, DBNull.Value },
                            { CUP_DETAILS.CurrentStepFinish,0 }
                           },
                           $"{CUP_DETAILS.CupNum} = @cupNo",
                           new SqlParameter("@cupNo", cupNum)
                       );
                        var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNum);
                        if (area != null)
                        {
                            area.OnCupDataReceived(cupNum);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("SendStopAsync", ex);
                }
                finally
                {
                    ResetAllSignalHandlers(cupNum);
                }

            });
        }

        public async Task SendWashAsync(int cupNum, string type)
        {
            await Task.Run(() =>
            {

                try
                {

                    // 1. 找到该杯所在的发送段
                    int sendIdx = cupNum - this._startCupNo;
                    var send = this._send[sendIdx];

                    // 2. 找到该杯所在的接收段

                    FC_4_Receive_Zone zone = this._receiveAll.GetZoneFromCupNo(cupNum, this._startCupNo);


                    // 3. 根据类型转换名称字符（调用FillProcessNameToCodes）和设置总步数
                    if (string.IsNullOrEmpty(type))
                        throw new ArgumentOutOfRangeException("未知洗杯类型");
                    string name = type;


                    int totalStepNum = CupAuxiliary.WashCupDic[type].Count;
                    FillProcessNameToCodes(name, send.Base);
                    send.Base.TotalStepNum.Value = (ushort)totalStepNum;
                    send.Base.StartStopReady.SetValue(1);

                    // 5. 查找总浴量
                    double totalWeight = 0;
                    var cupArea = SmartColor.My_AutomaticModule.CupCommManager.Instance.FindCupAreaByCupNum(cupNum);
                    bool isHighTemp = name == CupAuxiliary.HighTempWashCupType;
                    if (cupArea != null)
                    {
                        if (cupArea.AreaType == 2)
                            totalWeight = isHighTemp ? My_ConPar.WashCup.HighTempWash_BigAddWater : My_ConPar.WashCup.Wash_BigAddWater;
                        else
                            totalWeight = isHighTemp ? My_ConPar.WashCup.HighTempWash_AddWater : My_ConPar.WashCup.Wash_AddWater;
                    }

                    // 6. 更新数据库

                    SqlServer.Update(CUP_DETAILS.TableName,
                        new Dictionary<string, object>
                        {
                            { CUP_DETAILS.DyeingCode, name },
                            { CUP_DETAILS.IsUsing, 1 },
                            { CUP_DETAILS.TotalStep, totalStepNum },
                            { CUP_DETAILS.StepNum, 0 },
                            { CUP_DETAILS.StartTime, DateTime.Now },
                            { CUP_DETAILS.TotalWeight, totalWeight },
                            { CUP_DETAILS.CurrentStepFinish,0 }
                        },
                        $"{CUP_DETAILS.CupNum} = @cupNo",
                        new SqlParameter("@cupNo", cupNum));

                    var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNum);
                    if (area != null)
                    {
                        area.OnCupDataReceived(cupNum);
                    }

                    //  启动温度记录
                    var recorder = SmartColor.My_File.CupTempRecorder.Get(cupNum);
                    recorder.SetCylinderComm(this);
                    recorder.StartRecord();


                    // 7. 发送
                    lock (this._zoneLock)
                    {
                        ResetSendParamToZero(send.Param);
                        WriteToHMI(send.Base);
                    }

                    // 8. 记录日志
                    var dt = DateTime.Now;
                    _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                    {
                        [SmartColor.My_DataBase.RUN_TABLE.Dail] = $"{cupNum}号杯启动{type}指令"
                    }, dt);



                }
                catch (Exception ex)
                {
                    Logger.Error("SendWashAsync", ex);
                }
                finally
                {
                    ResetAllSignalHandlers(cupNum);
                }
            });
        }

        public async Task SyncCoverStatus(int cupNum)
        {
            await Task.Run(() =>
            {
                try
                {

                    var (mainInfo, subInfo) = CupAuxiliary.GetMSCupInfo(cupNum);
                    var sendIdx = (cupNum - this._startCupNo);
                    lock (this._zoneLock)
                    {
                        ushort coverStatus1 = (ushort)(mainInfo.CoverStatus ?? 0);
                        this._send[sendIdx].Cover.CoverStatus.Value = coverStatus1;
                        WriteToHMI(this._send[sendIdx].Cover);
                    }
                    // await Task.Delay(100);

                }
                catch (Exception ex)
                {
                    Logger.Error("SyncCoverStatus", ex);
                }


            });
        }

        /// <summary>
        /// 重置所有信号上升沿处理器（适用于信号消隐或流程复位后）
        /// </summary>
        /// <param name="cupNo">杯号</param>
        private void ResetAllSignalHandlers(int cupNo)
        {
            Thread.Sleep(1000);// 等待信号稳定，避免刚复位就触发上升沿事件
            _alarmTipHandler.Reset(cupNo);
            _allFinishHandler.Reset(cupNo);
            _cupSignalHandler.Reset(cupNo);
            _putClothHandler.Reset(cupNo);
            _waitDataHandler.Reset(cupNo);
            _moduleRestartHandler.Reset(cupNo);
            _historyHandler.Reset(cupNo);
        }
    }
}
