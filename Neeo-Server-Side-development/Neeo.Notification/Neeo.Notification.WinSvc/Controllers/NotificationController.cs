using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common;
using LibNeeo;
using Logger;
using Neeo.Notification.Model;
using Newtonsoft.Json;

namespace Neeo.Notification.WinSvc.Controllers
{
    public class NotificationController : ApiController
    {
        private bool _logRequestResponse =
        Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.LogRequestResponse]);


        [HttpPost]
        public async Task<HttpResponseMessage> Send([FromBody]NotificationModel notification)
        {
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> " + JsonConvert.SerializeObject(notification));
            }

            //Console.WriteLine("Request ===> " + JsonConvert.SerializeObject(notification));

            #endregion

            if (ModelState.IsValid)
            {
                try
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    await Task.Factory.StartNew(() => NotificationHandler.GetInstance().ProcessNotification(notification));
                    //NotificationHandler.GetInstance().ProcessNotification(notification);
                    watch.Stop();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (ApplicationException appExp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        JsonConvert.SerializeObject(notification) + ", error:" + appExp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt32(appExp.Message), "");
                }
                catch (Exception exp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        JsonConvert.SerializeObject(notification) + ", error:" + exp.Message, exp, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "");
                }

            }
            else
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                                JsonConvert.SerializeObject(notification), System.Reflection.MethodBase.GetCurrentMethod().Name);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "");
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ResetCount([FromBody] ResetCountRequestModel request)
        {
            if (ModelState.IsValid)
            {
                ulong temp = 0;
                if (!NeeoUtility.IsNullOrEmpty(request.uID) && ulong.TryParse(request.uID, out temp))
                {
                    request.uID = request.uID.Trim();
                    try
                    {
                        if (await Task.Factory.StartNew(() => NeeoUser.ResetOfflineMessageCount(request.uID)))
                        {
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }

                        return
                            Request.CreateResponse(
                                (HttpStatusCode)Convert.ToInt32(CustomHttpStatusCode.UnknownError.ToString("D")));
                    }
                    catch (ApplicationException appExp)
                    {
                        LogManager.CurrentInstance.ErrorLogger.LogError(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                            "user id = " + request.uID + ", error:" + appExp.Message,
                            System.Reflection.MethodBase.GetCurrentMethod().Name);
                        return Request.CreateErrorResponse((HttpStatusCode)Convert.ToInt32(appExp.Message), "");
                    }
                    catch (Exception exp)
                    {
                        LogManager.CurrentInstance.ErrorLogger.LogError(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                            "user id = " + request.uID + ", error:" + exp.Message, exp,
                            System.Reflection.MethodBase.GetCurrentMethod().Name);
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "");
                    }
                }
                else
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "user id = " + request.uID
                        , System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "");
                }
            }
            else
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "user id = " + request.uID
                        , System.Reflection.MethodBase.GetCurrentMethod().Name);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "");
            }
        }

        [Route("")]
        [HttpGet]
        public void KeepAlive()
        {
            //if (_notificationManager == null)
            //{
            //    _notificationManager = new NotificationManager()s;
            //}

            //_notificationManager.KeepAliveChannel();
        }

        [HttpGet]
        public void Stop()
        {
            //if (_notificationManager == null)
            //{
            //    _notificationManager = new NotificationManager();
            //}

            //_notificationManager.StopAllServices();
        }

    }
}
