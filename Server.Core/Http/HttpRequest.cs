namespace Server.Core.Http;

public class HttpRequest
{

    public HttpMethod Method { get; init; }

    public Uri Url { get; init; }   
    
    private Version? Version { get; init; }

    public HttpHeaderDictionary Headers { get; init; }

    public MemoryStream? Body { get; init; }

    internal HttpRequest(
        HttpMethod method, 
        Uri url, 
        Version? version, 
        HttpHeaderDictionary headers, 
        MemoryStream? body) 
    {
        this.Method = method;
        Url = url;
        Version = version;
        this.Headers = headers;
        Body = body;
    }
}