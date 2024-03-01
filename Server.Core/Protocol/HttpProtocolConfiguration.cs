using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.Protocol;

/// <summary>
/// Configuration for the http protocol
/// </summary>
public struct HttpProtocolConfigurations
{
    public HashSet<string> Methods { get; }

    public HashSet<Version> Versions { get; }

    public Dictionary<string, string[]> AllowedHeaders { get; }

    public HttpProtocolConfigurations()
    {
        Methods = new HashSet<string>();
        Versions = new HashSet<Version>();
    }
}
