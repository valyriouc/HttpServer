using Server.Core;
using Server.Core.Http;

namespace Server.Http.Http.Version;

public interface IRuleSystem<TReq, TRes>
{
    public bool IsValidRequest(TReq request, out List<ValidationResult> results);

    public bool IsValidResponse(TRes response, out List<ValidationResult> results);
}

internal class VersionManager : Dictionary<HttpVersion, IHttpRuleSystem>
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
