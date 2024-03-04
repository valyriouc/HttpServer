
namespace Server.Generic;

/// <summary>
/// Interface that builds a platform which can handle a specific (network) protocol
/// </summary>
/// <typeparam name="TPlatform"></typeparam>
public interface IProtocolPlatformBuilder<TProtocol> : IBuilder<TProtocol> 
    where TProtocol : IProtocolPlatform
{
    public IProtocolPlatformBuilder<TProtocol> WithMethod(string method);

    public IProtocolPlatformBuilder<TProtocol> WithVersion(Version version);

    public IProtocolPlatformBuilder<TProtocol> WithoutHeader(string name, HashSet<string> values);
}
