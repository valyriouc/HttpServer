using Server.Core.Application;
using Server.Core.Application.Core;
using Server.Core.Http;
using Server.Core.Logging;

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server.Core;

public readonly struct HttpServerConfig
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

public class HttpServer<TApp> : IDisposable
    where TApp : IApplication
{
    private readonly Socket listener;
    private readonly ILogger logger;

    private int requestCounter;

    private readonly TApp application;

    public HttpServer(TApp app, ILogger serverLogger)
    {
        requestCounter = 0;

        logger = serverLogger;
        application = app;

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
        try
        {
            NetworkStream inStream = new NetworkStream(client);

            Memory<byte> input = await inStream.PackIntoAsync(token);
            
            logger.Info("Handling request");
            Memory<byte> response = await ProcessRequest(input);

            if (response.IsEmpty)
            {
                return;
            }

            NetworkStream stream = new NetworkStream(client, true);
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
    
    private async Task<Memory<byte>> ProcessRequest(ReadOnlyMemory<byte> data)
    {
        HttpParser parser = new HttpParser(data, this.logger);

        try
        {
            HttpRequest? request = parser.Parse();

            if (request is null)
            {
                return Memory<byte>.Empty;
            }

            IEndpoint endpoint = application.Create(request);

            IResponse content = await endpoint.ExecuteAsync(request);

            HttpResponse response = new HttpResponse();
            
            await content.WriteToBodyAsync(response.Body);

            Memory<byte> r = await response.WriteHttpAsync(content.ContentType);

            Console.WriteLine(Encoding.UTF8.GetString(r.ToArray()));
            response.Dispose();
            return r;
        }
        catch (HttpParserException ex)
        {
            logger.Error(ex);
            throw HttpException.InternalServerError("Error when reading the http data!");
        }
        catch(HttpException ex)
        {
            logger.Error(ex);
            HttpResponse response = HttpResponse.FromHttpException(ex);
            Memory<byte> r = await response.WriteHttpAsync(HttpContentType.Text);
            response.Dispose();
            return r;
        }
        catch(Exception ex)
        {
            logger.Error(ex);
            HttpResponse response = HttpResponse.FromUnexpectedException(ex); 
            Memory<byte> r = await response.WriteHttpAsync(HttpContentType.Text);
            response.Dispose();
            return r;
        }
    }

    public void Dispose()
    {
        listener.Dispose();
        GC.SuppressFinalize(this);
    }
}

internal static class StreamExtensions
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