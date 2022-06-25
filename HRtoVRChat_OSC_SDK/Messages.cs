using ProtoBuf;

namespace HRtoVRChat_OSC_SDK;

public class Messages
{
    public static string GetMessageType(object? message) => (string) Convert.ChangeType(
        message.GetType().GetField("MessageType").GetValue(message), typeof(string));

    public static object? DeserializeMessage(byte[] data)
    {
        object? message = default;
        using (MemoryStream ms = new MemoryStream(data))
        {
            foreach (Type derrivedType in MessageCache.MessageTypes)
            {
                object tempMessage = Serializer.Deserialize(derrivedType, ms);
                if (tempMessage != null)
                {
                    message = tempMessage;
                    break;
                }
            }
        }
        return message;
    }

    public static T DeserializeMessage<T>(byte[] data)
    {
        T message;
        using (MemoryStream ms = new MemoryStream(data))
        {
            message = Serializer.Deserialize<T>(ms);
        }
        return message;
    }
    
    [ProtoContract]
    public class HRMessage : MessageTools
    {
        [ProtoMember(1)]
        public readonly string MessageType = "HRMessage";
        
        [ProtoMember(2)]
        public string SDKName { get; set; }
        
        [ProtoMember(3)]
        public int HR { get; set; }
        
        [ProtoMember(4)]
        public bool IsOpen { get; set; }
        
        [ProtoMember(5)]
        public bool IsActive { get; set; }
    }
    
    [ProtoContract]
    public class GetHRData : MessageTools
    {
        [ProtoMember(1)]
        public readonly string MessageType = "GetHRData";
    }

    [ProtoContract]
    public class UpdateMessage : MessageTools
    {
        [ProtoMember(1)]
        public readonly string MessageType = "UpdateMessage";
    }
    
    [ProtoContract]
    public class AppBridgeMessage : MessageTools
    {
        [ProtoMember(1)]
        public readonly string MessageType = "AppBridgeMessage";
        
        [ProtoMember(2)]
        public string CurrentSourceName { get; set; }
        
        [ProtoMember(3)]
        public int onesHR { get; set; }
        
        [ProtoMember(4)]
        public int tensHR { get; set; }
        
        [ProtoMember(5)]
        public int hundredsHR { get; set; }
        
        [ProtoMember(6)]
        public bool isHRConnected { get; set; }
        
        [ProtoMember(7)]
        public bool isHRActive { get; set; }
        
        [ProtoMember(8)]
        public bool isHRBeat { get; set; }
        
        [ProtoMember(9)]
        public float HRPercent { get; set; }
        
        [ProtoMember(10)]
        public int HR { get; set; }
        
        [ProtoMember(11)]
        public AvatarInfo? CurrentAvatar { get; set; }
    }

    [ProtoContract]
    public class AvatarInfo
    {
        [ProtoMember(1)]
        public string id { get; set; }
        [ProtoMember(2)]
        public string name { get; set; }
        [ProtoMember(3)]
        public List<string> parameters { get; set; }
    }
}

public class MessageTools
{
    public byte[] Serialize()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            Serializer.Serialize(ms, this);
            byte[] bytes = ms.ToArray();
            return bytes;
        }
    }
}

public static class MessageCache
{
    public static readonly List<Type> MessageTypes = new List<Type>
    {
        typeof(Messages.HRMessage),
        typeof(Messages.GetHRData)
    };
}