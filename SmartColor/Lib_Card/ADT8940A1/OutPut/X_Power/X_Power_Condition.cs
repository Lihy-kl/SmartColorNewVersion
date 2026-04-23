using System;
using System.Threading;

namespace Lib_Card.ADT8940A1.OutPut.X_Power
{
    /// <summary>
    /// 带条件检查
    /// </summary>
    public class X_Power_Condition : X_Power
    {
        public override int X_Power_Off()
        {
            /* 条件
             *     1：X轴未运行
             * 
             */

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;

            int iXStatus = CardObject.OA1.ReadAxisStatus(Convert.ToInt32(boaed.Axis_X));
            if (-1 == iXStatus)
                return -1;

            if (0 == iXStatus)
            {
                int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_X_Power), 0);
                if (-1 == iRes)
                    return -1;
                return 0;
            }
            else
            {
                throw new Exception("X轴正在运行");
            }
        }

        public override int X_Power_On()
        {
            /* 条件
             *     1：X轴未报警
             *     2：X轴未运行
             * 
             */


            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;

            int iXAlarm = CardObject.OA1Input.InPutStatus(Convert.ToInt32(boaed.InPut_X_Alarm));
            if (-1 == iXAlarm)
                return -1;

            int iXStatus = CardObject.OA1.ReadAxisStatus(Convert.ToInt32(boaed.Axis_X));
            if (-1 == iXStatus)
                return -1;

            if (0 == iXAlarm && 0 == iXStatus)
            {
                int iRes = CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_X_Power), 1);
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
                if (1 == iXAlarm)
                    throw new Exception("X轴伺服器报警");
                else
                    throw new Exception("X轴正在运行");
            }
        }
    }
}
