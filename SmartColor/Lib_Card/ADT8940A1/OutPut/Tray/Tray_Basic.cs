using System;

namespace Lib_Card.ADT8940A1.OutPut.Tray
{
    /// <summary>
    /// 无条件检查
    /// </summary>
    public class Tray_Basic : Tray
    {
        public override int Tray_Off()
        {

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Tray), 0))
                return -1;
            return 0;
        }

        public override int Tray_On()
        {

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Tray), 1))
                return -1;
            return 0;
        }
    }
}
