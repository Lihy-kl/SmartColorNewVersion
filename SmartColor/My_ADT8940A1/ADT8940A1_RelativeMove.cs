using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartColor.My_ADT8940A1
{
    internal class ADT8940A1_RelativeMove
    {
        /// <summary>
        /// 相对移动（异步方法）
        /// </summary>
        /// <param name="axis">轴号 0：X 1:Y 2:Z 3:转盘</param>
        /// <param name="pulse">坐标</param>
        /// <param name="hSpeed">速度</param>
        /// <param name="upSpeed">加减速</param>
        /// <returns>动作完成/异常码</returns>
        public Task<int> RelativeMoveAsync(short axis, int pulse, int hSpeed, int upSpeed)
        {
            var boaed = SmartColor.My_ConPar.Object.CurrentMachine as SmartColor.My_ConPar.Type.BoaedCard.IO;
            return Task.Run(() =>
            {
                try
                {
                    if(axis == 0)
                    {
                        Lib_Card.Base.Card.MoveArg s_MoveArg = new Lib_Card.Base.Card.MoveArg()
                        {
                            Pulse = pulse,
                            LSpeed = 0,
                            HSpeed = hSpeed,
                            Time = upSpeed
                        };
                        if (-1 == Lib_Card.CardObject.OA1Axis.Relative_X(1, s_MoveArg, 0))
                            throw new Exception("驱动异常");
                    }
                    else if (axis == 1)
                    {
                        Lib_Card.Base.Card.MoveArg s_MoveArg = new Lib_Card.Base.Card.MoveArg()
                        {
                            Pulse = pulse,
                            LSpeed = 0,
                            HSpeed = hSpeed,
                            Time = upSpeed
                        };
                        if (-1 == Lib_Card.CardObject.OA1Axis.Relative_Y(1, s_MoveArg, 0))
                            throw new Exception("驱动异常");
                    }
                    else if (axis == 2)
                    {
                        Lib_Card.Base.Card.MoveArg s_MoveArg = new Lib_Card.Base.Card.MoveArg()
                        {
                            Pulse = pulse,
                            LSpeed = 0,
                            HSpeed = hSpeed,
                            Time = upSpeed
                        };
                        if (-1 == Lib_Card.CardObject.OA1Axis.Relative_Z( s_MoveArg, 0))
                            throw new Exception("驱动异常");
                    }

                }
                catch (Exception ex)
                {
                    if (ex.Message == "X轴正在运行")
                    {
                        return 11;
                    }
                    else if (ex.Message == "X轴伺服器报警")
                    {
                        return 18;
                    }
                    else if (ex.Message == "X轴矢能未接通")
                    {
                        return 19;
                    }
                    else if (ex.Message == "X轴正限位已通")
                    {
                        return 32;
                    }
                    else if (ex.Message == "X轴准备信号未接通")
                    {
                        return 21;
                    }
                    else if (ex.Message == "Y轴准备信号未接通")
                    {
                        return 121;
                    }
                    else if (ex.Message == "Y轴正在运行")
                    {
                        return 111;
                    }
                    else if (ex.Message == "Y轴伺服器报警")
                    {
                        return 101;
                    }
                    else if (ex.Message == "Y轴矢能未接通")
                    {
                        return 102;
                    }
                    else if (ex.Message == "Y轴正限位已通")
                    {
                        return 132;
                    }
                    else if (ex.Message == "Z轴反限位已通")
                    {
                        return 231;
                    }
                    else if (ex.Message.Contains("脉冲计算异常"))
                    {
                        return 242;
                    }
                    else if (ex.Message == "气缸上超时")
                    {
                        return 301;
                    }
                    else if (ex.Message == "气缸下信号已接通")
                    {
                        return 301;
                    }
                    else if (ex.Message == "气缸下超时")
                    {
                        return 322;
                    }
                    else if (ex.Message == "气缸上信号已接通")
                    {
                        return 321;
                    }
                    else if (ex.Message == "抓手A打开超时")
                    {
                        return 401;
                    }
                    else if (ex.Message == "抓手B打开超时")
                    {
                        return 402;
                    }
                    else if (ex.Message == "抓手A关闭超时")
                    {
                        return 411;
                    }
                    else if (ex.Message == "抓手B关闭超时")
                    {
                        return 412;
                    }
                    else if (ex.Message == "接液盘收回超时")
                    {
                        return 511;
                    }
                    else if (ex.Message == "接液盘出信号已接通")
                    {
                        return 512;
                    }
                    else if (ex.Message == "接液盘伸出超时")
                    {
                        return 501;
                    }
                    else if (ex.Message == "接液盘回信号已接通")
                    {
                        return 502;
                    }
                    else if (ex.Message == "泄压气缸上超时")
                    {
                        return 601;
                    }
                    else if (ex.Message == "泄压气缸下已通")
                    {
                        return 602;
                    }
                    else if (ex.Message == "阻挡气缸收回超时")
                    {
                        return 1112;
                    }
                    else if (ex.Message == "气缸阻挡限位已通")
                    {
                        return 1122;
                    }
                    else if (ex.Message == "气缸慢速中超时")
                    {
                        return 1123;
                    }
                    else if (ex.Message == "慢速中限位与气缸下已通")
                    {
                        return 1129;
                    }
                    else if (ex.Message == "气缸到阻挡超时")
                    {
                        return 1131;
                    }
                    else if (ex.Message == "未发现针筒")
                    {
                        return 2101;
                    }
                    else
                    {
                        //其他错误
                        return -1;
                    }
                }

                return 2;
            });
        }
    }
}
