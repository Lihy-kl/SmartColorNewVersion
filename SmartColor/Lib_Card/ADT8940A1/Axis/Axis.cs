using System;
using System.ComponentModel.Design;
using System.Threading;

namespace Lib_Card.ADT8940A1.Axis
{
    public abstract class Axis
    {
        /// <summary>
        /// 退出信号
        /// </summary>
        public static bool Axis_Exit { get; set; } = false;

        /// <summary>
        /// 暂停信号
        /// </summary>
        public static bool Axis_Paused { get; set; } = false;

     

        /// <summary>
        /// X轴相对移动
        /// 异常：
        ///     1：X轴矢能未接通
        ///     2：X轴伺服器报警
        ///     3：X轴正限位已通
        ///     4：X轴反限位已通
        ///     5：气缸上超时
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <param name="s_MoveArg">运动参数</param>
        /// <param name="iReserve">预留</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int Relative_X(int iCylinderVersion, ADT8940A1_Card.MoveArg s_MoveArg, int iReserve);

        /// <summary>
        /// X轴绝对移动
        /// 异常：
        ///     1：X轴矢能未接通
        ///     2：X轴伺服器报警
        ///     3：X轴正限位已通
        ///     4：X轴反限位已通
        ///     5：气缸上超时
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <param name="iPulse">脉冲</param>
        /// <param name="iReserve">预留</param>
        /// <returns>0：正常；-1：异常；-2：收到退出消息</returns>
        public abstract int Absolute_X(int iCylinderVersion, int iPulse, int iReserve);

        /// <summary>
        /// X轴回零
        /// 异常：
        ///     1：X轴矢能未接通
        ///     2：X轴伺服器报警
        ///     3：X轴正限位已通
        ///     4：气缸上超时
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <param name="iReserve">预留</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int Home_X(int iCylinderVersion, int iReserve);

        /// <summary>
        /// Y轴相对移动
        /// 异常：
        ///     1：Y轴矢能未接通
        ///     2：Y轴伺服器报警
        ///     3：Y轴正限位已通
        ///     4：Y轴反限位已通
        ///     5：气缸上超时
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <param name="s_MoveArg">运动参数</param>
        /// <param name="iReserve">预留</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int Relative_Y(int iCylinderVersion, ADT8940A1_Card.MoveArg s_MoveArg, int iReserve);

        /// <summary>
        /// Y轴绝对移动
        /// 异常：
        ///     1：Y轴矢能未接通
        ///     2：Y轴伺服器报警
        ///     3：Y轴正限位已通
        ///     4：Y轴反限位已通
        ///     5：气缸上超时
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <param name="iPulse">脉冲</param>
        /// <param name="iReserve">预留</param>
        /// <returns>0：正常；-1：异常；-2：收到退出消息</returns>
        public abstract int Absolute_Y(int iCylinderVersion, int iPulse, int iReserve);

        /// <summary>
        /// Y轴回零
        /// 异常：
        ///     1：Y轴矢能未接通
        ///     2：Y轴伺服器报警
        ///     3：Y轴正限位已通
        ///     4：气缸上超时
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <param name="iReserve">预留</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int Home_Y(int iCylinderVersion, int iReserve);

        /// <summary>
        /// Z轴相对移动
        /// 异常：
        ///     1：Z轴反限位已通
        /// </summary>
        /// <param name="s_MoveArg">运动参数</param>
        /// <param name="iReserve">预留</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int Relative_Z(ADT8940A1_Card.MoveArg s_MoveArg, int iReserve);

        /// <summary>
        /// Z轴绝对移动
        /// 异常：
        ///     1：脉冲计算异常
        ///     2：Z轴反限位已通
        ///     3：抓手A打开超时
        ///     4：抓手B打开超时
        ///     5：Z轴正在运行
        ///     6：气缸未在上限位
        ///     7：接液盘收回超时
        /// </summary>
        /// <param name="iSyringeType">0：小针筒；1：大针筒；</param>
        /// <param name="iPulse">脉冲</param>
        /// <param name="iReserve">预留</param>
        /// <returns>0：正常；-1：异常；-2：收到退出消息</returns>
        public abstract int Absolute_Z(int iSyringeType,int iPulse, int iReserve);

        /// <summary>
        /// Z轴回零
        /// 异常：
        ///     1：抓手A关闭超时
        ///     2：抓手B关闭超时
        ///     3：气缸上超时
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <param name="iReserve">预留</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int Home_Z(int iCylinderVersion, int iReserve);
    }
}
