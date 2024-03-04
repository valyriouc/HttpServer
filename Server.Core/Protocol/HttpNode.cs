namespace Server.Core.Protocol;

public enum HttpPart
{
    Method = 0,
    Url = 1,
    Version = 2,
    Header = 3,
    Body = 4
}

public struct HttpNode
{
    public HttpPart Part { get; }

    public byte[] Content { get; }

    public HttpNode(HttpPart part, byte[] content)
    {
        Part = part;
        Content = content;
    }
}
