using SuperSimpleTcp;

namespace HRtoVRChat_OSC_SDK;

public abstract class HRSDK
{
    private SimpleTcpClient? client;
    public bool isClientConnected => client?.IsConnected ?? false;
    
    public HRSDK(bool autoOpen = true, string ipPort = "127.0.0.1:9000")
    {
        client = new SimpleTcpClient(ipPort);
        client.Events.Connected += (sender, args) => OnSDKOpened();
        client.Events.Disconnected += (sender, args) => OnSDKClosed();
        if(autoOpen)
            Open();
    }
    
    public abstract int HR { get; set; }
    public abstract bool IsOpen { get; set; }
    public abstract bool IsActive { get; set; }

    public virtual void OnSDKOpened(){}
    public virtual void OnSDKClosed(){}

    public void Update()
    {
        if (isClientConnected)
        {
            Messages.HRMessage hrm = new Messages.HRMessage
            {
                HR = HR,
                IsOpen = IsOpen,
                IsActive = IsActive
            };
            byte[] data = hrm.Serialize();
            client?.Send(data);
        }
    }

    public void Open() => client?.Connect();
    public void Close() => client?.Disconnect();
}