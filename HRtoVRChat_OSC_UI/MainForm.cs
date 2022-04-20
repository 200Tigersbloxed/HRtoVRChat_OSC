using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Tommy.Serializer;

namespace HRtoVRChat_OSC_UI
{
    public partial class MainForm : MaterialForm
    {
        public static Action<int> processProgressChanged = i => { };
        public static Action<int> overallProgressChanged = i => { };
        public static Action<string> UpdateConsoleOutput = s => { };
        private bool gotLocalVersion;
        private string localVersion;
        private bool gotCloudVersion;

        public bool IsOSCRunning => Process.GetProcessesByName("HRtoVRChat_OSC").Length > 0;
        
        public MainForm()
        {
            InitializeComponent();

            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.EnforceBackcolorOnAllComponents = false;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Red800, Primary.Red900,
                Primary.Red500, Accent.Red100, TextShade.WHITE);

            Closed += (sender, args) =>
            {
                Application.Exit();
            };

            tabPage2.VisibleChanged += (sender, args) => UpdateVersionLabels(tabPage2.Visible, false);
            refreshVersions.Click += (sender, args) => UpdateVersionLabels(true);
            materialCheckbox1.CheckedChanged += (sender, args) => VerifyCheckboxes(ref materialCheckbox1);
            materialCheckbox2.CheckedChanged += (sender, args) => VerifyCheckboxes(ref materialCheckbox2);
            processProgressChanged += i => processProgress.Value = i;
            overallProgressChanged += i => overallProgress.Value = i;
            updateSoftwareButton.Click += (sender, args) => GithubTools.DownloadLatestVersion(() =>
                MessageBox.Show("Finished Downloading Software!", "HRtoVRChat_OSC_UI", MessageBoxButtons.OK,
                    MessageBoxIcon.Information));
            startButton.Click += (sender, args) => StartProgram();
            stopButton.Click += (sender, args) =>
            {
                try
                {
                    myStreamWriter?.WriteLine("exit");
                    UpdateConsoleOutput.Invoke("> exit");
                    myStreamWriter?.Close();
                    CurrentProcess?.Kill();
                }
                catch(Exception){}
            };
            sendCommand.Click += (sender, args) =>
            {
                if (IsOSCRunning)
                {
                    try
                    {
                        myStreamWriter?.WriteLine(commandInput.Text);
                        UpdateConsoleOutput.Invoke("> " + commandInput.Text);
                        commandInput.Text = String.Empty;
                    }
                    catch(Exception){}
                }
            };
            UpdateConsoleOutput += s => Invoke((Action)(() =>
            {
                richTextBox1.Text = richTextBox1.Text + "\n" + s;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }));
            System.Timers.Timer timer = new System.Timers.Timer(2500)
            {
                Enabled = true,
                AutoReset = true
            };
            timer.Elapsed += (sender, args) =>
            {
                if (IsOSCRunning)
                {
                    try
                    {
                        Invoke((Action) (() =>
                        {
                            statusLabel.Text = "RUNNING";
                        }));
                    }
                    catch(Exception){}
                }
                else
                {
                    try
                    {
                        Invoke((Action) (() =>
                        {
                            statusLabel.Text = "STOPPED";
                        }));
                    }
                    catch(Exception){}
                }
            };
            UpdateVersionLabels(true);
            ConfigManager.CreateConfig();
            SetupConfigRadios();
            updateConfigValueButton.Click += (sender, args) => UpdateConfigButtonPressed();
            killAllProcesses.Click += (sender, args) =>
            {
                foreach (Process process in Process.GetProcessesByName("HRtoVRChat_OSC"))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception)
                    {
                    }
                }
            };

            if (!NETDetect.IsNet6Installed())
            {
                bool install = false;
                DialogResult dr = MessageBox.Show(".NET6 was not detected! Would you like to install it?",
                    "HRtoVRChat_OSC_UI", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                    install = true;
                else
                {
                    DialogResult dr2 =
                        MessageBox.Show(
                            "Are you sure? Not installing the .NET6 Runtime may cause issues. Install the .NET6 Runtime?",
                            "HRtoVRChat_OSC_UI", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (dr2 == DialogResult.Yes)
                        install = true;
                }

                if (install)
                    NETDetect.InstallNETSDK();
            }
        }

        private void SetupConfigRadios()
        {
            ipRadioButton.CheckedChanged += (sender, args) => NewConfigRadioSelected(ipRadioButton);
            portRadioButton.CheckedChanged += (sender, args) => NewConfigRadioSelected(portRadioButton);
            hrTypeRadioButton.CheckedChanged += (sender, args) => NewConfigRadioSelected(hrTypeRadioButton);
            fitbitURLRadioButton.CheckedChanged += (sender, args) => NewConfigRadioSelected(fitbitURLRadioButton);
            hyperateSessionIdRadioButton.CheckedChanged += (sender, args) => NewConfigRadioSelected(hyperateSessionIdRadioButton);
            pulsoidwidgetRadioButton.CheckedChanged += (sender, args) => NewConfigRadioSelected(pulsoidwidgetRadioButton);
            pulsoidkeyRadioButton.CheckedChanged += (sender, args) => NewConfigRadioSelected(pulsoidkeyRadioButton);
            textfilelocationRadioButton.CheckedChanged += (sender, args) => NewConfigRadioSelected(textfilelocationRadioButton);
            MaxHRRadioButton.CheckedChanged += (sender, args) => NewConfigRadioSelected(MaxHRRadioButton);
            MinHRRadioButton.CheckedChanged += (sender, args) => NewConfigRadioSelected(MinHRRadioButton);
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

        private void NewConfigRadioSelected(RadioButton btn)
        {
            if(ConfigManager.LoadedConfig == null)
                ConfigManager.CreateConfig();
            if (ConfigManager.LoadedConfig == null)
            {
                MessageBox.Show("Loaded Config is null! Cannot continue");
                return;
            }
            if (btn.Checked)
            {
                // Find the current instance
                FieldInfo field = ConfigManager.LoadedConfig.GetType()
                    .GetField(btn.Text, BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    configNameLabel.Text = field.Name;
                    configType.Text = FriendlyName(field.FieldType).ToLower();
                    // Get the value
                    string value = field.GetValue(ConfigManager.LoadedConfig).ToString();
                    configValueInput.Text = value;
                    // Get the Description
                    try
                    {
                        TommyComment comment = (TommyComment) Attribute.GetCustomAttribute(field, typeof(TommyComment));
                        configDescriptionLabel.Text = "Description: " + comment.Value;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Failed to get Description of Config Value " + btn.Text);
                    }
                }
                else
                    MessageBox.Show("Failed to find config field " + btn.Text);
            }
        }

        private void UpdateConfigButtonPressed()
        {
            if(ConfigManager.LoadedConfig == null)
                ConfigManager.CreateConfig();
            if (ConfigManager.LoadedConfig == null)
            {
                MessageBox.Show("Loaded Config is null! Cannot continue");
                return;
            }
            if (File.Exists(ConfigManager.ConfigLocation))
            {
                string configName = configNameLabel.Text;
                string configValue = configValueInput.Text;
                if (configName != "value")
                {
                    // Get field
                    FieldInfo field = ConfigManager.LoadedConfig.GetType()
                        .GetField(configName, BindingFlags.Public | BindingFlags.Instance);
                    if (field != null)
                    {
                        // Update the value
                        field.SetValue(ConfigManager.LoadedConfig, Convert.ChangeType(configValue, field.FieldType));
                        // Save the Config
                        ConfigManager.SaveConfig(ConfigManager.LoadedConfig);
                    }
                    else
                        MessageBox.Show("Failed to find config field " + configName);
                }
            }
            else
                MessageBox.Show("Config does not exist at location: " + Path.GetFullPath(ConfigManager.ConfigLocation));
        }

        public void UpdateVersionLabels(bool tabPageVisible, bool bypassgotcheck)
        {
            if (tabPageVisible)
            {
                // Get the current Version
                if (!gotCloudVersion || bypassgotcheck)
                {
                    availableVersionLabel.Text = "Latest Version: " + GithubTools.GetLatestVersion();
                    gotCloudVersion = true;
                }
                // Get the local Version
                if (!gotLocalVersion || bypassgotcheck)
                {
                    string file = Path.Combine(GithubTools.outputPath, "version.txt");
                    string version = String.Empty;
                    if (File.Exists(file))
                    {
                        using (StreamReader sr = new StreamReader(file))
                        {
                            string text = sr.ReadToEnd();
                            version = text;
                            localVersion = text;
                            gotLocalVersion = true;
                        }
                    }
                    else
                        version = "unknown";
                    currentVersionLabel.Text = "Installed Version: " + version;
                }
            }
        }

        public void UpdateVersionLabels(bool bypassgotcheck) => UpdateVersionLabels(true, bypassgotcheck);

        public void VerifyCheckboxes(ref MaterialCheckbox checkbox)
        {
            if (materialCheckbox1.Checked && materialCheckbox2.Checked)
            {
                materialCheckbox1.Checked = false;
                materialCheckbox2.Checked = false;
                checkbox.Checked = true;
            }
        }

        public Process CurrentProcess;
        public StreamWriter myStreamWriter;

        public void StartProgram()
        {
            if (GithubTools.IsHRtoVRChat_OSCPresent && !IsOSCRunning)
            {
                string args = String.Empty;
                richTextBox1.Text = "HRtoVRChat_OSC " + localVersion + " Created by: 200Tigersbloxed";
                if (materialCheckbox1.Checked)
                    args += "--auto-start ";
                if (materialCheckbox2.Checked)
                    args += "--skip-vrc-check ";
                using (Process exec = new Process())
                {
                    exec.StartInfo = new ProcessStartInfo
                    {
                        FileName = GithubTools.Executable,
                        WorkingDirectory = GithubTools.outputPath,
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    exec.OutputDataReceived += (sender, eventArgs) =>
                    {
                        UpdateConsoleOutput.Invoke(eventArgs.Data);
                    };
                    exec.Start();
                    exec.StandardInput.AutoFlush = true;
                    myStreamWriter = exec.StandardInput;
                    exec.BeginOutputReadLine();
                    CurrentProcess = exec;
                }
            }
        }
    }
}