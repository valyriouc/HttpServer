using System.Net;

namespace Server.Core;

internal readonly struct ServerConfig
{
    public IPAddress Address { get; }

    public int Port { get; }

    public ServerConfig(string ipAddress, int port)
    {
        Address = IPAddress.Parse(ipAddress);
        Port = port;
    }

    public IPEndPoint CreateEndpoint() =>
        new IPEndPoint(Address, Port);
}
