using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SmartColor.My_Tool
{
    public static class BatchNoHelper
    {
        /// <summary>
        /// 生成批次号（yyyyMMddNNNN），并更新数据库
        /// </summary>
        public static string GenerateBatchNoAndUpdate()
        {
            var dt = SmartColor.My_DataBase.EnabledData.Enabled_set;
            if (dt == null || dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];
            var oldBatchNo = row[SmartColor.My_DataBase.ENABLED_SET.BatchName]?.ToString();

            string today = DateTime.Now.ToString("yyyyMMdd");
            int newSeq = 1;

            if (!string.IsNullOrEmpty(oldBatchNo) && oldBatchNo.Length == 12)
            {
                string oldDate = oldBatchNo.Substring(0, 8);
                string oldSeqStr = oldBatchNo.Substring(8, 4);

                if (oldDate == today && int.TryParse(oldSeqStr, out int oldSeq))
                {
                    newSeq = oldSeq + 1;
                }
            }

            string newBatchNo = $"{today}{newSeq.ToString("D4")}";

            SmartColor.My_DataBase.SqlServer.Update(
                SmartColor.My_DataBase.ENABLED_SET.TableName,
                new Dictionary<string, object> { [SmartColor.My_DataBase.ENABLED_SET.BatchName] = newBatchNo },
                $"{SmartColor.My_DataBase.ENABLED_SET.MyID} = @MyID",
                new SqlParameter("@MyID", row[SmartColor.My_DataBase.ENABLED_SET.MyID])
            );

            return newBatchNo;
        }
    }
}