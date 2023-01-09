using System.Reflection;
using HarmonyLib;
using HRtoVRChat_OSC_SDK;
using SuperSimpleTcp;

namespace HRtoVRChat_OSC.HRManagers;

public class SDKManager : HRManager
{
    public static string PreferredSDK = String.Empty;
    
    private Thread _worker;
    private CancellationTokenSource token;

    private SimpleTcpServer server;

    public static readonly string SDKsLocation = "SDKs";
    private readonly Dictionary<ExternalHRSDK, Messages.HRMessage> ExternalHrsdks = new();
    private readonly Dictionary<HRSDK, Messages.HRMessage> Hrsdks = new();
    private readonly Dictionary<string, Messages.HRMessage> RemoteSDKs = new();

    private void SetHRDataBySDKName(string name, Messages.HRMessage hrm)
    {
        // Remote
        foreach (KeyValuePair<string,Messages.HRMessage> keyValuePair in RemoteSDKs)
        {
            if (keyValuePair.Value.SDKName == name)
                RemoteSDKs[keyValuePair.Key] = hrm;
        }
        // ExternalHRSDK (Obsolete)
        foreach (KeyValuePair<ExternalHRSDK,Messages.HRMessage> externalHrsdk in ExternalHrsdks)
        {
            if (externalHrsdk.Key.SDKName == name)
                ExternalHrsdks[externalHrsdk.Key] = hrm;
        }
        // HRSDK
        foreach (KeyValuePair<HRSDK,Messages.HRMessage> hrsdk in Hrsdks)
        {
            if (hrsdk.Key.Options.SDKName == name)
                Hrsdks[hrsdk.Key] = hrm;
        }
    }

    private void SendDataToSDK(string name, Messages.HRMessage data)
    {
        // Remote
        foreach (KeyValuePair<string,Messages.HRMessage> keyValuePair in RemoteSDKs)
        {
            if (keyValuePair.Value.SDKName == name)
            {
                // Serialize
                byte[] serialize = data.Serialize();
                // Send
                server.Send(keyValuePair.Key, serialize);
            }
        }
        // ExternalHRSDK (Obsolete) is not supported
        // HRSDK
        foreach (KeyValuePair<HRSDK,Messages.HRMessage> hrsdk in Hrsdks)
        {
            if (hrsdk.Key.Options.SDKName == name)
                hrsdk.Key.OnSDKData(data);
        }
    }

    private Messages.HRMessage? GetRemoteHRDataBySDKName(string name)
    {
        // Remote
        foreach (KeyValuePair<string,Messages.HRMessage> keyValuePair in RemoteSDKs)
        {
            if (keyValuePair.Value.SDKName == name)
                return RemoteSDKs[keyValuePair.Key];
        }
        // ExternalHRSDK (Obsolete)
        foreach (KeyValuePair<ExternalHRSDK,Messages.HRMessage> externalHrsdk in ExternalHrsdks)
        {
            if (externalHrsdk.Key.SDKName == name)
                return ExternalHrsdks[externalHrsdk.Key];
        }
        // HRSDK
        foreach (KeyValuePair<HRSDK,Messages.HRMessage> hrsdk in Hrsdks)
        {
            if (hrsdk.Key.Options.SDKName == name)
                return Hrsdks[hrsdk.Key];
        }
        return null;
    }

    private Messages.HRMessage? GetPreferredHRData()
    {
        if (!string.IsNullOrEmpty(PreferredSDK))
        {
            Messages.HRMessage? target = GetRemoteHRDataBySDKName(PreferredSDK);
            if (target != null)
                return target;
        }
        // If the above is false, or returns null, then select automatically
        // Prefer Internal
        foreach (KeyValuePair<HRSDK,Messages.HRMessage> keyValuePair in Hrsdks)
        {
            if (keyValuePair.Value.IsActive)
                return keyValuePair.Value;
        }
        foreach (KeyValuePair<ExternalHRSDK,Messages.HRMessage> keyValuePair in ExternalHrsdks)
        {
            if (keyValuePair.Value.IsActive)
                return keyValuePair.Value;
        }
        // Then Remote
        foreach (KeyValuePair<string,Messages.HRMessage> keyValuePair in RemoteSDKs)
        {
            if (keyValuePair.Value.IsActive)
                return keyValuePair.Value;
        }
        return null;
    }

