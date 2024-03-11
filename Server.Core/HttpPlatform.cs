using Server.Core.Http;
using Server.Http.Http.Version;
using Vectorize.Server;

namespace Server.Http;

internal class HttpPlatform : IProtocolPlatform<HttpRequest, HttpResponse>
{
    private VersionManager ProtocolVersion { get; }   

    internal HttpPlatform(
        VersionManager protocolVersion)
    {
        ProtocolVersion = protocolVersion;
    }

    public Task<HttpResponse> HandleRequestAsync(HttpRequest request, CancellationToken token)
    {
        ProtocolVersion.GetRuleSet()
    }


}

file static class HttpVersionExtensions
{
    public static HttpVersion Version
}