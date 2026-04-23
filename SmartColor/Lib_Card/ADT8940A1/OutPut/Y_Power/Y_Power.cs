namespace Lib_Card.ADT8940A1.OutPut.Y_Power
{
    /// <summary>
    /// Y轴矢能
    /// </summary>
    public abstract class Y_Power
    {
        /// <summary>
        /// Y轴矢能打开
        /// 异常：
        ///     1：Y轴正在运行
        ///     2：Y轴伺服器报警
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Y_Power_On();

        /// <summary>
        /// Y轴矢能关闭
        /// 异常：
        ///     1：Y轴正在运行
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Y_Power_Off();
    }
}
