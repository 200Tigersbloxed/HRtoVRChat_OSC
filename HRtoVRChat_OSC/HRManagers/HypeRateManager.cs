using Newtonsoft.Json.Linq;

namespace HRtoVRChat_OSC.HRManagers
{
    public class HypeRateManager : HRManager
    {
        private WebsocketTemplate wst;
        private Thread _thread;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        private bool IsConnected
        {
            get
            {
                if (wst != null)
                {
                    return wst.IsAlive;
                }
                return false;
            }
        }
        public int HR { get; private set; }
        public string Timestamp { get; private set; }

        public bool Init(string id)
        {
            tokenSource = new CancellationTokenSource();
            StartThread(id);
            LogHelper.Log("Initialized WebSocket!");
            return IsConnected;
        }

        private async void HandleMessage(string message)
        {
            try
            {
                // Parse the message and get the HR or Pong
                JObject jo = JObject.Parse(message);
                if (jo["method"] != null)
                {
                    string pingId = jo["pingId"]?.Value<string>();
                    await wst.SendMessage("{\"method\": \"pong\", \"pingId\": \"" + pingId + "\"}");
                }
                else
                {
                    HR = Convert.ToInt32(jo["hr"].Value<string>());
                    Timestamp = jo["timestamp"].Value<string>();
                }
            }
            catch (Exception) { }
        }

        public void StartThread(string id)
        {
            _thread = new Thread(async () =>
            {
                wst = new WebsocketTemplate("wss://hrproxy.fortnite.lol:2096/hrproxy");
                bool noerror = true;
                try
                {
                    await wst.Start();
                }
                catch(Exception e)
                {
                    LogHelper.Error("Failed to connect to HypeRate server!", e);
                    noerror = false;
                }
                if (noerror)
                {
                    await wst.SendMessage("{\"reader\": \"hyperate\", \"identifier\": \"" + id + "\"}");
                    while (!tokenSource.IsCancellationRequested)
                    {
                        if (IsConnected)
                        {
                            string message = await wst.ReceiveMessage();
                            if (!string.IsNullOrEmpty(message))
                                HandleMessage(message);
                        }
                        else
                        {
                            // Stop and Restart
                            Program.RestartHRListener();
                        }
                        Thread.Sleep(1);
                    }
                }
                await Close();
                LogHelper.Log("Closed HypeRate");
            });
            _thread.Start();
        }

        public int GetHR() => HR;

        private async Task Close()
        {
            if (wst != null)
                if (wst.IsAlive)
                    try
                    {
                        await wst.Stop();
                        wst = null;
                    }
                    catch(Exception e)
                    {
                        LogHelper.Error("Failed to close connection to HypeRate Server! Exception: ", e);
                    }
                else
                    LogHelper.Warn("WebSocket is not alive! Did you mean to Dispose()?");
            else
                LogHelper.Warn("WebSocket is null! Did you mean to Initialize()?");
        }

        public void Stop()
        {
            if (wst != null)
            {
                tokenSource.Cancel();
                LogHelper.Debug("Sent message to Stop WebSocket");
            }
            else
                LogHelper.Warn("WebSocket is already null! Did you mean to Initialize()?");
        }

        public bool IsOpen() => IsConnected;

        public bool IsActive() => IsConnected;
    }

