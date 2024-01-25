using HttpServer.ServerConsole.Logging;

using System.Net;
using System.Net.Http;
using System.Net.Sockets;

namespace HttpServer.ServerConsole;

internal readonly struct HttpServerConfig
{
    public IPAddress Address { get; }

    public int Port { get; }

    public HttpServerConfig(string ipAddress, int port)
    {
        Address = IPAddress.Parse(ipAddress);
        Port = port;
    }

    public IPEndPoint CreateEndpoint() => 
        new IPEndPoint(Address, Port);
   
}

internal class HttpServer<TApp> : IDisposable
    where TApp : IServerApplication
{
    private readonly Socket listener;
    private readonly ILogger logger;

    private int requestCounter;

    public HttpServer(ILogger serverLogger)
    {
        requestCounter = 0;

        logger = serverLogger;
        listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task<int> RunAsync(
        HttpServerConfig config, 
        CancellationToken token)
    {
        try
        {
            listener.Bind(config.CreateEndpoint());
            listener.Listen();

            logger.Info($"Listen on port {config.Port}!");

            while (true)
            {
                Socket client = await listener.AcceptAsync(token);
                requestCounter += 1;

                await Task.Run(async () =>
                {
                    Socket socket = new Socket(client.SafeHandle);
                    await HandleRequestAsync(socket, token);
                });

                client.Close();

                logger.Info($"Request number {requestCounter} is now be in progress!");
            }

            return 0;
        }
        catch(Exception ex)
        {
            logger.Error(ex);
            return -1;
        }
    } 

    private async Task HandleRequestAsync(Socket client, CancellationToken token)
    { 
        NetworkStream outStream = new NetworkStream(client);

        MemoryStream clientToServer = await outStream.PackIntoAsync(token);

        try
        {
            using MemoryStream serverToClient = ProcessRequest(clientToServer);
            using NetworkStream stream = await serverToClient.PackIntoAsync(client, token);

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
    
    private MemoryStream ProcessRequest(MemoryStream stream)
    {
        
    }

    public void Dispose()
    {
        listener.Dispose();
        GC.SuppressFinalize(this);
    }
}

internal static class StreamExtensions
{
    public static async Task<MemoryStream> PackIntoAsync(
        this NetworkStream stream, 
        CancellationToken token)
    {
        MemoryStream os = new MemoryStream();
        Memory<byte> buffer = new Memory<byte>();

        while (true)
        {
            if (!stream.DataAvailable)
            {
                break;
            }

            int _ = await stream.ReadAsync(buffer, token);
            os.Write(buffer.Span);
        }

        return os;
    }

    public static async Task<NetworkStream> PackIntoAsync(
        this MemoryStream stream, 
        Socket socket, 
        CancellationToken token)
    {
        Memory<byte> buffer = new();
        await stream.ReadAsync(buffer, token);

        NetworkStream os = new NetworkStream(socket, true);
        await os.WriteAsync(buffer, token);
        return os;
    }
}