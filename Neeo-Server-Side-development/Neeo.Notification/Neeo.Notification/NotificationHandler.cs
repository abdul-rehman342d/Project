using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Extension;
using DAL;
using LibNeeo;
using LibNeeo.MUC;
using LibNeeo.Plugin;
using Logger;
using Neeo.Notification.Model;
using Newtonsoft.Json;
using PushSharp.Google;

namespace Neeo.Notification
{
    public sealed class NotificationHandler
    {
        private static NotificationHandler _instance;
        private static object syncLock = new object();
        private AppNotificationServices _services;

        protected NotificationHandler()
        {
            _services = new AppNotificationServices();
        }

        public static NotificationHandler GetInstance()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new NotificationHandler();

                    }
                }
            }

            return _instance;
        }

        public void StartService()
        {
            if (_services != null)
            {
                _services.StartServices();
            }
        }

        public void StopService()
        {
            if (_services != null)
            {
                _services.StopServices();
            }
        }

        public void ProcessNotification(NotificationModel notificationModel)
        {
            switch (notificationModel.NType)
            {
                case NotificationType.Im:
                    SendImNotification(notificationModel);
                    break;

                case NotificationType.IncomingSipCall:
                    SendCallNotification(notificationModel);
                    break;

                case NotificationType.Mcr:
                    SendMcrNotification(notificationModel);
                    break;

                case NotificationType.GInvite:
                    SendGroupInviteNotification(notificationModel);
                    break;

                case NotificationType.GIm:
                    SendGroupImNotification(notificationModel);
                    break;

                case NotificationType.UpdateProfilePic:
                    SendUpdateProfilePicNotification(notificationModel);
                    break;
            }
        }

        private void SendImNotification(NotificationModel notificationModel)
        {
            ulong tempNumber;

            if (NeeoUtility.IsNullOrEmpty(notificationModel.DToken) || notificationModel.Dp == null || notificationModel.IMTone == null ||
            NeeoUtility.IsNullOrEmpty(notificationModel.Alert) || NeeoUtility.IsNullOrEmpty(notificationModel.SenderID) || NeeoUtility.IsNullOrEmpty(notificationModel.ReceiverID) || !ulong.TryParse(notificationModel.ReceiverID, out tempNumber) ||
                notificationModel.Badge == 0)
            {
                throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
            }

            var receiver = new NeeoUser(notificationModel.ReceiverID)
            {
                DeviceToken = notificationModel.DToken.Trim(),
                ImTone = notificationModel.IMTone.GetValueOrDefault(),
                OfflineMsgCount = notificationModel.Badge,
                DevicePlatform = notificationModel.Dp.GetValueOrDefault(),
                PnSource = notificationModel.PnSource.HasValue ? notificationModel.PnSource.Value : PushNotificationSource.Default

            };

            if (notificationModel.Alert.Length > NotificationAppConfiguration.NotificationMsgLength)
            {
                notificationModel.Alert = notificationModel.Alert.Substring(0, NotificationAppConfiguration.NotificationMsgLength) + "...";
            }

            if (receiver.DevicePlatform == DevicePlatform.iOS)
            {
                _services.ApnsService.Send(new List<NeeoUser>() { receiver }, notificationModel);
            }
            else if (receiver.DevicePlatform == DevicePlatform.Android)
            {
                _services.GcmService.Send(new List<NeeoUser>() { receiver }, notificationModel);
            }
        }

        private void SendCallNotification(NotificationModel notificationModel)
        {
            ulong tempNumber;

            if (NeeoUtility.IsNullOrEmpty(notificationModel.CallerID) ||
                !ulong.TryParse(notificationModel.CallerID, out tempNumber) ||
                NeeoUtility.IsNullOrEmpty(notificationModel.ReceiverID) ||
                !ulong.TryParse(notificationModel.ReceiverID, out tempNumber))
            {
                throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
            }
            // Get the name of the user from data base.
            var dbManager = new DbManager();
            var userInfo = dbManager.GetUserInforForNotification(notificationModel.ReceiverID,
                notificationModel.CallerID, false);

            if (NeeoUtility.IsNullOrEmpty(userInfo[NeeoConstants.ReceiverDeviceToken]) ||
               NeeoUtility.IsNullOrEmpty(userInfo[NeeoConstants.CallerName]))
            {
                throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
            }

            notificationModel.Alert = NotificationAppConfiguration.IncomingCallText.Replace("[" + NeeoConstants.CallerName + "]", userInfo[NeeoConstants.CallerName]);

            var receiver = new NeeoUser(notificationModel.ReceiverID)
            {
                CallingTone = (CallingTone)Convert.ToInt16(userInfo[NeeoConstants.ReceiverCallingTone]),
                DevicePlatform = (DevicePlatform)Convert.ToInt16(userInfo[NeeoConstants.ReceiverUserDeviceplatform]),
                DeviceToken = userInfo[NeeoConstants.ReceiverDeviceToken].Trim(),
                PnSource = (PushNotificationSource)Convert.ToInt32(userInfo[NeeoConstants.PushNotificationSource])
            };

            if (receiver.DevicePlatform == DevicePlatform.iOS)
            {
                _services.ApnsService.Send(new List<NeeoUser>() { receiver }, notificationModel);
            }
            else if (receiver.DevicePlatform == DevicePlatform.Android)
            {
                _services.GcmService.Send(new List<NeeoUser>() { receiver }, notificationModel);
            }
        }

        private void SendMcrNotification(NotificationModel notificationModel)
        {
            ulong tempNumber;

            if (NeeoUtility.IsNullOrEmpty(notificationModel.CallerID) ||
                !ulong.TryParse(notificationModel.CallerID, out tempNumber) ||
                NeeoUtility.IsNullOrEmpty(notificationModel.ReceiverID) ||
                !ulong.TryParse(notificationModel.ReceiverID, out tempNumber) || notificationModel.McrCount == 0)
            {
                throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
            }

            var dbManager = new DbManager();
            var userInfo = dbManager.GetUserInforForNotification(notificationModel.ReceiverID, notificationModel.CallerID, true, notificationModel.McrCount);

            if (NeeoUtility.IsNullOrEmpty(userInfo[NeeoConstants.ReceiverDeviceToken]))
            {
                throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
            }

            notificationModel.Alert = NotificationAppConfiguration.McrText.Replace("[" + NeeoConstants.CallerName + "]", userInfo[NeeoConstants.CallerName]);

            var receiver = new NeeoUser(notificationModel.ReceiverID)
            {
                DevicePlatform = (DevicePlatform)Convert.ToInt16(userInfo[NeeoConstants.ReceiverUserDeviceplatform]),
                DeviceToken = userInfo[NeeoConstants.ReceiverDeviceToken].Trim(),
                PnSource = (PushNotificationSource)Convert.ToInt32(userInfo[NeeoConstants.PushNotificationSource]),
                OfflineMsgCount = Convert.ToInt32(userInfo[NeeoConstants.OfflineMessageCount])
            };

            if (receiver.DevicePlatform == DevicePlatform.iOS)
            {
                _services.ApnsService.Send(new List<NeeoUser>() { receiver }, notificationModel);
            }
            else if (receiver.DevicePlatform == DevicePlatform.Android)
            {
                _services.GcmService.Send(new List<NeeoUser>() { receiver }, notificationModel);
            }
        }

        private void SendGroupImNotification(NotificationModel notificationModel)
        {
            if (NeeoUtility.IsNullOrEmpty(notificationModel.SenderID) || notificationModel.RID == 0 ||
                NeeoUtility.IsNullOrEmpty(notificationModel.RName) || NeeoUtility.IsNullOrEmpty(notificationModel.Alert) ||
                notificationModel.MessageType == null)
            {
                throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
            }

            List<NeeoUser> lstgroupParticipant = NeeoGroup.GetGroupParticipants(notificationModel.RID,
                    notificationModel.SenderID, (int)notificationModel.MessageType);

            if (lstgroupParticipant.Count == 0)
            {
                return;
            }

            var taskUpdateOfflineCount = Task.Factory.StartNew(() =>
                {
                    const string delimeter = ",";
                    LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Group participants - " + JsonConvert.SerializeObject(lstgroupParticipant), System.Reflection.MethodBase.GetCurrentMethod().Name);
                    IEnumerable<string> userList = from item in lstgroupParticipant
                                                   where item.PresenceStatus == Presence.Offline
                                                   select item.UserID;
                    string userString = string.Join(delimeter, userList);

                    //lstgroupParticipant.Where(i => i.PresenceStatus == Presence.Offline).Select(a => a.UserID)
                    //    .Aggregate((current, next) => current + delimeter + next);

                    LogManager.CurrentInstance.InfoLogger.LogInfo(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "User list - " + userString, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    if (NeeoUtility.IsNullOrEmpty(userString))
                        return;
                    var dbManager = new DbManager();
                    dbManager.UpdateUserOfflineCount(userString, delimeter);
                });

            var taskSendiOSNotification = Task.Factory.StartNew(() =>
            {
                var iosUserList =
                    lstgroupParticipant.Where(
                        x =>
                            x.DevicePlatform == DevicePlatform.iOS && x.PresenceStatus == Presence.Offline).ToList();

                _services.ApnsService.Send(iosUserList, notificationModel);
            });

            var taskSendAndroidNotification = Task.Factory.StartNew(() =>
            {
                var androidUserList =
                    lstgroupParticipant.Where(
                        x =>
                            x.DevicePlatform == DevicePlatform.Android && x.PresenceStatus == Presence.Offline).ToList();

                _services.GcmService.Send(androidUserList, notificationModel);
            });
            Task.WaitAll(taskUpdateOfflineCount, taskSendiOSNotification, taskSendAndroidNotification);
        }

        private void SendGroupInviteNotification(NotificationModel notificationModel)
        {
            ulong tempNumber;

            if (NeeoUtility.IsNullOrEmpty(notificationModel.DToken) || notificationModel.Dp == null || notificationModel.IMTone == null ||
                    NeeoUtility.IsNullOrEmpty(notificationModel.Alert) || NeeoUtility.IsNullOrEmpty(notificationModel.RName) || NeeoUtility.IsNullOrEmpty(notificationModel.ReceiverID) || !ulong.TryParse(notificationModel.ReceiverID, out tempNumber))
            {
                throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
            }

            var receiver = new NeeoUser(notificationModel.ReceiverID)
            {
                DeviceToken = notificationModel.DToken.Trim(),
                ImTone = notificationModel.IMTone.GetValueOrDefault(),
                OfflineMsgCount = notificationModel.Badge,
                DevicePlatform = notificationModel.Dp.GetValueOrDefault(),
                PnSource = notificationModel.PnSource.HasValue ? notificationModel.PnSource.Value : PushNotificationSource.Default

            };

            if (receiver.DevicePlatform == DevicePlatform.iOS)
            {
                _services.ApnsService.Send(new List<NeeoUser>() { receiver }, notificationModel);
            }
            else if (receiver.DevicePlatform == DevicePlatform.Android)
            {
                _services.GcmService.Send(new List<NeeoUser>() { receiver }, notificationModel);
            }
        }

        private void SendUpdateProfilePicNotification(NotificationModel notificationModel)
        {
            ulong tempNumber;

            if (NeeoUtility.IsNullOrEmpty(notificationModel.SenderID) ||
                !ulong.TryParse(notificationModel.SenderID, out tempNumber))
            {
                throw new ApplicationException(CustomHttpStatusCode.InvalidArguments.ToString("D"));
            }
            // Get the name of the user from data base.
            List<NeeoUser> userRoasterList = NeeoUser.GetUserRoster(notificationModel.SenderID);

            if (userRoasterList.Count == 0)
            {
                return;
            }

            notificationModel.Alert = "Update";

            var taskSendiOSNotification = Task.Factory.StartNew(() =>
            {
                var iosUserList =
                    userRoasterList.Where(
                        x =>
                            x.DevicePlatform == DevicePlatform.iOS && x.PresenceStatus == Presence.Offline).ToList();

                _services.ApnsService.Send(iosUserList, notificationModel);
            });

            var taskSendAndroidNotification = Task.Factory.StartNew(() =>
            {
                var androidUserList =
                    userRoasterList.Where(
                        x =>
                            x.DevicePlatform == DevicePlatform.Android 
                            //&& x.PresenceStatus == Presence.Offline
                            ).ToList();

                _services.GcmService.Send(androidUserList, notificationModel);
            });

            Task.WaitAll(taskSendiOSNotification, taskSendAndroidNotification);
        }

        //private void SendNotificatoin(NeeoU)
    }
}
