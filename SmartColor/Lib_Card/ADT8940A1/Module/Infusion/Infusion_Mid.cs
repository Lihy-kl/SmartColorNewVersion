using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lib_Card.ADT8940A1.Module.Infusion
{
    public class Infusion_Mid : Infusion
    {
        public override int LiquidInfusion(int iSyringeType, int iPulse, bool b)
        {
            OutPut.Tray.Tray tray = new OutPut.Tray.Tray_Condition();
            if (b)
                if (-1 == tray.Tray_Off())
                    return -1;

            OutPut.Cylinder.Cylinder cylinder = new OutPut.Cylinder.DualControl.Cylinder_Condition();
            if (-1 == cylinder.CylinderMid())
                return -1;
            
            OutPut.Waste.Waste waste = new OutPut.Waste.Waste_Basic();
            if (-1 == waste.Waste_On())
                return -1;

           
            int iZRes = CardObject.OA1Axis.Absolute_Z(iSyringeType, iPulse, 0);
            if (0 != iZRes)
                return iZRes;

            Thread.Sleep(500);

            if (-1 == cylinder.CylinderUp(0))
                return -1;

            if (-1 == tray.Tray_On())
                return -1;

            if (-1 == waste.Waste_Off())
                return -1;

          

            return 0;
        }
    }
}
