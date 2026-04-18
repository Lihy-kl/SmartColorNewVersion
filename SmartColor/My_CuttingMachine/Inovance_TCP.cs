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
using System.Windows.Forms;

namespace SmartColor.My_CuttingMachine
{
    /// <summary>
    /// 汇川PLC机台信息类，包含机台的最大母液瓶号，软件是否过期。
    /// </summary>
    internal class Inovance_TCP_ControlInfo
    {
        /// <summary>最大母液瓶号</summary>
        public ProtocolItem MaxBottleNum { get; set; }
        /// <summary>通信状态（0=断开，1=连接）</summary>
        public ProtocolItem CommunicationState { get; set; }
    }

    /// <summary>
    /// 汇川PLC版本信息结构体，包含版本号--年、版本号--月日
    /// </summary>
    internal class Inovance_TCP_VersionInfo
    {
        /// <summary>版本号——年</summary>
        public ProtocolItem VersionYear { get; set; }
        /// <summary>版本号——月日</summary>
        public ProtocolItem VersionMD { get; set; }

    }

    /// <summary>
    /// 汇川PLC发送区协议结构体，包含工位所有相关寄存器数据点。
    /// 每个字段对应PLC中的一个寄存器，便于批量管理和访问。
    /// 上位机发送区
    /// </summary>
    internal class Inovance_TCP_Send
    {
        /// <summary>最大调液量（1000ml=1000），D3030</summary>
        public ProtocolItem MaxLiquidAmount { get; set; }
        /// <summary>原浓度（*1000000），D3031</summary>
        public ProtocolItem OriginalConcentration { get; set; }

        /// <summary>目标浓度（*1000000），D3033</summary>
        public ProtocolItem TargetConcentration { get; set; }

        /// <summary>单位（%=0，g/l=1，G/L=2,WATER=3），D3035</summary>
        public ProtocolItem Unit { get; set; }

        /// <summary>总步数(1=1)，D3036</summary>
        public ProtocolItem TotalSteps { get; set; }

        /// <summary>接收完成标志位(0=无，1=接收完成)，D3037</summary>
        public ProtocolItem ReceiveFinishFlag { get; set; }

        /// <summary>步骤1工艺，D3038（0=无，1=加大冷水，2=加小冷水，3=加热水，4=加染助剂，5=搅拌，6=加补充剂,7=温水）</summary>
        public ProtocolItem Step1Process { get; set; }
        /// <summary>步骤1比例（100%=100），D3039</summary>
        public ProtocolItem Step1Ratio { get; set; }
        /// <summary>步骤2工艺，D3040</summary>
        public ProtocolItem Step2Process { get; set; }
        /// <summary>步骤2比例，D3041</summary>
        public ProtocolItem Step2Ratio { get; set; }
        /// <summary>步骤3工艺，D3042</summary>
        public ProtocolItem Step3Process { get; set; }
        /// <summary>步骤3比例，D3043</summary>
        public ProtocolItem Step3Ratio { get; set; }
        /// <summary>步骤4工艺，D3044</summary>
        public ProtocolItem Step4Process { get; set; }
        /// <summary>步骤4比例，D3045</summary>
        public ProtocolItem Step4Ratio { get; set; }
        /// <summary>步骤5工艺，D3046</summary>
        public ProtocolItem Step5Process { get; set; }
        /// <summary>步骤5比例，D3047</summary>
        public ProtocolItem Step5Ratio { get; set; }
        /// <summary>开希原瓶号（20=20），D3048</summary>
        public ProtocolItem DilutionOriginalBottleNo { get; set; }
        /// <summary>步骤6工艺，D3049</summary>
        public ProtocolItem Step6Process { get; set; }
        /// <summary>步骤6比例，D3050</summary>
        public ProtocolItem Step6Ratio { get; set; }
        /// <summary>开稀原瓶重量（100g=100），D3051</summary>
        public ProtocolItem DilutionOriginalBottleWeight { get; set; }

        /// <summary>步骤1温水比例（100%=100），D3053</summary>
        public ProtocolItem Step1WarmWaterRatio { get; set; }
        /// <summary>步骤2温水比例，D3054</summary>
        public ProtocolItem Step2WarmWaterRatio { get; set; }
        /// <summary>步骤3温水比例，D3055</summary>
        public ProtocolItem Step3WarmWaterRatio { get; set; }
        /// <summary>步骤4温水比例，D3056</summary>
        public ProtocolItem Step4WarmWaterRatio { get; set; }
        /// <summary>步骤5温水比例，D3057</summary>
        public ProtocolItem Step5WarmWaterRatio { get; set; }
        /// <summary>步骤6温水比例，D3058</summary>
        public ProtocolItem Step6WarmWaterRatio { get; set; }

