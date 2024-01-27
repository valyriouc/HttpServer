using Server.Core.Application.Core;
using Server.Core.Http;

namespace Server.Testing;

internal sealed class TestingEndpoint : IEndpoint
{
    public async Task<IResponse> ExecuteAsync(HttpRequest request)
    {
        await Task.CompletedTask;
        return new TestingResponse();
    }
}
