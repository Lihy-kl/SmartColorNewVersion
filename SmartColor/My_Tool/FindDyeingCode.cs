using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartColor.My_Tool
{
    internal class FindDyeingCode
    {
        /// <summary>
        /// 获取染固色工艺详情。
        /// 根据传入的染固色工艺代码，优先从当前工艺表中查找对应的工艺详情，
        /// 若未找到则从历史工艺表中查找，并进一步获取历史工艺流程明细。
        /// 返回工艺主表行集合及工艺流程明细集合。
        /// </summary>
        /// <param name="codeValue">染固色工艺代码</param>
        /// <returns>
        /// Tuple：
        ///   Item1：工艺主表DataRow集合（List&lt;DataRow&gt;），
        ///   Item2：工艺流程明细集合（List&lt;object[]&gt;），
        ///         每个object[]包含：序号、工艺名称、温度、速率、比例或时间、备注、索引号、工艺代码、工艺类型
        /// </returns>
        public static (List<DataRow>, List<object[]>) GetDyeingDetails(string codeValue)
        {
            var allRows = new List<object[]>();
            var drs = new List<DataRow>();

            // 若工艺代码为空或主表数据未加载，直接返回空结果
            if (string.IsNullOrEmpty(codeValue) || My_DataBase.DyeingCodeData.Dyeing_code == null) return (drs, allRows);

            // 1. 先查找当前工艺主表
            drs = My_DataBase.DyeingCodeData.Dyeing_code.AsEnumerable()
                .Where(r => r.Field<string>(My_DataBase.DYEING_CODE.DyeingCode) == codeValue.ToString())
                .OrderBy(r => r[My_DataBase.DYEING_CODE.IndexNum])
                .ToList();

            if (drs.Count == 0)
            {
                // 2. 若当前主表未找到，则查找历史工艺主表
                drs = My_DataBase.DyeingCodeData.History_Dyeing_code.AsEnumerable()
                    .Where(r => r.Field<string>(My_DataBase.HISTORY_DYEING_CODE.DyeingCode) == codeValue.ToString())
                    .OrderBy(r => r[My_DataBase.HISTORY_DYEING_CODE.IndexNum])
                    .ToList();

                // 历史主表也未找到，直接返回空
                if (drs.Count == 0) return (drs, allRows);

                int i = 0;
                // 遍历历史主表行，查找对应的历史工艺流程明细
                foreach (DataRow row in drs)
                {
                    var index = row[My_DataBase.HISTORY_DYEING_CODE.IndexNum];
                    var cd = row[My_DataBase.HISTORY_DYEING_CODE.Code];
                    var type = row[My_DataBase.HISTORY_DYEING_CODE.Type];

                    // 若工艺代码为空或流程表未加载，跳过
                    if (cd == null || cd == DBNull.Value || My_DataBase.DyeingData.History_Dyeing_process == null) continue;

                    // 查找历史工艺流程明细
                    var drs1 = My_DataBase.DyeingData.History_Dyeing_process.AsEnumerable()
                        .Where(r => r.Field<string>(My_DataBase.HISTORY_DYEING_PROCESS.Code) == cd.ToString())
                        .OrderBy(r => r[My_DataBase.HISTORY_DYEING_PROCESS.StepNum]);

                    // 组装明细数据
                    foreach (var dataRow in drs1)
                    {
                        allRows.Add(new object[] {
                            ++i, // 序号
                            dataRow[My_DataBase.HISTORY_DYEING_PROCESS.TechnologyName] ?? "", // 工艺名称
                            dataRow[My_DataBase.HISTORY_DYEING_PROCESS.Temp] ?? "",           // 温度
                            dataRow[My_DataBase.HISTORY_DYEING_PROCESS.Rate] ?? "",           // 速率
                            dataRow[My_DataBase.HISTORY_DYEING_PROCESS.ProportionOrTime] ?? "",// 比例或时间
                            dataRow[My_DataBase.HISTORY_DYEING_PROCESS.Rev] ?? "",            // 备注
                            index, // 索引号
                            cd,    // 工艺代码
                            type   // 工艺类型
                        });
                    }
                }
            }
            else
            {
                int i = 0;
                // 遍历当前主表行，查找对应的工艺流程明细
                foreach (DataRow row in drs)
                {
                    var index = row[My_DataBase.DYEING_CODE.IndexNum];
                    var cd = row[My_DataBase.DYEING_CODE.Code];
                    var type = row[My_DataBase.DYEING_CODE.Type];

                    // 若工艺代码为空或流程表未加载，跳过
                    if (cd == null || cd == DBNull.Value || My_DataBase.DyeingData.Dyeing_process == null) continue;

                    // 查找工艺流程明细
                    var drs1 = My_DataBase.DyeingData.Dyeing_process.AsEnumerable()
                        .Where(r => r.Field<string>(My_DataBase.DYEING_PROCESS.Code) == cd.ToString())
                        .OrderBy(r => r[My_DataBase.DYEING_PROCESS.StepNum]);

                    // 组装明细数据
                    foreach (var dataRow in drs1)
                    {
                        allRows.Add(new object[] {
                            ++i, // 序号
                            dataRow[My_DataBase.DYEING_PROCESS.TechnologyName] ?? "", // 工艺名称
                            dataRow[My_DataBase.DYEING_PROCESS.Temp] ?? "",           // 温度
                            dataRow[My_DataBase.DYEING_PROCESS.Rate] ?? "",           // 速率
                            dataRow[My_DataBase.DYEING_PROCESS.ProportionOrTime] ?? "",// 比例或时间
                            dataRow[My_DataBase.DYEING_PROCESS.Rev] ?? "",            // 备注
                            index, // 索引号
                            cd,    // 工艺代码
                            type   // 工艺类型
                        });
                    }
                }
            }

            // 返回主表行和明细集合
            return (drs, allRows);
        }


        /// <summary>
        /// 根据染色工艺代码、配方代码、版本号，生成完整的染色明细（与CtDropDetail一致）
        /// 支持一个加药工艺中包含多个染助剂的情况，每个助剂单独输出明细行
        /// </summary>
        /// <param name="dyeingCode">染色工艺代码</param>
        /// <param name="formulaCode">配方代码</param>
        /// <param name="versionNum">配方版本号</param>
        /// <returns>染色明细字典列表</returns>
        public static List<Dictionary<string, object>> GetAllDyeDetailFromFormulaCode(string dyeingCode, string formulaCode, int versionNum)
        {
            var result = new List<Dictionary<string, object>>();
            // 获取工艺步骤明细
            var (drs, steps) = GetDyeingDetails(dyeingCode);
            int roundDigits = My_Tool.BalanceStableReading.RetainDecimals();
            double clothWeight = 0;
            double nonAnhydrationWR = 0;
            List<double> handleBRList = null;

            // 查询配方头表，获取布重、浴比、含水率等信息
            var head = SmartColor.My_DataBase.SqlServer.Select(
                SmartColor.My_DataBase.FORMULA_HEAD.TableName,
                $"{SmartColor.My_DataBase.FORMULA_HEAD.FormulaCode}=@FormulaCode AND {SmartColor.My_DataBase.FORMULA_HEAD.VersionNum}=@VersionNum",
                new System.Data.SqlClient.SqlParameter("@FormulaCode", formulaCode),
                new System.Data.SqlClient.SqlParameter("@VersionNum", versionNum)
            );
            if (head != null && head.Rows.Count > 0)
            {
                handleBRList = head.Rows[0][SmartColor.My_DataBase.FORMULA_HEAD.HandleBRList]?.ToString()
                    .Split('|').Where(s => double.TryParse(s, out _)).Select(s => Convert.ToDouble(s)).ToList();
                clothWeight = Convert.ToDouble(head.Rows[0][SmartColor.My_DataBase.FORMULA_HEAD.ClothWeight] ?? "0");
                if (head.Rows[0].Table.Columns.Contains(SmartColor.My_DataBase.FORMULA_HEAD.Non_AnhydrationWR))
                    double.TryParse(head.Rows[0][SmartColor.My_DataBase.FORMULA_HEAD.Non_AnhydrationWR]?.ToString(), out nonAnhydrationWR);
            }
            if (handleBRList == null) return result;

            // 按“排液”分段
            var segments = new List<List<object[]>>();
            var currentSegment = new List<object[]>();
            foreach (var step in steps)
            {
                currentSegment.Add(step);
                string techName = step[1]?.ToString() ?? "";
                if (techName.Contains("排液"))
                {
                    segments.Add(currentSegment);
                    currentSegment = new List<object[]>();
                }
            }
            if (currentSegment.Count > 0)
                segments.Add(currentSegment);

            int stepNum = 0;
            int segmentIdx = 0;
            foreach (var segment in segments)
            {
                if (segment.Count == 0) continue;
                // 取本段第一个步骤的indexNum，查找浴比
                int indexNum = Convert.ToInt32(segment[0][6]);
                double bathRatio = 0;
                if (handleBRList == null || indexNum <= 0)
                {
                    bathRatio = 0;
                }
                else if (indexNum > handleBRList.Count)
                {
                    bathRatio = handleBRList[0]; // 默认取第一个浴比
                }
                else
                {
                    bathRatio = handleBRList[indexNum - 1];
                }

                double totalBath = clothWeight * bathRatio;

                // 统计本段所有加药步骤的加药量（支持多助剂）
                var addChemSteps = segment.Where(d => d[1]?.ToString().Contains("加") == true && d[1]?.ToString() != "加水").ToList();
                double totalChem = 0;
                // 记录每个加药步骤下所有助剂的加药量列表
                var chemWeightDict = new Dictionary<object[], List<double>>();
                // 记录每个加药步骤下所有助剂的DataRow明细
                var chemDetailDict = new Dictionary<object[], List<DataRow>>();
                foreach (var d in addChemSteps)
                {
                    string tn = d[1]?.ToString() ?? "";
                    string code = d[7]?.ToString() ?? "";
                    double percent = Convert.ToDouble(d[4] ?? "0");
                    // 查询该加药步骤下所有助剂明细
                    var dt = SmartColor.My_DataBase.SqlServer.Select(
                        SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.TableName,
                        $"{SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.FormulaCode}=@FormulaCode AND " +
                        $"{SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.VersionNum}=@VersionNum AND " +
                        $"{SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.Code}=@Code AND " +
                        $"{SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.No} = @No AND " +
                        $"{SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.TechnologyName} = @TechName",
                        new System.Data.SqlClient.SqlParameter("@FormulaCode", formulaCode),
                        new System.Data.SqlClient.SqlParameter("@VersionNum", versionNum),
                        new System.Data.SqlClient.SqlParameter("@Code", code),
                        new System.Data.SqlClient.SqlParameter("@No", indexNum),
                        new System.Data.SqlClient.SqlParameter("@TechName", tn)
                    );
                    var chemWeights = new List<double>();
                    var chemDetails = new List<DataRow>();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            // 计算每个助剂的加药量
                            double baseDropWeight = Convert.ToDouble(dr[SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.ObjectDropWeight] ?? "0");
                            double chemWeight = Math.Round(baseDropWeight * percent / 100, roundDigits);
                            chemWeights.Add(chemWeight);
                            chemDetails.Add(dr);
                            totalChem += chemWeight;
                        }
                    }
                    chemWeightDict[d] = chemWeights;
                    chemDetailDict[d] = chemDetails;
                }

                // 统计本段所有“加水”步骤的加水量
                var addWaterSteps = segment.Where(d => d[1]?.ToString() == "加水").ToList();
                double totalWater = 0;
                var waterWeightDict = new Dictionary<object[], double>();
                foreach (var d in addWaterSteps)
                {
                    // “加水”步骤的加水量 = 百分比字段 * 总浴量 / 100
                    double percent = Convert.ToDouble(d[4] ?? "0");
                    double water = Math.Round(totalBath * percent / 100, roundDigits);
                    waterWeightDict[d] = water;
                    totalWater += water;
                }

                // 计算加水量，需扣除加药量和布重含水量
                double waterWeight = Math.Round(totalBath - totalChem - totalWater - (clothWeight * nonAnhydrationWR), roundDigits);

                // 标记是否已补加水
                bool waterAdded = false;

                // 逐步输出每个步骤
                foreach (var d in segment)
                {
                    string techName = d[1]?.ToString() ?? "";
                    string code = d[7]?.ToString() ?? "";
                    string dyeType = d[8]?.ToString() ?? "1";

                    if (techName.Contains("加") && techName != "加水")
                    {
                        // 多助剂情况，逐个输出
                        if (chemDetailDict.ContainsKey(d))
                        {
                            var chemDetails = chemDetailDict[d];
                            var chemWeights = chemWeightDict[d];
                            for (int i = 0; i < chemDetails.Count; i++)
                            {
                                var dr = chemDetails[i];
                                var dict = new Dictionary<string, object>
                                {
                                    [SmartColor.My_DataBase.DYE_DETAILS.StepNum] = ++stepNum,
                                    [SmartColor.My_DataBase.DYE_DETAILS.TechnologyName] = techName,
                                    [SmartColor.My_DataBase.DYE_DETAILS.Temp] = d[2]?.ToString() ?? "",
                                    [SmartColor.My_DataBase.DYE_DETAILS.TempSpeed] = d[3]?.ToString() ?? "",
                                    [SmartColor.My_DataBase.DYE_DETAILS.Time] = d[4]?.ToString() ?? "",
                                    [SmartColor.My_DataBase.DYE_DETAILS.RotorSpeed] = d[5]?.ToString() ?? "",
                                    [SmartColor.My_DataBase.DYE_DETAILS.IndexNum] = d[6]?.ToString() ?? "",
                                    [SmartColor.My_DataBase.DYE_DETAILS.Code] = code,
                                    [SmartColor.My_DataBase.DYE_DETAILS.DyeType] = dyeType,
                                    [SmartColor.My_DataBase.DYE_DETAILS.FormulaCode] = formulaCode,
                                    [SmartColor.My_DataBase.DYE_DETAILS.VersionNum] = versionNum,
                                    [SmartColor.My_DataBase.DYE_DETAILS.BottleSelection] = dr[SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.BottleSelection],
                                    [SmartColor.My_DataBase.DYE_DETAILS.Finish] = 0,
                                    [SmartColor.My_DataBase.DYE_DETAILS.AssistantCode] = dr[SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.AssistantCode],
                                    [SmartColor.My_DataBase.DYE_DETAILS.FormulaDosage] = dr[SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.FormulaDosage],
                                    [SmartColor.My_DataBase.DYE_DETAILS.UnitOfAccount] = dr[SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.UnitOfAccount],
                                    [SmartColor.My_DataBase.DYE_DETAILS.BottleNum] = dr[SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.BottleNum],
                                    [SmartColor.My_DataBase.DYE_DETAILS.SettingConcentration] = dr[SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.SettingConcentration],
                                    [SmartColor.My_DataBase.DYE_DETAILS.RealConcentration] = dr[SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.RealConcentration],
                                    [SmartColor.My_DataBase.DYE_DETAILS.AssistantName] = dr[SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.AssistantName],
                                    [SmartColor.My_DataBase.DYE_DETAILS.RealDropWeight] = dr[SmartColor.My_DataBase.FORMULA_HANDLE_DETAILS.RealDropWeight],
                                    [SmartColor.My_DataBase.DYE_DETAILS.ObjectDropWeight] = chemWeights[i]
                                };

                                // 如果在第一个排液分段内，ObjectWaterWeight=0
                                if (segmentIdx == 0)
                                {
                                    dict[SmartColor.My_DataBase.DYE_DETAILS.ObjectWaterWeight] = 0;
                                }
                                else
                                {
                                    // 如果没有加水步骤，把加水量补到第一个加药的第一个助剂
                                    if (!addWaterSteps.Any() && !waterAdded && i == 0)
                                    {
                                        dict[SmartColor.My_DataBase.DYE_DETAILS.ObjectWaterWeight] = waterWeight;
                                        waterAdded = true;
                                    }
                                    else
                                    {
                                        dict[SmartColor.My_DataBase.DYE_DETAILS.ObjectWaterWeight] = 0;
                                    }
                                }
                                result.Add(dict);
                            }
                        }
                        else
                        {
                            // 没有明细时的兜底逻辑
                            var dict = new Dictionary<string, object>
                            {
                                [SmartColor.My_DataBase.DYE_DETAILS.StepNum] = ++stepNum,
                                [SmartColor.My_DataBase.DYE_DETAILS.TechnologyName] = techName,
                                [SmartColor.My_DataBase.DYE_DETAILS.Temp] = d[2]?.ToString() ?? "",
                                [SmartColor.My_DataBase.DYE_DETAILS.TempSpeed] = d[3]?.ToString() ?? "",
                                [SmartColor.My_DataBase.DYE_DETAILS.Time] = d[4]?.ToString() ?? "",
                                [SmartColor.My_DataBase.DYE_DETAILS.RotorSpeed] = d[5]?.ToString() ?? "",
                                [SmartColor.My_DataBase.DYE_DETAILS.IndexNum] = d[6]?.ToString() ?? "",
                                [SmartColor.My_DataBase.DYE_DETAILS.Code] = code,
                                [SmartColor.My_DataBase.DYE_DETAILS.DyeType] = dyeType,
                                [SmartColor.My_DataBase.DYE_DETAILS.FormulaCode] = formulaCode,
                                [SmartColor.My_DataBase.DYE_DETAILS.VersionNum] = versionNum,
                                [SmartColor.My_DataBase.DYE_DETAILS.BottleSelection] = 0,
                                [SmartColor.My_DataBase.DYE_DETAILS.Finish] = 0,
                                [SmartColor.My_DataBase.DYE_DETAILS.AssistantCode] = null,
                                [SmartColor.My_DataBase.DYE_DETAILS.FormulaDosage] = null,
                                [SmartColor.My_DataBase.DYE_DETAILS.UnitOfAccount] = null,
                                [SmartColor.My_DataBase.DYE_DETAILS.BottleNum] = null,
                                [SmartColor.My_DataBase.DYE_DETAILS.SettingConcentration] = null,
                                [SmartColor.My_DataBase.DYE_DETAILS.RealConcentration] = null,
                                [SmartColor.My_DataBase.DYE_DETAILS.AssistantName] = null,
                                [SmartColor.My_DataBase.DYE_DETAILS.RealDropWeight] = null,
                                [SmartColor.My_DataBase.DYE_DETAILS.ObjectDropWeight] = 0,
                                [SmartColor.My_DataBase.DYE_DETAILS.ObjectWaterWeight] = 0
                            };
                            result.Add(dict);
                        }
                    }
                    else if (techName == "加水")
                    {
                        // “加水”步骤的加水量
                        double thisWater = waterWeightDict.ContainsKey(d) ? waterWeightDict[d] : 0;
                        var dict = new Dictionary<string, object>
                        {
                            [SmartColor.My_DataBase.DYE_DETAILS.StepNum] = ++stepNum,
                            [SmartColor.My_DataBase.DYE_DETAILS.TechnologyName] = techName,
                            [SmartColor.My_DataBase.DYE_DETAILS.Temp] = d[2]?.ToString() ?? "",
                            [SmartColor.My_DataBase.DYE_DETAILS.TempSpeed] = d[3]?.ToString() ?? "",
                            [SmartColor.My_DataBase.DYE_DETAILS.Time] = d[4]?.ToString() ?? "",
                            [SmartColor.My_DataBase.DYE_DETAILS.RotorSpeed] = d[5]?.ToString() ?? "",
                            [SmartColor.My_DataBase.DYE_DETAILS.IndexNum] = d[6]?.ToString() ?? "",
                            [SmartColor.My_DataBase.DYE_DETAILS.Code] = code,
                            [SmartColor.My_DataBase.DYE_DETAILS.DyeType] = dyeType,
                            [SmartColor.My_DataBase.DYE_DETAILS.FormulaCode] = formulaCode,
                            [SmartColor.My_DataBase.DYE_DETAILS.VersionNum] = versionNum,
                            [SmartColor.My_DataBase.DYE_DETAILS.BottleSelection] = 0,
                            [SmartColor.My_DataBase.DYE_DETAILS.Finish] = 0,
                            [SmartColor.My_DataBase.DYE_DETAILS.ObjectWaterWeight] = thisWater,
                            [SmartColor.My_DataBase.DYE_DETAILS.ObjectDropWeight] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.AssistantCode] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.FormulaDosage] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.UnitOfAccount] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.BottleNum] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.SettingConcentration] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.RealConcentration] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.AssistantName] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.RealDropWeight] = null
                        };
                        result.Add(dict);
                    }
                    else
                    {
                        // 其它步骤，包括排液
                        var dict = new Dictionary<string, object>
                        {
                            [SmartColor.My_DataBase.DYE_DETAILS.StepNum] = ++stepNum,
                            [SmartColor.My_DataBase.DYE_DETAILS.TechnologyName] = techName,
                            [SmartColor.My_DataBase.DYE_DETAILS.Temp] = d[2]?.ToString() ?? "",
                            [SmartColor.My_DataBase.DYE_DETAILS.TempSpeed] = d[3]?.ToString() ?? "",
                            [SmartColor.My_DataBase.DYE_DETAILS.Time] = d[4]?.ToString() ?? "",
                            [SmartColor.My_DataBase.DYE_DETAILS.RotorSpeed] = d[5]?.ToString() ?? "",
                            [SmartColor.My_DataBase.DYE_DETAILS.IndexNum] = d[6]?.ToString() ?? "",
                            [SmartColor.My_DataBase.DYE_DETAILS.Code] = code,
                            [SmartColor.My_DataBase.DYE_DETAILS.DyeType] = dyeType,
                            [SmartColor.My_DataBase.DYE_DETAILS.FormulaCode] = formulaCode,
                            [SmartColor.My_DataBase.DYE_DETAILS.VersionNum] = versionNum,
                            [SmartColor.My_DataBase.DYE_DETAILS.BottleSelection] = 0,
                            [SmartColor.My_DataBase.DYE_DETAILS.Finish] = 0,
                            [SmartColor.My_DataBase.DYE_DETAILS.ObjectWaterWeight] = 0,
                            [SmartColor.My_DataBase.DYE_DETAILS.ObjectDropWeight] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.AssistantCode] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.FormulaDosage] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.UnitOfAccount] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.BottleNum] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.SettingConcentration] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.RealConcentration] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.AssistantName] = null,
                            [SmartColor.My_DataBase.DYE_DETAILS.RealDropWeight] = null
                        };
                        result.Add(dict);
                    }
                }
                segmentIdx++;
            }
            return result;
        }

    }
}