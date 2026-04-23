using System;

namespace Lib_Card.ADT8940A1.OutPut.Water
{
    /// <summary>
    /// 无条件检查
    /// </summary>
    public class Water_Basic : Water
    {
        public override int Water_Off()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Water), 0))
                return -1;
            return 0;
        }

        public override int Water_On()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Water), 1))
                return -1;
            return 0;
        }
    }
}
