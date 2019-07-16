using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibNeeo.IO;

namespace LibNeeo.MediaSharing
{
    public class UploadSession
    {
        public string SessionID { get; set; }
        public NeeoFileInfo FileInfo { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public UploadSession()
        {
            
        }

        public UploadSession(NeeoFileInfo fileInfo)
        {
            FileInfo = fileInfo;
            CreationDate = DateTime.UtcNow;
        }
    }
}
