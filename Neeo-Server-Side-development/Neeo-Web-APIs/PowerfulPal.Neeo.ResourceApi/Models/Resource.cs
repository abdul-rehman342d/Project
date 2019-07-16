using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Common;
using Newtonsoft.Json;

namespace PowerfulPal.Neeo.ResourceApi.Models
{
    public abstract class Resource
    {
        [Required]
        public string ResourceId { get; set; }

        //[Required]
        //[Range(1, 3)]
        public DevicePlatform DevicePlatform { get; set; }

        //[Required]
        //[Range(1, 2)]
        public ResourceType ResourceType { get; set; }
    }
}