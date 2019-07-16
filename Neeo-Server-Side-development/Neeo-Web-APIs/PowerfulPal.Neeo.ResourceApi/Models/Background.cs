using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Common;

namespace PowerfulPal.Neeo.ResourceApi.Models
{
    public class Background : Resource
    {
        [Required]
        public string Filename { get; set; }

        [Required]
        public string ViewType { get; set; }

        [Required]
        public string Resolution { get; set; }
    }
}