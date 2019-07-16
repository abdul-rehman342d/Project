using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;

namespace SyncApi.Models
{
    public class ContactSyncRequest
    {
        public string userID;
        public string uID;
        public Common.Contact[] contacts { get; set; }
        public bool? all { get; set; }    
    }
}