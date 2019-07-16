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
using LibNeeo.Plugin;
using Logger;

namespace LibNeeo.Activation
{
    /// <summary>
    /// Activates and manages user on XMPP server.
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
                if (isResend == true)
                {
                    SmsManager.SendSms(phoneNumber, activationCode, isRegenerated);
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
                                    SmsManager.SendSms(phoneNumber, activationCode, isRegenerated);
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

        #endregion

        #region Account Registration

        /// <summary>
        /// Setup user account along with user registration on XMPP server.
        /// </summary>
        /// <param name="phoneNumber">A string containing user's phone number.</param>
        /// <param name="devicePlatform">An integer specifying user's platform</param>
        /// <param name="deviceVenderID">A string containing device specific vender id.</param>
        /// <returns>true if account is successfully setup; otherwise, false.</returns>
        public static bool SetupUserWithRegistration(string phoneNumber, DevicePlatform devicePlatform, string deviceVenderID, string applicationID)
        {
            bool isAccountCreated = false;
            bool isDirectoryStructureCreated = false;
            DbManager dbManager = new DbManager();

            if (!dbManager.CheckUserAlreadyRegistered(phoneNumber))
            {
                try
                {
                    if (dbManager.StartTransaction())
                    {

                        dbManager.DeleteUserFromBlockList(phoneNumber);
                        User.CreateUserDirectoryStructure(phoneNumber);
                        isDirectoryStructureCreated = true;
                        RegisterUserOnXmpp(phoneNumber, deviceVenderID, applicationID);
                        isAccountCreated = true;
                        dbManager.UpdateUserDeviceInfo(phoneNumber, deviceVenderID, devicePlatform);
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
                    if (isDirectoryStructureCreated)
                    {
                        User.DeleteUserDirectoryStructure(phoneNumber);
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
                    throw new ApplicationException(CustomHttpStatusCode.UnknownError.ToString("D"));
                }

            }
            else
            {

                try
                {
                    if (dbManager.StartTransaction())
                    {
                        dbManager.DeleteUserFromBlockList(phoneNumber);
                        dbManager.UpdateUserDeviceInfo(phoneNumber, deviceVenderID, devicePlatform);
                        ChangeUserDeviceKey(phoneNumber, deviceVenderID, applicationID);
                        dbManager.CommitTransaction();
                        User.DeleteFile(phoneNumber,FileClassfication.Profile,NeeoUtility.ConvertToFileName(phoneNumber));
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
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, appExp + " NeeoActivation file - 192");
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
                    throw new ApplicationException(CustomHttpStatusCode.UnknownError.ToString("D"));
                }
            }
        }

        #endregion

        #region XMPP Operations

        /// <summary>
        /// Registers user on XMPP server based on user's phone number using User Service. 
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number.This is used to register user on XMPP server.</param>
        /// <param name="deviceVenderID">A string containing device specific vender id.</param>
        /// <returns>true if user account is successfully registered on XMPP server; otherwise, false.</returns>
        private static void RegisterUserOnXmpp(string phoneNumber, string deviceVenderID, string applicationID)
        {
            string deviceKey = GenerateDeviceKey(phoneNumber, deviceVenderID, applicationID);
            UserService.AddUser(phoneNumber, deviceKey, string.Empty, NeeoUtility.ConvertToJid(phoneNumber));
        }

        /// <summary>
        /// Changes the user account's device key on XMPP server.
        /// </summary>
        /// <param name="userID">A string containing the phone number as user id.</param>
        /// <param name="deviceVenderID">A string containing device specific vender id.</param>
        /// <returns>true if user account's device key is successfully changed on XMPP server; otherwise, false.</returns>
        private static void ChangeUserDeviceKey(string userID, string deviceVenderID, string applicationID)
        {
            string deviceKey = GenerateDeviceKey(userID, deviceVenderID, applicationID);
            UserService.UpdateUser(userID, deviceKey, null, null);
        }

        /// <summary>
        /// Deletes user's account on XMPP server.
        /// </summary>
        /// <param name="userID">A string containing the phone number as user id.</param>
        /// <returns>true if account is successfully deleted; otherwise, false.</returns>
        public static bool DeleteUserOnXmpp(string userID)
        {
            try
            {
                UserService.DeleteUser(userID);
                return true;
            }
            catch (ApplicationException appEx)
            {
                //log
                return false;
            }
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
            StringBuilder sBuilder = new StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashedData = md5.ComputeHash(Encoding.UTF8.GetBytes(hashingData));

                for (int i = 0; i < hashedData.Length; i++)
                {
                    sBuilder.Append(hashedData[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

        #endregion

        #endregion
    }
}
