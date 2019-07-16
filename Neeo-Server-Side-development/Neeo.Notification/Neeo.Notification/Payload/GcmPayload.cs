using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Extension;
using LibNeeo;
using PushSharp.Apple;
using PushSharp.Google;

namespace Neeo.Notification
{
    public class GcmPayload : Payload
    {
        public override Dictionary<string, object> Create(NeeoUser receiver, Model.NotificationModel notificationModel)
        {
            var payload = new Dictionary<string, object>();

            payload.Add(StringConstants.MessageId, notificationModel.MsgId);

            switch (notificationModel.NType)
            {
                case NotificationType.Im:

                    payload.Add(GcmStringConstant.Alert, notificationModel.Alert);
                    payload.Add(StringConstants.NotificationID, notificationModel.NType.ToString("D"));
                    payload.Add(StringConstants.SenderID, notificationModel.SenderID);

                    break;

                case NotificationType.IncomingSipCall:

                    payload.Add(GcmStringConstant.Alert, notificationModel.Alert);
                    payload.Add(StringConstants.NotificationID, notificationModel.NType.ToString("D"));
                    payload.Add(StringConstants.Timestamp, DateTime.UtcNow.ToString(NeeoConstants.TimestampFormat));
                    payload.Add(StringConstants.CallerID, notificationModel.CallerID);

                    break;

                case NotificationType.Mcr:

                    payload.Add(GcmStringConstant.Alert, notificationModel.Alert);
                    payload.Add(StringConstants.NotificationID, notificationModel.NType.ToString("D"));
                    payload.Add(StringConstants.CallerID, notificationModel.CallerID);

                    break;

                case NotificationType.GIm:

                    payload.Add(GcmStringConstant.Alert, notificationModel.Alert);
                    payload.Add(StringConstants.NotificationID, notificationModel.NType.ToString("D"));
                    payload.Add(StringConstants.RoomID, notificationModel.RName);

                    break;

                case NotificationType.GInvite:

                    payload.Add(GcmStringConstant.Alert, notificationModel.Alert);
                    payload.Add(StringConstants.NotificationID, NotificationType.GIm.ToString("D"));
                    payload.Add(StringConstants.RoomID, notificationModel.RName);

                    break;

                case NotificationType.UpdateProfilePic:

                    payload.Add(GcmStringConstant.Alert, notificationModel.Alert);
                    payload.Add(StringConstants.NotificationID, notificationModel.NType.ToString("D"));
                    //payload.Add(StringConstants.NotificationID, NotificationType.Im.ToString("D"));
                    payload.Add(StringConstants.SenderID, notificationModel.SenderID);

                    break;
            }

            return payload;
        }
    }
}
