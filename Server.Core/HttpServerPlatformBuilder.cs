using Server.Core.Logging;
using Server.Core.Protocol;
using Server.Generic;

using System.Net;

namespace Server.Core;

/// <summary>
/// Builder for a server platform
/// </summary>
/// <typeparam name="THandler"></typeparam>
public class HttpServerPlatformBuilder : IServerPlatformBuilder
{
    private ILogger? logger;
    private IPAddress? ipAddress;
    private ushort? port;

    public HttpProtocolPlatformBuilder protocolBuilder;

    public HttpServerPlatformBuilder()
    {

    }

    public HttpServerPlatformBuilder ConfigureProtocol(Action<HttpProtocolPlatformBuilder> configure)
    {
        protocolBuilder = new HttpProtocolPlatformBuilder();
        configure(this.protocolBuilder);
        return this;
    }

    public HttpServerPlatformBuilder WithLogger(ILogger logger)
    {
        this.logger = logger;
        return this;
    }

    private void ThrowIfNull(object? nullable, string name)
    {
        if (nullable is null)
        {
            throw new ArgumentNullException(name);
        }
    }

    public IServerPlatformBuilder WithAddress(IPAddress address)
    {
        if (address is null)
        {
            throw new ArgumentNullException(nameof(address));
        }

        this.ipAddress = address;

        return this;
    }

    public IServerPlatformBuilder WithPort(ushort port)
    { 
        this.port = port;
        return this;
    }

    public IServerPlatform Build()
    {
        ThrowIfNull(port, nameof(port));
        ThrowIfNull(protocolBuilder, nameof(protocolBuilder));

        if (logger is null)
        {
            // TODO: We can make the logger optional!
            throw new ArgumentException(nameof(logger));
        }

        protocolBuilder.WithLogger(logger);
        HttpProtocolPlatform protocol = protocolBuilder.Build();

        return new ServerPlatform(
            protocol,
            new(ipAddress, (ushort)port!),
            logger);
    }
}