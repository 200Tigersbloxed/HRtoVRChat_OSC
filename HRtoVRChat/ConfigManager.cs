using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Avalonia.Controls;
using Tommy.Serializer;

namespace HRtoVRChat
{
    public class ConfigManager
    {
        public static Config LoadedConfig { get; private set; }
        public static UIConfig LoadedUIConfig { get; private set; }
        public static readonly string ConfigLocation = Path.Combine(SoftwareManager.OutputPath, "config.cfg");
        public static string UIConfigLocation
        {
            get
            {
                if(!string.IsNullOrEmpty(SoftwareManager.LocalDirectory))
                    return SoftwareManager.LocalDirectory + "/" + "uiconfig.cfg";
                return "uiconfig.cfg";
            }
        }
        
        public static void CreateConfig()
        {
            if (Directory.Exists(SoftwareManager.OutputPath) && File.Exists(ConfigLocation))
            {
                // Load
                Config nc = TommySerializer.FromTomlFile<Config>(ConfigLocation) ?? new Config();
                //SaveConfig(nc);
                LoadedConfig = nc;
            }
            else
                LoadedConfig = new Config();

            if (File.Exists(UIConfigLocation))
            {
                // Load
                UIConfig nuic = TommySerializer.FromTomlFile<UIConfig>(UIConfigLocation) ?? new UIConfig();
                //SaveConfig(nc);
                LoadedUIConfig = nuic;
            }
            else
                LoadedUIConfig = new UIConfig();
        }

