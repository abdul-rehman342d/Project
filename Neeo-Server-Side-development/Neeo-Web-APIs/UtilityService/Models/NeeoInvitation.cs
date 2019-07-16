using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Common.Entities;

namespace UtilityService.Models
{
    public class NeeoInvitation : BaseRequest
    {
        public string UName { get; set; }

        [Required]
        public string Contacts { get; set; }

        public string Lang { get; set; }
    }
}