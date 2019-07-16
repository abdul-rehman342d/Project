using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNeeo.Voip
{
    /// <summary>
    /// A class containing the missed call record details
    /// </summary>
    public class MissedCallRecord
    {
        /// <summary>
        /// A string containing the the missed call caller id.
        /// </summary>
        public string callerID { get; set; }

        /// <summary>
        /// A string containing the missed call time.
        /// </summary>
        public string time { get; set; }

    }
}
