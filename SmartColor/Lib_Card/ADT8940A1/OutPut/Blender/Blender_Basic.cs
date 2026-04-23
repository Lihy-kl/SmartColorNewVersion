using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Lib_Card.ADT8940A1.OutPut.Blender
{
    /// <summary>
    /// 无条件检查
    /// </summary>
    public class Blender_Basic : Blender
    {
        public override int Blender_Off()
        {

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (SmartColor.My_ConPar.Hardware.BlenderType == 1)
            {
                if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Blender), 0))
                    return -1;
            }
            else
            {
                if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Blender), 1))
                    return -1;
            }
            return 0;
        }

        public override int Blender_On()
        {

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (SmartColor.My_ConPar.Hardware.BlenderType == 1)
            {
                if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Blender), 1))
                    return -1;
            }
            else
            {
                if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Blender), 0))
                    return -1;
            }
            return 0;
        }
    }
}
