using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace HRtoVRChatLauncher;

public partial class MainWindow : Window
{
    private readonly string randomImageUrl = "https://hrproxy.fortnite.lol/getRandomImage";
    
    public MainWindow()
    {
        InitializeComponent();
        Install.RequestInfoTextUpdate += s => Dispatcher.UIThread.InvokeAsync(() => ActionText.Text = s);
        Install.RequestProgressBarUpdate += percentage => Dispatcher.UIThread.InvokeAsync(() => ProgressBar.Value = percentage);
        ActionText.Text = "Checking for HRtoVRChat";
        GifVector.Stretch = Stretch.Fill;
        if (!Install.Uninstall)
        {
            Bitmap? b = DownloadRandomImage();
            if (b != null)
                ImageVector.Source = b;
        }
        else
        {
            GifVector.IsVisible = false;
            ImageVector.IsVisible = false;
            SadImageVector.IsVisible = true;
        }
        new Thread(() =>
        {
            Install.Launch(launch =>
            {
                switch (launch)
                {
                    case LaunchCallback.Executed:
                        Install.RequestInfoTextUpdate.Invoke("Launched HRtoVRChat!");
                        Install.RequestProgressBarUpdate.Invoke(100);
                        Thread.Sleep(1000);
                        Environment.Exit(0);
                        break;
                    case LaunchCallback.FileDoesNotExist:
                        Install.RequestInfoTextUpdate.Invoke("HRtoVRChat was not found!");
                        Install.InstallSoftware(() =>
                        {
                            Install.Launch(launchCallback =>
                            {
                                if (launchCallback == LaunchCallback.Exception)
                                    Install.RequestInfoTextUpdate.Invoke("Failed to Launch HRtoVRChat!");
                                Thread.Sleep(1000);
                                Environment.Exit(0);
                            });
                        });
                        break;
                    case LaunchCallback.Exception:
                        Thread.Sleep(1000);
                        Environment.Exit(0);
                        break;
                    case LaunchCallback.Uninstalled:
                        Thread.Sleep(3000);
                        Environment.Exit(0);
                        break;
                }
            });
        }).Start();
    }
    
    public static Bitmap ConvertToAvaloniaBitmap(System.Drawing.Bitmap bitmap)
    {
        if (bitmap == null)
            return null;
        System.Drawing.Bitmap bitmapTmp = new System.Drawing.Bitmap(bitmap);
        var bitmapdata = bitmapTmp.LockBits(new Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        Bitmap bitmap1 = new Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
            bitmapdata.Scan0,
            new Avalonia.PixelSize(bitmapdata.Width, bitmapdata.Height),
            new Avalonia.Vector(96, 96),
            bitmapdata.Stride);
        bitmapTmp.UnlockBits(bitmapdata);
        bitmapTmp.Dispose();
        return bitmap1;
    }

    private Bitmap? DownloadRandomImage()
    {
        try
        {
            foreach (string file in Directory.GetFiles(Install.Cache))
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                string filetype = Path.GetExtension(file);
                if (filename == "o")
                {
                    if (!filetype.Contains(".gif"))
                    {
                        Stream s = File.OpenRead(file);
                        System.Drawing.Bitmap b = new System.Drawing.Bitmap(s);
                        GifVector.IsVisible = false;
                        ImageVector.IsVisible = true;
                        return ConvertToAvaloniaBitmap(b);
                    }
                    else
                    {
                        Stream s = File.OpenRead(file);
                        GifVector.SourceStream = s;
                        GifVector.IsVisible = true;
                        ImageVector.IsVisible = false;
                        return null;
                    }
                }
            }
            bool isGif = false;
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(randomImageUrl);
            if (client.ResponseHeaders["Content-Type"] == "image/gif")
                isGif = true;
            if (!isGif)
            {
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(stream);
                GifVector.IsVisible = false;
                ImageVector.IsVisible = true;
                return ConvertToAvaloniaBitmap(bitmap);
            }
            string outputFile = Path.Combine(Install.Cache, "a.gif");
            FileStream fileStream = File.Create(outputFile);
            stream.CopyTo(fileStream);
            fileStream.Dispose();
            stream = File.OpenRead(outputFile);
            GifVector.SourceStream = stream;
            GifVector.IsVisible = true;
            ImageVector.IsVisible = false;
            return null;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}