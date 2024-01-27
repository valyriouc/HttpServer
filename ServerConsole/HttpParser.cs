using HttpServer.ServerConsole.Logging;
using System.Net;
using System.Text;

namespace ServerConsole;

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

internal class HttpParserException : Exception
{
    public HttpParserException(string? message) : base(message)
    {
    }
}
internal class HttpParser
{
    private enum ParsingState
    {
        NotStarted,
        Method = 0,
        Url = 1, 
        Version = 2,
        Headers = 3,
        Body = 4,
        Finished = 5
    }

    private readonly ILogger logger;
    private readonly HttpRequestBuilder builder;

    private readonly ReadOnlyMemory<byte> data;
    private int ptr;
    private ParsingState state;

    public HttpParser(ReadOnlyMemory<byte> data, ILogger logger)
    {
        this.data = data;
        this.ptr = 0;
        this.state = ParsingState.NotStarted;
        this.logger = logger;

        builder = new HttpRequestBuilder();
    }

    public HttpRequest? Parse()
    {
        if (this.data.IsEmpty)
        {
            return null;
        }

        if (this.state != ParsingState.NotStarted)
        {
            throw new HttpParserException("Invalid parsing state!");
        }

        ParseMethod();
        ParseUrl();
        ParseVersion();
        
        while(this.state == ParsingState.Headers)
        {
            ParseHeader();
        }

        if (this.state == ParsingState.Body)
        {
            ReadBody();
        }
        
        return builder.Build();
    } 

    private void EnsureSpace()
    {
        Console.WriteLine((char)this.data.Span[ptr]);

        if (this.data.Span[ptr] != 0x20)
        {
            throw new HttpParserException("Expected a space!");
        }
        else
        {
            ptr += 1;
        }
    }

    private void ParseMethod()
    {
        switch(this.data.Span[0])
        {
            case 0x47:
                {
                    builder.WithMethod(HttpMethod.Get);
                    ptr += 3;
                    logger.Info("Read get message!");
                }
                break;
            case 0x50:
                {
                    if (this.data.Span[1] == 0x4f)
                    {
                        builder.WithMethod(HttpMethod.Post);
                        ptr += 4;
                        logger.Info("Read post message!");
                    }
                    else
                    {
                        throw new NotImplementedException("Http method is not supported!");
                    }
                }
                break;
            default:
                throw new NotImplementedException("Http method is not supported!");
                break;
        }

        this.state = ParsingState.Method;
    }
    
    private void ParseUrl()
    {
        if (this.state != ParsingState.Method)
        {
            throw new HttpParserException("Invalid parser state!");
        }

        EnsureSpace();

        logger.Warn(ptr.ToString());

        ReadOnlySpan<byte> slice = this.data.Slice(ptr).Span;

        int index = slice.IndexOf((byte)0x20);

        builder.WithUrl(Encoding.UTF8.GetString(slice[0..index]));

        ptr += index;

        this.state = ParsingState.Url;
    }

    private void ParseVersion()
    {
        if (this.state != ParsingState.Url)
        {
            throw new HttpParserException("Invalid parser state!");
        }

        EnsureSpace();

        ReadOnlySpan<byte> slice = this.data.Slice(ptr).Span;

        int index = slice.IndexOf((byte)0x0a) + 1;

        ReadOnlySpan<byte> line = slice[0..index];

        foreach (byte b in line)
        {
            Console.Write((char)b);
        }

        if ((line.Length != 8 || line.Length != 10) && line[line.Length - 1] != 0x0a)
        {
            throw HttpException.InternalServerError("Malformed head line");
        }

        this.state = ParsingState.Version;

        if (line.Length == 8)
        {
            if (line[5] == 0x31)
            {
                builder.WithVersion(HttpVersion.Version10);
            }
            if (line[5] == 0x32)
            {
                builder.WithVersion(HttpVersion.Version20);

            }
            if (line[5] == 0x33)
            {
                builder.WithVersion(HttpVersion.Version30);
            }

            ptr += line.Length;
            return;
        }

        if (line.Length == 10)
        {
            builder.WithVersion(HttpVersion.Version11);
            ptr += line.Length;
            return;
        }

        throw HttpException.InternalServerError("Error when parsing the http version!");

    }

    private void ParseHeader()
    {
        if (this.state != ParsingState.Version)
        {
            throw new HttpParserException("Invalid parser state!");
        }

        ReadOnlySpan<byte> slice = this.data.Slice(ptr).Span;

        if (slice.IsEmpty)
        {
            this.state = ParsingState.Finished;
            return;
        }

        if (slice[0] == 0x0a)
        {
            this.state = ParsingState.Body;
            return;
        }

        int nameLength = slice.IndexOf((byte)0x3a) - 1;
        string name = Encoding.UTF8.GetString(slice[0..nameLength]);
        ptr += nameLength + 1;

        //EnsureWhiteSpace(slice);
        ptr += 1;

        int valueLength = slice.IndexOf((byte)0x0a) - 1;
        string value = Encoding.UTF8.GetString(slice[ptr..valueLength]);
        ptr += valueLength + 1;

        builder.WithHeader(name, value);
    }

    private void ReadBody()
    {

    }

}
