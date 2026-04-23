using Lib_Card;
using Lib_Card.ADT8940A1;
using Lib_Card.ADT8940A1.Module.Home;
using SmartColor.My_ConPar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lib_Card.ADT8940A1.Module
{
    public class GetOrPutClamp
    {
        /// <summary>
        /// 拿夹具
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <returns>0：正常；-1：异常；-2：收到退出消息</returns>
        public int GetClamp(int iCylinderVersion)
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
            if (-1 == cylinder.CylinderDown(0))
                return -1;


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


            Base.Card.MoveArg s_Movearg = new Base.Card.MoveArg();
            if (bDelay && 0 == iSyringe)
            {
                if (-1 == tongs.Tongs_Off())
                    return -1;


                s_Movearg.Pulse = -(SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Z_Offset;
                s_Movearg.LSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_S_LSpeed;
                s_Movearg.HSpeed = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_S_HSpeed;
                s_Movearg.Time = (SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Move_S_USpeed;

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

                    Home.Home home = new Home.Home_Condition();
                    if (-1 == home.Home_Z(iCylinderVersion))
                        throw new Exception("驱动异常");
                    throw new Exception("未发现抓手");
                }
            }



            return 0;
        }

        /// <summary>
        /// 放夹具
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <returns>0：正常；-1：异常；-2：收到退出消息</returns>
        public int PutClamp(int iCylinderVersion)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            bool bDelay = false;
            int iSyringe = -1;
            Thread threadS = new Thread(() =>
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
                throw new Exception("未发现抓手");
            }

            //读取z轴坐标，判断是否在零位
            try
            {
                int iZRes = CardObject.OA1Axis.Absolute_Z(0, 0, 0);
                if (0 != iZRes)
                    return iZRes;
            }
            catch (Exception ex)
            {
                if ("Z轴反限位已通" != ex.Message)
                    throw;
            }

            OutPut.Tray.Tray tray = new OutPut.Tray.Tray_Condition();
            if (-1 == tray.Tray_Off())
                return -1;

            OutPut.Cylinder.Cylinder cylinder;
            if (0 == iCylinderVersion)
                cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
            else
                cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();
            int res = cylinder.CylinderDown(0);
            OutPut.Tongs.Tongs tongs = new OutPut.Tongs.Tongs_Condition();
            if (-1 == res)
                return -1;

            //松开抓手
            if (-1 == tongs.Tongs_Off())
                return -1;
            //气缸上
            if (-1 == cylinder.CylinderUp(0))
                return -1;

            return 0;
        }
    }
}
