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

    public IEnumerable<TNode> Parse();
}
