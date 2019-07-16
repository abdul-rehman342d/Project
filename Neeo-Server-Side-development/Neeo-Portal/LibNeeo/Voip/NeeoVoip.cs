using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Common;

namespace LibNeeo.VoipApi
{
    public static class NeeoVoipApi
    {
        /// <summary>
        /// 
        /// </summary>
        private static string _voipServerUrl;

        /// <summary>
        /// 
        /// </summary>
        private static string _voipSecretKey;

        /// <summary>
        /// 
        /// </summary>
        private static string _voipDomain;

        /// <summary>
        /// 
        /// </summary>
        public static void RegisterUser()
        {
            if (_voipServerUrl == null)
            {
                _voipServerUrl = ConfigurationManager.AppSettings[NeeoConstants.VoipServerUrl].ToString();
            }

            if (_voipSecretKey == null)
            {
                _voipSecretKey = ConfigurationManager.AppSettings[NeeoConstants.VoipSecretKey].ToString();
            }

            if (_voipDomain == null)
            {
                _voipDomain = ConfigurationManager.AppSettings[NeeoConstants.Domain].ToString();
            }

            
        }
    }
}
