using System;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace HRtoVRChat_OSC_UI
{
    public class NETDetect
    {
        public static bool IsNet6Installed
        {
            get
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
        }

        private static bool didComplete;
        public static bool InstallNETSDK()
        {
            didComplete = false;
            try
            {
                new Thread(() =>
                {
                    using (WebClient client = new WebClient())
                    {
                        string outputFile = "windowsdesktop-runtime-6.0.4-win-x64.exe";
                        client.DownloadFileCompleted += (sender, args) =>
                        {
                            Process netInstaller = Process.Start("windowsdesktop-runtime-6.0.4-win-x64.exe");
                            while (!netInstaller?.HasExited ?? false){}
                            didComplete = true;
                        };
                        client.DownloadFileAsync(
                            new Uri(
                                "https://download.visualstudio.microsoft.com/download/pr/f13d7b5c-608f-432b-b7ec-8fe84f4030a1/" +
                                "5e06998f9ce23c620b9d6bac2dae6c1d/windowsdesktop-runtime-6.0.4-win-x64.exe"), outputFile);
                    }
                }).Start();
            }
            catch (Exception)
            {
                didComplete = true;
                return false;
            }
            while(!didComplete){}
            return true;
        }
    }
}