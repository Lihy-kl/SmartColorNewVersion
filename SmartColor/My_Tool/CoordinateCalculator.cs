using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SmartColor.My_ConPar;

namespace SmartColor.My_Tool
{
    /// <summary>
    /// 区域坐标查找类（带缓存优化版，分区缓存）
    /// 自动判断编号属于哪个区域，并返回对应的坐标（支持杯子、杯盖、母液瓶、备布区、出布区、夹具、天平等）
    /// 启动或布局切换时批量构建缓存，查找时O(1)极快，且不会串区。
    /// </summary>
    public static class AreaCoordinateFinder
    {
        // 杯区编号到坐标缓存（只缓存配液杯/滴液区/翻转缸/转子缸等）
        private static readonly Dictionary<int, (int x, int y)> _cupCoordinateCache = new Dictionary<int, (int x, int y)>();
        // 杯盖编号到坐标缓存
        private static readonly Dictionary<int, (int x, int y)> _cupLidCoordinateCache = new Dictionary<int, (int x, int y)>();
        // 备布区编号到坐标缓存
        private static readonly Dictionary<int, (int x, int y)> _prepareClothCoordinateCache = new Dictionary<int, (int x, int y)>();
        // 出布区编号到坐标缓存
        private static readonly Dictionary<int, (int x, int y)> _outClothCoordinateCache = new Dictionary<int, (int x, int y)>();
        // 母液瓶编号到坐标缓存
        private static readonly Dictionary<int, (int x, int y)> _bottleCoordinateCache = new Dictionary<int, (int x, int y)>();
        // 干布夹具坐标
        private static (int x, int y)? _dryClampCoordinate;
        // 湿布夹具坐标
        private static (int x, int y)? _wetClampCoordinate;
        // 天平坐标
        private static (int x, int y)? _balanceCoordinate;
        // 洗针区坐标
        private static (int x, int y)? _washCoordinate;

        /// <summary>
        /// 构建所有区域的坐标缓存（建议在启动或布局切换后调用一次）
        /// </summary>
        public static void BuildAllCoordinateCache(Type layoutType)
        {
            _cupCoordinateCache.Clear();
            _cupLidCoordinateCache.Clear();
            _prepareClothCoordinateCache.Clear();
            _outClothCoordinateCache.Clear();
            _bottleCoordinateCache.Clear();
            _dryClampCoordinate = null;
            _wetClampCoordinate = null;
            _balanceCoordinate = null;
            _washCoordinate = null;

            var areaProps = layoutType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var prop in areaProps)
            {
                var areaObj = prop.GetValue(null) as SmartColor.My_ConPar.Area.Base;
                if (areaObj == null) continue;

                switch (areaObj.AreaType)
                {
                    // 杯区（翻转缸、转子缸、滴液区等）
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        CacheCupArea(areaObj);
                        CacheCupLidArea(areaObj);
                        break;
                    // 母液瓶区
                    case 9:
                        CacheBottleArea(areaObj);
                        break;
                    // 干布夹具
                    case 10:
                        _dryClampCoordinate = (GetInt(areaObj, "X"), GetInt(areaObj, "Y"));
                        break;
                    // 湿布夹具
                    case 11:
                        _wetClampCoordinate = (GetInt(areaObj, "X"), GetInt(areaObj, "Y"));
                        break;
                    // 天平
                    case 1:
                        _balanceCoordinate = (GetInt(areaObj, "BalanceCX"), GetInt(areaObj, "BalanceCY"));
                        break;
                    // 洗针区
                    case 13:
                        _washCoordinate = (GetInt(areaObj, "X"), GetInt(areaObj, "Y"));
                        break;
                    // 备布区
                    case 14:
                        CachePrepareClothArea(areaObj);
                        break;
                    // 出布区
                    case 15:
                        CacheOutClothArea(areaObj);
                        break;
                        // 其他区域可按需扩展
                }
            }
        }

