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
        client.Events.DataReceived += (sender, args) =>
        {
            try
            {
                byte[] data = args.Data;
                object? fakeDeserialize = Messages.DeserializeMessage(data);
                string messageType = Messages.GetMessageType(fakeDeserialize);
                switch (messageType)
                {
                    case "HRMessage":
                        Messages.HRMessage hrm = Messages.DeserializeMessage<Messages.HRMessage>(data);
                        OnSDKData(hrm);
                        break;
                }
            }
            catch(Exception){}
        };
        if(autoOpen)
            Open();
    }
    
    public abstract int HR { get; set; }
    public abstract bool IsOpen { get; set; }
    public abstract bool IsActive { get; set; }

    public virtual void OnSDKOpened(){}
    public virtual void OnSDKData(Messages.HRMessage message){}
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

    public void RequestHRData()
    {
        if (isClientConnected)
        {
            Messages.GetHRData ghrd = new Messages.GetHRData();
            byte[] data = ghrd.Serialize();
            client?.Send(data);
        }
    }

    public void Open() => client?.Connect();
    public void Close() => client?.Disconnect();
}