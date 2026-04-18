using SmartColor.My_SemiAutoModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_Tool
{
    public class AlarmInfo
    {
        public RobotAlarmOverview.AlarmType Type { get; }
        public string Description { get; }

        public AlarmInfo(RobotAlarmOverview.AlarmType type, string description)
        {
            Type = type;
            Description = description;
        }
    }

    public class RobotAlarmOverview
    {
        /// <summary>
        /// 设备报警字典，键为报警代码，值为报警描述
        /// </summary>
        public static readonly Dictionary<int, string> AlarmDictionary = new Dictionary<int, string>
        {
            // X轴报警
            { 11, "X轴正在运行" },
            { 12, "X轴使能关闭超时" },
            { 13, "X轴报警复位失败" },
            { 18, "X轴伺服器报警" },
            { 19, "X轴使能打开超时" },
            { 21, "X轴准备信号无接通" },
            { 22, "X轴正反限位已通" },
            { 31, "X轴反向限位已通" },
            { 32, "X轴正向限位已通" },
            { 34, "前光幕遮挡" },
            { 35, "左光幕遮挡" },
            { 36, "右光幕遮挡" },
            { 37, "后光幕遮挡" },

            // Y轴报警
            { 101, "Y轴伺服器报警" },
            { 102, "Y轴使能打开超时" },
            { 111, "Y轴正在运行" },
            { 112, "Y轴使能关闭超时" },
            { 113, "Y轴报警复位失败" },
            { 121, "Y轴准备信号无接通" },
            { 122, "Y轴正反转位已通" },
            { 131, "Y轴反向限位已通" },
            { 132, "Y轴正向限位已通" },

            // Z轴报警
            { 201, "Z轴伺服器报警" },
            { 202, "Z轴使能打开超时" },
            { 211, "Z轴正在运行" },
            { 212, "Z轴使能关闭超时" },
            { 213, "Z轴报警复位失败" },
            { 221, "Z轴准备信号无接通" },
            { 231, "Z轴反向限位已通" },
            { 232, "Z轴正向限位已通（无）" },
            { 241, "Z轴目标位置超行程" },
            { 242, "脉冲计算异常" },

            // 气缸报警
            { 301, "气缸上超时" },
            { 302, "气缸下已通" },
            { 311, "气缸单输出点不存在气缸中信号" },
            { 312, "气缸中超时" },
            { 321, "气缸上已通" },
            { 322, "气缸下超时" },

            // 抓手报警
            { 401, "抓手A打开超时" },
            { 402, "抓手B打开超时" },
            { 411, "抓手A关闭超时" },
            { 412, "抓手B关闭超时" },

            // 接液盘报警
            { 431, "撑盖气缸合到位信号已通" },
            { 432, "撑盖气缸开超时" },
            { 433, "撑盖气缸开到位信号已通" },
            { 434, "撑盖气缸合超时" },
            { 501, "接液盘伸出超时" },
            { 502, "接液盘回限位已通" },
            { 511, "接液盘收回超时" },
            { 512, "接液盘出限位已通" },

            // 泄压气缸报警
            { 601, "泄压上超时" },
            { 602, "泄压气缸下已通" },
            { 611, "泄压气缸下超时" },
            { 612, "泄压气缸上已通" },

            // 阻挡气缸报警
            { 1101, "阻挡回限位已通" },
            { 1102, "阻挡伸出超时" },
            { 1111, "阻挡出限位已通" },
            { 1112, "阻挡收回超时" },
            { 1113, "气缸在阻挡位置" },
            { 1118, "气缸到慢速中2超时" },
            { 1119, "气缸到慢速中3超时" },
            { 1120, "气缸阻挡位与气缸下同时接通" },
            { 1121, "气缸单输出点不存在气缸慢速中信号" },
            { 1122, "气缸阻挡限位已通" },
            { 1123, "气缸慢速中超时" },
            { 1124, "气缸阻挡和气缸下限位同时接通" },
            { 1125, "气缸上和气缸阻挡限位同时接通" },
            { 1126, "气缸上和气缸下限位同时接通" },
            { 1127, "气缸上与慢速中限位同时接通" },
            { 1128, "慢速中限位和气缸阻挡位同时接通" },
            { 1129, "慢速中限位与气缸下已通" },
            { 1131, "气缸到阻挡位超时" },

            // 需要确认的报警
            { 2101, "未发现针筒" },
            { 2203,"注液时气缸慢速中超时" },
            { 2204,"注液时气缸慢速中3超时" },
            { 2401, "发现针筒" },
            { 2701, "发现杯盖或针筒" },//动作检查专用
            { 2702, "未发现杯盖" },


            { 2705, "关盖失败" },
            { 2706, "放盖失败" },
            { 2707, "二次关盖复压失败" },
            { 2708, "二次关盖失败" },
            { 2709, "二次关盖未发现杯盖" },
            { 2716, "关盖时抓手打开失败" },
            { 2717, "关盖时撑盖开失败" },
            { 2718, "关盖撑盖时气缸下失败" },
            { 2713, "配液杯取盖抓手关失败" },
            { 2714, "放盖区取盖抓手关失败" },
            { 2715, "放盖后抓手打开失败" },

            // 天平报警
            { 3301, "天平通讯异常" },
            { 3302, "天平开机未拿走废液桶" },
            { 3303, "天平超下限" },
            { 3304, "天平超上限" },
            { 3305, "请先清空废液桶" },

            // 自动出入布报警
            { 4501, "未发现抓手" },
            { 4502, "取布夹时抓手关失败" },

            // 撑盖报警
            { 5401, "撑盖确认时撑盖开失败" },
            { 5402, "撑盖确认时气缸下失败" },
            { 5403, "无撑盖选项" },
            { 5404, "撑盖确认时抓手合失败" },
            { 5411, "放瓶时气缸下失败" },
            { 5412, "放瓶失败" },
            { 5421, "加粉超时" },

            { 8703, "配液杯开盖失败" },
            { 8704, "放盖区取盖失败" },
            // 询问类报警
            { 10301, "气缸上超时询问" },
            { 10302, "气缸下已通询问" },
            { 10312, "气缸中超时询问" },
            { 10321, "气缸上已通询问" },
            { 10322, "气缸下超时询问" },
            { 10401, "抓手A打开超时询问" },
            { 10402, "抓手B打开超时询问" },
            { 10411, "抓手A关闭超时询问" },
            { 10412, "抓手B关闭超时询问" },
            { 10431, "撑盖气缸合到位信号已通（询问）" },
            { 10432, "撑盖气缸开超时（询问）" },
            { 10433, "撑盖气缸开到位信号已通（询问）" },
            { 10434, "撑盖气缸合超时（询问）" },
            { 10501, "接液盘伸出超时询问" },
            { 10502, "接液盘回限位已通询问" },
            { 10511, "接液盘收回超时询问" },
            { 10512, "接液盘出限位已通询问" },
            { 10601, "泄压气缸上超时询问" },
            { 10602, "泄压气缸下已通询问" },
            { 10611, "泄压气缸下超时询问" },
            { 10612, "泄压气缸上已通询问" },
            { 11101, "阻挡伸出超时询问" },
            { 11102, "阻挡回限位已通询问" },
            { 11111, "阻挡出限位已通询问" },
            { 11112, "阻挡收回超时询问" },
            { 11118, "气缸到慢速中2超时询问" },
            { 11119, "气缸到慢速中3超时询问" },
            { 11120, "气缸阻挡位与气缸下同时接通询问" },
            { 11122, "气缸阻挡限位已通询问" },
            { 11123, "气缸慢速中超时询问" },
            { 11124, "气缸阻挡和气缸下限位同时接通询问" },
            { 11125, "气缸上和气缸阻挡限位同时接通询问" },
            { 11126, "气缸上和气缸下限位同时接通询问" },
            { 11127, "气缸上与慢速中限位同时接通询问" },
            { 11128, "慢速中限位和气缸阻挡位同时接通询问" },
            { 11129, "慢速中限位与气缸下已通询问" },
            { 11131, "气缸到阻挡位超时询问" },
            { 12401, "发现针筒询问" },
            { 12701, "发现杯盖或针筒询问" },//动作检查专用  特殊处理，不要给默认按钮
            { 12702, "未发现杯盖询问（取消）" },
            { 13301, "天平通讯异常询问" },
            { 13302, "天平开机未拿走废液桶询问" },
            { 13303, "天平超下限询问" },
            { 13304, "天平超上限询问" },
            { 13305, "请先清空废液桶询问" }
        };

        public enum AlarmType
        {
            /// <summary>
            ///  <2000，异常报警
            /// </summary>
            Exception,

            /// <summary>
            /// 2000~9999，PLC已退出功能块，需流程层人工确认
            /// </summary>
            PlcExit,

            /// <summary>
            /// >=10000，PLC功能块卡住，需写入PLC
            /// </summary>
            PlcQuery,

            /// <summary>
            /// 需要人工确认的报警,点完后直接进行下一步
            /// </summary>
            ManualConfirmation ,

            /// <summary>
            /// 机械手复位，点完后机械手回到待机位，重新开始流程
            /// </summary>
            MechanicalReset


        }

        public static AlarmInfo GetAlarmInfo(int alarmCode)
        {
            AlarmType type;
            if (alarmCode < 2000)
                type = AlarmType.Exception;
            else if (alarmCode >= 2101 && alarmCode <= 2709)
            {
                if (alarmCode == 2203 || alarmCode == 2204)
                    type = AlarmType.MechanicalReset;
                else
                    type = AlarmType.ManualConfirmation;
            }
               
            else if (alarmCode < 10000)
                type = AlarmType.PlcExit;

            else
                type = AlarmType.PlcQuery;

            string desc = AlarmDictionary.TryGetValue(alarmCode, out var d) ? d : "未知报警";
            if (alarmCode >= 34 && alarmCode <= 37)
            {
                if (My_ConPar.Hardware.Shield == 1)
                {
                    switch (alarmCode)
                    {
                        case 34:
                            desc = "防护罩前门已打开";
                            break;
                        case 35:
                            desc = "防护罩左门已打开";
                            break;
                        case 36:
                            desc = "防护罩右门已打开";
                            break;
                        case 37:
                            desc = "防护罩后门已打开";
                            break;
                    }
                }
            }
            return new AlarmInfo(type, desc);
        }

        /// <summary>
        /// 半自动结果处理方法，根据PLC返回的结果码判断是成功、需要人工确认还是异常，并返回相应的结果对象或抛出异常
        /// </summary>
        /// <param name="result">返回值</param>
        /// <param name="actionName">动作名称</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static SemiAutoResult HandleSemiAutoResult(
            int result,
            string actionName)
        {
            if (result == 2)
            {
                return new SemiAutoResult
                {
                    Level = SemiAutoResultCode.Success,
                    Message = $"{actionName}成功"
                };
            }
            else
            {
                var alarm = GetAlarmInfo(result);
                switch (alarm.Type)
                {
                    case AlarmType.Exception:
                        throw new Exception($"{actionName}异常:{alarm.Description}");

                    case AlarmType.ManualConfirmation:
                        return new SemiAutoResult
                        {
                            Level = SemiAutoResultCode.Exception,
                            Message = $"{actionName}需要人工确认:{alarm.Description},确认已完成，请点'确认'",
                            Exception = new Exception($"{actionName}人工确认:{alarm.Description}")
                        };
                    case AlarmType.PlcExit:
                        return new SemiAutoResult
                        {
                            Level = SemiAutoResultCode.NeedInteraction,
                            Message = actionName.Contains("配液杯撑盖异常") ? $"{actionName}询问:{alarm.Description},请检查，恢复后点击'重试'，确认盖子已正常关闭请点'已确认'" :
                            $"{actionName}询问:{alarm.Description},请检查，恢复后点击'重试'，终止流程请点'退出'"
                        };
                    case AlarmType.MechanicalReset:
                        return new SemiAutoResult
                        {
                            Level = SemiAutoResultCode.MechanicalReset,
                            Message = $"{actionName}询问:{alarm.Description},请检查，恢复后点击'重试'，终止流程请点'退出'"
                        };
                    default:
                        throw new Exception($"{actionName}未知异常:{result}");
                }
            }
        }
    }

}
