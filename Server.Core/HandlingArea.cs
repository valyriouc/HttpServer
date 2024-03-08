using Server.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using Vectorize.Core;

namespace Server.Http;

public interface IHandlingPlatform
{
    public Task MakeItHappenAsync(ReadOnlyMemory<byte> bytes, CancellationToken token);

    public Task<Stream> GetResponseAsync(CancellationToken token);
}

internal class HandlingArea : IStreamable 
{
    public IHandlingPlatform Platform { get; init; }

    public HandlingArea(IHandlingPlatform platform)
    {
        if (Platform is null)
        {
            throw new ArgumentNullException(nameof(Platform));  
        }

        Platform = platform;
    }

    public async Task<Stream> StreamThisAsync(ReadOnlyMemory<byte> bytes, CancellationToken token)
    {

    }
}

