using SmartColor.My_ConPar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_SemiAutoModule
{
    /// <summary>
    /// 半自动助手工厂类，根据机台类型自动选择对应的半自动实现
    /// </summary>
    public static  class SemiAutoHelperFactory
    {
        /// <summary>
        /// 是否在待机位
        /// </summary>
        public static bool IsStandby = false;

        /// <summary>
        /// 当前半自动助手实例（单例缓存）
        /// </summary>
        private static  ISemiAutoHelper _instance;

        /// <summary>
        /// 获取当前机型对应的半自动助手实例
        /// </summary>
        public static  ISemiAutoHelper Current
        {
            get
            {
                if (_instance == null)
                {
                    switch (Machine.MachineType)
                    {
                        case 0: // PLC控制系统
                            _instance = new PLCSemiAutoHelperAdapter();
                            break;
                        case 1: // ADT8940A1控制系统
                           // _instance = new ADT8940A1SemiAutoHelperAdapter();
                            break;
                        default:
                            throw new System.NotSupportedException("不支持的机台类型");
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 重置当前实例（切换机型时需调用）
        /// </summary>
        public static  void Reset() => _instance = null;
    }
}