        /// <summary>备用1，D3059</summary>
        public ProtocolItem Remark1 { get; set; }

        /// <summary>备用2，D3060</summary>
        public ProtocolItem Remark2 { get; set; }
        /// <summary>备用3，D3061</summary>
        public ProtocolItem Remark3 { get; set; }
        /// <summary>备用4，D3062</summary>
        public ProtocolItem Remark4 { get; set; }
        /// <summary>备用5，D3063</summary>
        public ProtocolItem Remark5 { get; set; }
        /// <summary>备用6，D3064</summary>
        public ProtocolItem Remark6 { get; set; }
        /// <summary>备用7，D3065</summary>
        public ProtocolItem Remark7 { get; set; }

        /// <summary>备用8，D3066</summary>
        public ProtocolItem Remark8 { get; set; }

        /// <summary>备用9，D3067</summary>
        public ProtocolItem Remark9 { get; set; }

        /// <summary>备用10，D3068</summary>
        public ProtocolItem Remark10 { get; set; }

        /// <summary>备用11，D3069</summary>
        public ProtocolItem Remark11 { get; set; }

        /// <summary>字1，D3070</summary>
        public ProtocolItem Word1 { get; set; }
        /// <summary>字2，D3071</summary>
        public ProtocolItem Word2 { get; set; }
        /// <summary>字3，D3072</summary>
        public ProtocolItem Word3 { get; set; }
        /// <summary>字4，D3073</summary>
        public ProtocolItem Word4 { get; set; }
        /// <summary>字5，D3074</summary>
        public ProtocolItem Word5 { get; set; }
        /// <summary>字6，D3075</summary>
        public ProtocolItem Word6 { get; set; }
        /// <summary>字7，D3076</summary>
        public ProtocolItem Word7 { get; set; }
        /// <summary>字8，D3077</summary>
        public ProtocolItem Word8 { get; set; }
        /// <summary>字9，D3078</summary>
        public ProtocolItem Word9 { get; set; }
        /// <summary>字10，D3079</summary>
        public ProtocolItem Word10 { get; set; }
        /// <summary>字11，D3080</summary>
        public ProtocolItem Word11 { get; set; }
        /// <summary>字12，D3081</summary>
        public ProtocolItem Word12 { get; set; }
        /// <summary>字13，D3082</summary>
        public ProtocolItem Word13 { get; set; }
        /// <summary>字14，D3083</summary>
        public ProtocolItem Word14 { get; set; }
        /// <summary>字15，D3084</summary>
        public ProtocolItem Word15 { get; set; }
        /// <summary>字16，D3085</summary>
        public ProtocolItem Word16 { get; set; }
        /// <summary>字17，D3086</summary>
        public ProtocolItem Word17 { get; set; }
        /// <summary>字18，D3087</summary>
        public ProtocolItem Word18 { get; set; }
        /// <summary>字19，D3088</summary>
        public ProtocolItem Word19 { get; set; }
        /// <summary>字20，D3089</summary>
        public ProtocolItem Word20 { get; set; }
        /// <summary>字21，D3090</summary>
        public ProtocolItem Word21 { get; set; }
        /// <summary>字22，D3091</summary>
        public ProtocolItem Word22 { get; set; }
        /// <summary>字23，D3092</summary>
        public ProtocolItem Word23 { get; set; }
        /// <summary>字24，D3093</summary>
        public ProtocolItem Word24 { get; set; }
        /// <summary>字25，D3094</summary>
        public ProtocolItem Word25 { get; set; }
        /// <summary>字26，D3095</summary>
        public ProtocolItem Word26 { get; set; }
        /// <summary>字27，D3096</summary>
        public ProtocolItem Word27 { get; set; }
        /// <summary>字28，D3097</summary>
        public ProtocolItem Word28 { get; set; }
        /// <summary>字29，D3098</summary>
        public ProtocolItem Word29 { get; set; }
        /// <summary>字30，D3099</summary>
        public ProtocolItem Word30 { get; set; }
        /// <summary>字31，D3100</summary>
        public ProtocolItem Word31 { get; set; }
        /// <summary>字32，D3101</summary>
        public ProtocolItem Word32 { get; set; }
        /// <summary>字33，D3102</summary>
        public ProtocolItem Word33 { get; set; }
        /// <summary>字34，D3103</summary>
        public ProtocolItem Word34 { get; set; }
        /// <summary>字35，D3104</summary>
        public ProtocolItem Word35 { get; set; }
        /// <summary>字36，D3105</summary>
        public ProtocolItem Word36 { get; set; }
        /// <summary>字37，D3106</summary>
        public ProtocolItem Word37 { get; set; }
        /// <summary>字38，D3107</summary>
        public ProtocolItem Word38 { get; set; }
        /// <summary>字39，D3108</summary>
        public ProtocolItem Word39 { get; set; }
        /// <summary>字40，D3109</summary>
        public ProtocolItem Word40 { get; set; }
        /// <summary>字41，D3110</summary>
        public ProtocolItem Word41 { get; set; }
        /// <summary>字42，D3111</summary>
        public ProtocolItem Word42 { get; set; }
        /// <summary>字43，D3112</summary>
        public ProtocolItem Word43 { get; set; }
        /// <summary>字44，D3113</summary>
        public ProtocolItem Word44 { get; set; }
        /// <summary>字45，D3114</summary>
        public ProtocolItem Word45 { get; set; }
        /// <summary>字46，D3115</summary>
        public ProtocolItem Word46 { get; set; }
        /// <summary>字47，D3116</summary>
        public ProtocolItem Word47 { get; set; }
        /// <summary>字48，D3117</summary>
        public ProtocolItem Word48 { get; set; }
        /// <summary>字49，D3118</summary>
        public ProtocolItem Word49 { get; set; }
        /// <summary>字50，D3119</summary>
        public ProtocolItem Word50 { get; set; }



    }

