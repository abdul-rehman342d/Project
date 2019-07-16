using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushSharp.Apple;
using PushSharp.Google;
using PushSharp.Windows;


namespace Neeo.Notification.Factory
{
    public class NotificationServiceFactory
    {
        public NotificationService GetServiceInstance(PushNotificationType pushNotificationType)
        {
            switch (pushNotificationType)
            {
                case PushNotificationType.Apns:
                    return new ApnsService();

                case PushNotificationType.Gcm:
                    return new GcmService();

                case PushNotificationType.WinPush:
                    return new WnsService();

                case PushNotificationType.Pushy:
                    return new ApnsService();
            }
            throw new NullReferenceException();
        }
    }
}
