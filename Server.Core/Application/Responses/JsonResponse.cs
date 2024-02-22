using System.Text.Json;

namespace Server.Core.Application.Responses;

public class JsonResponse : ResponseBase
{
    public JsonResponse(string content) 
        : base(HttpContentType.Json, content)
    {
    }

    public JsonResponse(byte[] content) 
        : base(HttpContentType.Json, content)
    {
    }

    public static JsonResponse From<TEntity>(TEntity entity)
        where TEntity : class
    {
        string json = JsonSerializer.Serialize<TEntity>(entity);
        return new JsonResponse(json);
    }

    public static JsonResponse From<TEntity>(IEnumerable<TEntity> entities)
    {
        string json = JsonSerializer.Serialize<IEnumerable<TEntity>>(entities);
        return new JsonResponse(json);
    }
}
