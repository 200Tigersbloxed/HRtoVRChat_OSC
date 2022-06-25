using System.Net.WebSockets;
using System.Text;

namespace HRtoVRChat_OSC.HRManagers
{
    public class FitbitManager : HRManager
    {
        private ClientWebSocket cws = null;
        private Thread _thread;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        private bool IsConnected => cws?.State == WebSocketState.Open;
        public bool FitbitIsConnected { get; private set; } = false;
        public int HR { get; private set; } = 0;

        public bool Init(string url)
        {
            tokenSource = new CancellationTokenSource();
            StartThread(url);
            LogHelper.Log("Initialized WebSocket!");
            return IsConnected;
        }

        int sendreceiveeror = 0;
        private async Task SendAndReceiveMessage(string message)
        {
            if(cws != null)
            {
                if(cws.State == WebSocketState.Open)
                {
                    bool didSend = false;
                    bool didReceive = false;

                    byte[] sendBody = Encoding.UTF8.GetBytes(message);
                    try
                    {
                        await cws.SendAsync(new ArraySegment<byte>(sendBody), WebSocketMessageType.Text, true, CancellationToken.None);
                        didSend = true;
                    }
                    catch(Exception e)
                    {
                        LogHelper.Error("Failed to Send Message to Fitbit Server! Exception: ", e);
                    }
                    var clientbuffer = new ArraySegment<byte>(new byte[1024]);
                    WebSocketReceiveResult result = null;
                    try
                    {
                        result = await cws.ReceiveAsync(clientbuffer, CancellationToken.None);
                        didReceive = true;
                    }
                    catch(Exception e)
                    {
                        LogHelper.Error("Failed to Recieve Message from Fitbit Server! Exception: ", e);
                    }
                    if(result != null)
                        if (result.Count != 0 || result.CloseStatus == WebSocketCloseStatus.Empty)
                        {
                            string msg = Encoding.ASCII.GetString(clientbuffer.Array);
                            if (msg.Contains("yes"))
                                FitbitIsConnected = true;
                            else if (msg.Contains("no"))
                                FitbitIsConnected = false;
                            else
                                try { HR = Convert.ToInt32(msg); } catch (Exception) { }
                        }
                    if(!(didSend && didReceive))
                    {
                        sendreceiveeror++;
                        if(sendreceiveeror >= 15)
                        {
                            await Close();
                            LogHelper.Warn("Failed to Send and Receive message too many times! Closed Socket.");
                        }
                    }
                }
            }
        }

        public void StartThread(string url)
        {
            _thread = new Thread(async () =>
            {
                cws = new ClientWebSocket();
                bool noerror = true;
                try
                {
                    await cws.ConnectAsync(new Uri(url), CancellationToken.None);
                }
                catch(Exception e)
                {
                    LogHelper.Error("Failed to connect to Fitbit Server! Exception: ", e);
                    noerror = false;
                }
                if (noerror)
                {
                    while (!tokenSource.IsCancellationRequested)
                    {
                        if(!IsConnected)
                            Stop();
                        await SendAndReceiveMessage("getHR");
                        await SendAndReceiveMessage("checkFitbitConnection");
                        Thread.Sleep(500);
                    }
                }
                await Close();
            });
            _thread.Start();
        }

        public string GetName() => "FitbitHRtoWS";

        public int GetHR() => HR;

        private async Task Close()
        {
            if (cws != null)
                if (cws.State == WebSocketState.Open)
                    try
                    {
                        await cws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client Disconnect", CancellationToken.None);
                        cws.Dispose();
                        cws = null;
                    }
                    catch(Exception e)
                    {
                        LogHelper.Error("Failed to Close connection with the Fitbit Server! Exception: ", e);
                    }
                else
                    LogHelper.Warn("WebSocket is not alive! Did you mean to Dispose()?");
            else
                LogHelper.Warn("WebSocket is null! Did you mean to Initialize()?");
        }

        public void Stop()
        {
            if (cws != null)
            {
                tokenSource.Cancel();
                LogHelper.Debug("Sent message to Stop WebSocket");
            }
            else
                LogHelper.Warn("WebSocket is already null! Did you mean to Initialize()?");
        }

        public bool IsOpen() => IsConnected && FitbitIsConnected;

        public bool IsActive() => IsConnected;
    }

