using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace HRtoVRChat
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            // Cache any assets
            AssetTools.Init();
            // Continue
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            TrayIconManager.Init(this);

            base.OnFrameworkInitializationCompleted();
        }

        
    }
}