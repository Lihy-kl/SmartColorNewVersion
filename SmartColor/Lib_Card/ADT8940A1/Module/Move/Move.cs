using System;
using System.ComponentModel.Design;
using System.Threading;

namespace Lib_Card.ADT8940A1.Module.Move
{
    /// <summary>
    /// 定点移动
    /// </summary>
    public abstract class Move
    {
        /// <summary>
        /// 定点移动
        /// 异常：
        ///     1：X轴矢能未接通
        ///     2：X轴伺服器报警
        ///     3：X轴正限位已通
        ///     4：X轴反限位已通
        ///     5：Y轴矢能未接通
        ///     6：Y轴伺服器报警
        ///     7：Y轴正限位已通
        ///     8：Y轴反限位已通
        ///     9：气缸上超时
        ///    10：发现针筒
        ///    11：X轴正在运行
        ///    12：Y轴正在运行
        ///    13：抓手A关闭超时
        ///    14：抓手B关闭超时
        ///    15：抓手A打开超时
        ///    16：抓手B打开超时
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <param name="iNo">瓶号/杯号/预留</param>
        /// <param name="iX">X坐标</param>
        /// <param name="iY">Y坐标</param>
        /// /// <param name="iType">移动类型 0母液瓶 1缸杯位 2 天平位 3 待机位置 4 放盖区 5 泄压区 6:干布区域 7:湿布区域 8:干布夹子 9:湿布夹子</param>
        /// <returns>0：正常；-1：异常；-2：收到退出消息</returns>
        public abstract int TargetMove(int iCylinderVersion, int iNo,int iX,int iY, int iType);


    }
}
