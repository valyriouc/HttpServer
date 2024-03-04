using Server.Generic;

namespace Server.Core;
public interface IProtocolPlatform
{
    public Task<Memory<byte>> HandleOperationAsync(Memory<byte> request);
}