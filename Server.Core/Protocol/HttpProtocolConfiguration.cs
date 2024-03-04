namespace Server.Core.Protocol;

/// <summary>
/// Configuration for the http protocol
/// </summary>
public struct HttpProtocolConfigurations
{
    public HashSet<string> Methods { get; }

    public HashSet<Version> Versions { get; }

    public Dictionary<string, HashSet<string>> ForbiddenHeaders { get; }

    public HttpProtocolConfigurations()
    {
        Methods = new HashSet<string>();
        Versions = new HashSet<Version>();
        ForbiddenHeaders = new Dictionary<string, HashSet<string>>();    
    }
}
