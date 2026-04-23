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

namespace SmartColor.My_Form.DyeingMan
{
    public partial class DyeingMan : Form
    {

        private bool _isSwitchingFocus = false;
        public DyeingMan()
        {
            InitializeComponent();
            SetControlAttribute();
            this.ctFormulaBrowse1.CurrentRowChanged -= CurrentRowChangedHandler;
            this.ctFormulaBrowse1.CurrentRowChanged += CurrentRowChangedHandler;

            this.ctDropRecord1.CurrentRowChanged -= CurrentRowChangedHandler;
            this.ctDropRecord1.CurrentRowChanged += CurrentRowChangedHandler;

            this.ctBatchData1.CurrentRowChanged -= CurrentRowChangedHandler;
            this.ctBatchData1.CurrentRowChanged += CurrentRowChangedHandler;

            this.ctDropDetail1.FormulaDataChange -= CtDropDetail1_FormulaDataChange;
            this.ctDropDetail1.FormulaDataChange += CtDropDetail1_FormulaDataChange;

            this.ctDropDetail1.BatchChange -= CtDropDetail1_BatchChange;
            this.ctDropDetail1.BatchChange += CtDropDetail1_BatchChange;

            this.ctDropDetail1.WaitChange -= CtDropDetail1_WaitChange;
            this.ctDropDetail1.WaitChange += CtDropDetail1_WaitChange;

            SmartColor.My_Tool.CupAuxiliary.CupFinished -= CupAuxiliary_CupFinished;
            SmartColor.My_Tool.CupAuxiliary.CupFinished += CupAuxiliary_CupFinished;
            
        }

        private void CtDropDetail1_FormulaDataChange(object sender, EventArgs e)
        {
            this.ctFormulaBrowse1.GetData();
            this.ctFormulaBrowse1.RequestRefresh();
        }

        private void CupAuxiliary_CupFinished()
        {
            this.ctFormulaBrowse1.RequestRefresh();
        }

        private void CtDropDetail1_WaitChange(object sender, EventArgs e)
        {
            this.ctFormulaBrowse1.RequestRefresh();
        }

        private void CtDropDetail1_BatchChange(object sender, int e)
        {
            this.ctBatchData1.OnBatchRelatedEvent(e);
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

        private void SetControlAttribute()
        {
            // 设置CtDropHead控件属性
            this.ctDropHead1.SetMode(CtDropHead.Mode.Dye);

            // 设置CtDropDetail控件属性
            this.ctDropDetail1.SetMode(CtDropDetail.Mode.DYE);

            // 设置CtFormulaBrowse控件属性
            this.ctFormulaBrowse1.SetTableWaitName(My_DataBase.WAIT_LIST.TableName);

            // 设置CtBatchData控件属性
            this.ctBatchData1.SetTableName( My_DataBase.DROP_HEAD.TableName);

        }



        private void DyeingMan_Load(object sender, EventArgs e)
        {
            this.ctDropDetail1.BindDropHead(ctDropHead1);
            this.ctDropDetail1.BindFormulaBrowse(ctFormulaBrowse1);
            this.ctDropDetail1.SetButtonEnabled();
            this.ctFormulaBrowse1.HeadTarget = ctDropHead1;
            this.ctBatchData1.HeadTarget = ctDropHead1;
            this.ctDropRecord1.DropHeadTarget = ctDropHead1;
            this.ctFormulaBrowse1.RequestRefresh();
            this.ctFormulaBrowse1.ClearSelect(); // 再次清空
            this.ctDropRecord1.ClearSelect();

        }

        private void CurrentRowChangedHandler(object sender, DataTable e)
        {
            if (this._isSwitchingFocus) return; // 防止递归触发
            this._isSwitchingFocus = true;

            if (sender == this.ctFormulaBrowse1)
            {
                this.ctDropRecord1.ClearSelect();
                this.ctBatchData1.ClearSelect();
            }
            else if (sender == this.ctDropRecord1)
            {
                this.ctFormulaBrowse1.ClearSelect();
                this.ctBatchData1.ClearSelect();
            }
            else if (sender == this.ctBatchData1)
            {
                this.ctDropRecord1.ClearSelect();
                this.ctFormulaBrowse1.ClearSelect();
            }
            this.ctDropDetail1.ResetMode();
            this.ctDropHead1.ClearFormulaCode();

            this._isSwitchingFocus = false;
        }
    }

}
