using SmartColor.My_RobotManager;
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
    public partial class CtTask : UserControl
    {
        private object _task;

        public CtTask(string index, object task)
        {
            InitializeComponent();
            if (LabIndex == null || LabType == null || LabTaskName == null || LabUseingTime == null)
                throw new InvalidOperationException("控件未正确初始化");
            _task = task;
            dynamic dt = task;
            LabIndex.Text = index;
            try
            {
                LabType.Text = My_ConPar.Order.BigProcess.GetTypeName(dt.BusinessType);
                LabTaskName.Text = dt.TaskName;
                LabUseingTime.Text = (DateTime.Now - dt.CreatedTime).ToString(@"hh\:mm\:ss");
            }
            catch
            {
                LabType.Text = "";
                LabTaskName.Text = "";
                LabUseingTime.Text = "";
            }
            UpdateStatus();

            // 明确设置控件大小和间距，便于布局
            this.Size = new Size(256, 61); // 根据实际需求调整
            this.Margin = new Padding(4, 4, 4, 4);
            this.BackColor = Color.LightYellow; // 便于观察
            // 注册事件到所有Label
            foreach (Control c in this.Controls)
            {
                c.MouseDown += (s, e) => this.OnMouseDown(e);
                c.MouseMove += (s, e) => this.OnMouseMove(e);
            }
            this.timer1.Start();
        }

       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (timer1 != null)
                {
                    timer1.Stop();
                    timer1.Dispose();
                }
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            dynamic dt = _task;
            try
            {
                LabUseingTime.Text = (DateTime.Now - dt.CreatedTime).ToString(@"hh\:mm\:ss");
            }
            catch
            {
                LabUseingTime.Text = "";
            }
        }

        public void UpdateStatus(bool run = false)
        {
            if (run)
            {
                LabIndex.BackColor = Color.LimeGreen;
                this.ContextMenuStrip = this.contextMenuStrip1;
            }
            else
            {
                LabIndex.BackColor = SystemColors.Control;
                this.ContextMenuStrip = this.contextMenuStrip2;
            }
        }

        private void TSMIPause_Click(object sender, EventArgs e)
        {
            SmartColor.My_RobotManager.RobotTaskManager.Instance.PauseTask(_task);
        }

        private void TSMIResume_Click(object sender, EventArgs e)
        {
            SmartColor.My_RobotManager.RobotTaskManager.Instance.ResumeTask(_task);
        }

        private void TMSICancel_Click(object sender, EventArgs e)
        {
            SmartColor.My_RobotManager.RobotTaskManager.Instance.CancelTask(_task);
        }

        public void SetTask(string index, object task)
        {
            _task = task;
            LabIndex.Text = index;
            dynamic dt = task;
            try
            {
                LabType.Text = My_ConPar.Order.BigProcess.GetTypeName(dt.BusinessType);
                LabTaskName.Text = dt.TaskName;
                LabUseingTime.Text = (DateTime.Now - dt.CreatedTime).ToString(@"hh\:mm\:ss");
            }
            catch
            {
                LabType.Text = "";
                LabTaskName.Text = "";
                LabUseingTime.Text = "";
            }
        }
    }
}