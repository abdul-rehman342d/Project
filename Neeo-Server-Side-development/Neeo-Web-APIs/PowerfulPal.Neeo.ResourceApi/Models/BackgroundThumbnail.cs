using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Common;

namespace PowerfulPal.Neeo.ResourceApi.Models
{
    public class BackgroundThumbnail : Resource
    {
        [Required]
        public string Filename { get; set; }

        public string ViewType { get; set; }

        public string Resolution { get; set; }
    }
}