using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Form.Correction
{
    public partial class BottleSelf : Form
    {
        public BottleSelf()
        {
            InitializeComponent();
        }

        private async void BtnRecheck_Click(object sender, EventArgs e)
        {
            // 1. 解析输入
            var input = TxtRecheckWeight.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("请输入需要自检的瓶号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                My_File.LocalTranslator.ShowMessage("输入的瓶号均不存在，请检查！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (notExistNos.Count > 0)
            {
                My_File.LocalTranslator.ShowMessage($"以下瓶号不存在，已跳过：{string.Join(",", notExistNos)}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            var dt = DateTime.Now;
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.Machine] = "点击母液瓶自检启动按钮"
            }, dt);

            // 3. 启动批量自检
            var results = await SmartColor.My_AutomaticModule.BottleSelfRobotTask.EnqueueBatchBottleCorrectionAsync(validBottleNos);

            // 4. 结果分组与统一交互
            var success = results.Values.Where(r => r.Code == 0).Select(r => r.BottleNo).ToList();
            var failed = results.Values.Where(r => r.Code > 0).Select(r => r.BottleNo).ToList();
            var error = results.Values.Where(r => r.Code < 0).ToList();

            // 成功气泡提示
            if (success.Count > 0)
            {
                SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip(
                    $"自检成功：{string.Join(",", success)}"
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
                Logger.Error($"瓶号{r.BottleNo}自检异常：{r.Message}", r.Exception);
            }
        }

        // 新增重试方法
        private void ShowRetryDialog(List<int> retryBottleNos, int retryCount)
        {
            SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                "自检失败",
                $"以下瓶号自检失败：{string.Join(",", retryBottleNos)}\n是否重新自检？",
                async btn =>
                {
                    if (btn == "重试")
                    {
                        var results = await SmartColor.My_AutomaticModule.BottleSelfRobotTask.EnqueueBatchBottleCorrectionAsync(retryBottleNos);

                        var success = results.Values.Where(r => r.Code == 0).Select(r => r.BottleNo).ToList();
                        var failed = results.Values.Where(r => r.Code > 0).Select(r => r.BottleNo).ToList();
                        var error = results.Values.Where(r => r.Code < 0).ToList();

                        if (success.Count > 0)
                        {
                            SmartColor.My_Tool.MessageEventManager.Instance.RequestShowBalloonTip(
                                $"重试自检成功：{string.Join(",", success)}"
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
                                // 超过2次，提示检查密封圈
                                SmartColor.My_Tool.MessageEventManager.Instance.RequestShowMessage(
                                    "自检失败",
                                    $"以下瓶号多次自检失败：{string.Join(",", failed)}\n请检查针筒密封圈！",
                                    null,
                                    new[] { "确定" },
                                    "确定"
                                );
                            }
                        }
                        foreach (var r in error)
                        {
                            Logger.Error($"瓶号{r.BottleNo}自检异常：{r.Message}", r.Exception);
                        }
                    }
                },
                new[] { "重试", "取消" },
                "重试"
            );
        }


    }
}
