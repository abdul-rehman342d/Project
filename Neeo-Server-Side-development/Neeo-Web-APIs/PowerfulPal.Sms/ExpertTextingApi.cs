using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Common;
using Logger;
using PowerfulPal.Sms.com.experttexting.www;

namespace PowerfulPal.Sms
{
    internal class ExpertTextingApi : SmsApi
    {
        /// <summary>
        /// Key for getting api user id.
        /// </summary>
        private const string ExpertUserId = "expertUserId";
        /// <summary>
        /// Key for getting api password.
        /// </summary>
        private const string ExpertPassword = "expertPassword";
        /// <summary>
        /// Key for getting api key.
        /// </summary>
        private const string ExpertApiKey = "expertApiKey";
        /// <summary>
        /// Specifies api user id.
        /// </summary>
        private readonly string _userId = ConfigurationManager.AppSettings[ExpertUserId];
        /// <summary>
        /// Specifies api password.
        /// </summary>
        private readonly string _password = ConfigurationManager.AppSettings[ExpertPassword];
        /// <summary>
        /// Specifies api api key.
        /// </summary>
        private readonly string _apiKey = ConfigurationManager.AppSettings[ExpertApiKey];
        /// <summary>
        /// Specifies Successor Sms Api.
        /// </summary>
        internal SmsApi SuccessorSmsApi;

        /// <summary>
        /// Sends SMS to the given number.
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number for sending SMS.</param>
        /// <param name="msgBody">A string containing message body text.</param>
        /// <param name="isUnicode">if true, message body contains unicode text; otherwise false.(Optional)</param>
        public override void SendSms(string[] phoneNumber, string msgBody, bool isUnicode = false)
        {
            const string success = "SUCCESS";
            var exptTextingApi = new ExptTextingAPI();
            XmlNode response;
            if (isUnicode)
            {
                response = exptTextingApi.SendMultilingualSMS(_userId, _password, _apiKey, NeeoConstants.AppName, phoneNumber[0], msgBody);
            }
            else
            {
                response = exptTextingApi.SendSMS(_userId, _password, _apiKey, NeeoConstants.AppName, phoneNumber[0], msgBody);
            }
            if (response.ChildNodes[0].InnerText != success)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Expert-Texting-Api - Phone # : \"" + phoneNumber[0] + "\" - " + response.ChildNodes[0].InnerText);
                throw new ApplicationException(CustomHttpStatusCode.SmsApiException.ToString("D"));
            }
        }
    }
}
