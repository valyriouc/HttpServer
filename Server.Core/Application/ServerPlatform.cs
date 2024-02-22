using Server.Core.Application.Core;
using Server.Core.Logging;

namespace Server.Core.Application;

public enum HttpPart
{
    Method=0,
    Url = 1,
    Version = 2,
    Header = 3,
    Body = 4
}

public interface IHttpParser
{
    public IEnumerable<(HttpPart, byte[])> Parse(ReadOnlyMemory<byte> payload);
}

public interface IRequestBuilder
{

}

public interface IResponseBuilder
{

}

internal class ServerPlatformBuilder
{

}

internal class ServerPlatform<TApp, TParser, BRequest, BResponse>
    where TApp : IApplication
    where TParser : IHttpParser
    where BRequest : IRequestBuilder
    where BResponse : IResponseBuilder
{
    public TParser Parser { get; init; }

    public BRequest RequestBuilder { get; init; }

    public BResponse ResponseBuilder { get; init; }

    public TApp Application { get; init; }

    public ILogger Logger { get; init; }

    internal ServerPlatform(
        TApp app, 
        TParser parser, 
        BRequest request, 
        BResponse response, 
        ILogger logger)
    {
        Application = app;
        Parser = parser;
        RequestBuilder = request;
        ResponseBuilder = response;
        Logger = logger;
    }

    public async Task<Memory<byte>> HandleAsync(Memory<byte> payload)
    {
        return Memory<byte>.Empty;
    }
}
