using Server.Core.Application.Core;
using Server.Core.Http;
using Server.Core.Logging;
using Server.Generic;

using System.Net;
using System.Text;

namespace Server.Core.Protocol;

/// <summary>
/// Class which represents a module that is capabable of handling http 
/// </summary>
public class HttpProtocolPlatform : IProtocolPlatform<HttpResponse>
{
    public ILogger Logger { get; }

    public IApplication Application { get; }

    public IParser<HttpNode> Parser { get; }

    public HttpProtocolConfigurations Config { get; }

    public HttpProtocolPlatform(
        HttpProtocolConfigurations config,
        Func<IParser<HttpNode>> getParserFunc, 
        IApplication app,
        ILogger logger)
    {
        Config = config;
        Application = app;
        Parser = getParserFunc();
        Logger = logger;
    }

    public async Task<HttpResponse> HandleOperationAsync(Memory<byte> request)
    {
        try
        {
            Parser.Feed(request);
            IEnumerable<HttpNode> nodes = Parser.Parse();

            HttpRequest req = BuildRequest(nodes);

            IEndpoint endpoint = Application.Create(req);

            HttpResponse response = new HttpResponse();

            // THis should be called payload 
            IResponse res = await endpoint.ExecuteAsync(req);

            Parser.Deconstruct();

            await res.WriteToBodyAsync(response.Body);

            return response;
        }
        catch (HttpException ex)
        {
            Logger.Error(ex);   
            return HttpResponse.FromHttpException(ex);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return HttpResponse.FromUnexpectedException(ex);
        }
    }
    private HttpRequest BuildRequest(IEnumerable<HttpNode> nodes)
    {
        HttpRequestBuilder builder = new();

        IEnumerator<HttpNode> enumerator = nodes.GetEnumerator();

        enumerator.MoveNext();
        HttpMethod method = enumerator.Current.GetMethod();

        if (!Config.Methods.Contains(method.Method))
        {
            throw HttpException.MethodNotAllowed(
                $"Method {method.Method} is not supported!");
        }

        builder.WithMethod(method);

        enumerator.MoveNext();
        string path = enumerator.Current.GetPath();

        // TODO: Implement a check for allowed urls!
        // Maybe a pattern matching system!
        builder.WithUrl(path);

        enumerator.MoveNext();
        Version version = enumerator.Current.GetVersion();
        if (!Config.Versions.Contains(version))
        {
            throw HttpException.BadRequest(
                $"Http version {version.Major}.{version.Minor} is not supported!");
        }

        builder.WithVersion(version);

        HttpNode current = enumerator.Current;
        bool hasBody = false;

        while(enumerator.MoveNext())
        {
            current = enumerator.Current;

            if (current.Part == HttpPart.Body)
            {
                hasBody = true;
                break;
            }

            (string name, string value) = current.GetHeader();

            // Make this to restricted headers!
            bool contains = Config.ForbiddenHeaders.ContainsKey(name);
            if (contains)
            {
                throw HttpException.BadRequest(
                    "This header is not allowed by the server!");
            }
            
            if (contains)
            {
                if (Config.ForbiddenHeaders[name].Contains(value))
                {
                    throw HttpException.BadRequest(
                        $"Header value {value} is not allowed for header {name}!");
                }
            }

            builder.WithHeader(name, value);
        }

        if (hasBody)
        {
            ReadOnlySpan<byte> body = current.GetBody();
            builder.WithBody(body);
        }

        return builder.Build();
    }
}

file static class HttpNodeExtensions
{
    public static HttpMethod GetMethod(this HttpNode node)
    {
        if (node.Part is not HttpPart.Method)
        {
            throw new HttpParserException("Expected a http method node!");
        }

        byte[] bytes = node.Content;
        return HttpMethod.Parse(Encoding.UTF8.GetString(bytes));
    }

    public static Version GetVersion(this HttpNode node)
    {
        if (node.Part is not HttpPart.Version)
        {
            throw new HttpParserException("Expected a http version node!");
        }

        byte[] bytes = node.Content;

        string version = Encoding.UTF8.GetString(bytes); 
        
        switch (version)
        {
            case "HTTP1.1":
                return HttpVersion.Version11;
                break;
            case "HTTP1":
                return HttpVersion.Version10;
                break;
            case "HTTP2":
                return HttpVersion.Version20;
                break;
            case "HTTP3":
                return HttpVersion.Version30;
                break;
            default:
                throw new HttpParserException("Could not determine http version!");
        }
    }

    public static string GetPath(this HttpNode node)
    {
        if (node.Part is not HttpPart.Url)
        {
            throw new HttpParserException("Expected a http url node!");
        }

        byte[] bytes = node.Content;
        // Uri can`t handle only a path with get parameters
        return Encoding.UTF8.GetString(bytes);
    }

    public static (string, string) GetHeader(this HttpNode node)
    {
        if (node.Part is not HttpPart.Header)
        {
            throw new HttpParserException("Expected a http header node!");
        }

        byte[] bytes = node.Content;
        string[] parts = Encoding.UTF8.GetString(bytes).Split(":");

        if (parts.Length != 2)
            throw new HttpParserException("Malformed http header!");

        return (parts[0].Trim(), parts[1].Trim());
    }

    public static ReadOnlySpan<byte> GetBody(this HttpNode node)
    {
        if (node.Part is not HttpPart.Body)
        {
            throw new HttpParserException("Expected a http body node!");
        }

        byte[] bytes = node.Content;
        return bytes;
    }
}
