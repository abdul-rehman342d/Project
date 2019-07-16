using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Logger;
using CmSmsGateway.Api;

namespace PowerfulPal.Sms
{
    public class CmSmsGatewayApi : SmsApi
    {
        /// <summary>
        /// Key for getting api production token.
        /// </summary>
        private const string ProductionToken = "cmProductionToken";
        /// <summary>
        /// Specifies api production token.
        /// </summary>
        private readonly string _productionToken = ConfigurationManager.AppSettings[ProductionToken];
        /// <summary>
        /// Specifies Successor Sms Api.
        /// </summary>
        public SmsApi SuccessorSmsApi;
        /// <summary>
        /// Sends SMS to the given number.
        /// </summary>
        /// <param name="phoneNumber">A string containing the phone number for sending SMS.</param>
        /// <param name="msgBody">A string containing message body text.</param>
        /// <param name="isUnicode">if true, message body contains unicode text; otherwise false.(Optional)</param>
        public override void SendSms(string[] phoneNumber, string msgBody, bool isUnicode = false)
        {
            CmSmsGatewayClient cmSmsGatewayClient = new CmSmsGatewayClient(_productionToken);
            string result = cmSmsGatewayClient.SendSms(phoneNumber[0], NeeoConstants.AppName, msgBody,
                isUnicode ? ContentType.UnicodeText : ContentType.Text);
            if (result != string.Empty)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "CmSmsGateway - Phone # : \"" + phoneNumber[0] + "\" - " + result);
                throw new ApplicationException(CustomHttpStatusCode.SmsApiException.ToString("D"));   
            }
        }
    }
}
