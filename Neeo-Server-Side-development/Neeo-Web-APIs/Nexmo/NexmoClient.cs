using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Util;
using Newtonsoft.Json;


namespace Neeo.Nexmo
{
    public class NexmoClient
    {
        private const string NexmoApiUrl = "https://rest.nexmo.com/sms/json";
        private readonly string _nexmoApiKey;
        private readonly string _nexmoApiSecret;

        public NexmoClient(string apiKey, string apiSecret)
        {
            _nexmoApiKey = apiKey;
            _nexmoApiSecret = apiSecret;
        }

        public NexmoResponse SendMessage(string recipient, string sender, string message, bool isUnicode = false)
        {
            NexmoRequest request = new NexmoRequest()
            {
                ApiKey = _nexmoApiKey,
                ApiSecret = _nexmoApiSecret,
                From = sender,
                To = recipient,
                Text = message,
                Type = isUnicode ? "unicode" : ""
            };
            request.WithUnicode(isUnicode);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string requestBody = JsonConvert.SerializeObject(request);
            WebClient webClient = new WebClient()
            {
                Encoding = Encoding.UTF8
            };
            webClient.Headers.Add("Content-Type", "application/json;charset=utf-8");
            return this.ParseSmsResponseJson(webClient.UploadString(NexmoApiUrl, requestBody));
        }

        private NexmoResponse ParseSmsResponseJson(string json)
        {
            // hyphens are not allowed in in .NET var names
            json = json.Replace("message-count", "MessageCount");
            json = json.Replace("message-price", "MessagePrice");
            json = json.Replace("message-id", "MessageId");
            json = json.Replace("remaining-balance", "RemainingBalance");
            return JsonConvert.DeserializeObject<NexmoResponse>(json);
        }
    }
}
