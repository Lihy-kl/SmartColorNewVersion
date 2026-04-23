namespace Lib_Card.ADT8940A1.InPut
{
    /// <summary>
    /// 输入
    /// </summary>
    public abstract class InPut
    {
        /// <summary>
        /// 输入状态       
        /// </summary>
        /// <param name="iInPutNo">输入点</param>
        /// <returns>-1：异常；0：无信号；1：有信号</returns>
        public abstract int InPutStatus(int iInPutNo);
    }
}
