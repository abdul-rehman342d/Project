using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DAL;
using Newtonsoft.Json;
using PushSharp;
using PushSharp.Apple;
using PushSharp.Android;
using PushSharp.Core;
using System.IO;
using System.Web;
using System.Configuration;
using Common;
using Logger;
using NotificationService;

namespace NotificationService
{
    /// <summary>
    /// Manages push notifications that have to be sent on user's device
    /// </summary>
    public sealed class NotificationManager
    {
        /// <summary>
        /// An object responsible for sending push notification. 
        /// </summary>
        
        private static PushBroker _push;
        //public static NotificationManager Notification = new NotificationManager();
        private const string NotificationID = "pnid";
        private static string _iosApplicationDefaultTone;
        private static string _iosIncomingCallingTone;
        private static string _incomingCallingMsgText;
        private static string _mcrMsgText;
        private static string _actionKeyText;

        
        /// <summary>
        /// 
        /// </summary>
        
       public NotificationManager()
        {
            if (_push == null)
            {
                _push = new PushBroker();
                //Registring events.
                _push.OnNotificationSent += NotificationSent;
                _push.OnChannelException += ChannelException;
                _push.OnServiceException += ServiceException;
                _push.OnNotificationFailed += NotificationFailed;
                _push.OnNotificationRequeue += NotificationRequeue;
                _push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
                _push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
                _push.OnChannelCreated += ChannelCreated;
                _push.OnChannelDestroyed += ChannelDestroyed;
                var appleCertificate = File.ReadAllBytes(ConfigurationManager.AppSettings[NeeoConstants.AppleCertificatePath]);
                _push.RegisterAppleService(new ApplePushChannelSettings(true, appleCertificate, ConfigurationManager.AppSettings[NeeoConstants.AppleCertificatePwd]));
                //_push.RegisterGcmService(new GcmPushChannelSettings(ConfigurationManager.AppSettings[NeeoConstants.GoogleApiKey].ToString()));
            }
        }

