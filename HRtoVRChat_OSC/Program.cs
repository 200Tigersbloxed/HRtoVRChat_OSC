﻿using System.Reflection;
using HRtoVRChat_OSC.HRManagers;
using HRtoVRChat_OSC_SDK;

namespace HRtoVRChat_OSC
{
    class Program
    {
        private static HRType hrType = HRType.Unknown;
        private static HRManager activeHRManager;
        private static AppBridge _appBridge = new AppBridge();
        private static bool isRestarting;
        private static bool RunHeartBeat = false;
        public static Action<int, int, int, int, bool, bool> OnHRValuesUpdated = (ones, tens, hundreds, HR, isConnected, isActive) => { };
        public static Action<bool, bool> OnHeartBeatUpdate = (isHeartBeat, shouldStart) => { };
        private static NeosBridge _neosBridge;
        
        private class currentHRSplit
        {
            public int ones = 0;
            public int tens = 0;
            public int hundreds = 0;
        }

        public static string[] Gargs { get; private set; } = new string[]{};
        
        private static CustomTimer loopCheck;
        
        private static Thread VerifyVRCOpen;
        private static CancellationTokenSource vvoToken = new CancellationTokenSource();
        private static Thread BeatThread;
        private static CancellationTokenSource btToken = new CancellationTokenSource();
        static void Main(string[] args)
        {
            Gargs = args;
            ConfigManager.CreateConfig();
            bool foundOnStart = OSCManager.Detect();
            OSCAvatarListener.Init();
            _appBridge.InitServer(() =>
            {
                if (activeHRManager != null)
                {
                    try
                    {
                        Messages.AppBridgeMessage apm = new()
                        {
                            CurrentSourceName = activeHRManager.GetName(),
                            CurrentAvatar = OSCAvatarListener.CurrentAvatar?.ToAvatarInfo()
                        };
                        foreach (ParamsManager.HRParameter hrParameter in ParamsManager.Parameters)
                        {
                            if(hrParameter.OriginalParameterName != null && hrParameter.ParamValue != null)
                                try
                                {
                                    PropertyInfo? pi = apm.GetType().GetProperty(hrParameter.OriginalParameterName);
                                    if (pi != null)
                                        pi.SetValue(apm, Convert.ChangeType(hrParameter.ParamValue, pi.PropertyType));
                                }
                                catch (Exception e)
                                {
                                    LogHelper.Error(
                                        "Failed to convert Parameter " + hrParameter.OriginalParameterName + " for AppBridge!", e);
                                }
                        }
                        return apm;
                    }
                    catch (Exception e)
                    {
                        LogHelper.Error("Failed to forward data from activeHRManager to AppBridge!", e);
                        return null;
                    }
                }
                else
                    return null;
            });
            if (!Directory.Exists(SDKManager.SDKsLocation))
                Directory.CreateDirectory(SDKManager.SDKsLocation);
            if (args.Contains("--neos-bridge"))
            {
                LogHelper.Log("Enabling NeosBridge");
                _neosBridge = new NeosBridge();
                NeosBridge.OnCommand += command => HandleCommand(command, true);
            }
            if (foundOnStart)
            {
                Check();
            }
            else
            {
                if (args.Contains("--auto-start"))
                {
                    LogHelper.Log("VRChat not found! Waiting for VRChat...");
                    loopCheck = new CustomTimer(5000, (ct) => LoopCheck());
                }
                else
                {
                    Check();
                }
            }
            HandleCommand(Console.ReadLine());
        }

        private static void LoopCheck()
        {
            bool foundVRC = OSCManager.Detect();;
            if (foundVRC)
            {
                LogHelper.Log("Found VRChat! Starting...");
                Check();
            }
        }

