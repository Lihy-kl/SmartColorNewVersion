using Lib_Card.ADT8940A1.OutPut.Block;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lib_Card.ADT8940A1.Module
{
    public class CylinderMo
    {
        /// <summary>
        /// 气缸慢速中
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <param name="i_type">0：杯盖区取挡水板；1：放布区取布笼 2：杯盖区放挡水板  3：出布区放布笼 4：配液杯取挡水板 5：配液杯放挡水板 6：配液杯放布笼 7：其他</param>
        /// <returns>0：正常；-1：异常；-2：收到退出消息</returns>
        public int CylinderSlow(int iCylinderVersion,int i_type)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;

            bool bReset = false;
            int i_rep = 0;
            int i_rep_slow = 0;
        lable:
            //检查气缸多个传感器信号是否存在信号
            int i_Cylinder_Up = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
            int i_Cylinder_Down = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
            int i_Slow_Cylinder_Mid = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Slow_Cylinder_Mid));
            int i_Cylinder_Block = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Block));

            int i_count = 0;
            if(i_Cylinder_Up == 1)
            {
                i_count++;
            }
            if (i_Cylinder_Down == 1)
            {
                i_count++;
            }
            if (i_Slow_Cylinder_Mid == 1)
            {
                i_count++;
            }
            if (i_Cylinder_Block == 1)
            {
                i_count++;
            }
            if(i_count > 1)
            {
                bool needSleepAfterInquire = false;
                while (true)
                {
                    //慢速中限位与气缸下已通询问
                    int code = 11129;
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
                                throw new Exception("慢速中限位与气缸下已通");
                            }
                        }
                    }
                }

                //string s = null;
                ////显示多限位接通
                //if (SmartColor.My_ConPar.Machine.Language == 0)
                //    s = CardObject.InsertD("气缸多个限位已接通，请检查，排除异常请点是，退出运行请点否", " CylinderUp");
                //else
                //    s = CardObject.InsertD("Multiple limit positions of the cylinder have been connected, please check, troubleshooting please click Yes, exit please click no", " CylinderUp");

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
                //    throw new Exception("气缸多个限位已接通");
                //}
            }
            //已经在慢速中限位
            if (i_Slow_Cylinder_Mid == 1)
                return 0;

            OutPut.Cylinder.Cylinder cylinder;
            if (0 == iCylinderVersion)
                cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
            else
                cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();

            //判断是否在气缸上位置，如果不是，就先回到气缸上
            if (i_Cylinder_Up==0)
            {
                if (-1 == cylinder.CylinderUp(0))
                    return -1;
                goto lable;
            }

            int iXStatus = CardObject.OA1.ReadAxisStatus(Convert.ToInt32(boaed.Axis_X));
            if (-1 == iXStatus)
                return -1;

            int iYStatus = CardObject.OA1.ReadAxisStatus(Convert.ToInt32(boaed.Axis_Y));
            if (-1 == iYStatus)
                return -1;



            int iXAlarm = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_X_Alarm));
            if (-1 == iXAlarm)
                return -1;

            int iYAlarm = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Y_Alarm));
            if (-1 == iYAlarm)
                return -1;

            int iTrayIn = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tray_In));
            if (-1 == iTrayIn)
                return -1;

            int iTrayOut = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tray_Out));
            if (-1 == iTrayOut)
                return -1;

            if (iTrayIn == 0 || iTrayOut == 1)
            {
                //接液盘收回
                OutPut.Tray.Tray tray = new OutPut.Tray.Tray_Condition();
                if (-1 == tray.Tray_Off())
                    return -1;
                goto lable;
            }
            //判断
            //气缸慢速阀打开，气缸下输出打开，关闭气缸上输出，
            if (0 == iXStatus && 0 == iYStatus && 0 == iXAlarm &&
                    0 == iYAlarm && 1 == iTrayIn)
            {

                //Lib_Card.ADT8940A1.OutPut.Block.Block block = new Lib_Card.ADT8940A1.OutPut.Block.Block_Condition();
                //if (-1 == block.Block_In())
                //    return -1;

                int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Up), 0);
                if (-1 == iRes)
                    return -1;

                iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Down), 1);
                if (-1 == iRes)
                    return -1;

                iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Slow_Cylinder), 1);
                if (-1 == iRes)
                    return -1;

                bool bDelay = false;
                Thread thread = new Thread(() =>
                {
                    int iDelay = Convert.ToInt32(SmartColor.My_ConPar.Delay.Cylinder * 1000.00);
                    Thread.Sleep(iDelay);
                    bDelay = true;
                });
                thread.Start();



                string s = null;
                while (true)
                {
                    Thread.Sleep(1);
                    i_Slow_Cylinder_Mid = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Slow_Cylinder_Mid));
                    i_Cylinder_Block = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Block));
                    if (-1 == i_Slow_Cylinder_Mid)
                        return -1;
                    else if (1 == i_Slow_Cylinder_Mid)
                        break;
                    //判断是否到了气缸阻挡限位，如果是证明已经走过了
                    if (-1 == i_Cylinder_Block)
                        return -1;
                    else if (1 == i_Cylinder_Block)
                    {
                        //关闭所有输出
                        iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Up), 0);
                        if (-1 == iRes)
                            return -1;

                        iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Down), 0);
                        if (-1 == iRes)
                            return -1;

                        iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Slow_Cylinder), 0);
                        if (-1 == iRes)
                            return -1;

                        if (i_type == 0 || i_type == 1)
                        {
                            if(i_rep > 1)
                            {
                                //直接完成不报警
                                
                            }
                            else
                            {
                                //重复执行一次
                                i_rep++;
                                goto lable;
                            }
                        }
                        else if (i_type == 3 || i_type == 2 || i_type == 4 || i_type == 5 || i_type == 6)
                        {
                            //直接完成
                        }
                        else
                        {

                            bool needSleepAfterInquire = false;
                            while (true)
                            {
                                //气缸阻挡限位已通
                                int code = 11122;
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
                                            throw new Exception("气缸阻挡限位已通");
                                        }
                                    }
                                }
                            }

                            //if (SmartColor.My_ConPar.Machine.Language == 0)
                            //    s = CardObject.InsertD("气缸阻挡限位已通，请检查，排除异常请点是，退出运行请点否", " CylinderUp");
                            //else
                            //    s = CardObject.InsertD("The cylinder blocking limit has been activated, please check, troubleshooting please click Yes, exit please click no", " CylinderUp");

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
                            //    throw new Exception("气缸阻挡限位已通");
                            //}
                        }

                        //throw new Exception("气缸阻挡限位已通");
                    }

                    if (bDelay)
                    {
                        //关闭所有输出
                        iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Up), 0);
                        if (-1 == iRes)
                            return -1;

                        iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Down), 0);
                        if (-1 == iRes)
                            return -1;

                        iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Slow_Cylinder), 0);
                        if (-1 == iRes)
                            return -1;
                        if (i_type == 0 || i_type == 1)
                        {
                            if (i_rep_slow > 1)
                            {
                                //直接完成不报警
                                throw new Exception("气缸慢速中超时");

                            }
                            else
                            {
                                //重复执行一次
                                i_rep_slow++;
                                goto lable;
                            }
                        }
                        else if(i_type == 3 || i_type == 4 || i_type == 2)
                        {
                        }
                        else if ( i_type == 5 || i_type == 6)
                        {
                            //需要放回去再执行一次
                            throw new Exception("气缸慢速中超时");
                        }
                        else
                        {
                            bool needSleepAfterInquire = false;
                            while (true)
                            {
                                //气缸慢速中超时
                                int code = 11123;
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
                                            throw new Exception("气缸慢速中超时");
                                        }
                                    }
                                }
                            }

                            //if (SmartColor.My_ConPar.Machine.Language == 0)
                            //    s = CardObject.InsertD("气缸慢速中超时，请检查，排除异常请点是，退出运行请点否", " CylinderUp");
                            //else
                            //    s = CardObject.InsertD("The cylinder exceeded the time limit during slow speed, please check, troubleshooting please click Yes, exit please click no", " CylinderUp");

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
                            //    throw new Exception("气缸慢速中超时");
                            //}
                        }

                    }
                }

                //到位关闭所有输出

                int Res = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Up), 0);
                if (-1 == iRes)
                    return -1;

                iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Down), 0);
                if (-1 == iRes)
                    return -1;
                //延迟1秒关
                Thread.Sleep(1000);
                iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Slow_Cylinder), 0);
                if (-1 == iRes)
                    return -1;

                i_Cylinder_Up = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
                i_Cylinder_Down = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
                i_Slow_Cylinder_Mid = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Slow_Cylinder_Mid));
                i_Cylinder_Block = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Block));

                i_count = 0;
                if (i_Cylinder_Up == 1)
                {
                    i_count++;
                }
                if (i_Cylinder_Down == 1)
                {
                    i_count++;
                }
                if (i_Slow_Cylinder_Mid == 1)
                {
                    i_count++;
                }
                if (i_Cylinder_Block == 1)
                {
                    i_count++;
                }
                if (i_count > 1)
                {
                    bool needSleepAfterInquire = false;
                    while (true)
                    {
                        //慢速中限位与气缸下已通询问
                        int code = 11129;
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
                                    throw new Exception("慢速中限位与气缸下已通");
                                }
                            }
                        }
                    }

                    //s = null;
                    ////显示多限位接通
                    //if (SmartColor.My_ConPar.Machine.Language == 0)
                    //    s = CardObject.InsertD("气缸多个限位已接通，请检查，排除异常请点是，退出运行请点否", " CylinderUp");
                    //else
                    //    s = CardObject.InsertD("Multiple limit positions of the cylinder have been connected, please check, troubleshooting please click Yes, exit please click no", " CylinderUp");

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
                    //    throw new Exception("气缸多个限位已接通");
                    //}
                }

                return 0;

            }
            else
            {
                if (1 == iXStatus)
                    throw new Exception("X轴正在运行");

                else if (1 == iYStatus)
                    throw new Exception("Y轴正在运行");

                else if (1 == iXAlarm)
                    throw new Exception("X轴伺服器报警");

                else if (1 == iYAlarm)
                    throw new Exception("Y轴伺服器报警");

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
                                        bReset = false;
                                        goto lable;
                                    }
                                    else
                                    {
                                        throw new Exception("接液盘收回超时");
                                    }
                                }
                            }
                        }

                        //string s;
                        //if (SmartColor.My_ConPar.Machine.Language == 0)
                        //    s = CardObject.InsertD("接液盘未收回，请检查，确定收回请点是，退出运行请点否", " CylinderDown");
                        //else
                        //    s = CardObject.InsertD("The liquid tray is not recovered, please check to determine the recovery please click yes, exit please click no", " CylinderDown");

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
                        //    throw new Exception("接液盘未收回");
                        //}
                    }
                }
            }
        }

        /// <summary>
        /// 气缸到阻挡位
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <returns>0：正常；-1：异常；-2：收到退出消息</returns>
        public int CylinderBlock(int iCylinderVersion)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            bool bReset = false;
        lable:
            //检查气缸多个传感器信号是否存在信号
            int i_Cylinder_Up = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
            int i_Cylinder_Down = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
            int i_Slow_Cylinder_Mid = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Slow_Cylinder_Mid));
            int i_Cylinder_Block = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Block));

            int i_count = 0;
            if (i_Cylinder_Up == 1)
            {
                i_count++;
            }
            if (i_Cylinder_Down == 1)
            {
                i_count++;
            }
            if (i_Slow_Cylinder_Mid == 1)
            {
                i_count++;
            }
            if (i_Cylinder_Block == 1)
            {
                i_count++;
            }
            if (i_count > 1)
            {
                bool needSleepAfterInquire = false;
                while (true)
                {
                    //慢速中限位与气缸下已通询问
                    int code = 11129;
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
                                throw new Exception("慢速中限位与气缸下已通");
                            }
                        }
                    }
                }

                //string s = null;
                ////显示多限位接通
                //if (SmartColor.My_ConPar.Machine.Language == 0)
                //    s = CardObject.InsertD("气缸多个限位已接通，请检查，排除异常请点是，退出运行请点否", " CylinderUp");
                //else
                //    s = CardObject.InsertD("Multiple limit positions of the cylinder have been connected, please check, troubleshooting please click Yes, exit please click no", " CylinderUp");

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
                //    throw new Exception("气缸多个限位已接通");
                //}
            }
            //已经在气缸阻挡位
            if (i_Cylinder_Block == 1)
                return 0;

            OutPut.Cylinder.Cylinder cylinder;
            if (0 == iCylinderVersion)
                cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
            else
                cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();

            //判断是否在气缸下位置，如果是，就先回到气缸上
            if (i_Cylinder_Down == 1)
            {
                if (-1 == cylinder.CylinderUp(0))
                    return -1;
                goto lable;
            }

            int iXStatus = CardObject.OA1.ReadAxisStatus(Convert.ToInt32(boaed.Axis_X));
            if (-1 == iXStatus)
                return -1;

            int iYStatus = CardObject.OA1.ReadAxisStatus(Convert.ToInt32(boaed.Axis_Y));
            if (-1 == iYStatus)
                return -1;



            int iXAlarm = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_X_Alarm));
            if (-1 == iXAlarm)
                return -1;

            int iYAlarm = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Alarm);
            if (-1 == iYAlarm)
                return -1;

            int iTrayIn = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tray_In));
            if (-1 == iTrayIn)
                return -1;

            int iTrayOut = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tray_Out));
            if (-1 == iTrayOut)
                return -1;

            int iBlock_In = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Block_In));
            if (-1 == iBlock_In)
                return -1;

            int iBlockOut = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Block_Out));
            if (-1 == iBlockOut)
                return -1;

            if (iTrayIn == 0 || iTrayOut == 1)
            {
                //接液盘收回
                OutPut.Tray.Tray tray = new OutPut.Tray.Tray_Condition();
                if (-1 == tray.Tray_Off())
                    return -1;
                goto lable;
            }
            if(iBlock_In == 1 || iBlockOut == 0)
            {
                //伸出阻挡气缸
                Lib_Card.ADT8940A1.OutPut.Block.Block block = new Lib_Card.ADT8940A1.OutPut.Block.Block_Condition();
                if (-1 == block.Block_Out())
                    return -1;
                goto lable;
            }
            //判断
            //气缸下输出打开，关闭气缸上输出，
            if (0 == iXStatus && 0 == iYStatus && 0 == iXAlarm &&
                    0 == iYAlarm && 1 == iTrayIn&& 1==iBlockOut)
            {
                int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Up), 0);
                if (-1 == iRes)
                    return -1;

                iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Down), 1);
                if (-1 == iRes)
                    return -1;

                bool bDelay = false;
                Thread thread = new Thread(() =>
                {
                    int iDelay = Convert.ToInt32(SmartColor.My_ConPar.Delay.Cylinder * 1000.00);
                    Thread.Sleep(iDelay);
                    bDelay = true;
                });
                thread.Start();



                string s = null;
                while (true)
                {
                    Thread.Sleep(1);
                    i_Cylinder_Block = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Block));

                    //判断是否到了气缸阻挡限位，如果是证明已经走过了
                    if (-1 == i_Cylinder_Block)
                        return -1;
                    else if (1 == i_Cylinder_Block)
                        break;
                    if (bDelay)
                    {
                        //关闭所有输出
                        iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Up), 0);
                        if (-1 == iRes)
                            return -1;

                        iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Down), 0);
                        if (-1 == iRes)
                            return -1;
                        //if (SmartColor.My_ConPar.Machine.Language == 0)
                        //    s = CardObject.InsertD("气缸到阻挡超时，请检查，排除异常请点是，退出运行请点否", " CylinderUp");
                        //else
                        //    s = CardObject.InsertD("The cylinder has exceeded the blocking time limit, please check, troubleshooting please click Yes, exit please click no", " CylinderUp");

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
                        //    throw new Exception("气缸到阻挡超时");
                        //}

                        throw new Exception("气缸到阻挡超时");
                    }
                }
                iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Up), 0);
                if (-1 == iRes)
                    return -1;

                iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Down), 0);
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

                else if (1 == iXAlarm)
                    throw new Exception("X轴伺服器报警");

                else if (1 == iYAlarm)
                    throw new Exception("Y轴伺服器报警");

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

                        //string s;
                        //if (SmartColor.My_ConPar.Machine.Language == 0)
                        //    s = CardObject.InsertD("接液盘未收回，请检查，确定收回请点是，退出运行请点否", " CylinderDown");
                        //else
                        //    s = CardObject.InsertD("The liquid tray is not recovered, please check to determine the recovery please click yes, exit please click no", " CylinderDown");

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
                        //    throw new Exception("接液盘未收回");
                        //}
                    }
                }
            }
        }
    }
}