    /// <summary>
    /// 汇川PLC工位类，包含一个工位的所有协议项（寄存器数据点）
    /// 每个字段对应PLC中的一个寄存器，便于批量管理和访问。
    /// 上位机接收区。
    /// </summary>
    internal class Inovance_TCP_Receive
    {
        /// <summary>工位号（唯一标识工位）</summary>
        public ProtocolItem StationNo { get; set; }
        /// <summary>瓶号（当前工位处理的瓶子编号）</summary>
        public ProtocolItem BottleNo { get; set; }
        /// <summary>实际浓度（*1000000）</summary>
        public ProtocolItem ActualConcentration { get; set; }
        /// <summary>实际总量（*1000）</summary>
        public ProtocolItem ActualTotal { get; set; }
        /// <summary>瓶号输入完成标志（0=无，1=输入完成）</summary>
        public ProtocolItem InputFinishFlag { get; set; }
        /// <summary>泡制完成标志（0=无，1=完成，2=失败，3=中止，4=断电中止）</summary>
        public ProtocolItem BrewFinishFlag { get; set; }
        /// <summary>染助剂重量（*100）</summary>
        public ProtocolItem AssistantWeight { get; set; }
        /// <summary>备料标志（0=不备料，1=备料）</summary>
        public ProtocolItem PrepareFlag { get; set; }
        /// <summary>瓶号输入方式（0=人工，1=扫码，2=干预）</summary>
        public ProtocolItem InputMode { get; set; }
        /// <summary>对应数据库的ID号</summary>
        public ProtocolItem MyID { get; set; }
    }


    /// <summary>
    /// 汇川PLC TCP通讯管理类。
    /// 封装了工位数据的批量读取、打印、连接与断开等操作。
    /// 通过ModbusTCP协议与PLC进行数据交互。
    /// </summary>
    internal class Inovance_TCP
    {
        /// <summary>
        /// 通知显示重联开料机按钮
        /// </summary>
        public event Action<string, string> ShowResetBrewButton;

        /// <summary>
        /// 更新母液瓶信息
        /// </summary>
        public event Action<int> UpdateBottleInfo;

        /// <summary>
        /// Modbus TCP通讯实例，负责底层数据收发
        /// </summary>
        private readonly ModbusTCP _modbus;

        /// <summary>
        /// PLC站号（UnitId），通常为1
        /// </summary>
        private readonly byte _unitId = 1;

