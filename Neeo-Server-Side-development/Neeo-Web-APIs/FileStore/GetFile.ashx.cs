using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Common;
using Common.Extension;
using LibNeeo;
using LibNeeo.IO;
using LibNeeo.MediaSharing;
using LibNeeo.MUC;

namespace FileStore
{
    /// <summary>
    /// Summary description for GetFile
    /// </summary>
    public class GetFile : IHttpHandler
    {
        /// <summary>
        /// holds the http context object for setting up response header on sending response.
        /// </summary>
        private HttpContext _httpContext;

        /// <summary>
        /// Processes the http request to send the required response.
        /// </summary>
        /// <remarks>It is used to send image data to the requester if the request  </remarks>
        /// <param name="context">An object holding http context object for setting up response header on sending response.</param>
        public void ProcessRequest(HttpContext context)
        {
            _httpContext = context;
            LibNeeo.IO.File file = null;
            ulong timeStamp = 0;
            short fileClass = 0;

            if (!NeeoUtility.IsNullOrEmpty(context.Request.QueryString["id"])
                    && !NeeoUtility.IsNullOrEmpty(context.Request.QueryString["sig"])
                    && !NeeoUtility.IsNullOrEmpty(context.Request.QueryString["fc"])
                    && !NeeoUtility.IsNullOrEmpty(context.Request.QueryString["mt"]))
            {
                string fileID = HttpUtility.UrlEncode(context.Request.QueryString["id"].ToString());
                FileCategory fileCategory = (FileCategory)Convert.ToInt16(context.Request.QueryString["fc"]);
                MediaType mediaType = (MediaType)Convert.ToInt16(context.Request.QueryString["mt"]);

                if (NeeoUtility.GenerateSignature(fileID + fileCategory.ToString("D") + mediaType.ToString("D")) == context.Request.QueryString["sig"].ToString())
                {
                    file = GetRequestedFile(fileID, fileCategory, mediaType);
                    if (file != null)
                    {
                        SetResponseWithFileData(file);
                    }
                    else
                    {
                        Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "File ID: "+ fileID +", File Category: Shared, Status: File does not exists.");
                    }
                }
                
            }
            else if (!NeeoUtility.IsNullOrEmpty(context.Request.QueryString["id"])
                    && !NeeoUtility.IsNullOrEmpty(context.Request.QueryString["sig"]))
            {
                // This part is to give backward compatibility for shared file service given with release 3
                string fileID = HttpUtility.UrlEncode(context.Request.QueryString["id"].ToString());

                if (NeeoUtility.GenerateSignature(fileID) == context.Request.QueryString["sig"].ToString())
                {
                    file = GetRequestedFile(fileID, FileCategory.Shared, MediaType.Image);
                    if (file != null)
                    {
                        SetResponseWithFileData(file);
                    }
                    else
                    {
                        Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "File ID: " + fileID + ", File Category: Group, Status: File does not exists.");
                    }
                }
            }
            SetResponseHeaders((int)HttpStatusCode.BadRequest);
        }

        private LibNeeo.IO.File GetRequestedFile(string fileID, FileCategory fileCategory, MediaType mediaType)
        {
            string filePath = null;
            switch (fileCategory)
            {
                case FileCategory.Shared:
                    return SharedMedia.GetMedia(fileID, mediaType);
                    break;
                case FileCategory.Group:
                    return NeeoGroup.GetGroupIcon(fileID);
                    break;
                default:
                    return null;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #region Response Headers
        protected void SetResponseHeaders(int code)
        {
            _httpContext.Response.StatusCode = code;
            if (NeeoDictionaries.HttpStatusCodeDescriptionMapper.ContainsKey((int)code))
            {
                _httpContext.Response.StatusDescription =
                    Common.NeeoDictionaries.HttpStatusCodeDescriptionMapper[code];
                _httpContext.Response.Write(Common.NeeoDictionaries.HttpStatusCodeDescriptionMapper[code]);
            }
            else
            {
                _httpContext.Response.Write("Request has some error.");
            }
            _httpContext.Response.Flush();
            _httpContext.Response.End();
        }

        protected void SetResponseWithFileData(LibNeeo.IO.File file)
        {
            string contentType = file.Info.MimeType.GetDescription();

            if(file.Info.MimeType == MimeType.DocWordx)
            {
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }

            if (file.Info.MimeType == MimeType.DocPptx)
            {
                contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            }

            if (file.Info.MimeType == MimeType.DocXlsx)
            {
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }

            _httpContext.Response.ClearContent();
            _httpContext.Response.ClearHeaders();
            _httpContext.Response.Buffer = true;
            _httpContext.Response.ContentType = contentType;
            _httpContext.Response.AddHeader("Content-Length", file.Info.Length.ToString());
            _httpContext.Response.TransmitFile(file.Info.FullPath);
            //_httpContext.Response.AddHeader("Ext", MimeTypeMapping.GetMimeTypeDetail(file.Info.MimeType.GetDescription()).Extension);
            //_httpContext.Response.BinaryWrite(System.IO.File.ReadAllBytes(file.Info.FullPath));
            _httpContext.Response.Flush();
            _httpContext.Response.End();

        }
        #endregion
    }
}