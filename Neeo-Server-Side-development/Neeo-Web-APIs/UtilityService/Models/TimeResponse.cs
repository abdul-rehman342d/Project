﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace UtilityService.Models
{
    public class TimeResponse
    {
        [JsonPropertyAttribute("response")]
        public string Time { get; set; }
    }
}