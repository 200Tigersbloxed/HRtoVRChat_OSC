using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace HRtoVRChat;

public static class TrayIconManager
{
    public static MainWindow? MainWindow;
    
    public static Dictionary<string, NativeMenuItemBase> nativeMenuItems = new Dictionary<string, NativeMenuItemBase>
    {
        ["Status"] = new NativeMenuItem
        {
            Header = "Status: STOPPED",
            ToggleType = NativeMenuItemToggleType.None
        },
        ["-1"] = new NativeMenuItemSeparator(),
        ["AutoStart"] = new NativeMenuItem
        {
            Header = "Auto Start",
            ToggleType = NativeMenuItemToggleType.CheckBox,
            Command = new TrayIconClicked("AutoStart", "Auto Start")
        },
        ["SkipVRCCheck"] = new NativeMenuItem
        {
            Header = "Skip VRChat Check",
            ToggleType = NativeMenuItemToggleType.CheckBox,
            Command = new TrayIconClicked("SkipVRCCheck", "Skip VRChat Check")
        },
        ["NeosBridge"] = new NativeMenuItem
        {
            Header = "NeosBridge",
            ToggleType = NativeMenuItemToggleType.CheckBox,
            Command = new TrayIconClicked("NeosBridge", "Neos Bridge")
        },
        ["-2"] = new NativeMenuItemSeparator(),
        ["Start"] = new NativeMenuItem
        {
            Header = "Start",
            ToggleType = NativeMenuItemToggleType.None,
            Command = new TrayIconClicked("Start", "Start")
        },
        ["Stop"] = new NativeMenuItem
        {
            Header = "Stop",
            ToggleType = NativeMenuItemToggleType.None,
            Command = new TrayIconClicked("Stop", "Stop")
        },
        ["Kill"] = new NativeMenuItem
        {
            Header = "Kill all Processes",
            ToggleType = NativeMenuItemToggleType.None,
            Command = new TrayIconClicked("Kill", "Kill all Processes")
        },
        ["-3"] = new NativeMenuItemSeparator(),
        ["HideApplication"] = new NativeMenuItem
        {
            Header = "Hide Application",
            ToggleType = NativeMenuItemToggleType.CheckBox,
            Command = new TrayIconClicked("HideApplication", "Hide Application")
        },
        ["Exit"] = new NativeMenuItem
        {
            Header = "Exit",
            ToggleType = NativeMenuItemToggleType.None,
            Command = new TrayIconClicked("Exit", "Exit")
        }
    };

    public static void Init(AvaloniaObject o)
    {
        NativeMenu nm = new NativeMenu();
        foreach (var (key, value) in nativeMenuItems)
            nm.Add(value);
        TrayIcon trayIcon = new TrayIcon
        {
            Icon = new WindowIcon(AssetTools.Icon),
            ToolTipText = "HRtoVRChat",
            Menu = nm
        };
        TrayIcons ti = new TrayIcons();
        ti.Add(trayIcon);
        TrayIcon.SetIcons(o, ti);
    }

    public static void Update(UpdateTrayIconInformation information)
    {
        foreach (KeyValuePair<string,NativeMenuItemBase> keyValuePair in nativeMenuItems)
        {
            if (!keyValuePair.Key.Contains('-'))
            {
                NativeMenuItem nativeMenuItem = (NativeMenuItem) keyValuePair.Value;
                switch (keyValuePair.Key)
                {
                    case "Status":
                        if(!string.IsNullOrEmpty(information.Status))
                            nativeMenuItem.Header = "Status: " + information.Status;
                        break;
                    case "AutoStart":
                        if (information.AutoStart != null)
                        {
                            NativeMenuItem as_nmi = (NativeMenuItem) nativeMenuItems["AutoStart"];
                            as_nmi.Header =
                                information.AutoStart ?? false ? "✅ Auto Start" : "Auto Start";
                            as_nmi.IsChecked = information.AutoStart ?? false;
                        }
                        break;
                    case "SkipVRCCheck":
                        if (information.SkipVRCCheck != null)
                        {
                            NativeMenuItem svc_nmi = (NativeMenuItem) nativeMenuItems["SkipVRCCheck"];
                            svc_nmi.Header =
                                information.SkipVRCCheck ?? false ? "✅ Skip VRChat Check" : "Skip VRChat Check";
                            svc_nmi.IsChecked = information.SkipVRCCheck ?? false;
                        }
                        break;
                    case "NeosBridge":
                        if (information.NeosBridge != null)
                        {
                            NativeMenuItem svc_nmi = (NativeMenuItem) nativeMenuItems["NeosBridge"];
                            svc_nmi.Header =
                                information.NeosBridge ?? false ? "✅ Neos Bridge" : "Neos Bridge";
                            svc_nmi.IsChecked = information.NeosBridge ?? false;
                        }
                        break;
                    case "HideApplication":
                        if (information.HideApplication != null)
                        {
                            NativeMenuItem ha_nmi = (NativeMenuItem) nativeMenuItems["HideApplication"];
                            ha_nmi.Header =
                                information.HideApplication ?? false ? "✅ Hide Application" : "Hide Application";
                            ha_nmi.IsChecked = information.HideApplication ?? false;
                        }
                        break;
                }
            }
        }
    }

    public class UpdateTrayIconInformation
    {
        public string Status = String.Empty;
        public bool? AutoStart;
        public bool? SkipVRCCheck;
        public bool? HideApplication;
        public bool? NeosBridge;
    }

    private class TrayIconClicked : ICommand
    {
        private string id;
        private string cachedHeader;
        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            NativeMenuItem nmi = (NativeMenuItem) nativeMenuItems[id];
            if (nmi.ToggleType == NativeMenuItemToggleType.CheckBox)
            {
                nmi.IsChecked = !nmi.IsChecked;
                if (nmi.IsChecked)
                    nmi.Header = "✅ " + cachedHeader;
                else
                    nmi.Header = cachedHeader;
            }
            if (MainWindow != null)
            {
                switch (id)
                {
                    case "AutoStart":
                        MainWindow.AutoStartCheckBox.IsChecked = nmi.IsChecked;
                        MainWindow.AutoStartButtonPressed(null, null);
                        break;
                    case "SkipVRCCheck":
                        MainWindow.SkipVRCCheckBox.IsChecked = nmi.IsChecked;
                        MainWindow.SkipVRCCheckButtonPressed(null, null);
                        break;
                    case "NeosBridge":
                        MainWindow.NeosBridgeCheckBox.IsChecked = nmi.IsChecked;
                        MainWindow.NeosBridgeButtonPressed(null, null);
                        break;
                    case "Start":
                        MainWindow.StartButtonPressed(null, null);
                        break;
                    case "Stop":
                        MainWindow.StopButtonPressed(null, null);
                        break;
                    case "Kill":
                        MainWindow.KillButtonPressed(null, null);
                        break;
                    case "HideApplication":
                        if(nmi.IsChecked)
                            MainWindow.Hide();
                        else
                            MainWindow.Show();
                        break;
                    case "Exit":
                        MainWindow.KillButtonPressed(null, null);
                        Environment.Exit(0);
                        break;
                }
            }
        }

        public event EventHandler? CanExecuteChanged = (sender, args) => {};
        public TrayIconClicked(string id, string cachedHeader)
        {
            this.id = id;
            this.cachedHeader = cachedHeader;
        }
    }
}