        /// <summary>
        /// Sends notifications to the specified device token and device platform .
        /// </summary>
        /// <param name="notificationType">An enum specifying the notification type.</param>
        /// <param name="devicePlatform">An enum specifying the device platform.</param>
        /// <param name="deviceToken">A string containing the device token.</param>
        /// <param name="data">A dictionary containing the notification data.</param>
        public void SendNotification(NotificationType notificationType, DevicePlatform devicePlatform, string deviceToken, Dictionary<string, string> data)
        {

            int mcrCount = 0;

            switch (notificationType)
            {
                case NotificationType.IM:
                    int badgeCount = 0;

                    string notificationMsg = data[NeeoConstants.Alert];

                    if (!NeeoUtility.IsNullOrEmpty(notificationMsg) &&
                        Int32.TryParse(data[NeeoConstants.Badge], out badgeCount))
                    {
                        if (notificationMsg.Length > 50)
                        {
                            notificationMsg = notificationMsg.Substring(0, 50) + "...";
                        }
                        // mcrCount = call to service to get mcr count
                        badgeCount += mcrCount;
                        if (devicePlatform == DevicePlatform.iOS)
                        {
                            if (_iosApplicationDefaultTone == null)
                            {
                                _iosApplicationDefaultTone =
                                    ConfigurationManager.AppSettings[NeeoConstants.IosApplicationDefaultTone];
                            }
                            _push.QueueNotification(new AppleNotification(deviceToken)
                                .WithAlert(notificationMsg)
                                .WithBadge(badgeCount)
                                .WithSound(_iosApplicationDefaultTone)
                                .WithCustomItem(NotificationID, NotificationType.IM.ToString("D")));
                        }
                        else if (devicePlatform == DevicePlatform.Android)
                        {
                            //_push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(deviceToken)
                            // .WithJson("{\"alert\":\"" + notificationMsg + "\",\"badge\":" + bageCount + ",\"sound\":\"sound.caf\"}"));
                        }
                        else
                        {
                            //do nothing
                        }
                    }
                    else
                    {
                        throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
                    }
                    break;
                case NotificationType.IncomingSipCall:
                    string callerID;
                    string callName;
                    if (_incomingCallingMsgText == null)
                    {
                        _incomingCallingMsgText = ConfigurationManager.AppSettings[NeeoConstants.IncomingCallingMsgText];
                    }

                    callerID = data["CallerID"];
                    if (!NeeoUtility.IsNullOrEmpty(callerID))
                    {
                        // Get the name of the user from data base.
                        string userProfileName = "";
                        if (devicePlatform == DevicePlatform.iOS)
                        {
                            if (_actionKeyText == null)
                            {
                                _actionKeyText = ConfigurationManager.AppSettings[NeeoConstants.ActionKeyText];
                            }
                            if (_iosIncomingCallingTone == null)
                            {
                                _iosIncomingCallingTone =
                                    ConfigurationManager.AppSettings[NeeoConstants.IosIncomingCallingTone];
                            }

                            var i = new AppleNotification(deviceToken)
                                .WithAlert(_incomingCallingMsgText + userProfileName, _actionKeyText)
                                .WithSound(_iosIncomingCallingTone)
                                .WithCustomItem(NotificationID, NotificationType.IncomingSipCall.ToString("D"));
                           _push.QueueNotification(i);
                        }
                        else if (devicePlatform == DevicePlatform.Android)
                        {
                            //_push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(deviceToken)
                            // .WithJson("{\"alert\":\"" + notificationMsg + "\",\"badge\":" + bageCount + ",\"sound\":\"sound.caf\"}"));
                        }
                        else
                        {
                            // do nothing
                        }
                    }
                    else
                    {
                        throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
                    }
                    break;
                case NotificationType.MCR:
                    if (_mcrMsgText == null)
                    {
                        _mcrMsgText = ConfigurationManager.AppSettings[NeeoConstants.McrMsgText];
                    }

                    // mcrCount = call to service to get mcr count\
                    mcrCount = 8;
                    if (devicePlatform == DevicePlatform.iOS)
                    {
                        if (_iosApplicationDefaultTone == null)
                        {
                            _iosApplicationDefaultTone = ConfigurationManager.AppSettings[NeeoConstants.IosApplicationDefaultTone];
                        }

                        _push.QueueNotification(new AppleNotification(deviceToken)
                            .WithAlert("text")
                            .WithSound(_iosApplicationDefaultTone)
                            .WithCustomItem(NotificationID, NotificationType.MCR.ToString("D")));
                    }
                    else if (devicePlatform == DevicePlatform.Android)
                    {
                        //_push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(deviceToken)
                        // .WithJson("{\"alert\":\"" + notificationMsg + "\",\"badge\":" + bageCount + ",\"sound\":\"sound.caf\"}"));
                    }
                    else
                    {
                        //do nothing
                    }
                    break;
            }
        }
        /// <summary>
        /// An event that is called when channel destroyed.
        /// </summary>
        /// <param name="sender"></param>
        private void ChannelDestroyed(object sender)
        {
        }
        /// <summary>
        /// An event is called when channel created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pushChannel"></param>
        private void ChannelCreated(object sender, PushSharp.Core.IPushChannel pushChannel)
        {

        }
        /// <summary>
        /// An event that is called when device subscription changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oldSubscriptionId"></param>
        /// <param name="newSubscriptionId"></param>
        /// <param name="notification"></param>
        private void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, PushSharp.Core.INotification notification)
        {
            LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Device subscription changed.");
        }
        /// <summary>
        /// An event that is called when device subscrpition expired.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="expiredSubscriptionId"></param>
        /// <param name="expirationDateUtc"></param>
        /// <param name="notification"></param>
        private void DeviceSubscriptionExpired(object sender, string expiredSubscriptionId, DateTime expirationDateUtc, PushSharp.Core.INotification notification)
        {
            LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Device subscription expired.");
        }
        /// <summary>
        /// An event that is called when notification failed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notification"></param>
        /// <param name="error"></param>
        private void NotificationFailed(object sender, PushSharp.Core.INotification notification, Exception error)
        {
            LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, error.Message);
        }
        /// <summary>
        /// An event that is called when service exception occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="error"></param>
        private void ServiceException(object sender, Exception error)
        {
            LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, error.Message);
        }
        /// <summary>
        /// An event that is called when channel exception occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pushChannel"></param>
        /// <param name="error"></param>
        private void ChannelException(object sender, PushSharp.Core.IPushChannel pushChannel, Exception error)
        {
            LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, error.Message);
        }
        /// <summary>
        /// An event that is call when notification successfully sent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notification"></param>
        private void NotificationSent(object sender, PushSharp.Core.INotification notification)
        {

        }

        private void NotificationRequeue(object sender, NotificationRequeueEventArgs e)
        {
          
        }
    }
}
