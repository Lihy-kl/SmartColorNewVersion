namespace Lib_Card.ADT8940A1.Module.Home
{
    /// <summary>
    /// 回零
    /// </summary>
    public abstract class Home
    {

        /// <summary>
        /// 回原点完成标志位
        /// </summary>
        public static bool Home_XYZFinish { get; set; } = false;

        /// <summary>
        /// XYZ回零
        /// 异常：
        ///     1：发现针筒
        ///     2：X轴矢能未接通
        ///     3：X轴伺服器报警
        ///     4：X轴正限位已通
        ///     5：Y轴矢能未接通
        ///     6：Y轴伺服器报警
        ///     7：Y轴正限位已通
        ///     8：X轴正在运行
        ///     9：Y轴正在运行
        ///    10：气缸上超时
        ///    11：抓手A关闭超时
        ///    12：抓手B关闭超时
        ///    13：抓手A打开超时
        ///    14：抓手B打开超时
        ///      
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int Home_XYZ(int iCylinderVersion);

        /// <summary>
        /// Z回零
        /// 异常:
        ///      1：抓手A关闭超时
        ///      2：抓手B关闭超时
        ///      3：抓手A打开超时
        ///      4：抓手B打开超时
        ///      5：气缸上超时
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int Home_Z(int iCylinderVersion);
    }
}
