using System.ComponentModel;

/// <summary>
/// Specifies the mime type of the file.
/// </summary>
public enum MimeType : ushort
{
    [Description("image/jpeg")]
    ImageJpeg = 1,
    [Description("image/jpg")]
    ImageJpg = 2,
    [Description("audio/mpeg")]
    AudioMpeg = 3,
    [Description("audio/m4a")]
    AudioM4a = 4,
    [Description("audio/wav")]
    AudioWav = 5,
    [Description("audio/x-aac")]
    AudioXAac = 6,
    [Description("video/mp4")]
    VideoMp4 = 7,
    [Description("video/3gpp")]
    Video3gpp = 8,
    [Description("video/quicktime")]
    VideoQuickTime = 9,
    [Description("video/x-msvideo")]
    VideoXMsVideo = 10,
    [Description("video/x-ms-wmv")]
    VideoXMsWmv = 11,
    [Description("application/pdf")]
    DocPdf = 12,
    [Description("text/plain")]
    DocTxt = 13,
    [Description("application/rtf")]
    DocRtf = 14,
    [Description("application/msword")]
    DocWord = 15,
    [Description("application/mswordx")]
    DocWordx = 16,
    [Description("application/vnd.ms-powerpoint")]
    DocPpt = 17,
    [Description("application/vnd.ms-powerpointx")]
    DocPptx = 18,
    [Description("application/vnd.ms-excel")]
    DocXls = 19,
    [Description("application/vnd.ms-excelx")]
    DocXlsx = 20
}

/// <summary>
/// Specifies the mime type of the file.
/// </summary>
public enum MediaType : ushort
{
    Image = 1,
    Audio = 2,
    Video = 3,
    Document = 4
}