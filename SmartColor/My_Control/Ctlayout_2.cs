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
    public partial class Ctlayout_2 : UserControl
    {
        public Ctlayout_2()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            Panel[] panels = { panel1, panel2, panel3,panel4,panel5,panel6 };
            object[] areas = { My_ConPar.Area.Layout_2.Area_1,
                               My_ConPar.Area.Layout_2.Area_2, 
                               My_ConPar.Area.Layout_2.Area_3,
                               My_ConPar.Area.Layout_2.Area_4,
                               My_ConPar.Area.Layout_2.Area_5,
                               My_ConPar.Area.Layout_2.Area_6};

            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].Controls.Clear();
                panels[i].Controls.Add(My_File.ConfigHelper.FillControlsn((My_ConPar.Area.Base)areas[i]));
            }
        }
    }
}
