using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neeo.Notification.Factory;

namespace Neeo.Notification
{
    public class AppNotificationServices
    {
        public NotificationService ApnsService { get; set; }
        public NotificationService GcmService { get; set; }

        public AppNotificationServices()
        {
            var serviceFactory = new NotificationServiceFactory();
            ApnsService = serviceFactory.GetServiceInstance(PushNotificationType.Apns);
            GcmService = serviceFactory.GetServiceInstance(PushNotificationType.Gcm);
        }

        public void StartServices()
        {
            ApnsService.Start();
            GcmService.Start();
        }

        public void StopServices()
        {
            ApnsService.Stop();
            GcmService.Stop();
        }
    }
}
