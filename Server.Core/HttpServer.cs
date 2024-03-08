using Server.Core.Http;
using Server.Core.Protocol;
using Server.Http.Protocol;
using Vectorize.Logging;
using Vectorize.Server.Handles;
using Vectorize.Server.Protocol;
using Vectorize.Server.Server;

namespace Server.Http;

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