using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace LibNeeo.IO
{
    public class Video : File
    {
        public Video()
        {
            Info = new NeeoFileInfo() { MediaType = MediaType.Video };
        }
    }
}
