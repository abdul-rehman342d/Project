using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Common.Extension;

namespace LibNeeo.IO
{
    /// <summary>
    /// Maps mime type to media type and extension.
    /// </summary>
    public static class MimeTypeMapping
    {
        /// <summary>
        /// Mime type mapping dictionary
        /// </summary>
        private static Dictionary<string, MimeTypeDetail> MimeTypeMappingDictionary = new Dictionary<string, MimeTypeDetail>()
        {
            {"image/jpeg", new MimeTypeDetail(){MimeType = MimeType.ImageJpeg, MediaType = MediaType.Image, Extension = ".jpg"}},
            {"image/jpg", new MimeTypeDetail(){MimeType = MimeType.ImageJpg, MediaType = MediaType.Image, Extension = ".jpg"}},
            {"audio/mpeg", new MimeTypeDetail(){MimeType = MimeType.AudioMpeg, MediaType = MediaType.Audio, Extension = ".mp3"}},
            {"audio/m4a", new MimeTypeDetail(){MimeType = MimeType.AudioM4a, MediaType = MediaType.Audio, Extension = ".m4a"}},
            {"audio/wav", new MimeTypeDetail(){MimeType = MimeType.AudioWav, MediaType = MediaType.Audio, Extension = ".wav"}},
            {"audio/x-aac", new MimeTypeDetail(){MimeType = MimeType.AudioXAac, MediaType = MediaType.Audio, Extension = ".aac"}},
            {"video/mp4", new MimeTypeDetail(){MimeType = MimeType.VideoMp4, MediaType = MediaType.Video, Extension = ".mp4"}},
            {"video/3gpp", new MimeTypeDetail(){MimeType = MimeType.Video3gpp, MediaType = MediaType.Video, Extension = ".3pg"}},
            {"video/quicktime", new MimeTypeDetail(){MimeType = MimeType.VideoQuickTime, MediaType = MediaType.Video, Extension = ".mov"}},
            {"video/x-msvideo", new MimeTypeDetail(){MimeType = MimeType.VideoXMsVideo, MediaType = MediaType.Video, Extension = ".avi"}},
            {"video/x-ms-wmv", new MimeTypeDetail(){MimeType = MimeType.VideoXMsWmv, MediaType = MediaType.Video, Extension = ".wmv"}},
            {"application/pdf", new MimeTypeDetail(){MimeType = MimeType.DocPdf, MediaType = MediaType.Document, Extension = ".pdf"}},
            {"application/vnd.ms-powerpoint", new MimeTypeDetail(){MimeType = MimeType.DocPpt, MediaType = MediaType.Document, Extension = ".ppt"}},
            {"application/vnd.ms-powerpointx", new MimeTypeDetail(){MimeType = MimeType.DocPptx, MediaType = MediaType.Document, Extension = ".pptx"}},
            {"application/vnd.ms-excel", new MimeTypeDetail(){MimeType = MimeType.DocXls, MediaType = MediaType.Document, Extension = ".xls"}},
            {"application/vnd.ms-excelx", new MimeTypeDetail(){MimeType = MimeType.DocXlsx, MediaType = MediaType.Document, Extension = ".xlsx"}},
            {"application/msword", new MimeTypeDetail(){MimeType = MimeType.DocWord, MediaType = MediaType.Document, Extension = ".doc"}},
            {"application/mswordx", new MimeTypeDetail(){MimeType = MimeType.DocWordx, MediaType = MediaType.Document, Extension = ".docx"}},
            {"text/plain", new MimeTypeDetail(){MimeType = MimeType.DocTxt, MediaType = MediaType.Document, Extension = ".txt"}},
            {"application/rtf", new MimeTypeDetail(){MimeType = MimeType.DocRtf, MediaType = MediaType.Document, Extension = ".rtf"}},
            

        };

        /// <summary>
        /// Gets mime type details for given mime type
        /// </summary>
        /// <param name="mimeType">A string containing the mime type</param>
        /// <returns>Mime type details including media type and extension</returns>
        public static MimeTypeDetail GetMimeTypeDetail(string mimeType)
        {
            return MimeTypeMappingDictionary[mimeType];
        }
        /// <summary>
        /// Gets the mime type for a given file extension.
        /// </summary>
        /// <param name="extension">A string containing the extension of the file</param>
        /// <returns>An mime type enum corresponds to the given extension</returns>
        /// <exception cref="ApplicationException"></exception>
        public static MimeType GetMimeType(string extension)
        {
            var enumerator = MimeTypeMappingDictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.Extension == extension)
                {
                    return enumerator.Current.Value.MimeType;
                }
            }
            throw new ApplicationException("Invalid extension");
        }
        /// <summary>
        /// Validates whether the given mime type is an allowed one with respect to the application.  
        /// </summary>
        /// <param name="mimeType">A string containing the mime type</param>
        /// <returns>True if it is an allowed mime type for the application; otherwise false</returns>
        public static bool ValidateMimeType(string mimeType)
        {
            if (MimeTypeMappingDictionary.ContainsKey(mimeType))
            {
                return true;
            }
            return false;
        }
    }
}
