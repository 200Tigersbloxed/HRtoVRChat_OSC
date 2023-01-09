using System.Diagnostics;
using System.Globalization;
using SuperSimpleTcp;

namespace HRtoVRChat_OSC_SDK;

public abstract class HRSDK
{
    private SimpleTcpClient? client;
    
    /// <summary>
    /// The Options; mainly used for Reflection, but are still required to be set.
    /// </summary>
    public abstract HRSDKOptions Options { get; }

    /// <summary>
    /// Whether or not the client is connected to the server
    /// </summary>
    public bool IsSDKConnected => (client?.IsConnected ?? false) || IsReflected;

    private bool _isReflected = false;

    /// <summary>
    /// Whether or not the SDK was loaded through Reflection.
    /// </summary>
    public bool IsReflected => _isReflected;
    
    /// <summary>
    /// The current HeartRate
    /// </summary>
    public abstract int HR { get; set; }
    
    /// <summary>
    /// If the device transmitting data to the source is connected.
    /// If your service does not support this, then you can point it to IsActive
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
    /// Callback for when the client is updated by the server
    /// </summary>
    public virtual void OnSDKUpdate(){}
    
    /// <summary>
    /// Callback for when the client gets a message from the server
    /// </summary>
    /// <param name="message"></param>
    public virtual void OnSDKData(Messages.HRMessage message){}
    
    /// <summary>
    /// Callback for when the client's connection to the server is closed
    /// </summary>
    public virtual void OnSDKClosed(){}

    // Used for Patching
    private void on_push_data(Messages.HRMessage hrm){}
    private void on_pull_data(string requesting_sdk){}

    /// <summary>
    /// Updates the current HeartRate data to the server
    /// </summary>
    public void PushData()
    {
        if (IsSDKConnected)
        {
            Messages.HRMessage hrm = new Messages.HRMessage
            {
                SDKName = Options.SDKName,
                HR = HR,
                IsOpen = IsOpen,
                IsActive = IsActive
            };
            if (!IsReflected)
            {
                byte[] data = hrm.Serialize();
                client?.Send(data);
            }
            else
                on_push_data(hrm);
        }
    }

    /// <summary>
    /// Requests the current HeartRate data that the server has.
    /// This is called back in the OnSDKData virtual void.
    /// </summary>
    public void PullData()
    {
        if (IsSDKConnected)
        {
            if (!IsReflected)
            {
                Messages.GetHRData ghrd = new Messages.GetHRData();
                byte[] data = ghrd.Serialize();
                client?.Send(data);
            }
            else
                on_pull_data(Options.SDKName);
        }
    }

    /// <summary>
    /// Open a connection to the server
    /// </summary>
    public void Open()
    {
        if (client == null && !IsSDKConnected && !IsReflected)
        {
            client = new SimpleTcpClient(Options.IpPort);
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
                        case "UpdateMessage":
                            OnSDKUpdate();
                            break;
                    }
                }
                catch(Exception){}
            };
            client?.Connect();
        }
    }

    /// <summary>
    /// Close the connection to the server
    /// </summary>
    public void Close()
    {
        if (!IsReflected)
            client?.Disconnect();
    }
    
    private void on_log(HRSDK instance, LogLevel logLevel, object msg, ConsoleColor color = ConsoleColor.White, Exception? e = null){}
    
    /// <summary>
    /// LogHelper duplication for HRSDK.
    /// Will implement native if used with Reflection.
    /// Highly recommended to use this for logging, alongside any other logger. 
    /// </summary>
    /// <param name="logLevel">The Level to Log on</param>
    /// <param name="msg">The message to log</param>
    /// <param name="color">The color of the log (ConsoleColor/UI Only)</param>
    /// <param name="e">An optional Exception (Error level log only!)</param>
    public void Log(LogLevel logLevel, object msg, ConsoleColor color = ConsoleColor.White, Exception? e = null)
    {
        string time = DateTime.Now.ToString(CultureInfo.CurrentCulture).Split(' ')[1];
        if (!IsReflected)
        {
            if (e != null && logLevel != LogLevel.Error)
                msg += " | Exception: " + e;
            /*StackFrame frame = new StackFrame(1);
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(
                        $"[{time}] [{frame.GetMethod()?.DeclaringType}:{frame.GetMethod()}] [SDK : {Options.SDKName}] (DEBUG): {msg}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Log:
                    Console.ForegroundColor = color;
                    Console.WriteLine(
                        $"[{time}] [{frame.GetMethod()?.DeclaringType}] [SDK : {Options.SDKName}] (LOG): {msg}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(
                        $"[{time}] [{frame.GetMethod()?.DeclaringType}] [SDK : {Options.SDKName}] (WARN): {msg}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    object log =
                        $"[{time}] [{frame.GetMethod()?.DeclaringType}:{frame.GetMethod()}] [SDK : {Options.SDKName}] (ERROR): {msg}";
                    if (e != null)
                        log = $"{log} | Exception: {e}";
                    Console.WriteLine(log);
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }*/
            Messages.HRLogMessage hrLogMessage = new Messages.HRLogMessage
            {
                LogLevel = logLevel,
                Message = msg.ToString(),
                Color = color
            };
            byte[] data = hrLogMessage.Serialize();
            client?.Send(data);
        }
        else
            on_log(this, logLevel, msg, color, e);
    }
    
    public enum LogLevel
    {
        Debug,
        Log,
        Warn,
        Error
    }
}