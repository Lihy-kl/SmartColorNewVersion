using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
namespace Lib_SerialPort
{
    public class SerialPortBase
    {
        #region 静态方法
        /// <summary>
        /// 设置串口号
        /// </summary>
        /// <param name="obj">需要绑定的项的集合（如：ComboBox中项的集合ComboBox.Items）</param>
        public static void SetPortNameValues(IList obj)
        {
            obj.Clear();
            try
            {
                foreach (string str in SerialPort.GetPortNames())
                {
                    obj.Add(str);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("未能查询到串口名称——" + e.ToString());
            }

        }

        /// <summary>
        /// 设置波特率
        /// </summary>
        /// <param name="obj">需要绑定的项的集合（如：ComboBox中项的集合ComboBox.Items）</param>
        public static void SetBauRateValues(IList obj)
        {
            obj.Clear();
            foreach (BaudRates rate in Enum.GetValues(typeof(BaudRates)))
            {
                obj.Add(((int)rate).ToString());
            }
        }

        /// <summary>
        /// 设置数据位
        /// </summary>
        /// <param name="obj">需要绑定的项的集合（如：ComboBox中项的集合ComboBox.Items）</param>
        public static void SetDataBitsValues(IList obj)
        {
            obj.Clear();
            foreach (DataBits databit in Enum.GetValues(typeof(DataBits)))
            {
                obj.Add(((int)databit).ToString());
            }
        }

        /// <summary>
        /// 设置校验位列表
        /// </summary>
        /// <param name="obj">需要绑定的项的集合（如：ComboBox中项的集合ComboBox.Items）</param>
        public static void SetParityValues(IList obj)
        {
            obj.Clear();
            foreach (string str in Enum.GetNames(typeof(Parity)))
            {
                obj.Add(str);
            }
        }

        /// <summary>
        /// 设置停止位
        /// </summary>
        /// <param name="obj">需要绑定的项的集合（如：ComboBox中项的集合ComboBox.Items）</param>
        public static void SetStopBitValues(IList obj)
        {
            obj.Clear();
            foreach (string str in Enum.GetNames(typeof(StopBits)))
            {
                obj.Add(str);
            }
        }
        #endregion

        #region 变量属性
        public event SerialErrorReceivedEventHandler ErrorReceived;

        private readonly SerialPort comPort = new SerialPort();
        private string portName = "COM1";//串口号，默认COM1
        private BaudRates baudRate = BaudRates.BR_9600;//波特率
        private Parity parity = Parity.None;//校验位
        private StopBits stopBits = StopBits.One;//停止位
        private DataBits dataBits = DataBits.Eight;//数据位        


        /// <summary>
        /// 串口号
        /// </summary>
        public string PortName
        {
            get { return portName; }
            set { portName = value; }
        }

        /// <summary>
        /// 波特率
        /// </summary>
        public BaudRates BaudRate
        {
            get { return baudRate; }
            set { baudRate = value; }
        }

        /// <summary>
        /// 奇偶校验位
        /// </summary>
        public Parity Parity
        {
            get { return parity; }
            set { parity = value; }
        }

        /// <summary>
        /// 数据位
        /// </summary>
        public DataBits DataBits
        {
            get { return dataBits; }
            set { dataBits = value; }
        }

        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits StopBits
        {
            get { return stopBits; }
            set { stopBits = value; }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 无参构造函数
        /// </summary>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public SerialPortBase()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
            BoundEvents();
        }

        void BoundEvents()
        {
            comPort.DataReceived += new SerialDataReceivedEventHandler(ComPort_DataReceived);
            comPort.ErrorReceived += new SerialErrorReceivedEventHandler(ComPort_ErrorReceived);
        }

        /// <summary>
        /// 参数构造函数（使用枚举参数构造）
        /// </summary>
        /// <param name="baud">波特率</param>
        /// <param name="par">奇偶校验位</param>
        /// <param name="sBits">停止位</param>
        /// <param name="dBits">数据位</param>
        /// <param name="name">串口号</param>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public SerialPortBase(string name, BaudRates baud, Parity par, DataBits dBits, StopBits sBits)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
            this.portName = name;
            this.baudRate = baud;
            this.parity = par;
            this.dataBits = dBits;
            this.stopBits = sBits;
            BoundEvents();
        }

        /// <summary>
        /// 参数构造函数（使用字符串参数构造）
        /// </summary>
        /// <param name="baud">波特率</param>
        /// <param name="par">奇偶校验位</param>
        /// <param name="sBits">停止位</param>
        /// <param name="dBits">数据位</param>
        /// <param name="name">串口号</param>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public SerialPortBase(string name, string baud, string par, string dBits, string sBits)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
            this.portName = name;
            this.baudRate = (BaudRates)Enum.Parse(typeof(BaudRates), baud);
            this.parity = (Parity)Enum.Parse(typeof(Parity), par);
            this.dataBits = (DataBits)Enum.Parse(typeof(DataBits), dBits);
            this.stopBits = (StopBits)Enum.Parse(typeof(StopBits), sBits);
            BoundEvents();
        }
        #endregion

