using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Common;
using LibNeeo.Resource;

namespace SettingsService.Controllers
{
    [RoutePrefix("api/resource/background")]
    public class BackgroundController : ApiController
    {
        [Route("{backgroundId}")]
        [HttpPost]
        public HttpResponseMessage GetBackgrounds(string backgroundId)
        {
            try
            {
                var background = ResourceManager.GetResourceInfo(backgroundId, ResourceType.Background);
                return Request.CreateResponse(HttpStatusCode.OK, background.GetComponentsUrl());
            }
            catch (ApplicationException applicationException)
            {
                return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt16(applicationException.Message), NeeoDictionaries.HttpStatusCodeDescriptionMapper[Convert.ToInt16(applicationException.Message)]);
            }
            catch (Exception exception)
            {
                Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().GetType(), exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
           
        }
    }
}
