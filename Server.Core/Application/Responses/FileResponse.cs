using Server.Core.Application.Core;

namespace Server.Core.Application.Responses;

public sealed class FileResponse
{
    public string Filepath { get; init; }

    private FileResponse(string filepath)
    {
        if (!File.Exists(filepath))
        {
            throw HttpException.NotFound(
                "File does not exists!");
        }

        Filepath = filepath;
    }

    private async Task<byte[]> ReadAsync() => 
        await File.ReadAllBytesAsync(Filepath);

    public static async Task<TResponse> FromAsync<TInner, TResponse>(string filepath)
        where TInner : ICreateableResponse<TResponse>
        where TResponse : IResponse
    {
        FileResponse res = new FileResponse(filepath);

        byte[] fileContent = await res.ReadAsync();

        TResponse inner = TInner.Create(fileContent);

        return inner;
    } 
}
