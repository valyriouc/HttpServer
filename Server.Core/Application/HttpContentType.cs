namespace Server.Core.Application;

public enum HttpContentType
{
    Html = 0,
    Text = 1,
    Json = 2,
    Css = 3,
    Js = 4
}

internal static class HttpContentTypeExtensions
{
    public static string ToStringType(this HttpContentType contentType) => contentType switch
    {
        HttpContentType.Html => "text/html",
        HttpContentType.Text => "text/plain",
        HttpContentType.Json => "application/json",
        HttpContentType.Css => "text/css",
        HttpContentType.Js => "text/javascript",
        _ => throw HttpException.InternalServerError("Content type not found!")
    };
}