using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Common.Entities;
using Newtonsoft.Json;

namespace PowerfulPal.Neeo.VoipApi.Models
{
    public class UserDTO : BaseRequest
    {
       
        [Required]
        [JsonProperty("user-list")]
        public List<string> UserList { get; set; } 
    }
}