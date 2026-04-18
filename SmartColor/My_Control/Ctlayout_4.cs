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
    public partial class Ctlayout_4 : UserControl
    {
        public Ctlayout_4()
        {
            InitializeComponent();
            Init();
           
        }

        private void Init()
        {
            Panel[] panels = { panel1, panel2, panel3, panel4, panel5, panel6,panel7,panel8,
                               panel9,panel10,panel11,panel12,panel13,panel14,panel15,panel16};
            object[] areas = { My_ConPar.Area.Layout_4.Area_1,
                               My_ConPar.Area.Layout_4.Area_2,
                               My_ConPar.Area.Layout_4.Area_3,
                               My_ConPar.Area.Layout_4.Area_4,
                               My_ConPar.Area.Layout_4.Area_5,
                               My_ConPar.Area.Layout_4.Area_6,
                               My_ConPar.Area.Layout_4.Area_7,
                               My_ConPar.Area.Layout_4.Area_8,
                               My_ConPar.Area.Layout_4.Area_9,
                               My_ConPar.Area.Layout_4.Area_10,
                               My_ConPar.Area.Layout_4.Area_11,
                               My_ConPar.Area.Layout_4.Area_12,
                               My_ConPar.Area.Layout_4.Area_13,
                               My_ConPar.Area.Layout_4.Area_14,
                               My_ConPar.Area.Layout_4.Area_15,
                               My_ConPar.Area.Layout_4.Area_16
                             };

            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].Controls.Clear();
                panels[i].Controls.Add(My_File.ConfigHelper.FillControlsn((My_ConPar.Area.Base)areas[i]));
            }
        }
    }
}