        public static void SaveConfig(Config config)
        {
            if (!Directory.Exists(Path.GetDirectoryName(ConfigLocation)))
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigLocation));
            TommySerializer.ToTomlFile(config, ConfigLocation);
        }
        public static void SaveConfig(UIConfig uiConfig) => TommySerializer.ToTomlFile(uiConfig, UIConfigLocation);
        
        private static RadioButton NewRadioButton(MainWindow instance, string name) => new RadioButton
        {
            Content = name,
            Command = new OnConfigRadioButtonPressed(instance, name),
            GroupName = "ConfigValues"
        };

        public static void InitStackPanels(MainWindow instance)
        {
            List<string> ConfigValues = new();
            foreach (FieldInfo fieldInfo in new Config().GetType().GetFields())
                ConfigValues.Add(fieldInfo.Name);
            List<RadioButton> LeftButtonsCreated = new();
            List<RadioButton> RightButtonsCreated = new();
            if (ConfigValues.Count % 2 == 0)
            {
                int i = 0;
                foreach (string configValue in ConfigValues)
                {
                    RadioButton newConfigButton = NewRadioButton(instance, configValue);
                    if(i < ConfigValues.Count / 2)
                        LeftButtonsCreated.Add(newConfigButton);
                    else
                        RightButtonsCreated.Add(newConfigButton);
                    i++;
                }
            }
            else
            {
                // Treat it as it's even, but it's not
                int fakeEvenTotal = ConfigValues.Count - 1;
                int i = 0;
                while (i < fakeEvenTotal)
                {
                    RadioButton newConfigButton = NewRadioButton(instance, ConfigValues[i]);
                    if(i < fakeEvenTotal / 2)
                        LeftButtonsCreated.Add(newConfigButton);
                    else
                        RightButtonsCreated.Add(newConfigButton);
                    i++;
                }
                // Add the final one
                RadioButton newConfigButtonOdd = NewRadioButton(instance, ConfigValues[i]);
                RightButtonsCreated.Add(newConfigButtonOdd);
            }
            // Remove Incompatible Config Types
            // L BOZO ENUMERATION WAS MODIFIED
            int side = -1;
            bool didError = true;
            bool completedLeft = false;
            bool completedRight = false;
            while (didError)
            {
                try
                {
                    if(!completedLeft)
                        foreach (RadioButton radioButton in LeftButtonsCreated)
                        {
                            if (((string) radioButton.Content).Contains("ParameterNames"))
                            {
                                LeftButtonsCreated.Remove(radioButton);
                                side = 0;
                            }
                            completedLeft = true;
                        }
                    if(!completedRight)
                        foreach (RadioButton radioButton in RightButtonsCreated)
                        {
                            if (((string) radioButton.Content).Contains("ParameterNames"))
                            {
                                RightButtonsCreated.Remove(radioButton);
                                side = 1;
                            }
                            completedRight = true;
                        }
                    didError = false;
                }
                catch (Exception)
                {
                    didError = true;
                }
            }
            // Add buttons
            foreach (RadioButton leftRadioButton in LeftButtonsCreated)
                instance.LeftStackPanelConfig.Children.Add(leftRadioButton);
            foreach (RadioButton rightRadioButton in RightButtonsCreated)
                instance.RightStackPanelConfig.Children.Add(rightRadioButton);
            // Add the final ParameterNames Button
            Button pnButton = new Button
            {
                Content = "Open ParameterNames Menu",
                Command = new SpecialCommandButton(() =>
                {
                    if(!ParameterNames.IsOpen)
                        new ParameterNames().Show();
                })
            };
            switch (side)
            {
                case 0:
                    instance.LeftStackPanelConfig.Children.Add(pnButton);
                    break;
                case 1:
                    instance.RightStackPanelConfig.Children.Add(pnButton);
                    break;
            }
        }
    }

    public class OnConfigRadioButtonPressed : ICommand
    {
        public static string SelectedConfigValue { get; private set; }
        
        private string Name { get; }
        private MainWindow Instance { get; }
        
        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            FieldInfo targetField = ConfigManager.LoadedConfig.GetType().GetField(Name);
            TommyComment desc = (TommyComment) Attribute.GetCustomAttribute(targetField, typeof(TommyComment));
            Instance.ConfigNameLabel.Text = targetField.Name;
            Instance.ConfigValueType.Text = FriendlyName(targetField.FieldType).ToLower();
            Instance.ConfigDescription.Text = desc.Value;
            SelectedConfigValue = Name;
            if (targetField.GetValue(ConfigManager.LoadedConfig) != null)
                Instance.ConfigValue.Text = targetField.GetValue(ConfigManager.LoadedConfig).ToString();
        }
        
        // BEGIN CREDITS
        
        /*
         * ToCsv and FriendlyName Methods made by Phil
         * Changes were made to remove this in parameters for both Methods
         * https://stackoverflow.com/a/34001032/12968919
         */
        
        private static string ToCsv(IEnumerable<object> collectionToConvert, string separator = ", ")
        {
            return String.Join(separator, collectionToConvert.Select(o => o.ToString()));
        }
        
        private static string FriendlyName(Type type)
        {
            if (type.IsGenericType)
            {
                var namePrefix = type.Name.Split(new [] {'`'}, StringSplitOptions.RemoveEmptyEntries)[0];
                var genericParameters = ToCsv(type.GetGenericArguments().Select(FriendlyName));
                return namePrefix + "<" + genericParameters + ">";
            }

            return type.Name;
        }
        
        // END CREDITS

        public event EventHandler? CanExecuteChanged = (sender, args) => { };

        public OnConfigRadioButtonPressed(MainWindow Instance, string Name)
        {
            this.Instance = Instance;
            this.Name = Name;
        }
    }

    public class SpecialCommandButton : ICommand
    {
        private Action OnClick;
        
        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => OnClick.Invoke();

        public event EventHandler? CanExecuteChanged = (sender, args) => {};

        public SpecialCommandButton(Action OnClick) => this.OnClick = OnClick;
    }
    
    [TommyTableName("HRtoVRChat_OSC")]
    public class Config
    {
        [TommyComment("The IP to send messages to")]
        [TommyInclude]
        public string ip = "127.0.0.1";
        [TommyComment("The Port to send messages to")]
        [TommyInclude]
        public int port = 9000;
        [TommyComment("The Port to receive messages from")]
        [TommyInclude]
        public int receiverPort = 9001;
        [TommyComment("The source from where to pull Heart Rate Data")]
        [TommyInclude]
        public string hrType = "unknown";
        [TommyComment("(FitbitHRtoWS Only) The WebSocket to listen to data")]
        [TommyInclude]
        public string fitbitURL = "ws://localhost:8080/";
        [TommyComment("(HypeRate Only) The code to pull HypeRate Data from")]
        [TommyInclude]
        public string hyperateSessionId = String.Empty;
        [TommyComment("(Pulsoid Only) The widgetId to pull HeartRate Data from")]
        [TommyInclude]
        public string pulsoidwidget = String.Empty;
        [TommyComment("(Stromno Only) The widgetId to pull HeartRate Data from Stromno")]
        [TommyInclude]
        public string stromnowidget = String.Empty;
        [TommyComment("(PulsoidSocket Only) The key for the OAuth API to pull HeartRate Data from")]
        [TommyInclude]
        public string pulsoidkey = String.Empty;
        [TommyComment("(TextFile Only) The location of the text file to pull HeartRate Data from")]
        [TommyInclude]
        public string textfilelocation = String.Empty;
        [TommyComment("The maximum HR for HRPercent")]
        [TommyInclude]
        public double MaxHR = 255;
        [TommyComment("The minimum HR for HRPercent")]
        [TommyInclude]
        public double MinHR = 0;
        [TommyComment("A dictionary containing what names to use for default parameters. DON'T CHANGE THE KEYS, CHANGE THE VALUES!")]
        [TommyInclude]
        public Dictionary<string, string> ParameterNames = new Dictionary<string, string>
        {
            ["onesHR"] = "onesHR",
            ["tensHR"] = "tensHR",
            ["hundredsHR"] = "hundredsHR",
            ["isHRConnected"] = "isHRConnected",
            ["isHRActive"] = "isHRActive",
            ["isHRBeat"] = "isHRBeat",
            ["HRPercent"] = "HRPercent",
            ["FullHRPercent"] = "FullHRPercent",
            ["HR"] = "HR"
        };
        [TommyComment("Allow HRtoVRChat_OSC to be used with ChilloutVR. Requires an OSC mod for ChilloutVR")]
        [TommyInclude]
        public bool ExpandCVR = true;

        public static bool DoesConfigExist() => File.Exists(ConfigManager.ConfigLocation);
    }

    [TommyTableName("HRtoVRChat")]
    public class UIConfig
    {
        [TommyComment("Automatically Start HRtoVRChat_OSC when VRChat is detected")] [TommyInclude]
        public bool AutoStart;

        [TommyComment("Force HRtoVRChat_OSC to run whether or not VRChat is detected")] [TommyInclude]
        public bool SkipVRCCheck;

        [TommyComment("Broadcast data over a WebSocket designed for Neos")] [TommyInclude]
        public bool NeosBridge;
    }
}