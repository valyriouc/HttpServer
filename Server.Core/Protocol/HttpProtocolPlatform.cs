using Server.Core.Application.Core;
using Server.Generic;

using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

namespace Server.Core.Protocol;

/// <summary>
/// Class which represents a module that is capabable of handling http 
/// </summary>
public class HttpProtocolPlatform<TParser> : IProtocolPlatform
    where TParser : IParser<HttpNode>
{
    public Dictionary<string, IApplication> Applications { get; }

    public TParser Parser { get; }

    public HashSet<Middleware> RequestPipeline { get; }

    public HttpProtocolConfigurations Config { get; }
    
    public HttpProtocolPlatform(Func<TParser> getParserFunc)
    {
        Applications = new Dictionary<string, IApplication>();
        RequestPipeline = new HashSet<Middleware>();
        Parser = getParserFunc();
    }

    public async Task<Memory<byte>> HandleOperationAsync(Memory<byte> request)
    {
        Parser.Feed(request);
        IEnumerable<HttpNode> nodes = Parser.Parse();
        Parser.Deconstruct();
       

        // Get response 
        throw new NotImplementedException();
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

    public static Stream GetBody(this HttpNode node)
    {
        if (node.Part is not HttpPart.Body)
        {
            throw new HttpParserException("Expected a http body node!");
        }

        byte[] bytes = node.Content;
        return new MemoryStream(bytes);
    }
}
