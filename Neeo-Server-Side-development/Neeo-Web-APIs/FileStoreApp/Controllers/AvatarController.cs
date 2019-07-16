using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding.Binders;
using System.Web.UI.WebControls;
using Common;
using Common.Controllers;
using Common.Extension;
using FileStoreApp.Models;
using LibNeeo;
using LibNeeo.IO;

namespace FileStoreApp.Controllers
{
    //[System.Web.Http.RoutePrefix("~/v1/avatar")]
    public class AvatarController : NeeoApiController
    {
        [Route("avatar/{uid}/{timestamp}/{dimension}")]
        public async Task<HttpResponseMessage> Get([FromUri]AvatarRequest avatarRequest)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Bad Request");
            }
            try
            {
                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    NeeoFileInfo fileInfo = null;
                    ulong avatarUpdatedTimeStamp = 0;
                    int dimension = Convert.ToInt32(avatarRequest.Dimension);
                    NeeoUser user = new NeeoUser(avatarRequest.Uid);
                    switch (user.GetAvatarState(avatarRequest.Timestamp, out avatarUpdatedTimeStamp, out fileInfo))
                    {
                        case AvatarState.Modified:
                            var response = Request.CreateResponse(HttpStatusCode.OK);
                            response.Headers.Add("ts", avatarUpdatedTimeStamp.ToString());
                            response.Content =
                                new ByteArrayContent(MediaUtility.ResizeImage(fileInfo.FullPath, dimension, dimension));
                            //response.Content = new StreamContent(new FileStream(fileInfo.FullPath,FileMode.Open));
                            response.Content.Headers.ContentType =
                                new MediaTypeHeaderValue(
                                    MimeTypeMapping.GetMimeType(fileInfo.Extension).GetDescription());
                            return response;
                        case AvatarState.NotModified:
                            return Request.CreateResponse(HttpStatusCode.NotModified);
                        default:
                            return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                });
            }
            catch (AggregateException aggregateException)
            {
                Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, aggregateException.Message, aggregateException, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
            }
            catch (Exception exception)
            {
                Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }

        [AcceptVerbs("PUT", "POST", "DELETE")]
        public HttpResponseMessage All()
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Bad Request");
        }
    }
}
