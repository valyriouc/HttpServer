using Vectorize.Logging;
using Vectorize.Server.Protocol;

namespace Server.Http.Protocol;

// TODO: This can be a generic generator
internal sealed class HttpGenerator : ILoggable
{
    public ILogger Logger { get; }

    public HttpGenerator(ILogger logger)
    {
        Logger = logger;
    }

    public ReadOnlyMemory<byte> Generate(IEnumerable<ParserNode> input)
    {
        List<byte> bytes = new List<byte>();

        foreach (ParserNode node in input)
        {
            bytes.AddRange(node.Content);
        }

        return bytes.ToArray();
    }
}
