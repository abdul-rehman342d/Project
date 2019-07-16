using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Common;
using Logger;
using Newtonsoft.Json;
using SettingsService.Models;
using LibNeeo;

namespace SettingsService.Controllers
{
    [RoutePrefix("api/settings")]
    public class SettingsController : ApiController
    {
        private bool _logRequestResponse =
        Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.LogRequestResponse]);

        [HttpPost]
        [Route("tone")]
        public HttpResponseMessage Tone([FromBody]ToneSetting toneSetting)
        {
            toneSetting.Uid = (toneSetting.Uid != null) ? toneSetting.Uid.Trim() : toneSetting.Uid;
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> " + JsonConvert.SerializeObject(toneSetting));
            }

            #endregion

            //#region user authentication 

            //IEnumerable<string> headerValues;
            //string keyFromClient = "";
            //if (Request.Headers.TryGetValues("key", out headerValues))
            //{
            //    keyFromClient = headerValues.First();
            //}

            //if (NeeoUtility.AuthenticateUserRequest(toneSetting.UID, keyFromClient))
            //{
            //    #endregion

            if (ModelState.IsValid)
            {
                try
                {
                    if (Enum.IsDefined(typeof (IMTone), toneSetting.IMTone))
                    {
                        NeeoUser user = new NeeoUser(toneSetting.Uid.Trim());
                        if (user.UpdateSettings((IMTone)toneSetting.IMTone, ToneType.IMTone))
                        {
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                catch (ApplicationException appException)
                {
                    return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt32(appException.Message), "");
                }
                catch (Exception exception)
                {
                    Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().GetType(), exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().GetType().Name);
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "");
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            //}
            //else
            //{
            //    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            //}
        }

        [HttpPost]
        [Route("calling-tone")]
        public HttpResponseMessage CallingTone([FromBody]Models.CallingTone callingTone)
        {
            callingTone.Uid = callingTone.Uid.Trim();
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> " + JsonConvert.SerializeObject(callingTone));
            }

            #endregion

            if (ModelState.IsValid)
            {
                try
                {
                    if (Enum.IsDefined(typeof(Common.CallingTone), callingTone.Tone))
                    {
                        NeeoUser user = new NeeoUser(callingTone.Uid.Trim());
                        if (user.UpdateSettings((Common.CallingTone)callingTone.Tone, ToneType.CallingTone))
                        {
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
                catch (ApplicationException appException)
                {
                    return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt32(appException.Message), "");
                }
                catch (Exception exception)
                {
                    Logger.LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().GetType(), exception.Message, exception, System.Reflection.MethodBase.GetCurrentMethod().GetType().Name);
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "");
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
