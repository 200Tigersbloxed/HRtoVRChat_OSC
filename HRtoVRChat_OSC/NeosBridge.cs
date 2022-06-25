using System.Reflection;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace HRtoVRChat_OSC;

public class NeosBridge
{
    public static Action<string> OnCommand = s => { };
    public static NeosMessage _neosMessage = new();

    public CancellationTokenSource cts;

    private float GetHRPercent(float HR)
    {
        float targetFloat = 0f;
        float maxhr = (float) ConfigManager.LoadedConfig.MaxHR;
        float minhr = (float) ConfigManager.LoadedConfig.MinHR;
        if (HR > maxhr)
            targetFloat = 1;
        else if (HR < minhr)
            targetFloat = 0;
        else
            targetFloat = (HR - minhr) / (maxhr - minhr);
        return targetFloat;
    }

    public NeosBridge()
    {
        cts = new CancellationTokenSource();
        WebSocketServer server = new WebSocketServer(4206);
        Thread worker = new Thread(() =>
        {
            Program.OnHRValuesUpdated += (ones, tens, hundreds, hr, isConnected, isActive) =>
            {
                _neosMessage.onesHR = ones;
                _neosMessage.tensHR = tens;
                _neosMessage.hundredsHR = hundreds;
                _neosMessage.HR = hr;
                _neosMessage.isConnected = isConnected;
                _neosMessage.isActive = isActive;
                _neosMessage.HRPercent = GetHRPercent(hr);
                try
                {
                    string msg = _neosMessage.Serialize();
                    server.WebSocketServices.Broadcast(msg);
                }
                catch (Exception e)
                {
                    LogHelper.Warn("Failed to broadcast message! Exception: " + e);
                }
            };
            Program.OnHeartBeatUpdate += (isHRBeat, shouldStart) =>
            {
                _neosMessage.isHRBeat = isHRBeat;
                try
                {
                    string msg = _neosMessage.Serialize();
                    server.WebSocketServices.Broadcast(msg);
                }
                catch (Exception e)
                {
                    LogHelper.Warn("Failed to broadcast message! Exception: " + e);
                }
            };
            server.AddWebSocketService<NeosSocketBehavior>("/HRtoVRChat");
            server.Start();
            while (!cts.IsCancellationRequested)
            {
                Thread.Sleep(10);
            }
            server.Stop();
        });
        worker.Start();
    }

    public class NeosSocketBehavior : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs messageEventArgs)
        {
            string msg = messageEventArgs.Data;
            OnCommand.Invoke(msg);
        }
    }

    public class NeosMessage
    {
        public int onesHR;
        public int tensHR;
        public int hundredsHR;
        public int HR;
        public bool isConnected;
        public bool isActive;
        public bool isHRBeat;
        public float HRPercent;

        public string Serialize()
        {
            string msg = String.Empty;
            foreach (FieldInfo fieldInfo in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
                msg += fieldInfo.Name + "=" + fieldInfo.GetValue(this) + ",";
            return msg.TrimEnd(',');
        }
    }
}