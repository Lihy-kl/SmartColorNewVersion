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
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SmartColor.My_Form.ConPar
{
    /// <summary>
    /// 配置参数显示与编辑窗口。
    /// 支持通过 DataGridView 展示和编辑类型或实例的参数，并在关闭时选择是否保存更改。
    /// </summary>
    public partial class ConParShow : Form
    {
        // 配置文件名
        private readonly string _fileName;
        // 需要展示参数的类型（用于静态参数）
        private readonly Type _type;
        // 需要展示参数的实例对象（用于实例参数，可为 null）
        private readonly object _instanceObj;

        private bool _allowExit = false;

        /// <summary>
        /// 构造函数，初始化用于静态参数的窗口。
        /// </summary>
        /// <param name="name">窗口标题前缀</param>
        /// <param name="fileName">配置文件名</param>
        /// <param name="type">参数类型</param>
        public ConParShow(string name, string fileName, Type type,bool allowExit =false)
        {
            InitializeComponent();
            this._type = type;
            this._fileName = fileName;
            this.Text = name + "配置参数";
            this._allowExit = allowExit;
        }



        /// <summary>
        /// 构造函数，初始化用于实例参数的窗口。
        /// </summary>
        /// <param name="name">窗口标题前缀</param>
        /// <param name="fileName">配置文件名</param>
        /// <param name="type">参数类型</param>
        /// <param name="instanceObj">参数实例对象</param>
        /// <param name="allowExit">是否允许退出</param>
        public ConParShow(string name, string fileName, Type type, object instanceObj,bool allowExit = false)
            : this(name, fileName, type, allowExit)
        {
            this._instanceObj = instanceObj;
           
            
        }

        


        /// <summary>
        /// 窗体加载事件，根据是否有实例对象加载参数到 DataGridView。
        /// </summary>
        private void ConParShow_Load(object sender, EventArgs e)
        {
            if (_instanceObj != null)
            {
                // 加载实例参数到 DataGridView
                My_File.ConfigHelper.LoadTypeToDataGridView(dataGridView1, _instanceObj);
            }
            else
            {
                // 加载静态参数到 DataGridView
                My_File.ConfigHelper.LoadTypeToDataGridView(dataGridView1, this._type);
            }

          
        }

        /// <summary>
        /// 窗体关闭事件，提示用户是否保存更改，并根据选择保存参数。
        /// </summary>
        private void ConParShow_FormClosing(object sender, FormClosingEventArgs e)
        {
            dataGridView1.EndEdit(); // 结束编辑，确保数据同步
            DialogResult dialogResult = My_File.LocalTranslator.ShowMessage(
                "确认保存吗？", this.Text + "保存",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                if (_instanceObj != null)
                {
                    // 保存实例参数
                    My_File.ConfigHelper.WriteDataGridViewToType(dataGridView1, _instanceObj);
                    if(!(_instanceObj is My_ConPar.Area.Base))
                    {
                        // 同步写入配置文件
                        var path = Path.Combine(Environment.CurrentDirectory, "Config", this._fileName);
                        My_File.ConfigHelper.WriteAllValuesToFile(path, _instanceObj);
                    }

                }
            
                else
                {
                    // 保存静态参数
                    My_File.ConfigHelper.WriteDataGridViewToType(dataGridView1, this._type);
                    // 同步写入配置文件
                    var path = Path.Combine(Environment.CurrentDirectory, "Config", this._fileName);
                    My_File.ConfigHelper.WriteAllValuesToFile(path, this._type);
                }
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                if(this._allowExit)
                {
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }
                // 用户取消保存，阻止窗体关闭
                e.Cancel = true;
            }
        }
    }
}