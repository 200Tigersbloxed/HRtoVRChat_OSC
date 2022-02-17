namespace HRtoVRChat_OSC
{
    class Program
    {
        private static HRType hrType = HRType.Unknown;
        private static HRManager activeHRManager;
        private static bool isRestarting;
        public static Action<int, int, int, int, bool, bool> OnHRValuesUpdated = (ones, tens, hundreds, HR, isConnected, isActive) => { };
        public static Action<bool, bool> OnHeartBeatUpdate = (isHeartBeat, shouldStart) => { };
        public static bool isHeartBeat { get; private set; } = false;
        
        private class currentHRSplit
        {
            public int ones = 0;
            public int tens = 0;
            public int hundreds = 0;
        }

        private static string[] Gargs;
        
        private static CustomTimer loopCheck;
        
        private static Thread VerifyVRCOpen;
        static void Main(string[] args)
        {
            bool foundOnStart = OSCManager.Detect();
            Gargs = args;
            ConfigManager.CreateConfig();
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
                LogHelper.Warn("VRChat was not detected! Press any key to continue.");
                Console.ReadKey();
            }
        }

        private static void HandleCommand(string? input)
        {
            string[] inputs = input?.Split(' ') ?? new string[0];
            switch (inputs[0])
            {
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
                default:
                    LogHelper.Warn($"Unknown Command \"{inputs[0]}\"!");
                    break;
            }
            HandleCommand(Console.ReadLine());
        }

        public static CustomTimer BoopUwUTimer;
        
        private static void Start()
        {
            if (!Gargs.Contains("--skip-vrc-check"))
            {
                VerifyVRCOpen = new Thread(() =>
                {
                    bool isOpen = OSCManager.Detect();
                    while (isOpen)
                    {
                        isOpen = OSCManager.Detect();
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
            LogHelper.Debug("Started");
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
            ParamsManager.Parameters.Clear();
            // Stop Extraneous Tasks
            if (loopCheck?.IsRunning ?? false)
                loopCheck.Close();
            LogHelper.Debug("Stopped");
            // Quit the App
            if (quitApp)
                Environment.Exit(1);
            if (autoStart)
            {
                LogHelper.Log("Restarting when VRChat Detected");
                loopCheck = new CustomTimer(5000, (ct) => LoopCheck());
            }
        }

        private static void RestartHRListener()
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
                case "hyperate":
                    hrt = HRType.HypeRate;
                    break;
                case "pulsoid":
                    hrt = HRType.Pulsoid;
                    break;
                case "pulsoidsocket":
                    hrt = HRType.PulsoidSocket;
                    break;
                case "textfile":
                    hrt = HRType.TextFile;
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
                    activeHRManager = new HRManagers.FitbitManager();
                    activeHRManager.Init(ConfigManager.LoadedConfig.fitbitURL);
                    break;
                case HRType.HypeRate:
                    activeHRManager = new HRManagers.HypeRateManager();
                    activeHRManager.Init(ConfigManager.LoadedConfig.hyperateSessionId);
                    break;
                case HRType.Pulsoid:
                    activeHRManager = new HRManagers.PulsoidManager();
                    activeHRManager.Init(ConfigManager.LoadedConfig.pulsoidwidget);
                    break;
                case HRType.PulsoidSocket:
                    activeHRManager = new HRManagers.PulsoidSocketManager();
                    activeHRManager.Init(ConfigManager.LoadedConfig.pulsoidkey);
                    break;
                case HRType.TextFile:
                    activeHRManager = new HRManagers.TextFileManager();
                    activeHRManager.Init(ConfigManager.LoadedConfig.textfilelocation);
                    break;
                default:
                    LogHelper.Warn("No hrType was selected! Please see README if you think this is an error!");
                    Stop(true);
                    break;
            }
            // Start HeartBeats if there was a valid choice
            if (activeHRManager != null && !fromRestart)
            {
                LogHelper.Debug("Starting Beating!");
                HeartBeat();
            }
        }

        private static void StopHRListener()
        {
            if (activeHRManager != null)
            {
                if (!activeHRManager.IsActive())
                {
                    LogHelper.Warn("HRListener is currently inactive! Start it first!");
                    return;
                }
                activeHRManager.Stop();
            }
            activeHRManager = null;
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
        }

        static void HeartBeat()
        {
            if(activeHRManager != null || isRestarting)
            {
                bool io = activeHRManager.IsOpen();
                // This should be started by the Melon Update void
                if (io)
                {
                    isHeartBeat = false;
                    // Get HR
                    float HR = activeHRManager.GetHR();
                    if (HR != 0)
                    {
                        isHeartBeat = false;
                        OnHeartBeatUpdate.Invoke(isHeartBeat, false);
                        // Calculate wait interval
                        float waitTime = default(float);
                        // When lowering the HR significantly, this will cause issues with the beat bool
                        // Dubbed the "Breathing Exercise" bug
                        // There's a 'temp' fix for it right now, but I'm not sure how it'll hold up
                        try { waitTime = (1 / ((HR - 0.2f) / 60)); } catch (Exception) { /*Just a Divide by Zero Exception*/ }
                        new ExecuteInTime((int) (waitTime * 1000), (eit) =>
                        {
                            isHeartBeat = true;
                            OnHeartBeatUpdate.Invoke(isHeartBeat, false);
                            HeartBeat();
                        });
                    }
                }
                else
                {
                    isHeartBeat = false;
                    OnHeartBeatUpdate.Invoke(isHeartBeat, true);
                    HeartBeat();
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
            HypeRate,
            Pulsoid,
            PulsoidSocket,
            TextFile,
            Unknown
        }
    }
}