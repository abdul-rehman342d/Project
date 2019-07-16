using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SyncApi.DTO
{

    /// <summary>
    /// contains the contact state.
    /// </summary>
    public class ContactStatusDTO
    {
        
        /// <summary>
        /// Specifies the contact phone number.
        /// </summary>
        [JsonProperty("ph")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Specifies the given contact is an application user or not.
        /// </summary>
        [JsonProperty("isUser")]
        public bool IsUser { get; set; }

        /// <summary>
        /// Specifies the given contact is already subscribed or not in the user's roster .
        /// </summary>
        [JsonProperty("isSub")]
        public int IsSubscribed { get; set; }

    }

}
