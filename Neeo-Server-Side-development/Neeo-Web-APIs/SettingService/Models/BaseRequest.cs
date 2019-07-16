using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SettingsService.Models
{
    public class BaseRequest
    {
        [Required]
        [RegularExpression("^([0-9]+)(\\s)*$")]
        public string Uid { get; set; }
    }
}