namespace Lib_Card.ADT8940A1.OutPut.Cylinder
{
    /// <summary>
    /// 上下气缸
    /// </summary>
    public abstract class Cylinder
    {
        /// <summary>
        /// 气缸上
        /// i_judge：0代表开盖 1 代表关盖 2代表关盖确认气缸下到位离开就停止
        /// 异常：
        ///     1：气缸上超时
        ///     2：阻挡气缸收回超时
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int CylinderUp(int i_judge);

        /// <summary>
        /// 气缸下
        /// i_judge：0代表开盖 1 代表关盖 2代表关盖确认关好调用，不报气缸下超时 3开盖时取杯盖气缸下
        /// 异常：
        ///     1：X轴正在运行
        ///     2：Y轴正在运行
        ///     3：X轴伺服器报警
        ///     4：Y轴伺服器报警
        ///     5：接液盘未收回
        ///     6：气缸下超时
        ///     7：阻挡气缸收回超时
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int CylinderDown(int i_judge);

        /// <summary>
        /// 气缸中
        /// 异常：
        ///     1：X轴正在运行
        ///     2：Y轴正在运行
        ///     3：X轴伺服器报警
        ///     4：Y轴伺服器报警
        ///     5：接液盘未收回
        ///     6：气缸下超时
        ///     7：气缸未在上限位
        ///     8：阻挡气缸伸出超时
        ///     
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int CylinderMid();
    
    }
}
