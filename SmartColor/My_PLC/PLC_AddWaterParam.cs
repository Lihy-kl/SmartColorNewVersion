using SmartColor.My_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_PLC
{
    /// <summary>加水参数</summary>
    internal class PLC_AddWaterParam : PLC_SemiAutoParamBase
    {
        /// <summary>加水时间 D801 (s)</summary>
        public ProtocolItem WaterTime { get; set; }
        /// <summary>是否使用流量计 D803</summary>
        public ProtocolItem UseFlowMeter { get; set; }

        /// <summary>
        /// 加水参数构造函数（全部参数）
        /// </summary>
        /// <param name="waterTime">加水时间/s</param>
        /// <param name="useFlowMeter">是否使用流量计（0=否，1=是）</param>
        public PLC_AddWaterParam(double waterTime, short useFlowMeter)
            : base(PLC.SemiAutomaticOperation.AddWater)
        {
            WaterTime = new ProtocolItem(801, typeof(int), (int)(waterTime * 1000));
            UseFlowMeter = new ProtocolItem(803, typeof(short), useFlowMeter);
        }

        /// <summary>
        /// 加水参数构造函数（全部参数）
        /// </summary>
        /// <param name="objectWeight">目标重量</param>
        /// <param name="waterTime">加水时间/s</param>
        public PLC_AddWaterParam(double objectWeight, double waterTime) : base(PLC.SemiAutomaticOperation.AddWater)
        {
            if(objectWeight <=2)
            {
                waterTime = waterTime / 0.9;
            }
            WaterTime = new ProtocolItem(801, typeof(int), (int)(waterTime * 1000));
            UseFlowMeter = new ProtocolItem(803, typeof(short), My_ConPar.Hardware.UseFlowmeter);
        }
    }

}
