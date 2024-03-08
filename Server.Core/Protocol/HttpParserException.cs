namespace Server.Core.Protocol;

internal class HttpParserException : Exception
{
    public HttpParserException(string? message)
        : base(message)
    {
    }
}