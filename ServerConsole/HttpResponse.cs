using HttpServer.ServerConsole;

using System.Net;
using System.Text;

namespace ServerConsole;

internal class HttpResponse : IDisposable
{ 
    public HttpStatusCode StatusCode { get; set; }

    public HttpHeaderDictionary Headers { get; }

    public Version Version { get; } = HttpVersion.Version11;

    public Stream Body { get; init; }

    public HttpResponse()
    {
        StatusCode = HttpStatusCode.OK;
        Headers = new HttpHeaderDictionary();
        Body = new MemoryStream();
        Body.Position = 0;
    }

    private HttpResponse(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
        Headers = new HttpHeaderDictionary();
        Body = new MemoryStream();
        Body.Position = 0;
    }

    internal async Task<Memory<byte>> WriteHttpAsync(HttpContentType type)
    {
        List<byte> bytes = [.. Version.GetHttpVersionBytes()];

        bytes
            .ChainableSpaceWriting()
            .ChainableAdd(StatusCode.GetBytes())
            .ChainableSpaceWriting()
            .ChainableAdd(StatusCode.GetNameBytes())
            .ChainableNewLineWriting();

        Headers.Add("content-type", type.ToStringType());
        Headers.Add("content-length", Body.Length.ToString());

        // Writing the headers 
        IEnumerable<byte> headers = Headers.ConvertTo();
        bytes.AddRange(headers);
        
        if (Body.Length != 0)
        {
            Body.Position = 0;
            bytes.Add(0x0a);

            Memory<byte> bodyBytes = new byte[Body.Length];
            await Body.ReadAsync(bodyBytes);
            bytes.AddRange(bodyBytes.ToArray());
        }

        await Body.DisposeAsync();

        return bytes.ToArray();
    }

    internal static HttpResponse FromHttpException(HttpException ex)
    {
        HttpResponse response = new(ex.StatusCode);

        response.Body.Write(Encoding.UTF8.GetBytes(ex.Message));
        response.Body.Flush();

        return response;
    }

    internal static HttpResponse FromUnexpectedException(Exception ex)
    {
        return FromHttpException(HttpException.InternalServerError(ex.Message, ex));
    }

    public void Dispose()
    {
        Body.Dispose();
        GC.SuppressFinalize(this);
    }
}

internal static class HttpHeaderDictionaryExtensions
{
    public static IEnumerable<byte> ConvertTo(this HttpHeaderDictionary headers)
    {
        foreach (KeyValuePair<string, string> pair in headers)
        {
            foreach (byte b in Encoding.UTF8.GetBytes($"{pair.Key}: {pair.Value}\n"))
            {
                yield return b;
            }
        }
    }
}

internal static class ListExtensions
{
    public static List<byte> ChainableSpaceWriting(this List<byte> list)
    {
        list.Add((byte)0x20);
        return list;
    }

    public static List<byte> ChainableNewLineWriting(this List<byte> list)
    {
        list.Add((byte)0x0a);
        return list;
    }

    public static List<byte> ChainableAdd(this List<byte> list, byte item)
    {
        list.Add(item);
        return list;
    }

    public static List<byte> ChainableAdd(this List<byte> list, byte[] items)
    {
        foreach (byte item in items)
        {
            list.ChainableAdd(item);
        }

        return list;
    }
}

internal static class VersionExtensions
{
    public static byte[] GetHttpVersionBytes(this Version version)
    {
        if (version.Minor == 1 && version.Major == 1)
        {
            return Encoding.UTF8.GetBytes("HTTP/1.1");
        }
        else
        {
            return Encoding.UTF8.GetBytes($"HTTP/{version.Major}");
        }
    }
}

internal static class HttpStatusCodeExtensions
{
    public static byte[] GetBytes(this HttpStatusCode statusCode)
    {
        return Encoding.UTF8.GetBytes(((int)statusCode).ToString());
    }

    public static byte[] GetNameBytes(this HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.OK => Encoding.UTF8.GetBytes("OK, here we are!"),
        HttpStatusCode.BadRequest => Encoding.UTF8.GetBytes("This is very bad!"),
        HttpStatusCode.NotFound => Encoding.UTF8.GetBytes("We couldn't find it!"),
        HttpStatusCode.MethodNotAllowed => Encoding.UTF8.GetBytes("What are you trying to do!"),
        HttpStatusCode.Forbidden => Encoding.UTF8.GetBytes("Do you really thought so!"),
        HttpStatusCode.InternalServerError => Encoding.UTF8.GetBytes("Complete system failure!"),
        _ => Encoding.UTF8.GetBytes(statusCode.ToString()),
    };
}