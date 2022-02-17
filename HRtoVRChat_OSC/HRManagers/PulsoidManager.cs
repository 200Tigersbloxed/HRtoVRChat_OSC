using Newtonsoft.Json.Linq;

namespace HRtoVRChat_OSC.HRManagers
{
    public class PulsoidManager : HRManager
    {
        private WebsocketTemplate wst;
        private bool shouldUpdate;
        private Thread _thread;
        
        public int HR { get; private set; }
        public string Timestamp { get; private set; }
        
        private bool IsConnected => wst?.IsAlive ?? false;
        
        public bool Init(string d1)
        {
            shouldUpdate = true;
            StartThread(d1);
            LogHelper.Log("Started Pulsoid!");
            return true;
        }
        
        void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
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

        private void StartThread(string id)
        {
            VerifyClosedThread();
            _thread = new Thread(async () =>
            {
                bool noerror = true;
                wst = new WebsocketTemplate("wss://hrproxy.fortnite.lol:2096/hrproxy");
                try
                {
                    await wst.Start();
                }
                catch (Exception)
                {
                    LogHelper.Error("Failed to start Pulsoid!");
                    noerror = false;
                }

                if (noerror)
                {
                    await wst.SendMessage("{\"reader\": \"pulsoid\", \"identifier\": \"" + id + "\"}");
                    while (shouldUpdate)
                    {
                        if (IsConnected)
                        {
                            string message = await wst.ReceiveMessage();
                            if (!string.IsNullOrEmpty(message))
                                HandleMessage(message);
                        }
                        Thread.Sleep(1);
                    }
                    await Close();
                    LogHelper.Log("Closed Pulsoid");
                }
                _thread?.Abort();
            });
            _thread.Start();
        }

        private async Task Close()
        {
            await wst?.Stop();
        }

        public int GetHR() => HR;

        public void Stop()
        {
            shouldUpdate = false;
            LogHelper.Debug("Sent message to Stop Pulsoid");
        }

        public bool IsOpen() => IsConnected;

        public bool IsActive() => IsConnected;
    }
}