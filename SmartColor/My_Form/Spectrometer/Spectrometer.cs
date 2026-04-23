using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Form.Spectrometer
{
    public partial class Spectrometer : Form
    {
        private int _ID;
        private string _formulaCode;
        private string _versionNum;
        public Spectrometer(string formulaCode, string versionNum, int id)
        {
            InitializeComponent();
            _formulaCode = formulaCode;
            _versionNum = versionNum;
            _ID = id;
            this.Text = $"分光仪测色 - 序号:{_ID} - 配方代码:{_formulaCode} - 版本号:{_versionNum}";
        }

        private void TsbWhiteCorrection_Click(object sender, EventArgs e)
        {

        }

        private void TsbBlackCorrection_Click(object sender, EventArgs e)
        {

        }

        private void TsbStandardSample_Click(object sender, EventArgs e)
        {

        }

        private void TsbMeasureTheSample_Click(object sender, EventArgs e)
        {

        }

        private void BtnSave_Click(object sender, EventArgs e)
        {

        }

        private void TsmiStandardSampleData_Click(object sender, EventArgs e)
        {
            using (var fmr = new SmartColor.My_Form.Spectrometer.StandardSampleData())
            {
                fmr.ShowDialog();
            }
        }

        private void TsmiMeasureTheSampleData_Click(object sender, EventArgs e)
        {
            using (var fmr = new SmartColor.My_Form.Spectrometer.MeasureTheSampleData())
            {
                fmr.ShowDialog();
            }
        }
    }
}
