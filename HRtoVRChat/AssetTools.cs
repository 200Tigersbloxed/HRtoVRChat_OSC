using System.IO;
using System.Reflection;
using Avalonia.Media.Imaging;

namespace HRtoVRChat;

public static class AssetTools
{
    public static IBitmap Icon;

    public static void Init()
    {
        Icon = GetBitmap("hrtovrchat_emb.ico");
    }
    
    private static IBitmap? GetBitmap(string fileName)
    {
        using (Stream stream =
               Assembly.GetExecutingAssembly().GetManifestResourceStream("HRtoVRChat.Assets." + fileName))
        {
            if (stream != null)
                return new Bitmap(stream);
            else
                return null;
        }
    }
}