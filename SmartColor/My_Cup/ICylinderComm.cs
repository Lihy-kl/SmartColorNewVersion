using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartColor.My_Tool.CupAuxiliary;

namespace SmartColor.My_Cup
{
    /// <summary>
    /// 单杯滴液启动结果类型(成功，异常，取消)
    /// </summary>
    public class DyeingResult
    {
        /// <summary>结果码(成功，异常，取消)</summary>
        public My_Tool.Result.ResultCode Code { get; set; }

        /// <summary>杯号</summary>
        public int CupNo { get; set; }

        /// <summary>详细信息</summary>
        public string Message { get; set; }

        /// <summary>异常对象</summary>
        public Exception Exception { get; set; }
    }
    /// <summary>
    /// 通用缸通讯接口，适配所有翻转缸/转子缸等
    /// </summary>
    public interface ICylinderComm
    {

        /// <summary>
        /// 当前通讯线程是否正在运行
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// 杯号滴液请求（带开盖判断）
        /// </summary>
        Task< DyeingResult> RequestDropLiquidAsync(int cupNum, int cupChoice);

        /// <summary>
        /// 杯号染色启动
        /// </summary>
        Task<DyeingResult> DyeingStartAsync(int cupNum, int cupChoice);

        /// <summary>
        /// 获取指定杯号的实时温度
        /// </summary>
        double GetCupTemp(int cupNum);

        /// <summary>
        /// 停止通讯
        /// </summary>
        void RequestStop();

        /// <summary>
        /// 发送停止指令
        /// </summary>
        /// <param name="cupNum"></param>
        Task SendStopAsync(int cupNum,bool needOffLine = true);


        /// <summary>
        /// 发送洗杯/高温洗杯指令(前洗杯/滴液失败洗杯不升温， 其他 带升温)
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <param name="Type">1=停止洗杯；2=前洗杯；3=失败洗杯，4=高温洗杯</param>
        /// <returns></returns>
        Task SendWashAsync(int cupNum,string Type);

        /// <summary>
        /// 发送下线指令
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <param name="cupChoice">杯选择</param>
        /// <returns></returns>
        Task SendOffLine(int cupNum, int cupChoice);

        /// <summary>
        /// 发送上线指令
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <param name="cupChoice">杯选择</param>
        /// <returns></returns>
        Task SendOnLine(int cupNum, int cupChoice);

        /// <summary>
        /// 发送暂停指令
        /// </summary>
        /// <returns></returns>
        Task SendPause();

        /// <summary>
        /// 发送恢复指令
        /// </summary>
        /// <returns></returns>
        Task SendResume();

        /// <summary>
        /// 同步杯盖状态
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <returns></returns>
        Task SyncCoverStatus(int cupNum);

        /// <summary>
        /// 发送加药开始状态
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <returns></returns>
        Task<bool> SendAddChemicaStart(int cupNum);

        /// <summary>
        /// 发送加药完成状态
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <returns></returns>
        Task SendAddChemicaFinish(int cupNum);

        /// <summary>
        /// 发送确认继续指令
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <param name="choice">选择：1 = 自动出入布完成 5=显示确定键</param>
        /// <returns></returns>
        Task SendShowSure(int cupNum,int choice);

        /// <summary>
        /// 发送下一步指令
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <param name="stepInfo">步骤详情</param>
        Task SendNextStep(int cupNum, StepInfo stepInfo);

        /// <summary>
        /// 出放布确认申请处理
        /// </summary>
        /// <param name="cupNo">杯号</param>
        /// <param name="tc">操作类型</param>
        /// <param name="stayTank">是否出布留在缸里</param>
        /// <returns></returns>
        Task PutClothConfirm(int cupNo, string tc,bool stayTank= false);

       
    }
}
