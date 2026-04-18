using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SmartColor.My_File;

namespace SmartColor.My_Control
{
    /// <summary>
    /// 自定义TabControl，支持标签页关闭按钮和双击分离为独立窗口
    /// </summary>
    public class CtTab : TabControl
    {
        private const int CLOSE_BUTTON_SIZE = 23;
        private const int CLOSE_BUTTON_MARGIN = 20;
        private readonly Image _closeButtonImage;
        private readonly Dictionary<string, Form> _detachedForms = new Dictionary<string, Form>();

        public CtTab()
        {
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.Padding = new Point(CLOSE_BUTTON_SIZE + CLOSE_BUTTON_MARGIN, 3);
            this._closeButtonImage = Properties.Resources.close; // 需确保有close.png资源
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            Rectangle tabRect = this.GetTabRect(e.Index);
            tabRect.Inflate(-2, -2);

            Font font = this.Font;
            Color color = this.TabPages[e.Index].ForeColor;

            if (e.Index == this.SelectedIndex)
            {
                font = new Font(this.Font, FontStyle.Bold);
                color = Color.Red;
            }

            TextRenderer.DrawText(e.Graphics, this.TabPages[e.Index].Text, font, tabRect, color);

            if (e.Index != 0)
            {
                Rectangle closeButtonRect = new Rectangle(
                    tabRect.Right - CLOSE_BUTTON_SIZE,
                    tabRect.Top,
                    CLOSE_BUTTON_SIZE,
                    CLOSE_BUTTON_SIZE);
                e.Graphics.DrawImage(_closeButtonImage, closeButtonRect);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            try
            {
                for (int i = 0; i < this.TabPages.Count; i++)
                {
                    if (i == 0) continue;

                    Rectangle tabRect = this.GetTabRect(i);
                    tabRect.Inflate(-2, -2);
                    Rectangle closeButtonRect = new Rectangle(
                        tabRect.Right - CLOSE_BUTTON_SIZE,
                        tabRect.Top,
                        CLOSE_BUTTON_SIZE,
                        CLOSE_BUTTON_SIZE);

                    if (closeButtonRect.Contains(e.Location))
                    {
                        foreach (Control ctrl in this.TabPages[i].Controls)
                        {
                            if (ctrl is Form f)
                            {
                                f.Close();
                            }
                        }
                        Logger.Info($"CtTab: Closed tab '{this.TabPages[i].Name}' at index {i}.");
                        this.TabPages.RemoveAt(i);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CtTab OnMouseClick", ex);
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            try
            {
                for (int i = 0; i < this.TabPages.Count; i++)
                {
                    Rectangle tabRect = this.GetTabRect(i);
                    tabRect.Inflate(-2, -2);

                    Rectangle closeButtonRect = new Rectangle(
                        tabRect.Right - CLOSE_BUTTON_SIZE,
                        tabRect.Top,
                        CLOSE_BUTTON_SIZE,
                        CLOSE_BUTTON_SIZE);

                    if (tabRect.Contains(e.Location) && !closeButtonRect.Contains(e.Location))
                    {
                        TabPage tabPage = this.TabPages[i];
                        Form form = null;
                        foreach (Control ctrl in tabPage.Controls)
                        {
                            if (ctrl is Form f)
                            {
                                form = f;
                                break;
                            }
                        }
                        if (form != null)
                        {
                            if (form.Text == "首页") return;
                            tabPage.Controls.Remove(form);
                            form.TopLevel = true;
                            form.FormBorderStyle = FormBorderStyle.Sizable;
                            form.Dock = DockStyle.None;
                            form.FormClosed += (s, args) => _detachedForms.Remove(tabPage.Name);
                            form.Show();
                            this.TabPages.Remove(tabPage);
                            _detachedForms[tabPage.Name] = form;
                            Logger.Info($"CtTab: Detached tab '{tabPage.Name}' to window.");
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("CtTab OnMouseDoubleClick", ex);
            }
        }

        public void OpenTab(string key, string title, Form form)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OpenTab(key, title, form)));
                return;
            }

            try
            {
                TabPage tabPage = FindTabPageByKey(key);
                if (tabPage == null)
                {
                    if (_detachedForms.TryGetValue(key, out Form detachedForm))
                    {
                        detachedForm.Hide();
                        detachedForm.TopLevel = false;
                        detachedForm.FormBorderStyle = FormBorderStyle.None;
                        detachedForm.Dock = DockStyle.Fill;
                        tabPage = new TabPage(title + "   ") { Name = key };
                        tabPage.Controls.Add(detachedForm);
                        detachedForm.Show();
                        this.TabPages.Add(tabPage);
                        _detachedForms.Remove(key);
                        Logger.Info($"CtTab: Re-embedded detached tab '{key}'.");
                    }
                    else
                    {
                        tabPage = new TabPage(title + "   ") { Name = key };
                        form.TopLevel = false;
                        form.FormBorderStyle = FormBorderStyle.None;
                        form.Dock = DockStyle.Fill;
                        tabPage.Controls.Add(form);
                        form.Show();
                        this.TabPages.Add(tabPage);
                        Logger.Info($"CtTab: Opened new tab '{key}'.");
                    }
                }
                this.SelectedTab = tabPage;
            }
            catch (Exception ex)
            {
                Logger.Error($"CtTab OpenTab key={key}", ex);
            }
        }

        private TabPage FindTabPageByKey(string key)
        {
            foreach (TabPage tabPage in this.TabPages)
            {
                if (tabPage.Name == key)
                {
                    return tabPage;
                }
            }
            if (_detachedForms.ContainsKey(key))
            {
                return null;
            }
            return null;
        }
    }
}