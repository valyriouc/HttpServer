namespace Server.Core.Http;

public class HttpRequest
{
    private enum UrlPart
    {
        Path = 2,
        Query = 3
    }

    public HttpMethod Method { get; init; }

    private Dictionary<UrlPart, string> UrlParts { get; set; }
    
    private Version? Version { get; init; }

    public string Path
    {
        get
        {
            return UrlParts[UrlPart.Path];
        }
    }

    public IEnumerable<string> Parameters
    {
        get
        {
            foreach (string param in UrlParts[UrlPart.Query].Split("&"))
            {
                yield return param;
            }
        }
    }

    public HttpHeaderDictionary Headers { get; init; }

    public MemoryStream? Body { get; init; }

    internal HttpRequest(
        HttpMethod method, 
        string url, 
        Version? version, 
        HttpHeaderDictionary headers, 
        MemoryStream? body) 
    {
        this.Method = method;
        UrlParts = ParseUrl(url);
        Version = version;
        this.Headers = headers;
        Body = body;
    }

    private static Dictionary<UrlPart,string> ParseUrl(string url)
    {
        Dictionary<UrlPart, string> parts = new();

        int i = url.IndexOf("?");

        string? path = i == -1 ? 
            url : 
            url.TakeWhile(x => x != '?').ToString();

        if (path is null)
        {
            throw new Exception("Could not parse path from url!");
        }

        parts[UrlPart.Path] = path;

        if (i == -1)
            return parts;

        int skip = path.Length;
        string? query = url
            .Skip(skip)
            .Take(url.Length - skip)
            .ToString();

        if (query is null)
        {
            throw new Exception("Could not parse query from url!");
        }

        parts[UrlPart.Query] = query;

        return parts;
    }
}