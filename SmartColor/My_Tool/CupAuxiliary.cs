using SmartColor.My_AutomaticModule;
using SmartColor.My_ConPar;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Form.Homepage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SmartColor.My_Tool.CupAuxiliary;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SmartColor.My_Tool
{
    /// <summary>
    /// 杯辅助类
    /// </summary>
    public class CupAuxiliary
    {
        // 在 CupAuxiliary 类内添加事件定义
        public static event Action CupFinished; // 参数为 cupNum


        // 静态构造函数，类加载时自动执行
        static CupAuxiliary()
        {
            WashCup.ParameterChanged += OnWashCupParameterChanged;
        }


        private static void OnWashCupParameterChanged(string paramName, double value)
        {
            // 更新StopWashCup、FailWashCup、HighTempWashCup中的相关参数
            switch (paramName)
            {
                case nameof(WashCup.Wash_Temp):
                    // StopWashCup第4步温度
                    UpdateWashCupListTemp(StopWashCup, 4, value);
                    break;
                case nameof(WashCup.Wash_TempSpeed):
                    // StopWashCup第4步升温速率
                    UpdateWashCupListTempSpeed(StopWashCup, 4, value);
                    break;
                case nameof(WashCup.HighTempWash_Temp):
                    // HighTempWashCup第4步温度
                    UpdateWashCupListTemp(HighTempWashCup, 4, value);
                    break;
                case nameof(WashCup.HighTempWash_TempSpeed):
                    // HighTempWashCup第4步升温速率
                    UpdateWashCupListTempSpeed(HighTempWashCup, 4, value);
                    break;
                    // 如有其它参数需同步，继续补充
            }
        }

        private static void UpdateWashCupListTemp(List<WashCupPar> list, int stepIndex, double temp)
        {
            // stepIndex为1-based，List为0-based
            if (list != null && list.Count >= stepIndex)
            {
                var item = list[stepIndex - 1];
                item.Temp = temp;
                list[stepIndex - 1] = item;
            }
        }

        private static void UpdateWashCupListTempSpeed(List<WashCupPar> list, int stepIndex, double tempSpeed)
        {
            if (list != null && list.Count >= stepIndex)
            {
                var item = list[stepIndex - 1];
                item.TempSpeed = tempSpeed;
                list[stepIndex - 1] = item;
            }
        }
        #region 洗杯辅助

        #region 洗杯类型常量
        /// <summary>停止洗杯</summary>
        public const string StopWashCupType = "停止洗杯";
        /// <summary>失败洗杯</summary>
        public const string FailWashCupType = "失败洗杯";
        /// <summary>高温洗杯</summary>
        public const string HighTempWashCupType = "高温洗杯";
        /// <summary>前洗杯</summary>
        public const string PreWashCupType = "前洗杯";
        #endregion

        /// <summary>
        /// 洗杯工艺步骤结构体
        /// </summary>
        public struct WashCupPar
        {
            public int StepNo { get; set; }
            public String TechnologyName { get; set; }
            public double Temp { get; set; }
            public double TempSpeed { get; set; }
            public double SetTime { get; set; }
        }

        /// <summary>停止洗杯流程</summary>
        public static readonly List<WashCupPar> StopWashCup = new List<WashCupPar>()
        {
            new WashCupPar{StepNo = 1,TechnologyName = "排液",Temp = 0,TempSpeed = 0,SetTime = 0 },
            new WashCupPar{StepNo = 2,TechnologyName = "出布",Temp = 0,TempSpeed = 0,SetTime = 0},
            new WashCupPar{StepNo = 3,TechnologyName = "加水",Temp = 0,TempSpeed = 0,SetTime = 0},
            new WashCupPar{StepNo = 4,TechnologyName = "温控",Temp = My_ConPar.WashCup.Wash_Temp,TempSpeed = My_ConPar.WashCup.Wash_TempSpeed,SetTime = 1},
            new WashCupPar{StepNo = 5,TechnologyName = "排液",Temp = 0,TempSpeed = 0,SetTime = 0}
        };

        /// <summary>失败洗杯/前洗杯流程</summary>
        public static readonly List<WashCupPar> FailWashCup = new List<WashCupPar>()
        {
            new WashCupPar{StepNo = 1,TechnologyName = "排液",Temp = 0,TempSpeed = 0 ,SetTime = 0},
            new WashCupPar{StepNo = 2,TechnologyName = "出布",Temp = 0,TempSpeed = 0,SetTime = 0},
            new WashCupPar{StepNo = 3,TechnologyName = "加水",Temp = 0,TempSpeed = 0,SetTime = 0},
            new WashCupPar{StepNo = 4,TechnologyName = "搅拌",Temp = 0,TempSpeed = 0,SetTime = 1},
            new WashCupPar{StepNo = 5,TechnologyName = "排液",Temp = 0,TempSpeed = 0,SetTime = 0}
        };

        /// <summary>高温流程</summary>
        public static readonly List<WashCupPar> HighTempWashCup = new List<WashCupPar>()
        {
            new WashCupPar{StepNo = 1,TechnologyName = "排液",Temp = 0,TempSpeed = 0,SetTime = 0},
            new WashCupPar{StepNo = 2,TechnologyName = "出布",Temp = 0,TempSpeed = 0,SetTime = 0},
            new WashCupPar{StepNo = 3,TechnologyName = "加水",Temp = 0,TempSpeed = 0,SetTime = 0},
            new WashCupPar{StepNo = 4,TechnologyName = "温控",Temp = My_ConPar.WashCup.HighTempWash_Temp,TempSpeed = My_ConPar.WashCup.HighTempWash_TempSpeed,SetTime = 1},
            new WashCupPar{StepNo = 5,TechnologyName = "排液",Temp = 0,TempSpeed = 0,SetTime = 0},
            new WashCupPar{StepNo = 6,TechnologyName = "加水",Temp = 0,TempSpeed = 0,SetTime = 0},
            new WashCupPar{StepNo = 7,TechnologyName = "搅拌",Temp = 0,TempSpeed = 0,SetTime = 1},
            new WashCupPar{StepNo = 8,TechnologyName = "排液",Temp = 0,TempSpeed = 0,SetTime = 0}
        };

        /// <summary>洗杯字典</summary>
        public static Dictionary<string, List<WashCupPar>> WashCupDic = new Dictionary<string, List<WashCupPar>>()
        {
            { StopWashCupType,StopWashCup},
            { FailWashCupType,FailWashCup },
            { PreWashCupType,FailWashCup },
            { HighTempWashCupType,HighTempWashCup}
        };

        #endregion

        #region 辅助方法

        /// <summary>
        /// 查找主副杯对应关系
        /// 优化：直接用GetMSCupInfo获取主副杯号，减少重复代码
        /// </summary>
        /// <param name="cupNum">当前杯号</param>
        /// <returns>主杯号、副杯号</returns>
        public static (int mainCup, int subCup) GetCupPair(int cupNum)
        {
            // 优先从cup_details表获取主副杯号
            var dt = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {cupNum}");
            if (dt == null || dt.Rows.Count == 0)
                return (cupNum, cupNum); // 异常情况只返回自己

            var row = dt.Rows[0];
            int otherCupNum = cupNum;
            if (row.Table.Columns.Contains(CUP_DETAILS.MainCupNum) && row[CUP_DETAILS.MainCupNum] != DBNull.Value)
            {
                int.TryParse(row[CUP_DETAILS.MainCupNum]?.ToString(), out otherCupNum);
            }
            int mainCup = Math.Min(cupNum, otherCupNum);
            int subCup = Math.Max(cupNum, otherCupNum);
            return (mainCup, subCup);
        }

        /// <summary>
        /// 当前杯转换杯选择
        /// 优化：简化逻辑，直接比较主副杯号
        /// </summary>
        /// <param name="cupNo">当前杯号</param>
        /// <returns>0=双杯；1=主杯；2=副杯</returns>
        public static int ReturnCyrrentCupChioce(int cupNo)
        {
            var (mainCup, subCup) = GetCupPair(cupNo);
            if (mainCup == subCup) return 0;
            if (mainCup == cupNo) return 1;
            if (subCup == cupNo) return 2;
            return 0;
        }

        /// <summary>
        /// 返回转换杯选择
        /// 优化：简化逻辑，直接比较主副杯号
        /// </summary>
        /// <param name="cupNo">当前杯号</param>
        /// <returns>0=双杯；1=主杯；2=副杯</returns>
        public static ushort GetCupChioce(int cupNo)
        {
            var (mainInfo, subInfo) = GetMSCupInfo(cupNo);
            if (mainInfo.IsUsing == 1)
            {
                if (subInfo.IsUsing == 0)
                {
                    return 1;
                }
            }
            else
            {
                if (subInfo.IsUsing == 1)
                {
                    return 2;
                }
            }
            return 0;
        }

        /// <summary>
        /// 主副杯中将正在使用的杯号组成字符串
        /// </summary>
        /// <param name="cupNo">杯号</param>
        /// <returns></returns>
        public static string GetIsUseing(int cupNo)
        {
            var (mainCup, subCup) = GetCupPair(cupNo);
            var select = GetCupChioce(cupNo);
            if (mainCup != subCup)
            {
                switch (select)
                {
                    case 0:
                        return $"{mainCup}和{subCup}";
                    case 1:
                        return $"{mainCup}";
                    case 2:
                        return $"{subCup}";
                    default:
                        return string.Empty;
                }
            }
            else
            {
                return mainCup.ToString();
            }

        }

        /// <summary>
        /// 查找主副杯中最大的液量
        /// 优化：直接用GetMSCupInfo获取主副杯详情
        /// </summary>
        /// <param name="cupNo">杯号</param>
        /// <returns>最大液量</returns>
        public static double ReturnMaxWeight(int cupNo)
        {
            var (mainCup, subCup) = GetMSCupInfo(cupNo);
            double mainWeight = Convert.ToDouble(mainCup.CurrentWeight ?? 0);
            double subWeight = Convert.ToDouble(subCup.CurrentWeight ?? 0);
            return Math.Max(mainWeight, subWeight);
        }

        /// <summary>
        /// 步骤信息结构体
        /// </summary>
        public struct StepInfo
        {

            public int StepNum;
            public string TechnologyName;
            public double Temp;
            public double TempSpeed;
            public double SetTime;
            public int RotorSpeed;
            public int DyeType;
        }

        /// <summary>
        /// 洗杯流程获取下一步工艺
        /// 优化：增加参数校验和异常信息
        /// </summary>
        /// <param name="washType">洗杯类型</param>
        /// <param name="currentStepNo">当前步号</param>
        /// <returns>步骤信息结构体</returns>
        public static StepInfo GetNextWashCupStepInfo(string washType, int currentStepNo)
        {
            if (!WashCupDic.TryGetValue(washType, out var process) || process == null)
                throw new Exception($"未知的洗杯工艺类型: {washType}");
            if (currentStepNo < 0 || currentStepNo >= process.Count)
                throw new Exception("超出最大步号");
            WashCupPar par = process[currentStepNo];

            var info = new StepInfo
            {
                StepNum = par.StepNo,
                TechnologyName = par.TechnologyName,
                Temp = par.Temp,
                TempSpeed = par.TempSpeed,
                SetTime = par.SetTime,
                RotorSpeed = 0,
                DyeType = 0
            };

            // 温控工艺特殊处理
            if (info.TechnologyName == "温控" && info.Temp < 40)
            {
                info.TechnologyName = "搅拌";
                info.Temp = 0;
                info.TempSpeed = 0;
            }
            return info;
        }

        /// <summary>
        /// 根据杯号获取下一步染固色工艺流程
        /// 优化：异常信息更详细
        /// </summary>
        /// <param name="headID">配方表头ID号</param>
        /// <param name="currentStepNo">当前步</param>
        /// <returns>步骤信息结构体</returns>
        public static StepInfo GetNextStepInfo(int headID, int currentStepNo)
        {
            var dyeRows = SqlServer.Select(DYE_DETAILS.TableName,
                $"{DYE_DETAILS.HeadID} = {headID} AND {DYE_DETAILS.StepNum} = {currentStepNo + 1} ");

            if (dyeRows == null || dyeRows.Rows.Count == 0)
            {
                return new StepInfo
                {
                    StepNum = 0,
                    TechnologyName = "未知工艺",
                    Temp = 0,
                    TempSpeed = 0,
                    SetTime = 0,
                    RotorSpeed = 0,
                    DyeType = 0
                };
            }

            var row = dyeRows.Rows[0];
            return new StepInfo
            {

                StepNum = row[DYE_DETAILS.StepNum] != DBNull.Value ? Convert.ToInt32(row[DYE_DETAILS.StepNum]) : 0,
                TechnologyName = row[DYE_DETAILS.TechnologyName]?.ToString(),
                Temp = row[DYE_DETAILS.Temp] != DBNull.Value ? Convert.ToDouble(row[DYE_DETAILS.Temp]) : 0,
                TempSpeed = row[DYE_DETAILS.TempSpeed] != DBNull.Value ? Convert.ToDouble(row[DYE_DETAILS.TempSpeed]) : 0,
                SetTime = row[DYE_DETAILS.Time] != DBNull.Value ? Convert.ToDouble(row[DYE_DETAILS.Time]) : 0,
                RotorSpeed = row[DYE_DETAILS.RotorSpeed] != DBNull.Value ? Convert.ToInt32(row[DYE_DETAILS.RotorSpeed]) : 0,
                DyeType = row[DYE_DETAILS.DyeType] != DBNull.Value ? Convert.ToInt32(row[DYE_DETAILS.DyeType]) : 0
            };
        }

        /// <summary>
        /// 根据杯号查找当前步类型（DyeType），优先查主杯，主杯无HeadID时查副杯
        /// 优化：用GetMSCupInfo简化主副杯查找
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <returns>DyeType（int），未找到返回-1</returns>
        public static (int DyeType, string TechnologyName) GetCurrentStepDyeType(int cupNum)
        {
            var (mainCup, subCup) = GetMSCupInfo(cupNum);
            int headID = 0;
            int stepNum = 0;
            // 优先查主杯
            if (mainCup.IsUsing == 1)
            {
                headID = mainCup.HeadID ?? 0;
                stepNum = mainCup.StepNum != null ? int.TryParse(mainCup.StepNum, out stepNum) ? stepNum : 0 : 0;
            }
            else if (subCup.IsUsing == 1)
            {
                headID = subCup.HeadID ?? 0;
                stepNum = subCup.StepNum != null ? int.TryParse(subCup.StepNum, out stepNum) ? stepNum : 0 : 0;
            }

            // 查找DyeType
            if (headID != 0)
            {
                var dyeRows = SqlServer.Select(DYE_DETAILS.TableName,
                    $"{DYE_DETAILS.HeadID} = {headID} AND {DYE_DETAILS.StepNum} = {stepNum}");
                if (dyeRows != null && dyeRows.Rows.Count > 0)
                {
                    var dyeRow = dyeRows.Rows[0];
                    int dyeType = 0;
                    if (dyeRow.Table.Columns.Contains(DYE_DETAILS.DyeType) && dyeRow[DYE_DETAILS.DyeType] != DBNull.Value)
                        dyeType = Convert.ToInt32(dyeRow[DYE_DETAILS.DyeType]);
                    string technologyName = dyeRow.Table.Columns.Contains(DYE_DETAILS.TechnologyName) ? dyeRow[DYE_DETAILS.TechnologyName]?.ToString() : "未知工艺";
                    return (dyeType, technologyName);
                }
            }
            return (-1, "未知工艺");
        }

        /// <summary>
        /// 杯归档：将该杯相关数据从DROP_HEAD/DETAILS/DYE_DETAILS迁移到历史表
        /// 优化：无明显冗余，已是异步和分主副杯处理
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <param name="cupChoice">杯选择</param>
        /// <returns></returns>
        public static void CupFinish(int cupNum, int cupChoice)
        {
            var (mainCup, subCup) = GetCupPair(cupNum);

            // 避免重复归档
            if (mainCup == subCup)
            {
                CupFinishSingle(mainCup);
            }
            else
            {
                if (cupChoice == 0 || cupChoice == 1)
                    CupFinishSingle(mainCup);
                if (cupChoice == 0 || cupChoice == 2)
                    CupFinishSingle(subCup);
            }

            UpdateWaitList(mainCup);
            if (mainCup != subCup)
                UpdateWaitList(subCup);
        }

        /// <summary>
        /// 杯归档：将该杯相关数据从DROP_HEAD/DETAILS/DYE_DETAILS迁移到历史表
        /// 优化：内部逻辑已较为紧凑，增加注释
        /// </summary>
        private static void CupFinishSingle(int cupNum)
        {
            try
            {

                // 1. 查找HeadID
                var dt = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {cupNum}");
                if (dt == null || dt.Rows.Count == 0)
                {
                    Logger.Error($"未找到{cupNum}杯的cup_details数据，无法完成归档。");
                    return;
                }
                var cupRow = dt.Rows[0];
                if (!cupRow.Table.Columns.Contains(CUP_DETAILS.HeadID) || cupRow[CUP_DETAILS.HeadID] == DBNull.Value)
                {
                    Logger.Error($"未找到{cupNum}杯的HeadID，无法完成归档。");
                    return;
                }
                int headId = Convert.ToInt32(cupRow[CUP_DETAILS.HeadID]);


                // 2. 获取相关数据
                var headRows = SqlServer.Select(DROP_HEAD.TableName, $"{DROP_HEAD.MyID} = {headId}");
                if (headRows == null || headRows.Rows.Count == 0)
                {
                    Logger.Error($"未找到HeadID={headId}的DROP_HEAD数据，无法完成归档。");
                    return;
                }
                var headRow = headRows.Rows[0];
                var detailsRows = SqlServer.Select(DROP_DETAILS.TableName, $"{DROP_DETAILS.HeadID} = {headId}");
                var dyeRows = SqlServer.Select(DYE_DETAILS.TableName, $"{DYE_DETAILS.HeadID} = {headId}");
                string batchName = headRow.Table.Columns.Contains(DROP_HEAD.BatchName) ? headRow[DROP_HEAD.BatchName]?.ToString() : "";
                // 3. 迁移DROP_HEAD到HISTORY_HEAD（字段过滤）
                var historyHeadDict = new Dictionary<string, object>();
                var historyHeadFields = TableDefinition.TableSchemas[HISTORY_HEAD.TableName].Select(f => f.Name).ToHashSet();
                foreach (DataColumn col in headRow.Table.Columns)
                {
                    if (historyHeadFields.Contains(col.ColumnName) && col.ColumnName != HISTORY_HEAD.MyID)
                        historyHeadDict[col.ColumnName] = headRow[col.ColumnName];
                }
                // 3.1 处理温度与步骤数据（txt文档）
                CupTempRecorder.Get(cupNum).StopRecord();
                historyHeadDict[HISTORY_HEAD.ProcessData] = CupTempRecorder.GetProcessData(cupNum) ?? new byte[0];
                historyHeadDict[HISTORY_HEAD.MarkStep] = CupTempRecorder.GetMarkStep(cupNum) ?? "";
                historyHeadDict[HISTORY_HEAD.FinishTime] = DateTime.Now;
                historyHeadDict[HISTORY_HEAD.HeadID] = headId;
                SqlServer.Insert(HISTORY_HEAD.TableName, historyHeadDict);

                // 4. 迁移DROP_DETAILS到HISTORY_DETAILS（字段过滤）
                var historyDetailsList = new List<Dictionary<string, object>>();
                var historyDetailsFields = TableDefinition.TableSchemas[HISTORY_DETAILS.TableName].Select(f => f.Name).ToHashSet();

                foreach (DataRow row in detailsRows.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var field in historyDetailsFields)
                    {
                        if (row.Table.Columns.Contains(field))
                            dict[field] = row[field];
                    }
                    historyDetailsList.Add(dict);
                }
                if (historyDetailsList.Count > 0)
                    SqlServer.Insert(HISTORY_DETAILS.TableName, historyDetailsList);

                // 5. 迁移DYE_DETAILS到HISTORY_DYE（字段过滤）
                string formulaCode = headRow.Table.Columns.Contains(DROP_HEAD.FormulaCode) ? headRow[DROP_HEAD.FormulaCode]?.ToString() : "";
                string versionNum = headRow.Table.Columns.Contains(DROP_HEAD.VersionNum) ? headRow[DROP_HEAD.VersionNum]?.ToString() : "";
                var historyDyeFields = TableDefinition.TableSchemas[HISTORY_DYE.TableName].Select(f => f.Name).ToHashSet();
                if (!string.IsNullOrEmpty(formulaCode) && !string.IsNullOrEmpty(versionNum))
                {
                    var handleRows = SqlServer.Select(
                        FORMULA_HANDLE_DETAILS.TableName,
                        null,
                        $"{FORMULA_HANDLE_DETAILS.FormulaCode} = @formulaCode AND {FORMULA_HANDLE_DETAILS.VersionNum} = @VersionNum",
                        FORMULA_HANDLE_DETAILS.No,
                        true,
                        new SqlParameter("@formulaCode", formulaCode),
                        new SqlParameter("@VersionNum", versionNum)
                    );
                    var historyDyeList = new List<Dictionary<string, object>>();
                    foreach (DataRow handleRow in handleRows.Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in handleRow.Table.Columns)
                        {
                            if (historyDyeFields.Contains(col.ColumnName))
                                dict[col.ColumnName] = handleRow[col.ColumnName];
                        }

                        int no = handleRow.Table.Columns.Contains("NO") && handleRow["NO"] != DBNull.Value ? Convert.ToInt32(handleRow["NO"]) : 0;
                        string techName = handleRow.Table.Columns.Contains(FORMULA_HANDLE_DETAILS.TechnologyName) ? handleRow[FORMULA_HANDLE_DETAILS.TechnologyName]?.ToString() : "";

                        double realDropWeightSum = dyeRows.AsEnumerable()
                             .Where(row =>
                                 row.Table.Columns.Contains(DYE_DETAILS.IndexNum) &&
                                 row[DYE_DETAILS.IndexNum] != DBNull.Value &&
                                 Convert.ToInt32(row[DYE_DETAILS.IndexNum]) == no &&
                                 row.Table.Columns.Contains(DYE_DETAILS.TechnologyName) &&
                                 (row[DYE_DETAILS.TechnologyName]?.ToString() ?? "") == techName
                             )
                             .Sum(row =>
                                 row.Table.Columns.Contains(DYE_DETAILS.RealDropWeight) &&
                                 double.TryParse(row[DYE_DETAILS.RealDropWeight]?.ToString(), out double val)
                                     ? val
                                     : 0
                             );
                        dict[HISTORY_DYE.RealDropWeight] = realDropWeightSum;
                        dict[HISTORY_DYE.CupNum] = cupNum;
                        dict[HISTORY_DYE.HeadID] = headId;
                        dict[HISTORY_DYE.BatchName] = batchName;
                        historyDyeList.Add(dict);
                    }
                    if (historyDyeList.Count > 0)
                        SqlServer.Insert(HISTORY_DYE.TableName, historyDyeList);
                }

                // 6. 删除原表数据
                SqlServer.Delete(DROP_HEAD.TableName, $"{DROP_HEAD.MyID} = @HeadID", new SqlParameter("@HeadID", headId));
                SqlServer.Delete(DROP_DETAILS.TableName, $"{DROP_DETAILS.HeadID} = @HeadID", new SqlParameter("@HeadID", headId));
                SqlServer.Delete(DYE_DETAILS.TableName, $"{DYE_DETAILS.HeadID} = @HeadID", new SqlParameter("@HeadID", headId));

                CupAuxiliary.ResetCupDetails(cupNum);
                Logger.Info($"{cupNum}杯归档完成（HeadID={headId}），数据已迁移");


            }
            catch (Exception ex)
            {
                Logger.Error("CupFinishSingle", ex);
            }

        }

        /// <summary>
        /// 归档后自动插入批次并自动滴液
        /// </summary>
        public static void HandleCupFinished(int cupNum, bool forceInsert = false)
        {
            // 防止重复归档

            var dropHead = DropBatchData.GetHeadData();
            if (dropHead != null && dropHead.AsEnumerable().Any(r => Convert.ToInt32(r[DROP_HEAD.CupNum]) == cupNum))
                return;

            // 获取等待列表

            var waitList = DropWaitData.GetData();
            if (waitList == null || waitList.Rows.Count == 0)
                return;
            var waitRows = waitList.AsEnumerable().OrderBy(r => Convert.ToInt32(r[WAIT_LIST.MyID])).ToList();

            // 获取主副杯状态
            CupData.GetData();
            var (mainInfo, subInfo) = GetMSCupInfo(cupNum);
            int mainCupNum = mainInfo.CupNum;
            int subCupNum = subInfo.CupNum;
            int mainEnable = mainInfo.Enable ?? 0;
            int subEnable = subInfo.Enable ?? 0;
            int mainIsUsing = mainInfo.IsUsing ?? 0;
            int subIsUsing = subInfo.IsUsing ?? 0;

            // 1. 主副杯都下线 或 有一个在使用，直接退出
            if (!forceInsert && ((mainEnable == 0 && subEnable == 0) || mainIsUsing == 1 || subIsUsing == 1))
                return;

            // 2. 只插入批次表，不分配批次号
            // 记录新插入的HeadID（其实后续批次号分配时会统一处理所有未分配批次号的批次）
            void InsertIfFound(DataRow waitRow, int cup)
            {
                if (waitRow != null)
                {
                    InsertDropBatchAndGetHeadId(waitRow, cup);
                    SqlServer.Delete(WAIT_LIST.TableName, $"{WAIT_LIST.MyID}=@MyID",
                        new SqlParameter("@MyID", waitRow[WAIT_LIST.MyID]));
                    waitRows.Remove(waitRow);
                }
            }

            if (mainEnable == 1 && subEnable == 0)
            {
                var mainWaitRow = waitRows.FirstOrDefault(r => Convert.ToInt32(r[WAIT_LIST.CupNum]) == mainCupNum);
                if (mainWaitRow != null)
                    InsertIfFound(mainWaitRow, mainCupNum);
                else
                {
                    var zeroWaitRow = waitRows.FirstOrDefault(r => Convert.ToInt32(r[WAIT_LIST.CupNum]) == 0);
                    if (zeroWaitRow != null)
                        InsertIfFound(zeroWaitRow, mainCupNum);
                }
            }
            else if (mainEnable == 0 && subEnable == 1)
            {
                var subWaitRow = waitRows.FirstOrDefault(r => Convert.ToInt32(r[WAIT_LIST.CupNum]) == subCupNum);
                if (subWaitRow != null)
                    InsertIfFound(subWaitRow, subCupNum);
                else
                {
                    var zeroWaitRow = waitRows.FirstOrDefault(r => Convert.ToInt32(r[WAIT_LIST.CupNum]) == 0);
                    if (zeroWaitRow != null)
                        InsertIfFound(zeroWaitRow, subCupNum);
                }
            }
            else if (mainEnable == 1 && subEnable == 1)
            {
                var mainWaitRow = waitRows.FirstOrDefault(r => Convert.ToInt32(r[WAIT_LIST.CupNum]) == mainCupNum);
                if (mainWaitRow != null)
                {
                    InsertIfFound(mainWaitRow, mainCupNum);
                    string mainDyeingCode = GetDyeingCodeFromWaitRow(mainWaitRow);

                    var subWaitRow = waitRows.FirstOrDefault(r =>
                        Convert.ToInt32(r[WAIT_LIST.CupNum]) == subCupNum &&
                        GetDyeingCodeFromWaitRow(r) == mainDyeingCode);
                    if (subWaitRow != null)
                        InsertIfFound(subWaitRow, subCupNum);
                    else
                    {
                        var zeroWaitRow = waitRows.FirstOrDefault(r =>
                            Convert.ToInt32(r[WAIT_LIST.CupNum]) == 0 &&
                            GetDyeingCodeFromWaitRow(r) == mainDyeingCode);
                        if (zeroWaitRow != null)
                            InsertIfFound(zeroWaitRow, subCupNum);
                    }
                }
                else
                {
                    var subWaitRow = waitRows.FirstOrDefault(r => Convert.ToInt32(r[WAIT_LIST.CupNum]) == subCupNum);
                    if (subWaitRow != null)
                    {
                        InsertIfFound(subWaitRow, subCupNum);
                        string subDyeingCode = GetDyeingCodeFromWaitRow(subWaitRow);
                        var zeroWaitRow = waitRows.FirstOrDefault(r =>
                            Convert.ToInt32(r[WAIT_LIST.CupNum]) == 0 &&
                            GetDyeingCodeFromWaitRow(r) == subDyeingCode);
                        if (zeroWaitRow != null)
                            InsertIfFound(zeroWaitRow, mainCupNum);
                    }
                    else
                    {
                        var zeroWaitRow = waitRows.FirstOrDefault(r => Convert.ToInt32(r[WAIT_LIST.CupNum]) == 0);
                        if (zeroWaitRow != null)
                        {
                            InsertIfFound(zeroWaitRow, mainCupNum);
                            string mainDyeingCode = GetDyeingCodeFromWaitRow(zeroWaitRow);
                            var zeroWaitRow2 = waitRows.FirstOrDefault(r =>
                                Convert.ToInt32(r[WAIT_LIST.CupNum]) == 0 &&
                                GetDyeingCodeFromWaitRow(r) == mainDyeingCode);
                            if (zeroWaitRow2 != null)
                                InsertIfFound(zeroWaitRow2, subCupNum);
                        }
                    }
                }
            }
            if (My_ConPar.Choices.UseAutoDrip == 1)
            {

                // 通知批次管理器有新批次可滴（只允许Type=3自动染杯）
                SmartColor.My_AutomaticModule.DropBatchManager.RequestBatchStart(
                row =>
                {
                    // 取杯号
                    int cupNo = row.Table.Columns.Contains(SmartColor.My_DataBase.DROP_HEAD.CupNum)
                        ? Convert.ToInt32(row[SmartColor.My_DataBase.DROP_HEAD.CupNum])
                        : 0;
                    // 查找CUP_DETAILS.Type
                    var dt = SqlServer.Select(SmartColor.My_DataBase.CUP_DETAILS.TableName, $"{SmartColor.My_DataBase.CUP_DETAILS.CupNum} = {cupNo}");
                    if (dt == null || dt.Rows.Count == 0) return false;
                    var cupRow = dt.Rows[0];
                    int type = cupRow.Table.Columns.Contains(SmartColor.My_DataBase.CUP_DETAILS.Type) && cupRow[SmartColor.My_DataBase.CUP_DETAILS.Type] != DBNull.Value
                        ? Convert.ToInt32(cupRow[SmartColor.My_DataBase.CUP_DETAILS.Type])
                        : 0;
                    return type == 3;
                }
            );
            }

            CupFinished?.Invoke();
        }



        /// <summary>
        /// 插入批次，返回新HeadID
        /// </summary>
        private static int? InsertDropBatchAndGetHeadId(DataRow waitRow, int targetCupNum)
        {

            SqlServer.Delete(DROP_HEAD.TableName, $"{DROP_HEAD.CupNum} = {targetCupNum}");
            SqlServer.Delete(DROP_DETAILS.TableName, $"{DROP_DETAILS.CupNum} = {targetCupNum}");
            SqlServer.Delete(My_DataBase.DYE_DETAILS.TableName, $"{DYE_DETAILS.CupNum} = {targetCupNum}");

            string formulaCode = waitRow[WAIT_LIST.FormulaCode]?.ToString();
            string versionNum = waitRow[WAIT_LIST.VersionNum]?.ToString();
            var formulaHead = SqlServer.Select(FORMULA_HEAD.TableName,
                $"{FORMULA_HEAD.FormulaCode}=@FormulaCode AND {FORMULA_HEAD.VersionNum}=@VersionNum",
                new SqlParameter("@FormulaCode", formulaCode),
                new SqlParameter("@VersionNum", versionNum));
            var formulaDetails = SqlServer.Select(FORMULA_DETAILS.TableName,
                $"{FORMULA_DETAILS.FormulaCode}=@FormulaCode AND {FORMULA_DETAILS.VersionNum}=@VersionNum",
                new SqlParameter("@FormulaCode", formulaCode),
                new SqlParameter("@VersionNum", versionNum));
            if (formulaHead != null && formulaHead.Rows.Count > 0)
            {
                var headDict = formulaHead.Rows[0].Table.Columns.Cast<DataColumn>().ToDictionary(
                    c => c.ColumnName, c => formulaHead.Rows[0][c.ColumnName]);
                headDict[DROP_HEAD.CupNum] = targetCupNum;
                if (headDict[DROP_HEAD.ClothNum] is int clothNum && clothNum == 0)
                {
                    headDict[DROP_HEAD.ClothNum] = targetCupNum;
                }

                headDict.Remove(FORMULA_HEAD.MyID);
                int newHeadId = SqlServer.InsertAndGetIdentity(DROP_HEAD.TableName, headDict);
                // 明细
                foreach (DataRow dr in formulaDetails.Rows)
                {
                    var dict = dr.Table.Columns.Cast<DataColumn>().ToDictionary(c => c.ColumnName, c => dr[c.ColumnName]);
                    dict[DROP_DETAILS.HeadID] = newHeadId;
                    dict[DROP_DETAILS.CupNum] = targetCupNum;
                    SqlServer.Insert(DROP_DETAILS.TableName, dict);
                }
                // 染色明细
                string dyeingCode = formulaHead.Rows[0][FORMULA_HEAD.DyeingCode]?.ToString();
                var dyeDetailList = FindDyeingCode.GetAllDyeDetailFromFormulaCode(dyeingCode, formulaCode, Convert.ToInt32(versionNum));
                foreach (var dict in dyeDetailList)
                {
                    dict[DYE_DETAILS.HeadID] = newHeadId;
                    dict[DYE_DETAILS.CupNum] = targetCupNum;
                    SqlServer.Insert(DYE_DETAILS.TableName, dict);
                }

                if(!string.IsNullOrEmpty(dyeingCode) )
                {
                    var(mainCup,subCup) = GetMSCupInfo(targetCupNum);
                    if(mainCup.CupNum != subCup.CupNum)
                    {
                        var currentCupNum = mainCup.CupNum == targetCupNum ? mainCup.CupNum : subCup.CupNum;
                        var otherCupNum = mainCup.CupNum == targetCupNum ? subCup.CupNum : mainCup.CupNum;
                        SyncMainSubCupStepNo(currentCupNum, otherCupNum);
                    }

                }

                return newHeadId;
            }
            return null;
        }

        public static void SyncMainSubCupStepNo(int cupNumA, int cupNumB)
        {
            // 获取所有IndexNum（段）
            var allRowsA = SqlServer.Select(
                My_DataBase.DYE_DETAILS.TableName, null,
                $"{My_DataBase.DYE_DETAILS.CupNum} = '{cupNumA}'",
                orderBy: $"{My_DataBase.DYE_DETAILS.StepNum}"
            );
            var allRowsB = SqlServer.Select(
                My_DataBase.DYE_DETAILS.TableName, null,
                $"{My_DataBase.DYE_DETAILS.CupNum} = '{cupNumB}'",
                orderBy: $"{My_DataBase.DYE_DETAILS.StepNum}"
            );
            if (allRowsB == null || allRowsB.Rows.Count == 0) return;

            var allIndexNums = allRowsA.Rows.Cast<DataRow>()
                .Select(r => Convert.ToInt32(r[My_DataBase.DYE_DETAILS.IndexNum]))
                .Union(allRowsB.Rows.Cast<DataRow>().Select(r => Convert.ToInt32(r[My_DataBase.DYE_DETAILS.IndexNum])))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            foreach (var indexNum in allIndexNums)
            {
                SyncSectionStepsByIndexNum(cupNumA, cupNumB, indexNum);
            }
        }

        private static void SyncSectionStepsByIndexNum(int cupNumA, int cupNumB, int indexNum)
        {
            while (true)
            {
                var stepsA = SqlServer.Select(
                    My_DataBase.DYE_DETAILS.TableName, null,
                    $"{My_DataBase.DYE_DETAILS.CupNum} = '{cupNumA}' AND {My_DataBase.DYE_DETAILS.IndexNum} = {indexNum}",
                    orderBy: My_DataBase.DYE_DETAILS.StepNum
                ).Rows.Cast<DataRow>().ToList();

                var stepsB = SqlServer.Select(
                    My_DataBase.DYE_DETAILS.TableName, null,
                    $"{My_DataBase.DYE_DETAILS.CupNum} = '{cupNumB}' AND {My_DataBase.DYE_DETAILS.IndexNum} = {indexNum}",
                    orderBy: My_DataBase.DYE_DETAILS.StepNum
                ).Rows.Cast<DataRow>().ToList();

                // 以步数多的为基准
                var baseSteps = stepsA.Count >= stepsB.Count ? stepsA : stepsB;
                var targetSteps = stepsA.Count < stepsB.Count ? stepsA : stepsB;
                var targetCupNum = stepsA.Count < stepsB.Count ? cupNumA : cupNumB;

                bool changed = false;

                for (int i = 0; i < baseSteps.Count; i++)
                {
                    string baseTech = baseSteps[i][My_DataBase.DYE_DETAILS.TechnologyName]?.ToString() ?? "";
                    string targetTech = i < targetSteps.Count ? targetSteps[i][My_DataBase.DYE_DETAILS.TechnologyName]?.ToString() ?? "" : null;
                    var baseStep = baseSteps[i];
                    var stepNum = Convert.ToInt32(baseStep[My_DataBase.DYE_DETAILS.StepNum]);
                    if (baseTech != targetTech)
                    {
                        // 插入缺失步骤
                        InsertStepAndShiftInSection(targetCupNum, indexNum, stepNum, baseStep);
                        changed = true;
                        break;
                    }
                }

                // 如果目标步数比基准少，补齐剩余
                if (!changed && targetSteps.Count < baseSteps.Count)
                {
                    for (int i = targetSteps.Count; i < baseSteps.Count; i++)
                    {
                        var baseStep = baseSteps[i];
                        var stepNum = Convert.ToInt32(baseStep[My_DataBase.DYE_DETAILS.StepNum]);
                        InsertStepAndShiftInSection(targetCupNum, indexNum, stepNum, baseStep);
                        changed = true;
                    }
                }

                if (!changed)
                    break; // 本段已一致
            }
        }

        private static void InsertStepAndShiftInSection(int cupNum, int indexNum, int stepNum, DataRow templateRow)
        {
            // 1. 后续所有StepNum递增（倒序处理）
            var rows = SqlServer.Select(
                My_DataBase.DYE_DETAILS.TableName, null,
                $"{My_DataBase.DYE_DETAILS.CupNum} = '{cupNum}' AND {My_DataBase.DYE_DETAILS.IndexNum} = {indexNum} AND {My_DataBase.DYE_DETAILS.StepNum} >= {stepNum}",
                orderBy: $"{My_DataBase.DYE_DETAILS.StepNum}",
                ascending: false
            ).Rows.Cast<DataRow>().ToList();

            foreach (DataRow row in rows)
            {
                int oldStepNum = Convert.ToInt32(row[My_DataBase.DYE_DETAILS.StepNum]);
                SqlServer.Update(
                    My_DataBase.DYE_DETAILS.TableName,
                    new Dictionary<string, object> { { My_DataBase.DYE_DETAILS.StepNum, oldStepNum + 1 } },
                    $"{My_DataBase.DYE_DETAILS.StepNum} = '{row[My_DataBase.DYE_DETAILS.StepNum]}' AND {My_DataBase.DYE_DETAILS.CupNum} = '{cupNum}' AND {My_DataBase.DYE_DETAILS.IndexNum} = {indexNum}"
                );
            }

            // 2. 插入新步骤
            var dict = CopyDyeDetailRowAsDict(templateRow, cupNum);
            dict[My_DataBase.DYE_DETAILS.IndexNum] = indexNum;
            dict[My_DataBase.DYE_DETAILS.StepNum] = stepNum;

            SqlServer.Insert(My_DataBase.DYE_DETAILS.TableName, dict);
        }

        // 辅助方法：复制一行为占位，药量为0，杯号为目标杯号
        private static Dictionary<string, object> CopyDyeDetailRowAsDict(DataRow src, int cupNum)
        {
            var dict = new Dictionary<string, object>();
            // 先查目标杯号当前批次的 HeadID, BatchName, FormulaCode, VersionNum
            var cupRows = SqlServer.Select(
                My_DataBase.DYE_DETAILS.TableName,
                $"{My_DataBase.DYE_DETAILS.CupNum} = '{cupNum}'"
            );
            DataRow cupRow = cupRows.Rows.Count > 0 ? cupRows.Rows[0] : null;

            foreach (DataColumn col in src.Table.Columns)
            {
                switch (col.ColumnName)
                {
                    case "CupNum":
                        dict[col.ColumnName] = cupNum;
                        break;
                    case "ObjectDropWeight":
                    case "RealDropWeight":
                        dict[col.ColumnName] = 0;
                        break;
                    case "HeadID":
                    case "BatchName":
                    case "FormulaCode":
                    case "VersionNum":
                        if (cupRow != null)
                            dict[col.ColumnName] = cupRow[col.ColumnName];
                        else
                            dict[col.ColumnName] = src[col];
                        break;
                    default:
                        dict[col.ColumnName] = src[col];
                        break;
                }
            }
            return dict;
        }



        /// <summary>
        /// 从等待行获取染固色工艺代码
        /// </summary>
        private static string GetDyeingCodeFromWaitRow(DataRow waitRow)
        {
            string formulaCode = waitRow[WAIT_LIST.FormulaCode]?.ToString();
            string versionNum = waitRow[WAIT_LIST.VersionNum]?.ToString();
            var formulaHead = SqlServer.Select(FORMULA_HEAD.TableName,
                $"{FORMULA_HEAD.FormulaCode}=@FormulaCode AND {FORMULA_HEAD.VersionNum}=@VersionNum",
                new SqlParameter("@FormulaCode", formulaCode),
                new SqlParameter("@VersionNum", versionNum));
            if (formulaHead != null && formulaHead.Rows.Count > 0)
                return formulaHead.Rows[0][FORMULA_HEAD.DyeingCode]?.ToString();
            return null;
        }


        public static void UpdateWaitList(int cupNum)
        {
            Task.Run(async () =>
            {
                try
                {
                    // 触发事件，通知界面刷新
                    CupFinished?.Invoke();
                    await Task.Yield(); // 确保数据先被上层处理，再继续执行后续逻辑
                    await Task.Delay(60000);
                    HandleCupFinished(cupNum);
                }
                catch (Exception ex)
                {
                    Logger.Error("更新待机列表数据失败", ex);
                }
            });

        }

       

        /// <summary>
        /// 设置杯详情为初始状态
        /// 优化：无明显冗余
        /// </summary>
        /// <param name="cupNum">杯号</param>
        public static void ResetCupDetails(int cupNum)
        {
            var updateDict = new Dictionary<string, object>
            {
                { CUP_DETAILS.Statues, "待机" },
                { CUP_DETAILS.Enable, 1 },
                { CUP_DETAILS.IsUsing, 0 },
                { CUP_DETAILS.DyeingCode, DBNull.Value },
                { CUP_DETAILS.FormulaCode, DBNull.Value },
                { CUP_DETAILS.CurrentWeight, 0 },
                { CUP_DETAILS.TotalWeight, 0 },
                { CUP_DETAILS.SetTime, 0 },
                { CUP_DETAILS.DyeType, DBNull.Value },
                { CUP_DETAILS.StartTime, DBNull.Value },
                { CUP_DETAILS.StepStartTime, DBNull.Value },
                { CUP_DETAILS.StepNum, DBNull.Value },
                { CUP_DETAILS.TotalStep, DBNull.Value },
                { CUP_DETAILS.TechnologyName, DBNull.Value },
                { CUP_DETAILS.SetTemp, DBNull.Value },
                { CUP_DETAILS.RecordIndex, 0 },
                { CUP_DETAILS.Cooperate, 0 },
                { CUP_DETAILS.Fail, 0 },
                { CUP_DETAILS.CurrentStepFinish,0 },
                { CUP_DETAILS.ReceptionTime, DBNull.Value }
            };
            SqlServer.Update(CUP_DETAILS.TableName, updateDict, $"{CUP_DETAILS.CupNum} = @CupNum", new SqlParameter("@CupNum", cupNum));
            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNum);
            if (area != null)
            {
                area.OnCupDataReceived(cupNum);
            }
        }

        /// <summary>
        /// 更新出入布状态
        /// 优化：用GetMSCupInfo简化主副杯详情获取
        /// </summary>
        /// <param name="cupNum">杯号</param>
        /// <param name="haveCloth">0=出布：1=放布</param>
        public static void UpdateHaveCloth(int cupNum, int haveCloth)
        {
            var (mainCup, subCup) = GetCupPair(cupNum);
            var (mainInfo, subInfo) = GetMSCupInfo(cupNum);

            // 判断主副杯“放布/出布”完成状态
            bool mainInCloth = haveCloth == 1 ?
                (mainInfo.TechnologyName == "放布" && mainInfo.HeadID > 0 && !string.IsNullOrEmpty(mainInfo.StepNum)) :
                (mainInfo.TechnologyName == "出布" && !string.IsNullOrEmpty(mainInfo.StepNum));
            bool subInCloth = haveCloth == 1 ?
                (subInfo.TechnologyName == "放布" && subInfo.HeadID > 0 && !string.IsNullOrEmpty(subInfo.StepNum)) :
                (subInfo.TechnologyName == "出布" && !string.IsNullOrEmpty(subInfo.StepNum));

            int mainHeadID = mainInfo.HeadID ?? 0;
            int subHeadID = subInfo.HeadID ?? 0;
            int mainStepNum = 0, subStepNum = 0;
            int.TryParse(mainInfo.StepNum, out mainStepNum);
            int.TryParse(subInfo.StepNum, out subStepNum);

            if (mainInCloth && mainInfo.IsUsing == 1)
            {

                SqlServer.Update(CUP_DETAILS.TableName,
                    new Dictionary<string, object> { { CUP_DETAILS.HaveCloth, haveCloth } },
                    $"{CUP_DETAILS.CupNum} = @cupNo",
                    new SqlParameter("@cupNo", mainCup));

                SqlServer.Update(DYE_DETAILS.TableName,
                    new Dictionary<string, object> { { DYE_DETAILS.Finish, 1 } },
                    $"{DYE_DETAILS.HeadID} = @headID AND {DYE_DETAILS.StepNum} = @stepNo",
                    new SqlParameter("@headID", mainHeadID),
                    new SqlParameter("@stepNo", mainStepNum));

            }
            if (subInCloth && subInfo.IsUsing == 1)
            {
                SqlServer.Update(CUP_DETAILS.TableName,
                    new Dictionary<string, object> { { CUP_DETAILS.HaveCloth, haveCloth } },
                    $"{CUP_DETAILS.CupNum} = @cupNo",
                    new SqlParameter("@cupNo", subCup));

                SqlServer.Update(DYE_DETAILS.TableName,
                    new Dictionary<string, object> { { DYE_DETAILS.Finish, 1 } },
                    $"{DYE_DETAILS.HeadID} = @headID AND {DYE_DETAILS.StepNum} = @stepNo",
                    new SqlParameter("@headID", subHeadID),
                    new SqlParameter("@stepNo", subStepNum));
            }


        }

        /// <summary>
        /// 放布确认结果结构体
        /// </summary>
        public struct PutClothConfirmResult
        {
            public ushort sureKey;
            public int dyeType;
            public int headID;
            public int stepNo;
            public string dyeingCode;
        }

        /// <summary>
        /// 根据主杯号判断放布确认值（0/1/2/3）
        /// 优化：用GetMSCupInfo简化主副杯详情获取
        /// </summary>
        public static PutClothConfirmResult GetPutClothConfirmValue(int cupNo)
        {

            var (mainInfo, subInfo) = GetMSCupInfo(cupNo);

            string mainTech = mainInfo.TechnologyName;
            string subTech = subInfo.TechnologyName;
            string mainDyeingCode = mainInfo.DyeingCode;
            string subDyeingCode = subInfo.DyeingCode;
            int mainUse = mainInfo.IsUsing ?? 0;
            int subUse = subInfo.IsUsing ?? 0;
            int mainHeadID = mainInfo.HeadID ?? 0;
            int subHeadID = subInfo.HeadID ?? 0;
            int mainStepNum = 0, subStepNum = 0;
            int.TryParse(mainInfo.StepNum, out mainStepNum);
            int.TryParse(subInfo.StepNum, out subStepNum);

            // 判断主副杯“放布”完成状态
            bool mainInCloth = mainUse == 1 && mainTech == "放布" && mainHeadID > 0 && mainStepNum > 0;
            bool subInCloth = subUse == 1 && subTech == "放布" && subHeadID > 0 && subStepNum > 0;
            bool mainInClothFinish = false, subInClothFinish = false;
            if (mainInCloth)
            {
                var dt = SqlServer.Select(DYE_DETAILS.TableName, $"{DYE_DETAILS.HeadID} = {mainHeadID} AND {DYE_DETAILS.StepNum} = {mainStepNum} AND {DYE_DETAILS.TechnologyName} = '放布'");
                if (dt.Rows.Count > 0 && dt.Rows[0][DYE_DETAILS.Finish] != DBNull.Value)
                    mainInClothFinish = Convert.ToInt32(dt.Rows[0][DYE_DETAILS.Finish]) == 1;
            }
            if (subInCloth)
            {
                var dt = SqlServer.Select(DYE_DETAILS.TableName, $"{DYE_DETAILS.HeadID} = {subHeadID} AND {DYE_DETAILS.StepNum} = {subStepNum} AND {DYE_DETAILS.TechnologyName} = '放布'");
                if (dt.Rows.Count > 0 && dt.Rows[0][DYE_DETAILS.Finish] != DBNull.Value)
                    subInClothFinish = Convert.ToInt32(dt.Rows[0][DYE_DETAILS.Finish]) == 1;
            }

            // 1. 有“放布”未完成，优先返回未完成的那只杯
            if ((mainInCloth && !mainInClothFinish) || (subInCloth && !subInClothFinish))
            {
                bool useMain = mainInCloth && !mainInClothFinish;
                int useHeadID = useMain ? mainHeadID : subHeadID;
                int useStepNum = useMain ? mainStepNum : subStepNum;
                string useDyeingCode = useMain ? mainDyeingCode : subDyeingCode;
                ushort confirmValue = SmartColor.My_ConPar.Choices.UseClamp == 1 ? (ushort)1 : (ushort)2;


                var (dyeType, technologyName) = GetCurrentStepDyeType(useMain ? mainInfo.CupNum : subInfo.CupNum);
                if (dyeType == -1) dyeType = 0;

                return new PutClothConfirmResult
                {
                    sureKey = confirmValue,
                    dyeType = dyeType,
                    headID = useHeadID,
                    stepNo = useStepNum,
                    dyeingCode = useDyeingCode
                };
            }

            // 2. 都“放布”且都完成，返回3和主杯headID/stepNo
            if ((mainInCloth && mainInClothFinish) || (subInCloth && subInClothFinish))
            {
                bool useMain = mainInCloth && !mainInClothFinish;
                int useHeadID = mainInCloth ? mainHeadID : subHeadID;
                int useStepNum = mainInCloth ? mainStepNum : subStepNum;
                string useDyeingCode = useMain ? mainDyeingCode : subDyeingCode;
                return new PutClothConfirmResult
                {
                    sureKey = 3,
                    dyeType = 0,
                    headID = useHeadID,
                    stepNo = useStepNum,
                    dyeingCode = useDyeingCode
                };
            }

            // 3. 另一杯不是放布，查批次表和等待列表是否有另一杯的配方数据
            bool hasBatchTask = false, hasWaitTask = false;

            var batchRows = SqlServer.Select(DROP_HEAD.TableName, $"{DROP_HEAD.CupNum} = {subInfo.CupNum}");
            hasBatchTask = batchRows.Rows.Count > 0;


            var waitRows = SqlServer.Select(WAIT_LIST.TableName, $"{WAIT_LIST.CupNum} = {subInfo.CupNum}");
            hasWaitTask = waitRows.Rows.Count > 0;

            if (hasBatchTask || hasWaitTask)
                return new PutClothConfirmResult
                {
                    sureKey = 0,
                    dyeType = 0,
                    headID = 0,
                    stepNo = 0
                };

            // 4. 其它情况
            return new PutClothConfirmResult
            {
                sureKey = 0,
                dyeType = 0,
                headID = 0,
                stepNo = 0
            };
        }

        /// <summary>
        /// 根据主杯号判断出布确认值（1=自动出布，2=显示确定键，3=都已完成）
        /// 优化：用GetMSCupInfo简化主副杯详情获取
        /// </summary>
        public static PutClothConfirmResult GetOutClothConfirmValue(int cupNo)
        {

            var (mainInfo, subInfo) = GetMSCupInfo(cupNo);

            string mainTech = mainInfo.TechnologyName;
            string subTech = subInfo.TechnologyName;
            string mainDyeingCode = mainInfo.DyeingCode;
            string subDyeingCode = subInfo.DyeingCode;
            int mainUse = mainInfo.IsUsing ?? 0;
            int subUse = subInfo.IsUsing ?? 0;
            int mainHeadID = mainInfo.HeadID ?? 0;
            int subHeadID = subInfo.HeadID ?? 0;
            int mainStepNum = 0, subStepNum = 0;
            int mainHaveCloth = mainInfo.HaveCloth ?? 0;
            int subHaveCloth = subInfo.HaveCloth ?? 0;
            int.TryParse(mainInfo.StepNum, out mainStepNum);
            int.TryParse(subInfo.StepNum, out subStepNum);

            // 判断主副杯“出布”完成状态
            bool mainOutCloth = mainUse == 1 && mainTech == "出布" && mainStepNum > 0;
            bool subOutCloth = subUse == 1 && subTech == "出布" && subStepNum > 0;
            bool mainOutClothFinish = mainOutCloth && mainHaveCloth == 0;
            bool subOutClothFinish = subOutCloth && subHaveCloth == 0;

            // 1. 有“出布”未完成，优先返回未完成的那只杯
            if ((mainOutCloth && !mainOutClothFinish) || (subOutCloth && !subOutClothFinish))
            {
                bool useMain = mainOutCloth && !mainOutClothFinish;
                int useHeadID = useMain ? mainHeadID : subHeadID;
                int useStepNum = useMain ? mainStepNum : subStepNum;
                string useDyeingCode = useMain ? mainDyeingCode : subDyeingCode;
                ushort confirmValue = SmartColor.My_ConPar.Choices.UseClampOut == 1 ? (ushort)1 : (ushort)2;

                //判断是否是洗杯工艺,洗杯工艺全部人工出布
                if (mainOutCloth)
                {
                    bool mainIsWash = !string.IsNullOrEmpty(mainDyeingCode) && CupAuxiliary.WashCupDic.ContainsKey(mainDyeingCode);
                    if (mainIsWash)
                        confirmValue = 2;
                }
                else
                {
                    bool subIsWash = !string.IsNullOrEmpty(subDyeingCode) && CupAuxiliary.WashCupDic.ContainsKey(subDyeingCode);
                    if (subIsWash)
                        confirmValue = 2;
                }


                var (dyeType, technologyName) = GetCurrentStepDyeType(useMain ? mainInfo.CupNum : subInfo.CupNum);
                if (dyeType == -1) dyeType = 0;

                return new PutClothConfirmResult
                {
                    sureKey = confirmValue,
                    dyeType = dyeType,
                    headID = useHeadID,
                    stepNo = useStepNum,
                    dyeingCode = useDyeingCode
                };
            }

            // 2. 都“出布”且都完成，返回3和主杯headID/stepNo
            if ((mainOutCloth && mainOutClothFinish) || (subOutCloth && subOutClothFinish))
            {
                bool useMain = mainOutCloth && !mainOutClothFinish;
                int useHeadID = mainOutCloth ? mainHeadID : subHeadID;
                int useStepNum = mainOutCloth ? mainStepNum : subStepNum;
                string useDyeingCode = useMain ? mainDyeingCode : subDyeingCode;
                return new PutClothConfirmResult
                {
                    sureKey = 3,
                    dyeType = 0,
                    headID = useHeadID,
                    stepNo = useStepNum,
                    dyeingCode = useDyeingCode
                };
            }

            // 3. 其它情况，直接返回2（显示确定键）
            return new PutClothConfirmResult
            {
                sureKey = 2,
                dyeType = 0,
                headID = 0,
                stepNo = 0
            };
        }

        /// <summary>
        /// 异步更新杯状态
        /// 优化：无明显冗余
        /// </summary>
        public static void UpdateCupState(int cupNo, string status)
        {
            Task.Run(() =>
            {
                if (status == "下线")
                {
                    OffLineCupDetails(cupNo);
                }

                else
                {
                    SqlServer.Update(CUP_DETAILS.TableName,
                        new Dictionary<string, object>
                        {
                            {CUP_DETAILS.Statues, status }
                        },
                        $"{CUP_DETAILS.CupNum} = @cupNo",
                        new SqlParameter("@cupNo", cupNo));
                }


            });
        }

        public static void OffLineCupDetails(int cupNum)
        {
            SqlServer.Update(CUP_DETAILS.TableName,
                       new Dictionary<string, object>
                       {
                            { CUP_DETAILS.Statues, "下线" },
                            { CUP_DETAILS.Enable, 0 },
                            { CUP_DETAILS.IsUsing, 0 },
                            { CUP_DETAILS.DyeingCode, DBNull.Value },
                            { CUP_DETAILS.FormulaCode, DBNull.Value },
                            { CUP_DETAILS.CurrentWeight, 0 },
                            { CUP_DETAILS.TotalWeight, 0 },
                            { CUP_DETAILS.SetTime, 0 },
                            { CUP_DETAILS.DyeType, DBNull.Value },
                            { CUP_DETAILS.StartTime, DBNull.Value },
                            { CUP_DETAILS.StepStartTime, DBNull.Value },
                            { CUP_DETAILS.StepNum, DBNull.Value },
                            { CUP_DETAILS.TotalStep, DBNull.Value },
                            { CUP_DETAILS.TechnologyName, DBNull.Value },
                            { CUP_DETAILS.SetTemp, DBNull.Value },
                            { CUP_DETAILS.RecordIndex, 0 },
                            { CUP_DETAILS.Cooperate, 0 },
                            { CUP_DETAILS.Fail, 0 },

                            { CUP_DETAILS.ReceptionTime, DBNull.Value }
                       },
                       $"{CUP_DETAILS.CupNum} = @cupNo",
                       new SqlParameter("@cupNo", cupNum));
            var area = CupCommManager.Instance.FindCupAreaByCupNum(cupNum);
            if (area != null)
            {
                area.OnCupDataReceived(cupNum);
            }
        }

        /// <summary>
        /// 异步更新杯工艺名称和步号
        /// 优化：无明显冗余
        /// </summary>
        public static void UpdateCupTechnologyName(int cupNo, string technologyName, int stepNo)
        {
            Task.Run(() =>
            {
                SqlServer.Update(CUP_DETAILS.TableName,
                    new Dictionary<string, object>
                    {
                        {CUP_DETAILS.TechnologyName, technologyName},
                        {CUP_DETAILS.StepNum, stepNo}
                    },
                    $"{CUP_DETAILS.CupNum} = @cupNo",
                    new SqlParameter("@cupNo", cupNo));
            });
        }

        /// <summary>
        /// 配液杯详情结构体，包含cup_details表所有字段
        /// </summary>
        public struct CupDetailInfo
        {
            public int CupNum;
            public int? MainCupNum;
            public int? IsFixed;
            public int? Enable;
            public int? IsUsing;
            public string Statues;
            public string FormulaCode;
            public string DyeingCode;
            public DateTime? StartTime;
            public decimal? SetTemp;
            public decimal? RealTemp;
            public decimal? TotalWeight;
            public decimal? CurrentWeight;
            public string StepNum;
            public string TotalStep;
            public string TechnologyName;
            public DateTime? StepStartTime;
            public int? SetTime;
            public int? RecordIndex;
            public int? Cooperate;
            public int? Type;
            public int? CoverStatus;
            public int? Fail;
            public int? HeadID;
            public int? HaveCloth;
            public int? CurrentStepFinish;
            public DateTime? ReceptionTime;
            public int? DyeType;

        }

        /// <summary>
        /// 获取使用状态的主副杯详情
        /// 优化：已是最优实现
        /// </summary>
        /// <param name="cupNo">杯号</param>
        /// <returns>主杯详情，副杯详情</returns>
        public static (CupDetailInfo mainCup, CupDetailInfo subCup) GetMSCupInfo(int cupNo)
        {

            var (mainCupNum, subCupNum) = GetCupPair(cupNo);

            CupDetailInfo GetCupDetail(int cupNum)
            {
                var dt = SqlServer.Select(CUP_DETAILS.TableName, $"{CUP_DETAILS.CupNum} = {cupNum}");
                if (dt == null || dt.Rows.Count == 0)
                    return default;

                var row = dt.Rows[0];
                CupDetailInfo info = new CupDetailInfo
                {
                    CupNum = row[CUP_DETAILS.CupNum] != DBNull.Value ? Convert.ToInt32(row[CUP_DETAILS.CupNum]) : 0,
                    MainCupNum = row.Table.Columns.Contains(CUP_DETAILS.MainCupNum) && row[CUP_DETAILS.MainCupNum] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.MainCupNum]) : null,
                    IsFixed = row.Table.Columns.Contains(CUP_DETAILS.IsFixed) && row[CUP_DETAILS.IsFixed] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.IsFixed]) : null,
                    Enable = row.Table.Columns.Contains(CUP_DETAILS.Enable) && row[CUP_DETAILS.Enable] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.Enable]) : null,
                    IsUsing = row.Table.Columns.Contains(CUP_DETAILS.IsUsing) && row[CUP_DETAILS.IsUsing] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.IsUsing]) : null,
                    Statues = row.Table.Columns.Contains(CUP_DETAILS.Statues) && row[CUP_DETAILS.Statues] != DBNull.Value ? row[CUP_DETAILS.Statues].ToString() : null,
                    FormulaCode = row.Table.Columns.Contains(CUP_DETAILS.FormulaCode) && row[CUP_DETAILS.FormulaCode] != DBNull.Value ? row[CUP_DETAILS.FormulaCode].ToString() : null,
                    DyeingCode = row.Table.Columns.Contains(CUP_DETAILS.DyeingCode) && row[CUP_DETAILS.DyeingCode] != DBNull.Value ? row[CUP_DETAILS.DyeingCode].ToString() : null,
                    StartTime = row.Table.Columns.Contains(CUP_DETAILS.StartTime) && row[CUP_DETAILS.StartTime] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row[CUP_DETAILS.StartTime]) : null,
                    SetTemp = row.Table.Columns.Contains(CUP_DETAILS.SetTemp) && row[CUP_DETAILS.SetTemp] != DBNull.Value ? (decimal?)Convert.ToDecimal(row[CUP_DETAILS.SetTemp]) : null,
                    RealTemp = row.Table.Columns.Contains(CUP_DETAILS.RealTemp) && row[CUP_DETAILS.RealTemp] != DBNull.Value ? (decimal?)Convert.ToDecimal(row[CUP_DETAILS.RealTemp]) : null,
                    TotalWeight = row.Table.Columns.Contains(CUP_DETAILS.TotalWeight) && row[CUP_DETAILS.TotalWeight] != DBNull.Value ? (decimal?)Convert.ToDecimal(row[CUP_DETAILS.TotalWeight]) : null,
                    CurrentWeight = row.Table.Columns.Contains(CUP_DETAILS.CurrentWeight) && row[CUP_DETAILS.CurrentWeight] != DBNull.Value ? (decimal?)Convert.ToDecimal(row[CUP_DETAILS.CurrentWeight]) : null,
                    StepNum = row.Table.Columns.Contains(CUP_DETAILS.StepNum) && row[CUP_DETAILS.StepNum] != DBNull.Value ? row[CUP_DETAILS.StepNum].ToString() : null,
                    TotalStep = row.Table.Columns.Contains(CUP_DETAILS.TotalStep) && row[CUP_DETAILS.TotalStep] != DBNull.Value ? row[CUP_DETAILS.TotalStep].ToString() : null,
                    TechnologyName = row.Table.Columns.Contains(CUP_DETAILS.TechnologyName) && row[CUP_DETAILS.TechnologyName] != DBNull.Value ? row[CUP_DETAILS.TechnologyName].ToString() : null,
                    StepStartTime = row.Table.Columns.Contains(CUP_DETAILS.StepStartTime) && row[CUP_DETAILS.StepStartTime] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row[CUP_DETAILS.StepStartTime]) : null,
                    SetTime = row.Table.Columns.Contains(CUP_DETAILS.SetTime) && row[CUP_DETAILS.SetTime] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.SetTime]) : null,
                    RecordIndex = row.Table.Columns.Contains(CUP_DETAILS.RecordIndex) && row[CUP_DETAILS.RecordIndex] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.RecordIndex]) : null,
                    Cooperate = row.Table.Columns.Contains(CUP_DETAILS.Cooperate) && row[CUP_DETAILS.Cooperate] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.Cooperate]) : null,
                    Type = row.Table.Columns.Contains(CUP_DETAILS.Type) && row[CUP_DETAILS.Type] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.Type]) : null,
                    CoverStatus = row.Table.Columns.Contains(CUP_DETAILS.CoverStatus) && row[CUP_DETAILS.CoverStatus] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.CoverStatus]) : null,
                    Fail = row.Table.Columns.Contains(CUP_DETAILS.Fail) && row[CUP_DETAILS.Fail] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.Fail]) : null,
                    HeadID = row.Table.Columns.Contains(CUP_DETAILS.HeadID) && row[CUP_DETAILS.HeadID] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.HeadID]) : null,
                    HaveCloth = row.Table.Columns.Contains(CUP_DETAILS.HaveCloth) && row[CUP_DETAILS.HaveCloth] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.HaveCloth]) : null,
                    CurrentStepFinish = row.Table.Columns.Contains(CUP_DETAILS.CurrentStepFinish) && row[CUP_DETAILS.CurrentStepFinish] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.CurrentStepFinish]) : null,
                    ReceptionTime = row.Table.Columns.Contains(CUP_DETAILS.ReceptionTime) && row[CUP_DETAILS.ReceptionTime] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row[CUP_DETAILS.ReceptionTime]) : null,
                    DyeType = row.Table.Columns.Contains(CUP_DETAILS.DyeType) && row[CUP_DETAILS.DyeType] != DBNull.Value ? (int?)Convert.ToInt32(row[CUP_DETAILS.DyeType]) : null,
                };
                return info;
            }


            var mainCup = GetCupDetail(mainCupNum);
            if (mainCupNum == subCupNum) // 主副杯相同，说明没有副杯
            {
                return (mainCup, mainCup);
            }

            var subCup = GetCupDetail(subCupNum);

            return (mainCup, subCup);
        }

        /// <summary>
        /// 获取主副杯中当前步未完成指定工艺的杯号列表
        /// </summary>
        /// <param name="cupInfos">主副杯信息列表</param>
        /// <param name="technologyName">工艺名称（如"加水"、"加药"、"放布"）</param>
        /// <returns>需要动作的杯号列表</returns>
        public static List<int> GetNeedActionCupNos(List<My_Tool.CupAuxiliary.CupDetailInfo> cupInfos, string technologyName)
        {
            var needCupNos = new List<int>();
            foreach (var info in cupInfos)
            {
                if (info.IsUsing == 1 && info.TechnologyName == technologyName)
                {
                    // 直接通过CurrentStepFinish字段判断
                    if (info.CurrentStepFinish != 1)
                    {
                        needCupNos.Add(info.CupNum);
                    }
                }
            }
            return needCupNos;
        }

        public static void ClearBatchCupData(int cupNo)
        {
            My_DataBase.SqlServer.Delete(My_DataBase.DROP_HEAD.TableName, $"{My_DataBase.DROP_HEAD.CupNum} = @CupNum",
                   new System.Data.SqlClient.SqlParameter("@CupNum", cupNo));
            My_DataBase.SqlServer.Delete(My_DataBase.DROP_DETAILS.TableName, $"{My_DataBase.DROP_DETAILS.CupNum} = @CupNum",
                new System.Data.SqlClient.SqlParameter("@CupNum", cupNo));
            My_DataBase.SqlServer.Delete(My_DataBase.DYE_DETAILS.TableName, $"{My_DataBase.DYE_DETAILS.CupNum} = @CupNum",
                new System.Data.SqlClient.SqlParameter("@CupNum", cupNo));
            CupFinished?.Invoke();
        }

        /// <summary>
        /// 查找主副杯中正在使用的杯，判断下一步工艺（支持洗杯/染色/后处理）
        /// </summary>
        public static async Task SendNextStepIfNeeded(int cupNo)
        {
            await Task.Run(() =>
             {
                 var (mainCup, subCup) = My_Tool.CupAuxiliary.GetMSCupInfo(cupNo);
                 My_Tool.CupAuxiliary.CupDetailInfo useCup = mainCup.IsUsing == 1 ? mainCup : (subCup.IsUsing == 1 ? subCup : mainCup);

                 bool isWash = My_Tool.CupAuxiliary.WashCupDic.ContainsKey(useCup.DyeingCode ?? "");
                 int stepNo = 0;
                 int.TryParse(useCup.StepNum, out stepNo);
                 int headID = useCup.HeadID ?? 0;

                 My_Tool.CupAuxiliary.StepInfo nextStepInfo = isWash
                     ? My_Tool.CupAuxiliary.GetNextWashCupStepInfo(useCup.DyeingCode ?? "", stepNo)
                     : My_Tool.CupAuxiliary.GetNextStepInfo(headID, stepNo);



                 var area = CupCommManager.Instance.FindCupAreaByCupNum(useCup.CupNum);
                 if (area != null)
                 {
                     var comm = CupCommManager.Instance.GetCommObject(area);
                     if (comm != null)
                     {
                         Task.Run(async () =>
                         {
                             await comm.SendNextStep(useCup.CupNum, nextStepInfo);
                             await comm.SendAddChemicaFinish(useCup.CupNum);
                         });

                     }
                 }
             });
        }

        /// <summary>
        /// 等待锁止信号
        /// </summary>
        /// <param name="cup">杯号</param>
        /// <param name="allowAlarm">超时是否允许报警</param>
        /// <returns></returns>
        public static async Task<bool> WaitLockUpOk(int cup, bool allowAlarm = true)
        {
            var ctCup = await CupCommManager.Instance.FindCupByCupNumAsync(cup);
            const int timeoutMs = 60000;
            const int pollMs = 500;
            const int requiredCount = 3;
            int waited = 0;
            bool alarmed = false;
            int lockUpCount = 0;
            while (true)
            {
                await Task.Delay(pollMs);
                waited += pollMs;

              

                if (lockUpCount >= requiredCount)
                {
                    return true;
                }


                if (ctCup.LockStatus == 1)
                {
                    lockUpCount++;

                }
                else
                    lockUpCount = 0;

                if (waited >= timeoutMs && !alarmed && allowAlarm)
                {
                    if (ctCup.Status == "待机")
                    {
                        Logger.Error($"{My_Tool.CupAuxiliary.GetIsUseing(cup)}号杯状态变为待机，停止等待锁止信号");
                        return false;
                    }
                    else
                    {
                        var useName = My_Tool.CupAuxiliary.GetIsUseing(cup);
                        Logger.Error($"{useName}号杯锁止上信号等待超时");
                        alarmed = true;
                    }
                   
                    // 继续等待
                }
            }
        }
        #endregion
    }
}