namespace Server.Http.Http.Version;

public enum HttpVersion
{
    Http09,
    Http1,
    Http11,
    Http2,
    Http3
}

internal static class HttpVersionExtensions
{
    public static string ToString(this HttpVersion version) => version switch
    {

        HttpVersion.Http1 => "Http/1",
        HttpVersion.Http11 => "Http/1.1",
        HttpVersion.Http2 => "Http/2",
        HttpVersion.Http3 => "Http/3"
    };
}

