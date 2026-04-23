using System;

namespace Lib_Card.ADT8940A1.OutPut.Tongs
{
    /// <summary>
    /// 无条件检查
    /// </summary>
    public class Tongs_Basic : Tongs
    {
        public override int Tongs_Off()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (SmartColor.My_ConPar.Hardware.TongType == 0)
            {
                if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_TongsOn), 0))
                    return -1;
            }
            else
            {
                if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_TongsOn), 0))
                    return -1;
                if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_TongsOff), 1))
                    return -1;
            }
            return 0;
        }

        public override int Tongs_On()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (SmartColor.My_ConPar.Hardware.TongType == 0)
            {
                if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_TongsOn), 1))
                    return -1;
            }
            else
            {
                if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_TongsOn), 1))
                    return -1;
                if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_TongsOff), 0))
                    return -1;
            }
            return 0;
        }
    }
}