        /// <summary>
        /// 工位创建工厂方法，简化初始化，减少重复代码
        /// </summary>
        /// <param name="baseAddr">工位起始寄存器地址</param>
        /// <returns>初始化后的工位对象</returns>
        private static Inovance_TCP_Receive CreateReceive(int baseAddr)
        {
            return new Inovance_TCP_Receive
            {
                StationNo = new My_Interaction.ProtocolItem(baseAddr + 0, typeof(ushort)),
                BottleNo = new My_Interaction.ProtocolItem(baseAddr + 1, typeof(ushort)),
                ActualConcentration = new My_Interaction.ProtocolItem(baseAddr + 2, typeof(int)),
                ActualTotal = new My_Interaction.ProtocolItem(baseAddr + 4, typeof(int)),
                InputFinishFlag = new My_Interaction.ProtocolItem(baseAddr + 6, typeof(ushort)),
                BrewFinishFlag = new My_Interaction.ProtocolItem(baseAddr + 7, typeof(ushort)),
                AssistantWeight = new My_Interaction.ProtocolItem(baseAddr + 8, typeof(int)),
                PrepareFlag = new My_Interaction.ProtocolItem(baseAddr + 10, typeof(ushort)),
                InputMode = new My_Interaction.ProtocolItem(baseAddr + 11, typeof(ushort)),
                MyID = new My_Interaction.ProtocolItem(baseAddr + 12, typeof(int))
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
        private static Inovance_TCP_Send CreateSend(int baseAddr)
        {
            var send = new Inovance_TCP_Send
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
                Step6Process = new My_Interaction.ProtocolItem(baseAddr + 19, typeof(ushort)),
                Step6Ratio = new My_Interaction.ProtocolItem(baseAddr + 20, typeof(ushort)),
                DilutionOriginalBottleWeight = new My_Interaction.ProtocolItem(baseAddr + 21, typeof(int)),
                Step1WarmWaterRatio = new My_Interaction.ProtocolItem(baseAddr + 23, typeof(ushort)),
                Step2WarmWaterRatio = new My_Interaction.ProtocolItem(baseAddr + 24, typeof(ushort)),
                Step3WarmWaterRatio = new My_Interaction.ProtocolItem(baseAddr + 25, typeof(ushort)),
                Step4WarmWaterRatio = new My_Interaction.ProtocolItem(baseAddr + 26, typeof(ushort)),
                Step5WarmWaterRatio = new My_Interaction.ProtocolItem(baseAddr + 27, typeof(ushort)),
                Step6WarmWaterRatio = new My_Interaction.ProtocolItem(baseAddr + 28, typeof(ushort)),
                Remark1 = new My_Interaction.ProtocolItem(baseAddr + 29, typeof(ushort)),
                Remark2 = new My_Interaction.ProtocolItem(baseAddr + 30, typeof(ushort)),
                Remark3 = new My_Interaction.ProtocolItem(baseAddr + 31, typeof(ushort)),
                Remark4 = new My_Interaction.ProtocolItem(baseAddr + 32, typeof(ushort)),
                Remark5 = new My_Interaction.ProtocolItem(baseAddr + 33, typeof(ushort)),
                Remark6 = new My_Interaction.ProtocolItem(baseAddr + 34, typeof(ushort)),
                Remark7 = new My_Interaction.ProtocolItem(baseAddr + 35, typeof(ushort)),
                Remark8 = new My_Interaction.ProtocolItem(baseAddr + 36, typeof(ushort)),
                Remark9 = new My_Interaction.ProtocolItem(baseAddr + 37, typeof(ushort)),
                Remark10 = new My_Interaction.ProtocolItem(baseAddr + 38, typeof(ushort)),
                Remark11 = new My_Interaction.ProtocolItem(baseAddr + 39, typeof(ushort))

            };

            // 字1~字50（D3070~D3119）
            for (int i = 0; i < 50; i++)
            {
                int addr = baseAddr + 40 + i; // D3070 = D3030+40
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
                    case 9: send.Word9 = word; break;
                    case 10: send.Word10 = word; break;
                    case 11: send.Word11 = word; break;
                    case 12: send.Word12 = word; break;
                    case 13: send.Word13 = word; break;
                    case 14: send.Word14 = word; break;
                    case 15: send.Word15 = word; break;
                    case 16: send.Word16 = word; break;
                    case 17: send.Word17 = word; break;
                    case 18: send.Word18 = word; break;
                    case 19: send.Word19 = word; break;
                    case 20: send.Word20 = word; break;
                    case 21: send.Word21 = word; break;
                    case 22: send.Word22 = word; break;
                    case 23: send.Word23 = word; break;
                    case 24: send.Word24 = word; break;
                    case 25: send.Word25 = word; break;
                    case 26: send.Word26 = word; break;
                    case 27: send.Word27 = word; break;
                    case 28: send.Word28 = word; break;
                    case 29: send.Word29 = word; break;
                    case 30: send.Word30 = word; break;
                    case 31: send.Word31 = word; break;
                    case 32: send.Word32 = word; break;
                    case 33: send.Word33 = word; break;
                    case 34: send.Word34 = word; break;
                    case 35: send.Word35 = word; break;
                    case 36: send.Word36 = word; break;
                    case 37: send.Word37 = word; break;
                    case 38: send.Word38 = word; break;
                    case 39: send.Word39 = word; break;
                    case 40: send.Word40 = word; break;
                    case 41: send.Word41 = word; break;
                    case 42: send.Word42 = word; break;
                    case 43: send.Word43 = word; break;
                    case 44: send.Word44 = word; break;
                    case 45: send.Word45 = word; break;
                    case 46: send.Word46 = word; break;
                    case 47: send.Word47 = word; break;
                    case 48: send.Word48 = word; break;
                    case 49: send.Word49 = word; break;
                    case 50: send.Word50 = word; break;
                }
            }

            return send;
        }

