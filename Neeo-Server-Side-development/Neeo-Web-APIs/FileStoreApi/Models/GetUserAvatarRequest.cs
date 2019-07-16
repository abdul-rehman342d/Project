using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace FileStoreApi.Models
{
    public class GetUserAvatarRequest : Common.Entities.BaseRequest
    {
        [JsonProperty(PropertyName = "ts")]
        public ulong Ts { get; set; }
        [JsonProperty(PropertyName = "dim")]
        public uint Dim { get; set; }
    }
}