using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace HRtoVRChat;

public partial class SetupWizard : Window
{
    public SetupWizard()
    {
        InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
    }

    private Action onDone;
    public SetupWizard(Action onDone)
    {
        this.onDone = onDone;
        InitializeComponent();
    }

    private List<Canvas> Windows = new();

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        Closed += (sender, args) =>
        {
            if (onDone != null)
                onDone.Invoke();
        };
        ConfigManager.CreateConfig();
        Prepare1();
        Prepare2();
    }

    public static async Task<bool> AskToSetup()
    {
        var br = await MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
        {
            Icon = MessageBox.Avalonia.Enums.Icon.Question,
            WindowIcon = new WindowIcon(AssetTools.Icon),
            ContentTitle = "HRtoVRChat",
            ContentHeader = "No Config was Found!",
            ContentMessage = "Run the SetupWizard?",
            ButtonDefinitions = ButtonEnum.YesNo
        }).Show();
        if ((br & ButtonResult.Yes) != 0)
            return true;
        return false;
    }
    
    private readonly Dictionary<string, HRTypeSelector> hrtypes = new ()
    {
        ["fhr"] = new HRTypeSelector("fitbithrtows")
        {
            ExtraInfos = new()
            {
                new HRTypeExtraInfo("fitbitURL", "The WebSocket to listen to data", "ws://localhost:8080/", typeof(string))
            }
        },
        ["hrp"] = new HRTypeSelector("hrproxy")
        {
            ExtraInfos = new()
            {
                new HRTypeExtraInfo("hrproxyId", "The code to pull HRProxy Data from", "ABCD", typeof(string))
            }
        },
        ["hr"] = new HRTypeSelector("hyperate")
        {
            ExtraInfos = new()
            {
                new HRTypeExtraInfo("hyperateSessionId", "The code to pull HypeRate Data from", "ABCD", typeof(string))
            }
        },
        ["ps"] = new HRTypeSelector("pulsoid")
        {
            ExtraInfos = new()
            {
                new HRTypeExtraInfo("pulsoidwidget", "The widgetId to pull HeartRate Data from", "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", typeof(string))
            }
        },
        ["pss"] = new HRTypeSelector("pulsoidsocket")
        {
            ExtraInfos = new()
            {
                new HRTypeExtraInfo("pulsoidkey", "The key for the OAuth API to pull HeartRate Data from", "https://github.com/200Tigersbloxed/HRtoVRChat_OSC/wiki/Upgrading-from-Pulsoid-to-PulsoidSocket", typeof(string))
            }
        },
        ["sn"] = new HRTypeSelector("stromno")
        {
            ExtraInfos = new()
            {
                new HRTypeExtraInfo("stromnowidget", "The widgetId to pull HeartRate Data from Stromno", "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", typeof(string))
            }
        },
        ["tf"] = new HRTypeSelector("textfile")
        {
            ExtraInfos = new()
            {
                new HRTypeExtraInfo("textfilelocation", "The location of the text file to pull HeartRate Data from", "/desktop/file.txt", typeof(string))
            }
        },
        ["oc"] = new HRTypeSelector("omnicept"),
        ["sdk"] = new HRTypeSelector("sdk")
    };

    private void MoveNext(int current)
    {
        Windows[current].IsVisible = false;
        if (Windows.Count > current + 1)
            Windows[current + 1].IsVisible = true;
        else
            Done();
    }

    private void GoBack(int current)
    {
        Windows[current].IsVisible = false;
        if (current - 1 >= 0)
            Windows[current - 1].IsVisible = true;
    }

    private void Prepare1()
    {
        Canvas currentCanvas = this.FindControl<Canvas>("hrtypepanel");
        Windows.Add(currentCanvas);
        List<RadioButton> rbs = new();
        RadioButton selected = null;
        StackPanel sp1 = this.FindControl<StackPanel>("sp11");
        StackPanel sp2 = this.FindControl<StackPanel>("sp12");
        foreach (IControl sp1Child in sp1.Children)
        {
            RadioButton rb = (RadioButton) sp1Child;
            rb.Checked += (sender, args) => selected = rb;
            rbs.Add(rb);
        }
        foreach (IControl sp2Child in sp2.Children)
        {
            RadioButton rb = (RadioButton) sp2Child;
            rb.Checked += (sender, args) => selected = rb;
            rbs.Add(rb);
        }
        this.FindControl<Button>("continue1").Command = new OnButtonClick(() =>
        {
            if (selected != null)
            {
                if(hrtypes.ContainsKey(selected.Name))
                    hrtypes[selected.Name].ShowExtra((vals) =>
                    {
                        ConfigManager.LoadedConfig.hrType = hrtypes[selected.Name].Name;
                        foreach (HRTypeExtraInfo hrTypeExtraInfo in vals)
                        {
                            ConfigManager.LoadedConfig.GetType().GetField(hrTypeExtraInfo.name).SetValue(
                                ConfigManager.LoadedConfig,
                                Convert.ChangeType(hrTypeExtraInfo.AppliedValue, hrTypeExtraInfo.to));
                        }
                        MoveNext(0);
                    });
                else
                    MoveNext(0);
            }
        });
    }

    private void Prepare2()
    {
        Canvas currentCanvas = this.FindControl<Canvas>("endpointpanel");
        Windows.Add(currentCanvas);
        Canvas se = this.FindControl<Canvas>("showextra2");
        bool ad = false;
        this.FindControl<RadioButton>("thisdevice").Click += (sender, args) =>
        {
            ad = false;
            se.IsVisible = false;
        };
        this.FindControl<RadioButton>("anotherdevice").Click += (sender, args) =>
        {
            ad = true;
            se.IsVisible = true;
        };
        this.FindControl<Button>("continue2").Command = new OnButtonClick(() =>
        {
            if (ad)
            {
                TextBox ipb = this.FindControl<TextBox>("ip");
                TextBox spb = this.FindControl<TextBox>("sendport");
                TextBox lpb = this.FindControl<TextBox>("listenport");
                ConfigManager.LoadedConfig.ip = ipb.Text;
                ConfigManager.LoadedConfig.port = Convert.ToInt32(spb.Text);
                ConfigManager.LoadedConfig.receiverPort = Convert.ToInt32(lpb.Text);
            }
            MoveNext(1);
        });
        this.FindControl<Button>("back2").Command = new OnButtonClick(() => GoBack(1));
    }

    private void Done()
    {
        ConfigManager.SaveConfig(ConfigManager.LoadedConfig);
        if(onDone != null)
            onDone.Invoke();
        MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
        {
            ButtonDefinitions = ButtonEnum.Ok,
            WindowIcon = new WindowIcon(AssetTools.Icon),
            Icon = MessageBox.Avalonia.Enums.Icon.Info,
            ContentTitle = "HRtoVRChat",
            ContentHeader = "Completed SetupWizard!",
            ContentMessage = "You can always visit the Config tab to change any of these settings again."
        }).Show();
        Close();
    }

    private record HRTypeExtraInfo(string name, string description, string example, Type to)
    {
        public string AppliedValue { get; set; }
    }

    private record HRTypeSelector(string Name)
    {
        public List<HRTypeExtraInfo> ExtraInfos { get; init; } = new();

        public void ShowExtra(Action<List<HRTypeExtraInfo>> onDone)
        {
            if (ExtraInfos.Count <= 0)
            {
                onDone.Invoke(new());
                return;
            }
            Window newWindow = new Window
            {
                Width = 500,
                Height = 500,
                MaxWidth = 500,
                MaxHeight = 500,
                MinWidth = 500,
                MinHeight = 500,
                CanResize = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Icon = new WindowIcon(AssetTools.Icon),
                Title = Name + " Extra"
            };
            Canvas canvas = new Canvas();
            StackPanel sp = new StackPanel();
            canvas.Children.Add(sp);
            sp.SetValue(Canvas.TopProperty, 5);
            sp.SetValue(Canvas.LeftProperty, 5);
            Dictionary<HRTypeExtraInfo, TextBox> texts = new();
            foreach (HRTypeExtraInfo hrTypeExtraInfo in ExtraInfos)
            {
                Label title = new Label
                {
                    Content = hrTypeExtraInfo.name,
                    FontSize = 16
                };
                Label desc = new Label
                {
                    Content = hrTypeExtraInfo.description
                };
                TextBox tb = new TextBox
                {
                    Watermark = "example: " + hrTypeExtraInfo.example,
                    Width = 490
                };
                texts.Add(hrTypeExtraInfo, tb);
                sp.Children.Add(title);
                sp.Children.Add(desc);
                sp.Children.Add(tb);
            }
            Button doneButton = new Button
            {
                Content = "DONE",
                Command = new OnButtonClick(() =>
                {
                    List <HRTypeExtraInfo> ret = new();
                    foreach (KeyValuePair<HRTypeExtraInfo,TextBox> keyValuePair in texts)
                    {
                        keyValuePair.Key.AppliedValue = keyValuePair.Value.Text;
                        ret.Add(keyValuePair.Key);
                    }
                    onDone.Invoke(ret);
                    newWindow.Close();
                })
            };
            canvas.Children.Add(doneButton);
            doneButton.SetValue(Canvas.TopProperty, 465);
            doneButton.SetValue(Canvas.LeftProperty, 5);
            newWindow.Content = canvas;
            newWindow.Show();
        }
    }
    
    private class OnButtonClick : ICommand
    {
        public bool CanExecute(object? parameter) => true;

        private Action instance;
        public void Execute(object? parameter)
        {
            instance.Invoke();
        }

        public event EventHandler? CanExecuteChanged = (sender, args) => { };
        public OnButtonClick(Action wtd) => instance = wtd;
    }
}