using Server.Core.Application.Core;
using Server.Core.Logging;

namespace Server.Core.Application;

public interface IServerPlatform
{
    public Task StartAsync(CancellationToken cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken);

    public Task RestartAsync(CancellationToken cancellationToken);
}

internal class ServerPlatform<THandler> : IServerPlatform
    where THandler : IProtocolPlatform
{
    private readonly ILogger logger;
    private readonly ServerConfig serverConfig;

    private HttpServer<THandler> Server { get; init; }

    internal ServerPlatform(
        THandler handler,
        ServerConfig serverConfig,
        ILogger logger)
    {
        Server = new HttpServer<THandler>(handler, logger);

        this.serverConfig = serverConfig;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken) =>
        await Server.RunAsync(this.serverConfig, cancellationToken);

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task RestartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
