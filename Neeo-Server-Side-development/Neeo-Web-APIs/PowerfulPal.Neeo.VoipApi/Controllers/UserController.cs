using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Controllers;
using Common.Entities;
using Common.Extension;
using LibNeeo.Voip;
using Logger;
using PowerfulPal.Neeo.VoipApi.Models;

namespace PowerfulPal.Neeo.VoipApi.Controllers
{
    [RoutePrefix("rtsip/v1/user")]
    public class UserController : NeeoApiController
    {
        [Route("{uid}/blocked-list")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetBlockedUser([FromUri]BaseRequest request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                var result = await Task.Factory.StartNew(() => NeeoVoipApi.GetBlockedUser(request.Uid));
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (ApplicationException applicationException)
            {
                var statusCode = (HttpStatusCode)(Convert.ToInt32(applicationException.Message));
                return SetCustomResponseMessage(null, statusCode);
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Route("block")]
        [HttpPut]
        public async Task<HttpResponseMessage> Block(UserDTO request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                await Task.Factory.StartNew(() => NeeoVoipApi.BlockUser(request.Uid, request.UserList));
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (ApplicationException applicationException)
            {
                return SetCustomResponseMessage(null, (HttpStatusCode)(Convert.ToInt32(applicationException.Message)));
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [Route("unblock")]
        [HttpPut]
        public async Task<HttpResponseMessage> Unblock(UserDTO request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            try
            {
                await Task.Factory.StartNew(() => NeeoVoipApi.UnBlockUser(request.Uid, request.UserList));
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (ApplicationException applicationException)
            {
                return SetCustomResponseMessage(null, (HttpStatusCode)(Convert.ToInt32(applicationException.Message)));
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exception.Message, exception);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
