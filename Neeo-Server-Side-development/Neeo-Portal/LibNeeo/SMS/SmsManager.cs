using System;
using System.Drawing.Text;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.ComponentModel;
using System.Configuration;
using Twilio;
using Common;
using Logger;
using Nexmo;

namespace LibNeeo
{
    /// <summary>
    /// Handles SMS sending to the user's phone number.
    /// </summary>
    public static class SmsManager
    {
        #region Data Members

        /// <summary>
        /// Specifies the twilio accound id.
        /// </summary>
        private static string _twilioAccountSid;

        /// <summary>
        /// Specifies the twilio authentication token.
        /// </summary>
        private static string _twilioAuthToken;

        /// <summary>
        /// Specifies the tvilio phone number.
        /// </summary>
        private static string _twilioPhoneNumber;

        /// <summary>
        /// Specifies the tvilio phone number.
        /// </summary>
        private static string _nexmoApiKey;

        /// <summary>
        /// Specifies the tvilio phone number.
        /// </summary>
        private static string _nexmoApiSecret;

        /// <summary>
        /// Specifies the activation code mask.
        /// </summary>
        private static string _activationCodeMask;

        /// <summary>
        /// Specifies the sms body.
        /// </summary>
        private static string _smsBody;

        #endregion

        #region Member Functions

        /// <summary>
        /// Sends SMS to the given number.
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number for sending SMS.</param>
        /// <param name="activationCode">A string containing the activation code.</param>
        /// <param name="isRegenerated">A bool if true specifying that the request is for sending regenerated activation code; otherwise false.</param>
        /// <remarks><paramref name="isRegenerated"/> is mainly used to switch between the sms sending APIs. For the first time ,send code to user with primary API i.e. Nexmo and send the regenerated code with the secondary API i.e. Tiwilio.</remarks>
        /// <returns>true if SMS is successfully sent; otherwise, false.</returns>
        public static void SendSms(string phoneNumber, string activationCode, bool isRegenerated)
        {
            if (_activationCodeMask == null)
            {
                _activationCodeMask = ConfigurationManager.AppSettings[NeeoConstants.ActivationCodeMask].ToString();
            }

            if (_smsBody == null)
            {
                _smsBody = ConfigurationManager.AppSettings[NeeoConstants.SmsBody].ToString();
                _smsBody = _smsBody.Replace(".", "." + Environment.NewLine);
            }

            if (!isRegenerated)
            {
                SendThroughPrimaryApi(NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber), activationCode);
            }
            else
            {
                SendThroughSecondaryApi(NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber), activationCode);
            }
        }

        private static void SendThroughPrimaryApi(string phoneNumber, string activationCode)
        {
            if (_nexmoApiKey == null)
            {
                _nexmoApiKey = ConfigurationManager.AppSettings[NeeoConstants.NexmoApiKey].ToString();
            }

            if (_nexmoApiSecret == null)
            {
                _nexmoApiSecret = ConfigurationManager.AppSettings[NeeoConstants.NexmoApiSecret].ToString();
            }

            NexmoClient nexmoClient = new NexmoClient(_nexmoApiKey, _nexmoApiSecret);
            SmsResponse smsResponse = nexmoClient.SendMessage(phoneNumber, NeeoConstants.AppName,
                _smsBody.Replace(_activationCodeMask, activationCode));
            MessageStatus messageStatus = (MessageStatus)Convert.ToUInt16(smsResponse.Messages[0].Status);

            switch (messageStatus)
            {
                case MessageStatus.InvalidMessage:
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Nexmo - Phone # : " + phoneNumber + " Status : " + messageStatus.ToString() + ", Description : " + NexmoDictionaries.MessageStatusDescriptionDictionary[(short)messageStatus]);
                    throw new ApplicationException(CustomHttpStatusCode.InvalidNumber.ToString("D"));
                    break;

                case MessageStatus.Throttled:
                case MessageStatus.MissingParams:
                case MessageStatus.InvalidParams:
                case MessageStatus.InvalidCredentials:
                case MessageStatus.InternalError:
                case MessageStatus.NumberBarred:
                case MessageStatus.PartnerAccountBarred:
                case MessageStatus.PartnerQuotaExceeded:
                case MessageStatus.CommunicationFailed:
                case MessageStatus.InvalidSignature:
                case MessageStatus.InvalidSenderAddress:
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Nexmo - Status : " + messageStatus.ToString() + ", Description : " + NexmoDictionaries.MessageStatusDescriptionDictionary[(short)messageStatus]);
                    SendThroughSecondaryApi(phoneNumber, activationCode);
                    break;
            }
        }

        private static void SendThroughSecondaryApi(string phoneNumber, string activationCode)
        {
            if (_twilioAccountSid == null)
            {
                _twilioAccountSid = ConfigurationManager.AppSettings[NeeoConstants.TwilioAccountSid].ToString();
            }

            if (_twilioAuthToken == null)
            {
                _twilioAuthToken = ConfigurationManager.AppSettings[NeeoConstants.TwilioAuthToken].ToString();
            }

            if (_twilioPhoneNumber == null)
            {
                _twilioPhoneNumber = ConfigurationManager.AppSettings[NeeoConstants.TwilioPhoneNumber].ToString();
            }

            TwilioRestClient twilioClient = new TwilioRestClient(_twilioAccountSid, _twilioAuthToken);
            Twilio.Message message = twilioClient.SendMessage(_twilioPhoneNumber, phoneNumber,
                _smsBody.Replace(_activationCodeMask, activationCode));
            if (message.RestException == null)
            {
                if (message.Status == "failed")
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Twilio - Phone # : \"" + phoneNumber + "\"  is invalid.");
                    throw new ApplicationException(CustomHttpStatusCode.InvalidNumber.ToString("D"));
                }
            }
            else
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Twilio - " + message.RestException.Message);
                throw new ApplicationException(CustomHttpStatusCode.SmsApiException.ToString("D"));
            }
        }

        #endregion
    }
}
