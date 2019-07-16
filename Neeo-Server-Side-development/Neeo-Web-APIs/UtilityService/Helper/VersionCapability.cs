using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Logger;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UtilityService.Models
{
    public static class VersionCapability
    {
        private static Dictionary<string, Capability> _versionCapabilities;

        static VersionCapability()
        {
            LoadVersionsCapabilities();
        }

        private static void LoadVersionsCapabilities()
        {
            const string appVersionsFilePathKey = "appVersionsFilePath";
            string appVersionsFilePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[appVersionsFilePathKey]);

            if (_versionCapabilities == null)
            {
                if (File.Exists(appVersionsFilePath))
                {
                    StreamReader reader = new StreamReader(appVersionsFilePath);
                    var json = reader.ReadToEnd();
                    reader.Close();
                    _versionCapabilities = JsonConvert.DeserializeObject<Dictionary<string, Capability>>(json);
                }
                else
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + " ===> File does not exist @ path:" + appVersionsFilePath);
                    throw new ApplicationException(HttpStatusCode.InternalServerError.ToString("D"));
                }
            }
        }

        public static Capability GetVersionCapability(string version)
        {
            return _versionCapabilities[version];
        }
    }
}