using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace HRtoVRChat_OSC_UI
{
    public static class SystemTrayManager
    {
        private static NotifyIcon notifyIcon;

        private static bool isHidden;
        private static MenuItem hiddenMI;
        private static MenuItem statusMI;
        private static MenuItem processesMI;
        private static MenuItem autostartMI;
        private static MenuItem skipvrccheckMI;

        public static void Init(MainForm mainForm)
        {
            // Tray Menu
            ContextMenu trayMenu = new ContextMenu();
            statusMI = trayMenu.MenuItems.Add("Status: " + (mainForm.IsOSCRunning ? "RUNNING" : "STOPPED"),
                (sender, args) => { });
            processesMI = trayMenu.MenuItems.Add("Active Processes: 0", (sender, args) => { });
            trayMenu.MenuItems.Add(new MenuItem("-"));
            autostartMI = trayMenu.MenuItems.Add("Auto Start", (sender, args) =>
            {
                mainForm.UpdateCheckboxLinks(ref mainForm.autoStartChecked, !mainForm.autoStartChecked);
                autostartMI.Checked = mainForm.autoStartChecked;
            });
            skipvrccheckMI = trayMenu.MenuItems.Add("Skip VRChat Check", (sender, args) =>
            {
                mainForm.UpdateCheckboxLinks(ref mainForm.skipvrccheckChecked, !mainForm.skipvrccheckChecked);
                skipvrccheckMI.Checked = mainForm.skipvrccheckChecked;
            });
            trayMenu.MenuItems.Add(new MenuItem("-"));
            trayMenu.MenuItems.Add("Start", (sender, args) => { mainForm.StartProgram(); });
            trayMenu.MenuItems.Add("Stop", (sender, args) => { mainForm.StopProgram(); });
            trayMenu.MenuItems.Add("Kill All Processes", (sender, args) =>
            {
                foreach (Process process in Process.GetProcessesByName("HRtoVRChat_OSC"))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception)
                    {
                    }
                }
            });
            trayMenu.MenuItems.Add(new MenuItem("-"));
            hiddenMI = trayMenu.MenuItems.Add(isHidden ? "Show Application" : "Hide Application", (sender, args) =>
            {
                isHidden = !isHidden;
                if(isHidden)
                    mainForm.Hide();
                else
                    mainForm.Show();
                hiddenMI.Text = isHidden ? "Show Application" : "Hide Application";
            });
            trayMenu.MenuItems.Add("Exit", (sender, args) =>
            {
                if(mainForm.IsOSCRunning)
                    mainForm.StopProgram();
                Application.Exit();
            });
            // TrayIcon
            using (Stream stream = Assembly.GetExecutingAssembly()
                       .GetManifestResourceStream("HRtoVRChat_OSC_UI.hrtovrchat_logo.ico"))
            {
                if (stream != null)
                {
                    notifyIcon = new NotifyIcon
                    {
                        Text = "HRtoVRChat_OSC_UI",
                        Icon = new Icon(stream),
                        ContextMenu = trayMenu,
                        Visible = true
                    };
                }
            }
        }

        public static void UpdateStatusMI(bool toggle)
        {
            if (statusMI != null)
            {
                statusMI.Checked = toggle;
                statusMI.Text = "Status: " + (toggle ? "RUNNING" : "STOPPED");
            }
        }

        public static void UpdateProcessesCount(int count)
        {
            if(processesMI != null)
                processesMI.Text = "Active Processes: " + count;
        }

        public static void UpdateLaunchOptions((bool, bool) options)
        {
            autostartMI.Checked = options.Item1;
            skipvrccheckMI.Checked = options.Item2;
        }
    }
}