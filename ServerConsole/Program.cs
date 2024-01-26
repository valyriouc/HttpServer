using HttpServer.ServerConsole.Logging;

namespace HttpServer.ServerConsole;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        // TODO: Args for later
        HttpServer<TestingApplication> server = new HttpServer<TestingApplication>(
            new TestingApplication(),
            new LogOntoConsole(true));

        HttpServerConfig config = new HttpServerConfig("127.0.0.1", 9999);
        
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        await server.RunAsync(config, tokenSource.Token);
        
    }
}