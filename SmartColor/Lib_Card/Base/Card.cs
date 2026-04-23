namespace Lib_Card.Base
{
    public abstract class Card
    {
        #region 接口

       

        #region 运动参数
        /// <summary>
        /// 运动参数
        /// </summary>
        public struct MoveArg
        {
            /// <summary>
            /// 脉冲
            /// </summary>
            public int Pulse;
            /// <summary>
            /// 起始速度
            /// </summary>
            public int LSpeed;
            /// <summary>
            /// 驱动速度
            /// </summary>
            public int HSpeed;
            /// <summary>
            /// 加减速时间
            /// </summary>
            public double Time;
        }
        #endregion

        #region 回零参数

        /// <summary>
        /// 回零参数
        /// </summary>
        public struct HomeArg
        {
            /// <summary>
            /// 回零起始速度
            /// </summary>
            public  int Home_LSpeed { get; set; }

            /// <summary>
            /// 回零驱动速度
            /// </summary>
            public  int Home_HSpeed { get; set; }

            /// <summary>
            /// 回零加速度
            /// </summary>
            public  int Home_USpeed { get; set; }

            /// <summary>
            /// 回零爬行速度
            /// </summary>
            public  int Home_CSpeed { get; set; }

            /// <summary>
            /// 回零偏移量
            /// </summary>
            public  int Home_Offset { get; set; }
        }
        #endregion

        #region 读取版本号
        /// <summary>
        /// 读取版本号
        /// </summary>
        public abstract int GetVersion();
        #endregion

        #region 板卡初始化
        /// <summary>
        /// 板卡初始化
        /// </summary>
        public abstract void CardInit();
        #endregion

        #region 读取输入状态
        /// <summary>
        /// 读取输入状态
        /// </summary>
        /// <param name="iInPutNo">输入点编号</param>
        /// <returns>-1:异常；0：无信号；1：有信号</returns>
        public abstract int ReadInPut(int iInPutNo);
        #endregion

        #region 读取输出状态
        /// <summary>
        /// 读取输出状态
        /// </summary>
        /// <param name="iOutPutNo">输出点编号</param>
        /// <returns>-1:异常；0：无信号；1：有信号</returns>
        public abstract int ReadOutPut(int iOutPutNo);
        #endregion

        #region 写入输出状态
        /// <summary>
        /// 写入输出状态
        /// </summary>
        /// <param name="iOutPutNo">输出点编号</param>
        /// <param name="iStatus">状态</param>
        /// <returns>0:正常；-1:异常</returns>
        public abstract int WriteOutPut(int iOutPutNo, int iStatus);
        #endregion

        #region 读取轴状态
        /// <summary>
        /// 读取轴状态
        /// </summary>
        /// <param name="iAxisNo">轴号</param>
        /// <returns>-1：异常；0：未运行；1：运行</returns>
        public abstract int ReadAxisStatus(int iAxisNo);
        #endregion

        #region 读取轴速度
        /// <summary>
        /// 读取轴速度
        /// </summary>
        /// <param name="iAxisNo">轴号</param>
        /// <param name="iSpeed">当前速度</param>
        /// <returns>-1：异常；非-1：正常</returns>
        public abstract int ReadAxisSpeed(int iAxisNo,ref int iSpeed);
        #endregion

        #region 读取轴逻辑位置
        /// <summary>
        /// 读取轴逻辑位置
        /// </summary>
        /// <param name="iAxisNo">轴号</param>
        /// <param name="iPosition">当前位置</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int ReadAxisCommandPosition(int iAxisNo, ref int iPosition);
        #endregion


        #region 读取轴实际位置
        /// <summary>
        /// 读取轴实际逻辑位置
        /// </summary>
        /// <param name="iAxisNo">轴号</param>
        /// <param name="iPosition">当前位置</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int ReadAxisActualPosition(int iAxisNo, ref int iPosition);
        #endregion


        #region 实际位置置0
        /// <summary>
        /// 实际位置置0
        /// </summary>
        /// <param name="iAxisNo">轴号</param>      
        /// <returns>0：正常；-1：异常</returns>
        public abstract int SetAxisActualPosition(int iAxisNo);
        #endregion

        #region 逻辑位置设定
        /// <summary>
        /// 逻辑位置设定
        /// </summary>
        /// <param name="iAxisNo">轴号</param>  
        /// <param name="iPosition">设定位置</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int SetAxisCommandPosition(int iAxisNo,int iPosition);
        #endregion
       
        #region 清空轴位置
        /// <summary>
        /// 清空轴位置
        /// </summary>
        /// <param name="iAxisNo">轴号</param>
        /// <returns>0：正常：-1：异常</returns>
        public abstract int ClearAxisPosition(int iAxisNo);
        #endregion

        #region 设置回零信号
        /// <summary>
        /// 设置回零信号
        /// </summary>
        /// <param name="iAxisNo">轴号</param>
        /// <param name="iHomeDir">回零方向</param>
        /// <param name="iOffset">偏移量</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int SetHomeMode(int iAxisNo,int iHomeDir,int iOffset);
        #endregion

        #region 设置回零速度
        /// <summary>
        /// 设置回零速度
        /// </summary>
        /// <param name="iAxisNo">轴号</param>
        /// <param name="s_HomeArg">回零参数</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int SetHomeSpeed(int iAxisNo, HomeArg s_HomeArg);

        #endregion

        #region 获取回零状态

        /// <summary>
        /// 获取回零状态
        /// </summary>
        /// <param name="iAxisNo">轴号</param>
        /// <returns>0：回零成功；-1：参数1错误；-2：参数2错误：-3：回零未启动；</returns>
        public abstract int GetHomeStatus(int iAxisNo);

        #endregion

        #region 回零
        /// <summary>
        /// 回零
        /// </summary>
        /// <param name="iAxisNo">轴号</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int Home(int iAxisNo);
        #endregion

        #region 相对移动
        /// <summary>
        /// 相对移动
        /// </summary>
        /// <param name="iAxisNo">轴号</param>
        /// <param name="s_MoveArg">运动参数</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int RelativeMove(int iAxisNo, MoveArg s_MoveArg);
        #endregion

        #region 绝对移动
        /// <summary>
        /// 绝对移动
        /// </summary>
        /// <param name="iAxisNo">轴号</param>
        /// <param name="s_MoveArg">运动参数</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int AbsoluteMove(int iAxisNo, MoveArg s_MoveArg);
        #endregion

        #region 减速停止
        /// <summary>
        /// 减速停止
        /// </summary>
        /// <param name="iAxisNo">轴号</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int DecStop(int iAxisNo);
        #endregion

        #region 立即停止
        /// <summary>
        /// 立即停止
        /// </summary>
        /// <param name="iAxisNo">轴号</param>
        /// <returns>0：正常；-1：异常</returns>
        public abstract int SuddnStop(int iAxisNo);
        #endregion

        #endregion
    }
}
