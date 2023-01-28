using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace HRtoVRChat;

public static class SoftwareManager
{
    public static string LocalDirectory
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "HRtoVRChat");
            return String.Empty;
        }
    }

    public static string OutputPath
    {
        get
        {
            if (LocalDirectory != String.Empty)
                return Path.Combine(LocalDirectory, "HRtoVRChat_OSC");
            return "HRtoVRChat_OSC";
        }
    }

    public static string ExecutableName
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "HRtoVRChat_OSC.exe";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "HRtoVRChat_OSC-macos";
            return "HRtoVRChat_OSC-linux";
        }
    }
    public static string ExecutableLocation => Path.Combine(OutputPath, ExecutableName);
    public static bool IsInstalled => Directory.Exists(OutputPath) && File.Exists(ExecutableLocation);
    public static string gitUrl => "https://github.com/200Tigersbloxed/HRtoVRChat_OSC/releases/latest";
    public static string latestDownload =>
        "https://github.com/200Tigersbloxed/HRtoVRChat_OSC/releases/latest/download/HRtoVRChat_OSC.zip";

    public static bool IsUpdating { get; private set; } = false;

    public static Action<string?, string> OnConsoleUpdate = (line, color) => { };
    public static Action<int, int> RequestUpdateProgressBars = (x, y) => { };
    private static Process? CurrentProcess;
    private static StreamWriter? myStreamWriter;

    public static bool IsSoftwareRunning
    {
        get
        {
            bool nativeRunning;
            try
            {
                nativeRunning = !(CurrentProcess?.HasExited ?? true);
            }
            catch (Exception)
            {
                // Last resort, try the Process Way
                nativeRunning = Process.GetProcessesByName("HRtoVRChat_OSC").Length > 0;
            }
            return nativeRunning;
        }
    }

    private static string GetArgs()
    {
        List<string> Args = new();
        if (ConfigManager.LoadedUIConfig != null)
        {
            if(ConfigManager.LoadedUIConfig.AutoStart)
                Args.Add("--auto-start");
            if(ConfigManager.LoadedUIConfig.SkipVRCCheck)
                Args.Add("--skip-vrc-check");
            if(ConfigManager.LoadedUIConfig.NeosBridge)
                Args.Add("--neos-bridge");
            if(ConfigManager.LoadedUIConfig.UseLegacyBool)
                Args.Add("--use-01-bool");
            try
            {
                foreach (string s in ConfigManager.LoadedUIConfig.OtherArgs.Split(' '))
                    Args.Add(s);
            }catch(Exception){}
        }
        string newargs = String.Empty;
        foreach (string arg in Args)
            newargs += arg + " ";
        return newargs;
    }

    private static void ContinueStartSoftware()
    {
        CurrentProcess.Start();
        CurrentProcess.StandardInput.AutoFlush = true;
        myStreamWriter = CurrentProcess.StandardInput;
        CurrentProcess.BeginOutputReadLine();
    }

    public static void StartSoftware()
    {
        if (IsInstalled)
        {
            bool chmodHandle = false;
            CurrentProcess = new Process();
            CurrentProcess.StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = OutputPath,
                Arguments = GetArgs(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                CurrentProcess.StartInfo.FileName = Path.Combine(OutputPath, ExecutableName);
            else
            {
                chmodHandle = true;
                // Make sure the file is executable first
                Process chmodProcess = new Process
                {
                    StartInfo = new ProcessStartInfo("chmod", $"+x {Path.Combine(OutputPath, ExecutableName)}")
                    {
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };
                chmodProcess.Exited += (sender, args) =>
                {
                    CurrentProcess.StartInfo.FileName = Path.Combine(OutputPath, ExecutableName);
                    ContinueStartSoftware();
                };
                chmodProcess.Start();
            }
            CurrentProcess.OutputDataReceived += (sender, eventArgs) =>
            {
                OnConsoleUpdate.Invoke(eventArgs.Data, String.Empty);
            };
            if(!chmodHandle)
                ContinueStartSoftware();
        }
        else
        {
            MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                ButtonDefinitions = ButtonEnum.Ok,
                ContentTitle = "HRtoVRChat",
                ContentMessage = "HRtoVRChat_OSC is not installed! Please navigate to the Updates tab and install it.",
                WindowIcon = new WindowIcon(AssetTools.Icon),
                Icon = Icon.Error,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            }).Show();
        }
    }

    public static void SendCommand(string command)
    {
        if (IsSoftwareRunning && myStreamWriter != null)
        {
            try
            {
                myStreamWriter?.WriteLine(command);
                OnConsoleUpdate.Invoke("> " + command, "Purple");
            }
            catch (Exception)
            {
                MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
                {
                    ButtonDefinitions = ButtonEnum.Ok,
                    ContentTitle = "HRtoVRChat",
                    ContentMessage = "Failed to send command due to an error!",
                    WindowIcon = new WindowIcon(AssetTools.Icon),
                    Icon = Icon.Error,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                }).Show();
            }
        }
    }

    public static void StopSoftware()
    {
        if (IsSoftwareRunning)
        {
            try
            {
                SendCommand("exit");
                myStreamWriter?.Close();
            }
            catch(Exception){}
        }
    }
    
    private static string[] ExcludeFilesOnDelete =
    {
        "config.cfg"
    };

    private static string[] ExcludeDirectoriesOnDelete =
    {
        "SDKs",
        "Logs"
    };

    public static async Task InstallSoftware(Action? callback = null)
    {
        if (!IsUpdating)
        {
            if (!IsSoftwareRunning)
            {
                UpdateProgressBars(0, 0);
                // Make sure the Directory Exists
                if (!Directory.Exists(OutputPath))
                    Directory.CreateDirectory(OutputPath);
                // Check if there's any files in the Directory
                if (Directory.GetFiles(OutputPath).Length > 0)
                {
                    var messageBox = await MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
                    {
                        ButtonDefinitions = ButtonEnum.YesNo,
                        ContentTitle = "HRtoVRChat",
                        ContentMessage = "All files in " + Path.GetFullPath(OutputPath) + " are going to be deleted! Are you sure?",
                        WindowIcon = new WindowIcon(AssetTools.Icon),
                        Icon = Icon.Error,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    }).Show();
                    if ((messageBox & ButtonResult.Yes) != 0)
                    {
                        IsUpdating = true;
                        // Delete all files
                        foreach (string file in Directory.GetFiles(OutputPath))
                        {
                            if(!ExcludeFilesOnDelete.Contains(Path.GetFileName(file)))
                                DeleteFileAndWait(file);
                        }
                        // Delete all directories
                        foreach (string directory in Directory.GetDirectories(OutputPath))
                        {
                            if(!ExcludeDirectoriesOnDelete.Contains(new DirectoryInfo(Path.GetDirectoryName(directory)).Name))
                                DeleteDirectoryAndWait(directory);
                        }
                    }
                    else
                        return;
                }

                UpdateProgressBars(0, 25);
                // Download the file
                using (WebClient client = new WebClient())
                {
                    string outputFile = Path.Combine(OutputPath, "HRtoVRChat_OSC.zip");
                    client.DownloadFileCompleted += (sender, args) =>
                    {
                        UpdateProgressBars(0, 50);
                        // Extract the Zip
                        ZipFile.ExtractToDirectory(outputFile, OutputPath);
                        UpdateProgressBars(100, 50);
                        // Create version.txt
                        UpdateProgressBars(0, 75);
                        File.WriteAllText(Path.Combine(OutputPath, "version.txt"), GetLatestVersion());
                        UpdateProgressBars(100, 100);
                        IsUpdating = false;
                        callback?.Invoke();
                    };
                    client.DownloadProgressChanged += (sender, args) => UpdateProgressBars(args.ProgressPercentage, 25);
                    client.DownloadFileAsync(new Uri(latestDownload), outputFile);
                }
            }
            else
                MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
                {
                    ButtonDefinitions = ButtonEnum.Ok,
                    ContentTitle = "HRtoVRChat",
                    ContentMessage = "There's an instance of HRtoVRChat_OSC running, please close out of it before continuing!",
                    WindowIcon = new WindowIcon(AssetTools.Icon),
                    Icon = Icon.Error,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                }).Show();
        }
    }

    private static void UpdateProgressBars(int x, int y) => RequestUpdateProgressBars.Invoke(x, y);
    
    /// <summary>
    /// Method made by JPK from Stackoverflow
    /// No edits were made
    /// https://stackoverflow.com/a/39254732/12968919
    /// </summary>
    /// <param name="filepath">Path of the file</param>
    /// <param name="timeout">Optional Timeout</param>
    private static void DeleteFileAndWait(string filepath, int timeout = 30000)
    {
        using (var fw = new FileSystemWatcher(Path.GetDirectoryName(filepath), Path.GetFileName(filepath)))
        using (var mre = new ManualResetEventSlim())
        {
            fw.EnableRaisingEvents = true;
            fw.Deleted += (object sender, FileSystemEventArgs e) =>
            {
                mre.Set();
            };
            File.Delete(filepath);
            mre.Wait(timeout);
        }
    }
        
    private static void DeleteDirectoryAndWait(string directory, int timeout = 30000)
    {
        using (var fw = new FileSystemWatcher(Path.GetDirectoryName(directory)))
        using (var mre = new ManualResetEventSlim())
        {
            fw.EnableRaisingEvents = true;
            fw.Deleted += (object sender, FileSystemEventArgs e) =>
            {
                mre.Set();
            };
            Directory.Delete(directory, true);
            mre.Wait(timeout);
        }
    }
    
    public static string GetLatestVersion()
    {
        // Get the URL
        string url = GetFinalRedirect(gitUrl);
        if (!string.IsNullOrEmpty(url))
        {
            // Parse the Url
            string[] slashSplit = url.Split('/');
            string tag = slashSplit[slashSplit.Length - 1];
            return tag;
        }
        return String.Empty;
    }

    public static string GetInstalledVersion()
    {
        if (IsInstalled)
        {
            string file = Path.Combine(OutputPath, "version.txt");
            if (File.Exists(file))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    string text = sr.ReadToEnd();
                    return text;
                }
            }
            else
                return "unknown";
        }
        else
            return "unknown";
    }
    
    /// <summary>
    /// Method by Marcelo Calbucci and edited by Uwe Keim. 
    /// No changes to this method were made. 
    /// https://stackoverflow.com/a/28424940/12968919
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static string GetFinalRedirect(string url)
    {
        if(string.IsNullOrWhiteSpace(url))
            return url;
        
        int maxRedirCount = 8;  // prevent infinite loops
        string newUrl = url;
        do
        {
            HttpWebRequest req = null;
            HttpWebResponse resp = null;
            try
            {
                req = (HttpWebRequest) HttpWebRequest.Create(url);
                req.Method = "HEAD";
                req.AllowAutoRedirect = false;
                resp = (HttpWebResponse)req.GetResponse();
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return newUrl;
                    case HttpStatusCode.Redirect:
                    case HttpStatusCode.MovedPermanently:
                    case HttpStatusCode.RedirectKeepVerb:
                    case HttpStatusCode.RedirectMethod:
                        newUrl = resp.Headers["Location"];
                        if (newUrl == null)
                            return url;
        
                        if (newUrl.IndexOf("://", System.StringComparison.Ordinal) == -1)
                        {
                            // Doesn't have a URL Schema, meaning it's a relative or absolute URL
                            Uri u = new Uri(new Uri(url), newUrl);
                            newUrl = u.ToString();
                        }
                        break;
                    default:
                        return newUrl;
                }
                url = newUrl;
            }
            catch (WebException)
            {
                // Return the last known good URL
                return newUrl;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (resp != null)
                    resp.Close();
            }
        } 
        while (maxRedirCount-- > 0);
            return newUrl;
    }
}