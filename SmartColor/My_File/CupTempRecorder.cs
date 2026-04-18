using SmartColor.My_AutomaticModule;
using SmartColor.My_ConPar;
using SmartColor.My_Cup;
using SmartColor.My_DataBase;
using SmartColor.My_Form.Homepage;
using SmartColor.My_Tool;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SmartColor.My_File
{
    /// <summary>
    /// 单杯温度记录与操作步骤记录管理器
    /// </summary>
    public class CupTempRecorder
    {
        private static readonly string DataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "APP_DATA");
        private static readonly ConcurrentDictionary<int, CupTempRecorder> _recorders = new ConcurrentDictionary<int, CupTempRecorder>();
        private readonly object _fileLock = new object();
        private readonly int _cupNum;
        private readonly string _tempFile;
        private readonly string _stepFile;
        private readonly List<double> _temps = new List<double>();
        private readonly List<string> _steps = new List<string>();
        private Timer _timer;
        private int _pointIndex = 0;

        // 注入的通讯接口
        private ICylinderComm _cylinderComm;

        /// <summary>
        /// 获取指定杯号的温度记录器（单例）
        /// </summary>
        public static CupTempRecorder Get(int cupNum)
        {
            return _recorders.GetOrAdd(cupNum, n => new CupTempRecorder(n));
        }

        private CupTempRecorder(int cupNum)
        {
            _cupNum = cupNum;
            if (!Directory.Exists(DataDir))
                Directory.CreateDirectory(DataDir);
            _tempFile = Path.Combine(DataDir, $"{cupNum}.txt");
            _stepFile = Path.Combine(DataDir, $"{cupNum}_step.txt");
        }

        /// <summary>
        /// 绑定通讯对象（必须在StartRecord前调用）
        /// </summary>
        public void SetCylinderComm(ICylinderComm comm)
        {
            _cylinderComm = comm;
        }

        /// <summary>
        /// 启动温度记录（染色启动时调用）
        /// </summary>
        public void StartRecord(bool clearHistory = true)
        {
            Task.Run(() =>
            {
                if (clearHistory)
                {
                    lock (_fileLock)
                    {
                        File.WriteAllText(_tempFile, string.Empty, Encoding.UTF8);
                        File.WriteAllText(_stepFile, string.Empty, Encoding.UTF8);
                    }
                    _temps.Clear();
                    _steps.Clear();
                    _pointIndex = 0;
                }
                else
                {
                    // 读取已有温度点数，继续追加
                    if (File.Exists(_tempFile))
                    {
                        var tempStr = File.ReadAllText(_tempFile, Encoding.UTF8);
                        _pointIndex = string.IsNullOrEmpty(tempStr) ? 0 : tempStr.Split('@').Length - 1;
                    }
                    else
                    {
                        _pointIndex = 0;
                    }
                }
                // 延迟首次采集，避免通讯未就绪
                System.Threading.Tasks.Task.Delay(500).ContinueWith(_ => RecordTemperature());

                // 启动定时器
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                }
                _timer = new Timer(My_ConPar.Delay.TemRecordInterval * 1000);
                _timer.Elapsed += (s, e) => RecordTemperature();
                _timer.AutoReset = true;
                _timer.Start();

            });




        }



        /// <summary>
        /// 停止温度记录（可在染色结束时调用）
        /// </summary>
        public void StopRecord()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }

        /// <summary>
        /// 记录一次温度（自动调用）
        /// </summary>
        private void RecordTemperature()
        {
            double temp = GetCurrentTemp(_cupNum);
            int retry = 0;
            const int maxRetry = 2;
            while (temp == 0 && retry < maxRetry)
            {
                // 等待一小段时间再采集，避免瞬时通讯异常
                Task.Delay(10000).Wait();
                temp = GetCurrentTemp(_cupNum);
                retry++;
            }
            // 如果多次采集仍为0，可考虑写入特殊标记或日志
            if (temp == 0)
            {
                // 这里可以写日志，例如
                Logger.Error($"{_cupNum}号杯温度采集多次为0，可能通讯异常");
                return;
            }
            _temps.Add(temp);
            _pointIndex++;
            lock (_fileLock)
            {
                File.AppendAllText(_tempFile, (temp.ToString("F2") + "@"), Encoding.UTF8);
            }
        }

        /// <summary>
        /// 记录操作步骤与温度点（染色过程调用）
        /// </summary>
        /// <param name="cooperate">工艺动作（如“放布”）</param>
        /// <summary>
        /// 记录操作步骤与温度点（染色过程调用）
        /// 新增：传入步骤号stepNum，避免重复插入。
        /// 1. 先检查内存中是否已存在当前温度点标签（_steps）。
        /// 2. 再检查文件中最大步骤号，若stepNum <= maxStepNum则为重复插入。
        /// 3. 满足插入条件则写入文件和内存。
        /// </summary>
        /// <param name="cooperate">工艺动作（如“放布”）</param>

        /// <returns>插入的步骤号</returns>
        public int RecordStep(string cooperate)
        {
            // 1. 检查内存中是否已存在当前温度点标签
            bool hasLabel = _steps.Exists(s =>
            {
                var parts = s.Split(',');
                if (parts.Length < 2) return false;
                var idxStr = parts[1].TrimEnd('@');
                int idx;
                return int.TryParse(idxStr, out idx) && idx == _pointIndex;
            });

            // 2. 检查文件中最大步骤号，避免重复插入
            var cupdt = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {_cupNum}");
            if (cupdt == null || cupdt.Rows.Count == 0)
            {
                My_File.Logger.Error($"DropFinishAsync: 未找到{_cupNum}杯的cup_details数据");
                return 0;
            }
            var cupRow = cupdt.Rows[0];
            int stepNum = cupRow.Table.Columns.Contains(CUP_DETAILS.StepNum) && cupRow[CUP_DETAILS.StepNum] != DBNull.Value
                ? Convert.ToInt32(cupRow[CUP_DETAILS.StepNum])
                : 0;

            int maxStepNum = -1;
            lock (_fileLock)
            {
                if (File.Exists(_stepFile))
                {
                    string fileContent = File.ReadAllText(_stepFile, Encoding.UTF8);
                    var steps = fileContent.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    maxStepNum = steps.Length;
                    // 如果当前步骤号 <= 文件中最大步骤号，则为重复插入
                    if (stepNum <= maxStepNum)
                        return maxStepNum;
                }

                // 3. 满足插入条件则写入文件和内存
                int targetIndex = hasLabel ? _pointIndex + 1 : _pointIndex;
                string step = $"{cooperate},{targetIndex}@";
                _steps.Add(step);
                File.AppendAllText(_stepFile, step, Encoding.UTF8);
                return targetIndex;
            }
        }

        /// <summary>
        /// 获取当前温度（通过ICylinderComm接口）
        /// </summary>
        private double GetCurrentTemp(int cupNum)
        {
            if (_cylinderComm == null)
                return 0;
            return _cylinderComm.GetCupTemp(cupNum);
        }

        /// <summary>
        /// 获取温度数据（Base64加密，返回byte[]，用于存ProcessData字段）
        /// </summary>
        public static byte[] GetProcessData(int cupNum)
        {
            string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "APP_DATA");
            string tempFile = Path.Combine(dataDir, $"{cupNum}.txt");
            if (!File.Exists(tempFile))
                return new byte[0];

            // 读取温度原始字符串
            string tempStr = File.ReadAllText(tempFile, Encoding.UTF8);
            if (string.IsNullOrWhiteSpace(tempStr))
                return new byte[0];

            // Base64加密（返回byte[]，便于存数据库varbinary）
            string base64Str = Base64.Base64Encrypt(tempStr);
            return Encoding.Default.GetBytes(base64Str);
        }

        /// <summary>
        /// 获取步骤数据（原始字符串，存MarkStep字段）
        /// </summary>
        public static string GetMarkStep(int cupNum)
        {
            string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "APP_DATA");
            string stepFile = Path.Combine(dataDir, $"{cupNum}_step.txt");
            if (!File.Exists(stepFile))
                return string.Empty;

            // 读取步骤原始字符串
            string stepStr = File.ReadAllText(stepFile, Encoding.UTF8);
            return stepStr ?? string.Empty;
        }

        /// <summary>
        /// 清空指定杯号的温度和步骤数据（可在染色结束后调用，或开料前调用）
        /// </summary>
        /// <param name="cupNum">杯号</param>
        public static void ClearFiles(int cupNum)
        {
            string tempFile = Path.Combine(DataDir, $"{cupNum}.txt");
            string stepFile = Path.Combine(DataDir, $"{cupNum}_step.txt");
            if (File.Exists(tempFile))
                File.WriteAllText(tempFile, string.Empty, Encoding.UTF8);
            if (File.Exists(stepFile))
                File.WriteAllText(stepFile, string.Empty, Encoding.UTF8);

        }
    }
}