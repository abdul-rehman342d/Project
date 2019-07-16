using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Extension;
using Twilio;

namespace LibNeeo.IO
{
    public static class FileFactory
    {
        public static File Create(string mimeType)
        {
            File file = null;
            var mimeTypeDetail = MimeTypeMapping.GetMimeTypeDetail(mimeType);
            switch (mimeTypeDetail.MediaType)
            {
                case MediaType.Image:
                    file = new Image();
                    file.Info.Extension = mimeTypeDetail.Extension;
                    file.Info.MimeType = mimeTypeDetail.MimeType;
                    break;
                case MediaType.Audio:
                    file = new Audio();
                     file.Info.Extension = mimeTypeDetail.Extension;
                    file.Info.MimeType = mimeTypeDetail.MimeType;
                    break;
                case MediaType.Video:
                    file = new Video();
                     file.Info.Extension = mimeTypeDetail.Extension;
                    file.Info.MimeType = mimeTypeDetail.MimeType;
                    break;
                case MediaType.Document:
                    file = new Document();
                    file.Info.Extension = mimeTypeDetail.Extension;
                    file.Info.MimeType = mimeTypeDetail.MimeType;
                    break;
            }
            return file;
        }    
    }
}
