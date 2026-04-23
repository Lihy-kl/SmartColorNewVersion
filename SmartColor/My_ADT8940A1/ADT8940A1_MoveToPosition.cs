using Lib_Card;
using Lib_Card.ADT8940A1.Module.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartColor.My_ADT8940A1
{
    internal class ADT8940A1_MoveToPosition
    {
        public Task<int> MoveToPosition(int iCylinderVersion, int iX, int iY, int iType)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            return Task.Run(() =>
            {
                try
                {
                    if (!Lib_Card.ADT8940A1.Module.Home.Home.Home_XYZFinish)
                    {
                        Lib_Card.ADT8940A1.Module.Home.Home home = new Lib_Card.ADT8940A1.Module.Home.Home_Condition();
                        if (-1 == home.Home_XYZ(iCylinderVersion))
                            return -1;
                    }

                    int iXRes = -1;
                    Thread threadX = new Thread(() =>
                    {
                        try
                        {
                            iXRes = CardObject.OA1Axis.Absolute_X(iCylinderVersion, iX, 0);
                        }
                        catch (Exception ex)
                        {
                            if ("X轴矢能未接通" == ex.Message)
                                iXRes = -3;
                            if ("X轴伺服器报警" == ex.Message)
                                iXRes = -4;
                            if ("X轴正限位已通" == ex.Message)
                                iXRes = -5;
                            if ("X轴反限位已通" == ex.Message)
                                iXRes = -6;
                        }
                    });
                    threadX.Start();

                    int iYRes = CardObject.OA1Axis.Absolute_Y(iCylinderVersion, iY, 0);
                    if (0 != iYRes)
                        return iYRes;

                    threadX.Join();
                    if (-1 == iXRes)
                        return -1;
                    else if (-2 == iYRes)
                        return -2;
                    else if (-3 == iXRes)
                        throw new Exception("X轴矢能未接通");
                    else if (-4 == iXRes)
                        throw new Exception("X轴伺服器报警");
                    else if (-5 == iXRes)
                        throw new Exception("X轴正限位已通");
                    else if (-6 == iXRes)
                        throw new Exception("X轴反限位已通");


                    if (iType == 1)
                    {
                        Lib_Card.ADT8940A1.OutPut.X_Power.X_Power x_Power = new Lib_Card.ADT8940A1.OutPut.X_Power.X_Power_Condition();
                        if (-1 == x_Power.X_Power_Off())
                            return -1;

                        Lib_Card.ADT8940A1.OutPut.Y_Power.Y_Power y_Power = new Lib_Card.ADT8940A1.OutPut.Y_Power.Y_Power_Condition();
                        if (-1 == y_Power.Y_Power_Off())
                            return -1;

                        Lib_Card.ADT8940A1.OutPut.Tray.Tray tray = new Lib_Card.ADT8940A1.OutPut.Tray.Tray_Condition();
                        if (-1 == tray.Tray_Off())
                            return -1;

                        //if (SmartColor.My_ConPar.Other.ActualPosition == 0)
                        //{
                        //    Home.Home_XYZFinish = false;
                        //}
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
                    else if (ex.Message == "X轴准备信号未接通")
                    {
                        return 21;
                    }
                    else if (ex.Message == "Y轴准备信号未接通")
                    {
                        return 121;
                    }
                    else if (ex.Message == "Y轴正在运行")
                    {
                        return 111;
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
                    else if (ex.Message == "气缸上超时")
                    {
                        return 301;
                    }
                    else if (ex.Message == "气缸下信号已接通")
                    {
                        return 301;
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
