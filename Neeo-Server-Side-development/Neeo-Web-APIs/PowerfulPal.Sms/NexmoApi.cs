using System;
using System.Configuration;
using System.IO;
using System.Net;
using Common;
using Logger;
using Neeo.Nexmo;


namespace PowerfulPal.Sms
{
    internal class NexmoApi : SmsApi, IVoice
    {
        /// <summary>
        /// Key for getting Nexmo account api key used in sms sending process.
        /// </summary>
        private const string NexmoApiKey = "nexmoApiKey";

        /// <summary>
        /// Key for getting Nexmo account api secret code used in sms sending process.
        /// </summary>
        private const string NexmoApiSecret = "nexmoApiSecret";

        /// <summary>
        /// Key for getting Nexmo phone number used.
        /// </summary>
        private const string NexmoPhoneNumber = "nexmoPhoneNumber";

        /// <summary>
        /// Key for getting Nexmo SSML url.
        /// </summary>
        private const string NexmoSsmlUrl = "nexmoSsmlUrl";

        /// <summary>
        /// Key for getting Nexmo application id.
        /// </summary>
        private const string NexmoAppId = "nexmoAppId";

        /// <summary>
        /// Key for getting Nexmo application key path.
        /// </summary>
        private const string NexmoAppKeyPath = "nexmoAppKeyPath";

        /// <summary>
        /// Specifies the successor api.
        /// </summary>
        public IVoice SuccessorApi { get; set; }

        /// <summary>
        /// Specifies the Nexmo api key.
        /// </summary>
        private readonly string _nexmoApiKey = ConfigurationManager.AppSettings[NexmoApiKey];

        /// <summary>
        /// Specifies the Nexmo api secret.
        /// </summary>
        private readonly string _nexmoApiSecret = ConfigurationManager.AppSettings[NexmoApiSecret];

        /// <summary>
        /// Specifies the Nexmo phone number.
        /// </summary>
        private readonly string _nexmoPhoneNumber = ConfigurationManager.AppSettings[NexmoPhoneNumber];

        /// <summary>
        /// Specifies the Nexmo SSML url.
        /// </summary>
        private readonly string _nexmoSsmlUrl = ConfigurationManager.AppSettings[NexmoSsmlUrl];

        /// <summary>
        /// Specifies the Nexmo app id.
        /// </summary>
        private readonly string _nexmoAppId = ConfigurationManager.AppSettings[NexmoAppId];

        /// <summary>
        /// Specifies the Nexmo app key path.
        /// </summary>
        private readonly string _nexmoAppKeyPath = ConfigurationManager.AppSettings[NexmoAppKeyPath];

        /// <summary>
        /// Call to the given number.
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number for sending SMS.</param>
        /// <param name="code">A string containing code.</param>
        public void Call(string phoneNumber, string code)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var Client = new Nexmo.Api.Client(creds: new Nexmo.Api.Request.Credentials
            {
                ApiKey = _nexmoApiKey,
                ApiSecret = _nexmoApiSecret,
                ApplicationId = _nexmoAppId,
                ApplicationKey = File.ReadAllText(_nexmoAppKeyPath)
            });

            var results = Client.Call.Do(new Nexmo.Api.Voice.Call.CallCommand
            {
                to = new[]
                {
                    new Nexmo.Api.Voice.Call.Endpoint {
                        type = "phone",
                        number = phoneNumber
                    }
                },
                from = new Nexmo.Api.Voice.Call.Endpoint
                {
                    type = "phone",
                    number = _nexmoPhoneNumber
                },

                answer_url = new[]
                {
                    _nexmoSsmlUrl.Replace("{{code}}", code)
                }
            });

            LogManager.CurrentInstance.InfoLogger.LogInfo(
                       System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Nexmo voice call - Phone # : \"" + phoneNumber[0] + "\"  is completed with UUId: " + results.uuid);
        }

        /// <summary>
        /// Sends SMS to the given number.
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number for sending SMS.</param>
        /// <param name="msgBody">A string containing message body text.</param>
        /// <param name="isUnicode">if true, message body contains unicode text; otherwise false.(Optional)</param>
        /// <returns>true if SMS is successfully sent; otherwise, false.</returns>
        public override void SendSms(string[] phoneNumber, string msgBody, bool isUnicode = false)
        {
            NexmoClient nexmoClient = new NexmoClient(_nexmoApiKey, _nexmoApiSecret);
            NexmoResponse smsResponse = nexmoClient.SendMessage(phoneNumber[0], NeeoConstants.AppName, msgBody, isUnicode);
            MessageStatus messageStatus = (MessageStatus)Convert.ToUInt16(smsResponse.Messages[0].Status);

            switch (messageStatus)
            {
                case MessageStatus.InvalidMessage:
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Nexmo - Phone # : " + phoneNumber[0] + " Status : " + messageStatus.ToString() + ", Description : " + NexmoDictionaries.MessageStatusDescriptionDictionary[(short)messageStatus]);

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
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Nexmo - Phone # : " + phoneNumber[0] + " Status : " + messageStatus.ToString() + ", Description : " + NexmoDictionaries.MessageStatusDescriptionDictionary[(short)messageStatus]);

                    throw new ApplicationException(CustomHttpStatusCode.SmsApiException.ToString("D"));
                    break;
            }
        }
    }
}
