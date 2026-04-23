using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lib_Card.ADT8940A1.Module
{
    public class WashSyringe
    {
        /// <summary>
        /// 洗针筒
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <returns>0：正常；-1：异常；-2：收到退出消息</returns>
        public int Wash(int iCylinderVersion, int iSyringeType)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;

            //接液盘收回
            OutPut.Tray.Tray tray = new OutPut.Tray.Tray_Condition();
            if (-1 == tray.Tray_Off())
                return -1;

            int iRes = -1;
            OutPut.Cylinder.Cylinder cylinder;
            if (0 == iCylinderVersion)
                cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
            else
                cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();

            //先洗外壁
            //打开洗针阀
            iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Wash_In), 1);
            if (-1 == iRes)
                return -1;
            iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Wash_Out), 1);
            if (-1 == iRes)
                return -1;
            //循环执行2次
            for (int i = 0; i < 2; i++)
            {
                
                    

                //气缸下
                if (-1 == cylinder.CylinderDown(0))
                    return -1;

                //气缸上
                if (-1 == cylinder.CylinderUp(0))
                    return -1;

                //第二步洗内壁
                //气缸下
                if (-1 == cylinder.CylinderDown(0))
                    return -1;
                //执行

                //抽拉到最大脉冲值
                int iZRes1 = CardObject.OA1Axis.Absolute_Z(iSyringeType, iSyringeType == 0 ? SmartColor.My_ConPar.Other.S_MaxPulse : SmartColor.My_ConPar.Other.B_MaxPulse, 0);
                if (0 != iZRes1)
                    return iZRes1;
                //反推到0脉冲值
                try
                {
                    iZRes1 = CardObject.OA1Axis.Absolute_Z(iSyringeType, 0, 0);
                    if (0 != iZRes1)
                        return iZRes1;
                }
                catch (Exception ex)
                {
                    if ("Z轴反限位已通" != ex.Message)
                        throw;
                }
                if (i == 1)
                {
                    //关闭洗针阀
                    iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Wash_In), 0);
                    if (-1 == iRes)
                        return -1;
                    iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Wash_Out), 0);
                    if (-1 == iRes)
                        return -1;
                }

                //气缸上
                if (-1 == cylinder.CylinderUp(0))
                    return -1;

                Thread.Sleep(5000);
            }

            

            

            //打开气缸下输出，关闭气缸上输出延时1秒

            iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Up), 0);
            if (-1 == iRes)
                return -1;
            iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Down), 1);
            if (-1 == iRes)
                return -1;

            Thread.Sleep(1000);
            //关闭气缸下输出
            iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Down), 0);
            if (-1 == iRes)
                return -1;

            //气缸向下偏移原点偏移量
            try
            {
                int iZRes1 = CardObject.OA1Axis.Absolute_Z(iSyringeType, -(SmartColor.My_ConPar.Object.CurrentMotion as SmartColor.My_ConPar.Type.BoaedCard.Motion).Home_Z_Offset, 0);
                if (0 != iZRes1)
                    return iZRes1;
            }
            catch (Exception ex)
            {
                if ("Z轴反限位已通" != ex.Message)
                    throw;
            }
            //回到原点位
            try
            {
                int iZRes1 = CardObject.OA1Axis.Absolute_Z(iSyringeType, 0, 0);
                if (0 != iZRes1)
                    return iZRes1;
            }
            catch (Exception ex)
            {
                if ("Z轴反限位已通" != ex.Message)
                    throw;
            }
            Thread.Sleep(1000);

            //气缸下
            if (-1 == cylinder.CylinderDown(0))
                return -1;

            //吹气打开
            iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Wash_Blow), 1);
            if (-1 == iRes)
                return -1;

            Thread.Sleep(3000);

            //气缸上
            if (-1 == cylinder.CylinderUp(0))
                return -1;

            //关闭吹气
            iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Wash_Blow), 0);
            if (-1 == iRes)
                return -1;

            

            //接液盘出
            if (-1 == tray.Tray_On())
                return -1;
            return 0;
        }
    }
}
