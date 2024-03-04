using System.Net;

namespace Server.Generic;

public interface IServerPlatformBuilder : IBuilder<IServerPlatform>
{
    public IServerPlatformBuilder WithAddress(IPAddress address);

    public IServerPlatformBuilder WithPort(ushort port);

}
