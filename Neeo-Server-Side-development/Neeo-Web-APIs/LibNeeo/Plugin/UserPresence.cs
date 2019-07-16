using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Common;
using Logger;
using RestSharp;

namespace LibNeeo.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    public enum Presence : ushort
    {
        Online = 1,
        Offline = 2,
        NotAvailable = 3
    }

    public static class UserPresence
    {
        private static string _baseUrl;
        private const string ServiceUri = "plugins/presence/status?jid={jid}&type=xml";

        public static Presence GetUserPresence(string jid)
        {
            string requestUri;
            
            requestUri = ServiceUri;

            RestRequest request = new RestRequest(requestUri);
            request.Method = Method.GET;
            request.AddUrlSegment("jid", jid);
            return ExecuteRequest(request);

        }

        private static Presence ExecuteRequest(RestRequest request)
        {
            const string typeUnavailable = "unavailable";
            const string typeError = "error";
            if (_baseUrl == null)
            {
                _baseUrl = ConfigurationManager.AppSettings[NeeoConstants.XmppBaseUrl].ToString();
            }
            RestClient client = new RestClient(_baseUrl);
            var response = client.Execute(request);
            if (response.ErrorException == null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response.Content);
                switch (doc.DocumentElement.Attributes[0].Value)
                {
                    case typeUnavailable:
                        return Presence.Offline;
                    case typeError:
                        LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name , doc.DocumentElement.InnerText);
                        return Presence.NotAvailable;
                        break;
                    default:
                        return Presence.Online;
                }
            }
            else
            {
                //log response.ErrorMessage
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, response.ErrorMessage);
                throw new ApplicationException(CustomHttpStatusCode.ServerConnectionError.ToString("D"));
            }
        }
    }
}
