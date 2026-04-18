using SmartColor.My_PLC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartColor.My_Form.Help
{
    public partial class Abort : Form
    {
        public Abort()
        {
            InitializeComponent();
        }

        private void Abort_Load(object sender, EventArgs e)
        {
            string s_name = My_ConPar.AbortInfo.Manufacturer;
            label6.Text = s_name;
            string s_tel = My_ConPar.AbortInfo.Tel;
            label8.Text = s_tel;



            //设置内容居中显示

            ctDataGridView1.Rows.Add("软件版本", Application.ProductVersion);
            if (My_ConPar.Machine.MachineType == 0)
            {
                var plc = SmartColor.My_ConPar.Object.CurrentPLC as SmartColor.My_PLC.PLC;
                ctDataGridView1.Rows.Add("PLC版本", plc.GetPLCVersion());
            }
            if (My_ConPar.Machine.CuttingMachine == 2)
            {
                var inovanceConfig = SmartColor.My_ConPar.Object.CurrentCuttingObj as SmartColor.My_CuttingMachine.Inovance_TCP;
                ctDataGridView1.Rows.Add("开料机版本", inovanceConfig.ReadVersionInfo());
            }


            //显示打板机版本
            foreach (var area in My_AutomaticModule.CupCommManager.Instance.GetAllCupAreas())
            {
                switch (area.AreaType)
                {
                    case 2:
                        {

                            var comm = SmartColor.My_AutomaticModule.CupCommManager.Instance.GetCommObject(area) as SmartColor.My_Cup.FC_4;
                            if (comm != null)
                            {
                                var version = comm.ReadVersionInfo();
                                ctDataGridView2.Rows.Add(area.AreaName, version.Item1, version.Item2, version.Item3);
                            }
                        }
                        break;
                    case 3:
                        {

                            var comm = SmartColor.My_AutomaticModule.CupCommManager.Instance.GetCommObject(area) as SmartColor.My_Cup.FC_6;
                            if (comm != null)
                            {
                                var version = comm.ReadVersionInfo();
                                ctDataGridView2.Rows.Add(area.AreaName, version.Item1, version.Item2, version.Item3);
                            }
                        }
                        break;
                    case 4:
                        {

                            var comm = SmartColor.My_AutomaticModule.CupCommManager.Instance.GetCommObject(area) as SmartColor.My_Cup.FC_12;
                            if (comm != null)
                            {
                                var version = comm.ReadVersionInfo();
                                ctDataGridView2.Rows.Add(area.AreaName, version.Item1, version.Item2, version.Item3);
                            }
                        }

                        break;
                    case 5:
                        break;
                    default:
                        break;
                }

            }

            ctDataGridView1.ClearSelection();

        }

        private void DataGridView_Leave(object sender, EventArgs e)
        {
            var dataGrid = sender as My_Control.CtDataGridView;
            dataGrid.ClearSelection();
        }
    }
}
