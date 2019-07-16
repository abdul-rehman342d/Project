using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Common.Entities;

namespace UtilityService.Models
{
    public class CapabilityRequest: BaseRequest
    {
        [Required]
        public string[] Contacts { get; set; }

        [Required]
        public ushort Cap{ get; set; }
    }
}