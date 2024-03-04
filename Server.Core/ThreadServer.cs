using Server.Core.Logging;
using Server.Generic;

using System.Net.Sockets;

namespace Server.Core;

/// <summary>
/// Represents a basic server which waits for and handles network requests
/// </summary>
public interface IServerBackbone
{
    public Task RunAsync(CancellationToken cancellationToken);
}

/// <summary>
/// This is the implementation of a generic server backbone 
/// TODO: Implement a server base where different servers can be implemented 
/// </summary>
/// <typeparam name="THandler"></typeparam>
internal class ThreadServer : IServerBackbone, IDisposable
{

    private readonly Socket listener;
    private readonly ILogger logger;
    private readonly ServerConfig config;

    private int requestCounter;

    private readonly IProtocolPlatform handler;

    internal ThreadServer(IProtocolPlatform handler, ServerConfig config, ILogger logger)
    {
        requestCounter = 0;

        this.logger = logger;
        this.handler = handler;
        this.config = config;

        listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        List<Task> tasks = new List<Task>();

        try
        {
            listener.Bind(config.CreateEndpoint());
            listener.Listen();

            logger.Info($"Listen on port {config.Port}!");

            while (!cancellationToken.IsCancellationRequested)
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