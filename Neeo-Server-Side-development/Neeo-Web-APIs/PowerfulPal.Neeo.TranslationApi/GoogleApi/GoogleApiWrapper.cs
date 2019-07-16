using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using RestSharp;

namespace PowerfulPal.Neeo.SpamApi.GoogleApi
{
    public class GoogleApiWrapper
    {
        private readonly string _key;
        private const string ApiBaseUrl = "https://www.googleapis.com/language/translate/v2";

        public GoogleApiWrapper(string key)
        {
            _key = key;
        }

        public string GetTranslatedText(string key, string format, string source, string target, string text)
        {
            var url = ApiBaseUrl + "?key={key}&source={source}&target={target}&q={text}";


            var request = new RestRequest(url);
            request.AddUrlSegment("key", _key);
            request.AddUrlSegment("source", source);
            request.AddUrlSegment("target", target);
            request.AddUrlSegment("text", text);

            if (Common.NeeoUtility.IsNullOrEmpty(format))
            {
                url += "&format={format}";
                request.AddUrlSegment("format", format);
            }

            var client = new RestClient();
            var response = client.Execute(request);
            if (response.ErrorException == null)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseContent = JsonConvert.DeserializeObject<dynamic>(response.Content);
                    return responseContent["data"]["translations"][0]["translatedText"];
                }
                throw new ApplicationException(response.StatusCode + " - " + response.StatusDescription);
            }
            throw new ApplicationException(response.ErrorMessage);
        }

        public Dictionary<string, object> GetTranslatedText(string queryString)
        {
            var url = ApiBaseUrl + queryString;
            var request = new RestRequest(queryString, Method.GET);

            var client = new RestClient(ApiBaseUrl);
            var response = client.Execute(request);
            if (response.ErrorException == null)
            {
                return new Dictionary<string, object>()
                {
                    {"StatusCode", response.StatusCode},
                    {"Result", JsonConvert.DeserializeObject(response.Content)}
                };
            }
            throw new ApplicationException(response.ErrorMessage);
        }
    }
}