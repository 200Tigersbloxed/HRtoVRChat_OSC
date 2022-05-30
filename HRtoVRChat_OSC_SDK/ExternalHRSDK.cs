namespace HRtoVRChat_OSC_SDK;

public abstract class ExternalHRSDK
{
    /// <summary>
    /// The name of your SDK
    /// </summary>
    public abstract string SDKName { get; set; }
    
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
    /// Register the SDK regardless of if Initialize() returns false.
    /// Note that this will not override the inactivity check on IsActive
    /// </summary>
    public bool OverrideInitializeAdd { get; set; } = false;

    /// <summary>
    /// Check whether or not the SDK should be used
    /// </summary>
    /// <returns></returns>
    public abstract bool Initialize();
    
    /// <summary>
    /// An Update frame called from a secondary Thread every 10ms
    /// </summary>
    public virtual void Update(){}
    
    /// <summary>
    /// Called by the app when the Data Source is closing.
    /// For example, this should be used to close a WebSocket connection, if one is made.
    /// </summary>
    public virtual void Destroy(){}
}