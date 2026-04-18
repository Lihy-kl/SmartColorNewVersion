using SmartColor.My_Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Form.UVMan
{
    public partial class UVMan : Form
    {
        private bool _isSwitchingFocus = false;
        public UVMan()
        {
            InitializeComponent();
            SetControlAttribute();

            this.ctFormulaBrowse1.CurrentRowChanged -= CurrentRowChangedHandler;
            this.ctFormulaBrowse1.CurrentRowChanged += CurrentRowChangedHandler;
            this.ctBatchData1.CurrentRowChanged -= CurrentRowChangedHandler;
            this.ctBatchData1.CurrentRowChanged += CurrentRowChangedHandler;
            this.ctDropDetail1.FormulaDataChange -= CtDropDetail1_FormulaDataChange;
            this.ctDropDetail1.FormulaDataChange += CtDropDetail1_FormulaDataChange;

            this.ctDropDetail1.BatchChange -= CtDropDetail1_BatchChange;
            this.ctDropDetail1.BatchChange += CtDropDetail1_BatchChange;
            this.ctDropDetail1.WaitChange -= CtDropDetail1_WaitChange;
            this.ctDropDetail1.WaitChange += CtDropDetail1_WaitChange;
        }

        private void CtDropDetail1_WaitChange(object sender, EventArgs e)
        {
            ctFormulaBrowse1.RequestRefresh();
        }

        private void CtDropDetail1_BatchChange(object sender, int e)
        {
            ctBatchData1.OnBatchRelatedEvent(e);
        }

        private void CtDropDetail1_FormulaDataChange(object sender, EventArgs e)
        {
            ctFormulaBrowse1.GetData();
            ctFormulaBrowse1.RequestRefresh();
        }

        private void SetControlAttribute()
        {
            // 设置CtDropHead控件属性
            this.ctDropHead1.SetMode(CtDropHead.Mode.ABS);

            // 设置CtDropDetail控件属性
            this.ctDropDetail1.SetMode(CtDropDetail.Mode.ABS);

            // 设置CtFormulaBrowse控件属性
            this.ctFormulaBrowse1.SetTableWaitName(My_DataBase.ABS_WAIT_LIST.TableName);

            // 设置CtBatchData控件属性
            this.ctBatchData1.SetTableName(My_DataBase.ABS_DROP_HEAD.TableName);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            if (keyData == Keys.F10)
            {
                if (ctBatchData1 != null && ctBatchData1.HandleShortcutKeys(keyData))
                    return true;
                return base.ProcessCmdKey(ref msg, keyData);
            }
            else
            {
                if (ctDropDetail1 != null && ctDropDetail1.HandleShortcutKeys(keyData))
                    return true;
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void CurrentRowChangedHandler(object sender, DataTable e)
        {
            if (_isSwitchingFocus) return; // 防止递归触发
            _isSwitchingFocus = true;

            if (sender == this.ctFormulaBrowse1)
            {
                this.ctBatchData1.ClearSelect();
            }
            else if (sender == this.ctBatchData1)
            {
                this.ctFormulaBrowse1.ClearSelect();
            }

            this.ctUvCurveChart1.ClearCurves();

            if (e == null || e.Rows.Count == 0)
            {
                _isSwitchingFocus = false;
                this.ctDropDetail1.ResetMode();
                this.ctDropHead1.ClearFormulaCode();
                return;
            }

            // 遍历所有行，分别显示标样和试样
            foreach (DataRow row in e.Rows)
            {
                var absObj = row[My_DataBase.ABS_HISTORY_HEAD.Abs];
                var standObj = row[My_DataBase.ABS_HISTORY_HEAD.Stand];
                var startWaveObj = row[My_DataBase.ABS_HISTORY_HEAD.StartWave];
                var intWaveObj = row[My_DataBase.ABS_HISTORY_HEAD.IntWave];

                if (startWaveObj == null || intWaveObj == null ||
                    !int.TryParse(startWaveObj.ToString(), out int start) ||
                    !int.TryParse(intWaveObj.ToString(), out int interval))
                {
                    _isSwitchingFocus = false;
                    this.ctDropDetail1.ResetMode();
                    this.ctDropHead1.ClearFormulaCode();
                    return;
                }
                if (absObj != null)
                {
                    var absValues = absObj.ToString()
                        .Split('/')
                        .Select(v => double.TryParse(v, out double result) ? result : 0.0)
                        .ToArray();

                    string name = "试样";
                    Color color = Color.Black;
                    if (standObj != DBNull.Value && standObj != null && Convert.ToInt16(standObj) == 1)
                    {
                        name = "标样";
                        color = Color.Red;
                    }

                    this.ctUvCurveChart1.AddCurve(name, color, absValues, start, interval);
                }
            }

            this.ctDropDetail1.ResetMode();
            this.ctDropHead1.ClearFormulaCode();

            _isSwitchingFocus = false;
        }

        private void UVMan_Load(object sender, EventArgs e)
        {
            this.ctDropDetail1.BindDropHead(ctDropHead1);
            this.ctDropDetail1.SetButtonEnabled();
            this.ctFormulaBrowse1.HeadTarget = ctDropHead1;
            this.ctBatchData1.HeadTarget = ctDropHead1;
            this.ctFormulaBrowse1.ClearSelect(); // 再次清空
        }
    }
}
