using System;
using System.Data;

namespace SmartColor.My_Tool
{
    /// <summary>
    /// 母液瓶信息结构体
    /// </summary>
    public struct BottlePulseInfo
    {
        /// <summary>结果码</summary>
        public int Result;
        /// <summary>针筒类型</summary>
        public short SyringeType;
        /// <summary>校正值</summary>
        public double Adjust;
        /// <summary>校正重量</summary>
        public double AdjustWeight;
        /// <summary>当前重量</summary>
        public double CurrentWeight;
        /// <summary>单位</summary>
        public string UnitOfAccount;
    }

    internal class BottleAuxiliary
    {
        /// <summary>
        /// 查找母液瓶资料
        /// </summary>
        /// <param name="bottleNo">瓶号</param>
        /// <returns>BottlePulseInfo结构体</returns>
        public static BottlePulseInfo GetBottleInfo(int bottleNo)
        {
            var info = new BottlePulseInfo
            {
                Result = -1,
                SyringeType = 0,
                Adjust = 0,
                AdjustWeight = 0,
                CurrentWeight = 0,
                UnitOfAccount = null
            };

            try
            {
                var bottleTable = SmartColor.My_DataBase.BottleData.Bottle_details;
                if (bottleTable == null)
                    return info;

                var rows = bottleTable.Select($"{SmartColor.My_DataBase.BOTTLE_DETAILS.BottleNum} = {bottleNo}");
                if (rows.Length == 0)
                    return info; // 未找到

                info.Adjust = rows[0][SmartColor.My_DataBase.BOTTLE_DETAILS.AdjustValue] != DBNull.Value ?
                    Convert.ToDouble(rows[0][SmartColor.My_DataBase.BOTTLE_DETAILS.AdjustValue]) : 0;
                info.AdjustWeight = rows[0][SmartColor.My_DataBase.BOTTLE_DETAILS.CurrentAdjustWeight] != DBNull.Value ?
                    Convert.ToDouble(rows[0][SmartColor.My_DataBase.BOTTLE_DETAILS.CurrentAdjustWeight]) : 0;
                string s = rows[0][SmartColor.My_DataBase.BOTTLE_DETAILS.SyringeType]?.ToString();
                if (s == null)
                {
                    info.Result = -1;
                    return info;
                }

                switch (s)
                {
                    case "小针筒":
                        info.SyringeType = 0;
                        break;
                    case "大针筒":
                        info.SyringeType = 1;
                        break;
                    default:
                        info.Result = -2;
                        return info;
                }

                double currentWeight = 0;
                double.TryParse(rows[0][SmartColor.My_DataBase.BOTTLE_DETAILS.CurrentWeight]?.ToString(), out currentWeight);
                info.CurrentWeight = currentWeight;

                // 查找染助剂代码和单位
                string assistantCode = rows[0][SmartColor.My_DataBase.BOTTLE_DETAILS.AssistantCode]?.ToString();
                string unitOfAccount = null;
                var assistantTable = SmartColor.My_DataBase.AssistantData.Assistant_details;
                if (!string.IsNullOrEmpty(assistantCode) && assistantTable != null)
                {
                    var assistantRows = assistantTable.Select($"{SmartColor.My_DataBase.ASSISTANT_DETAILS.AssistantCode} = '{assistantCode}'");
                    if (assistantRows.Length > 0)
                    {
                        unitOfAccount = assistantRows[0][SmartColor.My_DataBase.ASSISTANT_DETAILS.UnitOfAccount]?.ToString();
                        info.UnitOfAccount = unitOfAccount;
                    }
                    else
                    {
                        info.Result = -6; // 助剂资料未找到
                        return info;
                    }
                }

                info.Result = 0;
                return info;
            }
            catch
            {
                info.Result = -5;
                return info;
            }
        }

        /// <summary>
        /// 获取当前瓶的允许误差
        /// </summary>
        /// <param name="bottleNo">瓶号</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static double GetAllowErr(int bottleNo)
        {
            var info = SmartColor.My_Tool.BottleAuxiliary.GetBottleInfo(bottleNo);
            if (info.Result != 0)
                throw new Exception("获取允许误差值异常");
            var unit = info.UnitOfAccount;
            if(unit == "g/l" || unit == "G/L")
            {
                return My_ConPar.Other.AssAErr;
            }
            else
            {
                return My_ConPar.Other.DyeAErr;
            }
        }
    }
}