namespace Server.Generic;

public interface IProtocolPlatform<TResponse> where TResponse : IToBytesConvertable
{
    public Task<TResponse> HandleOperationAsync(Memory<byte> request);
}