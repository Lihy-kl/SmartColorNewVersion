using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_ADT8940A1
{
    internal class ADT8940A1_ActionCheck
    {
        /// <summary>
        /// 动作检查
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public Task<int> ActionCheckAsync()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            return Task.Run(() =>
            {
                try
                {
                    Lib_Card.CardObject.OA1.SuddnStop(boaed.Axis_X);
                    Lib_Card.CardObject.OA1.SuddnStop(boaed.Axis_Y);
                    Lib_Card.CardObject.OA1.SuddnStop(boaed.Axis_Z);
                    int iRes = Lib_Card.CardObject.OA1Input.InPutStatus(Convert.ToInt32( boaed.InPut_Syringe));
                    if (SmartColor.My_ConPar.Choices.IgnoreSyringeSensor == 1)
                    {
                        iRes = 0;
                    }

                    if (1 == iRes)
                    {
                        //new FADM_Object.MyAlarm("请先拿住针筒点确定", "温馨提示",false,1);
                        //My_File.LocalTranslator.ShowMessage("请先拿住针筒点确定", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Question);

                        var tcs = new ManualResetEvent(false);
                        // 超过2次，提示检查密封圈
                        SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                            "温馨提示",
                            $"请先拿住针筒点确定",
                             btn =>
                             {
                                 
                                 tcs.Set();
                             },
                            new[] { "确定" },
                            "确定"
                        );
                        tcs.WaitOne(); // 等待用户操作
                        Lib_Card.ADT8940A1.OutPut.Tongs.Tongs tongs = new Lib_Card.ADT8940A1.OutPut.Tongs.Tongs_Basic();
                        tongs.Tongs_Off();


                    }

                    for (int i = 0; i < 16; i++)
                    {
                        if (SmartColor.My_ConPar.Hardware.BlenderType == 0)
                        {
                            if (i == Convert.ToInt32(boaed.OutPut_Blender))
                            {
                                continue;
                            }
                        }
                        Lib_Card.CardObject.OA1.WriteOutPut(i, 0);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "X轴正在运行")
                    {
                        return 11;
                    }
                    else if (ex.Message == "X轴伺服器报警")
                    {
                        return 18;
                    }
                    else if (ex.Message == "X轴矢能未接通")
                    {
                        return 19;
                    }
                    else if (ex.Message == "X轴正限位已通")
                    {
                        return 32;
                    }
                    else if (ex.Message == "Y轴正在运行")
                    {
                        return 111;
                    }
                    else if (ex.Message == "X轴准备信号未接通")
                    {
                        return 21;
                    }
                    else if (ex.Message == "Y轴准备信号未接通")
                    {
                        return 121;
                    }
                    else if (ex.Message == "Y轴伺服器报警")
                    {
                        return 101;
                    }
                    else if (ex.Message == "Y轴矢能未接通")
                    {
                        return 102;
                    }
                    else if (ex.Message == "Y轴正限位已通")
                    {
                        return 132;
                    }
                    else if (ex.Message == "Z轴反限位已通")
                    {
                        return 231;
                    }
                    else if (ex.Message.Contains("脉冲计算异常"))
                    {
                        return 242;
                    }
                    else if (ex.Message == "气缸上超时")
                    {
                        return 301;
                    }
                    else if (ex.Message == "气缸下信号已接通")
                    {
                        return 301;
                    }
                    else if (ex.Message == "气缸下超时")
                    {
                        return 322;
                    }
                    else if (ex.Message == "气缸上信号已接通")
                    {
                        return 321;
                    }
                    else if (ex.Message == "抓手A打开超时")
                    {
                        return 401;
                    }
                    else if (ex.Message == "抓手B打开超时")
                    {
                        return 402;
                    }
                    else if (ex.Message == "抓手A关闭超时")
                    {
                        return 411;
                    }
                    else if (ex.Message == "抓手B关闭超时")
                    {
                        return 412;
                    }
                    else if (ex.Message == "接液盘收回超时")
                    {
                        return 511;
                    }
                    else if (ex.Message == "接液盘出信号已接通")
                    {
                        return 512;
                    }
                    else if (ex.Message == "接液盘伸出超时")
                    {
                        return 501;
                    }
                    else if (ex.Message == "接液盘回信号已接通")
                    {
                        return 502;
                    }
                    else if (ex.Message == "泄压气缸上超时")
                    {
                        return 601;
                    }
                    else if (ex.Message == "泄压气缸下已通")
                    {
                        return 602;
                    }
                    else if (ex.Message == "阻挡气缸收回超时")
                    {
                        return 1112;
                    }
                    else if (ex.Message == "气缸阻挡限位已通")
                    {
                        return 1122;
                    }
                    else if (ex.Message == "气缸慢速中超时")
                    {
                        return 1123;
                    }
                    else if (ex.Message == "慢速中限位与气缸下已通")
                    {
                        return 1129;
                    }
                    else if (ex.Message == "气缸到阻挡超时")
                    {
                        return 1131;
                    }
                    else if (ex.Message == "未发现针筒")
                    {
                        return 2101;
                    }
                    else if (ex.Message == "发现针筒")
                    {
                        return 2401;
                    }
                    else
                    {
                        //其他错误
                        return -1;
                    }
                }

                return 2;
            });
        }
    }
}
