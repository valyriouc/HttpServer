namespace Server.Core.Http;

public class HttpHeaderDictionary : Dictionary<string, string>
{
    public HttpHeaderDictionary()
    {
    }

    public HttpHeaderDictionary(Dictionary<string, string> headers)
    {
        foreach (KeyValuePair<string, string> pair in headers)
        {
            this.Add(pair.Key, pair.Value);
        }
    }
}
