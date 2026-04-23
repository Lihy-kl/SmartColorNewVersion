using SmartColor.My_Control;
using SmartColor.My_Cup;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Form.Homepage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_AutomaticModule
{
    /// <summary>
    /// 缸通讯统一管理器（单例）
    /// 通过Home实例递归查找所有CtCupArea，实现按杯号查找区域、通讯对象等功能。
    /// 所有通讯线程的启动、停止、滴液等操作都由本类统一调度，CtCupArea只负责UI和参数。
    /// </summary>
    public class CupCommManager
    {
        // 新增：线程安全锁
        private readonly object _commMapLock = new object();
        // 单例实现
        private static CupCommManager _instance;
        public static CupCommManager Instance => _instance ?? (_instance = new CupCommManager());
        // 在 CupCommManager 类中添加
        private readonly Dictionary<string, object> _ipThreadLockMap = new Dictionary<string, object>();
        // 当前主界面Home实例
        private Home _home;

        // 区域到通讯对象的映射（如FC_6/FC_12等，按需扩展）
        private readonly Dictionary<CtCupArea, ICylinderComm> _areaCommMap = new Dictionary<CtCupArea, ICylinderComm>();

        private CupCommManager() { }

        /// <summary>
        /// 设置主界面Home实例，必须在Home加载后调用一次
        /// </summary>
        /// <param name="home">主界面Home实例</param>
        public void SetHome(Home home)
        {
            _home = home;
            _areaCommMap.Clear();

            // 自动恢复温度记录
            foreach (var area in GetAllCupAreas())
            {
                // 确保通讯对象已创建
                this.EnsureCommThread(area,true);
                var comm = GetCommObject(area);
                if (comm == null) continue;

                int firstNo = area.StartCupNo;
                int lastNo = area.StartCupNo + area.TotalCupNum - 1;
                var table = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} >= {firstNo} AND  {CUP_DETAILS.CupNum} <= {lastNo}");
                if (table.Rows.Count == 0) continue;
              
                foreach (DataRow row in table.Rows)
                {
                    int cupNum = Convert.ToInt32(row[CUP_DETAILS.CupNum]);
                    string status = row[CUP_DETAILS.Statues]?.ToString();
                    // 这里根据你的业务判断哪些状态需要恢复温度记录
                    if (status != "待机" && status != "下线" && status != "滴液")
                    {
                        var recorder = SmartColor.My_File.CupTempRecorder.Get(cupNum);
                        recorder.SetCylinderComm(comm);
                        recorder.StartRecord(false);
                    }
                }
            }
        }

        /// <summary>
        /// 递归查找所有CtCupArea控件
        /// </summary>
        public IEnumerable<CtCupArea> GetAllCupAreas()
        {
            if (_home == null) yield break;
            foreach (var area in FindCupAreas(_home))
                yield return area;
        }

        /// <summary>
        /// 递归遍历控件树，查找所有CtCupArea
        /// </summary>
        private IEnumerable<CtCupArea> FindCupAreas(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is CtCupArea cupArea)
                    yield return cupArea;
                if (ctrl.HasChildren)
                {
                    foreach (var child in FindCupAreas(ctrl))
                        yield return child;
                }
            }
        }

        /// <summary>
        /// 按杯号查找所属CtCupArea
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <returns>所属区域控件</returns>
        public CtCupArea FindCupAreaByCupNum(int cupNum)
        {
            if (_home == null)
                return null;

            if (_home.InvokeRequired)
            {
                // 切换到UI线程执行
                return (CtCupArea)_home.Invoke(new Func<int, CtCupArea>(FindCupAreaByCupNum), cupNum);
            }

            foreach (var area in GetAllCupAreas())
            {
                if (area.ContainsCup(cupNum))
                    return area;
            }
            return null;
        }

        /// <summary>
        /// 根据杯号查找CtCup控件（异步）
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <returns>对应CtCup控件，未找到返回null</returns>
        public async Task<CtCup> FindCupByCupNumAsync(int cupNum)
        {
            if (_home == null)
                return null;

            if (_home.InvokeRequired)
            {
                // 切换到UI线程执行
                return await Task.FromResult((CtCup)_home.Invoke(new Func<int, CtCup>(FindCupByCupNumSync), cupNum));
            }
            else
            {
                return FindCupByCupNumSync(cupNum);
            }
        }

        // 新增同步方法，专用于UI线程内查找
        private CtCup FindCupByCupNumSync(int cupNum)
        {
            var area = FindCupAreaByCupNum(cupNum);
            if (area == null || area.IsDisposed)
                return null;

            const int maxRetry = 20; // 最多重试20次
            const int delayMs = 10;  // 每次重试间隔10ms

            for (int retry = 0; retry < maxRetry; retry++)
            {
                try
                {
                    var groupBoxes = area.Controls.OfType<GroupBox>().ToArray();
                    foreach (var cupArea in groupBoxes)
                    {
                        if (cupArea.IsDisposed) continue;
                        var cups = cupArea.Controls.OfType<CtCup>().ToArray();
                        foreach (var cup in cups)
                        {
                            if (cup.IsDisposed || cup.Parent == null) continue;
                            if (int.TryParse(cup.NO, out int no) && no == cupNum)
                            {
                                return cup;
                            }
                        }
                    }
                    // 没找到，稍等再试
                    System.Threading.Thread.Sleep(delayMs);
                }
                catch (ObjectDisposedException)
                {
                    // 控件被释放，重试
                    System.Threading.Thread.Sleep(delayMs);
                }
                catch (InvalidOperationException)
                {
                    // 控件集合变更，重试
                    System.Threading.Thread.Sleep(delayMs);
                }
            }
            // 超过最大重试次数仍未找到
            Logger.Error($"FindCupByCupNumSync: 超过最大重试次数，未找到杯号{cupNum}对应的CtCup控件");
            return null;
        }

        /// <summary>
        /// 确保区域通讯线程已启动（如未启动则自动创建并启动）
        /// </summary>
        /// <param name="area">区域控件</param>
        public void EnsureCommThread(CtCupArea area, bool isFristStart = false)
        {
            if(My_ConPar.Machine.MachineType == 2)
            {
                //脱机版不启动通讯线程
                return; 
            }

            lock (_commMapLock)
            {

                if(area.AreaType >=2 && area.AreaType <= 4)
                {
                    // 只有缸型区域才需要通讯线程
                }
                else
                {
                    // 非缸型区域不启动通讯线程
                    return;
                }

                string ip = area.Ip;
                if (string.IsNullOrEmpty(ip))
                    throw new Exception("区域IP不能为空");

                // 保证同一IP只允许一个线程
                lock (_ipThreadLockMap)
                {
                    if (_ipThreadLockMap.ContainsKey(ip))
                    {
                        // 已有线程在跑，直接返回
                        return;
                    }
                    // 标记该IP已启动线程
                    _ipThreadLockMap[ip] = new object();
                }

                // 如果已存在通讯对象且正在运行，不再新建
                if (_areaCommMap.TryGetValue(area, out var comm))
                {
                    if (comm.IsRunning)
                        return;
                    _areaCommMap.Remove(area);
                }


                // 启动通讯线程
                switch (area.AreaType)
                {
                    case 2:
                        {
                            var fc4 = new SmartColor.My_Cup.FC_4(
                                area.Ip, area.Port, area.StartCupNo, area.HmiType, area.AreaName);

                            fc4.OnCupDataReceived += area.OnCupDataReceived;

                            Task.Run(async () =>
                            {
                                try
                                {
                                    await fc4.StartCom(area, isFristStart);
                                }
                                finally
                                {
                                    lock (_ipThreadLockMap)
                                    {
                                        _ipThreadLockMap.Remove(ip);
                                    }
                                    lock (_commMapLock)
                                    {
                                        _areaCommMap.Remove(area);
                                    }
                                }
                            });
                            _areaCommMap[area] = fc4;
                            break;
                        }
                    case 3:
                        {
                            var fc6 = new SmartColor.My_Cup.FC_6(
                                area.Ip, area.Port, area.StartCupNo, area.HmiType, area.AreaName);

                            fc6.OnCupDataReceived += area.OnCupDataReceived;

                            Task.Run(async () =>
                            {
                                try
                                {
                                    await fc6.StartCom(area, isFristStart);
                                }
                                finally
                                {
                                    lock (_ipThreadLockMap)
                                    {
                                        _ipThreadLockMap.Remove(ip);
                                    }
                                    lock (_commMapLock)
                                    {
                                        _areaCommMap.Remove(area);
                                    }
                                }
                            });
                            _areaCommMap[area] = fc6;
                            break;
                        }
                    case 4:
                        {
                            var fc12 = new SmartColor.My_Cup.FC_12(
                                area.Ip, area.Port, area.StartCupNo, area.HmiType, area.AreaName);

                            fc12.OnCupDataReceived += area.OnCupDataReceived;

                            Task.Run(async () =>
                            {
                                try
                                {
                                    await fc12.StartCom(area, isFristStart);
                                }
                                finally
                                {
                                    lock (_ipThreadLockMap)
                                    {
                                        _ipThreadLockMap.Remove(ip);
                                    }
                                    lock (_commMapLock)
                                    {
                                        _areaCommMap.Remove(area);
                                    }
                                }
                            });
                            _areaCommMap[area] = fc12;
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 按杯号请求滴液（自动查找区域、确保通讯、发起滴液流程）
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <param name="cupChoice">杯子选择：0=两杯都选，1=主杯，2=副杯</param>
        /// <returns>滴液准备是否成功</returns>
        public async Task<DyeingResult> RequestDropLiquidAsync(int cupNum, int cupChoice = 1)
        {
            var area = FindCupAreaByCupNum(cupNum);
            if (area == null)
                throw new Exception($"未找到杯号{cupNum}对应的区域");

            this.EnsureCommThread(area);

            // 获取通讯对象
            if (_areaCommMap.TryGetValue(area, out var comm))
            {
                return await comm.RequestDropLiquidAsync(cupNum, cupChoice);
            }
            throw new Exception($"区域通讯对象未初始化或类型不匹配");
        }

        /// <summary>
        /// 按杯号请求染色
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <param name="cupChoice">杯子选择：0=两杯都选，1=主杯，2=副杯</param>
        /// <returns>滴液准备是否成功</returns>
        public async Task<DyeingResult> RequestDyeingAsync(int cupNum, int cupChoice)
        {
            var area = FindCupAreaByCupNum(cupNum);
            if (area == null)
                throw new Exception($"未找到杯号{cupNum}对应的区域");

            this.EnsureCommThread(area);

            // 获取通讯对象
            if (_areaCommMap.TryGetValue(area, out var comm))
            {
                return await comm.DyeingStartAsync(cupNum, cupChoice);
            }
            throw new Exception($"区域通讯对象未初始化或类型不匹配");
        }

        /// <summary>
        /// 按区域获取通讯对象（如FC_12/FC_6等）
        /// </summary>
        /// <param name="area">区域控件</param>
        /// <returns>通讯对象</returns>
        public ICylinderComm GetCommObject(CtCupArea area)
        {
            _areaCommMap.TryGetValue(area, out var obj);
            return obj;
        }

        /// <summary>
        /// 停止通讯线程
        /// </summary>
        public void StopCommThread(CtCupArea area)
        {
            if (_areaCommMap.TryGetValue(area, out var comm))
            {
                comm.RequestStop();

                // 其他缸型同理
                _areaCommMap.Remove(area);
            }
        }

        /// <summary>
        /// 判断某区域是否需要启动通讯
        /// </summary>
        /// <param name="area">区域</param>
        /// <returns>true:需要启动；false:不需要</returns>
        public bool ShouldStartComm(CtCupArea area)
        {
            // 查询该区域所有杯子的状态
            int firstNo = area.StartCupNo;
            int lastNo = area.StartCupNo + area.TotalCupNum - 1;
            var dtTable = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.Statues} <> '下线' AND  {CUP_DETAILS.CupNum} >= {firstNo} AND {CUP_DETAILS.CupNum} <= {lastNo}");
            if (dtTable.Rows.Count == 0)
                return false;
            // 只要有不是“下线”的杯子，就需要通讯
          
            foreach (DataRow row in dtTable.Rows)
            {
                string status = row[CUP_DETAILS.Statues]?.ToString();
                if (!string.Equals(status, "下线", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取当前杯支持的最大总浴量
        /// </summary>
        /// <param name="cupNo">杯号</param>
        /// <returns></returns>
        public double GetCupMaxWeight(int cupNo)
        {
            var area = FindCupAreaByCupNum(cupNo);
            if (area == null)
                return 0;
            if (area.AreaType == 2)
            {
                //4杯大翻转缸
                return My_ConPar.Other.HandleMaxWeight_Big;

            }
            else if (area.AreaType == 8)
            {
                return My_ConPar.Other.DripMaxWeight;
            }
            else
            {
                return My_ConPar.Other.HandleMaxWeight;
            }
        }


    }
}