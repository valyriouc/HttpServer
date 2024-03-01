namespace Server.Generic;

/// <summary>
/// Interface that builds a platform which can handle a specific (network) protocol
/// </summary>
/// <typeparam name="TPlatform"></typeparam>
public interface IProtocolPlatformBuilder<TPlatform> : IBuilder<TPlatform>
{
    public IProtocolPlatformBuilder<TPlatform> WithMethods(HashSet<string> methods);

    public IProtocolPlatformBuilder<TPlatform> WithVersions(HashSet<Version> versions);

    public IProtocolPlatformBuilder<TPlatform> WithHeaders(Dictionary<string, string[]> headers);
}