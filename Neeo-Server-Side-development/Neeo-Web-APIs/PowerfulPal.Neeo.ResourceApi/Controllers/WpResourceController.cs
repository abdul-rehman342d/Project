using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Common;
using Common.Controllers;
using Common.Extension;
using LibNeeo.IO;
using LibNeeo.Resource;
using PowerfulPal.Neeo.ResourceApi.Models;

namespace PowerfulPal.Neeo.ResourceApi.Controllers
{
    [RoutePrefix("wp/resources")]
    public class WpResourceController : NeeoApiController
    {
        [Route("background/{ResourceId}/{Filename}/view={viewType:values(port|land)}/res={resolution:values(FHD|WVGA|WXGA)}")]
        public HttpResponseMessage Get([FromUri]Background background)
        {
            if (!ModelState.IsValid || background == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Bad Request");
            }
            background.DevicePlatform = DevicePlatform.WindowsMobile;
            background.ResourceType = ResourceType.Background;
            try
            {
                var file = ResourceManager.GetResourceContent(background.Filename, background.ResourceId,
                    background.ResourceType, background.DevicePlatform, background.ViewType, background.Resolution);
                 var response = Request.CreateResponse(HttpStatusCode.OK);
                 response.Content = new ByteArrayContent(file);
            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue(
                   MimeMapping.GetMimeMapping(background.Filename));
            return response;
            }
            catch (ApplicationException appException)
            {
                return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt32(appException.Message), "");
            }
            catch (Exception exception)
            {
                Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Route("background/{ResourceId}/{Filename}/thumbnail")]
        public HttpResponseMessage GetThumbnail([FromUri]BackgroundThumbnail backgroundThumbnail)
        {
            if (!ModelState.IsValid || backgroundThumbnail == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Bad Request");
            }
            backgroundThumbnail.DevicePlatform = DevicePlatform.WindowsMobile;
            backgroundThumbnail.ResourceType = ResourceType.Background;
            backgroundThumbnail.ViewType = "port";
            backgroundThumbnail.Resolution = "FHD";
            try
            {
                var file = ResourceManager.GetResourceThumbnail(backgroundThumbnail.Filename, backgroundThumbnail.ResourceId,
                    backgroundThumbnail.ResourceType, backgroundThumbnail.DevicePlatform, backgroundThumbnail.ViewType, backgroundThumbnail.Resolution);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(file);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(
                       MimeMapping.GetMimeMapping(backgroundThumbnail.Filename));
                return response;
            }
            catch (ApplicationException appException)
            {
                return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt32(appException.Message), "");
            }
            catch (Exception exception)
            {
                Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
