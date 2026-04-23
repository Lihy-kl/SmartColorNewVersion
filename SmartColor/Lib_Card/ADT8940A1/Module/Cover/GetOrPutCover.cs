using Lib_Card.ADT8940A1.Module.Home;
using Lib_Card.ADT8940A1.OutPut.Blender;
using Lib_Card.ADT8940A1;
using Lib_Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lib_Card.ADT8940A1.OutPut.Tongs;

namespace Lib_Card.ADT8940A1.Module
{
    public class GetOrPutCover
    {
        /// <summary>
        /// 拿盖
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <param name="iType">0:开盖 1：关盖</param>
        /// <returns>0：正常；-1：异常；-2：收到退出消息</returns>
        public int GetCover(int iCylinderVersion, int iType)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            OutPut.Tray.Tray tray = new OutPut.Tray.Tray_Condition();
            if (-1 == tray.Tray_Off())
                return -1;

            OutPut.Cylinder.Cylinder cylinder;
            if (0 == iCylinderVersion)
                cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
            else
                cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();
            if (iType == 0)
            {
                //如果有撑盖部件，就在开盖时，下不到位也去夹盖子
                if (SmartColor.My_ConPar.Hardware.Tongs_Decompression == 1)
                {
                    if (-1 == cylinder.CylinderDown(1))
                        return -1;
                    //气缸下不到位，返回-9就直接继续进行
                }
                else
                {
                    if (-1 == cylinder.CylinderDown(0))
                        return -1;
                }
            }
            else
            {
                if (-1 == cylinder.CylinderDown(0))
                    return -1;
            }


                OutPut.Tongs.Tongs tongs = new OutPut.Tongs.Tongs_Condition();
            if (-1 == tongs.Tongs_On())
                return -1;

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


            if (bDelay)
            {
                if (-1 == tongs.Tongs_Off())
                    return -1;

                if (-1 == cylinder.CylinderUp(0))
                    return -1;

                if (-1 == cylinder.CylinderDown(0))
                    return -1;

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
                    iSyringe = CardObject.OA1Input.InPutStatus(Convert.ToInt32( boaed.InPut_Syringe));
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

                if (bDelay)
                {
                    if (-1 == tongs.Tongs_Off())
                        return -1;
                    if (-1 == cylinder.CylinderUp(0))
                        return -1;

                    Home.Home home = new Home.Home_Condition();
                    if (-1 == home.Home_Z(iCylinderVersion))
                        throw new Exception("驱动异常");
                    throw new Exception("未发现杯盖");
                }
                int res = cylinder.CylinderUp(1);
                if (-1 == res)
                    return -1;
                else if (-9 == res)
                {
                    //拿着盖子升不到位，先气缸下，松开抓手，再升一次，看看是否因为气压不够导致
                    if (-1 == cylinder.CylinderDown(0))
                        return -1;
                    if (-1 == tongs.Tongs_Off())
                        return -1;

                    res = cylinder.CylinderUp(1);
                    if (-1 == res)
                        return -1;
                    else if (-9 == res)
                    {
                        //throw new Exception("气缸上超时");
                        cylinder.CylinderUp(0);
                    }
                    //松开杯盖可以正常升到位
                    if (iType == 1)
                    {
                        //杯盖区拿盖失败
                        throw new Exception("放盖区取盖失败");
                    }
                    else
                    {
                        throw new Exception("配液杯取盖失败");
                    }
                }


            }

            //获取盖子
            return 0;
        }

        /// <summary>
        /// 放盖
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <param name="iType">0:开盖 1：关盖</param>
        /// <param name="x">撑盖x坐标</param>
        /// <param name="y">撑盖y坐标</param>
        /// <returns>0：正常；-1：异常；-2：收到退出消息</returns>
        public int PutCover(int iCylinderVersion, int iType,int x,int y)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            //放盖子
            OutPut.Tray.Tray tray = new OutPut.Tray.Tray_Condition();
            if (-1 == tray.Tray_Off())
                return -1;

            OutPut.Cylinder.Cylinder cylinder;
            if (0 == iCylinderVersion)
                cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
            else
                cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();
            int res = cylinder.CylinderDown(1);
            OutPut.Tongs.Tongs tongs = new OutPut.Tongs.Tongs_Condition();
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
                    else
                    {
                        throw new Exception("放盖失败");
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
                //开盖直接气缸上
                if (iType == 0)
                {
                    if (-1 == cylinder.CylinderUp(0))
                        return -1;
                }
                //关盖时，离开下限位就重新气缸下
                else
                {
                    //气缸上
                    if (-1 == cylinder.CylinderUp(0))
                        return -1;
                    //移动到撑盖位置
                    Lib_Card.ADT8940A1.Module.Move.Move move = new Lib_Card.ADT8940A1.Module.Move.TargeMove();
                    int iMove = move.TargetMove(1, 0, x, y, 0);
                    if (-1 == iMove)
                        throw new Exception("驱动异常");
                    //关闭抓手，气缸下，判断是否到位
                    if (-1 == tongs.Tongs_On())
                        return -1;
                    res = cylinder.CylinderDown(0);
                    if (-1 == res)
                        return -1;
                    //到位后打开抓手，判断撑盖到位信号是否存在
                    OutPut.Tongs.Tongs tongsign = new OutPut.Tongs.Tongs_Basic();
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
                    string s1 = null, s2 = null;
                    while (true)
                    {
                        Thread.Sleep(1);
                        int iSupportCover = CardObject.OA1Input.InPutStatus(Convert.ToInt32( boaed.InPut_SupportCover));
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
                            throw new Exception("关盖失败");
                        }

                    }

                    //撑盖信号到位后，直接气缸上
                    if (-1 == cylinder.CylinderUp(0))
                        return -1;
                }
            }
            else
            {
                //开盖直接气缸上
                if (iType == 0)
                {
                    if (-1 == cylinder.CylinderUp(0))
                        return -1;
                }
                //关盖时，离开下限位就重新气缸下
                else
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

            return 0;
        }
    }
}
