using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using LibNeeo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using Common;
using Common.Extension;
using Neeo.Notification;

namespace Neeo.Notification
{
    public class ApnsPayload : Payload
    {
        public override Dictionary<string, object> Create(NeeoUser receiver, Model.NotificationModel notificationModel)
        {
            var payload = new Dictionary<string, object>();
            var apsObject = new Dictionary<string, object>();

            payload.Add(StringConstants.MessageId, notificationModel.MsgId);

            payload.Add(ApnsStringConstant.Aps, apsObject);
            
            switch (notificationModel.NType)
            {
                case NotificationType.Im:

                    //payload = "{";
                    //payload += "\"aps\":{";
                    //payload += "\"alert\":\"" + notificationModel.Alert + "\",";
                    ////payload += "\"sound\":\"" + notificationModel.IMTone.GetDescription() + "\",";
                    //payload += "\"sound\":\"imTone1.m4r\",";
                    //payload += "\"badge\" : "+ notificationModel.Badge;
                    //payload += "},";
                    //payload += "\"" + StringConstants.NotificationID + "\" :\"" + NotificationType.Im.ToString("D") + "\",";
                    //payload += "\"" + StringConstants.SenderID + "\" :\"" + notificationModel.SenderID + "\"";
                    //payload += "}";

                    apsObject.Add(ApnsStringConstant.Alert, notificationModel.Alert);
                    apsObject.Add(ApnsStringConstant.Sound, receiver.ImTone.GetDescription());
                    apsObject.Add(ApnsStringConstant.Badge, receiver.OfflineMsgCount);

                    payload.Add(StringConstants.NotificationID, notificationModel.NType.ToString("D"));
                    payload.Add(StringConstants.SenderID, notificationModel.SenderID);
                    
         
                    break;

                case NotificationType.IncomingSipCall:

                    var alertObject = new Dictionary<string, object>();
                    alertObject.Add(ApnsStringConstant.Body, notificationModel.Alert);
                    alertObject.Add(ApnsStringConstant.ActionLocKey, ApnsAppConfiguration.ActionKeyText);

                    apsObject.Add(ApnsStringConstant.Alert, alertObject);
                    apsObject.Add(ApnsStringConstant.Sound, receiver.CallingTone.GetDescription());

                    payload.Add(StringConstants.NotificationID, notificationModel.NType.ToString("D"));
                    payload.Add(StringConstants.Timestamp, DateTime.UtcNow.ToString(NeeoConstants.TimestampFormat));


                    break;

                case NotificationType.Mcr:

                    apsObject.Add(ApnsStringConstant.Alert, notificationModel.Alert);
                    apsObject.Add(ApnsStringConstant.Sound, ApnsAppConfiguration.McrTone);
                    apsObject.Add(ApnsStringConstant.Badge, notificationModel.Badge);

                    payload.Add(StringConstants.NotificationID, notificationModel.NType.ToString("D"));
                    payload.Add(StringConstants.CallerID, notificationModel.CallerID);

                    break;

                case NotificationType.GIm:
                    apsObject.Add(ApnsStringConstant.Alert, notificationModel.Alert);
                    apsObject.Add(ApnsStringConstant.Sound, receiver.ImTone.GetDescription());
                    apsObject.Add(ApnsStringConstant.Badge, receiver.OfflineMsgCount + 1);

                    payload.Add(StringConstants.NotificationID, NotificationType.GIm.ToString("D"));
                    payload.Add(StringConstants.RoomID, notificationModel.RName);

                    break;

                case NotificationType.GInvite:

                    apsObject.Add(ApnsStringConstant.Alert, notificationModel.Alert);
                    apsObject.Add(ApnsStringConstant.Sound, receiver.ImTone.GetDescription());
                    apsObject.Add(ApnsStringConstant.Badge, receiver.OfflineMsgCount);

                    payload.Add(StringConstants.NotificationID, NotificationType.GIm.ToString("D"));
                    payload.Add(StringConstants.RoomID, notificationModel.RName);

                    break;

                case NotificationType.UpdateProfilePic:

                    apsObject.Add(ApnsStringConstant.ContentAvailable, 1);
                    //apsObject.Add(ApnsStringConstant.Alert, notificationModel.Alert);
                    payload.Add(StringConstants.NotificationID, notificationModel.NType.ToString("D"));
                    //payload.Add(StringConstants.NotificationID, NotificationType.Im.ToString("D"));
                    payload.Add(StringConstants.SenderID, notificationModel.SenderID);


                    break;
            }

            return payload;
        }

        
    }
}
