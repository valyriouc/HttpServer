namespace Server.Core.Http;

internal class HttpRequestBuilder
{
    private HttpMethod? method;
    private string? url;
    private Version? version;

    public Dictionary<string, string> headers;

    public Stream? body;

    public HttpRequestBuilder()
    {
        headers = new Dictionary<string, string>();
    }

    public HttpRequestBuilder WithMethod(HttpMethod method)
    {
        this.method = method;
        return this;
    }

    public HttpRequestBuilder WithUrl(string url)
    {
        this.url = url;
        return this;
    }

    public HttpRequestBuilder WithVersion(Version version)
    {
        this.version = version;
        return this;
    }

    public HttpRequestBuilder WithHeader(string name, string value)
    {
        headers.Add(name, value);
        return this;
    }

    public HttpRequestBuilder WithBody(ReadOnlySpan<byte> body)
    {
        this.body = new MemoryStream();
        this.body.Write(body);
        this.body.Position = 0;
        return this;
    }

    public HttpRequest Build()
    {
        if (method is null)
        {
            throw new ArgumentException("Missing method!");
        }

        if (url is null)
        {
            throw new ArgumentException("Missing url!");
        }

        HttpHeaderDictionary headers = new HttpHeaderDictionary(this.headers);

        return new HttpRequest(
            method,
            url,
            version,
            headers,
            this.body);
    }
}

