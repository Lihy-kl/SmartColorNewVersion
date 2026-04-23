using System;
using System.Net.Sockets;
using System.Net;
using SmartColor.My_File;
using System.Threading;

namespace SmartColor.My_Interaction
{
    /// <summary>
    /// Modbus TCP协议通讯类。
    /// 支持连接、断开、收发数据、异常处理、帧组包与解析，并集成日志。
    /// 同时实现常用Modbus功能方法（读线圈、写线圈、读保持寄存器、写保持寄存器等），便于直接调用。
    /// 可通过实例标识区分多实例日志，便于排查问题。
    /// </summary>
    internal class ModbusTCP : IDisposable
    {
        /// <summary>TCP客户端对象，用于与服务器建立连接</summary>
        private TcpClient _client;
        /// <summary>服务器IP地址</summary>
        private readonly string _ip;
        /// <summary>服务器端口号</summary>
        private readonly int _port;
        /// <summary>实例标识（建议唯一），用于日志区分</summary>
        private readonly string _instanceName;
        /// <summary>连接超时时间（毫秒）</summary>
        public int ConnectTimeout { get; set; } = 1000;
        /// <summary>通讯超时时间（毫秒），用于发送和接收</summary>
        public int CommTimeout { get; set; } = 3000;
        /// <summary>最近一次错误信息</summary>
        public string LastError { get; private set; }
        /// <summary>是否已连接</summary>
        public bool IsConnected
        {
            get
            {
                return _client != null && _client.Connected;
            }
        }
        /// <summary>并发锁，保证线程安全</summary>
        private readonly object _lock = new object();
        /// <summary>事务ID，每次请求自增</summary>
        private ushort _transactionId = 0;

        /// <summary>
        /// 构造函数，初始化IP、端口和实例标识
        /// </summary>
        /// <param name="ip">服务器IP地址</param>
        /// <param name="port">服务器端口号</param>
        /// <param name="instanceName">实例标识（建议唯一）</param>
        public ModbusTCP(string ip, int port, string instanceName = null)
        {
            _ip = ip;
            _port = port;
            _instanceName = string.IsNullOrWhiteSpace(instanceName) ? Guid.NewGuid().ToString() : instanceName;
            Logger.Info($"[{_instanceName}] ModbusTCP实例化，IP:{ip}, 端口:{port}");
        }