        /// <summary>
        /// 机台信息创建工厂方法，简化初始化，减少重复代码
        /// </summary>
        /// <param name="baseAddr">起始地址</param>
        /// <returns>机台信息</returns>
        private static Inovance_TCP_ControlInfo CreateControlInfo(int baseAddr)
        {
            return new Inovance_TCP_ControlInfo
            {
                MaxBottleNum = new My_Interaction.ProtocolItem(baseAddr, typeof(ushort)),
                CommunicationState = new My_Interaction.ProtocolItem(baseAddr + 1, typeof(ushort))
            };
        }

        /// <summary>
        /// 开料机版本号创建工厂方法，简化初始化，减少重复代码
        /// </summary>
        /// <param name="baseAddr">起始地址</param>
        /// <returns>开料机版本号信息</returns>
        private static Inovance_TCP_VersionInfo CreateVersionInfo(int baseAddr)
        {
            return new Inovance_TCP_VersionInfo
            {
                VersionYear = new My_Interaction.ProtocolItem(baseAddr, typeof(ushort)),
                VersionMD = new My_Interaction.ProtocolItem(baseAddr + 1, typeof(ushort))
            };
        }


        /// <summary>
        /// 所有工位的接受区协议项集合。
        /// 每个工位包含一组寄存器，地址、名称、类型等已预定义。
        /// 便于批量读取和管理。
        /// </summary>
        private readonly List<Inovance_TCP_Receive> _receive = new List<Inovance_TCP_Receive>
        {
            //一工位
            CreateReceive(3010),
            //二工位
            CreateReceive(3210),
            //三工位
            CreateReceive(3410),
            //四工位
            CreateReceive(3610),
            //五工位
            CreateReceive(3810)
        };

        /// <summary>
        /// 所有工位的发送区协议项集合。
        /// 每个工位包含一组寄存器，地址、名称、类型等已预定义。
        /// 便于批量读取和管理。
        /// </summary>
        private readonly List<Inovance_TCP_Send> _send = new List<Inovance_TCP_Send>
        {
            //一工位
            CreateSend(3030),
            //二工位
            CreateSend(3230),
            //三工位
            CreateSend(3430),
            //四工位
            CreateSend(3630),
            //五工位
            CreateSend(3830)
        };

        /// <summary>
        /// 机台信息协议项集合。
        /// </summary>
        private readonly Inovance_TCP_ControlInfo _controlInfo = CreateControlInfo(2800);

        /// <summary>
        /// 开料机版本号协议项集合。
        /// </summary>
        private readonly Inovance_TCP_VersionInfo _version = CreateVersionInfo(2802);


        /// <summary>
        /// 构造函数，初始化ModbusTCP通讯实例。
        /// </summary>
        /// <param name="ip">PLC服务器IP地址</param>
        /// <param name="port">PLC服务器端口号</param>
        public Inovance_TCP(string ip, int port)
        {
            // 实例化ModbusTCP对象，指定IP、端口和实例标识
            _modbus = new ModbusTCP(ip, port, "InovancePLC");
        }

        /// <summary>
        /// 连接到PLC服务器。
        /// 实际调用ModbusTCP的Connect方法，支持自动重连。
        /// </summary>
        public void Connect()
        {
            // 建立TCP连接，底层支持重试机制
            _modbus.Connect();
        }

        /// <summary>
        /// 断开与PLC服务器的连接。
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
                    ushort[] regs = _modbus.ReadHoldingRegisters(_unitId, (ushort)ws.StationNo.Address, (ushort)regCount);

