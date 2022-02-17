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
                LoadedConfig = nc;
            }
            else
            {
                // Create
                LogHelper.Log("No Config Found! Creating Config.");
                Config nc = new Config();
                TommySerializer.ToTomlFile(nc, ConfigLocation);
                LoadedConfig = nc;
            }
            LogHelper.Log("Loaded Config!");
        }
    }
    
    [TommyTableName("HRtoVRChat_OSC")]
    public class Config
    {
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
        [TommyComment("(TextFile Onlu) The location of the text file to pull HeartRate Data from")]
        [TommyInclude]
        public string textfilelocation = String.Empty;
        [TommyComment("The maximum HR for HRPercent")]
        [TommyInclude]
        public double MaxHR = 150;
        [TommyComment("The minimum HR for HRPercent")]
        [TommyInclude]
        public double MinHR = 0;
    }
}