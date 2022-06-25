using HP.Omnicept;
using HP.Omnicept.Messaging;
using HP.Omnicept.Messaging.Messages;

namespace HRtoVRChat_OSC.HRManagers;

public class OmniceptManager : HRManager
{
    public int HR { get; set; }
    
    private Glia m_gliaClient;
    private GliaValueCache m_gliaValCache;
    private bool m_isConnected;
    private HeartRate lastHeartRate;
    
    private Thread _worker;
    private CancellationTokenSource token;
    private void VerifyDeadThread()
    {
        if (_worker != null)
        {
            if(_worker.IsAlive)
                token.Cancel();
            _worker = null;
        }
        token = new CancellationTokenSource();
    }
    
    private void StopGlia()
    { 
        // Verify Glia is Disposed
        if(m_gliaValCache != null)
            m_gliaValCache?.Stop();
        if(m_gliaClient != null)
            m_gliaClient?.Dispose();
        m_gliaValCache = null;
        m_gliaClient = null;
        m_isConnected = false;
        Glia.cleanupNetMQConfig();
    }

    private bool StartGlia(bool isTesting = false)
    {
        // Verify Glia is Disposed
        StopGlia();
        bool ret;
        
        // Start Glia
        try
        {
            m_gliaClient = new Glia("HRtoVRChat_OSC",
                new SessionLicense(String.Empty, String.Empty, LicensingModel.Core, false));
            m_gliaValCache = new GliaValueCache(m_gliaClient.Connection);
            SubscriptionList sl = new SubscriptionList
            {
                Subscriptions =
                {
                    new Subscription(MessageTypes.ABI_MESSAGE_HEART_RATE, String.Empty, String.Empty,
                        String.Empty, String.Empty, new MessageVersionSemantic("1.0.0"))
                }
            };
            m_gliaClient.setSubscriptions(sl);
            m_isConnected = true;
            ret = true;
        }
        catch (Exception)
        {
            m_isConnected = false;
            ret = false;
        }
        if (!m_isConnected || isTesting)
            StopGlia();
        if (isTesting)
            return ret;
        return m_isConnected;
    }
    
    void HandleMessage(ITransportMessage msg)
    {
        switch (msg.Header.MessageType)
        {
            case MessageTypes.ABI_MESSAGE_HEART_RATE:
                lastHeartRate = m_gliaClient.Connection.Build<HeartRate>(msg);
                break;
        }
    }
        
    ITransportMessage RetrieveMessage()
    {
        ITransportMessage msg = null;
        if (m_gliaValCache != null)
        {
            try
            {
                msg = m_gliaValCache.GetNext();
            }
            catch (HP.Omnicept.Errors.TransportError e)
            {
                LogHelper.Error(e.Message);
            }
        }
        return msg;
    }
    
    public bool Init(string d1)
    {
        bool status = StartGlia(true);
        if (status)
        {
            LogHelper.Log("Started Omnicept!");
            StartThread();
        }
        else
            LogHelper.Error("Failed to start Omnicept!");
        return status;
    }
    
    public void StartThread()
    {
        VerifyDeadThread();
        _worker = new Thread(() =>
        {
            LogHelper.Debug("Omnicept Thread Started");
            StartGlia();
            while (!token.IsCancellationRequested)
            {
                // Update message
                try
                {
                    if (m_isConnected)
                    {
                        ITransportMessage msg = RetrieveMessage();
                        if(msg != null)
                            HandleMessage(msg);
                    }
                }
                catch (Exception e)
                {
                    LogHelper.Error("Failed to get message! " + e);
                }
                if (lastHeartRate != null)
                {
                    HR = (int)lastHeartRate.Rate;
                }
                Thread.Sleep(10);
            }
            // Thread-Safe; don't need to call it in this thread :)
            //StopGlia();
        });
        _worker.Start();
    }
    
    public string GetName() => "Omnicept";

    public int GetHR() => HR;

    public void Stop()
    {
        StopGlia();
        token.Cancel();
    }

    public bool IsOpen() => m_isConnected && HR > 0;

    public bool IsActive() => m_isConnected;
}