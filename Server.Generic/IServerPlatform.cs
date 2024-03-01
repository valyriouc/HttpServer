namespace Server.Generic;

/// <summary>
/// Inteface which represents a server platform module 
/// Server platform encapsulate should encapsulate a tcp module 
/// and a protocol handler 
/// </summary>
public interface IServerPlatform
{
    public Task StartAsync(CancellationToken cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken);

    public Task RestartAsync(CancellationToken cancellationToken);
}