    /*
    public class HypeRateManager : HRManager
    {
        private object hypeRate = null;
        private Assembly hypeRate_assembly = null;

        public void hyperate_create(string id)
        {
            if (hypeRate_assembly == null)
                hypeRate_assembly = DependencyManager.GetAssemblyByName("HypeRate.NET.dll");
            Type hypeRate_type = hypeRate_assembly.GetType("HypeRate.NET.HeartRate");
            if (hypeRate_type != null)
            {
                object[] parameters = new object[hypeRate_type.GetConstructors()[0].GetParameters().Length];
                parameters[0] = id;
                object hypeRate_instance = Activator.CreateInstance(hypeRate_type, parameters);
                if (hypeRate_instance != null)
                {
                    hypeRate = hypeRate_instance;
                    LogHelper.Debug("HypeRateManager", "Created hypeRate Instance!");
                }
                else
                    LogHelper.Error("HypeRateManager", "Failed to create HeartRate Instance!");
            }
            else
                LogHelper.Error("HypeRateManager", "Failed to find HeartRate Type!");
        }

        public void hyperate_subscribe()
        {
            if(hypeRate != null)
            {
                Type hypeRate_type = hypeRate_assembly.GetType("HypeRate.NET.HeartRate");
                if (hypeRate_type != null)
                {
                    MethodInfo subMethod = hypeRate_type.GetMethod("Subscribe");
                    if(subMethod != null)
                    {
                        object[] subParameters = new object[subMethod.GetParameters().Length];
                        subMethod.Invoke(hypeRate, subParameters);
                    }
                    else
                        LogHelper.Error("HypeRateManager", "Failed to find Subscribe method!");
                }
                else
                    LogHelper.Error("HypeRateManager", "Failed to find HeartRate Type!");
            }
        }

        public void hyperate_unsubscribe()
        {
            if (hypeRate != null)
            {
                Type hypeRate_type = hypeRate_assembly.GetType("HypeRate.NET.HeartRate");
                if (hypeRate_type != null)
                {
                    MethodInfo unsubMethod = hypeRate_type.GetMethod("Unsubscribe");
                    if (unsubMethod != null)
                    {
                        object[] unsubParameters = new object[unsubMethod.GetParameters().Length];
                        unsubMethod.Invoke(hypeRate, unsubParameters);
                    }
                    else
                        LogHelper.Error("HypeRateManager", "Failed to find Unsubscribe method!");
                }
                else
                    LogHelper.Error("HypeRateManager", "Failed to find HeartRate Type!");
            }
        }

        public int hyperate_gethr()
        {
            int hr = 0;
            if(hypeRate != null)
            {
                Type hypeRate_type = hypeRate_assembly.GetType("HypeRate.NET.HeartRate");
                if (hypeRate_type != null)
                {
                    PropertyInfo hrp = hypeRate_type.GetProperty("HR");
                    if (hrp != null)
                    {
                        object value = null;
                        try
                        {
                            hrp.GetValue(hypeRate);
                            hr = (int)value;
                        }
                        catch (Exception e) { LogHelper.Error("HypeRateManager", "Failed to get or convert HR value! Exception: " + e); }
                    }
                    else
                        LogHelper.Error("HypeRateManager", "Failed to find HR Property!");
                }
                else
                    LogHelper.Error("HypeRateManager", "Failed to find HeartRate Type!");
            }
            return hr;
        }

        public bool hyperate_getissubscribed()
        {
            bool issubscribed = false;
            if (hypeRate != null)
            {
                Type hypeRate_type = hypeRate_assembly.GetType("HypeRate.NET.HeartRate");
                if (hypeRate_type != null)
                {
                    PropertyInfo issub = hypeRate_type.GetProperty("isSubscribed");
                    if (issub != null)
                    {
                        object value = null;
                        try
                        {
                            issub.GetValue(hypeRate);
                            issubscribed = (bool)value;
                        }
                        catch (Exception e) { LogHelper.Error("HypeRateManager", "Failed to get or convert isSubscribed value! Exception: " + e); }
                    }
                    else
                        LogHelper.Error("HypeRateManager", "Failed to find isSubscribed Property!");
                }
                else
                    LogHelper.Error("HypeRateManager", "Failed to find HeartRate Type!");
            }
            return issubscribed;
        }

        private Thread _thread = null;
        private int forwardedHR = 0;

        public bool Init(string sessionId)
        {
            StartThread(sessionId);
            return IsOpen();
        }

        void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
        }

        void StartThread(string sessionId)
        {
            VerifyClosedThread();
            _thread = new Thread(() =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                if (hypeRate == null)
                {
                    hyperate_create(sessionId);
                    LogHelper.Log("HypeRateManager", "HypeRate Initialized!");
                    Subscribe();
                }
                else
                    LogHelper.Warn("HypeRateManager", "hypeRate already initialized! Please Unsubscribe() then Dispose() before continuing!");
                while (hyperate_getissubscribed())
                {
                    forwardedHR = hyperate_gethr();
                    Thread.Sleep(10);
                }
            });
            _thread.Start();
        }

        private void Subscribe()
        {
            if (hypeRate != null)
            {
                hyperate_subscribe();
                LogHelper.Log("HypeRateManager", "Subscribed to HypeRate Data!");
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate is null! Did you Initialize()?");
        }

        public int GetHR() => forwardedHR;

        public void Stop()
        {
            if (hypeRate != null)
            {
                hyperate_unsubscribe();
                LogHelper.Log("HypeRateManager", "Unsubscribed from HypeRate Data!");
                hypeRate = null;
                VerifyClosedThread();
                forwardedHR = 0;
                LogHelper.Log("HypeRateManager", "HypeRate disposed!");
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate is already Disposed! Did you mean to Initialize()?");
        }

        public bool IsOpen()
        {
            if (hypeRate != null)
                return hyperate_getissubscribed();
            else
                return false;
        }

        public bool IsActive() => IsOpen();
    }
    */

    /*
    public class HypeRateManager : HRManager
    {
        public HeartRate hypeRate;
        private Thread _thread = null;
        private int forwardedHR = 0;

        public bool Init(string sessionId)
        {
            StartThread(sessionId);
            return IsOpen();
        }

        void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
        }

        void StartThread(string sessionId)
        {
            VerifyClosedThread();
            _thread = new Thread(() =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                if (hypeRate == null)
                {
                    hypeRate = new HeartRate(sessionId);
                    LogHelper.Log("HypeRateManager", "HypeRate Initialized!");
                    Subscribe();
                }
                else
                    LogHelper.Warn("HypeRateManager", "hypeRate already initialized! Please Unsubscribe() then Dispose() before continuing!");
                while (hypeRate.isSubscribed)
                {
                    forwardedHR = hypeRate.HR;
                    Thread.Sleep(10);
                }
            });
            _thread.Start();
        }

        private void Subscribe()
        {
            if (hypeRate != null)
            {
                hypeRate.Subscribe();
                LogHelper.Log("HypeRateManager", "Subscribed to HypeRate Data!");
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate is null! Did you Initialize()?");
        }

        public int GetHR() => forwardedHR;

        public void Stop()
        {
            if (hypeRate != null)
            {
                hypeRate.Unsubscribe();
                LogHelper.Log("HypeRateManager", "Unsubscribed from HypeRate Data!");
                hypeRate = null;
                VerifyClosedThread();
                forwardedHR = 0;
                LogHelper.Log("HypeRateManager", "HypeRate disposed!");
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate is already Disposed! Did you mean to Initialize()?");
        }

        public bool IsOpen()
        {
            if (hypeRate != null)
                return hypeRate.isSubscribed;
            else
                return false;
        }

        public bool IsActive() => IsOpen();
    }
    */
}
