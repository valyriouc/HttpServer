using Server.Core.Application.Core;
using Server.Core.Application.Responses;
using Server.Core.Http;
using Server.Core;

namespace Server.Static;

internal class StaticEndpoint : IEndpoint
{
    public string RootDir { get; init; }

    public StaticEndpoint(string rootDir)
    {
        if (!Directory.Exists(rootDir))
        {
            throw HttpException.NotFound(
                "Base directory does not exists!");
        }

        RootDir = rootDir;
    }
    
    public async Task<IResponse> ExecuteAsync(HttpRequest request)
    {
        if (request.Path == "/")
        {
            return await FileResponse.FromAsync<HtmlResponse, HtmlResponse>(
                Path.Combine(RootDir, "index.html"));
        }

        string resource = request.Path.Substring(1);
        string path = Path.Combine(RootDir, resource);

        Console.WriteLine(RootDir);
        Console.WriteLine(path);

        if (!File.Exists(path))
        {
            throw HttpException.NotFound(
                $"File {request.Path} does not exists!");
        }


        switch (Path.GetExtension(path).Replace(".", string.Empty))
        {
            case "css":
                return await FileResponse.FromAsync<CssResponse, CssResponse>(path);
            case "html":
                return await FileResponse.FromAsync<HtmlResponse, HtmlResponse>(path);
            case "js":
                return await FileResponse.FromAsync<JavaScriptResponse, JavaScriptResponse>(path);
            default:
                throw HttpException.BadRequest("File not supported!");
        }
    }
}