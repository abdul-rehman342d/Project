using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neeo.Notification
{
    public enum PushNotificationType
    {
        Apns,
        Gcm,
        WinPush,
        Pushy
    }

    public enum NotificationType : short
    {
        /// <summary>
        /// Instant Message
        /// </summary>
        Im = 1,

        /// <summary>
        /// Incoming Sip Call
        /// </summary>
        IncomingSipCall = 2,

        /// <summary>
        /// Missed Call Record
        /// </summary>
        Mcr = 3,

        /// <summary>
        /// Group Instant Message
        /// </summary>
        GIm = 4,

        /// <summary>
        /// Group Invitation
        /// </summary>
        GInvite = 5,

        /// <summary>
        /// Update profile picture of the user
        /// </summary>
        UpdateProfilePic = 6
    }
}
