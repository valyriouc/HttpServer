using Server.Core.Application.Core;

using System.Text;

namespace Server.Core.Application.Responses;

public interface ICreateableResponse<TInner>
    where TInner : IResponse
{
    public abstract static TInner Create(string content);

    public abstract static TInner Create(byte[] bytes);
}

public abstract class ResponseBase : IResponse 
{
    public HttpContentType ContentType { get; init; }

    public byte[] Content { get; init; }

    public ResponseBase(HttpContentType type, string content)
        : this(type, Encoding.UTF8.GetBytes(content))
    { 

    }

    public ResponseBase(HttpContentType type, byte[] content)
    {
        ContentType = type;
        Content = content;    
    }

    public virtual async Task WriteToBodyAsync(Stream body)
    {
        await body.WriteAsync(Content);
        await body.FlushAsync();
    }
}
