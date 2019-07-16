using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Common;
using RestSharp;
using Newtonsoft.Json;
using Logger;


namespace LibNeeo.Voip
{
    public static class NeeoVoipApi
    {
        /// <summary>
        /// 
        /// </summary>
        private static string _voipServerUrl;

        private const string Password = "&pass={password}";
        private const string PushEnabled = "&pushEnabled={pushEnabled}";
        private const string Status = "&status={status}";

        private static string _mcrServiceUrl;
        private const string McrServiceUrl = "mcrServiceUrl";
        private const string Mcrc = "?mcrc=get";
        private const string UserID = "&mob={mobile}";
        private const string OperationID = "&opid={opid}";
        private const string Flush = "&flush={flush}";
        private const string RegistrationFailureError = "Registration Failed";
        private const string UserExistanceError = "Mobile Number Already Exists";
        private const string PasswordUpdateError = "Password Updating ";
        private const string PushEnabledError = "Push Enabled";
        private const string UserNotFoundError = "User Not Found";


        #region Voip Registration

        /// <summary>
        /// 
        /// </summary>
        public static void RegisterUser(string username, string password, PushEnabled pushEnabled)
        {
            RequestMode requestMode = RequestMode.Add;
            
            if (_voipServerUrl == null)
            {
                _voipServerUrl = ConfigurationManager.AppSettings[NeeoConstants.VoipServerUrl];
            }

            RestRequest request = new RestRequest();
            request.AddQueryParameter("mode", requestMode.ToString("G").ToLower());
            request.AddQueryParameter("mob", username);
            request.AddQueryParameter("pass", password);

            if (pushEnabled != Voip.PushEnabled.NotSpecified)
            {
                request.AddQueryParameter("pushEnabled", pushEnabled.ToString("D"));
            }

            try
            {
                ExecuteRequest(request, requestMode);
            }
            catch (ApplicationException firstAppException)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                     System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, firstAppException.Message);
                try
                {
                    ExecuteBackupPolicy(firstAppException.Message, request, requestMode);
                }
                catch (ApplicationException secondAppException)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                     System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, secondAppException.Message);
                    try
                    {
                        ExecuteBackupPolicy(firstAppException.Message, request, requestMode);
                    }
                    catch (ApplicationException thirdAppException)
                    {
                        throw new Exception(thirdAppException.Message);
                    }

                }

            }
        }

        private static void ExecuteBackupPolicy(string message, RestRequest request, RequestMode requestMode)
        {
            switch (message)
            {
                case RegistrationFailureError:
                    ExecuteRequest(request, requestMode);
                    break;
                case UserExistanceError:
                    UpdateUserAccount(request.Parameters[1].Value.ToString(), request.Parameters[2].Value.ToString(), (PushEnabled)Convert.ToInt16(request.Parameters[3].Value), UserStatus.NotSpecified);
                    break;
                case PasswordUpdateError:
                case PushEnabledError:
                    UpdateUserAccount(request.Parameters[1].Value.ToString(), request.Parameters[2].Value.ToString(), (PushEnabled)Convert.ToInt16(request.Parameters[3].Value), UserStatus.NotSpecified);
                    break;
                case UserNotFoundError:
                    RegisterUser(request.Parameters[1].Value.ToString(), request.Parameters[2].Value.ToString(), (PushEnabled)Convert.ToInt16(request.Parameters[3].Value));
                    break;
                default:
                    throw new Exception(message);
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pushEnabled"></param>
        public static void UpdateUserAccount(string username, string password, PushEnabled pushEnabled, UserStatus userStatus)
        {
            RequestMode requestMode = RequestMode.Set;

            if (_voipServerUrl == null)
            {
                _voipServerUrl = ConfigurationManager.AppSettings[NeeoConstants.VoipServerUrl];
            }

            RestRequest request = new RestRequest();
            request.AddQueryParameter("mode", requestMode.ToString("G").ToLower());
            request.AddQueryParameter("mob", username);

            if (password != null)
            {
                request.AddQueryParameter("pass", password);
            }

            if (pushEnabled != Voip.PushEnabled.NotSpecified)
            {
                request.AddQueryParameter("pushEnabled", pushEnabled.ToString("D"));
            }

            if (userStatus != Voip.UserStatus.NotSpecified)
            {
                request.AddQueryParameter("status", userStatus.ToString("D"));
            }

            try
            {
                ExecuteRequest(request, requestMode);
            }
            catch (ApplicationException firstAppException)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                     System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, firstAppException.Message);
                try
                {
                    if (!NeeoUtility.IsNullOrEmpty(password))
                    {
                        ExecuteBackupPolicy(firstAppException.Message, request, requestMode);
                    }
                    else
                    {
                        throw new ApplicationException(CustomHttpStatusCode.ServerInternalError.ToString("D"));
                    }
                }
                catch (ApplicationException secondAppException)
                {
                    throw new Exception(secondAppException.Message);
                }
            }
        }

        /// <summary>
        /// Executes the request and manipulates the response.
        /// </summary>
        /// <param name="request">An object containing the request to be processed.</param>
        private static void ExecuteRequest(RestRequest request, RequestMode requestMode)
        {
            const string responseResult = "result";
            const string responseSuccessStatus = "Success";
            const string responseMessage = "message";
            const string responseUserStatus = "status";
            const string responsePushEnabledStatus = "pushEnabled";
            const string responsePasswordChangeStatus = "pass";
            const string startingString = "{";
            int indexOfStartingString = 0;
            RestClient client = new RestClient(_voipServerUrl);
            var response = client.Execute(request);

            if (response.ErrorException == null)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    switch (requestMode)
                    {
                        case RequestMode.Add:
                            var responseContent = JsonConvert.DeserializeObject<dynamic>(response.Content);
                            string responseValue = responseContent[responseResult].ToString();
                            if (responseValue != responseSuccessStatus)
                            {
                                throw new ApplicationException(responseContent[responseMessage].ToString());
                            }
                            break;
                        case RequestMode.Set:
                            indexOfStartingString = response.Content.IndexOf(startingString);
                            var voipResponse = JsonConvert.DeserializeObject<VoipResponse>(response.Content.Substring(indexOfStartingString, response.Content.Length - indexOfStartingString));
                            if (voipResponse.Pass != null)
                            {
                                if (voipResponse.Pass[responseResult] != responseSuccessStatus)
                                {
                                    throw new ApplicationException(voipResponse.Pass[responseMessage].ToString());
                                }
                            }
                            if (voipResponse.PushEnabled != null)
                            {
                                if (voipResponse.PushEnabled[responseResult] != responseSuccessStatus)
                                {
                                    throw new ApplicationException(voipResponse.PushEnabled[responseMessage].ToString());
                                }
                            }
                            if (voipResponse.Status != null)
                            {
                                if (voipResponse.Status[responseResult] != responseSuccessStatus)
                                {
                                    throw new ApplicationException(voipResponse.Status[responseMessage].ToString());
                                }
                            }
                            break;

                        case RequestMode.Get:
                            // Do nothing. It will be updated based on the future requirements.
                            break;
                    }
                }
                else
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                           System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, response.Content.ToString());
                    throw new ApplicationException(HttpStatusCode.InternalServerError.ToString("D"));
                }
            }
            else
            {
                //log response.ErrorMessage
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, response.ErrorMessage);
                throw new ApplicationException(CustomHttpStatusCode.ServerConnectionError.ToString("D"));
            }
        }

        #endregion

        #region MCR Data

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static string GetMcrCount(string userID)
        {
            const string responseResult = "\"result\":\"Error\"";
            if (_mcrServiceUrl == null)
            {
                _mcrServiceUrl = ConfigurationManager.AppSettings[McrServiceUrl];
            }
            const int operationID = 1;
            const string responseMcrCount = "mcrCount";

            string requestUrl = _mcrServiceUrl + Mcrc + UserID + OperationID;
            RestRequest request = new RestRequest(requestUrl);
            request.AddUrlSegment("mobile", userID);
            request.AddUrlSegment("opid", operationID.ToString());

            RestClient client = new RestClient();
            var response = client.Execute(request);

            if (response.ErrorException == null)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseContent = JsonConvert.DeserializeObject<dynamic>(response.Content);
                    if (response.Content.Contains(responseResult))
                    {
                        LogManager.CurrentInstance.ErrorLogger.LogError(
                           System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, responseContent.ToString());
                        throw new ApplicationException(HttpStatusCode.InternalServerError.ToString("D"));
                    }
                    else
                    {
                        return responseContent[responseMcrCount].ToString();
                    }
                }
                else
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, response.Content.ToString());
                    throw new ApplicationException(HttpStatusCode.InternalServerError.ToString("D"));
                }
            }
            else
            {

                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, response.ErrorMessage);
                throw new ApplicationException(HttpStatusCode.InternalServerError.ToString("D"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="flush"></param>
        /// <returns></returns>
        public static McrData GetMcrDetails(string userID, bool flush)
        {
            const string responseResult = "\"result\":\"Error\"";
            if (_mcrServiceUrl == null)
            {
                _mcrServiceUrl = ConfigurationManager.AppSettings[McrServiceUrl];
            }
            const int operationID = 2;

            string requestUrl = _mcrServiceUrl + Mcrc + UserID + OperationID + Flush;
            RestRequest request = new RestRequest(requestUrl);
            request.AddUrlSegment("mobile", userID);
            request.AddUrlSegment("opid", operationID.ToString());
            request.AddUrlSegment("flush", Convert.ToString(flush == true ? 1 : 0));

            RestClient client = new RestClient();
            var response = client.Execute(request);

            if (response.ErrorException == null)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    if (response.Content.Contains(responseResult))
                    {
                        var responseContent = JsonConvert.DeserializeObject<dynamic>(response.Content);
                        LogManager.CurrentInstance.ErrorLogger.LogError(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, responseContent.ToString());
                        throw new ApplicationException(HttpStatusCode.InternalServerError.ToString("D"));
                    }
                    else
                    {
                        var responseContent = JsonConvert.DeserializeObject<McrData>(response.Content);
                        return responseContent;
                    }
                }
                else
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                           System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, response.Content.ToString());
                    throw new ApplicationException(HttpStatusCode.InternalServerError.ToString("D"));
                }
            }
            else
            {

                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, response.ErrorMessage);
                throw new ApplicationException(HttpStatusCode.InternalServerError.ToString("D"));
            }
        }

        #endregion

    }
}
