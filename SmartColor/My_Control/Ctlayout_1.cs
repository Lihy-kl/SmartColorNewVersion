using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Control
{
    public partial class Ctlayout_1 : UserControl
    {
        public Ctlayout_1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            Panel[] panels = { panel1, panel2, panel3};
            object[] areas = { My_ConPar.Area.Layout_1.Area_1, My_ConPar.Area.Layout_1.Area_2, My_ConPar.Area.Layout_1.Area_3 };

            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].Controls.Clear();
                panels[i].Controls.Add(My_File.ConfigHelper.FillControlsn((My_ConPar.Area.Base)areas[i]));
            }
        }



        }
}
