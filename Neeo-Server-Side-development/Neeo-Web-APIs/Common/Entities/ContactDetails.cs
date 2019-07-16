using System.Runtime.Serialization;

namespace Common
{
    /// <summary>
    /// A class that holds the contact details.
    /// </summary>
    [DataContract]
    public class ContactDetails
    {
        #region Iphone 1.x.x, Android 1.x.x
        /*They will be deprecated in future.*/

        /// <summary>
        /// Specifies the contact phone number.
        /// </summary>
        [DataMember]
        public string ContactPhoneNumber { get; set; }

        /// <summary>
        /// Specifies the given contact is an application user or not.
        /// </summary>
        [DataMember]
        public bool IsNeeoUser { get; set; }
        /// <summary>
        /// Specifies the given contact is already subscribed or not in the user's roster .
        /// </summary>
        [DataMember]
        public int IsAlreadySubscribed { get; set; }
        /// <summary>
        /// Specifies the avatar state of the given contact.
        /// </summary>
        [DataMember]
        public ushort AvatarState { get; set; }

        /// <summary>
        /// Specifies the avatar timestamp.
        /// </summary>
        [DataMember]
        public ulong AvatarTimestamp { get; set; }

        #endregion
    }
}
