using SmartColor.My_Control;
using SmartColor.My_DataBase;
using SmartColor.My_File;
using SmartColor.My_Form.BasicData;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SmartColor.My_Form.HistoricalData
{
    public partial class UV : Form
    {
        /// <summary>
        /// 标样吸光度数组
        /// </summary>
        private double[] _sABS = null;

        /// <summary>
        /// 标样起始波长
        /// </summary>
        private int _sStartWave = 0;

        /// <summary>
        /// 标样结束波长
        /// </summary>
        private int _sEndWave = 0;

        /// <summary>
        /// 标样间隔波长
        /// </summary>
        private int _sIntervalWave = 0;

        /// <summary>
        /// 控制DataGridView显示小数点位数，默认2位
        /// </summary>
        private int _decimalPlaces = 2;

        public UV()
        {
            InitializeComponent();
            // 设置为吸光度历史模式
            this.ctHistoryDyeBrowse1.SetMode(My_Control.CtHistoryDyeBrowse.BrowseMode.HistoryAbs);
            this.ctDropHead1.SetMode(CtDropHead.Mode.ABS);
            this.ctDropDetail1.SetMode(CtDropDetail.Mode.ABS); // 设置为吸光度明细模式
            this.ctDropDetail1.BindDropHead(ctDropHead1);

            // 绑定事件
            this.ctHistoryDyeBrowse1.CurrentRowChanged += CtHistoryDyeBrowse1_CurrentRowChanged;

            // 只读设置
            this.ctDropHead1.ReadOnly();
            this.ctDropDetail1.ReadOnly();

            this.ctHistoryDyeBrowse1.ContextMenuStrip = contextMenuStrip1;
            this.ctHistoryDyeBrowse1.ctRecord1.ContextMenuStrip = contextMenuStrip3;
            this.ctDataGridView1.AutoFitAllColumns();
        }

        /// <summary>
        /// 格式化数值，保留指定小数点位数
        /// </summary>
        private string FormatValue(double value)
        {
            return Math.Round(value, _decimalPlaces).ToString($"F{_decimalPlaces}");
        }

        /// <summary>
        /// 历史染色记录行变更事件，刷新页面数据
        /// </summary>
        private void CtHistoryDyeBrowse1_CurrentRowChanged(object sender, EventArgs e)
        {
            var row = this.ctHistoryDyeBrowse1.CurrentRow;
            if (row != null)
            {
                // 获取当前选中记录的ID
                int id = Convert.ToInt32(row[My_DataBase.ABS_HISTORY_HEAD.MyID]);

                // 查询整行数据（所有字段）
                DataTable dt = SqlServer.Select(
                    My_DataBase.ABS_HISTORY_HEAD.TableName,
                    null, // null表示查询所有字段
                    $"{My_DataBase.ABS_HISTORY_HEAD.MyID}=@id",
                    null,
                    true,
                    new System.Data.SqlClient.SqlParameter("@id", id)
                );

                if (dt != null && dt.Rows.Count > 0)
                {
                    // 填充表头控件
                    this.ctDropHead1.FillControlsFromDataTable(dt, CtDropHead.DataSource.History);

                    // 仅在浏览模式下刷新数据
                    if (this.ctHistoryDyeBrowse1.ctRecord1.ContextMenuStrip == contextMenuStrip3)
                    {
                        // 清空历史数据
                        ClearControlsData();

                        // 获取配方代码和类型
                        var formulaCode = dt.Rows[0][My_DataBase.ABS_HISTORY_HEAD.FormulaCode]?.ToString();
                        var type = dt.Rows[0][My_DataBase.ABS_HISTORY_HEAD.Type]?.ToString();
                        var standObj = dt.Rows[0][My_DataBase.ABS_HISTORY_HEAD.Stand];
                        if (standObj != null && standObj != DBNull.Value && Convert.ToInt16(standObj) == 1)
                        {
                            // 如果当前数据是标样，则只添加当前数据
                            AddABSDate(dt.Rows[0], "标样");
                            return;
                        }
                        else
                        {

                            // 查询标样数据（同配方代码、类型，且Stand=1，所有字段）
                            DataTable stdDt = SqlServer.Select(
                                My_DataBase.ABS_HISTORY_HEAD.TableName,
                                null, // 查询所有字段
                                $"{My_DataBase.ABS_HISTORY_HEAD.FormulaCode}=@code AND {My_DataBase.ABS_HISTORY_HEAD.Type}=@type AND {My_DataBase.ABS_HISTORY_HEAD.Stand}=1",
                                null,
                                true,
                                new System.Data.SqlClient.SqlParameter("@code", formulaCode),
                                new System.Data.SqlClient.SqlParameter("@type", type)
                            );

                            // 如果有标样数据，先添加标样
                            if (stdDt != null && stdDt.Rows.Count > 0)
                            {
                                AddABSDate(stdDt.Rows[0], "标样");
                            }

                            // 再添加当前试样数据
                            AddABSDate(dt.Rows[0], "试样");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 切换至浏览模式，清空数据
        /// </summary>
        private void Tsmi_BrowseMode_Click(object sender, EventArgs e)
        {
            this.ctHistoryDyeBrowse1.ctRecord1.ContextMenuStrip = contextMenuStrip3;
            ClearControlsData();
        }

        /// <summary>
        /// 切换至混合模式，清空数据
        /// </summary>
        private void Tsmi_MixedMode_Click(object sender, EventArgs e)
        {
            this.ctHistoryDyeBrowse1.ctRecord1.ContextMenuStrip = contextMenuStrip2;
            ClearControlsData();
        }

        /// <summary>
        /// 清空所有控件数据
        /// </summary>
        private void ClearControlsData()
        {
            this.ctDataGridView1.Rows.Clear();
            this.ctUvCurveChart1.ClearCurves();
            this.ctLabColorWheelPanel1.ClearPoints();
            // 清空标样参数
            this._sABS = null;
            this._sStartWave = 0;
            this._sEndWave = 0;
            this._sIntervalWave = 0;
        }

        /// <summary>
        /// 添加吸光度数据到曲线图、色轮图和表格
        /// </summary>
        /// <param name="dataRow">数据行</param>
        /// <param name="name">标识名称（标样/试样）</param>
        private void AddABSDate(DataRow dataRow, string name = null)
        {
            // 获取吸光度编码及波长参数
            var absCode = dataRow[My_DataBase.ABS_HISTORY_HEAD.Abs]?.ToString();
            var startWaveObj = dataRow[My_DataBase.ABS_HISTORY_HEAD.StartWave]?.ToString();
            var endWaveObj = dataRow[My_DataBase.ABS_HISTORY_HEAD.EndWave]?.ToString();
            var intWaveObj = dataRow[My_DataBase.ABS_HISTORY_HEAD.IntWave]?.ToString();

            // 校验波长参数有效性
            if (startWaveObj == null || intWaveObj == null || endWaveObj == null ||
                !int.TryParse(startWaveObj, out int start) ||
                !int.TryParse(intWaveObj, out int interval) ||
                !int.TryParse(endWaveObj, out int end))
            {
                return;
            }

            if (absCode != null)
            {
                // 计算波长点数
                int waveCount = ((end - start) / interval) + 2;

                // 解析吸光度数据为 double 数组
                var absValues = absCode.Split('/')
                    .Select(v => double.TryParse(v, out double result) ? result : 0.0)
                    .ToArray();

                // 长度校验，不一致则不显示
                if (absValues.Length != waveCount)
                {
                    return;
                }

                // 默认名称为配方代码
                if (name == null)
                {
                    name = $"{this.ctDataGridView1.Rows.Count + 1}-{dataRow[My_DataBase.ABS_HISTORY_HEAD.FormulaCode]?.ToString()}";
                }


                // 添加曲线到图表
                this.ctUvCurveChart1.AddCurveAutoColor(name, absValues, start, interval);

                // 计算 L, a, b 色度值
                var (l, a, b) = My_Tool.ColorCalculation.CalculateLAB(absValues, start, end, interval, 10);

                // 添加到色轮图
                this.ctLabColorWheelPanel1.AddPoint(a, b);

                // 添加到表格
                if (this.ctDataGridView1.Rows.Count == 0)
                {
                    // 标样数据，保存吸光度数组和波长参数
                    this._sABS = new double[absValues.Length];
                    this._sABS = absValues;
                    this._sStartWave = start;
                    this._sEndWave = end;
                    this._sIntervalWave = interval;

                    this.ctDataGridView1.Rows.Add(
                        this.ctDataGridView1.Rows.Count + 1,
                        FormatValue(l), FormatValue(a), FormatValue(b),
                        null, null, null, null, null, null, null
                    );
                }
                else
                {
                    // 试样数据，计算与标样的色差等指标
                    var sl = this.ctDataGridView1.Rows[0].Cells[1].Value?.ToString();
                    var sa = this.ctDataGridView1.Rows[0].Cells[2].Value?.ToString();
                    var sb = this.ctDataGridView1.Rows[0].Cells[3].Value?.ToString();

                    var dl = l - Convert.ToDouble(sl);
                    var da = a - Convert.ToDouble(sa);
                    var db = b - Convert.ToDouble(sb);

                    // 计算色差 CMC
                    var decmc = My_Tool.ColorCalculation.CalculateCMC(
                        Convert.ToDouble(sl), Convert.ToDouble(sa), Convert.ToDouble(sb),
                        l, a, b, 2, 1);

                    // 标样参数有效性校验
                    if (this._sABS != null)
                    {
                        // 计算重叠波长区间
                        int overlapStart = Math.Max(start, this._sStartWave);
                        int overlapEnd = Math.Min(end, this._sEndWave);
                        int overlapInterval = (interval == this._sIntervalWave) ? interval : Math.Min(interval, this._sIntervalWave);

                        // 计算重叠区间的点数
                        int overlapCount = ((overlapEnd - overlapStart) / overlapInterval) + 1;

                        // 如果重叠区间无效，则不显示
                        if (overlapCount <= 0)
                        {
                            return;
                        }

                        // 构造重叠区间的吸光度数组
                        double[] sampleOverlap = new double[overlapCount];
                        double[] stdOverlap = new double[overlapCount];

                        for (int i = 0; i < overlapCount; i++)
                        {
                            int wave = overlapStart + i * overlapInterval;

                            // 计算试样和标样在各自数组中的索引
                            int sampleIdx = (wave - start) / interval;
                            int stdIdx = (wave - this._sStartWave) / this._sIntervalWave;

                            // 索引越界则不显示
                            if (sampleIdx < 0 || sampleIdx >= absValues.Length ||
                                stdIdx < 0 || stdIdx >= this._sABS.Length)
                            {
                                return;
                            }

                            sampleOverlap[i] = absValues[sampleIdx];
                            stdOverlap[i] = this._sABS[stdIdx];
                        }

                        // 计算强度最大吸收波长
                        var swl = My_Tool.ColorCalculation.SWL(sampleOverlap, stdOverlap);

                        // 计算外观力份
                        var sum = My_Tool.ColorCalculation.SUM(sampleOverlap, stdOverlap, overlapStart, overlapEnd, overlapInterval);

                        // 计算综合强度
                        var wsum = My_Tool.ColorCalculation.WSUM(sampleOverlap, stdOverlap, overlapStart, overlapEnd, overlapInterval, 10);

                        this.ctDataGridView1.Rows.Add(
                            this.ctDataGridView1.Rows.Count + 1,
                            FormatValue(l), FormatValue(a), FormatValue(b),
                            FormatValue(dl), FormatValue(da), FormatValue(db),
                            FormatValue(decmc), FormatValue(swl), FormatValue(sum), FormatValue(wsum)
                        );
                    }
                }

                // 自动适应所有列宽
                this.ctDataGridView1.AutoFitAllColumns();
                this.ctDataGridView1.ClearSelection();
            }
        }

        private void Tsmi_Insert_Click(object sender, EventArgs e)
        {
            var row = this.ctHistoryDyeBrowse1.CurrentRow;
            if (row != null)
            {
                // 获取当前选中记录的ID
                int id = Convert.ToInt32(row[My_DataBase.ABS_HISTORY_HEAD.MyID]);

                // 查询整行数据（所有字段）
                DataTable dt = SqlServer.Select(
                    My_DataBase.ABS_HISTORY_HEAD.TableName,
                    null, // null表示查询所有字段
                    $"{My_DataBase.ABS_HISTORY_HEAD.MyID}=@id",
                    null,
                    true,
                    new System.Data.SqlClient.SqlParameter("@id", id)
                );

                if (dt != null && dt.Rows.Count > 0)
                {
                    // 再添加当前试样数据
                    AddABSDate(dt.Rows[0]);
                }
            }
        }

        private void Tsmi_Clear_Click(object sender, EventArgs e)
        {
            ClearControlsData();
        }

        private void Tsmi_SetStand_Click(object sender, EventArgs e)
        {
            var row = this.ctHistoryDyeBrowse1.CurrentRow;
            if (row != null)
            {
                // 获取当前选中记录的ID
                int id = Convert.ToInt32(row[My_DataBase.ABS_HISTORY_HEAD.MyID]);

                if (DialogResult.OK == LocalTranslator.ShowMessage("确定将当前行设为标样吗？", "温馨提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                {
                    // 查询当前记录的配方代码和类型
                    DataTable dt = SqlServer.Select(
                        My_DataBase.ABS_HISTORY_HEAD.TableName,
                        null,
                        $"{My_DataBase.ABS_HISTORY_HEAD.MyID}=@id",
                        null,
                        true,
                        new System.Data.SqlClient.SqlParameter("@id", id)
                    );

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        var formulaCode = dt.Rows[0][My_DataBase.ABS_HISTORY_HEAD.FormulaCode]?.ToString();
                        var type = dt.Rows[0][My_DataBase.ABS_HISTORY_HEAD.Type]?.ToString();

                        // 1. 先将同配方代码、类型下所有标样设为0
                        SqlServer.Update(
                            My_DataBase.ABS_HISTORY_HEAD.TableName,
                            new System.Collections.Generic.Dictionary<string, object>
                            {
                        { My_DataBase.ABS_HISTORY_HEAD.Stand, 0 }
                            },
                            $"{My_DataBase.ABS_HISTORY_HEAD.FormulaCode}=@code AND {My_DataBase.ABS_HISTORY_HEAD.Type}=@type AND {My_DataBase.ABS_HISTORY_HEAD.Stand}=1",
                            new System.Data.SqlClient.SqlParameter("@code", formulaCode),
                            new System.Data.SqlClient.SqlParameter("@type", type)
                        );

                        // 2. 再将当前记录设为标样
                        SqlServer.Update(
                            My_DataBase.ABS_HISTORY_HEAD.TableName,
                            new System.Collections.Generic.Dictionary<string, object>
                            {
                        { My_DataBase.ABS_HISTORY_HEAD.Stand, 1 }
                            },
                            $"{My_DataBase.ABS_HISTORY_HEAD.MyID}=@id",
                            new System.Data.SqlClient.SqlParameter("@id", id)
                        );

                        LocalTranslator.ShowMessage("设置标样成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 可选：刷新界面数据
                        this.ctHistoryDyeBrowse1.Reload();
                        CtHistoryDyeBrowse1_CurrentRowChanged(sender, e);
                    }
                }
            }
            else
            {
                LocalTranslator.ShowMessage("请先选择一条记录作为标样！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

         
    }
}