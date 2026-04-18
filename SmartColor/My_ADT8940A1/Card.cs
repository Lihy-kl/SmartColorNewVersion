using Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartColor.My_File;

namespace SmartColor.My_ADT8940A1
{
    /// <summary>
    /// 板卡类
    /// 集成Logger，记录初始化过程及异常，便于排查问题。
    /// </summary>
    internal class Card
    {
        /// <summary>
        /// 板卡对象
        /// </summary>
        public static  Card CurrentBoardCard;

        /// <summary>
        /// 构造函数，初始化板卡并记录日志
        /// </summary>
        public Card()
        {
            Logger.Info("板卡初始化开始");
            try
            {
                int res = Adt8940a1m.adt8940a1_initial();

                switch (res)
                {
                    case 0:
                        throw new Exception("未检查到板卡");

                    case -1:
                        throw new Exception("未安装端口驱动程序");

                    case -2:
                        throw new Exception("PCI桥存在故障");

                    case -3:
                        throw new Exception("拨码开关设置重复");

                    case -4:
                        throw new Exception("拨码开关读取异常");

                    default:
                        //清除缓存
                        if (1 == Adt8940a1m.adt8940a1_reset_fifo(0))
                        {
                            throw new Exception("板卡清除缓存异常");
                        }

                        ////设定脉冲工作方式和加速度
                        //if (-1 != My_ConPar.BoardCardIO.Axis_X)
                        //{
                        //    if (1 == Adt8940a1m.adt8940a1_set_pulse_mode(0, My_ConPar.BoardCardIO.Axis_X, 0, 0, 0))
                        //    {
                        //        throw new Exception("X轴设定脉冲工作方式异常");
                        //    }
                        //    if (1 == Adt8940a1m.adt8940a1_set_acc(0, My_ConPar.BoardCardIO.Axis_X, 50000))
                        //    {
                        //        throw new Exception("X轴设定加速度异常");
                        //    }
                        //}
                        //if (-1 != My_ConPar.BoardCardIO.Axis_Y)
                        //{
                        //    if (1 == Adt8940a1m.adt8940a1_set_pulse_mode(0, My_ConPar.BoardCardIO.Axis_Y, 0, 0, 0))
                        //    {
                        //        throw new Exception("Y轴设定脉冲工作方式异常");
                        //    }
                        //    if (1 == Adt8940a1m.adt8940a1_set_acc(0, My_ConPar.BoardCardIO.Axis_Y, 50000))
                        //    {
                        //        throw new Exception("Y轴设定加速度异常");
                        //    }
                        //}
                        //if (-1 != My_ConPar.BoardCardIO.Axis_Z)
                        //{
                        //    if (1 == Adt8940a1m.adt8940a1_set_pulse_mode(0, My_ConPar.BoardCardIO.Axis_Z, 1, 0, 0))
                        //    {
                        //        throw new Exception("Z轴设定脉冲工作方式异常");
                        //    }
                        //    if (1 == Adt8940a1m.adt8940a1_set_acc(0, My_ConPar.BoardCardIO.Axis_Z, 50000))
                        //    {
                        //        throw new Exception("Z轴设定加速度异常");
                        //    }
                        //}
                        Logger.Info("板卡初始化全部完成");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("板卡初始化异常", ex);
                throw;
            }
        }
    }
}