                    int idx = 0;
                    SetProtocolItemValue(ws.StationNo, regs[idx++]);
                    SetProtocolItemValue(ws.BottleNo, regs[idx++]);
                    SetProtocolItemValue(ws.ActualConcentration, (regs[idx++] | regs[idx++] << 16));
                    SetProtocolItemValue(ws.ActualTotal, (regs[idx++] | regs[idx++] << 16));
                    SetProtocolItemValue(ws.InputFinishFlag, regs[idx++]);
                    SetProtocolItemValue(ws.BrewFinishFlag, regs[idx++]);
                    SetProtocolItemValue(ws.AssistantWeight, (regs[idx++] | regs[idx++] << 16));
                    SetProtocolItemValue(ws.PrepareFlag, regs[idx++]);
                    SetProtocolItemValue(ws.InputMode, regs[idx++]);
                    SetProtocolItemValue(ws.MyID, (regs[idx++] | regs[idx++] << 16));
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
        /// 读取开料机的版本号（格式：yyyyMMdd）
        /// </summary>
        /// <returns>版本号字符串，如"20260321"，读取失败返回"未知"</returns>
        public string ReadVersionInfo()
        {
            try
            {
                int regCount = GetVersionCount(_version);
                ushort[] regs = _modbus.ReadHoldingRegisters(_unitId, (ushort)_version.VersionYear.Address, (ushort)regCount);
                int idx = 0;
                SetProtocolItemValue(_version.VersionYear, regs[idx++]);
                SetProtocolItemValue(_version.VersionMD, regs[idx++]);

                var yearObj = _version.VersionYear?.Value;
                var monthDayObj = _version.VersionMD?.Value;

                if (yearObj == null || monthDayObj == null)
                    return "未知";

                int year = Convert.ToInt32(yearObj);
                int monthDay = Convert.ToInt32(monthDayObj);
                string monthDayStr = monthDay.ToString("D4"); // 补齐4位
                string version = $"{year}{monthDayStr}";
                return version;
            }
            catch (Exception ex)
            {
                Logger.Error($"读取版本信息失败", ex);
                return "未知";
            }
        }


