using System.ComponentModel.DataAnnotations;

namespace NeeoPushNotificationService.Models
{
    public class ResetCountRequest
    {
        [Required]
        public string uID { get; set; }
    }
}