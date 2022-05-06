using Tommy.Serializer;

namespace HRtoVRChat_OSC
{
    public class ConfigManager
    {
        public static Config LoadedConfig { get; private set; }
        public static readonly string ConfigLocation = "config.cfg";
        
        public static void CreateConfig()
        {
            if (File.Exists(ConfigLocation))
            {
                // Load
                LogHelper.Log("Loading Config.");
                Config nc = TommySerializer.FromTomlFile<Config>(ConfigLocation) ?? new Config();
                SaveConfig(nc);
                LoadedConfig = nc;
            }
            else
            {
                // Create
                LogHelper.Log("No Config Found! Creating Config.");
                Config nc = new Config();
                SaveConfig(nc);
                LoadedConfig = nc;
            }
            LogHelper.Log("Loaded Config!");
        }

        public static void SaveConfig(Config config) => TommySerializer.ToTomlFile(config, ConfigLocation);
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
        [TommyComment("(PulsoidSocket Only) The key for the OAuth API to pull HeartRate Data from")]
        [TommyInclude]
        public string pulsoidkey = String.Empty;
        [TommyComment("(TextFile Only) The location of the text file to pull HeartRate Data from")]
        [TommyInclude]
        public string textfilelocation = String.Empty;
        [TommyComment("The maximum HR for HRPercent")]
        [TommyInclude]
        public double MaxHR = 150;
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
            ["HR"] = "HR"
        };
    }
}