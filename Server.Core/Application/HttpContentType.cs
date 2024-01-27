
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.Application;

public enum HttpContentType
{
    Html = 0,
    Text = 1,
    Json = 2
}

internal static class HttpContentTypeExtensions
{
    public static string ToStringType(this HttpContentType contentType) => contentType switch
    {
        HttpContentType.Html => "text/html",
        HttpContentType.Text => "text/plain",
        _ => throw HttpException.InternalServerError("Content type not found!")
    };
}