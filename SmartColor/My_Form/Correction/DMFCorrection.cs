using SmartColor.My_AutomaticModule;
using SmartColor.My_File; // 引入LocalTranslator
using SmartColor.My_Tool; // 引入MessageEventManager
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Form.Correction
{
    /// <summary>
    /// 溶解剂校正/验证界面
    /// 负责与业务流程交互，并根据业务层返回值进行UI提示和交互
    /// </summary>
    public partial class DMFCorrection : Form
    {
        public DMFCorrection()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 校正启动按钮点击事件
        /// 业务流程：
        ///   1. 调用业务层溶解剂校正流程
        ///   2. 根据返回值进行UI提示和交互
        /// 返回值说明（业务层EnqueueDMFCorrectionAsync）：
        ///   Code=0  —— 校正成功
        ///   Code>0  —— 业务失败（如误差过大、设备未检测到加溶解剂量等）
        ///   Code=-1 —— 代码异常
        ///   Code=-2 —— 任务被取消
        /// </summary>
        private async void BtnCorrection_Click(object sender, EventArgs e)
        {
            var dt = DateTime.Now;
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.Machine] = "点击溶解剂校正启动按钮"
            }, dt);

            var result = await SmartColor.My_AutomaticModule.DMFCorrectionRobotTask.EnqueueDMFCorrectionAsync();

            switch (result.Code)
            {
                case My_Tool.Result.ResultCode.Success:
                    MessageEventManager.Instance.RequestShowBalloonTip("溶解剂校正完成！");
                    break;
                
                case My_Tool.Result.ResultCode.Failure:
                    MessageEventManager.Instance.RequestShowMessage(
                        "校正失败",
                        result.Message ?? "加溶解剂误差过大或设备异常，是否重试？",
                        btn =>
                        {
                            if (btn == "重试")
                                BtnCorrection_Click(sender, e);
                        },
                        new[] { "重试", "取消" },
                        "重试"
                    );
                    break;
                case My_Tool.Result.ResultCode.Exception:
                    My_File.Logger.Error("溶解剂校正异常", result.Exception);
                    break;
                case My_Tool.Result.ResultCode.Canceled:
                    MessageEventManager.Instance.RequestShowBalloonTip("溶解剂校正任务已被取消");
                    break;
            }
        }

        /// <summary>
        /// 验证启动按钮点击事件
        /// 业务流程：
        ///   1. 校验输入的验证重量
        ///   2. 调用业务层溶解剂验证流程
        ///   3. 根据返回值进行UI提示和交互
        /// 返回值说明（业务层EnqueueDMFVerifyAsync）：
        ///   Code=0  —— 验证成功
        ///   Code>0  —— 业务失败（如天平异常、加溶解剂失败等）
        ///   Code=-1 —— 代码异常
        ///   Code=-2 —— 任务被取消
        /// </summary>
        private async void BtnRecheck_Click(object sender, EventArgs e)
        {
            var dt = DateTime.Now;
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.Machine] = "点击溶解剂验证启动按钮"
            }, dt);

            if (!int.TryParse(TxtRecheckWeight.Text, out int recheckWeight) || recheckWeight <= 0)
            {
                LocalTranslator.ShowMessage("请输入有效的验证重量,仅支持正整数！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = await SmartColor.My_AutomaticModule.DMFCorrectionRobotTask.EnqueueDMFVerifyAsync(9999, recheckWeight);

            switch(result.Code)
            {
                case My_Tool.Result.ResultCode.Success:
                    MessageEventManager.Instance.RequestShowBalloonTip("溶解剂验证成功！");
                    break;
                case My_Tool.Result.ResultCode.Failure:
                    MessageEventManager.Instance.RequestShowMessage(
                        "验证失败",
                        result.Message ?? "加溶解剂误差过大或设备异常，是否重试？",
                        btn =>
                        {
                            if (btn == "重试")
                                BtnRecheck_Click(sender, e);
                        },
                        new[] { "重试", "取消" },
                        "重试"
                    );
                    break;
                case My_Tool.Result.ResultCode.Exception:
                    My_File.Logger.Error("溶解剂验证异常", result.Exception);
                    break;
                case My_Tool.Result.ResultCode.Canceled:
                    MessageEventManager.Instance.RequestShowBalloonTip( "溶解剂验证任务已被取消");
                    break;
            }
            
        }
    }
}