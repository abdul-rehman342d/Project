using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// A class that holds the contacts avatar time stamp.
    /// </summary>
    [DataContract]
    public class ContactAvatarTimestamp
    {
        /// <summary>
        /// Specifies the contact phone number.
        /// </summary>
        [DataMember]
        public string Ph { get; set; }

        /// <summary>
        /// Specifies the contact's avatar time stamp.
        /// </summary>
        [DataMember]
        public ulong Ts { get; set; }
    }
}
