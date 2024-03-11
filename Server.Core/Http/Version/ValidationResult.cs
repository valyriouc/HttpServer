using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Http.Version;

public struct ValidationResult
{
    public string Message { get; init; }

    public string Expected { get; init; }

    public ValidationResult(string message, string expected)
    {
        this.Message = message;
        this.Expected = expected;
    }
}