using System;
using System.Configuration;
using Common;
using Logger;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using static Twilio.Rest.Api.V2010.Account.CallResource;

namespace PowerfulPal.Sms
{
    internal class TwilioApi : SmsApi, IVoice
    {
        /// <summary>
        /// Key for getting Twilio account secret id.
        /// </summary>
        private const string TwilioAccountSid = "twilioAccountSid";

        /// <summary>
        /// Key for getting Twilio authentication token.
        /// </summary>
        private const string TwilioAuthToken = "twilioAuthToken";

        /// <summary>
        /// Key for getting Twilio phone number used.
        /// </summary>
        private const string TwilioPhoneNumber = "twilioPhoneNumber";

        /// <summary>
        /// Key for getting Twilio SSML url.
        /// </summary>
        private const string TwilioSsmlUrl = "twilioSsmlUrl";

        /// <summary>
        /// Specifies the successor api.
        /// </summary>
        public IVoice SuccessorApi { get; set; }

        /// <summary>
        /// Specifies the Twilio accound id.
        /// </summary>
        private readonly string _twilioAccountSid = ConfigurationManager.AppSettings[TwilioAccountSid];

        /// <summary>
        /// Specifies the Twilio authentication token.
        /// </summary>
        private readonly string _twilioAuthToken = ConfigurationManager.AppSettings[TwilioAuthToken];

        /// <summary>
        /// Specifies the Twilio phone number.
        /// </summary>
        private readonly string _twilioPhoneNumber = ConfigurationManager.AppSettings[TwilioPhoneNumber];

        /// <summary>
        /// Specifies the Twilio SSML url.
        /// </summary>
        private readonly string _twilioSsmlUrl = ConfigurationManager.AppSettings[TwilioSsmlUrl];

        /// <summary>
        /// Call to the given number.
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number for sending SMS.</param>
        /// <param name="code">A string containing code.</param>
        public void Call(string phoneNumber, string code)
        {
            TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);

            var to = new PhoneNumber(NeeoUtility.FormatAsIntlPhoneNumber(phoneNumber));
            var from = new PhoneNumber(_twilioPhoneNumber);
            var call = Create(to, from,
                url: new Uri(_twilioSsmlUrl.Replace("{{code}}", code)));

            LogManager.CurrentInstance.InfoLogger.LogInfo(
                       System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Twilio voice call - Phone # : \"" + phoneNumber[0] + "\"  is completed with Sid: " + call.Sid);
        }

        /// <summary>
        /// Sends SMS to the given number.
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number for sending SMS.</param>
        /// <param name="msgBody">A string containing message body text.</param>
        /// <param name="isUnicode">if true, message body contains unicode text; otherwise false.(Optional)</param>
        public override void SendSms(string[] phoneNumber, string msgBody, bool isUnicode = false)
        {
            TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);
            MessageResource message = MessageResource.Create(from: _twilioPhoneNumber, to: phoneNumber[0], body: msgBody);
            if (message != null)
            {
                if (message.Status.ToString() == StatusEnum.Completed.ToString().ToLower() || message.Status.ToString() == StatusEnum.Queued.ToString().ToLower())
                {
                    LogManager.CurrentInstance.InfoLogger.LogInfo(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Twilio - Phone # : \"" + phoneNumber[0] + "\"  is completed with Sid: " + message.Sid);
                }
                else if (message.Status.ToString() == StatusEnum.Failed.ToString().ToLower())
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Twilio - Phone # : \"" + phoneNumber[0] + "\"  is invalid.  Error: " + message.ErrorMessage);
                    throw new ApplicationException(CustomHttpStatusCode.InvalidNumber.ToString("D"));
                }
                else
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Twilio - Phone # : \"" + phoneNumber[0] + "\" - Status: " + message.Status + "\" - Error: " + message.ErrorMessage);
                    throw new ApplicationException(CustomHttpStatusCode.SmsApiException.ToString("D"));
                }
            }
            else
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Twilio - Phone # : \"" + phoneNumber[0] + "\" - Api failed to response");
                throw new ApplicationException(CustomHttpStatusCode.SmsApiException.ToString("D"));
            }
        }
    }
}
