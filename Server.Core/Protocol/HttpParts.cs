namespace Server.Core.Protocol;

internal enum HttpRequestParts
{
    Method = 0,
    Url = 1,
    Version = 2,
    Header = 3,
    Body = 4
}

internal enum HttpResponseParts
{
    Version = 0,
    StatusCode = 1,
    Header = 2,
    Body = 3
}