using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using System.Web;
using System.Configuration;
using Common;
using Logger;
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
        private static string _actionKeyText;
        private static ushort _notificationMsgLength;

        /// <summary>
        /// 
        /// </summary>
        public NotificationManager()
        {
            if (_push == null)
            {
                Console.WriteLine("Constructor fired.");
                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Notification constructor is running.");
                try
                {
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
                    var appleCertificate =
                        File.ReadAllBytes(ConfigurationManager.AppSettings[NeeoConstants.AppleCertificatePath]);
                    var channelSettings = new ApplePushChannelSettings(true, appleCertificate,
                        ConfigurationManager.AppSettings[NeeoConstants.AppleCertificatePwd]);
                    channelSettings.ConnectionTimeout = 36000;
                    _push.RegisterAppleService(channelSettings);
                    _push.RegisterGcmService(
                        new GcmPushChannelSettings(ConfigurationManager.AppSettings[NeeoConstants.GoogleApiKey]));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
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
                            DeviceToken = notification.DToken,
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
                            //_push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(receiver.DeviceToken)
                            // .WithJson("{\"alert\":\"" + notification.Alert + "\"}"));
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

                        devicePlatform = (DevicePlatform)Convert.ToInt16(userInfo[NeeoConstants.ReceiverUserDeviceplatform]);
                        if (!NeeoUtility.IsNullOrEmpty(userInfo[NeeoConstants.ReceiverDeviceToken]) && !NeeoUtility.IsNullOrEmpty(userInfo[NeeoConstants.CallerName]))
                        {
                            deviceToken = userInfo[NeeoConstants.ReceiverDeviceToken];
                        }
                        else
                        {
                            throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
                        }

                        if (devicePlatform == DevicePlatform.iOS)
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
                            Console.WriteLine("device token : " + deviceToken + " alert  : " +
                                              _incomingCallingMsgText.Replace("[" + NeeoConstants.CallerName + "]",
                                                  userInfo[NeeoConstants.CallerName]));
                            _push.QueueNotification(new AppleNotification(deviceToken)
                                .WithAlert(_incomingCallingMsgText.Replace("[" + NeeoConstants.CallerName + "]", userInfo[NeeoConstants.CallerName]), "", _actionKeyText, new List<object>() { })
                                .WithSound(_iosIncomingCallingTone)
                                .WithCustomItem(NotificationConstants.NotificationID, NotificationType.IncomingSipCall.ToString("D"))
                                .WithCustomItem(NotificationConstants.Timestamp, DateTime.UtcNow.ToString(NeeoConstants.TimestampFormat)));
                        }
                        else if (devicePlatform == DevicePlatform.Android)
                        {
                            _push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(deviceToken)
                             .WithJson("{\"alert\":\"" + _incomingCallingMsgText.Replace("[" + NeeoConstants.CallerName + "]", userInfo[NeeoConstants.CallerName]) + "\"}"));
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

                        devicePlatform = (DevicePlatform)Convert.ToInt16(userInfo[NeeoConstants.ReceiverUserDeviceplatform]);
                        if (!NeeoUtility.IsNullOrEmpty(userInfo[NeeoConstants.ReceiverDeviceToken]))
                        {
                            deviceToken = userInfo[NeeoConstants.ReceiverDeviceToken];
                        }
                        else
                        {
                            throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
                        }
                        badgeCount = Convert.ToInt32(userInfo[NeeoConstants.OfflineMessageCount]);
                        if (devicePlatform == DevicePlatform.iOS)
                        {
                            if (_iosApplicationMcrTone == null)
                            {
                                _iosApplicationMcrTone =
                                    ConfigurationManager.AppSettings[NotificationConstants.IosApplicationMcrTone];
                            }
                            _push.QueueNotification(new AppleNotification(deviceToken)
                                .WithAlert(_mcrMsgText.Replace("[" + NeeoConstants.CallerName + "]", userInfo[NeeoConstants.CallerName]))
                                .WithBadge(badgeCount)
                                .WithSound(_iosApplicationMcrTone)
                                .WithCustomItem(NotificationConstants.NotificationID, NotificationType.Mcr.ToString("D"))
                                .WithCustomItem(NotificationConstants.CallerID, notification.CallerID));
                        }
                        else if (devicePlatform == DevicePlatform.Android)
                        {
                            _push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(deviceToken)
                            .WithJson("{\"alert\":\"" + _mcrMsgText.Replace("[" + NeeoConstants.CallerName + "]", userInfo[NeeoConstants.CallerName]) + "\"}"));
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
                    if (!NeeoUtility.IsNullOrEmpty(notification.SenderID) && notification.RID != 0 && !NeeoUtility.IsNullOrEmpty(notification.RName) && !NeeoUtility.IsNullOrEmpty(notification.Alert))
                    {
                        List<NeeoUser> lstgroupParticipant = NeeoGroup.GetGroupParticipants(notification.RID,
                            notification.SenderID,1);
                        if (lstgroupParticipant.Count > 0)
                        {
                            var taskUpdateOfflineCount = Task.Factory.StartNew(() =>
                            {
                                LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Group participants - " + JsonConvert.SerializeObject(lstgroupParticipant), System.Reflection.MethodBase.GetCurrentMethod().Name);
                                IEnumerable<string> userList = from item in lstgroupParticipant
                                                               where item.PresenceStatus == Presence.Offline
                                                               select item.UserID;
                                string userString = string.Join(delimeter, userList);

                                //lstgroupParticipant.Where(i => i.PresenceStatus == Presence.Offline).Select(a => a.UserID)
                                //    .Aggregate((current, next) => current + delimeter + next);

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
                                                item.DeviceToken)
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
                            DeviceToken = notification.DToken,
                            ImTone = notification.IMTone.GetValueOrDefault(),
                            OfflineMsgCount = notification.Badge,
                            DevicePlatform = notification.Dp.GetValueOrDefault()

                        };

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
        /// 
        /// </summary>
        public void KeepAliveChannel()
        {
            _push.QueueNotification(new AppleNotification("d3ae769fa776934ce5a6b94994dae23d37ea9e8f6c111bbfe1b4682dacddcc41")
                               .WithAlert("keep alive")
                               .WithSound("Sound.caf"));
        }
        /// <summary>
        /// An event that is called when channel destroyed.
        /// </summary>
        /// <param name="sender"></param>
        private void ChannelDestroyed(object sender)
        {
            Console.WriteLine("Channel destroyed on " + DateTime.Now);
        }
        /// <summary>
        /// An event is called when channel created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pushChannel"></param>
        private void ChannelCreated(object sender, PushSharp.Core.IPushChannel pushChannel)
        {
            Console.WriteLine("Channel created on " + DateTime.Now);
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
            Console.WriteLine("Notification Faild. ==> " + ((PushSharp.Apple.NotificationFailureException)(error)).ErrorStatusDescription);
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
            Console.WriteLine("Notification Sent.");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotificationRequeue(object sender, NotificationRequeueEventArgs e)
        {
            Console.WriteLine("Notification requeue. notification ==> " + e.Notification + " cause ==> " + e.RequeueCause);
        }
    }
}
