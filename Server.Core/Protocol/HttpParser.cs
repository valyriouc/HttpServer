using System.Text;
using Vectorize.Logging;
using Vectorize.Server.Protocol;

namespace Server.Core.Protocol;

public class HttpParser : IParser
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

    public IEnumerable<ParserNode> Parse()
    {
        if (data.IsEmpty)
        {
            yield break;
        }

        if (State != ParserState.NotStarted)
        {
            throw new HttpParserException("Invalid parsing state!");
        }

        State = ParserState.Parsing;

        yield return ParseMethod();
        yield return new ParserNode(Array.Empty<byte>());

        EnsureSpace();

        yield return ParseUrl();
        yield return new ParserNode(Array.Empty<byte>());

        EnsureSpace();

        yield return ParseVersion();
        yield return new ParserNode(Array.Empty<byte>());

        EnsureNewLine();

        foreach (ParserNode header in ParseHeaders())
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

    protected ParserNode ParseMethod()
    {
        int index = data.Span.IndexOf((byte)0x20);
        ptr += index;
        return new ParserNode(data.Span[ptr..index].ToArray());
    }

    protected ParserNode ParseUrl()
    {
        if (State != ParserState.Parsing)
        {
            throw new HttpParserException("Invalid parser state!");
        }

        ReadOnlySpan<byte> slice = data.Span.Slice(ptr);

        int index = slice.IndexOf((byte)0x20);

        ptr += index;

        return new ParserNode( 
            slice[0..index].ToArray());
    }

    protected ParserNode ParseVersion()
    {
        if (State != ParserState.Parsing)
        {
            throw new HttpParserException("Invalid parser state!");
        }

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
                return new ParserNode(Encoding.UTF8.GetBytes("HTTP1"));
            }
            if (line[5] == 0x32)
            {
                return new ParserNode(Encoding.UTF8.GetBytes("HTTP2"));
            }
            if (line[5] == 0x33)
            {
                return new ParserNode(Encoding.UTF8.GetBytes("HTTP3"));
            }
        }

        if (line.Length == 9)
        {
            ptr += line.Length - 1;
            return new ParserNode(Encoding.UTF8.GetBytes("HTTP1.1"));
        }

        throw HttpException.InternalServerError("Error when parsing the http version!");
    }

    protected IEnumerable<ParserNode> ParseHeaders()
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
                yield return new ParserNode(Array.Empty<byte>());
                break;
            }

            int length = slice.IndexOf((byte)'\r') + 1;
            ptr += length - 1;

            EnsureNewLine();

            yield return new ParserNode(slice[0..length].ToArray());
        }
    }

    protected ParserNode ReadBody()
    {
        ReadOnlySpan<byte> slice = data.Span.Slice(ptr);
        int firstNull = slice.IndexOf((byte)0x00);
        return new ParserNode(slice[..firstNull].ToArray());
    }

    public void Deconstruct()
    {
        State = ParserState.NotStarted;
        this.data = null;
        ptr = 0;
    }
}