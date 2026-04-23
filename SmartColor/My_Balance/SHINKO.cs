using System.Text;
using System.Threading;
using System;

namespace Lib_SerialPort.Balance
{
    public class SHINKO : SerialPortBase
    {
        public SHINKO():base() { }
        

        /// <summary>
        /// 日本新光电子秤读数
        /// </summary>
        public static double BalanceValue { get; set; }

        /// <summary>
        /// 读数据
        /// 异常：
        ///      1：9999 指令异常
        ///      2：8888 数据异常
        ///      3：6666 通讯异常
        /// </summary>
        /// <param name="buffer"></param>
        public void WriteAndRead()
        {
            byte[] buffer = { 0X4F, 0X38, 0X0D, 0X0A };
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

                    if (datapool[0] == 0X15) //指令异常
                        BalanceValue = 9999;
                    else if (datapool[13] == 0X45) //数据异常
                        BalanceValue = 8888;
                    else
                    {
                        if (datapool.Count == 16)
                        {
                            byte[] bytes = new byte[11];
                            for (int i = 0; i < 9; i++)
                                bytes[i] = datapool[i + 1];

                            BalanceValue = Convert.ToDouble(Encoding.ASCII.GetString(bytes, 0, 9));
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
