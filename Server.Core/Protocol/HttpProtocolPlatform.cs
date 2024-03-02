using Server.Core.Application.Core;

namespace Server.Core.Protocol;

/// <summary>
/// Class which represents a module that is capabable of handling http 
/// </summary>
public class HttpProtocolPlatform : IProtocolPlatform
{
    public Dictionary<string, IApplication> Applications { get; }

    public HashSet<Middleware> RequestPipeline { get; }

    public HttpProtocolConfigurations Config { get; }

    public HttpProtocolPlatform()
    {
        Applications = new Dictionary<string, IApplication>();
        RequestPipeline = new HashSet<Middleware>();
    }

    public async Task<Memory<byte>> HandleOperationAsync(Memory<byte> request)
    {
        throw new NotImplementedException();
    }
}
