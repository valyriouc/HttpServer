using Server.Core.Application.Core;
using Server.Core.Http;
using Server.Core.Logging;

namespace Server.Core.Application;

public interface IBuilder<TResult> 
{
    public TResult Build();
}

public delegate HttpResponse Middleware(HttpRequest request, Middleware next);

public struct HttpProtocolConfigurations
{
    public HashSet<string> Methods { get; }

    public HashSet<double> Versions { get; }

    public HttpProtocolConfigurations()
    {
        Methods = new HashSet<string>();
        Versions = new HashSet<double>();
    }

}

public class HttpProtocolPlatform : IProtocolPlatform
{
    public Dictionary<string, IApplication> Applications { get; } 
    
    public HashSet<Middleware> RequestPipeline { get; }

    public HttpProtocolPlatform()
    {
        Applications = new Dictionary<string, IApplication>();
        RequestPipeline = new HashSet<Middleware>();
    }

    public Task<Memory<byte>> HandleOperationAsync(Memory<byte> request)
    {
        throw new NotImplementedException();
    }
}

public interface IProtocolPlatformBuilder<TPlatform> : IBuilder<TPlatform>
{
    public IProtocolPlatformBuilder<TPlatform> WithMethods(HashSet<string> methods);

    public IProtocolPlatformBuilder<TPlatform> WithVersions(HashSet<Version> versions);

    public IProtocolPlatformBuilder<TPlatform> WithHeaders(Dictionary<string, string[]> headers);
}

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

        ThrowIfNull(this.port, nameof(port));
        ThrowIfNull(this.protocolBuilder, nameof(protocolBuilder));

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
