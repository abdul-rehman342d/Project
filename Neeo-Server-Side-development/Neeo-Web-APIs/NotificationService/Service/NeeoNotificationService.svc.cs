using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Configuration;
using System.Text;
using Common;
using Logger;
using Newtonsoft.Json;
using PushSharp.Core;
using PushSharp;
using PushSharp.Apple;
using PushSharp.Android;
using System.IO;



namespace NotificationService
{
    public class NeeoNotificationService : INeeoNotificationService
    {
        private bool _logRequestResponse =
    Convert.ToBoolean(ConfigurationManager.AppSettings[NeeoConstants.LogRequestResponse]);

        /// <summary>
        /// Sends push notifications with message and badge count to the user device specified with device token.
        /// </summary>
        /// <param name="nType">An integer containing specifying notification type.</param>
        /// <param name="dp">An integer containing specifying device platform.</param>
        /// <param name="dToken">A string containing the device token of the user device to whom message has to be delivered.</param>
        /// <param name="data">A dictionary having notification data.</param>
        public void SendNotification(short nType, short dp, string dToken, Dictionary<string, string> data)
        {
            NotificationType notificationType = (NotificationType)nType;
            DevicePlatform devicePlatform = (DevicePlatform)dp;
            if (Enum.IsDefined(typeof(NotificationType), nType) && Enum.IsDefined(typeof(DevicePlatform), dp) && !NeeoUtility.IsNullOrEmpty(dToken))
            {
                try
                {
                    NotificationManager notificationManager = new NotificationManager();
                    notificationManager.SendNotification(notificationType, devicePlatform, dToken, data);
                }
                catch (ApplicationException appExp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "notificationType:" + notificationType + ", devicePlatform:" + devicePlatform + ", deviceToken:" + dToken + ", data:" + JsonConvert.SerializeObject(data) + ", error:" + appExp.Message);
                    NeeoUtility.SetServiceResponseHeaders((CustomHttpStatusCode)Convert.ToInt32(appExp.Message));
                }
                catch (Exception exp)
                {
                    LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "notificationType:" + notificationType + ", devicePlatform:" + devicePlatform + ", deviceToken:" + dToken + ", data:" + JsonConvert.SerializeObject(data) + ", error:" + exp.Message, exp);
                    NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.ServerInternalError);
                }
            }
            else
            {
                NeeoUtility.SetServiceResponseHeaders(CustomHttpStatusCode.InvalidArguments);
            }
        }
    }
}
