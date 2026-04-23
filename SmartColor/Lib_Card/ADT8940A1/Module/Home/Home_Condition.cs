using System;
using System.Threading;

namespace Lib_Card.ADT8940A1.Module.Home
{
    public class Home_Condition : Home
    {
        public override int Home_XYZ(int iCylinderVersion)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            OutPut.X_Power.X_Power xpower = new OutPut.X_Power.X_Power_Condition();
            if (-1 == xpower.X_Power_On())
                return -1;

            OutPut.Y_Power.Y_Power ypower = new OutPut.Y_Power.Y_Power_Condition();
            if (-1 == ypower.Y_Power_On())
                return -1;

           
            int iSyringe = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Syringe));
            if (SmartColor.My_ConPar.Choices.IgnoreSyringeSensor==1)
            {
                iSyringe = 0;
            }
            if (-1 == iSyringe)
                return -1;
            else if (1 == iSyringe)
                throw new Exception("发现针筒");

            if (0 == iCylinderVersion)
            {
                OutPut.Cylinder.Cylinder cylinder = new OutPut.Cylinder.SingleControl.Cylinder_Condition();
                if (-1 == cylinder.CylinderUp(0))
                    return -1;
            }
            else
            {
                OutPut.Cylinder.Cylinder cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();
                if (-1 == cylinder.CylinderUp(0))
                    return -1;
            }

            OutPut.Tongs.Tongs tongs = new OutPut.Tongs.Tongs_Condition();
            if (-1 == tongs.Tongs_On())
                return -1;

          

            int iXRes = -1;
            Thread threadx = new Thread(() =>
            {
                try
                {
                    iXRes = CardObject.OA1Axis.Home_X(iCylinderVersion, 0);
                }
                catch (Exception ex)
                {
                    if ("X轴矢能未接通" == ex.Message)
                        iXRes = -2;
                    if ("X轴伺服器报警" == ex.Message)
                        iXRes = -3;
                    if ("X轴正限位已通" == ex.Message)
                        iXRes = -4;
                }
            });
            threadx.Start();

            int iYRes = -1;
            Thread thready = new Thread(() =>
            {
                try
                {
                    iYRes = CardObject.OA1Axis.Home_Y(iCylinderVersion, 0);
                }
                catch (Exception ex)
                {
                    if ("Y轴矢能未接通" == ex.Message)
                        iYRes = -2;
                    if ("Y轴伺服器报警" == ex.Message)
                        iYRes = -3;
                    if ("Y轴正限位已通" == ex.Message)
                        iYRes = -4;
                }
            });
            thready.Start();

            int iZRes = CardObject.OA1Axis.Home_Z(iCylinderVersion, 0);
            if (-1 == iZRes)
                return -1;

            threadx.Join();
            if (-1 == iXRes)
                return -1;
            else if (-2 == iXRes)
                throw new Exception("X轴矢能未接通");
            else if (-3 == iXRes)
                throw new Exception("X轴伺服器报警");
            else if (-4 == iXRes)
                throw new Exception("X轴正限位已通");

            thready.Join();
            if (-1 == iYRes)
                return -1;
            else if (-2 == iYRes)
                throw new Exception("Y轴矢能未接通");
            else if (-3 == iYRes)
                throw new Exception("Y轴伺服器报警");
            else if (-4 == iYRes)
                throw new Exception("Y轴正限位已通");

            if (-1 == tongs.Tongs_Off())
                return -1;
            //if (SmartColor.My_ConPar.Other.ActualPosition == 1)
            {
                OutPut.X_Power.X_Power x_Power = new OutPut.X_Power.X_Power_Condition();
                if (-1 == x_Power.X_Power_Off())
                    return -1;

                OutPut.Y_Power.Y_Power y_Power = new OutPut.Y_Power.Y_Power_Condition();
                if (-1 == y_Power.Y_Power_Off())
                    return -1;
            }

            Thread.Sleep(500);
            if (-1 == CardObject.OA1.SetAxisActualPosition(Convert.ToInt32(boaed.Axis_X)))
                return -1;
            if (-1 == CardObject.OA1.SetAxisActualPosition(Convert.ToInt32(boaed.Axis_Y)))
                return -1;

            Home_XYZFinish = true;


            return 0;

        }

        public override int Home_Z(int iCylinderVersion)
        {
            OutPut.Tongs.Tongs tongs = new OutPut.Tongs.Tongs_Condition();
            if (-1 == tongs.Tongs_On())
                return -1;

           
            if (-1 == CardObject.OA1Axis.Home_Z(iCylinderVersion, 0))
                return -1;

            if (-1 == tongs.Tongs_Off())
                return -1;

            return 0;

        }
    }
}
