using SmartColor.My_AutomaticModule;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Form.Correction
{
    public partial class BottleAddWater : Form
    {
        public BottleAddWater()
        {
            InitializeComponent();
        }

        private async void BtnAddWaterDebug_Click(object sender, EventArgs e)
        {
            BtnAddWaterDebug.Enabled = false;
            try
            {
                // 1. 解析输入
                var input = TxtAddWaterBottleNos .Text.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    MessageBox.Show("请输入需要加水的瓶号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var bottleNos = new List<int>();
                var parts = input.Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    if (part.Contains("-"))
                    {
                        var range = part.Split('-');
                        if (range.Length == 2 && int.TryParse(range[0], out int start) && int.TryParse(range[1], out int end))
                        {
                            if (start > end) (start, end) = (end, start);
                            for (int i = start; i <= end; i++)
                                bottleNos.Add(i);
                        }
                    }
                    else if (int.TryParse(part, out int num))
                    {
                        bottleNos.Add(num);
                    }
                }

                if (bottleNos.Count == 0)
                {
                    MessageBox.Show("输入格式有误，请检查！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2. 校验瓶号是否存在
                var validBottleNos = new List<int>();
                var notExistNos = new List<int>();
                foreach (var no in bottleNos.Distinct())
                {
                    var rows = BottleData.Bottle_details?.Select($"{BOTTLE_DETAILS.BottleNum} = {no}");
                    if (rows != null && rows.Length > 0)
                        validBottleNos.Add(no);
                    else
                        notExistNos.Add(no);
                }

                if (validBottleNos.Count == 0)
                {
                    LocalTranslator.ShowMessage("输入的瓶号均不存在，请检查！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (notExistNos.Count > 0)
                {
                    LocalTranslator.ShowMessage($"以下瓶号不存在，已跳过：{string.Join(",", notExistNos)}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                var dt = DateTime.Now;
                _ = RunTableMan.InsertAsync(new Dictionary<string, object>
                {
                    [SmartColor.My_DataBase.RUN_TABLE.Machine] = "点击母液瓶加水调试启动按钮"
                }, dt);

                // 3. 启动批量加水调试
                var results = await BottleAddWaterDebug.EnqueueBatchBottleAddWaterDebugAsync(validBottleNos);

                // 4. 结果分组与统一交互
                var success = results.Values.Where(r => r.Code == My_Tool.Result.ResultCode.Success).Select(r => r.BottleNo).ToList();
                var failed = results.Values.Where(r => r.Code == My_Tool.Result.ResultCode.Failure).Select(r => r.BottleNo).ToList();
                var error = results.Values.Where(r => r.Code < 0).ToList();

                // 成功气泡提示
                if (success.Count > 0)
                {
                    SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip(
                        $"加水调试成功：{string.Join(",", success)}"
                    );
                }

                // 失败弹窗，支持重试（初始重试次数为1）
                if (failed.Count > 0)
                {
                    ShowRetryDialog(failed, 1);
                }

                // 异常/取消气泡提示
                foreach (var r in error)
                {
                    Logger.Error($"瓶号{r.BottleNo}加水调试异常：{r.Message}", r.Exception);
                }
            }
            finally
            {
                BtnAddWaterDebug.Enabled = true;
            }
        }

        // 新增重试方法
        // 重试弹窗逻辑，带重试计数
        private void ShowRetryDialog(List<int> retryBottleNos, int retryCount)
        {
            SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                "加水调试失败",
                $"以下瓶号加水调试失败：{string.Join(",", retryBottleNos)}\n是否重新加水？",
                async btn =>
                {
                    if (btn == "重试")
                    {
                        var results = await BottleAddWaterDebug.EnqueueBatchBottleAddWaterDebugAsync(retryBottleNos);

                        var success = results.Values.Where(r => r.Code == My_Tool.Result.ResultCode.Success).Select(r => r.BottleNo).ToList();
                        var failed = results.Values.Where(r => r.Code == My_Tool.Result.ResultCode.Failure).Select(r => r.BottleNo).ToList();
                        var error = results.Values.Where(r => r.Code < 0).ToList();

                        if (success.Count > 0)
                        {
                            SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip(
                                $"重试加水调试成功：{string.Join(",", success)}"
                            );
                        }
                        if (failed.Count > 0)
                        {
                            if (retryCount < 2)
                            {
                                // 继续重试
                                ShowRetryDialog(failed, retryCount + 1);
                            }
                            else
                            {
                                // 超过2次，提示检查硬件
                                SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                                    "加水调试失败",
                                    $"以下瓶号多次加水调试失败：{string.Join(",", failed)}\n请检查相关硬件！",
                                    null,
                                    new[] { "确定" },
                                    "确定"
                                );
                            }
                        }
                        foreach (var r in error)
                        {
                            Logger.Error($"瓶号{r.BottleNo}加水调试异常：{r.Message}", r.Exception);
                        }
                    }
                },
                new[] { "重试", "取消" },
                "重试"
            );
        }
    }
}