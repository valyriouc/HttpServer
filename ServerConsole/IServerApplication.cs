using ServerConsole;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ServerConsole;

internal enum HttpContentType
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

internal interface IResponse
{
    public HttpContentType ContentType { get; }

    public Task WriteToBodyAsync(Stream body);
}

internal interface IEndpoint
{
    public Task<IResponse> ExecuteAsync(HttpRequest request);
}

/// <summary>
/// Just a config on how to reacting to 
/// </summary>
internal interface IServerApplication
{
    public IEndpoint Create(HttpRequest request);
}

internal sealed class TestingResponse : IResponse
{
    public HttpContentType ContentType => HttpContentType.Html;

    public async Task WriteToBodyAsync(Stream body)
    {
        string content = """
            <html>
                <body>
                    <title>Serving</title>
                </body>
                <head>
                    <h1>Hello from my server</h1>
                </head>
            </html>
            """;

        body.Write(Encoding.UTF8.GetBytes(content));
        await body.FlushAsync();
        body.Position = 0;
    }
}

internal sealed class TestingEndpoint : IEndpoint
{
    public async Task<IResponse> ExecuteAsync(HttpRequest request)
    {
        await Task.CompletedTask;
        return new TestingResponse();
    }
}

internal sealed class TestingApplication : IServerApplication
{
    public IEndpoint Create(HttpRequest request)
    {
        Console.WriteLine($"Path: {request.Path}");

        switch(request.Path)
        {
            case "/":
                return new TestingEndpoint();
                break;
            default:
                throw HttpException.NotFound(string.Empty);
                break;
        }
    }
}
