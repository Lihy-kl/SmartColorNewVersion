using System;
using System.Threading.Tasks;

namespace SmartColor.My_Tool
{
    /// <summary>
    /// 全局消息事件管理器
    /// 负责统一管理消息弹窗和气泡提示事件，采用单例模式，
    /// 便于在应用程序各处统一触发和订阅消息相关事件。
    /// </summary>
    public class MessageEventManager
    {
        // 单例实例，确保全局只有一个事件管理器对象
        private static  readonly MessageEventManager _instance = new MessageEventManager();

        /// <summary>
        /// 获取全局唯一的事件管理器实例
        /// </summary>
        public static  MessageEventManager Instance => _instance;

        // 私有构造函数，防止外部实例化
        private MessageEventManager() { }

        /// <summary>
        /// 显示消息事件
        /// 参数说明：
        ///   title   —— 消息框标题
        ///   content —— 消息内容
        ///   callback—— 用户点击按钮后的回调（参数为按钮文本）
        ///   buttons —— 按钮文本数组（如“确定”、“取消”等）
        /// 用于UI层弹窗或交互提示
        /// </summary>
        public event Action<string, string, Action<string>, string[],string> ShowMessageRequested;

        /// <summary>
        /// 显示气泡/Toast提示事件
        /// 参数说明：
        ///   message —— 气泡提示内容
        /// 用于UI层显示非阻塞式提示
        /// </summary>
        public event Action<string> ShowBalloonTip;     

        /// <summary>
        /// 语音循环播报事件（key区分，text内容，intervalMs间隔）
        /// </summary>
        public event Action<string, string> ShowLoopSpeakMessageRequested;
        /// <summary>
        /// 停止语音循环播报事件
        /// </summary>
        public event Action<string> CloseLoopSpeakMessageRequested;


        /// <summary>
        /// 触发消息弹窗事件
        /// 调用此方法会通知所有订阅者显示消息弹窗
        /// </summary>
        /// <param name="title">消息框标题</param>
        /// <param name="content">消息内容</param>
        /// <param name="callback">用户点击按钮后的回调（参数为按钮文本）</param>
        /// <param name="buttons">按钮文本数组</param>
        /// <param name="defaultButton">默认按钮</param>
        public void RequestShowMessage(string title, string content, Action<string> callback, string[] buttons, string defaultButton = null)
        {
            // 传递defaultButton到UI层
            ShowMessageRequested?.Invoke(title, content, callback, buttons, defaultButton);
        }

        /// <summary>
        /// 触发气泡提示事件
        /// 调用此方法会通知所有订阅者显示气泡/Toast提示
        /// </summary>
        /// <param name="message">气泡提示内容</param>
        public void RequestShowBalloonTip(string message)
        {
            ShowBalloonTip?.Invoke(message);
        }

		/// <summary>
		/// 异步弹窗，等待用户操作
		/// </summary>
		public Task<string> RequestShowMessageAsync(string title, string content, string[] buttons, string defaultButton = null)
		{
			var tcs = new TaskCompletionSource<string>();

			// 触发事件，UI 层需调用 tcs.SetResult(btnText)
			ShowMessageRequested?.Invoke(title, content, btn =>
			{
				tcs.TrySetResult(btn);
			}, buttons, defaultButton);

			return tcs.Task;
		}

        /// <summary>
        /// 触发语音循环播报
        /// </summary>
        public void RequestLoopSpeak(string key, string text)
        {
          
            ShowLoopSpeakMessageRequested?.Invoke(key, text); // 新增：弹出无按钮提示
        }


        /// <summary>
        /// 触发停止语音循环播报
        /// </summary>
        public void RequestStopLoopSpeak(string key)
        {
           
            CloseLoopSpeakMessageRequested?.Invoke(key); // 新增：关闭无按钮提示
        }
    }
}