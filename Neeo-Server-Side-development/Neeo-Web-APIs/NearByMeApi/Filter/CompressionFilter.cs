using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Compression
{
    public class CompressionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext httpActionContext)
        {
            var responseContent = httpActionContext.Response.Content;
            var bytes = responseContent == null ? null : responseContent.ReadAsByteArrayAsync().Result;
            var zlibbedContent = bytes == null ? new byte[0] :
            CompressionHelper.GzipByte(bytes);
            httpActionContext.Response.Content = new ByteArrayContent(zlibbedContent);
            //Set response header
            httpActionContext.Response.Content.Headers.Remove("Content-Type");
            httpActionContext.Response.Content.Headers.Add("Content-encoding", "gzip");
            httpActionContext.Response.Content.Headers.Add("Content-Type", "application/json");
            base.OnActionExecuted(httpActionContext);
        }
    }

    public class CompressionHelper
    {
        public static byte[] GzipByte(byte[] str)
        {
            if (str == null)
            {
                return null;
            }

            using (var output = new MemoryStream())
            {
                using (var compressor = new Ionic.Zlib.GZipStream(output, Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestSpeed))
                {
                    compressor.Write(str, 0, str.Length);
                }
                return output.ToArray();
            }
        }
    }
}