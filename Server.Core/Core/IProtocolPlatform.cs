using Server.Core.Application.Core;

namespace Server.Core;
public interface IProtocolPlatform
{
    public void Configure(IApplication application);

    public Task<Memory<byte>> HandleOperationAsync(Memory<byte> request);
}