using System;
using System.Drawing;
using System.Windows.Forms;

namespace BrushSound
{
    /// <summary>
    /// 系统托盘图标管理
    /// </summary>
    public class TrayIcon : IDisposable
    {
        private NotifyIcon notifyIcon;
        private Form mainForm;
        private bool firstHideShown = false;

        public event Action ExitRequested;

        public TrayIcon(Form mainForm)
        {
            this.mainForm = mainForm;

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Text = "画笔咻咻";
            notifyIcon.Visible = true;

            // 双击恢复窗口
            notifyIcon.DoubleClick += (s, e) => ShowMainWindow();

            // 右键菜单
            var menu = new ContextMenuStrip();
            menu.Items.Add("显示 画笔咻咻", null, (s, e) => ShowMainWindow());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("退出", null, (s, e) =>
            {
                if (ExitRequested != null) ExitRequested();
            });
            notifyIcon.ContextMenuStrip = menu;
        }

        /// <summary>
        /// 最小化到托盘
        /// </summary>
        public void HideToTray()
        {
            mainForm.Hide();
            mainForm.ShowInTaskbar = false;

            if (!firstHideShown)
            {
                firstHideShown = true;
                try
                {
                    notifyIcon.BalloonTipTitle = "画笔咻咻";
                    notifyIcon.BalloonTipText = "已缩小到系统托盘，双击图标可恢复窗口。";
                    notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                    notifyIcon.ShowBalloonTip(3000);
                }
                catch { }
            }
        }

        /// <summary>
        /// 恢复主窗口
        /// </summary>
        public void ShowMainWindow()
        {
            mainForm.Show();
            mainForm.ShowInTaskbar = true;
            if (mainForm.WindowState == FormWindowState.Minimized)
                mainForm.WindowState = FormWindowState.Normal;
            mainForm.Activate();
            mainForm.BringToFront();
        }

        /// <summary>
        /// 更新悬浮提示（可跟随自定义标题）
        /// </summary>
        public void UpdateTooltip(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) text = "画笔咻咻";
                if (text.Length > 63) text = text.Substring(0, 63);
                notifyIcon.Text = text;
            }
            catch { }
        }

        public void Dispose()
        {
            if (notifyIcon != null)
            {
                try { notifyIcon.Visible = false; } catch { }
                try { notifyIcon.Dispose(); } catch { }
                notifyIcon = null;
            }
        }
    }
}