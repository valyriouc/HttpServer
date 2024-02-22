namespace Server.Core.Application.Responses;

public class JavaScriptResponse : ResponseBase, 
    ICreateableResponse<JavaScriptResponse>
{
    public JavaScriptResponse(string content) 
        : base(HttpContentType.Js, content) { }

    public JavaScriptResponse(byte[] content) 
        : base(HttpContentType.Js, content) { }

    public static JavaScriptResponse Create(string content) => 
        new JavaScriptResponse(content);

    public static JavaScriptResponse Create(byte[] bytes) => 
        new JavaScriptResponse(bytes);
}
