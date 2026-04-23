namespace Lib_Card.ADT8940A1.Module.Infusion
{
    /// <summary>
    /// 注液
    /// </summary>
    public abstract class Infusion
    {
        /// <summary>
        /// 注液
        /// 异常：
        ///     1：Z轴正在运行
        ///     2：Z轴反限位已通
        ///     3：脉冲计算异常
        ///     4：气缸未在上限位
        ///     5：接液盘收回超时
        ///     6：接液盘伸出超时
        ///     7：抓手A打开超时
        ///     8：抓手B打开超时
        /// </summary>
        /// <param name="iSyringeType">0：小针筒；1：大针筒</param>
        /// <param name="iPulse">注液后脉冲</param>
        /// <param name="b">是否需要调用收接液盘</param>
        /// <returns>0：正常；-1：异常；-2；收到退出消息</returns>
        public abstract int LiquidInfusion(int iSyringeType, int iPulse,bool b);
    }
}
