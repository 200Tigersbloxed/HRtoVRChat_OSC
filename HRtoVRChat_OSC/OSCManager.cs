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
            OscMessage message = new OscMessage(destination, data);
            UDPSender sender = new UDPSender("127.0.0.1", 9000);
            sender.Send(message);
        }
    }
}