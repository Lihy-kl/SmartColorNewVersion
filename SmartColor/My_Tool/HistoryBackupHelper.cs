using SmartColor.My_File;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Tool
{
    public static  class HistoryBackupHelper
    {
        /// <summary>
        /// 通用历史数据备份与联动更新
        /// 优化点：
        /// 1. 减少重复代码，合并弹窗逻辑。
        /// 2. SQL语句拼接安全性提升，避免SQL注入风险（已对单引号做替换）。
        /// 3. 代码结构更清晰，便于维护。
        /// 4. 添加详细注释，便于理解每一步操作。
        /// </summary>
        /// <param name="code">主表工艺代码</param>
        /// <param name="datePrefix">备份前缀</param>
        /// <param name="cOrD">"修改"或"删除"</param>
        /// <param name="showType">界面显示类型（如"染固色工艺"、"染色工艺"）</param>
        /// <param name="mainTable">主表名</param>
        /// <param name="historyTable">历史表名</param>
        /// <param name="getDyeingCodeList">获取所有相关DyeingCode的方法</param>
        /// <param name="getAllCodeList">获取所有相关Code的方法</param>
        /// <returns>是否继续操作</returns>
        public static  bool BackupAndUpdateHistory(
            string code,
            string datePrefix,
            string cOrD,
            string showType,
            string mainTable,
            string historyTable,
            Func<string, List<string>> getDyeingCodeList,
            Func<List<string>, List<string>> getAllCodeList)
        {
            // 获取所有相关DyeingCode
            var dyeingCodeList = getDyeingCodeList(code);
            if (dyeingCodeList.Count == 0)
            {
                // 若为删除操作，弹窗确认
                if (cOrD == "删除" && !ShowConfirmDialog("确定删除吗？"))
                    return false;
                return true;
            }
            // 拼接DyeingCode字符串，防止SQL注入
            string joinedCodes = string.Join("', '", dyeingCodeList.Select(s => s.Replace("'", "''")));

            // 获取所有相关Code
            var allCodeList = getAllCodeList(dyeingCodeList);
            if (allCodeList.Count == 0)
            {
                // 若为删除操作，弹窗确认
                if (cOrD == "删除" && !ShowConfirmDialog("确定删除吗？"))
                    return false;

                return true;
            }
            string joinedAllCodes = string.Join("', '", allCodeList.Select(s => s.Replace("'", "''")));

            // 判断是否需要联动更新（历史表/配方表/临时表中是否有相关数据）
            bool needUpdate = HasRelatedHistory(joinedCodes);

            if (needUpdate)
            {
                // 联动更新弹窗提示
                string message = $"{cOrD}已使用过的{showType}代码，会将历史配方和染色记录中的相关资料修改\n" +
                                 "染固色代码-->日期+染固色代码\n" +
                                 "染色工艺代码-->日期+染色工艺代码\n" +
                                 "后处理工艺代码-->日期+后处理工艺代码\n" +
                                 $"是否继续{cOrD}？";
                if (!ShowConfirmDialog(message))
                    return false;

                // 1. 备份主表到历史表
                ExecuteBackupMainTable(mainTable, historyTable, datePrefix, joinedCodes);

                // 2. 备份工艺流程到历史工艺表
                ExecuteBackupProcessTable(datePrefix, joinedAllCodes);

                // 3. 联动更新历史染色记录、配方等
                ExecuteUpdateHistory(datePrefix, joinedCodes, joinedAllCodes);

               
          


            }
            else
            {
                // 若为删除操作，弹窗确认
                if (cOrD == "删除" && !ShowConfirmDialog("确定删除吗？"))
                    return false;
            }
            //删除主表
            DeleteMainTableRecords(mainTable, joinedCodes);
            return true;
        }

        /// <summary>
        /// 删除主表中相关DyeingCode的记录
        /// </summary>
        private static void DeleteMainTableRecords(string mainTable, string joinedCodes)
        {
            string sqlDelete = $@"
                DELETE FROM {mainTable}
                WHERE {My_DataBase.DYEING_CODE.DyeingCode} IN ('{joinedCodes}')";
            My_DataBase.SqlServer.ExecuteNonQuery(sqlDelete);
        }

        /// <summary>
        /// 判断是否有相关历史数据需要联动更新
        /// </summary>
        private static  bool HasRelatedHistory(string joinedCodes)
        {
            // 检查历史表、配方表、临时表是否有相关DyeingCode
            return My_DataBase.SqlServer.Select(My_DataBase.HISTORY_HEAD.TableName, $"{My_DataBase.HISTORY_HEAD.DyeingCode} IN ('{joinedCodes}')").Rows.Count > 0
                || My_DataBase.SqlServer.Select(My_DataBase.FORMULA_HEAD.TableName, $"{My_DataBase.FORMULA_HEAD.DyeingCode} IN ('{joinedCodes}')").Rows.Count > 0
                || My_DataBase.SqlServer.Select(My_DataBase.FORMULA_HEAD_TEMP.TableName, $"{My_DataBase.FORMULA_HEAD_TEMP.DyeingCode} IN ('{joinedCodes}')").Rows.Count > 0;
        }

        /// <summary>
        /// 备份主表数据到历史表
        /// </summary>
        private static  void ExecuteBackupMainTable(string mainTable, string historyTable, string datePrefix, string joinedCodes)
        {
            string sqlInsertHistory = $@"
                INSERT INTO {historyTable}
                ({My_DataBase.HISTORY_DYEING_CODE.DyeingCode},
                 {My_DataBase.HISTORY_DYEING_CODE.Type},
                 {My_DataBase.HISTORY_DYEING_CODE.Step},
                 {My_DataBase.HISTORY_DYEING_CODE.Code}, 
                 {My_DataBase.HISTORY_DYEING_CODE.IndexNum},
                 {My_DataBase.HISTORY_DYEING_CODE.IsUse},
                 {My_DataBase.HISTORY_DYEING_CODE.Remark})
                SELECT
                    '{datePrefix}_' + {My_DataBase.DYEING_CODE.DyeingCode},
                    {My_DataBase.DYEING_CODE.Type},
                    {My_DataBase.DYEING_CODE.Step},
                    '{datePrefix}_' + {My_DataBase.DYEING_CODE.Code},
                    {My_DataBase.DYEING_CODE.IndexNum},
                    0,
                    {My_DataBase.DYEING_CODE.Remark}
                FROM {mainTable}
                WHERE {My_DataBase.DYEING_CODE.DyeingCode} IN ('{joinedCodes}')";
            My_DataBase.SqlServer.ExecuteNonQuery(sqlInsertHistory);
        }

        /// <summary>
        /// 备份工艺流程到历史工艺表
        /// </summary>
        private static  void ExecuteBackupProcessTable(string datePrefix, string joinedAllCodes)
        {
            string sqlInsertHistoryProcess = $@"
                INSERT INTO {My_DataBase.HISTORY_DYEING_PROCESS.TableName}
                ({My_DataBase.HISTORY_DYEING_PROCESS.StepNum},
                 {My_DataBase.HISTORY_DYEING_PROCESS.TechnologyName},
                 {My_DataBase.HISTORY_DYEING_PROCESS.ProportionOrTime},
                 {My_DataBase.HISTORY_DYEING_PROCESS.Temp},
                 {My_DataBase.HISTORY_DYEING_PROCESS.Rate},
                 {My_DataBase.HISTORY_DYEING_PROCESS.Type},
                 {My_DataBase.HISTORY_DYEING_PROCESS.Rev},
                 {My_DataBase.HISTORY_DYEING_PROCESS.Remark},
                 {My_DataBase.HISTORY_DYEING_PROCESS.OpenMedicine},
                 {My_DataBase.HISTORY_DYEING_PROCESS.Code})
                SELECT
                    {My_DataBase.DYEING_PROCESS.StepNum},
                    {My_DataBase.DYEING_PROCESS.TechnologyName},
                    {My_DataBase.DYEING_PROCESS.ProportionOrTime},
                    {My_DataBase.DYEING_PROCESS.Temp},
                    {My_DataBase.DYEING_PROCESS.Rate},
                    {My_DataBase.DYEING_PROCESS.Type},
                    {My_DataBase.DYEING_PROCESS.Rev},
                    {My_DataBase.DYEING_PROCESS.Remark},
                    {My_DataBase.DYEING_PROCESS.OpenMedicine},
                    '{datePrefix}_' + {My_DataBase.DYEING_PROCESS.Code}
                FROM {My_DataBase.DYEING_PROCESS.TableName}
                WHERE {My_DataBase.DYEING_PROCESS.Code} IN ('{joinedAllCodes}')";
            My_DataBase.SqlServer.ExecuteNonQuery(sqlInsertHistoryProcess);
        }

        /// <summary>
        /// 联动更新历史染色记录、配方等相关表
        /// </summary>
        private static  void ExecuteUpdateHistory(string datePrefix, string joinedCodes, string joinedAllCodes)
        {
            // 历史染色记录表
            string sqlUpdateHistoryDye = $@"
                UPDATE d
                SET d.{My_DataBase.HISTORY_DYE.Code} = '{datePrefix}_' + d.{My_DataBase.HISTORY_DYE.Code}
                FROM {My_DataBase.HISTORY_DYE.TableName} d
                INNER JOIN {My_DataBase.HISTORY_HEAD.TableName} h
                    ON d.{My_DataBase.HISTORY_HEAD.BatchName} = h.{My_DataBase.HISTORY_HEAD.BatchName} AND d.{My_DataBase.HISTORY_HEAD.CupNum} = h.{My_DataBase.HISTORY_HEAD.CupNum}
                WHERE h.{My_DataBase.HISTORY_HEAD.DyeingCode} IN ('{joinedCodes}') AND d.{My_DataBase.HISTORY_DYE.Code} IN ('{joinedAllCodes}')";
            My_DataBase.SqlServer.ExecuteNonQuery(sqlUpdateHistoryDye);

            // 历史头表
            string sqlUpdateHistoryHead = $@"
                UPDATE {My_DataBase.HISTORY_HEAD.TableName}
                SET {My_DataBase.HISTORY_HEAD.DyeingCode} = '{datePrefix}_' + {My_DataBase.HISTORY_HEAD.DyeingCode}
                WHERE {My_DataBase.HISTORY_HEAD.DyeingCode} IN ('{joinedCodes}')";
            My_DataBase.SqlServer.ExecuteNonQuery(sqlUpdateHistoryHead);

            // 配方明细表
            string sqlUpdateFormulaDetails = $@"
                UPDATE d
                SET d.{My_DataBase.FORMULA_HANDLE_DETAILS.Code} = '{datePrefix}_' + d.{My_DataBase.FORMULA_HANDLE_DETAILS.Code}
                FROM {My_DataBase.FORMULA_HANDLE_DETAILS.TableName} d
                INNER JOIN {My_DataBase.FORMULA_HEAD.TableName} h
                    ON d.{My_DataBase.FORMULA_HEAD.FormulaCode} = h.{My_DataBase.FORMULA_HEAD.FormulaCode} AND d.{My_DataBase.FORMULA_HEAD.VersionNum} = h.{My_DataBase.FORMULA_HEAD.VersionNum}
                WHERE h.{My_DataBase.FORMULA_HEAD.DyeingCode} IN ('{joinedCodes}') AND d.{My_DataBase.FORMULA_HANDLE_DETAILS.Code} IN ('{joinedAllCodes}')";
            My_DataBase.SqlServer.ExecuteNonQuery(sqlUpdateFormulaDetails);

            // 配方头表
            string sqlUpdateFormulaHead = $@"
                UPDATE {My_DataBase.FORMULA_HEAD.TableName}
                SET {My_DataBase.FORMULA_HEAD.DyeingCode} = '{datePrefix}_' + {My_DataBase.FORMULA_HEAD.DyeingCode}
                WHERE {My_DataBase.FORMULA_HEAD.DyeingCode} IN ('{joinedCodes}')";
            My_DataBase.SqlServer.ExecuteNonQuery(sqlUpdateFormulaHead);

            // 临时配方明细表
            string sqlUpdateFormulaDetailsTemp = $@"
                UPDATE d
                SET d.{My_DataBase.FORMULA_HANDLE_DETAILS_TEMP.Code} = '{datePrefix}_' + d.{My_DataBase.FORMULA_HANDLE_DETAILS_TEMP.Code}
                FROM {My_DataBase.FORMULA_HANDLE_DETAILS_TEMP.TableName} d
                INNER JOIN {My_DataBase.FORMULA_HEAD_TEMP.TableName} h
                    ON d.{My_DataBase.FORMULA_HEAD_TEMP.FormulaCode} = h.{My_DataBase.FORMULA_HEAD_TEMP.FormulaCode} AND d.{My_DataBase.FORMULA_HEAD_TEMP.VersionNum} = h.{My_DataBase.FORMULA_HEAD_TEMP.VersionNum}
                WHERE h.{My_DataBase.FORMULA_HEAD_TEMP.DyeingCode} IN ('{joinedCodes}') AND d.{My_DataBase.FORMULA_HANDLE_DETAILS_TEMP.Code} IN ('{joinedAllCodes}')";
            My_DataBase.SqlServer.ExecuteNonQuery(sqlUpdateFormulaDetailsTemp);

            // 临时配方头表
            string sqlUpdateFormulaHeadTemp = $@"
                UPDATE {My_DataBase.FORMULA_HEAD_TEMP.TableName}
                SET {My_DataBase.FORMULA_HEAD_TEMP.DyeingCode} = '{datePrefix}_' + {My_DataBase.FORMULA_HEAD_TEMP.DyeingCode}
                WHERE {My_DataBase.FORMULA_HEAD_TEMP.DyeingCode} IN ('{joinedCodes}')";
            My_DataBase.SqlServer.ExecuteNonQuery(sqlUpdateFormulaHeadTemp);
        }

        /// <summary>
        /// 统一弹窗确认逻辑
        /// </summary>
        private static  bool ShowConfirmDialog(string message)
        {
            DialogResult dialogResult = LocalTranslator.ShowMessage(message, "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return dialogResult == DialogResult.Yes;
        }
    }
}