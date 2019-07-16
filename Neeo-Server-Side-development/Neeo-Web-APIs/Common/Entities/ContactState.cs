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
    public class ContactState
    {
        #region Iphone 2.0.0, Android x.x.x

        /// <summary>
        /// Specifies the contact phone number.
        /// </summary>
        [DataMember]
        public string Ph { get; set; }

        /// <summary>
        /// Specifies the given contact is an application user or not.
        /// </summary>
        [DataMember]
        public bool IsUsr { get; set; }

        /// <summary>
        /// Specifies the given contact is already subscribed or not in the user's roster .
        /// </summary>
        [DataMember]
        public int IsSub { get; set; }

        //[DataMember]
        //public ulong Ts { get; set; }

        #endregion
    }

}
