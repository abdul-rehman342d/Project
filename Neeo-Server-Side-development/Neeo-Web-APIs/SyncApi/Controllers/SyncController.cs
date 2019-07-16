using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using Common;
using LibNeeo;
using Logger;
using Newtonsoft.Json;
using Common.Controllers;
using SyncApi.Models;


namespace SyncApi.Controllers
{
    [RoutePrefix("Service/NeeoSyncingService.svc")]
    
    public class SyncController : NeeoApiController
    {
        private bool _logRequestResponse =
           Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.LogRequestResponse]);
        
        #region Contact Sycning

        #region Iphone 1.x.x, Android 1.x.x
        /*They will be deprecated in future.*/

        /// <summary>
        /// Lookup the user's contacts on the server for existance.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        /// <param name="contacts">A array of type 'Contact' containing the contacts.</param>
        /// <returns>A list containing user's contacts those are application users.</returns>
        [HttpPost]
        [Route("SyncUpContacts")]
        public HttpResponseMessage SyncUpContacts(ContactSyncRequest request)
        {
            Dictionary<string, object> responseData = new Dictionary<string, object>();
            responseData.Add("SyncUpContactsResult", null);

            if (request != null)
            {
                request.userID = (request.userID != null) ? request.userID.Trim() : request.userID;

                #region log user request and response

                /***********************************************
             To log user request
             ***********************************************/
                if (_logRequestResponse)
                {
                    LogManager.CurrentInstance.InfoLogger.LogInfo(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" + "Request ===> User ID : " +
                        request.userID + "- Contact List :" + JsonConvert.SerializeObject(request.contacts));
                }

                #endregion

                //#region user authentication 

                //IEnumerable<string> headerValues;
                //string keyFromClient = "";
                //if (Request.Headers.TryGetValues("key", out headerValues))
                //{
                //    keyFromClient = headerValues.First();
                //}

                //if (NeeoUtility.AuthenticateUserRequest(syncRequest.userID, keyFromClient))
                //{
                //    #endregion

                if (!NeeoUtility.IsNullOrEmpty(request.userID) && request.contacts != null)
                {
                    if (request.contacts.Length > 0)
                    {
                        NeeoUser user = new NeeoUser(request.userID);
                        try
                        {
                            var response = user.GetContactsRosterState(request.contacts, false, true);

                            #region log user request and response

                            /***********************************************
                         To log user response
                         ***********************************************/
                            if (_logRequestResponse)
                            {
                                LogManager.CurrentInstance.InfoLogger.LogInfo(
                                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                                    System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                                    "Response==> User ID : " + request.userID + "- Contact List :" +
                                    JsonConvert.SerializeObject(response));
                            }

                            #endregion

                            responseData["SyncUpContactsResult"] = response;
                            return Request.CreateResponse(HttpStatusCode.OK, responseData);
                        }
                        catch (ApplicationException appExp)
                        {
                            return SetCustomResponseMessage(responseData,
                                (HttpStatusCode)(Convert.ToInt32(appExp.Message)));
                        }
                        catch (Exception exp)
                        {
                            LogManager.CurrentInstance.ErrorLogger.LogError(
                                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, responseData);
                        }
                    }
                }
            }
            return SetCustomResponseMessage(responseData, HttpStatusCode.BadRequest);
                
            //}
            //else
            //{
            //    return Request.CreateResponse(HttpStatusCode.Unauthorized, responseData);
            //}
        }

        #endregion

        #region Iphone 2.0.0, Android x.x.x

        /// <summary>
        /// Lookup the user's contacts on the server for existance. It is a wrapping method with short parameter name.
        /// </summary>
        /// <param name="uID">A string containing the user id.</param>
        /// <param name="contacts">A array of type 'Contact' containing the contacts.</param>
        /// <returns>A list containing user's contacts those are application users.</returns>
        [HttpPost]
        [Route("SyncContacts")]
        public HttpResponseMessage SyncContacts(ContactSyncRequest request)
        {
            Dictionary<string, object> responseData = new Dictionary<string, object>();
            responseData.Add("SyncContactsResult", null);

            if (request != null)
            {
                request.uID = (request.uID != null) ? request.uID.Trim() : request.uID;

                #region log user request and response

                /***********************************************
             To log user request
             ***********************************************/
                if (_logRequestResponse)
                {
                    LogManager.CurrentInstance.InfoLogger.LogInfo(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" + "Request ===> User ID : " +
                        request.uID + "- Contact List :" + JsonConvert.SerializeObject(request.contacts));
                }

                #endregion

                //#region user authentication 

                //IEnumerable<string> headerValues;
                //string keyFromClient = "";
                //if (Request.Headers.TryGetValues("key", out headerValues))
                //{
                //    keyFromClient = headerValues.First();
                //}

                //if (NeeoUtility.AuthenticateUserRequest(syncRequest.uID, keyFromClient))
                //{
                //    #endregion

                if (!NeeoUtility.IsNullOrEmpty(request.uID) && request.contacts != null)
                {
                    if (request.contacts.Length > 0)
                    {
                        bool getAllContacts = request.all ?? true;
                        NeeoUser user = new NeeoUser(request.uID);
                        try
                        {
                            var result = user.GetContactsRosterState(request.contacts, true, getAllContacts);
                            if (getAllContacts)
                            {
                                responseData["SyncContactsResult"] = result.ConvertAll(
                                    new Converter<ContactDetails, ContactState>(ContactDetailsToContactStateMapping));
                            }
                            else
                            {
                                responseData["SyncContactsResult"] = result.ConvertAll(
                                    new Converter<ContactDetails, ContactSubscriptionDetails>(ContactDetailsToContactSubscriptionMapping)); ;
                            }
                            

                            #region log user request and response

                            /***********************************************
                             To log user response
                            ***********************************************/
                            if (_logRequestResponse)
                            {
                                LogManager.CurrentInstance.InfoLogger.LogInfo(
                                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                                    System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" +
                                    "Response==> User ID : " + request.uID + "- Contact List :" +
                                    JsonConvert.SerializeObject(responseData));
                            }

                            #endregion

                            return Request.CreateResponse(HttpStatusCode.OK, responseData);
                        }
                        catch (ApplicationException appExp)
                        {
                            return SetCustomResponseMessage(responseData,
                                (HttpStatusCode) (Convert.ToInt32(appExp.Message)));
                        }
                        catch (Exception exp)
                        {
                            LogManager.CurrentInstance.ErrorLogger.LogError(
                                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                            return Request.CreateResponse(HttpStatusCode.InternalServerError, responseData);
                        }
                    }
                }
            }
            return SetCustomResponseMessage(responseData, HttpStatusCode.BadRequest);
            //}
            //else
            //{
            //    return Request.CreateResponse(HttpStatusCode.Unauthorized, responceDic);
            //    
            //}
        }

        private static ContactState ContactDetailsToContactStateMapping(ContactDetails contactDetails)
        {
            return new ContactState()
            {
                Ph = contactDetails.ContactPhoneNumber,
                IsUsr = contactDetails.IsNeeoUser,
                IsSub = contactDetails.IsAlreadySubscribed,
                //Ts = contactDetails.AvatarTimestamp
            };
        }

        private static ContactSubscriptionDetails ContactDetailsToContactSubscriptionMapping(ContactDetails contactDetails)
        {
            return new ContactSubscriptionDetails()
            {
                Ph = contactDetails.ContactPhoneNumber,
                Ts = contactDetails.AvatarTimestamp,
                IsSub = contactDetails.IsAlreadySubscribed
            };
        }

        #endregion


        #endregion

        #region Contact Avatar Timestamp

        /// <summary>
        /// Gets avatar timestamps details for the user's provided contacts.
        /// </summary>
        /// <param name="uID">A string containing the user id.</param>
        /// <param name="contacts">A string containing the ',' separated contacts.</param>
        /// <returns>A list containing user's contacts avatar timestamp.</returns>

        [HttpPost]
        [Route("GetContactsTS")]
        public HttpResponseMessage GetContactAvatarTimestamp(AvatarTimestampRequest request)
        {
            if (request != null)
            {
                request.uID = (request.uID != null)
                    ? request.uID.Trim()
                    : request.uID;


                #region log user request and response

                /***********************************************
                    To log user response
            ***********************************************/
                if (_logRequestResponse)
                {
                    LogManager.CurrentInstance.InfoLogger.LogInfo(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                        System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" + "Response==> User ID : " +
                        request.uID + "- Contact List :" + request.contacts);
                }

                #endregion

                //#region user authentication 

                //IEnumerable<string> headerValues;
                //string keyFromClient = "";
                //if (Request.Headers.TryGetValues("key", out headerValues))
                //{
                //    keyFromClient = headerValues.First();
                //}

                //if (NeeoUtility.AuthenticateUserRequest(getTimeStampRequest.uID, keyFromClient))
                //{
                //    #endregion

                    if (!NeeoUtility.IsNullOrEmpty(request.uID) &&
                        !NeeoUtility.IsNullOrEmpty(request.contacts))
                    {
                        NeeoUser user = new NeeoUser(request.uID.Trim());
                        var result = user.GetContactsAvatarTimestamp(request.contacts);

                        #region log user request and response

                        /***********************************************
                    To log user response
                    ***********************************************/
                        if (_logRequestResponse)
                        {
                            LogManager.CurrentInstance.InfoLogger.LogInfo(
                                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                                System.Reflection.MethodBase.GetCurrentMethod().Name + "===>" + "Response==> User ID : " +
                                request.uID + "- Contact List :" + JsonConvert.SerializeObject(result));
                        }

                        #endregion

                        return Request.CreateResponse(HttpStatusCode.OK, result);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            //}
            //else
            //{

            //    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            //}
        }

            #endregion
        

       
    }
}
