using System;
using System.Threading;

namespace Lib_Card.ADT8940A1.OutPut.Y_Power
{
    /// <summary>
    /// 无条件检查
    /// </summary>
    public class Y_Power_Basic : Y_Power
    {
        public override int Y_Power_Off()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Y_Power), 0))
                return -1;
            return 0;
        }

        public override int Y_Power_On()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Y_Power), 1))
                return -1;
            if (SmartColor.My_ConPar.Machine.MachineType == 1 && SmartColor.My_ConPar.Hardware.ServoType == 1)
            {
                Thread.Sleep(500);
            }
            return 0;
        }
    }
}
