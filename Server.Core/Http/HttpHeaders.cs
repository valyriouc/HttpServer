namespace Server.Core.Http;

public class HttpHeaders : Dictionary<string, string>
{
    public string? Accept => this.ContainsKey("Accept") ? this["Accept"] : null;
    public string? AcceptCharset => this.ContainsKey("Accept-Charset") ? this["Accept-Charset"] : null;
    public string? AcceptEncoding => this.ContainsKey("Accept-Encoding") ? this["Accept-Encoding"] : null;
    public string? AcceptLanguage => this.ContainsKey("Accept-Language") ? this["Accept-Language"] : null;
    public string? Authorization => this.ContainsKey("Authorization") ? this["Authorization"] : null;
    public string? CacheControl => this.ContainsKey("Cache-Control") ? this["Cache-Control"] : null;
    public string? Connection => this.ContainsKey("Connection") ? this["Connection"] : null;
    public string? ContentLength => this.ContainsKey("Content-Length") ? this["Content-Length"] : null;
    public string? ContentType => this.ContainsKey("Content-Type") ? this["Content-Type"] : null;
    public string? Date => this.ContainsKey("Date") ? this["Date"] : null;
    public string? Expect => this.ContainsKey("Expect") ? this["Expect"] : null;
    public string? From => this.ContainsKey("From") ? this["From"] : null;
    public string? Host => this.ContainsKey("Host") ? this["Host"] : null;
    public string? IfMatch => this.ContainsKey("If-Match") ? this["If-Match"] : null;
    public string? IfModifiedSince => this.ContainsKey("If-Modified-Since") ? this["If-Modified-Since"] : null;
    public string? IfNoneMatch => this.ContainsKey("If-None-Match") ? this["If-None-Match"] : null;
    public string? IfRange => this.ContainsKey("If-Range") ? this["If-Range"] : null;
    public string? IfUnmodifiedSince => this.ContainsKey("If-Unmodified-Since") ? this["If-Unmodified-Since"] : null;
    public string? MaxForwards => this.ContainsKey("Max-Forwards") ? this["Max-Forwards"] : null;
    public string? Pragma => this.ContainsKey("Pragma") ? this["Pragma"] : null;
    public string? ProxyAuthorization => this.ContainsKey("Proxy-Authorization") ? this["Proxy-Authorization"] : null;
    public string? Range => this.ContainsKey("Range") ? this["Range"] : null;
    public string? Referer => this.ContainsKey("Referer") ? this["Referer"] : null;
    public string? TE => this.ContainsKey("TE") ? this["TE"] : null;
    public string? Upgrade => this.ContainsKey("Upgrade") ? this["Upgrade"] : null;
    public string? UserAgent => this.ContainsKey("User-Agent") ? this["User-Agent"] : null;
    public string? Via => this.ContainsKey("Via") ? this["Via"] : null;
    public string? Warning => this.ContainsKey("Warning") ? this["Warning"] : null;

    public HttpHeaders()
    {
    }

    public HttpHeaders(Dictionary<string, string> headers)
    {
        foreach (KeyValuePair<string, string> pair in headers)
        {
            this.Add(pair.Key, pair.Value);
        }
    }
}
