using System;
using System.IO;
using Tommy.Serializer;

namespace HRtoVRChat_OSC_UI
{
    public class ConfigManager
    {
        public static Config LoadedConfig { get; private set; }
        public static readonly string ConfigLocation = Path.Combine(GithubTools.outputPath, "config.cfg");
        
        public static void CreateConfig()
        {
            if (File.Exists(ConfigLocation))
            {
                // Load
                Config nc = TommySerializer.FromTomlFile<Config>(ConfigLocation) ?? new Config();
                //SaveConfig(nc);
                LoadedConfig = nc;
            }
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
        public double MaxHR = 150;
        [TommyComment("The minimum HR for HRPercent")]
        [TommyInclude]
        public double MinHR = 0;
    }
}