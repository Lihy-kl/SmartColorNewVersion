using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SmartColor
{
    internal static class Program
    {
        // 互斥体名称，确保唯一
        private static Mutex _mutex;

        // 导入Win32 API用于窗口操作
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int SW_SHOWMAXIMIZED = 3;

        [STAThread]
        static void Main()
        {
            bool createdNew;
            _mutex = new Mutex(true, "SmartColor_SingleInstance_Mutex", out createdNew);
            if (!createdNew)
            {
                // 查找已存在的进程
                var current = Process.GetCurrentProcess();
                foreach (var process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        // 激活并最大化已存在的窗口
                        IntPtr hWnd = process.MainWindowHandle;
                        if (hWnd != IntPtr.Zero)
                        {
                            ShowWindow(hWnd, SW_SHOWMAXIMIZED);
                            SetForegroundWindow(hWnd);
                        }
                        break;
                    }
                }
                return;
            }

            // 检查是否为管理员
            if (!IsRunAsAdmin())
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        FileName = Application.ExecutablePath,
                        Verb = "runas"
                    };
                    Process.Start(startInfo);
                }
                catch
                {
                    // 用户取消UAC
                }
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var splash = new My_Form.Login.SplashForm())
            {
                Application.Run(splash);
            }

            using (var loginForm = new My_Form.Login.LoginForm())
            {
                var result = loginForm.ShowDialog();
                if (result == DialogResult.Yes)
                {
                    Application.Run(new My_Form.Main.MainForm());
                }
            }
        }

        // 判断当前进程是否以管理员身份运行
        private static bool IsRunAsAdmin()
        {
            try
            {
                WindowsIdentity id = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(id);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }
    }
}