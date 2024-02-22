using System.Net;

namespace Server.Core;

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
