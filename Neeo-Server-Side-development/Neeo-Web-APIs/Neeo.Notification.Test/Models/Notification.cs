using System.ComponentModel.DataAnnotations;
using Common;

namespace NeeoPushNotificationService.Notification.Test.Models
{
    public class Notification
    {
        [Required]
        [Range(1,5)]
        public NotificationType NType { get; set; }

        public string DToken { get; set; }
        [Range(1,3)]
        public DevicePlatform? Dp { get; set; }

        [Range(0, 1)]
        public PushNotificationSource? PnSource { get; set; }

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