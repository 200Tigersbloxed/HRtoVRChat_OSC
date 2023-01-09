using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;

namespace HRtoVRChatLauncher;

public static class Install
{
    public static bool Uninstall { get; private set; }
    
    public static Action<string> RequestInfoTextUpdate = s => { };
    public static Action<int> RequestProgressBarUpdate = percentage => { };
    public static string gitUrl => "https://github.com/200Tigersbloxed/HRtoVRChat_OSC/releases/latest";

    public static string latestDownload
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return
                    "https://github.com/200Tigersbloxed/HRtoVRChat_OSC/releases/latest/download/HRtoVRChat_win-x64.zip";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return
                    "https://github.com/200Tigersbloxed/HRtoVRChat_OSC/releases/latest/download/HRtoVRChat_osx-x64.zip";
            return
                "https://github.com/200Tigersbloxed/HRtoVRChat_OSC/releases/latest/download/HRtoVRChat_linux-x64.zip";
        }
    }
    
    public static string Cache { get; private set; }
    public static string Location { get; private set; }
    public static string ExecutableName
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "HRtoVRChat.exe";
            return "HRtoVRChat";
        }
    }
    
    public static void Init(string[]? args = null)
    {
        if (args != null)
        {
            int i = 0;
            foreach (string s in args)
            {
                if (s.Contains("-uninstall"))
                    Uninstall = true;
                if (s.Contains("-path"))
                {
                    string p = args[i + 1];
                    if (!string.IsNullOrEmpty(p))
                        Cache = p;
                }
                i++;
            }
        }
        string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (string.IsNullOrEmpty(Cache))
        {
            string hrip = Path.Combine(appdata, "HRtoVRChatLauncher");
            if (!Directory.Exists(hrip))
                Directory.CreateDirectory(hrip);
            Cache = hrip;
        }
        string hrp = Path.Combine(Cache, "HRtoVRChat");
        if (!Directory.Exists(hrp))
            Directory.CreateDirectory(hrp);
        Location = hrp;
    }

    public static void Launch(Action<LaunchCallback> callback)
    {
        if (Uninstall)
        {
            DeleteSoftware();
            callback.Invoke(LaunchCallback.Uninstalled);
            return;
        }
        string file;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            file = Path.Combine(Location, "HRtoVRChat.exe");
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            file = Path.Combine(Location, "HRtoVRChat.app", "Contents", "MacOS", "HRtoVRChat");
        else
            file = Path.Combine(Location, "HRtoVRChat");
        if (!File.Exists(file))
        {
            callback.Invoke(LaunchCallback.FileDoesNotExist);
            return;
        }
        if (GetInstalledVersion() != GetLatestVersion())
        {
            callback.Invoke(LaunchCallback.FileDoesNotExist);
            return;
        }
        try
        {
            ProcessStartInfo processStartInfo = new()
            {
                FileName = ExecutableName,
                WorkingDirectory = Location,
                UseShellExecute = true
            };
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Make sure the file is executable first
                Process chmodProcess = new Process
                {
                    StartInfo = new ProcessStartInfo("chmod", $"+x {file}")
                    {
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };
                chmodProcess.Exited += (sender, args) =>
                {
                    Process.Start(processStartInfo);
                };
                chmodProcess.Start();
            }
            else
                Process.Start(processStartInfo);
            callback.Invoke(LaunchCallback.Executed);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            callback.Invoke(LaunchCallback.Exception);
        }
    }
    
    private static string[] ExcludeFilesOnDelete =
    {
        "uiconfig.cfg"
    };

    private static string[] ExcludeDirectoriesOnDelete =
    {
        "HRtoVRChat_OSC"
    };

    private static bool IsUpdating;

    private static void UpdateProgressBar(int x) => RequestProgressBarUpdate.Invoke(x);
    
    public static void InstallSoftware(Action? callback = null)
    {
        if (!IsUpdating)
        {
            UpdateProgressBar(0);
            RequestInfoTextUpdate.Invoke("Deleting old files...");
            // Check if there's any files in the Directory
            IsUpdating = true;
            if (Directory.GetFiles(Location).Length > 0)
            {
                // Delete all files
                foreach (string file in Directory.GetFiles(Location))
                {
                    UpdateProgressBar(0);
                    if (!ExcludeFilesOnDelete.Any(s => s.Contains(Path.GetFileName(file))))
                    {
                        RequestInfoTextUpdate.Invoke("Deleting file " + Path.GetFileName(file) + "...");
                        DeleteFileAndWait(file);
                    }
                    UpdateProgressBar(100);
                }
                // Delete all directories
                foreach (string directory in Directory.GetDirectories(Location))
                {
                    // TODO: support forward slash
                    string[] dna = directory.Split('\\');
                    string dn = dna[^1];
                    UpdateProgressBar(0);
                    // why am i only allowed to use Contains on two separate array variables only once????
                    if (!ExcludeDirectoriesOnDelete.Contains(dn))
                    {
                        RequestInfoTextUpdate.Invoke("Deleting Directory " + dn + "...");
                        DeleteDirectoryAndWait(directory);
                    }
                    UpdateProgressBar(100);
                }
            }
            RequestInfoTextUpdate.Invoke("Downloading latest version...");
            UpdateProgressBar(0);
            // Download the file
            using (WebClient client = new WebClient())
            {
                string outputFile = Path.Combine(Location, "HRtoVRChat.zip");
                client.DownloadFileCompleted += (sender, args) =>
                {
                    RequestInfoTextUpdate.Invoke("Extracting new files...");
                    UpdateProgressBar(50);
                    // Extract the Zip
                    ZipFile.ExtractToDirectory(outputFile, Location);
                    UpdateProgressBar(100);
                    RequestInfoTextUpdate.Invoke("Creating version cache...");
                    // Create version.txt
                    UpdateProgressBar(0);
                    File.WriteAllText(Path.Combine(Location, "version.txt"), GetLatestVersion());
                    UpdateProgressBar(100);
                    RequestInfoTextUpdate.Invoke("Done!");
                    IsUpdating = false;
                    callback?.Invoke();
                };
                client.DownloadProgressChanged += (sender, args) =>
                {
                    RequestInfoTextUpdate.Invoke("Downloading latest version... (" + args.ProgressPercentage +
                                                 "%)");
                    UpdateProgressBar(args.ProgressPercentage);
                };
                client.DownloadFileAsync(new Uri(latestDownload), outputFile);
            }
        }
    }

    private static void DeleteSoftware(Action? onDone = null)
    {
        RequestInfoTextUpdate.Invoke("Removing HRtoVRChat...");
        UpdateProgressBar(0);
        string d = Path.GetFullPath(Cache);
        if (Directory.Exists(d))
            DeleteDirectoryAndWait(d);
        UpdateProgressBar(100);
        RequestInfoTextUpdate.Invoke("Removed HRtoVRChat");
        onDone?.Invoke();
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
        if (File.Exists(Path.Combine(Location, "version.txt")))
        {
            string file = Path.Combine(Location, "version.txt");
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
}

public enum LaunchCallback
{
    FileDoesNotExist,
    Exception,
    Executed,
    Uninstalled
}