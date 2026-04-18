using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SmartColor.My_File;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 出布区域控件
    /// </summary>
    public partial class CtOutClothArea : UserControl
    {
        private int _rowCount = 3;
        private int _colCount = 4;
        private int _startNo = 1;
        private GroupBox _groupBox;
        private int _vertical = 1; // 0-横着编号,1-竖着编号

        public CtOutClothArea()
        {
            InitializeComponent();
            InitGroupBox("X号出布区");
            this.Resize += (s, e) => LayoutCloths();
            LayoutCloths();
        }

        /// <summary>
        /// 初始化GroupBox
        /// </summary>
        private void InitGroupBox(string text)
        {
            if (_groupBox == null)
            {
                _groupBox = new GroupBox
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                    Font = new Font("微软雅黑", 10, FontStyle.Bold)
                };
                this.Controls.Add(_groupBox);
            }
            else
            {
                _groupBox.Text = text;
            }
        }

        /// <summary>
        /// 设置行数、列数、起始编号和GroupBox标题，自动布局
        /// </summary>
        public void SetLayout(int rowCount, int colCount, int startNo = 1, int vertical = 1, string groupText = "出布区")
        {

            _rowCount = colCount;
            _colCount = rowCount;

            _startNo = startNo;
            _vertical = vertical;
            InitGroupBox(groupText);
            LayoutCloths();
        }

        private void LayoutCloths()
        {
            if (_groupBox == null) return;

            _groupBox.SuspendLayout();
            _groupBox.Controls.Clear();

            try
            {
                Rectangle area = _groupBox.DisplayRectangle;
                int totalWidth = area.Width;
                int totalHeight = area.Height;

                int spacingX = 8; // 固定间隔
                int spacingY = 8;

                int clothWidth = (totalWidth - (_colCount + 1) * spacingX) / _colCount;
                int clothHeight = (totalHeight - (_rowCount + 1) * spacingY) / _rowCount;

                int startX = area.X;
                int startY = area.Y;

                int no = _startNo;
                if (_vertical == 0) // 横着编号
                {
                    for (int row = 0; row < _rowCount; row++)
                    {
                        for (int col = 0; col < _colCount; col++)
                        {
                            int x = startX + spacingX + col * (clothWidth + spacingX);
                            int y = startY + spacingY + row * (clothHeight + spacingY);
                            var cloth = new CtPrepareCloth
                            {
                                Size = new Size(clothWidth, clothHeight),
                                Location = new Point(x, y),
                                NO = no.ToString()
                            };
                            _groupBox.Controls.Add(cloth);
                            no++;
                        }
                    }
                }
                else
                {
                    // 竖着编号
                    for (int col = 0; col < _colCount; col++)
                    {
                        for (int row = 0; row < _rowCount; row++)
                        {
                            int x = startX + spacingX + col * (clothWidth + spacingX);
                            int y = startY + spacingY + row * (clothHeight + spacingY);

                            var cloth = new CtPrepareCloth
                            {
                                Size = new Size(clothWidth, clothHeight),
                                Location = new Point(x, y),
                                NO = no.ToString()
                            };
                            _groupBox.Controls.Add(cloth);
                            no++;
                        }
                    }
                }

            }
          
            catch (Exception ex)
            {
                Logger.Error("CtOutClothArea LayoutCloths", ex);
            }

            _groupBox.ResumeLayout();
        }

        /// <summary>
        /// 更改指定编号出布控件的属性
        /// </summary>
        public void UpdateCloth(string no, Action<CtOutCloth> updateAction)
        {
            if (_groupBox == null || string.IsNullOrEmpty(no) || updateAction == null) return;

            try
            {
                foreach (Control ctrl in _groupBox.Controls)
                {
                    if (ctrl is CtOutCloth cloth && cloth.NO == no)
                    {
                        updateAction(cloth);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"CtOutClothArea UpdateCloth NO={no}", ex);
            }
        }

        /// <summary>
        /// 批量更改所有出布控件属性
        /// </summary>
        public void UpdateAllCloths(Action<CtOutCloth> updateAction)
        {
            if (_groupBox == null || updateAction == null) return;

            try
            {
                foreach (Control ctrl in _groupBox.Controls)
                {
                    if (ctrl is CtOutCloth cloth)
                    {
                        updateAction(cloth);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CtOutClothArea UpdateAllCloths", ex);
            }
        }

        /// <summary>
        /// 获取所有出布控件
        /// </summary>
        public CtOutCloth[] GetAllCloths()
        {
            return _groupBox?.Controls.OfType<CtOutCloth>().ToArray() ?? Array.Empty<CtOutCloth>();
        }
    }
}