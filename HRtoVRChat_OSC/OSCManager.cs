using System.Diagnostics;
using System.Text.Json;
using HRtoVRChat_OSC_SDK;
using SharpOSC;

namespace HRtoVRChat_OSC
{
    public static class OSCManager
    {
        private static UDPListener listener = new UDPListener(9001, packet => OnOscMessage.Invoke((OscMessage?) packet));
        public static Action<OscMessage?> OnOscMessage = oscm => { };

        public static bool Detect()
        {
            int processes = Process.GetProcessesByName("VRChat").Length;
            if (Program.Gargs.Contains("--neos-bridge"))
                processes += Process.GetProcessesByName("Neos").Length;
            return processes > 0;
        }

        public static void SendMessage(string destination, object data)
        {
            object realdata = data;
            // If it's a bool, it needs to be converted to a 0, 1 format
            if (Type.GetTypeCode(realdata.GetType()) == TypeCode.Boolean)
            {
                bool dat = (bool) Convert.ChangeType(realdata, TypeCode.Boolean);
                if (dat)
                    realdata = 1;
                else
                    realdata = 0;
            }
            OscMessage message = new OscMessage(destination, realdata);
            UDPSender sender = new UDPSender(ConfigManager.LoadedConfig.ip, ConfigManager.LoadedConfig.port);
            sender.Send(message);
        }
    }

    public static class OSCAvatarListener
    {
        public static AvatarChangeMessage? CurrentAvatar { get; private set; }
        public static Action<AvatarChangeMessage> OnAvatarChanged = message => { };

        public static void Init()
        {
            OSCManager.OnOscMessage += message =>
            {
                string msg = message?.ToString() ?? "unknown";
                if (message != null && msg != "unknown")
                {
                    switch (message?.Address)
                    {
                        case "/avatar/change":
                            // Find the AvatarFile
                            string location = FindAvatarLocation((string) message.Arguments[0]);
                            if (location != String.Empty && File.Exists(location))
                            {
                                // Read the file
                                string text = File.ReadAllText(location);
                                AvatarChangeMessage? acm = JsonSerializer.Deserialize<AvatarChangeMessage>(text);
                                if(acm != null)
                                {
                                    CurrentAvatar = acm;
                                    OnAvatarChanged.Invoke(acm);
                                }
                            }
                            break;
                    }
                }
            };
        }

        private static string FindAvatarLocation(string id)
        {
            // Since this is just for debug, any user directory will work fine
            string fileLocation = String.Empty;
            string vrchat_osc_data_location =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/../LocalLow",
                    "VRChat/VRChat/OSC");
            if (Directory.Exists(vrchat_osc_data_location))
            {
                foreach (string directory in Directory.GetDirectories(vrchat_osc_data_location))
                {
                    foreach (string file in Directory.GetFiles(Path.Combine(directory, "Avatars")))
                    {
                        string fn = Path.GetFileNameWithoutExtension(file);
                        if (fn == id)
                            fileLocation = file;
                    }
                }
            }
            if(string.IsNullOrEmpty(fileLocation) || !File.Exists(fileLocation))
                LogHelper.Warn("No Config File Found for avatar with id " + id);
            return fileLocation;
        }

        public record AvatarChangeMessage
        {
            public string id { get; set; }
            public string name { get; set; }
            public List<AvatarParameters> parameters { get; set; }

            public Messages.AvatarInfo ToAvatarInfo()
            {
                Messages.AvatarInfo ai = new Messages.AvatarInfo
                {
                    id = id,
                    name = name,
                    parameters = new List<string>()
                };
                foreach (AvatarParameters avatarParameters in parameters)
                    ai.parameters.Add(avatarParameters.name);
                return ai;
            }
        }

        public record AvatarParameters
        {
            public string name { get; set; }
            public AvatarInputOutput input { get; set; }
            public AvatarInputOutput output { get; set; }
        }

        public record AvatarInputOutput
        {
            public string address { get; set; }
            public string type { get; set; }
        }
    }
}