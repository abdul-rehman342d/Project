using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Common.Controllers;
using Common.Entities;
using FileStoreApi.Models;
using System.Configuration;
using LibNeeo;
using LibNeeo.IO;
using Common;
using LibNeeo.Url;
using Logger;
using System.IO;
using System.Web;
using System.Threading.Tasks;

namespace FileStoreApi.Controllers
{
    [RoutePrefix("v1/profile")]
    public class ProfileController : NeeoApiController
    {
        private bool _logRequestResponse =
       Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.LogRequestResponse]);

        /// <summary>
        /// Updates user information into the database and the profile picture.
        /// </summary>
        /// <param name="userID">A string containing phone number as user id.</param>
        /// <param name="name">A string containing the name of the user.</param>
        /// <param name="fileData">A base64 encoded string containing the file data.</param>
        /// <returns>
        /// true if information is successfully updated; otherwise, false.
        /// </returns>
        ///        //UpdateUserInformation
        [Route("")]
        public HttpResponseMessage PutUserInformation([FromBody]UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            request.Uid = (request.Uid != null) ? request.Uid.Trim() : request.Uid;
            request.name = (request.name != null) ? request.name.Trim() : request.name;
            request.fileData = (request.fileData != null) ? request.fileData.Trim() : request.fileData;
            bool isUpdated = false;
            ulong temp = 0;
            LogRequest(request);

           
            NeeoUser user = new NeeoUser(request.Uid);
            try
            {
                isUpdated = user.UpdateUserProfile(request.name, new LibNeeo.IO.File() { Data = request.fileData });
                return Request.CreateResponse(HttpStatusCode.OK, isUpdated);
                #region old impl
                //using (TransactionScope scope = new TransactionScope())
                //{
                //    if (!user.UpdateUsersDisplayName(name))
                //    {
                //        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidUser);
                //    }
                //    else if (NeeoUtility.IsNullOrEmpty(fileData))
                //    {
                //        isCompleted = true;
                //        scope.Complete();
                //    }
                //    else
                //    {
                //        LibNeeo.IO.File file = new LibNeeo.IO.File()
                //        {
                //            Data = fileData,
                //            FileOwner = userID,
                //            MediaType = MediaType.Image,
                //            MimeType = MimeType.Image_jpeg
                //        };
                //        if (user.SaveFile(file, FileCategory.Profile))
                //        {
                //            isCompleted = true;
                //            scope.Complete();
                //        }
                //    }
                //}
                #endregion
            }
            catch (ApplicationException appExp)
            {
                return SetCustomResponseMessage(isUpdated, (HttpStatusCode)(Convert.ToInt32(appExp.Message)));
            }
            catch (Exception exp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                return SetCustomResponseMessage(isUpdated, (HttpStatusCode)CustomHttpStatusCode.ServerInternalError);
            }
        }

        /// <summary>
        /// Updates user information into the database and the profile picture.
        /// </summary>
        /// <param name="userID">A string containing phone number as user id.</param>
        /// <param name="name">A string containing the name of the user.</param>
        /// <param name="fileData">A base64 encoded string containing the file data.</param>
        /// <returns>
        /// true if information is successfully updated; otherwise, false.
        /// </returns>
        ///        //UpdateUserInformation


        [Route("")]
        public async Task<HttpResponseMessage> PostUserInformation()
        {
            if (!Request.Headers.Contains("uid"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (!Request.Headers.Contains("name"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            string uId = Request.Headers.GetValues("uid").First();
            string name = Request.Headers.GetValues("name").First(); ;
            bool isUpdated = false;

            if (Request.Content.IsMimeMultipartContent())
            {
                MultipartMemoryStreamProvider provider = await Request.Content.ReadAsMultipartAsync(new MultipartMemoryStreamProvider());
                foreach (HttpContent content in provider.Contents)
                {
                    if (content.Headers.ContentType == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                    if (content.Headers.ContentDisposition.FileName == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                    string contentType = content.Headers.ContentType.ToString();
                    string sourceExtension = content.Headers.ContentDisposition.FileName.Split(new char[] { '.' }).Last().Split(new char[] { '"' }).First();

                    if (!MimeTypeMapping.ValidateMimeType(contentType))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                    }

                    using (var stream = content.ReadAsStreamAsync().Result)
                    {

                        NeeoUser user = new NeeoUser(uId);
                        try
                        {
                            isUpdated = user.UpdateUserProfile(name, new LibNeeo.IO.File() { FileStream = new FileDataStream(){ Stream = stream }});
                            return Request.CreateResponse(HttpStatusCode.OK, isUpdated);
                            
                        }
                        catch (ApplicationException appExp)
                        {
                            return SetCustomResponseMessage(isUpdated, (HttpStatusCode)(Convert.ToInt32(appExp.Message)));
                        }
                        catch (Exception exp)
                        {
                            LogManager.CurrentInstance.ErrorLogger.LogError(
                                MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                            return SetCustomResponseMessage(isUpdated, (HttpStatusCode)CustomHttpStatusCode.ServerInternalError);
                        }
                    }
                }
            }
            else
            {
                NeeoUser user = new NeeoUser(uId);
                try
                {
                    isUpdated = user.UpdateUserProfile(name, null);
                    return Request.CreateResponse(HttpStatusCode.OK, isUpdated);

                }
                catch (ApplicationException appExp)
                {
                    return SetCustomResponseMessage(isUpdated, (HttpStatusCode)(Convert.ToInt32(appExp.Message)));
                }
                catch (Exception exp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                    return SetCustomResponseMessage(isUpdated, (HttpStatusCode)CustomHttpStatusCode.ServerInternalError);
                }

            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Gets the user's name base on the User ID.
        /// </summary>
        /// <param name="uID">A string containing the user id.</param>
        /// <returns>It returns the profile name.</returns>
        /// GetProfileName
        [Route("name")]
        public HttpResponseMessage GetProfileName([FromUri]BaseRequest request)
        {
            string result = null;

            if (request == null || !ModelState.IsValid )
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            } 
            LogRequest(request);
            ulong temp = 0;
            try
            {
                request.Uid = request.Uid.Trim();
                NeeoUser user = new NeeoUser(request.Uid);
                result = user.GetUserProfileName();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (ApplicationException appExp)
            {
                return SetCustomResponseMessage("", (HttpStatusCode)(CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
            }
            catch (Exception exp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Gets the user's avatar base on the previous time stamp of the avatar.
        /// </summary>
        /// <param name="uID">A string containing the user id.</param>
        /// <param name="timeStamp">An integer containing the time stamp that has to be matched with the existing image. It is optional.</param>
        /// <param name="requiredDimension">An integer specifying the dimension of the image required. It is optional.</param>
        [Route("avatar")]
        public HttpResponseMessage GetUserAvatar([FromUri]GetUserAvatarRequest request)
        {
           
            if (request == null || !ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            LogRequest(request);
            ulong temp = 0;

            
                //uID = HttpUtility.UrlEncode(uID);
            request.Uid = request.Uid.Trim();
            NeeoUser user = new NeeoUser(request.Uid);
                NeeoFileInfo filePath = null;
                ulong avatarTimeStamp;
                switch (user.GetAvatarState(request.Ts, out avatarTimeStamp, out filePath))
                {
                    case AvatarState.NotExist:
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                        
                    case AvatarState.Modified:
                        string url = NeeoUrlBuilder.BuildAvatarUrl(request.Uid, request.Ts, request.Dim);
                        return RedirectServiceToUrl(url, avatarTimeStamp);
                        
                    case AvatarState.NotModified:
                        return Request.CreateResponse(HttpStatusCode.NotModified);

                    default:
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

            
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);

            //}

        }

        /// <summary>
        /// Gets the user's name and avatar base on the User ID.
        /// </summary>
        /// <param name="uID">A string containing the user id.</param>
        /// <returns>It returns the avatar and name in header.</returns>
        /// SyncProfile
        public HttpResponseMessage PostSyncProfile(string uID)
        {
            uID = (uID != null) ? uID.Trim() : uID;
            Stream resultStream = null;
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    MethodBase.GetCurrentMethod().DeclaringType, MethodBase.GetCurrentMethod().Name + "===>" +
                    "Request ===> userID : " + uID);
            }

            #endregion

            //    #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            //    #endregion


            ulong temp = 0;


            if (NeeoUtility.IsNullOrEmpty(uID) || !ulong.TryParse(uID, out temp))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            else
            {
                uID = uID.Trim();
                NeeoUser user = new NeeoUser(uID);

                try
                {
                    Request.Headers.Add("name", user.GetUserProfileName());
                    resultStream = user.GetAvatarStream();
                    return Request.CreateResponse(HttpStatusCode.OK, resultStream);
                }
                catch (ApplicationException appExp)
                {
                    return SetCustomResponseMessage("", (HttpStatusCode)(CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                }
                catch (Exception exp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                    return Request.CreateResponse(HttpStatusCode.Ambiguous);
                }
            }

            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode) HttpStatusCode.Unauthorized);
            //    return resultStream;
            //}
        }

        /// <summary>
        /// Redirects a service call to url specified in <paramref name="url"/>
        /// </summary>
        /// <param name="url">A string containing the url on which request has to be redirected.</param>
        [NonAction]
        private HttpResponseMessage RedirectServiceToUrl(string url, ulong avatarTimeStamp = 0)
        {
            HttpResponseMessage response = Request.CreateResponse();
            if (avatarTimeStamp != 0)
            {
                response.Headers.Add("ts", avatarTimeStamp.ToString());
            }
            response.StatusCode = HttpStatusCode.Redirect;
            Uri uri = new Uri(url);
            response.Headers.Location = uri;
            return response;
        }
    }
}
