namespace Lib_Card.ADT8940A1.Module.Extraction
{
    /// <summary>
    /// 抽液
    /// </summary>
    public abstract class Extraction
    {
        /// <summary>
        /// 抽液
        /// 异常：
        ///     1：未发现针筒
        ///     2：气缸上超时
        ///     3：气缸未在上限位
        ///     4：气缸下超时
        ///     5：X轴正在运行
        ///     6：X轴伺服器报警
        ///     7：Y轴正在运行
        ///     8：Y轴伺服器报警
        ///     9：Z轴正在运行
        ///    10：Z轴反限位已通
        ///    11：脉冲计算异常
        ///    12：接液盘未收回
        ///    13：接液盘收回超时
        ///    14：抓手A关闭超时
        ///    15：抓手B关闭超时
        ///    16：抓手A打开超时
        ///    17：抓手B打开超时
        /// </summary>
        /// <param name="iCylinderVersion">0：单控上下气缸；1：双控上下气缸</param>
        /// <param name="iSyringeType">0：小针筒；1：大针筒</param>
        /// <param name="iPulse">抽液脉冲（不包含反推脉冲）</param>
        /// <param name="iIsAssitant">0：染料；1：助剂</param>
        /// <returns>0：正常；-1：异常；-2：收到退出消息</returns>
        public abstract int FluidExtraction(int iCylinderVersion, int iSyringeType, int iPulse,int iIsAssitant);
    }
}
