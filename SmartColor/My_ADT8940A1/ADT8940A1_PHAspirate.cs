using Lib_Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartColor.My_ADT8940A1
{
    internal class ADT8940A1_PHAspirate
    {
        /// <summary>
        /// PH抽液（异步方法）
        /// </summary>
        /// <param name="z">Z轴坐标</param>
        /// <param name="syringeType">针筒规格 0=小针筒 1=大针筒</param>
        /// <returns>动作完成/异常码</returns>
        public Task<int> PHAspirateAsync(int z, short syringeType)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            return Task.Run(() =>
            {
                try
                {
                    //Lib_Card.ADT8940A1.OutPut.Blender.Blender blender = new Lib_Card.ADT8940A1.OutPut.Blender.Blender_Basic();
                    //if (-1 == blender.Blender_On())
                    //    return -1;

                    Lib_Card.ADT8940A1.OutPut.Tray.Tray tray = new Lib_Card.ADT8940A1.OutPut.Tray.Tray_Condition();


                    Lib_Card.ADT8940A1.OutPut.Cylinder.Cylinder cylinder;
                    if (0 == SmartColor.My_ConPar.Hardware.CylinderType)
                        cylinder = new Lib_Card.ADT8940A1.OutPut.Cylinder.SingleControl.Cylinder_Condition();
                    else
                        cylinder = new Lib_Card.ADT8940A1.OutPut.Cylinder.DualControl.Cylinder_Condition();



                    Lib_Card.ADT8940A1.OutPut.Tongs.Tongs tongs = new Lib_Card.ADT8940A1.OutPut.Tongs.Tongs_Condition();


                    bool bDelay = false;
                    Thread threadS = new Thread(() =>
                    {
                        int iDelay = Convert.ToInt32(SmartColor.My_ConPar.Delay.Syringe * 1000.00);
                        Thread.Sleep(iDelay);
                        bDelay = true;
                    });
                    threadS.Start();

                    int iSyringe = 0;

                    while (true)
                    {

                        Thread.Sleep(1);
                        iSyringe = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Syringe));
                        if (SmartColor.My_ConPar.Choices.IgnoreSyringeSensor == 1)
                        {
                            iSyringe = 1;
                        }
                        if (-1 == iSyringe)
                            return -1;
                        else if (1 == iSyringe)
                            break;

                        if (bDelay)
                            break;

                    }


                    Lib_Card.Base.Card.MoveArg s_Movearg = new Lib_Card.Base.Card.MoveArg();
                    if (bDelay && 0 == iSyringe)
                    {
                        if (-1 == tongs.Tongs_Off())
                            return -1;

                        if (0 == syringeType)
                        {
                            s_Movearg.Pulse = -(SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Z_Offset;
                            s_Movearg.LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_S_LSpeed;
                            s_Movearg.HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_S_HSpeed;
                            s_Movearg.Time = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_S_USpeed;
                        }
                        else if (1 == syringeType)
                        {
                            s_Movearg.Pulse = -(SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Z_Offset;
                            s_Movearg.LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_B_LSpeed;
                            s_Movearg.HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_B_HSpeed;
                            s_Movearg.Time = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_B_USpeed;
                        }
                        try
                        {
                            if (-1 == CardObject.OA1Axis.Relative_Z(s_Movearg, 0))
                                return -1;
                        }
                        catch (Exception ex)
                        {
                            if ("Z轴反限位已通" != ex.Message)
                                throw;
                        }

                        if (-1 == tongs.Tongs_On())
                            return -1;

                        bDelay = false;
                        threadS = new Thread(() =>
                        {
                            int iDelay = Convert.ToInt32(SmartColor.My_ConPar.Delay.Syringe * 1000.00);
                            Thread.Sleep(iDelay);
                            bDelay = true;
                        });
                        threadS.Start();

                        while (true)
                        {
                            Thread.Sleep(1);
                            iSyringe = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Syringe));
                            if (SmartColor.My_ConPar.Choices.IgnoreSyringeSensor == 1)
                            {
                                iSyringe = 1;
                            }
                            if (-1 == iSyringe)
                                return -1;
                            else if (1 == iSyringe)
                                break;

                            if (bDelay)
                                break;

                        }

                        if (bDelay && 0 == iSyringe)
                        {
                            if (-1 == tongs.Tongs_Off())
                                return -1;
                            if (-1 == cylinder.CylinderUp(0))
                                return -1;

                            Lib_Card.ADT8940A1.Module.Home.Home home = new Lib_Card.ADT8940A1.Module.Home.Home_Condition();
                            if (-1 == home.Home_Z(SmartColor.My_ConPar.Hardware.CylinderType))
                                throw new Exception("驱动异常");

                            throw new Exception("未发现针筒");
                        }
                    }

                    int iTotalPulse = z /*- Configure.Parameter.Other_Z_BackPulse*/;
                    int iZRes = CardObject.OA1Axis.Absolute_Z(syringeType, iTotalPulse, 0);
                    if (0 != iZRes)
                        return iZRes;
                    if (SmartColor.My_ConPar.Other.Push == 1)
                    {
                        //先反推完在升气缸
                        iZRes = CardObject.OA1Axis.Absolute_Z(syringeType, z, 0);
                        if (0 != iZRes)
                            return iZRes;

                        int iCRes = -1;
                        Thread threadC = new Thread(() =>
                        {
                            try
                            {
                                iCRes = cylinder.CylinderUp(0);
                            }
                            catch (Exception ex)
                            {
                                if ("气缸上超时" == ex.Message)
                                    iCRes = -2;
                                else if ("阻挡气缸收回超时" == ex.Message)
                                    iCRes = -3;
                            }

                        });

                        threadC.Start();


                        threadC.Join();

                        if (-1 == iCRes)
                            return -1;
                        else if (-2 == iCRes)
                            throw new Exception("气缸上超时");
                        else if (-3 == iCRes)
                            throw new Exception("阻挡气缸收回超时");
                    }
                    else
                    {
                        int iCRes = -1;
                        Thread threadC = new Thread(() =>
                        {
                            try
                            {
                                iCRes = cylinder.CylinderUp(0);
                            }
                            catch (Exception ex)
                            {
                                if ("气缸上超时" == ex.Message)
                                    iCRes = -2;
                                else if ("阻挡气缸收回超时" == ex.Message)
                                    iCRes = -3;
                            }

                        });

                        threadC.Start();
                        iZRes = CardObject.OA1Axis.Absolute_Z(syringeType, z + SmartColor.My_ConPar.Other.Z_BackPulse, 0);
                        if (0 != iZRes)
                            return iZRes;

                        threadC.Join();

                        if (-1 == iCRes)
                            return -1;
                        else if (-2 == iCRes)
                            throw new Exception("气缸上超时");
                        else if (-3 == iCRes)
                            throw new Exception("阻挡气缸收回超时");
                    }

                    if (-1 == tray.Tray_On())
                        return -1;
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
                    else if (ex.Message == "未发现杯盖")
                    {
                        return 2702;
                    }
                    else if (ex.Message == "配液杯取盖失败")
                    {
                        return 8703;
                    }
                    else if (ex.Message == "关盖失败")
                    {
                        return 2705;
                    }
                    else if (ex.Message == "二次关盖复压失败")
                    {
                        return 2707;
                    }
                    else if (ex.Message == "二次关盖失败")
                    {
                        return 2708;
                    }
                    else if (ex.Message == "二次关盖未发现杯盖")
                    {
                        return 2709;
                    }
                    else if (ex.Message == "撑盖确认时气缸下失败")
                    {
                        return 5402;
                    }
                    else if (ex.Message == "撑盖确认时撑盖开失败")
                    {
                        return 5401;
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
