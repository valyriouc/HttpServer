namespace Server.Generic;

public interface IProtocolPlatform
{
    public Task<Memory<byte>> HandleOperationAsync(Memory<byte> request);
}