using HttpServer.ServerConsole.Logging;

using Server.Core;

namespace Server.Testing;

internal class Program
{
    static async Task Main(string[] args)
    {
        LogOntoConsole logger = new(true);

        TestingApplication app = new TestingApplication();
        using HttpServer<TestingApplication> server = new HttpServer<TestingApplication>(app, logger);

        HttpServerConfig config = new("127.0.0.1", 9999);

        CancellationTokenSource cts = new CancellationTokenSource();

        await server.RunAsync(config, cts.Token);
    }
}
