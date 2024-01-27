using System.Net;
using System.Text;

namespace Server.Core.Extensions;

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