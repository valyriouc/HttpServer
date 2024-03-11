using Server.Core.Http;

namespace Server.Http.Http.Version;


// HTTP 1.1
public class FirstSecondVersionRuleSystem : FirstVersionRuleSystem
{
    protected override bool IsMethodAllowed(HttpRequest request, List<ValidationResult> results)
    {
        if (!base.IsMethodAllowed(request, results) ||
            request.Method != HttpMethod.Put ||
            request.Method != HttpMethod.Patch ||
            request.Method != HttpMethod.Delete ||
            request.Method != HttpMethod.Connect ||
            request.Method != HttpMethod.Trace ||
            request.Method != HttpMethod.Options)
        {
            return false;
        }

        return true;
    }

    protected override bool HasRequiredRequestHeaders(
        HttpRequest request,
        List<ValidationResult> results)
    {
        if (!base.HasRequiredRequestHeaders(request, results) ||
            request.Headers.ContainsKey("Host"))
        {
            return false;
        }

        return true;
    }
}