        #region 事件处理函数
        /// <summary>
        /// 数据仓库
        /// </summary>
        public List<byte> datapool = new List<byte>();//存放接收的所有字节
        public bool bSuccess = false;
        public int nLen = 0;
        /// <summary>
        /// 数据接收处理
        /// </summary>
        void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (comPort.IsOpen)     //判断是否打开串口
            {

                Byte[] receivedData = new Byte[comPort.BytesToRead];        //创建接收字节数组
                comPort.Read(receivedData, 0, receivedData.Length);         //读取数据


                datapool.AddRange(receivedData);

                if (receivedData.Length >= 2)
                {
                    byte[] receivedData1 = new byte[datapool.Count - 2];
                    ushort receivedCRC = BitConverter.ToUInt16(receivedData, receivedData.Length - 2);
                    ushort calculatedCRC = CalculateCRC(datapool);

                    if (receivedCRC == calculatedCRC)
                    {
                        //this.receivedData = receivedData;
                        bSuccess = true;
                    }
                }


            }
            else
            {
                Console.WriteLine("请打开某个串口");
            }
        }

        private ushort CalculateCRC(List<byte> data)
        {
            ushort crc = 0xFFFF;

            for (int j = 0; j < data.Count - 2; j++)
            {
                byte b = data[j];
                crc ^= (ushort)b;

                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }

            return crc;
        }


        /// <summary>
        /// 错误处理函数
        /// </summary>
        void ComPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            ErrorReceived?.Invoke(sender, e);
        }
        #endregion

        #region 串口关闭/打开
        /// <summary>
        /// 端口是否已经打开
        /// </summary>
        public bool IsOpen
        {
            get
            {

                return comPort.IsOpen;
            }
        }

        /// <summary>
        /// 打开端口
        /// </summary>
        /// <returns></returns>
        public void Open()
        {
            if (comPort.IsOpen) comPort.Close();

            comPort.PortName = portName;
            comPort.BaudRate = (int)baudRate;
            comPort.Parity = parity;
            comPort.DataBits = (int)dataBits;
            comPort.StopBits = stopBits;

            comPort.Open();
        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        public void Close()
        {
            if (comPort.IsOpen) comPort.Close();
        }

        /// <summary>
        /// 丢弃来自串行驱动程序的接收和发送缓冲区的数据
        /// </summary>
        public void DiscardBuffer()
        {
            comPort.DiscardInBuffer();
            comPort.DiscardOutBuffer();
        }
        #endregion

        #region 写入数据
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="buffer"></param>
        public void Write(byte[] buffer, int offset, int count)
        {
            lock (this)
            {
                if (!(comPort.IsOpen)) comPort.Open();
                comPort.DiscardOutBuffer();
                comPort.DiscardInBuffer();
                comPort.Write(buffer, offset, count);
            }
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="buffer">写入端口的字节数组</param>
        public void Write(byte[] buffer)
        {
            lock (this)
            {
                if (!(comPort.IsOpen)) comPort.Open();
                comPort.DiscardOutBuffer();
                comPort.DiscardInBuffer();
                comPort.Write(buffer, 0, buffer.Length);
            }
        }
        #endregion
    }

    #region 波特率、数据位的枚举
    /// <summary>
    /// 串口数据位列表（5,6,7,8）
    /// </summary>
    public enum DataBits : int
    {
        Five = 5,
        Six = 6,
        Sevent = 7,
        Eight = 8
    }

    /// <summary>
    /// 串口波特率列表。
    /// 75,110,150,300,600,1200,2400,4800,9600,14400,19200,28800,38400,56000,57600,
    /// 115200,128000,230400,256000
    /// </summary>
    public enum BaudRates : int
    {
        BR_75 = 75,
        BR_110 = 110,
        BR_150 = 150,
        BR_300 = 300,
        BR_600 = 600,
        BR_1200 = 1200,
        BR_2400 = 2400,
        BR_4800 = 4800,
        BR_9600 = 9600,
        BR_14400 = 14400,
        BR_19200 = 19200,
        BR_28800 = 28800,
        BR_38400 = 38400,
        BR_56000 = 56000,
        BR_57600 = 57600,
        BR_115200 = 115200,
        BR_128000 = 128000,
        BR_230400 = 230400,
        BR_256000 = 256000
    }
    #endregion
}



