namespace Server.Generic;

public enum ParserState
{
    NotStarted = 0,
    Parsing = 1,
    Finished = 3
}

public interface IParser<TNode>
{
    public ParserState State { get; set; }

    public void Feed(ReadOnlyMemory<byte> payload);

    public IEnumerable<TNode> Parse();

    public void Deconstruct();
}
