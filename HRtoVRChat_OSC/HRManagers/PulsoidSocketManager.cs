using Newtonsoft.Json.Linq;

namespace HRtoVRChat_OSC.HRManagers
{
    class PulsoidSocketManager : HRManager
    {
        CancellationTokenSource shouldUpdate = new CancellationTokenSource();
        string pubUrl = String.Empty;
        int HR = 0;

        private Thread? _thread = null;

        private WebsocketTemplate? wst;

        public bool Init(string url)
        {
            shouldUpdate = new CancellationTokenSource();
            pubUrl = "wss://dev.pulsoid.net/api/v1/data/real_time?access_token=" + url;
            StartThread();
            LogHelper.Log("PulsoidSocketManager Initialized!");
            return true;
        }

        void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    Stop();
            }
        }

        void StartThread()
        {
            VerifyClosedThread();
            _thread = new Thread(async () =>
            {
                wst = new WebsocketTemplate(pubUrl);
                await wst.Start();
                while (!shouldUpdate.IsCancellationRequested)
                {
                    // old method RIP ;(
                    /*
                    int parsedHR = default(int);
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(pubUrl);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        // Now Parse the Information
                        JObject jo = null;
                        try
                        {
                            jo = JObject.Parse(responseBody);
                        }
                        catch (Exception e)
                        {
                            LogHelper.Error("APIReaderManager", "Failed to parse JObject! Exception: " + e);
                        }
                        if (jo != null)
                            try
                            {
                                parsedHR = jo["bpm"].Value<int>();
                            }
                            catch (Exception)
                            {
                                // try and just parse the raw text
                                try
                                {
                                    parsedHR = Convert.ToInt32(responseBody);
                                }
                                catch (Exception){}
                            }
                    }
                    catch (HttpRequestException e)
                    {
                        LogHelper.Error("APIReaderManager", "Failed to get HttpRequest! Exception: " + e);
                    }
                    HR = parsedHR;
                    */
                    int parsedHR = default(int);
                    if (wst != null && wst.IsAlive)
                    {
                        // Update stuff
                        string data = await wst.ReceiveMessage();
                        if (!string.IsNullOrEmpty(data))
                        {
                            // Parse HR
                            JObject jo = null;
                            try
                            {
                                jo = JObject.Parse(data);
                            }
                            catch (Exception e)
                            {
                                LogHelper.Error("Failed to parse JObject! Exception: " + e);
                            }
                            if (jo != null)
                                try
                                {
                                    parsedHR = jo["data"]["heart_rate"].Value<int>();
                                }
                                catch (Exception)
                                {
                                    LogHelper.Error("Failed to parse Herat Rate!");
                                }
                        }
                    }

                    HR = parsedHR;
                }
            });
            _thread.Start();
        }

        public void Stop()
        {
            shouldUpdate.Cancel();
            VerifyClosedThread();
        }

        public string GetName() => "Pulsoid";
        public int GetHR() => HR;
        public bool IsOpen() => (wst?.IsAlive ?? false) && HR > 0;
        public bool IsActive() => wst?.IsAlive ?? false;
    }
}
