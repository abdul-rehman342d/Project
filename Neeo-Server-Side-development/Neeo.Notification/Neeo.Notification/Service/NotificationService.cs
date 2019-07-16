using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using LibNeeo;

namespace Neeo.Notification
{
    public abstract class NotificationService
    {
        protected abstract void InitializeService();
        protected abstract void BindEvents();
        public abstract void Start();
        public abstract void Send(List<NeeoUser> receiverList, Model.NotificationModel notification);
        public abstract void Stop();
    }
}
