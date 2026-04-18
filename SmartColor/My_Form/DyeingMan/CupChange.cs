using SmartColor.My_DataBase;
using SmartColor.My_File; // 引入LocalTranslator
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Form.DyeingMan
{
    public partial class CupChange : Form
    {
        /// <summary>
        /// 杯号修改类型枚举
        /// </summary>
        public enum CupChangeType
        {
            DropBatch = 0,      // 滴液批次表
            DropWaitList = 1,   // 滴液等待列表
            AbsBatch = 2,       // ABS滴液批次表
            AbsWaitList = 3     // ABS滴液等待列表
        }

        /// <summary>
        /// 杯号修改信息结构体
        /// </summary>
        public struct CupChangeInfo
        {
            /// <summary>
            /// 原杯号
            /// </summary>
            public string OldCupNo;

            /// <summary>
            /// 数据库主键ID
            /// </summary>
            public int MyID;

            /// <summary>
            /// 杯号修改类型
            /// </summary>
            public CupChangeType Type;   
        }

        /// <summary>
        /// 新杯号（只读属性）
        /// </summary>
        public int NewCupNum { get; private set; } = 0;

        // 当前杯号修改信息
        private CupChangeInfo _cupInfo = new CupChangeInfo();

        /// <summary>
        /// 构造函数，初始化窗体与杯号信息
        /// </summary>
        /// <param name="cupInfo">杯号修改信息</param>
        public CupChange(CupChangeInfo cupInfo)
        {
            InitializeComponent();
            this._cupInfo = cupInfo;
            this.textBox1.Text = cupInfo.OldCupNo;
        }

        /// <summary>
        /// 保存按钮点击事件，校验并更新杯号
        /// </summary>
        private void btn_Save_Click(object sender, EventArgs e)
        {
            string newCupNo = textBox1.Text.Trim();

            // 校验输入是否为空
            if (string.IsNullOrEmpty(newCupNo))
            {
                LocalTranslator.ShowMessage("请输入新的杯号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 校验输入是否为正整数
            if (!int.TryParse(newCupNo, out int cupNum) || cupNum <= 0)
            {
                LocalTranslator.ShowMessage("杯号必须为正整数！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.SelectAll();
                textBox1.Focus();
                return;
            }

            // 校验杯号是否可用
            if (!IsCupValid(cupNum, out string reason))
            {
                LocalTranslator.ShowMessage($"杯号不可用：{reason}\n请重新输入！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.SelectAll();
                textBox1.Focus();
                return;
            }

            // 根据类型批量更新相关表的杯号字段
            switch (_cupInfo.Type)
            {
                case CupChangeType.DropBatch:
                    UpdateCupNum(cupNum, new[]
                    {
                        (My_DataBase.DROP_HEAD.TableName, My_DataBase.DROP_HEAD.CupNum, My_DataBase.DROP_HEAD.MyID),
                        (My_DataBase.DROP_DETAILS.TableName, My_DataBase.DROP_DETAILS.CupNum, My_DataBase.DROP_DETAILS.HeadID),
                        (My_DataBase.DYE_DETAILS.TableName, My_DataBase.DYE_DETAILS.CupNum, My_DataBase.DYE_DETAILS.HeadID)
                    });
                    break;
                case CupChangeType.DropWaitList:
                    UpdateCupNum(cupNum, new[]
                    {
                        (My_DataBase.WAIT_LIST.TableName, My_DataBase.WAIT_LIST.CupNum, My_DataBase.WAIT_LIST.MyID)
                    });
                    break;
                case CupChangeType.AbsBatch:
                    UpdateCupNum(cupNum, new[]
                    {
                        (My_DataBase.ABS_DROP_HEAD.TableName, My_DataBase.ABS_DROP_HEAD.CupNum, My_DataBase.ABS_DROP_HEAD.MyID),
                        (My_DataBase.ABS_DROP_DETAILS.TableName, My_DataBase.ABS_DROP_DETAILS.CupNum, My_DataBase.ABS_DROP_DETAILS.HeadID),
                        (My_DataBase.ABS_DETAILS.TableName, My_DataBase.ABS_DETAILS.CupNum, My_DataBase.ABS_DETAILS.HeadID)
                    });
                    break;
                case CupChangeType.AbsWaitList:
                    UpdateCupNum(cupNum, new[]
                    {
                        (My_DataBase.ABS_WAIT_LIST.TableName, My_DataBase.ABS_WAIT_LIST.CupNum, My_DataBase.ABS_WAIT_LIST.MyID)
                    });
                    break;
            }

            // 更新新杯号属性，关闭窗体并返回OK
            NewCupNum = cupNum;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// 批量更新指定表的杯号字段
        /// </summary>
        /// <param name="newCupNum">新杯号</param>
        /// <param name="targets">需要更新的表、字段、主键元组数组</param>
        private void UpdateCupNum(int newCupNum, (string table, string cupField, string idField)[] targets)
        {
            foreach (var t in targets)
            {
                // 调用SqlServer.Update方法，更新指定表的杯号字段
                SqlServer.Update(
                    t.table,
                    new Dictionary<string, object> { [t.cupField] = newCupNum },
                    $"{t.idField} = @MyID",
                    new SqlParameter("@MyID", _cupInfo.MyID)
                );
            }
        }

        /// <summary>
        /// 校验杯号是否可用，并返回不可用原因
        /// </summary>
        /// <param name="cupNum">待校验杯号</param>
        /// <param name="reason">不可用原因</param>
        /// <returns>可用返回true，否则false</returns>
        private bool IsCupValid(int cupNum, out string reason)
        {
            reason = "";
          

            // 查询杯号是否存在
            var dt = SqlServer.Select(My_DataBase.CUP_DETAILS.TableName, $"{My_DataBase.CUP_DETAILS.CupNum} = {cupNum}");
            if (dt == null || dt.Rows.Count == 0)
            {
                reason = "杯号不存在";
                return false;
            }

            var row = dt.Rows[0];
            int enable = Convert.ToInt32(row[My_DataBase.CUP_DETAILS.Enable]);
            int isUsing = Convert.ToInt32(row[My_DataBase.CUP_DETAILS.IsUsing]);

            // 校验杯号是否启用
            if (enable != 1)
            {
                reason = "杯号未启用";
                return false;
            }

            // 校验杯号是否正在使用
            if (isUsing == 1)
            {
                reason = "杯号正在使用";
                return false;
            }

            // 校验主副杯号是否冲突
            if (row.Table.Columns.Contains(My_DataBase.CUP_DETAILS.MainCupNum) &&
                int.TryParse(row[My_DataBase.CUP_DETAILS.MainCupNum]?.ToString(), out int mainCupNum) &&
                mainCupNum > 0 && mainCupNum != cupNum)
            {
                var pairRows = dt.Select($"{My_DataBase.CUP_DETAILS.CupNum} = {mainCupNum}");
                if (pairRows.Length > 0)
                {
                    var pairRow = pairRows[0];
                    int pairEnable = Convert.ToInt32(pairRow[My_DataBase.CUP_DETAILS.Enable]);
                    int pairIsUsing = Convert.ToInt32(pairRow[My_DataBase.CUP_DETAILS.IsUsing]);
                    if (pairEnable == 1 && pairIsUsing == 1)
                    {
                        reason = $"主副杯号({mainCupNum})正在使用";
                        return false;
                    }
                }
            }

            // 检查批次表是否已用（等待列表不检查）
            if (_cupInfo.Type == CupChangeType.DropBatch)
            {
                var batchDt = My_DataBase.DropBatchData.GetHeadData();
                if (batchDt != null && batchDt.AsEnumerable().Any(r =>
                    r[My_DataBase.DROP_HEAD.CupNum]?.ToString() == cupNum.ToString() &&
                    Convert.ToInt32(r[My_DataBase.DROP_HEAD.MyID]) != _cupInfo.MyID))
                {
                    reason = "该杯号已被滴液批次表使用";
                    return false;
                }
            }
            else if (_cupInfo.Type == CupChangeType.AbsBatch)
            {
                var batchDt = My_DataBase.ABSBatchData.GetHeadData();
                if (batchDt != null && batchDt.AsEnumerable().Any(r =>
                    r[My_DataBase.ABS_DROP_HEAD.CupNum]?.ToString() == cupNum.ToString() &&
                    Convert.ToInt32(r[My_DataBase.ABS_DROP_HEAD.MyID]) != _cupInfo.MyID))
                {
                    reason = "该杯号已被ABS滴液批次表使用";
                    return false;
                }
            }
            // 等待列表类型不检查是否已用

            return true;
        }
    }
}