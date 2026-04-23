using Lib_Card.ADT8940A1.OutPut.Tongs;
using SmartColor.My_Tool;
using System;
using System.Reflection.Emit;
using System.Threading;

namespace Lib_Card.ADT8940A1.OutPut.Tray
{
    /// <summary>
    /// 带条件检查
    /// </summary>
    public class Tray_Condition : Tray
    {
        public override int Tray_Off()
        {
            /* 条件
             *    1：Z轴未运行
             *    2：气缸在上限位
             */

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
        labelTop:
            bool  bReset = false;
            int iTrayIn = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tray_In));
            if (-1 == iTrayIn)
                return -1;
            //else if (1 == iTrayIn)
            //    return 0;
            else
            {
                lable:
                int iZStatus = CardObject.OA1.ReadAxisStatus(Convert.ToInt32(boaed.Axis_Z));
                if (-1 == iZStatus)
                    return -1;



                int iCylinderUp = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
                if (-1 == iCylinderUp)
                    return -1;
                int iCylinderDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
                if (-1 == iCylinderDown)
                    return -1;
                if (iCylinderUp == 0 || iCylinderDown == 1)
                {
                    //气缸上
                    OutPut.Cylinder.Cylinder cylinder;
                    if (0 == 1)
                        cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
                    else
                        cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();

                    if (-1 == cylinder.CylinderUp(0))
                        return -1;

                    goto labelTop;
                }

                if (0 == iZStatus && 1 == iCylinderUp)
                {
                    int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Tray), 0);
                    if (-1 == iRes)
                        return -1;

                    bool bDelay = false;
                    Thread thread = new Thread(() =>
                    {
                        int iDelay = Convert.ToInt32(SmartColor.My_ConPar.Delay.Tray * 1000.00);
                        Thread.Sleep(iDelay);
                        bDelay = true;
                    });
                    thread.Start();

