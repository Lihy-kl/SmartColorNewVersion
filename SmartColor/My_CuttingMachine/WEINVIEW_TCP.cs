using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Interaction;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartColor.My_CuttingMachine
{
    /// <summary>
    /// 威伦触摸屏信息类，包含机台的最大母液瓶号，软件是否过期。
    /// 根据威纶通触摸屏手册提供的Modbus寄存器地址 -1 
    /// </summary>
    internal class WEINVIEW_ControlInfo
    {
        /// <summary>最大母液瓶号</summary>
        public ProtocolItem MaxBottleNum { get; set; }

    }

    /// <summary>
    /// 威伦触摸屏发送区协议结构体，包含工位所有相关寄存器数据点。
    /// 每个字段对应HMI中的一个寄存器，便于批量管理和访问。
    /// 上位机发送区
    /// </summary>
    internal class WEINVIEW_Send
    {
        /// <summary>最大调液量（1000ml=1000），10100</summary>
        public ProtocolItem MaxLiquidAmount { get; set; }
        /// <summary>原浓度（*1000000），10101</summary>
        public ProtocolItem OriginalConcentration { get; set; }

        /// <summary>目标浓度（*1000000），10103</summary>
        public ProtocolItem TargetConcentration { get; set; }

        /// <summary>单位（%=0，g/l=1，G/L=2,WATER=3），10105</summary>
        public ProtocolItem Unit { get; set; }
        /// <summary>总步数(1=1)，10106</summary>
        public ProtocolItem TotalSteps { get; set; }
        /// <summary>接收完成标志位(0=无，1=接收完成)，D10107</summary>
        public ProtocolItem ReceiveFinishFlag { get; set; }
        /// <summary>步骤1工艺，10108（0=无，1=加大冷水，2=加小冷水，3=加热水，4=加染助剂，5=搅拌，6=加补充剂,7=温水）</summary>
        public ProtocolItem Step1Process { get; set; }
        /// <summary>步骤1比例（100%=100），10109</summary>
        public ProtocolItem Step1Ratio { get; set; }
        /// <summary>步骤2工艺，10110</summary>
        public ProtocolItem Step2Process { get; set; }
        /// <summary>步骤2比例，10111</summary>
        public ProtocolItem Step2Ratio { get; set; }
        /// <summary>步骤3工艺，10112</summary>
        public ProtocolItem Step3Process { get; set; }
        /// <summary>步骤3比例，10113</summary>
        public ProtocolItem Step3Ratio { get; set; }
        /// <summary>步骤4工艺，10114</summary>
        public ProtocolItem Step4Process { get; set; }
        /// <summary>步骤4比例，10115</summary>
        public ProtocolItem Step4Ratio { get; set; }
        /// <summary>步骤5工艺，10116</summary>
        public ProtocolItem Step5Process { get; set; }
        /// <summary>步骤5比例，10117</summary>
        public ProtocolItem Step5Ratio { get; set; }
        /// <summary>开希原瓶号（20=20），10118</summary>
        public ProtocolItem DilutionOriginalBottleNo { get; set; }

        /// <summary>备用1，10119</summary>
        public ProtocolItem Remark1 { get; set; }
        /// <summary>字1，10120</summary>
        public ProtocolItem Word1 { get; set; }
        /// <summary>字2，10121</summary>
        public ProtocolItem Word2 { get; set; }
        /// <summary>字3，10122</summary>
        public ProtocolItem Word3 { get; set; }
        /// <summary>字4，10123</summary>
        public ProtocolItem Word4 { get; set; }
        /// <summary>字5，10124</summary>
        public ProtocolItem Word5 { get; set; }
        /// <summary>字6，10125</summary>
        public ProtocolItem Word6 { get; set; }
        /// <summary>字7，10126</summary>
        public ProtocolItem Word7 { get; set; }
        /// <summary>字8，10127</summary>
        public ProtocolItem Word8 { get; set; }
        /// <summary>备用2，10128</summary>
        public ProtocolItem Remark2 { get; set; }
        /// <summary>备用3，10129</summary>
        public ProtocolItem Remark3 { get; set; }
    }

    /// <summary>
    /// 威伦触摸屏工位类，包含一个工位的所有协议项（寄存器数据点）
    /// 每个字段对应HMI中的一个寄存器，便于批量管理和访问。
    /// 上位机接收区。
    /// </summary>
    internal class WEINVIEW_Receive
    {
        /// <summary>对应数据库的ID号,10137</summary>
        public ProtocolItem MyID { get; set; }

        /// <summary>瓶号输入方式（0=人工，1=扫码，2=干预）,10139</summary>
        public ProtocolItem InputMode { get; set; }

        /// <summary>工位号（唯一标识工位）, 10140</summary>
        public ProtocolItem StationNo { get; set; }
        /// <summary>瓶号（当前工位处理的瓶子编号）, 10141</summary>
        public ProtocolItem BottleNo { get; set; }
        /// <summary>实际浓度（*1000000）, 10142</summary>
        public ProtocolItem ActualConcentration { get; set; }
        /// <summary>实际总量（*100000）, 10144</summary>
        public ProtocolItem ActualTotal { get; set; }
        /// <summary>瓶号输入完成标志（0=无，1=输入完成）, 10146</summary>
        public ProtocolItem InputFinishFlag { get; set; }
        /// <summary>泡制完成标志（0=无，1=完成，2=失败，3=中止，4=断电中止）, 10147</summary>
        public ProtocolItem BrewFinishFlag { get; set; }
        /// <summary>染助剂重量（*100）, 10148</summary>
        public ProtocolItem AssistantWeight { get; set; }
        /// <summary>备料标志（0=不备料，1=备料）, 10150</summary>
        public ProtocolItem PrepareFlag { get; set; }

    }

    /// <summary>
    /// 威伦触摸屏 TCP通讯管理类。
    /// 封装了工位数据的批量读取、打印、连接与断开等操作。
    /// 通过ModbusTCP协议与HMI进行数据交互。
    /// </summary>
    internal class WEINVIEW_TCP
    {
        /// <summary>
        /// 通知显示重联开料机按钮
        /// </summary>
        public event Action<string,string> ShowResetBrewButton;

        

        /// <summary>
        /// 更新母液瓶信息
        /// </summary>
        public event Action<int> UpdateBottleInfo;

        /// <summary>
        /// Modbus TCP通讯实例，负责底层数据收发
        /// </summary>
        private readonly ModbusTCP _modbus;

        /// <summary>
        /// HMI站号（UnitId），通常为1
        /// </summary>
        private readonly byte _unitId = 1;

        /// <summary>
        /// 工位创建工厂方法，简化初始化，减少重复代码
        /// </summary>
        /// <param name="baseAddr">工位起始寄存器地址</param>
        /// <returns>初始化后的工位对象</returns>
        private static  WEINVIEW_Receive CreateReceive(int baseAddr)
        {
            return new WEINVIEW_Receive
            {
                MyID = new My_Interaction.ProtocolItem(baseAddr + 0, typeof(int)),
                InputMode = new My_Interaction.ProtocolItem(baseAddr + 2, typeof(ushort)),
                StationNo = new My_Interaction.ProtocolItem(baseAddr + 3, typeof(ushort)),
                BottleNo = new My_Interaction.ProtocolItem(baseAddr + 4, typeof(ushort)),
                ActualConcentration = new My_Interaction.ProtocolItem(baseAddr + 5, typeof(int)),
                ActualTotal = new My_Interaction.ProtocolItem(baseAddr + 7, typeof(int)),
                InputFinishFlag = new My_Interaction.ProtocolItem(baseAddr + 9, typeof(ushort)),
                BrewFinishFlag = new My_Interaction.ProtocolItem(baseAddr + 10, typeof(ushort)),
                AssistantWeight = new My_Interaction.ProtocolItem(baseAddr + 11, typeof(int)),
                PrepareFlag = new My_Interaction.ProtocolItem(baseAddr + 13, typeof(ushort)),

            };
        }

        /// <summary>
        /// 辅助方法：根据ProtocolItem的数据类型安全赋值，保证类型一致性。
        /// </summary>
        private void SetProtocolItemValue(My_Interaction.ProtocolItem item, object value)
        {
            if (item == null) return;
            if (value == null || value == DBNull.Value)
            {
                item.Value = item.DataType == typeof(int) ? 0 : (object)(ushort)0;
                return;
            }
            try
            {
                if (item.DataType == typeof(int))
                    item.Value = Convert.ToInt32(value);
                else if (item.DataType == typeof(ushort))
                    item.Value = Convert.ToUInt16(value);
                else
                    item.Value = value;
            }
            catch
            {
                item.Value = item.DataType == typeof(int) ? 0 : (object)(ushort)0;
            }
        }

        /// <summary>
        /// 发送区工位创建工厂方法，简化初始化，减少重复代码
        /// </summary>
        /// <param name="baseAddr">工位发送区起始寄存器地址（如3030）</param>
        /// <returns>初始化后的发送区工位对象</returns>
        private static  WEINVIEW_Send CreateSend(int baseAddr)
        {
            var send = new WEINVIEW_Send
            {
                MaxLiquidAmount = new My_Interaction.ProtocolItem(baseAddr + 0, typeof(ushort)),
                OriginalConcentration = new My_Interaction.ProtocolItem(baseAddr + 1, typeof(int)),
                TargetConcentration = new My_Interaction.ProtocolItem(baseAddr + 3, typeof(int)),
                Unit = new My_Interaction.ProtocolItem(baseAddr + 5, typeof(ushort)),
                TotalSteps = new My_Interaction.ProtocolItem(baseAddr + 6, typeof(ushort)),
                ReceiveFinishFlag = new My_Interaction.ProtocolItem(baseAddr + 7, typeof(ushort)),
                Step1Process = new My_Interaction.ProtocolItem(baseAddr + 8, typeof(ushort)),
                Step1Ratio = new My_Interaction.ProtocolItem(baseAddr + 9, typeof(ushort)),
                Step2Process = new My_Interaction.ProtocolItem(baseAddr + 10, typeof(ushort)),
                Step2Ratio = new My_Interaction.ProtocolItem(baseAddr + 11, typeof(ushort)),
                Step3Process = new My_Interaction.ProtocolItem(baseAddr + 12, typeof(ushort)),
                Step3Ratio = new My_Interaction.ProtocolItem(baseAddr + 13, typeof(ushort)),
                Step4Process = new My_Interaction.ProtocolItem(baseAddr + 14, typeof(ushort)),
                Step4Ratio = new My_Interaction.ProtocolItem(baseAddr + 15, typeof(ushort)),
                Step5Process = new My_Interaction.ProtocolItem(baseAddr + 16, typeof(ushort)),
                Step5Ratio = new My_Interaction.ProtocolItem(baseAddr + 17, typeof(ushort)),
                DilutionOriginalBottleNo = new My_Interaction.ProtocolItem(baseAddr + 18, typeof(ushort)),
                Remark1 = new My_Interaction.ProtocolItem(baseAddr + 19, typeof(ushort)),
                Remark2 = new My_Interaction.ProtocolItem(baseAddr + 28, typeof(ushort)),
                Remark3 = new My_Interaction.ProtocolItem(baseAddr + 29, typeof(ushort)),


            };

            // 字1~字50（10120~D10129）
            for (int i = 0; i < 8; i++)
            {
                int addr = baseAddr + 20 + i; // 10120 = 10100+20
                var word = new My_Interaction.ProtocolItem(addr, typeof(ushort));
                switch (i + 1)
                {
                    case 1: send.Word1 = word; break;
                    case 2: send.Word2 = word; break;
                    case 3: send.Word3 = word; break;
                    case 4: send.Word4 = word; break;
                    case 5: send.Word5 = word; break;
                    case 6: send.Word6 = word; break;
                    case 7: send.Word7 = word; break;
                    case 8: send.Word8 = word; break;
                }
            }

            return send;
        }


        /// <summary>
        /// 机台信息创建工厂方法，简化初始化，减少重复代码
        /// </summary>
        /// <param name="baseAddr">起始地址</param>
        /// <returns>机台信息</returns>
        private static  WEINVIEW_ControlInfo CreateControlInfo(int baseAddr)
        {
            return new WEINVIEW_ControlInfo
            {
                MaxBottleNum = new My_Interaction.ProtocolItem(baseAddr, typeof(ushort)),
            };
        }

        /// <summary>
        /// 所有工位的接受区协议项集合。
        /// 每个工位包含一组寄存器，地址、名称、类型等已预定义。
        /// 便于批量读取和管理。
        /// </summary>
        private readonly List<WEINVIEW_Receive> _receive = new List<WEINVIEW_Receive>
        {
            //一工位
            CreateReceive(10136),
            //二工位
            CreateReceive(10236),
            //三工位
            CreateReceive(10336),
            //四工位
            CreateReceive(10436),
            //五工位
            CreateReceive(10536)
        };

        /// <summary>
        /// 所有工位的发送区协议项集合。
        /// 每个工位包含一组寄存器，地址、名称、类型等已预定义。
        /// 便于批量读取和管理。
        /// </summary>
        private readonly List<WEINVIEW_Send> _send = new List<WEINVIEW_Send>
        {
            //一工位
            CreateSend(10099),
            //二工位
            CreateSend(10199),
            //三工位
            CreateSend(10299),
            //四工位
            CreateSend(10399),
            //五工位
            CreateSend(10499)
        };

        /// <summary>
        /// 机台信息协议项集合。
        /// </summary>
        private readonly WEINVIEW_ControlInfo _controlInfo = CreateControlInfo(10999);

        /// <summary>
        /// 构造函数，初始化ModbusTCP通讯实例。
        /// </summary>
        /// <param name="ip">HMI服务器IP地址</param>
        /// <param name="port">HMI服务器端口号</param>
        public WEINVIEW_TCP(string ip, int port)
        {
            // 实例化ModbusTCP对象，指定IP、端口和实例标识
            _modbus = new ModbusTCP(ip, port, "WEINVIEW_TCP");
        }

        /// <summary>
        /// 连接到HMI服务器。
        /// 实际调用ModbusTCP的Connect方法，支持自动重连。
        /// </summary>
        public void Connect()
        {
            // 建立TCP连接，底层支持重试机制
            _modbus.Connect();
        }

        /// <summary>
        /// 断开与HMI服务器的连接。
        /// </summary>
        private void Disconnect()
        {
            // 关闭TCP连接，释放资源
            _modbus.Disconnect();
        }

        /// <summary>
        /// 读取所有工位的接受区数据，并填充到各ProtocolItem的Value属性。
        /// 每个工位按顺序批量读取12个寄存器，分别对应各数据点。
        /// 增加异常处理，保证通讯异常时不会导致程序崩溃。
        /// </summary>
        private int ReadAllReceive()
        {
            foreach (var ws in _receive)
            {
                try
                {
                    int regCount = GetRegisterCount(ws);
                    ushort[] regs = _modbus.ReadHoldingRegisters(_unitId, (ushort)ws.MyID.Address, (ushort)regCount);

                    int idx = 0;
                    SetProtocolItemValue(ws.MyID, (regs[idx++] | regs[idx++] << 16));
                    SetProtocolItemValue(ws.InputMode, regs[idx++]);
                    SetProtocolItemValue(ws.StationNo, regs[idx++]);
                    SetProtocolItemValue(ws.BottleNo, regs[idx++]);
                    SetProtocolItemValue(ws.ActualConcentration, (regs[idx++] | regs[idx++] << 16));
                    SetProtocolItemValue(ws.ActualTotal, (regs[idx++] | regs[idx++] << 16));
                    SetProtocolItemValue(ws.InputFinishFlag, regs[idx++]);
                    SetProtocolItemValue(ws.BrewFinishFlag, regs[idx++]);
                    SetProtocolItemValue(ws.AssistantWeight, (regs[idx++] | regs[idx++] << 16));
                    SetProtocolItemValue(ws.PrepareFlag, regs[idx++]);


                    // 开料
                    if (Convert.ToInt32(ws.InputFinishFlag.Value) == 1)
                    {
                        StartBrew(ws, _send[Convert.ToInt16(ws.StationNo.Value) - 1]);
                    }

                    // 完成
                    if (Convert.ToInt32(ws.BrewFinishFlag.Value) != 0)
                    {
                        FinishBrew(ws);
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error($"读取地址 {ws.StationNo.Address} 失败", ex);
                    return -1;
                }

            }
            return 0;
        }




        /// <summary>
        /// 开料：根据瓶号查找数据库，填充Send结构体并写入HMI
        /// </summary>
        /// <param name="bottleNo">瓶号</param>
        /// <param name="send">发送区结构体</param>
        private void StartBrew(WEINVIEW_Receive receive, WEINVIEW_Send send)
        {
            //先清空步骤和名称
            ClearBrewStepsAndName(send);

            //找到瓶号对应的数据
            int bottleNo = Convert.ToInt32(receive.BottleNo.Value);

            MessageEventManager.Instance.RequestShowBalloonTip($"收到{bottleNo}号母液瓶请求泡制信息");

            DataTable dt = BottleData.Bottle_details;
            DataRow[] rows = dt.Select($"{BOTTLE_DETAILS.BottleNum}={bottleNo}");
            if (rows.Length == 0)
            {
                Logger.Info($"未找到瓶号 {bottleNo} 的数据。");
                return;
            }

            //找到原瓶号

            var originalbottleno = (rows[0][BOTTLE_DETAILS.OriginalBottleNum] is DBNull)? 0: Convert.ToInt16(rows[0][BOTTLE_DETAILS.OriginalBottleNum]);
            var originalconcentration = 0.0;
            var originalcurrentweight = 0.0;
            if ( originalbottleno != 0)
            {
                var originalDt = BottleData.Bottle_details.Select($"{BOTTLE_DETAILS.BottleNum}={originalbottleno}");
                if (originalDt.Length == 0)
                {
                    Logger.Info($"未找到原瓶号 {originalbottleno} 的数据。");
                    return;
                }
                // 填充原浓度和原瓶重量
                originalconcentration = Convert.ToDouble(originalDt[0][BOTTLE_DETAILS.RealConcentration]);
                originalcurrentweight = Convert.ToDouble(originalDt[0][BOTTLE_DETAILS.CurrentWeight]);
            }

            //找到染助剂对应的数据
            var asscode = rows[0][BOTTLE_DETAILS.AssistantCode].ToString();
            var assistantDt = AssistantData.Assistant_details;
            var assistantRows = assistantDt.Select($"{ASSISTANT_DETAILS.AssistantCode}='{asscode}'");
            if (assistantRows.Length == 0)
            {
                Console.WriteLine($"未找到染助剂编码 {asscode} 的数据。");
                return;
            }

            var name = assistantRows[0][ASSISTANT_DETAILS.AssistantName].ToString();
            var unit = assistantRows[0][ASSISTANT_DETAILS.UnitOfAccount].ToString();

            //找到开料流程
            var brewcd = rows[0][BOTTLE_DETAILS.BrewingCode].ToString();
            var brewRows = My_DataBase.BrewData.Brewing_process.Select($"{BREWING_PROCESS.BrewingCode}='{brewcd}'").OrderBy(r => r[BREWING_PROCESS.StepNum]);

            var row = rows[0];
            SetProtocolItemValue(send.MaxLiquidAmount, Convert.ToInt16(row[BOTTLE_DETAILS.AllowMaxWeight]));
            SetProtocolItemValue(send.OriginalConcentration, Convert.ToInt32(originalconcentration * 1000000));
            SetProtocolItemValue(send.TargetConcentration, Convert.ToInt32(Convert.ToDouble(row[BOTTLE_DETAILS.SettingConcentration]) * 1000000));
            SetProtocolItemValue(send.Unit, ConvertUnitToCode(unit));
            SetProtocolItemValue(send.TotalSteps, brewRows.Count());
            SetProtocolItemValue(send.ReceiveFinishFlag, 0);

            SetProtocolItemValue(send.DilutionOriginalBottleNo, originalbottleno);


            //填充步骤
            FillBrewSteps(brewRows, send);

            //填充名称
            FillAssistantName(name, send);


            // 写入HMI
            WriteSendToHMI(send);


            //(由于完成标志位定义在前面，必须所有资料传完后，再传完成标志位) 写入完成标志位
            SetProtocolItemValue(send.ReceiveFinishFlag, 1);
            _modbus.WriteSingleRegister(_unitId, (ushort)send.ReceiveFinishFlag.Address, (ushort)send.ReceiveFinishFlag.Value);


            int i = SqlServer.InsertAndGetIdentity(BREW_RUN_TABLE.TableName,
                            new Dictionary<string, object>
                            {
                                { BREW_RUN_TABLE.StartDateTime, DateTime.Now },
                                { BREW_RUN_TABLE.BottleNum,bottleNo },
                                { BREW_RUN_TABLE.BrewCode,  brewcd},
                                { BREW_RUN_TABLE.SettingConcentration,  row[BOTTLE_DETAILS.SettingConcentration]},
                                { BREW_RUN_TABLE.AllowMaxWeight, row[BOTTLE_DETAILS.AllowMaxWeight] },
                                { BREW_RUN_TABLE.OriginalBottleNum, row[BOTTLE_DETAILS.OriginalBottleNum] },
                                { BREW_RUN_TABLE.OriginalConcentration, originalconcentration },
                                { BREW_RUN_TABLE.InputMode,  ConvertInputModeToString(Convert.ToInt16( receive.InputMode.Value))} ,
                                { BREW_RUN_TABLE.AssistantName, name }

                            });
            //清除瓶号输入完成标志位
            SetProtocolItemValue(receive.MyID, i);
            SetProtocolItemValue(receive.InputFinishFlag, 0);
            WriteReceiveToHMI(receive);
        }

        /// <summary>
        /// 瓶号输入方式转换为字符串
        /// </summary>
        /// <param name="inputMode">输入方式</param>
        /// <returns>字符串</returns>
        private string ConvertInputModeToString(int inputMode)
        {
            switch (inputMode)
            {
                case 0:
                    return "人工";
                case 1:
                    return "扫码";
                case 2:
                    return "干预";
                default:
                    return "未知";
            }
        }

        /// <summary>
        /// 将染助剂名称填充到Send结构体的字1~字50中
        /// </summary>
        /// <param name="name">染助剂名称</param>
        /// <param name="send">要填充的结构体</param>
        private void FillAssistantName(string name, WEINVIEW_Send send)
        {
            // 1. GB2312编码
            Encoding fromEncoding = Encoding.UTF8;
            Encoding toEncoding = Encoding.GetEncoding("GB2312");
            byte[] srcBytes = fromEncoding.GetBytes(name ?? "");
            byte[] gbBytes = Encoding.Convert(fromEncoding, toEncoding, srcBytes);

            // 2. 组装8个ushort（每个2字节，低字节在前，高字节在后，空位补0x000D）
            ushort[] assistantNameWords = new ushort[8];
            for (int i = 0; i < assistantNameWords.Length; i++)
            {
                int idx = i * 2;
                byte low = idx < gbBytes.Length ? gbBytes[idx] : (byte)0x00;
                byte high = (idx + 1) < gbBytes.Length ? gbBytes[idx + 1] : (byte)0x0D;
                assistantNameWords[i] = (ushort)((high << 8) | low);
            }

            // 3. 填充到Send结构体
            SetProtocolItemValue(send.Word1, assistantNameWords[0]);
            SetProtocolItemValue(send.Word2, assistantNameWords[1]);
            SetProtocolItemValue(send.Word3, assistantNameWords[2]);
            SetProtocolItemValue(send.Word4, assistantNameWords[3]);
            SetProtocolItemValue(send.Word5, assistantNameWords[4]);
            SetProtocolItemValue(send.Word6, assistantNameWords[5]);
            SetProtocolItemValue(send.Word7, assistantNameWords[6]);
            SetProtocolItemValue(send.Word8, assistantNameWords[7]);

        }
        //  清零所有泡制步骤和名称
        private void ClearBrewStepsAndName(WEINVIEW_Send send)
        {
            foreach (var prop in typeof(WEINVIEW_Send).GetProperties())
            {
                if ((prop.Name.StartsWith("Step") || prop.Name.StartsWith("Word")) && prop.PropertyType == typeof(ProtocolItem))
                {
                    var item = prop.GetValue(send) as ProtocolItem;
                    if (item != null)
                    {
                        if (prop.Name.StartsWith("Word"))
                        {
                            SetProtocolItemValue(item, 0x0D); // 名称类字段填0x0D
                        }
                        else if (prop.Name.StartsWith("Step"))
                        {
                            SetProtocolItemValue(item, 0);    // 步骤类字段填0
                        }
                    }
                    
                }
            }
        }

        /// <summary>
        /// 填充泡制步骤到Send结构体
        /// </summary>
        /// <param name="brewRows">当前泡料流程</param>
        /// <param name="send">Send结构体</param>
        private void FillBrewSteps(IEnumerable<DataRow> brewRows, WEINVIEW_Send send)
        {
            int stepIndex = 0;
            foreach (var brewRow in brewRows)
            {
                stepIndex++;
                ushort tn = ConvertTechnologyNameToCode(brewRow[BREWING_PROCESS.TechnologyName]?.ToString());
                ushort pot = Convert.ToUInt16(brewRow[BREWING_PROCESS.ProportionOrTime]);
                switch (stepIndex)
                {
                    case 1:
                        SetProtocolItemValue(send.Step1Process, tn);
                        SetProtocolItemValue(send.Step1Ratio, pot);

                        break;
                    case 2:
                        SetProtocolItemValue(send.Step2Process, tn);
                        SetProtocolItemValue(send.Step2Ratio, pot);

                        break;
                    case 3:
                        SetProtocolItemValue(send.Step3Process, tn);
                        SetProtocolItemValue(send.Step3Ratio, pot);

                        break;
                    case 4:
                        SetProtocolItemValue(send.Step4Process, tn);
                        SetProtocolItemValue(send.Step4Ratio, pot);

                        break;
                    case 5:
                        SetProtocolItemValue(send.Step5Process, tn);
                        SetProtocolItemValue(send.Step5Ratio, pot);

                        break;

                    default:
                        break;
                }
            }
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
                case "加大冷水":
                    i = 1; break;
                case "加小冷水":
                    i = 2; break;
                case "加热水":
                    i = 3; break;
                case "手动加染助剂":
                    i = 4; break;
                case "搅拌":
                    i = 5; break;
                case "加补充剂":
                    i = 6; break;
                case "加温水":
                    i = 7; break;
                default:
                    break;
            }
            return i;
        }


        /// <summary>
        /// 单位转换为代码
        /// </summary>
        /// <param name="unit">单位</param>
        /// <returns>代码</returns>
        private int ConvertUnitToCode(string unit)
        {
            int i = 0;
            switch (unit)
            {
                case "%":
                    i = 0;
                    break;
                case "g/l":
                    i = 1;
                    break;
                case "G/L":
                    i = 2;
                    break;
                case "WATER":
                    i = 3;
                    break;
                default:
                    i = 0;
                    break;
            }
            return i;

        }



        /// <summary>
        /// 将Send结构体数据写入HMI
        /// </summary>
        /// <param name="send">发送区结构体</param>
        private void WriteSendToHMI(WEINVIEW_Send send)
        {
            List<ushort> values = new List<ushort>();
            foreach (var prop in typeof(WEINVIEW_Send).GetProperties())
            {
                var item = prop.GetValue(send) as My_Interaction.ProtocolItem;
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
                    // 如果ProtocolItem为null或Value为null，补0
                    values.Add(0);
                }
            }
            // 写入HMI
            _modbus.WriteMultipleRegisters(_unitId, (ushort)send.MaxLiquidAmount.Address, values.ToArray());
        }

        /// <summary>
        /// 将Receive结构体数据批量写入HMI
        /// </summary>
        /// <param name="receive">接收区结构体</param>
        private void WriteReceiveToHMI(WEINVIEW_Receive receive)
        {
            List<ushort> values = new List<ushort>();
            foreach (var prop in typeof(WEINVIEW_Receive).GetProperties())
            {
                var item = prop.GetValue(receive) as My_Interaction.ProtocolItem;
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
                    // 如果ProtocolItem为null或Value为null，补0
                    values.Add(0);
                }
            }
            // 写入HMI，起始地址为MyID.Address
            _modbus.WriteMultipleRegisters(_unitId, (ushort)receive.MyID.Address, values.ToArray());
        }

        /// <summary>
        /// 写入机台信息到HMI
        /// </summary>
        public void WriteControlInfoToHMI()
        {
            List<ushort> values = new List<ushort>();

            var trialDaysLeft = My_Form.Help.Register.GetTrialDaysLeft();
            int i = 0;
            if (trialDaysLeft != null)
            {

                if (Convert.ToInt32(trialDaysLeft) > 0)
                {
                    i = 1;
                }
                else
                {
                    i = 0;
                }
            }
            else
            {
                i = 1;
            }

            if (i == 0)
            {
                SetProtocolItemValue(_controlInfo.MaxBottleNum, 0);

            }
            else
            {
                int maxBottleNum = 0;
                switch (SmartColor.My_ConPar.Machine.MachineLayout)
                {
                    case 0: // 常规滴液机布局
                        maxBottleNum = ((SmartColor.My_ConPar.Area.BottleArea.Bottle)SmartColor.My_ConPar.Area.Layout_1.Area_1).BottleNum;
                        break;
                    case 1: // 常规打版机布局
                        maxBottleNum = ((SmartColor.My_ConPar.Area.BottleArea.Bottle)SmartColor.My_ConPar.Area.Layout_2.Area_1).BottleNum;
                        break;
                    case 2: // 全自动打板机布局
                        maxBottleNum = ((SmartColor.My_ConPar.Area.BottleArea.Bottle)SmartColor.My_ConPar.Area.Layout_3.Area_1).BottleNum;
                        break;
                    case 3: // 全自动打板布局
                        maxBottleNum = ((SmartColor.My_ConPar.Area.BottleArea.Bottle)SmartColor.My_ConPar.Area.Layout_4.Area_1).BottleNum;
                        break;
                }
                SetProtocolItemValue(_controlInfo.MaxBottleNum, maxBottleNum);
            }
            foreach (var prop in typeof(WEINVIEW_ControlInfo).GetProperties())
            {
                var item = prop.GetValue(_controlInfo) as My_Interaction.ProtocolItem;
                if (item != null && item.Value != null)
                {
                    values.Add(Convert.ToUInt16(item.Value));
                }
                else
                {
                    // 如果ProtocolItem为null或Value为null，补0
                    values.Add(0);
                }
            }
            // 写入HMI
            _modbus.WriteMultipleRegisters(_unitId, (ushort)_controlInfo.MaxBottleNum.Address, values.ToArray());
        }

        /// <summary>
        /// 完成：根据PrepareFlag写入不同表
        /// </summary>
        /// <param name="ws">接收区结构体</param>

        private void FinishBrew(WEINVIEW_Receive ws)
        {
            MessageEventManager.Instance.RequestShowBalloonTip($"收到{ws.BottleNo.Value}号母液瓶泡制完成信息");
            int ff = Convert.ToInt16(ws.BrewFinishFlag.Value);
            int pf = Convert.ToInt16(ws.PrepareFlag.Value);
            var bn = ws.BottleNo.Value;
            var ac = Convert.ToInt32(ws.ActualConcentration.Value) / 1000000.00;
            var at = Convert.ToInt32(ws.ActualTotal.Value) / 100000.00;
            var am = Convert.ToInt32(ws.AssistantWeight.Value) / 100.00;
            //找到原瓶号
            DataTable dt = BottleData.Bottle_details;
            DataRow[] rows = dt.Select($"{BOTTLE_DETAILS.BottleNum}={bn}");
            if (rows.Length == 0)
            {
                Logger.Info($"未找到瓶号 {bn} 的数据。");
                return;
            }

            var originalbottleno = (rows[0][BOTTLE_DETAILS.OriginalBottleNum] is DBNull) ? 0 : Convert.ToInt16(rows[0][BOTTLE_DETAILS.OriginalBottleNum]);

            var endtime = DateTime.Now;
            if (originalbottleno > 0)
            {
                DataRow[] rows1 = dt.Select($"{BOTTLE_DETAILS.BottleNum}={originalbottleno}");
                if (rows1.Length == 0)
                {
                    Logger.Info($"未找到瓶号 {originalbottleno} 的数据。");
                    return;
                }

                if (My_ConPar.Choices.UseMotherDate == 0)
                {
                    endtime = (rows1[0][BOTTLE_DETAILS.BrewingData] is DBNull) ? DateTime.Now : Convert.ToDateTime(rows1[0][BOTTLE_DETAILS.BrewingData]);
                }

                //减去原瓶号的对应的重量
                SqlServer.ExecuteNonQuery(
                    @"UPDATE bottle_details
                    SET CurrentWeight = CASE 
                        WHEN CurrentWeight - @aw < 0 THEN 0 
                        ELSE CurrentWeight - @aw 
                    END
                    WHERE BottleNum = @originalbottleno",
                    new SqlParameter("@aw", am),
                    new SqlParameter("@originalbottleno", originalbottleno)
                );
            }
            else
            {
                //预留扣减粉体重量

            }

            var id = ws.MyID.Value;

            if (ff == 1)
            {
                if (pf == 1)
                {
                    // 写入备料相关表
                    SqlServer.Insert(PRE_BREW.TableName,
                        new Dictionary<string, object>
                        {
                           {PRE_BREW.BottleNum,bn },
                           {PRE_BREW.RealConcentration,ac },
                           {PRE_BREW.CurrentWeight,at },
                           {PRE_BREW.BrewingData,endtime }
                        });
                    Logger.Info($"备料完成，瓶号:{bn} 浓度:{ac} 重量:{at}");
                }

                else
                {
                    // 写入母液瓶表
                    SqlServer.Update(BOTTLE_DETAILS.TableName,
                        new Dictionary<string, object>
                        {
                            { BOTTLE_DETAILS.RealConcentration, ac },
                            {BOTTLE_DETAILS.CurrentWeight,at },
                            {BOTTLE_DETAILS.BrewingData,endtime },
                            {BOTTLE_DETAILS.AdjustSuccess,0 }
                        },
                        $"{BOTTLE_DETAILS.BottleNum} = {bn}");

                    UpdateBottleInfo?.Invoke(Convert.ToInt16(bn));
                    Logger.Info($"母液瓶更新，瓶号:{bn} 浓度:{ac} 重量:{at}");


                }
            }

            //更新开料运行记录表
            if (id != null)
            {
                //更新开料运行记录表
                SqlServer.ExecuteNonQuery(
                    $@"UPDATE {BREW_RUN_TABLE.TableName}
                        SET {BREW_RUN_TABLE.FinishDateTime} = @FinishDateTime,
                        {BREW_RUN_TABLE.RealConcentration} = @RealConcentration,
                        {BREW_RUN_TABLE.CurrentWeight} = @CurrentWeight,
                        {BREW_RUN_TABLE.ReasonCessation} = @ReasonCessation,
                        {BREW_RUN_TABLE.UseingTime} = RIGHT('0' + CAST(DATEDIFF(SECOND, {BREW_RUN_TABLE.StartDateTime}, @FinishDateTime)/3600 AS VARCHAR),2) + ':' +
                                      RIGHT('0' + CAST((DATEDIFF(SECOND, {BREW_RUN_TABLE.StartDateTime}, @FinishDateTime)%3600)/60 AS VARCHAR),2) + ':' +
                                      RIGHT('0' + CAST(DATEDIFF(SECOND, {BREW_RUN_TABLE.StartDateTime}, @FinishDateTime)%60 AS VARCHAR),2)
                         WHERE {BREW_RUN_TABLE.MyID} = @MyID",
                     new System.Data.SqlClient.SqlParameter("@FinishDateTime", endtime),
                     new System.Data.SqlClient.SqlParameter("@RealConcentration", ac),
                     new System.Data.SqlClient.SqlParameter("@CurrentWeight", at),
                     new System.Data.SqlClient.SqlParameter("@ReasonCessation", ConverReasonCessationToString(ff)),
                     new System.Data.SqlClient.SqlParameter("@MyID", id)
                );
                Logger.Info($"开料运行记录表已更新，ID:{id} 完成时间:{endtime}");
            }

         

            //清除泡制完成标志位
            SetProtocolItemValue(ws.BottleNo, 0);
            SetProtocolItemValue(ws.ActualConcentration, 0);
            SetProtocolItemValue(ws.ActualTotal, 0);
            SetProtocolItemValue(ws.InputFinishFlag, 0);
            SetProtocolItemValue(ws.BrewFinishFlag, 0);
            SetProtocolItemValue(ws.AssistantWeight, 0);
            SetProtocolItemValue(ws.PrepareFlag, 0);
            SetProtocolItemValue(ws.InputMode, 0);
            SetProtocolItemValue(ws.MyID, 0);

            WriteReceiveToHMI(ws);
        }

        /// <summary>
        /// 停止原因代码转字符串
        /// </summary>
        /// <param name="reasonCessation">停止原因代码</param>
        /// <returns>字符串</returns>
        private string ConverReasonCessationToString(int reasonCessation)
        {
            string s = string.Empty;
            switch (reasonCessation)
            {

                case 0:
                    s = "无";
                    break;
                case 1:
                    s = "泡制成功";
                    break;
                case 2:
                    s = "泡制失败";
                    break;
                case 3:
                    s = "泡制终止";
                    break;
                case 4:
                    s = "断电终止";
                    break;
                default:
                    break;
            }
            return s;
        }


        /// <summary>
        /// 计算指定工位需要读取的寄存器数量（根据数据类型自动判断）
        /// </summary>
        /// <param name="ws">工位对象</param>
        /// <returns>需要读取的寄存器数量</returns>
        private static  int GetRegisterCount(WEINVIEW_Receive ws)
        {
            int count = 0;
            foreach (var prop in typeof(WEINVIEW_Receive).GetProperties())
            {
                var item = prop.GetValue(ws) as My_Interaction.ProtocolItem;
                if (item != null)
                {
                    // int类型占2个寄存器，ushort类型占1个
                    if (item.DataType == typeof(int))
                        count += 2;
                    else
                        count += 1;
                }
            }
            return count;
        }


        /// <summary>
        /// 启动威纶通讯线程。
        /// </summary>
        public void StartCom()
        {
            int retryCount = 0;
            const int maxRetry = 5;
            const int retryDelayMs = 1000;

            try
            {
                while (retryCount < maxRetry)
                {
                    try
                    {
                        // 1. 连接HMI（内部已自带3次重试）
                        Connect();

                        // 2. 发送机台配置
                        WriteControlInfoToHMI();

                        // 3. 主循环
                        while (true)
                        {
                            if (!_modbus.IsConnected)
                            {
                                throw new Exception("Modbus未连接");
                            }

                            if (ReadAllReceive() == -1)
                            {
                                throw new Exception("读取工位数据失败");
                            }

                            Thread.Sleep(10); // 合理延时，减少CPU占用
                        }
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        Logger.Error($"开料机通讯异常，第{retryCount}次重试: {ex.Message}", ex);
                       
                        Disconnect();
                        Thread.Sleep(retryDelayMs);
                    }
                }

                // 超过最大重试次数，通知主界面
                ShowResetBrewButton?.Invoke("开料机通讯失败", "已超过最大重试次数，退出通讯线程");
                Logger.Error("开料机通讯连续失败，已超过最大重试次数，退出通讯线程。");
            }
            finally
            {
                Disconnect();
            }
        }

    }
}
