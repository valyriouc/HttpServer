using Server.Core.Http;
using Server.Core.Protocol;
using Server.Generic;
using Vectorize.Logging;
using Vectorize.Server;
using Vectorize.Server.Handles;

namespace Server.Http;

internal interface IProtocolPlatform<TRequest, TResponse>
    where TRequest : IFromParsing<TRequest>
    where TResponse : IToParsingInput
{
    public Task<TResponse> HandleRequestAsync(TRequest request, CancellationToken token);
}

internal class HttpPlatform : IProtocolPlatform<HttpRequest, HttpResponse>
{
    public Task<HttpResponse> HandleRequestAsync(HttpRequest request, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}

internal class HttpGenerator : ILoggable
{
    public ILogger Logger { get; }

    public HttpGenerator(ILogger logger)
    {
        Logger = logger;
    }

    public ReadOnlyMemory<byte> Generate(IEnumerable<ParserNode> input)
    {

        return Array.Empty<byte>();
    }
}

internal class HttpServer : IServerConsumer
{
    public ILogger Logger { get; set; }

    public HttpParser Parser { get; }

    public HttpGenerator Generator { get; set; }

    public HttpPlatform Platform { get; set;}

    public HttpServer(HttpPlatform platform, ILogger logger)
    {
        Logger = logger;
        Platform = platform;
        Parser = new HttpParser(Logger);
        Generator = new HttpGenerator(Logger);
    }

    public async Task ConsumeAsync(IByteHandle handle, CancellationToken token)
    {
        Parser.Feed(await handle.ReadBytesAsync(token));

        IEnumerable<ParserNode> parsed = Parser.Parse();

        HttpResponse res = await Platform.HandleRequestAsync(
            await HttpRequest.GetFromParsingAsync(parsed, token), 
            token);

        ReadOnlyMemory<byte> generated = Generator.Generate(await res.TransformToAsync(token));

        await handle.WriteBytesAsync(generated, token);
    }
}