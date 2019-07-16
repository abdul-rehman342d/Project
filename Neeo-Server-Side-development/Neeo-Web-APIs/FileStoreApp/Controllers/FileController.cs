using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Web;
using System.Web.Http;
using Common;
using Common.Controllers;
using Common.Extension;
using FileStoreApp.Models;
using LibNeeo.IO;
using LibNeeo.MediaSharing;
using LibNeeo.MUC;

namespace FileStoreApp.Controllers
{
    public class FileController : NeeoApiController
    {
        [Route("file/{filecategory}/{mediatype}/{signature}/view={fullname}")]
        public HttpResponseMessage Get([FromUri]FileRequest fileRequest)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Bad Request");
            }
            if (NeeoUtility.GenerateSignature(fileRequest.Name + fileRequest.FileCategory.ToString("D") + fileRequest.MediaType.ToString("D")) == fileRequest.Signature)
            {
                byte[] fileBytes;
                var file = GetRequestedFile(fileRequest);
                if (file != null)
                {
                    if (Request.Headers.Range != null)
                    {
                         var delimeter = new char[] {'-', '='};
                         var rangeValues = Request.Headers.Range.ToString().Split(delimeter);
                        if (rangeValues.Length != 3)
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                        if (!((Convert.ToInt64(rangeValues[1]) <= Convert.ToInt64(rangeValues[2])) && (Convert.ToInt64(rangeValues[2]) <= file.Info.Length)))
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                        fileBytes = File.GetBytesArray(file.Info.FullPath, Convert.ToInt32(rangeValues[1]),
                            Convert.ToInt32(rangeValues[2]));
                    }
                    else
                    {
                        fileBytes = File.GetBytesArray(file.Info.FullPath);
                    }
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileBytes);
                        //new StreamContent(File.GetStream(file.Info.FullPath));
                    //response.Content = new StreamContent(new FileStream(fileInfo.FullPath,FileMode.Open));
                    response.Content.Headers.ContentType =
                        new MediaTypeHeaderValue(
                            MimeTypeMapping.GetMimeType(file.Info.Extension).GetDescription());
                    return response;
                }
                Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "File ID: " + fileRequest.FullName + ", File Category: Shared, Status: File does not exists.");
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Bad Request");
        }

        private LibNeeo.IO.File GetRequestedFile(FileRequest fileRequest)
        {
            switch (fileRequest.FileCategory)
            {
                case FileCategory.Shared:
                    return SharedMedia.GetMedia(fileRequest.Name, fileRequest.MediaType);
                case FileCategory.Group:
                    return NeeoGroup.GetGroupIcon(fileRequest.Name);
                default:
                    return null;
            }
        }

    }
}