        /// <summary>
        /// 缓存杯区（翻转缸/转子缸/滴液区等）所有编号的坐标
        /// </summary>
        private static void CacheCupArea(object areaObj)
        {
            int start = GetInt(areaObj, "StartPosition");
            int row = GetInt(areaObj, "Row");
            int col = GetInt(areaObj, "Column");
            int vertical = GetInt(areaObj, "Vertical");
            if (row <= 0 || col <= 0) return;
            int total = row * col;

            // 判断是否为滴液区
            bool isDrop = areaObj is SmartColor.My_ConPar.Area.Drop.Drop;

            for (int i = 0; i < total; i++)
            {
                int num = start + i;
                int r, c;
                if (vertical == 0)
                {
                    r = i / col;
                    c = i % col;
                }
                else
                {
                    r = i % row;
                    c = i / row;
                }

                int x = GetInt(areaObj, $"CupCX_{i+1}");
                int y = GetInt(areaObj, $"CupCY_{i+1}");

                // 只有滴液区才允许用首杯+间隔推算
                if ((x == 0 && y == 0) && isDrop)
                {
                    x = GetInt(areaObj, "CupCX_1") + c * GetInt(areaObj, "CupIX");
                    y = GetInt(areaObj, "CupCY_1") + r * GetInt(areaObj, "CupIY");
                }

                // 非滴液区且没有独立属性，直接跳过，避免错误
                if (!isDrop && (x == 0 && y == 0))
                    continue;

                _cupCoordinateCache[num] = (x, y);
            }
        }

        /// <summary>
        /// 缓存杯盖区所有编号的坐标
        /// </summary>
        private static void CacheCupLidArea(object areaObj)
        {
            int start = GetInt(areaObj, "StartPosition");
            int row = GetInt(areaObj, "Row");
            int col = GetInt(areaObj, "Column");
            if (row <= 0 || col <= 0) return;
            int total = row * col;
            for (int i = 0; i < total; i++)
            {
                int num = start + i;
                int x = GetInt(areaObj, $"LidCX_{i+1}");
                int y = GetInt(areaObj, $"LidCY_{i+1}");
                if (x != 0 || y != 0)
                    _cupLidCoordinateCache[num] = (x, y);
            }
        }

        /// <summary>
        /// 缓存备布区所有编号的坐标
        /// </summary>
        private static void CachePrepareClothArea(object areaObj)
        {
            int start = GetInt(areaObj, "StartPosition");
            int row = GetInt(areaObj, "Row");
            int col = GetInt(areaObj, "Column");
            int vertical = GetInt(areaObj, "Vertical");
            int x1 = GetInt(areaObj, "PrepareClothX_1");
            int y1 = GetInt(areaObj, "PrepareClothY_1");
            int ix = GetInt(areaObj, "PrepareClothIX");
            int iy = GetInt(areaObj, "PrepareClothIY");
            if (row <= 0 || col <= 0) return;
            int total = row * col;
            for (int i = 0; i < total; i++)
            {
                int num = start + i;
                int r, c;
                if (vertical == 0)
                {
                    r = i % row;
                    c = i / row;
                  
                }
                else
                {
                    r = i / col;
                    c = i % col;
                }
                int x = x1 + c * ix;
                int y = y1 + r * iy;
                _prepareClothCoordinateCache[num] = (x, y);
            }
        }

        /// <summary>
        /// 缓存出布区所有编号的坐标
        /// </summary>
        private static void CacheOutClothArea(object areaObj)
        {
            int start = GetInt(areaObj, "StartPosition");
            int row = GetInt(areaObj, "Row");
            int col = GetInt(areaObj, "Column");
            int vertical = GetInt(areaObj, "Vertical");
            int x1 = GetInt(areaObj, "OutClothX_1");
            int y1 = GetInt(areaObj, "OutClothY_1");
            int ix = GetInt(areaObj, "OutClothIX");
            int iy = GetInt(areaObj, "OutClothIY");
            if (row <= 0 || col <= 0) return;
            int total = row * col;
            for (int i = 0; i < total; i++)
            {
                int num = start + i;
                int r, c;
                if (vertical == 0)
                {
                    r = i % row;
                    c = i / row;
                  
                }
                else
                {
                    r = i / col;
                    c = i % col;

                }
                int x = x1 + c * ix;
                int y = y1 + r * iy;
                _outClothCoordinateCache[num] = (x, y);
            }
        }

