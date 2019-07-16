using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Web.Util;
using LibNeeo;
using Common;
using LibNeeo.IO;
using Logger;

namespace FileStore
{
    /// <summary>
    /// Summary description for FileHandler
    /// </summary>
    public class GetAvatar : IHttpHandler
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
            NeeoFileInfo fileInfo = null;
            ulong avatarTimeStamp = 0;
            ulong avatarUpdatedTimeStamp = 0;
            uint requiredDimension = 0;

            if (!NeeoUtility.IsNullOrEmpty(context.Request.QueryString["uid"]))
            {
                string uID = HttpUtility.UrlEncode(context.Request.QueryString["uid"].ToString());
                NeeoUser user = new NeeoUser(uID);

                if (!NeeoUtility.IsNullOrEmpty(context.Request.QueryString["ts"]))
                {
                    if (!ulong.TryParse(context.Request.QueryString["ts"], out avatarTimeStamp))
                    {
                        SetResponseHeaders((int)HttpStatusCode.BadRequest);
                    }
                }
                switch (user.GetAvatarState(avatarTimeStamp, out avatarUpdatedTimeStamp, out fileInfo))
                {
                    case AvatarState.NotExist:
                        SetResponseHeaders((int)HttpStatusCode.BadRequest);
                        break;
                    case AvatarState.Modified:
                        if (!NeeoUtility.IsNullOrEmpty(context.Request.QueryString["dim"]))
                        {
                            UInt32.TryParse(context.Request.QueryString["dim"], out requiredDimension);
                        }
                        SetResponseWithFileData(fileInfo.FullPath, avatarUpdatedTimeStamp, requiredDimension);
                        break;
                    case AvatarState.NotModified:
                        SetResponseHeaders((int)HttpStatusCode.NotModified);
                        break;
                }
            }
            else
            {
                SetResponseHeaders((int)HttpStatusCode.BadRequest);

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
            _httpContext.Response.StatusDescription = Common.NeeoDictionaries.HttpStatusCodeDescriptionMapper[code];
            _httpContext.Response.Write(Common.NeeoDictionaries.HttpStatusCodeDescriptionMapper[code]);
            _httpContext.Response.Flush();
            _httpContext.Response.End();
        }

        protected void SetResponseWithFileData(string filePath, ulong avatarTimeStamp, uint requiredDimension)
        {

            byte[] fileBinary = MediaUtility.ResizeImage(filePath, Convert.ToInt32(requiredDimension), Convert.ToInt32(requiredDimension));
            _httpContext.Response.ClearContent();
            _httpContext.Response.ClearHeaders();
            _httpContext.Response.AppendHeader("ts", avatarTimeStamp.ToString());
            _httpContext.Response.Buffer = true;
            _httpContext.Response.ContentType = "image/jpeg";
            _httpContext.Response.BinaryWrite(fileBinary);
            _httpContext.Response.Flush();
            _httpContext.Response.End();

        }

        protected void SetResponseWithFileData(string filePath)
        {
            byte[] fileBinary = System.IO.File.ReadAllBytes(filePath);
            _httpContext.Response.ClearContent();
            _httpContext.Response.ClearHeaders();
            _httpContext.Response.Buffer = true;
            _httpContext.Response.ContentType = "image/jpeg";
            _httpContext.Response.BinaryWrite(fileBinary);
            _httpContext.Response.Flush();
            _httpContext.Response.End();

        }
        #endregion
    }
}