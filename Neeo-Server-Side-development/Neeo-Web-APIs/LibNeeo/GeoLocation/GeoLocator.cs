using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using RestSharp;
using System.Configuration;
using Newtonsoft.Json;
using RestSharp.Extensions;
using Logger;

namespace LibNeeo
{
    /// <summary>
    /// Locates the user location based on user's ip address.
    /// </summary>
    public static class GeoLocator
    {
        /// <summary>
        /// Contains the ip locator api url.
        /// </summary>
        private static string _ipLocatorApiUrl;

        /// <summary>
        /// A constant string containing the key to get the country code.
        /// </summary>
        private const string CountryCode = "geoplugin_countryCode";

        /// <summary>
        /// Gets the location of the user based on its ip.
        /// </summary>
        /// <param name="ip">A string containing the ip address of the requesting user.</param>
        /// <returns>A string containing the country code (e.g. PK).</returns>
        public static string GetLocation(string ip)
        {
            if (_ipLocatorApiUrl == null)
            {
                _ipLocatorApiUrl = ConfigurationManager.AppSettings[NeeoConstants.IpLocatorApiUrl].ToString();
            }

            RestRequest request = new RestRequest(_ipLocatorApiUrl);
            request.AddUrlSegment("ip", ip);
            RestClient restClient = new RestClient();
            var response = restClient.Execute(request);
            if (response.ErrorException == null)
            {
                string responseContent = response.Content;
                var dictionary = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return dictionary[CountryCode].Value;
            }
            else
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, response.ErrorMessage);
                throw new ApplicationException(CustomHttpStatusCode.ServerConnectionError.ToString("D"));
            }
        }
    }
}