        private static void Check()
        {
            bool fromAutoStart = Gargs.Contains("--auto-start");
            if (OSCManager.Detect() || Gargs.Contains("--skip-vrc-check"))
            {
                if(loopCheck?.IsRunning ?? false)
                    loopCheck.Close();
                ParamsManager.InitParams();
                Start();
                HandleCommand(Console.ReadLine());
            }
            else
            {
                if (fromAutoStart)
                {
                    loopCheck.Close();
                    loopCheck = new CustomTimer(5000, (ct) => LoopCheck());
                }
                // Save all logs to file
                DateTime dt = DateTime.Now;
                LogHelper.SaveToFile($"{dt.Hour}-{dt.Minute}-{dt.Second}-{dt.Millisecond} {dt.Day}-{dt.Month}-{dt.Year}");
                // Exit
                LogHelper.Warn("VRChat was not detected! Press any key to continue.");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }

        private static readonly string HelpCommandString = "\n\n[Help]\n" +
                                                           "exit - Exits the app.\n" +
                                                           "starthr - Manually starts the HeartRateManager if it isn't already started.\n" +
                                                           "stophr - Manually stops the HeartRateManager if it is already started.\n" +
                                                           "restarthr - Stops then Starts the HeartRateManager.\n" +
                                                           "startbeat - Starts HeartBeat if it isn't enabled already.\n" +
                                                           "stopbeat - Stops the HeartBeat if it is already started.\n" +
                                                           "refreshconfig - Refreshes the Config from File.\n" +
                                                           "biassdk [sdkname] - Forces a specific SDK to be used (SDK hrType only)\n" +
                                                           "unbiassdk - Does not prefer any SDK (SDK hrType only)\n" +
                                                           "destroysdk [sdkname] - Unloads an SDK by name\n" +
                                                           "help - Shows available commands.\n";

        private static void HandleCommand(string? input, bool fromBridge = false)
        {
            string[] inputs = input?.Split(' ') ?? new string[0];
            switch (inputs[0].ToLower())
            {
                case "help":
                    LogHelper.Log(HelpCommandString);
                    break;
                case "exit":
                    Stop(true);
                    break;
                case "starthr":
                    StartHRListener();
                    break;
                case "stophr":
                    StopHRListener();
                    break;
                case "restarthr":
                    RestartHRListener();
                    break;
                case "startbeat":
                    if (BeatThread?.IsAlive ?? false)
                        LogHelper.Warn("Cannot start beat as it's already started!");
                    else
                    {
                        RunHeartBeat = true;
                        btToken = new CancellationTokenSource();
                        BeatThread = new Thread(() =>
                        {
                            RunHeartBeat = true;
                            HeartBeat();
                        });
                        BeatThread.Start();
                        LogHelper.Log("Started HeartBeat");
                    }
                    break;
                case "stopbeat":
                    if (BeatThread?.IsAlive ?? false)
                    {
                        try
                        {
                            btToken.Cancel();
                        }
                        catch(Exception e){LogHelper.Debug(e);}
                        RunHeartBeat = false;
                    }
                    LogHelper.Log("Stopped HRBeat");
                    break;
                case "refreshconfig":
                    ParamsManager.ResetParams();
                    ConfigManager.CreateConfig();
                    ParamsManager.InitParams();
                    break;
                case "biassdk":
                    if (!string.IsNullOrEmpty(inputs[1]))
                        SDKManager.PreferredSDK = inputs[1];
                    break;
                case "unbiassdk":
                    SDKManager.PreferredSDK = String.Empty;
                    break;
                case "destroysdk":
                    if (activeHRManager != null && !string.IsNullOrEmpty(inputs[1]))
                    {
                        SDKManager s =
                            (SDKManager) Convert.ChangeType(activeHRManager, typeof(SDKManager));
                        s.DestroySDKByName(inputs[1]);
                    }
                    break;
                default:
                    LogHelper.Warn($"Unknown Command \"{inputs[0]}\"!");
                    break;
            }
            if(!fromBridge)
                HandleCommand(Console.ReadLine());
        }

        public static CustomTimer BoopUwUTimer;
        
        private static void Start()
        {
            if (!Gargs.Contains("--skip-vrc-check"))
            {
                vvoToken = new CancellationTokenSource();
                VerifyVRCOpen = new Thread(() =>
                {
                    bool isOpen = OSCManager.Detect();
                    while (!vvoToken.IsCancellationRequested)
                    {
                        isOpen = OSCManager.Detect();
                        if(!isOpen)
                            vvoToken.Cancel();
                        Thread.Sleep(1500);
                    }
                    LogHelper.Log("Thread Stopped");
                    bool fromAutoStart = Gargs.Contains("--auto-start");
                    Stop(!fromAutoStart, fromAutoStart);
                });
                VerifyVRCOpen.Start();
            }
            // Continue
            StartHRListener();
            // Start Coroutine
            BoopUwUTimer = new CustomTimer(1000, (ct) => BoopUwU());
            LogHelper.Log("Started");
        }

        private static void Stop(bool quitApp = false, bool autoStart = false)
        {
            // Stop Everything
            if(activeHRManager != null)
                try
                {
                    BoopUwUTimer.Close();
                }
                catch (Exception e)
                {
                    LogHelper.Error("Failed to stop ActiveHRManager", e);
                }
            // Stop HR Listener
            StopHRListener();
            // Clear Parameters
            ParamsManager.ResetParams();
            // Stop Extraneous Tasks
            if (loopCheck?.IsRunning ?? false)
                loopCheck.Close();
            if(_neosBridge != null)
                _neosBridge.cts.Cancel();
            LogHelper.Log("Stopped");
            // Quit the App
            if (quitApp)
            {
                // Save all logs to file
                DateTime dt = DateTime.Now;
                LogHelper.SaveToFile($"{dt.Hour}-{dt.Minute}-{dt.Second}-{dt.Millisecond} {dt.Day}-{dt.Month}-{dt.Year}");
                // Stop AppBridge
                _appBridge.StopServer();
                // Exit
                Environment.Exit(0);
            }
            if (autoStart)
            {
                LogHelper.Log("Restarting when VRChat Detected");
                loopCheck = new CustomTimer(5000, (ct) => LoopCheck());
            }
        }

        public static void RestartHRListener()
        {
            int loops = 0;
            if (!isRestarting)
            {
                isRestarting = true;
                // Called for when you need to Reset the HRListener
                StopHRListener();
                Task.Factory.StartNew(() =>
                {
                    while(loops <= 2)
                    {
                        Task.Delay(1000);
                        loops++;
                    }
                    isRestarting = false;
                    StartHRListener(true);
                });
            }
        }

        private static HRType StringToHRType(string input)
        {
            HRType hrt = HRType.Unknown;
            switch (input.ToLower())
            {
                case "fitbithrtows":
                    hrt = HRType.FitbitHRtoWS;
                    break;
                case "hrproxy":
                    hrt = HRType.HRProxy;
                    break;
                case "hyperate":
                    hrt = HRType.HypeRate;
                    break;
                case "pulsoid":
                    hrt = HRType.Pulsoid;
                    break;
                case "stromno":
                    hrt = HRType.Stromno;
                    break;
                case "pulsoidsocket":
                    hrt = HRType.PulsoidSocket;
                    break;
                case "textfile":
                    hrt = HRType.TextFile;
                    break;
                case "omnicept":
                    hrt = HRType.Omnicept;
                    break;
                case "sdk":
                    hrt = HRType.SDK;
                    break;
            }

            return hrt;
        }

        private static void StartHRListener(bool fromRestart = false)
        {
            // Start Manager based on Config
            hrType = StringToHRType(ConfigManager.LoadedConfig.hrType);
            // Check activeHRManager
            if (activeHRManager != null)
                if (activeHRManager.IsActive())
                {
                    LogHelper.Warn("HRListener is currently active! Stop it first");
                    return;
                }
            switch (hrType)
            {
                case HRType.FitbitHRtoWS:
                    activeHRManager = new FitbitManager();
                    activeHRManager.Init(ConfigManager.LoadedConfig.fitbitURL);
                    break;
                case HRType.HRProxy:
                    activeHRManager = new HRProxyManager();
                    activeHRManager.Init(ConfigManager.LoadedConfig.hrproxyId);
                    break;
                case HRType.HypeRate:
                    activeHRManager = new HypeRateManager();
                    activeHRManager.Init(ConfigManager.LoadedConfig.hyperateSessionId);
                    break;
                case HRType.Pulsoid:
                    LogHelper.Warn("\n=========================================================================================\n" +
                                   "WARNING ABOUT PULSOID\n" +
                                   "It is detected that you're using the Pulsoid Method for grabbing HR Data,\n" +
                                   "Please note that this method will soon be DEPRECATED and replaced with PulsoidSocket!\n" +
                                   "Please see the URL below on how to upgrade!\n" +
                                   "https://github.com/200Tigersbloxed/HRtoVRChat_OSC/wiki/Upgrading-from-Pulsoid-to-PulsoidSocket \n" +
                                   "=========================================================================================\n\n" +
                                   "Starting Pulsoid in 25 Seconds...");
                    Thread.Sleep(25000);
                    activeHRManager = new PulsoidManager();
                    activeHRManager.Init(ConfigManager.LoadedConfig.pulsoidwidget);
                    break;
                case HRType.Stromno:
                    activeHRManager = new PulsoidManager();
                    activeHRManager.Init(ConfigManager.LoadedConfig.stromnowidget);
                    break;
                case HRType.PulsoidSocket:
                    activeHRManager = new PulsoidSocketManager();
                    activeHRManager.Init(ConfigManager.LoadedConfig.pulsoidkey);
                    break;
                case HRType.TextFile:
                    activeHRManager = new TextFileManager();
                    activeHRManager.Init(ConfigManager.LoadedConfig.textfilelocation);
                    break;
                case HRType.Omnicept:
                    activeHRManager = new OmniceptManager();
                    activeHRManager.Init(String.Empty);
                    break;
                case HRType.SDK:
                    activeHRManager = new SDKManager();
                    activeHRManager.Init("127.0.0.1:9000");
                    break;
                default:
                    LogHelper.Warn("No hrType was selected! Please see README if you think this is an error!");
                    Stop(true);
                    break;
            }
            // Start HeartBeats if there was a valid choice
            if (!RunHeartBeat)
            {
                if (BeatThread?.IsAlive ?? false)
                {
                    try
                    {
                        btToken.Cancel();
                    }
                    catch(Exception){}
                    RunHeartBeat = false;
                }
                btToken = new CancellationTokenSource();
                BeatThread = new Thread(() =>
                {
                    LogHelper.Log("Starting Beating!");
                    RunHeartBeat = true;
                    HeartBeat();
                });
                BeatThread.Start();
            }
            else if(activeHRManager == null)
                LogHelper.Warn("Can't start beat as ActiveHRManager is null!");
        }

        private static void StopHRListener()
        {
            if (activeHRManager != null)
            {
                if (!activeHRManager.IsActive())
                {
                    LogHelper.Warn("HRListener is currently inactive! Attempting to stop anyways.");
                    //return;
                }
                activeHRManager.Stop();
            }
            activeHRManager = null;
            // Stop Beating
            if (BeatThread?.IsAlive ?? false)
            {
                try
                {
                    btToken.Cancel();
                }
                catch(Exception){}
                RunHeartBeat = false;
            }
        }

        // why did i name the ienumerator this and why haven't i changed it
        static void BoopUwU()
        {
            currentHRSplit chs = new currentHRSplit();
            if (activeHRManager != null)
            {
                int HR = activeHRManager.GetHR();
                bool isOpen = activeHRManager.IsOpen();
                bool isActive = activeHRManager.IsActive();
                // Cast to currentHRSplit
                chs = intToHRSplit(HR);
                OnHRValuesUpdated.Invoke(chs.ones, chs.tens, chs.hundreds, HR, isOpen, isActive);
            }
            else
                OnHRValuesUpdated.Invoke(0, 0, 0, 0, false, false);
        }

        static void HeartBeat()
        {
            bool waited = false;
            while (!btToken.IsCancellationRequested)
            {
                if(!RunHeartBeat)
                    btToken.Cancel();
                else
                {
                    if(activeHRManager != null)
                {
                    bool io = activeHRManager.IsOpen();
                    // This should be started by the Melon Update void
                    if (io)
                    {
                        // Get HR
                        float HR = activeHRManager.GetHR();
                        if (HR > 0)
                        {
                            if (waited)
                            {
                                LogHelper.Log("Found ActiveHRManager! Starting HeartBeat.");
                                waited = false;
                            }
                            OnHeartBeatUpdate.Invoke(false, false);
                            // Calculate wait interval
                            float waitTime = default(float);
                            // When lowering the HR significantly, this will cause issues with the beat bool
                            // Dubbed the "Breathing Exercise" bug
                            // There's a 'temp' fix for it right now, but I'm not sure how it'll hold up
                            try { waitTime = (1 / ((HR - 0.2f) / 60)); } catch (Exception) { /*Just a Divide by Zero Exception*/ }
                            Thread.Sleep((int) (waitTime * 1000));
                            OnHeartBeatUpdate.Invoke(true, false);
                            Thread.Sleep(100);
                        }
                        else
                        {
                            LogHelper.Warn("Cannot beat as HR is Less than or equal to zero");
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        OnHeartBeatUpdate.Invoke(false, true);
                        LogHelper.Debug("Waiting for ActiveHRManager for beating");
                        waited = true;
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    LogHelper.Warn("Cannot beat as ActiveHRManager is null!");
                    Thread.Sleep(1000);
                }
                }
            }
        }

        private static currentHRSplit intToHRSplit(int hr)
        {
            currentHRSplit chs = new currentHRSplit();
            if (hr < 0)
                LogHelper.Error("HeartRate is below zero.");
            else
            {
                var currentNumber = hr.ToString().Select(x => int.Parse(x.ToString()));
                int[] numbers = currentNumber.ToArray();
                if(hr <= 9)
                {
                    // why is your HR less than 10????
                    try
                    {
                        chs.ones = numbers[0];
                        chs.tens = 0;
                        chs.hundreds = 0;
                    }
                    catch (Exception) { }
                }
                else if(hr <= 99)
                {
                    try
                    {
                        chs.ones = numbers[1];
                        chs.tens = numbers[0];
                        chs.hundreds = 0;
                    }
                    catch (Exception) { }
                }
                else if(hr >= 100)
                {
                    try
                    {
                        chs.ones = numbers[2];
                        chs.tens = numbers[1];
                        chs.hundreds = numbers[0];
                    }
                    catch (Exception) { }
                }
                else
                {
                    // if your heart rate is above 999 then you need to see a doctor
                    // for real what
                }
            }

            return chs;
        }

        private enum HRType
        {
            FitbitHRtoWS,
            HRProxy,
            HypeRate,
            Pulsoid,
            Stromno,
            PulsoidSocket,
            TextFile,
            Omnicept,
            SDK,
            Unknown
        }
    }
}