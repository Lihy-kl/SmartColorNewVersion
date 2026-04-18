using System;
using System.IO.Ports;
using System.Threading;
using SmartColor.My_File;

namespace SmartColor.My_Interaction
{
    /// <summary>
    /// 串口Modbus RTU协议底层接口，集成日志，支持自动重连、重试、并发安全、异常码解析。
    /// 支持实例标识，便于多实例日志区分和排查问题。
    /// </summary>
    internal class ModbusRTU : IDisposable
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
        /// <summary>并发锁，保证串口操作线程安全</summary>
        private readonly object _lock = new object();
        /// <summary>读取超时时间（毫秒）</summary>
        public int ReadTimeout { get; set; } = 1000;
        /// <summary>写入超时时间（毫秒）</summary>
        public int WriteTimeout { get; set; } = 1000;
        /// <summary>帧间延时（毫秒），用于发送和接收之间的等待</summary>
        public int FrameDelay { get; set; } = 100;
        /// <summary>操作最大重试次数</summary>
        public int MaxRetry { get; set; } = 3;
        /// <summary>重连延时（毫秒），用于串口打开失败后的等待</summary>
        public int ReconnectDelay { get; set; } = 1000;
        /// <summary>串口最大重连次数</summary>
        public int MaxReconnect { get; set; } = 3;
        /// <summary>串口是否已连接</summary>
        public bool IsConnected => _serialPort != null && _serialPort.IsOpen;
        /// <summary>最近一次错误信息</summary>
        public string LastError { get; private set; }
        /// <summary>实例标识（建议唯一），用于日志区分</summary>
        private readonly string _instanceName;

        /// <summary>
        /// 构造函数，初始化串口参数和实例标识
        /// </summary>
        /// <param name="portName">串口端口名</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="parity">校验位</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="instanceName">实例标识（建议唯一）</param>
        public ModbusRTU(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, string instanceName = null)
        {
            _portName = portName;
            _baudRate = baudRate;
            _parity = parity;
            _dataBits = dataBits;
            _stopBits = stopBits;
            _instanceName = string.IsNullOrWhiteSpace(instanceName) ? Guid.NewGuid().ToString() : instanceName;
            InitSerialPort();
            Logger.Info($"[{_instanceName}] ModbusRTU实例化，端口:{portName}, 波特率:{baudRate}, 数据位:{dataBits}, 停止位:{stopBits}, 校验:{parity}");
        }

