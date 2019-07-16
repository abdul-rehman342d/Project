using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushSharp;
using PushSharp.Apple;
using PushSharp.Android;
using PushSharp.Core;
using System.IO;
using System.Web;
using Common;

namespace Notification
{
    public class NeeoNotification
    {
        private static PushBroker _push;
        //private devicePlatform type;

        public NeeoNotification()
        {
            if (_push == null)
            {
                _push = new PushBroker();

                _push.OnNotificationSent += NotificationSent;
                _push.OnChannelException += ChannelException;
                _push.OnServiceException += ServiceException;
                _push.OnNotificationFailed += NotificationFailed;
                _push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
                _push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
                _push.OnChannelCreated += ChannelCreated;
                _push.OnChannelDestroyed += ChannelDestroyed;
                var appleCertificate = File.ReadAllBytes("E:\\APNSDistCertificate.p12");
                _push.RegisterAppleService(new ApplePushChannelSettings(true, appleCertificate, "PowerfulP1234"));
                _push.RegisterGcmService(new GcmPushChannelSettings("YOUR Google API's Console API Access  API KEY for Server Apps HERE"));
            }
        }

        public void SendNotification(string deviceToken, string notificationMsg, int platform, int bageCount)
        {
            DevicePlatform type = (DevicePlatform)platform;
            if (type == DevicePlatform.iOS)
            {
                try
                {
                    //var appleCertificate = File.ReadAllBytes("E:\\APNSDistCertificate.p12");
                    //_push.RegisterAppleService(new ApplePushChannelSettings(true,appleCertificate, "PowerfulP1234"));
                    _push.QueueNotification(new AppleNotification(deviceToken)
                                                    .WithAlert(notificationMsg)
                                                    .WithBadge(bageCount)
                                                    .WithSound("default"));

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else if (type == DevicePlatform.Android)
            {
                _push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(deviceToken)
                 .WithJson("{\"alert\":\"" + notificationMsg + "\",\"badge\":" + bageCount + ",\"sound\":\"sound.caf\"}"));
            }
        }

        private void ChannelDestroyed(object sender)
        {
        }
        private void ChannelCreated(object sender, PushSharp.Core.IPushChannel pushChannel)
        {
        }
        private void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, PushSharp.Core.INotification notification)
        {
        }
        private void DeviceSubscriptionExpired(object sender, string expiredSubscriptionId, DateTime expirationDateUtc, PushSharp.Core.INotification notification)
        {
        }
        private void NotificationFailed(object sender, PushSharp.Core.INotification notification, Exception error)
        {
        }
        private void ServiceException(object sender, Exception error)
        {
        }
        private void ChannelException(object sender, PushSharp.Core.IPushChannel pushChannel, Exception error)
        {
        }
        private void NotificationSent(object sender, PushSharp.Core.INotification notification)
        {
        }
    }
}
