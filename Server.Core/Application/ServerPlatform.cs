using Server.Core.Application.Core;
using Server.Core.Http;
using Server.Core.Logging;

namespace Server.Core.Application;

/// <summary>
/// Abstract interface for a builder 
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IBuilder<TResult> 
{
    public TResult Build();
}

/// <summary>
/// Represents a middleware which transforms the http request on its way to the target endpoint 
/// </summary>
/// <param name="request"></param>
/// <param name="next"></param>
/// <returns></returns>
public delegate HttpResponse Middleware(HttpRequest request, Middleware next);

/// <summary>
/// Configuration for the http protocol
/// </summary>
public struct HttpProtocolConfigurations
{
    public HashSet<string> Methods { get; }

    public HashSet<Version> Versions { get; }

    public Dictionary<string, string[]> AllowedHeaders { get; }

    public HttpProtocolConfigurations()
    {
        Methods = new HashSet<string>();
        Versions = new HashSet<Version>();
    }

}

/// <summary>
/// Class which represents a module which is capabable of handling http 
/// </summary>
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

/// <summary>
/// Interface that builds a platform which can handle a specific (network) protocol
/// </summary>
/// <typeparam name="TPlatform"></typeparam>
public interface IProtocolPlatformBuilder<TPlatform> : IBuilder<TPlatform>
{
    public IProtocolPlatformBuilder<TPlatform> WithMethods(HashSet<string> methods);

    public IProtocolPlatformBuilder<TPlatform> WithVersions(HashSet<Version> versions);

    public IProtocolPlatformBuilder<TPlatform> WithHeaders(Dictionary<string, string[]> headers);
}

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

/// <summary>
/// Inteface which represents a server platform module 
/// Server platform encapsulate should encapsulate a tcp module 
/// and a protocol handler 
/// </summary>
public interface IServerPlatform
{
    public Task StartAsync(CancellationToken cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken);

    public Task RestartAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Implementation of a server platform 
/// </summary>
/// <typeparam name="THandler"></typeparam>
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
