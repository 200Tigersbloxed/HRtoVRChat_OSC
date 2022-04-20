using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HRtoVRChat_OSC_UI
{
    public class NETDetect
    {
        public static bool IsNet6Installed()
        {
            Process cmd = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };
            cmd.Start();
            cmd.StandardInput.WriteLine("dotnet --info");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            string output = cmd.StandardOutput.ReadToEnd();
            string[] byLine = Regex.Split(output, "\r\n|\r|\n");
            bool foundNetSDK = false;
            for (int i = 0; i < byLine.Length; i++)
            {
                string line = byLine[i];
                if (line.Contains(".NET runtimes"))
                {
                    // Get the next line
                    int y = i + 1;
                    string nextSDK = byLine[y];
                    while (nextSDK != String.Empty)
                    {
                        try
                        {
                            string dotSplit = nextSDK.Substring(2).Split(' ')[1].Split('.')[0];
                            if (Convert.ToInt32(dotSplit) >= 6)
                                foundNetSDK = true;
                            y++;
                            nextSDK = byLine[y];
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                }
            }
            return foundNetSDK;
        }
        
        public static void InstallNETSDK()
        {
            Process netInstaller =
                Process.Start(
                    "https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-6.0.4-windows-x64-installer");
            DialogResult dr = MessageBox.Show("Please click OK once you've finished installing", "HRtoVRChat_OSC_UI",
                MessageBoxButtons.OK);
        }
    }
}