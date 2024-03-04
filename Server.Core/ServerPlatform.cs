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
internal class ServerPlatform : IServerPlatform
{
    private readonly ILogger logger;

    // TODO: Later this should be an abstract interface for all types of servers 
    private IServerBackbone Server { get; init; }

    internal ServerPlatform(
        IProtocolPlatform protocol,
        ServerConfig serverConfig,
        ILogger logger)
    {
        this.logger = logger;
        Server = new ThreadServer(protocol, serverConfig, logger);
    }

    public async Task StartAsync(CancellationToken cancellationToken) =>
        await Server.RunAsync(cancellationToken);

    public async Task StopAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public async Task RestartAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
