namespace Lib_Card.ADT8940A1.OutPut.Tongs
{
    /// <summary>
    /// 抓手
    /// </summary>
    public abstract class Tongs
    {
        /// <summary>
        /// 抓手关闭
        /// 异常:
        ///      1：抓手A关闭超时
        ///      2：抓手B关闭超时
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Tongs_On();

        /// <summary>
        /// 抓手打开
        /// 异常:
        ///     1：抓手A打开超时
        ///     2：抓手B打开超时
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Tongs_Off();
    }
}
