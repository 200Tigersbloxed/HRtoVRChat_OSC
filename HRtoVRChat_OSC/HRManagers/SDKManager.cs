using System.Reflection;
using HRtoVRChat_OSC_SDK;
using SuperSimpleTcp;

namespace HRtoVRChat_OSC.HRManagers;

public class SDKManager : HRManager
{
    private Thread _worker;
    private CancellationTokenSource token;

    private SimpleTcpServer server;

    private readonly string SDKsLocation =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HRtoVRChat_OSC/SDKs");
    private readonly List<ExternalHRSDK> ExternalHrsdks = new();

    private int HR;
    private bool isActive;
    private bool isOpen;
    
    public bool Init(string d1)
    {
        if (_worker != null)
        {
            token.Cancel();
        }

        token = new CancellationTokenSource();
        _worker = new Thread(() =>
        {
            server = new SimpleTcpServer(d1);
            server.Events.ClientConnected += (sender, args) => LogHelper.Log("SDK Connected!");
            server.Events.ClientDisconnected += (sender, args) => LogHelper.Log("SDK Disconnected!");
            server.Events.DataReceived += (sender, args) =>
            {
                byte[] data = args.Data;
                object? fakeDeserialize = Messages.DeserializeMessage(data);
                string messageType = Messages.GetMessageType(fakeDeserialize);
                switch (messageType)
                {
                    case "HRMessage":
                        Messages.HRMessage hrm = Messages.DeserializeMessage<Messages.HRMessage>(data);
                        HR = hrm.HR;
                        isActive = hrm.IsActive;
                        isOpen = hrm.IsOpen;
                        break;
                    case "GetHRData":
                        Messages.HRMessage hrm_ghrd = new Messages.HRMessage
                        {
                            HR = HR,
                            IsActive = isActive,
                            IsOpen = isOpen
                        };
                        server.Send(args.IpPort, hrm_ghrd.Serialize());
                        break;
                    default:
                        LogHelper.Warn("Unknown Debug Message: " + messageType);
                        break;
                }
            };
            server.Start();
            LogHelper.Debug("Started SDK Server at " + d1);
            if (!Directory.Exists(SDKsLocation))
                Directory.CreateDirectory(SDKsLocation);
            foreach (string file in Directory.GetFiles(SDKsLocation, "*.dll"))
            {
                // Attempt to load the file
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);
                    List<Type> externalHrsdks =
                        assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(ExternalHRSDK))).ToList();
                    foreach (Type externalHrsdk in externalHrsdks)
                    {
                        try
                        {
                            ExternalHRSDK loaded = (ExternalHRSDK) Activator.CreateInstance(externalHrsdk);
                            if (loaded != null)
                            {
                                if (loaded.Initialize() || loaded.OverrideInitializeAdd)
                                {
                                    LogHelper.Log("Loaded ExternalHRSDK " + loaded.SDKName);
                                    ExternalHrsdks.Add(loaded);
                                    loaded.OnHRMessageUpdated += message =>
                                    {
                                        if (message.IsActive)
                                        {
                                            HR = message.HR;
                                            isActive = message.IsActive;
                                            isOpen = message.IsOpen;
                                        }
                                    };
                                }
                                else
                                    LogHelper.Error(loaded.SDKName + " failed to initialize!");
                            }
                            else
                                LogHelper.Error("Failed to create an ExternalHRSDK under the file " + file);
                        }
                        catch (Exception e)
                        {
                            LogHelper.Error("Unknown Error while loading an ExternalHRSDK from file" + file, e);
                        }
                    }
                }
                catch (Exception ee)
                {
                    LogHelper.Error("Unknown Exception while processing an ExternalHRSDK from file " + file, ee);
                }
            }
            LogHelper.Debug("Finished loading all External SDKs");
            while (!token.IsCancellationRequested)
            {
                int c = server.GetClients().ToList().Count;
                foreach (ExternalHRSDK externalHrsdk in ExternalHrsdks)
                {
                    // Update first
                    externalHrsdk.Update();
                    // THEN do checks
                    if (externalHrsdk.CurrentHRData.IsActive)
                        c++;
                }
                if (c <= 0)
                {
                    // reset all data
                    HR = 0;
                    isOpen = false;
                    isActive = false;
                }
                Thread.Sleep(10);
            }
            server?.Stop();
            foreach (ExternalHRSDK externalHrsdk in ExternalHrsdks)
                externalHrsdk.Destroy();
        });
        _worker.Start();
        return true;
    }

    public int GetHR() => HR;

    public void Stop() => token.Cancel();

    public bool IsOpen() => isOpen;

    public bool IsActive() => isActive;
}