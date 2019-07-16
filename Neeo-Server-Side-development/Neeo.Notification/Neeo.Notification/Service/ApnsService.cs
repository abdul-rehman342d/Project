using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibNeeo;
using Logger;
using Neeo.Notification.Factory;
using Neeo.Notification.Model;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;

namespace Neeo.Notification
{
    public class ApnsService : NotificationService
    {
        private ApnsServiceBroker _apnsServiceBroker;

        public ApnsService()
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
                Dictionary<string, object> payload;
                payload = new ApnsPayload().Create(item, notificationModel);

                var notification = new ApnsNotification(item.DeviceToken, JObject.FromObject(payload));

                _apnsServiceBroker.QueueNotification(notification);
            });

        }

        public override void Stop()
        {
            _apnsServiceBroker.Stop();
        }

        protected override void InitializeService()
        {
            var apnsConfig = new ApnsConfiguration(
                                                    ApnsAppConfiguration.CertificateType == "production" ? ApnsConfiguration.ApnsServerEnvironment.Production : ApnsConfiguration.ApnsServerEnvironment.Sandbox,
                                                    ApnsAppConfiguration.CertificatePath,
                                                    ApnsAppConfiguration.CertificatePwd,
                                                    validateIsApnsCertificate: false
            );
            _apnsServiceBroker = new ApnsServiceBroker(apnsConfig);
            _apnsServiceBroker.ChangeScale(10);
        }

        protected override void BindEvents()
        {
            _apnsServiceBroker.OnNotificationFailed += (notification, aggregateEx) =>
            {

                aggregateEx.Handle(ex =>
                {

                    // See what kind of exception it was to further diagnose
                    if (ex is ApnsNotificationException)
                    {
                        var notificationException = (ApnsNotificationException)ex;

                        // Deal with the failed notification
                        var apnsNotification = notificationException.Notification;
                        var statusCode = notificationException.ErrorStatusCode;

                        // Console.WriteLine($"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}");
                        LogManager.CurrentInstance.ErrorLogger.LogError(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                            "Apple Notification Failed: ID = "+ apnsNotification.Identifier +", Code = "+ statusCode);

                    }
                    else
                    {
                        // Inner exception might hold more useful information like an ApnsConnectionException           
                        // Console.WriteLine($"Apple Notification Failed for some unknown reason : {ex.InnerException}")
                        LogManager.CurrentInstance.ErrorLogger.LogError(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                            "Apple Notification Failed, Reason : "+ ex.InnerException);
                    }

                    // Mark it as handled
                    return true;
                });
            };


            _apnsServiceBroker.OnNotificationSucceeded += (notification) =>
            {
                LogManager.CurrentInstance.InfoLogger.LogInfo(
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                            "Apple Notification sent.\n\r notification: " + notification.Payload);
            };
        }

        public override void Start()
        {
            _apnsServiceBroker.Start();
        }
    }
}
