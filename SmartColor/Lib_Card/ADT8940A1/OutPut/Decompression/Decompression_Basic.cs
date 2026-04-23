using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib_Card.ADT8940A1.OutPut.Decompression
{
    public class Decompression_Basic : Decompression
    {
        public override int Decompression_Down()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Decompression), 1))
                return -1;
            return 0;
        }

        public override int Decompression_Up()
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Decompression), 0))
                return -1;
            return 0;
        }

        public override int Decompression_Down_Right()
        {
            //var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            //if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Decompression_Right), 1))
            //    return -1;
            return 0;
        }

        public override int Decompression_Up_Right()
        {
            //var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            //if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Decompression_Right), 0))
            //    return -1;
            return 0;
        }
    }
}
