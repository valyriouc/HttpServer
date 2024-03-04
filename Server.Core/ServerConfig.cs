using System.Net;

namespace Server.Core;

internal readonly struct ServerConfig
{
    public IPAddress Address { get; }

    public ushort Port { get; }

    public ServerConfig(IPAddress ipAddress, ushort port)
    {
        Address = ipAddress;
        Port = port;
    }

    public ServerConfig(string ipAddress, ushort port) : this(IPAddress.Parse(ipAddress), port)
    {

    }

    public IPEndPoint CreateEndpoint() =>
        new IPEndPoint(Address, Port);
}
