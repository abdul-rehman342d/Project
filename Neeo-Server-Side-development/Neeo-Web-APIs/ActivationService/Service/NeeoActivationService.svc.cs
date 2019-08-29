using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Activation;
using System.Configuration;
using System.Web;
using Common;
using LibNeeo.Activation;
using LibNeeo;
using Logger;
using Newtonsoft.Json;
using LibNeeo.SMS;

namespace ActivationService
{

    /// <summary>
    /// Gives implementation of the service resources/ objects defined in the interface <see cref="INeeoActivationService"/>.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class NeeoActivationService : INeeoActivationService
    {
        private readonly bool _logRequestResponse = Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.LogRequestResponse]);
        private readonly bool _numberValidityCheck = Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.NumberValidityCheck]);


        #region INeeo Implementation

        #region Activation

        #region Iphone 1.x.x, Android 1.x.x
        /*They will be deprecated in future.*/

        /// <summary>
        /// Sends activation code to the phone number provided in <paramref name="phoneNumber"/>.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="phoneNumber">A phone number on which activation code has to be sent.</param>
        /// <param name="devicePlatform">Platform of the user's device.</param>
        /// <param name="activationCode">A code that has to be sent on the give phone number.</param>
        /// <param name="isResend">A bool value indicating this is a resending code request if true,otherwise false (default value). </param>
        /// <param name="isRegenerated">A bool value indicating this is a regenerated code sending request if true,otherwise false (default value). </param>
        /// <returns>An integer if 0 = sms has not been sent, 1 = sms has been successfully sent or remaining minutes to unblock user.</returns>

        public int SendActivationCode(string phoneNumber, short devicePlatform, string activationCode, bool isResend, bool isRegenerated)
        {

            phoneNumber = (phoneNumber != null) ? phoneNumber.Trim() : phoneNumber;
            activationCode = (activationCode != null) ? activationCode.Trim() : activationCode;
            int smsSendingResult = (int)SmsSendingStatus.SendingFailed;

            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + " ===> " +
                    "Request ===> phoneNumber : " + phoneNumber + ", devicePlatform : " + devicePlatform +
                    ", activationCode : " + activationCode + ", isResend : " + isResend + ", isRegenerated : " +
                    isRegenerated);
            }

            #endregion

            //#region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //   if (NeeoUtility.AuthenticateUserRequest( phoneNumber, keyFromClient))
            //{
            //#endregion

            DevicePlatform userDevicePlatform = (DevicePlatform)devicePlatform;
            uint tempActivationCode = 0;
            ulong tempPhoneNumber = 0;
            if (!NeeoUtility.IsNullOrEmpty(phoneNumber) && !NeeoUtility.IsNullOrEmpty(activationCode) && Enum.IsDefined(typeof(DevicePlatform), devicePlatform) && uint.TryParse(activationCode, out tempActivationCode) && ulong.TryParse(phoneNumber, out tempPhoneNumber))
            {
                try
                {
                    if (NeeoUtility.IsPhoneNumberInInternationalFormat(phoneNumber))
                    {
                        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);
                        return smsSendingResult;
                    }
                    if (!NeeoUtility.ValidatePhoneNumber(NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber)) && _numberValidityCheck)
                    {
                        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);
                        return smsSendingResult;
                    }
                    smsSendingResult = NeeoActivation.SendActivationCodeToUnBlockedUser(phoneNumber, userDevicePlatform, activationCode, isResend, isRegenerated);
                }
                catch (ApplicationException appExp)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                }
                return smsSendingResult;
            }
            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            return smsSendingResult;

            //}
            //   else
            //   {
            //       NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);
            //       return smsSendingResult;
            //   }

        }

        /// <summary>
        /// Registers and configures user account.
        /// </summary>
        /// <param name="phoneNumber">A string containing phone number for registering account.</param>
        /// <param name="client">An object containing the client information.</param>
        /// <returns>true if the account is successfully registered on the server; otherwise, false.</returns>
        public bool RegisterUser(string phoneNumber, UserAgent client)
        {

            phoneNumber = (phoneNumber != null) ? phoneNumber.Trim() : phoneNumber;
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + " ===> " +
                    "Request ===> phoneNumber : " + phoneNumber + ", client : " + JsonConvert.SerializeObject(client));
            }

            #endregion


            // #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(phoneNumber, keyFromClient))
            //{
            //#endregion
            if (client != null)
            {
                ulong tempPhoneNumber = 0;
                if (!NeeoUtility.IsNullOrEmpty(phoneNumber) && !NeeoUtility.IsNullOrEmpty(client.DeviceVenderID) && !NeeoUtility.IsNullOrEmpty(client.ApplicationID) && Enum.IsDefined(typeof(DevicePlatform), client.DevicePlatform) && Enum.IsDefined(typeof(PushNotificationSource), client.Pns) && ulong.TryParse(phoneNumber, out tempPhoneNumber))
                {
                    try
                    {
                        if (NeeoUtility.IsPhoneNumberInInternationalFormat(phoneNumber))
                        {
                            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);
                            return false;
                        }
                        if (!NeeoUtility.ValidatePhoneNumber(NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber)) && _numberValidityCheck)
                        {
                            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);
                            return false;
                        }
                        if (NeeoActivation.SetupUserAccount(phoneNumber, client.ApplicationID, client.AppVer, new DeviceInfo() { DeviceModel = client.DM, DevicePlatform = (DevicePlatform)client.DevicePlatform, DeviceVenderID = client.DeviceVenderID, DeviceToken = client.DeviceToken, PushNotificationSource = (PushNotificationSource)client.Pns, OsVersion = client.OsVer }))
                        {
                            return true;
                        }
                        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.TransactionFailure);
                        return false;
                    }
                    catch (ApplicationException appExp)
                    {
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                        return false;
                    }
                }
            }
            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            return false;
            //}
            //   else
            //   {
            //      NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);

            //       return false;
            //   }
        }

        #endregion

        #region Iphone 2.0.0, Android x.x.x

        /// <summary>
        /// Sends activation code to the phone number provided in <paramref name="ph"/>. It is a wrapping method with short parameter name.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="ph">A phone number on which activation code has to be sent.</param>
        /// <param name="dP">Platform of the user's device.</param>
        /// <param name="actCode">A code that has to be sent on the give phone number.</param>
        /// <param name="isRes">A bool value indicating this is a resending code request if true,otherwise false (default value). </param>
        /// <param name="isReg">A bool value indicating this is a regenerated code sending request if true,otherwise false (default value). </param>
        /// <param name="appKey">A string containing the appKey(For Android).</param>
        /// <returns>An integer if 0 = sms has not been sent, 1 = sms has been successfully sent or remaining minutes to unblock user.</returns>
        public int SendActCode(string ph, short dP, string actCode, bool isRes, bool isReg, string appKey, short sType)
        {
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + " ===> " +
                    "Request ===> phoneNumber : " + ph + ", devicePlatform : " + dP +
                    ", activationCode : " + actCode + ", isResend : " + isRes + ", isRegenerated : " + isReg + ", appKey : " + appKey + ", serviceType: " + sType);
            }

            #endregion

            //return this.SendActivationCode(ph, dP, actCode, isRes, isReg);
            ph = (ph != null) ? ph.Trim() : ph;
            actCode = (actCode != null) ? actCode.Trim() : actCode;
            appKey = appKey?.Trim();

            int codeSendingResult = (int)SmsSendingStatus.SendingFailed;
            DevicePlatform userDevicePlatform = (DevicePlatform)dP;
            CodeSendingService codeSendingService = (CodeSendingService)sType;
            uint tempActivationCode = 0;
            ulong tempPhoneNumber = 0;

            string restrictSpecifiedAeasSMS = ConfigurationManager.AppSettings[NeeoConstants.RestrictSpecifiedAeasSMS];
            string registerationRequestCheckEnable = ConfigurationManager.AppSettings[NeeoConstants.EnableRegisterationRequestCheck];

            if (!NeeoUtility.IsNullOrEmpty(ph) && !NeeoUtility.IsNullOrEmpty(actCode) && Enum.IsDefined(typeof(DevicePlatform), dP) && Enum.IsDefined(typeof(CodeSendingService), sType) && uint.TryParse(actCode, out tempActivationCode) && ulong.TryParse(ph, out tempPhoneNumber))
            {
                try
                {
                    if (NeeoUtility.IsPhoneNumberInInternationalFormat(ph))
                    {
                        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);
                        return codeSendingResult;
                    }
                    if (!NeeoUtility.ValidatePhoneNumber(NeeoUtility.FormatAsIntlPhoneNumber(ph)) && _numberValidityCheck)
                    {
                        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);
                        return codeSendingResult;
                    }
                    if (codeSendingService == CodeSendingService.Call)
                    {
                        codeSendingResult = NeeoActivation.CallForActivationCode(ph, userDevicePlatform, actCode);
                    }
                    else
                    {
                        if (registerationRequestCheckEnable == "0" ||NeeoActivation.CheckUserRegisterationRequest(ph))
                        {
                            if (ConfigurationManager.AppSettings[NeeoConstants.AWSStatus] != "enabled")
                            {

                                if (restrictSpecifiedAeasSMS == "1" && (ph.StartsWith("994") || ph.StartsWith("33")))
                                {
                                    SmsManager.InsertActivationSMSLog("-1", ph, NeeoUtility.GetActivationMessage(actCode, appKey), isRes, isReg, 1, appKey, "FRAZ", "", false);
                                }
                                else
                                {
                                    codeSendingResult = NeeoActivation.SendActivationCode(ph, userDevicePlatform, actCode, appKey, true);
                                }
                            }
                            else
                            {
                                //Amazon
                                string vendorMessageId = "";
                                string status = "";
                                SmsManager.SendThroughAmazon(ph, NeeoUtility.GetActivationMessage(actCode, appKey).ToString(), isRes, 1, true, out vendorMessageId, out status);
                                codeSendingResult = 1;
                                return codeSendingResult;
                            }
                        }
                        else
                        {
                            codeSendingResult = -1;
                            SmsManager.InsertActivationSMSLog("-1", ph, NeeoUtility.GetActivationMessage(actCode, appKey).ToString(), false, false, 1, "", "UnRegisteredUser", "", false);
                        }
                    }
                }
                catch (ApplicationException appExp)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                }
                return codeSendingResult;
            }
            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            return codeSendingResult;
        }

        /// <summary>
        /// Sends activation code to the phone number provided in <paramref name="ph"/>. It is a wrapping method with short parameter name.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="ph">A phone number on which activation code has to be sent.</param>
        /// <param name="dP">Platform of the user's device.</param>
        /// <param name="actCode">A code that has to be sent on the give phone number.</param>
        /// <param name="isRes">A bool value indicating this is a resending code request if true,otherwise false (default value). </param>
        /// <param name="isReg">A bool value indicating this is a regenerated code sending request if true,otherwise false (default value). </param>
        /// <param name="appKey">A string containing the appKey(For Android).</param>
        /// <returns>An integer if 0 = sms has not been sent, 1 = sms has been successfully sent or remaining minutes to unblock user.</returns>
        /// <param name="deviceInfo">A string which contain the calling device info. </param>
        /// <param name="isDebugged">A bool to determine if used for debuging and no need to send sms   .</param>
        public int SendAppActivationCode(string ph, short dP, string actCode, bool isRes, bool isReg, string appKey, short sType, string deviceInfo, bool isDebugged)
        {
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + " ===> " +
                    "Request ===> phoneNumber : " + ph + ", devicePlatform : " + dP +
                    ", activationCode : " + actCode + ", isResend : " + isRes + ", isRegenerated : " + isReg + ", appKey : " + appKey + ", serviceType: " + sType);
            }

            #endregion

            if (deviceInfo == null)
            {
                deviceInfo = string.Empty;
            }


            //return this.SendActivationCode(ph, dP, actCode, isRes, isReg);
            ph = (ph != null) ? ph.Trim() : ph;
            actCode = (actCode != null) ? actCode.Trim() : actCode;
            appKey = appKey?.Trim();

            int codeSendingResult = (int)SmsSendingStatus.SendingFailed;
            DevicePlatform userDevicePlatform = (DevicePlatform)dP;
            CodeSendingService codeSendingService = (CodeSendingService)sType;
            uint tempActivationCode = 0;
            ulong tempPhoneNumber = 0;
            string restrictSpecifiedAeasSMS = ConfigurationManager.AppSettings[NeeoConstants.RestrictSpecifiedAeasSMS];
            string registerationRequestCheckEnable = ConfigurationManager.AppSettings[NeeoConstants.EnableRegisterationRequestCheck];
            

            if (!NeeoUtility.IsNullOrEmpty(ph) && !NeeoUtility.IsNullOrEmpty(actCode) && Enum.IsDefined(typeof(DevicePlatform), dP) && Enum.IsDefined(typeof(CodeSendingService), sType) && uint.TryParse(actCode, out tempActivationCode) && ulong.TryParse(ph, out tempPhoneNumber))
            {
                SMSLog currentSms = new SMSLog();
                currentSms.vendorMessageId = "-1";
                currentSms.messageBody = NeeoUtility.GetActivationMessage(actCode, appKey).ToString();
                currentSms.receiver = ph;
                currentSms.isResend = isRes;
                currentSms.isRegenerate = false;
                currentSms.messageType = 1;
                currentSms.appKey = appKey;
                currentSms.status = "XXX";
                currentSms.deviceInfo = deviceInfo;
                currentSms.isDebugged = isDebugged;



                if (NeeoUtility.IsPhoneNumberInInternationalFormat(ph))
                {
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);
                    return codeSendingResult;
                }
                if (!NeeoUtility.ValidatePhoneNumber(NeeoUtility.FormatAsIntlPhoneNumber(ph)) && _numberValidityCheck)
                {
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);
                    return codeSendingResult;
                }
                try
                {
                    if (codeSendingService == CodeSendingService.Call)
                    {
                        codeSendingResult = NeeoActivation.CallForActivationCode(ph, userDevicePlatform, actCode);
                    }
                    else
                    {

                        // NeeoActivation.CheckUserAlreadyRegistered(ph)
                        if (registerationRequestCheckEnable == "0" || NeeoActivation.CheckUserRegisterationRequest(ph))
                        {
                            /*
                            if (ph.StartsWith("994") || ph.StartsWith("33"))
                            {
                                if (currentSms.isDebugged == true)
                                {
                                    currentSms.status = "Debugged";
                                }
                                else
                                {
                                    currentSms.status = "FRAZ";
                                }
                            }
                            else
                            {*/
                            if (isDebugged == false)
                            {
                                if (ConfigurationManager.AppSettings[NeeoConstants.AWSStatus] != "enabled")
                                {
                                    if (restrictSpecifiedAeasSMS == "1" && (ph.StartsWith("994") || ph.StartsWith("33")))
                                    {
                                        if (currentSms.isDebugged == true)
                                        {
                                            currentSms.status = "Debugged";
                                        }
                                        else
                                        {
                                            currentSms.status = "FRAZ";
                                        }
                                    }
                                    else
                                    {
                                        codeSendingResult = NeeoActivation.SendActivationCode(ph, userDevicePlatform, actCode, appKey, false);
                                        currentSms.status = codeSendingResult.ToString();
                                    }
                                }
                                else
                                {
                                    string vendorMessageId = "";
                                    string status = "";
                                    SmsManager.SendThroughAmazon(ph, currentSms.messageBody, isRes, 1, false, out vendorMessageId, out status);
                                    if (vendorMessageId.Length > 5 && status.Length > 0)
                                    {
                                        currentSms.vendorMessageId = vendorMessageId;
                                        currentSms.status = status;
                                    }
                                    codeSendingResult = 1;
                                }
                            }
                            else
                            {
                                currentSms.status = "Debugged";
                                codeSendingResult = 1;

                            }
                            // }
                        }
                        else
                        {
                            currentSms.status = "UnRegisteredUser";
                            codeSendingResult = -1;
                        }
                    }
                }
                catch (ApplicationException appExp)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                }
                finally
                {
                    SmsManager.InsertActivationSMSLog(currentSms.vendorMessageId, currentSms.receiver, currentSms.messageBody, currentSms.isResend, currentSms.isRegenerate, currentSms.messageType, currentSms.appKey, currentSms.status, currentSms.deviceInfo, currentSms.isDebugged);
                }
                return codeSendingResult;
            }

            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            return codeSendingResult;
        }

        ///// <summary>
        ///// Sends activation code to the phone number provided in <paramref name="ph"/>. It is a wrapping method with short parameter name.
        ///// </summary>
        ///// <remarks>
        ///// </remarks>
        ///// <param name="ph">A phone number on which activation code has to be sent.</param>
        ///// <param name="dP">Platform of the user's device.</param>
        ///// <param name="actCode">A code that has to be sent on the give phone number.</param>
        ///// <returns>An integer if 0 = sms has not been sent, 1 = sms has been successfully sent or remaining minutes to unblock user.</returns>
        //public int SendCodeTest(string ph, short dP, string actCode)
        //{
        //    #region log user request and response

        //    /***********************************************
        //     To log user request and response
        //     ***********************************************/
        //    if (_logRequestResponse)
        //    {
        //        LogManager.CurrentInstance.InfoLogger.LogInfo(
        //            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + " ===> " +
        //            "Request ===> phoneNumber : " + ph + ", devicePlatform : " + dP +
        //            ", activationCode : " + actCode);
        //    }

        //    #endregion

        //    //return this.SendActivationCode(ph, dP, actCode, isRes, isReg);
        //    ph = (ph != null) ? ph.Trim() : ph;
        //    actCode = (actCode != null) ? actCode.Trim() : actCode;

        //    int smsSendingResult = (int)SmsSendingStatus.SendingFailed;
        //    DevicePlatform userDevicePlatform = (DevicePlatform)dP;
        //    uint tempActivationCode = 0;
        //    ulong tempPhoneNumber = 0;
        //    if (!NeeoUtility.IsNullOrEmpty(ph) && !NeeoUtility.IsNullOrEmpty(actCode) && Enum.IsDefined(typeof(DevicePlatform), dP))
        //    {
        //        try
        //        {
        //            if (NeeoUtility.IsPhoneNumberInInternationalFormat(ph))
        //            {
        //                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);
        //                return smsSendingResult;
        //            }
        //            if (!NeeoUtility.ValidatePhoneNumber(NeeoUtility.FormatAsIntlPhoneNumber(ph)) && _numberValidityCheck)
        //            {
        //                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);
        //                return smsSendingResult;
        //            }
        //            smsSendingResult = NeeoActivation.SendActivationCodeTest(ph, userDevicePlatform, actCode);
        //        }
        //        catch (ApplicationException appExp)
        //        {
        //            NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
        //        }

        //        return smsSendingResult;
        //    }
        //    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
        //    return smsSendingResult;
        //}

        /// <summary>
        /// Registers and configures user account. It is a wrapping method with short parameter name.
        /// </summary>
        /// <param name="ph">A string containing phone number for registering account.</param>
        /// <param name="client">An object containing the client information.</param>
        /// <returns>true if the account is successfully registered on the server; otherwise, false.</returns>
        public bool RegisterAppUser(string ph, UserAgent client)

        {
            ph = (ph != null) ? ph.Trim() : ph;
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + " ===> " +
                    "Request ===> phoneNumber : " + ph + ", client : " + JsonConvert.SerializeObject(client));
            }

            #endregion

            //        #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //   if (NeeoUtility.AuthenticateUserRequest( ph, keyFromClient))
            //{
            //#endregion

            if (client != null)
            {
                ulong tempPhoneNumber = 0;

                if (!NeeoUtility.IsNullOrEmpty(ph) && !NeeoUtility.IsNullOrEmpty(client.DVenID) && !NeeoUtility.IsNullOrEmpty(client.AppID) && Enum.IsDefined(typeof(DevicePlatform), client.DP) && Enum.IsDefined(typeof(PushNotificationSource), client.Pns) && ulong.TryParse(ph, out tempPhoneNumber) && !NeeoUtility.IsNullOrEmpty(client.AppVer) && !NeeoUtility.IsNullOrEmpty(client.OsVer) && !NeeoUtility.IsNullOrEmpty(client.DM))
                {
                    try
                    {
                        if (NeeoUtility.IsPhoneNumberInInternationalFormat(ph))
                        {
                            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);
                            return false;
                        }
                        if (!NeeoUtility.ValidatePhoneNumber(NeeoUtility.FormatAsIntlPhoneNumber(ph)) && _numberValidityCheck)
                        {
                            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidNumber);
                            return false;
                        }
                        if (NeeoActivation.SetupUserAccount(ph, client.AppID, client.AppVer, new DeviceInfo() { DeviceModel = client.DM, DevicePlatform = (DevicePlatform)client.DP, DeviceVenderID = client.DVenID, DeviceToken = client.DToken?.Trim(), PushNotificationSource = (PushNotificationSource)client.Pns, OsVersion = client.OsVer, DeviceTokenVoIP = client.DTokenVoIP?.Trim() }))
                        {
                            return true;
                        }
                        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.TransactionFailure);
                        return false;
                    }
                    catch (ApplicationException appExp)
                    {
                        NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                        return false;
                    }
                }
            }
            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            return false;

            //}
            //   else
            //   {
            //       NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);

            //       return false;
            //   }
        }

        /// <summary>
        /// Deletes user account and its all data from Neeo.
        /// </summary>
        /// <param name="uID">A string containing the user id. </param>
        #endregion

        public void UnregisterUser(string uID)
        {
            uID = (uID != null) ? uID.Trim() : uID;
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + " ===> " +
                    "Request ===>  userID : " + uID);
            }

            #endregion

            //   #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            //#endregion
            try
            {
                NeeoActivation.DeleteUserAccount(uID);
            }
            catch (ApplicationException appExp)
            {
                NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
            }
            catch (Exception exp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.UnknownError);
            }
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);

            //}
        }

        #endregion

        #region Device Token

        /// <summary>
        /// Updates user's device token into the database for push notifications.
        /// </summary>
        /// <param name="opID">an short integer specifying the type of the operation to be performed.</param>
        /// <param name="uID">A string containing phone number as user id.</param>
        /// <param name="deviceToken">A 64-character string indicating the user's device token.</param>
        /// <returns>true if the device token is successfully updated in database; otherwise, false.</returns>
        public bool DeviceToken(short opID, string uID, UserAgent client)
        {
            uID = (uID != null) ? uID.Trim() : uID;
            const string deletionValue = "-1";

            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + " ===> " +
                    "Request ===> opID : " + opID + ", userID : " + uID + ", client : " + JsonConvert.SerializeObject(client));
            }

            #endregion

            // #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            //#endregion
            if (client != null)
            {

                OperationType operationType = (OperationType)opID;
                if ((Enum.IsDefined(typeof(DevicePlatform), client.DP) &&
                     Enum.IsDefined(typeof(PushNotificationSource), client.Pns) &&
                     !NeeoUtility.IsNullOrEmpty(client.DToken) && !NeeoUtility.IsNullOrEmpty(uID) &&
                     !NeeoUtility.IsNullOrEmpty(client.DVenID) && Enum.IsDefined(typeof(OperationType), opID) &&
                     operationType == OperationType.Update))
                {
                    //do nothing
                }
                else if ((Enum.IsDefined(typeof(DevicePlatform), client.DP) &&
                          Enum.IsDefined(typeof(PushNotificationSource), client.Pns) && !NeeoUtility.IsNullOrEmpty(uID) &&
                          !NeeoUtility.IsNullOrEmpty(client.DVenID) && Enum.IsDefined(typeof(OperationType), opID) &&
                          operationType == OperationType.Delete))
                {
                    client.DToken = deletionValue;
                }
                else
                {
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
                    return false;
                }
                NeeoUser user = new NeeoUser(uID.Trim());
                try
                {
                    return user.UpdateUserDeviceInfo(null,
                        new DeviceInfo()
                        {
                            DeviceToken = client.DToken?.Trim(),
                            DeviceTokenVoIP = client.DTokenVoIP?.Trim(),
                            PushNotificationSource = (PushNotificationSource)client.Pns,
                            DeviceVenderID = client.DVenID,
                            DevicePlatform = (DevicePlatform)client.DP
                        }, true);
                }
                catch (ApplicationException appExp)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                    return false;
                }
                catch (Exception exp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.ServerInternalError);
                    return false;
                }
            }
            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            return false;
            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);
            //    return false;
            //}
        }

        #region Deprecated function
        /// <summary>
        /// Removes user's device token from database to stop push notifications.
        /// </summary>
        /// <param name="userID">A string containing phone number as user id.</param>
        /// <returns>true if the device token is successfully removed; otherwise, false.</returns>
        /*public bool RemoveUserDeviceToken(string userID)
        {
            if (!NeeoUtility.IsNullOrEmpty(userID))
            {
                NeeoUser user = new NeeoUser(userID);
                try
                {
                    if (user.UpdateUserDeviceToken(null))
                    {
                        return true;
                    }
                    else
                    {
                        SetServiceResponseHeaders(CustomHttpStatusCode.InvalidUser);
                        return false;
                    }
                }
                catch (ApplicationException appExp)
                {
                    SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                    return false;
                }
                catch (Exception exp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                    SetServiceResponseHeaders(CustomHttpStatusCode.UnknownError);
                    return false;
                }
            }
            else
            {
                SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
                return false;
            }
        }
        */
        #endregion

        #endregion

        #region Checking Application Operatibility

        /// <summary>
        /// Checks application operatibility and update the user device information in the database for a register user. 
        /// </summary>
        /// <param name="uID">A string containing phone number as user id.</param>
        /// <param name="client">An object containing the client information.</param>

        public string CheckAppCompatibility(string uID, UserAgent client)
        {
            uID = (uID != null) ? uID.Trim() : uID;
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + " ===> " +
                    "Request ===> uID : " + uID + ", client : " + JsonConvert.SerializeObject(client));
            }

            #endregion

            //  #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            //#endregion
            if (client != null)
            {
                ulong tempPhoneNumber = 0;
                if (Enum.IsDefined(typeof(DevicePlatform), client.DP) && !NeeoUtility.IsNullOrEmpty(client.OsVer) &&
                    !NeeoUtility.IsNullOrEmpty(client.AppVer) && !NeeoUtility.IsNullOrEmpty(client.DVenID))
                {
                    switch (CheckCompatibility(client))
                    {
                        case AppCompatibility.Incompatible:
                            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.Incompatible);
                            break;
                        case AppCompatibility.IncompatibleAppVersion:
                            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.Incompatible);
                            break;
                        case AppCompatibility.IncompatibleOsVersion:
                            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.Incompatible);
                            break;
                        case AppCompatibility.Compatible:

                            if (!NeeoUtility.IsNullOrEmpty(uID))
                            {
                                if (ulong.TryParse(uID, out tempPhoneNumber))
                                {
                                    NeeoUser user = new NeeoUser(uID);

                                    try
                                    {
                                        if (!user.UpdateUserDeviceInfo(client.AppVer,
                                            new DeviceInfo()
                                            {
                                                DevicePlatform = (DevicePlatform)client.DP,
                                                DeviceVenderID = client.DVenID,
                                                OsVersion = client.OsVer
                                            }, false))
                                        {
                                            NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidUser);

                                        }
                                    }
                                    catch (ApplicationException appExp)
                                    {
                                        NeeoUtility.SetServiceResponseHeaders(
                                            (CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                                    }
                                    catch (Exception exp)
                                    {
                                        LogManager.CurrentInstance.ErrorLogger.LogError(
                                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message,
                                            exp);
                                        NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.UnknownError);
                                    }
                                }
                                else
                                {
                                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
                }
            }
            else
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }

            //}
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);

            //}
            return "OK";
        }

        private AppCompatibility CheckCompatibility(UserAgent client)
        {
            if ((DevicePlatform)client.DP == DevicePlatform.iOS)
            {
                if (NeeoUtility.GetIntegerValue(client.AppVer) >=
                    NeeoUtility.GetIntegerValue(ConfigurationManager.AppSettings[NeeoConstants.IosCriticalVersion]))
                {
                    return AppCompatibility.Compatible;
                }
                return AppCompatibility.Incompatible;
            }
            if ((DevicePlatform)client.DP == DevicePlatform.Android)
            {
                if (NeeoUtility.GetIntegerValue(client.AppVer) < NeeoUtility.GetIntegerValue(ConfigurationManager.AppSettings[NeeoConstants.AndroidFeatureVersion]))
                {
                    return AppCompatibility.Compatible;
                }
                if (NeeoUtility.GetIntegerValue(client.AppVer) >=
                    NeeoUtility.GetIntegerValue(ConfigurationManager.AppSettings[NeeoConstants.AndroidCriticalVersion]))
                {
                    return AppCompatibility.Compatible;
                }
                return AppCompatibility.Incompatible;
            }
            if ((DevicePlatform)client.DP == DevicePlatform.WindowsMobile)
            {
                if (NeeoUtility.GetIntegerValue(client.AppVer) >=
                     NeeoUtility.GetIntegerValue(ConfigurationManager.AppSettings[NeeoConstants.WPCriticalVersion]))
                {
                    return AppCompatibility.Compatible;
                }
                return AppCompatibility.Incompatible;
            }
            return AppCompatibility.Incompatible;
        }

        /// <summary>
        /// Insert a log record for user registeration request
        /// </summary>
        /// <param name="username">A string containing the user id.</param>
        /// <param name="latitude">A float contain latitude value.</param>
        /// <param name="longitude">A float contain longitude value.</param>
        /// <returns>Returns true on successful insertion.</returns>
        public bool InsertUserRegisterationRequestsLog(string username, float latitude, float longitude)
        {
            return NeeoActivation.InsertMembersRequestsLog(username, latitude, longitude);
        }

        #endregion

        #region Country Detection

        /// <summary>
        /// Gets the country of the requesting user 
        /// </summary>
        /// <returns>A string containing the country code (e.g. PK)</returns>
        public string GetCountry()
        {
            var cip = HttpContext.Current.Request.UserHostAddress;
            var requestMessageProperties = OperationContext.Current.IncomingMessageProperties;
            var remoteEndpointMessageProperty = requestMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            if (remoteEndpointMessageProperty != null)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().GetType(), "Ip address:" + remoteEndpointMessageProperty.Address);
                try
                {
                    return GeoLocator.GetLocation(remoteEndpointMessageProperty.Address);
                }
                catch (ApplicationException appExp)
                {
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                    return null;
                }
            }
            else
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().GetType(), "Endpoint message property is null.");
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.ServerInternalError);
                return null;
            }
        }


        #endregion

        #region Service Internal Methods

        /// <summary>
        /// Seting custome status code in service response header
        /// </summary>
        /// <param name="code">An enumeration representing custome status code.</param>
        //protected void SetServiceResponseHeaders(CustomHttpStatusCode code)
        //{
        //    OutgoingWebResponseContext response = WebOperationContext.Current.OutgoingResponse;
        //    response.StatusCode = (System.Net.HttpStatusCode)code;
        //    if (NeeoDictionaries.HttpStatusCodeDescriptionDictionary.ContainsKey((int)code))
        //        response.StatusDescription = NeeoDictionaries.HttpStatusCodeDescriptionDictionary[(int)code];
        //}

        #endregion

        #region User Verification

        /// <summary>
        /// Verify user account information based on user's information hash.
        /// </summary>
        /// <param name="uID">string containing the user id.</param>
        /// <param name="hash">string containing the user's information hash.</param>
        /// <returns>Returns true if hash codes are same else returns false.</returns>
        public bool VerifyUser(string uID, string hash)
        {
            uID = (uID != null) ? uID.Trim() : uID;
            #region log user request and response

            /***********************************************
             To log user request and response
             ***********************************************/
            if (_logRequestResponse)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, " ===> " +
                    "Request ===> userID : " + uID + ", hash : " + hash, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            #endregion

            // #region Verify User
            //var request = OperationContext.Current.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            //string keyFromClient = request.Headers["key"];
            //if (NeeoUtility.AuthenticateUserRequest(uID, keyFromClient))
            //{
            // #endregion
            try
            {
                if (!(NeeoUtility.IsNullOrEmpty(uID) && NeeoUtility.IsNullOrEmpty(hash)))
                {
                    NeeoUser user = new NeeoUser(uID.Trim());
                    return user.VerifyUser(hash);
                }
                return false;
            }
            catch (ApplicationException appExp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, " ===> " +
                    "Request ===> userID : " + uID + ", hash : " + hash + " ===> " + appExp.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
                if (appExp.Message == CustomHttpStatusCode.InvalidUser.ToString("D"))
                {
                    return false;
                }
                NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)(Convert.ToInt32(appExp.Message)));
                return false;
            }
            catch (Exception exp)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, " ===> " +
                    "Request ===> userID : " + uID + ", hash : " + hash + " ===> " + exp.Message, exp, System.Reflection.MethodBase.GetCurrentMethod().Name);
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.ServerInternalError);
                return false;
            }
            // }
            //else
            //{
            //    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)HttpStatusCode.Unauthorized);
            //    return false;
            //}

        }

        #endregion

        #endregion
    }
}