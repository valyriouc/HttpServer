using Server.Core.Application.Core;
using Server.Core.Logging;
using Server.Generic;

namespace Server.Core.Protocol;

public class HttpProtocolPlatformBuilder : IProtocolPlatformBuilder<HttpProtocolPlatform>
{
    private Func<IParser<HttpNode>>? ParserProvider { get; set; }

    private HttpProtocolConfigurations Config { get; } = new HttpProtocolConfigurations();

    private IApplication? Application { get; set; }

    public IProtocolPlatformBuilder<HttpProtocolPlatform> WithoutHeader(string name, HashSet<string> values)
    {
        Config.ForbiddenHeaders.Add(name, values);
        return this;
    }

    public IProtocolPlatformBuilder<HttpProtocolPlatform> WithMethod(string method)
    {
        Config.Methods.Add(method);
        return this;
    }

    public IProtocolPlatformBuilder<HttpProtocolPlatform> WithVersion(Version version)
    {
        Config.Versions.Add(version); 
        return this;
    }

    public HttpProtocolPlatformBuilder WithParser(Func<IParser<HttpNode>> parserProvider)
    {
        ParserProvider = parserProvider;
        return this;
    }

    public HttpProtocolPlatformBuilder WithApplicaton(IApplication app)
    {
        Application = app;
        return this;
    }

    private void ThrowIfNull(object? nullable, string identifier)
    {
        if (nullable is null)
        {
            throw new ArgumentNullException(identifier);
        }
    }

    private ILogger logger;

    internal HttpProtocolPlatformBuilder WithLogger(ILogger logger)
    {
        this.logger = logger;
        return this;
    }

    public HttpProtocolPlatform Build()
    {
        ThrowIfNull(Application, nameof(Application));

        if (ParserProvider is null)
        {
            ParserProvider = () => new HttpParser(logger);
        }

        return new(Config, ParserProvider!, Application!, logger);
    }

}