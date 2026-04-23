namespace Lib_Card.ADT8940A1.OutPut.X_Power
{
    /// <summary>
    /// X轴矢能
    /// </summary>
    public abstract class X_Power
    {
        /// <summary>
        /// X轴矢能打开
        /// 异常：
        ///     1：X轴正在运行
        ///     2：X轴伺服器报警
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int X_Power_On();

        /// <summary>
        /// Y轴矢能关闭
        /// 异常：
        ///     1：X轴正在运行
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int X_Power_Off();
    }
}
