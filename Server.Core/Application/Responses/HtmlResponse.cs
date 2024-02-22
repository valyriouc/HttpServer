using System.Text;

namespace Server.Core.Application.Responses;


public class HtmlResponse : ResponseBase, ICreateableResponse<HtmlResponse>
{
    public HtmlResponse(string content) 
        : base(HttpContentType.Html, Encoding.UTF8.GetBytes(content)) 
    {
    }

    public HtmlResponse(byte[] content)
        : base(HttpContentType.Html, content)
    {
    }

    public static HtmlResponse Create(string content) => 
        new HtmlResponse(content);

    public static HtmlResponse Create(byte[] bytes) => 
        new HtmlResponse(bytes);
}