        /// <summary>
        /// 连接到服务器，自动重连机制
        /// </summary>
        /// <param name="maxRetry">最大重试次数</param>
        /// <param name="retryDelay">重试间隔（毫秒）</param>
        public void Connect(int maxRetry = 3, int retryDelay = 1000)
        {
            lock (_lock)
            {
                int retry = 0;
                while (retry < maxRetry)
                {
                    try
                    {
                        // 每次重试都重新创建TcpClient，避免异步冲突
                        if (_client != null)
                        {
                            _client.Close();
                            _client = null;
                        }
                        _client = new TcpClient();

                        var result = _client.BeginConnect(_ip, _port, null, null);
                        bool success = result.AsyncWaitHandle.WaitOne(ConnectTimeout);
                        if (!success)
                            throw new TimeoutException("TCP连接超时");
                        _client.EndConnect(result);
                        _client.ReceiveTimeout = CommTimeout;
                        _client.SendTimeout = CommTimeout;
                        Logger.Info($"[{_instanceName}] TCP连接成功：{_ip}:{_port}");
                        LastError = null;
                        return;
                    }
                    catch (Exception ex)
                    {
                        LastError = ex.Message;
                        //  Logger.Error($"[{_instanceName}] TCP连接失败({retry + 1}/{maxRetry})：{_ip}:{_port}", ex);
                        retry++;
                        System.Threading.Thread.Sleep(retryDelay);
                    }
                }
                throw new Exception($"[{_instanceName}] TCP连接失败，已重试{maxRetry}次");
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            lock (_lock)
            {
                try
                {
                    if (_client != null)
                    {
                        _client.Close();
                        _client = null;
                        Logger.Info($"[{_instanceName}] TCP连接已断开：{_ip}:{_port}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"[{_instanceName}] TCP断开失败：{_ip}:{_port}", ex);
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">要发送的字节数组</param>
        private void Send(byte[] data)
        {
            const int maxRetry = 5;
            int retry = 0;
            while (true)
            {
                lock (_lock)
                {
                    try
                    {
                        if (!IsConnected)
                        {
                            //  Logger.Error($"[{_instanceName}] 未连接到服务器，尝试自动重连...");
                            Connect();
                        }
                        _client.GetStream().Write(data, 0, data.Length);
                        Logger.Info($"[{_instanceName}] 发送数据：{BitConverter.ToString(data)}");
                        return; // 发送成功，退出
                    }
                    catch (Exception ex)
                    {
                        LastError = ex.Message;
                      //  Logger.Error($"[{_instanceName}] TCP发送数据失败(第{retry + 1}次)", ex);
                        retry++;
                        if (retry >= maxRetry)
                        {
                           // Logger.Error($"[{_instanceName}] TCP发送数据失败，已重试{maxRetry}次", ex);
                            throw; // 超过最大重试次数，抛出异常
                        }
                        // 如果是未连接异常，尝试重连
                        if (ex is InvalidOperationException && ex.Message.Contains("未连接到服务器"))
                        {
                            try
                            {
                                Connect();
                            }
                            catch (Exception connEx)
                            {
                                Logger.Error($"[{_instanceName}] 自动重连失败: {connEx.Message}", connEx);
                            }
                        }
                        Thread.Sleep(50); // 可根据需要调整重试间隔
                    }
                }
            }
        }

        /// <summary>
        /// 动态接收数据，根据MBAP头Length字段自动读取完整响应
        /// </summary>
        /// <returns>完整Modbus TCP响应帧</returns>
        private byte[] ReceiveDynamic()
        {
            lock (_lock)
            {
                try
                {
                    if (!IsConnected)
                        throw new InvalidOperationException("未连接到服务器");
                    var stream = _client.GetStream();

                    // 1. 先读6字节MBAP头
                    byte[] mbap = new byte[6];
                    int read = 0;
                    while (read < 6)
                    {
                        int n = stream.Read(mbap, read, 6 - read);
                        if (n == 0) throw new Exception("TCP连接已关闭(读取MBAP头时)");
                        read += n;
                    }

                    // 2. 解析Length字段
                    int len = (mbap[4] << 8) | mbap[5];
                    if (len <= 0 || len > 260)
                        throw new Exception($"MBAP长度字段异常: {len}");

                    // 3. 读取UnitId+PDU
                    byte[] data = new byte[len];
                    read = 0;
                    while (read < len)
                    {
                        int n = stream.Read(data, read, len - read);
                        if (n == 0) throw new Exception("TCP连接已关闭(读取数据时)");
                        read += n;
                    }

                    // 4. 合并返回
                    byte[] result = new byte[6 + len];
                    Buffer.BlockCopy(mbap, 0, result, 0, 6);
                    Buffer.BlockCopy(data, 0, result, 6, len);

                    Logger.Info($"[{_instanceName}] 接收数据：{BitConverter.ToString(result, 0, result.Length)}");
                    return result;
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                    Logger.Error($"[{_instanceName}] TCP接收数据失败", ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// 发送Modbus TCP请求并接收响应（自动读取完整响应）
        /// </summary>
        /// <param name="request">Modbus TCP请求帧</param>
        /// <returns>Modbus TCP响应帧</returns>
        private byte[] SendRequest(byte[] request)
        {
            lock (_lock)
            {
                // 检查连接状态，不可靠时可尝试重连
                if (_client == null || !_client.Connected)
                {
                    Connect();
                }
                try
                {
                    Send(request);
                    return ReceiveDynamic();
                }
                catch
                {
                    // 发生异常时，尝试断开连接，便于下次重连
                    Disconnect();
                    throw;
                }
            }
        }

        /// <summary>
        /// 组包Modbus TCP请求帧（MBAP头+PDU）
        /// </summary>
        /// <param name="transactionId">事务ID</param>
        /// <param name="unitId">站号</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="data">PDU数据区</param>
        /// <returns>完整Modbus TCP请求帧</returns>
        private static byte[] BuildRequest(ushort transactionId, byte unitId, byte functionCode, byte[] data)
        {
            // MBAP头7字节 + PDU(功能码+数据区)
            byte[] frame = new byte[7 + 1 + data.Length];
            frame[0] = (byte)(transactionId >> 8);
            frame[1] = (byte)(transactionId & 0xFF);
            frame[2] = 0x00;
            frame[3] = 0x00;
            // Length = UnitId(1) + FunctionCode(1) + Data.Length
            int length = 1 + 1 + data.Length;
            frame[4] = (byte)(length >> 8);
            frame[5] = (byte)(length & 0xFF);
            frame[6] = unitId;
            frame[7] = functionCode;
            Array.Copy(data, 0, frame, 8, data.Length);
            return frame;
        }

        /// <summary>
        /// 解析Modbus TCP响应帧（MBAP头+PDU），并处理异常码
        /// </summary>
        /// <param name="response">响应帧</param>
        /// <returns>去除MBAP头后的PDU数据</returns>
        private static byte[] ParseResponse(byte[] response)
        {
            if (response == null || response.Length < 8)
                throw new ArgumentException("响应帧长度不足");
            byte[] pdu = new byte[response.Length - 7];
            Array.Copy(response, 7, pdu, 0, pdu.Length);
            // 检查Modbus异常码
            if ((pdu[0] & 0x80) != 0)
            {
                byte exceptionCode = pdu[1];
                throw new Exception($"Modbus异常响应: 功能码={pdu[0]}, 异常码={exceptionCode}");
            }
            return pdu;
        }

        /// <summary>
        /// 事务ID自增，溢出自动归零
        /// </summary>
        private ushort NextTransactionId()
        {
            if (_transactionId == ushort.MaxValue)
                _transactionId = 0;
            else
                _transactionId++;
            return _transactionId;
        }

        /// <summary>
        /// 读线圈（功能码01）
        /// </summary>
        /// <param name="unitId">站号</param>
        /// <param name="startAddr">起始地址</param>
        /// <param name="count">读取数量</param>
        /// <returns>bool数组，每个元素表示对应线圈的状态</returns>
        public bool[] ReadCoils(byte unitId, ushort startAddr, ushort count)
        {
            if (count < 1 || count > 2000) throw new ArgumentOutOfRangeException(nameof(count), "线圈数量必须在1~2000之间");
            byte[] data = new byte[4];
            data[0] = (byte)(startAddr >> 8);
            data[1] = (byte)(startAddr & 0xFF);
            data[2] = (byte)(count >> 8);
            data[3] = (byte)(count & 0xFF);
            byte[] req = BuildRequest(NextTransactionId(), unitId, 0x01, data);
            byte[] resp = SendRequest(req);
            byte[] pdu = ParseResponse(resp);
            if (pdu[0] != 0x01) throw new Exception("功能码错误");
            int byteCount = pdu[1];
            bool[] result = new bool[count];
            for (int i = 0; i < count; i++)
            {
                int byteIndex = 2 + i / 8;
                int bitIndex = i % 8;
                result[i] = ((pdu[byteIndex] >> bitIndex) & 0x01) == 1;
            }
            Logger.Info($"[{_instanceName}] 读线圈结果: {string.Join(",", result)}");
            return result;
        }

        /// <summary>
        /// 写单个线圈（功能码05）
        /// </summary>
        /// <param name="unitId">站号</param>
        /// <param name="addr">线圈地址</param>
        /// <param name="value">写入值</param>
        /// <returns>写入成功返回true</returns>
        public bool WriteSingleCoil(byte unitId, ushort addr, bool value)
        {
            byte[] data = new byte[4];
            data[0] = (byte)(addr >> 8);
            data[1] = (byte)(addr & 0xFF);
            data[2] = value ? (byte)0xFF : (byte)0x00;
            data[3] = 0x00;
            byte[] req = BuildRequest(NextTransactionId(), unitId, 0x05, data);
            byte[] resp = SendRequest(req);
            byte[] pdu = ParseResponse(resp);
            bool ok = pdu[0] == 0x05 && pdu[1] == data[0] && pdu[2] == data[1] && pdu[3] == data[2] && pdu[4] == data[3];
            Logger.Info($"[{_instanceName}] 写单线圈结果: {ok}");
            Thread.Sleep(10); // 写入后稍作延时，确保数据稳定
            return ok;
        }

        /// <summary>
        /// 批量写线圈（功能码15）
        /// </summary>
        /// <param name="unitId">站号</param>
        /// <param name="startAddr">起始地址</param>
        /// <param name="values">要写入的线圈状态数组</param>
        /// <returns>写入成功返回true</returns>
        public bool WriteMultipleCoils(byte unitId, ushort startAddr, bool[] values)
        {
            int count = values.Length;
            if (count < 1 || count > 1968) throw new ArgumentOutOfRangeException(nameof(count), "批量写线圈数量必须在1~1968之间");
            int byteCount = (count + 7) / 8;
            byte[] coilBytes = new byte[byteCount];
            for (int i = 0; i < count; i++)
            {
                if (values[i])
                    coilBytes[i / 8] |= (byte)(1 << (i % 8));
            }
            byte[] data = new byte[5 + byteCount];
            data[0] = (byte)(startAddr >> 8);
            data[1] = (byte)(startAddr & 0xFF);
            data[2] = (byte)(count >> 8);
            data[3] = (byte)(count & 0xFF);
            data[4] = (byte)byteCount;
            Array.Copy(coilBytes, 0, data, 5, byteCount);
            byte[] req = BuildRequest(NextTransactionId(), unitId, 0x0F, data);
            byte[] resp = SendRequest(req);
            byte[] pdu = ParseResponse(resp);
            bool ok = pdu[0] == 0x0F && pdu[1] == data[0] && pdu[2] == data[1] && pdu[3] == data[2] && pdu[4] == data[3];
            Logger.Info($"[{_instanceName}] 批量写线圈结果: {ok}");
            Thread.Sleep(10); // 写入后稍作延时，确保数据稳定
            return ok;
        }

        /// <summary>
        /// 读保持寄存器（功能码03）
        /// </summary>
        /// <param name="unitId">站号</param>
        /// <param name="startAddr">起始地址</param>
        /// <param name="count">读取数量</param>
        /// <returns>ushort数组，每个元素表示对应寄存器的值</returns>
        public ushort[] ReadHoldingRegisters(byte unitId, ushort startAddr, ushort count)
        {
            if (count < 1 || count > 125) throw new ArgumentOutOfRangeException(nameof(count), "寄存器数量必须在1~125之间");
            byte[] data = new byte[4];
            data[0] = (byte)(startAddr >> 8);
            data[1] = (byte)(startAddr & 0xFF);
            data[2] = (byte)(count >> 8);
            data[3] = (byte)(count & 0xFF);
            byte[] req = BuildRequest(NextTransactionId(), unitId, 0x03, data);
            byte[] resp = SendRequest(req);
            byte[] pdu = ParseResponse(resp);
            if (pdu[0] != 0x03) throw new Exception("功能码错误");
            int byteCount = pdu[1];
            ushort[] result = new ushort[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = (ushort)(pdu[2 + i * 2] << 8 | pdu[3 + i * 2]);
            }
            Logger.Info($"[{_instanceName}] 读保持寄存器结果: {string.Join(",", result)}");
            return result;
        }

        /// <summary>
        /// 写单个保持寄存器（功能码06）
        /// </summary>
        /// <param name="unitId">站号</param>
        /// <param name="addr">寄存器地址</param>
        /// <param name="value">写入值</param>
        /// <returns>写入成功返回true</returns>
        public bool WriteSingleRegister(byte unitId, ushort addr, ushort value)
        {
            const int maxRetry = 3;
            ushort[] readBack = new ushort[0];
            for (int attempt = 0; attempt < maxRetry; attempt++)
            {
                byte[] data = new byte[4];
                data[0] = (byte)(addr >> 8);
                data[1] = (byte)(addr & 0xFF);
                data[2] = (byte)(value >> 8);
                data[3] = (byte)(value & 0xFF);
                byte[] req = BuildRequest(NextTransactionId(), unitId, 0x06, data);
                byte[] resp = SendRequest(req);
                byte[] pdu = ParseResponse(resp);
                bool ok = pdu[0] == 0x06 && pdu[1] == data[0] && pdu[2] == data[1] && pdu[3] == data[2] && pdu[4] == data[3];
                Logger.Info($"[{_instanceName}] 写单寄存器结果: {ok}");
                //  Thread.Sleep(50); // 写入后稍作延时，确保数据稳定

                // 校验写入
                try
                {
                    readBack = ReadHoldingRegisters(unitId, addr, 1);
                    if (readBack.Length == 1 && readBack[0] == value)
                        return true;

                }
                catch (Exception ex)
                {
                    Logger.Error($"[{_instanceName}] 写单寄存器校验异常", ex);
                }
                Thread.Sleep(20);
            }
            Logger.Error($"[{_instanceName}] 写单寄存器校验失败", new Exception($"期望={value}, 实际={readBack[0]}"));
            return false;
        }

        /// <summary>
        /// 批量写保持寄存器（功能码16）
        /// </summary>
        /// <param name="unitId">站号</param>
        /// <param name="startAddr">起始地址</param>
        /// <param name="values">要写入的寄存器值数组</param>
        /// <returns>写入成功返回true</returns>
        public bool WriteMultipleRegisters(byte unitId, ushort startAddr, ushort[] values)
        {
            int count = values.Length;
            if (count < 1 || count > 123) throw new ArgumentOutOfRangeException(nameof(count), "批量写寄存器数量必须在1~123之间");
            const int maxRetry = 3;
            ushort[] readBack = new ushort[0];
            for (int attempt = 0; attempt < maxRetry; attempt++)
            {
                int byteCount = count * 2;
                byte[] data = new byte[5 + byteCount];
                data[0] = (byte)(startAddr >> 8);
                data[1] = (byte)(startAddr & 0xFF);
                data[2] = (byte)(count >> 8);
                data[3] = (byte)(count & 0xFF);
                data[4] = (byte)byteCount;
                for (int i = 0; i < count; i++)
                {
                    data[5 + i * 2] = (byte)(values[i] >> 8);
                    data[6 + i * 2] = (byte)(values[i] & 0xFF);
                }
                byte[] req = BuildRequest(NextTransactionId(), unitId, 0x10, data);
                byte[] resp = SendRequest(req);
                byte[] pdu = ParseResponse(resp);
                bool ok = pdu[0] == 0x10 && pdu[1] == data[0] && pdu[2] == data[1] && pdu[3] == data[2] && pdu[4] == data[3];
                Logger.Info($"[{_instanceName}] 批量写寄存器结果: {ok}");
                //  Thread.Sleep(50); // 写入后稍作延时，确保数据稳定

                // 校验写入
                try
                {
                    readBack = ReadHoldingRegisters(unitId, startAddr, (ushort)count);
                    bool match = readBack.Length == count;
                    for (int i = 0; i < count && match; i++)
                    {
                        if (readBack[i] != values[i])
                            match = false;
                    }
                    if (match)
                        return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"[{_instanceName}] 批量写寄存器校验异常", ex);
                }
                Thread.Sleep(20);
            }
            Logger.Error($"[{_instanceName}] 批量写寄存器校验失败", new Exception($"期望=[{string.Join(",", values)}], 实际=[{string.Join(",", readBack)}]"));

            return false;
        }

        /// <summary>
        /// 释放资源，断开连接
        /// </summary>
        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 析构函数，确保资源释放
        /// </summary>
        ~ModbusTCP()
        {
            Dispose();
        }
    }
}