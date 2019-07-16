using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using Common.Controllers;
using FileStoreApi.Models;
using LibNeeo.Url;
using Logger;
using LibNeeo;
using Common;
using LibNeeo.IO;
using LibNeeo.MUC;
namespace FileStoreApi.Controllers
{
    [RoutePrefix("v1/group")]
    public class GroupController : NeeoApiController
    {

        private bool _logRequestResponse = Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.LogRequestResponse]);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uID"></param>
        /// <param name="data"></param>
        /// <param name="gID"></param>
        /// UploadGroupIcon
        [Route("icon/upload")]
        public HttpResponseMessage Post([FromBody] UploadGroupIconRequest request)
        {
           
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            LogRequest(request);
            ulong temp = 0;
            request.Uid = request.Uid.Trim();
            request.gID = request.gID.ToLower();
            try
            {
                var file = new LibNeeo.IO.File()
                {
                    Info = new NeeoFileInfo()
                    {
                        Creator = request.Uid,
                        MimeType = MimeType.ImageJpeg,
                        MediaType = MediaType.Image
                    },
                    Data = request.data,
                };
                if (NeeoGroup.SaveGroupIcon(request.Uid, request.gID.ToLower(), file))
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            catch (ApplicationException appExp)
            {
                return SetCustomResponseMessage("", (HttpStatusCode)(CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
            }
            catch (Exception exception)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name, exception);
                return SetCustomResponseMessage("", (HttpStatusCode)CustomHttpStatusCode.ServerInternalError);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="gID"></param>
        /// GetGroupIcon
        [Route("icon")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HttpResponseMessage Get([FromUri]GetGroupIconRequest request)
        {
            LogRequest(request);
            ulong temp = 0;
            if (NeeoUtility.IsNullOrEmpty(request.Uid) || !ulong.TryParse(request.Uid, out temp) ||
                NeeoUtility.IsNullOrEmpty(request.gID))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
            {
                request.Uid = request.Uid.Trim();
                request.gID = request.gID.ToLower();
                try
                {
                    if (NeeoGroup.GroupIconExists(request.gID.ToLower()))
                    {
                        string url =
                            NeeoUrlBuilder.BuildFileUrl(ConfigurationManager.AppSettings[NeeoConstants.FileServerUrl], request.gID,
                                FileCategory.Group, MediaType.Image);
                        return RedirectServiceToUrl(url);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                }
                catch (Exception exception)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name, exception);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="gID"></param>
        ///  DeleteGroupIcon
        [Route("icon")]
        public HttpResponseMessage Delete(string uId, string gID)
        {


            #region log user request and response

            /***********************************************
             To log user request
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> senderID : " + uId + ", groupID : " + gID);
            }

            #endregion

            ulong temp = 0;

            if (NeeoUtility.IsNullOrEmpty(uId) || !ulong.TryParse(uId, out temp) ||
                NeeoUtility.IsNullOrEmpty(gID))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
            {
                uId = uId.Trim();
                gID = gID.ToLower();
                try
                {
                    NeeoGroup.DeleteGroupIcon(groupID: gID, userID: uId);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception exception)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        System.Reflection.MethodBase.GetCurrentMethod().Name, exception);
                    
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
        }

        #region Service Private Methods

        /// <summary>
        /// Redirects a service call to url specified in <paramref name="url"/>
        /// </summary>
        /// <param name="url">A string containing the url on which request has to be redirected.</param>
        private HttpResponseMessage RedirectServiceToUrl(string url, ulong avatarTimeStamp = 0)
        {
            HttpResponseMessage response = Request.CreateResponse();
            if (avatarTimeStamp != 0)
            {
                response.Headers.Add("ts", avatarTimeStamp.ToString());
            }
            response.StatusCode = System.Net.HttpStatusCode.Redirect;
            System.Uri uri = new Uri(url);
            response.Headers.Location = uri;
            return response;
        }
        #endregion
    }
}
