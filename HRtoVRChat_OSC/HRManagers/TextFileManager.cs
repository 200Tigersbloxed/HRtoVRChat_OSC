namespace HRtoVRChat_OSC.HRManagers
{
    class TextFileManager : HRManager
    {
        bool shouldUpdate = false;
        string pubFe = String.Empty;
        int HR = 0;

        private Thread _thread = null;

        bool HRManager.Init(string fileLocation)
        {
            bool fe = File.Exists(fileLocation);
            if (fe)
            {
                LogHelper.Log("Found text file!");
                pubFe = fileLocation;
                shouldUpdate = true;
                StartThread();
            }
            else
                LogHelper.Error("Failed to find text file!");
            return fe;
        }

        void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
        }

        void StartThread()
        {
            VerifyClosedThread();
            _thread = new Thread(() =>
            {
                while (shouldUpdate)
                {
                    bool failed = false;
                    int tempHR = 0;
                    // get text
                    string text = String.Empty;
                    try { text = File.ReadAllText(pubFe); } catch (Exception e) { LogHelper.Error("Failed to find Text File! Exception: ", e); failed = true; }
                    // cast to int
                    if (!failed)
                        try { tempHR = Convert.ToInt32(text); } catch (Exception e) { LogHelper.Error("Failed to parse to int! Exception: ", e); }
                    HR = tempHR;
                    Thread.Sleep(500);
                }
            });
            _thread.Start();
        }

        void HRManager.Stop()
        {
            shouldUpdate = false;
            VerifyClosedThread();
        }

        int HRManager.GetHR() => HR;
        public bool IsOpen() => shouldUpdate;
        public bool IsActive() => IsOpen();
    }
}
