using SmartColor.My_Tool;
using System;
using System.Threading;

namespace Lib_Card.ADT8940A1.OutPut.Tongs
{
    /// <summary>
    /// 带条件检查
    /// </summary>
    public class Tongs_Condition : Tongs
    {
        public override int Tongs_Off()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
        lable:
            int iTongsA = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tongs_A));
            if (-1 == iTongsA)
                return -1;
            int iTongsB = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tongs_B));
            if (-1 == iTongsB)
                return -1;
            if (1 == iTongsA && 1 == iTongsB)
                return 0;
            else
            {
                if (SmartColor.My_ConPar.Hardware.TongType == 0)
                {
                    int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_TongsOn), 0);
                    if (-1 == iRes)
                        return -1;
                }
                else
                {
                    int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_TongsOn), 0);
                    if (-1 == iRes)
                        return -1;
                    int iRes1 = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_TongsOff), 1);
                    if (-1 == iRes1)
                        return -1;
                }

                bool bDelay = false;
                Thread thread = new Thread(() =>
                {
                    int iDelay = Convert.ToInt32(SmartColor.My_ConPar.Delay.Tongs * 1000.00);
                    Thread.Sleep(iDelay);
                    bDelay = true;
                });
                thread.Start();

                string s1 = null, s2 = null;
                while (true)
                {
                    Thread.Sleep(1);
                    iTongsA = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tongs_A));
                    if (-1 == iTongsA)
                        return -1;

                    iTongsB = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tongs_B));
                    if (-1 == iTongsB)
                        return -1;

                    if (1 == iTongsA && 1 == iTongsB)
                        break;

                    if (bDelay)
                    {
                        if (1 == iTongsA)
                        {
                            bool needSleepAfterInquire = false;
                            while (true)
                            {
                                //抓手A打开超时
                                int code = 10401;
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

                                            goto lable;
                                        }
                                        else
                                        {
                                            throw new Exception("抓手A打开超时");
                                        }
                                    }
                                }
                            }

                            ////s1 = CardObject.InsertD("抓手A打开超时", "Tongs_Off");
                            //if (SmartColor.My_ConPar.Machine.Language == 0)
                            //    s1 = CardObject.InsertD("抓手A打开超时，请检查，排除异常请点是，退出运行请点否", " Tongs_Off");
                            //else
                            //    s1 = CardObject.InsertD("Gripper A opens the timeout, please check, troubleshoot the exception, please click Yes, exit the run, please click No", " Tongs_Off");
                            //while (true)
                            //{
                            //    Thread.Sleep(1);
                            //    if (Lib_Card.CardObject.keyValuePairs[s1].Choose != 0)
                            //        break;

                            //}
                            //int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s1].Choose;
                            //CardObject.DeleteD(s1);
                            //if (Alarm_Choose == 1)
                            //{
                            //    goto lable;
                            //}
                            //else
                            //{
                            //    throw new Exception("抓手A打开超时");
                            //}
                        }
                        else if (1 == iTongsB)
                        {
                            bool needSleepAfterInquire = false;
                            while (true)
                            {
                                //抓手B打开超时
                                int code = 10402;
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

                                            goto lable;
                                        }
                                        else
                                        {
                                            throw new Exception("抓手B打开超时");
                                        }
                                    }
                                }
                            }

                            ////s2 = CardObject.InsertD("抓手B打开超时", "Tongs_Off");
                            //if (SmartColor.My_ConPar.Machine.Language == 0)
                            //    s2 = CardObject.InsertD("抓手B打开超时，请检查，排除异常请点是，退出运行请点否", " Tongs_Off");
                            //else
                            //    s2 = CardObject.InsertD("Gripper B opens the timeout, please check, troubleshoot the exception, please click Yes, exit the run, please click No", " Tongs_Off");

                            //while (true)
                            //{
                            //    Thread.Sleep(1);
                            //    if (Lib_Card.CardObject.keyValuePairs[s2].Choose != 0)
                            //        break;

                            //}
                            //int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s2].Choose;
                            //CardObject.DeleteD(s2);
                            //if (Alarm_Choose == 1)
                            //{
                            //    goto lable;
                            //}
                            //else
                            //{
                            //    throw new Exception("抓手B打开超时");
                            //}
                        }
                    }

                }

                //if (bDelay)
                //{
                //    Lib_Card.CardObject.DeleteD(s1);
                //    Lib_Card.CardObject.DeleteD(s2);
                //}
                return 0;
            }
        }

        public override int Tongs_On()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
        lable:
            int iTongsA = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tongs_A));
            if (-1 == iTongsA)
                return -1;
            int iTongsB = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tongs_B));
            if (-1 == iTongsB)
                return -1;
            if (0 == iTongsA && 0 == iTongsB)
                return 0;
            else
            {
                if (SmartColor.My_ConPar.Hardware.TongType == 0)
                {
                    int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_TongsOn), 1);
                    if (-1 == iRes)
                        return -1;
                }
                else
                {
                    int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_TongsOn), 1);
                    if (-1 == iRes)
                        return -1;

                    int iRes1 = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_TongsOff), 0);
                    if (-1 == iRes1)
                        return -1;
                }

                bool bDelay = false;
                Thread thread = new Thread(() =>
                {
                    int iDelay = Convert.ToInt32(SmartColor.My_ConPar.Delay.Tongs * 1000.00);
                    Thread.Sleep(iDelay);
                    bDelay = true;
                });
                thread.Start();

                string s1 =null, s2 =null;
                while (true)
                {
                    Thread.Sleep(1);
                    iTongsA = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tongs_A));
                    if (-1 == iTongsA)
                        return -1;

                    iTongsB = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tongs_B));
                    if (-1 == iTongsB)
                        return -1;

                    if (0 == iTongsA && 0 == iTongsB)
                        break;

                    if (bDelay)
                    {
                        if (1 == iTongsA)
                        {
                            bool needSleepAfterInquire = false;
                            while (true)
                            {
                                //抓手A关闭超时
                                int code = 10411;
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

                                            goto lable;
                                        }
                                        else
                                        {
                                            throw new Exception("抓手A关闭超时");
                                        }
                                    }
                                }
                            }

                            //if (SmartColor.My_ConPar.Machine.Language == 0)
                            //    s1 = CardObject.InsertD("抓手A关闭超时，请检查，排除异常请点是，退出运行请点否", " Tongs_On");
                            //else
                            //    s1 = CardObject.InsertD("Gripper A closes the timeout. Please check. For troubleshooting exceptions, click Yes. For exiting the operation, click No", " Tongs_On");

                            //while (true)
                            //{
                            //    Thread.Sleep(1);
                            //    if (Lib_Card.CardObject.keyValuePairs[s1].Choose != 0)
                            //        break;

                            //}
                            //int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s1].Choose;
                            //CardObject.DeleteD(s1);
                            //if (Alarm_Choose == 1)
                            //{
                            //    goto lable;
                            //}
                            //else
                            //{
                            //    throw new Exception("抓手A关闭超时");
                            //}
                        }
                        else if (1 == iTongsB)
                        {
                            bool needSleepAfterInquire = false;
                            while (true)
                            {
                                //抓手B关闭超时
                                int code = 10412;
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

                                            goto lable;
                                        }
                                        else
                                        {
                                            throw new Exception("抓手B关闭超时");
                                        }
                                    }
                                }
                            }

                            //if (SmartColor.My_ConPar.Machine.Language == 0)
                            //    s2 = CardObject.InsertD("抓手B关闭超时，请检查，排除异常请点是，退出运行请点否", " Tongs_On");
                            //else
                            //    s2 = CardObject.InsertD("Gripper B closes the timeout. Please check. For troubleshooting exceptions, click Yes. For exiting the operation, click No", " Tongs_On");

                            //while (true)
                            //{
                            //    Thread.Sleep(1);
                            //    if (Lib_Card.CardObject.keyValuePairs[s2].Choose != 0)
                            //        break;

                            //}
                            //int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s2].Choose;
                            //CardObject.DeleteD(s2);
                            //if (Alarm_Choose == 1)
                            //{
                            //    goto lable;
                            //}
                            //else
                            //{
                            //    throw new Exception("抓手B关闭超时");
                            //}
                        }
                    }

                }

                //if (bDelay)
                //{
                //    Lib_Card.CardObject.DeleteD(s1);
                //    Lib_Card.CardObject.DeleteD(s2);
                //}
                return 0;
            }
        }
    }
}
