using Server.Core.Http;

namespace Server.Core.Application.Core;

public interface IApplication
{
    public IEndpoint Create(HttpRequest request);
}
