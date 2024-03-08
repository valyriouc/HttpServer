using Server.Core.Http;
using Vectorize.Server;

namespace Server.Http;

internal class HttpPlatform : IProtocolPlatform<HttpRequest, HttpResponse>
{
    public Task<HttpResponse> HandleRequestAsync(HttpRequest request, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}