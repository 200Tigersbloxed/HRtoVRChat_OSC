using SuperSimpleTcp;

namespace HRtoVRChat_OSC_SDK;

public abstract class HRSDK
{
    private SimpleTcpClient? client;
    
    /// <summary>
    /// Whether or not the client is connected to the server
    /// </summary>
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
    
    /// <summary>
    /// The current HeartRate
    /// </summary>
    public abstract int HR { get; set; }
    
    /// <summary>
    /// If the device transmitting data to the source is connected.
    /// If your service does not support this, then you can point it to IsOpen
    /// </summary>
    public abstract bool IsOpen { get; set; }
    
    /// <summary>
    /// If there's an active connection to the source
    /// </summary>
    public abstract bool IsActive { get; set; }

    /// <summary>
    /// Callback for when the client opens a connection with the server
    /// </summary>
    public virtual void OnSDKOpened(){}
    
    /// <summary>
    /// Callback for when the client gets a message from the server
    /// </summary>
    /// <param name="message"></param>
    public virtual void OnSDKData(Messages.HRMessage message){}
    
    /// <summary>
    /// Callback for when the client's connection to the server is closed
    /// </summary>
    public virtual void OnSDKClosed(){}

    /// <summary>
    /// Updates the current HeartRate data to the server
    /// </summary>
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

    /// <summary>
    /// Requests the current HeartRate data that the server has.
    /// This is called back in the OnSDKData virtual void.
    /// </summary>
    public void RequestHRData()
    {
        if (isClientConnected)
        {
            Messages.GetHRData ghrd = new Messages.GetHRData();
            byte[] data = ghrd.Serialize();
            client?.Send(data);
        }
    }

    /// <summary>
    /// Open a connection to the server
    /// </summary>
    public void Open() => client?.Connect();
    
    /// <summary>
    /// Close the connection to the server
    /// </summary>
    public void Close() => client?.Disconnect();
}