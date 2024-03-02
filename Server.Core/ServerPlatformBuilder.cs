using Server.Core.Logging;
using Server.Generic;

namespace Server.Core;

/// <summary>
/// Builder for a server platform
/// </summary>
/// <typeparam name="THandler"></typeparam>
public class ServerPlatformBuilder<THandler> : IBuilder<IServerPlatform>
    where THandler : IProtocolPlatform
{
    private ILogger? logger;
    private string? ipAddress;
    private ushort? port;

    private IProtocolPlatformBuilder<THandler>? protocolBuilder;

    public ServerPlatformBuilder()
    {

    }

    public ServerPlatformBuilder<THandler> ConfigureProtocol(Func<IProtocolPlatformBuilder<THandler>> configure)
    {
        protocolBuilder = configure();
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

    private void ThrowIfNull(object? nullable, string name)
    {
        if (nullable is null)
        {
            throw new ArgumentNullException(name);
        }
    }

    public IServerPlatform Build()
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            throw new ArgumentException(nameof(ipAddress));
        }

        ThrowIfNull(port, nameof(port));
        ThrowIfNull(protocolBuilder, nameof(protocolBuilder));

        if (logger is null)
        {
            // TODO: We can make the logger optional!
            throw new ArgumentException(nameof(logger));
        }

        return new ServerPlatform<THandler>(
            protocolBuilder!.Build(),
            new(ipAddress, (ushort)port!),
            logger);
    }
}

