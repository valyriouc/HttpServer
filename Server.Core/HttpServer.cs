using Server.Core.Logging;
using System.Net.Sockets;

namespace Server.Core;

internal class HttpServer<THandler> : IDisposable
    where THandler : IProtocolHandler
{
    private readonly Socket listener;
    private readonly ILogger logger;

    private int requestCounter;

    private readonly THandler handler;

    internal HttpServer(THandler app, ILogger serverLogger)
    {
        requestCounter = 0;

        logger = serverLogger;
        handler = app;

        listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task<int> RunAsync(
        HttpServerConfig config, 
        CancellationToken token)
    {
        List<Task> tasks = new List<Task>();

        try
        {
            listener.Bind(config.CreateEndpoint());
            listener.Listen();

            logger.Info($"Listen on port {config.Port}!");

            while (true)
            {
                Socket client = await listener.AcceptAsync(token);
                requestCounter += 1;

                Task t = Task.Run(async () =>
                {
                    using (client)
                    {
                        await HandleRequestAsync(client, token);
                    }
                });

                logger.Info($"Request number {requestCounter} is now be in progress!");

                tasks.Add(t);
            }
        }
        catch(Exception ex)
        {
            logger.Error(ex);
            return -1;
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

            if (response.IsEmpty)
            {
                logger.Warn("Response was empty!");
                return;
            }

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