using SmartColor.My_File;
using System;
using System.IO.Ports;
using System.Text;

namespace SmartColor.My_Interaction
{
    /// <summary>
    /// 自由协议串口通讯类。
    /// 支持发送、接收原始报文和字符串，集成日志，支持实例标识，便于多实例日志区分。
    /// 提供闭环通讯接口（SendAndReceive），上层只需调用闭环方法，其他方法均为私有。
    /// 适用于非标准Modbus等自定义协议场景。
    /// 可扩展协议解析、校验等功能。
    /// </summary>
    internal class FreeAgreement : IDisposable
    {
        /// <summary>串口对象，用于实际的串口通信</summary>
        private SerialPort _serialPort;
        /// <summary>串口端口名（如"COM1"）</summary>
        private readonly string _portName;
        /// <summary>串口波特率</summary>
        private readonly int _baudRate;
        /// <summary>串口校验位</summary>
        private readonly Parity _parity;
        /// <summary>串口数据位</summary>
        private readonly int _dataBits;
        /// <summary>串口停止位</summary>
        private readonly StopBits _stopBits;
        /// <summary>实例标识（建议唯一），用于日志区分</summary>
        private readonly string _instanceName;
        /// <summary>并发锁，保证串口操作线程安全</summary>
        private readonly object _lock = new object();
        /// <summary>读取超时时间（毫秒）</summary>
        public int ReadTimeout { get; set; } = 1000;
        /// <summary>写入超时时间（毫秒）</summary>
        public int WriteTimeout { get; set; } = 1000;
        /// <summary>最近一次错误信息</summary>
        public string LastError { get; private set; }
        /// <summary>串口是否已连接</summary>
        public bool IsConnected => _serialPort != null && _serialPort.IsOpen;

        /// <summary>
        /// 构造函数，初始化串口参数和实例标识
        /// </summary>
        /// <param name="portName">串口端口名</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="parity">校验位</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="instanceName">实例标识（建议唯一）</param>
        public FreeAgreement(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, string instanceName = null)
        {
            _portName = portName;
            _baudRate = baudRate;
            _parity = parity;
            _dataBits = dataBits;
            _stopBits = stopBits;
            _instanceName = string.IsNullOrWhiteSpace(instanceName) ? Guid.NewGuid().ToString() : instanceName;
            InitSerialPort();
            Logger.Info($"[{_instanceName}] FreeAgreement实例化，端口:{portName}, 波特率:{baudRate}, 数据位:{dataBits}, 停止位:{stopBits}, 校验:{parity}");
        }

        /// <summary>
        /// 初始化串口对象，设置参数和超时
        /// </summary>
        private void InitSerialPort()
        {
            _serialPort = new SerialPort(_portName, _baudRate, _parity, _dataBits, _stopBits)
            {
                ReadTimeout = ReadTimeout,
                WriteTimeout = WriteTimeout
            };
        }

        /// <summary>
        /// 打开串口，若未初始化则先初始化
        /// </summary>
        public void Open()
        {
            lock (_lock)
            {
                try
                {
                    if (_serialPort == null)
                        InitSerialPort();
                    if (!_serialPort.IsOpen)
                    {
                        _serialPort.Open();
                        Logger.Info($"[{_instanceName}] 串口[{_portName}]已打开");
                    }
                    LastError = null;
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                    Logger.Error($"[{_instanceName}] 打开串口[{_portName}]失败", ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// 关闭串口，释放资源
        /// </summary>
        public void Close()
        {
            lock (_lock)
            {
                try
                {
                    if (_serialPort != null && _serialPort.IsOpen)
                    {
                        _serialPort.Close();
                        Logger.Info($"[{_instanceName}] 串口[{_portName}]已关闭");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"[{_instanceName}] 关闭串口[{_portName}]失败", ex);
                }
            }
        }

        /// <summary>
        /// 闭环通讯：发送原始报文并接收响应
        /// </summary>
        /// <param name="sendData">要发送的字节数组</param>
        /// <param name="responseLength">期望接收的字节数</param>
        /// <returns>接收到的字节数组</returns>
        public byte[] SendAndReceive(byte[] sendData, int responseLength)
        {
            lock (_lock)
            {
                try
                {
                    if (!IsConnected)
                        throw new InvalidOperationException("串口未打开");
                    _serialPort.DiscardInBuffer();
                    _serialPort.Write(sendData, 0, sendData.Length);
                    Logger.Info($"[{_instanceName}] 闭环发送数据: {BitConverter.ToString(sendData)}");
                    byte[] buffer = new byte[responseLength];
                    int read = _serialPort.Read(buffer, 0, responseLength);
                    if (read < responseLength)
                        Logger.Error($"[{_instanceName}] 闭环接收数据长度不足，期望:{responseLength} 实际:{read}");
                    Logger.Info($"[{_instanceName}] 闭环接收数据: {BitConverter.ToString(buffer, 0, read)}");
                    return buffer;
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                    Logger.Error($"[{_instanceName}] 闭环通讯失败", ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// 闭环通讯：发送字符串并接收响应字符串（ASCII编码）
        /// </summary>
        /// <param name="sendText">要发送的字符串</param>
        /// <param name="responseLength">期望接收的字节数</param>
        /// <returns>接收到的字符串</returns>
        public string SendAndReceiveString(string sendText, int responseLength)
        {
            var respBytes = SendAndReceive(Encoding.ASCII.GetBytes(sendText), responseLength);
            return Encoding.ASCII.GetString(respBytes);
        }

        /// <summary>
        /// 发送原始报文（私有方法，建议只在内部使用）
        /// </summary>
        /// <param name="data">要发送的字节数组</param>
        private void Send(byte[] data)
        {
            if (!IsConnected)
                throw new InvalidOperationException("串口未打开");
            _serialPort.Write(data, 0, data.Length);
            Logger.Info($"[{_instanceName}] 发送数据: {BitConverter.ToString(data)}");
        }

        /// <summary>
        /// 接收原始报文（私有方法，建议只在内部使用）
        /// </summary>
        /// <param name="length">期望接收的字节数</param>
        /// <returns>接收到的字节数组</returns>
        private byte[] Receive(int length)
        {
            if (!IsConnected)
                throw new InvalidOperationException("串口未打开");
            byte[] buffer = new byte[length];
            int read = _serialPort.Read(buffer, 0, length);
            if (read < length)
                Logger.Error($"[{_instanceName}] 接收数据长度不足，期望:{length} 实际:{read}");
            Logger.Info($"[{_instanceName}] 接收数据: {BitConverter.ToString(buffer, 0, read)}");
            return buffer;
        }

        /// <summary>
        /// 接收字符串（私有方法，建议只在内部使用，ASCII编码）
        /// </summary>
        /// <param name="length">期望接收的字节数</param>
        /// <returns>接收到的字符串</returns>
        private string ReceiveString(int length)
        {
            var bytes = Receive(length);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// 释放串口资源，关闭串口并释放对象
        /// </summary>
        public void Dispose()
        {
            lock (_lock)
            {
                try
                {
                    Close();
                    if (_serialPort != null)
                    {
                        _serialPort.Dispose();
                        _serialPort = null;
                        Logger.Info($"[{_instanceName}] 串口[{_portName}]资源已释放");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"[{_instanceName}] 释放串口[{_portName}]资源失败", ex);
                }
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 析构函数，确保资源释放
        /// </summary>
        ~FreeAgreement()
        {
            Dispose();
        }
    }
}