    /*
    public class FitbitManager : HRManager
    {
        private Assembly websocket_sharp = null;
        private object websocket = null;

        public void get_websocketsharp_assembly() => websocket_sharp = DependencyManager.GetAssemblyByName("websocket-sharp.dll");

        public void websocket_create(string url)
        {
            if (websocket_sharp == null)
                get_websocketsharp_assembly();
            Type websocket_type = websocket_sharp.GetType("WebSocketSharp.WebSocket");
            if (websocket_type != null)
            {
                string[] protocols = new string[1];
                protocols[0] = "connection";
                object[] parameters = new object[2];
                parameters[0] = url;
                parameters[1] = (Il2CppStringArray)protocols;
                object websocket_instance = Activator.CreateInstance(websocket_type, parameters);
                if (websocket_instance != null)
                {
                    websocket = websocket_instance;
                    LogHelper.Debug("FitbitManager", "Created WebSocket Instance!");
                }
                else
                    LogHelper.Error("FitbitManager", "Failed to create WebSocket instance!");
            }
            else
                LogHelper.Error("FitbitManager", "Failed to find WebSocket Type!");
        }

        public void websocket_init()
        {
            if(websocket != null)
            {
                Type websocket_type = websocket_sharp.GetType("WebSocketSharp.WebSocket");
                if (websocket_type != null)
                {
                    // Find _message Action
                    foreach(PropertyInfo pi in websocket_type.GetProperties())
                    {
                        if(pi.Name == "_message")
                        {
                            var ac = pi.GetValue(websocket);
                            GCHandle handle1 = GCHandle.Alloc(ac);
                            Delegate hookedAc = Marshal.GetDelegateForFunctionPointer((IntPtr)handle1, typeof(object));
                            
                        }
                    }
                    // Start WebSocket
                    MethodInfo connectMethod = websocket_type.GetMethod("Connect");
                    if (connectMethod != null)
                    {
                        object[] connectParams = new object[connectMethod.GetParameters().Length];
                        connectMethod.Invoke(websocket, connectParams);
                    }
                    else
                        LogHelper.Error("FitbitManager", "Failed to Find Connect Method!");
                }
                else
                    LogHelper.Error("FitbitManager", "Failed to find WebSocket Type!");
            }
        }

        public void websocket_onmessage(object e)
        {
            // Get e Data
            string data = e.GetType().GetProperty("Data").ToString();
            LogHelper.Log("FitbitManager", "Got message: " + data);
            // Handle
            switch (data.ToLower())
            {
                case "yes":
                    FitbitIsConnected = true;
                    break;
                case "no":
                    FitbitIsConnected = false;
                    break;
                default:
                    // Assume it's the HeartRate
                    try { HR = Convert.ToInt32(data); } catch (Exception) { }
                    break;
            }
        }

        // 0 - Connecting, 1 - Open, 2 - Closing, 3 - Closed
        public int websocket_getstate()
        {
            if(websocket != null)
            {
                Type websocketstate_type = websocket_sharp.GetType("WebSocketSharp.WebSocketState");
                if (websocketstate_type != null)
                {
                    object state = websocket.GetType().GetProperty("ReadyState").GetValue(websocket);
                    object underlyingValue = Convert.ChangeType(state, Enum.GetUnderlyingType(state.GetType()));
                    return (int)underlyingValue;
                }
            }
            return -1;
        }

        public void websocket_setssl()
        {
            if(websocket != null)
            {
                Type websocketstate_type = websocket_sharp.GetType("WebSocketSharp.WebSocketState");
                if (websocketstate_type != null)
                {
                    // Get SslConfiguration Property
                    object sslconf = websocket.GetType().GetProperty("SslConfiguration").GetValue(websocket);
                    if (sslconf != null)
                    {
                        PropertyInfo[] esslp = sslconf.GetType().GetProperties();
                        if (esslp != null)
                        {
                            try
                            {
                                foreach(PropertyInfo pi in esslp)
                                {
                                    // For whatever reason, it's _enabledProtocols, not _enabledSslProtocols
                                    // And apparently, EnabledSslProtocols does not have a Set method?
                                    // So I have to set it's private property, which should work just as fine
                                    if (pi.Name == "_enabledProtocols")
                                    {
                                        MethodInfo setMethod = pi.GetSetMethod(true);
                                        object[] @params = new object[setMethod.GetParameters().Length];
                                        // idk where Authentication is :/ oh well
                                        //@params[0] = (Il2CppSystem.Security.Authentication.SslProtocols)System.Security.Authentication.SslProtocols.Tls12;
                                        @params[0] = 3072;
                                        setMethod.Invoke(sslconf, @params);
                                    }
                                }
                            }
                            catch (Exception e) { LogHelper.Error("FitbitManager", "Failed to set EnabledSslProtocols! Exception: " + e); }
                        }
                        else
                            LogHelper.Error("FitbitManager", "Failed to find EnabledSslProtocols!");
                    }
                    else
                        LogHelper.Error("FitbitManager", "Failed to find SslConfiguration!");
                }
            }
        }

        public void websocket_send(string data)
        {
            if (websocket != null)
            {
                Type websocket_type = websocket_sharp.GetType("WebSocketSharp.WebSocket");
                if (websocket_type != null)
                {
                    // Send WebSocket
                    MethodInfo sendMethod = websocket_type.GetMethod("Send");
                    if (sendMethod != null)
                    {
                        object[] sendParams = new object[sendMethod.GetParameters().Length];
                        sendParams[0] = data;
                        sendMethod.Invoke(websocket, sendParams);
                    }
                    else
                        LogHelper.Error("FitbitManager", "Failed to Find Send Method!");
                }
            }
        }

        public void websocket_close()
        {
            if(websocket != null)
            {
                Type websocket_type = websocket_sharp.GetType("WebSocketSharp.WebSocket");
                if (websocket_type != null)
                {
                    // Start WebSocket
                    MethodInfo closeMethod = websocket_type.GetMethod("Close");
                    if (closeMethod != null)
                    {
                        object[] closeParams = new object[closeMethod.GetParameters().Length];
                        closeMethod.Invoke(websocket, closeParams);
                    }
                    else
                        LogHelper.Error("FitbitManager", "Failed to Find Close Method!");
                }
                else
                    LogHelper.Error("FitbitManager", "Failed to find WebSocket Type!");
            }
        }

        public bool FitbitIsConnected { get; private set; } = false;
        public bool IsConnected
        {
            get
            {
                bool wc = false;
                if(websocket != null)
                    wc = websocket_getstate() == 2;
                return wc;
            }
        }
        public int HR { get; private set; } = 0;

        private Thread _thread = null;

        public bool Init(string url)
        {
            get_websocketsharp_assembly();
            StartThread(url);
            if (IsConnected)
                return false;
            LogHelper.Log("FitbitManager", "WebSocket Initialized!");
            return IsConnected;
        }

        void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
        }

        class WebSocketURIInfo
        {
            public bool isValid = false;
            public bool isSecure = false;
        }

        bool validateIP(string ip)
        {
            IPAddress ipAddress = null;
            bool isValidIp = IPAddress.TryParse(ip, out ipAddress);
            return isValidIp;
        }

        WebSocketURIInfo GetSocketInfo(string url)
        {
            WebSocketURIInfo wsurii = new WebSocketURIInfo();
            wsurii.isValid = url.Contains("ws://") || url.Contains("wss://");
            try
            {
                string[] colonSplit = url.Split(':');
                wsurii.isSecure = colonSplit[0] == "wss";
            }
            catch (Exception)
            {
                wsurii.isValid = false;
            }

            return wsurii;
        }

        void StartThread(string url)
        {
            VerifyClosedThread();
            _thread = new Thread(() =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                WebSocketURIInfo wsurii = GetSocketInfo(url);
                if (wsurii.isValid)
                {
                    if(websocket != null)
                    {
                        if(websocket_getstate() == 2)
                            websocket_close();
                        websocket = null;
                    }
                    websocket_create(url);
                    if (wsurii.isSecure)
                        websocket_setssl();
                    LogHelper.Debug("FitbitManager", "isValid: " + wsurii.isValid + ", isSecure: " + wsurii.isSecure);
                    websocket_init();
                }
                else
                {
                    LogHelper.Error("FitbitManager", "The WebSocket URI is invalid!");
                }
                while (IsConnected)
                {
                    getHRMessage();
                    getFitbitConnectionMessage();
                    Thread.Sleep(500);
                }
            });
            _thread.Start();
        }

        private void getHRMessage()
        {
            if (websocket != null)
                if (websocket_getstate() == 2)
                    websocket_send("getHR");
        }

        private void getFitbitConnectionMessage()
        {
            if (websocket != null)
                if (websocket_getstate() == 2)
                    websocket_send("checkFitbitConnection");
        }

        public int GetHR() => HR;

        private void Close()
        {
            if (websocket != null)
                if (websocket_getstate() == 2)
                    websocket_close();
                else
                    LogHelper.Warn("FitbitManager", "WebSocket is not alive! Did you mean to Dispose()?");
            else
                LogHelper.Warn("FitbitManager", "WebSocket is null! Did you mean to Initialize()?");
        }

        public void Stop()
        {
            if (websocket != null)
            {
                if (websocket_getstate() == 2)
                {
                    Close();
                    websocket = null;
                }
                else
                    websocket = null;
                VerifyClosedThread();
            }
            else
                LogHelper.Warn("FitbitManager", "WebSocket is already null! Did you mean to Initialize()?");
        }

        public bool IsOpen() => IsConnected && FitbitIsConnected;

        public bool IsActive() => IsConnected;
    }
    */

