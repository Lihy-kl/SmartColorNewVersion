namespace Lib_Card.ADT8940A1.OutPut.Tray
{
    /// <summary>
    /// 接液盘
    /// </summary>
    public abstract class Tray
    {
        /// <summary>
        /// 接液盘伸出
        /// 异常：
        ///     1：Z轴正在运行
        ///     2：气缸未在上限位
        ///     3：接液盘伸出超时
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Tray_On();

        /// <summary>
        /// 接液盘收回
        /// 异常：
        ///     1：Z轴正在运行
        ///     2：气缸未在上限位
        ///     3：接液盘收回超时
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Tray_Off();
    }
}
