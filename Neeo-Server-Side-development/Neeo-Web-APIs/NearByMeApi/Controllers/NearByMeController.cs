using Common;
using Common.Controllers;
using LibNeeo.NearByMe;
using LibNeeo.NearByMe.Model;
using PowerfulPal.Neeo.NearByMeApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace PowerfulPal.Neeo.NearByMeApi.Controllers
{
    /// <summary>
    /// This near by me service for Neeo Messenger.
    /// </summary>
    [RoutePrefix("api/v1/near-by-me")]
    public class NearByMeController : NeeoApiController
    {
        /// <summary>
        /// This end point gets the NearByMe settings of the user.
        /// </summary>
        /// <param name="uid">The string containing the user phone number</param>
        /// <returns></returns>
        [Route("{uid:regex(^([0-9]+)(\\s)*$)}/setting")]
        [HttpGet]
        public HttpResponseMessage GetSetting(string uid)
        {
            try
            {
                LogRequest(uid);

                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                
                NearByMeSetting setting = NearByMeManager.GetInstance().GetNearByMeSetting(uid);
                return Request.CreateResponse(HttpStatusCode.OK, setting);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        public HttpResponseMessage PostNearByUser([FromBody]UserSearchByLocationModel model)
        {
            try
            {
                LogRequest(model);

                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                var nearByUser = new NearByUser() { UId = model.UId, Latitude = model.Latitude, Longitude = model.Longitude, IsCurrentLocation = model.IsCurrentLocation };
                List<NearByUser> nearByUsers = NearByMeManager.GetInstance().GetNearByMeUserByLocation(nearByUser);
                return Request.CreateResponse(HttpStatusCode.OK, new { NearByUsers = nearByUsers });
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{uid:regex(^([0-9]+)(\\s)*$)}/gps")]
        [HttpPut]
        public HttpResponseMessage PutUserGpsLocation ([FromBody]UserLocationModel model, string uid)
        {
            try
            {
                LogRequest(model);

                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                var nearByUser = new NearByUser() { UId = uid, Latitude = model.Latitude, Longitude = model.Longitude };
                bool opertionResult = NearByMeManager.GetInstance().UpsertUserGpsLocation(nearByUser);

                if (!opertionResult)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
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

        [Route("{uid:regex(^([0-9]+)(\\s)*$)}/setting")]
        [HttpPut]
        public HttpResponseMessage UpdateSetting([FromBody]NearByMeSettingModel model, string uid)
        {
            try
            {
                LogRequest(model);

                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                var setting = new NearByMeSetting()
                {
                    UserName = uid,
                    Enabled = model.Enabled,
                    NotificationOn = model.NotificationOn,
                    NotificationTone = model.NotificationTone,
                    ShowInfo = model.ShowInfo,
                    ShowProfileImage = model.ShowProfileImage,
                    IsPrivateAccount = model.IsPrivateAccount
                };

                bool opertionResult = NearByMeManager.GetInstance().UpsertNearByMeSetting(setting);

                if (!opertionResult)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
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

        [Route("friend-request")]
        [HttpPost]
        public HttpResponseMessage PostFriendRequest([FromBody]FriendRequestModel model)
        {
            try
            {
                LogRequest(model);

                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                if(model.UId == model.FriendUId)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "UId and FriendUId cannot be same.");
                }

                FriendRequest request = NearByMeManager.GetInstance().IsFriendRequestExist(model.UId, model.FriendUId);

                if (request != null && request.SenderId == model.UId)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (request != null && request.RecipientId == model.UId && request.Status != FriendRequestStatus.Pending)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "A request is already exist with " + request.Status.ToString() + ".");
                }

                var friendRequest = new FriendRequest() { SenderId = model.UId, RecipientId = model.FriendUId, Status = FriendRequestStatus.Pending };
                bool operationCompleted = NearByMeManager.GetInstance().UpsertFriendRequest(friendRequest);

                return Request.CreateResponse(HttpStatusCode.OK);
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

        [Route("{uid:regex(^([0-9]+)(\\s)*$)}/friend-request/{senderId:regex(^([0-9]+)(\\s)*$)}/accept")]
        [HttpPut]
        public HttpResponseMessage AcceptFriendRequest(string uId, string senderId)
        {
            try
            {
                LogRequest("UId: " + uId + ", SenderId: " + senderId);

                if (uId == senderId)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "UId and FriendUId cannot be same.");
                }

                FriendRequest request = NearByMeManager.GetInstance().IsFriendRequestExist(uId, senderId);

                if(request == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Friend request does not exist." );
                }

                if (request != null && request.SenderId == uId)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "You cannot take action on your request.");
                }

                if (request != null && request.Status == FriendRequestStatus.Accepted)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (request != null && request.Status != FriendRequestStatus.Pending)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request cannot be updated as it is no more in pending state.");
                }

                var friendRequest = new FriendRequest() { RecipientId = senderId, SenderId = uId, Status = FriendRequestStatus.Accepted };
                bool operationCompleted = NearByMeManager.GetInstance().UpsertFriendRequest(friendRequest);

                return Request.CreateResponse(HttpStatusCode.OK);
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

        [Route("{uid:regex(^([0-9]+)(\\s)*$)}/friend-request/{senderId:regex(^([0-9]+)(\\s)*$)}/reject")]
        [HttpPut]
        public HttpResponseMessage RejectFriendRequest(string uId, string senderId)
        {
            try
            {
                LogRequest("UId: " + uId + ", SenderId: " + senderId);

                if (uId == senderId)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "UId and FriendUId cannot be same.");
                }

                FriendRequest request = NearByMeManager.GetInstance().IsFriendRequestExist(uId, senderId);

                if (request == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Friend request does not exist.");
                }

                if(request != null && request.SenderId == uId)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "You cannot take action on your request.");
                }

                if (request != null && request.Status == FriendRequestStatus.Rejected)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (request != null && request.Status != FriendRequestStatus.Pending)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request cannot be updated as it is no more in pending state.");
                }

                var friendRequest = new FriendRequest() { RecipientId = senderId, SenderId = uId, Status = FriendRequestStatus.Rejected };
                bool operationCompleted = NearByMeManager.GetInstance().UpsertFriendRequest(friendRequest);

                return Request.CreateResponse(HttpStatusCode.OK);
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

        [Route("{uid:regex(^([0-9]+)(\\s)*$)}/friend-request-details")]
        [HttpGet]
        public HttpResponseMessage GetFriendRequestDetails(string uId)
        {
            try
            {
                LogRequest("UId: " + uId);

                FriendRequestDetails friendRequestDetails = NearByMeManager.GetInstance().GetFriendRequestDetails(uId);

                return Request.CreateResponse(HttpStatusCode.OK, friendRequestDetails);
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

        [Route("{uid:regex(^([0-9]+)(\\s)*$)}friend-request/{friendUId:regex(^([0-9]+)(\\s)*$)}")]
        [HttpDelete]
        public HttpResponseMessage DeleteFriendRequest(string uId, string friendUId)
        {
            try
            {
                LogRequest("UId: " + uId + ", SenderId: " + friendUId);

                if (uId == friendUId)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "UId and FriendUId cannot be same.");
                }

                var friendRequest = new FriendRequest() { RecipientId = uId, SenderId = friendUId };
                bool operationCompleted = NearByMeManager.GetInstance().DeleteFriendRequest(friendRequest);

                return Request.CreateResponse(HttpStatusCode.OK);
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

        //By Uzair
        [Route("GetNearByUsersByLocation")]
        [HttpGet]
        public HttpResponseMessage GetNearByUsersByLocation([FromUri]UserSearch model)
        {
            try
            {
                LogRequest(model);

                if (!ModelState.IsValid)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                List<NearByUser> nearByUsers = NearByMeManager.GetInstance().GetNearByUsersByLocation(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { NearByUsers = nearByUsers });
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
