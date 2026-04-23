using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lib_SerialPort.Balance
{
    /// <summary>
    /// 梅特勒天平
    /// </summary>
    public class METTLER : SerialPortBase
    {
        public METTLER():base() { }
        

        /// <summary>
        /// 梅特勒天平天平读数
        /// </summary>
        public static double BalanceValue { get; set; }

        /// <summary>
        /// 梅特勒天平天平读数
        /// </summary>
        public static bool bZeroSign { get; set; }

        public static bool bReSetSign { get; set; }

        /// <summary>
        /// 复位（TAC），把皮重去掉，正常测量值
        /// </summary>
        /// <param name="buffer"></param>
        public void Reset()
        {

            byte[] buffer = { 0X54,0X41,0X43, 0X0D, 0X0A };
            bSuccess = false;
            datapool.Clear();

            Write(buffer);
            //需要读取字节数

            ManualResetEvent P_mre = new ManualResetEvent(false);

            Thread P_thd_time = new Thread(() =>
            {
                bool P_bl = true;
                Thread P_thd_time_1 = new Thread(() =>
                {
                    while (P_bl)
                    {
                        //获取结束符0D 0A
                        if (datapool.Count > 2 && datapool[datapool.Count-2] == 0X0D && datapool[datapool.Count-1] == 0X0A)
                        {
                            P_mre.Set();
                            break;
                        }
                        Thread.Sleep(1);
                    }
                });
                P_thd_time_1.Start();
                // 设置超时等待时间
                if (P_mre.WaitOne(2000))
                {
                    P_mre.Reset();
                    return;
                }
                else
                {
                    // 等待超时
                    P_bl = false;
                    P_thd_time_1.Join();

                    ////通讯失败
                    //BalanceValue = 6666;
                }
            });

            // 启动线程
            P_thd_time.Start();
            P_thd_time.Join();


            Thread.Sleep(50);
        }

        /// <summary>
        /// 称皮重(T)，把显示屏当前值改为0
        /// </summary>
        /// <param name="buffer"></param>
        public void Zero()
        {

            byte[] buffer = { 0X54, 0X0D, 0X0A };
            bSuccess = false;
            datapool.Clear();

            Write(buffer);
            //需要读取字节数

            ManualResetEvent P_mre = new ManualResetEvent(false);

            Thread P_thd_time = new Thread(() =>
            {
                bool P_bl = true;
                Thread P_thd_time_1 = new Thread(() =>
                {
                    while (P_bl)
                    {
                        //获取结束符0D 0A
                        if (datapool.Count > 2 && datapool[datapool.Count-2] == 0X0D && datapool[datapool.Count - 1] == 0X0A)
                        {
                            P_mre.Set();
                            break;
                        }
                        Thread.Sleep(1);
                    }
                });
                P_thd_time_1.Start();
                // 设置超时等待时间
                if (P_mre.WaitOne(2000))
                {
                    P_mre.Reset();
                    return;
                }
                else
                {
                    // 等待超时
                    P_bl = false;
                    P_thd_time_1.Join();

                    ////通讯失败
                    //BalanceValue = 6666;
                }
            });

            // 启动线程
            P_thd_time.Start();
            P_thd_time.Join();


            Thread.Sleep(50);
        }

        /// <summary>
        /// 读数据
        /// 异常：
        ///     1：9999 超上限
        ///     2：8888 超下限
        ///     3：7777 归零异常
        ///     4：6666 通讯异常
        /// </summary>
        /// <param name="buffer"></param>
        public void WriteAndRead()
        {

            byte[] buffer = { 0X53, 0X49, 0X0D, 0X0A };
            bSuccess = false;
            datapool.Clear();

            Write(buffer);
            //需要读取字节数

            ManualResetEvent P_mre = new ManualResetEvent(false);

            Thread P_thd_time = new Thread(() =>
            {
                bool P_bl = true;
                Thread P_thd_time_1 = new Thread(() =>
                {
                    while (P_bl)
                    {
                        //获取结束符0D 0A
                        if (datapool.Count > 2 && datapool[datapool.Count - 2] == 0X0D && datapool[datapool.Count - 1] == 0X0A)
                        {
                            P_mre.Set();
                            break;
                        }
                        Thread.Sleep(1);
                    }
                });
                P_thd_time_1.Start();
                // 设置超时等待时间
                if (P_mre.WaitOne(2000))
                {
                    P_mre.Reset();

                    if (datapool[2] == 0X2B) //超上限
                        BalanceValue = 9999;
                    else if (datapool[2] == 0X2D) //超下限
                        BalanceValue = 8888;
                    else if (datapool[2] == 0X49) //未归零
                        BalanceValue = 7777;
                    else
                    {
                        if(datapool.Count == 18)
                        {
                            byte[] bytes = new byte[11];
                            for(int i = 0; i < 11; i++)
                                bytes[i] = datapool[i+4];
                            try
                            {
                                BalanceValue = Convert.ToDouble(Encoding.ASCII.GetString(bytes, 0, 11));
                            }
                            catch { }
                        }
                    }
                    return;
                }
                else
                {
                    // 等待超时
                    P_bl = false;
                    P_thd_time_1.Join();

                    //通讯失败
                    BalanceValue = 6666;
                }
            });

            // 启动线程
            P_thd_time.Start();
            P_thd_time.Join();


            Thread.Sleep(50);
        }
    }
}
