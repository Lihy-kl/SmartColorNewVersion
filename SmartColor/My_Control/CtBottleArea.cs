using SmartColor.My_ConPar.Area.Balance;
using SmartColor.My_File;
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
    /// <summary>
    /// 母液瓶区域
    /// </summary>
    public partial class CtBottleArea : UserControl
    {
        //public Action<ICustomUpdatable> RegisterCustomControl; // 由主窗体赋值

        internal Balance EmbeddedBalance { get; set; } = new Balance();
        private int _bottleNum = 0;
        private int _colCount = 0;
        public CtBottleArea()
        {
            InitializeComponent();
            this.Resize += (s, e) => DrawMotherBottles();
            DrawMotherBottles();
            My_Form.Login.LoginForm.UserChanged -= LoginForm_UserChanged;
            My_Form.Login.LoginForm.UserChanged += LoginForm_UserChanged;
            UpdateAddWaterMenuVisibility();
        }

        private void LoginForm_UserChanged(object sender, EventArgs e)
        {
            UpdateAddWaterMenuVisibility();
        }

        public void SetLayout(int bottleNum, int colCount, Balance balance = null)
        {
            _bottleNum = bottleNum;
            _colCount = colCount;
            if (balance != null)
                EmbeddedBalance = balance;
            DrawMotherBottles();
        }

        private void UpdateAddWaterMenuVisibility()
        {
            // 获取当前用户权限，1=操作员，2=工程师
            int purview = 1;
            if (SmartColor.My_Form.Login.LoginForm.UserCache.TryGetValue(Properties.Settings.Default.Account, out var userInfo))
            {
                purview = userInfo.Purview;
            }
            // 只有工程师权限可见
            TSMIBottleAddWater.Visible = (purview == 2);
        }

        private void DrawMotherBottles()
        {
            this.Controls.Clear();

            try
            {
                int bottleNum = this._bottleNum;
                int bottleColumn = this._colCount;

                if (bottleNum <= 0 || bottleColumn <= 0)
                {
                    return;
                }
                // 计算总列数
                int fullCols, remainder, totalCols;
                bool hasRemainder = bottleNum % bottleColumn != 0;

                if (!hasRemainder)
                {
                    fullCols = bottleNum / bottleColumn;
                    totalCols = fullCols;
                    remainder = 0;
                }
                else
                {
                    fullCols = bottleNum / bottleColumn - 1;
                    remainder = bottleNum % bottleColumn + bottleColumn;
                    totalCols = fullCols + 2;
                }

                int minSpacing = 4;
                // 动态计算瓶子尺寸
                int maxBottleWidth = (this.ClientSize.Width - minSpacing * (totalCols + 1)) / totalCols;
                int maxBottleHeight = (this.ClientSize.Height - minSpacing * (bottleColumn + 1)) / bottleColumn;
                Size bottleSize = new Size(
                    Math.Max(20, maxBottleWidth),
                    Math.Max(40, maxBottleHeight)
                );

                int usedWidth = totalCols * bottleSize.Width;
                int usedHeight = bottleColumn * bottleSize.Height;
                int spacingX = totalCols > 1 ? (this.ClientSize.Width - usedWidth) / (totalCols + 1) : (this.ClientSize.Width - usedWidth) / 2;
                int spacingY = bottleColumn > 1 ? (this.ClientSize.Height - usedHeight) / (bottleColumn + 1) : (this.ClientSize.Height - usedHeight) / 2;
                spacingX = Math.Max(minSpacing, spacingX);
                spacingY = Math.Max(minSpacing, spacingY);

                int index = 0;

                // 1. 画完整的列
                for (int c = 0; c < fullCols; c++)
                {
                    for (int r = 0; r < bottleColumn; r++)
                    {
                        Point loc = new Point(
                            spacingX + c * (bottleSize.Width + spacingX),
                            spacingY + r * (bottleSize.Height + spacingY)
                        );
                        var bottle = new CtBottle
                        {
                            Size = bottleSize,
                            Location = loc,
                            NO = (index + 1).ToString()
                        };
                        RegisterCustomControl?.Invoke(bottle); // 注册到主窗体
                        this.Controls.Add(bottle);

                        index++;
                    }
                }

                if (!hasRemainder)
                {
                    return;
                }

                // 2. 画天平
                int balanceCol = fullCols;
                int balanceRow = 0;
                Size balanceOriginalSize = new Size(160, 160);
                Size balanceLayoutSize = new Size(
                    bottleSize.Width * 2 + spacingX,
                    bottleSize.Height * 3 + spacingY * 2
                );
                float scaleW = (float)balanceLayoutSize.Width / balanceOriginalSize.Width;
                float scaleH = (float)balanceLayoutSize.Height / balanceOriginalSize.Height;
                float scale = Math.Min(scaleW, scaleH);
                Size balanceFinalSize = new Size(
                    Math.Max(40, (int)(balanceOriginalSize.Width * scale)),
                    Math.Max(40, (int)(balanceOriginalSize.Height * scale))
                );
                int balanceAreaX = spacingX + balanceCol * (bottleSize.Width + spacingX);
                int balanceAreaY = spacingY + balanceRow * (bottleSize.Height + spacingY);
                int balanceAreaWidth = bottleSize.Width * 2 + spacingX;
                int balanceAreaHeight = bottleSize.Height * 3 + spacingY * 2;
                Point balanceLoc = new Point(
                    balanceAreaX + (balanceAreaWidth - balanceFinalSize.Width) / 2,
                    balanceAreaY + (balanceAreaHeight - balanceFinalSize.Height) / 2
                );

                var balance = new CtBalance
                {
                    Size = balanceFinalSize,
                    Location = balanceLoc,
                    MaxValue = EmbeddedBalance.MaxValue,
                    NO = "天平"
                };
                this.Controls.Add(balance);

                // 3. 画天平下方的剩余瓶子
                int leftColCount = (remainder + 1) / 2;
                int rightColCount = remainder / 2;
                int bottleStartIndex = index + 1;

                for (int r = 0; r < leftColCount; r++)
                {
                    Point loc = new Point(
                        spacingX + balanceCol * (bottleSize.Width + spacingX),
                        spacingY + (r + 3) * (bottleSize.Height + spacingY)
                    );
                    var bottle = new CtBottle
                    {
                        Size = bottleSize,
                        Location = loc,
                        NO = (bottleStartIndex++).ToString()
                    };
                    RegisterCustomControl?.Invoke(bottle); // 注册到主窗体
                    this.Controls.Add(bottle);
                }

                for (int r = 0; r < rightColCount; r++)
                {
                    Point loc = new Point(
                        spacingX + (balanceCol + 1) * (bottleSize.Width + spacingX),
                        spacingY + (r + 3) * (bottleSize.Height + spacingY)
                    );
                    var bottle = new CtBottle
                    {
                        Size = bottleSize,
                        Location = loc,
                        NO = (bottleStartIndex++).ToString()
                    };
                    RegisterCustomControl?.Invoke(bottle); // 注册到主窗体
                    this.Controls.Add(bottle);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CtMotherBottleArea DrawMotherBottles", ex);
            }
        }

        private Action<My_Interface.ICustomUpdatable> _registerCustomControl;
        public Action<My_Interface.ICustomUpdatable> RegisterCustomControl
        {
            get => _registerCustomControl;
            set
            {
                _registerCustomControl = value;
                // 只要赋值就重绘，保证注册
                DrawMotherBottles();
            }
        }

        private void TSMIWaterCorrection_Click(object sender, EventArgs e)
        {
            var frm = new SmartColor.My_Form.Correction.WaterCorrection();
            frm.Show(); // 非模态显示，不影响主界面操作
        }

        private void TMSIDMF_Click(object sender, EventArgs e)
        {
            var frm = new SmartColor.My_Form.Correction.DMFCorrection();
            frm.Show(); // 非模态显示，不影响主界面操作
        }

        private void TSMIBottleCorrection_Click(object sender, EventArgs e)
        {
            var frm = new SmartColor.My_Form.Correction.BottleCorrection();
            frm.Show(); // 非模态显示，不影响主界面操作
        }

        private void TSMIBottleSelf_Click(object sender, EventArgs e)
        {
            var frm = new SmartColor.My_Form.Correction.BottleSelf();
            frm.Show(); // 非模态显示，不影响主界面操作
        }

        private void TSMIBottleABS_Click(object sender, EventArgs e)
        {

        }

        private void TSMIBottleWash_Click(object sender, EventArgs e)
        {
            var frm = new SmartColor.My_Form.Correction.WashSyringe();
            frm.Show(); // 非模态显示，不影响主界面操作
        }

        private void TSMIBottleAddWater_Click(object sender, EventArgs e)
        {
            var frm = new SmartColor.My_Form.Correction.BottleAddWater();
            frm.Show(); // 非模态显示，不影响主界面操作
        }
    }
}