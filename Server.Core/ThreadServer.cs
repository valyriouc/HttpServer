using Server.Core.Logging;
using Server.Generic;
using System.Net.Sockets;

namespace Server.Core;

/// <summary>
/// Represents a basic server which waits and handles network requests
/// </summary>
public interface IServerBackbone<THandler> : IServerPlatform
{

}

/// <summary>
/// This is the implementation of a generic server backbone 
/// TODO: Implement a server base where different servers can be implemented 
/// </summary>
/// <typeparam name="THandler"></typeparam>
internal class ThreadServer<THandler> : IServerBackbone<THandler>, IDisposable
    where THandler : IProtocolPlatform
{
    private readonly Socket listener;
    private readonly ILogger logger;
    private readonly ServerConfig config;

    private int requestCounter;
    private bool isRunning = true;

    private readonly THandler handler;

    internal ThreadServer(THandler handler, ServerConfig config, ILogger logger)
    {
        requestCounter = 0;

        this.logger = logger;
        this.handler = handler;
        this.config = config;

        listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        List<Task> tasks = new List<Task>();

        try
        {
            listener.Bind(config.CreateEndpoint());
            listener.Listen();

            logger.Info($"Listen on port {config.Port}!");

            while (isRunning)
            {
                Socket client = await listener.AcceptAsync(cancellationToken);
                requestCounter += 1;

                Task t = Task.Run(async () =>
                {
                    using (client)
                    {
                        await HandleRequestAsync(client, cancellationToken);
                    }
                });

                logger.Info($"Request number {requestCounter} is now be in progress!");

                tasks.Add(t);
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex);
        }
        finally
        {
            await Task.WhenAll(tasks);
        }
    }

    private async Task HandleRequestAsync(Socket client, CancellationToken token)
    { 
        try
        {
            NetworkStream stream = new NetworkStream(client);

            Memory<byte> request = await stream.PackIntoAsync(token);
            
            logger.Info("Handling request...");
            Memory<byte> response = await handler.HandleOperationAsync(request);
            logger.Info("Handled request successfully!");

            await stream.WriteAsync(response);

            stream.Flush();
        }
        catch (Exception ex)
        {
            logger.Error(ex);
        }
        finally
        {
            client.Close();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RestartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        listener.Dispose();
        GC.SuppressFinalize(this);
    }
}

file static class StreamExtensions
{
    public static async Task<Memory<byte>> PackIntoAsync(
        this NetworkStream stream, 
        CancellationToken token)
    {
        List<byte> bytes = new List<byte>();

        while (true)
        {
            if (!stream.DataAvailable)
            {
                break;
            }

            byte[] buffer = new byte[256];
            int _ = await stream.ReadAsync(buffer, token);
            bytes.AddRange(buffer);
        }

        return bytes.ToArray();
    }
}