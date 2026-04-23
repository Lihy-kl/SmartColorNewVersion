namespace Lib_Card.ADT8940A1.OutPut.Blender
{
    /// <summary>
    /// 停止搅拌
    /// </summary>
    public abstract class Blender
    {
        /// <summary>
        /// 停止搅拌打开
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Blender_On();

        /// <summary>
        /// 停止搅拌关闭      
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Blender_Off();
    }
}
