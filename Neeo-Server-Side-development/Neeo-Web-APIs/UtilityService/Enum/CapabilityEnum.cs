using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UtilityService
{
    public enum AppCapabilities : ushort
    {
        /// <summary>
        /// VoIP Audio Calling
        /// </summary>
        Vac = 1 << 0,

        /// <summary>
        /// VoIP Video Calling
        /// </summary>
        Vvc = 1 << 1,

        /// <summary>
        /// Audio Sharing
        /// </summary>
        Ash = 1 << 2,

        /// <summary>
        /// Video Sharing
        /// </summary>
        Vsh = 1 << 3,

        /// <summary>
        /// Photo Sharing
        /// </summary>
        Psh = 1 << 4,

        /// <summary>
        /// Group Chat
        /// </summary>
        Gch = 1 << 5,

        /// <summary>
        /// Typing Presence
        /// </summary>
        Tp = 1 << 6,

        /// <summary>
        /// All Capabilities
        /// </summary>
        All = Vac | Vvc | Ash | Vsh | Psh | Gch | Tp

    }
}