namespace Server.Core;
public interface IProtocolHandler
{
    public Task<Memory<byte>> HandleOperationAsync(Memory<byte> request);
}