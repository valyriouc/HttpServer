using Server.Core.Application;
using Server.Core.Extensions;
using Server.Generic;

using System.Net;
using System.Text;

namespace Server.Core.Http;

public class HttpResponse : IDisposable, IToBytesConvertable
{ 
    public HttpStatusCode StatusCode { get; set; }

    public HttpContentType ContentType { get; set; }

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

    public async Task<Memory<byte>> ToBytesAsync()
    {
        List<byte> bytes = [.. Version.GetHttpVersionBytes()];

        bytes
            .ChainableSpaceWriting()
            .ChainableAdd(StatusCode.GetBytes())
            .ChainableSpaceWriting()
            .ChainableAdd(StatusCode.GetNameBytes())
            .ChainableNewLineWriting();

        Headers.Add("content-type", ContentType.ToStringType());
        Headers.Add("content-length", Body.Length.ToString());

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

        public static HttpResponse FromHttpException(HttpException ex)
    {
        HttpResponse response = new(ex.StatusCode);

        response.Body.Write(Encoding.UTF8.GetBytes(ex.Message));
        response.Body.Flush();

        return response;
    }

    public static HttpResponse FromUnexpectedException(Exception ex)
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
