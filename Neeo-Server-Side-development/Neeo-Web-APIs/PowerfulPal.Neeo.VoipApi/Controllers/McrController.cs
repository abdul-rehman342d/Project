using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Controllers;
using Common;
using Common.Entities;
using LibNeeo.Voip;
using Logger;
using PowerfulPal.Neeo.VoipApi.Models;

namespace PowerfulPal.Neeo.VoipApi.Controllers
{
    [RoutePrefix("v1/mcr")]
    public class McrController : NeeoApiController
    {
        [Route("count")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetCount(BaseRequest request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                var result = await Task.Factory.StartNew(() => NeeoVoipApi.GetMcrCount(request.Uid));
                return Request.CreateResponse(HttpStatusCode.OK, new Dictionary<string, string>() { { "GetMcrCountResult", result } });
            }
            catch (ApplicationException applicationException)
            {
                return SetCustomResponseMessage("", (HttpStatusCode)(Convert.ToInt32(applicationException.Message)));
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Route("details")]
        [HttpPost]
        public async Task<HttpResponseMessage> GetDetails(McrDetailsRequest request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                var result = await Task.Factory.StartNew(() => NeeoVoipApi.GetMcrDetails(request.Uid, request.Flush));
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (ApplicationException applicationException)
            {
                return SetCustomResponseMessage("", (HttpStatusCode)(Convert.ToInt32(applicationException.Message)));
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
