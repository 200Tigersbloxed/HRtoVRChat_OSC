using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.TextMate;
using HRtoVRChat_OSC_SDK;
using TextMateSharp.Grammars;
using WebViewControl;

namespace HRtoVRChat;

public partial class MainWindow : Window
{
    private readonly Dictionary<ProgramPanels, IControl> WindowPanels = new();
    private string lastLineColor = "White";
    private TextEditor _textEditor;
    private RichTextModel richTextModel = new RichTextModel();

    private Thread SecondaryThread;
    private CancellationTokenSource cancellationTokenSource = new();

    private AppBridge _appBridge;

    private WebView webview;
    private readonly string homeURL = "https://hrtovrchat.fortnite.lol/applatest";
    
    public MainWindow()
    {
        // Set Settings
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            WebView.Settings.PersistCache = false;
            WebView.Settings.LogFile = "ceflog.txt";
        }
        if (!string.IsNullOrEmpty(SoftwareManager.LocalDirectory) && !Directory.Exists(SoftwareManager.LocalDirectory))
            Directory.CreateDirectory(SoftwareManager.LocalDirectory);
        // Init the Window
        InitializeComponent();
        // Set Instances
        TrayIconManager.MainWindow = this;
        // Cache Window Panels
        WindowPanels.Add(ProgramPanels.Home, HomeCanvas);
        WindowPanels.Add(ProgramPanels.Program, ProgramCanvas);
        WindowPanels.Add(ProgramPanels.Updates, UpdatesCanvas);
        WindowPanels.Add(ProgramPanels.Config, ConfigCanvas);
        WindowPanels.Add(ProgramPanels.IncomingData, IncomingDataCanvas);
        // Load the Config Views
        ConfigManager.CreateConfig();
        ConfigManager.InitStackPanels(this);
        AutoStartCheckBox.IsChecked = ConfigManager.LoadedUIConfig.AutoStart;
        SkipVRCCheckBox.IsChecked = ConfigManager.LoadedUIConfig.SkipVRCCheck;
        NeosBridgeCheckBox.IsChecked = ConfigManager.LoadedUIConfig.NeosBridge;
        VerifyCheckBoxes();
        // Subscribe to events
        SoftwareManager.OnConsoleUpdate += (message, overrideColor) =>
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (message != null && _textEditor != null)
                {
                    int currentLength = OutputTextBox.Text?.Length ?? 0;
                    string color = message.Contains("(DEBUG)") ? "DarkGray" :
                        message.Contains("(LOG)") ? "White" :
                        message.Contains("(WARN)") ? "Yellow" :
                        message.Contains("(ERROR)") ? "Red" : lastLineColor;
                    lastLineColor = color;
                    if (!string.IsNullOrEmpty(overrideColor))
                        color = overrideColor;
                    if(currentLength > 0)
                        AppendTextWithColor("\n" + message, Color.Parse(color));
                    else
                        AppendTextWithColor(message, Color.Parse(color));
                    _textEditor.ScrollToEnd();
                }
            });
        };
        SoftwareManager.RequestUpdateProgressBars += (x, y) =>
        {
            TotalProgressBar.Value = x;
            TaskProgressBar.Value = y;
        };
        // Setup the Program Output
        _textEditor = this.FindControl<TextEditor>("OutputTextBox");
        var  _registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        _textEditor.InstallTextMate(_registryOptions);
        _textEditor.TextArea.TextView.LineTransformers.Add(new RichTextColorizer(richTextModel));
        // Update Button Text
        LatestVersionLabel.Text = "Latest Version: " + SoftwareManager.GetLatestVersion();
        InstalledVersionLabel.Text = "Installed Version: " + SoftwareManager.GetInstalledVersion();
        if (!SoftwareManager.IsInstalled)
            UpdateButton.Content = "INSTALL SOFTWARE";
        // Multithreading Tasks
        cancellationTokenSource = new();
        SecondaryThread = new Thread(async () =>
        {
            bool attemptConnect = false;
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    StatusLabel.Content = "STATUS: ";
                    StatusLabel.Content += SoftwareManager.IsSoftwareRunning ? "RUNNING" : "STOPPED";
                        
                    AppBridgeConnectedLabel.Text = "App Bridge Connection Status: ";
                    AppBridgeConnectedLabel.Text +=
                        (_appBridge?.IsClientConnected ?? false) ? "Connected" : "Not Connected";
                        
                    TrayIconManager.Update(new TrayIconManager.UpdateTrayIconInformation
                    {
                        Status = SoftwareManager.IsSoftwareRunning ? "RUNNING" : "STOPPED"
                    });
                });
                if (SoftwareManager.IsSoftwareRunning && !attemptConnect && (_appBridge?.IsClientConnected ?? false) == false)
                {
                    _appBridge = new AppBridge();
                    _appBridge.OnAppBridgeMessage += async message =>
                    {
                        await Dispatcher.UIThread.InvokeAsync(async () =>
                        {
                            string avatarParameters = String.Empty;
                            foreach (string currentAvatarParameter in message.CurrentAvatar?.parameters ?? new List<string>())
                                avatarParameters += currentAvatarParameter + "\n";
                            OutputIncomingData.Text = $"Current Source: {message.CurrentSourceName}\n\n" +
                                                      "-- Parameters --\n" +
                                                      $"onesHR: {message.onesHR}\n" +
                                                      $"tensHR: {message.tensHR}\n" +
                                                      $"hundredsHR: {message.hundredsHR}\n" +
                                                      $"isHRConnected: {message.isHRConnected}\n" +
                                                      $"isHRActive: {message.isHRActive}\n" +
                                                      $"isHRBeat: {message.isHRBeat} (inaccurate over AppBridge)\n" +
                                                      $"HRPercent: {message.HRPercent}\n" +
                                                      $"FullHRPercent: {message.FullHRPercent}\n" +
                                                      $"HR: {message.HR}\n\n" +
                                                      "-- Current Avatar --\n" +
                                                      $"name: {message.CurrentAvatar?.name ?? "unknown"}\n" +
                                                      $"id: {message.CurrentAvatar?.id ?? "unknown"}\n" +
                                                      "== parameters ==\n" +
                                                      $"{avatarParameters}";
                        });
                    };
                    _appBridge.OnClientDisconnect += async () =>
                    {
                        _appBridge.StopClient();
                        await Dispatcher.UIThread.InvokeAsync(async () =>
                        {
                            OutputIncomingData.Clear();
                        });
                    };
                    _appBridge.InitClient();
                    attemptConnect = true;
                }
                else
                    attemptConnect = false;
                Thread.Sleep(10);
            }
        });
        SecondaryThread.Start();
        // Set Tray Icon Text
        if (ConfigManager.LoadedUIConfig.AutoStart)
        {
            ((NativeMenuItem) TrayIconManager.nativeMenuItems["AutoStart"]).Header = "✅ Auto Start";
            ((NativeMenuItem) TrayIconManager.nativeMenuItems["AutoStart"]).IsChecked = true;
        }
        if (ConfigManager.LoadedUIConfig.SkipVRCCheck)
        {
            ((NativeMenuItem) TrayIconManager.nativeMenuItems["SkipVRCCheck"]).Header = "✅ Skip VRChat Check";
            ((NativeMenuItem) TrayIconManager.nativeMenuItems["SkipVRCCheck"]).IsChecked = true;
        }
        // Setup Home WebView
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            webview = new WebView
            {
                Address = homeURL,
                Width = 450,
                Height = 500
            };
            HomeCanvas.Children.Add(webview);
        }
        // Check the SetupWozard
        if (!Config.DoesConfigExist())
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                bool b = await SetupWizard.AskToSetup();
                if (b)
                {
                    Hide();
                    new SetupWizard(() => Show()).Show();
                }
            });
    }
    
    // Left Bar Panel
    
    public void HomeButtonPressed(object? sender, RoutedEventArgs routedEventArgs) =>
        SelectCanvas(ProgramPanels.Home);
    
    public void ProgramButtonPressed(object? sender, RoutedEventArgs routedEventArgs) =>
        SelectCanvas(ProgramPanels.Program);
    
    public void UpdatesButtonPressed(object? sender, RoutedEventArgs routedEventArgs) =>
        SelectCanvas(ProgramPanels.Updates);
    
    public void ConfigButtonPressed(object? sender, RoutedEventArgs routedEventArgs) =>
        SelectCanvas(ProgramPanels.Config);
    
    public void IncomingDataButtonPressed(object? sender, RoutedEventArgs routedEventArgs) =>
        SelectCanvas(ProgramPanels.IncomingData);

    public enum ProgramPanels
    {
        Home,
        Program,
        Updates,
        Config,
        IncomingData
    }

    private void SelectCanvas(ProgramPanels panel)
    {
        // Hide All First
        foreach (var (key, value) in WindowPanels)
            value.IsVisible = false;
        // Then show the Target One
        WindowPanels[panel].IsVisible = true;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && webview != null)
        {
            if(panel == ProgramPanels.Home)
                webview.Reload(true);
        }
    }

    public void HideAppButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        TrayIconManager.Update(new TrayIconManager.UpdateTrayIconInformation{HideApplication = true});
        Hide();
    }
    
    public void ExitAppButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        KillButtonPressed(null, null);
        Environment.Exit(0);
    }
    
    public void GitHubButtonPressed(object? sender, RoutedEventArgs routedEventArgs) =>
        OpenBrowser("https://github.com/200Tigersbloxed/HRtoVRChat_OSC");
    
    // Home Panel

    public void HereButtonPressed(object? sender, RoutedEventArgs routedEventArgs) =>
        OpenBrowser(homeURL);
    
    // Program Panel

    public void StartButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        _textEditor.Clear();
        SoftwareManager.OnConsoleUpdate(
            $"HRtoVRChat_OSC {SoftwareManager.GetInstalledVersion()} Created by 200Tigersbloxed\n", String.Empty);
        SoftwareManager.StartSoftware();
    }

    public void StopButtonPressed(object? sender, RoutedEventArgs routedEventArgs) => SoftwareManager.StopSoftware();
    
    public void KillButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        SoftwareManager.StopSoftware();
        try
        {
            foreach (Process process in Process.GetProcessesByName("HRtoVRChat_OSC"))
            {
                process.Kill();
            }
        }
        catch(Exception){}
    }

    public void AutoStartButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        ConfigManager.LoadedUIConfig.AutoStart = AutoStartCheckBox.IsChecked ?? false;
        VerifyCheckBoxes(ref AutoStartCheckBox, out ConfigManager.LoadedUIConfig.AutoStart);
    }

    public void SkipVRCCheckButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        ConfigManager.LoadedUIConfig.SkipVRCCheck = SkipVRCCheckBox.IsChecked ?? false;
        VerifyCheckBoxes(ref SkipVRCCheckBox, out ConfigManager.LoadedUIConfig.SkipVRCCheck);
    }

    public void NeosBridgeButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        ConfigManager.LoadedUIConfig.NeosBridge = NeosBridgeCheckBox.IsChecked ?? false;
        TrayIconManager.Update(new TrayIconManager.UpdateTrayIconInformation
        {
            AutoStart = AutoStartCheckBox.IsChecked,
            SkipVRCCheck = SkipVRCCheckBox.IsChecked,
            NeosBridge = NeosBridgeCheckBox.IsChecked
        });
        ConfigManager.SaveConfig(ConfigManager.LoadedUIConfig);
    }
    
    public void SendButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        SoftwareManager.SendCommand(CommandBox.Text);
        CommandBox.Clear();
    }

    // Config Panel

    public void SaveConfigButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        if (!string.IsNullOrEmpty(OnConfigRadioButtonPressed.SelectedConfigValue))
        {
            FieldInfo targetField = ConfigManager.LoadedConfig.GetType()
                .GetField(OnConfigRadioButtonPressed.SelectedConfigValue);
            if (targetField != null)
            {
                if(targetField.Name == "ParameterNames")
                    return;
                targetField.SetValue(ConfigManager.LoadedConfig,
                    Convert.ChangeType(ConfigValue.Text, targetField.FieldType));
                    ConfigManager.SaveConfig(ConfigManager.LoadedConfig);
            }
        }
    }
    
    // Update Software Portion

    public async void UpdateSoftwareButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        await SoftwareManager.InstallSoftware(() =>
        {
            UpdateButton.Content = "UPDATE SOFTWARE";
        });
    }

    public void UpdatesRefreshButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        LatestVersionLabel.Text = "Latest Version: " + SoftwareManager.GetLatestVersion();
        InstalledVersionLabel.Text = "Installed Version: " + SoftwareManager.GetInstalledVersion();
    }
    
    // Tools
    
    private void OpenBrowser(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }
        else
            try
            {
                Process.Start("https://github.com/200Tigersbloxed/HRtoVRChat_OSC");
            }
            catch(Exception){}
    }
    
    // HUGE thanks to this post
    // https://github.com/icsharpcode/AvalonEdit/issues/244#issuecomment-725214919
    private void AppendTextWithColor(string text, Color color)
    {
        _textEditor.AppendText(text);
        richTextModel.ApplyHighlighting(_textEditor.Text.Length - text.Length, text.Length, new HighlightingColor() { Foreground = new SimpleHighlightingBrush(color)});
    }

    private void VerifyCheckBoxes(ref CheckBox checkBox, out bool config)
    {
        if((SkipVRCCheckBox.IsChecked ?? false) && (AutoStartCheckBox.IsChecked ?? false))
        {
            SkipVRCCheckBox.IsChecked = false;
            ConfigManager.LoadedUIConfig.SkipVRCCheck = false;
            AutoStartCheckBox.IsChecked = false;
            ConfigManager.LoadedUIConfig.AutoStart = false;
            checkBox.IsChecked = true;
            config = true;
        }
        config = checkBox.IsChecked ?? false;
        TrayIconManager.Update(new TrayIconManager.UpdateTrayIconInformation
        {
            AutoStart = AutoStartCheckBox.IsChecked,
            SkipVRCCheck = SkipVRCCheckBox.IsChecked
        });
        ConfigManager.SaveConfig(ConfigManager.LoadedUIConfig);
    }
    
    private void VerifyCheckBoxes()
    {
        if((SkipVRCCheckBox.IsChecked ?? false) && (AutoStartCheckBox.IsChecked ?? false))
        {
            SkipVRCCheckBox.IsChecked = false;
            ConfigManager.LoadedUIConfig.SkipVRCCheck = false;
            AutoStartCheckBox.IsChecked = false;
            ConfigManager.LoadedUIConfig.AutoStart = false;
            TrayIconManager.Update(new TrayIconManager.UpdateTrayIconInformation
            {
                AutoStart = false,
                SkipVRCCheck = false
            });
        }
        ConfigManager.SaveConfig(ConfigManager.LoadedUIConfig);
    }
}