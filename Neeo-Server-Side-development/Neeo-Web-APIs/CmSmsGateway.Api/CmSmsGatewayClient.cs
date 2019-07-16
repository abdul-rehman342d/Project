using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CmSmsGateway.Api
{
    public class CmSmsGatewayClient
    {
        private const string ApiBaseUrl = "https://secure.cm.nl/smssgateway/cm/gateway.ashx";
        private string _productionToken;

        public CmSmsGatewayClient(string productionToken)
        {
            _productionToken = productionToken;
        }

        public string SendSms(string recipient, string sender, string message, ContentType contentType)
        {
            CmMessage cmMessage = new CmMessage()
            {
                Id = Guid.NewGuid().ToString("N"),
                From = sender,
                To = recipient,
                Body = message,
                ContentType = contentType,
                MinimumNumberofMessageParts = 1,
                MaximumNumberofMessageParts = 8
            };
            string requestContent = CmXmlGenerator.Generate(_productionToken, cmMessage);
            try
            {
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                webClient.Headers.Add("Content-Type", "application/xml;charset=utf-8");
                return webClient.UploadString(ApiBaseUrl, requestContent);
            }
            catch (WebException wex)
            {
                return string.Format("{0} - {1}", wex.Status, wex.Message);
            }
        }

        public string SendSms(string toNumber, string from, string message)
        {
            return SendSms(toNumber, from, message, ContentType.Text);
        }

        

    }
}
