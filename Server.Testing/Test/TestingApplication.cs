using Server.Core;
using Server.Core.Application.Core;
using Server.Core.Http;

namespace Server.Testing;

internal sealed class TestingApplication : IApplication
{
    public IEndpoint Create(HttpRequest request)
    {
        switch (request.Path)
        {
            case "/":
                return new TestingEndpoint();
                break;
            default:
                throw HttpException.NotFound(string.Empty);
                break;
        }
    }
}