    /*
    public class FitbitManager : HRManager
    {
        private WebSocket ws = null;
        public bool FitbitIsConnected { get; private set; } = false;
        public int HR { get; private set; } = 0;

        public bool isConnected { get; private set; }

        private Thread _thread = null;

        public bool Init(string url)
        {
            StartThread(url);
            if (isConnected)
                return false;
            LogHelper.Log("FitbitManager", "WebSocket Initialized!");
            return isConnected;
        }

        void VerifyClosedThread()
        {
            if(_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
        }

        class WebSocketURIInfo
        {
            public bool isValid = false;
            public bool isSecure = false;
        }

        bool validateIP(string ip)
        {
            IPAddress ipAddress = null;
            bool isValidIp = IPAddress.TryParse(ip, out ipAddress);
            return isValidIp;
        }

        WebSocketURIInfo GetSocketInfo(string url)
        {
            WebSocketURIInfo wsurii = new WebSocketURIInfo();
            wsurii.isValid = url.Contains("ws://") || url.Contains("wss://");
            try
            {
                string[] colonSplit = url.Split(':');
                wsurii.isSecure = colonSplit[0] == "wss";
            }
            catch (Exception)
            {
                wsurii.isValid = false;
            }

            return wsurii;
        }

        void StartThread(string url)
        {
            VerifyClosedThread();
            _thread = new Thread(() =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                WebSocketURIInfo wsurii = GetSocketInfo(url);
                if (wsurii.isValid)
                {
                    if (ws != null)
                    {
                        Close();
                    }
                    ws = new WebSocket(url);
                    if(wsurii.isSecure)
                        ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                    LogHelper.Debug("FitbitManager", "isValid: " + wsurii.isValid + ", isSecure: " + wsurii.isSecure);
                    ws.OnOpen += Ws_OnOpen;
                    ws.OnMessage += OnMessage;
                    ws.OnClose += Ws_OnClose;
                    ws.Connect();
                }
                else
                {
                    LogHelper.Error("FitbitManager", "The WebSocket URI is invalid!");
                }
                while (isConnected)
                {
                    getHRMessage();
                    getFitbitConnectionMessage();
                    Thread.Sleep(500);
                }
            });
            _thread.Start();
        }

        private void getHRMessage()
        {
            if(ws != null)
                if (ws.ReadyState == WebSocketState.Open)
                    ws.Send("getHR");
        }

        private void getFitbitConnectionMessage()
        {
            if (ws != null)
                if (ws.ReadyState == WebSocketState.Open)
                    ws.Send("checkFitbitConnection");
        }

        public int GetHR() => HR;

        private void Close()
        {
            if (ws != null)
                if (ws.ReadyState == WebSocketState.Open)
                    ws.Close();
                else
                    LogHelper.Warn("FitbitManager", "WebSocket is not alive! Did you mean to Dispose()?");
            else
                LogHelper.Warn("FitbitManager", "WebSocket is null! Did you mean to Initialize()?");
        }

        public void Stop()
        {
            if (ws != null)
            {
                if (ws.ReadyState == WebSocketState.Open)
                {
                    Close();
                    ws = null;
                }
                else
                    ws = null;
                VerifyClosedThread();
            }
            else
                LogHelper.Warn("FitbitManager", "WebSocket is already null! Did you mean to Initialize()?");
        }

        public bool IsOpen() => isConnected && FitbitIsConnected;

        public bool IsActive() => isConnected;

        private void OnMessage(object sender, MessageEventArgs e)
        {
            switch (e.Data.ToLower())
            {
                case "yes":
                    FitbitIsConnected = true;
                    break;
                case "no":
                    FitbitIsConnected = false;
                    break;
                default:
                    // Assume it's the HeartRate
                    try { HR = Convert.ToInt32(e.Data); } catch (Exception) { I have no clue what this is then }
                    break;
            }
        }

        private void Ws_OnOpen(object sender, EventArgs e) => isConnected = true;
        private void Ws_OnClose(object sender, CloseEventArgs e) => isConnected = false;
    }
    */
}
