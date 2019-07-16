using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Logger;
using Newtonsoft.Json;

namespace Common
{
    public static class InvitationMessage
    {
        #region Data Members
        /// <summary>
        /// An Object of Dictionary which contains Invitation message converted in all languages.
        /// </summary>
        private static Dictionary<string, string> _localizedMessages;
        #endregion Data Members

        #region Constructors
        static InvitationMessage()
        {
            LoadMessages();
        }
        #endregion Constructors

        #region internal methods
        /// <summary>
        /// it initializes the Localized messages Dictionary.
        /// </summary>
        private static void LoadMessages()
        {
            const string localizeMessagesFilePathKey = "localizeMessagesFilePath";
            string messagesFilePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[localizeMessagesFilePathKey]);

            if (_localizedMessages == null)
            {
                if (File.Exists(messagesFilePath))
                {
                    StreamReader reader = new StreamReader(messagesFilePath);
                    var json = reader.ReadToEnd();
                    reader.Close();
                    _localizedMessages = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
                else
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(
                                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + " ===> File does not exist @ path:" + messagesFilePath);
                    throw new ApplicationException(HttpStatusCode.InternalServerError.ToString("D"));
                }
            }
        }
        #endregion internal methods

        #region Localization
        /// <summary>
        /// it returns the Localized message against a language code.
        /// </summary>
        public static string GetLocalizedMessage(string languageCode)
        {
            if (_localizedMessages != null)
            {
                if (_localizedMessages.ContainsKey(languageCode))
                    return _localizedMessages[languageCode];
                else
                {
                    LogManager.CurrentInstance.InfoLogger.LogInfo(
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, System.Reflection.MethodBase.GetCurrentMethod().Name + "===> Invalid language = " + languageCode);
                    throw new ApplicationException(CustomHttpStatusCode.InvalidLanguageCode.ToString("D"));
                }
            }
            else
            {
                LoadMessages();
                return GetLocalizedMessage(languageCode);
            }
        }
        #endregion Localization

    }


}
