using System.ComponentModel.DataAnnotations;

namespace NeeoPushNotificationService.Notification.Test.Models
{
    public class ResetCountRequest
    {
        [Required]
        public string uID { get; set; }
    }
}