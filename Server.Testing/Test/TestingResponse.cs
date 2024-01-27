using Server.Core.Application;
using Server.Core.Application.Core;
using System.Text;

namespace Server.Testing;

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