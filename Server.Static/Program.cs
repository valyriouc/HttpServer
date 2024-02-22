using HttpServer.ServerConsole.Logging;

using Server.Core;
using Server.Core.Logging;

namespace Server.Static;

internal class Program
{
    static async Task Main(string[] args)
    {
        ILogger logger = new LogOntoConsole(true);

        StaticFileApplication app = new StaticFileApplication();

        HttpServer<StaticFileApplication> server = new HttpServer<StaticFileApplication>(app, logger);

        HttpServerConfig config = new("127.0.0.1", 3233);

        CancellationTokenSource cts = new();

        await server.RunAsync(config, cts.Token);
    }
}
