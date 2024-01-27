using System.Text;

namespace Server.Core.Extensions;

internal static class VersionExtensions
{
    public static byte[] GetHttpVersionBytes(this Version version)
    {
        if (version.Minor == 1 && version.Major == 1)
        {
            return Encoding.UTF8.GetBytes("HTTP/1.1");
        }
        else
        {
            return Encoding.UTF8.GetBytes($"HTTP/{version.Major}");
        }
    }
}
