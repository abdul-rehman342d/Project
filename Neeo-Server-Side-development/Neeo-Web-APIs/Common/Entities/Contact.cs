using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// A class that holds the contact's avatar details.
    /// </summary>
    /// <remarks>This class holds the contact and its avatar time stamp information which will be used to check whether the avatar has been updated on the server or not.</remarks>
    public class Contact
    {
        #region Iphone 1.x.x, Android 1.x.x
        /*They will be deprecated in future.*/

        /// <summary>
        /// Specifies the contact phone number.
        /// </summary>
        public string ContactPhoneNumber { get; set; }
        /// <summary>
        /// Specifies the time stamp of the contacts avatar.
        /// </summary>
        public ulong AvatarTimeStamp { get; set; }

        #endregion

        #region Iphone 2.0.0, Android x.x.x

        /// <summary>
        /// Specifies the contact phone number.
        /// </summary>
        public string Ph { get; set; }

        #endregion

    }
}