        //  清零所有泡制步骤和名称
        private void ClearBrewStepsAndName(Inovance_TCP_Send send)
        {
            foreach (var prop in typeof(Inovance_TCP_Send).GetProperties())
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
        /// 开料：根据瓶号查找数据库，填充Send结构体并写入PLC
        /// </summary>
        /// <param name="bottleNo">瓶号</param>
        /// <param name="send">发送区结构体</param>
        private void StartBrew(Inovance_TCP_Receive receive, Inovance_TCP_Send send)
        {

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

            var originalbottleno = (rows[0][BOTTLE_DETAILS.OriginalBottleNum] is DBNull) ? 0 : Convert.ToInt16(rows[0][BOTTLE_DETAILS.OriginalBottleNum]);
            var originalconcentration = 0.0;
            var originalcurrentweight = 0.0;
            if (originalbottleno != 0)
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
            var aw = Convert.ToUInt16(row[BOTTLE_DETAILS.AllowMaxWeight]);
            var oc = Convert.ToInt32(originalconcentration * 1000000);
            var sc = Convert.ToInt32(Convert.ToDouble(row[BOTTLE_DETAILS.SettingConcentration]) * 1000000);
            var ob = row[BOTTLE_DETAILS.OriginalBottleNum];
            var ow = Convert.ToInt32(originalcurrentweight);
            SetProtocolItemValue(send.MaxLiquidAmount, aw);
            SetProtocolItemValue(send.OriginalConcentration, oc);
            SetProtocolItemValue(send.TargetConcentration, sc);
            SetProtocolItemValue(send.Unit, ConvertUnitToCode(unit));
            SetProtocolItemValue(send.TotalSteps, brewRows.Count());
            SetProtocolItemValue(send.ReceiveFinishFlag, 0);

            SetProtocolItemValue(send.DilutionOriginalBottleNo, ob);
            SetProtocolItemValue(send.DilutionOriginalBottleWeight, ow);

            //填充步骤
            FillBrewSteps(brewRows, send);

            //填充名称
            FillAssistantName(name, send);


            // 写入PLC
            WriteSendToPLC(send);

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
            WriteReceiveToPLC(receive);
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
        private void FillAssistantName(string name, Inovance_TCP_Send send)
        {
            // 1. GB2312编码
            Encoding fromEncoding = Encoding.UTF8;
            Encoding toEncoding = Encoding.GetEncoding("GB2312");
            byte[] srcBytes = fromEncoding.GetBytes(name ?? "");
            byte[] gbBytes = Encoding.Convert(fromEncoding, toEncoding, srcBytes);

            // 2. 组装50个ushort（每个2字节，低字节在前，高字节在后，空位补0x000D）
            ushort[] assistantNameWords = new ushort[50];
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
            SetProtocolItemValue(send.Word9, assistantNameWords[8]);
            SetProtocolItemValue(send.Word10, assistantNameWords[9]);
            SetProtocolItemValue(send.Word11, assistantNameWords[10]);
            SetProtocolItemValue(send.Word12, assistantNameWords[11]);
            SetProtocolItemValue(send.Word13, assistantNameWords[12]);
            SetProtocolItemValue(send.Word14, assistantNameWords[13]);
            SetProtocolItemValue(send.Word15, assistantNameWords[14]);
            SetProtocolItemValue(send.Word16, assistantNameWords[15]);
            SetProtocolItemValue(send.Word17, assistantNameWords[16]);
            SetProtocolItemValue(send.Word18, assistantNameWords[17]);
            SetProtocolItemValue(send.Word19, assistantNameWords[18]);
            SetProtocolItemValue(send.Word20, assistantNameWords[19]);
            SetProtocolItemValue(send.Word21, assistantNameWords[20]);
            SetProtocolItemValue(send.Word22, assistantNameWords[21]);
            SetProtocolItemValue(send.Word23, assistantNameWords[22]);
            SetProtocolItemValue(send.Word24, assistantNameWords[23]);
            SetProtocolItemValue(send.Word25, assistantNameWords[24]);
            SetProtocolItemValue(send.Word26, assistantNameWords[25]);
            SetProtocolItemValue(send.Word27, assistantNameWords[26]);
            SetProtocolItemValue(send.Word28, assistantNameWords[27]);
            SetProtocolItemValue(send.Word29, assistantNameWords[28]);
            SetProtocolItemValue(send.Word30, assistantNameWords[29]);
            SetProtocolItemValue(send.Word31, assistantNameWords[30]);
            SetProtocolItemValue(send.Word32, assistantNameWords[31]);
            SetProtocolItemValue(send.Word33, assistantNameWords[32]);
            SetProtocolItemValue(send.Word34, assistantNameWords[33]);
            SetProtocolItemValue(send.Word35, assistantNameWords[34]);
            SetProtocolItemValue(send.Word36, assistantNameWords[35]);
            SetProtocolItemValue(send.Word37, assistantNameWords[36]);
            SetProtocolItemValue(send.Word38, assistantNameWords[37]);
            SetProtocolItemValue(send.Word39, assistantNameWords[38]);
            SetProtocolItemValue(send.Word40, assistantNameWords[39]);
            SetProtocolItemValue(send.Word41, assistantNameWords[40]);
            SetProtocolItemValue(send.Word42, assistantNameWords[41]);
            SetProtocolItemValue(send.Word43, assistantNameWords[42]);
            SetProtocolItemValue(send.Word44, assistantNameWords[43]);
            SetProtocolItemValue(send.Word45, assistantNameWords[44]);
            SetProtocolItemValue(send.Word46, assistantNameWords[45]);
            SetProtocolItemValue(send.Word47, assistantNameWords[46]);
            SetProtocolItemValue(send.Word48, assistantNameWords[47]);
            SetProtocolItemValue(send.Word49, assistantNameWords[48]);
            SetProtocolItemValue(send.Word50, assistantNameWords[49]);
        }

        /// <summary>
        /// 填充泡制步骤到Send结构体
        /// </summary>
        /// <param name="brewRows">当前泡料流程</param>
        /// <param name="send">Send结构体</param>
        private void FillBrewSteps(IEnumerable<DataRow> brewRows, Inovance_TCP_Send send)
        {
            int stepIndex = 0;
            foreach (var brewRow in brewRows)
            {
                stepIndex++;
                ushort tn = ConvertTechnologyNameToCode(brewRow[BREWING_PROCESS.TechnologyName]?.ToString());
                ushort ra = (ushort)((brewRow[BREWING_PROCESS.Ratio] is DBNull) ? 0 : Convert.ToUInt16(brewRow[BREWING_PROCESS.Ratio]));
                ushort pot = Convert.ToUInt16(brewRow[BREWING_PROCESS.ProportionOrTime]);
                switch (stepIndex)
                {
                    case 1:
                        SetProtocolItemValue(send.Step1Process, tn);
                        SetProtocolItemValue(send.Step1Ratio, pot);
                        SetProtocolItemValue(send.Step1WarmWaterRatio, ra);
                        break;
                    case 2:
                        SetProtocolItemValue(send.Step2Process, tn);
                        SetProtocolItemValue(send.Step2Ratio, pot);
                        SetProtocolItemValue(send.Step2WarmWaterRatio, ra);
                        break;
                    case 3:
                        SetProtocolItemValue(send.Step3Process, tn);
                        SetProtocolItemValue(send.Step3Ratio, pot);
                        SetProtocolItemValue(send.Step3WarmWaterRatio, ra);
                        break;
                    case 4:
                        SetProtocolItemValue(send.Step4Process, tn);
                        SetProtocolItemValue(send.Step4Ratio, pot);
                        SetProtocolItemValue(send.Step4WarmWaterRatio, ra);
                        break;
                    case 5:
                        SetProtocolItemValue(send.Step5Process, tn);
                        SetProtocolItemValue(send.Step5Ratio, pot);
                        SetProtocolItemValue(send.Step5WarmWaterRatio, ra);
                        break;
                    case 6:
                        SetProtocolItemValue(send.Step6Process, tn);
                        SetProtocolItemValue(send.Step6Ratio, pot);
                        SetProtocolItemValue(send.Step6WarmWaterRatio, ra);
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
        /// 将Send结构体数据写入PLC
        /// </summary>
        /// <param name="send">发送区结构体</param>
        private void WriteSendToPLC(Inovance_TCP_Send send)
        {
            List<ushort> values = new List<ushort>();
            foreach (var prop in typeof(Inovance_TCP_Send).GetProperties())
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
            // 写入PLC
            _modbus.WriteMultipleRegisters(_unitId, (ushort)send.MaxLiquidAmount.Address, values.ToArray());
        }

        /// <summary>
        /// 将Receive结构体数据批量写入PLC
        /// </summary>
        /// <param name="receive">接收区结构体</param>
        private void WriteReceiveToPLC(Inovance_TCP_Receive receive)
        {
            List<ushort> values = new List<ushort>();
            foreach (var prop in typeof(Inovance_TCP_Receive).GetProperties())
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
            // 写入PLC，起始地址为StationNo.Address
            _modbus.WriteMultipleRegisters(_unitId, (ushort)receive.StationNo.Address, values.ToArray());
        }

        /// <summary>
        /// 写入机台信息到PLC
        /// </summary>
        public void WriteControlInfoToPLC()
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
            SetProtocolItemValue(_controlInfo.CommunicationState, i);
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
                case 3:
                    maxBottleNum = ((SmartColor.My_ConPar.Area.BottleArea.Bottle)SmartColor.My_ConPar.Area.Layout_4.Area_1).BottleNum;
                    break;
            }
            SetProtocolItemValue(_controlInfo.MaxBottleNum, maxBottleNum);
            foreach (var prop in typeof(Inovance_TCP_ControlInfo).GetProperties())
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
            // 写入PLC
            _modbus.WriteMultipleRegisters(_unitId, (ushort)_controlInfo.MaxBottleNum.Address, values.ToArray());
        }

        /// <summary>
        /// 完成：根据PrepareFlag写入不同表
        /// </summary>
        /// <param name="ws">接收区结构体</param>

        private void FinishBrew(Inovance_TCP_Receive ws)
        {
            MessageEventManager.Instance.RequestShowBalloonTip($"收到{ws.BottleNo.Value}号母液瓶泡制完成信息");
            int ff = Convert.ToUInt16(ws.BrewFinishFlag.Value);
            int pf = Convert.ToUInt16(ws.PrepareFlag.Value);
            var bn = ws.BottleNo.Value;
            var ac = Convert.ToInt32(ws.ActualConcentration.Value) / 1000000.00;
            var at = Convert.ToInt32(ws.ActualTotal.Value) / 1000.00;
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

            WriteReceiveToPLC(ws);
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
        private static int GetRegisterCount(Inovance_TCP_Receive ws)
        {
            int count = 0;
            foreach (var prop in typeof(Inovance_TCP_Receive).GetProperties())
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
        /// 计算版本号需要读取的寄存器数量（根据数据类型自动判断）
        /// </summary>
        /// <param name="version">版本号对象</param>
        /// <returns>需要读取的寄存器数量</returns>
        private static int GetVersionCount(Inovance_TCP_VersionInfo version)
        {
            int count = 0;
            foreach (var prop in typeof(Inovance_TCP_VersionInfo).GetProperties())
            {
                var item = prop.GetValue(version) as My_Interaction.ProtocolItem;
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
        /// 启动汇川PLC通讯线程
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
                        WriteControlInfoToPLC();

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