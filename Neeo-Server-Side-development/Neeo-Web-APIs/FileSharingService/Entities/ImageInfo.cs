using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FileSharingService
{
    /// <summary>
    /// A class that holds the image information that has to be sent.
    /// </summary>
    [DataContract]
    public class ImageInfo
    {
        /// <summary>
        /// Keeps the image url.
        /// </summary>
        [DataMember]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Keeps the image time stamp (i.e image creation date)
        /// </summary>
        [DataMember]
        public ulong TimeStamp { get; set; }
    }
}