namespace Server.Http.Http;

using System;

public enum MimeType
{
    // Text
    TextPlain,
    TextHtml,
    TextXml,
    TextCss,
    TextCsv,

    // Images
    ImageJpeg,
    ImagePng,
    ImageGif,
    ImageSvg,
    ImageBmp,

    // Audio
    AudioMpeg,
    AudioWav,
    AudioOgg,

    // Video
    VideoMp4,
    VideoWebm,
    VideoAvi,

    // Application
    ApplicationPdf,
    ApplicationJson,
    ApplicationXml,
    ApplicationZip,
    ApplicationOctetStream
}

public static class MimeTypeExtensions
{
    public static string ToMimeTypeString(this MimeType mimeType)
    {
        switch (mimeType)
        {
            case MimeType.TextPlain:
                return "text/plain";
            case MimeType.TextHtml:
                return "text/html";
            case MimeType.TextXml:
                return "text/xml";
            // ... Add other cases for remaining MIME types ...
            default:
                throw new ArgumentOutOfRangeException(nameof(mimeType));
        }
    }
}