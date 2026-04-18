using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Form.MachineDebugging
{
    public partial class LidLayout : Form
    {
        public int SelectedLayoutType { get; private set; } = 0; // 默认布局1

        public LidLayout()
        {
            InitializeComponent();
            Rdo1.CheckedChanged += (s, e) => { if (Rdo1.Checked) SelectedLayoutType = 0; };
            Rdo2.CheckedChanged += (s, e) => { if (Rdo2.Checked) SelectedLayoutType = 1; };
            Rdo3.CheckedChanged += (s, e) => { if (Rdo3.Checked) SelectedLayoutType = 2; };
            Rdo4.CheckedChanged += (s, e) => { if (Rdo4.Checked) SelectedLayoutType = 3; };
            Rdo5.CheckedChanged += (s, e) => { if (Rdo5.Checked) SelectedLayoutType = 4; };
            BtnSure.Click += (s, e) => {this.DialogResult = DialogResult.OK;this.Close(); };

        }
    }
}