        /// <summary>
        /// 缓存母液瓶区所有编号的坐标
        /// </summary>
        private static void CacheBottleArea(object areaObj)
        {
            int bottleNum = GetInt(areaObj, "BottleNum");
            int bottleColumn = GetInt(areaObj, "BottleColumn");
            int bottleCX_1 = GetInt(areaObj, "BottleCX_1");
            int bottleCY_1 = GetInt(areaObj, "BottleCY_1");
            int bottleIX = GetInt(areaObj, "BottleIX");
            int bottleIY = GetInt(areaObj, "BottleIY");
            int originPosition = My_ConPar.Hardware.OriginPosition;
            for (int i = 1; i <= bottleNum; i++)
            {
                int x, y;
                if (bottleNum % bottleColumn != 0)
                {
                    int normalBottle = bottleNum - 14;
                    if (i <= normalBottle)
                    {
                        x = originPosition == 0 ?
                            bottleCX_1 + ((i - 1) % bottleColumn) * bottleIX :
                            bottleCX_1 - ((i - 1) % bottleColumn) * bottleIX;
                        y = bottleCY_1 + ((i - 1) / bottleColumn) * bottleIY;
                    }
                    else if (i > normalBottle && i <= normalBottle + 7)
                    {
                        int adjustedNum = i + 3;
                        x = originPosition == 0 ?
                            bottleCX_1 + ((adjustedNum - 1) % bottleColumn) * bottleIX :
                            bottleCX_1 - ((adjustedNum - 1) % bottleColumn) * bottleIX;
                        y = bottleCY_1 + ((adjustedNum - 1) / bottleColumn) * bottleIY;
                    }
                    else
                    {
                        int adjustedNum = i + 6;
                        x = originPosition == 0 ?
                            bottleCX_1 + ((adjustedNum - 1) % bottleColumn) * bottleIX :
                            bottleCX_1 - ((adjustedNum - 1) % bottleColumn) * bottleIX;
                        y = bottleCY_1 + ((adjustedNum - 1) / bottleColumn) * bottleIY;
                    }
                }
                else
                {
                    x = originPosition == 0 ?
                        bottleCX_1 + ((i - 1) % bottleColumn) * bottleIX :
                        bottleCX_1 - ((i - 1) % bottleColumn) * bottleIX;
                    y = bottleCY_1 + ((i - 1) / bottleColumn) * bottleIY;
                }
                _bottleCoordinateCache[i] = (x, y);
            }
        }

        /// <summary>
        /// 反射获取int类型属性值
        /// </summary>
        private static int GetInt(object obj, string propName)
        {
            var prop = obj.GetType().GetProperty(propName);
            if (prop != null && prop.PropertyType == typeof(int))
                return (int)prop.GetValue(obj);
            return 0;
        }

        // ------------------ 查询接口（异步） ------------------

        /// <summary>
        /// 异步查找杯心坐标（O(1)缓存查找）
        /// </summary>
        public static Task<(bool found, int x, int y)> TryGetCupCoordinateAsync(int num)
        {
            if (_cupCoordinateCache.TryGetValue(num, out var coord))
                return Task.FromResult((true, coord.x, coord.y));
            return Task.FromResult((false, 0, 0));
        }

        /// <summary>
        /// 异步查找杯盖坐标（O(1)缓存查找）
        /// </summary>
        public static Task<(bool found, int x, int y)> TryGetCupLidCoordinateAsync(int num)
        {
            if (_cupLidCoordinateCache.TryGetValue(num, out var coord))
                return Task.FromResult((true, coord.x, coord.y));
            return Task.FromResult((false, 0, 0));
        }

        /// <summary>
        /// 异步查找杯子或杯盖的坐标（自动判断所属区域，O(1)缓存查找）
        /// </summary>
        public static Task<(bool found, int x, int y)> TryGetCupOrLidCoordinateAsync(int num, bool isLid)
        {
            return isLid ? TryGetCupLidCoordinateAsync(num) : TryGetCupCoordinateAsync(num);
        }

        /// <summary>
        /// 异步查找母液瓶区坐标（O(1)缓存查找）
        /// </summary>
        public static Task<(bool found, int x, int y)> TryGetBottleCoordinateAsync(int num)
        {
            if (_bottleCoordinateCache.TryGetValue(num, out var coord))
                return Task.FromResult((true, coord.x, coord.y));
            return Task.FromResult((false, 0, 0));
        }

        /// <summary>
        /// 异步查找备布区坐标（O(1)缓存查找）
        /// </summary>
        public static Task<(bool found, int x, int y)> TryGetPrepareClothCoordinateAsync(int num)
        {
            if (_prepareClothCoordinateCache.TryGetValue(num, out var coord))
                return Task.FromResult((true, coord.x, coord.y));
            return Task.FromResult((false, 0, 0));
        }

        /// <summary>
        /// 异步查找出布区坐标（O(1)缓存查找）
        /// </summary>
        public static Task<(bool found, int x, int y)> TryGetOutClothCoordinateAsync(int num)
        {
            if (_outClothCoordinateCache.TryGetValue(num, out var coord))
                return Task.FromResult((true, coord.x, coord.y));
            return Task.FromResult((false, 0, 0));
        }

