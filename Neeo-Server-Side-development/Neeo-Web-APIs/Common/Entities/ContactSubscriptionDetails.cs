using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// contains the contact state.
    /// </summary>
    public class ContactSubscriptionDetails
    {
        /// <summary>
        /// Specifies the contact phone number.
        /// </summary>
        [DataMember]
        public string Ph { get; set; }

        /// <summary>
        /// Specifies the given contact is already subscribed or not in the user's roster .
        /// </summary>
        [DataMember]
        public int IsSub { get; set; }

        /// <summary>
        /// Specifies the avatar timestamp.
        /// </summary>
        [DataMember]
        public ulong Ts { get; set; }
    }
}
