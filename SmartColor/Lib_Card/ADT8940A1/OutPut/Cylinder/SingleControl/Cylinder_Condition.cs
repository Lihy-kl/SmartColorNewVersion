using System;
using System.Threading;

namespace Lib_Card.ADT8940A1.OutPut.Cylinder.SingleControl
{
    /// <summary>
    /// 单控版
    /// 有条件检查
    /// </summary>
    public class Cylinder_Condition : Cylinder
    {
        public override int CylinderDown(int i_judge)
        {
            /* 条件
             *    1：X轴未运行
             *    2：Y轴未运行
             *    3：X轴未报警
             *    4：Y轴未报警
             *    5：接液盘收回状态
             */

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            bool bReset = false;
            int iCylinderDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
            if (-1 == iCylinderDown)
                return -1;
            else if (1 == iCylinderDown)
                return 0;
            else
            {
                lable:
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

                if (0 == iXStatus && 0 == iYStatus && 0 == iXAlarm &&
                    0 == iYAlarm && 1 == iTrayIn)
                {

                    int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Down), 1);
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
                        iCylinderDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
                        if (-1 == iCylinderDown)
                            return -1;
                        else if (1 == iCylinderDown)
                            break;
                        if (bDelay)
                        {
                            // 用于开关盖判断
                            if (i_judge == 1)
                            {
                                return -9;
                            }
                            else if (i_judge == 2)
                            {
                                return -8;
                            }
                            else
                            {
                                //s = CardObject.InsertD("气缸下超时", " CylinderDown");
                                if (SmartColor.My_ConPar.Machine.Language == 0)
                                    s = CardObject.InsertD("气缸下超时，请检查，排除异常请点是，退出运行请点否", " CylinderUp");
                                else
                                    s = CardObject.InsertD("Under the cylinder timeout, please check, troubleshooting please click Yes, exit please click no", " CylinderUp");

                                while (true)
                                {
                                    Thread.Sleep(1);
                                    if (Lib_Card.CardObject.keyValuePairs[s].Choose != 0)
                                        break;

                                }
                                int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s].Choose;
                                CardObject.DeleteD(s);
                                if (Alarm_Choose == 1)
                                {
                                    goto lable;
                                }
                                else
                                {
                                    throw new Exception("气缸下超时");
                                }
                            }
                        }

                    }

                    //if (bDelay)
                    //    Lib_Card.CardObject.DeleteD(s);


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
                            string s;
                            if (SmartColor.My_ConPar.Machine.Language == 0)
                                s = CardObject.InsertD("接液盘未收回，请检查，确定收回请点是，退出运行请点否", " CylinderDown");
                            else
                                s = CardObject.InsertD("The liquid tray is not recovered, please check to determine the recovery please click yes, exit please click no", " CylinderDown");

                            while (true)
                            {
                                Thread.Sleep(1);
                                if (Lib_Card.CardObject.keyValuePairs[s].Choose != 0)
                                    break;

                            }
                            int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s].Choose;
                            CardObject.DeleteD(s);
                            if (Alarm_Choose == 1)
                            {
                                bReset = false;
                                goto lable;
                            }
                            else
                            {
                                throw new Exception("接液盘未收回");
                            }
                        }
                    }
                }
            }
        }

        public override int CylinderUp(int i_judge)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
        lable:
            int iCylinderUp = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
            if (-1 == iCylinderUp)
                return -1;
            else if (1 == iCylinderUp)
                return 0;
            else
            {

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
                    iCylinderUp = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
                    if (-1 == iCylinderUp)
                        return -1;
                    else if (1 == iCylinderUp)
                        break;
                    if (i_judge == 2)
                    {
                        //气缸下无信号就退出
                        int iCylinderDown1 = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
                        if (-1 == iCylinderDown1)
                            return -1;
                        else if (0 == iCylinderDown1)
                        {
                            return -8;
                        }
                    }
                    if (bDelay)
                    {
                        // 用于开关盖判断
                        if (i_judge == 1)
                        {
                            return -9;
                        }
                        else
                        {
                            //s = CardObject.InsertD("气缸上超时", " CylinderUp");
                            if (SmartColor.My_ConPar.Machine.Language == 0)
                                s = CardObject.InsertD("气缸上超时，请检查，排除异常请点是，退出运行请点否", " CylinderUp");
                            else
                                s = CardObject.InsertD("Time out on the cylinder, please check, troubleshoot the exception, please click Yes, exit the operation, please click No", " CylinderUp");

                            while (true)
                            {
                                Thread.Sleep(1);
                                if (Lib_Card.CardObject.keyValuePairs[s].Choose != 0)
                                    break;

                            }
                            int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s].Choose;
                            CardObject.DeleteD(s);
                            if (Alarm_Choose == 1)
                            {
                                goto lable;
                            }
                            else
                            {
                                throw new Exception("气缸上超时");
                            }
                        }
                    }
                }

                //if (bDelay)
                //    Lib_Card.CardObject.DeleteD(s);

                return 0;
            }
        }

        public override int CylinderMid()
        {
            return -1;
        }
    }
}
