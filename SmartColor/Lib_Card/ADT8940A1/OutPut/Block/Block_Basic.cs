using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib_Card.ADT8940A1.OutPut.Block
{
    public class Block_Basic : Block
    {
        public override int Block_In()
        {
            if (SmartColor.My_ConPar.Hardware.Block == 0)
            {
                return 0;
            }

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Block_Out), 0))
                return -1;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Block_In), 1))
                return -1;
            return 0;
        }

        public override int Block_Out()
        {
            if (SmartColor.My_ConPar.Hardware.Block == 0)
            {
                return 0;
            }

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Block_Out), 1))
                return -1;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Block_In), 0))
                return -1;
            return 0;
        }
    }
}
