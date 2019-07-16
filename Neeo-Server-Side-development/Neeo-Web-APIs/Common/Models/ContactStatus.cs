using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class ContactStatus
    {
        public Contact Contact { get; set; }

        /// <summary>
        /// Specifies the given contact is an application user or not.
        /// </summary>
        public bool IsNeeoUser { get; set; }
        /// <summary>
        /// Specifies the given contact is already subscribed or not in the user's roster .
        /// </summary>
        public int IsAlreadySubscribed { get; set; }

        /// <summary>
        /// Specifies the avatar timestamp.
        /// </summary>
        public ulong AvatarTimestamp { get; set; }
    }
}