        /// <summary>
        /// 初始化串口对象
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
        /// 打开串口，支持自动重连
        /// </summary>
        public void Open()
        {
            lock (_lock)
            {
                int retry = 0;
                while (retry < MaxReconnect)
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
                        return;
                    }
                    catch (Exception ex)
                    {
                        LastError = ex.Message;
                        Logger.Error($"[{_instanceName}] 打开串口[{_portName}]失败", ex);
                        Thread.Sleep(ReconnectDelay);
                        retry++;
                    }
                }
                throw new Exception($"[{_instanceName}] 串口[{_portName}]打开失败，已重试{MaxReconnect}次");
            }
        }

        /// <summary>
        /// 关闭串口
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
        /// 释放串口资源
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
        ~ModbusRTU()
        {
            Dispose();
        }

        /// <summary>
        /// 通用串口操作重试
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="action">操作委托</param>
        /// <param name="logTag">日志标签</param>
        /// <returns>操作结果</returns>
        private T ExecuteWithRetry<T>(Func<T> action, string logTag)
        {
            int retry = 0;
            while (retry < MaxRetry)
            {
                try
                {
                    lock (_lock)
                    {
                        return action();
                    }
                }
                catch (TimeoutException ex)
                {
                    LastError = ex.Message;
                    Logger.Error($"[{_instanceName}] {logTag}超时重试({retry + 1}/{MaxRetry})", ex);
                    Thread.Sleep(FrameDelay);
                    retry++;
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                    Logger.Error($"[{_instanceName}] {logTag}异常重试({retry + 1}/{MaxRetry})", ex);
                    Thread.Sleep(FrameDelay);
                    retry++;
                }
            }
            throw new Exception($"[{_instanceName}] {logTag}失败，已重试{MaxRetry}次");
        }

        /// <summary>
        /// 读线圈（功能码01）
        /// </summary>
        /// <param name="slaveId">从站地址</param>
        /// <param name="startAddr">起始地址</param>
        /// <param name="count">读取数量</param>
        /// <returns>返回bool数组，每个元素表示对应线圈的状态</returns>
        public bool[] ReadCoils(byte slaveId, ushort startAddr, ushort count)
        {
            return ExecuteWithRetry(() =>
            {
                Logger.Info($"[{_instanceName}] 读线圈: 站号={slaveId}, 起始地址={startAddr}, 数量={count}");
                byte[] frame = BuildReadFrame(slaveId, 0x01, startAddr, count);
                Logger.Info($"[{_instanceName}] 发送帧: {BitConverter.ToString(frame)}");
                _serialPort.DiscardInBuffer();
                _serialPort.Write(frame, 0, frame.Length);

                Thread.Sleep(FrameDelay);

                int byteCount = (count + 7) / 8;
                byte[] buffer = new byte[5 + byteCount];
                int read = _serialPort.Read(buffer, 0, buffer.Length);

                Logger.Info($"[{_instanceName}] 接收帧: {BitConverter.ToString(buffer, 0, read)}");

                CheckModbusException(buffer, buffer.Length);

                if (read < buffer.Length)
                    throw new Exception("响应长度不足");

                if (!CheckCRC(buffer, buffer.Length))
                    throw new Exception("CRC校验失败");

                bool[] result = new bool[count];
                for (int i = 0; i < count; i++)
                {
                    int byteIndex = 3 + i / 8;
                    int bitIndex = i % 8;
                    result[i] = ((buffer[byteIndex] >> bitIndex) & 0x01) == 1;
                }
                Logger.Info($"[{_instanceName}] 读线圈结果: {string.Join(",", result)}");
                return result;
            }, "读线圈");
        }

        /// <summary>
        /// 写单个线圈（功能码05）
        /// </summary>
        /// <param name="slaveId">从站地址</param>
        /// <param name="addr">线圈地址</param>
        /// <param name="value">写入值</param>
        /// <returns>返回true表示写入成功，false表示失败</returns>
        public bool WriteSingleCoil(byte slaveId, ushort addr, bool value)
        {
            return ExecuteWithRetry(() =>
            {
                Logger.Info($"[{_instanceName}] 写单线圈: 站号={slaveId}, 地址={addr}, 值={value}");
                byte[] frame = new byte[8];
                frame[0] = slaveId;
                frame[1] = 0x05;
                frame[2] = (byte)(addr >> 8);
                frame[3] = (byte)(addr & 0xFF);
                frame[4] = value ? (byte)0xFF : (byte)0x00;
                frame[5] = 0x00;
                var crc = CRC16(frame, 6);
                frame[6] = crc[0];
                frame[7] = crc[1];

                Logger.Info($"[{_instanceName}] 发送帧: {BitConverter.ToString(frame)}");
                _serialPort.DiscardInBuffer();
                _serialPort.Write(frame, 0, frame.Length);

                Thread.Sleep(FrameDelay);

                byte[] buffer = new byte[8];
                int read = _serialPort.Read(buffer, 0, buffer.Length);

                Logger.Info($"[{_instanceName}] 接收帧: {BitConverter.ToString(buffer, 0, read)}");

                CheckModbusException(buffer, buffer.Length);

                if (read < buffer.Length)
                    throw new Exception("响应长度不足");

                if (!CheckCRC(buffer, buffer.Length))
                    throw new Exception("CRC校验失败");

                for (int i = 0; i < 6; i++)
                {
                    if (buffer[i] != frame[i])
                        return false;
                }
                Logger.Info($"[{_instanceName}] 写单线圈成功");
                return true;
            }, "写单线圈");
        }

        /// <summary>
        /// 批量写线圈（功能码15）
        /// </summary>
        /// <param name="slaveId">从站地址</param>
        /// <param name="startAddr">起始地址</param>
        /// <param name="values">要写入的线圈状态数组</param>
        /// <returns>返回true表示写入成功，false表示失败</returns>
        public bool WriteMultipleCoils(byte slaveId, ushort startAddr, bool[] values)
        {
            return ExecuteWithRetry(() =>
            {
                Logger.Info($"[{_instanceName}] 批量写线圈: 站号={slaveId}, 起始地址={startAddr}, 数量={values.Length}");
                int count = values.Length;
                int byteCount = (count + 7) / 8;
                byte[] coilBytes = new byte[byteCount];
                for (int i = 0; i < count; i++)
                {
                    if (values[i])
                        coilBytes[i / 8] |= (byte)(1 << (i % 8));
                }

                byte[] frame = new byte[9 + byteCount];
                frame[0] = slaveId;
                frame[1] = 0x0F;
                frame[2] = (byte)(startAddr >> 8);
                frame[3] = (byte)(startAddr & 0xFF);
                frame[4] = (byte)(count >> 8);
                frame[5] = (byte)(count & 0xFF);
                frame[6] = (byte)byteCount;
                Array.Copy(coilBytes, 0, frame, 7, byteCount);

                var crc = CRC16(frame, 7 + byteCount);
                frame[7 + byteCount] = crc[0];
                frame[8 + byteCount] = crc[1];

                Logger.Info($"[{_instanceName}] 发送帧: {BitConverter.ToString(frame)}");
                _serialPort.DiscardInBuffer();
                _serialPort.Write(frame, 0, frame.Length);

                Thread.Sleep(FrameDelay);

                byte[] buffer = new byte[8];
                int read = _serialPort.Read(buffer, 0, buffer.Length);

                Logger.Info($"[{_instanceName}] 接收帧: {BitConverter.ToString(buffer, 0, read)}");

                CheckModbusException(buffer, buffer.Length);

                if (read < buffer.Length)
                    throw new Exception("响应长度不足");

                if (!CheckCRC(buffer, buffer.Length))
                    throw new Exception("CRC校验失败");

                for (int i = 0; i < 6; i++)
                {
                    if (buffer[i] != frame[i])
                        return false;
                }
                Logger.Info($"[{_instanceName}] 批量写线圈成功");
                return true;
            }, "批量写线圈");
        }

        /// <summary>
        /// 读保持寄存器（功能码03）
        /// </summary>
        /// <param name="slaveId">从站地址</param>
        /// <param name="startAddr">起始地址</param>
        /// <param name="count">读取数量</param>
        /// <returns>返回ushort数组，每个元素表示对应寄存器的值</returns>
        public ushort[] ReadHoldingRegisters(byte slaveId, ushort startAddr, ushort count)
        {
            return ExecuteWithRetry(() =>
            {
                Logger.Info($"[{_instanceName}] 读保持寄存器: 站号={slaveId}, 起始地址={startAddr}, 数量={count}");
                byte[] frame = BuildReadFrame(slaveId, 0x03, startAddr, count);
                Logger.Info($"[{_instanceName}] 发送帧: {BitConverter.ToString(frame)}");
                _serialPort.DiscardInBuffer();
                _serialPort.Write(frame, 0, frame.Length);

                Thread.Sleep(FrameDelay);

                byte[] buffer = new byte[5 + 2 * count];
                int read = _serialPort.Read(buffer, 0, buffer.Length);

                Logger.Info($"[{_instanceName}] 接收帧: {BitConverter.ToString(buffer, 0, read)}");

                CheckModbusException(buffer, buffer.Length);

                if (read < buffer.Length)
                    throw new Exception("响应长度不足");

                if (!CheckCRC(buffer, buffer.Length))
                    throw new Exception("CRC校验失败");

                ushort[] result = new ushort[count];
                for (int i = 0; i < count; i++)
                {
                    result[i] = (ushort)(buffer[3 + i * 2] << 8 | buffer[4 + i * 2]);
                }
                Logger.Info($"[{_instanceName}] 读保持寄存器结果: {string.Join(",", result)}");
                return result;
            }, "读保持寄存器");
        }

        /// <summary>
        /// 写单个保持寄存器（功能码06）
        /// </summary>
        /// <param name="slaveId">从站地址</param>
        /// <param name="addr">寄存器地址</param>
        /// <param name="value">写入值</param>
        /// <returns>返回true表示写入成功，false表示失败</returns>
        public bool WriteSingleRegister(byte slaveId, ushort addr, ushort value)
        {
            return ExecuteWithRetry(() =>
            {
                Logger.Info($"[{_instanceName}] 写单寄存器: 站号={slaveId}, 地址={addr}, 值={value}");
                byte[] frame = new byte[8];
                frame[0] = slaveId;
                frame[1] = 0x06;
                frame[2] = (byte)(addr >> 8);
                frame[3] = (byte)(addr & 0xFF);
                frame[4] = (byte)(value >> 8);
                frame[5] = (byte)(value & 0xFF);
                var crc = CRC16(frame, 6);
                frame[6] = crc[0];
                frame[7] = crc[1];

                Logger.Info($"[{_instanceName}] 发送帧: {BitConverter.ToString(frame)}");
                _serialPort.DiscardInBuffer();
                _serialPort.Write(frame, 0, frame.Length);

                Thread.Sleep(FrameDelay);

                byte[] buffer = new byte[8];
                int read = _serialPort.Read(buffer, 0, buffer.Length);

                Logger.Info($"[{_instanceName}] 接收帧: {BitConverter.ToString(buffer, 0, read)}");

                CheckModbusException(buffer, buffer.Length);

                if (read < buffer.Length)
                    throw new Exception("响应长度不足");

                if (!CheckCRC(buffer, buffer.Length))
                    throw new Exception("CRC校验失败");

                for (int i = 0; i < 6; i++)
                {
                    if (buffer[i] != frame[i])
                        return false;
                }
                Logger.Info($"[{_instanceName}] 写单寄存器成功");
                return true;
            }, "写单寄存器");
        }

        /// <summary>
        /// 批量写保持寄存器（功能码16）
        /// </summary>
        /// <param name="slaveId">从站地址</param>
        /// <param name="startAddr">起始地址</param>
        /// <param name="values">要写入的寄存器值数组</param>
        /// <returns>返回true表示写入成功，false表示失败</returns>
        public bool WriteMultipleRegisters(byte slaveId, ushort startAddr, ushort[] values)
        {
            return ExecuteWithRetry(() =>
            {
                Logger.Info($"[{_instanceName}] 批量写寄存器: 站号={slaveId}, 起始地址={startAddr}, 数量={values.Length}");
                int count = values.Length;
                int byteCount = count * 2;
                byte[] frame = new byte[9 + byteCount];
                frame[0] = slaveId;
                frame[1] = 0x10;
                frame[2] = (byte)(startAddr >> 8);
                frame[3] = (byte)(startAddr & 0xFF);
                frame[4] = (byte)(count >> 8);
                frame[5] = (byte)(count & 0xFF);
                frame[6] = (byte)byteCount;
                for (int i = 0; i < count; i++)
                {
                    frame[7 + i * 2] = (byte)(values[i] >> 8);
                    frame[8 + i * 2] = (byte)(values[i] & 0xFF);
                }
                var crc = CRC16(frame, 7 + byteCount);
                frame[7 + byteCount] = crc[0];
                frame[8 + byteCount] = crc[1];

                Logger.Info($"[{_instanceName}] 发送帧: {BitConverter.ToString(frame)}");
                _serialPort.DiscardInBuffer();
                _serialPort.Write(frame, 0, frame.Length);

                Thread.Sleep(FrameDelay);

                byte[] buffer = new byte[8];
                int read = _serialPort.Read(buffer, 0, buffer.Length);

                Logger.Info($"[{_instanceName}] 接收帧: {BitConverter.ToString(buffer, 0, read)}");

                CheckModbusException(buffer, buffer.Length);

                if (read < buffer.Length)
                    throw new Exception("响应长度不足");

                if (!CheckCRC(buffer, buffer.Length))
                    throw new Exception("CRC校验失败");

                for (int i = 0; i < 6; i++)
                {
                    if (buffer[i] != frame[i])
                        return false;
                }
                Logger.Info($"[{_instanceName}] 批量写寄存器成功");
                return true;
            }, "批量写寄存器");
        }

        /// <summary>
        /// 构建Modbus读帧
        /// </summary>
        /// <param name="slaveId">从站地址</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="startAddr">起始地址</param>
        /// <param name="count">数量</param>
        /// <returns>返回完整的Modbus RTU请求帧</returns>
        private byte[] BuildReadFrame(byte slaveId, byte functionCode, ushort startAddr, ushort count)
        {
            byte[] frame = new byte[8];
            frame[0] = slaveId;
            frame[1] = functionCode;
            frame[2] = (byte)(startAddr >> 8);
            frame[3] = (byte)(startAddr & 0xFF);
            frame[4] = (byte)(count >> 8);
            frame[5] = (byte)(count & 0xFF);
            var crc = CRC16(frame, 6);
            frame[6] = crc[0];
            frame[7] = crc[1];
            return frame;
        }

        /// <summary>
        /// 校验Modbus RTU帧的CRC16
        /// </summary>
        /// <param name="data">数据帧</param>
        /// <param name="length">数据长度</param>
        /// <returns>校验通过返回true，否则false</returns>
        private bool CheckCRC(byte[] data, int length)
        {
            var crc = CRC16(data, length - 2);
            bool ok = crc[0] == data[length - 2] && crc[1] == data[length - 1];
            if (!ok)
                Logger.Error($"[{_instanceName}] CRC校验失败: 数据={BitConverter.ToString(data, 0, length)}");
            return ok;
        }

        /// <summary>
        /// 检查Modbus异常响应
        /// </summary>
        /// <param name="buffer">响应数据</param>
        /// <param name="length">数据长度</param>
        private void CheckModbusException(byte[] buffer, int length)
        {
            if (length >= 5 && (buffer[1] & 0x80) != 0)
            {
                byte exceptionCode = buffer[2];
                string msg = $"Modbus异常响应: 功能码={buffer[1]}, 异常码={exceptionCode}";
                Logger.Error($"[{_instanceName}] {msg}");
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Modbus RTU CRC16校验
        /// </summary>
        /// <param name="data">数据帧</param>
        /// <param name="length">数据长度</param>
        /// <returns>返回2字节CRC校验码</returns>
        private byte[] CRC16(byte[] data, int length)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    else
                        crc >>= 1;
                }
            }
            return new byte[] { (byte)(crc & 0xFF), (byte)(crc >> 8) };
        }
    }
}