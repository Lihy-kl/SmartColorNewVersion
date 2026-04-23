namespace Lib_Card.ADT8940A1.OutPut.Water
{
    /// <summary>
    /// 加水
    /// </summary>
    public abstract class Water
    {
        /// <summary>
        /// 加水打开
        /// 异常：
        ///     1：X轴正在运行
        ///     2: Y轴正在运行
        ///     3: 接液盘未伸出
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Water_On();

        /// <summary>
        /// 加水关闭
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Water_Off();
    }
}