    public void DestroySDKByName(string name)
    {
        // Remote
        foreach (KeyValuePair<string,Messages.HRMessage> keyValuePair in RemoteSDKs)
        {
            if (keyValuePair.Value.SDKName == name)
            {
                server.DisconnectClient(keyValuePair.Key);
                LogHelper.Debug("Disconnected Remote HRSDK " + name);
            }
        }
        // ExternalHRSDK (Obsolete)
        foreach (KeyValuePair<ExternalHRSDK,Messages.HRMessage> externalHrsdk in ExternalHrsdks)
        {
            if (externalHrsdk.Key.SDKName == name)
            {
                externalHrsdk.Key.Destroy();
                try
                {
                    ExternalHrsdks.Remove(externalHrsdk.Key);
                    LogHelper.Debug("Destroyed ExternalHRSDK " + name);
                }
                catch(Exception e) {LogHelper.Error("Failed to Dispose ExternalHRSDK " + externalHrsdk.Key.SDKName + "!", e);}
            }
        }
        // HRSDK
        foreach (KeyValuePair<HRSDK,Messages.HRMessage> hrsdk in Hrsdks)
        {
            if (hrsdk.Key.Options.SDKName == name)
            {
                hrsdk.Key.OnSDKClosed();
                try
                {
                    Hrsdks.Remove(hrsdk.Key);
                    LogHelper.Debug("Disconnected HRSDK " + name);
                }
                catch(Exception e) {LogHelper.Error("Failed to Dispose ExternalHRSDK " + hrsdk.Key.Options.SDKName + "!", e);}
            }
        }
    }

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
            server.Events.ClientConnected += (sender, args) =>
            {
                RemoteSDKs.Add(args.IpPort, new Messages.HRMessage());
                LogHelper.Log("SDK Connected!");
            };
            server.Events.ClientDisconnected += (sender, args) =>
            {
                try
                {
                    RemoteSDKs.Remove(args.IpPort);
                }
                catch(Exception e) {LogHelper.Error("Failed to remove RemoteSDK!", e);}
                LogHelper.Log("SDK Disconnected!");
            };
            server.Events.DataReceived += (sender, args) =>
            {
                byte[] data = args.Data;
                object? fakeDeserialize = Messages.DeserializeMessage(data);
                string messageType = Messages.GetMessageType(fakeDeserialize);
                switch (messageType)
                {
                    case "HRMessage":
                        Messages.HRMessage hrm = Messages.DeserializeMessage<Messages.HRMessage>(data);
                        foreach (KeyValuePair<string,Messages.HRMessage> remoteSdK in RemoteSDKs)
                        {
                            if (remoteSdK.Key == args.IpPort)
                                RemoteSDKs[remoteSdK.Key] = hrm;
                        }
                        break;
                    case "HRLogMessage":
                        Messages.HRLogMessage hrLogMessage = Messages.DeserializeMessage<Messages.HRLogMessage>(data);
                        switch (hrLogMessage.LogLevel)
                        {
                            case HRSDK.LogLevel.Debug:
                                LogHelper.Debug(hrLogMessage.Message);
                                break;
                            case HRSDK.LogLevel.Log:
                                LogHelper.Log(hrLogMessage.Message, hrLogMessage.Color);
                                break;
                            case HRSDK.LogLevel.Warn:
                                LogHelper.Log(hrLogMessage.Message);
                                break;
                            case HRSDK.LogLevel.Error:
                                LogHelper.Error(hrLogMessage.Message);
                                break;
                        }
                        break;
                    case "GetHRData":
                        Messages.HRMessage hrm_ghrd = new Messages.HRMessage
                        {
                            SDKName = GetPreferredHRData()?.SDKName ?? "unknown",
                            HR = GetHR(),
                            IsActive = IsActive(),
                            IsOpen = IsOpen()
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
            SDKPatches.OnHRData += message =>
            {
                if (message.IsActive)
                    SetHRDataBySDKName(message.SDKName, message);
            };
            SDKPatches.OnRequestData += sdk =>
            {
                Messages.HRMessage hrm = new Messages.HRMessage
                {
                    HR = GetHR(),
                    IsActive = IsActive(),
                    IsOpen = IsOpen()
                };
                SendDataToSDK(sdk, hrm);
            };
            foreach (string file in Directory.GetFiles(SDKsLocation, "*.dll"))
            {
                // Attempt to load the file
                try
                {
                    Assembly assembly = Assembly.LoadFile(Path.GetFullPath(file));
                    List<Type> externalHrsdks =
                        assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(ExternalHRSDK))).ToList();
                    List<Type> HRSDKs = assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(HRSDK))).ToList();
                    // ExternalHRSDK (Obsolete)
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
                                    ExternalHrsdks.Add(loaded, loaded.CurrentHRData);
                                    loaded.OnHRMessageUpdated += message =>
                                    {
                                        if (message.IsActive)
                                            SetHRDataBySDKName(message.SDKName, message);
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
                    // HRSDK
                    foreach (Type hrsdk in HRSDKs)
                    {
                        try
                        {
                            HRSDK loaded = (HRSDK) Activator.CreateInstance(hrsdk);
                            if (loaded != null)
                            {
                                // Reflection
                                loaded.GetType().BaseType
                                    .GetField("_isReflected", BindingFlags.NonPublic | BindingFlags.Instance)
                                    .SetValue(loaded, true);
                                // Harmony Patches
                                Harmony sdkPatch = new Harmony(loaded.Options.SDKName + "-patch");
                                sdkPatch.Patch(
                                    typeof(HRSDK).GetMethod("on_push_data",
                                        BindingFlags.Instance | BindingFlags.NonPublic),
                                    new HarmonyMethod(typeof(SDKPatches).GetMethod("on_push_data",
                                        BindingFlags.Static | BindingFlags.NonPublic)));
                                sdkPatch.Patch(
                                    typeof(HRSDK).GetMethod("on_pull_data",
                                        BindingFlags.Instance | BindingFlags.NonPublic),
                                    new HarmonyMethod(typeof(SDKPatches).GetMethod("on_pull_data",
                                        BindingFlags.Static | BindingFlags.NonPublic)));
                                sdkPatch.Patch(
                                    typeof(HRSDK).GetMethod("on_log",
                                        BindingFlags.Instance | BindingFlags.NonPublic),
                                    new HarmonyMethod(typeof(SDKPatches).GetMethod("on_log",
                                        BindingFlags.Static | BindingFlags.NonPublic)));
                                LogHelper.Log("Loaded HRSDK " + loaded.Options.SDKName);
                                Hrsdks.Add(loaded, new Messages.HRMessage{SDKName = loaded.Options.SDKName});
                                loaded.OnSDKOpened();
                            }
                            else
                                LogHelper.Error("Failed to create an HRSDK under the file " + file);
                        }
                        catch (Exception e)
                        {
                            LogHelper.Error("Unknown Error while loading an HRSDK from file" + file, e);
                        }
                    }
                }
                catch (Exception ee)
                {
                    LogHelper.Error("Unknown Exception while processing an HRSDK from file " + file, ee);
                }
            }
            LogHelper.Debug("Finished loading all HRSDKs");
            int networked_update = 0;
            while (!token.IsCancellationRequested)
            {
                int c = 0;
                foreach (string client in server.GetClients())
                {
                    try
                    {
                        if (networked_update >= 10)
                        {
                            server.Send(client, new Messages.UpdateMessage().Serialize());
                            networked_update = 0;
                        }
                        else
                            networked_update++;
                        c++;
                    }
                    catch(Exception){LogHelper.Warn("Socket Connection was closed when sending update!");}
                }
                foreach (KeyValuePair<ExternalHRSDK,Messages.HRMessage> externalHrsdk in ExternalHrsdks)
                {
                    // Update first
                    externalHrsdk.Key.Update();
                    // THEN do checks
                    if (externalHrsdk.Key.CurrentHRData.IsActive)
                        c++;
                }
                foreach (KeyValuePair<HRSDK,Messages.HRMessage> hrsdk in Hrsdks)
                {
                    // Update first
                    hrsdk.Key.OnSDKUpdate();
                    // THEN do checks
                    if (hrsdk.Key.IsActive)
                        c++;
                }
                Thread.Sleep(10);
            }
            server?.Stop();
            foreach (KeyValuePair<HRSDK,Messages.HRMessage> hrsdk in Hrsdks)
                hrsdk.Key.OnSDKClosed();
            Hrsdks.Clear();
            foreach (KeyValuePair<ExternalHRSDK,Messages.HRMessage> externalHrsdk in ExternalHrsdks)
                externalHrsdk.Key.Destroy();
            ExternalHrsdks.Clear();
        });
        _worker.Start();
        return true;
    }

    public string GetName()
    {
        Messages.HRMessage? hrm = GetPreferredHRData();
        return hrm?.SDKName ?? "sdk";
    }

    public int GetHR() => GetPreferredHRData()?.HR ?? 0;

    public void Stop() => token.Cancel();

    public bool IsOpen() => GetPreferredHRData()?.IsOpen ?? false;

    public bool IsActive() => GetPreferredHRData()?.IsActive ?? false;
}

