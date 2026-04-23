using Lib_Card.Base;
using SmartColor.My_Tool;
using System;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace Lib_Card.ADT8940A1.Axis
{
    public class Axis_Condition : Axis
    {
        public static bool isLoopSpeakActive = false;

        public override int Absolute_X(int iCylinderVersion, int iPulse, int iReserve)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            //是否存在光幕遮挡
            bool b_ha = false;
            /* 条件：
             *     1：X轴矢能已通
             *     2：X轴未运行
             *     3：X轴未报警
             *     4：X轴正限位未通
             *     5：X轴反限位未通
             *     6：光幕A未遮挡
             *     7：光幕B未遮挡
             *     8：急停未通
             *     9：气缸在上限位
             *     10：暂停无信号
             *     11：退出无信号
             * 
             */

            //如果正限位接通，需要向反向移动时，先联动移出限位
            int iXCorotation1 = CardObject.OA1Input.InPutStatus(boaed.InPut_X_Corotation);
            if (-1 == iXCorotation1)
                return -1;
            else if (1 == iXCorotation1)
            {
                int iResPos = 0;
                if (-1 == CardObject.OA1.ReadAxisActualPosition(boaed.Axis_X, ref iResPos))
                    return -1;
                if (iPulse < iResPos)
                {
                    OutPut.X_Power.X_Power xpower1 = new OutPut.X_Power.X_Power_Condition();
                    if (-1 == xpower1.X_Power_On())
                    {
                        return -1;
                    }

                    ADT8940A1_Card.MoveArg s_MoveArgGo = new Card.MoveArg()
                    {
                        Pulse = -10000,
                        LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_X_LSpeed,
                        HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_X_HSpeed,
                        Time = 1
                    };

                    int iResX = Lib_Card.CardObject.OA1Axis.Relative_X(iCylinderVersion, s_MoveArgGo, 0);
                    if (-1 == iResX)
                        throw new Exception("驱动异常");
                }
            }
            //如果反限位接通，需要向反向移动时，先联动移出限位
            int iXReverse1 = CardObject.OA1Input.InPutStatus(boaed.InPut_X_Reverse);
            if (-1 == iXReverse1)
                return -1;
            else if (1 == iXReverse1)
            {
                int iResPos = 0;

                if (-1 == CardObject.OA1.ReadAxisActualPosition(boaed.Axis_X, ref iResPos))
                    return -1;
                if (iPulse > iResPos)
                {
                    OutPut.X_Power.X_Power xpower1 = new OutPut.X_Power.X_Power_Condition();
                    if (-1 == xpower1.X_Power_On())
                    {
                        return -1;
                    }
                    ADT8940A1_Card.MoveArg s_MoveArgGo = new Card.MoveArg()
                    {
                        Pulse = 10000,
                        LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_X_LSpeed,
                        HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_X_HSpeed,
                        Time = 1
                    };

                    int iResX = Lib_Card.CardObject.OA1Axis.Relative_X(iCylinderVersion, s_MoveArgGo, 0);
                    if (-1 == iResX)
                        throw new Exception("驱动异常");
                }
            }


        lable:
            ADT8940A1_Card.MoveArg s_MoveArg = new Card.MoveArg()
            {
                Pulse = iPulse,
                LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_X_LSpeed,
                HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_X_HSpeed,
                Time = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_X_USpeed
            };
            //if (SmartColor.My_ConPar.Other.ActualPosition == 1)
            {
                int iResPos = 0;

                if (-1 == CardObject.OA1.ReadAxisActualPosition(boaed.Axis_X, ref iResPos))
                    return -1;

                if (-1 == CardObject.OA1.SetAxisCommandPosition(boaed.Axis_X, iResPos))
                    return -1;
            }

            while (true)
            {
                Thread.Sleep(1);
                int iXPowerStatus = CardObject.OA1.ReadOutPut(boaed.OutPut_X_Power);
                if (-1 == iXPowerStatus)
                    return -1;
                else if (0 == iXPowerStatus)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_X))
                        return -1;

                    OutPut.X_Power.X_Power xpower = new OutPut.X_Power.X_Power_Condition();
                    if (-1 == xpower.X_Power_On())
                    {
                        return -1;
                    }

                    continue;
                }

                int iXAlarm = CardObject.OA1Input.InPutStatus(boaed.InPut_X_Alarm);
                if (-1 == iXAlarm)
                    return -1;
                else if (1 == iXAlarm)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_X))
                        return -1;
                    throw new Exception("X轴伺服器报警");
                }

                int iXCorotation = CardObject.OA1Input.InPutStatus(boaed.InPut_X_Corotation);
                if (-1 == iXCorotation)
                    return -1;
                else if (1 == iXCorotation)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_X))
                        return -1;
                    throw new Exception("X轴正限位已通");
                }

                int iXReverse = CardObject.OA1Input.InPutStatus(boaed.InPut_X_Reverse);
                if (-1 == iXReverse)
                    return -1;
                else if (1 == iXReverse)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_X))
                        return -1;
                    throw new Exception("X轴反限位已通");
                }
                //if (SmartColor.My_ConPar.Other.ActualPosition == 1)
                {
                    int iXReady = CardObject.OA1Input.InPutStatus(boaed.InPut_X_Ready);
                    if (-1 == iXReady)
                        return -1;
                    else if (0 == iXReady)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                            return -1;
                        throw new Exception("X轴准备信号未接通");
                    }
                }
                
                int iSunxA = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_A));
                if (-1 == iSunxA)
                    return -1;
                else if (1 == iSunxA)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(35);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }

                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                        return -1;

                    continue;
                }

                int iSunxB = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_B));
                if (-1 == iSunxB)
                    return -1;
                else if (1 == iSunxB)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(36);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                        return -1;

                    continue;
                }

                

                int iPositionNowY = 0;
                int iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Y, ref iPositionNowY);
                if (-1 == iPositionRes)
                    return -1;

                int iStop = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_Stop));
                if (-1 == iStop)
                    return -1;
                else if (1 == iStop)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(34);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }

                    if (iPositionNowY > SmartColor.My_ConPar.Other.Sunx_Back_Y)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                            return -1;

                        continue;
                    }
                }
                if (!b_ha)
                {
                    if (isLoopSpeakActive)
                    {
                        _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                        isLoopSpeakActive = false;
                    }
                }


                if (Axis_Paused)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                        return -1;

                    continue;

                }

                if (Axis_Exit)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                        return -1;
                    return -2;
                }


                int iCylinderUp = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
                if (-1 == iCylinderUp)
                    return -1;
                int iCylinderDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
                if (-1 == iCylinderDown)
                    return -1;
                
                if (0 == iCylinderUp||1== iCylinderDown)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                        return -1;

                    OutPut.Cylinder.Cylinder cylinder;
                    if (0 == iCylinderVersion)
                        cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
                    else
                        cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();

                    if (-1 == cylinder.CylinderUp(0))
                        return -1;

                    continue;

                }

                //int iCylinderDown = CardObject.OA1Input.InPutStatus(plc.InPut_Cylinder_Down);
                //if (-1 == iCylinderDown)
                //    return -1;
                //else if (1 == iCylinderDown)
                //{
                //    if (-1 == CardObject.OA1.DecStop(plc.Axis_X))
                //        return -1;
                //    string s = CardObject.InsertD("气缸下已接通，请检查，确定没有接通请点是，退出运行请点否", " Absolute_X");
                //    while (true)
                //    {
                //        Thread.Sleep(1);
                //        if (Lib_Card.CardObject.keyValuePairs[s].Choose != 0)
                //            break;

                //    }
                //    int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s].Choose;
                //    CardObject.DeleteD(s);

                //    if (Alarm_Choose == 1)
                //    {
                //        goto lable;
                //    }
                //    else
                //    {
                //        throw new Exception("气缸下已接通");
                //    }

                //}

                //if (SmartColor.My_ConPar.Hardware.Machine_Decompression == 1)
                //{
                //    int iDecompression = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Decompression_Up));
                //    if (-1 == iDecompression)
                //        return -1;
                //    int iDecompressionDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Decompression_Down));
                //    if (-1 == iDecompressionDown)
                //        return -1;
                //    if (0 == iDecompression || 1 == iDecompressionDown)
                //    {
                //        if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                //            return -1;

                //        OutPut.Decompression.Decompression decompression = new OutPut.Decompression.Decompression_Condition();
                //        if (-1 == decompression.Decompression_Up())
                //            return -1;

                //        continue;
                //    }
                //}

                //if (SmartColor.My_ConPar.Hardware.Machine_Decompression == 2)
                //{

                //    int iDecompression_Right = CardObject.OA1Input.InPutStatus(plc.InPut_Decompression_Up_Right);
                //    if (-1 == iDecompression_Right)
                //        return -1;
                //    else if (0 == iDecompression_Right)
                //    {
                //        if (-1 == CardObject.OA1.DecStop(plc.Axis_X))
                //            return -1;

                //        OutPut.Decompression.Decompression decompression_Right = new OutPut.Decompression.Decompression_Condition();
                //        if (-1 == decompression_Right.Decompression_Up_Right())
                //            return -1;
                //    }
                //}

                int iPositionNowX = 0;
                iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_X, ref iPositionNowX);
                if (-1 == iPositionRes)
                    return -1;



                int iXStatus = CardObject.OA1.ReadAxisStatus(boaed.Axis_X);
                if (-1 == iXStatus)
                    return -1;
                else if (0 == iXStatus)
                {
                    if (iPositionNowX == s_MoveArg.Pulse)
                    {
                        if (isLoopSpeakActive)
                        {
                            _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                            isLoopSpeakActive = false;
                        }
                        return 0;
                    }

                    if (-1 == CardObject.OA1.AbsoluteMove(boaed.Axis_X, s_MoveArg))
                        return -1;
                }
                //Thread.Sleep(1);

                //if (-1 == CardObject.OA1.SuddnStop(plc.Axis_X))
                //    return -1;
                //Thread.Sleep(1);
            }
        }

        public override int Absolute_Y(int iCylinderVersion, int iPulse, int iReserve)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            /* 条件：
            *     1：Y轴矢能已通
            *     2：Y轴未运行
            *     3：Y轴未报警
            *     4：Y轴正限位未通
            *     5：Y轴反限位未通
            *     6：光幕A未遮挡
            *     7：光幕B未遮挡
            *     8：急停未通
            *     9：气缸在上限位
            *     10：暂停无信号
            *     11：退出无信号
            * 
            */

            //如果正限位接通，需要向反向移动时，先联动移出限位
            int iYCorotation1 = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Corotation);
            if (-1 == iYCorotation1)
                return -1;
            else if (1 == iYCorotation1)
            {
                int iResPos = 0;
                if (-1 == CardObject.OA1.ReadAxisActualPosition(boaed.Axis_Y, ref iResPos))
                    return -1;
                if (iPulse < iResPos)
                {
                    OutPut.Y_Power.Y_Power ypower1 = new OutPut.Y_Power.Y_Power_Condition();
                    if (-1 == ypower1.Y_Power_On())
                    {
                        return -1;
                    }
                    ADT8940A1_Card.MoveArg s_MoveArgGo = new Card.MoveArg()
                    {
                        Pulse = -10000,
                        LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_X_LSpeed,
                        HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_X_HSpeed,
                        Time = 1
                    };

                    int iResY = Lib_Card.CardObject.OA1Axis.Relative_Y(iCylinderVersion, s_MoveArgGo, 0);
                    if (-1 == iResY)
                        throw new Exception("驱动异常");
                }
            }
            //如果反限位接通，需要向反向移动时，先联动移出限位
            int iYReverse1 = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Reverse);
            if (-1 == iYReverse1)
                return -1;
            else if (1 == iYReverse1)
            {
                int iResPos = 0;
                if (-1 == CardObject.OA1.ReadAxisActualPosition(boaed.Axis_Y, ref iResPos))
                    return -1;
                if (iPulse > iResPos)
                {
                    OutPut.Y_Power.Y_Power ypower1 = new OutPut.Y_Power.Y_Power_Condition();
                    if (-1 == ypower1.Y_Power_On())
                    {
                        return -1;
                    }
                    ADT8940A1_Card.MoveArg s_MoveArgGo = new Card.MoveArg()
                    {
                        Pulse = 10000,
                        LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_X_LSpeed,
                        HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_X_HSpeed,
                        Time = 1
                    };

                    int iResY = Lib_Card.CardObject.OA1Axis.Relative_Y(iCylinderVersion, s_MoveArgGo, 0);
                    if (-1 == iResY)
                        throw new Exception("驱动异常");
                }
            }

        lable:

            ADT8940A1_Card.MoveArg s_MoveArg = new Card.MoveArg()
            {
                Pulse = iPulse,
                LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_Y_LSpeed,
                HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_Y_HSpeed,
                Time = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_Y_USpeed
            };
            //if (SmartColor.My_ConPar.Other.ActualPosition == 1)
            {
                int iResPos = 0;
                if (-1 == CardObject.OA1.ReadAxisActualPosition(boaed.Axis_Y, ref iResPos))
                    return -1;

                if (-1 == CardObject.OA1.SetAxisCommandPosition(boaed.Axis_Y, iResPos))
                    return -1;
            }

            int iYAT = 0;
            while (true)
            {
                Thread.Sleep(1);
                int iYPowerStatus = CardObject.OA1.ReadOutPut(boaed.OutPut_Y_Power);
                if (-1 == iYPowerStatus)
                    return -1;
                else if (0 == iYPowerStatus)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_Y))
                        return -1;

                    OutPut.Y_Power.Y_Power ypower = new OutPut.Y_Power.Y_Power_Condition();
                    if (-1 == ypower.Y_Power_On())
                    {
                        return -1;
                    }

                    continue;
                }

                int iYAlarm = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Alarm);
                if (-1 == iYAlarm)
                    return -1;
                else if (1 == iYAlarm)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_Y))
                        return -1;
                    if (++iYAT > 5)
                    {
                        throw new Exception("Y轴伺服器报警");
                    }
                }
                else
                {
                    iYAT = 0;
                }

                int iYCorotation = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Corotation);
                if (-1 == iYCorotation)
                    return -1;
                else if (1 == iYCorotation)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_Y))
                        return -1;
                    throw new Exception("Y轴正限位已通");
                }

                int iYReverse = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Reverse);
                if (-1 == iYReverse)
                    return -1;
                else if (1 == iYReverse)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_Y))
                        return -1;
                    throw new Exception("Y轴反限位已通");
                }
                //if (SmartColor.My_ConPar.Other.ActualPosition == 1)
                {
                    int iYReady = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Ready);
                    if (-1 == iYReady)
                        return -1;
                    else if (0 == iYReady)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                            return -1;
                        throw new Exception("Y轴准备信号未接通");
                    }
                }
                bool b_ha = false;
                int iSunxA = CardObject.OA1Input.InPutStatus(Convert.ToInt32( boaed.InPut_Sunx_A));
                if (-1 == iSunxA)
                    return -1;
                else if (1 == iSunxA)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(35);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                        return -1;

                    continue;
                }

                int iSunxB = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_B));
                if (-1 == iSunxB)
                    return -1;
                else if (1 == iSunxB)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(36);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                        return -1;

                    continue;
                }

                int iPositionNowY = 0;
                int iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Y, ref iPositionNowY);
                if (-1 == iPositionRes)
                    return -1;

                int iStop = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_Stop));
                if (-1 == iStop)
                    return -1;
                else if (1 == iStop)
                {

                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(34);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (s_MoveArg.Pulse == iPulse)
                    {

                        if (iPulse > iPositionNowY)
                        {
                            //前移
                            if (iPulse > SmartColor.My_ConPar.Other.Sunx_Back_Y)
                            {
                                if (iPulse != SmartColor.My_ConPar.Other.Sunx_Back_Y)
                                {
                                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                                        return -1;
                                    iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Y, ref iPositionNowY);
                                    if (-1 == iPositionRes)
                                        return -1;
                                    if (iPositionNowY < SmartColor.My_ConPar.Other.Sunx_Back_Y)
                                    {
                                        //在母液区

                                        s_MoveArg.Pulse = SmartColor.My_ConPar.Other.Sunx_Back_Y;

                                    }
                                    continue;
                                }
                                else
                                {


                                }
                            }

                        }
                        else if (iPulse < iPositionNowY)
                        {
                            //后移
                            if (iPositionNowY > SmartColor.My_ConPar.Other.Sunx_Back_Y)
                            {
                                //在配液区
                                if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                                    return -1;

                                continue;
                            }
                            else
                            {

                            }


                        }
                    }


                }
                else
                {
                    s_MoveArg.Pulse = iPulse;
                }

                if (!b_ha)
                {
                    if (isLoopSpeakActive)
                    {
                        _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                        isLoopSpeakActive = false;
                    }
                }

                if (Axis_Paused)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                        return -1;

                    continue;

                }

                if (Axis_Exit)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                        return -1;
                    return -2;
                }

                int iCylinderUp = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
                if (-1 == iCylinderUp)
                    return -1;
                int iCylinderDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
                if (-1 == iCylinderDown)
                    return -1;
                if (0 == iCylinderUp||1== iCylinderDown)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                        return -1;

                    OutPut.Cylinder.Cylinder cylinder;
                    if (0 == iCylinderVersion)
                        cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
                    else
                        cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();

                    if (-1 == cylinder.CylinderUp(0))
                        return -1;

                    continue;

                }

                //int iCylinderDown = CardObject.OA1Input.InPutStatus(plc.InPut_Cylinder_Down);
                //if (-1 == iCylinderDown)
                //    return -1;
                //else if (1 == iCylinderDown)
                //{
                //    if (-1 == CardObject.OA1.DecStop(plc.Axis_X))
                //        return -1;
                //    string s = CardObject.InsertD("气缸下已接通，请检查，确定没有接通请点是，退出运行请点否", " Absolute_Y");
                //    while (true)
                //    {
                //        Thread.Sleep(1);
                //        if (Lib_Card.CardObject.keyValuePairs[s].Choose != 0)
                //            break;

                //    }
                //    int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s].Choose;
                //    CardObject.DeleteD(s);
                //    if (Alarm_Choose == 1)
                //    {
                //        goto lable;
                //    }
                //    else
                //    {
                //        throw new Exception("气缸下已接通");
                //    }

                //}

                //if (SmartColor.My_ConPar.Hardware.Machine_Decompression == 1)
                //{
                //    int iDecompression = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Decompression_Up));
                //    if (-1 == iDecompression)
                //        return -1;
                //    int iDecompressionDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Decompression_Down));
                //    if (-1 == iDecompressionDown)
                //        return -1;
                //    if (0 == iDecompression || 1 == iDecompressionDown)
                //    {
                //        if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                //            return -1;

                //        OutPut.Decompression.Decompression decompression = new OutPut.Decompression.Decompression_Condition();
                //        if (-1 == decompression.Decompression_Up())
                //            return -1;
                //        continue;
                //    }
                //}

                //if (SmartColor.My_ConPar.Hardware.Machine_Decompression == 2)
                //{

                //    int iDecompression_Right = CardObject.OA1Input.InPutStatus(plc.InPut_Decompression_Up_Right);
                //    if (-1 == iDecompression_Right)
                //        return -1;
                //    else if (0 == iDecompression_Right)
                //    {
                //        if (-1 == CardObject.OA1.DecStop(plc.Axis_X))
                //            return -1;

                //        OutPut.Decompression.Decompression decompression_Right = new OutPut.Decompression.Decompression_Condition();
                //        if (-1 == decompression_Right.Decompression_Up_Right())
                //            return -1;
                //    }
                //}



                int iYStatus = CardObject.OA1.ReadAxisStatus(boaed.Axis_Y);
                if (-1 == iYStatus)
                    return -1;
                else if (0 == iYStatus)
                {
                    if (iPositionNowY == iPulse)
                    {

                        if (isLoopSpeakActive)
                        {
                            _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                            isLoopSpeakActive = false;
                        }
                        return 0;
                    }

                    if (-1 == CardObject.OA1.AbsoluteMove(boaed.Axis_Y, s_MoveArg))
                        return -1;
                }


            }
        }

        public override int Absolute_Z(int iType, int iPulse, int iReserve)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            /* 条件：
             *     1：Z轴反限位未通
             *     2：Z轴未运行
             *     3：急停未通
             *     4：抓手A关闭
             *     5：抓手B关闭
             *     6：接液盘收回
             *     7：暂停无信号
             *     8：退出无信号
             * 
             */






            ADT8940A1_Card.MoveArg s_MoveArg;

            if (0 == iType)
            {
                if (iPulse > SmartColor.My_ConPar.Other.S_MaxPulse - SmartColor.My_ConPar.Other.Z_BackPulse + /*(SmartDyeing.FADM_Object.Communal._b_isDripReserveFirst ? 5000 : 0)*/0)
                    throw new Exception("脉冲计算异常：" + iPulse + " > " + (SmartColor.My_ConPar.Other.S_MaxPulse - SmartColor.My_ConPar.Other.Z_BackPulse + /*(SmartDyeing.FADM_Object.Communal._b_isDripReserveFirst ? 5000 : 0)*/0));

                s_MoveArg = new Card.MoveArg()
                {
                    Pulse = iPulse,
                    LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_S_LSpeed,
                    HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_S_HSpeed,
                    Time = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_S_USpeed
                };
            }
            else
            {
                if (iPulse > SmartColor.My_ConPar.Other.B_MaxPulse - SmartColor.My_ConPar.Other.Z_BackPulse + /*(SmartDyeing.FADM_Object.Communal._b_isDripReserveFirst ? 5000 : 0)*/0)
                    throw new Exception("脉冲计算异常：" + iPulse + " > " + (SmartColor.My_ConPar.Other.B_MaxPulse - SmartColor.My_ConPar.Other.Z_BackPulse + /*(SmartDyeing.FADM_Object.Communal._b_isDripReserveFirst ? 5000 : 0)*/0));

                s_MoveArg = new Card.MoveArg()
                {
                    Pulse = iPulse,
                    LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_B_LSpeed,
                    HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_B_HSpeed,
                    Time = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_B_USpeed
                };
            }


            while (true)
            {
                Thread.Sleep(1);
                int iZCorotation = CardObject.OA1Input.InPutStatus(boaed.InPut_Z_Origin);
                if (-1 == iZCorotation)
                    return -1;
                else if (1 == iZCorotation && iPulse <= 0)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_Z))
                        return -1;
                    throw new Exception("Z轴反限位已通");
                }
                //else if (1 == iZCorotation)
                //{
                //    //第二次抓针筒时不报警
                //    return 0;
                //}

                //脉冲为正数时，方向向下时才报警，方向向上就不报警
                else if (1 == iZCorotation)
                {
                    int iPositionNowZ1 = 0;
                    int iPositionRes1 = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Z, ref iPositionNowZ1);
                    if (-1 == iPositionRes1)
                        return -1;
                    if (iPulse <= iPositionNowZ1)
                        throw new Exception("Z轴反限位已通");
                }

                int iPositionNowY = 0;
                int iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Y, ref iPositionNowY);
                if (-1 == iPositionRes)
                    return -1;

                int iStop = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_Stop));
                if (-1 == iStop)
                    return -1;
                else if (1 == iStop)
                {

                    if (iPositionNowY > SmartColor.My_ConPar.Other.Sunx_Back_Y)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_Z))
                            return -1;

                        continue;
                    }
                }

                if (Axis_Paused)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Z))
                        return -1;

                    continue;

                }

                if (Axis_Exit)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Z))
                        return -1;
                    return -2;
                }

                int iTongsA = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tongs_A));
                if (-1 == iTongsA)
                    return -1;
                else if (1 == iTongsA)
                {
                    OutPut.Tongs.Tongs tongs = new OutPut.Tongs.Tongs_Condition();
                    if (-1 == tongs.Tongs_On())
                        return -1;
                    continue;
                }

                int iTongsB = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tongs_B));
                if (-1 == iTongsB)
                    return -1;
                else if (1 == iTongsB)
                {
                    OutPut.Tongs.Tongs tongs = new OutPut.Tongs.Tongs_Condition();
                    if (-1 == tongs.Tongs_On())
                        return -1;
                    continue;
                }

                int iTray = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tray_In));
                if (-1 == iTray)
                    return -1; 
                //int iTrayOut = CardObject.OA1Input.InPutStatus(plc.InPut_Tray_Out);
                //if (-1 == iTrayOut)
                    //return -1;
                if (0 == iTray/*||1== iTrayOut*/)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Z))
                        return -1;
                    //无条件调用接液盘回
                    OutPut.Tray.Tray tray = new OutPut.Tray.Tray_Basic();
                    if (-1 == tray.Tray_Off())
                        return -1;
                }

                int iPositionNow = 0;
                iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Z, ref iPositionNow);
                if (-1 == iPositionRes)
                    return -1;

                int iZStatus = CardObject.OA1.ReadAxisStatus(boaed.Axis_Z);
                if (-1 == iZStatus)
                    return -1;
                else if (0 == iZStatus)
                {
                    if (iPositionNow == s_MoveArg.Pulse)
                        return 0;
                    if (-1 == CardObject.OA1.AbsoluteMove(boaed.Axis_Z, s_MoveArg))
                        return -1;
                }


            }
        }

        public override int Home_X(int iCylinderVersion, int iReserve)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            //是否存在光幕遮挡
            bool b_ha = false;
        /* 条件：
         *     1：X轴矢能已通
         *     2：X轴未运行
         *     3：X轴未报警
         *     4：X轴正限位未通
         *     5：光幕A未遮挡
         *     6：光幕B未遮挡
         *     7：急停未通
         *     8：气缸在上限位
         *     9：暂停无信号
         * 
         */




        lable:
            ADT8940A1_Card.HomeArg s_HomeArg = new Card.HomeArg()
            {
                Home_LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_X_LSpeed,
                Home_HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_X_HSpeed,
                Home_USpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_X_USpeed,
                Home_CSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_X_CSpeed,
                Home_Offset = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_X_Offset
            };

            int iHomeStatus = -3;
            while (true)
            {
                Thread.Sleep(1);
                int iXPowerStatus = CardObject.OA1.ReadOutPut(boaed.OutPut_X_Power);
                if (-1 == iXPowerStatus)
                    return -1;
                else if (0 == iXPowerStatus)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_X))
                        return -1;

                    throw new Exception("X轴矢能未接通");
                }

                int iXAlarm = CardObject.OA1Input.InPutStatus(boaed.InPut_X_Alarm);
                if (-1 == iXAlarm)
                    return -1;
                else if (1 == iXAlarm)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_X))
                        return -1;
                    throw new Exception("X轴伺服器报警");
                }

                int iXCorotation = CardObject.OA1Input.InPutStatus(boaed.InPut_X_Corotation);
                if (-1 == iXCorotation)
                    return -1;
                else if (1 == iXCorotation)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_X))
                        return -1;
                    throw new Exception("X轴正限位已通");
                }
                //if (SmartColor.My_ConPar.Other.ActualPosition == 1)
                {
                    int iXReady = CardObject.OA1Input.InPutStatus(boaed.InPut_X_Ready);
                    if (-1 == iXReady)
                        return -1;
                    else if (0 == iXReady)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                            return -1;
                        throw new Exception("X轴准备信号未接通");
                    }
                }

                int iSunxA = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_A));
                if (-1 == iSunxA)
                    return -1;
                else if (1 == iSunxA)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(35);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                        return -1;

                    continue;
                }

                int iSunxB = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_B));
                if (-1 == iSunxB)
                    return -1;
                else if (1 == iSunxB)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(36);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                        return -1;

                    continue;
                }

                int iPositionNowY = 0;
                int iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Y, ref iPositionNowY);
                if (-1 == iPositionRes)
                    return -1;

                int iStop = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_Stop));
                if (-1 == iStop)
                    return -1;
                else if (1 == iStop)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(34);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (iPositionNowY > SmartColor.My_ConPar.Other.Sunx_Back_Y)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                            return -1;

                        continue;
                    }
                }
                if (!b_ha)
                {
                    if (isLoopSpeakActive)
                    {
                        _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                        isLoopSpeakActive = false;
                    }
                }

                int iCylinderUp = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
                if (-1 == iCylinderUp)
                    return -1;
                int iCylinderDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
                if (-1 == iCylinderDown)
                    return -1;

                if (0 == iCylinderUp||1== iCylinderDown)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                        return -1;

                    OutPut.Cylinder.Cylinder cylinder;
                    if (0 == iCylinderVersion)
                        cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
                    else
                        cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();

                    if (-1 == cylinder.CylinderUp(0))
                        return -1;

                    continue;
                }

                //int iCylinderDown = CardObject.OA1Input.InPutStatus(plc.InPut_Cylinder_Down);
                //if (-1 == iCylinderDown)
                //    return -1;
                //else if (1 == iCylinderDown)
                //{
                //    if (-1 == CardObject.OA1.DecStop(plc.Axis_X))
                //        return -1;
                //    string s = CardObject.InsertD("气缸下已接通，请检查，确定没有接通请点是，退出运行请点否", " Home_X");
                //    while (true)
                //    {
                //        Thread.Sleep(1);
                //        if (Lib_Card.CardObject.keyValuePairs[s].Choose != 0)
                //            break;

                //    }
                //    int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s].Choose;
                //    CardObject.DeleteD(s);
                //    if (Alarm_Choose == 1)
                //    {
                //        goto lable;
                //    }
                //    else
                //    {
                //        throw new Exception("气缸下已接通");
                //    }

                //}

                //if (SmartColor.My_ConPar.Hardware.Machine_Decompression == 1)
                //{
                //    int iDecompression = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Decompression_Up));
                //    if (-1 == iDecompression)
                //        return -1;
                //    int iDecompressionDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Decompression_Down));
                //    if (-1 == iDecompressionDown)
                //        return -1;

                //    if (0 == iDecompression || 1 == iDecompressionDown)
                //    {
                //        if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                //            return -1;

                //        OutPut.Decompression.Decompression decompression = new OutPut.Decompression.Decompression_Condition();
                //        if (-1 == decompression.Decompression_Up())
                //            return -1;

                //        continue;
                //    }
                //}

                //if (SmartColor.My_ConPar.Hardware.Machine_Decompression == 2)
                //{

                //    int iDecompression_Right = CardObject.OA1Input.InPutStatus(plc.InPut_Decompression_Up_Right);
                //    if (-1 == iDecompression_Right)
                //        return -1;
                //    else if (0 == iDecompression_Right)
                //    {
                //        if (-1 == CardObject.OA1.DecStop(plc.Axis_X))
                //            return -1;

                //        OutPut.Decompression.Decompression decompression_Right = new OutPut.Decompression.Decompression_Condition();
                //        if (-1 == decompression_Right.Decompression_Up_Right())
                //            return -1;
                //    }
                //}

                int iXStatus = CardObject.OA1.ReadAxisStatus(boaed.Axis_X);
                if (-1 == iXStatus)
                    return -1;
                else if (0 == iXStatus)
                {
                    if (-3 == iHomeStatus)
                    {
                        if (-1 == CardObject.OA1.SetHomeMode(boaed.Axis_X, 0, s_HomeArg.Home_Offset))
                            return -1;

                        if (-1 == CardObject.OA1.SetHomeSpeed(boaed.Axis_X, s_HomeArg))
                            return -1;

                        if (-1 == CardObject.OA1.Home(boaed.Axis_X))
                            return -1;
                    }
                }

                iHomeStatus = CardObject.OA1.GetHomeStatus(boaed.Axis_X);

                if (0 == iHomeStatus && 0 == iXStatus)
                {
                    if (isLoopSpeakActive)
                    {
                        _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                        isLoopSpeakActive = false;
                    }
                    return 0;
                }


            }



        }

        public override int Home_Y(int iCylinderVersion, int iReserve)
        {
            //是否存在光幕遮挡
            bool b_ha = false;
            /* 条件：
             *     1：Y轴矢能已通
             *     2：Y轴未运行
             *     3：Y轴未报警
             *     4：Y轴正限位未通
             *     5：光幕A未遮挡
             *     6：光幕B未遮挡
             *     7：急停未通
             *     8：气缸在上限位
             *     9：暂停无信号
             * 
             */

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;

        lable:
            ADT8940A1_Card.HomeArg s_HomeArg = new Card.HomeArg()
            {
                Home_LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Y_LSpeed,
                Home_HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Y_HSpeed,
                Home_USpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Y_USpeed,
                Home_CSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Y_CSpeed,
                Home_Offset = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Y_Offset
            };

            int iHomeStatus = -3;
            int iYAT = 0;
            while (true)
            {
                Thread.Sleep(1);
                int iYPowerStatus = CardObject.OA1.ReadOutPut(boaed.OutPut_Y_Power);
                if (-1 == iYPowerStatus)
                    return -1;
                else if (0 == iYPowerStatus)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_Y))
                        return -1;

                    throw new Exception("Y轴矢能未接通");
                }

                int iYAlarm = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Alarm);
                if (-1 == iYAlarm)
                    return -1;
                else if (1 == iYAlarm)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_Y))
                        return -1;
                    if (++iYAT > 5)
                    {
                        throw new Exception("Y轴伺服器报警");
                    }
                }
                else
                {
                    iYAT = 0;
                }

                int iYCorotation = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Corotation);
                if (-1 == iYCorotation)
                    return -1;
                else if (1 == iYCorotation)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_Y))
                        return -1;
                    throw new Exception("Y轴正限位已通");
                }
                //if (SmartColor.My_ConPar.Other.ActualPosition == 1)
                {
                    int iYReady = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Ready);
                    if (-1 == iYReady)
                        return -1;
                    else if (0 == iYReady)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                            return -1;
                        throw new Exception("Y轴准备信号未接通");
                    }
                }

                int iSunxA = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_A));
                if (-1 == iSunxA)
                    return -1;
                else if (1 == iSunxA)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(35);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                        return -1;

                    continue;
                }

                int iSunxB = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_B));
                if (-1 == iSunxB)
                    return -1;
                else if (1 == iSunxB)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(36);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                        return -1;

                    continue;
                }

                int iPositionNowY = 0;
                int iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Y, ref iPositionNowY);
                if (-1 == iPositionRes)
                    return -1;

                int iStop = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_Stop));
                if (-1 == iStop)
                    return -1;
                else if (1 == iStop)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(34);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (iPositionNowY > SmartColor.My_ConPar.Other.Sunx_Back_Y)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                            return -1;

                        continue;
                    }
                }

                if (!b_ha)
                {
                    if (isLoopSpeakActive)
                    {
                        _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                        isLoopSpeakActive = false;
                    }
                }

                int iCylinderUp = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
                if (-1 == iCylinderUp)
                    return -1;
                int iCylinderDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
                if (-1 == iCylinderDown)
                    return -1;
                if (0 == iCylinderUp||1== iCylinderDown)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                        return -1;

                    OutPut.Cylinder.Cylinder cylinder;
                    if (0 == iCylinderVersion)
                        cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
                    else
                        cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();

                    if (-1 == cylinder.CylinderUp(0))
                        return -1;

                    continue;

                }

                //int iCylinderDown = CardObject.OA1Input.InPutStatus(plc.InPut_Cylinder_Down);
                //if (-1 == iCylinderDown)
                //    return -1;
                //else if (1 == iCylinderDown)
                //{
                //    if (-1 == CardObject.OA1.DecStop(plc.Axis_X))
                //        return -1;
                //    string s = CardObject.InsertD("气缸下已接通，请检查，确定没有接通请点是，退出运行请点否", " Home_Y");
                //    while (true)
                //    {
                //        Thread.Sleep(1);
                //        if (Lib_Card.CardObject.keyValuePairs[s].Choose != 0)
                //            break;

                //    }
                //    int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s].Choose;
                //    CardObject.DeleteD(s);
                //    if (Alarm_Choose == 1)
                //    {
                //        goto lable;
                //    }
                //    else
                //    {
                //        throw new Exception("气缸下已接通");
                //    }

                //}
                //if (SmartColor.My_ConPar.Hardware.Machine_Decompression == 1)
                //{
                //    int iDecompression = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Decompression_Up));
                //    if (-1 == iDecompression)
                //        return -1;
                //    int iDecompressionDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Decompression_Down));
                //    if (-1 == iDecompressionDown)
                //        return -1;
                //    if (0 == iDecompression || 1 == iDecompressionDown)
                //    {
                //        if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                //            return -1;

                //        OutPut.Decompression.Decompression decompression = new OutPut.Decompression.Decompression_Condition();
                //        if (-1 == decompression.Decompression_Up())
                //            return -1;

                //        continue;
                //    }
                //}

                //if (SmartColor.My_ConPar.Hardware.Machine_Decompression == 2)
                //{

                //    int iDecompression_Right = CardObject.OA1Input.InPutStatus(plc.InPut_Decompression_Up_Right);
                //    if (-1 == iDecompression_Right)
                //        return -1;
                //    else if (0 == iDecompression_Right)
                //    {
                //        if (-1 == CardObject.OA1.DecStop(plc.Axis_X))
                //            return -1;

                //        OutPut.Decompression.Decompression decompression_Right = new OutPut.Decompression.Decompression_Condition();
                //        if (-1 == decompression_Right.Decompression_Up_Right())
                //            return -1;
                //    }
                //}

                int iYStatus = CardObject.OA1.ReadAxisStatus(boaed.Axis_Y);
                if (-1 == iYStatus)
                    return -1;
                else if (0 == iYStatus)
                {
                    if (-3 == iHomeStatus)
                    {
                        if (-1 == CardObject.OA1.SetHomeMode(boaed.Axis_Y, 0, s_HomeArg.Home_Offset))
                            return -1;

                        if (-1 == CardObject.OA1.SetHomeSpeed(boaed.Axis_Y, s_HomeArg))
                            return -1;

                        if (-1 == CardObject.OA1.Home(boaed.Axis_Y))
                            return -1;
                    }
                }

                iHomeStatus = CardObject.OA1.GetHomeStatus(boaed.Axis_Y);

                if (0 == iHomeStatus && 0 == iYStatus)
                {
                    if (isLoopSpeakActive)
                    {
                        _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                        isLoopSpeakActive = false;
                    }
                    return 0;
                }



            }



        }

        public override int Home_Z(int iCylinderVersion, int iReserve)
        {
            /* 条件：
             *     1：Z轴未运行
             *     2：急停未通
             *     3：抓手A关闭
             *     4：抓手B关闭
             *     5：气缸在上限位
             * 
             */

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;

            ADT8940A1_Card.HomeArg s_HomeArg = new Card.HomeArg()
            {
                Home_LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Z_LSpeed,
                Home_HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Z_HSpeed,
                Home_USpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Z_USpeed,
                Home_CSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Z_CSpeed,
                Home_Offset = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Z_Offset
            };

            int iHomeStatus = -3;
            while (true)
            {
                Thread.Sleep(1);
                int iCylinderUp = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
                if (-1 == iCylinderUp)
                    return -1;
                int iCylinderDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
                if (-1 == iCylinderDown)
                    return -1;

                if (0 == iCylinderUp || 1 == iCylinderDown)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                        return -1;

                    OutPut.Cylinder.Cylinder cylinder;
                    if (0 == iCylinderVersion)
                        cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
                    else
                        cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();

                    if (-1 == cylinder.CylinderUp(0))
                        return -1;
                    continue;

                }

                int iTongsA = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tongs_A));
                if (-1 == iTongsA)
                    return -1;
                else if (1 == iTongsA)
                {
                    OutPut.Tongs.Tongs tongs = new OutPut.Tongs.Tongs_Condition();
                    if (-1 == tongs.Tongs_On())
                        return -1;
                    continue;
                }

                int iTongsB = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Tongs_B));
                if (-1 == iTongsB)
                    return -1;
                else if (1 == iTongsB)
                {
                    OutPut.Tongs.Tongs tongs = new OutPut.Tongs.Tongs_Condition();
                    if (-1 == tongs.Tongs_On())
                        return -1;
                    continue;
                }

                int iPositionNowY = 0;
                int iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Y, ref iPositionNowY);
                if (-1 == iPositionRes)
                    return -1;

                int iStop = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_Stop));
                if (-1 == iStop)
                    return -1;
                else if (1 == iStop)
                {

                    if (iPositionNowY > SmartColor.My_ConPar.Other.Sunx_Back_Y)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_Z))
                            return -1;

                        continue;
                    }
                }

                


                int iZStatus = CardObject.OA1.ReadAxisStatus(boaed.Axis_Z);
                if (-1 == iZStatus)
                    return -1;
                else if (0 == iZStatus)
                {
                    if (-3 == iHomeStatus)
                    {
                        if (-1 == CardObject.OA1.SetHomeMode(boaed.Axis_Z, 0, s_HomeArg.Home_Offset))
                            return -1;

                        if (-1 == CardObject.OA1.SetHomeSpeed(boaed.Axis_Z, s_HomeArg))
                            return -1;

                        if (-1 == CardObject.OA1.Home(boaed.Axis_Z))
                            return -1;
                    }
                }

                iHomeStatus = CardObject.OA1.GetHomeStatus(boaed.Axis_Z);

                if (0 == iHomeStatus && 0 == iZStatus)
                    return 0;


            }
        }

        public override int Relative_X(int iCylinderVersion, Card.MoveArg s_MoveArg, int iReserve)
        {
            //是否存在光幕遮挡
            bool b_ha = false;
            /* 条件：
             *     1：X轴矢能已通
             *     2：X轴未运行
             *     3：X轴未报警
             *     4：X轴正限位未通
             *     5：X轴反限位未通
             *     6：光幕A未遮挡
             *     7：光幕B未遮挡
             *     8：急停未通
             *     9：气缸在上限位
             * 
             */

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;


        lable:
            int iPulse = s_MoveArg.Pulse;
            int iPositionBegin = 0;
            int iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_X, ref iPositionBegin);
            if (-1 == iPositionRes)
                return -1;

            while (true)
            {
                Thread.Sleep(1);
                int iXPowerStatus = CardObject.OA1.ReadOutPut(boaed.OutPut_X_Power);
                if (-1 == iXPowerStatus)
                    return -1;
                else if (0 == iXPowerStatus)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_X))
                        return -1;

                    throw new Exception("X轴矢能未接通");
                }

                int iXAlarm = CardObject.OA1Input.InPutStatus(boaed.InPut_X_Alarm);
                if (-1 == iXAlarm)
                    return -1;
                else if (1 == iXAlarm)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_X))
                        return -1;
                    throw new Exception("X轴伺服器报警");
                }

                int iXCorotation = CardObject.OA1Input.InPutStatus(boaed.InPut_X_Corotation);
                if (-1 == iXCorotation)
                    return -1;
                else if (1 == iXCorotation)
                {
                    if (s_MoveArg.Pulse > 0)
                    {
                        if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_X))
                            return -1;
                        throw new Exception("X轴正限位已通");
                    }
                }

                int iXReverse = CardObject.OA1Input.InPutStatus(boaed.InPut_X_Reverse);
                if (-1 == iXReverse)
                    return -1;
                else if (1 == iXReverse)
                {
                    if (s_MoveArg.Pulse < 0)
                    {
                        if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_X))
                            return -1;
                        throw new Exception("X轴反限位已通");
                    }
                }
                //if (SmartColor.My_ConPar.Other.ActualPosition == 1)
                {
                    int iXReady = CardObject.OA1Input.InPutStatus(boaed.InPut_X_Ready);
                    if (-1 == iXReady)
                        return -1;
                    else if (0 == iXReady)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                            return -1;
                        throw new Exception("X轴准备信号未接通");
                    }
                }

                int iSunxA = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_A));
                if (-1 == iSunxA)
                    return -1;
                else if (1 == iSunxA)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(35);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                        return -1;

                    continue;
                }

                int iSunxB = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_B));
                if (-1 == iSunxB)
                    return -1;
                else if (1 == iSunxB)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(36);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                        return -1;

                    continue;
                }

                int iPositionNowY = 0;
                iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Y, ref iPositionNowY);
                if (-1 == iPositionRes)
                    return -1;

                int iStop = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_Stop));
                if (-1 == iStop)
                    return -1;
                else if (1 == iStop)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(34);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (iPositionNowY > SmartColor.My_ConPar.Other.Sunx_Back_Y)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                            return -1;

                        continue;
                    }
                }

                if (!b_ha)
                {
                    if (isLoopSpeakActive)
                    {
                        _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                        isLoopSpeakActive = false;
                    }
                }

                int iCylinderUp = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
                if (-1 == iCylinderUp)
                    return -1;
                int iCylinderDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
                if (-1 == iCylinderDown)
                    return -1;
                
                if (0 == iCylinderUp||1== iCylinderDown)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                        return -1;

                    OutPut.Cylinder.Cylinder cylinder;
                    if (0 == iCylinderVersion)
                        cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
                    else
                        cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();

                    if (-1 == cylinder.CylinderUp(0))
                        return -1;
                    continue;

                }

                //int iCylinderDown = CardObject.OA1Input.InPutStatus(plc.InPut_Cylinder_Down);
                //if (-1 == iCylinderDown)
                //    return -1;
                //else if (1 == iCylinderDown)
                //{
                //    if (-1 == CardObject.OA1.DecStop(plc.Axis_X))
                //        return -1;
                //    string s = CardObject.InsertD("气缸下已接通，请检查，确定没有接通请点是，退出运行请点否", " Relative_X");
                //    while (true)
                //    {
                //        Thread.Sleep(1);
                //        if (Lib_Card.CardObject.keyValuePairs[s].Choose != 0)
                //            break;

                //    }
                //    int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s].Choose;
                //    CardObject.DeleteD(s);
                //    if (Alarm_Choose == 1)
                //    {
                //        goto lable;
                //    }
                //    else
                //    {
                //        throw new Exception("气缸下已接通");
                //    }

                //}

                //if (SmartColor.My_ConPar.Hardware.Machine_Decompression == 1)
                //{
                //    int iDecompression = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Decompression_Up));
                //    if (-1 == iDecompression)
                //        return -1;
                //    int iDecompressionDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Decompression_Down));
                //    if (-1 == iDecompressionDown)
                //        return -1;

                //    if (0 == iDecompression || 1 == iDecompressionDown)
                //    {
                //        if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                //            return -1;

                //        OutPut.Decompression.Decompression decompression = new OutPut.Decompression.Decompression_Condition();
                //        if (-1 == decompression.Decompression_Up())
                //            return -1;
                //        continue;
                //    }
                //}

                //if (SmartColor.My_ConPar.Hardware.Machine_Decompression == 2)
                //{

                //    int iDecompression_Right = CardObject.OA1Input.InPutStatus(plc.InPut_Decompression_Up_Right);
                //    if (-1 == iDecompression_Right)
                //        return -1;
                //    else if (0 == iDecompression_Right)
                //    {
                //        if (-1 == CardObject.OA1.DecStop(plc.Axis_X))
                //            return -1;

                //        OutPut.Decompression.Decompression decompression_Right = new OutPut.Decompression.Decompression_Condition();
                //        if (-1 == decompression_Right.Decompression_Up_Right())
                //            return -1;
                //    }
                //}

                int iPositionNow = 0;
                iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_X, ref iPositionNow);
                if (-1 == iPositionRes)
                    return -1;

                int iXStatus = CardObject.OA1.ReadAxisStatus(boaed.Axis_X);
                if (-1 == iXStatus)
                    return -1;
                else if (0 == iXStatus)
                {
                    s_MoveArg.Pulse = iPulse - (iPositionNow - iPositionBegin);

                    if (0 == s_MoveArg.Pulse)
                    {
                        if (isLoopSpeakActive)
                        {
                            _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                            isLoopSpeakActive = false;
                        }
                        return 0;
                    }
                    if (-1 == CardObject.OA1.RelativeMove(boaed.Axis_X, s_MoveArg))
                        return -1;

                }


            }
        }

        public override int Relative_Y(int iCylinderVersion, Card.MoveArg s_MoveArg, int iReserve)
        {
            //是否存在光幕遮挡
            bool b_ha = false;
            /* 条件：
             *     1：Y轴矢能已通
             *     2：Y轴未运行
             *     3：Y轴未报警
             *     4：Y轴正限位未通
             *     5：Y轴反限位未通
             *     6：光幕A未遮挡
             *     7：光幕B未遮挡
             *     8：急停未通
             *     9：气缸在上限位
             * 
             */


            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;

        lable:
            int iPulse = s_MoveArg.Pulse;
            int iPositionBegin = 0;
            int iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Y, ref iPositionBegin);
            if (-1 == iPositionRes)
                return -1;
            int iYAT = 0;
            while (true)
            {
                Thread.Sleep(1);
                int iYPowerStatus = CardObject.OA1.ReadOutPut(boaed.OutPut_Y_Power);
                if (-1 == iYPowerStatus)
                    return -1;
                else if (0 == iYPowerStatus)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_Y))
                        return -1;

                    throw new Exception("Y轴矢能未接通");
                }

                int iYAlarm = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Alarm);
                if (-1 == iYAlarm)
                    return -1;
                else if (1 == iYAlarm)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_Y))
                        return -1;
                    if (++iYAT > 5)
                    {
                        throw new Exception("Y轴伺服器报警");
                    }
                }
                else
                {
                    iYAT = 0;
                }

                int iYCorotation = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Corotation);
                if (-1 == iYCorotation)
                    return -1;
                else if (1 == iYCorotation)
                {
                    if (s_MoveArg.Pulse > 0)
                    {
                        if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_Y))
                            return -1;
                        throw new Exception("Y轴正限位已通");
                    }
                }
                int iYReverse = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Reverse);
                if (-1 == iYReverse)
                    return -1;
                else if (1 == iYReverse)
                {
                    if (s_MoveArg.Pulse < 0)
                    {
                        if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_Y))
                            return -1;
                        throw new Exception("Y轴反限位已通");
                    }
                }
                //if (SmartColor.My_ConPar.Other.ActualPosition == 1)
                {
                    int iYReady = CardObject.OA1Input.InPutStatus(boaed.InPut_Y_Ready);
                    if (-1 == iYReady)
                        return -1;
                    else if (0 == iYReady)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                            return -1;
                        throw new Exception("Y轴准备信号未接通");
                    }
                }

                int iSunxA = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_A));
                if (-1 == iSunxA)
                    return -1;
                else if (1 == iSunxA)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(35);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                        return -1;

                    continue;
                }

                int iSunxB = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_B));
                if (-1 == iSunxB)
                    return -1;
                else if (1 == iSunxB)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(36);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                        return -1;

                    continue;
                }

                int iPositionNowY = 0;
                iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Y, ref iPositionNowY);
                if (-1 == iPositionRes)
                    return -1;

                int iStop = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_Stop));
                if (-1 == iStop)
                    return -1;
                else if (1 == iStop)
                {
                    b_ha = true;
                    if (!isLoopSpeakActive)
                    {
                        var alarm = RobotAlarmOverview.GetAlarmInfo(34);
                        // 只在首次进入时触发
                        _ = Task.Run(() => MessageEventManager.Instance.RequestLoopSpeak("光幕遮挡", alarm.Description));
                        isLoopSpeakActive = true;
                    }
                    if (iPositionNowY > SmartColor.My_ConPar.Other.Sunx_Back_Y)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_Y))
                            return -1;

                        continue;
                    }
                }
                if (!b_ha)
                {
                    if (isLoopSpeakActive)
                    {
                        _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                        isLoopSpeakActive = false;
                    }
                }
                int iCylinderUp = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Up));
                if (-1 == iCylinderUp)
                    return -1;
                int iCylinderDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Cylinder_Down));
                if (-1 == iCylinderDown)
                    return -1;

                if (0 == iCylinderUp || 1 == iCylinderDown)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                        return -1;

                    OutPut.Cylinder.Cylinder cylinder;
                    if (0 == iCylinderVersion)
                        cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
                    else
                        cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();

                    if (-1 == cylinder.CylinderUp(0))
                        return -1;
                    continue;

                }

                //int iCylinderDown = CardObject.OA1Input.InPutStatus(plc.InPut_Cylinder_Down);
                //if (-1 == iCylinderDown)
                //    return -1;
                //else if (1 == iCylinderDown)
                //{
                //    if (-1 == CardObject.OA1.DecStop(plc.Axis_X))
                //        return -1;
                //    string s = CardObject.InsertD("气缸下已接通，请检查，确定没有接通请点是，退出运行请点否", " Relative_Y");
                //    while (true)
                //    {
                //        Thread.Sleep(1);
                //        if (Lib_Card.CardObject.keyValuePairs[s].Choose != 0)
                //            break;

                //    }
                //    int Alarm_Choose = Lib_Card.CardObject.keyValuePairs[s].Choose;
                //    CardObject.DeleteD(s);
                //    if (Alarm_Choose == 1)
                //    {
                //        goto lable;
                //    }
                //    else
                //    {
                //        throw new Exception("气缸下已接通");
                //    }

                //}
                //if (SmartColor.My_ConPar.Hardware.Machine_Decompression == 1)
                //{
                //    int iDecompression = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Decompression_Up));
                //    if (-1 == iDecompression)
                //        return -1;
                //    int iDecompressionDown = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Decompression_Down));
                //    if (-1 == iDecompressionDown)
                //        return -1;

                //    if (0 == iDecompression || 1 == iDecompressionDown)
                //    {
                //        if (-1 == CardObject.OA1.DecStop(boaed.Axis_X))
                //            return -1;

                //        OutPut.Decompression.Decompression decompression = new OutPut.Decompression.Decompression_Condition();
                //        if (-1 == decompression.Decompression_Up())
                //            return -1;
                //        continue;
                //    }
                //}
                //if (SmartColor.My_ConPar.Hardware.Machine_Decompression == 2)
                //{

                //    int iDecompression_Right = CardObject.OA1Input.InPutStatus(plc.InPut_Decompression_Up_Right);
                //    if (-1 == iDecompression_Right)
                //        return -1;
                //    else if (0 == iDecompression_Right)
                //    {
                //        if (-1 == CardObject.OA1.DecStop(plc.Axis_X))
                //            return -1;

                //        OutPut.Decompression.Decompression decompression_Right = new OutPut.Decompression.Decompression_Condition();
                //        if (-1 == decompression_Right.Decompression_Up_Right())
                //            return -1;
                //    }
                //}

                int iPositionNow = 0;
                iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Y, ref iPositionNow);
                if (-1 == iPositionRes)
                    return -1;

                int iYStatus = CardObject.OA1.ReadAxisStatus(boaed.Axis_Y);
                if (-1 == iYStatus)
                    return -1;
                else if (0 == iYStatus)
                {
                    s_MoveArg.Pulse = iPulse - (iPositionNow - iPositionBegin);

                    if (0 == s_MoveArg.Pulse)
                    {
                        if (isLoopSpeakActive)
                        {
                            _ = Task.Run(() => MessageEventManager.Instance.RequestStopLoopSpeak("光幕遮挡"));
                            isLoopSpeakActive = false;
                        }
                        return 0;
                    }
                    if (-1 == CardObject.OA1.RelativeMove(boaed.Axis_Y, s_MoveArg))
                        return -1;

                }


            }
        }

        public override int Relative_Z(Card.MoveArg s_MoveArg, int iReserve)
        {
            /* 条件：
             *     1：Z轴反限位未通
             *     2：Z轴未运行
             *     3：急停未通
             * 
             */



            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;

            int iPulse = s_MoveArg.Pulse;
            int iPositionBegin = 0;
            int iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Z, ref iPositionBegin);
            if (-1 == iPositionRes)
                return -1;

            while (true)
            {
                Thread.Sleep(1);
                int iZCorotation = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Z_Origin));
                if (-1 == iZCorotation)
                    return -1;
                else if (1 == iZCorotation && iPulse < 0)
                {
                    if (-1 == CardObject.OA1.SuddnStop(boaed.Axis_Z))
                        return -1;
                    throw new Exception("Z轴反限位已通");
                }
                //else if (1 == iZCorotation)
                //{
                //    //第二次抓针筒时不报警
                //    return 0;
                //}

                int iPositionNowY = 0;
                iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Y, ref iPositionNowY);
                if (-1 == iPositionRes)
                    return -1;

                int iStop = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Sunx_Stop));
                if (-1 == iStop)
                    return -1;
                else if (1 == iStop)
                {

                    if (iPositionNowY > SmartColor.My_ConPar.Other.Sunx_Back_Y)
                    {
                        if (-1 == CardObject.OA1.DecStop(boaed.Axis_Z))
                            return -1;

                        continue;
                    }
                }

                if (Axis_Exit)
                {
                    if (-1 == CardObject.OA1.DecStop(boaed.Axis_Z))
                        return -1;
                    return -2;
                }

                int iPositionNow = 0;
                iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Z, ref iPositionNow);
                if (-1 == iPositionRes)
                    return -1;

                int iZStatus = CardObject.OA1.ReadAxisStatus(boaed.Axis_Z);
                if (-1 == iZStatus)
                    return -1;
                else if (0 == iZStatus)
                {
                    s_MoveArg.Pulse = iPulse - (iPositionNow - iPositionBegin);

                    if (0 == s_MoveArg.Pulse)
                        return 0;
                    if (-1 == CardObject.OA1.RelativeMove(boaed.Axis_Z, s_MoveArg))
                        return -1;
                }


            }
        }
    }
}
