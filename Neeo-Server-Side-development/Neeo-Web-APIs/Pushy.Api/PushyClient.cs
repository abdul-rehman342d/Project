using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Logger;
using Newtonsoft.Json;
using Pushy.Api.Entities;

namespace Pushy.Api
{
    public static class PushyClient
    {
        private const string PushyApiKey = "pushyApiKey";
        private const string BaseUrl = "https://pushy.me/push?api_key=";
        private static readonly string ApiKey = ConfigurationManager.AppSettings[PushyApiKey];

        public static void SendPush(PushyPushRequest push)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetApiUrl());
            request.ContentType = "application/json";
            request.Method = "POST";
            string postData = JsonConvert.SerializeObject(push);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException exc)
            {
                string errorJSON = new StreamReader(exc.Response.GetResponseStream()).ReadToEnd();
                PushyAPIError error = JsonConvert.DeserializeObject<PushyAPIError>(errorJSON);
                throw new Exception(error.error);
            }
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string responseData = reader.ReadToEnd();
            LogManager.CurrentInstance.InfoLogger.LogInfo(
                                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                                "Request : "+JsonConvert.SerializeObject(push) + ", Status :" + responseData, System.Reflection.MethodBase.GetCurrentMethod().Name);
            reader.Close();
            response.Close();
            dataStream.Close();
        }

        private static string GetApiUrl()
        {
            return BaseUrl + ApiKey;
        }
    }
}
