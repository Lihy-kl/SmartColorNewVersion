using com.google.zxing;
using SmartColor.My_Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_File
{
    /// <summary>
    /// 简单日志组件，支持写入文本文件
    /// </summary>
    public static  class Logger
    {
        private static  readonly object _lock = new object();
        private static  string _logDirectory = AppDomain.CurrentDomain.BaseDirectory + "Logs";
        private static  string _logFile => Path.Combine(_logDirectory, DateTime.Now.ToString("yyyyMMdd") + ".log");
        private static  DateTime _lastCleanDate = DateTime.MinValue;
        private static  int _logRetentionDays = 7;
        /// <summary>
        /// 写入信息日志
        /// </summary>
        public static  void Info(string message)
        {
            if (My_ConPar.Machine.OpenLog == 0)
                return; // 如果未开启日志，则直接返回
            WriteLog("INFO", message);

        }

        /// <summary>
        /// 写入错误日志
        /// </summary>
        public static  void Error(string message, Exception ex = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine(message);
            if (ex != null)
            {
                sb.AppendLine("Exception: " + ex.Message);
                sb.AppendLine("StackTrace: " + ex.StackTrace);
            }
            WriteLog("ERROR", sb.ToString());
            try
            {
                MessageEventManager.Instance.RequestShowBalloonTip(message);
                AlarmTableMan.Insert(new Dictionary<string, object>
                {
                    [SmartColor.My_DataBase.ALARM_TABLE.AlarmHead] = message,
                    [SmartColor.My_DataBase.ALARM_TABLE.AlarmDetails] = ex != null ? $"Exception: {ex.Message}\r\nStackTrace: {ex.StackTrace}" : "No additional details"
                });
            }
            catch
            {
                // 气泡提示失败时静默处理
            }
        }

        /// <summary>
        /// 写入日志到文件，并在新的一天自动清理过期日志
        /// </summary>
        private static  void WriteLog(string level, string message)
        {
            try
            {
                lock (_lock)
                {
                    if (!Directory.Exists(_logDirectory))
                        Directory.CreateDirectory(_logDirectory);

                    // 每天只清理一次
                    if (_lastCleanDate.Date != DateTime.Now.Date)
                    {
                        CleanOldLogs(_logRetentionDays); // 保留最近7天
                        _lastCleanDate = DateTime.Now.Date;
                    }

                    string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}\r\n";
                    File.AppendAllText(_logFile, log, Encoding.UTF8);

                }
            }
            catch
            {
                // 日志写入失败时静默处理，防止影响主流程
            }
        }

        /// <summary>
        /// 清理过期日志文件，只保留最近指定天数的日志
        /// </summary>
        /// <param name="days">要保留的天数（如7表示保留最近7天）</param>
        public static  void CleanOldLogs(int days = 7)
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                    return;

                var files = Directory.GetFiles(_logDirectory, "*.log");
                var expireDate = DateTime.Now.AddDays(-days);

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < expireDate)
                    {
                        try { fileInfo.Delete(); } catch { /* 忽略单个文件删除异常 */ }
                    }
                }
            }
            catch
            {
                // 清理失败时静默处理
            }
        }
    }
}