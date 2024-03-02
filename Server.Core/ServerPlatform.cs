using Server.Core.Http;
using Server.Core.Logging;
using Server.Generic;

namespace Server.Core;

/// <summary>
/// Represents a middleware which transforms the http request on its way to the target endpoint 
/// </summary>
/// <param name="request"></param>
/// <param name="next"></param>
/// <returns></returns>
public delegate HttpResponse Middleware(HttpRequest request, Middleware next);

/// <summary>
/// Implementation of a server platform 
/// </summary>
/// <typeparam name="THandler"></typeparam>
internal class ServerPlatform<THandler> : IServerPlatform
    where THandler : IProtocolPlatform
{
    private readonly ILogger logger;

    // TODO: Later this should be an abstract interface for all types of servers 
    private IServerBackbone<THandler> Server { get; init; }

    internal ServerPlatform(
        THandler handler,
        ServerConfig serverConfig,
        ILogger logger)
    {
        this.logger = logger;
        Server = new ThreadServer<THandler>(handler, serverConfig, logger);
    }

    public async Task StartAsync(CancellationToken cancellationToken) =>
        await Server.StartAsync(cancellationToken);

    public async Task StopAsync(CancellationToken cancellationToken) => 
        await Server.StopAsync(cancellationToken);

    public async Task RestartAsync(CancellationToken cancellationToken) => 
        await Server.RestartAsync(cancellationToken);
}