public class SDKPatches
{
    public static Action<Messages.HRMessage> OnHRData = message => { };
    public static Action<string> OnRequestData = requesting_sdk => { };

    private static void on_push_data(Messages.HRMessage hrm) => OnHRData.Invoke(hrm);
    private static void on_pull_data(string requesting_sdk) => OnRequestData.Invoke(requesting_sdk);

    private static void on_log(HRSDK instance, HRSDK.LogLevel logLevel, object msg, ConsoleColor color = ConsoleColor.White, Exception? e = null)
    {
        switch (logLevel)
        {
            case HRSDK.LogLevel.Debug:
                LogHelper.Debug($"[SDK : {instance.Options.SDKName}] {msg}");
                break;
            case HRSDK.LogLevel.Log:
                LogHelper.Log($"[SDK : {instance.Options.SDKName}] {msg}", color);
                break;
            case HRSDK.LogLevel.Warn:
                LogHelper.Warn($"[SDK : {instance.Options.SDKName}] {msg}");
                break;
            case HRSDK.LogLevel.Error:
                if(e != null)
                    LogHelper.Error($"[SDK : {instance.Options.SDKName}] {msg}", e);
                else
                    LogHelper.Error($"[SDK : {instance.Options.SDKName}] {msg}");
                break;
        }
    }
}