using Server.Core.Http;
using Server.Core.Logging;

using System.Net;
using System.Text;

namespace Server.Core;

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

    protected readonly ReadOnlyMemory<byte> data;
                
    private readonly ILogger logger;
    private readonly HttpRequestBuilder builder;

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
        EnsureNewLine();

        if (this.state != ParsingState.Version)
        {
            throw new HttpParserException("Invalid parser state!");
        }

        do
        {
            ParseHeader();
        } while (this.state == ParsingState.Headers);

        if (this.state == ParsingState.Body)
        {
            ReadBody();
        }
        
        return builder.Build();
    } 

    private void EnsureSpace()
    {
        if (this.data.Span[ptr] != 0x20)
        {
            throw new HttpParserException("Expected a space!");
        }
        else
        {
            ptr += 1;
        }
    }

    private void EnsureNewLine()
    {
        if (this.data.Span[ptr] == (byte)'\r' && 
            this.data.Span[ptr+1] == (byte)'\n')
        {
            ptr += 2;
            return;
        }

        if (this.data.Span[ptr] == (byte)'\n' || 
            this.data.Span[ptr] == (byte)'\r')
        {
            ptr += 1;
            return;
        }

        throw new HttpParserException("Expected a new line!");
    }

    protected internal void ParseMethod()
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
    
    protected internal void ParseUrl()
    {
        if (this.state != ParsingState.Method)
        {
            throw new HttpParserException("Invalid parser state!");
        }

        EnsureSpace();

        logger.Warn(ptr.ToString());

        ReadOnlySpan<byte> slice = this.data.Span.Slice(ptr);

        int index = slice.IndexOf((byte)0x20);

        builder.WithUrl(Encoding.UTF8.GetString(slice[0..index]));

        ptr += index;

        this.state = ParsingState.Url;
    }

    protected internal void ParseVersion()
    {
        if (this.state != ParsingState.Url)
        {
            throw new HttpParserException("Invalid parser state!");
        }

        EnsureSpace();

        ReadOnlySpan<byte> slice = this.data.Span.Slice(ptr);

        int index = slice.IndexOf((byte)'\r') + 1;

        ReadOnlySpan<byte> line = slice[0..index];

        if ((line.Length != 7 || 
            line.Length != 9) && 
            line[line.Length - 1] != (byte)'\r')
        {
            throw HttpException.InternalServerError(
                "Malformed head line");
        }

        this.state = ParsingState.Version;

        if (line.Length == 7)
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

            ptr += line.Length - 1;
            return;
        }

        if (line.Length == 9)
        {
            builder.WithVersion(HttpVersion.Version11);
            ptr += line.Length - 1;
            return;
        }

        throw HttpException.InternalServerError("Error when parsing the http version!");
    }

    protected internal void ParseHeader()
    {
        this.state = ParsingState.Headers;

        ReadOnlySpan<byte> slice = this.data.Span.Slice(ptr);

        if (slice.IsEmpty)
        {
            this.state = ParsingState.Finished;
            return;
        }

        if (slice[0] == (byte)'\r' && slice[1] == (byte)'\n')
        {
            this.state = ParsingState.Body;
            return;
        }

        int nameLength = slice.IndexOf((byte)0x3a);
        string name = Encoding.UTF8.GetString(slice[0..nameLength]);
        ptr += nameLength + 1;

        EnsureSpace();

        slice = this.data.Span.Slice(ptr);

        int valueLength = slice.IndexOf((byte)'\r') + 1;
        string value = Encoding.UTF8.GetString(slice[0..valueLength]);
        ptr += valueLength - 1;

        EnsureNewLine();

        builder.WithHeader(name.Trim(), value.Trim());
    }

    protected void ReadBody()
    {
        ReadOnlySpan<byte> slice = this.data.Span.Slice(ptr);
        int firstNull = slice.IndexOf((byte)0x00);
        builder.WithBody(slice[..firstNull]);
    }
}
