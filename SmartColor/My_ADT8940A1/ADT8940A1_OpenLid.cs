using Lib_Card;
using Lib_Card.ADT8940A1.Module.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_ADT8940A1
{
    internal class ADT8940A1_OpenLid
    {
        /// <summary>
        /// 开盖
        /// </summary>
        /// <param name="lockSignal">锁止信号</param>
        /// <returns>动作完成/异常码</returns>
        public Task<int> OpenLidAsync()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            return Task.Run(() =>
            {
                try
                {
                    
                    int iRes = Lib_Card.CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Syringe));
                    if (SmartColor.My_ConPar.Choices.IgnoreSyringeSensor == 1)
                    {
                        iRes = 0;
                    }

                    if (1 == iRes)
                    {
                        //new FADM_Object.MyAlarm("请先拿住针筒点确定", "温馨提示",false,1);
                        //My_File.LocalTranslator.ShowMessage("请先拿住针筒点确定", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Question);

                        var tcs = new ManualResetEvent(false);
                        // 超过2次，提示检查密封圈
                        SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                            "温馨提示",
                            $"请先拿住针筒点确定",
                             btn =>
                             {

                                 tcs.Set();
                             },
                            new[] { "确定" },
                            "确定"
                        );
                        tcs.WaitOne(); // 等待用户操作
                        Lib_Card.ADT8940A1.OutPut.Tongs.Tongs tongs1 = new Lib_Card.ADT8940A1.OutPut.Tongs.Tongs_Basic();
                        tongs1.Tongs_Off();


                    }

                    Lib_Card.ADT8940A1.OutPut.Tray.Tray tray = new Lib_Card.ADT8940A1.OutPut.Tray.Tray_Condition();
                    if (-1 == tray.Tray_Off())
                        return -1;

                    Lib_Card.ADT8940A1.OutPut.Cylinder.Cylinder cylinder;
                    if (0 == SmartColor.My_ConPar.Hardware.CylinderType)
                        cylinder = new Lib_Card.ADT8940A1.OutPut.Cylinder.SingleControl.Cylinder_Condition();
                    else
                        cylinder = new Lib_Card.ADT8940A1.OutPut.Cylinder.DualControl.Cylinder_Condition();
                    int iType = 0;
                    if (iType == 0)
                    {
                        ////如果有撑盖部件，就在开盖时，下不到位也去夹盖子
                        //if (SmartColor.My_ConPar.Hardware.Tongs_Decompression == 1)
                        //{
                        //    if (-1 == cylinder.CylinderDown(1))
                        //        return -1;
                        //    //气缸下不到位，返回-9就直接继续进行
                        //}
                        //else
                        //{
                        //    if (-1 == cylinder.CylinderDown(0))
                        //        return -1;
                        //}

                        int i_ret = 0;
                        i_ret = cylinder.CylinderDown(3);
                        //气缸下不到位
                        if(i_ret == -7)
                        {
                            throw new Exception("配液杯取盖失败");
                        }
                        else if (i_ret == -1)
                        {
                            return -1;
                        }
                    }
                   


                    Lib_Card.ADT8940A1.OutPut.Tongs.Tongs tongs = new Lib_Card.ADT8940A1.OutPut.Tongs.Tongs_Basic();
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

                    //没发现杯盖，再重新下去取一次
                    if (bDelay)
                    {
                        if (-1 == tongs.Tongs_Off())
                            return -1;

                        if (-1 == cylinder.CylinderUp(0))
                            return -1;

                        int i_ret = 0;
                        i_ret = cylinder.CylinderDown(3);
                        //气缸下不到位
                        if (i_ret == -7)
                        {
                            throw new Exception("配液杯取盖失败");
                        }
                        else if (i_ret == -1)
                        {
                            return -1;
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

                        if (bDelay)
                        {
                            if (-1 == tongs.Tongs_Off())
                                return -1;
                            if (-1 == cylinder.CylinderUp(0))
                                return -1;

                            Lib_Card.ADT8940A1.Module.Home.Home home = new Lib_Card.ADT8940A1.Module.Home.Home_Condition();
                            if (-1 == home.Home_Z(SmartColor.My_ConPar.Hardware.CylinderType))
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

                            throw new Exception("配液杯取盖失败");

                        }
                    }
                    //发现杯盖
                    else
                    {
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

                            throw new Exception("配液杯取盖失败");

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
