using Server.Core.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Http.Version;

// HTTP 1.0
public class FirstVersionRuleSystem : IHttpRuleSystem
{
    public bool IsValidRequest(HttpRequest request, out List<ValidationResult> results)
    {
        bool isValid = true;
        results = new List<ValidationResult>();

        if (!IsMethodAllowed(request, results))
        {
            isValid = false;
        }

        if (!HasRequiredRequestHeaders(request, results))
        {
            isValid = false;
        }

        return isValid;
    }

    protected virtual bool IsMethodAllowed(
        HttpRequest request,
        List<ValidationResult> results)
    {
        if (request.Method != HttpMethod.Get ||
            request.Method != HttpMethod.Post ||
            request.Method != HttpMethod.Head)
        {
            results.Add(
                new ValidationResult(
                    "Method is not allowed!",
                    $"GET, POST, HEAD!"));

            return false;
        }
        return true;
    }

    protected virtual bool HasRequiredRequestHeaders(
        HttpRequest request,
        List<ValidationResult> results) => true;

    public bool IsValidResponse(
        HttpResponse response,
        out List<ValidationResult> results)
    {
        bool isValid = true;
        results = new List<ValidationResult>();

        if (!HasRequiredResponseHeaders(response, results))
        {
            isValid = false;
        }


        return isValid;
    }

    protected virtual bool HasRequiredResponseHeaders(
        HttpResponse response,
        List<ValidationResult> results)
    {
        if (!response.Headers.ContainsKey("Content-type"))
        {
            results.Add(new ValidationResult(
                "Content type is required in the response!",
                "Content-Type: <contenttype>"));

            return false;
        }

        return true;
    }
}