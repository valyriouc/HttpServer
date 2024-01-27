namespace Server.Core.Application.Core;

public interface IResponse
{
    public HttpContentType ContentType { get; }

    public Task WriteToBodyAsync(Stream body);
}