        /// <summary>
        /// 异步查找干布夹具坐标
        /// </summary>
        public static Task<(bool found, int x, int y)> TryGetDryClampCoordinateAsync()
        {
            if (_dryClampCoordinate.HasValue)
                return Task.FromResult((true, _dryClampCoordinate.Value.x, _dryClampCoordinate.Value.y));
            return Task.FromResult((false, 0, 0));
        }

        /// <summary>
        /// 异步查找湿布夹具坐标
        /// </summary>
        public static Task<(bool found, int x, int y)> TryGetWetClampCoordinateAsync()
        {
            if (_wetClampCoordinate.HasValue)
                return Task.FromResult((true, _wetClampCoordinate.Value.x, _wetClampCoordinate.Value.y));
            return Task.FromResult((false, 0, 0));
        }

        /// <summary>
        /// 异步查找天平坐标
        /// </summary>
        public static Task<(bool found, int x, int y)> TryGetBalanceCoordinateAsync()
        {
            if (_balanceCoordinate.HasValue)
                return Task.FromResult((true, _balanceCoordinate.Value.x, _balanceCoordinate.Value.y));
            // 兼容老逻辑
            int x = My_ConPar.Object.CurrentBalance?.BalanceCX ?? 0;
            int y = My_ConPar.Object.CurrentBalance?.BalanceCY ?? 0;
            return Task.FromResult((x != 0 || y != 0) ? (true, x, y) : (false, 0, 0));
        }

        /// <summary>
        /// 异步查找洗针区坐标
        /// </summary>
        public static Task<(bool found, int x, int y)> TryGetWashCoordinateAsync()
        {
            if (_washCoordinate.HasValue)
                return Task.FromResult((true, _washCoordinate.Value.x, _washCoordinate.Value.y));
            return Task.FromResult((false, 0, 0));
        }

        /// <summary>
        /// 异步查找泄压坐标（基于杯号，需反射+偏移）
        /// </summary>
        public static Task<(bool found, int x, int y)> TryGetDecompressionCoordinateAsync(int num)
        {
            var layoutType = My_ConPar.Object.CurrentLayout as Type;
            if (layoutType == null) return Task.FromResult((false, 0, 0));
            var areaProps = layoutType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var prop in areaProps)
            {
                var areaObj = prop.GetValue(null) as SmartColor.My_ConPar.Area.Base;
                if (areaObj == null) continue;
                if (IsCupArea(areaObj))
                {
                    int start = GetInt(areaObj, "StartPosition");
                    int row = GetInt(areaObj, "Row");
                    int col = GetInt(areaObj, "Column");
                    if (row > 0 && col > 0)
                    {
                        int end = start + row * col - 1;
                        if (num >= start && num <= end)
                        {
                            int x = GetInt(areaObj, $"CupCX_{num}") + GetInt(areaObj, "DecompressionOffsetX");
                            int y = GetInt(areaObj, $"CupCY_{num}") + GetInt(areaObj, "DecompressionOffsetY");
                            return Task.FromResult((true, x, y));
                        }
                    }
                }
            }
            return Task.FromResult((false, 0, 0));
        }

        /// <summary>
        /// 异步查找待机坐标
        /// </summary>
        public static Task<(bool found, int x, int y)> TryGetStandbyCoordinateAsync()
        {
            int x = My_ConPar.Other.StandbyX;
            int y = My_ConPar.Other.StandbyY;
            return Task.FromResult((true, x, y));
        }

        // ------------------ 区域类型判断 ------------------

        /// <summary>
        /// 杯子相关区域类型判断
        /// </summary>
        private static bool IsCupArea(object areaObj)
        {
            return areaObj is SmartColor.My_ConPar.Area.Drop.Drop
                || areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_4
                || areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_6
                || areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_12
                || areaObj is SmartColor.My_ConPar.Area.FlipCylinder.FC_16
                || areaObj is SmartColor.My_ConPar.Area.RotorCylinder.RC_4
                || areaObj is SmartColor.My_ConPar.Area.RotorCylinder.RC_10;
        }

        public static void UpdateCupCoordinateCache(int cupNo, int x, int y)
        {
            lock (_cupCoordinateCache)
            {
                _cupCoordinateCache[cupNo] = (x, y);
            }
        }
    }
}