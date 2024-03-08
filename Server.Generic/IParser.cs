namespace Server.Generic;

public enum ParserState
{
    NotStarted = 0,
    Parsing = 1,
    Finished = 3
}

public struct ParserNode
{
    public bool IsEmpty => Content.Length == 0;

    public byte[] Content { get; }

    public ParserNode(byte[] content)
    {
        Content = content;
    }
}

public interface IParser
{
    public ParserState State { get; set; }

    public void Feed(ReadOnlyMemory<byte> payload);

    public IEnumerable<ParserNode> Parse();

    public void Deconstruct();
}
