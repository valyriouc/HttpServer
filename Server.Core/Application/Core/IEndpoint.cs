using Server.Core.Http;

namespace Server.Core.Application.Core;

public interface IEndpoint
{
    public Task<IResponse> ExecuteAsync(HttpRequest request);
}
