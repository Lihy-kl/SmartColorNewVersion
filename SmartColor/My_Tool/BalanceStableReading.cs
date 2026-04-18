using com.google.zxing;
using SmartColor.My_File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartColor.My_Tool
{
    /// <summary>
    /// 天平稳定读数类
    /// </summary>
    internal class BalanceStableReading
    {
        /// <summary>
        /// 天平当前读数
        /// </summary>
        public static double CurrentRead = 0;

        /// <summary>
        /// 天平异常标志位
        /// </summary>
        public static bool BalanceError = false;

        
       

        /// <summary>
        /// 天平稳定读数(异步)
        /// </summary>
        /// <param name="maxWaitSeconds">超时时间，默认10s</param>
        /// <returns>稳定读数</returns>
        public static async Task<double> StableReadingAsync(int maxWaitSeconds = 10)
        {
            double result = 0;
            int elapsedMs = 0;
            int sleepMs = Convert.ToInt32(My_ConPar.Delay.Balance_Read * 1000);

            // 运行表记录启动
            var dt = DateTime.Now;
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.RobotHand] = "天平稳定读数启动"
            }, dt);

            try
            {
                while (true)
                {
                    await Task.Delay(sleepMs).ConfigureAwait(false);
                    double read1 = CurrentRead;
                    await Task.Delay(sleepMs).ConfigureAwait(false);
                    elapsedMs += sleepMs;
                    double read2 = CurrentRead;

                    double errorValue = Math.Abs(read1 - read2);
                    Logger.Info($"天平读数检测: read1={read1}, read2={read2}, error={errorValue}");

                    if (errorValue <= My_ConPar.Other.Stable_Value)
                    {
                        int roundDigits = RetainDecimals();
                        result = Math.Round((read1 + read2) / 2.0, roundDigits);

                        Logger.Info($"天平读数稳定: 结果={result}, 保留小数={roundDigits}");

                        // 运行表记录完成
                         dt = DateTime.Now;
                        _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                        {
                            [SmartColor.My_DataBase.RUN_TABLE.RobotHand] = $"天平稳定读数完成，结果:{result}"
                        }, dt);

                        return result;
                    }

                    if (elapsedMs >= maxWaitSeconds * 1000)
                    {
                        Logger.Error("天平读数在最大等待时间内未达到稳定状态。");

                        bool continueWait = false;
                        var tcs = new TaskCompletionSource<bool>();

                        // 异步弹窗
                        MessageEventManager.Instance.RequestShowMessage(
                            "天平稳定读数报警",
                            "天平读数长时间不稳定，是否继续等待？",
                            btn =>
                            {
                                if (btn == "继续等待")
                                    continueWait = true;
                                tcs.SetResult(true);
                            },
                            new[] { "继续等待", "退出" },
                            "继续等待"
                        );

                        await tcs.Task.ConfigureAwait(false);

                        if (continueWait)
                        {
                            elapsedMs = 0; // 重新计时
                            continue;
                        }
                        else
                        {
                            // 运行表记录退出
                            dt = DateTime.Now;
                            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                            {
                                [SmartColor.My_DataBase.RUN_TABLE.RobotHand] = "天平稳定读数用户选择退出"
                            }, dt);
                            return result; // 用户选择退出
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("天平稳定读数异常", ex);
                return result;
            }
        }

        /// <summary>
        /// 天平检查
        /// </summary>
        /// <returns></returns>
        public static async Task BalanceCheck()
        {
            // 运行表记录启动
            var dt = DateTime.Now;
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.RobotHand] = "天平检查启动"
            }, dt);

            try
            {

                while (true)
                {
                    if (!BalanceError)
                    {
                        double max = My_ConPar.Other.BalanceMaxWeight;
                        if (CurrentRead + 200.000 > max)
                        {
                            //预留关闭势能功能


                            var btn = await MessageEventManager.Instance.RequestShowMessageAsync(
                            "天平预警",
                            "废液桶液量过高，请先清空废液桶，再点击继续",
                            new[] { "继续" },
                            "继续");

                            if (btn == "继续")
                            {
                                continue;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        continue;
                    }

                }
                // 运行表记录完成
                dt = DateTime.Now;
                _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                {
                    [SmartColor.My_DataBase.RUN_TABLE.RobotHand] = "天平检查完成"
                }, dt);
            }
            catch (Exception ex)
            {
                Logger.Error("天平检查异常", ex);
                throw;
            }
        }


        /// <summary>
        /// 先检查天平，再读取稳定数据
        /// </summary>
        /// <returns>返回稳定重量</returns>
        public static async Task<double> CheckAndReadBalanceAsync()
        {
            await My_Tool.BalanceStableReading.BalanceCheck();
            return await My_Tool.BalanceStableReading.StableReadingAsync();
        }


        /// <summary>
        /// 根据当前天平类型返回应保留的小数位数
        /// </summary>
        /// <returns>小数位数</returns>
        public static int RetainDecimals()
        {
            // 防御性编程，避免空引用
            if ( My_ConPar.Object.CurrentBalance == null)
                return 2; // 或你希望的默认小数位数

            int roundDigits = (My_ConPar.Object.CurrentBalance.BalanceType == 0 ||
                               My_ConPar.Object.CurrentBalance.BalanceType == 2 ||
                               My_ConPar.Object.CurrentBalance.BalanceType == 3) ? 2 : 3;
            return roundDigits;
        }
    }
}