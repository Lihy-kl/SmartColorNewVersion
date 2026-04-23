namespace Lib_Card.ADT8940A1.OutPut.Buzzer
{
    /// <summary>
    /// 蜂鸣器
    /// </summary>
    public abstract class Buzzer
    {
        /// <summary>
        /// 蜂鸣器打开
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Buzzer_On();

        /// <summary>
        /// 蜂鸣器关闭
        /// </summary>
        /// <returns>0：正常；-1：异常；</returns>
        public abstract int Buzzer_Off();
    }
}
