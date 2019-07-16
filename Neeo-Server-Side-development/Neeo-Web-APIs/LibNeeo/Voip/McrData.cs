using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibNeeo.Voip
{
    /// <summary>
    /// A class that contains the user's missed class records detail.
    /// </summary>
    public class McrData
    {
        /// <summary>
        /// A string containing the user id.
        /// </summary>
        public string userID { get; set; }

        /// <summary>
        /// A list containing the missed class records.
        /// </summary>
        public List<MissedCallRecord> mcr { get; set; }
    }
}