using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SettingsService.Models
{
    public class ResourceRequest : BaseRequest
    {
        [Required]
        [JsonProperty("Id")]
        public string ResourceId { get; set; }

        [Required]
        [Range(1,3)]
        [JsonProperty("dp")]
        public DevicePlatform DevicePlatform { get; set; }

        [Required]
        [JsonProperty("appId")]
        public string ApplicationId { get; set; }
    }
}