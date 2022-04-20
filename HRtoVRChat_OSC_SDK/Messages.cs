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
        public int HR { get; set; }
        
        [ProtoMember(3)]
        public bool IsOpen { get; set; }
        
        [ProtoMember(4)]
        public bool IsActive { get; set; }
    }
    
    [ProtoContract]
    public class GetHRData : MessageTools
    {
        [ProtoMember(1)]
        public readonly string MessageType = "GetHRData";
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