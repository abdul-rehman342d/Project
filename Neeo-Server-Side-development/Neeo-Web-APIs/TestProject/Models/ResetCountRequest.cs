using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotificationService.Models
{
    public class ResetCountRequest
    {
        [Required]
        public string uID { get; set; }
    }
}