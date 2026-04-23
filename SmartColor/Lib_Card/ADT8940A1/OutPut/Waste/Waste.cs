namespace Lib_Card.ADT8940A1.OutPut.Waste
{
    /// <summary>
    /// 废液回收
    /// </summary>
    public abstract class Waste
    {
        /// <summary>
        /// 废液回收打开
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Waste_On();

        /// <summary>
        /// 废液回收关闭
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Waste_Off();

    }
}
