namespace Server.Core;

internal class HttpParserException : Exception
{
    public HttpParserException(string? message) 
        : base(message)
    {
    }
}