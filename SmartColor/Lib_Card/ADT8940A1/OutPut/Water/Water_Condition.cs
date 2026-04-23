using SmartColor.My_Tool;
using System;
using System.Threading;

namespace Lib_Card.ADT8940A1.OutPut.Water
{
    /// <summary>
    /// 带条件检查
    /// </summary>
    public class Water_Condition : Water
    {
        public override int Water_Off()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Water), 0))
                return -1;
            return 0;
        }

        public override int Water_On()
        {
            /* 条件
             *     1：X轴未运行
             *     2：Y轴未运行
             *     3：接液盘伸出状态
             * 
             */
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
        labelTop:
            bool bReset = false;
            lable:
            int iXStatus = CardObject.OA1.ReadAxisStatus(Convert.ToInt32(boaed.Axis_X));
            if (-1 == iXStatus)
                return -1;

            int iYStatus = CardObject.OA1.ReadAxisStatus(Convert.ToInt32(boaed.Axis_Y));
            if (-1 == iYStatus)
                return -1;

           

            int iTray_Out = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tray_Out));
            if (-1 == iTray_Out)
                return -1;
            else if (0 == iTray_Out)
            {
                //接液盘出
                Lib_Card.ADT8940A1.OutPut.Tray.Tray tray = new Lib_Card.ADT8940A1.OutPut.Tray.Tray_Condition();
                if (-1 == tray.Tray_On())
                    return -1;
                goto labelTop;
            }

            if (0 == iXStatus && 0 == iYStatus && 1 == iTray_Out)
            {
                int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Water), 1);
                if (-1 == iRes)
                    return -1;
               
                return 0;

            }
            else
            {
                if (1 == iXStatus)
                    throw new Exception("X轴正在运行");

                else if (1 == iYStatus)
                    throw new Exception("Y轴正在运行");

                else
                {
                    if (!bReset)
                    {
                        Thread.Sleep(1000);
                        bReset = true;
                        goto lable;
                    }
                    else
                    {
                        bool needSleepAfterInquire = false;
                        while (true)
                        {
                            //接液盘伸出超时询问
                            int code = 10501;
                            int resultCode = code;
                            if (code >= 10000)
                            {
                                var alarm = RobotAlarmOverview.GetAlarmInfo(resultCode);
                                var tcs = new ManualResetEvent(false);
                                //if (code == 12401 || code == 12701)
                                //{
                                //    MessageEventManager.Instance.RequestShowMessage(
                                //      "温馨提示",
                                //      alarm.Description + ",需要人工确认：请先点击确认键，10秒后将自动开启抓手，请准备手动接住针筒/杯盖",
                                //      btn =>
                                //      {
                                //          if (btn == "确认")
                                //          {
                                //              needSleepAfterInquire = true;
                                //          }
                                //          else
                                //          {
                                //              needSleepAfterInquire = false;
                                //          }
                                //          tcs.Set();
                                //      },
                                //      new[] { "确认" },
                                //      "确认"
                                //  );
                                //    tcs.WaitOne(); // 等待用户操作
                                //    if (needSleepAfterInquire)
                                //    {
                                //        Thread.Sleep(10000); // 休眠10s
                                //        needSleepAfterInquire = false;
                                //    }
                                //}
                                //else
                                {

                                    MessageEventManager.Instance.RequestShowMessage(
                                        "温馨提示",
                                        alarm.Description + ",气压或传感器故障。请在完成修复后，点击“继续”以恢复正常运行。如需中止流程，请点击“退出”",
                                        btn =>
                                        {
                                            if (btn == "继续")
                                            {
                                                needSleepAfterInquire = true;

                                            }
                                            else
                                            {
                                                needSleepAfterInquire = false;
                                            }
                                            tcs.Set();
                                        },
                                        new[] { "继续", "退出" },
                                        "继续"
                                    );
                                    tcs.WaitOne(); // 等待用户操作
                                    if (needSleepAfterInquire)
                                    {
                                        bReset = false;
                                        goto lable;
                                    }
                                    else
                                    {
                                        throw new Exception("接液盘伸出超时询问");
                                    }
                                }
                            }
                        }

                        //string s;
                        //if (SmartColor.My_ConPar.Machine.Language == 0)
                        //    s = CardObject.InsertD("接液盘未伸出，请检查，确定伸出请点是，退出运行请点否", " Water_On");
                        //else
                        //    s = CardObject.InsertD("The liquid plate is not extended, please check to make sure that the extension point is yes, and the exit point is no", " Water_On");

                        //while (true)
                        //{
                        //    Thread.Sleep(1);
                        //    if (Lib_Card.CardObject.keyValuePairs[s].Choose != 0)
                        //        break;

                        //}
                        //int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s].Choose;
                        //CardObject.DeleteD(s);
                        //if (Alarm_Choose == 1)
                        //{
                        //    bReset = false;
                        //    goto lable;
                        //}
                        //else
                        //{
                        //    throw new Exception("接液盘未伸出");
                        //}
                    }
                }

            }
        }
    }
}
