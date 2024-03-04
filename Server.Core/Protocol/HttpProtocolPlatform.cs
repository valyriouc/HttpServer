using Server.Core.Application.Core;
using Server.Core.Http;
using Server.Generic;
using System.Text;

namespace Server.Core.Protocol;

/// <summary>
/// Class which represents a module that is capabable of handling http 
/// </summary>
public class HttpProtocolPlatform<TParser> : IProtocolPlatform
    where TParser : IParser<HttpNode>
{
    public IApplication Application { get; }

    public TParser Parser { get; }

    public HashSet<Middleware> RequestPipeline { get; }

    public HttpProtocolConfigurations Config { get; }
    
    public HttpProtocolPlatform(Func<TParser> getParserFunc, IApplication app)
    {
        Application = app;
        RequestPipeline = new HashSet<Middleware>();
        Parser = getParserFunc();
    }

    public async Task<Memory<byte>> HandleOperationAsync(Memory<byte> request)
    {
        Parser.Feed(request);
        IEnumerable<HttpNode> nodes = Parser.Parse();
        Parser.Deconstruct();

        HttpRequest req = BuildRequest(nodes);

        IEndpoint endpoint = Application.Create(req);

        HttpResponse response = new HttpResponse();

        IResponse res = await endpoint.ExecuteAsync(req);

        await res.WriteToBodyAsync(response.Body);

        return await response.WriteHttpAsync(res.ContentType);
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
        Uri uri = enumerator.Current.GetUrl();

        // TODO: Implement a check for allowed urls!
        // Maybe a pattern matching system!
        builder.WithUrl(uri);

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
            if (Config.AllowedHeaders.ContainsKey(name))
            {
                throw HttpException.BadRequest(
                    "This header is not allowed by the server!");
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
        return Version.Parse(Encoding.UTF8.GetString(bytes));
    }

    public static Uri GetUrl(this HttpNode node)
    {
        if (node.Part is not HttpPart.Url)
        {
            throw new HttpParserException("Expected a http url node!");
        }

        byte[] bytes = node.Content;
        return new Uri(Encoding.UTF8.GetString(bytes));
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
