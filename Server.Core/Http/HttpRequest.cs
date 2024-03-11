using Server.Http.Http.Version;
using Vectorize.Server;
using Vectorize.Server.Protocol;

namespace Server.Core.Http;

public class HttpRequest : IFromParsing<HttpRequest>
{

    public HttpMethod Method { get; init; }

    public HttpResourceIdentifier Url { get; init; }

    public HttpVersion Version { get; init; }

    public HttpHeaders Headers { get; init; }

    public MemoryStream? Body { get; init; }

    public HttpRequest()
    {

    }

    public HttpRequest(
        HttpMethod method, 
        HttpResourceIdentifier url, 
        HttpVersion version, 
        HttpHeaders headers, 
        MemoryStream? body) 
    {
        this.Method = method;
        Url = url;
        Version = version;
        this.Headers = headers;
        Body = body;
    }

    public static Task<HttpRequest> GetFromParsingAsync(IEnumerable<ParserNode> nodes, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}

public struct HttpResourceIdentifier 
{
    public string Unformatted { get; init; }

    public string Path { get; init; }

    public IEnumerable<(string, string)> Query { get; }

    public HttpResourceIdentifier(string unformattedPath)
    {
        Unformatted = unformattedPath;
        string[] splitted = Unformatted.Split("?");

        if (splitted.Length < 1 || splitted.Length > 2)
            throw new Exception("Invalid http path!");

        Query = new List<(string, string)>();

        if (splitted.Length == 2)
        {
            Query = ParseQueryString(splitted[1]);
        }
    }
        
    public static IEnumerable<(string, string)> ParseQueryString(string query)
    { 
        foreach (string param in query.Split("&"))
        {
            if (param.Count(x => x == '=') != 1)
            {
                continue;
            }

            string[] splitted = param.Split("=");
            yield return (splitted[0], splitted[1]);
        }
    }
}
