using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using LibNeeo;
using Logger;
using Neeo.Notification.Factory;
using Neeo.Notification.Model;
using Newtonsoft.Json.Linq;
using PushSharp.Core;
using PushSharp.Google;
using Pushy.Api;
using Pushy.Api.Entities;

namespace Neeo.Notification
{
    public class GcmService : NotificationService
    {
        private GcmServiceBroker _gcmServiceBroker;

        public GcmService()
        {
            InitializeService();
            BindEvents();
        }

        public override void Send(List<NeeoUser> receiverList, NotificationModel notificationModel)
        {
            if (receiverList == null || receiverList.Count == 0)
            {
                LogManager.CurrentInstance.ErrorLogger.LogError(
                           System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                           "receiverList is either null or empty.");
                return;
            }

            Parallel.ForEach(receiverList, (item) =>
            {
                var payload = new GcmPayload().Create(item, notificationModel);

                if (item.PnSource == PushNotificationSource.Pushy)
                {
                    var pushyRequest = new PushyPushRequest(payload, new string[] { item.DeviceToken });
                    PushyClient.SendPush(pushyRequest);
                    return;
                }

                var notification = new GcmNotification()
                {
                    RegistrationIds = new List<string> { item.DeviceToken },
                    Priority = GcmNotificationPriority.High,
                    Data = JObject.FromObject(payload)
                };
                _gcmServiceBroker.QueueNotification(notification);
            });

        }

        public override void Stop()
        {
            _gcmServiceBroker.Stop();
        }

        protected override void InitializeService()
        {
            var apnsConfig = new GcmConfiguration("", GcmAppConfiguration.GoogleApiKey, null);
            _gcmServiceBroker = new GcmServiceBroker(apnsConfig);
        }

        protected override void BindEvents()
        {
            _gcmServiceBroker.OnNotificationFailed += (notification, aggregateEx) =>
            {

                aggregateEx.Handle(ex =>
                {

                    // See what kind of exception it was to further diagnose
                    if (ex is GcmNotificationException)
                    {
                        var notificationException = (GcmNotificationException)ex;

                        // Deal with the failed notification
                        var gcmNotification = notificationException.Notification;
                        var description = notificationException.Description;

                        //Console.WriteLine($
                        //"GCM Notification Failed: ID={gcmNotification.MessageId}, Desc={description}")
                        //;
                        LogManager.CurrentInstance.ErrorLogger.LogError(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                            "Gcm Failed: ID = " + gcmNotification.MessageId + ", Desc = " + notificationException.Description);
                    }
                    else if (ex is GcmMulticastResultException)
                    {
                        var multicastException = (GcmMulticastResultException)ex;

                        foreach (var succeededNotification in multicastException.Succeeded)
                        {
                            //Console.WriteLine($
                            //"GCM Notification Succeeded: ID={succeededNotification.MessageId}")
                            //;
                            LogManager.CurrentInstance.InfoLogger.LogInfo(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                            "Gcm Succeeded: ID = " + succeededNotification.MessageId);
                        }

                        foreach (var failedKvp in multicastException.Failed)
                        {
                            var n = failedKvp.Key;
                            var e = failedKvp.Value;

                            //Console.WriteLine($
                            //"GCM Notification Failed: ID={n.MessageId}, Desc={e.Description}")
                            //;
                            LogManager.CurrentInstance.ErrorLogger.LogError(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                            "Gcm Failed: ID = " + n.MessageId + ", Desc = " + e.Message);
                        }

                    }
                    else if (ex is DeviceSubscriptionExpiredException)
                    {
                        var expiredException = (DeviceSubscriptionExpiredException)ex;

                        var oldId = expiredException.OldSubscriptionId;
                        var newId = expiredException.NewSubscriptionId;

                        //Console.WriteLine($
                        //"Device RegistrationId Expired: {oldId}")
                        //;

                        LogManager.CurrentInstance.ErrorLogger.LogError(
                           System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                           "Gcm Device RegistrationId Expired: oldId = " + oldId + ", newId = " + newId);

                        if (!string.IsNullOrWhiteSpace(newId))
                        {
                            // If this value isn't null, our subscription changed and we should update our database
                            //Console.WriteLine($
                            //"Device RegistrationId Changed To: {newId}")
                            //;
                        }
                    }
                    else if (ex is RetryAfterException)
                    {
                        var retryException = (RetryAfterException)ex;
                        // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                        //Console.WriteLine($
                        //"GCM Rate Limited, don't send more until after {retryException.RetryAfterUtc}")
                        //;
                        LogManager.CurrentInstance.ErrorLogger.LogError(
                           System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                           "GCM Rate Limited, don't send more until after " + retryException.RetryAfterUtc + " in UTC.");
                    }
                    else
                    {
                        //Console.WriteLine("GCM Notification Failed for some unknown reason");
                        LogManager.CurrentInstance.ErrorLogger.LogError(
                           System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                           "GCM Notification Failed for some unknown reason");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            _gcmServiceBroker.OnNotificationSucceeded += (notification) => LogManager.CurrentInstance.InfoLogger.LogInfo(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                            "Google Notification sent.\n\r notification: " + notification.Data); ;
        }

        public override void Start()
        {
            _gcmServiceBroker.Start();
        }
    }
}
