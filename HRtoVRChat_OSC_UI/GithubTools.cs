using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HRtoVRChat_OSC_UI
{
    public static class GithubTools
    {
        public static readonly string gitUrl = "https://github.com/200Tigersbloxed/HRtoVRChat_OSC/releases/latest";

        public static readonly string latestDownload =
            "https://github.com/200Tigersbloxed/HRtoVRChat_OSC/releases/latest/download/HRtoVRChat_OSC.zip";

        public static readonly string outputPath = "HRtoVRChat_OSC";
        
        public static string Executable => Path.Combine(outputPath, "HRtoVRChat_OSC.exe");
        public static bool IsHRtoVRChat_OSCPresent => File.Exists(Executable);

        private static string[] ExcludeFilesOnDelete =
        {
            "config.cfg"
        };
        
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

        public static void DownloadLatestVersion(Action callback = null)
        {
            if (Process.GetProcessesByName("HRtoVRChat_OSC").Length <= 0)
            {
                UpdateProgressBars(0, 0);
                // Make sure the Directory Exists
                if (!Directory.Exists(outputPath))
                    Directory.CreateDirectory(outputPath);
                // Check if there's any files in the Directory
                if (Directory.GetFiles(outputPath).Length > 0)
                {
                    DialogResult dr =
                        MessageBox.Show(
                            "All files in " + Path.GetFullPath(outputPath) + " are going to be deleted! Are you sure?",
                            "HRtoVRChat_OSC_UI", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (dr == DialogResult.Yes)
                    {
                        // Delete all files
                        foreach (string file in Directory.GetFiles(outputPath))
                        {
                            if(!ExcludeFilesOnDelete.Contains(Path.GetFileName(file)))
                                File.Delete(file);
                        }
                    }
                    else
                        return;
                }

                UpdateProgressBars(0, 25);
                // Download the file
                using (WebClient client = new WebClient())
                {
                    string outputFile = Path.Combine(outputPath, "HRtoVRChat_OSC.zip");
                    client.DownloadFileCompleted += (sender, args) =>
                    {
                        UpdateProgressBars(0, 50);
                        // Extract the Zip
                        ZipFile.ExtractToDirectory(outputFile, outputPath);
                        UpdateProgressBars(100, 50);
                        // Create version.txt
                        UpdateProgressBars(0, 75);
                        File.WriteAllText(Path.Combine(outputPath, "version.txt"), GetLatestVersion());
                        UpdateProgressBars(100, 100);
                        callback?.Invoke();
                    };
                    client.DownloadProgressChanged += (sender, args) => UpdateProgressBars(args.ProgressPercentage, 25);
                    client.DownloadFileAsync(new Uri(latestDownload), outputFile);
                }
            }
            else
                MessageBox.Show(
                    "There's an instance of HRtoVRChat_OSC running, please close out of it before continuing!",
                    "HRtoVRChat_OSC_UI", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void UpdateProgressBars(int process, int overall)
        {
            MainForm.overallProgressChanged.Invoke(overall);
            MainForm.processProgressChanged.Invoke(process);
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
                } while (maxRedirCount-- > 0);
        
                return newUrl;
            }
    }
}