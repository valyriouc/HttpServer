using Server.Core;
using Server.Core.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Http.Version;

public enum HttpVersion
{
    Http1,
    Http11,
    Http2,
    Http3
}

internal static class HttpVersionExtensions
{
    public static string ToString(this HttpVersion version) => version switch
    {
        HttpVersion.Http1 => "Http/1",
        HttpVersion.Http11 => "Http/1.1",
        HttpVersion.Http2 => "Http/2",
        HttpVersion.Http3 => "Http/3"
    };
}

public struct ValidationResults
{
    public string Message { get; init; }

    public string Expected { get; init; }
}

public interface IRuleSystem<T>
{
    public bool TryValidate(T input, out List<ValidationResults> results);
}

public interface IHttpRuleSystem : IRuleSystem<HttpRequest>
{

}

// HTTP 1.0
public class FirstVersionRuleSystem : IHttpRuleSystem
{
    public bool TryValidate(HttpRequest input, out List<ValidationResults> results)
    {
        results = new List<ValidationResults>();
        return true;
    }
}

// HTTP 1.1
public class FirstSecondVersionRuleSystem : FirstVersionRuleSystem
{
    public bool TryValidate(HttpRequest input, out List<ValidationResults> results)
    {
        if (!base.TryValidate(input, out results))
            return false;

        // TODO: Host-header is required!!!
        
        return true;
    }
}

public class VersionManager : Dictionary<HttpVersion, IHttpRuleSystem>
{
    public IHttpRuleSystem GetRuleSet(HttpVersion version)
    {
        if (!this.ContainsKey(version))
        {
            throw HttpException
                .BadRequest($"Http version {version.ToString()} is not supported!");
        }

        return this[version];
    }
}
