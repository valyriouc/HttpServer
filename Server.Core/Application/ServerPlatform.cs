﻿using Server.Core.Logging;

namespace Server.Core.Application;

public interface IBuilder<TResult> 
{
    public TResult Build();
}

public interface IProtocolPlatformBuilder<TPlatform> : IBuilder<TPlatform>
{

}

public class ServerPlatformBuilder<THandler> : IBuilder<IServerPlatform>
    where THandler : IProtocolPlatform
{
    private ILogger? logger;
    private string? ipAddress;
    private ushort port;

    private IProtocolPlatformBuilder<THandler>? protocolBuilder;

    public ServerPlatformBuilder()
    {

    }

    public ServerPlatformBuilder<THandler> ConfigureProtocol(Func<IProtocolPlatformBuilder<THandler>> configure)
    {
        this.protocolBuilder = configure();
        return this;
    }

    public ServerPlatformBuilder<THandler> WithLogger(ILogger logger)
    {
        this.logger = logger;
        return this;
    }

    public ServerPlatformBuilder<THandler> WithIpAddress(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            throw new ArgumentException(nameof(ipAddress)); 
        }

        this.ipAddress = ipAddress;

        return this;
    }

    public ServerPlatformBuilder<THandler> WithPort(ushort port)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            throw new ArgumentException(nameof(ipAddress));
        }

       this.port = port;    

        return this;
    }

    public IServerPlatform Build()
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            throw new ArgumentException(nameof(ipAddress));
        }

        if (protocolBuilder is null)
        {
            throw new ArgumentException(nameof(protocolBuilder));
        }

        if (logger is null)
        {
            // TODO: We can make the logger optional!
            throw new ArgumentException(nameof(logger));
        }

        return new ServerPlatform<THandler>(protocolBuilder!.Build(), new(ipAddress, port), logger);
    }
}

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
