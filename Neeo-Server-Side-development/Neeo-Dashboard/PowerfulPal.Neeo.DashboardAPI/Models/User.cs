using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using Newtonsoft.Json;

namespace PowerfulPal.Neeo.DashboardAPI.Models
{
    public class User
    {
        public int UID { get; set; }
        [Required]
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }

        [JsonIgnore]
        private bool SerializePassword
        {
            get
            {
                return false;
            }
        }

        public bool ShouldSerializePassword()
        {
            return SerializePassword;
        }
    }
}