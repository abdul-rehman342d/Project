using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PowerfulPal.Neeo.SpamApi.Models
{
    public class TranslateModel
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public string Source { get; set; }

        [Required]
        public string Target { get; set; }
    }
}