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
    /// 水校正/验证界面
    /// 负责与业务流程交互，并根据业务层返回值进行UI提示和交互
    /// </summary>
    public partial class WaterCorrection : Form
    {
        public WaterCorrection()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 校正启动按钮点击事件
        /// 业务流程：
        ///   1. 调用业务层水校正流程
        ///   2. 根据返回值进行UI提示和交互
        /// 返回值说明（业务层EnqueueWaterCorrectionAsync）：
        ///   Code=0  —— 校正成功
        ///   Code>0  —— 业务失败（如误差过大、设备未检测到加水量等）
        ///   Code=-1 —— 代码异常
        ///   Code=-2 —— 任务被取消
        /// </summary>
        private async void BtnCorrection_Click(object sender, EventArgs e)
        {
            var dt = DateTime.Now;
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.Machine] = "点击水校正启动按钮"
            }, dt);

            var result = await SmartColor.My_AutomaticModule.WaterCorrectionRobotTask.EnqueueWaterCorrectionAsync();

            switch (result.Code)
            {
                case My_Tool.Result.ResultCode.Success:
                    MessageEventManager.Instance.RequestShowBalloonTip(result.Message ?? "水校正成功！");
                    break;
                case My_Tool.Result.ResultCode.Failure:
                    MessageEventManager.Instance.RequestShowMessage(
                        "校正失败",
                        result.Message ?? "加水误差过大或设备异常，是否重试？",
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
                    My_File.Logger.Error("水校正异常", result.Exception);
                    break;
                case My_Tool.Result.ResultCode.Canceled:
                    MessageEventManager.Instance.RequestShowBalloonTip(result.Message ?? "水校正任务已被取消");
                    break;
            }
        }

        /// <summary>
        /// 验证启动按钮点击事件
        /// 业务流程：
        ///   1. 校验输入的验证重量
        ///   2. 调用业务层水验证流程
        ///   3. 根据返回值进行UI提示和交互
        /// 返回值说明（业务层EnqueueWaterVerifyAsync）：
        ///   Code=0  —— 验证成功
        ///   Code>0  —— 业务失败（如天平异常、加水失败等）
        ///   Code=-1 —— 代码异常
        ///   Code=-2 —— 任务被取消
        /// </summary>
        private async void BtnRecheck_Click(object sender, EventArgs e)
        {
           
            if (!int.TryParse(TxtRecheckWeight.Text, out int recheckWeight) || recheckWeight <= 0)
            {
                LocalTranslator.ShowMessage("请输入有效的验证重量,仅支持正整数！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var dt = DateTime.Now;
            _ = RunTableMan.InsertAsync(new Dictionary<string, object>
            {
                [SmartColor.My_DataBase.RUN_TABLE.Machine] = "点击水验证启动按钮"
            },dt);

            var result = await SmartColor.My_AutomaticModule.WaterCorrectionRobotTask.EnqueueWaterVerifyAsync(false, 9999, recheckWeight);
            switch (result.Code)
            {
                case My_Tool.Result.ResultCode.Success:
                    MessageEventManager.Instance.RequestShowBalloonTip( "水验证成功！");
                    break;
                case My_Tool.Result.ResultCode.Failure:
                    MessageEventManager.Instance.RequestShowMessage(
                        "验证失败",
                        result.Message ?? "加水流程失败或设备异常，是否重试？",
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
                    My_File.Logger.Error("水验证异常",result.Exception);
                    break;
                case My_Tool.Result.ResultCode.Canceled:
                    MessageEventManager.Instance.RequestShowBalloonTip( "水验证任务已被取消");
                    break;
            }
           
        }
    }
}