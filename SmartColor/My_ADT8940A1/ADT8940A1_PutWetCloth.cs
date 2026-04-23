using Lib_Card;
using Lib_Card.ADT8940A1.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartColor.My_ADT8940A1
{
    internal class ADT8940A1_PutWetCloth
    {
        /// <summary>
        /// 放湿布（异步方法）
        /// </summary>
        /// <returns>动作完成/异常码</returns>
        public Task<int> PutWetClothAsync()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;



            return Task.Run(() =>
            {
                try
                {

                    //阻挡出
                    Lib_Card.ADT8940A1.OutPut.Block.Block block = new Lib_Card.ADT8940A1.OutPut.Block.Block_Condition();
                    if (-1 == block.Block_Out())
                        return -1;

                    CylinderMo cylinderMo = new CylinderMo();
                    //气缸到阻挡位
                    if (-1 == cylinderMo.CylinderBlock(SmartColor.My_ConPar.Hardware.CylinderType))
                        return -1;

                    //开始放布
                    //Z轴移动
                    try
                    {
                        int iZRes = CardObject.OA1Axis.Absolute_Z(0, SmartColor.My_ConPar.Other.OpenPulse, 0);
                        if (-1 == iZRes)
                            return -1;
                    }
                    catch (Exception ex)
                    {
                        if ("Z轴反限位已通" != ex.Message)
                            throw;
                    }
                    //放布延时
                    Thread.Sleep(2000);

                    //废液回抽关闭
                    Lib_Card.ADT8940A1.OutPut.Waste.Waste waste = new Lib_Card.ADT8940A1.OutPut.Waste.Waste_Basic();
                    if (-1 == waste.Waste_Off())
                        return -1;


                    //气缸上
                    Lib_Card.ADT8940A1.OutPut.Cylinder.Cylinder cylinder;
                    if (0 == SmartColor.My_ConPar.Hardware.CylinderType)
                        cylinder = new Lib_Card.ADT8940A1.OutPut.Cylinder.SingleControl.Cylinder_Condition();
                    else
                        cylinder = new Lib_Card.ADT8940A1.OutPut.Cylinder.DualControl.Cylinder_Condition();
                    if (-1 == cylinder.CylinderUp(0))
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
                    else if (ex.Message == "未发现抓手")
                    {
                        return 4501;
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
