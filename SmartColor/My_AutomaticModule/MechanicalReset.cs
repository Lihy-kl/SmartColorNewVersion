using SmartColor.My_RobotManager;
using SmartColor.My_SemiAutoModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_AutomaticModule
{
    internal class MechanicalReset
    {
        public class MechanicalResetResult
        {
            /// <summary>结果码，(成功，失败，异常，取消)</summary>
            public My_Tool.Result.ResultCode Code { get; set; }
            /// <summary>详细信息</summary>
            public string Message { get; set; }
          
            /// <summary>异常对象</summary>
            public Exception Exception { get; set; }
        }

        /// <summary>
        /// 机械手复位，移动机械手到母液瓶位置，释放针头
        /// </summary>
        /// <param name="bottleNo">瓶号</param>
        /// <param name="message">提示信息</param>
        /// <returns></returns>
        public static async Task<MechanicalResetResult> ResetMechanical(int bottleNo,string message)
        {
            var result = new MechanicalResetResult
            {
                Code = My_Tool.Result.ResultCode.NeedInteraction,
                Message = message,
                Exception = null
            };

            var bottleCT = My_Tool.AreaCoordinateFinder.TryGetBottleCoordinateAsync(bottleNo);

            var currentTask = RobotTaskManager.Instance.CurrentTask as RobotBusinessTask<BottleCorrectionResult>;
           
            var bottleCTR = await bottleCT;
            if (!bottleCTR.found)
            {
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = $"获取{bottleNo}号母液瓶坐标异常";
                result.Exception = new Exception($"获取{bottleNo}号母液瓶坐标异常");
                return result;
            }
            if (currentTask != null)
                await currentTask.CheckPauseOnlyAsync();
           
            var semiAutoResult = await SemiAutoHelperFactory.Current.MoveToBottleAsync(bottleNo, bottleCTR.x, bottleCTR.y, 1, 0);
            switch (semiAutoResult.Level)
            {
                case SemiAutoResultCode.Exception:
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = semiAutoResult.Message;
                    result.Exception = semiAutoResult.Exception;
                    return result;
                case SemiAutoResultCode.NeedInteraction:
                    result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                    result.Message = semiAutoResult.Message;
                    return result;
                default:
                    break;
            }

            var info = SmartColor.My_Tool.BottleAuxiliary.GetBottleInfo(bottleNo);
            if (info.Result != 0)
            {
                result.Code = My_Tool.Result.ResultCode.Exception;
                result.Message = $"获取{bottleNo}号母液瓶信息异常";
                result.Exception = new Exception($"获取{bottleNo}号母液瓶信息异常");
                return result;
            }

            short syringeType = info.SyringeType;

            if (currentTask != null)
                await currentTask.CheckPauseOnlyAsync();
            var releaseNeedleTask = await SemiAutoHelperFactory.Current.ReleaseNeedleAsync(syringeType);
            switch (releaseNeedleTask.Level)
            {
                case SemiAutoResultCode.Exception:
                    result.Code = My_Tool.Result.ResultCode.Exception;
                    result.Message = semiAutoResult.Message;
                    result.Exception = semiAutoResult.Exception;
                    return result;
                case SemiAutoResultCode.NeedInteraction:
                    result.Code = My_Tool.Result.ResultCode.NeedInteraction;
                    result.Message = semiAutoResult.Message;
                    return result;
                default:
                    break;
            }

            return result;
        }
    }
}
