using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LibNeeo.IO
{
    public class FileDataStream
    {
        public Stream Stream { get; set; }
        public long Postion { get; set; }
    }
}