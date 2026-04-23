using System;
using System.Threading;

namespace Lib_Card.ADT8940A1.OutPut.Y_Power
{
    /// <summary>
    /// 带条件检查
    /// </summary>
    public class Y_Power_Condition : Y_Power
    {
        public override int Y_Power_Off()
        {
            /* 条件
             *     1：Y轴未运行
             * 
             */

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;

            int iYStatus = CardObject.OA1.ReadAxisStatus(Convert.ToInt32(boaed.Axis_Y));
            if (-1 == iYStatus)
                return -1;

            if (0 == iYStatus)
            {
                int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Y_Power), 0);
                if (-1 == iRes)
                    return -1;
                return 0;
            }
            else
            {
                throw new Exception("Y轴正在运行");
            }
        }

        public override int Y_Power_On()
        {
            /* 条件
             *     1：Y轴未报警
             *     2：Y轴未运行
             * 
             */


            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;

            int iYAlarm = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_Y_Alarm));
            if (-1 == iYAlarm)
                return -1;

            int iYStatus = CardObject.OA1.ReadAxisStatus(Convert.ToInt32(boaed.Axis_Y));
            if (-1 == iYStatus)
                return -1;

            if (0 == iYAlarm && 0 == iYStatus)
            {
                int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Y_Power), 1);
                if (-1 == iRes)
                    return -1;
                if (SmartColor.My_ConPar.Machine.MachineType == 1 && SmartColor.My_ConPar.Hardware.ServoType == 1)
                {
                    Thread.Sleep(500);
                }
                return 0;
            }
            else
            {
                if (1 == iYAlarm)
                    throw new Exception("Y轴伺服器报警");
                else
                    throw new Exception("Y轴正在运行");
            }
        }
    }
}
