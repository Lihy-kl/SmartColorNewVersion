using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Lib_Card.ADT8940A1.OutPut.Apocenosis
{
    public class Apocenosis_Basic : Apocenosis
    {
        public override int Apocenosis_Off()
        {
            //var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            //if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Apocenosis), 0))
            //    return -1;
            return 0;
        }

        public override int Apocenosis_On()
        {
            //var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            //if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Apocenosis), 1))
            //    return -1;
            return 0;
        }
    }
}
