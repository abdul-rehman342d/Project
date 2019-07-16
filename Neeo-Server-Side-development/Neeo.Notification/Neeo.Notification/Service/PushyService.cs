using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibNeeo;
using Neeo.Notification.Factory;
using Neeo.Notification.Model;
using PushSharp.Apple;

namespace Neeo.Notification
{
    public class PushyService : NotificationService
    {
        private ApnsServiceBroker _apnsServiceBroker;

        public PushyService()
        {
            InitializeService();
            BindEvents();
        }

        public override void Send(List<NeeoUser> receiverList, NotificationModel notificationModel)
        {
            if (receiverList == null || receiverList.Count == 0)
            {
                //log error
                return;
            }

            Parallel.ForEach(receiverList, (item) =>
            {
                var notification = (ApnsNotification)new ApnsPayload().Create(notificationModel);
                notification.DeviceToken = item.DeviceToken;
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
                                                    ApnsAppConfiguration.CertificatePwd);
            _apnsServiceBroker = new ApnsServiceBroker(apnsConfig);            
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
                        var notificationException = (ApnsNotificationException) ex;

                        // Deal with the failed notification
                        var apnsNotification = notificationException.Notification;
                        var statusCode = notificationException.ErrorStatusCode;

                       // Console.WriteLine($"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}");

                    }
                    else
                    {
                        // Inner exception might hold more useful information like an ApnsConnectionException           
                       // Console.WriteLine($"Apple Notification Failed for some unknown reason : {ex.InnerException}")
                        ;
                    }

                    // Mark it as handled
                    return true;
                });
            };


            _apnsServiceBroker.OnNotificationSucceeded += (notification) =>
            {
                //Console.WriteLine ("Apple Notification Sent!");
            };
        }

        public override void Start()
        {
            _apnsServiceBroker.Start();
        }
    }
}
