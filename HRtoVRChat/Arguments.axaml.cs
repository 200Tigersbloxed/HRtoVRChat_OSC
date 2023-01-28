using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace HRtoVRChat;

public partial class Arguments : Window
{
    public CheckBox autostart;
    public CheckBox skipvrc;
    public CheckBox neosbridge;
    public CheckBox legacybool;
    public TextBox otherargs;
    
    public Arguments()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        foreach (IControl control in ((Canvas) Content).Children)
        {
            switch (control.Name)
            {
                case "AutoStartCheckBox":
                    autostart = (CheckBox) control;
                    break;
                case "SkipVRCCheckBox":
                    skipvrc = (CheckBox) control;
                    break;
                case "NeosBridgeCheckBox":
                    neosbridge = (CheckBox) control;
                    break;
                case "LegacyBoolCheckBox":
                    legacybool = (CheckBox) control;
                    break;
                case "OtherLaunchArgs":
                    otherargs = (TextBox) control;
                    break;
            }
        }
        Closed += (sender, args) =>
        {
            ConfigManager.LoadedUIConfig.OtherArgs = otherargs!.Text;
            ConfigManager.SaveConfig(ConfigManager.LoadedUIConfig);
            TrayIconManager.ArgumentsWindow = new Arguments();
        };
        autostart!.IsChecked = ConfigManager.LoadedUIConfig.AutoStart;
        skipvrc!.IsChecked = ConfigManager.LoadedUIConfig.SkipVRCCheck;
        neosbridge!.IsChecked = ConfigManager.LoadedUIConfig.NeosBridge;
        legacybool!.IsChecked = ConfigManager.LoadedUIConfig.UseLegacyBool;
        otherargs!.Text = ConfigManager.LoadedUIConfig.OtherArgs;
        VerifyCheckBoxes();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public void AutoStartButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        ConfigManager.LoadedUIConfig.AutoStart = autostart.IsChecked ?? false;
        VerifyCheckBoxes(ref autostart, out ConfigManager.LoadedUIConfig.AutoStart);
    }

    public void SkipVRCCheckButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        ConfigManager.LoadedUIConfig.SkipVRCCheck = skipvrc.IsChecked ?? false;
        VerifyCheckBoxes(ref skipvrc, out ConfigManager.LoadedUIConfig.SkipVRCCheck);
    }

    public void NeosBridgeButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        ConfigManager.LoadedUIConfig.NeosBridge = neosbridge.IsChecked ?? false;
        TrayIconManager.Update(new TrayIconManager.UpdateTrayIconInformation
        {
            AutoStart = autostart.IsChecked,
            SkipVRCCheck = skipvrc.IsChecked,
            NeosBridge = neosbridge.IsChecked
        });
        ConfigManager.SaveConfig(ConfigManager.LoadedUIConfig);
    }
    
    public void LegacyBoolButtonPressed(object? sender, RoutedEventArgs routedEventArgs)
    {
        ConfigManager.LoadedUIConfig.UseLegacyBool = legacybool.IsChecked ?? false;
        ConfigManager.SaveConfig(ConfigManager.LoadedUIConfig);
    }
    
    private void VerifyCheckBoxes(ref CheckBox checkBox, out bool config)
    {
        if((skipvrc.IsChecked ?? false) && (autostart.IsChecked ?? false))
        {
            skipvrc.IsChecked = false;
            ConfigManager.LoadedUIConfig.SkipVRCCheck = false;
            autostart.IsChecked = false;
            ConfigManager.LoadedUIConfig.AutoStart = false;
            checkBox.IsChecked = true;
            config = true;
        }
        config = checkBox.IsChecked ?? false;
        TrayIconManager.Update(new TrayIconManager.UpdateTrayIconInformation
        {
            AutoStart = autostart.IsChecked,
            SkipVRCCheck = skipvrc.IsChecked
        });
        ConfigManager.SaveConfig(ConfigManager.LoadedUIConfig);
    }
    
    private void VerifyCheckBoxes()
    {
        if((skipvrc.IsChecked ?? false) && (autostart.IsChecked ?? false))
        {
            skipvrc.IsChecked = false;
            ConfigManager.LoadedUIConfig.SkipVRCCheck = false;
            autostart.IsChecked = false;
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