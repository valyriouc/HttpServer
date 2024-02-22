using System.Text.Json;

namespace Server.Core.Extensions;

public static class MemoryStreamExtensions 
{
    public static async Task<TEntity> AsJsonAsync<TEntity>(
        this MemoryStream stream, 
        JsonSerializerOptions? options=null)
    {
        try
        {
            stream.Position = 0;
            TEntity? entity = await JsonSerializer.DeserializeAsync<TEntity>(stream, options);
            if (entity is null)
            {
                throw HttpException.BadRequest(
                    "Could not deserialize the requested object!");
            }
            return entity;
        }
        catch (JsonException ex)
        {
            throw HttpException.BadRequest(ex.Message);
        }
    }

    public static async Task<TEntity> AsCsvAsync<TEntity>(this MemoryStream stream)
    {
        throw new NotImplementedException();
    }

    public static async Task<TEntity> AsXmlAsync<TEntity>(this MemoryStream stream)
    {
        throw new NotImplementedException();
    }

    public static async Task<TEntity> AsHttpBodyAsync<TEntity>(this MemoryStream stream)
    {
        throw new NotImplementedException();
    }
}
