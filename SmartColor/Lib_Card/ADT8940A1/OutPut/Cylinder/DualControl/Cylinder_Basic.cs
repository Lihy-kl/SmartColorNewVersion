using System;

namespace Lib_Card.ADT8940A1.OutPut.Cylinder.DualControl
{
    /// <summary>
    /// 双控版
    /// 无条件检查
    /// </summary>
    public class Cylinder_Basic : Cylinder
    {
        public override int CylinderDown(int i_judge)
        {

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Up), 0))
                return -1;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Down), 1))
                return -1;
            return 0;
        }

        public override int CylinderUp(int i_judge)
        {

            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Down), 0))
                return -1;
            if (-1 == CardObject.OA1.WriteOutPut(Convert.ToInt32(boaed.OutPut_Cylinder_Up), 1))
                return -1;
            return 0;
        }

        public override int CylinderMid()
        {
            return -1;
        }
    }
}
