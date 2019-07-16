using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PowerfulPal.Neeo.NearByMeApi.Models
{
    public class NearByMeSettingModel
    {
        public bool Enabled { get; set; }
        public ushort NotificationTone { get; set; }
        public bool NotificationOn { get; set; }
        public bool ShowInfo { get; set; }
        public bool ShowProfileImage { get; set; }
        public bool IsPrivateAccount { get; set; }
    }
}