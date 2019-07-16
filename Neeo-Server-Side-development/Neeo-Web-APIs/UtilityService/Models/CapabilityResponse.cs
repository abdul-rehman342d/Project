using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace UtilityService.Models
{
    public class CapabilityResponse
    {
        [JsonPropertyAttribute("uid")]
        public string UID { get; set; }

        [JsonPropertyAttribute("cap")]
        public Dictionary<string, Capability> ContactsAppCapabilities { get; set; }
    }
}