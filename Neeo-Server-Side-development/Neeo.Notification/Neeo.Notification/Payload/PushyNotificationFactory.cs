using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using PushSharp.Apple;

namespace Neeo.Notification.Factory
{
    public class PushyNotificationFactory : Payload
    {
        public override object Create(Model.NotificationModel notificationModel)
        {
            var notification = new ApnsNotification();

            switch (notificationModel.NType)
            {
                case NotificationType.Im:
                    break;

                case NotificationType.IncomingSipCall:
                    break;

                case NotificationType.Mcr:
                    break;

                case NotificationType.GInvite:
                    break;

                case NotificationType.GIm:
                    break;
            }

            return notification;
        }
    }
}
