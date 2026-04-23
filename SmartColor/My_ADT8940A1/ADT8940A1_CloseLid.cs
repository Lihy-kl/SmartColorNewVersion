using Lib_Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartColor.My_ADT8940A1
{
    internal class ADT8940A1_CloseLid
    {
        /// <summary>
        /// 关盖
        /// </summary>
        /// <param name="lockSignal">锁止信号</param>
        /// <returns>动作完成/异常码</returns>
        public Task<int> CloseLidLidAsync()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;



            return Task.Run(() =>
            {
                //获取现有坐标用于计算撑盖坐标
                int iPositionNowY = 0;
                int iPositionRes = CardObject.OA1.ReadAxisCommandPosition(boaed.Axis_Y, ref iPositionNowY);
                if (-1 == iPositionRes)
                    return -1;

                int iPositionNowX = 0;

                if (-1 == CardObject.OA1.ReadAxisActualPosition(boaed.Axis_X, ref iPositionNowX))
                    return -1;

                int x, y;
                x = iPositionNowX;
                y = iPositionNowY + My_ConPar.Other.SupportCoverY;

                try
                {
                    int iType = 1;
                    //放盖子
                    Lib_Card.ADT8940A1.OutPut.Tray.Tray tray = new Lib_Card.ADT8940A1.OutPut.Tray.Tray_Condition();
                    if (-1 == tray.Tray_Off())
                        return -1;

                    Lib_Card.ADT8940A1.OutPut.Cylinder.Cylinder cylinder;
                    if (0 == SmartColor.My_ConPar.Hardware.CylinderType)
                        cylinder = new Lib_Card.ADT8940A1.OutPut.Cylinder.SingleControl.Cylinder_Condition();
                    else
                        cylinder = new Lib_Card.ADT8940A1.OutPut.Cylinder.DualControl.Cylinder_Condition();
                    int res = cylinder.CylinderDown(1);
                    Lib_Card.ADT8940A1.OutPut.Tongs.Tongs tongs = new Lib_Card.ADT8940A1.OutPut.Tongs.Tongs_Condition();
                    if (-1 == res)
                        return -1;
                    else if (-9 == res)
                    {

                        //第一次放盖子失败
                        if (-1 == cylinder.CylinderUp(0))
                            return -1;
                        res = cylinder.CylinderDown(1);
                        if (-1 == res)
                            return -1;
                        else if (-9 == res)
                        {
                            //第二次放盖子失败,松开抓手

                            if (-1 == tongs.Tongs_Off())
                                return -1;
                            if (iType == 1)
                            {
                                throw new Exception("关盖失败");
                            }
                        }

                    }

                    //如果关盖就要压2秒
                    if (iType == 1)
                        Thread.Sleep(2000);
                    if (-1 == tongs.Tongs_Off())
                        return -1;
                    //如果有撑盖部件
                    if (SmartColor.My_ConPar.Hardware.Tongs_Decompression == 1)
                    {

                        //关盖时，离开下限位就重新气缸下
                        //else
                        {
                            //气缸上
                            if (-1 == cylinder.CylinderUp(0))
                                return -1;
                            //移动到撑盖位置
                            Lib_Card.ADT8940A1.Module.Move.Move move = new Lib_Card.ADT8940A1.Module.Move.TargeMove();
                            int iMove = move.TargetMove(SmartColor.My_ConPar.Hardware.CylinderType, 0, x, y, 0);
                            if (-1 == iMove)
                                throw new Exception("驱动异常");

                            //关闭抓手，气缸下，判断是否到位
                            if (-1 == tongs.Tongs_On())
                                return -1;
                            res = cylinder.CylinderDown(1);
                            if (-1 == res)
                                return -1;
                            else if (-9 == res)
                            {
                                //撑盖位气缸下失败
                                //气缸上
                                if (-1 == cylinder.CylinderUp(0))
                                    return -1;
                                if (-1 == tongs.Tongs_Off())
                                    return -1;
                                throw new Exception("撑盖确认时气缸下失败");
                            }
                            //到位后打开抓手，判断撑盖到位信号是否存在
                            Lib_Card.ADT8940A1.OutPut.Tongs.Tongs tongsign = new Lib_Card.ADT8940A1.OutPut.Tongs.Tongs_Basic();
                            tongsign.Tongs_Off();
                            //等待延时是否打开正常
                            bool bDelay = false;
                            Thread thread = new Thread(() =>
                            {
                                int iDelay = Convert.ToInt32(SmartColor.My_ConPar.Delay.Tongs * 1000.00);
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
                            //string s1 = null, s2 = null;
                            while (true)
                            {
                                Thread.Sleep(1);
                                int iSupportCover = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_SupportCover));
                                if (-1 == iSupportCover)
                                    return -1;
                                if (1 == iSupportCover)
                                    break;

                                if (bDelay)
                                {
                                    //如果撑开不到位，直接报关盖失败
                                    if (-1 == tongs.Tongs_On())
                                        return -1;
                                    //气缸上
                                    if (-1 == cylinder.CylinderUp(0))
                                        return -1;
                                    if (-1 == tongs.Tongs_Off())
                                        return -1;
                                    throw new Exception("撑盖确认时撑盖开失败");
                                }

                            }

                            //撑盖信号到位后，直接气缸上
                            if (-1 == cylinder.CylinderUp(0))
                                return -1;
                        }
                    }
                    else
                    {
                        //关盖时，离开下限位就重新气缸下
                        //else
                        {
                            bool b_repress = false;
                        lab_repress:
                            //气缸上离开下限位
                            res = cylinder.CylinderUp(2);
                            if (res == -1)
                            {
                                return -1;
                            }
                            //重新气缸下，再怼一次确认
                            res = cylinder.CylinderDown(2);
                            if (res == -1)
                            {
                                return -1;
                            }
                            //气缸下不到位,重新关闭抓手
                            else if (res == -8)
                            {
                                if (b_repress)
                                {
                                    //正常气缸上
                                    res = cylinder.CylinderUp(0);
                                    if (res == -1)
                                    {
                                        return -1;
                                    }
                                    throw new Exception("二次关盖复压失败");
                                }
                                //第一次下怼确认不成功
                                else
                                {
                                    bool bfirst = true;
                                lab_again:
                                    try
                                    {
                                        if (-1 == tongs.Tongs_On())
                                            return -1;
                                    }
                                    catch (Exception e)
                                    {
                                        if (e.Message == "抓手A关闭超时" || e.Message == "抓手B关闭超时")
                                        {
                                            //重新打开抓手
                                            if (-1 == tongs.Tongs_Off())
                                                return -1;
                                            throw new Exception(e.Message);
                                        }
                                    }
                                    //判断针筒感应器是否有信号
                                    int iSyringe = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Syringe));
                                    if (SmartColor.My_ConPar.Choices.IgnoreSyringeSensor == 1)
                                    {
                                        iSyringe = 1;
                                    }
                                    if (-1 == iSyringe)
                                        return -1;
                                    else if (1 == iSyringe)
                                    {
                                        //正常气缸上
                                        res = cylinder.CylinderUp(0);
                                        if (res == -1)
                                        {
                                            return -1;
                                        }

                                        //正常气缸下,再关一次盖
                                        res = cylinder.CylinderDown(1);
                                        if (res == -1)
                                        {
                                            return -1;
                                        }
                                        else if (res == -9)
                                        {
                                            //重新打开抓手
                                            if (-1 == tongs.Tongs_Off())
                                                return -1;
                                            //正常气缸上
                                            res = cylinder.CylinderUp(0);
                                            if (res == -1)
                                            {
                                                return -1;
                                            }
                                            //直接报二次关盖失败
                                            throw new Exception("二次关盖失败");
                                        }
                                        else
                                        {
                                            //二次关盖成功,然后重新再怼一次
                                            if (!b_repress)
                                            {
                                                b_repress = true;
                                                goto lab_repress;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //重新再抓一次啊
                                        if (bfirst)
                                        {
                                            bfirst = false;

                                            //重新打开抓手
                                            if (-1 == tongs.Tongs_Off())
                                                return -1;

                                            goto lab_again;
                                        }
                                        //第二次抓取没有信号
                                        else
                                        {
                                            //重新打开抓手
                                            if (-1 == tongs.Tongs_Off())
                                                return -1;
                                            //正常气缸上
                                            res = cylinder.CylinderUp(0);
                                            if (res == -1)
                                            {
                                                return -1;
                                            }
                                            throw new Exception("二次关盖未发现杯盖");
                                        }
                                    }
                                }

                            }
                            //到位后重新气缸上
                            else if (res == 0)
                            {
                                if (-1 == cylinder.CylinderUp(0))
                                    return -1;
                            }
                        }
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
