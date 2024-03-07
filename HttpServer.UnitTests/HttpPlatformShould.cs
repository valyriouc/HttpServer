using HttpServer.ServerConsole.Logging;

using Server.Core;
using Server.Core.Application.Core;
using Server.Core.Application.Responses;
using Server.Core.Http;
using Server.Core.Protocol;
using Server.Generic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.UnitTests;

internal class TestEndpoint : IEndpoint
{
    public async Task<IResponse> ExecuteAsync(HttpRequest request)
    {

        if (request.Method == HttpMethod.Post)
        {
            return new JsonResponse("{\"name\": \"alpha\"}");
        }
        else
        {
            return new JsonResponse("{\"name\":\"beta\"}");
        }
    }
}

internal class TestApp : IApplication
{
    public IEndpoint Create(HttpRequest request)
    {
        return new TestEndpoint();
    }
}

public class HttpPlatformShould
{

    [Fact]
    public async Task ThrowsExceptionWhenMethodIsNotRegistered()
    {
        IProtocolPlatform<HttpResponse> platform = new HttpProtocolPlatformBuilder()
            .WithApplicaton(new TestApp())
            .WithParser(() => new HttpParser(new LogOntoConsole(true)))
            .WithMethod("GET")
            .WithMethod("POST")
            .Build();

        HttpRequest request = new()
        {
            Method = HttpMethod.Put,
            Url = new HttpResourceIdentifier("/"),
            Headers = new()
            {
                { "Content-Type", "application/json" },
                { "Accept", "application/json" }
            },

        };

        HttpResponse res = await platform.HandleOperationAsync(request.ToBytes());

        Assert.Equal(HttpStatusCode.InternalServerError, res.StatusCode);
    }
}

internal static class HttpRequestExtensions
{
    public static byte[] ToBytes(this HttpRequest request)
    {
        List<byte> bytes = [.. Encoding.UTF8.GetBytes(request.Method.Method)];

        bytes.Add((byte)' ');

        bytes.AddRange(Encoding.UTF8.GetBytes(request.Url.Unformatted));

        bytes.Add((byte)' ');

        bytes.AddRange(Encoding.UTF8.GetBytes("HTTP/1.1\r\n"));

        foreach((string name, string value) in request.Headers)
        {
            bytes.AddRange(Encoding.UTF8.GetBytes($"{name}: {value}\r\n"));
        }

        return bytes.ToArray();
    }
}
