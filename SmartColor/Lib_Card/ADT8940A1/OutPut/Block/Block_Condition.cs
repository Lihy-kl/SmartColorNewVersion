using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lib_Card.ADT8940A1.OutPut.Block
{
    public class Block_Condition : Block
    {
        public override int Block_In()
        {
            /* 条件
             *    1：气缸在上限位
             */


            if (SmartColor.My_ConPar.Hardware.Block == 0)
            {
                return 0;
            }

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;

        lable:
            int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Block_Out), 0);
            if (-1 == iRes)
                return -1;
            iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Block_In), 1);
            if (-1 == iRes)
                return -1;

            bool bDelay = false;
            Thread thread = new Thread(() =>
            {
                int iDelay = Convert.ToInt32(SmartColor.My_ConPar.Delay.Block * 1000.00);
                Thread.Sleep(iDelay);
                bDelay = true;
            });
            thread.Start();

            //string sPath = Environment.CurrentDirectory + "\\Config\\DataBase.ini";
            //Lib_DataBank.SQLServer.SQLServerCon con = new Lib_DataBank.SQLServer.SQLServerCon()
            //{
            //    Server = Lib_File.Ini.GetIni("FADM", "Server", sPath),
            //    Port = Lib_File.Ini.GetIni("FADM", "Port", sPath),
            //    Database = Lib_File.Ini.GetIni("FADM", "Database", sPath),
            //    UserName = Lib_File.Ini.GetIni("FADM", "UserName", sPath),
            //    Password = Lib_File.Ini.GetIni("FADM", "Password", sPath)
            //};
            //Lib_DataBank.SQLServer sQLServer = new Lib_DataBank.SQLServer(con);

            int iBlockOut = 0;
            string s = null;
            while (true)
            {
                Thread.Sleep(1);
                iBlockOut = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Block_In));
                if (-1 == iBlockOut)
                    return -1;
                else if (1 == iBlockOut)
                    break;
                if (bDelay)
                {
                    bool needSleepAfterInquire = false;
                    while (true)
                    {
                        //阻挡收回超时
                        int code = 11112;
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
                                    throw new Exception("阻挡气缸收回超时");
                                }
                            }
                        }
                    }

                    ////s = CardObject.InsertD("阻挡气缸收回超时", "Block_In");
                    //if (SmartColor.My_ConPar.Machine.Language == 0)
                    //    s = CardObject.InsertD("阻挡气缸收回超时，请检查，排除异常请点是，退出运行请点否", " Block_In");
                    //else
                    //    s = CardObject.InsertD("Block cylinder recovery timeout, please check, troubleshooting please click Yes, exit please click no", " Block_In");
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
                    //    throw new Exception("阻挡气缸收回超时");
                    //}
                }


            }

            //if (bDelay)
            //    Lib_Card.CardObject.DeleteD(s);

            return 0;


        }

        public override int Block_Out()
        {
            /* 条件
             *    1：气缸在上限位
             */
            if (SmartColor.My_ConPar.Hardware.Block == 0)
            {
                return 0;
            }

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            bool bReset = false;
        lable:
            int iCylinderUp = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
            if (-1 == iCylinderUp)
                return -1;



            if (1 == iCylinderUp)
            {
                int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Block_Out), 1);
                if (-1 == iRes)
                    return -1;
                iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Block_In), 0);
                if (-1 == iRes)
                    return -1;

                bool bDelay = false;
                Thread thread = new Thread(() =>
                {
                    int iDelay = Convert.ToInt32(SmartColor.My_ConPar.Delay.Block * 1000.00);
                    Thread.Sleep(iDelay);
                    bDelay = true;
                });
                thread.Start();

                int iBlockOut = 0;
                string s = null;
                while (true)
                {
                    Thread.Sleep(1);
                    iBlockOut = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Block_Out));
                    if (-1 == iBlockOut)
                        return -1;
                    else if (1 == iBlockOut)
                        break;
                    if (bDelay)
                    {
                        bool needSleepAfterInquire = false;
                        while (true)
                        {
                            //阻挡伸出超时
                            int code = 11101;
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
                                        throw new Exception("阻挡气缸伸出超时");
                                    }
                                }
                            }
                        }

                        ////s = CardObject.InsertD("阻挡气缸伸出超时", "Block_Out");
                        //if (SmartColor.My_ConPar.Machine.Language == 0)
                        //    s = CardObject.InsertD("阻挡气缸伸出超时，请检查，排除异常请点是，退出运行请点否", " Block_Out");
                        //else
                        //    s = CardObject.InsertD("Block cylinder extension time out, please check, troubleshooting please click Yes, exit operation please click no", " Block_Out");
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
                        //    if (SmartColor.My_ConPar.Machine.Language == 0)
                        //        throw new Exception("阻挡气缸伸出超时");
                        //    else
                        //        throw new Exception("Block cylinder extension time out");
                        //}
                    }

                }

                //if (bDelay)
                //    Lib_Card.CardObject.DeleteD(s);

                return 0;

            }
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
                        //气缸上超时
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
                                    throw new Exception("气缸上超时");
                                }
                            }
                        }
                    }

                    //string s;
                    //if (SmartColor.My_ConPar.Machine.Language == 0)
                    //    s = CardObject.InsertD("气缸未在上限位，请检查，确定到位请点是，退出运行请点否", " Block_Out");
                    //else
                    //    s = CardObject.InsertD("The cylinder is not in the upper limit, please check, confirm in place, please click Yes, exit operation, please click No", " Block_Out");

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
                    //    if (SmartColor.My_ConPar.Machine.Language == 0)
                    //        throw new Exception("气缸未在上限位");
                    //    else
                    //        throw new Exception("Cylinder is not in upper limit position");
                    //}
                }
            }
        }
    }
}
