using Server.Core.Application.Core;
using Server.Core.Http;

namespace Server.Static;

internal class StaticFileApplication : IApplication
{ 
    public IEndpoint Create(HttpRequest request)
    {
        return new StaticEndpoint("C:\\Hackerspace\\Serving\\");
    }
}
