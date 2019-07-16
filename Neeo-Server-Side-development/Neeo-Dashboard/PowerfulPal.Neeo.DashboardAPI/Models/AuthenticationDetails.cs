using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerfulPal.Neeo.DashboardAPI.Models
{
    public class AuthenticationDetails
    {
        public string Username { get; set; }
        public string AuthKey { get; set; }
        public string LastLoginTime { get; set; }
        public string LastSyncTime { get; set; }
    }
}