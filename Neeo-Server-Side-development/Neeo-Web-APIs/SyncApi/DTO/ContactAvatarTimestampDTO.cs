using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace SyncApi.DTO
{
    public class ContactAvatarTimestampDTO
    {
        /// <summary>
        /// Specifies the contact phone number.
        /// </summary>
        [JsonProperty("ph")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Specifies the avatar timestamp.
        /// </summary>
        [JsonProperty("avatarTs")]
        public ulong AvatarTimestamp { get; set; }
    }
}