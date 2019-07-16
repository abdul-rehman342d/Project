using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileStoreApp.Models
{
    public class AvatarRequest : Common.Entities.BaseRequest
    {
        public ulong Timestamp { get; set; }
        public uint Dimension { get; set; }
    }
}