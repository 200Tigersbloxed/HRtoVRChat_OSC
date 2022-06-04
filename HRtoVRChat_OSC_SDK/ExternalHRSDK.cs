#nullable enable

namespace HRtoVRChat_OSC_SDK;

public abstract class ExternalHRSDK
{
    /// <summary>
    /// The name of your SDK
    /// </summary>
    public abstract string SDKName { get; set; }

    /// <summary>
    /// What the SDK will listen for when to update it's data. Please don't set this at runtime.
    /// </summary>
    public Action<Messages.HRMessage> OnHRMessageUpdated = message => { };
    
    private Messages.HRMessage _currentHRData = new();
    
    /// <summary>
    /// The current HR data
    /// </summary>
    public Messages.HRMessage CurrentHRData
    {
        get => _currentHRData;
        set
        {
            _currentHRData = value;
            OnHRMessageUpdated.Invoke(value);
        }
    }

    /// <summary>
    /// Register the SDK regardless of if Initialize() returns false.
    /// Note that this will not override the inactivity check on IsActive
    /// </summary>
    public bool OverrideInitializeAdd { get; set; } = false;

    /// <summary>
    /// Check whether or not the SDK should be used
    /// </summary>
    /// <returns>The status of whether or not the SDK should be used</returns>
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