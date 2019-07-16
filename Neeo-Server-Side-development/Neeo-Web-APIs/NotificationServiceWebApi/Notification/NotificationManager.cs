using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using Common.Extension;
using DAL;
using LibNeeo;
using LibNeeo.MUC;
using LibNeeo.Plugin;
using LibNeeo.Voip;
using Newtonsoft.Json;
using PushSharp;
using PushSharp.Apple;
using PushSharp.Android;
using PushSharp.Core;
using Pushy.Api;
using System.Web;
using System.Configuration;
using Common;
using Logger;
using Pushy.Api.Entities;
using File = System.IO.File;

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
        private static string _iosApplicationMcrTone;
        private static string _iosIncomingCallingTone;
        private static string _incomingCallingMsgText;
        private static string _mcrMsgText;
        private static string _groupMsgText;
        private static string _actionKeyText;
        private static ushort _notificationMsgLength;


        /// <summary>
        /// 
        /// </summary>

        public NotificationManager()
        {
            if (_push == null)
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Notification constructor is running.");
                _push = new PushBroker();
                //Registring events.
                _notificationMsgLength = 0;
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
                _push.RegisterGcmService(new GcmPushChannelSettings(ConfigurationManager.AppSettings[NeeoConstants.GoogleApiKey]));
            }
        }

        /// <summary>
        /// Sends notifications to the specified device token and device platform .
        /// </summary>
        /// <param name="notification">An enum specifying the notification type.</param>
        public void SendNotification(Notification notification)
        {
            int badgeCount = 0;
            ulong tempNumber;
            const string delimeter = ",";
            DevicePlatform devicePlatform;
            string deviceToken = null;

            switch (notification.NType)
            {
                case NotificationType.Im:
                    #region IM notfication

                    if (_notificationMsgLength == 0)
                    {
                        _notificationMsgLength = Convert.ToUInt16(ConfigurationManager.AppSettings[NotificationConstants.NotificationMsgLength]);
                    }
                    if (!NeeoUtility.IsNullOrEmpty(notification.DToken) && notification.Dp != null && notification.IMTone != null &&
                    !NeeoUtility.IsNullOrEmpty(notification.Alert) && !NeeoUtility.IsNullOrEmpty(notification.SenderID) && !NeeoUtility.IsNullOrEmpty(notification.ReceiverID) && ulong.TryParse(notification.ReceiverID, out tempNumber) &&
                        notification.Badge != 0)
                    {
                        NeeoUser receiver = new NeeoUser(notification.ReceiverID)
                        {
                            DeviceToken = notification.DToken.Trim(),
                            ImTone = notification.IMTone.GetValueOrDefault(),
                            OfflineMsgCount = notification.Badge,
                            DevicePlatform = notification.Dp.GetValueOrDefault()

                        };

                        if (notification.Alert.Length > _notificationMsgLength)
                        {
                            notification.Alert = notification.Alert.Substring(0, _notificationMsgLength) + "...";
                        }

                        if (receiver.DevicePlatform == DevicePlatform.iOS)
                        {

                            _push.QueueNotification(new AppleNotification(receiver.DeviceToken)
                                .WithAlert(notification.Alert)
                                .WithBadge(receiver.OfflineMsgCount)
                                .WithSound(receiver.ImTone.ToString("G").Replace('_', '.'))
                                .WithCustomItem(NotificationConstants.NotificationID, NotificationType.Im.ToString("D"))
                                .WithCustomItem(NotificationConstants.SenderID, notification.SenderID));
                        }
                        else if (receiver.DevicePlatform == DevicePlatform.Android)
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
                    #endregion

                case NotificationType.IncomingSipCall:
                                        #region Incoming sip call notification
                    
                    if (_incomingCallingMsgText == null)
                    {
                        _incomingCallingMsgText = ConfigurationManager.AppSettings[NotificationConstants.IncomingCallingMsgText];
                    }

                    if (!NeeoUtility.IsNullOrEmpty(notification.CallerID) && ulong.TryParse(notification.CallerID, out tempNumber) && !NeeoUtility.IsNullOrEmpty(notification.ReceiverID) && ulong.TryParse(notification.ReceiverID, out tempNumber))
                    {
                        // Get the name of the user from data base.
                        DbManager dbManager = new DbManager();
                        var userInfo = dbManager.GetUserInforForNotification(notification.ReceiverID, notification.CallerID, false);
                        NeeoUser receiver = new NeeoUser(notification.ReceiverID)
                        {
                            CallingTone = (CallingTone)Convert.ToInt16(userInfo[NeeoConstants.ReceiverCallingTone]),
                            DevicePlatform = (DevicePlatform)Convert.ToInt16(userInfo[NeeoConstants.ReceiverUserDeviceplatform])
                        };

                        if (!NeeoUtility.IsNullOrEmpty(userInfo[NeeoConstants.ReceiverDeviceToken]) && !NeeoUtility.IsNullOrEmpty(userInfo[NeeoConstants.CallerName]))
                        {
                            receiver.DeviceToken = userInfo[NeeoConstants.ReceiverDeviceToken].Trim();
                            receiver.PnSource =
                                (PushNotificationSource)Convert.ToInt32(userInfo[NeeoConstants.PushNotificationSource]);
                        }
                        else
                        {
                            throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
                        }

                        if (receiver.DevicePlatform == DevicePlatform.iOS)
                        {
                            if (_actionKeyText == null)
                            {
                                _actionKeyText = ConfigurationManager.AppSettings[NotificationConstants.ActionKeyText];
                            }
                            if (_iosIncomingCallingTone == null)
                            {
                                _iosIncomingCallingTone =
                                    ConfigurationManager.AppSettings[NotificationConstants.IosIncomingCallingTone];
                            }

                            _push.QueueNotification(new AppleNotification(receiver.DeviceToken)
                                .WithAlert(_incomingCallingMsgText.Replace("[" + NeeoConstants.CallerName + "]", userInfo[NeeoConstants.CallerName]), "", _actionKeyText, new List<object>() { })
                                .WithSound(receiver.CallingTone.GetDescription())
                                .WithCustomItem(NotificationConstants.NotificationID, NotificationType.IncomingSipCall.ToString("D"))
                                .WithCustomItem(NotificationConstants.Timestamp, DateTime.UtcNow.ToString(NeeoConstants.TimestampFormat)));
                        }
                        else if (receiver.DevicePlatform == DevicePlatform.Android)
                        {
                            Dictionary<string, string> payload = new Dictionary<string, string>()
                            {
                                {"alert",_incomingCallingMsgText.Replace("[" + NeeoConstants.CallerName + "]", userInfo[NeeoConstants.CallerName])},
                                {NotificationConstants.NotificationID,NotificationType.IncomingSipCall.ToString("D")},
                                {NotificationConstants.Timestamp,DateTime.UtcNow.ToString(NeeoConstants.TimestampFormat)},
                                {NotificationConstants.CallerID, notification.CallerID}
                            };
                            switch (receiver.PnSource)
                            {
                                case PushNotificationSource.Default:
                            _push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(receiver.DeviceToken)
                             .WithJson(JsonConvert.SerializeObject(payload)));
                                    break;
                                case PushNotificationSource.Pushy:
                                    var pushyRequest = new PushyPushRequest(payload, new string[] {receiver.DeviceToken});
                                    PushyClient.SendPush(pushyRequest);
                                    break;

                        }
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
                    #endregion

                case NotificationType.Mcr:
                    #region Mcr notification

                    if (_mcrMsgText == null)
                    {
                        _mcrMsgText = ConfigurationManager.AppSettings[NotificationConstants.McrMsgText];
                    }

                    if (!NeeoUtility.IsNullOrEmpty(notification.CallerID) && ulong.TryParse(notification.CallerID, out tempNumber) && !NeeoUtility.IsNullOrEmpty(notification.ReceiverID) && ulong.TryParse(notification.ReceiverID, out tempNumber) && notification.McrCount != 0)
                    {

                        DbManager dbManager = new DbManager();
                        var userInfo = dbManager.GetUserInforForNotification(notification.ReceiverID, notification.CallerID, true, notification.McrCount);

                        NeeoUser receiver = new NeeoUser(notification.ReceiverID)
                        {
                            DevicePlatform = (DevicePlatform)Convert.ToInt16(userInfo[NeeoConstants.ReceiverUserDeviceplatform]),
                        };

                        if (!NeeoUtility.IsNullOrEmpty(userInfo[NeeoConstants.ReceiverDeviceToken]))
                        {
                            receiver.DeviceToken = userInfo[NeeoConstants.ReceiverDeviceToken].Trim();
                            receiver.PnSource =
                                (PushNotificationSource)Convert.ToInt32(userInfo[NeeoConstants.PushNotificationSource]);
                        }
                        else
                        {
                            throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
                        }
                        badgeCount = Convert.ToInt32(userInfo[NeeoConstants.OfflineMessageCount]);
                        if (receiver.DevicePlatform == DevicePlatform.iOS)
                        {
                            if (_iosApplicationMcrTone == null)
                            {
                                _iosApplicationMcrTone =
                                    ConfigurationManager.AppSettings[NotificationConstants.IosApplicationMcrTone];
                            }
                            _push.QueueNotification(new AppleNotification(receiver.DeviceToken)
                                .WithAlert(_mcrMsgText.Replace("[" + NeeoConstants.CallerName + "]", userInfo[NeeoConstants.CallerName]))
                                .WithBadge(badgeCount)
                                .WithSound(_iosApplicationMcrTone)
                                .WithCustomItem(NotificationConstants.NotificationID, NotificationType.Mcr.ToString("D"))
                                .WithCustomItem(NotificationConstants.CallerID, notification.CallerID));
                        }
                        else if (receiver.DevicePlatform == DevicePlatform.Android)
                        {
                            Dictionary<string, string> payload = new Dictionary<string, string>()
                            {
                                {"alert",_mcrMsgText.Replace("[" + NeeoConstants.CallerName + "]", userInfo[NeeoConstants.CallerName])},
                                {NotificationConstants.NotificationID, NotificationType.Mcr.ToString("D")},
                                {NotificationConstants.CallerID, notification.CallerID}
                            };
                            switch (receiver.PnSource)
                            {
                                case PushNotificationSource.Default:
                                    _push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(receiver.DeviceToken)
                            .WithJson(JsonConvert.SerializeObject(payload)));
                                    break;
                                case PushNotificationSource.Pushy:
                                    var pushyRequest = new PushyPushRequest(payload, new string[] {receiver.DeviceToken});
                                    PushyClient.SendPush(pushyRequest);
                                    break;

                            }
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
                    #endregion

                case NotificationType.GIm:
                    #region Group notification
                    if (!NeeoUtility.IsNullOrEmpty(notification.SenderID) && notification.RID != 0 && !NeeoUtility.IsNullOrEmpty(notification.RName) && !NeeoUtility.IsNullOrEmpty(notification.Alert) && notification.MessageType != null)
                    {
                       
                        List<NeeoUser> lstgroupParticipant = NeeoGroup.GetGroupParticipants(notification.RID,
                            notification.SenderID, (int) notification.MessageType );
                       
                        
                        if (lstgroupParticipant.Count > 0)
                        {
                            var taskUpdateOfflineCount = Task.Factory.StartNew(() =>
                            {
                                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Group participants - " + JsonConvert.SerializeObject(lstgroupParticipant), System.Reflection.MethodBase.GetCurrentMethod().Name);
                                IEnumerable<string> userList = from item in lstgroupParticipant
                                                               where item.PresenceStatus == Presence.Offline
                                                               select item.UserID;
                                string userString = string.Join(delimeter, userList);


                                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "User list - " + userString, System.Reflection.MethodBase.GetCurrentMethod().Name);
                                if (!NeeoUtility.IsNullOrEmpty(userString))
                                {
                                    DbManager dbManager = new DbManager();
                                    dbManager.UpdateUserOfflineCount(userString, delimeter);
                                }
                            });
                            var taskScheduleNotifications = Task.Factory.StartNew(() =>
                            {
                                Parallel.ForEach(lstgroupParticipant, item =>
                                //foreach (var item in lstgroupParticipant)
                                {
                                    if (item.DevicePlatform == DevicePlatform.iOS)
                                    {
                                        if (!NeeoUtility.IsNullOrEmpty(item.DeviceToken) && item.DeviceToken != "-1" &&
                                            item.PresenceStatus == Presence.Offline)
                                        {
                                            _push.QueueNotification(new AppleNotification(
                                                item.DeviceToken.Trim())
                                                .WithAlert(notification.Alert)
                                                .WithBadge(item.OfflineMsgCount + 1)
                                                .WithSound(item.ImTone.ToString("G").Replace('_', '.'))
                                                .WithCustomItem(NotificationConstants.NotificationID, NotificationType.GIm.ToString("D"))
                                                .WithCustomItem(NotificationConstants.RoomID, notification.RName));
                                        }
                                    }
                                    else if (item.DevicePlatform == DevicePlatform.Android)
                                    {
                                        //_push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(deviceToken)
                                        // .WithJson("{\"alert\":\"" + notificationMsg + "\",\"badge\":" + bageCount + ",\"sound\":\"sound.caf\"}"));
                                    }
                                    else
                                    {
                                        //do nothing
                                    }

                                });
                            });
                            taskUpdateOfflineCount.Wait();
                            taskScheduleNotifications.Wait();
                        }
                    }
                    else
                    {
                        throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
                    }
                    break;
                    #endregion

                case NotificationType.GInvite:
                    #region Group invitation notification

                    if (!NeeoUtility.IsNullOrEmpty(notification.DToken) && notification.Dp != null && notification.IMTone != null &&
                    !NeeoUtility.IsNullOrEmpty(notification.Alert) && !NeeoUtility.IsNullOrEmpty(notification.RName) && !NeeoUtility.IsNullOrEmpty(notification.ReceiverID) && ulong.TryParse(notification.ReceiverID, out tempNumber))
                    {
                        NeeoUser receiver = new NeeoUser(notification.ReceiverID)
                        {
                            DeviceToken = notification.DToken.Trim(),
                            ImTone = notification.IMTone.GetValueOrDefault(),
                            OfflineMsgCount = notification.Badge,
                            DevicePlatform = notification.Dp.GetValueOrDefault()

                        };

                        //badgeCount += (notification.Badge + mcrCount);
                        if (receiver.DevicePlatform == DevicePlatform.iOS)
                        {
                            _push.QueueNotification(new AppleNotification(receiver.DeviceToken)
                                .WithAlert(notification.Alert)
                                .WithBadge(receiver.OfflineMsgCount)
                                .WithSound(receiver.ImTone.ToString("G").Replace('_', '.'))
                                .WithCustomItem(NotificationConstants.NotificationID,
                                    NotificationType.GIm.ToString("D"))
                                .WithCustomItem(NotificationConstants.RoomID, notification.RName));
                        }
                        else if (receiver.DevicePlatform == DevicePlatform.Android)
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
                    #endregion
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
            LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Device subscription changed.", System.Reflection.MethodBase.GetCurrentMethod().Name);
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
            LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Device subscription expired.", System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
        /// <summary>
        /// An event that is called when notification failed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notification"></param>
        /// <param name="error"></param>
        private void NotificationFailed(object sender, PushSharp.Core.INotification notification, Exception error)
        {
            LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, error.Message + " ==> " + ((PushSharp.Apple.NotificationFailureException)(error)).ErrorStatusCode.ToString() + "-" + ((PushSharp.Apple.NotificationFailureException)(error)).ErrorStatusDescription, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
        /// <summary>
        /// An event that is called when service exception occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="error"></param>
        private void ServiceException(object sender, Exception error)
        {
            LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, error.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
        /// <summary>
        /// An event that is called when channel exception occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pushChannel"></param>
        /// <param name="error"></param>
        private void ChannelException(object sender, PushSharp.Core.IPushChannel pushChannel, Exception error)
        {
            LogManager.CurrentInstance.ErrorLogger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, error.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
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
