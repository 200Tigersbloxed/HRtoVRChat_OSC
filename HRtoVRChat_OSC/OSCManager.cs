using System.Diagnostics;
using SharpOSC;

namespace HRtoVRChat_OSC
{
    public static class OSCManager
    {
        private static UDPListener listener = new UDPListener(9001, packet => OnOscMessage.Invoke((OscMessage) packet));
        public static Action<OscMessage> OnOscMessage = oscm => { };

        public static bool Detect() => Process.GetProcessesByName("VRChat").Length > 0;

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
}