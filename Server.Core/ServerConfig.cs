using System.Net;

namespace Server.Core;

internal readonly struct ServerConfig
{
    public IPAddress Address { get; }

    public ushort Port { get; }

    public ServerConfig(string ipAddress, ushort port)
    {
        Address = IPAddress.Parse(ipAddress);
        Port = port;
    }

    public IPEndPoint CreateEndpoint() =>
        new IPEndPoint(Address, Port);
}
