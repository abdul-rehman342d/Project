using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerfulPal.Neeo.DashboardAPI.Models
{
    public class UpdateUserRequest
    {
        public string authKey { get; set; }

        [Required]
        public bool isActive { get; set; }

        [Required]
        public string userName { get; set; }
    }
}