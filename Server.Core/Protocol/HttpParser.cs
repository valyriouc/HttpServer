using Server.Core.Logging;
using Server.Generic;

using System.Text;

namespace Server.Core.Protocol;

internal class HttpParser : IParser<HttpNode>, ILoggeble
{
    protected ReadOnlyMemory<byte> data;
    private int ptr;

    public ILogger Logger { get; }

    public ParserState State { get; set; }

    public HttpParser(ILogger logger)
    {
        ptr = 0;
        State = ParserState.NotStarted;
        Logger = logger;
    }

    public void Feed(ReadOnlyMemory<byte> payload)
    {
        this.data = payload;
    }


    public IEnumerable<HttpNode> Parse()
    {
        if (data.IsEmpty)
        {
            throw new HttpParserException("Invalid parser data!");
        }

        if (State != ParserState.NotStarted)
        {
            throw new HttpParserException("Invalid parsing state!");
        }

        State = ParserState.Parsing;

        yield return ParseMethod();
        yield return ParseUrl();
        yield return ParseVersion();
        
        EnsureNewLine();

        foreach (HttpNode header in ParseHeaders())
        {
            yield return header;
        }

        if (State == ParserState.Parsing)
        {
            yield return ReadBody();
        }
    }

    private void EnsureSpace()
    {
        if (data.Span[ptr] != 0x20)
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
        if (data.Span[ptr] == (byte)'\r' &&
            data.Span[ptr + 1] == (byte)'\n')
        {
            ptr += 2;
            return;
        }

        if (data.Span[ptr] == (byte)'\n' ||
            data.Span[ptr] == (byte)'\r')
        {
            ptr += 1;
            return;
        }

        throw new HttpParserException("Expected a new line!");
    }

    protected HttpNode ParseMethod()
    {
        switch (data.Span[0])
        {
            case 0x47:
                {
                    Logger.Info("Read get method!");
                    ptr += 3;
                    return new HttpNode(HttpPart.Method, Encoding.UTF8.GetBytes("GET"));
                }
            case 0x50:
                {
                    if (data.Span[1] == 0x4f)
                    {
                        Logger.Info("Read post method!");
                        ptr += 4;
                        return new HttpNode(HttpPart.Method, Encoding.UTF8.GetBytes("POST"));
                    }
                    else
                    {
                        throw new NotImplementedException("Http method is not supported!");
                    }
                }
            default:
                throw new NotImplementedException("Http method is not supported!");
        }
    }

    protected HttpNode ParseUrl()
    {
        if (State != ParserState.Parsing)
        {
            throw new HttpParserException("Invalid parser state!");
        }

        EnsureSpace();

        ReadOnlySpan<byte> slice = data.Span.Slice(ptr);

        int index = slice.IndexOf((byte)0x20);

        ptr += index;

        return new HttpNode(
            HttpPart.Url, 
            slice[0..index].ToArray());
    }

    protected HttpNode ParseVersion()
    {
        if (State != ParserState.Parsing)
        {
            throw new HttpParserException("Invalid parser state!");
        }

        EnsureSpace();

        ReadOnlySpan<byte> slice = data.Span.Slice(ptr);

        int index = slice.IndexOf((byte)'\r') + 1;

        ReadOnlySpan<byte> line = slice[0..index];

        if ((line.Length != 7 ||
            line.Length != 9) &&
            line[line.Length - 1] != (byte)'\r')
        {
            throw HttpException.InternalServerError(
                "Malformed head line");
        }

        if (line.Length == 7)
        {
            ptr += line.Length - 1;

            if (line[5] == 0x31)
            {
                return new HttpNode(HttpPart.Version, Encoding.UTF8.GetBytes("HTTP1"));
            }
            if (line[5] == 0x32)
            {
                return new HttpNode(HttpPart.Version, Encoding.UTF8.GetBytes("HTTP2"));
            }
            if (line[5] == 0x33)
            {
                return new HttpNode(HttpPart.Version, Encoding.UTF8.GetBytes("HTTP3"));
            }
        }

        if (line.Length == 9)
        {
            ptr += line.Length - 1;
            return new HttpNode(HttpPart.Version, Encoding.UTF8.GetBytes("HTTP1.1"));
        }

        throw HttpException.InternalServerError("Error when parsing the http version!");
    }

    protected IEnumerable<HttpNode> ParseHeaders()
    {
        if (State != ParserState.Parsing)
        {
            throw new HttpParserException(
                "Invalid parser state!");
        }

        while (true)
        {
            ReadOnlySpan<byte> slice = data.Span.Slice(ptr);

            if (slice.IsEmpty)
            {
                State = ParserState.Finished;
                break;
            }

            if (slice[0] == (byte)'\r' && slice[1] == (byte)'\n')
            {
                break;
            }

            int length = slice.IndexOf((byte)'\r') + 1;
            ptr += length - 1;

            EnsureNewLine();

            yield return new HttpNode(HttpPart.Header, slice[0..length].ToArray());
        }
    }

    protected HttpNode ReadBody()
    {
        ReadOnlySpan<byte> slice = data.Span.Slice(ptr);
        int firstNull = slice.IndexOf((byte)0x00);
        return new HttpNode(HttpPart.Body, slice[..firstNull].ToArray());
    }

    public void Deconstruct()
    {
        State = ParserState.NotStarted;
        this.data = null;
        ptr = 0;
    }
}
