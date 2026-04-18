using SmartColor.My_ConPar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Form.ConPar
{
    public partial class AbortInfo : Form
    {
        public AbortInfo()
        {
            InitializeComponent();
        }
        private void UpdateInfo_Load(object sender, EventArgs e)
        {
            string version = Application.ProductVersion;

            switch (version)
            {
                case "1.0.0.0":
                    DoForVersion1000();
                    break;

                default:

                    break;
            }
        }


        private void AbortInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Config", "AbortInfo.ini");
            My_ConPar.AbortInfo.LastVersion =
                Application.ProductVersion;
            My_File.ConfigHelper.WriteAllValuesToFile(path,
                 typeof(My_ConPar.AbortInfo));
        }

        private void DoForVersion1000()
        {
            TxtUpdateInfo.Text =
                "1.0.0.0\r\n" +
                "初始版本发布\r\n";
        }


    }
}
