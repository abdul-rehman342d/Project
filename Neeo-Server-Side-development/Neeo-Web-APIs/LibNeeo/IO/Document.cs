using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace LibNeeo.IO
{
    public class Document : File
    {
        public Document()
        {
            Info = new NeeoFileInfo() { MediaType = MediaType.Document };
        }
    }
}
