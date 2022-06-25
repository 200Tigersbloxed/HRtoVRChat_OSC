namespace HRtoVRChat_OSC_SDK;

public record HRSDKOptions
{
    public string SDKName { get; }
    public string IpPort { get; set; } = "127.0.0.1:9000";

    public HRSDKOptions(string SDKName)
    {
        if (SDKName.Split(' ').Length > 1)
            throw new Exception("SDKName cannot contain spaces!");
        this.SDKName = SDKName;
    }
}