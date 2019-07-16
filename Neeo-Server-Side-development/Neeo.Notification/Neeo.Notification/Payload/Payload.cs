using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibNeeo;
using Neeo.Notification.Model;
using PushSharp.Apple;
using PushSharp.Google;
using PushSharp.Windows;

namespace Neeo.Notification
{
    public abstract class Payload
    {
        public abstract Dictionary<string, object> Create(NeeoUser receiver, NotificationModel notification);
    }
}
