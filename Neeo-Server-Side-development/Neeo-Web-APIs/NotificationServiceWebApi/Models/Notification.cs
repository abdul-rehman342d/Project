using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Web;
using Common;


namespace NotificationService
{
    public class Notification
    {
        [Required]
        [Range(1,5)]
        public NotificationType NType { get; set; }

        public string DToken { get; set; }
        [Range(1,2)]
        public DevicePlatform? Dp { get; set; }

        public string Alert { get; set; }

        public int Badge { get; set; }

        public string ReceiverID { get; set; }

        public string CallerID { get; set; }

        public string SenderID { get; set; }

        public int McrCount { get; set; }

        [Range(1,9)]
        public IMTone? IMTone { get; set; }

        public int RID { get; set; }

        public string RName { get; set; }

        [Range(1, 5)]
        public int? MessageType { get; set; }

    }
}