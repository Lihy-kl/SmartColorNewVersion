using com.google.zxing;
using SmartColor.My_Control;
using SmartColor.My_CuttingMachine;
using SmartColor.My_File;
using SmartColor.My_Interaction;
using SmartColor.My_SemiAutoModule;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>
    /// PLC通讯主类，负责ModbusTCP通讯、批量读写、队列调度、线程管理等。
    /// 支持写优先、读任务合并、自动循环读等功能，便于高效与PLC交互。
    /// </summary>
    internal class PLC
    {
        /// <summary>
        /// 通知机械手状态更新
        /// </summary>
        public event Action<string> ShowState;

        /// <summary>读失败标志，用于指示上次读操作是否失败</summary>
        private bool _readFail = false;

        /// <summary>ModbusTCP通讯对象，负责与PLC建立连接和数据收发</summary>
        private My_Interaction.ModbusTCP _plcTcp;

        /// <summary>写操作队列，所有写操作优先执行</summary>
        private readonly Queue<Action> _writeQueue = new Queue<Action>();
        /// <summary>读操作队列，支持批量读任务合并</summary>
        private readonly Queue<PLC_ReadTask> _readQueue = new Queue<PLC_ReadTask>();
        /// <summary>线程信号，用于唤醒调度线程</summary>
        private readonly AutoResetEvent _writeEvent = new AutoResetEvent(false);

        /// <summary>线程运行标志</summary>
        private volatile bool _isRunning = true;
        /// <summary>手动操作地址</summary>
        private ushort _manualOperationAddr = 830;
        /// <summary>询问操作地址</summary>
        private ushort _inquireAddr = 831;

        /// <summary>机械手锁对象，确保同一时间只有一个操作访问对应设备</summary>
        private static readonly object _robotLock = new object();
        /// <summary> 转盘锁对象，确保同一时间只有一个操作访问对应设备</summary>
        private static readonly object _turntableLock = new object();  //
        /// <summary> 洗瓶锁对象，确保同一时间只有一个操作访问对应设备</summary>
        private static readonly object _washerLock = new object();     // 
        /// <summary> UV锁对象，确保同一时间只有一个操作访问对应设备</summary>
        private static readonly object _uvLock = new object();


        // 新增：读线程和信号
        private Thread _readWorker = null;
        private Thread _writeWorker = null;
        private readonly AutoResetEvent _readEvent = new AutoResetEvent(false);

        /// <summary>
        /// 半自动操作PLC协议枚举，定义所有半自动操作的枚举类型。
        /// </summary>
        public enum SemiAutomaticOperation
        {
            /// <summary>无操作</summary>
            None = 0,
            /// <summary>回原点</summary>
            Home = 1,
            /// <summary>定移动</summary>
            MoveToPosition = 2,
            /// <summary>抽液</summary>
            Aspirate = 3,
            /// <summary>注液</summary>
            Dispense = 4,
            /// <summary>加水</summary>
            AddWater = 5,
            /// <summary>放针</summary>
            ReleaseNeedle = 6,
            /// <summary>泄压</summary>
            Decompress = 7,
            /// <summary>相对移动</summary>
            RelativeMove = 8,
            /// <summary>复位</summary>
            Reset = 9,
            /// <summary>动作检查</summary>
            ActionCheck = 10,
            /// <summary>开盖</summary>
            OpenLid = 11,
            /// <summary>放盖</summary>
            PutLid = 12,
            /// <summary>取盖</summary>
            TakeLid = 13,
            /// <summary>关盖</summary>
            CloseLid = 14,
            /// <summary>天平检查</summary>
            BalanceCheck = 15,
            /// <summary>机械手抓取</summary>
            RobotHandGrasping = 16,
            /// <summary>机械手松放</summary>
            RobotArmRelease = 17,
            /// <summary>取干布</summary>
            TakeDryCloth = 18,
            /// <summary>取湿布</summary>
            TakeWetCloth = 19,
            /// <summary>放干布</summary>
            PutDryCloth = 20,
            /// <summary>放湿布</summary>
            PutWetCloth = 21,
            /// <summary>UV杯抽液</summary>
            UVAspirate = 22,
            /// <summary>洗针筒</summary>
            WashSyringe = 23,
            /// <summary>加溶解剂</summary>
            AddSolvent = 24,
            /// <summary>撑盖</summary>
            SupportCover = 25,
            /// <summary>PH抽液</summary>
            PHAspirate = 26,
            /// <summary>PH排液</summary>
            PHDrain = 27,
            /// <summary>取母液瓶</summary>
            TakeMotherBottle = 28,
            /// <summary>放母液瓶</summary>
            PutMotherBottle = 29,
            /// <summary>加粉</summary>
            AddPowder = 30,
            /// <summary>加粉前气缸到阻挡位</summary>
            BeforeAddPowderBlock = 31,
            /// <summary>UV相关操作起始值预留</summary>
            UVRelatedStart = 50,
            /// <summary>洗瓶相关操作起始值预留</summary>
            WASHERelatedStart = 100,
            /// <summary>转盘相关操作起始值预留</summary>
            TRUNTableRelatedStart = 150


        }

        /// <summary>
        /// 根据半自动操作枚举获取操作名称
        /// </summary>
        /// <param name="op">半自动操作枚举</param>
        /// <returns>操作名称</returns>
        private static string GetSemiAutomaticOperationName(SemiAutomaticOperation op)
        {
            switch (op)
            {
                case SemiAutomaticOperation.Home: return "回原点";
                case SemiAutomaticOperation.MoveToPosition: return "定点移动";
                case SemiAutomaticOperation.Aspirate: return "抽液";
                case SemiAutomaticOperation.Dispense: return "注液";
                case SemiAutomaticOperation.AddWater: return "加水";
                case SemiAutomaticOperation.ReleaseNeedle: return "放针";
                case SemiAutomaticOperation.Decompress: return "泄压";
                case SemiAutomaticOperation.RelativeMove: return "相对移动";
                case SemiAutomaticOperation.Reset: return "复位";
                case SemiAutomaticOperation.ActionCheck: return "动作检查";
                case SemiAutomaticOperation.OpenLid: return "开盖";
                case SemiAutomaticOperation.PutLid: return "放盖";
                case SemiAutomaticOperation.TakeLid: return "取盖";
                case SemiAutomaticOperation.CloseLid: return "关盖";
                case SemiAutomaticOperation.BalanceCheck: return "天平检查";
                case SemiAutomaticOperation.RobotHandGrasping: return "机械手抓取";
                case SemiAutomaticOperation.RobotArmRelease: return "机械手松放";
                case SemiAutomaticOperation.TakeDryCloth: return "取干布";
                case SemiAutomaticOperation.TakeWetCloth: return "取湿布";
                case SemiAutomaticOperation.PutDryCloth: return "放干布";
                case SemiAutomaticOperation.PutWetCloth: return "放湿布";
                case SemiAutomaticOperation.UVAspirate: return "UV杯抽液";
                case SemiAutomaticOperation.WashSyringe: return "洗针筒";
                case SemiAutomaticOperation.AddSolvent: return "加溶解剂";
                case SemiAutomaticOperation.SupportCover: return "撑盖";
                case SemiAutomaticOperation.PHAspirate: return "PH抽液";
                case SemiAutomaticOperation.PHDrain: return "PH排液";
                case SemiAutomaticOperation.TakeMotherBottle: return "取母液瓶";
                case SemiAutomaticOperation.PutMotherBottle: return "放母液瓶";
                case SemiAutomaticOperation.AddPowder: return "加粉";
                case SemiAutomaticOperation.BeforeAddPowderBlock: return "加粉前气缸到阻挡位";
                default: return "未知";
            }
        }

        /// <summary>
        /// 根据半自动操作类型获取对应的锁对象，保证同一硬件的操作串行，不同硬件可并行
        /// </summary>
        /// <param name="op">半自动操作类型</param>
        /// <returns>对应的锁对象</returns>
        private object GetOperationLock(SemiAutomaticOperation op)
        {
            // 机械手相关
            switch (op)
            {
                // 机械手相关操作全部串行
                case SemiAutomaticOperation.Home:
                case SemiAutomaticOperation.MoveToPosition:
                case SemiAutomaticOperation.Aspirate:
                case SemiAutomaticOperation.Dispense:
                case SemiAutomaticOperation.ReleaseNeedle:
                case SemiAutomaticOperation.Decompress:
                case SemiAutomaticOperation.RelativeMove:
                case SemiAutomaticOperation.Reset:
                case SemiAutomaticOperation.ActionCheck:
                case SemiAutomaticOperation.OpenLid:
                case SemiAutomaticOperation.PutLid:
                case SemiAutomaticOperation.TakeLid:
                case SemiAutomaticOperation.CloseLid:
                case SemiAutomaticOperation.BalanceCheck:
                case SemiAutomaticOperation.RobotHandGrasping:
                case SemiAutomaticOperation.RobotArmRelease:
                case SemiAutomaticOperation.TakeDryCloth:
                case SemiAutomaticOperation.TakeWetCloth:
                case SemiAutomaticOperation.PutDryCloth:
                case SemiAutomaticOperation.PutWetCloth:
                case SemiAutomaticOperation.AddSolvent:
                case SemiAutomaticOperation.SupportCover:
                case SemiAutomaticOperation.PHAspirate:
                case SemiAutomaticOperation.PHDrain:
                case SemiAutomaticOperation.TakeMotherBottle:
                case SemiAutomaticOperation.PutMotherBottle:
                case SemiAutomaticOperation.AddPowder:
                case SemiAutomaticOperation.BeforeAddPowderBlock:
                case SemiAutomaticOperation.UVAspirate:
                case SemiAutomaticOperation.WashSyringe:
                    return _robotLock;
                //预留 UV相关操作
                case SemiAutomaticOperation.UVRelatedStart:
                    return _uvLock;
                // 预留洗瓶相关操作
                case SemiAutomaticOperation.WASHERelatedStart:
                    return _washerLock;
                // 预留转盘相关操作
                case SemiAutomaticOperation.TRUNTableRelatedStart:
                    return _turntableLock;
                default:
                    return _robotLock; // 默认机械手锁
            }
        }

        /// <summary>
        /// 手动操作PLC协议枚举，定义所有手动操作的枚举类型。
        /// </summary>
        public enum ManualOperation
        {
            /// <summary>无动作</summary>
            None = 0,
            /// <summary>X矢能开</summary>
            XEnableOn = 1,
            /// <summary>X矢能关</summary>
            XEnableOff = 2,
            /// <summary>Y矢能开</summary>
            YEnableOn = 3,
            /// <summary>Y矢能关</summary>
            YEnableOff = 4,
            /// <summary>气缸上</summary>
            CylinderUp = 5,
            /// <summary>气缸下</summary>
            CylinderDown = 6,
            /// <summary>抓手开</summary>
            TongsOpen = 7,
            /// <summary>抓手关</summary>
            TongsClose = 8,
            /// <summary>搅拌输出点开</summary>
            BlenderOn = 9,
            /// <summary>搅拌输出点关</summary>
            BlenderOff = 10,
            /// <summary>接液盘伸出</summary>
            TrayOut = 11,
            /// <summary>接液盘收回</summary>
            TrayIn = 12,
            /// <summary>抽废液开</summary>
            WasteOn = 13,
            /// <summary>抽废液关</summary>
            WasteOff = 14,
            /// <summary>水开</summary>
            WaterOn = 15,
            /// <summary>水关</summary>
            WaterOff = 16,
            /// <summary>峰鸣器开</summary>
            BuzzerOn = 17,
            /// <summary>峰鸣器关</summary>
            BuzzerOff = 18,
            /// <summary>排液开</summary>
            DrainOn = 19,
            /// <summary>排液关</summary>
            DrainOff = 20,
            /// <summary>阻挡开（无使用）</summary>
            BlockOn = 21,
            /// <summary>阻挡关（无使用）</summary>
            BlockOff = 22,
            /// <summary>泄压上</summary>
            DecompressionUp = 23,
            /// <summary>泄压下</summary>
            DecompressionDown = 24,
            /// <summary>泄压右上</summary>
            DecompressionRightUp = 25,
            /// <summary>泄压右下</summary>
            DecompressionRightDown = 26,
            /// <summary>X轴报警复位</summary>
            XAlarmReset = 27,
            /// <summary>Y轴报警复位</summary>
            YAlarmReset = 28,
            /// <summary>手动天平秤清零</summary>
            BalanceZero = 29,
            /// <summary>手动天平秤复位</summary>
            BalanceReset = 30,
            /// <summary>Z矢能开</summary>
            ZEnableOn = 31,
            /// <summary>Z矢能关</summary>
            ZEnableOff = 32,
            /// <summary>Z轴报警复位</summary>
            ZAlarmReset = 33,
            /// <summary>阻挡出</summary>
            BlockOut = 34,
            /// <summary>阻挡回</summary>
            BlockIn = 35,
            /// <summary>气缸慢速中</summary>
            CylinderSlowMid = 36,
            /// <summary>气缸到阻挡位</summary>
            CylinderBlock = 37,
            /// <summary>A助剂步进矢能开</summary>
            AAgentStepEnableOn = 38,
            /// <summary>A助剂步进矢能关</summary>
            AAgentStepEnableOff = 39,
            /// <summary>A助剂步进报警复位</summary>
            AAgentStepAlarmReset = 40,
            /// <summary>B助剂步进矢能开</summary>
            BAgentStepEnableOn = 41,
            /// <summary>B助剂步进矢能关</summary>
            BAgentStepEnableOff = 42,
            /// <summary>B助剂步进报警复位</summary>
            BAgentStepAlarmReset = 43,
            /// <summary>洗针进水阀开</summary>
            WashInValveOn = 44,
            /// <summary>洗针进水阀关</summary>
            WashInValveOff = 45,
            /// <summary>洗针排水阀开</summary>
            WashOutValveOn = 46,
            /// <summary>洗针排水阀关</summary>
            WashOutValveOff = 47,
            /// <summary>洗针吹气阀开</summary>
            WashBlowValveOn = 48,
            /// <summary>洗针吹气阀关</summary>
            WashBlowValveOff = 49,
            /// <summary>溶解剂开</summary>
            SolventOn = 50,
            /// <summary>溶解剂关</summary>
            SolventOff = 51,
            /// <summary>气缸慢速中2</summary>
            CylinderSlowMid2 = 52,
            /// <summary>气缸慢速中3</summary>
            CylinderSlowMid3 = 53
        }

        /// <summary>
        /// 询问操作PLC协议枚举，定义所有询问操作的枚举类型。
        /// </summary>
        public enum Inquire
        {
            /// <summary>继续</summary>
            Continue = 1,
            /// <summary>退出</summary>
            Exit = 2,

        }

        /// <summary>
        /// 读取模式枚举，定义PLC读取数据时的不同模式。
        /// </summary>
        public enum PlcReadMode { Normal, Debug }

        /// <summary>当前读取模式，默认为普通模式</summary>
        private PlcReadMode _currentReadMode = PlcReadMode.Normal;

        /// <summary>
        /// 创建并初始化 PLC_InPut 对象，所有协议项地址依次递增
        /// </summary>
        /// <param name="baseAddr">起始寄存器地址（如3500）</param>
        /// <returns>初始化后的 PLC_InPut 对象</returns>
        public static PLC_InPut CreateInPut(int baseAddr)
        {
            var plc = My_ConPar.Object.CurrentMachine as My_ConPar.Type.PLC.IO;

            return new PLC_InPut
            {
                X_A_FeedbackLine = new ProtocolItem(baseAddr + 0, typeof(int), -1),
                X_B_FeedbackLine = new ProtocolItem(baseAddr + 2, typeof(int), -1),
                Y_A_FeedbackLine = new ProtocolItem(baseAddr + 4, typeof(int), -1),
                Y_B_FeedbackLine = new ProtocolItem(baseAddr + 6, typeof(int), -1),
                Z_A_FeedbackLine = new ProtocolItem(baseAddr + 8, typeof(int), -1),
                Z_B_FeedbackLine = new ProtocolItem(baseAddr + 10, typeof(int), -1),
                X_Exceptional = new ProtocolItem(baseAddr + 12, typeof(int), -1),
                Y_Exceptional = new ProtocolItem(baseAddr + 14, typeof(int), -1),
                Z_Exceptional = new ProtocolItem(baseAddr + 16, typeof(int), -1),
                X_Ready = new ProtocolItem(baseAddr + 18, typeof(int), -1),
                Y_Ready = new ProtocolItem(baseAddr + 20, typeof(int), -1),
                Z_Ready = new ProtocolItem(baseAddr + 22, typeof(int), -1),
                X_Corotation = new ProtocolItem(baseAddr + 24, typeof(int), -1),
                X_Reverse = new ProtocolItem(baseAddr + 26, typeof(int), -1),
                X_Origin = new ProtocolItem(baseAddr + 28, typeof(int), -1),
                Y_Corotation = new ProtocolItem(baseAddr + 30, typeof(int), -1),
                Y_Reverse = new ProtocolItem(baseAddr + 32, typeof(int), -1),
                Y_Origin = new ProtocolItem(baseAddr + 34, typeof(int), -1),
                Z_Corotation = new ProtocolItem(baseAddr + 36, typeof(int), -1),
                Z_Reverse = new ProtocolItem(baseAddr + 38, typeof(int), PlcAddressToInt(true, plc.InPut_Z_Reverse)),
                Z_Origin = new ProtocolItem(baseAddr + 40, typeof(int), -1),
                Sunx_Stop = new ProtocolItem(baseAddr + 42, typeof(int), PlcAddressToInt(true, plc.InPut_Sunx_Stop)),
                Sunx_A = new ProtocolItem(baseAddr + 44, typeof(int), PlcAddressToInt(true, plc.InPut_Sunx_A)),
                Sunx_B = new ProtocolItem(baseAddr + 46, typeof(int), PlcAddressToInt(true, plc.InPut_Sunx_B)),
                Syringe = new ProtocolItem(baseAddr + 48, typeof(int), PlcAddressToInt(true, plc.InPut_Syringe)),
                Tongs_A = new ProtocolItem(baseAddr + 50, typeof(int), PlcAddressToInt(true, plc.InPut_Tongs_A)),
                Tongs_B = new ProtocolItem(baseAddr + 52, typeof(int), PlcAddressToInt(true, plc.InPut_Tongs_B)),
                Cylinder_Up = new ProtocolItem(baseAddr + 54, typeof(int), PlcAddressToInt(true, plc.InPut_Cylinder_Up)),
                Cylinder_Mid = new ProtocolItem(baseAddr + 56, typeof(int), PlcAddressToInt(true, plc.InPut_Cylinder_Mid)),
                Cylinder_Down = new ProtocolItem(baseAddr + 58, typeof(int), PlcAddressToInt(true, plc.InPut_Cylinder_Down)),
                Tray_Out = new ProtocolItem(baseAddr + 60, typeof(int), PlcAddressToInt(true, plc.InPut_Tray_Out)),
                Tray_In = new ProtocolItem(baseAddr + 62, typeof(int), PlcAddressToInt(true, plc.InPut_Tray_In)),
                Decompression_Up = new ProtocolItem(baseAddr + 64, typeof(int), PlcAddressToInt(true, plc.InPut_Decompression_Up)),
                Decompression_Down = new ProtocolItem(baseAddr + 66, typeof(int), PlcAddressToInt(true, plc.InPut_Decompression_Down)),
                Block_Out = new ProtocolItem(baseAddr + 68, typeof(int), PlcAddressToInt(true, plc.InPut_Block_Out)),
                Block_In = new ProtocolItem(baseAddr + 70, typeof(int), PlcAddressToInt(true, plc.InPut_Block_In)),
                Slow_Cylinder_Mid = new ProtocolItem(baseAddr + 72, typeof(int), PlcAddressToInt(true, plc.InPut_Slow_Cylinder_Mid)),
                Cylinder_Block = new ProtocolItem(baseAddr + 74, typeof(int), PlcAddressToInt(true, plc.InPut_Cylinder_Block)),
                Sunx_Back = new ProtocolItem(baseAddr + 76, typeof(int), PlcAddressToInt(true, plc.InPut_Sunx_Back)),
                SupportCover = new ProtocolItem(baseAddr + 78, typeof(int), PlcAddressToInt(true, plc.InPut_SupportCover)),
                Slow_Cylinder_Mid_2 = new ProtocolItem(baseAddr + 80, typeof(int), PlcAddressToInt(true, plc.InPut_Slow_Cylinder_Mid_2)),
                Slow_Cylinder_Mid_3 = new ProtocolItem(baseAddr + 82, typeof(int), PlcAddressToInt(true, plc.InPut_Slow_Cylinder_Mid_3)),
                RotorCylinderStop = new ProtocolItem(baseAddr + 84, typeof(int), PlcAddressToInt(true, plc.InPut_RotorCylinderStop)),
                Backup_1 = new ProtocolItem(baseAddr + 86, typeof(int), -1),
                Barometer = new ProtocolItem(baseAddr + 88, typeof(int), -1),
                Backup_2 = new ProtocolItem(baseAddr + 90, typeof(int), -1),
                Backup_3 = new ProtocolItem(baseAddr + 92, typeof(int), -1),
                Backup_4 = new ProtocolItem(baseAddr + 94, typeof(int), -1),
                Brew_Cylinder_Up = new ProtocolItem(baseAddr + 96, typeof(int), PlcAddressToInt(true, plc.InPut_Brew_Cylinder_Up)),
                Brew_Cylinder_Down = new ProtocolItem(baseAddr + 98, typeof(int), PlcAddressToInt(true, plc.InPut_Brew_Cylinder_Down)),
                Brew_SwingArmOut = new ProtocolItem(baseAddr + 100, typeof(int), PlcAddressToInt(true, plc.InPut_Brew_SwingArmOut)),
                Brew_SwingArmIn = new ProtocolItem(baseAddr + 102, typeof(int), PlcAddressToInt(true, plc.InPut_Brew_SwingArmIn)),
                Brew_BWHC_Out = new ProtocolItem(baseAddr + 104, typeof(int), PlcAddressToInt(true, plc.InPut_Brew_BWHC_Out)),
                Brew_BWHC_In = new ProtocolItem(baseAddr + 106, typeof(int), PlcAddressToInt(true, plc.InPut_Brew_BWHC_In)),
                Brew_BWHC_Up = new ProtocolItem(baseAddr + 108, typeof(int), PlcAddressToInt(true, plc.InPut_Brew_BWHC_Up)),
                Brew_BWHC_Down = new ProtocolItem(baseAddr + 110, typeof(int), PlcAddressToInt(true, plc.InPut_Brew_BWHC_Down)),
                Backup_5 = new ProtocolItem(baseAddr + 112, typeof(int), -1),
                HotWaterTankFloatBall = new ProtocolItem(baseAddr + 114, typeof(int), PlcAddressToInt(true, plc.InPut_HotWaterTankFloatBall)),
                Backup_6 = new ProtocolItem(baseAddr + 116, typeof(int), -1),
            };
        }

        /// <summary>
        /// PLC地址字符串转int值，支持X区、Y区、D区等格式。
        /// 用于将PLC地址字符串（如X10、D100.5）转换为实际寄存器地址。
        /// </summary>
        /// <param name="InPut">True:输入区；false:输出区</param>
        /// <param name="addr">PLC地址字符串</param>
        /// <returns>转换后的int类型地址，失败返回-1</returns>
        public static int PlcAddressToInt(bool InPut, string addr)
        {
            if (string.IsNullOrWhiteSpace(addr))
                return -1;

            addr = addr.Trim().ToUpper();

            if (InPut)
            {
                // X区
                if (addr.StartsWith("X"))
                {
                    if (int.TryParse(addr.Substring(1), out int x))
                        return (x / 10) * 100 + (x % 10);
                }
                // D区
                if (addr.StartsWith("D"))
                {
                    if (double.TryParse(addr.Substring(1), out double dAddr))
                        return 10000 + Convert.ToInt32(dAddr * 100);
                }
            }
            else
            {

                // Y区
                if (addr.StartsWith("Y"))
                {
                    if (int.TryParse(addr.Substring(1), out int y))
                        return 20000 + (y / 10) * 100 + (y % 10);
                }
                // D区，带小数点
                if (addr.StartsWith("D"))
                {
                    if (double.TryParse(addr.Substring(1), out double dAddr))
                        return 30000 + Convert.ToInt32(dAddr * 100);
                }
            }
            // 其它情况
            return -1;
        }

        /// <summary>
        /// 创建并初始化 PLC_OutPut 对象，所有协议项地址依次递增
        /// </summary>
        /// <param name="baseAddr">起始寄存器地址（如3900）</param>
        /// <returns>初始化后的 PLC_OutPut 对象</returns>
        public static PLC_OutPut CreateOutPut(int baseAddr)
        {
            var plc = My_ConPar.Object.CurrentMachine as My_ConPar.Type.PLC.IO;

            return new PLC_OutPut
            {
                X_Pulse = new ProtocolItem(baseAddr + 0, typeof(int), -1),
                X_Direction = new ProtocolItem(baseAddr + 2, typeof(int), -1),
                Y_Pulse = new ProtocolItem(baseAddr + 4, typeof(int), -1),
                Y_Direction = new ProtocolItem(baseAddr + 6, typeof(int), -1),
                Z_Pulse = new ProtocolItem(baseAddr + 8, typeof(int), -1),
                Z_Direction = new ProtocolItem(baseAddr + 10, typeof(int), -1),
                X_Power = new ProtocolItem(baseAddr + 12, typeof(int), -1),
                Y_Power = new ProtocolItem(baseAddr + 14, typeof(int), -1),
                Z_Power = new ProtocolItem(baseAddr + 16, typeof(int), -1),
                X_Reset = new ProtocolItem(baseAddr + 18, typeof(int), -1),
                Y_Reset = new ProtocolItem(baseAddr + 20, typeof(int), -1),
                Z_Reset = new ProtocolItem(baseAddr + 22, typeof(int), -1),
                Blender = new ProtocolItem(baseAddr + 24, typeof(int), PlcAddressToInt(false, plc.OutPut_Blender)),
                Buzzer = new ProtocolItem(baseAddr + 26, typeof(int), PlcAddressToInt(false, plc.OutPut_Buzzer)),
                TongsOn = new ProtocolItem(baseAddr + 28, typeof(int), PlcAddressToInt(false, plc.OutPut_TongsOn)),
                TongsOff = new ProtocolItem(baseAddr + 30, typeof(int), PlcAddressToInt(false, plc.OutPut_TongsOff)),
                Cylinder_Up = new ProtocolItem(baseAddr + 32, typeof(int), PlcAddressToInt(false, plc.OutPut_Cylinder_Up)),
                Cylinder_Down = new ProtocolItem(baseAddr + 34, typeof(int), PlcAddressToInt(false, plc.OutPut_Cylinder_Down)),
                Tray = new ProtocolItem(baseAddr + 36, typeof(int), PlcAddressToInt(false, plc.OutPut_Tray)),
                Waste = new ProtocolItem(baseAddr + 38, typeof(int), PlcAddressToInt(false, plc.OutPut_Waste)),
                Water = new ProtocolItem(baseAddr + 40, typeof(int), PlcAddressToInt(false, plc.OutPut_Water)),
                Decompression = new ProtocolItem(baseAddr + 42, typeof(int), PlcAddressToInt(false, plc.OutPut_Decompression)),
                RedLight = new ProtocolItem(baseAddr + 44, typeof(int), -1),
                GreenLight = new ProtocolItem(baseAddr + 46, typeof(int), -1),
                Block_Out = new ProtocolItem(baseAddr + 48, typeof(int), PlcAddressToInt(false, plc.OutPut_Block_Out)),
                Block_In = new ProtocolItem(baseAddr + 50, typeof(int), PlcAddressToInt(false, plc.OutPut_Block_In)),
                Slow_Cylinder = new ProtocolItem(baseAddr + 52, typeof(int), PlcAddressToInt(false, plc.OutPut_Slow_Cylinder)),
                Wash_In = new ProtocolItem(baseAddr + 54, typeof(int), PlcAddressToInt(false, plc.OutPut_Wash_In)),
                Wash_Out = new ProtocolItem(baseAddr + 56, typeof(int), PlcAddressToInt(false, plc.OutPut_Wash_Out)),
                Wash_Blow = new ProtocolItem(baseAddr + 58, typeof(int), PlcAddressToInt(false, plc.OutPut_Wash_Blow)),
                Backup_1 = new ProtocolItem(baseAddr + 60, typeof(int), -1),
                Solvent = new ProtocolItem(baseAddr + 62, typeof(int), PlcAddressToInt(false, plc.OutPut_Solvent)),
                Backup_2 = new ProtocolItem(baseAddr + 64, typeof(int), -1),
                Backup_3 = new ProtocolItem(baseAddr + 66, typeof(int), -1),
                Brew_Cylinder_Up = new ProtocolItem(baseAddr + 68, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_BWHC_Up)),
                Brew_Cylinder_Down = new ProtocolItem(baseAddr + 70, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_Cylinder_Down)),
                Brew_HotWater = new ProtocolItem(baseAddr + 72, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_HotWater)),
                Brew_BigColdWater = new ProtocolItem(baseAddr + 74, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_BigColdWater)),
                Brew_SmallColdWater = new ProtocolItem(baseAddr + 76, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_SmallColdWater)),
                Brew_HotWaterPump = new ProtocolItem(baseAddr + 78, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_HotWaterPump)),
                Brew_SwingArmOut = new ProtocolItem(baseAddr + 80, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_SwingArmOut)),
                Brew_SwingArmIn = new ProtocolItem(baseAddr + 82, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_SwingArmIn)),
                Brew_BWHC_Out = new ProtocolItem(baseAddr + 84, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_BWHC_Out)),
                Brew_BWHC_In = new ProtocolItem(baseAddr + 86, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_BWHC_In)),
                Backup_4 = new ProtocolItem(baseAddr + 88, typeof(int), -1),
                HotWaterTankInletValve = new ProtocolItem(baseAddr + 90, typeof(int), PlcAddressToInt(false, plc.OutPut_HotWaterTankInletValve)),
                Brew_BWHC_Up = new ProtocolItem(baseAddr + 92, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_BWHC_Up)),
                Brew_BWHC_Down = new ProtocolItem(baseAddr + 94, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_BWHC_Down)),
                Brew_Wash_Blow = new ProtocolItem(baseAddr + 96, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_Wash_Blow)),
                Brew_WashColdWater = new ProtocolItem(baseAddr + 98, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_WashColdWater)),
                Brew_HotWind = new ProtocolItem(baseAddr + 100, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_HotWind)),
                Brew_WaterHeaterHeating = new ProtocolItem(baseAddr + 102, typeof(int), PlcAddressToInt(false, plc.OutPut_Brew_WaterHeaterHeating)),
            };
        }

        /// <summary>
        /// 创建并初始化 PLC_ConPar 对象，所有协议项地址依次递增
        /// </summary>
        /// <param name="baseAddr">起始寄存器地址（如1000）</param>
        /// <returns>初始化后的 PLC_ConPar 对象</returns>
        public static PLC_ConPar CreateConPar(int baseAddr)
        {
            var balance = My_ConPar.Object.CurrentBalance;
            return new PLC_ConPar
            {
                // 1. 直接映射的配置项
                BlenderType = new ProtocolItem(baseAddr + 0, typeof(int), SmartColor.My_ConPar.Hardware.BlenderType),
                UseAbs = new ProtocolItem(baseAddr + 2, typeof(int), SmartColor.My_ConPar.Machine.UseAbs),
                ValveTerminal = new ProtocolItem(baseAddr + 4, typeof(int), SmartColor.My_ConPar.Hardware.ValveTerminal),
                AutoBrew = new ProtocolItem(baseAddr + 6, typeof(int), SmartColor.My_ConPar.Machine.CuttingMachine == 5 ? 1 : 0),
                BalanceType = new ProtocolItem(baseAddr + 8, typeof(int), balance?.BalanceType ?? -1),
                Decompression = new ProtocolItem(baseAddr + 10, typeof(int), SmartColor.My_ConPar.Hardware.Decompression),
                BlockType = new ProtocolItem(baseAddr + 12, typeof(int), SmartColor.My_ConPar.Hardware.Block),
                MidCylinder = new ProtocolItem(baseAddr + 14, typeof(int), SmartColor.My_ConPar.Hardware.MidCylinder),
                Sunx_Back = new ProtocolItem(baseAddr + 16, typeof(int), SmartColor.My_ConPar.Hardware.Sunx_Back),

                // 2. 延时参数（秒转毫秒）
                Delay_Cylinder = new ProtocolItem(baseAddr + 18, typeof(int), (int)(SmartColor.My_ConPar.Delay.Cylinder * 1000)),
                Delay_Tongs = new ProtocolItem(baseAddr + 20, typeof(int), (int)(SmartColor.My_ConPar.Delay.Tongs * 1000)),
                Delay_Syringe = new ProtocolItem(baseAddr + 22, typeof(int), (int)(SmartColor.My_ConPar.Delay.Syringe * 1000)),
                Delay_Tray = new ProtocolItem(baseAddr + 24, typeof(int), (int)(SmartColor.My_ConPar.Delay.Tray * 1000)),
                Balance_Reset = new ProtocolItem(baseAddr + 26, typeof(int), (int)(SmartColor.My_ConPar.Delay.Balance_Reset * 1000)),
                Balance_Read = new ProtocolItem(baseAddr + 28, typeof(int), (int)(SmartColor.My_ConPar.Delay.Balance_Read * 1000)),
                Buzzer_Finish = new ProtocolItem(baseAddr + 30, typeof(int), (int)(SmartColor.My_ConPar.Delay.Buzzer_Finish * 1000)),
                Delay_Decompression = new ProtocolItem(baseAddr + 32, typeof(int), (int)(SmartColor.My_ConPar.Delay.Decompression * 1000)),
                Delay_Block = new ProtocolItem(baseAddr + 34, typeof(int), (int)(SmartColor.My_ConPar.Delay.Block * 1000)),

                // 3. 其它参数
                Sunx_Stop_Y = new ProtocolItem(baseAddr + 36, typeof(int), SmartColor.My_ConPar.Other.Sunx_Stop_Y),
                Sunx_Back_Y = new ProtocolItem(baseAddr + 38, typeof(int), SmartColor.My_ConPar.Other.Sunx_Back_Y),
                S_MaxPulse = new ProtocolItem(baseAddr + 40, typeof(int), SmartColor.My_ConPar.Other.S_MaxPulse),
                B_MaxPulse = new ProtocolItem(baseAddr + 42, typeof(int), SmartColor.My_ConPar.Other.B_MaxPulse),
                BalanceMaxWeight = new ProtocolItem(baseAddr + 44, typeof(int), (int)SmartColor.My_ConPar.Other.BalanceMaxWeight),
                BrewBottleWeightMin = new ProtocolItem(baseAddr + 46, typeof(int), SmartColor.My_ConPar.Other.BrewBottleWeightMin),
                BrewBottleWeightMax = new ProtocolItem(baseAddr + 48, typeof(int), SmartColor.My_ConPar.Other.BrewBottleWeightMax),
                CylinderHeightDeviationPulse = new ProtocolItem(baseAddr + 50, typeof(int), SmartColor.My_ConPar.Other.CylinderHeightDeviationPulse),
                PositionInCylinder = new ProtocolItem(baseAddr + 52, typeof(int), SmartColor.My_ConPar.Other.PositionInCylinder),
                PositionInCylinderSlow1 = new ProtocolItem(baseAddr + 54, typeof(int), SmartColor.My_ConPar.Other.PositionInCylinderSlow1),
                PositionInCylinderSlow2 = new ProtocolItem(baseAddr + 56, typeof(int), SmartColor.My_ConPar.Other.PositionInCylinderSlow2),
                PositionInCylinderSlow3 = new ProtocolItem(baseAddr + 58, typeof(int), SmartColor.My_ConPar.Other.PositionInCylinderSlow3),
                CylinderStopPosition = new ProtocolItem(baseAddr + 60, typeof(int), SmartColor.My_ConPar.Other.CylinderStopPosition),
                CylinderDownPosition = new ProtocolItem(baseAddr + 62, typeof(int), SmartColor.My_ConPar.Other.CylinderDownPosition),
                CylinderPositioningRange = new ProtocolItem(baseAddr + 64, typeof(int), SmartColor.My_ConPar.Other.CylinderPositioningRange),
                UseCylinderPositioningEncoder = new ProtocolItem(baseAddr + 66, typeof(int), SmartColor.My_ConPar.Hardware.UseCylinderPositioningEncoder),

            };
        }

        /// <summary>
        /// 创建并初始化 PLC_Receive 对象，所有协议项地址依次递增
        /// </summary>
        /// <param name="baseAddr">起始寄存器地址（如900）</param>
        /// <returns>初始化后的 PLC_Receive 对象</returns>
        public static PLC_Receive CreateReceive(int baseAddr)
        {
            return new PLC_Receive
            {
                RobotActionCode = new ProtocolItem(baseAddr + 0, typeof(ushort)), // 900
                BalanceData = new ProtocolItem(baseAddr + 1, typeof(int)), // 901
                LightCurtainSignal = new ProtocolItem(baseAddr + 3, typeof(ushort)), // 903
                ExecuteFinished = new ProtocolItem(baseAddr + 4, typeof(ushort)), // 904
                InputX0_X17 = new ProtocolItem(baseAddr + 5, typeof(int)), // 905
                OutputY0_Y17 = new ProtocolItem(baseAddr + 7, typeof(int)), // 907
                X_CurrentPosition = new ProtocolItem(baseAddr + 9, typeof(int)), // 909
                Y_CurrentPosition = new ProtocolItem(baseAddr + 11, typeof(int)), // 911
                Z_CurrentPosition = new ProtocolItem(baseAddr + 13, typeof(int)), // 913
                X_CurrentSpeed = new ProtocolItem(baseAddr + 15, typeof(int)), // 915
                Y_CurrentSpeed = new ProtocolItem(baseAddr + 17, typeof(int)), // 917
                Z_CurrentSpeed = new ProtocolItem(baseAddr + 19, typeof(int)), // 919
                X_AlarmCode = new ProtocolItem(baseAddr + 21, typeof(int)), // 921
                Y_AlarmCode = new ProtocolItem(baseAddr + 22, typeof(int)), // 922
                InputX20_X57 = new ProtocolItem(baseAddr + 23, typeof(int)), // 923
                OutputY20_Y57 = new ProtocolItem(baseAddr + 25, typeof(int)), // 925
                PLCVersionYear = new ProtocolItem(baseAddr + 27, typeof(ushort)), // 927
                PLCVersionMonthDay = new ProtocolItem(baseAddr + 28, typeof(ushort)), // 928
                AAgentStepCurrentSpeed = new ProtocolItem(baseAddr + 29, typeof(int)), // 929
                BAgentStepCurrentSpeed = new ProtocolItem(baseAddr + 31, typeof(int)), // 931
                FlowMeterPulse = new ProtocolItem(baseAddr + 33, typeof(int)), // 933
                CylinderCheckSpeed = new ProtocolItem(baseAddr + 35, typeof(int)), // 935
                TurntableCurrentPosition = new ProtocolItem(baseAddr + 37, typeof(int)), // 937
                TurntableSpeed = new ProtocolItem(baseAddr + 39, typeof(int)), // 939
                TurntableAlarmCode = new ProtocolItem(baseAddr + 41, typeof(ushort)), // 941
                PowderWeight = new ProtocolItem(baseAddr + 42, typeof(int)), // 942
                TurntableActionResult = new ProtocolItem(baseAddr + 44, typeof(ushort)), // 944
                BottleWasherActionResult = new ProtocolItem(baseAddr + 45, typeof(ushort)), // 945
                UseTime = new ProtocolItem(baseAddr + 46, typeof(ushort)), // 946
                CylinderEncoderPosition = new ProtocolItem(baseAddr + 47, typeof(int)), // 947-948
            };
        }

        /// <summary>
        /// 接受区域PLC数据对象
        /// </summary>
        private PLC_Receive _Receive = CreateReceive(900);

        /// <summary>
        /// 设置当前读取模式
        /// </summary>
        public void SetReadMode(PlcReadMode mode)
        {
            _currentReadMode = mode;
            _writeEvent.Set();
        }

        /// <summary>
        /// 构造函数，初始化PLC通讯并记录日志
        /// </summary>
        public PLC()
        {
            Logger.Info("PLC初始化开始");
            try
            {
                var plc = My_ConPar.Object.CurrentMachine as My_ConPar.Type.PLC.IO;
                _plcTcp = new My_Interaction.ModbusTCP(plc.IP, plc.Port, "PLC");
                _plcTcp.Connect();
                Logger.Info("PLC连接成功");
            }
            catch (Exception ex)
            {
                Logger.Error("PLC连接失败", ex);
                Dispose();
                throw;
            }


        }

        /// <summary>
        /// 手动操作入队，将指定手动操作枚举写入PLC
        /// </summary>
        /// <param name="operation">手动操作枚举</param>
        public void EnqueueManualOperation(ManualOperation operation)
        {
            EnqueueWrite(() =>
            {
                ushort value = (ushort)operation;
                _plcTcp.WriteSingleRegister(1, _manualOperationAddr, value);
                Logger.Info($"手动操作入队: {operation}({value}) 已写入PLC地址 {_manualOperationAddr}");
            });
        }

        /// <summary>
        /// 询问操作，将指定询问枚举写入PLC
        /// </summary>
        /// <param name="operation">询问枚举</param>
        public void EnqueueInquire(Inquire operation)
        {
            EnqueueWrite(() =>
            {
                ushort value = (ushort)operation;
                _plcTcp.WriteSingleRegister(1, _inquireAddr, value);
                Logger.Info($"询问操作: {operation}({value}) 已写入PLC地址 {_inquireAddr}");
            });
        }

        /// <summary>
        /// 半自动操作入队，将SemiAutoParamBase对象中的所有ProtocolItem批量写入PLC
        /// </summary>
        /// <param name="param">SemiAutoParamBase对象</param>
        private void EnqueueSemiAutomaticOperation(PLC_SemiAutoParamBase param)
        {

            try
            {
                // 收集所有ProtocolItem
                var props = param.GetType().GetProperties();
                var items = new List<ProtocolItem>();
                foreach (var p in props)
                {
                    var item = p.GetValue(param) as ProtocolItem;
                    if (item != null && item.Value != null)
                        items.Add(item);
                }
                // 按地址排序
                items = items.OrderBy(i => i.Address).ToList();
                if (items.Count == 0) return;
                int startAddr = items[0].Address;
                int endAddr = items.Last().Address;
                int count = 0;
                // 计算总寄存器数（考虑int类型占2个寄存器）
                for (int addr = startAddr; addr <= endAddr;)
                {
                    var item = items.FirstOrDefault(i => i.Address == addr);
                    if (item != null)
                    {
                        if (item.DataType == typeof(int))
                        {
                            count += 2;
                            addr += 2;
                        }
                        else
                        {
                            count += 1;
                            addr += 1;
                        }
                    }
                    else
                    {
                        count += 1;
                        addr += 1;
                    }
                }
                ushort[] regsToWrite = new ushort[count];
                int regIndex = 0;
                for (int addr = startAddr; addr <= endAddr;)
                {
                    var item = items.FirstOrDefault(i => i.Address == addr);
                    if (item != null)
                    {
                        if (item.DataType == typeof(int))
                        {
                            int val = Convert.ToInt32(item.Value);
                            regsToWrite[regIndex++] = (ushort)(val & 0xFFFF);
                            regsToWrite[regIndex++] = (ushort)((val >> 16) & 0xFFFF);
                            addr += 2;
                        }
                        else if (item.DataType == typeof(bool))
                        {
                            regsToWrite[regIndex++] = (ushort)((bool)item.Value ? 1 : 0);
                            addr += 1;
                        }
                        else
                        {
                            regsToWrite[regIndex++] = Convert.ToUInt16(item.Value);
                            addr += 1;
                        }
                    }
                    else
                    {
                        // 补0
                        regsToWrite[regIndex++] = 0;
                        addr += 1;
                    }
                }

                int maxRetry = 3; // 最大重试次数
                int retryCount = 0;
                bool writeSuccess = false;

                while (retryCount < maxRetry && !writeSuccess)
                {
                    retryCount++;
                    try
                    {
                        // 1. 写入数据到PLC
                        _plcTcp.WriteMultipleRegisters(1, (ushort)startAddr, regsToWrite);
                        Logger.Info($"半自动操作参数批量写入D{startAddr}~D{startAddr + count - 1}，第{retryCount}次写入完成");

                        // 2. 短暂延迟，等待PLC处理
                        Thread.Sleep(20);

                        // 3. 从PLC回读相同地址区间的数据
                        ushort[] regsReadBack = _plcTcp.ReadHoldingRegisters(1, (ushort)startAddr, (ushort)count);

                        // 4. 比较写入和读取的数据
                        writeSuccess = true;
                        if (Convert.ToInt32(_Receive.RobotActionCode.Value) != 2)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                if (regsReadBack[i] != regsToWrite[i])
                                {
                                    writeSuccess = false;
                                    Logger.Error($"第{retryCount}次校验失败：地址{startAddr + i}，写入值={regsToWrite[i]}, 读取值={regsReadBack[i]}");
                                    break;
                                }
                            }
                        }

                        if (writeSuccess)
                        {
                            Logger.Info($"第{retryCount}次写入校验成功，确认数据已写入PLC");
                        }
                        else
                        {
                            if (retryCount < maxRetry)
                            {
                                // Logger.Error($"第{retryCount}次校验失败，开始第{retryCount + 1}次重试写入...");
                            }
                            else
                            {
                                Logger.Error($"半自动操作参数写入PLC失败，经过{maxRetry}次重试后校验仍未通过");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"第{retryCount}次写入或读取过程中发生异常", ex);
                        if (retryCount >= maxRetry)
                        {
                            throw; // 重试次数用尽后抛出异常
                        }
                    }
                }
                // --- 新增校验逻辑结束 ---
            }
            catch (Exception ex)
            {
                Logger.Error("WriteSemiAutomaticOperationSync", ex);
                throw;
            }
        }

        /// <summary>
        /// 异步执行半自动动作：
        /// 1. 先将动作参数批量写入PLC
        /// 2. 轮询等待RobotActionCode不为运行中（1）即认为动作结束（2为完成，其他为错误码）
        /// 3. 检测到结束后，自动将RobotActionCode清零
        /// 4. 返回实际结果码，由调用方自行判断成功或错误
        /// 说明：本方法为异步，不会阻塞UI线程，适合界面调用
        /// </summary>
        /// <param name="param">半自动动作参数（包含所有协议项）</param>
        /// <param name="runningCode">运行中标志值（通常为1）</param>
        /// <returns>Task，完成时返回PLC的实际结果码（2为成功，其他为各自错误码）</returns>
        public Task<int> ExecuteSemiAutomaticOperationAsync(PLC_SemiAutoParamBase param, int runningCode = 1)
        {
            return Task.Run(async () =>
            {
                var op = (SemiAutomaticOperation)Convert.ToInt32(param.OperationType.Value);
                var opLock = GetOperationLock(op);

                int resultCode = -1;
                bool isLoopSpeakActive = false;

                // 只锁定硬件相关的串行部分

                lock (opLock)
                {
                    try
                    {
                        // 新增：检查PLC是否处于运行中
                        bool show = false;
                        while (true)
                        {
                            Thread.Sleep(10);
                            if (Convert.ToInt32(_Receive.RobotActionCode.Value) == 1 && !show)
                            {
                                show = true;
                                Logger.Error("PLC正在执行动作，禁止重复下发半自动操作参数。");

                            }
                            else
                            {
                                break; // PLC不在运行中，继续执行写入
                            }

                        }


                        if (Convert.ToInt32(_Receive.RobotActionCode.Value) != 0)
                        {
                            _plcTcp.WriteSingleRegister(1, (ushort)_Receive.RobotActionCode.Address, 0);
                            while (true)
                            {
                                Thread.Sleep(10);
                                int code = Convert.ToInt32(_Receive.RobotActionCode.Value);
                                if (code == 0)
                                    break;
                            }
                        }
                        // 参数批量写入PLC（入队，同步执行）
                        EnqueueSemiAutomaticOperation(param);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("等待半自动动作完成异常", ex);
                    }
                }


                // 日志与气泡提示（不阻塞主流程）
                string s = GetSemiAutomaticOperationName(op);
                Logger.Info($"半自动动作发送，名称:{s}");
                ShowState?.Invoke(s);
                int lastAlarmCode = -1;
                bool isAlarmDialogActive = false;
                bool needSleepAfterInquire = false;
                while (true)
                {
                    await Task.Delay(5); // 减少CPU占用

                    int code = Convert.ToInt32(_Receive.RobotActionCode.Value);

                    // --- 光幕遮挡语音播报逻辑 ---
                    if (code >= 34 && code <= 37)
                    {
                        if (!isLoopSpeakActive)
                        {
                            var alarm = RobotAlarmOverview.GetAlarmInfo(code);
                            // 只在首次进入时触发
                            _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                            isLoopSpeakActive = true;
                        }
                    }
                    else
                    {
                        if (isLoopSpeakActive)
                        {
                            _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                            isLoopSpeakActive = false;
                        }
                    }
                    // --- 结束新增 ---

                    if (code != runningCode && code != 0)
                    {
                        resultCode = code;
                        if (code >= 10000)
                        {
                            var alarm = RobotAlarmOverview.GetAlarmInfo(resultCode);
                            var tcs = new TaskCompletionSource<bool>();
                            if (code == 12401 || code == 12701)
                            {
                                _ = Task.Run(() =>
                                {
                                    MessageEventManager.Instance.RequestShowMessage(
                                        "温馨提示",
                                        alarm.Description + ",需要人工确认：请先点击确认键，10秒后将自动开启抓手，请准备手动接住针筒/杯盖",
                                        btn =>
                                        {
                                            My_ConPar.Object.CurrentPLC.EnqueueInquire(btn == "确认" ? PLC.Inquire.Continue : PLC.Inquire.Continue);
                                            needSleepAfterInquire = true;
                                            tcs.SetResult(true);
                                        },
                                        new[] { "确认" }
                                       
                                    );
                                });
                                await tcs.Task;
                                if (needSleepAfterInquire)
                                {
                                    await Task.Delay(10000);
                                    needSleepAfterInquire = false;
                                }
                            }
                            else
                            {
                                if (!isAlarmDialogActive || code != lastAlarmCode)
                                {
                                    isAlarmDialogActive = true;
                                    lastAlarmCode = code;
                                    _ = Task.Run(() =>
                                        {
                                            MessageEventManager.Instance.RequestShowMessage(
                                                "温馨提示",
                                                alarm.Description + ",气压或传感器故障。请在完成修复后，点击“继续”以恢复正常运行。如需中止流程，请点击“退出”",
                                                btn =>
                                                {
                                                    My_ConPar.Object.CurrentPLC.EnqueueInquire(btn == "继续" ? PLC.Inquire.Continue : PLC.Inquire.Exit);

                                                    tcs.SetResult(true);
                                                },
                                                new[] { "继续", "退出" },
                                                "继续"
                                            );
                                        });
                                    await tcs.Task;
                                }
                                else
                                {
                                    // 已弹窗，等待用户操作
                                    await Task.Delay(100);
                                }
                                while (true)
                                {
                                    int code1 = Convert.ToInt32(_Receive.RobotActionCode.Value);
                                    if (code != code1)
                                    {
                                        isAlarmDialogActive = false; // 报警码变化后允许再次弹窗
                                        break;
                                    }
                                    Thread.Sleep(100);
                                }
                            }
                            continue; // 继续等待动作完成
                        }
                        else if (code >= 34 && code <= 37)
                        {
                            continue;
                        }
                        else
                        {
                            // 增加重试机制，最多重试10次
                            int retry = 0;
                            while (true)
                            {
                                bool writeSuccess = _plcTcp.WriteSingleRegister(1, (ushort)_Receive.RobotActionCode.Address, 0);
                                Logger.Info($"清零RobotActionCode，写入PLC，第{retry + 1}次，结果:{writeSuccess}");
                                await Task.Delay(10);
                                code = Convert.ToInt32(_Receive.RobotActionCode.Value);
                                if (code == 0)
                                    break;
                                retry++;
                                if (retry >= 10)
                                {
                                    Logger.Error("清零RobotActionCode多次失败，退出重试");
                                    break;
                                }
                            }
                            Logger.Info($"半自动动作结束，RobotActionCode={code}，已清零");
                            ShowState?.Invoke((code == 2 || code == 0) ? "待机" : $"{s}异常");
                        }
                        break;
                    }
                }

                // --- 结束时确保停止语音播报 ---
                if (isLoopSpeakActive)
                {
                    _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                    isLoopSpeakActive = false;
                }
                // --- 结束 ---

                return resultCode;
            });
        }

        /// <summary>
        /// 写操作入队，优先执行。用于业务或界面触发的写操作。
        /// </summary>
        /// <param name="writeAction">写操作委托</param>
        private void EnqueueWrite(Action writeAction)
        {
            if (writeAction == null) return;
            lock (_writeQueue)
            {
                _writeQueue.Enqueue(writeAction);
            }
            _writeEvent.Set();
        }

        /// <summary>
        /// 读操作入队。支持指定起始地址、长度和回调，便于UI或业务批量读取PLC数据。
        /// </summary>
        /// <param name="startAddr">起始寄存器地址</param>
        /// <param name="count">读取数量</param>
        /// <param name="callback">读取完成后的回调</param>
        public void EnqueueRead(ushort startAddr, ushort count, Action<ushort[]> callback)
        {
            lock (_readQueue)
            {
                _readQueue.Enqueue(new PLC_ReadTask(startAddr, count, callback));
            }
            _writeEvent.Set();
        }

        /// <summary>
        /// 启动调度线程，循环处理写/读队列和自动读任务。
        /// </summary>
        public void StartAutoRead()
        {
            _isRunning = true;
            _writeWorker = new Thread(WriteWorkerLoop) { IsBackground = true };

            _readWorker = new Thread(ReadWorkerLoop) { IsBackground = true };
            _writeWorker.Start();
            _readWorker.Start();
        }

        public async void CheckAndResetOnStartup()
        {

            //1. 发送999到OperationType，下发给PLC
            var resetParam = new PLC_ActionCheckParam();
            resetParam.OperationType.Value = 999;
            EnqueueSemiAutomaticOperation(resetParam);
            Logger.Info("已发送999到OperationType，等待PLC处理...");
            while (true)
            {
                Thread.Sleep(50);
                var code = Convert.ToInt32(_Receive.RobotActionCode.Value);
                if (code == 2)
                    break;
            }



            // 4. 调用动作检查
            Logger.Info("启动后调用动作检查");
            var result = await ExecuteSemiAutomaticOperationAsync(new PLC_ActionCheckParam());
            var sar = RobotAlarmOverview.HandleSemiAutoResult(result, "动作检查");
            switch (sar.Level)
            {
                case SemiAutoResultCode.Success:
                    break;
                default:
                    throw new Exception(sar.Message);

            }
        }

        // 写线程主循环
        private void WriteWorkerLoop()
        {
            while (_isRunning)
            {
                Action writeAction = null;
                lock (_writeQueue)
                {
                    if (_writeQueue.Count > 0)
                        writeAction = _writeQueue.Dequeue();
                }
                if (writeAction != null)
                {
                    try { writeAction(); }
                    catch (Exception ex) { Logger.Error("PLC写操作异常", ex); }
                }
                else
                {
                    _writeEvent.WaitOne(50);
                }
            }
        }

        public static bool _isFristStart = true;

        // 读线程主循环
        private void ReadWorkerLoop()
        {
            while (_isRunning)
            {
                if (_isFristStart)
                {

                    _isFristStart = false;
                    _ = Task.Run(() => { CheckAndResetOnStartup(); });
                }
                // 1. 队列读任务（UI/业务批量读，仍可合并区块）
                List<PLC_ReadTask> tasksToRead = new List<PLC_ReadTask>();
                lock (_readQueue)
                {
                    while (_readQueue.Count > 0)
                        tasksToRead.Add(_readQueue.Dequeue());
                }
                if (tasksToRead.Count > 0)
                {
                    // 合并区块逻辑只用于队列读任务
                    tasksToRead.Sort((a, b) => a.StartAddr.CompareTo(b.StartAddr));
                    List<(ushort start, ushort count, List<PLC_ReadTask> group)> merged = new List<(ushort, ushort, List<PLC_ReadTask>)>();
                    foreach (var task in tasksToRead)
                    {
                        if (merged.Count == 0)
                        {
                            merged.Add((task.StartAddr, task.Count, new List<PLC_ReadTask> { task }));
                        }
                        else
                        {
                            var last = merged[merged.Count - 1];
                            ushort lastEnd = (ushort)(last.start + last.count);
                            ushort taskEnd = (ushort)(task.StartAddr + task.Count);
                            if (task.StartAddr <= lastEnd)
                            {
                                ushort newEnd = taskEnd > lastEnd ? taskEnd : lastEnd;
                                merged[merged.Count - 1] = (last.start, (ushort)(newEnd - last.start), last.group);
                                last.group.Add(task);
                            }
                            else
                            {
                                merged.Add((task.StartAddr, task.Count, new List<PLC_ReadTask> { task }));
                            }
                        }
                    }
                    foreach (var m in merged)
                    {
                        try
                        {
                            var regs = _plcTcp.ReadHoldingRegisters(1, m.start, m.count);
                            foreach (var t in m.group)
                            {
                                int offset = t.StartAddr - m.start;
                                ushort[] sub = new ushort[t.Count];
                                Array.Copy(regs, offset, sub, 0, t.Count);
                                t.Callback?.Invoke(sub);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("PLC批量读操作异常", ex);
                        }
                    }
                }

                // 2. 自动循环读（根据模式分别处理）
                try
                {
                    if (_readFail)
                    {
                        Thread.Sleep(50);
                        continue;
                    }
                    if (_currentReadMode == PlcReadMode.Normal)
                    {
                        // 只读完成标志位和天平（900-902，3字）
                        ushort addr = (ushort)_Receive.RobotActionCode.Address;
                        ushort[] regs = _plcTcp.ReadHoldingRegisters(1, addr, 3);
                        _Receive.RobotActionCode.Value = regs[0];
                        _Receive.BalanceData.Value = (regs[2] << 16) | regs[1];
                    }
                    else if (_currentReadMode == PlcReadMode.Debug)
                    {
                        // 全部读取
                        GetReceiveBlock(_Receive, out int startAddr, out int count);
                        ushort[] regs = _plcTcp.ReadHoldingRegisters(1, (ushort)startAddr, (ushort)count);
                        var props = typeof(PLC_Receive).GetProperties();
                        foreach (var p in props)
                        {
                            var item = p.GetValue(_Receive) as ProtocolItem;
                            if (item != null)
                            {
                                int offset = item.Address - startAddr;
                                if (item.DataType == typeof(int))
                                {
                                    if (offset + 1 < regs.Length)
                                        item.Value = (regs[offset + 1] << 16) | regs[offset];
                                }
                                else if (item.DataType == typeof(ushort))
                                {
                                    if (offset < regs.Length)
                                        item.Value = regs[offset];
                                }
                            }
                        }
                    }

                    int roundDigits = My_Tool.BalanceStableReading.RetainDecimals();
                    My_Tool.BalanceStableReading.CurrentRead = Math.Round(((int)_Receive.BalanceData.Value) / 1000.00, roundDigits);



                }
                catch (Exception ex)
                {
                    if (!_readFail)
                    {
                        MessageEventManager.Instance.RequestShowMessage("PLC读取数据异常", ex.Message, btn =>
                        {
                            if (btn == "确定")
                                _readFail = false;
                        }, new[] { "确定" }, "确定");
                        _readFail = true;
                    }
                }

                // 等待信号或超时
                _readEvent.WaitOne(10);
            }
        }



        /// <summary>
        /// 获取PLC_Receive结构体的最小起始地址和总寄存器数（自动适配所有属性）
        /// </summary>
        /// <param name="receive">PLC_Receive对象</param>
        /// <param name="startAddr">输出：最小起始地址</param>
        /// <param name="count">输出：总寄存器数</param>
        private static void GetReceiveBlock(PLC_Receive receive, out int startAddr, out int count)
        {
            var props = typeof(PLC_Receive).GetProperties();
            int minAddr = int.MaxValue, maxAddr = int.MinValue;
            foreach (var p in props)
            {
                var item = p.GetValue(receive) as ProtocolItem;
                if (item != null)
                {
                    if (item.Address < minAddr) minAddr = item.Address;
                    int len = item.DataType == typeof(int) ? 2 : 1;
                    int itemEnd = item.Address + len - 1;
                    if (itemEnd > maxAddr) maxAddr = itemEnd;
                }
            }
            startAddr = minAddr;
            count = maxAddr - minAddr + 1;
        }

        /// <summary>
        /// 断开PLC连接并释放资源
        /// </summary>
        // 释放资源
        public void Dispose()
        {
            _isRunning = false;
            _writeEvent.Set();
            _readEvent.Set();
            if (_writeWorker != null && _writeWorker.IsAlive)
            {
                _writeWorker.Join(500);
            }
            if (_readWorker != null && _readWorker.IsAlive)
            {
                _readWorker.Join(500);
            }
            try
            {
                if (_plcTcp != null)
                {
                    _plcTcp.Disconnect();
                    _plcTcp.Dispose();
                    Logger.Info("PLC连接已断开，资源已释放");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("PLC断开或释放资源失败", ex);
            }
        }

        /// <summary>
        /// 将输入区（PLC_InPut）数据批量写入PLC
        /// </summary>
        public void WriteInputToPLC()
        {
            PLC_InPut input = CreateInPut(3500);
            if (_plcTcp == null) throw new InvalidOperationException("PLC未初始化");
            List<ushort> values = new List<ushort>();
            int startAddr = -1;
            foreach (var prop in typeof(PLC_InPut).GetProperties())
            {
                var item = prop.GetValue(input) as ProtocolItem;
                if (item != null)
                {
                    if (startAddr == -1) startAddr = item.Address;
                    if (item.Value == null)
                    {
                        // int类型补-1（0xFFFF, 0xFFFF），ushort类型补-1（0xFFFF）
                        if (item.DataType == typeof(int))
                        {
                            values.Add(0xFFFF);
                            values.Add(0xFFFF);
                        }
                        else
                        {
                            values.Add(0xFFFF);
                        }
                    }
                    else if (item.DataType == typeof(int))
                    {
                        int val = Convert.ToInt32(item.Value);
                        values.Add((ushort)(val & 0xFFFF));
                        values.Add((ushort)((val >> 16) & 0xFFFF));
                    }
                    else if (item.DataType == typeof(bool))
                    {
                        values.Add((ushort)((bool)item.Value ? 1 : 0));
                    }
                    else
                    {
                        values.Add(Convert.ToUInt16(item.Value));
                    }
                }
            }
            if (startAddr >= 0 && values.Count > 0)
            {
                _plcTcp.WriteMultipleRegisters(1, (ushort)startAddr, values.ToArray());
            }
        }

        /// <summary>
        /// 将输出区（PLC_OutPut）数据批量写入PLC
        /// </summary>
        public void WriteOutputToPLC()
        {
            PLC_OutPut output = CreateOutPut(3900);
            if (_plcTcp == null) throw new InvalidOperationException("PLC未初始化");
            List<ushort> values = new List<ushort>();
            int startAddr = -1;
            foreach (var prop in typeof(PLC_OutPut).GetProperties())
            {
                var item = prop.GetValue(output) as ProtocolItem;
                if (item != null)
                {
                    if (startAddr == -1) startAddr = item.Address;
                    if (item.Value == null)
                    {
                        if (item.DataType == typeof(int))
                        {
                            values.Add(0xFFFF);
                            values.Add(0xFFFF);
                        }
                        else
                        {
                            values.Add(0xFFFF);
                        }
                    }
                    else if (item.DataType == typeof(int))
                    {
                        int val = Convert.ToInt32(item.Value);
                        values.Add((ushort)(val & 0xFFFF));
                        values.Add((ushort)((val >> 16) & 0xFFFF));
                    }
                    else if (item.DataType == typeof(bool))
                    {
                        values.Add((ushort)((bool)item.Value ? 1 : 0));
                    }
                    else
                    {
                        values.Add(Convert.ToUInt16(item.Value));
                    }
                }
            }
            if (startAddr >= 0 && values.Count > 0)
            {
                _plcTcp.WriteMultipleRegisters(1, (ushort)startAddr, values.ToArray());
            }
        }

        /// <summary>
        /// 将配置区（PLC_ConPar）数据批量写入PLC
        /// </summary>
        public void WriteConParToPLC()
        {
            PLC_ConPar conPar = CreateConPar(1000);
            if (_plcTcp == null) throw new InvalidOperationException("PLC未初始化");
            List<ushort> values = new List<ushort>();
            int startAddr = -1;
            foreach (var prop in typeof(PLC_ConPar).GetProperties())
            {
                var item = prop.GetValue(conPar) as ProtocolItem;
                if (item != null)
                {
                    if (startAddr == -1) startAddr = item.Address;
                    if (item.Value == null)
                    {
                        // int类型补-1（0xFFFF, 0xFFFF），ushort类型补-1（0xFFFF）
                        if (item.DataType == typeof(int))
                        {
                            values.Add(0xFFFF);
                            values.Add(0xFFFF);
                        }
                        else
                        {
                            values.Add(0xFFFF);
                        }
                    }

                    else if (item.DataType == typeof(int))
                    {
                        int val = Convert.ToInt32(item.Value);
                        values.Add((ushort)(val & 0xFFFF));
                        values.Add((ushort)((val >> 16) & 0xFFFF));
                    }
                    else if (item.DataType == typeof(bool))
                    {
                        values.Add((ushort)((bool)item.Value ? 1 : 0));
                    }
                    else
                    {
                        values.Add(Convert.ToUInt16(item.Value));
                    }
                }
            }
            if (startAddr >= 0 && values.Count > 0)
            {
                _plcTcp.WriteMultipleRegisters(1, (ushort)startAddr, values.ToArray());
            }
        }

        /// <summary>
        /// 获取版本号字符串，格式为"YYYYMMDD"，
        /// </summary>
        /// <returns></returns>
        public string GetPLCVersion()
        {
            // 主动读取版本号寄存器
            ushort versionAddr = (ushort)_Receive.PLCVersionYear.Address;
            ushort[] versionRegs = _plcTcp.ReadHoldingRegisters(1, versionAddr, 2);
            _Receive.PLCVersionYear.Value = versionRegs[0];
            _Receive.PLCVersionMonthDay.Value = versionRegs[1];

            var yearObj = _Receive.PLCVersionYear?.Value;
            var monthDayObj = _Receive.PLCVersionMonthDay?.Value;

            if (yearObj == null || monthDayObj == null)
                return "未知";

            int year = Convert.ToInt32(yearObj);
            int monthDay = Convert.ToInt32(monthDayObj);
            string monthDayStr = monthDay.ToString("D4");
            string version = $"{year}{monthDayStr}";
            return version;
        }



    }
}