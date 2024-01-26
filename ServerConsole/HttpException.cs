using System.Net;

namespace ServerConsole;

internal sealed class HttpException : Exception
{
    public HttpStatusCode StatusCode { get; }

    private HttpException(HttpStatusCode statusCode, string? message) 
        : base(message)
    {
        StatusCode = statusCode;
    }

    private HttpException(HttpStatusCode statusCode, string? message, Exception? innerException) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }

    public static HttpException Forbidden(string message) =>
        new HttpException(HttpStatusCode.Forbidden, message);

    public static HttpException MethodNotAllowed(string message) => 
        new HttpException(HttpStatusCode.MethodNotAllowed, message);

    public static HttpException NotFound(string message) => 
        new HttpException(HttpStatusCode.NotFound, message);

    public static HttpException BadRequest(string message) => 
        new HttpException(HttpStatusCode.BadRequest, message);

    public static HttpException Unauthorized(string message) => 
        new HttpException(HttpStatusCode.Unauthorized, message);

    public static HttpException InternalServerError(
        string message, 
        Exception? inner=null) => new HttpException(
            HttpStatusCode.InternalServerError, 
            message, 
            inner);

}
