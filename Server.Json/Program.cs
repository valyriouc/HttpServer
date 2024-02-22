using Server.Core;
using Server.Core.Http;
using Server.Core.Application.Core;
using Server.Core.Application.Responses;
using Server.Core.Extensions;
using Server.Core.Logging;
using HttpServer.ServerConsole.Logging;
using System.Text.Json;

namespace Server.Json;

internal class Credentials
{
    public string Username { get; set; }

    public string Password { get; set; }    

    public Credentials()
    {

    }
}

internal class PostEndpoint : IEndpoint
{
    public async Task<IResponse> ExecuteAsync(HttpRequest request)
    {  
        if (request.Method == HttpMethod.Post)
        {
            if (request.Body is null)
            {
                throw HttpException.BadRequest(
                    "Expected a body!");
            }

            Credentials creds = await request.Body!.AsJsonAsync<Credentials>(new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });

            Console.WriteLine($"Username: {creds.Username}");
            Console.WriteLine($"Password: {creds.Password}");
            return JsonResponse.From(creds);
        }
        else
        {
            return new JsonResponse("{\"hello\": 1223}");
        }
    }
}

internal class ServerApplication : IApplication
{
    public IEndpoint Create(HttpRequest request) => request.Path switch
    {
        "/" => new PostEndpoint(),
        _ => throw HttpException.NotFound(string.Empty)
    };
}

internal static class Program
{
    public static async Task Main()
    {
        ServerApplication app = new ServerApplication();

        ILogger logger = new LogOntoConsole(true);

        HttpServer<ServerApplication> server = new HttpServer<ServerApplication>(app, logger);

        HttpServerConfig config = new HttpServerConfig("127.0.0.1", 3233);

        CancellationTokenSource source = new CancellationTokenSource();

        await server.RunAsync(config, source.Token);
    }
}