                    string s = null;
                    while (true)
                    {
                        Thread.Sleep(1);
                        iTrayIn = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tray_In));
                        if (-1 == iTrayIn)
                            return -1;
                        else if (1 == iTrayIn)
                            break;
                        if (bDelay)
                        {
                            bool needSleepAfterInquire = false;
                            while (true)
                            {
                                //接液盘收回超时
                                int code = 10511;
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
                                            throw new Exception("接液盘收回超时");
                                        }
                                    }
                                }
                            }
                            ////s = CardObject.InsertD("接液盘收回超时", "Tray_Off");
                            //if (SmartColor.My_ConPar.Machine.Language == 0)
                            //    s = CardObject.InsertD("接液盘收回超时，请检查，排除异常请点是，退出运行请点否", " Tray_On");
                            //else
                            //    s = CardObject.InsertD("Liquid disk recovery timeout, please check, troubleshoot the exception, please click Yes, exit the operation, please click No", " Tray_On");

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
                            //    goto lable;
                            //}
                            //else
                            //{
                            //    throw new Exception("接液盘收回超时");
                            //}
                        }

                    }
                    //if(bDelay)
                    //Lib_Card.CardObject.DeleteD(s);

                    //再判断一下接液盘回信号是否有，有就提示，要手动确认没问题才可以继续
                    int iTrayOut = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tray_Out));
                    if (-1 == iTrayOut)
                        return -1;
                    else if (1 == iTrayOut)
                    {
                        bool needSleepAfterInquire = false;
                        while (true)
                        {
                            //接液盘出信号已接通
                            int code = 10512;
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
                                        throw new Exception("接液盘出信号已接通");
                                    }
                                }
                            }
                        }

                        //if (SmartColor.My_ConPar.Machine.Language == 0)
                        //    s = CardObject.InsertD("接液盘出信号已接通，请检查，确定无接通请点是，退出运行请点否", " Tray_Off");
                        //else
                        //    s = CardObject.InsertD("The outgoing signal of the liquid plate is connected, please check, confirm that it is not connected, please click Yes, please click No to exit the operation", " Tray_Off");

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
                        //    goto labelTop;
                        //}
                        //else
                        //{
                        //    throw new Exception("接液盘出信号已接通");
                        //}
                    }

                    return 0;

                }
                else
                {
                    if (1 == iZStatus)
                        throw new Exception("Z轴正在运行");

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
                                //气缸上超时询问
                                int code = 10301;
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
                                            throw new Exception("气缸上超时询问");
                                        }
                                    }
                                }
                            }

                            //string s;
                            //if (SmartColor.My_ConPar.Machine.Language == 0)
                            //    s = CardObject.InsertD("气缸未在上限位，请检查，确定到位请点是，退出运行请点否", " Tray_On");
                            //else
                            //    s = CardObject.InsertD("The cylinder is not in the upper limit, please check, confirm in place, please click Yes, exit operation, please click No", " Tray_On");

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
                            //    throw new Exception("气缸未在上限位");
                            //}

                        }
                    }

                }
            }
        }

        public override int Tray_On()
        {
            /* 条件
            *    1：Z轴未运行
            *    2：气缸在上限位
            */

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
        labelTop:
            bool bReset = false;
            int iTrayOut = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tray_Out));
            if (-1 == iTrayOut)
                return -1;
            //else if (1 == iTrayOut)
            //    return 0;
            else
            {
                lable:
                int iZStatus = CardObject.OA1.ReadAxisStatus(Convert.ToInt32(boaed.Axis_Z));
                if (-1 == iZStatus)
                    return -1;



                int iCylinderUp = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
                if (-1 == iCylinderUp)
                    return -1;
                int iCylinderDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
                if (-1 == iCylinderDown)
                    return -1;
                if(iCylinderUp==0 || iCylinderDown==1)
                {
                    //气缸上
                    OutPut.Cylinder.Cylinder cylinder;
                    if (0 == 1)
                        cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
                    else
                        cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();

                    if (-1 == cylinder.CylinderUp(0))
                        return -1;

                    goto labelTop;
                }

                if (0 == iZStatus && 1 == iCylinderUp)
                {
                    int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Tray), 1);
                    if (-1 == iRes)
                        return -1;

                    bool bDelay = false;
                    Thread thread = new Thread(() =>
                    {
                        int iDelay = Convert.ToInt32(SmartColor.My_ConPar.Delay.Tray * 1000.00);
                        Thread.Sleep(iDelay);
                        bDelay = true;
                    });
                    thread.Start();

                    string s = null;
                    while (true)
                    {
                        Thread.Sleep(1);
                        iTrayOut = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tray_Out));
                        if (-1 == iTrayOut)
                            return -1;
                        else if (1 == iTrayOut)
                            break;
                        if (bDelay)
                        {
                            bool needSleepAfterInquire = false;
                            while (true)
                            {
                                //接液盘伸出超时
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

                                            goto lable;
                                        }
                                        else
                                        {
                                            throw new Exception("接液盘伸出超时");
                                        }
                                    }
                                }
                            }

                            ////s = CardObject.InsertD("接液盘伸出超时", "Tray_On");
                            //if (SmartColor.My_ConPar.Machine.Language == 0)
                            //    s = CardObject.InsertD("接液盘伸出超时，请检查，排除异常请点是，退出运行请点否", " Tray_On");
                            //else
                            //    s = CardObject.InsertD("Liquid plate extension time out, please check, troubleshooting please click Yes, exit please click no", " Tray_On");

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
                            //    goto lable;
                            //}
                            //else
                            //{
                            //    throw new Exception("接液盘伸出超时");
                            //}
                        }
                        
                    }
                    //if (bDelay)
                    //    Lib_Card.CardObject.DeleteD(s);

                    //再判断一下接液盘回信号是否有，有就提示，要手动确认没问题才可以继续
                    int iTrayIn = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tray_In));
                    if (-1 == iTrayIn)
                        return -1;
                    else if (1 == iTrayIn)
                    {
                        bool needSleepAfterInquire = false;
                        while (true)
                        {
                            //接液盘回信号已接通
                            int code = 10502;
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
                                        throw new Exception("接液盘回信号已接通");
                                    }
                                }
                            }
                        }

                        //if (SmartColor.My_ConPar.Machine.Language == 0)
                        //    s = CardObject.InsertD("接液盘回信号已接通，请检查，确定无接通请点是，退出运行请点否", " Tray_On");
                        //else
                        //    s = CardObject.InsertD("The return signal of the liquid tray is connected, please check, confirm that it is not connected, please click Yes, please click No to exit the operation", " Tray_On");

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
                        //    goto labelTop;
                        //}
                        //else
                        //{
                        //    throw new Exception("接液盘回信号已接通");
                        //}
                    }
                    return 0;

                }
                else
                {
                    if (1 == iZStatus)
                        throw new Exception("Z轴正在运行");

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
                                //气缸上超时询问
                                int code = 10301;
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
                                            throw new Exception("气缸上超时询问");
                                        }
                                    }
                                }
                            }

                            //string s;
                            //if (SmartColor.My_ConPar.Machine.Language == 0)
                            //    s = CardObject.InsertD("气缸未在上限位，请检查，确定到位请点是，退出运行请点否", " Tray_On");
                            //else
                            //    s = CardObject.InsertD("The cylinder is not in the upper limit, please check, confirm in place, please click Yes, exit operation, please click No", " Tray_On");

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
                            //    throw new Exception("气缸未在上限位");
                            //}
                        }
                    }

                }
            }
        }
    }
}
