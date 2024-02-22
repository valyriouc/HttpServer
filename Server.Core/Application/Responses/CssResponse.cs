namespace Server.Core.Application.Responses;

public class CssResponse : ResponseBase, ICreateableResponse<CssResponse>
{
    public CssResponse(string content)
        : base(HttpContentType.Css, content) 
    {

    }

    public CssResponse(byte[] content) 
        : base(HttpContentType.Css, content)
    {

    }

    public static CssResponse Create(string content) => 
        new CssResponse(content);

    public static CssResponse Create(byte[] bytes) =>
        new CssResponse(bytes);
}
