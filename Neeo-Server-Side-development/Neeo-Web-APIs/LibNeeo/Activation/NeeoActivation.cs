using System;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web;
using System.ComponentModel;
using System.Configuration;
using System.Transactions;
using System.Security.Cryptography;
using System.Text;
using Common;
using DAL;
using LibNeeo.IO;
using LibNeeo.Plugin;
using LibNeeo.Voip;
using Logger;
using PowerfulPal.Sms;
using RestSharp.Extensions;

namespace LibNeeo.Activation
{
    /// <summary>
    /// Activates and manages user accounts on XMPP server.
    /// </summary>
    public static class NeeoActivation
    {
        #region Member Functions

        #region SMS Sending

        /// <summary>
        /// Sends messages to the users who are not blocked.
        /// </summary>
        /// <remarks>It manipulates the user requests whether it is resending activation code request or not, block or unblock the user based on maximum allowed request in 24 hrs and then sends sms to the users who are not blocked.</remarks>
        /// <param name="phoneNumber">A string containing the user's phone number.</param>
        /// <param name="devicePlatform">An enum specifying the device platform.</param>
        /// <param name="activationCode">A string containing activation code to be sent.</param>
        /// <param name="isResend">A bool if true specifying that the request is for resending activation code; otherwise false.</param>
        /// <param name="isRegenerated">A bool if true specifying that the request is for sending regenerated activation code; otherwise false.</param>
        /// <returns>An integer if 0 = sms has not been sent, -1 = sms has been successfully sent or number of remaining minutes to unblock user = user is blocked based on blocking policy. </returns>
        public static int SendActivationCodeToUnBlockedUser(string phoneNumber, DevicePlatform devicePlatform, string activationCode, bool isResend, bool isRegenerated)
        {
            int smsSendingResult = (int)SmsSendingStatus.SendingFailed;
            if (devicePlatform == DevicePlatform.iOS || devicePlatform == DevicePlatform.Android)
            {
                if (isResend)
                {
                    //SmsManager.SendActivationCode(phoneNumber, activationCode, isRegenerated);
                    PowerfulPal.Sms.SmsManager.GetInstance().Twilio.SendSms(new[] { NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber) }, NeeoUtility.GetActivationMessage(activationCode));
                    smsSendingResult = (int)SmsSendingStatus.Sent;
                    return smsSendingResult;
                }
                else
                {
                    DbManager dbManager = new DbManager();
                    if (dbManager.StartTransaction())
                    {
                        try
                        {
                            int userBlockedState = dbManager.GetUserBlockedState(phoneNumber);
                            switch ((UserState)userBlockedState)
                            {
                                case UserState.NotBlocked:
                                    PowerfulPal.Sms.SmsManager.GetInstance().SendSms(new[] { NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber) }, NeeoUtility.GetActivationMessage(activationCode));
                                    smsSendingResult = (int)SmsSendingStatus.Sent;
                                    break;

                                default:
                                    smsSendingResult = userBlockedState;
                                    break;
                            }

                            dbManager.CommitTransaction();
                            return smsSendingResult;
                        }
                        catch (ApplicationException appExp)
                        {
                            dbManager.RollbackTransaction();
                            throw;
                        }
                        catch (Exception exp)
                        {
                            dbManager.RollbackTransaction();
                            LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                            throw new ApplicationException(CustomHttpStatusCode.UnknownError.ToString("D"));
                        }
                    }
                    else
                    {
                        throw new ApplicationException(CustomHttpStatusCode.TransactionFailure.ToString("D"));
                        return smsSendingResult;
                    }
                }
            }
            else
            {
                // handling for secondary devices.
                return smsSendingResult;
            }
        }

        /// <summary>
        /// Sends activation code to phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="devicePlatform"></param>
        /// <param name="activationCode"></param>
        /// <param name="appKey">A string containing the appKey(For Android).</param>
        /// <returns></returns>
        public static int SendActivationCode(string phoneNumber, DevicePlatform devicePlatform, string activationCode, string appKey,bool logSMS)
        {
            int smsSendingResult = (int)SmsSendingStatus.SendingFailed;
            string smsSendingSource = ConfigurationManager.AppSettings[NeeoConstants.SmsSendingSource]?? "default";
            string messageBody = NeeoUtility.GetActivationMessage(activationCode, appKey);
            if (Enum.IsDefined(typeof(DevicePlatform), devicePlatform))
            {
                var dbManager = new DbManager();
                if (dbManager.StartTransaction())
                {
                    try
                    {
                        var userAttemptsDetails = dbManager.GetUserAttemptsCount(phoneNumber, activationCode);
                        switch ((UserState)userAttemptsDetails["blockedState"])
                        {
                            case UserState.NotBlocked:
                                if (smsSendingSource.Trim() == "primary")
                                {
                                    PowerfulPal.Sms.SmsManager.GetInstance().PrimaryApi.SendSms(new[] { NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber) }, messageBody);
                                    smsSendingResult = (int)SmsSendingStatus.Sent;
                                }
                                else if (smsSendingSource.Trim() == "secondary")
                                {
                                    PowerfulPal.Sms.SmsManager.GetInstance().SecondaryApi.SendSms(new[] { NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber) }, messageBody);
                                    smsSendingResult = (int)SmsSendingStatus.Sent;
                                }
                                else if (userAttemptsDetails["attemptsCount"] % 2 == 0)
                                {
                                    PowerfulPal.Sms.SmsManager.GetInstance().SecondaryApi.SendSms(new[] { NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber) }, messageBody);
                                    smsSendingResult = (int)SmsSendingStatus.Sent;
                                }
                                else
                                {
                                    PowerfulPal.Sms.SmsManager.GetInstance().SendSms(new[] { NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber) }, messageBody);
                                    smsSendingResult = (int)SmsSendingStatus.Sent;
                                }
                                break;

                            default:
                                smsSendingResult = userAttemptsDetails["blockedState"];
                                break;
                        }

                        dbManager.CommitTransaction();
                        return smsSendingResult;
                    }
                    catch (ApplicationException appExp)
                    {
                        dbManager.RollbackTransaction();
                        throw;
                    }
                    catch (Exception exp)
                    {
                        dbManager.RollbackTransaction();
                        LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                        throw new ApplicationException(CustomHttpStatusCode.UnknownError.ToString("D"));
                    }
                    finally
                    {
                        if (logSMS)
                        {
                            DbManager _dbManager = new DbManager();
                            _dbManager.InsertSMSLog("-1", phoneNumber, messageBody, false, false, 1, appKey, smsSendingResult.ToString());
                        }
                    }
                }
                else
                {
                    throw new ApplicationException(CustomHttpStatusCode.TransactionFailure.ToString("D"));
                }
            }
            else
            {
                // handling for secondary devices.
                return smsSendingResult;
            }
        }

        /// <summary>
        /// Calls phone number for activation code
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="devicePlatform"></param>
        /// <param name="activationCode"></param>
        /// <returns></returns>
        public static int CallForActivationCode(string phoneNumber, DevicePlatform devicePlatform, string activationCode)
        {
            int callStatus = (int)CallStatus.CallFailed;

            if (Enum.IsDefined(typeof(DevicePlatform), devicePlatform))
            {
                var dbManager = new DbManager();

                if (dbManager.StartTransaction())
                {
                    try
                    {
                        var userAttemptsDetails = dbManager.GetUserAttemptsCount(phoneNumber, activationCode);
                        switch ((UserState)userAttemptsDetails["blockedState"])
                        {
                            case UserState.NotBlocked:
                                if (userAttemptsDetails["attemptsCount"] % 2 == 0)
                                {
                                    VoiceManager.GetInstance().SecondaryApi.Call(phoneNumber, activationCode);
                                    callStatus = (int)CallStatus.CalledSuccessfully;
                                }
                                else
                                {
                                    VoiceManager.GetInstance().Call(phoneNumber, activationCode);
                                    callStatus = (int)CallStatus.CalledSuccessfully;
                                }

                                break;

                            default:
                                callStatus = userAttemptsDetails["blockedState"];

                                break;
                        }

                        dbManager.CommitTransaction();
                        return callStatus;
                    }
                    catch (ApplicationException appExp)
                    {
                        dbManager.RollbackTransaction();
                        throw;
                    }
                    catch (Exception exp)
                    {
                        dbManager.RollbackTransaction();
                        LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                        throw new ApplicationException(CustomHttpStatusCode.UnknownError.ToString("D"));
                    }
                }
                else
                {
                    throw new ApplicationException(CustomHttpStatusCode.TransactionFailure.ToString("D"));
                }
            }
            else
            {
                // handling for secondary devices.
                return callStatus;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="devicePlatform"></param>
        /// <param name="activationCode"></param>
        /// <returns></returns>
        public static int SendActivationCodeTest(string phoneNumber, DevicePlatform devicePlatform, string activationCode)
        {
            int smsSendingResult = (int)SmsSendingStatus.SendingFailed;
            string smsSendingSource = ConfigurationManager.AppSettings[NeeoConstants.SmsSendingSource] ?? "default";
            if (Enum.IsDefined(typeof(DevicePlatform), devicePlatform))
            {
                var dbManager = new DbManager();
                if (dbManager.StartTransaction())
                {
                    try
                    {
                        var userAttemptsDetails = dbManager.GetUserAttemptsCount(phoneNumber, activationCode);
                        switch ((UserState)userAttemptsDetails["blockedState"])
                        {
                            case UserState.NotBlocked:
                                if (smsSendingSource.Trim() == "primary")
                                {
                                    PowerfulPal.Sms.SmsManager.GetInstance().SendSms(new[] { NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber) }, activationCode);
                                    smsSendingResult = (int)SmsSendingStatus.Sent;
                                }
                                else if (smsSendingSource.Trim() == "secondary")
                                {
                                    PowerfulPal.Sms.SmsManager.GetInstance().Twilio.SendSms(new[] { NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber) }, activationCode);
                                    smsSendingResult = (int)SmsSendingStatus.Sent;
                                }
                                else if (userAttemptsDetails["attemptsCount"] % 2 == 0)
                                {
                                    PowerfulPal.Sms.SmsManager.GetInstance().Twilio.SendSms(new[] { NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber) }, activationCode);
                                    smsSendingResult = (int)SmsSendingStatus.Sent;
                                }
                                else
                                {
                                    PowerfulPal.Sms.SmsManager.GetInstance().SendSms(new[] { NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber) }, activationCode);
                                    smsSendingResult = (int)SmsSendingStatus.Sent;
                                }
                                break;

                            default:
                                smsSendingResult = userAttemptsDetails["blockedState"];
                                break;
                        }

                        dbManager.CommitTransaction();
                        return smsSendingResult;
                    }
                    catch (ApplicationException appExp)
                    {
                        dbManager.RollbackTransaction();
                        throw;
                    }
                    catch (Exception exp)
                    {
                        dbManager.RollbackTransaction();
                        LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                        throw new ApplicationException(CustomHttpStatusCode.UnknownError.ToString("D"));
                    }
                }
                else
                {
                    throw new ApplicationException(CustomHttpStatusCode.TransactionFailure.ToString("D"));
                    return smsSendingResult;
                }
            }
            else
            {
                // handling for secondary devices.
                return smsSendingResult;
            }
        }

        #endregion

        #region Account Registration

        /// <summary>
        /// Setup user account along with user registration on XMPP server.
        /// </summary>
        /// <param name="phoneNumber">A string containing user's phone number.</param>
        /// <param name="applicationID">An string containing the application id.</param>
        /// <param name="applicationVersion">A string containing application version.</param>
        /// <param name="deviceInfo">An object of DeviceInfo class that contains the device inforamtion.</param>
        /// <returns>true if account is successfully setup; otherwise, false.</returns>
        public static bool SetupUserAccount(string phoneNumber, string applicationID, string applicationVersion, DeviceInfo deviceInfo)
        {
            bool isAccountCreated = false;
            bool isDirectoriesCreated = false;
            DbManager dbManager = new DbManager();
            string deviceKey = GenerateDeviceKey(phoneNumber, deviceInfo.DeviceVenderID, applicationID);
            if (!dbManager.CheckUserAlreadyRegistered(phoneNumber))
            {
                try
                {
                    if (dbManager.StartTransaction())
                    {
                        dbManager.DeleteUserFromBlockList(phoneNumber);
                        FileManager.CreateUserDirectory(phoneNumber);
                        isDirectoriesCreated = true;
                        RegisterUserOnXmpp(phoneNumber, deviceKey);
                        isAccountCreated = true;
                        dbManager.UpdateUserDeviceInfo(phoneNumber, applicationID, applicationVersion, deviceInfo, true, true);
                        //NeeoVoipApi.RegisterUser(phoneNumber, deviceKey, deviceInfo.DeviceToken == null ? PushEnabled.False : PushEnabled.True);
                        NeeoVoipApi.RegisterUser(phoneNumber, deviceKey, PushEnabled.True);
                        dbManager.CommitTransaction();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (ApplicationException appExp)
                {
                    dbManager.RollbackTransaction();
                    if (isAccountCreated)
                    {
                        DeleteUserOnXmpp(phoneNumber);
                    }
                    if (isDirectoriesCreated)
                    {
                        FileManager.DeleteUserDirectory(phoneNumber);
                    }

                    throw;
                }
                catch (System.UnauthorizedAccessException unAuthExp)
                {
                    dbManager.RollbackTransaction();
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, unAuthExp.Message, unAuthExp);
                    throw new ApplicationException(CustomHttpStatusCode.FileSystemException.ToString("D"));
                }
                catch (System.Security.SecurityException secExp)
                {
                    dbManager.RollbackTransaction();
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, secExp.Message, secExp);
                    throw new ApplicationException(CustomHttpStatusCode.FileSystemException.ToString("D"));
                }
                catch (Exception exp)
                {
                    dbManager.RollbackTransaction();
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                    throw new ApplicationException(CustomHttpStatusCode.ServerInternalError.ToString("D"));
                }

            }
            else
            {

                try
                {
                    if (dbManager.StartTransaction())
                    {
                        dbManager.DeleteUserFromBlockList(phoneNumber);
                        dbManager.UpdateUserDeviceInfo(phoneNumber, applicationID, applicationVersion, deviceInfo, true, true);
                        ChangeUserDeviceKey(phoneNumber, deviceKey);
                        NeeoVoipApi.UpdateUserAccount(phoneNumber, deviceKey, deviceInfo.DeviceToken == null ? PushEnabled.False : PushEnabled.True, UserStatus.NotSpecified);
                        dbManager.CommitTransaction();
                        //NeeoUser.DeleteUserAvatar(phoneNumber);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (ApplicationException appExp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, appExp.Message);
                    dbManager.RollbackTransaction();
                    throw;
                }
                catch (System.UnauthorizedAccessException unAuthExp)
                {
                    dbManager.RollbackTransaction();
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, unAuthExp.Message, unAuthExp);
                    throw new ApplicationException(CustomHttpStatusCode.FileSystemException.ToString("D"));
                }
                catch (System.Security.SecurityException secExp)
                {
                    dbManager.RollbackTransaction();
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, secExp.Message, secExp);
                    throw new ApplicationException(CustomHttpStatusCode.FileSystemException.ToString("D"));
                }
                catch (Exception exp)
                {
                    dbManager.RollbackTransaction();
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, exp.Message, exp);
                    throw new ApplicationException(CustomHttpStatusCode.ServerInternalError.ToString("D"));
                }
            }
        }

        /// <summary>
        /// Check the existence of the account based on the user phone number.
        /// </summary>
        /// <param name="phoneNumber">A string containing user's phone number.</param>
        /// <returns>true if account is existed otherwise false.</returns>
        public static bool CheckUserAlreadyRegistered(string phoneNumber)
        {
            DbManager dbManager = new DbManager();
            if (dbManager.CheckUserAlreadyRegistered(phoneNumber))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Checks whether user's registeration request exists or not. 
        /// </summary>
        /// <param name="phoneNumber">A string containing user's phone number.</param>
        /// <returns>true if user request exists otherwise false.</returns>
        public static bool  CheckUserRegisterationRequest(string phoneNumber)
        {
            DbManager dbManager = new DbManager();       
            return dbManager.CheckUserRegisterationRequest(phoneNumber);
        }

        /// <summary>
        /// Insert a log record for user registeration request
        /// </summary>
        /// <param name="username">A string containing the user id.</param>
        /// <param name="latitude">A float contain latitude value.</param>
        /// <param name="longitude">A float contain longitude value.</param>
        /// <returns>Returns true on successful insertion.</returns>
        public static bool InsertMembersRequestsLog(string username, float latitude, float longitude)
        {            
            DbManager dbManager = new DbManager();
            return dbManager.InsertMembersRequestsLog(username, latitude, longitude);         
        }

        /// <summary>
        /// Deletes user account from Neeo.
        /// </summary>
        /// <param name="userID">A string containing the user id.</param>
        public static void DeleteUserAccount(string userID)
        {
            DeleteUserOnXmpp(userID);
            FileManager.DeleteUserDirectory(userID);
        }

        #endregion

        #region XMPP Operations

        /// <summary>
        /// Registers user on XMPP server based on user's phone number using User Service. 
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number.This is used to register user on XMPP server.</param>
        /// <param name="deviceKey">A string containing generated device key for xmpp registration .</param>
        /// <returns>true if user account is successfully registered on XMPP server; otherwise, false.</returns>
        private static void RegisterUserOnXmpp(string phoneNumber, string deviceKey)
        {
            UserService.AddUser(phoneNumber, deviceKey, string.Empty, NeeoUtility.ConvertToJid(phoneNumber));
        }

        /// <summary>
        /// Changes the user account's device key on XMPP server.
        /// </summary>
        /// <param name="userID">A string containing the phone number as user id.</param>
        /// <param name="deviceKey">A string containing generated device key for xmpp registration .</param>
        /// <returns>true if user account's device key is successfully changed on XMPP server; otherwise, false.</returns>
        private static void ChangeUserDeviceKey(string userID, string deviceKey)
        {
            UserService.UpdateUser(userID, deviceKey, string.Empty, null);
        }

        /// <summary>
        /// Deletes user's account on XMPP server.
        /// </summary>
        /// <param name="userID">A string containing the phone number as user id.</param>
        private static void DeleteUserOnXmpp(string userID)
        {
            UserService.DeleteUser(userID);
        }

        #endregion

        #region  Encryption

        /// <summary>
        /// Generates MD5 encrypted device key based on phone number and device vender id.
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number.</param>
        /// <param name="deviceVenderID">A string containing the device specific venderID.</param>
        /// <returns>A hexadecimal string generated from MD5 encryption.</returns>
        private static string GenerateDeviceKey(string phoneNumber, string deviceVenderID, string applicationID)
        {
            string hashingData = phoneNumber + deviceVenderID + applicationID;
            return NeeoUtility.GenerateMd5Hash(hashingData);
        }

        #endregion

        #endregion
    }
}
