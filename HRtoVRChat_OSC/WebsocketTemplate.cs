using System.Net.WebSockets;
using System.Text;

namespace HRtoVRChat_OSC
{
    public class WebsocketTemplate
{
    public string wsUri;
    private ClientWebSocket cws = null;

    public bool IsAlive => cws?.State == WebSocketState.Open;
    
    public WebsocketTemplate(string wsUri)
    {
        this.wsUri = wsUri;
    }

    public async Task<bool> Start()
    {
        bool noerror = false;
        try
        {
            if (cws == null)
                cws = new ClientWebSocket();
            await cws.ConnectAsync(new Uri(wsUri), CancellationToken.None);
            noerror = true;
        }
        catch(Exception e)
        {
            LogHelper.Error("Failed to connect to HypeRate server! Exception: ", e);
        }

        return noerror;
    }

    private int senderror = 0;
    public async Task SendMessage(string message, bool closeonfail = true)
    {
        if (cws != null)
        {
            if (cws.State == WebSocketState.Open)
            {
                byte[] sendBody = Encoding.UTF8.GetBytes(message);
                try
                {
                    await cws.SendAsync(new ArraySegment<byte>(sendBody), WebSocketMessageType.Text, true, CancellationToken.None);
                    senderror = 0;
                }
                catch (Exception e) 
                {
                    senderror++;
                    if (senderror > 15 && closeonfail)
                    {
                        await Stop();
                    }
                }
            }
        }
    }

    private int receiveerror = 0;
    public async Task<string> ReceiveMessage(bool closeonfail = true)
    {
        var clientbuffer = new ArraySegment<byte>(new byte[1024]);
        WebSocketReceiveResult result = null;
        try
        {
            result = await cws.ReceiveAsync(clientbuffer, CancellationToken.None);
        }
        catch(Exception e)
        {
            receiveerror++;
            if (receiveerror > 15 && closeonfail)
            {
                await Stop();
            }
        }
        // Only check if result is not null
        if(result != null)
            if (result.Count != 0 || result.CloseStatus == WebSocketCloseStatus.Empty)
            {
                string msg = Encoding.ASCII.GetString(clientbuffer.Array ?? Array.Empty<byte>());
                return msg;
            }
        return String.Empty;
    }

    public async Task<bool> Stop()
    {
        if (cws != null)
            if (cws.State == WebSocketState.Open)
                try
                {
                    await cws.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, CancellationToken.None);
                    cws.Dispose();
                    cws = null;
                    return true;
                }
                catch (Exception e)
                {
                    LogHelper.Error("Failed to close connection to HypeRate Server! Exception: ", e);
                    return false;
                }
            else
                return false;
        return false;
    }
}
}